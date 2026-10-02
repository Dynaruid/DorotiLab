"""Verify desktop C-only initialization rejects obsolete settings before running the app.

Run under eng/run-with-timeout.py with an already-built Windows, Qt or Apple desktop executable.
Positive unset/C probes belong to native_frame_pipeline_collect.py. This script
only starts its own processes and retains each diagnostic and payload identity.
"""
import argparse
import json
import os
from pathlib import Path
import subprocess
import time

from native_frame_pipeline_collect import payload_identity

ROOT = Path(__file__).resolve().parents[2]
REMOVED = ('DOROTI_VARIABLE_BLUR_SERIAL_FRAMES', 'DOROTI_VARIABLE_BLUR_PIPELINE',
           'DOROTI_NATIVE_PRESENTATION', 'DOROTI_IOS_SHADER_PRESENTATION')


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--exe', type=Path, required=True)
    parser.add_argument('--output', type=Path, required=True)
    parser.add_argument('--apple-receipt', action='store_true',
                        help='Accept an Apple startup ArgumentException receipt; stop the native event loop after rejection.')
    args = parser.parse_args()
    out = args.output.resolve()
    if not args.exe.is_file() or out.exists() or not out.is_relative_to(ROOT / 'temp/testing'):
        parser.error('Use an existing executable and fresh temp/testing output')
    if args.apple_receipt and not any(path.suffix == '.app' for path in args.exe.resolve().parents):
        parser.error('--apple-receipt requires an executable inside an Apple app bundle')
    out.mkdir(parents=True)
    results = []
    (out / 'payload-manifest.json').write_text(json.dumps(payload_identity(args.exe), indent=2))
    cases = [('mode-' + value, 'DOROTI_NATIVE_FRAME_MODE', value) for value in ('A', 'B', 'invalid')]
    cases += [(key, key, value) for key, value in zip(REMOVED, ('0', '1', 'async', 'async'))]
    try:
        for name, key, value in cases:
            environment = os.environ.copy()
            for selector in ('DOROTI_NATIVE_FRAME_MODE', *REMOVED):
                environment.pop(selector, None)
            environment['DOROTI_NATIVE_FRAME_MODE'] = 'C'
            environment[key] = value
            evidence = out / (name + '.json')
            environment['DOROTI_MAUI_EVIDENCE'] = str(evidence)
            result = {'setting': key, 'value': value, 'status': 'FAIL'}
            results.append(result)
            failure = Path(str(evidence) + '.exception.txt')
            with (out / (name + '.log')).open('w', encoding='utf-8') as log:
                process = subprocess.Popen([str(args.exe.resolve())], cwd=ROOT, env=environment,
                                           stdout=log, stderr=subprocess.STDOUT)
                try:
                    deadline = time.monotonic() + 30
                    while process.poll() is None:
                        if args.apple_receipt and failure.exists(): break
                        if time.monotonic() >= deadline: raise TimeoutError(f'{key} did not reject initialization')
                        time.sleep(.05)
                    result['exitCodeBeforeCleanup'] = process.poll()
                finally:
                    if process.poll() is None:
                        process.terminate()
                        try: process.wait(timeout=5)
                        except subprocess.TimeoutExpired: process.kill(); process.wait(timeout=5)
            message = (out / (name + '.log')).read_text(encoding='utf-8', errors='replace')
            if failure.exists():
                message += failure.read_text(encoding='utf-8', errors='replace')
            receipt = args.apple_receipt and failure.exists()
            if receipt:
                assert 'System.ArgumentException' in message and 'ValidateFrameConfiguration' in message, message
                assert not evidence.exists(), 'Rejected initialization produced normal rendering evidence'
            result['rejectionSignal'] = 'Apple startup ArgumentException receipt' if receipt else 'process exit'
            exit_code = result['exitCodeBeforeCleanup']
            assert receipt or exit_code is not None and exit_code != 0, f'{key}={value} did not fail initialization'
            assert key in message and 'removed' in message.lower(), message
            result['status'] = 'PASS'
            print(f'PASS: initialization rejected {key}={value}', flush=True)
    finally:
        (out / 'results.json').write_text(json.dumps(results, indent=2))


if __name__ == '__main__':
    main()
