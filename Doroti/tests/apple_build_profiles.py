"""Evaluate real iOS runner profiles without compiling an application.

Run on the pinned Apple toolchain via eng/run-with-timeout.py --timeout 1200.
"""
from pathlib import Path
import subprocess

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
