
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
const proxy=createServer(async(req,res)=>{try {if(req.url.startsWith('/__webview_test')){res.writeHead(200,{'Content-Type':'text/html','Cross-Origin-Opener-Policy':'same-origin','Cross-Origin-Embedder-Policy':'require-corp','Cross-Origin-Resource-Policy':'cross-origin'});res.end('<!doctype html><title>Browser content</title><input value="same origin"><style>body{background:#abc}</style>');return;}const r=await fetch(upstream+req.url);const body=Buffer.from(await r.arrayBuffer());if(r.status===200)assets.set(req.url,{sha256:createHash('sha256').update(body).digest('hex'),bytes:body.length});res.writeHead(r.status,{'Content-Type':r.headers.get('content-type')||'application/octet-stream','Cross-Origin-Opener-Policy':'same-origin','Cross-Origin-Embedder-Policy':'require-corp','Cross-Origin-Resource-Policy':'same-origin','Cache-Control':'no-store'});res.end(body);}catch(e){res.writeHead(502);res.end(String(e));}});
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


 try {
  await cdp('Page.navigate',{url:`http://127.0.0.1:${proxy.address().port}/?dorotiTestbedMode=texture-web&dorotiRenderer=${mode}&dorotiResizeDiagnostics=1`});
  await until(()=>evaluate(`document.documentElement.dataset.dorotiBootstrapStage==='started'`),'ready');
  await evaluate(`(async()=>{globalThis.probe=(await getDotnetRuntime(0).getAssemblyExports('DorotiTestbedApp.Web.dll')).DorotiTestbedApp.Web.Validation.WebTextureExport;globalThis.sample=await import('./plugins/textures.js');globalThis.registry=(await import('./_content/Doroti.Host.Web/doroti.web.textures.js')).texturesForCanvas();globalThis.id=await sample.select('canvas');await probe.Show(id,false,0);})()`);
  await until(()=>evaluate(`registry.diagnostics().then(d=>d.imported>=3)`),'active imported frames');await shot('before-loss');
  const before=await evaluate('registry.diagnostics()');
  await evaluate(`globalThis.lossReceipt=new Promise(resolve=>registry.endpoint.addEventListener('message',e=>{if(e.data.kind==='disposed')resolve(e.data.textures);if(e.data.kind==='texture-response'&&e.data.request===999999)resolve(e.data);if(e.data.kind==='context-lost')setTimeout(()=>registry.endpoint.postMessage({protocolVersion:5,kind:'texture',operation:'diagnostics',request:999999}),100);}));__dorotiResizeDiagnostics.loseContext('doroti-surface')`);
  const after=await evaluate('lossReceipt');
  check(after.live===0&&after.pending===0&&after.registrations===0,'context/device loss releases all textures');
  check(after.received===after.closed&&after.imported===after.retired,'loss closes sources and retires images exactly once');
  check(await evaluate(`registry.createFrameProducer().then(()=>false,e=>e.code==='Disposed')`),'lost view requires fresh binding');
  const errors=events.filter(e=>e.method==='Runtime.exceptionThrown'||e.method==='Runtime.consoleAPICalled'&&e.params.type==='error'&&!e.params.args.some(a=>/Buffer async map failed with status Aborted.*Device was lost/.test(a.value??'')));
  check(errors.length===0,'no unhandled exception on loss');
  await writeFile(join(out,'result.json'),JSON.stringify({status:'PASS',tests,before,after,errors},null,2));
  await writeFile(join(out,'assets.json'),JSON.stringify({browser:await cdp('Browser.getVersion'),assets:Object.fromEntries(assets)},null,2));
 } finally {await writeFile(join(out,'events.json'),JSON.stringify(events,null,2));}
 await cdp('Browser.close');
}finally{if(socket)socket.close();chrome.kill();proxy.close();await writeFile(join(out,'chrome.log'),errors.join(''));}
