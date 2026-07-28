const predef = require("./tools/predef");
const meta = require("./tools/meta");
const p = require("./tools/plotting");

// ---------------------------------------------------------------------------
// keyLevelSignals
//
// Entry signals from sweep-and-reclaim on scored key levels.
// Price pokes THROUGH a level and closes back inside -> the move beyond ran
// stops and failed, so fade it.
//
//   upper level swept  ->  SHORT        lower level swept  ->  LONG
//
// Entry, stop and target are all fixed before the signal prints.
//
// Strictly causal: a level created by bar j cannot be swept before bar j+1,
// and a signal on bar k uses nothing after bar k.
//
// PERFORMANCE NOTES (this plotter runs on every repaint, so it has to be cheap)
//   1. Analysis is windowed to the last `lookbackBars` bars plus warm-up. A
//      level cannot outlive maxLevelAgeBars, so older bars cannot affect the
//      result and there is no reason to touch them.
//   2. Confluence uses a sliding bucket map keyed on tick, not a linear scan
//      over every prior pivot. That was ~1.2M operations per repaint.
//   3. Results are cached against the data and the numeric params, so opening
//      the parameter dialog does not recompute anything.
// ---------------------------------------------------------------------------

const PROM_NORM_ATR = 0.5;
const REJ_NORM      = 0.6;
const DIST_NORM_ATR = 0.75;
const EXP_NORM_ATR  = 1.4;
const ER_NORM       = 0.7;
const CONF_NORM     = 3;

const W = { prom: 25, rej: 20, dist: 20, exp: 10, reg: 15, conf: 10 };
const W_SUM = W.prom + W.rej + W.dist + W.exp + W.reg + W.conf;

const CONFLUENCE_LOOKBACK = 300;

function clamp01(v) {
    if (!(v > 0)) return 0;
    return v > 1 ? 1 : v;
}

class keyLevelSignals {
    map(d) {
        return { open: d.open(), high: d.high(), low: d.low(), close: d.close() };
    }
}

// ---------------------------------------------------------------------------
// cache
// ---------------------------------------------------------------------------
let CACHE = { key: null, signals: null };

function cacheKey(props, series, n) {
    const last = series.get(n - 1);
    const prev = n > 1 ? series.get(n - 2) : last;
    return n + ':' + last.close + ':' + last.high + ':' + last.low + ':' + prev.close + ':' +
        props.swingLookback + ',' + props.atrPeriod + ',' + props.erPeriod + ',' +
        props.minRejection + ',' + props.confluenceTicks + ',' + props.levelThreshold + ',' +
        props.maxLevelAgeBars + ',' + props.maxActiveLevels + ',' + props.minSweepTicks + ',' +
        props.maxSweepAtr + ',' + props.reclaimTicks + ',' + props.allowNextBarReclaim + ',' +
        props.stopBufferTicks + ',' + props.minRR + ',' + props.fallbackRR + ',' +
        props.maxRiskAtr + ',' + props.maxEr + ',' + props.lookbackBars + ',' +
        props.maxRR + ',' + props.trendPeriod + ',' + props.maxAdverseAtr + ',' +
        props.cooldownBars + ',' + props.dedupeAtr;
}

