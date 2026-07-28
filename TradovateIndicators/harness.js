'use strict';

// ---------------------------------------------------------------------------
// harness.js — does the key level scorer actually sort outcomes?
//
//   node harness.js <bars.csv> [--horizon 12] [--stop 8] [--rr 2] [--tick 0.25]
//
// CSV needs a header row and open/high/low/close columns. Timestamp optional
// but needed for the session breakdown. Column names are matched loosely, so
// TradingView and Tradovate exports both work as-is.
//
// The scorer sees bars 0..i. The labeller sees bars i+1..i+H. Entry is the
// CLOSE of the signal bar — the only price you are guaranteed to be able to
// act on when the signal appears. Entering at the level itself would assume a
// limit fill that may never come, which is the commonest way a harness lies.
//
// TWO TRAPS THIS CODE IS BUILT TO AVOID, both caught by running it on random
// data before running it on anything real:
//
//   1. Raw survival rate is contaminated by distance. A level 2 ATR from the
//      close survives more often than one 0.2 ATR away on ANY data, random
//      included, purely because the barrier is further off. So we compute the
//      first-passage survival a driftless random walk would give that exact
//      level, and score the model on EXCESS over it. Excess is the skill.
//
//   2. Risk must be measured from the entry to the stop, not from the level to
//      the stop. The stop sits beyond the level, the entry sits back at the
//      close, so true risk is larger than the buffer and using the buffer
//      understates every loss.
// ---------------------------------------------------------------------------

const fs = require('fs');
const path = require('path');
const { scoreAll } = require('./scoring');

function parseArgs(argv) {
    const a = { horizon: 12, stop: 8, rr: 2, tick: 0.25, costTicks: 1.5, file: null };
    for (let i = 2; i < argv.length; i++) {
        const t = argv[i];
        if (t === '--horizon') a.horizon = Number(argv[++i]);
        else if (t === '--stop') a.stop = Number(argv[++i]);
        else if (t === '--rr') a.rr = Number(argv[++i]);
        else if (t === '--tick') a.tick = Number(argv[++i]);
        else if (t === '--cost') a.costTicks = Number(argv[++i]);
        else if (!t.startsWith('--')) a.file = t;
    }
    return a;
}

// ---------------------------------------------------------------------------
// CSV
// ---------------------------------------------------------------------------
function loadBars(file) {
    const text = fs.readFileSync(file, 'utf8').trim();
    const lines = text.split(/\r?\n/).filter(l => l.trim().length);
    if (lines.length < 2) throw new Error('CSV has no data rows');

    const delim = lines[0].indexOf('\t') >= 0 ? '\t' : ',';
    const header = lines[0].split(delim).map(h => h.trim().toLowerCase().replace(/^"|"$/g, ''));

    const find = (...names) => {
        for (const nm of names) { const i = header.findIndex(h => h === nm); if (i >= 0) return i; }
        for (const nm of names) { const i = header.findIndex(h => h.includes(nm)); if (i >= 0) return i; }
        return -1;
    };

    const iO = find('open', 'o'), iH = find('high', 'h');
    const iL = find('low', 'l'), iC = find('close', 'c', 'last');
    const iT = find('timestamp', 'time', 'date', 'datetime');
    if (iO < 0 || iH < 0 || iL < 0 || iC < 0) {
        throw new Error('no open/high/low/close columns in: ' + header.join(', '));
    }

    const bars = [];
    for (let r = 1; r < lines.length; r++) {
        const f = lines[r].split(delim);
        const o = Number(f[iO]), h = Number(f[iH]), l = Number(f[iL]), c = Number(f[iC]);
        if (![o, h, l, c].every(Number.isFinite) || h < l) continue;
        let ts = null;
        if (iT >= 0) {
            const raw = (f[iT] || '').trim().replace(/^"|"$/g, '');
            const asNum = Number(raw);
            if (Number.isFinite(asNum) && raw.length >= 9) ts = new Date(asNum > 1e12 ? asNum : asNum * 1000);
            else { const d = new Date(raw); if (!isNaN(d.getTime())) ts = d; }
        }
        bars.push({ open: o, high: h, low: l, close: c, ts });
    }
    return bars;
}

