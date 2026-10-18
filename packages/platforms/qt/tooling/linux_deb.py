"""Provider-owned Debian 13/amd64 packager. Shipped in Doroti.Tool.Qt.

No host installation, portable installer, shell evaluation or maintainer scripts.
The CLI owns this process; the process owns publish, staging and all child tools.
"""
import argparse
from datetime import datetime, timezone
from email.utils import format_datetime
import gzip
import hashlib
import json
import os
from pathlib import Path
import platform
import re
import shutil
import signal
import struct
import subprocess
import sys
import tempfile
from urllib.parse import urlsplit

PROPERTIES = (
    "ApplicationId", "ApplicationTitle", "AssemblyName", "Version", "DorotiLinuxIcon",
    "TargetFramework", "RuntimeIdentifier", "DorotiTarget", "DorotiHostKind", "NETCoreSdkVersion", "ProjectAssetsFile",
    "SelfContained", "PublishAot", "PublishTrimmed", "PublishSingleFile", "PublishReadyToRun",
    "UseAppHost", "DorotiQtQuick", "DorotiQtGraphite", "DorotiQtWebEngine", "DorotiGStreamerTextures",
    "DorotiDebPackageId", "DorotiDebMaintainer", "DorotiDebDescription", "DorotiDebHomepage",
    "DorotiDebCategories", "DorotiDebMimeTypes", "DorotiDebUriSchemes", "DorotiDebVersion",
    "DorotiDebRevision", "DorotiDebCopyright", "DorotiDebExcludeLttngProvider",
)
# Profile properties are opt-in for deb only. NativeAOT/trimming/single-file/R2R
# are checked BEFORE these properties are applied, never silently downgraded.
PUBLISH_PROPERTIES = ["-p:SelfContained=true", "-p:PublishTrimmed=false", "-p:PublishSingleFile=false",
                      "-p:PublishAot=false", "-p:PublishReadyToRun=false", "-p:UseAppHost=true"]
QUICK_PACKAGES = (
    "libqt6core6t64", "libqt6gui6", "libqt6widgets6", "libqt6quick6", "libqt6qml6",
    "libqt6quickcontrols2-6", "qt6-wayland", "qml6-module-qtquick", "qml6-module-qtquick-controls",
    "qml6-module-qtquick-window", "qml6-module-qtquick-templates", "qml6-module-qtqml",
    "qml6-module-qtqml-models", "qml6-module-qtqml-workerscript",
)
SYSTEM_PACKAGES = ("libicu76", "libssl3t64", "libgssapi-krb5-2", "zlib1g", "libvulkan1",
                   "libfontconfig1", "fonts-dejavu-core")
WEB_PACKAGES = ("libqt6webenginequick6", "libqt6webchannel6", "qml6-module-qtwebengine",
                "qml6-module-qtwebchannel", "libqt6webenginecore6-bin", "libqt6webengine6-data")
TOOLS = ("dpkg-deb", "dpkg-shlibdeps", "dpkg-query", "dpkg", "readelf", "objdump",
         "desktop-file-validate", "lintian", "pkg-config")
STOP_PATH = None
SESSION_ID = None


def check_stop():
    if STOP_PATH and STOP_PATH.is_file():
        try:
            if json.loads(STOP_PATH.read_text())["sessionId"] == SESSION_ID:
                raise InterruptedError("deb packaging canceled")
        except (ValueError, KeyError):
            pass


def command(arguments, cwd=None, echo=False, allowed=(0,)):
    check_stop()
    process = subprocess.Popen(list(map(str, arguments)), cwd=cwd, stdout=subprocess.PIPE,
                               stderr=subprocess.PIPE, text=True, start_new_session=True,
                               env=os.environ | {"LC_ALL": "C"})
    try:
        while True:
            try:
                output, error = process.communicate(timeout=0.25)
                break
            except subprocess.TimeoutExpired:
                check_stop()
        if echo:
            print(output, end="", flush=True)
            print(error, end="", file=sys.stderr, flush=True)
        if process.returncode not in allowed:
            raise ValueError(f"{arguments[0]} exited {process.returncode}:\n{output}\n{error}")
        return output, error
    finally:
        if process.poll() is None:
            os.killpg(process.pid, signal.SIGTERM)
            try:
                process.wait(timeout=3)
            except subprocess.TimeoutExpired:
                os.killpg(process.pid, signal.SIGKILL)
                process.wait()


