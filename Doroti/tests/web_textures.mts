import assert from 'node:assert/strict';
import { readFile } from 'node:fs/promises';
import { stripTypeScriptTypes } from 'node:module';
import { test } from 'node:test';
import vm from 'node:vm';

// Execute the actual main/Worker modules with deterministic GPU/DOM doubles.
// No pixel buffers are allocated; these tests prove admission and lifetime rules.
class Frame {
  closed = 0;
  constructor(public width: number, public height: number) {}
  close() { this.closed++; }
}
class VideoFrame extends Frame {
  constructor(source: { videoWidth: number; videoHeight: number }) { super(source.videoWidth, source.videoHeight); }
  get displayWidth() { return this.width; }
  get displayHeight() { return this.height; }
}
const tick = () => new Promise(resolve => setImmediate(resolve));
async function modules(timers: Record<string, unknown> = {}) {
  const snapshots: Frame[] = [], videoFrames: VideoFrame[] = [];
  class CapturedVideoFrame extends VideoFrame {
    constructor(source: { videoWidth: number; videoHeight: number }) { super(source); videoFrames.push(this); }
  }
  const context = vm.createContext({ EventTarget, MessageEvent, DOMException, setTimeout, clearTimeout,
    queueMicrotask, performance, ImageBitmap: Frame, VideoFrame: CapturedVideoFrame,
    createImageBitmap: async (canvas: Frame) => {
      const frame = new Frame(canvas.width, canvas.height); snapshots.push(frame); return frame;
    }, ...timers });
  const cache = new Map<string, vm.SourceTextModule>();
  async function load(url: URL): Promise<vm.SourceTextModule> {
    const key = url.href;
    if (cache.has(key)) return cache.get(key)!;
    const stub = url.pathname.endsWith('/doroti.web.ts') ? 'export function stagePlatformBitmap() {}'
      : url.pathname.endsWith('/doroti.web.gpu-effects.ts')
        ? 'export function releaseEffectPrograms() {} export function allocateEffect() {} export function executeEffect() {}' : undefined;
    const source = stub ?? stripTypeScriptTypes(await readFile(url, 'utf8'), { mode: 'transform' });
    const module = new vm.SourceTextModule(source, { context, identifier: key });
    cache.set(key, module);
    await module.link((specifier, parent) => load(new URL(specifier.replace(/\.js$/, '.ts'), parent.identifier)));
    return module;
  }
  const main = await load(new URL('../src/Doroti.Host.Web/Web/doroti.web.textures.ts', import.meta.url));
  const worker = await load(new URL('../src/Doroti.Host.Web/Web/doroti.web.texture-worker.ts', import.meta.url));
  await main.evaluate(); await worker.evaluate();
  return { main: main.namespace as any, worker: worker.namespace as any, snapshots, videoFrames };
}

function initialize(worker: any, backend: string, limit = 8192) {
  let id = 0, handle = 0, destroyed = 0;
  const released: number[] = [];
  const completions: (() => void)[] = [];
  const gl = new Proxy({ MAX_TEXTURE_SIZE: 1, NO_ERROR: 0, TIMEOUT_EXPIRED: 2, WAIT_FAILED: 3,
    getParameter: (key: number) => key === 1 ? limit : 0,
    isContextLost: () => false, createTexture: () => ({}), getError: () => 0,
    deleteTexture: () => { destroyed++; }, fenceSync: () => ({}), clientWaitSync: () => 4,
  }, { get: (object, key) => key in object ? object[key as keyof typeof object] : () => {} });
  worker.initializeTextures({ RegisterBrowserTexture: () => String(++id), MarkBrowserTexture() {},
    UnregisterBrowserTexture() {}, ReleaseBrowserTextureImage: (token: number) => released.push(token) },
  backend === 'webgpu' ? { textureDimensionLimit: () => limit,
    copyTextureSource: () => ({ handle: ++handle, destroy: () => { destroyed++; } }),
    textureWorkDone: () => new Promise<void>(resolve => completions.push(resolve)), diagnostics: () => ({}) } : undefined,
  () => ({ textures: [], getNewId: () => ++handle, currentContext: { GLctx: gl } }));
  return { released, completions, get destroyed() { return destroyed; } };
}

