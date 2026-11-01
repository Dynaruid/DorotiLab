"""Summarize complete UP-to-scrollEnd CPU/submission windows, excluding idle.

Per-stage exports preserve earlier gestures that a bounded trace would evict.
These diagnostics do not establish displayed FPS or physical input acceptance.
"""
import argparse
from collections import Counter
import importlib.util
import json
from pathlib import Path

spec = importlib.util.spec_from_file_location(
    "measure_scroll", Path(__file__).with_name("measure-android-resume-scroll.py")
)
measure = importlib.util.module_from_spec(spec)
spec.loader.exec_module(measure)


def moving_window(data, up, tail_seconds):
    trace = data["trace"]
    if not trace or min(t["recordedAtMicroseconds"] for t in trace) > up["atUs"]:
        return None
    limit = up["atUs"] + tail_seconds * 1e6
    if any(i["action"] == 0 and up["atUs"] < i["atUs"] <= limit for i in data["inputs"]):
        return None
    updates = [t for t in trace if t["phase"] == 22
               and t["scrollActivity"] == "BallisticScrollActivity"
               and up["atUs"] < t["recordedAtMicroseconds"] < limit]
    if not updates:
        return None
    position = Counter(t["scrollPositionId"] for t in updates).most_common(1)[0][0]
    updates = [t for t in updates if t["scrollPositionId"] == position]
    ends = [t for t in trace if t["phase"] == 23 and t["scrollPositionId"] == position
            and updates[-1]["recordedAtMicroseconds"] <= t["recordedAtMicroseconds"] < limit]
    if not ends:
        return None
    stop = ends[0]["recordedAtMicroseconds"]
    end = next((f["atUs"] for f in data["frames"] if f["atUs"] >= stop and f["submitted"]), None)
    if end is None:
        return None
    frames = [f for f in data["frames"] if up["atUs"] < f["atUs"] <= end]
    submitted = [f for f in frames if f["submitted"]]
    gaps = [(b["vsyncNanos"] - a["vsyncNanos"]) / 1e6
            for a, b in zip(submitted, submitted[1:])]
    return {
        "window": "UP through the first submission completing the recorded scrollEnd frame",
        "durationMs": round((end - up["atUs"]) / 1000, 3),
        "submitted": len(submitted),
        "ownerMs": measure.stats([f["ownerMs"] for f in frames]),
        "callbackMs": measure.stats([f["callbackMs"] for f in frames]),
        "allocated": measure.stats([f["allocated"] for f in frames]),
        "gc": {g: frames[-1][g] - frames[0][g] for g in ("gen0", "gen1", "gen2")},
        "nativeGapMs": measure.stats(gaps),
        "nativeGapsOver12ms": sum(g > 12 for g in gaps),
        "semanticsBuilds": sum(t["phase"] == 27 and up["atUs"] < t["recordedAtMicroseconds"] <= end
                              for t in trace),
        "offsetStart": updates[0]["scrollOffset"],
        "offsetEnd": updates[-1]["scrollOffset"],
        "extentMax": updates[-1]["scrollMaxExtent"],
        "updates": len(updates),
    }


def write_json(path, value):
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(value, indent=2) + "\n", encoding="utf-8")


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("manifests", type=Path, nargs="+")
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--outliers-output", type=Path)
    parser.add_argument("--outlier-ms", type=float, default=50)
    args = parser.parse_args()
    reports, outliers = [], []
    for path in args.manifests:
        manifest = json.loads(path.read_text(encoding="utf-8"))
        label = manifest["label"]
        tail_seconds = manifest.get("flingTailSeconds", 3)
        for trial in manifest["trials"]:
            default_profile = label + "-" + str(trial["trial"]) + ".json"
            for stage in trial["stages"]:
                profile = path.parent / stage.get("profile", default_profile)
                data = json.loads(profile.read_text(encoding="utf-8"))
                down = next(i for i in data["inputs"] if i["action"] == 0
                            and abs(i["eventMs"] - stage["downMs"]) < 5)
                gesture = down["gesture"]
                up = next(i for i in data["inputs"] if i["gesture"] == gesture and i["action"] == 1)
                identity = {"label": label, "trial": trial["trial"],
                            "stage": stage["name"], "gesture": gesture}
                window = moving_window(data, up, tail_seconds)
                if window is not None:
                    report = {**identity, **window}
                    reports.append(report)
                    print(label, trial["trial"], stage["name"], "p95", window["ownerMs"]["p95"],
                          "max", window["ownerMs"]["max"], "GC", window["gc"]["gen0"],
                          "semantics", window["semanticsBuilds"])
                if args.outliers_output:
                    for frame in data["frames"]:
                        if not (up["atUs"] < frame["atUs"] <= up["atUs"] + tail_seconds * 1e6
                                and frame["ownerMs"] >= args.outlier_ms):
                            continue
                        start = frame["atUs"] - frame["ownerMs"] * 1000
                        phases = [t for t in data["trace"]
                                  if start <= t["recordedAtMicroseconds"] <= frame["atUs"]]
                        outliers.append({**identity, "profile": str(profile), "frame": frame,
                                         "afterUpMs": round((frame["atUs"] - up["atUs"]) / 1000, 3),
                                         "completeMovingWindow": window is not None, "phases": phases})
    write_json(args.output, reports)
    if args.outliers_output:
        write_json(args.outliers_output, outliers)


if __name__ == "__main__":
    main()
