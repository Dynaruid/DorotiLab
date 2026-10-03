"""Built Assets font preset: actual Skia Korean glyphs with no external requests.

Run after the Sample2 Assets + DorotiWebFontValidation build, under 1200s wrapper.
Own server/browser only; decoder denial is an explicit separate failure fixture.
"""
from pathlib import Path
import json
import os
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
server = subprocess.Popen(['dotnet','run','--project',str(ROOT / 'samples/DorotiSampleApp2/web/DorotiSampleApp2.Web.csproj'),
    '-c','Debug','--no-build','--no-launch-profile','-p:DorotiSampleWebFontSource=Assets','-p:DorotiWebFontValidation=true'],
    cwd=ROOT,env=os.environ | {'ASPNETCORE_URLS':url.rstrip('/')},stdout=log,stderr=subprocess.STDOUT)
try:
    deadline = time.monotonic() + 60
    while True:
        try: urllib.request.urlopen(url,timeout=2).close(); break
        except OSError:
            if server.poll() is not None or time.monotonic() > deadline: raise RuntimeError('Font fixture server did not start')
            time.sleep(.2)
    with sync_playwright() as p:
        browser = p.chromium.launch(channel='chrome',headless=True)
        checks = []
        for deny_decoder, deny_worker_csp in ((False,False),(True,False),(False,True)):
            page = browser.new_page(viewport={'width':1000,'height':800})
            messages = []; external = []; blocked = []
            page.on('console',lambda message: messages.append(message.text))
            page.on('pageerror',lambda error: messages.append(str(error)))
            def route(request_route):
                request_url = request_route.request.url
                if not request_url.startswith(url) and not request_url.startswith(('blob:','data:')):
                    external.append(request_url); request_route.abort(); return
                if deny_decoder and '/fonts/decoder/decompress.js' in request_url:
                    blocked.append(request_url); request_route.abort(); return
                if deny_worker_csp and request_route.request.is_navigation_request():
                    response = request_route.fetch()
                    request_route.fulfill(response=response,headers=response.headers | {'content-security-policy': "worker-src 'none'"})
                    return
                request_route.continue_()
            page.route('**/*',route)
            page.goto(url + '?dorotiRenderer=worker-direct-webgl')
            try:
                page.wait_for_function('["started","failed"].includes(document.documentElement.dataset.dorotiBootstrapStage)',timeout=90000)
                stage = page.evaluate('document.documentElement.dataset.dorotiBootstrapStage')
                assert not external, external
                if not deny_decoder and not deny_worker_csp:
                    deadline = time.monotonic() + 60
                    while not any('DOROTI_FONT_PROBE:' in message for message in messages) and time.monotonic() < deadline:
                        page.wait_for_timeout(100)
                reports = [json.loads(message.split('DOROTI_FONT_PROBE:',1)[1]) for message in messages if 'DOROTI_FONT_PROBE:' in message]
                if deny_worker_csp:
                    assert stage == 'failed' and any('worker' in message.lower() and ('security' in message.lower() or 'content security' in message.lower() or 'violates' in message.lower()) for message in messages), (stage,messages)
                elif deny_decoder:
                    assert blocked and any('decoding failed' in message.lower() for message in messages), (stage,blocked,messages)
                else:
                    assert stage == 'started' and len(reports) == 1, (stage,messages)
                    assert reports[0]['koreanGlyphs'] and reports[0]['registeredBytes'] > 0, reports
                    page.screenshot(path=str(out / 'offline.png'))
                checks.append(dict(deniedDecoder=deny_decoder,deniedWorkerCsp=deny_worker_csp,stage=stage,external=external,blocked=blocked,reports=reports,messages=messages))
                (out / 'checks.json').write_text(json.dumps(checks,ensure_ascii=False,indent=2),encoding='utf-8')
            except BaseException:
                (out / 'failure.json').write_text(json.dumps(dict(external=external,blocked=blocked,messages=messages,dataset=page.evaluate('({...document.documentElement.dataset})')),ensure_ascii=False,indent=2),encoding='utf-8')
                raise
            finally: page.close()
        (out / 'result.json').write_text(json.dumps(dict(status='PASS',browser=browser.version,checks=checks,csp='blocked worker diagnostic verified; general deployment notVerified',visibleKoreanPixels='notVerified'),ensure_ascii=False,indent=2),encoding='utf-8')
        browser.close()
        print('PASS: offline startup + actual Skia Korean glyphs, zero external requests; denied decoder produces explicit font diagnostic.')
finally:
    if server.poll() is None:
        subprocess.run(['taskkill','/PID',str(server.pid),'/T','/F'],stdout=subprocess.DEVNULL,stderr=subprocess.DEVNULL) if os.name == 'nt' else server.terminate()
    server.wait(timeout=10); log.close()
