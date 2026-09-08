import { chromium } from 'playwright';
const browser = await chromium.launch({ channel: 'chromium', headless: true, args: ['--ignore-gpu-blocklist', '--enable-gpu-rasterization'] });
try {
  const page = await browser.newPage({ viewport: { width: 1280, height: 900 } });
  const logs = [];
  page.on('worker', worker => {
    // Diagnostic instrumentation only: preserve native behavior and report its actual validation message.
    worker.evaluate(() => {
      if (typeof GPUAdapter === 'undefined') return;
      const native = GPUAdapter.prototype.requestDevice;
      GPUAdapter.prototype.requestDevice = async function(...args) {
        const device = await native.apply(this, args);
        device.addEventListener('uncapturederror', event => console.error('GPU_VALIDATION_DETAIL ' + event.error.message));
        return device;
      };
    }).catch(() => {});
  });
  page.on('console', m => logs.push({ type: m.type(), text: m.text() }));
  await page.addInitScript(() => {
    globalThis.__combinedMessages = [];
    const Original = Worker;
    globalThis.Worker = class extends Original {
      constructor(url, options) {
        super(url, options);
        this.addEventListener('message', e => {
          if (e.data?.kind !== 'doroti-managed-port') return;
          e.data.port.addEventListener('message', m => {
            if (['runtime-ready', 'fatal', 'disposed'].includes(m.data?.kind)) globalThis.__combinedMessages.push(m.data);
          });
        });
      }
    };
  });
  await page.goto('http://127.0.0.1:5208/?dorotiResizeDiagnostics=1&dorotiRenderer=worker-direct-webgpu&dorotiTestbedMode=sample&dorotiParallelLayout=1', { timeout: 120000 });
  await page.waitForFunction(() => globalThis.__combinedMessages.some(m => m.kind === 'disposed'), null, { timeout: 20000 }).catch(() => {});
  console.log(JSON.stringify({ messages: await page.evaluate(() => globalThis.__combinedMessages), logs }, null, 2));
} finally { await browser.close(); }
