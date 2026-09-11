"""Exercise the actual Windows App SDK product fixture with explicit official assets.

This is an opt-in runtime experiment. It does not remove legacy build inputs or
claim that the final package/template has switched. Each command has a 1200s limit.
"""
import argparse
import hashlib
import json
import os
from pathlib import Path
import zipfile
from run import ROOT, OUT, VERSION, run, save


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--gpu", nargs="+", default=["AMD", "NVIDIA"])
    parser.add_argument("--resize", action="store_true")
    parser.add_argument("--acrylic", action="store_true")
    parser.add_argument("--no-validation", action="store_true")
    args = parser.parse_args()
    print(OUT, flush=True)
    project = ROOT / "Doroti/validation/hwnd-exact-cpp-product"
    if run("build", ["dotnet", "build", project / "Doroti.Validation.HwndExactCppProduct.csproj", "-c", "Release", "--nologo"]): return 1
    graph = json.loads((ROOT / "Doroti/artifacts/validation/build/hwnd-exact-cpp-product/obj/project.assets.json").read_text())
    target = graph["targets"]["net10.0-windows10.0.19041.0/win-x64"]
    entry = next(k for k in target[f"SkiaSharp/{VERSION}"]["runtime"] if k.endswith("SkiaSharp.dll"))
    cache = Path(os.environ.get("NUGET_PACKAGES", str(Path.home() / ".nuget/packages")))
    package = cache / "skiasharp" / VERSION
    archive = package / f"skiasharp.{VERSION}.nupkg"
    with zipfile.ZipFile(archive) as z: managed = z.read(entry)
    binary = ROOT / "Doroti/artifacts/validation/build/hwnd-exact-cpp-product/bin/Release/net10.0-windows10.0.19041.0/win-x64"
    if (binary / "SkiaSharp.dll").read_bytes() != managed or (package / entry).read_bytes() != managed:
        raise RuntimeError("Product managed DLL differs from resolved official archive entry")
    if run("managed-signature", ["dotnet", "nuget", "verify", archive, "--all"]): return 1
    native = ROOT / "Doroti/artifacts/stock-graphite/20260911T134338875511Z/W0-1/official/libSkiaSharp.dll"
    native_hash = "07ce51fd59e099b9561b0327223c27b21aa5605b5b8f4484dd297fdb8c8725a1"
    if hashlib.sha256(native.read_bytes()).hexdigest() != native_hash: raise RuntimeError("Native official DLL changed")
    manifest = OUT / "official-manifest.json"
    save(manifest, dict(nativePath=str(native), packageId="SkiaSharp.NativeAssets.Win32", version=VERSION, rid="win-x64",
                        sha256=native_hash, managedSha256=hashlib.sha256(managed).hexdigest()))
    save(OUT / "managed-provenance.json", dict(package="SkiaSharp", version=VERSION, entry=entry, bytes=len(managed), sha256=hashlib.sha256(managed).hexdigest()))
    os.environ["DOROTI_WINDOWS_GRAPHITE_OFFICIAL_MANIFEST"] = str(manifest)
    os.environ["DOROTI_WINDOWS_VULKAN_VALIDATION"] = "0" if args.no_validation else "1"
    rows = []
    for gpu in args.gpu:
        os.environ["DOROTI_WINDOWS_VULKAN_DEVICE"] = gpu
        options = ["--presenter", "default", "--smoke-ms", "3000", "--report", OUT / gpu / "report.json"]
        if not args.resize: options.append("--no-resize-burst")
        if args.acrylic: options.append("--acrylic")
        code = run(gpu, [binary / "Doroti.Validation.HwndExactCppProduct.exe", *options])
        report = OUT / gpu / "report.json"
        native_warnings = [line for line in (OUT / gpu / "stderr.log").read_text(encoding="utf-8").splitlines() if "[skia] WARNING" in line or "[skia] ERROR" in line]
        rows.append(dict(gpu=gpu, exitCode=code, nativeWarnings=native_warnings, report=json.loads(report.read_text()) if report.exists() else None))
    passed = all(r["exitCode"] == 0 and not r["nativeWarnings"] for r in rows)
    save(OUT / "summary.json", dict(status="PASS-scoped-product-fixture" if passed else "FAIL",
                                   officialManifest=str(manifest), resize=args.resize, acrylic=args.acrylic,
                                   finalPackaging="notVerified", performance="notVerified", physicalScanOut="notVerified", rows=rows))
    return 0 if passed else 1


if __name__ == "__main__":
    raise SystemExit(main())
