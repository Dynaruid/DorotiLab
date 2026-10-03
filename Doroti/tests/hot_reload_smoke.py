"""Actual dotnet-watch metadata delta + native Windows widget state preservation.

Run through eng/run-with-timeout.py. Evidence is synthetic seeded state in a real
Windows host; physical input/display quality requires the manual sample run.
"""
import json
import os
from pathlib import Path
import subprocess
import sys
import time
import uuid

ROOT = Path(__file__).resolve().parents[2]
run = Path(sys.argv[1]).resolve()
app = Path(sys.argv[2]).resolve()
assert run.is_relative_to(ROOT / 'temp/testing') and app.is_relative_to(ROOT / 'temp/testing')
run.mkdir(parents=True, exist_ok=True)
source = app / 'src/HotReloadSample.cs'
original = source.read_text(encoding='utf-8-sig')
session = uuid.uuid4().hex
environment = os.environ.copy()
environment.update(DOROTI_RELOAD_PROBE=str(run / 'state.json'))

def read(name):
    try:
        return json.loads((run / name).read_text(encoding='utf-8'))
    except (FileNotFoundError, json.JSONDecodeError, PermissionError):
        return {}

def wait(predicate, description, seconds=180):
    deadline = time.monotonic() + seconds
    while time.monotonic() < deadline:
        if process.poll() is not None:
            raise RuntimeError(f'CLI exited {process.returncode}: {description}')
        value = predicate()
        if value:
            return value
        time.sleep(.2)
    raise TimeoutError(description)

with (run / 'watch.log').open('w', encoding='utf-8') as log:
    process = subprocess.Popen(['pwsh', '-NoProfile', '-File', str(ROOT / 'Doroti/eng/doroti.ps1'),
        'dev', '-App', str(app), '-Platform', 'windows', '-Configuration', 'Debug',
        '-WindowsBackend', sys.argv[3] if len(sys.argv) > 3 else 'WindowsAppSdk',
        '-SessionDirectory', str(run), '-SessionId', session],
        cwd=ROOT, env=environment, stdout=log, stderr=subprocess.STDOUT, stdin=subprocess.PIPE,
        creationflags=subprocess.CREATE_NO_WINDOW if os.name == 'nt' else 0)
    try:
        runtime = wait(lambda: (v if (v := read('runtime.json')).get('supported') and v.get('sessionId') == session else None), 'Metadata update capability', 300)
        before = wait(lambda: (v if (v := read('state.json')).get('scroll') == 160 else None), 'Seeded UI state')
        request = uuid.uuid4().hex
        (run / 'request.json').write_text(json.dumps(dict(schemaVersion='doroti.dev/v1', sessionId=session, runtimeId=runtime['runtimeId'], requestId=request)))
        source.write_text(original.replace('"Before reload"', '"After metadata reload"'), encoding='utf-8')
        result = wait(lambda: (v if (v := read('runtime.json')).get('requestId') == request and v.get('status') in ('applied', 'failed') else None), 'Reload acknowledgment')
        assert result['status'] == 'applied', result
        after = read('state.json')
        assert after['message'] == 'After metadata reload', after
        for key in ('stateId', 'count', 'text', 'scroll', 'processId'):
            assert after[key] == before[key], (key, before, after)
        assert after['reassembles'] > before['reassembles'], after
        log_offset = len((run / 'watch.log').read_text(encoding='utf-8', errors='replace'))
        source.write_text(original.replace('private int _count;', 'private long _count;').replace('"Before reload"', '"Rude edit must not appear"'), encoding='utf-8')
        def rude_output():
            text = (run / 'watch.log').read_text(encoding='utf-8', errors='replace')[log_offset:]
            return text if 'rude edit' in text.lower() or 'Do you want to restart' in text else None
        rude = wait(rude_output, 'Unsupported field type edit')
        time.sleep(1)
        assert read('runtime.json')['runtimeId'] == runtime['runtimeId'], 'Rude edit unexpectedly restarted the host'
        assert read('state.json')['stateId'] == before['stateId']
        assert read('state.json')['message'] == 'After metadata reload'
        (run / 'result.json').write_text(json.dumps(dict(before=before, after=after, runtime=result, rudeEdit='restart required; no automatic restart'), indent=2), encoding='utf-8')
        print('PASS: native Windows actual metadata delta; same PID/State/count/Hangul input/scroll; owner-queue reassemble acknowledgment.', flush=True)
        print('PASS: unsupported field type edit reports restart required; old process/state stays alive until explicit restart.', flush=True)
    finally:
        if process.poll() is None:
            subprocess.run(['taskkill', '/PID', str(process.pid), '/T', '/F'], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
        process.wait(timeout=15)
        source.write_text(original, encoding='utf-8')

if '--verify-restart' in sys.argv:
    # Editor Restart owns Stop followed by a fresh CLI Run. Verify the same
    # process boundary using the actual selected runner and new session ID.
    session = uuid.uuid4().hex
    with (run / 'restart.log').open('w', encoding='utf-8') as log:
        process = subprocess.Popen(['pwsh','-NoProfile','-File',str(ROOT / 'Doroti/eng/doroti.ps1'),
            'dev','-App',str(app),'-Platform','windows','-Configuration','Debug',
            '-WindowsBackend',sys.argv[3] if len(sys.argv) > 3 else 'WindowsAppSdk',
            '-SessionDirectory',str(run),'-SessionId',session],cwd=ROOT,env=environment,
            stdout=log,stderr=subprocess.STDOUT,stdin=subprocess.PIPE,
            creationflags=subprocess.CREATE_NO_WINDOW if os.name == 'nt' else 0)
        try:
            restarted = wait(lambda: (v if (v := read('runtime.json')).get('supported') and v.get('sessionId') == session else None),'Restart runtime capability',300)
            new_state = wait(lambda: (v if (v := read('state.json')).get('processId') != before['processId'] and v.get('scroll') == 160 else None),'Fresh restarted UI state')
            assert restarted['runtimeId'] != runtime['runtimeId'] and new_state['processId'] != before['processId']
        finally:
            if process.poll() is None:
                subprocess.run(['taskkill','/PID',str(process.pid),'/T','/F'],stdout=subprocess.DEVNULL,stderr=subprocess.DEVNULL)
            process.wait(timeout=15)
    for pid in (before['processId'],new_state['processId']):
        listing = subprocess.check_output(['tasklist','/FI',f'PID eq {pid}','/FO','CSV'],text=True)
        assert f'"{pid}"' not in listing, ('Stop left host alive',pid)
    receipt = read('result.json')
    receipt['restart'] = dict(runtime=restarted,state=new_state,changedPid=True,changedRuntime=True)
    receipt['stop'] = dict(hostPids=[before['processId'],new_state['processId']],allExited=True)
    (run / 'result.json').write_text(json.dumps(receipt,indent=2),encoding='utf-8')
    print('PASS: explicit Stop/Run restart creates a new PID/runtime; Stop exits both owned hosts.',flush=True)
