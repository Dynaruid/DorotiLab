"""Public UIKit scene control/API-close probe. Run through run-with-timeout.py."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import subprocess
import time

parser = argparse.ArgumentParser()
parser.add_argument('--app', type=Path, required=True)
parser.add_argument('--output', type=Path, required=True)
parser.add_argument('--native-close', action='store_true')
args = parser.parse_args()
args.output.mkdir(parents=True, exist_ok=False)
output = args.output.resolve()
exe = args.app.resolve() / 'Contents/MacOS/DorotiTestbedApp.MacCatalyst'
state = output / 'states.json'
identity = dict(app=str(args.app.resolve()), executableSha256=hashlib.sha256(exe.read_bytes()).hexdigest(),
    nativeClose=args.native_close,
    head=subprocess.check_output(['git', 'rev-parse', 'HEAD'], text=True, timeout=30).strip())
(output / 'run-info.json').write_text(json.dumps(identity, indent=2))
env = dict(os.environ, DOROTI_CATALYST_DESKTOP_PROBE=str(state),
    DOROTI_CATALYST_NATIVE_CLOSE='1' if args.native_close else '0',
    DOROTI_MAUI_EVIDENCE=str(output / 'render.json'))
with (output / 'app.log').open('w') as log:
    process = subprocess.Popen([str(exe)], env=env, stdout=log, stderr=subprocess.STDOUT)
    try:
        deadline = time.monotonic() + 120
        closed = Path(str(state) + '.closed')
        while not closed.exists() and time.monotonic() < deadline:
            errors = list(output.glob('*.error')) + list(output.glob('*exception*'))
            if errors:
                raise RuntimeError('\n'.join(p.read_text() for p in errors))
            if process.poll() is not None:
                raise RuntimeError(f'App exited before closed evidence: {process.returncode}')
            time.sleep(.1)
        assert closed.exists(), 'Scene did not close within 120 seconds'
        native = json.loads(Path(str(state) + '.native.json').read_text())
        result = json.loads(closed.read_text())
        assert native['result'] == 'PASS' and result['Closed'] and result['remaining'] == 0
        assert result['closingCallbacks'] == (0 if args.native_close else 2), result
        states = json.loads(state.read_text())
        assert all(value > 0 for value in states['initial']['size']), states['initial']
        assert all(abs(a-b) < 1 for a,b in zip(states['intermediate']['size'], [550,475])), states['intermediate']
        assert all(abs(a-b) < 1 for a,b in zip(states['resized']['size'], [500,450])), states['resized']
        assert all(abs(a-b) < 1 for a,b in zip(states['fixedResize']['size'], [520,460])), states['fixedResize']
        time.sleep(.5)
        summary = dict(status='PASS', identity=identity, native=native, closed=result, initialObserved=states['initial'],
            processAfterSceneClose='running' if process.poll() is None else process.returncode,
            limitation='Native close bypasses managed cancellation; UIKit owns process lifetime.')
        (output / 'result.json').write_text(json.dumps(summary, indent=2))
        print(json.dumps(summary, indent=2))
    finally:
        # Explicit policy does not request process exit. Stop only this fixture,
        # after closed evidence, without labeling SIGTERM as a graceful app quit.
        if process.poll() is None:
            process.terminate()
            try: process.wait(timeout=5)
            except subprocess.TimeoutExpired:
                process.kill()
                process.wait(timeout=5)
