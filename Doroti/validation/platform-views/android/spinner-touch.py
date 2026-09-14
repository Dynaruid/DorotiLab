"""Checks that the visible loading spinner passes a tap through to its native button."""
import argparse
import json
from pathlib import Path
import re
import subprocess
import time
import xml.etree.ElementTree as ET

parser = argparse.ArgumentParser()
parser.add_argument('--serial', required=True)
parser.add_argument('--output', type=Path, required=True)
args = parser.parse_args()
args.output.mkdir(parents=True, exist_ok=False)
commands = []


def adb(*parts):
    command = ['adb', '-s', args.serial, *map(str, parts)]
    completed = subprocess.run(command, capture_output=True, timeout=1200)
    commands.append(dict(command=command, exitCode=completed.returncode))
    (args.output / 'commands.json').write_text(json.dumps(commands, indent=2), encoding='utf-8')
    completed.check_returncode()
    return completed.stdout


def capture(label):
    remote = '/sdcard/doroti-spinner-' + str(time.time_ns()) + '.xml'
    adb('shell', 'uiautomator', 'dump', remote)
    xml = adb('shell', 'cat', remote)
    adb('shell', 'rm', remote)
    (args.output / (label + '.xml')).write_bytes(xml)
    (args.output / (label + '.png')).write_bytes(adb('exec-out', 'screencap', '-p'))
    return list(ET.fromstring(xml).iter('node'))


result = dict(status='FAIL', serial=args.serial, physicalInput=False)
try:
    before = capture('before')
    assert any('Native view loading' in n.get('content-desc', '') or 'Native view loading' in n.get('text', '') for n in before)
    button = next(n for n in before if n.get('class') == 'android.widget.Button' and n.get('content-desc', '').startswith('doroti-platform-view-'))
    left, top, right, bottom = map(int, re.findall(r'\d+', button.get('bounds')))
    scale = (right - left) / 220
    point = [round(left + 48 * scale), round(top + 48 * scale)]
    count = int(re.search(r'\d+', button.get('text')).group()) if 'Native clicks:' in button.get('text') else 0
    adb('shell', 'input', 'tap', *point)
    time.sleep(.4)
    after = capture('after')
    assert any(n.get('text') == 'Native clicks: ' + str(count + 1) for n in after), 'Spinner intercepted or duplicated the native click'
    result.update(status='PASS', nativeClicksBefore=count, nativeClicksAfter=count + 1, point=point)
finally:
    (args.output / 'result.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
print(json.dumps(result), flush=True)
