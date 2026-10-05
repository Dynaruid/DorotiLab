"""Frozen CPU command/resource/admission contracts; GPU/native acceptance is separate."""
from pathlib import Path
import runpy
import sys

if sys.argv[1:]:
    sys.exit("This fixture does not accept arguments.")
sys.argv.append("--frames")
runpy.run_path(str(Path(__file__).with_name("platform_bootstrap_contract.py")), run_name="__main__")
