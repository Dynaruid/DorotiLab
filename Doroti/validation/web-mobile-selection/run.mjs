// Run through ../run-with-timeout.py. One fresh browser per invocation; no retries.
import { createServer } from 'node:http';
import { createHash } from 'node:crypto';
import { spawn } from 'node:child_process';
import { access, mkdir, readFile, writeFile } from 'node:fs/promises';
import { resolve, join } from 'node:path';

const [directory, upstream, profile = 'android', backend = 'worker-direct-webgl', width = '390', dpr = '1'] = process.argv.slice(2);
if (!directory || !upstream) throw Error('run.mjs OUTPUT URL_OR_WWWROOT [android|iphone|ipad|desktop] [backend] [width] [dpr]');
const out = resolve(directory);
if (await access(join(out, 'chrome-profile')).then(() => true, () => false)) throw Error('Use a fresh output directory');
await mkdir(out, { recursive: true });
const assets = new Map(), events = [];
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
  '--headless=new',
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
  const mobile = profile !== 'desktop';
  const frameworkSelection = profile === 'android';
  const nativeIosSelection = profile === 'iphone' || profile === 'ipad';
  const devices = {
    android: {userAgent:'Mozilla/5.0 (Linux; Android 14; Pixel 8) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Mobile Safari/537.36', platform:'Linux armv8l'},
    iphone: {userAgent:'Mozilla/5.0 (iPhone; CPU iPhone OS 18_0 like Mac OS X) AppleWebKit/605.1.15 Version/18.0 Mobile/15E148 Safari/604.1', platform:'iPhone'},
    ipad: {userAgent:'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15) AppleWebKit/605.1.15 Version/18.0 Safari/605.1.15', platform:'MacIntel'},
  };
  if (mobile) {
    if (!devices[profile]) throw Error('Unknown device profile');
    await cdp('Emulation.setUserAgentOverride', devices[profile], page);
    await cdp('Emulation.setTouchEmulationEnabled', {enabled:true,maxTouchPoints:5}, page);
  }
  await cdp('Emulation.setDeviceMetricsOverride', { width: +width, height: 900, deviceScaleFactor: +dpr, mobile }, page);
  const query = new URLSearchParams({ dorotiTestbedMode:'sample',dorotiRenderer:backend });
  const url = `http://127.0.0.1:${proxy.address().port}/?${query}`;
  await cdp('Browser.grantPermissions', {origin:new URL(url).origin,permissions:['clipboardReadWrite','clipboardSanitizedWrite']});
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
  const shot = async name => {
    const x = await cdp('Page.captureScreenshot', { format: 'png' }, page);
    await writeFile(join(out, name + '.png'), Buffer.from(x.data, 'base64'));
  };
  const dom = () => evaluate(`Array.from(document.querySelectorAll('[role],[aria-label]')).map(e=>({role:e.getAttribute('role'),label:e.getAttribute('aria-label'),description:e.getAttribute('aria-description'),text:e.textContent,rect:(()=>{const r=e.getBoundingClientRect();return [r.x,r.y,r.width,r.height]})()}))`);
  const checks = [];
  const check = (ok, name) => { if (!ok) throw Error(name); checks.push(name); };
  const tap = async (x, y, hold = 60) => {
    if (mobile) {
      await cdp('Input.dispatchTouchEvent', {type:'touchStart',touchPoints:[{x,y}]}, page);
      await wait(hold);
      await cdp('Input.dispatchTouchEvent', {type:'touchEnd',touchPoints:[]}, page);
    } else {
      for (const type of ['mousePressed','mouseReleased'])
        await cdp('Input.dispatchMouseEvent', {type,x,y,button:'left',clickCount:1}, page);
    }
    await wait(450);
  };
  await save('environment', {version,profile,backend,ua:await evaluate('navigator.userAgent')});
  const policy = await evaluate(`document.querySelector('.doroti-root').dataset.dorotiTextSelection`);
  check(policy === (frameworkSelection ? 'framework' : 'browser'), 'correct mobile/desktop selection owner');
  let textbox;
  for (let i = 0; i < 30; i++) {
    textbox = (await dom()).find(n => n.role === 'textbox' && /Filled|Outlined/.test(n.label ?? '') && n.rect[1] >= 60 && n.rect[1] + n.rect[3] < 800 && n.rect[2] > 0);
    if (textbox) break;
    // Use the section gutter so nested drawers/carousels do not consume scroll.
    const scrollX = +width > 1000 ? +width*.75 : 12;
    await cdp('Input.dispatchMouseEvent', {type:'mouseWheel',x:scrollX,y:650,deltaX:0,deltaY:650}, page);
    await wait(220);
  }
  await save('text-field-dom', await dom());
  if (!textbox) throw Error('No visible TextField');
  const [x,y,w,h] = textbox.rect;
  await tap(x+w/2,y+h/2);
  check(await evaluate('document.activeElement?.matches("input,textarea")'), 'native editing endpoint focused');
  await cdp('Input.insertText', {text:'mobile selection'}, page);
  await wait(500);
  check(await evaluate(`document.querySelector('#doroti-ime').value === 'mobile selection'`), 'text input retained');
  const styles = await evaluate(`(()=>{
    const nodes=[document.querySelector('canvas'),document.querySelector('.doroti-semantics'),
      document.querySelector('.doroti-semantics [data-doroti-semantics-id]:not(input):not(textarea)'),
      document.querySelector('#doroti-ime'),document.querySelector('[role=textbox]')];
    return nodes.map(e=>({tag:e?.tagName,select:e&&getComputedStyle(e).userSelect,hidden:e?.getAttribute('aria-hidden')}));
  })()`);
  await save('styles', styles);
  check(styles.slice(0,3).every(s=>s.select==='none'), 'canvas and non-editable accessibility DOM are unselectable');
  check(styles.slice(3).every(s=>s.select==='text'), 'native editing endpoints remain selectable');
  const editablePaint = await evaluate(`(()=>{
    const e=document.querySelector('#doroti-ime'),s=getComputedStyle(e),selection=getComputedStyle(e,'::selection');
    return {filter:s.filter,opacity:s.opacity,caret:s.caretColor,
      color:s.color,fill:s.webkitTextFillColor,selection:selection.backgroundColor};
  })()`);
  await save('editable-paint', editablePaint);
  check(editablePaint.filter==='opacity(0)' && editablePaint.caret==='rgba(0, 0, 0, 0)', 'DOM caret, handles and selection paint remain hidden');
  check(styles[1].hidden !== 'true', 'accessibility tree remains exposed');
  const accessibility = await cdp('Accessibility.getFullAXTree', {}, page);
  check(accessibility.nodes.some(n=>!n.ignored && n.role?.value==='textbox'), 'browser accessibility tree contains an exposed textbox');
  const menuPrevented = await evaluate(`(()=>{const e=new MouseEvent('contextmenu',{bubbles:true,cancelable:true});document.querySelector('#doroti-ime').dispatchEvent(e);return e.defaultPrevented})()`);
  check(menuPrevented === frameworkSelection, 'native context menu ownership');
  if (nativeIosSelection) {
    // The decoration can extend beyond the DOM editable. Exercise a framework
    // gesture there as well, so the toolbar guard is tested independently.
    await tap(x+24,y+h/2,850);
    check(!(await dom()).some(n => n.role==='button' && /^(Copy|Cut|Paste|Select All)$/i.test(n.label ?? n.text ?? '')), 'iOS framework gesture does not add a toolbar');
    await evaluate(`document.addEventListener('pointerdown',e=>window.__selectionPointer={trusted:e.isTrusted,prevented:e.defaultPrevented,target:e.target.id})`);
    const editableRect = await evaluate(`document.querySelector('#doroti-ime').getBoundingClientRect().toJSON()`);
    await tap(editableRect.x + Math.min(24, editableRect.width/2), editableRect.y + editableRect.height/2, 850);
    await save('native-gesture', await evaluate(`({pointer:window.__selectionPointer,active:document.activeElement?.id,inputRect:document.querySelector('#doroti-ime').getBoundingClientRect().toJSON()})`));
    check(await evaluate(`window.__selectionPointer?.trusted && window.__selectionPointer.prevented && window.__selectionPointer.target==='doroti-ime'`), 'iOS editable gesture reaches framework selection');
    check(!(await dom()).some(n => n.role==='button' && /^(Copy|Cut|Paste|Select All)$/i.test(n.label ?? n.text ?? '')), 'iOS long press does not add a framework toolbar');
    await shot('canvas-ios-caret');
    const wordPoint = {x:editableRect.x + Math.min(24, editableRect.width/2),y:editableRect.y + editableRect.height/2};
    for (let i=0;i<2;i++) {
      await cdp('Input.dispatchTouchEvent', {type:'touchStart',touchPoints:[wordPoint]}, page);
      await wait(50);
      await cdp('Input.dispatchTouchEvent', {type:'touchEnd',touchPoints:[]}, page);
      await wait(60);
    }
    await wait(400);
    check(await evaluate(`(()=>{const e=document.querySelector('#doroti-ime');return e.selectionStart===0 && e.selectionEnd===6})()`), 'framework double tap selects the painted word');
    await shot('canvas-ios-handles');
    // Approximate the padded handle hit area, then validate the resulting
    // framework selection; DOM metrics are not an alignment oracle.
    const handlePoint = await evaluate(`(()=>{
      const e=document.querySelector('#doroti-ime'),r=e.getBoundingClientRect(),s=getComputedStyle(e);
      const c=document.createElement('canvas').getContext('2d');c.font=s.font;
      return {x:r.x+c.measureText('mobile').width,y:r.bottom+6};
    })()`);
    await cdp('Input.dispatchTouchEvent', {type:'touchStart',touchPoints:[handlePoint]}, page);
    for (let i=1;i<=6;i++) {
      await cdp('Input.dispatchTouchEvent', {type:'touchMove',touchPoints:[{x:handlePoint.x+i*10,y:handlePoint.y}]}, page);
      await wait(40);
    }
    await cdp('Input.dispatchTouchEvent', {type:'touchEnd',touchPoints:[]}, page);
    await wait(400);
    check(await evaluate(`(()=>{const e=document.querySelector('#doroti-ime');return e.selectionStart===0 && e.selectionEnd>6})()`), 'canvas end-handle drag extends the selection');
    check(!(await dom()).some(n => n.role==='button' && /^(Copy|Cut|Paste|Select All)$/i.test(n.label ?? n.text ?? '')), 'handle drag does not add a framework toolbar');
    await shot('canvas-ios-handle-drag');
    // Chromium emulation cannot show UIKit UI. Exercise the native endpoint's
    // selection/edit events and verify the managed semantics acknowledge them.
    await evaluate(`document.querySelector('#doroti-ime').setSelectionRange(0,6)`);
    await wait(450);
    check(await evaluate(`Array.from(document.querySelectorAll('[role=textbox]')).some(e=>e.value==='mobile selection' && e.selectionStart===0 && e.selectionEnd===6)`), 'native selection propagates to framework semantics');
    await shot('native-ios-range-selection');
    await cdp('Input.insertText', {text:'native'}, page);
    await wait(450);
    check(await evaluate(`document.querySelector('#doroti-ime').value==='native selection' && Array.from(document.querySelectorAll('[role=textbox]')).some(e=>e.value==='native selection')`), 'native replacement updates text and semantics');
    await shot('native-ios-selection');
  } else if (frameworkSelection) {
    // A long press must reach Android's framework selection recognizer.
    await tap(x+24,y+h/2,850);
    await save('toolbar-dom', await dom());
    await shot('toolbar');
    const copy = (await dom()).find(n => n.role==='button' && /^(Copy|COPY)$/.test(n.label ?? n.text ?? ''));
    const cut = (await dom()).find(n => n.role==='button' && /^(Cut|CUT)$/.test(n.label ?? n.text ?? ''));
    check(!!copy && !!cut, 'selected text displays rendered Copy and Cut buttons');
    const selection = await evaluate(`(()=>{const e=document.querySelector('#doroti-ime');return {start:e.selectionStart,end:e.selectionEnd,value:e.value}})()`);
    check(selection.end > selection.start, 'framework selection reaches native endpoint');
    await save('selection', selection);
    await tap(cut.rect[0]+cut.rect[2]/2,cut.rect[1]+cut.rect[3]/2);
    const expected = selection.value.slice(0,selection.start)+selection.value.slice(selection.end);
    check(await evaluate(`document.querySelector('#doroti-ime').value`) === expected, 'rendered Cut updates native text');
    check(await evaluate(`Array.from(document.querySelectorAll('[role=textbox]')).some(e=>e.value===${JSON.stringify(expected)})`), 'Cut propagates to semantics');
    const clipboard = await evaluate('navigator.clipboard.readText()');
    check(clipboard === selection.value.slice(selection.start,selection.end), 'Cut writes selected text to clipboard');
    check(await evaluate('getSelection().toString()') === '', 'touch selection does not select the HTML document');
    await shot('after-cut');
    await tap(x+24,y+h/2,850);
    const paste = (await dom()).find(n=>n.role==='button' && /^Paste$/i.test(n.label ?? n.text ?? ''));
    check(!!paste, 'rendered Paste is available');
    const beforePaste = await evaluate(`(()=>{const e=document.querySelector('#doroti-ime');return {start:e.selectionStart,end:e.selectionEnd,value:e.value}})()`);
    await tap(paste.rect[0]+paste.rect[2]/2,paste.rect[1]+paste.rect[3]/2);
    const pasted = beforePaste.value.slice(0,beforePaste.start)+clipboard+beforePaste.value.slice(beforePaste.end);
    check(await evaluate(`document.querySelector('#doroti-ime').value`) === pasted, 'rendered Paste replaces the selected text');
    await shot('after-paste');
  } else {
    await evaluate(`document.addEventListener('contextmenu',e=>window.__selectionMenu={trusted:e.isTrusted,prevented:e.defaultPrevented,target:e.target.id})`);
    const editableRect = await evaluate(`document.querySelector('#doroti-ime').getBoundingClientRect().toJSON()`);
    const menuPoint = {x:editableRect.x + Math.min(24, editableRect.width/2),y:editableRect.y + editableRect.height/2};
    await cdp('Input.dispatchMouseEvent', {type:'mousePressed',...menuPoint,button:'right',buttons:2,clickCount:1}, page);
    await cdp('Input.dispatchMouseEvent', {type:'mouseReleased',...menuPoint,button:'right',buttons:0,clickCount:1}, page);
    await wait(400);
    await save('desktop-context-menu', await evaluate(`({menu:window.__selectionMenu,active:document.activeElement?.id,inputRect:document.querySelector('#doroti-ime').getBoundingClientRect().toJSON()})`));
    check(await evaluate('window.__selectionMenu?.trusted && !window.__selectionMenu.prevented'), 'trusted desktop right click retains native menu');
    check(!(await dom()).some(n => n.role==='button' && /^(Copy|COPY|Cut|CUT)$/.test(n.label ?? n.text ?? '')), 'desktop does not add a framework toolbar');
    await shot('desktop');
  }
  check(!await evaluate('document.documentElement.dataset.dorotiRendererError'), 'no renderer failure');
  check(!events.some(e=>e.method==='Runtime.exceptionThrown'), 'no unhandled page exception');
  await save('result', {status:'PASS',profile,backend,checks,physicalMobile:'notVerified'});
  console.log(JSON.stringify({status:'PASS',profile,checks}));
  await cdp('Browser.close');
} catch (error) {
  await save('failure', {message:String(error),stack:error.stack});
  throw error;
} finally {
  socket?.close(); chrome.kill(); proxy.closeAllConnections(); proxy.close();
  await save('events',events); await save('assets',Object.fromEntries(assets));
  await writeFile(join(out,'chrome.log'),chromeLog.join(''));
}
