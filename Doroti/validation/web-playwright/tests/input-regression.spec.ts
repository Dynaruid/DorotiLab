import { PNG } from "pngjs";
import { test, expect } from "./helpers/fixtures.js";
import { captureDiagnostics, openDoroti, waitForSettledPresenter } from "./helpers/doroti-diagnostics.js";

function countPixelDifferences(first: Buffer, second: Buffer): number {
  const left = PNG.sync.read(first);
  const right = PNG.sync.read(second);
  expect({ width: left.width, height: left.height }).toEqual({ width: right.width, height: right.height });
  let differences = 0;
  for (let offset = 0; offset < left.data.length; offset += 4) {
    if (Math.abs(left.data[offset] - right.data[offset]) > 1 ||
        Math.abs(left.data[offset + 1] - right.data[offset + 1]) > 1 ||
        Math.abs(left.data[offset + 2] - right.data[offset + 2]) > 1 ||
        Math.abs(left.data[offset + 3] - right.data[offset + 3]) > 1) differences++;
  }
  return differences;
}

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

test("native editable selection state does not paint over the Canvas TextField @headed", async ({
  page,
  runtimeErrors,
}) => {
  await page.emulateMedia({ colorScheme: "dark" });
  await openDoroti(page);
  const textField = page.getByRole("textbox", { name: /Text field/ });
  const fieldBounds = await textField.boundingBox();
  if (!fieldBounds) throw new Error("Doroti TextField semantics node has no browser bounds.");
  await page.mouse.click(fieldBounds.x + fieldBounds.width / 2, fieldBounds.y + fieldBounds.height / 2);

  const input = page.locator("#doroti-ime");
  await expect(input).toBeVisible();
  await input.fill("1231231234");
  const beforeSelection = await waitForSettledPresenter(page);
  await input.evaluate((element) => {
    const editable = element as HTMLTextAreaElement;
    editable.focus({ preventScroll: true });
    editable.setSelectionRange(1, 8, "forward");
    editable.dispatchEvent(new Event("select", { bubbles: true }));
    document.dispatchEvent(new Event("selectionchange"));
  });
  await expect.poll(async () => input.evaluate((element) => {
    const editable = element as HTMLTextAreaElement;
    return [editable.selectionStart, editable.selectionEnd];
  })).toEqual([1, 8]);
  await expect.poll(async () => (await captureDiagnostics(page)).presenter.frontRequestId ?? 0)
    .toBeGreaterThan(beforeSelection.presenter.frontRequestId ?? 0);
  await waitForSettledPresenter(page);

  const inputBounds = await input.boundingBox();
  if (!inputBounds) throw new Error("Doroti native text endpoint has no browser bounds.");
  const clip = {
    x: Math.floor(inputBounds.x),
    y: Math.floor(inputBounds.y),
    width: Math.ceil(inputBounds.width),
    height: Math.ceil(inputBounds.height),
  };
  await page.evaluate(() => new Promise<void>((resolve) => requestAnimationFrame(() => resolve())));
  const nativeLayerAttached = await page.screenshot({ clip });
  const nativeStyle = await input.evaluate((element) => {
    const style = getComputedStyle(element);
    const selection = getComputedStyle(element, "::selection");
    // Opacity suppresses the native layer without blurring the active editor.
    // visibility:hidden closes the browser text connection and changes the
    // framework-owned Canvas selection, invalidating this paint comparison.
    element.style.opacity = "0";
    return {
      filter: style.filter,
      opacity: style.opacity,
      color: style.color,
      selectionColor: selection.color,
      selectionBackground: selection.backgroundColor,
    };
  });
  await page.evaluate(() => new Promise<void>((resolve) => requestAnimationFrame(() => resolve())));
  const nativeLayerHidden = await page.screenshot({ clip });
  await input.evaluate((element) => element.style.removeProperty("opacity"));

  const differingPixels = countPixelDifferences(nativeLayerAttached, nativeLayerHidden);
  console.log("NATIVE_SELECTION_VISUAL", JSON.stringify({ differingPixels, nativeStyle }));
  expect(nativeStyle.filter).toBe("opacity(0)");
  expect(differingPixels).toBe(0);
  expect(runtimeErrors).toEqual([]);
});

