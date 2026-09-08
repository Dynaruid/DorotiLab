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
let page;
try {
  page = await browser.newPage({ viewport: { width: 1280, height: 900 }, deviceScaleFactor: 1 });
  let reportFailure;
  const runtimeFailure = new Promise(resolve => { reportFailure = resolve; });
  page.on('pageerror', error => { result.errors.push(String(error)); console.error(String(error)); reportFailure(error); });
  page.on('console', message => { if (message.type() === 'error') {
    result.consoleErrors.push(message.text()); console.error(message.text()); reportFailure(Error(message.text()));
  } });
  console.log('bootstrap: navigating');
  const response = await page.goto((process.env.DOROTI_WEB_BASE_URL ?? 'http://127.0.0.1:5192') +
    '/?dorotiTestbedMode=sample&dorotiResizeDiagnostics=1&dorotiInputMarkers=1');
  result.headers = await response.allHeaders();
  result.browser = browser.version();
  result.url = page.url();
  result.documentIsolated = await page.evaluate(() => crossOriginIsolated);
  console.log('bootstrap: document isolated', result.documentIsolated);
  if (!result.documentIsolated) throw Error('Origin is not cross-origin isolated');
  const ready = width => Promise.race([page.waitForFunction(width => {
    const d = globalThis.__dorotiResizeDiagnostics;
    if (!d?.hosts().length) return false;
    const s = JSON.parse(d.snapshot(d.hosts()[0]));
    const p = JSON.parse(d.presenter('doroti-surface'));
    return s.logicalWidth === width && p.frontRequestId > 0 && p.frontGeneration === s.resizeEpoch.generation &&
      JSON.parse(d.capture(d.hosts()[0])).some(e => e.phase === 'front-commit' && e.detail &&
        JSON.parse(e.detail).generation === s.resizeEpoch.generation && JSON.parse(e.detail).sceneDisposition === 'exact-rendered');
  }, width, { timeout: 120000 }), runtimeFailure.then(error => { throw error; })]);
  await ready(1280);
  console.log('bootstrap: exact first content');
  await Promise.race([page.waitForFunction(() =>
    document.querySelector('[data-doroti-worker-runtime="ready"]') !== null, undefined, { timeout: 30000 }),
  runtimeFailure.then(error => { throw error; })]);
  result.hostState = await page.evaluate(() => {
    const d = globalThis.__dorotiResizeDiagnostics;
    return { stage: document.documentElement.dataset.dorotiBootstrapStage,
      role: { ...document.querySelector('[data-doroti-worker-runtime="ready"]').dataset },
      presenter: JSON.parse(d.presenter('doroti-surface')),
      snapshot: JSON.parse(d.snapshot(d.hosts()[0])),
      trace: JSON.parse(d.capture(d.hosts()[0])).slice(-12) };
  });
  if (result.hostState.role.dorotiSharedRuntimeRenderThread !== 'true' ||
      result.hostState.role.dorotiRenderWorkerIsolated !== 'true' ||
      Number(result.hostState.role.dorotiRenderThreadId) <= 0 ||
      result.hostState.presenter.mainManagedRuntimeCount !== 1 || result.hostState.presenter.workerManagedRuntimeCount !== 0)
    throw Error('Renderer does not report the expected main-runtime/shared-worker ownership');
  result.mainRuntime = await page.evaluate(() => {
    const runtime = globalThis.getDotnetRuntime?.(0);
    return { location: document.documentElement.dataset.dorotiRuntimeLocation,
      info: runtime?.runtimeBuildInfo ?? null,
      sharedHeap: !!runtime && runtime.localHeapViewU8().buffer instanceof SharedArrayBuffer };
  });
  if (result.mainRuntime.location !== 'main' || !result.mainRuntime.info?.wasmEnableThreads || !result.mainRuntime.sharedHeap)
    throw Error('No main-owned threaded runtime with shared memory was found');
  // Idle pthreads can be parked in Atomics.wait and cannot service arbitrary
  // CDP evaluations. Observe the active role through its normal host protocol.
  result.workers = page.workers().map(worker => ({ url: worker.url() }));
  // The first exact commit can be the empty surface before the app's first
  // framework frame. First content requires actual pixels as well as readiness.
  const contentDeadline = Date.now() + 10000;
  do {
    const initial = await page.screenshot({ path: `${directory}/${label}-initial.png` });
    const png = PNG.sync.read(initial), colors = new Set();
    for (let i = 0; i < png.data.length; i += 16) colors.add(png.data.readUInt32BE(i));
    result.initialColors = colors.size;
    if (colors.size >= 100) break;
    if (Date.now() >= contentDeadline) throw Error('Initial image has insufficient painted content');
    await page.waitForTimeout(250);
  } while (true);
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
  if (page) {
    result.failureWorkers = page.workers().map(worker => ({ url: worker.url() }));
    await page.screenshot({ path: `${directory}/${label}-failure.png`, timeout: 5000 }).catch(() => {});
  }
  process.exitCode = 1;
} finally {
  await writeFile(`${directory}/${label}.json`, JSON.stringify(result, null, 2), { flag: 'wx' });
  console.log(JSON.stringify(result, null, 2));
  await browser.close(); clearTimeout(deadline);
}
