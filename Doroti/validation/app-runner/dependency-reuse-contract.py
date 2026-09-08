"""End-to-end CLI freshness test with an isolated local NuGet package/cache."""
import json
import os
from pathlib import Path
import subprocess
import sys
from xml.sax.saxutils import escape

repo = Path(__file__).resolve().parents[3]
root = Path(sys.argv[1]).resolve()
root.mkdir(parents=True, exist_ok=False)
env = dict(os.environ, NUGET_PACKAGES=str(root / "packages"))
for name in ["Directory.Build.props", "Directory.Build.targets"]:
    (root / name).write_text("<Project />", encoding="utf-8")
(root / "Directory.Packages.props").write_text("<Project />", encoding="utf-8")
feed = root / "feed"
feed.mkdir()
(root / "NuGet.Config").write_text(f'<configuration><packageSources><clear/><add key="fixture" value="{escape(str(feed))}"/></packageSources></configuration>', encoding="utf-8")


def run(args, label, success=True):
    result = subprocess.run(args, cwd=repo, env=env, capture_output=True, text=True, timeout=1200)
    text = result.stdout + result.stderr
    (root / f"{label}.log").write_text(text, encoding="utf-8")
    assert (result.returncode == 0) == success, text
    return text


package = root / "package-source"
package.mkdir()
(package / "Fixture.csproj").write_text('''<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup>
<TargetFramework>net10.0</TargetFramework><PackageId>Doroti.DependencyFixture</PackageId><Version>1.0.0</Version>
</PropertyGroup></Project>''', encoding="utf-8")
(package / "Dependency.cs").write_text('public static class Dependency { public const string Value = "ONE"; }', encoding="utf-8")
run(["dotnet", "pack", str(package / "Fixture.csproj"), "-c", "Release", "-o", str(feed), "--nologo"], "pack")
app = root / "app"
app.mkdir()
child = app / "child"
child.mkdir()
(child / "Child.csproj").write_text('''<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup>
<ItemGroup><PackageReference Include="Doroti.DependencyFixture" Version="1.0.0" /></ItemGroup></Project>''', encoding="utf-8")
(child / "Child.cs").write_text('public static class Child { public static string Read() => Dependency.Value; }', encoding="utf-8")
runner = app / "Runner.csproj"
runner.write_text('''<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net10.0</TargetFramework>
<OutputType>Exe</OutputType><EnableDefaultCompileItems>false</EnableDefaultCompileItems></PropertyGroup>
<ItemGroup><Compile Include="Program.cs" /><ProjectReference Include="child/Child.csproj" /></ItemGroup></Project>''', encoding="utf-8")
(app / "Program.cs").write_text('System.Console.WriteLine("fixture-value=" + Child.Read());', encoding="utf-8")
(app / "doroti-workspace.json").write_text(json.dumps({"schemaVersion": "doroti.workspace/v1", "applicationProject": "child/Child.csproj",
    "platforms": {p: "Runner.csproj" for p in ["android", "ios", "linux", "macos", "maccatalyst", "web", "windows"]}}), encoding="utf-8")
cli = ["pwsh", "-NoProfile", "-File", str(repo / "Doroti/eng/doroti.ps1")]


def invoke(verb, label, extra=(), success=True):
    return run(cli + [verb, "-App", str(app), "-Platform", "linux", "-Configuration", "Release"] + list(extra), label, success)


assert "rebuilding before recording success" in invoke("build", "initial-build")
assert "rebuilding before recording success" not in invoke("build", "unchanged-build")
assert "fixture-value=ONE" in invoke("run", "unchanged-reuse", ["-LastSuccessful"])
# Model a compiler server warmed by a prior direct/legacy build. A one-time
# isolated rebuild must not allow subsequent CLI compiles to reuse its metadata.
run(["dotnet", "build", str(child / "Child.csproj"), "-c", "Release", "-t:Rebuild", "--no-restore", "--nologo"], "warm-shared-compiler")
cached = root / "packages/doroti.dependencyfixture/1.0.0/lib/net10.0/Fixture.dll"
original = cached.read_bytes()
replacement = original.replace("ONE".encode("utf-16-le"), "TWO".encode("utf-16-le"))
assert replacement != original and len(replacement) == len(original)
stamp = cached.stat().st_mtime_ns
cached.write_bytes(replacement)
os.utime(cached, ns=(stamp, stamp))
rejected = invoke("run", "changed-dependency-reuse-rejected", ["-LastSuccessful"], success=False)
assert "The last successful artifact is stale" in rejected
rebuilt = invoke("run", "changed-dependency-rebuilt")
assert "rebuilding before recording success" in rebuilt and "fixture-value=TWO" in rebuilt
(child / "Child.cs").write_text('public static class Child { public static string Read() => Dependency.Value; } // source edit', encoding="utf-8")
assert "fixture-value=TWO" in invoke("run", "source-edit-after-dependency-rebuild")
state = app / ".doroti/launch-state/linux-WindowsAppSdk-Release-default-rid.json"
saved = state.read_bytes()
data = json.loads(saved)
assert data["schemaVersion"] == "doroti.launch-state/v3" and data["dependencies"]
data["schemaVersion"] = "doroti.launch-state/v2"
state.write_text(json.dumps(data), encoding="utf-8")
assert "The last successful artifact is stale" in invoke("run", "old-state-rejected", ["-LastSuccessful"], success=False)
state.write_bytes(saved)
(app / "Program.cs").write_text("this does not compile", encoding="utf-8")
invoke("build", "failed-build", success=False)
assert state.read_bytes() == saved, "Failed build replaced the successful record"
result = {"status": "PASS", "scenarios": 8, "transitiveNuGetContentChangePreservedSizeAndMtime": True,
          "reusedValue": "ONE", "rebuiltValue": "TWO", "failedBuildPreservedPreviousRecord": True}
(root / "result.json").write_text(json.dumps(result, indent=2), encoding="utf-8")
print(json.dumps(result, indent=2))