// ---------------------------------------------------------------------------
// analysis — one windowed pass, no per-bar allocation
// ---------------------------------------------------------------------------
function analyse(instance, series) {
    const props = instance.props;
    const n = series.data.length;
    const signals = [];
    if (n < 30) return signals;

    const tickSize = (instance.contractInfo && instance.contractInfo.tickSize) || 0.25;
    const invTick = 1 / tickSize;

    const swingLookback = Math.max(2, Math.round(props.swingLookback));
    const atrPeriod     = Math.max(2, Math.round(props.atrPeriod));
    const erPeriod      = Math.max(2, Math.round(props.erPeriod));
    const maxAge        = Math.max(5, Math.round(props.maxLevelAgeBars));
    const maxActive     = Math.max(1, Math.round(props.maxActiveLevels));
    const conflTicks    = Math.max(0, Math.round(props.confluenceTicks));

    // ---- window ----
    // Data window: generous, so confluence memory is well stocked.
    const pad = Math.max(atrPeriod, erPeriod, swingLookback, CONFLUENCE_LOOKBACK, maxAge) + 10;
    const want = Math.max(50, Math.round(props.lookbackBars));
    const base = Math.max(0, n - want - pad);
    const m = n - base;                       // analysed bar count

    // Emission warm-up: only what the maths genuinely needs. Gating this on
    // CONFLUENCE_LOOKBACK swallowed the entire chart when fewer than ~350 bars
    // were loaded, which is most charts.
    const trendPeriod = Math.max(5, Math.round(props.trendPeriod));
    const emitPad = Math.max(atrPeriod, erPeriod, swingLookback, trendPeriod) + 5;
    const emitFrom = base + Math.min(emitPad, m - 1);

    const O = new Float64Array(m), H = new Float64Array(m);
    const L = new Float64Array(m), C = new Float64Array(m);
    for (let i = 0; i < m; i++) {
        const b = series.get(base + i);
        O[i] = b.open; H[i] = b.high; L[i] = b.low; C[i] = b.close;
    }

    // ---- ATR, travel ----
    const trSum = new Float64Array(m + 1);
    for (let i = 0; i < m; i++) {
        let tr = H[i] - L[i];
        if (i > 0) {
            const pc = C[i - 1];
            const a = Math.abs(H[i] - pc), c = Math.abs(L[i] - pc);
            if (a > tr) tr = a;
            if (c > tr) tr = c;
        }
        trSum[i + 1] = trSum[i] + tr;
    }
    const travel = new Float64Array(m);
    for (let i = 1; i < m; i++) travel[i] = travel[i - 1] + Math.abs(C[i] - C[i - 1]);

    // ---- rolling extremes of the PREVIOUS swingLookback bars ----
    const prevMaxHigh = new Float64Array(m), prevMinLow = new Float64Array(m);
    {
        const dq = new Int32Array(m);
        let head = 0, tail = 0;
        for (let i = 0; i < m; i++) {
            while (head < tail && dq[head] < i - swingLookback) head++;
            prevMaxHigh[i] = head < tail ? H[dq[head]] : NaN;
            while (head < tail && H[dq[tail - 1]] <= H[i]) tail--;
            dq[tail++] = i;
        }
    }
    {
        const dq = new Int32Array(m);
        let head = 0, tail = 0;
        for (let i = 0; i < m; i++) {
            while (head < tail && dq[head] < i - swingLookback) head++;
            prevMinLow[i] = head < tail ? L[dq[head]] : NaN;
            while (head < tail && L[dq[tail - 1]] >= L[i]) tail--;
            dq[tail++] = i;
        }
    }

    // ---- pivots for confluence ----
    const pvIdx = [], pvVal = [];
    for (let j = 1; j < m - 1; j++) {
        if (H[j] > H[j - 1] && H[j] > H[j + 1]) { pvIdx.push(j); pvVal.push(H[j]); }
        if (L[j] < L[j - 1] && L[j] < L[j + 1]) { pvIdx.push(j); pvVal.push(L[j]); }
    }

    // Sliding bucket map: tick bucket -> count of pivots currently in window.
    // Replaces the old linear scan; a query is now (2*conflTicks+1) lookups.
    const buckets = new Map();
    const bump = (v, d) => {
        const k = Math.round(v * invTick);
        const c = (buckets.get(k) || 0) + d;
        if (c > 0) buckets.set(k, c); else buckets.delete(k);
    };

    const warmup = Math.max(swingLookback, atrPeriod, erPeriod, 3);
    let pvIn = 0, pvOut = 0;

    // level state, kept as flat parallel arrays to avoid object churn
    const upper = [], lower = [];

    // last emitted signal per direction, for cluster suppression
    let lastShort = null, lastLong = null;

    for (let i = warmup; i < m; i++) {
        const gIdx = base + i;

        const atrStart = Math.max(0, i - atrPeriod + 1);
        const atr = (trSum[i + 1] - trSum[atrStart]) / (i - atrStart + 1);

        const erStart = Math.max(0, i - erPeriod);
        const path = travel[i] - travel[erStart];
        const net = C[i] - C[erStart];
        const er = path > 0 ? Math.abs(net) / path : 0;
        const erDir = net > 0 ? 1 : (net < 0 ? -1 : 0);

        // ---------------- level lifecycle ----------------
        const step = (list, isUpper) => {
            let w = 0;
            for (let a = 0; a < list.length; a++) {
                const lv = list[a];
                if (i - lv.birth > maxAge) continue;          // aged out
                if (lv.birth >= i) { list[w++] = lv; continue; }  // not armed yet

                const breached = isUpper ? (H[i] > lv.value) : (L[i] < lv.value);
                const reclaimed = isUpper
                    ? (C[i] < lv.value - tickSize * props.reclaimTicks)
                    : (C[i] > lv.value + tickSize * props.reclaimTicks);

                if (lv.breachAt < 0) {
                    if (!breached) { list[w++] = lv; continue; }
                    lv.breachAt = i;
                    lv.extreme = isUpper ? H[i] : L[i];
                } else {
                    lv.extreme = isUpper ? Math.max(lv.extreme, H[i]) : Math.min(lv.extreme, L[i]);
                }

                const depth = isUpper ? (lv.extreme - lv.value) : (lv.value - lv.extreme);
                const sizeOk = depth >= tickSize * props.minSweepTicks &&
                               depth <= atr * props.maxSweepAtr;

                if (reclaimed) {
                    if (sizeOk && gIdx >= emitFrom) emit(lv, i, gIdx, isUpper, atr, er, erDir);
                    continue;                                  // consumed either way
                }
                if (lv.breachAt === i && props.allowNextBarReclaim >= 1) { list[w++] = lv; continue; }
                // breached and never reclaimed -> genuinely broken, drop it
            }
            list.length = w;
        };

        const emit = (lv, i2, gIdx2, isUpper, atr2, er2, erDir2) => {
            // Regime veto 1 — efficiency ratio. Catches smooth, fast trends.
            if (erDir2 === (isUpper ? 1 : -1) && er2 >= props.maxEr) return;

            // Regime veto 2 — net displacement. The efficiency ratio measures
            // SMOOTHNESS, not direction persistence, so a slow grinding trend
            // reads as chop and slips through veto 1. This one measures how far
            // price has actually travelled against the trade, in ATR, and does
            // not care how tidily it got there.
            const tStart = Math.max(0, i2 - trendPeriod);
            const netTrend = C[i2] - C[tStart];
            const adverseAtr = (isUpper ? netTrend : -netTrend) / atr2;
            if (adverseAtr >= props.maxAdverseAtr) return;

            // Cluster suppression — same direction, near the same price, close
            // in time is one idea taken twice, and it doubles the risk on it.
            const prev = isUpper ? lastShort : lastLong;
            if (prev && (i2 - prev.i) <= props.cooldownBars &&
                Math.abs(lv.value - prev.level) <= props.dedupeAtr * atr2) return;

            const entry = C[i2];
            const stop = isUpper
                ? (lv.extreme + tickSize * props.stopBufferTicks)
                : (lv.extreme - tickSize * props.stopBufferTicks);
            const risk = Math.abs(stop - entry);
            if (!(risk > 0) || risk > atr2 * props.maxRiskAtr) return;

            let target = null;
            const opposing = isUpper ? lower : upper;
            for (let a = 0; a < opposing.length; a++) {
                const o = opposing[a];
                if (o.birth >= i2) continue;
                if (isUpper ? (o.value < entry) : (o.value > entry)) {
                    if (target === null) target = o.value;
                    else target = isUpper ? Math.max(target, o.value) : Math.min(target, o.value);
                }
            }
            if (target === null) {
                target = isUpper ? (entry - props.fallbackRR * risk)
                                 : (entry + props.fallbackRR * risk);
            }
            // Cap the target. Taking the nearest opposing level unconditionally
            // produced 50-60 point targets on a 10 point stop — a multi-hour
            // hold dressed up as a 5-minute trade. Nothing blocks price before
            // the capped level, so shortening it costs nothing.
            let rr = Math.abs(target - entry) / risk;
            if (rr > props.maxRR) {
                rr = props.maxRR;
                target = isUpper ? (entry - rr * risk) : (entry + rr * risk);
            }
            if (rr < props.minRR) return;

            signals.push({
                index: gIdx2, dir: isUpper ? -1 : 1,
                entry, stop, target, rr, level: lv.value, score: lv.score
            });
            if (isUpper) lastShort = { i: i2, level: lv.value };
            else lastLong = { i: i2, level: lv.value };
        };

        step(upper, true);
        step(lower, false);

        // ---------------- score this bar, maybe create levels ----------------
        while (pvIn < pvIdx.length && pvIdx[pvIn] <= i - 2) { bump(pvVal[pvIn], +1); pvIn++; }
        while (pvOut < pvIn && pvIdx[pvOut] < i - CONFLUENCE_LOOKBACK) { bump(pvVal[pvOut], -1); pvOut++; }

        const range = H[i] - L[i];
        if (range > 0 && atr > 0) {
            for (let s = 0; s < 2; s++) {
                const side = s === 0 ? 1 : -1;
                const prevExt = side === 1 ? prevMaxHigh[i] : prevMinLow[i];
                if (!isFinite(prevExt)) continue;

                const prom = side === 1 ? (H[i] - prevExt) : (prevExt - L[i]);
                if (!(prom > 0)) continue;

                const rej = side === 1 ? (H[i] - C[i]) / range : (C[i] - L[i]) / range;
                if (rej < props.minRejection) continue;

                const price = side === 1 ? H[i] : L[i];

                let touches = 0;
                const kb = Math.round(price * invTick);
                for (let q = kb - conflTicks; q <= kb + conflTicks; q++) {
                    const c = buckets.get(q);
                    if (c) touches += c;
                }

                const adverse = (erDir === side) ? er : 0;
                const score = (
                    W.prom * clamp01(prom / (atr * PROM_NORM_ATR)) +
                    W.rej * clamp01(rej / REJ_NORM) +
                    W.dist * clamp01((side === 1 ? H[i] - C[i] : C[i] - L[i]) / (atr * DIST_NORM_ATR)) +
                    W.exp * clamp01(range / (atr * EXP_NORM_ATR)) +
                    W.reg * clamp01(1 - adverse / ER_NORM) +
                    W.conf * clamp01(touches / CONF_NORM)
                ) / W_SUM;

                if (score >= props.levelThreshold) {
                    const lv = { value: price, birth: i, score, breachAt: -1, extreme: price };
                    if (side === 1) { upper.push(lv); if (upper.length > maxActive) upper.shift(); }
                    else { lower.push(lv); if (lower.length > maxActive) lower.shift(); }
                }
            }
        }
    }

    return signals;
}

