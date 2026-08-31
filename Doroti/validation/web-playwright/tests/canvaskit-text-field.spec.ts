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

test("CanvasKit TextField preserves inter-word spaces @headed", async ({ page, runtimeErrors }, testInfo) => {
  await openDoroti(page);
  const textField = page.getByRole("textbox", { name: /Text field/ });
  const clearFocus = page.getByRole("button", { name: "Clear focus" });

  const renderValue = async (value: string, attachment: string) => {
    await textField.evaluate((element) => (element as HTMLElement).click());
    await page.locator("#doroti-ime").fill(value);
    await expect(textField).toHaveValue(value);
    await clearFocus.evaluate((element) => (element as HTMLElement).click());
    await waitForSettledPresenter(page);
    const screenshot = await textField.screenshot();
    await testInfo.attach(attachment, { body: screenshot, contentType: "image/png" });
    return rightmostNeutralInk(screenshot);
  };

  const shortRight = await renderValue("WW", "textfield-two-glyphs");
  const spacedRight = await renderValue("WW WW", "textfield-inter-word-space");

  const screenshot = await page.screenshot();
  await testInfo.attach("textfield-space", { body: screenshot, contentType: "image/png" });
  await attachDiagnostics(page, testInfo);

  await expect(textField).toHaveValue("WW WW");
  expect(shortRight, "two-glyph reference has visible neutral text ink").toBeGreaterThan(0);
  expect(spacedRight - shortRight,
    "the word after the inter-word space is rasterized, not merely retained in semantics")
    .toBeGreaterThan(30);
  expect(runtimeErrors).toEqual([]);
});
