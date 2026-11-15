import assert from "node:assert/strict";
import { chromium, expect, firefox } from "@playwright/test";
import { startPublishServers } from "./serve-publish.mjs";

assert.ok(process.argv[2], "Pass Testbed's dual-published wwwroot directory.");
const fixture = await startPublishServers(process.argv[2]);
const browser = process.argv.includes("--firefox")
  ? await firefox.launch({ firefoxUserPrefs: { "webgl.force-enabled": true, "gfx.webrender.software": true } })
  : await chromium.launch({ args: ["--enable-unsafe-swiftshader"] });
const textureDiagnostics = page => page.evaluate(async () => {
  const registry = (await import(new URL("./_content/Doroti.Host.Web/doroti.web.textures.js", document.baseURI))).texturesForCanvas();
  return { ...await registry.diagnostics(), sourceBytes: registry.sourceBytes };
});
async function waitForTextures(page, ready) {
  let value;
  await expect.poll(async () => { value = await textureDiagnostics(page); return ready(value); }, { timeout: 30000 }).toBe(true);
  return value;
}
async function tap(page, name, sourceAction = false) {
  const selector = `[role="tab"][aria-label^="${name}"], [role="button"][aria-label="${name}"]`;
  const control = page.locator(selector).first();
  await control.waitFor({ state: "attached" });
  await page.waitForFunction(selector => {
    const element = document.querySelector(selector);
    return element && element.getAttribute("aria-disabled") !== "true";
  }, selector);
  // Exercise the existing semantics action. Texture allocation can complete
  // before its changed preview geometry reaches the main DOM; a pointer aimed
  // at the previous rectangle is not a texture-lifetime test.
  if (!sourceAction) { await control.evaluate(element => element.click()); return; }
  await control.evaluate((element, selector) => new Promise((resolve, reject) => {
    let busy = false;
    const observer = new MutationObserver(() => {
      const current = document.querySelector(selector);
      if (current?.getAttribute("aria-disabled") === "true") busy = true;
      else if (busy && current) { observer.disconnect(); clearTimeout(timeout); resolve(); }
    });
    const timeout = setTimeout(() => { observer.disconnect(); reject(new Error("Texture source action did not complete its busy/ready transition.")); }, 30000);
    observer.observe(document.documentElement, { subtree: true, childList: true, attributes: true, attributeFilter: ["aria-disabled"] });
    element.click();
  }), selector);
}
try {
  for (const mode of ["threaded", "compatible"]) {
    const context = await browser.newContext({ viewport: { width: 900, height: 1100 } });
    const page = await context.newPage(), errors = [];
    page.on("pageerror", error => errors.push(String(error)));
    page.on("console", message => { if (message.type() === "error") errors.push(message.text()); });
    try {
      const origin = fixture.addresses.find(value => value.isolated === (mode === "threaded")).url;
      await page.goto(`${origin}/?dorotiResizeDiagnostics=1`);
      await page.waitForFunction(() => document.querySelector('[data-doroti-worker-runtime="ready"]') && document.querySelector("[data-doroti-first-frame-at]"), null, { timeout: 120000 });
      await tap(page, "Texture"); await tap(page, "Canvas", true);
      const canvas = await waitForTextures(page, value => value.registrations === 1 && value.sources.some(source => source.sequence > 0) && value.live > 0);
      assert.deepEqual(errors, [], "Replacing the managed pixel entry must not resubmit its retired ID.");
      await tap(page, "Freeze"); await page.locator('[role="button"][aria-label="Resume"]').waitFor({ state: "attached" });
      const frozen = await textureDiagnostics(page);
      const stillFrozen = await waitForTextures(page, value => value.sources[0].sequence >= frozen.sources[0].sequence + 2);
      assert.equal(stillFrozen.imported, frozen.imported, "Frozen rendering must retain its image while the producer advances.");
      await tap(page, "Resume");
      await page.locator('[role="button"][aria-label="Freeze"]').waitFor({ state: "attached" });
      await tap(page, "Resize", true);
      await tap(page, "ImageBitmap", true);
      await expect.poll(() => page.evaluate(async () => (await import(new URL("./plugins/textures.js", document.baseURI))).diagnostics().source), { timeout: 30000 }).toBe("bitmap");
      await waitForTextures(page, value => value.registrations === 1 && value.sources.some(source => source.sequence > 0) && value.live > 0);
      assert.deepEqual(errors, [], "Replacing an external source must remove its old layer before unregistration.");
      await tap(page, "Stop", true);
      await waitForTextures(page, value => value.registrations === 0 && value.live === 0 && value.gpuBytes === 0 && value.completions === 0 && value.sourceBytes === 0);
      assert.deepEqual(errors, [], "Stopping a source must submit a texture-free tree before retiring its ID.");
      await tap(page, "Canvas", true);
      await waitForTextures(page, value => value.live > 0);
      await page.evaluate(() => dispatchEvent(new Event("pagehide")));
      await page.waitForFunction(() => document.querySelector('[data-doroti-worker-runtime="disposed"]'));
      const retired = await page.evaluate(() => JSON.parse(document.querySelector('[data-doroti-worker-runtime="disposed"]').dataset.dorotiShutdownTextures));
      for (const field of ["live", "registrations", "gpuBytes", "pending", "completions"]) assert.equal(retired[field], 0, `Shutdown retained ${field}.`);
      assert.equal(await page.evaluate(async () => {
        try { (await import(new URL("./_content/Doroti.Host.Web/doroti.web.textures.js", document.baseURI))).texturesForCanvas(); return "still-live"; }
        catch (error) { return error.code; }
      }), "NotReady");
      assert.deepEqual(errors, []);
      console.log(JSON.stringify({ mode, result: "PASS", browser: browser.version(),
        scenario: "semantic activation; managed JS-plugin round trip; canvas/ImageBitmap upload; freeze/resume/resize; external GPU/source retirement; shutdown",
        active: { registrations: canvas.registrations, live: canvas.live, gpuBytes: canvas.gpuBytes },
        stopped: "no external registrations, live allocations, GPU bytes, completion waits or source bytes",
        limitations: "Headless software GPU; camera, video, WebCodecs and hardware texture acceptance are separate." }));
    } catch (error) {
      console.error(JSON.stringify({ mode, errors, labels: await page.locator("[aria-label]").evaluateAll(nodes => nodes.map(node => [node.getAttribute("role"), node.getAttribute("aria-label")])) }, null, 2));
      throw error;
    } finally { await context.close(); }
  }
} finally { await browser.close(); await fixture.close(); }
