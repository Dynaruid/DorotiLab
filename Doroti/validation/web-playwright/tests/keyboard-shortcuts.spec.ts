import { test, expect } from "./helpers/fixtures.js";
import { openDoroti } from "./helpers/doroti-diagnostics.js";

test("browser clipboard host bridge preserves Unicode and empty text", async ({ page, context }) => {
  await context.grantPermissions(["clipboard-read", "clipboard-write"]);
  await openDoroti(page);
  const results = await page.evaluate(async () => {
    const moduleUrl = "/_content/Doroti.Host.Web/doroti.web.js";
    const bridge = await import(moduleUrl);
    await navigator.clipboard.writeText("external 한글 🧪");
    const read = await bridge.readClipboardText();
    await bridge.writeClipboardText("framework 복사");
    const written = await navigator.clipboard.readText();
    await bridge.writeClipboardText("");
    const empty = await bridge.readClipboardText();
    return { read, written, empty };
  });
  expect(results).toEqual({ read: "external 한글 🧪", written: "framework 복사", empty: "" });
});

test("native text clipboard shortcuts and reverse focus traversal", async ({ page, context }) => {
  await context.grantPermissions(["clipboard-read", "clipboard-write"]);
  await openDoroti(page);
  await page.getByRole("textbox", { name: /Text field/ })
    .evaluate(element => (element as HTMLElement).click());
  const input = page.locator("#doroti-ime");
  await expect(input).toBeFocused();
  await input.fill("Doroti 한글");
  await page.keyboard.press("Control+a");
  await expect.poll(() => input.evaluate(element => {
    const field = element as HTMLTextAreaElement;
    return [field.selectionStart, field.selectionEnd];
  })).toEqual([0, 9]);
  await page.keyboard.press("Control+c");
  await expect.poll(() => page.evaluate(() => navigator.clipboard.readText())).toBe("Doroti 한글");
  await page.keyboard.press("Control+x");
  await expect(input).toHaveValue("");
  await page.keyboard.press("Control+v");
  await expect(input).toHaveValue("Doroti 한글");
  await page.keyboard.press("Control+a");
  await page.evaluate(() => navigator.clipboard.writeText("replaced"));
  await page.keyboard.press("Control+v");
  await expect(input).toHaveValue("replaced");
  await page.keyboard.press("Shift+Tab");
  await expect(input).not.toBeFocused();
  await page.keyboard.press("Tab");
  await expect(input).toBeFocused();
});
