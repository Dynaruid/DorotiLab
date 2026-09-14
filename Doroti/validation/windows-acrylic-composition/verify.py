"""Windows Acrylic/native-raster composition gate; run with a 1200s timeout."""
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
OUT = Path(os.environ.get('DOROTI_PLATFORM_VIEW_GATE_OUTPUT') or
    ROOT / 'Doroti/artifacts/validation/platform-views/windows-acrylic-composition' / datetime.datetime.now().strftime('%Y%m%d-%H%M%S'))
OUT.mkdir(parents=True, exist_ok=True)
EXE = ROOT / 'DorotiTestbedApp/windowsappsdk/bin/Release/net10.0-windows10.0.19041.0/win-x64/DorotiTestbedApp.WindowsAppSdk.exe'
u = c.WinDLL('user32', use_last_error=True)
k = c.WinDLL('kernel32', use_last_error=True)
k.CreateFileW.argtypes = [w.LPCWSTR, w.DWORD, w.DWORD, w.LPVOID, w.DWORD, w.DWORD, w.HANDLE]
k.CreateFileW.restype = w.HANDLE
k.ReadFile.argtypes = [w.HANDLE, w.LPVOID, w.DWORD, c.POINTER(w.DWORD), w.LPVOID]
k.CloseHandle.argtypes = [w.HANDLE]
k.OpenProcess.argtypes = [w.DWORD, w.BOOL, w.DWORD]
k.OpenProcess.restype = w.HANDLE
u.GetGuiResources.argtypes = [w.HANDLE, w.DWORD]
u.GetWindowThreadProcessId.argtypes = [w.HWND, c.POINTER(w.DWORD)]
u.MapVirtualKeyW.argtypes = [w.UINT, w.UINT]
class GuiThreadInfo(c.Structure):
    _fields_ = [('cbSize', w.DWORD), ('flags', w.DWORD), ('active', w.HWND), ('focus', w.HWND),
        ('capture', w.HWND), ('menu', w.HWND), ('move', w.HWND), ('caret', w.HWND), ('caretRect', w.RECT)]
u.GetGUIThreadInfo.argtypes = [w.DWORD, c.POINTER(GuiThreadInfo)]

def focus(hwnd):
    process_id = w.DWORD()
    thread = u.GetWindowThreadProcessId(hwnd, c.byref(process_id))
    info = GuiThreadInfo(); info.cbSize = c.sizeof(info)
    assert u.GetGUIThreadInfo(thread, c.byref(info))
    return info.focus

def key(hwnd, code, up=False):
    parameter = 1 | (u.MapVirtualKeyW(code, 0) << 16) | (0xC0000000 if up else 0)
    u.SendMessageW(hwnd, 0x101 if up else 0x100, code, parameter)

def gdi_objects(process):
    handle = k.OpenProcess(0x400, False, process.pid)
    try: return u.GetGuiResources(handle, 0)
    finally: k.CloseHandle(handle)
u.SetProcessDpiAwarenessContext.argtypes = [w.HANDLE]
u.SetProcessDpiAwarenessContext(w.HANDLE(-4))
u.GetWindowRect.argtypes = [w.HWND, c.POINTER(w.RECT)]
u.ClientToScreen.argtypes = [w.HWND, c.POINTER(w.POINT)]
u.ScreenToClient.argtypes = [w.HWND, c.POINTER(w.POINT)]
u.PostMessageW.argtypes = [w.HWND, w.UINT, w.WPARAM, w.LPARAM]
u.SetForegroundWindow.argtypes = [w.HWND]
u.SendMessageW.argtypes = [w.HWND, w.UINT, w.WPARAM, w.LPARAM]
u.SendMessageW.restype = w.LPARAM
u.SetWindowTextW.argtypes = [w.HWND, w.LPCWSTR]
u.GetWindowTextW.argtypes = [w.HWND, w.LPWSTR, c.c_int]
u.GetClassNameW.argtypes = [w.HWND, w.LPWSTR, c.c_int]
u.GetDpiForWindow.argtypes = [w.HWND]
u.IsWindowVisible.argtypes = [w.HWND]
u.IsWindow.argtypes = [w.HWND]
u.GetWindowLongPtrW.argtypes = [w.HWND, c.c_int]
u.GetWindowLongPtrW.restype = c.c_ssize_t
u.SetWindowLongPtrW.argtypes = [w.HWND, c.c_int, c.c_ssize_t]
u.SetWindowPos.argtypes = [w.HWND, w.HWND, c.c_int, c.c_int, c.c_int, c.c_int, w.UINT]
u.WindowFromPoint.argtypes = [w.POINT]
u.WindowFromPoint.restype = w.HWND

