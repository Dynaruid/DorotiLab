#!/usr/bin/env python3
"""Fail when the app-owned Qt shim and template diverge, including binary assets."""
from pathlib import Path
import hashlib
import sys

ROOT = Path(__file__).resolve().parents[3]
APP = ROOT / "DorotiTestbedApp/linux/native"
TEMPLATE = ROOT / "Doroti/templates/Doroti.Templates/content/doroti-app/linux/native"


def files(directory: Path) -> dict[str, str]:
    return {
        str(path.relative_to(directory)): hashlib.sha256(path.read_bytes()).hexdigest()
        for path in directory.rglob("*")
        if path.is_file()
    }


def main() -> int:
    app, template = files(APP), files(TEMPLATE)
    mismatches = sorted(path for path in app.keys() | template.keys() if app.get(path) != template.get(path))
    for path in mismatches:
        print(f"MISMATCH {path}: app={app.get(path, 'missing')} template={template.get(path, 'missing')}")
    if mismatches:
        return 1
    print(f"PASS: {len(app)} native files match the template byte for byte")
    return 0


if __name__ == "__main__":
    sys.exit(main())
