"""Real NuGet consumers, .NET trimming, font subsetting and incremental/unsafe-use checks."""
import argparse
import json
import platform
from pathlib import Path
import subprocess
import sys
import uuid
import xml.etree.ElementTree as ET
import zipfile

ROOT = Path(__file__).resolve().parents[2]
parser = argparse.ArgumentParser()
parser.add_argument("--resume", type=Path)
parser.add_argument("--remaining", action="store_true", help="Resume after successful publish checks, using the same feed and fixtures")
options = parser.parse_args()
if options.remaining and not options.resume:
    parser.error("--remaining requires --resume")
RUN = options.resume.resolve() if options.resume else ROOT / "temp/testing/icon-packages" / uuid.uuid4().hex
FEED = RUN / "feed"
FEED.mkdir(parents=True, exist_ok=True)
TOOL = ROOT / "tools/Doroti.IconTools/bin/Release/net10.0/Doroti.IconTools.dll"
NUGET = RUN / ("nuget-" + uuid.uuid4().hex)
checks = json.loads((RUN / "receipt.json").read_text()) if options.remaining else []
RID = ("win" if sys.platform == "win32" else "osx" if sys.platform == "darwin" else "linux") + ("-arm64" if platform.machine().lower() in ("arm64", "aarch64") else "-x64")


