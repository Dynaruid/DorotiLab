"""Own Android SDK metadata Hot Reload and relay device frame acknowledgments.

The SDK packages/applies deltas over its authenticated USB WebSocket transport.
ADB run-as exchanges session JSON only; it never applies code or restarts on save.
"""
import argparse
import hashlib
import json
import os
from pathlib import Path
import re
import shlex
import shutil
import signal
import subprocess
import threading


def atomic_json(path, value):
    temporary = path.with_suffix('.tmp')
    temporary.write_text(json.dumps(value, indent=2), encoding='utf-8')
    temporary.replace(path)


def find_adb():
    executable = shutil.which('adb')
    if executable:
        return executable
    for root in (os.environ.get('ANDROID_SDK_ROOT'), os.environ.get('ANDROID_HOME'),
                 str(Path(os.environ.get('LOCALAPPDATA', '~')) / 'Android/Sdk')):
        if root:
            candidate = Path(root) / 'platform-tools' / ('adb.exe' if os.name == 'nt' else 'adb')
            if candidate.is_file():
                return str(candidate)
    raise RuntimeError('Install Android platform-tools and put adb on PATH or set ANDROID_SDK_ROOT.')


def select_device(adb, requested):
    output = subprocess.check_output([adb, 'devices'], text=True, timeout=15)
    devices = dict(line.split()[:2] for line in output.splitlines()[1:] if len(line.split()) >= 2)
    if requested:
        if devices.get(requested) != 'device':
            raise ValueError(f'ADB device {requested} is unavailable or unauthorized.')
        return requested
    ready = [device for device, state in devices.items() if state == 'device']
    if len(ready) != 1:
        raise ValueError('Connect one authorized Android device/emulator or specify -Device <serial>.')
    return ready[0]


