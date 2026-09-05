"""Summarize stationary fixture work; never native latency or physical evidence."""
import json
import statistics
import sys
sys.dont_write_bytecode = True
import importlib.util
from pathlib import Path

spec = importlib.util.spec_from_file_location("resize_phases", Path(__file__).with_name("analyze-resize-phases.py"))
phases = importlib.util.module_from_spec(spec)
spec.loader.exec_module(phases)


def summary(values):
    return {"count": len(values), "median": statistics.median(values) if values else None,
            "min": min(values) if values else None, "max": max(values) if values else None}


def analyze(path):
    data = json.loads(Path(path).read_text(encoding="utf-8-sig"))
    result = {"path": str(path), "source": data["source"], "variants": []}
    for sample in data["samples"]:
        durations = []
        counts = []
        for step in sample["steps"][1:]:
            a = step["before"]["presenter"]["uiDiagnostics"]["frameTimings"]
            b = step["after"]["presenter"]["uiDiagnostics"]["frameTimings"]
            count = b["count"] - a["count"]
            if count:
                durations.append((b["dispatchTotalMilliseconds"] - a["dispatchTotalMilliseconds"])/count)
                counts.append(count)
        stages = sample.get("stages")
        ui = stages["ui"] if stages else {}
        applies = [e for e in ui.get("entries", []) if e["stage"] == "ui-resize-applied"]
        work = ui.get("framework", {}).get("trace", {}).get("work", {})
        work_samples = work.get("samples", [])
        totals = dict(zip(work.get("names", []), work_samples[-1]["totals"])) if work_samples else {}
        frame_phases = None
        if applies:
            # The initial page frame precedes the first changed viewport apply.
            # End at the last collected callback, including stationary settle.
            frame_phases = phases.analyze({"stages": stages, "corpus": "stationary-only",
                "manifest": {"source": data["source"]}, "stimulus": {
                    "motionStartEpochMilliseconds": applies[0]["time"],
                    "motionEndEpochMilliseconds": max(e["time"] for e in ui["entries"])}})
        result["variants"].append({"trace": sample["trace"], "stationaryFrameMeanMilliseconds": summary(durations),
                                   "frameCountsPerTransition": counts, "workCumulativeIncludingStartup": totals,
                                   "framePhasesAfterFirstResize": frame_phases,
                                   "resizeAppliesIncludingStartup": applies,
                                   "uiDropped": ui.get("dropped"), "workDropped": work.get("dropped")})
    result["limitation"] = "Stationary transitions with browser polling; per-transition mean is not individual frame p95 or native drag feel. Startup excluded only from timing means."
    return result


if __name__ == '__main__':
    result = analyze(sys.argv[1])
    Path(sys.argv[2]).write_text(json.dumps(result, indent=2), encoding="utf-8")
    print(json.dumps([{k:v for k,v in item.items() if k not in ['resizeAppliesIncludingStartup', 'workCumulativeIncludingStartup', 'framePhasesAfterFirstResize']}
                      for item in result["variants"]], indent=2))
