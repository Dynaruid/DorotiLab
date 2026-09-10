"""Stage the exact Android Graphite asset without replacing stock NuGet files."""
import argparse
import hashlib
import json
import os
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
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--cpu', choices=['arm64', 'x64'], default='arm64')
    args = parser.parse_args()
    stage = DOROTI / 'artifacts/native-graphite/distribution' / f'android-{args.cpu}'
    manifest = stage / 'build-provenance.json'
    inputs = {name: sha(HERE / name) for name in ['stage-android.py', 'build-android.py', 'doroti_graphite_interop.inc']}
    if manifest.exists():
        data = json.loads(manifest.read_text())
        if data.get('inputs') == inputs and all((stage/name).exists() and sha(stage/name) == value for name,value in data['files'].items()):
            print(f'Graphite android-{args.cpu} distribution is current: {stage}')
            return
    sdk = Path(os.environ.get('ANDROID_HOME', Path.home() / 'AppData/Local/Android/Sdk'))
    ndk = Path(os.environ['ANDROID_NDK_HOME']) if os.environ.get('ANDROID_NDK_HOME') else sorted((sdk/'ndk').iterdir(), key=lambda p: [int(n) for n in p.name.split('.')])[-1]
    subprocess.run([sys.executable, str(HERE/'build-android.py'), '--cpu', args.cpu, '--ndk', str(ndk)], check=True, timeout=1200)
    stage.mkdir(parents=True, exist_ok=True)
    source = DOROTI / 'artifacts/native-graphite/skia-build'
    shutil.copy2(source/f'out/doroti-android-{args.cpu}/libSkiaSharp.so', stage/'libSkiaSharp.so')
    shutil.copy2(source/'LICENSE', stage/'LICENSE.txt')
    notices = Path.home()/'.nuget/packages/skiasharp.nativeassets.win32/4.154.0-preview.1.26454.9/THIRD-PARTY-NOTICES.txt'
    shutil.copy2(notices, stage/'THIRD-PARTY-NOTICES.txt')
    data = {'schema':'doroti.graphite-distribution/v1','bridgeAbi':3,'rid':f'android-{args.cpu}',
            'minimumApi':24,'inputs':inputs,'files':{name:sha(stage/name) for name in ['libSkiaSharp.so','LICENSE.txt','THIRD-PARTY-NOTICES.txt']}}
    manifest.write_text(json.dumps(data,indent=2))
    print(manifest)

if __name__ == '__main__':
    main()
