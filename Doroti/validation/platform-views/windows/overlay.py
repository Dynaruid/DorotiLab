"""Explicit NativeOverlay product fixture, independently from C acceptance."""
import sys
sys.dont_write_bytecode = True
import product as p

def main():
    environment = p.os.environ.copy()
    environment.update(DOROTI_TESTBED_MODE='platform-views', DOROTI_PLATFORM_VIEW_COMPOSITION='overlay',
        DOROTI_WINDOWS_PRESENTER='Vulkan', DOROTI_PLATFORM_VIEW_EVIDENCE=str(p.OUT / 'frame.json'),
        DOROTI_WINDOWS_EXPERIMENTAL_ACRYLIC_READY_FILE=str(p.OUT / 'ready.json'))
    hwnd = 0
    with (p.OUT / 'product.log').open('w', encoding='utf-8') as log:
        process = p.subprocess.Popen([str(p.EXE)], cwd=p.EXE.parent, env=environment, stdout=log, stderr=p.subprocess.STDOUT)
        try:
            ready = p.wait_for(lambda: p.read(p.OUT / 'ready.json'), process)
            hwnd = ready['hwnd']
            frame = p.wait_for(lambda: (v if (v := p.read(p.OUT / 'frame.json')) and len(v['native']) == 2 else None), process)
            p.u.SetWindowPos(hwnd, p.w.HWND(-1), 20, 20, 0, 0, 0x41)
            p.time.sleep(.4)
            image, rect = p.capture(hwnd, 'overlay')
            origin = p.w.POINT(); p.u.ClientToScreen(hwnd, p.c.byref(origin))
            scale = frame['dpi'] / 96
            editor = next(item for item in frame['native'] if item['handle']['InstanceId'] == 2)
            x = (editor['bounds'][0] + editor['bounds'][2]) / 2
            y = editor['bounds'][3] - 10
            pixel = image.getpixel((origin.x - rect.left + round(x * scale), origin.y - rect.top + round(y * scale)))[:3]
            assert min(pixel) >= 245, f'NativeOverlay editor is not visible: {pixel}'
            assert not frame['shields'], 'B fixture contains a foreground shield'
            result = dict(productLive='passed', composition='NativeOverlay', nativeCount=2, editorPixel=pixel,
                physical='notVerified', ime='notVerified')
        finally:
            if hwnd and process.poll() is None: p.u.PostMessageW(hwnd, 0x10, 0, 0)
            try: code = process.wait(timeout=20)
            except p.subprocess.TimeoutExpired:
                process.kill(); process.wait(); raise RuntimeError('NativeOverlay product close timed out')
            if code: raise RuntimeError(f'NativeOverlay product exited {code}')
    result['closeExitCode'] = code
    (p.OUT / 'result.json').write_text(p.json.dumps(result, indent=2))
    print(p.json.dumps(result), flush=True)

if __name__ == '__main__':
    try: main()
    except Exception:
        print((p.OUT / 'product.log').read_text(encoding='utf-8', errors='replace')[-10000:], flush=True)
        raise
