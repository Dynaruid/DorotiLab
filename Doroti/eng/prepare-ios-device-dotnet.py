"""Prepare a user-owned dotnet host for the qualified iOS RC1 build tools.

Reuses installed SDKs/workloads and adds the desktop runtime required by bgen.
Does not modify the system installation. Not a workload installer.
"""
import argparse
import hashlib
import json
from pathlib import Path
import platform
import shutil
import subprocess
import tempfile
import urllib.request
import xml.etree.ElementTree as ET
import zipfile

VERSION = '11.0.0-rc.1.26426.105'


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--base', type=Path, default=Path('/usr/local/share/dotnet'))
    parser.add_argument('--destination', type=Path, default=Path.home() / '.local/share/doroti/dotnet-ios-development')
    parser.add_argument('--runtime-package', type=Path, help='Optional previously downloaded Microsoft runtime nupkg')
    args = parser.parse_args()
    if platform.system() != 'Darwin':
        parser.error('This setup is for macOS.')
    rid = {'arm64': 'osx-arm64', 'x86_64': 'osx-x64'}.get(platform.machine())
    if rid is None:
        parser.error('Unsupported Mac architecture')
    base = args.base.resolve()
    destination = args.destination.resolve()
    if not (base / 'dotnet').is_file() or not (base / 'sdk').is_dir():
        parser.error('--base must contain an installed dotnet host and SDK')
    if destination == base or destination in base.parents or base in destination.parents:
        parser.error('Use a destination separate from the base installation')
    package_id = 'microsoft.netcore.app.runtime.' + rid
    url = f'https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet11/nuget/v3/flat2/{package_id}/{VERSION}/{package_id}.{VERSION}.nupkg'
    with tempfile.TemporaryDirectory(prefix='doroti-ios-runtime-') as temporary:
        package = args.runtime_package or Path(temporary) / 'runtime.nupkg'
        if not args.runtime_package:
            print('Downloading Microsoft runtime ' + VERSION, flush=True)
            with urllib.request.urlopen(url, timeout=120) as response, package.open('wb') as output:
                shutil.copyfileobj(response, output)
        with zipfile.ZipFile(package) as archive:
            spec_name = next(name for name in archive.namelist() if name.lower() == package_id + '.nuspec')
            spec = ET.fromstring(archive.read(spec_name))
            values = {node.tag.rsplit('}', 1)[-1]: node.text for node in spec.iter()}
            if values.get('id', '').lower() != package_id or values.get('version') != VERSION:
                parser.error('Runtime package identity/version does not match the qualified toolchain')
            destination.mkdir(parents=True, exist_ok=True)
            for name in ['host', 'sdk', 'sdk-manifests', 'packs', 'metadata', 'templates']:
                source, link = base / name, destination / name
                if not source.exists():
                    continue
                if link.exists() or link.is_symlink():
                    if not link.is_symlink() or link.resolve() != source.resolve():
                        parser.error('Destination contains an unrelated entry: ' + str(link))
                else:
                    link.symlink_to(source, target_is_directory=True)
            host = destination / 'dotnet'
            if not host.exists() or host.read_bytes() != (base / 'dotnet').read_bytes():
                shutil.copy2(base / 'dotnet', host)
            for framework in (base / 'shared').iterdir():
                target = destination / 'shared' / framework.name
                target.mkdir(parents=True, exist_ok=True)
                for version in framework.iterdir():
                    link = target / version.name
                    if not link.exists():
                        link.symlink_to(version, target_is_directory=True)
            runtime = destination / 'shared/Microsoft.NETCore.App' / VERSION
            if runtime.is_symlink():
                # An already installed system runtime needs no extraction.
                runtime = None
            else:
                runtime.mkdir(exist_ok=True)
                prefixes = (f'runtimes/{rid}/native/', f'runtimes/{rid}/lib/net11.0/')
                for name in archive.namelist():
                    if name.startswith(prefixes) and not name.endswith('/'):
                        output = runtime / Path(name).name
                        data = archive.read(name)
                        if not output.exists() or output.read_bytes() != data:
                            output.write_bytes(data)
        (destination / 'doroti-toolchain.json').write_text(json.dumps({
            'base': str(base), 'runtimeVersion': VERSION, 'source': url,
            'sha512': hashlib.sha512(package.read_bytes()).hexdigest()}, indent=2) + '\n')
    subprocess.run([str(destination / 'dotnet'), '--list-runtimes'], check=True)
    print('Use -DotnetPath ' + str(destination / 'dotnet'))


if __name__ == '__main__':
    main()
