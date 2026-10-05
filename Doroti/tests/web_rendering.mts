import assert from 'node:assert/strict';
import { test } from 'node:test';
import { ResizeAdmissionWindow } from '../../packages/platforms/web/Doroti.Host.Web/Web/doroti.web.admission.ts';
import { CanvasCapacityPolicy, applyCanvasCapacity, initialCanvasCapacity, selectRendererPolicy, resolveRendererPolicy } from '../../packages/platforms/web/Doroti.Host.Web/Web/doroti.web.policy.ts';
import { developmentBridge } from '../../packages/platforms/web/Doroti.Host.Web/Web/doroti.web.hot-reload.ts';
import { openFileOwner, retainBrowserFiles, readBrowserFile, releaseBrowserFile, closeFileOwner } from '../../packages/platforms/web/Doroti.Host.Web/Web/doroti.web.files.ts';
import { configureNavigation, openApplicationNavigation, reportApplicationRoute,
  saveApplicationRestoration, closeApplicationNavigation } from '../../packages/platforms/web/Doroti.Host.Web/Web/doroti.web.navigation.ts';
import { BrowserWebView, WebViewFailure } from '../../packages/platforms/web/Doroti.Host.Web/Web/doroti.web.webview.ts';

test('WebView controller empty allowlist rejects before src or document state mutation', async () => {
  for (const allowed of [null, [], ['http://127.0.0.1:12345']]) {
    const changes: unknown[] = [], requests: string[] = [];
    const element = Object.assign(new EventTarget(), { style: {}, sandbox: { add() {} }, contentWindow: null,
      title: '', referrerPolicy: '', srcdoc: '', removeAttribute() {}, remove() {}, ownerDocument: { defaultView: new EventTarget() } });
    Object.defineProperty(element, 'src', { set(value) { requests.push(value); } });
    const view = new BrowserWebView({ owner: 1, id: 1, generation: 1 } as any, { Profile: 2, AllowedOrigins: allowed as any },
      event => changes.push(event), { createElement: () => element, defaultView: element.ownerDocument.defaultView } as any);
    const initial = await view.execute({ Operation: 1 });
    for (const url of ['invalid', 'file:///tmp/test', 'http://127.0.0.1:12346/a', 'https://127.0.0.1:12345/a', 'http://127.0.0.1:12345/a']) {
      const permitted = url.startsWith('http') && (allowed === null || allowed.includes(new URL(url).origin));
      const before = await view.execute({ Operation: 1 }), count = changes.length, requestCount = requests.length;
      if (permitted) await view.execute({ Operation: 2, Text: url });
      else {
        await assert.rejects(view.execute({ Operation: 2, Text: url }), (error: any) => error instanceof WebViewFailure && error.code === 3);
        assert.deepEqual(await view.execute({ Operation: 1 }), before);
        assert.equal(changes.length, count); assert.equal(requests.length, requestCount);
      }
    }
    if (allowed?.length === 0) assert.deepEqual(await view.execute({ Operation: 1 }), initial);
    view.dispose();
  }
});

test('desktop canvas grows only the deficient axis and bounds DPR/device/byte headroom', () => {
  const policy = new CanvasCapacityPolicy(false);
  let width = 1000;
  for (const requested of [1001, 1501, 2251]) {
    const next = policy.next(requested, 1000, width, 1000, 0);
    assert.equal(next.height, 1000); assert.ok(next.width >= requested); width = next.width;
  }
  assert.equal(policy.next(1000, 1001, 1000, 1000, 0).width, 1000);
  assert.deepEqual(initialCanvasCapacity(4000, 2000, 2, false, 0, 0, { dimension: 8192, bytes: 128 * 1024 * 1024 }), { width: 8000, height: 4000 });
  assert.throws(() => initialCanvasCapacity(5000, 1, 2, true), /dimension/);
  assert.throws(() => policy.next(10000, 10000, 1, 1, 0), /admission/);
});

test('browser file grants isolate owners, bound reads and revoke pending reads', async () => {
  const descriptor = Object.getOwnPropertyDescriptor(globalThis, 'document');
  Object.defineProperty(globalThis, 'document', { configurable: true, value: {} });
  try {
    openFileOwner(801); openFileOwner(802);
    const [file] = retainBrowserFiles(801, [new File([new Uint8Array(70000).fill(42)], '한글.bin')]);
    assert.equal(file.length, 70000);
    await assert.rejects(readBrowserFile(802, file.token, 0, 3), /unknown or revoked/);
    assert.equal((await readBrowserFile(801, file.token, 65535, 65536)).length, 4465);
    await assert.rejects(readBrowserFile(801, file.token, -1, 3), /Invalid bounded/);
    await assert.rejects(readBrowserFile(801, file.token, 0, 65537), /Invalid bounded/);
    const pending = readBrowserFile(801, file.token, 0, 65536);
    releaseBrowserFile(801, file.token);
    await assert.rejects(pending, /revoked/);
    const [next] = retainBrowserFiles(801, [new File(['bye'], 'next.txt')]);
    closeFileOwner(801);
    await assert.rejects(readBrowserFile(801, next.token, 0, 3), /revoked/);
  } finally {
    closeFileOwner(801); closeFileOwner(802);
    if (descriptor) Object.defineProperty(globalThis, 'document', descriptor);
    else delete (globalThis as any).document;
  }
});

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
