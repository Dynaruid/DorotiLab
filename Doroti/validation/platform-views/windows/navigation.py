"""Testbed navigation creates and disposes live native controls. Use record.py."""
import sys
sys.dont_write_bytecode = True
import product as p


def main():
    environment = p.os.environ.copy()
    environment.update(DOROTI_TESTBED_MODE='sample', DOROTI_WINDOWS_PRESENTER='Vulkan',
        DOROTI_PLATFORM_VIEW_EVIDENCE=str(p.OUT / 'frame.json'),
        DOROTI_WINDOWS_EXPERIMENTAL_ACRYLIC_READY_FILE=str(p.OUT / 'ready.json'))
    hwnd = 0
    checks = []
    with (p.OUT / 'product.log').open('w', encoding='utf-8') as log:
        process = p.subprocess.Popen([str(p.EXE)], cwd=p.EXE.parent, env=environment,
            stdout=log, stderr=p.subprocess.STDOUT)
        try:
            hwnd = p.wait_for(lambda: p.read(p.OUT / 'ready.json'), process)['hwnd']
            scale = p.u.GetDpiForWindow(hwnd) / 96
            p.u.GetClientRect.argtypes = [p.w.HWND, p.c.POINTER(p.w.RECT)]
            p.u.SetWindowPos(hwnd, p.w.HWND(-1), 20, 20, round(720 * scale), round(640 * scale), 0x40)
            p.time.sleep(2)
            p.capture(hwnd, 'bottom-bar')

            def controls():
                return [item for item in p.children(hwnd) if item['cls'] in ('Button', 'Edit')]

            def bottom(index):
                rect = p.w.RECT(); p.u.GetClientRect(hwnd, p.c.byref(rect))
                p.click(hwnd, rect.right / scale * (index + .5) / 5, rect.bottom / scale - 40, scale)

            previous = set()
            for cycle in range(3):
                bottom(4)
                live = p.wait_for(lambda: (items if len(items := controls()) == 2 and all(x['visible'] for x in items) else None), process)
                current = {item['hwnd'] for item in live}
                frame = p.wait_for(lambda: (value if (value := p.read(p.OUT / 'frame.json')) and len(value['native']) == 2 else None), process)
                ids = {item['handle']['InstanceId'] for item in frame['native']}
                assert not ids & previous, 'Remount reused IDs while prior disposal could still be pending'
                previous = ids
                if cycle == 0:
                    p.time.sleep(.3)
                    p.capture(hwnd, 'platform-views-bottom')
                bottom(1)
                p.wait_for(lambda: not controls() and all(not p.u.IsWindow(h) for h in current), process)
                checks.append(f'bottom-navigation-open-dispose-{cycle + 1}')
            p.capture(hwnd, 'color-after-dispose')
            p.u.SetWindowPos(hwnd, p.w.HWND(-1), 20, 20, round(1100 * scale), round(760 * scale), 0x40)
            p.time.sleep(2)
            p.capture(hwnd, 'side-rail')
            p.click(hwnd, 40, 262, scale)
            live = p.wait_for(lambda: (items if len(items := controls()) == 2 and all(x['visible'] for x in items) else None), process)
            p.time.sleep(.3)
            image, rect = p.capture(hwnd, 'platform-views-rail')
            editor = next(item for item in live if item['cls'] == 'Edit')
            pixel = image.getpixel((editor['rect'][2] - rect.left - 10, editor['rect'][3] - rect.top - 10))[:3]
            assert min(pixel) >= 245, f'Native editor is not visible: {pixel}'
            p.click(hwnd, 40, 130, scale)
            p.wait_for(lambda: not controls(), process)
            checks.append('rail-navigation-open-visible-editor-dispose')
            result = dict(productLive='passed', checks=checks, physical='notVerified')
        finally:
            if hwnd and process.poll() is None:
                p.capture(hwnd, 'final')
                p.u.PostMessageW(hwnd, 0x10, 0, 0)
            try: code = process.wait(timeout=20)
            except p.subprocess.TimeoutExpired:
                process.kill(); process.wait(); raise RuntimeError('Testbed close timed out')
            if code: raise RuntimeError(f'Testbed exited {code}')
    result['closeExitCode'] = code
    (p.OUT / 'result.json').write_text(p.json.dumps(result, indent=2))
    print(p.json.dumps(result), flush=True)


if __name__ == '__main__':
    try: main()
    except Exception:
        print((p.OUT / 'product.log').read_text(encoding='utf-8', errors='replace')[-10000:], flush=True)
        raise
