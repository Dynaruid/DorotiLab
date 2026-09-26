"""Visible product WGSL smoke gate. Synthetic input, not physical interaction proof."""
import ctypes as c
from ctypes import wintypes as w
from pathlib import Path
import os
import subprocess
import time
import json
from PIL import ImageGrab

ROOT = Path(__file__).resolve().parents[3]
OUT = Path(os.environ.get('DOROTI_GPU_EFFECT_OUT', str(ROOT / 'Doroti/artifacts/gpu-effects/windows')))
OUT.mkdir(parents=True, exist_ok=True)
EXE = Path(os.environ.get('DOROTI_GPU_EFFECT_EXE', str(ROOT / 'DorotiTestbedApp/windowsappsdk/bin/Release/net10.0-windows10.0.19041.0/win-x64/DorotiTestbedApp.WindowsAppSdk.exe')))
u = c.WinDLL('user32')
u.SetProcessDpiAwarenessContext.argtypes = [w.HANDLE]
u.SetProcessDpiAwarenessContext(w.HANDLE(-4))
u.GetWindowThreadProcessId.argtypes = [w.HWND, c.POINTER(w.DWORD)]
u.IsWindowVisible.argtypes = [w.HWND]
u.GetWindowRect.argtypes = [w.HWND, c.POINTER(w.RECT)]
u.SetWindowPos.argtypes = [w.HWND, w.HWND, c.c_int, c.c_int, c.c_int, c.c_int, w.UINT]
u.SetForegroundWindow.argtypes = [w.HWND]
u.PostMessageW.argtypes = [w.HWND, w.UINT, w.WPARAM, w.LPARAM]
u.ScreenToClient.argtypes = [w.HWND, c.POINTER(w.POINT)]
u.GetDpiForWindow.argtypes = [w.HWND]
u.GetForegroundWindow.restype = w.HWND
u.GetCursorPos.argtypes = [c.POINTER(w.POINT)]
u.SetCursorPos.argtypes = [c.c_int, c.c_int]
u.mouse_event.argtypes = [w.DWORD, w.DWORD, w.DWORD, w.DWORD, c.c_size_t]
ENUM = c.WINFUNCTYPE(w.BOOL, w.HWND, w.LPARAM)
u.EnumWindows.argtypes = [ENUM, w.LPARAM]
dwm = c.WinDLL('dwmapi')
dwm.DwmGetWindowAttribute.argtypes = [w.HWND, w.DWORD, c.c_void_p, w.DWORD]

def window(pid):
    found = []
    @ENUM
    def visit(hwnd, _):
        owner = w.DWORD()
        u.GetWindowThreadProcessId(hwnd, c.byref(owner))
        if owner.value == pid and u.IsWindowVisible(hwnd):
            cloaked = w.DWORD()
            if dwm.DwmGetWindowAttribute(hwnd, 14, c.byref(cloaked), c.sizeof(cloaked)) == 0 and not cloaked.value:
                found.append(hwnd)
        return True
    u.EnumWindows(visit, 0)
    return found[0] if found else None

def capture(hwnd, name):
    rect = w.RECT()
    u.GetWindowRect(hwnd, c.byref(rect))
    im = ImageGrab.grab((rect.left, rect.top, rect.right, rect.bottom), include_layered_windows=True)
    im.save(OUT / f'{name}.png')
    return im, rect

def stripes(im):
    # Independent display oracle: recognize substantial consecutive primary-color
    # runs, without using the shader output as a reference image.
    rows = {'RGB': [], 'BGR': []}
    for y in range(0, im.height, 4):
        runs = []
        previous, start = None, 0
        for x in range(im.width):
            r, g, b = im.getpixel((x, y))[:3]
            color = 'R' if r > 245 and g < 10 and b < 10 else 'G' if g > 245 and r < 10 and b < 10 else 'B' if b > 245 and r < 10 and g < 10 else None
            if color != previous:
                if previous and x - start > 25:
                    runs.append((previous, start, x))
                previous, start = color, x
        for i in range(len(runs) - 2):
            triple = runs[i:i + 3]
            key = ''.join(r[0] for r in triple)
            if key in rows and all(triple[j][2] == triple[j + 1][1] for j in range(2)):
                rows[key].append((y, triple[0][1], triple[-1][2]))
    return rows

