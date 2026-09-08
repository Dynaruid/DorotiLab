"""Overlay current U/T work and the preserved WebGPU candidate in an empty detached worktree."""
from pathlib import Path
import hashlib
import json
import subprocess

root = Path(__file__).resolve().parents[2]
target = root / '.doroti/work3/combined/source'
archive = root / 'history/26-09-08/work3-webgpu-candidate'
manifest = json.loads((archive / 'manifest.json').read_text())
def git(*args, cwd=root):
    return subprocess.check_output(['git', *args], cwd=cwd)
def sha(data): return hashlib.sha256(data).hexdigest()
if not target.is_dir() or git('status', '--porcelain', cwd=target).strip():
    raise SystemExit('Requires an existing clean detached worktree at .doroti/work3/combined/source')
if git('rev-parse', 'HEAD', cwd=target).decode().strip() != manifest['head']:
    raise SystemExit('Unexpected worktree base')
patch = git('diff', '--binary', manifest['head'])
(target.parent / 'current-ut.patch').write_bytes(patch)
subprocess.run(['git', 'apply', '--whitespace=nowarn', '-'], input=patch, cwd=target, check=True)
copied = {}
for name in git('ls-files', '--others', '--exclude-standard').decode().splitlines():
    if not name.startswith(('Doroti/', 'DorotiTestbedApp/')): continue
    path = root / name
    if path.suffix not in ('.cs', '.csproj', '.ts', '.mjs', '.py', '.props', '.targets', '.json', '.md'): continue
    dest = target / name
    dest.parent.mkdir(parents=True, exist_ok=True)
    data = path.read_bytes()
    dest.write_bytes(data)
    copied[name] = sha(data)
for name, entry in manifest['files'].items():
    path = target / name
    if entry['originalSha256'] is None:
        if path.exists(): raise SystemExit('Added G path already exists: ' + name)
    elif sha(path.read_bytes().replace(b'\r\n', b'\n')) != sha((root / name).read_bytes().replace(b'\r\n', b'\n')):
        raise SystemExit('Unexpected source difference before G overlay: ' + name)
    data = (archive / 'sources' / (name + '.txt')).read_bytes()
    if sha(data) != entry['candidateSha256']: raise SystemExit('Candidate hash mismatch: ' + name)
    # Refuse to replace newly edited root G files, including invisible changes.
    if entry['originalSha256'] is not None and sha((root / name).read_bytes()) != entry['originalSha256']:
        raise SystemExit('Root G source changed since retirement: ' + name)
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_bytes(data)
    copied[name] = sha(data)
overrides = root / 'history/26-09-08/work3-combined-candidate/manifest.json'
if overrides.exists():
    for name, entry in json.loads(overrides.read_text())['overrides'].items():
        if name not in manifest['files']: raise SystemExit('Override is outside G ownership: ' + name)
        data = (overrides.parent / entry['file']).read_bytes()
        if sha(data) != entry['sha256']: raise SystemExit('Override hash mismatch: ' + name)
        (target / name).write_bytes(data)
        copied[name] = sha(data)
combined = git('diff', '--binary', 'HEAD', cwd=target)
(target.parent / 'combined.patch').write_bytes(combined)
(target.parent / 'source-state.json').write_text(json.dumps(dict(
    base=manifest['head'], currentPatchSha256=sha(patch), combinedTrackedPatchSha256=sha(combined),
    copiedSources=copied, candidateManifestSha256=sha((archive / 'manifest.json').read_bytes()),
    limitations='Preserved G3 lifetime failure; explicit experimental combination, not product renderer adoption.'
), indent=2), encoding='utf-8')
print('Prepared isolated combination at', target)
