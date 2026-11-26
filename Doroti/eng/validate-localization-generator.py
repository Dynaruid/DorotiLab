"""Check a separately restored ARB analyzer package and compiler diagnostics."""
import argparse
import json
import os
import re
import shutil
import subprocess
import sys
import uuid
import zipfile
from pathlib import Path
from xml.sax.saxutils import escape


ROOT = Path(__file__).resolve().parents[2]
TIMEOUT = ROOT / "Doroti/eng/run-with-timeout.py"
GENERATOR = ROOT / "tools/Doroti.Localization.Generator/Doroti.Localization.Generator.csproj"
FIXTURE = ROOT / "Doroti/tests/Doroti.Localization.Tests/l10n"


def run(target, label, *command, expected=None):
    result = subprocess.run([sys.executable, str(TIMEOUT), *map(str, command)], cwd=target,
                            stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True,
                            encoding="utf-8", errors="replace")
    (target / (label + ".log")).write_text(result.stdout, encoding="utf-8")
    if expected is not None:
        if result.returncode and expected in result.stdout and re.search(r"\.arb\(\d+,\d+\): error DLOC001", result.stdout):
            print(f"PASS {label}")
            return
    elif result.returncode == 0:
        print(f"PASS {label}")
        return
    raise SystemExit(f"FAIL {label}\n{result.stdout}")


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("product_bin", type=Path, help="Directory of previously built localization test assemblies.")
    parser.add_argument("--artifacts", type=Path, default=ROOT / "Doroti/artifacts/work2-generator")
    parser.add_argument("--sdk", choices=("10", "11"), default="11")
    parser.add_argument("--aot", action="store_true")
    args = parser.parse_args()
    product_bin = args.product_bin.resolve(strict=True)
    target = args.artifacts.resolve() / f"sdk{args.sdk}-{uuid.uuid4().hex[:10]}"
    target.mkdir(parents=True)
    sdk = "10.0.403" if args.sdk == "10" else "11.0.100"
    (target / "global.json").write_text(json.dumps({"sdk": {"version": sdk, "rollForward": "latestPatch"}}), encoding="utf-8")
    for filename in ("Directory.Build.props", "Directory.Build.targets", "Directory.Packages.props"):
        (target / filename).write_text("<Project />\n", encoding="utf-8")

    feed = target / "feed"
    run(target, "pack", "dotnet", "pack", GENERATOR, "-c", "Release", "-o", feed,
        "--artifacts-path", target / "build-tool")
    package = feed / "Doroti.Localization.Generator.0.4.1-beta.nupkg"
    with zipfile.ZipFile(package) as archive:
        files = set(archive.namelist())
        required = {"analyzers/dotnet/cs/Doroti.Localization.Generator.dll",
                    "build/Doroti.Localization.Generator.props", "licenses/Unicode-3.0.txt"}
        if not required <= files or any(name.startswith("lib/") for name in files):
            raise SystemExit(f"Unexpected analyzer package contents: {sorted(files)}")

    project = target / "Consumer.csproj"
    refs = sorted(product_bin.glob("Doroti*.dll"))
    references = "\n    ".join(f'<Reference Include="{escape(path.stem)}"><HintPath>{escape(str(path))}</HintPath></Reference>'
                               for path in refs if path.stem != "Doroti.Localization.Tests")
    project.write_text(f'''<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework><OutputType>Exe</OutputType><TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <DorotiLocalizationClass>PackageStrings</DorotiLocalizationClass><DorotiLocalizationNamespace>Independent</DorotiLocalizationNamespace>
    <RestorePackagesPath>$(MSBuildThisFileDirectory)packages</RestorePackagesPath>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="Program.cs" />
    <PackageReference Include="Doroti.Localization.Generator" Version="0.4.1-beta" PrivateAssets="all" />
    <AdditionalFiles Include="input/*.arb" />
    {references}
  </ItemGroup>
</Project>
''', encoding="utf-8")
    (target / "Program.cs").write_text('''using Independent;
var expected = int.Parse(args[0]);
var text = PackageStrings.Get(new Doroti.Ui.Locale("en", "US")).greeting("Ada", 1, "gift");
if (PackageStrings.SupportedLocales.Count != expected || text != "{Hello} Ada: a gift.") return 1;
return 0;
''', encoding="utf-8")
    config = target / "NuGet.config"
    config.write_text(f'<configuration><packageSources><clear/><add key="local" value="{escape(str(feed))}"/></packageSources></configuration>',
                      encoding="utf-8")
    target_files = target / "input"
    target_files.mkdir()
    for filename in ("en.arb", "ko.arb"):
        shutil.copyfile(FIXTURE / filename, target_files / filename)
    run(target, "restore", "dotnet", "restore", project, "--configfile", config)
    run(target, "initial", "dotnet", "run", "--project", project, "--no-restore", "--", "2")

    malformed = {
        "duplicate": ('{"@@locale":"en","entry":"A","entry":"B"}', "Duplicate JSON key 'entry'"),
        "braces": ('{"@@locale":"en","entry":"{name"}', "Expected ','"),
        "identifier": ('{"@@locale":"en","my-key":"A"}', "cannot be used as a generated C# member"),
        "format": ('{"@@locale":"en","entry":"{n}","@entry":{"placeholders":{"n":{"type":"long","format":"compact"}}}}',
                   "Unsupported format 'compact'"),
        "choice": ('{"@@locale":"en","entry":"{n, plural, one {x}}","@entry":{"placeholders":{"n":{"type":"long"}}}}',
                   "requires an 'other' branch"),
    }
    english = target_files / "en.arb"
    for label, (source, expected) in malformed.items():
        english.write_text(source, encoding="utf-8")
        run(target, label, "dotnet", "build", project, "--no-restore", expected=expected)
    shutil.copyfile(FIXTURE / "en.arb", english)
    korean = target_files / "ko.arb"
    korean.write_text('{"@@locale":"ko","greeting":"{another}"}', encoding="utf-8")
    run(target, "missing-and-mismatched", "dotnet", "build", project, "--no-restore", expected="Unknown placeholder 'another'")
    shutil.copyfile(FIXTURE / "ko.arb", korean)
    run(target, "recovered", "dotnet", "run", "--project", project, "--no-restore", "--", "2")
    korean.unlink()
    run(target, "incremental-removal", "dotnet", "run", "--project", project, "--no-restore", "--", "1")

    if args.aot:
        rid = "win-x64" if os.name == "nt" else "linux-x64"
        publish = target / "native"
        config.write_text(config.read_text(encoding="utf-8").replace("</packageSources>",
            '<add key="nuget.org" value="https://api.nuget.org/v3/index.json"/></packageSources>'), encoding="utf-8")
        run(target, "aot-publish", "dotnet", "publish", project, "-c", "Release", "-r", rid,
            "--self-contained", "true", "-p:PublishAot=true", "-o", publish)
        run(target, "aot-consumer", publish / ("Consumer.exe" if os.name == "nt" else "Consumer"), "1")


if __name__ == "__main__":
    main()
