import { chromium } from '@playwright/test';
import { mkdir, writeFile } from 'node:fs/promises';

const label = process.argv[2];
if (!label || !/^[\w-]+$/.test(label)) throw Error('A unique artifact label is required');
const result = { label, errors: [], console: [] };
const browser = await chromium.launch({ headless: true });
try {
  const page = await browser.newPage();
  page.on('pageerror', error => result.errors.push(String(error)));
  page.on('console', message => { if (result.console.length < 25) result.console.push(message.text()); });
  await page.addInitScript(() => {
    const OriginalWorker = globalThis.Worker;
    globalThis.__workerMessages = [];
    globalThis.Worker = class extends OriginalWorker {
      constructor(...args) {
        super(...args);
        this.addEventListener('message', event => {
          if (globalThis.__workerMessages.length >= 30) return;
          const data = event.data;
          globalThis.__workerMessages.push({ url: String(args[0]), keys: Object.keys(data ?? {}),
            kind: data?.kind, protocolVersion: data?.protocolVersion,
            controls: Object.entries(data ?? {}).filter(([, value]) => value && typeof value === 'object' && 'monoCmd' in value)
              .map(([key, value]) => ({ key, monoCmd: value.monoCmd, info: value.info })) });
        });
      }
    };
  });
  await page.goto(process.env.DOROTI_WEB_BASE_URL ?? 'http://127.0.0.1:5192/?dorotiTestbedMode=sample');
  await page.waitForTimeout(10000);
  result.messages = await page.evaluate(() => globalThis.__workerMessages);
  result.workers = await Promise.all(page.workers().map(async worker => ({ url: worker.url(),
    ...await worker.evaluate(() => ({ sidecar: globalThis.dotnetSidecar ?? null,
      onmessage: String(globalThis.onmessage), importScripts: typeof importScripts,
      runtime: globalThis.getDotnetRuntime?.(0)?.runtimeBuildInfo ?? null,
      isolated: crossOriginIsolated })).catch(error => ({ error: String(error) })) })));
} finally {
  await mkdir('artifacts/threads-bootstrap', { recursive: true });
  await writeFile(`artifacts/threads-bootstrap/${label}.json`, JSON.stringify(result, null, 2), { flag: 'wx' });
  console.log(JSON.stringify(result, null, 2));
  await browser.close();
}
