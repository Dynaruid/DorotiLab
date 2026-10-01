"""Summarize Xcode 27 Metal XML exports for one app PID, not individual blur stages."""
import argparse
import collections
import json
import math
import xml.etree.ElementTree as ET
from pathlib import Path


def rows(path):
    root = ET.parse(path).getroot()
    references = {element.get("id"): element for element in root.iter() if element.get("id")}
    columns = [column.findtext("mnemonic") for column in root.find(".//schema").findall("col")]
    for row in root.findall(".//row"):
        yield {name: references[element.get("ref")] if element.get("ref") else element
               for name, element in zip(columns, row)}


def interval_union(intervals):
    """GPU channels overlap: measure their union rather than summing their durations."""
    ordered = sorted(intervals)
    left, right = ordered[0]
    total = 0
    for start, end in ordered[1:]:
        if start <= right:
            right = max(right, end)
        else:
            total += right - left
            left, right = start, end
    return total + right - left


def summarize(gpu, submissions, pid):
    def belongs(row):
        return row["process"].get("fmt", "").endswith(f"({pid})")

    commands = {row["cmdbuffer-id"].text: int(row["num-encoders"].text)
                for row in rows(submissions) if belongs(row)}
    groups = collections.defaultdict(list)
    for row in rows(gpu):
        identifier = row["cmdbuffer-id"].text
        if belongs(row) and commands.get(identifier, 0) > 0 and row["state"].text == "Active":
            start = int(row["start"].text)
            groups[identifier].append((start, start + int(row["duration"].text)))
    ordered = sorted(groups, key=lambda key: min(start for start, _ in groups[key]))
    # Discard boundary buffers that may straddle trace start/end. This remains a
    # short, profiler-instrumented observation, not a steady-state benchmark.
    selected = ordered[1:-1]
    if not selected:
        raise ValueError("No interior app GPU command buffers matched the submission export")

    def distribution(values):
        values = sorted(values)
        return {"samples": len(values), **{
            name: values[math.ceil(q * len(values)) - 1]
            for name, q in [("p50Ms", .5), ("p95Ms", .95), ("p99Ms", .99)]}}

    return {
        "pid": pid,
        "scope": "short profiler run; all app work per nonempty command buffer; boundary buffers excluded",
        "gpuActiveUnion": distribution([interval_union(groups[key]) / 1e6 for key in selected]),
        "gpuFirstToLastSpan": distribution([
            (max(end for _, end in groups[key]) - min(start for start, _ in groups[key])) / 1e6
            for key in selected]),
        "encoderCountDistribution": dict(collections.Counter(commands[key] for key in selected)),
        "allAppSubmissionEncoderCounts": dict(collections.Counter(commands.values())),
        "variableBlurGpuMilliseconds": None,
    }


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--gpu", type=Path, required=True)
    parser.add_argument("--submissions", type=Path, required=True)
    parser.add_argument("--pid", type=int, required=True)
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args()
    args.output.write_text(json.dumps(summarize(args.gpu, args.submissions, args.pid), indent=2) + "\n")


if __name__ == "__main__":
    main()
