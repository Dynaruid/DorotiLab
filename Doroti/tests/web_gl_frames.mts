import assert from 'node:assert/strict';
import { test } from 'node:test';
import { WebGlFrameQueue } from '../../packages/platforms/web/Doroti.Host.Web/Web/doroti.web.gl-frames.ts';

class Gpu {
  SYNC_GPU_COMMANDS_COMPLETE = 1;
  TIMEOUT_EXPIRED = 2;
  ALREADY_SIGNALED = 3;
  CONDITION_SATISFIED = 4;
  WAIT_FAILED = 5;
  status = this.TIMEOUT_EXPIRED;
  lost = false;
  allocate = true;
  fences: WebGLSync[] = [];
  deleted: WebGLSync[] = [];
  flushed = 0;
  isContextLost() { return this.lost; }
  fenceSync() {
    if (!this.allocate) return null;
    const fence = {} as WebGLSync;
    this.fences.push(fence);
    return fence;
  }
  clientWaitSync(_: WebGLSync, flags: number, timeout: number) {
    assert.equal(flags, 0); assert.equal(timeout, 0);
    return this.status;
  }
  deleteSync(fence: WebGLSync) { this.deleted.push(fence); }
  flush() { this.flushed++; }
  get gl() { return this as unknown as WebGL2RenderingContext; }
}
const tick = () => new Promise(resolve => setTimeout(resolve, 8));

test('slow GPU bounds WebGL to two frames while the owner keeps accepting newer input', async () => {
  const gpu = new Gpu(), frames = new WebGlFrameQueue(gpu.gl);
  for (let i = 0; i < 2; i++) { await frames.waitForCapacity(); frames.submitted(); }
  let ready = false, latestInput = 0;
  const waiting = frames.waitForCapacity().then(() => { ready = true; });
  for (let i = 0; i < 3; i++) { latestInput++; await tick(); }
  assert.equal(ready, false); assert.equal(latestInput, 3);
  assert.equal(frames.diagnostics().inFlight, 2);
  gpu.status = gpu.CONDITION_SATISFIED;
  await waiting;
  assert.equal(gpu.deleted.length, 2);
  frames.submitted();
  await frames.drainForShutdown();
  assert.equal(gpu.flushed, 3);
  assert.equal(frames.diagnostics().peakInFlight, 2);
  assert.equal(frames.diagnostics().completedSubmissions, 3);
  assert.deepEqual(gpu.deleted, gpu.fences);
});

test('context loss cancels a capacity wait and releases every fence exactly once', async () => {
  const gpu = new Gpu(), frames = new WebGlFrameQueue(gpu.gl);
  frames.submitted(); frames.submitted();
  const waiting = frames.waitForCapacity();
  gpu.lost = true;
  frames.contextLost(); frames.contextLost();
  await waiting; await frames.drainForShutdown();
  frames.submitted();
  assert.equal(gpu.fences.length, 2);
  assert.deepEqual(gpu.deleted, gpu.fences);
  assert.equal(frames.diagnostics().completedSubmissions, 0);
});

test('shutdown drains GPU draws before releasing the owner and wakes a capacity waiter', async () => {
  const gpu = new Gpu(), frames = new WebGlFrameQueue(gpu.gl);
  frames.submitted(); frames.submitted();
  const waiting = frames.waitForCapacity();
  let drained = false;
  const drain = frames.drainForShutdown().then(() => { drained = true; });
  await waiting; await tick();
  assert.equal(drained, false); assert.equal(gpu.deleted.length, 0);
  frames.submitted(); assert.equal(gpu.fences.length, 2);
  gpu.status = gpu.ALREADY_SIGNALED;
  await drain;
  assert.deepEqual(gpu.deleted, gpu.fences);
});

test('failed fence retains ownership and rejects future work until confirmed context loss', async () => {
  const gpu = new Gpu(), frames = new WebGlFrameQueue(gpu.gl);
  frames.submitted(); gpu.status = gpu.WAIT_FAILED;
  await assert.rejects(frames.waitForCapacity(), /completion failed/);
  assert.throws(() => frames.submitted(), /completion failed/);
  await assert.rejects(frames.drainForShutdown(), /completion failed/);
  assert.equal(gpu.deleted.length, 0);
  gpu.lost = true; frames.contextLost();
  await frames.drainForShutdown();
  assert.deepEqual(gpu.deleted, gpu.fences);
});

test('fence allocation failure is fatal instead of silently admitting unbounded work', async () => {
  const gpu = new Gpu(), frames = new WebGlFrameQueue(gpu.gl);
  gpu.allocate = false;
  assert.throws(() => frames.submitted(), /allocation failed/);
  await assert.rejects(frames.waitForCapacity(), /allocation failed/);
  await assert.rejects(frames.drainForShutdown(), /allocation failed/);
  assert.equal(gpu.fences.length, 0);
  assert.equal(frames.diagnostics().inFlight, 0);
});

test('a completion timeout retains GPU ownership and does not reopen admission', async context => {
  let now = 0;
  context.mock.method(performance, 'now', () => now);
  const gpu = new Gpu(), frames = new WebGlFrameQueue(gpu.gl);
  frames.submitted(); now = 30001;
  await assert.rejects(frames.waitForCapacity(), /timed out/);
  await assert.rejects(frames.drainForShutdown(), /timed out/);
  assert.equal(gpu.deleted.length, 0);
  gpu.lost = true;
  await frames.drainForShutdown();
  assert.deepEqual(gpu.deleted, gpu.fences);
});