def line(value, name, required=True):
    if not isinstance(value, str) or (required and not value.strip()) or any(ord(c) < 32 or ord(c) == 127 for c in value):
        raise ValueError(f"{name} requires a single nonempty line without control characters")
    return value.strip()


def debian_version(version, revision="1"):
    # Reject build metadata rather than dropping it and producing colliding files.
    if "+" in version:
        raise ValueError("Build metadata is unsupported; set a unique DorotiDebVersion without '+'")
    match = re.fullmatch(r"(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)(?:-([0-9A-Za-z.-]+))?", version)
    if not match or not re.fullmatch(r"[1-9][0-9]*", revision):
        raise ValueError("DorotiDebVersion/Version requires X.Y.Z[-prerelease]; DorotiDebRevision requires a positive integer")
    prerelease = match[4]
    if prerelease and any(not item or (item.isdigit() and len(item) > 1 and item[0] == "0") for item in prerelease.split(".")):
        raise ValueError("Invalid semantic prerelease version")
    return ".".join(match.group(1, 2, 3)) + ("~" + prerelease if prerelease else "") + "-" + revision


def list_property(value, pattern, name):
    values = value.strip(";").split(";") if value else []
    if any(not re.fullmatch(pattern, item) for item in values) or len(values) != len(set(values)):
        raise ValueError(f"Invalid or duplicate {name}")
    return values


def metadata_from_model(model, project):
    properties = model["Properties"]
    def prop(name, fallback=""):
        return line(properties.get(name, fallback) or fallback, name, required=False)
    for name, expected in (("TargetFramework", "net10.0"), ("RuntimeIdentifier", "linux-x64"),
                           ("DorotiTarget", "Linux"), ("DorotiHostKind", "Qt"),
                           ("DorotiQtQuick", "true"), ("DorotiQtGraphite", "true")):
        if prop(name) != expected:
            raise ValueError(f"deb requires {name}={expected}; evaluated {prop(name)!r}")
    for name in ("PublishAot", "PublishTrimmed", "PublishSingleFile", "PublishReadyToRun", "DorotiGStreamerTextures"):
        if prop(name).lower() not in ("", "false"):
            raise ValueError(f"deb does not support {name}={prop(name)}")
    if prop("UseAppHost").lower() == "false":
        raise ValueError("deb requires UseAppHost=true; remove the incompatible project setting")
    if prop("DorotiQtWebEngine") not in ("true", "false"):
        raise ValueError("DorotiQtWebEngine must be true or false")
    if prop("DorotiDebExcludeLttngProvider") not in ("", "true", "false"):
        raise ValueError("DorotiDebExcludeLttngProvider must be true or false")
    identity = prop("ApplicationId")
    if not re.fullmatch(r"[A-Za-z][A-Za-z0-9_-]*(?:\.[A-Za-z0-9_-]+)+", identity):
        raise ValueError("ApplicationId must be a stable reverse DNS identifier")
    resources = [item for item in model["Items"]["EmbeddedResource"] if item.get("LogicalName") == "Doroti.Application.Manifest"]
    if len(resources) != 1:
        raise ValueError("Exactly one evaluated Doroti.Application.Manifest is required")
    manifest = json.loads(Path(resources[0]["FullPath"]).read_text(encoding="utf-8-sig"))
    if manifest.get("applicationId") != identity or manifest.get("targetRid") != "linux-x64":
        raise ValueError("ApplicationId/RID disagrees with the selected application manifest")
    package = prop("DorotiDebPackageId")
    if not re.fullmatch(r"[a-z0-9][a-z0-9+.-]+", package):
        raise ValueError("Set an explicit DorotiDebPackageId (at least two lowercase Debian package name characters)")
    maintainer = prop("DorotiDebMaintainer")
    if not re.fullmatch(r"[^<>]+ <[^\s<>@]+@[^\s<>@]+\.[^\s<>@]+>", maintainer):
        raise ValueError("DorotiDebMaintainer requires Name <email@example.org>")
    title = line(prop("ApplicationTitle"), "ApplicationTitle")
    description = line(prop("DorotiDebDescription"), "DorotiDebDescription")
    executable = prop("AssemblyName")
    if not executable or executable in (".", "..") or any(c in executable for c in "/\\\"'`$%"):
        raise ValueError("AssemblyName is not a safe executable filename")
    homepage = prop("DorotiDebHomepage")
    if homepage and (urlsplit(homepage).scheme not in ("https", "http") or not urlsplit(homepage).netloc):
        raise ValueError("DorotiDebHomepage must be an absolute HTTP(S) URL")
    categories = list_property(prop("DorotiDebCategories", "Utility"), r"[A-Za-z][A-Za-z0-9-]*", "DorotiDebCategories")
    mime = list_property(prop("DorotiDebMimeTypes"), r"[a-zA-Z0-9!#$&^_.+-]+/[a-zA-Z0-9!#$&^_.+-]+", "DorotiDebMimeTypes")
    schemes = list_property(prop("DorotiDebUriSchemes"), r"[a-z][a-z0-9+.-]*", "DorotiDebUriSchemes")
    if any(value.startswith("x-scheme-handler/") for value in mime):
        raise ValueError("Declare URI handlers in DorotiDebUriSchemes")
    icon = Path(line(prop("DorotiLinuxIcon"), "DorotiLinuxIcon"))
    copyright_file = Path(line(prop("DorotiDebCopyright"), "DorotiDebCopyright"))
    if not icon.is_absolute():
        icon = project.parent / icon
    if not copyright_file.is_absolute():
        copyright_file = project.parent / copyright_file
    png_size(icon)
    if not copyright_file.is_file() or not copyright_file.read_text(encoding="utf-8-sig").strip():
        raise ValueError("DorotiDebCopyright must reference a nonempty UTF-8 license/copyright document")
    version = prop("DorotiDebVersion") or prop("Version")
    return dict(packageId=package, applicationId=identity, title=title, description=description,
                maintainer=maintainer, homepage=homepage, categories=categories, mimeTypes=mime,
                uriSchemes=schemes, executable=executable, version=version,
                debianVersion=debian_version(version, prop("DorotiDebRevision", "1")),
                architecture="amd64", rid="linux-x64", framework="net10.0", configuration="Release",
                selfContained=True, quick=True, webView=prop("DorotiQtWebEngine") == "true",
                excludeLttngProvider=prop("DorotiDebExcludeLttngProvider") == "true",
                icon=str(icon.resolve()), copyright=str(copyright_file.resolve()),
                sdk=prop("NETCoreSdkVersion"), projectAssetsFile=prop("ProjectAssetsFile"), project=str(project))


