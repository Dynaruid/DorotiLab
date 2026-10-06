"""Native Windows scroll-onset regression; requires a built Release sample.

Uses native wheel messages, including small high-resolution deltas. Timing ends
at native frame submission; this is not physical trackpad or scanout validation.
Run through Doroti/eng/run-with-timeout.py --timeout 1200.
"""
import argparse
import json
import os
from pathlib import Path
import subprocess


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--output", default="temp/testing/carousel-windows-scroll")
    args = parser.parse_args()
    if os.name != "nt":
        raise SystemExit("This regression requires Windows.")
    root = Path(__file__).resolve().parents[3]
    exe = root / "samples/DorotiCarouselApp/windowsappsdk/bin/Release/net10.0-windows10.0.19041.0/win-x64/DorotiCarouselApp.WindowsAppSdk.exe"
    output = Path(args.output).resolve()
    output.mkdir(parents=True, exist_ok=True)
    report = output / "native-report.json"
    if report.exists():
        report.unlink()
    env = dict(os.environ, DOROTI_WINDOWS_APPSDK_SMOKE_MS="8000",
               DOROTI_WINDOWS_APPSDK_SCROLL_SMOKE="1",
               DOROTI_WINDOWS_APPSDK_DIAGNOSTICS="1",
               DOROTI_FRAME_TRACE_CAPACITY="65536",
               DOROTI_WINDOWS_APPSDK_REPORT=str(report))
    env.pop("DOROTI_WINDOWS_APPSDK_INPUT_SMOKE", None)
    startup = subprocess.STARTUPINFO()
    startup.dwFlags |= subprocess.STARTF_USESHOWWINDOW
    startup.wShowWindow = 1
    run = subprocess.run([str(exe)], cwd=exe.parent, env=env, startupinfo=startup,
                         capture_output=True, text=True, timeout=90)
    (output / "native.log").write_text(run.stdout + run.stderr, encoding="utf-8")
    if run.returncode != 0 or not report.exists():
        raise RuntimeError(f"Native scroll probe failed: exit={run.returncode}; see {output / 'native.log'}")
    document = json.loads(report.read_text(encoding="utf-8"))
    frames = document["frames"]
    trace = document["renderer"]["trace"]
    # DorotiFramePhase: scrollStart=21, scrollUpdate=22, sceneSubmitted=12,
    # present=14. Require an actually moved scene, not a scroll-start UI change.
    starts = [entry for entry in trace if entry["phase"] == 21
              and entry["scrollActivity"] == "CarouselWheelScrollActivity"]
    if len(starts) != 2:
        raise RuntimeError(f"Expected two uninterrupted wheel bursts, got {len(starts)}.")
    latencies = []
    for index, start in enumerate(starts):
        end = starts[index + 1]["sequence"] if index + 1 < len(starts) else float("inf")
        movement = next(entry for entry in trace
                        if start["sequence"] < entry["sequence"] < end
                        and entry["phase"] == 22 and entry["scrollDelta"] != 0)
        scene = next(entry for entry in trace
                     if movement["sequence"] < entry["sequence"] < end and entry["phase"] == 12)
        present = next(entry for entry in trace
                       if scene["sequence"] < entry["sequence"] < end and entry["phase"] == 14
                       and entry["sceneSequence"] >= scene["sceneSequence"])
        latency = (present["recordedAtMicroseconds"] - start["recordedAtMicroseconds"]) / 1000
        # A regression guard against the reported second-long startup stall,
        # not a frame-rate or hardware benchmark acceptance threshold.
        if latency >= 500:
            raise RuntimeError(f"Burst {index + 1} did not submit movement promptly: {latency:.2f}ms.")
        latencies.append(round(latency, 3))
    if frames["failedTerminals"] or frames["staleInputPresentsPrevented"]:
        raise RuntimeError("Native input invalidated a completed scroll frame.")
    result = dict(status="PASS", packets=24, burstStartToNativeSubmissionMs=latencies,
                  staleInputPresentsPrevented=frames["staleInputPresentsPrevented"],
                  physicalTrackpad="notVerified", scanout="notVerified", displayFps="notMeasured")
    (output / "result.json").write_text(json.dumps(result, indent=2), encoding="utf-8")
    print(json.dumps(result))


if __name__ == "__main__":
    main()
