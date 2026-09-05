import { execFile } from "node:child_process";
import { readFile, writeFile } from "node:fs/promises";
import { createHash } from "node:crypto";
import { resolve } from "node:path";
import { promisify } from "node:util";
import { test, expect } from "./helpers/fixtures.js";
import { openDoroti, resetDiagnostics, waitForSettledPresenter, percentile, assertPresenterContract, frontCommitGeneration } from "./helpers/doroti-diagnostics.js";

const execFileAsync = promisify(execFile);
const twentyMinutes = 20 * 60 * 1000;
// DevTools video/trace capture can change the very cadence measured here.
test.use({ trace: "off", video: "off", screenshot: "only-on-failure" });
interface Rect { left: number; top: number; right: number; bottom: number }
interface NativeEvidence {
  dragMillisecondsRequested: number; dragPixels: number; inputHzRequested: number; inputMethod: string;
  windowDpi: number; displayRefreshHz: number; logOnly: boolean;
  dragTiming: { dragStartCounter: number; mouseUpCounter: number; actualDurationMicroseconds: number };
  clockCalibration: { qpcFrequency: number };
  windowSamples: { performanceCounter: number; window: Rect; intendedWindow: Rect }[];
}
interface ClockCalibration { qpc: number; unixMilliseconds: number; qpcFrequency: number; uncertaintyMilliseconds: number }
const edges = process.env.DOROTI_FAST_RESIZE_EDGE ? [process.env.DOROTI_FAST_RESIZE_EDGE] : ["Right", "Bottom", "Left", "TopLeft"];
const durations = process.env.DOROTI_FAST_RESIZE_MS ? [Number(process.env.DOROTI_FAST_RESIZE_MS)] : [150, 600];
const motions = process.env.DOROTI_FAST_RESIZE_MOTION ? [process.env.DOROTI_FAST_RESIZE_MOTION] : ["expand", "shrink", "reverse"];
const runCount = Number(process.env.DOROTI_FAST_RESIZE_RUNS ?? 3);
if (!Number.isInteger(runCount) || runCount < 1 || runCount > 3) throw new Error("DOROTI_FAST_RESIZE_RUNS must be 1..3");

