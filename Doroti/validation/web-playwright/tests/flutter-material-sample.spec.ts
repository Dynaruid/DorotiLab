import { test, expect } from "@playwright/test";

test("Flutter pinned Material sample reference screens", async ({ page }, testInfo) => {
  test.skip(process.env.DOROTI_FLUTTER_REFERENCE !== "1", "Requires the separately built pinned Flutter main.dart reference.");
  const errors: string[] = [];
  page.on("pageerror", error => errors.push(error.message));
  await page.setViewportSize({ width: 800, height: 900 });
  await page.goto("/");
  await page.locator("flt-semantics-placeholder").dispatchEvent("click");
  await expect(page.getByText("Flutter Material 3", { exact: true })).toBeAttached();
  await testInfo.attach("reference-semantics", { body: await page.locator("body").ariaSnapshot(), contentType: "text/plain" });
  for (const width of [800, 1600]) {
    await page.setViewportSize({ width, height: 900 });
    await page.waitForTimeout(1500);
    for (const destination of ["Components", "Color", "Typography", "Elevation"]) {
      const control = page.getByRole("tab", { name: destination, exact: true }).or(page.getByRole("button", { name: new RegExp(`^${destination}(?:\\s|$)`) })).first();
      await expect(control).toBeAttached();
      const box = (await control.boundingBox())!;
      await page.mouse.click(box.x + box.width / 2, box.y + box.height / 2);
      await page.waitForTimeout(600);
      await page.screenshot({ path: testInfo.outputPath(`reference-${destination.toLowerCase()}-${width}.png`) });
    }
  }
  await page.setViewportSize({ width: 800, height: 900 });
  await page.waitForTimeout(1500);
  const brightness = (await page.getByRole("button", { name: "", exact: true }).first().boundingBox())!;
  await page.mouse.click(brightness.x + brightness.width / 2, brightness.y + brightness.height / 2);
  for (const destination of ["Components", "Color", "Typography", "Elevation"]) {
    const control = page.getByRole("tab", { name: destination, exact: true }).or(page.getByRole("button", { name: new RegExp(`^${destination}(?:\\s|$)`) })).first();
    const box = (await control.boundingBox())!;
    await page.mouse.click(box.x + box.width / 2, box.y + box.height / 2);
    await page.mouse.move(790, 70);
    await page.waitForTimeout(600);
    await page.screenshot({ path: testInfo.outputPath(`reference-dark-${destination.toLowerCase()}-800.png`) });
  }
  expect(errors).toEqual([]);
});
