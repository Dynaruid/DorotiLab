"""Debug Android restart/deploy loop. No unqualified device metadata transport is advertised."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import signal
import subprocess
import time

def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--runner', type=Path, required=True)
    parser.add_argument('--app-root', type=Path, required=True)
    parser.add_argument('--session-directory', type=Path, required=True)
    parser.add_argument('--session-id', required=True)
    parser.add_argument('--dotnet', default='dotnet')
    parser.add_argument('--device')
    parser.add_argument('--rid')
    parser.add_argument('--inspect', action='store_true', help='Evaluate runtime without deploying.')
    args = parser.parse_args()
    properties = ['-p:Configuration=Debug'] + (['-p:RuntimeIdentifier=' + args.rid] if args.rid else [])
    evaluated = subprocess.check_output([args.dotnet, 'msbuild', str(args.runner),
        '-getProperty:TargetFramework,RuntimeIdentifier,UseMonoRuntime,PublishAot,RunAOTCompilation,ApplicationId',
        *properties], cwd=args.app_root, text=True)
    profile = json.loads(evaluated)['Properties']
    if 'android' not in profile['TargetFramework']:
        raise RuntimeError('Android development requires an Android runner.')
    if profile['PublishAot'].lower() == 'true' or profile['RunAOTCompilation'].lower() == 'true':
        raise RuntimeError('Android dev requires Debug JIT without AOT. Disable AOT in the selected profile.')
    reason = 'Device metadata update transport is not qualified for this runtime; source changes rebuild/redeploy and reset state.'
    runtime = dict(schemaVersion='doroti.dev/v1', sessionId=args.session_id,
        runtimeId='android-restart-' + args.session_id, processId=os.getpid(), supported=False,
        status='restart-only', revision=0, mode='restart', profile=profile, error=reason)
    args.session_directory.mkdir(parents=True, exist_ok=True)
    status = args.session_directory / 'runtime.json'
    def publish(state, error=reason):
        runtime.update(status=state, error=error, revision=runtime['revision'] + 1)
        temporary = status.with_suffix('.tmp')
        temporary.write_text(json.dumps(runtime, indent=2), encoding='utf-8')
        temporary.replace(status)
    publish('restart-only')
    print(json.dumps(runtime, indent=2), flush=True)
    if args.inspect: return
    stop = False
    def stopped(*_):
        nonlocal stop
        stop = True
    for sig in (signal.SIGINT, signal.SIGTERM): signal.signal(sig, stopped)
    def source_identity():
        digest = hashlib.sha256()
        for directory, dirs, files in os.walk(args.app_root):
            dirs[:] = sorted(d for d in dirs if d not in ('bin', 'obj', 'build', '.gradle', '.doroti', '.git', 'node_modules'))
            for name in sorted(files):
                if Path(name).suffix not in ('.cs', '.csproj', '.props', '.targets', '.xml', '.json'): continue
                path = Path(directory) / name
                digest.update(str(path.relative_to(args.app_root)).encode())
                digest.update(path.read_bytes())
        return digest.digest()
    def force_stop():
        # Stop only this declared app on the explicitly selected device.
        package = profile.get('ApplicationId')
        if package:
            subprocess.run(['adb', *(['-s', args.device] if args.device else []), 'shell', 'am', 'force-stop', package], check=False)
    identity = None
    try:
        while not stop:
            current = source_identity()
            if current != identity:
                identity = current
                publish('restarting')
                force_stop()
                command = [args.dotnet, 'run', '--project', str(args.runner), '--configuration', 'Debug', *properties]
                if args.device: command += ['--device', args.device]
                child = subprocess.Popen(command, cwd=args.app_root)
                changed = False
                while child.poll() is None and not stop:
                    time.sleep(.4)
                    if source_identity() != identity: changed = True; break
                if child.poll() is None:
                    if os.name == 'nt': subprocess.run(['taskkill', '/PID', str(child.pid), '/T', '/F'], check=False, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
                    else: child.terminate()
                code = child.wait()
                if changed: continue
                publish('restart-only' if code == 0 else 'failed', reason if code == 0 else f'Android deploy failed ({code}); edit source or restart to retry.')
            time.sleep(.4)
    finally:
        force_stop()
        publish('closed')

if __name__ == '__main__': main()
