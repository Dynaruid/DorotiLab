"""Verify red-only live backdrop in the actual Material sample Platform views page."""
import argparse
import io
import json
from pathlib import Path
import re
import subprocess
import time
import xml.etree.ElementTree as ET
from PIL import Image, ImageChops, ImageStat

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


def tree(label):
    remote = '/sdcard/doroti-backdrop-' + str(time.time_ns()) + '.xml'
    for attempt in range(3):
        dumped = adb('shell', 'uiautomator', 'dump', remote)
        if b'dumped to:' in dumped:
            break
        time.sleep(1)
    xml = adb('shell', 'cat', remote)
    adb('shell', 'rm', remote)
    (args.output / (label + '.xml')).write_bytes(xml)
    return list(ET.fromstring(xml).iter('node'))


def screen(label):
    png = adb('exec-out', 'screencap', '-p')
    (args.output / (label + '.png')).write_bytes(png)
    return Image.open(io.BytesIO(png)).convert('RGB')


def bounds(node):
    return list(map(int, re.findall(r'\d+', node.get('bounds'))))


def tap_node(node):
    l, t, r, b = bounds(node)
    adb('shell', 'input', 'tap', (l+r)//2, (t+b)//2)
    time.sleep(.4)


def named(nodes, label):
    return next(n for n in nodes if n.get('text', '').startswith(label) and n.get('clickable') == 'true')


def native(nodes, cls):
    return next(n for n in nodes if n.get('class') == 'android.widget.' + cls
                and n.get('content-desc', '').startswith('doroti-platform-view-'))


def launch(enabled):
    label = 'on' if enabled else 'off'
    adb('shell', 'am', 'force-stop', 'dev.doroti.testbed')
    adb('shell', 'am', 'start', '-n', 'dev.doroti.testbed/crc64c80c495bd333b69c.MainActivity',
        '--es', 'doroti_testbed_mode', 'sample', '--es', 'doroti_platform_view_backdrop', str(int(enabled)))
    time.sleep(2)
    nodes = tree(label + '-start')
    tap_node(named(nodes, 'Platform views'))
    time.sleep(1)
    nodes = tree(label + '-page')
    assert any(n.get('selected') == 'true' and n.get('text', '').startswith('Platform views') for n in nodes)
    native(nodes, 'Button'); native(nodes, 'EditText')
    return nodes, screen(label)


result = dict(status='FAIL', serial=args.serial, mode='sample', page='Platform views', physicalInput=False)
try:
    off_nodes, off = launch(False)
    nodes, on = launch(True)
    button = native(nodes, 'Button')
    left, top, right, bottom = bounds(button)
    scale = (right-left)/220
    ox, oy = left-20*scale, top-20*scale

    def roi(x, y, w, h):
        return tuple(round(v) for v in (ox+x*scale, oy+y*scale, ox+(x+w)*scale, oy+(y+h)*scale))

    red = roi(230, 100, 100, 80)
    green = roi(130, 65, 70, 12)
    checker = roi(20, 190, 100, 30)
    red_diff = ImageChops.difference(off.crop(red), on.crop(red))
    changed = sum(max(pixel) > 8 for pixel in red_diff.get_flattened_data())
    assert changed > 200*scale*scale, 'Red backdrop has no visible effect'
    assert max(ImageStat.Stat(ImageChops.difference(off.crop(green), on.crop(green))).mean) < 1, 'Green changed'
    assert max(ImageStat.Stat(ImageChops.difference(off.crop(checker), on.crop(checker))).mean) < 1, 'Outside background changed'
    colors = on.crop(checker).getcolors(100000)
    assert colors and sum(count > 30 for count, rgb in colors) >= 2, 'Checkerboard missing'
    outside = ImageChops.difference(off, on)
    outside.paste((0, 0, 0), red)
    outside.paste((0, 0, 0), roi(50, 50, 36, 36))
    outside_mean = ImageStat.Stat(outside.crop(roi(0, 0, min(440, (on.width-ox)/scale), 240))).mean
    assert max(outside_mean) < 1, 'Backdrop changed pixels outside the red rectangle'
    result.update(redChangedPixels=changed, greenUnchanged=True, checkerOutsideUnchanged=True,
                  outsideSceneMeanDifference=outside_mean)

    # Spinner occupies the native button; taps must reach it exactly once.
    adb('shell', 'input', 'tap', round(ox+68*scale), round(oy+68*scale))
    nodes = tree('spinner-tap')
    assert native(nodes, 'Button').get('text') == 'Native clicks: 1'
    frame = screen('spinner-next')
    assert ImageChops.difference(on.crop(roi(50, 50, 36, 36)), frame.crop(roi(50, 50, 36, 36))).getbbox()
    result['spinnerMotionAndTap'] = True

    # Red shield should receive its tap, preserving the editor text/focus path.
    adb('shell', 'input', 'tap', round(ox+275*scale), round(oy+150*scale))
    nodes = tree('red-tap')
    assert any('Foreground taps: 1' in n.get('text', '') for n in nodes)
    result['redInputShield'] = True
    before_text = native(nodes, 'EditText').get('text')
    adb('shell', 'input', 'tap', round(ox+200*scale), round(oy+155*scale))
    adb('shell', 'input', 'keyevent', 'KEYCODE_MOVE_END')
    adb('shell', 'input', 'text', 'Blur1234')
    edited = tree('editing')
    after_text = native(edited, 'EditText').get('text')
    assert after_text != before_text and after_text.endswith('1234'), 'Native editor did not update'
    adb('shell', 'input', 'keyevent', 'KEYCODE_BACK')
    time.sleep(.7)
    edited_frame = screen('edited')
    assert ImageChops.difference(on.crop(red), edited_frame.crop(red)).getbbox(), 'Blur failed to refresh after native edit'
    result.update(nativeTextBefore=before_text, nativeTextAfter=after_text, liveBlurUpdated=True)

    nodes = tree('before-leave')
    tap_node(named(nodes, 'Color'))
    nodes = tree('color')
    assert not any(n.get('content-desc', '').startswith('doroti-platform-view-') for n in nodes)
    tap_node(named(nodes, 'Platform views'))
    nodes = tree('returned')
    assert native(nodes, 'Button').get('text') == 'Native button'
    native(nodes, 'EditText')
    screen('returned')
    tap_node(named(nodes, 'Dispose controls'))
    nodes = tree('disposed')
    assert not any(n.get('content-desc', '').startswith('doroti-platform-view-') for n in nodes)
    screen('disposed')
    tap_node(named(nodes, 'Create controls'))
    nodes = tree('recreated')
    native(nodes, 'Button'); native(nodes, 'EditText')
    screen('recreated')
    result.update(navigationAndRecreation=True, status='PASS')
finally:
    pid = adb('shell', 'pidof', 'dev.doroti.testbed').decode().strip()
    log = adb('logcat', '-d', '--pid=' + pid).decode(errors='replace')
    (args.output / 'logcat.txt').write_text(log, encoding='utf-8')
    errors = [line for line in log.splitlines() if 'FATAL EXCEPTION' in line or re.search(r'E (DorotiGraphite|DorotiMauiFailure)', line)]
    result['runtimeErrors'] = errors
    if errors: result['status'] = 'FAIL'
    (args.output / 'result.json').write_text(json.dumps(result, indent=2, ensure_ascii=False), encoding='utf-8')
print(json.dumps(result, ensure_ascii=False), flush=True)
assert result['status'] == 'PASS'
