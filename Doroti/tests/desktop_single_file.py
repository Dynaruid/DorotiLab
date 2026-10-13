"""Publish isolated bundles with adjacent licenses; verify extraction and no-build updates."""
from pathlib import Path
import argparse
import hashlib
import json
import os
import platform
import shutil
import subprocess

ROOT = Path(__file__).resolve().parents[2]
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument("--output", required=True, type=Path)
args = parser.parse_args()
out = args.output.resolve()
assert out.is_relative_to(ROOT / "temp/testing")
out.mkdir(parents=True, exist_ok=False)
build = ROOT / "packages/platforms/build"
native = out / "native"
native.mkdir()
notices = native / "licenses"
shutil.copytree(ROOT / "packages/platforms/qt/native/licenses", notices)
(notices / "NOTICE.txt").write_text("payload-v1", encoding="utf-8")
(out / "APP-LICENSE.txt").write_text("developer-selected license", encoding="utf-8")
payload = ["libdoroti_qt_host.so", "libdoroti_texture_gstreamer.so",
           "libdoroti_webview_qt.so", "doroti-webview-runtime.json"]
for name in payload:
    (native / name).write_text("payload-v1", encoding="utf-8")
for name in ("Directory.Build.props", "Directory.Build.targets"):
    (out / name).write_text("<Project />", encoding="utf-8")
