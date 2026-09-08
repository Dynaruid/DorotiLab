"""Freeze evaluated Blazor build endpoints for revision-bound static A/B runs."""
import hashlib
import json
import pathlib
import shutil
import sys

manifest = pathlib.Path(sys.argv[1]).resolve()
target = pathlib.Path(sys.argv[2]).resolve()
target.mkdir(parents=True, exist_ok=True)
records = []
for endpoint in json.loads(manifest.read_text(encoding="utf-8-sig"))["Endpoints"]:
    if endpoint["Selectors"] or any(h["Name"].lower() == "content-encoding" for h in endpoint["ResponseHeaders"]):
        continue
    source = pathlib.Path(endpoint["AssetFile"])
    destination = (target / endpoint["Route"]).resolve()
    if not destination.is_relative_to(target):
        raise ValueError("Endpoint escapes artifact root")
    destination.parent.mkdir(parents=True, exist_ok=True)
    shutil.copyfile(source, destination)
    records.append(dict(path=endpoint["Route"], bytes=source.stat().st_size,
                        sha256=hashlib.sha256(source.read_bytes()).hexdigest()))
(target.parent / (target.name + "-manifest.json")).write_text(json.dumps(records, indent=2), encoding="utf-8")
print(f"Frozen {len(records)} endpoints at {target}")
