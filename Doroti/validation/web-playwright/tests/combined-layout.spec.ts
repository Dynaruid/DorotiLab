import { test, expect } from './helpers/fixtures.js';
import { openDoroti, captureDiagnostics } from './helpers/doroti-diagnostics.js';
import type { Page } from '@playwright/test';
import { PNG } from 'pngjs';

const query = '&dorotiTestbedMode=sample&dorotiParallelLayout=1&dorotiLayoutMode=parallel';
async function click(page: Page, role: 'checkbox' | 'button', name: string) {
  const target = page.getByRole(role, { name, exact: true });
  const bounds = (await target.boundingBox())!;
  expect(bounds).not.toBeNull();
  await page.mouse.click(bounds.x + bounds.width / 2, bounds.y + bounds.height / 2);
}

test('WebGPU Material sample and parallel panel share one runtime with equal serial pixels', async ({ page, runtimeErrors }, testInfo) => {
  const rows: any[] = [];
  const blockingWarnings: string[] = [];
  page.on('console', message => {
    const marker = 'DOROTI_PARALLEL_LAYOUT ';
    const text = message.text();
    if (text.includes(marker)) rows.push(JSON.parse(text.slice(text.indexOf(marker) + marker.length)));
    if (text.includes('Blocking the thread with JS interop')) blockingWarnings.push(text);
  });
  await page.setViewportSize({ width: 1280, height: 900 });
  const frame = await openDoroti(page, query);
  expect(frame.presenter.mode).toBe('worker-direct-webgpu');
  expect(frame.snapshot.gpu.api).toBe('webgpu');
  expect(frame.snapshot.gpu.hardware).toBe(true);
  expect(frame.snapshot.gpu.softwareFallbackUsed).toBe(false);
  expect(frame.presenter.mainManagedRuntimeCount).toBe(1);
  expect(frame.presenter.workerManagedRuntimeCount).toBe(0);
  expect(await page.evaluate(() => {
    const runtime = (globalThis as any).getDotnetRuntime(0);
    return runtime.runtimeBuildInfo.wasmEnableThreads && runtime.localHeapViewU8().buffer instanceof SharedArrayBuffer;
  })).toBe(true);
  await expect.poll(() => rows.length).toBeGreaterThan(0);
  const parallel = rows.at(-1);
  expect(parallel.leftThread).not.toBe(parallel.rightThread);
  expect(parallel.leftThread).not.toBe(parallel.owner);
  expect(parallel.rightThread).not.toBe(parallel.owner);
  expect(parallel.OverlapMs).toBeGreaterThan(0);
  await page.mouse.move(10, 10); await page.waitForTimeout(500);
  const caption = (await page.locator('[aria-label^="실험용 treemap"]').boundingBox())!;
  const clip = { x: 25, y: Math.ceil(caption.y + caption.height + 15), width: 1230, height: 90 };
  const before = await page.screenshot({ clip });
  let count = rows.length;
  await click(page, 'checkbox', 'Serial');
  await expect.poll(() => rows.length).toBeGreaterThan(count);
  expect(rows.at(-1).parallel).toBe(false);
  expect(rows.at(-1).leftThread).toBe(rows.at(-1).owner);
  expect(rows.at(-1).checksum).toBe(parallel.checksum);
  await page.mouse.move(10, 10); await page.waitForTimeout(500);
  const after = await page.screenshot({ clip });
  await testInfo.attach('parallel-tile-crop', { body: before, contentType: 'image/png' });
  await testInfo.attach('serial-tile-crop', { body: after, contentType: 'image/png' });
  const a = PNG.sync.read(before), b = PNG.sync.read(after);
  let changed = 0, maximumDelta = 0;
  for (let i = 0; i < a.data.length; i += 4) {
    let delta = 0;
    for (let c = 0; c < 4; c++) delta = Math.max(delta, Math.abs(a.data[i + c] - b.data[i + c]));
    if (delta) changed++;
    maximumDelta = Math.max(maximumDelta, delta);
  }
  await testInfo.attach('pixel-comparison', { body: JSON.stringify({ changed, maximumDelta, pixels: a.width * a.height, clip }), contentType: 'application/json' });
  expect.soft(changed, 'Exact tile pixel parity').toBe(0);
  count = rows.length;
  await click(page, 'checkbox', 'Parallel (2 threads)');
  await expect.poll(() => rows.length).toBeGreaterThan(count);
  count = rows.length;
  await click(page, 'button', 'New layout');
  await expect.poll(() => rows.length).toBeGreaterThan(count);
  const seed = rows.at(-1).seed;
  expect(seed).toBe(1);
  await page.setViewportSize({ width: 800, height: 900 });
  await expect.poll(() => rows.at(-1)?.width).toBe(760);
  await page.setViewportSize({ width: 1280, height: 900 });
  await expect.poll(() => rows.at(-1)?.width).toBe(1240);
  expect(rows.at(-1).seed).toBe(seed);
  const restored = rows.at(-1).checksum;
  const toggle = page.locator('[role="button"][aria-description="Toggle brightness"]');
  const bounds = (await toggle.boundingBox())!;
  await page.mouse.click(bounds.x + bounds.width / 2, bounds.y + bounds.height / 2);
  await page.mouse.move(10, 10); await page.waitForTimeout(700);
  expect(rows.at(-1).seed).toBe(seed);
  expect(rows.at(-1).checksum).toBe(restored);
  await expect(page.getByRole('checkbox', { name: 'Parallel (2 threads)', exact: true })).toHaveAttribute('aria-checked', 'true');
  // Scroll the original Material controls under the fixed experimental panel.
  await page.mouse.move(1100, 720); await page.mouse.wheel(0, 380); await page.waitForTimeout(500);
  expect((await captureDiagnostics(page)).presenter.mode).toBe('worker-direct-webgpu');
  expect(rows.every(row => row.resumedThread === row.owner)).toBe(true);
  await testInfo.attach('combined-layout-execution', { body: JSON.stringify(rows, null, 2), contentType: 'application/json' });
  await testInfo.attach('combined-layout', { body: await page.screenshot(), contentType: 'image/png' });
  expect(runtimeErrors).toEqual([]);
  expect(blockingWarnings).toEqual([]);
});

test('combined WebGPU required shutdown gate remains independently checked', async ({ page, runtimeErrors }) => {
  await openDoroti(page, query);
  await expect(page.getByRole('checkbox', { name: 'Parallel (2 threads)', exact: true })).toBeAttached();
  await page.waitForTimeout(700);
  await page.evaluate(() => globalThis.dispatchEvent(new PageTransitionEvent('pagehide')));
  await expect.poll(() => page.evaluate(() => (globalThis as any).__dorotiResizeDiagnostics.hosts().length)).toBe(0);
  await page.waitForTimeout(700);
  // Do not ignore the preserved Dawn mapping error or turn this known failure into an expected pass.
  expect(runtimeErrors).toEqual([]);
});