for (const edge of edges) for (const duration of durations) for (const motion of motions) {
  test(`@headed CanvasKit native 600px ${duration}ms ${edge} ${motion}`, async ({ page, runtimeErrors }, testInfo) => {
    test.skip(process.platform !== "win32");
    test.skip(process.env.DOROTI_WEB_FAST_RESIZE !== "1", "Use run-web-playwright.ps1 -FastResize for the owned native driver");
    test.skip(process.env.DOROTI_WEB_RENDERER_MODE !== "worker-canvaskit-webgl");
    const driver = resolve(process.cwd(), "../../../.doroti/build/windows-resize-capture-vulkan/Release/Doroti.WindowsResizeCapture.exe");
    const driverSha256 = createHash("sha256").update(await readFile(driver)).digest("hex");
    const script = resolve(process.cwd(), "../../eng/run-web-native-drag.ps1");
    for (let run = 0; run < runCount; run++) {
      const initial = await openDoroti(page, `${process.env.DOROTI_RESIZE_EXPERIMENT_QUERY ?? ""}&dorotiCanvasKitTrace=1`);
      assertPresenterContract(initial);
      const title = `doroti-fast-${edge}-${duration}-${motion}-${run}-${Date.now()}`;
      await page.evaluate(value => { document.title = value; }, title);
      await resetDiagnostics(page);
      const nativePath = testInfo.outputPath(`native-${run}.json`);
      // No Playwright call, delay, screenshot, or settle in the active drag.
      await execFileAsync("pwsh", ["-NoProfile", "-File", script, "-TitleToken", title,
        "-OutputPath", nativePath, "-Edge", edge, "-Motion", motion, "-DragMilliseconds", String(duration)],
      { windowsHide: true, timeout: twentyMinutes });
      const final = await waitForSettledPresenter(page);
      const native: NativeEvidence = JSON.parse(await readFile(nativePath, "utf8"));
      const calibration: { start: ClockCalibration; end: ClockCalibration } = JSON.parse(await readFile(`${nativePath}.clock.json`, "utf8"));
      const stages = await page.evaluate(async () => (globalThis as any).__dorotiCanvasKitExperiment.collect());
      const toEpochTime = (qpc: number) => calibration.start.unixMilliseconds +
        (qpc-calibration.start.qpc)/calibration.start.qpcFrequency*1000;
      const clockDrift = Math.abs(toEpochTime(calibration.end.qpc)-calibration.end.unixMilliseconds);
      const clockUncertainty = calibration.start.uncertaintyMilliseconds + calibration.end.uncertaintyMilliseconds;
      const motionStart = toEpochTime(native.dragTiming.dragStartCounter);
      const motionEnd = toEpochTime(native.windowSamples.at(-1)!.performanceCounter);
      // Exclude the driver's deterministic setup resize. Include late observer
      // delivery after the motion so a slow final target cannot disappear.
      const targets = final.trace.filter(e => e.phase === "target-observed" &&
        stages.main.timeOrigin + e.epoch.timestampMicroseconds/1000 >= motionStart-clockUncertainty);
      const widths = native.windowSamples.map(s => s.window.right-s.window.left);
      const heights = native.windowSamples.map(s => s.window.bottom-s.window.top);
      const widthExcursion = Math.max(...widths)-Math.min(...widths);
      const heightExcursion = Math.max(...heights)-Math.min(...heights);
      const horizontal = edge.includes("Left") || edge.includes("Right");
      const vertical = edge.includes("Top") || edge.includes("Bottom");
      const excursion = Math.min(horizontal ? widthExcursion : Infinity, vertical ? heightExcursion : Infinity);
      const inputSpan = (native.windowSamples.at(-1)!.performanceCounter-native.windowSamples[0].performanceCounter)
        / native.clockCalibration.qpcFrequency * 1000;
      const expectedSpan = duration; // reverse also uses the full 150/600ms for out-and-back.
      const fronts = final.trace.filter(e => e.phase === "front-commit");
      const latencies = targets.map(target => {
        const front = fronts.find(f => f.timestampMicroseconds >= target.timestampMicroseconds && frontCommitGeneration(f) >= target.epoch.generation);
        return front ? (front.timestampMicroseconds-target.timestampMicroseconds)/1000 : null;
      });
      const stimulus = { edge, motion, requestedPixels: 600, requestedMilliseconds: duration,
        inputHz: native.inputHzRequested, nativeInputSpanMilliseconds: inputSpan,
        mouseHeldMilliseconds: native.dragTiming.actualDurationMicroseconds/1000,
        observedNativeExcursionPixels: excursion,
        observedWidthExcursionPixels: widthExcursion, observedHeightExcursionPixels: heightExcursion,
        minimumNativeExcursionPixels: 480, // identical 80% excursion check to the Windows driver
        motionStartEpochMilliseconds: motionStart, motionEndEpochMilliseconds: motionEnd,
        clockUncertaintyMilliseconds: clockUncertainty, clockDriftMilliseconds: clockDrift,
        qualified: inputSpan <= expectedSpan + 25 && inputSpan >= expectedSpan - 10 && excursion >= 480 };
      const finite = latencies.filter((v): v is number => v !== null);
      const seenGenerations = new Set<number>();
      const activeFronts = fronts.filter(e => {
        const generation = frontCommitGeneration(e);
        const time = stages.main.timeOrigin + e.timestampMicroseconds/1000;
        if (time < motionStart || time > motionEnd || seenGenerations.has(generation)) return false;
        seenGenerations.add(generation);
        return true;
      });
      const frontIntervals = activeFronts.slice(1).map((e,i) => (e.timestampMicroseconds-activeFronts[i].timestampMicroseconds)/1000);
      const lastTarget = targets.at(-1);
      const finalFront = lastTarget && fronts.find(e => frontCommitGeneration(e) >= lastTarget.epoch.generation);
      const settle = lastTarget && finalFront ? (finalFront.timestampMicroseconds-lastTarget.timestampMicroseconds)/1000 : null;
      const following = { targetToCaughtUpFrontP95: finite.length ? percentile(finite,.95) : null,
        targetToCaughtUpFrontMax: finite.length ? Math.max(...finite) : null,
        samples: finite.length, activeFrontCount: activeFronts.length,
        activeFrontIntervalP95: frontIntervals.length ? percentile(frontIntervals,.95) : null,
        settleMilliseconds: settle,
        status: finite.length && percentile(finite,.95) <= 33.3 && frontIntervals.length &&
          percentile(frontIntervals,.95) <= 33.3 && settle !== null && settle <= 50 ? "PASS" : "FAIL" };
      const report = { schema: "doroti.canvaskit-native-fast-resize/v1", run, stimulus, driverSha256,
        renderer: final.presenter.mode, scheduling: final.presenter.uiDiagnostics?.frameTimings.liveResizeThrottle,
        following, native, calibration, diagnostics: final, stages,
        limitation: "Same Windows 240Hz native input driver, log-only (no WGC); not physical scan-out acknowledgement. Reverse driver holds at origin 100ms after motion; inputSpan excludes that hold." };
      const path = testInfo.outputPath(`fast-resize-${run}.json`);
      await writeFile(path, JSON.stringify(report, null, 2));
      await testInfo.attach(`fast-resize-${run}`, { path, contentType: "application/json" });
      console.log("FAST_NATIVE_RESIZE", JSON.stringify({ run, ...stimulus, following }));
      expect(native.inputHzRequested).toBe(240);
      expect(native.dragPixels).toBe(600);
      expect(stimulus.qualified, "The actual stimulus must meet Windows speed/excursion; never slow down to get a pass").toBe(true);
      expect(clockDrift).toBeLessThanOrEqual(clockUncertainty+2);
      expect(targets.length).toBeGreaterThan(0);
      expect(latencies.every(v => v !== null)).toBe(true);
      expect(stages.main.dropped + stages.ui.dropped + stages.raster.dropped).toBe(0);
      assertPresenterContract(final);
      expect(final.presenter.uiDiagnostics?.buffers.outstanding).toBe(0);
      if (process.env.DOROTI_WEB_REQUIRE_LATENCY === "1") expect(following.status).toBe("PASS");
    }
    expect(runtimeErrors).toEqual([]);
  });
}
