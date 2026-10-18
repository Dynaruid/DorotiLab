"""Install/upgrade/remove/purge a real package ONLY in a disposable runtime.

Use docker/podman (Debian 13 image) or a disposable Debian rootfs under
temp/testing/linux-deb with bwrap. SDK/compiler/Qt development files are never
mounted into the runtime. GUI smoke is opt-in and uses ordinary UID 1000.
"""
import argparse
import hashlib
import json
import os
from pathlib import Path
import shutil
import subprocess
import sys
import uuid

ROOT = Path(__file__).resolve().parents[2]


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--v1", type=Path, required=True, help="v1 receipt.json")
    parser.add_argument("--v2", type=Path, required=True, help="v2 receipt.json")
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--engine", choices=("docker", "podman", "bwrap"))
    parser.add_argument("--image", default="debian:trixie-slim", help="Pin by digest for repeatable acceptance")
    parser.add_argument("--rootfs", type=Path, help="bwrap only: a disposable Debian 13 filesystem")
    parser.add_argument("--gui", action="store_true", help="xcb/Xvfb first-frame smoke; Wayland remains separate")
    parser.add_argument("--wayland", action="store_true", help="Also test an isolated Debian Weston headless compositor")
    args = parser.parse_args()
    output = args.output.resolve()
    if not output.is_relative_to(ROOT / "temp/testing/linux-deb"):
        parser.error("Keep lifecycle evidence in temp/testing/linux-deb/<run>/")
    output.mkdir(parents=True, exist_ok=True)
    report = dict(status="partial", install="not-run", upgrade="not-run", remove="not-run", purge="not-run", gui="not-run", wayland="not-run", menuClick="not-run")
    container = None
    engine = args.engine or next((tool for tool in ("docker", "podman") if shutil.which(tool)), None)
    try:
        if not engine or not shutil.which(engine):
            report["reason"] = "No disposable container engine; use docker/podman or --engine bwrap --rootfs PATH"
            print(report["reason"])
            return 2
        if args.wayland and not args.gui:
            parser.error("--wayland requires --gui")
        receipts = [json.loads(path.read_text()) for path in (args.v1, args.v2)]
        first, second = [receipt["metadata"] for receipt in receipts]
        assert first["packageId"] == second["packageId"] and first["applicationId"] == second["applicationId"]
        subprocess.run(["dpkg", "--compare-versions", first["debianVersion"], "lt", second["debianVersion"]], check=True)
        files = [path.resolve().parent / receipt["package"] for path, receipt in zip((args.v1, args.v2), receipts)]
        for path, receipt in zip(files, receipts):
            with path.open("rb") as stream:
                assert hashlib.file_digest(stream, "sha256").hexdigest() == receipt["sha256"]
        if engine == "bwrap":
            if not args.rootfs:
                parser.error("bwrap requires --rootfs")
            rootfs = args.rootfs.resolve()
            if not rootfs.is_relative_to(ROOT / "temp/testing/linux-deb") or rootfs.is_symlink():
                parser.error("Disposable rootfs must be in temp/testing/linux-deb")
            release = (rootfs / "etc/os-release").read_text()
            if 'ID=debian' not in release or 'VERSION_ID="13"' not in release:
                parser.error("Expected Debian 13 disposable rootfs")
            # The single-UID namespace cannot drop to _apt. This affects only
            # the disposable environment, and leaves the real host untouched.
            (rootfs / "etc/apt/apt.conf.d/00doroti-test").write_text('APT::Sandbox::User "root";\n')
            (rootfs / "home/doroti").mkdir(parents=True, exist_ok=True)
            with (rootfs / "etc/passwd").open("a") as stream:
                stream.write("doroti:x:1000:1000:Doroti test:/home/doroti:/bin/sh\n")
        else:
            container = "doroti-deb-" + uuid.uuid4().hex
            create = [engine, "run", "-d", "--name", container, "--platform", "linux/amd64",
                      "-v", str(output) + ":/evidence"]
            for index, path in enumerate(files):
                create += ["-v", str(path) + f":/v{index + 1}.deb:ro"]
            subprocess.run([*create, args.image, "sleep", "infinity"], check=True, stdout=subprocess.DEVNULL)

        def run(name, arguments, uid=0, environment=None, success=True):
            values = {"HOME": "/root" if uid == 0 else "/home/doroti", "PATH": "/usr/sbin:/usr/bin:/sbin:/bin",
                      "LC_ALL": "C", "DEBIAN_FRONTEND": "noninteractive", **(environment or {})}
            if engine == "bwrap":
                entry = ["bwrap", "--die-with-parent", "--unshare-user", "--uid", str(uid), "--gid", str(uid),
                         "--bind" if uid == 0 else "--ro-bind", str(rootfs), "/", "--proc", "/proc", "--dev-bind", "/dev", "/dev",
                         "--ro-bind", "/sys", "/sys", "--ro-bind", "/etc/resolv.conf", "/etc/resolv.conf",
                         "--bind", str(output), "/evidence", "--chdir", "/tmp"]
                for index, path in enumerate(files):
                    entry += ["--ro-bind", str(path), f"/v{index + 1}.deb"]
                if uid:
                    entry += ["--bind", str(rootfs / "home/doroti"), "/home/doroti", "--tmpfs", "/tmp"]
                for key in ("DOTNET_ROOT", "DOTNET_ROOT_X64", "LD_LIBRARY_PATH", "QT_PLUGIN_PATH", "QML2_IMPORT_PATH", "XDG_DATA_HOME", "XDG_CACHE_HOME", "XDG_RUNTIME_DIR", "WAYLAND_DISPLAY", "DISPLAY", "QTWEBENGINE_DISABLE_SANDBOX"):
                    entry += ["--unsetenv", key]
                for key, value in values.items():
                    entry += ["--setenv", key, value]
                entry += ["--", *arguments]
            else:
                entry = [engine, "exec", "--user", f"{uid}:{uid}"]
                for key, value in values.items():
                    entry += ["-e", key + "=" + value]
                entry += [container, *arguments]
            try:
                completed = subprocess.run(entry, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True, timeout=90 if uid else 600)
            except subprocess.TimeoutExpired as error:
                partial = error.stdout or b""
                (output / (name + ".log")).write_text(partial.decode(errors="replace") if isinstance(partial, bytes) else partial)
                raise
            (output / (name + ".log")).write_text(completed.stdout)
            if success and completed.returncode:
                raise RuntimeError(name + " failed:\n" + completed.stdout[-8000:])
            return completed.stdout

        run("apt-update", ["apt-get", "update"])
        run("install-v1", ["apt-get", "install", "-y", "--no-install-recommends", "/v1.deb"])
        binary = "/opt/" + first["packageId"] + "/" + first["executable"] + ".dll"
        if run("v1-binary", ["sha256sum", binary]).split()[0] != receipts[0]["payload"][first["executable"] + ".dll"]:
            raise RuntimeError("Installed v1 payload hash mismatch")
        report["install"] = "passed"
        if engine != "bwrap":
            run("user", ["useradd", "-m", "-u", "1000", "doroti"])
        packages = run("installed-packages", ["dpkg-query", "-W", "-f=${Package}\t${Version}\n"])
        if any(value.split("\t")[0].endswith("-dev") or value.startswith(("dotnet-sdk", "cmake\t", "gcc\t", "g++\t")) for value in packages.splitlines()):
            raise RuntimeError("Runtime contains developer packages")
        run("no-sdk", ["sh", "-c", "test ! -e /dotnet && ! command -v dotnet && ! command -v cmake && ! command -v gcc"])
        identity = hashlib.sha256(first["applicationId"].encode()).hexdigest().upper()
        marker = "/home/doroti/.local/share/Doroti/" + identity + "/navigation/keep.txt"
        run("create-data", ["sh", "-c", 'mkdir -p "$(dirname "$1")" && printf retained > "$1"', "sh", marker], uid=1000)

        def gui(name, qpa="xcb"):
            environment = {"QT_QPA_PLATFORM": qpa, "DOROTI_QT_DIAGNOSTICS": "1", "DOROTI_QT_VALIDATION_RESIZE_CYCLES": "10",
                           "DOROTI_TESTBED_WEBVIEW_PAGE_PROBE": "1" if first["webView"] else "0"}
            if qpa == "wayland":
                environment.update(WAYLAND_DISPLAY="wayland-deb", XDG_RUNTIME_DIR="/tmp/doroti-weston")
            wayland_script = '''set -eu
mkdir -p "$XDG_RUNTIME_DIR"
chmod 700 "$XDG_RUNTIME_DIR"
weston --backend=headless --renderer=pixman --socket=wayland-deb --idle-time=0 --log=/tmp/weston.log >/tmp/weston-console 2>&1 &
compositor=$!
trap 'kill "$compositor" 2>/dev/null || true; wait "$compositor" || true; cat /tmp/weston.log /tmp/weston-console' EXIT
attempt=0
while [ ! -S "$XDG_RUNTIME_DIR/wayland-deb" ]; do
  attempt=$((attempt + 1))
  if [ "$attempt" -ge 100 ]; then exit 1; fi
  sleep 0.1
done
"$1"
'''
            try:
                arguments = ["xvfb-run", "-a", "/usr/bin/" + first["packageId"]] if qpa == "xcb" else ["sh", "-c", wayland_script, "sh", "/usr/bin/" + first["packageId"]]
                log = run(name, arguments, uid=1000, environment=environment)
                summary = json.loads(next(value.split("=", 1)[1] for value in log.splitlines() if value.startswith("doroti.qt.summary=")))
                if summary["frames"]["presented"] <= 0 or summary["frames"]["failed"] != 0:
                    raise RuntimeError("Installed app did not present a valid first frame")
                if first["webView"] and summary.get("compositionFrames", 0) <= 0:
                    raise RuntimeError("WebView probe did not compose a native view")
                return summary
            except (RuntimeError, subprocess.SubprocessError, ValueError, KeyError, StopIteration) as error:
                report.setdefault("guiFailures", []).append(str(error))
                return None

        if args.gui:
            run("gui-runtime", ["apt-get", "install", "-y", "--no-install-recommends", "xvfb", "xauth", "mesa-vulkan-drivers", *(["weston"] if args.wayland else [])])
            report["guiV1"] = gui("gui-v1")
            if args.wayland:
                report["waylandV1"] = gui("wayland-v1", "wayland")
        run("upgrade-v2", ["apt-get", "install", "-y", "--no-install-recommends", "/v2.deb"])
        version = run("installed-version", ["dpkg-query", "-W", "-f=${Version}", first["packageId"]]).strip()
        if version != second["debianVersion"]:
            raise RuntimeError("Upgrade did not replace the package version")
        if run("v2-binary", ["sha256sum", binary]).split()[0] != receipts[1]["payload"][second["executable"] + ".dll"]:
            raise RuntimeError("Installed v2 payload hash mismatch")
        run("upgrade-data", ["sh", "-c", 'test "$(cat "$1")" = retained', "sh", marker], uid=1000)
        report["upgrade"] = "passed"
        if args.gui:
            report["guiV2"] = gui("gui-v2")
            report["gui"] = "passed-xcb-xvfb" if report["guiV1"] and report["guiV2"] else "failed"
            if args.wayland:
                report["waylandV2"] = gui("wayland-v2", "wayland")
                report["wayland"] = "passed" if report["waylandV1"] and report["waylandV2"] else "failed"
        owned_files = run("owned-files", ["dpkg-query", "-L", first["packageId"]]).splitlines()
        run("remove", ["apt-get", "remove", "-y", first["packageId"]])
        run("remove-data", ["sh", "-c", 'test "$(cat "$1")" = retained', "sh", marker], uid=1000)
        report["remove"] = "passed"
        # A package without conffiles disappears from dpkg's status database
        # after remove; reinstall to test a real purge of an installed package.
        run("reinstall-for-purge", ["apt-get", "install", "-y", "--no-install-recommends", "/v2.deb"])
        run("purge", ["apt-get", "purge", "-y", first["packageId"]])
        run("purge-data", ["sh", "-c", 'test "$(cat "$1")" = retained', "sh", marker], uid=1000)
        removed_paths = ["/opt/" + first["packageId"], "/usr/bin/" + first["packageId"], "/usr/share/applications/" + first["applicationId"] + ".desktop", "/usr/share/doc/" + first["packageId"]]
        removed_paths += [path for path in owned_files if path.startswith("/usr/share/icons/") and path.endswith("/" + first["applicationId"] + ".png")]
        for path in removed_paths:
            run("removed-" + Path(path).name, ["test", "!", "-e", path])
        report.update(purge="passed", userData="preserved", status=("failed" if report.get("guiFailures") else "passed") if args.gui else "partial", image=args.image if engine != "bwrap" else str(rootfs))
        print(json.dumps(report, indent=2))
        return (1 if report.get("guiFailures") else 0) if args.gui else 2
    except (OSError, RuntimeError, AssertionError, subprocess.SubprocessError) as error:
        report.update(status="failed", reason=str(error))
        print(str(error), file=sys.stderr)
        return 1
    finally:
        (output / "lifecycle.json").write_text(json.dumps(report, indent=2))
        if container:
            subprocess.run([engine, "rm", "-f", container], stdout=subprocess.DEVNULL, check=False)


if __name__ == "__main__":
    sys.exit(main())
