"""Collect a replaceable Qt runtime; never install SDKs during evaluation.

Discover runtime files from a normal SDK, or use an optional doroti-sdk.json
for an explicit redistribution inventory. Unknown modules and SDK build tools
are excluded from discovery. Record automatic discovery separately from review.
"""
import argparse
import hashlib
import json
import os
from pathlib import Path, PurePosixPath
import re
import shutil
import subprocess
import tempfile

SCHEMA = "doroti.qt-runtime/v1"
MODULES = set("Core Gui Widgets OpenGL OpenGLWidgets Network DBus Qml QmlMeta QmlModels QmlWorkerScript QmlIntegration Quick QuickControls2 QuickTemplates2 QuickControls2Impl QuickControls2Basic QuickControls2BasicStyleImpl QuickControls2Fusion QuickControls2FusionStyleImpl QuickControls2Imagine QuickControls2ImagineStyleImpl QuickControls2Material QuickControls2MaterialStyleImpl QuickControls2Universal QuickControls2UniversalStyleImpl QuickControls2FluentWinUI3 QuickControls2FluentWinUI3StyleImpl QuickControls2IOS QuickControls2IOSStyleImpl QuickLayouts QuickShapes QuickShapesDesignHelpers QuickParticles QuickDialogs2 QuickDialogs2QuickImpl QuickDialogs2Utils Svg SvgWidgets XcbQpa WaylandClient WebChannel WebChannelQuick Positioning WebEngineCore WebEngineQuick".split())
MODULES.add("WebEngineQuickDelegatesQml")
MODULES.add("ICU")
MODULES.update("QmlCore QmlNetwork QmlXmlListModel QuickEffects QuickControls2Native QuickVectorImage QuickVectorImageHelpers QuickVectorImageGenerator PositioningQuick LabsFolderListModel WlShellIntegration".split())
ENGINE_MODULES = {"WebEngineCore", "WebEngineQuick", "WebEngineQuickDelegatesQml"}
FORBIDDEN = re.compile(r"Quick3D|QuickTimeline|VirtualKeyboard|WaylandCompositor|CanvasPainter|Graphs|Charts|avcodec.*gpl", re.I)
PROPERTIES = ("DorotiQtVersion", "DorotiQtSdkDirectory", "DorotiQtWebEngine",
              "DorotiQtWebEngineVersion", "DorotiQtWebEngineSdkDirectory",
              "DorotiQtWebEngineDeployment", "DorotiQtWebEnginePackageId",
              "DorotiQtWebEnginePackageVersion")


def digest(path):
    with Path(path).open("rb") as stream:
        return hashlib.file_digest(stream, "sha256").hexdigest()


def run(*args):
    result = subprocess.run(list(map(str, args)), check=True, capture_output=True, text=True)
    return result.stdout


def relative(name):
    if not isinstance(name, str) or not name or "\\" in name or any(ord(c) < 32 for c in name):
        raise ValueError("Invalid runtime path: " + repr(name))
    path = PurePosixPath(name)
    if path.is_absolute() or ".." in path.parts or str(path) != name:
        raise ValueError("Runtime path must stay inside its prefix: " + name)
    return name


