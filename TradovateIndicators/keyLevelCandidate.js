const predef = require("./tools/predef");
const meta = require("./tools/meta");
const p = require("./tools/plotting");

// ---------------------------------------------------------------------------
// keyLevelCandidate
//
// One green dot. It appears on a bar when that bar's extreme scores high enough
// to be a plausible future key level — above the bar for an UPPER candidate,
// below the bar for a LOWER candidate.
//
// Strictly causal: bar i is scored using bars 0..i only. There is no next-bar
// confirmation anywhere, so a dot never appears or disappears after the bar has
// closed. The only bar that can change its mind is the live one.
// ---------------------------------------------------------------------------

// Normalisation constants — the value of each raw measure that counts as "full
// marks" for that sub-score. Tune these before you touch the weights.
const PROM_NORM_ATR = 0.5;   // clearing the lookback extreme by 0.5 ATR = full marks
const REJ_NORM      = 0.6;   // closing 60% of the bar range away from the extreme
const DIST_NORM_ATR = 0.75;  // extreme sitting 0.75 ATR away from the close
const EXP_NORM_ATR  = 1.4;   // bar range at 1.4 ATR
const ER_NORM       = 0.7;   // efficiency ratio at which the regime penalty maxes out
const CONF_NORM     = 3;     // 3 prior pivots in the price band

function clamp01(v) {
    if (!(v > 0)) return 0;
    return v > 1 ? 1 : v;
}

class keyLevelCandidate {
    map(d) {
        return {
            open: d.open(),
            high: d.high(),
            low: d.low(),
            close: d.close()
        };
    }
}

