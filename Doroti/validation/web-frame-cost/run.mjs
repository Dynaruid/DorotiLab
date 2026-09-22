// Run through ../run-with-timeout.py. One fresh browser per invocation; no retries.
import { createServer } from 'node:http';
import { createHash } from 'node:crypto';
import { spawn } from 'node:child_process';
import { access, mkdir, readFile, writeFile } from 'node:fs/promises';
import { resolve, join } from 'node:path';

const [directory, upstream, profile = 'minimal', backend = 'worker-direct-webgpu', width = '1280', dpr = '1'] = process.argv.slice(2);
if (!directory || !upstream) throw Error('run.mjs OUTPUT URL_OR_WWWROOT [inspect|diagnosis|minimal|off|smoke] [backend] [width] [dpr]');
const out = resolve(directory);
if (await access(join(out, 'chrome-profile')).then(() => true, () => false)) throw Error('Use a fresh output directory');
await mkdir(out, { recursive: true });
const assets = new Map(), events = [], input = [], segments = [];
const save = (name, value) => writeFile(join(out, name + '.json'), JSON.stringify(value, null, 2));
const proxy = createServer(async (req, res) => {
  try {
    let response, body;
    if (/^https?:/.test(upstream)) {
      response = await fetch(upstream + req.url);
      body = Buffer.from(await response.arrayBuffer());
    } else {
      const root = resolve(upstream);
      const path = decodeURIComponent(new URL(req.url, 'http://localhost').pathname);
      const file = resolve(root, '.' + (path === '/' ? '/index.html' : path));
      if (!file.startsWith(root + '\\') && !file.startsWith(root + '/')) throw Error('Path outside publish root');
      const mime = { js:'text/javascript', mjs:'text/javascript', wasm:'application/wasm', html:'text/html', css:'text/css', json:'application/json', ttf:'font/ttf' };
      try { body = await readFile(file); response = {ok:true,status:200}; }
      catch { body = Buffer.from('Not found'); response = {ok:false,status:404}; }
      response.headers = {get: () => mime[file.split('.').at(-1)] || 'application/octet-stream'};
    }
    if (response.ok) assets.set(req.url, { sha256: createHash('sha256').update(body).digest('hex'), bytes: body.length });
    res.writeHead(response.status, { 'Content-Type': response.headers.get('content-type') || 'application/octet-stream',
      'Cross-Origin-Opener-Policy': 'same-origin', 'Cross-Origin-Embedder-Policy': 'require-corp', 'Cache-Control': 'no-store' });
    res.end(body);
  } catch (error) { res.writeHead(502); res.end(String(error)); }
});
await new Promise(r => proxy.listen(0, '127.0.0.1', r));
const chromePath = process.env.DOROTI_CHROME ?? (process.platform === 'darwin'
  ? '/Applications/Google Chrome.app/Contents/MacOS/Google Chrome'
  : 'C:/Program Files/Google/Chrome/Application/chrome.exe');
