"""Debian packaging contracts; optional inspection of a real generated artifact.

These checks never install packages. Installation/GUI evidence is separate.
"""
import argparse
import copy
import importlib.util
import json
from pathlib import Path
import struct
import subprocess
import sys
import tempfile
import threading
import time
from types import SimpleNamespace
import unittest

ROOT = Path(__file__).resolve().parents[2]
spec = importlib.util.spec_from_file_location("linux_deb", ROOT / "packages/platforms/qt/tooling/linux_deb.py")
deb = importlib.util.module_from_spec(spec)
spec.loader.exec_module(deb)


class PackagingContracts(unittest.TestCase):
    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory(prefix="한글 package ")
        self.addCleanup(self.temporary.cleanup)
        self.root = Path(self.temporary.name)
        self.icon = self.root / "icon.png"
        self.icon.write_bytes(b"\x89PNG\r\n\x1a\n" + struct.pack(">I", 13) + b"IHDR" + struct.pack(">II", 128, 128))
        (self.root / "COPYRIGHT").write_text("Copyright fixture\nLicense: BSD-3-Clause\n")
        manifest = self.root / "application-manifest.json"
        manifest.write_text(json.dumps(dict(applicationId="org.example.fixture", targetRid="linux-x64")))
        self.model = dict(Properties={name: "" for name in deb.PROPERTIES}, Items=dict(EmbeddedResource=[dict(FullPath=str(manifest), LogicalName="Doroti.Application.Manifest")]))
        self.model["Properties"].update(ApplicationId="org.example.fixture", ApplicationTitle="한글 fixture", AssemblyName="한글 app",
            Version="1.2.0-beta.1", TargetFramework="net10.0", RuntimeIdentifier="linux-x64", DorotiTarget="Linux", DorotiHostKind="Qt",
            DorotiQtQuick="true", DorotiQtGraphite="true", DorotiQtWebEngine="false", DorotiLinuxIcon=str(self.icon),
            DorotiDebPackageId="example-fixture", DorotiDebMaintainer="Example <test@example.org>", DorotiDebDescription="Packaging fixture",
            DorotiDebCopyright="COPYRIGHT")

    def metadata(self):
        return deb.metadata_from_model(self.model, self.root / "fixture.csproj")

    def test_versions(self):
        self.assertEqual(deb.debian_version("1.2.0-beta.1"), "1.2.0~beta.1-1")
        self.assertEqual(deb.debian_version("1.2.0", "2"), "1.2.0-2")
        for bad in ("1.2", "01.2.0", "1.2.0+sha.1", "1.2.0-beta..1", "1.2.0-01", "1.2.0\nDepends: evil"):
            with self.subTest(bad=bad), self.assertRaises(ValueError):
                deb.debian_version(bad)
        self.assertEqual(subprocess.run(["dpkg", "--compare-versions", deb.debian_version("1.2.0-beta.1"), "lt", deb.debian_version("1.2.0")]).returncode, 0)

    def test_evaluated_metadata_and_identity(self):
        metadata = self.metadata()
        self.assertEqual(metadata["packageId"], "example-fixture")
        self.assertEqual(metadata["applicationId"], "org.example.fixture")
        self.assertEqual(metadata["copyright"], str(self.root / "COPYRIGHT"))
        self.model["Properties"]["ApplicationId"] = "org.example.other"
        with self.assertRaisesRegex(ValueError, "manifest"):
            self.metadata()

    def test_invalid_combinations_fail_before_publish(self):
        original = copy.deepcopy(self.model)
        for name, value in (("RuntimeIdentifier", "linux-arm64"), ("PublishAot", "true"), ("PublishTrimmed", "true"),
                            ("PublishSingleFile", "true"), ("PublishReadyToRun", "true"), ("DorotiGStreamerTextures", "true"),
                            ("DorotiQtQuick", "false"), ("DorotiQtGraphite", "false"), ("DorotiQtWebEngine", "yes"), ("UseAppHost", "false")):
            self.model = copy.deepcopy(original)
            self.model["Properties"][name] = value
            with self.subTest(name=name), self.assertRaises(ValueError):
                self.metadata()

    def test_no_metadata_injection_or_automatic_package_id(self):
        original = copy.deepcopy(self.model)
        for name, value in (("DorotiDebPackageId", ""), ("DorotiDebPackageId", "Example_Fixture"),
                            ("DorotiDebMaintainer", "Bad\nDepends: evil"), ("ApplicationTitle", "x\nExec=evil"),
                            ("DorotiDebDescription", "x\r\nDepends: evil"), ("AssemblyName", "../evil"),
                            ("AssemblyName", "app'$(echo bad)"), ("DorotiDebHomepage", "javascript:alert(1)"),
                            ("DorotiDebMimeTypes", "text/plain;;text/html"), ("DorotiDebUriSchemes", "https;https")):
            self.model = copy.deepcopy(original)
            self.model["Properties"][name] = value
            with self.subTest(name=name, value=value), self.assertRaises(ValueError):
                self.metadata()

    def test_desktop_handlers_are_explicit(self):
        self.assertNotIn("MimeType=", deb.desktop(self.metadata()))
        self.model["Properties"].update(DorotiDebUriSchemes="example", DorotiDebMimeTypes="text/plain", ApplicationTitle="한글 \\ title")
        entry = deb.desktop(self.metadata())
        self.assertIn("Name=한글 \\\\ title", entry)
        self.assertIn("Exec=/usr/bin/example-fixture %U", entry)
        self.assertIn("MimeType=text/plain;x-scheme-handler/example;", entry)
        self.assertNotIn("xdg-mime", entry)

    def test_actual_elf_architecture(self):
        path = self.root / "elf"
        path.write_bytes(b"\x7fELF\x02\x01" + b"\x00" * 12 + struct.pack("<H", 62))
        self.assertTrue(deb.elf(path))
        path.write_bytes(b"\x7fELF\x02\x01" + b"\x00" * 12 + struct.pack("<H", 183))
        with self.assertRaisesRegex(ValueError, "amd64"):
            deb.elf(path)

    def test_cancel_signal_identity(self):
        signal_file = self.root / "stop.json"
        deb.STOP_PATH, deb.SESSION_ID = signal_file, "owned-session"
        try:
            signal_file.write_text('{"sessionId":"another-session"}')
            deb.check_stop()
            signal_file.write_text('{"sessionId":"owned-session"}')
            with self.assertRaises(InterruptedError):
                deb.command(["sh", "-c", "exit 0"])
        finally:
            deb.STOP_PATH = deb.SESSION_ID = None

    def test_running_command_cancellation_and_staging_rollback(self):
        signal_file = self.root / "running-stop.json"
        deb.STOP_PATH, deb.SESSION_ID = signal_file, "running"
        timer = threading.Timer(0.3, lambda: signal_file.write_text('{"sessionId":"running"}'))
        timer.start()
        started = time.monotonic()
        try:
            with self.assertRaises(InterruptedError):
                deb.command([sys.executable, "-c", "import time; time.sleep(30)"])
            self.assertLess(time.monotonic() - started, 5)
        finally:
            timer.join()
            deb.STOP_PATH = deb.SESSION_ID = None
        metadata = self.metadata()
        inspect, command = deb.inspect, deb.command
        output = self.root / "output"
        try:
            deb.inspect = lambda *_: metadata
            def fail(*_, **__):
                raise ValueError("injected publish failure")
            deb.command = fail
            with self.assertRaisesRegex(ValueError, "injected"):
                deb.build(SimpleNamespace(metadata=json.dumps(metadata), output=str(output), dotnet="selected dotnet", publish_arguments='["publish"]'))
            self.assertEqual(list(output.iterdir()), [])
            self.assertTrue(self.icon.is_file())
        finally:
            deb.inspect, deb.command = inspect, command

    def test_publish_refreshes_restored_notice_paths(self):
        planned = self.metadata() | {"nugetRoots": ["/old-machine/cache"]}
        restored = planned | {"nugetRoots": [str(self.root / "restored-cache")]}
        inspect, command, package = deb.inspect, deb.command, deb.package
        probes = iter([planned, restored])
        try:
            deb.inspect = lambda *_: next(probes)
            deb.command = lambda *_, **__: ("", "")
            def check_package(source, output, metadata, work):
                self.assertEqual(metadata["nugetRoots"], restored["nugetRoots"])
                return output / "fixture.deb"
            deb.package = check_package
            deb.build(SimpleNamespace(metadata=json.dumps(planned), output=str(self.root / "refreshed"), dotnet="selected", publish_arguments='["publish"]'))
        finally:
            deb.inspect, deb.command, deb.package = inspect, command, package


