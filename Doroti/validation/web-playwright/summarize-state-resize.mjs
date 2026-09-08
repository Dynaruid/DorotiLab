import { readdir, readFile, writeFile } from 'node:fs/promises';
import { measureResizeFollowing } from './tests/helpers/resize-following.ts';

const root='artifacts/state-resize', results=[];
for(const file of (await readdir(root)).filter(f=>f.endsWith('.json')&&f!=='summary.json')) {
 const p=JSON.parse(await readFile(`${root}/${file}`,'utf8'));
 if(!Array.isArray(p.profiles)||!p.after?.trace)continue; // Failure records and derived summaries are not latency samples.
 const m=p.profiles[0]?.managed, start=p.profileBefore[0]?.managed?.clockMicroseconds;
 const trace=m?.frame?.Skia?.Trace??[], entries=new Map((m?.profile?.entries??[]).map(e=>[e.Id,e]));
 const counts=new Map((m?.work?.Samples??[]).map(s=>[s.Boundary.TraceSequence,s.Totals]));
 const frames=[];
 for(let i=0;i<trace.length;i++) {
  const a=trace[i];if(a.Phase!==7||a.RecordedAtMicroseconds<start)continue;
  let end=trace.findIndex((e,j)=>j>i&&e.Phase===7);if(end<0)end=trace.length;
  const phase=new Map(trace.slice(i,end).map(e=>[e.Phase,e])), spans={};
  for(const [name,x,y] of [['build',7,8],['layout',8,9],['paint',9,10],['semantics',27,28]])
   if(phase.has(x)&&phase.has(y))spans[name]=(phase.get(y).RecordedAtMicroseconds-phase.get(x).RecordedAtMicroseconds)/1000;
  const b=phase.get(8),x=counts.get(a.Sequence),y=b&&counts.get(b.Sequence);
  const work=x&&y?Object.fromEntries(m.work.Names.map((name,j)=>[name,y[j]-x[j]]).filter(([,v])=>v)):null;
  const f=m.profile.frames.find(f=>f[0]===a.Sequence),top=[];
  if(f)for(let j=2;j<22;j+=4)if(f[j+1])top.push({type:entries.get(f[j]).Type,calls:f[j+1],selfMs:f[j+3]/1000});
  frames.push({sequence:a.Sequence,spans,work,top});
 }
 const targets=p.after.trace.filter(e=>e.phase==='target-observed').map(e=>({time:e.timestampMicroseconds/1000,generation:e.epoch.generation,width:e.epoch.logicalWidth,height:e.epoch.logicalHeight,dpr:e.epoch.devicePixelRatio}));
 const fronts=p.after.trace.filter(e=>e.phase==='front-commit'&&e.source==='worker-direct-surface').map(e=>{const d=JSON.parse(e.detail),t=targets.find(t=>t.generation===d.generation);return {time:e.timestampMicroseconds/1000,generation:d.generation,width:e.surfaceWidth/(t?.dpr??1),height:e.surfaceHeight/(t?.dpr??1),dpr:t?.dpr??1};});
 const following=p.scenario.startsWith('resize')?measureResizeFollowing(targets,fronts,p.before.time,p.end):null;
 results.push({label:p.label,scenario:p.scenario,detailed:p.detailed,errors:p.errors,onsetMs:p.onsetMs,callback:p.callback,selection:p.selection,following,frames,
  limitation:'Detailed and minimal corpora remain separate. Frame costs include instrumented work only; physical display latency is not verified.'});
}
await writeFile(`${root}/summary.json`,JSON.stringify(results,null,2));
console.log(JSON.stringify(results.map(r=>({label:r.label,onset:r.onsetMs,callback:r.callback,resize:r.following&&{fronts:r.following.activeFrontCount,gap:r.following.boundaryInclusiveGaps.max,tracking:r.following.caughtUp.p95,settle:r.following.settleFromObserverMilliseconds}})),null,2));
