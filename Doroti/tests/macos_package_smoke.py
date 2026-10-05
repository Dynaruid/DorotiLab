"""Local extracted-app install/update/remove fixture, not macOS Installer or clean OS qualification."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import plistlib
import shutil
import subprocess
import sys
import uuid

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT / 'Doroti/eng'))
from release_receipt import receipt_environment, validate_receipt
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--candidate', type=Path, required=True)
parser.add_argument('--previous', type=Path)
parser.add_argument('--output', type=Path, required=True)
args = parser.parse_args()
output = args.output.resolve()
assert output.is_relative_to(ROOT / 'temp/testing') and not output.exists()
output.mkdir(parents=True)


def candidate(path):
    record = json.loads((path / 'candidate.json').read_text())
    assert record['status'].startswith('PASS') and 'macos' in record['artifacts']
    for name, expected in record['artifacts']['macos'].items():
        file = path / 'macos' / name
        assert file.resolve().is_relative_to((path / 'macos').resolve())
        assert hashlib.sha256(file.read_bytes()).hexdigest() == expected, name
    apps = list((path / 'macos').glob('*.app'))
    if not apps:
        packages = list((path / 'macos').glob('*.pkg'))
        assert len(packages) == 1, 'Expected one app or Apple SDK installer payload'
        extracted = output / ('unpacked-' + uuid.uuid4().hex)
        subprocess.run(['pkgutil', '--expand-full', str(packages[0]), str(extracted)], check=True, timeout=1200)
        apps = [app for app in extracted.rglob('*.app') if app.is_dir()]
    assert len(apps) == 1
    return record, apps[0]


def run(app, label, record):
    subprocess.run(['codesign', '--verify', '--deep', '--strict', str(app)], check=True, timeout=1200)
    plist = plistlib.loads((app / 'Contents/Info.plist').read_bytes())
    executable = plist['CFBundleExecutable']
    assert Path(executable).name == executable, 'Invalid native executable name'
    receipt = output / (label + '-native-consumer.json')
    run_id = uuid.uuid4().hex
    environment = receipt_environment(os.environ | {'NUGET_PACKAGES': str(output / 'empty-cache'),
        'DOROTI_MACOS_GRAPHITE': '1'}, receipt, run_id, record['version'])
    result = subprocess.run([str(app / 'Contents/MacOS' / executable)], cwd=output, env=environment,
        stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True, timeout=90)
    (output / (label + '.log')).write_text(result.stdout)
    assert result.returncode == 0, result.stdout
    if record.get('schemaVersion') == 'doroti.release-candidate/v2':
        validate_receipt(receipt, run_id, record['version'])
    else:
        assert 'PASS: NuGet-only Release native presentation, second window, resize and close.' in result.stdout, result.stdout


new_record, new_app = candidate(args.candidate.resolve())
old_record, old_app = candidate((args.previous or args.candidate).resolve())
installed, staging, backup = (output / name for name in ['CandidateApp.app', 'staging.app', 'previous.app'])
data = output / 'userdata.json'
data.write_text('{"text":"한글 사용자 데이터","selection":[0,2]}')
expected = data.read_bytes()
subprocess.run(['ditto', str(old_app), str(installed)], check=True, timeout=1200)
run(installed, 'installed', old_record)
subprocess.run(['ditto', str(new_app), str(staging)], check=True, timeout=1200)
subprocess.run(['codesign', '--verify', '--deep', '--strict', str(staging)], check=True, timeout=1200)
installed.rename(backup)
staging.rename(installed)
run(installed, 'updated', new_record)
assert data.read_bytes() == expected
shutil.rmtree(backup)
shutil.rmtree(installed)
assert data.read_bytes() == expected and not installed.exists()
report = {'status': 'PASS', 'previous': old_record['version'], 'candidate': new_record['version'],
    'scope': 'local extracted .app install/update/remove and Unicode userdata preservation',
    'updateMode': 'distinct previous candidate' if args.previous else 'same-candidate replacement',
    'providerVersions': new_record.get('versions', {}).get('providers', {}),
    'osInstaller': 'notVerified', 'cleanOs': 'notVerified', 'notarization': 'notVerified'}
(output / 'summary.json').write_text(json.dumps(report, ensure_ascii=False, indent=2))
print(json.dumps(report, ensure_ascii=False))
