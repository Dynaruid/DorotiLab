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