def png_size(path):
    with path.open("rb") as stream:
        header = stream.read(24)
    if len(header) != 24 or header[:8] != b"\x89PNG\r\n\x1a\n" or header[12:16] != b"IHDR":
        raise ValueError(f"Expected PNG icon: {path}")
    width, height = struct.unpack(">II", header[16:24])
    if width != height or not 1 <= width <= 4096:
        raise ValueError("DorotiLinuxIcon must be a square PNG of at most 4096 pixels")
    return width


def environment():
    if sys.version_info < (3, 11):
        raise ValueError("deb packaging requires Python 3.11 or newer")
    release = dict(item.split("=", 1) for item in Path("/etc/os-release").read_text().splitlines() if "=" in item)
    release = {key: value.strip('"') for key, value in release.items()}
    architecture = command(["dpkg", "--print-architecture"])[0].strip()
    if release.get("ID") != "debian" or release.get("VERSION_ID") != "13" or architecture != "amd64":
        raise ValueError("deb initially requires a Debian 13 amd64 build environment; build inside a disposable Debian 13 container/VM")
    for tool in TOOLS:
        if not shutil.which(tool):
            raise ValueError(f"deb packaging requires {tool}; ordinary directory publish does not")
    libraries = {package: command(["dpkg-query", "-W", "-f=${Version}", package])[0] for package in ("libc6", "libstdc++6", "libgcc-s1")}
    return dict(osRelease=release, architecture=architecture, kernel=platform.release(), libraries=libraries,
                tools={tool: command([tool, "--version"])[0].splitlines()[0] for tool in ("dpkg-deb", "readelf", "lintian")})


def runtime_dependencies(metadata):
    result = {}
    packages = SYSTEM_PACKAGES + QUICK_PACKAGES + (WEB_PACKAGES if metadata["webView"] else ())
    for package in packages:
        output = command(["dpkg-query", "-W", "-f=${db:Status-Status}\t${Version}", package])[0]
        status, version = output.split("\t")
        if status != "installed":
            raise ValueError(f"Required runtime package is not installed: {package}")
        # Dynamic modules lack symbol-level discovery. Conservatively use the
        # exact build-environment release as a minimum, recorded in the receipt.
        result[package] = version
    core = command(["pkg-config", "--modversion", "Qt6Core"])[0].strip()
    minimum = "6.8.0" if metadata["webView"] else "6.6.0"
    try:
        command(["dpkg", "--compare-versions", core, "ge", minimum])
    except ValueError as error:
        raise ValueError(f"deb requires Qt >= {minimum}; found {core}") from error
    metadata["qt"] = core
    if metadata["webView"]:
        metadata["webEngine"] = command(["pkg-config", "--modversion", "Qt6WebEngineQuick"])[0].strip()
    for module, filename in (("libqt6gui6", "libqxcb.so"), ("qt6-wayland", "libqwayland-egl.so")):
        files = command(["dpkg-query", "-L", module])[0].splitlines()
        if not any(path.endswith("/platforms/" + filename) and Path(path).is_file() for path in files):
            raise ValueError("QPA platform plugin is missing: " + filename)
    return result


