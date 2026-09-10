"""Run the owned Windows testbed for 8 seconds, close only its windows, timeout at 20 minutes."""
from pathlib import Path
import ctypes
import json
import os
import subprocess
import sys
import time

root = Path(__file__).resolve().parents[3]
mode = sys.argv[1] if len(sys.argv) > 1 else "native"
if mode not in ("native", "maui"):
    raise SystemExit("Expected native or maui")
folder, name = ("windowsappsdk", "WindowsAppSdk") if mode == "native" else ("windows", "Windows")
evidence = root / "Doroti/validation/evidence/media-query-safe-area"
label = "windows-native-smoke" if mode == "native" else "maui-windows-smoke"
exe = root / f"DorotiTestbedApp/{folder}/bin/Debug/net10.0-windows10.0.19041.0/win-x64/DorotiTestbedApp.{name}.exe"
env = os.environ.copy()
env.update(DOROTI_TESTBED_MODE="media-query", DOROTI_WINDOWS_APPSDK_SMOKE_MS="8000",
           DOROTI_MAUI_EVIDENCE=str(evidence / "maui-windows-runtime.json"))
with (evidence / (label + ".log")).open("w", encoding="utf-8") as log:
    process = subprocess.Popen([str(exe)], cwd=exe.parent, env=env, stdout=log, stderr=subprocess.STDOUT)
    time.sleep(8)
    callback_type = ctypes.WINFUNCTYPE(ctypes.c_bool, ctypes.c_void_p, ctypes.c_void_p)

    @callback_type
    def close_owned_window(hwnd, _):
        owner = ctypes.c_ulong()
        ctypes.windll.user32.GetWindowThreadProcessId(ctypes.c_void_p(hwnd), ctypes.byref(owner))
        if owner.value == process.pid and ctypes.windll.user32.IsWindowVisible(ctypes.c_void_p(hwnd)):
            ctypes.windll.user32.PostMessageW(ctypes.c_void_p(hwnd), 0x10, 0, 0)
        return True

    if process.poll() is None:
        ctypes.windll.user32.EnumWindows(close_owned_window, 0)
    try:
        code = process.wait(timeout=1192)
    except subprocess.TimeoutExpired:
        subprocess.run(["taskkill", "/PID", str(process.pid), "/T", "/F"], check=False)
        process.wait()
        code = 124
text = (evidence / (label + ".log")).read_text(encoding="utf-8", errors="replace")
snapshots = [json.loads(line.split("MQ-FIXTURE ", 1)[1]) for line in text.splitlines() if "MQ-FIXTURE " in line]
result = {"status": "automatedPassed" if code == 0 and snapshots and "Exception" not in text else "failed",
          "exitCode": code, "timeoutSeconds": 1200, "snapshots": snapshots,
          "scope": "Actual Windows initial hardware-rendered view and own-window close; touch keyboard and multi-DPI remain notVerified"}
(evidence / (label + ".json")).write_text(json.dumps(result, indent=2), encoding="utf-8")
print(label, result["status"], code, "snapshots", len(snapshots))
sys.exit(0 if result["status"] == "automatedPassed" else 1)
