"""Compile pinned CLDR cardinal conditions into Doroti's decimal evaluator."""
import argparse
import hashlib
import json
import re
import xml.etree.ElementTree as ET
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
OUTPUT = ROOT / "src/Doroti.Intl/CardinalPluralRules.cs"
PROVENANCE = ROOT / "src/Doroti.Intl/data/plural-provenance.json"
RELATION = re.compile(r"([nivftec])(?:\s*%\s*(\d+))?\s*(!?=)\s*([0-9.,]+)")


def relation(text):
    match = RELATION.fullmatch(text.strip())
    if not match:
        raise ValueError(f"Unknown cardinal relation: {text!r}")
    operand, modulo, comparator, terms = match.groups()
    ranges = [tuple(int(x) for x in term.split("..")) for term in terms.split(",")]
    if operand in "ce":
        # A plain decimal has no compact exponent.
        matches = any(span[0] <= 0 <= span[-1] for span in ranges)
        return "true" if matches == (comparator == "=") else "false"
    value = f"o.{operand.upper()}" + (f" % {modulo}" if modulo else "")
    patterns = [str(span[0]) if len(span) == 1 else f">= {span[0]} and <= {span[1]}" for span in ranges]
    check = f"{value} is " + " or ".join(patterns)
    if operand == "n" and any(len(span) > 1 for span in ranges):
        check = f"o.NIsInteger && {check}"
    if comparator == "!=":
        return f"!({check})"
    return f"({check})"


def condition(text):
    alternatives = []
    for segment in text.split(" or "):
        terms = [relation(term) for term in segment.split(" and ")]
        if "false" in terms:
            continue
        terms = [term for term in terms if term != "true"]
        if not terms:
            return "true"
        alternatives.append(" && ".join(terms))
    return "\n                || ".join(f"({alternative})" for alternative in alternatives) or "false"


def generate(xml):
    cardinal = ET.fromstring(xml).find(".//plurals[@type='cardinal']")
    if cardinal is None:
        raise ValueError("No CLDR cardinal rules.")
    groups = []
    for group in cardinal.findall("pluralRules"):
        locales = sorted(group.attrib["locales"].split())
        rules = []
        for rule in group:
            category = rule.attrib["count"]
            formula = rule.text.split("@", 1)[0].strip()
            if category != "other":
                rules.append((category.title(), condition(formula)))
        groups.append((locales, rules))

    lines = [
        "// Generated from the pinned CLDR cardinal data by Doroti/eng/generate-cardinal-plurals.py.",
        "// Copyright Unicode, Inc. SPDX-License-Identifier: Unicode-3.0.",
        "using Doroti.Runtime;", "", "namespace Doroti.Intl;", "",
        "internal static class CardinalPluralRules", "{",
        "    internal static int Group(LocaleTag locale) => locale switch", "    {",
    ]
    for index, (locales, _) in enumerate(groups):
        for locale in locales:
            if "_" in locale:
                language, suffix = locale.split("_")
                field = "Script" if len(suffix) == 4 else "Region"
                lines.append(f'        {{ Language: "{language}", {field}: "{suffix}" }} => {index},')
    lines.extend(["        _ => locale.Language switch", "        {"])
    for index, (locales, _) in enumerate(groups):
        primary = [f'"{locale}"' for locale in locales if "_" not in locale]
        if primary:
            while len(primary) > 7:
                lines.append("            " + " or ".join(primary[:7]) + " or")
                primary = primary[7:]
            lines.append("            " + " or ".join(primary) + f" => {index},")
    lines.extend(["            _ => -1,", "        },", "    };", "",
                  "    internal static PluralCategory Select(int group, CardinalOperands o) => group switch", "    {"])
    for index, (_, rules) in enumerate(groups):
        for category, test in rules:
            if test == "false":
                continue
            guard = "" if test == "true" else f" when {test}"
            lines.append(f"        {index}{guard} => PluralCategory.{category},")
    lines.extend(["        _ => PluralCategory.Other,", "    };", "}", ""])
    return "\n".join(lines)


def decimal_examples(xml):
    """Exercise every rule group with the pinned sample endpoints that fit a plain decimal."""
    groups = ET.fromstring(xml).findall(".//plurals[@type='cardinal']/pluralRules")
    for group in groups:
        locale = group.attrib["locales"].split()[0].replace("_", "-")
        for rule in group:
            category = rule.attrib["count"]
            samples = rule.text.split("@", 1)[1] if "@" in rule.text else ""
            for sample in dict.fromkeys(re.split(r"[,~\s]+", samples)):
                if re.fullmatch(r"[0-9]+(?:\.[0-9]+)?", sample):
                    yield f"{locale}\t{sample}\t{category}\n"


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("xml", type=Path)
    parser.add_argument("--check", action="store_true")
    parser.add_argument("--examples", type=Path)
    args = parser.parse_args()
    data = args.xml.read_bytes()
    expected = json.loads(PROVENANCE.read_text(encoding="utf-8"))["sourceSha256"]
    if hashlib.sha256(data).hexdigest() != expected:
        parser.error("The XML does not match the pinned CLDR SHA-256.")
    generated = generate(data)
    if args.check:
        if OUTPUT.read_text(encoding="utf-8") != generated:
            parser.error("CardinalPluralRules.cs is stale.")
    else:
        OUTPUT.write_text(generated, encoding="utf-8", newline="\n")
    if args.examples:
        args.examples.write_text("".join(decimal_examples(data)), encoding="utf-8", newline="\n")
