"""Evaluate real Apple runner profiles without compiling an application.

Run on the pinned Apple toolchain via eng/run-with-timeout.py --timeout 1200.
"""
from pathlib import Path
import json
import argparse
import tempfile
import subprocess

parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--device-sdk', help='Installed .NET 11 SDK to evaluate the device development profile')
args = parser.parse_args()

ROOT = Path(__file__).resolve().parents[2]
PROJECT = ROOT / "samples/DorotiSampleApp2/ios/DorotiSampleApp2.iOS.csproj"
BASE = [
    "dotnet", "msbuild", str(PROJECT), "-nologo", "-getProperty:MtouchInterpreter",
    "-p:DorotiIosTargetFramework=net10.0-ios27.0",
]
CASES = [
    ("device", "Debug", "ios-arm64", [], "-all,DorotiSampleApp2,DorotiSampleApp2.iOS"),
    ("simulator", "Debug", "iossimulator-arm64", [], "all,-Doroti.Host.Maui"),
    ("explicit", "Debug", "ios-arm64", ["-p:MtouchInterpreter=all"], "all"),
    ("custom-app", "Debug", "ios-arm64", ["-p:DorotiIosDebugInterpretedAssemblies=CustomApp"], "-all,CustomApp"),
    ("disabled", "Debug", "ios-arm64", ["-p:UseInterpreter=false"], "-all"),
    ("release-mono", "Release", "ios-arm64", ["-p:DorotiCompilationMode=Mono"], "-all"),
    ("native-aot", "Debug", "ios-arm64", ["-p:DorotiCompilationMode=NativeAot"], ""),
    ("simulator-development", "Debug", "iossimulator-arm64", ["-p:DorotiIosDevelopment=true"], "all,-Doroti.Host.Maui"),
]
for name, configuration, rid, overrides, expected in CASES:
    result = subprocess.run(
        BASE + [f"-p:Configuration={configuration}", f"-p:RuntimeIdentifier={rid}"] + overrides,
        cwd=ROOT, capture_output=True, text=True, check=True, timeout=60,
    )
    actual = result.stdout.strip()
    assert actual == expected, (name, actual, expected, result.stderr)
    print(f"PASS {name}: {actual!r}")

profile = ROOT / "Doroti/src/Doroti.Runner.Sdk/Sdk/Doroti.IosNativeAot.props"
template = ROOT / "Doroti/templates/Doroti.Templates/content/doroti-app/ios/Doroti.IosNativeAot.props"
assert profile.read_text() == template.read_text(), "Template iOS profile differs from SDK profile."
print("PASS template profile parity")
assert (ROOT / "samples/DorotiTestbedApp/ios/AppleFeatureProbe.cs").read_bytes() == (ROOT / "samples/DorotiTestbedApp/macos/AppleFeatureProbe.cs").read_bytes(), "Apple feature probes differ between runner directories."
print("PASS Apple feature probe parity")

for overrides in [["-p:Configuration=Release"], ["-p:UseInterpreter=false"], ["-p:DorotiCompilationMode=NativeAot"], ["-p:RuntimeIdentifier=ios-arm64"], ["-p:RuntimeIdentifier=ios-arm64", "-p:MtouchInterpreter=-all", "-p:DOROTI_DEV_HOTRELOAD_ENDPOINT=ws://192.168.1.2:5678"]]:
    result = subprocess.run(['dotnet', 'msbuild', str(PROJECT), '-nologo',
        '-t:ValidateDorotiIosDevelopment', '-p:DorotiIosDevelopment=true',
        '-p:DorotiIosTargetFramework=net10.0-ios27.0', '-p:RuntimeIdentifier=iossimulator-arm64'] + overrides,
        cwd=ROOT, capture_output=True, text=True, timeout=60)
    assert result.returncode != 0 and 'iOS development sessions' in result.stdout, result.stdout + result.stderr
print('PASS development rejects Release, interpreter opt-out, NativeAOT and unconfigured/mixed-AOT .NET 10 device sessions')

