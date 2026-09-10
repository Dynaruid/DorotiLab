"""Build and stage the pinned Windows Graphite asset, without changing the NuGet cache.

Requires Windows, VS C++ x64 tools/SDK, LLVM, Ninja, Git, Python. Each child
operation has a 20-minute timeout. Sources and logs stay under artifacts.
"""
import argparse
import hashlib
import json
import os
from pathlib import Path
import shutil
import subprocess
import sys

SKIA_REVISION = "cc43af052d3d98e605bee4ddc98671dafded1c57"
SKIASHARP_REVISION = "143a933a753dbfeca1909524b2c06c546c5c3e20"
HERE = Path(__file__).resolve().parent
DOROTI = HERE.parents[1]
PACKAGE_VERSION = "4.154.0-preview.1.26454.9"


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def stage_inputs():
    return {"builderSha256": sha(Path(__file__)), "bridgeSha256": sha(HERE / "doroti_graphite_interop.inc"),
            "skiaRevision": SKIA_REVISION, "skiaSharpRevision": SKIASHARP_REVISION,
            "skiaSharpPackageVersion": PACKAGE_VERSION}


def stage_is_current(directory):
    try:
        manifest = json.loads((directory / "build-provenance.json").read_text())
        return manifest["inputs"] == stage_inputs() and manifest["bridgeAbi"] == 3 and all(
            sha(directory / name) == digest for name, digest in manifest["files"].items())
    except (OSError, KeyError, ValueError):
        return False