def command(label, *args, expected=0):
    print("Running " + label, flush=True)
    result = subprocess.run([sys.executable, str(ROOT / "Doroti/eng/run-with-timeout.py"), "--timeout", "1200", *map(str, args)],
                            cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
    (RUN / (label + ".log")).write_text(result.stdout, encoding="utf-8")
    if (result.returncode == 0) != (expected == 0):
        raise RuntimeError(label + ":\n" + result.stdout[-10000:])
    checks.append({"name": label, "exitCode": result.returncode})
    return result.stdout


def inspect(path):
    return json.loads(command("inspect-" + str(len(checks)), "dotnet", TOOL, "--inspect", path))


def project_graph(projects):
    found = {}
    def visit(project):
        project = project.resolve()
        if project in found:
            return
        saved = RUN / ("evaluate-" + project.stem + ".log")
        model = json.loads(saved.read_text(encoding="utf-8") if saved.exists() else command("evaluate-" + project.stem, "dotnet", "msbuild", project, "-nologo",
                                  "-getProperty:IsPackable,PackageId", "-getItem:ProjectReference"))
        found[project] = model
        for reference in model["Items"]["ProjectReference"]:
            visit(Path(reference["FullPath"]))
    for project in projects:
        visit(project)
    return found


def consumer(name, program):
    folder = RUN / name
    folder.mkdir(exist_ok=True)
    for filename in ("Directory.Build.props", "Directory.Build.targets", "Directory.Packages.props"):
        (folder / filename).write_text("<Project/>", encoding="utf-8")
    (folder / "NuGet.Config").write_text(f'''<configuration><packageSources><clear/>
<add key="local" value="{FEED}"/><add key="nuget" value="https://api.nuget.org/v3/index.json"/>
</packageSources><packageSourceMapping><packageSource key="local"><package pattern="Doroti.*"/></packageSource>
<packageSource key="nuget"><package pattern="*"/></packageSource></packageSourceMapping></configuration>''', encoding="utf-8")
    (folder / "Consumer.csproj").write_text('''<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup>
<TargetFramework>net10.0</TargetFramework><OutputType>Exe</OutputType><ImplicitUsings>enable</ImplicitUsings>
<TreatWarningsAsErrors>true</TreatWarningsAsErrors></PropertyGroup><ItemGroup>
<PackageReference Include="Doroti.Icons.Material" Version="1.0.0-alpha.1"/>
<PackageReference Include="Doroti.Icons.Cupertino" Version="1.0.0-alpha.1"/>
</ItemGroup></Project>''', encoding="utf-8")
    (folder / "Program.cs").write_text(program, encoding="utf-8")
    return folder


def build(folder, *properties, icons_only=True):
    command("build-" + folder.name + "-" + str(len(checks)), "dotnet", "build", folder / "Consumer.csproj", "-c", "Release", "--nologo",
            "-p:RestorePackagesPath=" + str(NUGET), *properties)
    assets = json.loads((folder / "obj/project.assets.json").read_text(encoding="utf-8"))
    names = {name.split("/")[0] for name in assets["libraries"]}
    assert not names & {"Doroti.IconTools", "Mono.Cecil"}, names
    if icons_only:
        assert not names & {"Doroti.Material", "Doroti.Cupertino"}, names
    assert all(value["type"] == "package" for value in assets["libraries"].values()), "NuGet consumer resolved repository projects"
    return folder / "bin/Release/net10.0"


try:
    roots = [ROOT / f"packages/Doroti.{name}/Doroti.{name}.csproj" for name in ("Material", "Cupertino")]
    if not options.remaining:
        command("build-icon-closure", "dotnet", "build", roots[0], "-c", "Release", "--nologo")
    graph = project_graph(roots)
    for project, model in graph.items():
        if not options.remaining and model["Properties"]["IsPackable"] == "true" and (project.stem.startswith("Doroti.Icons.") or not list(FEED.glob(project.stem + ".*.nupkg"))):
            command("pack-" + project.stem, "dotnet", "pack", project, "-c", "Release", "--no-build", "--nologo", "-o", FEED)
    for family in ("Material", "Cupertino"):
        package = FEED / f"Doroti.Icons.{family}.1.0.0-alpha.1.nupkg"
        with zipfile.ZipFile(package) as archive:
            files = archive.namelist()
            assert f"buildTransitive/Doroti.Icons.{family}.targets" in files
            assert "buildTransitive/tools/Doroti.IconTools.dll" in files and "buildTransitive/tools/Mono.Cecil.dll" in files
            spec = ET.fromstring(archive.read(next(name for name in files if name.endswith(".nuspec"))))
            dependencies = {node.attrib["id"] for node in spec.iter() if node.tag.endswith("dependency")}
            assert dependencies == {"Doroti.Framework.Widgets"}, dependencies
            assert any("LICENSE.Mono.Cecil" in name for name in files)

    if not options.remaining:
        both = consumer("both", 'Console.WriteLine($"{Doroti.Material.Icons.arrow_back.codePoint}/{Doroti.Cupertino.CupertinoIcons.heart.codePoint}");')
        binary = build(both)
        material = inspect(binary / "Doroti.Icons.Material.dll")
        cupertino = inspect(binary / "Doroti.Icons.Cupertino.dll")
        assert material["codePoints"] == [0xe092], material
        assert cupertino["codePoints"] == [0xf442], cupertino
        assert material["fontBytes"] < 300000 and cupertino["fontBytes"] < 15000
        command("run-both", "dotnet", binary / "Consumer.dll")

        # Changing icons must restore original package inputs before deriving the next subset.
        (both / "Program.cs").write_text('Console.WriteLine(Doroti.Material.Icons.home.codePoint);', encoding="utf-8")
        binary = build(both)
        home = inspect(binary / "Doroti.Icons.Material.dll")
        assert home["codePoints"] == [0xe318], home
        assert inspect(binary / "Doroti.Icons.Cupertino.dll")["fontBytes"] == 0
        command("run-incremental", "dotnet", binary / "Consumer.dll")

        published = RUN / "published"
        command("publish-trimmed", "dotnet", "publish", both / "Consumer.csproj", "-c", "Release", "-r", RID,
                "--self-contained", "true", "-p:PublishTrimmed=true", "-p:TrimMode=link", "-p:RestorePackagesPath=" + str(NUGET), "-o", published)
        trimmed = inspect(published / "Doroti.Icons.Material.dll")
        assert trimmed["getters"] <= 2 and trimmed["codePoints"] == [0xe318], trimmed
        assert not (published / "Doroti.Icons.Cupertino.dll").exists(), "Unused icon package survived .NET trimming"
        command("run-published", published / ("Consumer.exe" if sys.platform == "win32" else "Consumer"))

        ready = RUN / "ready-to-run"
        command("publish-ready-to-run", "dotnet", "publish", both / "Consumer.csproj", "-c", "Release", "-r", RID,
                "--self-contained", "true", "-p:PublishTrimmed=false", "-p:PublishReadyToRun=true", "-p:RestorePackagesPath=" + str(NUGET), "-o", ready)
        r2r = inspect(ready / "Doroti.Icons.Material.dll")
        assert r2r["readyToRun"] and r2r["codePoints"] == [0xe318], r2r
        command("run-ready-to-run", ready / ("Consumer.exe" if sys.platform == "win32" else "Consumer"))

    else:
        both = RUN / "both"

    design = consumer("design", 'Console.WriteLine(Doroti.Material.Icons.home.codePoint);')
    project = design / "Consumer.csproj"
    project.write_text(project.read_text().replace('<PackageReference Include="Doroti.Icons.Material" Version="1.0.0-alpha.1"/>',
                       '<PackageReference Include="Doroti.Material" Version="1.0.0-alpha.1"/>').replace(
                       '<PackageReference Include="Doroti.Icons.Cupertino" Version="1.0.0-alpha.1"/>', ''), encoding="utf-8")
    # The feed contains the current core, rather than historical minimum versions of the design ranges.
    with zipfile.ZipFile(next(FEED.glob("Doroti.Framework.Widgets.*.nupkg"))) as archive:
        spec = ET.fromstring(archive.read(next(n for n in archive.namelist() if n.endswith(".nuspec"))))
        core_version = next(node.text for node in spec.iter() if node.tag.endswith("}version"))
    pins = ''.join(f'<PackageReference Include="{model["Properties"]["PackageId"]}" Version="{core_version}"/>'
                   for path, model in graph.items() if model["Properties"]["IsPackable"] == "true"
                   and path not in roots and not path.stem.startswith("Doroti.Icons."))
    project.write_text(project.read_text().replace('</ItemGroup>', pins + '</ItemGroup>'), encoding="utf-8")
    design_binary = build(design, icons_only=False)
    assert (design_binary / "doroti-icon-tree-shaking.json").exists(), "Transitive design dependency did not import icon targets"
    assert inspect(design_binary / "Doroti.Icons.Material.dll")["fontBytes"] < 10000

    dynamic = consumer("dynamic", 'Console.WriteLine(new Doroti.Framework.Widgets.IconData(Environment.TickCount & 0xffff, fontFamily: "MaterialIcons").codePoint);')
    dynamic_binary = build(dynamic)
    assert inspect(dynamic_binary / "Doroti.Icons.Material.dll")["fontBytes"] == 1645184
    assert any(item["mode"] == "full" for item in json.loads((dynamic_binary / "doroti-icon-tree-shaking.json").read_text()))
    reflection = consumer("reflection", 'var type = Type.GetType("Doroti.Material.Icons, Doroti.Icons.Material")!; Console.WriteLine(type.GetProperty("home")!.GetValue(null));')
    reflection_binary = build(reflection)
    assert inspect(reflection_binary / "Doroti.Icons.Material.dll")["fontBytes"] == 1645184
    command("run-reflection", "dotnet", reflection_binary / "Consumer.dll")
    no_shake = build(both, "-p:DorotiIconTreeShaking=false")
    assert inspect(no_shake / "Doroti.Icons.Material.dll")["fontBytes"] == 1645184
    checks.append({"name": "package-font-and-trimming-contract", "status": "PASS"})
finally:
    (RUN / "receipt.json").write_text(json.dumps(checks, indent=2), encoding="utf-8")
    print("Receipts: " + str(RUN), flush=True)
