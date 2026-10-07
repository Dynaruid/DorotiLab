"""Source and isolated NuGet development-server integration, using real Blazor manifests."""
import contextlib
import http.server
import json
import os
from pathlib import Path
import signal
import socket
import ssl
import subprocess
import threading
import time
import urllib.request
import uuid
import zipfile

ROOT = Path(__file__).resolve().parents[2]
RUN = ROOT / 'temp/testing/web-dev-proxy' / uuid.uuid4().hex
RUN.mkdir(parents=True)


def command(name, args, env=None, success=True):
    result = subprocess.run([str(arg) for arg in args], cwd=ROOT, env=env,
                            capture_output=True, text=True, timeout=1200)
    (RUN / (name + '.log')).write_text(result.stdout + result.stderr, encoding='utf-8')
    assert (result.returncode == 0) == success, (name, result.returncode, result.stdout, result.stderr)
    return result.stdout


def port():
    with socket.socket() as sock:
        sock.bind(('127.0.0.1', 0))
        return sock.getsockname()[1]


@contextlib.contextmanager
def server(name, args, url, env, tls=None):
    with (RUN / (name + '.log')).open('w', encoding='utf-8') as log:
        process = subprocess.Popen([str(arg) for arg in args], cwd=ROOT, env=env,
                                   stdout=log, stderr=subprocess.STDOUT, start_new_session=os.name != 'nt')
        try:
            deadline = time.monotonic() + 120
            while True:
                assert process.poll() is None, (name, (RUN / (name + '.log')).read_text(encoding='utf-8'))
                try:
                    with urllib.request.urlopen(url, timeout=2, context=tls) as response:
                        response.read()
                    break
                except OSError:
                    if time.monotonic() > deadline:
                        raise TimeoutError(name)
                    time.sleep(.2)
            yield
        finally:
            if os.name == 'nt':
                subprocess.run(['taskkill', '/PID', str(process.pid), '/T', '/F'],
                               stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL, check=False)
            elif process.poll() is None:
                os.killpg(process.pid, signal.SIGTERM)
            process.wait(timeout=15)


class Backend(http.server.BaseHTTPRequestHandler):
    def do_GET(self):
        body = json.dumps({'path': self.path, 'host': self.headers['Host']}).encode()
        self.send_response(200)
        self.send_header('Content-Type', 'application/json')
        self.send_header('Content-Length', str(len(body)))
        self.end_headers()
        self.wfile.write(body)

    def log_message(self, *_):
        pass


def fixture(mode, feed):
    app = RUN / mode
    web = app / 'web'
    (web / 'wwwroot').mkdir(parents=True)
    (app / 'App.csproj').write_text('<Project Sdk="Microsoft.NET.Sdk" />', encoding='utf-8')
    (web / 'Program.cs').write_text('System.Console.WriteLine("fixture");', encoding='utf-8')
    (web / 'wwwroot/index.html').write_text('<!doctype html><html><body>proxy fixture</body></html>', encoding='utf-8')
    (web / 'wwwroot/fixture.js').write_text('console.log("asset fixture");', encoding='utf-8')
    extra = (f'<Import Project="{ROOT / "packages/platforms/web/tooling/buildTransitive/Doroti.Tool.Web.targets"}" />'
             if mode == 'source' else '<ItemGroup><PackageReference Include="Doroti.Tool.Web" Version="0.4.0-beta" /></ItemGroup>')
    project = web / 'Fixture.csproj'
    project.write_text(f'''<Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">
      <PropertyGroup><TargetFramework>net10.0</TargetFramework><DorotiTarget>Web</DorotiTarget>
      <DorotiAppProject>../App.csproj</DorotiAppProject><WasmEnableThreads>false</WasmEnableThreads>
      </PropertyGroup>
      <ItemGroup><PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" Version="10.0.11" />
      <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.DevServer" Version="10.0.11" PrivateAssets="all" /></ItemGroup>
      {extra}</Project>''', encoding='utf-8')
    (app / 'NuGet.Config').write_text(f'''<configuration><packageSources><clear />
      <add key="local" value="{feed}" /><add key="nuget" value="https://api.nuget.org/v3/index.json" />
      </packageSources></configuration>''', encoding='utf-8')
    env = {**os.environ, 'NUGET_PACKAGES': str(RUN / ('nuget-' + mode))}
    command(mode + '-build', ['dotnet', 'build', project, '-c', 'Debug', '--nologo'], env)
    return app, project, env


feed = RUN / 'feed'
for name, project in [
    ('contracts', 'Doroti/src/Doroti.Tooling.Contracts/Doroti.Tooling.Contracts.csproj'),
    ('sdk', 'Doroti/src/Doroti.Tooling.Extension.Sdk/Doroti.Tooling.Extension.Sdk.csproj'),
    ('web-tool', 'packages/platforms/web/tooling/Doroti.Tool.Web.csproj'),
]:
    command(name + '-pack', ['dotnet', 'pack', project, '-c', 'Release', '--output', feed, '--artifacts-path', RUN / 'pack-build', '--nologo'])
with zipfile.ZipFile(feed / 'Doroti.Tool.Web.0.4.0-beta.nupkg') as package:
    for file in ['Doroti.Web.DevServer.csproj', 'Program.cs', 'WebDevConfig.cs', 'DevelopmentProxy.cs']:
        assert 'devserver/' + file in package.namelist(), file
    assert 'buildTransitive/Doroti.Tool.Web.targets' in package.namelist()
    assert not any(name.startswith('tools/web-devserver/') for name in package.namelist())
    binaries = [name for name in package.namelist() if name.lower().endswith(('.dll', '.exe'))]
    assert binaries == ['lib/net10.0/Doroti.Tool.Web.dll'], binaries

