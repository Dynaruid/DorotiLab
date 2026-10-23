"""Run the Release MAUI WinUI editing probe; physical keyboard/IME not verified."""
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
parser.add_argument('--exe', type=Path, default=ROOT / 'samples/DorotiTestbedApp/windows/bin/x64/Release/net10.0-windows10.0.19041.0/win-x64/DorotiTestbedApp.Windows.exe')
parser.add_argument('--output', type=Path, required=True)
args = parser.parse_args()
out, exe = args.output.resolve(), args.exe.resolve()
if not out.is_relative_to(ROOT / 'temp/testing') or out.exists():
    raise RuntimeError('Use a fresh output directory under temp/testing.')
out.mkdir(parents=True)
report = out / 'result.json'
env = {key: value for key, value in os.environ.items() if not (key.startswith('DOROTI_') and 'PROBE' in key)}
env.update(DOROTI_TEXT_EDITING_PROBE=str(report), DOROTI_MAUI_EVIDENCE=str(out / 'host.json'))
user32 = ctypes.WinDLL('user32')
user32.GetWindowThreadProcessId.argtypes = [ctypes.c_void_p, ctypes.POINTER(ctypes.c_ulong)]
user32.IsHungAppWindow.argtypes = [ctypes.c_void_p]
user32.IsWindowVisible.argtypes = [ctypes.c_void_p]
observed = []
with (out / 'process.log').open('w', encoding='utf-8') as log:
    with subprocess.Popen([str(exe)], cwd=ROOT, env=env, stdout=log, stderr=subprocess.STDOUT) as process:
        deadline = time.monotonic() + 180
        callback_type = ctypes.WINFUNCTYPE(ctypes.c_bool, ctypes.c_void_p, ctypes.c_void_p)

        @callback_type
        def observe(hwnd, _):
            pid = ctypes.c_ulong()
            user32.GetWindowThreadProcessId(hwnd, ctypes.byref(pid))
            if pid.value == process.pid and user32.IsWindowVisible(hwnd):
                observed.append({'hwnd': hwnd, 'responding': not bool(user32.IsHungAppWindow(hwnd))})
            return True

        try:
            while not report.exists():
                user32.EnumWindows(observe, None)
                error = out / 'host.json.exception.txt'
                if error.exists():
                    raise RuntimeError(error.read_text(encoding='utf-8-sig'))
                if process.poll() is not None:
                    raise RuntimeError(f'App exited before report: {process.returncode}')
                if time.monotonic() > deadline:
                    raise TimeoutError('Live Windows editing probe timed out.')
                time.sleep(.1)
            result = json.loads(report.read_text(encoding='utf-8-sig'))
            assert result['status'] == 'PASS', result
            # This fixture qualifies input callbacks, not application shutdown.
            # Retire only the process launched here if native threads outlive
            # window.Close; retain that boundary in the evidence.
            cleanup = 'natural-exit'
            try:
                code = process.wait(timeout=5)
            except subprocess.TimeoutExpired:
                cleanup = 'runner-terminated-after-probe'
                process.terminate()
                code = process.wait(timeout=10)
            result.update(processExitCode=code, responsiveWindowObserved=any(x['responding'] for x in observed),
                          processCleanup=cleanup, applicationShutdown='PASS' if cleanup == 'natural-exit' and code == 0 else 'notVerified',
                          exeSha256=hashlib.sha256(exe.read_bytes()).hexdigest(),
                          appAssemblySha256=hashlib.sha256(exe.with_suffix('.dll').read_bytes()).hexdigest(),
                          bridgeSha256=hashlib.sha256((exe.parent / 'Doroti.Host.Maui.dll').read_bytes()).hexdigest())
            report.write_text(json.dumps(result, indent=2, ensure_ascii=False), encoding='utf-8')
            assert result['status'] == 'PASS', result
            assert result['responsiveWindowObserved'], result
            assert cleanup != 'natural-exit' or code == 0, result
            edits = sum(case.get('repetitions', 1) for case in result['cases'])
            print(f"PASS: Release MAUI WinUI, {len(result['cases'])} cases / {edits} edits, responsive window; physical IME notVerified; cleanup={cleanup}")
        finally:
            if process.poll() is None:
                process.terminate()
                process.wait(timeout=10)
