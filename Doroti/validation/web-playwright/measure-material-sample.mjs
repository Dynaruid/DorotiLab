import { chromium } from '@playwright/test';
import { mkdir, writeFile } from 'node:fs/promises';

// Browser CPU/submit evidence only. mouse.wheel includes CDP pacing, so submits
// per wall second are not display FPS. All runs use a 20-minute outer deadline.
const label = process.argv[2] ?? 'sample';
if (!/^[a-zA-Z0-9_-]+$/.test(label)) throw new Error('Use a simple artifact label');
const traced = !process.argv.includes('--no-trace');
const timeout = setTimeout(() => { console.error('20-minute timeout'); process.exit(1); }, 20 * 60 * 1000);
const browser = await chromium.launch({headless:true, args:['--enable-gpu-rasterization','--ignore-gpu-blocklist','--use-angle=default']});
try {
  const page = await browser.newPage({viewport:{width:1280,height:900}});
  const errors=[];
  page.on('pageerror', e=>{errors.push(String(e));console.error(String(e));});
  const managedProbes=[];
  page.on('console', m=>{if(m.type()==='error') {errors.push(m.text());console.error(m.text());} else if(m.text().includes('_PROBE')) {managedProbes.push(m.text()); console.log(m.text());}});
  await page.goto('http://127.0.0.1:5088/?dorotiTestbedMode=sample&dorotiResizeDiagnostics=1'+(traced?'&dorotiCanvasKitTrace=1':'')+(process.env.DOROTI_PERF_QUERY??''));
  await page.waitForFunction(()=>globalThis.__dorotiResizeDiagnostics?.hosts().length, null,{timeout:120000});
  console.log('host ready');
  await page.waitForFunction(()=>JSON.parse(globalThis.__dorotiResizeDiagnostics.presenter('doroti-surface')).frontRequestId > 0,null,{timeout:120000});
  console.log('front ready');
  // The first submitted front may precede the expensive initial sample layout.
  // Exclude startup from counter deltas as well as from the trace window.
  await page.waitForTimeout(process.argv.includes('--cold') ? 0 : 10000);
  const progress = process.argv.includes('--progress');
  if (progress) {
    const startProgress = page.locator('[role="button"][aria-description="Start progress"]');
    let bounds;
    for (let i = 0; i < 40; i++) {
      bounds = await startProgress.count() ? await startProgress.boundingBox() : null;
      if (bounds && bounds.y > 150 && bounds.y + bounds.height < 780) break;
      await page.mouse.move(500, 600); await page.mouse.wheel(0, 200); await page.waitForTimeout(500);
    }
    if (!bounds || bounds.y < 150 || bounds.y + bounds.height >= 780) throw new Error('Progress control not visible');
    await page.mouse.click(bounds.x + bounds.width / 2, bounds.y + bounds.height / 2);
    await page.locator('[aria-description="Stop progress"]').waitFor({state:'attached'});
    await page.mouse.move(20, 40);
    await page.waitForTimeout(3000);
  }
  const read=()=>page.evaluate(()=>{
    const d=globalThis.__dorotiResizeDiagnostics, id=d.hosts()[0], snapshot=JSON.parse(d.snapshot(id));
    return {time:performance.timeOrigin+performance.now(),timeOrigin:performance.timeOrigin,snapshot,presenter:JSON.parse(d.presenter(snapshot.canvasId))};
  });
  const before=await read();
  const start=Date.now();
  const sweep = process.argv.includes('--sweep');
  if (progress) await page.waitForTimeout(6000);
  else for (const column of sweep ? [500,1000] : [500]) {
    await page.mouse.move(column,600);
    const steps = sweep ? 180 : 120;
    for(let i=0;i<steps;i++) {await page.mouse.wheel(0,(i<steps/2?1:-1)*(sweep?100:12)); await page.waitForTimeout(16);}
  }
  const after=await read();
  const stages=traced?await page.evaluate(()=>globalThis.__dorotiCanvasKitExperiment?.collect()??null):null;
  const resizeTrace=await page.evaluate(()=>{
    const d=globalThis.__dorotiResizeDiagnostics;
    return JSON.parse(d.capture(d.hosts()[0]));
  });
  const stats = values => {
    values.sort((a,b)=>a-b);
    return {count:values.length,median:values.length?values[Math.floor(values.length/2)]:null,
      p95:values.length?values[Math.ceil(values.length*.95)-1]:null,max:values.length?values.at(-1):null};
  };
  const ui=stages?.ui?.entries??[];
  const ends=new Map(ui.filter(e=>e.stage==='frame-end').map(e=>[e.detail.callbackId,e]));
  const frameCosts=ui.filter(e=>e.stage==='frame-start' && e.time>=before.time && ends.get(e.detail.callbackId)?.time<=after.time)
    .map(e=>ends.get(e.detail.callbackId).time-e.time);
  const uiFrameMilliseconds=stats(frameCosts);
  const directCommits=resizeTrace.filter(e=>e.phase==='front-commit' && e.source==='worker-direct-surface' &&
    e.timestampMicroseconds/1000+after.timeOrigin>=before.time && e.timestampMicroseconds/1000+after.timeOrigin<=after.time);
  const directSurfaceMilliseconds=stats(directCommits.map(e=>JSON.parse(e.detail).managedSurfaceMicroseconds/1000));
  const directCommitIntervals=stats(directCommits.slice(1).map((e,i)=>(e.timestampMicroseconds-directCommits[i].timestampMicroseconds)/1000));
  const result={label,traced,sweep,progress,cold:process.argv.includes('--cold'),elapsed:Date.now()-start,errors,managedProbes,before,after,stages,resizeTrace,uiFrameMilliseconds,directSurfaceMilliseconds,directCommitIntervals,
    limitation:'CPU and GPU-submit notification evidence, not display FPS or physical scan-out; CDP wheel input is paced by the driver.'};
  await mkdir('artifacts/sample-perf',{recursive:true});
  await writeFile(`artifacts/sample-perf/${label}.json`,JSON.stringify(result,null,2));
  if(after.presenter.mode==='worker-direct-webgl') {
    console.log(JSON.stringify({label,errors,elapsed:after.time-before.time,mode:after.presenter.mode,commits:directCommits.length,directSurfaceMilliseconds,directCommitIntervals,gpu:after.snapshot.gpu},null,2));
  } else {
  const a=before.presenter.uiDiagnostics.frameTimings,b=after.presenter.uiDiagnostics.frameTimings;
  const ra=before.presenter.rasterDiagnostics,rb=after.presenter.rasterDiagnostics;
  console.log(JSON.stringify({label,errors,elapsed:after.time-before.time,frames:b.count-a.count,uiFrameMilliseconds,dispatchMean:(b.dispatchTotalMilliseconds-a.dispatchTotalMilliseconds)/(b.count-a.count),rasterMean:(rb.timings.replayTotalMilliseconds-ra.timings.replayTotalMilliseconds)/(rb.timings.replayCount-ra.timings.replayCount),submits:rb.submittedScenes-ra.submittedScenes,failed:rb.failedScenes,mode:after.presenter.mode,gpu:after.snapshot.gpu},null,2));
  }
  if (!process.argv.includes('--no-screenshot')) await page.screenshot({path:`artifacts/${label}.png`});
} finally {await browser.close();clearTimeout(timeout);}
