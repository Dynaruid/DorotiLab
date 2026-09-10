"""Compile and run the isolated ABI 3 native probe on an explicitly selected Android device.

No application installation or product renderer change. Evidence and diagnostic
files are retained; every command and the on-device process has a 20-minute limit.
"""
import argparse
from datetime import datetime, timezone
import hashlib
import json
import os
from pathlib import Path
import shutil
import signal
import subprocess
import sys

HERE = Path(__file__).resolve().parent
DOROTI = HERE.parents[1]


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--serial", required=True)
    parser.add_argument("--source", type=Path, default=DOROTI / "artifacts/native-graphite/skia-build")
    parser.add_argument("--native-library", type=Path)
    parser.add_argument("--ndk", type=Path, default=os.environ.get("ANDROID_NDK_HOME"))
    args = parser.parse_args()
    if args.ndk is None:
        parser.error("--ndk or ANDROID_NDK_HOME is required")
    stamp = datetime.now(timezone.utc).strftime("%Y%m%dT%H%M%S%fZ")
    evidence = DOROTI / "artifacts/native-graphite/android-runs" / stamp
    evidence.mkdir(parents=True)
    remote = f"/data/local/tmp/doroti-graphite-{stamp}"
    report = {"schema": "doroti.android-graphite-probe/v1", "status": "FAIL", "serial": args.serial,
              "deviceDirectory": remote, "productQualified": False, "physicalScanOut": "notVerified",
              "surfacePresent": "notVerified", "synchronizationValidation": "notVerified", "commands": []}

    def save():
        (evidence / "report.json").write_text(json.dumps(report, indent=2))

    def run(command, expected=0):
        command = [str(value) for value in command]
        log = evidence / f"{len(report['commands']):02}.log"
        result = {"command": command, "timeoutSeconds": 1200, "log": str(log)}
        report["commands"].append(result)
        with log.open("w") as output:
            child = subprocess.Popen(command, stdout=output, stderr=subprocess.STDOUT,
                                     start_new_session=sys.platform != "win32")
            try:
                result["exitCode"] = child.wait(timeout=1200)
            except subprocess.TimeoutExpired:
                if sys.platform == "win32":
                    subprocess.run(["taskkill", "/PID", str(child.pid), "/T", "/F"], capture_output=True, timeout=30)
                else:
                    os.killpg(child.pid, signal.SIGKILL)
                child.wait()
                result.update(exitCode=124, timedOut=True)
        save()
        if result["exitCode"] != expected:
            raise RuntimeError(f"Unexpected exit {result['exitCode']}: {log}")
        return log.read_text()

    try:
        abi = run(["adb", "-s", args.serial, "shell", "getprop", "ro.product.cpu.abi"]).strip()
        if abi != "arm64-v8a":
            raise RuntimeError(f"This probe recipe qualifies Android arm64 only, got {abi}")
        report["model"] = run(["adb", "-s", args.serial, "shell", "getprop", "ro.product.model"]).strip()
        report["api"] = int(run(["adb", "-s", args.serial, "shell", "getprop", "ro.build.version.sdk"]).strip())
        report["declaredFeatures"] = run(["adb", "-s", args.serial, "shell", "pm", "list", "features"])
        platform = "windows-x86_64" if sys.platform == "win32" else "darwin-x86_64" if sys.platform == "darwin" else "linux-x86_64"
        llvm = args.ndk.resolve() / "toolchains/llvm/prebuilt" / platform / "bin"
        suffix = ".exe" if sys.platform == "win32" else ""
        library = (args.native_library or args.source / "out/doroti-android-arm64/libSkiaSharp.so").resolve()
        executable = evidence / "graphite-android"
        run([llvm / f"clang++{suffix}", "--target=aarch64-linux-android24", "-std=c++17", "-O2",
             "-static-libstdc++", "-I", args.source.resolve(), HERE / "graphite-android.cpp",
             "-L", library.parent, "-lSkiaSharp", "-lvulkan", "-llog", "-landroid", "-ldl",
             "-Wl,-rpath,$ORIGIN", "-Wl,-z,max-page-size=16384", "-o", executable])
        deployed_library = evidence / "libSkiaSharp.so"
        shutil.copyfile(library, deployed_library)
        run([llvm / f"llvm-strip{suffix}", "--strip-unneeded", deployed_library])
        report["elf"] = run([llvm / f"llvm-readelf{suffix}", "-d", "-l", deployed_library])
        report["files"] = [{"path": str(path), "sha256": hashlib.sha256(path.read_bytes()).hexdigest()}
                           for path in [library, deployed_library, executable, HERE / "graphite-android.cpp", args.ndk / "source.properties"]]
        run(["adb", "-s", args.serial, "shell", "mkdir", remote])
        run(["adb", "-s", args.serial, "push", deployed_library, executable, remote + "/"])
        run(["adb", "-s", args.serial, "shell", "chmod", "700", remote + "/graphite-android"])
        output = run(["adb", "-s", args.serial, "shell",
                      f"LD_LIBRARY_PATH={remote} timeout -s KILL 1200 {remote}/graphite-android"], expected=2)
        if "contextGenerations=3 frames=36 normalTeardown=PASS" not in output:
            raise RuntimeError("Missing final native probe receipt")
        if f"nativeLibraryPath={remote}/libSkiaSharp.so" not in output:
            raise RuntimeError("The device loaded a different Skia module")
        report["probeOutput"] = output
        report["deviceHashes"] = run(["adb", "-s", args.serial, "shell", "sha256sum",
                                      remote + "/libSkiaSharp.so", remote + "/graphite-android"])
        for path in [deployed_library, executable]:
            if hashlib.sha256(path.read_bytes()).hexdigest() not in report["deviceHashes"]:
                raise RuntimeError("On-device asset hash differs from the deployed artifact")
        report["status"] = "PARTIAL"
    except Exception as exception:
        report["error"] = str(exception)
        raise
    finally:
        save()
        print(evidence / "report.json", flush=True)
    return 2


if __name__ == "__main__":
    raise SystemExit(main())
