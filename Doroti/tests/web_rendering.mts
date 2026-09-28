import assert from 'node:assert/strict';
import { test } from 'node:test';
import { ResizeAdmissionWindow } from '../src/Doroti.Host.Web/Web/doroti.web.admission.ts';
import { CanvasCapacityPolicy, applyCanvasCapacity, initialCanvasCapacity, selectRendererPolicy, resolveRendererPolicy } from '../src/Doroti.Host.Web/Web/doroti.web.policy.ts';
import { developmentBridge } from '../src/Doroti.Host.Web/Web/doroti.web.hot-reload.ts';

test('development status bridge only accepts an explicit local session endpoint', () => {
  const token = 'a'.repeat(48), session = 'b'.repeat(32);
  const query = (url: string) => new URLSearchParams({ dorotiDevBridge: url, dorotiDevSession: session }).toString();
  const valid = `http://127.0.0.1:54321/${token}`;
  assert.deepEqual(developmentBridge(query(valid)), { url: valid, sessionId: session });
  for (const url of [`https://example.com/${token}`, `http://user@127.0.0.1:54321/${token}`, `${valid}?extra=1`, 'http://127.0.0.1:54321/invalid'])
    assert.equal(developmentBridge(query(url)), undefined);
  assert.equal(developmentBridge(''), undefined);
});

test('resize pressure stays bounded and acknowledgment admits only latest pending metrics', () => {
  const sent: number[] = [];
  const queue = new ResizeAdmissionWindow<{ epoch: { generation: number } }>(value => sent.push(value.epoch.generation));
  for (let generation = 1; generation <= 30; generation++) queue.queue({ epoch: { generation } });
  assert.deepEqual(sent, [1, 2, 3, 4]); assert.equal(queue.pendingCount, 5);
  queue.acknowledge(99); assert.equal(queue.pendingCount, 5);
  queue.acknowledge(1); assert.deepEqual(sent, [1, 2, 3, 4, 30]);
  queue.acknowledge(1); assert.equal(queue.pendingCount, 4);
  queue.reset(); assert.equal(queue.pendingCount, 0);
  queue.acknowledge(30); assert.equal(queue.pendingCount, 0);
  queue.queue({ epoch: { generation: 31 } }); assert.equal(sent.at(-1), 31);
});
test('mobile backing does not add desktop headroom and shrinks after hysteresis', () => {
  assert.deepEqual(initialCanvasCapacity(360, 800, 3, true, 1920, 1080), { width: 1080, height: 2400 });
  const mobile = new CanvasCapacityPolicy(true);
  assert.deepEqual(mobile.next(1080, 2400, 0, 0, 0), { width: 1080, height: 2400, wakeAfter: 0 });
  assert.deepEqual(mobile.next(1080, 1200, 1080, 2400, 500), { width: 1080, height: 2400, wakeAfter: 1500 });
  assert.deepEqual(mobile.next(1080, 1200, 1080, 2400, 2000), { width: 1080, height: 1200, wakeAfter: 0 });
  let width = 2400, height = 1080, peak = width * height;
  const canvas = { get width() { return width; }, set width(v: number) { width = v; peak = Math.max(peak, width * height); }, get height() { return height; }, set height(v: number) { height = v; peak = Math.max(peak, width * height); } };
  applyCanvasCapacity(canvas, 1080, 2400); assert.equal(peak, 2592000);
});
test('auto fallback preserves explicit renderer choices', async () => {
  const platform = { userAgent: 'Android', platform: 'Linux', maxTouchPoints: 5 };
  const unavailable = { isSecureContext: true, crossOriginIsolated: true, navigator: {} };
  const auto = await resolveRendererPolicy(selectRendererPolicy('', platform), 'main', unavailable);
  assert.equal(auto.selected, 'worker-direct-webgl');
  const explicit = await resolveRendererPolicy(selectRendererPolicy('?dorotiRenderer=worker-direct-webgpu', platform), 'main', unavailable);
  assert.equal(explicit.selected, 'worker-direct-webgpu'); assert.equal(explicit.reason, 'explicit-override');
});
