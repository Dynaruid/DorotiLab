#!/usr/bin/env python3
"""Inventory bundle/ZIP bytes and hashes without following links or changing inputs."""
import argparse
from collections import defaultdict
import hashlib
import json
import os
from pathlib import Path
import plistlib
import stat
import zipfile


def digest(stream):
    value = hashlib.sha256()
    for block in iter(lambda: stream.read(1024 * 1024), b""):
        value.update(block)
    return value.hexdigest()


def category(name, executable=None):
    path = Path(name)
    lower = name.lower()
    if executable and name == executable:
        return "executable"
    if path.name.startswith("libaot-"):
        return "mono-aot"
    if path.suffix in (".so", ".dylib", ".a") or ".framework/" in lower:
        if "skia" in lower:
            return "skia-native"
        if "mono" in path.name.lower():
            return "mono-native"
        return "native-other"
    if path.suffix.lower() in (".ttf", ".otf", ".woff", ".woff2"):
        return "fonts"
    if path.suffix.lower() in (".png", ".jpg", ".jpeg", ".webp", ".svg", ".heic", ".car"):
        return "images"
    if path.suffix.lower() in (".sksl", ".frag", ".metallib"):
        return "shaders"
    if path.suffix == ".dll" or "assemblies.blob" in lower:
        return "managed-payload"
    if path.suffix == ".dex":
        return "dex"
    return "other"


def inventory(path):
    path = Path(path)
    files = []
    archive_bytes = None
    executable = None
    if path.is_dir():
        plist = path / "Info.plist"
        if plist.is_file():
            executable = plistlib.loads(plist.read_bytes()).get("CFBundleExecutable")
        for root, directories, names in os.walk(path, followlinks=False):
            for name in sorted(names + [d for d in directories if (Path(root) / d).is_symlink()]):
                item = Path(root) / name
                relative = item.relative_to(path).as_posix()
                if item.is_symlink():
                    files.append(dict(path=relative, kind="symlink", target=os.readlink(item)))
                    continue
                if not item.is_file():
                    raise ValueError(f"Unsupported bundle entry: {item}")
                with item.open("rb") as stream:
                    sha256 = digest(stream)
                files.append(dict(path=relative, kind="file", bytes=item.stat().st_size,
                                  sha256=sha256, category=category(relative, executable)))
    else:
        archive_bytes = path.stat().st_size
        with zipfile.ZipFile(path) as archive:
            for item in archive.infolist():
                if item.is_dir():
                    continue
                link = stat.S_ISLNK(item.external_attr >> 16)
                with archive.open(item) as stream:
                    sha256 = digest(stream)
                files.append(dict(path=item.filename, kind="symlink" if link else "file",
                                  bytes=item.file_size, compressedBytes=item.compress_size,
                                  sha256=sha256, category=category(item.filename)))
    totals = defaultdict(int)
    for item in files:
        if item["kind"] == "file":
            totals[item["category"]] += item["bytes"]
    total = sum(totals.values())
    compressed = sum(item.get("compressedBytes", 0) for item in files)
    return dict(schemaVersion="doroti.size-report/v1", path=str(path.resolve()),
                bundleRawBytes=total if path.is_dir() else None,
                archiveUncompressedBytes=total if not path.is_dir() else None,
                archiveBytes=archive_bytes,
                zipEntryCompressedBytes=compressed if archive_bytes is not None else None,
                archiveOverheadBytes=archive_bytes - compressed if archive_bytes is not None else None,
                deviceInstalledBytes=None, storeTransferBytes=None,
                MB=total / 1_000_000, MiB=total / 1_048_576,
                categoriesBytes=dict(sorted(totals.items())), files=sorted(files, key=lambda x: x["path"]))


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("path", type=Path)
    parser.add_argument("--mode", required=True, choices=["Mono", "NativeAot"])
    parser.add_argument("--rid", required=True)
    parser.add_argument("--commit", required=True)
    parser.add_argument("--output", required=True, type=Path)
    args = parser.parse_args()
    report = inventory(args.path)
    report.update(mode=args.mode, rid=args.rid, commit=args.commit)
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(report, indent=2, ensure_ascii=False) + "\n")
    print(f"Inventory: {args.output} ({len(report['files'])} entries)")


if __name__ == "__main__":
    main()
