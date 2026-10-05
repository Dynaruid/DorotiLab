"""Evaluated build, NuGet, assembly and template contracts for independent designs."""
from datetime import datetime, timezone
import json
from pathlib import Path
import shutil
import subprocess
import sys
import uuid
import xml.etree.ElementTree as ET
import zipfile

ROOT = Path(__file__).resolve().parents[2]
RUN_ROOT = ROOT / "temp/testing/design-decoupling/packages"
run = RUN_ROOT / uuid.uuid4().hex
run.mkdir(parents=True)
feed = run / "feed"
feed.mkdir()
results = []


def command(label, *args, cwd=ROOT, expected=0):
    print("Running " + label, flush=True)
    invocation = [sys.executable, str(ROOT / "Doroti/eng/run-with-timeout.py"), "--timeout", "1200", *map(str, args)]
    completed = subprocess.run(invocation, cwd=cwd, text=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
    (run / (label + ".log")).write_text(completed.stdout, encoding="utf-8")
    if (expected == 0 and completed.returncode != 0) or (expected != 0 and completed.returncode == 0):
        raise RuntimeError(label + " failed:\n" + completed.stdout[-12000:])
    results.append({"name": label, "exitCode": completed.returncode, "expectedFailure": expected != 0})
    return completed.stdout


def evaluate(project, properties=()):
    return json.loads(command("evaluate-" + project.stem + "-" + str(len(results)), "dotnet", "msbuild", project,
        "-nologo", "-getProperty:TargetFramework,Version,PackageVersion,PackageId", "-getItem:ProjectReference,PackageReference,InternalsVisibleTo", *properties))


def closure(roots):
    seen = {}
    def visit(project):
        project = project.resolve()
        if project in seen:
            return
        model = evaluate(project)
        seen[project] = model
        for reference in model["Items"]["ProjectReference"]:
            visit(Path(reference["FullPath"]))
    for project in roots:
        visit(project)
    return seen


def verify_feed():
    details = []
    for package in sorted(feed.glob("*.nupkg")):
        with zipfile.ZipFile(package) as archive:
            nuspec = ET.fromstring(archive.read(next(name for name in archive.namelist() if name.endswith(".nuspec"))))
        metadata = next(node for node in nuspec if node.tag.endswith("metadata"))
        identity = next(node.text for node in metadata if node.tag.endswith("id"))
        version = next(node.text for node in metadata if node.tag.endswith("version"))
        dependencies = {node.attrib["id"]: node.attrib["version"] for node in metadata.iter() if node.tag.endswith("dependency")}
        if identity not in ("Doroti.Material", "Doroti.Cupertino"):
            assert not set(dependencies) & {"Doroti.Material", "Doroti.Cupertino", "MaterialColorUtilities"}, (identity, dependencies)
        if identity == "Doroti.Cupertino":
            assert not set(dependencies) & {"Doroti.Material", "MaterialColorUtilities"}
        if identity in ("Doroti.Material", "Doroti.Cupertino"):
            assert all(value == "[0.4.0-alpha.1, 0.5.0)" for key, value in dependencies.items() if key.startswith("Doroti.") and key != "Doroti.Cupertino"), dependencies
        details.append({"id": identity, "version": version, "dependencies": dependencies})
    return details


def nuget_config(folder):
    folder.mkdir(parents=True, exist_ok=True)
    (folder / "Directory.Build.props").write_text("<Project />", encoding="utf-8")
    (folder / "Directory.Build.targets").write_text("<Project />", encoding="utf-8")
    (folder / "Directory.Packages.props").write_text("<Project />", encoding="utf-8")
    (folder / "NuGet.Config").write_text(f'''<configuration>
  <packageSources><clear/><add key="local" value="{feed}"/><add key="nuget" value="https://api.nuget.org/v3/index.json"/></packageSources>
  <packageSourceMapping><packageSource key="local"><package pattern="Doroti.*"/></packageSource><packageSource key="nuget"><package pattern="*"/></packageSource></packageSourceMapping>
</configuration>''', encoding="utf-8")


CORE_PROGRAM = '''using Doroti.Framework.Widgets;
using Doroti.Testing;
using Doroti.Ui;
using System.Reflection;
using var tester = new WidgetTester();
tester.pumpWidget(new Directionality(textDirection: TextDirection.ltr, child: new Text("Independent consumer", style: new Doroti.Framework.Painting.TextStyle(fontSize: 18, color: new Color(0xff123456)))));
if (tester.text("Independent consumer").Count != 1) throw new Exception("Core widget did not render.");
if (FrameworkShaderManifest.Assets.Any(asset => asset.Id.StartsWith("material."))) throw new Exception("Core registered Material implicitly.");
var pending = new Queue<Assembly>(); pending.Enqueue(typeof(Widget).Assembly);
var seen = new HashSet<string>();
while (pending.TryDequeue(out var assembly))
{
    if (!seen.Add(assembly.GetName().Name!)) continue;
    foreach (var reference in assembly.GetReferencedAssemblies())
    {
        if (reference.Name is "Doroti.Material" or "Doroti.Cupertino" or "MaterialColorUtilities") throw new Exception("Core assembly has design dependency.");
        if (reference.Name!.StartsWith("Doroti.")) pending.Enqueue(Assembly.Load(reference));
    }
}
'''


def consumer(label, design, core="0.4.0-alpha.1", material="1.0.0-alpha.1", cupertino="1.0.0-alpha.1"):
    folder = run / label
    nuget_config(folder)
    references = f'<PackageReference Include="Doroti.Testing" Version="{core}" />'
    program = CORE_PROGRAM
    if design in ("cupertino", "both"):
        references += f'<PackageReference Include="Doroti.Cupertino" Version="{cupertino}" />'
        program += 'tester.pumpWidget(new Doroti.Cupertino.CupertinoApp(home: new Text("Cupertino consumer")));\nif (tester.text("Cupertino consumer").Count != 1) throw new Exception("Cupertino consumer failed.");\n'
    if design in ("material", "both"):
        references += f'<PackageReference Include="Doroti.Material" Version="{material}" />'
        program += 'tester.pumpWidget(new Doroti.Material.MaterialApp(home: new Doroti.Material.Scaffold(body: new Text("Material consumer"))));\nif (tester.text("Material consumer").Count != 1) throw new Exception("Material consumer failed.");\nDoroti.Material.MaterialShaderAssets.Register();\nFrameworkShaderLoader.LoadProgram("material.ink-sparkle").asTask().GetAwaiter().GetResult();\n'
    native = "Win32" if sys.platform == "win32" else "macOS" if sys.platform == "darwin" else "Linux"
    references += f'<PackageReference Include="SkiaSharp.NativeAssets.{native}" Version="4.154.0-preview.1.26454.9" />'
    (folder / "Consumer.csproj").write_text(f'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net10.0</TargetFramework><OutputType>Exe</OutputType><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable><TreatWarningsAsErrors>true</TreatWarningsAsErrors></PropertyGroup><ItemGroup>{references}</ItemGroup></Project>', encoding="utf-8")
    (folder / "Program.cs").write_text(program + 'Console.WriteLine("PASS: isolated NuGet-only consumer");\n', encoding="utf-8")
    command(label, "dotnet", "run", "--project", folder / "Consumer.csproj", "-c", "Debug", "-p:RestorePackagesPath=" + str(run / "nuget"))
    assets = json.loads((folder / "obj/project.assets.json").read_text())
    names = {name.split("/")[0] for name in assets["libraries"]}
    if design == "widgets":
        assert not names & {"Doroti.Material", "Doroti.Cupertino", "MaterialColorUtilities"}, names
    if design == "cupertino":
        assert not names & {"Doroti.Material", "MaterialColorUtilities"}, names
    assert not any(value["type"] == "project" for value in assets["libraries"].values()), "Consumer has a repository ProjectReference"
    results[-1]["resolvedPackages"] = sorted(names)


try:
    material_project = ROOT / "packages/Doroti.Material/Doroti.Material.csproj"
    cupertino_project = ROOT / "packages/Doroti.Cupertino/Doroti.Cupertino.csproj"
    testing_project = ROOT / "Doroti/src/Doroti.Testing/Doroti.Testing.csproj"
    graph = closure([material_project, testing_project])
    for project, model in graph.items():
        package = model["Properties"]["PackageId"]
        names = {Path(reference["FullPath"]).stem for reference in model["Items"]["ProjectReference"]}
        if package not in ("Doroti.Material", "Doroti.Cupertino"):
            assert not names & {"Doroti.Material", "Doroti.Cupertino"}, (package, names)
            assert not any(reference["Identity"] == "MaterialColorUtilities" for reference in model["Items"]["PackageReference"]), package
            assert not any(reference["Identity"] in ("Doroti.Material", "Doroti.Cupertino") for reference in model["Items"]["InternalsVisibleTo"]), package
    for project in (material_project, cupertino_project):
        evaluated = evaluate(project, ["-p:Version=9.9.9"])
        assert evaluated["Properties"]["Version"] == "1.0.0-alpha.1", evaluated
    command("build-design", "dotnet", "build", material_project, "-c", "Debug", "--nologo")
    command("build-testing", "dotnet", "build", testing_project, "-c", "Debug", "--nologo")
    for project in graph:
        command("pack-" + project.stem, "dotnet", "pack", project, "-c", "Debug", "--no-build", "-o", feed, "--nologo")
    package_details = verify_feed()
    for design in ("widgets", "cupertino", "material", "both"):
        consumer("consumer-" + design, design)
    for design, project in (("Material", material_project), ("Cupertino", cupertino_project)):
        command("pack-design-update-" + design, "dotnet", "pack", project, "-c", "Debug", "--no-build", "-o", feed, "-p:Doroti" + design + "Version=1.0.0-alpha.2")
    consumer("consumer-design-update", "both", material="1.0.0-alpha.2", cupertino="1.0.0-alpha.2")
    core_projects = [project for project in graph if project not in (material_project, cupertino_project)]
    for project in core_projects:
        command("pack-core-update-" + project.stem, "dotnet", "pack", project, "-c", "Debug", "--no-build", "-o", feed, "-p:Version=0.4.0-alpha.2")
    consumer("consumer-core-update", "both", core="0.4.0-alpha.2")
    # Range rejection needs only the design framework closure, not preview Skia renderer packages.
    for project in core_projects:
        if project.stem.startswith("Doroti.Skia") or project.stem == "Doroti.Testing":
            continue
        command("pack-outside-core-" + project.stem, "dotnet", "pack", project, "-c", "Debug", "--no-build", "-o", feed, "-p:Version=0.5.0")
    for design in ("Material", "Cupertino"):
        outside = run / ("outside-range-" + design)
        nuget_config(outside)
        (outside / "Rejected.csproj").write_text(f'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net10.0</TargetFramework><TreatWarningsAsErrors>true</TreatWarningsAsErrors></PropertyGroup><ItemGroup><PackageReference Include="Doroti.Framework.Widgets" Version="0.5.0"/><PackageReference Include="Doroti.{design}" Version="1.0.0-alpha.1"/></ItemGroup></Project>', encoding="utf-8")
        rejected = command("outside-range-rejected-" + design, "dotnet", "restore", outside / "Rejected.csproj", "-p:RestorePackagesPath=" + str(run / "nuget"), expected=1)
        assert "NU1608" in rejected or "NU1107" in rejected, rejected
    sdk = ROOT / "Doroti/src/Doroti.App.Sdk/Doroti.App.Sdk.csproj"
    command("pack-app-sdk", "dotnet", "pack", sdk, "-c", "Debug", "-o", feed)
    hive = run / "template-hive"
    command("template-install", "dotnet", "new", "install", ROOT / "Doroti/templates/Doroti.Templates/content/doroti-app", "--debug:custom-hive", hive)
    for design in ("widgets", "material", "cupertino"):
        folder = run / ("template-" + design)
        command("template-create-" + design, "dotnet", "new", "doroti-app", "-n", "ContractApp", "-o", folder, "--design", design, "--debug:custom-hive", hive)
        nuget_config(folder)
        project = folder / "ContractApp.csproj"
        command("template-build-" + design, "dotnet", "build", project, "-c", "Debug", "-p:RestorePackagesPath=" + str(run / "nuget"))
        assets = json.loads((folder / "obj/project.assets.json").read_text())
        names = {name.split("/")[0] for name in assets["libraries"]}
        if design == "widgets":
            assert not names & {"Doroti.Material", "Doroti.Cupertino", "MaterialColorUtilities"}
        if design == "cupertino":
            assert not names & {"Doroti.Material", "MaterialColorUtilities"}
    negative = run / "removed-identity"
    nuget_config(negative)
    (negative / "Removed.csproj").write_text('<Project Sdk="Doroti.App.Sdk/0.4.0-alpha.1"><ItemGroup><PackageReference Include="Doroti.Framework.Material" Version="0.3.0-beta"/></ItemGroup></Project>', encoding="utf-8")
    output = command("removed-design-rejected", "dotnet", "build", negative / "Removed.csproj", "-p:RestorePackagesPath=" + str(run / "nuget"), expected=1)
    assert "DOROTIDESIGN001" in output, output
    report = {"schema": "doroti.design-package-verification/v1", "verifiedUtc": datetime.now(timezone.utc).isoformat(),
        "result": "PASS", "results": results, "packages": verify_feed(), "runId": run.name,
        "scope": "evaluated graph, real nupkg nuspec, isolated restore/build/CPU execution, core AssemblyRef, independent candidate versions and template app build",
        "notVerified": ["native provider startup G1", "physical input/IME", "GPU shader display", "trim/AOT"], "rawArtifacts": "removed after successful summary"}
    target = ROOT / "Doroti/docs/migrations/design-platform/design-package-verification.json"
    target.write_text(json.dumps(report, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    assert run.resolve().parent == RUN_ROOT.resolve()
    shutil.rmtree(run)
    print("PASS: independent design package and template contracts; " + str(target), flush=True)
except BaseException:
    print("Failure evidence retained: " + str(run), flush=True)
    raise
