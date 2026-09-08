import { test, expect } from "@playwright/test";
import { readFile } from "node:fs/promises";
import path from "node:path";

// Run dotnet build Doroti/src/Doroti.Host.Web first. The compiled production
// module handles all DOM and actions; the fixture replaces only the GPU/managed
// runtime boundary, so no WASM app or renderer startup is needed for this test.
test("semantics projection does not echo input and keeps live assistive actions", async ({ page }) => {
  const output = path.resolve(process.cwd(), "../../src/Doroti.Host.Web/obj/Debug/net10.0/Doroti.Web/wwwroot");
  const errors: string[] = [];
  page.on("pageerror", error => errors.push(error.message));
  await page.route("http://semantics.test/**", async route => {
    const name = new URL(route.request().url()).pathname.slice(1);
    if (!name) {
      await route.fulfill({ contentType: "text/html", body: '<main class="doroti-root" style="width:500px;height:300px"><canvas id="doroti-surface"></canvas><textarea id="doroti-ime" hidden></textarea><div id="doroti-semantics"></div></main>' });
      return;
    }
    if (!/^doroti\.[a-z.-]+\.js$/.test(name)) { await route.fulfill({ status: 404 }); return; }
    let body = await readFile(path.join(output, name), "utf8");
    if (name === "doroti.web.js") body += `
      export function createSemanticsFixture(callbacks) {
        configureManagedCallbacks(callbacks);
        directWorkerBootstrap = true;
        try { createHost(901, "doroti-surface", 500, 300); }
        finally { directWorkerBootstrap = false; }
      }`;
    await route.fulfill({ contentType: "application/javascript", body });
  });
  await page.goto("http://semantics.test/");
  const results = await page.evaluate(async () => {
    const url = "http://semantics.test/doroti.web.js";
    const module = await import(url);
    const calls: { id: number; action: number; args: unknown }[] = [];
    const callbacks = Object.fromEntries([
      "dispatchAnimationFrame", "dispatchSnapshot", "dispatchResizeEpoch", "dispatchPointerBatch", "dispatchWheel",
      "dispatchKey", "dispatchFocus", "dispatchTextEditing", "dispatchTextAction", "dispatchTextConnectionClosed",
    ].map(name => [name, () => {}]));
    callbacks.dispatchSemanticsAction = (...args: unknown[]) => calls.push({ id: args[1] as number, action: args[2] as number, args: JSON.parse(args[4] as string) });
    module.createSemanticsFixture(callbacks);
    const field = { id: 1, rect: [0, 0, 200, 40], value: "alpha", textSelectionBase: 1, textSelectionExtent: 3,
      flags: { textField: true, enabled: true }, actions: (1 << 21) | (1 << 11) };
    const button = { id: 2, rect: [0, 50, 200, 90], flags: { enabled: true }, actions: 1 };
    const slider = { id: 3, rect: [0, 100, 200, 140], role: "slider", value: "5", flags: { enabled: true }, actions: (1 << 6) | (1 << 7) };
    let generation = 0;
    const update = (nodes: unknown[]) => module.updateSemantics(901, JSON.stringify({ generation: ++generation, nodes }));
    const idle = () => new Promise(resolve => setTimeout(resolve, 50));
    update([field, button, slider]);
    const input = document.getElementById("doroti-semantics-901-1") as HTMLInputElement;
    const nativeButton = document.getElementById("doroti-semantics-901-2")!;
    const nativeSlider = document.getElementById("doroti-semantics-901-3")!;
    await idle();
    input.dispatchEvent(new Event("input"));
    input.dispatchEvent(new Event("select"));
    const projection = calls.splice(0);
    const selection = [input.selectionStart, input.selectionEnd];
    // A synchronous event from another node during attribute projection must
    // also be suppressed, not merely events on the node currently being set.
    const setAttribute = HTMLElement.prototype.setAttribute;
    HTMLElement.prototype.setAttribute = function (name, value) {
      if (this === nativeSlider && name === "aria-valuenow") nativeButton.click();
      return setAttribute.call(this, name, value);
    };
    try { update([field, button, { ...slider, value: "6" }]); }
    finally { HTMLElement.prototype.setAttribute = setAttribute; }
    const crossNodeProjection = calls.splice(0);
    update([{ ...field, value: "alpha\nbeta", textSelectionBase: 100, textSelectionExtent: 100 }, button, slider]);
    await idle();
    input.dispatchEvent(new Event("input")); input.dispatchEvent(new Event("select"));
    const normalizedProjection = calls.splice(0);
    update([field, button, slider]);
    await idle();

    nativeButton.click();
    nativeSlider.dispatchEvent(new KeyboardEvent("keydown", { key: "ArrowUp", bubbles: true }));
    input.value = "edited";
    input.dispatchEvent(new Event("input"));
    input.setSelectionRange(1, 2, "backward");
    input.dispatchEvent(new Event("select"));
    const live = calls.splice(0);
    input.value = field.value; input.dispatchEvent(new Event("input"));
    input.setSelectionRange(1, 3); input.dispatchEvent(new Event("select"));
    const rapidReturn = calls.splice(0);
    input.dispatchEvent(new Event("input")); input.dispatchEvent(new Event("select"));
    const repeatedInput = calls.splice(0);
    update([{ ...field, flags: { ...field.flags, readOnly: true } },
      { ...button, flags: { enabled: false } }, { ...slider, flags: { enabled: false } }]);
    await idle(); calls.length = 0;
    nativeButton.click();
    nativeSlider.dispatchEvent(new KeyboardEvent("keydown", { key: "ArrowUp" }));
    input.value = "blocked"; input.dispatchEvent(new Event("input"));
    const blocked = calls.splice(0);
    update([{ ...field, actions: 0 }]);
    await idle(); calls.length = 0;
    input.value = "unsupported"; input.dispatchEvent(new Event("input"));
    const unsupported = calls.splice(0);
    update([]);
    nativeButton.click(); input.dispatchEvent(new Event("input"));
    const removed = calls.splice(0);
    module.closeHost(901);
    return { projection, selection, crossNodeProjection, normalizedProjection, live, rapidReturn, repeatedInput, blocked, unsupported, removed };
  });
  expect(results.selection).toEqual([1, 3]);
  expect(results.projection).toEqual([]);
  expect(results.crossNodeProjection).toEqual([]);
  expect(results.normalizedProjection).toEqual([]);
  expect(results.live).toContainEqual({ id: 2, action: 1, args: null });
  expect(results.live).toContainEqual({ id: 3, action: 1 << 6, args: null });
  expect(results.live).toContainEqual({ id: 1, action: 1 << 21, args: "edited" });
  expect(results.live).toContainEqual({ id: 1, action: 1 << 11, args: { base: 2, extent: 1 } });
  expect(results.rapidReturn).toContainEqual({ id: 1, action: 1 << 21, args: "alpha" });
  expect(results.rapidReturn).toContainEqual({ id: 1, action: 1 << 11, args: { base: 1, extent: 3 } });
  expect(results.repeatedInput).toEqual([]);
  expect(results.blocked).toEqual([]);
  expect(results.unsupported).toEqual([]);
  expect(results.removed).toEqual([]);
  expect(errors).toEqual([]);
});
