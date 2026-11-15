import assert from "node:assert/strict";
import test from "node:test";
import { pathToFileURL } from "node:url";
import { resolve } from "node:path";
const root = process.env.DOROTI_WEB_MODULE_ROOT;
const moduleAt = name => import(pathToFileURL(resolve(root, `doroti.web.${name}.js`)));
const { RenderSessionReadiness, WorkerStartupLifetime, awaitWithAbort } = await moduleAt("lifetime");
const { ManagedEndpoint } = await moduleAt("managed-worker");
const { PointerMoveAdmission } = await moduleAt("pointer-admission");
const { dorotiProtocolVersion: protocolVersion } = await moduleAt("protocol");
const { MonoWorkerPreload } = await moduleAt("mono");
const { closeRetiredWorkerMessage, closeExternalLeases, createDorotiWorker } = await moduleAt("worker-host");

test("retired platform frames close every transferred bitmap exactly once", () => {
  const original = globalThis.ImageBitmap;
  class Bitmap { closed = 0; close() { this.closed++; } }
  globalThis.ImageBitmap = Bitmap;
  try {
    const first = new Bitmap(), second = new Bitmap();
    closeRetiredWorkerMessage({ bitmap: first, controlKind: "platform-frame", payload: { rasters: [{ bitmap: first }, null, { bitmap: second }] } });
    assert.equal(first.closed, 1); assert.equal(second.closed, 1);
  } finally { if (original === undefined) delete globalThis.ImageBitmap; else globalThis.ImageBitmap = original; }
});
test("one failing lease callback cannot leave other retired requests pending", () => {
  const leases = new Map([[1, "a"], [2, "b"]]), closed = [];
  assert.throws(() => closeExternalLeases(leases, id => { closed.push(id); if (id === 1) throw new Error("receipt failed"); }), AggregateError);
  assert.deepEqual(closed, [1, 2]); assert.equal(leases.size, 0);
});
test("an isolated Worker constructor failure releases the bootstrap Blob URL", () => {
  const previous = { Worker: globalThis.Worker, isolated: globalThis.crossOriginIsolated,
    create: URL.createObjectURL, revoke: URL.revokeObjectURL };
  const released = [];
  Object.assign(globalThis, { crossOriginIsolated: true, Worker: class { constructor() { throw new Error("CSP denied"); } } });
  URL.createObjectURL = () => "blob:owned-worker"; URL.revokeObjectURL = value => released.push(value);
  try {
    assert.throws(() => createDorotiWorker(new URL("https://example.test/worker.js")), /CSP denied/);
    assert.deepEqual(released, ["blob:owned-worker"]);
  } finally {
    URL.createObjectURL = previous.create; URL.revokeObjectURL = previous.revoke;
    if (previous.Worker === undefined) delete globalThis.Worker; else globalThis.Worker = previous.Worker;
    if (previous.isolated === undefined) delete globalThis.crossOriginIsolated; else globalThis.crossOriginIsolated = previous.isolated;
  }
});

test("Mono 11 startup waits for native preload receipts and lets runtime handlers run first", async () => {
  const preload = new MonoWorkerPreload();
  const workers = [new EventTarget(), new EventTarget()];
  workers.forEach(worker => preload.observe(worker));
  let ready = false, nativeHandled = 0;
  workers.forEach(worker => worker.addEventListener("message", event => { if (event.data.cmd === 3) nativeHandled++; }));
  const waiting = preload.ready().then(() => { ready = true; assert.equal(nativeHandled, 2); });
  workers[0].dispatchEvent(new MessageEvent("message", { data: { cmd: "preload" } }));
  await new Promise(resolveTurn => setTimeout(resolveTurn, 0)); assert.equal(ready, false);
  workers[0].dispatchEvent(new MessageEvent("message", { data: { cmd: 3 } }));
  await new Promise(resolveTurn => setTimeout(resolveTurn, 0)); assert.equal(ready, false);
  workers[1].dispatchEvent(new MessageEvent("message", { data: { cmd: 3 } }));
  await waiting; assert.equal(ready, true);
});
test("a pthread failure rejects preload instead of leaving startup pending", async () => {
  const preload = new MonoWorkerPreload(); preload.observe(new EventTarget());
  const waiting = preload.ready(); preload.fail(new Error("pthread download failed"));
  await assert.rejects(waiting, /pthread download failed/);
  await assert.rejects(preload.ready(), /pthread download failed/);
});

