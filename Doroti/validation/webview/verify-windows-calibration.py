"""Spatial pixel calibration of native WebView2 and Doroti raster backdrops."""
import ctypes
import datetime
import importlib.util
import json
import os
from pathlib import Path
import subprocess
import sys
import time
from PIL import ImageChops, ImageStat

sys.dont_write_bytecode = True
ROOT = Path(__file__).resolve().parents[3]
OUT = ROOT / 'Doroti/artifacts/webview' / datetime.date.today().isoformat() / 'windows' / ('calibration-' + time.strftime('%H%M%S'))
OUT.mkdir(parents=True)
os.environ['DOROTI_PLATFORM_VIEW_GATE_OUTPUT'] = str(OUT)
spec = importlib.util.spec_from_file_location('capture', ROOT / 'Doroti/validation/windows-acrylic-composition/verify.py')
g = importlib.util.module_from_spec(spec)
spec.loader.exec_module(g)
g.EXE = Path(os.environ.get('DOROTI_WEBVIEW_GATE_EXE') or g.EXE)

def main():
    signal = OUT / 'capture'
    env = os.environ.copy()
    env.update(DOROTI_TESTBED_MODE='platform-effects', DOROTI_WINDOWS_EFFECT_CALIBRATION=str(signal),
        DOROTI_PLATFORM_VIEW_EVIDENCE=str(OUT / 'frame.json'),
        DOROTI_WINDOWS_EXPERIMENTAL_ACRYLIC_READY_FILE=str(OUT / 'ready.json'),
        DOROTI_WEBVIEW_USER_DATA=str(OUT / 'profiles'))
    images = {}
    print(OUT, flush=True)
    with (OUT / 'product.log').open('w', encoding='utf-8') as log:
        process = subprocess.Popen([str(g.EXE)], cwd=g.EXE.parent, env=env, stdout=log, stderr=log)
        hwnd = 0
        try:
            hwnd = g.wait_for(lambda: g.read(OUT / 'ready.json'), process)['hwnd']
            g.u.SetForegroundWindow(hwnd)
            deadline = time.monotonic() + 120
            while time.monotonic() < deadline:
                done = Path(str(signal) + '.done')
                if done.exists():
                    assert done.read_text() == 'PASS', done.read_text()
                    break
                stage = Path(str(signal) + '.stage')
                name = stage.read_text() if stage.exists() else ''
                if name and name not in images:
                    frame = g.read(OUT / 'frame.json')
                    if not frame or not frame['native']:
                        time.sleep(.05)
                        continue
                    image, rectangle = g.capture(hwnd, name)
                    origin = g.w.POINT()
                    assert g.u.ClientToScreen(hwnd, g.c.byref(origin))
                    scale = g.u.GetDpiForWindow(hwnd) / 96
                    native = frame['native'][0]['bounds']
                    # Upper strip avoids the sharp foreground text at effect center.
                    x = origin.x - rectangle.left + (native[0] + 310) * scale
                    y = origin.y - rectangle.top + (native[1] + 100) * scale
                    crop = image.convert('RGB').crop((round(x - 100 * scale), round(y), round(x + 100 * scale), round(y + 8 * scale)))
                    images[name] = (crop, scale)
                    (OUT / (name + '.json')).write_text(json.dumps(frame, indent=2), encoding='utf-8')
                    Path(str(signal) + '.ack').write_text(name)
                time.sleep(.05)
            else:
                raise TimeoutError('Calibration fixture timed out')
        finally:
            if hwnd: g.u.PostMessageW(hwnd, 0x10, 0, 0)
            try: process.wait(timeout=20)
            except subprocess.TimeoutExpired:
                subprocess.run(['taskkill', '/PID', str(process.pid), '/T', '/F'], timeout=20, check=False)
                raise RuntimeError('Calibration shutdown timed out')
            assert process.returncode == 0, process.returncode
    measurements = {}
    for name in ('native-zero', 'native-4', 'native-16', 'raster-4', 'raster-16'):
        image, scale = images[name]
        values = [sum(image.getpixel((x, y))[0] for y in range(image.height)) / image.height for x in range(image.width)]
        def crossing(level):
            for x in range(1, len(values)):
                if values[x - 1] <= level <= values[x] and values[x] > values[x - 1]:
                    return x - 1 + (level - values[x - 1]) / (values[x] - values[x - 1])
            raise AssertionError(f'{name}: no crossing {level}; {min(values)}..{max(values)}')
        sigma = (crossing(229.5) - crossing(25.5)) / 2.563103 / scale
        measurements[name] = {'measuredLogicalSigma': sigma}
        expected = int(name.split('-')[-1]) if not name.endswith('zero') else 0
        assert sigma < .8 if expected == 0 else abs(sigma - expected) / expected < .2, (name, sigma)
    for before, after in [('native-zero', 'native-reset'), ('color-one', 'color-reset')]:
        difference = max(ImageStat.Stat(ImageChops.difference(images[before][0], images[after][0])).mean)
        measurements[after] = {'resetMeanDifference': difference}
        assert difference < 1, (after, difference)
    colors = {name: ImageStat.Stat(images[name][0]).mean for name in ('color-one', 'color-zero', 'color-two', 'color-tint')}
    assert max(colors['color-zero']) - min(colors['color-zero']) < 3, colors
    assert max(colors['color-two']) - min(colors['color-two']) > 1.5 * (max(colors['color-one']) - min(colors['color-one'])), colors
    expected_tint = [value * 127 / 255 + tint * 128 / 255 for value, tint in zip(colors['color-one'], (0, 0, 255))]
    assert max(abs(a - b) for a, b in zip(colors['color-tint'], expected_tint)) < 5, (colors, expected_tint)
    result = {'status': 'PASS', 'scope': 'product screen pixels; Gaussian edge width, native/raster, saturation, independent tint, reset',
              'measurements': measurements, 'colors': colors, 'physicalInput': 'notVerified'}
    (OUT / 'result.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
    print(json.dumps(result), flush=True)

if __name__ == '__main__':
    main()
