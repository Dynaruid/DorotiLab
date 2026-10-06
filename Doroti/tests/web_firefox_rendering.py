"""Installed Firefox DPR/cadence/continuous-scroll regression.

Run through eng/run-with-timeout.py --timeout 1200. Requires Selenium/geckodriver.
Synthetic wheel cadence and GPU submit times do not establish physical display FPS.
"""
import argparse
from functools import partial
from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer
import json
from pathlib import Path
import statistics
import threading
import time
from selenium import webdriver
from selenium.webdriver.firefox.options import Options
from selenium.webdriver.firefox.service import Service
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.common.action_chains import ActionChains

p=argparse.ArgumentParser()
p.add_argument('--root',type=Path,required=True)
p.add_argument('--out',type=Path,required=True)
p.add_argument('--renderer',choices=['webgl','webgpu'],default='webgl')
p.add_argument('--headed',action='store_true')
p.add_argument('--width',type=int,default=1280)
p.add_argument('--height',type=int,default=900)
a=p.parse_args()
ROOT=Path(__file__).resolve().parents[2]
if not a.out.resolve().is_relative_to(ROOT/'temp/testing') or a.out.exists():p.error('Use a new output directory under temp/testing.')
if not (a.root/'index.html').is_file():p.error('Root must be a frozen published wwwroot.')
a.out.mkdir(parents=True,exist_ok=False)
class Handler(SimpleHTTPRequestHandler):
    extensions_map=SimpleHTTPRequestHandler.extensions_map|{'.mjs':'text/javascript','.wasm':'application/wasm'}
    def end_headers(self):
        self.send_header('Cross-Origin-Opener-Policy','same-origin')
        self.send_header('Cross-Origin-Embedder-Policy','require-corp')
        self.send_header('Cache-Control','no-store');super().end_headers()
    def log_message(self,*args):pass
server=ThreadingHTTPServer(('127.0.0.1',0),partial(Handler,directory=str(a.root.resolve())))
threading.Thread(target=server.serve_forever,daemon=True).start()
options=Options(); options.binary_location='C:/Program Files/Mozilla Firefox/firefox.exe'
if not a.headed:options.add_argument('-headless')
driver=None; records=[]
def js(code):return driver.execute_script('return '+code)
def async_js(code):
    result=driver.execute_async_script('const done=arguments[arguments.length-1];Promise.resolve(eval(arguments[0])).then(done,e=>done({probeError:String(e)}));',code)
    if isinstance(result,dict) and 'probeError' in result:raise RuntimeError(result)
    return result
def stats(values):
    values=sorted(values)
    return {'count':len(values),'p50':statistics.median(values) if values else None,
            'p95':values[min(len(values)-1,int(len(values)*.95))] if values else None,'max':max(values) if values else None}
