import { test, expect } from "./helpers/fixtures.js";
import { captureDiagnostics, openDoroti } from "./helpers/doroti-diagnostics.js";

test("offscreen worker has single runtime ownership and one bounded crash recovery", async ({ page, runtimeErrors }) => {
  const before = await openDoroti(page);
  test.skip(before.presenter.mode !== "offscreen-worker", "worker-only protocol validation");
  expect(before.presenter.mainManagedRuntimeCount).toBe(0);
  expect(before.presenter.workerManagedRuntimeCount).toBe(1);
  expect(before.presenter.rasterCanvasAttached).toBe(false);
  await expect(page.locator("script[data-doroti-blazor-loader]")).toHaveCount(0);
  const crashed = await page.evaluate((canvasId) => {
    const diagnostics = (globalThis as typeof globalThis & {
      __dorotiResizeDiagnostics?: { crashWorker(id: string): boolean };
    }).__dorotiResizeDiagnostics;
    return diagnostics?.crashWorker(canvasId) ?? false;
  }, before.snapshot.canvasId);
  expect(crashed).toBe(true);
  await page.waitForFunction(() => {
    const diagnostics = (globalThis as typeof globalThis & {
      __dorotiResizeDiagnostics?: {
        hosts(): number[]; snapshot(id: number): string; presenter(id: string): string;
      };
    }).__dorotiResizeDiagnostics;
    if (!diagnostics) return false;
    const hostId = diagnostics.hosts()[0];
    const snapshot = JSON.parse(diagnostics.snapshot(hostId)) as {
      canvasId: string; resizeEpoch: { generation: number };
    };
    const presenter = JSON.parse(diagnostics.presenter(snapshot.canvasId)) as {
      workerRestartCount: number; frontGeneration: number | null; queueDepth: number;
    };
    return presenter.workerRestartCount === 1 && presenter.queueDepth === 0 &&
      presenter.frontGeneration === snapshot.resizeEpoch.generation;
  }, undefined, { timeout: 120_000 });
  const after = await captureDiagnostics(page);
  expect(after.presenter.workerRestartCount).toBe(1);
  expect(after.presenter.frontGeneration).toBe(after.snapshot.resizeEpoch.generation);
  expect(runtimeErrors).toEqual([]);
});
