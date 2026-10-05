"""Compare managed delta packets with full snapshots in the actual Web DOM host.

Run after --web-semantics <packets.json> and a Testbed Web Debug build, through
eng/run-with-timeout.py. Chrome automation does not qualify physical IME or AT.
"""
import argparse
import json
import os
from pathlib import Path
import socket
import subprocess
import sys
import time
import urllib.request
from playwright.sync_api import sync_playwright

ROOT = Path(__file__).resolve().parents[2]
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--packets', type=Path, required=True)
parser.add_argument('--output', type=Path, required=True)
parser.add_argument('--services', action='store_true', help='Also run the broader existing file/navigation/text/drop service regression.')
args = parser.parse_args()
out = args.output.resolve()
if not out.is_relative_to(ROOT / 'temp/testing') or out.exists():
    raise RuntimeError('Use a new directory under temp/testing.')
out.mkdir(parents=True)
fixtures = json.loads(args.packets.read_text(encoding='utf-8'))
with socket.socket() as sock:
    sock.bind(('127.0.0.1', 0))
    port = sock.getsockname()[1]
url = f'http://127.0.0.1:{port}/'
log = (out / 'server.log').open('w', encoding='utf-8')
server = subprocess.Popen(['dotnet', 'run', '--project', 'samples/DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj',
    '-c', 'Debug', '--no-build', '--no-launch-profile'], cwd=ROOT,
    env=os.environ | {'ASPNETCORE_URLS': url.rstrip('/')}, stdout=log, stderr=subprocess.STDOUT)

DOM = '''() => Array.from(document.querySelectorAll('#doroti-semantics [data-doroti-semantics-id]')).map(e => ({
    id:Number(e.dataset.dorotiSemanticsId), tag:e.tagName,
    attributes:Array.from(e.attributes).filter(a=>!['id','style','data-doroti-semantics-id'].includes(a.name)).map(a=>[a.name,a.value]).sort(),
    style:{left:e.style.left,top:e.style.top,width:e.style.width,height:e.style.height},
    children:Array.from(e.children).map(c=>Number(c.dataset.dorotiSemanticsId)),
    value: 'value' in e ? e.value : null,
    selection:'selectionStart' in e ? [e.selectionStart,e.selectionEnd,e.selectionDirection] : null
}))'''

def wait_live(page):
    try:
        page.wait_for_function('["started","failed"].includes(document.documentElement.dataset.dorotiBootstrapStage)', timeout=90000)
        assert page.evaluate('document.documentElement.dataset.dorotiBootstrapStage') == 'started', errors
        page.wait_for_selector('#doroti-semantics [data-doroti-semantics-id]', state='attached', timeout=30000)
    except BaseException as error:
        page.screenshot(path=str(out / 'failure.png'))
        (out / 'failure.json').write_text(json.dumps({'error':str(error),'errors':errors,'renderer':renderer,
            'url':page.url,'state':page.evaluate('({dataset:{...document.documentElement.dataset},semantics:document.getElementById("doroti-semantics")?.outerHTML,body:document.body.innerText})')}, ensure_ascii=False, indent=2))
        raise

