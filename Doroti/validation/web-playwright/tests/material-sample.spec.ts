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
    // The last action can rest near the viewport bottom at maximum scroll.
    // Require a fully visible target, without an unreachable 750px cutoff.
    if (box && box.y > 150 && box.y + box.height < (page.viewportSize()?.height ?? 900) - 16) return;
    await page.mouse.move(x, 780);
    await page.mouse.wheel(0, box && box.y < 150 ? -200 : direction * 200);
    await page.waitForTimeout(500);
  }
  throw new Error("Target did not become visible while scrolling: " + locator);
}

test("Material sample four destinations, theme controls, and responsive navigation", async ({ page, runtimeErrors }, testInfo) => {
  let logged = 0; page.on("console", message => { if (message.type() === "error" && logged++ < 12) console.log("SAMPLE_RUNTIME", message.text()); });
  await page.setViewportSize({ width: 800, height: 900 });
  await openDoroti(page, "&dorotiTestbedMode=sample");
  await expect(page.getByRole("heading", { name: "Doroti Material 3", exact: true })).toBeAttached();
  await visibleFrame(page);
  await page.screenshot({ path: testInfo.outputPath("initial-components-800.png") });
  for (const destination of ["Color", "Typography", "Elevation", "Components"]) {
    const target = page.getByRole("tab", { name: destination, exact: true }).first();
    await expect(target).toBeAttached();
    const before = await captureDiagnostics(page);
    await pointer(page, target);
    await expect(target).toHaveAttribute("aria-selected", "true");
    await expect.poll(async () => (await captureDiagnostics(page)).presenter.frontRequestId ?? 0).toBeGreaterThan(before.presenter.frontRequestId ?? 0);
    const after = await captureDiagnostics(page);
    expect(after.presenter.rasterDiagnostics?.failedScenes ?? 0, after.presenter.rasterDiagnostics?.lastFailureReason).toBe(0);
    await page.mouse.move(790, 70);
    await page.waitForTimeout(600);
    await page.screenshot({ path: testInfo.outputPath(`${destination.toLowerCase()}-800.png`) });
  }
  for (const width of [999, 1000, 1001, 1499, 1500, 1501, 390, 1280, 1600]) {
    await page.setViewportSize({ width, height: width === 390 ? 844 : 900 });
    await expect.poll(async () => {
      const bundle = await captureDiagnostics(page);
      return bundle.snapshot.logicalWidth === width && bundle.presenter.frontGeneration === bundle.snapshot.resizeEpoch.generation;
    }).toBe(true);
    // The pinned navigation transition lasts one second.
    await page.waitForTimeout(1100);
    const frame = await captureDiagnostics(page);
    expect(frame.presenter.rasterDiagnostics?.failedScenes ?? 0, frame.presenter.rasterDiagnostics?.lastFailureReason).toBe(0);
    await page.screenshot({ path: testInfo.outputPath(`responsive-${width}.png`) });
  }
  for (const destination of ["Color", "Typography", "Elevation", "Components"]) {
    const target = page.getByRole("group", { name: new RegExp(`^${destination} Tab [1-4] of 4$`) });
    const before = await captureDiagnostics(page);
    await pointer(page, target);
    await expect(target).toHaveAttribute("aria-selected", "true");
    await expect.poll(async () => (await captureDiagnostics(page)).presenter.frontRequestId ?? 0).toBeGreaterThan(before.presenter.frontRequestId ?? 0);
    if (destination === "Components") await expect(page.getByRole("button", { name: "Elevated", exact: true }).first()).toBeAttached();
    await page.mouse.move(1580, 40);
    await page.waitForTimeout(1000);
    await page.screenshot({ path: testInfo.outputPath(`${destination.toLowerCase()}-1600.png`) });
  }
  await page.setViewportSize({ width: 800, height: 900 });
  await expect.poll(async () => {
    const frame = await captureDiagnostics(page);
    return frame.snapshot.logicalWidth === 800 && frame.presenter.frontGeneration === frame.snapshot.resizeEpoch.generation;
  }).toBe(true);
  await page.waitForTimeout(1100);
  const brightnessButton = page.getByRole("button").and(page.locator('[aria-description="Toggle brightness"]'));
  await expect(brightnessButton).toHaveCount(1);
  const beforeBrightness = await captureDiagnostics(page);
  await pointer(page, brightnessButton);
  await expect.poll(async () => (await captureDiagnostics(page)).presenter.frontRequestId ?? 0).toBeGreaterThan(beforeBrightness.presenter.frontRequestId ?? 0);
  await page.waitForTimeout(600);
  for (const destination of ["Components", "Color", "Typography", "Elevation"]) {
    const target = page.getByRole("tab", { name: destination, exact: true }).first();
    const before = await captureDiagnostics(page);
    await pointer(page, target);
    await expect(target).toHaveAttribute("aria-selected", "true");
    if (destination !== "Components")
      await expect.poll(async () => (await captureDiagnostics(page)).presenter.frontRequestId ?? 0).toBeGreaterThan(before.presenter.frontRequestId ?? 0);
    await page.mouse.move(790, 70);
    await page.waitForTimeout(600);
    await page.screenshot({ path: testInfo.outputPath(`dark-${destination.toLowerCase()}-800.png`) });
  }
  await testInfo.attach("sample-semantics", { body: await page.locator("body").ariaSnapshot(), contentType: "text/plain" });
  expect(runtimeErrors).toEqual([]);
});


