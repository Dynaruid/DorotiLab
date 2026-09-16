"""Capture the working tree and compile an isolated, unsuppressed A1 baseline."""
from __future__ import annotations

import hashlib
import json
import re
import shutil
import subprocess
import sys
from datetime import datetime, timezone
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]
PRAGMA = re.compile(r"^#pragma warning disable (CS\d+(?:,[ \t]*CS\d+)*)[ \t]*$", re.M)
DIAGNOSTIC = re.compile(
    r"^(.*?)\((\d+),(\d+)\): (warning|error) (\w+): (.*?) \[(.*?)\]$", re.M
)


def write_json(path, value):
    path.write_text(json.dumps(value, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")


def execute(command, cwd, logfile):
    wrapper = ROOT / "Doroti/validation/run-with-timeout.py"
    with logfile.open("w", encoding="utf-8") as stream:
        result = subprocess.run([sys.executable, str(wrapper), *command], cwd=cwd,
                                stdout=stream, stderr=subprocess.STDOUT)
    if result.returncode:
        raise RuntimeError(f"Command failed ({result.returncode}): {command}; see {logfile}")


def main():
    run = datetime.now(timezone.utc).strftime("%Y%m%dT%H%M%SZ")
    out = ROOT / "Doroti/artifacts/warning-remediation-a1" / run
    isolated = out / "isolated"
    isolated.mkdir(parents=True)
    tracked = subprocess.check_output(["git", "ls-files", "-z"], cwd=ROOT).decode().split("\0")
    selected = [p for p in tracked if p and (
        p.startswith(("Doroti/src/", "DorotiTestbedApp/", "Doroti/eng/"))
        or ("/" not in p or p.count("/") == 1 and p.startswith("Doroti/"))
        or p.startswith("reference/flutter_sample_app/assets/")
    )]
    # Include untracked product changes too; the baseline describes this working tree.
    selected += subprocess.check_output(
        ["git", "ls-files", "--others", "--exclude-standard", "-z", "Doroti/src", "DorotiTestbedApp"],
        cwd=ROOT).decode().split("\0")
    hashes = {}
    for relative in sorted(set(selected) - {""}):
        source = ROOT / relative
        if not source.is_file():
            continue
        dest = isolated / relative
        dest.parent.mkdir(parents=True, exist_ok=True)
        shutil.copy2(source, dest)
        hashes[relative] = hashlib.sha256(source.read_bytes()).hexdigest()
    suppressions = {}
    for source in (ROOT / "Doroti/src").glob("Doroti.Framework.*/*.cs"):
        contents = source.read_text(encoding="utf-8-sig")
        matches = list(PRAGMA.finditer(contents))
        if not matches:
            continue
        relative = source.relative_to(ROOT).as_posix()
        suppressions[relative] = sorted({code for m in matches for code in re.findall(r"CS\d+", m[1])})
        # Preserve line numbers for direct comparison to the working tree.
        (isolated / relative).write_text(PRAGMA.sub("", contents), encoding="utf-8")
    baseline = {
        "run": run, "head": subprocess.check_output(["git", "rev-parse", "HEAD"], cwd=ROOT).decode().strip(),
        "status": subprocess.check_output(["git", "status", "--short"], cwd=ROOT).decode(),
        "sdk": subprocess.check_output(["dotnet", "--version"], cwd=ROOT).decode().strip(),
        "sourceHashes": hashes, "suppressions": suppressions,
        "suppressedFiles": len(suppressions), "fileCodePairs": sum(map(len, suppressions.values())),
        "diagnosticOnlyOverride": "TreatWarningsAsErrors=false",
        "existingNoWarn": {name: (ROOT / f"Doroti/src/Doroti.Framework.{name}/Doroti.Framework.{name}.csproj").read_text()
                           for name in ("Material", "Cupertino")},
    }
    write_json(out / "baseline.json", baseline)
    (out / "working-tree.patch").write_bytes(subprocess.check_output(["git", "diff", "HEAD", "--binary"], cwd=ROOT))
    (out.parent / "latest.txt").write_text(str(out), encoding="utf-8")
    projects = ["DorotiTestbedApp/DorotiTestbedApp.csproj",
                "Doroti/src/Doroti.Framework.WidgetPreviews/Doroti.Framework.WidgetPreviews.csproj"]
    all_records = []
    commands = []
    for configuration in ("Debug", "Release"):
        unique = {}
        for index, project in enumerate(projects):
            log = out / f"diagnostic-{configuration.lower()}-{index}.log"
            command = ["dotnet", "build", project, "-c", configuration, "--nologo",
                       "--disable-build-servers", "--tl:off", "-m:1", "-p:TreatWarningsAsErrors=false"]
            commands.append({"cwd": str(isolated), "command": command, "log": log.name})
            write_json(out / "commands.json", commands)
            execute(command, isolated, log)
            for m in DIAGNOSTIC.finditer(log.read_text(encoding="utf-8-sig")):
                file, line, column, severity, code, message, owner = m.groups()
                file = Path(file).relative_to(isolated).as_posix()
                owner = Path(owner).relative_to(isolated).as_posix()
                key = (owner, configuration, file, int(line), int(column), code)
                unique[key] = dict(project=owner, configuration=configuration, file=file,
                                   line=int(line), column=int(column), severity=severity, code=code,
                                   message=message, symbol=None, category="untriaged", status="open")
        records = sorted(unique.values(), key=lambda x: (x["project"], x["file"], x["line"], x["column"], x["code"]))
        write_json(out / f"diagnostics-{configuration.lower()}.json", records)
        print(f"{configuration}: {len(records)} unique diagnostics", flush=True)
        all_records.extend(records)
    write_json(out / "warning-ledger.json", all_records)
    print(out, flush=True)


if __name__ == "__main__":
    main()
