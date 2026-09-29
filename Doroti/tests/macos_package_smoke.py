"""Local extracted-app install/update/remove fixture, not macOS Installer or clean OS qualification."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import shutil
import subprocess

ROOT = Path(__file__).resolve().parents[2]
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
    assert len(apps) == 1
    return record['version'], apps[0]


def run(app, label):
    subprocess.run(['codesign', '--verify', '--deep', '--strict', str(app)], check=True, timeout=1200)
    result = subprocess.run([str(app / 'Contents/MacOS/CandidateApp.MacOS')], cwd=output,
        env=os.environ | {'NUGET_PACKAGES': str(output / 'empty-cache'), 'DOROTI_RELEASE_SMOKE': '1'},
        stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True, timeout=1200)
    (output / (label + '.log')).write_text(result.stdout)
    assert result.returncode == 0 and 'PASS: NuGet-only Release native presentation, second window, resize and close.' in result.stdout, result.stdout


new_version, new_app = candidate(args.candidate.resolve())
old_version, old_app = candidate((args.previous or args.candidate).resolve())
installed, staging, backup = (output / name for name in ['CandidateApp.app', 'staging.app', 'previous.app'])
data = output / 'userdata.json'
data.write_text('{"text":"한글 사용자 데이터","selection":[0,2]}')
expected = data.read_bytes()
subprocess.run(['ditto', str(old_app), str(installed)], check=True, timeout=1200)
run(installed, 'installed')
subprocess.run(['ditto', str(new_app), str(staging)], check=True, timeout=1200)
subprocess.run(['codesign', '--verify', '--deep', '--strict', str(staging)], check=True, timeout=1200)
installed.rename(backup)
staging.rename(installed)
run(installed, 'updated')
assert data.read_bytes() == expected
shutil.rmtree(backup)
shutil.rmtree(installed)
assert data.read_bytes() == expected and not installed.exists()
report = {'status': 'PASS', 'previous': old_version, 'candidate': new_version,
    'scope': 'local extracted .app install/update/remove and Unicode userdata preservation',
    'osInstaller': 'notVerified', 'cleanOs': 'notVerified', 'notarization': 'notVerified'}
(output / 'summary.json').write_text(json.dumps(report, ensure_ascii=False, indent=2))
print(json.dumps(report, ensure_ascii=False))
