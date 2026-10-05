"""Read-only qualification and bounded warm CPU comparison of published Mono WASM AOT.

Use eng/run-with-timeout.py --timeout 1200. Published assets stay frozen while
Chrome runs. Counts/CPU submit intervals are not displayed FPS or physical input.
"""
import argparse
from functools import partial
from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer
import json
from pathlib import Path
import statistics
import threading
from playwright.sync_api import sync_playwright

ROOT = Path(__file__).resolve().parents[2]
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--aot', type=Path, required=True)
parser.add_argument('--baseline', type=Path)
parser.add_argument('--output', type=Path, required=True)
parser.add_argument('--pairs', type=int, choices=[1, 2, 3], default=3)
parser.add_argument('--compare-only', action='store_true', help='Use after the frozen AOT payload already passed the smoke run.')
args = parser.parse_args()
if args.compare_only and not args.baseline:
    parser.error('--compare-only requires --baseline.')
if args.baseline and args.baseline.resolve() == args.aot.resolve():
    parser.error('AOT and baseline must use separate published directories.')
out = args.output.resolve()
if not out.is_relative_to(ROOT / 'temp/testing') or out.exists():
    raise RuntimeError('Use a new output directory under temp/testing.')
out.mkdir(parents=True)

def percentile(values, fraction):
    if not values:
        return None
    values = sorted(values)
    position = (len(values) - 1) * fraction
    low = int(position)
    high = min(low + 1, len(values) - 1)
    return values[low] + (values[high] - values[low]) * (position - low)

def phase(rows, kind):
    values = [row[4] - row[3] for row in rows if row[0] == kind]
    return {'count':len(values), 'p50Ms':percentile(values,.5), 'p95Ms':percentile(values,.95), 'maxMs':max(values) if values else None}

def interval_union_ms(rows):
    # WebGL raster can run inside the framework callback. Never sum nested spans.
    spans = sorted((row[3],row[4]) for row in rows if row[0] in (1,3))
    total = 0
    start = end = None
    for left,right in spans:
        if start is None:
            start,end = left,right
        elif left <= end:
            end = max(end,right)
        else:
            total += end-start
            start,end = left,right
    return total + (end-start if start is not None else 0)

def start_progress(page, label):
    # IconButton's tooltip is an aria-description, not its accessible name.
    progress = page.locator('#doroti-semantics [aria-description="Start progress"]')
    try:
        for _ in range(12):
            box = progress.bounding_box(timeout=1000) if progress.count() else None
            if box and 80 < box['y'] < 650:
                break
            page.mouse.move(400,600)
            page.mouse.wheel(0,250)
            page.wait_for_timeout(600)
        else:
            raise AssertionError('The progress workload was not visible.')
        page.mouse.move(400,600)
        page.mouse.wheel(0,box['y']-350)
        page.wait_for_timeout(700)
        box = progress.bounding_box(timeout=1000)
        assert box and 80 < box['y'] < 650
        page.mouse.click(box['x']+box['width']/2,box['y']+box['height']/2,delay=100)
        page.locator('#doroti-semantics [aria-description="Stop progress"]').wait_for(state='attached')
    except BaseException:
        page.screenshot(path=str(out/f'{label}-progress-failure.png'))
        (out/f'{label}-progress-failure.json').write_text(json.dumps(page.evaluate('Array.from(document.querySelectorAll("#doroti-semantics [data-doroti-semantics-id]")).map(e=>({html:e.outerHTML,box:{y:e.getBoundingClientRect().y,height:e.getBoundingClientRect().height}}))'),indent=2))
        raise

