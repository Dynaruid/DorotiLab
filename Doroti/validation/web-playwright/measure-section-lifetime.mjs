import { chromium } from '@playwright/test';
import { mkdir, writeFile } from 'node:fs/promises';

const label=process.argv[2];
if (!/^[\w-]+$/.test(label)) throw Error('Simple label required');
const deadline=setTimeout(()=>{console.error('20-minute timeout');process.exit(1);},1200000);
const browser=await chromium.launch({headless:true,args:['--enable-gpu-rasterization','--ignore-gpu-blocklist','--use-angle=default']});
const samples=[],errors=[];
try {
 const page=await browser.newPage({viewport:{width:1280,height:900}});
 page.on('pageerror',e=>errors.push(String(e)));
 await page.goto((process.env.DOROTI_WEB_BASE_URL??'http://127.0.0.1:5088')+'/?dorotiTestbedMode=sample&dorotiResizeDiagnostics=1'+(process.env.DOROTI_PERF_QUERY??''));
 const ready=async width=>page.waitForFunction(width=>{const d=globalThis.__dorotiResizeDiagnostics;if(!d?.hosts().length)return false;
  const s=JSON.parse(d.snapshot(d.hosts()[0])),p=JSON.parse(d.presenter('doroti-surface'));
  return s.logicalWidth===width && p.frontRequestId>0 && s.resizeEpoch.generation===p.frontGeneration;},width,{timeout:120000});
 await ready(1280); await page.waitForTimeout(2000);
 for(let cycle=0;cycle<20;cycle++) {
  for(const x of [500,1000]) {await page.mouse.move(x,600);await page.mouse.wheel(0,cycle%2===0?1200:-700);await page.waitForTimeout(350);}
  for(const width of [800,1280]) {await page.setViewportSize({width,height:900});await ready(width);await page.waitForTimeout(1200);}
  const all=await Promise.all(page.workers().map(w=>w.evaluate(()=>globalThis.__dorotiDirectDiagnostics?.()??null)));
  const m=all.find(p=>p?.managed)?.managed,p=m?.profile,s=m?.frame?.Skia;
  samples.push({cycle,managedHeapBytes:p?.managedHeapBytes??null,allocatedBytes:p?.managedAllocatedBytes??null,
   collections:p?.gcCollections??null,pictures:s?.PictureRasterCacheEntries??null,imageFilterSurfaces:s?.ActiveImageFilterSurfaces??null,
   failed:s?.Failed??null});
 }
 await mkdir('artifacts/section-lifetime',{recursive:true});
 await writeFile(`artifacts/section-lifetime/${label}.json`,JSON.stringify({label,url:page.url(),browser:browser.version(),samples,errors,
  limitations:'Managed heap estimates are not forced-GC live bytes. GC pause, process private bytes and WASM capacity notMeasured; physical display notVerified.'},null,2));
 console.log(JSON.stringify({label,errors,lastFive:samples.slice(-5)}));
 if(errors.length || samples.some(s=>s.failed>0))process.exitCode=1;
} catch(error) {
 await mkdir('artifacts/section-lifetime',{recursive:true});
 await writeFile(`artifacts/section-lifetime/${label}.failure.json`,JSON.stringify({label,samples,errors,error:String(error)},null,2));
 throw error;
} finally {await browser.close();clearTimeout(deadline);}