def settings(properties, package_format="directory"):
    get = lambda key, default="": properties.get(key) or default
    value = dict(qt=get("DorotiQtVersion", "6.12.0"), sdk=get("DorotiQtSdkDirectory"),
                 webView=get("DorotiQtWebEngine", "false") == "true",
                 webEngine=get("DorotiQtWebEngineVersion", "6.140.0"),
                 engineSdk=get("DorotiQtWebEngineSdkDirectory"),
                 deployment=get("DorotiQtWebEngineDeployment", "bundled"),
                 packageId=get("DorotiQtWebEnginePackageId"),
                 packageVersion=get("DorotiQtWebEnginePackageVersion"))
    if value["qt"] != "6.12.0":
        raise ValueError("Only Qt 6.12.0 is currently accepted for bundling; qualify new patches explicitly")
    if value["deployment"] not in ("bundled", "download"):
        raise ValueError("DorotiQtWebEngineDeployment must be bundled or download")
    if value["deployment"] == "download":
        if not value["webView"] or package_format != "deb":
            raise ValueError("WebEngine download requires WebEngine=true and packageformat=deb")
        if not re.fullmatch(r"[a-z0-9][a-z0-9+.-]+", value["packageId"]):
            raise ValueError("Set DorotiQtWebEnginePackageId for the separate engine package")
        if not re.fullmatch(r"[0-9][A-Za-z0-9.+~:-]*", value["packageVersion"]):
            raise ValueError("Set a valid DorotiQtWebEnginePackageVersion")
        run("dpkg", "--validate-version", value["packageVersion"])
    if value["webView"] and not re.fullmatch(r"6\.[0-9]+\.[0-9]+", value["webEngine"]):
        raise ValueError("Pin the independent WebEngine version")
    if not value["sdk"] or (value["webView"] and not value["engineSdk"]):
        raise ValueError("Publish requires DorotiQtSdkDirectory and, when enabled, DorotiQtWebEngineSdkDirectory")
    return value


def dynamic(path):
    with path.open("rb") as stream:
        header = stream.read(20)
    if header[:4] != b"\x7fELF":
        return None
    if len(header) < 20 or header[4:6] != b"\x02\x01" or header[18:20] != b"\x3e\x00":
        raise ValueError("Qt runtime requires linux-x64 ELF: " + str(path))
    text = run("readelf", "-d", path)
    return dict(needed=re.findall(r"\(NEEDED\).*\[([^\]]+)\]", text),
                soname=next(iter(re.findall(r"\(SONAME\).*\[([^\]]+)\]", text)), path.name))


