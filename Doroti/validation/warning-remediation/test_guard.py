import tempfile
import unittest
from pathlib import Path

from guard import check


class WarningGuardTests(unittest.TestCase):
    def test_rejects_suppression_and_setting_bypasses(self):
        with tempfile.TemporaryDirectory(prefix="doroti-warning-guard-") as directory:
            root = Path(directory).resolve()
            module = root / "Doroti/src/Doroti.Framework.Sample"
            module.mkdir(parents=True)
            source = module / "Sample.cs"
            props = root / "Doroti/Directory.Build.props"
            original = "<Project><PropertyGroup><Nullable>enable</Nullable><TreatWarningsAsErrors>true</TreatWarningsAsErrors></PropertyGroup></Project>"
            props.write_text(original)
            source.write_text("class Sample {}")
            self.assertEqual(check(root), ([], 0))
            for forbidden in ["#pragma warning disable CS8600", "#pragma warning disable", "#nullable disable"]:
                source.write_text(forbidden + "\nclass Sample {}")
                self.assertTrue(check(root)[0], forbidden)
            source.write_text("class Sample {}")
            for mutation in [original.replace("enable", "disable"), original.replace("true", "false"), original.replace("</PropertyGroup>", "<NoWarn>CS8600</NoWarn></PropertyGroup>")]:
                props.write_text(mutation)
                self.assertTrue(check(root)[0], mutation)

    def test_nested_product_sources_are_checked_but_build_outputs_are_not(self):
        with tempfile.TemporaryDirectory(prefix="doroti-warning-guard-") as directory:
            root = Path(directory).resolve()
            module = root / "Doroti/src/Doroti.Framework.Sample"
            generated = module / "obj/Generated.cs"
            generated.parent.mkdir(parents=True)
            generated.write_text("#pragma warning disable CS8600")
            self.assertEqual(check(root), ([], 0))
            source = module / "Controls/Control.cs"
            source.parent.mkdir()
            source.write_text("#pragma warning disable CS8600")
            self.assertTrue(check(root)[0])

    def test_progress_baseline_does_not_allow_new_codes(self):
        with tempfile.TemporaryDirectory(prefix="doroti-warning-guard-") as directory:
            root = Path(directory).resolve()
            path = root / "Doroti/src/Doroti.Framework.Sample/Sample.cs"
            path.parent.mkdir(parents=True)
            baseline = {"suppressions": {path.relative_to(root).as_posix(): ["CS8600"]}}
            path.write_text("#pragma warning disable CS8600\nclass Sample {}")
            self.assertEqual(check(root, baseline), ([], 1))
            self.assertTrue(check(root)[0])
            path.write_text("#pragma warning disable CS8600, CS8602\nclass Sample {}")
            self.assertTrue(check(root, baseline)[0])


if __name__ == "__main__":
    unittest.main()
