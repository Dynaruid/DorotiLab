"""Observe content, DWM outline and caption separately during OS resizing.

Run through run-with-timeout.py. Requires the optional artifact-local dxcam,
numpy and comtypes packages used by the other Windows displayed-frame gates.
Only starts/controls its own testbed process; restores the mouse on exit.
The optional GDI cross-check is CPU sampled, not a unique-frame/display oracle.
"""
import ctypes as c
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
import dxcam
import numpy as np
from PIL import Image, ImageGrab
from flicker_geometry import DwmRightBorder

spec = importlib.util.spec_from_file_location('resize', Path(__file__).with_name('verify-resize.py'))
g = importlib.util.module_from_spec(spec)
spec.loader.exec_module(g)
g.u.GetForegroundWindow.restype = g.w.HWND
g.u.WindowFromPoint.argtypes = [g.w.POINT]
g.u.WindowFromPoint.restype = g.w.HWND
g.u.GetAncestor.argtypes = [g.w.HWND, g.w.UINT]
g.u.GetAncestor.restype = g.w.HWND
g.u.SetCursorPos.argtypes = [c.c_int, c.c_int]
g.u.GetCursorPos.argtypes = [c.POINTER(g.w.POINT)]
g.u.ClientToScreen.argtypes = [g.w.HWND, c.POINTER(g.w.POINT)]
g.u.GetWindowLongW.argtypes = [g.w.HWND, c.c_int]
g.u.GetWindowLongW.restype = c.c_long
g.u.mouse_event.argtypes = [g.w.DWORD, g.w.DWORD, g.w.DWORD, g.w.DWORD, c.c_size_t]
saved = g.w.POINT()
g.u.GetCursorPos(c.byref(saved))
camera = process = worker = background = None
hwnd = None
host_kind = os.environ.get('DOROTI_RESIZE_HOST', 'maui')
assert host_kind in ('maui', 'windowsappsdk')
try:
    background = tk.Tk()
    background.overrideredirect(True)
    background.geometry('1800x1250+100+80')
    background.configure(bg='#c03030')
    background.update()
    env = dict(os.environ, DOROTI_MAUI_EVIDENCE=str(g.OUT / 'evidence.json'),
               DOROTI_WINDOWS_RESIZE_TIMELINE=str(g.OUT / 'timeline.json'),
               DOROTI_WINDOWS_MAUI_GRAPHITE='1', DOROTI_TESTBED_MODE='sample')
    process = subprocess.Popen([str(g.EXE)], cwd=g.EXE.parent, env=env)
    hwnd = g.wait_until(lambda: g.window_for(process.pid))
    g.u.SetForegroundWindow(hwnd)
    if host_kind == 'maui':
        g.wait_until(lambda: (e := g.evidence()) and e['frame']['presented'] > 0)
    assert g.u.SetWindowPos(hwnd, None, 200, 160, 960, 720, 0x14)
    time.sleep(1.3)
    # One-pixel change ensures diagnostics after the one-second write throttle.
    assert g.u.SetWindowPos(hwnd, None, 200, 160, 961, 720, 0x14)
    expected_client = g.w.RECT()
    assert g.u.GetClientRect(hwnd, c.byref(expected_client))
    client = g.w.RECT()
    g.u.GetClientRect(hwnd, c.byref(client))
    origin = g.w.POINT()
    g.u.ClientToScreen(hwnd, c.byref(origin))
    outer = g.w.RECT()
    g.u.GetWindowRect(hwnd, c.byref(outer))
    native_caption_height = origin.y-outer.top
    if host_kind == 'maui':
        e = g.wait_until(lambda: (e := g.evidence()) and e['surface']['pixelWidth'] == expected_client.right and e)
        scale = e['surface']['devicePixelRatio']
        inset = native_caption_height + client.bottom - e['surface']['pixelHeight']
        origin.y = outer.top
    else:
        g.u.GetDpiForWindow.argtypes = [g.w.HWND]
        scale = g.u.GetDpiForWindow(hwnd)/96
        outer = g.w.RECT()
        g.u.GetWindowRect(hwnd, c.byref(outer))
        inset = origin.y-outer.top
        origin.y = outer.top
        time.sleep(.5)
    region = (origin.x, origin.y, origin.x + 1350, origin.y + 900)
    backend = os.environ.get('DOROTI_FLICKER_CAPTURE', 'dxgi')
    assert backend in ('dxgi', 'gdi'), 'Supported capture backends: dxgi, gdi'
    if backend != 'gdi':
        camera = dxcam.create(output_color='RGB', max_buffer_len=4, backend=backend)
    g.u.SetForegroundWindow(hwnd)
    if g.u.GetForegroundWindow() != hwnd:
        # Windows can reject programmatic activation. Raise only our test window,
        # then click its caption only after verifying the hit HWND belongs to it.
        assert g.u.SetWindowPos(hwnd, None, 0, 0, 0, 0, 0x13)
        activation_bounds = g.w.RECT()
        g.u.GetWindowRect(hwnd, c.byref(activation_bounds))
        point = g.w.POINT(activation_bounds.left+100, activation_bounds.top+24)
        assert g.u.GetAncestor(g.u.WindowFromPoint(point), 2) == hwnd, 'Test caption is occluded; input aborted'
        g.u.SetCursorPos(point.x, point.y)
        g.u.mouse_event(2, 0, 0, 0, 0)
        g.u.mouse_event(4, 0, 0, 0, 0)
        g.wait_until(lambda: g.u.GetForegroundWindow() == hwnd, seconds=2)
    def grab(new_frame_only=True):
        return (camera.grab(region=region, new_frame_only=new_frame_only) if camera
                else np.asarray(ImageGrab.grab(region, include_layered_windows=True)))
    assert g.u.GetForegroundWindow() == hwnd, 'Test window lost foreground before calibration'
    frame = grab(False)
    assert frame is not None
    ys = slice(inset + round(12*scale), inset + round(44*scale))
    xs = slice(round(16*scale), round(208*scale))
    def edges(frame):
        title = frame[ys, xs, :3].astype(np.int16)
        return np.max(np.abs(title[:, 1:] - title[:, :-1]), axis=2) > 30
    border = DwmRightBorder(frame, inset, client.right)
    def caption_center(frame, right):
        band = frame[round(10*scale):round(24*scale), max(0, right-round(50*scale)):right, :3]
        xs = np.nonzero((band > 210).all(axis=2))[1]
        return max(0, right-round(50*scale))+(int(xs.min())+int(xs.max()))/2 if len(xs) else None
    band_y = inset + round(56*scale) - 6
    def layout_landmarks(frame):
        right = border.locate(frame)
        action_band = frame[inset+round(80*scale):inset+round(100*scale), :right, :3]
        action_x = np.nonzero((action_band > 170).all(axis=2))[1]
        action_center = (int(action_x.min()) + int(action_x.max())) / 2 if len(action_x) else None
        column = frame[:, 20, :3].astype(np.int16)
        neutral = np.max(column, axis=1) - np.min(column, axis=1) < 16
        bottom = int(np.flatnonzero(neutral)[-1])
        nav_top = bottom-round(75*scale)
        nav_band = frame[nav_top:bottom-round(40*scale), round(30*scale):round(62*scale), :3]
        nav_y = np.nonzero((nav_band > 190).all(axis=2))[0]
        nav_center = nav_top + (int(nav_y.min()) + int(nav_y.max())) / 2 if len(nav_y) else None
        return right, bottom, action_center, nav_center
    r0, b0, action0, nav0 = layout_landmarks(frame)
    assert action0 is not None and nav0 is not None, 'Layout landmarks missing'
    reference_action_offset = action0-r0/2
    reference_nav_offset = nav0-b0
    caption0 = caption_center(frame, r0)
    assert caption0 is not None, 'Close glyph missing in baseline'
    reference_caption_offset = caption0-r0
    reference = edges(frame)
    assert int(reference.sum()) > 100, 'Persistent title is not visible in the baseline'
    Image.fromarray(frame).save(g.OUT / 'flicker-before.png')
    phase = ['settle']
    done = threading.Event()
    errors = []
    def stimulus():
        try:
            time.sleep(.2)
            for edge in ('right', 'bottom'):
                rect = g.w.RECT()
                g.u.GetWindowRect(hwnd, c.byref(rect))
                start = (rect.right-3, (rect.top+rect.bottom)//2) if edge == 'right' else ((rect.left+rect.right)//2, rect.bottom-3)
                assert g.u.GetForegroundWindow() == hwnd, 'Test window lost foreground; input aborted'
                g.u.SetCursorPos(*start)
                time.sleep(.08)
                phase[0] = edge
                g.u.mouse_event(2, 0, 0, 0, 0)
                try:
                    for step in range(12):
                        assert g.u.GetForegroundWindow() == hwnd, 'Test window lost foreground; input aborted'
                        delta = 24 * (step+1 if step < 6 else 11-step)
                        g.u.SetCursorPos(start[0] + (delta if edge == 'right' else 0),
                                         start[1] + (delta if edge == 'bottom' else 0))
                        time.sleep(.035)
                finally:
                    g.u.mouse_event(4, 0, 0, 0, 0)
                phase[0] = 'settle'
                time.sleep(.25)
        except Exception as error:
            errors.append(repr(error))
        finally:
            done.set()
    worker = threading.Thread(target=stimulus)
    worker.start()
    styles = dict(extended=int(g.u.GetWindowLongW(hwnd, -20)), style=int(g.u.GetWindowLongW(hwnd, -16)))
    rows = []
    bad = exposed = out_of_phase = caption_mismatch = 0
    black_edges = white_edges = 0
    started = time.perf_counter()
    while not done.is_set() and time.perf_counter()-started < 15:
        assert g.u.GetForegroundWindow() == hwnd, 'Test window lost foreground; capture aborted'
        capture_started = time.perf_counter()
        frame = grab()
        capture_finished = time.perf_counter()
        if frame is None:
            time.sleep(.002)
            continue
        os_rect = g.w.RECT()
        g.u.GetClientRect(hwnd, c.byref(os_rect))
        score = float(edges(frame)[reference].mean())
        broken = score < .8
        observed_right, observed_bottom, action_center, nav_center = layout_landmarks(frame)
        action_error = action_center-observed_right/2-reference_action_offset if action_center is not None else None
        nav_error = nav_center-observed_bottom-reference_nav_offset if nav_center is not None else None
        layout_mismatch = (action_error is None or abs(action_error) > 4
                           or nav_error is None or abs(nav_error) > 4)
        close_center = caption_center(frame, observed_right)
        close_error = close_center-observed_right-reference_caption_offset if close_center is not None else None
        close_mismatch = close_error is None or abs(close_error) > 4
        expected_color = np.median(frame[band_y-1:band_y+2, 100:250, :3], axis=(0,1))
        edge_band = frame[band_y-1:band_y+2, observed_right-20:observed_right-4, :3].astype(np.float64)
        edge_difference = float(np.abs(edge_band - expected_color).max())
        edge_exposed = edge_difference > 12
        black_edge = float((edge_band.max(axis=2) < 5).mean()) > .75
        white_edge = float((edge_band.min(axis=2) > 250).mean()) > .75
        rows.append(dict(captureStartedQpcSeconds=capture_started, captureFinishedQpcSeconds=capture_finished, seconds=capture_finished-started, phase=phase[0], osClientWidth=os_rect.right, osClientHeight=os_rect.bottom, titleEdgeCoverage=score, broken=broken, displayedClientRight=observed_right, edgeDifference=edge_difference, exposedEdge=edge_exposed, blackEdge=black_edge, whiteEdge=white_edge, actionCenterError=action_error, navigationCenterError=nav_error, layoutOutOfPhase=layout_mismatch, closeCenterError=close_error, captionOutOfPhase=close_mismatch))
        if (broken or edge_exposed or layout_mismatch or close_mismatch) and bad + exposed + out_of_phase + caption_mismatch < 12:
            Image.fromarray(frame).save(g.OUT / f'flicker-bad-{bad+exposed+out_of_phase+caption_mismatch:02d}.png')
        bad += int(broken)
        exposed += int(edge_exposed)
        black_edges += int(black_edge)
        white_edges += int(white_edge)
        out_of_phase += int(layout_mismatch)
        caption_mismatch += int(close_mismatch)
    worker.join(timeout=5)
    assert done.is_set() and not errors, errors
    assert len(rows) >= 12, 'Insufficient captured-frame samples'
    assert all(sum(row['phase'] == edge for row in rows) >= 4 for edge in ('right', 'bottom'))
    assert max(r['osClientWidth'] for r in rows)-min(r['osClientWidth'] for r in rows) >= 96, 'Width stimulus not observed'
    assert max(r['osClientHeight'] for r in rows)-min(r['osClientHeight'] for r in rows) >= 96, 'Height stimulus not observed'
    result = dict(status='passed' if bad == 0 and exposed == 0 and out_of_phase == 0 and caption_mismatch == 0 else 'failed', capture=f'{backend} desktop capture; CPU observation timestamps',
                  host=host_kind, styles=styles, samples=len(rows), blankTitleFrames=bad, exposedEdgeFrames=exposed, blackEdgeFrames=black_edges, whiteEdgeFrames=white_edges, layoutOutOfPhaseFrames=out_of_phase, captionOutOfPhaseFrames=caption_mismatch, frames=rows,
                  timeline=str(g.OUT / 'timeline.json') if host_kind == 'maui' else None, geometryOracle='DWM two-pixel outline; caption glyph checked separately',
                  physicalDisplay='notVerified', output=str(g.OUT))
    (g.OUT / 'flicker-result.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
    print(json.dumps({k:v for k,v in result.items() if k != 'frames'}, indent=2))
    if os.environ.get('DOROTI_FLICKER_OBSERVE_ONLY') != '1':
        assert bad == 0 and exposed == 0 and out_of_phase == 0 and caption_mismatch == 0, 'Displayed-frame continuity/geometry check failed'
except Exception as error:
    result_path = g.OUT / 'flicker-result.json'
    if not result_path.exists():
        result_path.write_text(json.dumps(dict(status='notMeasured', reason=str(error),
                                              physicalDisplay='notVerified', output=str(g.OUT)), indent=2), encoding='utf-8')
    raise
finally:
    if worker:
        worker.join(timeout=5)
    g.u.mouse_event(4, 0, 0, 0, 0)
    g.u.SetCursorPos(saved.x, saved.y)
    if camera:
        camera.release()
    if process and process.poll() is None:
        if hwnd:
            g.u.PostMessageW(hwnd, 0x10, 0, 0)
        try:
            assert process.wait(timeout=15) == 0
        except subprocess.TimeoutExpired:
            process.kill()
            process.wait(timeout=10)

    if background:
        background.destroy()
