"""Keep failure propagation and bounded execution from silently regressing."""
import importlib.util
from pathlib import Path
import sys
import shutil
import subprocess

spec = importlib.util.spec_from_file_location("timeout_runner", Path(__file__).parents[1] / "eng/run-with-timeout.py")
runner = importlib.util.module_from_spec(spec)
spec.loader.exec_module(runner)
assert runner.run([sys.executable, "-c", "raise SystemExit(7)"]) == 7
assert runner.run([sys.executable, "-c", "import time; time.sleep(60)"], timeout=0.2) == 124
assert runner.run([sys.executable, "-c", "pass"]) == 0
print("Runner failure, timeout, success: PASS")
if len(sys.argv) > 1:
    scratch = Path(sys.argv[1]).resolve()
    root = Path(__file__).resolve().parents[2]
    assert scratch.is_relative_to(root / "temp/testing")
    eng = scratch / "Doroti/eng"
    eng.mkdir(parents=True)
    for name in ("doroti.ps1", "launch-identity.ps1"):
        shutil.copyfile(root / "Doroti/eng" / name, eng / name)
    validation = eng / "validate.ps1"
    validation.write_text("param([string] $Suite)\nexit 7\n")
    for verb in ("validate", "audit", "release"):
        result = subprocess.run(["pwsh", "-NoProfile", "-File", str(eng / "doroti.ps1"), verb], capture_output=True, text=True)
        assert result.returncode != 0 and "Release: PASS" not in result.stdout, (verb, result.stdout)
    validation.unlink()
    result = subprocess.run(["pwsh", "-NoProfile", "-File", str(eng / "doroti.ps1"), "validate"], capture_output=True, text=True)
    assert result.returncode != 0, "Missing validation script was accepted"
    print("Top-level validate/audit/release failure propagation and missing runner: PASS")
