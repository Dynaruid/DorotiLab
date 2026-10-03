"""UIKit native smoke. Run with run-with-timeout.py --timeout 1200.

Uses only the testbed's probes; physical input, VoiceOver and OS drag delivery
are separate qualification gates. Simulator and Catalyst results stay separate.
"""
import argparse
import json
import hashlib
import base64
import os
from pathlib import Path
import subprocess
import time
import uuid

ROOT = Path(__file__).resolve().parents[2]
p = argparse.ArgumentParser(description=__doc__)
p.add_argument('--target', choices=['ios', 'maccatalyst'], required=True)
p.add_argument('--output', type=Path, required=True)
p.add_argument('--simulator', help='Simulator UUID; defaults to a booted or latest available iPhone.')
p.add_argument('--cases', default='services,input,features,navigation,restoration,multi')
p.add_argument('--activation', choices=['os', 'native-callback'], default='os')
p.add_argument('--renderer', choices=['graphite', 'ganesh'], default='graphite')
p.add_argument('--skip-build', action='store_true')
p.add_argument('--app', type=Path, help='Already-built app bundle, including an isolated ArtifactsPath build.')
a = p.parse_args()
out = a.output.resolve()
if not out.is_relative_to(ROOT / 'temp/testing'): p.error('Output must be under temp/testing.')
if out.exists() and any(out.iterdir()): p.error('Use an empty output directory to avoid stale evidence.')
out.mkdir(parents=True, exist_ok=True)
ios = a.target == 'ios'
if ios and a.simulator is None:
    inventory = json.loads(subprocess.check_output(['xcrun', 'simctl', 'list', 'devices', 'available', '--json'], text=True))
    phones = [device for runtime, devices in inventory['devices'].items() if '.iOS-' in runtime
              for device in devices if device.get('isAvailable') and 'iPhone' in device['name']]
    if not phones: p.error('No available iPhone simulator. Supply --simulator after installing an iOS runtime.')
    a.simulator = next((device['udid'] for device in phones if device['state'] == 'Booted'), phones[-1]['udid'])
platform = 'ios' if ios else 'macos'
name = 'iOS' if ios else 'MacCatalyst'
rid = 'iossimulator-arm64' if ios else 'maccatalyst-arm64'
tfm = 'net10.0-ios27.0' if ios else 'net10.0-maccatalyst27.0'
project = ROOT / f'samples/DorotiTestbedApp/{platform}/DorotiTestbedApp.{name}.csproj'
if not a.skip_build and a.app is None:
    with (out / 'build.log').open('w') as log:
        subprocess.run(['dotnet', 'build', str(project), '-c', 'Debug', '-r', rid,
            '-p:' + ('DorotiIosTargetFramework' if ios else 'DorotiMacCatalystTargetFramework') + '=' + tfm],
            cwd=ROOT, stdout=log, stderr=subprocess.STDOUT, check=True, timeout=1200)
apps = [a.app.resolve()] if a.app else list((project.parent / ('bin/' + rid if ios else 'bin') / 'Debug' / tfm / rid).glob('*.app'))
if len(apps) != 1: raise RuntimeError(f'Expected one app: {apps}')
app = apps[0]
if not app.is_dir() or app.suffix != '.app': p.error('Supply an existing .app bundle.')
runid = 'apple-smoke-' + uuid.uuid4().hex
results = {'restorationId': runid, 'target': a.target, 'tfm': tfm, 'rid': rid, 'renderer': a.renderer, 'activation': a.activation, 'physicalInput': 'notVerified', 'checks': {}}
(out / 'run.json').write_text(json.dumps(results, indent=2))
base = os.environ | {'DOROTI_IOS_GRAPHITE': '1' if a.renderer == 'graphite' else '0', 'DOROTI_SAMPLE': 'reload', 'DOROTI_RESTORATION_ID': runid}
bundle = 'dev.doroti.testbed'
if ios:
    subprocess.run(['xcrun', 'simctl', 'bootstatus', a.simulator, '-b'], check=True, timeout=120)
    subprocess.run(['xcrun', 'simctl', 'install', a.simulator, str(app)], check=True, timeout=120)
    container = Path(subprocess.check_output(['xcrun', 'simctl', 'get_app_container', a.simulator, bundle, 'data'], text=True).strip())
    native_output = container / 'Documents' / runid
    native_output.mkdir(parents=True)
else:
    native_output = out
checkpoint = (container / 'Library' if ios else Path.home() / 'Library') / 'restoration' / (hashlib.sha256(runid.encode()).hexdigest().upper() + '.json')


def stop(process):
    if ios:
        subprocess.run(['xcrun', 'simctl', 'terminate', a.simulator, bundle], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL, timeout=20)
    elif process is not None and process.poll() is None:
        process.terminate()
        process.wait(timeout=20)