test("status below the TextField is not an I-beam or selection target @headed", async ({
  page,
  runtimeErrors,
}) => {
  await openDoroti(page);
  const textField = page.getByRole("textbox", { name: /Text field/ });
  const status = page.locator('[data-doroti-semantics-identifier="text-field-status"]');
  await expect(status).toBeAttached();

  const fieldBounds = await textField.boundingBox();
  const initialStatusBounds = await status.boundingBox();
  if (!fieldBounds || !initialStatusBounds) throw new Error("TextField boundary semantics are unavailable.");
  expect(initialStatusBounds.y + initialStatusBounds.height / 2)
    .toBeGreaterThan(fieldBounds.y + fieldBounds.height);

  await page.mouse.move(fieldBounds.x + Math.min(120, fieldBounds.width / 3), fieldBounds.y + fieldBounds.height / 2);
  await expect.poll(async () => page.locator(".doroti-root")
    .evaluate((element) => getComputedStyle(element).cursor)).toBe("text");

  await page.mouse.move(initialStatusBounds.x + initialStatusBounds.width / 2,
    initialStatusBounds.y + initialStatusBounds.height / 2);
  await expect.poll(async () => page.locator(".doroti-root")
    .evaluate((element) => getComputedStyle(element).cursor)).not.toBe("text");

  await page.mouse.click(fieldBounds.x + fieldBounds.width / 2, fieldBounds.y + fieldBounds.height / 2);
  const input = page.locator("#doroti-ime");
  await expect(input).toBeVisible();
  await input.fill("Doroti boundary selection");
  const textLength = await input.evaluate((element) => (element as HTMLTextAreaElement).value.length);
  await input.evaluate((element) => {
    const editable = element as HTMLTextAreaElement;
    editable.setSelectionRange(editable.value.length, editable.value.length);
  });

  const statusBounds = await status.boundingBox();
  if (!statusBounds) throw new Error("TextField status has no browser bounds.");
  const statusY = statusBounds.y + statusBounds.height / 2;
  await page.mouse.move(statusBounds.x + statusBounds.width - 4, statusY);
  await page.mouse.down({ button: "left" });
  await page.mouse.move(statusBounds.x + 4, statusY, { steps: 8 });
  await page.mouse.up({ button: "left" });

  const afterDrag = await input.evaluate((element) => {
    const editable = element as HTMLTextAreaElement;
    return {
      hidden: editable.hidden,
      selectionStart: editable.selectionStart ?? 0,
      selectionEnd: editable.selectionEnd ?? 0,
    };
  });
  expect(afterDrag.hidden ||
    (afterDrag.selectionStart === textLength && afterDrag.selectionEnd === textLength)).toBe(true);
  expect(runtimeErrors).toEqual([]);
});

test("browser-native context menu targets the active editable @headed", async ({ page, runtimeErrors }) => {
  await openDoroti(page);
  const textField = page.getByRole("textbox", { name: /Text field/ });
  const fieldBounds = await textField.boundingBox();
  if (!fieldBounds) throw new Error("Doroti TextField semantics node has no browser bounds.");

  await page.mouse.click(fieldBounds.x + fieldBounds.width / 2, fieldBounds.y + fieldBounds.height / 2);
  const input = page.locator("#doroti-ime");
  await expect(input).toBeVisible();
  const readNativeEditingContract = () => input.evaluate((element) => {
    const editable = element as HTMLTextAreaElement;
    return {
      autocomplete: editable.autocomplete,
      autocorrect: editable.getAttribute("autocorrect"),
      fontFamily: editable.style.fontFamily,
      fontSize: editable.style.fontSize,
      fontWeight: editable.style.fontWeight,
      lineHeight: editable.style.lineHeight,
      textAlign: editable.style.textAlign,
    };
  });
  await expect.poll(readNativeEditingContract).toMatchObject({
    autocomplete: "off",
    autocorrect: "on",
    fontWeight: "400",
    textAlign: "start",
  });
  const nativeEditingContract = await readNativeEditingContract();
  expect(nativeEditingContract.fontFamily).not.toBe("");
  expect(Number.parseFloat(nativeEditingContract.fontSize)).toBeGreaterThan(0);
  expect(Number.parseFloat(nativeEditingContract.lineHeight)).toBeGreaterThan(0);
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
  await expect.poll(async () => input.evaluate((element) => getComputedStyle(element).cursor)).toBe("text");
  await page.mouse.down({ button: "left" });
  await page.mouse.move(selectionEndX, textY, { steps: 8 });
  await expect.poll(async () => input.evaluate((element) => ({
    root: getComputedStyle(element.closest(".doroti-root") as HTMLElement).cursor,
    editable: getComputedStyle(element).cursor,
  }))).toEqual({ root: "text", editable: "text" });
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
