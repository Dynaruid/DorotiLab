"""Pack the Windows host closure, then restore/publish/run outside the repository.
One short product execution; every child has the repository's 1200s limit.
"""
import hashlib
import json
import os
from pathlib import Path
import shutil
import tempfile
import xml.etree.ElementTree as ET
from xml.sax.saxutils import escape
import zipfile
from run import ROOT, OUT, run, save

def main():
    print(OUT, flush=True)
    version = "0.2.0-work0final20260912"
    feed = OUT / "packages"
    names = ["Doroti.Target.Windows.WindowsAppSdk.win-x64", "Doroti.Framework.Services"]
    projects = []; visited = set()
    def visit(path):
        path = path.resolve()
        if path in visited: return
        visited.add(path)
        for item in ET.parse(path).iter("ProjectReference"):
            visit(path.parent / item.attrib["Include"].replace("\\", "/"))
        projects.append(path)
    for name in names: visit(ROOT / f"Doroti/src/{name}/{name}.csproj")
    for project in projects:
        if run("pack/" + project.stem, ["dotnet", "pack", project, "-c", "Release", "-o", feed,
                                       "-p:Version=" + version, "-p:PackageVersion=" + version, "--nologo"]): return 1
    package_entries = {}
    for package in feed.glob("*.nupkg"):
        with zipfile.ZipFile(package) as archive:
            entries = archive.namelist()
            if any("graphite/" in e.lower() or "libdorotigraphite" in e.lower() for e in entries):
                raise RuntimeError("Retired native asset leaked into package: " + package.name)
            package_entries[package.name] = entries
    save(OUT / "package-entries.json", package_entries)
    consumer = Path(tempfile.mkdtemp(prefix="doroti-official-windows-consumer-"))
    fixture = ROOT / "Doroti/validation/hwnd-exact-cpp-product"
    for name in ("Program.cs", "doroti-application-manifest.json"): shutil.copy2(fixture / name, consumer / name)
    project = consumer / "Consumer.csproj"
    references = "\n".join(f'<PackageReference Include="{name}" Version="{version}" />' for name in names)
    project.write_text(f'''<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType><TargetFramework>net10.0-windows10.0.19041.0</TargetFramework>
    <RuntimeIdentifier>win-x64</RuntimeIdentifier><PlatformTarget>x64</PlatformTarget>
    <DefaultItemExcludes>$(DefaultItemExcludes);packages/**;publish/**</DefaultItemExcludes><SelfContained>true</SelfContained><WindowsAppSDKSelfContained>true</WindowsAppSDKSelfContained>
    <ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable>
    <DorotiTarget>Windows</DorotiTarget><AssemblyName>Doroti.Validation.HwndExactCppProduct</AssemblyName>
    <SupportedOSPlatformVersion>10.0.19041.0</SupportedOSPlatformVersion>
  </PropertyGroup>
  <ItemGroup>{references}
    <EmbeddedResource Include="doroti-application-manifest.json" LogicalName="Doroti.Application.Manifest" />
  </ItemGroup>
</Project>''', encoding="utf-8")
    config = consumer / "NuGet.Config"
    config.write_text(f'<configuration><packageSources><clear/><add key="local" value="{escape(str(feed))}"/><add key="nuget" value="https://api.nuget.org/v3/index.json"/></packageSources></configuration>')
    os.environ["NUGET_PACKAGES"] = str(consumer / "packages")
    os.environ.pop("DOROTI_WINDOWS_GRAPHITE_OFFICIAL_MANIFEST", None)
    os.environ.pop("DOROTI_WINDOWS_GRAPHITE_NATIVE", None)
    save(OUT / "consumer.json", dict(path=str(consumer), cache=os.environ["NUGET_PACKAGES"], nativeSelection="default; no manifest/environment override"))
    if run("restore", ["dotnet", "restore", project, "--configfile", config]): return 1
    if run("publish", ["dotnet", "publish", project, "-c", "Release", "--no-restore", "-o", consumer / "publish", "--nologo"]): return 1
    graph = json.loads((consumer / "obj/project.assets.json").read_text())
    if any(v["type"] == "project" for v in graph["libraries"].values()): raise RuntimeError("A source project leaked into package-only graph")
    shutil.copy2(consumer / "obj/project.assets.json", OUT / "project.assets.json")
    binary = consumer / "publish"
    if (binary / "graphite").exists(): raise RuntimeError("Custom distribution leaked into publish")
    save(OUT / "native-files.json", [dict(name=p.name, sha256=hashlib.sha256(p.read_bytes()).hexdigest())
                                    for p in binary.glob("*.dll") if "skia" in p.name.lower() or "doroti_windows" in p.name])
    os.environ["DOROTI_WINDOWS_VULKAN_VALIDATION"] = "1"
    os.environ["DOROTI_WINDOWS_VULKAN_DEVICE"] = "NVIDIA"
    code = run("product", [binary / "Doroti.Validation.HwndExactCppProduct.exe", "--presenter", "default", "--smoke-ms", "8000", "--acrylic", "--report", OUT / "product/report.json"])
    save(OUT / "summary.json", dict(status="PASS-clean-Windows-host-consumer" if code == 0 else "FAIL", productExit=code,
        noProjectReferences=True, officialDefault=True, noCustomSkiaAssets=True, performance="prior evidence only; not remeasured"))
    return code

if __name__ == "__main__": raise SystemExit(main())