def inspect(dotnet, project, framework, rid, configuration):
    if configuration != "Release" or framework != "net10.0" or rid != "linux-x64":
        raise ValueError("deb requires Release, net10.0 and linux-x64")
    build_environment = environment()
    project = Path(project).resolve()
    output = command([dotnet, "msbuild", project, "-nologo", "-getProperty:" + ",".join(PROPERTIES),
                      "-getItem:EmbeddedResource", "-p:Configuration=" + configuration,
                      "-p:TargetFramework=" + framework, "-p:RuntimeIdentifier=" + rid])[0]
    metadata = metadata_from_model(json.loads(output), project)
    assets_path = Path(metadata["projectAssetsFile"])
    if not assets_path.is_absolute():
        assets_path = project.parent / assets_path
    if not assets_path.is_file():
        raise ValueError("Restore the Linux runner before deb publish: " + str(assets_path))
    metadata["nugetRoots"] = list(json.loads(assets_path.read_text())["packageFolders"])
    metadata["buildEnvironment"] = build_environment
    metadata["dynamicDependencies"] = runtime_dependencies(metadata)
    return metadata


def desktop(metadata):
    # Keys with free text use desktop-entry string escaping. Exec contains only
    # the validated package ID and a single spec-defined field code.
    def escape(value):
        return value.replace("\\", "\\\\")
    handlers = metadata["mimeTypes"] + ["x-scheme-handler/" + value for value in metadata["uriSchemes"]]
    fields = ["[Desktop Entry]", "Type=Application", "Name=" + escape(metadata["title"]),
              "Comment=" + escape(metadata["description"]),
              "Exec=/usr/bin/" + metadata["packageId"] + (" %U" if handlers else ""),
              "TryExec=/usr/bin/" + metadata["packageId"], "Icon=" + metadata["applicationId"],
              "Terminal=false", "Categories=" + ";".join(metadata["categories"]) + ";"]
    if handlers:
        fields.append("MimeType=" + ";".join(handlers) + ";")
    return "\n".join(fields) + "\n"


def elf(path):
    with path.open("rb") as stream:
        header = stream.read(20)
    if header[:4] != b"\x7fELF":
        return False
    if len(header) < 20 or header[4:6] != b"\x02\x01" or struct.unpack("<H", header[18:20])[0] != 62:
        raise ValueError(f"Expected ELF64 little-endian amd64: {path}")
    return True


