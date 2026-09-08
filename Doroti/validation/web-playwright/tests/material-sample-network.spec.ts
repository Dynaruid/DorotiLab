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

test("Material sample image failure, retry, superseded response and brightness", async ({ page, context, runtimeErrors }, testInfo) => {
  let failLeaves = true;
  let releasePetals!: () => void;
  const petalsGate = new Promise<void>(resolve => { releasePetals = resolve; });
  let petalsRequested = false;
  await context.route("**/content_based_color_scheme_1.png", async route => {
    if (failLeaves) await route.fulfill({ status: 200, contentType: "image/png", body: "invalid-image-fixture" });
    else await route.continue();
  });
  await context.route("**/content_based_color_scheme_6.png", async route => {
    petalsRequested = true;
    await petalsGate;
    await route.continue();
  });
  await page.setViewportSize({ width: 1600, height: 1000 });
  await openDoroti(page, "&dorotiTestbedMode=sample");
  await visibleFrame(page);
  const selected = (name: string) => page.locator(`[aria-description="${name}"]`).first();
  await pointer(page, selected("Leaves"));
  const retry = page.getByRole("button", { name: "Retry image", exact: true });
  await expect(retry).toBeAttached();
  await expect(selected("M3 Baseline")).toHaveAttribute("aria-selected", "true");
  await page.screenshot({ path: testInfo.outputPath("image-failure.png") });
  failLeaves = false;
  await pointer(page, retry);
  await expect(selected("Leaves")).toHaveAttribute("aria-selected", "true", { timeout: 60000 });
  await expect(retry).not.toBeAttached();
  await expect.poll(() => petalsRequested).toBe(true);
  await pointer(page, selected("Petals"));
  await expect(page.getByRole("group", { name: "Loading Petals…", exact: true })).toBeAttached();
  await pointer(page, selected("Blue"));
  await expect(selected("Blue")).toHaveAttribute("aria-selected", "true");
  releasePetals();
  await page.waitForResponse(response => response.url().endsWith("content_based_color_scheme_6.png"));
  await page.waitForTimeout(2500);
  await expect(selected("Blue")).toHaveAttribute("aria-selected", "true");
  await expect(selected("Petals")).not.toHaveAttribute("aria-selected", "true");
  const brightness = page.getByRole("switch").first();
  await expect(brightness).toHaveAttribute("aria-checked", "true");
  await pointer(page, brightness);
  await expect(brightness).toHaveAttribute("aria-checked", "false");
  await page.waitForTimeout(500);
  await page.screenshot({ path: testInfo.outputPath("dark-blue.png") });
  await pointer(page, brightness);
  await expect(brightness).toHaveAttribute("aria-checked", "true");
  const frame = await captureDiagnostics(page);
  expect(frame.trace.filter(entry => entry.terminal === "failed")).toEqual([]);
  expect(runtimeErrors).toEqual([]);
});