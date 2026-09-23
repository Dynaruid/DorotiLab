
// Run with the 20-minute timeout wrapper and a new evidence directory.
// Start the Web dev server first; its printed URL is the optional third argument.
import {createServer} from 'node:http';
import {spawn} from 'node:child_process';
import {mkdir,writeFile,readFile} from 'node:fs/promises';
import {join} from 'node:path';
const upstream = process.argv[3] ?? 'http://127.0.0.1:5000';
const out=process.argv[2]; await mkdir(out,{recursive:true});
const proxy=createServer(async(req,res)=>{try {const r=await fetch(upstream+req.url);const body=Buffer.from(await r.arrayBuffer());res.writeHead(r.status,{'Content-Type':r.headers.get('content-type')||'application/octet-stream','Cross-Origin-Opener-Policy':'same-origin','Cross-Origin-Embedder-Policy':'require-corp','Cross-Origin-Resource-Policy':'same-origin','Cache-Control':'no-store'});res.end(body);}catch(e){res.writeHead(502);res.end(String(e));}});
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
 socket.onmessage=ev=>{const m=JSON.parse(ev.data);if(m.id){const p=pending.get(m.id);pending.delete(m.id);m.error?p.reject(Error(JSON.stringify(m.error))):p.resolve(m.result);}else if(['Runtime.exceptionThrown','Runtime.consoleAPICalled','Log.entryAdded'].includes(m.method)){events.push(m);}};
 const cdp=(method,params={})=>new Promise((resolve,reject)=>{const id=++next;const timer=setTimeout(()=>{pending.delete(id);reject(Error('CDP timeout: '+method));},15000);pending.set(id,{resolve:v=>{clearTimeout(timer);resolve(v)},reject:e=>{clearTimeout(timer);reject(e)}});socket.send(JSON.stringify({id,method,params}));});
 const evaluate=async(expression)=>{const x=await cdp('Runtime.evaluate',{expression,returnByValue:true,awaitPromise:true});if(x.exceptionDetails)throw Error(JSON.stringify(x.exceptionDetails));return x.result.value;};
 await cdp('Page.enable');await cdp('Runtime.enable');await cdp('Log.enable');
 await cdp('Emulation.setDeviceMetricsOverride',{width:1000,height:800,deviceScaleFactor:1,mobile:false});
 await cdp('Page.navigate',{url:'http://127.0.0.1:'+proxy.address().port+'/?dorotiTestbedMode=sample'});
 let state;for(let n=0;n<120;n++){state=await evaluate(`({html:{...document.documentElement.dataset},root:{...document.querySelector('main')?.dataset},text:document.body?.innerText?.slice(0,400),hosts:globalThis.__dorotiResizeDiagnostics?.hosts()})`);if(state.html?.dorotiBootstrapStage==='failed')throw Error(JSON.stringify(state));if(state.root?.dorotiWorkerRuntime==='ready')break;await wait(500);}
 await wait(1200);

 if(state.root?.dorotiWorkerRuntime!=='ready')throw Error('Web product not ready: '+JSON.stringify(state));
 const shot=async(name)=>{const x=await cdp('Page.captureScreenshot',{format:'png'});await writeFile(join(out,name+'.png'),Buffer.from(x.data,'base64'));};
 const nodes=()=>evaluate(`Array.from(document.querySelectorAll('[role]')).map(e=>{const r=e.getBoundingClientRect();return {role:e.getAttribute('role'),label:e.getAttribute('aria-label'),checked:e.getAttribute('aria-checked'),selected:e.getAttribute('aria-selected'),disabled:e.getAttribute('aria-disabled'),x:r.x+r.width/2,y:r.y+r.height/2,w:r.width,h:r.height}}).filter(e=>e.w>0&&e.h>20&&e.x>0&&e.x<1000&&e.y>58&&e.y<720)`);
 const click=async(x,y)=>{await cdp('Input.dispatchMouseEvent',{type:'mouseMoved',x,y});await cdp('Input.dispatchMouseEvent',{type:'mousePressed',x,y,button:'left',clickCount:1});await wait(80);await cdp('Input.dispatchMouseEvent',{type:'mouseReleased',x,y,button:'left',clickCount:1});await wait(350);};
 const check=(v,m)=>{if(!v)throw Error(m);console.log('PASS '+m);};
 const until=async(fn,label)=>{for(let n=0;n<40;n++){const v=await fn();if(v)return v;await wait(100);}throw Error('Timed out: '+label);};
 const seek=async(predicate,label)=>{await cdp('Input.dispatchMouseEvent',{type:'mouseWheel',x:750,y:400,deltaX:0,deltaY:-5000});await wait(350);for(let n=0;n<55;n++){const visible=await nodes();const v=visible.find(predicate);if(v){await wait(650);const settled=(await nodes()).find(predicate);if(settled)return settled;}await cdp('Input.dispatchMouseEvent',{type:'mouseWheel',x:750,y:400,deltaX:0,deltaY:240});await wait(350);}throw Error('Not found: '+label);};
 const tab=async(label)=>{const item=await evaluate(`(()=>{const e=document.querySelector('[role="tab"][aria-label=${JSON.stringify(label)}]');if(!e)return null;const r=e.getBoundingClientRect();return {x:r.x+r.width/2,y:r.y+r.height/2};})()`);check(item,'tab '+label+' exists');await click(item.x,item.y);await until(()=>evaluate(`document.querySelector('[role="tab"][aria-label=${JSON.stringify(label)}]')?.getAttribute('aria-selected')==='true'`),'select '+label);};
 try {
   await shot('initial');
   const defaults=await until(async()=>{const visible=await nodes();return visible.some(x=>x.role==='button'&&x.label==='Filled'&&x.disabled==='true')?visible:null;},'initial controls');check(defaults.some(x=>x.role==='button'&&x.label==='Filled'&&x.disabled==='true'),'disabled button stays disabled');
   await tab('Color');await shot('color');await tab('Components');
   const box=await seek(x=>x.role==='checkbox'&&x.label==='Option 1'&&x.disabled!=='true','checkbox');
   console.log('checkbox target '+JSON.stringify(box));await click(box.x,box.y);await until(async()=>(await nodes()).some(x=>x.role==='checkbox'&&x.label===box.label&&x.checked!==box.checked),'checkbox toggle');
   check(true,'checkbox changes state after scrolling');await shot('checkbox');
   const picker=await seek(x=>x.role==='button'&&(x.label??'').includes('Show date picker'),'date picker');await click(picker.x,picker.y);
   const cancel=await until(async()=>(await nodes()).find(x=>x.role==='button'&&/^cancel$/i.test(x.label??'')),'picker cancel');await shot('date-picker');await click(cancel.x,cancel.y);
   await until(async()=>!(await nodes()).some(x=>x.role==='button'&&/^cancel$/i.test(x.label??'')),'picker dismissal');check(true,'picker cancellation returns to controls');
   const toggle=await seek(x=>x.role==='switch'&&x.disabled!=='true','switch');await click(toggle.x,toggle.y);await until(async()=>(await nodes()).some(x=>x.role==='switch'&&Math.abs(x.x-toggle.x)<2&&x.checked!==toggle.checked),'switch toggle');check(true,'switch changes state');
   const field=await seek(x=>x.role==='textbox'&&/Filled/.test(x.label??'')&&x.disabled!=='true','text field');await click(field.x,field.y);await cdp('Input.insertText',{text:'A1 web input'});
   await until(()=>evaluate(`document.querySelector('#doroti-ime')?.value.includes('A1 web input')`),'text entry');check(true,'text editing after scrolling');await shot('text-input');
   await tab('Color');await until(()=>evaluate(`document.querySelector('#doroti-ime')?.hidden===true`),'text connection closes');await tab('Components');check(true,'navigation disposes and remounts controls');
   const presenter=await evaluate(`JSON.parse(globalThis.__dorotiResizeDiagnostics.presenter('doroti-surface'))`);
   check(presenter.mode==='worker-direct-webgl'&&!presenter.contextLost,'default WebGL2 renderer remains active');
   check(!events.some(e=>e.method==='Runtime.exceptionThrown'),'no unhandled page exceptions');
   await writeFile(join(out,'result.json'),JSON.stringify({status:'passed',state,presenter,tests:['disabled callback','navigation','scroll and checkbox','picker cancellation','switch','text input','dispose and remount'],input:'Chrome CDP browser input',physicalInput:'notVerified'},null,2));
 }catch(error){await shot('failure');await writeFile(join(out,'failure-nodes.json'),JSON.stringify(await nodes(),null,2));throw error;}
 finally{await writeFile(join(out,'events.json'),JSON.stringify(events,null,2));await writeFile(join(out,'state.json'),JSON.stringify(state,null,2));}
 await cdp('Browser.close');
 if(state.root?.dorotiWorkerRuntime!=='ready')throw Error('Web product not ready');
}finally{if(socket)socket.close();chrome.kill();proxy.close();await writeFile(join(out,'chrome.log'),errors.join(''));}