def click(hwnd, x, y, scale, native_hit_test=True):
    # Synthetic HWND dispatch. This exercises the real raster/native window
    # procs but deliberately makes no claim about physical input delivery.
    point = w.POINT(); u.ClientToScreen(hwnd, c.byref(point))
    point.x += round(x * scale); point.y += round(y * scale)
    target = hwnd
    if native_hit_test:
        candidate = u.WindowFromPoint(point)
        hit = u.SendMessageW(candidate, 0x84, 0, point.x | (point.y << 16))
        if candidate and hit == 1:
            target = candidate
        else:
            for child in children(hwnd):
                left, top, right, bottom = child['rect']
                if child['visible'] and child['cls'] in ('Edit', 'Button') and left <= point.x < right and top <= point.y < bottom:
                    target = child['hwnd']; break
    u.ScreenToClient(target, c.byref(point))
    position = (point.x & 0xffff) | ((point.y & 0xffff) << 16)
    u.SendMessageW(target, 0x200, 0, position)
    u.SendMessageW(target, 0x201, 1, position)
    u.SendMessageW(target, 0x202, 0, position)

def probe(frame):
    return [int(value) for value in frame['probe'].split(',')[:3]]

def children(hwnd):
    result = []
    callback_type = c.WINFUNCTYPE(w.BOOL, w.HWND, w.LPARAM)
    def record(child, _):
        name, cls, rect = c.create_unicode_buffer(256), c.create_unicode_buffer(256), w.RECT()
        u.GetWindowTextW(child, name, 256); u.GetClassNameW(child, cls, 256); u.GetWindowRect(child, c.byref(rect))
        result.append(dict(hwnd=child, name=name.value, cls=cls.value, visible=bool(u.IsWindowVisible(child)),
            rect=[rect.left, rect.top, rect.right, rect.bottom], style=u.GetWindowLongPtrW(child, -16), ex=u.GetWindowLongPtrW(child, -20)))
        return True
    u.EnumChildWindows.argtypes = [w.HWND, callback_type, w.LPARAM]
    u.EnumChildWindows(hwnd, callback_type(record), 0)
    return result

def read(path):
    # Allow atomic replacement by the producer while the reader holds its handle.
    handle = k.CreateFileW(str(path), 0x80000000, 7, None, 3, 0, None)
    if handle == w.HANDLE(-1).value: return None
    try:
        buffer, count = c.create_string_buffer(65536), w.DWORD()
        if not k.ReadFile(handle, buffer, len(buffer), c.byref(count), None): return None
        try: return json.loads(buffer.raw[:count.value])
        except (UnicodeDecodeError, json.JSONDecodeError): return None
    finally: k.CloseHandle(handle)

def wait_for(predicate, process, timeout=35):
    end = time.monotonic() + timeout
    while time.monotonic() < end:
        value = predicate()
        if value: return value
        if process.poll() is not None:
            raise RuntimeError(f'Product exited early: {process.returncode}')
        time.sleep(.05)
    raise TimeoutError('Product composition did not reach the requested state')

def capture(hwnd, name):
    rect = w.RECT()
    if not u.GetWindowRect(hwnd, c.byref(rect)): raise c.WinError(c.get_last_error())
    image = ImageGrab.grab((rect.left, rect.top, rect.right, rect.bottom), all_screens=True)
    image.save(OUT / (name + '.png'))
    return image, rect

