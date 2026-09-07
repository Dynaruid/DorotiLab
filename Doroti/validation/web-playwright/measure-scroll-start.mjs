import { chromium } from '@playwright/test';
import { mkdir, writeFile } from 'node:fs/promises';

const label=process.argv[2]??'scroll',x=Number(process.argv[3]??1000);
if(!/^[\w-]+$/.test(label))throw Error('simple label required');
const distance=Number(process.argv[4]??120);
if(!Number.isFinite(x)||!Number.isFinite(distance)||distance<=0)throw Error('finite x and positive distance required');
const detailed=!process.argv.includes('--minimal');
const deadline=setTimeout(()=>{console.error('20-minute timeout');process.exit(1);},1200000);
const browser=await chromium.launch({headless:true,args:['--enable-gpu-rasterization','--ignore-gpu-blocklist','--use-angle=default']});
try {
 const page=await browser.newPage({viewport:{width:1280,height:900}}),errors=[];
 page.on('pageerror',e=>errors.push(String(e)));
 await page.goto((process.env.DOROTI_WEB_BASE_URL??'http://127.0.0.1:5088')+`/?dorotiTestbedMode=sample&dorotiResizeDiagnostics=${detailed?1:0}&dorotiInputMarkers=1`);
 await page.waitForFunction(()=>globalThis.__dorotiResizeDiagnostics&&JSON.parse(globalThis.__dorotiResizeDiagnostics.presenter('doroti-surface')).frontRequestId>0,null,{timeout:120000});
 await page.waitForTimeout(10000);
 await page.evaluate(()=>{
  globalThis.__scrollInputs=[];
  document.querySelector('.doroti-root').addEventListener('wheel',e=>{
   const time=e.timeStamp;
   queueMicrotask(()=>{const d=globalThis.__dorotiResizeDiagnostics;globalThis.__scrollInputs.push({time,sequence:JSON.parse(d.snapshot(d.hosts()[0])).inputSequence});});
  });
 });
 const read=()=>page.evaluate(()=>{const d=globalThis.__dorotiResizeDiagnostics,id=d.hosts()[0];return {time:performance.now(),snapshot:JSON.parse(d.snapshot(id)),trace:JSON.parse(d.capture(id))};});
 const profile=async()=>detailed?(await Promise.all(page.workers().map(w=>w.evaluate(()=>globalThis.__dorotiDirectDiagnostics?.()??null)))).filter(Boolean):[];
 const steps=[];
 let imageEntry=null;
 await page.mouse.move(x,600);
 if(process.argv.includes('--image')) {
  const control=page.getByRole('button',{name:'Extract colors',exact:true});
  for(let i=0;i<60;i++) {
   const entryBefore=await profile(),entryClock=await read();
   const b=await control.count()?await control.boundingBox():null;
   if(b&&b.y>200&&b.y+b.height<890)break;
   await page.mouse.wheel(0,b&&b.y<200?-200:300);await page.waitForTimeout(350);
   const entryAfter=await profile();
   if(!imageEntry && !entryBefore[0]?.managed?.profile.entries.some(e=>e.Type==='MaterialSample.SampleImageDemo'&&e.Calls) && entryAfter[0]?.managed?.profile.entries.some(e=>e.Type==='MaterialSample.SampleImageDemo'&&e.Calls)) { imageEntry={before:entryClock,profileBefore:entryBefore,profiles:entryAfter,after:await read()}; console.log('captured first Image demo entry'); }
   if(i===59) { console.log(await page.locator('[role=button]').evaluateAll(ns=>ns.map(n=>({name:n.textContent,label:n.getAttribute('aria-label'),rect:n.getBoundingClientRect().toJSON()})))); throw Error('Image demo did not become visible'); }
  }
  await page.waitForTimeout(3000);
  await mkdir('artifacts/scroll-start',{recursive:true});
  await page.screenshot({path:`artifacts/scroll-start/${label}-image.png`});
  await page.mouse.wheel(0,-160);await page.waitForTimeout(800);
  await page.mouse.wheel(0,2*distance);await page.waitForTimeout(1000);
  await page.mouse.wheel(0,-2*distance);await page.waitForTimeout(1000);
 }
 if(process.argv.includes('--middle')) {
  for(let i=0;i<6;i++){await page.mouse.wheel(0,200);await page.waitForTimeout(350);}
  await page.mouse.wheel(0,2*distance);await page.waitForTimeout(1000);
  await page.mouse.wheel(0,-2*distance);await page.waitForTimeout(1000);
 }
 await page.waitForTimeout(1500);
 for(const [name,delta] of [['first',distance],['continue',distance],['return',-2*distance],['restart',distance]]) {
  if(name==='restart')await page.waitForTimeout(5000);
  const profileBefore=await profile(),before=await read();
  const inputIndex=await page.evaluate(()=>globalThis.__scrollInputs.length);
  await page.mouse.wheel(0,delta);
  await page.waitForFunction(i=>{const input=globalThis.__scrollInputs[i],d=globalThis.__dorotiResizeDiagnostics;return input&&JSON.parse(d.capture(d.hosts()[0])).some(e=>e.phase==='front-commit'&&e.inputSequence>=input.sequence&&JSON.parse(e.detail).sceneDisposition==='exact-rendered');},inputIndex,{timeout:120000});
  await page.waitForTimeout(500);
  const after=await read(),profiles=await profile(),input=await page.evaluate(i=>globalThis.__scrollInputs[i],inputIndex);
  const fronts=after.trace.filter(e=>e.phase==='front-commit'&&e.inputSequence>=input.sequence&&JSON.parse(e.detail).sceneDisposition==='exact-rendered');
  const frames=after.trace.filter(e=>e.phase==='framework-frame'&&e.timestampMicroseconds/1000>=before.time);
  const old=new Map((profileBefore[0]?.managed?.profile.entries??[]).map(e=>[e.Id,e]));
  const top=(profiles[0]?.managed?.profile.entries??[]).map(e=>({type:e.Type,kind:e.Kind,calls:e.Calls-(old.get(e.Id)?.Calls??0),selfMs:(e.SelfMicroseconds-(old.get(e.Id)?.SelfMicroseconds??0))/1000})).filter(e=>e.calls).sort((a,b)=>b.selfMs-a.selfMs).slice(0,12);
  const step={name,delta,input,before,after,profileBefore,profiles,onsetMs:fronts[0].timestampMicroseconds/1000-input.time,callbackMax:frames.length?Math.max(...frames.map(e=>e.durationMicroseconds/1000)):null,top,imageControlMounted:await page.getByRole('radio',{name:'Local image',exact:true}).count()};
  steps.push(step);console.log(JSON.stringify({label,name,onsetMs:step.onsetMs,callbackMax:step.callbackMax,imageControlMounted:step.imageControlMounted,top}));
 }
 await mkdir('artifacts/scroll-start',{recursive:true});await writeFile(`artifacts/scroll-start/${label}.json`,JSON.stringify({label,x,detailed,errors,imageEntry,steps,limitation:'Wheel input to new scene commit notification, not scan-out. New and retained sections are distinguished by profile calls.'},null,2));
 if(errors.length)throw Error(errors.join('\n'));
} finally {await browser.close();clearTimeout(deadline);}
