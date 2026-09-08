"""Verify the isolated combination, active boot hashes, and unchanged comparison snapshots."""
from pathlib import Path
import base64
import hashlib
import json
import re
import sys

root = Path(__file__).resolve().parents[2]
out = root / '.doroti/work3'
source = out / 'combined/source'
repaired = '--repaired' in sys.argv
web = out / ('combined/repaired-wwwroot' if repaired else 'combined/wwwroot')
def sha(path): return hashlib.sha256(path.read_bytes()).hexdigest()
candidate = json.loads((root / 'history/26-09-08/work3-webgpu-candidate/manifest.json').read_text())
overrides = json.loads((root / 'history/26-09-08/work3-combined-candidate/manifest.json').read_text())['overrides'] if repaired else {}
for name, entry in candidate['files'].items():
    assert sha(source / name) == (overrides[name]['sha256'] if name in overrides else entry['candidateSha256']), name
    if entry['originalSha256'] is not None: assert sha(root / name) == entry['originalSha256'], name
    else: assert not (root / name).exists(), name
for name in ['DorotiTestbedApp/src/App.cs', 'DorotiTestbedApp/src/ParallelLayoutLab.cs',
             'DorotiTestbedApp/src/MaterialSample/SampleApp.cs',
             'Doroti/src/Doroti.Framework.Rendering/PreparedTreemapLayout.cs',
             'Doroti/src/Doroti.Host.Web/Web/doroti.web.managed-worker.ts']:
    assert (root / name).read_bytes().replace(b'\r\n', b'\n') == (source / name).read_bytes().replace(b'\r\n', b'\n'), name
old = json.loads((out / 'audit-parallel.json').read_text())
paths = {'S1': out.parent / 'threads-fix/repaired-wwwroot', 'S2': out / 'u2/s2/wwwroot',
         'Final': out / 'final/wwwroot', 'Parallel': out / 'parallel/final-wwwroot'}
for name, path in paths.items():
    for entry in old['assets'][name]['files']:
        assert sha(path / entry['path']) == entry['sha256'], (name, entry['path'])
index = (web / 'index.html').read_text(encoding='utf-8')
imports = json.loads(re.search(r'<script type="importmap">(.*?)</script>', index, re.S)[1])
loader = imports['imports']['./_framework/dotnet.js']
script = (web / loader).read_text(encoding='utf-8')
boot = json.loads(script.split('/*json-start*/', 1)[1].split('/*json-end*/', 1)[0])
active = {}
for entries in boot['resources'].values():
    if not isinstance(entries, list): continue
    for entry in entries:
        path = web / '_framework' / entry['name']
        assert path.is_file(), path
        digest = hashlib.sha256(path.read_bytes()).digest()
        if 'hash' in entry: assert entry['hash'] == 'sha256-' + base64.b64encode(digest).decode(), path
        active[entry['name']] = dict(bytes=path.stat().st_size, sha256=digest.hex())
for name, integrity in imports['integrity'].items():
    path = web / name
    if path.exists(): assert integrity == 'sha256-' + base64.b64encode(hashlib.sha256(path.read_bytes()).digest()).decode(), path
resolved = []
for path in (out / 'combined/artifacts/obj').glob('**/project.assets.json'):
    data = json.loads(path.read_text(encoding='utf-8-sig'))
    packages = [key for key in data['libraries'] if key.startswith('SkiaSharp')]
    assert all(key.endswith('/4.154.0-preview.1.26454.9') for key in packages), path
    if packages: resolved.append(str(path.relative_to(root)))
link = (out / 'combined/artifacts/obj/DorotiTestbedApp.Web/release/wasm/for-publish/emcc-link.rsp').read_text()
checks = {term: link.count(term) for term in ['mt,simd', 'DorotiSkiaInterop.js', 'webgpu.cpp', '_wgpuCreateInstance', '_wgpuTextureRelease']}
assert all(value == 1 for value in checks.values()), checks
result = dict(candidateSourceMatches=True, rootCandidateStillRetired=True, comparisonHashesUnchanged=True,
              activeBootResources=active, packageProjects=resolved, linkChecks=checks,
              rawBytes=sum(p.stat().st_size for p in web.rglob('*') if p.is_file() and p.suffix not in ('.br', '.gz')))
(out / ('combined/audit-repaired.json' if repaired else 'combined/audit.json')).write_text(json.dumps(result, indent=2), encoding='utf-8')
print(json.dumps(dict(activeResources=len(active), rawBytes=result['rawBytes'], linkChecks=checks), indent=2))
