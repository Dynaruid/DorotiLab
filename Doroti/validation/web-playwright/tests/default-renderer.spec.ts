import { test, expect } from "./helpers/fixtures.js";
import { openDoroti } from "./helpers/doroti-diagnostics.js";

for (const selection of ["omitted", "auto", "worker-direct-webg", "document-webgl", "worker-direct-webgl", "worker-direct-webgpu", "worker-canvaskit-webgl", "offscreen-worker", "offscreen-bitmap"] as const) {
  test(`renderer selection: ${selection}`, async ({ page, runtimeErrors }) => {
    test.skip((process.env.DOROTI_WEB_RENDERER_MODE ?? "auto") !== "auto",
      "Default selection is checked without a forced renderer.");
    const query = selection === "omitted" ? "" : `&dorotiRenderer=${selection}`;
    const requestedAssets: string[] = [];
    page.on("request", request => requestedAssets.push(new URL(request.url()).pathname));
    const bundle = await openDoroti(page, query);
    const explicit = selection === "worker-direct-webgl" || selection === "worker-direct-webgpu";
    const expected = explicit ? selection : "worker-direct-webgpu";
    expect(bundle.presenter.mode).toBe(expected);
    if (expected === "worker-direct-webgpu") {
      // A CPU-side submission alone can precede asynchronous GPU validation.
      // Wait for the diagnostic scene's submissions to complete on the owner.
      await expect.poll(async () => {
        // Idle WASM pthreads can be blocked in Atomics.wait. Inspect candidates
        // concurrently so an idle pool worker cannot block the render owner.
        const gpu = await Promise.any(page.workers().map(async worker => {
          const gpu = await worker.evaluate(() =>
            (globalThis as any).__dorotiDirectDiagnostics?.().webgpu ?? null);
          if (!gpu) throw new Error("Not the render owner");
          return gpu;
        }));
        return gpu.submittedFrames >= 2 && gpu.completedSubmissions === gpu.submittedFrames && !gpu.failure;
      }).toBe(true);
      await expect(page.locator("#doroti-surface")).toBeVisible();
      expect(await page.evaluate(() => document.documentElement.dataset.dorotiRendererError ?? null)).toBeNull();
    }
    {
      expect(bundle.presenter.mainManagedRuntimeCount).toBe(1);
      expect(bundle.presenter.workerManagedRuntimeCount).toBe(0);
      expect(bundle.presenter.visibleContext).toBe(expected === "worker-direct-webgpu" ? "transferred-offscreen-webgpu" : "transferred-offscreen-webgl2");
      expect(bundle.presenter.frontRequestId).toBeGreaterThan(0);
      expect(bundle.presenter.fallbackReason).toBeNull();
      expect(bundle.presenter.requestedMode).toBe(explicit ? selection : "auto");
    }
    expect(runtimeErrors).toEqual([]);
    expect(requestedAssets.filter(path => /canvaskit|doroti\.ui\.worker|blazor\.webassembly/i.test(path))).toEqual([]);
  });
}
