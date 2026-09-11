"""Validate and preserve alternative retirement evidence without rerunning GPU work.

Usage: python write-retirement-evidence.py <run-directory> <output-directory>
Run through validation/run-with-timeout.py.
"""
import hashlib
import json
from pathlib import Path
import sys

ROOT = Path(__file__).resolve().parents[3]


def main():
    run, output = map(lambda p: Path(p).resolve(), sys.argv[1:])
    read = lambda p: json.loads(p.read_text(encoding="utf-8-sig"))
    summary = read(run / "summary.json")
    if summary["status"] != "PASS" or len(summary["reports"]) != 6:
        raise RuntimeError("Alternative protocol matrix did not pass")
    sources = read(run / "source-identity.json")
    for source in sources:
        path = ROOT / source["path"]
        if path.suffix in (".cs", ".csproj") and hashlib.sha256(path.read_bytes()).hexdigest() != source["sha256"]:
            raise RuntimeError(f"GPU evidence is stale for {path}")
    close, dispose, delayed_retirement = [], [], []
    asset = read(run / "official-asset.json")
    for item in summary["reports"]:
        r = item["report"]
        if item["exitCode"] != 0 or r["validationCount"] != 0 or r["callbackFault"]:
            raise RuntimeError("Runtime/validation failure")
        if r["requestedAsset"]["sha256"] != r["loadedAsset"]["sha256"] or r["loadedAsset"]["sha256"] != asset["sha256"]:
            raise RuntimeError("Wrong native asset")
        if r["peakLiveGenerations"] != 1 or r["liveGenerationsAfterRun"] != 0 or len(r["retirementGenerations"]) != 3:
            raise RuntimeError("Generation ownership imbalance")
        for generation in r["retirementGenerations"]:
            p = generation["protocol"]
            if not generation["nativeObjectsReleased"] or p["disposeNativeWaitCalls"] or p["ownerThread"] == p["coordinatorThread"]:
                raise RuntimeError("Retirement ownership/wait contract failed")
            if p["rejectedGenerations"] != 1000 or p["rejectedFrames"] != 1000:
                raise RuntimeError("Admission cap not exercised")
            if item["mode"] == "retire-delayed":
                if p["pendingFenceAtClose"] != "NotReady" or not p["callbackPendingAtClose"] or not p["callbackCompleted"] or "FaultedHold" not in p["events"]:
                    raise RuntimeError("Delayed callback/hold contract not exercised")
                if not r["originalFiveSecondFullRetirementGate"].startswith("FAIL"):
                    raise RuntimeError("Original acceptance was incorrectly promoted")
                delayed_retirement.append(generation["fullNativeRetirementMilliseconds"])
            elif generation["fullNativeRetirementMilliseconds"] >= 5000:
                raise RuntimeError("Ready/cancel native reclamation deadline failed")
            if item["mode"] == "retire-cancel" and (not p["cancelledBeforeInsert"] or p["graphFramesSubmitted"] or p["hostInputWaitSubmitted"]):
                raise RuntimeError("Unavailable host input was submitted")
            close.append(p["logicalCloseMilliseconds"])
            dispose.append(p["disposeMilliseconds"])
    commands = read(run / "commands.json")
    if any(c["timeoutSeconds"] != 1200 or c["exitCode"] != 0 for c in commands):
        raise RuntimeError("Command timeout/exit evidence mismatch")
    evidence = dict(schema="doroti.stock-graphite.retirement-evidence/v1", status="PASS-scoped-protocol",
                    overallW0="PARTIAL", runDirectory=str(run.relative_to(ROOT)), commands=commands,
                    executedSources=sources, asset=asset, results=summary,
                    measurements=dict(contextGenerations=18, maxCloseAcknowledgementMs=max(close),
                                      maxDrainedDisposeMs=max(dispose), delayedFullNativeRetirementMs=delayed_retirement),
                    sourceReferences=[
                        "https://raw.githubusercontent.com/mono/SkiaSharp/143a933a753dbfeca1909524b2c06c546c5c3e20/binding/SkiaSharp/Gpu/Graphite/SKGraphiteContext.cs",
                        "https://docs.vulkan.org/refpages/latest/refpages/source/vkGetFenceStatus.html",
                        "https://docs.vulkan.org/refpages/latest/refpages/source/vkDestroyDevice.html"],
                    rawFiles=[dict(path=str(p.relative_to(ROOT)),sha256=hashlib.sha256(p.read_bytes()).hexdigest())
                              for p in sorted(run.rglob("*")) if p.is_file()])
    output.mkdir(parents=True, exist_ok=True)
    (output / "evidence.json").write_text(json.dumps(evidence, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    print(json.dumps(evidence["measurements"], indent=2))
    print("PASS: asset, source, ownership, callbacks, admission cap, timeout and original acceptance boundaries")


if __name__ == "__main__":
    main()
