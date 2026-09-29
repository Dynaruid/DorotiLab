"""Build a selected, versioned NuGet feed and qualify a template consumer with an isolated cache.

This creates local unsigned candidates, never publishes a package or claims signing/install qualification.
Run through eng/run-with-timeout.py (1200 seconds). Raw consumers/logs are disposable.
"""
import argparse
from datetime import datetime, timezone
import hashlib
import json
import os
from pathlib import Path
import re
import shutil
import stat
import sys
import subprocess
import uuid
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--targets', nargs='+', choices=['windows', 'web', 'android', 'macos', 'ios', 'maccatalyst'], default=['windows', 'web'])
    parser.add_argument('--macos-tfm', choices=['net10.0-macos', 'net10.0-macos27.0'], default='net10.0-macos')
    parser.add_argument('--ios-tfm', default='net10.0-ios27.0')
    parser.add_argument('--catalyst-tfm', default='net10.0-maccatalyst27.0')
    parser.add_argument('--android-rid', choices=['android-arm64', 'android-x64'], default='android-arm64')
    parser.add_argument('--output', type=Path)
    parser.add_argument('--version', default='0.3.0-beta.rc.' + datetime.now(timezone.utc).strftime('%Y%m%d%H%M%S'))
    args = parser.parse_args()
    if len(set(args.targets) & {'macos', 'android', 'ios', 'maccatalyst'}) > 1:
        parser.error('Qualify each MAUI platform pack in a separate candidate run.')
    mac_properties = ['-r', 'osx-arm64', '-p:DorotiMacOSTargetFramework=' + args.macos_tfm]
    apple = next((target for target in args.targets if target in ('ios', 'maccatalyst')), None)
    apple_rid = 'iossimulator-arm64' if apple == 'ios' else 'maccatalyst-arm64'
    apple_tfm = args.ios_tfm if apple == 'ios' else args.catalyst_tfm
    apple_properties = ['-r', apple_rid, '-p:' + ('DorotiIosTargetFramework' if apple == 'ios' else 'DorotiMacCatalystTargetFramework') + '=' + apple_tfm,
                        '-p:DorotiCompilationMode=Mono'] if apple else []
    if not re.fullmatch(r'[0-9]+\.[0-9]+\.[0-9]+(?:-[0-9A-Za-z.-]+)?', args.version):
        parser.error('Version must be a NuGet semantic version.')
    output = (args.output or ROOT / 'Doroti/artifacts/release' / args.version).resolve()
    if output.exists() and any(output.iterdir()):
        parser.error('Output must be empty; published candidate versions are never overwritten.')
    output.mkdir(parents=True, exist_ok=True)
    run = ROOT / 'temp/testing/release-candidate' / uuid.uuid4().hex[:12]
    run.mkdir(parents=True)
    packages = output / 'packages'
    packages.mkdir()
    record = {'revision': subprocess.check_output(['git', 'rev-parse', 'HEAD'], cwd=ROOT, text=True).strip(),
              'version': args.version, 'targets': args.targets, 'configuration': 'Release', 'signing': 'notVerified',
              'cleanMachineInstall': 'notVerified', 'physicalInput': 'notVerified', 'checks': [], 'status': 'running'}

    def command(name, *command_args, cwd=ROOT, env=None):
        print('Running ' + name, flush=True)
        log_path = run / (name + '.log')
        with log_path.open('w', encoding='utf-8') as log:
            result = subprocess.run(command_args, cwd=cwd,
                env={**(env or os.environ), 'MSBUILDDISABLENODEREUSE': '1', 'DOTNET_CLI_USE_MSBUILD_SERVER': '0'},
                stdout=log, stderr=subprocess.STDOUT)
        if result.returncode:
            raise RuntimeError(f'{name}: exit {result.returncode}\n' + log_path.read_text(encoding='utf-8', errors='replace')[-10000:])
        record['checks'].append(name)

    try:
        (output / 'dotnet-info.txt').write_bytes(subprocess.check_output(['dotnet', '--info'], cwd=ROOT))
        # Include uncommitted/new product source in the provenance, not only HEAD.
        files = subprocess.check_output(['git', 'ls-files', '-co', '--exclude-standard', '-z'], cwd=ROOT).decode().split('\0')
        source = hashlib.sha256()
        for name in sorted(set(files)):
            path = ROOT / name
            if name and path.is_file():
                source.update(name.encode()); source.update(path.read_bytes())
        record['sourceTreeSha256'] = source.hexdigest()
        roots = [ROOT / 'Doroti/src/Doroti.Framework.Material/Doroti.Framework.Material.csproj',
                 ROOT / 'Doroti/src/Doroti.Plugins/Doroti.Plugins.csproj']
        for target in args.targets:
            package = {'windows': 'Doroti.Target.Windows.WindowsAppSdk.win-x64', 'web': 'Doroti.Target.Web.browser-wasm',
                       'android': 'Doroti.Target.Android.Maui.' + args.android_rid,
                       'macos': 'Doroti.Target.MacOS.Maui.osx-arm64',
                       'ios': 'Doroti.Target.iOS.Maui.iossimulator-arm64',
                       'maccatalyst': 'Doroti.Target.MacCatalyst.Maui.maccatalyst-arm64'}[target]
            roots.append(ROOT / 'Doroti/src' / package / (package + '.csproj'))
        projects = set()

        def visit(path):
            path = path.resolve()
            if path in projects or path.suffix != '.csproj': return
            projects.add(path)
            if path.stem == 'Doroti.Host.Maui':
                evaluated = json.loads(subprocess.check_output(['dotnet', 'msbuild', str(path), '-getItem:ProjectReference',
                    '-p:TargetFramework=' + (apple_tfm if apple else args.macos_tfm if 'macos' in args.targets else 'net10.0-android'),
                    '-p:DorotiMacOSTargetFramework=' + args.macos_tfm,
                    '-p:RuntimeIdentifier=' + (apple_rid if apple else 'osx-arm64' if 'macos' in args.targets else args.android_rid)], cwd=ROOT, text=True))
                for reference in evaluated['Items']['ProjectReference']:
                    visit(Path(reference['FullPath']))
            else:
                for reference in ET.parse(path).iter('ProjectReference'):
                    visit(path.parent / reference.attrib['Include'].replace('\\', '/'))
        version = '-p:Version=' + args.version
        for project in roots:
            properties = mac_properties if project.stem == 'Doroti.Target.MacOS.Maui.osx-arm64' else []
            if apple and project.stem.startswith(('Doroti.Target.iOS.', 'Doroti.Target.MacCatalyst.')): properties = apple_properties
            command('build-' + project.stem, 'dotnet', 'build', str(project), '-c', 'Release', version, '--nologo', *properties)
            visit(project)
        for sdk in ['Doroti.App.Sdk', 'Doroti.Runner.Sdk']:
            project = ROOT / 'Doroti/src' / sdk / (sdk + '.csproj')
            command('build-' + sdk, 'dotnet', 'build', str(project), '-c', 'Release', version, '--nologo')
            projects.add(project)
        template = ROOT / 'Doroti/templates/Doroti.Templates/Doroti.Templates.csproj'
        command('template', 'dotnet', 'pack', str(template), '-c', 'Release', version, '-o', str(packages), '--nologo')
        for project in sorted(projects):
            pack_properties = ['-p:RuntimeIdentifier=' + args.android_rid] if project.stem == 'Doroti.Host.Maui' else []
            if 'macos' in args.targets and project.stem in ('Doroti.Host.Maui', 'Doroti.Target.MacOS.Maui.osx-arm64'):
                pack_properties = ['-p:RuntimeIdentifier=osx-arm64', '-p:DorotiMacOSTargetFramework=' + args.macos_tfm]
            if apple and (project.stem == 'Doroti.Host.Maui' or project.stem.startswith(('Doroti.Target.iOS.', 'Doroti.Target.MacCatalyst.'))):
                pack_properties = apple_properties
            command('pack-' + project.stem, 'dotnet', 'pack', str(project), '-c', 'Release', '--no-build', version, '-o', str(packages), '--nologo', *pack_properties)
        consumer = run / 'consumer'
        hive = str(run / 'template-hive')
        command('template-install', 'dotnet', 'new', 'install', str(packages / f'Doroti.Templates.{args.version}.nupkg'), '--debug:custom-hive', hive)
        command('template-create', 'dotnet', 'new', 'doroti-app', '-n', 'CandidateApp', '-o', str(consumer), '--debug:custom-hive', hive)
        for path in consumer.rglob('*.csproj'):
            path.write_text(path.read_text(encoding='utf-8-sig').replace('0.3.0-beta', args.version), encoding='utf-8')
        app_project = consumer / 'CandidateApp.csproj'
        app_project.write_text(app_project.read_text().replace('</Project>',
            f'<ItemGroup><PackageReference Include="Doroti.Plugins" Version="{args.version}" /></ItemGroup></Project>'))
        (consumer / 'Directory.Build.targets').write_text('<Project />')
        config = ET.Element('configuration')
        sources = ET.SubElement(config, 'packageSources')
        ET.SubElement(sources, 'clear')
        ET.SubElement(sources, 'add', key='candidate', value=str(packages))
        ET.SubElement(sources, 'add', key='nuget', value='https://api.nuget.org/v3/index.json')
        ET.ElementTree(config).write(consumer / 'NuGet.Config', encoding='utf-8', xml_declaration=True)
        environment = {**os.environ, 'NUGET_PACKAGES': str(run / 'nuget'), 'NUGET_HTTP_CACHE_PATH': str(run / 'http-cache')}
        startup = consumer / 'desktop/DesktopStartup.cs'
        startup.write_text(startup.read_text(encoding='utf-8-sig').replace('Options = new WindowOptions', '''OnCreated = async (context, ct) =>
                {
                    if (Environment.GetEnvironmentVariable("DOROTI_RELEASE_SMOKE") != "1") return;
                    await context.Window.WaitUntilReadyToShowAsync(ct);
                    var second = await context.Windows.CreateWindowAsync(desktop.LegacyMainWindow with
                    {
                        Options = new WindowOptions { Title = "Package consumer second window", Size = new Size(500, 600) },
                        OnCreated = null,
                    }, ct);
                    await second.WaitUntilReadyToShowAsync(ct);
                    await second.SetSizeAsync(new Size(520, 620), ct);
                    await second.CloseAsync();
                    await context.Window.CloseAsync();
                    Console.WriteLine("PASS: NuGet-only Release native presentation, second window, resize and close.");
                },
                Options = new WindowOptions'''), encoding='utf-8')
        for target in args.targets:
            project = consumer / ('macos' if target == 'maccatalyst' else target) / f'CandidateApp.{dict(macos="MacOS", ios="iOS", maccatalyst="MacCatalyst").get(target, target.title())}.csproj'
            target_properties = ['-r', args.android_rid, '-p:EmbedAssembliesIntoApk=true'] if target == 'android' else []
            if target == 'macos':
                target_properties = mac_properties + ['-p:EnableCodeSigning=true', '-p:LinkMode=None']
                project.write_text(project.read_text().replace('</PropertyGroup>', '''
    <DorotiDesktopProject>../desktop/CandidateApp.Desktop.csproj</DorotiDesktopProject>
    <DorotiDesktopStartupType>CandidateApp.Desktop.DesktopStartup</DorotiDesktopStartupType>
  </PropertyGroup>''', 1))
            if target in ('ios', 'maccatalyst'):
                target_properties = apple_properties + ['-p:EnableCodeSigning=false', '-p:CreatePackage=false', '-p:LinkMode=None', '-p:PublishAot=false']
            # Apple permits simulator build/run, but publish requires a signed device RID.
            operation = 'build' if target == 'ios' else 'publish'
            output_properties = [] if target == 'ios' else ['-o', str(output / target)]
            command(operation + '-' + target, 'dotnet', operation, str(project), '-c', 'Release', '--nologo',
                    '-p:PublishTrimmed=' + ('true' if target in ('macos', 'ios', 'maccatalyst') else 'false'), '-p:RunAOTCompilation=false', *output_properties, *target_properties, cwd=consumer, env=environment)
            if target == 'windows':
                command('native-package-consumer', 'dotnet', str(output / target / 'CandidateApp.Windows.dll'),
                        cwd=output / target, env={**environment, 'DOROTI_RELEASE_SMOKE': '1'})
            elif target == 'macos':
                apps = list((output / target).glob('*.app'))
                if not apps:
                    installers = list((output / target).glob('*.pkg'))
                    if len(installers) != 1: raise RuntimeError('Expected one macOS installer payload.')
                    expanded = run / 'expanded-pkg'
                    command('expand-macos-package', 'pkgutil', '--expand-full', str(installers[0]), str(expanded))
                    payloads = list(expanded.rglob('*.app'))
                    if len(payloads) != 1: raise RuntimeError('Expected one app in the macOS installer.')
                    installed = output / target / payloads[0].name
                    command('extract-macos-app', 'ditto', str(payloads[0]), str(installed))
                    apps = [installed]
                if len(apps) != 1: raise RuntimeError('Expected one published macOS app bundle.')
                command('native-package-consumer', str(apps[0] / 'Contents/MacOS/CandidateApp.MacOS'),
                        cwd=output / target, env={**environment, 'DOROTI_RELEASE_SMOKE': '1'})
                native_log = (run / 'native-package-consumer.log').read_text(encoding='utf-8', errors='replace')
                if 'PASS: NuGet-only Release native presentation, second window, resize and close.' not in native_log:
                    raise RuntimeError('The native consumer exited without completing its window qualification.')
                command('macos-signature', 'codesign', '--verify', '--deep', '--strict', str(apps[0]))
                record['macos'] = {'tfm': args.macos_tfm, 'rid': 'osx-arm64', 'signing': 'ad-hoc only',
                    'notarization': 'notVerified', 'nativeAot': 'unsupported'}
            elif target in ('ios', 'maccatalyst'):
                apps = list((output / target).glob('*.app'))
                if not apps:
                    base_output = project.parent / ('bin/' + apple_rid if target == 'ios' else 'bin')
                    apps = list((base_output / 'Release' / apple_tfm / apple_rid).glob('*.app'))
                    if len(apps) == 1:
                        installed = output / target / apps[0].name
                        command('copy-apple-app', 'ditto', str(apps[0]), str(installed))
                        apps = [installed]
                if len(apps) != 1: raise RuntimeError('Expected one published Apple app bundle.')
                if target in ('ios', 'maccatalyst'):
                    # --deep alone treats loose Mono dylibs as resources and can
                    # leave their post-link signatures invalid on Apple Silicon.
                    for index, native in enumerate([*apps[0].rglob('*.dylib'), *apps[0].rglob('*.framework')]):
                        command('apple-sign-native-' + str(index), 'codesign', '--force', '--sign', '-', str(native))
                        command('apple-verify-native-' + str(index), 'codesign', '--verify', '--strict', str(native))
                    command('apple-local-signature', 'codesign', '--force', '--deep', '--sign', '-', str(apps[0]))
                    command('apple-verify-signature', 'codesign', '--verify', '--deep', '--strict', str(apps[0]))
                command('apple-native-consumer', sys.executable, str(ROOT / 'Doroti/tests/apple_package_smoke.py'),
                    '--target', target, '--app', str(apps[0]), '--output', str(run / 'apple-runtime'))
                record[target] = {'tfm': apple_tfm, 'rid': apple_rid,
                    'signing': 'ad-hoc only' if target == 'maccatalyst' else 'simulator ad-hoc only',
                    'nativeRuntime': json.loads((run / 'apple-runtime/summary.json').read_text()), 'deviceAot': 'notVerified',
                    'devicePublish': 'notVerified' if target == 'ios' else 'notApplicable'}
            elif target == 'android':
                if not any((output / target).glob('*-Signed.apk')):
                    raise RuntimeError('Android package-only publish produced no installable APK.')
                record['android'] = {'rid': args.android_rid, 'signing': 'development key only', 'deviceRuntime': 'notVerified'}
            else:
                wwwroot = output / target / 'wwwroot'
                if not (wwwroot / 'index.html').is_file() or not any((wwwroot / '_framework').glob('*.wasm')):
                    raise RuntimeError('Published Web bootstrap or wasm payload is missing.')
                record['webBrowserRuntime'] = 'notVerified'
        record['packages'] = {path.name: hashlib.sha256(path.read_bytes()).hexdigest() for path in packages.glob('*.nupkg')}
        record['artifacts'] = {target: {path.relative_to(output / target).as_posix(): hashlib.sha256(path.read_bytes()).hexdigest()
            for path in (output / target).rglob('*') if path.is_file()} for target in args.targets}
        record['status'] = 'PASS: isolated package-only template restore/build; selected device/desktop publish and deployment have separate records'
        print(record['status'], flush=True)
    except BaseException as error:
        record['status'] = 'FAILED'
        record['error'] = str(error)
        record['investigationDirectory'] = str(run)
        raise
    finally:
        if record['status'].startswith('PASS'):
            if not run.resolve().is_relative_to((ROOT / 'temp/testing').resolve()) or run.is_symlink():
                raise RuntimeError('Unsafe test cleanup path.')
            def remove_readonly(function, path, error):
                os.chmod(path, stat.S_IWRITE | stat.S_IREAD)
                function(path)
            try:
                shutil.rmtree(run, onerror=remove_readonly)
                record['cleanup'] = 'removed owned raw consumer, cache and logs'
            except OSError as error:
                record['cleanup'] = 'pending: ' + str(error)
                record['investigationDirectory'] = str(run)
                print('Cleanup pending: ' + str(run), flush=True)
        else:
            print('Investigation: ' + str(run), flush=True)
        (output / 'candidate.json').write_text(json.dumps(record, ensure_ascii=False, indent=2), encoding='utf-8')
        print('Candidate: ' + str(output), flush=True)


if __name__ == '__main__':
    main()
