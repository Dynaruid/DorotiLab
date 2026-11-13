import assert from "node:assert/strict";
import { spawn } from "node:child_process";
import { createInterface } from "node:readline";
import { fileURLToPath } from "node:url";
import { resolve } from "node:path";
import { chromium } from "@playwright/test";

const root = process.argv[2];
assert.ok(root, "Pass the dual-published wwwroot directory.");
const server = spawn(process.execPath, [fileURLToPath(new URL("serve-publish.mjs", import.meta.url)), resolve(root)], { stdio: ["ignore", "pipe", "inherit"] });
let browser;
let currentPage, trace;
try {
  const addresses = await new Promise((resolveAddresses, reject) => {
    const values = [];
    const timer = setTimeout(() => reject(new Error("Smoke server startup timed out.")), 15000);
    server.once("error", reject);
    server.once("exit", code => reject(new Error(`Smoke server exited: ${code}`)));
    createInterface({ input: server.stdout }).on("line", line => {
      values.push(JSON.parse(line));
      if (values.length === 2) { clearTimeout(timer); resolveAddresses(values); }
    });
  });
  browser = await chromium.launch({ args: ["--enable-unsafe-swiftshader"] });
  const results = [];
  const scenarios = [
    { isolated: false, query: "", mode: "compatible" },
    { isolated: true, query: "", mode: "threaded" },
    { isolated: true, query: "?dorotiExecutionMode=compatible", mode: "compatible" },
    { isolated: false, path: "/demo/deep/route", query: "", mode: "compatible" },
    { isolated: false, query: "?dorotiExecutionMode=threaded", error: "DOROTIWEB035" },
    { isolated: false, query: "?dorotiExecutionMode=compatible&dorotiRenderer=worker-direct-webgpu", error: "WebGPU requires" },
  ];
  for (const scenario of scenarios.filter(value => (!process.argv.includes("--startup-only") || !value.query) &&
    (!process.argv.includes("--compatible-only") || value.mode === "compatible"))) {
    const context = await browser.newContext();
    const page = await context.newPage();
    currentPage = page;
    const requests = [], errors = [], consoleErrors = [];
    trace = { scenario, requests, errors, consoleErrors, console: [] };
    page.on("request", request => requests.push(request.url()));
    page.on("pageerror", error => errors.push(String(error)));
    page.on("console", message => {
      if (["error", "warning"].includes(message.type())) trace.console.push(message.text());
      if (message.type() === "error") consoleErrors.push(message.text());
    });
    const origin = addresses.find(value => value.isolated === scenario.isolated).url;
    await page.goto(origin + (scenario.path ?? "/") + scenario.query);
    assert.equal(await page.evaluate(() => crossOriginIsolated), scenario.isolated);
    if (scenario.error) {
      await page.waitForFunction(() => document.documentElement.dataset.dorotiBootstrapStage === "failed", null, { timeout: 30000 });
      assert.match(await page.evaluate(() => document.documentElement.dataset.dorotiBootstrapError), new RegExp(scenario.error));
      assert.ok(!requests.some(url => url.includes("/runtimes/") && url.endsWith(".js")), "Invalid requests must fail before importing .NET.");
    } else {
      await page.waitForFunction(() => !!document.querySelector("[data-doroti-first-frame-at]") &&
        !!document.querySelector('[data-doroti-worker-runtime="ready"]'), null, { timeout: 120000 });
      const actual = await page.evaluate(() => ({ selected: document.documentElement.dataset.dorotiExecutionMode,
        runtime: document.querySelector("[data-doroti-runtime-threads]") ? { ...document.querySelector("[data-doroti-runtime-threads]").dataset } : null,
        firstFrame: { ...document.querySelector("[data-doroti-first-frame-at]").dataset } }));
      assert.ok(actual.runtime, JSON.stringify({ scenario, actual, errors }));
      assert.equal(actual.selected, scenario.mode);
      assert.equal(actual.runtime.dorotiRuntimeThreads, String(scenario.mode === "threaded"));
      assert.equal(actual.runtime.dorotiRuntimeSharedHeap, String(scenario.mode === "threaded"));
      assert.ok(Number(actual.runtime.dorotiRenderThreadId) > 0);
      assert.ok(Number(actual.runtime.dorotiFirstFrameAt) > 0 && Number(actual.runtime.dorotiRuntimeReadyAt) > 0);
      const other = scenario.mode === "threaded" ? "compatible" : "threaded";
      assert.ok(!requests.some(url => url.includes(`/runtimes/${other}/`)), "Unselected runtime downloaded.");
      if (scenario.mode === "compatible") assert.ok(!requests.some(url => /dotnet\.native\.worker/.test(url)), "Compatible runtime created a pthread.");
      const mainRuntime = await page.evaluate(() => {
        const runtime = globalThis.getDotnetRuntime?.(0);
        return runtime ? { version: runtime.runtimeBuildInfo.productVersion, threads: runtime.runtimeBuildInfo.wasmEnableThreads,
          shared: runtime.localHeapViewU8().buffer instanceof SharedArrayBuffer, second: !!globalThis.getDotnetRuntime(1) } : null;
      });
      if (scenario.mode === "threaded") {
        assert.equal(mainRuntime.threads, true);
        assert.equal(mainRuntime.shared, true);
        assert.equal(mainRuntime.second, false);
      } else {
        assert.equal(mainRuntime, null);
        const runtimeOwners = [];
        for (const worker of page.workers()) {
          const info = await worker.evaluate(() => {
            const runtime = globalThis.getDotnetRuntime?.(0);
            return runtime ? { threads: runtime.runtimeBuildInfo.wasmEnableThreads,
              shared: typeof SharedArrayBuffer !== "undefined" && runtime.localHeapViewU8().buffer instanceof SharedArrayBuffer,
              second: !!globalThis.getDotnetRuntime(1) } : null;
          });
          if (info) runtimeOwners.push(info);
        }
        assert.deepEqual(runtimeOwners, [{ threads: false, shared: false, second: false }]);
      }
      await page.locator("#doroti-surface").click({ position: { x: 40, y: 40 } });
      await page.mouse.wheel(0, 150);
      await page.evaluate(() => new Promise(resolveFrame => requestAnimationFrame(() => requestAnimationFrame(resolveFrame))));
      assert.deepEqual(errors, []);
      assert.deepEqual(consoleErrors, []);
      results.push({ ...scenario, ...actual, workers: page.workers().map(worker => worker.url()),
        runtimeRequestCount: requests.filter(url => url.includes(`/runtimes/${scenario.mode}/`)).length });
    }
    await context.close();
  }
  console.log(JSON.stringify(results, null, 2));
} catch (error) {
  if (currentPage && !currentPage.isClosed()) {
    console.error(JSON.stringify({ ...trace, document: await currentPage.evaluate(() => ({
      bootstrap: { ...document.documentElement.dataset },
      host: [...document.querySelectorAll("[data-doroti-host]")].map(element => ({ ...element.dataset })),
    })), workers: currentPage.workers().map(worker => worker.url()) }, null, 2));
  }
  throw error;
} finally {
  await browser?.close();
  server.kill();
}
