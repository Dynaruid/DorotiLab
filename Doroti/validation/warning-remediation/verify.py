"""Run the A1 compile/contract exit gates with a 20-minute deadline per command."""
from __future__ import annotations

import argparse
import hashlib
import json
import subprocess
import sys
import time
from pathlib import Path
from xml.sax.saxutils import quoteattr

ROOT = Path(__file__).resolve().parents[3]
HERE = Path(__file__).resolve().parent
WRAPPER = ROOT / "Doroti/validation/run-with-timeout.py"


def source_hashes():
    paths = subprocess.check_output(
        ["git", "ls-files", "--cached", "--others", "--exclude-standard", "-z"], cwd=ROOT
    ).decode().split("\0")
    return {
        name: hashlib.sha256((ROOT / name).read_bytes()).hexdigest()
        for name in sorted(set(paths))
        if name and (ROOT / name).is_file()
        and (Path(name).suffix in {".cs", ".csproj", ".props", ".targets", ".dart", ".py", ".mjs", ".ts"}
             or Path(name).name == ".editorconfig")
    }


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--run", type=Path, required=True)
    parser.add_argument("--windows", action="store_true", help="Also build and exercise the Windows product with OS input.")
    args = parser.parse_args()
    out = args.run.resolve()
    out.mkdir(parents=True, exist_ok=True)
    projects = json.loads((HERE / "validated-projects.json").read_text())
    solution = out / "validated.slnx"
    solution.write_text("<Solution>\n" + "".join(
        f"  <Project Path={quoteattr(str(ROOT / p))} />\n" for p in projects
    ) + "</Solution>\n")
    before = source_hashes()
    (out / "validated-source-hashes.json").write_text(json.dumps(before, indent=2))
    results = []

    def step(name, command):
        print(f"START {name}", flush=True)
        started = time.monotonic()
        with (out / f"{name}.log").open("w", encoding="utf-8") as log:
            result = subprocess.run([sys.executable, str(WRAPPER), *map(str, command)],
                                    cwd=ROOT, stdout=log, stderr=subprocess.STDOUT)
        row = dict(name=name, command=list(map(str, command)), exitCode=result.returncode,
                   seconds=round(time.monotonic() - started, 2), log=f"{name}.log")
        results.append(row)
        (out / "verification.json").write_text(json.dumps(results, indent=2))
        print(f"END {name}: exit={result.returncode}, seconds={row['seconds']}", flush=True)
        if result.returncode:
            print((out / f"{name}.log").read_text(encoding="utf-8")[-6000:], flush=True)
            raise SystemExit(result.returncode)

    build_args = ["--no-restore", "--nologo", "--disable-build-servers", "--tl:off", "-m:1"]
    step("guard", [sys.executable, HERE / "guard.py"])
    step("guard-tests", [sys.executable, HERE / "test_guard.py"])
    step("source-tools-build", ["dotnet", "build", HERE / "SourceTools/SourceTools.csproj", *build_args])
    step("ide0002-check", ["dotnet", HERE / "SourceTools/bin/Debug/net10.0/SourceTools.dll",
                          "check-ide0002", solution, out / "ide0002-check.json"])
    for configuration in ("Debug", "Release"):
        for label, project in (
            ("testbed", "DorotiTestbedApp/DorotiTestbedApp.csproj"),
            ("previews", "Doroti/src/Doroti.Framework.WidgetPreviews/Doroti.Framework.WidgetPreviews.csproj"),
            ("contracts-build", "Doroti/validation/warning-remediation/Contracts/Contracts.csproj"),
        ):
            step(f"{label}-{configuration.lower()}", ["dotnet", "build", project, "-c", configuration, *build_args])
        step(f"contracts-{configuration.lower()}", ["dotnet", HERE / f"Contracts/bin/{configuration}/net10.0/Contracts.dll"])
    step("compiler-dispatch", ["pwsh", "-NoProfile", "-File", "tools/Doroti.DartToCSharp/validation/virtual-dispatch/validate.ps1"])
    if args.windows:
        step("windows-release", ["dotnet", "build", "DorotiTestbedApp/windowsappsdk/DorotiTestbedApp.WindowsAppSdk.csproj", "-c", "Release", *build_args])
        step("windows-product-input", [sys.executable, "Doroti/validation/platform-views/verify-windows-winui-input.py", out / "windows-winui-input"])
    step("diff-check", ["git", "-c", "core.safecrlf=false", "diff", "--check"])
    if source_hashes() != before:
        raise SystemExit("Source changed during verification. Revalidate the affected gates.")
    print("A1 automated gates passed for the recorded source hashes.", flush=True)


if __name__ == "__main__":
    main()
