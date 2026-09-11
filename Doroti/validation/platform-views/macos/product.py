"""Exercise the built AppKit Testbed. LaunchServices is deliberately bypassed to retain logs and process ownership."""
import json
import os
from pathlib import Path
import subprocess
import time

root = Path(__file__).resolve().parents[4]
artifacts = root / "Doroti/artifacts/validation/platform-views/macos-product"
artifacts.mkdir(parents=True, exist_ok=True)
executables = list((root / "DorotiTestbedApp/macos/bin/Debug").glob("**/*.app/Contents/MacOS/DorotiTestbedApp.MacOS"))
if len(executables) != 1:
    raise RuntimeError("Build the AppKit Testbed Debug runner first; expected one executable, found " + str(executables))
for renderer, native in (("1", True), ("0", True), ("1", False), ("0", False)):
    name = ("graphite" if renderer == "1" else "ganesh") + ("" if native else "-zero")
    result = artifacts / (name + ".json")
    failure = Path(str(result) + ".exception.txt")
    metrics = artifacts / (name + "-frames.json")
    for path in (result, failure, metrics, Path(str(metrics) + ".exception.txt")):
        path.unlink(missing_ok=True)
    environment = dict(os.environ, DOROTI_TESTBED_MODE="platform-views" if native else "sample", DOROTI_PLATFORM_VIEW_COMPOSITION="overlay",
                       DOROTI_MACOS_GRAPHITE=renderer, DOROTI_PLATFORM_VIEW_EVIDENCE=str(result), DOROTI_MAUI_EVIDENCE=str(metrics))
    if not native: environment.pop("DOROTI_PLATFORM_VIEW_EVIDENCE", None)
    with (artifacts / (name + ".log")).open("w") as log:
        process = subprocess.Popen([str(executables[0])], cwd=root, env=environment, stdout=log, stderr=subprocess.STDOUT)
        try:
            deadline = time.monotonic() + 60
            while True:
                if failure.exists(): raise RuntimeError(failure.read_text())
                if process.poll() is not None: raise RuntimeError(f"Product exited early: {process.returncode}; see {name}.log")
                if time.monotonic() > deadline: raise TimeoutError("Product evidence did not arrive: " + name)
                try:
                    frames = json.loads(metrics.read_text()) if metrics.exists() else None
                    if frames and frames["frame"]["failed"] > 0:
                        raise RuntimeError("Product reported failed GPU frames: " + name)
                    if frames and frames["frame"]["presented"] > 0 and (not native or result.exists()): break
                except json.JSONDecodeError:
                    pass  # The existing diagnostics writer can be observed between truncate and write.
                time.sleep(.1)
            if native:
                data = json.loads(result.read_text())
                assert data["passed"] and len(data["identifiers"]) == 2
            # A native subtree alone is insufficient: require successful product GPU frames too.
            time.sleep(1)
            frames = json.loads(metrics.read_text())
            assert frames["frame"]["presented"] > 0 and frames["frame"]["failed"] == 0, frames["frame"]
            screenshot = subprocess.run(["screencapture", "-x", "-l", str(data["windowNumber"]), str(artifacts / (name + ".png"))],
                                        capture_output=True, text=True) if native else None
            print(json.dumps({"renderer": name, "productProbe": "passed", "gpuCompleted": frames["frame"]["presented"],
                              "screenshotExitCode": screenshot.returncode if screenshot else None,
                              "screenshotError": screenshot.stderr.strip() if screenshot else None}), flush=True)
        finally:
            if process.poll() is None:
                process.terminate()
                try: process.wait(timeout=10)
                except subprocess.TimeoutExpired: process.kill(); process.wait()
print("PASS AppKit product NativeOverlay (Graphite and Ganesh); real input/IME/VoiceOver/C/close-reopen remain notVerified", flush=True)
