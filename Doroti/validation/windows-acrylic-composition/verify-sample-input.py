"""Acrylic + animated PlatformView navigation/input gate; use a 1200s parent timeout."""
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
OUT = ROOT / 'Doroti/artifacts/validation/windows-platform-view-freeze' / time.strftime('%Y%m%d-%H%M%S')
OUT.mkdir(parents=True)
os.environ['DOROTI_PLATFORM_VIEW_GATE_OUTPUT'] = str(OUT)
spec = importlib.util.spec_from_file_location('gate', Path(__file__).with_name('verify.py'))
g = importlib.util.module_from_spec(spec)
spec.loader.exec_module(g)
g.u.SendMessageTimeoutW.argtypes = [w.HWND, w.UINT, w.WPARAM, w.LPARAM, w.UINT, w.UINT, c.POINTER(c.c_size_t)]
g.u.GetClientRect.argtypes = [w.HWND, c.POINTER(w.RECT)]
g.u.SetCursorPos.argtypes = [c.c_int, c.c_int]


def send(hwnd, msg, wp, lp):
    result = c.c_size_t()
    if not g.u.SendMessageTimeoutW(hwnd, msg, wp, lp, 2, 3000, c.byref(result)):
        raise TimeoutError(f'UI stopped responding to {msg:x}')
    return result.value


g.u.SendMessageW = send


class MouseInput(c.Structure):
    _fields_ = [('dx', w.LONG), ('dy', w.LONG), ('data', w.DWORD),
        ('flags', w.DWORD), ('time', w.DWORD), ('extra', c.c_size_t)]


class InputUnion(c.Union):
    _fields_ = [('mouse', MouseInput)]


class Input(c.Structure):
    _fields_ = [('type', w.DWORD), ('value', InputUnion)]


g.u.SendInput.argtypes = [w.UINT, c.POINTER(Input), c.c_int]


def mouse(flags, data=0):
    event = Input(0, InputUnion(MouseInput(0, 0, data & 0xffffffff, flags, 0, 0x444f5250)))
    assert g.u.SendInput(1, c.byref(event), c.sizeof(event)) == 1, 'SendInput failed'


def click(hwnd, x, y, scale):
    point = w.POINT(round(x * scale), round(y * scale))
    assert g.u.ClientToScreen(hwnd, c.byref(point))
    assert g.u.SetCursorPos(point.x, point.y)
    time.sleep(.08)
    mouse(2)
    try:
        # Deliberately hold across animation frames and composition commits.
        time.sleep(.15)
    finally:
        mouse(4)


def visible_rasters(hwnd):
    return sorted(child['hwnd'] for child in g.children(hwnd)
        if child['visible'] and child['name'] == 'Doroti GPU raster slice')


