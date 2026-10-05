"""Real typed and process tooling contracts; no native Windowing acceptance."""
from pathlib import Path
import subprocess, sys
ROOT = Path(__file__).resolve().parents[2]
sys.exit(subprocess.call([sys.executable, str(ROOT / "Doroti/eng/run-with-timeout.py"), "--timeout", "1200", "dotnet", "run", "--project", str(ROOT / "Doroti/tests/Doroti.Tooling.Contracts.Tests"), "-c", "Debug"], cwd=ROOT))