// ---------------------------------------------------------------------------
// Drawing — deliberately restrained. Only the most recent `fullDetailBars`
// worth of signals get rails; older ones are just a small chevron, which keeps
// both the chart and the draw-call count down.
// ---------------------------------------------------------------------------
function signalPlotter(canvas, instance, series) {
    const props = instance.props;
    const n = series.data.length;
    if (n < 30) return;

    const key = cacheKey(props, series, n);
    if (CACHE.key !== key) {
        CACHE = { key: key, signals: analyse(instance, series) };
    }
    const all = CACHE.signals;
    if (!all.length) return;

    const tickSize = (instance.contractInfo && instance.contractInfo.tickSize) || 0.25;
    const maxShow = Math.max(1, Math.round(props.maxSignalsShown));
    const zoneBars = Math.max(1, Math.round(props.zoneBars));
    const gap = tickSize * props.arrowGapTicks;
    const lastX = p.x.get(series.get(n - 1));

    const first = Math.max(0, all.length - maxShow);

    for (let s = first; s < all.length; s++) {
        const sg = all[s];
        const bar = series.get(sg.index);
        const x1 = p.x.get(bar);
        const endIdx = Math.min(n - 1, sg.index + zoneBars);

        // Filled risk and reward blocks, tiled one bar at a time. A vertical
        // line with relativeWidth 1 covers exactly one bar, so consecutive
        // bars tile into a solid block. This is what makes a signal read as
        // ONE object instead of three unrelated horizontal lines.
        if (props.zoneOpacity > 0) {
            for (let j = sg.index; j <= endIdx; j++) {
                const xj = p.x.get(series.get(j));
                canvas.drawLine({ x: xj, y: sg.entry }, { x: xj, y: sg.stop },
                    { color: props.riskColor, opacity: props.zoneOpacity, relativeWidth: 1 });
                canvas.drawLine({ x: xj, y: sg.entry }, { x: xj, y: sg.target },
                    { color: props.rewardColor, opacity: props.zoneOpacity, relativeWidth: 1 });
            }
        }

        let x2 = p.x.get(series.get(endIdx));
        if (endIdx === n - 1) x2 = p.x.relative(lastX, 40);

        canvas.drawLine({ x: x1, y: sg.stop }, { x: x2, y: sg.stop },
            { color: props.riskColor, opacity: props.railOpacity, width: 1.5 });
        canvas.drawLine({ x: x1, y: sg.target }, { x: x2, y: sg.target },
            { color: props.rewardColor, opacity: props.railOpacity, width: 1.5 });
        canvas.drawLine({ x: x1, y: sg.entry }, { x: x2, y: sg.entry },
            { color: props.entryColor, opacity: 1, width: 2 });

        const above = sg.dir === -1;
        const sgn = above ? 1 : -1;
        const dirColor = sg.dir === 1 ? props.longColor : props.shortColor;
        const apexY = (above ? bar.high : bar.low) + sgn * gap;
        const armY = apexY + sgn * tickSize * props.arrowHeightTicks;
        const st = { color: dirColor, opacity: 1, width: 3 };
        canvas.drawLine({ x: x1, y: apexY },
            { x: p.x.relative(x1, -props.arrowSizePx), y: armY }, st);
        canvas.drawLine({ x: x1, y: apexY },
            { x: p.x.relative(x1, props.arrowSizePx), y: armY }, st);
    }
}

