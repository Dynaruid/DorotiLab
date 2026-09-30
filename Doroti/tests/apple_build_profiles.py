"""Evaluate real iOS runner profiles without compiling an application.

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

for overrides in [["-p:Configuration=Release"], ["-p:UseInterpreter=false"], ["-p:DorotiCompilationMode=NativeAot"], ["-p:RuntimeIdentifier=ios-arm64"]]:
    result = subprocess.run(['dotnet', 'msbuild', str(PROJECT), '-nologo',
        '-t:ValidateDorotiIosDevelopment', '-p:DorotiIosDevelopment=true',
        '-p:DorotiIosTargetFramework=net10.0-ios27.0', '-p:RuntimeIdentifier=iossimulator-arm64'] + overrides,
        cwd=ROOT, capture_output=True, text=True, timeout=60)
    assert result.returncode != 0 and 'iOS development sessions' in result.stdout, result.stdout + result.stderr
print('PASS development rejects Release, interpreter opt-out, NativeAOT and .NET 10 device sessions')

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
