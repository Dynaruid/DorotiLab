import { execFile } from "node:child_process";
import { readFile, writeFile } from "node:fs/promises";
import { createHash } from "node:crypto";
import { resolve, dirname } from "node:path";
import { promisify } from "node:util";
import { test, expect } from "./helpers/fixtures.js";
import { openDoroti, resetDiagnostics, waitForSettledPresenter, percentile, assertPresenterContract, frontCommitGeneration } from "./helpers/doroti-diagnostics.js";
import { measureResizeFollowing, measureNativeObserver } from "./helpers/resize-following.js";
import { observeServedAssets, sourceManifest } from "./helpers/resize-manifest.js";
import { decodeResizePixelMarker } from "./helpers/resize-pixel-marker.js";

const execFileAsync = promisify(execFile);
const twentyMinutes = 20 * 60 * 1000;
// DevTools video/trace capture can change the very cadence measured here.
test.use({ trace: "off", video: "off", screenshot: "only-on-failure" });
interface Rect { left: number; top: number; right: number; bottom: number }
interface NativeEvidence {
  monitorRect: Rect;
  dragMillisecondsRequested: number; dragPixels: number; inputHzRequested: number; inputMethod: string;
  windowDpi: number; displayRefreshHz: number; logOnly: boolean;
  dragTiming: { dragStartCounter: number; mouseUpCounter: number; actualDurationMicroseconds: number };
  clockCalibration: { qpcFrequency: number };
  windowSamples: { performanceCounter: number; window: Rect; intendedWindow: Rect }[];
  frames?: { png?: string; callbackEntryCounter: number; width: number; height: number; window: Rect; [key: string]: unknown }[];
}
interface ClockCalibration { qpc: number; unixMilliseconds: number; qpcFrequency: number; uncertaintyMilliseconds: number }
const edges = process.env.DOROTI_FAST_RESIZE_EDGE ? [process.env.DOROTI_FAST_RESIZE_EDGE] : ["Right", "Bottom", "Left", "TopLeft"];
const durations = process.env.DOROTI_FAST_RESIZE_MS ? [Number(process.env.DOROTI_FAST_RESIZE_MS)] : [150, 600];
const motions = process.env.DOROTI_FAST_RESIZE_MOTION ? [process.env.DOROTI_FAST_RESIZE_MOTION] : ["expand", "shrink", "reverse"];
const runCount = Number(process.env.DOROTI_FAST_RESIZE_RUNS ?? 3);
const stageTraceEnabled = process.env.DOROTI_FAST_RESIZE_TRACE === "1";
const captureEnabled = process.env.DOROTI_FAST_RESIZE_CAPTURE === "1";
if (!Number.isInteger(runCount) || runCount < 1 || runCount > 3) throw new Error("DOROTI_FAST_RESIZE_RUNS must be 1..3");

