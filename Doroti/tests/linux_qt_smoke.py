"""Interactive Linux Qt qualification. Native automation is not physical input/Orca.

Run under eng/run-with-timeout.py (1,200 seconds). Own raw artifacts remain at --output
until the result is recorded; no access to unrelated windows or user files.
"""
from pathlib import Path
import argparse
import json
import os
import shlex
import subprocess
import time
import sys

ROOT = Path(__file__).resolve().parents[2]
parser = argparse.ArgumentParser()
parser.add_argument('--output', required=True, type=Path)
parser.add_argument('--qpa', choices=['wayland', 'xcb'], default='wayland' if os.environ.get('WAYLAND_DISPLAY') else 'xcb')
parser.add_argument('--skip-build', action='store_true')
parser.add_argument('--exe', type=Path, help='Explicit isolated build payload to qualify.')
parser.add_argument('--cases', default='multi,desktop,services,navigation,input,resize')
args = parser.parse_args()
out = args.output.resolve()
assert out.is_relative_to(ROOT / 'temp/testing')
out.mkdir(parents=True, exist_ok=True)
exe = args.exe.resolve() if args.exe else ROOT / 'samples/DorotiTestbedApp/linux/bin/linux-x64/Debug/net10.0/linux-x64/DorotiTestbedApp.Linux'
if not args.skip_build:
    with (out / 'build.log').open('w') as log:
        subprocess.run(['dotnet', 'build', str(ROOT / 'samples/DorotiTestbedApp/linux/DorotiTestbedApp.Linux.csproj'),
            '-c', 'Debug', '-p:DorotiLinuxDesktop=true', '-v:minimal'], stdout=log, stderr=subprocess.STDOUT, check=True, timeout=1200)
subprocess.run([sys.executable, str(ROOT / 'Doroti/tests/linux_qt_files.py'), str(exe.parent / 'libdoroti_qt_host.so'), str(out / 'file-grants')], check=True, timeout=1200)
flags = shlex.split(subprocess.check_output(['pkg-config', '--cflags', '--libs', 'Qt6Quick', 'Qt6Widgets'], text=True))
driver = out / 'libevidence.so'
subprocess.run(['c++', '-shared', '-fPIC', str(ROOT / 'Doroti/tests/native/qt_evidence.cpp'), '-o', str(driver), *flags], check=True, timeout=1200)
env = os.environ | {'QT_QPA_PLATFORM': args.qpa, 'LD_PRELOAD': str(driver), 'XDG_DATA_HOME': str(out / 'userdata')}
# Use Qt's in-process dialog for deterministic automation, with the same Qt API.
env['QT_QPA_PLATFORMTHEME'] = 'generic'
results = {}

def start(name, extra, arguments=()):
    log = (out / (name + '.log')).open('w')
    process = subprocess.Popen([str(exe), *arguments], env=env | extra, stdout=log, stderr=subprocess.STDOUT)
    process._doroti_log = log
    return process

def finish(process, name):
    try:
        code = process.wait(timeout=100)
        assert code == 0, (name, code)
    finally:
        if process.poll() is None: process.kill(); process.wait()
        process._doroti_log.close()
    text = (out / (name + '.log')).read_text()
    for error in ['PlatformView creation failed.', 'doroti.qt.desktop.failure=', 'Unhandled exception.', 'doroti.qt.fatal=', 'managed.fatal=']:
        assert error not in text, (name, error)
    summaries = [json.loads(line.split('=', 1)[1]) for line in text.splitlines() if line.startswith('doroti.qt.summary=')]
    assert summaries and all(item['frames']['failed'] == 0 for item in summaries), name
    return summaries

def wait_file(file, process, expected=None):
    deadline = time.monotonic() + 70
    while time.monotonic() < deadline:
        assert not Path(str(file) + '.error').exists(), Path(str(file) + '.error').read_text() if Path(str(file) + '.error').exists() else ''
        if file.exists() and (expected is None or expected in file.read_text()): return
        assert process.poll() is None, ('early exit', process.returncode, file)
        time.sleep(.05)
    raise TimeoutError(str(file))

