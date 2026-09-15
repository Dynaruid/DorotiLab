"""Capture gallery -> Platform views handoffs. Use run-with-timeout.py (1200s).
Requires the same optional DXGI capture packages as verify-scroll-frames.py.
"""
import importlib.util
import json
import os
from pathlib import Path
import subprocess
import sys
import threading
import time

sys.dont_write_bytecode = True
ROOT = Path(__file__).resolve().parents[3]
sys.path.insert(0, str(ROOT / 'Doroti/artifacts/validation/capture-python'))
import dxcam
import numpy as np
from PIL import Image

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
    camera = None
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
            time.sleep(.8)
            origin = w.POINT()
            g.u.ClientToScreen(hwnd, c.byref(origin))
            camera = dxcam.create(output_color='BGRA', max_buffer_len=8)
            camera.start(region=(origin.x, origin.y, origin.x + round(720*scale), origin.y + round(640*scale)), target_fps=120)
            phase = ['before']
            errors = []
            finished = threading.Event()
            def stimulus():
                try:
                    time.sleep(.25)
                    for cycle in range(3):
                        phase[0] = f'enter-{cycle}'
                        s.click(hwnd, 648, 590, scale)
                        g.wait_for(lambda: (f if (f:=g.read(OUT/'frame.json')) and len(f['native'])==2 else None), process)
                        time.sleep(.4)
                        phase[0] = f'exit-{cycle}'
                        s.click(hwnd, 72, 590, scale)
                        g.wait_for(lambda: (f if (f:=g.read(OUT/'frame.json')) and not f['native'] else None), process)
                        time.sleep(.4)
                except Exception as error:
                    errors.append(repr(error))
                finally:
                    finished.set()
            worker = threading.Thread(target=stimulus)
            worker.start()
            rows=[]; saved=0; baseline=None; last_timestamp=None; native_seen=set()
            start=time.monotonic()
            while not finished.is_set() and time.monotonic()-start < 20:
                frame, timestamp = camera.get_latest_frame(with_timestamp=True)
                if timestamp == last_timestamp: continue
                last_timestamp=timestamp
                # The Material title stays unchanged throughout tab navigation.
                title=frame[round(12*scale):round(42*scale),round(16*scale):round(350*scale),:3].astype(np.int16)
                edges=(np.max(np.abs(title[:,1:]-title[:,:-1]),axis=2)>30)
                if baseline is None: baseline=edges
                score=float(edges[baseline].mean())
                broken=score < .8
                current_phase=phase[0]
                green=frame[round(56*scale):round(580*scale),round(140*scale),:3]
                has_green=int(((green[:,0]==85)&(green[:,1]==170)&(green[:,2]==0)).sum()) >= round(70*scale)
                white=frame[round(56*scale):round(580*scale),round(380*scale),:3]
                native_pixels=int((white>250).all(axis=1).sum())
                native_missing=current_phase.startswith('enter') and has_green and current_phase in native_seen and native_pixels < round(90*scale)
                if current_phase.startswith('enter') and has_green and native_pixels >= round(90*scale): native_seen.add(current_phase)
                row=dict(timestamp=timestamp,phase=current_phase,titleEdgeCoverage=score,broken=broken,
                    nativePixels=native_pixels,nativeMissing=native_missing)
                if broken:
                    row['backendFrame']=g.read(OUT/'frame.json')
                    row['windows']=g.children(hwnd)
                rows.append(row)
                if len(rows)==1 or (broken and saved<12):
                    Image.fromarray(frame[:,:,[2,1,0]]).save(OUT/f'entry-{len(rows):03d}.png')
                    saved+=int(broken)
            worker.join(timeout=5)
            assert finished.is_set() and not errors, errors
            result=dict(capture='unique DXGI displayed frames',samples=len(rows),blankFrames=sum(r['broken'] for r in rows),
                nativeDropFrames=sum(r['nativeMissing'] for r in rows),nativeEntriesObserved=len(native_seen),cycles=3,frames=rows)
            (OUT/'entry-frames.json').write_text(json.dumps(result,indent=2),encoding='utf-8')
            print(json.dumps({k:v for k,v in result.items() if k!='frames'}),flush=True)
            assert len(rows)>60
            assert result['nativeEntriesObserved']==3
            if os.environ.get('DOROTI_ENTRY_REQUIRE_COHERENT')=='1':
                assert result['blankFrames']==0, 'Persistent title disappeared during presenter handoff'
                assert result['nativeDropFrames']==0, 'Native content disappeared after becoming visible'
        finally:
            if camera: camera.release()
            if hwnd and process.poll() is None: g.u.PostMessageW(hwnd,16,0,0)
            try: process.wait(timeout=20)
            except subprocess.TimeoutExpired: process.kill(); process.wait()
            assert process.returncode==0,process.returncode


if __name__=='__main__': main()
