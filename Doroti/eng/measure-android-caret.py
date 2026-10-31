"""Reproduce Testbed cursor-handle dragging on the 1080x2340 Galaxy.

Run with run-with-timeout.py --timeout 1200. Requires the shell-only
doroti-scroll.jar from eng/android/ScrollGesture.java. Injected touch is not
physical finger/display acceptance. UI queries run outside measured gestures.
"""
import argparse
import hashlib
import json
import os
import re
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
    parser.add_argument('--last-field', action='store_true', help='Isolate the second TextField from the first field selection-status listener; requires --without-native-views')
    parser.add_argument('--observe-accessibility', action='store_true', help='Separate diagnostic: keep a bounded UiAutomation event consumer attached during motion; does not change enabled services')
    parser.add_argument('--perfetto', action='store_true', help='Separate diagnostic: capture scheduler, GC and SurfaceView timing around motion')
    parser.add_argument('--components', action='store_true', help='Use the actual Components page, scroll to Text fields, and drag its full-width Filled field')
    parser.add_argument('--work-trace', action='store_true', help='Separate diagnostic: bounded framework contract capture during the gesture')
    args = parser.parse_args()
    if args.last_field and not args.without_native_views:
        parser.error('--last-field requires --without-native-views for the calibrated visible field')
    if args.components and (args.last_field or args.without_native_views):
        parser.error('--components uses the normal page, without input-fixture options')
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
        'serial': args.serial, 'sample': 'components' if args.components else 'input', 'allocationProfile': args.allocation,
        'text': text, 'field': 'last' if args.last_field else 'first', 'observeAccessibility': args.observe_accessibility,
        'nativeViews': not args.without_native_views and not args.components, 'perfetto': args.perfetto,
        'workTrace': args.work_trace, 'physicalTouchAccepted': False, 'trials': []}
    for trial in range(1, args.trials + 1):
        prefix = out / f'{args.label}-{trial}'
        call('shell', 'am', 'force-stop', package)
        call('shell', 'rm', '-f', '/sdcard/Android/data/' + package + '/files/doroti-scroll-profile.json')
        extras = ['--es', 'DOROTI_SCROLL_PROFILE', '1']
        if not args.components:
            extras += ['--es', 'doroti_sample', 'input']
        if args.allocation:
            extras += ['--es', 'DOROTI_ALLOCATION_PROFILE', '1', '--es', 'DOROTI_LAYOUT_PROFILE', '1', '--es', 'DOROTI_STAGE_TRACE', '1']
        call('shell', 'am', 'start', '-n', activity, *extras)
        time.sleep(10)
        if args.components:
            for step in range(12):
                setup = dump(Path(str(prefix) + f'.setup-{step}.xml'))
                fields = [n for n in setup if n.get('content-desc') == 'Filled' and n.get('class') == 'android.widget.EditText'
                    and int(re.findall(r'\d+', n['bounds'])[2]) - int(re.findall(r'\d+', n['bounds'])[0]) > 600]
                if fields:
                    left, top, right, bottom = map(int, re.findall(r'\d+', fields[0]['bounds']))
                    if 320 <= top <= 900:
                        call('shell', 'input', 'tap', '400', str(top+90))
                        break
                header = next((n for n in setup if 'Text inputs\nText fields' in n.get('text', '')), None)
                if header is not None and int(re.findall(r'\d+', header['bounds'])[1]) < 500:
                    call('shell', 'input', 'swipe', '12', '500', '12', '1500', '500')
                else:
                    call('shell', 'input', 'swipe', '12', '1850', '12', '450', '120')
                time.sleep(.8)
            else:
                raise RuntimeError('Components Filled field not reached through normal scrolling')
        elif args.last_field:
            call('shell', 'input', 'tap', '275', '885')
            time.sleep(1)
            setup = dump(prefix.with_suffix('.setup.xml'))
            field = next(n for n in setup if n.get('content-desc') == 'Framework input after native views')
            left, top, right, bottom = map(int, re.findall(r'\d+', field['bounds']))
            call('shell', 'input', 'tap', str((left+right)//2), str((top+bottom)//2))
        else:
            call('shell', 'input', 'tap', '400', '580')
        call('shell', 'input', 'text', text)
        time.sleep(2)
        if args.without_native_views and not args.last_field:
            call('shell', 'input', 'tap', '275', '885')
            time.sleep(1)
        nodes = dump(prefix.with_suffix('.before.xml'))
        assert any(n['text'] == text and n['class'] == 'android.widget.EditText' for n in nodes), 'Framework field setup failed'
        text_y, handle_y = 550, 625
        if args.components:
            field = next(n for n in nodes if n.get('content-desc') == 'Filled' and n.get('class') == 'android.widget.EditText'
                and int(re.findall(r'\d+', n['bounds'])[2]) - int(re.findall(r'\d+', n['bounds'])[0]) > 600)
            _, top, _, bottom = map(int, re.findall(r'\d+', field['bounds']))
            text_y, handle_y = top+100, top+172
        elif args.last_field:
            field = next(n for n in nodes if n.get('content-desc') == 'Framework input after native views')
            _, top, _, bottom = map(int, re.findall(r'\d+', field['bounds']))
            text_y, handle_y = top+80, top+155
        call('shell', 'input', 'tap', '350', str(text_y))
        time.sleep(.3)
        state = {name: call('shell', *command) for name, command in {
            'battery': ['dumpsys', 'battery'], 'display': ['dumpsys', 'display'],
            'accessibility': ['settings', 'get', 'secure', 'enabled_accessibility_services'],
            'lowPower': ['settings', 'get', 'global', 'low_power']}.items()}
        prefix.with_suffix('.state.json').write_text(json.dumps(state), encoding='utf-8')
        observer = None
        if args.observe_accessibility:
            # Remote timeout owns cleanup even if this test exits early. Discard
            # global event payloads; record only our app's exported diagnostics.
            observer = subprocess.Popen([adb, '-s', args.serial, 'shell', 'timeout', '12', 'uiautomator', 'events'],
                stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
            time.sleep(.5)
            assert observer.poll() is None, 'Bounded accessibility observer failed to start'
        call('logcat', '-c')
        if args.work_trace:
            call('shell', 'am', 'start', '-n', activity, '--es', 'DOROTI_WORK_TRACE_START', '1')
            time.sleep(.3)
        recorder = None
        if args.perfetto:
            config = Path(__file__).parent / 'android/caret.perfetto.pbtxt'
            recorder = subprocess.Popen([adb, '-s', args.serial, 'shell', 'perfetto', '--txt', '-c',
                '-', '-o', '/data/misc/perfetto-traces/doroti-caret.perfetto-trace'],
                stdin=subprocess.PIPE, stdout=subprocess.DEVNULL)
            recorder.stdin.write(config.read_bytes())
            recorder.stdin.close()
            time.sleep(1)
            assert recorder.poll() is None, 'Perfetto exited before the gesture'
        path = f'348,{handle_y},0 950,{handle_y},800 150,{handle_y if args.last_field else 695},1600 850,{handle_y},2400'
        if args.components:
            path = f'348,{handle_y},0 800,{handle_y},800 240,{handle_y},1600 750,{handle_y},2400'
        injection = call('shell', 'CLASSPATH=/data/local/tmp/doroti-scroll.jar app_process /system/bin ScrollGesture ' + path)
        time.sleep(.3)
        call('shell', 'am', 'start', '-n', activity, '--es', 'DOROTI_SCROLL_EXPORT', '1')
        # The intent can return before serialization finishes. The current
        # runner publishes atomically; bounded retries also support old APKs.
        for attempt in range(30):
            time.sleep(.2)
            try:
                call('pull', '/sdcard/Android/data/' + package + '/files/doroti-scroll-profile.json', str(prefix.with_suffix('.json')))
                profile = json.loads(prefix.with_suffix('.json').read_text(encoding='utf-8'))
                break
            except (subprocess.CalledProcessError, json.JSONDecodeError):
                if attempt == 29:
                    raise
        if recorder is not None:
            assert recorder.wait(timeout=30) == 0, 'Perfetto capture failed'
            call('pull', '/data/misc/perfetto-traces/doroti-caret.perfetto-trace', str(prefix.with_suffix('.perfetto-trace')))
        if observer is not None:
            assert observer.wait(timeout=15) in (0, 124, 137, 143), 'Accessibility observer failed'
        nodes = dump(prefix.with_suffix('.after.xml'))
        call('shell', 'screencap', '-p', '/sdcard/doroti-caret.png')
        call('pull', '/sdcard/doroti-caret.png', str(prefix.with_suffix('.png')))
        log = call('logcat', '-d', '-v', 'monotonic')
        prefix.with_suffix('.log').write_text(log, encoding='utf-8')
        result = {'trial': trial, 'injection': injection, 'path': path, 'selection': [n['text'] for n in nodes if n['text'].startswith('Selection ')],
            'textUnchanged': any(n['text'] == text for n in nodes), 'fatalException': 'FATAL EXCEPTION' in log,
            'renderErrorLines': sum(' E DorotiMauiFailure:' in line or ' E DorotiGraphite:' in line for line in log.splitlines()),
            'nativeMagnifier': profile.get('magnifier')}
        if args.components:
            result['sharedFilledValues'] = sum(n.get('class') == 'android.widget.EditText' and n['text'] == text for n in nodes)
            result['imeShowCallsInLog'] = log.count('showSoftInput(View,I)')
            assert result['sharedFilledValues'] == 3, 'All three controller peers must retain the input text'
        if profile.get('editing'):
            result['editing'] = profile['editing']
            assert profile['editing']['length'] == len(text)
            assert 0 <= profile['editing']['base'] == profile['editing']['extent'] <= len(text)
            if not args.last_field and not args.components:
                match = re.search(r'Selection (\d+):(\d+)', result['selection'][0])
                assert match and profile['editing']['base'] == int(match[1]) and profile['editing']['extent'] == int(match[2])
        manifest['trials'].append(result)
        print(json.dumps(result), flush=True)
        assert result['textUnchanged'] and not result['fatalException'] and result['renderErrorLines'] == 0
        if result['nativeMagnifier'] is not None:
            magnifier = result['nativeMagnifier']
            assert magnifier['updates'] > 20 and magnifier['created'] == magnifier['dismissed'] > 0, magnifier
    (out / (args.label + '-manifest.json')).write_text(json.dumps(manifest, indent=2), encoding='utf-8')


if __name__ == '__main__':
    main()
