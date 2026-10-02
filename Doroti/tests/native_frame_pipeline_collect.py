"""Bounded C-only queue/lifecycle and before/after probes; submission counts are never displayed FPS.

Run each repeat under eng/run-with-timeout.py. Three repeats of the default five
conditions and two C payloads produce 30 runs, with alternating candidate order. Genuine
display events can be attached as display-events.json in each run directory:
{method, timestampsNanoseconds:[...]}. Keep the capture tool's original output.
No events means notMeasured, never a performance PASS.
"""
import argparse
import hashlib
import json
import math
import os
from pathlib import Path
import subprocess
import time

ROOT = Path(__file__).resolve().parents[2]


def digest(path):
    with path.open('rb') as stream:
        return hashlib.file_digest(stream, 'sha256').hexdigest()


def payload_identity(exe):
    # Apphosts alone can be byte-identical across source changes. Include the
    # managed dependencies, native shim/Skia and actual runtime/deps manifests.
    root = exe.resolve().parent
    files = sorted(p for p in root.rglob('*') if p.is_file() and
                   (p.suffix in ('.exe','.dll','.so','.json') or '.so.' in p.name))
    entries = [{'path':p.relative_to(root).as_posix(),'sha256':digest(p)} for p in files]
    value = hashlib.sha256(''.join(x['path']+'\0'+x['sha256']+'\n' for x in entries).encode()).hexdigest()
    return {'sha256':value,'files':entries}


def display_summary(path):
    if not path.exists():
        return {'status': 'notMeasured', 'fps': None, 'inputToDisplay': None,
                'hardwareOverlap': 'notMeasured'}
    capture = json.loads(path.read_text())
    assert capture.get('method'), 'Display capture provenance is required'
    stamps = capture['timestampsNanoseconds']
    assert len(stamps) > 2 and all(b > a for a, b in zip(stamps, stamps[1:])), 'Invalid display timeline'
    low, high = stamps[0] + 5_000_000_000, stamps[0] + 35_000_000_000
    assert stamps[-1] >= high, 'Incomplete 30-second warm display window'
    intervals = [(b-a)/1e6 for a,b in zip(stamps, stamps[1:]) if low <= a and b <= high]
    ordered = sorted(intervals)
    assert ordered
    return {'status': 'measured', 'method': capture['method'], 'sourceSha256': digest(path),
            'samples': len(intervals), 'fps': 1000*len(intervals)/sum(intervals),
            **{f'p{int(q*100)}Ms': ordered[math.ceil(q*len(ordered))-1] for q in (.5,.95,.99)},
            'over25MsFraction': sum(x > 25 for x in intervals)/len(intervals),
            'inputToDisplay': None, 'hardwareOverlap': 'notMeasured'}


