
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
  await cdp('Page.navigate',{url:`http://127.0.0.1:${proxy.address().port}/__webview_test`});
  const result=await evaluate(`(async()=>{
    const {BrowserPlatformComposition}=await import('/_content/Doroti.Host.Web/doroti.web.composition.js');
    const passed=[],check=(ok,name)=>{if(!ok)throw Error(name);passed.push(name)};
    const roots=[document.createElement('div'),document.createElement('div')];roots.forEach(r=>{Object.assign(r.style,{position:'relative',width:'300px',height:'200px'});document.body.append(r)});
    const owners=roots.map((r,i)=>new BrowserPlatformComposition(r,document.createElement('canvas'),String(i+1),()=>{}));
    const id=owner=>({owner:String(owner),id:'7',generation:'1'});
    const create=(owner)=>owners[owner-1].request({action:'create',identity:id(owner),viewType:'doroti/webview',options:{Profile:2,Html:'<!doctype html><input value="keep">'}});
    await Promise.all([create(1),create(2)]);check(roots.every(r=>r.querySelector('iframe')),'two DOM owners');
    const bounds={left:0,top:0,width:300,height:200};
    const batch=(owner,frame)=>({version:2,owner:String(owner),epoch:1,surfaceGeneration:1,frame,views:[{identity:id(owner),bounds,visible:true,order:1}],shields:[],effects:[]});
    const packet=(owner,frame)=>({batch:batch(owner,frame),rasters:[]});
    check(JSON.parse(owners[0].commit(packet(1,1),1)).accepted,'initial placement');
    check(!JSON.parse(owners[0].commit(packet(1,1),1)).accepted,'late placement ACK rejected');
    check(!JSON.parse(owners[0].commit(packet(1,2),2)).accepted,'resize generation mismatch rejected');
    const foreign=JSON.parse(await owners[0].request({action:'command',identity:id(2),command:{Operation:1}}));check(foreign.error===1,'foreign owner rejected');
    await owners[0].request({action:'disable',identity:id(1)});
    check(!JSON.parse(owners[0].commit(packet(1,3),1)).accepted,'retiring instance never reactivated by a late frame');
    await owners[0].dispose();check(roots[0].querySelectorAll('iframe').length===0&&roots[1].querySelectorAll('iframe').length===1,'owner close does not remove peer');
    check(!JSON.parse(owners[0].commit(packet(1,4),1)).accepted,'late frame after close rejected');
    await owners[1].dispose();check(roots.every(r=>r.children.length===0),'DOM listener and resource owners close');
    const {DorotiPlatformViewDomRegistry}=await import('/_content/Doroti.Host.Web/doroti.web.platform-views.js');
    const registry=new DorotiPlatformViewDomRegistry(roots[0],'3',()=>{},()=>{});let release,disposed=0;
    registry.register('slow',()=>new Promise(r=>release=r));const pending=registry.create(id(3),'slow');const close=registry.dispose();
    release({element:document.createElement('iframe'),dispose:()=>disposed++});await pending;await close;
    check(disposed===1&&registry.liveCount===0&&roots[0].children.length===0,'late resource creation reclaimed after close');
    const primary=document.createElement('canvas');roots[0].append(primary);
    const retained=new BrowserPlatformComposition(roots[0],primary,'4',()=>{});
    const raster=(order,left,pixels)=>({order,bounds:{left,top:10,width:2,height:1},width:2,height:1,pixels});
    const pixels=new Uint8Array([255,0,0,255,0,0,255,128]);
    const frame=(frame,rasters)=>({batch:{version:2,owner:'4',epoch:1,surfaceGeneration:1,frame,views:[],shields:[],effects:[]},rasters});
    check(JSON.parse(retained.commit(frame(1,[raster(0,0,pixels),raster(48,30,pixels)]),1)).accepted,'independent raster slots accepted');
    const canvases=[...roots[0].querySelectorAll('[data-doroti-raster]')];
    check(JSON.parse(retained.commit(frame(2,[raster(0,0,new Uint8Array()),raster(48,80,new Uint8Array())]),1)).accepted,'geometry-only raster commit accepted');
    check([...roots[0].querySelectorAll('[data-doroti-raster]')].every((c,i)=>c===canvases[i])&&roots[0].dataset.dorotiPlatformBytes==='0','translation preserves canvases with zero pixel upload');
    check(canvases[1].style.transform==='translate(80px, 10px)'&&canvases[1].style.zIndex==='48'&&canvases[0].style.transform==='translate(0px, 10px)','independent raster geometry and paint order retained');
    check([...canvases[1].getContext('2d').getImageData(0,0,2,1).data].every((v,i)=>v===pixels[i]),'translation preserves foreground RGBA pixels');
    check(!JSON.parse(retained.commit(frame(2,[raster(48,100,new Uint8Array())]),1)).accepted&&canvases[1].style.transform==='translate(80px, 10px)','stale geometry cannot move retained pixels');
    const wrongSize=raster(48,100,new Uint8Array());wrongSize.width=3;
    check(!JSON.parse(retained.commit(frame(3,[wrongSize]),1)).accepted,'resized raster cannot reuse stale backing');
    check(JSON.parse(retained.commit(frame(3,[]),1)).accepted&&roots[0].querySelectorAll('[data-doroti-raster]').length===0&&primary.style.opacity==='1','removing slices restores primary canvas');
    await retained.dispose();primary.remove();
    return {status:'PASS',scope:'main DOM adapter; two full products not claimed',passed};
  })()`);
  await writeFile(join(out,'result.json'),JSON.stringify(result,null,2));console.log(JSON.stringify(result));
 }finally{await writeFile(join(out,'events.json'),JSON.stringify(events,null,2));}
 await cdp('Browser.close');
}finally{if(socket)socket.close();chrome.kill();proxy.close();await writeFile(join(out,'chrome.log'),errors.join(''));}
