"""Build and run an external Mac Catalyst package-only desktop consumer.

Run verify-package.py first. Invoke this script through run-with-timeout.py.
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
consumer = Path(tempfile.mkdtemp(prefix='doroti-desktop-catalyst-'))
TFM = os.environ.get('DOROTI_CATALYST_PACKAGE_TFM', 'net10.0-maccatalyst27.0')

def run(args, cwd=ROOT, env=None):
    result = subprocess.run(args, cwd=cwd, env=env, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True)
    assert result.returncode == 0, result.stdout
    return result.stdout

for name in ['Doroti.Skia.RuntimeEffects', 'Doroti.Skia.Rendering', 'Doroti.Host.Maui',
             'Doroti.Target.MacCatalyst.Maui.maccatalyst-arm64', 'Doroti.App.Sdk', 'Doroti.Runner.Sdk']:
    args = ['dotnet', 'pack', str(ROOT / f'Doroti/src/{name}/{name}.csproj'), '-c', 'Release', '-o', str(FEED), '--nologo', '-v:q']
    if name in ['Doroti.Host.Maui', 'Doroti.Target.MacCatalyst.Maui.maccatalyst-arm64']:
        args += ['-p:RuntimeIdentifier=maccatalyst-arm64', f'-p:DorotiMacCatalystTargetFramework={TFM}']
    run(args)
    print(f'PACK {name}', flush=True)
for folder in ['desktop', 'macos']:
    (consumer / folder).mkdir()
shutil.copyfile(ROOT / 'global.json', consumer / 'global.json')
(consumer / 'NuGet.Config').write_text(f'<configuration><packageSources><clear/><add key="local" value="{FEED}"/><add key="nuget" value="https://api.nuget.org/v3/index.json"/></packageSources></configuration>')
(consumer / 'Probe.csproj').write_text('''<Project Sdk="Doroti.App.Sdk/0.3.0-beta">
<PropertyGroup><RootNamespace>Probe</RootNamespace></PropertyGroup><ItemGroup>
<PackageReference Include="Doroti.Framework.Widgets" Version="0.3.0-beta"/>
</ItemGroup></Project>''')
(consumer / 'Program.cs').write_text('''using Doroti.Hosting;
using Doroti.Ui;
using Doroti.Framework;
using Doroti.Framework.Widgets;
namespace Probe;
public sealed class Program : IDorotiApplicationStartup {
 public void Configure(DorotiApplicationBuilder b) => b.UseView(new("Package consumer", new Size(450, 800)))
 .UseEntrypoint(() => new DorotiWidgetEntrypoint(() => new SizedBox(width: double.PositiveInfinity, height: double.PositiveInfinity,
 child: new ColoredBox(color: new Color(0xff336699)))));
}''')
(consumer / 'desktop/Probe.Desktop.csproj').write_text('''<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup>
<TargetFramework>net10.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable>
</PropertyGroup><ItemGroup><PackageReference Include="Doroti.Desktop" Version="0.3.0-beta"/></ItemGroup></Project>''')
for name in ['MacCatalystDesktopStartup.cs']:
    shutil.copyfile(ROOT / 'DorotiTestbedApp/desktop' / name, consumer / 'desktop' / name)
runner = consumer / 'macos/Probe.MacCatalyst.csproj'
runner.write_text(f'''<Project Sdk="Microsoft.NET.Sdk"><Sdk Name="Doroti.Runner.Sdk" Version="0.3.0-beta"/>
<PropertyGroup>
<TargetFramework>{TFM}</TargetFramework><RuntimeIdentifier>maccatalyst-arm64</RuntimeIdentifier>
<AssemblyName>DorotiTestbedApp.MacCatalyst</AssemblyName><RootNamespace>DorotiTestbedApp.MacCatalyst</RootNamespace>
<DorotiAppProject>../Probe.csproj</DorotiAppProject><DorotiTarget>MacCatalyst</DorotiTarget>
<DorotiDesktopProject>../desktop/Probe.Desktop.csproj</DorotiDesktopProject>
<DorotiDesktopStartupType>DorotiTestbedApp.Desktop.MacCatalystDesktopStartup</DorotiDesktopStartupType>
<DorotiHostKind>Maui</DorotiHostKind><DorotiNativeEntryKind>UIKit-Main</DorotiNativeEntryKind><UseMaui>true</UseMaui>
<DefineConstants>$(DefineConstants);MACCATALYST;DOROTI_MAUI</DefineConstants>
<SupportedOSPlatformVersion>17.0</SupportedOSPlatformVersion>
<ApplicationManifest>Info.plist</ApplicationManifest><AppManifest>Info.plist</AppManifest><AppBundleManifest>Info.plist</AppBundleManifest>
<ApplicationTitle>Doroti Package Probe</ApplicationTitle><ApplicationId>dev.doroti.desktop.probe</ApplicationId>
</PropertyGroup><ItemGroup><PackageReference Include="Doroti.Target.MacCatalyst.Maui.maccatalyst-arm64" Version="0.3.0-beta"/>
<DorotiNativeBindingProject Include="binding/DorotiTestbedApp.MacCatalyst.Native.csproj"/>
</ItemGroup></Project>''')
(consumer / 'macos/AppDelegate.cs').write_text('''using Foundation;
using Doroti.Host.Maui;
namespace DorotiTestbedApp.MacCatalyst;
[Register("DorotiDesktopCatalystPackageDelegate")]
public sealed class AppDelegate : DorotiMauiUIApplicationDelegate {
 protected override Doroti.Hosting.DorotiApplicationDescriptor CreateApplicationDescriptor() => Doroti.Generated.DorotiBootstrap.Create([]);
 public override bool FinishedLaunching(UIKit.UIApplication application, NSDictionary? options) {
  var result = base.FinishedLaunching(application, options);
  _ = MacCatalystDesktopEvidence.RunAsync();
  return result;
 }
}''')
shutil.copyfile(ROOT / 'DorotiTestbedApp/macos/MacCatalystDesktopEvidence.cs', consumer / 'macos/MacCatalystDesktopEvidence.cs')
shutil.copyfile(ROOT / 'DorotiTestbedApp/macos/DorotiNativePlatformBridge.cs', consumer / 'macos/DorotiNativePlatformBridge.cs')
shutil.copyfile(ROOT / 'DorotiTestbedApp/macos/Info.plist', consumer / 'macos/Info.plist')
for folder in ['binding', 'native']:
    shutil.copytree(ROOT / 'DorotiTestbedApp/macos' / folder, consumer / 'macos' / folder,
                    ignore=shutil.ignore_patterns('bin', 'obj', '.DS_Store', 'xcuserdata'))
(consumer / 'macos/application-manifest.json').write_text(json.dumps(dict(schemaVersion='doroti.application-capabilities/v1', applicationId='dev.doroti.desktop.probe', targetRid='maccatalyst-arm64', resources=[], plugins=[])))
env = dict(os.environ, NUGET_PACKAGES=str(consumer / 'packages'))
run(['dotnet', 'build', str(runner), '-c', 'Release', f'-p:DorotiMacCatalystTargetFramework={TFM}', '--nologo', '-v:q'], cwd=consumer, env=env)
assets = json.loads((consumer / 'macos/obj/project.assets.json').read_text())
assert not any(v.get('type') == 'project' and name.startswith('Doroti.') for name, v in assets['libraries'].items()), 'Source library reference leaked'
app = next((consumer / 'macos/bin').rglob('Doroti Package Probe.app'))
probe_output = run(['python3', str(Path(__file__).with_name('verify-maccatalyst.py')), '--app', str(app), '--output', str(consumer / 'probe')], env=env)
result = dict(status='PASS', consumer=str(consumer), catalystExecution='PASS', packageOnly=True, tfm=TFM, probeOutput=probe_output)
(OUT / 'maccatalyst-result.json').write_text(json.dumps(result, indent=2))
print(json.dumps(result, indent=2))
