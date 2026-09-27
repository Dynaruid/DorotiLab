"""Rebuild the bundled Vulkan shader after editing gaussian.frag."""
import argparse
import hashlib
import json
from pathlib import Path
import shutil
import subprocess
import tempfile

parser = argparse.ArgumentParser()
parser.add_argument('--qsb', default=shutil.which('qsb') or '/usr/lib/qt6/bin/qsb')
args = parser.parse_args()
directory = Path(__file__).resolve().parent
source = directory / 'gaussian.frag'
output = directory / 'gaussian.frag.qsb'
version = subprocess.check_output([args.qsb, '--version'], text=True, timeout=1200).strip()
with tempfile.TemporaryDirectory() as scratch:
    baked = Path(scratch) / output.name
    # Qt 6.4 serialization is readable by the supported Qt 6.6+ runtimes.
    # This backend requires Vulkan, so only SPIR-V is needed in the package.
    subprocess.run([args.qsb, '--qsbversion', '64', '-o', str(baked), str(source)],
                   check=True, timeout=1200)
    output.write_bytes(baked.read_bytes())
metadata = dict(tool=version, qsbCompatibility='6.4', graphicsApi='Vulkan',
                hashes={p.name: hashlib.sha256(p.read_bytes()).hexdigest() for p in (source, output)})
source.with_suffix('.frag.json').write_text(json.dumps(metadata, indent=2) + '\n')
print(f'Baked {output.name} ({output.stat().st_size} bytes)')
