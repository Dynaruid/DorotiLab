import { writeFile } from "node:fs/promises";
import { test, expect } from "./helpers/fixtures.js";
import { openDoroti, waitForSettledPresenter } from "./helpers/doroti-diagnostics.js";

test("Flutter and Doroti continuous resize comparison records endpoint limits", async ({ page, browser, runtimeErrors }, testInfo) => {
  test.skip(!process.env.DOROTI_FLUTTER_BASE_URL);
  const requestedRenderer = process.env.DOROTI_FLUTTER_RENDERER ?? "canvaskit";
  const runs = [];
  for (let run = 0; run < Number(process.env.DOROTI_DIFFERENTIAL_RUNS ?? 3); run++) {
    for (const framework of ["doroti", "flutter"]) {
      await page.setViewportSize({ width: 1280, height: 800 });
      let gpu: unknown;
      const runtimeAssets: string[] = [];
      const recordAsset = (response: import("@playwright/test").Response) => {
        if (/canvaskit|skwasm/.test(response.url())) runtimeAssets.push(response.url());
      };
      page.on("response", recordAsset);
      if (framework === "doroti") gpu = (await openDoroti(page)).snapshot.gpu;
      else {
        await page.goto(`${process.env.DOROTI_FLUTTER_BASE_URL}/?renderer=${requestedRenderer}`);
        await page.waitForFunction(() => Boolean((globalThis as any).__flutterResizeFrame), undefined, { timeout: 120000 });
      }
      await page.waitForTimeout(500);
      await page.evaluate((framework) => {
        const state = globalThis as any;
        state.__resizeSamples = [];
        state.__resizeSampling = true;
        const sample = () => {
          if (!state.__resizeSampling) return;
          let front;
          if (framework === "flutter") front = state.__flutterResizeFrame;
          else {
            const d = state.__dorotiResizeDiagnostics;
            const s = JSON.parse(d.snapshot(d.hosts()[0]));
            const p = JSON.parse(d.presenter(s.canvasId));
            front = { generation: p.frontGeneration, width: p.displayWidth, height: p.displayHeight, target: s.resizeEpoch };
          }
          state.__resizeSamples.push({ time: performance.timeOrigin + performance.now(), width: innerWidth, height: innerHeight, front });
          requestAnimationFrame(sample);
        };
        requestAnimationFrame(sample);
      }, framework);
      const trajectory = [];
      for (let index = 0; index < 120; index++) {
        const phase = index % 60, step = phase < 30 ? phase : 60-phase;
        const size = { width: 1280-step*12, height: 800-step*6 };
        trajectory.push({ ...size, time: await page.evaluate(() => performance.timeOrigin + performance.now()) });
        await page.setViewportSize(size);
        await page.waitForTimeout(16);
      }
      if (framework === "doroti") await waitForSettledPresenter(page);
      else await page.waitForFunction(() => {
        const f = (globalThis as any).__flutterResizeFrame;
        return f?.width === innerWidth && f?.height === innerHeight;
      });
      const capture = await page.evaluate(() => {
        const state = globalThis as any;
        state.__resizeSampling = false;
        return { samples: state.__resizeSamples, crossOriginIsolated, dpr: devicePixelRatio, userAgent: navigator.userAgent };
      });
      page.off("response", recordAsset);
      if (framework === "flutter") expect(runtimeAssets.some(url => url.includes(`${requestedRenderer}.wasm`))).toBe(true);
      const screenshot = testInfo.outputPath(`${framework}-${run}.png`);
      await page.screenshot({ path: screenshot });
      await testInfo.attach(`${framework}-${run}`, { path: screenshot, contentType: "image/png" });
      runs.push({ framework, run, gpu, runtimeAssets, trajectory, capture });
    }
  }
  const result = {
    schema: "doroti.flutter-resize/v1", browser: browser.version(), runs,
    dorotiRenderer: process.env.DOROTI_WEB_RENDERER_MODE, flutterRenderer: requestedRenderer,
    flutterSdkSha: process.env.DOROTI_FLUTTER_REVISION,
    relativeLatencyGate: "notComparable",
    reason: "Doroti GPU/main commit and Flutter framework-post-frame are different endpoints; demo workloads also differ.",
    physicalDrag: "notVerified", flutterThreading: requestedRenderer === "canvaskit" ? "CanvasKit renderer" : "notVerified; isolation recorded",
  };
  await writeFile(testInfo.outputPath("flutter-resize.json"), JSON.stringify(result, null, 2));
  expect(runtimeErrors).toEqual([]);
});
