#!/usr/bin/env python3
"""Launch one built Apple testbed and retain actual frame/load evidence.

Run via validation/run-with-timeout.py. Cleanup uses SIGTERM and is deliberately
not counted as a successful GPU retirement or normal application shutdown.
"""
import argparse
import json
import os
from pathlib import Path
import plistlib
import subprocess
import time


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('bundle', type=Path)
    parser.add_argument('--output', type=Path, required=True)
    args = parser.parse_args()
    bundle = args.bundle.resolve()
    out = args.output.resolve()
    out.mkdir(parents=True, exist_ok=True)
    with (bundle / 'Contents/Info.plist').open('rb') as f:
        plist = plistlib.load(f)
    binary = bundle / 'Contents/MacOS' / plist['CFBundleExecutable']
    metrics = out / 'frames.json'
    metrics.unlink(missing_ok=True)
    failure = Path(str(metrics) + '.exception.txt')
    failure.unlink(missing_ok=True)
    env = os.environ | {'DOROTI_MAUI_EVIDENCE': str(metrics), 'MTL_DEBUG_LAYER': '1',
                        'DOROTI_TESTBED_MODE': 'sample', 'DOROTI_RESIZE_FIXTURE': 'none'}
    for key in ('DOROTI_MACOS_GRAPHITE', 'DOROTI_IOS_GRAPHITE', 'DOROTI_EXIT_AFTER_EVIDENCE'):
        env.pop(key, None)  # Exercise the product defaults.
    report = {'status': 'FAIL', 'applicationId': plist['CFBundleIdentifier'], 'command': [str(binary)],
              'physicalInput': 'notVerified', 'performance': 'notVerified', 'normalShutdown': 'notVerified'}
    with (out / 'runtime.log').open('w') as log:
        process = subprocess.Popen([str(binary)], env=env, stdout=log, stderr=subprocess.STDOUT)
        try:
            deadline = time.monotonic() + 90
            while time.monotonic() < deadline:
                if process.poll() is not None:
                    raise RuntimeError(f'Product exited before evidence: {process.returncode}')
                if failure.exists():
                    raise RuntimeError(failure.read_text())
                try:
                    data = json.loads(metrics.read_text())
                except (FileNotFoundError, json.JSONDecodeError):
                    time.sleep(.1)
                    continue
                if data['frame']['failed']:
                    raise RuntimeError('Product reported failed frames')
                if data['frame']['presented'] > 0 and data['frame']['replayed'] > 0:
                    break
                time.sleep(.1)
            else:
                raise TimeoutError('No completed product frame/replay within 90 seconds')
            if 'Graphite-Metal' not in data['frame']['backend']:
                raise RuntimeError('Product did not use the default Graphite/Metal backend')
            mapped = subprocess.run(['vmmap', str(process.pid)], capture_output=True, text=True, timeout=30)
            (out / 'vmmap.log').write_text(mapped.stdout + mapped.stderr)
            skia = [line for line in mapped.stdout.splitlines() if 'libSkiaSharp' in line]
            if mapped.returncode or not skia:
                raise RuntimeError('Unable to identify the loaded Skia image')
            report.update(status='PASS-render-load', frame=data['frame'], loadedSkia=skia,
                          vmmapExit=mapped.returncode, binary=str(binary))
        except Exception as error:
            report['error'] = str(error)
        finally:
            if process.poll() is None:
                process.terminate()
                try:
                    process.wait(timeout=10)
                except subprocess.TimeoutExpired:
                    process.kill()
                    process.wait()
            report['cleanupExitCode'] = process.returncode
    (out / 'result.json').write_text(json.dumps(report, indent=2) + '\n')
    print(report['status'], report.get('error', ''), flush=True)
    return 0 if report['status'] == 'PASS-render-load' else 1


if __name__ == '__main__':
    raise SystemExit(main())
