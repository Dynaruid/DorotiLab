"""Install a built testbed and collect a fresh UIKit product probe on an iPhone.
Invoke through validation/run-with-timeout.py. Does not build or claim physical input.
"""
import argparse
import json
from pathlib import Path
import subprocess
import time
import uuid

parser = argparse.ArgumentParser()
parser.add_argument('--device', required=True)
parser.add_argument('--app', type=Path, required=True)
parser.add_argument('--output', type=Path, required=True)
parser.add_argument('--mode', choices=['platform-views', 'platform-effects', 'webview-sample'], default='platform-views')
args = parser.parse_args()
args.output.mkdir(parents=True, exist_ok=True)
commands = []

def run(command, label, check=True):
    started = time.monotonic()
    with (args.output / (label + '.log')).open('w') as log:
        result = subprocess.run(command, stdout=log, stderr=subprocess.STDOUT, timeout=1200)
    commands.append(dict(command=command, exit=result.returncode, seconds=time.monotonic()-started))
    if check and result.returncode:
        raise RuntimeError(f'{label} failed; see {label}.log')
    return result.returncode

try:
    run(['xcrun', 'devicectl', 'device', 'install', 'app', '--device', args.device, str(args.app.resolve())], 'install')
    name = f'platform-views-{uuid.uuid4().hex}.txt'
    env = dict(DOROTI_TESTBED_MODE=args.mode, DOROTI_UIKIT_EVIDENCE='1', DOROTI_UIKIT_EVIDENCE_NAME=name)
    if args.mode == 'webview-sample':
        env.update(DOROTI_TESTBED_MODE='sample', DOROTI_TESTBED_WEBVIEW_PAGE_PROBE='1')
    run(['xcrun', 'devicectl', 'device', 'process', 'launch', '--device', args.device,
         '--terminate-existing', '--environment-variables', json.dumps(env), 'dev.doroti.testbed'], 'launch')
    destination = args.output / 'probe.txt'
    deadline = time.monotonic() + 180
    while time.monotonic() < deadline:
        time.sleep(2)
        code = run(['xcrun', 'devicectl', 'device', 'copy', 'from', '--device', args.device,
                    '--domain-type', 'appDataContainer', '--domain-identifier', 'dev.doroti.testbed',
                    '--source', 'Documents/' + name, '--destination', str(destination.resolve())], 'copy', check=False)
        if code == 0:
            content = destination.read_text()
            if 'RESULT=FAIL' in content:
                raise RuntimeError(content)
            if 'RESULT=PASS' in content:
                print(content)
                break
    else:
        raise TimeoutError('No completed product probe within 180 seconds')
finally:
    (args.output / 'commands.json').write_text(json.dumps(commands, indent=2))
