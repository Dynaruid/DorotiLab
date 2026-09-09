#!/usr/bin/env python3
"""Run the shared Graphite probe on Linux; exit 2 is PARTIAL, never product PASS."""
import argparse
from datetime import datetime, timezone
import hashlib
import json
import os
from pathlib import Path
import platform
import signal
import shutil
import subprocess
import sys

ROOT = Path(__file__).resolve().parents[3]
PROJECT = Path(__file__).resolve().parent


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--native-library", type=Path)
    parser.add_argument("--device")
    parser.add_argument("--allow-software", action="store_true")
    parser.add_argument("--extended-context", action="store_true")
    parser.add_argument("--context-cycles", type=int, choices=range(1, 11), default=1)
    parser.add_argument("--self-test", choices=["graphite-after-submit", "graphite-device-lost"])
    parser.add_argument("--no-build", action="store_true")
    args = parser.parse_args()
    if sys.platform != "linux" or platform.machine() != "x86_64":
        parser.error("Use run-graphite.ps1 for Windows; this runner requires Linux x64.")
    if args.native_library and not args.native_library.is_file():
        parser.error("--native-library must name an existing libSkiaSharp.so.")
    evidence = ROOT / "Doroti/artifacts/native-graphite/linux-runs" / datetime.now(timezone.utc).strftime("%Y%m%dT%H%M%S%fZ")
    evidence.mkdir(parents=True)
    print(evidence, flush=True)
    manifest = {"schema": "doroti.graphite-linux-run/v1", "status": "FAIL", "productQualified": False,
                "timeoutSeconds": 1200, "commands": [], "physicalScanOut": "notVerified", "performance": "notVerified",
                "environment": {key: os.environ.get(key) for key in (
                    "VK_DRIVER_FILES", "VK_ICD_FILENAMES", "VK_LAYER_PATH", "VK_ADD_LAYER_PATH",
                    "DISPLAY", "WAYLAND_DISPLAY", "XDG_SESSION_TYPE")}}

    def save():
        (evidence / "manifest.json").write_text(json.dumps(manifest, indent=2) + "\n")

    def run(command, name):
        log = evidence / f"{name}.log"
        record = {"command": command, "log": str(log), "timeoutSeconds": 1200}
        manifest["commands"].append(record)
        save()
        with log.open("w") as output:
            child = subprocess.Popen(command, cwd=ROOT, stdout=output, stderr=subprocess.STDOUT, start_new_session=True)
            try:
                code = child.wait(timeout=1200)
            except subprocess.TimeoutExpired:
                os.killpg(child.pid, signal.SIGKILL)
                child.wait()
                code = 124
                record["timedOut"] = True
        record.update(exitCode=code, sha256=hashlib.sha256(log.read_bytes()).hexdigest())
        save()
        return code

    save()
    # Keep GL and Vulkan evidence side by side. VMware SVGA3D/LLVM is
    # hardware-backed GL even when this machine exposes only a CPU Vulkan ICD.
    for command, name in ((["git", "rev-parse", "HEAD"], "commit"),
                          (["git", "status", "--short"], "status"),
                          (["git", "diff", "--binary"], "working-diff"),
                          (["glxinfo", "-B"], "opengl"),
                          (["vulkaninfo", "--summary"], "vulkan")):
        if shutil.which(command[0]):
            run(command, name)
    manifest["targets"] = [json.loads(path.read_text()) for path in
                           sorted((ROOT / "Doroti/src").glob("Doroti.Target.*/doroti-target-manifest.json"))]
    manifest["probeSources"] = [{"path": str(path.relative_to(ROOT)),
                                 "sha256": hashlib.sha256(path.read_bytes()).hexdigest()}
                                for path in sorted(PROJECT.iterdir()) if path.suffix in (".cs", ".csproj", ".py")]
    save()
    if not args.no_build:
        code = run(["dotnet", "build", str(PROJECT / "Doroti.Validation.WindowsVulkanCapability.csproj")], "build")
        if code:
            print((evidence / "build.log").read_text()[-6000:])
            return 1
    binary = PROJECT / "bin/Debug/net10.0/linux-x64/Doroti.Validation.WindowsVulkanCapability.dll"
    manifest["probe"] = {"path": str(binary), "sha256": hashlib.sha256(binary.read_bytes()).hexdigest()}
    report = evidence / "probe.json"
    command = ["dotnet", str(binary), "--graphite", "--output", str(report)]
    if args.native_library:
        command += ["--graphite-native", str(args.native_library.resolve())]
    if args.device:
        command += ["--device", args.device]
    if args.allow_software:
        command += ["--allow-software"]
    if args.extended_context:
        command += ["--graphite-extended-context"]
    command += ["--graphite-context-cycles", str(args.context_cycles)]
    if args.self_test:
        command += ["--self-test", args.self_test]
    code = run(command, "probe")
    result = json.loads(report.read_text()) if report.exists() else {}
    manifest["status"] = "PARTIAL" if code == 2 and result.get("status") == "PARTIAL" else "FAIL"
    manifest["report"] = str(report)
    if code != 2:
        manifest["firstFailure"] = result.get("error", f"Probe exited {code}; inspect its raw log and lastOperation.")
    save()
    print(f"{manifest['status']} (exit {code}): {report}", flush=True)
    return 2 if manifest["status"] == "PARTIAL" else 1


if __name__ == "__main__":
    raise SystemExit(main())
