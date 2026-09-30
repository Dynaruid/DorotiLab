"""Exercise installer sequencing/failures without installing or using sudo."""

import contextlib
import hashlib
import importlib.util
import io
import json
from pathlib import Path
import subprocess
import tempfile
import unittest
from unittest.mock import patch


spec = importlib.util.spec_from_file_location(
    "updater", Path(__file__).resolve().parents[1] / "update-dotnet-macos.py")
updater = importlib.util.module_from_spec(spec)
spec.loader.exec_module(updater)


class UpdateTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name) / "dotnet"
        self.staging = Path(self.temp.name) / "staging"
        self.staging.mkdir()
        self.enterContext(patch.object(updater, "DOTNET_ROOT", self.root))
        self.enterContext(patch.object(updater, "DOTNET", self.root / "dotnet"))
        self.enterContext(contextlib.redirect_stdout(io.StringIO()))

    # unittest.enterContext was only added in Python 3.11; macOS has 3.9.
    def enterContext(self, context):
        value = context.__enter__()
        self.addCleanup(context.__exit__, None, None, None)
        return value

    def installed_sdk(self, major):
        dll = self.root / "sdk" / updater.PROFILES[major]["sdk"] / "dotnet.dll"
        dll.parent.mkdir(parents=True)
        dll.touch()

    def fake_run(self, major, wrong_workload=False, fail_install=False):
        profile = updater.PROFILES[major]

        def invoke(command, cwd=None, capture=False):
            args = [str(x) for x in command]
            if "--version" in args and "install" not in args:
                selection = json.loads((cwd / "global.json").read_text())["sdk"]
                self.assertEqual(selection["version"], profile["sdk"])
                self.assertEqual(selection["rollForward"], "disable")
                self.assertNotIn("workloadVersion", selection)
                if "workload" in args:
                    return "old-manifests" if wrong_workload else profile["workloads"]
                return profile["sdk"]
            if "workload" in args and "install" in args:
                self.assertEqual(args, updater.workload_command(profile))
                if fail_install:
                    raise subprocess.CalledProcessError(1, args)
                tpv = ".".join(profile["ios"].split(".")[:2])
                marker = self.root / "packs" / f"Microsoft.iOS.Sdk.net{major}.0_{tpv}" / profile["ios"] / "Sdk" / "Sdk.props"
                marker.parent.mkdir(parents=True)
                marker.touch()
            return ""
        return invoke

    def test_existing_sdks_still_update_and_verify_each_workload(self):
        for major in ("10", "11"):
            self.installed_sdk(major)
            with patch.object(updater, "run", side_effect=self.fake_run(major)), \
                    patch.object(updater, "installer_info") as fetch:
                updater.update(major, "arm64", self.staging)
                fetch.assert_not_called()

    def test_missing_sdk_checks_signature_before_installing(self):
        with patch.object(updater, "installer_info", return_value={}), \
                patch.object(updater, "download_installer"), \
                patch.object(updater, "run", side_effect=self.fake_run("10")) as run:
            updater.update("10", "arm64", self.staging)
        commands = [str(call.args[0]) for call in run.call_args_list]
        self.assertIn("--check-signature", commands[0])
        self.assertIn("/usr/sbin/installer", commands[1])

    def test_failed_signature_never_installs(self):
        with patch.object(updater, "installer_info", return_value={}), \
                patch.object(updater, "download_installer"), \
                patch.object(updater, "run", side_effect=subprocess.CalledProcessError(1, "pkgutil")) as run:
            with self.assertRaises(subprocess.CalledProcessError):
                updater.update("10", "arm64", self.staging)
            self.assertEqual(run.call_count, 1)

    def test_failed_workload_stops_before_success_checks(self):
        self.installed_sdk("11")
        with patch.object(updater, "run", side_effect=self.fake_run("11", fail_install=True)) as run:
            with self.assertRaises(subprocess.CalledProcessError):
                updater.update("11", "arm64", self.staging)
        self.assertIn("install", run.call_args.args[0])

    def test_old_workload_is_not_reported_as_success(self):
        self.installed_sdk("11")
        with patch.object(updater, "run", side_effect=self.fake_run("11", wrong_workload=True)):
            with self.assertRaisesRegex(RuntimeError, "워크로드 검증 실패"):
                updater.update("11", "arm64", self.staging)

    def test_download_rejects_bad_hash(self):
        payload = b"corrupted installer"
        dest = self.staging / "installer.pkg"
        with patch.object(updater.urllib.request, "urlopen", return_value=io.BytesIO(payload)):
            with self.assertRaisesRegex(RuntimeError, "SHA-512"):
                updater.download_installer({"url": "https://example.invalid", "hash": hashlib.sha512(b"original").hexdigest()}, dest)
        self.assertFalse(dest.exists())

    def test_dry_run_does_not_download_or_execute(self):
        with patch.object(updater.platform, "system", return_value="Darwin"), \
                patch.object(updater.os, "geteuid", return_value=501), \
                patch.object(updater, "architecture", return_value="arm64"), \
                patch.object(updater, "run") as run, \
                patch.object(updater, "installer_info") as fetch:
            self.assertEqual(updater.main(["--dry-run"]), 0)
            run.assert_not_called()
            fetch.assert_not_called()


if __name__ == "__main__":
    unittest.main()