def verify_payload(source, metadata):
    executable = source / metadata["executable"]
    required = [executable, source / (metadata["executable"] + ".dll"),
                source / (metadata["executable"] + ".deps.json"), source / (metadata["executable"] + ".runtimeconfig.json"),
                source / "libhostfxr.so", source / "libhostpolicy.so", source / "libcoreclr.so",
                source / "System.Private.CoreLib.dll", source / "libSkiaSharp.so", source / "libdoroti_qt_host.so",
                source / "appicon.png", source / "licenses/doroti-linux/LGPL-3.txt",
                source / "licenses/doroti-linux/GPL-3.txt"]
    if metadata["webView"]:
        required += [source / "libdoroti_webview_qt.so", source / "doroti-webview-runtime.json"]
    if any(not path.is_file() or path.is_symlink() for path in required):
        raise ValueError("Incomplete directory publish: " + ", ".join(str(path) for path in required if not path.is_file() or path.is_symlink()))
    runtime = json.loads((source / (metadata["executable"] + ".runtimeconfig.json")).read_text())["runtimeOptions"]
    if "framework" in runtime or "frameworks" in runtime or not runtime.get("includedFrameworks"):
        raise ValueError("deb requires a self-contained runtimeconfig")
    if not elf(executable):
        raise ValueError("Published apphost is not ELF")
    host_dynamic = command(["readelf", "-d", source / "libdoroti_qt_host.so"])[0]
    if "libQt6Quick.so" not in host_dynamic or (("libdoroti_webview_qt.so" in host_dynamic) != metadata["webView"]):
        raise ValueError("Native Qt host does not match the evaluated Quick/WebView profile")
    if (source / "appicon.png").read_bytes() != Path(metadata["icon"]).read_bytes():
        raise ValueError("Published appicon.png differs from evaluated DorotiLinuxIcon")
    if (source / "libdoroti_texture_gstreamer.so").exists():
        raise ValueError("GStreamer is outside the initial deb profile")
    if not metadata["webView"] and any((source / name).exists() for name in ("libdoroti_webview_qt.so", "doroti-webview-runtime.json")):
        raise ValueError("WebView payload disagrees with evaluated features")
    if metadata["webView"]:
        manifest = json.loads((source / "doroti-webview-runtime.json").read_text())
        if (manifest.get("schemaVersion") != 1 or manifest.get("engineSource") != "system-distribution"
                or set(manifest.get("shims", [])) != {"libdoroti_qt_host.so", "libdoroti_webview_qt.so"}
                or set(manifest.get("qmlModules", [])) != {"QtQuick", "QtQuick.Controls", "QtWebEngine", "QtWebChannel"}
                or manifest.get("abiVersion") != 1 or manifest.get("abiBytes") != 32
                or manifest.get("minimumQtApi") != "6.8.0" or manifest.get("sandbox") != "default-required"
                or manifest.get("builtQt") != metadata["qt"] or manifest.get("builtWebEngine") != metadata["webEngine"]):
            raise ValueError("WebView runtime manifest disagrees with the packaging profile/environment")
        files = "\n".join(command(["dpkg-query", "-L", package])[0] for package in WEB_PACKAGES)
        for name in manifest.get("runtimeFiles", []):
            if not any(path.endswith("/" + name) and Path(path).is_file() for path in files.splitlines()):
                raise ValueError("System WebView runtime file is missing: " + name)
        if not manifest.get("runtimeFiles"):
            raise ValueError("WebView runtime manifest has no runtime closure")
    paths = sorted(source.rglob("*"))
    for path in paths:
        check_stop()
        if path.is_symlink() or not (path.is_file() or path.is_dir()):
            raise ValueError(f"Unsupported symlink or special file in publish payload: {path}")
        line(str(path.relative_to(source)), "payload path")
        if re.match(r"(?:libQt6|QtWebEngineProcess)", path.name):
            raise ValueError("Qt binaries must remain system dependencies: " + str(path))
        if path.is_file():
            elf(path)


def dependencies(stage, payload, metadata, work):
    files = [path for path in sorted(payload.rglob("*")) if path.is_file() and elf(path)]
    needed = set()
    for path in files:
        dynamic = command(["readelf", "-d", path])[0]
        needed.update(re.findall(r"\(NEEDED\).*\[([^\]]+)\]", dynamic))
        for search_path in re.findall(r"\((?:RPATH|RUNPATH)\).*\[([^\]]+)\]", dynamic):
            if any(not re.fullmatch(r"\$(?:ORIGIN|\{ORIGIN\})(?:/[A-Za-z0-9_.+-]+)*", value) for value in search_path.split(":")):
                raise ValueError("Nonportable native library search path in " + str(path))
    # dpkg-shlibdeps warns (rather than failing) for some unversioned missing
    # SONAMEs. Explicitly resolve every NEEDED first, without executing ldd.
    internal = {path.name for path in files}
    for path in files:
        internal.update(re.findall(r"\(SONAME\).*\[([^\]]+)\]", command(["readelf", "-d", path])[0]))
    search = [Path("/lib/x86_64-linux-gnu"), Path("/usr/lib/x86_64-linux-gnu"), Path("/lib64")]
    for name in needed - internal:
        if not any((directory / name).is_file() for directory in search):
            raise ValueError("Unresolved external ELF dependency: " + name +
                             ("; Debian 13 lacks this legacy ABI. Set DorotiDebExcludeLttngProvider=true to explicitly omit the optional .NET LTTng provider" if name == "liblttng-ust.so.0" else ""))
    (work / "debian").mkdir()
    (work / "debian/control").write_text("Source: " + metadata["packageId"] + "\nSection: utils\nPriority: optional\nMaintainer: " + metadata["maintainer"] +
                                          "\n\nPackage: " + metadata["packageId"] + "\nArchitecture: amd64\nDescription: " + metadata["description"] + "\n")
    # The DEBIAN directory marks the package root, allowing dpkg-shlibdeps to
    # recognize private libraries as belonging to the same binary package.
    arguments = ["dpkg-shlibdeps", "-O", "-S" + str(stage)]
    arguments += ["-l" + str(directory) for directory in sorted({path.parent for path in files})]
    arguments += ["-e" + str(path) for path in files]
    output, warnings = command(arguments, cwd=work)
    (work / "shlibdeps.log").write_text(output + warnings)
    if "cannot find library" in warnings:
        raise ValueError("Unresolved ELF dependency: " + warnings)
    generated = next((value.split("=", 1)[1] for value in output.splitlines() if value.startswith("shlibs:Depends=")), "")
    if not generated:
        raise ValueError("dpkg-shlibdeps produced no runtime dependencies")
    dynamic = [package + " (>= " + version + ")" for package, version in sorted(metadata["dynamicDependencies"].items())]
    values = sorted(set(generated.split(", ") + dynamic))
    if any(re.search(r"(?:-dev|-dbg|dotnet-sdk|build-essential|cmake)(?:\s|$)", value) for value in values):
        raise ValueError("Build-only dependency leaked into Depends")
    return ", ".join(values), [str(path.relative_to(payload)) for path in files]


