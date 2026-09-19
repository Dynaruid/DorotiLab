"""Post-scroll WinUI/framework input regression; run with a 1200-second parent timeout."""
import ctypes as c
from ctypes import wintypes as w
import importlib.util
import json
import os
from pathlib import Path
import subprocess
import sys
import time

sys.dont_write_bytecode = True
ROOT = Path(__file__).resolve().parents[3]
OUT = Path(sys.argv[1]).resolve() if len(sys.argv) > 1 else ROOT / 'Doroti/artifacts/windows-winui-input'
OUT.mkdir(parents=True, exist_ok=True)
os.environ['DOROTI_PLATFORM_VIEW_GATE_OUTPUT'] = str(OUT)
spec = importlib.util.spec_from_file_location('gate', Path(__file__).parents[1] / 'windows-acrylic-composition/verify.py')
g = importlib.util.module_from_spec(spec)
spec.loader.exec_module(g)
g.u.SendMessageTimeoutW.argtypes = [w.HWND, w.UINT, w.WPARAM, w.LPARAM, w.UINT, w.UINT, c.POINTER(c.c_size_t)]

def send(hwnd, msg, wp=0, lp=0):
    result = c.c_size_t()
    if not g.u.SendMessageTimeoutW(hwnd, msg, wp, lp, 2, 3000, c.byref(result)):
        raise TimeoutError(f'UI stopped responding: hwnd={hwnd}, message={msg:x}')
    return result.value

g.u.SendMessageW = send

class MouseInput(c.Structure):
    _fields_ = [('dx', w.LONG), ('dy', w.LONG), ('data', w.DWORD),
        ('flags', w.DWORD), ('time', w.DWORD), ('extra', c.c_size_t)]
class KeyboardInput(c.Structure):
    _fields_ = [('vk', w.WORD), ('scan', w.WORD), ('flags', w.DWORD), ('time', w.DWORD), ('extra', c.c_size_t)]
class InputUnion(c.Union):
    _fields_ = [('mouse', MouseInput), ('keyboard', KeyboardInput)]
class Input(c.Structure):
    _fields_ = [('type', w.DWORD), ('value', InputUnion)]
g.u.SendInput.argtypes = [w.UINT, c.POINTER(Input), c.c_int]
g.u.SetCursorPos.argtypes = [c.c_int, c.c_int]

def mouse(flags, data=0):
    event = Input(0, InputUnion(MouseInput(0, 0, data & 0xffffffff, flags, 0, 0x444f5250)))
    assert g.u.SendInput(1, c.byref(event), c.sizeof(event)) == 1

def move(hwnd, x, y, scale):
    point = w.POINT(round(x * scale), round(y * scale))
    assert g.u.ClientToScreen(hwnd, c.byref(point))
    assert g.u.SetCursorPos(point.x, point.y)
    time.sleep(.1)

def click(hwnd, x, y, scale):
    move(hwnd, x, y, scale)
    mouse(2)
    try: time.sleep(.15)
    finally: mouse(4)

def type_text(text):
    # XAML focus telemetry can precede completion of the pointer-up/input handoff.
    # Exercise normal typing after that handoff, rather than a zero-time burst.
    time.sleep(.15)
    for char in text:
        for flags in (4, 6):
            event = Input(1, InputUnion(keyboard=KeyboardInput(0, ord(char), flags, 0, 0x444f5250)))
            assert g.u.SendInput(1, c.byref(event), c.sizeof(event)) == 1

