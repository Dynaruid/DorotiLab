"""Signed iOS device smoke; invoke through run-with-timeout.py --timeout 1200.

Installs without uninstalling or resetting app data. Probes run on the selected
physical device; generated touch/text events do not establish physical IME or
VoiceOver acceptance. Final foreground app remains available for manual use.
"""
import argparse
import hashlib
import json
from pathlib import Path
import plistlib
import subprocess
import time
import uuid

ROOT = Path(__file__).resolve().parents[2]
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--device', required=True)
parser.add_argument('--app', type=Path, required=True)
parser.add_argument('--output', type=Path, required=True)
parser.add_argument('--cases', default='services,input,features,rotation,shutdown,lifecycle')
parser.add_argument('--sample', choices=['reload', 'material'], default='reload',
                    help='Scene used for rotation/lifecycle; material exercises the full Components screen.')
parser.add_argument('--profile-frames', action='store_true', help='Enable the optional detailed native CPU/GPU trace.')
parser.add_argument('--assert-rotation-sync', action='store_true',
                    help='Opt in to the stricter 5%%/10%% viewport phase budget; default records timing only.')
args = parser.parse_args()
out = args.output.resolve()
if not out.is_relative_to(ROOT / 'temp/testing') or out.exists():
    parser.error('Use a fresh output under temp/testing.')
app = args.app.resolve()
if not app.is_dir() or app.suffix != '.app': parser.error('Supply the signed device .app.')
out.mkdir(parents=True)
bundle = plistlib.loads((app / 'Info.plist').read_bytes())['CFBundleIdentifier']
run_id = 'device-smoke-' + uuid.uuid4().hex
summary = {'result': 'running', 'device': args.device, 'bundle': bundle, 'runId': run_id,
           'physicalInput': 'notVerified', 'voiceOver': 'notVerified', 'checks': {}, 'caseFailures': {},
           'rotationPhaseBudget': 'asserted' if args.assert_rotation_sync else 'measuredOnly',
           'rotationFps': 'UIKit system cadence; no FPS acceptance threshold or application cap',
           'sample': args.sample, 'detailedFrameProfiling': args.profile_frames}
base = {'DOROTI_IOS_GRAPHITE': '1', 'DOROTI_SAMPLE': 'reload' if args.sample == 'reload' else '',
        'DOROTI_RESTORATION_ID': run_id}
if args.profile_frames: base['DOROTI_VARIABLE_BLUR_PROFILE'] = '1'
sequence = 0


def save():
    (out / 'summary.json').write_text(json.dumps(summary, indent=2) + '\n')


def run(*arguments, timeout=30, optional=False):
    global sequence
    sequence += 1
    result = subprocess.run(arguments, capture_output=True, text=True, timeout=timeout)
    (out / f'command-{sequence:03}.log').write_text(result.stdout + result.stderr)
    if result.returncode and not optional: raise RuntimeError(result.stdout + result.stderr)
    return result


def device(*arguments, **kwargs):
    return run('xcrun', 'devicectl', 'device', *arguments, '--device', args.device, **kwargs)


def launch(name, environment, terminate=True, target=None):
    path = out / (name + '-launch.json')
    options = ['--terminate-existing'] if terminate else []
    variables = ['--environment-variables', json.dumps(environment)] if environment else []
    # Options precede the positional bundle identifier; keep argv structured.
    run('xcrun', 'devicectl', 'device', 'process', 'launch', '--device', args.device,
        '--json-output', str(path), *options, *variables, target or bundle)
    return json.loads(path.read_text())['result']['process']['processIdentifier']


def copy(remote, local):
    return device('copy', 'from', '--domain-type', 'appDataContainer', '--domain-identifier', bundle,
        '--source', remote, '--destination', str(local), timeout=15, optional=True).returncode == 0


def wait_json(remote, name, timeout=100):
    target = out / (name + '.json')
    error = out / (name + '.error')
    deadline = time.monotonic() + timeout
    while time.monotonic() < deadline:
        if copy(remote + '.error', error): raise RuntimeError(error.read_text())
        if copy(remote + '.exception.txt', error): raise RuntimeError(error.read_text())
        if copy(remote, target):
            try: return json.loads(target.read_text())
            except json.JSONDecodeError: pass
        time.sleep(.4)
    raise TimeoutError('Missing device result: ' + remote)


def frame(remote, name):
    deadline = time.monotonic() + 100
    while time.monotonic() < deadline:
        value = wait_json('Documents/' + remote, name, timeout=30)
        if value['frame']['presented'] > 0:
            assert value['frame']['failed'] == 0, value
            pipeline = value['surface']['nativeFramePipeline']
            assert pipeline['mode'] == 'C' and pipeline['completedGpuFrames'] > 0, pipeline
            assert 0 < pipeline['maximumGpuFrames'] <= 2, pipeline
            assert value['softwareFallbackFrames'] == 0, value
            return value
        time.sleep(.4)
    raise TimeoutError('No actual Metal completion')


def terminate(pid):
    device('process', 'terminate', '--pid', str(pid), timeout=20, optional=True)


