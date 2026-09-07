import { chromium } from '@playwright/test';
import { mkdir, writeFile } from 'node:fs/promises';

const label=process.argv[2], scenario=process.argv[3]??'button';
if(!/^[\w-]+$/.test(label)) throw Error('simple label required');
const detailed=!process.argv.includes('--minimal');
const deadline=setTimeout(()=>{console.error('20-minute timeout');process.exit(1);},1200000);
const browser=await chromium.launch({headless:true,args:['--enable-gpu-rasterization','--ignore-gpu-blocklist','--use-angle=default']});
const stats=a=>{a.sort((a,b)=>a-b);return {n:a.length,p95:a[Math.ceil(a.length*.95)-1]??null,max:a.at(-1)??null};};
try {
 const page=await browser.newPage({viewport:{width:1280,height:900}}),errors=[];
 page.on('pageerror',e=>errors.push(String(e)));
 await page.goto((process.env.DOROTI_WEB_BASE_URL??'http://127.0.0.1:5088')+`/?dorotiTestbedMode=sample&dorotiResizeDiagnostics=${detailed?1:0}&dorotiInputMarkers=1`);
 const read=()=>page.evaluate(()=>{const d=globalThis.__dorotiResizeDiagnostics,id=d.hosts()[0];return {time:performance.now(),snapshot:JSON.parse(d.snapshot(id)),presenter:JSON.parse(d.presenter('doroti-surface')),trace:JSON.parse(d.capture(id))};});
 const managed=async()=>detailed?(await Promise.all(page.workers().map(w=>w.evaluate(()=>globalThis.__dorotiDirectDiagnostics?.()??null)))).filter(Boolean):[];
 await page.waitForFunction(()=>globalThis.__dorotiResizeDiagnostics&&JSON.parse(globalThis.__dorotiResizeDiagnostics.presenter('doroti-surface')).frontRequestId>0,null,{timeout:120000});
 await page.waitForTimeout(10000);
 let bounds;
 if(scenario==='button') {
  const button=page.getByRole('radio',{name:'Week',exact:true});
  for(let i=0;i<30;i++) {bounds=await button.count()?await button.boundingBox():null;if(bounds&&bounds.y>150&&bounds.y+bounds.height<780)break;await page.mouse.move(500,600);await page.mouse.wheel(0,180);await page.waitForTimeout(400);}
  if(!bounds||bounds.y<150||bounds.y+bounds.height>=780)throw Error('Week button not visible');
 }
 await page.mouse.move(20,40);await page.waitForTimeout(2000);
 await page.evaluate(()=>{globalThis.__stateInputs=[];document.querySelector('.doroti-root').addEventListener('pointerup',e=>{const time=e.timeStamp;queueMicrotask(()=>{const d=globalThis.__dorotiResizeDiagnostics;globalThis.__stateInputs.push({time,sequence:JSON.parse(d.snapshot(d.hosts()[0])).inputSequence});});});});
 const profileBefore=await managed(),before=await read(),steps=[];
 if(scenario==='button')await page.mouse.click(bounds.x+bounds.width/2,bounds.y+bounds.height/2);
 else for(const width of scenario==='resize-wide'?[1240,1200,1160,1120,1160,1200,1240,1280]:[980,900,800,1100,1400,900,1100,1280]) {await page.setViewportSize({width,height:900});steps.push({width,time:await page.evaluate(()=>performance.now())});await page.waitForTimeout(45);}
 const end=await page.evaluate(()=>performance.now());
 if(scenario==='button')await page.waitForFunction(()=>{const input=globalThis.__stateInputs[0],d=globalThis.__dorotiResizeDiagnostics;return input&&JSON.parse(d.capture(d.hosts()[0])).some(e=>e.phase==='front-commit'&&e.inputSequence>=input.sequence&&JSON.parse(e.detail).sceneDisposition==='exact-rendered');},null,{timeout:120000});
 await page.waitForFunction(()=>{const d=globalThis.__dorotiResizeDiagnostics,s=JSON.parse(d.snapshot(d.hosts()[0])),p=JSON.parse(d.presenter('doroti-surface'));return p.frontGeneration===s.resizeEpoch.generation;},null,{timeout:120000});
 await page.waitForTimeout(scenario==='button'?250:1500);
 const after=await read(),profiles=await managed(),inputs=await page.evaluate(()=>globalThis.__stateInputs);
 const trace=after.trace.filter(e=>e.timestampMicroseconds/1000>=before.time);
 const fronts=trace.filter(e=>e.phase==='front-commit'&&e.source==='worker-direct-surface');
 const input=inputs[0],causal=fronts.find(e=>e.inputSequence>=input?.sequence&&JSON.parse(e.detail).sceneDisposition==='exact-rendered');
 const callbacks=trace.filter(e=>e.phase==='framework-frame').map(e=>e.durationMicroseconds/1000);
 const output={label,scenario,detailed,errors,before,after,profileBefore,profiles,inputs,steps,end,
  onsetMs:causal?causal.timestampMicroseconds/1000-input.time:null,callback:stats(callbacks),
  activeFronts:fronts.filter(e=>e.timestampMicroseconds/1000<=end).length,
  selection:scenario==='button'?await page.getByRole('radio',{name:'Week',exact:true}).getAttribute('aria-checked'):null,
  limitation:'Sequential CDP input and scene commits; not physical display latency. Detailed exports excluded from action window.'};
 await mkdir('artifacts/state-resize',{recursive:true});await writeFile(`artifacts/state-resize/${label}.json`,JSON.stringify(output,null,2));
 console.log(JSON.stringify({label,scenario,errors,onsetMs:output.onsetMs,callback:output.callback,activeFronts:output.activeFronts,selection:output.selection}));
 if(errors.length)process.exitCode=1;
} finally {await browser.close();clearTimeout(deadline);}
