"""Build the NG1 interop probe asset from pinned Skia, never install into NuGet/product.

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


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--vs", type=Path, default=Path("C:/Program Files/Microsoft Visual Studio/18/Community"))
    parser.add_argument("--llvm", type=Path, default=Path("C:/Program Files/LLVM"))
    parser.add_argument("--jobs", type=int, default=8)
    args = parser.parse_args()
    if sys.platform != "win32":
        parser.error("This bootstrap qualifies win-x64 only.")
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
        "bridgeAbi": 2, "productQualified": False,
        "files": [{"path": str(p), "sha256": hashlib.sha256(p.read_bytes()).hexdigest()}
                  for p in [dll, root / "src/c/doroti_graphite_interop.inc", out / "args.gn", root / "DEPS", root / "bin/gn.exe"]]}
    (run_dir / "manifest.json").write_text(json.dumps(manifest, indent=2))
    print(run_dir / "manifest.json")


if __name__ == "__main__":
    main()
