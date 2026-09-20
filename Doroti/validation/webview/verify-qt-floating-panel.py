"""Exercise the actual Material WebView tab: drag/shield/toggle/resize without recreating its browser."""
import argparse
import json
import os
from pathlib import Path
import subprocess

p = argparse.ArgumentParser()
p.add_argument('--app', type=Path, required=True)
p.add_argument('--driver', type=Path, required=True)
p.add_argument('--output', type=Path, required=True)
p.add_argument('--qpa', choices=['wayland', 'xcb'], required=True)
a = p.parse_args()
out = a.output.resolve()
out.mkdir(parents=True, exist_ok=False)
steps = []
def step(action, **values): steps.append(dict(action=action, **values))
def capture(name): step('capture', path=str(out/name))
def js(name, code): step('js', path=str(out/name), code=code)
step('wait', wait=2000)
capture('home')
step('click', label='WebView\nTab 6 of 6', wait=4000)
capture('youtube')
js('youtube', 'return location.href')
# Local sample makes input/state assertions independent of YouTube network/media.
# Icon-only toolbar controls currently have no accessible name in this host.
step('click', x=232, y=124, wait=1500)
js('setup', "document.querySelector('input').value='retained panel state';window.nativeClicks=0;document.addEventListener('click',()=>nativeClicks++);return true")
capture('initial')
step('drag', label='Floating panel', dx=180, dy=120)
capture('moved')
js('shield', 'return nativeClicks')
step('click', label='Hide panel')
capture('hidden')
step('click', label='Show panel')
capture('shown')
step('click', x=500, y=400, nativePoint=True)
js('outside', 'return nativeClicks')
step('resize', width=980, height=620, wait=1500)
capture('resized')
js('retained', "return document.querySelector('input').value")
script = out/'script.json'
script.write_text(json.dumps(steps, indent=2))
env = dict(os.environ, QT_QPA_PLATFORM=a.qpa, DOROTI_TESTBED_MODE='sample',
           DOROTI_QT_TEST_SCRIPT=str(script), LD_PRELOAD=str(a.driver.resolve()))
checks = {}
report = dict(qpa=a.qpa, input='Qt injected events through product ingress', checks=checks)
try:
    with (out/'product.log').open('w') as log:
        run = subprocess.run(['dotnet', str(a.app.resolve())], env=env, stdout=log, stderr=log, timeout=180)
    report['exit'] = run.returncode
    def read(name): return json.loads((out/(name+'.json')).read_text())
    start, moved, hidden, shown, resized = [read(n) for n in ('initial','moved','hidden','shown','resized')]
    checks['exit'] = run.returncode == 0
    checks['youtubeConfigured'] = 'youtube.com/watch?v=hI9HQfCAw64' in read('youtube1')
    checks['initialBackdrop'] = start['effectItems'] == 1 and len(start['effectRects']) == 1
    x, y, w, h = start['effectRects'][0]
    mx, my, _, _ = moved['effectRects'][0]
    checks['dragMovesPanel'] = mx-x > 100 and my-y > 60
    checks['dragShield'] = read('shield1') == 0
    checks['hideRemovesEffect'] = hidden['effectItems'] == 0
    checks['showPreservesPosition'] = shown['effectRects'] == moved['effectRects']
    checks['outsidePageInteractive'] = read('outside1') == 1
    identity = lambda value: [c['id'] for c in value['controls']]
    checks['nativeIdentityPreserved'] = all(identity(value) == identity(start) for value in (moved,hidden,shown,resized))
    checks['documentStatePreserved'] = read('retained1') == 'retained panel state'
    bx, by, bw, bh = resized['effectRects'][0]
    web = resized['controls'][0]
    checks['resizeKeepsPanelInside'] = (bx >= web['x'] and by >= web['y'] and
        bx+bw <= web['x']+web['width']+.1 and by+bh <= web['y']+web['height']+.1)
    log = (out/'product.log').read_text()
    checks['noLayoutOrNativeFailure'] = not any(s in log for s in ('overflowed by', 'managed.fatal=', 'Unhandled exception', 'VUID-', 'PlatformView operation rejected'))
    report['status'] = 'PASS' if all(checks.values()) else 'FAIL'
except Exception as error:
    report.update(status='FAIL', error=str(error))
(out/'result.json').write_text(json.dumps(report, indent=2)+'\n')
print(json.dumps(report, indent=2))
raise SystemExit(0 if report['status']=='PASS' else 1)
