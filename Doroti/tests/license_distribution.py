"""Verify library-owned notices through source references, NuGet transitivity and packed SDKs."""
from pathlib import Path
import argparse
import subprocess
import uuid
import xml.etree.ElementTree as ET
import zipfile

ROOT = Path(__file__).resolve().parents[2]
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--output', required=True, type=Path)
args = parser.parse_args()
out = args.output.resolve()
assert out.is_relative_to(ROOT / 'temp/testing')
out.mkdir(parents=True, exist_ok=False)
for name in ['Directory.Build.props', 'Directory.Build.targets']:
    (out / name).write_text('<Project />', encoding='utf-8')
feed = out / 'feed'
config = ET.Element('configuration')
sources = ET.SubElement(config, 'packageSources')
ET.SubElement(sources, 'clear')
ET.SubElement(sources, 'add', key='fixture', value=str(feed))
ET.SubElement(sources, 'add', key='nuget', value='https://api.nuget.org/v3/index.json')
ET.ElementTree(config).write(out / 'NuGet.Config', encoding='utf-8', xml_declaration=True)
version = '1.0.0-license' + uuid.uuid4().hex[:12]
registration = ROOT / 'Doroti/eng/Doroti.LicenseRegistration.targets'


def run(name, *command):
    result = subprocess.run(command, cwd=ROOT, capture_output=True, text=True,
                            encoding='utf-8', errors='replace', timeout=1200)
    (out / (name + '.log')).write_text(result.stdout + result.stderr, encoding='utf-8')
    assert result.returncode == 0, result.stdout + result.stderr
    print('PASS', name, flush=True)
    return result.stdout


projects = {}
for kind in ['Used', 'Unused']:
    name = 'LicenseFixture.' + kind
    directory = out / name
    directory.mkdir()
    (directory / 'LICENSE').write_text('registered by ' + name, encoding='utf-8')
    (directory / 'NOTICE.source.txt').write_text('renamed notice', encoding='utf-8')
    original = directory / 'buildTransitive'
    original.mkdir()
    (original / (name + '.targets')).write_text(f'<Project><PropertyGroup><OriginalLibraryTarget>{kind}</OriginalLibraryTarget></PropertyGroup></Project>')
    project = directory / (name + '.csproj')
    project.write_text(f'''<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup><TargetFramework>net10.0</TargetFramework><PackageId>{name}</PackageId><Version>{version}</Version></PropertyGroup>
  <ItemGroup>
    <DorotiLicense Include="LICENSE" />
    <DorotiLicense Include="NOTICE.source.txt" TargetPath="licenses/{name}/RENAMED" />
    <None Include="buildTransitive/{name}.targets" Pack="true" PackagePath="buildTransitive/" />
  </ItemGroup>
  <Import Project="{registration.as_posix()}" />
</Project>''', encoding='utf-8')
    projects[kind] = project
    run('pack-' + kind, 'dotnet', 'pack', str(project), '-c', 'Release', '-o', str(feed), '-v:q')

source = out / 'source-consumer'
source.mkdir()
source_project = source / 'Consumer.csproj'
source_project.write_text(f'''<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup>
  <ItemGroup><ProjectReference Include="{projects['Used'].as_posix()}" /></ItemGroup>
</Project>''', encoding='utf-8')
source_publish = source / 'publish'
run('source-reference', 'dotnet', 'publish', str(source_project), '-c', 'Release', '-o', str(source_publish), '-v:q')
assert (source_publish / 'licenses/LicenseFixture.Used/LICENSE').read_text() == 'registered by LicenseFixture.Used'
assert (source_publish / 'licenses/LicenseFixture.Used/RENAMED').read_text() == 'renamed notice'
assert not (source_publish / 'licenses/LicenseFixture.Unused').exists()

wrapper = out / 'wrapper'
wrapper.mkdir()
wrapper_project = wrapper / 'Wrapper.csproj'
wrapper_project.write_text(f'''<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup><TargetFramework>net10.0</TargetFramework><PackageId>LicenseFixture.Wrapper</PackageId><Version>{version}</Version></PropertyGroup>
  <ItemGroup>
    <PackageReference Include="LicenseFixture.Used" Version="{version}" />
    <PackageReference Include="SkiaSharp" Version="4.154.0-preview.1.26454.9" />
  </ItemGroup>
</Project>''', encoding='utf-8')
run('pack-transitive-wrapper', 'dotnet', 'pack', str(wrapper_project), '-c', 'Release', '-o', str(feed), '-v:q')

for sdk in ['Doroti.App.Sdk', 'Doroti.Runner.Sdk']:
    sdk_feed = out / sdk / 'feed'
    run('pack-' + sdk, 'dotnet', 'pack', str(ROOT / f'Doroti/src/{sdk}/{sdk}.csproj'),
        '-c', 'Release', '-o', str(sdk_feed), '-v:q')
    extracted = out / sdk / 'extracted'
    with zipfile.ZipFile(next(sdk_feed.glob('*.nupkg'))) as package:
        package.extractall(extracted)
    consumer = out / sdk / 'consumer'
    consumer.mkdir()
    project = consumer / 'Consumer.csproj'
    project.write_text(f'''<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup>
  <ItemGroup>
    <PackageReference Include="LicenseFixture.Wrapper" Version="{version}" />
  </ItemGroup>
  <Import Project="{(extracted / 'Sdk/Doroti.Licenses.targets').as_posix()}" />
</Project>''', encoding='utf-8')
    publish = out / sdk / 'publish'
    run('publish-' + sdk, 'dotnet', 'publish', str(project), '-c', 'Release', '-o', str(publish), '-v:q')
    assert (publish / 'licenses/LicenseFixture.Used/LICENSE').read_text() == 'registered by LicenseFixture.Used'
    assert (publish / 'licenses/LicenseFixture.Used/RENAMED').read_text() == 'renamed notice'
    assert not (publish / 'licenses/nuget').exists()
    assert not (publish / 'licenses/LicenseFixture.Unused').exists()
    assert not (publish / 'licenses/doroti-linux').exists()
    assert not (publish / 'LICENSE').exists()
    state = run('preserve-library-target-' + sdk, 'dotnet', 'msbuild', str(project), '-nologo', '-getProperty:OriginalLibraryTarget')
    assert state.strip() == 'Used', state
    project.write_text(project.read_text().replace('  <ItemGroup>\n    <PackageReference Include="LicenseFixture.Wrapper" Version="' + version + '" />\n  </ItemGroup>\n', ''), encoding='utf-8')
    without = out / sdk / 'publish-without-dependencies'
    run('remove-dependencies-' + sdk, 'dotnet', 'publish', str(project), '-c', 'Release', '-o', str(without), '-v:q')
    assert not (without / 'licenses').exists()

print('PASS library registration, source/NuGet transitivity, preserved targets, no automatic NuGet harvesting and dependency removal.', flush=True)