def sha256(path):
    with path.open("rb") as stream:
        return hashlib.file_digest(stream, "sha256").hexdigest()


def write(path, content, mode=0o644):
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(content, encoding="utf-8")
    path.chmod(mode)


def package(source, output, metadata, work):
    verify_payload(source, metadata)
    stage = work / "stage"
    package_id = metadata["packageId"]
    payload = stage / "opt" / package_id
    excluded = ["libcoreclrtraceptprovider.so"] if metadata["excludeLttngProvider"] else []
    shutil.copytree(source, payload, ignore=lambda directory, names: excluded if Path(directory) == source else [])
    for path in [stage, *sorted(stage.rglob("*"))]:
        check_stop()
        # Shared objects/data are 0644; preserve actual executable helpers.
        executable = False
        if path.is_file() and path.stat().st_mode & 0o111 and ".so" not in path.name:
            with path.open("rb") as stream:
                prefix = stream.read(4)
            executable = prefix == b"\x7fELF" or prefix.startswith(b"#!")
        mode = 0o755 if path.is_dir() or executable else 0o644
        path.chmod(mode)
    (payload / metadata["executable"]).chmod(0o755)
    # No XDG overrides: Qt keeps user data under the application ID, owned by
    # the user. No script ever removes those paths, including on purge.
    write(stage / "usr/bin" / package_id, "#!/bin/sh\n"
          "# Qt Wayland must load the runtime SONAME, not the development symlink.\n"
          "export QT_VULKAN_LIB=\"${QT_VULKAN_LIB:-libvulkan.so.1}\"\n"
          "# Debian 13 acceptance currently covers xcb; retain explicit user choices.\n"
          "export QT_QPA_PLATFORM=\"${QT_QPA_PLATFORM:-xcb}\"\n"
          "exec '/opt/" + package_id + "/" + metadata["executable"] + "' \"$@\"\n", 0o755)
    entry = stage / "usr/share/applications" / (metadata["applicationId"] + ".desktop")
    write(entry, desktop(metadata))
    command(["desktop-file-validate", entry])
    size = png_size(source / "appicon.png")
    icon = stage / f"usr/share/icons/hicolor/{size}x{size}/apps" / (metadata["applicationId"] + ".png")
    icon.parent.mkdir(parents=True)
    shutil.copyfile(source / "appicon.png", icon)
    icon.chmod(0o644)
    doc = stage / "usr/share/doc" / package_id
    write(doc / "copyright", Path(metadata["copyright"]).read_text(encoding="utf-8-sig"))
    frameworks = json.loads((source / (metadata["executable"] + ".runtimeconfig.json")).read_text())["runtimeOptions"]["includedFrameworks"]
    metadata["includedFrameworks"] = frameworks
    for framework in frameworks:
        if framework["name"] != "Microsoft.NETCore.App":
            raise ValueError("Additional self-contained frameworks require a reviewed license closure: " + framework["name"])
        for filename in ("LICENSE.TXT", "THIRD-PARTY-NOTICES.TXT"):
            candidates = [Path(directory) / "microsoft.netcore.app.runtime.linux-x64" / framework["version"] / filename for directory in metadata["nugetRoots"]]
            license_file = next((path for path in candidates if path.is_file()), None)
            if not license_file:
                raise ValueError("Missing self-contained .NET runtime notice: " + filename)
            write(doc / "dotnet" / filename, license_file.read_text(encoding="utf-8-sig"))
    write(doc / "README.Debian", "Doroti Qt application; self-contained untrimmed .NET JIT.\n"
          "Qt is supplied by the system. Vulkan 1.2 and a working GPU driver/display are required.\n"
          "User XDG data is retained on upgrade, remove and purge.\n"
          "A local .deb does not configure an APT repository or automatic updates.\n"
          "Additional application and third-party licenses are under /opt/" + package_id + "/licenses/.\n")
    timestamp = datetime.fromtimestamp(int(os.environ["SOURCE_DATE_EPOCH"]), timezone.utc) if "SOURCE_DATE_EPOCH" in os.environ else datetime.now(timezone.utc)
    changelog = (package_id + " (" + metadata["debianVersion"] + ") unstable; urgency=medium\n\n"
                 "  * Package the evaluated Doroti Qt application and runtime notices.\n\n"
                 " -- " + metadata["maintainer"] + "  " + format_datetime(timestamp) + "\n")
    changelog_path = doc / "changelog.Debian.gz"
    changelog_path.write_bytes(gzip.compress(changelog.encode("utf-8"), mtime=0))
    changelog_path.chmod(0o644)
    # This is a third-party /opt application bundle, not a Debian archive source
    # package. Retain exact upstream native bytes for Skia provenance and the
    # publish hash manifest; do not strip/relink vendored runtime libraries.
    overrides = ["# Application-private bundle; dpkg owns every installed file.",
                 package_id + ": dir-or-file-in-opt [opt/" + package_id + "/]",
                 package_id + ": dir-or-file-in-opt [opt/" + package_id + "/*]",
                 "# Preserve upstream Skia/.NET bundled codec/compression code and license notices.",
                 package_id + ": embedded-library freetype [opt/" + package_id + "/libSkiaSharp.so]",
                 package_id + ": embedded-library libjpeg [opt/" + package_id + "/libSkiaSharp.so]",
                 package_id + ": embedded-library zlib [opt/" + package_id + "/libSystem.IO.Compression.Native.so]",
                 "# Preserve native provenance/hash identity and diagnostic symbols."]
    overrides += [package_id + ": unstripped-binary-or-object [opt/" + package_id + "/" + name + "]" for name in
                  ("libSkiaSharp.so", "libdoroti_qt_host.so", "libdoroti_webview_qt.so") if (payload / name).is_file()]
    write(stage / "usr/share/lintian/overrides" / package_id, "\n".join(overrides) + "\n")
    control = stage / "DEBIAN/control"
    write(control, "Package: " + package_id + "\nVersion: " + metadata["debianVersion"] + "\nArchitecture: amd64\nMaintainer: " + metadata["maintainer"] + "\nDescription: " + metadata["description"] + "\n Self-contained Doroti Qt desktop application using system Qt runtimes.\n")
    depends, elf_files = dependencies(stage, payload, metadata, work)
    installed_size = (sum(path.stat().st_size for path in stage.rglob("*") if path.is_file() and "DEBIAN" not in path.parts) + 1023) // 1024
    control_text = control.read_text() + "Section: utils\nPriority: optional\nInstalled-Size: " + str(installed_size) + "\nDepends: " + depends + "\n"
    if metadata["homepage"]:
        control_text += "Homepage: " + metadata["homepage"] + "\n"
    write(control, control_text)
    # Normalize all generated parent directories too (TemporaryDirectory is 0700).
    for path in stage.rglob("*"):
        if path.is_dir():
            path.chmod(0o755)
    basename = package_id + "_" + metadata["debianVersion"] + "_amd64"
    artifacts = work / "artifacts"
    artifacts.mkdir()
    deb = artifacts / (basename + ".deb")
    command(["dpkg-deb", "--root-owner-group", "-Zxz", "-z1", "--build", stage, deb])
    command(["dpkg-deb", "--info", deb])
    contents = command(["dpkg-deb", "--contents", deb])[0]
    write(artifacts / "contents.txt", contents)
    # Retain warnings, but any lintian error makes the package fail.
    lint, lint_error = command(["lintian", "--allow-root", "--tag-display-limit", "0", str(deb)])
    write(artifacts / "lintian.txt", lint + lint_error)
    if any(value.startswith("E:") for value in lint.splitlines()):
        raise ValueError("lintian rejected the package:\n" + lint + lint_error)
    digest = sha256(deb)
    write(artifacts / (deb.name + ".sha256"), digest + "  " + deb.name + "\n")
    shutil.copyfile(work / "shlibdeps.log", artifacts / "shlibdeps.log")
    receipt = dict(schemaVersion="doroti.linux-deb/v1", package=deb.name, sha256=digest,
                   metadata=metadata, depends=depends, elfFiles=elf_files,
                   defaultQpa="xcb",
                   lintianOverrides=overrides,
                   excludedRuntimeFiles=excluded,
                   publishPayload={str(path.relative_to(source)): sha256(path) for path in sorted(source.rglob("*")) if path.is_file()},
                   payload={str(path.relative_to(payload)): sha256(path) for path in sorted(payload.rglob("*")) if path.is_file()},
                   verification=dict(package="passed", desktop="passed", lintian="no-errors",
                                     install="not-run", gui="not-run", upgrade="not-run", remove="not-run", purge="not-run"))
    write(artifacts / "receipt.json", json.dumps(receipt, indent=2, ensure_ascii=False) + "\n")
    destination = output / basename
    if destination.exists():
        raise ValueError("Artifact version already exists; use a new DorotiDebVersion/revision or a different output: " + str(destination))
    check_stop()
    artifacts.chmod(0o755)
    # The package, hash, receipt and reports become visible together.
    artifacts.rename(destination)
    return destination / deb.name