try:
    driver=webdriver.Firefox(options=options,service=Service(log_output=str(a.out/'geckodriver.log')))
    driver.set_script_timeout(120)
    driver.set_window_size(a.width,a.height)
    viewport=js('({x:outerWidth-innerWidth,y:outerHeight-innerHeight})')
    driver.set_window_size(a.width+viewport['x'],a.height+viewport['y'])
    driver.get(f'http://127.0.0.1:{server.server_port}/?dorotiFrameCost=1&dorotiRenderer=worker-direct-{a.renderer}')
    WebDriverWait(driver,120).until(lambda d:js('document.documentElement.dataset.dorotiBootstrapStage') in ['started','failed'])
    assert js('document.documentElement.dataset.dorotiBootstrapStage')=='started'
    WebDriverWait(driver,90).until(lambda d:js('!!document.querySelector("#doroti-semantics [role=slider]")'))
    time.sleep(2)
    identity=js('({dataset:{...document.documentElement.dataset},root:{...document.querySelector(".doroti-root").dataset},snapshot:JSON.parse(__dorotiResizeDiagnostics.snapshot(__dorotiResizeDiagnostics.hosts()[0])),viewport:{width:innerWidth,height:innerHeight,dpr:devicePixelRatio},ua:navigator.userAgent,gpu:!!navigator.gpu})')
    assert identity['dataset']['dorotiRenderer']=='worker-direct-'+a.renderer,identity
    assert identity['snapshot']['gpu']['hardware'],identity
    print(json.dumps(identity),flush=True)
    def park_pointer():
        actions=ActionChains(driver)
        actions.w3c_actions.pointer_action.move_to_location(10,10)
        actions.perform()
        time.sleep(.5)
    park_pointer()
    def capture():return async_js('__dorotiFrameCost("capture")')
    def diagnostics():return async_js('__dorotiFrameCost("diagnostics")')
    def reset():async_js('__dorotiFrameCost("reset")')
    def snapshot_labels():return js('Array.from(document.querySelectorAll("#doroti-semantics [aria-label]")).map(e=>({label:e.getAttribute("aria-label"),y:e.getBoundingClientRect().y,height:e.getBoundingClientRect().height}))')
    def record(name,ingress=None):
        data=capture(); diag=diagnostics(); rows=data['rows']
        rec={'name':name,'ingress':ingress,'data':data,'diagnostics':diag,'labels':snapshot_labels(),
             'frameworkMs':stats([r[4]-r[3] for r in rows if r[0]==1]),
             'rasterMs':stats([r[4]-r[3] for r in rows if r[0]==3]),
             'inputQueueMs':stats([r[3]-r[5] for r in rows if r[0]==2 and r[5]>0]),
             'wheelQueueMs':stats([r[3]-r[5] for r in rows if r[0]==5]),
             'wheelCount':sum(r[1] for r in rows if r[0]==5),
             'submitIntervalsMs':stats([y[4]-x[4] for x,y in zip([r for r in rows if r[0]==3],[r for r in rows if r[0]==3][1:])])}
        records.append(rec)
        print(json.dumps({k:rec[k] for k in ['name','frameworkMs','rasterMs','inputQueueMs','submitIntervalsMs']}),flush=True)
        (a.out/'partial.json').write_text(json.dumps({'identity':identity,'records':records},indent=2),encoding='utf-8')
    reset(); raf=async_js('(async()=>{let rows=[];for(let i=0;i<180;i++){await new Promise(requestAnimationFrame);rows.push(performance.timeOrigin+performance.now())}return rows})()')
    record('components-idle',raf)
    idle_skia=records[-1]['diagnostics']['managed']['skia']
    assert idle_skia['Work']['CommandCacheHits']>0,idle_skia['Work']
    if a.renderer=='webgl':
        assert idle_skia['PictureRasterCacheHits']>0,idle_skia
    box=js('(()=>{let e=Array.from(document.querySelectorAll("#doroti-semantics [aria-label]")).find(e=>e.getAttribute("aria-label").startsWith("Variable Blur")&&e.getBoundingClientRect().y>innerHeight-90);let b=e.getBoundingClientRect();return {x:b.x+b.width/2,y:b.y+b.height/2}})()')
    actions=ActionChains(driver); actions.w3c_actions.pointer_action.move_to_location(box['x'],box['y']); actions.click().perform()
    park_pointer()
    time.sleep(2)
    before_scroll=diagnostics()
    reset()
    ingress=async_js('(async()=>{const root=document.querySelector(".doroti-root"),rows=[];for(let i=0;i<180;i++){await new Promise(requestAnimationFrame);const delta=i<120?3.5:3.5*Math.exp(-(i-120)/15);const epoch=performance.timeOrigin+performance.now();root.dispatchEvent(new WheelEvent("wheel",{clientX:innerWidth*.6,clientY:innerHeight-140,deltaY:delta,deltaMode:0,bubbles:true,cancelable:true}));rows.push([epoch,delta])}return rows})()')
    record('continuous-wheel',ingress)
    time.sleep(.5); record('wheel-settle-500ms')
    settled=records[-1]
    assert settled['wheelCount']==180,settled['wheelCount']
    assert settled['data']['dropped']==0
    scroll_before=max(before_scroll['mainScroll'],key=lambda node:node.get('max',0))
    scroll_after=next(node for node in settled['diagnostics']['mainScroll'] if node['id']==scroll_before['id'])
    expected=min(scroll_before['max'],scroll_before['position']+sum(row[1] for row in ingress))
    assert abs(scroll_after['position']-expected)<1,(scroll_before,scroll_after,expected)
    last_wheel=max((r for r in settled['data']['rows'] if r[0]==5),key=lambda r:r[2])
    submissions=[r for r in settled['data']['rows'] if r[0]==3 and r[2]>=last_wheel[2]]
    assert submissions,'No raster submission reached the final wheel input.'
    last_wheel_submit_ms=min(r[4] for r in submissions)-last_wheel[5]
    driver.save_screenshot(str(a.out/'scrolled.png'))
    for step in range(8):
        driver.set_window_size(a.width+viewport['x']+step*10,a.height+viewport['y']-step*5)
        time.sleep(.08)
    time.sleep(1.2)
    resize=diagnostics()
    assert resize['managed']['frame']['Failed']==0,resize['managed']['frame']
    assert resize['backing']['width']>=resize['backing']['requiredWidth']
    assert resize['backing']['height']>=resize['backing']['requiredHeight']
    driver.save_screenshot(str(a.out/'resized.png'))
    (a.out/'result.json').write_text(json.dumps({'status':'PASS','browser':driver.capabilities['browserVersion'],
        'headed':a.headed,'identity':identity,'records':records,'scrollBefore':scroll_before,'scrollAfter':scroll_after,
        'expectedScroll':expected,'lastWheelToSubmitMs':last_wheel_submit_ms,'resize':resize,
        'physicalTrackpad':'notVerified','displayFPS':'notMeasured'},indent=2),encoding='utf-8')
finally:
    if driver:driver.quit()
    server.shutdown();server.server_close()
