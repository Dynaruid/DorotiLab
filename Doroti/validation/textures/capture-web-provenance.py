"""Record current source and shipped bytes without treating hashes as pixel proof."""
from datetime import datetime, timezone
import hashlib
import json
from pathlib import Path
import subprocess

def git(*args):
    return subprocess.check_output(['git', *args], text=True, encoding='utf-8').strip()

def sha(path):
    digest = hashlib.sha256()
    with path.open('rb') as stream:
        for chunk in iter(lambda: stream.read(1024*1024), b''):
            digest.update(chunk)
    return digest.hexdigest()

names = set(git('diff', '--name-only').splitlines()) | set(git('ls-files', '--others', '--exclude-standard').splitlines())
root = Path('Doroti/artifacts/textures/web')
publish = root/'publish/wwwroot'
result = {
    'timeUtc': datetime.now(timezone.utc).isoformat(), 'head': git('rev-parse', 'HEAD'),
    'sources': {name: sha(Path(name)) for name in sorted(names) if Path(name).is_file()},
    'publish': {str(path.relative_to(publish)): sha(path) for path in sorted(publish.rglob('*'))
                if path.is_file() and path.suffix in {'.js', '.mjs', '.wasm', '.mp4'}},
    'packages': {path.name: sha(path) for path in sorted((root/'packages').glob('*.nupkg'))},
}
(root/'source-provenance.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
print(f"Recorded {len(result['sources'])} source files and {len(result['publish'])} published assets")
