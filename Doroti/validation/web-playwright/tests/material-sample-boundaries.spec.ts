import { test, expect } from "./helpers/fixtures.js";
import { PNG } from "pngjs";
import type { Page, Locator } from "@playwright/test";
import { openDoroti, captureDiagnostics } from "./helpers/doroti-diagnostics.js";

async function pointer(page: Page, locator: Locator) {
  await expect(locator).toBeAttached();
  let previousBounds = '';
  await expect.poll(async () => {
    const bounds = JSON.stringify(await locator.boundingBox());
    const stable = bounds !== 'null' && bounds === previousBounds; previousBounds = bounds; return stable;
  }, { intervals: [150], message: 'Pointer target geometry settles before clicking' }).toBe(true);
  const box = await locator.boundingBox();
  expect(box).not.toBeNull();
  await page.mouse.click(box!.x + box!.width / 2, box!.y + box!.height / 2);
}
async function visibleFrame(page: Page) {
  await expect.poll(async () => {
    const png = PNG.sync.read(await page.screenshot());
    const colors = new Set<number>();
    for (let i = 0; i < png.data.length; i += 80) colors.add(png.data.readUInt32BE(i));
    return colors.size;
  }, { message: "A presented sample frame contains rendered content, not only the clear color" }).toBeGreaterThan(20);
}
async function scrollTo(page: Page, locator: Locator, x: number, direction = 1) {
  for (let step = 0; step < 60; step++) {
    const box = await locator.count() ? await locator.first().boundingBox() : null;
    if (box && box.y > 150 && box.y + box.height < 750) return;
    await page.mouse.move(x, 780);
    await page.mouse.wheel(0, box && box.y < 150 ? -200 : direction * 200);
    await page.waitForTimeout(500);
  }
  throw new Error("Target did not become visible while scrolling: " + locator);
}

test("Material sample content and settings thresholds, URL activation", async ({ page, context, runtimeErrors }, testInfo) => {
  await context.route("https://pub.dev/packages/dynamic_color", route => route.fulfill({ status: 200, contentType: "text/html", body: "<title>URL launcher target</title>" }));
  await page.setViewportSize({ width: 499, height: 844 });
  await openDoroti(page, "&dorotiTestbedMode=sample");
  await visibleFrame(page);
  await pointer(page, page.getByRole("tab", { name: "Color", exact: true }));
  const link = page.getByRole("button", { name: "Create platform dynamic color schemes with the dynamic_color package.", exact: true });
  for (const width of [499, 500, 501, 499]) {
    await page.setViewportSize({ width, height: 844 });
    if (width < 500) await expect(link).toBeAttached(); else await expect(link).not.toBeAttached();
    await page.waitForTimeout(300);
    await page.screenshot({ path: testInfo.outputPath(`color-${width}.png`) });
  }
  const popupPromise = page.waitForEvent("popup");
  await pointer(page, link);
  const popup = await popupPromise;
  await expect(popup).toHaveURL("https://pub.dev/packages/dynamic_color");
  await popup.close();
  await page.bringToFront();
  await page.evaluate(() => { window.open = () => null; });
  await pointer(page, link);
  const blockedFeedback = page.getByRole("group", { name: /popup.*blocked|blocked.*popup/i }).first();
  await expect(blockedFeedback).toBeAttached();
  // The bottom snackbar covers the low settings image row until it dismisses.
  await expect(blockedFeedback).not.toBeAttached();
  await pointer(page, page.getByRole("tab", { name: "Elevation", exact: true }));
  // Eight logical pixels of sliver padding on both sides precede the LayoutBuilder.
  for (const extent of [449, 450, 451]) {
    await page.setViewportSize({ width: extent + 16, height: 844 });
    await page.waitForTimeout(400);
    await page.screenshot({ path: testInfo.outputPath(`elevation-cross-axis-${extent}.png`) });
  }
  for (const height of [739, 740, 741]) {
    await page.setViewportSize({ width: 1600, height });
    await expect.poll(async () => { const d = await captureDiagnostics(page); return d.snapshot.logicalWidth === 1600 && d.snapshot.logicalHeight === height && d.presenter.frontGeneration === d.snapshot.resizeEpoch.generation; }).toBe(true);
    await page.waitForTimeout(1500);
    const petals = page.locator('[aria-description="Petals"]').first();
    for (let step = 0; step < 5; step++) {
      const box = await petals.boundingBox();
      if (box && box.y + box.height < height) break;
      await page.mouse.move(180, height - 40); await page.mouse.wheel(0, 150); await page.waitForTimeout(200);
    }
    await pointer(page, petals);
    await expect(petals).toHaveAttribute("aria-selected", "true", { timeout: 60000 });
    await page.screenshot({ path: testInfo.outputPath(`settings-${height}.png`) });
  }
  const frame = await captureDiagnostics(page);
  expect(frame.presenter.rasterDiagnostics?.failedScenes ?? 0, frame.presenter.rasterDiagnostics?.lastFailureReason).toBe(0);
  expect(runtimeErrors).toEqual([]);
});
test("Material sample initial low height image activation", async ({ page, runtimeErrors }, testInfo) => {
  await page.setViewportSize({ width: 1600, height: 739 });
  await openDoroti(page, "&dorotiTestbedMode=sample");
  await visibleFrame(page);
  const petals = page.locator('[aria-description="Petals"]').first();
  await page.screenshot({ path: testInfo.outputPath("before-low-height.png") });
  await pointer(page, petals);
  await expect(petals).toHaveAttribute("aria-selected", "true", { timeout: 60000 });
  await page.screenshot({ path: testInfo.outputPath("after-low-height.png") });
  expect(runtimeErrors).toEqual([]);
});

test("Material sample resize into low height settings", async ({ page, runtimeErrors }, testInfo) => {
  await page.setViewportSize({ width: 499, height: 844 });
  await openDoroti(page, "&dorotiTestbedMode=sample");
  await visibleFrame(page);
  await pointer(page, page.getByRole("tab", { name: "Elevation", exact: true }));
  for (const height of [739, 740, 741]) {
    await page.setViewportSize({ width: 1600, height });
    await expect.poll(async () => { const d = await captureDiagnostics(page); return d.snapshot.logicalWidth === 1600 && d.snapshot.logicalHeight === height && d.presenter.frontGeneration === d.snapshot.resizeEpoch.generation; }).toBe(true);
    await page.waitForTimeout(1500);
    const petals = page.locator('[aria-description="Petals"]').first();
    await testInfo.attach(`bounds-${height}`, { body: JSON.stringify(await petals.boundingBox()), contentType: "application/json" });
    await page.screenshot({ path: testInfo.outputPath(`before-${height}.png`) });
    await pointer(page, petals);
    await expect(petals).toHaveAttribute("aria-selected", "true", { timeout: 60000 });
    await page.screenshot({ path: testInfo.outputPath(`after-${height}.png`) });
  }
  expect(runtimeErrors).toEqual([]);
});
