"""Real managed G0 subsets; native Windowing acceptance is a separate gate."""
from pathlib import Path
from datetime import datetime, timezone
import json
import shutil
import subprocess
import sys
import uuid

ROOT = Path(__file__).resolve().parents[2]
if sys.argv[1:] not in ([], ["--frames"]):
    sys.exit("Expected no arguments or --frames.")
frames_only = sys.argv[1:] == ["--frames"]
label = "frames" if frames_only else "bootstrap"
run_root = ROOT / "temp/testing/platform-decoupling" / label
run = run_root / uuid.uuid4().hex
run.mkdir(parents=True)
print(f"Contract artifacts: {run}", flush=True)
command = [sys.executable, str(ROOT / "Doroti/eng/run-with-timeout.py"), "--timeout", "1200",
    "dotnet", "run", "--project", str(ROOT / "Doroti/tests/Doroti.Platform.Contracts.Tests"),
    "-c", "Debug", "--artifacts-path", str(run / "build")]
if frames_only:
    command.extend(["--", "--frames"])
with (run / "bootstrap.log").open("w", encoding="utf-8") as log:
    result = subprocess.run(command, cwd=ROOT, stdout=log, stderr=subprocess.STDOUT)
output = (run / "bootstrap.log").read_text(encoding="utf-8", errors="replace")
print(output, flush=True)
if result.returncode == 0:
    evidence = ROOT / "Doroti/docs/migrations/design-platform" / f"{label}-verification.json"
    evidence.write_text(json.dumps({
        "schema": f"doroti.{label}-verification/v1",
        "verifiedUtc": datetime.now(timezone.utc).isoformat(),
        "command": "python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/tests/" + ("frame_submission_contract.py" if frames_only else "platform_bootstrap_contract.py"),
        "result": "PASS",
        "scope": "managed/headless contracts and real shared widget tree/CPU rendering; G0 partial",
        "results": [line for line in output.splitlines() if line.startswith("PASS:")],
        "notVerified": ["native windows G1", "physical IME/native focus", "GPU fence and native owner/frame bridge acceptance"],
        "runId": run.name,
        "rawArtifacts": "removed after successful summary",
    }, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    assert run.resolve().parent == run_root.resolve() and run_root.resolve().is_relative_to(ROOT.resolve())
    shutil.rmtree(run)
    print(f"Verified evidence: {evidence}; successful raw artifacts removed.", flush=True)
sys.exit(result.returncode)
