"""Summarize native trial reports without pooling latency samples across trials."""
import argparse
import json
import re
import statistics
from pathlib import Path

parser = argparse.ArgumentParser()
parser.add_argument("root", type=Path)
args = parser.parse_args()
groups = {}
trials = []
for path in sorted(args.root.glob("**/fast-resize-*.json")):
    if "test-results" not in path.parts or not re.fullmatch(r"fast-resize-\d+\.json", path.name):
        continue
    report = json.loads(path.read_text(encoding="utf-8-sig"))
    if report.get("schema") != "doroti.canvaskit-native-fast-resize/v2":
        continue
    label = path.relative_to(args.root).parts[0]
    match = re.fullmatch(r"(Right|Bottom|Left|TopLeft)-(150|600)-(expand|shrink|reverse)-(\d+)-(.+)", label)
    variant = match[5] if match else label
    stimulus, old, new = report["stimulus"], report["following"], report["followingV2"]
    trial = dict(path=str(path), variant=variant, edge=stimulus["edge"],
        duration=stimulus["requestedMilliseconds"], motion=stimulus["motion"],
        stimulus=stimulus["qualified"], legacy=old["status"], v2=new["status"],
        p95=old["targetToCaughtUpFrontP95"], gapMax=new["boundaryInclusiveGaps"]["max"],
        first=new["firstFrontMilliseconds"], settle=new["settleFromObserverMilliseconds"],
        source=report["manifest"]["source"]["sourceIdentitySha256"],
        build=report["manifest"]["buildKind"], corpus=report["corpus"])
    trials.append(trial)
    groups.setdefault(variant, []).append(trial)

def summary(rows):
    valid = [r for r in rows if r["stimulus"]]
    values = [r["p95"] for r in valid if r["p95"] is not None]
    return dict(trials=len(rows), stimulusPass=len(valid),
        legacyPass=sum(r["legacy"] == "PASS" for r in valid),
        v2Pass=sum(r["v2"] == "PASS" for r in valid),
        p95Trials=[r["p95"] for r in valid],
        medianTrialP95=statistics.median(values) if values else None,
        maxBoundaryGap=max((r["gapMax"] for r in valid), default=None))

result = dict(endpoint="main notification; not captured presentation or physical scan-out",
    groups={name: summary(rows) for name, rows in groups.items()},
    sourceIdentities=sorted({r["source"] for r in trials}), trials=trials)
args.root.mkdir(parents=True, exist_ok=True)
(args.root / "summary.json").write_text(json.dumps(result, indent=2), encoding="utf-8")
print(json.dumps(result["groups"], indent=2))