def build(arguments):
    metadata = json.loads(arguments.metadata)
    output = Path(arguments.output).resolve()
    output.mkdir(parents=True, exist_ok=True)
    if (output / (metadata["packageId"] + "_" + metadata["debianVersion"] + "_amd64")).exists():
        raise ValueError("Artifact version already exists; use a new version/revision or a different output")
    with tempfile.TemporaryDirectory(prefix=".doroti-deb-", dir=output) as temporary:
        work = Path(temporary)
        source = work / "publish"
        # Re-evaluate to catch edits/environment changes between plan and execution.
        current = inspect(arguments.dotnet, metadata["project"], metadata["framework"], metadata["rid"], "Release")
        if current != metadata:
            raise ValueError("Packaging inputs changed after planning; retry publish")
        publish = json.loads(arguments.publish_arguments)
        if not publish or publish[0] != "publish":
            raise ValueError("Expected provider dotnet publish arguments")
        print("deb: publishing self-contained directory", flush=True)
        command([arguments.dotnet, *publish, *PUBLISH_PROPERTIES, "--output", source], echo=True)
        # Restore can replace stale package-folder paths from another machine
        # or container. Read the actual resolved runtime notice locations while
        # still rejecting changes to application/profile/environment inputs.
        resolved = inspect(arguments.dotnet, metadata["project"], metadata["framework"], metadata["rid"], "Release")
        if {key: value for key, value in resolved.items() if key != "nugetRoots"} != {key: value for key, value in metadata.items() if key != "nugetRoots"}:
            raise ValueError("Packaging inputs changed during publish; retry publish")
        metadata = resolved
        result = package(source, output, metadata, work)
        print("deb: " + str(result), flush=True)


