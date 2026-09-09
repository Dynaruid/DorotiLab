#!/usr/bin/env python3
"""Run the existing Apple probe with bounded, captured child processes."""
import argparse
import datetime
import hashlib
import json
import os
from pathlib import Path
import signal
import subprocess
import sys

ROOT = Path(__file__).resolve().parents[3]
PROJECT = Path(__file__).resolve().parent
TIMEOUT = 1200


def run(command, log, env):
    with log.open("w") as output:
        child = subprocess.Popen(command, cwd=ROOT, env=env, stdout=output,
                                 stderr=subprocess.STDOUT, start_new_session=True)
        try:
            return child.wait(timeout=TIMEOUT)
        except subprocess.TimeoutExpired:
            os.killpg(child.pid, signal.SIGKILL)
            child.wait()
            output.write("\nExternal 1200-second timeout; process group killed.\n")
            return 124


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--mode", choices=["all", "contract", "ganesh", "graphite"], default="all")
    parser.add_argument("--no-build", action="store_true")
    args = parser.parse_args()
    if sys.platform != "darwin":
        parser.error("This probe requires macOS with Xcode and the .NET macos workload.")
    stamp = datetime.datetime.now(datetime.timezone.utc).strftime("%Y%m%dT%H%M%S%fZ")
    evidence = ROOT / "Doroti/artifacts/native-graphite/apple-runs" / stamp
    evidence.mkdir(parents=True)
    print(evidence, flush=True)
    manifest = {"schema": "doroti.graphite-apple-run/v1", "status": "FAIL", "timeoutSeconds": TIMEOUT,
                "runs": [], "physicalScanOut": "notVerified", "performance": "notVerified"}
    manifest_path = evidence / "manifest.json"

    def save():
        manifest_path.write_text(json.dumps(manifest, indent=2) + "\n")

    save()
    env = os.environ.copy()
    # Inherited mode variables must not silently turn the baseline into a candidate.
    for key in ("DOROTI_APPKIT_SPIKE_CONTRACT", "DOROTI_APPKIT_SPIKE_GRAPHITE", "DOROTI_APPKIT_SPIKE_AUTOMATE"):
        env.pop(key, None)
    if not args.no_build:
        command = ["dotnet", "build", str(PROJECT / "Doroti.Validation.AppKitMetalSpike.csproj")]
        code = run(command, evidence / "build.log", env)
        manifest["build"] = {"command": command, "exitCode": code}
        save()
        if code:
            print((evidence / "build.log").read_text()[-6000:])
            return 1
    binary = PROJECT / "bin/Debug/net10.0-macos/osx-arm64/Doroti AppKit Metal Spike.app/Contents/MacOS/Doroti.Validation.AppKitMetalSpike"
    manifest["app"] = {"path": str(binary), "sha256": hashlib.sha256(binary.read_bytes()).hexdigest()}
    modes = ["contract", "ganesh", "graphite"] if args.mode == "all" else [args.mode]
    passed = True
    for mode in modes:
        output = evidence / (mode + ".json")
        run_env = env | {"MTL_DEBUG_LAYER": "1", "DOROTI_APPKIT_SPIKE_EVIDENCE": str(output)}
        if mode == "contract":
            run_env["DOROTI_APPKIT_SPIKE_CONTRACT"] = "1"
        else:
            run_env["DOROTI_APPKIT_SPIKE_AUTOMATE"] = "1"
            if mode == "graphite":
                run_env["DOROTI_APPKIT_SPIKE_GRAPHITE"] = "1"
        log = evidence / (mode + ".log")
        code = run([str(binary)], log, run_env)
        report = json.loads(output.read_text()) if output.exists() else {}
        ok = code == 0 and report.get("status") == "PASS"
        if mode != "contract":
            native = report.get("native", {})
            ok &= (native.get("resourcesReleased") is True and native.get("outstandingFrames") == 0
                   and native.get("commandBuffersErrored") == 0 and native.get("shutdownStartedWithInFlight", 0) > 0)
        manifest["runs"].append({"mode": mode, "exitCode": code, "status": "PASS" if ok else "FAIL",
                                 "report": str(output), "log": str(log), "metalDebugLayerRequested": True,
                                 "logSha256": hashlib.sha256(log.read_bytes()).hexdigest()})
        passed &= ok
        save()
        print(f"{mode}: {'PASS' if ok else 'FAIL'} (exit {code})", flush=True)
    # Only the selected probe contracts are qualified, never the five-OS product migration.
    manifest["status"] = "PASS" if passed else "FAIL"
    save()
    return 0 if passed else 1


if __name__ == "__main__":
    raise SystemExit(main())
