"""Bounded native desktop-controller probe. Starts and closes only its own process."""
import ctypes as c
from ctypes import wintypes as w
import importlib.util
import json
import os
from pathlib import Path
import subprocess
import time

spec = importlib.util.spec_from_file_location('resize', Path(__file__).parents[1] / 'windows-maui/verify-resize.py')
g = importlib.util.module_from_spec(spec)
spec.loader.exec_module(g)
probe = g.OUT / 'desktop.json'
env = dict(os.environ, DOROTI_MAUI_EVIDENCE=str(g.OUT / 'evidence.json'),
           DOROTI_WINDOWS_MAUI_GRAPHITE='1', DOROTI_TESTBED_MODE='sample',
           DOROTI_DESKTOP_SAMPLE=os.environ.get('DOROTI_DESKTOP_SAMPLE', 'sudoku'), DOROTI_DESKTOP_PROBE=str(probe))
process = subprocess.Popen([str(g.EXE)], cwd=g.EXE.parent, env=env)
try:
    deadline = time.monotonic() + 45
    while not probe.exists() and time.monotonic() < deadline and process.poll() is None:
        failure = g.OUT / 'evidence.json.exception.txt'
        if failure.exists():
            raise AssertionError(failure.read_text(encoding='utf-8-sig'))
        time.sleep(.05)
    assert probe.exists(), f'No probe; exit={process.poll()}; evidence={g.OUT}'
    result = json.loads(probe.read_text(encoding='utf-8-sig'))
    assert not result['beforeShow']['Visible'], result
    assert result['beforeShow']['size'] == [450, 800], result
    assert result['shown']['Visible'] and result['hidden']['Visible'] is False, result
    assert result['resize']['size'] == [500, 650], result
    assert result['maximized']['presentation'] == 'Maximized', result
    assert result['restored']['presentation'] == 'Normal', result
    assert result['minimized']['presentation'] == 'Minimized', result
    assert result['fullscreen']['presentation'] == 'FullScreen', result
    assert result['windowed']['presentation'] == 'Normal', result
    assert result['appearance'] == {'changed': 'Applied', 'reset': 'Applied'}, result
    hwnd = g.wait_until(lambda: g.window_for(process.pid))
    g.u.GetWindowTextW.argtypes = [w.HWND, w.LPWSTR, c.c_int]
    title = c.create_unicode_buffer(256)
    g.u.GetWindowTextW(hwnd, title, len(title))
    assert title.value == 'Doroti Desktop Probe', title.value
    # Keep the caption on screen even when 800 DIP exceeds this monitor's work area.
    g.u.SetWindowPos(hwnd, None, 100, 100, 0, 0, 0x15)
    class Point(c.Structure):
        _fields_ = [('x', c.c_long), ('y', c.c_long)]
    class MinMax(c.Structure):
        _fields_ = [('reserved', Point), ('maxSize', Point), ('maxPos', Point), ('minTrack', Point), ('maxTrack', Point)]
    mm = MinMax()
    outcome = c.c_size_t()
    g.u.SendMessageTimeoutW(hwnd, 0x24, 0, c.addressof(mm), 2, 2000, c.byref(outcome))
    outer, client = w.RECT(), w.RECT()
    g.u.GetWindowRect(hwnd, c.byref(outer)); g.u.GetClientRect(hwnd, c.byref(client))
    scale = result['final']['Scale']
    assert mm.minTrack.x - (outer.right - outer.left - client.right) == round(350 * scale), (mm.minTrack.x, outer.right - outer.left, client.right, scale)
    assert mm.minTrack.y - (outer.bottom - outer.top - client.bottom) == round(500 * scale), (mm.minTrack.y, outer.bottom - outer.top, client.bottom, scale)
    g.ImageGrab.grab((outer.left, outer.top, outer.right, outer.bottom)).save(g.OUT / 'desktop.png')
    g.u.PostMessageW(hwnd, 0x10, 0, 0)
    g.wait_until(lambda: Path(str(probe) + '.close').exists())
    time.sleep(.3)
    assert process.poll() is None and Path(str(probe) + '.close').read_text() == '1', 'First native close was not canceled'
    g.u.PostMessageW(hwnd, 0x10, 0, 0)
    code = process.wait(timeout=15)
    assert code == 0, f'Unclean exit: {code:#x}'
    result.update(status='PASS', cleanExit=True, physicalDisplay='notVerified', customChrome='notImplemented')
    (g.OUT / 'desktop-result.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
    print(json.dumps(dict(result, output=str(g.OUT)), indent=2))
finally:
    if process.poll() is None:
        process.kill()
        process.wait(timeout=10)
