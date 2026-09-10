import { chromium } from "../web-playwright/node_modules/playwright/index.mjs";
import fs from "node:fs/promises";
import assert from "node:assert/strict";

const source = await fs.readFile(new URL("../../src/Doroti.Host.Web/obj/Debug/net10.0/Doroti.Web/wwwroot/doroti.web.environment.js", import.meta.url), "utf8");
const protocol = await fs.readFile(new URL("../../src/Doroti.Host.Web/obj/Debug/net10.0/Doroti.Web/wwwroot/doroti.web.protocol.js", import.meta.url), "utf8");
const browser = await chromium.launch({ headless: true });
try {
  const page = await browser.newPage({ viewport: { width: 360, height: 800 } });
  const errors = []; page.on("pageerror", error => errors.push(error.message));
  await page.setContent('<style>body{margin:0}#root{width:360px;height:800px}textarea{position:absolute;top:100px}</style><main id="root"><textarea></textarea></main>');
  await page.addScriptTag({ type: "module", content: source + "\nwindow.Provider = BrowserViewEnvironment; window.edgeOcclusion = edgeOcclusion;" });
  await page.addScriptTag({ type: "module", content: protocol + "\nwindow.validateEnvironment = validateViewEnvironment; window.protocolVersion = dorotiProtocolVersion;" });
  const results = await page.evaluate(() => {
    const vk = new EventTarget(); vk.boundingRect = new DOMRect();
    Object.defineProperty(navigator, "virtualKeyboard", { configurable: true, value: vk });
    const root = document.querySelector("#root"), input = document.querySelector("textarea");
    let changes = 0;
    const environment = new window.Provider(root, input, () => changes++);
    window.environment = environment; window.changes = () => changes;
    root.lastElementChild.style.padding = "24px 10px 20px 8px";
    environment.refresh();
    const initial = structuredClone(environment.value);
    input.focus(); vk.boundingRect = new DOMRect(0, 500, 360, 300); vk.dispatchEvent(new Event("geometrychange"));
    const docked = structuredClone(environment.value);
    const same = changes; environment.refresh();
    const deduplicated = changes === same;
    vk.boundingRect = new DOMRect(40, 550, 280, 200); vk.dispatchEvent(new Event("geometrychange"));
    const floating = structuredClone(environment.value);
    root.style.height = "500px"; vk.boundingRect = new DOMRect(0, 500, 360, 300); environment.refresh();
    const resized = structuredClone(environment.value);
    root.style.height = "800px"; input.blur(); environment.refresh();
    const hidden = structuredClone(environment.value);
    let invalid = 0;
    for (const value of [null, {}, { ...initial, environmentGeneration: -1 },
      { ...initial, viewInsets: { left: 0, top: 0, right: 0, bottom: -1 } }]) {
      try { window.validateEnvironment(value); } catch { invalid++; }
    }
    window.validateEnvironment(docked);
    return { initial, docked, floating, resized, hidden, deduplicated, invalid, version: window.protocolVersion };
  });
  assert.equal(results.version, 4);
  assert.deepEqual(results.initial.viewPadding, { left: 8, top: 24, right: 10, bottom: 20 });
  assert.equal(results.docked.viewInsets.bottom, 300);
  assert.equal(results.docked.viewPadding.bottom, 20);
  assert.equal(results.floating.viewInsets.bottom, 0);
  assert.equal(results.resized.viewInsets.bottom, 0);
  assert.equal(results.hidden.viewInsets.bottom, 0);
  assert.equal(results.deduplicated, true); assert.equal(results.invalid, 4);
  await page.emulateMedia({ reducedMotion: "reduce", forcedColors: "active" });
  await page.waitForFunction(() => window.environment.value.reduceMotion && window.environment.value.highContrast);
  await page.evaluate(() => { window.environment.dispose(); window.disposedChanges = window.changes(); });
  await page.emulateMedia({ reducedMotion: "no-preference", forcedColors: "none" });
  await page.evaluate(() => new Promise(resolve => requestAnimationFrame(() => requestAnimationFrame(resolve))));
  assert.equal(await page.evaluate(() => window.changes() === window.disposedChanges), true);

  const fallback = await page.evaluate(() => {
    delete navigator.virtualKeyboard;
    const viewport = new EventTarget(); Object.assign(viewport, { height: 800, width: 360, offsetTop: 0, offsetLeft: 0, scale: 1 });
    Object.defineProperty(window, "visualViewport", { configurable: true, value: viewport });
    const root = document.querySelector("#root"), input = document.querySelector("textarea");
    const environment = new window.Provider(root, input, () => {});
    input.focus(); viewport.height = 500; viewport.dispatchEvent(new Event("resize"));
    const docked = environment.value.viewInsets.bottom;
    viewport.scale = 2; viewport.dispatchEvent(new Event("resize"));
    const pinch = environment.value.viewInsets.bottom;
    viewport.scale = 1; viewport.height = 750; viewport.dispatchEvent(new Event("resize"));
    const toolbar = environment.value.viewInsets.bottom;
    root.style.marginLeft = "20px"; root.style.width = "300px"; viewport.height = 500; environment.refresh();
    const embedded = environment.value.viewInsets.bottom;
    environment.dispose(); return { docked, pinch, toolbar, embedded };
  });
  assert.deepEqual(fallback, { docked: 300, pinch: 0, toolbar: 0, embedded: 0 });
  assert.deepEqual(errors, []);
  console.log(JSON.stringify({ status: "automatedPassed", engine: browser.version(), results, fallback,
    limits: "DOM fixtures inject occlusion geometry; physical mobile IME and renderer presentation are separate gates." }, null, 2));
} finally { await browser.close(); }