def wait(path, process, content=None):
    deadline = time.monotonic() + 100
    while time.monotonic() < deadline:
        errors = list(path.parent.glob('*.error')) + list(path.parent.glob('*.exception.txt'))
        if errors: raise RuntimeError('\n'.join(error.read_text() for error in errors))
        if path.exists() and (content is None or content in path.read_text()): return path.read_text()
        if not ios and process.poll() is not None: raise RuntimeError(f'Exited {process.returncode} before {path}')
        time.sleep(.1)
    raise TimeoutError(str(path))


for case in a.cases.split(','):
    if ios and case == 'multi': continue
    if not ios and case == 'rotation': raise ValueError('Rotation probe requires iOS.')
    directory = native_output / case
    directory.mkdir(exist_ok=True)
    marker = directory / 'result.json'
    extra = {'DOROTI_MAUI_EVIDENCE': str(directory / 'evidence.json')}
    if case == 'multi': extra |= {'DOROTI_MULTIWINDOW_PROBE': str(marker), 'DOROTI_SAMPLE': 'input'}
    elif case == 'services': extra['DOROTI_UIKIT_SERVICES_PROBE'] = str(marker)
    elif case == 'features': extra['DOROTI_APPLE_FEATURE_PROBE'] = str(marker)
    elif case == 'input': extra |= {'DOROTI_SAMPLE': 'input', 'DOROTI_INPUT_PROBE': str(marker)}
    elif case == 'rotation': extra['DOROTI_UIKIT_ROTATION_PROBE'] = str(marker)
    elif case in ('navigation', 'restoration'): extra |= {'DOROTI_SAMPLE': 'navigation', 'DOROTI_NAVIGATION_PROBE': str(marker)}
    else: raise ValueError(case)
    if case == 'navigation' and a.activation == 'native-callback':
        extra['DOROTI_UIKIT_ACTIVATION_PROBE'] = 'https://doroti.example/details/apple-smoke'
    log = (out / (case + '.log')).open('w')
    process = None
    try:
        if ios:
            environment = os.environ | {'SIMCTL_CHILD_' + k: v for k, v in (base | extra).items() if k.startswith('DOROTI_')}
            subprocess.run(['xcrun', 'simctl', 'launch', '--terminate-running-process', a.simulator, bundle], env=environment,
                stdout=log, stderr=subprocess.STDOUT, check=True, timeout=120)
            process = None
        else:
            process = subprocess.Popen([str(app / 'Contents/MacOS' / ('DorotiTestbedApp.' + name))],
                env=base | extra, stdout=log, stderr=subprocess.STDOUT)
        wait(marker, process)
        if case == 'navigation':
            link = 'doroti-testbed://app/details/apple-smoke'
            if a.activation == 'os':
                if ios: subprocess.run(['xcrun', 'simctl', 'openurl', a.simulator, link], check=True, timeout=20)
                else: subprocess.run(['open', '-a', str(app), link], check=True, timeout=20)
            wait(marker, process, 'apple-smoke')
            # A widget build marker precedes the post-frame restoration checkpoint.
            # Force-stop only after the requested route is durably committed.
            deadline = time.monotonic() + 40
            while time.monotonic() < deadline:
                saved = json.loads(checkpoint.read_text()) if checkpoint.exists() else {}
                route = marker.read_text().splitlines()[0]
                payload = base64.b64decode(saved.get('data') or '')
                if saved.get('location') == route and route.encode() in payload:
                    (directory / 'checkpoint.json').write_text(json.dumps(saved))
                    break
                time.sleep(.1)
            else: raise TimeoutError('Route/restoration checkpoint did not commit: ' + str(saved))
        if case == 'restoration': wait(marker, process, 'apple-smoke')
        # The multiwindow probe closes all windows before its result marker.
        # Its shared evidence path can contain a pre-close snapshot from another
        # view; it cannot establish live GPU progress after the views detach.
        if a.renderer == 'graphite' and case != 'multi':
            evidence = directory / 'evidence.json'
            wait(evidence, process)
            deadline = time.monotonic() + 30
            while time.monotonic() < deadline:
                try:
                    pipeline = json.loads(evidence.read_text())['surface']['nativeFramePipeline']
                    if pipeline and pipeline['completedGpuFrames'] > 0:
                        assert pipeline['mode'] == 'C' and 0 < pipeline['maximumGpuFrames'] <= 2, pipeline
                        results.setdefault('framePipelines', {})[case] = pipeline
                        break
                except json.JSONDecodeError:
                    pass
                time.sleep(.1)
            else: raise TimeoutError('Missing completed C-only Metal pipeline evidence: ' + str(evidence))
        results['checks'][case] = marker.read_text()
        if ios:
            subprocess.run(['xcrun', 'simctl', 'io', a.simulator, 'screenshot', str(out / (case + '.png'))], check=True, timeout=20)
    finally:
        stop(process)
        log.close()
        if ios:
            import shutil
            shutil.copytree(directory, out / case, dirs_exist_ok=True)
    print('PASS ' + a.target + ' ' + case, flush=True)
(out / 'summary.json').write_text(json.dumps(results, indent=2))
if ios:
    # Only this execution's unique device-side raw evidence; app/user data remain.
    import shutil
    shutil.rmtree(native_output)
