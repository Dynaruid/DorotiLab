"""Qt bundle contracts using real ELF fixtures; no Qt SDK/network/host installs."""
import argparse
import copy
import importlib.util
import json
import lzma
import os
from pathlib import Path
import shutil
import subprocess
import sys
import tempfile
import unittest

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT / "packages/platforms/build"))
import qt_runtime as qt
LOADER = None
PACKAGE_TESTS = False
ARTIFACTS = None


class RuntimeContracts(unittest.TestCase):
    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory(prefix="Qt 런타임 ")
        self.addCleanup(self.temporary.cleanup)
        self.root = Path(self.temporary.name)
        self.sdk = self.root / "sdk"
        self.native = self.root / "native"
        self.publish = self.root / "publish"
        for path in (self.sdk, self.native, self.publish):
            path.mkdir()
        if LOADER:
            for path in LOADER.glob("RuntimeLoader.*"):
                shutil.copyfile(path, self.publish / path.name)
        self.config = qt.settings({"DorotiQtSdkDirectory": str(self.sdk)})
        self.library(self.sdk / "lib/libQt6Core.so.6.12.0", "libQt6Core.so.6")
        for name in ("plugins/platforms/libqxcb.so", "plugins/platforms/libqwayland-egl.so"):
            self.library(self.sdk / name, Path(name).name)
        self.data = self.inventory(self.sdk, "framework", "6.12.0")
        self.native_build(False)

    def library(self, path, soname, needed=None):
        path.parent.mkdir(parents=True, exist_ok=True)
        source = self.root / "fixture.c"
        source.write_text("int fixture(void) { return 42; }\n")
        subprocess.run(["cc", "-shared", "-fPIC", str(source), "-Wl,-soname," + soname, "-Wl,--no-as-needed", "-lc", "-o", str(path)], check=True)
        if needed:
            subprocess.run(["patchelf", "--add-needed", needed, str(path)], check=True)

    def inventory(self, prefix, kind, version):
        if kind == "framework":
            names = ("QtQuick", "QtQml", "QtQml/Models", "QtQml/WorkerScript", "QtQuick/Window", "QtQuick/Controls", "QtQuick/Templates")
            for name in names:
                name = "qml/" + name + "/qmldir"
                path = prefix / name
                path.parent.mkdir(parents=True, exist_ok=True)
                path.write_text("module QtQuick\nimport QtQml auto\n" if name == "qml/QtQuick/qmldir" else "module Fixture\n")
        else:
            for name in ("qml/QtWebEngine/qmldir", "resources/qtwebengine_resources.pak", "resources/qtwebengine_resources_100p.pak",
                         "resources/qtwebengine_resources_200p.pak", "resources/icudtl.dat", "resources/v8_context_snapshot.bin",
                         "translations/qtwebengine_locales/en-US.pak", "libexec/QtWebEngineProcess"):
                path = prefix / name
                path.parent.mkdir(parents=True, exist_ok=True)
                if name == "libexec/QtWebEngineProcess":
                    main = self.root / "helper.c"
                    main.write_text("int main(void) { return 0; }\n")
                    subprocess.run(["cc", str(main), "-o", str(path)], check=True)
                else:
                    path.write_text("module QtWebEngine\n" if name.endswith("qmldir") else "fixture\n")
        data = dict(schemaVersion="doroti.qt-sdk/v1", version=version, frameworkVersion="6.12.0", architecture="linux-x64", kind=kind,
                    review="TEST FIXTURE ONLY; not a license approval", buildConfiguration=dict(ffmpegLicense="LGPL"),
                    archives=[dict(url="https://example.org/exact-binary.7z", sha256="a" * 64)], sources=[], documents=[], files=[])
        for role in ("LGPL-3.0", "GPL-3.0", "copyright", "third-party", "replacement", "build-instructions", "sbom"):
            name = "notices/" + role + ".txt"
            path = prefix / name
            path.parent.mkdir(exist_ok=True)
            path.write_text("TEST FIXTURE " + role)
            data["documents"].append(dict(path=name, role=role, sha256=qt.digest(path)))
        source = prefix / "notices/source.tar.xz"
        source.write_bytes(lzma.compress(b"test source fixture"))
        data["sources"] = [dict(path="notices/source.tar.xz", sha256=qt.digest(source), binaryArchives=["a" * 64],
                                patches="none (fixture)", buildInstructions="notices/build-instructions.txt")]
        for path in prefix.rglob("*"):
            if path.is_file() and not path.is_relative_to(prefix / "notices"):
                name = str(path.relative_to(prefix))
                module = "Core" if kind == "framework" else "WebEngineCore"
                data["files"].append(dict(path=name, module=module, license="LGPL-3.0-only", sha256=qt.digest(path)))
        qt.write_json(prefix / "doroti-sdk.json", data)
        return data

    def native_build(self, web):
        self.library(self.native / "libdoroti_qt_host.so", "libdoroti_qt_host.so", "libQt6Core.so.6")
        names = ["libdoroti_qt_host.so"]
        if web:
            self.library(self.native / "libdoroti_webview_qt.so", "libdoroti_webview_qt.so", "libQt6WebEngineCore.so.6")
            names.append("libdoroti_webview_qt.so")
        build = dict(schemaVersion="doroti.qt-build/v1", qt="6.12.0", architecture="linux-x64", hostAbi=7, webViewAbi=1, quick=True, graphite=True,
                     webView=web, webEngine="6.140.0", shims={name: qt.digest(self.native / name) for name in names})
        qt.write_json(self.native / "doroti-qt-build.json", build)
        for name in names:
            shutil.copyfile(self.native / name, self.publish / name)

    def engine(self, deployment):
        prefix = self.root / "engine-sdk"
        prefix.mkdir()
        self.library(prefix / "lib/libQt6WebEngineCore.so.6.140.0", "libQt6WebEngineCore.so.6", "libQt6Core.so.6")
        self.inventory(prefix, "webengine", "6.140.0")
        self.config.update(webView=True, webEngine="6.140.0", engineSdk=str(prefix), deployment=deployment,
                           packageId="fixture-engine", packageVersion="6.140.0-1")
        self.native_build(True)
        return prefix

    def test_bundle_real_elf_closure_and_relocation(self):
        qt.bundle(self.publish, self.native, self.config)
        data = qt.verify(self.publish, "6.12.0", False)
        root = self.publish / data["framework"]["path"]
        self.assertTrue((root / "lib/libQt6Core.so.6").is_file())
        rpath = qt.run("patchelf", "--print-rpath", root / "plugins/platforms/libqxcb.so")
        self.assertIn("$ORIGIN/../../lib", rpath)
        moved = self.root / "한글 relocated"
        self.publish.rename(moved)
        qt.verify(moved)

    def test_bundle_without_sdk_manifest_discovers_runtime_and_excludes_build_assets(self):
        (self.sdk / "doroti-sdk.json").unlink()
        shutil.rmtree(self.sdk / "notices")
        self.library(self.sdk / "lib/libQt6Graphs.so.6.12.0", "libQt6Graphs.so.6")
        (self.sdk / "lib/libQt6Core.so.6").symlink_to("libQt6Core.so.6.12.0")
        (self.sdk / "lib/development.a").write_bytes(b"SDK build asset")
        (self.sdk / "qml/QtQuick/development.o").write_bytes(b"QML build asset")
        (self.sdk / "qml/QtQuick/Window.qml").write_text("import QtQuick\nItem {}\n")
        # SDK parent directory names must not be interpreted as QML modules.
        relocated = self.root / "tooling/sdk"
        relocated.parent.mkdir()
        self.sdk.rename(relocated)
        self.sdk = relocated
        self.config["sdk"] = str(self.sdk)
        data = qt.bundle(self.publish, self.native, self.config)
        part = data["framework"]
        self.assertEqual(part["inventory"], "discovered")
        self.assertIsNone(part["review"])
        self.assertEqual(part["sources"], [])
        names = {entry["path"] for entry in part["files"]}
        self.assertIn("lib/libQt6Core.so.6", names)
        self.assertIn("qml/QtQuick/Window.qml", names)
        self.assertFalse(any("Graphs" in name or name.endswith((".a", ".o")) for name in names))
        self.assertFalse((self.sdk / "doroti-sdk.json").exists())
        qt.verify(self.publish)

    def test_manifestless_bundled_engine_preserves_candidate_label(self):
        engine = self.engine("bundled")
        for prefix in (self.sdk, engine):
            (prefix / "doroti-sdk.json").unlink()
        files = {path.relative_to(engine).as_posix(): qt.digest(path) for path in engine.rglob("*") if path.is_file()}
        qt.write_json(engine / "doroti-sdk-install.json", dict(files=files, lock=dict(
            kind="webengine", version="6.140.0", frameworkVersion="6.12.0", architecture="linux-x64",
            releaseName="6.140.0-rc", archives=[])))
        data = qt.bundle(self.publish, self.native, self.config)
        self.assertEqual(data["webEngine"]["releaseName"], "6.140.0-rc")
        self.assertIsNone(data["webEngine"]["review"])
        qt.verify(self.publish)

    def test_manifestless_separate_engine_is_verified(self):
        engine = self.engine("download")
        for prefix in (self.sdk, engine):
            (prefix / "doroti-sdk.json").unlink()
        output = self.root / "engine-output"
        data = qt.bundle(self.publish, self.native, self.config, output)
        qt.verify_part(output, data["webEngine"])
        self.assertEqual(data["webEngine"]["packageVersion"], "6.140.0-1")
        qt.verify(self.publish)

    def test_manifestless_sdk_still_checks_installed_hashes_and_version(self):
        (self.sdk / "doroti-sdk.json").unlink()
        lock = dict(kind="framework", version="6.12.0", frameworkVersion="6.12.0", architecture="linux-x64")
        files = {path.relative_to(self.sdk).as_posix(): qt.digest(path) for path in self.sdk.rglob("*") if path.is_file()}
        qt.write_json(self.sdk / "doroti-sdk-install.json", dict(lock=lock, files=files))
        qt.sdk_inventory(self.sdk, "6.12.0", "6.12.0", "framework")
        with self.assertRaisesRegex(ValueError, "install lock"):
            qt.sdk_inventory(self.sdk, "6.12.0", "6.11.0", "framework")
        (self.sdk / "qml/QtQuick/qmldir").write_text("changed")
        with self.assertRaisesRegex(ValueError, "changed installed"):
            qt.sdk_inventory(self.sdk, "6.12.0", "6.12.0", "framework")

    def test_manifestless_sdk_rejects_escaping_file_links(self):
        (self.sdk / "doroti-sdk.json").unlink()
        (self.root / "outside.qml").write_text("Item {}")
        (self.sdk / "qml/QtQuick/outside.qml").symlink_to(self.root / "outside.qml")
        with self.assertRaisesRegex(ValueError, "leaves its prefix"):
            qt.sdk_inventory(self.sdk, "6.12.0", "6.12.0", "framework")

    def test_republish_replaces_previous_owned_runtime(self):
        qt.bundle(self.publish, self.native, self.config)
        shutil.copyfile(self.native / "libdoroti_qt_host.so", self.publish / "libdoroti_qt_host.so")
        qt.bundle(self.publish, self.native, self.config)
        qt.verify(self.publish)

    def test_bundled_independent_engine_version(self):
        self.engine("bundled")
        data = qt.bundle(self.publish, self.native, self.config)
        self.assertEqual(data["webEngine"]["version"], "6.140.0")
        qt.verify(self.publish, "6.12.0", True)

    def test_separate_engine_has_exact_package_contract(self):
        self.engine("download")
        engine = self.root / "engine-output"
        data = qt.bundle(self.publish, self.native, self.config, engine)
        self.assertFalse((self.publish / "qt/webengine/6.140.0").exists())
        installed = json.loads((engine / "engine.json").read_text())
        self.assertEqual(installed["packageVersion"], "6.140.0-1")
        self.assertEqual(data["webEngine"]["path"], "/opt/doroti/qt-webengine/fixture-engine/6.140.0-1")
        qt.verify(self.publish)

    def test_download_combinations(self):
        base = dict(DorotiQtSdkDirectory=str(self.sdk), DorotiQtWebEngineDeployment="download",
                    DorotiQtWebEnginePackageId="fixture-engine", DorotiQtWebEnginePackageVersion="6.140.0-1")
        for changes, fmt in (({}, "deb"), ({"DorotiQtWebEngine": "true"}, "directory"),
                             ({"DorotiQtWebEngine": "true", "DorotiQtWebEngineSdkDirectory": "engine", "DorotiQtWebEnginePackageId": "bad\nDepends: evil"}, "deb")):
            with self.subTest(changes=changes), self.assertRaises(ValueError):
                qt.settings(base | changes, fmt)
        accepted = qt.settings(base | dict(DorotiQtWebEngine="true", DorotiQtWebEngineSdkDirectory="engine"), "deb")
        self.assertEqual(accepted["webEngine"], "6.140.0")

    def test_no_engine_sdk_needed_when_disabled(self):
        self.assertFalse(self.config["webView"])
        qt.bundle(self.publish, self.native, self.config)
        self.assertNotIn("webEngine", qt.verify(self.publish))

    def test_unsupported_framework_and_prebuilt_versions(self):
        with self.assertRaisesRegex(ValueError, "Only Qt"):
            qt.settings(dict(DorotiQtVersion="6.8.2", DorotiQtSdkDirectory=str(self.sdk)))
        build_path = self.native / "doroti-qt-build.json"
        build = json.loads(build_path.read_text())
        build["qt"] = "6.8.2"
        qt.write_json(build_path, build)
        with self.assertRaisesRegex(ValueError, "manifest"):
            qt.bundle(self.publish, self.native, self.config)

    def test_prebuilt_bytes_and_profile_are_checked(self):
        with (self.native / "libdoroti_qt_host.so").open("ab") as stream:
            stream.write(b"changed")
        with self.assertRaisesRegex(ValueError, "differs"):
            qt.bundle(self.publish, self.native, self.config)

    def test_unknown_module_gpl_and_missing_source_fail_closed(self):
        for change in (lambda data: data["files"][0].update(module="Graphs"),
                       lambda data: data["files"][0].update(license="GPL-3.0-only"),
                       lambda data: data.update(sources=[]),
                       lambda data: data.update(documents=[]),
                       lambda data: data["sources"][0].update(binaryArchives=["b" * 64])):
            data = copy.deepcopy(self.data)
            change(data)
            qt.write_json(self.sdk / "doroti-sdk.json", data)
            with self.assertRaises(ValueError):
                qt.sdk_inventory(self.sdk, "6.12.0", "6.12.0", "framework")

    def test_transitive_qt_dependency_cannot_fall_back_to_system(self):
        self.data["files"][0]["sha256"] = "0" * 64
        part = dict(files=[dict(path="lib/libQt6Core.so.6", soname="libQt6Core.so.6", needed=["libQt6Graphs.so.6"])])
        with self.assertRaisesRegex(ValueError, "transitive"):
            qt.validate_closure([part], [])

    def test_supplier_candidate_label_and_archive_identity_are_preserved(self):
        qt.write_json(self.sdk / "doroti-sdk-install.json", dict(lock=dict(releaseName="6.12.0-rc", archives=self.data["archives"])))
        with self.assertRaisesRegex(ValueError, "release name"):
            qt.sdk_inventory(self.sdk, "6.12.0", "6.12.0", "framework")
        self.data["releaseName"] = "6.12.0-rc"
        qt.write_json(self.sdk / "doroti-sdk.json", self.data)
        qt.sdk_inventory(self.sdk, "6.12.0", "6.12.0", "framework")
        self.data["archives"][0]["url"] = "https://example.org/different-binary.7z"
        qt.write_json(self.sdk / "doroti-sdk.json", self.data)
        with self.assertRaisesRegex(ValueError, "archive lock"):
            qt.sdk_inventory(self.sdk, "6.12.0", "6.12.0", "framework")

    def test_runtime_files_cannot_escape_inventory_prefix(self):
        for name in ("../outside", "/etc/passwd", "qml/../../outside", "qml\\evil", "x\nEvil"):
            with self.subTest(name=name), self.assertRaises(ValueError):
                qt.relative(name)

    def test_changed_runtime_fails_packaging_verification(self):
        qt.bundle(self.publish, self.native, self.config)
        target = self.publish / "qt/framework/6.12.0/plugins/platforms/libqxcb.so"
        target.write_bytes(b"changed")
        with self.assertRaisesRegex(ValueError, "changed"):
            qt.verify(self.publish)

    def loader(self, **environment):
        env = {key: value for key, value in os.environ.items() if not key.startswith(("QT_", "QTWEBENGINE", "QML", "DOROTI_QT_"))}
        env.update(environment)
        return subprocess.run(["dotnet", str(self.publish / "RuntimeLoader.dll")], env=env, capture_output=True, text=True, timeout=30)

    def test_loader_selects_private_runtime_preserves_qpa_and_allows_modified_lgpl(self):
        qt.bundle(self.publish, self.native, self.config)
        first = self.loader(WAYLAND_DISPLAY="fixture-wayland")
        self.assertEqual(first.returncode, 0, first.stderr)
        self.assertEqual(json.loads(first.stdout), dict(value=42, qpa="wayland"))
        replacement = self.root / "modified-framework"
        shutil.copytree(self.publish / "qt/framework/6.12.0", replacement)
        source = self.root / "modified.c"
        source.write_text("int fixture(void) { return 73; }\n")
        subprocess.run(["cc", "-shared", "-fPIC", str(source), "-Wl,-soname,libQt6Core.so.6", "-o", str(replacement / "lib/libQt6Core.so.6")], check=True)
        # Loader uses the real versioned path chosen by the manifest.
        shutil.copyfile(replacement / "lib/libQt6Core.so.6", replacement / "lib/libQt6Core.so.6.12.0")
        modified = self.loader(DOROTI_QT_FRAMEWORK_PATH=str(replacement), QT_QPA_PLATFORM="xcb")
        self.assertEqual(modified.returncode, 0, modified.stderr)
        self.assertEqual(json.loads(modified.stdout), dict(value=73, qpa="xcb"))

    def test_loader_missing_private_library_fails_before_system_fallback(self):
        qt.bundle(self.publish, self.native, self.config)
        (self.publish / "qt/framework/6.12.0/lib/libQt6Core.so.6.12.0").unlink()
        result = self.loader()
        self.assertNotEqual(result.returncode, 0)
        self.assertIn("Required Qt runtime file is missing", result.stderr)

    def test_loader_missing_bundle_manifest_does_not_use_system_qt(self):
        qt.bundle(self.publish, self.native, self.config)
        (self.publish / "doroti-qt-runtime.json").unlink()
        result = self.loader()
        self.assertNotEqual(result.returncode, 0)
        self.assertIn("private Qt runtime manifest is missing", result.stderr)

    def test_loader_missing_separate_engine_requires_installation(self):
        self.engine("download")
        qt.bundle(self.publish, self.native, self.config, self.root / "separate-engine")
        result = self.loader(DOROTI_QT_WEBENGINE_PATH=str(self.root / "missing-engine"))
        self.assertNotEqual(result.returncode, 0)
        self.assertIn("Qt runtime prefix is missing", result.stderr)

    def test_locked_sdk_install_reuse_and_changed_content(self):
        spec = importlib.util.spec_from_file_location("qt_installer", ROOT / "packages/platforms/build/install-qt-sdk.py")
        installer = importlib.util.module_from_spec(spec)
        spec.loader.exec_module(installer)
        cache = self.root / "cache"
        cache.mkdir()
        source = self.root / "source-archive"
        source.write_bytes(b"fixture locked archive")
        checksum = qt.digest(source)
        shutil.copyfile(source, cache / checksum)
        lock = self.root / "sdk-lock.json"
        qt.write_json(lock, dict(schemaVersion="doroti.qt-sdk-lock/v1", architecture="linux-x64", payloadPrefix=".",
                      archives=[dict(url="https://example.org/source.tar.xz", sha256=checksum, format="file", destination="sources/qt.tar.xz")]))
        output = self.root / "installed-sdk"
        installer.install(lock, output, cache)
        installer.install(lock, output, cache)
        (output / "sources/qt.tar.xz").write_bytes(b"changed")
        with self.assertRaisesRegex(ValueError, "different/changed"):
            installer.install(lock, output, cache)

    def test_real_separate_engine_deb_has_only_os_dependencies_and_dpkg_ownership(self):
        if not PACKAGE_TESTS:
            self.skipTest("Enable --package-tests inside the Debian packaging environment")
        spec = importlib.util.spec_from_file_location("qt_deb", ROOT / "packages/platforms/qt/tooling/linux_deb.py")
        deb = importlib.util.module_from_spec(spec)
        spec.loader.exec_module(deb)
        self.engine("download")
        engine_output = self.root / "engine-output"
        runtime = qt.bundle(self.publish, self.native, self.config, engine_output)
        stage, app_stage = self.root / "engine-stage", self.root / "app-stage"
        prefix = stage / runtime["webEngine"]["path"].lstrip("/")
        shutil.copytree(engine_output, prefix)
        app_payload = app_stage / "opt/fixture-app"
        shutil.copytree(self.publish, app_payload)
        deb.write(app_stage / "DEBIAN/control", "Package: fixture-app\nArchitecture: amd64\n")
        deb.write(stage / "DEBIAN/control", "Package: fixture-engine\nArchitecture: amd64\n")
        work, artifacts = self.root / "package-work", self.root / "artifacts"
        work.mkdir()
        artifacts.mkdir()
        metadata = dict(packageId="fixture-app", maintainer="Fixture <fixture@example.org>", description="Qt runtime packaging fixture")
        result = deb.engine_package(stage, prefix, runtime["webEngine"], metadata, app_stage, app_payload, work, artifacts)
        self.assertNotIn("libqt6", result["depends"])
        self.assertIn("libc6", result["depends"])
        control = qt.run("dpkg-deb", "-f", artifacts / result["package"])
        self.assertIn("Version: 6.140.0-1", control)
        self.assertIn("X-Doroti-Qt-Framework-Version: 6.12.0", control)
        self.assertNotIn("Pre-Depends:", control)
        contents = qt.run("dpkg-deb", "-c", artifacts / result["package"])
        self.assertTrue(all("root/root" in line for line in contents.splitlines()))
        ARTIFACTS.mkdir(parents=True, exist_ok=True)
        for path in artifacts.iterdir():
            shutil.copyfile(path, ARTIFACTS / path.name)
        qt.write_json(ARTIFACTS / "fixture-engine.json", result | dict(scope="ELF fixture only; not a Qt release", install="not-run", gui="not-run"))


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--package-tests", action="store_true")
    args = parser.parse_args()
    PACKAGE_TESTS = args.package_tests
    ARTIFACTS = args.output.resolve() / "fixture-artifacts"
    args.output.mkdir(parents=True, exist_ok=True)
    LOADER = args.output.resolve() / "loader"
    subprocess.run(["dotnet", "build", str(ROOT / "Doroti/tests/fixtures/qt-runtime/RuntimeLoader.csproj"),
                    "-o", str(LOADER), "--nologo"], check=True)
    result = unittest.TextTestRunner(verbosity=2).run(unittest.defaultTestLoader.loadTestsFromTestCase(RuntimeContracts))
    qt.write_json(args.output / "contracts.json", dict(status="passed" if result.wasSuccessful() else "failed",
                  tests=result.testsRun, install="not-run", gui="not-run", scope="real ELF fixtures, no runtime qualification"))
    raise SystemExit(0 if result.wasSuccessful() else 1)
