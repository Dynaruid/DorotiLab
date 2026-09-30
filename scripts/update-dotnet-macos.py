#!/usr/bin/env python3
"""Install the pinned .NET 10/11 SDKs and Apple/MAUI workloads on macOS.

Run as your normal user; only installer/workload operations use sudo.
Version references (checked 2026-09-30):
https://github.com/dotnet/macios/releases/tag/dotnet-10.0.1xx-xcode27.0-10722
https://github.com/dotnet/macios/releases/tag/dotnet-11.0.1xx-rc1-12193
"""

import argparse
import hashlib
import json
import os
from pathlib import Path
import platform
import shlex
import shutil
import subprocess
import sys
import tempfile
import urllib.parse
import urllib.request


PROFILES = {
    "10": {
        "sdk": "10.0.401",
        "workloads": "10.0.401.1",
        "ios": "27.0.10722",
        "xcode": "27.0",
    },
    "11": {
        "sdk": "11.0.100-rc.1.26425.128",
        "workloads": "11.0.100-rc.1.26458.5",
        "ios": "26.5.12193-net11-rc.1",
        "xcode": "26.6",
    },
}
DOTNET_ROOT = Path("/usr/local/share/dotnet")
DOTNET = DOTNET_ROOT / "dotnet"
WORKLOAD_IDS = ("ios", "maui", "macos", "maccatalyst", "wasm-tools")
NUGET_SOURCE = "https://api.nuget.org/v3/index.json"


def run(command, cwd=None, capture=False):
    command = [str(part) for part in command]
    print("+ " + shlex.join(command), flush=True)
    # Do not inherit an IDE's SDK/MSBuild resolver override.
    env = os.environ.copy()
    for name in ("MSBuildSDKsPath", "DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR",
                 "DOTNET_MSBUILD_SDK_RESOLVER_SDKS_DIR",
                 "DOTNET_MSBUILD_SDK_RESOLVER_SDKS_VER", "DOTNET_HOST_PATH",
                 "DOTNET_ROLL_FORWARD", "DOTNET_ROLL_FORWARD_TO_PRERELEASE"):
        env.pop(name, None)
    env["DOTNET_ROOT"] = str(DOTNET_ROOT)
    result = subprocess.run(command, cwd=cwd, env=env, check=True,
                            text=True, stdout=subprocess.PIPE if capture else None)
    if capture:
        print(result.stdout.rstrip(), flush=True)
        return result.stdout.strip()
    return ""


def architecture():
    machine = platform.machine()
    if machine == "arm64":
        return "arm64"
    if machine == "x86_64":
        translated = subprocess.run(
            ["/usr/sbin/sysctl", "-in", "sysctl.proc_translated"],
            capture_output=True, text=True, check=False)
        if translated.stdout.strip() == "1":
            raise RuntimeError("Rosetta 터미널입니다. 네이티브 ARM64 터미널에서 실행하세요.")
        return "x64"
    raise RuntimeError("지원하지 않는 아키텍처: " + machine)


def installer_info(major, arch):
    url = f"https://builds.dotnet.microsoft.com/dotnet/release-metadata/{major}.0/releases.json"
    print("릴리스 메타데이터: " + url, flush=True)
    with urllib.request.urlopen(url, timeout=60) as response:
        metadata = json.load(response)
    expected = PROFILES[major]["sdk"]
    for release in metadata["releases"]:
        for sdk in release.get("sdks", [release.get("sdk", {})]):
            if sdk.get("version") != expected:
                continue
            for entry in sdk["files"]:
                if entry.get("rid") == f"osx-{arch}" and entry["name"].endswith(".pkg"):
                    parsed = urllib.parse.urlparse(entry["url"])
                    if parsed.scheme != "https" or parsed.hostname not in (
                        "builds.dotnet.microsoft.com", "download.visualstudio.microsoft.com",
                        "dotnetcli.azureedge.net",
                    ):
                        raise RuntimeError("Microsoft 다운로드 URL을 확인할 수 없습니다.")
                    return entry
    raise RuntimeError(f"공식 메타데이터에 SDK {expected} / osx-{arch} 설치 파일이 없습니다.")


def download_installer(entry, destination):
    print("SDK 다운로드: " + entry["url"], flush=True)
    digest = hashlib.sha512()
    with urllib.request.urlopen(entry["url"], timeout=120) as response, destination.open("wb") as output:
        while True:
            chunk = response.read(1024 * 1024)
            if not chunk:
                break
            output.write(chunk)
            digest.update(chunk)
    if digest.hexdigest().lower() != entry["hash"].lower():
        destination.unlink()
        raise RuntimeError("SDK SHA-512 검증 실패. 설치하지 않았습니다.")
    print("SDK SHA-512 검증 통과", flush=True)


def workload_command(profile):
    return ["/usr/bin/sudo", "-H", str(DOTNET), "workload", "install", *WORKLOAD_IDS,
            "--version", profile["workloads"], "--source", NUGET_SOURCE]