def close_windows(process):
    import ctypes
    from ctypes import wintypes
    user = ctypes.WinDLL('user32', use_last_error=True)
    callback = ctypes.WINFUNCTYPE(wintypes.BOOL, wintypes.HWND, wintypes.LPARAM)
    user.GetWindowThreadProcessId.argtypes = [wintypes.HWND, ctypes.POINTER(wintypes.DWORD)]
    user.PostMessageW.argtypes = [wintypes.HWND, wintypes.UINT, wintypes.WPARAM, wintypes.LPARAM]
    user.GetWindow.argtypes = [wintypes.HWND, wintypes.UINT]
    user.GetWindow.restype = wintypes.HWND
    user.IsWindowVisible.argtypes = [wintypes.HWND]
    @callback
    def visit(hwnd, _):
        owner = wintypes.DWORD()
        user.GetWindowThreadProcessId(hwnd, ctypes.byref(owner))
        if owner.value == process.pid and user.IsWindowVisible(hwnd):
            user.PostMessageW(hwnd, 0x10, 0, 0)  # WM_CLOSE, own process only
        return True
    user.EnumWindows.argtypes = [callback, wintypes.LPARAM]
    user.EnumWindows(visit, 0)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--platform', choices=['windows', 'qt', 'android'], required=True)
    parser.add_argument('--output', type=Path, required=True)
    parser.add_argument('--exe', type=Path)
    parser.add_argument('--driver', type=Path, help='Qt test-only libevidence.so')
    parser.add_argument('--adb', default='adb')
    parser.add_argument('--device')
    parser.add_argument('--package', default='dev.doroti.sample2')
    parser.add_argument('--activity', default='crc6467bcc435301192e0.MainActivity')
    parser.add_argument('--conditions', default='off:0,fast:20,fast:32,adaptive:20,adaptive:32')
    parser.add_argument('--setting', choices=['unset', 'C'], default='unset', help='Probe the only policy, with no selector or explicit C')
    parser.add_argument('--candidate', choices=['before', 'after'], default='after', help='Payload label without a baseline')
    parser.add_argument('--baseline-exe', type=Path, help='Preserved before-C desktop executable for paired regression')
    parser.add_argument('--repeat', type=int, choices=[1,2,3], default=1)
    parser.add_argument('--seconds', type=float, default=38)
    args = parser.parse_args()
    out = args.output.resolve()
    assert out.is_relative_to(ROOT / 'temp/testing'), 'Raw evidence must be under temp/testing'
    if out.exists():
        parser.error('Use a fresh output directory; earlier runs and failures are preserved')
    assert 5 <= args.seconds <= 45
    if args.platform != 'android' and (args.exe is None or not args.exe.exists()):
        parser.error('--exe must identify an existing built payload')
    out.mkdir(parents=True, exist_ok=True)
    if args.baseline_exe and (args.platform == 'android' or not args.baseline_exe.exists()):
        parser.error('--baseline-exe requires an existing desktop executable')
    candidates = [('before', args.baseline_exe), ('after', args.exe)] if args.baseline_exe else [(args.candidate, args.exe)]
    identities = {label: payload_identity(exe) for label, exe in candidates if exe}
    if identities:
        (out/'payload-manifest.json').write_text(json.dumps(identities,indent=2))
    results = []
    def adb(*command, **kwargs):
        assert args.device, 'Use an explicit owned device ID'
        return subprocess.run([args.adb, '-s', args.device, *command], check=True, timeout=30, **kwargs)
    conditions = args.conditions.split(',')
    if args.repeat % 2 == 0: conditions.reverse(); candidates.reverse()
    try:
        for index, condition in enumerate(conditions):
            mode, sigma = condition.split(':')
            assert mode in ('off','full','adaptive','fast','fixed','kawase') and 0 <= float(sigma) <= 32
            order = candidates[index % len(candidates):] + candidates[:index % len(candidates)]
            for candidate, exe in order:
                identity = identities.get(candidate)
                run = out / f'r{args.repeat}-{mode}-{sigma}-{candidate}-{args.setting}'
                run.mkdir()  # Never overwrite an earlier failure/run.
                settings = {'DOROTI_VARIABLE_BLUR_BENCHMARK': mode,
                            'DOROTI_VARIABLE_BLUR_BENCHMARK_SIGMA': sigma}
                if args.setting == 'C': settings['DOROTI_NATIVE_FRAME_MODE'] = 'C'
                result = {'condition': condition, 'policy': 'C', 'candidate': candidate, 'setting': args.setting, 'repeat': args.repeat,
                          'seconds': args.seconds, 'physicalInput': 'notVerified'}
                results.append(result)
                try:
                    if args.platform == 'android':
                        cache = f'/sdcard/Android/data/{args.package}/cache/doroti-maui-evidence.json'
                        adb('shell', 'am', 'force-stop', args.package)
                        adb('shell', 'rm', '-f', cache)
                        extra = ['--es','DOROTI_MAUI_EVIDENCE','1']
                        for key,value in settings.items(): extra += ['--es',key,value]
                        with (run/'launch.log').open('w') as log:
                            adb('shell','am','start','-W','-n',f'{args.package}/{args.activity}',*extra,stdout=log,stderr=subprocess.STDOUT)
                        time.sleep(args.seconds)
                        adb('pull', cache, str(run/'evidence.json'), stdout=subprocess.DEVNULL)
                        adb('shell','am','force-stop',args.package)
                        document = json.loads((run/'evidence.json').read_text())
                        result['pipeline'] = document['surface']['nativeFramePipeline']
                        result['frames'] = document['frame']
                    else:
                        assert exe and exe.exists()
                        environment = os.environ.copy()
                        for key in ('DOROTI_NATIVE_FRAME_MODE','DOROTI_VARIABLE_BLUR_SERIAL_FRAMES',
                                    'DOROTI_VARIABLE_BLUR_PIPELINE','DOROTI_NATIVE_PRESENTATION','DOROTI_IOS_SHADER_PRESENTATION',
                                    'DOROTI_VARIABLE_BLUR_BENCHMARK_STATIC','DOROTI_SAMPLE'):
                            environment.pop(key, None)
                        environment.update(settings)
                        environment['DOROTI_MAUI_EVIDENCE'] = str(run/'evidence.json')
                        environment['DOROTI_WINDOWS_APPSDK_REPORT'] = str(run/'evidence.json')
                        environment['DOROTI_WINDOWS_APPSDK_DIAGNOSTICS'] = '1'
                        if args.platform == 'qt':
                            assert args.driver and args.driver.exists()
                            environment['LD_PRELOAD'] = str(args.driver.resolve())
                            environment['DOROTI_QT_DIAGNOSTICS'] = '1'
                            environment['DOROTI_QT_PIPELINE_DURATION_MS'] = str(int(args.seconds*1000))
                        with (run/'application.log').open('w', encoding='utf-8') as log:
                            process = subprocess.Popen([str(exe.resolve())], env=environment, cwd=ROOT,
                                                       stdout=log,stderr=subprocess.STDOUT)
                            try:
                                if args.platform == 'windows':
                                    time.sleep(args.seconds)
                                    close_windows(process)
                                code = process.wait(timeout=args.seconds+30)
                                assert code == 0, f'Application exit {code}'
                            finally:
                                if process.poll() is None: process.kill(); process.wait()
                        output = (run/'application.log').read_text(encoding='utf-8',errors='replace')
                        assert not any(x in output for x in ('Unhandled exception','managed.fatal=','doroti.qt.fatal=','PlatformView creation failed.'))
                        result['apphostSha256'] = digest(exe)
                        result['payloadSha256'] = identity['sha256']
                        if args.platform == 'qt':
                            prefix = 'doroti.qt.summary='
                            summaries = [json.loads(x[len(prefix):]) for x in output.splitlines() if x.startswith(prefix)]
                            assert summaries
                            document = summaries[-1]
                            (run/'evidence.json').write_text(json.dumps(document,indent=2))
                            result['pipeline'] = {k: document[k] for k in ('nativeFrameMode','frameworkPreparedPulses',
                                'quickMaximumPending','quickConsumersSubmitted','quickConsumersCompleted','gpuRetirement')}
                            result['frames'] = document['frames']
                            assert document['quickConsumersSubmitted'] == document['quickConsumersCompleted']
                        else:
                            document = json.loads((run/'evidence.json').read_text())
                            result['pipeline'] = document.get('nativeFramePipeline') or document['surface']['nativeFramePipeline']
                            result['frames'] = document.get('frames') or document['frame']
                    pipeline = result['pipeline']
                    actual = pipeline.get('mode') or pipeline.get('nativeFrameMode')
                    assert actual == 'C', actual
                    maximum = pipeline.get('maximumPending',pipeline.get('maximumGpuFrames',pipeline.get('quickMaximumPending',0)))
                    assert maximum <= 2, pipeline
                    frames = result['frames']
                    failed = frames.get('failed', frames.get('failedTerminals', 0))
                    presented = frames.get('presented', frames.get('presentedTerminals', 0))
                    assert failed == 0, f'{failed} failed frames'
                    assert presented > 0, 'No presentation receipts during the probe'
                    result['status'] = 'PASS'
                    result['display'] = display_summary(run/'display-events.json')
                    print(f'PASS {run.name} maxPending={maximum}; display={result["display"]["status"]}',flush=True)
                except Exception as error:
                    result['status'] = 'FAIL'; result['error'] = str(error)
                    raise
                finally:
                    (run/'result.json').write_text(json.dumps(result,indent=2))
    finally:
        (out/'results.json').write_text(json.dumps(results,indent=2))


if __name__ == '__main__': main()
