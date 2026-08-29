import { test, expect } from "./helpers/fixtures.js";
import { assertPresenterContract, openDoroti } from "./helpers/doroti-diagnostics.js";

test("Release app reaches an exact hardware-WebGL2 front", async ({ page, runtimeErrors }) => {
  const bundle = await openDoroti(page);
  expect(runtimeErrors).toEqual([]);
  expect(await page.locator("html").getAttribute("data-doroti-bootstrap-stage")).toBe("started");
  expect(bundle.presenter.frontGeneration).toBe(bundle.snapshot.resizeEpoch.generation);
  expect(bundle.snapshot.resizeEpoch.physicalWidth).toBe(Math.round(bundle.snapshot.logicalWidth * bundle.snapshot.devicePixelRatio));
  expect(bundle.snapshot.resizeEpoch.physicalHeight).toBe(Math.round(bundle.snapshot.logicalHeight * bundle.snapshot.devicePixelRatio));
  assertPresenterContract(bundle);
});
