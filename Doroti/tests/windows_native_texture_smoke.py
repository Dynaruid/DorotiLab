"""Actual Windows MAUI importer with the existing test-only D3D11 GPU producer."""
import json
import os
from pathlib import Path
import re
import subprocess
import sys
import time
ROOT = Path(__file__).resolve().parents[2]
out = Path(sys.argv[1]).resolve()
assert out.is_relative_to(ROOT / 'temp/testing') and not out.exists()
out.mkdir(parents=True)
exe = ROOT / 'samples/DorotiTestbedApp/windows/bin/Debug/net10.0-windows10.0.19041.0/win-x64/DorotiTestbedApp.Windows.exe'
producer = Path(sys.argv[2]).resolve()
report = out / 'result.json'
env = os.environ | dict(DOROTI_SAMPLE='native-texture-probe', DOROTI_DESKTOP_SAMPLE='solid',
    DOROTI_MAUI_TEXTURE_PROBE=str(report), DOROTI_MAUI_EVIDENCE=str(out / 'evidence.json'),
    DOROTI_TEXTURE_TEST_LIBRARY=str(producer), DOROTI_WINDOWS_COMPOSITION_SURFACE='1', DOROTI_WINDOWS_MAUI_GRAPHITE='1')
for name in ('DOROTI_MAUI_CONNECTION_PROBE', 'DOROTI_DESKTOP_PROBE', 'DOROTI_INPUT_PROBE', 'DOROTI_MULTIWINDOW_PROBE'):
    env.pop(name, None)
with (out / 'application.log').open('w', encoding='utf-8') as log:
    process = subprocess.Popen([str(exe)], cwd=ROOT, env=env, stdout=log, stderr=subprocess.STDOUT)
    try:
        deadline = time.monotonic() + 90
        while process.poll() is None:
            for error in (Path(str(report) + '.error'), out / 'evidence.json.exception.txt'):
                if error.exists(): raise RuntimeError(error.read_text(encoding='utf-8')[:5000])
            if time.monotonic() > deadline: raise TimeoutError('Native GPU probe did not close.')
            time.sleep(.1)
        assert process.returncode == 0, process.returncode
    finally:
        if process.poll() is None: process.terminate(); process.wait(timeout=10)
value = json.loads(report.read_text())
assert value['released'] == 12 and value['wrongAdapterRejected'], value
text = (out / 'application.log').read_text(encoding='utf-8', errors='replace')
receipts = re.findall(r'doroti.texture.importer platform=Windows imported=(\d+) retired=(\d+) live=(\d+)', text)
assert receipts and any(int(i) > 0 and int(i) == int(r) and int(l) == 0 for i, r, l in receipts), text[-5000:]
print('PASS: MAUI same-adapter D3D11 -> Vulkan import, wrong-adapter rejection, producer release and GPU consumer retirement; visible pixels not verified.')
