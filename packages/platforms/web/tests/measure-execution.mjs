import assert from "node:assert/strict";
import { readFile, readdir, stat, writeFile } from "node:fs/promises";
import { resolve } from "node:path";
import { cpus, platform, release, totalmem } from "node:os";
import { chromium, expect } from "@playwright/test";
import { startPublishServers } from "./serve-publish.mjs";
const [beforeRoot, afterRoot, output] = process.argv.slice(2);
assert.ok(beforeRoot && afterRoot && output, "Pass before-wwwroot, after-wwwroot and output.json. Runs three fresh-browser/warm pairs per mode.");
const roots = [resolve(beforeRoot), resolve(afterRoot)];
const manifests = await Promise.all(roots.map(root => readFile(resolve(root, "doroti.runtime-profile.json"), "utf8").then(JSON.parse)));
for (const field of ["runtimeContract", "runtimeVersion"])
  assert.deepEqual(manifests[0][field], manifests[1][field], `Cannot compare different ${field}.`);
const comparisonProvenance = manifests.map(({ provenance: value }) => ({
  ...Object.fromEntries(["sdk", "targetFramework", "runtimeFlavor", "runtimePack", "configuration", "aot", "simd", "interpretWidgetTree"]
    .map(key => [key, value[key] ?? ""])),
  // Older manifests recorded the Emscripten tool version in emscriptenPack.
  emscriptenVersion: value.emscriptenVersion || (/^[36]\./.test(value.emscriptenPack) ? value.emscriptenPack : ""),
}));
assert.ok(comparisonProvenance.every(value => value.emscriptenVersion), "Emscripten version must be recorded.");
assert.deepEqual(comparisonProvenance[0], comparisonProvenance[1], "Cannot compare different compiler/runtime/application settings.");
const fixtures = await Promise.all(roots.map(root => startPublishServers(root)));
const percentile = (values, fraction) => values.length ? [...values].sort((a, b) => a - b)[Math.min(values.length - 1, Math.ceil(values.length * fraction) - 1)] : null;
const summary = values => ({ count: values.length, p50: percentile(values, .5), p95: percentile(values, .95) });
const results = [];
try {
  // Finish native builds before this command: concurrent compiler load invalidates comparisons.
  for (let repeat = 0; repeat < 3; repeat++) {
    const order = ["before-threaded", "after-threaded", "after-compatible"];
    const variants = [...order.slice(repeat), ...order.slice(0, repeat)];
    for (const variant of variants) {
      const index = variant.startsWith("before") ? 0 : 1;
      const mode = variant.endsWith("compatible") ? "compatible" : "threaded";
      const browser = await chromium.launch({ args: ["--enable-unsafe-swiftshader"] });
      try {
        const page = await browser.newPage({ viewport: { width: 1180, height: 760 } });
        const errors = [];
        const pendingRequests = new Set();
        let networkSizes = [];
        page.on("pageerror", error => errors.push(String(error)));
        page.on("console", message => { if (message.type() === "error") errors.push(message.text()); });
        page.on("request", request => pendingRequests.add(request));
        page.on("requestfinished", request => {
          pendingRequests.delete(request);
          networkSizes.push(request.sizes().then(sizes => ({ url: request.url(), ...sizes })));
        });
        page.on("requestfailed", request => { pendingRequests.delete(request); errors.push(`Request failed: ${request.url()}`); });
        // The historical host reads diagnostics from the live query string.
        // Apply the same URL-only instrumentation to both releases so Router
        // path replacement cannot turn off the baseline's commit markers.
        await page.addInitScript(() => {
          for (const method of ["pushState", "replaceState"]) {
            const original = history[method];
            history[method] = function(state, title, url) {
              if (url !== null && url !== undefined) {
                const target = new URL(url, location.href);
                for (const key of ["dorotiFrameCost", "dorotiInputMarkers", "dorotiRenderer", "dorotiExecutionMode"]) {
                  const value = new URL(location.href).searchParams.get(key);
                  if (value !== null) target.searchParams.set(key, value);
                }
                url = target.href;
              }
              return original.call(this, state, title, url);
            };
          }
        });
        const address = fixtures[index].addresses.find(value => value.isolated === (mode === "threaded")).url;
        for (const cache of ["fresh-browser", "same-page-reload"]) {
          networkSizes = [];
          console.log(JSON.stringify({ stage: "sample-start", variant, repeat: repeat + 1, cache }));
          const url = `${address}/?dorotiExecutionMode=${mode}&dorotiRenderer=worker-direct-webgl&dorotiFrameCost=1&dorotiInputMarkers=1`;
          if (cache === "fresh-browser") await page.goto(url); else await page.reload();
          try {
            await page.waitForFunction(() => document.documentElement.dataset.dorotiBootstrapStage === "failed" ||
              (!!document.querySelector('[data-doroti-worker-runtime="ready"]') &&
                !!document.querySelector("[data-doroti-first-frame-at]")), null, { timeout: 120000 });
            assert.equal(await page.evaluate(() => document.documentElement.dataset.dorotiBootstrapStage === "failed"), false);
          } catch (error) {
            const state = await page.evaluate(() => ({ url: location.href, root: { ...document.documentElement.dataset },
              host: { ...document.querySelector("[data-doroti-host-id]")?.dataset } }));
            throw new Error(`Startup failed for ${variant}/${cache}: ${JSON.stringify({ state, errors })}`, { cause: error });
          }
          const intervalStart = await page.evaluate(async () => { await globalThis.__dorotiFrameCost("reset"); return performance.now(); });
          await page.locator("#doroti-surface").click({ position: { x: 400, y: 400 } });
          for (let input = 0; input < 12; input++) {
            await page.mouse.wheel(0, input < 6 ? 90 : -90);
            await page.evaluate(() => new Promise(done => requestAnimationFrame(() => requestAnimationFrame(done))));
          }
          await expect.poll(() => page.evaluate(async () => {
            const module = await import(new URL("./_content/Doroti.Host.Web/doroti.web.js", document.baseURI));
            const sequence = JSON.parse(globalThis.__dorotiResizeDiagnostics.snapshot(1)).inputSequence;
            return Number.isSafeInteger(sequence) && JSON.parse(module.captureResizeTrace(1))
              .some(frame => frame.phase === "front-commit" && frame.inputSequence >= sequence);
          }), { timeout: 30000 }).toBe(true);
          const capture = await page.evaluate(async () => {
            const cost = await globalThis.__dorotiFrameCost("finish");
            const host = document.querySelector("[data-doroti-first-frame-at]");
            const module = await import(new URL("./_content/Doroti.Host.Web/doroti.web.js", document.baseURI));
            return { cost, startup: Number(host.dataset.dorotiFirstFrameAt), ready: Number(host.dataset.dorotiRuntimeReadyAt),
              commits: JSON.parse(module.captureResizeTrace(1)), gpu: JSON.parse(globalThis.__dorotiResizeDiagnostics.snapshot(1)).gpu, timeOrigin: performance.timeOrigin,
              };
          });
          const paints = capture.cost.trace.rows.filter(row => row[0] === 3 && row[5] >= 0);
          const inputRows = capture.cost.trace.rows.filter(row => row[0] === 2 && row[5] > 0);
          const commits = capture.commits.filter(row => row.phase === "front-commit" && row.timestampMicroseconds / 1000 >= intervalStart);
          const latency = inputRows.flatMap(input => {
            const commit = commits.find(frame => frame.inputSequence >= input[2] && frame.timestampMicroseconds / 1000 + capture.timeOrigin >= input[5]);
            return commit ? [commit.timestampMicroseconds / 1000 + capture.timeOrigin - input[5]] : [];
          });
          assert.ok(capture.startup > 0 && capture.ready > 0, "Require actual first-frame and runtime-ready timestamps.");
          assert.ok(paints.length > 0 && inputRows.length > 0 && commits.length > 1, "Timing capture must contain real paints, input and canvas commits.");
          assert.equal(latency.length, inputRows.length, "Every admitted input must reach a causal canvas commit before capture.");
          assert.ok(latency.every(value => Number.isFinite(value) && value >= 0), "Input latency clocks must agree.");
          assert.equal(capture.cost.trace.dropped, 0, "Dropped trace records invalidate timing comparisons.");
          assert.ok(capture.cost.after.managed.managedHeapBytes > 0 && capture.cost.after.wasmBytes > 0);
          await expect.poll(() => pendingRequests.size, { timeout: 15000 }).toBe(0);
          const network = await Promise.all(networkSizes);
          assert.ok(network.some(row => row.url.includes(`/runtimes/${mode}/_framework/`) && row.url.endsWith(".wasm") && row.responseBodySize > 0),
            "Transfer totals must include the selected runtime's Worker-owned downloads.");
          assert.deepEqual(errors, [], "Browser errors invalidate the performance sample.");
          results.push({ variant, repeat: repeat + 1, cache, browser: browser.version(), firstFrameMs: capture.startup,
            runtimeReadyMs: capture.ready, inputToCommitMs: summary(latency), unmatchedInputs: inputRows.length - latency.length,
            managedPaintMs: summary(paints.map(row => row[4] - row[3])),
            commitIntervalMs: summary(commits.slice(1).map((row, index) => (row.timestampMicroseconds - commits[index].timestampMicroseconds) / 1000)),
            mainToWorkerQueueMs: summary(inputRows.map(row => row[3] - row[5])),
            inputDispatchMs: summary(inputRows.map(row => row[4] - row[3])),
            managedHeapBytes: capture.cost.after.managed.managedHeapBytes, wasmBytes: capture.cost.after.wasmBytes,
            transferBytes: network.reduce((bytes, row) => bytes + row.responseHeadersSize + row.responseBodySize, 0),
            encodedBytes: network.reduce((bytes, row) => bytes + row.responseBodySize, 0),
            traceDropped: capture.cost.trace.dropped, gpu: capture.gpu,
            evidence: { inputRows, paints, commits, network } });
          await writeFile(resolve(output + ".partial.json"), JSON.stringify({ status: "partial", results }, null, 2));
          console.log(JSON.stringify({ stage: "sample-complete", variant, repeat: repeat + 1, cache, samples: results.length }));
        }
      } finally { await browser.close(); }
    }
  }
  async function treeSize(root) {
    let bytes = 0;
    for (const entry of await readdir(root, { withFileTypes: true })) bytes += entry.isDirectory() ? await treeSize(resolve(root, entry.name)) : (await stat(resolve(root, entry.name))).size;
    return bytes;
  }
  const regressions = [];
  for (const cache of ["fresh-browser", "same-page-reload"]) {
    for (const [metric, select] of [["firstFrameMs", row => row.firstFrameMs], ["inputToCommitP95Ms", row => row.inputToCommitMs.p95],
      ["managedPaintP95Ms", row => row.managedPaintMs.p95], ["commitIntervalP95Ms", row => row.commitIntervalMs.p95]]) {
      const before = percentile(results.filter(row => row.variant === "before-threaded" && row.cache === cache).map(select), .5);
      const after = percentile(results.filter(row => row.variant === "after-threaded" && row.cache === cache).map(select), .5);
      assert.ok(Number.isFinite(before) && before > 0 && Number.isFinite(after) && after >= 0, `Missing ${cache}/${metric} samples.`);
      const changePercent = (after - before) / before * 100;
      regressions.push({ cache, metric, before, after, changePercent, thresholdPercent: 10, reviewRequired: after > before * 1.1 });
    }
  }
  const publishTimings = await Promise.all(roots.map(async (root, index) => {
    let receipt;
    try { receipt = JSON.parse(await readFile(resolve(root, "..", "..", "qualification.json"), "utf8")); }
    catch (error) { if (error.code === "ENOENT") return { status: "not-recorded" }; throw error; }
    assert.equal(receipt.status, "passed"); assert.equal(receipt.buildId, manifests[index].buildId);
    return { status: "recorded", elapsedMilliseconds: receipt.elapsedMilliseconds, generationReused: receipt.generationReused };
  }));
  const compatibleTargets = { scope: "provisional loopback/headless desktop budget; not a device SLA",
    inputToCommitP95Ms: 100, freshBrowserFirstFrameMs: 10000, reloadFirstFrameMs: 5000,
    results: results.filter(row => row.variant === "after-compatible").map(row => ({ repeat: row.repeat, cache: row.cache,
      met: row.inputToCommitMs.p95 <= 100 && row.firstFrameMs <= (row.cache === "fresh-browser" ? 10000 : 5000) })) };
  const report = { conditions: { repeats: 3, renderer: "worker-direct-webgl", viewport: "1180x760", headless: true,
    gpu: "SwiftShader allowed; inspect hardware separately", cache: "HTTP no-store; warm is same-page reload with WASM code cache retained",
    instrumentation: "Identical frame-cost/input markers and URL-only preservation of diagnostic options across Router path replacement in both releases; drain to the last causal input commit.",
    transfer: "Playwright completed-request encoded response bodies plus response headers, including Worker requests and navigation; excludes request/TLS framing bytes.",
    order: "Rotate all three variants through first, second and third position across the three repeats.",
    publishTimingComparison: "Not qualified without identical fresh/cached build conditions; missing historical receipts remain not-recorded.",
    displayedLatency: "not measured", hardwarePerformanceAcceptance: false,
    machine: { platform: platform(), release: release(), cpu: cpus()[0]?.model, logicalCpus: cpus().length, totalMemoryBytes: totalmem() },
    comparisonProvenance: comparisonProvenance[0], provenance: manifests.map(value => value.provenance),
    legacyProvenance: "Missing old wasmSdkPack/monoTargetsPack fields remain unknown; compare recorded compiler/runtime axes only." },
    roots, buildIds: manifests.map(value => value.buildId), publishBytes: await Promise.all(roots.map(treeSize)), publishTimings, regressions, compatibleTargets,
    regressionReviewRequired: regressions.some(value => value.reviewRequired), results };
  await writeFile(resolve(output), JSON.stringify(report, null, 2));
  for (const variant of ["before-threaded", "after-threaded", "after-compatible"]) {
    const rows = results.filter(row => row.variant === variant);
    console.log(JSON.stringify({ variant, firstFrameMs: summary(rows.map(row => row.firstFrameMs)),
      inputToCommitP95Ms: summary(rows.map(row => row.inputToCommitMs.p95).filter(value => value !== null)) }));
  }
  console.log(JSON.stringify({ measurement: "complete", hardwarePerformanceAcceptance: false,
    compatibleBudgetMet: compatibleTargets.results.every(value => value.met), regressionReviewRequired: report.regressionReviewRequired, regressions }));
} finally { await Promise.all(fixtures.map(fixture => fixture.close())); }
