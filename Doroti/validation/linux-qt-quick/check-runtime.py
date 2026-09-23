#!/usr/bin/env python3
"""Check the system Qt closure of a published Linux Doroti app before launch.

This is a deployment diagnostic, not a substitute for running the app on the
target compositor. It reports each missing group independently as JSON.
"""
import argparse
import json
import os
from pathlib import Path
import subprocess
import sys


def qt_paths(executable):
    result = subprocess.run([executable, "--query"], capture_output=True, text=True)
    if result.returncode:
        raise RuntimeError(result.stderr.strip() or f"{executable} --query failed")
    return dict(line.split(":", 1) for line in result.stdout.splitlines() if ":" in line)


def inferred_qt_paths(host):
    """Find standard Qt runtime layout when qtpaths6 dev tools are not installed."""
    if not host.is_file():
        return None
    result = subprocess.run(["ldd", str(host)], capture_output=True, text=True)
    core = next((Path(line.split("=>", 1)[1].strip().split(" ", 1)[0])
                 for line in result.stdout.splitlines()
                 if "libQt6Core.so.6 => /" in line), None)
    if core is None:
        return None
    libdir = core.parent
    # Debian/Ubuntu multiarch and typical /usr/lib64 packages use qt6 below
    # the library directory; upstream Qt installs put plugins/qml at prefix.
    if libdir.name == "lib":
        prefix = libdir.parent
        module_root = prefix
        data_root = prefix
        libexec = prefix / "libexec"
    elif libdir.name == "lib64":
        prefix = libdir.parent
        module_root = libdir / "qt6"
        data_root = prefix / "share/qt6"
        libexec = module_root / "libexec"
    else:
        prefix = libdir.parent.parent
        module_root = libdir / "qt6"
        data_root = prefix / "share/qt6"
        libexec = libdir.parent / "qt6/libexec"
    return {"QT_INSTALL_PLUGINS": str(module_root / "plugins"),
            "QT_INSTALL_QML": str(module_root / "qml"),
            "QT_INSTALL_DATA": str(data_root),
            "QT_INSTALL_TRANSLATIONS": str(data_root / "translations"),
            "QT_INSTALL_LIBEXECS": str(libexec),
            "QT_VERSION": None}


def split_paths(value):
    return [Path(part) for part in value.split(os.pathsep) if part]


def check_file(results, name, paths, executable=False):
    paths = list(dict.fromkeys(paths))
    found = next((path for path in paths if path.is_file() and
                  os.access(path, os.R_OK | (os.X_OK if executable else 0))), None)
    results.append({"name": name, "status": "passed" if found else "failed",
                    "path": str(found) if found else None,
                    "candidates": [str(path) for path in paths]})


def check_linker(results, shim):
    if not shim.is_file():
        return
    result = subprocess.run(["ldd", str(shim)], capture_output=True, text=True)
    missing = [line.strip() for line in result.stdout.splitlines() if "not found" in line]
    results.append({"name": f"linker:{shim.name}",
                    "status": "passed" if result.returncode == 0 and not missing else "failed",
                    "missing": missing, "error": result.stderr.strip() if result.returncode else None})