test("Material sample pointer controls, sheets, dialogs, search and image scrolling", async ({ page, runtimeErrors }, testInfo) => {
  await page.setViewportSize({ width: 1600, height: 900 });
  await openDoroti(page, "&dorotiTestbedMode=sample");
  await visibleFrame(page);
  await testInfo.attach("initial-semantics", { body: await page.locator("body").ariaSnapshot(), contentType: "text/plain" });
  const elevated = page.getByRole("button", { name: "Elevated", exact: true });
  await expect(elevated.first()).not.toHaveAttribute("aria-disabled", "true");
  await expect(elevated.nth(1)).toHaveAttribute("aria-disabled", "true");
  for (const [open, close] of [["Show dialog", "Okay"], ["Show full-screen dialog", "Close"]]) {
    const target = page.getByRole("button", { name: open, exact: true });
    await scrollTo(page, target, 600);
    await page.screenshot({ path: testInfo.outputPath(open.replaceAll(" ", "-") + "-before.png") });
    await pointer(page, target);
    const dismiss = page.getByRole("button", { name: close, exact: true }).last();
    await expect(dismiss).toBeAttached();
    await pointer(page, dismiss);
    await expect(dismiss).not.toBeAttached();
  }
  for (const modal of [true, false]) {
    const open = page.getByRole("button", { name: modal ? "Show modal bottom sheet" : "Show bottom sheet", exact: true });
    await scrollTo(page, open, 600, -1);
    await pointer(page, open);
    await expect(page.getByRole("group", { name: "Share", exact: true })).toBeAttached();
    await page.screenshot({ path: testInfo.outputPath(modal ? "modal-sheet.png" : "persistent-sheet.png") });
    if (modal) await page.mouse.click(300, 100);
    else await pointer(page, page.getByRole("button", { name: "Hide bottom sheet", exact: true }));
    await expect(page.getByRole("group", { name: "Share", exact: true })).not.toBeAttached();
  }
  const search = page.getByRole("textbox", { name: "Search colors", exact: true });
  await scrollTo(page, search, 1250);
  await pointer(page, search);
  await expect(page.getByRole("group", { name: "No search history.", exact: true })).toBeAttached();
  await expect(page.locator("#doroti-ime")).toBeFocused();
  await page.keyboard.type("blu");
  await expect(page.locator("#doroti-ime")).toHaveValue("blu");
  const blue = page.getByRole("button", { name: "blue", exact: true });
  await expect(blue).toBeAttached();
  await pointer(page, blue);
  await expect(page.getByRole("group", { name: /Last selected color is blue/ }).first()).toBeAttached();
  const selectedSearch = page.getByRole("group", { name: "Last selected color is blue", exact: true }).getByRole("textbox");
  await expect(selectedSearch).toHaveValue("blue");
  await pointer(page, selectedSearch);
  await expect(page.locator("#doroti-ime")).toBeFocused();
  await page.keyboard.press("Control+A");
  await page.keyboard.press("Backspace");
  await expect(blue).toBeAttached();
  await page.keyboard.press("Escape");
  await expect(search).toBeAttached();
  await testInfo.attach("final-semantics", { body: await page.locator("body").ariaSnapshot(), contentType: "text/plain" });
  await page.screenshot({ path: testInfo.outputPath("search-complete.png") });
  const frame = await captureDiagnostics(page);
  expect(frame.presenter.rasterDiagnostics?.failedScenes ?? 0, frame.presenter.rasterDiagnostics?.lastFailureReason).toBe(0);
  expect(runtimeErrors).toEqual([]);
});


