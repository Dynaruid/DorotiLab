#!/usr/bin/env python3
"""Build an isolated linux-x64 Graphite bridge from the pinned Skia revision.

Requires clang/clang++, ninja, git, Python and fontconfig development headers.
Does not install into the NuGet cache or qualify a redistributable Linux binary.
"""
import argparse
from datetime import datetime, timezone
import hashlib
import json
import os
from pathlib import Path
import platform
import shutil
import signal
import subprocess
import sys

SKIA_REVISION = "cc43af052d3d98e605bee4ddc98671dafded1c57"
SKIASHARP_REVISION = "143a933a753dbfeca1909524b2c06c546c5c3e20"
HERE = Path(__file__).resolve().parent
DOROTI = HERE.parents[1]


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--jobs", type=int, default=4)
    parser.add_argument("--cc", default="clang")
    parser.add_argument("--cxx", default="clang++")
    args = parser.parse_args()
    if sys.platform != "linux" or platform.machine() != "x86_64":
        parser.error("This bootstrap builds linux-x64 only.")
    if args.jobs < 1:
        parser.error("--jobs must be positive.")
    for executable in (args.cc, args.cxx, "ninja", "git"):
        if not shutil.which(executable):
            parser.error(f"Required executable missing: {executable}")
    root = DOROTI / "artifacts/native-graphite/skia-linux-build"
    root.mkdir(parents=True, exist_ok=True)
    evidence = DOROTI / "artifacts/native-graphite/build-runs" / datetime.now(timezone.utc).strftime("%Y%m%dT%H%M%S%fZ-linux")
    evidence.mkdir(parents=True)
    print(evidence, flush=True)
    manifest = {"schema": "doroti.graphite-native-build/v1", "rid": "linux-x64", "status": "FAIL",
                "skiaRevision": SKIA_REVISION, "skiaSharpRevision": SKIASHARP_REVISION,
                "bridgeAbi": 2, "productQualified": False, "commands": []}

    def save():
        (evidence / "manifest.json").write_text(json.dumps(manifest, indent=2) + "\n")

    def run(command):
        command = [str(item) for item in command]
        log = evidence / f"{len(manifest['commands']):02}.log"
        record = {"command": command, "timeoutSeconds": 1200, "log": str(log)}
        manifest["commands"].append(record)
        save()
        with log.open("w") as output:
            child = subprocess.Popen(command, cwd=root, stdout=output, stderr=subprocess.STDOUT, start_new_session=True)
            try:
                record["exitCode"] = child.wait(timeout=1200)
            except subprocess.TimeoutExpired:
                os.killpg(child.pid, signal.SIGKILL)
                child.wait()
                record["exitCode"] = 124
                record["timedOut"] = True
        save()
        if record["exitCode"]:
            raise RuntimeError(f"Command failed ({record['exitCode']}): {log}")
        return log.read_text()

    try:
        if not (root / ".git").exists():
            run(["git", "init"])
            run(["git", "remote", "add", "origin", "https://github.com/mono/skia.git"])
        # An interrupted initial fetch is resumable, while a different checkout is refused.
        head = subprocess.run(["git", "rev-parse", "--verify", "HEAD"], cwd=root, capture_output=True, text=True, timeout=1200)
        if head.returncode:
            run(["git", "fetch", "--depth=1", "origin", SKIA_REVISION])
            run(["git", "checkout", "--detach", "FETCH_HEAD"])
        actual = run(["git", "rev-parse", "HEAD"]).strip()
        if actual != SKIA_REVISION:
            raise RuntimeError(f"Refusing different Skia revision: {actual}")
        run([sys.executable, "tools/git-sync-deps"])
        if not (root / "bin/gn").exists():
            run([sys.executable, "bin/fetch-gn"])
        source = root / "src/c/sk_graphite_vulkan.cpp"
        original = run(["git", "show", "HEAD:src/c/sk_graphite_vulkan.cpp"])
        addition = '\n#include "src/c/doroti_graphite_interop.inc"\n'
        if source.read_text() not in (original, original + addition):
            raise RuntimeError("Upstream C shim has unrelated edits; refusing to replace it.")
        source.write_text(original + addition)
        shutil.copyfile(HERE / "doroti_graphite_interop.inc", root / "src/c/doroti_graphite_interop.inc")
        out = root / "out/doroti-linux-x64"
        out.mkdir(parents=True, exist_ok=True)
        # Local diagnostic recipe using the host C++ runtime. A portable RID package
        # still needs the supported glibc/sysroot, dependency and license gates.
        flags = '''target_os="linux"
target_cpu="x64"
is_official_build=true
skia_enable_tools=false
skia_enable_ganesh=true
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
extra_cflags=["-DSKIA_C_DLL", "-DHAVE_SYSCALL_GETRANDOM", "-DXML_DEV_URANDOM", "-DSK_AVOID_SLOW_RASTER_PIPELINE_BLURS", "-DSK_ENABLE_LEGACY_SHADERCONTEXT"]
'''
        flags += f"cc={json.dumps(str(Path(shutil.which(args.cc)).resolve()))}\ncxx={json.dumps(str(Path(shutil.which(args.cxx)).absolute()))}\n"
        (out / "args.gn").write_text(flags)
        run([args.cc, "--version"])
        run(["ninja", "--version"])
        run([root / "bin/gn", "gen", out, f"--script-executable={sys.executable}"])
        run(["ninja", "-C", out, "SkiaSharp", "-j", args.jobs])
        library = out / "libSkiaSharp.so"
        run(["readelf", "-d", library])
        run(["readelf", "--version-info", library])
        exports = run(["nm", "-D", "--defined-only", library])
        for name in ("doroti_graphite_interop_version", "doroti_graphite_vk_texture_get_state",
                     "doroti_graphite_vk_texture_set_state", "doroti_graphite_vk_insert_recording",
                     "doroti_graphite_has_unfinished_gpu_work", "doroti_graphite_vk_context_create", "sk_graphite_context_make_vulkan"):
            if name not in exports:
                raise RuntimeError(f"Missing native export: {name}")
        manifest["files"] = [{"path": str(p), "sha256": hashlib.sha256(p.read_bytes()).hexdigest()}
                             for p in (library, root / "src/c/doroti_graphite_interop.inc", out / "args.gn", root / "DEPS", root / "LICENSE", root / "bin/gn")]
        manifest["status"] = "PASS"
        save()
        print(evidence / "manifest.json", flush=True)
        return 0
    except Exception as exception:
        manifest["error"] = str(exception)
        save()
        raise


if __name__ == "__main__":
    raise SystemExit(main())
