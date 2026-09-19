"""Bounded 0/1/4-view product workload observations, not frame-rate acceptance."""
import ctypes as c
from ctypes import wintypes as w
import datetime
import json
import os
from pathlib import Path
import subprocess
import time

ROOT = Path(__file__).resolve().parents[3]
OUT = ROOT / 'Doroti/artifacts/webview' / datetime.date.today().isoformat() / 'windows' / ('workloads-' + time.strftime('%H%M%S'))
EXE = ROOT / 'DorotiTestbedApp/windowsappsdk/bin/Release/net10.0-windows10.0.19041.0/win-x64/DorotiTestbedApp.WindowsAppSdk.exe'

class Memory(c.Structure):
    _fields_ = [('cb', w.DWORD), ('faults', w.DWORD)] + [(name, c.c_size_t) for name in
        ('peakWorking', 'working', 'peakPaged', 'paged', 'peakNonPaged', 'nonPaged', 'pagefile', 'peakPagefile', 'private')]

def main():
    OUT.mkdir(parents=True)
    kernel = c.WinDLL('kernel32')
    kernel.OpenProcess.argtypes = [w.DWORD, w.BOOL, w.DWORD]
    kernel.OpenProcess.restype = w.HANDLE
    kernel.CloseHandle.argtypes = [w.HANDLE]
    psapi = c.WinDLL('psapi')
    psapi.GetProcessMemoryInfo.argtypes = [w.HANDLE, c.POINTER(Memory), w.DWORD]
    results = []
    for count in (0, 1, 4):
        for mode in ('idle', 'animation', 'scroll', 'modal'):
            run = OUT / f'{count}-{mode}'
            run.mkdir()
            env = os.environ.copy()
            env.update(DOROTI_TESTBED_MODE='webview-workload', DOROTI_WEBVIEW_COUNT=str(count), DOROTI_WEBVIEW_WORKLOAD=mode,
                DOROTI_WINDOWS_APPSDK_SMOKE_MS='10000', DOROTI_PLATFORM_VIEW_EVIDENCE=str(run / 'frame.json'),
                DOROTI_WINDOWS_APPSDK_DIAGNOSTICS='1', DOROTI_WINDOWS_APPSDK_REPORT=str(run / 'report.json'),
                DOROTI_WEBVIEW_USER_DATA=str(run / 'profiles'))
            with (run / 'product.log').open('w', encoding='utf-8') as log:
                process = subprocess.Popen([str(EXE)], cwd=EXE.parent, env=env, stdout=log, stderr=log)
                handle = kernel.OpenProcess(0x410, False, process.pid)
                peak_private = peak_working = 0
                deadline = time.monotonic() + 40
                while process.poll() is None and time.monotonic() < deadline:
                    memory = Memory(); memory.cb = c.sizeof(memory)
                    if handle and psapi.GetProcessMemoryInfo(handle, c.byref(memory), memory.cb):
                        peak_private = max(peak_private, memory.private)
                        peak_working = max(peak_working, memory.working)
                    time.sleep(.2)
                if handle: kernel.CloseHandle(handle)
                if process.poll() is None:
                    subprocess.run(['taskkill', '/PID', str(process.pid), '/T', '/F'], timeout=20, check=False)
                    raise TimeoutError(f'{count}-{mode}')
                assert process.returncode == 0, (count, mode, process.returncode)
            text = (run / 'product.log').read_text(encoding='utf-8')
            summary = json.loads(next(line.split('=', 1)[1] for line in text.splitlines() if line.startswith('doroti.windows.summary=')))
            assert summary['FailedTerminals'] == 0 and summary['OperationalDebugErrors'] == 0
            frame = json.loads((run / 'frame.json').read_text()) if (run / 'frame.json').exists() else None
            if count:
                assert frame['webView']['loadedViews'] == count, (count, mode, frame)
                assert frame['sessionPendingRetirements'] == 0
            results.append({'count': count, 'mode': mode, 'processPeakPrivateBytes': peak_private,
                'processPeakWorkingBytes': peak_working, 'browserProcessMemory': 'notObserved',
                'rendererPresented': summary['RendererPresented'], 'frame': frame,
                'observation': 'raster/readback and UI commit CPU timings; not scanout or input latency'})
            (OUT / 'result.json').write_text(json.dumps({'status': 'observed', 'runs': results}, indent=2), encoding='utf-8')
            print(count, mode, peak_private, frame['timings'] if frame else 'zero-native fast path; no composition timings', flush=True)

if __name__ == '__main__':
    main()
