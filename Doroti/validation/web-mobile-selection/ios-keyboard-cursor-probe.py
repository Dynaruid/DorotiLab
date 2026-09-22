"""Compare WebKit keyboard point-selection with/without IME hit testing.

Requires a booted iOS Simulator and Xcode. The test app uses WebKit SPI only
to exercise the native point-selection path; no SPI is shipped in Doroti.
This is not an end-to-end gesture on the system software keyboard.
"""
import json
import plistlib
from pathlib import Path
import subprocess
import sys
import time

device, output = sys.argv[1:]
out = Path(output).resolve()
out.mkdir(parents=True, exist_ok=False)
app = out / 'CursorProbe.app'
app.mkdir()
bundle = 'dev.doroti.keyboardcursorprobe'


def run(*args):
    return subprocess.check_output(args, text=True, stderr=subprocess.STDOUT, timeout=60).strip()


(app / 'Info.plist').write_bytes(plistlib.dumps({
    'CFBundleIdentifier': bundle, 'CFBundleExecutable': 'Probe',
    'CFBundleName': 'CursorProbe', 'CFBundleVersion': '1',
    'CFBundlePackageType': 'APPL', 'LSRequiresIPhoneOS': True,
    'UILaunchScreen': {}, 'UIDeviceFamily': [1, 2],
}))
sdk = run('xcrun', '--sdk', 'iphonesimulator', '--show-sdk-path')
source = Path(__file__).with_suffix('.m')
run('xcrun', 'clang', '-fobjc-arc', '-fblocks', '-target', 'arm64-apple-ios18.0-simulator',
    '-isysroot', sdk, '-framework', 'UIKit', '-framework', 'WebKit', str(source), '-o', str(app / 'Probe'))
(out / 'environment.json').write_text(json.dumps({
    'device': device, 'sdk': sdk,
    'devices': json.loads(run('xcrun', 'simctl', 'list', 'devices', '--json')),
}, indent=2))
run('xcrun', 'simctl', 'install', device, str(app))
try:
    container = Path(run('xcrun', 'simctl', 'get_app_container', device, bundle, 'data'))
    result = container / 'Documents/result.json'
    result.unlink(missing_ok=True)
    run('xcrun', 'simctl', 'launch', device, bundle)
    deadline = time.monotonic() + 60
    while not result.exists() and time.monotonic() < deadline:
        time.sleep(.25)
    rows = json.loads(result.read_text())
    (out / 'observations.json').write_text(json.dumps(rows, indent=2))
    assert len(rows) == 4 and all('state' in r for r in rows), rows
    before, after = rows[:2], rows[2:]
    assert before[0]['state']['start'] == before[1]['state']['start'], rows
    assert after[0]['state']['start'] < after[1]['state']['start'], rows
    assert all(r['state']['active'] == 'e' and r['state']['hit'] == 'e' for r in after), rows
    assert all(r['selectionAssistantSuppressed'] for r in after), rows
    (out / 'result.json').write_text(json.dumps({
        'status': 'PASS', 'nativePointSelection': 'verified',
        'nativeSelectionAssistantSuppression': 'verified',
        'physicalKeyboardGesture': 'notVerified',
    }, indent=2))
    print('PASS: WebKit cursor follows native point selection while native selection UI stays suppressed')
finally:
    run('xcrun', 'simctl', 'uninstall', device, bundle)