// ---------------------------------------------------------------------------
// first-passage reference model
// ---------------------------------------------------------------------------
function normCdf(z) {
    const t = 1 / (1 + 0.2316419 * Math.abs(z));
    const d = 0.3989422804014327 * Math.exp(-z * z / 2);
    const p = d * t * (0.319381530 + t * (-0.356563782 + t * (1.781477937 +
              t * (-1.821255978 + t * 1.330274429))));
    return z > 0 ? 1 - p : p;
}

// P(a driftless walk starting at the close never touches a barrier `d` away
// within H bars) = 2*Phi(d / (sigma*sqrt(H))) - 1
function theorySurvival(d, sigma, H) {
    if (!(sigma > 0) || !(H > 0)) return 0;
    if (!(d > 0)) return 0;
    const z = d / (sigma * Math.sqrt(H));
    const p = 2 * normCdf(z) - 1;
    return p < 0 ? 0 : (p > 1 ? 1 : p);
}

// rolling per-bar close-to-close volatility — the sigma the model above needs
function rollingSigma(bars, win) {
    const n = bars.length;
    const out = new Float64Array(n);
    const d = new Float64Array(n);
    for (let i = 1; i < n; i++) d[i] = bars[i].close - bars[i - 1].close;
    let s = 0, s2 = 0, cnt = 0;
    for (let i = 1; i < n; i++) {
        s += d[i]; s2 += d[i] * d[i]; cnt++;
        if (cnt > win) { const o = d[i - win]; s -= o; s2 -= o * o; cnt--; }
        const m = s / cnt;
        const v = Math.max(0, s2 / cnt - m * m);
        out[i] = Math.sqrt(v);
    }
    return out;
}

// ---------------------------------------------------------------------------
// labelling
// ---------------------------------------------------------------------------
function survived(bars, i, side, level, H) {
    const end = Math.min(bars.length - 1, i + H);
    for (let j = i + 1; j <= end; j++) {
        if (side === 1 ? bars[j].high > level : bars[j].low < level) return false;
    }
    return true;
}

function label(bars, sigma, cand, args) {
    const i = cand.index, H = args.horizon;
    if (i + H >= bars.length) return null;

    const entry = cand.close;
    const buffer = args.stop * args.tick;
    const stopPx = cand.side === 1 ? (cand.level + buffer) : (cand.level - buffer);
    const risk = Math.abs(entry - stopPx);                 // TRUE risk, entry to stop
    if (!(risk > 0)) return null;
    const targetPx = cand.side === 1 ? (entry - args.rr * risk) : (entry + args.rr * risk);
    const costR = (args.costTicks * args.tick) / risk;

    let maxHigh = -Infinity, minLow = Infinity;
    for (let j = i + 1; j <= i + H; j++) {
        if (bars[j].high > maxHigh) maxHigh = bars[j].high;
        if (bars[j].low < minLow) minLow = bars[j].low;
    }

    // first touch wins; ties resolve as a loss, which is the pessimistic and
    // correct assumption when you cannot see inside the bar
    let r = null, outcome = 'timeout', held = H;
    for (let j = i + 1; j <= i + H; j++) {
        const b = bars[j];
        const hitStop = cand.side === 1 ? b.high >= stopPx : b.low <= stopPx;
        const hitTgt = cand.side === 1 ? b.low <= targetPx : b.high >= targetPx;
        if (hitStop) { r = -1; outcome = 'stop'; held = j - i; break; }
        if (hitTgt) { r = args.rr; outcome = 'target'; held = j - i; break; }
    }
    if (r === null) {
        const exit = bars[i + H].close;
        r = (cand.side === 1 ? (entry - exit) : (exit - entry)) / risk;
    }

    const dist = Math.abs(cand.level - entry);
    const pTheory = theorySurvival(dist, sigma[i], H);

    return {
        survived: survived(bars, i, cand.side, cand.level, H),
        pTheory,
        mfe: (cand.side === 1 ? (entry - minLow) : (maxHigh - entry)) / cand.atr,
        mae: (cand.side === 1 ? (maxHigh - entry) : (entry - minLow)) / cand.atr,
        r: r - costR,
        outcome, holdBars: held
    };
}

// ---------------------------------------------------------------------------
// stats
// ---------------------------------------------------------------------------
const mean = xs => xs.length ? xs.reduce((a, b) => a + b, 0) / xs.length : 0;
const se = (p, n) => n > 0 ? Math.sqrt(p * (1 - p) / n) : 0;

function pad(s, w, right) { s = String(s); return right ? s.padStart(w) : s.padEnd(w); }

