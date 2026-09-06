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


test("Material sample pickers, menus, selection and text editing", async ({ page, context, runtimeErrors }, testInfo) => {
  await page.setViewportSize({ width: 1600, height: 900 });
  await openDoroti(page, "&dorotiTestbedMode=sample");
  await visibleFrame(page);
  for (const name of ["Show date picker", "Show time picker"]) {
    const button = page.getByRole("button", { name, exact: true });
    await scrollTo(page, button, 1250);
    await pointer(page, button);
    const ok = page.getByRole("button", { name: "OK", exact: true });
    await expect(ok).toBeAttached();
    await page.screenshot({ path: testInfo.outputPath(name.replaceAll(" ", "-") + ".png") });
    await pointer(page, ok);
    await expect(ok).not.toBeAttached();
    const feedback = page.getByLabel(name === "Show date picker" ? /^Selected Date:/ : /^Selected time:/);
    await expect(feedback).toBeAttached();
    await expect(feedback).not.toBeAttached();
  }
  const menu = page.getByRole("button", { name: "Show menu", exact: true });
  await scrollTo(page, menu, 1250);
  await pointer(page, menu);
  const item = page.getByRole("button", { name: "Item 1", exact: true });
  await expect(item).toBeAttached();
  const itemBox = (await item.boundingBox())!;
  await page.mouse.move(itemBox.x + itemBox.width / 2, itemBox.y + itemBox.height / 2);
  await page.waitForTimeout(300);
  await page.keyboard.press("Escape");
  await expect(item).not.toBeAttached();
  await pointer(page, page.locator('[role="button"][aria-description="Open menu"]').last());
  const submenu = page.getByRole("button", { name: "Menu 3", exact: true });
  await expect(submenu).toBeAttached();
  const submenuBox = (await submenu.boundingBox())!;
  // Vertical submenu opens on hover; clicking it after hover would toggle it closed.
  await page.mouse.move(submenuBox.x + submenuBox.width / 2, submenuBox.y + submenuBox.height / 2);
  const nestedItem = page.getByRole("button", { name: "Menu 3.2", exact: true });
  await expect(nestedItem).toBeAttached();
  await pointer(page, nestedItem);
  await expect(nestedItem).not.toBeAttached();
  await expect(submenu).not.toBeAttached();
  const color = page.getByRole("textbox", { name: "Color", exact: true });
  await scrollTo(page, color, 1250);
  await pointer(page, color);
  await expect(page.locator("#doroti-ime")).toBeFocused();
  await page.keyboard.type("Green");
  await page.keyboard.press("ArrowDown");
  await page.keyboard.press("Enter");
  await expect(color).toHaveValue("Green");
  const filled = page.getByRole("textbox", { name: "Filled", exact: true }).first();
  await scrollTo(page, filled, 1250);
  await pointer(page, filled);
  await expect(page.locator("#doroti-ime")).toBeFocused();
  await expect(page.locator("#doroti-ime")).toHaveValue("");
  await expect(filled).toHaveValue("");
  await page.keyboard.type("Material");
  await expect(filled).toHaveValue("Material");
  await page.keyboard.press("Control+A");
  await page.keyboard.type("Doroti");
  await expect(filled).toHaveValue("Doroti");
  await context.grantPermissions(["clipboard-read", "clipboard-write"]);
  await page.keyboard.press("Control+A");
  await page.keyboard.press("Control+C");
  await expect.poll(() => page.evaluate(() => navigator.clipboard.readText())).toBe("Doroti");
  await page.keyboard.press("Control+X");
  await expect(filled).toHaveValue("");
  await page.keyboard.press("Control+V");
  await expect(filled).toHaveValue("Doroti");
  await page.keyboard.press("Tab");
  await page.screenshot({ path: testInfo.outputPath("text-edited.png") });
  const frame = await captureDiagnostics(page);
  expect(frame.presenter.rasterDiagnostics?.failedScenes ?? 0, frame.presenter.rasterDiagnostics?.lastFailureReason).toBe(0);
  expect(runtimeErrors).toEqual([]);
});

test("Material sample high DPI pointer opens and closes a sheet @dpr", async ({ page, runtimeErrors }) => {
  await page.setViewportSize({ width: 1600, height: 900 });
  await openDoroti(page, "&dorotiTestbedMode=sample");
  await visibleFrame(page);
  const button = page.getByRole("button", { name: "Show modal bottom sheet", exact: true });
  await scrollTo(page, button, 600);
  await pointer(page, button);
  const close = page.getByRole("button", { name: "Close bottom sheet", exact: true });
  await expect(close).toBeAttached();
  await pointer(page, close);
  await expect(close).not.toBeAttached();
  const frame = await captureDiagnostics(page);
  expect(frame.presenter.rasterDiagnostics?.failedScenes ?? 0).toBe(0);
  expect(runtimeErrors).toEqual([]);
});

test("Material sample selection controls update their shared state", async ({ page, runtimeErrors }) => {
  await page.setViewportSize({ width: 1600, height: 900 });
  await openDoroti(page, "&dorotiTestbedMode=sample");
  await visibleFrame(page);
  const week = page.getByRole("radio", { name: "Week", exact: true });
  await scrollTo(page, week, 600);
  await pointer(page, week);
  await expect(week).toHaveAttribute("aria-checked", "true");
  await expect(page.getByRole("radio", { name: "Day", exact: true })).toHaveAttribute("aria-checked", "false");
  const extraSmall = page.getByRole("button", { name: "XS", exact: true });
  await pointer(page, extraSmall);
  await expect(extraSmall).toHaveAttribute("aria-selected", "true");
  await expect(page.getByRole("button", { name: "L", exact: true })).toHaveAttribute("aria-selected", "true");
  const option = page.getByRole("checkbox", { name: "Option 1", exact: true });
  await scrollTo(page, option, 1250);
  await expect(option).toHaveAttribute("aria-checked", "true");
  await pointer(page, option);
  await expect(option).toHaveAttribute("aria-checked", "mixed");
  await pointer(page, option);
  await expect(option).toHaveAttribute("aria-checked", "false");
  const filter = page.getByRole("checkbox", { name: "Filter", exact: true });
  await scrollTo(page, filter.first(), 1250);
  await expect(filter.first()).toHaveAttribute("aria-checked", "true");
  await pointer(page, filter.first());
  await expect(filter.first()).toHaveAttribute("aria-checked", "false");
  await expect(filter.nth(1)).toHaveAttribute("aria-checked", "false");
  await expect(filter.nth(1)).toHaveAttribute("aria-disabled", "true");
  const second = page.getByRole("radio", { name: "Option 2", exact: true });
  await scrollTo(page, second, 1250);
  await pointer(page, second);
  await expect(second).toHaveAttribute("aria-checked", "true");
  await expect(page.getByRole("radio", { name: "Option 1", exact: true })).toHaveAttribute("aria-checked", "false");
  await expect(page.getByRole("radio", { name: "Option 3", exact: true })).toHaveAttribute("aria-disabled", "true");
  const frame = await captureDiagnostics(page);
  expect(frame.presenter.rasterDiagnostics?.failedScenes ?? 0).toBe(0);
  expect(runtimeErrors).toEqual([]);
});
