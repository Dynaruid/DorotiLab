import {readFile,writeFile} from 'node:fs/promises';
const rows=[];
for(const [mode,scenario,count] of [['detailed','wide',1],['detailed','cross',2],['minimal','wide',1],['minimal','cross',1]])
 for(let run=1;run<=count;run++) for(const variant of ['before','after']) {
  const label=`responsive-final-${mode}-${variant}-${scenario}-${run}`;
  const p=JSON.parse(await readFile(`artifacts/state-resize/${label}.json`,'utf8'));
  if(p.errors.length)throw Error(`${label} has errors`);
  const target=p.after.snapshot.resizeEpoch;
  const fronts=p.after.trace.filter(e=>e.phase==='front-commit'&&e.source==='worker-direct-surface');
  const exact=fronts.find(e=>{const d=JSON.parse(e.detail);return d.generation===target.generation&&d.sceneDisposition==='exact-rendered';});
  if(!exact)throw Error(`${label}: missing final exact-generation commit`);
  let work=null;
  if(p.detailed) {
   const a=p.profileBefore[0].managed,b=p.profiles[0].managed;
   const av=a.work.Samples.at(-1).Totals,bv=b.work.Samples.at(-1).Totals;
   work=Object.fromEntries(b.work.Names.map((n,i)=>[n,bv[i]-av[i]]));
   work.allocatedBytes=b.profile.managedAllocatedBytes-a.profile.managedAllocatedBytes;
   work.failed=b.frame.Failed-a.frame.Failed;
   if(work.failed)throw Error(`${label}: failed renderer frame`);
  }
  rows.push({label,mode,scenario,variant,run,callbackP95:p.callback.p95,callbackMax:p.callback.max,
   activeProgressiveAndExactCommits:p.activeFronts,
   finalMetricsToExactCommitMs:(exact.timestampMicroseconds-target.timestampMicroseconds)/1000,work});
 }
await writeFile('artifacts/state-resize/responsive-final-summary.json',JSON.stringify({rows,
 limitation:'Bounded sequential CDP resize; callback p95 equals max when fewer than 20 frames. Active commits include progressive older generations. Final latency requires the actual final generation in commit detail. Not physical window-drag or scan-out acceptance.'},null,2));
for(const {work,...row} of rows)console.log(JSON.stringify({...row,rebuilds:work?.Rebuild,dependencies:work?.DependencyChanged}));
