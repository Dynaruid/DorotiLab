"""AppKit/Metal native automation. Run with eng/run-with-timeout.py --timeout 1200.

Captures only this testbed's windows. Physical IME/VoiceOver/Finder and clean OS
distribution are separate gates. Raw files remain at --output for investigation.
"""
import argparse
import json
import os
from pathlib import Path
import subprocess
import time
import uuid

ROOT = Path(__file__).resolve().parents[2]
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--output', type=Path, required=True)
parser.add_argument('--tfm', default='net10.0-macos27.0')
parser.add_argument('--skip-build', action='store_true')
parser.add_argument('--cases', default='multi,windowing,desktop,services,input,features,navigation,rendering,lifecycle')
parser.add_argument('--renderer', choices=['graphite', 'ganesh'], default='graphite')
args = parser.parse_args()
out = args.output.resolve()
assert out.is_relative_to(ROOT / 'temp/testing')
out.mkdir(parents=True, exist_ok=True)
project = ROOT / 'samples/DorotiTestbedApp/macos/DorotiTestbedApp.MacOS.csproj'
if not args.skip_build:
    with (out / 'build.log').open('w') as log:
        subprocess.run(['dotnet', 'build', str(project), '-c', 'Debug', '-r', 'osx-arm64',
            '-p:DorotiMacOSTargetFramework=' + args.tfm], stdout=log, stderr=subprocess.STDOUT, check=True, timeout=1200)
app = project.parent / 'bin/Debug' / args.tfm / 'osx-arm64/Doroti Testbed (AppKit).app'
exe = app / 'Contents/MacOS/DorotiTestbedApp.MacOS'
restoration_id = 'macos-smoke-' + uuid.uuid4().hex
base_env = os.environ | {'DOROTI_MACOS_GRAPHITE': '1' if args.renderer == 'graphite' else '0',
    'DOROTI_SAMPLE': 'reload', 'DOROTI_DESKTOP_SAMPLE': 'solid', 'DOROTI_RESTORATION_ID': restoration_id}
results = {'renderer': args.renderer, 'physicalInput': 'notVerified', 'restorationId': restoration_id, 'checks': {}}
active = []


def start(name, extra=None, arguments=()):
    directory = out / name
    directory.mkdir()
    log = (directory / 'app.log').open('w')
    process = subprocess.Popen([str(exe), *arguments], env=base_env | {
        'DOROTI_MACOS_AUTOMATION': str(directory), 'DOROTI_MAUI_EVIDENCE': str(directory / 'evidence.json')
    } | (extra or {}), stdout=log, stderr=subprocess.STDOUT)
    active.append(process)
    process._doroti = (directory, log)
    return process


def check_errors(process):
    directory, _ = process._doroti
    errors = list(directory.glob('*.error')) + list(directory.glob('*.exception.txt'))
    assert not errors, '\n'.join(p.read_text() for p in errors)


def wait_file(process, path, contains=None):
    until = time.monotonic() + 65
    while time.monotonic() < until:
        check_errors(process)
        failure = Path(str(path) + '.error')
        assert not failure.exists(), failure.read_text() if failure.exists() else ''
        if path.exists() and (contains is None or contains in path.read_text()): return
        assert process.poll() is None, ('early exit', process.returncode, path)
        time.sleep(.05)
    raise TimeoutError(str(path))


def command(process, op='snapshot', terminal=False, **values):
    directory, _ = process._doroti
    response = directory / 'response.json'
    response.unlink(missing_ok=True)
    temporary = directory / 'request.tmp'
    temporary.write_text(json.dumps({'op': op} | values))
    temporary.replace(directory / 'request.json')
    if op == 'exit' or terminal: return
    wait_file(process, response)
    return json.loads(response.read_text())


def windows(process, count):
    until = time.monotonic() + 60
    while time.monotonic() < until:
        data = command(process)
        if len(data) == count and all(w['IsVisible'] and w['views'][0]['CommandBuffersCompleted'] > 0 for w in data): return data
        time.sleep(.1)
    raise TimeoutError('native window count/presentation')


