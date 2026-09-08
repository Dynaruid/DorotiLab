"""Freeze S1 evidence and inspect exact target NuGet dependency manifests."""
from concurrent.futures import ThreadPoolExecutor
import hashlib
import io
import json
from pathlib import Path
import subprocess
import urllib.request
import xml.etree.ElementTree as ET
import zipfile

root = Path(__file__).resolve().parents[2]
out = root / '.doroti/work3/u0'
out.mkdir(parents=True, exist_ok=True)
version = '4.154.0-preview.1.26454.9'
sha = lambda p: hashlib.file_digest(p.open('rb'), 'sha256').hexdigest()
manifest = json.loads((root / 'history/26-09-08/wasm-main-runtime-manifest.json').read_text())
checks = []
for group in ('sourceHashes', 'assetHashes'):
    for item in manifest.get(group, []):
        path = (root / manifest['frozenAssets'] if group == 'assetHashes' else root) / item['path']
        actual = sha(path) if path.exists() else None
        checks.append(dict(group=group, **item, actual=actual, matches=actual == item['sha256']))
tracked = subprocess.check_output(['git', 'ls-files'], cwd=root, text=True).splitlines()
tracked = sorted(set(tracked) | {str(p.relative_to(root)).replace('\\', '/')
    for p in (root / 'DorotiTestbedApp').glob('*/packages*.lock.json')})
refs = []
packages = set()
for name in tracked:
    path = root / name
    if name.startswith('history/') or not path.is_file():
        continue
    if path.suffix in ('.props', '.csproj', '.targets') or 'lock.json' in name:
        value = path.read_text(encoding='utf-8-sig')
        if 'SkiaSharp' in value or 'HarfBuzzSharp' in value:
            refs.append(dict(path=name, sha256=sha(path)))
        if 'lock.json' in name:
            for dependencies in json.loads(value).get('dependencies', {}).values():
                packages.update(k for k in dependencies if k.startswith('SkiaSharp'))
tree = ET.parse(root / 'Doroti/Directory.Packages.props')
packages.update(e.attrib['Include'] for e in tree.iter('PackageVersion') if e.attrib['Include'].startswith('SkiaSharp'))

def fetch(package):
    url = f'https://api.nuget.org/v3-flatcontainer/{package.lower()}/{version}/{package.lower()}.{version}.nupkg'
    path = out / (package + '.nupkg')
    if not path.exists():
        with urllib.request.urlopen(url, timeout=90) as response:
            path.write_bytes(response.read())
    with zipfile.ZipFile(path) as archive:
        nuspec = archive.read(next(n for n in archive.namelist() if n.endswith('.nuspec')))
        (out / (package + '.nuspec')).write_bytes(nuspec)
        spec = ET.fromstring(nuspec)
        dependencies = [e.attrib for e in spec.iter() if e.tag.endswith('dependency')]
        native = [n for n in archive.namelist() if n.endswith(('.a', '.dll', '.so', '.dylib'))]
    return dict(package=package, version=version, url=url, sha256=sha(path), dependencies=dependencies, nativeAssets=native)

with ThreadPoolExecutor(max_workers=4) as pool:
    results = list(pool.map(fetch, sorted(packages)))
    # Include native dependencies selected by mobile/desktop TFM groups, even
    # when their platforms cannot be built on this machine.
    pending = {d['id'] for r in results for d in r['dependencies']
               if d['id'].startswith('SkiaSharp')} - packages
    while pending:
        added = list(pool.map(fetch, sorted(pending)))
        results.extend(added)
        packages.update(pending)
        pending = {d['id'] for r in added for d in r['dependencies']
                   if d['id'].startswith('SkiaSharp')} - packages
result = dict(head=subprocess.check_output(['git', 'rev-parse', 'HEAD'], cwd=root, text=True).strip(),
              baselineChecks=checks, references=refs, packages=results)
(out / 'inventory-complete.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
print(json.dumps(dict(packages=len(results), references=len(refs), baselineMismatches=[c for c in checks if not c['matches']]), indent=2))
