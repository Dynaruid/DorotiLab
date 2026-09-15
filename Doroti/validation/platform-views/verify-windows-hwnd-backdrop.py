"""Mounted HWND backdrop pixels and live native repaint; use run-with-timeout.py (1200s)."""
import ctypes as c
from ctypes import wintypes as w
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
OUT = ROOT / 'Doroti/artifacts/validation/windows-hwnd-backdrop' / time.strftime('%Y%m%d-%H%M%S')
OUT.mkdir(parents=True)
os.environ['DOROTI_PLATFORM_VIEW_GATE_OUTPUT'] = str(OUT)
spec = importlib.util.spec_from_file_location('gate', ROOT / 'Doroti/validation/windows-acrylic-composition/verify.py')
g = importlib.util.module_from_spec(spec)
spec.loader.exec_module(g)
g.u.EnableWindow.argtypes = [w.HWND, w.BOOL]
g.u.IsWindowEnabled.argtypes = [w.HWND]
g.u.RedrawWindow.argtypes = [w.HWND, w.LPVOID, w.HANDLE, w.UINT]


def run(enabled):
    directory = OUT / ('on' if enabled else 'off')
    directory.mkdir()
    g.OUT = directory
    env = os.environ.copy()
    env.update(DOROTI_TESTBED_MODE='platform-views', DOROTI_PLATFORM_VIEW_BACKDROP=str(int(enabled)),
               DOROTI_PLATFORM_VIEW_EVIDENCE=str(directory / 'frame.json'),
               DOROTI_WINDOWS_EXPERIMENTAL_ACRYLIC_READY_FILE=str(directory / 'ready.json'))
    hwnd = 0
    with (directory / 'product.log').open('w', encoding='utf-8') as log:
        process = subprocess.Popen([str(g.EXE)], cwd=g.EXE.parent, env=env, stdout=log, stderr=subprocess.STDOUT)
        try:
            hwnd = g.wait_for(lambda: g.read(directory / 'ready.json'), process)['hwnd']
            g.u.SetWindowPos(hwnd, w.HWND(-1), 30, 30, 0, 0, 0x41)
            frame = g.wait_for(lambda: (f if (f := g.read(directory / 'frame.json')) and len(f['native']) == 2 else None), process)
            scale = frame['dpi'] / 96
            g.click(hwnd, 245, 72, scale)
            frame = g.wait_for(lambda: (f if (f := g.read(directory / 'frame.json')) and g.probe(f)[0] == 6 else None), process)
            assert len(frame['hwndBackdrops']) == int(enabled)
            editor = next(n['hwnd'] for n in frame['native'] if n['handle']['InstanceId'] == 2)
            def capture(native_enabled, name):
                # A disabled EDIT repaints its own background. The framework
                # scene and native identity stay unchanged.
                g.u.EnableWindow(editor, native_enabled)
                assert bool(g.u.IsWindowEnabled(editor)) == native_enabled
                assert g.u.RedrawWindow(editor, None, None, 0x581)
                time.sleep(.7)
                image, rect = g.capture(hwnd, name)
                origin = w.POINT()
                assert g.u.ClientToScreen(hwnd, c.byref(origin))
                def crop(bounds):
                    left, top, right, bottom = bounds
                    return image.convert('RGB').crop((origin.x - rect.left + round(left * scale),
                        origin.y - rect.top + round(top * scale), origin.x - rect.left + round(right * scale),
                        origin.y - rect.top + round(bottom * scale)))
                return (crop((240, 276, 320, 300)), crop((240, 302, 320, 324)),
                        crop((185, 224, 215, 250)), crop((240, 224, 320, 250)))

            text = capture(True, 'native-enabled')
            blank = capture(False, 'native-disabled')
            change = ImageStat.Stat(ImageChops.difference(text[3], blank[3])).mean
            assert max(change) > 2, f'Native repaint did not update foreground: {change}'
            # Repaint once more without changing the framework scene or HWND identity.
            restored = capture(True, 'native-restored')
            assert max(ImageStat.Stat(ImageChops.difference(text[0], restored[0])).mean) < 2
            final = g.read(directory / 'frame.json')
            assert [(n['handle'], n['hwnd']) for n in final['native']] == [(n['handle'], n['hwnd']) for n in frame['native']]
            if enabled:
                original = w.RECT()
                assert g.u.GetWindowRect(hwnd, c.byref(original))
                generation = final['frame']['SurfaceGeneration']
                assert g.u.SetWindowPos(hwnd, None, 30, 30, 900, 240, 0x14)
                small = g.wait_for(lambda: (f if (f := g.read(directory / 'frame.json')) and
                    f['frame']['SurfaceGeneration'] > generation else None), process)
                g.capture(hwnd, 'foreground-outside-viewport')
                assert g.u.SetWindowPos(hwnd, None, original.left, original.top,
                    original.right - original.left, original.bottom - original.top, 0x14)
                g.wait_for(lambda: (f if (f := g.read(directory / 'frame.json')) and
                    f['frame']['SurfaceGeneration'] > small['frame']['SurfaceGeneration'] else None), process)
                resumed = capture(True, 'after-resize')
                assert max(ImageStat.Stat(ImageChops.difference(text[0], resumed[0])).mean) < 2
            return text, dict(nativeRepaintMeanDifference=change, frame=final)
        finally:
            if hwnd and process.poll() is None:
                g.u.PostMessageW(hwnd, 0x10, 0, 0)
            try:
                process.wait(timeout=15)
            except subprocess.TimeoutExpired:
                process.kill()
                process.wait()
            assert process.returncode == 0, f'Product failed: {process.returncode}'


def edge_strength(image):
    # Maximum row-to-row change distinguishes a filtered native bottom edge
    # from its sharp boundary, regardless of the checkerboard phase.
    return max(sum(ImageStat.Stat(ImageChops.difference(image.crop((0, y, image.width, y + 1)),
        image.crop((0, y - 1, image.width, y)))).mean) for y in range(1, image.height))


def main():
    off, off_result = run(False)
    on, on_result = run(True)
    sharp, blurred = edge_strength(off[0]), edge_strength(on[0])
    assert blurred < sharp * .6, f'Native control edge was not blurred: {sharp} -> {blurred}'
    checker_off, checker_on = ImageStat.Stat(off[1]).stddev[0], ImageStat.Stat(on[1]).stddev[0]
    assert checker_on < checker_off * .65, f'Doroti backdrop was not blurred: {checker_off} -> {checker_on}'
    outside = max(ImageStat.Stat(ImageChops.difference(off[2], on[2])).mean)
    assert outside < 2, f'Pixels outside the foreground clip changed: {outside}'
    result = dict(scope='mounted Windows product; screen pixels; synthetic HWND input',
        nativeEdgeStrength=[sharp, blurred], checkerContrast=[checker_off, checker_on],
        outsideClipDifference=outside, off=off_result, on=on_result, physicalInput='notVerified')
    (OUT / 'result.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
    print(json.dumps(dict(result='passed', output=str(OUT), nativeEdgeStrength=[sharp, blurred],
                          checkerContrast=[checker_off, checker_on], outsideClipDifference=outside)), flush=True)


if __name__ == '__main__':
    main()
