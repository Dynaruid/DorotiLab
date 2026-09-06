import { test, expect } from "@playwright/test";

test("Flutter reference image demo displays asset and URL palettes", async ({ page }, testInfo) => {
  test.skip(process.env.DOROTI_FLUTTER_IMAGE_DEMO !== "1", "Requires the reference main.dart Web build on DOROTI_WEB_BASE_URL.");
  await page.setViewportSize({ width: 1280, height: 1000 });
  await page.goto("/");
  await page.locator("flt-semantics-placeholder").dispatchEvent("click");
  await page.mouse.move(1080, 600);
  // The second Components column owns its own scrolling surface.
  for (let attempt = 0; attempt < 8; attempt++) {
    await page.mouse.wheel(0, 1800);
    if (await page.getByRole("button", { name: "Extract colors", exact: true }).isVisible()) break;
    await page.waitForTimeout(250);
  }
  const demo = page.getByRole("group", { name: /^Image demo/ });
  await expect(demo).toBeVisible();
  await page.getByRole("button", { name: "Extract colors", exact: true }).dispatchEvent("click");
  await expect(demo).toHaveAccessibleName(/Light palette.*Dark palette/s, { timeout: 45_000 });
  await page.mouse.wheel(0, 700);
  await page.waitForTimeout(250);
  await page.screenshot({ path: testInfo.outputPath("local-palette.png") });
  await page.getByRole("button", { name: "Image URL", exact: true }).dispatchEvent("click");
  await expect(demo).not.toHaveAccessibleName(/Light palette/);
  await page.getByRole("button", { name: "Extract colors", exact: true }).dispatchEvent("click");
  await expect(demo).toHaveAccessibleName(/Light palette.*Dark palette/s, { timeout: 45_000 });
  await expect(page.getByRole("img", { name: "Photo loaded from the Unsplash image URL" })).toBeVisible();
  await expect(page.getByText("Image could not be loaded.", { exact: true })).toHaveCount(0);
  await page.mouse.wheel(0, 700);
  await page.waitForTimeout(250);
  await page.screenshot({ path: testInfo.outputPath("url-palette.png") });
});
