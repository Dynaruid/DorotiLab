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
from android_session_ownership import AndroidSessionOwnership
from android_port_ownership import listener_identity


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


def stop_requested(directory, session_id):
    try:
        value = json.loads((directory / 'stop.json').read_text(encoding='utf-8-sig'))
        return isinstance(value, dict) and value.get('sessionId') == session_id
    except (OSError, ValueError):
        return False


class DeviceSession:
    def __init__(self, directory, session_id, adb, device, package, ownership=None):
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
        self.ownership = ownership
        self.process_start = None
        self.port_identity = None
        self.port = None

    def process_identity(self, pid):
        result = self.shell('exec-out', 'run-as', self.package, 'cat', '/proc/' + str(pid) + '/stat')
        if result.returncode: return None
        fields = result.stdout.rsplit(b')', 1)[-1].split()
        return fields[19] if len(fields) > 19 else None  # field 22, after pid and comm

    def shell(self, *arguments, **kwargs):
        return subprocess.run([*self.adb, *arguments], capture_output=True, timeout=5, **kwargs)

    def close(self, error='Android development connection closed.', status='closed'):
        value = self.runtime or dict(schemaVersion='doroti.dev/v1', sessionId=self.session_id,
                                    runtimeId='android-' + self.session_id, revision=0)
        atomic_json(self.directory / 'runtime.json', {**value, 'supported': False,
                    'status': status, 'error': error})

    def poll(self):
        if self.ownership is not None and not self.ownership.owns(): return
        try:
            # Record the listener once, including startup before a runtime connects.
            # A new listener reusing the same port must never become this owner's port.
            port_file = self.directory / 'android-hot-reload-port.txt'
            if self.ownership is not None and self.port_identity is None and port_file.is_file():
                port = port_file.read_text(encoding='utf-8-sig').strip()
                if port.isdecimal() and 0 < int(port) < 65536:
                    identity = listener_identity(int(port))
                    if identity is not None:
                        self.port = port; self.port_identity = identity
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
            if self.ownership is not None: self.process_start = self.process_identity(pid)
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
            if self.ownership is None or not self.ownership.owns(): return
            try:
                if self.runtime is not None and self.process_start is not None:
                    result = self.shell('exec-out', 'run-as', self.package, 'cat', self.device_directory + '/runtime.json')
                    current = json.loads(result.stdout) if result.returncode == 0 else {}
                    processes = self.shell('shell', 'pidof', self.package)
                    pid = self.runtime['processId']
                    if (current.get('sessionId') == self.session_id and current.get('runtimeId') == self.runtime['runtimeId']
                            and current.get('processId') == pid and str(pid).encode() in processes.stdout.split()
                            and self.process_identity(pid) == self.process_start):
                        self.shell('shell', 'am', 'force-stop', self.package)
            except (OSError, ValueError, subprocess.SubprocessError) as error:
                print(f'Android Stop: {error}', flush=True)
            port_file = self.directory / 'android-hot-reload-port.txt'
            if port_file.exists():
                port = port_file.read_text(encoding='utf-8-sig').strip()
                current_listener = listener_identity(int(port)) if port.isdecimal() and 0 < int(port) < 65536 else None
                if port == self.port and self.port_identity is not None and current_listener in (None, self.port_identity):
                    try:
                        self.shell('reverse', '--remove', 'tcp:' + port)
                    except (OSError, subprocess.SubprocessError) as error:
                        print(f'Android USB cleanup: {error}', flush=True)
        finally:
            try: self.close()
            finally:
                if self.ownership is not None: self.ownership.release()


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--runner', type=Path, required=True)
    parser.add_argument('--app-root', type=Path, required=True)
    parser.add_argument('--session-directory', type=Path, required=True)
    parser.add_argument('--session-id', required=True)
    parser.add_argument('--dotnet', default='dotnet')
    parser.add_argument('--adb')
    parser.add_argument('--cache-root', type=Path)
    parser.add_argument('--lease-directory', type=Path)
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
        adb = args.adb or find_adb()
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
    cache_root = (args.cache_root or Path(__file__).resolve().parents[1] / 'artifacts/ad').resolve()
    lease_directory = args.lease_directory or Path.home() / '.doroti/android-owners'
    ownership = AndroidSessionOwnership(lease_directory,
        device, profile['ApplicationId'], args.session_id)
    session = DeviceSession(args.session_directory, args.session_id, adb, device, profile['ApplicationId'], ownership)
    try:
        directory_ownership = AndroidSessionOwnership(lease_directory, 'session-directory', str(args.session_directory), args.session_id)
    except Exception:
        ownership.release()
        raise
    session.close('Waiting for Android SDK build and runtime connection.', 'starting')
    stop = threading.Event()
    for sig in (signal.SIGINT, signal.SIGTERM):
        signal.signal(sig, lambda *_: stop.set())
    # aapt2 still rejects long Windows resource paths. Use a short owned cache,
    # keyed by project/toolchain/RID, separate from ordinary build/publish output.
    cache_key = hashlib.sha256((str(args.runner) + profile['NETCoreSdkVersion'] +
                               profile['RuntimeIdentifier'] + profile['TargetFramework']).encode()).hexdigest()[:12]
    artifacts = cache_root / cache_key
    command = [args.dotnet, 'watch', '--project', str(args.runner), '--device', device,
               'run', '--configuration', 'Debug', '--no-launch-profile',
               '--property:DorotiAndroidDevelopment=true', '--property:RuntimeIdentifier=' + profile['RuntimeIdentifier'],
               '--property:ArtifactsPath=' + str(artifacts)]
    child = None
    try:
        child = subprocess.Popen(command, cwd=args.app_root, env=environment, start_new_session=os.name != 'nt')
        while child.poll() is None and not stop.wait(.3):
            if stop_requested(args.session_directory, args.session_id):
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
        try: session.stop()
        finally: directory_ownership.release()


if __name__ == '__main__':
    raise SystemExit(main())
