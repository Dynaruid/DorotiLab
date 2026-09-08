import { chromium } from '@playwright/test';
import { PNG } from 'pngjs';
import { mkdir, writeFile } from 'node:fs/promises';

const label=process.argv[2],width=Number(process.argv[3]??1280);
if(!/^[\w-]+$/.test(label))throw Error('Simple label required');
const deadline=setTimeout(()=>{console.error('20-minute timeout');process.exit(1);},1200000);
const browser=await chromium.launch({headless:true,args:['--enable-gpu-rasterization','--ignore-gpu-blocklist','--use-angle=default']});
const errors=[];
try {
 const page=await browser.newPage({viewport:{width,height:900}});
 page.on('pageerror',e=>errors.push(String(e)));
 await page.goto((process.env.DOROTI_WEB_BASE_URL??'http://127.0.0.1:5088')+'/?dorotiTestbedMode=sample&dorotiResizeDiagnostics=0'+(process.env.DOROTI_PERF_QUERY??''));
 await page.getByRole('group',{name:'Actions Common buttons',exact:true}).waitFor({state:'visible',timeout:120000});
 await page.waitForFunction(()=>{const d=globalThis.__dorotiResizeDiagnostics,s=JSON.parse(d.snapshot(d.hosts()[0])),p=JSON.parse(d.presenter('doroti-surface'));
  return p.frontRequestId>0&&p.frontGeneration===s.resizeEpoch.generation;},null,{timeout:120000});
 const screenshot=await page.screenshot(),png=PNG.sync.read(screenshot),colors=new Set();
 for(let i=0;i<png.data.length;i+=80)colors.add(png.data.readUInt32BE(i));
 if(colors.size<=20)throw Error('Presented PNG contains no sample content');
 const result=await page.evaluate(()=>({availabilityUpperBoundMs:performance.now(),resources:performance.getEntriesByType('resource').map(r=>({name:r.name,transferSize:r.transferSize,encodedBodySize:r.encodedBodySize}))}));
 await mkdir('artifacts/section-startup',{recursive:true});
 await writeFile(`artifacts/section-startup/${label}.json`,JSON.stringify({label,width,browser:browser.version(),url:page.url(),errors,...result,
  limitation:'Cold runtime, diagnostics OFF. Upper bound includes semantic readiness and screenshot readback; not scan-out latency or first-content instant.'},null,2));
 console.log(JSON.stringify({label,width,availabilityUpperBoundMs:result.availabilityUpperBoundMs,errors}));
 if(errors.length)process.exitCode=1;
} finally {await browser.close();clearTimeout(deadline);}
