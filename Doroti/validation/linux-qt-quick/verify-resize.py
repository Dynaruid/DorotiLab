#!/usr/bin/env python3
"""Run ten actual product resizes and prove the requested Vulkan layer loaded."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import subprocess
import time

parser = argparse.ArgumentParser()
parser.add_argument('--app', type=Path, required=True)
parser.add_argument('--qpa', choices=('wayland', 'xcb'), required=True)
parser.add_argument('--output', type=Path, required=True)
args = parser.parse_args()
out = args.output.resolve()
out.mkdir(parents=True, exist_ok=False)
env = dict(os.environ, QT_QPA_PLATFORM=args.qpa, DOROTI_TESTBED_MODE='platform-effects',
           DOROTI_QT_DIAGNOSTICS='1', DOROTI_QT_VALIDATION_RESIZE_CYCLES='10',
           DOROTI_QT_VALIDATION_PLATFORM_VIEWS=str(out / 'final'))
with (out / 'product.log').open('w') as stream:
    process = subprocess.Popen(['dotnet', str(args.app.resolve())], env=env,
                               stdout=stream, stderr=subprocess.STDOUT)
    mapped = set()
    start = time.monotonic()
    while process.poll() is None and time.monotonic() - start < 30:
        try:
            for line in Path(f'/proc/{process.pid}/maps').read_text().splitlines():
                path = line.split()[-1]
                if path.startswith('/') and ('VkLayer_khronos_validation' in path
                                             or 'libdoroti_qt_host.so' in path):
                    mapped.add(path)
        except (OSError, IndexError):
            pass
        time.sleep(.01)
    if process.poll() is None:
        process.kill()
    exit_code = process.wait()
log = (out / 'product.log').read_text(errors='replace')
final = out / 'final.json'
checks = {
    'exit': exit_code == 0,
    'completedTenCycles': final.is_file(),
    'noVulkanValidationError': 'VUID-' not in log and 'Validation Error' not in log,
    'noManagedFatal': 'managed.fatal=' not in log and 'Unhandled exception' not in log,
    'layerMapped': any('libVkLayer_khronos_validation.so' in path for path in mapped),
    'hostMapped': any('libdoroti_qt_host.so' in path for path in mapped),
}
report = {'qpa': args.qpa, 'cyclesRequested': 10, 'exit': exit_code,
          'appSha256': hashlib.sha256(args.app.read_bytes()).hexdigest(),
          'mapped': {path: hashlib.sha256(Path(path).read_bytes()).hexdigest()
                     for path in sorted(mapped) if Path(path).is_file()},
          'checks': checks, 'status': 'passed' if all(checks.values()) else 'failed'}
(out / 'result.json').write_text(json.dumps(report, indent=2) + '\n')
print(json.dumps({'status': report['status'], 'checks': checks}, indent=2))
raise SystemExit(0 if report['status'] == 'passed' else 1)
