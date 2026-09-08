import { chromium } from '@playwright/test';
import { mkdir, writeFile } from 'node:fs/promises';

const label=process.argv[2];
if(!/^[\w-]+$/.test(label)) throw Error('Simple label required');
const output={label,preparation:[],segments:[],errors:[],limitations:'Detailed sequential CDP workload; no physical display, stable p95, GC pause, or minimally instrumented acceptance.'};
const deadline=setTimeout(()=>{console.error('20-minute timeout');process.exit(1);},1200000);
const browser=await chromium.launch({headless:true,args:['--enable-gpu-rasterization','--ignore-gpu-blocklist','--use-angle=default']});
try {
 const page=await browser.newPage({viewport:{width:1280,height:900},deviceScaleFactor:1});
 page.on('pageerror',error=>output.errors.push(String(error)));
 const url=(process.env.DOROTI_WEB_BASE_URL??'http://127.0.0.1:5189')+'/?dorotiTestbedMode=sample&dorotiResizeDiagnostics=1&dorotiInputMarkers=1';
 await page.goto(url);
 const read=()=>page.evaluate(()=>{const d=globalThis.__dorotiResizeDiagnostics,id=d.hosts()[0];return {time:performance.now(),snapshot:JSON.parse(d.snapshot(id)),presenter:JSON.parse(d.presenter('doroti-surface')),trace:JSON.parse(d.capture(id))};});
 const managed=async()=>{
  const snapshots=await Promise.all(page.workers().map(worker=>worker.evaluate(()=>globalThis.__dorotiDirectDiagnostics?.()??null)));
  const result=snapshots.find(value=>value?.managed);
  if(!result)throw Error('Managed diagnostic export unavailable');
  return result;
 };
 const ready=width=>page.waitForFunction(width=>{
  const d=globalThis.__dorotiResizeDiagnostics;if(!d?.hosts().length)return false;
  const s=JSON.parse(d.snapshot(d.hosts()[0])),p=JSON.parse(d.presenter('doroti-surface'));
  return s.logicalWidth===width&&p.frontRequestId>0&&p.frontGeneration===s.resizeEpoch.generation&&
   JSON.parse(d.capture(d.hosts()[0])).some(e=>e.phase==='front-commit'&&e.detail&&
    JSON.parse(e.detail).generation===s.resizeEpoch.generation&&JSON.parse(e.detail).sceneDisposition==='exact-rendered');
 },width,{timeout:120000});
 await ready(1280);await page.waitForTimeout(10000);
 output.initial=await managed();
 for(const x of [500,1000]) {
  await page.mouse.move(x,700);
  // Fixed preparation: do not coalesce several wheel steps before materialization.
  for(let i=0;i<12;i++) {await page.mouse.wheel(0,600);await page.waitForTimeout(750);}
  const bottom=await managed();
  await page.mouse.wheel(0,-20000);await page.waitForTimeout(1000);
  output.preparation.push({columnX:x,forwardInputs:12,delta:600,waitMs:750,returnDelta:-20000,
   visitedAtBottom:bottom.managed.components.visitedSections});
 }
 await page.mouse.move(20,40);await page.waitForTimeout(2000);
 const prepared=await managed();output.prepared=prepared;
 const ids=prepared.managed.components.visitedSections;
 if(ids.length!==29||ids.some((id,i)=>id!==i))throw Error('Expected all section IDs 0..28, got '+JSON.stringify(ids));
 output.environment={url,browser:browser.version(),viewport:{width:1280,height:900},dpr:1};
 for(const [scenario,widths] of [
  ['resize-wide',[1240,1200,1160,1120,1160,1200,1240,1280]],
  ['resize-break',[980,900,800,1100,1400,900,1100,1280]],
 ]) {
  await ready(1280);await page.waitForTimeout(2000);
  const profileBefore=await managed(),before=await read(),steps=[];
  for(const width of widths) {
   await page.setViewportSize({width,height:900});
   steps.push(await page.evaluate(width=>{const d=globalThis.__dorotiResizeDiagnostics;return {width,time:performance.now(),observed:JSON.parse(d.snapshot(d.hosts()[0])).resizeEpoch};},width));
   await page.waitForTimeout(45);
  }
  const end=await page.evaluate(()=>performance.now());
  await ready(1280);await page.waitForTimeout(1500);
  const after=await read(),profileAfter=await managed();
  const events=after.trace.filter(e=>e.timestampMicroseconds/1000>=before.time);
  const callbacks=events.filter(e=>e.phase==='framework-frame').map(e=>e.durationMicroseconds/1000);
  const generation=after.snapshot.resizeEpoch.generation;
  const exact=events.filter(e=>e.phase==='front-commit'&&e.detail&&JSON.parse(e.detail).generation===generation&&JSON.parse(e.detail).sceneDisposition==='exact-rendered');
  if(!exact.length||!callbacks.length)throw Error('Missing exact final front or callback evidence');
  const a=profileBefore.managed,b=profileAfter.managed;
  const componentDelta=b.components.values.map((value,i)=>value-a.components.values[i]);
  const segment={scenario,before,after,profileBefore,profileAfter,steps,end,callbacksMs:callbacks,
   callbackMaxMs:Math.max(...callbacks),callbackTotalMs:callbacks.reduce((sum,value)=>sum+value,0),
   finalSettleMs:exact[0].timestampMicroseconds/1000-end,exactFinalGeneration:generation,
   componentDelta,managedAllocatedBytes:b.profile.managedAllocatedBytes-a.profile.managedAllocatedBytes,
   heapEstimateBefore:a.profile.managedHeapBytes,heapEstimateAfter:b.profile.managedHeapBytes,
   gcCollections:b.profile.gcCollections.map((value,i)=>value-a.profile.gcCollections[i])};
  output.segments.push(segment);
  console.log(JSON.stringify({label,scenario,maxMs:segment.callbackMaxMs,totalMs:segment.callbackTotalMs,settleMs:segment.finalSettleMs,allocated:segment.managedAllocatedBytes,mapCalls:componentDelta[0],mapBytes:componentDelta[2]}));
 }
 if(output.errors.length)throw Error('Runtime errors: '+output.errors.join('; '));
 output.status='PASS';
} catch(error) {
 output.status='FAIL';output.failure=String(error);output.stack=error.stack;process.exitCode=1;
 console.error(output.failure);
} finally {
 await mkdir('artifacts/indexed-hamt',{recursive:true});
 await writeFile(`artifacts/indexed-hamt/${label}.json`,JSON.stringify(output,null,2));
 await browser.close();clearTimeout(deadline);
}
