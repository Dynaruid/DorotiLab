"""Active-window Acrylic pixel response to two controlled background colors."""
import ctypes as c
import json
import os
import subprocess
import time
import tkinter as tk
from PIL import ImageChops, ImageGrab, ImageStat
import importlib.util
from pathlib import Path

spec = importlib.util.spec_from_file_location('resize', Path(__file__).with_name('verify-resize.py'))
g = importlib.util.module_from_spec(spec)
spec.loader.exec_module(g)
g.u.SetCursorPos.argtypes = [c.c_int, c.c_int]
g.u.GetCursorPos.argtypes = [c.POINTER(g.w.POINT)]
g.u.mouse_event.argtypes = [g.w.DWORD, g.w.DWORD, g.w.DWORD, g.w.DWORD, c.c_size_t]
background = tk.Tk()
background.overrideredirect(True)
background.geometry('1100x850+70+70')
background.configure(bg='#ff2020')
background.update()
cursor = g.w.POINT()
g.u.GetCursorPos(c.byref(cursor))
process = None
hwnd = None

try:
    env = dict(os.environ, DOROTI_MAUI_EVIDENCE=str(g.OUT / 'evidence.json'),
               DOROTI_WINDOWS_MAUI_GRAPHITE='1', DOROTI_TESTBED_MODE='sample')
    process = subprocess.Popen([str(g.EXE)], cwd=g.EXE.parent, env=env)
    hwnd = g.wait_until(lambda: g.window_for(process.pid))
    g.u.SetWindowPos(hwnd, None, 100, 100, 960, 700, 0x14)
    g.u.SetForegroundWindow(hwnd)
    g.wait_until(lambda: (e := g.evidence()) and e['frame']['presented'] > 0)
    time.sleep(1)

    def capture(name, color):
        background.configure(bg=color)
        background.update()
        time.sleep(.8)
        bounds = g.w.RECT()
        g.u.GetWindowRect(hwnd, c.byref(bounds))
        im = ImageGrab.grab((bounds.left, bounds.top, bounds.right, bounds.bottom))
        im.save(g.OUT / f'{name}.png')
        return im

    off_red = capture('off-red', '#ff2020')
    off_blue = capture('off-blue', '#2020ff')
    # At 200% DPI this is the Acrylic action in the sample's narrow app bar.
    # Derive its position from the actual window and scale, not screen pixels.
    scale = g.evidence()['surface']['devicePixelRatio']
    bounds = g.w.RECT()
    g.u.GetWindowRect(hwnd, c.byref(bounds))
    g.u.SetCursorPos(bounds.right - round(106 * scale), bounds.top + round(60 * scale))
    g.u.mouse_event(2, 0, 0, 0, 0)
    time.sleep(.15)
    g.u.mouse_event(4, 0, 0, 0, 0)
    time.sleep(1)
    on_red = capture('on-red', '#ff2020')
    on_blue = capture('on-blue', '#2020ff')
    # App bar blank area, below the native title bar and clear of controls.
    region = (round(265*scale), round(40*scale), round(305*scale), round(80*scale))
    def response(a, b):
        return ImageStat.Stat(ImageChops.difference(a.crop(region), b.crop(region))).mean[:3]
    off = response(off_red, off_blue)
    on = response(on_red, on_blue)
    result = dict(offBackgroundResponse=off, onBackgroundResponse=on,
                  physicalAppearance='notVerified', output=str(g.OUT))
    (g.OUT / 'acrylic-result.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
    print(json.dumps(result, indent=2), flush=True)
    assert max(off) < 3, 'Opaque control responds to background'
    assert max(on) > max(off) + 5, 'Acrylic did not reveal the desktop background'
    g.u.PostMessageW(hwnd, 0x10, 0, 0)
    assert process.wait(timeout=15) == 0
    assert not (g.OUT / 'evidence.json.exception.txt').exists()
    result['status'] = 'passed'
    result['cleanExit'] = True
    (g.OUT / 'acrylic-result.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
finally:
    g.u.SetCursorPos(cursor.x, cursor.y)
    if process is not None and process.poll() is None:
        if hwnd:
            g.u.PostMessageW(hwnd, 0x10, 0, 0)
        try:
            process.wait(timeout=10)
        except subprocess.TimeoutExpired:
            process.kill()
            process.wait(timeout=10)
    background.destroy()