def discover_sdk(prefix, version, qt, kind):
    """Build an in-memory inventory without writing to the SDK or claiming review."""
    core = "WebEngineCore" if kind == "webengine" else "Core"
    if not (prefix / ("lib/libQt6" + core + ".so." + version)).is_file():
        raise ValueError("SDK lacks the selected Qt version: " + str(prefix) + " / " + version)
    receipt = prefix / "doroti-sdk-install.json"
    installed = json.loads(receipt.read_text()) if receipt.is_file() else None
    lock = installed["lock"] if installed else {}
    if lock and any(lock.get(key) != value for key, value in
                    (("version", version), ("frameworkVersion", qt), ("kind", kind), ("architecture", "linux-x64"))):
        raise ValueError("SDK install lock does not match the selected runtime: " + str(receipt))
    data = dict(schemaVersion="doroti.qt-sdk/v1", version=version, frameworkVersion=qt,
                architecture="linux-x64", kind=kind, inventory="discovered", review=None,
                releaseName=lock.get("releaseName", version), archives=lock.get("archives", []),
                files=[], documents=[], sources=[])

    def add(path, section, module=None, role=None):
        name = relative(path.relative_to(prefix).as_posix())
        if not path.resolve().is_relative_to(prefix) or not path.is_file():
            raise ValueError("SDK file leaves its prefix or is missing: " + str(path))
        checksum = digest(path)
        if installed and installed["files"].get(name) != checksum:
            raise ValueError("Missing or changed installed SDK file: " + str(path))
        entry = dict(path=name, sha256=checksum)
        if module:
            entry.update(module=module, license="Unicode-DFS-2016" if module == "ICU" else "LGPL-3.0-only")
        if role:
            entry["role"] = role
        data[section].append(entry)

    for path in sorted((prefix / "lib").glob("*.so.*")):
        if path.is_symlink():
            continue  # collect() creates SONAME aliases after rebasing.
        match = re.fullmatch(r"libQt6([A-Za-z0-9]+)\.so\." + re.escape(version), path.name)
        if match and match[1] in MODULES and ((match[1] in ENGINE_MODULES) == (kind == "webengine")):
            add(path, "files", match[1])
        elif kind == "framework" and re.fullmatch(r"libicu(?:uc|i18n|data)\.so\.73(?:\.[0-9]+)*", path.name):
            add(path, "files", "ICU")
    qml_roots = {"QtWebEngine": "WebEngineQuick"} if kind == "webengine" else {
        "QML": "Qml", "QtQml": "Qml", "QtQuick": "Quick", "QtCore": "QmlCore",
        "QtNetwork": "QmlNetwork", "QtWebChannel": "WebChannelQuick",
        "QtPositioning": "PositioningQuick", "Qt/labs/folderlistmodel": "LabsFolderListModel"}
    for directory, module in qml_roots.items():
        for path in sorted((prefix / "qml" / directory).rglob("*")):
            name = path.relative_to(prefix).as_posix()
            if path.is_file() and (path.suffix in (".so", ".qml", ".js", ".mjs", ".qmltypes", ".png", ".svg", ".jpg") or path.name == "qmldir"):
                if not FORBIDDEN.search(name) and not any(part in ("tooling", "DesignSupport", "LocalStorage") for part in PurePosixPath(name).parts):
                    add(path, "files", module)
    if kind == "framework":
        plugins = ("platforms", "xcbglintegrations", "wayland-graphics-integration-client",
                   "wayland-shell-integration", "wayland-decoration-client", "imageformats",
                   "iconengines", "tls", "platforminputcontexts", "platformthemes", "networkinformation")
        for directory in plugins:
            for path in sorted((prefix / "plugins" / directory).glob("*.so")):
                if directory != "platforms" or path.name in ("libqxcb.so", "libqwayland.so", "libqwayland-egl.so"):
                    add(path, "files", "Gui")
    else:
        add(prefix / "libexec/QtWebEngineProcess", "files", "WebEngineCore")
        for directory in ("resources", "translations/qtwebengine_locales"):
            for path in sorted((prefix / directory).rglob("*")):
                if path.is_file() and path.suffix in (".pak", ".dat", ".bin"):
                    add(path, "files", "WebEngineCore")
    # Preserve supplied documentation without requiring publishers to assemble it
    # before an ordinary build. No automatic inventory asserts release approval.
    for directory, role in (("sbom", "sbom"), ("licenses", "license"), ("notices", "notice"), ("sources", "source")):
        for path in sorted((prefix / directory).rglob("*")):
            if path.is_file():
                add(path, "sources" if directory == "sources" else "documents", role=role)
    for path in sorted(prefix.glob("config_*.summary")) + sorted(prefix.glob("config_*.opt")):
        add(path, "documents", role="build-configuration")
    return data


