import { test, expect } from "./helpers/fixtures.js";
import { openDoroti } from "./helpers/doroti-diagnostics.js";

test("selected presenter proves its real WebGL2/Skia/display ownership", async ({ page, runtimeErrors }) => {
  const bundle = await openDoroti(page);
  const capability = await page.evaluate((canvasId) => {
    const diagnostics = (globalThis as typeof globalThis & {
      __dorotiResizeDiagnostics?: { capability(id: string): string };
    }).__dorotiResizeDiagnostics;
    if (!diagnostics) throw new Error("Doroti diagnostics are unavailable.");
    return JSON.parse(diagnostics.capability(canvasId)) as Record<string, unknown>;
  }, bundle.snapshot.canvasId);
  expect(runtimeErrors).toEqual([]);
  expect(capability.hardwareWebGl2).toBe(true);
  expect(capability.actualManagedSkiaRaster).toBe(true);
  if (bundle.presenter.mode === "worker-direct-webgl") {
    expect(capability.offscreenCanvas).toBe(true);
    expect(bundle.presenter.visibleContext).toBe("transferred-offscreen-webgl2");
    expect(bundle.presenter.bitmapCreated).toBe(0);
    expect(bundle.presenter.bitmapConsumed).toBe(0);
    expect(bundle.presenter.bitmapClosed).toBe(0);
    expect(bundle.presenter.activeBitmaps).toBe(0);
  } else {
    expect(capability.exactBitmapCommit).toBe(true);
  }
  if (bundle.presenter.mode !== "document-webgl" && bundle.presenter.mode !== "worker-direct-webgl") {
    expect(capability.offscreenCanvas).toBe(true);
    expect(capability.createImageBitmap).toBe(true);
    expect(bundle.presenter.rasterCanvasAttached).toBe(false);
    expect(bundle.presenter.visibleContext).toBe("bitmaprenderer");
    expect(bundle.presenter.bitmapCreated).toBeGreaterThan(0);
    expect(bundle.presenter.bitmapCreated).toBe(
      bundle.presenter.bitmapConsumed + bundle.presenter.bitmapClosed + bundle.presenter.activeBitmaps);
    expect(bundle.presenter.activeBitmaps).toBeLessThanOrEqual(1);
  }
  if (bundle.presenter.mode === "offscreen-worker" || bundle.presenter.mode === "worker-direct-webgl") {
    expect(bundle.presenter.mainManagedRuntimeCount).toBe(0);
    expect(bundle.presenter.workerManagedRuntimeCount).toBe(1);
    expect(bundle.presenter.workerRestartCount).toBeLessThanOrEqual(1);
  }
});
