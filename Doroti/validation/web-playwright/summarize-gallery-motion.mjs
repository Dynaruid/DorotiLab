import { readFile, writeFile } from 'node:fs/promises';

const root='artifacts/gallery-motion';
const median=a=>{a.sort((x,y)=>x-y);const n=a.length;return n%2?a[(n-1)/2]:(a[n/2-1]+a[n/2])/2;};
const records=[];
for(const scenario of ['carousel','snapping','scroll']) {
 const variants={};
 for(const variant of ['before','after']) {
  const runs=[];
  for(let run=1;run<=2;run++) {
   const file=`gallery-fling-${variant}-${scenario}-${run}.json`;
   const p=JSON.parse(await readFile(`${root}/${file}`,'utf8'));
   if(!p.changed||p.errors.length||!p.fling||!p.detailed)throw Error(`Invalid run ${file}`);
   const a=p.profileBefore[0].managed,b=p.profileAfter[0].managed;
   const count=b.frame.Submitted-a.frame.Submitted;
   const active=e=>p.windows.some(w=>e.timestampMicroseconds/1000>=w.begin&&(e.timestampMicroseconds+e.durationMicroseconds)/1000<=w.end);
   const raster=p.after.trace.filter(e=>e.phase==='managed-raster-end'&&active(e)).map(e=>e.durationMicroseconds/1000);
   raster.sort((x,y)=>x-y);
   runs.push({file,callbackP50:p.callbacks.p50,callbackP95:p.callbacks.p95,
    rasterP50:median(raster),rasterP95:raster[Math.ceil(raster.length*.95)-1],
    coastP50:p.coastIntervals.p50,coastP95:p.coastIntervals.p95,coastSamples:p.coastIntervals.n,
    allocatedPerSubmitted:(b.profile.managedAllocatedBytes-a.profile.managedAllocatedBytes)/count,
    commandCache:b.frame.Skia.Work,
    failed:b.frame.Failed-a.frame.Failed, gpu:p.after.snapshot.gpu});
  }
  variants[variant]={runs,median:Object.fromEntries(['callbackP50','callbackP95','rasterP50','rasterP95','coastP50','coastP95','allocatedPerSubmitted'].map(k=>[k,median(runs.map(r=>r[k]))]))};
 }
 records.push({scenario,...variants,reductionPercent:Object.fromEntries(['callbackP50','callbackP95','rasterP50','rasterP95','coastP95','allocatedPerSubmitted'].map(k=>[k,(1-variants.after.median[k]/variants.before.median[k])*100]))});
}
const result={records,limitation:'Two independent runs per scenario and revision; exploratory A/B, not confidence intervals or physical display FPS. CDP gesture delivery varies with workload. Coast interval samples can include idle boundaries.'};
await writeFile(`${root}/summary.json`,JSON.stringify(result,null,2));
for(const r of records)console.log(JSON.stringify({scenario:r.scenario,before:r.before.median,after:r.after.median,reductionPercent:r.reductionPercent}));
