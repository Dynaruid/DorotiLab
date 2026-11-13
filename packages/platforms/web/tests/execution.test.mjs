import assert from "node:assert/strict";
import test from "node:test";
import { pathToFileURL } from "node:url";
import { resolve } from "node:path";

const root = process.env.DOROTI_WEB_MODULE_ROOT;
if (!root) throw new Error("Set DOROTI_WEB_MODULE_ROOT to the production TypeScript output.");
const execution = await import(pathToFileURL(resolve(root, "doroti.web.execution.js")));
const { monoWorkerUrls } = await import(pathToFileURL(resolve(root, "doroti.web.mono.js")));
const { resolveRendererPolicy, selectRendererPolicy } = await import(pathToFileURL(resolve(root, "doroti.web.policy.js")));
const base = "https://example.test/demo/";
const manifest = { schemaVersion: 2, hostContract: "doroti.web.execution/v1", buildId: "generation-a", runtimeContract: "mono-10", runtimeVersion: "10.0.11",
  variants: Object.fromEntries(["threaded", "compatible"].map(mode => [mode, {
    threads: mode === "threaded", loaderUrl: `runtimes/${mode}/_framework/dotnet.fingerprint.js`, profileUrl: `runtimes/${mode}/doroti.runtime-profile.json`,
    renderers: mode === "threaded" ? ["worker-direct-webgl", "worker-direct-webgpu"] : ["worker-direct-webgl"],
  }])) };
const isolated = { secureContext: true, isolated: true, sharedMemory: true };

