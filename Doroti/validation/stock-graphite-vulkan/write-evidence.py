"""Assemble reviewable, version-controlled evidence from immutable run directories.

Usage: python write-evidence.py <candidate-run> <baseline-run> <graph-run> <output-dir>
Paths are repository-relative or absolute. No GPU tests are rerun here.
"""
import hashlib
import json
from pathlib import Path
import sys

ROOT = Path(__file__).resolve().parents[3]


def read(path):
    return json.loads(path.read_text(encoding="utf-8-sig"))


def main():
    if len(sys.argv) != 5:
        raise SystemExit(__doc__)
    candidate, baseline, graph, output = [Path(p).resolve() for p in sys.argv[1:]]
    runs = dict(candidate=candidate, baseline=baseline, graph=graph)
    evidence = dict(schema="doroti.stock-graphite.w0-evidence/v1", status="PARTIAL",
                    head=(candidate / "W0-0/head/stdout.log").read_text().strip(),
                    runDirectories={key: str(path.relative_to(ROOT)).replace("\\", "/") for key, path in runs.items()},
                    officialAsset=read(candidate / "W0-0/official-asset.json"),
                    targets=read(candidate / "W0-0/targets.json"),
                    cachedNativeAssets=read(baseline / "W0-0/cached-native-assets.json"),
                    resolvedGraph=read(graph / "resolved-graph.json"),
                    commands={key: read(path / "commands.json") for key, path in runs.items()},
                    executedHarnessSources=read(candidate / "W0-0/harness-source-identity.json"),
                    baseline={}, candidate={}, files=[])
    for gpu in ("AMD", "NVIDIA"):
        evidence["candidate"][gpu] = {}
        for mode in ("sync", "async", "shutdown"):
            stage = "W0-4-early" if mode == "shutdown" else "W0-1"
            evidence["candidate"][gpu][mode] = read(candidate / f"{stage}/win-x64/{gpu}/{mode}/report.json")
        evidence["baseline"][gpu] = read(baseline / f"W0-0/windowsappsdk-product/{gpu}/report.json")
    evidence["baseline"]["d3d12Diagnostic"] = read(baseline / "W0-0/d3d12-diagnostic/report.json")
    for run in runs.values():
        for path in sorted(run.rglob("*")):
            if path.is_file() and path.suffix in (".log", ".json", ".png", ".rgba", ".binlog", ".xml"):
                evidence["files"].append(dict(path=str(path.relative_to(ROOT)).replace("\\", "/"),
                                              bytes=path.stat().st_size, sha256=hashlib.sha256(path.read_bytes()).hexdigest()))
    output.mkdir(parents=True, exist_ok=True)
    (output / "evidence.json").write_text(json.dumps(evidence, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    print(output / "evidence.json")


if __name__ == "__main__":
    main()
