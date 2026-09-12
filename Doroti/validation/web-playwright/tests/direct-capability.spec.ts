import { test, expect } from "./helpers/fixtures.js";
import { openDoroti } from "./helpers/doroti-diagnostics.js";

test("selected direct presenter proves Skia ownership without bitmap transfers", async ({ page, runtimeErrors }) => {
  const bundle = await openDoroti(page);
  const capability = await page.evaluate((canvasId) => {
    const diagnostics = (globalThis as any).__dorotiResizeDiagnostics;
    return JSON.parse(diagnostics.capability(canvasId));
  }, bundle.snapshot.canvasId);
  const webgpu = bundle.presenter.mode === "worker-direct-webgpu";
  expect(capability[webgpu ? "hardwareWebGpu" : "hardwareWebGl2"])
    .toBe(bundle.snapshot.gpu.hardware && !bundle.snapshot.gpu.softwareFallbackUsed);
  expect(capability.actualManagedSkiaRaster).toBe(true);
  expect(capability.offscreenCanvas).toBe(true);
  expect(capability.exactBitmapCommit).toBe(false);
  expect(bundle.presenter.visibleContext).toBe(webgpu ? "transferred-offscreen-webgpu" : "transferred-offscreen-webgl2");
  expect(bundle.presenter.bitmapCreated).toBe(0);
  expect(bundle.presenter.bitmapConsumed).toBe(0);
  expect(bundle.presenter.bitmapClosed).toBe(0);
  expect(bundle.presenter.activeBitmaps).toBe(0);
  expect(bundle.presenter.mainManagedRuntimeCount).toBe(1);
  expect(bundle.presenter.workerManagedRuntimeCount).toBe(0);
  expect(runtimeErrors).toEqual([]);
});
