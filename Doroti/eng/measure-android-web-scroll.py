"""Bounded Pixel 5 Android-browser Web scroll probe (1080x2340 touch coordinates).

Run with run-with-timeout.py --timeout 1200. Requires Python Playwright and adb;
the Android browser must already expose CDP on --cdp-port. Continuous mode uses
the current eng/android/ScrollGesture.java built and pushed as doroti-scroll.jar
under /data/local/tmp. Keep compilation and screen recording outside the measured
window. Worker/GPU submission timings do not establish physical display FPS.
"""
import argparse
import json
from pathlib import Path
import subprocess
import time
from playwright.sync_api import sync_playwright

parser=argparse.ArgumentParser(description=__doc__)
parser.add_argument('--label',required=True)
parser.add_argument('--url',default='http://127.0.0.1:5088/')
parser.add_argument('--browser',choices=['android','chrome'],default='android')
parser.add_argument('--renderer',choices=['auto','webgl','webgpu'],default='auto')
parser.add_argument('--trials',type=int,choices=[1,2,3],default=1)
parser.add_argument('--profile',action='store_true')
parser.add_argument('--gesture',choices=['swipes','continuous'],default='swipes')
parser.add_argument('--serial',default='emulator-5554')
parser.add_argument('--cdp-port',type=int,default=9223)
parser.add_argument('--output',type=Path,default=Path('temp/testing/android-web-scroll'))
parser.add_argument('--warmup',action='store_true',help='One unmeasured continuous pass and slow return to the top')
args=parser.parse_args()
if args.gesture=='continuous' and args.browser!='android':parser.error('continuous gestures require the Android shell driver')
if args.warmup and (args.gesture!='continuous' or args.browser!='android'):parser.error('warmup currently requires Android continuous mode')
out=args.output
out.mkdir(parents=True,exist_ok=True)

def stats(values):
    values=sorted(values)
    return {'n':len(values),'p50':values[int((len(values)-1)*.5)],'p95':values[int((len(values)-1)*.95)],'max':values[-1],
            'over33':sum(x>33.34 for x in values),'over50':sum(x>50 for x in values)} if values else None