with http.server.ThreadingHTTPServer(('127.0.0.1', 0), Backend) as backend:
    thread = threading.Thread(target=backend.serve_forever, daemon=True)
    thread.start()
    try:
        for mode in ['source', 'package']:
            app, project, env = fixture(mode, feed)
            config_port = port()
            config = app / 'web_dev_config.jsonc'
            settings = {'server': {
                'host': '127.0.0.1', 'port': config_port,
                'headers': [{'name': 'X-Development', 'value': 'configured'},
                            {'name': 'Cross-Origin-Opener-Policy', 'value': 'unsafe-none'}],
                'proxy': [{'target': f'http://127.0.0.1:{backend.server_port}/base/', 'prefix': '/api/', 'replace': '/'}]
            }}
            config.write_text('// Integration config with comments.\n' + json.dumps(settings), encoding='utf-8')
            run_args = ['dotnet', 'run', '--project', project, '-c', 'Debug', '--no-build', '--no-launch-profile', '-p:WasmEnableThreads=true']
            for cli in [False, True]:
                selected_port = port() if cli else config_port
                url = f'http://127.0.0.1:{selected_port}'
                arguments = run_args + (['--', '--urls', url] if cli else [])
                with server(f'{mode}-cli-{cli}', arguments, url, {**env, 'ASPNETCORE_URLS': f'http://127.0.0.1:{port()}'}):
                    # The executable and dependencies are built/restored into the app's local cache.
                    cached_servers = list((app / '.doroti/cache/web-devserver').glob('*/bin/Doroti.Web.DevServer.dll'))
                    assert len(cached_servers) == 1, cached_servers
                    assert (cached_servers[0].parent / 'Yarp.ReverseProxy.dll').is_file()
                    if mode == 'package':
                        package_sources = Path(env['NUGET_PACKAGES']) / 'doroti.tool.web/0.4.0-beta/devserver'
                        assert not (package_sources / 'obj').exists() and not (package_sources / 'bin').exists()
                    for path in ['/', '/client/route', '/fixture.js', '/_framework/blazor.webassembly.js']:
                        with urllib.request.urlopen(url + path, timeout=10) as response:
                            assert response.headers['X-Development'] == 'configured'
                            assert response.headers['Cross-Origin-Opener-Policy'] == 'same-origin'
                            assert response.headers['Cross-Origin-Embedder-Policy'] == 'require-corp'
                            assert len(response.read()) > 10
                    with urllib.request.urlopen(url + '/api/hello%20world?q=a%2Fb', timeout=10) as response:
                        assert json.load(response) == {'path': '/base/hello%20world?q=a%2Fb', 'host': f'127.0.0.1:{backend.server_port}'}
            if mode == 'source':
                url = f'http://127.0.0.1:{config_port}'
                watch = ['dotnet', 'watch', '--project', project, 'run', '--no-launch-profile']
                with server('source-watch', watch, url, {**env, 'DOTNET_WATCH_SUPPRESS_LAUNCH_BROWSER': '1'}):
                    with urllib.request.urlopen(urllib.request.Request(url, headers={'Accept': 'text/html'}), timeout=10) as response:
                        assert b'aspnetcore-browser-refresh.js' in response.read(), 'SDK browser-refresh injection'
                    with urllib.request.urlopen(url + '/_framework/aspnetcore-browser-refresh.js', timeout=10) as response:
                        assert len(response.read()) > 100
                    with urllib.request.urlopen(url + '/api/watch', timeout=10) as response:
                        assert json.load(response)['path'] == '/base/watch'
            else:
                certificate_directory = app / 'certs'
                command('test-certificate', ['dotnet', 'run', '--project', ROOT / 'packages/platforms/web/tests/Doroti.Web.DevServer.Tests/Doroti.Web.DevServer.Tests.csproj', '-c', 'Release', '--', '--write-test-certificate', certificate_directory])
                settings['server']['https'] = {'certPath': 'certs/localhost.pem', 'certKeyPath': 'certs/localhost-key.pem'}
                config.write_text(json.dumps(settings), encoding='utf-8')
                url = f'https://127.0.0.1:{config_port}'
                tls = ssl.create_default_context(cafile=str(certificate_directory / 'localhost.pem'))
                with server('package-https', run_args, url, env, tls):
                    with urllib.request.urlopen(url + '/api/tls', timeout=10, context=tls) as response:
                        assert json.load(response)['path'] == '/base/tls'
            config.rename(app / 'disabled.jsonc')
            properties = command(mode + '-no-config', ['dotnet', 'msbuild', project, '-nologo', '-t:ComputeRunArguments', '-getProperty:RunArguments'], env)
            assert 'blazor-devserver.dll' in properties
            command(mode + '-missing-config', ['dotnet', 'msbuild', project, '-nologo', '-t:ComputeRunArguments', '-p:DorotiWebDevConfig=missing.jsonc'], env, success=False)
            properties = command(mode + '-opt-out', ['dotnet', 'msbuild', project, '-nologo', '-t:ComputeRunArguments', '-getProperty:RunArguments', '-p:DorotiWebDevConfig=disabled.jsonc', '-p:DorotiWebDevServer=false'], env)
            assert 'blazor-devserver.dll' in properties
            print(f'{mode}: startup, manifests, SPA fallback, isolation/custom headers, live proxy, JSONC/CLI precedence, default/opt-out and {"SDK watch injection" if mode == "source" else "PEM HTTPS"}: PASS', flush=True)
    finally:
        backend.shutdown()
        thread.join(timeout=5)
print(f'Web development proxy integration: PASS; logs: {RUN}', flush=True)
