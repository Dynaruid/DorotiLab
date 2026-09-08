import { test, expect } from './helpers/fixtures.js';
import { openDoroti } from './helpers/doroti-diagnostics.js';

test('browser owner timers stress cancellation, Future delays and timeouts without the .NET timer queue', async ({ page, runtimeErrors }, testInfo) => {
  test.skip(process.env.DOROTI_TIMER_VALIDATION !== '1', 'Requires a DorotiTimerValidation publish.');
  await openDoroti(page, '&dorotiTestbedMode=sample');
  const owner = await Promise.any(page.workers().map(async worker => {
    if (!await worker.evaluate(() => typeof (globalThis as any).__dorotiDirectDiagnostics === 'function'))
      throw new Error('Idle pthread');
    return worker;
  }));
  const before = await owner.evaluate(() => (globalThis as any).__dorotiDirectDiagnostics().timers);
  expect(before.backend).toBe('browser-owner-timeout');
  expect(before.systemTimerCount).toBe(0);
  const result = await owner.evaluate(async () => {
    const runtime = (globalThis as any).getDotnetRuntime(0);
    const exports = await runtime.getAssemblyExports('DorotiTestbedApp.Web.dll');
    return JSON.parse(await exports.DorotiTestbedApp.Web.Validation.TimerValidationExport.Run(false, 60000));
  });
  expect(result.callbacks).toBeGreaterThan(10000);
  expect(result.wrongThread).toBe(0);
  expect(result.canceledCalls).toBe(0);
  expect(result.zeroCalls).toBe(1);
  expect(result.delayed).toBe(42);
  expect(result.timeoutResult).toBe(99);
  expect(result.timedOut).toBe(true);
  expect(result.peakSystemTimers).toBe(0);
  expect(result.diagnostics.crossThreadOperations).toBeGreaterThan(0);
  const after = await owner.evaluate(() => (globalThis as any).__dorotiDirectDiagnostics().timers);
  expect(after.liveTimers).toBeLessThanOrEqual(before.liveTimers);
  expect(after.systemTimerCount).toBe(0);
  expect(runtimeErrors).toEqual([]);
  await testInfo.attach('browser-timers', { body: JSON.stringify({ before, result, after }, null, 2), contentType: 'application/json' });
});
