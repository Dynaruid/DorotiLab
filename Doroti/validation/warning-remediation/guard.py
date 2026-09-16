"""Reject reintroduced framework suppressions and weakened compiler settings."""
from __future__ import annotations

import argparse
import json
import re
import xml.etree.ElementTree as ET
from pathlib import Path

EXPECTED_NOWARN = {"CS0219", "CS8524", "CS8846"}


def check(root: Path, baseline: dict | None = None) -> tuple[list[str], int]:
    errors = []
    remaining = 0
    source_root = root / "Doroti/src"
    sources = (source for module in source_root.glob("Doroti.Framework.*")
               for source in module.rglob("*.cs")
               if not {"bin", "obj"}.intersection(source.relative_to(module).parts))
    for source in sources:
        text = source.read_text(encoding="utf-8-sig")
        relative = source.relative_to(root).as_posix()
        if re.search(r"^\s*#nullable\s+disable\b", text, re.M):
            errors.append(f"{relative}: nullable analysis disabled")
        directives = re.findall(r"^\s*#pragma\s+warning\s+disable([^\n]*)", text, re.M)
        if directives:
            remaining += 1
            allowed = set((baseline or {}).get("suppressions", {}).get(relative, []))
            codes = {c for directive in directives for c in re.findall(r"CS\d+", directive)}
            if baseline is None or not codes or not codes <= allowed:
                errors.append(f"{relative}: warning suppression remains or was expanded: {sorted(codes)}")
    configs = [root / "Doroti/Directory.Build.props", root / "Doroti/Directory.Build.targets"]
    configs += list(source_root.glob("Doroti.Framework.*/*.csproj"))
    for config in configs:
        if not config.exists():
            continue
        for node in ET.parse(config).getroot().iter():
            value = (node.text or "").strip()
            if node.tag == "Nullable" and value != "enable":
                errors.append(f"{config}: Nullable must remain enable")
            if node.tag == "TreatWarningsAsErrors" and value.lower() != "true":
                errors.append(f"{config}: TreatWarningsAsErrors must remain true")
            if node.tag == "NoWarn":
                codes = set(re.findall(r"CS\d+", value))
                allowed = EXPECTED_NOWARN if config.parent.name in {"Doroti.Framework.Material", "Doroti.Framework.Cupertino"} else set()
                remainder = re.sub(r"CS\d+|\$\(NoWarn\)|[;,\s]", "", value)
                if not codes <= allowed or remainder:
                    errors.append(f"{config}: unsupported NoWarn: {value}")
    for config in [root / ".editorconfig", root / "Doroti/.editorconfig"]:
        if config.exists() and re.search(r"dotnet_diagnostic\.CS\d+\.severity\s*=\s*(none|silent)", config.read_text(), re.I):
            errors.append(f"{config}: compiler diagnostics suppressed in editor configuration")
    return errors, remaining


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[3])
    parser.add_argument("--baseline", type=Path, help="Progress check only: allow the captured residual pragmas, without expansion.")
    args = parser.parse_args()
    baseline = json.loads(args.baseline.read_text(encoding="utf-8-sig")) if args.baseline else None
    errors, remaining = check(args.root.resolve(), baseline)
    for error in errors:
        print(error)
    if not errors:
        print(f"Warning guard PASS; remaining pragma files={remaining}." + (" Baseline comparison only; not a completion gate." if baseline else ""))
    return 1 if errors else 0


if __name__ == "__main__":
    raise SystemExit(main())
