"""Small maintained suites. All raw evidence belongs to one disposable run directory."""
import json
from pathlib import Path
import re
import shutil
import subprocess
import sys
import uuid
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]
RUN_ROOT = ROOT / "temp/testing"


def source():
    tracked = subprocess.check_output(["git", "ls-files", "-z"], cwd=ROOT).decode().split("\0")
    forbidden = [p for p in tracked if p.startswith("temp/")]
    if forbidden:
        raise RuntimeError(f"Temporary files are tracked: {forbidden}")
    # Current entry points and referenced planning archives, not deleted probe reports.
    docs = [ROOT / p for p in ("history/26-10-03/platform-gap-summary.md", "history/26-09-28/plan-summary.md", "README.md", "README.ko.md", "Doroti/README.md", "history/26-10-03/works/README.md",
        "Doroti/tests/README.md", "Doroti/docs/support-status.md", "Doroti/docs/desktop-windows.md",
        "Doroti/docs/rendering-baselines.md", "Doroti/docs/web-host-architecture.md", "Doroti/docs/development-hot-reload.md", "tools/vscode-doroti/README.md",
        "Doroti/docs/application-navigation.md", "Doroti/docs/desktop-window-context.md", "Doroti/docs/release-candidates.md",
        "Doroti/docs/platform-views/support-matrix.md", "Doroti/docs/validation/2026-10-04-web-structure.md",
        "samples/DorotiTestbedApp/README.md", "samples/DorotiSampleApp2/README.md",
        "history/26-10-05/README.md", "history/26-10-05/work-summary.md", "history/26-10-05/work2-summary.md", "history/26-10-05/work3-summary.md",
        "Doroti/docs/doctor.md", "Doroti/docs/validation/2026-10-04-full-review.md",
        "packages/platforms/maui/README.md",
        "Doroti/templates/Doroti.Templates/content/doroti-app/desktop/README.md")]
    docs += list((ROOT / "history/26-10-03/works").rglob("*.md"))
    docs += list((ROOT / "Doroti/docs/migrations/design-platform").glob("*.md"))
    docs += [ROOT / f"packages/Doroti.{design}/README.md" for design in ("Material", "Cupertino")]
    broken = []
    for doc in set(docs):
        for target in re.findall(r"\]\(([^)]+)\)", doc.read_text(encoding="utf-8-sig")):
            target = target.split("#", 1)[0]
            if not target or re.match(r"[a-z]+:", target):
                continue
            if not (doc.parent / target).exists():
                broken.append(f"{doc.relative_to(ROOT)} -> {target}")
    if broken:
        raise RuntimeError("Broken current documentation links:\n" + "\n".join(broken))
    print("Source: PASS (current documentation paths; tracked temporary-file policy)", flush=True)


