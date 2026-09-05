import { writeFile } from "node:fs/promises";
import { execFileSync } from "node:child_process";
import { createHash } from "node:crypto";
import { test, expect } from "./helpers/fixtures.js";
import { openDoroti, waitForSettledPresenter, percentile, assertPresenterContract } from "./helpers/doroti-diagnostics.js";

interface Stage {
  stage: string; time: number; generation: number; sequence: number;
  detail: Record<string, number | string>;
}
interface Ring { entries: Stage[]; dropped: number; timeOrigin: number; clock: string }
interface Collection { main: Ring; ui: Ring; raster: Ring }
function stats(values: number[]) {
  return values.length ? { count: values.length, p50: percentile(values, .5), p95: percentile(values, .95),
    p99: percentile(values, .99), max: Math.max(...values) } : { count: 0, p50: null, p95: null, p99: null, max: null };
}

test("CanvasKit legacy slow CDP stage baseline @resize-benchmark", async ({ page, browser, runtimeErrors }, testInfo) => {
  test.skip(process.env.DOROTI_WEB_RENDERER_MODE !== "worker-canvaskit-webgl");
  const experimentQuery = process.env.DOROTI_RESIZE_EXPERIMENT_QUERY ?? "";
  const ab = process.env.DOROTI_RESIZE_AB === "1";
  const traceAb = process.env.DOROTI_RESIZE_TRACE_AB === "1";
  const runs = [];
  for (let run = 0; run < (ab || traceAb ? 6 : 3); run++) {
    const traceEnabled = !traceAb || run % 2 === 0;
    const query = ab ? (run % 2 === 0 ? (process.env.DOROTI_RESIZE_BASELINE_QUERY ?? "")
      : (experimentQuery || "&dorotiResizeScheduling=display")) : experimentQuery;
    await page.setViewportSize({ width: 1280, height: 800 });
    const initial = await openDoroti(page, `&dorotiCanvasKitTrace=${traceEnabled ? 1 : 0}${query}`);
    const runtimeAssets: Record<string, string> = {};
    for (const name of ["doroti.ui.worker.js", "doroti.canvaskit.worker.js", "doroti.canvaskit.host.js", "doroti.canvaskit.trace.js"]) {
      const asset = await page.request.get(`/_content/Doroti.Host.Web/${name}`);
      expect(asset.ok(), name).toBe(true);
      runtimeAssets[name] = createHash("sha256").update(await asset.body()).digest("hex");
    }
    assertPresenterContract(initial);
    await page.waitForTimeout(500);
    const activeStart = await page.evaluate(() => performance.timeOrigin + performance.now());
    // Fixed requested trajectory; actual command/observation times are retained.
    // There is no per-size renderer settle wait.
    const trajectory = [];
    for (let index = 0; index < 120; index++) {
      const phase = index % 60;
      const step = phase < 30 ? phase : 60 - phase;
      const width = 1280 - step * 12;
      const height = 800 - step * 6;
      const commandTime = await page.evaluate(() => performance.timeOrigin + performance.now());
      await page.setViewportSize({ width, height });
      trajectory.push({ index, width, height, commandTime });
      await page.waitForTimeout(16);
    }
    const activeEnd = await page.evaluate(() => performance.timeOrigin + performance.now());
    const final = await waitForSettledPresenter(page);
    const collection: Collection = traceEnabled ? await page.evaluate(async () => {
      const experiment = (globalThis as unknown as {
        __dorotiCanvasKitExperiment: { collect(): Promise<Collection> };
      }).__dorotiCanvasKitExperiment;
      return experiment.collect();
    }) : await page.evaluate((trace) => {
      const origin = performance.timeOrigin;
      const epochs = trace.filter(e => e.phase === "target-observed").map(e => ({
        stage: "main-resize-observed", time: origin + e.timestampMicroseconds / 1000,
        generation: e.epoch.generation, sequence: 0, detail: { ...e.epoch },
      }));
      const commits = trace.filter(e => e.phase === "front-commit").map(e => {
        const d = JSON.parse(e.detail ?? "{}");
        return { stage: "main-commit-received", time: origin + e.timestampMicroseconds / 1000,
          generation: Number(d.generation), sequence: e.requestId, detail: d };
      });
      const ring = (entries: Stage[]): Ring => ({ entries, dropped: 0, timeOrigin: origin, clock: "main origin + trace timestamp; Raster commitEpochMilliseconds" });
      return { main: ring([...epochs, ...commits].sort((a,b) => a.time-b.time)), ui: ring([]),
        raster: ring(commits.map(e => ({ ...e, stage: "gpu-submit", time: Number(e.detail.commitEpochMilliseconds) }))) };
    }, final.trace);
    const main = collection.main.entries;
    const ui = collection.ui.entries;
    const raster = collection.raster.entries;
    const epochs = main.filter(e => e.stage === "main-resize-observed");
    // Epoch timestamps are main performance.now microseconds; use that original
    // observation, rather than the time its trace was appended after dispatch.
    const observed = (e: Stage) => collection.main.timeOrigin + Number(e.detail.timestampMicroseconds) / 1000;
    const byEpoch = new Map(epochs.map(e => [e.generation, e]));
    const activeEpochs = epochs.filter(e => observed(e) >= activeStart && observed(e) <= activeEnd);
    const submissions = raster.filter(e => e.stage === "gpu-submit" && byEpoch.has(e.generation) &&
      observed(byEpoch.get(e.generation)!) >= activeStart);
    const firstSubmit = new Map<number, Stage>();
    for (const event of submissions) if (!firstSubmit.has(event.generation)) firstSubmit.set(event.generation, event);
    const latency = [...firstSubmit.values()].map(e => e.time - observed(byEpoch.get(e.generation)!));
    const fronts = [...firstSubmit.values()].filter(e => e.time <= activeEnd);
    const intervals = fronts.slice(1).map((e, i) => e.time - fronts[i].time);
    const stageDeltas = (start: string, end: string, left: Stage[], right: Stage[]) => {
      const starts = new Map(left.filter(e => e.stage === start).map(e => [e.sequence, e.time]));
      return right.filter(e => e.stage === end && starts.has(e.sequence) && e.time >= activeStart && e.time <= activeEnd)
        .map(e => e.time - starts.get(e.sequence)!);
    };
    const frameStarts = new Map(ui.filter(e => e.stage === "frame-start").map(e => [e.detail.callbackId, e.time]));
    const frameTimes = ui.filter(e => e.stage === "frame-end" && e.time >= activeStart && e.time <= activeEnd)
      .map(e => e.time - frameStarts.get(e.detail.callbackId)!);
    const terminals = ui.filter(e => e.stage === "ui-terminal-received" && e.time >= activeStart && e.time <= activeEnd);
    const sends = ui.filter(e => e.stage === "scene-send");
    const handoffGaps = terminals.flatMap(e => {
      const next = sends.find(s => s.time >= e.time && s.sequence > e.sequence);
      return next ? [next.time - e.time] : [];
    });
    // Piecewise-constant integral over every target/front transition, including
    // starvation and superseded epochs. No successful-sample-only age metric.
    const allSubmits = raster.filter(e => e.stage === "gpu-submit");
    const times = [...new Set([activeStart, activeEnd,
      ...activeEpochs.map(observed), ...allSubmits.map(e => e.time).filter(t => t > activeStart && t < activeEnd)])].sort((a,b) => a-b);
    let mismatchPixelMilliseconds = 0;
    const ages: number[] = [];
    const steadyAges: number[] = [];
    for (let index = 0; index < times.length - 1; index++) {
      const t = times[index];
      const target = epochs.filter(e => observed(e) <= t).at(-1);
      const front = allSubmits.filter(e => e.time <= t).at(-1);
      const frontEpoch = front ? byEpoch.get(front.generation) : undefined;
      const a = target?.detail ?? initial.snapshot.resizeEpoch;
      const b = frontEpoch?.detail ?? initial.snapshot.resizeEpoch;
      const w = Number(a.logicalWidth), h = Number(a.logicalHeight);
      const fw = Number(b.logicalWidth), fh = Number(b.logicalHeight);
      mismatchPixelMilliseconds += (w * h + fw * fh - 2 * Math.min(w, fw) * Math.min(h, fh)) * (times[index+1] - t);
    }
    // Uniform samples avoid over-weighting intervals with many events.
    for (let t = activeStart; t <= activeEnd; t += 1000/60) {
      const front = allSubmits.filter(e => e.time <= t).at(-1);
      const epoch = front ? byEpoch.get(front.generation) : undefined;
      const age = t - (epoch ? observed(epoch) : collection.main.timeOrigin + initial.snapshot.resizeEpoch.timestampMicroseconds / 1000);
      ages.push(age);
      if (fronts.length && t >= fronts[0].time) steadyAges.push(age);
    }
    const lastEpoch = activeEpochs.at(-1)!;
    const settle = firstSubmit.get(lastEpoch.generation);
    const report = {
      run, query, traceEnabled, activeStart, activeEnd, trajectory, gpu: initial.snapshot.gpu, runtimeAssets,
      renderer: initial.presenter.mode, dpr: initial.snapshot.devicePixelRatio,
      epochToSubmit: stats(latency), frontIntervals: stats(intervals), contentAgeProxy: stats(ages),
      steadyContentAgeProxy: stats(steadyAges),
      mismatchPixelMilliseconds, stallsOver100ms: intervals.filter(t => t > 100).length,
      settleMilliseconds: settle ? settle.time - observed(lastEpoch) : null,
      supersededEpochs: activeEpochs.filter(e => !firstSubmit.has(e.generation)).length,
      stages: { uiFrame: stats(frameTimes), paragraph: stats(ui.filter(e => e.stage === "frame-end" && e.time >= activeStart && e.time <= activeEnd).map(e => Number(e.detail.paragraphMilliseconds))),
        uiBusyFraction: traceEnabled ? frameTimes.reduce((sum, value) => sum + value, 0) / (activeEnd - activeStart) : null,
        managedMap: stats(ui.filter(e => e.stage === "canvaskit-map" && e.time >= activeStart && e.time <= activeEnd).map(e => Number(e.detail.durationMicroseconds) / 1000)),
        managedEncode: stats(ui.filter(e => e.stage === "canvaskit-encode" && e.time >= activeStart && e.time <= activeEnd).map(e => Number(e.detail.durationMicroseconds) / 1000)),
        encodedToSend: stats(stageDeltas("scene-encoded", "scene-send", ui, ui)),
        sendToRaster: stats(stageDeltas("scene-send", "raster-scene-received", ui, raster)),
        decode: stats(stageDeltas("raster-scene-received", "raster-decoded", raster, raster)),
        raster: stats(stageDeltas("raster-start", "gpu-submit", raster, raster)),
        terminalDelivery: stats(terminals.map(e => e.time - Number(e.detail.sentTime))),
        terminalToNextSend: stats(handoffGaps),
        submitToMain: stats(stageDeltas("gpu-submit", "main-commit-received", raster, main)) },
      diagnostics: final.presenter, collection,
    };
    runs.push(report);
    const path = testInfo.outputPath(`resize-run-${run}.json`);
    await writeFile(path, JSON.stringify(report, null, 2));
    await testInfo.attach(`resize-run-${run}`, { path, contentType: "application/json" });
    console.log("RESIZE_RUN", JSON.stringify({ ...report, trajectory: undefined, collection: undefined, diagnostics: undefined }));
    assertPresenterContract(final);
    expect(Object.values(collection).map(ring => ring.dropped)).toEqual([0, 0, 0]);
    expect(latency.length).toBeGreaterThan(10);
    expect(latency.every(t => t >= 0)).toBe(true);
    expect(settle).toBeDefined();
    expect(final.presenter.uiDiagnostics?.buffers.outstanding).toBe(0);
    expect(final.presenter.admittedSceneCount).toBe(final.presenter.sceneTerminalCount);
    if (query.includes("dorotiResizeScheduling=display")) {
      expect(final.presenter.uiDiagnostics?.frameTimings.liveResizeThrottle.enabled).toBe(false);
      expect(final.presenter.uiDiagnostics?.frameTimings.liveResizeThrottle.deferredRafCount).toBe(0);
    }
    const admitted = ui.filter(e => e.stage === "scene-encoded");
    for (const scene of admitted)
      expect(ui.filter(e => e.stage === "ui-scene-terminal" && e.sequence === scene.sequence)).toHaveLength(1);
  }
  const summary = {
    schema: "doroti.canvaskit-resize/v1", gitHead: execFileSync("git", ["rev-parse", "HEAD"], { encoding: "utf8" }).trim(),
    experimentQuery, ab, traceAb, browser: browser.version(), configuration: "Release (wrapper supplied)",
    gates: { epochP95: 33.3, intervalP95: 33.3, settle: 50, relativeP95Improvement: .20 },
    limitations: ["GPU submission and main-message proxies; scan-out and mouse border drag notVerified",
      "Legacy slow CDP diagnostic only. Fast resize acceptance uses canvaskit-native-fast-resize.spec.ts and the Windows 240Hz SendInput driver.",
      "CDP viewport trajectory; demo workload only; input and CPU/GPU memory not measured",
      "Trace-off runs use the existing main target/front trace and do not measure individual UI stages"],
    runs: runs.map(({ collection, trajectory, diagnostics, ...rest }) => rest),
  };
  await writeFile(testInfo.outputPath("resize-summary.json"), JSON.stringify(summary, null, 2));
  expect(runtimeErrors).toEqual([]);
});
