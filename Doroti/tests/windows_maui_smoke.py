"""Windows MAUI integration using the MAUI executable, real WinUI controls and HWND dialogs."""
import argparse
import ctypes
import hashlib
import json
import os
from pathlib import Path
import subprocess
import time

ROOT = Path(__file__).resolve().parents[2]
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument("--output", required=True)
parser.add_argument("--mode", choices=("graphite", "embedded", "ganesh", "upload", "webview"), default="graphite")
args = parser.parse_args()
run = (ROOT / args.output).resolve()
if not run.is_relative_to(ROOT / "temp/testing"):
    raise ValueError("Evidence must be under temp/testing.")
run.mkdir(parents=True, exist_ok=True)
if any(run.iterdir()):
    raise FileExistsError("Use a fresh evidence directory; existing reports must not satisfy a new run.")
exe = ROOT / "samples/DorotiTestbedApp/windows/bin/Debug/net10.0-windows10.0.19041.0/win-x64/DorotiTestbedApp.Windows.exe"
report = run / "connections.json"
env = os.environ.copy()
for key in ("DOROTI_DESKTOP_PROBE", "DOROTI_INPUT_PROBE", "DOROTI_SAMPLE", "DOROTI_DESKTOP_SAMPLE", "DOROTI_DESKTOP_CLOSE_PROBE", "DOROTI_MAUI_EMBEDDED_PROBE"):
    env.pop(key, None)
env.update(DOROTI_MAUI_CONNECTION_PROBE=str(report), DOROTI_MAUI_EVIDENCE=str(run / "evidence.json"),
           DOROTI_WEBVIEW_USER_DATA=str(run / "webview-data"), DOROTI_WINDOWS_MAUI_GRAPHITE="0" if args.mode == "ganesh" else "1",
           DOROTI_WINDOWS_COMPOSITION_SURFACE="1")
if args.mode == "embedded":
    env["DOROTI_MAUI_EMBEDDED_PROBE"] = "1"
if args.mode in ("upload", "webview"):
    env.pop("DOROTI_MAUI_CONNECTION_PROBE")
    if args.mode == "upload":
        exe = ROOT / "samples/DorotiSampleApp2/windows/bin/Debug/net10.0-windows10.0.19041.0/win-x64/DorotiSampleApp2.Windows.exe"
        env["DOROTI_UPLOAD_PROBE"] = str(report)
        picked = run / "upload.txt"
        picked.write_text("Windows MAUI upload 한글", encoding="utf-8")
    else:
        env.update(DOROTI_SAMPLE="webview", DOROTI_WEBVIEW_SCENE_PROBE=str(report))
    user = ctypes.WinDLL("user32", use_last_error=True)
    user.GetWindowThreadProcessId.argtypes = [ctypes.c_void_p, ctypes.POINTER(ctypes.c_ulong)]
    user.GetClassNameW.argtypes = [ctypes.c_void_p, ctypes.c_wchar_p, ctypes.c_int]
    user.IsWindowVisible.argtypes = [ctypes.c_void_p]
    user.SetDlgItemTextW.argtypes = [ctypes.c_void_p, ctypes.c_int, ctypes.c_wchar_p]
    user.SetWindowTextW.argtypes = [ctypes.c_void_p, ctypes.c_wchar_p]
    user.SendMessageW.argtypes = [ctypes.c_void_p, ctypes.c_uint, ctypes.c_size_t, ctypes.c_ssize_t]
    user.SendMessageW.restype = ctypes.c_ssize_t
    user.GetDlgCtrlID.argtypes = [ctypes.c_void_p]
    user.PostMessageW.argtypes = [ctypes.c_void_p, ctypes.c_uint, ctypes.c_size_t, ctypes.c_ssize_t]
    callback_type = ctypes.WINFUNCTYPE(ctypes.c_bool, ctypes.c_void_p, ctypes.c_ssize_t)
    user.EnumWindows.argtypes = [callback_type, ctypes.c_ssize_t]
    user.EnumChildWindows.argtypes = [ctypes.c_void_p, callback_type, ctypes.c_ssize_t]
    def set_filename(dialog, path):
        fields = []
        def visit(hwnd, _):
            name = ctypes.create_unicode_buffer(128)
            user.GetClassNameW(hwnd, name, 128)
            if name.value == "Edit" and user.GetDlgCtrlID(hwnd) == 1148 and user.IsWindowVisible(hwnd):
                fields.append(hwnd)
            return True
        user.EnumChildWindows(dialog, callback_type(visit), 0)
        assert len(fields) == 1, "Visible filename edit missing"
        # Multi-select ComboBoxEx stores WM_SETTEXT separately from its filename model.
        # Character input through the real inner Edit emits the native change notifications.
        user.SendMessageW(fields[0], 0xB1, 0, -1)  # EM_SETSEL
        for character in path:
            user.SendMessageW(fields[0], 0x102, ord(character), 0)  # WM_CHAR
    def window_for(pid, cls=None):
        found = []
        def visit(hwnd, _):
            owner = ctypes.c_ulong()
            user.GetWindowThreadProcessId(hwnd, ctypes.byref(owner))
            name = ctypes.create_unicode_buffer(128)
            user.GetClassNameW(hwnd, name, 128)
            if owner.value == pid and user.IsWindowVisible(hwnd) and (cls is None or name.value == cls):
                found.append(hwnd)
            return True
        user.EnumWindows(callback_type(visit), 0)
        return found[0] if found else 0