function table(title, rows, cols) {
    console.log('\n' + title);
    console.log('-'.repeat(cols.reduce((a, c) => a + c.w + 2, 0)));
    console.log(cols.map(c => pad(c.h, c.w, c.r)).join('  '));
    rows.forEach(row => console.log(cols.map(c => pad(c.f(row), c.w, c.r)).join('  ')));
}

function summarise(name, items) {
    const n = items.length;
    if (!n) return null;
    const surv = items.filter(x => x.lab.survived).length / n;
    const theory = mean(items.map(x => x.lab.pTheory));
    const rs = items.map(x => x.lab.r);
    const rMean = mean(rs);
    const rSd = Math.sqrt(mean(rs.map(r => (r - rMean) * (r - rMean))));
    return {
        name, n, surv, theory,
        excess: surv - theory,
        excessSE: se(surv, n),
        mfe: mean(items.map(x => x.lab.mfe)),
        mae: mean(items.map(x => x.lab.mae)),
        win: items.filter(x => x.lab.r > 0).length / n,
        avgR: rMean,
        tstat: rSd > 0 ? rMean / (rSd / Math.sqrt(n)) : 0
    };
}

const COLS = [
    { h: 'bucket', w: 16, f: r => r.name },
    { h: 'n', w: 6, r: 1, f: r => r.n },
    { h: 'surv%', w: 7, r: 1, f: r => (r.surv * 100).toFixed(1) },
    { h: 'theory%', w: 8, r: 1, f: r => (r.theory * 100).toFixed(1) },
    { h: 'EXCESS', w: 8, r: 1, f: r => ((r.excess >= 0 ? '+' : '') + (r.excess * 100).toFixed(1)) },
    { h: '+/-2se', w: 8, r: 1, f: r => (r.excessSE * 200).toFixed(1) },
    { h: 'MFE', w: 6, r: 1, f: r => r.mfe.toFixed(2) },
    { h: 'MAE', w: 6, r: 1, f: r => r.mae.toFixed(2) },
    { h: 'win%', w: 6, r: 1, f: r => (r.win * 100).toFixed(1) },
    { h: 'avgR', w: 7, r: 1, f: r => r.avgR.toFixed(3) },
    { h: 't', w: 6, r: 1, f: r => r.tstat.toFixed(2) }
];