// ---------------------------------------------------------------------------
// Single forward pass over the series. Everything is O(n) except the confluence
// count, which walks a bounded window of prior pivots.
// Returns [{ index, side, score }] where side = +1 (upper) or -1 (lower).
// ---------------------------------------------------------------------------
function computeMarks(instance, series) {
    const props = instance.props;
    const n = series.data.length;
    const marks = [];
    if (n < 5) return marks;

    const tickSize = (instance.contractInfo && instance.contractInfo.tickSize) || 0.25;

    const bars = new Array(n);
    for (let i = 0; i < n; i++) bars[i] = series.get(i);

    const swingLookback = Math.max(2, Math.round(props.swingLookback));
    const atrPeriod     = Math.max(2, Math.round(props.atrPeriod));
    const erPeriod      = Math.max(2, Math.round(props.erPeriod));
    const conflLookback = Math.max(10, Math.round(props.confluenceLookback));
    const conflTol      = tickSize * props.confluenceTicks;

    // ---- ATR: prefix sums of true range ------------------------------------
    const trSum = new Float64Array(n + 1);
    for (let i = 0; i < n; i++) {
        const b = bars[i];
        let tr = b.high - b.low;
        if (i > 0) {
            const pc = bars[i - 1].close;
            const a = Math.abs(b.high - pc);
            const c = Math.abs(b.low - pc);
            if (a > tr) tr = a;
            if (c > tr) tr = c;
        }
        trSum[i + 1] = trSum[i] + tr;
    }

    // ---- cumulative close-to-close travel, for the efficiency ratio --------
    const travel = new Float64Array(n);
    for (let i = 1; i < n; i++) {
        travel[i] = travel[i - 1] + Math.abs(bars[i].close - bars[i - 1].close);
    }

    // ---- rolling max high / min low over the PREVIOUS swingLookback bars ----
    // Monotonic deque; the current bar is deliberately not in its own window.
    const prevMaxHigh = new Float64Array(n);
    const prevMinLow = new Float64Array(n);
    {
        const dq = new Int32Array(n);
        let head = 0, tail = 0;
        for (let i = 0; i < n; i++) {
            while (head < tail && dq[head] < i - swingLookback) head++;
            prevMaxHigh[i] = head < tail ? bars[dq[head]].high : NaN;
            while (head < tail && bars[dq[tail - 1]].high <= bars[i].high) tail--;
            dq[tail++] = i;
        }
    }
    {
        const dq = new Int32Array(n);
        let head = 0, tail = 0;
        for (let i = 0; i < n; i++) {
            while (head < tail && dq[head] < i - swingLookback) head++;
            prevMinLow[i] = head < tail ? bars[dq[head]].low : NaN;
            while (head < tail && bars[dq[tail - 1]].low >= bars[i].low) tail--;
            dq[tail++] = i;
        }
    }

    // ---- confirmed 3-bar pivots, used ONLY as confluence memory -------------
    // A pivot at index j is confirmed by bar j+1, so bar i may only see j <= i-2.
    const pvIndex = [];
    const pvValue = [];
    for (let j = 1; j < n - 1; j++) {
        const a = bars[j - 1], b = bars[j], c = bars[j + 1];
        if (b.high > a.high && b.high > c.high) { pvIndex.push(j); pvValue.push(b.high); }
        if (b.low < a.low && b.low < c.low) { pvIndex.push(j); pvValue.push(b.low); }
    }

    const w = {
        prom: Math.max(0, props.wProminence),
        rej:  Math.max(0, props.wRejection),
        dist: Math.max(0, props.wDistance),
        exp:  Math.max(0, props.wExpansion),
        reg:  Math.max(0, props.wRegime),
        conf: Math.max(0, props.wConfluence)
    };
    const wSum = w.prom + w.rej + w.dist + w.exp + w.reg + w.conf;
    if (wSum <= 0) return marks;

    const warmup = Math.max(swingLookback, atrPeriod, erPeriod, 3);
    let pvStart = 0, pvEnd = 0;

    for (let i = warmup; i < n; i++) {
        // advance the causal window of usable prior pivots
        while (pvEnd < pvIndex.length && pvIndex[pvEnd] <= i - 2) pvEnd++;
        while (pvStart < pvEnd && pvIndex[pvStart] < i - conflLookback) pvStart++;

        const b = bars[i];
        const range = b.high - b.low;
        if (!(range > 0)) continue;

        const atrStart = Math.max(0, i - atrPeriod + 1);
        const atr = (trSum[i + 1] - trSum[atrStart]) / (i - atrStart + 1);
        if (!(atr > 0)) continue;

        // regime: efficiency ratio + its direction
        const erStart = Math.max(0, i - erPeriod);
        const path = travel[i] - travel[erStart];
        const net = b.close - bars[erStart].close;
        const er = path > 0 ? Math.abs(net) / path : 0;
        const erDir = net > 0 ? 1 : (net < 0 ? -1 : 0);

        let best = null;

        for (let s = 0; s < 2; s++) {
            const side = s === 0 ? 1 : -1;          // +1 = upper level, -1 = lower level
            const prevExt = side === 1 ? prevMaxHigh[i] : prevMinLow[i];
            if (!isFinite(prevExt)) continue;

            // GATE 1 — the bar must actually make a fresh extreme for the window
            const prom = side === 1 ? (b.high - prevExt) : (prevExt - b.low);
            if (!(prom > 0)) continue;

            // GATE 2 — price must have been pushed back off that extreme
            const rej = side === 1
                ? (b.high - b.close) / range
                : (b.close - b.low) / range;
            if (rej < props.minRejection) continue;

            const sProm = clamp01(prom / (atr * PROM_NORM_ATR));
            const sRej  = clamp01(rej / REJ_NORM);

            const dist  = side === 1 ? (b.high - b.close) : (b.close - b.low);
            const sDist = clamp01(dist / (atr * DIST_NORM_ATR));

            const sExp  = clamp01(range / (atr * EXP_NORM_ATR));

            // a trend running toward the level is what kills it
            const adverse = (erDir === side) ? er : 0;
            const sReg = clamp01(1 - adverse / ER_NORM);

            const price = side === 1 ? b.high : b.low;
            let touches = 0;
            for (let m = pvStart; m < pvEnd; m++) {
                if (Math.abs(pvValue[m] - price) <= conflTol) touches++;
            }
            const sConf = clamp01(touches / CONF_NORM);

            const score = (w.prom * sProm + w.rej * sRej + w.dist * sDist +
                           w.exp * sExp + w.reg * sReg + w.conf * sConf) / wSum;

            if (score >= props.threshold && (best === null || score > best.score)) {
                best = { index: i, side: side, score: score };
            }
        }

        if (best) marks.push(best);
    }

    return marks;
}

