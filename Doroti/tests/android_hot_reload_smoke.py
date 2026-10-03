"""Real Android metadata updates, retained state, compile recovery and Stop.

Run through eng/run-with-timeout.py (20 minutes). Temporarily edits the Testbed
reload scene, restores it in finally, and uses automated state seeding.
"""
import argparse
import json
import os
from pathlib import Path
import subprocess
import time
import uuid

ROOT = Path(__file__).resolve().parents[2]


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('evidence', type=Path)
    parser.add_argument('--device', required=True)
    args = parser.parse_args()
    evidence = args.evidence.resolve()
    assert evidence.is_relative_to(ROOT / 'Doroti/artifacts')
    evidence.mkdir(parents=True, exist_ok=False)
    source = ROOT / 'samples/DorotiTestbedApp/src/MaterialSample/HotReloadSample.cs'
    original = source.read_bytes()
    text = original.decode('utf-8-sig')
    session_id = uuid.uuid4().hex
    probe = 'reload-state-' + session_id + '.json'
    adb = ['adb', '-s', args.device]
    environment = dict(os.environ, DOROTI_SAMPLE='reload', DOROTI_RELOAD_PROBE=probe)
    log = (evidence / 'watch.log').open('w', encoding='utf-8')
    process = subprocess.Popen(['pwsh', '-NoProfile', '-File', str(ROOT / 'Doroti/eng/doroti.ps1'),
        'dev', '-App', str(ROOT / 'samples/DorotiTestbedApp'), '-Platform', 'android', '-Device', args.device,
        '-SessionId', session_id, '-SessionDirectory', str(evidence)],
        cwd=ROOT, env=environment, stdout=log, stderr=subprocess.STDOUT)

    def read(name):
        try:
            if name == 'state.json':
                raw = subprocess.check_output([*adb, 'exec-out', 'run-as', 'dev.doroti.testbed',
                    'cat', 'files/' + probe], stderr=subprocess.DEVNULL, timeout=5)
                value = json.loads(raw)
                (evidence / name).write_text(json.dumps(value, ensure_ascii=False), encoding='utf-8')
                return value
            return json.loads((evidence / name).read_text(encoding='utf-8'))
        except (OSError, ValueError, subprocess.SubprocessError):
            return {}

    def wait(predicate, description, seconds=90):
        deadline = time.monotonic() + seconds
        while time.monotonic() < deadline:
            if process.poll() is not None:
                raise RuntimeError(f'CLI exited {process.returncode}: {description}; inspect {evidence / "watch.log"}')
            value = predicate()
            if value:
                return value
            time.sleep(.25)
        raise TimeoutError(description)

    def prepare(runtime):
        request = dict(schemaVersion='doroti.dev/v1', sessionId=session_id,
                       runtimeId=runtime['runtimeId'], requestId=str(uuid.uuid4()))
        (evidence / 'request.json').write_text(json.dumps(request), encoding='utf-8')
        wait(lambda: read('prepared.json') == request, 'device pre-save acknowledgment', 15)
        return request['requestId']

    def apply(runtime, message):
        request_id = prepare(runtime)
        source.write_text(text.replace('"Before reload"', json.dumps(message)), encoding='utf-8')
        after = wait(lambda: (v if (v := read('runtime.json')).get('requestId') == request_id and
                     v.get('status') in ('applied', 'failed') else None), 'reassembly frame acknowledgment')
        assert after['status'] == 'applied', after
        assert after['runtimeId'] == runtime['runtimeId'] and after['revision'] > runtime['revision'], after
        state = wait(lambda: (v if (v := read('state.json')).get('message') == message else None), 'updated method result')
        for key in ('stateId', 'count', 'text', 'scroll', 'processId'):
            assert state[key] == before[key], (key, state, before)
        assert state['reassembles'] > before['reassembles'], state
        return after

    try:
        runtime = wait(lambda: (v if (v := read('runtime.json')).get('supported') else None), 'runtime ready', 600)
        before = wait(lambda: (v if (v := read('state.json')).get('scroll') == 160 else None), 'seeded state')
        (evidence / 'before.json').write_text(json.dumps(before, ensure_ascii=False), encoding='utf-8')
        runtime = apply(runtime, 'Android metadata reload passed')
        print('PASS real metadata update; same PID, State, counter, Hangul text and scroll', flush=True)
        source.write_text(text.replace('"Before reload"', 'MISSING_RELOAD_SYMBOL'), encoding='utf-8')
        wait(lambda: 'error CS0103' in (evidence / 'watch.log').read_text(encoding='utf-8'), 'compiler error')
        assert read('runtime.json')['revision'] == runtime['revision']
        runtime = apply(runtime, 'Android compile recovery passed')
        print('PASS compiler-error correction and second metadata update', flush=True)
        (evidence / 'after.json').write_text(json.dumps(read('state.json'), ensure_ascii=False), encoding='utf-8')
        prepare(runtime)
        source.write_text(text.replace('private int _count;', 'private long _count;'), encoding='utf-8')
        wait(lambda: any(marker in (evidence / 'watch.log').read_text(encoding='utf-8').lower()
                         for marker in ('rude edit', 'error enc', 'do you want to restart')), 'rude edit')
        assert read('runtime.json')['revision'] == runtime['revision']
        assert read('state.json')['stateId'] == before['stateId']
        print('PASS unsupported edit retains previous runtime and state', flush=True)
        screenshot = subprocess.check_output([*adb, 'exec-out', 'screencap', '-p'], timeout=15)
        (evidence / 'after.png').write_bytes(screenshot)
        (evidence / 'result.json').write_text(json.dumps(dict(before=before, after=read('state.json'),
            runtime=runtime, device=args.device, compileRecovery=True, rudeEditRetainsState=True),
            ensure_ascii=False, indent=2), encoding='utf-8')
    finally:
        try:
            if process.poll() is None:
                (evidence / 'stop.json').write_text(json.dumps(dict(sessionId=session_id)), encoding='utf-8')
                try:
                    process.wait(timeout=30)
                except subprocess.TimeoutExpired:
                    subprocess.run(['taskkill', '/PID', str(process.pid), '/T', '/F'], check=False)
                    subprocess.run([*adb, 'shell', 'am', 'force-stop', 'dev.doroti.testbed'], check=False)
                    raise
        finally:
            source.write_bytes(original)
            log.close()
    status = read('runtime.json')
    assert process.returncode == 0 and status.get('status') == 'closed' and not status['supported'], status
    assert not subprocess.run([*adb, 'shell', 'pidof', 'dev.doroti.testbed'], capture_output=True, timeout=5).stdout.strip()
    port = (evidence / 'android-hot-reload-port.txt').read_text(encoding='utf-8-sig').strip()
    assert ('tcp:' + port).encode() not in subprocess.check_output([*adb, 'reverse', '--list'], timeout=5)
    print('PASS Stop closes app, watcher, session and owned USB port; source restored', flush=True)


if __name__ == '__main__':
    main()
