"""Launch only this test's MAUI process, capture its window and close it normally.
Invoke through run-with-timeout.py (1200 seconds). GUI timings are not benchmarks.
"""
import ctypes
from ctypes import wintypes as w
import hashlib
import json
import os
from pathlib import Path
import subprocess
import sys
import time
from PIL import ImageGrab

exe = Path(sys.argv[1]).resolve()
manifest = None if sys.argv[2] == "-" else Path(sys.argv[2]).resolve()
output = Path(sys.argv[3]).resolve()
output.mkdir(parents=True, exist_ok=True)
u = ctypes.WinDLL("user32", use_last_error=True)
u.SetThreadDpiAwarenessContext.argtypes = [ctypes.c_void_p]
u.SetThreadDpiAwarenessContext.restype = ctypes.c_void_p
u.SetThreadDpiAwarenessContext(ctypes.c_void_p(-4))
k = ctypes.WinDLL("kernel32", use_last_error=True)
p = ctypes.WinDLL("psapi", use_last_error=True)
enum_callback = ctypes.WINFUNCTYPE(w.BOOL, w.HWND, w.LPARAM)
u.EnumWindows.argtypes = [enum_callback, w.LPARAM]
u.GetWindowThreadProcessId.argtypes = [w.HWND, ctypes.POINTER(w.DWORD)]
u.IsWindowVisible.argtypes = [w.HWND]
u.GetWindowRect.argtypes = [w.HWND, ctypes.POINTER(w.RECT)]
u.SetForegroundWindow.argtypes = [w.HWND]
u.PostMessageW.argtypes = [w.HWND, w.UINT, w.WPARAM, w.LPARAM]
k.OpenProcess.argtypes = [w.DWORD, w.BOOL, w.DWORD]
k.OpenProcess.restype = w.HANDLE
k.CloseHandle.argtypes = [w.HANDLE]
p.EnumProcessModulesEx.argtypes = [w.HANDLE, ctypes.POINTER(w.HMODULE), w.DWORD, ctypes.POINTER(w.DWORD), w.DWORD]
p.GetModuleFileNameExW.argtypes = [w.HANDLE, w.HMODULE, w.LPWSTR, w.DWORD]


def windows(pid):
    found = []
    @enum_callback
    def callback(hwnd, _):
        owner = w.DWORD()
        u.GetWindowThreadProcessId(hwnd, ctypes.byref(owner))
        rect = w.RECT()
        if owner.value == pid and u.IsWindowVisible(hwnd) and u.GetWindowRect(hwnd, ctypes.byref(rect)):
            if rect.right - rect.left > 100 and rect.bottom - rect.top > 100:
                found.append((hwnd, (rect.left, rect.top, rect.right, rect.bottom)))
        return True
    u.EnumWindows(callback, 0)
    return found


