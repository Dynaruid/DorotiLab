import { chromium } from '@playwright/test';
import { mkdir, writeFile } from 'node:fs/promises';
import { PNG } from 'pngjs';

const label = process.argv[2];
if (!/^[\w-]+$/.test(label)) throw Error('A simple artifact label is required');
const directory = 'artifacts/threads-bootstrap';
await mkdir(directory, { recursive: true });
const result = { label, errors: [], consoleErrors: [], steps: [], limitation:
  'Threaded-runtime bootstrap and small UI smoke only; no parallel layout or managed compute overlap proof.' };
const deadline = setTimeout(() => { console.error('20-minute timeout'); process.exit(1); }, 1200000);
const browser = await chromium.launch({ headless: true, args:
  ['--enable-gpu-rasterization', '--ignore-gpu-blocklist', '--use-angle=default'] });
try {
  const page = await browser.newPage({ viewport: { width: 1280, height: 900 }, deviceScaleFactor: 1 });
  page.on('pageerror', error => result.errors.push(String(error)));
  page.on('console', message => { if (message.type() === 'error') result.consoleErrors.push(message.text()); });
  const response = await page.goto((process.env.DOROTI_WEB_BASE_URL ?? 'http://127.0.0.1:5192') +
    '/?dorotiTestbedMode=sample&dorotiResizeDiagnostics=1&dorotiInputMarkers=1');
  result.headers = await response.allHeaders();
  result.browser = browser.version();
  result.url = page.url();
  result.documentIsolated = await page.evaluate(() => crossOriginIsolated);
  if (!result.documentIsolated) throw Error('Origin is not cross-origin isolated');
  const ready = width => page.waitForFunction(width => {
    const d = globalThis.__dorotiResizeDiagnostics;
    if (!d?.hosts().length) return false;
    const s = JSON.parse(d.snapshot(d.hosts()[0]));
    const p = JSON.parse(d.presenter('doroti-surface'));
    return s.logicalWidth === width && p.frontRequestId > 0 && p.frontGeneration === s.resizeEpoch.generation &&
      JSON.parse(d.capture(d.hosts()[0])).some(e => e.phase === 'front-commit' && e.detail &&
        JSON.parse(e.detail).generation === s.resizeEpoch.generation && JSON.parse(e.detail).sceneDisposition === 'exact-rendered');
  }, width, { timeout: 120000 });
  await ready(1280);
  result.workers = await Promise.all(page.workers().map(async worker => ({ url: worker.url(),
    ...await worker.evaluate(() => {
      const runtime = globalThis.getDotnetRuntime?.(0);
      return { isolated: crossOriginIsolated, runtime: runtime?.runtimeBuildInfo ?? null,
        sharedHeap: !!runtime && runtime.localHeapViewU8().buffer instanceof SharedArrayBuffer };
    }) })));
  if (!result.workers.some(w => w.isolated && w.sharedHeap && w.runtime?.wasmEnableThreads))
    throw Error('No running threaded managed runtime with a shared heap was found');
  const initial = await page.screenshot({ path: `${directory}/${label}-initial.png` });
  const png = PNG.sync.read(initial), colors = new Set();
  for (let i = 0; i < png.data.length; i += 16) colors.add(png.data.readUInt32BE(i));
  result.initialColors = colors.size;
  if (colors.size < 100) throw Error('Initial image has insufficient painted content');
  for (const width of [800, 1280]) {
    await page.setViewportSize({ width, height: 900 });
    await ready(width);
    result.steps.push({ width, exact: true });
  }
  const beforeWheel = await page.screenshot();
  await page.mouse.move(1000, 650);
  await page.mouse.wheel(0, 500);
  // One bounded input observation, not repeated performance trials.
  await page.waitForTimeout(1500);
  result.wheelChangedPixels = !beforeWheel.equals(await page.screenshot({ path: `${directory}/${label}-scrolled.png` }));
  if (!result.wheelChangedPixels) throw Error('Wheel input did not change the presented image');
  if (result.errors.length || result.consoleErrors.length) throw Error('Runtime/console errors were reported');
  result.status = 'PASS';
} catch (error) {
  result.status = 'FAIL'; result.failure = String(error); result.stack = error.stack;
  process.exitCode = 1;
} finally {
  await writeFile(`${directory}/${label}.json`, JSON.stringify(result, null, 2));
  console.log(JSON.stringify(result, null, 2));
  await browser.close(); clearTimeout(deadline);
}
