"""Reproduce Testbed cursor-handle dragging on the 1080x2340 Galaxy.

Run with run-with-timeout.py --timeout 1200. Requires the shell-only
doroti-scroll.jar from eng/android/ScrollGesture.java. Injected touch is not
physical finger/display acceptance. UI queries run outside measured gestures.
"""
import argparse
import hashlib
import json
import os
from pathlib import Path
import subprocess
import time
import xml.etree.ElementTree as ET


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--serial', required=True)
    parser.add_argument('--label', required=True)
    parser.add_argument('--trials', type=int, choices=range(1, 4), default=3)
    parser.add_argument('--allocation', action='store_true')
    parser.add_argument('--without-native-views', action='store_true')
    args = parser.parse_args()
    out = Path('temp/galaxy-caret')
    out.mkdir(parents=True, exist_ok=True)
    adb = str(Path(os.environ['LOCALAPPDATA']) / 'Android/Sdk/platform-tools/adb.exe')
    package = 'dev.doroti.testbed'
    activity = package + '/crc64c80c495bd333b69c.MainActivity'
    text = 'The_quick_brown_fox_jumps_over_the_lazy_dog._Cursor_movement_should_stay_smooth.'

    def call(*command):
        return subprocess.check_output([adb, '-s', args.serial, *command],
            text=True, encoding='utf-8', errors='replace', timeout=60).strip()

    def dump(path):
        call('shell', 'uiautomator', 'dump', '/sdcard/doroti-caret.xml')
        call('pull', '/sdcard/doroti-caret.xml', str(path))
        return [n.attrib for n in ET.parse(path).iter('node') if n.get('package') == package]

    assert '1080x2340' in call('shell', 'wm', 'size'), 'This gesture is calibrated for the recorded Galaxy dimensions'
    apk = call('shell', 'pm', 'path', package).splitlines()[0].removeprefix('package:')
    local_apk = out / (args.label + '.apk')
    call('pull', apk, str(local_apk))
    manifest = {'apkSha256': hashlib.sha256(local_apk.read_bytes()).hexdigest(),
        'serial': args.serial, 'sample': 'input', 'allocationProfile': args.allocation,
        'text': text, 'nativeViews': not args.without_native_views, 'physicalTouchAccepted': False, 'trials': []}
    for trial in range(1, args.trials + 1):
        prefix = out / f'{args.label}-{trial}'
        call('shell', 'am', 'force-stop', package)
        call('shell', 'rm', '-f', '/sdcard/Android/data/' + package + '/files/doroti-scroll-profile.json')
        extras = ['--es', 'doroti_sample', 'input', '--es', 'DOROTI_SCROLL_PROFILE', '1']
        if args.allocation:
            extras += ['--es', 'DOROTI_ALLOCATION_PROFILE', '1', '--es', 'DOROTI_LAYOUT_PROFILE', '1', '--es', 'DOROTI_STAGE_TRACE', '1']
        call('shell', 'am', 'start', '-n', activity, *extras)
        time.sleep(10)
        call('shell', 'input', 'tap', '400', '580')
        call('shell', 'input', 'text', text)
        time.sleep(2)
        if args.without_native_views:
            call('shell', 'input', 'tap', '275', '885')
            time.sleep(1)
        nodes = dump(prefix.with_suffix('.before.xml'))
        assert any(n['text'] == text and n['class'] == 'android.widget.EditText' for n in nodes), 'Framework field setup failed'
        call('shell', 'input', 'tap', '350', '550')
        time.sleep(.3)
        state = {name: call('shell', *command) for name, command in {
            'battery': ['dumpsys', 'battery'], 'display': ['dumpsys', 'display'],
            'accessibility': ['settings', 'get', 'secure', 'enabled_accessibility_services'],
            'lowPower': ['settings', 'get', 'global', 'low_power']}.items()}
        prefix.with_suffix('.state.json').write_text(json.dumps(state), encoding='utf-8')
        call('logcat', '-c')
        injection = call('shell', 'CLASSPATH=/data/local/tmp/doroti-scroll.jar app_process /system/bin ScrollGesture '
            '348,625,0 950,625,800 150,695,1600 850,625,2400')
        time.sleep(.3)
        call('shell', 'am', 'start', '-n', activity, '--es', 'DOROTI_SCROLL_EXPORT', '1')
        time.sleep(.5)
        call('pull', '/sdcard/Android/data/' + package + '/files/doroti-scroll-profile.json', str(prefix.with_suffix('.json')))
        profile = json.loads(prefix.with_suffix('.json').read_text(encoding='utf-8'))
        nodes = dump(prefix.with_suffix('.after.xml'))
        call('shell', 'screencap', '-p', '/sdcard/doroti-caret.png')
        call('pull', '/sdcard/doroti-caret.png', str(prefix.with_suffix('.png')))
        log = call('logcat', '-d', '-v', 'monotonic')
        prefix.with_suffix('.log').write_text(log, encoding='utf-8')
        result = {'trial': trial, 'injection': injection, 'selection': [n['text'] for n in nodes if n['text'].startswith('Selection ')],
            'textUnchanged': any(n['text'] == text for n in nodes), 'fatalException': 'FATAL EXCEPTION' in log,
            'renderErrorLines': sum(' E DorotiMauiFailure:' in line or ' E DorotiGraphite:' in line for line in log.splitlines()),
            'nativeMagnifier': profile.get('magnifier')}
        manifest['trials'].append(result)
        print(json.dumps(result), flush=True)
        assert result['textUnchanged'] and not result['fatalException'] and result['renderErrorLines'] == 0
        if result['nativeMagnifier'] is not None:
            magnifier = result['nativeMagnifier']
            assert magnifier['updates'] > 20 and magnifier['created'] == magnifier['dismissed'] > 0, magnifier
    (out / (args.label + '-manifest.json')).write_text(json.dumps(manifest, indent=2), encoding='utf-8')


if __name__ == '__main__':
    main()
