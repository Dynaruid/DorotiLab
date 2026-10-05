import assert from 'node:assert/strict';
import { test } from 'node:test';
import { WorkerRequestMailbox } from '../../packages/platforms/web/Doroti.Host.Web/Web/doroti.web.requests.ts';
import { PlatformFrameStager } from '../../packages/platforms/web/Doroti.Host.Web/Web/doroti.web.platform-frames.ts';
import type { CompositionPacket, RasterPacket } from '../../packages/platforms/web/Doroti.Host.Web/Web/doroti.web.composition.ts';
import { WorkerStartupLifetime, cleanupAll } from '../../packages/platforms/web/Doroti.Host.Web/Web/doroti.web.lifetime.ts';

test('startup cancellation terminates awaiting caller and retires every late resource exactly once', async () => {
  for (const stage of ['runtime-create','exports','gpu-initialize','surface-initialize','application-start','texture-initialize']) {
    const lifetime = new WorkerStartupLifetime();
    const pending = deferred<number>(); let disposed = 0;
    const waiting = lifetime.wait(pending.promise, () => { disposed++; });
    const rejected = assert.rejects(waiting, /closed during startup/);
    lifetime.close(); lifetime.close(); await rejected;
    pending.resolve(1); await tick(); assert.equal(disposed, 1, stage);
    assert.throws(() => lifetime.check(), /closed/);
  }
  let second = 0;
  const errors = await cleanupAll([() => { throw new Error('first'); }, () => { second++; }, () => { throw new Error('last'); }]);
  assert.equal(second, 1); assert.equal(errors.length, 2);
});

const errors = { full: () => new Error('full'), closed: () => new Error('closed'), timeout: () => new Error('timeout') };
const tick = () => new Promise<void>(resolve => setImmediate(resolve));
function deferred<T>() {
  let resolve!: (value: T) => void, reject!: (reason: unknown) => void;
  const promise = new Promise<T>((yes, no) => { resolve = yes; reject = no; });
  return { promise, resolve, reject };
}

test('mailbox bounds admission, pairs out-of-order ACKs, and ignores duplicate/foreign replies', async () => {
  const mailbox = new WorkerRequestMailbox<string>(2, 30000, errors);
  const sent: number[] = [];
  const first = mailbox.request(id => sent.push(id));
  const second = mailbox.request(id => sent.push(id));
  await assert.rejects(mailbox.request(() => assert.fail('full mailbox must not send')), /full/);
  assert.equal(mailbox.resolve(99, 'foreign'), false);
  assert.equal(mailbox.resolve(sent[1], 'second'), true);
  assert.equal(mailbox.resolve(sent[1], 'duplicate'), false);
  mailbox.resolve(sent[0], 'first');
  assert.deepEqual(await Promise.all([first, second]), ['first', 'second']);
  await mailbox.drain();
  assert.equal(mailbox.pendingCount, 0);
});

test('mailbox releases capacity after a synchronous transfer failure or synchronous response', async () => {
  const mailbox = new WorkerRequestMailbox<number>(1, 30000, errors);
  await assert.rejects(mailbox.request(() => { throw new DOMException('detached', 'DataCloneError'); }), { name: 'DataCloneError' });
  assert.equal(mailbox.pendingCount, 0);
  assert.equal(await mailbox.request(id => mailbox.resolve(id, 42)), 42);
  await mailbox.drain();
});

test('mailbox timeout keeps its cause while owner shutdown cancels other callers', async () => {
  let mailbox!: WorkerRequestMailbox<string>;
  mailbox = new WorkerRequestMailbox<string>(2, 5, errors, () => mailbox.close());
  const timeout = assert.rejects(mailbox.request(() => {}), /timeout/);
  const closed = assert.rejects(mailbox.request(() => {}, null), /closed/);
  await Promise.all([timeout, closed]);
  await mailbox.drain();
  assert.equal(mailbox.pendingCount, 0);
  await assert.rejects(mailbox.request(() => assert.fail('closed mailbox must not send')), /closed/);
});

test('mailbox chooser requests have no deadline and are canceled by idempotent owner close', async () => {
  const mailbox = new WorkerRequestMailbox<string>(1, 1, errors);
  const request = mailbox.request(() => {}, null);
  await new Promise(resolve => setTimeout(resolve, 8));
  assert.equal(mailbox.pendingCount, 1);
  mailbox.close(); mailbox.close();
  await assert.rejects(request, /closed/);
  await mailbox.drain();
});

test('mailbox replacement rejects old requests and never reuses their correlation IDs', async () => {
  const mailbox = new WorkerRequestMailbox<string>(1, 30000, errors);
  let oldId = 0, newId = 0;
  const old = mailbox.request(id => { oldId = id; });
  mailbox.rejectAll(new Error('runtime lost'));
  await assert.rejects(old, /runtime lost/);
  const next = mailbox.request(id => { newId = id; });
  assert.ok(newId > oldId);
  assert.equal(mailbox.resolve(oldId, 'stale'), false);
  mailbox.resolve(newId, 'new session');
  assert.equal(await next, 'new session');
  await mailbox.drain();
});

