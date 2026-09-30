"""Real iOS metadata updates, state preservation, compile recovery and Stop.

Run via eng/run-with-timeout.py. Uses the Testbed reload scene and temporarily
edits its Message method; the original source is restored in finally.
"""
import argparse
import json
import os
from pathlib import Path
import signal
import subprocess
import time
import uuid

ROOT = Path(__file__).resolve().parents[2]


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('evidence', type=Path)
    parser.add_argument('--device', required=True)
    parser.add_argument('--framework', default='net10.0-ios27.0')
    parser.add_argument('--sdk-version')
    parser.add_argument('--dotnet', default='dotnet')
    parser.add_argument('--rid', default='iossimulator-arm64', choices=['ios-arm64', 'iossimulator-arm64', 'iossimulator-x64'])
    args = parser.parse_args()
    evidence = args.evidence.resolve()
    assert evidence.is_relative_to(ROOT / 'temp/testing')
    evidence.mkdir(parents=True, exist_ok=False)
    source = ROOT / 'samples/DorotiTestbedApp/src/MaterialSample/HotReloadSample.cs'
    original = source.read_bytes()
    text = original.decode('utf-8-sig')
    session_id = uuid.uuid4().hex
    physical = args.rid == 'ios-arm64'
    device_probe = f'reload-state-{session_id}.json'
    environment = dict(os.environ, DOROTI_SAMPLE='reload',
        DOROTI_RELOAD_PROBE=device_probe if physical else str(evidence / 'state.json'))
    log = (evidence / 'watch.log').open('w')
    process = subprocess.Popen(['pwsh', '-NoProfile', '-File', str(ROOT / 'Doroti/eng/doroti.ps1'),
        'dev', '-App', str(ROOT / 'samples/DorotiTestbedApp'), '-Platform', 'ios', '-Device', args.device,
        '-IosTargetFramework', args.framework, '-Rid', args.rid, '-SessionId', session_id, '-SessionDirectory', str(evidence), '-DotnetPath', args.dotnet] + (['-IosSdkVersion', args.sdk_version] if args.sdk_version else []),
        cwd=ROOT, env=environment, stdout=log, stderr=subprocess.STDOUT, start_new_session=True)

    def read(name):
        if physical and name == 'state.json':
            subprocess.run(['xcrun', 'devicectl', 'device', 'copy', 'from', '--device', args.device,
                '--domain-type', 'appDataContainer', '--domain-identifier', 'dev.doroti.testbed',
                '--source', 'Documents/' + device_probe, '--destination', str(evidence / name)],
                stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL, timeout=15, check=False)
        try:
            return json.loads((evidence / name).read_text())
        except (OSError, ValueError):
            return {}

    def wait(predicate, description, seconds=90):
        deadline = time.monotonic() + seconds
        while time.monotonic() < deadline:
            if process.poll() is not None:
                raise RuntimeError(f'CLI exited {process.returncode}: {description}')
            value = predicate()
            if value:
                return value
            time.sleep(.1)
        raise TimeoutError(description)

    def prepare(runtime):
        request_id = str(uuid.uuid4())
        request = dict(schemaVersion='doroti.dev/v1', sessionId=session_id,
                       runtimeId=runtime['runtimeId'], requestId=request_id)
        (evidence / 'request.json').write_text(json.dumps(request))
        wait(lambda: read('prepared.json') == request, 'runtime pre-save acknowledgment', 10)
        return request_id

    def apply(runtime, message):
        request_id = prepare(runtime)
        source.write_text(text.replace('"Before reload"', json.dumps(message)))
        after = wait(lambda: (v if (v := read('runtime.json')).get('requestId') == request_id and
                     v.get('status') in ('applied', 'failed') else None), 'successful frame acknowledgment')
        assert after['status'] == 'applied', after
        assert after['runtimeId'] == runtime['runtimeId'], after
        state = wait(lambda: (v if (v := read('state.json')).get('message') == message else None), 'updated method result')
        for key in ('stateId', 'count', 'text', 'scroll', 'processId'):
            assert state[key] == before[key], (key, state, before)
        assert state['reassembles'] > before['reassembles'], state
        return after

    try:
        runtime = wait(lambda: (v if (v := read('runtime.json')).get('supported') else None), 'runtime ready', 600)
        before = wait(lambda: (v if (v := read('state.json')).get('scroll') == 160 else None), 'seeded state')
        (evidence / 'before.json').write_text(json.dumps(before))
        runtime = apply(runtime, 'iOS metadata reload passed')
        print('PASS actual metadata update; same PID, State, count, Hangul text and scroll', flush=True)
        source.write_text(text.replace('"Before reload"', 'MISSING_RELOAD_SYMBOL'))
        wait(lambda: 'error CS0103' in (evidence / 'watch.log').read_text(), 'compiler error')
        assert read('runtime.json')['revision'] == runtime['revision']
        runtime = apply(runtime, 'iOS compile recovery passed')
        print('PASS compiler-error correction and second metadata update', flush=True)
        (evidence / 'after.json').write_text(json.dumps(read('state.json')))
        request_id = prepare(runtime)
        source.write_text(text.replace('private int _count;', 'private long _count;'))
        wait(lambda: any(marker in (evidence / 'watch.log').read_text().lower()
                         for marker in ('rude edit', 'error enc', 'do you want to restart')), 'rude edit')
        assert read('runtime.json')['revision'] == runtime['revision']
        assert read('state.json')['stateId'] == before['stateId']
        print('PASS unsupported edit retains previous runtime and state', flush=True)
        if not physical:
            subprocess.run(['xcrun', 'simctl', 'io', args.device, 'screenshot', str(evidence / 'after.png')], check=True)
    finally:
        try:
            if process.poll() is None:
                os.killpg(process.pid, signal.SIGTERM)
                process.wait(timeout=30)
            deadline = time.monotonic() + 30
            while read('runtime.json') and read('runtime.json').get('status') != 'closed' and time.monotonic() < deadline:
                time.sleep(.1)
        finally:
            source.write_bytes(original)
            log.close()
    status = read('runtime.json')
    assert status.get('status') == 'closed' and not status['supported'], status
    print('PASS Stop closes session; source restored', flush=True)


if __name__ == '__main__':
    main()
