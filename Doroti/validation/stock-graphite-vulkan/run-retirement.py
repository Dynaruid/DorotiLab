"""Run alternative shutdown protocols; original failure evidence stays immutable.

All builds and native runs use the repository's 1,200-second external timeout.
"""
import hashlib
from pathlib import Path
import json
import sys
from run import HERE, ROOT, OUT, run, save


def main():
    print(OUT, flush=True)
    run("head", ["git", "rev-parse", "HEAD"])
    run("status", ["git", "status", "--short"])
    asset_run = ROOT / "Doroti/artifacts/stock-graphite/20260911T134338875511Z"
    if len(sys.argv) > 1:
        asset_run = Path(sys.argv[1]).resolve()
    identity = json.loads((asset_run / "W0-0/official-asset.json").read_text())
    asset = asset_run / "W0-1/official/libSkiaSharp.dll"
    archive = asset_run / f"W0-0/{identity['package']}.{identity['version']}.nupkg"
    if hashlib.sha256(asset.read_bytes()).hexdigest() != identity["sha256"] or hashlib.sha256(archive.read_bytes()).hexdigest() != identity["archiveSha256"]:
        raise RuntimeError("Previously verified official asset changed")
    save(OUT / "official-asset.json", identity)
    save(OUT / "source-identity.json", [dict(path=str(p.relative_to(ROOT)), sha256=hashlib.sha256(p.read_bytes()).hexdigest())
                                       for p in sorted(HERE.glob("*")) if p.is_file()])
    if run("signature", ["dotnet", "nuget", "verify", archive, "--all"]):
        return 1
    if run("build", ["dotnet", "build", HERE / "StockGraphite.csproj", "-c", "Release", "--nologo"]):
        return 1
    exe = ROOT / "Doroti/artifacts/validation/build/stock-graphite-vulkan/bin/Release/net10.0/win-x64/StockGraphite.exe"
    reports = []
    for gpu in ("AMD", "NVIDIA"):
        for mode in ("retire-ready", "retire-delayed", "retire-cancel"):
            label = f"{gpu}/{mode}"
            path = OUT / label / "report.json"
            code = run(label, [exe, asset, gpu, mode, path])
            reports.append(dict(gpu=gpu, mode=mode, exitCode=code,
                                report=json.loads(path.read_text()) if path.exists() else None))
    save(OUT / "summary.json", dict(status="PASS" if all(r["exitCode"] == 0 for r in reports) else "FAIL",
                                   originalW0Acceptance="PARTIAL: delayed GPU reclamation exceeds five seconds",
                                   presentation="notVerified", reports=reports))
    print(OUT / "summary.json", flush=True)
    return 0 if all(r["exitCode"] == 0 for r in reports) else 1


if __name__ == "__main__":
    sys.exit(main())
