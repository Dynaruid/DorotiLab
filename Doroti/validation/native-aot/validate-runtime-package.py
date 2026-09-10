#!/usr/bin/env python3
"""Publish the Runtime contracts from an isolated NuGet consumer with no ProjectReference."""
import argparse
import hashlib
import json
from pathlib import Path
import shutil
import subprocess
import tempfile
from xml.sax.saxutils import escape

parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument("--feed", type=Path, required=True)
parser.add_argument("--output", type=Path, required=True)
parser.add_argument("--rid", required=True)
args = parser.parse_args()
here = Path(__file__).resolve().parent
root = here.parents[2]
output = args.output.resolve()
output.mkdir(parents=True, exist_ok=True)
temporary_root = root / ".doroti/tmp"
temporary_root.mkdir(parents=True, exist_ok=True)
with tempfile.TemporaryDirectory(prefix="runtime-package-", dir=temporary_root) as directory:
    project = Path(directory)
    project_text = f"""<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework><OutputType>Exe</OutputType>
    <AssemblyName>RuntimePackageConsumer</AssemblyName>
    <ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable>
    <PublishAot>true</PublishAot><TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <ILLinkTreatWarningsAsErrors>true</ILLinkTreatWarningsAsErrors>
    <IlcTreatWarningsAsErrors>true</IlcTreatWarningsAsErrors>
    <RestoreSources>{escape(str(args.feed.resolve()))};https://api.nuget.org/v3/index.json</RestoreSources>
    <RestorePackagesPath>{escape(str(output / 'nuget-cache'))}</RestorePackagesPath>
  </PropertyGroup>
  <ItemGroup><PackageReference Include="Doroti.Runtime" Version="0.2.0-beta" /></ItemGroup>
</Project>
"""
    (project / "Consumer.csproj").write_text(project_text)
    (output / "Consumer.csproj.txt").write_text(project_text)
    for name in ("Program.cs", "ErrorHandlerContract.cs", "FutureOrContract.cs"):
        shutil.copy2(here.parent / "runtime-async-contract" / name, project / name)
    publish = ["dotnet", "publish", str(project / "Consumer.csproj"), "-c", "Release", "-r", args.rid,
               "-o", str(output / "publish"), "-p:UseSharedCompilation=false"]
    with (output / "publish.log").open("w") as log:
        subprocess.run(publish, stdout=log, stderr=subprocess.STDOUT, check=True, timeout=1200)
    executable = output / "publish" / ("RuntimePackageConsumer.exe" if args.rid.startswith("win-") else "RuntimePackageConsumer")
    with (output / "execute.log").open("w") as log:
        subprocess.run([str(executable)], stdout=log, stderr=subprocess.STDOUT, check=True, timeout=1200)
    package = args.feed / "Doroti.Runtime.0.2.0-beta.nupkg"
    report = dict(status="pass", rid=args.rid, projectReferences=[], command=publish,
                  executableSha256=hashlib.sha256(executable.read_bytes()).hexdigest(),
                  packageSha256=hashlib.sha256(package.read_bytes()).hexdigest())
    (output / "result.json").write_text(json.dumps(report, indent=2) + "\n")
    print(json.dumps(report, indent=2))