network_base = ['dotnet', 'msbuild', str(PROJECT), '-nologo',
    '-p:Configuration=Debug', '-p:DorotiIosDevelopment=true', '-p:DorotiCompilationMode=Mono',
    '-p:DorotiIosTargetFramework=net10.0-ios27.0', '-p:RuntimeIdentifier=ios-arm64',
    '-p:DOROTI_DEV_HOTRELOAD_ENDPOINT=ws://192.168.1.2:5678']
items = json.loads(subprocess.check_output(network_base + ['-getItem:_DorotiIosHotReloadAgent,BundleResource'],
    cwd=ROOT, text=True, timeout=60))['Items']
agent = items['_DorotiIosHotReloadAgent'][0]['Identity']
assert Path(agent).is_file(), agent
assert any(item['Identity'] == agent and item['LogicalName'] == 'Microsoft.Extensions.DotNetDeltaApplier.dll'
           for item in items['BundleResource']), items
result = subprocess.run(network_base + [
    '-t:ValidateDorotiIosDevelopment;ValidateDorotiCompilationMode;DorotiConfigureDeviceHotReloadNetwork',
    '-p:RunArguments=--setenv=DOTNET_STARTUP_HOOKS=' + agent + ' --setenv=DOTNET_WATCH_HOTRELOAD_WEBSOCKET_ENDPOINT=ws://localhost:123 --setenv=DOTNET_WATCH_HOTRELOAD_WEBSOCKET_KEY=key --hotreload-url=ws://localhost:123 --hotreload-connection-mode=usb --',
    '-getProperty:UseMonoRuntime,MtouchInterpreter,MtouchLink,StartupHookSupport,RunArguments'],
    cwd=ROOT, capture_output=True, text=True, check=True, timeout=60)
network_profile = json.loads(result.stdout)['Properties']
assert network_profile['UseMonoRuntime'] == 'true', network_profile
assert network_profile['MtouchInterpreter'] == 'all,-Doroti.Host.Maui', network_profile
assert network_profile['MtouchLink'] == 'None' and network_profile['StartupHookSupport'] == 'true', network_profile
launch = network_profile['RunArguments']
assert '--hotreload-' not in launch and 'ws://localhost' not in launch, launch
assert '--setenv=DOTNET_WATCH_HOTRELOAD_WEBSOCKET_ENDPOINT=ws://192.168.1.2:5678' in launch, launch
assert '--setenv=DOTNET_WATCH_HOTRELOAD_WEBSOCKET_KEY=key' in launch, launch
assert '--setenv=DOTNET_STARTUP_HOOKS=Microsoft.Extensions.DotNetDeltaApplier ' in launch, launch
assert agent not in launch, launch
print('PASS .NET 10 device Mono/interpreter and SDK agent network launch')

if args.device_sdk:
    (ROOT / 'temp/testing').mkdir(parents=True, exist_ok=True)
    with tempfile.TemporaryDirectory(dir=ROOT / 'temp/testing', prefix='ios-device-profile-') as directory:
        Path(directory, 'global.json').write_text(json.dumps({'sdk': {
            'version': args.device_sdk, 'rollForward': 'disable', 'allowPrerelease': True}}))
        result = subprocess.run(['dotnet', 'msbuild', str(PROJECT), '-nologo',
            '-t:DorotiAlignIosDevelopmentCrossgen;ValidateDorotiIosDevelopment;ValidateDorotiCompilationMode',
            '-p:Configuration=Debug', '-p:DorotiIosDevelopment=true', '-p:DorotiCompilationMode=CoreClr',
            '-p:DorotiIosTargetFramework=net11.0-ios', '-p:RuntimeIdentifier=ios-arm64',
            '-getProperty:UseMonoRuntime,MtouchInterpreter,PublishAot,Registrar,DorotiIosMauiVersion,DorotiIosDevelopmentRuntimeVersion', '-getItem:KnownCrossgen2Pack,FrameworkReference'],
            cwd=directory, capture_output=True, text=True, check=True, timeout=60)
        result_json = json.loads(result.stdout)
        profile = result_json['Properties']
        assert profile['UseMonoRuntime'] == 'false', profile
        assert profile['MtouchInterpreter'] == '', profile
        assert profile['PublishAot'] != 'true', profile
        assert profile['Registrar'] == 'partial-static', profile
        assert profile['DorotiIosMauiVersion'].startswith('11.'), profile
        assert profile['DorotiIosDevelopmentRuntimeVersion'] == '11.0.0-rc.2.26478.114', profile
        for reference in result_json['Items']['FrameworkReference']:
            if reference['Identity'] == 'Microsoft.NETCore.App':
                assert reference['RuntimeFrameworkVersion'] == profile['DorotiIosDevelopmentRuntimeVersion']
            elif reference['Identity'].startswith('Microsoft.iOS'):
                assert 'RuntimeFrameworkVersion' not in reference, reference
        for pack in result_json['Items']['KnownCrossgen2Pack']:
            if pack['TargetFramework'] == 'net11.0':
                assert pack['Crossgen2PackVersion'] == profile['DorotiIosDevelopmentRuntimeVersion'], pack
    print('PASS .NET 11 device CoreCLR/untrimmed-compatible registrar profile')


