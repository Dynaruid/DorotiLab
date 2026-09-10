"""Check repeated Android process starts for crashes and input-dispatch ANRs.

Run against an explicitly selected device and an already installed app. Retains
logcat, UI hierarchy and screenshots; does not clear app data or system logs.
Every external command has the repository's 20-minute timeout.
"""
import argparse
import json
from pathlib import Path
import subprocess
import time


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--serial", required=True)
    parser.add_argument("--package", required=True)
    parser.add_argument("--activity", required=True)
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--rounds", type=int, default=3)
    parser.add_argument("--observe-seconds", type=int, default=20)
    args = parser.parse_args()
    if args.rounds < 1 or args.observe_seconds < 10:
        parser.error("Use at least one round and ten seconds of observation.")
    args.output.mkdir(parents=True, exist_ok=True)
    adb = ["adb", "-s", args.serial]

    def run(*command, check=True):
        return subprocess.run(adb + list(command), capture_output=True, timeout=1200, check=check)

    results = []
    for index in range(1, args.rounds + 1):
        prefix = args.output / f"launch-{index}"
        run("shell", "am", "force-stop", args.package)
        # logcat accepts epoch timestamps; using the device clock avoids host
        # timezone/skew and preserves pre-existing diagnostics.
        since = run("shell", "date", "+%s.%N").stdout.decode().strip()
        launched = time.monotonic()
        launch = run("shell", "am", "start", "-W", "-n", f"{args.package}/{args.activity}")
        prefix.with_suffix(".launch.txt").write_bytes(launch.stdout + launch.stderr)
        seen_pid = None
        failures = []
        while time.monotonic() - launched < args.observe_seconds:
            current = run("shell", "pidof", args.package, check=False).stdout.decode().strip()
            if current:
                if seen_pid is not None and seen_pid != current:
                    failures.append("process restarted during startup")
                seen_pid = current
            elif seen_pid is not None:
                failures.append("process exited during startup")
                break
            time.sleep(1)
        if not seen_pid:
            failures.append("app process did not start")
        log = run("logcat", "-d", "-T", since).stdout.decode(errors="replace")
        prefix.with_suffix(".logcat.txt").write_text(log, encoding="utf-8")
        if f"ANR in {args.package}" in log or any(
            args.package in line and "is not responding" in line for line in log.splitlines()
        ):
            failures.append("input dispatch ANR")
        if f"Process: {args.package}, PID:" in log and "FATAL EXCEPTION" in log:
            failures.append("fatal Android exception")
        if f">>> {args.package} <<<" in log and "Fatal signal" in log:
            failures.append("fatal native signal")
        remote_hierarchy = f"/sdcard/doroti-startup-{time.time_ns()}.xml"
        run("shell", "uiautomator", "dump", remote_hierarchy)
        hierarchy = run("shell", "cat", remote_hierarchy).stdout
        run("shell", "rm", remote_hierarchy)
        prefix.with_suffix(".xml").write_bytes(hierarchy)
        if f'package="{args.package}"'.encode() not in hierarchy:
            failures.append("app UI is not in the foreground")
        if b"android:id/aerr_wait" in hierarchy or b"android:id/aerr_close" in hierarchy:
            failures.append("Android error dialog is visible")
        prefix.with_suffix(".png").write_bytes(run("exec-out", "screencap", "-p").stdout)
        result = {"round": index, "pid": seen_pid, "failures": failures, "status": "FAIL" if failures else "PASS"}
        results.append(result)
        (args.output / "result.json").write_text(json.dumps(results, indent=2), encoding="utf-8")
        print(json.dumps(result), flush=True)
    if any(result["failures"] for result in results):
        raise SystemExit(1)


if __name__ == "__main__":
    main()