env = dict(os.environ, DOROTI_TESTBED_MODE=os.environ.get('DOROTI_GPU_EFFECT_MODE', 'gpu-effects'), DOROTI_MAUI_EVIDENCE=str(OUT / 'evidence.json'))
(OUT / 'evidence.json').unlink(missing_ok=True)
(OUT / 'result.json').unlink(missing_ok=True)
with (OUT / 'process.log').open('w', encoding='utf-8') as log:
    process = subprocess.Popen([str(EXE)], cwd=EXE.parent, env=env, stdout=log, stderr=log)
    hwnd = None
    try:
        deadline = time.monotonic() + 30
        while time.monotonic() < deadline and not hwnd:
            if process.poll() is not None:
                raise RuntimeError(f'App exited {process.returncode}; see process.log')
            hwnd = window(process.pid)
            time.sleep(.1)
        assert hwnd, 'No product window'
        u.SetForegroundWindow(hwnd)
        if 'WindowsAppSdk' not in EXE.name:
            for _ in range(25):
                try:
                    evidence = json.loads((OUT / 'evidence.json').read_text(encoding='utf-8-sig'))
                    if evidence['frame']['presented'] > 0:
                        break
                except (OSError, ValueError, KeyError):
                    pass
                time.sleep(.5)
            else:
                raise RuntimeError('MAUI did not present its first frame')
        results = []
        for index, (width, height) in enumerate([(1100, 950), (1200, 1050), (1050, 900)]):
            u.SetWindowPos(hwnd, None, 80, 60, width, height, 0x14)
            time.sleep(2)
            im, rect = capture(hwnd, f'enabled-{index}')
            rows = stripes(im)
            assert len(rows['RGB']) > 10 and len(rows['BGR']) > 10, f'Missing input/output stripes: {rows}'
            assert min(r[0] for r in rows['RGB']) < min(r[0] for r in rows['BGR'])
            results.append({'size': [width, height], 'originalRows': len(rows['RGB']), 'effectRows': len(rows['BGR'])})
        y, left, right = rows['BGR'][-1]
        scale = u.GetDpiForWindow(hwnd) / 96
        point = w.POINT(rect.left + (left + right) // 2, rect.top + y + round(46 * scale))
        assert u.GetForegroundWindow() == hwnd, 'Test app lost foreground'
        cursor = w.POINT()
        u.GetCursorPos(c.byref(cursor))
        try:
            u.SetCursorPos(point.x, point.y)
            u.mouse_event(2, 0, 0, 0, 0)
            u.mouse_event(4, 0, 0, 0, 0)
        finally:
            u.SetCursorPos(cursor.x, cursor.y)
        time.sleep(1)
        im, _ = capture(hwnd, 'disabled')
        disabled = stripes(im)
        assert not disabled['BGR'] and len(disabled['RGB']) > len(rows['RGB']) + 10, 'Toggle did not disable the GPU effect'
        result = {'status': 'PASS', 'host': str(EXE), 'mode': env['DOROTI_TESTBED_MODE'], 'resizeCases': results, 'disabled': True,
                  'physicalInput': 'notVerified', 'performance': 'notMeasured'}
    finally:
        if hwnd:
            u.PostMessageW(hwnd, 0x10, 0, 0)
        try:
            process.wait(timeout=10)
        except subprocess.TimeoutExpired:
            process.kill()
            process.wait(timeout=5)
assert process.returncode == 0, f'Product shutdown failed: {process.returncode}'
result['exitCode'] = process.returncode
(OUT / 'result.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
print(json.dumps(result))
