"""Dense, lossless whole-window resize capture. Run with run-with-timeout.py.

No PNG encoding or pixel analysis in the capture loop. A bounded in-memory
sequence is drained between drags. All acquired frames and DXGI accumulation
metadata are retained; this does not claim physical scan-out coverage.
"""
import ctypes as c
import hashlib
import importlib.util
import json
import os
from pathlib import Path
import subprocess
import sys
import threading
import time
import tkinter as tk

ROOT = Path(__file__).resolve().parents[3]
sys.path.insert(0, str(ROOT / 'Doroti/artifacts/validation/capture-python'))
import cv2
import dxcam
import numpy as np
from PIL import Image

spec = importlib.util.spec_from_file_location('resize', Path(__file__).with_name('verify-resize.py'))
g = importlib.util.module_from_spec(spec)
spec.loader.exec_module(g)
u, w = g.u, g.w
u.GetForegroundWindow.restype = w.HWND
u.GetCursorPos.argtypes = [c.POINTER(w.POINT)]
u.SetCursorPos.argtypes = [c.c_int, c.c_int]
u.WindowFromPoint.argtypes = [w.POINT]
u.WindowFromPoint.restype = w.HWND
u.GetAncestor.argtypes = [w.HWND, w.UINT]
u.GetAncestor.restype = w.HWND
u.ClientToScreen.argtypes = [w.HWND, c.POINTER(w.POINT)]
u.mouse_event.argtypes = [w.DWORD, w.DWORD, w.DWORD, w.DWORD, c.c_size_t]

def bounds():
    rect = w.RECT()
    assert u.GetWindowRect(hwnd, c.byref(rect))
    return [rect.left, rect.top, rect.right, rect.bottom]

def activate():
    u.SetForegroundWindow(hwnd)
    if u.GetForegroundWindow() != hwnd:
        assert u.SetWindowPos(hwnd, None, 0, 0, 0, 0, 0x13)
        left, top, _, _ = bounds()
        point = w.POINT(left+100, top+24)
        assert u.GetAncestor(u.WindowFromPoint(point), 2) == hwnd, 'Test caption occluded'
        u.SetCursorPos(point.x, point.y)
        u.mouse_event(2, 0, 0, 0, 0)
        u.mouse_event(4, 0, 0, 0, 0)
    g.wait_until(lambda: u.GetForegroundWindow() == hwnd, seconds=2)

def black_regions(frame, row):
    mask = (frame.max(axis=2) < 5).astype(np.uint8)
    # The resize cursor can itself contain black pixels; exclude only its
    # immediate neighborhood, not the rest of the edge being dragged.
    px, py = row['cursor'][0]-region[0], row['cursor'][1]-region[1]
    mask[max(0, py-36):py+37, max(0, px-36):px+37] = 0
    count, _, stats, _ = cv2.connectedComponentsWithStats(mask, 8)
    left, top, right, bottom = row['outer']
    result = []
    for x, y, width, height, area in stats[1:count]:
        if area < 128 or max(width, height) < 32 or min(width, height) < 2:
            continue
        if max(width, height)/min(width, height) < 3:
            continue
        screen_x, screen_y = x+region[0], y+region[1]
        # The ordinary native dark caption is solid black on this host. Do
        # not confuse that persistent non-client background with a resize strip.
        if (row['nativeCaptionHeight'] > 0 and width > .7*(right-left)
                and abs(height-row['nativeCaptionHeight']) <= 4
                and abs(screen_y-top) < 80):
            continue
        near_edge = min(abs(screen_x-left), abs(screen_x+width-right),
                        abs(screen_y-top), abs(screen_y+height-bottom)) <= 80
        if near_edge:
            result.append(dict(x=int(screen_x), y=int(screen_y), width=int(width),
                               height=int(height), pixels=int(area)))
    return result

saved = w.POINT()
u.GetCursorPos(c.byref(saved))
background = process = camera = worker = None
hwnd = None
host = os.environ.get('DOROTI_RESIZE_HOST', 'maui')
edges = os.environ.get('DOROTI_CAPTURE_EDGES', 'right,bottom,bottom-right,left,top,top-left,top-right,bottom-left').split(',')
directions = dict(right=(1, 0), bottom=(0, 1), left=(-1, 0), top=(0, -1),
                  **{'bottom-right': (1, 1), 'top-left': (-1, -1),
                     'top-right': (1, -1), 'bottom-left': (-1, 1)})
assert all(edge in directions for edge in edges)
region = (300, 100, 1750, 1300)
summary = dict(status='notMeasured', host=host, output=str(g.OUT), region=region,
               interpretation='DXGI acquired frames; CPU/QPC observations, not physical scan-out', edges=[])