class DeviceSession:
    def __init__(self, directory, session_id, adb, device, package):
        if not re.fullmatch(r'[A-Za-z0-9-]{1,80}', session_id):
            raise ValueError('Invalid development session ID.')
        if not re.fullmatch(r'[A-Za-z][A-Za-z0-9_]*(\.[A-Za-z][A-Za-z0-9_]*)+', package):
            raise ValueError('The Android runner must declare a valid ApplicationId.')
        self.directory = directory
        self.session_id = session_id
        self.adb = [adb, '-s', device]
        self.package = package
        self.device_directory = 'files/Doroti.Dev/' + session_id
        self.runtime = None
        self.delivered = None

    def shell(self, *arguments, **kwargs):
        return subprocess.run([*self.adb, *arguments], capture_output=True, timeout=5, **kwargs)

    def close(self, error='Android development connection closed.', status='closed'):
        value = self.runtime or dict(schemaVersion='doroti.dev/v1', sessionId=self.session_id,
                                    runtimeId='android-' + self.session_id, revision=0)
        atomic_json(self.directory / 'runtime.json', {**value, 'supported': False,
                    'status': status, 'error': error})

    def poll(self):
        try:
            result = self.shell('exec-out', 'run-as', self.package, 'cat', self.device_directory + '/runtime.json')
            if result.returncode:
                if self.runtime:
                    self.close()
                return
            runtime = json.loads(result.stdout)
            if not isinstance(runtime, dict):
                raise ValueError('Invalid Android runtime status.')
            pid = runtime.get('processId')
            if (runtime.get('schemaVersion') != 'doroti.dev/v1' or
                    runtime.get('sessionId') != self.session_id or
                    not runtime.get('runtimeId') or not isinstance(pid, int) or pid <= 0):
                raise ValueError('Invalid Android runtime session.')
            processes = self.shell('shell', 'pidof', self.package)
            if str(pid).encode() not in processes.stdout.split():
                self.close()
                return
            self.runtime = runtime
            atomic_json(self.directory / 'runtime.json', runtime)
            try:
                request = json.loads((self.directory / 'request.json').read_text(encoding='utf-8'))
            except (OSError, ValueError):
                return
            if not isinstance(request, dict):
                return
            if (request.get('schemaVersion') != 'doroti.dev/v1' or
                    request.get('sessionId') != self.session_id or
                    request.get('runtimeId') != runtime['runtimeId'] or
                    not isinstance(request.get('requestId'), str) or not request['requestId']):
                return
            if self.delivered != request:
                # Atomic promotion completes before the editor saves C# edits.
                target = self.device_directory + '/request.json'
                command = f'cat > {shlex.quote(target + ".tmp")} && mv {shlex.quote(target + ".tmp")} {shlex.quote(target)}'
                result = self.shell('shell', f'run-as {self.package} sh -c {shlex.quote(command)}',
                                    input=json.dumps(request).encode())
                if result.returncode:
                    raise OSError('Unable to deliver Android reload request.')
                self.delivered = request
                atomic_json(self.directory / 'prepared.json', request)
        except (OSError, ValueError, subprocess.SubprocessError):
            if self.runtime:
                self.close()

    def stop(self):
        try:
            try:
                self.shell('shell', 'am', 'force-stop', self.package)
            except (OSError, subprocess.SubprocessError) as error:
                print(f'Android Stop: {error}', flush=True)
            port_file = self.directory / 'android-hot-reload-port.txt'
            if port_file.exists():
                port = port_file.read_text(encoding='utf-8-sig').strip()
                if port.isdecimal() and 0 < int(port) < 65536:
                    try:
                        self.shell('reverse', '--remove', 'tcp:' + port)
                    except (OSError, subprocess.SubprocessError) as error:
                        print(f'Android USB cleanup: {error}', flush=True)
        finally:
            self.close()


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--runner', type=Path, required=True)
    parser.add_argument('--app-root', type=Path, required=True)
    parser.add_argument('--session-directory', type=Path, required=True)
    parser.add_argument('--session-id', required=True)
    parser.add_argument('--dotnet', default='dotnet')
    parser.add_argument('--device')
    parser.add_argument('--rid', choices=['android-arm64', 'android-x64'])
    parser.add_argument('--inspect', action='store_true', help='Evaluate the development profile without deploying.')
    args = parser.parse_args()
    if not re.fullmatch(r'[A-Za-z0-9-]{1,80}', args.session_id):
        raise ValueError('Invalid development session ID.')
    args.runner = args.runner.resolve()
    args.app_root = args.app_root.resolve()
    args.session_directory = args.session_directory.resolve()
    args.session_directory.mkdir(parents=True, exist_ok=True)
    environment = dict(os.environ, DOROTI_DEV_SESSION=str(args.session_directory),
        DOROTI_DEV_SESSION_ID=args.session_id, DOROTI_DEV_DEVICE_SESSION='Doroti.Dev/' + args.session_id,
        DOTNET_CLI_UI_LANGUAGE='en', DOTNET_WATCH_SUPPRESS_EMOJIS='1', DOTNET_WATCH_RESTART_ON_RUDE_EDIT='false')
    adb = device = None
    rid = args.rid
    if not args.inspect:
        adb = find_adb()
        device = select_device(adb, args.device)
        abi = subprocess.check_output([adb, '-s', device, 'shell', 'getprop', 'ro.product.cpu.abi'], text=True, timeout=15).strip()
        device_rid = {'arm64-v8a': 'android-arm64', 'x86_64': 'android-x64'}.get(abi)
        if not device_rid or rid and rid != device_rid:
            raise ValueError(f'Unsupported or mismatched Android ABI: {abi}, requested RID: {rid}.')
        rid = device_rid
    properties = ['-p:Configuration=Debug', '-p:DorotiAndroidDevelopment=true']
    if rid:
        properties.append('-p:RuntimeIdentifier=' + rid)
    evaluated = json.loads(subprocess.check_output([args.dotnet, 'msbuild', str(args.runner),
        '-getProperty:TargetFramework,RuntimeIdentifier,UseMonoRuntime,PublishAot,RunAOTCompilation,PublishTrimmed,Optimize,StartupHookSupport,ApplicationId,NETCoreSdkVersion',
        '-getItem:ProjectCapability', *properties], cwd=args.app_root, env=environment, text=True, timeout=30))
    profile = evaluated['Properties']
    capabilities = [item['Identity'] for item in evaluated['Items']['ProjectCapability']]
    if ('android' not in profile['TargetFramework'] or 'HotReloadWebSockets' not in capabilities or
            tuple(int(n) for n in profile['NETCoreSdkVersion'].split('-')[0].split('.')) < (10, 0, 400)):
        raise ValueError('Android Hot Reload requires .NET SDK 10.0.400+ and a workload with HotReloadWebSockets.')
    if (profile['UseMonoRuntime'].lower() != 'true' or profile['StartupHookSupport'].lower() != 'true' or
            any(profile[name].lower() == 'true' for name in ('PublishAot', 'RunAOTCompilation', 'PublishTrimmed', 'Optimize'))):
        raise ValueError('Android Hot Reload requires unoptimized, untrimmed Debug Mono without AOT.')
    print(json.dumps(dict(mode='metadata', device=device, profile=profile), indent=2), flush=True)
    if args.inspect:
        return 0
    session = DeviceSession(args.session_directory, args.session_id, adb, device, profile['ApplicationId'])
    session.close('Waiting for Android SDK build and runtime connection.', 'starting')
    stop = threading.Event()
    for sig in (signal.SIGINT, signal.SIGTERM):
        signal.signal(sig, lambda *_: stop.set())
    # aapt2 still rejects long Windows resource paths. Use a short owned cache,
    # keyed by project/toolchain/RID, separate from ordinary build/publish output.
    cache_key = hashlib.sha256((str(args.runner) + profile['NETCoreSdkVersion'] +
                               profile['RuntimeIdentifier'] + profile['TargetFramework']).encode()).hexdigest()[:12]
    artifacts = Path(__file__).resolve().parents[1] / 'artifacts/ad' / cache_key
    command = [args.dotnet, 'watch', '--project', str(args.runner), '--device', device,
               'run', '--configuration', 'Debug', '--no-launch-profile',
               '--property:DorotiAndroidDevelopment=true', '--property:RuntimeIdentifier=' + profile['RuntimeIdentifier'],
               '--property:ArtifactsPath=' + str(artifacts)]
    child = None
    try:
        child = subprocess.Popen(command, cwd=args.app_root, env=environment, start_new_session=os.name != 'nt')
        while child.poll() is None and not stop.wait(.3):
            if (args.session_directory / 'stop.json').exists():
                break
            session.poll()
        return child.returncode if child.poll() is not None else 0
    finally:
        if child and child.poll() is None:
            if os.name == 'nt':
                subprocess.run(['taskkill', '/PID', str(child.pid), '/T', '/F'], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL, check=False)
            else:
                os.killpg(child.pid, signal.SIGINT)
            try:
                child.wait(timeout=8)
            except subprocess.TimeoutExpired:
                if os.name == 'nt':
                    child.kill()
                else:
                    os.killpg(child.pid, signal.SIGKILL)
                child.wait()
        session.stop()


if __name__ == '__main__':
    raise SystemExit(main())