test('main texture timeout preserves Timeout and cancels siblings without retaining timers', async () => {
  const timers = new Map<number, () => void>();
  let nextTimer = 0;
  const { main } = await modules({ setTimeout(callback: () => void) { const id = ++nextTimer; timers.set(id, callback); return id; },
    clearTimeout(id: number) { timers.delete(id); } });
  class Endpoint extends EventTarget {
    sent: any[] = [];
    postMessage(message: any) { this.sent.push(message); }
  }
  const endpoint = new Endpoint(), registry = new main.BrowserTextureRegistry(endpoint);
  const first = registry.request('diagnostics'), second = registry.request('diagnostics');
  const firstCheck = assert.rejects(first, (error: any) => error.code === 'Timeout');
  const secondCheck = assert.rejects(second, (error: any) => error.code === 'Disposed');
  assert.equal(timers.size, 2);
  timers.values().next().value!();
  await Promise.all([firstCheck, secondCheck]);
  assert.equal(timers.size, 0);
  endpoint.dispatchEvent(new MessageEvent('message', { data: { protocolVersion: 5, kind: 'texture-response', request: 1 } }));
  await assert.rejects(registry.request('diagnostics'), (error: any) => error.code === 'Disposed');
  assert.equal(endpoint.sent.length, 2);
});

for (const backend of ['webgl', 'webgpu']) {
  test(`${backend}: 4K, 5K and 8K retain bounded destinations through retirement and resize`, async () => {
    const { worker } = await modules();
    const device = initialize(worker, backend);
    const registration = await worker.textureMessage({ operation: 'register', request: 1 });
    let sequence = 0;
    const send = (frame: Frame) => worker.textureMessage({ operation: 'frame', request: 2,
      ...registration, sourceGeneration: 1, sequence: ++sequence, frame });
    for (const [width, height] of [[3840, 2160], [5120, 2880], [7680, 4320]]) {
      const size = width * height * 4;
      const leases: any[] = [];
      for (let i = 0; i < 3; i++) {
        const frame = new Frame(width, height); await send(frame);
        const lease = worker.acquireTexture(registration.textureId);
        assert.ok(lease); assert.equal(frame.closed, 1); leases.push(lease);
      }
      const pending = new Frame(width, height); await send(pending);
      assert.equal(worker.acquireTexture(registration.textureId), null);
      assert.equal(pending.closed, 0);
      assert.equal(worker.diagnostics().gpuBytes, size * 3);
      assert.equal(worker.diagnostics().viewBudget, size * 4);
      for (const lease of leases) worker.retireTexture(lease.token);
      const drain = worker.flushRetired();
      if (backend === 'webgpu') {
        assert.equal(worker.diagnostics().gpuBytes, size * 3);
        assert.equal(worker.acquireTexture(registration.textureId), null);
        device.completions.shift()!();
      }
      await drain;
      const lease = worker.acquireTexture(registration.textureId);
      assert.ok(lease); assert.equal(pending.closed, 1);
      worker.retireTexture(lease.token);
      const last = worker.flushRetired(); device.completions.shift()?.(); await last;
    }
    const budget = worker.diagnostics().viewBudget;
    const canvas = { width: 320, height: 180 };
    const local = worker.registerLocalCanvas(canvas);
    canvas.width = 7680; canvas.height = 4320; local.markFrameAvailable();
    const localLease = worker.acquireTexture(local.textureId);
    assert.equal(localLease.width, 7680);
    worker.retireTexture(localLease.token);
    const done = worker.flushRetired(); device.completions.shift()?.(); await done;
    await send(new Frame(320, 180));
    assert.equal(worker.diagnostics().viewBudget, budget);
    const invalid = new Frame(8193, 1);
    await assert.rejects(send(invalid), /device limit/); assert.equal(invalid.closed, 1);
    assert.equal(worker.diagnostics().pendingBytes, 320 * 180 * 4);
    await local.dispose(); await worker.disposeTextures();
    const result = worker.diagnostics();
    assert.equal(result.pendingBytes, 0); assert.equal(result.gpuBytes, 0);
    assert.equal(result.received, result.closed);
    assert.equal(result.imported, result.retired);
    assert.equal(device.destroyed, device.released.length);
  });
}

