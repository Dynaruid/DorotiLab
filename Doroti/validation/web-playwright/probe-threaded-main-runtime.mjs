import { chromium } from '@playwright/test';
import { mkdir, writeFile } from 'node:fs/promises';

const label = process.argv[2];
if (!label || !/^[\w-]+$/.test(label)) throw Error('A unique artifact label is required');
const result = { label, errors: [], limitation: 'Runtime creation only; no Doroti app, renderer or input acceptance.' };
const browser = await chromium.launch({ headless: true });
try {
  const page = await browser.newPage();
  page.on('pageerror', error => result.errors.push(String(error)));
  await page.route('**/thread-runtime-check.html', route => route.fulfill({
    contentType: 'text/html', body: '<!doctype html><title>Thread runtime isolation</title>',
    headers: { 'Cross-Origin-Opener-Policy': 'same-origin', 'Cross-Origin-Embedder-Policy': 'require-corp' },
  }));
  await page.goto('http://127.0.0.1:5192/thread-runtime-check.html');
  result.runtime = await page.evaluate(async () => {
    const { dotnet } = await import('./_framework/dotnet.js');
    const runtime = await dotnet.create();
    return { isolated: crossOriginIsolated, ...runtime.runtimeBuildInfo,
      sharedHeap: runtime.localHeapViewU8().buffer instanceof SharedArrayBuffer };
  });
  result.status = result.runtime.isolated && result.runtime.sharedHeap && result.runtime.wasmEnableThreads && !result.errors.length ? 'PASS' : 'FAIL';
} catch (error) {
  result.status = 'FAIL'; result.failure = String(error);
} finally {
  if (result.status !== 'PASS') process.exitCode = 1;
  await mkdir('artifacts/threads-bootstrap', { recursive: true });
  await writeFile(`artifacts/threads-bootstrap/${label}.json`, JSON.stringify(result, null, 2), { flag: 'wx' });
  console.log(JSON.stringify(result, null, 2));
  await browser.close();
}
