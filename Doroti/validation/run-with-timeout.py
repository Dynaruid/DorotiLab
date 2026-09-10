"""Run one validation command with the repository's mandatory 20 minute limit."""
import subprocess
import sys

process = subprocess.Popen(sys.argv[1:])
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
