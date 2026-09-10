#!/usr/bin/env python3
"""Stage the pinned Graphite Linux asset for the local distribution's C/C++ runtime."""
import hashlib
import json
from pathlib import Path
import shutil
import subprocess
import sys
import xml.etree.ElementTree as ET

HERE = Path(__file__).resolve().parent
DOROTI = HERE.parents[1]

def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()

def main():
    if ET.parse(DOROTI/'Directory.Packages.props').find(".//PackageVersion[@Include='SkiaSharp']").attrib['Version'] != '4.154.0-preview.1.26454.9':
        raise RuntimeError('The managed Skia pin differs from the native Graphite recipe.')
    if sys.platform != 'linux':
        raise RuntimeError('Linux native staging must run on the intended Linux build distribution.')
    stage = DOROTI/'artifacts/native-graphite/distribution/linux-x64'
    manifest = stage/'build-provenance.json'
    inputs = {name:sha(HERE/name) for name in ['stage-linux.py','build-linux.py','doroti_graphite_interop.inc']}
    if manifest.exists():
        data=json.loads(manifest.read_text())
        if data.get('inputs') == inputs and all((stage/name).exists() and sha(stage/name)==value for name,value in data['files'].items()):
            print(f'Graphite linux-x64 distribution is current: {stage}')
            return
    subprocess.run([sys.executable,str(HERE/'build-linux.py')],check=True,timeout=1200)
    source=DOROTI/'artifacts/native-graphite/skia-linux-build'
    stage.mkdir(parents=True,exist_ok=True)
    shutil.copy2(source/'out/doroti-linux-x64/libSkiaSharp.so',stage/'libDorotiGraphite.so')
    shutil.copy2(source/'LICENSE',stage/'LICENSE.txt')
    notices=Path.home()/'.nuget/packages/skiasharp.nativeassets.linux/4.154.0-preview.1.26454.9/THIRD-PARTY-NOTICES.txt'
    shutil.copy2(notices,stage/'THIRD-PARTY-NOTICES.txt')
    data={'schema':'doroti.graphite-distribution/v1','rid':'linux-x64','bridgeAbi':3,'inputs':inputs,
          'compatibility':'Local build distribution glibc/libstdc++; other Linux distributions require qualification.',
          'files':{name:sha(stage/name) for name in ['libDorotiGraphite.so','LICENSE.txt','THIRD-PARTY-NOTICES.txt']}}
    manifest.write_text(json.dumps(data,indent=2))
    print(manifest)

if __name__ == '__main__':
    main()
