import { chromium } from "../web-playwright/node_modules/playwright/index.mjs";
import fs from "node:fs/promises";
import assert from "node:assert/strict";
import { PNG } from "../web-playwright/node_modules/pngjs/lib/png.js";
const evidence = new URL("../evidence/media-query-safe-area/", import.meta.url);
const browser = await chromium.launch({ channel: process.env.DOROTI_BROWSER_CHANNEL ?? "chrome", headless: true,
  args: ["--enable-gpu-rasterization", "--ignore-gpu-blocklist", "--use-angle=default"] });
const results = [];
try {
  for (const mode of ["worker-direct-webgpu", "worker-direct-webgl"]) {
    console.log("START " + mode);
    const page = await browser.newPage({ viewport: { width: 1000, height: 760 } });
    const errors = [], messages = [];
    page.on("pageerror", error => { errors.push(error.message); console.error(error.message); });
    page.on("console", message => { if (message.text().includes("MQ-FIXTURE")) messages.push(message.text()); else if (message.type() === "error") console.error(message.text()); });
    await page.goto(`http://127.0.0.1:5197/?dorotiResizeDiagnostics=1&dorotiRenderer=${mode}&dorotiTestbedMode=media-query`);
    await page.waitForFunction(() => {
      const api = window.__dorotiResizeDiagnostics, id = api?.hosts()[0];
      if (!id) return false;
      const snapshot = JSON.parse(api.snapshot(id));
      return JSON.parse(api.presenter(snapshot.canvasId)).frontGeneration > 0;
    }, null, { timeout: 120000 });
    const capture = () => page.evaluate(() => {
      const api = window.__dorotiResizeDiagnostics, snapshot = JSON.parse(api.snapshot(api.hosts()[0]));
      return { snapshot, presenter: JSON.parse(api.presenter(snapshot.canvasId)), semantics: document.querySelector("#doroti-semantics")?.textContent };
    });
    const initial = await capture();
    assert.equal(initial.presenter.mode, mode);
    assert.equal(initial.snapshot.gpu.hardware, true);
    assert.equal(initial.snapshot.gpu.softwareFallbackUsed, false);
    assert.deepEqual(initial.snapshot.viewInsets, { left: 0, top: 0, right: 0, bottom: 0 });
    await page.setViewportSize({ width: 900, height: 700 });
    await page.waitForFunction(() => {
      const api = window.__dorotiResizeDiagnostics, snapshot = JSON.parse(api.snapshot(api.hosts()[0]));
      return snapshot.logicalWidth === 900 && snapshot.logicalHeight === 700 &&
        JSON.parse(api.presenter(snapshot.canvasId)).frontGeneration === snapshot.resizeEpoch.generation;
    }, null, { timeout: 120000 });
    // Allow the browser compositor to consume the OffscreenCanvas publication.
    // A worker receipt alone is not evidence of visible pixels.
    await page.waitForTimeout(500);
    const resized = await capture();
    await page.emulateMedia({ reducedMotion: "reduce" });
    await page.waitForFunction(previous => {
      const api = window.__dorotiResizeDiagnostics, snapshot = JSON.parse(api.snapshot(api.hosts()[0]));
      const presenter = JSON.parse(api.presenter(snapshot.canvasId));
      return snapshot.reduceMotion && presenter.frontRequestId > previous;
    }, resized.presenter.frontRequestId, { timeout: 120000 });
    await page.waitForTimeout(500);
    const environment = await capture();
    assert.equal(environment.snapshot.resizeEpoch.generation, resized.snapshot.resizeEpoch.generation);
    assert.equal(environment.snapshot.surfaceGeneration, resized.snapshot.surfaceGeneration);
    assert.ok(environment.snapshot.environmentGeneration > resized.snapshot.environmentGeneration);
    assert.equal(errors.length, 0, errors.join("\n"));
    assert.ok(messages.length > 0, "managed MediaQuery fixture rendered");
    const pixels = await page.screenshot({ path: new URL(mode + ".png", evidence).pathname.replace(/^\/(\w:)/, "$1") });
    const png = PNG.sync.read(pixels), colors = new Set();
    for (let offset = 0; offset < png.data.length; offset += 68)
      colors.add(png.data.subarray(offset, offset + 3).toString("hex"));
    assert.ok(colors.size > 8, "visible rendered content, not just a cleared background");
    results.push({ mode, status: "automatedPassed", initial, resized, environment, messages, errors });
    await page.close();
    console.log("PASS " + mode);
  }
} finally {
  await fs.writeFile(new URL("browser-product.json", evidence), JSON.stringify({ browser: browser.version(), results,
    limits: "Desktop Chromium startup, real hardware presentation, resize and settings-only worker delivery; mobile IME/Safari/Firefox/embedded product matrix notVerified." }, null, 2));
  await browser.close();
}
