"""Bounded Galaxy scroll comparison. Run under run-with-timeout.py --timeout 1200.

ADB gestures are automation, never physical-input acceptance. No device settings
are changed. Keep builds, UI hierarchy inspection and screen recording outside
the measured windows. Ring export happens only after all gestures finish.
"""
import argparse
import hashlib
import json
import os
from pathlib import Path
import re
import shlex
import subprocess
import time


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--serial', required=True)
    parser.add_argument('--label', required=True)
    parser.add_argument('--output', type=Path, default=Path('temp/galaxy-scroll'))
    parser.add_argument('--trials', type=int, choices=range(1, 4), default=3)
    parser.add_argument('--profile', choices=['ring', 'log', 'off', 'allocation'], default='ring')
    parser.add_argument('--continuous', action='store_true', help='Use deployed /data/local/tmp/doroti-scroll.jar; one continuous reversing drag')
    parser.add_argument('--perfetto', action='store_true', help='Separate diagnostic run with scheduling/FrameTimeline trace')
    args = parser.parse_args()
    args.output.mkdir(parents=True, exist_ok=True)
    adb = Path(os.environ['LOCALAPPDATA']) / 'Android/Sdk/platform-tools/adb.exe'
    package = 'dev.doroti.testbed'
    component = package + '/crc64c80c495bd333b69c.MainActivity'

    def call(*cmd):
        return subprocess.check_output([str(adb), '-s', args.serial, *cmd],
                                      text=True, encoding='utf-8', errors='replace', timeout=60)

    apk = call('shell', 'pm', 'path', package).strip().removeprefix('package:')
    local_apk = args.output / f'{args.label}-installed.apk'
    call('pull', apk, str(local_apk))
    manifest = {'serial': args.serial, 'apkSha256': hashlib.sha256(local_apk.read_bytes()).hexdigest(),
                'profile': args.profile, 'input': 'adb-shell-continuous-touch' if args.continuous else 'adb-input-swipe',
                'presentObserved': False, 'trials': []}
    for trial in range(1, args.trials + 1):
        prefix = args.output / f'{args.label}-{trial}'
        call('shell', 'am', 'force-stop', package)
        if args.profile in ('ring', 'allocation'):
            # This single file is owned by this diagnostic. A failed export
            # must fail the run instead of reusing a previous process's ring.
            call('shell', 'rm', '-f', '/sdcard/Android/data/' + package + '/files/doroti-scroll-profile.json')
        call('logcat', '-c')
        extras = []
        flags = {'log': ['DOROTI_PLATFORM_FRAME_PROFILE', 'DOROTI_INPUT_TIMING'],
                 'ring': ['DOROTI_SCROLL_PROFILE'], 'off': [],
                 'allocation': ['DOROTI_SCROLL_PROFILE', 'DOROTI_ALLOCATION_PROFILE', 'DOROTI_LAYOUT_PROFILE', 'DOROTI_STAGE_TRACE']}
        for flag in flags[args.profile]:
            extras += ['--es', flag, '1']
        call('shell', 'am', 'start', '-n', component, *extras)
        time.sleep(12)
        state = {key: call('shell', *cmd) for key, cmd in {
            'battery': ['dumpsys', 'battery'], 'thermal': ['dumpsys', 'thermalservice'],
            'display': ['dumpsys', 'display'], 'accessibility': ['settings', 'get', 'secure', 'enabled_accessibility_services'],
            'lowPower': ['settings', 'get', 'global', 'low_power']}.items()}
        prefix.with_suffix('.state.json').write_text(json.dumps(state, ensure_ascii=False), encoding='utf-8')
        # Restore a middle-of-list starting position outside the measurement.
        call('shell', 'input', 'swipe', '950', '1900', '950', '650', '650')
        time.sleep(2)
        call('logcat', '-c')
        recorder = None
        if args.perfetto:
            config = Path(__file__).parent / 'android/scroll.perfetto.pbtxt'
            recorder = subprocess.Popen([str(adb), '-s', args.serial, 'shell', 'perfetto', '--txt', '-c',
                '-', '-o', '/data/misc/perfetto-traces/doroti-scroll.perfetto-trace'],
                stdin=subprocess.PIPE, stdout=subprocess.DEVNULL)
            recorder.stdin.write(config.read_bytes())
            recorder.stdin.close()
            time.sleep(1)
            if recorder.poll() is not None:
                raise RuntimeError('Perfetto exited before the measured gesture')
        gestures = []
        # Medium upward, fast downward, slow upward. One of each per trial.
        patterns = [(1850, 800, 350), (750, 1650, 120), (1800, 1000, 650)]
        if args.continuous:
            patterns = [(1800, 700, 2650)]
        for start, end, duration in patterns:
            start_uptime = call('shell', 'cat', '/proc/uptime').split()[0]
            if args.continuous:
                injection = call('shell', 'CLASSPATH=/data/local/tmp/doroti-scroll.jar app_process /system/bin ScrollGesture '
                    '950,1800,0 950,700,900 950,1750,1800 950,900,2550 950,700,2650')
            else:
                injection = call('shell', 'input', 'swipe', '950', str(start), '950', str(end), str(duration))
            time.sleep(2)
            gestures.append({'startBootTimeSeconds': start_uptime, 'x': 950, 'fromY': start, 'toY': end, 'durationMs': duration,
                             'continuous': args.continuous, 'injection': injection.strip()})
        log = call('logcat', '-d', '-v', 'monotonic')
        prefix.with_suffix('.log').write_text(log, encoding='utf-8')
        if args.profile in ('ring', 'allocation'):
            call('shell', 'am', 'start', '-n', component, '--es', 'DOROTI_SCROLL_EXPORT', '1')
            time.sleep(1)
            call('pull', '/sdcard/Android/data/' + package + '/files/doroti-scroll-profile.json', str(prefix.with_suffix('.json')))
        # Query only after motion, and retain raw layer names for provenance.
        layers = call('shell', 'dumpsys', 'SurfaceFlinger', '--list')
        prefix.with_suffix('.layers.txt').write_text(layers, encoding='utf-8')
        for layer in layers.splitlines():
            if 'SurfaceView[' + package in layer and '(BLAST)' in layer:
                name = re.sub(r'^RequestedLayerState\{', '', layer).split(' parentId=')[0]
                latency = call('shell', 'dumpsys SurfaceFlinger --latency ' + shlex.quote(name))
                prefix.with_suffix('.latency.txt').write_text(latency, encoding='utf-8')
                break
        call('shell', 'screencap', '-p', '/sdcard/doroti-scroll.png')
        call('pull', '/sdcard/doroti-scroll.png', str(prefix.with_suffix('.png')))
        if recorder is not None:
            if recorder.wait(timeout=30) != 0:
                raise RuntimeError('Perfetto capture failed')
            call('pull', '/data/misc/perfetto-traces/doroti-scroll.perfetto-trace', str(prefix.with_suffix('.perfetto-trace')))
        result = {'trial': trial, 'gestures': gestures, 'fatal': 'FATAL EXCEPTION' in log}
        manifest['trials'].append(result)
        print(json.dumps({'label': args.label, **result}), flush=True)
    (args.output / f'{args.label}-manifest.json').write_text(json.dumps(manifest, indent=2), encoding='utf-8')


if __name__ == '__main__':
    main()
