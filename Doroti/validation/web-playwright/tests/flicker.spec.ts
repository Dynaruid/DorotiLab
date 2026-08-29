import { PNG } from "pngjs";
import { test, expect } from "./helpers/fixtures.js";
import {
  assertPresenterContract,
  openDoroti,
  resetDiagnostics,
  setDiagnosticContextState,
  waitForSettledPresenter,
} from "./helpers/doroti-diagnostics.js";

function sampledColorCount(buffer: Buffer): number {
  const image = PNG.sync.read(buffer);
  const colors = new Set<number>();
  const pixelStride = Math.max(1, Math.floor(Math.min(image.width, image.height) / 120));
  for (let y = 0; y < image.height; y += pixelStride) {
    for (let x = 0; x < image.width; x += pixelStride) {
      const offset = (y * image.width + x) * 4;
      const alpha = image.data[offset + 3];
      if (alpha === 0) continue;
      colors.add((image.data[offset] << 16) | (image.data[offset + 1] << 8) | image.data[offset + 2]);
      if (colors.size > 16) return colors.size;
    }
  }
  return colors.size;
}

async function assertNonBlankSamples(page: import("@playwright/test").Page, durationMilliseconds: number): Promise<void> {
  const canvas = page.locator("#doroti-surface");
  const deadline = Date.now() + durationMilliseconds;
  let samples = 0;
  while (Date.now() < deadline) {
    const screenshot = await canvas.screenshot();
    expect(sampledColorCount(screenshot), `canvas sample ${samples}`).toBeGreaterThan(4);
    samples++;
    await page.waitForTimeout(500);
  }
  expect(samples).toBeGreaterThan(0);
}

test("continuous rendering, wheel, and resize contain no blank automated samples", async ({ page, runtimeErrors }) => {
  await openDoroti(page);
  await resetDiagnostics(page);
  await assertNonBlankSamples(page, 60_000);

  await page.evaluate(() => {
    const root = document.querySelector(".doroti-root");
    if (!(root instanceof HTMLElement)) throw new Error("Doroti root is unavailable.");
    const finish = performance.now() + 30_000;
    const timer = setInterval(() => {
      if (performance.now() >= finish) {
        clearInterval(timer);
        return;
      }
      root.dispatchEvent(new WheelEvent("wheel", {
        bubbles: true,
        cancelable: true,
        deltaMode: WheelEvent.DOM_DELTA_PIXEL,
        deltaY: 1.5,
      }));
    }, 16);
  });
  await assertNonBlankSamples(page, 30_000);

  const resizeDeadline = Date.now() + 30_000;
  let index = 0;
  while (Date.now() < resizeDeadline) {
    const width = index++ % 2 === 0 ? 1180 : 1220;
    await page.setViewportSize({ width, height: 800 });
    const screenshot = await page.locator("#doroti-surface").screenshot();
    expect(sampledColorCount(screenshot), `resize canvas sample ${index}`).toBeGreaterThan(4);
    await page.waitForTimeout(100);
  }

  const bundle = await waitForSettledPresenter(page);
  expect(runtimeErrors).toEqual([]);
  assertPresenterContract(bundle);
});

test("WebGL context loss restores a latest exact front", async ({ page, runtimeErrors }) => {
  const before = await openDoroti(page);
  await resetDiagnostics(page);
  const supported = await setDiagnosticContextState(page, "lose");
  test.skip(!supported, "WEBGL_lose_context is unavailable in this Chromium configuration.");
  await page.waitForFunction(() => {
    const diagnostics = (globalThis as typeof globalThis & { __dorotiResizeDiagnostics?: { hosts(): number[]; snapshot(id: number): string; presenter(id: string): string } }).__dorotiResizeDiagnostics;
    if (!diagnostics) return false;
    const hostId = diagnostics.hosts()[0];
    const snapshot = JSON.parse(diagnostics.snapshot(hostId)) as { canvasId: string };
    return (JSON.parse(diagnostics.presenter(snapshot.canvasId)) as { contextLost: boolean }).contextLost;
  });
  expect(await setDiagnosticContextState(page, "restore")).toBe(true);
  const after = await waitForSettledPresenter(page);
  expect(runtimeErrors).toEqual([]);
  expect(after.presenter.contextGeneration).toBeGreaterThan(before.presenter.contextGeneration);
  expect(after.presenter.frontGeneration).toBe(after.snapshot.resizeEpoch.generation);
  assertPresenterContract(after);
});
