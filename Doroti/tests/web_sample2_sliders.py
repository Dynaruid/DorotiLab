"""Frozen Sample2 slider, memory and CPU/submit probe (not displayed FPS).

Run through eng/run-with-timeout.py --timeout 1200 with the Playwright venv.
Use --profile for attribution only; keep it OFF for performance comparisons.
"""
import argparse
from functools import partial
from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer
import json
from pathlib import Path
import statistics
import re
import threading
from playwright.sync_api import sync_playwright

p = argparse.ArgumentParser()
p.add_argument('--root', type=Path, required=True)
p.add_argument('--out', type=Path, required=True)
p.add_argument('--browser', choices=['firefox', 'chrome'], default='firefox')
p.add_argument('--renderer', choices=['webgl', 'webgpu'], default='webgl')
p.add_argument('--runs', type=int, choices=[1, 2, 3], default=2)
p.add_argument('--dpr',type=float,choices=[1,1.5,2],default=1)
p.add_argument('--profile', action='store_true')
a = p.parse_args()
ROOT = Path(__file__).resolve().parents[2]
if not a.out.resolve().is_relative_to(ROOT / 'temp/testing') or a.out.exists():
    p.error('Use a new output directory under temp/testing.')
if not (a.root / 'index.html').is_file():
    p.error('Root must be a frozen published wwwroot.')
a.out.mkdir(parents=True, exist_ok=False)

class Handler(SimpleHTTPRequestHandler):
    extensions_map = SimpleHTTPRequestHandler.extensions_map | {'.mjs':'text/javascript','.wasm':'application/wasm'}
    def end_headers(self):
        self.send_header('Cross-Origin-Opener-Policy','same-origin')
        self.send_header('Cross-Origin-Embedder-Policy','require-corp')
        self.send_header('Cache-Control','no-store')
        super().end_headers()
    def log_message(self, *args): pass

def summary(values):
    values = sorted(values)
    return {'count':len(values), 'p50':statistics.median(values) if values else None,
            'p95':values[min(len(values)-1, int(len(values)*.95))] if values else None,
            'max':max(values) if values else None}