test("auto uses actual isolation and shared-memory conditions, preserving the application base", () => {
  for (const capabilities of [isolated, { ...isolated, secureContext: false }, { ...isolated, isolated: false }, { ...isolated, sharedMemory: false }]) {
    const selected = execution.selectExecution(execution.parseExecutionManifest(manifest), "auto", capabilities, base);
    const expected = Object.values(capabilities).every(Boolean) ? "threaded" : "compatible";
    assert.equal(selected.selected, expected);
    assert.equal(selected.applicationBaseUrl, base);
    assert.equal(selected.loaderUrl, `${base}runtimes/${expected}/_framework/dotnet.fingerprint.js`);
  }
});
test("explicit modes and legacy conflicts never silently change the request", () => {
  assert.equal(execution.selectExecution(manifest, "compatible", isolated, base).selected, "compatible");
  assert.throws(() => execution.selectExecution(manifest, "threaded", { ...isolated, isolated: false }, base), /DOROTIWEB035/);
  assert.throws(() => execution.selectExecution(manifest, "threaded", isolated, base, "worker"), /DOROTIWEB034/);
  assert.equal(execution.selectExecution(manifest, "auto", isolated, base, "worker").reason, "legacy-runtime-location");
  assert.throws(() => execution.selectExecution(manifest, "auto", isolated, base, "paint"), /DOROTIWEB034/);
});
test("v1 and single-variant deployments do not invent a missing runtime", () => {
  for (const threads of [true, false]) {
    const legacy = execution.parseExecutionManifest({ schemaVersion: 1, threads });
    assert.equal(execution.selectExecution(legacy, "auto", isolated, base).selected, threads ? "threaded" : "compatible");
    if (threads) assert.throws(() => execution.selectExecution(legacy, "auto", { ...isolated, isolated: false }, base), /not published/);
  }
  const compatible = { ...manifest, variants: { compatible: manifest.variants.compatible } };
  assert.equal(execution.selectExecution(compatible, "auto", isolated, base).reason, "compatible-only-deployment");
});
test("invalid schemas, runtime flavors, profiles and cross-origin assets fail before import", () => {
  for (const value of [null, [], {}, { schemaVersion: 1, threads: "false" }, { ...manifest, schemaVersion: 3 },
    { ...manifest, runtimeContract: "coreclr-11" }, { ...manifest, runtimeVersion: "11.0.0" }, { ...manifest, variants: {} },
    { schemaVersion: 1, threads: true, runtimeContract: "mono-10" },
    { schemaVersion: 1, threads: false, runtimeVersion: 10 },
    { ...manifest, variants: { compatible: { ...manifest.variants.compatible, renderers: ["worker-direct-webgpu"] } } },
    { ...manifest, variants: { threaded: { ...manifest.variants.threaded, threads: false } } }])
    assert.throws(() => execution.parseExecutionManifest(value));
  for (const url of ["https://other.test/dotnet.js", "data:text/javascript,export{}", "//other.test/a", "https://user:password@example.test/a"])
    assert.throws(() => execution.sameOriginRuntimeUrl(url, base), /DOROTIWEB033/);
  assert.throws(() => execution.executionMode("fallback"), /DOROTIWEB030/);
  assert.throws(() => execution.selectExecution({ ...manifest, variants: { ...manifest.variants,
    threaded: { ...manifest.variants.threaded, loaderUrl: "https://other.test/dotnet.js" } } }, "compatible", isolated, base), /DOROTIWEB033/);
});
test("variant generation, runtime version and threads must all match", () => {
  const selected = execution.selectExecution(manifest, "compatible", isolated, base);
  const profile = { schemaVersion: 1, threads: false, buildId: manifest.buildId, runtimeContract: "mono-10", runtimeVersion: manifest.runtimeVersion };
  execution.verifyVariantProfile(profile, selected);
  for (const bad of [{ ...profile, buildId: "stale" }, { ...profile, threads: true }, { ...profile, runtimeVersion: "10.0.14" }])
    assert.throws(() => execution.verifyVariantProfile(bad, selected), /DOROTIWEB037/);
});
test("actual runtime build info and heap reject mislabeled builds", () => {
  for (const threads of [false, true]) {
    const selection = { selected: threads ? "threaded" : "compatible", runtimeContract: "mono-10", runtimeVersion: "10.0.11" };
    const runtime = { runtimeBuildInfo: { wasmEnableThreads: threads, productVersion: "10.0.11" },
      localHeapViewU8: () => new Uint8Array(threads ? new SharedArrayBuffer(8) : new ArrayBuffer(8)) };
    assert.equal(execution.verifyRuntime(runtime, selection).sharedHeap, threads);
    assert.throws(() => execution.verifyRuntime({ ...runtime, localHeapViewU8: () => new Uint8Array(threads ? new ArrayBuffer(8) : new SharedArrayBuffer(8)) }, selection), /DOROTIWEB038/);
    assert.throws(() => execution.verifyRuntime({ ...runtime, runtimeBuildInfo: { ...runtime.runtimeBuildInfo, productVersion: "11.0.0" } }, selection), /DOROTIWEB038/);
  }
});
test("Mono 10 uses runtime metadata and Mono 11 only the exact selected loader entry", () => {
  const loader = `${base}runtimes/threaded/_framework/dotnet.loader-hash.js`;
  const config = { resources: { jsModuleWorker: [{ name: "dotnet.native.worker.real-hash.mjs" }] } };
  const ten = monoWorkerUrls("mono-10", loader, config);
  assert.deepEqual([...ten], [`${base}runtimes/threaded/_framework/dotnet.native.worker.real-hash.mjs`]);
  const eleven = monoWorkerUrls("mono-11", loader, {});
  assert.ok(eleven.has(loader));
  for (const unrelated of [`${base}dotnet.loader-hash.js`, `${loader}?other-worker=1`, `${base}other-worker.js`]) assert.ok(!eleven.has(unrelated));
  assert.throws(() => monoWorkerUrls("mono-10", loader, {}), /DOROTIWEB040/);
  assert.throws(() => monoWorkerUrls("mono-10", loader, { resources: { jsModuleWorker: [{ name: "https://other.test/worker.mjs" }] } }), /DOROTIWEB040/);
});
test("Android auto in compatible mode uses WebGL2; explicit WebGPU remains explicit", async () => {
  const auto = selectRendererPolicy("", { userAgent: "Android", platform: "Linux", maxTouchPoints: 5 });
  assert.equal((await resolveRendererPolicy(auto, "worker")).selected, "worker-direct-webgl");
  const explicit = { ...auto, requested: "worker-direct-webgpu" };
  assert.equal((await resolveRendererPolicy(explicit, "worker")).selected, "worker-direct-webgpu");
});
