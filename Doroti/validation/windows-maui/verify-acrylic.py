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
g.u.GetForegroundWindow.restype = g.w.HWND
g.u.GetCursorPos.argtypes = [c.POINTER(g.w.POINT)]
g.u.ClientToScreen.argtypes = [g.w.HWND, c.POINTER(g.w.POINT)]
dwm = c.WinDLL('dwmapi')
dwm.DwmGetWindowAttribute.argtypes = [g.w.HWND, g.w.DWORD, c.c_void_p, g.w.DWORD]
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
        # Tk may activate its background while servicing the color change.
        # Restore this test's app before sampling its active material.
        g.u.SetForegroundWindow(hwnd)
        time.sleep(.8)
        assert g.u.GetForegroundWindow() == hwnd, 'Test window lost foreground; capture aborted'
        bounds = g.w.RECT()
        g.u.GetWindowRect(hwnd, c.byref(bounds))
        im = ImageGrab.grab((bounds.left, bounds.top, bounds.right, bounds.bottom), include_layered_windows=True)
        im.save(g.OUT / f'{name}.png')
        return im

    off_red = capture('off-red', '#ff2020')
    off_blue = capture('off-blue', '#2020ff')
    # At 200% DPI this is the Acrylic action in the sample's narrow app bar.
    # Derive its position from the actual window and scale, not screen pixels.
    scale = g.evidence()['surface']['devicePixelRatio']
    bounds = g.w.RECT()
    g.u.GetWindowRect(hwnd, c.byref(bounds))
    assert g.u.GetForegroundWindow() == hwnd, 'Test window lost foreground; input aborted'
    g.u.SetCursorPos(bounds.right - round(106 * scale), bounds.top + round(60 * scale))
    g.u.mouse_event(2, 0, 0, 0, 0)
    time.sleep(.15)
    g.u.mouse_event(4, 0, 0, 0, 0)
    time.sleep(1)
    on_red = capture('on-red', '#ff2020')
    on_blue = capture('on-blue', '#2020ff')
    # App bar blank area, below the native title bar and clear of controls.
    region = (round(265*scale), round(40*scale), round(305*scale), round(80*scale))
    client_origin = g.w.POINT()
    assert g.u.ClientToScreen(hwnd, c.byref(client_origin))
    caption_height = client_origin.y - bounds.top
    assert caption_height > 20 * scale, 'Native caption is missing'
    caption_region = (round(265*scale), round(caption_height*.25),
                      round(305*scale), round(caption_height*.7))
    def response(a, b, region=region):
        return ImageStat.Stat(ImageChops.difference(a.crop(region), b.crop(region))).mean[:3]
    off = response(off_red, off_blue)
    on = response(on_red, on_blue)
    # The sample toggle changes client opacity; the window backdrop remains
    # Acrylic in both states, including the standard non-client caption.
    caption_off = response(off_red, off_blue, caption_region)
    caption_on = response(on_red, on_blue, caption_region)
    backdrop_type = g.w.DWORD()
    assert dwm.DwmGetWindowAttribute(hwnd, 38, c.byref(backdrop_type), c.sizeof(backdrop_type)) == 0
    result = dict(offBackgroundResponse=off, onBackgroundResponse=on,
                  captionOffBackgroundResponse=caption_off,
                  captionOnBackgroundResponse=caption_on,
                  captionRegion=caption_region, systemBackdropType=backdrop_type.value,
                  status='failed', physicalAppearance='notVerified', output=str(g.OUT))
    (g.OUT / 'acrylic-result.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
    assert max(off) < 3, 'Opaque control responds to background'
    assert max(on) > max(off) + 5, 'Acrylic did not reveal the desktop background'
    caption_mode = os.environ.get('DOROTI_DESKTOP_CAPTION', 'Backdrop').lower()
    result['captionMode'] = caption_mode
    if caption_mode == 'backdrop':
        assert backdrop_type.value == 3, 'Native caption did not select Desktop Acrylic'
        assert max(caption_off) > 5 and max(caption_on) > 5, \
            'Native caption did not reveal the desktop background'
    else:
        assert caption_mode in ('solid', 'system'), 'Unknown validation caption mode'
        assert backdrop_type.value == 1, 'System/Solid caption retained Acrylic'
        assert max(caption_off) < 3 and max(caption_on) < 3, 'Opaque caption responds to desktop background'
    assert g.evidence()['nativePointerEvents'] > 0, 'No native pointer ingress'
    g.u.PostMessageW(hwnd, 0x10, 0, 0)
    assert process.wait(timeout=15) == 0
    assert not (g.OUT / 'evidence.json.exception.txt').exists()
    result['status'] = 'passed'
    result['cleanExit'] = True
    (g.OUT / 'acrylic-result.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
    print(json.dumps(result, indent=2), flush=True)
except Exception as error:
    result_path = g.OUT / 'acrylic-result.json'
    if not result_path.exists():
        result_path.write_text(json.dumps(dict(status='notMeasured', reason=str(error),
                                              physicalAppearance='notVerified', output=str(g.OUT)), indent=2), encoding='utf-8')
    raise
finally:
    g.u.mouse_event(4, 0, 0, 0, 0)
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