def inspect_artifact(receipt_path, output):
    receipt = json.loads(receipt_path.read_text())
    package = receipt_path.parent / receipt["package"]
    assert deb.sha256(package) == receipt["sha256"]
    metadata = receipt["metadata"]
    contents = subprocess.check_output(["dpkg-deb", "--contents", str(package)], text=True)
    assert all("root/root" in entry for entry in contents.splitlines())
    assert not any("rwxrwx" in entry or "rws" in entry for entry in contents.splitlines())
    assert "/usr/bin/" + metadata["packageId"] in contents
    assert metadata["applicationId"] + ".desktop" in contents
    assert "licenses/doroti-linux/LGPL-3.txt" in contents
    with tempfile.TemporaryDirectory(dir=output) as temporary:
        stage = Path(temporary)
        subprocess.run(["dpkg-deb", "--extract", str(package), str(stage)], check=True)
        payload = stage / "opt" / metadata["packageId"]
        assert {str(path.relative_to(payload)): deb.sha256(path) for path in payload.rglob("*") if path.is_file()} == receipt["payload"]
        launcher = (stage / "usr/bin" / metadata["packageId"]).read_text()
        assert '"$@"' in launcher and "XDG_DATA_HOME=" not in launcher
        if "qtRuntime" in receipt:
            deb.qt_runtime.verify(payload, metadata["qt"], metadata["webView"])
            assert (payload / "qt/framework/6.12.0/lib/libQt6Core.so.6").is_file()
            assert "WAYLAND_DISPLAY" in launcher and "QT_QPA_PLATFORM=wayland" in launcher
        else:
            assert not any(stage.rglob("libQt6*"))
        entry = stage / "usr/share/applications" / (metadata["applicationId"] + ".desktop")
        subprocess.run(["desktop-file-validate", str(entry)], check=True)
    subprocess.run(["dpkg", "--validate-version", metadata["debianVersion"]], check=True)
    assert receipt["verification"]["gui"] == "not-run"
    assert set(receipt["publishPayload"]) - set(receipt["payload"]) == set(receipt["excludedRuntimeFiles"]) & set(receipt["publishPayload"])
    assert all(receipt["publishPayload"][name] == digest for name, digest in receipt["payload"].items())
    if "qtRuntime" in receipt:
        assert not any(name in receipt["depends"] for name in ("libqt6", "qml6-", "qt6-wayland"))
        engine = receipt.get("webEnginePackage")
        if engine:
            engine_file = receipt_path.parent / engine["package"]
            assert deb.sha256(engine_file) == engine["sha256"]
            runtime = receipt["qtRuntime"]["webEngine"]
            assert runtime["packageId"] + " (= " + runtime["packageVersion"] + ")" in receipt["depends"]
    else:
        assert ("qml6-module-qtwebengine" in receipt["depends"]) == metadata["webView"]
    (output / "artifact-inspection.json").write_text(json.dumps(dict(status="passed", package=str(package), install="not-run", gui="not-run"), indent=2))


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--receipt", type=Path)
    args = parser.parse_args()
    args.output.mkdir(parents=True, exist_ok=True)
    tests = unittest.TextTestRunner(verbosity=2).run(unittest.defaultTestLoader.loadTestsFromTestCase(PackagingContracts))
    if not tests.wasSuccessful():
        raise SystemExit(1)
    if args.receipt:
        inspect_artifact(args.receipt.resolve(), args.output.resolve())
    (args.output / "contracts.json").write_text(json.dumps(dict(status="passed", tests=tests.testsRun, install="not-run", gui="not-run"), indent=2))