def main():
    env = os.environ.copy()
    env.pop('DOROTI_TESTBED_MODE', None)  # exact default gallery -> sample path
    env.update(DOROTI_PLATFORM_VIEW_EVIDENCE=str(OUT / 'frame.json'),
        DOROTI_WINDOWS_EXPERIMENTAL_ACRYLIC_READY_FILE=str(OUT / 'ready.json'),
        DOROTI_WINDOWS_APPSDK_DIAGNOSTICS='1', DOROTI_WINDOWS_APPSDK_REPORT=str(OUT / 'report.json'))
    print(OUT, flush=True)
    samples = []
    with (OUT / 'product.log').open('w') as log:
        process = subprocess.Popen([str(g.EXE)], cwd=g.EXE.parent, env=env, stdout=log, stderr=subprocess.STDOUT)
        hwnd = 0
        try:
            hwnd = g.wait_for(lambda: g.read(OUT / 'ready.json'), process)['hwnd']
            g.u.SetForegroundWindow(hwnd)
            g.u.SetWindowPos(hwnd, w.HWND(-1), 0, 0, 0, 0, 0x43)
            scale = g.u.GetDpiForWindow(hwnd) / 96
            rect, client = w.RECT(), w.RECT()
            g.u.GetWindowRect(hwnd, c.byref(rect)); g.u.GetClientRect(hwnd, c.byref(client))
            assert g.u.SetWindowPos(hwnd, None, 30, 30,
                round(720 * scale) + rect.right - rect.left - client.right,
                round(640 * scale) + rect.bottom - rect.top - client.bottom, 0x14)
            time.sleep(.5)
            click(hwnd, 100, 88, scale)  # Open Material sample
            time.sleep(.5)
            g.capture(hwnd, 'before-acrylic')
            click(hwnd, 620, 28, scale)
            time.sleep(.6)
            g.capture(hwnd, 'acrylic-on')
            for cycle in range(2):
                click(hwnd, 648, 590, scale)
                frame = g.wait_for(lambda: (v if (v := g.read(OUT / 'frame.json')) and len(v['native']) == 2 else None), process, 10)
                # Settle the navigation ripple before checking a fixed topology.
                time.sleep(.7)
                identities = visible_rasters(hwnd)
                assert identities, 'Platform raster siblings are missing'
                for _ in range(8):
                    commits = g.read(OUT / 'frame.json')['commits']
                    g.wait_for(lambda: (v if (v := g.read(OUT / 'frame.json')) and v['commits'] > commits else None), process, 5)
                    assert visible_rasters(hwnd) == identities, 'Animation replaced the visible HWNDs beneath the pointer'
                    send(hwnd, 0, 0, 0)
                frame = g.read(OUT / 'frame.json')
                scene_y = frame['native'][0]['transform']['Dy']
                click(hwnd, 70, scene_y - 16, scale)
                frame = g.wait_for(lambda: (v if (v := g.read(OUT / 'frame.json')) and g.probe(v)[0] == 6 else None), process, 5)
                taps = g.probe(frame)[1]
                click(hwnd, 280, scene_y + 130, scale)
                g.wait_for(lambda: (v if (v := g.read(OUT / 'frame.json')) and g.probe(v)[1] == taps + 1 else None), process, 5)
                editor = frame['native'][1]['hwnd']
                click(hwnd, 380, scene_y + 100, scale)
                g.wait_for(lambda: g.focus(hwnd) == editor, process, 5)
                # Actual wheel input must continue reaching the framework from a raster sibling.
                point = w.POINT(round(500 * scale), round((scene_y + 110) * scale))
                g.u.ClientToScreen(hwnd, c.byref(point)); g.u.SetCursorPos(point.x, point.y)
                mouse(0x800, -120)
                frame = g.wait_for(lambda: (v if (v := g.read(OUT / 'frame.json')) and
                    v['native'][0]['transform']['Dy'] < scene_y else None), process, 5)
                g.capture(hwnd, f'platform-interaction-{cycle}')
                samples.append(dict(cycle=cycle, stableRasterWindows=identities, timings=frame['timings'],
                    overlapClick=True, foregroundClick=True, nativeFocus=True, wheelScroll=True))
                click(hwnd, 72, 590, scale)
                g.wait_for(lambda: (v if (v := g.read(OUT / 'frame.json')) and not v['native'] else None), process, 5)
                time.sleep(.3)
        except Exception:
            if hwnd: g.capture(hwnd, 'failure')
            raise
        finally:
            if hwnd: g.u.PostMessageW(hwnd, 16, 0, 0)
            try: code = process.wait(timeout=20)
            except subprocess.TimeoutExpired:
                process.kill(); process.wait()
                raise RuntimeError('Product shutdown exceeded 20 seconds')
        assert code == 0, f'Product exited with {code}'
    result = dict(status='passed', acrylic=True, inputTransport='OS SendInput; 150ms button hold',
        samples=samples, closeExitCode=code, physicalHumanInput='notVerified', artifacts=str(OUT))
    (OUT / 'result.json').write_text(json.dumps(result, indent=2))
    print(json.dumps(result), flush=True)


if __name__ == '__main__':
    main()