def finish(process):
    assert process.wait(timeout=65) == 0
    check_errors(process)
    directory, log = process._doroti
    log.close()
    text = (directory / 'app.log').read_text()
    assert 'Exception of type' not in text and 'Unhandled exception' not in text, text[-8000:]
    active.remove(process)


try:
    for case in args.cases.split(','):
        if case == 'windowing':
            path = out / 'windowing.json'
            process = start('windowing', {'DOROTI_APPKIT_WINDOWING_PROBE': str(path)})
            wait_file(process, path)
            result = json.loads(path.read_text())
            assert set(result) == {'Dialog', 'Popup', 'Tooltip', 'Satellite', 'menu', 'ownerClose'}, result
            assert not result['Tooltip']['canBecomeKey'] and result['menu']['selected'] == 1, result
            finish(process)
            results['checks']['windowing'] = result
        elif case == 'multi':
            for lifetime in ['OnLastWindowClosed', 'Explicit']:
                process = start('multi-' + lifetime, {'DOROTI_MULTIWINDOW_SAMPLE': '1',
                    'DOROTI_DESKTOP_LIFETIME': lifetime, 'DOROTI_SAMPLE': 'input'})
                before = windows(process, 2)
                first = next(w for w in before if w['Title'] != 'Doroti second window')
                second = next(w for w in before if w['Title'] == 'Doroti second window')
                assert first['id'] != second['id'] and first['width'] != second['width']
                directory, _ = process._doroti
                captures = []
                for window in before:
                    command(process, 'focus', id=window['id'])
                    time.sleep(.3)
                    png = directory / (str(window['id']) + '.png')
                    capture = subprocess.run(['screencapture', '-x', '-l', str(window['id']), str(png)], capture_output=True)
                    captures.append('captured' if capture.returncode == 0 else 'notVerified: ' + capture.stderr.decode().strip())
                command(process, 'close', id=first['id'])
                survivor = windows(process, 1)
                committed = survivor[0]['views'][0]['CommandBuffersCompleted']
                for i in range(12):
                    command(process, 'resize', id=second['id'], width=580 + i * 2, height=660 + i * 2)
                time.sleep(.5)
                after = windows(process, 1)
                assert after[0]['views'][0]['CommandBuffersCompleted'] > committed
                assert after[0]['views'][0]['CommandBuffersErrored'] == 0
                command(process, 'close', id=second['id'], terminal=lifetime == 'OnLastWindowClosed')
                if lifetime == 'Explicit':
                    time.sleep(.3)
                    assert process.poll() is None and command(process) == []
                    command(process, 'exit')
                finish(process)
                results['checks']['multi-' + lifetime] = {'before': before, 'survivor': after, 'screenshots': captures}
        elif case == 'stability':
            process = start(case)
            command(process, 'resize', width=600, height=700)
            before = windows(process, 1)
            started = time.monotonic()
            while time.monotonic() - started < 60:
                check_errors(process)
                assert process.poll() is None, 'AppKit exited during the 60-second observation'
                time.sleep(1)
            command(process, 'resize', width=620, height=720)
            deadline = time.monotonic() + 15
            while True:
                after = windows(process, 1)
                if after[0]['views'][0]['CommandBuffersCompleted'] > before[0]['views'][0]['CommandBuffersCompleted']: break
                assert time.monotonic() < deadline, 'Frames did not progress after the 60-second observation'
                time.sleep(.1)
            results['checks'][case] = {'seconds': time.monotonic() - started, 'before': before, 'after': after, 'normalExit': True}
            command(process, 'exit')
            finish(process)
        elif case == 'rendering':
            process = start(case)
            before = windows(process, 1)
            for i in range(24):
                command(process, 'resize', width=500 + (i % 8) * 8, height=680 + (i % 8) * 6)
            time.sleep(.5)
            after = windows(process, 1)
            assert after[0]['views'][0]['CommandBuffersErrored'] == 0
            assert after[0]['views'][0]['CommandBuffersCompleted'] > before[0]['views'][0]['CommandBuffersCompleted']
            view = after[0]['views'][0]
            pipeline = view['NativeFramePipeline']
            assert pipeline['Mode'] == 'C' and 0 < pipeline['MaximumGpuFrames'] <= 2, pipeline
            if args.renderer == 'ganesh': assert pipeline['MaximumGpuFrames'] == 1, pipeline
            assert pipeline['CompletedGpuFrames'] > 0, pipeline
            assert view['PresentedDrawables'] > 0, 'Supported AppKit drawable observation stopped reporting displays'
            assert view['PixelWidth'] == round(after[0]['width'] * after[0]['scale']), view
            assert view['PixelHeight'] == round(after[0]['height'] * after[0]['scale']), view
            results['checks'][case] = {'before': before, 'after': after, 'sustainedFps': 'notMeasured', 'resizeCount': 24}
            command(process, 'exit')
            finish(process)
        elif case == 'lifecycle':
            path = out / 'lifecycle.json'
            process = start(case, {'DOROTI_MACOS_FRAME_PROBE': str(path)})
            finish(process)
            report = json.loads(path.read_text())
            assert report['status'] == 'PASS', report
            results['checks'][case] = report
        elif case == 'desktop':
            path = out / 'desktop.json'
            process = start(case, {'DOROTI_DESKTOP_PROBE': str(path), 'DOROTI_DESKTOP_CLOSE_PROBE': 'api'})
            finish(process)
            report = json.loads(path.read_text())
            assert not report['beforeShow']['Visible'] and report['shown']['Visible']
            assert report['resize']['size'] == [500, 650] and report['minimized']['presentation'] == 'Minimized'
            assert Path(str(path) + '.close').read_text() == '2'
            results['checks'][case] = report
        elif case == 'services':
            path = out / 'services.json'
            process = start(case, {'DOROTI_MACOS_SERVICES_PROBE': str(path)})
            finish(process)
            assert not Path(str(path) + '.error').exists(), Path(str(path) + '.error').read_text() if Path(str(path) + '.error').exists() else ''
            results['checks'][case] = json.loads(path.read_text())
        elif case == 'features':
            path = out / 'features.json'
            process = start(case, {'DOROTI_APPLE_FEATURE_PROBE': str(path)})
            wait_file(process, path)
            report = json.loads(path.read_text())
            assert report['status'] == 'PASS', report
            results['checks'][case] = report
            command(process, 'exit')
            finish(process)
        elif case == 'input':
            path = out / 'input.json'
            process = start(case, {'DOROTI_SAMPLE': 'input', 'DOROTI_INPUT_PROBE': str(path)})
            wait_file(process, path)
            assert not Path(str(path) + '.error').exists()
            results['checks'][case] = json.loads(path.read_text())
            command(process, 'exit')
            finish(process)
        elif case == 'navigation':
            path = out / 'route.txt'
            cold, warm = 'doroti-testbed://navigation/#/cold', 'doroti-testbed://navigation/#/warm'
            process = start(case, {'DOROTI_SAMPLE': 'navigation', 'DOROTI_NAVIGATION_PROBE': str(path)}, [cold])
            wait_file(process, path, cold)
            subprocess.run(['open', '-a', str(app), warm], check=True, timeout=20)
            wait_file(process, path, warm)
            command(process, 'exit')
            finish(process)
            path.unlink()
            process = start('restoration', {'DOROTI_SAMPLE': 'navigation', 'DOROTI_NAVIGATION_PROBE': str(path)})
            wait_file(process, path, warm)
            results['checks'][case] = {'coldArgument': cold, 'warmLaunchServices': warm, 'restored': path.read_text()}
            command(process, 'exit')
            finish(process)
        else:
            raise ValueError('Unknown case: ' + case)
    results['status'] = 'PASS (native automation only)'
finally:
    for process in active:
        if process.poll() is None:
            process.terminate()
            try: process.wait(timeout=10)
            except subprocess.TimeoutExpired: process.kill(); process.wait()
        process._doroti[1].close()
    (out / 'summary.json').write_text(json.dumps(results, ensure_ascii=False, indent=2))
print(json.dumps(results, ensure_ascii=False, indent=2))
