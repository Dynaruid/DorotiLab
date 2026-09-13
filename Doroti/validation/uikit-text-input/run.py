#!/usr/bin/env python3
"""Exercise the production text-input bridge with real UIKit handlers."""
import argparse
from pathlib import Path
import subprocess
import time


def main():
    doroti = Path(__file__).resolve().parents[2]
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--simulator", default="booted")
    parser.add_argument("--bridge-source", type=Path, help="Optional previous bridge revision for a negative control")
    parser.add_argument("--skip-build", action="store_true")
    parser.add_argument("--output", type=Path, default=doroti / "artifacts/uikit-text-input")
    args = parser.parse_args()
    output = args.output.resolve()
    output.mkdir(parents=True, exist_ok=True)
    app = "dev.doroti.validation.uikit-text-input"

    def run(label, command, required=True):
        result = subprocess.run(command, cwd=doroti, capture_output=True, text=True, timeout=180)
        (output / f"{label}.log").write_text(result.stdout + result.stderr)
        if required and result.returncode:
            raise RuntimeError(f"{label} failed: {output / (label + '.log')}")
        return result.stdout.strip()

    if not args.skip_build:
        command = ["dotnet", "build", "validation/uikit-text-input/Doroti.Validation.UIKitTextInput.csproj", "-v:q"]
        if args.bridge_source:
            command.append(f"-p:DorotiTextInputBridgeSource={args.bridge_source.resolve()}")
        run("build", command)
    bundle = doroti / "artifacts/validation/build/uikit-text-input/bin/Debug/net10.0-ios/iossimulator-arm64/Doroti.Validation.UIKitTextInput.app"
    simctl = ["xcrun", "simctl"]
    run("terminate", simctl + ["terminate", args.simulator, app], required=False)
    run("install", simctl + ["install", args.simulator, str(bundle)])
    container = Path(run("container", simctl + ["get_app_container", args.simulator, app, "data"]))
    result_file = container / "Documents/result.txt"
    result_file.unlink(missing_ok=True)
    run("launch", simctl + ["launch", args.simulator, app])
    deadline = time.monotonic() + 60
    while time.monotonic() < deadline:
        if result_file.exists():
            result = result_file.read_text()
            (output / "result.txt").write_text(result)
            print(result)
            return 0 if result.splitlines()[-1] == "PASS" else 1
        time.sleep(.2)
    raise TimeoutError("UIKit input probe did not finish within 60 seconds")


if __name__ == "__main__":
    raise SystemExit(main())
