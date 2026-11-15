import assert from "node:assert/strict";
import { chromium } from "@playwright/test";
import { startPublishServers } from "./serve-publish.mjs";
assert.ok(process.argv[2], "Pass the runtime-only probe's published wwwroot.");
const fixture = await startPublishServers(process.argv[2]);
const browser = await chromium.launch();
try {
  const page = await browser.newPage();
  const messages = [], errors = [], requests = [];
  page.on("console", message => messages.push(message.text()));
  page.on("pageerror", error => errors.push(String(error)));
  page.on("request", request => requests.push(new URL(request.url()).pathname));
  const address = new URL(fixture.addresses.find(value => value.isolated).url);
  if (process.argv.includes("--trace")) address.searchParams.set("trace", "1");
  const pool = process.argv.find(value => value.startsWith("--pool="));
  if (pool) address.searchParams.set("pool", pool.slice(7));
  if (process.argv.includes("--wait-preload")) address.searchParams.set("waitPreload", "1");
  await page.goto(address.href, { waitUntil: "commit" });
  let status = "passed";
  try { await page.waitForFunction(() => document.querySelector("output")?.textContent.startsWith("owner="), null, { timeout: 60000 }); }
  catch (error) {
    const reproduced = messages.some(message => message.startsWith("probe: before Thread.Start")) &&
      !messages.some(message => message.includes("Thread.Start returned") || message.includes("probe: owner-enter"));
    if (!process.argv.includes("--expect-blocked") || !reproduced) throw error;
    status = "blocked: Thread.Start did not return within 60 seconds; no render owner entered";
  }
  if (status === "passed") {
    assert.ok(messages.some(message => message.includes("Thread.Start returned")));
    assert.ok(messages.some(message => message.includes("owner-enter")));
    assert.deepEqual(errors, []);
  }
  console.log(JSON.stringify({ qualification: "runtime-only; no Doroti/Skia/native relink", status, messages, errors,
    workers: page.workers().map(worker => worker.url()), requests }, null, 2));
} finally { await browser.close(); await fixture.close(); }
