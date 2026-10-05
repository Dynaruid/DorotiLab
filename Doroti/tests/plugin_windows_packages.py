"""Windows plugin or --drop qualification using only restored NuGet packages."""
from pathlib import Path
import shutil
import subprocess
import sys
import uuid
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]
drop = '--drop' in sys.argv
test_name = 'Doroti.Drop.Windows.Tests' if drop else 'Doroti.Plugin.Windows.Tests'
run_root = ROOT / ('temp/testing/m5a-windows-packages' if drop else 'temp/testing/m4-windows-packages')
run = run_root / uuid.uuid4().hex[:8]
run.mkdir(parents=True)

def command(name, *args):
    with (run / (name + '.log')).open('w', encoding='utf-8') as log:
        result = subprocess.run(args, cwd=ROOT, stdout=log, stderr=subprocess.STDOUT)
    output = (run / (name + '.log')).read_text(encoding='utf-8', errors='replace')
    if result.returncode:
        raise RuntimeError(f'{name}: exit {result.returncode}\n{output[-7000:]}')
    print(f'PASS: {name}', flush=True)
    if name == 'native-consumer': print(output, flush=True)

try:
    command('build', 'dotnet', 'build', f'Doroti/tests/{test_name}/{test_name}.csproj', '--nologo', '-v:q')
    projects = set()
    def visit(project):
        project = project.resolve()
        if project in projects: return
        projects.add(project)
        for reference in ET.parse(project).iter('ProjectReference'):
            visit(project.parent / reference.attrib['Include'].replace('\\', '/'))
    visit(ROOT / 'packages/platforms/windowsappsdk/Doroti.Host.WindowsAppSdk/Doroti.Host.WindowsAppSdk.csproj')
    if not drop: visit(ROOT / 'Doroti/src/Doroti.Plugins/Doroti.Plugins.csproj')
    packages = run / 'packages'
    for project in sorted(projects):
        command('pack-' + project.stem, 'dotnet', 'pack', str(project), '-c', 'Debug', '--no-build', '-o', str(packages), '--nologo')
    consumer = run / 'consumer'
    consumer.mkdir()
    shutil.copyfile(ROOT / f'Doroti/tests/{test_name}/Program.cs', consumer / 'Program.cs')
    (consumer / 'Consumer.csproj').write_text('''<Project Sdk="Microsoft.NET.Sdk">
<PropertyGroup><TargetFramework>net10.0-windows10.0.19041.0</TargetFramework><RuntimeIdentifier>win-x64</RuntimeIdentifier>
<OutputType>Exe</OutputType><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable>
<WindowsAppSDKSelfContained>true</WindowsAppSDKSelfContained><WindowsPackageType>None</WindowsPackageType><SelfContained>true</SelfContained>
</PropertyGroup><ItemGroup><PackageReference Include="Doroti.Host.WindowsAppSdk" Version="0.3.0-beta" />
<PackageReference Include="Doroti.Plugins" Version="0.3.0-beta" /></ItemGroup></Project>''')
    if drop:
        project = consumer / 'Consumer.csproj'
        project.write_text(project.read_text().replace('<ImplicitUsings>', '<UseWindowsForms>true</UseWindowsForms><ImplicitUsings>')
            .replace('<PackageReference Include="Doroti.Plugins" Version="0.3.0-beta" />', ''))
    config = ET.Element('configuration')
    sources = ET.SubElement(config, 'packageSources')
    ET.SubElement(sources, 'clear')
    ET.SubElement(sources, 'add', key='local', value=str(packages))
    ET.SubElement(sources, 'add', key='nuget', value='https://api.nuget.org/v3/index.json')
    ET.ElementTree(config).write(consumer / 'NuGet.Config', encoding='utf-8', xml_declaration=True)
    cache = '-p:RestorePackagesPath=' + str(run / 'nuget')
    command('consumer-publish', 'dotnet', 'publish', str(consumer / 'Consumer.csproj'), cache,
        '-c', 'Release', '-o', str(run / 'publish'), '--nologo')
    command('native-consumer', str(run / 'publish/Consumer.exe'), str(run / 'evidence'))
    if drop: command('consumer-dpi96', str(run / 'publish/Consumer.exe'), str(run / 'evidence96'), '--dpi-unaware')
    print('PASS: Windows NuGet-only restore/build/publish/run, ' + ('OS drop contracts' if drop else 'real picker and browser') + '. Raw artifacts removed.', flush=True)
except Exception:
    print(f'FAILED: retained investigation directory {run}', file=sys.stderr)
    raise
else:
    if not run.resolve().is_relative_to(run_root.resolve()):
        raise RuntimeError('Refusing cleanup outside test directory.')
    shutil.rmtree(run)
