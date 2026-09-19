"""Bounded product checks on an explicitly selected Android device. Run via run-with-timeout.py."""
import argparse
import json
import re
from pathlib import Path
import subprocess
import time

PACKAGE = 'dev.doroti.testbed'
REMOTE = f'/sdcard/Android/data/{PACKAGE}/files'


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--serial', required=True)
    parser.add_argument('--out', type=Path, required=True)
    parser.add_argument('--mode', choices=['commands', 'calibration', 'workloads', 'capture'], default='commands')
    args = parser.parse_args()
    args.out.mkdir(parents=True, exist_ok=True)

    def adb(*command, check=True, binary=False):
        result = subprocess.run(['adb', '-s', args.serial, *command], capture_output=True, timeout=30)
        if check and result.returncode:
            raise RuntimeError(result.stderr.decode(errors='replace'))
        return result.stdout if binary else result.stdout.decode(errors='replace').strip()

    activity = adb('shell', 'cmd', 'package', 'resolve-activity', '--brief', PACKAGE).splitlines()[-1]
    if '/' not in activity:
        raise RuntimeError('Installed testbed activity unavailable: ' + activity)
    (args.out / 'device.txt').write_text(adb('shell', 'getprop') + '\n' + adb('shell', 'dumpsys', 'webviewupdate'), encoding='utf-8')

    def start(mode='platform-effects', extras=()):
        adb('shell', 'am', 'force-stop', PACKAGE)
        adb('shell', 'am', 'start', '-W', '-n', activity, '--es', 'doroti_testbed_mode', mode,
            '--es', 'DOROTI_MAUI_EVIDENCE', '1', *extras)

    def screenshot(name):
        (args.out / (name + '.png')).write_bytes(adb('exec-out', 'screencap', '-p', binary=True))

    def wait_native_frame():
        deadline = time.monotonic() + 40
        while time.monotonic() < deadline:
            pid = adb('shell', 'pidof', PACKAGE, check=False)
            if pid:
                trace = adb('logcat', '-d', '--pid=' + pid, '-s', 'DorotiPlatformFrame:I')
                if re.search(r'readbackFrames=[1-9]\d*', trace):
                    return
            time.sleep(.5)
        raise TimeoutError('No committed native/raster product frame within 40 seconds.')

    def read(name):
        return adb('shell', 'cat', REMOTE + '/' + name, check=False)

    try:
        if args.mode == 'commands':
            adb('shell', 'rm', '-f', REMOTE + '/webview-evidence.txt')
            start(extras=('--es', 'doroti_webview_evidence', '1'))
            deadline = time.monotonic() + 100
            while time.monotonic() < deadline:
                result = read('webview-evidence.txt')
                if result:
                    (args.out / 'commands.txt').write_text(result, encoding='utf-8')
                    screenshot('commands')
                    if 'FAIL' in result or 'PASS Android WebView product commands' not in result:
                        raise AssertionError(result)
                    print(result, flush=True)
                    break
                if not adb('shell', 'pidof', PACKAGE, check=False):
                    raise RuntimeError('Testbed process exited before writing evidence.')
                time.sleep(1)
            else:
                raise TimeoutError('Product command evidence was not written within 100 seconds.')
        elif args.mode == 'calibration':
            for suffix in ('stage', 'ack', 'done'):
                adb('shell', 'rm', '-f', REMOTE + '/effect-calibration.' + suffix)
            start(extras=('--es', 'doroti_effect_calibration', '1'))
            wait_native_frame()
            deadline = time.monotonic() + 110
            captured = set()
            while time.monotonic() < deadline:
                done = read('effect-calibration.done')
                if done:
                    (args.out / 'calibration.txt').write_text(done, encoding='utf-8')
                    if done != 'PASS':
                        raise AssertionError(done)
                    print('Captured ' + ', '.join(sorted(captured)), flush=True)
                    break
                stage = read('effect-calibration.stage')
                if stage and stage not in captured:
                    if not all(c.isalnum() or c == '-' for c in stage):
                        raise ValueError('Unexpected stage')
                    screenshot(stage)
                    local_ack = args.out / 'ack.txt'
                    local_ack.write_text(stage, encoding='utf-8')
                    adb('push', str(local_ack), REMOTE + '/effect-calibration.ack')
                    captured.add(stage)
                time.sleep(.2)
            else:
                raise TimeoutError('Calibration timed out.')
        elif args.mode == 'workloads':
            results = []
            for count, mode in [(0, 'idle'), (0, 'animation'), (1, 'idle'), (1, 'animation'), (1, 'scroll'), (4, 'idle'), (4, 'animation'), (4, 'modal')]:
                start('webview-workload', ('--es', 'doroti_webview_count', str(count), '--es', 'doroti_webview_workload', mode))
                if count:
                    wait_native_frame()
                time.sleep(5)
                adb('shell', 'dumpsys', 'gfxinfo', PACKAGE, 'reset')
                time.sleep(4)
                name = f'{count}-{mode}'
                screenshot(name)
                (args.out / (name + '-gfx.txt')).write_text(adb('shell', 'dumpsys', 'gfxinfo', PACKAGE, 'framestats'), encoding='utf-8')
                (args.out / (name + '-memory.txt')).write_text(adb('shell', 'dumpsys', 'meminfo', PACKAGE), encoding='utf-8')
                pid = adb('shell', 'pidof', PACKAGE, check=False)
                if pid:
                    (args.out / (name + '-composition.txt')).write_text(adb('logcat', '-d', '--pid=' + pid, '-s', 'DorotiPlatformFrame:I'), encoding='utf-8')
                alive = bool(adb('shell', 'pidof', PACKAGE, check=False))
                results.append(dict(count=count, mode=mode, alive=alive))
                if not alive:
                    raise RuntimeError('Product exited during ' + name)
            (args.out / 'workloads.json').write_text(json.dumps(results, indent=2), encoding='utf-8')
        else:
            start()
            wait_native_frame()
            time.sleep(2)
            screenshot('live')
            time.sleep(1)
            screenshot('live-later')
    finally:
        (args.out / 'logcat.txt').write_text(adb('logcat', '-d', '-t', '6000'), encoding='utf-8')
        evidence = read('doroti-maui-evidence.json')
        if evidence:
            (args.out / 'host.json').write_text(evidence, encoding='utf-8')


if __name__ == '__main__':
    main()
