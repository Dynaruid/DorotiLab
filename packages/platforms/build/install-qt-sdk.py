"""Explicit, hash-locked Qt SDK installation (no MSBuild/network side effects).

The lock contains version, kind, frameworkVersion, payloadPrefix and archives:
[{url: https://..., sha256: ..., format: 7z}]. Source archives may instead use
format=file and a relative destination. Runtime review is a separate operation.
"""
import argparse
import hashlib
import json
import re
from pathlib import Path
import shutil
import subprocess
import tempfile
import urllib.request

from qt_runtime import digest, relative, write_json


def install(lock_path, output, cache):
    raw = lock_path.read_bytes()
    lock = json.loads(raw)
    if lock.get("schemaVersion") != "doroti.qt-sdk-lock/v1" or lock.get("architecture") != "linux-x64":
        raise ValueError("Expected a linux-x64 Qt SDK lock")
    if output.exists():
        receipt = output / "doroti-sdk-install.json"
        if receipt.is_file() and json.loads(receipt.read_text())["lockSha256"] == hashlib.sha256(raw).hexdigest():
            old = json.loads(receipt.read_text())
            if all((output / name).is_file() and digest(output / name) == value for name, value in old["files"].items()):
                return output
        raise ValueError("SDK output exists with different/changed content; choose a new prefix")
    output.parent.mkdir(parents=True, exist_ok=True)
    cache.mkdir(parents=True, exist_ok=True)
    with tempfile.TemporaryDirectory(prefix=".qt-sdk-", dir=output.parent) as temporary:
        stage = Path(temporary)
        sdk = stage / "sdk"
        sdk.mkdir()
        for entry in lock["archives"]:
            url, checksum = entry["url"], entry["sha256"]
            if not url.startswith("https://") or len(checksum) != 64 or any(c not in "0123456789abcdef" for c in checksum):
                raise ValueError("Every SDK archive requires an HTTPS URL and SHA-256")
            artifact = cache / checksum
            if not artifact.is_file() or digest(artifact) != checksum:
                partial = stage / "download"
                with urllib.request.urlopen(url, timeout=60) as response, partial.open("wb") as stream:
                    if not response.url.startswith("https://"):
                        raise ValueError("SDK download redirected away from HTTPS")
                    shutil.copyfileobj(response, stream)
                if digest(partial) != checksum:
                    raise ValueError("SDK archive SHA-256 mismatch: " + url)
                partial.replace(artifact)
            if entry["format"] == "file":
                target = sdk / relative(entry["destination"])
                target.parent.mkdir(parents=True, exist_ok=True)
                shutil.copyfile(artifact, target)
            elif entry["format"] == "7z":
                # Validate member paths; 7-Zip's default link protections remain
                # enabled. Inspect every extracted link before copying payloads.
                listing = subprocess.check_output(["7z", "l", "-slt", "--", str(artifact)], text=True)
                listing = listing.split("----------", 1)[1]
                for line in listing.splitlines():
                    for key in ("Path = ", "Symbolic Link = ", "Hard Link = "):
                        if line.startswith(key):
                            value = line[len(key):]
                            if key == "Path = ":
                                relative(value)
                            elif Path(value).is_absolute() or ".." in Path(value).parts:
                                raise ValueError("SDK archive contains a link outside its prefix")
                extracted = stage / "extracted"
                extracted.mkdir(exist_ok=True)
                result = subprocess.run(["7z", "x", "-y", "-o" + str(extracted), "--", str(artifact)], capture_output=True, text=True)
                if result.returncode:
                    # Qt archives contain normal .so -> .so.6 -> .so.VERSION
                    # chains. Recent 7-Zip refuses the first link in such a
                    # chain. Reconstruct only these checked, prefix-local links.
                    errors = [line for line in result.stderr.splitlines() if line.strip()]
                    links = [re.fullmatch(r"ERROR: Dangerous link via another link was ignored : (.+) : (.+)", line) for line in errors]
                    if result.returncode != 2 or not links or not all(links):
                        raise ValueError("SDK extraction failed: " + result.stderr)
                    for link in links:
                        name, target = link.groups()
                        destination = extracted / relative(name)
                        relative(target)
                        resolved = (destination.parent / target).resolve()
                        if not resolved.is_relative_to(extracted) or not resolved.is_file() or not destination.parent.resolve().is_relative_to(extracted):
                            raise ValueError("SDK link leaves extracted payload")
                        if destination.exists():
                            if destination.is_symlink() or destination.stat().st_size:
                                raise ValueError("SDK link repair would overwrite a file")
                            destination.unlink()
                        destination.symlink_to(target)
                prefix = extracted / relative(lock["payloadPrefix"])
                if not prefix.is_dir():
                    raise ValueError("SDK archive lacks locked payloadPrefix")
                for path in extracted.rglob("*"):
                    if path.is_symlink() and not path.resolve().is_relative_to(extracted):
                        raise ValueError("SDK extraction contains an escaping symlink")
                destination = sdk / relative(entry.get("destination", "."))
                shutil.copytree(prefix, destination, dirs_exist_ok=True, symlinks=True)
                shutil.rmtree(extracted)
            else:
                raise ValueError("Unsupported SDK archive format")
        if (sdk / "bin").is_dir():
            (sdk / "bin/qt.conf").write_text("[Paths]\nPrefix=..\n")
        receipt = dict(schemaVersion="doroti.qt-sdk-install/v1", lockSha256=hashlib.sha256(raw).hexdigest(), lock=lock,
                       files={str(path.relative_to(sdk)): digest(path) for path in sdk.rglob("*") if path.is_file()})
        write_json(sdk / "doroti-sdk-install.json", receipt)
        sdk.rename(output)
    return output


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--lock", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--cache", type=Path, required=True)
    args = parser.parse_args()
    try:
        print(install(args.lock.resolve(), args.output.resolve(), args.cache.resolve()))
    except (ValueError, OSError, KeyError, subprocess.CalledProcessError) as error:
        raise SystemExit("Qt SDK: " + str(error))
