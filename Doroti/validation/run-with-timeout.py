"""Run a validation child with the repository's 20 minute process-tree deadline."""
import os
import signal
import subprocess
import sys


def main():
    if len(sys.argv) < 2:
        raise SystemExit('usage: run-with-timeout.py command [arguments...]')
    process = subprocess.Popen(sys.argv[1:], start_new_session=os.name != 'nt')
    try:
        return process.wait(timeout=1200)
    except (subprocess.TimeoutExpired, KeyboardInterrupt):
        if os.name == 'nt':
            subprocess.run(['taskkill', '/PID', str(process.pid), '/T', '/F'], timeout=30,
                           stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
        else:
            os.killpg(process.pid, signal.SIGKILL)
        process.wait(timeout=30)
        print('Validation child interrupted or exceeded 1200 seconds.', file=sys.stderr)
        return 124


if __name__ == '__main__':
    raise SystemExit(main())
