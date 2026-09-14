"""Bounded real Material-page scroll workload; accepted frame timing is not scanout FPS."""
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
p.add_argument('--output', type=Path, required=True)
p.add_argument('--raster-mode', choices=['optimized', 'full-regions'], default='optimized')
a = p.parse_args()
a.output.mkdir(parents=True, exist_ok=False)
commands = []


def adb(*parts):
    cmd = ['adb', '-s', a.serial, *map(str, parts)]
    r = subprocess.run(cmd, capture_output=True, timeout=1200)
    commands.append(dict(command=cmd, exitCode=r.returncode))
    (a.output/'commands.json').write_text(json.dumps(commands, indent=2))
    r.check_returncode()
    return r.stdout


def capture(name):
    remote = '/sdcard/doroti-scroll.xml'
    for _ in range(3):
        if b'dumped to:' in adb('shell', 'uiautomator', 'dump', remote): break
        time.sleep(1)
    xml = adb('shell', 'cat', remote)
    (a.output/(name+'.xml')).write_bytes(xml)
    (a.output/(name+'.png')).write_bytes(adb('exec-out', 'screencap', '-p'))
    return list(ET.fromstring(xml).iter('node'))


def bounds(n): return list(map(int, re.findall(r'\d+', n.get('bounds'))))
def button(nodes): return next(n for n in nodes if n.get('class') == 'android.widget.Button'
                              and n.get('content-desc', '').startswith('doroti-platform-view-'))
def dist(values):
    values = sorted(values)
    return dict(mean=statistics.mean(values), p50=values[round((len(values)-1)*.5)],
                p95=values[round((len(values)-1)*.95)], maximum=max(values))


result = dict(status='FAIL', serial=a.serial, rasterMode=a.raster_mode, page='Material/Platform views', physicalInput=False, displayScanout='notVerified')
try:
    adb('shell', 'am', 'force-stop', 'dev.doroti.testbed')
    adb('shell', 'am', 'start', '-W', '-n', 'dev.doroti.testbed/crc64c80c495bd333b69c.MainActivity',
        '--es', 'doroti_testbed_mode', 'sample', '--es', 'DOROTI_MAUI_EVIDENCE', '1',
        '--es', 'doroti_platform_view_raster_mode', a.raster_mode,
        '--es', 'DOROTI_PLATFORM_FRAME_PROFILE', '1')
    time.sleep(2)
    nodes = capture('start')
    tab = next(n for n in nodes if n.get('text', '').startswith('Platform views') and n.get('clickable') == 'true')
    l,t,r,b = bounds(tab)
    adb('shell', 'input', 'tap', (l+r)//2, (t+b)//2)
    time.sleep(1)
    nodes = capture('before')
    original = button(nodes)
    l,t,r,b = bounds(original)
    scale = (r-l)/220
    # Start on page text, outside the nested horizontal example scroller.
    x = round(l+100*scale)
    y = round(t-130*scale)
    delta = round(90*scale)
    windows = []
    for i in range(6):
        y1,y2 = (y,y-delta) if i % 2 == 0 else (y-delta,y)
        start = float(adb('shell', 'date', '+%s.%N'))
        adb('shell', 'input', 'swipe', x,y1,x,y2,900)
        end = float(adb('shell', 'date', '+%s.%N'))
        windows.append((start,end))
        nodes = capture('swipe-'+str(i))
        current = button(nodes)
        assert current.get('content-desc') == original.get('content-desc'), 'Native identity changed while scrolling'
        if i == 0: assert abs(bounds(current)[1]-t) > 30*scale, 'Page did not scroll'
    pid = adb('shell', 'pidof', 'dev.doroti.testbed').decode().strip()
    log = adb('logcat', '-d', '-v', 'epoch', '--pid='+pid).decode(errors='replace')
    (a.output/'process.log').write_text(log, encoding='utf-8')
    errors = [line for line in log.splitlines() if re.search(r' E DorotiGraphite| E DorotiMauiFailure|FATAL EXCEPTION|Fatal signal', line)]
    assert not errors, errors[:1]
    rows,counters = [],[]
    for line in log.splitlines():
        match = re.match(r'\s*(\d+\.\d+)', line)
        if not match or not any(start <= float(match[1]) <= end for start,end in windows): continue
        if 'DorotiPlatformTiming' in line:
            rows.append(dict((k,float(v)) for k,v in re.findall(r'(ownerMs|vulkanMs|paintMs|fenceMs)=([\d.]+)',line)))
        if 'DorotiPlatformFrame' in line:
            counters.append(dict((k,int(v)) for k,v in re.findall(r'(frame|readbackBytes|reusedSlices)=(\d+)',line)))
    assert len(rows)>10, 'Missing scroll frames'
    duration=sum(end-start for start,end in windows)
    result.update(status='PASS', windows=windows, frames=len(rows), measuredSeconds=duration,
                  acceptedFramesPerSecond=len(rows)/duration, timingMs={k:dist([r[k] for r in rows]) for k in rows[0]},
                  over16ms=sum(r['ownerMs']>16.67 for r in rows), over33ms=sum(r['ownerMs']>33.33 for r in rows),
                  nativeIdentity=original.get('content-desc'), runtimeErrors=errors)
    # Cumulative counters span intervening captures too; keep those separate from timed gestures.
    if len(counters)>1: result['wholeSpanRasterDelta']={k:counters[-1][k]-counters[0][k] for k in counters[0]}
finally:
    (a.output/'result.json').write_text(json.dumps(result,indent=2),encoding='utf-8')
print(json.dumps(result),flush=True)