for (const edge of edges) for (const duration of durations) for (const motion of motions) {
  test(`@headed CanvasKit native 600px ${duration}ms ${edge} ${motion}`, async ({ page, runtimeErrors }, testInfo) => {
    test.skip(process.platform !== "win32");
    test.skip(process.env.DOROTI_WEB_FAST_RESIZE !== "1", "Use run-web-playwright.ps1 -FastResize for the owned native driver");
    test.skip(process.env.DOROTI_WEB_RENDERER_MODE !== "worker-canvaskit-webgl");
    const driver = resolve(process.cwd(), "../../../.doroti/build/windows-resize-capture-vulkan/Release/Doroti.WindowsResizeCapture.exe");
    const driverSha256 = createHash("sha256").update(await readFile(driver)).digest("hex");
    const script = resolve(process.cwd(), "../../eng/run-web-native-drag.ps1");
    const source = await sourceManifest(testInfo.outputPath("source.patch"));
    for (let run = 0; run < runCount; run++) {
      const finishAssetObservation = observeServedAssets(page);
      const initial = await openDoroti(page, `${process.env.DOROTI_RESIZE_EXPERIMENT_QUERY ?? ""}&dorotiCanvasKitTrace=${stageTraceEnabled ? 1 : 0}&dorotiFrameMarker=${captureEnabled ? 1 : 0}`);
      const served = await finishAssetObservation();
      assertPresenterContract(initial);
      const title = `doroti-fast-${edge}-${duration}-${motion}-${run}-${Date.now()}`;
      await page.evaluate(value => { document.title = value; }, title);
      await resetDiagnostics(page);
      const nativePath = testInfo.outputPath(`native-${run}.json`);
      // No Playwright call, delay, screenshot, or settle in the active drag.
      await execFileAsync("pwsh", ["-NoProfile", "-File", script, "-TitleToken", title,
        "-OutputPath", nativePath, "-Edge", edge, "-Motion", motion, "-DragMilliseconds", String(duration),
        ...(captureEnabled ? ["-Capture"] : [])],
      { windowsHide: true, timeout: twentyMinutes });
      const final = await waitForSettledPresenter(page);
      const native: NativeEvidence = JSON.parse(await readFile(nativePath, "utf8"));
      const capturedFrames = captureEnabled ? await Promise.all((native.frames ?? []).map(async frame => ({
        ...frame, marker: frame.png ? decodeResizePixelMarker(await readFile(resolve(dirname(nativePath), frame.png))) : null,
      }))) : [];
      const calibration: { start: ClockCalibration; end: ClockCalibration } = JSON.parse(await readFile(`${nativePath}.clock.json`, "utf8"));
      const stages = await page.evaluate(async enabled => enabled
        ? (globalThis as any).__dorotiCanvasKitExperiment.collect()
        : { main: { timeOrigin: performance.timeOrigin, dropped: 0, entries: [] },
          ui: { dropped: 0, entries: [] }, raster: { dropped: 0, entries: [] } }, stageTraceEnabled);
      const toEpochTime = (qpc: number) => calibration.start.unixMilliseconds +
        (qpc-calibration.start.qpc)/calibration.start.qpcFrequency*1000;
      const clockDrift = Math.abs(toEpochTime(calibration.end.qpc)-calibration.end.unixMilliseconds);
      const clockUncertainty = calibration.start.uncertaintyMilliseconds + calibration.end.uncertaintyMilliseconds;
      const motionStart = toEpochTime(native.dragTiming.dragStartCounter);
      const motionEnd = toEpochTime(native.windowSamples.at(-1)!.performanceCounter);
      const captured = capturedFrames.map(frame => ({ ...frame,
        callbackEpochMilliseconds: toEpochTime(Number(frame.callbackEntryCounter)),
      }));
      // The first setup PNG can still contain the prior resize epoch.
      const initialCapture = captured.filter(frame => frame.marker && frame.callbackEpochMilliseconds < motionStart).at(-1);
      const margins = initialCapture?.marker ? {
        chromeWidth: initialCapture.window.right - initialCapture.window.left - initialCapture.marker.physicalWidth,
        chromeHeight: initialCapture.window.bottom - initialCapture.window.top - initialCapture.marker.physicalHeight,
      } : null;
      const activeCaptures = captured.filter(frame => frame.callbackEpochMilliseconds >= motionStart && frame.callbackEpochMilliseconds <= motionEnd);
      const captureFollowing = captureEnabled ? {
        endpoint: "WGC callback with decoded pixel marker; callback time is not scan-out time",
        activeFrames: activeCaptures.length,
        undecodedActiveFrames: activeCaptures.filter(frame => !frame.marker).length,
        margins,
        frames: activeCaptures.map(frame => ({ time: frame.callbackEpochMilliseconds, marker: frame.marker,
          // F6-R captures the whole monitor, so PNG dimensions are not window dimensions.
          widthErrorPixels: frame.marker && margins ? Math.abs(frame.window.right - frame.window.left - margins.chromeWidth - frame.marker.physicalWidth) : null,
          heightErrorPixels: frame.marker && margins ? Math.abs(frame.window.bottom - frame.window.top - margins.chromeHeight - frame.marker.physicalHeight) : null,
          originErrorPixels: frame.marker && initialCapture?.marker ? {
            x: frame.marker.rootX - initialCapture.marker.rootX - (frame.window.left - initialCapture.window.left),
            y: frame.marker.rootY - initialCapture.marker.rootY - (frame.window.top - initialCapture.window.top),
          } : null })),
        status: margins && activeCaptures.length && activeCaptures.every(f => f.marker) ? "measured" : "notComparable",
      } : null;
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
      const followingV2 = measureResizeFollowing(
        final.trace.filter(e => e.phase === "target-observed").map(e => ({
          time: stages.main.timeOrigin + e.timestampMicroseconds / 1000,
          generation: e.epoch.generation, width: e.epoch.logicalWidth,
          height: e.epoch.logicalHeight, dpr: e.epoch.devicePixelRatio,
        })), fronts.map(e => {
          // Trace epoch is the host's current target, not necessarily the frame
          // that Raster completed. Read geometry from the actual commit payload.
          const frame = JSON.parse(e.detail ?? "{}");
          if (![frame.logicalWidth, frame.logicalHeight, frame.devicePixelRatio].every(Number.isFinite))
            throw new Error("CanvasKit front notification is missing frame geometry");
          return { time: stages.main.timeOrigin + e.timestampMicroseconds / 1000,
            generation: frontCommitGeneration(e), width: frame.logicalWidth,
            height: frame.logicalHeight, dpr: frame.devicePixelRatio };
        }), motionStart, motionEnd);
      const followingV2Status = following.status === "PASS" && followingV2.unreachedTargetCount === 0 &&
        followingV2.intervalStatus === "measured" && followingV2.over100msGapCount === 0 &&
        followingV2.settleFromObserverMilliseconds !== null && followingV2.settleFromObserverMilliseconds <= 50
        ? "PASS" : "FAIL";
      const nativeToObserver = measureNativeObserver(native.windowSamples.map(s => ({
        time: toEpochTime(s.performanceCounter), width: s.window.right - s.window.left, height: s.window.bottom - s.window.top,
      })), final.trace.filter(e => e.phase === "target-observed").map(e => ({
        time: stages.main.timeOrigin + e.timestampMicroseconds / 1000, generation: e.epoch.generation,
        width: e.epoch.logicalWidth, height: e.epoch.logicalHeight, dpr: e.epoch.devicePixelRatio,
      })), motionStart, clockUncertainty + clockDrift);
      const report = { schema: "doroti.canvaskit-native-fast-resize/v2", run, stimulus, driverSha256,
        manifest: { source, served, fixture: new URL(page.url()).searchParams.get("dorotiResizeFixture") ?? "F3", gpu: final.snapshot.gpu,
          buildKind: process.env.DOROTI_WEB_BUILD_MODE ?? "notVerified", flutterComparability: "notComparable" },
        renderer: final.presenter.mode, scheduling: final.presenter.uiDiagnostics?.frameTimings.liveResizeThrottle,
        capturedFrames, captureFollowing, nativeToObserver,
        corpus: captureEnabled ? "capture-diagnostic" : stageTraceEnabled ? "stage-trace-on-diagnostic" : "stage-trace-off-performance",
        following, followingV2: { ...followingV2, status: followingV2Status }, native, calibration, diagnostics: final, stages,
        limitation: `Same Windows 240Hz native input driver, ${captureEnabled ? "WGC capture" : "log-only (no WGC)"}; not physical scan-out acknowledgement. Reverse driver holds at origin 100ms after motion; inputSpan excludes that hold.` };
      const path = testInfo.outputPath(`fast-resize-${run}.json`);
      await writeFile(path, JSON.stringify(report, null, 2));
      await testInfo.attach(`fast-resize-${run}`, { path, contentType: "application/json" });
      console.log("FAST_NATIVE_RESIZE", JSON.stringify({ run, ...stimulus, following,
        followingV2: { status: followingV2Status, firstFrontMilliseconds: followingV2.firstFrontMilliseconds,
          gaps: followingV2.boundaryInclusiveGaps, unreached: followingV2.unreachedTargetCount,
          superseded: followingV2.supersededTargetCount, geometry: followingV2.geometry } }));
      expect(native.inputHzRequested).toBe(240);
      expect(native.dragPixels).toBe(600);
      expect(stimulus.qualified, "The actual stimulus must meet Windows speed/excursion; never slow down to get a pass").toBe(true);
      expect(clockDrift).toBeLessThanOrEqual(clockUncertainty+2);
      expect(targets.length).toBeGreaterThan(0);
      expect(latencies.every(v => v !== null)).toBe(true);
      expect(stages.main.dropped + stages.ui.dropped + stages.raster.dropped).toBe(0);
      assertPresenterContract(final);
      expect(final.presenter.uiDiagnostics?.buffers.outstanding).toBe(0);
      if (process.env.DOROTI_WEB_REQUIRE_LATENCY === "1") expect(followingV2Status).toBe("PASS");
    }
    expect(runtimeErrors).toEqual([]);
  });
}
