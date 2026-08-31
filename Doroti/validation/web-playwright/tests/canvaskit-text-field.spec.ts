import { test, expect } from "./helpers/fixtures.js";
import { attachDiagnostics, openDoroti, waitForSettledPresenter } from "./helpers/doroti-diagnostics.js";
import { PNG } from "pngjs";

function rightmostNeutralInk(screenshot: Buffer): number {
  const image = PNG.sync.read(screenshot);
  const top = Math.floor(image.height * 0.2);
  // The semantics input bounds include helper text below the outline. Restrict
  // the scan to the editable line so "Entered: ..." cannot satisfy the pixel gate.
  const bottom = Math.ceil(image.height * 0.6);
  const right = Math.min(image.width, 300);
  let result = -1;
  for (let y = top; y < bottom; y++) {
    for (let x = 0; x < right; x++) {
      const offset = (y * image.width + x) * 4;
      const red = image.data[offset];
      const green = image.data[offset + 1];
      const blue = image.data[offset + 2];
      const alpha = image.data[offset + 3];
      const maximum = Math.max(red, green, blue);
      const minimum = Math.min(red, green, blue);
      if (alpha > 200 && maximum < 155 && maximum - minimum < 28)
        result = Math.max(result, x);
    }
  }
  return result;
}

function focusedCaretBounds(screenshot: Buffer): { top: number; bottom: number } | null {
  const image = PNG.sync.read(screenshot);
  const top = Math.floor(image.height * 0.2);
  const bottom = Math.ceil(image.height * 0.6);
  const right = Math.min(image.width, 300);
  let textRight = -1;
  for (let y = top; y < bottom; y++) {
    for (let x = 20; x < right; x++) {
      const offset = (y * image.width + x) * 4;
      const red = image.data[offset];
      const green = image.data[offset + 1];
      const blue = image.data[offset + 2];
      const alpha = image.data[offset + 3];
      const maximum = Math.max(red, green, blue);
      const minimum = Math.min(red, green, blue);
      if (alpha > 200 && maximum < 155 && maximum - minimum < 28) {
        textRight = Math.max(textRight, x);
      }
    }
  }
  if (textRight < 0) return null;

  let caretTop = image.height;
  let caretBottom = -1;
  let caretPixels = 0;
  for (let y = Math.floor(image.height * 0.08); y < bottom; y++) {
    for (let x = textRight + 1; x <= Math.min(textRight + 12, image.width - 1); x++) {
      const offset = (y * image.width + x) * 4;
      const red = image.data[offset];
      const green = image.data[offset + 1];
      const blue = image.data[offset + 2];
      const alpha = image.data[offset + 3];
      if (alpha > 200 && blue - red > 20 && blue - green > 20 && red < 180 && green < 160) {
        caretPixels++;
        caretTop = Math.min(caretTop, y);
        caretBottom = Math.max(caretBottom, y);
      }
    }
  }
  if (caretPixels < 8 || caretBottom < caretTop) return null;
  return { top: caretTop, bottom: caretBottom };
}

test("CanvasKit TextField preserves spaces and strut-aligns the caret @headed", async ({ page, runtimeErrors }, testInfo) => {
  await openDoroti(page);
  const textField = page.getByRole("textbox", { name: /Text field/ });
  const clearFocus = page.getByRole("button", { name: "Clear focus" });

  const renderValue = async (value: string, attachment: string) => {
    let caretBounds: { top: number; bottom: number } | null = null;
    await textField.evaluate((element) => (element as HTMLElement).click());
    await page.locator("#doroti-ime").fill(value);
    await expect(textField).toHaveValue(value);
    if (value === "WW WW") {
      await waitForSettledPresenter(page);
      for (let attempt = 0; attempt < 12 && caretBounds === null; attempt++) {
        const focusedScreenshot = await textField.screenshot();
        caretBounds = focusedCaretBounds(focusedScreenshot);
        if (caretBounds !== null) {
          await testInfo.attach("textfield-focused-caret", {
            body: focusedScreenshot,
            contentType: "image/png",
          });
        } else {
          await page.waitForTimeout(50);
        }
      }
    }
    await clearFocus.evaluate((element) => (element as HTMLElement).click());
    await waitForSettledPresenter(page);
    const screenshot = await textField.screenshot();
    await testInfo.attach(attachment, { body: screenshot, contentType: "image/png" });
    return { right: rightmostNeutralInk(screenshot), caretBounds };
  };

  const short = await renderValue("WW", "textfield-two-glyphs");
  const spaced = await renderValue("WW WW", "textfield-inter-word-space");

  const screenshot = await page.screenshot();
  await testInfo.attach("textfield-space", { body: screenshot, contentType: "image/png" });
  await attachDiagnostics(page, testInfo);

  await expect(textField).toHaveValue("WW WW");
  expect(short.right, "two-glyph reference has visible neutral text ink").toBeGreaterThan(0);
  expect(spaced.right - short.right,
    "the word after the inter-word space is rasterized, not merely retained in semantics")
    .toBeGreaterThan(30);
  expect(spaced.caretBounds, "focused caret is visible in the editable line").not.toBeNull();
  const devicePixelRatio = await page.evaluate(() => globalThis.devicePixelRatio);
  // The document renderer/reference fixture starts the Material caret 12 logical
  // pixels below the semantics bounds. CanvasKit tight bounds started it about
  // 2 logical pixels too high; the strut rectangle restores the same inset.
  expect(Math.abs(((spaced.caretBounds?.top ?? Number.POSITIVE_INFINITY) / devicePixelRatio) - 12),
    "caret uses the strut top instead of the tighter, negative glyph top")
    .toBeLessThanOrEqual(0.5);
  expect(runtimeErrors).toEqual([]);
});
