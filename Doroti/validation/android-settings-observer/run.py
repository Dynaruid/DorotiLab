"""Run the Android JNI observer regression on an explicitly selected device.

Wrap this script with validation/run-with-timeout.py for the 20-minute suite limit.
"""
import argparse
import json
from pathlib import Path
import subprocess
import time

parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument("--serial", required=True)
parser.add_argument("--apk", type=Path, required=True)
parser.add_argument("--output", type=Path, required=True)
args = parser.parse_args()
args.output.mkdir(parents=True, exist_ok=True)
package = "dev.doroti.validation.settingsobserver"


def adb(*command, check=True):
    return subprocess.run(["adb", "-s", args.serial, *command], capture_output=True,
                          text=True, encoding="utf-8", errors="replace", timeout=1200, check=check).stdout


adb("install", "-r", "--user", "0", str(args.apk.resolve()))
adb("shell", "am", "force-stop", package)
since = adb("shell", "date", "+%s.%N").strip()
launch = adb("shell", "am", "start", "-W", "-n", f"{package}/{package}.MainActivity")
(args.output / "launch.txt").write_text(launch, encoding="utf-8")
deadline = time.monotonic() + 30
passed = False
log = ""
while time.monotonic() < deadline:
    log = adb("logcat", "-d", "-T", since, "-s", "DorotiObserverTest:I", "AndroidRuntime:E", "*:S")
    if "DorotiObserverTest: PASS:" in log:
        passed = True
        break
    if "DorotiObserverTest: FAIL:" in log or f"Process: {package}, PID:" in log:
        break
    time.sleep(0.5)
(args.output / "logcat.txt").write_text(log, encoding="utf-8")
result = {"status": "PASS" if passed else "FAIL", "serial": args.serial,
          "cases": ["active notification", "queued notification disposal", "late JNI callback", "reattach"]}
(args.output / "result.json").write_text(json.dumps(result, indent=2), encoding="utf-8")
print(json.dumps(result))
raise SystemExit(0 if passed else 1)
