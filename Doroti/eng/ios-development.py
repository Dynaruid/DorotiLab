"""Own an iOS dotnet-watch session and relay frame acknowledgments to the editor.

Code/deltas belong to the SDK's Hot Reload transport. Simulator HTTP and device
file relays only exchange doroti.dev/v1 status and pre-save request correlation.
"""
import argparse
import hmac
import ipaddress
import json
import os
import re
from pathlib import Path
import secrets
import shutil
import signal
import socket
import socketserver
import select
import subprocess
import threading
import time
from urllib.parse import unquote
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer


def network_relay(host, port):
    """Forward opaque SDK WebSocket bytes; authentication stays in the SDK."""
    class Handler(socketserver.BaseRequestHandler):
        def handle(self):
            self.request.settimeout(5)
            try:
                with socket.create_connection(('127.0.0.1', port), timeout=5) as upstream:
                    peers = [self.request, upstream]
                    while not self.server.stopping.is_set():
                        readable, _, _ = select.select(peers, [], [], 1)
                        for source in readable:
                            data = source.recv(65536)
                            if not data:
                                return
                            peers[1 - peers.index(source)].sendall(data)
            except OSError:
                return

    class Server(socketserver.ThreadingTCPServer):
        daemon_threads = True

        def __init__(self, *args):
            self.stopping = threading.Event()
            super().__init__(*args)

        def shutdown(self):
            self.stopping.set()
            super().shutdown()

    return Server((host, 0), Handler)


def local_network_address():
    # UDP connect selects the default route without sending a packet.
    with socket.socket(socket.AF_INET, socket.SOCK_DGRAM) as probe:
        probe.connect(('192.0.2.1', 9))
        return probe.getsockname()[0]


def atomic_json(path, value):
    temporary = path.with_suffix('.tmp')
    temporary.write_text(json.dumps(value), encoding='utf-8')
    temporary.replace(path)


class Session:
    def __init__(self, directory, session_id):
        self.directory = Path(directory)
        self.directory.mkdir(parents=True, exist_ok=True)
        self.session_id = session_id
        self.token = secrets.token_urlsafe(32)
        self.lock = threading.Lock()
        self.runtime = None
        self.last_seen = time.monotonic()

    def exchange(self, runtime, prepared):
        if (runtime.get('schemaVersion') != 'doroti.dev/v1' or
                runtime.get('sessionId') != self.session_id or not runtime.get('runtimeId')):
            raise ValueError('Invalid runtime session')
        with self.lock:
            self.runtime = runtime
            self.last_seen = time.monotonic()
            atomic_json(self.directory / 'runtime.json', runtime)
            try:
                request = json.loads((self.directory / 'request.json').read_text(encoding='utf-8'))
            except (OSError, ValueError):
                return {}
            if (request.get('sessionId') != self.session_id or
                    request.get('runtimeId') != runtime['runtimeId'] or not request.get('requestId')):
                return {}
            if prepared == request['requestId']:
                atomic_json(self.directory / 'prepared.json', request)
            return request

    def expire(self, force=False):
        with self.lock:
            if self.runtime and (force or time.monotonic() - self.last_seen > 5):
                atomic_json(self.directory / 'runtime.json', {
                    **self.runtime, 'supported': False, 'status': 'closed',
                    'error': 'iOS development connection closed.'})


def make_server(session):
    class Handler(BaseHTTPRequestHandler):
        def do_POST(self):
            if (self.path != '/session' or not hmac.compare_digest(
                    self.headers.get('Authorization', ''), 'Bearer ' + session.token)):
                self.send_error(403)
                return
            try:
                length = int(self.headers.get('Content-Length', '0'))
                if not 0 < length <= 65536:
                    raise ValueError('Invalid status size')
                runtime = json.loads(self.rfile.read(length))
                result = session.exchange(runtime, self.headers.get('X-Doroti-Prepared'))
            except (ValueError, TypeError, AttributeError):
                self.send_error(400)
                return
            body = json.dumps(result).encode()
            self.send_response(200)
            self.send_header('Content-Type', 'application/json')
            self.send_header('Content-Length', str(len(body)))
            self.end_headers()
            self.wfile.write(body)

        def setup(self):
            super().setup()
            self.connection.settimeout(5)

        def log_message(self, *_):
            pass

    return ThreadingHTTPServer(('127.0.0.1', 0), Handler)


def stop_device_app(device, bundle_id):
    def info(kind, search):
        raw = subprocess.check_output(['xcrun', 'devicectl', 'device', 'info', kind,
            '--device', device, '--search', search, '--json-output', '-'],
            stderr=subprocess.DEVNULL, text=True, timeout=15)
        return json.loads(raw)['result']
    for app in info('apps', bundle_id)['apps']:
        if app.get('bundleIdentifier') != bundle_id:
            continue
        url = app['url'].rstrip('/') + '/'
        for process in info('processes', unquote(url.rstrip('/').rsplit('/', 1)[-1]))['runningProcesses']:
            if process.get('executable', '').startswith(url):
                subprocess.run(['xcrun', 'devicectl', 'device', 'process', 'signal',
                    '--device', device, '--pid', str(process['processIdentifier']), '--signal', 'SIGTERM'],
                    stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL, timeout=15)


