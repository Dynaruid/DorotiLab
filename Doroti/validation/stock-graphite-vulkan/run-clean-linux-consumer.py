"""Linux package/template consumer with isolated NuGet and template caches.

Each command is bounded by the repository's 1200-second wrapper. Product
execution is short (three native resize requests); software Vulkan results are
reported separately from hardware qualification. No Skia source is built.
"""
import datetime
import hashlib
import json
import os
import re
from pathlib import Path
import shutil
import subprocess
import sys
import tempfile
import xml.etree.ElementTree as ET
from xml.sax.saxutils import escape
import zipfile

ROOT = Path(__file__).resolve().parents[3]
DOROTI = ROOT / "Doroti"
VERSION = "0.2.0-linuxreview20260912"
SKIA_VERSION = "4.154.0-preview.1.26454.9"
NATIVE_HASH = "3a7778cce23720da6503e0ca845b423ad366cfe7e6a6b9ad6eea052fc0c91e92"
MANAGED_HASH = "7c8cdb451146fcb12899899286e279e6aa615a5fc9b610f01137a741819f210f"
STAMP = datetime.datetime.now(datetime.timezone.utc).strftime("%Y%m%dT%H%M%SZ")
OUT = DOROTI / "artifacts/stock-graphite" / STAMP / "linux-x64"
COMMANDS = []


def save(path, value):
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(value, indent=2, ensure_ascii=False) + "\n")


def run(label, args, *, cwd=DOROTI, env=None, required=True):
    log = OUT / label
    log.mkdir(parents=True, exist_ok=True)
    command = [sys.executable, str(DOROTI / "validation/run-with-timeout.py"), *map(str, args)]
    start = datetime.datetime.now(datetime.timezone.utc)
    with (log / "output.log").open("w") as stream:
        code = subprocess.run(command, cwd=cwd, env=env, stdout=stream, stderr=subprocess.STDOUT).returncode
    COMMANDS.append(dict(label=label, command=command, cwd=str(cwd), utc=start.isoformat(),
                         timeoutSeconds=1200, exitCode=code,
                         elapsedSeconds=(datetime.datetime.now(datetime.timezone.utc) - start).total_seconds()))
    save(OUT / "commands.json", COMMANDS)
    print(f"{label}: {code}", flush=True)
    if required and code:
        raise RuntimeError(f"{label} failed; see {log / 'output.log'}")
    return code, (log / "output.log").read_text()


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def verify_publish_modes(consumer, project, env):
    binary = consumer / "publish-self-contained"
    run("publish-self-contained", ["dotnet", "publish", project, "-c", "Release", "-o", binary,
                                    "--self-contained", "true"], cwd=consumer, env=env)
    for name, expected in (("libSkiaSharp.so", NATIVE_HASH), ("SkiaSharp.dll", MANAGED_HASH)):
        if sha(binary / name) != expected:
            raise RuntimeError("Self-contained asset mismatch: " + name)
    if not (binary / "libcoreclr.so").is_file() or not (binary / "libdoroti_qt_host.so").is_file():
        raise RuntimeError("Self-contained runtime or Qt helper missing")
    for option in ("PublishTrimmed", "PublishSingleFile"):
        code, output = run("reject/" + option, ["dotnet", "msbuild", project,
                           "-t:ValidateDorotiQtPublishMode", f"-p:{option}=true"], cwd=consumer, env=env, required=False)
        if code == 0 or "DOROTIQT001" not in output:
            raise RuntimeError("Unsupported Qt publish mode was not rejected: " + option)
    save(OUT / "publish-modes.json", dict(frameworkDependent="PASS", selfContained="PASS-assets",
        trimmed="rejected-DOROTIQT001", singleFile="rejected-DOROTIQT001", nativeAot="unsupported-existing-iOS-only-profile"))


