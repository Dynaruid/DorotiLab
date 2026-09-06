import { test, expect } from "./helpers/fixtures.js";
import { openDoroti, captureDiagnostics } from "./helpers/doroti-diagnostics.js";

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
