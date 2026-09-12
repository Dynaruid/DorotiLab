"""Bounded Graphite lifecycle probe for an already installed Android app.

Run via validation/run-with-timeout.py. Does not clear app data/logcat. Restores
the device's rotation policy. Captures are automated evidence, not physical input
or synchronization-validation/performance qualification.
"""
import argparse
import hashlib
import json
from pathlib import Path
import re
import subprocess
import time
import zipfile


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--serial", required=True)
    parser.add_argument("--package", default="dev.doroti.testbed")
    parser.add_argument("--output", required=True, type=Path)
    args = parser.parse_args()
    out = args.output.resolve()
    out.mkdir(parents=True, exist_ok=True)
    commands = []

    def adb(*parts, check=True):
        command = ["adb", "-s", args.serial, *map(str, parts)]
        start = time.monotonic()
        result = subprocess.run(command, capture_output=True, timeout=1200)
        commands.append(dict(command=command, exitCode=result.returncode,
                             elapsedSeconds=time.monotonic() - start))
        (out / "commands.json").write_text(json.dumps(commands, indent=2), encoding="utf-8")
        if check and result.returncode:
            raise RuntimeError(result.stderr.decode(errors="replace"))
        return result.stdout

    def text(*parts):
        return adb(*parts).decode(errors="replace").strip()

    component = text("shell", "cmd", "package", "resolve-activity", "--brief", args.package).splitlines()[-1]
    if not component.startswith(args.package + "/"):
        raise RuntimeError("Launcher activity not found")
    rotation = text("shell", "wm", "user-rotation").split()
    if not rotation or rotation[0] not in ("free", "lock"):
        raise RuntimeError("Cannot preserve device rotation policy")
    since = text("shell", "date", "+%s.%N")
    rows = []
    pid = None

    def capture(label):
        nonlocal pid
        current = text("shell", "pidof", args.package)
        if pid is not None and current != pid:
            raise RuntimeError("Process changed during lifecycle probe")
        pid = current
        (out / (label + ".png")).write_bytes(adb("exec-out", "screencap", "-p"))
        remote = "/sdcard/doroti-graphite-" + str(time.time_ns()) + ".xml"
        adb("shell", "uiautomator", "dump", remote)
        xml = adb("shell", "cat", remote)
        adb("shell", "rm", remote)
        (out / (label + ".xml")).write_bytes(xml)
        if ('package="' + args.package + '"').encode() not in xml:
            raise RuntimeError("App is not foreground: " + label)
        rows.append(dict(label=label, pid=pid))
        print(label, pid, flush=True)

    summary = dict(status="FAIL", device=args.serial, package=args.package, automated=True,
                   timeoutSeconds=1200, rotationBefore=rotation, cases=rows)
    try:
        adb("shell", "am", "force-stop", args.package)
        adb("shell", "wm", "user-rotation", "lock", "0")
        adb("shell", "am", "start", "-W", "-n", component, "--es", "DOROTI_MAUI_EVIDENCE", "1")
        time.sleep(4)
        capture("portrait")
        for value, label in ((1, "landscape"), (3, "reverse-landscape"), (0, "portrait-restored")):
            adb("shell", "wm", "user-rotation", "lock", value)
            time.sleep(2)
            capture(label)
        for index in range(2):
            adb("shell", "input", "keyevent", "KEYCODE_HOME")
            time.sleep(2)
            adb("shell", "am", "start", "-W", "-n", component)
            time.sleep(3)
            capture("resume-" + str(index + 1))
        log = adb("logcat", "-d", "-T", since, "--pid=" + pid).decode(errors="replace")
        (out / "process.log").write_text(log, encoding="utf-8")
        if re.search(r"FATAL EXCEPTION|Fatal signal|Surface retirement failed| E DorotiGraphite| E/DorotiGraphite", log):
            raise RuntimeError("Renderer/process failure; inspect process.log")
        if "DorotiGraphite official package=SkiaSharp.NativeAssets.Android" not in log:
            raise RuntimeError("No official asset runtime provenance")
        drained = re.findall(r"drained submitted=(\d+) completed=(\d+) outstanding=(\d+)", log)
        if len(drained) < 2 or any(a != b or c != "0" for a, b, c in drained):
            raise RuntimeError("Missing or unbalanced GPU retirement")
        paths = text("shell", "pm", "path", args.package)
        (out / "installed-paths.txt").write_text(paths, encoding="utf-8")
        expected = {"arm64-v8a": "63af1ec283b86965542bca1400ae446e6a179a7be5b6187e69aa5fa0bd49e180",
                    "x86_64": "9bb141c1ee6b40781044a4e13779c6f19a0b45554dc6030b5f1cf49b24f7e7b8"}
        abi = text("shell", "getprop", "ro.product.cpu.abi")
        assets = []
        for path in paths.splitlines():
            remote = path.removeprefix("package:")
            local = out / Path(remote).name
            adb("pull", remote, local)
            with zipfile.ZipFile(local) as archive:
                for entry in archive.infolist():
                    if entry.filename == f"lib/{abi}/libSkiaSharp.so":
                        assets.append(dict(apk=remote, entry=entry.filename,
                                           sha256=hashlib.sha256(archive.read(entry)).hexdigest()))
        if len(assets) != 1 or assets[0]["sha256"] != expected[abi]:
            raise RuntimeError("Installed APK set differs from official asset pin")
        summary.update(status="PASS-scoped-device-lifecycle", officialAssets=assets, drained=drained,
                       limits="No physical input, sync-validation, delayed GPU/device-loss or performance claim")
    finally:
        adb("shell", "wm", "user-rotation", *rotation)
        summary["rotationAfter"] = text("shell", "wm", "user-rotation")
        (out / "result.json").write_text(json.dumps(summary, indent=2), encoding="utf-8")
        (out / "all-process.log").write_bytes(adb("logcat", "-d", "-T", since))


if __name__ == "__main__":
    main()
