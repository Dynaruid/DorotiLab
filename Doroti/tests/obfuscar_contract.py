"""Exercise real Obfuscar output, project references, opt-out and AOT guards."""
from pathlib import Path
import platform
import subprocess
import sys

ROOT = Path(__file__).resolve().parents[2]
SCRATCH = ROOT / "Doroti/artifacts/obfuscar-contract"
APP_SDK = ROOT / "Doroti/src/Doroti.App.Sdk/Sdk"
RUNNER_RULES = ROOT / "Doroti/src/Doroti.Runner.Sdk/Sdk/Doroti.Obfuscation.targets"


def write(path, text):
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(text, encoding="utf-8")


def run(*args, ok=True):
    result = subprocess.run(args, cwd=ROOT, capture_output=True, text=True,
                            encoding="utf-8", errors="replace", timeout=1200)
    if ok and result.returncode:
        raise AssertionError(result.stdout + result.stderr)
    if not ok:
        assert result.returncode, result.stdout + result.stderr
    return result.stdout + result.stderr


write(SCRATCH / "Directory.Build.props", "<Project />")
write(SCRATCH / "Directory.Build.targets", "<Project />")
app = SCRATCH / "app/Fixture.App.csproj"
write(app, f'''<Project>
  <Import Project="{APP_SDK}/Sdk.props" />
  <PropertyGroup><AssemblyName>Fixture.App</AssemblyName></PropertyGroup>
  <Import Project="{APP_SDK}/Sdk.targets" />
</Project>''')
write(app.parent / "Program.cs", '''using System.Reflection;
namespace Fixture;
public static class Api {
    public static string Read() => new BusinessRule().Calculate();
    public static string ReadByName() => (string)typeof(ReflectedRule)
        .GetMethod("Read")!.Invoke(new ReflectedRule(), null)!;
}
internal sealed class BusinessRule {
    internal string Calculate() => "obfuscar-runtime-ok";
}
[Obfuscation(Exclude = true, ApplyToMembers = true)]
internal sealed class ReflectedRule { public string Read() => "reflection-ok"; }
''')
consumer = SCRATCH / "consumer/Consumer.csproj"
write(consumer, f'''<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup><TargetFramework>net10.0</TargetFramework><OutputType>Exe</OutputType>
    <DorotiAppProject>{app}</DorotiAppProject><DorotiTarget>Windows</DorotiTarget>
  </PropertyGroup>
  <ItemGroup><ProjectReference Include="{app}" /></ItemGroup>
  <Import Project="{RUNNER_RULES}" />
</Project>''')
write(consumer.parent / "Program.cs", '''using System;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using Fixture;
Console.WriteLine(Api.Read());
Console.WriteLine(Api.ReadByName());
Console.WriteLine("originalInternalType=" + (typeof(Api).Assembly.GetType("Fixture.BusinessRule") != null));
Console.WriteLine("originalInternalMethod=" + typeof(Api).Assembly.GetTypes()
    .SelectMany(t => t.GetMethods(System.Reflection.BindingFlags.Public |
        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
    .Any(m => m.Name == "Calculate"));
using var pe = new PEReader(File.OpenRead(typeof(Api).Assembly.Location));
var native = pe.PEHeaders.CorHeader!.ManagedNativeHeaderDirectory;
Console.WriteLine("readyToRunImage=" + (native.Size > 0 &&
    pe.GetSectionData(native.RelativeVirtualAddress).GetReader().ReadUInt32() == 0x00525452));
''')
base = ["dotnet", "build", str(consumer), "-c", "Release", "--nologo", "-p:UseSharedCompilation=false"]

enabled = run(*base, "-p:DorotiObfuscate=true")
print("Obfuscar build completed", flush=True)
assert "Doroti Obfuscar: Fixture.App.dll" in enabled, enabled
output = consumer.parent / "bin/Release/net10.0/Consumer.dll"
runtime = run("dotnet", str(output))
assert "obfuscar-runtime-ok" in runtime and "reflection-ok" in runtime, runtime
assert "originalInternalType=False" in runtime and "originalInternalMethod=False" in runtime, runtime
assert "readyToRunImage=False" in runtime, runtime
obfuscated = app.parent / "bin/Release/obfuscated/net10.0/Fixture.App.dll"
assert obfuscated.is_file()
assert not obfuscated.with_suffix(".pdb").exists()
assert (app.parent / "obj/Release/obfuscated/net10.0/Doroti.Obfuscar/mapping.xml").is_file()
print("Metadata, public API and reflection exclusion: PASS", flush=True)

# Repeating an incremental build must still consume the obfuscated DLL.
run(*base, "-p:DorotiObfuscate=true")
assert "originalInternalType=False" in run("dotnet", str(output))

# Turning off protection must restore the ordinary DLL without a clean/rebuild.
run(*base, "-p:DorotiObfuscate=false")
assert "originalInternalType=True" in run("dotnet", str(output))
print("Incremental opt-in/out: PASS", flush=True)

