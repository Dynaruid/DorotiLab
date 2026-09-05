// Run from this directory: node compare-canvaskit-memory.mjs
// Requires the preserved C0/C1 static servers on 5188/5189. No native drags.
import { chromium } from '@playwright/test';
import assert from 'node:assert/strict';
import { execFile } from 'node:child_process';
import { promisify } from 'node:util';
import { mkdir, writeFile } from 'node:fs/promises';
import { createHash } from 'node:crypto';
const execute = promisify(execFile);
const output = process.env.DOROTI_MEMORY_OUTPUT ?? 'artifacts/memory-c0-c1';
const query = '?dorotiResizeDiagnostics=1&dorotiRenderer=worker-canvaskit-webgl&dorotiResizeScheduling=display&dorotiCopyOwnership=owned&dorotiMetricsCoalescing=frame&dorotiEncodingCache=1';
const variants = {
  C0: { port:5188, widgets:'Doroti.Framework.Widgets.2rn4wg03sq.wasm', sha256:'3e71e02f03cfab0c12d011de1ed414c5f545497c81b8b99e1529bd55fc47d4fe' },
  C1: { port:5189, widgets:'Doroti.Framework.Widgets.ed9iv0a1r5.wasm', sha256:'0ea1e4630886de4bc9e1e9c6f7695e88e4e780f60f25321402736003864c2933' },
};
const plan = { startedUtc:new Date().toISOString(), variants, query,
  pairs:[['C0','C1'],['C1','C0'],['C1','C0'],['C0','C1']],
  viewport:{width:960,height:640}, deviceScaleFactor:1, headless:true,
  launchArgs:['--enable-gpu-rasterization','--ignore-gpu-blocklist','--use-angle=default'],
  sizes:[[800,640],[800,720],[960,720],[960,640]], warmupTransitions:20,
  measuredTransitions:120, checkpoints:[0,40,80,120], finalSamples:5,
  sampleDelayMs:300, startupIdleMs:1000, checkpointIdleMs:500,
  primary:'Median of five final natural-GC process PrivateMemorySize64 samples per fresh browser trial; summarize paired differences and ranges.',
  secondary:'WorkingSet64 sum (shared pages may repeat), renderer private bytes, V8 isolate heap, .NET WASM linear-memory capacity; post-V8-GC samples separate.',
  exclusions:'No .NET live managed heap, cumulative WASM managed allocations, GPU VRAM, peak or physical presentation claims. Probe run excluded. No native drags.' };