def main(suite, extra=()):
    run = RUN_ROOT / suite.lower() / uuid.uuid4().hex
    run.mkdir(parents=True)
    print(f"Test run: {run} (timeout/failure evidence retained until investigation ends)", flush=True)
    success = False

    def command(name, *args):
        print(f"Running {name}: {' '.join(args)}", flush=True)
        with (run / f"{name}.log").open("w", encoding="utf-8") as log:
            result = subprocess.run(args, cwd=ROOT, stdout=log, stderr=subprocess.STDOUT)
        if result.returncode:
            print((run / f"{name}.log").read_text(encoding="utf-8", errors="replace")[-18000:])
            raise RuntimeError(f"{name} failed: exit {result.returncode}")
        print(f"{name}: PASS", flush=True)

    try:
        if suite in ("Source", "Developer", "Release"):
            source()
            command("runner-contract", sys.executable, "Doroti/tests/runner_contract.py", str(run / "runner"))
            command("installer-contract", sys.executable, "Doroti/tests/installer_contract.py", str(run / "installer"))
            command("doctor-contract", sys.executable, "Doroti/tests/doctor_contract.py")
            command("full-review-tools", sys.executable, "Doroti/tests/full_review_tools.py")
            command("android-development-bridge", sys.executable, "Doroti/tests/android_development_bridge.py")
        if suite in ("Build", "Developer", "Release"):
            command("widget-regressions", "dotnet", "run", "--project",
                    "Doroti/tests/Doroti.Tests/Doroti.Tests.csproj", "-c", "Debug",
                    "--artifacts-path", str(run / "build"))
        if suite in ("Build", "Packages", "Targets", "Developer", "Release"):
            for design in ("Material", "Cupertino"):
                command(design.lower() + "-regressions", "dotnet", "run", "--project",
                    f"packages/Doroti.{design}/tests/Doroti.{design}.Tests/Doroti.{design}.Tests.csproj", "-c", "Debug",
                    "--artifacts-path", str(run / (design.lower() + "-build")))
            command("platform-bootstrap-contract", sys.executable,
                    "Doroti/tests/platform_bootstrap_contract.py")
            command("frame-submission-contract", sys.executable,
                    "Doroti/tests/frame_submission_contract.py")
            command("typed-transport-contract", sys.executable, "Doroti/tests/typed_transport_contract.py")
            command("tool-extension-contract", sys.executable,
                    "Doroti/tests/tool_extension_contract.py")
        if suite in ("Developer", "Release"):
            command("plugin-regressions", "dotnet", "run", "--project", "Doroti/tests/Doroti.Plugin.Tests/Doroti.Plugin.Tests.csproj", "--artifacts-path", str(run / "plugin-build"))
            command("os-drop-regressions", "dotnet", "run", "--project", "Doroti/tests/Doroti.Drop.Tests/Doroti.Drop.Tests.csproj", "--artifacts-path", str(run / "drop-build"))
            command("web-rendering", "node", "--experimental-transform-types", "--test", "Doroti/tests/web_rendering.mts")
            command("web-pointer-admission", "node", "--experimental-transform-types", "--test", "Doroti/tests/web_pointer_admission.mts")
            command("web-semantics", "node", "--experimental-transform-types", "--test", "Doroti/tests/web_semantics.mts")
            command("web-worker-lifecycle", "node", "--experimental-transform-types", "--test", "Doroti/tests/web_worker_lifecycle.mts")
            command("web-gl-frames", "node", "--experimental-transform-types", "--test", "Doroti/tests/web_gl_frames.mts")
            command("web-splash", "node", "--experimental-transform-types", "--test", "Doroti/tests/web_splash.mts")
            command("webcil-cache", sys.executable, "Doroti/tests/webcil_cache.py")
            command("web-textures", "node", "--experimental-transform-types", "--experimental-vm-modules", "--test", "Doroti/tests/web_textures.mts")
            command("web-full-review", "node", "--experimental-transform-types", "--experimental-vm-modules", "--test", "Doroti/tests/web_full_review.mts")
            command("web-managed-connection", "node", "--experimental-transform-types", "--experimental-vm-modules", "--test", "Doroti/tests/web_managed_connection.mts")
        if suite in ("Packages", "Developer", "Release"):
            command("maui-tool-devices", "dotnet", "run", "--project", "packages/platforms/maui/tests/Doroti.Tool.Maui.Tests/Doroti.Tool.Maui.Tests.csproj", "--artifacts-path", str(run / "maui-tool-build"))
            command("platform-provider-contract", sys.executable, "Doroti/tests/platform_provider_contract.py")
        if suite in ("Targets", "Release"):
            for target in ("windowsappsdk/DorotiTestbedApp.WindowsAppSdk.csproj", "web/DorotiTestbedApp.Web.csproj"):
                command(target.split('/')[0], "dotnet", "build", "samples/DorotiTestbedApp/" + target,
                        "-c", "Debug", "--nologo")
            command("web-startup", sys.executable, "Doroti/tests/web_smoke.py", str(run / "web"))
        if suite == "LinuxSmoke":
            command("linux-qt-profiles", sys.executable, "Doroti/tests/linux_qt_build_profiles.py", "--output", str(run / "profiles"))
            command("linux-qt", sys.executable, "Doroti/tests/linux_qt_smoke.py", "--output", str(run / "qt"))
        if suite == "AndroidSmoke":
            command("android", sys.executable, "Doroti/tests/android_smoke.py", "--output", str(run / "android"), *extra)
            android_result = json.loads((run / "android/summary.json").read_text(encoding="utf-8"))
            if android_result["status"] == "PARTIAL":
                success = True
                print("Validation AndroidSmoke: PARTIAL; requested device cases were SKIPPED", flush=True)
                return 2
        if suite in ("IOSSmoke", "CatalystSmoke"):
            command("apple-profiles", sys.executable, "Doroti/tests/apple_build_profiles.py")
            command("apple-provider-contract", sys.executable, "Doroti/tests/apple_provider_contract.py", "--output", str(run / "apple-provider.json"))
            target = "ios" if suite == "IOSSmoke" else "maccatalyst"
            extra_cases = ["--cases", "services,input,features,navigation,restoration,multi,shutdown"] if target == "ios" else []
            command("apple-" + target, sys.executable, "Doroti/tests/apple_smoke.py", "--target", target, "--activation", "native-callback" if target == "ios" else "os", "--output", str(run / target), *extra_cases)
        if suite == "MacOSSmoke":
            command("apple-profiles", sys.executable, "Doroti/tests/apple_build_profiles.py")
            command("apple-provider-contract", sys.executable, "Doroti/tests/apple_provider_contract.py", "--output", str(run / "apple-provider.json"))
            command("macos-appkit", sys.executable, "Doroti/tests/macos_smoke.py", "--output", str(run / "appkit"))
        if suite == "WindowsSmoke":
            for fixture in ("windows_shared_tree", "windows_window_kinds", "windows_native_menu", "windows_design_presentation", "windows_provider_packages"):
                command(fixture, sys.executable, "Doroti/tests/" + fixture + ".py")
            command("windows-smoke", sys.executable, "Doroti/tests/windows_smoke.py", str(run / "windows"),
                    "samples/DorotiTestbedApp/windowsappsdk/bin/Release/net10.0-windows10.0.19041.0/win-x64/DorotiTestbedApp.WindowsAppSdk.exe")
        if suite in ("Packages", "Developer", "Release"):
            command("design-package-contract", sys.executable, "Doroti/tests/design_package_contract.py")
        success = True
        evidence = "receipt retained" if suite == "AndroidSmoke" else "raw artifacts cleaned after summary"
        print(f"Validation {suite}: PASS; {evidence}", flush=True)
    finally:
        if success:
            resolved = run.resolve()
            if not resolved.is_relative_to(RUN_ROOT.resolve()) or run.is_symlink():
                raise RuntimeError(f"Unsafe cleanup path: {run}")
            if suite == "AndroidSmoke":
                print(f"Android receipt retained: {run / 'android/summary.json'}", flush=True)
            else:
                shutil.rmtree(resolved)
        else:
            print(f"FAIL evidence retained for investigation: {run}", flush=True)


if __name__ == "__main__":
    if len(sys.argv) < 2 or sys.argv[1] not in ("Source", "Build", "Targets", "WindowsSmoke", "AndroidSmoke", "LinuxSmoke", "MacOSSmoke", "IOSSmoke", "CatalystSmoke", "Packages", "Developer", "Release") or len(sys.argv) > 2 and sys.argv[1] != "AndroidSmoke":
        sys.exit("Unknown suite. Use Source, Build, Targets, WindowsSmoke, AndroidSmoke, LinuxSmoke, MacOSSmoke, IOSSmoke, CatalystSmoke, Packages, Developer, or Release.")
    sys.exit(main(sys.argv[1], sys.argv[2:]) or 0)