def main():
    if sys.platform != "linux":
        raise RuntimeError("Run on Linux x64 with Qt development packages installed.")
    OUT.mkdir(parents=True)
    print(OUT, flush=True)
    run("head", ["git", "rev-parse", "HEAD"])
    run("dirty", ["git", "status", "--short"])
    run("sdk", ["dotnet", "--info"])
    run("vulkan", ["vulkaninfo", "--summary"], required=False)
    run("contract", ["dotnet", "run", "--project", DOROTI / "validation/linux-qt-contract", "-c", "Release"])
    if len(sys.argv) == 3 and sys.argv[1] == "--feed":
        return validate_consumer(Path(sys.argv[2]).resolve())
    feed = OUT / "packages"
    projects = []
    visited = set()

    def visit(path):
        path = path.resolve()
        if path in visited:
            return
        visited.add(path)
        for item in ET.parse(path).iter("ProjectReference"):
            visit(path.parent / item.attrib["Include"].replace("\\", "/"))
        projects.append(path)

    for name in ("Doroti.Target.Linux.Qt.linux-x64", "Doroti.Framework.Material", "Doroti.App.Sdk", "Doroti.Runner.Sdk"):
        visit(DOROTI / f"src/{name}/{name}.csproj")
    projects.append(DOROTI / "templates/Doroti.Templates/Doroti.Templates.csproj")
    for project in projects:
        run("pack/" + project.stem, ["dotnet", "pack", project, "-c", "Release", "-o", feed,
                                    "-p:Version=" + VERSION, "-p:PackageVersion=" + VERSION, "--nologo"])
    inventory = {}
    for package in feed.glob("*.nupkg"):
        with zipfile.ZipFile(package) as archive:
            entries = archive.namelist()
            if any("libdorotigraphite" in e.lower() or "/graphite/" in e.lower() for e in entries):
                raise RuntimeError("Custom Skia asset in " + package.name)
            inventory[package.name] = dict(sha256=sha(package), entries=entries)
    save(OUT / "package-inventory.json", inventory)

    return validate_consumer(feed)


