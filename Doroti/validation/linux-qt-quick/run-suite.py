#!/usr/bin/env python3
"""Run the bounded Linux Qt gates and keep one log and status per step."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import subprocess
import sys
from datetime import datetime, timezone

ROOT = Path(__file__).resolve().parents[3]
TIMEOUT = ROOT / "Doroti/validation/run-with-timeout.py"
HERE = Path(__file__).resolve().parent


def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest() if path and path.is_file() else None


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--output", type=Path, required=True, help="New artifact directory")
    parser.add_argument("--shim", type=Path, help="libdoroti_qt_host.so from the build under test")
    parser.add_argument("--app", type=Path, help="Product DLL from the same build")
    parser.add_argument("--qpa", choices=("wayland", "xcb"))
    parser.add_argument("--product", action="store_true", help="Run the actual product fixture")
    args = parser.parse_args()
    if args.product and (not args.app or not args.qpa or not args.shim):
        parser.error("--product requires --app, --qpa, and --shim")
    output = args.output.resolve()
    output.mkdir(parents=True, exist_ok=False)
    runs = []
    index = {
        "schemaVersion": 1,
        "createdUtc": datetime.now(timezone.utc).isoformat(),
        "sourceCommit": subprocess.run(["git", "rev-parse", "HEAD"], cwd=ROOT,
                                       capture_output=True, text=True).stdout.strip(),
        "qpa": args.qpa,
        "qtVersion": subprocess.run(["pkg-config", "--modversion", "Qt6Core"],
                                    capture_output=True, text=True).stdout.strip(),
        "shim": str(args.shim.resolve()) if args.shim else None,
        "shimSha256": digest(args.shim),
        "app": str(args.app.resolve()) if args.app else None,
        "appSha256": digest(args.app),
        "runs": runs,
    }

    def save():
        (output / "index.json").write_text(json.dumps(index, indent=2) + "\n")

    def run(name, command, env=None):
        log = output / f"{name}.log"
        with log.open("w") as stream:
            result = subprocess.run([sys.executable, str(TIMEOUT), *map(str, command)],
                                    cwd=ROOT, env=env, stdout=stream, stderr=subprocess.STDOUT)
        runs.append({"name": name, "status": "passed" if result.returncode == 0 else "failed",
                     "exitCode": result.returncode, "log": log.name})
        save()
        print(f"{name}: {runs[-1]['status']} ({log})", flush=True)
        return result.returncode == 0

    def skip(name, reason):
        runs.append({"name": name, "status": "notVerified", "reason": reason})
        save()

    if not run("native-sync", [sys.executable, HERE / "check-native-sync.py"]):
        return 1
    if not run("managed-contract", ["dotnet", "run", "--project",
                                   ROOT / "Doroti/validation/linux-qt-contract/Contract.csproj"]):
        return 1
    if args.shim and args.shim.is_file():
        build = output / "driver"
        if not run("driver-configure", ["cmake", "-S", HERE, "-B", build,
                                        f"-DDOROTI_SHIM={args.shim.resolve()}"]):
            return 1
        if not run("driver-build", ["cmake", "--build", build, "-j2"]):
            return 1
        if not run("native-contract", [build / "native-contract"]):
            return 1
        if (build / "webview-contract").is_file():
            if not run("webview-contract", [build / "webview-contract"]):
                return 1
        else:
            skip("webview-contract", "WebEngine shim not present")
        if not run("gpu-contract", ["dotnet", "run", "--no-build", "--project",
                                    ROOT / "Doroti/validation/linux-qt-contract/Contract.csproj",
                                    "--", build / "libproduct-driver.so"]):
            return 1
        if args.product:
            env = dict(os.environ, QT_QPA_PLATFORM=args.qpa)
            if not run("product", [sys.executable, HERE / "verify-product.py", "--qpa", args.qpa,
                                   "--app", args.app.resolve(), "--driver",
                                   build / "libproduct-driver.so", "--output", output / "product"], env):
                return 1
        else:
            skip("product", "Pass --product with a matching app and QPA")
    else:
        for name in ("native-contract", "webview-contract", "gpu-contract", "product"):
            skip(name, "Pass --shim from the build under test")
    for name in ("physical-input", "orca", "clean-vm-deployment"):
        skip(name, "Requires a recorded manual device or clean VM run")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
