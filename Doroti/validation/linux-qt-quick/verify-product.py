"""Real Testbed Qt event-ingress checks; run under run-with-timeout.py.

The preload driver is validation-only. Images come from QQuickWindow::grabWindow
and include native WebEngine/Quick items; readback is NOT a product transport.
"""
import argparse
import hashlib
import json
import os
from pathlib import Path
import subprocess
import sys
from PIL import Image

p = argparse.ArgumentParser()
p.add_argument('--app', required=True, type=Path)
p.add_argument('--driver', required=True, type=Path)
p.add_argument('--output', required=True, type=Path)
p.add_argument('--qpa', choices=['xcb', 'wayland'], required=True)
args = p.parse_args()
out = args.output.resolve()
out.mkdir(parents=True, exist_ok=True)
steps = []
def step(action, **values):
    steps.append(dict(action=action, **values))
def capture(name):
    step('capture', path=str(out / name))
def click(x, y):
    step('click', x=x, y=y)
def probe(name, code=None):
    step('js', path=str(out / name), code=code or '''return ({
        clicks: Number(document.querySelector('button').textContent),
        text: document.querySelector('input').value, scroll: scrollY,
        animation: document.querySelector('#moving').getBoundingClientRect().x
    })''')

step('wait', wait=2000)
capture('live-a'); probe('live-a')
step('wait', wait=700)
capture('live-b'); probe('live-b')
click(150, 165); step('text', text='Qt retained edit')
click(445, 228); probe('pass')
click(330, 104); click(445, 228); probe('blocked')
click(360, 435); capture('foreground')
click(330, 104)
probe('paused', "document.getAnimations().forEach(a => { a.pause(); a.currentTime=1500; }); return ({paused:true})")
capture('blur-on')
click(220, 104); capture('blur-off'); probe('off')
click(220, 104)
step('wheel', x=580, y=360, delta=-120); probe('scroll')
step('wheel', x=580, y=360, delta=120)
click(230, 588); capture('two-native'); probe('two')
click(345, 588); capture('moved'); probe('moved')
click(460, 588); capture('disposed')
click(460, 588); step('wait', wait=1500); capture('recreated'); probe('recreated')

script = out / 'script.json'
script.write_text(json.dumps(steps, indent=2))
env = dict(os.environ, QT_QPA_PLATFORM=args.qpa,
           DOROTI_TESTBED_MODE='platform-effects',
           DOROTI_PLATFORM_VIEW_EVIDENCE=str(out),
           DOROTI_QT_TEST_SCRIPT=str(script), LD_PRELOAD=str(args.driver.resolve()),
           QSG_INFO='1', QT_LOGGING_RULES='qt.webenginecontext=true')
env.pop('DOROTI_QT_VALIDATION_RESIZE_CYCLES', None)
command = ['dotnet', str(args.app.resolve())]
with (out / 'product.log').open('w') as log:
    run = subprocess.run(command, env=env, stdout=log, stderr=subprocess.STDOUT, timeout=180)
checks = {}
def read(name):
    return json.loads((out / (name + '.json')).read_text())
try:
    a, b = read('live-a'), read('live-b')
    checks['exit'] = run.returncode == 0
    checks['liveNativeAnimation'] = abs(read('live-a1')['animation'] - read('live-b1')['animation']) > 1
    checks['liveCaptureChanged'] = (out/'live-a.png').read_bytes() != (out/'live-b.png').read_bytes()
    checks['nativeClickOnce'] = read('pass1')['clicks'] == 1
    checks['committedShield'] = read('blocked1')['clicks'] == 1
    checks['editingPreserved'] = read('off1')['text'] == 'Qt retained edit'
    checks['nativeWheel'] = read('scroll1')['scroll'] > 0
    identity = lambda name: [c['id'] for c in read(name)['controls']]
    checks['effectToggleIdentity'] = identity('blur-on') == identity('blur-off') == identity('live-a')
    checks['noEffectResourcesWhenDisabled'] = read('blur-off')['effectItems'] == 0 and not read('blur-off')['effectSourceGroup']
    checks['twoNativeAndEffect'] = len(identity('two-native')) == 2 and read('two-native')['effectItems'] == 1
    checks['movePreservesIdentity'] = identity('two-native') == identity('moved')
    checks['disposeRemovesNativeAndEffectHost'] = not identity('disposed') and read('disposed')['effectItems'] == 0
    checks['newGeneration'] = len(identity('recreated')) == 2 and not set(identity('recreated')) & set(identity('moved'))
    checks['captures'] = all(read(n)['windowCapture'] for n in ['live-a','live-b','two-native','moved','disposed','recreated'])
    checks['foregroundButton'] = any('Foreground taps: 1' in name for name in read('foreground')['accessibleNames'])
    def gradient(name):
        image = Image.open(out / (name + '.png')).convert('L')
        scale = image.width / 720
        crop = image.crop(tuple(round(v * scale) for v in (220, 330, 450, 370)))
        values = list(crop.getdata())
        return sum(abs(values[y*crop.width+x] - values[y*crop.width+x-1])
                   for y in range(crop.height) for x in range(1, crop.width)) / (crop.height*(crop.width-1))
    on_gradient, off_gradient = gradient('blur-on'), gradient('blur-off')
    checks['nativePixelsBlurred'] = off_gradient > 1 and on_gradient < .65 * off_gradient
    if 'VK_LAYER_KHRONOS_validation' in env.get('VK_INSTANCE_LAYERS', ''):
        checks['validationLayerLoaded'] = read('live-a')['validationLayerLoaded']
except (OSError, ValueError, KeyError) as error:
    checks['completeEvidence'] = False
    checks['evidenceError'] = str(error)
log = (out / 'product.log').read_text()
checks['noVulkanValidationError'] = 'VUID-' not in log and 'Validation Error' not in log
checks['noManagedFatal'] = 'managed.fatal=' not in log and 'Unhandled exception' not in log
report = dict(qpa=args.qpa, command=command, exit=run.returncode, timeout=False,
              input='automated Qt events through product ingress; not physical',
              appSha256=hashlib.sha256(args.app.read_bytes()).hexdigest(),
              driverSha256=hashlib.sha256(args.driver.read_bytes()).hexdigest(), checks=checks,
              validationLayers=env.get('VK_INSTANCE_LAYERS', 'notEnabled'),
              productBinaries={f.name: hashlib.sha256(f.read_bytes()).hexdigest()
                               for f in args.app.parent.iterdir() if f.suffix in ('.dll', '.so')},
              physical='notVerified', performance='notQualified', visualSimilarity='notQualified')
if 'on_gradient' in locals():
    report['nativeBlurGradient'] = dict(on=on_gradient, off=off_gradient,
                                      criterion='on < 0.65 * off in fixed native checker/text ROI; not cross-platform visual qualification')
(out/'result.json').write_text(json.dumps(report, indent=2))
print(json.dumps(report, indent=2))
sys.exit(0 if all(value is True for value in checks.values()) else 1)
