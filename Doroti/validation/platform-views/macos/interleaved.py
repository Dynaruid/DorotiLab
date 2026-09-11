"""Real AppKit product composition and native-state evidence on both Metal renderers."""
import json
import os
from pathlib import Path
import subprocess
import time
from pixels import validate

root = Path(__file__).resolve().parents[4]
artifacts = root / "Doroti/artifacts/validation/platform-views/macos-interleaved"
artifacts.mkdir(parents=True, exist_ok=True)
executables = list((root / "DorotiTestbedApp/macos/bin/Debug").glob("**/*.app/Contents/MacOS/DorotiTestbedApp.MacOS"))
if len(executables) != 1:
    raise RuntimeError("Build the AppKit Testbed Debug runner first.")

for renderer in ("1", "0"):
    name = "graphite" if renderer == "1" else "ganesh"
    result = artifacts / (name + ".json")
    metrics = artifacts / (name + "-frames.json")
    failure = Path(str(result) + ".exception.txt")
    for path in artifacts.glob(name + "*"):
        path.unlink()
    environment = dict(os.environ, DOROTI_TESTBED_MODE="platform-views", DOROTI_PLATFORM_VIEW_COMPOSITION="interleaved",
                       DOROTI_MACOS_GRAPHITE=renderer, DOROTI_PLATFORM_VIEW_EVIDENCE=str(result), DOROTI_MAUI_EVIDENCE=str(metrics))
    with (artifacts / (name + ".log")).open("w") as log:
        process = subprocess.Popen([str(executables[0])], cwd=root, env=environment, stdout=log, stderr=subprocess.STDOUT)
        stages = []
        try:
            for stage in (0, 1, 2, 3, 4, 5, 6, 7, 8, 9):
                stage_result = Path(str(result) + f".stage-{stage}.json")
                Path(str(result) + ".stage").write_text(str(stage))
                deadline = time.monotonic() + 60
                while not stage_result.exists():
                    if failure.exists(): raise RuntimeError(failure.read_text())
                    if process.poll() is not None: raise RuntimeError(f"Product exited: {process.returncode}; see {name}.log")
                    if time.monotonic() > deadline: raise TimeoutError(f"Stage {stage} evidence did not arrive for {name}")
                    time.sleep(.1)
                data = json.loads(stage_result.read_text())
                assert data["passed"] and data["editorText"] == "preserved-native-state", data
                if stages: assert stages[0]["identifiers"] == data["identifiers"], "Native identity changed"
                screenshot = artifacts / f"{name}-stage-{stage}.png"
                subprocess.run(["screencapture", "-x", "-o", "-l", str(data["windowNumber"]), str(screenshot)], check=True)
                data["screenshot"] = screenshot.name
                # Window captures carry the display's ICC profile (e.g. Display P3).
                # Preserve that original and compare a ColorSync-converted sRGB copy.
                normalized = screenshot.with_name(screenshot.stem + "-srgb.png")
                subprocess.run(["sips", "--matchTo", "/System/Library/ColorSync/Profiles/sRGB Profile.icc",
                                str(screenshot), "--out", str(normalized)], check=True, capture_output=True)
                data["pixelScreenshot"] = normalized.name
                stages.append(data)
                print(json.dumps({"renderer": name, "stage": stage, "hit": data["hitTarget"], "rasters": data["rasterOrders"]}), flush=True)
            time.sleep(1)
            frames = json.loads(metrics.read_text())
            assert frames["frame"]["presented"] > 0 and frames["frame"]["failed"] == 0, frames["frame"]
            pixels = validate(stages, artifacts)
            result.write_text(json.dumps({"passed": True, "renderer": name, "gpu": frames["frame"], "stages": stages,
                "pixels": pixels,
                "physicalInput": "notVerified", "koreanIme": "notVerified", "voiceOver": "notVerified"}, indent=2))
        finally:
            if process.poll() is None:
                process.terminate()
                try: process.wait(timeout=10)
                except subprocess.TimeoutExpired: process.kill(); process.wait()
print("PASS AppKit product interleaving stages, native identity/state and hit targets; inspect captured pixels separately.", flush=True)
