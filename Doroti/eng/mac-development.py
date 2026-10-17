"""Own an AppKit/Catalyst SDK watcher and close its local development session."""
import argparse
import json
import os
from pathlib import Path
import shutil
import signal
import subprocess
import threading
from apple_development_session import AppleDevelopmentSession, stop_requested


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--runner', required=True)
    parser.add_argument('--app-root', required=True)
    parser.add_argument('--platform', choices=['macos', 'maccatalyst'], required=True)
    parser.add_argument('--session-directory', type=Path, required=True)
    parser.add_argument('--session-id', required=True)
    parser.add_argument('--framework')
    parser.add_argument('--rid')
    parser.add_argument('--dotnet', default='dotnet')
    parser.add_argument('--working-directory', type=Path)
    args = parser.parse_args()
    with AppleDevelopmentSession(args.session_directory, args.session_id) as ownership:
        ownership.claim('runner\0' + str(Path(args.runner).resolve()))
        return run(args)


def run(args):
    dotnet = shutil.which(args.dotnet)
    if not dotnet:
        raise ValueError('The configured .NET executable was not found: ' + args.dotnet)
    dotnet = str(Path(dotnet).resolve())
    environment = dict(os.environ, DOTNET_HOST_PATH=dotnet, DOTNET_CUSTOM_PATH=dotnet,
        DOROTI_DEV_SESSION=str(args.session_directory), DOROTI_DEV_SESSION_ID=args.session_id,
        DOTNET_CLI_USE_MSBUILD_SERVER='0', MSBUILDDISABLENODEREUSE='1')
    query_environment = dict(environment, DOTNET_CLI_CONTEXT_VERBOSE='false')
    working_directory = args.working_directory or Path(args.runner).resolve().parent
    sdk = subprocess.check_output([dotnet, '--version'], cwd=working_directory, env=query_environment,
                                  text=True, timeout=30).strip()
    if tuple(map(int, sdk.split('-')[0].split('.'))) < (10, 0, 400):
        raise ValueError('Mac Hot Reload requires .NET SDK 10.0.400 or newer.')
    properties = ['--property:DorotiMacDevelopment=true', '--property:BuildInParallel=false',
                  '--property:RestoreDisableParallel=true', '--property:EnableAotAnalyzer=false',
                  '--property:EnableTrimAnalyzer=false']
    if args.framework:
        name = 'DorotiMacOSTargetFramework' if args.platform == 'macos' else 'DorotiMacCatalystTargetFramework'
        properties.append('--property:' + name + '=' + args.framework)
        # Restore evaluates the multi-target host without the runner's RID.
        # Keep it on this Apple profile without assigning a global RID/TFM to
        # shared net10.0 projects in dotnet watch's graph.
        properties.append('--property:DorotiHostTargetFrameworks=' + args.framework)
    if args.rid:
        prefix = 'osx-' if args.platform == 'macos' else 'maccatalyst-'
        if args.rid not in (prefix + 'arm64', prefix + 'x64'):
            raise ValueError('Runtime identifier does not match the selected Mac target.')
        properties.append('--property:DorotiHostRuntimeIdentifier=' + args.rid)
    # Ask the runner for its fixed RID rather than silently changing its descriptor.
    rid = subprocess.check_output([dotnet, 'msbuild', args.runner, '-nologo',
        '-getProperty:RuntimeIdentifier', *properties], cwd=working_directory, env=query_environment,
        text=True, timeout=30).strip()
    if args.rid and args.rid != rid:
        raise ValueError('The runner RID differs from the workspace; update the manifest and runner together.')
    artifacts = Path(args.app_root) / '.doroti/cache/development' / sdk / rid / 'mac'
    properties.append('--property:ArtifactsPath=' + str(artifacts))
    command = [dotnet, 'watch', '--project', args.runner, '--configuration', 'Debug',
               '--no-launch-profile', *properties]
    if environment.get('DOTNET_CLI_CONTEXT_VERBOSE', '').lower() in ('true', '1', 'trace'):
        command.append('--verbose')
    stop = threading.Event()
    for sig in (signal.SIGINT, signal.SIGTERM):
        signal.signal(sig, lambda *_: stop.set())
    child = None
    try:
        child = subprocess.Popen(command, cwd=working_directory, env=environment, start_new_session=True)
        parent = os.getppid()
        while child.poll() is None and not stop.wait(.25) and os.getppid() == parent and not stop_requested(args.session_directory, args.session_id):
            pass
        return child.returncode if child.poll() is not None else 0
    finally:
        if child:
            # The executable is launched directly, never via launchd/open.
            try:
                os.killpg(child.pid, signal.SIGINT)
            except ProcessLookupError:
                pass
            try:
                child.wait(timeout=8)
            except subprocess.TimeoutExpired:
                pass
            try:
                os.killpg(child.pid, signal.SIGKILL)
            except ProcessLookupError:
                pass
            child.wait()
        status = args.session_directory / 'runtime.json'
        try:
            runtime = json.loads(status.read_text())
            if runtime.get('sessionId') == args.session_id:
                temporary = status.with_suffix('.tmp')
                temporary.write_text(json.dumps({**runtime, 'supported': False, 'status': 'closed'}))
                temporary.replace(status)
        except (OSError, ValueError):
            pass


if __name__ == '__main__':
    raise SystemExit(main())
