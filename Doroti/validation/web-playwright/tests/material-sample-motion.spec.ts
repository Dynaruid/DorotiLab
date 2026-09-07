import { test, expect } from "./helpers/fixtures.js";
import { openDoroti, captureDiagnostics } from "./helpers/doroti-diagnostics.js";

test("Material sample progress animation changes presented pixels and stops", async ({ page, runtimeErrors }, testInfo) => {
  await page.setViewportSize({ width: 1280, height: 900 });
  await openDoroti(page, "&dorotiTestbedMode=sample");
  const start = page.locator('[role="button"][aria-description="Start progress"]');
  let bounds = null;
  for (let step = 0; step < 40; step++) {
    bounds = await start.count() ? await start.boundingBox() : null;
    if (bounds && bounds.y > 150 && bounds.y + bounds.height < 780) break;
    await page.mouse.move(500, 600); await page.mouse.wheel(0, 200); await page.waitForTimeout(500);
  }
  expect(bounds).not.toBeNull();
  expect(bounds!.y).toBeGreaterThan(150);
  expect(bounds!.y + bounds!.height).toBeLessThan(780);
  await page.mouse.click(bounds!.x + bounds!.width / 2, bounds!.y + bounds!.height / 2);
  const stop = page.locator('[role="button"][aria-description="Stop progress"]');
  await expect(stop).toBeAttached();
  await page.mouse.move(20, 40); await page.waitForTimeout(1000);
  const before = await captureDiagnostics(page);
  const first = await page.screenshot();
  await page.waitForTimeout(350);
  const second = await page.screenshot();
  const during = await captureDiagnostics(page);
  expect(during.presenter.frontRequestId).toBeGreaterThan(before.presenter.frontRequestId ?? 0);
  expect(first.equals(second), "The running indicator changes rendered pixels").toBe(false);
  const stopBounds = (await stop.boundingBox())!;
  await page.mouse.click(stopBounds.x + stopBounds.width / 2, stopBounds.y + stopBounds.height / 2);
  await expect(start).toBeAttached();
  await page.mouse.move(20, 40); await page.waitForTimeout(1500);
  const settled = await captureDiagnostics(page);
  await page.waitForTimeout(500);
  const after = await captureDiagnostics(page);
  expect(after.presenter.frontRequestId).toBe(settled.presenter.frontRequestId);
  expect(after.presenter.rasterDiagnostics?.failedScenes ?? 0).toBe(0);
  expect(runtimeErrors).toEqual([]);
  await testInfo.attach("animation-frame-accounting", { body: JSON.stringify({ before, during, settled, after }), contentType: "application/json" });
});

test("Material sample navigation preserves destination during resize reversal", async ({ page, runtimeErrors }, testInfo) => {
  await page.setViewportSize({ width: 800, height: 900 });
  await openDoroti(page, "&dorotiTestbedMode=sample");
  const color = page.getByRole("tab", { name: "Color", exact: true });
  await expect(color).toBeAttached();
  const box = (await color.boundingBox())!;
  await page.mouse.click(box.x + box.width / 2, box.y + box.height / 2);
  await expect(color).toHaveAttribute("aria-selected", "true");
  const samples: unknown[] = [];
  for (const width of [1280, 800, 1280, 800]) {
    await page.setViewportSize({ width, height: 900 });
    await expect.poll(async () => (await captureDiagnostics(page)).snapshot.logicalWidth).toBe(width);
    for (let i = 0; i < 3; i++) {
      const frame = await captureDiagnostics(page);
      const bounds = await page.getByRole("tab", { name: "Color", exact: true }).evaluateAll(nodes => nodes.map(node => {
        const rect = node.getBoundingClientRect();
        return { x: rect.x, y: rect.y, width: rect.width, height: rect.height, selected: node.getAttribute("aria-selected") };
      }));
      samples.push({ width, front: frame.presenter.frontRequestId, generation: frame.presenter.frontGeneration, bounds });
      expect(frame.presenter.rasterDiagnostics?.failedScenes ?? 0).toBe(0);
      await page.waitForTimeout(100);
    }
  }
  await expect.poll(async () => {
    const frame = await captureDiagnostics(page);
    return frame.presenter.frontGeneration === frame.snapshot.resizeEpoch.generation;
  }).toBe(true);
  await page.waitForTimeout(1100);
  await expect(color).toHaveCount(1);
  await expect(color).toHaveAttribute("aria-selected", "true");
  const settled = (await color.boundingBox())!;
  expect(settled.y).toBeGreaterThan(800);
  await expect(page.getByRole("button").and(page.locator('[aria-description="Toggle brightness"]'))).toHaveCount(1);
  await testInfo.attach("resize-reversal-geometry", { body: JSON.stringify({ status: "AUTOMATED_GEOMETRY_ONLY", samples, settled }, null, 2), contentType: "application/json" });
  expect(runtimeErrors).toEqual([]);
});
