import { writeFile } from 'node:fs/promises';
import { test, expect } from './helpers/fixtures.js';
import { openDoroti, captureDiagnostics, waitForSettledPresenter, assertPresenterContract } from './helpers/doroti-diagnostics.js';
import { observeServedAssets, sourceManifest } from './helpers/resize-manifest.js';

test.use({ trace: 'off', video: 'off', screenshot: 'only-on-failure' });
const c0 = '&dorotiResizeScheduling=display&dorotiCopyOwnership=owned&dorotiMetricsCoalescing=frame&dorotiEncodingCache=1';

test('CanvasKit bounded work counters connect stationary F3 metrics and frame phases', async ({ page }, testInfo) => {
  test.skip(process.env.DOROTI_WEB_RENDERER_MODE !== 'worker-canvaskit-webgl');
  const source = await sourceManifest(testInfo.outputPath('source.patch'));
  const samples: unknown[] = [];
  // Alternating diagnostic on/off stationary corpus, never native evidence.
  for (const trace of [false, true, true, false]) {
    await page.setViewportSize({ width: 960, height: 640 });
    const assets = observeServedAssets(page);
    await openDoroti(page, `${c0}&dorotiCanvasKitTrace=${trace ? 1 : 0}`);
    const served = await assets();
    const steps: unknown[] = [];
    for (const [width, height] of [[960,640], [800,640], [800,720], [960,720], [960,640]]) {
      const before = await captureDiagnostics(page);
      await page.setViewportSize({ width, height });
      if (width === before.snapshot.logicalWidth && height === before.snapshot.logicalHeight)
        await page.evaluate(() => { for (let i=0; i<8; i++) window.dispatchEvent(new Event('resize')); });
      await expect.poll(async () => {
        const s = (await captureDiagnostics(page)).snapshot;
        return [s.logicalWidth, s.logicalHeight];
      }).toEqual([width, height]);
      const after = await waitForSettledPresenter(page);
      assertPresenterContract(after);
      if (width === before.snapshot.logicalWidth && height === before.snapshot.logicalHeight)
        expect(after.snapshot.resizeEpoch.generation).toBe(before.snapshot.resizeEpoch.generation);
      steps.push({ width, height, before, after });
    }
    const stages = trace ? await page.evaluate(async () => (globalThis as any).__dorotiCanvasKitExperiment.collect()) : null;
    if (trace && process.env.DOROTI_WORK_BASELINE !== '1') {
      expect(stages.ui.dropped).toBe(0);
      expect(stages.ui.framework.trace.work.enabled).toBe(true);
      expect(stages.ui.framework.trace.work.dropped).toBe(0);
      const names = stages.ui.framework.trace.work.names as string[];
      expect(names).toContain('LayoutFastPath');
      expect(stages.ui.framework.trace.work.samples.length).toBeGreaterThan(0);
      const applies = stages.ui.entries.filter((e: any) => e.stage === 'ui-resize-applied');
      expect(applies.length).toBeGreaterThan(0);
      for (const e of applies) {
        expect(e.detail.applyMilliseconds).toBeGreaterThanOrEqual(0);
        expect(e.detail.snapshotJsonMilliseconds).toBeGreaterThanOrEqual(0);
        expect(e.detail.managedSnapshotMilliseconds).toBeGreaterThanOrEqual(0);
        expect(e.detail.reason).toBeTruthy();
      }
    }
    samples.push({ trace, served, steps, stages });
  }
  await writeFile(testInfo.outputPath('stationary-work.json'), JSON.stringify({ source, samples }, null, 2));
});