test('main source admission uses device dimensions, bounds 8K candidates and closes replacements', async () => {
  const { main, worker } = await modules(); initialize(worker, 'webgpu');
  const held: any[] = [];
  class Endpoint extends EventTarget {
    postMessage(message: any) {
      if (message.operation === 'frame') { held.push(message); return; }
      void worker.textureMessage(message).then((result: any) => this.reply(message, result));
    }
    reply(message: any, result = {}) {
      this.dispatchEvent(new MessageEvent('message', { data: { ...result, kind: 'texture-response',
        protocolVersion: 5, request: message.request } }));
    }
  }
  const endpoint = new Endpoint(); const registry = new main.BrowserTextureRegistry(endpoint);
  const a = await registry.createFrameProducer(), b = await registry.createFrameProducer(), c = await registry.createFrameProducer();
  const frames = Array.from({ length: 6 }, () => new Frame(7680, 4320));
  assert.equal(a.pushFrame(frames[0]), true); assert.equal(a.pushFrame(frames[1]), true);
  assert.equal(b.pushFrame(frames[2]), true); assert.equal(b.pushFrame(frames[3]), true);
  assert.equal(c.pushFrame(frames[4]), false); assert.equal(frames[4].closed, 0);
  assert.equal(a.pushFrame(frames[5]), true); assert.equal(frames[1].closed, 1);
  assert.equal(registry.sourceBytes, 7680 * 4320 * 4 * 4);
  for (const [width, height] of [[8193, 1], [0, 1], [-1, 4], [1.5, 1], [NaN, 1], [1, Infinity], [Number.MAX_SAFE_INTEGER, 4]]) {
    const invalid = new Frame(width, height);
    assert.throws(() => c.pushFrame(invalid), /device limit/); assert.equal(invalid.closed, 0);
  }
  while (held.length) { const message = held.shift(); message.frame.close(); endpoint.reply(message); await tick(); }
  assert.equal(registry.sourceBytes, 0);
  await registry.dispose(); frames[4].close();
  assert.ok(frames.every(frame => frame.closed === 1));
});

test('canvas snapshots admit 4K at DPR 2, reject oversized inputs before capture and release accounting', async () => {
  const { main, worker, snapshots } = await modules(); initialize(worker, 'webgpu');
  class Endpoint extends EventTarget {
    postMessage(message: any) {
      void worker.textureMessage(message).then((result: any) => this.dispatchEvent(new MessageEvent('message', {
        data: { ...result, kind: 'texture-response', protocolVersion: 5, request: message.request } })));
    }
  }
  const registry = new main.BrowserTextureRegistry(new Endpoint());
  const canvas = new Frame(7680, 4320);
  const entry = await registry.registerCanvas(canvas); entry.markFrameAvailable(); await tick();
  assert.equal(snapshots.length, 1); assert.equal(entry.lastError, undefined);
  assert.equal(registry.sourceBytes, 0); assert.equal(worker.diagnostics().pendingBytes, 7680 * 4320 * 4);
  canvas.width = 8193; entry.markFrameAvailable(); await tick();
  assert.equal(snapshots.length, 1); assert.equal(entry.lastError.code, 'Size');
  await registry.dispose();
  assert.equal(snapshots[0].closed, 1); assert.equal(worker.diagnostics().pendingBytes, 0);
});

test('video capture closes rejected device-sized frames and transfers 8K display dimensions', async () => {
  const { main, worker, videoFrames } = await modules(); initialize(worker, 'webgpu');
  class Endpoint extends EventTarget {
    postMessage(message: any) {
      void worker.textureMessage(message).then((result: any) => this.dispatchEvent(new MessageEvent('message', {
        data: { ...result, kind: 'texture-response', protocolVersion: 5, request: message.request } })));
    }
  }
  class Video extends EventTarget {
    videoWidth = 8193; videoHeight = 4320; readyState = 2;
    requestVideoFrameCallback() { return 1; }
    cancelVideoFrameCallback() {}
  }
  const registry = new main.BrowserTextureRegistry(new Endpoint());
  const video = new Video(); const entry = await registry.registerVideo(video);
  assert.equal(entry.lastError.code, 'Size'); assert.equal(videoFrames[0].closed, 1);
  video.videoWidth = 7680; video.dispatchEvent(new Event('seeked')); await tick();
  assert.equal(entry.counters.acknowledged, 1);
  assert.equal(worker.diagnostics().pendingBytes, 7680 * 4320 * 4);
  assert.equal(videoFrames[1].closed, 0);
  await registry.dispose(); assert.equal(videoFrames[1].closed, 1);
});
