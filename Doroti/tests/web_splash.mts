import assert from 'node:assert/strict';
import { test } from 'node:test';
import { attachDorotiSplash } from '../../packages/platforms/web/Doroti.Host.Web/Web/doroti.web.splash.ts';

class Element extends EventTarget {
  id = '';
  className = '';
  dataset: Record<string, string> = {};
  attributes = new Map<string, string>();
  textContent = '';
  hidden = false;
  removed = false;
  parentElement?: Element;
  children: Element[] = [];
  constructor(readonly tagName: string) { super(); }
  append(...children: Element[]) {
    for (const child of children) { child.parentElement = this; this.children.push(child); }
  }
  setAttribute(name: string, value: string) { this.attributes.set(name, value); }
  removeAttribute(name: string) { this.attributes.delete(name); }
  hasAttribute(name: string) { return this.attributes.has(name); }
  remove() {
    this.removed = true;
    if (this.parentElement) this.parentElement.children = this.parentElement.children.filter(child => child !== this);
  }
}

function withDom(run: (fixture: {
  body: Element; app: Element; surface: Element; events: EventTarget;
  listeners: Set<string>; reloads(): number;
}) => void, reducedMotion = true) {
  const events = new EventTarget(), body = new Element('body'), app = new Element('div');
  const surface = new Element('canvas');
  app.id = 'app';
  surface.id = 'doroti-surface';
  app.append(surface);
  body.append(app);
  const listeners = new Set<string>();
  let reloads = 0;
  function find(element: Element, id: string): Element | undefined {
    if (element.id === id) return element;
    for (const child of element.children) { const result = find(child, id); if (result) return result; }
  }
  const globals = {
    document: { body, getElementById: (id: string) => find(body, id), createElement: (tag: string) => new Element(tag) },
    addEventListener(type: string, listener: EventListener, options?: AddEventListenerOptions) {
      listeners.add(type); events.addEventListener(type, listener, options);
    },
    removeEventListener(type: string, listener: EventListener) {
      listeners.delete(type); events.removeEventListener(type, listener);
    },
    matchMedia: () => ({ matches: reducedMotion }),
    location: { reload() { reloads++; } },
  };
  const previous = new Map(Object.keys(globals).map(key => [key, Object.getOwnPropertyDescriptor(globalThis, key)]));
  try {
    Object.assign(globalThis, globals);
    run({ body, app, surface, events, listeners, reloads: () => reloads });
  } finally {
    for (const [key, descriptor] of previous) {
      if (descriptor) Object.defineProperty(globalThis, key, descriptor);
      else Reflect.deleteProperty(globalThis, key);
    }
  }
}

test('enabled by default: creates the loading screen outside the engine container', () => {
  withDom(({ body, app }) => {
    const controller = attachDorotiSplash();
    const splash = body.children[1];
    assert.equal(splash.id, 'doroti-splash');
    assert.equal(splash.parentElement, body);
    assert.equal(splash.dataset.dorotiSplashState, 'loading');
    assert.equal(app.attributes.get('aria-busy'), 'true');
    assert.deepEqual(splash.children.map(child => child.tagName), ['img', 'p', 'div', 'p', 'button']);
    assert.equal(splash.children[1].textContent, 'Doroti');
    assert.equal(splash.children[3].attributes.get('role'), 'status');
    assert.equal(splash.children[4].hidden, true);
    // Engine initialization owns and replaces #app's children.
    app.children = [];
    assert.equal(body.children[1], splash);
    controller.fail();
  });
});

test('disabled: creates no DOM, busy state or splash listeners', () => {
  withDom(({ body, app, events, listeners }) => {
    const controller = attachDorotiSplash(false);
    controller.complete();
    controller.fail();
    events.dispatchEvent(new Event('doroti-first-frame'));
    assert.deepEqual(body.children, [app]);
    assert.equal(app.hasAttribute('aria-busy'), false);
    assert.equal(listeners.size, 0);
  });
});

test('readiness waits for the first frame and reduced motion removes the overlay immediately', () => {
  withDom(({ body, app, events, listeners, reloads }) => {
    const controller = attachDorotiSplash(true);
    const splash = body.children[1], retry = splash.children[4];
    controller.complete();
    assert.equal(splash.removed, false);
    assert.equal(app.attributes.get('aria-busy'), 'true');
    events.dispatchEvent(new Event('doroti-first-frame'));
    assert.equal(splash.removed, true);
    assert.equal(splash.dataset.dorotiSplashState, 'ready');
    assert.equal(app.hasAttribute('aria-busy'), false);
    assert.equal(listeners.size, 0);
    retry.dispatchEvent(new Event('click'));
    assert.equal(reloads(), 0, 'retry handler is removed with the overlay');
    controller.fail();
    assert.equal(splash.dataset.dorotiSplashState, 'ready');
  });
});

test('a frame committed before readiness is recognized without waiting for another event', () => {
  withDom(({ body, surface, listeners }) => {
    const controller = attachDorotiSplash();
    const splash = body.children[1];
    surface.setAttribute('data-doroti-front-logical-width', '800');
    controller.complete();
    assert.equal(splash.removed, true);
    assert.equal(listeners.size, 0);
  });
});

test('normal motion fades on the first frame and removes on opacity transition completion', () => {
  withDom(({ body, events }) => {
    const controller = attachDorotiSplash();
    const splash = body.children[1];
    controller.complete();
    events.dispatchEvent(new Event('doroti-first-frame'));
    assert.equal(splash.dataset.dorotiSplashState, 'ready');
    assert.equal(splash.removed, false);
    const event = new Event('transitionend');
    Object.assign(event, { propertyName: 'opacity' });
    splash.dispatchEvent(event);
    assert.equal(splash.removed, true);
  }, false);
});

test('terminal worker failure after readiness exposes retry and cannot be dismissed by a late frame', () => {
  withDom(({ body, app, events, listeners, reloads }) => {
    const controller = attachDorotiSplash();
    const splash = body.children[1], status = splash.children[3], retry = splash.children[4];
    controller.complete();
    assert.equal(splash.removed, false);
    const event = new Event('doroti-runtime-state');
    Object.assign(event, { detail: { state: 'failed' } });
    events.dispatchEvent(event);
    assert.equal(splash.dataset.dorotiSplashState, 'failed');
    assert.equal(status.attributes.get('role'), 'alert');
    assert.equal(retry.hidden, false);
    assert.equal(app.hasAttribute('aria-busy'), false);
    assert.equal(listeners.size, 0);
    retry.dispatchEvent(new Event('click'));
    assert.equal(reloads(), 1);
    events.dispatchEvent(new Event('doroti-first-frame'));
    controller.complete();
    assert.equal(splash.removed, false);
    assert.equal(splash.dataset.dorotiSplashState, 'failed');
  });
});
