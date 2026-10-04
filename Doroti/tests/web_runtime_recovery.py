"""Chrome graphics-loss/restart integration. Use the 1200-second wrapper."""
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
out = Path(sys.argv[1]).resolve()
assert out.is_relative_to(ROOT / 'temp/testing') and not out.exists()
out.mkdir(parents=True)
with socket.socket() as sock:
    sock.bind(('127.0.0.1', 0)); port = sock.getsockname()[1]
url = f'http://127.0.0.1:{port}/'
log = (out / 'server.log').open('w', encoding='utf-8')
server_command = ['dotnet', 'run', '--project', str(ROOT / 'samples/DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj'),
    '-c', 'Debug', '--no-build', '--no-launch-profile']
if os.environ.get('DOROTI_WEB_TEST_ARTIFACTS'):
    artifacts = Path(os.environ['DOROTI_WEB_TEST_ARTIFACTS']).resolve()
    assert artifacts.is_relative_to(ROOT / 'temp/testing')
    server_command += ['--artifacts-path', str(artifacts)]
server = subprocess.Popen(server_command, cwd=ROOT, env=os.environ | {'ASPNETCORE_URLS': url.rstrip('/')}, stdout=log, stderr=subprocess.STDOUT)
try:
    deadline = time.monotonic() + 60
    while True:
        try: urllib.request.urlopen(url, timeout=2).close(); break
        except OSError:
            if server.poll() is not None or time.monotonic() > deadline: raise RuntimeError('Web server did not start.')
            time.sleep(.2)
    with sync_playwright() as p:
        browser = p.chromium.launch(channel='chrome', headless=True, args=['--enable-unsafe-webgpu'])
        records = []
        for renderer, runtime in ([('webgl','worker')] if '--worker-only' in sys.argv else
            [('webgl','main'), ('webgpu','main')] if '--main-only' in sys.argv else
            [('webgl','main'), ('webgpu','main'), ('webgl','worker')]):
            page = browser.new_page(viewport={'width':1000,'height':800})
            errors = []
            requests = []
            page.on('requestfailed', lambda request: requests.append(dict(url=request.url,error=request.failure)))
            page.on('response', lambda response: requests.append(dict(url=response.url,status=response.status)) if response.status >= 400 else None)
            page.on('pageerror', lambda error: errors.append(str(error)))
            page.on('console', lambda message: errors.append(message.text) if message.type == 'error' else None)
            page.add_init_script('window.runtimeEvents=[]; window.addEventListener("doroti-runtime-state",e=>runtimeEvents.push(e.detail));')
            page.goto(url + f'?dorotiSample=native-texture-probe&dorotiRenderer=worker-direct-{renderer}&dorotiRuntimeLocation={runtime}&dorotiResizeDiagnostics=1')
            try:
                page.wait_for_function('["started","failed"].includes(document.documentElement.dataset.dorotiBootstrapStage)', timeout=90000)
                if '--expect-threaded-rejection' in sys.argv:
                    assert page.evaluate('document.documentElement.dataset.dorotiBootstrapStage') == 'failed'
                    assert any('WasmEnableThreads=false' in error for error in errors), errors
                    (out / 'result.json').write_text(json.dumps(dict(status='PASS',threadedStandaloneRejected=True,errors=errors),indent=2))
                    print('PASS: threaded standalone ownership rejected with explicit WasmEnableThreads=false/main diagnostic.')
                    page.close(); browser.close()
                    sys.exit(0)
                assert page.evaluate('document.documentElement.dataset.dorotiBootstrapStage') == 'started', errors
            except Exception:
                page.screenshot(path=str(out / f'{renderer}-{runtime}-startup.png'))
                (out / 'failure.json').write_text(json.dumps(dict(renderer=renderer,runtime=runtime,errors=errors,requests=requests,dataset=page.evaluate('({...document.documentElement.dataset})'),root=page.evaluate('({...document.querySelector(".doroti-root")?.dataset})')),indent=2))
                raise
            page.evaluate('async()=>{window.runtimeApi=await import("./_content/Doroti.Host.Web/doroti.web.js");}')
            page.wait_for_function('runtimeApi.getDorotiRuntimeState()?.state === "ready"')
            page.wait_for_function('JSON.parse(__dorotiResizeDiagnostics.presenter("doroti-surface")).frontGeneration > 0', timeout=90000)
            before = page.evaluate('runtimeApi.getDorotiRuntimeState()')
            if runtime == 'main':
                clipboard = page.evaluate('''async()=>{
                    window.clipboardReads=0;
                    Object.defineProperty(navigator,'clipboard',{configurable:true,value:{readText(){clipboardReads++;throw Error('Implicit read forbidden');}}});
                    const exports=await getDotnetRuntime(0).getAssemblyExports('DorotiTestbedApp.Web.dll');
                    const probe=exports.DorotiTestbedApp.Web.Validation.WebTextureExport;
                    return {availability:await probe.ClipboardAvailability(),hasStrings:await probe.ClipboardHasStrings(),reads:clipboardReads};
                }''')
                assert clipboard == {'availability':'Unknown','hasStrings':True,'reads':0}, clipboard
            if runtime == 'main':
                page.evaluate('__dorotiResizeDiagnostics.loseContext("doroti-surface")')
                if renderer == 'webgl':
                    page.wait_for_function('document.querySelector(".doroti-root").dataset.dorotiPlatformContextLost === "true"')
                    page.evaluate('__dorotiResizeDiagnostics.restoreContext("doroti-surface")')
                try:
                    page.wait_for_function('["lost","failed"].includes(runtimeApi.getDorotiRuntimeState()?.state)', timeout=15000)
                except BaseException:
                    (out / 'loss-failure.json').write_text(json.dumps(dict(renderer=renderer, errors=errors,
                        state=page.evaluate('runtimeApi.getDorotiRuntimeState()'), events=page.evaluate('runtimeEvents'),
                        dataset=page.evaluate('({...document.documentElement.dataset})')),indent=2))
                    raise
                with page.expect_navigation(wait_until='load',timeout=30000):
                    page.evaluate('runtimeApi.restartDorotiWebHost()')
                page.wait_for_function('["started","failed"].includes(document.documentElement.dataset.dorotiBootstrapStage)', timeout=90000)
                stage = page.evaluate('document.documentElement.dataset.dorotiBootstrapStage')
                if stage!='started':
                    (out / 'restart-failure.json').write_text(json.dumps(dict(renderer=renderer,errors=errors,requests=requests,
                        dataset=page.evaluate('({...document.documentElement.dataset})')),indent=2))
                    raise RuntimeError('Restart bootstrap failed: '+repr(errors))
                page.evaluate('async()=>{window.runtimeApi=await import("./_content/Doroti.Host.Web/doroti.web.js");}')
            else:
                page.evaluate('window.oldCanvas=document.getElementById("doroti-surface"); __dorotiResizeDiagnostics.crashWorker("doroti-surface")')
                page.wait_for_function('runtimeApi.getDorotiRuntimeState()?.state === "ready" && runtimeApi.getDorotiRuntimeState().generation > 1', timeout=90000)
                assert page.evaluate('oldCanvas !== document.getElementById("doroti-surface")')
                generation = page.evaluate('runtimeApi.getDorotiRuntimeState().generation')
                page.evaluate('runtimeApi.restartDorotiWebHost()')
                page.wait_for_function('(generation)=>runtimeApi.getDorotiRuntimeState()?.state === "ready" && runtimeApi.getDorotiRuntimeState().generation > generation', arg=generation, timeout=90000)
            page.wait_for_function('JSON.parse(__dorotiResizeDiagnostics.presenter("doroti-surface")).frontGeneration > 0', timeout=90000)
            state = page.evaluate('runtimeApi.getDorotiRuntimeState()')
            assert state['renderer'] == before['renderer'], (before,state)
            page.evaluate('document.getElementById("doroti-surface").focus()')
            assert page.evaluate('document.activeElement.id') == 'doroti-surface'
            receipt = page.evaluate('JSON.parse(__dorotiResizeDiagnostics.presenter("doroti-surface"))')
            assert state['state'] == 'ready' and receipt['unpairedRequestCount'] <= 1, (state,receipt)
            page.set_viewport_size({'width':1000,'height':1700})
            page.wait_for_function('JSON.parse(__dorotiResizeDiagnostics.presenter("doroti-surface")).rasterHeight >= 1700', timeout=30000)
            resized = page.evaluate('JSON.parse(__dorotiResizeDiagnostics.presenter("doroti-surface"))')
            assert resized['displayWidth'] == receipt['displayWidth'], (receipt,resized)
            assert resized['displayHeight'] >= resized['rasterHeight'], resized
            fixture_requests = []
            page.on('request', lambda request: fixture_requests.append(request.url) if '/doroti-webview-fixture/' in request.url else None)
            page.route('**/doroti-webview-fixture/**', lambda route: route.fulfill(status=200,content_type='text/html',body='<title>loopback fixture</title>'))
            origins = page.evaluate('''async()=>{
                const {BrowserWebView}=await import('./_content/Doroti.Host.Web/doroti.web.webview.js');
                const result=[];
                for (const [name,allowed] of [['empty',[]],['match',[location.origin]],['null',null]]) {
                    const view=new BrowserWebView({viewId:1}, {Profile:2,AllowedOrigins:allowed}, ()=>{}, document);
                    const loaded=new Promise(resolve=>view.element.addEventListener('load',resolve,{once:true}));
                    document.body.append(view.element); await loaded;
                    const before=await view.execute({Operation:1}); const src=view.element.getAttribute('src');
                    const url=location.origin+'/doroti-webview-fixture/'+name;
                    let error=null;
                    try {await view.execute({Operation:2,Text:url});} catch(e) {error=e.code;}
                    const after=await view.execute({Operation:1});
                    if (name==='empty') {
                        if (error!==3 || src!==view.element.getAttribute('src') || before.NavigationId!==after.NavigationId || before.DocumentGeneration!==after.DocumentGeneration)
                            throw Error('Rejected origin mutated navigation state');
                    } else {
                        if(error!==null || view.element.src!==url || after.NavigationId!==before.NavigationId+1) throw Error('Allowed loopback navigation failed');
                        await new Promise(resolve=>view.element.addEventListener('load',resolve,{once:true}));
                    }
                    result.push({name,error,before,after}); view.dispose();
                }
                return result;
            }''')
            assert len(fixture_requests)==2 and not any(url.endswith('/empty') for url in fixture_requests), fixture_requests
            print(f'Checking {renderer}/{runtime} for 60 seconds after restart.', flush=True)
            page.wait_for_timeout(60000)
            stable = page.evaluate('runtimeApi.getDorotiRuntimeState()')
            assert stable['state']=='ready' and stable['renderer']==before['renderer'], stable
            page.screenshot(path=str(out / f'{renderer}-{runtime}-after-soak.png'))
            records.append(dict(renderer=renderer,runtime=runtime,before=before,after=state,receipt=receipt,resized=resized,
                originChecks=origins,loopbackRequests=fixture_requests,stableAfterSeconds=60,stable=stable,errors=errors))
            (out / 'checks.json').write_text(json.dumps(records,indent=2))
            page.close()
        (out / 'result.json').write_text(json.dumps(dict(status='PASS', browser=browser.version, checks=records,
            physicalInput='notVerified', restoration='only explicit serialized state'),indent=2), encoding='utf-8')
        browser.close()
        print('PASS: Chrome runtime loss/restart, fresh endpoint/first frame/focus, clipboard query without read; ' + ', '.join(r['renderer'] + '/' + r['runtime'] for r in records))
finally:
    if server.poll() is None:
        subprocess.run(['taskkill','/PID',str(server.pid),'/T','/F'],stdout=subprocess.DEVNULL,stderr=subprocess.DEVNULL) if os.name=='nt' else server.terminate()
    server.wait(timeout=10); log.close()
