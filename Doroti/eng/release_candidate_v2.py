"""Independent core/design/provider packing and isolated workspace v2 consumers.

This module owns local unsigned candidates. No remote publication is performed.
"""
import argparse
from datetime import datetime, timezone
import hashlib
import json
import os
from pathlib import Path
import re
import subprocess
import sys
import uuid
import xml.etree.ElementTree as ET
import zipfile
from release_receipt import receipt_environment, validate_receipt, validate_android_receipt

ROOT = Path(__file__).resolve().parents[2]
TEMPLATE = ROOT / 'Doroti/templates/Doroti.Templates/content/doroti-app'


def package_metadata(package):
    with zipfile.ZipFile(package) as archive:
        spec = ET.fromstring(archive.read(next(name for name in archive.namelist() if name.endswith('.nuspec'))))
        metadata = next(node for node in spec if node.tag.endswith('metadata'))
        def value(field):
            return next(node.text for node in metadata if node.tag.endswith('}' + field) or node.tag == field)
        return dict(id=value('id'), version=value('version'), sha256=hashlib.sha256(package.read_bytes()).hexdigest(),
                    dependencies={node.attrib['id']: node.attrib['version'] for node in metadata.iter() if node.tag.endswith('dependency')},
                    manifests={name: json.loads(archive.read(name)) for name in archive.namelist()
                               if name.endswith(('doroti-provider.json', 'doroti-target-manifest.json'))},
                    nativeAssets={name: hashlib.sha256(archive.read(name)).hexdigest() for name in archive.namelist()
                                  if name.startswith('runtimes/') and '/native/' in name})


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--dotnet', default='dotnet')
    parser.add_argument('--workspace', type=Path, default=TEMPLATE / 'doroti-workspace.json')
    parser.add_argument('--targets', nargs='+', default=['windows', 'web'])
    parser.add_argument('--design', choices=['widgets', 'material', 'cupertino'], default='widgets')
    parser.add_argument('--core-version', default='0.4.0-alpha.1')
    parser.add_argument('--material-version', default='1.0.0-alpha.1')
    parser.add_argument('--cupertino-version', default='1.0.0-alpha.1')
    parser.add_argument('--provider-version', action='append', default=[], metavar='PROVIDER=VERSION')
    parser.add_argument('--runtime-targets', nargs='*', default=[])
    parser.add_argument('--device', help='Explicit matching Android serial for mobile runtime qualification.')
    parser.add_argument('--web-font-preset', choices=['Default', 'Offline'], default='Offline')
    parser.add_argument('--output', type=Path)
    parser.add_argument('--build-root', type=Path, help='Optional disposable build cache; published candidate payloads remain separate and immutable.')
    args = parser.parse_args()
    overrides = {}
    for entry in args.provider_version:
        if entry.count('=') != 1: parser.error('--provider-version uses provider=version')
        provider, version = entry.split('=')
        if provider in overrides: parser.error('Duplicate provider version: ' + provider)
        overrides[provider] = version
    for version in [args.core_version, args.material_version, args.cupertino_version, *overrides.values()]:
        if not re.fullmatch(r'[0-9]+\.[0-9]+\.[0-9]+(?:-[0-9A-Za-z.-]+)?', version): parser.error('Invalid NuGet version: ' + version)
    workspace_path = args.workspace.resolve()
    workspace = json.loads(workspace_path.read_text(encoding='utf-8-sig'))
    if workspace.get('schemaVersion') != 'doroti.workspace/v2': parser.error('Expected workspace v2')
    if len(set(args.targets)) != len(args.targets) or any(alias not in workspace['platforms'] for alias in args.targets): parser.error('Targets must be unique declared workspace aliases')
    if not set(args.runtime_targets).issubset(args.targets): parser.error('Runtime targets must be selected targets')
    output = (args.output or ROOT / 'Doroti/artifacts/release' / (args.core_version + '-' + uuid.uuid4().hex[:8])).resolve()
    if output.exists() and any(output.iterdir()): parser.error('Output must be empty; candidates are never overwritten')
    output.mkdir(parents=True, exist_ok=True)
    feed = output / 'packages'; feed.mkdir()
    run = ROOT / 'temp/testing/release-candidate' / uuid.uuid4().hex
    run.mkdir(parents=True)
    # Isolate candidates from a running workspace tool and keep Android aapt2
    # resource paths short on Windows. Each candidate owns its build outputs.
    build_root = (args.build_root or ROOT / 'Doroti/artifacts/rc' / run.name[:8]).resolve()
    if not build_root.is_relative_to(ROOT / 'Doroti/artifacts'):
        parser.error('Build cache must be under Doroti/artifacts.')
    record = dict(schemaVersion='doroti.release-candidate/v2', version=args.core_version,
                  versions=dict(core=args.core_version, material=args.material_version, cupertino=args.cupertino_version, providers={}),
                  targets=args.targets, design=args.design, configuration='Release', status='running', checks=[],
                  sourceRevision=subprocess.check_output(['git', 'rev-parse', 'HEAD'], cwd=ROOT, text=True).strip(),
                  sourceWorkspace=str(workspace_path), buildArtifacts=str(build_root), signing='notVerified', cleanMachineInstall='notVerified',
                  physicalInput='notVerified', investigationDirectory=str(run))

    def command(label, arguments, cwd=ROOT, env=None):
        print('Running ' + label, flush=True)
        with (run / (label + '.log')).open('w', encoding='utf-8') as log:
            result = subprocess.run([sys.executable, str(ROOT / 'Doroti/eng/run-with-timeout.py'), '--timeout', '1200', *map(str, arguments)],
                                    cwd=cwd, env=env, stdout=log, stderr=subprocess.STDOUT)
        text = (run / (label + '.log')).read_text(encoding='utf-8-sig', errors='replace')
        if result.returncode: raise RuntimeError(label + ': ' + text[-10000:])
        record['checks'].append(label)
        return text

    try:
        # Discover declared roots, then use actual MSBuild evaluation for every graph/version decision.
        paths = subprocess.check_output(['rg', '--files', 'Doroti/src', 'packages', '-g', '*.csproj', '-g', '!**/obj/**', '-g', '!**/bin/**'], cwd=ROOT, text=True).splitlines()
        index = {}
        for name in paths:
            project = (ROOT / name).resolve()
            ids = list(ET.parse(project).iter('PackageId'))
            index.setdefault(ids[0].text if ids else project.stem, []).append(project)
        def source_project(identity):
            matches = index.get(identity, [])
            if len(matches) != 1: raise RuntimeError('Package source root is missing or ambiguous: ' + identity)
            return matches[0]
        properties = ['-p:DorotiCoreVersion=' + args.core_version, '-p:DorotiMaterialVersion=' + args.material_version,
                      '-p:DorotiCupertinoVersion=' + args.cupertino_version, '-p:ArtifactsPath=' + str(build_root / 'source')]
        roots = [(source_project(identity), None) for identity in ('Doroti.Framework.Widgets', 'Doroti.Plugins', 'Doroti.App.Sdk', 'Doroti.Runner.Sdk')]
        if args.design != 'widgets': roots.append((source_project('Doroti.' + args.design.title()), None))
        selected = {}; profiles = {}
        for alias in args.targets:
            platform = workspace['platforms'][alias]
            declaration = platform['providerManifest']
            if declaration.startswith('nuget:'):
                tool = source_project(declaration[6:]); manifest_path = tool.parent / 'doroti-provider.json'
            else:
                manifest_path = (workspace_path.parent / declaration).resolve()
                tool = source_project(Path(json.loads(manifest_path.read_text())['tool']['assembly']).stem)
            descriptor = json.loads(manifest_path.read_text(encoding='utf-8-sig'))
            if descriptor['schemaVersion'] != 'doroti.platform-provider/v1' or descriptor['id'] != platform['provider']: raise RuntimeError('Provider identity mismatch: ' + alias)
            profile = descriptor['profiles'][platform['profile']]
            host = 'windows' if sys.platform == 'win32' else 'macos' if sys.platform == 'darwin' else 'linux'
            if host not in profile['hostOperatingSystems']: raise RuntimeError('Unavailable provider host: ' + alias)
            if 'publish' not in profile['operations']: raise RuntimeError('Provider profile has no publish operation: ' + alias)
            if platform['provider'] in profiles and profiles[platform['provider']] != platform['profile']: raise RuntimeError('Qualify each provider profile in a separate run')
            profiles[platform['provider']] = platform['profile']
            version_file = tool.parent.parent / 'Version.props'
            version_property = next(node.tag for group in ET.parse(version_file).getroot() for node in group if node.tag.startswith('Doroti') and node.tag.endswith('Version'))
            version = overrides.get(platform['provider'], descriptor['version'])
            property_value = '-p:' + version_property + '=' + version
            if property_value not in properties: properties.append(property_value)
            record['versions']['providers'][platform['provider']] = version
            target = source_project(platform['targetPackage'])
            selected[alias] = dict(platform=platform, descriptor=descriptor, target=target, tool=tool)
            roots.extend([(target, platform), (tool, None)])
        if set(overrides) - set(profiles): raise RuntimeError('Version override for an unselected provider')
        graph = {}
        def visit(project, platform=None):
            project = project.resolve()
            if project in graph: return
            profile_properties = []
            if platform:
                provider_root = next(item['tool'].parent.parent for item in selected.values()
                                     if item['platform']['provider'] == platform['provider'])
                if project.is_relative_to(provider_root):
                    # A RID declared inside the target project is not a global
                    # restore input. Its multi-target Host otherwise restores
                    # every OS workload while packing a single Apple profile.
                    profile_properties.append('-p:RuntimeIdentifier=' + platform['runtimeIdentifier'])
            if platform and list(ET.parse(project).iter('TargetFrameworks')):
                profile_properties = ['-p:TargetFramework=' + platform['targetFramework'],
                                      '-p:RuntimeIdentifier=' + platform['runtimeIdentifier']]
            model = json.loads(command('evaluate-' + project.stem, [args.dotnet, 'msbuild', project, '-nologo',
                '-getProperty:PackageId,PackageVersion,Version,TargetFramework,TargetFrameworks,IsPackable', '-getItem:ProjectReference', *properties, *profile_properties]))
            graph[project] = (model, profile_properties)
            for reference in model['Items']['ProjectReference']: visit(Path(reference['FullPath']), platform)
        for project, platform in roots: visit(project, platform)
        digest = hashlib.sha256()
        for name in sorted(set(subprocess.check_output(['git', 'ls-files', '-co', '--exclude-standard', '-z'], cwd=ROOT).decode().split('\0'))):
            path = ROOT / name
            if name.startswith(('Doroti/src/', 'packages/', 'Doroti/eng/', 'Doroti/templates/')) and path.is_file():
                digest.update(name.encode()); digest.update(path.read_bytes())
        record['sourceTreeSha256'] = digest.hexdigest()
        record['evaluatedProjects'] = {str(path.relative_to(ROOT)): model['Properties'] for path, (model, _) in graph.items()}
        for project, _ in roots:
            command('build-' + project.stem, [args.dotnet, 'build', project, '-c', 'Release', '--nologo', *properties, *graph[project][1]])
        for project, (model, profile_properties) in graph.items():
            if model['Properties']['IsPackable'] != 'true': continue
            if profile_properties: command('build-profile-' + project.stem, [args.dotnet, 'build', project, '-c', 'Release', *properties, *profile_properties])
            command('pack-' + project.stem, [args.dotnet, 'pack', project, '-c', 'Release', '--no-build', '-o', feed, *properties, *profile_properties])
        command('pack-template', [args.dotnet, 'pack', TEMPLATE.parent.parent / 'Doroti.Templates.csproj', '-c', 'Release', '-o', feed, *properties])
        metadata = {path.name: package_metadata(path) for path in feed.glob('*.nupkg')}
        record['packageDetails'] = metadata
        versions = {item['id']: item['version'] for item in metadata.values()}
        for alias, item in selected.items():
            package = next(value for value in metadata.values() if value['id'] == item['platform']['targetPackage'])
            manifest = package['manifests']['doroti/doroti-target-manifest.json']
            if manifest['schemaVersion'] != 'doroti.target-package/v2' or manifest['packageVersion'] != package['version'] or manifest['providerVersion'] != record['versions']['providers'][item['platform']['provider']]: raise RuntimeError('Packaged target identity/version mismatch: ' + alias)
        hive = run / 'template-hive'; consumer = run / 'consumer'
        command('template-install', [args.dotnet, 'new', 'install', feed / ('Doroti.Templates.' + versions['Doroti.Templates'] + '.nupkg'), '--debug:custom-hive', hive])
        command('template-create', [args.dotnet, 'new', 'doroti-app', '-n', 'CandidateApp', '-o', consumer, '--design', args.design, '--debug:custom-hive', hive])
        for path in consumer.rglob('*.csproj'):
            text = path.read_text(encoding='utf-8-sig')
            for identity, version in versions.items():
                text = re.sub(r'(\b(?:Include|Update|Name)="' + re.escape(identity) + r'"[^>]*\bVersion=")[^"]+', lambda match: match[1] + version, text)
                text = re.sub(re.escape(identity) + r'/[0-9]+\.[0-9]+\.[0-9]+(?:-[0-9A-Za-z.-]+)?', identity + '/' + version, text)
            path.write_text(text, encoding='utf-8')
        if args.design == 'material':
            # Pin the selected compatible Cupertino candidate explicitly. Its range
            # minimum may belong to another release that is absent from this private feed.
            app_project = consumer / 'CandidateApp.csproj'
            app_project.write_text(app_project.read_text().replace('</Project>',
                f'<ItemGroup><PackageReference Include="Doroti.Cupertino" Version="{args.cupertino_version}" /></ItemGroup></Project>'))
        for name in ('Directory.Build.targets', 'Directory.Packages.props'): (consumer / name).write_text('<Project />', encoding='utf-8')
        # Preserve generated identity/version while blocking parent repository imports.
        consumer_props = ET.parse(consumer / 'Directory.Build.props').getroot()
        ET.SubElement(ET.SubElement(consumer_props, 'PropertyGroup'), 'ArtifactsPath').text = str(build_root / 'consumer')
        ET.ElementTree(consumer_props).write(consumer / 'Directory.Build.props', encoding='utf-8', xml_declaration=True)
        (consumer / 'NuGet.Config').write_text(f'<configuration><packageSources><clear/><add key="candidate" value="{feed}"/><add key="nuget" value="https://api.nuget.org/v3/index.json"/></packageSources><packageSourceMapping><packageSource key="candidate"><package pattern="Doroti.*"/></packageSource><packageSource key="nuget"><package pattern="*"/></packageSource></packageSourceMapping></configuration>', encoding='utf-8')
        generated = json.loads((consumer / 'doroti-workspace.json').read_text())
        generated_platforms = {}
        for alias, item in selected.items():
            matches = [value for value in generated['platforms'].values() if value['target'] == item['platform']['target']
                       and value['provider'] == item['platform']['provider'] and value['backend'] == item['platform']['backend']]
            if len(matches) != 1: raise RuntimeError('Template runner mapping is missing or ambiguous: ' + alias)
            generated_platforms[alias] = {**item['platform'], 'runner': matches[0]['runner'],
                                          'providerManifest': 'nuget:' + item['tool'].stem}
        generated['platforms'] = generated_platforms
        (consumer / 'doroti-workspace.json').write_text(json.dumps(generated, indent=2) + '\n')
        for alias, item in selected.items():
            if item['platform']['target'] == 'macOS':
                runner = consumer / generated['platforms'][alias]['runner']
                runner.write_text(runner.read_text().replace('</PropertyGroup>',
                    '<DorotiDesktopProject>../desktop/CandidateApp.Desktop.csproj</DorotiDesktopProject>'
                    '<DorotiDesktopStartupType>CandidateApp.Desktop.DesktopStartup</DorotiDesktopStartupType>'
                    '</PropertyGroup>', 1))
        environment = os.environ | dict(NUGET_PACKAGES=str(run / 'nuget'), NUGET_HTTP_CACHE_PATH=str(run / 'http-cache'))
        startup = consumer / 'desktop/DesktopStartup.cs'
        startup.write_text(startup.read_text().replace('Options = new WindowOptions', '''OnCreated = async (context, ct) =>
                {
                    if (Environment.GetEnvironmentVariable("DOROTI_RELEASE_SMOKE") != "1") return;
                    await context.Window.WaitUntilReadyToShowAsync(ct);
                    var second = await context.Windows.CreateWindowAsync(desktop.DefaultMainWindow with
                    { Options = new WindowOptions { Title = "Independent candidate", Size = new Size(500, 600) }, OnCreated = null }, ct);
                    await second.WaitUntilReadyToShowAsync(ct);
                    await second.SetSizeAsync(new Size(520, 620), ct);
                    await second.CloseAsync(CancellationToken.None);
                    System.IO.File.WriteAllText(Environment.GetEnvironmentVariable("DOROTI_RELEASE_RECEIPT")!,
                        System.Text.Json.JsonSerializer.Serialize(new { nativeFirstFrame = true, twoWindows = true,
                            secondResizedAndClosed = true, survivorsBeforeMainClose = 1,
                            runId = Environment.GetEnvironmentVariable("DOROTI_RELEASE_RUN_ID"),
                            version = Environment.GetEnvironmentVariable("DOROTI_RELEASE_VERSION") }));
                    await context.Window.CloseAsync(CancellationToken.None);
                }, Options = new WindowOptions'''))
        cli_project = ROOT / 'Doroti/src/Doroti.Tooling/Doroti.Tooling.csproj'
        command('build-cli', [args.dotnet, 'build', cli_project, '-c', 'Release', *properties])
        cli = Path(command('evaluate-cli-path', [args.dotnet, 'msbuild', cli_project, '-nologo', '-p:Configuration=Release',
                   '-getProperty:TargetPath', *properties]).strip())
        record['runtime'] = {}
        for alias, item in selected.items():
            runner = consumer / generated['platforms'][alias]['runner']
            publish_properties = ['-p:PublishTrimmed=false', '-p:RunAOTCompilation=false']
            if any('-' + platform in item['platform']['targetFramework'] for platform in ('macos', 'maccatalyst', 'ios')):
                # Apple SDKs require the trim pipeline even for an untrimmed
                # JIT candidate. Copy mode retains assemblies without lying to
                # the SDK about whether its pipeline runs.
                publish_properties = ['-p:PublishTrimmed=true', '-p:TrimMode=copy',
                                      '-p:LinkMode=None', '-p:MtouchLink=None', '-p:RunAOTCompilation=false']
            if item['platform']['runtimeIdentifier'] == 'browser-wasm': publish_properties.append('-p:DorotiWebFontPreset=' + args.web_font_preset)
            publish_properties += ['-p:TargetFramework=' + item['platform']['targetFramework'],
                                   '-p:RuntimeIdentifier=' + item['platform']['runtimeIdentifier']]
            record.setdefault('publishProperties', {})[alias] = publish_properties
            command('publish-' + alias, [args.dotnet, 'publish', runner, '-c', 'Release', '-o', output / alias, *publish_properties], cwd=consumer, env=environment)
            evaluated = json.loads(command('consumer-profile-' + alias, [args.dotnet, 'msbuild', runner, '-nologo',
                '-p:Configuration=Release', *publish_properties,
                '-getProperty:ProjectAssetsFile,TargetFramework,RuntimeIdentifier,TargetPlatformVersion,TargetPlatformMinVersion,UseMonoRuntime,PublishAot,TrimMode,LinkMode,MtouchLink,ApplicationId,ApplicationDisplayVersion,ApplicationVersion'],
                cwd=consumer, env=environment))['Properties']
            record.setdefault('consumerProfiles', {})[alias] = evaluated
            assets = json.loads(Path(evaluated['ProjectAssetsFile']).read_text())
            for value in assets['libraries'].values():
                if value['type'] == 'project' and not (runner.parent / value['msbuildProject']).resolve().is_relative_to(consumer.resolve()):
                    raise RuntimeError('Consumer references a project outside its generated workspace')
            command('describe-' + alias, [args.dotnet, cli, 'describe', '-App', consumer, '-Platform', alias], cwd=consumer, env=environment)
            record['runtime'][alias] = 'notVerified'
            if alias in args.runtime_targets:
                if item['platform']['target'] == 'Android':
                    if not args.device: raise RuntimeError('Android runtime qualification requires --device <serial>.')
                    record.setdefault('artifacts', {})[alias] = {path.relative_to(output / alias).as_posix(): hashlib.sha256(path.read_bytes()).hexdigest()
                        for path in (output / alias).rglob('*') if path.is_file()}
                    (output / 'candidate.json').write_text(json.dumps(record, indent=2), encoding='utf-8')
                    native_output = output / (alias + '-native')
                    command('runtime-' + alias, [sys.executable, ROOT / 'Doroti/tests/android_package_smoke.py',
                        '--candidate', output, '--alias', alias, '--device', args.device, '--run-id', run.name, '--output', native_output])
                    record['runtime'][alias] = validate_android_receipt(native_output / 'receipt.json', run.name, args.core_version,
                        evaluated['RuntimeIdentifier'], evaluated['ApplicationId'])
                    continue
                receipt = output / (alias + '-native-consumer.json')
                command('runtime-' + alias, [args.dotnet, cli, 'run', '-App', consumer, '-Platform', alias, '-Configuration', 'Release'], cwd=consumer,
                    env=receipt_environment(environment, receipt, run.name, args.core_version))
                record['runtime'][alias] = validate_receipt(receipt, run.name, args.core_version)
        record['packages'] = {name: value['sha256'] for name, value in metadata.items()}
        record['artifacts'] = {alias: {path.relative_to(output / alias).as_posix(): hashlib.sha256(path.read_bytes()).hexdigest()
                              for path in (output / alias).rglob('*') if path.is_file()} for alias in args.targets}
        record['status'] = 'PASS: independent local packages and isolated NuGet-only template publish; runtime qualification is recorded per alias'
        record['verifiedUtc'] = datetime.now(timezone.utc).isoformat()
        print(record['status'], flush=True)
    except BaseException as error:
        record['status'] = 'FAILED'; record['error'] = str(error)
        raise
    finally:
        (output / 'candidate.json').write_text(json.dumps(record, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
        print('Candidate: ' + str(output), flush=True)