with (run / "application.log").open("w", encoding="utf-8") as log:
    identity = {
        "baseHead": subprocess.check_output(["git", "rev-parse", "HEAD"], cwd=ROOT, text=True).strip(),
        "workingTree": True,
        "assemblies": {name: hashlib.sha256((exe.parent / name).read_bytes()).hexdigest() for name in
                       ("Doroti.Host.Maui.dll", "Doroti.Framework.Services.dll", "Doroti.Desktop.dll", "libSkiaSharp.dll")},
        "mode": args.mode,
    }
    (run / "identity.json").write_text(json.dumps(identity, indent=2), encoding="utf-8")
    process = subprocess.Popen([str(exe)], cwd=ROOT, env=env, stdout=log, stderr=subprocess.STDOUT)
    try:
        deadline = time.monotonic() + 1200
        dialog_sent = False
        while not report.exists():
            error = Path(str(report) + ".error")
            exception = run / "evidence.json.exception.txt"
            if error.exists() or exception.exists():
                raise RuntimeError((error if error.exists() else exception).read_text(encoding="utf-8-sig"))
            if process.poll() is not None:
                raise RuntimeError(f"MAUI exited before probe completion: {process.returncode}")
            if time.monotonic() > deadline:
                raise TimeoutError("Windows MAUI integration exceeded 1200 seconds.")
            if args.mode == "upload" and not dialog_sent and (dialog := window_for(process.pid, "#32770")):
                time.sleep(.5)
                set_filename(dialog, str(picked))
                assert user.PostMessageW(dialog, 0x111, 1, 0), "Picker accept failed"
                dialog_sent = True
            time.sleep(.1)
        if args.mode in ("upload", "webview"):
            if args.mode == "webview":
                from PIL import ImageGrab
                class WindowRect(ctypes.Structure):
                    _fields_ = [(name, ctypes.c_long) for name in ("left", "top", "right", "bottom")]
                user.GetWindowRect.argtypes = [ctypes.c_void_p, ctypes.POINTER(WindowRect)]
                rect = WindowRect()
                assert user.GetWindowRect(window_for(process.pid), ctypes.byref(rect))
                ImageGrab.grab(bbox=(rect.left, rect.top, rect.right, rect.bottom)).save(run / "window.png")
            assert user.PostMessageW(window_for(process.pid), 0x10, 0, 0), "MAUI close message failed"
        # Closing is an assertion with its own bounded native retirement window.
        code = process.wait(timeout=30)
        if code != 0:
            raise RuntimeError(f"MAUI close failed: {code}")
    finally:
        if process.poll() is None:
            process.terminate()
            process.wait()
result = json.loads(report.read_text(encoding="utf-8-sig"))
result["identity"] = identity
if args.mode == "webview":
    assert result["status"] == "PASS" and result["widgetScene"], result
    evidence = json.loads((run / "evidence.json").read_text(encoding="utf-8-sig"))
    assert evidence["frame"]["presented"] > 0 and evidence["bootstrapSource"] == "windows/App.xaml.cs", evidence["frame"]
    result["cleanClose"] = True
    report.write_text(json.dumps(result, indent=2), encoding="utf-8")
    print("Windows MAUI WebView widget scene: PASS (real controller, placement, raster frame and clean close; physical input notVerified)", flush=True)
    raise SystemExit(0)
if args.mode == "upload":
    assert result["status"] == "PASS" and result["readGrantsDisposed"], result
    assert result["previews"][0]["Text"] == "Windows MAUI upload 한글", result
    evidence = json.loads((run / "evidence.json").read_text(encoding="utf-8-sig"))
    assert evidence["bootstrapSource"] == "windows/App.xaml.cs", evidence["bootstrapSource"]
    result["cleanClose"] = True
    report.write_text(json.dumps(result, indent=2, ensure_ascii=False), encoding="utf-8")
    print("Windows MAUI Sample2 upload: PASS (real picker/preview/read grant/clean close; synthetic dialog commands)", flush=True)
    raise SystemExit(0)
assert result["status"] == "PASS" and result["backend"] == "Windows-MAUI", result
assert result["actual"]["nativeChildren"] == 0 and not result["actual"]["nativeWindowOutput"], result
assert result["actual"]["fullWindowOwner"] == (args.mode != "embedded"), result
assert all(result[key] for key in ("nativeButton", "nativeEditor", "webCommands", "staleDocumentRejected", "filePicker", "readGrant", "concurrentPickerRejected", "callerCancellation")), result
result["cleanClose"] = True
report.write_text(json.dumps(result, indent=2, ensure_ascii=False), encoding="utf-8")
print(f"Windows MAUI {args.mode}: PASS (native controls/WebView commands/picker/lifetime; physical input/display notVerified)", flush=True)
