"""Portable update transaction regression; only modifies the aggregate's owned scratch directory."""
import hashlib
import json
from pathlib import Path
import subprocess
import sys

root = Path(__file__).resolve().parents[2]
scratch = Path(sys.argv[1]).resolve()
assert scratch.is_relative_to(root / 'temp/testing')
scratch.mkdir(parents=True)
installer = root / 'Doroti/eng/install-candidate.ps1'
destination = scratch / 'installed'


def candidate(version):
    directory = scratch / version
    payload = directory / 'windows'
    payload.mkdir(parents=True)
    hashes = {}
    for name in ['app.txt', 'asset.txt']:
        data = (name + version + '한글').encode()
        (payload / name).write_bytes(data)
        hashes[name] = hashlib.sha256(data).hexdigest()
    (directory / 'candidate.json').write_text(json.dumps({
        'status': 'PASS: fixture only', 'version': version, 'revision': 'fixture',
        'artifacts': {'windows': hashes}}))
    return directory


def invoke(script, *args, succeeds=True):
    result = subprocess.run(['pwsh', '-NoProfile', '-File', str(script), *map(str, args)],
                            capture_output=True, text=True, encoding='utf-8', errors='replace')
    assert (result.returncode == 0) == succeeds, result.stdout + result.stderr
    return result


first, second = candidate('1.0.0'), candidate('1.0.1')
invoke(installer, '-InstallRoot', destination, '-CandidateRoot', first)
canary = destination / 'userdata/canary.txt'
canary.write_text('사용자 데이터', encoding='utf-8')
current = destination / 'current.json'
before = current.read_bytes()
# Inject one ordinary I/O failure after a successful copy, without changing the installer.
fault = scratch / 'copy-failure.ps1'
fault.write_text('''param($Installer, $InstallRoot, $CandidateRoot)
$ErrorActionPreference = 'Stop'
$script:copies = 0
function Copy-Item {
    param($LiteralPath, $Destination)
    if (++$script:copies -eq 2) { throw 'Injected copy failure' }
    Microsoft.PowerShell.Management\\Copy-Item -LiteralPath $LiteralPath -Destination $Destination
}
& $Installer -InstallRoot $InstallRoot -CandidateRoot $CandidateRoot
''', encoding='utf-8')
result = invoke(fault, '-Installer', installer, '-InstallRoot', destination, '-CandidateRoot', second, succeeds=False)
assert 'Injected copy failure' in result.stderr
assert current.read_bytes() == before
assert not (destination / 'versions/1.0.1-windows').exists(), 'Failed copy reserved the version; retry cannot recover.'
assert not list((destination / 'versions').glob('.staging-*')), 'Failed transaction retained its staging directory.'
invoke(installer, '-InstallRoot', destination, '-CandidateRoot', second)
assert json.loads(current.read_text(encoding='utf-8-sig'))['version'] == '1.0.1'
before = current.read_bytes()
(first / 'windows/app.txt').write_text('tampered')
invoke(installer, '-InstallRoot', destination, '-CandidateRoot', first, succeeds=False)
assert current.read_bytes() == before
extra = destination / 'versions/1.0.1-windows/unlisted.dll'
extra.write_text('unlisted payload')
result = invoke(installer, '-InstallRoot', destination, '-CandidateRoot', second, succeeds=False)
assert 'Unlisted installed file' in result.stderr
assert current.read_bytes() == before
invoke(installer, '-InstallRoot', destination, '-Action', 'Remove')
assert not current.exists() and not (destination / 'versions').exists()
assert canary.read_text(encoding='utf-8') == '사용자 데이터'
print('PASS: interrupted install rollback/retry, payload tamper/extra-file rejection, update/removal preserve userdata.')
