"""Extract the current localization contract, including inherited English defaults.

This is a bounded source inventory, not a C#/ARB parser or a runtime acceptance test.
Run with --check after changing a localization API; otherwise regenerate the tracked JSON.
"""
import argparse
import hashlib
import json
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
OUTPUT = ROOT / "Doroti/docs/localization/widget-inventory.json"
SOURCES = {
    "Widgets": ("Doroti/src/Doroti.Framework.Widgets/localizations.cs", "WidgetsLocalizations"),
    "Cupertino": ("packages/Doroti.Cupertino/src/localizations.cs", "CupertinoLocalizations"),
    "Material": ("packages/Doroti.Material/src/material_localizations.cs", "MaterialLocalizations"),
}
POLICIES = {
    "textDirection", "scriptCategory", "narrowWeekdays", "firstDayOfWeekIndex",
    "dateSeparator", "dateHelpText", "datePickerDateOrder", "datePickerDateTimeOrder",
    "timeOfDayFormat", "anteMeridiemAbbreviation", "postMeridiemAbbreviation",
}
PLURALS = {
    "licensesPackageDetailText", "selectedRowCountTitle", "remainingTextFieldCharacterCount",
    "datePickerHourSemanticsLabel", "datePickerMinuteSemanticsLabel", "timerPickerHourLabel",
    "timerPickerMinuteLabel", "timerPickerSecondLabel",
}


def inventory():
    sections = {}
    for design, (relative, type_name) in SOURCES.items():
        content = (ROOT / relative).read_text(encoding="utf-8-sig")
        start = content.index("public abstract class " + type_name)
        end = content.index("\ninternal class ", start)
        contract = content[start:end]
        members = []
        for match in re.finditer(
            r"public\s+(abstract|virtual)\s+(?!class\b)([\w<>?, ]+)\s+(\w+)\s*(\([^;{]*?\))?\s*(?:\{|=>|;)",
            contract,
        ):
            _, result_type, name, parameters = match.groups()
            if name.startswith(("format", "parse", "datePicker", "timerPicker")) and name not in POLICIES | PLURALS:
                category = "dotnet-format-with-widget-policy"
            elif name in POLICIES:
                category = "locale-calendar-hour-direction-policy"
            elif name in PLURALS:
                category = "message-with-cardinal-or-number"
            else:
                category = "translated-message-or-interpolation"
            offset = start + match.start()
            members.append({
                "name": name,
                "signature": " ".join((result_type.strip() + " " + name + (parameters or " { get; }")).split()),
                "line": content[:offset].count("\n") + 1,
                "category": category,
            })
        if not members or len({member["name"] for member in members}) != len(members):
            raise ValueError(f"Incomplete/duplicate inventory for {type_name}")
        sections[design] = {
            "source": relative,
            "sha256": hashlib.sha256(content.encode("utf-8")).hexdigest(),
            "memberCount": len(members),
            "members": members,
        }
    return json.dumps({
        "schema": 1,
        "sourceHashNormalization": "UTF-8 without BOM; CRLF and CR normalized to LF",
        "flutterApiBaseline": "56b8e1a851a594b1a154f8ea93270807dab22b9a",
        "qualification": "Current Doroti public resource contracts; not upstream translation completeness or runtime acceptance.",
        "sections": sections,
    }, ensure_ascii=False, indent=2) + "\n"


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--check", action="store_true")
    args = parser.parse_args()
    generated = inventory()
    if args.check:
        if not OUTPUT.exists() or OUTPUT.read_text(encoding="utf-8") != generated:
            parser.exit(1, "Localization inventory is stale. Run Doroti/eng/localization-inventory.py.\n")
    else:
        OUTPUT.parent.mkdir(parents=True, exist_ok=True)
        OUTPUT.write_text(generated, encoding="utf-8", newline="\n")
    for name, section in json.loads(generated)["sections"].items():
        print(f"{name}: {section['memberCount']} members")
