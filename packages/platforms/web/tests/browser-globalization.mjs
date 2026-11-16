import assert from "node:assert/strict";
import { chromium, firefox } from "@playwright/test";
import { startPublishServers } from "./serve-publish.mjs";

const directory = process.argv[2];
assert.ok(directory, "Pass Doroti.Intl.Web.Tests published wwwroot.");
const baseline = process.argv.includes("--baseline");
const shardMissing = process.argv.includes("--expect-missing-culture");
const engine = process.argv.includes("--firefox") ? firefox : chromium;
const fixture = await startPublishServers(directory);
const browser = await engine.launch();
const receipts = [];
try {
  for (const path of ["/", "/demo/"]) {
    const context = await browser.newContext({ locale: "en-US", serviceWorkers: "block" });
    const page = await context.newPage();
    const requests = [], errors = [];
    page.on("request", request => requests.push(new URL(request.url()).pathname));
    page.on("pageerror", error => errors.push(String(error)));
    const url = fixture.addresses.find(address => address.isolated).url + path;
    await page.goto(url, { waitUntil: "commit" });
    await page.waitForFunction(() => ["english", "failed"].includes(globalThis.__intlProbe?.status), null, { timeout: 90000 });
    assert.equal(await page.evaluate(() => globalThis.__intlProbe.status), "english",
      JSON.stringify({ state: await page.evaluate(() => globalThis.__intlProbe), errors, requests }));
    assert.equal(await page.locator("#date").textContent(), "Thursday, February 29, 2024");
    if (!baseline) {
      await page.locator("#korean").click();
      await page.waitForFunction(() => ["passed", "failed"].includes(globalThis.__intlProbe?.status), null, { timeout: 90000 });
    }
    const state = await page.evaluate(() => globalThis.__intlProbe);
    const icu = requests.filter(path => /icudt.*\.dat(?:$|\?)/.test(path));
    assert.ok(icu.length > 0, "Record the ICU file actually requested, not only the MSBuild property.");
    assert.deepEqual(errors, []);
    if (shardMissing) {
      assert.equal(state.status, "failed");
      assert.ok(state.errors.some(error => /globalization (?:data|resources)/.test(error)), JSON.stringify(state));
      assert.equal(await page.locator("#date").textContent(), "Thursday, February 29, 2024", "Failed transition retains the previous output.");
    } else {
      assert.equal(state.status, baseline ? "english" : "passed", JSON.stringify(state));
      assert.deepEqual(state.errors, []);
      if (!baseline) {
        assert.ok(state.contract.Checks >= 100);
        assert.equal(state.contract.Korean.Locale, "ko-KR");
        assert.equal(await page.locator("#date").textContent(), "2024년 2월 29일 목요일");
        assert.equal(await page.locator("html").getAttribute("lang"), "ko-KR");
      }
    }
    const receipt = { path, engine: engine.name(), qualification: "runtime formatting and DOM only; no Doroti render Worker/Skia/IME", state, icu };
    receipts.push(receipt);
    console.log(JSON.stringify(receipt));
    await context.close();
  }
  if (!baseline && !shardMissing) {
    for (const mode of ["missing", "corrupt"]) {
      const context = await browser.newContext({ locale: "en-US", serviceWorkers: "block" });
      const page = await context.newPage();
      const diagnostics = [];
      page.on("pageerror", error => diagnostics.push(String(error)));
      page.on("console", message => { if (message.type() === "error") diagnostics.push(message.text()); });
      await page.route(/icudt.*\.dat/, route => mode === "missing"
        ? route.abort("failed") : route.fulfill({ status: 200, contentType: "application/octet-stream", body: Buffer.from("corrupt ICU") }));
      await page.goto(fixture.addresses.find(address => address.isolated).url, { waitUntil: "commit" });
      await page.waitForFunction(() => globalThis.__intlProbe?.status === "failed", null, { timeout: 90000 });
      assert.equal(await page.locator("#date").textContent(), "", "Data load failure cannot masquerade as successful English output.");
      const receipt = { mode, state: await page.evaluate(() => globalThis.__intlProbe), diagnostics };
      receipts.push(receipt);
      console.log(JSON.stringify(receipt));
      await context.close();
    }
  }
  console.log(JSON.stringify({ status: "passed", receipts }, null, 2));
} finally {
  await browser.close();
  await fixture.close();
}
