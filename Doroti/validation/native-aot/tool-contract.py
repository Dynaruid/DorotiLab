#!/usr/bin/env python3
"""Exercise byte accounting, missing/tampered evidence, and evaluated compilation modes."""
import importlib.util
import json
import os
from pathlib import Path
import plistlib
import subprocess
import tempfile
import unittest
import zipfile

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[2]


def module(name):
    spec = importlib.util.spec_from_file_location(name.replace("-", "_"), HERE / f"{name}.py")
    result = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(result)
    return result


sizes = module("size-report")
mono = module("verify-mono")


class EvidenceContract(unittest.TestCase):
    def setUp(self):
        root = ROOT / ".doroti/tmp"
        root.mkdir(parents=True, exist_ok=True)
        self.temporary = tempfile.TemporaryDirectory(prefix="native-aot-", dir=root)
        self.root = Path(self.temporary.name)

    def tearDown(self):
        self.temporary.cleanup()

    def test_bundle_links_hashes_and_archive_overhead(self):
        bundle = self.root / "Example.app"
        bundle.mkdir()
        (bundle / "Info.plist").write_bytes(plistlib.dumps({"CFBundleExecutable": "Example"}))
        (bundle / "Example").write_bytes(b"native executable")
        (bundle / "font.ttf").write_bytes(b"font")
        (bundle / "font-link.ttf").symlink_to("font.ttf")
        report = sizes.inventory(bundle)
        expected = sum(p.stat().st_size for p in bundle.iterdir() if not p.is_symlink())
        self.assertEqual(report["bundleRawBytes"], expected)
        self.assertEqual(report["categoriesBytes"]["fonts"], 4)
        before = report["files"]
        (bundle / "font.ttf").write_bytes(b"FONT")
        self.assertNotEqual(before, sizes.inventory(bundle)["files"])
        archive = self.root / "Example.ipa"
        with zipfile.ZipFile(archive, "w", zipfile.ZIP_DEFLATED) as output:
            for p in bundle.iterdir():
                if not p.is_symlink():
                    output.write(p, f"Payload/Example.app/{p.name}")
        zipped = sizes.inventory(archive)
        self.assertEqual(zipped["archiveUncompressedBytes"], expected)
        self.assertEqual(zipped["zipEntryCompressedBytes"] + zipped["archiveOverheadBytes"], archive.stat().st_size)
        self.assertIsNone(zipped["deviceInstalledBytes"])

    def test_absence_of_evidence_never_passes(self):
        self.assertEqual(mono.check(None)["monoAbsent"], "notVerified")
        bundle = self.root / "Example.app"
        bundle.mkdir()
        self.assertEqual(mono.check(bundle)["monoAbsent"], "notVerified")
        (bundle / "libmonosgen-2.0.dylib").write_bytes(b"injected runtime")
        self.assertEqual(mono.check(bundle)["monoAbsent"], "fail")

    def test_nativeaot_bridge_is_distinct_from_monovm(self):
        library = "/sdk/native/libxamarin-dotnet-nativeaot.a"
        qualified, rejected = mono.qualify_bridge_symbols(
            f"[ 1] {library}(nativeaot-bridge-dotnet-nativeaot.o)\n"
            "0x100001000 0x0000000C [ 1] _mono_jit_exec\n", [library])
        self.assertIn("_mono_jit_exec", qualified)
        self.assertFalse(rejected)
        qualified, rejected = mono.qualify_bridge_symbols(
            "[ 1] /injected/libmonosgen-2.0.a(jit.o)\n"
            "0x100001000 0x0000000C [ 1] _mono_jit_exec\n", [library])
        self.assertFalse(qualified)
        self.assertTrue(rejected)

    def test_mode_evaluation(self):
        runner = ROOT / "DorotiTestbedApp/ios/DorotiTestbedApp.iOS.csproj"
        seen = {}
        for mode in ("Mono", "NativeAot", "Mono"):
            for configuration in ("Debug", "Release"):
                artifacts = self.root / mode
                command = ["dotnet", "msbuild", str(runner), "-nologo", "-p:RuntimeIdentifier=ios-arm64",
                           f"-p:Configuration={configuration}", f"-p:DorotiCompilationMode={mode}",
                           f"-p:ArtifactsPath={artifacts}", "-p:TrimMode=full",
                           "-getProperty:TargetFramework,MauiVersion,PublishAot,UseMonoRuntime,MtouchInterpreter,BaseIntermediateOutputPath,BaseOutputPath,NuGetLockFilePath,_DorotiExpectedNativeBindingTfm",
                           "-getItem:TrimmerRootDescriptor,DorotiTargetDescriptor"]
                result = subprocess.run(command, capture_output=True, text=True, check=True, timeout=1200)
                value = json.loads(result.stdout)
                props = value["Properties"]
                self.assertIn(str(artifacts), props["BaseIntermediateOutputPath"])
                self.assertIn(str(artifacts), props["NuGetLockFilePath"])
                if mode == "NativeAot":
                    self.assertEqual(props["TargetFramework"], "net11.0-ios")
                    self.assertEqual(props["UseMonoRuntime"], "false")
                    self.assertEqual(props["MauiVersion"], "11.0.0-rc.1.26451.6")
                    self.assertEqual(props["PublishAot"], "true")
                    self.assertEqual(props["MtouchInterpreter"], "")
                    self.assertFalse(value["Items"]["TrimmerRootDescriptor"])
                else:
                    self.assertEqual(props["UseMonoRuntime"], "true")
                    self.assertEqual(props["TargetFramework"], "net10.0-ios")
                    self.assertEqual(props["MauiVersion"], "10.0.90")
                    self.assertNotEqual(props["PublishAot"], "true")
                    self.assertEqual(props["MtouchInterpreter"], "-all")
                    self.assertTrue(value["Items"]["TrimmerRootDescriptor"])
                self.assertEqual(props["_DorotiExpectedNativeBindingTfm"], props["TargetFramework"])
                self.assertEqual(value["Items"]["DorotiTargetDescriptor"][0]["TargetFramework"], props["TargetFramework"])
                key = (mode, configuration)
                if key in seen:
                    self.assertEqual(seen[key], props)
                seen[key] = props

    def test_ios_nativeaot_dependency_framework_selection(self):
        projects = (
            ROOT / "Doroti/src/Doroti.Host.Maui/Doroti.Host.Maui.csproj",
            ROOT / "Doroti/src/Doroti.Target.iOS.Maui.ios-arm64/Doroti.Target.iOS.Maui.ios-arm64.csproj",
            ROOT / "DorotiTestbedApp/ios/binding/DorotiTestbedApp.iOS.Native.csproj",
        )
        for mode, expected in (("Mono", "net10.0-ios"), ("NativeAot", "net11.0-ios")):
            for project in projects:
                # The RID-less pass matters: NuGet queries it to choose the
                # nearest framework before evaluating the RID-specific build.
                for rid in ("", "ios-arm64"):
                    result = subprocess.run(
                        ["dotnet", "msbuild", str(project), "-nologo",
                         f"-p:DorotiCompilationMode={mode}", f"-p:RuntimeIdentifier={rid}",
                         "-getProperty:TargetFramework,TargetFrameworks,PublishAot",
                         "-getItem:EmbeddedResource"],
                        cwd=ROOT, capture_output=True, text=True, check=True, timeout=1200)
                    value = json.loads(result.stdout)
                    props = value["Properties"]
                    frameworks = props["TargetFrameworks"] or props["TargetFramework"]
                    self.assertIn(expected, frameworks.split(";"))
                    self.assertNotEqual(props["PublishAot"], "true", "Libraries must not inherit executable PublishAot.")
                    if project.name.startswith("Doroti.Target."):
                        resources = value["Items"]["EmbeddedResource"]
                        self.assertEqual(len(resources), 1)
                        manifest = json.loads(Path(resources[0]["FullPath"]).read_text())
                        self.assertEqual(manifest["targetFramework"], expected)


if __name__ == "__main__":
    unittest.main()
