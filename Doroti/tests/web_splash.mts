import assert from 'node:assert/strict';
import { test } from 'node:test';
import { attachDorotiSplash } from '../../packages/platforms/web/Doroti.Host.Web/Web/doroti.web.splash.ts';

class Element extends EventTarget {
  dataset: Record<string, string> = {};
  attributes = new Map<string, string>();
  textContent = '';
  hidden = true;
  removed = false;
  children = new Map<string, Element>();
  querySelector(selector: string) { return this.children.get(selector); }
  setAttribute(name: string, value: string) { this.attributes.set(name, value); }
  removeAttribute(name: string) { this.attributes.delete(name); }
  hasAttribute(name: string) { return this.attributes.has(name); }
  remove() { this.removed = true; }
}

test('terminal worker failure after readiness exposes retry and cannot be dismissed by a late frame', () => {
  const events = new EventTarget(), splash = new Element(), app = new Element();
  const status = new Element(), retry = new Element(), surface = new Element();
  app.setAttribute('aria-busy', 'true');
  splash.children.set('[data-doroti-splash-status]', status);
  splash.children.set('[data-doroti-splash-retry]', retry);
  const elements = new Map([['doroti-splash', splash], ['app', app], ['doroti-surface', surface]]);
  const globals = {
    document: { getElementById: (id: string) => elements.get(id) },
    addEventListener: events.addEventListener.bind(events),
    removeEventListener: events.removeEventListener.bind(events),
    matchMedia: () => ({ matches: true }),
  };
  const previous = new Map(Object.keys(globals).map(key => [key, Object.getOwnPropertyDescriptor(globalThis, key)]));
  try {
    Object.assign(globalThis, globals);
    const controller = attachDorotiSplash();
    controller.complete();
    assert.equal(splash.removed, false);
    const event = new Event('doroti-runtime-state');
    Object.assign(event, { detail: { state: 'failed' } });
    events.dispatchEvent(event);
    assert.equal(splash.dataset.dorotiSplashState, 'failed');
    assert.equal(status.attributes.get('role'), 'alert');
    assert.equal(retry.hidden, false);
    assert.equal(app.hasAttribute('aria-busy'), false);
    events.dispatchEvent(new Event('doroti-first-frame'));
    controller.complete();
    assert.equal(splash.removed, false);
    assert.equal(splash.dataset.dorotiSplashState, 'failed');
  } finally {
    for (const [key, descriptor] of previous) {
      if (descriptor) Object.defineProperty(globalThis, key, descriptor);
      else Reflect.deleteProperty(globalThis, key);
    }
  }
});
