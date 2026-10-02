"""Exercise cleanup against isolated repositories, never the real user caches."""
from pathlib import Path
import json
import platform
import shutil
import subprocess
import tempfile
import unittest


SCRIPT = Path(__file__).resolve().parents[2] / "cleanup-plan-all.ps1"


class CleanupTests(unittest.TestCase):
    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory(prefix="doroti-cleanup-")
        self.addCleanup(self.temporary.cleanup)
        self.base = Path(self.temporary.name).resolve()
        self.root = self.base / "workspace with [brackets]"
        self.root.mkdir()
        shutil.copyfile(SCRIPT, self.root / SCRIPT.name)
        self.write("Doroti/eng/doroti.ps1", "# fixture")
        self.write("src/Demo/Demo.csproj", "<Project />")
        self.write("src/Demo/Source.cs", "// preserve source")
        self.write(".gitignore", "**/bin/\n**/obj/\n**/.doroti/\n"
                   "/Doroti/artifacts/\n/temp/testing/\n**/node_modules/\n")
        self.git("init", "-q")
        self.git("add", ".")

    def write(self, relative, contents="disposable"):
        path = self.root / relative
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_text(contents, encoding="utf-8")
        return path

    def git(self, *arguments):
        subprocess.run(["git", "-C", str(self.root), *arguments],
                       check=True, capture_output=True, timeout=1200)

    def run_cleanup(self, *arguments):
        result = subprocess.run(
            ["pwsh", "-NoProfile", "-NonInteractive", "-File",
             str(self.root / SCRIPT.name), *arguments],
            text=True, capture_output=True, timeout=1200)
        self.assertEqual(result.returncode, 0, result.stdout + result.stderr)
        return result.stdout

    def seed_outputs(self):
        return [
            self.write("src/Demo/bin/Debug/Demo.dll"),
            self.write("src/Demo/obj/project.assets.json", "{}"),
            self.write("Doroti/artifacts/release/latest/package.nupkg"),
            self.write("Doroti/artifacts/release/0.3.0-beta.rc.20260929010529/package.nupkg"),
            self.write("temp/testing/a future run/capture.trace"),
            self.write("samples/NewApp/.doroti/cache/build.bin"),
            self.write(".doroti/tmp/session/output.log"),
            self.write("orphan/obj/project.assets.json", "{}"),
            self.write("orphan/bin/Release/net10.0/Orphan.deps.json", "{}"),
        ]

    def test_preview_does_not_change_files(self):
        outputs = self.seed_outputs()
        before = {path: path.read_bytes() for path in outputs}
        output = self.run_cleanup()
        self.assertIn("Preview only", output)
        for path, contents in before.items():
            self.assertEqual(path.read_bytes(), contents)

    def test_execute_removes_future_outputs_and_all_releases(self):
        outputs = self.seed_outputs()
        output = self.run_cleanup("-Execute")
        self.assertIn("Cleanup complete", output)
        self.assertTrue(all(not path.exists() for path in outputs))
        self.assertTrue((self.root / "src/Demo/Source.cs").exists())
        self.assertTrue((self.root / "src/Demo/Demo.csproj").exists())

    def test_whatif_does_not_delete(self):
        outputs = self.seed_outputs()
        self.run_cleanup("-Execute", "-WhatIf")
        self.assertTrue(all(path.exists() for path in outputs))

    def test_tracked_output_directory_is_preserved(self):
        tracked = self.write("src/Demo/obj/handwritten.cs", "// preserve")
        self.git("add", "-f", "src/Demo/obj/handwritten.cs")
        sibling = self.write("src/Demo/obj/project.assets.json", "{}")
        output = self.run_cleanup("-Execute")
        self.assertIn("Git-tracked", output)
        self.assertTrue(tracked.exists())
        self.assertTrue(sibling.exists())

    def test_sources_reference_and_session_data_are_preserved(self):
        kept = [
            self.write("reference/SDK/SDK.csproj", "<Project />"),
            self.write("reference/SDK/bin/utility.ps1"),
            self.write("scripts/bin/utility.sh"),
            self.write(".doroti/evidence/result.json", "{}"),
            self.write("samples/App/.doroti/dev/session.json", "{}"),
            self.write("temp/deploy/source.cs"),
        ]
        self.run_cleanup("-Execute")
        self.assertTrue(all(path.exists() for path in kept))

    def test_node_modules_are_opt_in(self):
        dependency = self.write("web/node_modules/package/index.js")
        self.run_cleanup("-Execute")
        self.assertTrue(dependency.exists())
        self.run_cleanup("-Execute", "-IncludeDependencies")
        self.assertFalse(dependency.exists())

    def test_nested_links_do_not_delete_external_data(self):
        outside = self.base / "outside"
        outside.mkdir()
        sentinel = outside / "keep.txt"
        sentinel.write_text("preserve", encoding="utf-8")
        output = self.root / "src/Demo/bin"
        output.mkdir()
        (output / "linked directory").symlink_to(outside, target_is_directory=True)
        (output / "linked file").symlink_to(sentinel)
        (output / "broken link").symlink_to(outside / "missing")
        self.run_cleanup("-Execute")
        self.assertFalse(output.exists())
        self.assertEqual(sentinel.read_text(encoding="utf-8"), "preserve")

    def test_linked_target_and_ancestor_are_preserved(self):
        outside = self.base / "outside"
        (outside / "cache").mkdir(parents=True)
        sentinel = outside / "cache/keep.txt"
        sentinel.write_text("preserve", encoding="utf-8")
        (self.root / "Doroti/artifacts").symlink_to(outside, target_is_directory=True)
        (self.root / ".doroti").symlink_to(outside, target_is_directory=True)
        self.run_cleanup("-Execute")
        self.assertTrue(sentinel.exists())
        self.assertTrue((self.root / "Doroti/artifacts").is_symlink())
        self.assertTrue((self.root / ".doroti").is_symlink())

    def test_repeated_execute_with_no_targets_succeeds(self):
        self.seed_outputs()
        self.run_cleanup("-Execute")
        output = self.run_cleanup("-Execute")
        self.assertIn("Selected: 0 targets", output)

    def seed_user_caches(self):
        profile = self.base / "profile"
        caches = [
            ".nuget/packages/package/version/package.nupkg",
            ".npm/_cacache/cache.bin",
            ".vscode/extensions/old.extension-1.0/main.js",
        ]
        if platform.system() == "Darwin":
            caches += [
                "Library/Application Support/Code/CachedExtensionVSIXs/install.vsix",
                "Library/Caches/Google/Chrome/Default/cache.bin",
                "Library/Application Support/Figma/DesktopProfile/v99/Code Cache/code.bin",
                "Library/Containers/com.apple.CoreDevice.CoreDeviceService/Data/"
                "Library/Caches/AppInstallationBinaryDeltas/app/install.bin",
            ]
        outputs = []
        for relative in caches:
            path = profile / relative
            path.parent.mkdir(parents=True, exist_ok=True)
            path.write_text("cache", encoding="utf-8")
            outputs.append(path)
        obsolete = profile / ".vscode/extensions/.obsolete"
        obsolete.write_text(json.dumps({"old.extension-1.0": True}), encoding="utf-8")
        preserved = [
            ".vscode/extensions/current.extension-2.0/main.js",
            "Library/Application Support/Code/User/settings.json",
            "Library/Application Support/Google/Chrome/Default/Bookmarks",
            "Library/Application Support/Google/Chrome/OptGuideOnDeviceModel/weights.bin",
            "Library/Containers/com.docker.docker/Data/Docker.raw",
        ]
        keep = []
        for relative in preserved:
            path = profile / relative
            path.parent.mkdir(parents=True, exist_ok=True)
            path.write_text("preserve", encoding="utf-8")
            keep.append(path)
        return profile, outputs, keep

    def test_all_scopes_preview_and_delete_isolated_caches(self):
        profile, caches, keep = self.seed_user_caches()
        outputs = self.seed_outputs()
        arguments = ("-Scope", "All", "-UserCacheRoot", str(profile))
        self.run_cleanup(*arguments)
        self.assertTrue(all(path.exists() for path in caches + outputs + keep))
        self.run_cleanup(*arguments, "-Execute")
        self.assertTrue(all(not path.exists() for path in caches + outputs))
        self.assertTrue(all(path.exists() for path in keep))

    def test_cache_only_scope_preserves_project_output(self):
        profile, _, _ = self.seed_user_caches()
        outputs = self.seed_outputs()
        self.run_cleanup("-Scope", "DeveloperCaches", "-UserCacheRoot",
                         str(profile), "-Execute")
        self.assertTrue(all(path.exists() for path in outputs))

    def test_invalid_obsolete_paths_fail_before_any_deletion(self):
        profile, caches, keep = self.seed_user_caches()
        obsolete = profile / ".vscode/extensions/.obsolete"
        for invalid in ("", "..", "../current.extension-2.0"):
            with self.subTest(path=invalid):
                obsolete.write_text(json.dumps({invalid: True}), encoding="utf-8")
                result = subprocess.run(
                    ["pwsh", "-NoProfile", "-NonInteractive", "-File",
                     str(self.root / SCRIPT.name), "-Scope", "DeveloperCaches",
                     "-UserCacheRoot", str(profile), "-Execute"],
                    text=True, capture_output=True, timeout=1200)
                self.assertNotEqual(result.returncode, 0, result.stdout + result.stderr)
                self.assertTrue(all(path.exists() for path in caches + keep))


if __name__ == "__main__":
    unittest.main()
