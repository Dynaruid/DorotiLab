"""Galaxy fixture input/IME/shield/lifecycle checks. Coordinates require the recorded 1080x2340 configuration."""
import argparse
from pathlib import Path
import re
import subprocess
import time
import xml.etree.ElementTree as ET

parser = argparse.ArgumentParser()
parser.add_argument('--serial', required=True)
parser.add_argument('--out', type=Path, required=True)
args = parser.parse_args()
args.out.mkdir(parents=True, exist_ok=True)
package = 'dev.doroti.testbed'
lines = []


def adb(*command, binary=False):
    result = subprocess.run(['adb', '-s', args.serial, *command], capture_output=True, timeout=30, check=True)
    return result.stdout if binary else result.stdout.decode(errors='replace').strip()


def capture(name):
    (args.out / (name + '.png')).write_bytes(adb('exec-out', 'screencap', '-p', binary=True))


def nodes():
    adb('shell', 'uiautomator', 'dump', '/sdcard/doroti-input-ui.xml')
    return ET.fromstring(adb('shell', 'cat', '/sdcard/doroti-input-ui.xml'))


def check(value, message):
    if not value:
        raise AssertionError(message)
    lines.append('PASS ' + message)


def tap(x, y):
    adb('shell', 'input', 'tap', str(x), str(y))
    time.sleep(.5)


def tap_text(label):
    target = next(n for n in nodes().iter('node') if n.get('text') == label and n.get('clickable') == 'true')
    left, top, right, bottom = map(int, re.findall(r'\d+', target.get('bounds')))
    if right <= left or bottom <= top:
        raise AssertionError('Control is not visible: ' + label)
    tap((left + right) // 2, (top + bottom) // 2)


def trace():
    return adb('logcat', '-d', '--pid=' + pid, '-s', 'DorotiPlatformView:I')


rotation = adb('shell', 'settings', 'get', 'system', 'user_rotation')
auto = adb('shell', 'settings', 'get', 'system', 'accelerometer_rotation')
check('1080x2340' in adb('shell', 'wm', 'size'), 'recorded physical dimensions')
activity = adb('shell', 'cmd', 'package', 'resolve-activity', '--brief', package).splitlines()[-1]
try:
    adb('shell', 'settings', 'put', 'system', 'accelerometer_rotation', '0')
    adb('shell', 'settings', 'put', 'system', 'user_rotation', '0')
    adb('shell', 'am', 'force-stop', package)
    adb('shell', 'am', 'start', '-W', '-n', activity, '--es', 'doroti_testbed_mode', 'platform-effects', '--es', 'DOROTI_MAUI_EVIDENCE', '1')
    pid = adb('shell', 'pidof', package)
    deadline = time.monotonic() + 40
    while time.monotonic() < deadline:
        if re.search(r'readbackFrames=[1-9]', adb('logcat', '-d', '--pid=' + pid, '-s', 'DorotiPlatformFrame:I')):
            break
        time.sleep(.5)
    else:
        raise TimeoutError('No native product frame')
    time.sleep(1)
    capture('initial')
    tap_text('Doroti button')
    check(any(n.get('text') == 'Foreground taps: 1' for n in nodes().iter('node')), 'foreground button receives one injected tap')
    before = trace().count(' action=Down')
    tap(600, 900)
    check(trace().count(' action=Down') == before + 1, 'effect passes native input through once')
    tap_text('Pass through input')
    before = trace().count(' action=Down')
    tap(600, 900)
    check(trace().count(' action=Down') == before, 'committed modal shield blocks native dispatch')
    capture('modal')
    tap_text('Block input')

    tap(280, 600)
    time.sleep(.5)
    adb('shell', 'input', 'keycombination', '113', '29')
    # Observed Samsung Korean keyboard in the recorded portrait configuration.
    for x, y in [(537, 1826), (861, 1826), (223, 1826), (383, 1679), (857, 1967), (434, 1826)]:
        tap(x, y)
    capture('korean-composing')
    check(any(n.get('text') == '한글' for n in nodes().iter('node')), 'Samsung IME composes Korean in native WebView')
    adb('shell', 'input', 'keyevent', 'KEYCODE_BACK')
    time.sleep(.7)
    tap(280, 1760)
    adb('shell', 'input', 'text', 'doroti_focus')
    time.sleep(.5)
    adb('shell', 'input', 'keyevent', 'KEYCODE_BACK')
    time.sleep(.7)
    capture('focus-roundtrip')
    current = nodes()
    check(any(n.get('text') == 'doroti_focus' for n in current.iter('node')), 'native to Doroti text focus and typing')
    check(any(n.get('text') == '한글' for n in current.iter('node')), 'native text survives focus transfer')

    before = trace().count('create handle=')
    for value, name in [('1', 'landscape'), ('3', 'reverse-landscape'), ('0', 'portrait-restored')]:
        adb('shell', 'settings', 'put', 'system', 'user_rotation', value)
        time.sleep(1.5)
        capture(name)
    adb('shell', 'input', 'keyevent', 'KEYCODE_HOME')
    time.sleep(1)
    adb('shell', 'am', 'start', '-W', '-n', activity)
    time.sleep(1.5)
    capture('resumed')
    check(trace().count('create handle=') == before, 'rotation and Home/resume retain native identity')
    check(any(n.get('text') == '한글' for n in nodes().iter('node')), 'rotation and resume retain native document text')
    # The existing Doroti text client may reopen its keyboard on resume.
    # Use the observed keyboard hide affordance, never Back on a closed keyboard.
    if 'mInputShown=true' in adb('shell', 'dumpsys', 'input_method'):
        tap(970, 2268)
        time.sleep(.5)
    adb('shell', 'input', 'swipe', '230', '1410', '230', '850', '450')
    time.sleep(.5)
    capture('native-scroll')
    tap_text('Second WebView')
    check(trace().count('create handle=') == before + 1, 'second WebView uses a distinct native instance')
    capture('two-webviews')
    tap_text('Move effect')
    check(trace().count('create handle=') == before + 1, 'moving effect retains native instances')
    capture('moved')
    tap_text('Dispose WebViews')
    time.sleep(1)
    capture('disposed')
    tap_text('Create WebViews')
    time.sleep(1)
    check(trace().count('create handle=') == before + 3, 'explicit disposal and recreation create two replacement instances')
    capture('recreated')
except Exception as error:
    lines.append('FAIL ' + repr(error))
    capture('failed')
    raise
finally:
    adb('shell', 'settings', 'put', 'system', 'user_rotation', rotation)
    adb('shell', 'settings', 'put', 'system', 'accelerometer_rotation', auto)
    (args.out / 'results.txt').write_text('\n'.join(lines), encoding='utf-8')
    (args.out / 'logcat.txt').write_text(adb('logcat', '-d', '--pid=' + pid), encoding='utf-8')
    print('\n'.join(lines))
