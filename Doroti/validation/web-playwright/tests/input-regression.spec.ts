import { test, expect } from "./helpers/fixtures.js";
import { openDoroti, waitForSettledPresenter } from "./helpers/doroti-diagnostics.js";

test("semantics, pointer, keyboard, and native text endpoint remain available", async ({ page, runtimeErrors }) => {
  await openDoroti(page);
  await expect(page.getByRole("application", { name: "Doroti Material Demo" })).toBeAttached();
  const button = page.getByRole("button", { name: "G6 Material button" });
  await expect(button).toBeAttached();
  await button.evaluate((element) => (element as HTMLElement).click());
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
