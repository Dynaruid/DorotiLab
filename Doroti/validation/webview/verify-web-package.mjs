
// Run with the 20-minute timeout wrapper and a new evidence directory.
// Start the Web dev server first; its printed URL is the optional third argument.
import {createServer} from 'node:http';
import {spawn} from 'node:child_process';
import {mkdir,writeFile,readFile} from 'node:fs/promises';
import {join} from 'node:path';
const mode=process.argv[4]??'worker-direct-webgpu';
const upstream = process.argv[3] ?? 'http://127.0.0.1:5000';
const out=process.argv[2]; await mkdir(out,{recursive:true});
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
 try{
  await cdp('Page.navigate',{url:`http://127.0.0.1:${proxy.address().port}/?dorotiRenderer=${mode}`});
  let ready=false;for(let i=0;i<30;i++){ready=await evaluate(`document.querySelector('iframe')?.contentDocument?.body?.dataset.ready==='3'&&document.querySelector('[data-doroti-platform-effect]')!==null`);if(ready)break;await wait(300)}
  if(!ready)throw Error('Package WebView/controller/effect not ready');
  await evaluate(`globalThis.original=document.querySelector('iframe');original.contentDocument.querySelector('input').value='package preserved'`);
  await cdp('Emulation.setDeviceMetricsOverride',{width:900,height:800,deviceScaleFactor:1.25,mobile:false});await wait(800);
  if(!await evaluate(`original===document.querySelector('iframe')&&original.contentDocument.querySelector('input').value==='package preserved'`))throw Error('Package identity lost');
  const shot=await cdp('Page.captureScreenshot',{format:'png'});await writeFile(join(out,'package.png'),Buffer.from(shot.data,'base64'));
  const state=await evaluate(`({iframes:document.querySelectorAll('iframe').length,rasters:document.querySelectorAll('[data-doroti-raster]').length,effects:document.querySelectorAll('[data-doroti-platform-effect]').length,renderer:JSON.parse(__dorotiResizeDiagnostics.presenter('doroti-surface')).mode})`);
  if(events.some(e=>e.method==='Runtime.exceptionThrown'))throw Error('Unhandled package page exception');
  await writeFile(join(out,'result.json'),JSON.stringify({status:'PASS',state,scope:'Generated application; local NuGet packages only; published static page; WebView controller JS, effect and identity',cleanMachine:'notVerified'},null,2));console.log('PASS generated package-only WebView/controller/effect/resize identity');
 }finally{await writeFile(join(out,'events.json'),JSON.stringify(events,null,2));}
 await cdp('Browser.close');
}finally{if(socket)socket.close();chrome.kill();proxy.close();await writeFile(join(out,'chrome.log'),errors.join(''));}
