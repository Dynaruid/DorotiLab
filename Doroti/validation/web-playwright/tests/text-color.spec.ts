import { test, expect } from "./helpers/fixtures.js";
import { attachDiagnostics, openDoroti, waitForSettledPresenter } from "./helpers/doroti-diagnostics.js";
import { PNG } from "pngjs";

function brightNeutralInk(screenshot: Buffer): number {
  const image = PNG.sync.read(screenshot);
  const top = Math.floor(image.height * 0.2);
  const bottom = Math.ceil(image.height * 0.6);
  const right = Math.min(image.width, 360);
  let count = 0;
  for (let y = top; y < bottom; y++) {
    for (let x = 0; x < right; x++) {
      const offset = (y * image.width + x) * 4;
      const red = image.data[offset];
      const green = image.data[offset + 1];
      const blue = image.data[offset + 2];
      const alpha = image.data[offset + 3];
      const maximum = Math.max(red, green, blue);
      const minimum = Math.min(red, green, blue);
      if (alpha > 220 && minimum > 175 && maximum - minimum < 35)
        count++;
    }
  }
  return count;
}

test("dark-theme TextField foreground reaches the selected renderer", async ({ page, runtimeErrors }, testInfo) => {
  await page.emulateMedia({ colorScheme: "dark" });
  await openDoroti(page);
  expect(await page.evaluate(() => matchMedia("(prefers-color-scheme: dark)").matches)).toBe(true);
  const textField = page.getByRole("textbox", { name: /Text field/ });
  const clearFocus = page.getByRole("button", { name: "Clear focus" });

  await textField.evaluate((element) => (element as HTMLElement).click());
  await page.locator("#doroti-ime").fill("Dark mode text");
  await expect(textField).toHaveValue("Dark mode text");
  await clearFocus.evaluate((element) => (element as HTMLElement).click());
  await waitForSettledPresenter(page);

  const screenshot = await textField.screenshot();
  await testInfo.attach("dark-theme-text-field", { body: screenshot, contentType: "image/png" });
  await attachDiagnostics(page, testInfo);

  expect(brightNeutralInk(screenshot),
    "dark-theme editable text is visibly light instead of falling back to black")
    .toBeGreaterThan(40);
  expect(runtimeErrors).toEqual([]);
});