def sdk_inventory(prefix, version, qt, kind):
    prefix = Path(prefix).resolve()
    manifest = prefix / "doroti-sdk.json"
    if not manifest.exists():
        data = discover_sdk(prefix, version, qt, kind)
        return validate_inventory(prefix, data, kind)
    data = json.loads(manifest.read_text())
    if (data.get("schemaVersion") != "doroti.qt-sdk/v1" or data.get("version") != version
            or data.get("frameworkVersion") != qt or data.get("architecture") != "linux-x64"
            or data.get("kind") != kind or not data.get("review")):
        raise ValueError("SDK identity/compatibility/review mismatch: " + str(manifest))
    if kind == "webengine" and data.get("buildConfiguration", {}).get("ffmpegLicense") != "LGPL":
        raise ValueError("WebEngine needs a reviewed non-GPL FFmpeg configuration")
    if not data.get("archives") or not data.get("sources") or not data.get("documents") or not data.get("files"):
        raise ValueError("SDK needs exact archives, corresponding sources, documents and runtime inventory")
    roles = {entry.get("role") for entry in data["documents"]}
    if not {"LGPL-3.0", "GPL-3.0", "copyright", "third-party", "replacement", "build-instructions", "sbom"} <= roles:
        raise ValueError("SDK needs license texts, copyright, third-party, replacement and build documents")
    for archive in data["archives"]:
        if not str(archive.get("url", "")).startswith("https://") or not re.fullmatch(r"[a-f0-9]{64}", archive.get("sha256", "")):
            raise ValueError("SDK archive provenance requires HTTPS and SHA-256")
    install_receipt = prefix / "doroti-sdk-install.json"
    if install_receipt.is_file():
        locked = json.loads(install_receipt.read_text())["lock"]
        if data.get("releaseName", version) != locked.get("releaseName", version):
            raise ValueError("SDK review must preserve the actual release name/channel from the install lock")
        locked_archives = {(entry["url"], entry["sha256"]) for entry in locked["archives"]}
        if not {(entry["url"], entry["sha256"]) for entry in data["archives"]} <= locked_archives:
            raise ValueError("SDK runtime provenance disagrees with the installed archive lock")
    for entry in data["sources"]:
        if not entry.get("binaryArchives") or not entry.get("buildInstructions") or not entry.get("patches"):
            raise ValueError("Corresponding source must map exact binary archives, patches and build instructions")
        if not set(entry["binaryArchives"]) <= {item["sha256"] for item in data["archives"]}:
            raise ValueError("Source references unknown binary archives")
        if entry["buildInstructions"] not in {item["path"] for item in data["documents"]}:
            raise ValueError("Corresponding source build instructions must be a shipped document")
    covered = {archive for entry in data["sources"] for archive in entry["binaryArchives"]}
    if covered != {item["sha256"] for item in data["archives"]}:
        raise ValueError("Every binary archive needs corresponding source")
    return validate_inventory(prefix, data, kind)


def validate_inventory(prefix, data, kind):
    entries = data["files"] + data["documents"] + data["sources"]
    seen = set()
    for entry in entries:
        name = relative(entry["path"])
        if name in seen:
            raise ValueError("Duplicate SDK inventory path: " + name)
        seen.add(name)
        path = prefix / name
        if not path.resolve().is_relative_to(prefix) or not path.is_file() or digest(path) != entry["sha256"]:
            raise ValueError("Missing or changed SDK inventory file: " + str(path))
    for entry in data["files"]:
        module = entry.get("module")
        license_id = "Unicode-DFS-2016" if module == "ICU" else "LGPL-3.0-only"
        if module not in MODULES or entry.get("license") != license_id or FORBIDDEN.search(entry["path"]):
            raise ValueError("Unreviewed/GPL-only runtime module: " + entry["path"])
        name = entry["path"]
        if not name.startswith(("lib/", "qml/", "plugins/", "libexec/", "resources/", "translations/")) or re.search(r"\.(a|la|prl|h|hpp|cmake|debug)$", name):
            raise ValueError("SDK build assets cannot enter the runtime: " + name)
        icu = module == "ICU" and re.fullmatch(r"lib/libicu(?:uc|i18n|data)\.so\.73(?:\.[0-9]+)*", name)
        if module == "ICU" and not icu:
            raise ValueError("Only reviewed ICU 73 shared libraries are accepted")
        if name.startswith("lib/") and not icu and not re.fullmatch(r"lib/libQt6[A-Za-z0-9]+\.so(?:\.[0-9]+)+", name):
            raise ValueError("Unreviewed non-Qt library in runtime: " + name)
        elf = dynamic(prefix / name)
        if elf and name.startswith("lib/") and not icu:
            actual = re.fullmatch(r"libQt6([A-Za-z0-9]+)\.so(?:\.[0-9]+)+", Path(name).name)
            if not actual or actual[1] != module:
                raise ValueError("Runtime module declaration disagrees with ELF filename: " + name)
        if kind == "framework" and module in ENGINE_MODULES:
            raise ValueError("Framework inventory must not contain WebEngine")
        entry["elf"] = elf
    qml = {entry["path"] for entry in data["files"] if entry["path"].endswith("/qmldir")}
    for entry in data["files"]:
        path = prefix / entry["path"]
        if entry["path"] in qml:
            text = path.read_text()
            for module in re.findall(r"^(?:(?:default )?import|depends)\s+([A-Za-z0-9_.]+)", text, re.M):
                # Some vendor qmldir files use the CMake module identifier.
                if module == "QtQuickControls2":
                    module = "QtQuick.Controls"
                required = "qml/" + module.replace(".", "/") + "/qmldir"
                # Cross-product QML dependencies are checked after collecting both SDKs.
                if required not in qml and not (kind == "webengine" and module.startswith(("QtQuick", "QtQml", "QtWebChannel"))):
                    raise ValueError("Missing QML dependency: " + module + " (from " + entry["path"] + ")")
    return prefix, data


