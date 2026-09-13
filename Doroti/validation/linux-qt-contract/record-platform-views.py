"""Record Linux PV-9 gates. Every command has an external 1200 second timeout.

Run from any directory; uses installed system Qt, without changing sandbox/QPA
security settings. QtWebEngine is linked only by the optional attachment probe.
"""
import datetime
import hashlib
import json
import os
from pathlib import Path
import shlex
import subprocess
import sys

ROOT = Path(__file__).resolve().parents[3]
VALIDATION = ROOT / "Doroti/validation/linux-qt-contract"
NATIVE = ROOT / "DorotiTestbedApp/linux/native"
BUILD = ROOT / "Doroti/artifacts/validation/linux-platform-views-native"
WRAPPER = ROOT / "Doroti/validation/run-with-timeout.py"
gate = sys.argv[1] if len(sys.argv) == 2 else ""
allowed = {"build", "managed", "owners-xcb", "owners-wayland", "attachment-xcb", "attachment-wayland",
           "abi-xcb", "abi-wayland", "product-build", "product-xcb", "product-wayland", "dependencies", "opengl-titlebar", "accessibility"}
if gate not in allowed:
    raise SystemExit("Choose: " + ", ".join(sorted(allowed)))
now = datetime.datetime.now(datetime.timezone.utc)
output = ROOT / "Doroti/artifacts/platform-views" / now.strftime("%Y-%m-%d") / "linux-qt" / (gate + "-" + now.strftime("%H%M%S-%f"))
output.mkdir(parents=True)
result = {"gate": gate, "startedUtc": now.isoformat(), "timeoutSeconds": 1200, "commands": [],
          "head": subprocess.check_output(["git", "rev-parse", "HEAD"], cwd=ROOT, text=True).strip(),
          "dirty": subprocess.check_output(["git", "status", "--short"], cwd=ROOT, text=True),
          "sourceReviewed": True, "build": False, "automated": False, "productLive": False,
          "physical": False, "nativeAot": False, "c1ThroughC6": "notVerified",
          "platform": os.uname()._asdict() if hasattr(os.uname(), "_asdict") else list(os.uname()),
          "sources": {str(p.relative_to(ROOT)): hashlib.sha256(p.read_bytes()).hexdigest()
                      for base in [NATIVE, ROOT / "Doroti/src/Doroti.Host.Qt", VALIDATION]
                      for p in base.rglob("*") if p.is_file() and p.suffix in {".cpp", ".h", ".cs", ".py"}
                      and not {"bin", "obj"}.intersection(p.parts)}}


def run(command, env=None):
    command = [str(value) for value in command]
    record = {"command": command, "environment": env or {}}
    result["commands"].append(record)
    log = output / (str(len(result["commands"])) + ".log")
    with log.open("w") as stream:
        process = subprocess.run([sys.executable, str(WRAPPER), *command], cwd=ROOT,
                                 env=os.environ | (env or {}), stdout=stream, stderr=subprocess.STDOUT)
    record.update(exitCode=process.returncode, log=log.name)
    if process.returncode:
        raise RuntimeError(f"exit {process.returncode}: {shlex.join(command)}; see {log}")