def main():
    env = os.environ.copy()
    for name in ('DOROTI_TESTBED_MODE', 'DOROTI_WINDOWS_APPSDK_SMOKE_MS', 'DOROTI_WINDOWS_PLATFORM_CAPTURE',
            'DOROTI_PLATFORM_VIEW_COMPOSITION', 'DOROTI_WINUI_BACKDROP_OMIT_NATIVE_PROBE'):
        env.pop(name, None)
    env.update(DOROTI_PLATFORM_VIEW_EVIDENCE=str(OUT / 'frame.json'),
        DOROTI_TESTBED_NATIVE_PAGE_PROBE='1',
        DOROTI_PLATFORM_VIEW_BACKDROP='1',
        DOROTI_WINDOWS_EXPERIMENTAL_ACRYLIC_READY_FILE=str(OUT / 'ready.json'))
    for name in ('ready.json', 'frame.json', 'result.json'):
        (OUT / name).unlink(missing_ok=True)
    hwnd = 0
    with (OUT / 'product.log').open('w') as log:
        process = subprocess.Popen([str(g.EXE)], cwd=g.EXE.parent, env=env, stdout=log, stderr=subprocess.STDOUT)
        try:
            hwnd = g.wait_for(lambda: g.read(OUT / 'ready.json'), process)['hwnd']
            frame = g.wait_for(lambda: (v if (v := g.read(OUT / 'frame.json')) and len(v['native']) == 2 and v['commits'] > 20 else None), process)
            g.u.SetForegroundWindow(hwnd)
            g.u.SetWindowPos(hwnd, w.HWND(-1), 30, 30, 0, 0, 0x41)
            scale = frame['dpi'] / 96
            before = frame['native'][0]['transform']['Dy']
            (OUT / 'before.json').write_text(json.dumps(frame, indent=2))
            (OUT / 'children.json').write_text(json.dumps(g.children(hwnd), indent=2))
            click(hwnd, 70, before - 16, scale)
            g.wait_for(lambda: (v if (v := g.read(OUT / 'frame.json')) and g.probe(v)[0] == 6 else None), process, 5)
            print('pre-scroll click passed', flush=True)
            print(f'wheel over raster, before={before}', flush=True)
            move(hwnd, 500, before + 110, scale)
            mouse(0x800, -120)
            frame = g.wait_for(lambda: (v if (v := g.read(OUT / 'frame.json')) and v['native'] and v['native'][0]['transform']['Dy'] < before else None), process, 8)
            print(f'scrolled: {frame["native"][0]["transform"]["Dy"]}', flush=True)
            time.sleep(.5)
            send(hwnd, 0)
            frame = g.read(OUT / 'frame.json')
            stage = g.probe(frame)[0]
            y = frame['native'][0]['transform']['Dy']
            taps = g.probe(frame)[1]
            click(hwnd, 280, y + 130, scale)
            g.wait_for(lambda: (v if (v := g.read(OUT / 'frame.json')) and g.probe(v)[1] > taps else None), process, 5)
            print('post-scroll blur foreground click passed', flush=True)
            clicks = frame['winUi'][0]['clicks']
            click(hwnd, 110, y + 40, scale)
            g.wait_for(lambda: (v if (v := g.read(OUT / 'frame.json')) and v['winUi'][0]['clicks'] > clicks else None), process, 5)
            print('post-scroll native button passed', flush=True)
            click(hwnd, 380, y + 100, scale)
            g.wait_for(lambda: (v if (v := g.read(OUT / 'frame.json')) and v['winUi'][1]['hasFocus'] else None), process, 5)
            type_text('scroll-ok')
            g.wait_for(lambda: (v if (v := g.read(OUT / 'frame.json')) and 'scroll-ok' in v['winUi'][1]['text'] else None), process, 5)
            print('post-scroll native editing passed', flush=True)
            click(hwnd, 70, y - 16, scale)
            frame = g.wait_for(lambda: (v if (v := g.read(OUT / 'frame.json')) and g.probe(v)[0] != stage else None), process, 8)
            print('post-scroll scenario click passed', flush=True)
            # Stage 7 removes the foreground shield: blur must pass through to XAML.
            click(hwnd, 280, y + 130, scale)
            g.wait_for(lambda: (v if (v := g.read(OUT / 'frame.json')) and v['winUi'][1]['hasFocus'] else None), process, 5)
            type_text('-through-blur')
            g.wait_for(lambda: (v if (v := g.read(OUT / 'frame.json')) and '-through-blur' in v['winUi'][1]['text'] else None), process, 5)
            move(hwnd, 110, y + 40, scale)
            mouse(0x800, -120)
            frame = g.wait_for(lambda: (v if (v := g.read(OUT / 'frame.json')) and v['native'] and v['native'][0]['transform']['Dy'] < y else None), process, 5)
            print('native wheel and blur pass-through passed', flush=True)
            for delta in [-120] * 10 + [120] * 14:
                move(hwnd, 500, 260, scale)
                mouse(0x800, delta)
                time.sleep(.12)
                send(hwnd, 0)
            frame = g.wait_for(lambda: (v if (v := g.read(OUT / 'frame.json')) and v['native'] and abs(v['native'][0]['transform']['Dy'] - before) < 1 else None), process, 8)
            (OUT / 'after-scroll.json').write_text(json.dumps(frame, indent=2))
            print('offscreen scroll and return passed', flush=True)
            click(hwnd, 72, 590, scale)
            g.wait_for(lambda: (v if (v := g.read(OUT / 'frame.json')) and not v['native'] else None), process, 8)
            # Six current destinations: Platform views is index four; the last
            # destination now opens the independently added WebView sample.
            click(hwnd, 540, 590, scale)
            frame = g.wait_for(lambda: (v if (v := g.read(OUT / 'frame.json')) and len(v['native']) == 2 else None), process, 8)
            time.sleep(.5)
            click(hwnd, 110, frame['native'][0]['transform']['Dy'] + 40, scale)
            g.wait_for(lambda: (v if (v := g.read(OUT / 'frame.json')) and v['winUi'][0]['clicks'] > 0 else None), process, 5)
            print('remount native interaction passed', flush=True)
        except Exception as error:
            (OUT / 'result.json').write_text(json.dumps(dict(status='failed', error=str(error)), indent=2))
            raise
        finally:
            if hwnd: g.u.PostMessageW(hwnd, 16, 0, 0)
            try: code = process.wait(timeout=15)
            except subprocess.TimeoutExpired:
                process.kill(); process.wait()
                raise RuntimeError('Product shutdown exceeded 15 seconds')
        assert code == 0, f'Product exited with {code}'
    result = dict(status='passed', wheelScroll=True, postScrollClick=True, postScrollNavigation=True,
        nativeButton=True, nativeEditing=True, nativeWheel=True, blurForegroundClick=True,
        blurNativePassThrough=True, offscreenScrollAndReturn=True, remountInput=True,
        inputTransport='OS SendInput; 150ms button hold', physicalHumanInput='notVerified')
    (OUT / 'result.json').write_text(json.dumps(result, indent=2))
    print(json.dumps(result), flush=True)

if __name__ == '__main__': main()
