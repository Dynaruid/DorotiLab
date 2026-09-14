"""Exercise the real Release sample tab with synthetic Qt input and a 1200s timeout."""
import datetime
import os
from pathlib import Path
import shlex
import subprocess
import sys

ROOT = Path(__file__).resolve().parents[3]
HERE = Path(__file__).resolve().parent
WRAPPER = ROOT / "Doroti/validation/run-with-timeout.py"
qpa = sys.argv[1] if len(sys.argv) == 2 else "xcb"
if qpa not in {"xcb", "wayland"}:
    raise SystemExit("Usage: sample-platform-views.py [xcb|wayland]")
stamp = datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%d/%H%M%S-%f")
output = ROOT / "Doroti/artifacts/validation/linux-platform-view-sample" / stamp / qpa
output.mkdir(parents=True)
print(output, flush=True)


def run(name, command, env=None):
    with (output / (name + ".log")).open("w") as log:
        subprocess.run([sys.executable, str(WRAPPER), *map(str, command)], cwd=ROOT,
                       env=env, stdout=log, stderr=subprocess.STDOUT, check=True)


run("build", ["dotnet", "build", "DorotiTestbedApp/linux/DorotiTestbedApp.Linux.csproj",
              "-c", "Release", "-r", "linux-x64", "-m:1", "-p:DorotiQtQuick=false"])
flags = shlex.split(subprocess.check_output(
    ["pkg-config", "--cflags", "--libs", "Qt6Widgets", "Qt6Test"], text=True))
probe = output / "sample-probe.so"
run("probe-build", ["c++", "-shared", "-fPIC", HERE / "sample-platform-views.cpp",
                    "-o", probe, *flags, "-ldl"])
env = os.environ.copy()
env.pop("DOROTI_PLATFORM_VIEW_COMPOSITION", None)
env.pop("DOROTI_QT_VALIDATION_RESIZE_CYCLES", None)
env.update(QT_QPA_PLATFORM=qpa, DOROTI_TESTBED_MODE="sample", LD_PRELOAD=str(probe),
           DOROTI_QT_SAMPLE_EVIDENCE=str(output))
run("sample", ["dotnet", "DorotiTestbedApp/linux/bin/linux-x64/Release/net10.0/linux-x64/"
               "DorotiTestbedApp.Linux.dll"], env)
log = (output / "sample.log").read_text()
assert "PROBE PASS" in log and "managed.fatal" not in log, log
print(f"PASS: {qpa} sample tab, native input, dispose/create, navigation, resize, popup/tooltip guard")
