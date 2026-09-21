
// Run with the 20-minute timeout wrapper and a new evidence directory.
// Start the Web dev server first; its printed URL is the optional third argument.
import {createServer} from 'node:http';
import {spawn} from 'node:child_process';
import {mkdir,writeFile,readFile} from 'node:fs/promises';
import {join,resolve} from 'node:path';
const mode=process.argv[4]??'worker-direct-webgpu';
const upstream = process.argv[3] ?? 'http://127.0.0.1:5000';
const out=resolve(process.argv[2]); await mkdir(out,{recursive:true});
const proxy=createServer(async(req,res)=>{try {if(req.url.startsWith('/__webview_test')){res.writeHead(200,{'Content-Type':'text/html','Cross-Origin-Opener-Policy':'same-origin','Cross-Origin-Embedder-Policy':'require-corp','Cross-Origin-Resource-Policy':'cross-origin'});res.end('<!doctype html><title>Browser content</title><input value="same origin"><style>body{background:#abc}</style>');return;}const r=await fetch(upstream+req.url);const body=Buffer.from(await r.arrayBuffer());res.writeHead(r.status,{'Content-Type':r.headers.get('content-type')||'application/octet-stream','Cross-Origin-Opener-Policy':'same-origin','Cross-Origin-Embedder-Policy':'require-corp','Cross-Origin-Resource-Policy':'same-origin','Cache-Control':'no-store'});res.end(body);}catch(e){res.writeHead(502);res.end(String(e));}});
await new Promise(r=>proxy.listen(0,'127.0.0.1',r));
const chrome=spawn('C:/Program Files/Google/Chrome/Application/chrome.exe',['--headless=new','--no-first-run','--no-default-browser-check','--remote-debugging-port=0','--remote-allow-origins=*','--enable-unsafe-webgpu','--window-size=1000,800','--user-data-dir='+join(out,'chrome-profile'),'about:blank'],{windowsHide:true,stdio:['ignore','ignore','pipe']});
const errors=[];chrome.stderr.on('data',b=>errors.push(b.toString()));
const wait=ms=>new Promise(r=>setTimeout(r,ms));let socket;
try{
 let port;for(let n=0;n<100;n++){try{port=(await readFile(join(out,'chrome-profile','DevToolsActivePort'),'utf8')).split('\n')[0];if(port)break;}catch{}await wait(100);}
 if(!port)throw Error('Chrome did not start');
 const tabs=await(await fetch('http://127.0.0.1:'+port+'/json')).json();
 socket=new WebSocket(tabs.find(t=>t.type==='page' && t.url==='about:blank').webSocketDebuggerUrl);await new Promise((resolve,reject)=>{socket.onopen=resolve;socket.onerror=reject;});
 let next=0;const pending=new Map(),events=[];
 socket.onmessage=ev=>{const m=JSON.parse(ev.data);if(m.id){const p=pending.get(m.id);pending.delete(m.id);m.error?p.reject(Error(JSON.stringify(m.error))):p.resolve(m.result);}else if(['Runtime.exceptionThrown','Runtime.consoleAPICalled','Log.entryAdded'].includes(m.method)){events.push(m);void writeFile(join(out,'events-live.json'),JSON.stringify(events,null,2));}};
 const cdp=(method,params={})=>new Promise((resolve,reject)=>{const id=++next;const timer=setTimeout(()=>{pending.delete(id);reject(Error('CDP timeout: '+method));},15000);pending.set(id,{resolve:v=>{clearTimeout(timer);resolve(v)},reject:e=>{clearTimeout(timer);reject(e)}});socket.send(JSON.stringify({id,method,params}));});
 const evaluate=async(expression)=>{const x=await cdp('Runtime.evaluate',{expression,returnByValue:true,awaitPromise:true});if(x.exceptionDetails)throw Error(JSON.stringify(x.exceptionDetails));return x.result.value;};


 await cdp('Page.enable');await cdp('Runtime.enable');await cdp('Log.enable');
 try {
  const until=async(fn)=>{for(let i=0;i<30;i++){if(await fn())return;await wait(500);}throw Error('Panel readiness timeout');};
  const check=(condition,message)=>{if(!condition)throw Error(message);};
  await cdp('Emulation.setDeviceMetricsOverride',{width:1000,height:800,deviceScaleFactor:1,mobile:false});
  await cdp('Page.navigate',{url:`http://127.0.0.1:${proxy.address().port}/?dorotiTestbedMode=sample&dorotiRenderer=${mode}&dorotiResizeDiagnostics=1`});
  await until(()=>evaluate(`document.querySelector('.doroti-root')?.dataset.dorotiWorkerRuntime==='ready'&&JSON.parse(__dorotiResizeDiagnostics.presenter('doroti-surface')).frontRequestId>0`));
  check(await evaluate(`JSON.parse(__dorotiResizeDiagnostics.presenter('doroti-surface')).mode===${JSON.stringify(mode)}`),'Requested renderer must be active');
  const mouse=(type,x,y,buttons=1)=>cdp('Input.dispatchMouseEvent',{type,x,y,button:type==='mouseMoved'&&!buttons?'none':'left',buttons,clickCount:type==='mouseMoved'?0:1});
  await wait(800);await mouse('mousePressed',917,760);await mouse('mouseReleased',917,760,0);
  await until(()=>evaluate(`document.querySelector('[data-doroti-input-shield]')?.getBoundingClientRect().width>0`));
  await wait(1500);
  await evaluate(`window.originalFrame=document.querySelector('iframe');window.originalRasterNodes=[...document.querySelectorAll('[data-doroti-raster]')];`);
  const bounds=()=>evaluate(`(()=>{const r=document.querySelector('[data-doroti-input-shield]').getBoundingClientRect();return {x:r.x,y:r.y,width:r.width,height:r.height}})()`);
  const snapshot=()=>evaluate(`(()=>{const root=document.querySelector('.doroti-root');return {bytes:Number(root.dataset.dorotiPlatformTotalBytes),commits:Number(root.dataset.dorotiPlatformCommits),residentBytes:Number(root.dataset.dorotiPlatformResidentBytes),rasters:[...document.querySelectorAll('[data-doroti-raster]')].map(c=>({order:c.dataset.dorotiRaster,width:c.width,height:c.height,x:c.getBoundingClientRect().x,y:c.getBoundingClientRect().y,retained:originalRasterNodes.includes(c)})),iframePreserved:originalFrame===document.querySelector('iframe'),error:document.documentElement.dataset.dorotiRendererError}})()`);
  const initial=await bounds();
  await writeFile(join(out,'before.png'),Buffer.from((await cdp('Page.captureScreenshot')).data,'base64'));
  const measurements=[];
  for(const direction of [1,-1]){
   const start=await bounds();let x=start.x+100,y=start.y+24;
   await mouse('mouseMoved',x,y,0);await mouse('mousePressed',x,y);
   x+=direction*24;await mouse('mouseMoved',x,y);await wait(250);
   await evaluate(`window.originalRasterNodes=[...document.querySelectorAll('[data-doroti-raster]')];`);
   const before=await snapshot();
   await evaluate(`__dorotiResizeDiagnostics.reset(1);window.dragCommitTimes=[];window.dragObserver=new MutationObserver(records=>{if(records.some(r=>r.attributeName==='data-doroti-platform-commits'))dragCommitTimes.push(performance.now())});dragObserver.observe(document.querySelector('.doroti-root'),{attributes:true,attributeFilter:['data-doroti-platform-commits']})`);
   const started=performance.now();
   for(let step=0;step<24;step++){x+=direction*10;y+=direction*3;await mouse('mouseMoved',x,y);await wait(17);}
   await wait(350);
   const elapsed=performance.now()-started;
   const after=await snapshot(),end=await bounds();
   const timing=await evaluate(`(()=>{dragObserver.disconnect();const trace=JSON.parse(__dorotiResizeDiagnostics.trace(1));const samples=trace.filter(x=>x.phase==='managed-raster-end').map(x=>x.durationMicroseconds/1000).sort((a,b)=>a-b);const intervals=dragCommitTimes.slice(1).map((t,i)=>t-dragCommitTimes[i]).sort((a,b)=>a-b);const p=(a,q)=>a.length?a[Math.floor((a.length-1)*q)]:null;return {rasterMs:{samples:samples.length,p50:p(samples,.5),p95:p(samples,.95)},domCommitIntervalMs:{samples:intervals.length,p50:p(intervals,.5),p95:p(intervals,.95)}}})()`);
   check(direction*(end.x-start.x)>180,'Panel must move in the requested direction, including a second drag from its new bounds');
   check(after.iframePreserved&&!after.error,'Iframe identity / renderer health');
   if(process.argv.includes('--expect-retained')){
    check(after.bytes===before.bytes,'Steady panel translation must not upload pixels');
    check(after.rasters.every(r=>r.retained),'Steady panel translation must preserve raster canvases');
   }
   measurements.push({direction,start,end,uploadBytes:after.bytes-before.bytes,commits:after.commits-before.commits,elapsedMs:elapsed,...timing,after});
   await mouse('mouseReleased',x,y,0);await wait(300);
  }
  await writeFile(join(out,'after.png'),Buffer.from((await cdp('Page.captureScreenshot')).data,'base64'));
  let highDpr;
  if(process.argv.includes('--expect-retained')){
   // Two full 3000x2700 rasters fit the original 64 MiB budget; independently
   // splitting their overlapping foreground would exceed it.
   await cdp('Emulation.setDeviceMetricsOverride',{width:1000,height:900,deviceScaleFactor:3,mobile:false});
   await until(()=>evaluate(`document.querySelector('[data-doroti-raster="0"]')?.width===3000&&document.querySelector('[data-doroti-raster="0"]')?.height===2700`));
   await wait(500);highDpr=await snapshot();
   check(highDpr.iframePreserved&&!highDpr.error&&highDpr.rasters.length===2&&highDpr.residentBytes<=64*1024*1024,'High DPR preserves the original memory budget through unsplit fallback');
   await cdp('Emulation.setDeviceMetricsOverride',{width:1000,height:800,deviceScaleFactor:1,mobile:false});
   await until(()=>evaluate(`document.querySelector('[data-doroti-raster="0"]')?.width===1000&&document.querySelectorAll('[data-doroti-raster]').length>2`));
  }
  const result={status:'PASS',renderer:mode,initial,measurements,highDpr,physicalInputLatency:'notVerified',note:'CDP input, managed raster timing and DOM acceptance intervals; not monitor presentation timing.'};
  await writeFile(join(out,'result.json'),JSON.stringify(result,null,2));console.log(JSON.stringify(result));
 } finally {
  await writeFile(join(out,'events.json'),JSON.stringify(events,null,2));
  await writeFile(join(out,'final.png'),Buffer.from((await cdp('Page.captureScreenshot')).data,'base64'));
 }
 await cdp('Browser.close');
} finally {if(socket)socket.close();chrome.kill();proxy.close();await writeFile(join(out,'chrome.log'),errors.join(''));}
