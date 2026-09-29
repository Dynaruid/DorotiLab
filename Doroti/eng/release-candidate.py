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
import subprocess
import uuid
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--targets', nargs='+', choices=['windows', 'web', 'android'], default=['windows', 'web'])
    parser.add_argument('--android-rid', choices=['android-arm64', 'android-x64'], default='android-arm64')
    parser.add_argument('--output', type=Path)
    parser.add_argument('--version', default='0.3.0-beta.rc.' + datetime.now(timezone.utc).strftime('%Y%m%d%H%M%S'))
    args = parser.parse_args()
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
                       'android': 'Doroti.Target.Android.Maui.' + args.android_rid}[target]
            roots.append(ROOT / 'Doroti/src' / package / (package + '.csproj'))
        projects = set()

        def visit(path):
            path = path.resolve()
            if path in projects or path.suffix != '.csproj': return
            projects.add(path)
            if path.stem == 'Doroti.Host.Maui':
                evaluated = json.loads(subprocess.check_output(['dotnet', 'msbuild', str(path), '-getItem:ProjectReference',
                    '-p:TargetFramework=net10.0-android', '-p:RuntimeIdentifier=' + args.android_rid], cwd=ROOT, text=True))
                for reference in evaluated['Items']['ProjectReference']:
                    visit(Path(reference['FullPath']))
            else:
                for reference in ET.parse(path).iter('ProjectReference'):
                    visit(path.parent / reference.attrib['Include'].replace('\\', '/'))
        version = '-p:Version=' + args.version
        for project in roots:
            command('build-' + project.stem, 'dotnet', 'build', str(project), '-c', 'Release', version, '--nologo')
            visit(project)
        for sdk in ['Doroti.App.Sdk', 'Doroti.Runner.Sdk']:
            project = ROOT / 'Doroti/src' / sdk / (sdk + '.csproj')
            command('build-' + sdk, 'dotnet', 'build', str(project), '-c', 'Release', version, '--nologo')
            projects.add(project)
        template = ROOT / 'Doroti/templates/Doroti.Templates/Doroti.Templates.csproj'
        command('template', 'dotnet', 'pack', str(template), '-c', 'Release', version, '-o', str(packages), '--nologo')
        for project in sorted(projects):
            pack_properties = ['-p:RuntimeIdentifier=' + args.android_rid] if project.stem == 'Doroti.Host.Maui' else []
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
            project = consumer / target / f'CandidateApp.{target.title()}.csproj'
            target_properties = ['-r', args.android_rid, '-p:EmbedAssembliesIntoApk=true'] if target == 'android' else []
            command('publish-' + target, 'dotnet', 'publish', str(project), '-c', 'Release', '--nologo',
                    '-p:PublishTrimmed=false', '-p:RunAOTCompilation=false', '-o', str(output / target), *target_properties, cwd=consumer, env=environment)
            if target == 'windows':
                command('native-package-consumer', 'dotnet', str(output / target / 'CandidateApp.Windows.dll'),
                        cwd=output / target, env={**environment, 'DOROTI_RELEASE_SMOKE': '1'})
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
        record['status'] = 'PASS: isolated package-only template restore/publish; deployment remains notVerified'
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
                shutil.rmtree(run, onexc=remove_readonly)
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
