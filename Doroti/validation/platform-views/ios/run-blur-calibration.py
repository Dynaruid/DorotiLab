"""Collect fresh public UIKit calibration pixels. Invoke with run-with-timeout.py.
Requires Pillow in this Python environment; does not claim physical display capture.
"""
import argparse
import json
from pathlib import Path
import shutil
import subprocess
import sys
import time
import uuid

parser = argparse.ArgumentParser()
connection = parser.add_mutually_exclusive_group(required=True)
connection.add_argument('--device')
connection.add_argument('--simulator')
parser.add_argument('--app', required=True, type=Path)
parser.add_argument('--output', required=True, type=Path)
args = parser.parse_args()
args.output.mkdir(parents=True, exist_ok=True)
name = 'blur-calibration-' + uuid.uuid4().hex
env = dict(DOROTI_TESTBED_MODE='platform-effects', DOROTI_UIKIT_BLUR_CALIBRATION='1',
           DOROTI_UIKIT_BLUR_CALIBRATION_NAME=name)
commands = []

def run(command, label, check=True):
    with (args.output / (label + '.log')).open('w') as log:
        result = subprocess.run(command, stdout=log, stderr=subprocess.STDOUT, timeout=1200)
    commands.append(dict(command=command, exit=result.returncode))
    if check and result.returncode:
        raise RuntimeError(label + ' failed')
    return result.returncode

try:
    if args.simulator:
        base = ['xcrun', 'simctl']
        run(base + ['install', args.simulator, str(args.app.resolve())], 'install')
        run(['env'] + ['SIMCTL_CHILD_' + k + '=' + v for k, v in env.items()] + base
            + ['launch', '--terminate-running-process', args.simulator, 'dev.doroti.testbed'], 'launch')
        container = Path(subprocess.check_output(base + ['get_app_container', args.simulator,
                         'dev.doroti.testbed', 'data'], text=True, timeout=1200).strip())
    else:
        base = ['xcrun', 'devicectl', 'device']
        run(base + ['install', 'app', '--device', args.device, str(args.app.resolve())], 'install')
        run(base + ['process', 'launch', '--device', args.device, '--terminate-existing',
                    '--environment-variables', json.dumps(env), 'dev.doroti.testbed'], 'launch')
    captures = args.output / 'captures'
    for attempt in range(30):
        time.sleep(2)
        if args.simulator:
            source = container / 'Documents' / name
            if not (source / 'result.txt').exists() and not (source / 'error.txt').exists():
                continue
            shutil.copytree(source, captures, dirs_exist_ok=True)
        else:
            run(base + ['copy', 'from', '--device', args.device, '--domain-type', 'appDataContainer',
                        '--domain-identifier', 'dev.doroti.testbed', '--source', 'Documents/' + name,
                        '--destination', str(captures.resolve())], 'copy', check=False)
        if (captures / 'error.txt').exists():
            raise RuntimeError((captures / 'error.txt').read_text())
        if (captures / 'result.txt').exists():
            break
    else:
        raise TimeoutError('Calibration did not finish')
    run([sys.executable, str(Path(__file__).with_name('analyze-blur-calibration.py')),
         str(captures), '--require-common-match'], 'analysis')
    print((args.output / 'analysis.log').read_text())
finally:
    (args.output / 'commands.json').write_text(json.dumps(commands, indent=2))
