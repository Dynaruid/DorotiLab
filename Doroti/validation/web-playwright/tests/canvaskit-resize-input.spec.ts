import { test, expect } from "./helpers/fixtures.js";
import { openDoroti, captureDiagnostics, waitForSettledPresenter, assertPresenterContract } from "./helpers/doroti-diagnostics.js";
import { decodeResizePixelMarker } from "./helpers/resize-pixel-marker.js";

const candidate = process.env.DOROTI_RESIZE_EXPERIMENT_QUERY ??
  "&dorotiResizeScheduling=display&dorotiCopyOwnership=owned&dorotiEncodingCache=1&dorotiMetricsCoalescing=frame";
for (const dpr of [1, 1.25, 1.5, 2]) {
  test.describe(`DPR ${dpr}`, () => {
    test.use({ deviceScaleFactor: dpr });
    test("CanvasKit captured frame keeps exact physical geometry", async ({ page, runtimeErrors }) => {
      test.skip(process.env.DOROTI_WEB_RENDERER_MODE !== "worker-canvaskit-webgl");
      await openDoroti(page, `${candidate}&dorotiResizeFixture=F0&dorotiFrameMarker=1`);
      for (const width of [960, 736, 1104, 960]) {
        await page.setViewportSize({ width, height: 640 });
        await expect.poll(async () => (await captureDiagnostics(page)).snapshot.resizeEpoch.logicalWidth).toBe(width);
        const state = await waitForSettledPresenter(page);
        expect(state.snapshot.resizeEpoch.devicePixelRatio).toBe(dpr);
        const marker = decodeResizePixelMarker(await page.locator(".doroti-root").screenshot());
        expect(marker).not.toBeNull();
        expect(marker!.physicalWidth).toBe(width * dpr);
        expect(marker!.physicalHeight).toBe(640 * dpr);
        expect(marker!.generation).toBe(state.snapshot.resizeEpoch.generation);
        assertPresenterContract(state);
      }
      expect(runtimeErrors).toEqual([]);
    });
  });
}

test("CanvasKit embedded resize preserves synthetic composition and input barriers", async ({ page, runtimeErrors }) => {
  test.skip(process.env.DOROTI_WEB_RENDERER_MODE !== "worker-canvaskit-webgl");
  await openDoroti(page, candidate);
  await page.locator(".doroti-root").evaluate(root => {
    Object.assign((root as HTMLElement).style, {
      position: "absolute", left: "32px", top: "24px", width: "1104px", height: "720px",
    });
  });
  await expect.poll(async () => {
    const epoch = (await captureDiagnostics(page)).snapshot.resizeEpoch;
    return [epoch.logicalWidth, epoch.logicalHeight];
  }).toEqual([1104, 720]);
  await waitForSettledPresenter(page);
  const field = await page.getByRole("textbox", { name: /Text field/ }).boundingBox();
  if (!field) throw new Error("Embedded TextField has no geometry");
  await page.mouse.click(field.x + field.width / 2, field.y + field.height / 2);
  const input = page.locator("#doroti-ime");
  await input.fill("");
  await input.evaluate(element => {
    const editor = element as HTMLTextAreaElement;
    editor.dispatchEvent(new CompositionEvent("compositionstart", { bubbles: true }));
    editor.value = "ㅎ";
    editor.setSelectionRange(1, 1);
    editor.dispatchEvent(new CompositionEvent("compositionupdate", { data: "ㅎ", bubbles: true }));
    editor.dispatchEvent(new InputEvent("input", { data: "ㅎ", isComposing: true, bubbles: true }));
  });
  await page.locator(".doroti-root").evaluate(root => { (root as HTMLElement).style.width = "960px"; });
  await expect.poll(async () => (await captureDiagnostics(page)).snapshot.resizeEpoch.logicalWidth).toBe(960);
  await waitForSettledPresenter(page);
  await expect(input).toHaveValue("ㅎ");
  await input.evaluate(element => {
    const editor = element as HTMLTextAreaElement;
    editor.value = "한글";
    editor.setSelectionRange(2, 2);
    editor.dispatchEvent(new CompositionEvent("compositionend", { data: "한글", bubbles: true }));
    editor.dispatchEvent(new InputEvent("input", { data: "한글", bubbles: true }));
  });
  await expect(page.getByRole("textbox", { name: /Text field/ })).toHaveValue("한글");
  await page.keyboard.press("Tab");
  const state = await waitForSettledPresenter(page);
  expect(state.snapshot.resizeEpoch.logicalWidth).toBe(960);
  expect(state.snapshot.resizeEpoch.logicalHeight).toBe(720);
  expect(runtimeErrors).toEqual([]);
  assertPresenterContract(state);
  // Synthetic browser composition events prove ordering, not physical OS IME.
});