def validate_native(native, config):
    path = Path(native) / "doroti-qt-build.json"
    build = json.loads(path.read_text())
    expected = dict(schemaVersion="doroti.qt-build/v1", qt=config["qt"],
                    webView=config["webView"], architecture="linux-x64", hostAbi=7)
    if config["webView"]:
        expected["webEngine"] = config["webEngine"]
        expected["webViewAbi"] = 1
    if any(build.get(key) != value for key, value in expected.items()):
        raise ValueError("Prebuilt/native Qt manifest does not match selected runtime: " + str(path))
    for name, value in build.get("shims", {}).items():
        relative(name)
        if digest(Path(native) / name) != value:
            raise ValueError("Prebuilt shim differs from build manifest: " + name)
    required = {"libdoroti_qt_host.so"} | ({"libdoroti_webview_qt.so"} if config["webView"] else set())
    if set(build.get("shims", {})) != required or build.get("quick") is not True or build.get("graphite") is not True:
        raise ValueError("Qt bundle requires matching Quick/Graphite native shims")
    return build


def write_json(path, value):
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(value, indent=2, ensure_ascii=False) + "\n")


def collect(prefix, data, destination):
    destination.mkdir(parents=True)
    files = []
    for section in ("files", "documents", "sources"):
        for entry in data[section]:
            target = destination / entry["path"]
            target.parent.mkdir(parents=True, exist_ok=True)
            shutil.copyfile(prefix / entry["path"], target)
            target.chmod(0o755 if entry["path"] == "libexec/QtWebEngineProcess" else 0o644)
            if section == "files":
                elf = entry["elf"]
                if elf:
                    # Rebase all ELF libraries/plugins/helpers, including transitive loads.
                    to_lib = os.path.relpath(destination / "lib", target.parent)
                    run("patchelf", "--set-rpath", "$ORIGIN:" + "$ORIGIN/" + to_lib, target)
                    # A real SONAME file avoids symlink assumptions in portable installers.
                    if target.parent == destination / "lib" and elf["soname"] != target.name:
                        alias = target.parent / elf["soname"]
                        if alias.exists():
                            raise ValueError("Duplicate SONAME: " + elf["soname"])
                        shutil.copyfile(target, alias)
                        alias.chmod(0o644)
                        files.append(dict(path="lib/" + alias.name, sha256=digest(alias), **elf))
                files.append(dict(path=entry["path"], sha256=digest(target), **(elf or {})))
    write_json(destination / "provenance.json", data)
    return dict(version=data["version"], releaseName=data.get("releaseName", data["version"]), frameworkVersion=data["frameworkVersion"],
                files=files, documents=data["documents"], sources=data["sources"],
                archives=data["archives"], review=data["review"], inventory=data.get("inventory", "reviewed"))


