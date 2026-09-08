import { chromium } from '@playwright/test';
import { mkdir, writeFile } from 'node:fs/promises';

const label=process.argv[2], scenario=process.argv[3]??'carousel';
if (!/^[\w-]+$/.test(label) || !['carousel','snapping','scroll'].includes(scenario)) throw Error('Invalid label/scenario');
const detailed=!process.argv.includes('--minimal');
const fling=process.argv.includes('--fling');
const steps=fling?6:24, stepDelay=fling?8:16;
const timeout=setTimeout(()=>{console.error('20-minute timeout');process.exit(1);},1200000);
const browser=await chromium.launch({headless:true,args:['--enable-gpu-rasterization','--ignore-gpu-blocklist','--use-angle=default']});
const errors=[];
const stats=a=>{a.sort((a,b)=>a-b);return {n:a.length,p50:a[Math.floor(a.length/2)]??null,p95:a[Math.ceil(a.length*.95)-1]??null,max:a.at(-1)??null};};
await mkdir('artifacts/gallery-motion',{recursive:true});
try {
 const page=await browser.newPage({viewport:{width:1280,height:900}});
 page.on('pageerror',e=>errors.push(String(e)));
 const session=await page.context().newCDPSession(page);
 await session.send('Emulation.setTouchEmulationEnabled',{enabled:true,maxTouchPoints:1});
 await page.goto((process.env.DOROTI_WEB_BASE_URL??'http://127.0.0.1:5090')+`/?dorotiTestbedMode=sample&dorotiSectionViewport=indexed&dorotiResizeDiagnostics=${detailed?1:0}&dorotiInputMarkers=1`+(process.env.DOROTI_PERF_QUERY??''));
 const read=()=>page.evaluate(()=>{const d=globalThis.__dorotiResizeDiagnostics,id=d.hosts()[0];return {time:performance.now(),snapshot:JSON.parse(d.snapshot(id)),presenter:JSON.parse(d.presenter('doroti-surface')),trace:JSON.parse(d.capture(id))};});
 const profile=async()=>detailed?(await Promise.all(page.workers().map(w=>w.evaluate(()=>globalThis.__dorotiDirectDiagnostics?.()??null)))).filter(Boolean):[];
 await page.waitForFunction(()=>globalThis.__dorotiResizeDiagnostics && JSON.parse(globalThis.__dorotiResizeDiagnostics.presenter('doroti-surface')).frontRequestId>0,null,{timeout:120000});
 await page.waitForTimeout(2500);
 let box;
 if(scenario!=='scroll') {
  const target=page.getByRole('group',{name:'Carousel',exact:true});
  for(let i=0;i<35;i++) {
   box=await target.count()?await target.boundingBox():null;
   if(box&&box.y>80&&box.y+box.height<840)break;
   await page.mouse.move(500,600);await page.mouse.wheel(0,box&&box.y<80?-250:350);await page.waitForTimeout(500);
  }
  await page.mouse.move(20,40);await page.waitForTimeout(2000);box=await target.boundingBox();
  if(!box||box.y<80||box.y+box.height>840)throw Error('Carousel target not visible');
 }
 const imageBefore=await page.screenshot();
 const profileBefore=await profile(),before=await read(),windows=[];
 let changed=false;
 for(let gesture=0;gesture<6;gesture++) {
  const reverse=gesture>=3;
  const start=scenario==='scroll'?{x:500,y:reverse?260:760}:{x:box.x+box.width*(reverse?.22:.78),y:box.y+(scenario==='snapping'?box.height*.78:box.height*.34)};
  const dx=scenario==='scroll'?0:box.width*.5*(reverse?1:-1),dy=scenario==='scroll'?(reverse?500:-500):0;
  const begin=await page.evaluate(()=>performance.now());
  const point=(x,y)=>[{x,y,id:1,radiusX:3,radiusY:3,force:1}];
  await session.send('Input.dispatchTouchEvent',{type:'touchStart',touchPoints:point(start.x,start.y)});
  for(let step=1;step<=steps;step++) {
   await session.send('Input.dispatchTouchEvent',{type:'touchMove',touchPoints:point(start.x+dx*step/steps,start.y+dy*step/steps)});
   await page.waitForTimeout(stepDelay);
  }
  await session.send('Input.dispatchTouchEvent',{type:'touchEnd',touchPoints:[]});
  const released=await page.evaluate(()=>performance.now());
  await page.waitForTimeout(fling?1000:700);
  windows.push({gesture,reverse,begin,released,end:await page.evaluate(()=>performance.now())});
  if(gesture===0) { const shot=await page.screenshot();changed=!imageBefore.equals(shot);await writeFile(`artifacts/gallery-motion/${label}.png`,shot); }
 }
 const after=await read(),profileAfter=await profile();
 const active=e=>windows.some(w=>e.timestampMicroseconds/1000>=w.begin && (e.timestampMicroseconds+e.durationMicroseconds)/1000<=w.end);
 const frames=after.trace.filter(e=>e.phase==='framework-frame'&&active(e));
 const fronts=after.trace.filter(e=>e.phase==='front-commit'&&e.source==='worker-direct-surface'&&active(e));
 const intervals=windows.flatMap(w=>{const a=fronts.filter(e=>e.timestampMicroseconds/1000>=w.begin&&e.timestampMicroseconds/1000<=w.end);return a.slice(1).map((e,i)=>(e.timestampMicroseconds-a[i].timestampMicroseconds)/1000);});
 const coastIntervals=windows.flatMap(w=>{const a=fronts.filter(e=>e.timestampMicroseconds/1000>=w.released&&e.timestampMicroseconds/1000<=w.end);return a.slice(1).map((e,i)=>(e.timestampMicroseconds-a[i].timestampMicroseconds)/1000);});
 const result={label,scenario,detailed,fling,errors,changed,browser:browser.version(),url:page.url(),box,windows,before,after,profileBefore,profileAfter,
  callbacks:stats(frames.map(e=>e.durationMicroseconds/1000)),commitIntervals:stats(intervals),commits:fronts.length,
  coastIntervals:stats(coastIntervals),
  limitation:'CDP touch gestures and commit notifications, not physical FPS. Profile exports and screenshots are outside gesture windows; aggregate profiles include setup between gestures.'};
 await writeFile(`artifacts/gallery-motion/${label}.json`,JSON.stringify(result,null,2));
 console.log(JSON.stringify({label,scenario,changed,errors,callbacks:result.callbacks,commitIntervals:result.commitIntervals,coastIntervals:result.coastIntervals,commits:result.commits}));
 if(!changed||errors.length)process.exitCode=1;
} catch(error) {await writeFile(`artifacts/gallery-motion/${label}.failure.json`,JSON.stringify({label,scenario,errors,error:String(error)},null,2));throw error;}
finally {await browser.close();clearTimeout(timeout);}
