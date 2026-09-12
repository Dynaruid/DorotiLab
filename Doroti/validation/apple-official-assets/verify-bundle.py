#!/usr/bin/env python3
"""Compare the deployed arm64 Mach-O sections with the pinned NuGet asset.

Apple thinning/stripping/codesigning changes whole-file hashes. Compare every
file-backed section and LC_UUID instead, and retain both whole-file hashes.
Package signature verification is a separate `dotnet nuget verify` operation.
"""
import argparse
import hashlib
import io
import json
from pathlib import Path
import plistlib
import struct
import zipfile

VERSION = '4.154.0-preview.1.26454.9'


def sha(data):
    return hashlib.sha256(data).hexdigest()


def arm64(data):
    if data[:4] == bytes.fromhex('cafebabe'):
        count, = struct.unpack_from('>I', data, 4)
        matches = []
        for i in range(count):
            cpu, subtype, offset, size, align = struct.unpack_from('>IIIII', data, 8 + i * 20)
            if cpu == 0x100000c:
                matches.append(data[offset:offset + size])
        if len(matches) != 1:
            raise ValueError('Expected one arm64 slice')
        data = matches[0]
    if data[:4] != bytes.fromhex('cffaedfe') or struct.unpack_from('<I', data, 4)[0] != 0x100000c:
        raise ValueError('Expected an arm64 Mach-O binary')
    return data


def identity(data):
    data = arm64(data)
    count, = struct.unpack_from('<I', data, 16)
    offset = 32
    sections = {}
    uuid = None
    for _ in range(count):
        command, size = struct.unpack_from('<II', data, offset)
        if size < 8 or offset + size > len(data):
            raise ValueError('Invalid Mach-O load command')
        if command == 0x1b:
            uuid = data[offset + 8:offset + 24].hex()
        if command == 0x19:
            nsects, = struct.unpack_from('<I', data, offset + 64)
            for i in range(nsects):
                section = offset + 72 + i * 80
                name, segment, addr, length, start = struct.unpack_from('<16s16sQQI', data, section)
                flags, = struct.unpack_from('<I', data, section + 64)
                if flags & 0xff in (1, 12, 18):  # zero-fill has no bytes in the file
                    continue
                key = segment.rstrip(b'\0').decode() + '/' + name.rstrip(b'\0').decode()
                if key in sections or start + length > len(data):
                    raise ValueError('Duplicate or invalid Mach-O section')
                sections[key] = {'bytes': length, 'sha256': sha(data[start:start + length])}
        offset += size
    if uuid is None or '__TEXT/__text' not in sections:
        raise ValueError('Missing UUID or executable code section')
    return {'uuid': uuid, 'sections': sections}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('bundle', type=Path)
    parser.add_argument('--platform', choices=['macos', 'maccatalyst'], required=True)
    parser.add_argument('--packages', type=Path, default=Path.home() / '.nuget/packages')
    parser.add_argument('--output', type=Path, required=True)
    args = parser.parse_args()
    package = 'skiasharp.nativeassets.' + args.platform
    package_dir = args.packages / package / VERSION
    archive = package_dir / f'{package}.{VERSION}.nupkg'
    native_entry = ('runtimes/osx/native/libSkiaSharp.dylib' if args.platform == 'macos'
                    else 'runtimes/maccatalyst/native/libSkiaSharp.framework.zip')
    with zipfile.ZipFile(archive) as z:
        package_asset = z.read(native_entry)
    if (package_dir / native_entry).read_bytes() != package_asset:
        raise ValueError('Cached asset differs from NuGet archive')
    if args.platform == 'maccatalyst':
        with zipfile.ZipFile(io.BytesIO(package_asset)) as z:
            package_asset = z.read('libSkiaSharp.framework/Versions/A/libSkiaSharp')
    # Resolve framework symlinks so the same native image is counted only once.
    candidates = {p.resolve() for p in args.bundle.rglob('*')
                  if p.is_file() and p.name in ('libSkiaSharp', 'libSkiaSharp.dylib')}
    if len(candidates) != 1:
        raise ValueError(f'Expected one deployed Skia binary, found {candidates}')
    deployed = candidates.pop()
    source_id = identity(package_asset)
    deployed_bytes = deployed.read_bytes()
    deployed_id = identity(deployed_bytes)
    if source_id != deployed_id:
        raise ValueError('Deployed UUID/file-backed sections differ from official arm64 asset')
    with (args.bundle / 'Contents/Info.plist').open('rb') as f:
        plist = plistlib.load(f)
    report = {'status': 'PASS', 'platform': args.platform, 'package': package, 'version': VERSION,
              'archive': str(archive), 'archiveSha256': sha(archive.read_bytes()),
              'sourceSha256': sha(package_asset), 'deployed': str(deployed), 'deployedSha256': sha(deployed_bytes),
              'arm64': source_id, 'applicationId': plist.get('CFBundleIdentifier'),
              'displayVersion': plist.get('CFBundleShortVersionString'), 'applicationVersion': plist.get('CFBundleVersion'),
              'runtimeLoad': 'notVerified; collect the running process image path separately',
              'signature': 'notVerified; run dotnet nuget verify separately'}
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(report, indent=2) + '\n')
    print(f'PASS: {deployed}')


if __name__ == '__main__':
    main()
