
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
 const check=(ok,name)=>{if(!ok)throw Error(name);console.log('PASS '+name);tests.push(name)}; const tests=[];
 const until=async(fn,name,timeout=120000)=>{let start=Date.now();while(Date.now()-start<timeout){const x=await fn();if(x)return x;if(await evaluate(`document.documentElement.dataset.dorotiRendererError||document.body.innerText.includes("Doroti failed to start")`))throw Error("Product startup failed");await wait(200);}throw Error('Timeout '+name)};
 const shot=async(name)=>{const x=await cdp('Page.captureScreenshot',{format:'png'});await writeFile(join(out,name+'.png'),Buffer.from(x.data,'base64'));};
 const command=async(op,text='',doc=0)=>JSON.parse(await evaluate(`probe.Command(${op},${JSON.stringify(text)},${doc})`));
 const stage=async(value)=>{const old=await evaluate(`document.querySelector('.doroti-root')?.dataset.dorotiPlatformFrame`);await evaluate(`probe.Stage(${value})`);await until(()=>evaluate(`document.querySelector('.doroti-root')?.dataset.dorotiPlatformFrame!==${JSON.stringify(old)}`),'stage frame');await wait(300)};
 let state;
 try{
  await cdp('Page.navigate',{url:`http://127.0.0.1:${proxy.address().port}/?dorotiTestbedMode=platform-effects&dorotiRenderer=${mode}&dorotiResizeDiagnostics=1`});
  await until(()=>evaluate(`document.querySelector('iframe')?.contentDocument?.querySelector('input')!=null`),'live iframe');
  await until(()=>evaluate(`Number(document.querySelector('.doroti-root')?.dataset.dorotiPlatformFrame)>0`),'composition ACK');
  await evaluate(`(async()=>{globalThis.probe=(await getDotnetRuntime(0).getAssemblyExports('DorotiTestbedApp.Web.dll')).DorotiTestbedApp.Web.Validation.WebPlatformExport})()`);
  const features=(await command(0)).Features;check(features.JavaScript&&features.BrowserDefaultProfile&&!features.EphemeralProfile,'public controller features');
  const unknown=await command(1);check(!unknown.HistoryKnown&&!unknown.NavigationStateKnown,'unobservable iframe navigation state stays unknown');
  check((await command(8,'1+2')).Json==='3','public controller JavaScript');
  check((await command(8,'undefined')).IsUndefined,'undefined distinct from null');
  check((await command(8,'null')).Json==='null','JSON null');
  check((await command(8,'throw new Error("expected")')).error==='JavaScript','JavaScript typed error');
  check((await command(6)).error==='Unsupported','history explicitly unsupported');
  check((await command(9)).error==='Unsupported','profile deletion explicitly unsupported');
  await evaluate(`globalThis.originalFrame=document.querySelector('iframe');originalFrame.contentDocument.querySelector('input').value='preserved 한글'`);
  await shot('blur');await stage(1);await shot('plain');
  check(await evaluate(`originalFrame===document.querySelector('iframe')&&originalFrame.contentDocument.querySelector('input').value==='preserved 한글'`),'effect toggle preserves iframe and editing');
  await stage(3);check(await evaluate(`document.querySelectorAll('iframe').length===2`),'two actual product WebViews');await shot('two');
  await stage(4);check(await evaluate(`originalFrame===document.querySelector('iframe')&&originalFrame.contentDocument.querySelector('input').value==='preserved 한글'`),'movement preserves iframe document');await shot('moved');
  await stage(0);
  const rect=await evaluate(`(()=>{const f=document.querySelector('iframe'),r=f.getBoundingClientRect(),b=f.contentDocument.querySelector('button').getBoundingClientRect();return{x:r.left+b.left+15,y:r.top+b.top+15}})()`);
  const click=async()=>{await cdp('Input.dispatchMouseEvent',{type:'mousePressed',...rect,button:'left',clickCount:1});await cdp('Input.dispatchMouseEvent',{type:'mouseReleased',...rect,button:'left',clickCount:1});await wait(200)};
  await click();check(await evaluate(`originalFrame.contentDocument.querySelector('button').textContent==='1'`),'native click through effect');
  await stage(2);await click();check(await evaluate(`originalFrame.contentDocument.querySelector('button').textContent==='1'`),'shield blocks native click');
  const originalDpr=await evaluate('devicePixelRatio');
  await cdp('Input.dispatchMouseEvent',{type:'mouseWheel',...rect,deltaX:0,deltaY:120,modifiers:2});await wait(250);
  check(await evaluate(`devicePixelRatio===${originalDpr}&&originalFrame.contentDocument.scrollingElement.scrollTop===0`),'shield consumes control-wheel without browser zoom or iframe scrolling');

  await stage(0);
  const focusClick=async(x,y)=>{await cdp('Input.dispatchMouseEvent',{type:'mousePressed',x,y,button:'left',clickCount:1});await cdp('Input.dispatchMouseEvent',{type:'mouseReleased',x,y,button:'left',clickCount:1});await wait(200)};
  const focusGeometry=await evaluate(`(()=>{const r=originalFrame.getBoundingClientRect(),i=originalFrame.contentDocument.querySelector('input').getBoundingClientRect();return{frameworkY:r.bottom+25,nativeX:r.left+i.left+20,nativeY:r.top+i.top+20}})()`);
  await focusClick(80,focusGeometry.frameworkY);await until(()=>evaluate(`document.activeElement?.id==='doroti-ime'`),'framework input focus');
  await cdp('Input.insertText',{text:'framework'});
  await focusClick(focusGeometry.nativeX,focusGeometry.nativeY);await cdp('Input.insertText',{text:'웹입력'});
  check(await evaluate(`document.activeElement===originalFrame&&originalFrame.contentDocument.querySelector('input').value.includes('웹입력')&&document.querySelector('#doroti-ime').hidden`),'framework to iframe native text focus and synthetic Korean insertion');
  await focusClick(80,focusGeometry.frameworkY);await cdp('Input.insertText',{text:' return'});
  check(await evaluate(`document.activeElement?.id==='doroti-ime'&&document.querySelector('#doroti-ime').value.includes('return')`),'iframe returns input to framework');
  for(const dpr of [1.25,1.5,2,1]){const viewportWidth=dpr===1.5?1001:dpr===2?1002:1000;await cdp('Emulation.setDeviceMetricsOverride',{width:viewportWidth,height:800,deviceScaleFactor:dpr,mobile:false});await wait(600);await until(()=>evaluate(`document.querySelector('[data-doroti-raster="0"]')?.width===${Math.round(viewportWidth*dpr)}`),'DPR raster');check(await evaluate(`originalFrame===document.querySelector('iframe')`),'DPR + resize '+dpr+' preserves identity and physical raster size')}
  for(let i=0;i<10;i++){await stage(5);await until(()=>evaluate(`document.querySelectorAll('iframe').length===0`),'dispose');await stage(0);await until(()=>evaluate(`document.querySelectorAll('iframe').length===1`),'recreate')}
  check(true,'10 product remove/recreate cycles');
  await until(async()=>!(await command(1)).IsLoading,'document ready');
  const current=(await command(1)).DocumentGeneration;
  check((await command(8,'42',current-1)).error==='NavigationChanged','stale document rejected');
  await evaluate(`void (globalThis.pendingScript=probe.Command(8,'new Promise(resolve=>setTimeout(()=>resolve(123),800))',0))`);
  await command(3,'<!doctype html><title>Replacement</title><input value="replacement">');
  check(JSON.parse(await evaluate('pendingScript')).error==='NavigationChanged','navigation cancels pending JavaScript');
  await until(async()=>(await command(1)).Title==='Replacement','new HTML');
  check((await command(8,'document.querySelector("input").value')).Json==='"replacement"','load HTML changes document');
  const origin=await evaluate('location.origin');
  await command(2,origin+'/__webview_test');await until(async()=>(await command(1)).Title==='Browser content','same-origin navigation');
  check((await command(8,'document.querySelector("input").value')).Json==='"same origin"','same-origin URL and JS');
  await command(4);await until(async()=>!(await command(1)).IsLoading,'reload');check(true,'same-origin reload');
  await command(2,origin.replace('127.0.0.1','localhost')+'/__webview_test');await until(async()=>!(await command(1)).IsLoading,'cross-origin load');
  check(!(await command(0)).Features.JavaScript,'cross-origin feature restriction');
  check((await command(8,'1')).error==='Unsupported','cross-origin JS is typed unsupported');
  check((await command(2,'file:///tmp/example')).error==='InvalidRequest','external scheme rejected');
  await stage(5);await stage(0);await until(()=>evaluate(`document.querySelector('iframe')?.contentDocument?.querySelector('input')!=null`),'restore fixture');

  state=await evaluate(`({root:Object.fromEntries(Object.entries(document.querySelector('.doroti-root').dataset).filter(([key])=>key!=="dorotiResizeDiagnostics")),presenter:JSON.parse(__dorotiResizeDiagnostics.presenter('doroti-surface')),iframes:document.querySelectorAll('iframe').length,rasters:document.querySelectorAll('[data-doroti-raster]').length})`);
  check(state.presenter.mode===mode,'requested renderer active');
  check(!events.some(e=>e.method==='Runtime.exceptionThrown'),'no unhandled page exception');
  await shot('final');await writeFile(join(out,'result.json'),JSON.stringify({status:'PASS',tests,state,physical:'notVerified'},null,2));
 }catch(error){await shot('failure');await writeFile(join(out,'failure.json'),JSON.stringify({error:String(error),tests,state:await evaluate(`({error:document.documentElement.dataset.dorotiRendererError,root:document.querySelector('.doroti-root')?.dataset,text:document.body.innerText,runtime:globalThis.getDotnetRuntime?.(0)?.runtimeBuildInfo})`)},null,2));throw error;}
 finally{await writeFile(join(out,'events.json'),JSON.stringify(events,null,2));}
 await cdp('Browser.close');
}finally{if(socket)socket.close();chrome.kill();proxy.close();await writeFile(join(out,'chrome.log'),errors.join(''));}
