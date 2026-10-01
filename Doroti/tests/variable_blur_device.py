"""Collect SampleApp2 Variable Blur evidence on a paired physical iPhone.

Build the Release app first. No frame readbacks or GPU completion waits are added.
This reports actual presentation intervals and CPU recording calls, NOT GPU time.
"""
import argparse
import json
import math
import subprocess
import time
import uuid
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
MODES = ["off", "full", "adaptive", "fast", "fixed"]


def summarize(evidence, hz):
    surface = evidence["surface"]
    intervals = surface.get("presentationIntervalsMilliseconds") or []
    if surface.get("presentedDrawables", 0) != len(intervals) + 1:
        raise ValueError("Missing or truncated presentation history; cannot select a warm window.")
    elapsed = 0
    warm = []
    for interval in intervals:
        start = elapsed
        elapsed += interval
        if start >= 5000 and elapsed <= 35000:
            warm.append(interval)
    if elapsed < 35000 or not warm:
        raise ValueError(f"Incomplete 30-second warm presentation window: {elapsed} ms")
    ordered = sorted(warm)
    def percentile(q):
        return ordered[math.ceil(q * len(ordered)) - 1]
    blur = evidence.get("variableBlur") or {}
    work = blur.get("lastFrameWork") or []
    # rasterEnd (FrameLifecycle.cs, enum value 26) stores an independently timed
    # rasterEnd-rasterStart duration in queueLatencyMicroseconds. Do not subtract
    # causally clamped phase timestamps to invent build/layout/paint durations.
    trace = evidence["frame"].get("trace") or []
    raster = sorted(x["queueLatencyMicroseconds"] / 1000 for x in trace if x["phase"] == 26)
    raster_summary = None if not raster else {
        "samples": len(raster), "p50Ms": raster[math.ceil(.5 * len(raster)) - 1],
        "p95Ms": raster[math.ceil(.95 * len(raster)) - 1],
        "p99Ms": raster[math.ceil(.99 * len(raster)) - 1],
        "scope": "retained frame trace tail; includes replay; not the 30-second presentation window",
    }
    return {
        "backend": surface["graphicsBackend"],
        "metalDevice": surface.get("metalDevice"),
        "viewportPixels": [surface["pixelWidth"], surface["pixelHeight"]],
        "dpr": surface["devicePixelRatio"],
        "configuredDisplayHz": hz,
        "presentationWindowMs": [5000, 35000],
        "intervalSamples": len(warm),
        "presentationP50Ms": percentile(.5),
        "presentationP95Ms": percentile(.95),
        "presentationP99Ms": percentile(.99),
        # A 1.5-period threshold tolerates display timestamp jitter. This is a
        # long-interval fraction, not a count of missed compositor deadlines.
        "longIntervalFraction": sum(x > 1500 / hz for x in warm) / len(warm),
        "metalAllocatedBytesAtEnd": surface.get("metalAllocatedBytes"),
        "capture": [x for x in work if x["stage"] == "backdrop-capture"],
        "captureDecisions": blur.get("captureDecisions"),
        "gaussianPassesLastFrame": sum(x["stage"] == "gaussian-pass" for x in work),
        "surfacesLastFrame": [x for x in work if x["stage"] == "scene-surface-clear"],
        "cpuStagesIncludingWarmup": blur.get("cpuStages"),
        "cpuRasterTraceTail": raster_summary,
        "gpuMilliseconds": None,
        "failedFrames": evidence["frame"]["failed"],
        "terminalBufferErrors": surface["commandBuffersErrored"],
        "omittedWorkItems": blur.get("omittedWorkItems"),
    }


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--device", required=True)
    parser.add_argument("--app", type=Path, help="Install this already-built .app before running.")
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--hz", type=float, required=True, help="Actual configured display rate, not marketing maximum.")
    parser.add_argument("--repeats", type=int, default=3)
    parser.add_argument("--modes", nargs="+", choices=MODES, default=MODES)
    parser.add_argument("--conditions", required=True, help="Power, thermal, brightness and instrumentation conditions.")
    args = parser.parse_args()
    if args.hz <= 0 or args.repeats < 1:
        parser.error("hz and repeats must be positive")
    out = args.output.resolve()
    if not out.is_relative_to(ROOT / "temp/testing") or out.exists():
        parser.error("Use a fresh directory under temp/testing")
    out.mkdir(parents=True)
    bundle = "dev.doroti.sample2"

    def run(*command):
        return subprocess.run(command, check=True, text=True, capture_output=True, timeout=120)

    run("xcrun", "devicectl", "device", "info", "details", "--device", args.device,
        "--json-output", str(out / "device.json"))
    if args.app:
        installed = run("xcrun", "devicectl", "device", "install", "app", "--device", args.device, str(args.app.resolve()))
        (out / "install.log").write_text(installed.stdout)
    summary = {"commit": run("git", "rev-parse", "HEAD").stdout.strip(),
               "dirty": bool(run("git", "status", "--porcelain").stdout),
               "conditions": args.conditions, "runs": [], "gpuTiming": "not measured"}
    (out / "working-tree.diff").write_text(run("git", "diff").stdout)
    for repeat in range(args.repeats):
        modes = args.modes if repeat % 2 == 0 else list(reversed(args.modes))
        for mode in modes:
            name = f"{repeat + 1}-{mode}"
            remote = "variable-blur-" + uuid.uuid4().hex + ".json"
            environment = {"DOROTI_IOS_GRAPHITE": "1", "DOROTI_VARIABLE_BLUR_PROFILE": "1",
                           "DOROTI_VARIABLE_BLUR_BENCHMARK": mode, "DOROTI_MAUI_EVIDENCE": remote}
            log_path = out / (name + ".log")
            print("RUN " + name, flush=True)
            with log_path.open("w") as log:
                process = subprocess.Popen(["xcrun", "devicectl", "device", "process", "launch",
                    "--device", args.device, "--terminate-existing", "--console",
                    "--environment-variables", json.dumps(environment), bundle], stdout=log, stderr=subprocess.STDOUT)
                try:
                    deadline = time.monotonic() + 100
                    while "VARIABLE_BLUR_BENCHMARK_COMPLETE" not in log_path.read_text():
                        if process.poll() is not None:
                            raise RuntimeError(f"Launch exited {process.returncode}; see {log_path}")
                        if time.monotonic() > deadline:
                            raise TimeoutError(f"Scroll did not finish; see {log_path}")
                        time.sleep(.5)
                    destination = out / (name + ".json")
                    run("xcrun", "devicectl", "device", "copy", "from", "--device", args.device,
                        "--domain-type", "appDataContainer", "--domain-identifier", bundle,
                        "--source", "Documents/" + remote, "--destination", str(destination))
                    result = summarize(json.loads(destination.read_text()), args.hz)
                    result.update(mode=mode, repeat=repeat + 1)
                    summary["runs"].append(result)
                    (out / "summary.json").write_text(json.dumps(summary, indent=2))
                    if result["failedFrames"] or result["terminalBufferErrors"]:
                        raise RuntimeError(f"Renderer errors; see {destination}")
                    if "Graphite-Metal" not in result["backend"]:
                        raise RuntimeError(f"Unexpected renderer; see {destination}")
                    if (result["gaussianPassesLastFrame"] > 0) != (mode != "off"):
                        raise RuntimeError(f"Blur mode did not render as requested; see {destination}")
                    print(f"DONE {name}: presentation p95 {result['presentationP95Ms']:.2f} ms", flush=True)
                finally:
                    if process.poll() is None:
                        process.terminate()  # devicectl forwards SIGTERM to this test process.
                        try:
                            process.wait(timeout=15)
                        except subprocess.TimeoutExpired:
                            process.kill()
                            process.wait(timeout=5)


if __name__ == "__main__":
    main()