library = out / "library"
library.mkdir()
(library / "PhysicalLibrary.csproj").write_text(f'''<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup><TargetFramework>net10.0</TargetFramework><AssemblyName>SkiaSharp</AssemblyName><TreatWarningsAsErrors>true</TreatWarningsAsErrors></PropertyGroup>
  <ItemGroup><DorotiLicense Include="{(ROOT / 'Doroti/LICENSE').as_posix()}" TargetPath="licenses/PhysicalLibrary/LICENSE" /></ItemGroup>
  <ItemGroup><DorotiLicense Include="{notices.as_posix()}/**/*" Exclude="{notices.as_posix()}/LGPL-2.1.txt" TargetPath="licenses/doroti-linux/%(RecursiveDir)%(Filename)%(Extension)" /></ItemGroup>
  <Import Project="{(ROOT / 'Doroti/eng/Doroti.LicenseRegistration.targets').as_posix()}" />
  <Target Name="RejectLibrarySingleFile" BeforeTargets="PrepareForBuild" Condition="'$(PublishSingleFile)' == 'true'">
    <Error Text="The executable's PublishSingleFile leaked into a referenced library." />
  </Target>
</Project>''', encoding="utf-8")
(library / "PhysicalLibrary.cs").write_text('''public static class PhysicalLibrary {
    public static string Path => typeof(PhysicalLibrary).Assembly.Location;
}''', encoding="utf-8")
rid = {"Windows": "win-x64", "Linux": "linux-x64"}[platform.system()]
exe_name = "BundleFixture" + (".exe" if platform.system() == "Windows" else "")
project = out / "BundleFixture.csproj"
project.write_text(f'''<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework><OutputType>Exe</OutputType>
    <ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <RuntimeIdentifier>{rid}</RuntimeIdentifier><SelfContained>false</SelfContained>
    <PublishSingleFile>true</PublishSingleFile><DorotiTarget>Linux</DorotiTarget>
    <DorotiHostKind>Qt</DorotiHostKind><DorotiQtWebEngine>true</DorotiQtWebEngine>
    <DorotiGStreamerTextures>true</DorotiGStreamerTextures>
    <DorotiQtNativeBuildDirectory>{native.as_posix()}</DorotiQtNativeBuildDirectory>
    <DorotiQtNativeSourceDirectory>{native.as_posix()}</DorotiQtNativeSourceDirectory>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="Program.cs" /><ProjectReference Include="library/PhysicalLibrary.csproj" />
    <None Update="APP-LICENSE.txt" Link="licenses/app/LICENSE" CopyToOutputDirectory="PreserveNewest" CopyToPublishDirectory="PreserveNewest" ExcludeFromSingleFile="true" />
  </ItemGroup>
  <Import Project="{build.as_posix()}/Doroti.SingleFile.targets" />
  <Import Project="{build.as_posix()}/Doroti.Qt.targets" />
  <Import Project="{(ROOT / 'Doroti/src/Doroti.App.Sdk/Sdk/Doroti.Licenses.targets').as_posix()}" />
</Project>''', encoding="utf-8")
(out / "Program.cs").write_text('''using System.Reflection;
using System.Text.Json;
var expected = args.Single();
if (!File.Exists(PhysicalLibrary.Path)) throw new Exception("Referenced DLL provenance was lost.");
foreach (var name in new[] { "libdoroti_qt_host.so", "libdoroti_texture_gstreamer.so",
    "libdoroti_webview_qt.so", "doroti-webview-runtime.json" })
    if (File.ReadAllText(Path.Combine(AppContext.BaseDirectory, name)) != expected)
        throw new Exception("Missing or stale bundle asset: " + name);
var licenses = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath!)!, "licenses");
foreach (var name in new[] { "README.md", "LGPL-3.txt", "GPL-3.txt", "LGPL-2.1.txt", "Wayland-background-effect-MIT.txt" })
    if (!File.Exists(Path.Combine(licenses, "doroti-linux", name)))
        throw new Exception("Missing Qt notice: " + name);
if (!File.Exists(Path.Combine(licenses, "PhysicalLibrary/LICENSE")))
    throw new Exception("Missing referenced library's registered license.");
if (Directory.Exists(Path.Combine(licenses, "Doroti.Runtime")) || Directory.Exists(Path.Combine(licenses, "Doroti.Skia.Fonts")))
    throw new Exception("Unused framework libraries added licenses.");
if (File.ReadAllText(Path.Combine(licenses, "doroti-linux/NOTICE.txt")) != expected)
    throw new Exception("Stale Qt notice in no-build publish.");
if (File.ReadAllText(Path.Combine(licenses, "app/LICENSE")) != "developer-selected license")
    throw new Exception("The developer-selected license was not published.");
if (Directory.Exists(Path.Combine(AppContext.BaseDirectory, "licenses")))
    throw new Exception("Licenses should be readable beside the executable without bundle extraction.");
#pragma warning disable IL3000 // The tested profile extracts every managed assembly.
var location = Assembly.GetExecutingAssembly().Location;
#pragma warning restore IL3000
if (!File.Exists(location) || Path.GetDirectoryName(location) != Path.TrimEndingDirectorySeparator(AppContext.BaseDirectory))
    throw new Exception("Full extraction did not preserve physical managed assembly provenance.");
var libraryHash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(PhysicalLibrary.Path))).ToLowerInvariant();
using var pe = new System.Reflection.PortableExecutable.PEReader(File.OpenRead(location));
var readyToRun = pe.PEHeaders.CorHeader!.ManagedNativeHeaderDirectory.Size > 0;
Console.WriteLine(JsonSerializer.Serialize(new { location, baseDirectory = AppContext.BaseDirectory, expected, libraryHash, readyToRun }));
''', encoding="utf-8")


def run(name, *command, error=None, env=None):
    result = subprocess.run(command, cwd=ROOT, capture_output=True, text=True,
                            encoding="utf-8", errors="replace", timeout=1200, env=env)
    output = result.stdout + result.stderr
    (out / (name + ".log")).write_text(output, encoding="utf-8")
    if error:
        assert result.returncode and error in output, output
    else:
        assert result.returncode == 0, output
    print("PASS", name, flush=True)
    return output


def verify(name, publish, expected, ready_to_run=False):
    assert sorted(p.name for p in publish.iterdir()) == [exe_name, "licenses"], list(publish.iterdir())
    assert not (publish / 'licenses/nuget').exists()
    isolated = out / (name + "-isolated")
    isolated.mkdir()
    executable = isolated / exe_name
    shutil.copy2(publish / exe_name, executable)
    shutil.copytree(publish / "licenses", isolated / "licenses")
    runtime = run(name, str(executable), expected, env=os.environ | {
        "DOTNET_BUNDLE_EXTRACT_BASE_DIR": str(out / "extracted")})
    state = json.loads(runtime)
    assert Path(state["baseDirectory"]).is_relative_to(out / "extracted"), state
    assert Path(state["location"]).is_file(), state
    original = library / "bin/Release/net10.0/SkiaSharp.dll"
    assert state["libraryHash"] == hashlib.sha256(original.read_bytes()).hexdigest(), state
    assert state["readyToRun"] == ready_to_run, state


