import assert from 'node:assert/strict';
import { readFile } from 'node:fs/promises';

const path = process.argv[2] ?? 'Doroti/src/Doroti.Host.Web/obj/Release/net10.0/Doroti.Web/wwwroot/doroti.web.policy.js';
const { selectRendererPolicy: select, initialCanvasCapacity: initial, CanvasCapacityPolicy: Capacity, applyCanvasCapacity } =
  await import('data:text/javascript;base64,' + Buffer.from(await readFile(path)).toString('base64'));
const iphone = { userAgent: 'Mozilla/5.0 (iPhone; CPU iPhone OS 26_6_1 like Mac OS X) CriOS/150 Mobile', platform: 'iPhone', maxTouchPoints: 5 };
const ipad = { userAgent: 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15) Safari/605', platform: 'MacIntel', maxTouchPoints: 5 };
const mac = { ...ipad, maxTouchPoints: 0 };
assert.equal(select('', iphone).selected, 'worker-direct-webgl');
assert.equal(select('', ipad).selected, 'worker-direct-webgl');
assert.equal(select('', mac).selected, 'worker-direct-webgpu');
assert.equal(select('', iphone).reason, 'ios-webkit-stability');
assert.equal(select('?dorotiRenderer=worker-direct-webgpu', iphone).selected, 'worker-direct-webgpu');
assert.equal(select('?dorotiRenderer=worker-direct-webgl', mac).reason, 'explicit-override');
assert.equal(select('?dorotiRenderer=unknown', iphone).requested, 'auto');
assert.equal(select('', { ...mac, userAgent: 'Android Chrome' }).memoryProfile, 'mobile');
assert.deepEqual(initial(390, 844, 3, true, 2000, 2000), { width: 1170, height: 2532 });
const capacity = new Capacity(true);
assert.deepEqual(capacity.next(1170, 2532, 1170, 2532, 0), { width: 1170, height: 2532, wakeAfter: 0 });
assert.deepEqual(capacity.next(2532, 1170, 1170, 2532, 100), { width: 2532, height: 1170, wakeAfter: 0 });
assert.deepEqual(capacity.next(1170, 2532, 2532, 1170, 200), { width: 1170, height: 2532, wakeAfter: 0 });
assert.equal(capacity.next(1170, 1400, 1170, 2532, 300).wakeAfter, 1900);
assert.equal(capacity.next(1170, 1400, 1170, 2532, 2199).height, 2532);
assert.equal(capacity.next(1170, 1400, 1170, 2532, 2200).height, 1400);
// Small address-bar changes retain a bounded amount of headroom.
assert.equal(capacity.next(1170, 1300, 1170, 1400, 5000).wakeAfter, 0);
// A changing target restarts the stability window, even after the allocation cooldown.
capacity.next(700, 800, 1170, 1400, 6000);
assert.equal(capacity.next(701, 800, 1170, 1400, 7000).width, 1170);
assert.equal(capacity.next(701, 800, 1170, 1400, 8000).width, 701);
const desktop = new Capacity(false);
assert.equal(desktop.next(800, 600, 1600, 1200, 10000).wakeAfter, 0);
assert.equal(desktop.next(1700, 600, 1600, 1200, 10001).width, 2400);
console.log('PASS backend selection and canvas capacity contracts');

let w = 1170, h = 2532;
const areas = [];
const canvas = { get width() { return w; }, get height() { return h; },
  set width(v) { w = v; areas.push(w * h); }, set height(v) { h = v; areas.push(w * h); } };
applyCanvasCapacity(canvas, 2532, 1170);
applyCanvasCapacity(canvas, 1170, 2532);
assert.ok(areas.every(area => area <= 1170 * 2532));
console.log('PASS orientation intermediate backing allocation');

const gpuPath = path.replace('doroti.web.policy.js', 'doroti.webgpu.js');
const gpu = await import('data:text/javascript;base64,' + Buffer.from(await readFile(gpuPath)).toString('base64'));
Object.assign(globalThis, { isSecureContext: true, crossOriginIsolated: true });
Object.defineProperty(globalThis, 'navigator', { value: {}, configurable: true });
await assert.rejects(() => gpu.initialize({}, () => {}), /requires a secure isolated origin and navigator.gpu/);
Object.defineProperty(globalThis, 'navigator', { value: { gpu: { requestAdapter: async () => null } }, configurable: true });
await assert.rejects(() => gpu.initialize({}, () => {}), /adapter is unavailable/);
console.log('PASS unsupported explicit WebGPU fails without backend fallback');
