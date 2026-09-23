#!/usr/bin/env python3
"""Gate the standalone Qt/Vulkan resize fixture on mapped validation output."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import re
import signal
import subprocess


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--executable", type=Path, required=True)
    parser.add_argument("--qpa", choices=("wayland", "xcb"), required=True)
    parser.add_argument("--output", type=Path, required=True, help="New result directory")
    parser.add_argument("--sync-delay-ms", type=int, default=50)
    args = parser.parse_args()
    if not 0 <= args.sync_delay_ms <= 500:
        parser.error("--sync-delay-ms must be between 0 and 500")
    executable = args.executable.resolve()
    if not executable.is_file():
        parser.error(f"Fixture not found: {executable}")
    output = args.output.resolve()
    output.mkdir(parents=True, exist_ok=False)
    environment = dict(os.environ, QT_QPA_PLATFORM=args.qpa,
                       DOROTI_QT_WSI_SYNC_DELAY_MS=str(args.sync_delay_ms))
    with (output / "fixture.log").open("w") as log:
        process = subprocess.Popen([str(executable)], env=environment,
                                   stdout=log, stderr=subprocess.STDOUT,
                                   start_new_session=True)
        try:
            exit_code = process.wait(timeout=30)
            timed_out = False
        except subprocess.TimeoutExpired:
            os.killpg(process.pid, signal.SIGKILL)
            process.wait()
            exit_code, timed_out = process.returncode, True
    content = (output / "fixture.log").read_text(errors="replace")
    swaps = re.search(r"frameSwaps=(\d+)", content)
    vuid_ids = sorted(set(re.findall(r"VUID-[A-Za-z0-9-]+", content)))
    checks = {
        "completedTenCycles": "cycles=10" in content,
        "validationLayerLoaded": "validationLayerLoaded=1" in content,
        "frameSwapped": swaps is not None and int(swaps.group(1)) > 0,
        "noVulkanValidationError": not vuid_ids and "Validation Error" not in content,
        "exit": exit_code == 0 and not timed_out,
    }
    report = {"schemaVersion": 1, "qpa": args.qpa, "syncDelayMs": args.sync_delay_ms,
              "fixtureSha256": hashlib.sha256(executable.read_bytes()).hexdigest(),
              "exitCode": exit_code, "timedOut": timed_out,
              "frameSwaps": int(swaps.group(1)) if swaps else None,
              "vuidIds": vuid_ids, "checks": checks,
              "status": "passed" if all(checks.values()) else "failed",
              "log": "fixture.log"}
    (output / "result.json").write_text(json.dumps(report, indent=2) + "\n")
    print(json.dumps(report, indent=2))
    return 0 if report["status"] == "passed" else 1


if __name__ == "__main__":
    raise SystemExit(main())
