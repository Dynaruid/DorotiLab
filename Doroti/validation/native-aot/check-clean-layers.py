#!/usr/bin/env python3
"""Build and reject DLR call sites in the complete converted UI framework."""
import argparse
import json
from pathlib import Path
import subprocess
import sys

HERE = Path(__file__).resolve().parent
PRODUCT = HERE.parents[1]
LAYERS = ("Foundation", "Scheduler", "Services", "Physics", "Gestures", "Animation", "Semantics", "Painting", "Rendering", "Widgets", "Cupertino", "Material")


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args()
    output = args.output.resolve()
    output.mkdir(parents=True, exist_ok=True)
    steps = []

    def run(name, command, expected=0):
        log = output / (name + ".log")
        process = subprocess.run(
            [sys.executable, str(HERE / "run.py"), "--log", str(log), "--", *map(str, command)],
            cwd=PRODUCT, timeout=1230, check=False)
        steps.append(dict(name=name, exitCode=process.returncode, expectedExitCode=expected, log=str(log)))
        if process.returncode != expected:
            raise RuntimeError(f"{name}: exit {process.returncode}, expected {expected}")
        return log

    passed = False
    try:
        project = HERE / "il-audit"
        run("audit-build", ["dotnet", "build", project, "-c", "Release", "-v:minimal", "-p:UseSharedCompilation=false"])
        audit = ["dotnet", project / "bin/Release/net10.0/Doroti.Validation.NativeAot.IlAudit.dll", "--expect-zero"]
        for configuration in ("Debug", "Release"):
            run(configuration.lower() + "-build", ["dotnet", "build", PRODUCT / "src/Doroti.Framework.Material", "-c", configuration, "-v:minimal", "-p:UseSharedCompilation=false"])
            assemblies = [PRODUCT / f"src/Doroti.Framework.{layer}/bin/{configuration}/net10.0/Doroti.Framework.{layer}.dll"
                          for layer in LAYERS]
            log = run(configuration.lower() + "-audit", audit + assemblies)
            report = json.loads(log.read_text())
            if len(report["assemblies"]) != len(LAYERS) or report["totalCallSiteFields"] != 0:
                raise RuntimeError("All complete framework assemblies must be inspected and have zero call sites")
        # A missing or empty input set must never be reported as zero call sites.
        run("missing-input", audit + [output / "does-not-exist.dll"], expected=2)
        run("empty-input", audit, expected=2)
        passed = True
    finally:
        (output / "gate-results.json").write_text(json.dumps(dict(
            schemaVersion="doroti.clean-layer-gate/v1", passed=passed, steps=steps,
            scope=list(LAYERS), iosTestbed="notVerified"), indent=2) + "\n")
    return 0 if passed else 1


if __name__ == "__main__":
    raise SystemExit(main())
