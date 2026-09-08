import { test, expect } from './helpers/fixtures.js';
import { openDoroti, captureDiagnostics } from './helpers/doroti-diagnostics.js';
import type { Page } from '@playwright/test';
import { test as faultTest } from '@playwright/test';

async function observeRole(page: Page) {
  await page.addInitScript(() => {
    const scope = globalThis as typeof globalThis & { __threadRoleMessages: Array<Record<string, unknown>> };
    scope.__threadRoleMessages = [];
    const NativeWorker = Worker;
    globalThis.Worker = class extends NativeWorker {
      constructor(url: string | URL, options?: WorkerOptions) {
        super(url, options);
        this.addEventListener('message', event => {
          if (event.data?.kind !== 'doroti-managed-port') return;
          const port = event.data.port as MessagePort;
          port.addEventListener('message', message => {
            if (['runtime-ready', 'fatal', 'disposed', 'gpu-disposed'].includes(message.data?.kind))
              scope.__threadRoleMessages.push(message.data);
          });
        });
      }
    };
  });
  await page.setViewportSize({ width: 1280, height: 900 });
  await openDoroti(page, '&dorotiTestbedMode=sample');
  await expect.poll(() => page.evaluate(() =>
    (globalThis as any).__threadRoleMessages.some((message: any) => message.kind === 'runtime-ready'))).toBe(true);
  const frame = await captureDiagnostics(page);
  expect(frame.presenter.mainManagedRuntimeCount).toBe(1);
  expect(frame.presenter.workerManagedRuntimeCount).toBe(0);
  expect(await page.evaluate(() => {
    const runtime = (globalThis as any).getDotnetRuntime(0);
    return runtime.runtimeBuildInfo.wasmEnableThreads && runtime.localHeapViewU8().buffer instanceof SharedArrayBuffer;
  })).toBe(true);
}

test('threaded renderer disposes its role without exiting the main runtime', async ({ page, runtimeErrors }) => {
  await observeRole(page);
  await page.mouse.move(1000, 650);
  await page.mouse.wheel(0, 500);
  // Covers a queued timer/frame wakeup racing the owner shutdown.
  await page.evaluate(() => globalThis.dispatchEvent(new PageTransitionEvent('pagehide')));
  await expect.poll(() => page.evaluate(() =>
    (globalThis as any).__threadRoleMessages.some((message: any) => message.kind === 'disposed'))).toBe(true);
  const state = await page.evaluate(() => ({
    messages: (globalThis as any).__threadRoleMessages,
    hosts: (globalThis as any).__dorotiResizeDiagnostics.hosts(),
    heap: (globalThis as any).getDotnetRuntime(0).localHeapViewU8().byteLength,
  }));
  expect(state.messages.filter((message: any) => message.kind === 'disposed')).toHaveLength(1);
  expect(state.messages.some((message: any) => message.kind === 'fatal')).toBe(false);
  expect(state.hosts).toEqual([]);
  expect(state.heap).toBeGreaterThan(0);
  const timerState = state.messages.find((message: any) => message.kind === 'disposed').timers;
  expect(timerState.liveTimers).toBe(0);
  expect(timerState.pending).toBe(0);
  expect(timerState.systemTimerCount).toBe(0);
  const gpu = state.messages.find((message: any) => message.kind === 'gpu-disposed')?.diagnostics;
  if (gpu) {
    expect(gpu.pendingMaps).toBe(0);
    expect(gpu.inFlight).toBe(0);
    expect(gpu.mappedRanges.active).toBe(0);
    expect(gpu.mappedRanges.discarded).toBe(0);
  }
  expect(runtimeErrors).toEqual([]);
});

test('threaded renderer rejects invalid private-port protocol without a second runtime', async ({ page, runtimeErrors }) => {
  await observeRole(page);
  await page.evaluate(() => (globalThis as any).__dorotiResizeDiagnostics.violateWorkerProtocol('doroti-surface'));
  await expect.poll(() => page.evaluate(() =>
    (globalThis as any).__threadRoleMessages.some((message: any) => message.kind === 'disposed'))).toBe(true);
  const messages = await page.evaluate(() => (globalThis as any).__threadRoleMessages);
  expect(messages.filter((message: any) => message.kind === 'runtime-ready')).toHaveLength(1);
  expect(messages.find((message: any) => message.kind === 'fatal').error).toContain('999');
  expect(runtimeErrors).toEqual([]);
});

faultTest('WebGPU device loss terminates the render role and preserves the shared runtime', async ({ page }, testInfo) => {
  const pageErrors: string[] = [];
  const consoleErrors: string[] = [];
  page.on('pageerror', error => pageErrors.push(String(error)));
  page.on('console', message => { if (message.type() === 'error') consoleErrors.push(message.text()); });
  await observeRole(page);
  const before = await captureDiagnostics(page);
  faultTest.skip(before.presenter.mode !== 'worker-direct-webgpu', 'WebGPU device-loss injection.');
  await page.mouse.move(1000, 650);
  await page.mouse.wheel(0, 500);
  await page.evaluate(() => (globalThis as any).__dorotiResizeDiagnostics.loseContext('doroti-surface'));
  await expect.poll(() => page.evaluate(() =>
    (globalThis as any).__threadRoleMessages.some((message: any) => message.kind === 'disposed'))).toBe(true);
  const result = await page.evaluate(() => ({
    messages: (globalThis as any).__threadRoleMessages,
    hosts: (globalThis as any).__dorotiResizeDiagnostics.hosts(),
    heap: (globalThis as any).getDotnetRuntime(0).localHeapViewU8().byteLength,
  }));
  expect(result.messages.filter((m: any) => m.kind === 'disposed')).toHaveLength(1);
  expect(result.messages.filter((m: any) => m.kind === 'runtime-ready')).toHaveLength(1);
  expect(result.messages.some((m: any) => m.kind === 'fatal' && /device lost/i.test(m.error))).toBe(true);
  expect(result.hosts).toEqual([]);
  expect(result.heap).toBeGreaterThan(0);
  const timers = result.messages.find((m: any) => m.kind === 'disposed').timers;
  expect(timers.liveTimers).toBe(0);
  expect(timers.pending).toBe(0);
  expect(timers.systemTimerCount).toBe(0);
  const gpu = result.messages.find((m: any) => m.kind === 'gpu-disposed').diagnostics;
  expect(gpu.pendingMaps).toBe(0);
  expect(gpu.inFlight).toBe(0);
  expect(gpu.mappedRanges.active).toBe(0);
  expect(gpu.mappedRanges.discarded).toBeGreaterThan(0);
  expect(pageErrors).toEqual([]);
  // Loss can cancel genuine in-flight native maps. Preserve those diagnostics;
  // normal shutdown and invalid-protocol shutdown still require zero errors.
  expect(consoleErrors.filter(error => !error.includes('[skia] Buffer async map failed') ||
    !/device.*lost/i.test(error))).toEqual([]);
  await testInfo.attach('device-loss', { body: JSON.stringify({ ...result, consoleErrors }), contentType: 'application/json' });
});