def validate_consumer(feed):
    save(OUT / "feed.json", dict(path=str(feed), packages={p.name: sha(p) for p in feed.glob("*.nupkg")}))
    consumer = Path(tempfile.mkdtemp(prefix="doroti-linux-official-consumer-"))
    shutil.copy2(DOROTI / "global.json", consumer / "global.json")
    config = consumer / "NuGet.Config"
    config.write_text(f'<configuration><packageSources><clear/><add key="local" value="{escape(str(feed))}"/>'
                      '<add key="nuget" value="https://api.nuget.org/v3/index.json"/></packageSources></configuration>')
    env = os.environ.copy()
    env.update(NUGET_PACKAGES=str(consumer / "packages"), DOTNET_CLI_HOME=str(consumer / "cli"))
    env.pop("DOROTI_LINUX_GRAPHITE", None)
    save(OUT / "consumer.json", dict(path=str(consumer), packageCache=env["NUGET_PACKAGES"],
                                    cliHome=env["DOTNET_CLI_HOME"], version=VERSION))
    run("template-install", ["dotnet", "new", "install", feed / f"Doroti.Templates.{VERSION}.nupkg"], cwd=consumer, env=env)
    app = consumer / "app"
    run("template-generate", ["dotnet", "new", "doroti-app", "-n", "LinuxConsumer", "-o", app], cwd=consumer, env=env)
    # C++ preprocessor branches belong to CMake/the compiler, not dotnet new.
    # In particular the template engine must not erase #ifdef DOROTI_QT_GRAPHITE.
    with zipfile.ZipFile(feed / f"Doroti.Templates.{VERSION}.nupkg") as archive:
        prefix = "content/doroti-app/"
        native_files = [e for e in archive.namelist() if e.startswith(prefix + "linux/native/") and not e.endswith("/")]
        if not native_files:
            raise RuntimeError("Template package has no Linux native sources")
        for entry in native_files:
            if (app / entry.removeprefix(prefix)).read_bytes() != archive.read(entry):
                raise RuntimeError("Template engine modified Linux native source: " + entry)
        save(OUT / "template-native-sources.json", dict(status="PASS-byte-identical", files=native_files))
    # Only the candidate package version changes; generated source/native shim
    # and SDK usage are exactly those shipped in the template package.
    for project in app.rglob("*.csproj"):
        project.write_text(project.read_text().replace("0.2.0-beta", VERSION))
    project = app / "linux/LinuxConsumer.Linux.csproj"
    run("restore", ["dotnet", "restore", project, "--configfile", config, "--no-http-cache"], cwd=consumer, env=env)
    binary = consumer / "publish"
    run("publish", ["dotnet", "publish", project, "-c", "Release", "--no-restore", "-o", binary,
                    "--self-contained", "false"], cwd=consumer, env=env)
    assets = next((app / "linux/obj").rglob("project.assets.json"))
    graph = json.loads(assets.read_text())
    # The generated runner references its app project; every Doroti dependency
    # must be a package. The app itself must have no source-project dependency.
    for name, library in graph["libraries"].items():
        if name.startswith("Doroti.") and library["type"] != "package":
            raise RuntimeError("Doroti source project leaked into the clean consumer")
    app_graph = json.loads(next((app / "obj").rglob("project.assets.json")).read_text())
    if any(lib["type"] != "package" for lib in app_graph["libraries"].values()):
        raise RuntimeError("Generated app retained a source project dependency")
    shutil.copy2(assets, OUT / "project.assets.json")
    if sha(binary / "libSkiaSharp.so") != NATIVE_HASH or sha(binary / "SkiaSharp.dll") != MANAGED_HASH:
        raise RuntimeError("Published SkiaSharp differs from the pinned official pair")
    if len(list(binary.rglob("libSkiaSharp.so"))) != 1 or list(binary.rglob("*DorotiGraphite*")):
        raise RuntimeError("Duplicate or custom Skia native library")
    save(OUT / "published-assets.json", [dict(name=p.name, sha256=sha(p)) for p in
                                        (binary / "libSkiaSharp.so", binary / "SkiaSharp.dll", binary / "libdoroti_qt_host.so")])
    official = Path(env["NUGET_PACKAGES"]) / "skiasharp.nativeassets.linux" / SKIA_VERSION
    run("official-signature", ["dotnet", "nuget", "verify", official / f"skiasharp.nativeassets.linux.{SKIA_VERSION}.nupkg", "--all"], cwd=consumer, env=env)
    for name in ("libSkiaSharp.so", "libdoroti_qt_host.so"):
        _, dependencies = run("ldd/" + name, ["ldd", binary / name])
        if "not found" in dependencies:
            raise RuntimeError("Missing loader dependency for " + name)
        run("elf/" + name, ["readelf", "-d", "-V", binary / name])
    probe = OUT / "native-contract"
    run("native-build", ["c++", "-std=c++20", "-Wall", "-Wextra", "-Werror", "-I", app / "linux/native/include",
                         DOROTI / "validation/linux-qt-contract/native.cpp", "-ldl", "-o", probe])
    rows = []
    for qpa in ("wayland", "xcb"):
        qpa_env = dict(env, QT_QPA_PLATFORM=qpa, DOROTI_QT_DIAGNOSTICS="1", DOROTI_QT_VALIDATION_RESIZE_CYCLES="3")
        # The probe needs idle time, so remove the product's resize automation.
        probe_env = {k: v for k, v in qpa_env.items() if k != "DOROTI_QT_VALIDATION_RESIZE_CYCLES"}
        run("native/" + qpa, [probe, binary / "libdoroti_qt_host.so"], cwd=consumer, env=probe_env)
        code, output = run("product/" + qpa, [binary / "LinuxConsumer.Linux"], cwd=consumer, env=qpa_env, required=False)
        official_loaded = "DorotiGraphite official package=SkiaSharp.NativeAssets.Linux" in output
        if not official_loaded:
            raise RuntimeError("Product did not report official library provenance")
        if code != 0:
            raise RuntimeError("Unexpected product failure for " + qpa)
        validation_messages = [line for line in output.splitlines()
                               if re.search(r"VUID-|SYNC-HAZARD|hazard detected|Validation (Error|Warning)", line)]
        software = "type=Cpu" in output
        runtime = ("FAIL-vulkan-validation" if validation_messages else
            "PASS-short-product-software-vulkan" if software else "PASS-short-product")
        rows.append(dict(qpa=qpa, exitCode=code, officialLoaded=official_loaded,
                         softwareVulkan=software, validationMessages=validation_messages, runtime=runtime))
    verify_publish_modes(consumer, project, env)
    validation_failed = any(r["validationMessages"] for r in rows)
    save(OUT / "summary.json", dict(status="FAIL" if validation_failed else "PARTIAL" if any(r["exitCode"] for r in rows) else "PASS-scoped",
        packageConsumer="PASS", generatedTemplate="PASS", nativeContract="PASS", products=rows,
        noSkiaSourceBuild=True, performance="notVerified", physicalDisplay="notVerified", nativeAot="unsupported",
        limitation="Native callback probe deliberately does not draw. Product success does not qualify physical input or GPU stall recovery."))
    return 1 if validation_failed else 0


if __name__ == "__main__":
    try:
        raise SystemExit(main())
    except Exception as error:
        save(OUT / "failure.json", dict(status="FAIL", error=str(error)))
        raise
