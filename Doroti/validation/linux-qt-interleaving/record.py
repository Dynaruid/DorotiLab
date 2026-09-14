"""Build and record independent Qt interleaving probes; each command has a 1200s timeout."""
import argparse
import datetime
import hashlib
import json
import os
from pathlib import Path
import subprocess
import sys

ROOT = Path(__file__).resolve().parents[3]
HERE = Path(__file__).resolve().parent
WRAPPER = ROOT / "Doroti/validation/run-with-timeout.py"
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument("--webengine", action="store_true", help="also build/test the optional system WebEngine target")
parser.add_argument("--qpa", choices=["xcb", "wayland", "all"], default="all")
args = parser.parse_args()
now = datetime.datetime.now(datetime.timezone.utc)
output = ROOT / "Doroti/artifacts/platform-views" / now.strftime("%Y-%m-%d") / "linux-qt-interleaving" / now.strftime("%H%M%S-%f")
output.mkdir(parents=True)
result = {"startedUtc": now.isoformat(), "scope": "independent topology experiment",
          "productIntegrated": False, "physicalInputVerified": False, "scanoutVerified": False,
          "head": subprocess.check_output(["git", "rev-parse", "HEAD"], cwd=ROOT, text=True).strip(),
          "sources": {p.name: hashlib.sha256(p.read_bytes()).hexdigest() for p in HERE.iterdir() if p.is_file()},
          "commands": [], "probes": [], "status": "RUNNING"}
print(output, flush=True)


def run(name, command, env=None):
    command = list(map(str, command))
    entry = {"command": command, "environment": env or {}, "log": name + ".log", "timeoutSeconds": 1200}
    result["commands"].append(entry)
    with (output / entry["log"]).open("w") as log:
        process = subprocess.run([sys.executable, str(WRAPPER), *command], cwd=ROOT,
                                 env=os.environ | (env or {}), stdout=log, stderr=subprocess.STDOUT)
    entry["exitCode"] = process.returncode
    return process.returncode


try:
    build = output / "build"
    if run("configure", ["cmake", "-S", HERE, "-B", build, "-G", "Ninja", "-DCMAKE_BUILD_TYPE=Release",
                         "-DDOROTI_PROBE_WEBENGINE=" + ("ON" if args.webengine else "OFF")]):
        raise RuntimeError("CMake configuration failed")
    if run("build", ["cmake", "--build", build, "-j", "2"]):
        raise RuntimeError("Build failed")
    kinds = ["widget-shell", "webengine-shell"] if args.webengine else ["widget-shell"]
    qpas = ["xcb", "wayland"] if args.qpa == "all" else [args.qpa]
    for kind in kinds:
        for qpa in qpas:
            name = kind + "-" + qpa
            directory = output / name
            code = run(name, [build / kind, directory], {"QT_QPA_PLATFORM": qpa})
            path = directory / "result.json"
            probe = json.loads(path.read_text()) if path.exists() else {"error": "No probe report"}
            result["probes"].append({"name": name, "exitCode": code, "result": probe})
            print(f"{name}: exit={code}, visual={probe.get('visualStatus', 'notVerified')}", flush=True)
    result["status"] = "PROBE_CHECKS_PASS" if all(
        p["exitCode"] == 0 and p["result"].get("automatedChecksPassed") for p in result["probes"]) else "FAIL"
except Exception as error:
    result.update(status="FAIL", error=str(error))
finally:
    result["finishedUtc"] = datetime.datetime.now(datetime.timezone.utc).isoformat()
    (output / "result.json").write_text(json.dumps(result, indent=2) + "\n")
print(result["status"])
raise SystemExit(0 if result["status"] == "PROBE_CHECKS_PASS" else 1)
