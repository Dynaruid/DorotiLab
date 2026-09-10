#!/usr/bin/env python3
"""Collect actual publish diagnostics, keeping reachability separate from source counts."""
import argparse
import hashlib
import json
from pathlib import Path
import re

parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument("log", type=Path)
parser.add_argument("--output", type=Path, required=True)
args = parser.parse_args()
diagnostics = {}
for line in args.log.read_text(errors="replace").splitlines():
    match = re.search(r"(?:AOT analysis |Trim analysis )?(?:error|warning) (IL\d+): (.+?)(?: \[/.*)?$", line)
    if not match:
        continue
    code, message = match.groups()
    location = re.sub(r"^\s*\d*>", "", line[:match.start()]).strip().rstrip(":")
    key = (code, message)
    if key in diagnostics:
        continue  # MSBuild prints diagnostics again in its summary.
    operation = ("DLR" if "RuntimeBinder" in message or "CallSite" in message else
                 "assembly-loading" if "Assembly.Load" in message else
                 "reflection" if "GetProperty" in message or "GetMethod" in message else
                 "dependency" if "Assembly '" in message or location == "ILC" else "other")
    diagnostics[key] = dict(id=hashlib.sha256((code + message).encode()).hexdigest()[:16],
        source=location, member=message.split(": Using member", 1)[0], warning=code,
        operation=operation, evidence=message, reachability="Release publish",
        debugReachability="notVerified", receiverTypeFlow="requires contract review",
        replacement="typed contract or dependency fix", fixture=None, status="open")
report = dict(schemaVersion="doroti.native-aot-blockers/v1", log=str(args.log),
              logSha256=hashlib.sha256(args.log.read_bytes()).hexdigest(),
              diagnostics=list(diagnostics.values()))
args.output.parent.mkdir(parents=True, exist_ok=True)
args.output.write_text(json.dumps(report, indent=2, ensure_ascii=False) + "\n")
print(f"{len(diagnostics)} unique diagnostics -> {args.output}")