test("startup needs both runtime readiness and a real commit, in either order", () => {
  for (const frameFirst of [false, true]) {
    let ready = 0;
    const startup = new RenderSessionReadiness(1000, () => ready++, assert.fail);
    startup.begin(1);
    if (frameFirst) startup.firstFrame(1); else startup.runtimeReady(1);
    assert.equal(ready, 0);
    if (frameFirst) startup.runtimeReady(1); else startup.firstFrame(1);
    assert.equal(ready, 1);
    startup.runtimeReady(1); startup.firstFrame(1);
    assert.equal(ready, 1);
  }
});
test("replaced sessions ignore late readiness and frames", () => {
  let ready = 0;
  const startup = new RenderSessionReadiness(1000, () => ready++, assert.fail);
  startup.begin(1); startup.runtimeReady(1);
  startup.begin(2); startup.firstFrame(1); startup.runtimeReady(1);
  startup.firstFrame(2); assert.equal(ready, 0);
  startup.runtimeReady(2); assert.equal(ready, 1);
});
test("missing first frame fails with its cause and closes readiness", async () => {
  let ready = 0;
  let startup;
  const failure = new Promise(resolveFailure => { startup = new RenderSessionReadiness(5, () => ready++, resolveFailure); });
  startup.begin(3); startup.runtimeReady(3);
  assert.match((await failure).message, /session 3.*first canvas frame/);
  startup.firstFrame(3); assert.equal(ready, 0);
});
test("closing startup cancels waits and retires resources that arrive late", async () => {
  const lifetime = new WorkerStartupLifetime();
  let deliver;
  let retired = 0;
  const waiting = lifetime.wait(new Promise(resolveResource => { deliver = resolveResource; }), () => retired++);
  lifetime.close(); await assert.rejects(waiting, /closed/i);
  deliver({}); await new Promise(resolveTurn => setImmediate(resolveTurn));
  assert.equal(retired, 1);
});
test("page cancellation interrupts a pending runtime await", async () => {
  const abort = new AbortController();
  const waiting = awaitWithAbort(new Promise(() => {}), abort.signal);
  abort.abort(new Error("page closed")); await assert.rejects(waiting, /page closed/);
});
test("managed endpoint disposes on its owner without killing the runtime pthread", async () => {
  const channel = new MessageChannel();
  const native = new EventTarget(); native.terminate = () => assert.fail("Runtime-owned Worker was killed.");
  const endpoint = new ManagedEndpoint(channel.port1, native);
  try {
    const dispose = new Promise(resolveMessage => channel.port2.addEventListener("message", event => resolveMessage(event.data), { once: true }));
    channel.port2.start(); endpoint.terminate(); endpoint.terminate();
    assert.deepEqual(await dispose, { protocolVersion, kind: "dispose" });
    const disposed = new Promise(resolveMessage => endpoint.addEventListener("message", resolveMessage, { once: true }));
    channel.port2.postMessage({ protocolVersion, kind: "disposed" }); await disposed;
    assert.throws(() => endpoint.postMessage({}), /closed/);
  } finally { channel.port1.close(); channel.port2.close(); }
});
test("a wrong protocol cannot masquerade as owner disposal", async () => {
  const channel = new MessageChannel();
  const endpoint = new ManagedEndpoint(channel.port1, new EventTarget());
  try {
    const invalid = new Promise(resolveMessage => endpoint.addEventListener("message", resolveMessage, { once: true }));
    channel.port2.postMessage({ protocolVersion: -1, kind: "disposed" }); await invalid;
    const reply = new Promise(resolveMessage => channel.port2.addEventListener("message", event => resolveMessage(event.data), { once: true }));
    channel.port2.start(); endpoint.postMessage({ kind: "still-owned" });
    assert.deepEqual(await reply, { kind: "still-owned" });
    const disposed = new Promise(resolveMessage => endpoint.addEventListener("message", resolveMessage, { once: true }));
    channel.port2.postMessage({ protocolVersion, kind: "disposed" }); await disposed;
  } finally { channel.port1.close(); channel.port2.close(); }
});
test("pointer batches retain motion order across IME, resize and cancel boundaries", () => {
  const sent = [];
  const inputs = new PointerMoveAdmission(input => sent.push(input), 2);
  const motion = sequence => ({ inputKind: "pointer", hostId: 1, inputSequence: sequence,
    payload: { phase: 0, pointerId: 1, samples: [0, 1, 2, 3, 4, 5, 6] } });
  inputs.push(motion(1)); inputs.push(motion(2)); inputs.push(motion(3));
  inputs.push({ inputKind: "text", hostId: 1, inputSequence: 4, payload: { text: "한글" } });
  inputs.push({ ...motion(5), payload: { phase: 3, pointerId: 1 } });
  assert.deepEqual(sent.flatMap(input => input.pointerBatch ?? [input]).map(input => input.inputSequence), [1, 2, 3, 4, 5]);
  inputs.reset(); inputs.acknowledge(3); assert.equal(sent.length, 4);
});
