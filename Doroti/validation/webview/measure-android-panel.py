"""Two bounded drags in the open Material WebView sample, with platform-copy costs.

Launch with DOROTI_MAUI_EVIDENCE=1, select local HTML, then inspect the screenshot
and pass the current header coordinates. Run under validation/run-with-timeout.py.
Window gfxinfo includes independently rendered WebView/blur frames; it is not a
panel frame rate or input-to-display measurement.
"""
import argparse
import json
from pathlib import Path
import re
import statistics
import subprocess
import time
import xml.etree.ElementTree as ET

p = argparse.ArgumentParser()
p.add_argument('--serial', required=True)
p.add_argument('--out', type=Path, required=True)
p.add_argument('--x', type=int)
p.add_argument('--y', type=int)
p.add_argument('--dy', type=int, default=600)
a = p.parse_args()
a.out.mkdir(parents=True, exist_ok=False)

def adb(*args):
    return subprocess.check_output(['adb', '-s', a.serial, *map(str, args)], timeout=1200)

def capture(name):
    (a.out / (name + '.png')).write_bytes(adb('exec-out', 'screencap', '-p'))

def header(name):
    adb('shell', 'uiautomator', 'dump', '/sdcard/doroti-panel-measure.xml')
    xml = adb('shell', 'cat', '/sdcard/doroti-panel-measure.xml')
    node = next((n for n in ET.fromstring(xml).iter('node')
                 if n.get('package') == 'dev.doroti.testbed' and n.get('text') == 'Floating panel'), None)
    if node is None:
        # Do not persist unrelated system UI/notification text when the app loses foreground.
        raise RuntimeError('Panel header unavailable: app/panel may be hidden or system UI may be open.')
    (a.out / (name + '.xml')).write_bytes(xml)
    return list(map(int, re.findall(r'\d+', node.get('bounds'))))

pid = adb('shell', 'pidof', 'dev.doroti.testbed').decode().strip()
def log():
    return adb('logcat', '-d', '--pid=' + pid, '-s', 'DorotiPlatformFrame:I', 'DorotiInputTiming:I', 'DorotiPlatformTiming:I').decode(errors='replace')

capture('start')
start_header = header('start')
x = a.x if a.x is not None else (start_header[0] + start_header[2]) // 2
y = a.y if a.y is not None else (start_header[1] + start_header[3]) // 2
adb('shell', 'dumpsys', 'gfxinfo', 'dev.doroti.testbed', 'reset')
before = log()
adb('shell', 'input', 'swipe', x, y, x, y + a.dy, 2500)
time.sleep(.3)
capture('moved')
moved_header = header('moved')
# Requery the moved header: the second gesture must start at its displayed position.
mx, my = (moved_header[0] + moved_header[2]) // 2, (moved_header[1] + moved_header[3]) // 2
adb('shell', 'input', 'swipe', mx, my, mx, y, 2500)
time.sleep(.3)
after = log()
capture('end')
end_header = header('end')
entries = after[len(before):] if after.startswith(before) else after
(a.out / 'platform.txt').write_text(entries)
(a.out / 'gfxinfo.txt').write_bytes(adb('shell', 'dumpsys', 'gfxinfo', 'dev.doroti.testbed'))
times = [float(v) for v in re.findall(r'uiMs=([\d.]+)', entries)]
if not times:
    raise RuntimeError('No platform commits; verify the header position and evidence launch extra.')
counter = lambda s: [int(v) for v in re.findall(r'readbackBytes=(\d+)', s)]
report = dict(serial=a.serial, pid=pid, drags=2, durationMs=2500, commits=len(times),
              uiMedianMs=statistics.median(times), uiMaxMs=max(times),
              readbackBytes=counter(after)[-1] - counter(before)[-1],
              changedSlices=sum(map(int, re.findall(r'changed=(\d+)', entries))),
              cachedSlices=sum(map(int, re.findall(r'cached=(\d+)', entries))),
              headerBounds=[start_header, moved_header, end_header],
              movesInRequestedDirection=(moved_header[1] - start_header[1]) * (1 if a.dy > 0 else -1) > abs(a.dy) / 2,
              dragsAgainAfterMoving=(moved_header[1] - end_header[1]) * (1 if a.dy > 0 else -1) > abs(a.dy) / 2,
              physicalInput='notVerified', panelPresentationLatency='notMeasured')
input_events = re.findall(r'action=(\w+) eventMs=(\d+) queuedMs=(\d+) history=(\d+) x=([\d.-]+) y=([\d.-]+) dispatchMs=([\d.]+)', entries)
if input_events:
    moves = [e for e in input_events if e[0] == 'Move']
    report['input'] = dict(
        actions={name: sum(e[0] == name for e in input_events) for name in ('Down', 'Move', 'Up', 'Cancel')},
        moveQueueMedianMs=statistics.median(int(e[2]) for e in moves) if moves else None,
        moveQueueMaxMs=max((int(e[2]) for e in moves), default=None),
        moveDispatchMedianMs=statistics.median(float(e[6]) for e in moves) if moves else None,
        moveDispatchMaxMs=max((float(e[6]) for e in moves), default=None),
        historicalSamples=sum(int(e[3]) for e in input_events))
    actions = report['input']['actions']
    report['isolatedScriptInput'] = actions['Down'] == 2 and actions['Up'] == 2 and actions['Cancel'] == 0
(a.out / 'result.json').write_text(json.dumps(report, indent=2) + '\n')
print(json.dumps(report, indent=2))
raise SystemExit(0 if report['movesInRequestedDirection'] and report['dragsAgainAfterMoving'] and
                 report.get('isolatedScriptInput', True) else 1)
