#!/usr/bin/env python3
"""Build and run the production UIKit Metal lifecycle probe on Catalyst or iOS."""
import argparse
import datetime
import json
import os
import signal
from pathlib import Path
import subprocess
import sys

ROOT = Path(__file__).resolve().parents[3]
PROJECT = Path(__file__).resolve().parent


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--dotnet-sdk', type=Path, help='Explicit SDK dotnet.dll (e.g. SDK 10 when SDK 11 is the default)')
    parser.add_argument('--output', type=Path)
    parser.add_argument('--device', help='Run the signed iOS probe on this devicectl device; temporarily replaces dev.doroti.testbed')
    args = parser.parse_args()
    out = (args.output or ROOT / 'Doroti/artifacts/uikit-metal-lifecycle' /
           datetime.datetime.now(datetime.timezone.utc).strftime('%Y%m%dT%H%M%SZ')).resolve()
    out.mkdir(parents=True, exist_ok=True)
    runs = []

    def run(label, command, env=None):
        with (out / (label + '.log')).open('w') as log:
            process = subprocess.Popen(list(map(str, command)), cwd=ROOT, env=env,
                                       stdout=log, stderr=subprocess.STDOUT, start_new_session=True)
            try:
                code = process.wait(timeout=1200)
            except subprocess.TimeoutExpired:
                os.killpg(process.pid, signal.SIGKILL)
                process.wait()
                log.write('\nExternal 1200-second timeout; process group killed.\n')
                code = 124
        runs.append({'label': label, 'command': list(map(str, command)), 'exitCode': code, 'timeoutSeconds': 1200})
        (out / 'commands.json').write_text(json.dumps(runs, indent=2) + '\n')
        print(label, code, flush=True)
        return code

    dotnet = ['dotnet', str(args.dotnet_sdk)] if args.dotnet_sdk else ['dotnet']
    project = PROJECT / ('ios/Doroti.Validation.UIKitMetalLifecycle.iOS.csproj' if args.device
                         else 'Doroti.Validation.UIKitMetalLifecycle.csproj')
    if run('build', [*dotnet, 'build', project,
                     '-m:1', '-p:UseSharedCompilation=false']):
        return 1
    report = out / 'runtime.json'
    if args.device:
        bundle = ROOT / ('Doroti/artifacts/validation/build/uikit-metal-lifecycle/ios/bin/Debug/'
                         'net10.0-ios/ios-arm64/Doroti.Validation.UIKitMetalLifecycle.iOS.app')
        if run('install', ['xcrun', 'devicectl', 'device', 'install', 'app', '--device', args.device,
                           '--json-output', out / 'install.json', bundle]):
            return 1
        sentinel = out / 'empty.json'
        sentinel.write_text('{}\n')
        if run('reset-report', ['xcrun', 'devicectl', 'device', 'copy', 'to', '--device', args.device,
                                '--domain-type', 'appDataContainer', '--domain-identifier', 'dev.doroti.testbed',
                                '--source', sentinel, '--destination', 'Documents/uikit-metal-lifecycle.json']):
            return 1
        code = run('runtime', ['xcrun', 'devicectl', 'device', 'process', 'launch', '--device', args.device,
                               '--terminate-existing', '--console', '--environment-variables',
                               json.dumps({'MTL_DEBUG_LAYER': '1', 'DOROTI_UIKIT_LIFECYCLE_EVIDENCE': '1'}),
                               '--json-output', out / 'launch.json', 'dev.doroti.testbed'])
        if run('copy-report', ['xcrun', 'devicectl', 'device', 'copy', 'from', '--device', args.device,
                               '--domain-type', 'appDataContainer', '--domain-identifier', 'dev.doroti.testbed',
                               '--source', 'Documents/uikit-metal-lifecycle.json', '--destination', report]):
            return 1
        termination = json.loads((out / 'launch.json').read_text()).get('result', {}).get('terminationResult', {})
        return 0 if code == 0 and termination.get('exitCode') == 0 and json.loads(report.read_text()).get('status') == 'PASS' else 1
    binary = ROOT / ('Doroti/artifacts/validation/build/uikit-metal-lifecycle/bin/Debug/'
                     'net10.0-maccatalyst/maccatalyst-arm64/Doroti UIKit Metal Lifecycle.app/'
                     'Contents/MacOS/Doroti.Validation.UIKitMetalLifecycle')
    code = run('runtime', [binary], os.environ | {'MTL_DEBUG_LAYER': '1', 'DOROTI_UIKIT_LIFECYCLE_EVIDENCE': str(report)})
    if code or not report.exists() or json.loads(report.read_text()).get('status') != 'PASS':
        return 1
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
