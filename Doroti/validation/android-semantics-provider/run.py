"""Build and run source-linked Android accessibility provider contracts.

Run through ../run-with-timeout.py to enforce the 20-minute suite limit.
"""
import argparse
from pathlib import Path
import subprocess
import time

parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument("--serial", required=True)
parser.add_argument("--output", required=True, type=Path)
parser.add_argument("--no-build", action="store_true")
args = parser.parse_args()
here = Path(__file__).resolve().parent
args.output.mkdir(parents=True, exist_ok=True)
package = "dev.doroti.validation.semanticsprovider"
adb = ["adb", "-s", args.serial]

def run(*command, check=True):
    return subprocess.run(command, capture_output=True, timeout=1200, check=check)

if not args.no_build:
    result = run("dotnet", "build", str(here / "Doroti.Validation.AndroidSemanticsProvider.csproj"), "-c", "Release", check=False)
    args.output.joinpath("build.log").write_bytes(result.stdout + result.stderr)
    result.check_returncode()
apk = here.parents[1] / "artifacts/validation/build/android-semantics-provider/bin/Release/net10.0-android/android-arm64" / (package + "-Signed.apk")
run(*adb, "install", "-r", "--user", "0", str(apk))
remote = f"/sdcard/Android/data/{package}/cache/result.txt"
run(*adb, "shell", "rm", "-f", remote)
activity = run(*adb, "shell", "cmd", "package", "resolve-activity", "--brief", package).stdout.decode().strip().splitlines()[-1]
since = run(*adb, "shell", "date", "+%s.%N").stdout.decode().strip()
run(*adb, "shell", "am", "start", "-S", "-W", "-n", activity)
while True:
    result = run(*adb, "shell", "cat", remote, check=False)
    if result.returncode == 0:
        break
    if run(*adb, "shell", "pidof", package, check=False).returncode:
        raise SystemExit("Contract app exited before producing evidence")
    time.sleep(.5)
args.output.joinpath("result.txt").write_bytes(result.stdout)
args.output.joinpath("logcat.txt").write_bytes(run(*adb, "logcat", "-d", "-T", since).stdout)
print(result.stdout.decode())
if not result.stdout.startswith(b"PASS"):
    raise SystemExit(1)
