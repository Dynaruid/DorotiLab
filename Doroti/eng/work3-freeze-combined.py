"""Copy a fresh snapshot, omitting old framework fingerprints left by incremental publish."""
from pathlib import Path
import json
import re
import shutil

root = Path(__file__).resolve().parents[2] / '.doroti/work3/combined'
source = root / 'artifacts/publish/DorotiTestbedApp.Web/release/wwwroot'
target = root / 'repaired-wwwroot'
if target.exists(): raise SystemExit('Refusing to overwrite an existing snapshot')
html = (source / 'index.html').read_text(encoding='utf-8')
imports = json.loads(re.search(r'<script type="importmap">(.*?)</script>', html, re.S)[1])
loader = source / imports['imports']['./_framework/dotnet.js']
boot = json.loads(loader.read_text(encoding='utf-8').split('/*json-start*/', 1)[1].split('/*json-end*/', 1)[0])
active = {Path(value).name for value in imports['imports'].values()}
for entries in boot['resources'].values():
    if isinstance(entries, list): active.update(entry['name'] for entry in entries)
omitted = []
for path in source.rglob('*'):
    if not path.is_file(): continue
    relative = path.relative_to(source)
    name = path.name.removesuffix('.br').removesuffix('.gz')
    if relative.parts[0] == '_framework' and re.search(r'\.[a-z0-9]{10}\.(wasm|js|mjs|dat)$', name) and name not in active:
        omitted.append(str(relative)); continue
    dest = target / relative
    dest.parent.mkdir(parents=True, exist_ok=True)
    shutil.copyfile(path, dest)
(root / 'freeze.json').write_text(json.dumps(dict(source=str(source), target=str(target), omittedUnusedFingerprints=omitted), indent=2), encoding='utf-8')
print('Frozen', target, '; omitted', len(omitted), 'unused fingerprint files; original snapshot preserved')
