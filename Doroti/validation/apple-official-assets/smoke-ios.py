#!/usr/bin/env python3
"""Install an iOS product, verify default Graphite frames, and exercise app resume.

Run with validation/native-aot/run.py (1,200-second external timeout). The app
remains installed and running. This is not normal-shutdown/performance evidence.
"""
import argparse
import hashlib
import json
from pathlib import Path
import plistlib
import re
import subprocess
import time


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('bundle', type=Path)
    parser.add_argument('--device', required=True)
    parser.add_argument('--output', type=Path, required=True)
    parser.add_argument('--loader-log', type=Path, help='Optional DYLD_PRINT_LIBRARIES console log from a separate launch of this bundle')
    args = parser.parse_args()
    out = args.output.resolve()
    out.mkdir(parents=True, exist_ok=True)
    with (args.bundle / 'Info.plist').open('rb') as stream:
        app = plistlib.load(stream)['CFBundleIdentifier']
    steps = []
    report = dict(status='FAIL', applicationId=app, physicalInput='notVerified',
                  performance='notVerified', normalShutdown='notVerified', runtimeLoad='notVerified')

    def run(label, command, required=True):
        result = subprocess.run(list(map(str, command)), capture_output=True, text=True, timeout=1200)
        (out / (label + '.log')).write_text(result.stdout + result.stderr)
        steps.append(dict(label=label, command=list(map(str, command)), exitCode=result.returncode))
        (out / 'commands.json').write_text(json.dumps(steps, indent=2) + '\n')
        if required and result.returncode:
            raise RuntimeError(f'{label} failed: see {out / (label + ".log")}')
        return result

    def device(label, *command, required=True):
        return run(label, ['xcrun', 'devicectl', 'device', *command[:2], '--device', args.device,
                           '--json-output', out / (label + '.json'), *command[2:]], required)

    def copy(label, name, required=True):
        return device(label, 'copy', 'from', '--domain-type', 'appDataContainer',
                      '--domain-identifier', app, '--source', 'Documents/' + name,
                      '--destination', out / name, required=required)

    def frames(label, after=None):
        deadline = time.monotonic() + 90
        while time.monotonic() < deadline:
            result = copy('copy-' + label, 'doroti-maui-evidence.json', required=False)
            if result.returncode == 0:
                try:
                    data = json.loads((out / 'doroti-maui-evidence.json').read_text())
                    frame = data['frame']
                    if frame['failed']:
                        raise RuntimeError(f'Failed product frames: {frame}')
                    if frame['presented'] > 0 and frame['replayed'] > 0 and (
                            after is None or frame['presented'] + frame['replayed'] > after):
                        if 'Graphite-Metal' not in frame['backend']:
                            raise RuntimeError(f'Unexpected backend: {frame["backend"]}')
                        (out / (label + '-frames.json')).write_text(json.dumps(data, indent=2) + '\n')
                        return frame
                except (KeyError, json.JSONDecodeError):
                    pass
            time.sleep(1)
        raise TimeoutError('No fresh completed Graphite frame/replay within 90 seconds')

    try:
        device('install', 'install', 'app', args.bundle.resolve())
        # Installation preserves application data. Replace only the diagnostic
        # snapshot so an old run cannot satisfy this run's frame gate.
        sentinel = out / 'empty.json'
        sentinel.write_text('{}\n')
        device('reset-evidence', 'copy', 'to', '--domain-type', 'appDataContainer',
               '--domain-identifier', app, '--source', sentinel,
               '--destination', 'Documents/doroti-maui-evidence.json')
        device('launch', 'process', 'launch', '--terminate-existing', '--environment-variables',
               json.dumps({'DOROTI_MAUI_EVIDENCE': '1', 'MTL_DEBUG_LAYER': '1',
                           'DOROTI_TESTBED_MODE': 'sample', 'DOROTI_RESIZE_FIXTURE': 'none'}), app)
        first = frames('initial')
        device('background', 'process', 'launch', 'com.apple.Preferences')
        time.sleep(2)
        device('resume', 'process', 'launch', app)
        resumed = frames('resumed', first['presented'] + first['replayed'])
        initial_pid = json.loads((out / 'launch.json').read_text())['result']['process']['processIdentifier']
        resumed_pid = json.loads((out / 'resume.json').read_text())['result']['process']['processIdentifier']
        if initial_pid != resumed_pid:
            raise RuntimeError('Resume restarted the process instead of preserving the renderer')
        report.update(status='PASS-render-resume', initial=first, resumed=resumed,
                      processIdentifier=resumed_pid)
        if args.loader_log:
            binary = args.bundle / 'Frameworks/libSkiaSharp.framework/libSkiaSharp'
            uuids = run('bundle-uuid', ['xcrun', 'dwarfdump', '--uuid', binary]).stdout
            expected = re.search(r'UUID: ([A-Fa-f0-9-]+) \(arm64\)', uuids)
            loaded = re.findall(r'dyld\[(\d+)\]: <([A-Fa-f0-9-]+)> (.*libSkiaSharp.framework/libSkiaSharp)',
                                args.loader_log.read_text())
            suffix = '/' + args.bundle.name + '/Frameworks/libSkiaSharp.framework/libSkiaSharp'
            if not expected or not loaded or any(
                    uuid.upper() != expected[1].upper() or not path.endswith(suffix) for _, uuid, path in loaded):
                raise RuntimeError('Loader log does not identify this bundle\'s arm64 Skia image')
            report.update(status='PASS-render-load-resume', runtimeLoad='PASS; separate launch of the same bundle',
                          loadedSkia=loaded, loaderLog=str(args.loader_log.resolve()),
                          loaderLogSha256=hashlib.sha256(args.loader_log.read_bytes()).hexdigest())
    except Exception as error:
        report['status'] = 'FAIL'
        report['error'] = str(error)
    finally:
        (out / 'result.json').write_text(json.dumps(report, indent=2) + '\n')
    print(report['status'], report.get('error', ''), flush=True)
    return 0 if report['status'].startswith('PASS') else 1


if __name__ == '__main__':
    raise SystemExit(main())