with sync_playwright() as p:
    browser=p.chromium.connect_over_cdp(f'http://127.0.0.1:{args.cdp_port}') if args.browser=='android' else p.chromium.launch(channel='chrome',headless=True)
    if args.browser=='android':
        page=browser.contexts[0].pages[0]
    else:
        context=browser.new_context(viewport={'width':393,'height':719},device_scale_factor=2.75,is_mobile=True,has_touch=True,
            user_agent='Mozilla/5.0 (Linux; Android 13; Pixel 5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/156.0.0.0 Mobile Safari/537.36')
        page=context.new_page()
    page.bring_to_front()
    errors=[]
    page.on('pageerror',lambda e:errors.append(str(e)))
    records=[]
    for trial in range(args.trials):
        params='dorotiFrameCost=1&dorotiTestbedMode=sample'
        if args.renderer!='auto':params+='&dorotiRenderer=worker-direct-'+args.renderer
        if args.profile:params+='&dorotiAllocationProfile=1&dorotiLayoutProfile=1&dorotiResizeDiagnostics=1'
        print(f'Starting {args.label} trial {trial+1}',flush=True)
        page.goto(args.url+('?' if '?' not in args.url else '&')+params,timeout=90000)
        try:
            page.wait_for_function("document.documentElement.dataset.dorotiBootstrapStage==='started' || document.documentElement.dataset.dorotiBootstrapError",timeout=90000)
            assert page.evaluate("document.documentElement.dataset.dorotiBootstrapStage")=='started'
        except Exception:
            failure=page.evaluate("({url:location.href,dataset:{...document.documentElement.dataset},root:{...document.querySelector('.doroti-root')?.dataset},body:document.body.innerText})")
            failure['errors']=errors
            (out/f'{args.label}-{trial+1}-startup-failure.json').write_text(json.dumps(failure,ensure_ascii=False,indent=2),encoding='utf-8')
            page.screenshot(path=str(out/f'{args.label}-{trial+1}-startup-failure.png'))
            raise
        print(f'Ready {args.label} trial {trial+1}',flush=True)
        page.wait_for_function("typeof window.__dorotiFrameCost==='function'")
        page.wait_for_timeout(2500)
        if args.warmup:
            subprocess.run(['adb','-s',args.serial,'shell','CLASSPATH=/data/local/tmp/doroti-scroll.jar','app_process','/system/bin','ScrollGesture',
                '950,1850,0','950,800,900','950,1750,1800','950,850,2700','950,1650,3600'],check=True,stdout=subprocess.DEVNULL)
            page.wait_for_timeout(1100)
            subprocess.run(['adb','-s',args.serial,'shell','input','swipe','950','800','950','1800','1500'],check=True,stdout=subprocess.DEVNULL)
            page.wait_for_timeout(600)
        meta=page.evaluate("""() => ({version:navigator.userAgent,dpr:devicePixelRatio,width:innerWidth,height:innerHeight,
          dataset:{...document.documentElement.dataset},gpu:!!navigator.gpu,timeOrigin:performance.timeOrigin})""")
        if args.browser=='android' and (meta['height']<600 or not 350<=meta['width']<=450):
            raise RuntimeError('This Pixel 5 probe requires a full portrait browser viewport with the keyboard closed.')
        before=page.evaluate("__dorotiFrameCost('diagnostics')")
        page.evaluate("""async() => {
          await __dorotiFrameCost('reset');
          window.probe={raf:[],events:[],running:true};
          const raf=t=>{if(!probe.running)return;probe.raf.push(t);requestAnimationFrame(raf)};requestAnimationFrame(raf);
          for(const type of ['pointerdown','pointermove','pointerup','pointercancel'])document.querySelector('.doroti-root').addEventListener(type,
            e=>probe.running&&probe.events.push([e.type,e.timeStamp,e.clientY,e.isTrusted,e.getCoalescedEvents?.().length??1]));
        }""")
        session=page.context.new_cdp_session(page)
        gestures=[]
        patterns=[(1850,800,650),(750,1650,250),(1800,1000,650)] if args.gesture=='swipes' else [(0,0,3600)]
        for y0,y1,duration in patterns:
            begin=time.perf_counter()
            if args.browser=='android':
                command=['adb','-s',args.serial,'shell']
                if args.gesture=='continuous':
                    command+=['CLASSPATH=/data/local/tmp/doroti-scroll.jar','app_process','/system/bin','ScrollGesture',
                              '950,1850,0','950,800,900','950,1750,1800','950,850,2700','950,1650,3600']
                else:command+=['input','swipe','950',str(y0),'950',str(y1),str(duration)]
                subprocess.run(command,check=True,stdout=subprocess.DEVNULL)
            else:
                x=340;y0=(y0-292)/2.75;y1=(y1-292)/2.75
                session.send('Input.dispatchTouchEvent',{'type':'touchStart','touchPoints':[{'x':x,'y':y0}]})
                steps=max(1,int(duration/16.667))
                started=time.perf_counter()
                for n in range(1,steps+1):
                    deadline=started+duration/1000*n/steps
                    time.sleep(max(0,deadline-time.perf_counter()))
                    session.send('Input.dispatchTouchEvent',{'type':'touchMove','touchPoints':[{'x':x,'y':y0+(y1-y0)*n/steps}]})
                session.send('Input.dispatchTouchEvent',{'type':'touchEnd','touchPoints':[]})
            gestures.append({'start':begin,'completed':time.perf_counter(),'pattern':[y0,y1,duration]})
            page.wait_for_timeout(1100)
        finish=page.evaluate("__dorotiFrameCost('finish')")
        main=page.evaluate('()=>{probe.running=false;return probe}')
        rows=finish['trace']['rows']
        groups={name:stats([r[4]-r[3] for r in rows if r[0]==kind]) for name,kind in [('framework',1),('input',2),('raster',3)]}
        groups['inputQueue']=stats([r[3]-r[5] for r in rows if r[0]==2 and r[5]>0])
        groups['mainRafInterval']=stats([b-a for a,b in zip(main['raf'],main['raf'][1:])])
        submissions=sorted(r[4] for r in rows if r[0]==3)
        groups['rasterInterval']=stats([b-a for a,b in zip(submissions,submissions[1:])])
        windows=[];down=None
        for kind,stamp,*rest in main['events']:
            if kind=='pointerdown':down=meta['timeOrigin']+stamp
            elif kind in ['pointerup','pointercancel'] and down is not None:
                windows.append((down,meta['timeOrigin']+stamp));down=None
        groups['rasterIntervalDuringDrag']=stats([b-a for a,b in zip(submissions,submissions[1:]) if any(start<=a<=b<=end for start,end in windows)])
        summary={'browser':browser.version,'label':args.label,'trial':trial+1,'meta':meta,'stats':groups,
                 'ownerAllocatedBytes':finish['after']['managed'].get('ownerAllocatedBytes'),'errors':errors,
                 'mainScroll':finish['after']['managed'].get('scroll'),'touchEvents':len(main['events']),
                 'coalescedSamples':sum(e[4] for e in main['events']), 'gesture':args.gesture, 'warmup':args.warmup}
        records.append(summary)
        result={'summary':summary,'before':before,'finish':finish,'main':main,'gestures':gestures}
        (out/f'{args.label}-{trial+1}.json').write_text(json.dumps(result,ensure_ascii=False),encoding='utf-8')
        print(json.dumps(summary,ensure_ascii=False,indent=2),flush=True)
        page.screenshot(path=str(out/f'{args.label}-{trial+1}.png'))
    (out/f'{args.label}-summary.json').write_text(json.dumps(records,ensure_ascii=False,indent=2),encoding='utf-8')
    if args.browser!='android':browser.close()