module.exports = {
    name: "keyLevelSignals",
    description: "Key Level Sweep & Reclaim — entry signals",
    calculator: keyLevelSignals,
    inputType: meta.InputType.BARS,
    params: {
        // performance — the single most important knob if it ever feels heavy
        lookbackBars:        predef.paramSpecs.number(1200, 100, 100),

        // which levels are worth waiting on
        swingLookback:       predef.paramSpecs.number(8, 1, 2),
        atrPeriod:           predef.paramSpecs.number(14, 1, 2),
        erPeriod:            predef.paramSpecs.number(20, 1, 2),
        minRejection:        predef.paramSpecs.number(0.25, 0.05, 0),
        confluenceTicks:     predef.paramSpecs.number(4, 1, 0),
        levelThreshold:      predef.paramSpecs.number(0.60, 0.05, 0),
        maxLevelAgeBars:     predef.paramSpecs.number(120, 10, 5),
        maxActiveLevels:     predef.paramSpecs.number(12, 1, 1),

        // what counts as a sweep
        minSweepTicks:       predef.paramSpecs.number(2, 1, 1),
        maxSweepAtr:         predef.paramSpecs.number(1.0, 0.1, 0.1),
        reclaimTicks:        predef.paramSpecs.number(2, 1, 0),
        allowNextBarReclaim: predef.paramSpecs.number(1, 1, 0),

        // trade construction
        stopBufferTicks:     predef.paramSpecs.number(4, 1, 0),
        minRR:               predef.paramSpecs.number(1.5, 0.1, 0.1),
        maxRR:               predef.paramSpecs.number(3.0, 0.5, 0.5),
        fallbackRR:          predef.paramSpecs.number(2.0, 0.5, 0.5),
        maxRiskAtr:          predef.paramSpecs.number(2.0, 0.25, 0.25),

        // trend / clustering filters
        maxEr:               predef.paramSpecs.number(0.60, 0.05, 0),
        trendPeriod:         predef.paramSpecs.number(50, 5, 5),
        maxAdverseAtr:       predef.paramSpecs.number(2.0, 0.25, 0),
        cooldownBars:        predef.paramSpecs.number(10, 1, 0),
        dedupeAtr:           predef.paramSpecs.number(1.0, 0.25, 0),

        // display
        maxSignalsShown:     predef.paramSpecs.number(6, 1, 1),
        zoneBars:            predef.paramSpecs.number(8, 1, 2),
        zoneOpacity:         predef.paramSpecs.number(0.13, 0.02, 0),
        railOpacity:         predef.paramSpecs.number(0.90, 0.05, 0),
        arrowGapTicks:       predef.paramSpecs.number(5, 1, 0),
        arrowHeightTicks:    predef.paramSpecs.number(7, 1, 1),
        arrowSizePx:         predef.paramSpecs.number(10, 1, 2),
        riskColor:           predef.paramSpecs.color("#ef5350"),
        rewardColor:         predef.paramSpecs.color("#26a69a"),
        entryColor:          predef.paramSpecs.color("#eeeeee"),
        longColor:           predef.paramSpecs.color("#26a69a"),
        shortColor:          predef.paramSpecs.color("#ef5350")
    },
    tags: ["My Indicators"],
    plotter: [
        predef.plotters.custom(signalPlotter)
    ]
};
