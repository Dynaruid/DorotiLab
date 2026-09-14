"""Record the actual Qt Quick/Graphite product with external 1200s timeouts."""
import argparse
import datetime
import hashlib
import json
import os
from pathlib import Path
import shlex
import subprocess
import sys

ROOT = Path(__file__).resolve().parents[3]
HERE = Path(__file__).resolve().parent
WRAPPER = ROOT / "Doroti/validation/run-with-timeout.py"
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument("--qpa", choices=["xcb", "wayland", "all"], default="all")
parser.add_argument("--validation", action="store_true", help="enable the installed Khronos Vulkan validation layer")
args = parser.parse_args()
now = datetime.datetime.now(datetime.timezone.utc)
output = ROOT / "Doroti/artifacts/platform-views" / now.strftime("%Y-%m-%d") / "linux-qt-quick" / now.strftime("%H%M%S-%f")
output.mkdir(parents=True)
result = {"head": subprocess.check_output(["git", "rev-parse", "HEAD"], cwd=ROOT, text=True).strip(),
          "dirty": subprocess.check_output(["git", "status", "--short"], cwd=ROOT, text=True),
          "startedUtc": now.isoformat(), "productIntegrated": True, "physicalInputVerified": False,
          "scanoutVerified": False, "vulkanValidation": args.validation, "commands": [], "status": "RUNNING",
          "sources": {str(p.relative_to(ROOT)): hashlib.sha256(p.read_bytes()).hexdigest()
                      for base in [ROOT / "DorotiTestbedApp/linux/native", ROOT / "Doroti/src/Doroti.Host.Qt",
                                   ROOT / "Doroti/src/Doroti.Skia.Vulkan", HERE]
                      for p in base.rglob("*") if p.is_file() and p.suffix in {".cs", ".cpp", ".h", ".py"}
                      and not {"bin", "obj"}.intersection(p.parts)}}
print(output, flush=True)


def run(name, command, env=None):
    entry = {"command": list(map(str, command)), "environment": env or {}, "log": name + ".log", "timeoutSeconds": 1200}
    result["commands"].append(entry)
    with (output / entry["log"]).open("w") as log:
        process = subprocess.run([sys.executable, str(WRAPPER), *entry["command"]], cwd=ROOT,
                                 env=os.environ | (env or {}), stdout=log, stderr=subprocess.STDOUT)
    entry["exitCode"] = process.returncode
    if process.returncode:
        raise RuntimeError(f"{name}: exit {process.returncode}; see {entry['log']}")
    return (output / entry["log"]).read_text()


try:
    run("build", ["dotnet", "build", "DorotiTestbedApp/linux/DorotiTestbedApp.Linux.csproj",
                  "-c", "Release", "-r", "linux-x64", "-m:1", "-p:DorotiQtQuick=true"])
    flags = shlex.split(subprocess.check_output(["pkg-config", "--cflags", "--libs", "Qt6Widgets", "Qt6Quick", "Qt6Test"], text=True))
    probe = output / "product-probe.so"
    run("probe-build", ["c++", "-shared", "-fPIC", HERE / "product-probe.cpp", "-o", probe, *flags, "-ldl"])
    app = ROOT / "DorotiTestbedApp/linux/bin/linux-x64/Release/net10.0/linux-x64/DorotiTestbedApp.Linux.dll"
    run("abi-build", ["c++", "-DDOROTI_QT_HOST_BUILD", HERE / "abi.cpp", "-I", ROOT / "DorotiTestbedApp/linux/native/include",
                      "-L", app.parent, "-Wl,-rpath," + str(app.parent), "-ldoroti_qt_host", *flags, "-o", output / "abi"])
    run("abi", [output / "abi"], {"QT_QPA_PLATFORM": "wayland" if args.qpa == "wayland" else "xcb"})
    for qpa in (["xcb", "wayland"] if args.qpa == "all" else [args.qpa]):
        for mode in ["platform-views", "sample"]:
            name = mode + "-" + qpa
            evidence = output / name
            evidence.mkdir()
            env = {"QT_QPA_PLATFORM": qpa, "DOROTI_TESTBED_MODE": mode, "DOROTI_QT_DIAGNOSTICS": "1",
                   "DOROTI_PLATFORM_VIEW_COMPOSITION": "interleaved", "DOROTI_QUICK_EVIDENCE": str(evidence),
                   "LD_PRELOAD": str(probe), "DOROTI_QT_VALIDATION_RESIZE_CYCLES": "0"}
            if args.validation:
                env["VK_INSTANCE_LAYERS"] = "VK_LAYER_KHRONOS_validation"
                env["VK_LOADER_DEBUG"] = "layer"
            log = run(name, ["dotnet", app], env)
            assert "QUICK PRODUCT PASS" in log and "managed.fatal" not in log, name
            if args.validation:
                assert 'Insert instance layer "VK_LAYER_KHRONOS_validation"' in log, name + " validation layer was not enabled"
                assert "vkDebug:" not in log and "Validation Error" not in log, name + " Vulkan validation error"
            summary = json.loads(log.split("doroti.qt.summary=")[-1].splitlines()[0])
            assert summary["frames"]["failed"] == 0 and summary["fullFrameCpuCopies"] == 0, summary
            print(name + ": PASS", flush=True)
    result["status"] = "PASS"
except Exception as error:
    result.update(status="FAIL", error=str(error))
finally:
    result["finishedUtc"] = datetime.datetime.now(datetime.timezone.utc).isoformat()
    (output / "result.json").write_text(json.dumps(result, indent=2) + "\n")
print(result["status"])
raise SystemExit(0 if result["status"] == "PASS" else 1)
