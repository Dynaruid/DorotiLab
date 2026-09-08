import { chromium } from '@playwright/test';
import { mkdir, writeFile } from 'node:fs/promises';

// Browser CPU/submit evidence only. mouse.wheel includes CDP pacing, so submits
// per wall second are not display FPS. All runs use a 20-minute outer deadline.
const label = process.argv[2] ?? 'sample';
if (!/^[a-zA-Z0-9_-]+$/.test(label)) throw new Error('Use a simple artifact label');
const traced = !process.argv.includes('--no-trace');
const onset = process.argv.includes('--onset');
const diagnostics = !process.argv.includes('--no-diagnostics');
const timeout = setTimeout(() => { console.error('20-minute timeout'); process.exit(1); }, 20 * 60 * 1000);
const browser = await chromium.launch({headless:true, args:['--enable-gpu-rasterization','--ignore-gpu-blocklist','--use-angle=default']});
try {
  const page = await browser.newPage({viewport:{width:1280,height:900}});
  const cpuSession=process.argv.includes('--cpu-profile')?await page.context().newCDPSession(page):null;
  const errors=[];
  page.on('pageerror', e=>{errors.push(String(e));console.error(String(e));});
  const managedProbes=[];
  page.on('console', m=>{if(m.type()==='error') {errors.push(m.text());console.error(m.text());} else if(m.text().includes('_PROBE')) {managedProbes.push(m.text()); console.log(m.text());}});
  await page.goto((process.env.DOROTI_WEB_BASE_URL??'http://127.0.0.1:5088')+'/?dorotiTestbedMode=sample&dorotiResizeDiagnostics='+(diagnostics?'1':'0')+(traced?'&dorotiCanvasKitTrace=1':'')+'&dorotiInputMarkers=1'+(process.env.DOROTI_PERF_QUERY??''));
  await page.waitForFunction(()=>globalThis.__dorotiResizeDiagnostics?.hosts().length, null,{timeout:120000});
  console.log('host ready');
  await page.waitForFunction(()=>JSON.parse(globalThis.__dorotiResizeDiagnostics.presenter('doroti-surface')).frontRequestId > 0,null,{timeout:120000});
  console.log('front ready');
  await page.evaluate(()=>{
    const stimuli=[];
    const root=document.querySelector('.doroti-root');
    for (const kind of ['pointerup','wheel']) root.addEventListener(kind,event=>{
      const time=performance.timeOrigin+event.timeStamp;
      queueMicrotask(()=>{
        const d=globalThis.__dorotiResizeDiagnostics;
        const sequence=JSON.parse(d.snapshot(d.hosts()[0])).inputSequence;
        if(stimuli.length<512) stimuli.push({kind,time,sequence,deltaY:event.deltaY??null,x:event.clientX});
      });
    });
    globalThis.__dorotiPerfStimuli=stimuli;
  });
  // The first submitted front may precede the expensive initial sample layout.
  // Exclude startup from counter deltas as well as from the trace window.
  await page.waitForTimeout(process.argv.includes('--cold') ? 0 : 10000);
  const progress = process.argv.includes('--progress');
  const restart = process.argv.includes('--restart');
  const read=()=>page.evaluate(()=>{
    const d=globalThis.__dorotiResizeDiagnostics, id=d.hosts()[0], snapshot=JSON.parse(d.snapshot(id));
    return {time:performance.timeOrigin+performance.now(),timeOrigin:performance.timeOrigin,snapshot,presenter:JSON.parse(d.presenter(snapshot.canvasId))};
  });
  const profileRead=async()=>diagnostics?(await Promise.all(page.workers().map(worker=>worker.evaluate(()=>globalThis.__dorotiDirectDiagnostics?.()??null)))).filter(Boolean):[];
  let profileBefore;
  let onsetDiagnostics;
  await page.evaluate(()=>{
    globalThis.__dorotiMainTasks=[];
    for(const type of ['longtask','long-animation-frame']) if(PerformanceObserver.supportedEntryTypes.includes(type))
      new PerformanceObserver(list=>{for(const entry of list.getEntries()) if(globalThis.__dorotiMainTasks.length<2048) globalThis.__dorotiMainTasks.push(entry.toJSON());}).observe({type,buffered:true});
  });
  let onsetBefore;
  if (progress) {
    const startProgress = page.locator('[role="button"][aria-description="Start progress"]');
    let bounds;
    for (let i = 0; i < 40; i++) {
      bounds = await startProgress.count() ? await startProgress.boundingBox() : null;
      if (bounds && bounds.y > 150 && bounds.y + bounds.height < 780) break;
      await page.mouse.move(500, 600); await page.mouse.wheel(0, 200); await page.waitForTimeout(500);
    }
    if (!bounds || bounds.y < 150 || bounds.y + bounds.height >= 780) throw new Error('Progress control not visible');
    // Scrolling can still have an outstanding managed frame after semantics
    // first reports an in-range target. Settle setup and refresh its hit rect.
    await page.mouse.move(20, 40);
    await page.waitForTimeout(2000);
    bounds = await startProgress.boundingBox();
    if (!bounds || bounds.y < 150 || bounds.y + bounds.height >= 780) throw new Error('Settled progress control not visible');
    if (restart) {
      await page.mouse.click(bounds.x + bounds.width / 2, bounds.y + bounds.height / 2);
      await page.locator('[aria-description="Stop progress"]').waitFor({state:'attached'});
      await page.mouse.move(20,40);
      await page.waitForTimeout(1000);
      const stopBounds=await page.locator('[aria-description="Stop progress"]').boundingBox();
      await page.mouse.click(stopBounds.x+stopBounds.width/2,stopBounds.y+stopBounds.height/2);
      await startProgress.waitFor({state:'attached'});
      await page.mouse.move(20,40);
      bounds=await startProgress.boundingBox();
    }
    if (onset) {
      if (!process.argv.includes('--cold')) await page.waitForTimeout(5000);
      if(cpuSession) await cpuSession.send('Tracing.start',{categories:'devtools.timeline,v8,disabled-by-default-v8.cpu_profiler',transferMode:'ReturnAsStream'});
      profileBefore = await profileRead();
      onsetBefore = await read();
    }
    bounds = await startProgress.boundingBox();
    if (!bounds) throw new Error('Progress target lost before input');
    await page.mouse.click(bounds.x + bounds.width / 2, bounds.y + bounds.height / 2);
    await page.locator('[aria-description="Stop progress"]').waitFor({state:'attached'});
    if (onset && diagnostics) {
      // Exporting managed diagnostics before the causal front can itself delay
      // that front. Keep serialization outside the measured response window.
      if (onsetBefore.presenter.mode === 'worker-direct-webgl') await page.waitForFunction(() => {
        const stimulus=globalThis.__dorotiPerfStimuli.filter(s=>s.kind==='pointerup').at(-1);
        const d=globalThis.__dorotiResizeDiagnostics;
        return stimulus && JSON.parse(d.capture(d.hosts()[0])).some(e=>e.phase==='front-commit' &&
          e.inputSequence>=stimulus.sequence && JSON.parse(e.detail).sceneDisposition==='exact-rendered');
      },null,{timeout:120000});
      onsetDiagnostics = await profileRead();
    }
    await page.mouse.move(20, 40);
    if (!onset) await page.waitForTimeout(3000);
  }
  const before=onsetBefore??await read();
  const start=Date.now();
  const sweepSegments=[];
  const sweep = process.argv.includes('--sweep');
  if (progress) await page.waitForTimeout(6000);
  else for (const column of sweep ? [500,1000] : [500]) {
    await page.mouse.move(column,600);
    const steps = sweep ? 180 : 120;
    let segmentBefore=await read();
    for(let i=0;i<steps;i++) {
      await page.mouse.wheel(0,(i<steps/2?1:-1)*(sweep?100:12)); await page.waitForTimeout(16);
      if(sweep && (i===0 || i===steps/2)) {
        await page.waitForTimeout(200);
        sweepSegments.push({column,visit:i===0?'new-entry':'revisit-entry',before:segmentBefore,after:await read(),diagnostics:await profileRead()});
      }
      if(i===steps/2-1 || i===steps-1) {
        const segmentAfter=await read();
        sweepSegments.push({column,visit:i===steps/2-1?'new':'revisit',before:segmentBefore,after:segmentAfter,diagnostics:await profileRead()});
        segmentBefore=await read();
      }
    }
  }
  const after=await read();
  const directDiagnostics=(await Promise.all(page.workers().map(worker=>worker.evaluate(()=>
    globalThis.__dorotiDirectDiagnostics?.()??null)))).filter(Boolean);
  const stimuli=await page.evaluate(()=>globalThis.__dorotiPerfStimuli);
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
  const steadyCommits=directCommits.filter(e=>e.timestampMicroseconds/1000+after.timeOrigin>=before.time+1000 &&
    e.timestampMicroseconds/1000+after.timeOrigin<=before.time+6000);
  const steadyCommitIntervals=stats(steadyCommits.slice(1).map((e,i)=>(e.timestampMicroseconds-steadyCommits[i].timestampMicroseconds)/1000));
  const stimulus=stimuli.find(s=>s.time>=before.time && s.kind===(progress?'pointerup':'wheel'));
  const rasters=[...(onsetDiagnostics?.[0]?.managed?.frame?.Skia?.Trace??[]),...(directDiagnostics[0]?.managed?.frame?.Skia?.Trace??[])].filter(e=>e.Phase===13 && !e.Reason);
  const causalCommit=stimulus&&directCommits.find(commit=>diagnostics ? rasters.some(frame=>
    frame.CausalFrameId===commit.requestId && frame.InputSequence>=stimulus.sequence) :
    commit.inputSequence>=stimulus.sequence && JSON.parse(commit.detail).sceneDisposition==='exact-rendered');
  const causalOnset={stimulus,requestId:causalCommit?.requestId??null,
    milliseconds:causalCommit?causalCommit.timestampMicroseconds/1000+after.timeOrigin-stimulus.time:null,
    limitation:'New scene correlated to the triggering input sequence; commit notification, not pixel/scan-out latency.'};
  const frameworkFrames=resizeTrace.filter(e=>e.phase==='framework-frame' &&
    e.timestampMicroseconds/1000+after.timeOrigin>=before.time &&
    (e.timestampMicroseconds+e.durationMicroseconds)/1000+after.timeOrigin<=after.time);
  const frameworkFrameMilliseconds=stats(frameworkFrames.map(e=>e.durationMicroseconds/1000));
  const onsetWindows=onset&&diagnostics?[100,500,1000,6000].map(milliseconds=>{
    const entries=directCommits.filter(e=>e.timestampMicroseconds/1000+after.timeOrigin<=before.time+milliseconds);
    return {milliseconds,commits:entries.length,frameworkLongTasks:frameworkFrames.filter(e=>e.durationMicroseconds>50000 && e.timestampMicroseconds/1000+after.timeOrigin<before.time+milliseconds).length,surfaceMilliseconds:stats(entries.map(e=>JSON.parse(e.detail).managedSurfaceMicroseconds/1000)),
      notificationDelayMilliseconds:entries.length?entries[0].timestampMicroseconds/1000+after.timeOrigin-before.time:null};
  }):null;
  const mainTasks=await page.evaluate(()=>globalThis.__dorotiMainTasks);
  const result={sweepSegments,onsetDiagnostics,profileBefore,mainTasks,label,traced,diagnostics,sweep,progress,restart,onset,onsetWindows,causalOnset,stimuli,directDiagnostics,frameworkFrameMilliseconds,steadyCommitIntervals,cold:process.argv.includes('--cold'),elapsed:Date.now()-start,errors,managedProbes,before,after,stages,resizeTrace,uiFrameMilliseconds,directSurfaceMilliseconds,directCommitIntervals,
    limitation:'CPU and GPU-submit notification evidence, not display FPS or physical scan-out; CDP wheel input is paced by the driver.'};
  await mkdir('artifacts/sample-perf',{recursive:true});
  if(cpuSession) {
    const complete=new Promise(resolve=>cpuSession.once('Tracing.tracingComplete',resolve));
    await cpuSession.send('Tracing.end'); const {stream}=await complete;
    let contents=''; while(true) {const chunk=await cpuSession.send('IO.read',{handle:stream});contents+=chunk.data;if(chunk.eof)break;}
    await cpuSession.send('IO.close',{handle:stream});
    await writeFile(`artifacts/sample-perf/${label}.cpu-trace.json`,contents);
  }
  await writeFile(`artifacts/sample-perf/${label}.json`,JSON.stringify(result,null,2));
  if(after.presenter.mode==='worker-direct-webgl') {
    console.log(JSON.stringify({label,errors,elapsed:after.time-before.time,mode:after.presenter.mode,commits:diagnostics?directCommits.length:null,frontRequestAdvance:after.presenter.frontRequestId-before.presenter.frontRequestId,directSurfaceMilliseconds,directCommitIntervals,frameworkFrameMilliseconds,onsetWindows,work:directDiagnostics[0]?.managed?.frame?.Skia?.Work,gpu:after.snapshot.gpu},null,2));
  } else {
  const a=before.presenter.uiDiagnostics.frameTimings,b=after.presenter.uiDiagnostics.frameTimings;
  const ra=before.presenter.rasterDiagnostics,rb=after.presenter.rasterDiagnostics;
  console.log(JSON.stringify({label,errors,elapsed:after.time-before.time,frames:b.count-a.count,uiFrameMilliseconds,dispatchMean:(b.dispatchTotalMilliseconds-a.dispatchTotalMilliseconds)/(b.count-a.count),rasterMean:(rb.timings.replayTotalMilliseconds-ra.timings.replayTotalMilliseconds)/(rb.timings.replayCount-ra.timings.replayCount),submits:rb.submittedScenes-ra.submittedScenes,failed:rb.failedScenes,mode:after.presenter.mode,gpu:after.snapshot.gpu},null,2));
  }
  if (!process.argv.includes('--no-screenshot')) await page.screenshot({path:`artifacts/${label}.png`});
} catch(error) {
  await mkdir('artifacts/sample-perf',{recursive:true});
  await writeFile(`artifacts/sample-perf/${label}.failure.json`,JSON.stringify({label,error:String(error),stack:error.stack},null,2));
  throw error;
} finally {await browser.close();clearTimeout(timeout);}