def check_qml_module(results, module, roots):
    relative = Path(*module.split(".")) / "qmldir"
    candidates = [root / relative for root in roots]
    check_file(results, f"qml:{module}", candidates)
    found = next((path for path in candidates if path.is_file()), None)
    if not found:
        return
    # A non-optional plugin declared by qmldir must be loadable from its
    # module directory. Optional plugins may be registered by a Qt library.
    for line in found.read_text(errors="replace").splitlines():
        parts = line.split()
        if len(parts) >= 2 and parts[0] == "plugin":
            check_file(results, f"qml-plugin:{module}",
                       [found.parent / f"lib{parts[1]}.so"])


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--publish", type=Path, required=True)
    parser.add_argument("--qpa", choices=("wayland", "xcb"), required=True)
    parser.add_argument("--quick", action="store_true")
    parser.add_argument("--webengine", action="store_true")
    parser.add_argument("--qtpaths", default="qtpaths6", help="qtpaths for the Qt runtime used by the shim")
    parser.add_argument("--output", type=Path, help="Optional JSON result file")
    args = parser.parse_args()
    if args.webengine and not args.quick:
        parser.error("--webengine requires --quick")

    publish = args.publish.resolve()
    checks = []
    host = publish / "libdoroti_qt_host.so"
    check_file(checks, "host-shim", [host])
    check_linker(checks, host)
    if args.webengine:
        web = publish / "libdoroti_webview_qt.so"
        check_file(checks, "webengine-shim", [web])
        check_file(checks, "webengine-manifest", [publish / "doroti-webview-runtime.json"])
        check_linker(checks, web)

    try:
        locations = qt_paths(args.qtpaths)
    except (OSError, RuntimeError) as error:
        locations = inferred_qt_paths(host) or {}
        checks.append({"name": "qt-paths", "status": "passed" if locations else "failed",
                       "source": "Qt6Core linker path (standard layout)" if locations else None,
                       "error": str(error)})
    else:
        checks.append({"name": "qt-paths", "status": "passed",
                       "version": locations.get("QT_VERSION"), "source": args.qtpaths})

    plugin_roots = split_paths(os.environ.get("QT_QPA_PLATFORM_PLUGIN_PATH", ""))
    plugin_roots += [root / "platforms" for root in split_paths(os.environ.get("QT_PLUGIN_PATH", ""))]
    if locations.get("QT_INSTALL_PLUGINS"):
        plugin_roots.append(Path(locations["QT_INSTALL_PLUGINS"]) / "platforms")
    plugins = ("libqwayland.so", "libqwayland-egl.so", "libqwayland-generic.so") \
        if args.qpa == "wayland" else ("libqxcb.so",)
    check_file(checks, f"qpa:{args.qpa}",
               [root / plugin for root in plugin_roots for plugin in plugins])

    if args.quick:
        qml_roots = split_paths(os.environ.get("QML_IMPORT_PATH", ""))
        qml_roots += split_paths(os.environ.get("QML2_IMPORT_PATH", ""))
        if locations.get("QT_INSTALL_QML"):
            qml_roots.append(Path(locations["QT_INSTALL_QML"]))
        modules = ["QtQuick", "QtQuick.Controls", "QtQuick.Controls.Basic"]
        if args.webengine:
            modules += ["QtWebEngine", "QtWebEngine.ControlsDelegates", "QtWebChannel"]
        for module in modules:
            check_qml_module(checks, module, qml_roots)

    if args.webengine:
        helper = os.environ.get("QTWEBENGINEPROCESS_PATH")
        helper_path = (Path(helper) if helper else
                       Path(locations.get("QT_INSTALL_LIBEXECS", "")) / "QtWebEngineProcess")
        check_file(checks, "webengine-helper", [helper_path], executable=True)
        resources = Path(os.environ.get("QTWEBENGINE_RESOURCES_PATH") or
                         str(Path(locations.get("QT_INSTALL_DATA", "")) / "resources"))
        for name in ("qtwebengine_resources.pak", "qtwebengine_resources_100p.pak",
                     "qtwebengine_resources_200p.pak"):
            check_file(checks, f"webengine-resource:{name}", [resources / name])
        locales = Path(os.environ.get("QTWEBENGINE_LOCALES_PATH") or
                       str(Path(locations.get("QT_INSTALL_TRANSLATIONS", "")) / "qtwebengine_locales"))
        check_file(checks, "webengine-locale:en-US", [locales / "en-US.pak"])

    report = {"schemaVersion": 1, "publish": str(publish), "qpa": args.qpa,
              "quick": args.quick, "webengine": args.webengine, "checks": checks,
              "status": "failed" if any(c["status"] == "failed" for c in checks) else "passed"}
    serialized = json.dumps(report, indent=2) + "\n"
    if args.output:
        args.output.write_text(serialized)
    print(serialized, end="")
    return 0 if report["status"] == "passed" else 1


if __name__ == "__main__":
    raise SystemExit(main())
