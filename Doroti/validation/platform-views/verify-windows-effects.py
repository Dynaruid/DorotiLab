"""Mounted Windows WebView2 + common PlatformEffect gate. Use the 1200s parent wrapper."""
import importlib.util
import datetime
import json
import os
from pathlib import Path
import subprocess
import sys
import time
from PIL import ImageStat

sys.dont_write_bytecode = True
ROOT = Path(__file__).resolve().parents[3]
OUT = Path(os.environ.get('DOROTI_PLATFORM_VIEW_GATE_OUTPUT') or ROOT / 'Doroti/artifacts/platform-views' / datetime.date.today().isoformat() / 'windows' / ('effects-' + time.strftime('%H%M%S')))
OUT.mkdir(parents=True)
os.environ['DOROTI_PLATFORM_VIEW_GATE_OUTPUT'] = str(OUT)
spec = importlib.util.spec_from_file_location('gate', ROOT / 'Doroti/validation/windows-acrylic-composition/verify.py')
g = importlib.util.module_from_spec(spec)
spec.loader.exec_module(g)
g.EXE = Path(os.environ.get('DOROTI_WEBVIEW_GATE_EXE') or g.EXE)


def main():
    env = os.environ.copy()
    env.update(DOROTI_TESTBED_MODE='platform-effects', DOROTI_PLATFORM_VIEW_EVIDENCE=str(OUT / 'frame.json'),
        DOROTI_WINDOWS_EXPERIMENTAL_ACRYLIC_READY_FILE=str(OUT / 'ready.json'),
        DOROTI_WINDOWS_APPSDK_DIAGNOSTICS='1', DOROTI_WINDOWS_APPSDK_REPORT=str(OUT / 'report.json'),
        DOROTI_WEBVIEW_USER_DATA=str(OUT / 'webview-profile'))
    print(OUT, flush=True)
    with (OUT / 'product.log').open('w', encoding='utf-8') as log:
        process = subprocess.Popen([str(g.EXE)], cwd=g.EXE.parent, env=env, stdout=log, stderr=log)
        hwnd = 0
        try:
            ready = g.wait_for(lambda: g.read(OUT / 'ready.json'), process)
            hwnd = ready['hwnd']
            g.u.SetForegroundWindow(hwnd)
            frame = g.wait_for(lambda: (value if (value := g.read(OUT / 'frame.json')) and
                len(value['native']) == 1 and value['webView']['loadedViews'] == 1 else None), process, timeout=30)
            g.capture(hwnd, 'initial')
            scale = g.u.GetDpiForWindow(hwnd) / 96
            g.capture(hwnd, 'blur-on')
            time.sleep(.6)
            g.capture(hwnd, 'blur-live')
            identity = frame['native'][0]['handle']
            def state(predicate):
                return g.wait_for(lambda: (value if (value := g.read(OUT / 'frame.json')) and predicate(value) else None), process)
            touch_spec = importlib.util.spec_from_file_location('touch', ROOT / 'Doroti/validation/webview/windows-touch.py')
            touch = importlib.util.module_from_spec(touch_spec)
            touch_spec.loader.exec_module(touch)
            before_touch = frame['webView']['nativeMessages']
            touch.tap(hwnd, 448, 198, scale)
            frame = state(lambda f: f['webView']['nativeMessages'] == before_touch + 1 and f['webView']['pointerEvents'] >= 2)
            g.click(hwnd, 250, 72, scale, native_hit_test=False)
            frame = state(lambda f: f['effectProbe'].startswith('False') and f['webView']['effects'] == 0)
            assert frame['webView']['effects'] == 0
            g.capture(hwnd, 'blur-off')
            g.click(hwnd, 250, 72, scale, native_hit_test=False)
            frame = state(lambda f: f['effectProbe'].startswith('True') and f['webView']['effects'] == 1)
            assert frame['webView']['effects'] == 1
            before = frame['webView']['nativeMessages']
            touch.mouse_click(hwnd, 448, 198, scale)
            frame = state(lambda f: f['webView']['nativeMessages'] == before + 1)
            g.click(hwnd, 380, 72, scale, native_hit_test=False)
            frame = state(lambda f: f['effectProbe'].startswith('True,True') and len(f['shields']) == 2)
            touch.mouse_click(hwnd, 448, 198, scale)
            time.sleep(.3)
            assert g.read(OUT / 'frame.json')['webView']['mouseEvents'] == frame['webView']['mouseEvents']
            g.click(hwnd, 360, 403, scale, native_hit_test=False)
            frame = state(lambda f: f['effectProbe'].endswith(',1'))
            assert frame['native'][0]['handle'] == identity
            g.capture(hwnd, 'input-result')
            g.click(hwnd, 240, 552, scale, native_hit_test=False)
            frame = state(lambda f: len(f['native']) == 2 and f['webView']['loadedViews'] == 2)
            assert frame['native'][0]['handle'] == identity
            time.sleep(.3)
            g.capture(hwnd, 'two-webviews-effect')
            g.click(hwnd, 360, 552, scale, native_hit_test=False)
            frame = state(lambda f: f['shields'][0]['bounds'][0] == 230)
            g.capture(hwnd, 'moved-effect')
            original = g.w.RECT()
            assert g.u.GetWindowRect(hwnd, g.c.byref(original))
            generation = frame['frame']['SurfaceGeneration']
            assert g.u.SetWindowPos(hwnd, None, 50, 30, 1700, 1450, 0x14)
            frame = state(lambda f: f['frame']['SurfaceGeneration'] > generation and len(f['native']) == 2)
            assert frame['native'][0]['handle'] == identity
            g.capture(hwnd, 'resized')
            generation = frame['frame']['SurfaceGeneration']
            assert g.u.SetWindowPos(hwnd, None, original.left, original.top, original.right - original.left, original.bottom - original.top, 0x14)
            frame = state(lambda f: f['frame']['SurfaceGeneration'] > generation and len(f['native']) == 2)
            g.click(hwnd, 480, 552, scale, native_hit_test=False)
            frame = state(lambda f: not f['native'] and f['webView']['liveInstances'] == 0)
            g.capture(hwnd, 'disposed')
            g.click(hwnd, 480, 552, scale, native_hit_test=False)
            frame = state(lambda f: len(f['native']) == 2 and f['webView']['loadedViews'] == 2)
            assert all(n['handle']['InstanceGeneration'] > identity['InstanceGeneration'] for n in frame['native'])
            def native_pixels_visible():
                capture, rectangle = g.capture(hwnd, 'recreated')
                origin = g.w.POINT()
                assert g.u.ClientToScreen(hwnd, g.c.byref(origin))
                bounds = frame['native'][0]['bounds']
                x = round(origin.x - rectangle.left + (bounds[0] + 10) * scale)
                y = round(origin.y - rectangle.top + (bounds[1] + 320) * scale)
                stats = ImageStat.Stat(capture.convert('RGB').crop((x, y, x + round(100 * scale), y + round(40 * scale))))
                effect_bounds = frame['shields'][0]['bounds']
                ex = round(origin.x - rectangle.left + (effect_bounds[0] + 100) * scale)
                ey = round(origin.y - rectangle.top + (effect_bounds[1] + 125) * scale)
                effect = ImageStat.Stat(capture.convert('RGB').crop((ex, ey, ex + round(100 * scale), ey + round(50 * scale))))
                return min(stats.mean) > 70 and min(stats.stddev) > 20 and min(effect.mean) < 230
            g.wait_for(native_pixels_visible, process, timeout=10)
            assert frame['readbackBytes'] == 0 and frame['uploadedBytes'] == 0
            assert frame['webView']['rasterUploadBytes'] == 0 and frame['webView']['rasterGpuCopies'] > 0
            (OUT / 'observed.json').write_text(json.dumps({'scope': 'mounted-product', 'frame': frame,
                'automated': ['OS injected touch through SendPointerInput', 'effect removal/restoration', 'native pass-through click', 'shield blocks native click',
                    'sharp foreground button', 'native identity preserved', 'two native views with middle raster and effect',
                    'effect movement', 'resize with native identity preserved', 'last native removal', 'recreation with fresh generation and visible native pixels'],
                'visualReview': 'pending', 'physicalInput': 'notVerified'}, indent=2), encoding='utf-8')
        except Exception:
            if hwnd and process.poll() is None:
                g.capture(hwnd, 'failure')
            raise
        finally:
            if hwnd and process.poll() is None:
                g.u.PostMessageW(hwnd, 0x10, 0, 0)
            try:
                process.wait(timeout=15)
            except subprocess.TimeoutExpired:
                process.kill(); process.wait()
            print('exit=', process.returncode, flush=True)
            assert process.returncode == 0, f'Product shutdown failed: {process.returncode}'


if __name__ == '__main__':
    main()
