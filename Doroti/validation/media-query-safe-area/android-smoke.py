"""API33 landscape emulator fixture. Run through run-with-timeout.py (1200 seconds)."""
from pathlib import Path
import hashlib
import json
import subprocess
import sys
import time
import xml.etree.ElementTree as ET

root = Path(__file__).resolve().parents[3]
evidence = root / "Doroti/validation/evidence/media-query-safe-area"
serial = sys.argv[1] if len(sys.argv) > 1 else "emulator-5554"
package = "dev.doroti.testbed"
apk = root / "DorotiTestbedApp/android/bin/android-x64/Debug/net10.0-android/android-x64/dev.doroti.testbed-Signed.apk"
manifest = ET.parse(root / "DorotiTestbedApp/android/obj/android-x64/Debug/net10.0-android/android-x64/AndroidManifest.xml")
android_name = "{http://schemas.android.com/apk/res/android}name"
activity = next(node.get(android_name) for node in manifest.iter("activity")
                if any(item.get(android_name) == "android.intent.action.MAIN" for item in node.iter("action")))
def adb(*arguments, binary=False):
    value = subprocess.check_output(["adb", "-s", serial, *arguments], timeout=1200)
    return value if binary else value.decode("utf-8", "replace")

if adb("shell", "getprop", "ro.build.version.sdk").strip() != "33" or not serial.startswith("emulator-"):
    raise SystemExit("This coordinate fixture is only for the observed API33 landscape emulator.")
adb("install", "-r", "-t", str(apk))
adb("shell", "am", "force-stop", package)
started = int(adb("shell", "date", "+%s").strip())
cases = {}
try:
    adb("shell", "am", "start", "-W", "-n", package + "/" + activity,
        "--es", "doroti_testbed_mode", "media-query", "--es", "DOROTI_MAUI_EVIDENCE", "1")
    time.sleep(2)
    remote = "/sdcard/Android/data/" + package + "/cache/doroti-maui-evidence.json"
    ready_by = time.monotonic() + 30
    while int(adb("shell", "stat", "-c", "%Y", remote).strip()) < started and time.monotonic() < ready_by:
        time.sleep(.25)
    def capture(name):
        assert int(adb("shell", "stat", "-c", "%Y", remote).strip()) >= started, "stale device evidence"
        data = json.loads(adb("shell", "cat", remote))
        (evidence / ("android-final-" + name + ".json")).write_text(json.dumps(data, indent=2), encoding="utf-8")
        (evidence / ("android-final-" + name + ".png")).write_bytes(adb("exec-out", "screencap", "-p", binary=True))
        assert data["surface"]["pixelWidth"] == 1920 and data["surface"]["pixelHeight"] == 1200, "fixture geometry changed"
        assert data["surface"]["nativeEnvironmentPhysicalSize"] == {"width": 1920, "height": 1200}
        assert data["frame"]["failed"] == 0
        cases[name] = data["surface"]
    capture("initial")
    adb("shell", "input", "tap", "350", "590")
    time.sleep(2)
    adb("shell", "input", "text", "mqtest")
    time.sleep(1)
    capture("caret")
    assert cases["caret"]["rawViewInsets"]["bottom"] > 0
    adb("shell", "input", "tap", "318", "394")
    time.sleep(1)
    capture("no-resize")
    adb("shell", "input", "keyevent", "4")
    time.sleep(2)
    capture("hidden")
    pid = adb("shell", "pidof", package).strip()
    log = adb("logcat", "-d", "--pid=" + pid)
    (evidence / "android-final.log").write_text(log, encoding="utf-8")
    snapshots = [json.loads(line.split("MQ-FIXTURE ", 1)[1]) for line in log.splitlines() if "MQ-FIXTURE " in line]
    assert any(not item["resize"] and item["viewInsets"]["bottom"] > 0 for item in snapshots)
    assert snapshots[-1]["viewInsets"]["bottom"] == 0
    assert all(case["rawViewPadding"]["bottom"] == 90 for case in cases.values())
    assert len({case["surfaceGeneration"] for case in cases.values()}) == 1
    assert "AssertionError" not in log and "I DOTNET  : System." not in log
    result = {"status": "automatedPassed", "physicalDevice": False, "api": 33,
              "device": serial, "apkSha256": hashlib.sha256(apk.read_bytes()).hexdigest(),
              "snapshots": snapshots, "surfaceCases": cases, "timeoutSeconds": 1200,
              "limits": "Observed landscape emulator only; physical Android, other APIs/navigation/fold/IME/rotation remain notVerified"}
    (evidence / "android-final.json").write_text(json.dumps(result, indent=2), encoding="utf-8")
    print("Android final APK IME/caret/no-extra-resize PASS", result["apkSha256"])
except Exception:
    pid = adb("shell", "pidof", package).strip()
    (evidence / "android-final-failure.log").write_text(adb("logcat", "-d", "--pid=" + pid), encoding="utf-8")
    raise
finally:
    adb("shell", "am", "force-stop", package)