def native_paths(pid):
    handle = k.OpenProcess(0x410, False, pid)
    if not handle: raise ctypes.WinError(ctypes.get_last_error())
    try:
        modules = (w.HMODULE * 2048)()
        needed = w.DWORD()
        if not p.EnumProcessModulesEx(handle, modules, ctypes.sizeof(modules), ctypes.byref(needed), 3):
            raise ctypes.WinError(ctypes.get_last_error())
        if needed.value > ctypes.sizeof(modules): raise RuntimeError("Module inventory truncated")
        result = []
        for i in range(needed.value // ctypes.sizeof(w.HMODULE)):
            text = ctypes.create_unicode_buffer(32768)
            p.GetModuleFileNameExW(handle, modules[i], text, len(text))
            if Path(text.value).name.lower() in ("libskiasharp.dll", "libdorotigraphite.dll"):
                result.append(text.value)
        return result
    finally: k.CloseHandle(handle)


environment = os.environ.copy()
if manifest:
    environment["DOROTI_WINDOWS_GRAPHITE_OFFICIAL_MANIFEST"] = str(manifest)
else:
    environment.pop("DOROTI_WINDOWS_GRAPHITE_OFFICIAL_MANIFEST", None)
environment.pop("DOROTI_WINDOWS_GRAPHITE_NATIVE", None)
environment["DOROTI_TESTBED_MODE"] = "sample"
environment["DOROTI_MAUI_EVIDENCE"] = str(output / "host-evidence.json")
environment["DOROTI_WINDOWS_MAUI_GRAPHITE"] = "1"
environment["DOROTI_WINDOWS_COMPOSITION_SURFACE"] = "1"
report = dict(status="FAIL", evidenceKind="MAUI-product-launch-capture", performance="notVerified",
              physicalScanOut="notVerified", exe=str(exe), manifest=str(manifest), externalTimeoutSeconds=1200)
with (output / "stdout.log").open("w", encoding="utf-8") as stdout, (output / "stderr.log").open("w", encoding="utf-8") as stderr:
    child = subprocess.Popen([str(exe)], cwd=exe.parent, env=environment, stdout=stdout, stderr=stderr)
    report["pid"] = child.pid
    try:
        deadline = time.monotonic() + 90
        found = []
        while child.poll() is None and time.monotonic() < deadline:
            found = windows(child.pid)
            if found and native_paths(child.pid): break
            time.sleep(0.2)
        if not found or child.poll() is not None: raise RuntimeError(f"MAUI window did not survive startup; exit={child.poll()}")
        hwnd, bounds = found[0]
        u.SetForegroundWindow(hwnd)
        time.sleep(8)
        if child.poll() is not None: raise RuntimeError(f"MAUI exited during first rendering; exit={child.poll()}")
        found = windows(child.pid)
        if not found: raise RuntimeError("MAUI visible window disappeared")
        hwnd, bounds = found[0]
        report["window"] = dict(handle=int(hwnd), bounds=bounds)
        paths = native_paths(child.pid)
        expected = json.loads(manifest.read_text()) if manifest else dict(
            nativePath=str(exe.parent / "libSkiaSharp.dll"),
            sha256="07ce51fd59e099b9561b0327223c27b21aa5605b5b8f4484dd297fdb8c8725a1")
        if len(paths) != 1 or Path(paths[0]).resolve() != Path(expected["nativePath"]).resolve():
            raise RuntimeError(f"Unexpected loaded Skia assets: {paths}")
        report["loadedNative"] = dict(path=paths[0], sha256=hashlib.sha256(Path(paths[0]).read_bytes()).hexdigest())
        if report["loadedNative"]["sha256"] != expected["sha256"]: raise RuntimeError("Loaded native hash mismatch")
        ImageGrab.grab(bbox=bounds, all_screens=True).save(output / "window.png")
        report["capture"] = str(output / "window.png")
        start = time.monotonic()
        u.PostMessageW(hwnd, 0x10, 0, 0)
        while windows(child.pid) and time.monotonic() - start < 5 and child.poll() is None: time.sleep(0.01)
        report["windowCloseAcknowledgementMs"] = (time.monotonic() - start) * 1000
        report["windowStillVisibleAtDeadline"] = bool(windows(child.pid))
        report["exitCode"] = child.wait(timeout=30)
        report["closeToProcessExitMs"] = (time.monotonic() - start) * 1000
        if report["exitCode"] != 0 or report["windowStillVisibleAtDeadline"]: raise RuntimeError("MAUI close/exit failed")
        if (output / "host-evidence.json.exception.txt").exists():
            raise RuntimeError("MAUI reported a host failure; inspect host-evidence.json.exception.txt")
        report["status"] = "PASS-launch-close; captured pixels require visual review"
    except Exception as error:
        report["error"] = repr(error)
    finally:
        if child.poll() is None:
            for hwnd, _ in windows(child.pid): u.PostMessageW(hwnd, 0x10, 0, 0)
            try: child.wait(timeout=15)
            except subprocess.TimeoutExpired:
                child.kill(); child.wait(); report["forcedTermination"] = True; report["status"] = "FAIL"
(output / "report.json").write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")
print(json.dumps(report, indent=2))
raise SystemExit(0 if report["status"].startswith("PASS") else 1)
