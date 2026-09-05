// These endpoints describe main-thread notifications, never captured pixels.
export interface ResizeObservation {
  time: number;
  generation: number;
  width: number;
  height: number;
  dpr: number;
}

function distribution(values: number[]) {
  const sorted = [...values].sort((a, b) => a - b);
  const quantile = (p: number) => sorted.length ? sorted[Math.ceil(sorted.length * p) - 1] : null;
  return { count: sorted.length, p95: quantile(.95), p99: quantile(.99), max: sorted.at(-1) ?? null };
}

function sameGeometry(a: ResizeObservation, b: ResizeObservation) {
  return a.width === b.width && a.height === b.height && a.dpr === b.dpr;
}

export function measureNativeObserver(
  native: { time: number; width: number; height: number }[],
  targets: ResizeObservation[], start: number, uncertainty: number,
) {
  const baseline = targets.filter(t => t.time < start).at(-1);
  if (!baseline || !native.length) return { status: "notComparable", reason: "missing pre-motion calibration" };
  const offset = { width: native[0].width - Math.round(baseline.width * baseline.dpr),
    height: native[0].height - Math.round(baseline.height * baseline.dpr) };
  const changes = native.filter((s, i) => i === 0 || s.width !== native[i - 1].width || s.height !== native[i - 1].height);
  const matches = targets.filter(t => t.time >= start).map(t => {
    const candidates = changes.filter(s => s.time <= t.time + uncertainty &&
      Math.abs(s.width - offset.width - Math.round(t.width * t.dpr)) <= 1 &&
      Math.abs(s.height - offset.height - Math.round(t.height * t.dpr)) <= 1);
    const candidate = candidates.length === 1 ? candidates[0] : null;
    return { generation: t.generation, candidates: candidates.length,
      latencyMilliseconds: candidate ? t.time - candidate.time : null,
      status: candidates.length === 1 ? "matched" : candidates.length ? "ambiguous" : "unmatched" };
  });
  return { status: "diagnostic", offsetPhysicalPixels: offset, uncertaintyMilliseconds: uncertainty,
    nativeSampleResolutionMilliseconds: distribution(native.slice(1).map((s, i) => s.time - native[i].time)),
    matches, matched: matches.filter(m => m.status === "matched").length,
    ambiguous: matches.filter(m => m.status === "ambiguous").length,
    unmatched: matches.filter(m => m.status === "unmatched").length,
    latency: distribution(matches.flatMap(m => m.latencyMilliseconds === null ? [] : [m.latencyMilliseconds])),
    limitation: "outer/client chrome offset calibrated before motion; sampled native rect, repeated reverse sizes not inferred" };
}