try:
    deadline = time.monotonic() + 60
    while True:
        try:
            urllib.request.urlopen(url, timeout=2).close()
            break
        except OSError:
            if server.poll() is not None or time.monotonic() > deadline:
                raise RuntimeError('Web server did not start.')
            time.sleep(.2)
    with sync_playwright() as playwright:
        browser = playwright.chromium.launch(channel='chrome', headless=True, args=['--enable-unsafe-webgpu'])
        records = []
        for renderer in ['webgl', 'webgpu']:
            print(f'Checking {renderer}: live managed host and DOM equivalence', flush=True)
            errors = []
            delta_page = browser.new_page(viewport={'width': 800, 'height': 600})
            full_page = browser.new_page(viewport={'width': 800, 'height': 600})
            for page in [delta_page, full_page]:
                page.on('pageerror', lambda error: errors.append(str(error)))
                page.goto(url + f'?dorotiSample=navigation&dorotiRenderer=worker-direct-{renderer}')
                wait_live(page)
                page.evaluate('''async () => {
                    const element=document.querySelector('#doroti-semantics [data-doroti-semantics-id]');
                    window.fixtureHost=Number(element.id.match(/^doroti-semantics-(\\d+)-/)[1]);
                    window.fixtureModule=await import('./_content/Doroti.Host.Web/doroti.web.js');
                    window.fixtureApply=packet=>fixtureModule.updateSemantics(fixtureHost,JSON.stringify({...packet,stream:packet.stream+1000000}));
                }''')
            # A revision gap uses the actual main -> render Worker -> managed snapshot response.
            # Verify the live owner before installing synthetic fixture streams.
            print(f'Checking {renderer}: managed snapshot recovery', flush=True)
            recovery = delta_page.evaluate('''async () => {
                const module=await import('./_content/Doroti.Host.Web/doroti.web.js');
                const root=document.getElementById('doroti-semantics');
                const element=root.querySelector('[data-doroti-semantics-id]');
                const host=Number(element.id.match(/^doroti-semantics-(\\d+)-/)[1]);
                const revision=Number(root.dataset.revision), stream=Number(root.dataset.stream);
                module.updateSemantics(host, JSON.stringify({schemaVersion:1,kind:'delta',stream,
                    revision:revision+100,baseRevision:revision+99,generation:999,nodes:[],removed:[]}));
                return {revision,stream};
            }''')
            delta_page.wait_for_function('(old)=>Number(document.getElementById("doroti-semantics").dataset.revision)>old.revision', arg=recovery)
            assert delta_page.evaluate('Number(document.getElementById("doroti-semantics").dataset.stream)') == recovery['stream']
            checks = []
            for index, step in enumerate(fixtures['steps']):
                if step['packet'] is not None:
                    delta_page.evaluate('(packet)=>fixtureApply(packet)', step['packet'])
                full_page.evaluate('(packet)=>fixtureApply(packet)', step['snapshot'] | {'stream': 2000000, 'revision': index + 1})
                actual, expected = delta_page.evaluate(DOM), full_page.evaluate(DOM)
                assert actual == expected, (renderer, step['name'], actual, expected)
                if step['name'] == 'initial':
                    delta_page.evaluate('''() => {
                        window.fixtureEditor=document.querySelector('[data-doroti-semantics-id="4"]');
                        fixtureEditor.focus(); fixtureEditor.setSelectionRange(1,3);
                        window.fixtureButton=document.querySelector('[data-doroti-semantics-id="3"]');
                    }''')
                elif step['name'] in ['parent-motion', 'editing', 'unchanged', 'reorder', 'reparent',
                                      'identifier-removed', 'identifier-added', 'branch-removed']:
                    assert delta_page.evaluate('fixtureEditor===document.querySelector("[data-doroti-semantics-id=\\"4\\"]")'), step['name']
                    assert delta_page.evaluate('document.activeElement===fixtureEditor'), (step['name'], 'focus lost')
                    assert delta_page.evaluate('fixtureButton===document.querySelector("[data-doroti-semantics-id=\\"3\\"]")'), step['name']
                if step['name'] == 'reorder' and renderer == 'webgl':
                    # Exercise focus restoration on browsers without the state-preserving move API.
                    delta_page.evaluate('Object.defineProperty(HTMLElement.prototype,"moveBefore",{value:undefined,configurable:true})')
                checks.append(step['name'])
                print(f'PASS: {renderer}/{step["name"]}', flush=True)
            assert not errors, errors
            delta_page.screenshot(path=str(out / f'{renderer}-recovered.png'))
            records.append({'renderer': renderer, 'checks': checks, 'managedResync': 'PASS', 'errors': errors})
            delta_page.close()
            full_page.close()
            if args.services:
                records[-1]['services'] = 'PARTIAL'
                with (out / f'{renderer}-services.log').open('w', encoding='utf-8') as services_log:
                    result = subprocess.run([sys.executable, str(ROOT / 'Doroti/tests/web_services_smoke.py'), '--url', url,
                        '--renderer', renderer, '--output', str(out / f'{renderer}-services')], cwd=ROOT,
                        stdout=services_log, stderr=subprocess.STDOUT)
                if result.returncode:
                    raise RuntimeError((out / f'{renderer}-services.log').read_text(encoding='utf-8'))
                records[-1]['services'] = 'PASS'
                print(f'PASS: {renderer}/live files, navigation, text/reload and drop services', flush=True)
        browser.close()
    (out / 'result.json').write_text(json.dumps({'status':'PASS','browser':browser.version,'records':records,
        'baselineChars':fixtures['baselineBytes'],'deltaChars':fixtures['deltaBytes'],
        'physicalInput':'notVerified','physicalAT':'notVerified','performanceFPS':'notMeasured'}, indent=2))
    print('PASS: WebGL/WebGPU managed delta/full DOM equivalence, focus/selection, topology and managed resync.', flush=True)
except BaseException as error:
    (out / 'result.json').write_text(json.dumps({'status':'PARTIAL','error':str(error),
        'records':locals().get('records',[]),'physicalInput':'notVerified','physicalAT':'notVerified','performanceFPS':'notMeasured'}, indent=2))
    if 'delta_page' in locals() and not delta_page.is_closed():
        try:
            delta_page.screenshot(path=str(out / 'failure.png'))
            (out / 'failure.json').write_text(json.dumps({'error':str(error),'errors':errors,'renderer':renderer,
                'url':delta_page.url,'state':delta_page.evaluate('({dataset:{...document.documentElement.dataset},semantics:document.getElementById("doroti-semantics")?.outerHTML,body:document.body.innerText})')}, ensure_ascii=False, indent=2))
        except Exception:
            pass
    raise
finally:
    if os.name == 'nt':
        subprocess.run(['taskkill', '/PID', str(server.pid), '/T', '/F'], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
    else:
        server.terminate()
    server.wait(timeout=10)
    log.close()