servers = []
records = []
try:
    urls = {}
    for name,directory in [('aot',args.aot),('baseline',args.baseline)]:
        if directory is None:
            continue
        directory = directory.resolve()
        if not (directory/'index.html').is_file():
            raise RuntimeError(f'Missing published index.html: {directory}')
        log = (out/f'{name}-server.log').open('w',encoding='utf-8')
        class Handler(SimpleHTTPRequestHandler):
            extensions_map = SimpleHTTPRequestHandler.extensions_map | {'.mjs':'text/javascript','.wasm':'application/wasm'}
            def end_headers(self):
                self.send_header('Cross-Origin-Opener-Policy','same-origin')
                self.send_header('Cross-Origin-Embedder-Policy','require-corp')
                self.send_header('Cache-Control','no-store')
                super().end_headers()
            def log_message(self,format,*values,log=log):
                log.write(format % values + '\n')
                log.flush()
        server = ThreadingHTTPServer(('127.0.0.1',0),partial(Handler,directory=str(directory)))
        thread = threading.Thread(target=server.serve_forever,daemon=True)
        thread.start()
        servers.append((server,thread,log))
        urls[name] = f'http://127.0.0.1:{server.server_port}/'

    with sync_playwright() as playwright:
        browser = playwright.chromium.launch(channel='chrome',headless=True,args=['--enable-unsafe-webgpu'])
        browser_version = browser.version

        def open_page(name, renderer='webgl', sample=None, cost=False):
            context = browser.new_context(viewport={'width':1000,'height':800},device_scale_factor=1)
            page = context.new_page()
            errors = []
            page.on('pageerror',lambda error:errors.append(str(error)))
            page.on('console',lambda message:errors.append(message.text) if message.type=='error' else None)
            query = f'?dorotiTestbedMode=sample&dorotiRenderer=worker-direct-{renderer}'
            if sample:
                query += '&dorotiSample='+sample
            if cost:
                query += '&dorotiFrameCost=1'
            try:
                page.goto(urls[name]+query)
                page.wait_for_function('["started","failed"].includes(document.documentElement.dataset.dorotiBootstrapStage)',timeout=120000)
                assert page.evaluate('document.documentElement.dataset.dorotiBootstrapStage') == 'started',errors
                page.wait_for_selector('#doroti-semantics [data-doroti-semantics-id]',state='attached',timeout=90000)
                assert page.evaluate('document.documentElement.dataset.dorotiRenderer') == 'worker-direct-'+renderer
                assert page.evaluate('crossOriginIsolated && getDotnetRuntime(0).localHeapViewU8().buffer instanceof SharedArrayBuffer')
                assert not errors,errors
                return context,page,errors
            except BaseException:
                page.screenshot(path=str(out/f'{name}-{renderer}-startup-failure.png'))
                (out/f'{name}-{renderer}-startup-failure.json').write_text(json.dumps({'errors':errors,'dataset':page.evaluate('({...document.documentElement.dataset})')},indent=2))
                context.close()
                raise

        # Exercise the actual app, not just the generated bootstrap receipt.
        for renderer in ([] if args.compare_only else ['webgl','webgpu']):
            print(f'Checking AOT {renderer}: Material and navigation/text',flush=True)
            context,page,errors = open_page('aot',renderer)
            page.wait_for_function('Array.from(document.querySelectorAll("[aria-label]")).some(e=>e.getAttribute("aria-label").includes("Components"))')
            page.screenshot(path=str(out/f'aot-{renderer}-components.png'))
            first_label = page.get_by_role('group',name='Actions\nCommon buttons',exact=True)
            first_label.wait_for(state='attached')
            original_y = first_label.bounding_box()['y']
            page.mouse.move(400,650)
            page.mouse.wheel(0,600)
            page.wait_for_function('(y)=>{ const e=Array.from(document.querySelectorAll("[aria-label]")).find(e=>e.getAttribute("aria-label")==="Actions\\nCommon buttons"); return !e || e.getBoundingClientRect().y < y-100; }',arg=original_y)
            page.set_viewport_size({'width':700,'height':700})
            # Desktop retains a larger physical canvas backing; its logical root
            # and framework navigation geometry should follow the viewport.
            page.wait_for_function('(()=>{ const root=document.querySelector(".doroti-root").getBoundingClientRect(); const tab=document.querySelector("#doroti-semantics [role=tab][aria-label=Components]")?.getBoundingClientRect(); return root.width === 700 && root.height === 700 && tab && tab.y > 600 && tab.bottom <= 700; })()')
            assert page.locator('#doroti-semantics [data-doroti-semantics-id]').count()>10
            assert not errors,errors
            context.close()
            context,page,errors = open_page('aot',renderer,'navigation')
            textbox = page.get_by_role('textbox')
            textbox.wait_for(state='attached')
            box = textbox.bounding_box()
            assert box and box['height']<200
            try:
                page.mouse.click(box['x']+20,box['y']+box['height']/2,delay=100)
                page.wait_for_function('document.activeElement?.id === "doroti-ime"')
                page.keyboard.insert_text('AOT 한글')
                page.wait_for_function('document.querySelector("#doroti-ime").value === "AOT 한글"')
                page.keyboard.press('Home')
                page.keyboard.press('Shift+ArrowRight')
                page.wait_for_function('(()=>{ const e=document.querySelector("#doroti-ime"); return e.selectionStart === 0 && e.selectionEnd === 1; })()')
                assert not errors,errors
            except BaseException:
                page.screenshot(path=str(out/f'{renderer}-text-failure.png'))
                (out/f'{renderer}-text-failure.json').write_text(json.dumps({'errors':errors,
                    'state':page.evaluate('({active:document.activeElement?.outerHTML,input:document.getElementById("doroti-ime")?.outerHTML,selection:{start:document.getElementById("doroti-ime")?.selectionStart,end:document.getElementById("doroti-ime")?.selectionEnd},root:{...document.querySelector(".doroti-root")?.dataset},dataset:{...document.documentElement.dataset},labels:Array.from(document.querySelectorAll("[aria-label]")).map(e=>e.getAttribute("aria-label"))})')},indent=2))
                raise
            records.append({'kind':'smoke','mode':'aot','renderer':renderer,'material':True,'scrollResize':True,
                'syntheticTextAndSelection':True,'errors':errors})
            context.close()

        if args.baseline:
            for pair in range(args.pairs):
                order = ['baseline','aot'] if pair%2==0 else ['aot','baseline']
                for name in order:
                    print(f'Warm CPU pair {pair+1}: {name}',flush=True)
                    context,page,errors = open_page(name,cost=True)
                    start_progress(page,f'{name}-pair-{pair+1}')
                    page.wait_for_timeout(2500)
                    before = page.evaluate('async()=>await __dorotiFrameCost("diagnostics")')
                    page.screenshot(path=str(out/f'{name}-pair-{pair+1}-progress.png'))
                    page.evaluate('async()=>await __dorotiFrameCost("reset")')
                    page.wait_for_timeout(6000)
                    result = page.evaluate('async()=>await __dorotiFrameCost("finish")')
                    rows = result['trace']['rows']
                    assert result['trace']['dropped']==0 and phase(rows,1)['count']>=30
                    assert phase(rows,3)['count']>=30
                    assert not errors,errors
                    (out/f'{name}-pair-{pair+1}.json').write_text(json.dumps({'before':before,'result':result},indent=2))
                    records.append({'kind':'warmProgress','mode':name,'pair':pair+1,'renderer':'webgl',
                        'frameworkCallback':phase(rows,1),'managedRasterSubmit':phase(rows,3),
                        'ownerCpuUnionMs':interval_union_ms(rows),
                        'ownerAllocatedBytes':result['after']['managed']['ownerAllocatedBytes'],
                        'wasmCapacityBytes':result['after']['wasmBytes'],'errors':errors})
                    context.close()
        browser.close()

    summary = {}
    for name in ['baseline','aot']:
        values = [record for record in records if record.get('kind')=='warmProgress' and record['mode']==name]
        if values:
            summary[name] = {'medianTrialCallbackP95Ms':statistics.median(record['frameworkCallback']['p95Ms'] for record in values),
                'medianTrialRasterP95Ms':statistics.median(record['managedRasterSubmit']['p95Ms'] for record in values),
                'trials':len(values)}
    (out/'result.json').write_text(json.dumps({'status':'PASS','browser':browser_version,'records':records,'summary':summary,
        'scope':'Desktop headless synthetic input; warm progress exploratory CPU timing, without matched logical target traces',
        'callbackIncludesNestedWebglRaster':True,'displayFPS':'notMeasured','physicalIME':'notVerified'},indent=2))
    print(json.dumps(summary),flush=True)
except BaseException as error:
    (out/'result.json').write_text(json.dumps({'status':'FAIL','error':str(error),'records':records},indent=2))
    raise
finally:
    for server,thread,log in servers:
        server.shutdown()
        server.server_close()
        thread.join(timeout=5)
        log.close()