try:
    for case in args.cases.split(','):
        if case == 'multi':
            for lifetime in ['OnLastWindowClosed', 'Explicit']:
                name = 'multi-' + lifetime
                path = out / (name + '.json')
                process = start(name, {'DOROTI_MULTIWINDOW_PROBE': str(path), 'DOROTI_SAMPLE': 'input', 'DOROTI_DESKTOP_LIFETIME': lifetime})
                finish(process, name)
                report = json.loads(path.read_text())
                captures = json.loads(Path(str(path) + '.native.json').read_text())
                assert len(captures) == 2 and report['remaining'] == 0 and report['exits'] == 1
                results[name] = report | {'captures': captures}
        elif case == 'desktop':
            path = out / 'desktop.json'
            process = start(case, {'DOROTI_QT_DESKTOP_PROBE': str(path), 'DOROTI_SAMPLE': 'reload'})
            finish(process, case)
            report = json.loads(Path(str(path) + '.closed').read_text())
            assert report['Closed'] and report['remaining'] == 0 and report['closingCallbacks'] == 2
            results[case] = json.loads(path.read_text())
        elif case == 'services':
            path = out / 'services.json'
            small = out / '한글.txt'; small.write_text('Qt 한글 read grant')
            large = out / 'large.bin'
            with large.open('wb') as file: file.truncate(5 * 1024 * 1024 * 1024)
            process = start(case, {'DOROTI_QT_SERVICES_PROBE': str(path), 'DOROTI_QT_PICK_FILE': str(small),
                'DOROTI_QT_LARGE_FILE': str(large), 'DOROTI_SAMPLE': 'plugins', 'DOROTI_QT_AUTOCLOSE_FILE': str(path)})
            finish(process, case)
            assert not Path(str(path) + '.error').exists(), Path(str(path) + '.error').read_text() if Path(str(path) + '.error').exists() else ''
            results[case] = json.loads(path.read_text())
        elif case == 'navigation':
            route = out / 'route.txt'; gate = out / 'navigation.close'
            navenv = {'DOROTI_SAMPLE': 'navigation', 'DOROTI_NAVIGATION_PROBE': str(route), 'DOROTI_QT_AUTOCLOSE_FILE': str(gate)}
            process = start('navigation-cold', navenv, ['doroti-testbed://cold/한글'])
            wait_file(route, process, 'cold')
            with (out / 'navigation-warm.log').open('w') as log:
                subprocess.run([str(exe), 'doroti-testbed://warm/second'], env=env | navenv, stdout=log, stderr=subprocess.STDOUT, check=True, timeout=1200)
            wait_file(route, process, 'warm/second')
            gate.write_text('close'); finish(process, 'navigation-cold')
            gate.unlink(); route.unlink()
            process = start('navigation-restored', navenv)
            wait_file(route, process, 'warm/second')
            gate.write_text('close'); finish(process, 'navigation-restored')
            gate.unlink(); route.unlink()
            process = start('navigation-crash', navenv)
            wait_file(route, process, 'warm/second')
            # A frame's build probe precedes Router's end-of-frame persistence.
            # Kill only after the requested route and its restoration bytes commit.
            import base64
            checkpoint_deadline = time.monotonic() + 15
            while True:
                snapshots = [json.loads(file.read_text()) for file in (out / 'userdata').rglob('*.json')]
                if snapshots and all('warm/second' in item['location'] and b'warm/second' in base64.b64decode(item['data'] or '') for item in snapshots): break
                if time.monotonic() > checkpoint_deadline: raise TimeoutError('Router checkpoint did not commit')
                time.sleep(.05)
            (out / 'before-kill.json').write_text(json.dumps(snapshots))
            process.kill(); process.wait(); process._doroti_log.close()
            checkpoints = list((out / 'userdata').rglob('*.json'))
            assert checkpoints and any(not json.loads(file.read_text())['cleanShutdown'] for file in checkpoints)
            route.unlink()
            process = start('navigation-after-crash', navenv)
            wait_file(route, process, 'warm/second')
            gate.write_text('close'); finish(process, 'navigation-after-crash')
            # Bad envelope versions must use a clean fallback, not saved route/data.
            for checkpoint in checkpoints: checkpoint.write_text('{"version":999,"location":"/must-not-restore"}')
            gate.unlink(); route.unlink()
            process = start('navigation-corrupt', navenv)
            wait_file(route, process)
            assert route.read_text().splitlines()[0] == '/'
            gate.write_text('close'); finish(process, 'navigation-corrupt')
            gate.unlink(); route.unlink()
            process = start('navigation-cold-priority', navenv, ['doroti-testbed://priority/page'])
            wait_file(route, process, 'priority/page')
            gate.write_text('close'); finish(process, 'navigation-cold-priority')
            checkpoints = list((out / 'userdata').rglob('*.json'))
            assert checkpoints and all(json.loads(file.read_text())['cleanShutdown'] for file in checkpoints)
            results[case] = {'cold': True, 'warm': True, 'restartRoute': True, 'freshXdgDirectory': True, 'forcedKillRestore': True, 'badVersionFallback': True, 'coldPriority': True}
        elif case == 'input':
            path = out / 'input.json'
            process = start(case, {'DOROTI_SAMPLE': 'input', 'DOROTI_INPUT_PROBE': str(path), 'DOROTI_QT_AUTOCLOSE_FILE': str(path),
                'DOROTI_QT_VALIDATION_ACCESSIBILITY_DUMP': '1'})
            finish(process, case)
            assert not Path(str(path) + '.error').exists(), Path(str(path) + '.error').read_text() if Path(str(path) + '.error').exists() else ''
            results[case] = json.loads(path.read_text())
        elif case == 'resize':
            process = start(case, {'DOROTI_SAMPLE': 'reload', 'DOROTI_QT_VALIDATION_RESIZE_CYCLES': '20'})
            results[case] = finish(process, case)[0]
        else: raise ValueError(case)
        print('PASS', case, flush=True)
finally:
    if 'process' in globals() and process.poll() is None:
        process.kill(); process.wait(); process._doroti_log.close()
    (out / 'results.json').write_text(json.dumps(results, ensure_ascii=False, indent=2))
print('Linux Qt automation PASS; physical IME/Orca/external drag remain notVerified.', flush=True)
