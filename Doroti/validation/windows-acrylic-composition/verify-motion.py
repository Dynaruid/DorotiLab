"""Bounded mounted window-drag/scroll measurement; use the 1200s parent wrapper."""
import importlib.util
import json
import os
from pathlib import Path
import subprocess
import sys
import time

sys.dont_write_bytecode = True
spec = importlib.util.spec_from_file_location('sample', Path(__file__).with_name('verify-sample-input.py'))
s = importlib.util.module_from_spec(spec)
spec.loader.exec_module(s)
g, c, w, OUT = s.g, s.c, s.w, s.OUT


def main():
    env = os.environ.copy()
    env.pop('DOROTI_TESTBED_MODE', None)
    env.update(DOROTI_PLATFORM_VIEW_EVIDENCE=str(OUT / 'frame.json'),
        DOROTI_WINDOWS_EXPERIMENTAL_ACRYLIC_READY_FILE=str(OUT / 'ready.json'))
    print(OUT, flush=True)
    hwnd = 0
    with (OUT / 'product.log').open('w', encoding='utf-8') as log:
        process = subprocess.Popen([str(g.EXE)], cwd=g.EXE.parent, env=env, stdout=log, stderr=subprocess.STDOUT)
        try:
            hwnd = g.wait_for(lambda: g.read(OUT / 'ready.json'), process)['hwnd']
            g.u.SetWindowPos(hwnd, w.HWND(-1), 30, 30, 0, 0, 0x41)
            scale = g.u.GetDpiForWindow(hwnd) / 96
            time.sleep(.5)
            s.click(hwnd, 100, 88, scale)
            time.sleep(.5)
            s.click(hwnd, 620, 28, scale)
            time.sleep(.5)
            s.click(hwnd, 648, 590, scale)
            g.wait_for(lambda: (f if (f := g.read(OUT / 'frame.json')) and len(f['native']) == 2 else None), process)
            time.sleep(1)
            samples = []
            def measure(name, action):
                first = g.read(OUT / 'frame.json')
                started = time.monotonic()
                delays = []
                window_left, native_top = [], []
                for i in range(48):
                    tick = time.monotonic()
                    action(i)
                    s.send(hwnd, 0, 0, 0)
                    delays.append((time.monotonic() - tick) * 1000)
                    position = w.RECT()
                    assert g.u.GetWindowRect(hwnd, c.byref(position))
                    window_left.append(position.left)
                    current = g.read(OUT / 'frame.json')
                    if current and current['native']:
                        native_top.append(current['native'][0]['transform']['Dy'])
                    time.sleep(.016)
                elapsed = time.monotonic() - started
                last = g.read(OUT / 'frame.json')
                commits = last['commits'] - first['commits']
                samples.append(dict(name=name, seconds=elapsed, backendCommits=commits,
                    commitsPerSecond=commits / elapsed, inputRoundTripP95Ms=sorted(delays)[45],
                    readbackBytesPerCommit=(last['readbackBytes'] - first['readbackBytes']) / max(1, commits),
                    windowTravelPixels=max(window_left) - min(window_left),
                    nativeScrollTravel=max(native_top) - min(native_top),
                    timings=last['timings'], frame=last))
                assert commits > 0
                if name == 'titlebar-drag':
                    assert max(window_left) - min(window_left) >= 60, 'Titlebar input did not move the window'
                else:
                    assert max(native_top) - min(native_top) >= 10, 'Wheel input did not scroll the native scene'
            rect = w.RECT()
            assert g.u.GetWindowRect(hwnd, c.byref(rect))
            x, y = rect.left + 250, rect.top + 24
            g.u.SetCursorPos(x, y)
            s.mouse(2)
            try:
                measure('titlebar-drag', lambda i: g.u.SetCursorPos(x + round(120 * (i if i < 24 else 47-i) / 23), y))
            finally:
                s.mouse(4)
            g.capture(hwnd, 'after-drag')
            point = w.POINT(round(500 * scale), round(360 * scale))
            g.u.ClientToScreen(hwnd, c.byref(point))
            g.u.SetCursorPos(point.x, point.y)
            measure('wheel-scroll', lambda i: s.mouse(0x800, -24 if i < 24 else 24))
            time.sleep(.4)
            g.capture(hwnd, 'after-scroll')
            result = dict(scope='mounted product, OS SendInput, backend commits are not display FPS', samples=samples)
            (OUT / 'motion.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
            print(json.dumps([dict((k, v) for k, v in row.items() if k != 'frame') for row in samples]), flush=True)
        finally:
            if hwnd and process.poll() is None:
                g.u.PostMessageW(hwnd, 16, 0, 0)
            try:
                process.wait(timeout=20)
            except subprocess.TimeoutExpired:
                process.kill(); process.wait()
            assert process.returncode == 0, process.returncode


if __name__ == '__main__':
    main()
