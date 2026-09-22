
// Run with the 20-minute timeout wrapper and a new evidence directory.
// Start the Web dev server first; its printed URL is the optional third argument.
import {createHash} from 'node:crypto';
import {createServer} from 'node:http';
import {spawn} from 'node:child_process';
import {mkdir,writeFile,readFile} from 'node:fs/promises';
import {join} from 'node:path';
const mode=process.argv[4]??'worker-direct-webgpu';
const upstream = process.argv[3] ?? 'http://127.0.0.1:5000';
const out=process.argv[2]; await mkdir(out,{recursive:true});
let upstreamReady=false;
for(let attempt=0;attempt<30;attempt++){try{const response=await fetch(upstream);if(response.ok){await response.arrayBuffer();upstreamReady=true;break;}}catch{}await new Promise(resolve=>setTimeout(resolve,100));}
if(!upstreamReady)throw Error('Product server is not listening: '+upstream);
const assets=new Map();
const proxy=createServer(async(req,res)=>{try {if(req.url.startsWith('/__texture_cross.png')){res.writeHead(200,{'Content-Type':'image/png','Cross-Origin-Resource-Policy':'cross-origin'});res.end(Buffer.from('iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAIAAACQd1PeAAAADElEQVR4nGP4z8AAAAMBAQDJ/pLvAAAAAElFTkSuQmCC','base64'));return;}if(req.url.startsWith('/__webview_test')){res.writeHead(200,{'Content-Type':'text/html','Cross-Origin-Opener-Policy':'same-origin','Cross-Origin-Embedder-Policy':'require-corp','Cross-Origin-Resource-Policy':'cross-origin'});res.end('<!doctype html><title>Browser content</title><input value="same origin"><style>body{background:#abc}</style>');return;}const r=await fetch(upstream+req.url);const body=Buffer.from(await r.arrayBuffer());if(r.status===200)assets.set(req.url,{sha256:createHash('sha256').update(body).digest('hex'),bytes:body.length});res.writeHead(r.status,{'Content-Type':r.headers.get('content-type')||'application/octet-stream','Cross-Origin-Opener-Policy':'same-origin','Cross-Origin-Embedder-Policy':'require-corp','Cross-Origin-Resource-Policy':'same-origin','Cache-Control':'no-store'});res.end(body);}catch(e){res.writeHead(502);res.end(String(e));}});
await new Promise(r=>proxy.listen(0,'127.0.0.1',r));
const chrome=spawn('C:/Program Files/Google/Chrome/Application/chrome.exe',['--headless=new','--no-first-run','--no-default-browser-check','--remote-debugging-port=0','--remote-allow-origins=*','--enable-unsafe-webgpu','--use-fake-device-for-media-stream','--window-size=1000,800','--user-data-dir='+join(out,'chrome-profile'),'about:blank'],{windowsHide:true,stdio:['ignore','ignore','pipe']});
const errors=[];chrome.stderr.on('data',b=>errors.push(b.toString()));
const wait=ms=>new Promise(r=>setTimeout(r,ms));let socket;
try{
 let port;for(let n=0;n<100;n++){try{port=(await readFile(join(out,'chrome-profile','DevToolsActivePort'),'utf8')).split('\n')[0];if(port)break;}catch{}await wait(100);}
 if(!port)throw Error('Chrome did not start');
 const tabs=await(await fetch('http://127.0.0.1:'+port+'/json')).json();
 socket=new WebSocket(tabs.find(t=>t.type==='page' && t.url==='about:blank').webSocketDebuggerUrl);await new Promise((resolve,reject)=>{socket.onopen=resolve;socket.onerror=reject;});
 let next=0;const pending=new Map(),events=[];
 socket.onmessage=ev=>{const m=JSON.parse(ev.data);if(m.id){const p=pending.get(m.id);pending.delete(m.id);m.error?p.reject(Error(JSON.stringify(m.error))):p.resolve(m.result);}else if(['Runtime.exceptionThrown','Runtime.consoleAPICalled','Log.entryAdded'].includes(m.method)){events.push(m);void writeFile(join(out,'events-live.json'),JSON.stringify(events,null,2));}};
 const cdp=(method,params={},sessionId)=>new Promise((resolve,reject)=>{const id=++next;const timer=setTimeout(()=>{pending.delete(id);reject(Error('CDP timeout: '+method));},15000);pending.set(id,{resolve:v=>{clearTimeout(timer);resolve(v)},reject:e=>{clearTimeout(timer);reject(e)}});socket.send(JSON.stringify({id,method,params,...(sessionId?{sessionId}:{})}));});
 const evaluate=async(expression)=>{const x=await cdp('Runtime.evaluate',{expression,returnByValue:true,awaitPromise:true}).catch(e=>{e.message+=' '+expression;throw e});if(x.exceptionDetails)throw Error(JSON.stringify(x.exceptionDetails));return x.result.value;};

 await cdp('Page.enable');await cdp('Runtime.enable');await cdp('Log.enable');
 const check=(ok,name)=>{if(!ok)throw Error(name);console.log('PASS '+name);tests.push(name)}; const tests=[];
 const until=async(fn,name,timeout=120000)=>{let start=Date.now();while(Date.now()-start<timeout){const x=await fn();if(x)return x;if(await evaluate(`document.documentElement.dataset.dorotiRendererError||document.body.innerText.includes("Doroti failed to start")`))throw Error("Product startup failed");await wait(200);}throw Error('Timeout '+name)};
 const shot=async(name)=>{const x=await cdp('Page.captureScreenshot',{format:'png'});await writeFile(join(out,name+'.png'),Buffer.from(x.data,'base64'));};

 let state;
 try {
  await cdp('Browser.setPermission',{permission:{name:'camera'},setting:'granted',origin:`http://127.0.0.1:${proxy.address().port}`});
  await cdp('Page.navigate',{url:`http://127.0.0.1:${proxy.address().port}/?dorotiTestbedMode=texture-web&dorotiRenderer=${mode}&dorotiResizeDiagnostics=1`});
  await until(()=>evaluate(`document.documentElement.dataset.dorotiBootstrapStage==='started'`),'product ready');
  await until(()=>evaluate(`JSON.parse(__dorotiResizeDiagnostics.presenter('doroti-surface')).frontGeneration>0`),'first product frame');
  await evaluate(`(async()=>{globalThis.probe=(await getDotnetRuntime(0).getAssemblyExports('DorotiTestbedApp.Web.dll')).DorotiTestbedApp.Web.Validation.WebTextureExport;globalThis.tm=await import('./_content/Doroti.Host.Web/doroti.web.textures.js');globalThis.registry=tm.texturesForCanvas();globalThis.sample=await import('./plugins/textures.js');globalThis.canvas=document.createElement('canvas');canvas.width=320;canvas.height=180;globalThis.entry=await registry.registerCanvas(canvas);})()`);
  check(await evaluate(`document.querySelectorAll('video').length===0`),'startup does not start media');
  await evaluate(`probe.Show(entry.textureId,true,0)`); await wait(300);
  await shot('blank');
  await evaluate(`sample.pattern(canvas,0);entry.markFrameAvailable()`);
  await until(()=>evaluate(`registry.diagnostics().then(d=>d.imported>0)`),'first frozen frame imported'); await wait(400);
  await shot('first');
  const builds=await evaluate('probe.Builds()');
  await evaluate(`sample.pattern(canvas,100);entry.markFrameAvailable()`); await wait(500); await shot('frozen');
  check(await evaluate('probe.Builds()')===builds,'frame arrival does not rebuild widgets');
  await evaluate(`probe.Show(entry.textureId,false,0)`); await wait(500); await shot('resumed');
  await evaluate(`probe.Show(entry.textureId,false,1)`); await wait(400); await shot('opacity');
  await evaluate(`probe.Show(entry.textureId,false,2)`); await wait(400); await shot('transform');
  await evaluate(`probe.Show(entry.textureId,false,0);canvas.width=160;canvas.height=90;sample.pattern(canvas,30);entry.markFrameAvailable()`); await wait(500); await shot('resize');
  await evaluate(`canvas.width=320;canvas.height=180;sample.pattern(canvas,0);entry.markFrameAvailable();probe.Show(entry.textureId,false,3)`);
  await until(()=>evaluate(`document.querySelector('iframe')?.contentDocument?.querySelector('input')!=null`),'mixed native view');await wait(500);await shot('mixed-a');
  await evaluate(`globalThis.originalFrame=document.querySelector('iframe');originalFrame.contentDocument.querySelector('input').value='preserved';sample.pattern(canvas,110);entry.markFrameAvailable()`);await wait(600);await shot('mixed-b');
  check(await evaluate(`originalFrame===document.querySelector('iframe')&&originalFrame.contentDocument.querySelector('input').value==='preserved'`),'texture update preserves overlapping native view identity');
  await evaluate(`probe.Show(entry.textureId,false,0)`);await wait(400);
  for(const dpr of [1.5,2]){await cdp('Emulation.setDeviceMetricsOverride',{width:1000,height:800,deviceScaleFactor:dpr,mobile:false});await wait(500);await shot('dpr-'+dpr);}
  await cdp('Emulation.clearDeviceMetricsOverride');await wait(300);
  await cdp('Page.setWebLifecycleState',{state:'frozen'});await wait(100);await cdp('Page.setWebLifecycleState',{state:'active'});await cdp('Page.bringToFront');await cdp('Emulation.setFocusEmulationEnabled',{enabled:true});await until(()=>evaluate(`document.visibilityState==='visible'`),'page visible after lifecycle resume');await evaluate(`entry.markFrameAvailable()`);await wait(200);
  check(!(await evaluate('registry.diagnostics()')).gpu?.failure,'resize and lifecycle resume keep the owning GPU');
  await evaluate(`(async()=>{for(let i=0;i<20;i++){const f=await createImageBitmap(canvas,{premultiplyAlpha:'premultiply'});entry.pushFrame(f);}})()`);
  check(await evaluate(`entry.pendingFrames<=2`),'bounded main transfer and latest candidate');
  await evaluate(`entry.dispose()`); await until(()=>evaluate(`registry.diagnostics().then(d=>d.live===0 && d.pending===0)`),'GPU retirement');
  const retired=await evaluate('registry.diagnostics()');check(retired.received===retired.closed&&retired.imported===retired.retired,'closed sources and retired GPU allocations balance');

  const localId=await evaluate(`probe.LocalCanvas('create')`);
  await evaluate(`probe.Show('${localId}',false,0)`);await wait(500);await shot('local-canvas');
  await evaluate(`probe.LocalCanvas('update')`);await wait(300);await shot('local-canvas-preserved');
  await evaluate(`probe.LocalCanvas('stop')`);
  check(!(await evaluate(`registry.diagnostics()`)).gpu?.failure,'no WebGPU validation error');
  for(const source of ['bitmap','frame','canvas','video']) {
    const id=await evaluate(`sample.select('${source}')`); await evaluate(`probe.Show('${id}',false,0)`); await wait(700); await shot(source+'-a'); await wait(700); await shot(source+'-b');
    if(source==='video'){await evaluate(`sample.select('pause')`);await wait(300);await shot('video-paused-a');await wait(350);await shot('video-paused-b');await evaluate(`sample.select('seek')`);await wait(400);await shot('video-seek');await evaluate(`sample.select('end')`);await until(()=>evaluate(`sample.diagnostics().ended`),'video ended');await evaluate(`sample.select('replace-video')`);await wait(500);await shot('video-replaced');check(await evaluate(`!sample.diagnostics().ended&&sample.diagnostics().time>0`),'video source replacement resumes');}
    await evaluate(`sample.stop()`); await until(()=>evaluate(`registry.diagnostics().then(d=>d.live===0)`),'source cleanup');
  }
  const codecId=await evaluate(`sample.webCodecsExample()`);await evaluate(`probe.Show('${codecId}',false,0)`);await wait(500);await shot('webcodecs');await evaluate(`sample.stop()`);
  await evaluate(`globalThis.cameraStreams=[];const getMedia=navigator.mediaDevices.getUserMedia.bind(navigator.mediaDevices);navigator.mediaDevices.getUserMedia=async c=>{const stream=await getMedia(c);cameraStreams.push(stream);return stream;}`);
  const cameraId=await evaluate(`sample.select('camera')`);await evaluate(`probe.Show('${cameraId}',false,0)`);await wait(1000);await shot('camera');await evaluate(`sample.stop()`);
  check(await evaluate(`cameraStreams.length===1&&cameraStreams.every(s=>s.getTracks().every(t=>t.readyState==='ended'))`),'owned camera stream stops');
  await cdp('Browser.setPermission',{permission:{name:'camera'},setting:'denied',origin:await evaluate('location.origin')});
  const denial=await evaluate(`(async()=>{const state=(await navigator.permissions.query({name:'camera'})).state;try{const e=await registry.startCamera();await e.dispose();return{state,granted:true};}catch(e){return{state,name:e.name,message:e.message}}})()`);
  check(denial.state==='denied'&&!denial.granted&&['NotAllowedError','NotFoundError'].includes(denial.name),'denied camera permission preserves browser failure '+JSON.stringify(denial));
  await evaluate(`(async()=>{const image=new Image();image.src=location.origin.replace('127.0.0.1','localhost')+'/__texture_cross.png';await image.decode();const tainted=document.createElement('canvas');tainted.width=tainted.height=1;tainted.getContext('2d').drawImage(image,0,0);globalThis.taintedEntry=await registry.registerCanvas(tainted);taintedEntry.markFrameAvailable();})()`);
  await until(()=>evaluate(`!!taintedEntry.lastError`),'origin-clean source error');const corsError=await evaluate(`({name:taintedEntry.lastError.name,code:taintedEntry.lastError.code,message:taintedEntry.lastError.message})`);check(corsError.name==='SecurityError'||corsError.code==='DataCloneError'&&/origin.clean/i.test(corsError.message),'CORS source is explicitly rejected '+JSON.stringify(corsError));await evaluate(`taintedEntry.dispose()`);
  for(let i=0;i<5;i++) {await evaluate(`(async()=>{const e=await registry.registerCanvas(canvas);e.markFrameAvailable();await e.dispose()})()`);}
  await until(()=>evaluate(`registry.diagnostics().then(d=>d.live===0&&d.pending===0)`),'final drain');
  state=await evaluate(`(async()=>({gpu:JSON.parse(__dorotiResizeDiagnostics.capability('doroti-surface')),textures:await registry.diagnostics(),userAgent:navigator.userAgent,isolated:crossOriginIsolated,presenter:JSON.parse(__dorotiResizeDiagnostics.presenter('doroti-surface'))}))()`);
  check(state.textures.registrations===0&&state.textures.received===state.textures.closed&&state.textures.imported===state.textures.retired,'all registrations, source frames and destinations returned');
  check(state.presenter.mode===mode,'requested GPU backend active');
  check(!events.some(e=>e.method==='Runtime.exceptionThrown'),'no unhandled exceptions');
  await evaluate(`globalThis.disposedReceipt=new Promise(resolve=>registry.endpoint.addEventListener('message',e=>{if(e.data.kind==='disposed')resolve(e.data)},{once:false}));registry.endpoint.postMessage({protocolVersion:5,kind:'dispose'})`);
  const disposed=await evaluate(`disposedReceipt`);state.disposed=disposed;
  check(disposed.textures.live===0&&disposed.textures.pending===0&&disposed.textures.registrations===0,'view shutdown drains GPU and sources');
  await writeFile(join(out,'assets.json'),JSON.stringify({browser:await cdp('Browser.getVersion'),assets:Object.fromEntries(assets)},null,2));
  await writeFile(join(out,'result.json'),JSON.stringify({status:'CONTRACTS_PASS_PIXELS_PENDING',tests,state,camera:'synthetic browser device; physical notVerified'},null,2));
 }catch(error){await shot('failure');await writeFile(join(out,'failure.json'),JSON.stringify({error:String(error),tests,state:await evaluate(`({error:document.documentElement.dataset.dorotiRendererError,text:document.body.innerText})`)},null,2));throw error;}
 finally{await writeFile(join(out,'events.json'),JSON.stringify(events,null,2));}
 await cdp('Browser.close');
}finally{if(socket)socket.close();chrome.kill();proxy.close();await writeFile(join(out,'chrome.log'),errors.join(''));}
