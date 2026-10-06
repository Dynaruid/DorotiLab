"""Exercise native text context-menu routing in the production DOM host.

Run through eng/run-with-timeout.py with Playwright, installed WebKit and Chrome.
--js-root points to the compiled Doroti.Host.Web TypeScript assets. The render
Worker is stubbed; real browser pointer events, capture and contextmenu dispatch
are used. This checks event routing, not the OS menu's visual presentation.
"""
import argparse
import json
from pathlib import Path
import sys
from urllib.parse import urlparse

from playwright.sync_api import sync_playwright


ROOT = Path(__file__).resolve().parents[2]
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--js-root', type=Path, required=True)
parser.add_argument('--output', type=Path, required=True)
args = parser.parse_args()
assets = args.js_root.resolve()
out = args.output.resolve()
if not (assets / 'doroti.web.js').is_file():
    raise RuntimeError('Compile the Web host TypeScript before running this test.')
if not out.is_relative_to(ROOT / 'temp/testing') or out.exists():
    raise RuntimeError('Use a new directory under temp/testing.')
out.mkdir(parents=True)

HTML = '''<!doctype html><link rel="stylesheet" href="/doroti.web.css"><div id="app"></div>
<script type="module">
import * as api from './assets/doroti.web.js';
import {dorotiProtocolVersion} from './assets/doroti.web.protocol.js';
window.fixture={api,events:[],inputs:[]};
window.Worker=class extends EventTarget {
    postMessage(message) {
        if(message.kind==='input') fixture.inputs.push(message);
        if(message.kind==='init') queueMicrotask(()=>this.dispatchEvent(new MessageEvent('message',{
            data:{protocolVersion:dorotiProtocolVersion,kind:'runtime-ready'}
        })));
    }
    terminate() {}
};
await api.startDorotiWorkerHost('worker-direct-webgl','worker');
api.setTextInputState(1,'Safari context menu',0,19,'text','enter',false,false,'none',false,2,false,true,true);
api.setEditableSizeAndTransform(1,300,80,JSON.stringify([1,0,0,0,0,1,0,0,0,0,1,0,40,40,0,1]));
api.showHost(1);
for(const type of ['pointerdown','pointerup','gotpointercapture','contextmenu'])
    document.addEventListener(type,event=>fixture.events.push({
        type,target:event.target.id,canceled:event.defaultPrevented,button:event.button
    }));
fixture.ready=true;
</script>'''


def route_request(route):
    path = urlparse(route.request.url).path
    if path == '/':
        route.fulfill(content_type='text/html', body=HTML)
    elif path == '/doroti.runtime-profile.json':
        route.fulfill(json={'schemaVersion': 1, 'threads': False})
    elif path == '/doroti.web.css':
        route.fulfill(content_type='text/css', path=str(ROOT / 'packages/platforms/web/Doroti.Host.Web/wwwroot/doroti.web.css'))
    elif path.startswith('/assets/') and '/' not in path[len('/assets/'):]:
        route.fulfill(content_type='text/javascript', path=str(assets / path[len('/assets/'):]))
    else:
        route.abort()


