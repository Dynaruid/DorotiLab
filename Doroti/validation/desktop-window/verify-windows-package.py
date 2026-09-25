"""External package-only Windows MAUI companion/SDK bootstrap and live probe.

Run verify-package.py first to populate the desktop/common dependency feed.
"""
import json
import os
from pathlib import Path
import shutil
import subprocess
import tempfile

ROOT = Path(__file__).resolve().parents[3]
OUT = ROOT / 'Doroti/artifacts/validation/desktop-package'
FEED = OUT / 'feed'
assert (FEED / 'Doroti.Desktop.0.3.0-beta.nupkg').exists(), 'Run verify-package.py first'
consumer = Path(tempfile.mkdtemp(prefix='doroti-desktop-windows-'))

def run(args, cwd=ROOT, env=None):
    result = subprocess.run(args, cwd=cwd, env=env, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True)
    assert result.returncode == 0, result.stdout
    return result.stdout

for name in ['Doroti.Graphics.DirectX', 'Doroti.Skia.RuntimeEffects', 'Doroti.Skia.Rendering', 'Doroti.Skia.Vulkan',
             'Doroti.Host.Maui', 'Doroti.Target.Windows.Maui.win-x64', 'Doroti.App.Sdk', 'Doroti.Runner.Sdk']:
    args = ['dotnet', 'pack', str(ROOT / f'Doroti/src/{name}/{name}.csproj'), '-c', 'Release', '-o', str(FEED), '--nologo', '-v:q']
    if name in ['Doroti.Host.Maui', 'Doroti.Target.Windows.Maui.win-x64']:
        args += ['-p:DorotiHostTargetFrameworks=net10.0-windows10.0.19041.0', '-p:Platform=x64']
    run(args)
    print(f'PACK {name}', flush=True)

for folder in ['desktop', 'windows']:
    (consumer / folder).mkdir()
(consumer / 'NuGet.Config').write_text(f'<configuration><packageSources><clear/><add key="local" value="{FEED}"/><add key="nuget" value="https://api.nuget.org/v3/index.json"/></packageSources></configuration>', encoding='utf-8')
(consumer / 'Probe.csproj').write_text('''<Project Sdk="Doroti.App.Sdk/0.3.0-beta">
<PropertyGroup><RootNamespace>Probe</RootNamespace></PropertyGroup><ItemGroup>
<PackageReference Include="Doroti.Framework.Widgets" Version="0.3.0-beta"/>
</ItemGroup></Project>''', encoding='utf-8')
(consumer / 'Program.cs').write_text('''using Doroti.Hosting;
using Doroti.Ui;
using Doroti.Framework;
using Doroti.Framework.Widgets;
namespace Probe;
public sealed class Program : IDorotiApplicationStartup {
 public void Configure(DorotiApplicationBuilder b) => b.UseView(new("Package consumer", new Size(450, 800)))
 .UseEntrypoint(() => new DorotiWidgetEntrypoint(() => new SizedBox(width: double.PositiveInfinity, height: double.PositiveInfinity,
 child: new ColoredBox(color: new Color(0xff336699)))));
}''', encoding='utf-8')
(consumer / 'desktop/Probe.Desktop.csproj').write_text('''<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup>
<TargetFramework>net10.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable>
</PropertyGroup><ItemGroup><PackageReference Include="Doroti.Desktop" Version="0.3.0-beta"/></ItemGroup></Project>''', encoding='utf-8')
for file in ['DesktopStartup.cs', 'DesktopProbe.cs']:
    shutil.copyfile(ROOT / f'DorotiTestbedApp/desktop/{file}', consumer / 'desktop' / file)
runner = consumer / 'windows/Probe.Windows.csproj'
runner.write_text('''<Project Sdk="Microsoft.NET.Sdk"><Sdk Name="Doroti.Runner.Sdk" Version="0.3.0-beta"/>
<PropertyGroup>
<TargetFramework>net10.0-windows10.0.19041.0</TargetFramework><RuntimeIdentifier>win-x64</RuntimeIdentifier>
<AssemblyName>Probe.Windows</AssemblyName><RootNamespace>DorotiTestbedApp.WinUI</RootNamespace>
<DorotiAppProject>../Probe.csproj</DorotiAppProject><DorotiTarget>Windows</DorotiTarget>
<DorotiDesktopProject>../desktop/Probe.Desktop.csproj</DorotiDesktopProject>
<DorotiDesktopStartupType>DorotiTestbedApp.Desktop.DesktopStartup</DorotiDesktopStartupType>
<DorotiHostKind>Maui</DorotiHostKind><DorotiNativeEntryKind>WinUI-Xaml</DorotiNativeEntryKind><UseMaui>true</UseMaui>
<DefineConstants>$(DefineConstants);WINDOWS;DOROTI_MAUI</DefineConstants>
<SupportedOSPlatformVersion>10.0.19041.0</SupportedOSPlatformVersion><TargetPlatformMinVersion>10.0.19041.0</TargetPlatformMinVersion>
<ApplicationManifest>app.manifest</ApplicationManifest><WindowsPackageType>None</WindowsPackageType>
</PropertyGroup><ItemGroup><PackageReference Include="Doroti.Target.Windows.Maui.win-x64" Version="0.3.0-beta"/></ItemGroup></Project>''', encoding='utf-8')
for file in ['App.xaml', 'App.xaml.cs', 'app.manifest']:
    shutil.copyfile(ROOT / f'DorotiTestbedApp/windows/{file}', consumer / 'windows' / file)
(consumer / 'windows/application-manifest.json').write_text(json.dumps(dict(schemaVersion='doroti.application-capabilities/v1', applicationId='dev.doroti.desktop.probe', targetRid='win-x64', resources=[], plugins=[])), encoding='utf-8')
env = dict(os.environ, NUGET_PACKAGES=str(consumer / 'packages'))
run(['dotnet', 'build', str(runner), '-c', 'Release', '-p:Platform=x64', '--nologo', '-v:q'], cwd=consumer, env=env)
assets = json.loads((consumer / 'windows/obj/project.assets.json').read_text(encoding='utf-8'))
assert not any(v.get('type') == 'project' and 'Doroti' in name for name, v in assets['libraries'].items()), 'Source library reference leaked'
exe = next((consumer / 'windows/bin').rglob('Probe.Windows.exe'))
probe_output = run(['python', str(Path(__file__).with_name('verify-windows.py'))], env=dict(env, DOROTI_MAUI_VALIDATION_EXE=str(exe)))
result = dict(status='PASS', consumer=str(consumer), windowsExecution='PASS', packageOnly=True, probeOutput=probe_output)
(OUT / 'windows-result.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
print(json.dumps(result, indent=2))
