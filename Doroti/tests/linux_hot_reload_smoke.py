"""Real Linux Qt metadata deltas in an isolated template; never edits user source.

Checks state preservation, compile recovery, rude edits and owned process Stop.
Run under eng/run-with-timeout.py with an interactive Wayland/xcb session.
"""
import argparse
import json
import os
from pathlib import Path
import signal
import subprocess
import time
import uuid

from development_fixture import create

ROOT = Path(__file__).resolve().parents[2]

def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--output', required=True, type=Path)
    parser.add_argument('--app', type=Path, help='Reuse an owned source fixture under temp/testing.')
    parser.add_argument('--qpa', choices=['wayland', 'xcb'], default='wayland' if os.environ.get('WAYLAND_DISPLAY') else 'xcb')
    args = parser.parse_args()
    out = args.output.resolve()
    assert out.is_relative_to(ROOT / 'temp/testing')
    out.mkdir(parents=True, exist_ok=False)
    app = args.app.resolve() if args.app else create(out / '한글 경로/ReloadApp', out / 'template-hive')
    assert app.is_relative_to(ROOT / 'temp/testing')
    source = app / 'src/HotReloadSample.cs'
    original = source.read_bytes()
    text = original.decode('utf-8-sig')
    session_id = uuid.uuid4().hex
    session = out / 'session'
    environment = os.environ | {'QT_QPA_PLATFORM': args.qpa, 'DOROTI_RELOAD_PROBE': str(out / 'state.json')}
    log = (out / 'watch.log').open('w')
    process = subprocess.Popen(['pwsh', '-NoProfile', '-File', str(ROOT / 'Doroti/eng/doroti.ps1'),
        'dev', '-App', str(app), '-Platform', 'linux', '-SessionId', session_id, '-SessionDirectory', str(session)],
        cwd=ROOT, env=environment, stdout=log, stderr=subprocess.STDOUT, stdin=subprocess.PIPE, start_new_session=True)
    report = {'qpa': args.qpa, 'fixture': 'source template / repository references', 'status': 'FAIL'}
    app_pid = None

    def read(path):
        try: return json.loads(path.read_text())
        except (OSError, ValueError): return {}

    def wait(predicate, description, seconds=90):
        deadline = time.monotonic() + seconds
        while time.monotonic() < deadline:
            if process.poll() is not None: raise RuntimeError(f'CLI exit {process.returncode}: {description}')
            value = predicate()
            if value: return value
            time.sleep(.1)
        raise TimeoutError(description)

    def request(runtime):
        request_id = uuid.uuid4().hex
        (session / 'request.json').write_text(json.dumps(dict(schemaVersion='doroti.dev/v1', sessionId=session_id,
            runtimeId=runtime['runtimeId'], requestId=request_id)))
        return request_id

    def apply(runtime, message):
        request_id = request(runtime)
        source.write_text(text.replace('"Before reload"', json.dumps(message)))
        result = wait(lambda: (r if (r := read(session / 'runtime.json')).get('requestId') == request_id and
            r.get('status') in ('applied', 'failed') else None), 'completed reassembly acknowledgment')
        assert result['status'] == 'applied' and result['runtimeId'] == runtime['runtimeId'], result
        assert result['revision'] > runtime['revision'], result
        after = wait(lambda: (s if (s := read(out / 'state.json')).get('message') == message else None), 'updated UI method')
        for key in ['stateId', 'count', 'text', 'scroll', 'processId']:
            assert after[key] == before[key], (key, before, after)
        assert after['reassembles'] > before['reassembles']
        report['after'] = after
        return result

    try:
        runtime = wait(lambda: (r if (r := read(session / 'runtime.json')).get('supported') and r.get('sessionId') == session_id else None), 'metadata capability', 600)
        app_pid = runtime['processId']
        before = wait(lambda: (s if (s := read(out / 'state.json')).get('scroll') == 160 else None), 'seeded widget state')
        report['before'] = before
        runtime = apply(runtime, 'Linux Qt metadata reload passed')
        print('PASS metadata update; same PID, State, count, Hangul text and scroll', flush=True)
        source.write_text(text.replace('"Before reload"', 'MISSING_RELOAD_SYMBOL'))
        wait(lambda: 'error CS0103' in (out / 'watch.log').read_text(), 'compiler error')
        assert read(session / 'runtime.json')['revision'] == runtime['revision']
        runtime = apply(runtime, 'Linux Qt compiler recovery passed')
        print('PASS compiler error recovery and second metadata update', flush=True)
        request(runtime)
        source.write_text(text.replace('private int _count;', 'private long _count;'))
        wait(lambda: any(marker in (out / 'watch.log').read_text().lower() for marker in
            ['rude edit', 'error enc', 'do you want to restart']), 'unsupported field edit')
        assert read(session / 'runtime.json')['runtimeId'] == runtime['runtimeId']
        assert read(session / 'runtime.json')['revision'] == runtime['revision']
        assert read(out / 'state.json')['stateId'] == before['stateId']
        report.update(runtime=runtime, compileRecovery=True, rudeEdit='restart required / previous state retained')
        print('PASS rude edit requests restart and retains the running state', flush=True)
    except Exception as error:
        report['error'] = str(error)
        raise
    finally:
        try:
            if process.poll() is None:
                os.killpg(process.pid, signal.SIGTERM)
                try: process.wait(timeout=30)
                except subprocess.TimeoutExpired:
                    os.killpg(process.pid, signal.SIGKILL)
                    process.wait(timeout=10)
                    report['stopForced'] = True
        finally:
            source.write_bytes(original)
            log.close()
            (out / 'result.json').write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n')
    if app_pid:
        deadline = time.monotonic() + 30
        while True:
            try: os.kill(app_pid, 0)
            except ProcessLookupError: break
            if time.monotonic() >= deadline:
                report['error'] = 'Stop left the Qt application alive'
                (out / 'result.json').write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n')
                raise AssertionError(report['error'])
            time.sleep(.1)
    assert not report.get('stopForced'), report
    report['stop'] = 'PASS / Qt process exited'
    report['status'] = 'PASS'
    (out / 'result.json').write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n')
    print('PASS Stop terminates the owned watcher/app tree; fixture source restored', flush=True)

if __name__ == '__main__': main()