records = []
try:
    with sync_playwright() as playwright:
        for engine, options in [(playwright.webkit, {}), (playwright.chromium, {'channel': 'chrome'})]:
            browser = engine.launch(headless=True, **options)
            for platform in ['macOS', 'iOS', 'android']:
                context = browser.new_context(viewport={'width': 800, 'height': 600})
                # Exercise mobile framework-menu policy with a mouse as well.
                # UA/platform injection does not claim physical-device coverage.
                navigator_platform = {'macOS': 'MacIntel', 'iOS': 'iPhone', 'android': 'Linux armv8l'}[platform]
                user_agent = {'macOS': 'Macintosh', 'iOS': 'iPhone', 'android': 'Android'}[platform]
                context.add_init_script(f'''
                    Object.defineProperty(navigator,'platform',{{get:()=>{json.dumps(navigator_platform)}}});
                    Object.defineProperty(navigator,'userAgent',{{get:()=>{json.dumps(user_agent)}}});
                    Object.defineProperty(navigator,'userAgentData',{{get:()=>undefined}});
                    Object.defineProperty(navigator,'maxTouchPoints',{{get:()=>0}});
                ''')
                context.route('http://127.0.0.1:51997/**', route_request)
                page = context.new_page()
                errors = []
                page.on('pageerror', lambda error: errors.append(str(error)))
                page.goto('http://127.0.0.1:51997/')
                page.wait_for_function('window.fixture?.ready')
                assert page.locator('.doroti-root').get_attribute('data-doroti-operating-system') == platform
                checks = []

                def click_menu(name, x, y, *, allowed, target=None, modifiers=None):
                    page.evaluate('fixture.events=[];fixture.inputs=[]')
                    if modifiers:
                        for modifier in modifiers:
                            page.keyboard.down(modifier)
                    try:
                        page.mouse.click(x, y, button='left' if modifiers else 'right')
                    finally:
                        if modifiers:
                            for modifier in reversed(modifiers):
                                page.keyboard.up(modifier)
                    events = page.evaluate('fixture.events')
                    menus = [event for event in events if event['type'] == 'contextmenu']
                    assert len(menus) == 1, (name, events)
                    assert menus[0]['canceled'] != allowed, (name, events)
                    if target:
                        assert menus[0]['target'] == target, (name, events)
                    if allowed:
                        assert not any(event['type'] == 'gotpointercapture' for event in events), (name, events)
                        assert not next(event for event in events if event['type'] == 'pointerdown')['canceled'], (name, events)
                    inputs = page.evaluate('fixture.inputs')
                    assert any(message['payload'].get('phase') == 1 for message in inputs), (name, inputs)
                    assert any(message['payload'].get('phase') == 2 for message in inputs), (name, inputs)
                    checks.append({'name': name, 'events': events})

                desktop = platform == 'macOS'
                click_menu('focused IME right-click', 80, 60, allowed=desktop,
                           target='doroti-ime' if desktop else None)
                if desktop:
                    if sys.platform == 'darwin':
                        click_menu('IME Control-click', 80, 60, allowed=True, target='doroti-ime', modifiers=['Control'])
                    # Native accessibility editors share the same menu policy.
                    page.evaluate('''() => {
                        for(const [index,tag] of ['input','textarea'].entries()) {
                            const editor=document.createElement(tag);
                            editor.id=`native-${tag}`;
                            editor.value='Semantics text';
                            editor.style.cssText=`position:absolute;left:400px;top:${40+index*100}px;width:250px;height:60px;pointer-events:auto`;
                            document.getElementById('doroti-semantics').append(editor);
                        }
                    }''')
                    for index, tag in enumerate(['input', 'textarea']):
                        click_menu(f'semantics {tag} right-click', 450, 60 + index * 100, allowed=True, target=f'native-{tag}')
                    page.evaluate('fixture.api.setContextMenuEnabled(1,false)')
                    click_menu('explicit browser-menu disable', 80, 60, allowed=False)
                    page.evaluate('fixture.api.setContextMenuEnabled(1,true)')
                    click_menu('browser-menu re-enable', 80, 60, allowed=True, target='doroti-ime')
                    click_menu('canvas page menu blocked', 700, 500, allowed=False)

                page.evaluate('fixture.events=[];fixture.inputs=[]')
                page.mouse.move(80, 60)
                page.mouse.down()
                page.mouse.move(250, 90, steps=3)
                page.mouse.up()
                events = page.evaluate('fixture.events')
                assert next(event for event in events if event['type'] == 'pointerdown')['canceled'], events
                assert any(event['type'] == 'gotpointercapture' for event in events), events
                inputs = page.evaluate('fixture.inputs')
                assert all(any(message['payload'].get('phase') == phase for message in inputs) for phase in [1, 0, 2]), inputs
                assert not errors, errors
                checks.append({'name': 'primary drag remains framework-owned', 'events': events})
                records.append({'browser': engine.name, 'version': browser.version, 'platform': platform, 'checks': checks})
                print(f'PASS: {engine.name}/{platform}: {len(checks)} context-menu and pointer checks', flush=True)
                context.close()
            browser.close()
    (out / 'result.json').write_text(json.dumps({'status': 'PASS', 'records': records}, indent=2))
except BaseException as error:
    (out / 'result.json').write_text(json.dumps({'status': 'FAIL', 'error': str(error), 'records': records}, indent=2))
    raise
