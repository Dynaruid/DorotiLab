"""Exercise the production WebCIL invalidation target with real MSBuild."""
import os
import json
from pathlib import Path
import subprocess
import tempfile
import unittest
from xml.sax.saxutils import escape


class WebcilCacheTests(unittest.TestCase):
    def test_widget_tree_interpretation_keeps_skia_aot_and_honors_opt_out(self):
        target = Path(__file__).resolve().parents[2] / "packages/platforms/build/Doroti.Web.targets"
        with tempfile.TemporaryDirectory(prefix="doroti-aot-tree-") as directory:
            project = Path(directory) / "tree.proj"
            project.write_text(f'''<Project>
              <ItemGroup>
                <_WasmAssembliesInternal Include="Doroti.Framework.Widgets.dll;Doroti.Framework.Rendering.dll;Doroti.Runtime.dll;Doroti.Skia.Rendering.dll" />
              </ItemGroup>
              <Import Project="{escape(str(target))}" />
              <Target Name="_WasmAotCompileApp" />
            </Project>''')
            for platform, aot, tree in [("Web", True, True), ("Web", True, False),
                                        ("Web", False, True), ("Android", True, True)]:
                result = subprocess.run([
                    "dotnet", "msbuild", str(project), "-nologo", "-verbosity:quiet",
                    "-target:_WasmAotCompileApp", "-getItem:_WasmAssembliesInternal",
                    f"-property:DorotiTarget={platform}",
                    f"-property:RunAOTCompilation={str(aot).lower()}",
                    f"-property:DorotiWebInterpretWidgetTree={str(tree).lower()}",
                ], capture_output=True, text=True)
                self.assertEqual(result.returncode, 0, result.stdout + result.stderr)
                items = json.loads(result.stdout)["Items"]["_WasmAssembliesInternal"]
                interpreted = {item["Filename"] for item in items
                               if item.get("AOT_InternalForceToInterpret") == "true"}
                expected = {"Doroti.Framework.Widgets", "Doroti.Framework.Rendering", "Doroti.Runtime"} if platform == "Web" and aot and tree else set()
                self.assertEqual(interpreted, expected)

    def test_execution_mode_and_assembly_content(self):
        target = Path(__file__).resolve().parents[2] / "packages/platforms/build/Doroti.Web.targets"
        with tempfile.TemporaryDirectory(prefix="doroti-webcil-") as directory:
            root = Path(directory)
            assemblies = [root / name for name in ("Package.dll", "Project.dll", "Runner.dll")]
            for assembly in assemblies:
                assembly.write_bytes(assembly.name.encode())
            project = root / "cache.proj"
            project.write_text(f'''<Project>
              <PropertyGroup>
                <DorotiTarget>Web</DorotiTarget><Configuration>Release</Configuration>
                <WasmEnableThreads>true</WasmEnableThreads><WasmStripILAfterAOT>true</WasmStripILAfterAOT>
                <IntermediateOutputPath>{escape(str(root / 'obj'))}/</IntermediateOutputPath>
                <TargetPath>{escape(str(assemblies[2]))}</TargetPath>
              </PropertyGroup>
              <ItemGroup>
                <ReferenceCopyLocalPaths Include="{escape(str(assemblies[0]))}" NuGetPackageId="Fixture" />
                <ReferenceCopyLocalPaths Include="{escape(str(assemblies[1]))}" />
              </ItemGroup>
              <Target Name="ResolveReferences" /><Target Name="_ResolveWasmConfiguration" />
              <Import Project="{escape(str(target))}" />
            </Project>''')
            caches = [root / "obj/webcil" / sub / f"{assembly.stem}.wasm"
                      for sub in ("", "publish") for assembly in assemblies]

            def seed():
                for cache in caches:
                    cache.parent.mkdir(parents=True, exist_ok=True)
                    cache.write_bytes(b"cached assembly")

            def run(aot, tree=False):
                result = subprocess.run([
                    "dotnet", "msbuild", str(project), "-nologo", "-verbosity:quiet",
                    "-target:DorotiInvalidateWebDependencyWebCil",
                    f"-property:RunAOTCompilation={str(aot).lower()}",
                    f"-property:DorotiWebInterpretWidgetTree={str(tree).lower()}",
                ], capture_output=True, text=True)
                self.assertEqual(result.returncode, 0, result.stdout + result.stderr)

            seed()
            run(True)
            self.assertFalse(any(cache.exists() for cache in caches), "Legacy caches must be invalidated")
            seed()
            run(True)
            self.assertTrue(all(cache.exists() for cache in caches), "Unchanged inputs should reuse caches")
            run(True, tree=True)
            self.assertFalse(any(cache.exists() for cache in caches), "Interpreted tree needs unstripped IL")
            seed()
            run(True, tree=True)
            self.assertTrue(all(cache.exists() for cache in caches), "Unchanged interpretation policy should reuse caches")
            identity = root / "obj/doroti-webcil-dependencies.identity"
            identity.write_text("\n".join(line for line in identity.read_text().splitlines()
                                          if not line.startswith("widget-tree-policy|")) + "\n")
            run(True, tree=True)
            self.assertFalse(any(cache.exists() for cache in caches), "An older tree policy must not reuse stripped Runtime IL")
            seed()
            run(False)
            self.assertFalse(any(cache.exists() for cache in caches), "Interpreted publishing needs full IL")
            seed()
            assemblies[1].write_bytes(b"changed project with an old timestamp")
            os.utime(assemblies[1], (1, 1))
            run(False)
            self.assertFalse(any(cache.exists() for cache in caches), "Project content must override timestamps")
            seed()
            assemblies[2].write_bytes(b"changed runner")
            os.utime(assemblies[2], (1, 1))
            run(False)
            self.assertFalse(any(cache.exists() for cache in caches), "Runner content must override timestamps")


if __name__ == "__main__":
    unittest.main()
