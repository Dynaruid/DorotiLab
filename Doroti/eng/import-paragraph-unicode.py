"""Import the pinned managed Unicode algorithms and word dictionaries for the Skia paragraph renderer."""

import argparse
import gzip
import hashlib
import io
import json
import re
import urllib.request
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
RENDERER = ROOT / "src/Doroti.Skia.Rendering"
AVALONIA = "5d9e6d1b0e87337f74f233bead580db186412357"
ICU = "ddc6b66926bfac6036144ca9902992a0d6d50af9"
DICTIONARIES = {"Thai": "thaidict", "Lao": "laodict", "Khmer": "khmerdict", "Burmese": "burmesedict"}
UCD = ("BiDi.trie.cs", "BiDiAlgorithm.cs", "BiDiClass.cs", "BiDiData.cs", "BiDiPairedBracketType.cs",
       "Codepoint.cs", "CodepointEnumerator.cs", "EastAsianWidth.trie.cs", "EastAsianWidthClass.cs",
       "GeneralCategory.cs", "Grapheme.cs", "GraphemeBreak.cs", "Segmentation.trie.cs", "GraphemeBreakClass.cs",
       "GraphemeEnumerator.cs", "IndicConjunctBreakClass.cs", "LineBreak.cs", "LineBreakClass.cs",
       "LineBreakEnumerator.cs", "Script.cs", "ScriptExtensions.data.cs", "SentenceBreakClass.cs",
       "UnicodeData.cs", "UnicodeData.Lookups.cs",
       "UnicodeData.trie.cs", "UnicodeTrie.cs", "UnicodeTrieBuilder.Constants.cs", "Utf16Utils.cs",
       "WordBreakClass.cs", "WordBreakEnumerator.cs", "WordSegment.cs")
BASE = "src/Avalonia.Base"
SOURCES = [f"{BASE}/Media/TextFormatting/Unicode/{file}" for file in UCD]
SOURCES += [f"{BASE}/Utilities/{file}.cs" for file in ("ArrayBuilder", "ArraySlice", "BidiDictionary",
                                                     "ImmutableReadOnlyListStructEnumerator", "MappedArraySlice")]
