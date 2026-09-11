"""Actual HWND close while its GPU queue waits on a real delayed producer.
Run through run-with-timeout.py. Only the child created here receives WM_CLOSE.
"""
import ctypes
from ctypes import wintypes as w
import json
import os
from pathlib import Path
import subprocess
import sys
import time

exe, manifest, output = map(lambda p: Path(p).resolve(), sys.argv[1:])
output.mkdir(parents=True, exist_ok=True)
u = ctypes.WinDLL("user32")
callback_type = ctypes.WINFUNCTYPE(w.BOOL, w.HWND, w.LPARAM)
u.EnumWindows.argtypes = [callback_type, w.LPARAM]
u.GetWindowThreadProcessId.argtypes = [w.HWND, ctypes.POINTER(w.DWORD)]
u.IsWindowVisible.argtypes = [w.HWND]
u.PostMessageW.argtypes = [w.HWND, w.UINT, w.WPARAM, w.LPARAM]

def windows(pid):
    found = []
    @callback_type
    def callback(hwnd, _):
        owner = w.DWORD()
        u.GetWindowThreadProcessId(hwnd, ctypes.byref(owner))
        if owner.value == pid and u.IsWindowVisible(hwnd): found.append(hwnd)
        return True
    u.EnumWindows(callback, 0)
    return found

marker = output / "producer.txt"
env = os.environ.copy()
env.update(DOROTI_WINDOWS_GRAPHITE_OFFICIAL_MANIFEST=str(manifest),
           DOROTI_WINDOWS_VULKAN_VALIDATION="1", DOROTI_WINDOWS_GRAPHITE_DELAY_MS="7000",
           DOROTI_WINDOWS_GRAPHITE_DELAY_MARKER=str(marker))
report = dict(status="FAIL", boundary="actual-window-visibility-and-process-retirement",
              externalTimeoutSeconds=1200, realDeviceLoss="notTested", permanentStall="notTested")
with (output / "stdout.log").open("w") as stdout, (output / "stderr.log").open("w") as stderr:
    child = subprocess.Popen([str(exe), "--presenter", "default", "--smoke-ms", "60000",
                             "--no-resize-burst", "--lifecycle-cycles", "0", "--report", str(output / "product.json")],
                            cwd=exe.parent, env=env, stdout=stdout, stderr=stderr)
    report["pid"] = child.pid
    try:
        deadline = time.monotonic() + 45
        while child.poll() is None and not marker.exists() and time.monotonic() < deadline: time.sleep(.01)
        owned = windows(child.pid)
        if not marker.exists() or not owned: raise RuntimeError("Real GPU wait or visible product window was not observed")
        start = time.monotonic()
        for hwnd in owned: u.PostMessageW(hwnd, 0x10, 0, 0)
        while windows(child.pid) and time.monotonic() - start < 5: time.sleep(.005)
        report["closeVisibilityMs"] = (time.monotonic() - start) * 1000
        report["hiddenWithinBudget"] = not windows(child.pid)
        report["exitCode"] = child.wait(timeout=30)
        report["closeToReclaimMs"] = (time.monotonic() - start) * 1000
        # The ordinary product fixture intentionally rejects a timed-out frame.
        # Its expected nonzero exit is not normal product qualification.
        stderr.flush(); stdout.flush()
        logs = (output / "stderr.log").read_text(errors="replace") + (output / "stdout.log").read_text(errors="replace")
        report["producerCompleted"] = "qualification producer completed with Success" in logs
        report["validationDiagnostics"] = [line for line in logs.splitlines() if "VUID-" in line or "[skia] WARNING" in line or "[skia] ERROR" in line]
        if not report["hiddenWithinBudget"] or not report["producerCompleted"] or report["validationDiagnostics"]:
            raise RuntimeError("Visibility, real producer completion or validation requirement failed")
        if report["closeToReclaimMs"] < 5000: raise RuntimeError("Fixture did not retain the delayed GPU owner")
        report["status"] = "PASS-delayed-close; normal-product-fixture-may-fail-on-expected-timeout"
    except Exception as error: report["error"] = repr(error)
    finally:
        if child.poll() is None:
            for hwnd in windows(child.pid): u.PostMessageW(hwnd, 0x10, 0, 0)
            try: child.wait(timeout=20)
            except subprocess.TimeoutExpired:
                child.kill(); child.wait(); report["forcedTermination"] = True; report["status"] = "FAIL"
(output / "report.json").write_text(json.dumps(report, indent=2) + "\n")
print(json.dumps(report, indent=2))
raise SystemExit(0 if report["status"].startswith("PASS") else 1)
