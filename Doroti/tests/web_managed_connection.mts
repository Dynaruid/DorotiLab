import assert from 'node:assert/strict';
import { readFile } from 'node:fs/promises';
import { stripTypeScriptTypes } from 'node:module';
import { test } from 'node:test';
import vm from 'node:vm';
import { MessageChannel, MessagePort } from 'node:worker_threads';
import { randomUUID } from 'node:crypto';
const tick = () => new Promise(resolve => setImmediate(resolve));

async function supervisor() {
  const timers = new Map<number, () => void>(); let nextTimer = 0;
  const canceled: string[] = [], started: string[] = [];
  let worker: EventTarget | undefined;
  class NativeWorker extends EventTarget {
    constructor(..._: unknown[]) { super(); worker = this; }
    terminate() { assert.fail('Runtime-owned pthread Worker was terminated.'); }
  }
  const runtime = { runtimeBuildInfo: { wasmEnableThreads: true }, localHeapViewU8: () => new Uint8Array(new SharedArrayBuffer(4)),
    getAssemblyExports: async () => ({ Doroti: { Host: { Web: { BrowserManagedRenderThread: {
      StartAsync: (_: string, token: string) => { started.push(token); new (context.Worker as any)('http://127.0.0.1/_framework/dotnet.native.worker.mjs'); return new Promise<void>(() => {}); },
      CancelAsync: async (token: string) => { canceled.push(token); },
    } } } } }) };
  const context = vm.createContext({ URL, EventTarget, MessageEvent, MessageChannel, MessagePort, SharedArrayBuffer, Uint8Array,
    crossOriginIsolated: true, Worker: NativeWorker, location: new URL('http://127.0.0.1/'), crypto: { randomUUID }, fixture: { runtime },
    document: { baseURI: 'http://127.0.0.1/', documentElement: { dataset: {} }, querySelector: () => null },
    addEventListener() {}, removeEventListener() {}, setTimeout(callback: () => void) { const id=++nextTimer; timers.set(id,callback); return id; },
    clearTimeout(id: number) { timers.delete(id); } });
  const cache = new Map<string, vm.SourceTextModule>();
  async function load(url: URL): Promise<vm.SourceTextModule> {
    if (cache.has(url.href)) return cache.get(url.href)!;
    const stub = url.pathname.endsWith('/fake-dotnet.ts') ? 'export const dotnet={withEnvironmentVariables(){return {create: async()=>fixture.runtime};}};'
      : url.pathname.endsWith('/doroti.web.hot-reload.ts') ? 'export function developmentBridge() {} export async function startBrowserHotReload() {}' : undefined;
    const source=stub ?? stripTypeScriptTypes(await readFile(url,'utf8'),{mode:'transform'});
    const module=new vm.SourceTextModule(source,{context,identifier:url.href,
      importModuleDynamically:async(specifier,parent)=>{const next=await load(new URL(specifier.replace(/\.js$/,'.ts'),parent.identifier));await next.evaluate();return next;}});
    cache.set(url.href,module);await module.link((specifier,parent)=>load(new URL(specifier.replace(/\.js$/,'.ts'),parent.identifier)));return module;
  }
  const module=await load(new URL('../../packages/platforms/web/Doroti.Host.Web/Web/doroti.web.managed-worker.ts',import.meta.url));await module.evaluate();
  return { api:module.namespace as any, timers, canceled, started,
    connect(token:string,port:MessagePort) { worker!.dispatchEvent(new MessageEvent('message',{data:{protocolVersion:5,kind:'doroti-managed-port',sessionToken:token,port}})); },
    crashOtherRuntimeWorker() {
      const finalizer = new (context.Worker as any)('http://127.0.0.1/_framework/dotnet.native.worker.finalizer.mjs');
      const error = new Event('error'); Object.defineProperty(error,'message',{value:'finalizer crashed'});
      finalizer.dispatchEvent(error);
    },
  };
}

test('managed connection timeout cancels its managed token and rejects/ACKs a late port', async () => {
  const host=await supervisor();
  const pending=host.api.createManagedDorotiWorker('http://127.0.0.1/_framework/fake-dotnet.js',new URL('http://127.0.0.1/role.js'));
  const rejected=assert.rejects(pending,/canceled or timed out/);
  await tick(); await tick();
  assert.equal(host.started.length,1); assert.equal(host.timers.size,1);
  host.timers.values().next().value!(); await rejected;
  assert.deepEqual(host.canceled,host.started);
  const channel=new MessageChannel(); const messages:any[]=[];
  channel.port1.on('message',value=>messages.push(value));
  host.connect(host.started[0],channel.port2); await tick(); await tick();
  assert.equal(messages[0].kind,'managed-rejected');
  channel.port1.close(); channel.port2.close();
});

test('a different runtime pthread failure rejects a pending managed connection immediately', async () => {
  const host=await supervisor();
  const pending=host.api.createManagedDorotiWorker('http://127.0.0.1/_framework/fake-dotnet.js',new URL('http://127.0.0.1/role.js'));
  const rejected=assert.rejects(pending,/finalizer crashed/);
  await tick(); await tick();
  host.crashOtherRuntimeWorker(); await rejected;
  assert.equal(host.timers.size,0);
});

test('a finalizer pthread failure notifies an active renderer once and rejects reuse of the aborted runtime', async () => {
  const host=await supervisor();
  const pending=host.api.createManagedDorotiWorker('http://127.0.0.1/_framework/fake-dotnet.js',new URL('http://127.0.0.1/role.js'));
  await tick(); await tick();
  const channel=new MessageChannel();
  host.connect(host.started[0],channel.port2);
  const endpoint=await pending;
  const failures:any[]=[];endpoint.addEventListener('message',(event:any)=>failures.push(event.data));
  host.crashOtherRuntimeWorker();host.crashOtherRuntimeWorker();
  assert.equal(failures.length,1);assert.equal(failures[0].kind,'fatal');
  assert.match(failures[0].error,/finalizer crashed/);
  await assert.rejects(host.api.createManagedDorotiWorker('http://127.0.0.1/_framework/fake-dotnet.js',new URL('http://127.0.0.1/role.js')),/finalizer crashed/);
  assert.equal(host.started.length,1);
  channel.port1.close();channel.port2.close();
});

test('managed accept ACK, duplicate/foreign rejection and disposal retain pthread ownership', async () => {
  const host=await supervisor();
  const pending=host.api.createManagedDorotiWorker('http://127.0.0.1/_framework/fake-dotnet.js',new URL('http://127.0.0.1/role.js'));
  await tick(); await tick();
  const channel=new MessageChannel(); const messages:any[]=[];
  channel.port1.on('message',value=>{messages.push(value); if(value.kind==='dispose') channel.port1.postMessage({protocolVersion:5,kind:'disposed'});});
  host.connect(host.started[0],channel.port2);
  const endpoint=await pending; await tick();
  assert.equal(messages[0].kind,'managed-accepted'); assert.equal(host.timers.size,0);
  for(const token of [host.started[0],'foreign']) {
    const duplicate=new MessageChannel(); const replies:any[]=[]; duplicate.port1.on('message',value=>replies.push(value));
    host.connect(token,duplicate.port2); await tick(); await tick(); assert.equal(replies[0].kind,'managed-rejected');
    duplicate.port1.close(); duplicate.port2.close();
  }
  endpoint.terminate(); endpoint.terminate(); await tick(); await tick();
  assert.equal(messages.filter(value=>value.kind==='dispose').length,1);
  channel.port1.close(); channel.port2.close();
});
