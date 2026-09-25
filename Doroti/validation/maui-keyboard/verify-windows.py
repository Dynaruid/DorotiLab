"""MAUI native keyboard -> framework/IME fixture. Run under run-with-timeout.py."""
import ctypes as c
from ctypes import wintypes as w
import json
import os
from pathlib import Path
import subprocess
import time

ROOT = Path(__file__).resolve().parents[3]
OUT = ROOT / 'Doroti/artifacts/validation/maui-keyboard/windows'
OUT.mkdir(parents=True, exist_ok=True)
REPORT = OUT / 'events.jsonl'
REPORT.unlink(missing_ok=True)
EXE = ROOT / 'DorotiTestbedApp/windows/bin/Release/net10.0-windows10.0.19041.0/win-x64/DorotiTestbedApp.Windows.exe'
u = c.WinDLL('user32', use_last_error=True)
u.SetProcessDpiAwarenessContext.argtypes = [w.HANDLE]
u.SetProcessDpiAwarenessContext(w.HANDLE(-4))
ENUM = c.WINFUNCTYPE(w.BOOL, w.HWND, w.LPARAM)
u.EnumWindows.argtypes = [ENUM, w.LPARAM]
u.GetWindowThreadProcessId.argtypes = [w.HWND, c.POINTER(w.DWORD)]
u.IsWindowVisible.argtypes = [w.HWND]
u.SetForegroundWindow.argtypes = [w.HWND]
u.ClientToScreen.argtypes = [w.HWND, c.POINTER(w.POINT)]
u.GetDpiForWindow.argtypes = [w.HWND]
u.PostMessageW.argtypes = [w.HWND, w.UINT, w.WPARAM, w.LPARAM]
u.CreateWindowExW.argtypes = [w.DWORD, w.LPCWSTR, w.LPCWSTR, w.DWORD, c.c_int, c.c_int, c.c_int, c.c_int, w.HWND, w.HMENU, w.HINSTANCE, w.LPVOID]
u.CreateWindowExW.restype = w.HWND
u.DestroyWindow.argtypes = [w.HWND]
class Message(c.Structure):
    _fields_ = [('hwnd', w.HWND), ('id', w.UINT), ('wp', w.WPARAM), ('lp', w.LPARAM),
                ('time', w.DWORD), ('point', w.POINT), ('private', w.DWORD)]
u.PeekMessageW.argtypes = [c.POINTER(Message), w.HWND, w.UINT, w.UINT, w.UINT]
u.DispatchMessageW.argtypes = [c.POINTER(Message)]

def pump():
    message = Message()
    while u.PeekMessageW(c.byref(message), None, 0, 0, 1):
        u.DispatchMessageW(c.byref(message))

class Mouse(c.Structure):
    _fields_ = [('x', w.LONG), ('y', w.LONG), ('data', w.DWORD), ('flags', w.DWORD), ('time', w.DWORD), ('extra', c.c_size_t)]
class Keyboard(c.Structure):
    _fields_ = [('vk', w.WORD), ('scan', w.WORD), ('flags', w.DWORD), ('time', w.DWORD), ('extra', c.c_size_t)]
class Value(c.Union):
    _fields_ = [('mouse', Mouse), ('key', Keyboard)]
class Input(c.Structure):
    _fields_ = [('kind', w.DWORD), ('value', Value)]
u.SendInput.argtypes = [w.UINT, c.POINTER(Input), c.c_int]

def send(event):
    assert u.SendInput(1, c.byref(event), c.sizeof(event)) == 1
    time.sleep(.08)

def key(scan, up=False, extended=False):
    send(Input(1, Value(key=Keyboard(0, scan, 8 | (2 if up else 0) | int(extended), 0, 0))))

def click(hwnd, x, y):
    scale = u.GetDpiForWindow(hwnd) / 96
    point = w.POINT(round(x * scale), round(y * scale))
    assert u.ClientToScreen(hwnd, c.byref(point))
    u.SetCursorPos(point.x, point.y)
    send(Input(0, Value(mouse=Mouse(0, 0, 0, 2, 0, 0))))
    send(Input(0, Value(mouse=Mouse(0, 0, 0, 4, 0, 0))))

def events():
    try:
        return [json.loads(line) for line in REPORT.read_text(encoding='utf-8-sig').splitlines()]
    except (OSError, ValueError):
        return []

def wait(check, timeout=30):
    until = time.monotonic() + timeout
    while time.monotonic() < until:
        pump()
        value = check()
        if value: return value
        time.sleep(.05)
    raise TimeoutError('Keyboard fixture did not reach expected state')

