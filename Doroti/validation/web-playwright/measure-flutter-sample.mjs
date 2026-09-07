import { chromium } from '@playwright/test';
import { writeFile, mkdir } from 'node:fs/promises';

// Reference-only Chrome timeline. ARIA mutation is not Doroti's scene commit
// endpoint; preserve it separately, without inventing a cross-renderer ratio.
const deadline=setTimeout(()=>process.exit(1),1200000);
try {
  for(let run=1;run<=3;run++) {
    const browser=await chromium.launch({headless:true,args:['--enable-gpu-rasterization','--ignore-gpu-blocklist','--use-angle=default']});
    try {
      const page=await browser.newPage({viewport:{width:1280,height:900}});
      const errors=[];page.on('pageerror',e=>errors.push(String(e)));
      await page.goto(process.env.DOROTI_FLUTTER_BASE_URL??'http://127.0.0.1:5090');
      await page.waitForTimeout(3000);
      const placeholder=page.locator('flt-semantics-placeholder');
      if(await placeholder.count()) await placeholder.evaluate(e=>e.click());
      const group=page.locator('[role="group"][aria-label*="Progress indicators"]');
      const play=group.locator('[role="button"]').filter({hasText:/^$/});
      let bounds;
      for(let step=0;step<10;step++) {
        bounds=await play.count()===1?await play.boundingBox():null;
        if(bounds && bounds.y>150 && bounds.y+bounds.height<780)break;
        await page.mouse.move(500,600);await page.mouse.wheel(0,300);await page.waitForTimeout(500);
      }
      if(!bounds || bounds.y<150 || bounds.y+bounds.height>=780)throw Error('Reference progress not visible');
      await page.waitForTimeout(5000);
      await play.evaluate(e=>{
        globalThis.__referenceOnset={};
        document.addEventListener('pointerup',event=>globalThis.__referenceOnset.input=performance.timeOrigin+event.timeStamp,{once:true,capture:true});
        new MutationObserver(()=>{
          if(e.getAttribute('aria-current')==='true' && !globalThis.__referenceOnset.aria)
            globalThis.__referenceOnset.aria=performance.timeOrigin+performance.now();
        }).observe(e,{attributes:true,attributeFilter:['aria-current']});
      });
      const cdp=await page.context().newCDPSession(page);
      await cdp.send('Tracing.start',{categories:'devtools.timeline,v8,disabled-by-default-v8.cpu_profiler',transferMode:'ReturnAsStream'});
      await page.mouse.click(bounds.x+bounds.width/2,bounds.y+bounds.height/2);
      await page.waitForFunction(()=>globalThis.__referenceOnset.aria);
      await page.mouse.move(20,40);await page.waitForTimeout(6000);
      const onset=await page.evaluate(()=>globalThis.__referenceOnset);
      const finished=new Promise(resolve=>cdp.once('Tracing.tracingComplete',resolve));
      await cdp.send('Tracing.end');const {stream}=await finished;
      let raw='';while(true){const chunk=await cdp.send('IO.read',{handle:stream});raw+=chunk.data;if(chunk.eof)break;}
      await cdp.send('IO.close',{handle:stream});
      await mkdir('artifacts/sample-perf',{recursive:true});
      await writeFile(`artifacts/sample-perf/work2-flutter-${run}.trace.json`,raw);
      const events=JSON.parse(raw).traceEvents;
      const callbacks=events.filter(e=>e.name==='FireAnimationFrame'&&e.ph==='X').map(e=>e.dur/1000).sort((a,b)=>a-b);
      const result={run,viewport:{width:1280,height:900,dpr:1},bounds,onset,inputToAriaMs:onset.aria-onset.input,errors,
        animationCallback:{count:callbacks.length,p95:callbacks[Math.ceil(callbacks.length*.95)-1],max:callbacks.at(-1)},
        limitation:'CanvasKit release JS reference, semantics explicitly enabled. Chrome rAF CPU and ARIA mutation, not scene commit or scan-out; lazy section boundaries differ.'};
      await writeFile(`artifacts/sample-perf/work2-flutter-${run}.json`,JSON.stringify(result,null,2));
      console.log(JSON.stringify(result));
    } finally {await browser.close();}
  }
} finally {clearTimeout(deadline);}
