#!/usr/bin/env python3
"""Observe Qt Quick resize/window-state transitions with the real product."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import subprocess

parser = argparse.ArgumentParser()
parser.add_argument('--app', type=Path, required=True)
parser.add_argument('--driver', type=Path, required=True)
parser.add_argument('--output', type=Path, required=True)
parser.add_argument('--qpa', choices=('wayland', 'xcb'), required=True)
args = parser.parse_args()
out = args.output.resolve()
out.mkdir(parents=True, exist_ok=False)
steps = [{'action': 'wait', 'wait': 1000}]
for name, action, values in (
    ('initial', 'capture', {}), ('resized', 'resize', {'width': 810, 'height': 690}),
    ('resized', 'capture', {}), ('back-size', 'resize', {'width': 720, 'height': 640}),
    ('back-size', 'capture', {}), ('maximized', 'maximize', {}),
    ('maximized', 'capture', {}), ('restored', 'restore', {}),
    ('restored', 'capture', {}), ('minimized', 'minimize', {}),
    ('minimized', 'capture', {}), ('reactivated', 'restore', {}),
    ('reactivated', 'capture', {}), ('hidden', 'hide', {}),
    ('hidden', 'capture', {}), ('shown', 'show', {}),
    ('shown', 'capture', {}), ('before-close', 'resize', {'width': 760, 'height': 670}),
    ('before-close', 'capture', {}),
):
    step = {'action': action, **values}
    if action == 'capture':
        step['path'] = str(out / name)
    steps.append(step)
(out / 'script.json').write_text(json.dumps(steps, indent=2) + '\n')
env = dict(os.environ, QT_QPA_PLATFORM=args.qpa,
           DOROTI_TESTBED_MODE='platform-effects', DOROTI_QT_DIAGNOSTICS='1',
           DOROTI_QT_TEST_SCRIPT=str(out / 'script.json'),
           LD_PRELOAD=str(args.driver.resolve()))
with (out / 'product.log').open('w') as log:
    try:
        result = subprocess.run(['dotnet', str(args.app.resolve())], env=env,
                                stdout=log, stderr=subprocess.STDOUT, timeout=180)
        exit_code = result.returncode
    except subprocess.TimeoutExpired:
        exit_code = 124
captures = {}
for name in ('initial', 'resized', 'back-size', 'maximized', 'restored',
             'minimized', 'reactivated', 'hidden', 'shown', 'before-close'):
    path = out / (name + '.json')
    if path.exists():
        record = json.loads(path.read_text())
        captures[name] = {key: record.get(key) for key in
                          ('qpa', 'windowWidth', 'windowHeight', 'windowDpr',
                           'windowVisible', 'windowState', 'windowCapture',
                           'validationLayerLoaded', 'rasterItems', 'effectItems')}
        captures[name]['nativeIds'] = [control['id'] for control in record.get('controls', [])]
log = (out / 'product.log').read_text(errors='replace')
checks = {
    'exit': exit_code == 0,
    'complete': len(captures) == 10,
    'resize': captures.get('resized', {}).get('windowWidth') == 810
              and captures.get('resized', {}).get('windowHeight') == 690,
    'restoreSize': captures.get('back-size', {}).get('windowWidth') == 720
                   and captures.get('back-size', {}).get('windowHeight') == 640,
    'hideShow': captures.get('hidden', {}).get('windowVisible') is False
                and captures.get('shown', {}).get('windowVisible') is True,
    'noVulkanValidationError': 'VUID-' not in log and 'Validation Error' not in log,
    'noManagedFatal': 'managed.fatal=' not in log and 'Unhandled exception' not in log,
}
if 'VK_LAYER_KHRONOS_validation' in env.get('VK_INSTANCE_LAYERS', ''):
    checks['validationLayerLoaded'] = captures.get('initial', {}).get('validationLayerLoaded') is True
# Compositors can refuse minimize/maximize. Preserve their observed state.
checks['maximizeAccepted'] = captures.get('maximized', {}).get('windowState') == 2
checks['minimizeAccepted'] = captures.get('minimized', {}).get('windowState') == 1
report = {'qpa': args.qpa, 'exit': exit_code,
          'appSha256': hashlib.sha256(args.app.read_bytes()).hexdigest(),
          'driverSha256': hashlib.sha256(args.driver.read_bytes()).hexdigest(),
          'checks': checks, 'captures': captures,
          'status': 'passed' if all(checks.values()) else 'partial' if all(
              checks[key] for key in checks if key not in ('maximizeAccepted', 'minimizeAccepted')) else 'failed'}
(out / 'result.json').write_text(json.dumps(report, indent=2) + '\n')
print(json.dumps({'status': report['status'], 'checks': checks}, indent=2))
raise SystemExit(0 if report['status'] != 'failed' else 1)
