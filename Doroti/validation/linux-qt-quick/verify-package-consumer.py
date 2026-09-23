#!/usr/bin/env python3
"""Pack the Linux dependency closure and build a template app outside the checkout."""
import argparse
import hashlib
import json
import os
import signal
from pathlib import Path
import subprocess
import sys
import time
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[3]
SRC = ROOT / "Doroti/src"
TEMPLATE = ROOT / "Doroti/templates/Doroti.Templates/Doroti.Templates.csproj"
TIMEOUT = ROOT / "Doroti/validation/run-with-timeout.py"
ROOTS = ("Doroti.Target.Linux.Qt.linux-x64", "Doroti.App.Sdk", "Doroti.Runner.Sdk",
         "Doroti.Framework.Widgets", "Doroti.Framework.Material")


def projects():
    seen, order = set(), []
    def visit(path):
        path = path.resolve()
        if path in seen:
            return
        seen.add(path)
        for ref in ET.parse(path).iter("ProjectReference"):
            visit(path.parent / ref.attrib["Include"].replace("\\", "/"))
        order.append(path)
    for name in ROOTS:
        visit(SRC / name / f"{name}.csproj")
    return order


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--output", type=Path, required=True, help="New directory outside the checkout")
    parser.add_argument("--configuration", choices=("Debug", "Release"), default="Debug")
    parser.add_argument("--quick", action="store_true")
    parser.add_argument("--webengine", action="store_true")
    parser.add_argument("--qpa", choices=("wayland", "xcb"), help="Run the published app on this host")
    args = parser.parse_args()
    if args.webengine and not args.quick:
        parser.error("--webengine requires --quick")
    output = args.output.resolve()
    if output == ROOT or ROOT in output.parents:
        parser.error("Output must be outside the source checkout")
    output.mkdir(parents=True, exist_ok=False)
    feed = output / "feed"
    feed.mkdir()
    index = {"schemaVersion": 1, "configuration": args.configuration,
             "quick": args.quick, "webengine": args.webengine, "steps": []}
    env = dict(os.environ, NUGET_PACKAGES=str(output / "nuget-packages"))

    def step(name, command, cwd=ROOT, custom_env=None):
        log = output / f"{name}.log"
        with log.open("w") as stream:
            result = subprocess.run([sys.executable, str(TIMEOUT), *map(str, command)],
                                    cwd=cwd, env=custom_env or env, stdout=stream,
                                    stderr=subprocess.STDOUT)
        index["steps"].append({"name": name, "exitCode": result.returncode,
                               "status": "passed" if result.returncode == 0 else "failed",
                               "log": log.name})
        (output / "index.json").write_text(json.dumps(index, indent=2) + "\n")
        print(f"{name}: {index['steps'][-1]['status']}", flush=True)
        return result.returncode == 0

    for project in projects():
        name = project.parent.name
        # Existing Debug/Release outputs are used; no ProjectReference is copied
        # into the consumer. Pack remains sequential because projects share obj.
        cached = (project.parent / "obj/project.assets.json").is_file()
        flags = ["--no-build", "--no-restore"] if cached else []
        if not step(f"pack-{name}", ["dotnet", "pack", project, "-c", args.configuration,
                                     *flags, "-o", feed, "-v:q"]):
            return 1
    if not step("pack-template", ["dotnet", "pack", TEMPLATE, "-c", args.configuration,
                                  "-o", feed, "-v:q"]):
        return 1
    template_package = feed / "Doroti.Templates.0.3.0-beta.nupkg"
    if not template_package.is_file():
        raise RuntimeError("Template package missing")
    if not step("install-template", ["dotnet", "new", "install", template_package, "--force"]):
        return 1
    try:
        app = output / "consumer"
        if not step("new-app", ["dotnet", "new", "doroti-app", "--name", "QtPackageProbe",
                                "--output", app]):
            return 1
        refs = [str(path.relative_to(app)) for path in app.rglob("*.csproj")
                if any(True for _ in ET.parse(path).iter("ProjectReference"))]
        if refs:
            raise RuntimeError(f"Generated app contains source ProjectReference: {refs}")
        index["projectReferences"] = refs
        config = app / "NuGet.Config"
        config.write_text(f'''<configuration><packageSources><clear/>
<add key="local" value="{feed}"/><add key="nuget" value="https://api.nuget.org/v3/index.json"/>
</packageSources><packageSourceMapping><packageSource key="local"><package pattern="Doroti.*"/></packageSource>
<packageSource key="nuget"><package pattern="*"/></packageSource></packageSourceMapping></configuration>\n''')
        project = app / "linux/QtPackageProbe.Linux.csproj"
        props = [f"-p:DorotiQtQuick={'true' if args.quick else 'false'}",
                 f"-p:DorotiQtWebEngine={'true' if args.webengine else 'false'}"]
        if not step("consumer-build", ["dotnet", "build", project, "-c", args.configuration,
                                       *props, "-v:q"], cwd=app):
            return 1
        publish = output / "publish"
        if not step("consumer-publish", ["dotnet", "publish", project, "-c", args.configuration,
                                         "--no-restore", *props, "-o", publish, "-v:q"], cwd=app):
            return 1
        expected = ("libdoroti_qt_host.so", "libdoroti_webview_qt.so") if args.webengine else ("libdoroti_qt_host.so",)
        if any(not (publish / name).is_file() for name in expected):
            raise RuntimeError("Expected native shim is absent from publish")
        if not args.webengine and any((publish / name).exists() for name in
                                      ("libdoroti_webview_qt.so", "doroti-webview-runtime.json")):
            raise RuntimeError("WebEngine OFF publish retained a WebEngine shim or manifest")
        index["packages"] = {path.name: hashlib.sha256(path.read_bytes()).hexdigest()
                             for path in feed.glob("*.nupkg")}
        index["publishShims"] = {path.name: hashlib.sha256(path.read_bytes()).hexdigest()
                                 for path in publish.glob("libdoroti_*.so")}
        if args.qpa:
            log = output / "consumer-run.log"
            run_env = dict(env, QT_QPA_PLATFORM=args.qpa,
                           DOROTI_QT_VALIDATION_RESIZE_CYCLES="10", DOROTI_QT_DIAGNOSTICS="1")
            with log.open("w") as stream:
                process = subprocess.Popen(["dotnet", str(publish / "QtPackageProbe.Linux.dll")],
                                           cwd=publish, env=run_env, stdout=stream,
                                           stderr=subprocess.STDOUT, start_new_session=True)
                mapped = set()
                start = time.monotonic()
                while process.poll() is None and time.monotonic() - start < 40:
                    try:
                        for line in Path(f"/proc/{process.pid}/maps").read_text().splitlines():
                            path = line.split()[-1]
                            if path.startswith("/") and any(token in path for token in
                                                             ("libdoroti_", "libSkiaSharp", "/libQt6")):
                                mapped.add(path)
                    except (OSError, IndexError):
                        pass
                    time.sleep(.02)
                if process.poll() is None:
                    os.killpg(process.pid, signal.SIGKILL)
                    process.wait()
            mapped_doroti = {Path(path).name for path in mapped if "libdoroti_" in path}
            valid = (process.returncode == 0 and "doroti.qt.summary=" in log.read_text(errors="replace")
                     and set(expected) <= mapped_doroti
                     and any(Path(path).name == "libSkiaSharp.so" and str(publish) in path
                             for path in mapped)
                     and all(str(publish) in path for path in mapped if "libdoroti_" in path))
            index["consumerRun"] = {
                "status": "passed" if valid else "failed", "exitCode": process.returncode,
                "qpa": args.qpa, "log": log.name,
                "mappedModules": {path: hashlib.sha256(Path(path).read_bytes()).hexdigest()
                                  for path in sorted(mapped) if Path(path).is_file()},
            }
            if not valid:
                (output / "index.json").write_text(json.dumps(index, indent=2) + "\n")
                return 1
        (output / "index.json").write_text(json.dumps(index, indent=2) + "\n")
        return 0
    finally:
        if not step("uninstall-template", ["dotnet", "new", "uninstall", "Doroti.Templates"]):
            raise RuntimeError("Template package could not be removed from the local dotnet new cache")


if __name__ == "__main__":
    raise SystemExit(main())
