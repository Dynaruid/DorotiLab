import { PNG } from "pngjs";
import { writeFile } from "node:fs/promises";
import { test, expect } from "./helpers/fixtures.js";
import { openDoroti, waitForSettledPresenter, assertPresenterContract, captureDiagnostics } from "./helpers/doroti-diagnostics.js";

for (const fixture of ["F0", "F1", "F2"]) {
  test(`CanvasKit ${fixture} direct and bitmap crop preserve pixels through resize`, async ({ page }, testInfo) => {
    test.skip(process.env.DOROTI_WEB_RENDERER_MODE !== "worker-canvaskit-webgl");
    const reference = new Map<string, PNG>();
    for (const [variant, presentation, query] of [
      ["baseline", "direct", ""],
      ["owned-cache", "direct", "&dorotiCopyOwnership=owned&dorotiPictureCache=1&dorotiMetricsCoalescing=frame"],
      ["encoding-cache", "direct", "&dorotiCopyOwnership=owned&dorotiEncodingCache=1&dorotiMetricsCoalescing=frame"],
      ["bitmap", "bitmap-crop", ""],
      ["bitmap-exact", "bitmap-exact", ""],
      ["bitmap-raf", "bitmap-crop", "&dorotiBitmapApply=raf"],
    ]) {
      await page.setViewportSize({ width: 960, height: 640 });
      await openDoroti(page, `&dorotiCanvasKitTrace=1&dorotiResizeFixture=${fixture}&dorotiPresentation=${presentation}${query}`);
      const identity = (await captureDiagnostics(page)).presenter.uiDiagnostics as any;
      expect(identity.resizeFixture).toBe(fixture);
      if (variant === "encoding-cache") expect(identity.encodingCache).toBe(true);
      if (variant === "owned-cache") {
        expect(identity.copyOwnership).toBe("owned");
        expect(identity.pictureCache).toBe(true);
        expect(identity.metricsCoalescing).toBe("frame");
      }
      let step = 0;
      for (const [width, height] of [[960, 640], [720, 500], [1100, 700], [960, 640]]) {
        await page.setViewportSize({ width, height });
        await expect.poll(async () => {
          const epoch = (await captureDiagnostics(page)).snapshot.resizeEpoch;
          return [epoch.logicalWidth, epoch.logicalHeight];
        }).toEqual([width, height]);
        const settled = await waitForSettledPresenter(page);
        assertPresenterContract(settled);
        const bytes = await page.locator(".doroti-root").screenshot();
        const pixels = PNG.sync.read(bytes);
        const key = `${step++}-${width}x${height}`;
        await writeFile(testInfo.outputPath(`${fixture}-${variant}-${key}.png`), bytes);
        await testInfo.attach(`${fixture}-${variant}-${key}`, { body: bytes, contentType: "image/png" });
        if (variant === "baseline") reference.set(key, pixels);
        else {
          const expected = reference.get(key)!;
          expect([pixels.width, pixels.height]).toEqual([expected.width, expected.height]);
          let different = 0;
          for (let i = 0; i < pixels.data.length; i += 4)
            if ([0, 1, 2, 3].some(channel => Math.abs(pixels.data[i + channel] - expected.data[i + channel]) > 2)) different++;
          expect(different / (pixels.width * pixels.height), `${fixture} crop/alpha/orientation pixel difference`).toBeLessThan(.001);
          await expect.poll(async () => (await captureDiagnostics(page)).presenter.activeBitmaps).toBe(0);
        }
      }
      await writeFile(testInfo.outputPath(`${fixture}-${variant}-stages.json`), JSON.stringify(
        await page.evaluate(async () => (globalThis as any).__dorotiCanvasKitExperiment.collect()), null, 2));
      await writeFile(testInfo.outputPath(`${fixture}-${variant}-diagnostics.json`), JSON.stringify(await captureDiagnostics(page), null, 2));
      if (presentation !== "direct") {
        const before = await captureDiagnostics(page);
        expect(await page.evaluate(id => (globalThis as any).__dorotiResizeDiagnostics.crashWorker(id),
          before.snapshot.canvasId)).toBe(true);
        await expect.poll(async () => (await captureDiagnostics(page)).presenter.rasterSessionId)
          .toBe(Number(before.presenter.rasterSessionId) + 1);
        const recovered = await waitForSettledPresenter(page);
        assertPresenterContract(recovered);
        await expect.poll(async () => (await captureDiagnostics(page)).presenter.activeBitmaps).toBe(0);
        const accounting = (await captureDiagnostics(page)).presenter;
        expect(accounting.bitmapCreated).toBe(accounting.bitmapConsumed + accounting.bitmapClosed);
        expect(accounting.activeCanvasLeaseCount).toBe(1);
        expect(accounting.canvasLeases?.filter(lease => lease.state === "retired"))
          .toEqual([expect.objectContaining({ terminalCount: 1 })]);
      }
    }
  });
}
