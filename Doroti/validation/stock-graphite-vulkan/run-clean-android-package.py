"""Inspect an APK consuming the packaged Android host, without repository references.
Pass the local feed produced by the clean consumer packs. This is packaging evidence;
it deliberately does not repeat physical-device or emulator qualification.
"""
import hashlib
import json
import os
from pathlib import Path
import sys
import tempfile
from xml.sax.saxutils import escape
import zipfile
from run import OUT, run, save


def main():
    feed = Path(sys.argv[1]).resolve()
    consumer = Path(tempfile.mkdtemp(prefix="doroti-official-android-package-"))
    print(OUT, flush=True)
    version = "0.2.0-work0final20260912"
    project = consumer / "Consumer.csproj"
    project.write_text(f'''<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0-android</TargetFramework><OutputType>Exe</OutputType>
    <RuntimeIdentifiers>android-arm64;android-x64</RuntimeIdentifiers>
    <UseMaui>true</UseMaui><MauiVersion>10.0.90</MauiVersion><SingleProject>true</SingleProject>
    <EnableDefaultMauiItems>false</EnableDefaultMauiItems>
    <ApplicationId>org.doroti.officialpackage</ApplicationId>
    <ApplicationTitle>Official package inspection</ApplicationTitle>
    <SupportedOSPlatformVersion>24.0</SupportedOSPlatformVersion>
    <PublishTrimmed>false</PublishTrimmed><RunAOTCompilation>false</RunAOTCompilation>
    <AndroidPackageFormats>apk</AndroidPackageFormats>
    <DefaultItemExcludes>$(DefaultItemExcludes);packages/**</DefaultItemExcludes>
  </PropertyGroup>
  <ItemGroup><PackageReference Include="Doroti.Host.Maui" Version="{version}" /><PackageReference Include="Microsoft.Maui.Controls" Version="10.0.90" /></ItemGroup>
</Project>''', encoding="utf-8")
    (consumer / "MainActivity.cs").write_text('''[Android.App.Activity(MainLauncher = true, Exported = true)]
public sealed class MainActivity : Android.App.Activity
{
    protected override void OnCreate(Android.OS.Bundle state)
    {
        base.OnCreate(state);
        SetContentView(new Android.Widget.TextView(this) {
            Text = typeof(Doroti.Host.Maui.DorotiMauiAndroidApplication).Assembly.GetName().Name });
    }
}
''', encoding="utf-8")
    config = consumer / "NuGet.Config"
    config.write_text(f'<configuration><packageSources><clear/><add key="local" value="{escape(str(feed))}"/><add key="nuget" value="https://api.nuget.org/v3/index.json"/></packageSources></configuration>')
    os.environ["NUGET_PACKAGES"] = str(consumer / "packages")
    save(OUT / "consumer.json", dict(path=str(consumer), feed=str(feed), cache=os.environ["NUGET_PACKAGES"]))
    if run("restore", ["dotnet", "restore", project, "--configfile", config]): return 1
    if run("apk", ["dotnet", "build", project, "-c", "Release", "--no-restore", "-t:SignAndroidPackage", "--nologo"]): return 1
    graph = json.loads((consumer / "obj/project.assets.json").read_text())
    if any(v["type"] == "project" for v in graph["libraries"].values()):
        raise RuntimeError("Project reference leaked into clean package consumer")
    save(OUT / "resolved-libraries.json", graph["libraries"])
    expected = {
        "lib/arm64-v8a/libSkiaSharp.so": "63af1ec283b86965542bca1400ae446e6a179a7be5b6187e69aa5fa0bd49e180",
        "lib/x86_64/libSkiaSharp.so": "9bb141c1ee6b40781044a4e13779c6f19a0b45554dc6030b5f1cf49b24f7e7b8",
    }
    apks = list((consumer / "bin/Release").rglob("*-Signed.apk"))
    if len(apks) != 1: raise RuntimeError(f"Expected one signed APK: {apks}")
    with zipfile.ZipFile(apks[0]) as archive:
        names = [n for n in archive.namelist() if "skia" in n.lower() or "graphite" in n.lower()]
        native = [n for n in names if n.endswith(".so")]
        if sorted(native) != sorted(expected): raise RuntimeError(f"Duplicate/unexpected Skia native entries: {native}")
        actual = {n: hashlib.sha256(archive.read(n)).hexdigest() for n in native}
        if actual != expected: raise RuntimeError(f"Official native asset mismatch: {actual}")
    save(OUT / "summary.json", dict(status="PASS-package-only-Android-APK-assets", apk=str(apks[0]),
        apkSha256=hashlib.sha256(apks[0].read_bytes()).hexdigest(), nativeEntries=actual,
        noProjectReferences=True, customSkiaBuild=False, runtime="not run; prior product evidence retained",
        compilation="Release packaging, no trim/AOT claim"))
    return 0


if __name__ == "__main__": raise SystemExit(main())
