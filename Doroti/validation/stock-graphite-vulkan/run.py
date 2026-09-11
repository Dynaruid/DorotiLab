"""Independent official Graphite experiment. Every child uses a 1,200s timeout.

Run from any directory: python path/to/run.py
The original research evidence and user package caches are never modified.
"""
import datetime
import hashlib
import json
import os
from pathlib import Path
import subprocess
import sys
import urllib.request
import zipfile

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[2]
VERSION = "4.154.0-preview.1.26454.9"
STAMP = datetime.datetime.now(datetime.timezone.utc).strftime("%Y%m%dT%H%M%S%fZ")
OUT = ROOT / "Doroti/artifacts/stock-graphite" / STAMP
OUT.mkdir(parents=True)
COMMANDS = []


def save(path, value):
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(value, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")


def run(label, args):
    directory = OUT / label
    directory.mkdir(parents=True, exist_ok=True)
    start = datetime.datetime.now(datetime.timezone.utc)
    with (directory / "stdout.log").open("w", encoding="utf-8") as stdout, (directory / "stderr.log").open("w", encoding="utf-8") as stderr:
        # Repository wrapper kills the whole child tree on Windows at 1,200 seconds.
        command = [sys.executable, str(ROOT / "Doroti/validation/run-with-timeout.py"), *map(str, args)]
        result = subprocess.run(command, cwd=ROOT, stdout=stdout, stderr=stderr, check=False)
    record = dict(label=label, command=command, utc=start.isoformat(), exitCode=result.returncode,
                  elapsedSeconds=(datetime.datetime.now(datetime.timezone.utc)-start).total_seconds(), timeoutSeconds=1200)
    COMMANDS.append(record)
    save(directory / "process.json", record)
    save(OUT / "commands.json", COMMANDS)
    print(label, result.returncode, flush=True)
    return result.returncode


def main():
    print(OUT, flush=True)
    for label, cmd in [
        ("W0-0/head", ["git", "rev-parse", "HEAD"]),
        ("W0-0/status", ["git", "status", "--short"]),
        ("W0-0/sdk", ["dotnet", "--info"]),
        ("W0-0/workloads", ["dotnet", "workload", "list"]),
        ("W0-0/vulkan", ["vulkaninfo", "--summary"]),
        ("W0-0/android", ["adb", "devices", "-l"]),
        ("W0-0/dependencies", ["rg", "-n", "doroti_graphite_|EnsureDorotiGraphite|native-graphite|stage-android.py|stage-linux.py|ExcludeAssets=\"all\"", "Doroti/src", "Doroti/eng", "Doroti/templates"]),
    ]:
        run(label, cmd)
    manifests = []
    for path in sorted((ROOT / "Doroti/src").glob("Doroti.Target.*/doroti-target-manifest.json")):
        manifests.append(dict(path=str(path.relative_to(ROOT)), manifest=json.loads(path.read_text(encoding="utf-8-sig"))))
    save(OUT / "W0-0/targets.json", manifests)
    source_identity = [dict(path=str(p.relative_to(ROOT)), sha256=hashlib.sha256(p.read_bytes()).hexdigest())
                       for p in sorted(HERE.glob("*")) if p.is_file()]
    save(OUT / "W0-0/harness-source-identity.json", source_identity)
    # Download from the official feed into the evidence directory; compare the full archive
    # and deployed DLL against the installed NuGet asset. Signature check is a separate command.
    package = "skiasharp.nativeassets.win32"
    url = f"https://api.nuget.org/v3-flatcontainer/{package}/{VERSION}/{package}.{VERSION}.nupkg"
    archive = OUT / "W0-0" / f"{package}.{VERSION}.nupkg"
    with urllib.request.urlopen(url, timeout=60) as response:
        archive.write_bytes(response.read())
    cache = Path(os.environ.get("NUGET_PACKAGES", str(Path.home() / ".nuget/packages")))
    cache_dir = cache / package / VERSION
    entry = "runtimes/win-x64/native/libSkiaSharp.dll"
    with zipfile.ZipFile(archive) as z:
        payload = z.read(entry)
    official = OUT / "W0-1/official/libSkiaSharp.dll"
    official.parent.mkdir(parents=True)
    official.write_bytes(payload)
    identity = dict(url=url, package=package, version=VERSION, entry=entry,
                    sha256=hashlib.sha256(payload).hexdigest(), archiveSha256=hashlib.sha256(archive.read_bytes()).hexdigest(),
                    installedAssetMatches=payload == (cache_dir / entry).read_bytes(),
                    installedArchiveMatches=archive.read_bytes() == (cache_dir / archive.name).read_bytes())
    save(OUT / "W0-0/official-asset.json", identity)
    if not identity["installedAssetMatches"] or not identity["installedArchiveMatches"]:
        raise RuntimeError("Official package differs from local cache")
    if run("W0-0/signature", ["dotnet", "nuget", "verify", archive, "--all"]):
        return 1
    project = HERE / "StockGraphite.csproj"
    if run("W0-1/build", ["dotnet", "build", project, "-c", "Release", "--nologo", f"-bl:{OUT / 'W0-1/build/build.binlog'}"]):
        return 1
    exe = ROOT / "Doroti/artifacts/validation/build/stock-graphite-vulkan/bin/Release/net10.0/win-x64/StockGraphite.exe"
    results = []
    for gpu in ("AMD", "NVIDIA"):
        for mode in ("sync", "async", "shutdown"):
            stage = "W0-4-early" if mode == "shutdown" else "W0-1"
            label = f"{stage}/win-x64/{gpu}/{mode}"
            code = run(label, [exe, official, gpu, mode, OUT / label / "report.json"])
            results.append(dict(gpu=gpu, mode=mode, exitCode=code))
    save(OUT / "summary.json", dict(status="PASS" if all(r["exitCode"] == 0 for r in results) else "FAIL", results=results,
                                   product="notVerified", presentation="notVerified"))
    return 0 if all(r["exitCode"] == 0 for r in results) else 1


if __name__ == "__main__":
    sys.exit(main())
