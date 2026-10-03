"""Small maintained suites. All raw evidence belongs to one disposable run directory."""
import json
from pathlib import Path
import re
import shutil
import subprocess
import sys
import uuid
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]
RUN_ROOT = ROOT / "temp/testing"


def source():
    tracked = subprocess.check_output(["git", "ls-files", "-z"], cwd=ROOT).decode().split("\0")
    forbidden = [p for p in tracked if p.startswith("temp/")]
    if forbidden:
        raise RuntimeError(f"Temporary files are tracked: {forbidden}")
    # Current entry points and referenced planning archives, not deleted probe reports.
    docs = [ROOT / p for p in ("history/26-10-03/platform-gap-summary.md", "history/26-09-28/plan-summary.md", "README.md", "README.ko.md", "Doroti/README.md", "history/26-10-03/works/README.md",
        "Doroti/tests/README.md", "Doroti/docs/support-status.md", "Doroti/docs/desktop-windows.md",
        "Doroti/docs/rendering-baselines.md", "Doroti/docs/development-hot-reload.md", "Doroti/tools/vscode-doroti/README.md",
        "Doroti/docs/application-navigation.md", "Doroti/docs/desktop-window-context.md", "Doroti/docs/release-candidates.md",
        "Doroti/docs/platform-views/support-matrix.md", "samples/DorotiTestbedApp/README.md", "samples/DorotiSampleApp2/README.md")]
    docs += list((ROOT / "history/26-10-03/works").rglob("*.md"))
    broken = []
    for doc in set(docs):
        for target in re.findall(r"\]\(([^)]+)\)", doc.read_text(encoding="utf-8-sig")):
            target = target.split("#", 1)[0]
            if not target or re.match(r"[a-z]+:", target):
                continue
            if not (doc.parent / target).exists():
                broken.append(f"{doc.relative_to(ROOT)} -> {target}")
    if broken:
        raise RuntimeError("Broken current documentation links:\n" + "\n".join(broken))
    print("Source: PASS (current documentation paths; tracked temporary-file policy)", flush=True)


