import assert from 'node:assert/strict';
import { test } from 'node:test';
import { configureNavigation, openApplicationNavigation, closeApplicationNavigation,
  reportApplicationRoute, selectApplicationHistory, exitApplicationNavigation }
  from '../../packages/platforms/web/Doroti.Host.Web/Web/doroti.web.navigation.ts';

async function browser(run: (fixture: ReturnType<typeof fixture>) => Promise<void>) {
  const globals = globalThis as any;
  const previous = new Map(['window', 'location', 'history', 'sessionStorage'].map(key => [key, Object.getOwnPropertyDescriptor(globals, key)]));
  const state = fixture(globals);
  try { await run(state); }
  finally {
    closeApplicationNavigation(7);
    for (const [key, descriptor] of previous) {
      if (descriptor) Object.defineProperty(globals, key, descriptor); else delete globals[key];
    }
  }
}

function fixture(globals: any) {
  const events = new EventTarget(), received: any[] = [], traversals: number[] = [];
  const entries = [{ url: 'https://previous.example/', state: null as any },
    { url: 'https://app.example/app', state: { unrelated: 'keep' } as any }];
  let index = 1;
  const update = () => { globals.location = new URL(entries[index].url); };
  Object.assign(globals, { window: events, location: new URL(entries[index].url),
    sessionStorage: { getItem() { return null; }, setItem() {} },
    history: {
      get state() { return entries[index].state; },
      pushState(state: any, _: string, url: string | URL) {
        entries.splice(++index); entries.push({ state, url: new URL(url, globals.location).href }); update();
      },
      replaceState(state: any, _: string, url: string | URL) {
        entries[index] = { state, url: new URL(url, globals.location).href }; update();
      },
      go(delta: number) {
        traversals.push(delta);
        queueMicrotask(() => {
          const next = index + delta;
          if (next < 0 || next >= entries.length || next === index) return;
          index = next; update(); events.dispatchEvent(new Event('popstate'));
        });
      },
    },
  });
  configureNavigation((host, json) => received.push({ host, ...JSON.parse(json) }));
  openApplicationNavigation(7, null);
  return { events, received, entries, traversals, get index() { return index; },
    get url() { return globals.location.href as string; },
    get state() { return entries[index].state; },
    async go(delta: number) { globals.history.go(delta); await new Promise(resolve => setImmediate(resolve)); },
    async settle() { await new Promise(resolve => setImmediate(resolve)); },
  };
}

test('Navigator browser Back stays in the document for dialogs, repeated Back and a veto', async () => browser(async b => {
  selectApplicationHistory(7, true);
  const length = b.entries.length;
  reportApplicationRoute(7, '/page', null, false);
  reportApplicationRoute(7, '/page', null, false);
  assert.equal(b.entries.length, length, 'Navigator updates must replace its single entry.');
  for (let modal = 2; modal >= 0; modal--) {
    await b.go(-1);
    assert.equal(b.url, 'https://app.example/page');
    assert.equal(b.index, 2);
    assert.equal(b.received.length, 3 - modal);
    assert.equal(b.received.at(-1).kind, 'pop');
    assert.equal(b.entries.length, length, 'A pending/vetoed framework decision must already have a restored guard.');
  }
  exitApplicationNavigation(7);
  await b.settle();
  assert.equal(b.url, 'https://previous.example/');
  assert.deepEqual(b.entries[1].state, { unrelated: 'keep' });
  assert.equal(b.received.length, 3, 'Root exit must not deliver another Back to the framework.');
}));

test('Router mode removes the Navigator guard and queues reports until traversal completes', async () => browser(async b => {
  selectApplicationHistory(7, true);
  selectApplicationHistory(7, false);
  reportApplicationRoute(7, '/first', '{"selection":1}', true);
  reportApplicationRoute(7, '/second', '{"selection":2}', false);
  await b.settle();
  assert.equal(b.entries.length, 3);
  assert.equal(b.url, 'https://app.example/second');
  await b.go(-1);
  assert.equal(b.received.at(-1).location, '/first');
  assert.equal(b.received.at(-1).state, '{"selection":1}');
  assert.equal(b.state.unrelated, 'keep');
  await b.go(1);
  assert.equal(b.received.at(-1).location, '/second');
  assert.equal(b.received.at(-1).state, '{"selection":2}');
  assert.ok(b.received.every(event => !event.kind));
}));

test('Navigator reload adopts its existing guard and disposal detaches callbacks', async () => browser(async b => {
  selectApplicationHistory(7, true);
  const length = b.entries.length;
  closeApplicationNavigation(7);
  openApplicationNavigation(7, null);
  selectApplicationHistory(7, true);
  assert.equal(b.entries.length, length);
  await b.go(-1);
  assert.equal(b.received.length, 1);
  closeApplicationNavigation(7);
  await b.go(-1);
  assert.equal(b.received.length, 1);
}));

test('a manually edited hash reaches Navigator as pushRoute without growing its history', async () => browser(async b => {
  selectApplicationHistory(7, true);
  (globalThis as any).history.pushState(null, '', '/app#/manual');
  b.events.dispatchEvent(new Event('popstate'));
  await b.settle();
  assert.deepEqual(b.received, [{ host: 7, kind: 'push', location: '/app#/manual' }]);
  reportApplicationRoute(7, '/app#/manual', null, false);
  assert.equal(b.url, 'https://app.example/app#/manual');
  assert.equal(b.index, 2);
}));

test('SystemNavigator.pop exits Router history from the current Back position', async () => browser(async b => {
  selectApplicationHistory(7, false);
  reportApplicationRoute(7, '/first', null, false);
  reportApplicationRoute(7, '/second', null, false);
  await b.go(-1);
  exitApplicationNavigation(7);
  await b.settle();
  assert.equal(b.url, 'https://previous.example/');
}));

test('bfcache return after root exit restores Navigator Back handling', async () => browser(async b => {
  selectApplicationHistory(7, true);
  exitApplicationNavigation(7);
  await b.settle();
  await b.go(1);
  b.events.dispatchEvent(new Event('pageshow'));
  assert.equal(b.url, 'https://app.example/app');
  await b.go(-1);
  assert.equal(b.received.at(-1).kind, 'pop');
  assert.equal(b.index, 2);
}));

test('late route reports cannot replace the destination of a pending root exit', async () => browser(async b => {
  selectApplicationHistory(7, true);
  exitApplicationNavigation(7);
  reportApplicationRoute(7, '/late', null, false);
  await b.settle();
  reportApplicationRoute(7, '/later', null, false);
  assert.equal(b.url, 'https://previous.example/');
  assert.equal(b.entries[1].url, 'https://app.example/app');
}));
