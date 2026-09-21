
// Run with the 20-minute timeout wrapper and a new evidence directory.
// Start the Web dev server first; its printed URL is the optional third argument.
import {createServer} from 'node:http';
import {spawn} from 'node:child_process';
import {mkdir,writeFile,readFile} from 'node:fs/promises';
import {join} from 'node:path';
const mode=process.argv[4]??'worker-direct-webgpu';
const upstream = process.argv[3] ?? 'http://127.0.0.1:5000';
const out=process.argv[2]; await mkdir(out,{recursive:true});
const proxy=createServer(async(req,res)=>{try {if(req.url.startsWith('/__webview_test')){res.writeHead(200,{'Content-Type':'text/html','Cross-Origin-Opener-Policy':'same-origin','Cross-Origin-Embedder-Policy':'require-corp','Cross-Origin-Resource-Policy':'cross-origin'});res.end('<!doctype html><title>Browser content</title><input value="same origin"><style>html,body{margin:0;height:100%;background:linear-gradient(to right,#000 50%,#fff 50%)}</style>');return;}const r=await fetch(upstream+req.url);const body=Buffer.from(await r.arrayBuffer());res.writeHead(r.status,{'Content-Type':r.headers.get('content-type')||'application/octet-stream','Cross-Origin-Opener-Policy':'same-origin','Cross-Origin-Embedder-Policy':'require-corp','Cross-Origin-Resource-Policy':'same-origin','Cache-Control':'no-store'});res.end(body);}catch(e){res.writeHead(502);res.end(String(e));}});
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
 const until=async(fn)=>{for(let i=0;i<30;i++){if(await fn())return;await wait(300);}throw Error('Calibration readiness timeout')};
 const shot=async(name)=>{await wait(250);const x=await cdp('Page.captureScreenshot',{format:'png'});await writeFile(join(out,name+'.png'),Buffer.from(x.data,'base64'));};
 const style=async(strength,saturation,tint,raster)=>{const old=await evaluate(`document.querySelector('.doroti-root').dataset.dorotiPlatformFrame`);await evaluate(`probe.Style(${strength},${saturation},${tint},${raster})`);await until(()=>evaluate(`document.querySelector('.doroti-root').dataset.dorotiPlatformFrame!==${JSON.stringify(old)}`));};
 try{
  await cdp('Page.navigate',{url:`http://127.0.0.1:${proxy.address().port}/?dorotiTestbedMode=platform-effects&dorotiRenderer=${mode}`});
  await until(()=>evaluate(`document.querySelector('iframe')?.contentDocument?.querySelector('input')!=null&&document.querySelector('iframe').getBoundingClientRect().width>0`));
  await evaluate(`(async()=>{globalThis.probe=(await getDotnetRuntime(0).getAssemblyExports('DorotiTestbedApp.Web.dll')).DorotiTestbedApp.Web.Validation.WebPlatformExport})()`);
  await evaluate(`probe.Command(3, '<!doctype html><style>html,body{margin:0;height:100%;background:linear-gradient(to right,#000 50%,#fff 50%)}</style>', 0)`);
  await until(()=>evaluate(`probe.Command(1,'',0).then(x=>!JSON.parse(x).IsLoading)`));
  const geometry=await evaluate(`(()=>{const r=document.querySelector('iframe').getBoundingClientRect();return{x:r.x,y:r.y,width:r.width,height:r.height}})()`);
  for(const raster of [false,true]){
   for(const sigma of [0,4,16]){await style(sigma/16,1,0,raster);await shot(`${raster?'raster':'native'}-${sigma}`);}
   await style(0,1,0,raster);await shot(`${raster?'raster':'native'}-reset`);
  }
  await style(.25,1,0,false);
  await evaluate(`probe.Command(2, location.origin.replace('127.0.0.1','localhost')+'/__webview_test', 0)`);
  await until(()=>evaluate(`probe.Command(1,'',0).then(x=>!JSON.parse(x).IsLoading)`));await shot('cross-origin-4');
  await evaluate(`probe.Command(3, '<!doctype html><style>html,body{margin:0;height:100%;background:linear-gradient(to right,#000 50%,#fff 50%)}</style>', 0)`);
  await until(()=>evaluate(`probe.Command(1,'',0).then(x=>!JSON.parse(x).IsLoading)`));
  await style(.25,1,-2147483393,false);await shot('blue-tint');
  await evaluate(`probe.Command(8, 'document.body.style.background="linear-gradient(to right,red 50%,blue 50%)";document.documentElement.style.background="red"', 0)`);
  await style(0,0,0,false);await shot('saturation-zero');
  await style(.75,1,0,false);
  await evaluate(`probe.Command(8, 'document.body.style.background="red"', 0)`);await shot('live-red');
  await evaluate(`probe.Command(8, 'document.body.style.background="blue"', 0)`);await shot('live-blue');
  await writeFile(join(out,'geometry.json'),JSON.stringify({geometry,renderer:mode,physical:'notVerified'},null,2));
  console.log('PASS calibration captures '+mode);
 }finally{await writeFile(join(out,'events.json'),JSON.stringify(events,null,2));}
 await cdp('Browser.close');
}finally{if(socket)socket.close();chrome.kill();proxy.close();await writeFile(join(out,'chrome.log'),errors.join(''));}
