// Run with node --experimental-vm-modules through run-with-timeout.py.
// Execute the compiled Worker queue/render functions with a controlled GPU
// completion gate. This verifies admission, not physical display latency.
import assert from 'node:assert/strict';
import { readFile } from 'node:fs/promises';
import { resolve, join } from 'node:path';
import { createContext, SourceTextModule, SyntheticModule } from 'node:vm';

const root = resolve(process.argv[2] ?? 'Doroti/src/Doroti.Host.Web/obj/Release/net10.0/Doroti.Web/wwwroot');
async function run(replaceDuringWait) {
  const messages = [], rendered = [], completed = [];
  let release;
  const gate = new Promise(resolve => { release = resolve; });
  const epoch = { generation: 1, logicalWidth: 100, logicalHeight: 100,
    physicalWidth: 100, physicalHeight: 100, devicePixelRatio: 1, timestampMicroseconds: 1 };
  const fixture = {
    epoch, gpu: { waitForCapacity: () => gate, submitted() {} },
    surface: { RenderGraphiteFrame(id) { rendered.push(id); return 'exact-rendered'; },
      CompleteFrame(id, generation, terminal) { completed.push({ id, generation, terminal }); } },
  };
  const context = createContext({ fixture, performance, URLSearchParams,
    addEventListener() {}, postMessage: message => messages.push(message),
    setTimeout: () => 1, clearTimeout() {} });
  const noop = () => {};
  const host = { configureWorkerBridge: noop, commitPlatformFrame: async () => true,
    discardPlatformFrame: noop, drainPlatformViews: noop, drainPlatformCaptures: noop,
    dispatchWorkerAnimationFrame: noop, dispatchWorkerInput: noop, dispatchWorkerResizeEpoch: noop,
    dispatchWorkerSnapshot: noop, initializeManagedCallbacks: noop };
  const modules = new Map();
  async function load(name) {
    if (modules.has(name)) return modules.get(name);
    const stub = name === './doroti.web.js' ? host : name === './doroti.web.texture-worker.js'
      ? { flushRetired: async () => {} } : null;
    const module = stub ? new SyntheticModule(Object.keys(stub), function () {
      for (const [key, value] of Object.entries(stub)) this.setExport(key, value);
    }, { context }) : new SourceTextModule(await readFile(join(root, name), 'utf8'), { context });
    modules.set(name, module);
    return module;
  }
  const source = await readFile(join(root, 'doroti.raster.worker.js'), 'utf8');
  // Test-only access to existing module state; production source is unchanged.
  const worker = new SourceTextModule(source + `
    runtimeState.transition('booting'); runtimeState.transition('ready');
    webgpu = fixture.gpu; surface = fixture.surface;
    snapshot = { resizeEpoch: fixture.epoch, surfaceGeneration: 1, visible: true };
    latestAdmissionGeneration = 1;
    presenter = { canvas: { width: 100, height: 100 }, contextGeneration: 1,
      current: null, latest: null, draining: false, nextRequestId: 0, contextLost: false };
    export function enqueue() { requestPresent(fixture.epoch); }
    export function idle() { return !presenter.draining && !presenter.current && !presenter.latest; }
  `, { context });
  await worker.link(load);
  await worker.evaluate();
  worker.namespace.enqueue();
  assert.deepEqual(rendered, [], 'capacity gate must block raster');
  if (replaceDuringWait) {
    worker.namespace.enqueue();
    worker.namespace.enqueue();
  }
  release();
  for (let i = 0; i < 30 && !worker.namespace.idle(); i++) await Promise.resolve();
  assert.equal(worker.namespace.idle(), true, 'queue must drain');
  assert.deepEqual(rendered, [replaceDuringWait ? 3 : 1]);
  assert.deepEqual(completed, [{ id: replaceDuringWait ? 3 : 1, generation: 1, terminal: 'submitted' }]);
  const terminal = messages.filter(m => m.kind === 'terminal').sort((a, b) => a.requestId - b.requestId);
  assert.deepEqual(terminal.map(m => [m.requestId, m.terminal]), replaceDuringWait
    ? [[1, 'superseded'], [2, 'superseded'], [3, 'submitted']] : [[1, 'submitted']]);
  assert.equal(messages.filter(m => m.kind === 'direct-commit').length, 1);
  assert.equal(messages.some(m => m.kind === 'fatal'), false);
}
await run(false);
await run(true);
console.log('PASS GPU capacity gate: unchanged frame submits; replaced requests never raster; latest frame submits once');
