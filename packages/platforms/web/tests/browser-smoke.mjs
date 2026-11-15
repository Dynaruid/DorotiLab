import assert from "node:assert/strict";
import { chromium, firefox } from "@playwright/test";
import { startPublishServers } from "./serve-publish.mjs";

const root = process.argv[2];
assert.ok(root, "Pass the dual-published wwwroot directory.");
const fixture = await startPublishServers(root);
const addresses = fixture.addresses;
let browser;
let currentPage, trace;
try {
  browser = process.argv.includes("--firefox")
    ? await firefox.launch({ firefoxUserPrefs: { "webgl.force-enabled": true, "gfx.webrender.software": true } })
    : await chromium.launch({ args: ["--enable-unsafe-swiftshader"] });
  const results = [];
  const singleMode = process.argv.find(value => value.startsWith("--single="))?.split("=")[1];
  assert.ok(!singleMode || ["threaded", "compatible"].includes(singleMode));
  const scenarios = singleMode ? [{ isolated: singleMode === "threaded", query: `?dorotiExecutionMode=${singleMode}`, mode: singleMode }] : [
    { isolated: false, query: "", mode: "compatible" },
    { isolated: true, query: "", mode: "threaded" },
    { isolated: true, query: "?dorotiExecutionMode=compatible", mode: "compatible" },
    { isolated: false, path: "/demo/deep/route", query: "", mode: "compatible" },
    { isolated: false, query: "?dorotiExecutionMode=threaded", error: "DOROTIWEB035" },
    { isolated: false, query: "?dorotiExecutionMode=compatible&dorotiRenderer=worker-direct-webgpu", error: "WebGPU requires" },
    { isolated: false, query: "?dorotiExecutionMode=compatible", staleHtml: true, error: "DOROTIWEB055" },
    { isolated: false, query: "?dorotiExecutionMode=compatible", capability: "Worker", error: "capabilities are unavailable" },
    { isolated: false, query: "?dorotiExecutionMode=compatible", capability: "OffscreenCanvas", error: "capabilities are unavailable" },
    { isolated: false, query: "?dorotiExecutionMode=compatible", capability: "WebGL2", error: "OffscreenCanvas WebGL2 support" },
    { isolated: false, path: "/__csp__/page", query: "?dorotiExecutionMode=compatible", error: "Worker|worker|Content Security Policy" },
  ];
  for (const scenario of scenarios.filter(value => (!process.argv.includes("--startup-only") || !value.query) &&
    (!process.argv.includes("--compatible-only") || value.mode === "compatible"))) {
    const context = await browser.newContext();
    const page = await context.newPage();
    await page.addInitScript(() => { globalThis.__dorotiOriginalWorker = globalThis.Worker; });
    if (scenario.capability === "WebGL2") await page.addInitScript(() => {
      const original = OffscreenCanvas.prototype.getContext;
      OffscreenCanvas.prototype.getContext = function(kind, ...args) { return kind === "webgl2" ? null : original.call(this, kind, ...args); };
    });
    else if (scenario.capability) await page.addInitScript(name => { Object.defineProperty(globalThis, name, { value: undefined, configurable: true }); }, scenario.capability);
    if (scenario.staleHtml) await page.route("**/doroti.runtime-profile.json", async route => {
      const response = await route.fetch(); const body = await response.json();
      body.buildId = "previous-generation"; await route.fulfill({ status: 200, headers: { "Content-Type": "application/json" }, json: body });
    });
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
    const url = new URL((scenario.path ?? "/") + scenario.query, origin);
    if (!scenario.error) url.searchParams.set("dorotiResizeDiagnostics", "1");
    await page.goto(url.href);
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
      assert.equal(await page.evaluate(() => document.documentElement.dataset.dorotiBootstrapStage), "started");
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
      await page.setViewportSize({ width: 1024, height: 720 });
      await page.setViewportSize({ width: 1180, height: 760 });
      await page.evaluate(() => new Promise(resolveFrame => requestAnimationFrame(() => requestAnimationFrame(resolveFrame))));
      assert.deepEqual(errors, []);
      assert.deepEqual(consoleErrors, []);
      const paintOwners = [];
      // Ordinary managed pthreads can block their JS event loop in Atomics.wait.
      // Only the JS-affine renderer must answer; do not wait on every pool thread.
      const owners = await Promise.all(page.workers().map(worker => Promise.race([
        worker.evaluate(() => globalThis.__dorotiDirectDiagnostics?.().managed).catch(() => null),
        new Promise(done => setTimeout(() => done(null), 5000)),
      ])));
      paintOwners.push(...owners.filter(Boolean));
      assert.equal(paintOwners.length, 1);
      assert.equal(paintOwners[0].renderOwnerThreadId, Number(actual.runtime.dorotiRenderThreadId));
      assert.equal(paintOwners[0].lastPaintThreadId, paintOwners[0].renderOwnerThreadId);
      assert.ok(paintOwners[0].lastPaintThreadId > 0);
      if (scenario.mode === "compatible" && !scenario.path && !scenario.query && !process.argv.includes("--startup-only")) {
        await page.evaluate(async () => (await import(new URL("./_content/Doroti.Host.Web/doroti.web.js", document.baseURI))).restartDorotiWebHost());
        await page.waitForFunction(previous => Number(document.querySelector("[data-doroti-first-frame-at]")?.dataset.dorotiFirstFrameAt) > previous &&
          document.querySelector('[data-doroti-worker-runtime="ready"]'), Number(actual.runtime.dorotiFirstFrameAt), { timeout: 120000 });
        assert.equal(await page.locator("#doroti-surface").count(), 1);
        assert.equal(await page.evaluate(async () => (await import(new URL("./_content/Doroti.Host.Web/doroti.web.js", document.baseURI))).getDorotiRuntimeState().generation), 2);
        assert.equal(await page.evaluate(() => document.documentElement.dataset.dorotiExecutionMode), "compatible");
        assert.ok(!requests.some(url => url.includes("/runtimes/threaded/")));
        await page.locator("#doroti-surface").click({ position: { x: 70, y: 80 } });
        await page.mouse.wheel(0, 40);
        await page.evaluate(() => new Promise(done => requestAnimationFrame(() => requestAnimationFrame(done))));
      }
      await page.evaluate(() => dispatchEvent(new Event("pagehide")));
      await page.waitForFunction(() => document.querySelector('[data-doroti-worker-runtime="disposed"]'), null, { timeout: 45000 });
      assert.equal(await page.evaluate(() => document.querySelector('[data-doroti-worker-runtime="disposed"]').dataset.dorotiShutdownRequests), "0");
      const textures = await page.evaluate(() => JSON.parse(document.querySelector('[data-doroti-worker-runtime="disposed"]').dataset.dorotiShutdownTextures));
      for (const field of ["live", "registrations", "gpuBytes", "pending", "completions"]) assert.equal(textures[field], 0, `Shutdown retained ${field}.`);
      assert.equal(await page.evaluate(() => Worker === globalThis.__dorotiOriginalWorker), true);
      assert.deepEqual(errors, []);
      assert.deepEqual(consoleErrors, []);
      results.push({ ...scenario, ...actual, workers: page.workers().map(worker => worker.url()),
        runtimeRequestCount: requests.filter(url => url.includes(`/runtimes/${scenario.mode}/`)).length,
        paintOwners: paintOwners.map(value => ({ owner: value.renderOwnerThreadId, paint: value.lastPaintThreadId, nativeSkia: value.nativeSkia })),
        shutdown: "disposed; no pending requests" });
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
  await fixture.close();
}
