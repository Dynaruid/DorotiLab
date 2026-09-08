import { test, expect } from './helpers/fixtures.js';
import { openDoroti } from './helpers/doroti-diagnostics.js';

test('numeric layout uses two threads with equal pixels and survives resize and shutdown', async ({ page, runtimeErrors }, testInfo) => {
  const rows: any[] = [];
  async function clickControl(role: 'checkbox' | 'button', name: string) {
    const control = page.getByRole(role, { name, exact: true });
    await expect(control).toBeAttached();
    const bounds = (await control.boundingBox())!;
    // Semantics nodes describe bounds; pointer input belongs to the visible canvas.
    await page.mouse.click(bounds.x + bounds.width / 2, bounds.y + bounds.height / 2);
  }
  page.on('console', message => {
    const marker = 'DOROTI_PARALLEL_LAYOUT ';
    const text = message.text();
    if (text.includes(marker)) rows.push(JSON.parse(text.slice(text.indexOf(marker) + marker.length)));
  });
  await page.addInitScript(() => {
    (globalThis as any).__layoutRoleMessages = [];
    const NativeWorker = Worker;
    globalThis.Worker = class extends NativeWorker {
      constructor(url: string | URL, options?: WorkerOptions) {
        super(url, options);
        this.addEventListener('message', event => {
          if (event.data?.kind !== 'doroti-managed-port') return;
          event.data.port.addEventListener('message', (message: MessageEvent) => {
            if (['runtime-ready', 'fatal', 'disposed'].includes(message.data?.kind))
              (globalThis as any).__layoutRoleMessages.push(message.data);
          });
        });
      }
    };
  });
  await page.setViewportSize({ width: 1280, height: 900 });
  const frame = await openDoroti(page, '&dorotiTestbedMode=parallel-layout&dorotiLayoutMode=parallel');
  expect(frame.presenter.mainManagedRuntimeCount).toBe(1);
  expect(frame.presenter.workerManagedRuntimeCount).toBe(0);
  await expect.poll(() => rows.length).toBeGreaterThan(0);
  const parallel = rows.at(-1);
  expect(parallel.parallel).toBe(true);
  expect(parallel.leftThread).not.toBe(parallel.rightThread);
  expect(parallel.leftThread).not.toBe(parallel.owner);
  expect(parallel.rightThread).not.toBe(parallel.owner);
  expect(parallel.resumedThread).toBe(parallel.owner);
  expect(parallel.OverlapMs).toBeGreaterThan(0);
  await page.mouse.move(10, 10); await page.waitForTimeout(700);
  const clip = { x: 24, y: 320, width: 1232, height: 540 };
  const pixels = await page.screenshot({ clip });
  let count = rows.length;
  await clickControl('checkbox', 'Serial');
  await expect.poll(() => rows.length).toBeGreaterThan(count);
  const serial = rows.at(-1);
  expect(serial.parallel).toBe(false);
  expect(serial.leftThread).toBe(serial.owner);
  expect(serial.rightThread).toBe(serial.owner);
  expect(serial.checksum).toBe(parallel.checksum);
  await page.mouse.move(10, 10); await page.waitForTimeout(700);
  expect(pixels.equals(await page.screenshot({ clip })), 'Serial and parallel rendered geometry is pixel-identical').toBe(true);
  count = rows.length;
  await clickControl('checkbox', 'Parallel (2 threads)');
  await expect.poll(() => rows.length).toBeGreaterThan(count);
  const beforeSeed = rows.at(-1).seed;
  count = rows.length;
  await clickControl('button', 'New layout');
  await expect.poll(() => rows.length).toBeGreaterThan(count);
  expect(rows.at(-1).seed).toBe(beforeSeed + 1);
  expect(rows.at(-1).checksum).not.toBe(parallel.checksum);
  // Rapid pending constraint changes must coalesce and never apply stale geometry.
  await page.setViewportSize({ width: 900, height: 900 });
  await expect.poll(() => rows.at(-1)?.width).toBe(860);
  const narrowRevision = rows.at(-1).revision;
  for (const width of [720, 1000, 1280]) await page.setViewportSize({ width, height: 900 });
  await expect.poll(() => rows.at(-1)?.width).toBe(1240);
  expect(rows.at(-1).revision).toBeGreaterThan(narrowRevision);
  expect(rows.at(-1).seed).toBe(beforeSeed + 1);
  await expect.poll(() => rows.at(-1)?.parallel).toBe(true);
  await page.waitForTimeout(800);
  expect(rows.every(row => row.owner === row.resumedThread)).toBe(true);
  await testInfo.attach('layout-execution', { body: JSON.stringify(rows, null, 2), contentType: 'application/json' });
  await testInfo.attach('parallel-layout', { body: await page.screenshot(), contentType: 'image/png' });
  await page.evaluate(() => globalThis.dispatchEvent(new PageTransitionEvent('pagehide')));
  await expect.poll(() => page.evaluate(() => (globalThis as any).__layoutRoleMessages.some((m: any) => m.kind === 'disposed'))).toBe(true);
  expect(await page.evaluate(() => (globalThis as any).__layoutRoleMessages.filter((m: any) => m.kind === 'fatal'))).toEqual([]);
  expect(runtimeErrors).toEqual([]);
});
