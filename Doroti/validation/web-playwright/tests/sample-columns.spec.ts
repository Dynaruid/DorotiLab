import { test, expect } from './helpers/fixtures.js';
import { openDoroti, captureDiagnostics } from './helpers/doroti-diagnostics.js';

for (const dpr of [1, 2]) test.describe(`DPR ${dpr}`, () => {
test.use({ deviceScaleFactor: dpr });
for (const initialWidth of [800, 1280]) {
  test(`sample columns follow logical width from ${initialWidth}`, async ({ page, runtimeErrors }, testInfo) => {
    await page.setViewportSize({ width: initialWidth, height: 900 });
    await openDoroti(page, '&dorotiTestbedMode=sample');
    for (const [step, width] of [initialWidth, 1280, 800, 1001, 1000, 390, 1501, 800].entries()) {
      await page.setViewportSize({ width, height: 900 });
      await expect.poll(async () => {
        const frame = await captureDiagnostics(page);
        return frame.snapshot.logicalWidth === width && frame.presenter.frontGeneration === frame.snapshot.resizeEpoch.generation;
      }).toBe(true);
      const actions = page.getByRole('group', { name: 'Actions Common buttons', exact: true });
      const navigation = page.getByRole('group', { name: 'Navigation Bottom app bar', exact: true });
      await expect.poll(async () => {
        const box = await actions.boundingBox();
        return box !== null && (width > 1000 ? box.width < width * .55 : box.width > width * .8);
      }, { message: `Gallery column width matches ${width} CSS pixels` }).toBe(true);
      await expect(navigation).toHaveCount(width > 1000 ? 1 : 0);
      if (width > 1000) {
        const left = (await actions.boundingBox())!;
        const right = (await navigation.boundingBox())!;
        expect(right.x).toBeGreaterThanOrEqual(left.x + left.width);
        expect(Math.abs(right.y - left.y)).toBeLessThan(1);
      }
      await page.screenshot({ path: testInfo.outputPath(`columns-${step}-${width}.png`) });
      console.log('COLUMN_GEOMETRY', JSON.stringify({ width, actions: await actions.boundingBox() }));
    }
    expect(runtimeErrors).toEqual([]);
  });
}
});
