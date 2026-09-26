"""Visible Windows product gate; synthetic interaction, not physical input proof."""
import ctypes as c
from ctypes import wintypes as w
import json
import os
from pathlib import Path
import statistics
import subprocess
import time
from PIL import ImageGrab

ROOT = Path(__file__).resolve().parents[3]
OUT = Path(os.environ.get('DOROTI_VARIABLE_BLUR_OUT', ROOT / 'Doroti/artifacts/variable-blur/windows'))
EXE = Path(os.environ.get('DOROTI_VARIABLE_BLUR_EXE', ROOT / 'DorotiTestbedApp/windowsappsdk/bin/Release/net10.0-windows10.0.19041.0/win-x64/DorotiTestbedApp.WindowsAppSdk.exe'))
OUT.mkdir(parents=True, exist_ok=True)
u = c.WinDLL('user32')
u.SetProcessDpiAwarenessContext.argtypes = [w.HANDLE]
u.SetProcessDpiAwarenessContext(w.HANDLE(-4))
u.GetWindowThreadProcessId.argtypes = [w.HWND, c.POINTER(w.DWORD)]
u.IsWindowVisible.argtypes = [w.HWND]
u.GetWindowRect.argtypes = [w.HWND, c.POINTER(w.RECT)]
u.SetWindowPos.argtypes = [w.HWND, w.HWND, c.c_int, c.c_int, c.c_int, c.c_int, w.UINT]
u.SetForegroundWindow.argtypes = [w.HWND]
u.GetForegroundWindow.restype = w.HWND
u.WindowFromPoint.argtypes = [w.POINT]
u.WindowFromPoint.restype = w.HWND
u.GetDpiForWindow.argtypes = [w.HWND]
u.ScreenToClient.argtypes = [w.HWND, c.POINTER(w.POINT)]
u.PostMessageW.argtypes = [w.HWND, w.UINT, w.WPARAM, w.LPARAM]
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
    im = ImageGrab.grab((rect.left, rect.top, rect.right, rect.bottom), include_layered_windows=True).convert('RGB')
    im.save(OUT / f'{name}.png')
    return im, rect


def locate(im, scale):
    # Find complete 20-stripe sharp rows independently of framework coordinates.
    rows = []
    for y in range(im.height):
        runs = []
        previous, start = None, 0
        for x in range(im.width + 1):
            rgb = im.getpixel((x, y)) if x < im.width else (127, 0, 0)
            value = 0 if max(rgb) < 5 else 1 if min(rgb) > 250 else None
            if value != previous:
                if previous is not None and 10 * scale < x - start < 18 * scale:
                    runs.append((previous, start, x))
                previous, start = value, x
        for i in range(len(runs) - 17):
            group = runs[i:i + 18]
            if all(group[j][2] == group[j + 1][1] for j in range(17)):
                left = group[0][1]
                # The white final stripe can merge into the white app background.
                rows.append((y, left, left + round(280 * scale)))
                break
    assert rows, 'No displayed stripe pattern'
    original_top = rows[0][0]
    original_bottom = original_top + round(120 * scale)
    later = [r for r in rows if r[0] > original_bottom + 10 * scale]
    assert later, 'No clear end of the progressive blur'
    return later[0]


def contrast(im, row, fraction, scale):
    top, left, right = row
    y = top + round(fraction * 120 * scale)
    values = [im.getpixel((x, y))[0] for x in range(left + round(32 * scale), right - round(32 * scale))]
    return statistics.pstdev(values)


env = dict(os.environ, DOROTI_TESTBED_MODE='variable-blur', DOROTI_MAUI_EVIDENCE=str(OUT / 'evidence.json'))
(OUT / 'result.json').unlink(missing_ok=True)
(OUT / 'evidence.json').unlink(missing_ok=True)
with (OUT / 'process.log').open('w', encoding='utf-8') as log:
    process = subprocess.Popen([str(EXE)], cwd=EXE.parent, env=env, stdout=log, stderr=log)
    hwnd = None
    try:
        deadline = time.monotonic() + 30
        while time.monotonic() < deadline and not hwnd:
            if process.poll() is not None:
                raise RuntimeError(f'App exited {process.returncode}')
            hwnd = window(process.pid)
            time.sleep(.1)
        assert hwnd, 'No product window'
        if 'WindowsAppSdk' not in EXE.name:
            deadline = time.monotonic() + 30
            while time.monotonic() < deadline:
                try:
                    evidence = json.loads((OUT / 'evidence.json').read_text(encoding='utf-8-sig'))
                    if evidence['frame']['presented'] > 0:
                        break
                except (OSError, ValueError, KeyError):
                    pass
                time.sleep(.25)
            else:
                raise RuntimeError('MAUI did not present its first frame')
        u.SetForegroundWindow(hwnd)
        results = []
        for index, (width, height) in enumerate([(1000, 1050), (1100, 1100), (950, 1000)]):
            u.SetWindowPos(hwnd, None, 80, 40, width, height, 0x14)
            time.sleep(3)
            im, rect = capture(hwnd, f'enabled-{index}')
            scale = u.GetDpiForWindow(hwnd) / 96
            row = locate(im, scale)
            levels = [contrast(im, row, fraction, scale) for fraction in (.03, .4, .7)]
            assert levels[0] > 100 and levels[0] > levels[1] > levels[2] and levels[2] < 40, levels
            results.append({'size': [width, height], 'contrastClearMiddleBlurred': levels})
        top, left, right = row
        x = rect.left + (left + right) // 2
        y = rect.top + top + round(156 * scale)
        if 'WindowsAppSdk' in EXE.name:
            point = w.POINT(x, y)
            u.ScreenToClient(hwnd, c.byref(point))
            position = (point.y << 16) | (point.x & 0xffff)
            u.PostMessageW(hwnd, 0x200, 0, position)
            u.PostMessageW(hwnd, 0x201, 1, position)
            u.PostMessageW(hwnd, 0x202, 0, position)
        else:
            u.SetForegroundWindow(hwnd)
            time.sleep(.2)
            click_window = u.WindowFromPoint(w.POINT(x, y))
            click_owner = w.DWORD()
            u.GetWindowThreadProcessId(click_window, c.byref(click_owner))
            assert click_owner.value == process.pid, 'Toggle point is covered by another app'
            cursor = w.POINT()
            u.GetCursorPos(c.byref(cursor))
            try:
                u.SetCursorPos(x, y)
                u.mouse_event(2, 0, 0, 0, 0)
                u.mouse_event(4, 0, 0, 0, 0)
            finally:
                u.SetCursorPos(cursor.x, cursor.y)
        time.sleep(1)
        disabled, _ = capture(hwnd, 'disabled')
        disabled_levels = [contrast(disabled, row, fraction, scale) for fraction in (.03, .4, .7)]
        assert min(disabled_levels) > 100, disabled_levels
        result = {'status': 'PASS', 'host': str(EXE), 'resizeCases': results,
                  'disabledContrast': disabled_levels, 'physicalInput': 'notVerified', 'performance': 'notMeasured'}
    finally:
        if hwnd:
            u.PostMessageW(hwnd, 0x10, 0, 0)
        try:
            process.wait(timeout=10)
        except subprocess.TimeoutExpired:
            process.kill()
            process.wait(timeout=5)
assert process.returncode == 0, f'Product shutdown failed: {process.returncode}'
(OUT / 'result.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
print(json.dumps(result))
