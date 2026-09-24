"""Live MAUI Graphite resize regression; run through run-with-timeout.py.

Synthetic Win32 resizes verify frame continuity and resource lifetime, not
physical edge-drag smoothness or display latency. Never target an existing app.
"""
import ctypes as c
from ctypes import wintypes as w
import datetime
import json
import os
from pathlib import Path
import subprocess
import time
from PIL import ImageGrab

ROOT = Path(__file__).resolve().parents[3]
OUT = ROOT / 'Doroti/artifacts/validation/windows-maui' / datetime.datetime.now().strftime('%Y%m%d-%H%M%S')
OUT.mkdir(parents=True)
EXE = ROOT / 'DorotiTestbedApp/windows/bin/x64/Release/net10.0-windows10.0.19041.0/win-x64/DorotiTestbedApp.Windows.exe'
u = c.WinDLL('user32', use_last_error=True)
u.SetProcessDpiAwarenessContext.argtypes = [w.HANDLE]
u.SetProcessDpiAwarenessContext(w.HANDLE(-4))
u.GetWindowThreadProcessId.argtypes = [w.HWND, c.POINTER(w.DWORD)]
u.IsWindowVisible.argtypes = [w.HWND]
u.GetWindowRect.argtypes = [w.HWND, c.POINTER(w.RECT)]
u.GetClientRect.argtypes = [w.HWND, c.POINTER(w.RECT)]
u.SetWindowPos.argtypes = [w.HWND, w.HWND, c.c_int, c.c_int, c.c_int, c.c_int, w.UINT]
u.PostMessageW.argtypes = [w.HWND, w.UINT, w.WPARAM, w.LPARAM]
u.SetForegroundWindow.argtypes = [w.HWND]
ENUM = c.WINFUNCTYPE(w.BOOL, w.HWND, w.LPARAM)
u.EnumWindows.argtypes = [ENUM, w.LPARAM]


def window_for(pid):
    found = []
    @ENUM
    def visit(hwnd, _):
        owner = w.DWORD()
        u.GetWindowThreadProcessId(hwnd, c.byref(owner))
        if owner.value == pid and u.IsWindowVisible(hwnd):
            found.append(hwnd)
        return True
    u.EnumWindows(visit, 0)
    return found[0] if found else None


def wait_until(check, seconds=30):
    deadline = time.monotonic() + seconds
    while time.monotonic() < deadline:
        value = check()
        if value:
            return value
        time.sleep(.05)
    raise TimeoutError('Timed out waiting for MAUI window/frame evidence')


def evidence():
    try:
        return json.loads((OUT / 'evidence.json').read_text(encoding='utf-8-sig'))
    except (OSError, ValueError):
        return None


def main():
    env = dict(os.environ, DOROTI_MAUI_EVIDENCE=str(OUT / 'evidence.json'),
               DOROTI_WINDOWS_MAUI_GRAPHITE='1', DOROTI_TESTBED_MODE='sample')
    hwnd = None
    with (OUT / 'process.log').open('w', encoding='utf-8') as log:
        process = subprocess.Popen([str(EXE)], cwd=EXE.parent, env=env, stdout=log, stderr=log)
        try:
            hwnd = wait_until(lambda: window_for(process.pid))
            u.SetForegroundWindow(hwnd)
            before = wait_until(lambda: (e := evidence()) and e['frame']['presented'] > 0 and e)
            initial_client = w.RECT()
            u.GetClientRect(hwnd, c.byref(initial_client))
            titlebar_inset = initial_client.bottom - before['surface']['pixelHeight']
            (OUT / 'before.json').write_text(json.dumps(before, indent=2), encoding='utf-8')
            for i in range(24):
                if i == 23:
                    # Evidence writes are throttled to one second. Leave a
                    # full interval before the final size so its snapshot is
                    # not suppressed by the preceding moving-frame write.
                    time.sleep(1.25)
                # Includes moving-origin and fixed-origin growth/shrinkage.
                step = i if i < 12 else 23 - i
                assert u.SetWindowPos(hwnd, None, 100 + step * 4, 100 + step * 3,
                                      960 + step * 12, 700 + step * 7, 0x14)
                time.sleep(.05)
            client = w.RECT()
            u.GetClientRect(hwnd, c.byref(client))
            bounds = w.RECT()
            u.GetWindowRect(hwnd, c.byref(bounds))
            time.sleep(1)
            ImageGrab.grab((bounds.left, bounds.top, bounds.right, bounds.bottom)).save(OUT / 'after-resize.png')
            print(f'output={OUT}; client={client.right}x{client.bottom}; process={process.poll()}', flush=True)
            after = wait_until(lambda: (e := evidence()) and
                               e['surface']['pixelWidth'] == client.right and
                               e['surface']['surfaceGeneration'] > before['surface']['surfaceGeneration'] + 3 and
                               e['frame']['presented'] > before['frame']['presented'] + 3 and e)
            time.sleep(1)
            after = evidence() or after
            surface = after['surface']
            trace = surface['resizeTrace']
            targets = [x for x in trace if x['phase'] == 'target']
            presents = [x for x in trace if x['phase'] == 'post-swap']
            ready = [x for x in trace if x['phase'] == 'surface-ready']
            assert len({x['epoch']['generation'] for x in presents}) > 4, 'No continuous resize presents'
            assert presents[-1]['epoch'] == targets[-1]['epoch'], 'Final frame is stale'
            assert surface['pixelWidth'] == targets[-1]['epoch']['physicalWidth']
            assert surface['pixelHeight'] == targets[-1]['epoch']['physicalHeight']
            assert surface['pixelHeight'] == client.bottom - titlebar_inset, 'Final height differs from content bounds'
            assert all('graphiteDeviceCreations=1;' in x['detail'] for x in ready), 'Device recreated during resize'
            assert after['frame']['failed'] == 0, after['frame']
            assert not (OUT / 'evidence.json.exception.txt').exists(), 'Renderer exception'
            bounds = w.RECT()
            u.GetWindowRect(hwnd, c.byref(bounds))
            ImageGrab.grab((bounds.left, bounds.top, bounds.right, bounds.bottom)).save(OUT / 'final.png')
            durations = sorted(x['durationMicroseconds'] for x in ready[1:])
            result = dict(status='passed', resizes=24, resizeTargets=len(targets),
                          presentedGenerations=len({x['epoch']['generation'] for x in presents}),
                          graphiteDeviceCreations=1, finalPhysical=[surface['pixelWidth'], surface['pixelHeight']],
                          surfacePrepareP95Microseconds=durations[int((len(durations)-1)*.95)] if durations else None,
                          physicalDrag='notVerified', acrylicAppearance='notVerified')
            u.PostMessageW(hwnd, 0x10, 0, 0)
            assert process.wait(timeout=15) == 0, 'Unclean exit'
            assert not (OUT / 'evidence.json.exception.txt').exists(), 'Shutdown exception'
            result['cleanExit'] = True
            (OUT / 'result.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
            print(json.dumps(dict(result, output=str(OUT)), indent=2))
        finally:
            if process.poll() is None:
                if hwnd:
                    u.PostMessageW(hwnd, 0x10, 0, 0)
                try:
                    process.wait(timeout=10)
                except subprocess.TimeoutExpired:
                    process.kill()
                    process.wait(timeout=10)


if __name__ == '__main__':
    main()