await mkdir(output,{recursive:true});
await writeFile(`${output}/plan.json`,JSON.stringify(plan,null,2));
let activeBrowser;
const timer=setTimeout(async()=>{console.error('20-minute timeout');try{await activeBrowser?.close();}finally{process.exit(124);}},20*60*1000);
const pause=ms=>new Promise(resolve=>setTimeout(resolve,ms));
async function attach(cdp,target) {
  const {sessionId}=await cdp.send('Target.attachToTarget',{targetId:target.targetId,flatten:false});
  let sequence=0;
  const call=(method,params={})=>new Promise((resolve,reject)=>{
    const id=++sequence;
    const timeout=setTimeout(()=>{cdp.off('Target.receivedMessageFromTarget',listener);reject(new Error(`CDP timeout: ${method}`));},20000);
    const listener=e=>{if(e.sessionId!==sessionId)return;const m=JSON.parse(e.message);if(m.id!==id)return;clearTimeout(timeout);cdp.off('Target.receivedMessageFromTarget',listener);m.error?reject(new Error(JSON.stringify(m.error))):resolve(m.result);};
    cdp.on('Target.receivedMessageFromTarget',listener);
    cdp.send('Target.sendMessageToTarget',{sessionId,message:JSON.stringify({id,method,params})}).catch(error=>{clearTimeout(timeout);cdp.off('Target.receivedMessageFromTarget',listener);reject(error);});
  });
  const evaluated=await call('Runtime.evaluate',{expression:"typeof getDotnetRuntime === 'function'",returnByValue:true});
  return {target,role:target.type==='page'?'main':evaluated.result.value?'ui':'raster',isolateId:(await call('Runtime.getIsolateId')).id,call};
}
async function settled(page,width=960,height=640) {
  const handle=await page.waitForFunction(({width,height})=>{
    const d=globalThis.__dorotiResizeDiagnostics;if(!d)return false;const id=d.hosts()[0];if(!id)return false;
    const s=JSON.parse(d.snapshot(id));const p=JSON.parse(d.presenter(s.canvasId));
    return s.logicalWidth===width&&s.logicalHeight===height&&p.queueDepth===0&&!p.contextLost&&(p.unpairedRequestCount??0)===0&&p.frontGeneration===s.resizeEpoch.generation;
  },{width,height},{timeout:120000});
  await handle.dispose();
}
async function processMemory(cdp) {
  const {processInfo}=await cdp.send('SystemInfo.getProcessInfo');
  const browserId=processInfo.find(p=>p.type==='browser').id;
  assert(Number.isSafeInteger(browserId));
  const ps=`$ErrorActionPreference='Stop'
$rows=Get-CimInstance Win32_Process
$owned=[System.Collections.Generic.HashSet[int]]::new()
[void]$owned.Add(${browserId})
do {$changed=$false; foreach($row in $rows) {if($owned.Contains([int]$row.ParentProcessId)) {if($owned.Add([int]$row.ProcessId)) {$changed=$true}}}} while($changed)
$result=@(foreach($row in $rows) {if($owned.Contains([int]$row.ProcessId)) {$p=Get-Process -Id $row.ProcessId -ErrorAction Stop; [pscustomobject]@{id=$p.Id; name=$row.Name; parentId=$row.ParentProcessId; privateBytes=$p.PrivateMemorySize64; workingSetBytes=$p.WorkingSet64; cpuSeconds=$p.CPU}}})
ConvertTo-Json -InputObject $result -Compress`;
  const {stdout}=await execute('powershell.exe',['-NoProfile','-NonInteractive','-Command',ps],{windowsHide:true,timeout:20000,maxBuffer:1024*1024});
  const rows=JSON.parse(stdout).map(p=>({...p,type:processInfo.find(q=>q.id===p.id)?.type??'utility/other'}));
  assert(rows.some(p=>p.id===browserId));
  assert(processInfo.every(p=>rows.some(row=>row.id===p.id)),'All CDP process IDs must be sampled');
  return {rows,totalPrivateBytes:rows.reduce((s,p)=>s+p.privateBytes,0),totalWorkingSetBytes:rows.reduce((s,p)=>s+p.workingSetBytes,0),rendererPrivateBytes:rows.filter(p=>p.type==='renderer').reduce((s,p)=>s+p.privateBytes,0)};
}
async function sample(cdp,sessions,label) {
  const processes=await processMemory(cdp);
  const isolates=[];
  for(const s of sessions) {
    const heap=await s.call('Runtime.getHeapUsage');
    const evaluated=await s.call('Runtime.evaluate',{expression:"typeof getDotnetRuntime === 'function' ? getDotnetRuntime(0).Module.HEAPU8.buffer.byteLength : null",returnByValue:true});
    assert(!evaluated.exceptionDetails);
    isolates.push({role:s.role,isolateId:s.isolateId,heap,dotnetLinearMemoryBytes:evaluated.result.value});
  }
  return {label,utc:new Date().toISOString(),processes,isolates};
}
const results=[];
try {
  for(let pair=0;pair<plan.pairs.length;pair++) for(const variant of plan.pairs[pair]) {
    const run={pair:pair+1,variant,startedUtc:new Date().toISOString(),samples:[],errors:[]};
    console.log(`Starting pair ${run.pair} ${variant}`);
    activeBrowser=await chromium.launch({headless:plan.headless,args:plan.launchArgs});
    try {
      run.browserVersion=activeBrowser.version();
      const page=await activeBrowser.newPage({viewport:plan.viewport,deviceScaleFactor:plan.deviceScaleFactor});
      page.on('pageerror',e=>run.errors.push(String(e)));
      const variantInfo=variants[variant];
      const assetPromise=page.waitForResponse(r=>r.url().endsWith(`/${variantInfo.widgets}`),{timeout:120000});
      await page.goto(`http://127.0.0.1:${variantInfo.port}/${query}`,{timeout:120000});
      const asset=await assetPromise;
      run.widgetsAsset={url:asset.url(),status:asset.status(),sha256:createHash('sha256').update(await asset.body()).digest('hex')};
      assert.equal(run.widgetsAsset.status,200);assert.equal(run.widgetsAsset.sha256,variantInfo.sha256);
      await settled(page);
      run.gpu=await page.evaluate(()=>{const d=globalThis.__dorotiResizeDiagnostics;return JSON.parse(d.snapshot(d.hosts()[0])).gpu;});
      assert.equal(run.gpu.hardware,true);assert.equal(run.gpu.softwareFallbackUsed,false);assert.equal(run.gpu.api,'webgl2');
      const cdp=await activeBrowser.newBrowserCDPSession();
      const targets=(await cdp.send('Target.getTargets')).targetInfos.filter(t=>['page','worker'].includes(t.type));
      const sessions=[];for(const target of targets)sessions.push(await attach(cdp,target));
      assert.deepEqual(sessions.map(s=>s.role).sort(),['main','raster','ui']);
      assert.equal(new Set(sessions.map(s=>s.isolateId)).size,3);
      run.targets=sessions.map(({target,role,isolateId})=>({target,role,isolateId}));
      await pause(plan.startupIdleMs);
      for(let index=0;index<plan.warmupTransitions+plan.measuredTransitions;index++) {
        if(index===plan.warmupTransitions) {await pause(plan.checkpointIdleMs);run.samples.push(await sample(cdp,sessions,'warm'));}
        const [width,height]=plan.sizes[index%plan.sizes.length];
        await page.setViewportSize({width,height});await settled(page,width,height);
        const measured=index+1-plan.warmupTransitions;
        if(measured>0&&plan.checkpoints.includes(measured)) {
          await pause(plan.checkpointIdleMs);
          run.samples.push(await sample(cdp,sessions,`resize-${measured}`));
        }
      }
      for(let index=0;index<plan.finalSamples;index++) {await pause(plan.sampleDelayMs);run.samples.push(await sample(cdp,sessions,`final-${index+1}`));}
      // Deliberately after all primary samples: V8 collection is not a .NET GC.
      for(const s of sessions) await s.call('HeapProfiler.collectGarbage');
      await pause(plan.checkpointIdleMs);
      run.samples.push(await sample(cdp,sessions,'post-v8-gc'));
      assert.deepEqual(run.errors,[]);
      run.completedTransitions=plan.warmupTransitions+plan.measuredTransitions;
      run.status='PASS';
    } catch(error) {run.status='FAIL';run.failure=String(error);throw error;}
    finally {
      await activeBrowser.close();activeBrowser=null;run.closedUtc=new Date().toISOString();results.push(run);
      await writeFile(`${output}/runs.json`,JSON.stringify({plan,results},null,2));
      console.log(`Finished pair ${run.pair} ${variant}: ${run.status}`);
    }
  }
} finally {await activeBrowser?.close();clearTimeout(timer);}
console.log(`Saved ${results.length} fresh-browser trials to ${output}/runs.json`);
