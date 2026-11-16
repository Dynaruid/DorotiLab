import assert from "node:assert/strict";
import { mkdir, readFile, writeFile } from "node:fs/promises";
import { dirname, resolve } from "node:path";
import { chromium, expect } from "@playwright/test";
import { startPublishServers } from "./serve-publish.mjs";

const [directory, output] = process.argv.slice(2);
assert.ok(directory && output, "Pass an opt-in Demo publish's wwwroot and an output JSON path.");
const baseline = process.argv.includes("--baseline");
const mode = process.argv.includes("--threaded") ? "threaded" : "compatible";
const repeats = process.argv.includes("--once") ? 1 : 3;
const manifest = JSON.parse(await readFile(resolve(directory, "doroti.runtime-profile.json"), "utf8"));
const fixture = await startPublishServers(directory);
const percentile = (values, fraction) => [...values].sort((a, b) => a - b)[Math.ceil(values.length * fraction) - 1];
const samples = [];
await mkdir(dirname(resolve(output)), { recursive: true });
try {
  // Complete builds before using the measured results. Three fresh-browser samples, twelve
  // English date changes each. Input is synthetic; these timings do not qualify hardware/IME.
  for (let repeat = 0; repeat < repeats; repeat++) {
    const browser = await chromium.launch({ args: ["--enable-unsafe-swiftshader"] });
    try {
      const page = await browser.newPage({ viewport: { width: 1000, height: 720 }, locale: "en-US" });
      const errors = [], transfers = [];
      page.on("pageerror", error => errors.push(String(error)));
      page.on("console", message => { if (message.type() === "error") errors.push(message.text()); });
      page.on("requestfinished", request => transfers.push(request.sizes().then(size => ({ url: request.url(), ...size }))));
      const base = fixture.addresses.find(address => address.isolated === (mode === "threaded")).url;
      await page.goto(`${base}/?dorotiExecutionMode=${mode}&dorotiRenderer=worker-webgl&dorotiFrameCost=1&dorotiResizeDiagnostics=1`, { waitUntil: "commit" });
      await page.waitForFunction(() => document.querySelector("[data-doroti-first-frame-at]")
        && document.querySelector('[data-doroti-worker-runtime="ready"]'), null, { timeout: 120000 });
      await expect(page.getByRole("group", { name: "Thursday, February 29, 2024", exact: true })).toBeVisible({ timeout: 30000 });
      const startup = await page.evaluate(() => ({ ...document.querySelector("[data-doroti-first-frame-at]").dataset }));
      assert.ok(Number(startup.dorotiFirstFrameAt) > 0);
      assert.equal(startup.dorotiRuntimeThreads, String(mode === "threaded"));
      await page.evaluate(() => globalThis.__dorotiFrameCost("reset"));
      for (let index = 0; index < 12; index++) {
        await page.getByRole("button", { name: "English date", exact: true }).click({ force: true });
        await expect(page.getByRole("group", { name: index % 2 === 0 ? "Friday, March 1, 2024" : "Thursday, February 29, 2024", exact: true })).toBeVisible();
      }
      async function settle() {
        await expect.poll(() => page.evaluate(async () => {
          const module = await import(new URL("./_content/Doroti.Host.Web/doroti.web.js", document.baseURI));
          const sequence = JSON.parse(globalThis.__dorotiResizeDiagnostics.snapshot(1)).inputSequence;
          return JSON.parse(module.captureResizeTrace(1)).some(frame => frame.phase === "front-commit" && frame.inputSequence >= sequence);
        }), { timeout: 30000 }).toBe(true);
      }
      await settle();
      const cost = await page.evaluate(() => globalThis.__dorotiFrameCost("finish"));
      assert.equal(cost.trace.dropped, 0);
      const paints = cost.trace.rows.filter(row => row[0] === 3 && row[5] >= 0).map(row => row[4] - row[3]);
      const frames = cost.trace.rows.filter(row => row[0] === 1).map(row => row[4] - row[3]);
      assert.ok(paints.length >= 12, "Every changed date must cause real managed paint work.");
      assert.ok(frames.length >= 12, "Record the framework frame callback separately from Skia raster work.");
      let korean = null;
      if (!baseline) {
        await page.evaluate(() => globalThis.__dorotiFrameCost("reset"));
        await page.getByRole("button", { name: "Korean date", exact: true }).click({ force: true });
        await expect(page.getByRole("group", { name: "2024년 2월 29일 목요일", exact: true })).toBeVisible();
        await settle();
        korean = await page.evaluate(async () => {
          const cost = await globalThis.__dorotiFrameCost("finish");
          const module = await import(new URL("./_content/Doroti.Host.Web/doroti.web.js", document.baseURI));
          return { cost, commits: JSON.parse(module.captureResizeTrace(1)), timeOrigin: performance.timeOrigin };
        });
        assert.equal(korean.cost.trace.dropped, 0);
        if (repeat === 0) await page.screenshot({ path: resolve(output + ".png") });
      }
      const network = await Promise.all(transfers);
      assert.deepEqual(errors, []);
      const sample = { repeat: repeat + 1, browser: browser.version(), startup,
        managedPaintMs: { count: paints.length, p50: percentile(paints, .5), p95: percentile(paints, .95) },
        frameworkFrameCallbackMs: { count: frames.length, p50: percentile(frames, .5), p95: percentile(frames, .95) },
        englishFrameTrace: cost.trace,
        encodedTransferBytes: network.reduce((sum, row) => sum + row.responseBodySize, 0),
        network, korean };
      samples.push(sample);
      console.log(JSON.stringify({ repeat: sample.repeat, firstFrameMs: Number(startup.dorotiFirstFrameAt), framework: sample.frameworkFrameCallbackMs, raster: sample.managedPaintMs }));
    } finally { await browser.close(); }
  }
} finally {
  await fixture.close();
  await writeFile(output, JSON.stringify({ status: samples.length === repeats ? "passed" : "incomplete", baseline, mode, manifest,
    qualification: "Doroti demo format-only opt-in page; headless software GPU and synthetic button input. No widget translation, physical input, scanout or hardware performance acceptance.", samples }, null, 2));
}