def main():
    global STOP_PATH, SESSION_ID
    parser = argparse.ArgumentParser(description=__doc__)
    sub = parser.add_subparsers(dest="command", required=True)
    probe = sub.add_parser("inspect")
    for name in ("dotnet", "project", "framework", "rid", "configuration"):
        probe.add_argument("--" + name, required=True)
    builder = sub.add_parser("build")
    for name in ("dotnet", "publish-arguments", "metadata", "output", "stop-signal", "session-id"):
        builder.add_argument("--" + name, required=True)
    arguments = parser.parse_args()
    def interrupted(*_):
        raise InterruptedError("deb packaging canceled")
    signal.signal(signal.SIGTERM, interrupted)
    signal.signal(signal.SIGINT, interrupted)
    try:
        if arguments.command == "inspect":
            print(json.dumps(inspect(arguments.dotnet, arguments.project, arguments.framework, arguments.rid, arguments.configuration), ensure_ascii=False))
        else:
            STOP_PATH, SESSION_ID = Path(arguments.stop_signal), arguments.session_id
            build(arguments)
        return 0
    except InterruptedError as error:
        print(str(error), file=sys.stderr)
        return 130
    except (ValueError, OSError, KeyError) as error:
        print("deb: " + str(error), file=sys.stderr)
        return 1
    finally:
        if STOP_PATH:
            STOP_PATH.unlink(missing_ok=True)


if __name__ == "__main__":
    sys.exit(main())
