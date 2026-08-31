import { test, expect } from "./helpers/fixtures.js";
import { captureDiagnostics, openDoroti, waitForSettledPresenter } from "./helpers/doroti-diagnostics.js";

test("semantics, pointer, keyboard, and native text endpoint remain available", async ({ page, runtimeErrors }) => {
  const initial = await openDoroti(page);
  await expect(page.getByRole("application", { name: "Doroti Material Demo" })).toBeAttached();
  const button = page.getByRole("button", { name: "G6 Material button" });
  await expect(button).toBeAttached();
  const clickStarted = await page.evaluate(() => performance.now());
  await button.evaluate((element) => (element as HTMLElement).click());
  await expect(page.getByRole("button", { name: "G6 Material button 1" })).toBeAttached();
  await expect.poll(async () => (await captureDiagnostics(page)).presenter.frontRequestId ?? 0)
    .toBeGreaterThan(initial.presenter.frontRequestId ?? 0);
  const afterButton = await captureDiagnostics(page);
  const clickCommitted = await page.evaluate(() => performance.now());
  console.log("INPUT_FRONT_LATENCY", JSON.stringify({ milliseconds: clickCommitted - clickStarted }));
  expect(clickCommitted - clickStarted).toBeLessThan(1_000);
  expect(afterButton.presenter.frontGeneration).toBe(initial.presenter.frontGeneration);
  const warmClickStarted = await page.evaluate(() => performance.now());
  await page.getByRole("button", { name: "G6 Material button 1" })
    .evaluate((element) => (element as HTMLElement).click());
  await expect(page.getByRole("button", { name: "G6 Material button 2" })).toBeAttached();
  await expect.poll(async () => (await captureDiagnostics(page)).presenter.frontRequestId ?? 0)
    .toBeGreaterThan(afterButton.presenter.frontRequestId ?? 0);
  const warmClickCommitted = await page.evaluate(() => performance.now());
  console.log("WARM_INPUT_FRONT_LATENCY", JSON.stringify({ milliseconds: warmClickCommitted - warmClickStarted }));
  expect(warmClickCommitted - warmClickStarted).toBeLessThan(250);
  const canvas = page.locator("#doroti-surface");
  const bounds = await canvas.boundingBox();
  if (!bounds) throw new Error("Doroti canvas has no browser bounds.");
  await page.mouse.click(bounds.x + 8, bounds.y + 8);
  const textField = page.getByRole("textbox", { name: /Text field/ });
  await textField.evaluate((element) => (element as HTMLElement).click());
  await page.locator("#doroti-ime").fill("Doroti 한국어");
  await page.keyboard.press("Tab");
  const bundle = await waitForSettledPresenter(page);
  expect(runtimeErrors).toEqual([]);
  expect(bundle.trace.filter((entry) => entry.phase === "text-editing-dispatched").length).toBeGreaterThan(0);
});

test("browser-native context menu targets the active editable @headed", async ({ page, runtimeErrors }) => {
  await openDoroti(page);
  const textField = page.getByRole("textbox", { name: /Text field/ });
  const fieldBounds = await textField.boundingBox();
  if (!fieldBounds) throw new Error("Doroti TextField semantics node has no browser bounds.");

  await page.mouse.click(fieldBounds.x + fieldBounds.width / 2, fieldBounds.y + fieldBounds.height / 2);
  const input = page.locator("#doroti-ime");
  await expect(input).toBeVisible();
  await input.fill("Doroti browser context menu");
  await expect(input).toHaveValue("Doroti browser context menu");
  await expect.poll(async () => (await captureDiagnostics(page)).trace
    .filter((entry) => entry.phase === "text-editing-dispatched").length).toBeGreaterThan(0);

  const inputBounds = await input.boundingBox();
  if (!inputBounds) throw new Error("Doroti native text endpoint has no browser bounds.");
  expect(inputBounds.width).toBeGreaterThan(100);
  expect(inputBounds.height).toBeGreaterThan(20);

  const textY = inputBounds.y + inputBounds.height / 2;
  const selectionStartX = inputBounds.x + Math.min(180, inputBounds.width - 12);
  const selectionEndX = inputBounds.x + Math.min(45, inputBounds.width / 4);
  await page.mouse.move(selectionStartX, textY);
  await page.mouse.down({ button: "left" });
  await page.mouse.move(selectionEndX, textY, { steps: 8 });
  await page.mouse.up({ button: "left" });
  await expect.poll(async () => input.evaluate((element) => {
    const editable = element as HTMLTextAreaElement;
    return Math.abs((editable.selectionEnd ?? 0) - (editable.selectionStart ?? 0));
  })).toBeGreaterThan(0);

  await page.evaluate(() => {
    (window as typeof window & { __dorotiContextMenuProbe?: unknown }).__dorotiContextMenuProbe = null;
    document.addEventListener("contextmenu", (event) => {
      const target = event.target;
      queueMicrotask(() => {
        (window as typeof window & { __dorotiContextMenuProbe?: unknown }).__dorotiContextMenuProbe = {
          defaultPrevented: event.defaultPrevented,
          targetId: target instanceof HTMLElement ? target.id : null,
          targetTag: target instanceof HTMLElement ? target.tagName : null,
          value: target instanceof HTMLTextAreaElement ? target.value : null,
          selectionStart: target instanceof HTMLTextAreaElement ? target.selectionStart : null,
          selectionEnd: target instanceof HTMLTextAreaElement ? target.selectionEnd : null,
        };
      });
    }, { once: true });
  });

  await page.mouse.click(
    Math.min(selectionStartX, selectionEndX) + Math.abs(selectionStartX - selectionEndX) / 2,
    textY,
    { button: "right" });
  await expect.poll(async () => page.evaluate(() =>
    (window as typeof window & { __dorotiContextMenuProbe?: unknown }).__dorotiContextMenuProbe))
    .not.toBeNull();
  const contextMenu = await page.evaluate(() =>
    (window as typeof window & { __dorotiContextMenuProbe?: unknown }).__dorotiContextMenuProbe);
  expect(contextMenu).toMatchObject({
    defaultPrevented: false,
    targetId: "doroti-ime",
    targetTag: "TEXTAREA",
    value: "Doroti browser context menu",
  });
  expect((contextMenu as { selectionEnd: number }).selectionEnd)
    .toBeGreaterThan((contextMenu as { selectionStart: number }).selectionStart);
  await page.keyboard.press("Escape");

  await expect(page.getByRole("button", { name: /^(Copy|Cut|Paste|복사|잘라내기|붙여넣기)$/i })).toHaveCount(0);
  expect(runtimeErrors).toEqual([]);
});
