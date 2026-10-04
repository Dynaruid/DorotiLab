import assert from 'node:assert/strict';
import { readFile } from 'node:fs/promises';
import { stripTypeScriptTypes } from 'node:module';
import { test } from 'node:test';
import vm from 'node:vm';
import { dorotiWebGpuRendererVersion } from '../src/Doroti.Host.Web/Web/doroti.web.protocol.ts';

const tick = () => new Promise(resolve => setImmediate(resolve));
// The VM canvas is not a native transferable. Keep its identity while modeling
// the same private-port message queue and closure contract.
class TestPort extends EventTarget {
  peer!: TestPort; closed=false;
  start() {}
  close() { this.closed=true; }
  toJSON() { return {closed:this.closed}; }
  postMessage(data: unknown) { queueMicrotask(() => { if (!this.peer.closed) this.peer.dispatchEvent(new MessageEvent('message',{data})); }); }
}
class TestChannel {
  port1=new TestPort(); port2=new TestPort();
  constructor() { this.port1.peer=this.port2; this.port2.peer=this.port1; }
}

// Execute the production raster-role entry, not just its lifetime helper.
// Await barriers expose disposal before each managed resource becomes ready.
for (const managed of [false, true]) for (const stage of managed
    ? ['callbacks','host-exports','gpu-initialize','surface-initialize','app-exports','app-start','textures']
    : ['create', 'callbacks', 'host-exports', 'app-exports', 'app-start', 'textures']) {
  test(`actual ${managed ? 'main-owned WebGPU' : 'standalone WebGL'} raster worker closes during ${stage} and suppresses late readiness`, { timeout:5000 }, async t => {
    const messages: any[] = [], stages: string[] = [];
    let release!: () => void, reached!: () => void;
    const paused = new Promise<void>(resolve => { release = resolve; });
    const atStage = new Promise<void>(resolve => { reached = resolve; });
    async function step<T>(name: string, value: T): Promise<T> {
      stages.push(name);
      if (name === stage) { reached(); await paused; }
      return value;
    }
    let exits = 0, stops = 0, closed = 0;
    let backend: any;
    const app = { Doroti: { Generated: { DorotiBootstrap: {
      StartWorker: () => step('app-start', 'started'), StopWorker: () => { stops++; },
    } } } };
    const surface = { ConfigureMemoryProfile() {}, CaptureDiagnostics: () => '{}',
      InitializeBrowserTextures: () => step('textures', undefined), DisposeGraphite: async () => {},
      InitializeGraphite: () => step('surface-initialize', undefined),
      CompleteFrame() {}, CaptureCostDiagnostics: () => '{}' };
    const exports = { Doroti: { Host: { Web: { DorotiWebWorkerSurface: surface,
      BrowserTimeProvider: { CaptureDiagnostics: () => '{}' }, BrowserManagedRenderThread: { CaptureThreadId: () => 1 } } } } };
    const runtime = { exit: () => { exits++; }, getConfig: () => ({ mainAssemblyName: 'app' }),
      getAssemblyExports: (name: string) => name === 'Doroti.Host.Web.dll' ? step('host-exports', exports) : step('app-exports', app) };
    const events = new EventTarget();
    class Canvas { width = 4; height = 4; }
    let hostPort: any;
    const context = vm.createContext({ URL, EventTarget, MessageEvent, MessageChannel:TestChannel, DOMException, AggregateError, console, performance,
      setTimeout, clearTimeout, queueMicrotask, crossOriginIsolated: true, OffscreenCanvas: Canvas,
      ...(managed ? {getDotnetRuntime: () => runtime} : {}),
      addEventListener: events.addEventListener.bind(events), postMessage: (value: any) => {
        messages.push(value);
        if (value.kind === 'fatal') reached();
        if (value.kind === 'doroti-managed-port') {
          hostPort = value.port;
          hostPort.addEventListener('message', (event: any) => { const message=event.data; messages.push(message); if (message.kind === 'fatal') reached(); });
          hostPort.postMessage({protocolVersion:5,kind:'managed-accepted',sessionToken:'fixture'});
        }
      },
      close: () => { closed++; }, fixture: { runtime, step, setBackend(value: any) { backend = value; } } });
    const cache = new Map<string, vm.SourceTextModule>();
    async function load(url: URL): Promise<vm.SourceTextModule> {
      if (cache.has(url.href)) return cache.get(url.href)!;
      const stub = url.pathname.endsWith('/fake-dotnet.ts') ? 'export const dotnet = { withEnvironmentVariables() { return { create() { return fixture.step("create", fixture.runtime); } }; } };'
        : url.pathname.endsWith('/doroti.web.ts') ? `
          export function configureWorkerBridge(value) { fixture.setBackend(value); }
          export const initializeManagedCallbacks = () => fixture.step("callbacks", undefined);
          export async function commitPlatformFrame() { return true; }
          export function discardPlatformFrame() {} export async function drainPlatformViews() {}
          export async function drainPlatformCaptures() {} export function stagePlatformBitmap() {}
          export function dispatchWorkerAnimationFrame() {} export function dispatchWorkerInput() {}
          export function dispatchWorkerResizeEpoch() {} export function dispatchWorkerSnapshot() {}`
        : url.pathname.endsWith('/doroti.web.gpu-effects.ts') ? 'export function releaseEffectPrograms() {} export function allocateEffect() {} export function executeEffect() {}'
        : url.pathname.endsWith('/doroti.webgpu.ts') ? `export const rendererContractVersion=1;
          export const initialize=()=>fixture.step('gpu-initialize', {api:'webgpu'});
          export async function drainForShutdown() {} export function releaseNative() {}`
        : undefined;
      const source = stub ?? stripTypeScriptTypes(await readFile(url, 'utf8'), { mode: 'transform' });
      const module = new vm.SourceTextModule(source, { context, identifier: url.href,
        initializeImportMeta(meta) { meta.url = url.href; },
        importModuleDynamically: async (specifier, parent) => { const next = await load(new URL(specifier.replace(/\.js$/, '.ts'), parent.identifier)); await next.evaluate(); return next; } });
      cache.set(url.href, module);
      await module.link((specifier, parent) => load(new URL(specifier.replace(/\.js$/, '.ts'), parent.identifier)));
      return module;
    }
    const worker = await load(new URL('../src/Doroti.Host.Web/Web/doroti.raster.worker.ts', import.meta.url)); await worker.evaluate();
    let role: Promise<void> | undefined;
    if (managed) role = (worker.namespace as any).startSharedRuntimeRole('fixture');
    const send = (kind: string, extra: any = {}) => {
      const data = { protocolVersion: 5, kind, ...extra };
      if (managed) hostPort.postMessage(data); else events.dispatchEvent(new MessageEvent('message', { data }));
    };
    t.after(() => { if (managed) send('dispose'); hostPort?.close(); release(); });
    const snapshot = { generation: 1, surfaceGeneration: 1, environmentGeneration: 1, logicalWidth: 4, logicalHeight: 4, devicePixelRatio: 1,
      resizeEpoch: { generation: 1, logicalWidth: 4, logicalHeight: 4, physicalWidth: 4, physicalHeight: 4, devicePixelRatio: 1, timestampMicroseconds: 0 } };
    const mode = managed ? 'worker-direct-webgpu' : 'worker-direct-webgl';
    send('init', { mode, rendererContractVersion:dorotiWebGpuRendererVersion, policy: { selected: mode, memoryProfile: 'desktop' },
      snapshot, canvas: new Canvas(), dotnetModuleUrl: new URL('../src/Doroti.Host.Web/Web/fake-dotnet.js', import.meta.url).href });
    await atStage;
    send('dispose'); send('dispose');
    if (role) { await role; await tick(); await tick(); }
    for (let turn = 0; turn < 10 && !messages.some(value => value.kind === 'disposed'); turn++) await tick();
    assert.equal(messages.filter(value => value.kind === 'disposed').length, 1, JSON.stringify(messages));
    assert.equal(messages.filter(value => value.kind === 'fatal').length, 0, JSON.stringify(messages));
    assert.equal(closed, managed ? 0 : 1);
    release(); await tick(); await tick();
    backend.requestFrame(1, 1); if (!managed) send('input', { inputSequence: 99 });
    assert.equal(messages.filter(value => value.kind === 'runtime-ready').length, 0);
    assert.equal(messages.filter(value => value.kind === 'fatal').length, 0, JSON.stringify(messages));
    assert.equal(exits, managed ? 0 : 1); if (stage === 'app-start' || stage === 'textures') assert.ok(stops >= 1);
    hostPort?.close();
  });
}