class Bitmap {
  closed = 0;
  close() { this.closed++; }
  get value() { return this as unknown as ImageBitmap; }
}
function raster(order: number): RasterPacket {
  return { order, bounds: { left: 0, top: 0, width: 4, height: 4 }, width: 4, height: 4, pixels: new Uint8Array() };
}
function batch(frame = 1): CompositionPacket['batch'] { return { frame } as CompositionPacket['batch']; }

test('sealed platform frames wait only for their own captures and transfer the matching bitmap', async () => {
  const frames = new PlatformFrameStager();
  const first = new Bitmap(), next = new Bitmap(), capture = deferred<ImageBitmap>();
  frames.stageBitmap(raster(1), Promise.resolve(first.value));
  frames.stageFrame(batch(1));
  frames.stageBitmap(raster(2), capture.promise);
  assert.equal(await frames.commit(async (packet, transfer) => {
    assert.equal(packet.batch.frame, 1);
    assert.deepEqual(transfer, [first.value]);
    return true;
  }), true);
  assert.equal(first.closed, 1);
  assert.equal(next.closed, 0);
  frames.stageFrame(batch(2));
  capture.resolve(next.value);
  assert.equal(await frames.commit(async packet => packet.batch.frame === 2), true);
  assert.equal(next.closed, 1);
  await frames.drainCaptures();
});

test('platform discard closes ready and late captures during an in-flight commit exactly once', async () => {
  const frames = new PlatformFrameStager();
  const ready = new Bitmap(), late = new Bitmap(), capture = deferred<ImageBitmap>();
  frames.stageBitmap(raster(1), Promise.resolve(ready.value));
  frames.stageBitmap(raster(2), capture.promise);
  frames.stageFrame(batch());
  const commit = frames.commit(async () => { assert.fail('discarded frame must not send'); });
  await tick();
  frames.discard(); frames.discard();
  assert.equal(ready.closed, 1);
  capture.resolve(late.value);
  assert.equal(await commit, false);
  await frames.drainCaptures();
  assert.equal(late.closed, 1);
});

test('platform capture failure closes a sibling that resolves after the commit rejected', async () => {
  const frames = new PlatformFrameStager();
  const late = new Bitmap(), capture = deferred<ImageBitmap>();
  frames.stageBitmap(raster(1), Promise.reject(new Error('capture failed')));
  frames.stageBitmap(raster(2), capture.promise);
  frames.stageFrame(batch());
  await assert.rejects(frames.commit(async () => { assert.fail('failed capture must not send'); }), /capture failed/);
  capture.resolve(late.value);
  await frames.drainCaptures();
  assert.equal(late.closed, 1);
});

test('platform failed transfer closes captures and permits the next frame', async () => {
  const frames = new PlatformFrameStager(), bitmap = new Bitmap();
  frames.stageBitmap(raster(1), Promise.resolve(bitmap.value));
  frames.stageFrame(batch());
  await assert.rejects(frames.commit(async () => { throw new Error('transfer failed'); }), /transfer failed/);
  assert.equal(bitmap.closed, 1);
  frames.stageRaster(raster(2)); frames.stageFrame(batch(2));
  assert.equal(await frames.commit(async () => true), true);
});

test('platform commits are bounded to one and keep the next staged frame intact', async () => {
  const frames = new PlatformFrameStager(), receipt = deferred<boolean>();
  frames.stageRaster(raster(1)); frames.stageFrame(batch(1));
  const first = frames.commit(() => receipt.promise);
  frames.stageRaster(raster(2)); frames.stageFrame(batch(2));
  await assert.rejects(frames.commit(async () => true), /already in flight/);
  receipt.resolve(true);
  assert.equal(await first, true);
  assert.equal(await frames.commit(async packet => packet.batch.frame === 2), true);
});

test('platform discard invalidates a receipt already awaiting DOM acceptance', async () => {
  const frames = new PlatformFrameStager(), receipt = deferred<boolean>(), bitmap = new Bitmap();
  frames.stageBitmap(raster(1), Promise.resolve(bitmap.value)); frames.stageFrame(batch());
  const commit = frames.commit(() => receipt.promise);
  await tick();
  frames.discard();
  receipt.resolve(true);
  assert.equal(await commit, false);
  assert.equal(bitmap.closed, 1);
});

test('platform staging rejects overwrite and discard drains uncommitted captures', async () => {
  const frames = new PlatformFrameStager(), bitmap = new Bitmap();
  frames.stageBitmap(raster(1), Promise.resolve(bitmap.value)); frames.stageFrame(batch());
  assert.throws(() => frames.stageFrame(batch(2)), /already staged/);
  frames.discard();
  await frames.drainCaptures();
  assert.equal(bitmap.closed, 1);
  assert.equal(await frames.commit(async () => { assert.fail('no frame must not send'); }), true);
});
