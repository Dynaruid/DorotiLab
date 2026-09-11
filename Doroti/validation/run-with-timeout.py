"""Run one validation command with the repository's mandatory 20 minute limit."""
import subprocess
import sys
import os
from pathlib import Path

environment = os.environ.copy()
environment.setdefault("PYTHONPYCACHEPREFIX", str(
    Path(__file__).resolve().parents[1] / "artifacts/validation/python-cache"))
process = subprocess.Popen(sys.argv[1:], env=environment)
try:
    sys.exit(process.wait(timeout=1200))
except subprocess.TimeoutExpired:
    if sys.platform == "win32":
        subprocess.run(["taskkill", "/PID", str(process.pid), "/T", "/F"], check=False)
    else:
        process.kill()
    process.wait()
    print("Validation exceeded 1200 seconds", file=sys.stderr)
    sys.exit(124)
