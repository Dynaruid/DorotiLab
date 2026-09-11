"""Run W0-2 journal and W0-3 official-asset GPU copy diagnostics."""
import argparse
import hashlib
import json
from run import HERE, ROOT, OUT, run, save


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--gpu", nargs="+", default=["AMD", "NVIDIA"])
    parser.add_argument("--mode", nargs="+", choices=["copy-one", "copy-two", "copy-msaa", "copy-v2"], default=["copy-one", "copy-two", "copy-msaa", "copy-v2"])
    args = parser.parse_args()
    print(OUT, flush=True)
    run("head", ["git", "rev-parse", "HEAD"])
    run("status", ["git", "status", "--short"])
    original = ROOT / "Doroti/artifacts/stock-graphite/20260911T134338875511Z"
    identity = json.loads((original / "W0-0/official-asset.json").read_text())
    asset = original / "W0-1/official/libSkiaSharp.dll"
    archive = original / f"W0-0/{identity['package']}.{identity['version']}.nupkg"
    if hashlib.sha256(asset.read_bytes()).hexdigest() != identity["sha256"] or hashlib.sha256(archive.read_bytes()).hexdigest() != identity["archiveSha256"]:
        raise RuntimeError("Official asset changed")
    save(OUT / "asset.json", identity)
    sources = list(HERE.glob("*")) + list((ROOT / "Doroti/src/Doroti.Skia.Vulkan/Stock").glob("*"))
    save(OUT / "source-identity.json", [dict(path=str(p.relative_to(ROOT)), sha256=hashlib.sha256(p.read_bytes()).hexdigest())
                                       for p in sorted(sources) if p.is_file()])
    if run("signature", ["dotnet", "nuget", "verify", archive, "--all"]): return 1
    if run("build", ["dotnet", "build", HERE / "StockGraphite.csproj", "-c", "Release", "--nologo"]): return 1
    exe = ROOT / "Doroti/artifacts/validation/build/stock-graphite-vulkan/bin/Release/net10.0/win-x64/StockGraphite.exe"
    if run("journal-tests", [exe, "--journal-tests"]): return 1
    rows = []
    for gpu in args.gpu:
        for mode in args.mode:
            label = f"{gpu}/{mode}"
            report = OUT / label / "report.json"
            code = run(label, [exe, asset, gpu, mode, report])
            native_warnings = [line for line in (OUT / label / "stderr.log").read_text(encoding="utf-8").splitlines() if "[skia] WARNING" in line or "[skia] ERROR" in line]
            rows.append(dict(gpu=gpu, mode=mode, exitCode=code, nativeWarnings=native_warnings, report=json.loads(report.read_text()) if report.exists() else None))
    passed = all(row["exitCode"] == 0 and not row["nativeWarnings"] for row in rows)
    save(OUT / "summary.json", dict(status="PASS-scoped-GPU-diagnostic" if passed else "FAIL", product="notVerified", rows=rows))
    print(OUT / "summary.json", flush=True)
    return 0 if passed else 1


if __name__ == "__main__":
    raise SystemExit(main())
