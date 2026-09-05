import { execFile } from "node:child_process";
import { readFile, writeFile } from "node:fs/promises";
import { dirname, resolve } from "node:path";
import { promisify } from "node:util";
import { test, expect } from "./helpers/fixtures.js";
import { openDoroti, percentile } from "./helpers/doroti-diagnostics.js";
import { decodeResizePixelMarker } from "./helpers/resize-pixel-marker.js";
import { observeServedAssets, sourceManifest } from "./helpers/resize-manifest.js";

const exec = promisify(execFile);
test.use({ video: "off", trace: "off", screenshot: "only-on-failure" });
for (const fixture of ["F0", "F1", "F2"]) {
  test(`@headed ${fixture} Doroti and Flutter share native capture endpoint`, async ({ page }, testInfo) => {
    test.skip(process.platform !== "win32" || !process.env.DOROTI_FLUTTER_BASE_URL);
    const reports: any[] = [];
    const source = await sourceManifest(testInfo.outputPath("source.patch"));
    for (let run = 0; run < Number(process.env.DOROTI_DIFFERENTIAL_RUNS ?? 3); run++) {
      // Alternate frameworks per trial; no calls inside the native trajectory.
      for (const framework of ["doroti", "flutter"]) {
        const finishAssetObservation = observeServedAssets(page);
        if (framework === "doroti") await openDoroti(page,
          `${process.env.DOROTI_RESIZE_EXPERIMENT_QUERY ?? ""}&dorotiResizeFixture=${fixture}&dorotiFrameMarker=1`);
        else {
          await page.goto(`${process.env.DOROTI_FLUTTER_BASE_URL}/?renderer=canvaskit&dorotiResizeFixture=${fixture}&dorotiFrameMarker=1`);
          await page.waitForFunction(() => Boolean((globalThis as any).__flutterResizeFrame));
        }
        const served = await finishAssetObservation();
        const title = `native-common-${framework}-${fixture}-${run}-${Date.now()}`;
        await page.evaluate(value => { document.title = value; }, title);
        const path = testInfo.outputPath(`${framework}-${run}.json`);
        await exec("pwsh", ["-NoProfile", "-File", resolve(process.cwd(), "../../eng/run-web-native-drag.ps1"),
          "-TitleToken", title, "-OutputPath", path, "-Edge", "TopLeft", "-Motion", "reverse",
          "-DragMilliseconds", "150", "-Capture"], { windowsHide: true, timeout: 20 * 60 * 1000 });
        const native = JSON.parse(await readFile(path, "utf8"));
        const start = native.dragTiming.dragStartCounter;
        const end = native.windowSamples.at(-1).performanceCounter;
        const frequency = native.clockCalibration.qpcFrequency;
        const frames = await Promise.all(native.frames.map(async (frame: any) => ({ ...frame,
          marker: frame.png ? decodeResizePixelMarker(await readFile(resolve(dirname(path), frame.png)), true) : null,
        })));
        const active = frames.filter((f: any) => f.callbackEntryCounter >= start && f.callbackEntryCounter <= end);
        const seen = new Set<number>(frames.filter((f: any) => f.callbackEntryCounter < start && f.marker).map((f: any) => f.marker.generation));
        const newFrames = active.filter((f: any) => {
          if (!f.marker || seen.has(f.marker.generation)) return false;
          seen.add(f.marker.generation); return true;
        });
        const times = [start, ...newFrames.map((f: any) => f.callbackEntryCounter), end];
        const gaps = times.slice(1).map((time: number, i: number) => (time - times[i]) / frequency * 1000);
        const initial = frames.find((f: any) => f.callbackEntryCounter < start && f.marker);
        const chromeWidth = initial ? initial.window.right - initial.window.left - initial.marker.physicalWidth : null;
        const chromeHeight = initial ? initial.window.bottom - initial.window.top - initial.marker.physicalHeight : null;
        const geometry = active.filter((f: any) => f.marker && initial).map((f: any) => ({
          widthError: Math.abs(f.window.right - f.window.left - chromeWidth! - f.marker.physicalWidth),
          heightError: Math.abs(f.window.bottom - f.window.top - chromeHeight! - f.marker.physicalHeight),
          centerXError: f.marker.center ? Math.abs(f.marker.center.x - initial.marker.rootX - (f.window.left - initial.window.left) -
            (f.window.right - f.window.left - chromeWidth!) / 2) : null,
          centerYError: f.marker.center ? Math.abs(f.marker.center.y - initial.marker.rootY - (f.window.top - initial.window.top) -
            (f.window.bottom - f.window.top - chromeHeight!) / 2) : null,
        }));
        const widths = native.windowSamples.map((s: any) => s.window.right - s.window.left);
        const heights = native.windowSamples.map((s: any) => s.window.bottom - s.window.top);
        const span = (end - native.windowSamples[0].performanceCounter) / frequency * 1000;
        const report = { framework, fixture, run, endpoint: "WGC callback + pixel marker", span,
          manifest: { source, served, renderer: "canvaskit", flutterRevision: process.env.DOROTI_FLUTTER_REVISION,
            dorotiBuild: process.env.DOROTI_DIFFERENTIAL_BUILD_MODE ?? "notVerified", flutterBuild: "Release web; see wrapper build logs" },
          excursion: Math.min(Math.max(...widths) - Math.min(...widths), Math.max(...heights) - Math.min(...heights)),
          activeFrames: active.length, decodedFrames: active.filter((f: any) => f.marker).length,
          firstNewFrameMilliseconds: newFrames.length ? (newFrames[0].callbackEntryCounter - start) / frequency * 1000 : null,
          newFrameCount: newFrames.length, boundaryGapP95: percentile(gaps, .95), boundaryGapMax: Math.max(...gaps),
          widthErrorP95: geometry.length ? percentile(geometry.map((g: any) => g.widthError), .95) : null,
          heightErrorP95: geometry.length ? percentile(geometry.map((g: any) => g.heightError), .95) : null,
          geometry, refreshHz: native.displayRefreshHz, native, frames };
        reports.push(report);
        await writeFile(testInfo.outputPath(`captured-${framework}-${run}.json`), JSON.stringify(report, null, 2));
        expect(span).toBeGreaterThanOrEqual(140);
        expect(span).toBeLessThanOrEqual(175);
        expect(report.excursion).toBeGreaterThanOrEqual(480);
        expect(active.length).toBeGreaterThan(0);
        expect(report.decodedFrames).toBe(active.length);
      }
    }
    const comparisons = reports.filter(r => r.framework === "doroti").map(d => {
      const f = reports.find(r => r.framework === "flutter" && r.run === d.run)!;
      return { run: d.run, dorotiGapP95: d.boundaryGapP95, flutterGapP95: f.boundaryGapP95,
        displayIntervalMilliseconds: 1000 / d.refreshHz,
        // Fixed before native candidate execution: reverse travels 1200px in
        // 150ms, so one display interval permits this much physical lag.
        geometryMarginPixels: 1200 / 150 * 1000 / d.refreshHz,
        geometryStatus: d.geometry.length !== d.activeFrames || f.geometry.length !== f.activeFrames ? "notComparable" :
          d.widthErrorP95 <= f.widthErrorP95 + 1200 / 150 * 1000 / d.refreshHz &&
          d.heightErrorP95 <= f.heightErrorP95 + 1200 / 150 * 1000 / d.refreshHz ? "PASS" : "FAIL",
        status: d.newFrameCount > 1 && f.newFrameCount > 1 &&
          d.boundaryGapP95 <= f.boundaryGapP95 + 1000 / d.refreshHz ? "PASS" : "FAIL",
        limitation: "Capture callback cadence, not physical scan-out; no post-frame vs GPU-submit comparison" };
    });
    await writeFile(testInfo.outputPath("common-capture-comparison.json"), JSON.stringify(comparisons, null, 2));
  });
}
