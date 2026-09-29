"""Launch a package-only Apple app and require native frame completion evidence.

Use run-with-timeout.py --timeout 1200. This is local launch qualification,
not clean OS deployment, physical input or a device AOT/signing test.
"""
import argparse
import json
import os
from pathlib import Path
import plistlib
import subprocess
import time

p = argparse.ArgumentParser(description=__doc__)
p.add_argument('--app', type=Path, required=True)
p.add_argument('--target', choices=['ios', 'maccatalyst'], required=True)
p.add_argument('--renderer', choices=['graphite', 'ganesh'], default='graphite')
p.add_argument('--output', type=Path, required=True)
p.add_argument('--simulator', help='Simulator UUID; defaults to a booted or latest available iPhone.')
a = p.parse_args()
root = Path(__file__).resolve().parents[2]
out = a.output.resolve()
if not out.is_relative_to(root / 'temp/testing'): p.error('Output must be under temp/testing.')
if out.exists() and any(out.iterdir()): p.error('Use an empty output directory to avoid stale evidence.')
out.mkdir(parents=True, exist_ok=True)
app = a.app.resolve()
ios = a.target == 'ios'
if ios and a.simulator is None:
    inventory = json.loads(subprocess.check_output(['xcrun', 'simctl', 'list', 'devices', 'available', '--json'], text=True))
    phones = [device for runtime, devices in inventory['devices'].items() if '.iOS-' in runtime
              for device in devices if device.get('isAvailable') and 'iPhone' in device['name']]
    if not phones: p.error('No available iPhone simulator. Supply --simulator after installing an iOS runtime.')
    a.simulator = next((device['udid'] for device in phones if device['state'] == 'Booted'), phones[-1]['udid'])
plist = plistlib.loads((app / ('Info.plist' if ios else 'Contents/Info.plist')).read_bytes())
bundle = plist['CFBundleIdentifier']
process = None
if ios:
    subprocess.run(['xcrun', 'simctl', 'bootstatus', a.simulator, '-b'], check=True, timeout=120)
    subprocess.run(['xcrun', 'simctl', 'install', a.simulator, str(app)], check=True, timeout=120)
    container = Path(subprocess.check_output(['xcrun', 'simctl', 'get_app_container', a.simulator, bundle, 'data'], text=True).strip())
    evidence = container / 'Documents/doroti-package-smoke.json'
else:
    evidence = out / 'evidence.json'
evidence.unlink(missing_ok=True)
Path(str(evidence) + '.exception.txt').unlink(missing_ok=True)
log = (out / 'app.log').open('w')
try:
    if ios:
        subprocess.run(['xcrun', 'simctl', 'launch', '--terminate-running-process', a.simulator, bundle],
            env=os.environ | {'SIMCTL_CHILD_DOROTI_MAUI_EVIDENCE': str(evidence), 'SIMCTL_CHILD_DOROTI_IOS_GRAPHITE': '1' if a.renderer == 'graphite' else '0'},
            stdout=log, stderr=subprocess.STDOUT, check=True, timeout=30)
    else:
        process = subprocess.Popen([str(app / 'Contents/MacOS' / plist['CFBundleExecutable'])],
            env=os.environ | {'DOROTI_MAUI_EVIDENCE': str(evidence), 'DOROTI_IOS_GRAPHITE': '1' if a.renderer == 'graphite' else '0'}, stdout=log, stderr=subprocess.STDOUT)
    deadline = time.monotonic() + 90
    while True:
        error = Path(str(evidence) + '.exception.txt')
        if error.exists(): raise RuntimeError(error.read_text())
        if evidence.exists():
            try:
                data = json.loads(evidence.read_text())
                if data['frame']['failed'] != 0: raise RuntimeError('Native frame failed.')
                if data['frame']['presented'] > 0: break
            except json.JSONDecodeError: pass
        if process is not None and process.poll() is not None: raise RuntimeError('Early native process exit.')
        if time.monotonic() >= deadline: raise TimeoutError('No native presentation receipt.')
        time.sleep(.1)
    summary = {'target': a.target, 'bundle': bundle, 'frameCompletions': data['frame']['presented'],
        'backend': data['frame']['backend'], 'softwareFallbackFrames': data['softwareFallbackFrames'],
        'cleanOS': 'notVerified', 'physicalInput': 'notVerified'}
    (out / 'summary.json').write_text(json.dumps(summary, indent=2))
    if ios:
        subprocess.run(['xcrun', 'simctl', 'io', a.simulator, 'screenshot', str(out / 'screen.png')], check=True, timeout=20)
    print('PASS: package-only Apple native frame completion.', flush=True)
finally:
    if ios:
        subprocess.run(['xcrun', 'simctl', 'terminate', a.simulator, bundle], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL, timeout=20)
        if evidence.exists():
            (out / 'evidence.json').write_bytes(evidence.read_bytes())
            evidence.unlink()
    elif process is not None and process.poll() is None:
        process.terminate()
        process.wait(timeout=20)
    log.close()