server = ThreadingHTTPServer(('127.0.0.1',0), partial(Handler,directory=str(a.root.resolve())))
threading.Thread(target=server.serve_forever,daemon=True).start()
records=[]
try:
    with sync_playwright() as pw:
        browser = (pw.firefox.launch(headless=True,firefox_user_prefs={'layout.css.devPixelsPerPx':str(a.dpr)}) if a.browser=='firefox' else
                   pw.chromium.launch(channel='chrome',headless=True,args=['--enable-unsafe-webgpu']))
        context = browser.new_context(viewport={'width':720,'height':840},device_scale_factor=a.dpr)
        page = context.new_page()
        errors=[]
        def record_error(error):
            errors.append(str(error))
            (a.out/'errors.json').write_text(json.dumps(errors,indent=2),encoding='utf-8')
        page.on('pageerror', record_error)
        page.on('console', lambda m: record_error(m.text) if m.type=='error' else None)
        page.goto(f'http://127.0.0.1:{server.server_port}/?dorotiFrameCost=1&dorotiRenderer=worker-direct-{a.renderer}'+('&dorotiAllocationProfile=1&dorotiLayoutProfile=1&dorotiResizeDiagnostics=1' if a.profile else ''))
        page.wait_for_function('["started","failed"].includes(document.documentElement.dataset.dorotiBootstrapStage)',timeout=120000)
        page.wait_for_selector('#doroti-semantics [role="slider"]',state='attached',timeout=90000)
        page.wait_for_timeout(1500)
        identity=page.evaluate('({dataset:{...document.documentElement.dataset},snapshot:JSON.parse(__dorotiResizeDiagnostics.snapshot(__dorotiResizeDiagnostics.hosts()[0])),ua:navigator.userAgent})')
        assert identity['dataset']['dorotiRenderer'] == 'worker-direct-'+a.renderer
        assert identity['snapshot']['gpu']['hardware'], identity
        assert identity['snapshot']['devicePixelRatio']==a.dpr,identity
        print(f'{a.browser} {browser.version}, {a.renderer}, hardware GPU',flush=True)
        def click_tab(label):
            box=page.evaluate('label=>{let e=Array.from(document.querySelectorAll("#doroti-semantics [aria-label]")).find(e=>e.getAttribute("aria-label").startsWith(label)&&e.getBoundingClientRect().y>innerHeight-90);let r=e.getBoundingClientRect();return {x:r.x+r.width/2,y:r.y+r.height/2}}',label)
            page.mouse.click(box['x'],box['y'],delay=70)
            page.wait_for_timeout(1200)
        for tab in ['Components','Variable Blur']:
            if tab!='Components': click_tab(tab)
            slider=page.get_by_role('slider')
            slider.wait_for(state='attached')
            box=slider.bounding_box()
            assert box and box['y']<840, box
            before=page.evaluate('async()=>await __dorotiFrameCost("diagnostics")')
            for run in range(a.runs):
                # Capture the actual thumb, then preserve every synthetic sample
                # at the browser's main rAF cadence (165Hz on the qualification host).
                page.evaluate('window.__probePointer=null;document.querySelector(".doroti-root").addEventListener("pointerdown",e=>window.__probePointer={pointerId:e.pointerId,pointerType:e.pointerType},{once:true})')
                label=page.evaluate('Array.from(document.querySelectorAll("#doroti-semantics [aria-label]")).map(e=>e.getAttribute("aria-label")).find(s=>s.startsWith("Volume:")||s.startsWith("Blur strength:"))')
                fraction=float(re.search(r'([0-9]+)',label)[1])/(100 if tab=='Components' else 32)
                thumb_x=box['x']+22+(box['width']-44)*fraction
                page.mouse.move(thumb_x,box['y']+box['height']/2)
                page.mouse.down()
                page.wait_for_timeout(250)
                page.evaluate('async()=>await __dorotiFrameCost("reset")')
                ingress=page.evaluate('''async box=>{
                  const root=document.querySelector('.doroti-root'), rows=[]; let middle=null;
                  for(let i=0;i<144;i++) {
                    await new Promise(requestAnimationFrame);
                    const fraction=i<72 ? box.fraction+(.85-box.fraction)*i/71 : .85-.7*(i-72)/71;
                    const x=box.x+22+(box.width-44)*fraction,y=box.y+box.height/2;
                    const start=performance.timeOrigin+performance.now();
                    root.dispatchEvent(new PointerEvent('pointermove',{...window.__probePointer,clientX:x,clientY:y,buttons:1,bubbles:true}));
                    rows.push([start,performance.timeOrigin+performance.now()]);
                    if(i===75) middle=Array.from(document.querySelectorAll('#doroti-semantics [aria-label]')).map(e=>e.getAttribute('aria-label')).find(s=>s.startsWith('Volume:')||s.startsWith('Blur strength:'));
                  }
                  return {rows,middle};
                }''', dict(box,fraction=fraction))
                data=page.evaluate('async()=>await __dorotiFrameCost("capture")')
                diag=page.evaluate('async()=>await __dorotiFrameCost("diagnostics")')
                page.mouse.up()
                page.wait_for_timeout(350)
                labels=page.evaluate('Array.from(document.querySelectorAll("#doroti-semantics [aria-label]")).map(e=>e.getAttribute("aria-label"))')
                final_label=next(s for s in labels if s.startswith('Volume:') or s.startswith('Blur strength:'))
                assert ingress['middle']!=label, (label,ingress['middle'])
                assert abs(float(re.search(r'([0-9]+)',final_label)[1])-(15 if tab=='Components' else 5))<=2,final_label
                rows=data['rows']
                assert data['dropped']==0, 'Cost ring overflow makes the comparison incomplete.'
                samples=sum(r[1] for r in rows if r[0]==4)
                if samples:
                    assert samples==len(ingress['rows']), (samples,len(ingress['rows']))
                assert diag['managed']['frame']['Failed']==0, diag['managed']['frame']
                cache=diag['managed']['cache']
                assert cache['TextEntries']<=cache['TextEntryLimit'],cache
                assert cache['RasterRgba8Bytes']<=cache['RasterRgba8Limit'],cache
                rec={'tab':tab,'run':run,'before':before,'after':diag,'data':data,'ingress':ingress,'labels':labels,
                     'retainedMotionSamples':samples or sum(r[0]==2 for r in rows),
                     'frameworkMs':summary([r[4]-r[3] for r in rows if r[0]==1]),
                     'rasterMs':summary([r[4]-r[3] for r in rows if r[0]==3]),
                     'inputMs':summary([r[4]-r[3] for r in rows if r[0]==2]),
                     'inputQueueMs':summary([r[3]-r[5] for r in rows if r[0]==2 and r[5]>0]),
                     'submitIntervalsMs':summary([y[4]-x[4] for x,y in zip([r for r in rows if r[0]==3],[r for r in rows if r[0]==3][1:])])}
                records.append(rec)
                (a.out/'partial.json').write_text(json.dumps({'records':records},indent=2),encoding='utf-8')
                print(json.dumps({k:rec[k] for k in ['tab','run','frameworkMs','rasterMs','inputQueueMs','submitIntervalsMs']}),flush=True)
                page.screenshot(path=str(a.out/f'{tab.replace(" ","-")}-{run}.png'))
            page.wait_for_timeout(5500)
            records[-1]['idle']=page.evaluate('async()=>await __dorotiFrameCost("diagnostics")')
        # Both notifier-owned values must survive tab disposal/offstage changes.
        click_tab('Components')
        assert any(s.startswith('Volume: 15%') for s in page.evaluate('Array.from(document.querySelectorAll("#doroti-semantics [aria-label]")).map(e=>e.getAttribute("aria-label"))'))
        click_tab('Variable Blur')
        assert any(s.startswith('Blur strength: 5') for s in page.evaluate('Array.from(document.querySelectorAll("#doroti-semantics [aria-label]")).map(e=>e.getAttribute("aria-label"))'))
        # Retaining the list widget must still allow inherited theme updates.
        click_tab('Settings')
        dark=page.locator('#doroti-semantics [aria-label="Dark"]')
        dark.wait_for(state='attached')
        rect=dark.bounding_box()
        page.mouse.click(rect['x']+rect['width']/2,rect['y']+rect['height']/2,delay=70)
        click_tab('Variable Blur')
        page.screenshot(path=str(a.out/'dark-retained-list.png'))
        from PIL import Image
        screenshot=Image.open(a.out/'dark-retained-list.png').convert('RGB')
        # Firefox can return CSS-sized captures despite a DPR 2 drawing surface.
        pixel=screenshot.getpixel((round(680*screenshot.width/720),round(620*screenshot.height/840)))
        assert max(pixel)<100, ('Retained list did not follow the dark theme',pixel)
        page.set_viewport_size({'width':900,'height':740})
        page.wait_for_timeout(800)
        assert any(s.startswith('Blur strength: 5') for s in page.evaluate('Array.from(document.querySelectorAll("#doroti-semantics [aria-label]")).map(e=>e.getAttribute("aria-label"))'))
        page.screenshot(path=str(a.out/'resized.png'))
        (a.out/'result.json').write_text(json.dumps({'status':'PASS' if not errors else 'FAIL',
            'browser':browser.version,'identity':identity,'errors':errors,'records':records,
            'valuesRetainedAcrossTabs':True,'retainedListThemeUpdate':True,'resize':True,
            'physicalInput':'notVerified','displayFPS':'notMeasured',
            'gpuProcessMemory':'notMeasured','profileEnabled':a.profile},indent=2),encoding='utf-8')
        assert not errors,errors
        context.close();browser.close()
finally:
    server.shutdown();server.server_close()
