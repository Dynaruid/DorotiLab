from __future__ import annotations

from pathlib import Path


ROOT = Path(__file__).resolve().parents[3]
SOURCE_ROOTS = (
    ROOT / "Doroti/src",
    ROOT / "Doroti/validation",
    ROOT / "tools/Doroti.DartToCSharp/src",
    ROOT / "tools/Doroti.DartToCSharp/validation",
)

REMOVED_MESSAGES = (
    "Dart null assertion failed.",
    "Dart control flow completed without a value.",
    "Dart closure completed without a value.",
    "Non-exhaustive Dart switch value.",
    "Dart abstract constructors cannot be invoked directly.",
    "Dart getter contract has no base implementation.",
    "Dart setter contract has no base implementation.",
    "A transpiled Dart assert failed.",
    "Dart type error.",
    "Dart threw null.",
    "Dart index access targeted null.",
    "Dart JSON object keys must be strings.",
    "Dart length was read from null.",
    "Dart Map.fromIterables requires equal key and value counts.",
    "Dart covariant queue value ",
    "requested Dart invocation",
    "is not Dart-indexable.",
    "is not a Dart JSON value.",
    "has no Dart length contract.",
    "Dart Future.error",
    "Dart unawaited",
)

REQUIRED_GENERATED_MESSAGES = (
    "A required value was null.",
    "Control flow completed without returning a value.",
    "Callback completed without returning a value.",
    "Switch expression did not handle the supplied value.",
)


def source_files():
    for source_root in SOURCE_ROOTS:
        for path in source_root.rglob("*.cs"):
            if "bin" not in path.parts and "obj" not in path.parts:
                yield path


def main() -> None:
    files = list(source_files())
    violations: list[str] = []
    for path in files:
        text = path.read_text(encoding="utf-8")
        for message in REMOVED_MESSAGES:
            if message in text:
                violations.append(f"{path.relative_to(ROOT)}: {message}")
    converter = "\n".join(
        path.read_text(encoding="utf-8")
        for path in (ROOT / "tools/Doroti.DartToCSharp/src").rglob("*.cs")
        if "bin" not in path.parts and "obj" not in path.parts
    )
    for message in REQUIRED_GENERATED_MESSAGES:
        if message not in converter:
            violations.append(f"converter template missing: {message}")
    if violations:
        raise SystemExit("\n".join(violations))
    print(
        f"Error-message terminology: PASS ({len(files)} source files; removed Dart-centric messages absent)"
    )


if __name__ == "__main__":
    main()
