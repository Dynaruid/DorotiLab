"""Two OS border drags using injected mouse input, not physical-display proof."""
import ctypes as c
import importlib.util
import json
import os
from pathlib import Path
import subprocess
import time
from PIL import ImageGrab

spec = importlib.util.spec_from_file_location('resize', Path(__file__).with_name('verify-resize.py'))
g = importlib.util.module_from_spec(spec)
spec.loader.exec_module(g)
g.u.SetCursorPos.argtypes = [c.c_int, c.c_int]
g.u.GetCursorPos.argtypes = [c.POINTER(g.w.POINT)]
g.u.mouse_event.argtypes = [g.w.DWORD, g.w.DWORD, g.w.DWORD, g.w.DWORD, c.c_size_t]
g.u.ShowWindow.argtypes = [g.w.HWND, c.c_int]
saved = g.w.POINT()
g.u.GetCursorPos(c.byref(saved))
process = None
hwnd = None
try:
    env = dict(os.environ, DOROTI_MAUI_EVIDENCE=str(g.OUT / 'evidence.json'),
               DOROTI_WINDOWS_MAUI_GRAPHITE='1', DOROTI_TESTBED_MODE='sample')
    process = subprocess.Popen([str(g.EXE)], cwd=g.EXE.parent, env=env)
    hwnd = g.wait_until(lambda: g.window_for(process.pid))
    g.u.SetForegroundWindow(hwnd)
    g.wait_until(lambda: (e := g.evidence()) and e['frame']['presented'] > 0)
    assert g.u.SetWindowPos(hwnd, None, 200, 200, 1000, 720, 0x14)
    time.sleep(.5)
    results = []
    for name in ('right', 'top-left'):
        rect = g.w.RECT()
        assert g.u.GetWindowRect(hwnd, c.byref(rect))
        start = (rect.right-3, (rect.top+rect.bottom)//2) if name == 'right' else (rect.left+3, rect.top+3)
        g.u.SetCursorPos(*start)
        time.sleep(.08)
        g.u.mouse_event(2, 0, 0, 0, 0)
        try:
            for step in range(1, 9):
                x = start[0] + (step*16 if name == 'right' else -step*12)
                y = start[1] + (0 if name == 'right' else -step*8)
                g.u.SetCursorPos(x, y)
                time.sleep(.025)
        finally:
            g.u.mouse_event(4, 0, 0, 0, 0)
        time.sleep(.4)
        after = g.w.RECT()
        assert g.u.GetWindowRect(hwnd, c.byref(after))
        assert after.right-after.left > rect.right-rect.left+40, f'{name}: OS did not resize'
        if name == 'top-left':
            assert after.left < rect.left-40 and after.top < rect.top-20
        ImageGrab.grab((after.left, after.top, after.right, after.bottom)).save(g.OUT / f'drag-{name}.png')
        results.append(dict(edge=name, finalRect=[after.left, after.top, after.right, after.bottom]))
    g.u.ShowWindow(hwnd, 3)  # Maximize changes non-client geometry, not WM_SIZING.
    time.sleep(.4)
    g.u.ShowWindow(hwnd, 9)
    time.sleep(.4)
    restored = g.w.RECT()
    assert g.u.GetWindowRect(hwnd, c.byref(restored))
    assert (restored.left, restored.top, restored.right, restored.bottom) == \
           (after.left, after.top, after.right, after.bottom), 'Restore lost window bounds'
    # Trigger a final snapshot after the one-second diagnostic throttle.
    time.sleep(1.2)
    assert g.u.SetWindowPos(hwnd, None, after.left, after.top,
                           after.right-after.left+1, after.bottom-after.top, 0x14)
    client = g.w.RECT()
    assert g.u.GetClientRect(hwnd, c.byref(client))
    evidence = g.wait_until(lambda: (e := g.evidence()) and
                           e['surface']['pixelWidth'] == client.right and e)
    trace = evidence['surface']['resizeTrace']
    prepared = [x for x in trace if x['phase'] == 'resize-frame-ready' and x['source'] == 'top-level.WM_SIZING']
    assert len(prepared) >= 4, 'The OS modal resize loop did not prepare frames'
    assert any('movingOrigin=True' in x['detail'] for x in prepared)
    assert any('movingOrigin=False' in x['detail'] for x in prepared)
    assert not any(x['phase'] == 'resize-frame-timeout' for x in trace), 'Native resize timed out'
    assert evidence['frame']['failed'] == 0
    assert not (g.OUT / 'evidence.json.exception.txt').exists()
    g.u.PostMessageW(hwnd, 0x10, 0, 0)
    assert process.wait(timeout=15) == 0
    result = dict(status='passed', drags=results, osSizingFrames=len(prepared),
                  maximizeRestore='passed',
                  cleanExit=True, physicalDrag='notVerified', output=str(g.OUT))
    (g.OUT / 'drag-result.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
    print(json.dumps(result, indent=2))
finally:
    g.u.mouse_event(4, 0, 0, 0, 0)
    g.u.SetCursorPos(saved.x, saved.y)
    if process and process.poll() is None:
        if hwnd:
            g.u.PostMessageW(hwnd, 0x10, 0, 0)
        try:
            process.wait(timeout=10)
        except subprocess.TimeoutExpired:
            process.kill()
            process.wait(timeout=10)
