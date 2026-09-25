"""AppKit desktop control/close probe; invoke via run-with-timeout.py."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import subprocess
import time

parser = argparse.ArgumentParser()
parser.add_argument('--app', required=True, type=Path)
parser.add_argument('--output', required=True, type=Path)
parser.add_argument('--renderer', choices=['graphite', 'ganesh'], default='graphite')
parser.add_argument('--material', choices=['normal', 'acrylic', 'legacy'], default='normal')
parser.add_argument('--quit', action='store_true')
parser.add_argument('--explicit', action='store_true')
parser.add_argument('--capture', action='store_true')
parser.add_argument('--caption', choices=['System', 'Solid', 'Backdrop'])
args = parser.parse_args()
args.output.mkdir(parents=True, exist_ok=False)
output = args.output.resolve()
exe = args.app.resolve() / 'Contents/MacOS/DorotiTestbedApp.MacOS'
state = output / 'states.json'
info = dict(renderer=args.renderer, material=args.material, quit=args.quit, explicit=args.explicit, caption=args.caption,
    executable=str(exe), executableSha256=hashlib.sha256(exe.read_bytes()).hexdigest(),
    hostSha256=hashlib.sha256((args.app.resolve() / 'Contents/MonoBundle/Doroti.Host.Maui.dll').read_bytes()).hexdigest(),
    head=subprocess.check_output(['git', 'rev-parse', 'HEAD'], text=True, timeout=30).strip())
(output / 'run-info.json').write_text(json.dumps(info, indent=2))
env = dict(os.environ, DOROTI_DESKTOP_SAMPLE=args.material,
    DOROTI_DESKTOP_PROBE=str(state), DOROTI_MAUI_EVIDENCE=str(output / 'render.json'),
    DOROTI_DESKTOP_PROBE_QUIT='1' if args.quit else '0',
    DOROTI_DESKTOP_LIFETIME='Explicit' if args.explicit else 'OnLastWindowClosed',
    DOROTI_DESKTOP_CAPTURE='1' if args.capture else '0',
    DOROTI_MACOS_GRAPHITE='0' if args.renderer == 'ganesh' else '1')
if args.caption: env['DOROTI_DESKTOP_CAPTION'] = args.caption
with (output / 'app.log').open('w') as log:
    process = subprocess.Popen([str(exe)], env=env, stdout=log, stderr=subprocess.STDOUT)
    try:
        deadline = time.monotonic() + 120
        while process.poll() is None and time.monotonic() < deadline:
            capture = Path(str(state) + '.capture')
            if capture.exists():
                subprocess.run(['/usr/sbin/screencapture', '-x', '-o', '-l', capture.read_text(), str(output / 'window.png')], check=True, timeout=30)
                capture.unlink()
            errors = list(output.glob('*error*')) + list(output.glob('*exception*'))
            if errors:
                raise RuntimeError('\n'.join(p.read_text() for p in errors))
            time.sleep(.1)
        if process.poll() is None:
            raise RuntimeError('AppKit probe did not close within 120 seconds')
        if process.returncode != 0:
            raise RuntimeError(f'AppKit exited with {process.returncode}; see app.log')
        native = json.loads(Path(str(state) + '.native.json').read_text())
        assert native['result'] == 'PASS', native
        assert Path(str(state) + '.close').read_text() == '2', 'Second close decision missing'
        closed = json.loads(Path(str(state) + '.closed').read_text())
        assert closed['Closed'] and closed['remaining'] == 0, closed
        if args.explicit and not args.quit: assert Path(str(state) + '.explicit').read_text() == 'PASS'
        print(json.dumps(dict(result='PASS', **info, native=native), indent=2))
    finally:
        if process.poll() is None:
            process.terminate()
            try:
                process.wait(timeout=5)
            except subprocess.TimeoutExpired:
                process.kill()
                process.wait(timeout=5)
