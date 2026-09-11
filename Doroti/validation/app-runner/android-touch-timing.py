"""Repeat Android Material sample touch transitions and retain timing evidence.

Use run-with-timeout.py for the repository's 20-minute limit. The installed APK
must include DOROTI_INPUT_TIMING support and start on the diagnostics gallery.
Coordinates are physical pixels; inspect the device screen before choosing them.
Slow-frame counts describe CPU render work, not physical display presentation.
"""
import argparse
import json
from pathlib import Path
import re
import subprocess
import time


def summarize(log):
    lines = log.splitlines()
    downs = [i for i, line in enumerate(lines) if "DorotiInputTiming: action=Down" in line]
    # Exclude gallery navigation and the first swipe (cold gesture/JIT work).
    warm = lines[downs[2]:] if len(downs) > 2 else []
    inputs = [float(m[1]) for line in warm if (m := re.search(r"dispatchMs=([\d.]+)", line))]
    frames = [float(m[1]) for line in warm if (m := re.search(r"TotalMs = ([\d.]+)", line))]
    return {
        "warmDowns": sum("DorotiInputTiming: action=Down" in line for line in warm),
        "warmUps": sum("DorotiInputTiming: action=Up" in line for line in warm),
        "loggedInputMaxMs": max(inputs, default=None),
        "loggedFrameMaxMs": max(frames, default=None),
        "inputDispatchesOver32Ms": sum(value > 32 for value in inputs),
        "renderFramesOver32Ms": sum(value > 32 for value in frames),
        "explicitJavaCollections": sum("Explicit concurrent" in line and " GC " in line for line in warm),
        "physicalPresentation": "notVerified",
    }


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--serial", required=True)
    parser.add_argument("--package", default="dev.doroti.testbed")
    parser.add_argument("--activity", required=True)
    parser.add_argument("--output", required=True, type=Path)
    parser.add_argument("--open-sample", nargs=2, type=int, required=True, metavar=("X", "Y"))
    parser.add_argument("--swipe", nargs=4, type=int, required=True, metavar=("X1", "Y1", "X2", "Y2"))
    parser.add_argument("--rounds", type=int, default=8)
    parser.add_argument("--extra", action="append", nargs=2, default=[], metavar=("KEY", "VALUE"))
    args = parser.parse_args()
    if args.rounds < 3:
        parser.error("Use at least three swipes, including one cold swipe.")
    args.output.mkdir(parents=True, exist_ok=True)
    adb = ["adb", "-s", args.serial]

    def run(*command):
        return subprocess.check_output(adb + list(map(str, command)), timeout=1200)

    since = run("shell", "date", "+%s.%N").decode().strip()
    run("shell", "am", "start", "-S", "-W", "-n", f"{args.package}/{args.activity}",
        "--es", "DOROTI_INPUT_TIMING", "1", *[part for key, value in args.extra for part in ("--es", key, value)])
    time.sleep(4)
    run("shell", "input", "tap", *args.open_sample)
    time.sleep(4)
    for index in range(args.rounds):
        x1, y1, x2, y2 = args.swipe
        coords = (x1, y1, x2, y2) if index % 2 == 0 else (x2, y2, x1, y1)
        run("shell", "input", "swipe", *coords, 500)
        time.sleep(.4)
    time.sleep(3)
    pid = run("shell", "pidof", args.package).decode().strip()
    log = run("logcat", "-d", "-T", since, "--pid=" + pid).decode(errors="replace")
    args.output.joinpath("logcat.txt").write_text(log, encoding="utf-8")
    args.output.joinpath("screen.png").write_bytes(run("exec-out", "screencap", "-p"))
    args.output.joinpath("meminfo.txt").write_bytes(run("shell", "dumpsys", "meminfo", args.package))
    activity = run("shell", "dumpsys", "activity", "activities").decode(errors="replace")
    args.output.joinpath("activity.txt").write_text(activity, encoding="utf-8")
    summary = summarize(log)
    summary.update(pid=pid, serial=args.serial, rounds=args.rounds,
                   foreground=any(args.package in line and "ResumedActivity" in line for line in activity.splitlines()),
                   runtimeErrors=any(marker in log for marker in ("FATAL EXCEPTION", "Fatal signal", "DorotiMauiFailure", "ANR in " + args.package)))
    args.output.joinpath("summary.json").write_text(json.dumps(summary, indent=2), encoding="utf-8")
    print(json.dumps(summary, indent=2))
    if summary["warmDowns"] != args.rounds - 1 or summary["warmUps"] != args.rounds - 1 or not summary["foreground"] or summary["runtimeErrors"]:
        raise SystemExit("Touch sequence or runtime validation failed; inspect the evidence.")


if __name__ == "__main__":
    main()
