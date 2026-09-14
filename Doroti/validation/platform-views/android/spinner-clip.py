"""Check the live spinner's stroke thickness at all four cardinal points across rotation."""
import argparse
import io
import json
import math
from pathlib import Path
import re
import subprocess
import time
import xml.etree.ElementTree as ET
from PIL import Image

parser = argparse.ArgumentParser()
parser.add_argument('--serial', required=True)
parser.add_argument('--output', type=Path, required=True)
parser.add_argument('--expect-clipped', action='store_true')
args = parser.parse_args()
args.output.mkdir(parents=True, exist_ok=False)
commands = []


def adb(*parts):
    command = ['adb', '-s', args.serial, *map(str, parts)]
    result = subprocess.run(command, capture_output=True, timeout=1200)
    commands.append(dict(command=command, exitCode=result.returncode))
    (args.output / 'commands.json').write_text(json.dumps(commands, indent=2), encoding='utf-8')
    result.check_returncode()
    return result.stdout


remote = '/sdcard/doroti-spinner-clip-' + str(time.time_ns()) + '.xml'
adb('shell', 'uiautomator', 'dump', remote)
xml = adb('shell', 'cat', remote)
adb('shell', 'rm', remote)
(args.output / 'tree.xml').write_bytes(xml)
button = next(n for n in ET.fromstring(xml).iter('node') if n.get('class') == 'android.widget.Button'
              and n.get('content-desc', '').startswith('doroti-platform-view-'))
left, top, right, bottom = map(int, re.findall(r'\d+', button.get('bounds')))
scale = (right - left) / 220
cx, cy = left + 48 * scale, top + 48 * scale
max_runs = dict(left=0, right=0, top=0, bottom=0)
directions = dict(left=(-1, 0), right=(1, 0), top=(0, -1), bottom=(0, 1))
minimum = math.ceil(3 * scale - 1.5)
for sample in range(30):
    png = adb('exec-out', 'screencap', '-p')
    (args.output / f'frame-{sample:02}.png').write_bytes(png)
    image = Image.open(io.BytesIO(png)).convert('RGB')
    for name, (dx, dy) in directions.items():
        for offset in (-1, 0, 1):
            run = 0
            for distance in range(math.floor(10 * scale), math.ceil(19 * scale)):
                x = round(cx + dx * distance + dy * offset)
                y = round(cy + dy * distance + dx * offset)
                r, g, b = image.getpixel((x, y))
                run = run + 1 if r < 60 and 100 < g < 200 and b > 220 else 0
                max_runs[name] = max(max_runs[name], run)
    if sample >= 11 and all(run >= minimum for run in max_runs.values()):
        break
    # Avoid repeatedly capturing the same gap in the expanding/rotating arc.
    time.sleep(.11 + (sample % 7) * .037)
intact = all(run >= minimum for run in max_runs.values())
passed = not intact if args.expect_clipped else intact
result = dict(status='PASS' if passed else 'FAIL', expectedClipped=args.expect_clipped,
              scale=scale, center=[cx, cy], expectedStrokePixels=3 * scale,
              minimumStrongColorRun=minimum, observedMaximumRuns=max_runs, samples=sample + 1,
              physicalInput=False)
(args.output / 'result.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
print(json.dumps(result), flush=True)
if not passed:
    raise RuntimeError('Unexpected spinner stroke clipping result')
