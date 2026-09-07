import { test, expect } from './helpers/fixtures.js';
import { openDoroti, captureDiagnostics } from './helpers/doroti-diagnostics.js';
import { measureResizeFollowing } from './helpers/resize-following.js';

for (const dpr of [1, 2]) {
  test(`direct capacity keeps a fixed pixel scale during resize DPR ${dpr}`, async ({ browser }, testInfo) => {
    const context = await browser.newContext({ viewport: { width: 1000, height: 720 }, deviceScaleFactor: dpr });
    const page = await context.newPage();
    const errors: string[] = [];
    page.on('pageerror', e => errors.push(String(e)));
    try {
      await openDoroti(page, '&dorotiRenderer=worker-direct-webgl&dorotiTestbedMode=sample');
      await page.waitForTimeout(3000);
      await page.evaluate(() => {
        const canvas = document.querySelector('canvas')!;
        const samples: unknown[] = [];
        const observe = () => {
          const ratio = Number(canvas.dataset.dorotiCapacityDevicePixelRatio);
          if (samples.length < 512) samples.push({ time: performance.now(), ratio,
            width: parseFloat(canvas.style.width), height: parseFloat(canvas.style.height),
            capacityWidth: Number(canvas.dataset.dorotiCapacityWidth),
            capacityHeight: Number(canvas.dataset.dorotiCapacityHeight) });
        };
        new MutationObserver(observe).observe(canvas, { attributes: true, attributeFilter: ['style'] });
        new ResizeObserver(observe).observe(canvas.closest('.doroti-root')!);
        Object.assign(globalThis, { __directGeometrySamples: samples });
        observe();
      });
      const start = await page.evaluate(() => performance.now());
      // Eight changes cover both directions, the navigation breakpoint and
      // growth beyond initial capacity. These are CDP steps, not native drag.
      for (const [width, height] of [[900,680],[760,620],[640,580],[820,640],[1100,760],[2000,1200],[1050,740],[1000,720]]) {
        await page.setViewportSize({ width, height });
        await page.waitForTimeout(45);
      }
      const end = await page.evaluate(() => performance.now());
      await expect.poll(async () => {
        const b = await captureDiagnostics(page);
        return b.presenter.frontGeneration === b.snapshot.resizeEpoch.generation;
      }).toBe(true);
      const bundle = await captureDiagnostics(page);
      const samples = await page.evaluate(() => (globalThis as any).__directGeometrySamples) as {
        width: number; height: number; ratio: number; capacityWidth: number; capacityHeight: number;
      }[];
      const targets = bundle.trace.filter(e => e.phase === 'target-observed').map(e => ({
        time: e.timestampMicroseconds / 1000, generation: e.epoch.generation,
        width: e.epoch.logicalWidth, height: e.epoch.logicalHeight, dpr: e.epoch.devicePixelRatio }));
      const fronts = bundle.trace.filter(e => e.phase === 'front-commit' && e.source === 'worker-direct-surface').map(e => {
        const detail = JSON.parse(e.detail!);
        const target = targets.find(t => t.generation === detail.generation);
        return { time: e.timestampMicroseconds / 1000, generation: detail.generation,
          width: e.surfaceWidth / (target?.dpr ?? dpr), height: e.surfaceHeight / (target?.dpr ?? dpr), dpr: target?.dpr ?? dpr };
      });
      await testInfo.attach('direct-resize-evidence', { body: JSON.stringify({ samples, bundle,
        following: measureResizeFollowing(targets, fronts, start, end),
        limitation: 'CDP viewport steps and commit notifications; physical drag and pixels are separate gates' }), contentType: 'application/json' });
      expect(samples.length).toBeGreaterThan(2);
      expect(samples.filter(s => Math.abs(s.width * s.ratio - s.capacityWidth) > 1 ||
        Math.abs(s.height * s.ratio - s.capacityHeight) > 1)).toEqual([]);
      expect(fronts.filter(f => f.time >= start && f.time <= end).length).toBeGreaterThan(1);
      expect(errors).toEqual([]);
    } finally { await context.close(); }
  });
}