// ---------------------------------------------------------------------------
function main() {
    const args = parseArgs(process.argv);
    if (!args.file) {
        console.error('usage: node harness.js <bars.csv> [--horizon 12] [--stop 8] [--rr 2] [--tick 0.25] [--cost 1.5]');
        process.exit(1);
    }

    const bars = loadBars(args.file);
    const sigma = rollingSigma(bars, 100);

    console.log('\nloaded ' + bars.length + ' bars from ' + path.basename(args.file));
    if (bars[0].ts && bars[bars.length - 1].ts) {
        console.log('range  ' + bars[0].ts.toISOString() + '  ->  ' + bars[bars.length - 1].ts.toISOString());
    }
    console.log('horizon ' + args.horizon + ' bars | stop ' + args.stop + ' ticks beyond level | target '
        + args.rr + 'R | cost ' + args.costTicks + ' ticks');

    let baseHold = 0, baseN = 0;
    for (let i = 1; i < bars.length - args.horizon; i++) {
        baseN += 2;
        if (survived(bars, i, 1, bars[i].high, args.horizon)) baseHold++;
        if (survived(bars, i, -1, bars[i].low, args.horizon)) baseHold++;
    }

    const cands = scoreAll(bars, { tickSize: args.tick });
    const items = [];
    for (const c of cands) {
        const lab = label(bars, sigma, c, args);
        if (lab) items.push({ c, lab });
    }
    if (!items.length) { console.error('no labelled signals'); process.exit(1); }

    const all = summarise('ALL', items);
    console.log('\nbaseline   arbitrary bar extreme holds ' + args.horizon + ' bars: '
        + (100 * baseHold / baseN).toFixed(1) + '%  (n=' + baseN + ')');
    console.log('gated      candidate level holds ' + args.horizon + ' bars: '
        + (all.surv * 100).toFixed(1) + '%   random-walk model says '
        + (all.theory * 100).toFixed(1) + '%   excess '
        + (all.excess >= 0 ? '+' : '') + (all.excess * 100).toFixed(1) + 'pp');
    console.log('gates fire on ' + (100 * items.length / (2 * bars.length)).toFixed(1) + '% of bar-sides');

    const sorted = items.slice().sort((a, b) => a.c.score - b.c.score);
    const buckets = [];
    for (let k = 0; k < 5; k++) {
        const lo = Math.floor(k * sorted.length / 5), hi = Math.floor((k + 1) * sorted.length / 5);
        const slice = sorted.slice(lo, hi);
        if (slice.length) {
            buckets.push(summarise('Q' + (k + 1) + ' ' + slice[0].c.score.toFixed(2)
                + '-' + slice[slice.length - 1].c.score.toFixed(2), slice));
        }
    }
    table('SCORE QUINTILES   <- EXCESS is the real test. surv% climbing is mostly distance, not skill.',
        buckets, COLS);

    const sweep = [];
    for (let t = 0.40; t <= 0.85001; t += 0.05) {
        const s = summarise('>= ' + t.toFixed(2), items.filter(x => x.c.score >= t));
        if (s && s.n >= 10) sweep.push(s);
    }
    table('THRESHOLD SWEEP', sweep, COLS);

    table('BY SIDE', [
        summarise('upper', items.filter(x => x.c.side === 1)),
        summarise('lower', items.filter(x => x.c.side === -1))
    ].filter(Boolean), COLS);

    table('BY REGIME', [
        summarise('ER<0.25 chop', items.filter(x => x.c.er < 0.25)),
        summarise('ER 0.25-0.50', items.filter(x => x.c.er >= 0.25 && x.c.er < 0.50)),
        summarise('ER>=0.50 trend', items.filter(x => x.c.er >= 0.50))
    ].filter(Boolean), COLS);

    if (bars.some(b => b.ts)) {
        const byHour = [];
        for (let h = 0; h < 24; h++) {
            const s = summarise('UTC ' + String(h).padStart(2, '0'),
                items.filter(x => bars[x.c.index].ts && bars[x.c.index].ts.getUTCHours() === h));
            if (s && s.n >= 25) byHour.push(s);
        }
        if (byHour.length) table('BY HOUR (UTC)', byHour, COLS);
    }

    console.log('\nSUB-SCORE vs EXCESS SURVIVAL   <- which factors earn their weight');
    for (const k of ['prom', 'rej', 'dist', 'exp', 'reg', 'conf']) {
        const srt = items.slice().sort((a, b) => a.c.parts[k] - b.c.parts[k]);
        const lo = srt.slice(0, Math.floor(srt.length / 3));
        const hi = srt.slice(-Math.floor(srt.length / 3));
        const sl = summarise('lo', lo), sh = summarise('hi', hi);
        const d = sh.excess - sl.excess;
        console.log('  ' + pad(k, 6) + ' low-third excess ' + pad((sl.excess * 100).toFixed(1), 6, 1)
            + 'pp   high-third ' + pad((sh.excess * 100).toFixed(1), 6, 1)
            + 'pp   spread ' + pad(((d >= 0 ? '+' : '') + (d * 100).toFixed(1)), 7, 1) + 'pp  '
            + (d > 0 ? '#'.repeat(Math.min(40, Math.round(d * 200))) : ''));
    }

    const outPath = path.join(path.dirname(args.file), 'signals.csv');
    const head = 'index,side,score,level,close,atr,er,prom,rej,dist,exp,reg,conf,'
        + 'survived,p_theory,excess,mfe_atr,mae_atr,r,outcome,hold_bars\n';
    fs.writeFileSync(outPath, head + items.map(x => [
        x.c.index, x.c.side, x.c.score.toFixed(4), x.c.level, x.c.close,
        x.c.atr.toFixed(3), x.c.er.toFixed(3),
        x.c.parts.prom.toFixed(3), x.c.parts.rej.toFixed(3), x.c.parts.dist.toFixed(3),
        x.c.parts.exp.toFixed(3), x.c.parts.reg.toFixed(3), x.c.parts.conf.toFixed(3),
        x.lab.survived ? 1 : 0, x.lab.pTheory.toFixed(4),
        ((x.lab.survived ? 1 : 0) - x.lab.pTheory).toFixed(4),
        x.lab.mfe.toFixed(3), x.lab.mae.toFixed(3),
        x.lab.r.toFixed(3), x.lab.outcome, x.lab.holdBars
    ].join(',')).join('\n') + '\n');
    console.log('\nwrote ' + items.length + ' labelled signals -> ' + outPath + '\n');
}

main();
