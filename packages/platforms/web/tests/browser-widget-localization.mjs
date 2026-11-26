import assert from "node:assert/strict";
import { chromium, expect } from "@playwright/test";
import { startPublishServers } from "./serve-publish.mjs";

const [root, mode = "compatible", basePath = "/", ...options] = process.argv.slice(2);
assert.ok(root && ["compatible", "threaded"].includes(mode), "Pass a localization probe wwwroot and Web mode.");
assert.ok(basePath === "/" || basePath === "/demo/", "Use / or /demo/ for the fixture base path.");
const servers = await startPublishServers(root);
try {
  const browser = await chromium.launch({ args: ["--enable-unsafe-swiftshader"] });
  try {
    const page = await browser.newPage({ viewport: { width: 1000, height: 720 } });
    const runtimeErrors = [];
    const icuRequests = new Set();
    page.on("pageerror", error => runtimeErrors.push(String(error)));
    page.on("requestfinished", request => {
      const path = new URL(request.url()).pathname;
      if (/\/icudt[^/]*\.dat$/.test(path)) icuRequests.add(path);
    });
    await page.addInitScript(() => {
      Object.defineProperty(navigator, "language", { get: () => "fr-FR" });
      Object.defineProperty(navigator, "languages", { configurable: true, get: () => ["fr-FR", "en-US", "ko-KR"] });
    });
    const base = servers.addresses.find(address => address.isolated === (mode === "threaded")).url;
    const url = `${base}${basePath}?dorotiExecutionMode=${mode}&dorotiRenderer=worker-webgl`;
    await page.goto(url, { waitUntil: "commit" });
    const canvas = page.locator("[data-doroti-first-frame-at]");
    await expect(canvas).toHaveAttribute("data-doroti-runtime-threads", String(mode === "threaded"), { timeout: 120000 });
    const byId = id => page.locator(`[data-doroti-semantics-identifier="locale-probe-${id}"]`);
    const caption = byId("caption"), widget = byId("widget"), subtree = byId("subtree");
    await expect(caption).toHaveAttribute("aria-label", "2 localized items", { timeout: 30000 });
    await expect(caption).toHaveAttribute("lang", "en-US");
    await expect(widget).toHaveAttribute("aria-label", "Cancel");
    await expect(subtree).toHaveAttribute("aria-label", "One localized item");

    const field = page.locator('[data-doroti-semantics-id][role="textbox"]').first();
    await field.click({ force: true });
    const ime = page.locator("#doroti-ime");
    await expect(ime).toHaveAttribute("lang", "en-US");
    await expect(ime).toHaveAttribute("dir", "ltr");

    await page.evaluate(() => {
      // Keep the first preference and navigator.language identical. Only the ordered tail changes.
      Object.defineProperty(navigator, "languages", { configurable: true, get: () => ["fr-FR", "ko-KR", "en-US"] });
      window.dispatchEvent(new Event("languagechange"));
    });
    await expect(caption).toHaveAttribute("aria-label", "현지화 항목 2개", { timeout: 30000 });
    await expect(caption).toHaveAttribute("lang", "ko-KR");
    await expect(caption).toHaveAttribute("dir", "ltr");
    await expect(widget).toHaveAttribute("aria-label", "취소");
    await expect(subtree).toHaveAttribute("lang", "en-US");
    await expect(subtree).toHaveAttribute("aria-label", "One localized item");
    await expect(ime).toHaveAttribute("lang", "ko-KR");
    await expect(ime).toHaveAttribute("dir", "ltr");
    assert.deepEqual(runtimeErrors, []);
    assert.ok([...icuRequests].some(path => path.startsWith(basePath)), "The runtime requested its ICU data from the selected deployment path.");
    console.log(`PASS ${mode} ${basePath} Chromium ${browser.version()}: ordered locale changes, ARB and Material resources, subtree lang, active IME, ICU ${[...icuRequests].join(", ")}`);
    if (options.includes("--icu-errors")) {
      for (const corruption of ["missing", "invalid"]) {
        const context = await browser.newContext({ serviceWorkers: "block" });
        try {
          let requests = 0;
          await context.route(/\/icudt[^/]*\.dat$/, route => {
            requests++;
            return corruption === "missing"
              ? route.fulfill({ status: 404 })
              : route.fulfill({ status: 200, contentType: "application/octet-stream", body: "invalid ICU data" });
          });
          const failing = await context.newPage();
          const diagnostics = [];
          failing.on("pageerror", error => diagnostics.push(String(error)));
          failing.on("console", message => { if (message.type() === "error") diagnostics.push(message.text()); });
          await failing.goto(url, { waitUntil: "commit" });
          await expect.poll(() => diagnostics.join("\n"), { timeout: 30000 }).toMatch(/Could not load ICU globalization data 'icudt/);
          assert.ok(requests > 0, "Expected an intercepted ICU request.");
          await expect(failing.locator("[data-doroti-first-frame-at]")).toHaveCount(0);
          console.log(`PASS ${mode} ${basePath} ${corruption} ICU data reports the failed asset before a first frame`);
        } finally { await context.close(); }
      }
    }
  } finally { await browser.close(); }
} finally { await servers.close(); }
