"""Read one diagnostic report; never combine trace-on and performance corpora.

Usage: python analyze-resize-phases.py path/to/fast-resize-0.json output.json
Managed durations use RecordedAtMicroseconds only. Scene identity joins the
managed phases to UI callbacks, avoiding an uncertain cross-clock subtraction.
"""
import json
import math
import statistics
import sys
from pathlib import Path


def stats(values):
    values = sorted(values)
    return {"count": len(values), "median": statistics.median(values) if values else None,
            "p95": values[max(0, math.ceil(len(values) * .95) - 1)] if values else None,
            "max": max(values) if values else None}


def analyze(report):
    stages = report["stages"]
    ui, raster = stages["ui"]["entries"], stages["raster"]["entries"]
    start = report["stimulus"]["motionStartEpochMilliseconds"]
    end = report["stimulus"]["motionEndEpochMilliseconds"]
    framework = stages["ui"].get("framework", {}).get("trace", {})
    entries = framework.get("entries", [])
    if not entries or not all(e.get("recordedAtMicroseconds", 0) > 0 for e in entries):
        raise ValueError("No complete actual managed phase clock; causal timestamps cannot time work")
    managed_by_scene = {}
    current = {}
    for e in entries:
        phase = e["phase"]
        if phase == "build":
            current = {}
        current[phase] = e
        if phase == "sceneSubmitted":
            managed_by_scene[e["scene"]] = current
    frames = []
    for first in ui:
        if first["stage"] != "frame-start" or not start <= first["time"] <= end:
            continue
        callback = first["detail"]["callbackId"]
        same = [e for e in ui if e["detail"].get("callbackId") == callback]
        last = next((e for e in same if e["stage"] == "frame-end"), None)
        encoded = next((e for e in same if e["stage"] == "scene-encoded"), None)
        if last is None or encoded is None:
            continue
        sequence = encoded["sequence"]
        phases = managed_by_scene.get(sequence, {})
        costs = {"uiFrame": last["time"] - first["time"],
                 "paragraphWithinFrame": last["detail"].get("paragraphMilliseconds", 0)}
        for name, a, b in [("build", "build", "layout"), ("layoutAndCompositing", "layout", "paint"),
                           ("paint", "paint", "sceneBuild"), ("sceneIncludingMapEncodeInterop", "sceneBuild", "sceneSubmitted"),
                           ("semantics", "semanticsBuild", "semanticsBuildEnd")]:
            if a in phases and b in phases:
                costs[name] = (phases[b]["recordedAtMicroseconds"] - phases[a]["recordedAtMicroseconds"]) / 1000
        for stage, name in [("canvaskit-map", "mapping"), ("canvaskit-encode", "encoding")]:
            matching = [e for e in same if e["stage"] == stage]
            if matching:
                costs[name] = sum(e["detail"]["durationMicroseconds"] for e in matching) / 1000
        costs["interopCopy"] = sum(e["detail"]["milliseconds"] for e in same if e["stage"] == "interop-copy")
        costs["validationAndUiCopy"] = encoded["detail"]["validateAndCopyMilliseconds"]
        if "sceneIncludingMapEncodeInterop" in costs:
            costs["sceneResidual"] = costs["sceneIncludingMapEncodeInterop"] - sum(costs.get(k, 0) for k in ["mapping", "encoding", "interopCopy", "validationAndUiCopy"])
        lookup = {e["stage"]: e for e in ui + raster if e["sequence"] == sequence}
        for name, a, b in [("encodedToSend", "scene-encoded", "scene-send"),
                           ("sendToRaster", "scene-send", "raster-scene-received"),
                           ("replayToSubmit", "raster-start", "gpu-submit"),
                           ("terminalToUi", "raster-terminal-sent", "ui-terminal-received")]:
            if a in lookup and b in lookup:
                costs[name] = lookup[b]["time"] - lookup[a]["time"]
        frames.append({"callback": callback, "scene": sequence, "generation": encoded["generation"],
                       "milliseconds": costs, "details": [e for e in same if e["stage"] in
                           ["canvaskit-encoding-cache", "canvaskit-picture-count", "canvaskit-mapped-command-count"]]})
    keys = sorted({key for f in frames for key in f["milliseconds"]})
    return {"schema": "doroti.resize-phases/v1", "corpus": report["corpus"],
            "source": report["manifest"]["source"], "frames": frames,
            "milliseconds": {key: stats([f["milliseconds"][key] for f in frames if key in f["milliseconds"]]) for key in keys},
            "limitations": ["Only callbacks beginning during native motion; single diagnostic trial",
                            "Paragraph is included in layout/paint; sceneResidual is a subtraction, not a new phase clock",
                            "Cross-worker times have browser clock precision limits; delayed UI ACK is not transport time",
                            "Missing semantics means deferred/not measured, not zero; no physical scan-out evidence"]}


if __name__ == "__main__":
    result = analyze(json.loads(Path(sys.argv[1]).read_text(encoding="utf-8-sig")))
    Path(sys.argv[2]).write_text(json.dumps(result, indent=2), encoding="utf-8")
    print(json.dumps(result["milliseconds"], indent=2))
