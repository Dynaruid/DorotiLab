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

    def test_ios_release_defaults(self):
        runner = ROOT / "DorotiTestbedApp/ios/DorotiTestbedApp.iOS.csproj"
        cases = [
            ("Debug", "", [], "Mono", "iossimulator-arm64"),
            ("Debug", "ios-arm64", [], "Mono", "ios-arm64"),
            ("Release", "", [], "NativeAot", "ios-arm64"),
            ("Release", "ios-arm64", [], "NativeAot", "ios-arm64"),
            ("Release", "iossimulator-arm64", [], "Mono", "iossimulator-arm64"),
            ("Release", "ios-arm64", ["-p:DorotiCompilationMode=Mono"], "Mono", "ios-arm64"),
            ("Release", "ios-arm64", ["-p:PublishAot=false"], "Mono", "ios-arm64"),
        ]
        for configuration, rid, overrides, mode, effective_rid in cases:
            command = ["dotnet", "msbuild", str(runner), "-nologo", f"-p:Configuration={configuration}",
                "-getProperty:DorotiCompilationMode,TargetFramework,RuntimeIdentifier,PublishAot,UseMonoRuntime,ArtifactsPath,MtouchInterpreter",
                "-getItem:ProjectReference"]
            if rid: command.append(f"-p:RuntimeIdentifier={rid}")
            value = json.loads(subprocess.run(command + overrides, cwd=ROOT, capture_output=True,
                text=True, check=True, timeout=1200).stdout)
            props = value["Properties"]
            self.assertEqual(props["DorotiCompilationMode"], mode)
            self.assertEqual(props["RuntimeIdentifier"], effective_rid)
            if mode == "NativeAot":
                self.assertEqual(props["TargetFramework"], "net11.0-ios")
                self.assertEqual(props["PublishAot"], "true")
                self.assertEqual(props["UseMonoRuntime"], "false")
                self.assertEqual(props["MtouchInterpreter"], "")
                self.assertIn("NativeAot", props["ArtifactsPath"])
                for reference in value["Items"]["ProjectReference"]:
                    metadata = reference["AdditionalProperties"]
                    self.assertIn("DorotiCompilationMode=NativeAot", metadata)
                    self.assertIn("DorotiIosTargetFramework=net11.0-ios", metadata)
                    self.assertNotIn("PublishAot=", metadata)
            else:
                self.assertEqual(props["TargetFramework"], "net10.0-ios")
                self.assertNotEqual(props["PublishAot"], "true")
                self.assertEqual(props["UseMonoRuntime"], "true")
        # Shared profile imports must not promote a neutral or non-iOS library.
        profile = ROOT / "Doroti/src/Doroti.Runner.Sdk/Sdk/Doroti.IosNativeAot.props"
        project = self.root / "Defaults.proj"
        project.write_text(f'<Project><Import Project="{profile}" /></Project>')
        for rid in ("", "android-arm64", "win-x64", "osx-arm64", "maccatalyst-arm64", "browser-wasm"):
            value = json.loads(subprocess.run(["dotnet", "msbuild", str(project), "-nologo",
                "-p:Configuration=Release", f"-p:RuntimeIdentifier={rid}",
                "-getProperty:DorotiCompilationMode,PublishAot"], capture_output=True,
                text=True, check=True, timeout=1200).stdout)["Properties"]
            self.assertEqual(value["DorotiCompilationMode"], "")
            self.assertNotEqual(value["PublishAot"], "true")

        # NuGet introduces transitive references after evaluation; these must
        # receive the same profile before RID-less framework discovery.
        host = ROOT / "Doroti/src/Doroti.Host.Maui/Doroti.Host.Maui.csproj"
        late_reference = self.root / "LateReference.targets"
        late_reference.write_text(f'''<Project>
          <Target Name="InjectLateReference" BeforeTargets="DorotiApplyIosCompilationProfileToTransitiveReferences">
            <ItemGroup>
              <_TransitiveProjectReferences Include="{host}" />
              <ProjectReference Include="@(_TransitiveProjectReferences)" />
            </ItemGroup>
          </Target>
        </Project>''')
        value = json.loads(subprocess.run(["dotnet", "msbuild", str(runner), "-nologo",
            "-p:Configuration=Release", f"-p:CustomAfterMicrosoftCommonTargets={late_reference}",
            "-t:DorotiApplyIosCompilationProfileToRestore;DorotiApplyIosCompilationProfileToTransitiveReferences",
            "-getProperty:_GenerateRestoreGraphProjectEntryInputProperties", "-getItem:ProjectReference"],
            cwd=ROOT, capture_output=True, text=True, check=True, timeout=1200).stdout)
        restore_properties = value["Properties"]["_GenerateRestoreGraphProjectEntryInputProperties"]
        self.assertIn("DorotiCompilationMode=NativeAot", restore_properties)
        self.assertIn("RuntimeIdentifier=ios-arm64", restore_properties)
        self.assertIn("ArtifactsPath=", restore_properties)
        self.assertNotIn("PublishAot=", restore_properties)
        reference = next(item for item in value["Items"]["ProjectReference"] if item["FullPath"] == str(host))
        self.assertIn("DorotiCompilationMode=NativeAot", reference["AdditionalProperties"])
        self.assertIn("DorotiIosTargetFramework=net11.0-ios", reference["AdditionalProperties"])
        self.assertNotIn("PublishAot=", reference["AdditionalProperties"])

        target = ROOT / "Doroti/src/Doroti.Target.iOS.Maui.ios-arm64/Doroti.Target.iOS.Maui.ios-arm64.csproj"
        props = json.loads(subprocess.run(["dotnet", "msbuild", str(target), "-nologo",
            "-p:Configuration=Release", "-getProperty:TargetFramework,DorotiCompilationMode,PublishAot"],
            cwd=ROOT, capture_output=True, text=True, check=True, timeout=1200).stdout)["Properties"]
        self.assertEqual(props["TargetFramework"], "net11.0-ios")
        self.assertEqual(props["DorotiCompilationMode"], "NativeAot")
        self.assertNotEqual(props["PublishAot"], "true")

    def test_cli_default_selection(self):
        helper = str(ROOT / "Doroti/eng/launch-identity.ps1").replace("'", "''")
        script = f". '{helper}'; " + """
        $rows = @(
            @('ios','Release','','ios-arm64','NativeAot'),
            @('ios','Release','','','NativeAot'),
            @('ios','Debug','','ios-arm64','Mono'),
            @('ios','Release','','iossimulator-arm64','Mono'),
            @('ios','Release','Mono','ios-arm64','Mono'),
            @('ios','Debug','NativeAot','ios-arm64','NativeAot'))
        foreach ($platform in @('android','windows','web','linux','macos','maccatalyst')) {
            $rows += ,@($platform,'Release','','','Mono')
        }
        foreach ($row in $rows) {
            $actual = Resolve-DorotiCompilationMode $row[0] $row[1] $row[2] $row[3]
            if ($actual -cne $row[4]) { throw "Unexpected compilation mode for $row : $actual" }
        }
        if (@(Get-DorotiCompilationArguments).Count -ne 0) { throw 'Unspecified helper mode must allow MSBuild defaults' }
        Write-Output 'CLI default selection PASS'
        """
        subprocess.run(["pwsh", "-NoProfile", "-Command", script], check=True, timeout=1200,
                       capture_output=True, text=True)

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
                    if mode == "NativeAot" and project.name == "Doroti.Host.Maui.csproj":
                        self.assertEqual(frameworks, expected, "NativeAOT restore must not require non-iOS workloads.")
                    self.assertNotEqual(props["PublishAot"], "true", "Libraries must not inherit executable PublishAot.")
                    if project.name.startswith("Doroti.Target."):
                        resources = value["Items"]["EmbeddedResource"]
                        self.assertEqual(len(resources), 1)
                        manifest = json.loads(Path(resources[0]["FullPath"]).read_text())
                        self.assertEqual(manifest["targetFramework"], expected)


if __name__ == "__main__":
    unittest.main()