class DeviceSession(Session):
    """Relay status through CoreDevice; SDK deltas use a separate connection."""
    def __init__(self, directory, session_id, device):
        super().__init__(directory, session_id)
        self.device = device
        self.bundle_id = None
        self.device_directory = 'Documents/Doroti.Dev/' + session_id
        self.delivered = None

    def copy(self, direction, source, destination):
        result = subprocess.run(['xcrun', 'devicectl', 'device', 'copy', direction,
            '--device', self.device, '--domain-type', 'appDataContainer', '--domain-identifier', self.bundle_id,
            '--source', str(source), '--destination', str(destination)],
            stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL, timeout=5)
        return result.returncode == 0

    def poll(self):
        target = self.directory / 'device-runtime.json'
        try:
            if not self.copy('from', self.device_directory + '/runtime.json', target):
                self.expire(force=True)
                return
            runtime = json.loads(target.read_text(encoding='utf-8'))
            pid = runtime.get('processId')
            if not isinstance(pid, int) or pid <= 0:
                raise ValueError('Invalid device PID')
            raw = subprocess.check_output(['xcrun', 'devicectl', 'device', 'info', 'processes',
                '--device', self.device, '--filter', 'processIdentifier = ' + str(pid), '--json-output', '-'],
                stderr=subprocess.DEVNULL, text=True, timeout=5)
            if not json.loads(raw)['result']['runningProcesses']:
                self.expire(force=True)
                return
            request = self.exchange(runtime, None)
            if request and self.delivered != request:
                # The metadata callback reads request.json. Copy it completely
                # before acknowledging to the editor that saving may proceed.
                pending = self.directory / 'device-request.json'
                atomic_json(pending, request)
                if self.copy('to', pending, self.device_directory + '/request.json'):
                    self.delivered = request
                    atomic_json(self.directory / 'prepared.json', request)
        except (OSError, ValueError, subprocess.SubprocessError):
            self.expire(force=True)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--runner', required=True)
    parser.add_argument('--app-root', required=True)
    parser.add_argument('--session-directory', required=True)
    parser.add_argument('--session-id', required=True)
    parser.add_argument('--rid', required=True, choices=['ios-arm64', 'iossimulator-arm64', 'iossimulator-x64'])
    parser.add_argument('--device', required=True)
    parser.add_argument('--framework')
    parser.add_argument('--sdk-version')
    parser.add_argument('--dotnet', default='dotnet')
    parser.add_argument('--host', help='Mac LAN IPv4 address reachable from a .NET 10 device')
    args = parser.parse_args()
    dotnet = shutil.which(args.dotnet)
    if not dotnet:
        raise ValueError('The configured .NET executable was not found: ' + args.dotnet)
    args.dotnet = str(Path(dotnet).resolve())
    physical = args.rid == 'ios-arm64'
    if physical and not args.framework and (args.sdk_version or '').startswith('11.'):
        args.framework = 'net11.0-ios'
    coreclr = physical and (args.framework or '').startswith('net11.')
    network_device = physical and not coreclr
    relay = None
    session = DeviceSession(args.session_directory, args.session_id, args.device) if physical else Session(args.session_directory, args.session_id)
    server = None if physical else make_server(session)
    if server:
        threading.Thread(target=server.serve_forever, daemon=True).start()
    environment = dict(os.environ,
        DOROTI_DEV_URL=f'http://127.0.0.1:{server.server_port}/session' if server else '',
        DOROTI_DEV_DEVICE_SESSION='Doroti.Dev/' + args.session_id if physical else '',
        DOROTI_DEV_TOKEN=session.token, DOROTI_DEV_SESSION_ID=args.session_id,
        DOTNET_HOST_PATH=args.dotnet, DOTNET_CUSTOM_PATH=args.dotnet,
        DOTNET_CLI_USE_MSBUILD_SERVER='0', MSBUILDDISABLENODEREUSE='1')
    # Do not give the sandbox a path on the Mac. The runtime publishes remotely.
    environment.pop('DOROTI_DEV_SESSION', None)
    command = [args.dotnet, 'watch', '--project', args.runner, '--configuration', 'Debug',
        '--runtime', args.rid, '--device', args.device, '--no-launch-profile',
        '--property:DorotiIosDevelopment=true', '--property:DorotiCompilationMode=' + ('CoreClr' if coreclr else 'Mono'),
        '--property:BuildInParallel=false', '--property:RestoreDisableParallel=true']
    if environment.get('DOTNET_CLI_CONTEXT_VERBOSE', '').lower() in ('true', '1', 'trace'):
        command.append('--verbose')
    if args.framework:
        command.append('--property:DorotiIosTargetFramework=' + args.framework)
    working_directory = args.app_root
    if args.sdk_version:
        if not re.fullmatch(r'[0-9]+\.[0-9]+\.[0-9]+(-[A-Za-z0-9.-]+)?', args.sdk_version):
            raise ValueError('Invalid SDK version')
        working_directory = session.directory / 'toolchain'
        working_directory.mkdir(exist_ok=True)
        atomic_json(working_directory / 'global.json', {'sdk': {
            'version': args.sdk_version, 'rollForward': 'disable', 'allowPrerelease': True}})
    child = None
    bundle_id = None
    stop = threading.Event()
    for sig in (signal.SIGINT, signal.SIGTERM):
        signal.signal(sig, lambda *_: stop.set())
    try:
        sdk = subprocess.check_output([args.dotnet, '--version'], cwd=working_directory,
                                      env=environment, text=True, timeout=30).strip()
        if tuple(int(part) for part in sdk.split('-')[0].split('.')) < (10, 0, 400):
            raise ValueError('iOS Hot Reload requires .NET SDK 10.0.400 or newer with mobile dotnet-watch support.')
        if coreclr and int(sdk.split('.')[0]) < 11:
            raise ValueError('Device Hot Reload requires .NET SDK 11; select an installed version with -IosSdkVersion.')
        if network_device:
            host = args.host or local_network_address()
            address = ipaddress.IPv4Address(host)
            if address.is_loopback or address.is_unspecified or address.is_multicast:
                raise ValueError('-IosHotReloadHost must be a Mac LAN IPv4 address reachable from the iPhone.')
            with socket.socket() as reservation:
                reservation.bind(('127.0.0.1', 0))
                port = reservation.getsockname()[1]
            relay = network_relay(host, port)
            threading.Thread(target=relay.serve_forever, daemon=True).start()
            environment['DOTNET_WATCH_AGENT_WEBSOCKET_PORT'] = str(port)
            environment['DOROTI_DEV_HOTRELOAD_ENDPOINT'] = f'ws://{host}:{relay.server_address[1]}'
            print(f'.NET 10 device Hot Reload: connect the iPhone and Mac to the same network ({host}). Allow local network access on the iPhone.', flush=True)
        # Native registrar/AOT caches are incompatible across Apple SDK/runtime
        # profiles. Never reuse normal Mono or NativeAOT build intermediates.
        artifacts = Path(args.app_root) / '.doroti/cache/development' / sdk / args.rid
        if network_device:
            artifacts /= 'mono-interpreter'
        query = [args.dotnet, 'msbuild', args.runner, '-nologo', '-getProperty:ApplicationId,DorotiIosDevelopmentRuntimeVersion',
                 '-p:Configuration=Debug', '-p:RuntimeIdentifier=' + args.rid,
                 '-p:DorotiIosDevelopment=true', '-p:DorotiCompilationMode=' + ('CoreClr' if coreclr else 'Mono'), '-p:ArtifactsPath=' + str(artifacts)]
        if args.framework:
            query.append('-p:DorotiIosTargetFramework=' + args.framework)
        query_environment = dict(environment, DOTNET_CLI_CONTEXT_VERBOSE='false')
        properties = json.loads(subprocess.check_output(query, cwd=working_directory, env=query_environment,
                                                       text=True, timeout=30))['Properties']
        bundle_id = properties['ApplicationId']
        if not bundle_id or any(c.isspace() for c in bundle_id):
            raise ValueError('The iOS runner must declare ApplicationId for session cleanup.')
        if coreclr:
            runtime_version = properties['DorotiIosDevelopmentRuntimeVersion']
            if not re.fullmatch(r'[0-9]+\.[0-9]+\.[0-9]+(-[A-Za-z0-9.-]+)?', runtime_version):
                raise ValueError('The device development profile must specify a runtime version.')
            # Apple framework copy/R2R caches may retain a previously bundled
            # native CoreCLR even after NuGet selects a different runtime pack.
            artifacts /= 'coreclr-' + runtime_version
        if physical:
            session.bundle_id = bundle_id
        command.append('--property:ArtifactsPath=' + str(artifacts))
        child = subprocess.Popen(command, cwd=working_directory, env=environment,
                                 start_new_session=True)
        parent = os.getppid()
        while child.poll() is None and not stop.wait(.25) and os.getppid() == parent:
            if physical:
                session.poll()
            else:
                session.expire()
        return child.returncode if child.poll() is not None else 0
    finally:
        if child and child.poll() is None:
            try:
                os.killpg(child.pid, signal.SIGINT)
            except ProcessLookupError:
                pass
            try:
                child.wait(timeout=8)
            except subprocess.TimeoutExpired:
                os.killpg(child.pid, signal.SIGKILL)
                child.wait()
        try:
            if child and bundle_id:
                # Simulator apps are launched by launchd, outside the watch process tree.
                if physical:
                    stop_device_app(args.device, bundle_id)
                else:
                    subprocess.run(['xcrun', 'simctl', 'terminate', args.device, bundle_id],
                                   stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL, timeout=15)
        finally:
            if server:
                server.shutdown()
                server.server_close()
            if relay:
                relay.shutdown()
                relay.server_close()
            session.expire(force=True)


if __name__ == '__main__':
    raise SystemExit(main())
