"""Import the initial widget ARBs from the pinned Flutter baseline.

The input directory must contain the exact files and hashes in widget-provenance.json.
Only the selected messages are written. Widget formatting policy lives in C#.
"""
import argparse
import hashlib
import json
import re
from pathlib import Path


ROOT = Path(__file__).resolve().parents[2]
LOCALIZATION_DOCS = ROOT / "Doroti/docs/localization"
STANDALONE_POLICIES = {
    "Widgets": {"textDirection"},
    "Cupertino": {"anteMeridiemAbbreviation", "postMeridiemAbbreviation", "timerPickerHourLabels", "timerPickerMinuteLabels",
                  "timerPickerSecondLabels"},
    "Material": {"narrowWeekdays", "firstDayOfWeekIndex", "anteMeridiemAbbreviation",
                 "postMeridiemAbbreviation", "dateSeparator"},
}
STRING_PARAMETERS = {"applicationName", "modalRouteContentName", "fullDate", "year", "day", "weekday"}
CATEGORIES = ("zero", "one", "two", "few", "many", "other")
ADDITIONAL_TAGS = {
    "en": ("en-US",), "ko": ("ko-KR",), "zh": ("zh-Hans",),
    "zh-HK": ("zh-Hant-HK",), "zh-TW": ("zh-Hant", "zh-Hant-TW"),
}
RTL_WIDGET_LANGUAGES = {"ar", "fa", "he", "ps", "ur", "ug"}


def names_in(message):
    # The pinned Flutter input interpolates `$name` directly, including adjacent Hangul.
    return re.findall(r"\$([A-Za-z][A-Za-z0-9]*)", message)


def message_format(message):
    return re.sub(r"\$([A-Za-z][A-Za-z0-9]*)", r"{\1}", message)


def plural(upstream, key, argument, exact_zero=False):
    cases = []
    for selector in CATEGORIES:
        text = upstream.get(key + selector.title())
        if text is not None:
            label = "=0" if exact_zero and selector == "zero" else selector
            cases.append(f"{label} {{{message_format(text)}}}")
    return "{" + argument + ", plural, " + " ".join(cases) + "}", [argument]