binary = g.EXE.parent / ('Doroti.Host.Maui.dll' if host == 'maui' else 'Doroti.Host.WindowsAppSdk.dll')
summary['binary'] = str(binary)
summary['binarySha256'] = hashlib.sha256(binary.read_bytes()).hexdigest()
try:
    background = tk.Tk()
    background.overrideredirect(True)
    background.geometry('1800x1250+100+80')
    background.configure(bg='#c03030')
    background.update()
    env = dict(os.environ, DOROTI_TESTBED_MODE='sample', DOROTI_WINDOWS_MAUI_GRAPHITE='1',
               DOROTI_MAUI_EVIDENCE=str(g.OUT/'evidence.json'),
               DOROTI_WINDOWS_RESIZE_TIMELINE=str(g.OUT/'timeline.json'))
    process = subprocess.Popen([str(g.EXE)], cwd=g.EXE.parent, env=env)
    hwnd = g.wait_until(lambda: g.window_for(process.pid))
    if host == 'maui':
        g.wait_until(lambda: (e := g.evidence()) and e['frame']['presented'] > 0)
    camera = dxcam.create(output_color='RGB', max_buffer_len=2)
    assert camera.width >= region[2] and camera.height >= region[3], 'Primary output too small for this fixture'
    for edge in edges:
        assert u.SetWindowPos(hwnd, None, 500, 300, 960, 720, 0x14)
        activate()
        time.sleep(.3)
        initial = bounds()
        client_origin = w.POINT()
        u.ClientToScreen(hwnd, c.byref(client_origin))
        native_caption_height = client_origin.y-initial[1]
        dx, dy = directions[edge]
        left, top, right, bottom = initial
        start = (left+3 if dx < 0 else right-3 if dx > 0 else (left+right)//2,
                 top+3 if dy < 0 else bottom-3 if dy > 0 else (top+bottom)//2)
        done = threading.Event()
        errors = []
        def stimulus():
            try:
                time.sleep(.1)
                assert u.GetForegroundWindow() == hwnd, 'Foreground lost before input'
                u.SetCursorPos(*start)
                time.sleep(.05)
                u.mouse_event(2, 0, 0, 0, 0)
                try:
                    for step in range(16):
                        assert u.GetForegroundWindow() == hwnd, 'Foreground lost during input'
                        delta = 16*(step+1 if step < 8 else 15-step)
                        u.SetCursorPos(start[0]+dx*delta, start[1]+dy*delta)
                        time.sleep(.04)
                finally:
                    u.mouse_event(4, 0, 0, 0, 0)
                time.sleep(.2)
            except Exception as error:
                errors.append(str(error))
            finally:
                done.set()
        frames = []
        worker = threading.Thread(target=stimulus)
        worker.start()
        while not done.is_set():
            assert u.GetForegroundWindow() == hwnd, 'Foreground lost during capture'
            started = time.perf_counter()
            frame = camera.grab(region=region)
            ended = time.perf_counter()
            if frame is None:
                time.sleep(.001)
                continue
            assert len(frames) < 180, 'Bounded frame storage exhausted; no frames silently dropped'
            cursor = w.POINT()
            u.GetCursorPos(c.byref(cursor))
            # Pinned dxcam 0.3.0 exposes these DXGI_FRAME_INFO values internally
            # for one-shot capture; its public timestamp property is ring-only.
            row = dict(captureStart=started, captureEnd=ended, outer=bounds(),
                       nativeCaptionHeight=native_caption_height,
                       cursor=[cursor.x, cursor.y],
                       desktopQpcTicks=int(camera._duplicator.latest_frame_ticks),
                       accumulatedFrames=int(camera._duplicator.accumulated_frames))
            frames.append((frame, row))
        worker.join(timeout=5)
        assert not errors, errors
        assert len(frames) >= 12, 'Insufficient frames'
        extent = 2 if dx else 3
        assert max(r['outer'][extent]-r['outer'][extent-2] for _, r in frames)-min(r['outer'][extent]-r['outer'][extent-2] for _, r in frames) >= 96, 'Resize stimulus missing'
        folder = g.OUT / edge
        folder.mkdir()
        # Persist every frame only after this drag has finished.
        rows = []
        for i, (frame, row) in enumerate(frames):
            row['blackRegions'] = black_regions(frame, row)
            row['file'] = f'{edge}/{i:04d}.png'
            Image.fromarray(frame).save(g.OUT / row['file'], compress_level=1)
            rows.append(row)
        (folder/'frames.json').write_text(json.dumps(rows, indent=2), encoding='utf-8')
        result = dict(edge=edge, frames=len(rows),
                      blackFrames=sum(bool(r['blackRegions']) for r in rows),
                      initialAccumulatedFrames=rows[0]['accumulatedFrames'],
                      coalescedDesktopUpdates=sum(max(0, r['accumulatedFrames']-1) for r in rows[1:]),
                      maximumCaptureMilliseconds=max((r['captureEnd']-r['captureStart'])*1000 for r in rows))
        summary['edges'].append(result)
        print(json.dumps(result), flush=True)
        del frames
    summary['status'] = 'blackDetected' if any(e['blackFrames'] for e in summary['edges']) else 'noBlackDetectedInAcquiredFrames'
finally:
    if worker:
        worker.join(timeout=5)
    u.mouse_event(4, 0, 0, 0, 0)
    u.SetCursorPos(saved.x, saved.y)
    if camera:
        camera.release()
    if process and process.poll() is None:
        if hwnd:
            u.PostMessageW(hwnd, 0x10, 0, 0)
        try:
            summary['exitCode'] = process.wait(timeout=15)
        except subprocess.TimeoutExpired:
            process.kill()
            process.wait(timeout=10)
            summary['exitCode'] = 'killedAfterTimeout'
    if background:
        background.destroy()
    (g.OUT/'dense-result.json').write_text(json.dumps(summary, indent=2), encoding='utf-8')
    print(json.dumps(summary, indent=2), flush=True)
