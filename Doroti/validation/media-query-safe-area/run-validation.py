"""Reproducible MQ validation. Every child has its own 1200-second timeout."""
from pathlib import Path
import datetime
import json
import os
import subprocess
import sys
import time

root = Path(__file__).resolve().parents[3]
evidence = root / "Doroti/artifacts/validation/media-query-safe-area"
evidence.mkdir(parents=True, exist_ok=True)
suite = sys.argv[1] if len(sys.argv) > 1 else "contracts"
if suite == "all":
    codes = [subprocess.run([sys.executable, __file__, part], cwd=root, check=False).returncode
             for part in ("builds", "contracts", "products")]
    sys.exit(1 if any(codes) else 0)
steps = []
def add(name, *command):
    steps.append((name, list(command)))

if suite == "contracts":
    for name, project, arguments in [
        ("common", "media-query-safe-area/Doroti.Validation.MediaQuerySafeArea.csproj", []),
        ("framework-work", "framework-work/Doroti.Validation.FrameworkWork.csproj", []),
        ("resize", "resize-contract/Doroti.Validation.ResizeContract.csproj", []),
        ("qt-abi", "linux-qt-contract/Doroti.Validation.LinuxQtContract.csproj", []),
        ("browser-wire", "media-query-web-wire/Doroti.Validation.MediaQueryWebWire.csproj", []),
        ("material-scaffold", "fcr7-material-widget/Doroti.Validation.Fcr7MaterialWidget.csproj", ["--scaffold-metrics"]),
        ("material-full", "fcr7-material-widget/Doroti.Validation.Fcr7MaterialWidget.csproj", []),
        ("windows-abi", "windowsappsdk-native-abi/Doroti.Validation.WindowsAppSdkNativeAbi.csproj",
         [str(root / "Doroti/artifacts/validation/build/windowsappsdk-native-abi/bin/Debug/net10.0-windows10.0.19041.0/win-x64/doroti_windows_appsdk_host_v1.dll")]),
    ]:
        add(name, "dotnet", "run", "--project", "Doroti/validation/" + project, "--", *arguments)
    add("browser-environment", "node", "Doroti/validation/media-query-safe-area/browser-environment.mjs")
elif suite == "builds":
    for rid in ["win-x64", "android-arm64", "android-x64", "ios-arm64", "iossimulator-arm64", "iossimulator-x64", "maccatalyst-arm64", "osx-arm64"]:
        add("maui-" + rid, "dotnet", "build", "Doroti/src/Doroti.Host.Maui/Doroti.Host.Maui.csproj", "-p:RuntimeIdentifier=" + rid, "--nologo", "-v:q")
    for name in ["Web", "WindowsAppSdk", "Qt"]:
        add("host-" + name, "dotnet", "build", f"Doroti/src/Doroti.Host.{name}/Doroti.Host.{name}.csproj", "--nologo", "-v:q")
    add("testbed", "dotnet", "build", "DorotiTestbedApp/DorotiTestbedApp.csproj", "--nologo", "-v:q")
elif suite == "products":
    add("windows-consumer", "dotnet", "build", "DorotiTestbedApp/windowsappsdk/DorotiTestbedApp.WindowsAppSdk.csproj", "--nologo", "-v:q")
    add("maui-windows-consumer", "dotnet", "build", "DorotiTestbedApp/windows/DorotiTestbedApp.Windows.csproj", "--nologo", "-v:q")
    add("web-consumer", "dotnet", "build", "DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj", "--nologo", "-v:q")
    add("android-consumer", "dotnet", "build", "DorotiTestbedApp/android/DorotiTestbedApp.Android.csproj", "-p:RuntimeIdentifier=android-x64", "-p:EmbedAssembliesIntoApk=true", "--nologo", "-v:q")
    for name in ["Doroti.Host.Web", "Doroti.Target.Windows.WindowsAppSdk.win-x64", "Doroti.Templates"]:
        folder = "templates" if name == "Doroti.Templates" else "src"
        add("pack-" + name, "dotnet", "pack", f"Doroti/{folder}/{name}/{name}.csproj", "-o", "Doroti/artifacts/mq-packages", "--nologo", "-v:q")
else:
    raise SystemExit("Expected contracts, builds or products")

results = []
for name, command in steps:
    print("START " + name, flush=True)
    start = time.monotonic()
    with (evidence / (name + ".log")).open("w", encoding="utf-8") as log:
        env = os.environ.copy()
        if name == "framework-work": env["DOROTI_STAGE_TRACE"] = "1"
        process = subprocess.Popen(command, cwd=root, env=env, stdout=log, stderr=subprocess.STDOUT)
        try:
            code = process.wait(timeout=1200)
        except subprocess.TimeoutExpired:
            subprocess.run(["taskkill", "/PID", str(process.pid), "/T", "/F"], stdout=log, stderr=log, check=False)
            process.wait()
            code = 124
    results.append({"name": name, "command": command, "exitCode": code,
        "status": "automatedPassed" if code == 0 else "failed", "timeoutSeconds": 1200,
        "elapsedSeconds": round(time.monotonic() - start, 2)})
    (evidence / (suite + ".json")).write_text(json.dumps({"utc": datetime.datetime.now(datetime.timezone.utc).isoformat(),
        "results": results}, indent=2), encoding="utf-8")
    print(f"END {name}: {code}", flush=True)
sys.exit(1 if any(result["exitCode"] for result in results) else 0)