def main():
    environment = os.environ.copy()
    environment.update(DOROTI_TESTBED_MODE='platform-views', DOROTI_PLATFORM_VIEW_COMPOSITION='interleaved',
        DOROTI_WINDOWS_PRESENTER='Vulkan', DOROTI_PLATFORM_VIEW_EVIDENCE=str(OUT / 'frame.json'),
        DOROTI_WINDOWS_EXPERIMENTAL_ACRYLIC_READY_FILE=str(OUT / 'ready.json'))
    hwnd = 0
    with (OUT / 'product.log').open('w', encoding='utf-8') as log:
        process = subprocess.Popen([str(EXE)], cwd=EXE.parent, env=environment, stdout=log, stderr=subprocess.STDOUT)
        try:
            ready = wait_for(lambda: read(OUT / 'ready.json'), process)
            hwnd = ready['hwnd']
            frame = wait_for(lambda: (value if (value := read(OUT / 'frame.json')) and len(value['native']) == 2 else None), process)
            u.SetForegroundWindow(hwnd)
            # The pixel oracle must observe this test-owned window, even when
            # another desktop app is active. The process is closed in finally.
            u.SetWindowPos(hwnd, w.HWND(-1), 0, 0, 0, 0, 0x43)
            assert u.GetWindowLongPtrW(hwnd, -20) & 0x200000, 'Opaque parent redirection returned'
            time.sleep(.5)
            image, rect = capture(hwnd, 'stage-5')
            (OUT / 'children.json').write_text(json.dumps(children(hwnd), indent=2))
            (OUT / 'stage-5.json').write_text(json.dumps(frame, indent=2))
            assert frame['rasterCount'] >= 3, 'No GPU/native/GPU interleaving'
            origin = w.POINT(); u.ClientToScreen(hwnd, c.byref(origin))
            scale = frame['dpi'] / 96
            pixel = image.getpixel((origin.x - rect.left + round(150 * scale), origin.y - rect.top + round(188 * scale)))[:3]
            assert max(abs(a-b) for a,b in zip(pixel, (0,170,85))) <= 3, f'Actual window does not show middle raster above native: {pixel}'
            editor_pixel = image.getpixel((origin.x - rect.left + round(380 * scale), origin.y - rect.top + round(238 * scale)))[:3]
            assert min(editor_pixel) > 245, f'Live native editor is missing from the actual window: {editor_pixel}'
            identities = {item['handle']['InstanceId']: (item['hwnd'], item['handle']['InstanceGeneration']) for item in frame['native']}
            editor_hwnd = identities[2][0]
            text_to_set = c.create_unicode_buffer('native state survives overlap')
            assert u.SendMessageW(editor_hwnd, 0x000C, 0, c.addressof(text_to_set)), 'WM_SETTEXT failed'
            samples = []
            def current_stage(stage):
                value = read(OUT / 'frame.json')
                if not value or probe(value)[0] != stage: return None
                # The widget probe changes before its frame commits. Require
                # the native placement as well, not only the requested stage.
                editor = next((item for item in value['native'] if item['handle']['InstanceId'] == 2), None)
                if editor and editor['bounds'][0] != (210 if stage == 9 else 180): return None
                return value
            def taps():
                return (read(OUT / 'frame.json.input.json') or {}).get('nativePointerDowns', 0)
            for stage in [6, 7, 8, 9, 0, 1, 2, 3, 4, 5]:
                click(hwnd, 245, 72, scale)
                frame = wait_for(lambda: current_stage(stage), process, 10)
                time.sleep(.2)
                image, rect = capture(hwnd, f'stage-{stage}')
                (OUT / f'stage-{stage}.json').write_text(json.dumps(frame, indent=2))
                assert identities == {item['handle']['InstanceId']: (item['hwnd'], item['handle']['InstanceGeneration']) for item in frame['native']}, 'Native identity changed with paint order'
                text = c.create_unicode_buffer(256); u.SendMessageW(editor_hwnd, 0x000D, 256, c.addressof(text))
                assert text.value == 'native state survives overlap', 'Native editing state was replaced'
                def pixel_at(x, y):
                    return image.getpixel((origin.x - rect.left + round(x * scale), origin.y - rect.top + round(y * scale)))[:3]
                color = pixel_at(280, 218)
                expected = (255, 133, 102) if stage in [5,6,7,9] else (255,51,0) if stage in [1,2] else (255,255,255)
                assert max(abs(a-b) for a,b in zip(color, expected)) <= 4, f'Incorrect native/foreground ordering or alpha at stage {stage}: {color} vs {expected}'
                if stage == 2:
                    assert max(abs(a-b) for a,b in zip(pixel_at(50,125), (255,51,0))) <= 4, 'Full native occlusion failed'
                if stage == 9:
                    assert next(item for item in frame['native'] if item['handle']['InstanceId'] == 2)['bounds'][0] == 210, 'Native did not move'
                if stage == 6:
                    before_native, before_foreground = taps(), probe(frame)[1]
                    click(hwnd, 280, 218, scale, True)
                    wait_for(lambda: (value if (value := current_stage(stage)) and probe(value)[1] == before_foreground + 1 else None), process, 10)
                    assert taps() == before_native, 'Shield forwarded pointer into native editor'
                    click(hwnd, 380, 238, scale, True)
                    wait_for(lambda: taps() == before_native + 1, process, 10)
                if stage in [7,8]:
                    before_native = taps()
                    click(hwnd, 280, 218, scale, True)
                    wait_for(lambda: taps() == before_native + 1, process, 10)
                samples.append(dict(stage=stage, foregroundPixel=color, identityPreserved=True))
            click(hwnd, 380, 238, scale)
            wait_for(lambda: focus(hwnd) == editor_hwnd, process, 10)
            key(editor_hwnd, 9); key(editor_hwnd, 9, True)
            wait_for(lambda: focus(hwnd) == hwnd, process, 10)
            key(hwnd, 16); key(hwnd, 9); key(hwnd, 9, True); key(hwnd, 16, True)
            wait_for(lambda: focus(hwnd) == editor_hwnd, process, 10)
            before_native = taps()
            click(hwnd, 500, 72, scale)
            modal = wait_for(lambda: (value if (value := read(OUT / 'frame.json')) and any(
                s['bounds'][2] - s['bounds'][0] > 500 and s['bounds'][3] - s['bounds'][1] > 400 for s in value['shields']) else None), process, 10)
            time.sleep(.3)
            capture(hwnd, 'modal')
            click(hwnd, 80, 130, scale)
            wait_for(lambda: (value if (value := read(OUT / 'frame.json')) and all(
                s['bounds'][2] - s['bounds'][0] < 500 for s in value['shields']) else None), process, 10)
            assert taps() == before_native, 'Modal barrier leaked a click to native button'
            gdi_before = gdi_objects(process)
            for cycle in range(3):
                old_controls = [item['hwnd'] for item in read(OUT / 'frame.json')['native']]
                click(hwnd, 390, 72, scale)
                wait_for(lambda: (value if (value := read(OUT / 'frame.json')) and not value['native'] and
                    all(not u.IsWindow(control) for control in old_controls) else None), process, 10)
                click(hwnd, 390, 72, scale)
                wait_for(lambda: (value if (value := read(OUT / 'frame.json')) and len(value['native']) == 2 else None), process, 10)
            capture(hwnd, 'recreated')
            final_frame = read(OUT / 'frame.json')
            gdi_after = gdi_objects(process)
            assert gdi_after <= gdi_before + 4, f'GDI objects grew across native lifecycle: {gdi_before} -> {gdi_after}'
            result = dict(productLive='passed', inputTransport='synthetic HWND messages', scenarios=samples, nativePointerDowns=taps(), physical='notVerified',
                ime='notVerified', modalBarrier='passed', productCreateDisposeCycles=3,
                tabAndShiftTab='passed', gdiObjectsBefore=gdi_before, gdiObjectsAfter=gdi_after, timings=final_frame['timings'],
                finalNativeCount=len(final_frame['native']), liveRasterWindowCount=len([item for item in children(hwnd) if item['name'] == 'Doroti GPU raster slice']),
                artifact=str(OUT))
        finally:
            if hwnd and process.poll() is None:
                u.PostMessageW(hwnd, 0x10, 0, 0)
            try: code = process.wait(timeout=1200)
            except subprocess.TimeoutExpired:
                process.kill()
                process.wait()
                raise RuntimeError('Product close did not complete within 1200 seconds')
            if code != 0: raise RuntimeError(f'Product close failed: {code}; {OUT / "product.log"}')
    errors = (OUT / 'product.log').read_text(encoding='utf-8', errors='replace')
    assert not any(marker in errors for marker in ['Unhandled exception', 'AssertionError', 'PlatformView creation failed']), errors[-4000:]
    result['closeExitCode'] = code
    (OUT / 'result.json').write_text(json.dumps(result, indent=2))
    print(json.dumps(result), flush=True)

if __name__ == '__main__':
    try: main()
    except Exception:
        print((OUT / 'product.log').read_text(encoding='utf-8', errors='replace')[-12000:], flush=True)
        raise
