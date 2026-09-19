"""OS touch injection for the WebView gate; distinct from physical touch."""
import ctypes as c
from ctypes import wintypes as w
import time

class PointerInfo(c.Structure):
    _fields_ = [('type', w.UINT), ('id', w.UINT), ('frame', w.UINT), ('flags', w.UINT),
                ('device', w.HANDLE), ('target', w.HWND), ('pixel', w.POINT), ('himetric', w.POINT),
                ('rawPixel', w.POINT), ('rawHimetric', w.POINT), ('time', w.DWORD), ('history', w.UINT),
                ('input', c.c_int32), ('keys', w.DWORD), ('performance', c.c_uint64), ('button', c.c_int32)]

class Touch(c.Structure):
    _fields_ = [('info', PointerInfo), ('flags', w.UINT), ('mask', w.UINT), ('contact', w.RECT),
                ('rawContact', w.RECT), ('orientation', w.UINT), ('pressure', w.UINT)]

def tap(hwnd, x, y, scale):
    user = c.WinDLL('user32', use_last_error=True)
    user.InitializeTouchInjection.argtypes = [w.UINT, w.DWORD]
    user.InjectTouchInput.argtypes = [w.UINT, c.POINTER(Touch)]
    user.ClientToScreen.argtypes = [w.HWND, c.POINTER(w.POINT)]
    assert user.InitializeTouchInjection(1, 3), c.get_last_error()
    point = w.POINT(round(x * scale), round(y * scale))
    assert user.ClientToScreen(hwnd, c.byref(point))
    touch = Touch()
    touch.info.type = 2
    touch.info.id = 0
    touch.info.pixel = point
    touch.info.flags = 0x10000 | 2 | 4
    touch.mask = 7
    touch.contact = w.RECT(point.x - 2, point.y - 2, point.x + 2, point.y + 2)
    touch.orientation = 90
    touch.pressure = 32000
    assert user.InjectTouchInput(1, c.byref(touch)), (c.get_last_error(), point.x, point.y,
        [user.GetSystemMetrics(i) for i in (76, 77, 78, 79)])
    time.sleep(.1)
    touch.info.flags = 0x40000
    assert user.InjectTouchInput(1, c.byref(touch)), c.get_last_error()

def mouse_click(hwnd, x, y, scale):
    user = c.WinDLL('user32', use_last_error=True)
    user.ClientToScreen.argtypes = [w.HWND, c.POINTER(w.POINT)]
    point = w.POINT(round(x * scale), round(y * scale))
    assert user.ClientToScreen(hwnd, c.byref(point))
    assert user.SetCursorPos(point.x, point.y)
    time.sleep(.05)
    user.mouse_event(2, 0, 0, 0, 0)
    time.sleep(.1)
    user.mouse_event(4, 0, 0, 0, 0)
