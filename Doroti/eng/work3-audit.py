"""Record resolved native packages, frozen asset hashes, and link ownership."""
import hashlib
import json
import sys
from pathlib import Path

root = Path(__file__).resolve().parents[2]
out = root / '.doroti/work3'
version = '4.154.0-preview.1.26454.9'
parallel = '--parallel' in sys.argv
final = '--final' in sys.argv or parallel
def sha(path):
    with path.open('rb') as stream:
        return hashlib.file_digest(stream, 'sha256').hexdigest()

assets = {}
asset_roots = [('S1', root / '.doroti/threads-fix/repaired-wwwroot'), ('S2', out / 'u2/s2/wwwroot')]
if final:
    asset_roots.append(('Final', out / 'final/wwwroot'))
if parallel:
    asset_roots.append(('Parallel', out / 'parallel/final-wwwroot'))
for name, path in asset_roots:
    entries = [dict(path=str(p.relative_to(path)).replace('\\','/'), bytes=p.stat().st_size, sha256=sha(p))
               for p in sorted(path.rglob('*')) if p.is_file()]
    assets[name] = dict(files=entries, native=[e for e in entries if '/dotnet.native.' in e['path']],
                        rawBytes=sum(e['bytes'] for e in entries if not e['path'].endswith(('.gz','.br'))))
resolved = []
for path in sorted(out.glob('**/project.assets.json')):
    data = json.loads(path.read_text(encoding='utf-8-sig'))
    packages = {key: value.get('sha512') for key,value in data['libraries'].items()
                if key.startswith(('SkiaSharp/', 'SkiaSharp.', 'HarfBuzzSharp/', 'HarfBuzzSharp.'))}
    if packages:
        resolved.append(dict(path=str(path.relative_to(root)), packages=packages))
mixed = [r for r in resolved if any(key.startswith('SkiaSharp') and not key.endswith('/'+version) for key in r['packages'])]
link_root = 'parallel' if parallel else 'final' if final else 'u2/s2'
link = (out / link_root / 'artifacts/obj/DorotiTestbedApp.Web/release/wasm/for-publish/emcc-link.rsp').read_text()
checks = {term:link.count(term) for term in ['mt,simd', 'DorotiSkiaInterop.js', 'webgpu.cpp', '_wgpuCreateInstance', '_wgpuTextureRelease']}
source = {}
for p in [root / 'Doroti/Directory.Packages.props', root / 'Doroti/src/Doroti.Runner.Sdk/Sdk/Sdk.props']:
    source[str(p.relative_to(root))] = sha(p)
result = dict(assets=assets,resolved=resolved,mixed=mixed,linkChecks=checks,source=source,
              limitations='Historical failed build directories are included; physical/Apple/Android execution is not established by asset resolution.')
if final:
    original = json.loads((out/'audit.json').read_text())
    for name in ['S1','S2']:
        if assets[name] != original['assets'][name]: raise SystemExit('Frozen baseline changed: '+name)
    manifest = json.loads((root/'history/26-09-08/work3-webgpu-candidate/manifest.json').read_text())
    for name, entry in manifest['files'].items():
        path=root/name
        if entry['originalSha256'] is None:
            if path.exists(): raise SystemExit('Rejected G source returned: '+name)
        elif sha(path)!=entry['originalSha256']:
            raise SystemExit('G source was not retired: '+name)
    result['retiredCandidateVerified'] = True
(out / ('audit-parallel.json' if parallel else 'audit-final.json' if final else 'audit.json')).write_text(json.dumps(result,indent=2),encoding='utf-8')
print(json.dumps(dict(mixed=mixed,linkChecks=checks,rawBytes={k:v['rawBytes'] for k,v in assets.items()}),indent=2))
if mixed or any(value!=1 for value in checks.values()):
    raise SystemExit('Mixed packages or duplicated/missing link inputs')