// ---------------------------------------------------------------------------
// Drawing.
//
// Older candidates get a quiet graded dot. Candidates on the most recent
// `alertHoldBars` bars get the loud treatment so you cannot walk past them:
//   1. a translucent column tinting the whole candidate bar
//   2. a ray at the candidate level running out past the right edge
//   3. a chevron pointing at the level
//   4. an oversized dot
// ---------------------------------------------------------------------------
function candidatePlotter(canvas, instance, series) {
    const props = instance.props;
    const n = series.data.length;
    if (n < 5) return;

    const marks = computeMarks(instance, series);
    if (!marks.length) return;

    const tickSize = (instance.contractInfo && instance.contractInfo.tickSize) || 0.25;
    const offset = tickSize * props.dotOffsetTicks;
    const nudge = tickSize * 0.05;
    const lastBarOnly = props.lastBarOnly >= 1;
    const holdBars = Math.max(0, Math.round(props.alertHoldBars));

    // Vertical extent for the column highlight. Deliberately over-extended past
    // the data range so it fills the pane at any zoom level; the excess clips.
    let lo = Infinity, hi = -Infinity;
    for (let i = 0; i < n; i++) {
        const b = series.get(i);
        if (b.low < lo) lo = b.low;
        if (b.high > hi) hi = b.high;
    }
    const pad = (hi - lo) * 0.6 + tickSize * 20;
    lo -= pad;
    hi += pad;

    const rayEndX = p.x.relative(p.x.get(series.get(n - 1)), props.rayExtendPx);

    marks.forEach(m => {
        if (lastBarOnly && m.index !== n - 1) return;

        const bar = series.get(m.index);
        const x = p.x.get(bar);
        const s = m.side;                                   // +1 upper, -1 lower
        const level = s === 1 ? bar.high : bar.low;
        const dotY = s === 1 ? (bar.high + offset) : (bar.low - offset);
        const loud = holdBars > 0 && m.index >= n - holdBars;

        if (loud) {
            // // 1. column tint over the candidate bar
            // canvas.drawLine(
            //     { x, y: lo },
            //     { x, y: hi },
            //     {
            //         color: props.alertColor,
            //         opacity: props.highlightOpacity,
            //         relativeWidth: 0.95
            //     }
            // );

            // 2. ray at the level, out into the right margin
            canvas.drawLine(
                { x, y: level },
                { x: rayEndX, y: level },
                {
                    color: props.alertColor,
                    opacity: 0.45 + 0.45 * m.score,
                    width: 1.5
                }
            );

            // // 3. chevron pointing back at the level
            // const apexY = level + s * (offset + tickSize * 3);
            // const armY = apexY + s * tickSize * props.arrowHeightTicks;
            // const armStyle = { color: props.alertColor, opacity: 1, width: 2.5 };
            // canvas.drawLine(
            //     { x, y: apexY },
            //     { x: p.x.relative(x, -props.arrowSizePx), y: armY },
            //     armStyle
            // );
            // canvas.drawLine(
            //     { x, y: apexY },
            //     { x: p.x.relative(x, props.arrowSizePx), y: armY },
            //     armStyle
            // );
        }

        // 4. the dot. Size grades with score, and swells on a live alert.
        const size = props.dotSizePx * (0.65 + 0.35 * m.score) * (loud ? 1.7 : 1);
        canvas.drawLine(
            { x, y: dotY - nudge },
            { x, y: dotY + nudge },
            { color: props.dotColor, width: size }
        );
    });
}

module.exports = {
    name: "keyLevelCandidate",
    description: "Key Level Candidate (causal, single dot)",
    calculator: keyLevelCandidate,
    inputType: meta.InputType.BARS,
    params: {
        // structure
        swingLookback:      predef.paramSpecs.number(8, 1, 2),
        atrPeriod:          predef.paramSpecs.number(14, 1, 2),
        erPeriod:           predef.paramSpecs.number(20, 1, 2),

        // hard gates
        minRejection:       predef.paramSpecs.number(0.25, 0.05, 0),
        threshold:          predef.paramSpecs.number(0.55, 0.05, 0),

        // confluence memory
        confluenceLookback: predef.paramSpecs.number(300, 10, 10),
        confluenceTicks:    predef.paramSpecs.number(4, 1, 0),

        // scoring weights (auto-normalised, so any scale works)
        wProminence:        predef.paramSpecs.number(25, 5, 0),
        wRejection:         predef.paramSpecs.number(20, 5, 0),
        wDistance:          predef.paramSpecs.number(20, 5, 0),
        wExpansion:         predef.paramSpecs.number(10, 5, 0),
        wRegime:            predef.paramSpecs.number(15, 5, 0),
        wConfluence:        predef.paramSpecs.number(10, 5, 0),

        // display
        dotOffsetTicks:     predef.paramSpecs.number(4, 1, 0),
        dotSizePx:          predef.paramSpecs.number(8, 1, 1),
        dotColor:           predef.paramSpecs.color("#00e676"),
        lastBarOnly:        predef.paramSpecs.number(0, 1, 0),

        // loud alert visuals — alertHoldBars = 0 turns them off entirely
        alertHoldBars:      predef.paramSpecs.number(2, 1, 0),
        alertColor:         predef.paramSpecs.color("#00e676"),
        highlightOpacity:   predef.paramSpecs.number(0.14, 0.02, 0),
        arrowSizePx:        predef.paramSpecs.number(9, 1, 2),
        arrowHeightTicks:   predef.paramSpecs.number(6, 1, 1),
        rayExtendPx:        predef.paramSpecs.number(60, 10, 0)
    },
    tags: ["My Indicators"],
    plotter: [
        predef.plotters.custom(candidatePlotter)
    ]
};