for platform, suffix, property_name, runtime, interpreter in [
    ('macos', 'MacOS', 'DorotiMacOSTargetFramework', 'false', ''),
    ('maccatalyst', 'MacCatalyst', 'DorotiMacCatalystTargetFramework', 'true', 'all,-Doroti.Host.Maui'),
]:
    project = ROOT / f'samples/DorotiTestbedApp/macos/DorotiTestbedApp.{suffix}.csproj'
    base = ['dotnet', 'msbuild', str(project), '-nologo',
            '-p:' + property_name + '=net10.0-' + platform + '27.0']
    query = ['-getProperty:UseMonoRuntime,MtouchInterpreter,TrimMode,RunWithOpen,StartupHookSupport,LinkMode,MtouchLink']
    development = ['-p:DorotiMacDevelopment=true', '-p:Configuration=Debug']
    profile = json.loads(subprocess.check_output(base + development + query,
        cwd=ROOT, text=True, timeout=60))['Properties']
    assert profile['UseMonoRuntime'] == runtime, profile
    assert profile['MtouchInterpreter'] == interpreter, profile
    assert profile['TrimMode'] == 'copy' and profile['RunWithOpen'] == 'false', profile
    assert profile['StartupHookSupport'] == 'true', profile
    assert profile['LinkMode'] == 'None' and profile['MtouchLink'] == 'None', profile
    capabilities = json.loads(subprocess.check_output(base + development + ['-getItem:ProjectCapability'],
        cwd=ROOT, text=True, timeout=60))['Items']['ProjectCapability']
    assert ('HotReloadWebSockets' in [item['Identity'] for item in capabilities]) == (platform == 'macos'), capabilities
    for configuration in ['Debug', 'Release']:
        normal = json.loads(subprocess.check_output(base + ['-p:Configuration=' + configuration] + query,
            cwd=ROOT, text=True, timeout=60))['Properties']
        assert normal['RunWithOpen'] != 'false', normal
        assert normal['MtouchInterpreter'] == ('-all' if platform == 'maccatalyst' else ''), normal
    for invalid in ['-p:Configuration=Release', '-p:PublishAot=true', '-p:Optimize=true', '-p:StartupHookSupport=false',
                    '-p:TrimMode=full', '-p:' + ('LinkMode' if platform == 'macos' else 'MtouchLink') + '=Full'] + (
            ['-p:UseInterpreter=false', '-p:MtouchInterpreter=-all'] if platform == 'maccatalyst' else []):
        result = subprocess.run(base + development + ['-t:ValidateDorotiMacDevelopment', invalid],
            cwd=ROOT, capture_output=True, text=True, timeout=60)
        assert result.returncode != 0 and 'development' in result.stdout, result.stdout + result.stderr
    print(f'PASS {platform} development profile, invalid modes rejected, ordinary Debug/Release unchanged')