publish = out / "publish"
run("publish", "dotnet", "publish", str(project), "-c", "Release", "-p:PublishSingleFile=true", "-o", str(publish), "-v:q")
verify("run-with-licenses", publish, "payload-v1")
for name in payload:
    (native / name).write_text("payload-v2", encoding="utf-8")
(notices / "NOTICE.txt").write_text("payload-v2", encoding="utf-8")
second = out / "publish-no-build"
run("publish-no-build", "dotnet", "publish", str(project), "-c", "Release", "--no-build", "-p:PublishSingleFile=true",
    "-o", str(second), "-v:q")
verify("run-no-build-with-licenses", second, "payload-v2")
ready_to_run = out / "publish-r2r"
run("publish-r2r", "dotnet", "publish", str(project), "-c", "Release", "-p:PublishSingleFile=true",
    "--self-contained", "true", "-p:PublishReadyToRun=true",
    "-o", str(ready_to_run), "-v:q")
verify("run-r2r-with-licenses", ready_to_run, "payload-v2", ready_to_run=True)
without_gstreamer = out / "publish-without-gstreamer"
run("publish-without-gstreamer", "dotnet", "publish", str(project), "-c", "Release", "--no-build",
    "-p:DorotiGStreamerTextures=false", "-o", str(without_gstreamer), "-v:q")
assert not (without_gstreamer / "licenses/doroti-linux/LGPL-2.1.txt").exists()
assert (without_gstreamer / "licenses/doroti-linux/LGPL-3.txt").is_file()
(native / "libdoroti_webview_qt.so").unlink()
run("reject-missing-no-build-asset", "dotnet", "publish", str(project), "-c", "Release",
    "--no-build", "-o", str(out / "missing"), "-v:q", error="DOROTIQT006")
for name, switches, error in [
    ("no-full-extraction", ["IncludeAllContentForSelfExtract=false"], "DOROTISINGLE002"),
    ("no-native-extraction", ["IncludeNativeLibrariesForSelfExtract=false"], "DOROTISINGLE002"),
    ("trimming", ["PublishTrimmed=true"], "DOROTISINGLE003"),
    ("native-aot", ["PublishAot=true"], "DOROTISINGLE003"),
    ("composite-r2r", ["PublishReadyToRun=true", "PublishReadyToRunComposite=true"], "DOROTISINGLE005"),
    ("macos", ["DorotiTarget=macOS"], "DOROTISINGLE001"),
    ("maccatalyst", ["DorotiTarget=MacCatalyst"], "DOROTISINGLE001"),
    ("windows-framework-dependent", ["DorotiTarget=Windows"], "DOROTISINGLE004"),
]:
    run("reject-" + name, "dotnet", "msbuild", str(project), "-nologo",
        "-t:ValidateDorotiSingleFile", *["-p:" + value for value in switches], error=error)

windows = ROOT / "samples/DorotiCarouselApp/windowsappsdk/DorotiCarouselApp.WindowsAppSdk.csproj"
if platform.system() == "Windows":
    state = json.loads(run("windows-defaults", "dotnet", "msbuild", str(windows), "-nologo",
        "-p:PublishSingleFile=true", "-p:Configuration=Release", "-t:ValidateDorotiSingleFile",
        "-getProperty:EnableMsixTooling,IncludeAllContentForSelfExtract,IncludeNativeLibrariesForSelfExtract,SelfContained"))
    assert all(value == "true" for value in state["Properties"].values()), state
print("PASS isolated desktop single-file publish, adjacent licenses, extraction, no-build notices/payload and configuration guards.", flush=True)
