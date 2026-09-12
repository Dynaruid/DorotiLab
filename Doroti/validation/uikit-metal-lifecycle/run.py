#!/usr/bin/env python3
"""Build and run the production UIKit Metal lifecycle probe on Mac Catalyst."""
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
    if run('build', [*dotnet, 'build', PROJECT / 'Doroti.Validation.UIKitMetalLifecycle.csproj',
                     '-m:1', '-p:UseSharedCompilation=false']):
        return 1
    binary = ROOT / ('Doroti/artifacts/validation/build/uikit-metal-lifecycle/bin/Debug/'
                     'net10.0-maccatalyst/maccatalyst-arm64/Doroti UIKit Metal Lifecycle.app/'
                     'Contents/MacOS/Doroti.Validation.UIKitMetalLifecycle')
    report = out / 'runtime.json'
    code = run('runtime', [binary], os.environ | {'MTL_DEBUG_LAYER': '1', 'DOROTI_UIKIT_LIFECYCLE_EVIDENCE': str(report)})
    if code or not report.exists() or json.loads(report.read_text()).get('status') != 'PASS':
        return 1
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