test("Material sample lazy component inventory and theme images", async ({ page, runtimeErrors }, testInfo) => {
  await page.setViewportSize({ width: 1600, height: 900 });
  await openDoroti(page, "&dorotiTestbedMode=sample");
  await visibleFrame(page);
  for (const x of [600, 1250]) {
    await page.mouse.move(x, 800);
    for (let step = 0; step < 16; step++) {
      await page.mouse.wheel(0, 650);
      await page.waitForTimeout(450);
      await page.screenshot({ path: testInfo.outputPath(`inventory-${x}-${step}.png`) });
    }
  }
  await testInfo.attach("inventory-semantics", { body: await page.locator("body").ariaSnapshot(), contentType: "text/plain" });
  await expect(page.getByRole("radio", { name: "Image URL", exact: true })).toBeAttached();
  // D33 follows Flutter's explicit extraction action. Scrolling into the demo
  // must not start quantization; exercise that action before expecting palettes.
  await expect(page.getByLabel(/Primary\s+#[0-9A-F]{6}/)).toHaveCount(0);
  const extract = page.getByRole("button", { name: "Extract colors", exact: true });
  await scrollTo(page, extract, 1250);
  await pointer(page, extract);
  await expect(page.getByLabel(/Primary\s+#[0-9A-F]{6}/).first()).toBeAttached();
  const frame = await captureDiagnostics(page);
  expect(frame.presenter.rasterDiagnostics?.failedScenes ?? 0, frame.presenter.rasterDiagnostics?.lastFailureReason).toBe(0);
  expect(runtimeErrors).toEqual([]);
});


test("Material sample visited sections preserve local state across a full scroll", async ({ page, runtimeErrors }) => {
  await page.setViewportSize({ width: 1600, height: 900 });
  await openDoroti(page, "&dorotiTestbedMode=sample");
  await visibleFrame(page);
  const outbox = page.getByRole("group", { name: /^Outbox Tab 2 of 7$/ });
  await scrollTo(page, outbox, 1250);
  await pointer(page, outbox);
  await expect(outbox).toHaveAttribute("aria-selected", "true");
  await scrollTo(page, page.getByRole("radio", { name: "Image URL", exact: true }), 1250);
  await scrollTo(page, outbox, 1250, -1);
  await expect(outbox).toHaveAttribute("aria-selected", "true");
  expect(runtimeErrors).toEqual([]);
});

test("Material sample seed and image theme selection preserves latest choice", async ({ page, runtimeErrors }, testInfo) => {
  await page.setViewportSize({ width: 1600, height: 1000 });
  await openDoroti(page, "&dorotiTestbedMode=sample");
  await visibleFrame(page);
  await testInfo.attach("theme-descriptions", { body: JSON.stringify(await page.locator("[aria-description]").evaluateAll(nodes => nodes.map(node => ({ description: node.getAttribute("aria-description"), role: node.getAttribute("role") }))), null, 2), contentType: "application/json" });
  for (const name of ["Indigo", "Blue", "Teal", "Green", "Yellow", "Orange", "Deep Orange", "Pink", "M3 Baseline"]) {
    const control = page.locator(`[aria-description="${name}"]`).first();
    await pointer(page, control);
    await expect(control).toHaveAttribute("aria-selected", "true");
  }
  for (const name of ["Leaves", "Peonies", "Bubbles", "Seaweed", "Sea Grapes", "Petals"]) {
    const control = page.locator(`[aria-description="${name}"]`).first();
    await pointer(page, control);
    await expect(control).toHaveAttribute("aria-selected", "true", { timeout: 60000 });
    await page.screenshot({ path: testInfo.outputPath(`theme-${name.replaceAll(" ", "-")}.png`) });
  }
  const frame = await captureDiagnostics(page);
  expect(frame.presenter.rasterDiagnostics?.failedScenes ?? 0, frame.presenter.rasterDiagnostics?.lastFailureReason).toBe(0);
  expect(runtimeErrors).toEqual([]);
});