def window(pid):
    found = []
    @ENUM
    def visit(hwnd, _):
        owner = w.DWORD()
        u.GetWindowThreadProcessId(hwnd, c.byref(owner))
        if owner.value == pid and u.IsWindowVisible(hwnd): found.append(hwnd)
        return True
    u.EnumWindows(visit, 0)
    return found[0] if found else None

env = dict(os.environ, DOROTI_TESTBED_MODE='keyboard-input', DOROTI_KEYBOARD_REPORT=str(REPORT),
           DOROTI_MAUI_EVIDENCE=str(OUT / 'evidence.json'))
hwnd = companion = 0
input_passed = False
with (OUT / 'product.log').open('w', encoding='utf-8') as log:
    process = subprocess.Popen([str(EXE)], cwd=EXE.parent, env=env, stdout=log, stderr=log)
    try:
        hwnd = wait(lambda: window(process.pid))
        wait(lambda: events())
        u.SetForegroundWindow(hwnd)
        time.sleep(.5)
        click(hwnd, 360, 45)
        time.sleep(.5)
        key(0x1e)
        wait(lambda: any(e.get('physical') == 0x70004 and e.get('type') == 'KeyDownEvent' for e in events()), 5)
        key(0x1e)
        wait(lambda: any(e.get('physical') == 0x70004 and e.get('type') == 'KeyRepeatEvent' for e in events()), 5)
        key(0x1e, up=True)
        a = wait(lambda: (v if len(v := [e for e in events() if e.get('physical') == 0x70004]) >= 3 else None))
        assert [e['type'] for e in a] == ['KeyDownEvent', 'KeyRepeatEvent', 'KeyUpEvent'], a
        key(0x1c, extended=True); key(0x1c, up=True, extended=True)
        wait(lambda: any(e.get('physical') == 0x70058 and e.get('logical') == 0x20000020d for e in events()))
        key(0x2a)
        companion = u.CreateWindowExW(0, 'STATIC', 'Doroti keyboard activation probe', 0x10cf0000, 20, 20, 240, 100, None, None, None, None)
        assert companion and u.SetForegroundWindow(companion)
        wait(lambda: any(e.get('physical') == 0x700e1 and e.get('synthesized') and e.get('type') == 'KeyUpEvent' for e in events()), 5)
        key(0x2a, up=True)
        u.DestroyWindow(companion); companion = 0
        u.SetForegroundWindow(hwnd)
        time.sleep(.3)
        click(hwnd, 100, 100)
        time.sleep(.5)
        for char in 'abc':
            for flag in (4, 6): send(Input(1, Value(key=Keyboard(0, ord(char), flag, 0, 0))))
        wait(lambda: any(e.get('kind') == 'text' and e.get('value') == 'abc' for e in events()), 5)
        key(0x0e); key(0x0e, up=True)
        wait(lambda: any(e.get('kind') == 'text' and e.get('value') == 'ab' for e in events()), 5)
        input_passed = True
        if os.environ.get('DOROTI_KEYBOARD_BLUR_BEFORE_CLOSE') == '1':
            click(hwnd, 360, 45)
            time.sleep(.5)
    finally:
        if companion: u.DestroyWindow(companion)
        # Always release modifiers injected by this fixture.
        key(0x2a, up=True); key(0x36, up=True)
        if hwnd: u.PostMessageW(hwnd, 0x10, 0, 0)
        try:
            wait(lambda: process.poll() is not None, 20)
            code = process.returncode
        except TimeoutError:
            stack = ROOT / 'Doroti/artifacts/diagnostics-tools/dotnet-stack.exe'
            if stack.exists():
                with (OUT / 'shutdown-stack.txt').open('w') as output:
                    subprocess.run([str(stack), 'report', '-p', str(process.pid)], stdout=output, stderr=output, timeout=15)
            process.kill(); process.wait()
            (OUT / 'result.json').write_text(json.dumps(dict(status='partial' if input_passed else 'failed',
                inputPassed=input_passed, shutdown='timeout', transport='OS SendInput',
                physicalKeyboard='notVerified'), indent=2))
            raise RuntimeError('MAUI shutdown timed out')
    assert code == 0, code
result = dict(status='passed', transport='OS SendInput', repeat=True, keypadEnter=True,
              focusLossRelease=True, nativeTextAndBackspace=True, exitCode=code,
              physicalKeyboard='notVerified')
(OUT / 'result.json').write_text(json.dumps(result, indent=2))
print(json.dumps(result))