SOURCES.append(f"{BASE}/Media/TextFormatting/FormattingBufferHelper.cs")


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("source", type=Path, help="cache directory for the pinned input files")
    parser.add_argument("--fetch", action="store_true", help="download missing inputs")
    parser.add_argument("--check", action="store_true", help="check tracked outputs")
    args = parser.parse_args()
    manifest = {"unicode": "17.0.0", "avaloniaRevision": AVALONIA, "icuRevision": ICU, "sources": []}

    def source(repo, revision, path):
        url = f"https://raw.githubusercontent.com/{repo}/{revision}/{path}"
        cached = args.source / repo / revision / path
        if args.fetch and not cached.is_file():
            cached.parent.mkdir(parents=True, exist_ok=True)
            with urllib.request.urlopen(url, timeout=30) as response:
                cached.write_bytes(response.read())
        raw = cached.read_bytes()
        manifest["sources"].append({"url": url, "sha256": hashlib.sha256(raw).hexdigest()})
        return raw

    def output(path, content):
        if args.check:
            actual = path.read_bytes()
            if path.suffix != ".gz":
                actual = actual.replace(b"\r\n", b"\n")
            if actual != content:
                raise SystemExit(f"Out of date: {path.relative_to(ROOT)}")
        else:
            path.parent.mkdir(parents=True, exist_ok=True)
            if not path.exists() or path.read_bytes() != content:
                path.write_bytes(content)

    for path in SOURCES:
        raw = source("AvaloniaUI/Avalonia", AVALONIA, path)
        code = raw.decode("utf-8-sig").replace("\r\n", "\n")
        if path.endswith("/BiDiAlgorithm.cs"):
            # N0 assigns the result to the working types. BD16 discards all
            # pairs from this isolating run if the opening bracket stack fills.
            if code.count("_runOriginalClasses[i] = direction;") != 2 or code.count("goto exit;") != 1:
                raise SystemExit("Recheck the Doroti bracket fixes against the pinned Avalonia source.")
            code = code.replace("_runOriginalClasses[i] = direction;", "_runResolvedClasses[i] = direction;")
            code = code.replace("goto exit;", "_pairedBrackets.Clear();\n                        return _pairedBrackets;")
            code = code.replace("        exit:\n\n", "")
            code = "// Doroti changes: resolve bracket NSMs in N0 and discard pairs on BD16 stack overflow.\n" + code
        code = code.replace("Avalonia.Media.TextFormatting.Unicode", "Doroti.Skia.Rendering.Unicode")
        code = code.replace("Avalonia.Media.TextFormatting", "Doroti.Skia.Rendering.Unicode")
        code = code.replace("Avalonia.Utilities", "Doroti.Skia.Rendering.Unicode")
        code = code.replace("using Doroti.Skia.Rendering.Unicode;\n", "")
        code = re.sub(r"(?m)^    public (?=(?:(?:readonly|static|ref|partial) )*(?:enum|struct|class|record) )", "    internal ", code)
        block = "namespace Doroti.Skia.Rendering.Unicode\n{\n"
        if block in code:
            prefix, body = code.split(block, 1)
            body = body.rstrip()
            if not body.endswith("\n}"):
                raise SystemExit(f"Unexpected namespace body: {path}")
            lines = body[:-2].splitlines()
            code = prefix + "namespace Doroti.Skia.Rendering.Unicode;\n\n" + "\n".join(
                line[4:] if line.startswith("    ") else line for line in lines
            ) + "\n"
        code = "// Adapted from Avalonia 12.2.0 for Doroti. See paragraph-unicode-provenance.json.\n" + code
        code = "\n".join(line.rstrip(" \t") for line in code.splitlines()) + "\n"
        output(RENDERER / "Unicode" / Path(path).name, code.encode("utf-8"))

    for repo, revision, path, target in [
        ("AvaloniaUI/Avalonia", AVALONIA, "licence.md", "Avalonia-MIT.txt"),
        ("unicode-org/icu", ICU, "LICENSE", "ICU-79.1-LICENSE.txt"),
    ]:
        raw = source(repo, revision, path)
        output(ROOT / "LICENSES" / target, raw.decode("utf-8-sig").replace("\r\n", "\n").encode("utf-8"))

    manifest["dictionaries"] = []
    for language, dictionary in DICTIONARIES.items():
        raw = source("unicode-org/icu", ICU, f"icu4c/source/data/brkitr/dictionaries/{dictionary}.txt")
        lines = [line.lstrip() for line in raw.decode("utf-8-sig").splitlines() if line.strip()]
        notices = [line for line in lines if line.startswith("#")]
        output(ROOT / f"LICENSES/{language}-dictionary-NOTICE.txt", ("\n".join(notices) + "\n").encode("utf-8"))
        words = sorted({line.rstrip() for line in lines if not line.startswith("#")})
        if any(len(word.split()) != 1 for word in words):
            raise SystemExit(f"Unexpected dictionary columns in {dictionary}.txt")
        compressed = io.BytesIO()
        with gzip.GzipFile(filename="", fileobj=compressed, mode="wb", mtime=0) as stream:
            stream.write(("\n".join(words) + "\n").encode("utf-8"))
        resource = f"{dictionary}.txt.gz"
        payload = compressed.getvalue()
        output(RENDERER / "Unicode" / resource, payload)
        manifest["dictionaries"].append({"language": language, "resource": resource, "entries": len(words),
                                         "compressedBytes": len(payload), "sha256": hashlib.sha256(payload).hexdigest()})
    manifest["changes"] = [
        "Import the runtime Unicode algorithms and trie data under internal Doroti namespaces.",
        "In BidiAlgorithm, apply UAX #9 N0 to the working types and return no BD16 pairs on stack overflow.",
        "Preserve dictionary notices separately and deduplicate, sort and compress the four word lists.",
    ]
    output(ROOT / "docs/localization/paragraph-unicode-provenance.json",
           (json.dumps(manifest, indent=2) + "\n").encode("utf-8"))


if __name__ == "__main__":
    main()
