"""Package-only common-session consumer, isolated from repo props and NuGet cache.
This does not qualify the final platform host packages/templates. Each child is
bounded by the repository's external 1200-second wrapper.
"""
import hashlib
import json
import os
from pathlib import Path
import shutil
import tempfile
from run import HERE, ROOT, OUT, VERSION, run, save

def main():
    print(OUT, flush=True)
    package_version = "0.2.0-work0candidate"
    feed = OUT / "packages"
    projects = ["Doroti.Runtime", "Doroti.Ui", "Doroti.Skia.RuntimeEffects", "Doroti.Skia.Rendering", "Doroti.Skia.Vulkan"]
    for name in projects:
        if run("pack/" + name, ["dotnet", "pack", ROOT / f"Doroti/src/{name}/{name}.csproj", "-c", "Release", "-o", feed,
                               "-p:PackageVersion=" + package_version, "-p:Version=" + package_version, "--nologo"]): return 1
    consumer = Path(tempfile.mkdtemp(prefix="doroti-official-package-consumer-"))
    for source in HERE.glob("*.cs"): shutil.copy2(source, consumer / source.name)
    project = consumer / "StockGraphite.Session.csproj"
    project.write_text(f'''<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><RuntimeIdentifier>win-x64</RuntimeIdentifier>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable>
    <DefineConstants>SHARED_SESSION</DefineConstants><TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Doroti.Skia.Vulkan" Version="{package_version}" />
    <PackageReference Include="SkiaSharp" Version="{VERSION}" />
    <PackageReference Include="SkiaSharp.NativeAssets.Win32" Version="{VERSION}" />
    <PackageReference Include="Silk.NET.Vulkan" Version="2.23.0" />
  </ItemGroup>
</Project>
''', encoding="utf-8")
    import xml.sax.saxutils as xml
    config = consumer / "NuGet.Config"
    config.write_text(f'<configuration><packageSources><clear/><add key="candidate" value="{xml.escape(str(feed))}"/><add key="official" value="https://api.nuget.org/v3/index.json"/></packageSources></configuration>', encoding="utf-8")
    os.environ["NUGET_PACKAGES"] = str(consumer / "nuget-cache")
    save(OUT / "consumer.json", dict(path=str(consumer), packages=str(feed), isolatedCache=os.environ["NUGET_PACKAGES"],
                                     boundary="common-session-only; final platform consumer notVerified"))
    if run("restore", ["dotnet", "restore", project, "--configfile", config, "--force", "--no-http-cache"]): return 1
    if run("publish", ["dotnet", "publish", project, "-c", "Release", "--no-restore", "-o", consumer / "publish", "--nologo"]): return 1
    graph = json.loads((consumer / "obj/project.assets.json").read_text())
    if any(v["type"] == "project" for v in graph["libraries"].values()): raise RuntimeError("Consumer retained a project reference")
    save(OUT / "resolved-libraries.json", graph["libraries"])
    shutil.copy2(consumer / "obj/project.assets.json", OUT / "project.assets.json")
    binary = consumer / "publish"
    if list(binary.rglob("*DorotiGraphite*")) or (binary / "graphite").exists(): raise RuntimeError("Custom native asset leaked into clean consumer")
    save(OUT / "published-assets.json", [dict(name=p.name, sha256=hashlib.sha256(p.read_bytes()).hexdigest())
                                        for p in binary.glob("*Skia*") if p.is_file()])
    rows = []
    for gpu in ("AMD", "NVIDIA"):
        code = run(gpu, [binary / "StockGraphite.Session.exe", binary / "libSkiaSharp.dll", gpu, "session", OUT / gpu / "report.json"])
        rows.append(dict(gpu=gpu, exitCode=code))
    passed = all(row["exitCode"] == 0 for row in rows)
    save(OUT / "summary.json", dict(status="PASS-package-only-common-session" if passed else "FAIL", rows=rows,
                                   hostConsumer="notVerified", noSkiaSourceBuild=True, noProjectReferences=True))
    return 0 if passed else 1

if __name__ == "__main__": raise SystemExit(main())
