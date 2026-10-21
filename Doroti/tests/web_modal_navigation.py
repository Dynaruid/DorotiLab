"""Chrome + actual managed Testbed: modal Back/Escape and Router Back/Forward.

Requires Python playwright and installed Chrome. Starts/stops its own built Debug
Web host. Run with eng/run-with-timeout.py --timeout 1200.
"""
import argparse
import json
import os
from pathlib import Path
import socket
import subprocess
import time
import urllib.request
from playwright.sync_api import sync_playwright

parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--output', required=True, type=Path)
parser.add_argument('--renderer', choices=['webgl', 'webgpu'], default='webgl')
args = parser.parse_args()
root = Path(__file__).resolve().parents[2]
out = args.output.resolve()
if not out.is_relative_to(root / 'temp/testing'):
    raise ValueError('Evidence must be under temp/testing.')
out.mkdir(parents=True, exist_ok=True)
with socket.socket() as sock:
    sock.bind(('127.0.0.1', 0))
    port = sock.getsockname()[1]
base = f'http://127.0.0.1:{port}/'
errors = []
checks = []
with (out / 'server.log').open('w', encoding='utf-8') as log:
    server = subprocess.Popen(['dotnet', 'run', '--project', 'samples/DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj',
        '-c', 'Debug', '--no-build', '--no-launch-profile'], cwd=root,
        env={**os.environ, 'ASPNETCORE_URLS': base.rstrip('/')}, stdout=log, stderr=subprocess.STDOUT)
    try:
        deadline = time.monotonic() + 60
        while True:
            if server.poll() is not None:
                raise RuntimeError(f'Web server exited: {server.returncode}')
            try:
                urllib.request.urlopen(base, timeout=2).close()
                break
            except OSError:
                if time.monotonic() >= deadline:
                    raise TimeoutError('Web server did not start.')
                time.sleep(.2)
        with sync_playwright() as p:
            browser = p.chromium.launch(channel='chrome', headless=True, args=['--enable-unsafe-webgpu'])
            page = browser.new_page(viewport={'width': 1200, 'height': 1000})
            page.on('pageerror', lambda error: errors.append(str(error)))
            page.on('console', lambda msg: errors.append(msg.text) if msg.type == 'error' else None)

            def ready(sample=''):
                page.goto(base + f'?dorotiSample={sample}&dorotiRenderer=worker-direct-{args.renderer}', wait_until='networkidle')
                page.wait_for_selector('#doroti-semantics [data-doroti-semantics-id]', state='attached', timeout=90000)
                assert page.evaluate('document.documentElement.dataset.dorotiRenderer') == 'worker-direct-' + args.renderer
                page.evaluate('globalThis.__dorotiModalDocument = crypto.randomUUID()')

            def labels():
                return page.locator('[aria-label]').evaluate_all('(es)=>es.map(e=>e.getAttribute("aria-label"))')

            def wait_label(text, present=True):
                try:
                    page.wait_for_function('([text,present])=>Array.from(document.querySelectorAll("[aria-label]")).some(e=>e.getAttribute("aria-label")===text)===present', arg=[text, present])
                except Exception:
                    page.screenshot(path=str(out / 'failure.png'))
                    (out / 'failure.json').write_text(json.dumps({'expected':text, 'present':present,
                        'labels':labels(), 'errors':errors, 'url':page.url,
                        'state':page.evaluate('({dataset:{...document.documentElement.dataset},history:history.state})')}, indent=2), encoding='utf-8')
                    raise

            def click(text, scroll=False):
                for attempt in range(30 if scroll else 1):
                    button = page.get_by_role('button', name=text, exact=True)
                    delta = 200
                    if button.count():
                        box = button.first.bounding_box()
                        if scroll and box and not 200 <= box['y'] <= 800:
                            delta = -200 if box['y'] < 200 else 200
                        else:
                            # Use the production accessibility action to open routes.
                            # This regression qualifies navigation, not canvas hit
                            # coordinates. Keep its node away from viewport clips.
                            button.first.evaluate('(element)=>element.click()')
                            return
                    page.mouse.move(400, 800)
                    page.mouse.wheel(0, delta)
                    page.wait_for_timeout(250)
                raise AssertionError((text, labels()))

            previous = base + 'previous-navigation-probe'
            page.route(previous, lambda route: route.fulfill(body='<h1>Previous document</h1>', content_type='text/html'))
            page.goto(previous)
            ready()
            document = page.evaluate('globalThis.__dorotiModalDocument')
            for button, marker in [('Show modal bottom sheet', 'Favorite'), ('Show dialog', 'What is a dialog?'),
                                   ('Show full-screen dialog', 'Full-screen dialog')]:
                print('Checking ' + button, flush=True)
                click(button, scroll=True)
                wait_label(marker)
                page.evaluate('history.back()')
                wait_label(marker, False)
                page.wait_for_timeout(300)
                assert page.evaluate('globalThis.__dorotiModalDocument') == document
                assert page.evaluate('history.state.dorotiHistory.kind') == 'entry'
                checks.append(button + ': browser Back closes modal, same document')
                print('PASS: ' + checks[-1], flush=True)

            # Move back to the dialog section after the full-screen overlay closes.
            click('Show dialog', scroll=True)
            wait_label('What is a dialog?')
            # Focus the production DOM text endpoint and simulate composition. This
            # isolates key ownership from app-specific text-field layout/IME setup.
            page.evaluate('''() => {
                const input = document.getElementById('doroti-ime');
                input.hidden = false; input.readOnly = false; input.focus();
                input.dispatchEvent(new CompositionEvent('compositionstart', {bubbles:true}));
            }''')
            page.keyboard.press('Escape')
            page.wait_for_timeout(500)
            assert 'What is a dialog?' in labels(), 'Composing Escape reached modal dismissal.'
            page.evaluate("document.getElementById('doroti-ime').dispatchEvent(new CompositionEvent('compositionend', {bubbles:true}))")
            page.keyboard.press('Escape')
            wait_label('What is a dialog?', False)
            assert page.evaluate('globalThis.__dorotiModalDocument') == document
            checks.append('focused DOM endpoint: composing Escape retained; non-composing Escape closes modal')
            page.screenshot(path=str(out / 'modals-closed.png'))
            page.evaluate('history.back()')
            page.wait_for_url(previous)
            assert page.get_by_role('heading', name='Previous document').is_visible()
            checks.append('root browser Back exits to previous document')

            ready('navigation')
            click('First page'); page.wait_for_url('**#/first')
            click('Second page'); page.wait_for_url('**#/second')
            page.go_back(); page.wait_for_url('**#/first'); wait_label('Route: ' + page.url.replace(base.rstrip('/'), ''))
            page.go_forward(); page.wait_for_url('**#/second'); wait_label('Route: ' + page.url.replace(base.rstrip('/'), ''))
            assert 'dorotiHistory' not in page.evaluate('history.state')
            checks.append('Router Back/Forward retains URI/state history without Navigator guard')
            assert not errors, errors
            (out / 'receipt.json').write_text(json.dumps({'result':'PASS', 'renderer':args.renderer, 'checks':checks,
                'errors':errors, 'scope':'actual Chrome + managed Worker; DOM semantics activation; automated keys/history; simulated DOM composition'}, indent=2), encoding='utf-8')
            print('PASS: ' + '; '.join(checks), flush=True)
            browser.close()
    finally:
        subprocess.run(['taskkill', '/PID', str(server.pid), '/T', '/F'], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
        server.wait(timeout=10)
