"""Native-window smoke. Requires an interactive Windows GPU runner, not a headless CI service."""
import json
import os
from pathlib import Path
import subprocess
import sys
import time

ROOT = Path(__file__).resolve().parents[2]
run = Path(sys.argv[1]).resolve()
if not run.is_relative_to(ROOT / "temp/testing"):
    raise RuntimeError("Evidence directory must be under temp/testing.")
run.mkdir(parents=True, exist_ok=True)
exe = Path(sys.argv[2]).resolve() if len(sys.argv) > 2 and not sys.argv[2].startswith('--') else ROOT / "samples/DorotiTestbedApp/windowsappsdk/bin/Debug/net10.0-windows10.0.19041.0/win-x64/DorotiTestbedApp.WindowsAppSdk.exe"
def run_application(launch, environment, log, evidence):
    # MAUI captures startup failures in a separate evidence file while a window
    # may remain open. Fail on that evidence instead of waiting twenty minutes.
    environment['DOROTI_MAUI_EVIDENCE'] = str(evidence)
    with subprocess.Popen(launch, cwd=ROOT, env=environment, stdout=log, stderr=subprocess.STDOUT) as process:
        deadline = time.monotonic() + 180
        try:
            while process.poll() is None:
                error = Path(str(evidence) + '.exception.txt')
                if error.exists():
                    raise RuntimeError(error.read_text(encoding='utf-8')[:4000])
                if time.monotonic() >= deadline:
                    raise RuntimeError(f'Native smoke did not complete; see {evidence}')
                time.sleep(.2)
            return process.returncode
        finally:
            if process.poll() is None:
                process.terminate()
                process.wait(timeout=10)
modes = () if "--multi-only" in sys.argv else ("input",) if "--input-only" in sys.argv else ("api", "native") if "--window-only" in sys.argv else ("api", "native", "input")
for mode in modes:
    report = run / f"{mode}.json"
    environment = os.environ.copy()
    for key in ("DOROTI_SAMPLE", "DOROTI_INPUT_PROBE", "DOROTI_DESKTOP_LIFETIME"):
        environment.pop(key, None)
    environment.update(DOROTI_DESKTOP_SAMPLE="solid", DOROTI_DESKTOP_PROBE=str(report),
                       DOROTI_DESKTOP_CLOSE_PROBE="native" if mode == "native" else "api")
    if mode == "input":
        environment.update(DOROTI_SAMPLE="input", DOROTI_INPUT_PROBE=str(run / "input-lifetime.json"))
    with (run / f"{mode}.log").open("w", encoding="utf-8") as log:
        result = subprocess.run([str(exe)], cwd=ROOT, env=environment, stdout=log, stderr=subprocess.STDOUT)
    if result.returncode:
        raise RuntimeError(f"{mode}: application exit {result.returncode}; see {run}")
    log_text = (run / f"{mode}.log").read_text(encoding="utf-8", errors="replace")
    if any(marker in log_text for marker in ("AssertionError", "FlutterError", "Unhandled exception")):
        raise RuntimeError(f"{mode}: framework error in {run / (mode + '.log')}")
    states = json.loads(report.read_text())
    closed = json.loads(Path(str(report) + ".closed").read_text())
    assert states["resize"]["size"] == [500, 650], states
    assert states["maximized"]["presentation"] == "Maximized", states
    assert states["fullscreen"]["presentation"] == "FullScreen", states
    assert states["hidden"]["Visible"] is False, states
    assert states["appearance"] == {"changed": "Applied", "reset": "Applied"}, states
    assert Path(str(report) + ".close").read_text() == "2"
    assert closed["Closed"] and closed["remaining"] == 0, closed
    if mode == "input":
        assert json.loads((run / "input-lifetime.json").read_text())["created"] == (8 if environment.get('DOROTI_MIXED_SCENE_PROBE') == '1' else 4)
        if environment.get('DOROTI_MIXED_SCENE_PROBE') == '1':
            platform = json.loads(Path(environment['DOROTI_PLATFORM_VIEW_EVIDENCE']).read_text())
            assert platform['mixedXamlCommits'] > 0 and platform['sessionPendingRetirements'] == 0, platform
    print(f"{mode}: PASS (native window state/lifetime; physical input and visible resize quality not measured)", flush=True)

for lifetime in (() if '--basic' in sys.argv else ('OnLastWindowClosed', 'Explicit')):
    report = run / (lifetime + '.json')
    environment = os.environ.copy()
    for key in ('DOROTI_DESKTOP_PROBE', 'DOROTI_INPUT_PROBE', 'DOROTI_DESKTOP_CLOSE_PROBE'):
        environment.pop(key, None)
    environment.update(DOROTI_SAMPLE='input', DOROTI_DESKTOP_SAMPLE='solid',
        DOROTI_MULTIWINDOW_PROBE=str(report), DOROTI_DESKTOP_LIFETIME=lifetime,
        DOROTI_PLATFORM_VIEW_EVIDENCE=str(run / (lifetime + '-platform.json')))
    with (run / (lifetime + '.log')).open('w', encoding='utf-8') as log:
        # The apphost supplies the unpackaged WinRT activation manifest used by
        # native editor islands. Launch the same artifact in every smoke mode.
        launch = [str(exe)]
        exit_code = run_application(launch, environment, log, run / (lifetime + '-maui.json'))
    output = (run / (lifetime + '.log')).read_text(encoding='utf-8', errors='replace')
    assert exit_code == 0, output[-5000:]
    assert not any(marker in output for marker in ('AssertionError', 'FlutterError', 'Unhandled exception')), output[-5000:]
    states = json.loads(report.read_text())
    assert states['before']['first'] == [470, 650] and states['before']['second'] == [580, 620], states
    assert states['remaining'] == 0 and states['survivor'] and states['lifetime'] == lifetime, states
    if exe.name != 'DorotiTestbedApp.Windows.exe':
        assert output.count('winui=textbox-created') == 2 and output.count('winui=shutdown-islands=0') == 2, output
    print(f'{lifetime}: PASS (two native HWNDs/editor islands, survivor resize and cleanup; physical input not measured)', flush=True)
