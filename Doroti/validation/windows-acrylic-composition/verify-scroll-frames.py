"""Observe presented native/raster edges during scroll. Use a 1200s parent timeout.

Requires numpy, Pillow, dxcam and comtypes. Optional artifact-local packages are
loaded from Doroti/artifacts/validation/capture-python; no packages/results go here.
"""
import importlib.util
import json
import os
from pathlib import Path
import subprocess
import sys
import threading
import time

sys.dont_write_bytecode = True
ROOT = Path(__file__).resolve().parents[3]
sys.path.insert(0, str(ROOT / 'Doroti/artifacts/validation/capture-python'))
import dxcam
import numpy as np
from PIL import Image

spec = importlib.util.spec_from_file_location('sample', Path(__file__).with_name('verify-sample-input.py'))
s = importlib.util.module_from_spec(spec)
spec.loader.exec_module(s)
g, c, w, OUT = s.g, s.c, s.w, s.OUT


def main():
    env = os.environ.copy()
    env.pop('DOROTI_TESTBED_MODE', None)
    env.update(DOROTI_PLATFORM_VIEW_EVIDENCE=str(OUT / 'frame.json'),
        DOROTI_WINDOWS_EXPERIMENTAL_ACRYLIC_READY_FILE=str(OUT / 'ready.json'))
    print(OUT, flush=True)
    hwnd = 0
    camera = None
    with (OUT / 'product.log').open('w', encoding='utf-8') as log:
        process = subprocess.Popen([str(g.EXE)], cwd=g.EXE.parent, env=env, stdout=log, stderr=subprocess.STDOUT)
        try:
            hwnd = g.wait_for(lambda: g.read(OUT / 'ready.json'), process)['hwnd']
            g.u.SetWindowPos(hwnd, w.HWND(-1), 30, 30, 0, 0, 0x41)
            scale = g.u.GetDpiForWindow(hwnd) / 96
            time.sleep(.5)
            s.click(hwnd, 100, 88, scale)
            time.sleep(.5)
            s.click(hwnd, 620, 28, scale)
            time.sleep(.5)
            s.click(hwnd, 648, 590, scale)
            g.wait_for(lambda: (f if (f := g.read(OUT / 'frame.json')) and len(f['native']) == 2 else None), process)
            time.sleep(.6)
            origin = w.POINT()
            g.u.ClientToScreen(hwnd, c.byref(origin))
            reentry = os.environ.get('DOROTI_SCROLL_REENTRY') == '1'
            initial = g.read(OUT / 'frame.json')
            identities = [n['handle'] for n in initial['native']]
            if reentry:
                s.click(hwnd, 380, initial['native'][0]['transform']['Dy'] + 100, scale)
                g.wait_for(lambda: g.focus(hwnd) == initial['native'][1]['hwnd'], process)
            region_top = 56 if reentry else 100
            region = (origin.x, origin.y + round(region_top * scale), origin.x + round(440 * scale), origin.y + round(530 * scale))
            g.u.SetCursorPos(origin.x + round(500 * scale), origin.y + round(360 * scale))
            camera = dxcam.create(output_color='BGRA', max_buffer_len=8)
            camera.start(region=region, target_fps=120)
            cycles = int(os.environ.get('DOROTI_SCROLL_CAPTURE_CYCLES', '1'))
            def stimulus():
                time.sleep(.2)
                for i in range(32 * cycles):
                    amount = 60 if reentry else 12
                    s.mouse(0x800, -amount if i % 32 < 16 else amount)
                    time.sleep(.05)
            worker = threading.Thread(target=stimulus)
            worker.start()
            rows = []
            bad_saved = 0
            started = time.monotonic()
            last_timestamp = None
            while time.monotonic() - started < 1 + 1.6 * cycles:
                frame, timestamp = camera.get_latest_frame(with_timestamp=True)
                if timestamp == last_timestamp:
                    continue
                last_timestamp = timestamp
                # Green middle raster and native editor have a fixed 20-logical-pixel
                # top-edge separation. Observe both in the same desktop frame.
                green = frame[:, round(140 * scale), :3]
                gy = np.flatnonzero((green[:, 1] == 170) & (green[:, 0] == 85) & (green[:, 2] < 4))
                white = frame[:, round(380 * scale), :3]
                ey = np.flatnonzero((white > 250).all(axis=1))
                error = None if len(ey) == 0 or len(gy) == 0 else float(ey[0] - gy[0] - 20 * scale)
                checker_missing = True
                if len(gy):
                    top = int(gy[0] - round(50 * scale))
                    bottom = int(gy[0] + round(170 * scale))
                    checker = frame[max(0, top):min(len(frame), bottom), round(420 * scale), :3]
                    checker_missing = bool(len(checker) == 0 or np.mean(((checker == 245).all(axis=1)) |
                        ((checker == 199).all(axis=1))) < .98)
                broken = error is None or abs(error) > 4 or len(ey) < round(90 * scale) or len(gy) < round(70 * scale) or checker_missing
                row = dict(timestamp=timestamp, greenTop=int(gy[0]) if len(gy) else None, editorTop=int(ey[0]) if len(ey) else None,
                    editorWhitePixels=len(ey), edgeErrorPixels=error, checkerMissing=checker_missing, broken=broken)
                if reentry:
                    checker = frame[:, round(420 * scale):round(432 * scale), :3]
                    checker_rows = np.flatnonzero((((checker == 245).all(axis=2)) | ((checker == 199).all(axis=2))).mean(axis=1) > .95)
                    expected = actual = 0
                    if len(checker_rows) >= 8:
                        scene_bottom = int(checker_rows[-1]) + 1
                        start = max(0, scene_bottom - round(160 * scale) + 4)
                        end = max(0, scene_bottom - round(60 * scale) - 4)
                        expected = max(0, end - start)
                        actual = int((white[start:end] > 250).all(axis=1).sum())
                    broken = expected >= 10 and expected - actual > 4
                    row.update(expectedNativePixels=expected, actualNativePixels=actual, checkerPixels=len(checker_rows), broken=broken)
                    if len(checker_rows) == 0 and not any(r.get('culledState') for r in rows):
                        row['culledState'] = g.read(OUT / 'frame.json')
                        row['nativeFocusReleased'] = g.focus(hwnd) == hwnd
                if broken:
                    row['backendFrame'] = g.read(OUT / 'frame.json')
                    row['windows'] = g.children(hwnd)
                rows.append(row)
                if (broken and bad_saved < 8) or len(rows) == 1:
                    Image.fromarray(frame[:, :, [2, 1, 0]]).save(OUT / f'scroll-{len(rows):03d}.png')
                    bad_saved += int(broken)
            worker.join(timeout=5)
            camera.stop()
            result = dict(capture='DXGI desktop duplication, unique presented timestamps',
                samples=len(rows), brokenFrames=sum(r['broken'] for r in rows),
                maxEdgeErrorPixels=max((abs(r['edgeErrorPixels']) for r in rows if r['edgeErrorPixels'] is not None), default=0),
                nativeTravelPixels=max(r['greenTop'] for r in rows if r['greenTop'] is not None)-min(r['greenTop'] for r in rows if r['greenTop'] is not None), frames=rows)
            if reentry:
                visibility = [r['expectedNativePixels'] >= 10 for r in rows]
                result.pop('maxEdgeErrorPixels')  # Clipped top edges are not the reentry oracle.
                result['reentries'] = sum(not a and b for a, b in zip(visibility, visibility[1:]))
                result['fullyCulledFrames'] = sum(r['checkerPixels'] == 0 for r in rows)
                result['maxNativeCoverageDeficit'] = max((r['expectedNativePixels'] - r['actualNativePixels'] for r in rows if r['expectedNativePixels'] >= 10), default=0)
                final = g.read(OUT / 'frame.json')
                assert [n['handle'] for n in final['native']] == identities, 'Native identity changed across culling'
                assert any(r.get('nativeFocusReleased') for r in rows), 'Offscreen native retained focus'
                assert result['fullyCulledFrames'] > 0, 'Complete scene did not leave the viewport'
                assert result['reentries'] >= cycles and not all(visibility), 'Native view never left and reentered the viewport'
            (OUT / 'scroll-frames.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
            print(json.dumps({k:v for k,v in result.items() if k != 'frames'}), flush=True)
            assert len(rows) >= 30 and result['nativeTravelPixels'] >= 50
            if os.environ.get('DOROTI_SCROLL_REQUIRE_COHERENT') == '1':
                assert result['brokenFrames'] == 0, 'Presented native and raster layers separated during scroll'
        finally:
            if camera:
                camera.release()
            if hwnd and process.poll() is None:
                g.u.PostMessageW(hwnd, 16, 0, 0)
            try:
                process.wait(timeout=20)
            except subprocess.TimeoutExpired:
                process.kill(); process.wait()
            assert process.returncode == 0, process.returncode


if __name__ == '__main__':
    main()
