"""Read-only browser qualification of an already published local Web candidate."""
from functools import partial
from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer
from pathlib import Path
import json
import sys
import threading
from playwright.sync_api import sync_playwright

ROOT = Path(__file__).resolve().parents[2]
candidate = Path(sys.argv[1]).resolve()
out = Path(sys.argv[2]).resolve()
assert candidate.is_relative_to(ROOT / 'temp/testing') and out.is_relative_to(ROOT / 'temp/testing') and not out.exists()
record = json.loads((candidate / 'candidate.json').read_text())
assert record['status'].startswith('PASS') and (candidate / 'web/wwwroot/index.html').is_file()
out.mkdir(parents=True)
log = (out / 'server.log').open('w',encoding='utf-8')
class Handler(SimpleHTTPRequestHandler):
    # Windows registry associations can label .mjs as text/plain.
    extensions_map = SimpleHTTPRequestHandler.extensions_map | {'.mjs':'text/javascript','.wasm':'application/wasm'}
    def end_headers(self):
        self.send_header('Cross-Origin-Opener-Policy','same-origin')
        self.send_header('Cross-Origin-Embedder-Policy','require-corp')
        super().end_headers()
    def log_message(self,format,*args): log.write(format % args + '\n')
server = ThreadingHTTPServer(('127.0.0.1',0),partial(Handler,directory=str(candidate / 'web/wwwroot')))
thread = threading.Thread(target=server.serve_forever,daemon=True); thread.start()
url = f'http://127.0.0.1:{server.server_port}/'
try:
    with sync_playwright() as p:
        browser = p.chromium.launch(channel='chrome',headless=True)
        page = browser.new_page(viewport={'width':1000,'height':800})
        messages = []; external = []
        page.on('console',lambda message: messages.append(message.text) if message.type == 'error' else None)
        page.on('pageerror',lambda error: messages.append(str(error)))
        def route(request_route):
            request_url = request_route.request.url
            if not request_url.startswith((url,'blob:','data:')):
                external.append(request_url); request_route.abort(); return
            request_route.continue_()
        page.route('**/*',route)
        try:
            page.goto(url + '?dorotiRenderer=worker-direct-webgl&dorotiResizeDiagnostics=1')
            page.wait_for_function('["started","failed"].includes(document.documentElement.dataset.dorotiBootstrapStage)',timeout=120000)
            assert page.evaluate('document.documentElement.dataset.dorotiBootstrapStage') == 'started', messages
            page.wait_for_function('JSON.parse(__dorotiResizeDiagnostics.presenter("doroti-surface")).frontGeneration > 0',timeout=120000)
            assert not external, external
            page.evaluate('document.getElementById("doroti-surface").focus()')
            assert page.evaluate('document.activeElement.id') == 'doroti-surface'
            page.screenshot(path=str(out / 'package.png'))
            receipt = page.evaluate('JSON.parse(__dorotiResizeDiagnostics.presenter("doroti-surface"))')
            (out / 'result.json').write_text(json.dumps(dict(status='PASS',candidateVersion=record['version'],browser=browser.version,
                firstFrame=True,focus=True,externalRequests=external,receipt=receipt,errors=messages,
                physicalInput='notVerified',koreanPixels='notVerified',server='loopback with COOP/COEP'),indent=2),encoding='utf-8')
            print('PASS: fresh NuGet-only Offline Web Release payload, first frame/focus and zero external requests.')
        except Exception:
            (out / 'failure.json').write_text(json.dumps(dict(messages=messages,external=external,dataset=page.evaluate('({...document.documentElement.dataset})')),indent=2))
            raise
        finally: browser.close()
finally:
    server.shutdown(); server.server_close(); thread.join(timeout=5); log.close()
