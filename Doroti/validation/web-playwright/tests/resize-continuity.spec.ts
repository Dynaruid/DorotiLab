import { test, expect } from "./helpers/fixtures.js";
import {
  assertPresenterContract,
  openDoroti,
  resetDiagnostics,
  waitForSettledPresenter,
} from "./helpers/doroti-diagnostics.js";

test("viewport A-B-C resize commits only the final exact target", async ({ page, runtimeErrors }) => {
  await openDoroti(page);
  await resetDiagnostics(page);
  const sizes = [
    { width: 960, height: 640 },
    { width: 1180, height: 760 },
    { width: 840, height: 700 },
    { width: 1240, height: 820 },
  ];
  for (const size of sizes) {
    await page.setViewportSize(size);
    await page.waitForTimeout(16);
  }
  const bundle = await waitForSettledPresenter(page);
  const final = sizes.at(-1)!;
  expect(runtimeErrors).toEqual([]);
  expect(bundle.snapshot.logicalWidth).toBe(final.width);
  expect(bundle.snapshot.logicalHeight).toBe(final.height);
  expect(bundle.snapshot.resizeEpoch.physicalWidth).toBe(Math.round(final.width * bundle.snapshot.devicePixelRatio));
  expect(bundle.snapshot.resizeEpoch.physicalHeight).toBe(Math.round(final.height * bundle.snapshot.devicePixelRatio));
  expect(bundle.presenter.frontGeneration).toBe(bundle.snapshot.resizeEpoch.generation);
  const finalCommit = bundle.trace.filter((entry) => entry.phase === "front-commit").at(-1);
  expect(finalCommit?.backingWidth).toBe(bundle.snapshot.resizeEpoch.physicalWidth);
  expect(finalCommit?.backingHeight).toBe(bundle.snapshot.resizeEpoch.physicalHeight);
  assertPresenterContract(bundle);
});

test("@dpr DPR 2 keeps logical, physical, and front generations coherent", async ({ page, runtimeErrors }) => {
  await openDoroti(page);
  await resetDiagnostics(page);
  await page.setViewportSize({ width: 1080, height: 720 });
  await page.waitForFunction(() => {
    const diagnostics = (globalThis as typeof globalThis & {
      __dorotiResizeDiagnostics?: { hosts(): number[]; snapshot(hostId: number): string };
    }).__dorotiResizeDiagnostics;
    if (!diagnostics) return false;
    const hostId = diagnostics.hosts()[0];
    const snapshot = JSON.parse(diagnostics.snapshot(hostId)) as {
      resizeEpoch: { logicalWidth: number; logicalHeight: number; devicePixelRatio: number };
    };
    return snapshot.resizeEpoch.logicalWidth === 1080 &&
      snapshot.resizeEpoch.logicalHeight === 720 &&
      snapshot.resizeEpoch.devicePixelRatio === 2;
  });
  const bundle = await waitForSettledPresenter(page);
  expect(runtimeErrors).toEqual([]);
  expect(bundle.snapshot.devicePixelRatio).toBe(2);
  expect(bundle.snapshot.resizeEpoch.logicalWidth).toBe(1080);
  expect(bundle.snapshot.resizeEpoch.logicalHeight).toBe(720);
  expect(bundle.snapshot.resizeEpoch.physicalWidth).toBe(2160);
  expect(bundle.snapshot.resizeEpoch.physicalHeight).toBe(1440);
  expect(bundle.presenter.frontGeneration).toBe(bundle.snapshot.resizeEpoch.generation);
  assertPresenterContract(bundle);
});

test("@headed Desktop Chrome window bounds preserve the final exact front", async ({ page, context, runtimeErrors }) => {
  await openDoroti(page);
  await resetDiagnostics(page);
  const session = await context.newCDPSession(page);
  const { windowId } = await session.send("Browser.getWindowForTarget");
  const bounds = [
    { width: 1100, height: 760 },
    { width: 1320, height: 860 },
    { width: 980, height: 720 },
    { width: 1280, height: 840 },
  ];
  for (const size of bounds) {
    await session.send("Browser.setWindowBounds", { windowId, bounds: size });
    await page.waitForTimeout(16);
  }
  const bundle = await waitForSettledPresenter(page);
  expect(runtimeErrors).toEqual([]);
  expect(bundle.presenter.frontGeneration).toBe(bundle.snapshot.resizeEpoch.generation);
  expect(bundle.presenter.queueDepth).toBe(0);
  assertPresenterContract(bundle);
});
