import { test, expect } from "./helpers/fixtures.js";
import { attachDiagnostics, openDoroti, waitForSettledPresenter } from "./helpers/doroti-diagnostics.js";

test("CanvasKit TextField preserves inter-word spaces @headed", async ({ page, runtimeErrors }, testInfo) => {
  await openDoroti(page);
  const textField = page.getByRole("textbox", { name: /Text field/ });
  await textField.evaluate((element) => (element as HTMLElement).click());
  await page.locator("#doroti-ime").fill("WW WW");
  await waitForSettledPresenter(page);

  const screenshot = await page.screenshot();
  await testInfo.attach("textfield-space", { body: screenshot, contentType: "image/png" });
  await attachDiagnostics(page, testInfo);

  await expect(page.locator("#doroti-ime")).toHaveValue("WW WW");
  expect(runtimeErrors).toEqual([]);
});
