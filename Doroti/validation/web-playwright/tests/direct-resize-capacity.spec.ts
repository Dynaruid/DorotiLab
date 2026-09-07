import { test, expect } from './helpers/fixtures.js';
import { openDoroti, captureDiagnostics } from './helpers/doroti-diagnostics.js';
import { measureResizeFollowing } from './helpers/resize-following.js';

for (const dpr of [1, 2]) {
  test(`direct capacity completes grow before return DPR ${dpr}`, async ({ browser }, testInfo) => {
    const context = await browser.newContext({ viewport: { width: 1000, height: 720 }, deviceScaleFactor: dpr });
    const page = await context.newPage();
    try {
      await openDoroti(page, '&dorotiRenderer=worker-direct-webgl&dorotiTestbedMode=sample');
      const before = await captureDiagnostics(page);
      const capacity = await page.locator('canvas').first().evaluate(canvas => ({
        width: Number(canvas.dataset.dorotiCapacityWidth), height: Number(canvas.dataset.dorotiCapacityHeight),
      }));
      await page.setViewportSize({ width: Math.ceil(capacity.width / dpr) + 100, height: 900 });
      await expect.poll(async () => {
        const frame = await captureDiagnostics(page);
        return frame.presenter.frontGeneration === frame.snapshot.resizeEpoch.generation &&
          frame.snapshot.resizeEpoch.physicalWidth > capacity.width;
      }).toBe(true);
      const grown = await captureDiagnostics(page);
      expect(await page.locator('canvas').first().evaluate(canvas => Number(canvas.dataset.dorotiCapacityWidth)))
        .toBeGreaterThan(capacity.width);
      await page.setViewportSize({ width: 1000, height: 720 });
      await expect.poll(async () => {
        const frame = await captureDiagnostics(page);
        return frame.presenter.frontGeneration === frame.snapshot.resizeEpoch.generation && frame.presenter.queueDepth === 0;
      }).toBe(true);
      const returned = await captureDiagnostics(page);
      expect(returned.presenter.frontRequestId).toBeGreaterThan(grown.presenter.frontRequestId ?? 0);
      await testInfo.attach('completed-capacity-grow', { body: JSON.stringify({ before, capacity, grown, returned }), contentType: 'application/json' });
    } finally { await context.close(); }
  });
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
      const managed = await Promise.all(page.workers().map(worker => worker.evaluate(() => (globalThis as any).__dorotiDirectDiagnostics?.() ?? null)));
      const following = measureResizeFollowing(targets, fronts, start, end);
      await testInfo.attach('direct-resize-evidence', { body: JSON.stringify({ samples, bundle, managed,
        following,
        limitation: 'CDP viewport steps and commit notifications; physical drag and pixels are separate gates' }), contentType: 'application/json' });
      expect(samples.length).toBeGreaterThan(2);
      expect(samples.filter(s => Math.abs(s.width * s.ratio - s.capacityWidth) > 1 ||
        Math.abs(s.height * s.ratio - s.capacityHeight) > 1)).toEqual([]);
      expect(fronts.filter(f => f.time >= start && f.time <= end).length).toBeGreaterThan(1);
      expect(errors).toEqual([]);
      expect(following.boundaryInclusiveGaps.max, "active resize starvation hard gate").toBeLessThan(100);
      expect(following.caughtUp?.p95, "target tracking hard gate").toBeLessThanOrEqual(50);
      expect(following.settleFromObserverMilliseconds, "latest exact hard gate").toBeLessThanOrEqual(100);
    } finally { await context.close(); }
  });
}
