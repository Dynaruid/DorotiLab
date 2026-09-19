"""Public controller commands in the real WindowsAppSDK product. Use the 1200s wrapper."""
import datetime
import json
import os
from pathlib import Path
import subprocess
import time

ROOT = Path(__file__).resolve().parents[3]
OUT = Path(os.environ.get('DOROTI_WEBVIEW_GATE_OUTPUT') or ROOT / 'Doroti/artifacts/webview' / datetime.date.today().isoformat() / 'windows' / time.strftime('%H%M%S'))
EXE = Path(os.environ.get('DOROTI_WEBVIEW_GATE_EXE') or ROOT / 'DorotiTestbedApp/windowsappsdk/bin/Release/net10.0-windows10.0.19041.0/win-x64/DorotiTestbedApp.WindowsAppSdk.exe')

def main():
    OUT.mkdir(parents=True, exist_ok=True)
    result = OUT / 'commands.txt'
    env = os.environ.copy()
    env.update(DOROTI_TESTBED_MODE='platform-effects', DOROTI_WINDOWS_WEBVIEW_EVIDENCE=str(result),
               DOROTI_PLATFORM_VIEW_EVIDENCE=str(OUT / 'frame.json'),
               DOROTI_WINDOWS_APPSDK_DIAGNOSTICS='1', DOROTI_WINDOWS_APPSDK_REPORT=str(OUT / 'report.json'),
               DOROTI_WEBVIEW_USER_DATA=str(OUT / 'profiles'))
    print(OUT, flush=True)
    with (OUT / 'product.log').open('w', encoding='utf-8') as log:
        process = subprocess.Popen([str(EXE)], cwd=EXE.parent, env=env, stdout=log, stderr=log)
        try:
            deadline = time.monotonic() + 120
            while time.monotonic() < deadline and process.poll() is None and not result.exists():
                time.sleep(.2)
            text = result.read_text(encoding='utf-8') if result.exists() else 'FAIL product fixture did not complete'
            print(text, flush=True)
            if 'FAIL' in text or 'PASS Windows WebView product commands' not in text:
                raise RuntimeError(text)
        finally:
            # Close only this fixture's top-level window, allowing coordinator retirement.
            import ctypes
            from ctypes import wintypes
            user = ctypes.WinDLL('user32')
            callback_type = ctypes.WINFUNCTYPE(wintypes.BOOL, wintypes.HWND, wintypes.LPARAM)
            def close(window, _):
                pid = wintypes.DWORD()
                user.GetWindowThreadProcessId(window, ctypes.byref(pid))
                if pid.value == process.pid:
                    user.PostMessageW(window, 0x10, 0, 0)
                return True
            user.EnumWindows(callback_type(close), 0)
            try: process.wait(timeout=20)
            except subprocess.TimeoutExpired:
                subprocess.run(['taskkill', '/PID', str(process.pid), '/T', '/F'], timeout=20, check=False)
                raise RuntimeError('Product close timed out')
            (OUT / 'exit.json').write_text(json.dumps({'exitCode': process.returncode}), encoding='utf-8')
            if process.returncode != 0:
                raise RuntimeError(f'Product exited {process.returncode}')

if __name__ == '__main__':
    main()
