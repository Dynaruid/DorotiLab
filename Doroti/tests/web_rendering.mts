import assert from 'node:assert/strict';
import { test } from 'node:test';
import { ResizeAdmissionWindow } from '../src/Doroti.Host.Web/Web/doroti.web.admission.ts';
import { CanvasCapacityPolicy, applyCanvasCapacity, initialCanvasCapacity, selectRendererPolicy, resolveRendererPolicy } from '../src/Doroti.Host.Web/Web/doroti.web.policy.ts';
import { developmentBridge } from '../src/Doroti.Host.Web/Web/doroti.web.hot-reload.ts';
import { configureNavigation, openApplicationNavigation, reportApplicationRoute,
  saveApplicationRestoration, closeApplicationNavigation } from '../src/Doroti.Host.Web/Web/doroti.web.navigation.ts';

test('browser navigation bridge preserves history state, isolates storage failure and detaches callbacks', () => {
  const globals = globalThis as any;
  const previous = new Map(['window', 'location', 'history', 'sessionStorage'].map(key => [key, Object.getOwnPropertyDescriptor(globals, key)]));
  const events = new EventTarget();
  const storage = new Map<string, string>();
  const entries: { state: any; url: string }[] = [];
  let state: any = { unrelated: 'keep' };
  try {
    Object.assign(globals, { window: events, location: new URL('https://app.example/app?mode=test'),
      history: { get state() { return state; }, pushState(next: any, _: string, url: URL) { state = next; entries.push({ state, url: String(url) }); },
        replaceState(next: any, _: string, url: URL) { state = next; entries[entries.length - 1] = { state, url: String(url) }; } },
      sessionStorage: { getItem(key: string) { return storage.get(key) ?? null; }, setItem(key: string, value: string) { storage.set(key, value); } } });
    const received: any[] = [];
    configureNavigation((id, json) => received.push({ id, ...JSON.parse(json) }));
    assert.equal(JSON.parse(openApplicationNavigation(7, 'sample')!).location, '/app?mode=test');
    reportApplicationRoute(7, '/app#/first', '{"text":"한글"}', false);
    reportApplicationRoute(7, '/app#/second', '{"text":"second"}', false);
    assert.equal(entries.length, 2);
    assert.throws(() => reportApplicationRoute(7, 'https://external.example/', null, false), /same-origin/);
    globals.location = new URL(entries[0].url); state = entries[0].state;
    events.dispatchEvent(new Event('popstate'));
    assert.equal(received[0].location, '/app#/first');
    assert.equal(received[0].state, '{"text":"한글"}');
    assert.equal(saveApplicationRestoration(7, '{"version":1,"cleanShutdown":false}'), null);
    events.dispatchEvent(new Event('pagehide'));
    assert.equal(JSON.parse(storage.values().next().value!).cleanShutdown, true);
    events.dispatchEvent(new Event('pageshow'));
    assert.equal(JSON.parse(storage.values().next().value!).cleanShutdown, false);
    for (const invalid of [42, { text: 'foreign' }, '{']) {
      state = { doroti: { state: invalid } };
      events.dispatchEvent(new Event('popstate'));
      assert.equal(received.at(-1).state, null);
      assert.equal(received.at(-1).location, '/app#/first');
    }
    globals.sessionStorage.setItem = () => { throw new Error('quota denied'); };
    assert.match(saveApplicationRestoration(7, '{}')!, /quota denied/);
    closeApplicationNavigation(7);
    events.dispatchEvent(new Event('popstate'));
    assert.equal(received.length, 4);
    globals.sessionStorage.setItem = (key: string, value: string) => storage.set(key, value);
    events.dispatchEvent(new Event('pagehide'));
    assert.equal(JSON.parse(storage.values().next().value!).cleanShutdown, false);
    assert.throws(() => reportApplicationRoute(7, '/', null, false), /closed/);
    state = { doroti: { state: { text: 'foreign' } } };
    assert.equal(JSON.parse(openApplicationNavigation(7, 'sample')!).state, null);
  } finally {
    closeApplicationNavigation(7);
    for (const [key, descriptor] of previous) {
      if (descriptor) Object.defineProperty(globals, key, descriptor); else delete globals[key];
    }
  }
});

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