def update(major, arch, staging):
    profile = PROFILES[major]
    sdk_dir = DOTNET_ROOT / "sdk" / profile["sdk"]
    if not (sdk_dir / "dotnet.dll").is_file():
        entry = installer_info(major, arch)
        package = staging / f"dotnet-{profile['sdk']}.pkg"
        download_installer(entry, package)
        # Verify Apple's package trust check as well as Microsoft's file hash.
        run(["/usr/sbin/pkgutil", "--check-signature", package])
        run(["/usr/bin/sudo", "/usr/sbin/installer", "-pkg", package, "-target", "/"])
    else:
        print(f"SDK {profile['sdk']}는 이미 설치되어 있습니다. 워크로드를 업데이트합니다.", flush=True)

    selection = staging / f"net{major}"
    selection.mkdir()
    # dotnet resolves global.json from cwd, not from a supplied csproj path.
    # Do not set workloadVersion here: it conflicts with install --version.
    (selection / "global.json").write_text(json.dumps({"sdk": {
        "version": profile["sdk"], "rollForward": "disable", "allowPrerelease": major == "11",
    }}) + "\n")
    (selection / "NuGet.Config").write_text(
        '<configuration><packageSources><clear />'
        f'<add key="nuget.org" value="{NUGET_SOURCE}" />'
        '</packageSources></configuration>\n')
    selected = run([DOTNET, "--version"], cwd=selection, capture=True)
    if selected != profile["sdk"]:
        raise RuntimeError(f"SDK 선택 불일치: expected {profile['sdk']}, got {selected}")
    print(f"\n.NET {major} 업데이트 전 워크로드:", flush=True)
    run([DOTNET, "workload", "--info"], cwd=selection)
    run(workload_command(profile), cwd=selection)
    installed = run([DOTNET, "workload", "--version"], cwd=selection, capture=True)
    if installed != profile["workloads"]:
        raise RuntimeError(f"워크로드 검증 실패: expected {profile['workloads']}, got {installed}")
    ios_platform = ".".join(profile["ios"].split(".")[:2])
    pack = DOTNET_ROOT / "packs" / f"Microsoft.iOS.Sdk.net{major}.0_{ios_platform}" / profile["ios"]
    if not (pack / "Sdk" / "Sdk.props").is_file():
        raise RuntimeError(f"iOS SDK 설치를 확인할 수 없습니다: {pack}")
    run([DOTNET, "workload", "--info"], cwd=selection)
    print(f"PASS: .NET {major} SDK / workload set / iOS SDK 설치 확인\n", flush=True)


def main(argv=None):
    parser = argparse.ArgumentParser(
        description="macOS의 .NET 10/11 SDK와 iOS·MAUI·macOS·Mac Catalyst·WebAssembly 도구를 업데이트합니다.",
        epilog="""일반 사용자로 실행하세요. 설치 단계에서만 sudo 비밀번호를 요청합니다.
.NET 10: Xcode 27.0 / .NET 11 RC1: Xcode 26.6 필요.
프로젝트 global.json, MAUI PackageReference, Native AOT 프로필과 Xcode 선택은 수정하지 않습니다.
설치 후에도 프로젝트의 TFM/MAUI 버전은 지원 조합으로 맞춰야 합니다.
SDK/워크로드 설치 확인은 앱의 Native AOT 빌드·실행 검증과 별개입니다.""",
        formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--dotnet-version", choices=("10", "11", "all"), default="all")
    parser.add_argument("--dry-run", action="store_true", help="다운로드·설치 없이 대상 버전과 명령만 표시")
    args = parser.parse_args(argv)
    if platform.system() != "Darwin":
        parser.error("이 스크립트는 macOS 전용입니다.")
    if os.geteuid() == 0:
        parser.error("스크립트 전체에 sudo를 붙이지 말고 일반 사용자로 실행하세요.")
    arch = architecture()
    majors = ("10", "11") if args.dotnet_version == "all" else (args.dotnet_version,)
    print(f"설치 경로: {DOTNET_ROOT} / 아키텍처: {arch}", flush=True)
    for major in majors:
        p = PROFILES[major]
        print(f".NET {major}: SDK {p['sdk']}, workload {p['workloads']}, iOS {p['ios']}, Xcode {p['xcode']}")
    print(".NET 11 업데이트만으로 Xcode 27 호환성이 생기지는 않습니다.", flush=True)
    current = shutil.which("dotnet")
    if current and Path(current).resolve() != DOTNET.resolve():
        print(f"주의: PATH의 dotnet({current})과 업데이트 대상({DOTNET})이 다릅니다.", flush=True)
    if args.dry_run:
        for major in majors:
            print(f"\n.NET {major}: SDK가 없으면 Microsoft .pkg 다운로드 → SHA-512/서명 확인 → sudo installer")
            print("임시 폴더의 global.json으로 SDK를 정확히 선택한 뒤:")
            print("  " + shlex.join(workload_command(PROFILES[major])))
        print("\nDRY RUN 완료. 다운로드/설치/설정 변경을 하지 않았습니다.")
        return 0
    if DOTNET.exists():
        run([DOTNET, "--list-sdks"])
    # Preflight credentials before a potentially large download, but do not run
    # this Python process as root or write root-owned files into the repository.
    run(["/usr/bin/sudo", "-v"])
    with tempfile.TemporaryDirectory(prefix="doroti-dotnet-update-") as directory:
        for major in majors:
            update(major, arch, Path(directory))
    run([DOTNET, "--list-sdks"])
    print("\n업데이트 완료. VS Code를 다시 시작한 뒤 SDK/워크로드 선택을 확인하세요.")
    print("Xcode 27: .NET 10 + net10.0-ios27.0 + MAUI 10.0.110 이상을 사용하세요.")
    print(".NET 11 RC1: Xcode 26.6을 선택해야 합니다. 버전 검사 우회는 적용하지 않았습니다.")
    return 0


if __name__ == "__main__":
    try:
        sys.exit(main())
    except (OSError, ValueError, RuntimeError, subprocess.CalledProcessError) as error:
        print(f"\n실패: {error}\n이미 완료된 설치는 유지됩니다. 원인을 해결한 뒤 같은 명령으로 재실행하세요.", file=sys.stderr)
        sys.exit(1)
    except KeyboardInterrupt:
        print("\n사용자가 중단했습니다. 설치가 일부 완료되었을 수 있습니다.", file=sys.stderr)
        sys.exit(130)
