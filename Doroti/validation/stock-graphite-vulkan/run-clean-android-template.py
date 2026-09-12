"""Pack the built Android graph, generate a package-only app and install its AAB.

Build the Release/arm64 testbed first. Uses an isolated template/NuGet cache and
retains commands and package hashes. Every command has a 1200-second timeout.
No public feed is changed. Installation requires an explicit device serial.
"""
import argparse
import hashlib
import io
import json
import os
from pathlib import Path
import shutil
import subprocess
import sys
import tempfile
from xml.sax.saxutils import escape
import zipfile

ROOT = Path(__file__).resolve().parents[3]
DOROTI = ROOT / "Doroti"


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--serial", required=True)
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--feed", type=Path, help="Reuse this run's already packed feed")
    args = parser.parse_args()
    out = args.output.resolve()
    out.mkdir(parents=True, exist_ok=True)
    feed = args.feed.resolve() if args.feed else out / "feed"
    feed.mkdir(parents=True, exist_ok=True)
    commands = []

    def run(label, parts, cwd=DOROTI, env=None):
        command = [sys.executable, str(DOROTI / "validation/run-with-timeout.py"), *map(str, parts)]
        with (out / (label + ".log")).open("w", encoding="utf-8") as stream:
            code = subprocess.run(command, cwd=cwd, env=env, stdout=stream, stderr=subprocess.STDOUT).returncode
        commands.append(dict(label=label, command=command, cwd=str(cwd), exitCode=code, timeoutSeconds=1200))
        (out / "commands.json").write_text(json.dumps(commands, indent=2), encoding="utf-8")
        print(label, code, flush=True)
        if code:
            raise RuntimeError(label + " failed; inspect retained log")

    if not args.feed:
        graph = json.loads((ROOT / "DorotiTestbedApp/android/obj/android-arm64/project.assets.json").read_text())
        for name, library in graph["libraries"].items():
            if library["type"] != "project" or not name.startswith("Doroti."):
                continue
            project = (ROOT / "DorotiTestbedApp/android" / library["path"]).resolve()
            # A runner can override referenced-project output paths. Never pack
            # with --no-build: that can select an older standalone DLL/AAR even
            # when the just-built project-reference app used current assemblies.
            command = ["dotnet", "pack", project, "-c", "Release", "--no-restore", "-o", feed,
                       "-p:BuildProjectReferences=false"]
            if project.stem == "Doroti.Host.Maui":
                command += ["-p:TargetFrameworks=net10.0-android", "-p:RuntimeIdentifier=android-arm64"]
            run("pack-" + project.stem, command)
        for name in ("Doroti.App.Sdk", "Doroti.Runner.Sdk", "Doroti.Templates"):
            folder = "templates" if name == "Doroti.Templates" else "src"
            run("pack-" + name, ["dotnet", "pack", DOROTI / folder / name / (name + ".csproj"),
                                 "-c", "Release", "-o", feed])
    hashes = {p.name: hashlib.sha256(p.read_bytes()).hexdigest() for p in feed.glob("*.nupkg")}
    for package in feed.glob("*.nupkg"):
        with zipfile.ZipFile(package) as archive:
            for name in archive.namelist():
                if name.endswith(".aar"):
                    with zipfile.ZipFile(io.BytesIO(archive.read(name))) as aar:
                        if any("skia" in item.lower() or "graphite/" in item.lower() for item in aar.namelist()):
                            raise RuntimeError("Doroti AAR repackages Skia instead of using NativeAssets: " + str(package))
    (out / "packages.json").write_text(json.dumps(hashes, indent=2), encoding="utf-8")
    consumer = Path(tempfile.mkdtemp(prefix="doroti-android-template-"))
    shutil.copy2(DOROTI / "global.json", consumer / "global.json")
    config = consumer / "NuGet.Config"
    config.write_text('<configuration><packageSources><clear/><add key="local" value="' + escape(str(feed)) +
                      '"/><add key="nuget" value="https://api.nuget.org/v3/index.json"/></packageSources></configuration>', encoding="utf-8")
    env = os.environ.copy()
    env.update(NUGET_PACKAGES=str(consumer / "packages"), DOTNET_CLI_HOME=str(consumer / "cli"))
    (out / "consumer.json").write_text(json.dumps(dict(path=str(consumer), environment={k: env[k] for k in
        ("NUGET_PACKAGES", "DOTNET_CLI_HOME")}), indent=2), encoding="utf-8")
    run("template-install", ["dotnet", "new", "install", feed / "Doroti.Templates.0.2.0-beta.nupkg"], consumer, env)
    app = consumer / "app"
    run("template-generate", ["dotnet", "new", "doroti-app", "-n", "AndroidConsumer", "-o", app,
                               "--applicationId", "dev.doroti.work0consumer"], consumer, env)
    manifest = app / "android/AndroidManifest.xml"
    if 'android:version="0x00402000"' not in manifest.read_text():
        raise RuntimeError("Generated template lost Vulkan 1.2 admission")
    project = app / "android/AndroidConsumer.Android.csproj"
    # dotnet restore -r sets RuntimeIdentifiers (plural), whereas this fixed
    # runner validates RuntimeIdentifier during restore as well as build.
    run("restore", ["dotnet", "restore", project, "-p:RuntimeIdentifier=android-arm64", "--configfile", config], consumer, env)
    run("identity", ["dotnet", "msbuild", project, "-p:RuntimeIdentifier=android-arm64",
                     "-getProperty:ApplicationId,ApplicationTitle,ApplicationVersion,ApplicationDisplayVersion"], consumer, env)
    identity = json.loads((out / "identity.log").read_text(encoding="utf-8"))["Properties"]
    if identity != dict(ApplicationId="dev.doroti.work0consumer", ApplicationTitle="AndroidConsumer",
                        ApplicationVersion="2", ApplicationDisplayVersion="0.2.0"):
        raise RuntimeError("Generated Android runner lost workspace application identity: " + str(identity))
    run("install", ["dotnet", "build", project, "-c", "Release", "-r", "android-arm64", "--no-restore",
                    "-t:Install", "-p:AdbTarget=-s%20" + args.serial], consumer, env)
    if "XA4301" in (out / "install.log").read_text(encoding="utf-8"):
        raise RuntimeError("Duplicate native entries were silently dropped by Android packaging")
    for assets in (app / "android/obj").rglob("project.assets.json"):
        graph = json.loads(assets.read_text())
        for name, library in graph["libraries"].items():
            if name.startswith("Doroti.") and library["type"] != "package":
                raise RuntimeError("Doroti source dependency leaked into consumer: " + name)
        shutil.copy2(assets, out / "project.assets.json")
    for package, expected in hashes.items():
        matches = list((consumer / "packages").glob("*/*/" + package.lower()))
        # Templates are installed in the isolated CLI cache, not the package graph.
        if not matches and package.startswith("Doroti.Templates."):
            continue
        if not matches:
            # The testbed includes Cupertino, which the basic template need not use.
            continue
        if any(hashlib.sha256(p.read_bytes()).hexdigest() != expected for p in matches):
            raise RuntimeError("Consumer resolved a different local package: " + package)
    apks = list((app / "android/bin").rglob("*-Signed.apk"))
    if len(apks) != 1:
        raise RuntimeError("Expected one generated APK")
    with zipfile.ZipFile(apks[0]) as archive:
        entries = [e for e in archive.infolist() if e.filename.endswith("libSkiaSharp.so")]
        if len(entries) != 1 or entries[0].filename != "lib/arm64-v8a/libSkiaSharp.so":
            raise RuntimeError("Unexpected native Skia inventory")
        native_hash = hashlib.sha256(archive.read(entries[0])).hexdigest()
        if native_hash != "63af1ec283b86965542bca1400ae446e6a179a7be5b6187e69aa5fa0bd49e180":
            raise RuntimeError("Generated APK has nonofficial Skia")
    (out / "result.json").write_text(json.dumps(dict(status="PASS-generated-package-app-install",
        consumer=str(consumer), apk=str(apks[0]), nativeSha256=native_hash,
        runtime="Run device-lifecycle.py separately; install is not rendering evidence"), indent=2), encoding="utf-8")


if __name__ == "__main__":
    main()
