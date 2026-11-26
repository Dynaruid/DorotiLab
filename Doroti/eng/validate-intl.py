"""Run one bounded P0 globalization qualification unit, with isolated build state and receipts.

Each child command uses run-with-timeout.py (1200 seconds, process-tree cleanup). Different
SDK/mode/configuration units run sequentially; do not share their intermediate directories.
"""
import argparse
import gzip
import hashlib
import json
import os
import struct
import subprocess
import sys
import time
import xml.etree.ElementTree as ET
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
TIMEOUT = ROOT / "Doroti/eng/run-with-timeout.py"


def managed_pe(content):
    """Classify the CLI directory, including managed PE files containing ReadyToRun code."""
    if len(content) < 64 or content[:2] != b"MZ":
        return False
    pe = struct.unpack_from("<I", content, 60)[0]
    if pe + 26 > len(content) or content[pe:pe + 4] != b"PE\0\0":
        return False
    optional = pe + 24
    magic = struct.unpack_from("<H", content, optional)[0]
    directory = optional + (112 if magic == 0x20b else 96 if magic == 0x10b else len(content))
    cli = directory + 14 * 8
    return cli + 8 <= len(content) and struct.unpack_from("<I", content, cli)[0] != 0


def sizes(directory):
    groups = {}
    for path in sorted(directory.rglob("*")):
        if not path.is_file() or path.suffix in {".br", ".gz", ".pdb"}:
            continue
        content = path.read_bytes()
        extension = path.suffix.lower()
        if extension == ".dat":
            group = "icu-data"
        elif extension == ".wasm":
            group = "native" if path.name.startswith("dotnet.native") else "managed-webcil"
        elif extension in {".dll", ".exe"}:
            group = "managed-pe-including-r2r" if managed_pe(content) else "native"
        elif extension in {".so", ".dylib"}:
            group = "native"
        else:
            group = "static-and-metadata"
        entry = groups.setdefault(group, {"files": 0, "bytes": 0, "sumGzipBytes": 0})
        entry["files"] += 1
        entry["bytes"] += len(content)
        entry["sumGzipBytes"] += len(gzip.compress(content, mtime=0))
    return {"scope": "Published files excluding symbols and compression sidecars; gzip per file, not measured install/download size.", "groups": groups,
            "icu": [{"path": str(path.relative_to(directory)), "bytes": path.stat().st_size,
                     "sha256": hashlib.sha256(path.read_bytes()).hexdigest()} for path in sorted(directory.rglob("*.dat"))]}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("unit", choices=["native", "web"])
    parser.add_argument("--sdk", choices=["10", "11"], default="10")
    parser.add_argument("--configuration", choices=["Debug", "Release"], default="Debug")
    parser.add_argument("--mode", choices=["compatible", "threaded"], default="compatible")
    parser.add_argument("--baseline", action="store_true")
    parser.add_argument("--icu-file", help="An actual BlazorIcuDataFileName; EFIGS is the missing-Korean negative fixture.")
    parser.add_argument("--firefox", action="store_true")
    parser.add_argument("--trim", action="store_true")
    parser.add_argument("--aot", action="store_true")
    parser.add_argument("--pack", action="store_true")
    parser.add_argument("--artifacts", type=Path, default=ROOT / "Doroti/artifacts/intl-p0")
    args = parser.parse_args()
    suffix = "-baseline" if args.baseline else "-" + Path(args.icu_file).stem if args.icu_file else ""
    name = f"{args.unit}-sdk{args.sdk}-{args.mode}-{args.configuration.lower()}{suffix}"
    target = args.artifacts.resolve() / name
    target.mkdir(parents=True, exist_ok=True)
    sdk_directory = target / "sdk"
    sdk_directory.mkdir(exist_ok=True)
    (sdk_directory / "global.json").write_text(json.dumps({"sdk": {
        "version": "10.0.400" if args.sdk == "10" else "11.0.100", "rollForward": "latestPatch", "allowPrerelease": False,
    }}), encoding="utf-8")
    receipt = {"schema": 1, "unit": name, "status": "running", "commands": [],
               "qualification": "P0 format/plural runtime probe; not Doroti rendering, shaping, physical input or full-plan acceptance."}
    environment = dict(os.environ, DOTNET_CLI_USE_MSBUILD_SERVER="0", MSBUILDDISABLENODEREUSE="1")

    def save():
        (target / "receipt.json").write_text(json.dumps(receipt, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")

    def run(label, command, overrides=None, cwd=sdk_directory):
        log = target / f"{label}.log"
        start = time.monotonic()
        print(f"Running {name}/{label}", flush=True)
        with log.open("w", encoding="utf-8") as output:
            result = subprocess.run([sys.executable, str(TIMEOUT), *map(str, command)], cwd=cwd,
                                    env=environment | (overrides or {}), stdout=output, stderr=subprocess.STDOUT)
        entry = {"label": label, "command": list(map(str, command)), "cwd": str(cwd),
                 "exitCode": result.returncode, "elapsedSeconds": round(time.monotonic() - start, 3), "log": str(log)}
        receipt["commands"].append(entry)
        save()
        if result.returncode:
            print("\n".join(log.read_text(encoding="utf-8", errors="replace").splitlines()[-25:]), flush=True)
            raise RuntimeError(f"{label} exited {result.returncode}; see {log}")

    save()
    try:
        run("sdk-version", ["dotnet", "--version"])
        run("inventory", [sys.executable, ROOT / "Doroti/eng/localization-inventory.py", "--check"])
        intermediate = target / "build"
        if args.unit == "native":
            project = ROOT / "Doroti/tests/Doroti.Intl.Tests/Doroti.Intl.Tests.csproj"
            run("environment", ["dotnet", "run", "--project", project, "-c", "Release", "--artifacts-path", intermediate, "--", "--inspect-profile"])
            dll = intermediate / "bin/Doroti.Intl.Tests/release/Doroti.Intl.Tests.dll"
            run("contract", ["dotnet", dll, "--profile"])
            run("invariant", ["dotnet", dll, "--expect-missing-data"], {
                "DOTNET_SYSTEM_GLOBALIZATION_INVARIANT": "1", "DOTNET_SYSTEM_GLOBALIZATION_PREDEFINED_CULTURES_ONLY": "0",
            })
            if args.trim or args.aot:
                rid = "win-x64" if os.name == "nt" else "linux-x64"
                for kind in (["trim"] if args.trim else []) + (["aot"] if args.aot else []):
                    publish = target / kind
                    options = ["-p:PublishTrimmed=true", "-p:TrimMode=full"] if kind == "trim" else ["-p:PublishAot=true"]
                    run(f"{kind}-publish", ["dotnet", "publish", project, "-c", "Release", "-r", rid, "--self-contained", "true",
                                          "--artifacts-path", target / f"build-{kind}", "-o", publish, *options])
                    run(f"{kind}-contract", [publish / ("Doroti.Intl.Tests.exe" if os.name == "nt" else "Doroti.Intl.Tests"), "--profile"])
                    receipt[kind + "Size"] = sizes(publish)
            if args.pack:
                run("pack-runtime", ["dotnet", "pack", ROOT / "Doroti/src/Doroti.Runtime/Doroti.Runtime.csproj", "-c", "Release",
                                     "--artifacts-path", target / "build-pack", "-o", target / "packages"])
                run("pack", ["dotnet", "pack", ROOT / "Doroti/src/Doroti.Intl/Doroti.Intl.csproj", "-c", "Release",
                             "--artifacts-path", target / "build-pack", "-o", target / "packages"])
                consumer = target / "package-consumer"
                consumer.mkdir(exist_ok=True)
                # Shadow repository build customizations. The consumer resolves only the nupkg
                # and the standard SDK; it has no source ProjectReference to Doroti.
                for filename in ("Directory.Build.props", "Directory.Build.targets", "Directory.Packages.props"):
                    (consumer / filename).write_text("<Project />\n", encoding="utf-8")
                (consumer / "Consumer.csproj").write_text('''<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup><TargetFramework>net10.0</TargetFramework><OutputType>Exe</OutputType>
    <ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable><IsAotCompatible>true</IsAotCompatible>
    <RestorePackagesPath>$(MSBuildThisFileDirectory).packages</RestorePackagesPath>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors></PropertyGroup>
  <ItemGroup><PackageReference Include="Doroti.Intl" Version="0.4.1-beta" /></ItemGroup>
</Project>
''', encoding="utf-8")
                (consumer / "Program.cs").write_text("Console.WriteLine(Doroti.Intl.Tests.GlobalizationContract.Run());\n", encoding="utf-8")
                (consumer / "GlobalizationContract.cs").write_bytes((project.parent / "GlobalizationContract.cs").read_bytes())
                config = ET.Element("configuration")
                sources = ET.SubElement(config, "packageSources")
                ET.SubElement(sources, "clear")
                ET.SubElement(sources, "add", key="local", value=str(target / "packages"))
                ET.SubElement(sources, "add", key="nuget.org", value="https://api.nuget.org/v3/index.json")
                ET.ElementTree(config).write(consumer / "NuGet.config", encoding="utf-8", xml_declaration=True)
                run("package-restore", ["dotnet", "restore", consumer / "Consumer.csproj", "--configfile", consumer / "NuGet.config"])
                run("package-consumer", ["dotnet", "run", "--project", consumer / "Consumer.csproj", "-c", "Release", "--no-restore"])
                licenses = list((consumer / "bin/Release/net10.0/licenses/Doroti.Intl").glob("*Unicode*"))
                if not licenses:
                    raise RuntimeError("The NuGet consumer did not receive the registered Unicode license.")
        else:
            publish = target / "publish"
            options = [f"-p:WasmEnableThreads={str(args.mode == 'threaded').lower()}", f"-p:UseIntlProbe={str(not args.baseline).lower()}"]
            if args.icu_file:
                options.append(f"-p:BlazorIcuDataFileName={args.icu_file}")
            run("configuration", ["dotnet", "msbuild", ROOT / "Doroti/tests/Doroti.Intl.Web.Tests/Doroti.Intl.Web.Tests.csproj",
                                  f"-p:Configuration={args.configuration}", *options,
                                  "-getProperty:TargetFramework,RuntimeFrameworkVersion,InvariantGlobalization,WasmEnableThreads,WasmBuildNative,RunAOTCompilation,BlazorWebAssemblyLoadAllGlobalizationData,BlazorIcuDataFileName"])
            run("publish", ["dotnet", "publish", ROOT / "Doroti/tests/Doroti.Intl.Web.Tests/Doroti.Intl.Web.Tests.csproj",
                            "-c", args.configuration, "--artifacts-path", intermediate, "-o", publish, *options])
            browser_options = (["--baseline"] if args.baseline else []) + (["--expect-missing-culture"] if args.icu_file == "icudt_EFIGS.dat" else [])
            run("chromium", ["node", ROOT / "packages/platforms/web/tests/browser-globalization.mjs", publish / "wwwroot", *browser_options])
            if args.firefox:
                run("firefox", ["node", ROOT / "packages/platforms/web/tests/browser-globalization.mjs", publish / "wwwroot", *browser_options, "--firefox"])
            receipt["publishSize"] = sizes(publish / "wwwroot")
        receipt["status"] = "passed"
    except BaseException:
        receipt["status"] = "failed"
        raise
    finally:
        save()
        print(f"{receipt['status']}: {target / 'receipt.json'}", flush=True)


if __name__ == "__main__":
    main()