const chrome = spawn(chromePath, [
  '--no-first-run', '--no-default-browser-check', '--remote-debugging-port=0', '--remote-allow-origins=*',
  '--window-size=1300,1000', '--user-data-dir=' + join(out, 'chrome-profile'), 'about:blank',
], { windowsHide: true, stdio: ['ignore', 'ignore', 'pipe'] });
const chromeLog = [];
chrome.stderr.on('data', b => chromeLog.push(b.toString()));
let chromeError;
chrome.on('error', error => { chromeError = error; });
const wait = ms => new Promise(r => setTimeout(r, ms));
let socket;
try {
  let port;
  for (let i = 0; i < 100; i++) {
    if (chromeError) throw chromeError;
    try { port = (await readFile(join(out, 'chrome-profile', 'DevToolsActivePort'), 'utf8')).split('\n')[0]; } catch {}
    if (port) break;
    await wait(100);
  }
  if (!port) throw Error('Chrome did not start');
  const version = await (await fetch(`http://127.0.0.1:${port}/json/version`)).json();
  socket = new WebSocket(version.webSocketDebuggerUrl);
  await new Promise((r, j) => { socket.onopen = r; socket.onerror = j; });
  const pending = new Map(); let next = 0;
  socket.onmessage = event => {
    const m = JSON.parse(event.data);
    if (m.id) { const p = pending.get(m.id); pending.delete(m.id); m.error ? p?.reject(Error(JSON.stringify(m.error))) : p?.resolve(m.result); }
    else if (['Runtime.exceptionThrown', 'Log.entryAdded', 'Runtime.consoleAPICalled'].includes(m.method)) events.push(m);
  };
  const cdp = (method, params = {}, sessionId) => new Promise((resolve, reject) => {
    const id = ++next;
    const timer = setTimeout(() => { pending.delete(id); reject(Error('CDP timeout: ' + method + ' ' + JSON.stringify(params))); }, 30000);
    pending.set(id, { resolve: value => { clearTimeout(timer); resolve(value); }, reject: e => { clearTimeout(timer); reject(e); } });
    socket.send(JSON.stringify({ id, method, params, ...(sessionId ? { sessionId } : {}) }));
  });
  const target = (await cdp('Target.getTargets')).targetInfos.find(t => t.type === 'page' && t.url === 'about:blank');
  const page = (await cdp('Target.attachToTarget', { targetId: target.targetId, flatten: true })).sessionId;
  const evaluate = async (expression, session = page) => {
    const x = await cdp('Runtime.evaluate', { expression, returnByValue: true, awaitPromise: true }, session);
    if (x.exceptionDetails) throw Error(JSON.stringify(x.exceptionDetails));
    return x.result.value;
  };
  await cdp('Runtime.enable', {}, page);
  await cdp('Page.enable', {}, page);
  const mobile = process.env.DOROTI_MEMORY_MOBILE === '1';
  if (mobile) await cdp('Emulation.setUserAgentOverride', {
    userAgent: 'Mozilla/5.0 (iPhone; CPU iPhone OS 26_6_1 like Mac OS X) AppleWebKit/605.1.15 CriOS/150.0 Mobile/15E148 Safari/604.1',
    platform: 'iPhone' }, page);
  await cdp('Emulation.setDeviceMetricsOverride', { width: +width, height: 900, deviceScaleFactor: +dpr, mobile: false }, page);
  const query = new URLSearchParams({ dorotiTestbedMode: 'sample' });
  if (backend !== 'auto') query.set('dorotiRenderer', backend);
  if (!['off', 'memory-off', 'memory-soak-off'].includes(profile)) query.set('dorotiFrameCost', '1');
  if (['diagnosis', 'inspect'].includes(profile)) {
    query.set('dorotiResizeDiagnostics', '1'); query.set('dorotiLayoutProfile', '1'); query.set('dorotiAllocationProfile', '1');
  }
  const url = `http://127.0.0.1:${proxy.address().port}/?${query}`;
  await cdp('Page.navigate', { url }, page);
  await cdp('Page.bringToFront', {}, page);
  for (let i = 0; ; i++) {
    const state = await evaluate(`({ready:document.documentElement.dataset.dorotiBootstrapStage==='started',error:document.documentElement.dataset.dorotiRendererError,text:document.body.innerText})`);
    if (state.error || state.text.includes('Doroti failed to start')) throw Error(JSON.stringify(state));
    if (state.ready) break;
    if (i >= 600) throw Error('Startup timeout');
    await wait(200);
  }
  await wait(1500);
  const worker = !['off', 'memory-off', 'memory-soak-off'].includes(profile);
  const shot = async name => {
    const x = await cdp('Page.captureScreenshot', { format: 'png' }, page);
    await writeFile(join(out, name + '.png'), Buffer.from(x.data, 'base64'));
  };
  const dom = () => evaluate(`Array.from(document.querySelectorAll('[role],[aria-label]')).map(e=>({role:e.getAttribute('role'),label:e.getAttribute('aria-label'),description:e.getAttribute('aria-description'),text:e.textContent,rect:(()=>{const r=e.getBoundingClientRect();return [r.x,r.y,r.width,r.height]})()}))`);
  const diagnostics = () => worker ? evaluate('__dorotiFrameCost("diagnostics")') : null;
  const segment = async (id, action) => {
    const before = await diagnostics();
    if (worker) await evaluate('__dorotiFrameCost("reset")');
    const processBefore = await cdp('SystemInfo.getProcessInfo');
    const start = Date.now();
    if (profile.startsWith('memory')) await save('progress', { id, start, phase: 'started', inputs: input.length });
    await action();
    const end = Date.now();
    const processAfter = await cdp('SystemInfo.getProcessInfo');
    const finished = worker ? await evaluate('__dorotiFrameCost("finish")') : null;
    const trace = finished?.trace ?? null;
    const after = finished?.after ?? null;
    const content = await dom();
    const main = profile.startsWith('memory') ? await evaluate(`({
      policy:{...document.documentElement.dataset},
      canvas:{...document.querySelector('canvas')?.dataset},
      viewport:{width:innerWidth,height:innerHeight,dpr:devicePixelRatio},visibility:document.visibilityState,focused:document.hasFocus()})`) : null;
    segments.push({ id, start, end, before, after, trace, content, main, processBefore, processAfter });
    await save('segments', segments);
    if (profile.startsWith('memory')) await save('progress', { id, end, phase: 'completed', inputs: input.length });
    await shot(id);
    console.log(id + ': ' + (trace?.rows.length ?? 'OFF') + ' numeric records');
  };
  const wheel = async (deltaY, x = +width * .3, y = 650) => {
    input.push({ kind: 'wheel', deltaY, x, y, at: Date.now() });
    await cdp('Input.dispatchMouseEvent', { type: 'mouseWheel', x, y, deltaX: 0, deltaY }, page);
    if (profile.startsWith('memory')) await save('input', input);
    await wait(180);
  };
  const clickLabel = async text => {
    const nodes = await dom();
    const node = nodes.find(n => (n.label?.includes(text)||n.description?.includes(text)) && n.rect[1] >= 0 && n.rect[1]+n.rect[3]/2 < 900 && n.rect[2] > 0);
    if (!node) { await shot('missing-label'); throw Error('Visible label missing: '+text); }
    const [x,y,w,h] = node.rect;
    for (const type of ['mousePressed','mouseReleased']) await cdp('Input.dispatchMouseEvent', {type,x:x+w/2,y:y+h/2,button:'left',clickCount:1},page);
    await wait(400);
  };
  await save('environment', { version, url, profile, backend, width: +width, height: 900, dpr: +dpr,
    browser: await evaluate(`({policy:{...document.documentElement.dataset},ua:navigator.userAgent,visibility:document.visibilityState,focus:document.hasFocus(),dpr:devicePixelRatio,runtime:getDotnetRuntime(0).runtimeBuildInfo,config:getDotnetRuntime(0).getConfig()})`),
    gpu: await cdp('SystemInfo.getInfo') });
  await save('initial-dom', await dom()); await shot('initial');
  await segment('S0', () => wait(profile === 'inspect' ? 1000 : 2500));
  if (profile.startsWith('memory')) {
    const { runMemoryWorkload } = await import('../web-memory/workload.mjs');
    await runMemoryWorkload({ segment, wheel, clickLabel, wait, dom, save, shot, evaluate,
      resize: async (w, h) => {
        await cdp('Emulation.setDeviceMetricsOverride', { width: w, height: h, deviceScaleFactor: +dpr, mobile: false }, page);
      }, width: +width, profile });
  } else if (profile === 'smoke') {
    await clickLabel('Color');
    await clickLabel('Components');
    await clickLabel('Toggle brightness');
    await shot('light-theme');
    await clickLabel('Toggle brightness');
    let textbox;
    for (let i=0;i<80;i++) {
      textbox = (await dom()).find(n=>n.role==='textbox' && n.rect[1]>=56 && n.rect[1]+n.rect[3]/2<880 && n.rect[2]>0);
      if (textbox) break;
      await wheel(180, +width>1000 ? +width*.75 : +width*.5);
    }
    if (!textbox) throw Error('Visible text input was not reached');
    const [x,y,w,h]=textbox.rect;
    for (const type of ['mousePressed','mouseReleased']) await cdp('Input.dispatchMouseEvent',{type,x:x+w/2,y:y+h/2,button:'left',clickCount:1},page);
    for (let i=0;i<20;i++) {
      if (await evaluate('document.activeElement?.matches("input,textarea")')) break;
      await wait(100);
    }
    await save('focus-before', await evaluate('({tag:document.activeElement?.tagName,value:document.activeElement?.value,html:document.activeElement?.outerHTML})'));
    await cdp('Input.insertText',{text:'frame-cost'},page);
    await wait(500);
    await save('focus-after', await evaluate('({tag:document.activeElement?.tagName,value:document.activeElement?.value,html:document.activeElement?.outerHTML})'));
    await shot('text-input');
    const editing = await evaluate('Array.from(document.querySelectorAll("[role=textbox]")).map(e=>({value:e.value,label:e.getAttribute("aria-label")}))');
    await save('editing', editing);
    if (!editing.some(e=>e.value === 'frame-cost')) throw Error('Semantics text input did not retain text');
    await segment('text-focus',()=>wait(500));
  } else if (profile !== 'inspect') {
    await segment('S1', async () => { for (let i = 0; i < 12; i++) await wheel(320); await wait(1000); });
    await segment('S2', async () => { for (let i = 0; i < 12; i++) await wheel(-320); for (let i = 0; i < 12; i++) await wheel(320); await wait(1000); });
    // Preparation is outside the timed interval and every delivered wheel is logged.
    for (let i=0;i<12;i++) await wheel(-320);
    for (let i=0;i<24;i++) {
      const nodes=await dom();
      if(nodes.some(n=>(n.label?.includes('Start progress')||n.description?.includes('Start progress'))&&n.rect[1]>=56&&n.rect[1]<800)) break;
      await wheel(60);
    }
    await wait(800);
    await save('progress-dom', await dom());
    await clickLabel('Start progress');
    await segment('S3', () => wait(10000));
    await clickLabel('Stop progress');
    await segment('S4', async () => { for (let i = 0; i < 12; i++) await wheel(-320); await clickLabel('Filled'); await wait(1000); });
    await clickLabel('Color\\n'.replace('\\n','\n'));
    await clickLabel('Components\\n'.replace('\\n','\n'));
    await segment('S5', () => wait(10000));
  }
  const rendererError = await evaluate('document.documentElement.dataset.dorotiRendererError');
  if (rendererError || events.some(e => e.method === 'Runtime.exceptionThrown'))
    throw Error('Runtime regression: ' + (rendererError || 'unhandled page exception'));
  await save('result', { status: 'OBSERVED', segments: segments.map(s => s.id), physicalLatency: 'notVerified' });
  await cdp('Browser.close');
} catch (error) {
  await save('failure', { message: String(error), stack: error.stack });
  throw error;
} finally {
  socket?.close(); chrome.kill(); proxy.closeAllConnections(); proxy.close();
  await save('input', input);
  await save('events', events); await save('assets', Object.fromEntries(assets));
  await writeFile(join(out, 'chrome.log'), chromeLog.join(''));
}