def validate_closure(parts, shims):
    records = [file for part in parts for file in part["files"] if "soname" in file]
    names = {file["soname"] for file in records}
    owners = {}
    for part in parts:
        for file in part["files"]:
            if file["path"].startswith("lib/") and file.get("soname"):
                owner = owners.setdefault(file["soname"], part)
                if owner is not part:
                    raise ValueError("Framework and WebEngine duplicate a shared Qt module: " + file["soname"])
    for file in records + shims:
        for needed in file["needed"]:
            if needed.startswith("libQt6") and needed not in names:
                raise ValueError("Missing bundled transitive Qt dependency: " + needed)
    for part in parts:
        paths = {file["path"] for file in part["files"]}
        if part is parts[0]:
            if not {"plugins/platforms/libqxcb.so", "qml/QtQuick/qmldir"} <= paths or not paths.intersection({"plugins/platforms/libqwayland.so", "plugins/platforms/libqwayland-egl.so"}):
                raise ValueError("Runtime requires xcb, native Wayland and QtQuick QML")
            required_qml = {"qml/" + module + "/qmldir" for module in
                            ("QtQml", "QtQml/Models", "QtQml/WorkerScript", "QtQuick/Window", "QtQuick/Controls", "QtQuick/Templates")}
            if not required_qml <= paths:
                raise ValueError("Runtime requires Quick/Controls/Templates/Window and QtQml imports")
        else:
            required = {"libexec/QtWebEngineProcess", "resources/qtwebengine_resources.pak",
                        "resources/qtwebengine_resources_100p.pak", "resources/qtwebengine_resources_200p.pak",
                        "resources/icudtl.dat", "resources/v8_context_snapshot.bin",
                        "translations/qtwebengine_locales/en-US.pak", "qml/QtWebEngine/qmldir"}
            if not required <= paths:
                raise ValueError("Incomplete WebEngine helper/resources/locales/QML runtime")


def bundle(publish, native, config, engine_output=None):
    publish = Path(publish).resolve()
    build = validate_native(native, config)
    framework = sdk_inventory(config["sdk"], config["qt"], config["qt"], "framework")
    engine = sdk_inventory(config["engineSdk"], config["webEngine"], config["qt"], "webengine") if config["webView"] else None
    if config["deployment"] == "download" and engine_output is None:
        raise ValueError("Separate engine deployment needs an output owned by the deb packager")
    # Re-publish may replace only a previously collector-owned runtime tree.
    previous = publish / "doroti-qt-runtime.json"
    if previous.is_symlink() or (publish / "qt.conf").is_symlink():
        raise ValueError("Qt publication metadata cannot be a symlink")
    if (publish / "qt").exists() and not previous.is_file():
        raise ValueError("Publish qt/ directory has no ownership manifest; use a clean output")
    if previous.exists():
        owned = json.loads(previous.read_text())
        if owned.get("schemaVersion") != SCHEMA or (publish / "qt").is_symlink():
            raise ValueError("Cannot replace an unowned Qt runtime")
    with tempfile.TemporaryDirectory(prefix=".qt-runtime-", dir=publish) as temporary:
        stage = Path(temporary)
        fw_path = "qt/framework/" + config["qt"]
        fw = collect(*framework, stage / fw_path)
        fw["path"] = fw_path
        manifest = dict(schemaVersion=SCHEMA, architecture="linux-x64", framework=fw,
                        deployment=config["deployment"], webView=config["webView"], build=build,
                        sandbox="default-required", replacement="DOROTI_QT_FRAMEWORK_PATH;DOROTI_QT_WEBENGINE_PATH")
        parts = [fw]
        if engine:
            en_path = "qt/webengine/" + config["webEngine"]
            en = collect(*engine, stage / en_path)
            parts.append(en)
            manifest["webEngine"] = en
            if config["deployment"] == "download":
                en["path"] = "/opt/doroti/qt-webengine/" + config["packageId"] + "/" + config["packageVersion"]
                en["packageId"], en["packageVersion"] = config["packageId"], config["packageVersion"]
                write_json(stage / en_path / "engine.json", dict(schemaVersion=SCHEMA, **en))
            else:
                en["path"] = en_path
        shims = [dynamic(Path(native) / name) for name in build["shims"]]
        validate_closure(parts, shims)
        for name, expected in build["shims"].items():
            if digest(publish / name) != expected:
                raise ValueError("Published shim differs from selected prebuilt: " + name)
            search = "$ORIGIN:$ORIGIN/" + fw_path + "/lib"
            if engine and config["deployment"] == "bundled":
                search += ":$ORIGIN/" + en_path + "/lib"
            # The shim can change; Skia provenance remains untouched.
            run("patchelf", "--set-rpath", search, publish / name)
        if engine and config["deployment"] == "download":
            shutil.copytree(stage / en_path, engine_output)
            shutil.rmtree(stage / en_path)
        if (publish / "qt").exists():
            (publish / "qt").rename(stage / "previous-qt")
        try:
            (stage / "qt").rename(publish / "qt")
        except BaseException:
            if (stage / "previous-qt").exists():
                (stage / "previous-qt").rename(publish / "qt")
            raise
        (publish / "qt.conf").write_text("[Paths]\nPrefix=" + fw_path + "\nLibraries=lib\nPlugins=plugins\nQmlImports=qml\n")
        write_json(publish / "doroti-qt-runtime.json", manifest)
    return manifest


