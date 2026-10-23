"""Production DOM text input in Chrome/WebKit; worker mocked, OS IME not exercised.

Run with eng/run-with-timeout.py --timeout 1200 after compiling the Web host.
Chrome also uses CDP's browser composition commands, not just dispatched events.
"""
import argparse
import hashlib
import json
from pathlib import Path
from urllib.parse import urlparse
from playwright.sync_api import sync_playwright

ROOT = Path(__file__).resolve().parents[2]
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--js-root', type=Path, required=True)
parser.add_argument('--output', type=Path, required=True)
args = parser.parse_args()
assets, out = args.js_root.resolve(), args.output.resolve()
if not (assets / 'doroti.web.js').is_file():
    raise RuntimeError('Compile the Web host TypeScript first.')
if not out.is_relative_to(ROOT / 'temp/testing') or out.exists():
    raise RuntimeError('Use a new output directory under temp/testing.')
out.mkdir(parents=True)

HTML = '''<!doctype html><link rel="stylesheet" href="/doroti.web.css"><div id="app"></div><script type="module">
import * as api from './assets/doroti.web.js';
import {dorotiProtocolVersion} from './assets/doroti.web.protocol.js';
window.fixture={api,inputs:[],events:[]};
// Rendering is outside this DOM fixture. Windows WebKit may omit transferable
// canvases; the mocked worker never consumes this placeholder surface.
globalThis.OffscreenCanvas ??= class {};
HTMLCanvasElement.prototype.transferControlToOffscreen ??= function(){return {};};
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
api.showHost(1);
fixture.input=document.querySelector('#doroti-ime');
fixture.apply=(text,base,extent,attach,ack=0)=>api.setTextInputState(
  1,text,base,extent,'text','enter',false,false,'none',false,2,false,attach,true,ack);
fixture.reset=(text,base,extent=base)=>{
  api.clearTextInput(1);fixture.apply(text,base,extent,true);fixture.inputs=[];fixture.events=[];
};
fixture.states=()=>fixture.inputs.filter(x=>x.inputKind==='text');
fixture.last=()=>fixture.states().at(-1)?.payload;
for(const type of ['compositionstart','compositionupdate','compositionend','beforeinput','input'])
  fixture.input.addEventListener(type,e=>fixture.events.push({type,data:e.data,value:fixture.input.value,
    start:fixture.input.selectionStart,end:fixture.input.selectionEnd,trusted:e.isTrusted}));
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


SCENARIOS = '''async () => {
  const f=fixture,i=f.input,checks=[];
  const tick=()=>new Promise(resolve=>setTimeout(resolve,0));
  function check(name,condition) { if(!condition) throw new Error(name+': '+JSON.stringify(f.last())); checks.push(name); }
  function event(type,data='') { i.dispatchEvent(new CompositionEvent(type,{data,bubbles:true})); }
  function input(data=null) { i.dispatchEvent(new InputEvent('input',{data,inputType:data===null?'insertText':'insertCompositionText',bubbles:true,isComposing:true})); }
  function expect(name,text,base,extent,cb,ce) {
    const s=f.last();check(name, s?.text===text&&s.selectionBase===base&&s.selectionExtent===extent&&s.composingBase===cb&&s.composingExtent===ce);
  }
  function preedit(text,caret,update=true) {
    if(update) event('compositionupdate',text);
    i.value='ab'+text+'cd';i.setSelectionRange(caret,caret);input(update?text:null);
  }
  f.reset('abcd',2);event('compositionstart');
  const count=f.states().length;
  event('compositionupdate','你好');
  check('pre-DOM compositionupdate publishes no mismatched revision',f.states().length===count);
  i.value='ab你好cd';i.setSelectionRange(3,3);input('你好');
  expect('middle caret retains whole preedit','ab你好cd',3,3,2,4);
  i.setSelectionRange(2,2);document.dispatchEvent(new Event('selectionchange'));
  expect('cursor-only start retains whole preedit','ab你好cd',2,2,2,4);
  i.setSelectionRange(4,4);document.dispatchEvent(new Event('selectionchange'));
  expect('cursor-only end retains whole preedit','ab你好cd',4,4,2,4);
  i.setSelectionRange(2,3,'backward');document.dispatchEvent(new Event('selectionchange'));
  expect('backward selection is independent of preedit','ab你好cd',3,2,2,4);
  const duplicate=f.states().length;document.dispatchEvent(new Event('selectionchange'));
  check('identical native state suppressed',f.states().length===duplicate);
  f.apply('stale',0,0,false,0);
  check('stale worker response rejected',i.value==='ab你好cd'&&i.selectionDirection==='backward');
  f.apply('formatter',0,0,false,f.states().at(-1).inputSequence);
  check('current formatter response cannot overwrite active composition',i.value==='ab你好cd'&&i.selectionEnd===3);
  preedit('你',3);expect('preedit shrink','ab你cd',3,3,2,3);
  preedit('😀😀',4);expect('UTF-16 preedit and middle caret','ab😀😀cd',4,4,2,6);
  event('compositionend','😀😀');expect('commit clears composition only','ab😀😀cd',4,4,-1,-1);
  f.reset('abcd',2);event('compositionstart');preedit('你好',3,false);
  expect('missing compositionupdate uses document anchors','ab你好cd',3,3,2,4);
  preedit('你好',4);event('compositionupdate','好');input('好');
  expect('partial commit narrows active preedit','ab你好cd',4,4,3,4);
  event('compositionend','好');
  f.reset('abXXcd',2,4);event('compositionstart');preedit('你好',3);
  expect('selected text replacement keeps suffix','ab你好cd',3,3,2,4);
  preedit('',2);event('compositionend');expect('cancel removes preedit','abcd',2,2,-1,-1);
  f.reset('aaaa',2);event('compositionstart');event('compositionupdate','aa');
  i.value='aaaaaa';i.setSelectionRange(3,3);input('aa');
  expect('repeated surrounding text is unambiguous','aaaaaa',3,3,2,4);
  event('compositionend','aa');
  f.reset('abcd',2);event('compositionstart');preedit('你好',3);
  event('compositionupdate','old queued data');f.reset('new field',4);
  await tick();
  check('clear/reattach retires old composition callback',i.value==='new field'&&i.selectionStart===4&&f.states().length===0);
  i.value='new Xfield';i.setSelectionRange(5,5);input();
  expect('new field edit has no stale composing range','new Xfield',5,5,-1,-1);
  f.reset('aaaa',2);
  for(let n=0;n<12;n++) { i.setRangeText('a',i.selectionStart,i.selectionEnd,'end');input(); }
  expect('12 rapid middle inserts','a'.repeat(16),14,14,-1,-1);
  for(let n=0;n<12;n++) { i.setRangeText('',i.selectionStart-1,i.selectionStart,'end');input(); }
  expect('12 rapid middle backspaces','aaaa',2,2,-1,-1);
  await tick();
  const states=f.states();check('input sequences strictly increase',states.every((s,n)=>!n||s.inputSequence>states[n-1].inputSequence));
  return checks;
}'''

records = []
try:
    with sync_playwright() as playwright:
        for engine, options in [(playwright.webkit, {}), (playwright.chromium, {'channel': 'chrome'})]:
            browser = engine.launch(headless=True, **options)
            context = browser.new_context(viewport={'width': 800, 'height': 600})
            context.route('http://127.0.0.1:51996/**', route_request)
            page = context.new_page()
            errors = []
            def on_error(error):
                errors.append(str(error))
                print(f'Browser error: {error}', flush=True)
            page.on('pageerror', on_error)
            page.goto('http://127.0.0.1:51996/')
            page.wait_for_function('window.fixture?.ready')
            checks = page.evaluate(SCENARIOS)
            cdp_cases = []
            if engine.name == 'chromium':
                session = context.new_cdp_session(page)
                for outcome in ['commit', 'cancel']:
                    page.evaluate("fixture.reset('abcd',2)")
                    session.send('Input.imeSetComposition', {'text': '你好', 'selectionStart': 1, 'selectionEnd': 1})
                    state = page.evaluate('fixture.last()')
                    assert state == {'text': 'ab你好cd', 'selectionBase': 3, 'selectionExtent': 3,
                                     'composingBase': 2, 'composingExtent': 4}, state
                    session.send('Input.imeSetComposition', {'text': '你好', 'selectionStart': 0, 'selectionEnd': 0})
                    state = page.evaluate('fixture.last()')
                    assert state['selectionExtent'] == 2 and state['composingExtent'] == 4, state
                    if outcome == 'commit':
                        session.send('Input.insertText', {'text': '你好'})
                    else:
                        session.send('Input.imeSetComposition', {'text': '', 'selectionStart': 0, 'selectionEnd': 0})
                    state = page.evaluate('fixture.last()')
                    assert state['text'] == ('ab你好cd' if outcome == 'commit' else 'abcd'), state
                    assert state['composingBase'] == state['composingExtent'] == -1, state
                    cdp_cases.append({'outcome': outcome, 'state': state, 'events': page.evaluate('fixture.events')})
                checks.append('Chrome browser composition: middle cursor, cursor-only, commit and cancel')
            assert not errors, errors
            records.append({'browser': engine.name, 'version': browser.version, 'checks': checks, 'cdp': cdp_cases})
            print(f'PASS: {engine.name}: {len(checks)} text input checks', flush=True)
            context.close()
            browser.close()
    result = {'status': 'PASS', 'records': records, 'physicalIme': 'notVerified',
              'appleSafari': 'notVerified', 'managedWorkerRoundTrip': 'notVerified', 'framePacing': 'notVerified',
              'hostJsSha256': hashlib.sha256((assets / 'doroti.web.js').read_bytes()).hexdigest()}
    (out / 'result.json').write_text(json.dumps(result, indent=2, ensure_ascii=False), encoding='utf-8')
except BaseException as error:
    (out / 'result.json').write_text(json.dumps({'status': 'FAIL', 'error': str(error), 'records': records}, indent=2), encoding='utf-8')
    raise