export function measureResizeFollowing(
  observations: ResizeObservation[], notifications: ResizeObservation[], start: number, end: number,
) {
  if (!Number.isFinite(start) || !Number.isFinite(end) || end <= start) throw new Error("Invalid motion interval");
  const targets = [...observations].sort((a, b) => a.time - b.time);
  const fronts = [...notifications].sort((a, b) => a.time - b.time);
  const activeTargets = targets.filter(t => t.time >= start && t.time <= end);
  // Include late observer delivery in latency/settle, but not the active window.
  const motionTargets = targets.filter(t => t.time >= start);
  const perTarget = motionTargets.map(t => {
    const caught = fronts.find(f => f.time >= t.time && f.generation >= t.generation);
    const exact = fronts.find(f => f.time >= t.time && f.generation === t.generation && sameGeometry(f, t));
    return { generation: t.generation, caughtUpMilliseconds: caught ? caught.time - t.time : null,
      exactEpochMilliseconds: exact ? exact.time - t.time : null,
      outcome: exact ? "exact" : caught ? "superseded" : "unreached" };
  });
  const seen = new Set(fronts.filter(f => f.time < start).map(f => f.generation));
  const activeFronts = fronts.filter(f => {
    if (f.time < start || f.time > end || seen.has(f.generation)) return false;
    seen.add(f.generation);
    return true;
  });
  const boundaries = [start, ...activeFronts.map(f => f.time), end];
  const gaps = boundaries.slice(1).map((time, i) => time - boundaries[i]);
  const intervals = activeFronts.slice(1).map((f, i) => f.time - activeFronts[i].time);

  // Integrate piecewise-constant target/front geometry. Missing initial front or
  // target remains uncovered time; it must not become zero error.
  const cuts = [...new Set([start, end, ...activeTargets.map(t => t.time),
    ...fronts.filter(f => f.time > start && f.time < end).map(f => f.time)])].sort((a, b) => a - b);
  const segments: { milliseconds: number; width: number; height: number; area: number; ageStart: number; activeAgeStart: number }[] = [];
  let uncoveredMilliseconds = 0;
  for (let i = 0; i < cuts.length - 1; i++) {
    const time = cuts[i], milliseconds = cuts[i + 1] - time;
    const target = targets.filter(t => t.time <= time).at(-1);
    const front = fronts.filter(f => f.time <= time).at(-1);
    const origin = front && targets.find(t => t.generation === front.generation && sameGeometry(t, front));
    if (!target || !front || !origin || origin.time > time) { uncoveredMilliseconds += milliseconds; continue; }
    segments.push({ milliseconds, width: Math.abs(target.width - front.width),
      height: Math.abs(target.height - front.height), area: Math.abs(target.width * target.height - front.width * front.height),
      ageStart: time - origin.time, activeAgeStart: time - Math.max(start, origin.time) });
  }
  const covered = segments.reduce((n, s) => n + s.milliseconds, 0);
  const ageDistribution = (key: "ageStart" | "activeAgeStart") => {
  const ageMax = segments.length ? Math.max(...segments.map(s => s[key] + s.milliseconds)) : null;
  let ageP95: number | null = null;
  if (ageMax !== null) {
    let low = 0, high = ageMax;
    for (let i = 0; i < 50; i++) {
      const middle = (low + high) / 2;
      const below = segments.reduce((n, s) => n + Math.max(0, Math.min(s.milliseconds, middle - s[key])), 0);
      if (below < covered * .95) low = middle; else high = middle;
    }
    ageP95 = high;
  }
  return { coveredMilliseconds: covered, uncoveredMilliseconds,
    meanMilliseconds: covered ? segments.reduce((n, s) => n + s.milliseconds * (s[key] + s.milliseconds / 2), 0) / covered : null,
    p95Milliseconds: ageP95, maxMilliseconds: ageMax };
  };
  const initialFront = fronts.filter(f => f.time <= start).at(-1);
  const initialOrigin = initialFront && targets.find(t => t.generation === initialFront.generation && sameGeometry(t, initialFront));
  const weighted = (key: "width" | "height" | "area") => {
    const integral = segments.reduce((n, s) => n + s[key] * s.milliseconds, 0);
    let sum = 0;
    const p95 = [...segments].sort((a, b) => a[key] - b[key]).find(s => (sum += s.milliseconds) >= covered * .95)?.[key] ?? null;
    return { integral, mean: covered ? integral / covered : null, p95, max: segments.length ? Math.max(...segments.map(s => s[key])) : null };
  };
  const last = motionTargets.at(-1);
  const settled = last && fronts.find(f => f.time >= last.time && f.generation === last.generation && sameGeometry(f, last));
  const caught = perTarget.flatMap(t => t.caughtUpMilliseconds === null ? [] : [t.caughtUpMilliseconds]);
  return {
    endpoint: "main-notification", activeTargetCount: activeTargets.length,
    targetCount: perTarget.length, perTarget,
    unreachedTargetCount: perTarget.filter(t => t.outcome === "unreached").length,
    supersededTargetCount: perTarget.filter(t => t.outcome === "superseded").length,
    caughtUp: distribution(caught), exactEpoch: distribution(perTarget.flatMap(t => t.exactEpochMilliseconds === null ? [] : [t.exactEpochMilliseconds])),
    activeFrontCount: activeFronts.length,
    firstFrontMilliseconds: activeFronts.length ? activeFronts[0].time - start : null,
    intervalStatus: activeFronts.length === 0 ? "FAIL" : activeFronts.length === 1 ? "insufficientSamples" : "measured",
    intervals: distribution(intervals), boundaryInclusiveGaps: distribution(gaps),
    over100msGapCount: gaps.filter(g => g > 100).length,
    geometry: { domain: "logical-css-pixels", coveredMilliseconds: covered, uncoveredMilliseconds,
      width: weighted("width"), height: weighted("height"), area: weighted("area") },
    // Preserve the old absolute age; expose pre-motion idle age separately.
    contentAge: ageDistribution("ageStart"),
    idleBaselineAgeMilliseconds: initialOrigin && initialOrigin.time <= start ? start - initialOrigin.time : null,
    activeMotionContentAge: ageDistribution("activeAgeStart"),
    settleFromObserverMilliseconds: settled && last ? settled.time - last.time : null,
    settleFromNativeEndMilliseconds: settled ? settled.time - end : null,
    nativeGeometry: "notComparable", capturedPresentation: "notVerified",
  };
}
