import assert from "node:assert/strict";
import { writeFile } from "node:fs/promises";
import { chromium, expect, firefox, webkit } from "@playwright/test";
import { startPublishServers } from "./serve-publish.mjs";

const [root, mode, imageFile, engine = "chromium"] = process.argv.slice(2);
const engines = { chromium, firefox, webkit };
assert.ok(root && ["compatible", "threaded"].includes(mode), "Pass the published wwwroot and the Web mode.");
assert.ok(Object.hasOwn(engines, engine), "Choose chromium, firefox, or webkit.");
const servers = await startPublishServers(root);
try {
  const browser = await engines[engine].launch();
  try {
    const errors = [];
    const page = await browser.newPage({ serviceWorkers: "block" });
    page.on("pageerror", error => errors.push(String(error)));
    const isolated = mode === "threaded";
    const address = servers.addresses.find(entry => entry.isolated === isolated);
    const origin = new URL(address.url).origin;
    const externalRequests = [];
    await page.route("**/*", route => {
      const url = route.request().url();
      if (new URL(url).origin === origin) return route.continue();
      externalRequests.push(url);
      return route.abort();
    });
    await page.goto(address.url, { waitUntil: "commit" });
    const status = page.locator("#status");
    await expect(status).toHaveAttribute("data-outcome", /^(pass|fail)$/, { timeout: 90000 });
    const receipt = await status.textContent();
    assert.equal(await status.getAttribute("data-outcome"), "pass", receipt);
    assert.match(receipt, new RegExp(`isolated=${isolated} threads=${isolated} sharedHeap=${isolated} runtime=\\d+\\.\\d+\\.\\d+`));
    assert.deepEqual(errors, []);
    assert.deepEqual(externalRequests, [], "A static paragraph deployment uses only its own assets.");
    if (imageFile) {
      const source = await page.locator("#result").getAttribute("src");
      await writeFile(imageFile, Buffer.from(source.split(",")[1], "base64"));
    }
    console.log(`PASS ${mode} ${engine} ${browser.version()} | ${receipt}`);
  } finally { await browser.close(); }
} finally { await servers.close(); }
