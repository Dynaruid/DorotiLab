import { writeFile } from 'node:fs/promises';
import { PNG } from 'pngjs';
import { test, expect } from './helpers/fixtures.js';
import { openDoroti, waitForSettledPresenter, captureDiagnostics } from './helpers/doroti-diagnostics.js';

test.use({ trace: 'off', video: 'off' });
test('F3 responsive gallery text crosses a stationary wrap threshold', async ({ page }, testInfo) => {
  const renderer = process.env.DOROTI_WEB_RENDERER_MODE ?? 'auto';
  test.fail(renderer === 'document-webgl',
    'C0 and C1 document raster retains one clipped line despite multiline semantics; see web-canvaskit-n-results.md. An eventual fix must remove this expected failure.');
  await page.setViewportSize({ width: 640, height: 800 });
  await openDoroti(page, '&dorotiResizeScheduling=display&dorotiCopyOwnership=owned&dorotiEncodingCache=1&dorotiMetricsCoalescing=frame');
  const label = page.getByRole('group', { name: 'Reviewed Material · promoted product · strict Skia GPU', exact: true });
  const observations: unknown[] = [];
  const at = async (width: number) => {
    await page.setViewportSize({ width, height: 800 });
    await expect.poll(async () => (await captureDiagnostics(page)).snapshot.logicalWidth).toBe(width);
    await waitForSettledPresenter(page);
    const box = await label.boundingBox();
    expect(box).not.toBeNull();
    expect(box!.x).toBeGreaterThanOrEqual(0);
    expect(box!.x + box!.width).toBeLessThanOrEqual(width + .1);
    const bytes = await label.screenshot();
    const png = PNG.sync.read(bytes);
    const bg = [...png.data.subarray(0,3)];
    const bands: number[][] = [];
    for (let y=0; y<png.height; y++) {
      let ink = 0;
      for (let x=0; x<png.width; x++) {
        const p = (y*png.width+x)*4;
        if ([0,1,2].some(c => Math.abs(png.data[p+c]-bg[c])>50)) ink++;
      }
      if (ink > 2) {
        if (!bands.length || y > bands.at(-1)!.at(-1)!+1) bands.push([]);
        bands.at(-1)!.push(y);
      }
    }
    const inkBands = bands.filter(b=>b.length>1).map(b=>[b[0], b.at(-1)]);
    observations.push({ width, box, inkBands });
    return { box: box!, bytes, png, inkBands };
  };
  const wide = await at(640);
  const narrow = await at(320);
  expect(narrow.inkBands.length).toBe(2);
  expect(wide.inkBands.length).toBe(1);
  let low = 320, high = 640;
  while (high - low > 1) {
    const mid = Math.floor((low+high)/2);
    // Locate the raster threshold itself. Semantics height may use different
    // paragraph metrics on a different renderer; it cannot stand in for ink.
    if ((await at(mid)).inkBands.length > 1) low = mid; else high = mid;
  }
  const endpoints = [];
  for (const width of [low, high]) {
    const { box, bytes, png, inkBands } = await at(width);
    await writeFile(testInfo.outputPath(`wrap-${width}.png`), bytes);
    endpoints.push({ width, box, rasterWidth: png.width, rasterHeight: png.height,
      inkBands });
  }
  await writeFile(testInfo.outputPath('responsive-wrap.json'), JSON.stringify({ observations, threshold: { low, high }, endpoints,
    limitation: 'Stationary browser pixels and semantics bounds; native wrap-in-motion and physical scan-out not verified' }, null, 2));
  expect(endpoints[0].inkBands.length).toBe(2);
  expect(endpoints[1].inkBands.length).toBe(1);
});
