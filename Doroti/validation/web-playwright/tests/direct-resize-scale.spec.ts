import { test, expect } from './helpers/fixtures.js';
import { openDoroti, captureDiagnostics } from './helpers/doroti-diagnostics.js';
import { PNG } from 'pngjs';

// Exercise the actual diagnostics page on both supported direct backends.
// DOM mutations alone do not prove pixel scale: sample intrinsic and displayed
// dimensions together on browser animation frames throughout the resize.
for (const renderer of ['worker-direct-webgpu', 'worker-direct-webgl']) {
  for (const dpr of [1, 2]) {
    test(`${renderer} diagnostics resize retains pixel scale DPR ${dpr}`, async ({ browser }, testInfo) => {
      const context = await browser.newContext({ viewport: { width: 1000, height: 720 }, deviceScaleFactor: dpr });
      const page = await context.newPage();
      const errors: string[] = [];
      page.on('pageerror', error => errors.push(String(error)));
      try {
        await openDoroti(page, `&dorotiRenderer=${renderer}&dorotiTestbedMode=diagnostics`);
        await page.bringToFront();
        const initial = await page.locator('canvas').first().evaluate((canvas: HTMLCanvasElement) => ({ width: canvas.width, height: canvas.height }));
        await page.evaluate(() => {
          const samples: { width: number; height: number; cssWidth: number; cssHeight: number }[] = [];
          const sample = () => {
            const canvas = document.querySelector('canvas')!;
            const box = canvas.getBoundingClientRect();
            samples.push({ width: canvas.width, height: canvas.height, cssWidth: box.width, cssHeight: box.height });
          };
          const observer = new ResizeObserver(sample);
          observer.observe(document.querySelector('.doroti-root')!);
          const state = { samples, active: true, stop: () => observer.disconnect() };
          Object.assign(globalThis, { __resizeScale: state });
          const tick = () => {
            sample();
            if (state.active) requestAnimationFrame(tick);
          };
          requestAnimationFrame(tick);
        });
        for (const [width, height] of [[960,680], [880,640], [760,580], [900,650], [1050,750], [1150,800], [1000,720]]) {
          await page.setViewportSize({ width, height });
          await page.waitForTimeout(45);
        }
        await expect.poll(async () => {
          const b = await captureDiagnostics(page);
          return b.presenter.frontGeneration === b.snapshot.resizeEpoch.generation && b.presenter.queueDepth === 0;
        }).toBe(true);
        const samples = await page.evaluate(() => {
          const state = (globalThis as any).__resizeScale;
          state.active = false;
          state.stop();
          return state.samples as { width: number; height: number; cssWidth: number; cssHeight: number }[];
        });
        await testInfo.attach('resize-scale', { body: JSON.stringify({ initial, samples, bundle: await captureDiagnostics(page), errors }), contentType: 'application/json' });
        await testInfo.attach('diagnostics-after-resize', { body: await page.screenshot(), contentType: 'image/png' });
        expect(samples.length).toBeGreaterThan(2);
        expect(samples.filter(s => s.width !== initial.width || s.height !== initial.height), 'viewport changes within capacity must not resize the transferred front').toEqual([]);
        expect(samples.filter(s => Math.abs(s.cssWidth * dpr - s.width) > 1 || Math.abs(s.cssHeight * dpr - s.height) > 1), 'intrinsic pixels and CSS must have the same DPR throughout resize').toEqual([]);
        expect(errors).toEqual([]);
      } finally { await context.close(); }
    });
  }
}

for (const renderer of ['worker-direct-webgpu', 'worker-direct-webgl']) {
for (const dpr of [1, 2]) {
  test(`${renderer} resize pixels stay fixed before main receives commit DPR ${dpr}`, async ({ browser }, testInfo) => {
    const context = await browser.newContext({ viewport: { width: 1000, height: 720 }, deviceScaleFactor: dpr });
    const page = await context.newPage();
    const errors: string[] = [];
    page.on('pageerror', error => errors.push(String(error)));
    try {
      await page.addInitScript(() => {
        const dispatch = EventTarget.prototype.dispatchEvent;
        const held: { target: EventTarget; event: Event }[] = [];
        const state = { hold: false, widths: [] as number[], release: () => {
          state.hold = false;
          for (const item of held.splice(0)) dispatch.call(item.target, item.event);
          state.widths.length = 0;
        } };
        Object.assign(globalThis, { __holdResizeCommit: state });
        EventTarget.prototype.dispatchEvent = function(event: Event) {
          if (state.hold && event instanceof MessageEvent && event.data?.kind === 'direct-commit') {
            held.push({ target: this, event });
            state.widths.push(Number(event.data.logicalWidth));
            return true;
          }
          return dispatch.call(this, event);
        };
      });
      await openDoroti(page, `&dorotiRenderer=${renderer}&dorotiTestbedMode=diagnostics`);
      const capacity = await page.locator('canvas').first().evaluate((canvas: HTMLCanvasElement) => ({ width: canvas.width, height: canvas.height }));
      for (const [width, height] of [[700, 720], [Math.ceil(capacity.width / dpr) + 100, Math.ceil(capacity.height / dpr) + 100], [1000, 720]]) {
        await page.evaluate(() => { (globalThis as any).__holdResizeCommit.hold = true; });
        await page.setViewportSize({ width, height });
        await page.waitForFunction(width => (globalThis as any).__holdResizeCommit.widths.includes(width), width);
        let bytes!: Buffer;
        const geometry: number[][] = [];
        // A Worker submission can precede the browser's visible update.
        // Wait for target edge pixels, retaining scale checks for EVERY
        // intervening capture (including the previously displayed frame).
        await expect.poll(async () => {
          bytes = await page.screenshot();
          const png = PNG.sync.read(bytes);
          const points: { x: number; y: number }[] = [];
          for (let y = 0; y < 100 * dpr; y++) for (let x = 0; x < 50 * dpr; x++) {
            const i = (y * png.width + x) * 4;
            if (png.data[i] === 255 && png.data[i + 1] === 23 && png.data[i + 2] === 68) points.push({ x, y });
          }
          geometry.push([points.length, Math.min(...points.map(p => p.x)), Math.max(...points.map(p => p.x)),
            Math.min(...points.map(p => p.y)), Math.max(...points.map(p => p.y))]);
          return [[width - 2, 65], [width - 10, height - 2]].every(([x, y]) => {
            const i = (y * dpr * png.width + x * dpr) * 4;
            return png.data[i] === 255 && png.data[i + 1] === 23 && png.data[i + 2] === 68 && png.data[i + 3] === 255;
          });
        }, { message: 'visible right/bottom markers follow the target while main commit remains held' }).toBe(true);
        await testInfo.attach(`pixels-before-commit-${width}`, { body: bytes, contentType: 'image/png' });
        await testInfo.attach(`pixel-geometry-${width}`, { body: JSON.stringify(geometry), contentType: 'application/json' });
        // Diagnostics' red L marker is 22x3 + 3x15 (3x3 overlap),
        // below the 56px app bar, at (2,59).
        // This checks displayed pixels during the cross-thread handoff,
        // including allocation growth, rather than trusting CSS/receipt data.
        expect(geometry).toEqual(geometry.map(() => [102 * dpr * dpr, 2 * dpr, 24 * dpr - 1, 59 * dpr, 74 * dpr - 1]));
        await page.evaluate(() => (globalThis as any).__holdResizeCommit.release());
      }
      expect(errors).toEqual([]);
    } finally { await context.close(); }
  });
}
}
