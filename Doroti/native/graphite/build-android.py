"""Cross-build the pinned ABI 3 diagnostic Android asset from a prepared Skia checkout.

Prepare sources with build-windows.py or build-linux.py first. This does not alter
MAUI dependencies, minimum SDK, app outputs, NuGet cache, or renderer defaults.
"""
import argparse
from datetime import datetime, timezone
import hashlib
import json
import os
from pathlib import Path
import signal
import subprocess
import sys

HERE = Path(__file__).resolve().parent
DOROTI = HERE.parents[1]
SKIA_REVISION = "cc43af052d3d98e605bee4ddc98671dafded1c57"


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--source", type=Path, default=DOROTI / "artifacts/native-graphite/skia-build")
    parser.add_argument("--ndk", type=Path, default=os.environ.get("ANDROID_NDK_HOME"))
    parser.add_argument("--cpu", choices=["arm64", "x64"], default="arm64")
    parser.add_argument("--jobs", type=int, default=8)
    args = parser.parse_args()
    if args.ndk is None or not args.ndk.is_dir():
        parser.error("Select an installed Android NDK using --ndk or ANDROID_NDK_HOME.")
    root = args.source.resolve()
    evidence = DOROTI / "artifacts/native-graphite/android-build-runs" / datetime.now(timezone.utc).strftime("%Y%m%dT%H%M%S%fZ")
    evidence.mkdir(parents=True)
    manifest = {"schema": "doroti.graphite-native-build/v1", "rid": f"android-{args.cpu}", "bridgeAbi": 3,
                "skiaRevision": SKIA_REVISION, "skiaSharpRevision": "143a933a753dbfeca1909524b2c06c546c5c3e20",
                "minimumDiagnosticApi": 24, "productQualified": False, "status": "FAIL", "commands": []}
    environment = dict(os.environ)
    if sys.platform == "win32":
        # Pinned GN archive rules invoke python3 by name, independently of
        # --script-executable. Keep the Windows Store alias out of this build.
        tool_bin = evidence / "tool-bin"
        tool_bin.mkdir()
        (tool_bin / "python3.cmd").write_text(f'@echo off\n"{sys.executable}" %*\n')
        environment["PATH"] = str(tool_bin) + os.pathsep + environment.get("PATH", "")
    manifest["python"] = sys.executable

    def save():
        (evidence / "manifest.json").write_text(json.dumps(manifest, indent=2))

    def run(command):
        command = [str(part) for part in command]
        log = evidence / f"{len(manifest['commands']):02}.log"
        record = {"command": command, "timeoutSeconds": 1200, "log": str(log)}
        manifest["commands"].append(record)
        with log.open("w") as output:
            child = subprocess.Popen(command, cwd=root, stdout=output, stderr=subprocess.STDOUT,
                                     start_new_session=sys.platform != "win32", env=environment)
            try:
                record["exitCode"] = child.wait(timeout=1200)
            except subprocess.TimeoutExpired:
                if sys.platform == "win32":
                    subprocess.run(["taskkill", "/PID", str(child.pid), "/T", "/F"], capture_output=True, timeout=30)
                else:
                    os.killpg(child.pid, signal.SIGKILL)
                child.wait()
                record.update(exitCode=124, timedOut=True)
        save()
        if record["exitCode"]:
            raise RuntimeError(f"Command failed: {log}")
        return log.read_text()

    try:
        if run(["git", "rev-parse", "HEAD"]).strip() != SKIA_REVISION:
            raise RuntimeError("Refusing a different Skia revision.")
        original = run(["git", "show", "HEAD:src/c/sk_graphite_vulkan.cpp"])
        addition = '\n#include "src/c/doroti_graphite_interop.inc"\n'
        source = root / "src/c/sk_graphite_vulkan.cpp"
        if source.read_text() != original + addition:
            raise RuntimeError("Prepare the pinned bridge checkout first using build-windows.py or build-linux.py.")
        if (root / "src/c/doroti_graphite_interop.inc").read_bytes() != (HERE / "doroti_graphite_interop.inc").read_bytes():
            raise RuntimeError("Prepared bridge differs from the current source; rebuild it first.")
        out = root / f"out/doroti-android-{args.cpu}"
        out.mkdir(parents=True, exist_ok=True)
        flags = f'''target_os="android"
target_cpu="{args.cpu}"
is_official_build=true
skia_enable_tools=false
skia_enable_graphite=true
skia_use_vulkan=true
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
ndk="{args.ndk.resolve().as_posix()}"
ndk_api=24
extra_cflags=["-DSKIA_C_DLL", "-DSK_AVOID_SLOW_RASTER_PIPELINE_BLURS", "-DSK_ENABLE_LEGACY_SHADERCONTEXT", "-DHAVE_SYSCALL_GETRANDOM", "-DXML_DEV_URANDOM"]
extra_ldflags=["-Wl,--build-id=sha1", "-Wl,-z,max-page-size=16384"]
'''
        (out / "args.gn").write_text(flags)
        run([root / ("bin/gn.exe" if sys.platform == "win32" else "bin/gn"), "gen", out, f"--script-executable={sys.executable}"])
        run(["ninja", "-C", out, "SkiaSharp", "-j", args.jobs])
        platform = "windows-x86_64" if sys.platform == "win32" else "darwin-x86_64" if sys.platform == "darwin" else "linux-x86_64"
        llvm = args.ndk / "toolchains/llvm/prebuilt" / platform / "bin"
        suffix = ".exe" if sys.platform == "win32" else ""
        library = out / "libSkiaSharp.so"
        exports = run([llvm / f"llvm-nm{suffix}", "-D", "--defined-only", library])
        for name in ["doroti_graphite_interop_version", "doroti_graphite_vk_context_create", "doroti_graphite_vk_texture_get_state",
                     "doroti_graphite_vk_texture_set_state", "doroti_graphite_vk_insert_recording", "doroti_graphite_has_unfinished_gpu_work"]:
            if name not in exports:
                raise RuntimeError(f"Missing ABI 3 symbol: {name}")
        run([llvm / f"llvm-readelf{suffix}", "-h", "-l", "-d", library])
        manifest["files"] = [{"path": str(path), "sha256": hashlib.sha256(path.read_bytes()).hexdigest()}
                             for path in [library, out / "args.gn", source, root / "src/c/doroti_graphite_interop.inc",
                                          root / "DEPS", root / "LICENSE", args.ndk / "source.properties"]]
        manifest["status"] = "PASS"
    except Exception as exception:
        manifest["error"] = str(exception)
        raise
    finally:
        save()
        print(evidence / "manifest.json", flush=True)


if __name__ == "__main__":
    main()