def main(suite):
    run = RUN_ROOT / suite.lower() / uuid.uuid4().hex
    run.mkdir(parents=True)
    print(f"Test run: {run} (timeout/failure evidence retained until investigation ends)", flush=True)
    success = False

    def command(name, *args):
        print(f"Running {name}: {' '.join(args)}", flush=True)
        with (run / f"{name}.log").open("w", encoding="utf-8") as log:
            result = subprocess.run(args, cwd=ROOT, stdout=log, stderr=subprocess.STDOUT)
        if result.returncode:
            print((run / f"{name}.log").read_text(encoding="utf-8", errors="replace")[-18000:])
            raise RuntimeError(f"{name} failed: exit {result.returncode}")
        print(f"{name}: PASS", flush=True)

    try:
        if suite in ("Source", "Developer", "Release"):
            source()
            command("runner-contract", sys.executable, "Doroti/tests/runner_contract.py", str(run / "runner"))
            command("installer-contract", sys.executable, "Doroti/tests/installer_contract.py", str(run / "installer"))
        if suite in ("Build", "Developer", "Release"):
            command("widget-regressions", "dotnet", "run", "--project",
                    "Doroti/tests/Doroti.Tests/Doroti.Tests.csproj", "-c", "Debug",
                    "--artifacts-path", str(run / "build"))
        if suite in ("Developer", "Release"):
            command("plugin-regressions", "dotnet", "run", "--project", "Doroti/tests/Doroti.Plugin.Tests/Doroti.Plugin.Tests.csproj", "--artifacts-path", str(run / "plugin-build"))
            command("os-drop-regressions", "dotnet", "run", "--project", "Doroti/tests/Doroti.Drop.Tests/Doroti.Drop.Tests.csproj", "--artifacts-path", str(run / "drop-build"))
            command("web-rendering", "node", "--experimental-transform-types", "--test", "Doroti/tests/web_rendering.mts")
            command("web-textures", "node", "--experimental-transform-types", "--experimental-vm-modules", "--test", "Doroti/tests/web_textures.mts")
        if suite in ("Targets", "Release"):
            for target in ("windowsappsdk/DorotiTestbedApp.WindowsAppSdk.csproj", "web/DorotiTestbedApp.Web.csproj"):
                command(target.split('/')[0], "dotnet", "build", "samples/DorotiTestbedApp/" + target,
                        "-c", "Debug", "--nologo")
            command("web-startup", sys.executable, "Doroti/tests/web_smoke.py", str(run / "web"))
        if suite == "LinuxSmoke":
            command("linux-qt-profiles", sys.executable, "Doroti/tests/linux_qt_build_profiles.py", "--output", str(run / "profiles"))
            command("linux-qt", sys.executable, "Doroti/tests/linux_qt_smoke.py", "--output", str(run / "qt"))
        if suite in ("IOSSmoke", "CatalystSmoke"):
            target = "ios" if suite == "IOSSmoke" else "maccatalyst"
            command("apple-" + target, sys.executable, "Doroti/tests/apple_smoke.py", "--target", target, "--activation", "native-callback" if target == "ios" else "os", "--output", str(run / target))
        if suite == "MacOSSmoke":
            command("macos-appkit", sys.executable, "Doroti/tests/macos_smoke.py", "--output", str(run / "appkit"))
        if suite == "WindowsSmoke":
            command("windows-smoke", sys.executable, "Doroti/tests/windows_smoke.py", str(run / "windows"))
        if suite == "Packages":
            command("package-build", "dotnet", "build", "Doroti/tests/Doroti.Tests/Doroti.Tests.csproj", "-c", "Debug", "--nologo")
            projects = {}
            def visit(project):
                project = project.resolve()
                if project in projects:
                    return
                projects[project] = True
                for reference in ET.parse(project).iter("ProjectReference"):
                    visit(project.parent / reference.attrib["Include"].replace("\\", "/"))
            visit(ROOT / "Doroti/src/Doroti.Testing/Doroti.Testing.csproj")
            visit(ROOT / "Doroti/src/Doroti.Framework.Cupertino/Doroti.Framework.Cupertino.csproj")
            visit(ROOT / "Doroti/src/Doroti.Desktop/Doroti.Desktop.csproj")
            packages = run / "packages"
            for project in projects:
                command("pack-" + project.stem, "dotnet", "pack", str(project), "-c", "Debug", "--no-build", "--output", str(packages), "--nologo")
            consumer = run / "consumer"
            consumer.mkdir()
            shutil.copyfile(ROOT / "Doroti/tests/Doroti.Tests/Program.cs", consumer / "Program.cs")
            shutil.copyfile(ROOT / "Doroti/tests/Doroti.Tests/DesktopCloseRegression.cs", consumer / "DesktopCloseRegression.cs")
            (consumer / "Consumer.csproj").write_text('''<Project Sdk="Microsoft.NET.Sdk">
<PropertyGroup><TargetFramework>net10.0</TargetFramework><OutputType>Exe</OutputType><ImplicitUsings>enable</ImplicitUsings>
<EnableDefaultCompileItems>false</EnableDefaultCompileItems></PropertyGroup>
<ItemGroup><Compile Include="Program.cs;DesktopCloseRegression.cs" /><PackageReference Include="Doroti.Testing" Version="0.3.0-beta" />
<PackageReference Include="Doroti.Desktop" Version="0.3.0-beta" />
<PackageReference Include="Doroti.Framework.Cupertino" Version="0.3.0-beta" />
<PackageReference Include="SkiaSharp.NativeAssets.Win32" Version="4.154.0-preview.1.26454.9" /></ItemGroup></Project>''')
            # Keep the consumer's restore cache independent of installed Doroti packages.
            cache = run / "nuget"
            configuration = ET.Element("configuration")
            sources = ET.SubElement(configuration, "packageSources")
            ET.SubElement(sources, "clear")
            ET.SubElement(sources, "add", key="local", value=str(packages))
            ET.SubElement(sources, "add", key="nuget", value="https://api.nuget.org/v3/index.json")
            ET.ElementTree(configuration).write(consumer / "NuGet.Config", encoding="utf-8", xml_declaration=True)
            command("package-consumer", "dotnet", "run", "--project", str(consumer / "Consumer.csproj"),
                    "-c", "Debug", "-p:RestorePackagesPath=" + str(cache))
        success = True
        print(f"Validation {suite}: PASS; raw artifacts cleaned after summary", flush=True)
    finally:
        if success:
            resolved = run.resolve()
            if not resolved.is_relative_to(RUN_ROOT.resolve()) or run.is_symlink():
                raise RuntimeError(f"Unsafe cleanup path: {run}")
            shutil.rmtree(resolved)
        else:
            print(f"FAIL evidence retained for investigation: {run}", flush=True)


if __name__ == "__main__":
    if len(sys.argv) != 2 or sys.argv[1] not in ("Source", "Build", "Targets", "WindowsSmoke", "LinuxSmoke", "MacOSSmoke", "IOSSmoke", "CatalystSmoke", "Packages", "Developer", "Release"):
        sys.exit("Unknown suite. Use Source, Build, Targets, WindowsSmoke, LinuxSmoke, MacOSSmoke, IOSSmoke, CatalystSmoke, Packages, Developer, or Release.")
    main(sys.argv[1])
