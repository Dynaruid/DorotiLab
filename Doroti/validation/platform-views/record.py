"""Run one PlatformView gate with the mandatory timeout and persist auditable evidence."""
import argparse
import datetime
import hashlib
import json
import os
from pathlib import Path
import platform
import shutil
import subprocess
import sys

ROOT = Path(__file__).resolve().parents[3]
DOROTI = ROOT / "Doroti"
CASES = {
    "common": ["dotnet", "run", "--project", "validation/platform-views/PlatformViews.csproj", "--no-launch-profile"],
    "windows-stacking": ["pwsh", "-NoProfile", "-File", "validation/platform-views/run-windows-stacking.ps1"],
    "windows-attachment": ["dotnet", "run", "--project", "validation/platform-views/windows/WindowsPlatformViews.csproj", "--no-launch-profile"],
    "web-dom": ["node", "validation/platform-views/web-dom.mjs"],
    "web-typescript": ["node", "validation/web-playwright/node_modules/typescript/bin/tsc", "--project", "src/Doroti.Host.Web/Web/tsconfig.json", "--noEmit"],
    "product-build": ["dotnet", "build", "Doroti.Product.slnx", "--nologo", "-m:1", "-p:UseSharedCompilation=false", "-v:q"],
    "windows-product-build": ["dotnet", "build", "../DorotiTestbedApp/windowsappsdk/DorotiTestbedApp.WindowsAppSdk.csproj", "--nologo", "-m:1", "-v:q"],
    "testbed-build": ["dotnet", "build", "../DorotiTestbedApp/DorotiTestbedApp.csproj", "--nologo", "-m:1", "-v:q"],
    "macos-attachment": [sys.executable, "validation/platform-views/macos/run.py"],
    "macos-product-build": ["dotnet", "build", "../DorotiTestbedApp/macos/DorotiTestbedApp.MacOS.csproj", "-r", "osx-arm64", "--nologo", "-m:1", "-v:q"],
    "macos-product-live": [sys.executable, "validation/platform-views/macos/product.py"],
    "macos-interleaved": [sys.executable, "validation/platform-views/macos/interleaved.py"],
}

def git(*args):
    return subprocess.check_output(["git", *args], cwd=ROOT, text=True, encoding="utf-8").strip()

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("gate", choices=CASES)
    args = parser.parse_args()
    out = DOROTI / "docs/validation/platform-views" / datetime.date.today().isoformat() / args.gate
    out.mkdir(parents=True, exist_ok=True)
    command = [sys.executable, "validation/run-with-timeout.py", *CASES[args.gate]]
    started = datetime.datetime.now(datetime.timezone.utc).isoformat()
    paths = set(git("diff", "--name-only").splitlines()) | set(git("ls-files", "--others", "--exclude-standard").splitlines())
    hashes = {name: hashlib.sha256((ROOT / name).read_bytes()).hexdigest() for name in sorted(paths)
              if (ROOT / name).is_file() and "/docs/validation/" not in name and (ROOT / name).suffix in {".cs", ".csproj", ".ts", ".cpp", ".py", ".ps1", ".mjs", ".md", ".json"}}
    (out / "source-hashes.json").write_text(json.dumps(hashes, indent=2), encoding="utf-8")
    with (out / "command.log").open("w", encoding="utf-8") as log:
        result = subprocess.run(command, cwd=DOROTI, stdout=log, stderr=subprocess.STDOUT)
    passed = result.returncode == 0
    artifacts = [str((out / name).relative_to(ROOT)).replace("\\", "/") for name in ["command.log", "source-hashes.json"]]
    if args.gate == "web-dom":
        for file in (DOROTI / "artifacts/validation/platform-views/web-dom").glob("*"):
            if file.suffix in {".png", ".json"}:
                destination = out / ("browser-result.json" if file.name == "result.json" else file.name)
                shutil.copyfile(file, destination)
                artifacts.append(str(destination.relative_to(ROOT)).replace("\\", "/"))
    if args.gate in {"macos-product-live", "macos-interleaved"}:
        folder = "macos-interleaved" if args.gate == "macos-interleaved" else "macos-product"
        for file in (DOROTI / "artifacts/validation/platform-views" / folder).glob("*"):
            destination = out / file.name
            shutil.copyfile(file, destination)
            artifacts.append(str(destination.relative_to(ROOT)).replace("\\", "/"))
    evidence = {
        "schemaVersion": "doroti.platform-views.evidence/v1", "commit": git("rev-parse", "HEAD"),
        "dirty": git("status", "--short").splitlines(), "target": args.gate,
        "os": platform.platform(), "rid": ("osx-arm64" if platform.machine() == "arm64" else "osx-x64") if sys.platform == "darwin" else
            ("win-x64" if sys.platform == "win32" else "linux-" + platform.machine()), "device": platform.machine(),
        "runtime": "dotnet SDK " + subprocess.check_output(["dotnet", "--version"], cwd=DOROTI, text=True).strip(),
        "renderer": "AppKit Graphite-Metal and Ganesh-Metal" if args.gate in {"macos-product-live", "macos-interleaved"} else
            "AppKit NSControl harness" if args.gate == "macos-attachment" else
            "isolated DOM" if args.gate == "web-dom" else "CPU/fake" if args.gate == "common" else "notApplicable",
        "command": subprocess.list2cmdline(command), "workingDirectory": str(DOROTI),
        "startedUtc": started, "finishedUtc": datetime.datetime.now(datetime.timezone.utc).isoformat(),
        "exitCode": result.returncode, "timeoutSeconds": 1200,
        "sourceReviewed": "passed", "build": "notVerified" if args.gate in {"macos-product-live", "macos-interleaved"} else "passed" if passed else "failed",
        "automated": ("passed" if passed else "failed") if not args.gate.endswith("-build") and args.gate != "web-typescript" else "notVerified",
        "productLive": ("passed" if passed else "failed") if args.gate in {"macos-product-live", "macos-interleaved"} else "notVerified",
        "physical": "notVerified", "nativeAot": "notVerified",
        "capabilities": [{"request": args.gate, "result": "passed" if passed else "failed"}],
        "artifacts": artifacts,
        "remaining": ["AppKit acceptance beyond recorded C scenarios, physical display synchronization, gesture mediation and complete PV-5/PV-10 remain open." if args.gate.startswith("macos-") else
                      "Actual product compositor integration and all platform B/C acceptance gates remain open.",
                      "Korean IME, screen readers, physical devices, deployment and 0/1/4-view performance are not qualified."],
    }
    (out / "result.json").write_text(json.dumps(evidence, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    print(json.dumps({"gate": args.gate, "exitCode": result.returncode, "evidence": str(out / "result.json")}), flush=True)
    if not passed:
        print("\n".join((out / "command.log").read_text(encoding="utf-8", errors="replace").splitlines()[-30:]), flush=True)
    return result.returncode

if __name__ == "__main__":
    raise SystemExit(main())