def converted(design, locale, members, upstream):
    language = locale.split("-")[0]
    upstream = dict(upstream)
    if design == "Material" and language == "ps":
        upstream["scrimOnTapHint"] = "$modalRouteContentName وتړئ"
    if design == "Material" and language == "ug":
        for key in ("dateRangeStartDateSemanticLabel", "dateRangeEndDateSemanticLabel"):
            upstream[key] = upstream[key].replace("fullDate$", "$fullDate")
    result = {"@@locale": locale}
    for name in (member["name"] for member in members):
        if name in STANDALONE_POLICIES[design] or name.startswith(("format", "parse")):
            continue
        if name in upstream:
            result[name] = (message_format(upstream[name]), names_in(upstream[name]))

    if design == "Widgets":
        result["textDirection"] = ("rtl" if language in RTL_WIDGET_LANGUAGES else "ltr", [])
    if design == "Material":
        for name, argument, korean_zero in (
            ("licensesPackageDetailText", "licenseCount", "라이선스 없음"),
            ("selectedRowCountTitle", "selectedRowCount", "선택한 항목 없음"),
            ("remainingTextFieldCharacterCount", "remainingCount", "남은 문자 없음"),
        ):
            if language == "ko":
                upstream[name + "Zero"] = korean_zero
            elif language != "en" and upstream.get(name + "Zero") in {
                "No licenses", "No items selected", "No characters remaining"
            }:
                del upstream[name + "Zero"]
            result[name] = plural(upstream, name, argument, exact_zero=True)
        approximate = upstream["pageRowsInfoTitleApproximate"]
        result["pageRowsInfoTitleApproximate"] = (message_format(approximate), names_in(approximate))
        if language == "ko":
            result["dateHelpText"] = ("yyyy. mm. dd.", [])
    if design == "Cupertino":
        if language == "eu":
            upstream["datePickerHourSemanticsLabelOne"] = "Ordu$hour\u0000a da"
            upstream["datePickerHourSemanticsLabelOther"] = "$hour\u0000ak dira"
        for name, argument in (
            ("datePickerHourSemanticsLabel", "hour"), ("datePickerMinuteSemanticsLabel", "minute"),
            ("timerPickerHourLabel", "hour"), ("timerPickerMinuteLabel", "minute"),
            ("timerPickerSecondLabel", "second"),
        ):
            text, arguments = plural(upstream, name, argument)
            result[name] = (text.replace("\u0000", ""), arguments)
            if name.startswith("timerPicker"):
                variants = [upstream[name + selector.title()] for selector in CATEGORIES
                            if name + selector.title() in upstream]
                result[name + "s"] = ("\n".join(dict.fromkeys(variants)), [])
        result["datePickerYearLabel"] = ("{year}년" if language == "ko" else "{year}", ["year"])
        result["datePickerDayLabel"] = ("{day}일" if language == "ko" else "{day}", ["day"])
        result["datePickerDayWithWeekday"] = (
            "{day}일 ({weekday})" if language == "ko" else "{weekday} {day}", ["day", "weekday"])
    return result


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("source", type=Path)
    parser.add_argument("--check", action="store_true")
    args = parser.parse_args()
    provenance = json.loads((LOCALIZATION_DOCS / "widget-provenance.json").read_text(encoding="utf-8"))
    inventory = json.loads((LOCALIZATION_DOCS / "widget-inventory.json").read_text(encoding="utf-8"))
    sources = {}
    for filename, expected_hash in provenance["inputs"].items():
        data = (args.source / filename).read_bytes()
        if hashlib.sha256(data).hexdigest() != expected_hash:
            parser.error(f"Unexpected SHA-256 for {filename}.")
        sources[filename] = json.loads(data.decode("utf-8-sig"))

    for design, inventory_section in inventory["sections"].items():
        expected = {}
        prefix = design.lower() + "_"
        locales = {}
        for filename, upstream in sources.items():
            if filename.startswith(prefix):
                locale = filename[len(prefix):-4].replace("_", "-")
                for tag in (locale, *ADDITIONAL_TAGS.get(locale, ())):
                    locales[tag] = upstream
        for locale in sorted(locales, key=lambda tag: (tag != "en", tag)):
            language = locale.split("-")[0]
            upstream = {**locales[language], **locales[locale]}
            converted_messages = converted(design, locale, inventory_section["members"], upstream)
            messages = {"@@locale": converted_messages.pop("@@locale")}
            for key, (text, arguments) in converted_messages.items():
                messages[key] = text
                names = set(arguments)
                if locale == "en":
                    expected[key] = names
                    if names:
                        placeholders = {}
                        for argument in dict.fromkeys(arguments):
                            if argument in STRING_PARAMETERS:
                                placeholders[argument] = {"type": "String"}
                            else:
                                placeholders[argument] = {"type": "long", "format": "decimalPattern"}
                        messages["@" + key] = {"placeholders": placeholders}
                elif expected.get(key) != names:
                    parser.error(f"Placeholder mismatch for {design}/{locale}/{key}.")
            if len(converted_messages) != len(expected):
                parser.error(f"Message set is incomplete for {design}/{locale}.")
            name = f"{prefix}{locale.replace('-', '_')}.arb"
            output = ROOT / f"packages/Doroti.Localizations.{design}/l10n" / name
            data = json.dumps(messages, ensure_ascii=False, indent=2) + "\n"
            if args.check:
                if output.read_text(encoding="utf-8") != data:
                    parser.error(f"Stale translated resources: {output.relative_to(ROOT)}.")
            else:
                output.parent.mkdir(parents=True, exist_ok=True)
                output.write_text(data, encoding="utf-8", newline="\n")
        print(f"{design}: {len(locales)} locale tags and {len(expected)} messages per tag")


if __name__ == "__main__":
    main()