def stage_asset(directory, library, build_manifest):
    packages = Path(os.environ.get("NUGET_PACKAGES", str(Path.home() / ".nuget/packages")))
    upstream = packages / "skiasharp.nativeassets.win32" / PACKAGE_VERSION
    directory.mkdir(parents=True, exist_ok=True)
    for source, name in [(library, "libSkiaSharp.dll"), (upstream / "LICENSE.txt", "LICENSE.txt"),
                         (upstream / "THIRD-PARTY-NOTICES.txt", "THIRD-PARTY-NOTICES.txt")]:
        shutil.copyfile(source, directory / name)
    manifest = {"schema": "doroti.graphite-distribution/v1", "rid": "win-x64", "bridgeAbi": 3,
                "inputs": stage_inputs(), "build": build_manifest,
                "files": {name: sha(directory / name) for name in ["libSkiaSharp.dll", "LICENSE.txt", "THIRD-PARTY-NOTICES.txt"]}}
    (directory / "build-provenance.json").write_text(json.dumps(manifest, indent=2))


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--vs", type=Path, default=Path("C:/Program Files/Microsoft Visual Studio/18/Community"))
    parser.add_argument("--llvm", type=Path, default=Path("C:/Program Files/LLVM"))
    parser.add_argument("--jobs", type=int, default=8)
    parser.add_argument("--ensure-distribution", action="store_true", help="Reuse a hash-verified staged asset, otherwise build it.")
    args = parser.parse_args()
    import xml.etree.ElementTree as ET
    version = ET.parse(DOROTI / "Directory.Packages.props").find(".//PackageVersion[@Include='SkiaSharp']").attrib["Version"]
    if version != PACKAGE_VERSION:
        raise RuntimeError("The managed SkiaSharp pin differs from the qualified native asset recipe.")
    distribution = DOROTI / "artifacts/native-graphite/distribution/win-x64"
    if args.ensure_distribution and stage_is_current(distribution):
        print(distribution / "build-provenance.json")
        return
    if sys.platform != "win32":
        parser.error("Build win-x64 on Windows first, or supply its hash-verified distribution when cross-packaging.")
    root = DOROTI / "artifacts/native-graphite/skia-build"
    root.mkdir(parents=True, exist_ok=True)
    run_dir = DOROTI / "artifacts/native-graphite/build-runs"
    from datetime import datetime, timezone
    run_dir = run_dir / datetime.now(timezone.utc).strftime("%Y%m%dT%H%M%SZ")
    run_dir.mkdir(parents=True)
    commands = []

    def run(command, *, env=None):
        ordinal = len(commands)
        record = {"command": [str(item) for item in command], "timeoutSeconds": 1200}
        commands.append(record)
        try:
            with (run_dir / f"{ordinal:02}.log").open("w") as log:
                proc = subprocess.Popen(command, cwd=root, env=env, stdout=log, stderr=subprocess.STDOUT)
                try:
                    record["exitCode"] = proc.wait(timeout=1200)
                except subprocess.TimeoutExpired:
                    subprocess.run(["taskkill", "/PID", str(proc.pid), "/T", "/F"], capture_output=True, timeout=30)
                    record["timedOut"] = True
                    raise
            if proc.returncode:
                raise RuntimeError(f"Command failed ({proc.returncode}): {run_dir / f'{ordinal:02}.log'}")
        finally:
            (run_dir / "commands.json").write_text(json.dumps(commands, indent=2))

    if not (root / ".git").exists():
        run(["git", "init"])
        run(["git", "remote", "add", "origin", "https://github.com/mono/skia.git"])
        run(["git", "fetch", "--depth=1", "origin", SKIA_REVISION])
        run(["git", "checkout", "--detach", "FETCH_HEAD"])
    actual = subprocess.check_output(["git", "rev-parse", "HEAD"], cwd=root, text=True, timeout=1200).strip()
    if actual != SKIA_REVISION:
        raise RuntimeError(f"Refusing different Skia revision: {actual}")
    run([sys.executable, "tools/git-sync-deps"])
    if not (root / "bin/gn.exe").exists():
        run([sys.executable, "bin/fetch-gn"])
    # Verify the one edited upstream file against Git before appending our bridge.
    source = root / "src/c/sk_graphite_vulkan.cpp"
    original = subprocess.check_output(["git", "show", "HEAD:src/c/sk_graphite_vulkan.cpp"], cwd=root, timeout=1200).decode()
    addition = '\n#include "src/c/doroti_graphite_interop.inc"\n'
    if source.read_text() not in (original, original + addition):
        raise RuntimeError("Upstream C shim has unrelated edits; refusing to replace it.")
    source.write_text(original + addition, newline="\n")
    shutil.copyfile(HERE / "doroti_graphite_interop.inc", root / "src/c/doroti_graphite_interop.inc")
    vcvars = args.vs / "VC/Auxiliary/Build/vcvarsall.bat"
    environment_text = subprocess.check_output(
        f'cmd.exe /d /s /c ""{vcvars}" amd64 >nul && set"', text=True, timeout=1200)
    environment = dict(os.environ)
    environment.update(line.split("=", 1) for line in environment_text.splitlines() if "=" in line and not line.startswith("="))
    out = root / "out/doroti-win-x64"
    out.mkdir(parents=True, exist_ok=True)
    # Pinned Windows recipe; Direct3D remains enabled for ABI/baseline parity.
    flags = '''target_os="win"
target_cpu="x64"
is_official_build=true
skia_enable_tools=false
skia_enable_fontmgr_win_gdi=false
skia_use_dng_sdk=true
skia_use_harfbuzz=false
skia_use_icu=false
skia_use_partition_alloc=false
skia_use_piex=true
skia_use_system_expat=false
skia_use_system_freetype2=false
skia_use_system_libjpeg_turbo=false
skia_use_system_libpng=false
skia_use_system_libwebp=false
skia_use_system_zlib=false
skia_enable_skottie=true
skia_use_vulkan=true
skia_use_direct3d=true
skia_use_freetype=false
skia_enable_fontmgr_custom_empty=false
skia_enable_fontmgr_win=true
skia_enable_graphite=true
extra_cflags=["-DSKIA_C_DLL", "-DSK_AVOID_SLOW_RASTER_PIPELINE_BLURS", "-DSK_ENABLE_LEGACY_SHADERCONTEXT", "/MT", "/EHsc", "/Z7", "/guard:cf", "-D_HAS_AUTO_PTR_ETC=1"]
extra_ldflags=["/DEBUG:FULL", "/DEBUGTYPE:CV,FIXUP", "/guard:cf", "/DELAYLOAD:d3d12.dll", "/DELAYLOAD:dxgi.dll", "/DELAYLOAD:D3DCOMPILER_47.dll", "/DEFAULTLIB:delayimp"]
'''
    flags += f'clang_win="{args.llvm.as_posix()}"\nwin_vc="{(args.vs / "VC").as_posix()}"\n'
    (out / "args.gn").write_text(flags)
    run([str(root / "bin/gn.exe"), "gen", str(out), f"--script-executable={sys.executable}"], env=environment)
    run(["ninja", "-C", str(out), "SkiaSharp", "-j", str(args.jobs)], env=environment)
    dll = out / "libSkiaSharp.dll"
    manifest = {"schema": "doroti.graphite-native-build/v1", "rid": "win-x64",
        "skiaRevision": SKIA_REVISION, "skiaSharpRevision": SKIASHARP_REVISION,
        "bridgeAbi": 3, "productQualified": False,
        "files": [{"path": str(p), "sha256": hashlib.sha256(p.read_bytes()).hexdigest()}
                  for p in [dll, root / "src/c/doroti_graphite_interop.inc", out / "args.gn", root / "DEPS", root / "bin/gn.exe"]]}
    (run_dir / "manifest.json").write_text(json.dumps(manifest, indent=2))
    stage_asset(distribution, dll, manifest)
    print(run_dir / "manifest.json")


if __name__ == "__main__":
    main()
