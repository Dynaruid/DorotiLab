"""Run the actual AppKit Testbed; invoke through validation/run-with-timeout.py."""
import argparse
import json
import hashlib
import os
from pathlib import Path
import subprocess
import time

parser = argparse.ArgumentParser()
parser.add_argument('--app', required=True, type=Path)
parser.add_argument('--output', required=True, type=Path)
parser.add_argument('--renderer', choices=['graphite', 'ganesh'], default='graphite')
args = parser.parse_args()
args.output.mkdir(parents=True, exist_ok=True)
result = args.output.resolve() / 'result.txt'
if result.exists():
    raise SystemExit('Use a fresh output directory to preserve earlier failures.')
env = dict(os.environ, DOROTI_TESTBED_MODE='platform-effects', DOROTI_PLATFORM_VIEW_EVIDENCE=str(result),
           DOROTI_MACOS_CAPTURE='1', DOROTI_MACOS_GRAPHITE='0' if args.renderer == 'ganesh' else '1')
executable = args.app.resolve() / 'Contents/MacOS/DorotiTestbedApp.MacOS'
source_paths = subprocess.check_output(['git', 'ls-files', '-co', '--exclude-standard', '--',
    'Doroti/src', 'DorotiTestbedApp/src', 'DorotiTestbedApp/macos', 'DorotiTestbedApp/assets/webview',
    'DorotiTestbedApp/DorotiTestbedApp.csproj', 'Doroti/validation/platform-views/macos'], text=True, timeout=30).splitlines()
source = hashlib.sha256()
for name in sorted(set(source_paths)):
    path = Path(name)
    if path.is_file():
        source.update(name.encode())
        source.update(path.read_bytes())
(args.output / 'run-info.json').write_text(json.dumps(dict(
    renderer=args.renderer, app=str(args.app.resolve()),
    head=subprocess.check_output(['git', 'rev-parse', 'HEAD'], text=True, timeout=30).strip(),
    sourceSha256=source.hexdigest(), executableSha256=hashlib.sha256(executable.read_bytes()).hexdigest()), indent=2))
with (args.output / 'app.log').open('w') as log:
    process = subprocess.Popen([str(executable)], env=env, stdout=log, stderr=subprocess.STDOUT)
    try:
        deadline = time.monotonic() + 180
        while time.monotonic() < deadline and process.poll() is None and not result.exists():
            marker = Path(str(result) + '.capture')
            if marker.exists():
                lines = marker.read_text().splitlines()
                if len(lines) != 4:
                    time.sleep(.05)
                    continue
                name, number, bounds, scale = lines
                capture = args.output / (name + '.png')
                subprocess.run(['/usr/sbin/screencapture', '-x', '-o', '-l', number, str(capture)], check=True, timeout=1200)
                (args.output / (name + '.json')).write_text(json.dumps(dict(window=int(number), bounds=list(map(float, bounds.split(','))), scale=float(scale))))
                marker.unlink()
            time.sleep(.05)
        if not result.exists():
            raise RuntimeError(f'No evidence, exit={process.poll()}; see app.log')
        print(result.read_text())
        if 'RESULT=PASS' not in result.read_text():
            raise SystemExit(1)
    finally:
        if process.poll() is None:
            process.terminate()
            try:
                process.wait(timeout=10)
            except subprocess.TimeoutExpired:
                process.kill()
                process.wait(timeout=10)