# A global opt-in must not leak through a project reference into an AOT app.
skipped = run(*base, "-p:DorotiObfuscate=true", "-p:AotAssemblies=true")
assert "Doroti Obfuscar skipped: AOT" in skipped, skipped
assert "Doroti Obfuscar: Fixture.App.dll" not in skipped, skipped
assert "originalInternalType=True" in run("dotnet", str(output))

# Evaluate the common runner policy for every platform, without claiming a
# native build on hosts that do not have those toolchains.
cases = [(target, [], target not in ("iOS", "MacCatalyst"))
         for target in ("Windows", "Android", "Web", "Linux", "macOS", "iOS", "MacCatalyst")]
cases += [("Web", [flag], False) for flag in (
    "PublishAot=true", "RunAOTCompilation=true",
    "AndroidEnableProfiledAot=true", "_RunAotCompiler=true", "AotAssemblies=true",
    "DorotiCompilationMode=NativeAot")]
cases += [(target, ["UseMonoRuntime=false"], True) for target in ("iOS", "MacCatalyst")]
cases += [("Windows", ["PublishReadyToRun=true"], True),
          ("Windows", ["PublishReadyToRun=true", "PublishAot=true"], False)]
for target, flags, expected in cases:
    args = ["dotnet", "msbuild", str(consumer), "-nologo", "-t:DorotiConfigureApplicationObfuscation",
            "-p:Configuration=Release", "-p:DorotiObfuscate=true", f"-p:DorotiTarget={target}",
            "-getProperty:DorotiApplicationObfuscationEnabled"]
    result = run(*args, *(f"-p:{flag}" for flag in flags))
    assert result.strip().endswith(str(expected).lower()), (target, flags, result)

debug = run("dotnet", "msbuild", str(consumer), "-nologo", "-t:DorotiConfigureApplicationObfuscation",
            "-p:Configuration=Debug", "-p:DorotiObfuscate=true", "-getProperty:DorotiApplicationObfuscationEnabled")
assert debug.strip().endswith("false"), debug
print("Platform policy and AOT guards: PASS", flush=True)

artifacts = SCRATCH / "custom-artifacts"
run(*base, "-p:DorotiObfuscate=true", f"-p:ArtifactsPath={artifacts}")
assert (artifacts / "bin/Fixture.App/release_obfuscated/Fixture.App.dll").is_file()
assert "originalInternalType=False" in run("dotnet", str(artifacts / "bin/Consumer/release/Consumer.dll"))

no_build = run("dotnet", "publish", str(consumer), "-c", "Release", "--no-build",
               "-p:DorotiObfuscate=true", ok=False)
assert "DOROTIOBF003" in no_build, no_build

# R2R must compile the protected IL, rather than replacing it with the original
# DLL or silently falling back to an IL-only publish. Inspect the final PE header
# and run the published app; then turn protection off without cleaning.
architecture = "arm64" if platform.machine().lower() in ("arm64", "aarch64") else "x64"
rid = {"win32": "win", "linux": "linux", "darwin": "osx"}[sys.platform] + "-" + architecture
r2r_output = SCRATCH / "r2r-publish"
r2r_command = ["dotnet", "publish", str(consumer), "-c", "Release", "-r", rid,
               "--self-contained", "false", "-p:PublishReadyToRun=true",
               "-p:UseSharedCompilation=false", "-o", str(r2r_output), "--nologo"]
for protect in (True, False, True):
    published = run(*r2r_command, f"-p:DorotiObfuscate={str(protect).lower()}")
    assert ("Doroti Obfuscar: Fixture.App.dll" in published) == protect, published
    runtime = run("dotnet", str(r2r_output / "Consumer.dll"))
    assert "readyToRunImage=True" in runtime, runtime
    assert f"originalInternalType={not protect}" in runtime, runtime
    assert f"originalInternalMethod={not protect}" in runtime, runtime
    assert "obfuscar-runtime-ok" in runtime and "reflection-ok" in runtime, runtime
    assert not (r2r_output / "mapping.xml").exists()
    assert not (r2r_output / "obfuscar.xml").exists()
    if protect:
        assert not (r2r_output / "Fixture.App.pdb").exists()
    print(f"ReadyToRun ({rid}, protected={protect}): header and runtime PASS", flush=True)
print("ReadyToRun publish header, runtime, reflection exclusion and opt-out: PASS", flush=True)

write(app.parent / "bad-rules.xml", "<ObfuscarRules><Var name='KeepPublicApi' value='false'/></ObfuscarRules>")
failed = run(*base, "-p:DorotiObfuscate=true", f"-p:DorotiObfuscarRules={app.parent / 'bad-rules.xml'}", ok=False)
assert "DOROTIOBF001" in failed, failed

print("PASS: real metadata rename, runtime, reflection exclusion, incremental opt-in/out, ReadyToRun publish, AOT guards and invalid-rule failure")
print("Platform policy evaluation only; native devices and platform packaging are not verified here.")
