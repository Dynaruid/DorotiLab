import { readdir, readFile, writeFile } from 'node:fs/promises';
import { measureResizeFollowing } from './tests/helpers/resize-following.ts';

const root = 'artifacts/state-resize';
const runs = [];
for (const file of (await readdir(root)).filter(f => /^wasm-.*\.json$/.test(f) && !f.includes('summary'))) {
  const p = JSON.parse(await readFile(`${root}/${file}`, 'utf8'));
  if (!p.after) continue;
  const before = p.profileBefore[0]?.managed?.profile, after = p.profiles[0]?.managed?.profile;
  const targets = p.after.trace.filter(e => e.phase === 'target-observed').map(e => ({time:e.timestampMicroseconds/1000,
    generation:e.epoch.generation,width:e.epoch.logicalWidth,height:e.epoch.logicalHeight,dpr:e.epoch.devicePixelRatio}));
  const fronts = p.after.trace.filter(e => e.phase === 'front-commit' && e.source === 'worker-direct-surface').map(e => {
    const d = JSON.parse(e.detail), target = targets.find(t => t.generation === d.generation);
    return {time:e.timestampMicroseconds/1000,generation:d.generation,width:e.surfaceWidth/(target?.dpr??1),height:e.surfaceHeight/(target?.dpr??1),dpr:target?.dpr??1};
  });
  runs.push({label:p.label,scenario:p.scenario,detailed:p.detailed,errors:p.errors,onsetMs:p.onsetMs,callback:p.callback,
    following:p.scenario.startsWith('resize') ? measureResizeFollowing(targets,fronts,p.before.time,p.end) : null,
    allocatedBytes:after && before ? after.managedAllocatedBytes-before.managedAllocatedBytes : null,
    managedLiveBefore:before?.managedHeapBytes??null,managedLiveAfter:after?.managedHeapBytes??null,
    collections:after && before ? after.gcCollections.map((v,i)=>v-before.gcCollections[i]) : null,
    gcPause:'notMeasured',wasmCapacity:'notMeasured',physicalDisplay:'notVerified'});
}
await writeFile(`${root}/wasm-structure-summary.json`, JSON.stringify(runs,null,2));
const median = values => {const a=values.filter(Number.isFinite).sort((a,b)=>a-b);return a.length ? a[Math.floor(a.length/2)] : null;};
const groups = new Map();
for (const run of runs) {
  const key=run.label.replace(/-\d+$/,'');
  if (!groups.has(key)) groups.set(key,[]);
  groups.get(key).push(run);
}
console.log(JSON.stringify([...groups].map(([label,items])=>({label,runs:items.length,
  medianCallbackP95:median(items.map(r=>r.callback.p95)),medianOnset:median(items.map(r=>r.onsetMs)),
  medianAllocatedBytes:median(items.map(r=>r.allocatedBytes)),errors:items.flatMap(r=>r.errors)})),null,2));
