"""Verify native Camera2/MediaPlayer frame delivery; screenshots remain separate visual evidence."""
import io
import json
from pathlib import Path
import re
import subprocess
import time
import numpy as np
from PIL import Image

adb_command = ["adb", "-s", "emulator-5554"]
out = Path("Doroti/artifacts/textures/android")
out.mkdir(parents=True, exist_ok=True)
results = []


def adb(*args):
    return subprocess.check_output(adb_command + list(args), timeout=20)


def logs():
    return adb("logcat", "-d", "-s", "DorotiTexture:D", "DorotiTextureProbe:D", "DorotiGraphite:D", "AndroidRuntime:E").decode("utf-8", "replace")


def screenshot(name):
    data = adb("exec-out", "screencap", "-p")
    (out / (name + ".png")).write_bytes(data)
    return np.array(Image.open(io.BytesIO(data)).convert("RGB"))


try:
    for source in ["camera", "video"]:
        adb("shell", "am", "force-stop", "dev.doroti.testbed")
        adb("logcat", "-c")
        adb("shell", "am", "start", "-n", "dev.doroti.testbed/crc64c80c495bd333b69c.MainActivity",
            "--es", "doroti_testbed_mode", "texture-native", "--es", "doroti_texture_source", source)
        counts = []
        for _ in range(12):
            time.sleep(0.5)
            log = logs()
            if re.search(r" E (DorotiTexture|DorotiGraphite|AndroidRuntime):", log):
                raise AssertionError(log[:8000])
            counts = [tuple(map(int, row)) for row in re.findall(r"received=(\d+) drawn=(\d+) imported=(\d+) retired=(\d+)", log)]
            if counts and counts[-1][1] > 5:
                break
        if not counts or counts[-1][1] <= 5:
            raise AssertionError(f"{source}: no native frames delivered")
        first = screenshot(source + "-final-a")
        time.sleep(0.8)
        second = screenshot(source + "-final-b")
        time.sleep(0.3)
        log = logs()
        (out / (source + "-final.log")).write_text(log, encoding="utf-8")
        samples = [tuple(map(int, row)) for row in re.findall(r"received=(\d+) drawn=(\d+) imported=(\d+) retired=(\d+)", log)]
        assert len(samples) >= 2 and samples[-1][0] > samples[0][0]
        assert all(imported == retired and imported > 0 for _, _, imported, retired in samples)
        assert not re.search(r" E (DorotiTexture|DorotiGraphite|AndroidRuntime):", log)
        changed = int(np.count_nonzero(first != second))
        if source == "video":
            assert changed > 1000, "Video frames did not change on screen"
        results.append({"source": source, "pass": True, "lastCounters": samples[-1],
                        "changedScreenshotChannels": changed, "physicalDevice": "notVerified"})
        print(f"PASS {source}: received/drawn/imported/retired={samples[-1]}, changedChannels={changed}", flush=True)
finally:
    (out / "source-checks.json").write_text(json.dumps(results, indent=2), encoding="utf-8")
