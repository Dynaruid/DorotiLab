"""Run one test command with a 20 minute wall limit and process-tree cleanup."""
import argparse
import os
import signal
import subprocess
import sys


def run(command, timeout=1200):
    process = subprocess.Popen(command, start_new_session=os.name != "nt")
    try:
        return process.wait(timeout=timeout)
    except (subprocess.TimeoutExpired, KeyboardInterrupt) as error:
        print(f"FAIL: {type(error).__name__} after limit {timeout}s: {command}", flush=True)
        if os.name == "nt":
            subprocess.run(["taskkill", "/PID", str(process.pid), "/T", "/F"], check=False)
        else:
            os.killpg(process.pid, signal.SIGKILL)
        process.wait()
        return 124 if isinstance(error, subprocess.TimeoutExpired) else 130


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--timeout", type=float, default=1200)
    parser.add_argument("command", nargs=argparse.REMAINDER)
    args = parser.parse_args()
    if not args.command or not 0 < args.timeout <= 1200:
        parser.error("Supply a command and a timeout in (0, 1200].")
    sys.exit(run(args.command, args.timeout))