def verify(publish, qt=None, web_view=None):
    publish = Path(publish)
    data = json.loads((publish / "doroti-qt-runtime.json").read_text())
    if data.get("schemaVersion") != SCHEMA or (qt and data["framework"]["version"] != qt) or (web_view is not None and data["webView"] != web_view):
        raise ValueError("Qt bundle manifest disagrees with the evaluated profile")
    if data.get("sandbox") != "default-required" or data.get("deployment") not in ("bundled", "download"):
        raise ValueError("Invalid Qt deployment/sandbox contract")
    parts = [data["framework"]] + ([data["webEngine"]] if data["webView"] else [])
    for part in parts:
        if part is not data["framework"] and data["deployment"] == "download":
            continue
        root = publish / relative(part["path"])
        verify_part(root, part)
    validate_closure(parts, [dynamic(publish / name) for name in data["build"]["shims"]])
    return data


def verify_part(root, part):
    for entry in part["files"] + part["documents"] + part["sources"]:
        path = root / relative(entry["path"])
        if not path.is_file() or path.is_symlink() or digest(path) != entry["sha256"]:
            raise ValueError("Qt bundle file missing or changed: " + str(path))
    expected = {entry["path"] for entry in part["files"] + part["documents"] + part["sources"]} | {"provenance.json"}
    if (root / "engine.json").exists():
        expected.add("engine.json")
    actual = {str(path.relative_to(root)) for path in root.rglob("*") if path.is_file()}
    if actual != expected or any(path.is_symlink() for path in root.rglob("*")):
        raise ValueError("Qt runtime contains files outside its recorded inventory")


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("operation", choices=("bundle", "validate-native", "stamp"))
    parser.add_argument("--native", type=Path, required=True)
    parser.add_argument("--publish", type=Path)
    parser.add_argument("--qt", default="6.12.0")
    parser.add_argument("--sdk", default="")
    parser.add_argument("--web-view", choices=("true", "false"), default="false")
    parser.add_argument("--webengine", default="6.140.0")
    parser.add_argument("--engine-sdk", default="")
    parser.add_argument("--deployment", default="bundled")
    args = parser.parse_args()
    if args.operation == "stamp":
        path = args.native / "doroti-qt-build.json"
        data = json.loads(path.read_text())
        data["shims"] = {name: digest(args.native / name) for name in
                         ["libdoroti_qt_host.so"] + (["libdoroti_webview_qt.so"] if data["webView"] else [])}
        write_json(path, data)
    else:
        config = settings(dict(zip(PROPERTIES, (args.qt, args.sdk, args.web_view, args.webengine, args.engine_sdk, args.deployment, "", ""))))
        if args.operation == "validate-native":
            validate_native(args.native, config)
        else:
            bundle(args.publish, args.native, config)


if __name__ == "__main__":
    try:
        main()
    except (ValueError, OSError, KeyError, subprocess.CalledProcessError) as error:
        raise SystemExit("Qt runtime: " + str(error))
