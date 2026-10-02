"""Verify continued Sample2 rendering across real iOS background/foreground cycles.

Run separately from presentation FPS collection. Components' activity indicator
must continue generating frames after each foreground transition. A few frames
immediately after activation are insufficient evidence of recovery.
"""
import argparse
import hashlib
import json
import subprocess
import time
import uuid
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
BUNDLE = 'dev.doroti.sample2'


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--device', required=True)
    parser.add_argument('--app', type=Path, required=True)
    parser.add_argument('--output', type=Path, required=True)
    parser.add_argument('--cycles', type=int, default=3)
    args = parser.parse_args()
    out = args.output.resolve()
    if out.exists() or not out.is_relative_to(ROOT / 'temp/testing') or args.cycles < 1:
        parser.error('Use a fresh temp/testing directory and positive cycles')
    out.mkdir(parents=True)
    remote = 'ios-frame-loop-lifecycle-' + uuid.uuid4().hex + '.json'
    env = {'DOROTI_VARIABLE_BLUR_PROFILE': '1', 'DOROTI_MAUI_EVIDENCE': remote}
    payload = hashlib.sha256()
    for path in sorted(args.app.resolve().rglob('*')):
        if path.is_file():
            payload.update(str(path.relative_to(args.app.resolve())).encode())
            payload.update(hashlib.sha256(path.read_bytes()).digest())
    summary = {'runtime': 'NativeAOT' if not list(args.app.rglob('*.dll')) else 'Mono',
               'payloadSha256': payload.hexdigest(), 'benchmark': False, 'defaultPath': True,
               'remoteEvidence': 'Documents/' + remote, 'environment': env, 'cycles': [],
               'hostSourceSha256': hashlib.sha256((ROOT / 'Doroti/src/Doroti.Host.Maui/DorotiUIKitGraphiteViewHandler.cs').read_bytes()).hexdigest()}

    def save():
        (out / 'summary.json').write_text(json.dumps(summary, indent=2) + '\n')

    def run(*command, timeout=60):
        result = subprocess.run(command, text=True, capture_output=True, timeout=timeout)
        if result.returncode:
            raise RuntimeError(result.stdout + result.stderr)
        return result

    def launch(bundle, name, extra=()):
        run('xcrun', 'devicectl', 'device', 'process', 'launch', '--device', args.device,
            '--json-output', str(out / (name + '.json')), *extra, bundle)
        return json.loads((out / (name + '.json')).read_text())['result']['process']['processIdentifier']

    def snapshot(name):
        destination = out / (name + '.json')
        # The app publishes asynchronously. Container transport can also close
        # during a read; keep the failed attempt and retry the same snapshot.
        for attempt in range(1, 6):
            try:
                run('xcrun', 'devicectl', 'device', 'copy', 'from', '--device', args.device,
                    '--domain-type', 'appDataContainer', '--domain-identifier', BUNDLE,
                    '--source', 'Documents/' + remote, '--destination', str(destination), timeout=15)
                data = json.loads(destination.read_text())
                break
            except (RuntimeError, json.JSONDecodeError, subprocess.TimeoutExpired) as error:
                (out / f'{name}-copy-{attempt}.log').write_text(str(error))
                if attempt == 5:
                    raise
                time.sleep(.5)
        assert data['frame']['failed'] == data['surface']['commandBuffersErrored'] == 0
        assert data['surface']['iosFrameLoop']['policy'] == 'C'
        assert data['surface']['iosFrameLoop']['maximumPending'] <= 2
        return data['surface']['commandBuffersCompleted'], data['surface']['iosFrameLoopState']

    save()
    try:
        run('xcrun', 'devicectl', 'device', 'install', 'app', '--device', args.device, str(args.app.resolve()))
        pid = launch(BUNDLE, 'initial-launch', ('--terminate-existing', '--environment-variables', json.dumps(env)))
        summary['pid'] = pid
        time.sleep(4)
        first, _ = snapshot('initial-early')
        time.sleep(4)
        second, state = snapshot('initial-late')
        assert second - first >= 60, f'Initial rendering stalled: {first}->{second}; {state}'
        for cycle in range(1, args.cycles + 1):
            launch('com.apple.Preferences', f'{cycle}-background-launch')
            time.sleep(2)
            resumed_pid = launch(BUNDLE, f'{cycle}-foreground-launch')
            assert resumed_pid == pid, 'Foreground started a new process instead of resuming'
            time.sleep(2)
            early, early_state = snapshot(f'{cycle}-foreground-early')
            time.sleep(5)
            late, late_state = snapshot(f'{cycle}-foreground-late')
            assert late - early >= 60, f'Foreground rendering stalled: {early}->{late}; {late_state}'
            summary['cycles'].append({'cycle': cycle, 'samePid': True, 'earlyCompleted': early,
                'lateCompleted': late, 'continuedFrames': late - early, 'earlyState': early_state,
                'lateState': late_state, 'failedFrames': 0, 'terminalErrors': 0})
            save()
            print(f'PASS cycle {cycle}: {early}->{late}; {late_state}', flush=True)
        summary['result'] = 'passed'
        save()
    except Exception as error:
        summary['result'] = 'failed'
        summary['error'] = f'{type(error).__name__}: {error}'
        save()
        raise


if __name__ == '__main__':
    main()
