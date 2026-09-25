"""Installed MAUI keyboard fixture on an explicit Android device; use 1200s wrapper."""
import argparse
import json
from pathlib import Path
import re
import subprocess
import time
import xml.etree.ElementTree as ET

p = argparse.ArgumentParser()
p.add_argument('--serial', required=True)
args = p.parse_args()
OUT = Path(__file__).resolve().parents[3] / 'Doroti/artifacts/validation/maui-keyboard/android'
OUT.mkdir(parents=True, exist_ok=True)
package = 'dev.doroti.testbed'
def adb(*command):
    return subprocess.check_output(['adb', '-s', args.serial, *command], timeout=30).decode(errors='replace').strip()
def wait(check, seconds=15):
    until = time.monotonic() + seconds
    while time.monotonic() < until:
        value = check()
        if value: return value
        time.sleep(.15)
    raise TimeoutError('Android keyboard fixture state missing')
def events():
    lines = adb('logcat', '-d', '--pid=' + pid, '-s', 'DOTNET:I').splitlines()
    return [json.loads(line.split('DOROTI_KEYBOARD:', 1)[1]) for line in lines if 'DOROTI_KEYBOARD:' in line]
def tap(label):
    adb('shell', 'uiautomator', 'dump', '/sdcard/doroti-keyboard.xml')
    root = ET.fromstring(adb('shell', 'cat', '/sdcard/doroti-keyboard.xml'))
    node = next(n for n in root.iter('node') if n.get('package') == package
                and label in (n.get('text'), n.get('content-desc')) and n.get('clickable') == 'true')
    left, top, right, bottom = map(int, re.findall(r'\d+', node.get('bounds')))
    adb('shell', 'input', 'tap', str((left + right)//2), str((top + bottom)//2))
    time.sleep(.4)

activity = adb('shell', 'cmd', 'package', 'resolve-activity', '--brief', package).splitlines()[-1]
adb('shell', 'am', 'force-stop', package)
adb('shell', 'am', 'start', '-W', '-n', activity, '--es', 'doroti_testbed_mode', 'keyboard-input')
pid = wait(lambda: adb('shell', 'pidof', package))
hold = None
try:
    wait(lambda: events())
    tap('Keyboard focus')
    adb('shell', 'input', 'keyevent', '--longpress', '29')
    keys = wait(lambda: (v if len(v := [e for e in events() if e.get('physical') == 0x70004]) >= 3 else None))
    assert [e['type'] for e in keys] == ['KeyDownEvent', 'KeyRepeatEvent', 'KeyUpEvent'], keys
    adb('shell', 'input', 'keyevent', '160')
    assert any(e.get('physical') == 0x70058 and e.get('logical') == 0x20000020d for e in events())
    hold = subprocess.Popen(['adb', '-s', args.serial, 'shell', 'input', 'keyevent', '--duration', '3000', '60'], stdout=subprocess.DEVNULL)
    wait(lambda: any(e.get('physical') == 0x700e5 and e.get('type') == 'KeyDownEvent' for e in events()), 3)
    adb('shell', 'input', 'keyevent', 'KEYCODE_HOME')
    # Android may cancel the injected key with a native up before reporting
    # window focus loss. Both paths must leave exactly one terminal release.
    wait(lambda: any(e.get('physical') == 0x700e5 and e.get('type') == 'KeyUpEvent' for e in events()), 5)
    hold.wait(timeout=5); hold = None
    adb('shell', 'am', 'start', '-W', '-n', activity)
    assert adb('shell', 'pidof', package) == pid, 'App restarted instead of resuming'
    tap('Input')
    adb('shell', 'input', 'text', 'abc')
    wait(lambda: any(e.get('kind') == 'text' and e.get('value') == 'abc' for e in events()))
    adb('shell', 'input', 'keyevent', '67')
    wait(lambda: any(e.get('kind') == 'text' and e.get('value') == 'ab' for e in events()))
    released = [e for e in events() if e.get('physical') == 0x700e5 and e.get('type') == 'KeyUpEvent']
    assert len(released) == 1, released
    (OUT / 'events.json').write_text(json.dumps(events(), indent=2))
    (OUT / 'screen.png').write_bytes(subprocess.check_output(['adb', '-s', args.serial, 'exec-out', 'screencap', '-p'], timeout=30))
    crash = adb('logcat', '-d', '--pid=' + pid, '-s', 'AndroidRuntime:E')
    assert 'FATAL EXCEPTION' not in crash, crash
    result = dict(status='passed', device=args.serial, model=adb('shell', 'getprop', 'ro.product.model'),
                  transport='ADB input on physical device', repeat=True, keypadEnter=True,
                  focusLossRelease=True, releaseWasSynthesized=released[0]['synthesized'],
                  sameProcessResume=True, nativeTextAndBackspace=True,
                  physicalKeyboardAndHumanIme='notVerified')
    (OUT / 'result.json').write_text(json.dumps(result, indent=2))
    print(json.dumps(result))
finally:
    if hold: hold.wait(timeout=5)
    adb('shell', 'am', 'force-stop', package)