try:
    if gate == "accessibility":
        directory = BUILD / "accessibility"
        run(["cmake", "-S", ROOT / "Doroti/validation/accessibility-projection-native", "-B", directory, "-G", "Ninja"])
        run(["cmake", "--build", directory, "-j", "4"])
        run(["ctest", "--test-dir", directory, "--output-on-failure"])
        result["automated"] = True
    elif gate == "opengl-titlebar":
        directory = BUILD / "opengl"
        run(["cmake", "-S", NATIVE, "-B", directory, "-G", "Ninja", "-DDOROTI_QT_GRAPHITE=OFF"])
        run(["cmake", "--build", directory, "-j", "4"])
        flags = shlex.split(subprocess.check_output(["pkg-config", "--cflags", "--libs", "Qt6Core", "Qt6Gui"], text=True))
        run(["g++", "-std=c++20", VALIDATION / "titlebar.cpp", "-I", NATIVE / "include", *flags, "-ldl", "-o", output / "probe"])
        for mode in ["unified", "solid"]:
            run([output / "probe", directory / "libdoroti_qt_host.so", mode], {"QT_QPA_PLATFORM": "xcb"})
        result["automated"] = True
    elif gate == "build":
        run(["cmake", "-S", NATIVE, "-B", BUILD, "-G", "Ninja", "-DCMAKE_BUILD_TYPE=Debug"])
        run(["cmake", "--build", BUILD, "-j", "4"])
        result["build"] = True
    elif gate == "managed":
        run(["dotnet", "run", "--project", VALIDATION / "Doroti.Validation.LinuxQtContract.csproj"])
        result["automated"] = True
    elif gate == "product-build":
        run(["dotnet", "build", ROOT / "DorotiTestbedApp/linux/DorotiTestbedApp.Linux.csproj", "-c", "Debug", "-r", "linux-x64", "-m:1"])
        result["build"] = True
    elif gate.startswith("product-"):
        qpa = gate.split("-")[1]
        run(["dotnet", ROOT / "DorotiTestbedApp/linux/bin/linux-x64/Debug/net10.0/linux-x64/DorotiTestbedApp.Linux.dll"],
            {"QT_QPA_PLATFORM": qpa, "DOROTI_TESTBED_MODE": "platform-views", "DOROTI_PLATFORM_VIEW_COMPOSITION": "overlay",
             "DOROTI_QT_VALIDATION_RESIZE_CYCLES": "50", "DOROTI_QT_VALIDATION_PLATFORM_VIEWS": str(output / "window")})
        window = json.loads((output / "window.json").read_text())
        assert len(window["controls"]) == 2 and all(c["visible"] and c["parentMatches"] for c in window["controls"]), window
        assert any(c["kind"] == "QLineEdit" and c["text"].endswith("!") for c in window["controls"]), window
        result.update(automated=True, productLive=True, windowCapture=window["windowCapture"])
    elif gate == "dependencies":
        run(["ldd", BUILD / "libdoroti_qt_host.so"])
        deps = (output / "1.log").read_text()
        assert not any(name in deps for name in ["WebEngine", "WebChannel", "Qt6Quick"]), deps
        template = ROOT / "Doroti/templates/Doroti.Templates/content/doroti-app/linux/native"
        for path in NATIVE.rglob("*"):
            if path.is_file():
                assert path.read_bytes() == (template / path.relative_to(NATIVE)).read_bytes(), path
        result["automated"] = True
    else:
        kind, qpa = gate.split("-")
        modules = ["Qt6Widgets", "Qt6Test"]
        source = "native.cpp" if kind == "abi" else "platform-view-owners.cpp" if kind == "owners" else "platform-views.cpp"
        if kind == "attachment":
            modules += ["Qt6WebEngineWidgets"]
        # Read-only build flag discovery, no shell evaluation of tool output.
        flags = shlex.split(subprocess.check_output(["pkg-config", "--cflags", "--libs", *modules], text=True))
        command = ["g++", "-std=c++20", VALIDATION / source, "-I", NATIVE / "include"]
        if kind == "owners":
            command += ["-DDOROTI_QT_HOST_BUILD", NATIVE / "src/doroti_qt_platform_views.cpp"]
        command += [*flags, "-ldl", "-o", output / "probe"]
        run(command)
        args = [] if kind == "owners" else [BUILD / "libdoroti_qt_host.so"]
        if kind == "attachment":
            args += [output / "window.png"]
        run([output / "probe", *args], {"QT_QPA_PLATFORM": qpa})
        result["automated"] = True
    result["status"] = "PASS"
except Exception as error:
    result.update(status="FAIL", error=str(error))
finally:
    result["finishedUtc"] = datetime.datetime.now(datetime.timezone.utc).isoformat()
    (output / "result.json").write_text(json.dumps(result, indent=2, ensure_ascii=False) + "\n")
    print(output)
raise SystemExit(0 if result["status"] == "PASS" else 1)
