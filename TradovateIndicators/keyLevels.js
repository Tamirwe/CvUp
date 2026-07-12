const predef = require("./tools/predef");
const meta = require("./tools/meta");
const p = require("./tools/plotting");

class keyLevels {
    map(d, index, history) {
        return {
            high: d.high(),
            low: d.low(),
            close: d.close(),
            time: d.timestamp().getTime()
        };
    }
}

// Pivots are only ever formed from fully-closed bars — the last (possibly
// still-forming) bar is excluded from being a candidate pivot or a neighbor.
function findPivots(series) {
    const size = series.data.length;
    const closedSize = size - 1;
    const pivots = [];

    for (let i = 1; i < closedSize - 1; i++) {
        const prev = series.get(i - 1);
        const curr = series.get(i);
        const next = series.get(i + 1);

        if (curr.high > prev.high && curr.high > next.high) {
            pivots.push({ index: i, value: curr.high, type: "upper" });
        }
        if (curr.low < prev.low && curr.low < next.low) {
            pivots.push({ index: i, value: curr.low, type: "lower" });
        }
    }
    return pivots;
}

// Crossing/invalidation checks run against ALL bars, including the live one,
// so a level disappears the instant price actually crosses it.
// Age is now measured in BAR COUNT, not wall-clock time — this makes the
// lifetime consistent across timeframes and immune to session gaps.
function computeValidPivots(instance, series, pivots) {
    const props = instance.props;
    const maxAgeBars = props.maxAgeBars;
    const maxCount = props.maxLevels;
    const size = series.data.length;

    if (size === 0) return { validUpper: [], validLower: [], current: null };

    const current = series.get(size - 1);
    const currentIndex = size - 1;

    function stillValid(pivot) {
        // pivot formed too many bars ago
        if (currentIndex - pivot.index > maxAgeBars) return false;

        for (let j = pivot.index + 1; j < size; j++) {
            const bar = series.get(j);
            if (pivot.type === "upper" && bar.high > pivot.value) return false;
            if (pivot.type === "lower" && bar.low < pivot.value) return false;
        }
        return true;
    }

    const validUpper = pivots.filter(pv => pv.type === "upper" && stillValid(pv)).slice(-maxCount);
    const validLower = pivots.filter(pv => pv.type === "lower" && stillValid(pv)).slice(-maxCount);

    return { validUpper, validLower, current };
}

function drawDot(canvas, x, value, color, widthPx, nudge) {
    canvas.drawLine({ x, y: value - nudge }, { x, y: value + nudge }, { color, width: widthPx });
}

function pivotPlotter(canvas, instance, series) {
    if (series.data.length < 3) return;

    const props = instance.props;
    const tickSize = (instance.contractInfo && instance.contractInfo.tickSize) || 0.25;
    const pivotOffset = tickSize * props.pivotOffsetTicks;
    const nudge = tickSize * 0.05;

    const pivots = findPivots(series);

    pivots.forEach(pv => {
        const x = p.x.get(series.get(pv.index));
        if (pv.type === "upper") drawDot(canvas, x, pv.value + pivotOffset, props.upperColor, props.dotSizePx, nudge);
        else drawDot(canvas, x, pv.value - pivotOffset, props.lowerColor, props.dotSizePx, nudge);
    });
}

function validityPlotter(canvas, instance, series) {
    if (series.data.length < 3) return;

    const props = instance.props;
    const tickSize = (instance.contractInfo && instance.contractInfo.tickSize) || 0.25;
    const validOffset = tickSize * (props.pivotOffsetTicks + props.validOffsetTicks);
    const nudge = tickSize * 0.05;

    const pivots = findPivots(series);
    const { validUpper, validLower } = computeValidPivots(instance, series, pivots);

    validUpper.forEach(pv => {
        const x = p.x.get(series.get(pv.index));
        drawDot(canvas, x, pv.value + validOffset, props.validColor, props.dotSizePx, nudge);
    });
    validLower.forEach(pv => {
        const x = p.x.get(series.get(pv.index));
        drawDot(canvas, x, pv.value - validOffset, props.validColor, props.dotSizePx, nudge);
    });
}

function proximityPlotter(canvas, instance, series) {
    if (series.data.length < 3) return;

    const props = instance.props;
    const tickSize = (instance.contractInfo && instance.contractInfo.tickSize) || 0.25;
    const nudge = tickSize * 0.05;
    const markOffset = tickSize * (props.pivotOffsetTicks + props.validOffsetTicks + props.proximityExtraTicks);

    const pivots = findPivots(series);
    const { validUpper, validLower, current } = computeValidPivots(instance, series, pivots);
    if (!current) return;

    const price = current.close;
    const x = p.x.get(current);

    const nearestUpper = validUpper
        .filter(pv => pv.value >= price)
        .reduce((min, pv) => (min === null || pv.value < min.value ? pv : min), null);
    const nearestLower = validLower
        .filter(pv => pv.value <= price)
        .reduce((max, pv) => (max === null || pv.value > max.value ? pv : max), null);

    // nothing valid on either side at all — nothing to show
    if (!nearestUpper && !nearestLower) return;

    const distUpper = nearestUpper ? (nearestUpper.value - price) : Infinity;
    const distLower = nearestLower ? (price - nearestLower.value) : Infinity;
    const closerIsUpper = distUpper <= distLower;

    if (closerIsUpper) {
        drawDot(canvas, x, current.high + markOffset, props.proximityColor, props.proximityDotSizePx, nudge);
    } else {
        drawDot(canvas, x, current.low - markOffset, props.proximityColor, props.proximityDotSizePx, nudge);
    }
}

module.exports = {
    name: "keyLevels",
    description: "Upper/Lower Key Levels + Validity + Proximity",
    calculator: keyLevels,
    inputType: meta.InputType.BARS,
    params: {
        pivotOffsetTicks: predef.paramSpecs.number(3, 1, 1),
        validOffsetTicks: predef.paramSpecs.number(2, 1, 1),
        maxAgeBars: predef.paramSpecs.number(150, 1, 1),
        maxLevels: predef.paramSpecs.number(40, 1, 1),
        dotSizePx: predef.paramSpecs.number(6, 1, 1),
        upperColor: predef.paramSpecs.color("#4da6ff"),
        lowerColor: predef.paramSpecs.color("#ff4fd8"),
        validColor: predef.paramSpecs.color("#33cc33"),
        proximityTicks: predef.paramSpecs.number(10, 1, 1),
        proximityExtraTicks: predef.paramSpecs.number(2, 1, 1),
        proximityDotSizePx: predef.paramSpecs.number(9, 1, 1),
        proximityColor: predef.paramSpecs.color("#ffa500")
    },
    tags: ["My Indicators"],
    plotter: [
        predef.plotters.custom(pivotPlotter),
        predef.plotters.custom(validityPlotter),
        predef.plotters.custom(proximityPlotter)
    ]
};