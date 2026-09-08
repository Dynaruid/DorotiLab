import { test, expect } from '@playwright/test';

test('unavailable default WebGPU reports failure without selecting WebGL', async ({ page }, testInfo) => {
  const errors: string[] = [];
  page.on('pageerror', error => errors.push(String(error)));
  page.on('console', event => { if (event.type() === 'error') errors.push(event.text()); });
  let injected = 0;
  await page.route(/\/doroti\.webgpu(?:\.[^.]+)?\.js$/, async route => {
    const response = await route.fetch();
    const source = await response.text();
    const original = 'const adapter = await navigator.gpu.requestAdapter();';
    expect(source).toContain(original);
    injected++;
    await route.fulfill({ response, body: source.replace(original, 'const adapter = null;') });
  });
  await page.goto('/?dorotiResizeDiagnostics=1&dorotiTestbedMode=sample');
  await expect.poll(() => errors.join('\n')).toContain('Doroti WebGPU adapter is unavailable');
  await expect.poll(() => page.evaluate(() =>
    (globalThis as any).__dorotiResizeDiagnostics.hosts().length)).toBe(0);
  expect(await page.evaluate(() => document.documentElement.dataset.dorotiRenderer)).toBe('worker-direct-webgpu');
  expect(await page.evaluate(() => (globalThis as any).getDotnetRuntime(0).localHeapViewU8().byteLength)).toBeGreaterThan(0);
  expect(injected).toBeGreaterThan(0);
  expect(errors.every(error => error.includes('Doroti WebGPU adapter is unavailable'))).toBe(true);
  await testInfo.attach('expected-unavailable-errors', { body: JSON.stringify(errors), contentType: 'application/json' });
});
