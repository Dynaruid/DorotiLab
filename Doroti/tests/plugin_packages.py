"""NuGet-only plugin consumer, generated metadata and negative SDK diagnostics."""
import json
from pathlib import Path
import shutil
import subprocess
import sys
import uuid
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]
run = ROOT / 'temp/testing/m4-plugins' / uuid.uuid4().hex[:8]
run.mkdir(parents=True)

def command(name, *args, expected=0):
    with (run / (name + '.log')).open('w', encoding='utf-8') as log:
        result = subprocess.run(args, cwd=ROOT, stdout=log, stderr=subprocess.STDOUT)
    output = (run / (name + '.log')).read_text(encoding='utf-8', errors='replace')
    if (expected == 0 and result.returncode != 0) or (expected != 0 and result.returncode == 0):
        raise RuntimeError(f'{name}: exit {result.returncode}\n{output[-7000:]}')
    print(f'PASS: {name}', flush=True)
    return output

try:
    command('build', 'dotnet', 'build', 'Doroti/tests/Doroti.Plugin.Tests/Doroti.Plugin.Tests.csproj', '--nologo', '-v:q')
    command('sdk-build', 'dotnet', 'build', 'Doroti/src/Doroti.Runner.Sdk/Doroti.Runner.Sdk.csproj', '--nologo', '-v:q')
    packages = run / 'packages'
    for name in ('Doroti.Runtime', 'Doroti.Ui', 'Doroti.Hosting', 'Doroti.Plugins', 'Doroti.Runner.Sdk'):
        command('pack-' + name, 'dotnet', 'pack', f'Doroti/src/{name}/{name}.csproj', '-c', 'Debug', '--no-build', '-o', str(packages), '--nologo')
    consumer = run / 'consumer'
    consumer.mkdir()
    (consumer / 'assets').mkdir()
    (consumer / 'assets/plugin.marker').write_text('packaged plugin asset')
    shutil.copyfile(ROOT / 'Doroti/tests/Doroti.Plugin.Tests/Program.cs', consumer / 'Program.cs')
    (consumer / 'application-manifest.json').write_text(json.dumps(dict(schemaVersion='doroti.application-capabilities/v1',
        applicationId='plugin.consumer', targetRid='win-x64', resources=[], plugins=[])))
    project = '''<Project Sdk="Microsoft.NET.Sdk">
<PropertyGroup><TargetFramework>net10.0</TargetFramework><OutputType>Exe</OutputType><ImplicitUsings>enable</ImplicitUsings>
<Nullable>enable</Nullable><RuntimeIdentifier>win-x64</RuntimeIdentifier><DorotiTarget>Windows</DorotiTarget>
<_DorotiGeneratedDirectory>obj/plugin/</_DorotiGeneratedDirectory></PropertyGroup>
<ItemGroup><PackageReference Include="Doroti.Plugins" Version="0.3.0-beta" />
<PackageReference Include="Doroti.Runner.Sdk" Version="0.3.0-beta" GeneratePathProperty="true" />
<DorotiPluginAsset Include="assets/plugin.marker" Link="plugin.marker" />
<EmbeddedResource Include="application-manifest.json" LogicalName="Doroti.Application.Manifest" /></ItemGroup>
<Import Project="$(PkgDoroti_Runner_Sdk)/Sdk/Doroti.Plugins.targets" Condition="Exists('$(PkgDoroti_Runner_Sdk)/Sdk/Doroti.Plugins.targets')" />
</Project>'''
    (consumer / 'Consumer.csproj').write_text(project)
    config = ET.Element('configuration')
    sources = ET.SubElement(config, 'packageSources')
    ET.SubElement(sources, 'clear')
    ET.SubElement(sources, 'add', key='local', value=str(packages))
    ET.SubElement(sources, 'add', key='nuget', value='https://api.nuget.org/v3/index.json')
    ET.ElementTree(config).write(consumer / 'NuGet.Config', encoding='utf-8', xml_declaration=True)
    cache = '-p:RestorePackagesPath=' + str(run / 'nuget')
    # Exercise the embedded SDK supplement as well as programmatic manifests.
    program = (consumer / 'Program.cs').read_text()
    program = program.replace('var handler = new NativeFeaturesHandler();', '''using (var generated = DorotiApplicationBoundary.Load(typeof(FakePicker).Assembly, "win-x64", [new NativeFeaturesHandler()]))
    Require(generated.Manifest.Plugins.Single().Id == "doroti.native-features", "Generated package manifest missing.");
var handler = new NativeFeaturesHandler();''')
    (consumer / 'Program.cs').write_text(program)
    command('consumer-restore', 'dotnet', 'restore', str(consumer / 'Consumer.csproj'), cache, '--nologo')
    command('consumer-run', 'dotnet', 'run', '--project', str(consumer / 'Consumer.csproj'), cache, '--no-restore')
    command('consumer-publish-trimmed', 'dotnet', 'publish', str(consumer / 'Consumer.csproj'), cache,
            '-c', 'Release', '--self-contained', 'true', '-p:PublishTrimmed=true', '-o', str(run / 'publish'), '--nologo')
    command('consumer-published-run', str(run / 'publish/Consumer.exe'))
    if (run / 'publish/plugin.marker').read_text() != 'packaged plugin asset':
        raise RuntimeError('Declared plugin asset was not published.')
    for name, item, code in (
        ('rid', '<DorotiNativePlugin Update="doroti.native-features" Rid="linux-x64" />', 'DOROTIPLUGIN002'),
        ('duplicate', '<DorotiNativePlugin Include="doroti.native-features" HandlerType="A.B" Channel="test/x" Codec="json" AbiVersion="1" PackageId="Test" Version="1" Rid="win-x64" />', 'DOROTIPLUGIN003'),
        ('channel', '<DorotiNativePlugin Include="another.plugin" HandlerType="A.B" Channel="doroti/native-features" Codec="json" AbiVersion="1" PackageId="Test" Version="1" Rid="win-x64" />', 'DOROTIPLUGIN004'),
        ('handler', '<DorotiNativePlugin Update="doroti.native-features" HandlerType="" />', 'DOROTIPLUGIN001'),
        ('asset', '<DorotiPluginAsset Include="missing-native.dll" />', 'DOROTIPLUGIN005'),
    ):
        # Modifications run after NuGet imports and before plugin generation.
        modified = project.replace('</Project>', f'<Target Name="Negative" BeforeTargets="GenerateDorotiPackagePlugins"><ItemGroup>{item}</ItemGroup></Target></Project>')
        (consumer / 'Consumer.csproj').write_text(modified)
        output = command('reject-' + name, 'dotnet', 'build', str(consumer / 'Consumer.csproj'), cache, '--no-restore', expected=1)
        if code not in output:
            raise RuntimeError(f'Expected {code}: {output[-3000:]}')
    print('PASS: NuGet-only restore/build/run/trimmed publish/run and SDK failures. Raw artifacts removed.', flush=True)
except Exception:
    print(f'FAILED: retained investigation directory {run}', file=sys.stderr)
    raise
else:
    if not run.resolve().is_relative_to((ROOT / 'temp/testing/m4-plugins').resolve()):
        raise RuntimeError('Refusing cleanup outside test directory.')
    shutil.rmtree(run)