save()
try:
    run('codesign', '--verify', '--deep', '--strict', str(app))
    payload = hashlib.sha256()
    for path in sorted(app.rglob('*')):
        if path.is_file():
            payload.update(path.relative_to(app).as_posix().encode())
            payload.update(hashlib.sha256(path.read_bytes()).digest())
    summary['payloadSha256'] = payload.hexdigest()
    device('install', 'app', str(app), timeout=120)
    for case in args.cases.split(','):
        remote = run_id + '/' + case
        evidence = remote + '/evidence.json'
        environment = base | {'DOROTI_MAUI_EVIDENCE': evidence}
        probes = {'services': 'DOROTI_UIKIT_SERVICES_PROBE', 'input': 'DOROTI_INPUT_PROBE',
                  'features': 'DOROTI_APPLE_FEATURE_PROBE', 'rotation': 'DOROTI_UIKIT_ROTATION_PROBE',
                  'shutdown': 'DOROTI_UIKIT_SHUTDOWN_PROBE'}
        if case in probes:
            environment[probes[case]] = remote + '/result.json'
            if case == 'input': environment['DOROTI_SAMPLE'] = 'input'
            if case == 'rotation' and args.assert_rotation_sync:
                environment['DOROTI_UIKIT_ROTATION_ASSERT_SYNC'] = '1'
        elif case != 'lifecycle': raise ValueError('Unknown device case: ' + case)
        pid = launch(case, environment)
        try:
            if case == 'lifecycle':
                initial = frame(evidence, case + '-initial')
                cycles = []
                for cycle in range(1, 4):
                    before = frame(evidence, f'{case}-{cycle}-before')
                    launch(f'{case}-{cycle}-background', {}, terminate=False, target='com.apple.Preferences')
                    time.sleep(2)
                    resumed = launch(f'{case}-{cycle}-foreground', environment, terminate=False)
                    assert resumed == pid, 'Foreground recreated the process'
                    time.sleep(3)
                    after = frame(evidence, f'{case}-{cycle}-after')
                    completed_before = before['surface']['nativeFramePipeline']['completedGpuFrames']
                    completed_after = after['surface']['nativeFramePipeline']['completedGpuFrames']
                    assert completed_after > completed_before, 'Foreground GPU rendering did not recover'
                    cycles.append({'cycle': cycle, 'samePid': True, 'beforeCompletedGpuFrames': completed_before,
                                   'afterCompletedGpuFrames': completed_after, 'replayed': after['frame']['replayed'],
                                   'failed': after['frame']['failed']})
                summary['checks'][case] = {'status': 'PASS', 'cycles': cycles, 'pid': pid,
                    'meaning': 'same process/shared application, new frames after each foreground; static scene, no FPS claim'}
            else:
                value = wait_json('Documents/' + remote + '/result.json', case + '-result')
                if case == 'shutdown':
                    assert value['sharedStopCompletion'] and value['viewDisposed'] and value['gpuRetiredBeforeCompletion'], value
                    assert value['after']['NativeFramePipeline']['PendingGpuFrames'] == 0, value
                else:
                    completed = frame(evidence, case + '-frame')
                    summary.setdefault('framePipelines', {})[case] = completed['surface']['nativeFramePipeline']
                summary['checks'][case] = value
            save()
            print('PASS physical iOS ' + case, flush=True)
        except Exception as error:
            if case == 'rotation':
                copy('Documents/' + remote + '/result.json.partial.json', out / 'rotation-partial.json')
            summary['caseFailures'][case] = str(error)
            summary['checks'][case] = {'status': 'FAILED', 'error': str(error)}
            save()
            print('FAIL physical iOS ' + case + ': ' + str(error), flush=True)
        finally:
            terminate(pid)
    environment = base | {'DOROTI_SAMPLE': '', 'DOROTI_MAUI_EVIDENCE': run_id + '/final-evidence.json'}
    summary['qualificationForegroundPid'] = launch('final-evidence', environment)
    summary['finalFrame'] = frame(run_id + '/final-evidence.json', 'final-frame')['frame']
    # Leave the actual manual-use app free of probe/evidence/profiling work.
    summary['foregroundPid'] = launch('final', {'DOROTI_IOS_GRAPHITE': '1'})
    summary['finalLaunch'] = 'normal Material app; no probe, evidence writer or detailed frame profiling'
    time.sleep(2)
    screenshot = device('capture', 'screenshot', '--destination', str(out / 'screen.png'), optional=True)
    summary['screenCapture'] = 'PASS' if screenshot.returncode == 0 else 'notVerified: ' + screenshot.stderr
    summary['deviceProbeFiles'] = 'Retained in app Documents/' + run_id + '; no app data reset'
    if summary['caseFailures']:
        raise RuntimeError('Physical iOS cases failed: ' + ', '.join(summary['caseFailures']))
    summary['result'] = 'PASS within physical-device native/synthetic scope'
    save()
except BaseException as error:
    summary['result'] = 'FAILED'
    summary['error'] = str(error)
    save()
    raise
