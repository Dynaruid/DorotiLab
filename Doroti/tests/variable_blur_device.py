"""Collect SampleApp2 Variable Blur evidence on a paired physical iPhone.

Build the Release app first. No frame readbacks or GPU completion waits are added.
This reports actual presentation intervals and CPU recording calls, NOT GPU time.
"""
import argparse
import hashlib
import json
import math
import subprocess
import time
import uuid
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
MODES = ["off", "full", "adaptive", "fast", "fixed", "kawase"]


def run_order(modes, repeats, start_repeat=1):
    for repeat in range(start_repeat - 1, start_repeat - 1 + repeats):
        ordered_modes = modes if repeat % 2 == 0 else list(reversed(modes))
        for mode in ordered_modes:
            yield repeat + 1, mode, "C"


def distribution(values):
    if not values:
        return None
    ordered = sorted(values)
    return {"samples": len(values), **{
        f"p{int(q * 100)}Ms": ordered[math.ceil(q * len(ordered)) - 1] for q in (.5, .95, .99)}}


def frame_loop_summary(loop):
    if loop.get("omittedEvents", 0):
        raise ValueError("Truncated iOS frame-loop history")
    events = loop.get("events") or []
    displays = sorted((x for x in events if x["phase"] == "displayed"), key=lambda x: x["timestampMicroseconds"])
    if not displays:
        raise ValueError("Missing actual drawable display events")
    low = displays[0]["timestampMicroseconds"] + 5_000_000
    high = displays[0]["timestampMicroseconds"] + 35_000_000
    if displays[-1]["timestampMicroseconds"] < high:
        raise ValueError("Incomplete iOS frame-loop measurement window")
    warm = [x for x in events if low <= x["timestampMicroseconds"] <= high]
    by_frame = {}
    for event in events:
        by_frame.setdefault(event["frameId"], {})[event["phase"]] = event
    scenes = {x["sceneId"]: x for x in events if x["phase"] == "scene-created"}
    arrived = {x["frameId"]: x["timestampMicroseconds"] for x in events if x["phase"] == "gpu-arrived"}
    hardware_ended = {x["frameId"]: x["timestampMicroseconds"] for x in events if x["phase"] == "gpu-ended"}
    completed = hardware_ended or arrived
    submitted = [x for x in events if x["phase"] == "submitted"]
    raster = [x for x in warm if x["phase"] == "raster-start"
              and by_frame[x["frameId"]].get("submitted", {}).get("sceneId", 0) > 0
              and by_frame[x["frameId"]].get("submitted", {}).get("reason", "new") == "new"]
    overlap = sum(any(old["frameId"] != x["frameId"]
        and old["timestampMicroseconds"] <= x["timestampMicroseconds"]
        and completed.get(old["frameId"], math.inf) > x["timestampMicroseconds"]
        for old in submitted) for x in raster)
    reasons = {}
    for event in warm:
        if event["phase"] == "admission":
            reason = event.get("reason") or "unknown"
            reasons[reason] = reasons.get(reason, 0) + 1
    phases = ("framework-end", "callback", "callback-wait", "raster-end", "drawable", "flush", "snap", "insert", "submit", "terminal-commit", "wait-scheduled")
    cpu = {phase: distribution([x["durationMilliseconds"] for x in warm if x["phase"] == phase]) for phase in phases}
    latency = {}
    for label, start, end in (("frameworkToDisplay", "framework-start", "displayed"),
            ("rasterToDisplay", "raster-start", "displayed"),
            ("submitToGpuArrival", "submitted", "gpu-arrived"),
            ("gpuCallbackOwnerDelay", "gpu-arrived", "gpu-owner"),
            ("displayCallbackOwnerDelay", "displayed", "display-owner")):
        values = []
        for display in displays:
            if not low <= display["timestampMicroseconds"] <= high:
                continue
            frame = by_frame[display["frameId"]]
            scene_id = display.get("sceneId", 0)
            source = by_frame[scenes[scene_id]["frameId"]] if start == "framework-start" and scene_id in scenes else frame
            if start in source and end in frame:
                duration = (frame[end]["timestampMicroseconds"] - source[start]["timestampMicroseconds"]) / 1000
                if duration >= 0:
                    values.append(duration)
        latency[label] = distribution(values)
    callback_latency = []
    for display in displays:
        if low <= display["timestampMicroseconds"] <= high:
            created = scenes.get(display.get("sceneId", 0))
            source = by_frame[created["frameId"]] if created else by_frame[display["frameId"]]
            callback = source.get("callback")
            if callback:
                start = callback["timestampMicroseconds"] - callback["durationMilliseconds"] * 1000
                callback_latency.append((display["timestampMicroseconds"] - start) / 1000)
    latency["callbackToDisplay"] = distribution(callback_latency)
    return {"policy": loop["policy"], "runtime": loop.get("runtime"),
        "windowOrigin": "first actual drawable display", "newRasterSamples": len(raster),
        "gpuCompletionTimeline": "same-queue terminal hardware end" if hardware_ended else "completion callback arrival",
        "hardwareCompletionSamples": len(hardware_ended),
        "overlapCount": overlap, "overlapFraction": overlap / len(raster) if raster else 0,
        "maximumPending": loop["maximumPending"],
        "maximumAwaitingPresentation": loop.get("maximumAwaitingPresentation"),
        "currentAwaitingPresentation": loop.get("currentAwaitingPresentation"), "admissionReasons": reasons,
        "cpuStages": cpu, "latency": latency,
        "inputToDisplay": None, "inputTimingScope": "synthetic scroll has no physical pointer input",
        "reconciledPresentationCredits": sum(x["phase"] == "presentation-credit-reconciled" for x in warm),
        "droppedPresentationFrames": sum(x["phase"] == "presentation-dropped" for x in warm),
        "transactionPresents": sum(x["phase"] == "terminal-commit" and x.get("reason") == "transaction" for x in warm),
        "asyncPresents": sum(x["phase"] == "terminal-commit" and x.get("reason") == "async" for x in warm)}


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
        "iosFrameLoop": frame_loop_summary(surface["iosFrameLoop"]) if surface.get("iosFrameLoop") else None,
        "targetFramework": evidence.get("targetFramework"),
        "backend": surface["graphicsBackend"],
        "metalDevice": surface.get("metalDevice"),
        "viewportPixels": [surface["pixelWidth"], surface["pixelHeight"]],
        "dpr": surface["devicePixelRatio"],
        "configuredDisplayHz": hz,
        "presentationWindowMs": [5000, 35000],
        "intervalSamples": len(warm),
        "presentationMeanFps": 1000 * len(warm) / sum(warm),
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
        "kawasePassesLastFrame": sum(x["stage"] in ("kawase-down", "kawase-up") for x in work),
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
    parser.add_argument("--build-identity", type=Path, help="JSON identity recorded for this build (SDK, workload, TFM, compilation runtime).")
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--hz", type=float, required=True, help="Actual configured display rate, not marketing maximum.")
    parser.add_argument("--repeats", type=int, default=3)
    parser.add_argument("--repeat-start", type=int, default=1, help="Continue repeat numbering and counterbalanced order after a reusable same-binary run.")
    parser.add_argument("--sigma", type=float, default=20, help="Logical blur sigma, 0..32.")
    parser.add_argument("--full-capture", action="store_true", help="Disable ROI capture for a same-binary domain comparison.")
    parser.add_argument("--intermediate", action="store_true", help="Use the original Adaptive intermediate composition for A/B.")
    parser.add_argument("--owned-subtrees", action="store_true", help="Force shader-free sibling scopes through owned full-frame layers.")
    parser.add_argument("--full-stages", action="store_true", help="Reconstruct Kawase stages over their entire padded domain.")
    parser.add_argument("--full-detail", action="store_true", help="Use capture-sized surfaces for the Gaussian detail.")
    parser.add_argument("--full-bands", action="store_true", help="Use whole-domain backing for Adaptive Gaussian bands.")
    parser.add_argument("--modes", nargs="+", choices=MODES, default=MODES)
    parser.add_argument("--conditions", required=True, help="Power, thermal, brightness and instrumentation conditions.")
    args = parser.parse_args()
    if args.hz <= 0 or args.repeats < 1 or args.repeat_start < 1:
        parser.error("hz and repeats must be positive")
    if not math.isfinite(args.sigma) or not 0 <= args.sigma <= 32:
        parser.error("sigma must be finite and between 0 and 32")
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
               "fullCapture": args.full_capture,
               "intermediate": args.intermediate,
               "sigma": args.sigma,
               "ownedSubtrees": args.owned_subtrees, "fullStages": args.full_stages,
               "fullDetail": args.full_detail,
               "fullBands": args.full_bands,
               "policy": "C",
               "repeatStart": args.repeat_start,
               "dirty": bool(run("git", "status", "--porcelain").stdout),
               "conditions": args.conditions, "runs": [], "attempts": [],
               "collectorSha256": hashlib.sha256(Path(__file__).read_bytes()).hexdigest(),
               "gpuTiming": "not measured"}
    if args.build_identity:
        summary["buildIdentity"] = json.loads(args.build_identity.read_text())
    # New renderer/shader files can be untracked during an experiment. git diff
    # alone does not identify the implementation that produced a measurement.
    inputs = list((ROOT / "Doroti/src/Doroti.Skia.Rendering").glob("SkiaSceneRenderer*.cs"))
    inputs += list((ROOT / "Doroti/src/Doroti.Skia.Rendering/Shaders").glob("*.sksl"))
    inputs += [ROOT / "samples/DorotiSampleApp2/src/VariableBlurPage.cs",
               ROOT / "Doroti/src/Doroti.Ui/FrameworkShaderAssets.cs",
               ROOT / "Doroti/src/Doroti.Ui/ImageFilter.VariableBlur.cs",
               ROOT / "Doroti/src/Doroti.Framework.Rendering/image_filter_config.cs",
               ROOT / "Doroti/src/Doroti.Skia.RuntimeEffects/DorotiSkiaRuntimeEffects.cs",
               ROOT / "Doroti/src/Doroti.Skia.RuntimeEffects/DorotiSkiaImageFilterRenderer.cs"]
    inputs += [ROOT / "Doroti/src/Doroti.Host.Maui" / name for name in
               ["DorotiUIKitGraphiteViewHandler.cs", "DorotiGraphiteView.cs", "MauiSkiaSurface.cs",
                "MauiHostAdapter.cs", "MauiFrameCallbackQueue.cs", "MauiFrameWakeQueue.cs", "IosFrameLifetime.cs", "IosFrameLoopDiagnostics.cs", "MauiHostContracts.cs",
                "MauiSkiaCapabilities.cs", "MauiFrameworkHost.cs", "DorotiMauiSurface.cs"]]
    inputs += [ROOT / "Doroti/src/Doroti.Host.Maui" / name for name in
               ["AppleGpuEffects.cs", "AppleMetalPresentation.cs"]]
    inputs += list((ROOT / "Doroti/src/Doroti.Skia.Rendering").glob("SkiaGraphiteSession*.cs"))
    inputs += [ROOT / "Doroti/src/Doroti.Skia.Rendering/SkiaShaderSceneAdmission.cs"]
    inputs += [ROOT / "Doroti/src/Doroti.Skia.Rendering/NativeFramePipeline.cs"]
    summary["sourceInputsSha256"] = {str(p.relative_to(ROOT)): hashlib.sha256(p.read_bytes()).hexdigest()
                                     for p in sorted(inputs)}
    if args.app:
        payload = hashlib.sha256()
        for path in sorted(args.app.resolve().rglob("*")):
            if path.is_file():
                payload.update(str(path.relative_to(args.app.resolve())).encode())
                payload.update(hashlib.sha256(path.read_bytes()).digest())
        summary["installedAppPayloadSha256"] = payload.hexdigest()
        summary["runtimeFromPayload"] = "Mono" if list(args.app.rglob("*.dll")) else "NativeAOT"
        summary["managedAssemblyCount"] = len(list(args.app.rglob("*.dll")))
    (out / "working-tree.diff").write_text(run("git", "diff").stdout)
    order = list(run_order(args.modes, args.repeats, args.repeat_start))
    summary["runOrder"] = [{"repeat": r, "mode": m, "policy": p} for r, m, p in order]
    (out / "summary.json").write_text(json.dumps(summary, indent=2))
    for repeat, mode, policy in order:
        name = f"{repeat}-{mode}-C"
        remote = "variable-blur-" + uuid.uuid4().hex + ".json"
        environment = {"DOROTI_IOS_GRAPHITE": "1", "DOROTI_VARIABLE_BLUR_PROFILE": "1",
                       "DOROTI_VARIABLE_BLUR_DISABLE_CROP": "1" if args.full_capture else "0",
                       "DOROTI_VARIABLE_BLUR_INTERMEDIATE": "1" if args.intermediate else "0",
                       "DOROTI_VARIABLE_BLUR_OWNED_SUBTREES": "1" if args.owned_subtrees else "0",
                       "DOROTI_VARIABLE_BLUR_FULL_STAGES": "1" if args.full_stages else "0",
                       "DOROTI_VARIABLE_BLUR_FULL_DETAIL": "1" if args.full_detail else "0",
                       "DOROTI_VARIABLE_BLUR_FULL_BANDS": "1" if args.full_bands else "0",
                       "DOROTI_VARIABLE_BLUR_BENCHMARK": mode, "DOROTI_MAUI_EVIDENCE": remote}
        environment["DOROTI_VARIABLE_BLUR_BENCHMARK_SIGMA"] = str(args.sigma)
        summary["attempts"].append({"repeat": repeat, "mode": mode, "policy": policy,
            "remoteEvidence": "Documents/" + remote, "environment": environment})
        (out / "summary.json").write_text(json.dumps(summary, indent=2))
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
                # Evidence is written asynchronously to the app container.
                # A transient copy failure must not kill a completed run
                # before the writer/container service makes it available.
                copy_attempts = 0
                for copy_attempts in range(1, 6):
                    try:
                        run("xcrun", "devicectl", "device", "copy", "from", "--device", args.device,
                            "--domain-type", "appDataContainer", "--domain-identifier", bundle,
                            "--source", "Documents/" + remote, "--destination", str(destination))
                        break
                    except subprocess.CalledProcessError as error:
                        (out / (name + f"-copy-{copy_attempts}.log")).write_text(error.stdout + error.stderr)
                        if copy_attempts == 5:
                            raise
                        time.sleep(.5)
                result = summarize(json.loads(destination.read_text()), args.hz)
                result.update(mode=mode, repeat=repeat, policy=policy, evidenceCopyAttempts=copy_attempts)
                loop = result["iosFrameLoop"]
                if not loop:
                    raise RuntimeError(f"App has no iOS frame-loop diagnostics; see {destination}")
                if loop and loop["policy"] != policy:
                    raise RuntimeError(f"Requested policy {policy} did not execute; see {destination}")
                summary["runs"].append(result)
                (out / "summary.json").write_text(json.dumps(summary, indent=2))
                if result["failedFrames"] or result["terminalBufferErrors"]:
                    raise RuntimeError(f"Renderer errors; see {destination}")
                if "Graphite-Metal" not in result["backend"]:
                    raise RuntimeError(f"Unexpected renderer; see {destination}")
                if (result["gaussianPassesLastFrame"] + result["kawasePassesLastFrame"] > 0) != (mode != "off" and args.sigma > 0):
                    raise RuntimeError(f"Blur mode did not render as requested; see {destination}")
                print(f"DONE {name}: {result['presentationMeanFps']:.2f} FPS, p95 {result['presentationP95Ms']:.2f} ms"
                      + (f", overlap {loop['overlapCount']}/{loop['newRasterSamples']}, pending max {loop['maximumPending']}" if loop else ""), flush=True)
            except Exception as error:
                summary["failedRun"] = {"repeat": repeat, "mode": mode, "policy": policy,
                    "error": f"{type(error).__name__}: {error}", "log": str(log_path),
                    "remoteEvidence": "Documents/" + remote, "environment": environment}
                (out / "summary.json").write_text(json.dumps(summary, indent=2))
                raise
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
