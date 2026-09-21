"""Bounded emulator acceptance for the opt-in native canvas fixture. Run through run-with-timeout.py."""
import io
import json
from pathlib import Path
import re
import subprocess
import time

import numpy as np
from PIL import Image

ADB = ["adb", "-s", "emulator-5554"]
OUT = Path("Doroti/artifacts/textures/android")
OUT.mkdir(parents=True, exist_ok=True)
checks = []


def adb(*args):
    return subprocess.check_output(ADB + list(args), timeout=20)


def capture(name):
    data = adb("exec-out", "screencap", "-p")
    (OUT / f"{name}.png").write_bytes(data)
    return np.array(Image.open(io.BytesIO(data)).convert("RGB"))


def check(name, value):
    checks.append({"check": name, "pass": bool(value)})
    print(("PASS " if value else "FAIL ") + name, flush=True)
    if not value:
        raise AssertionError(name)


def find_texture(pixels):
    delta = pixels.max(axis=2).astype(int) - pixels.min(axis=2)
    mask = (delta > 200) & (pixels.max(axis=2) > 230)
    ys, xs = np.nonzero(mask)
    if not len(xs):
        return None
    return int(xs.min()), int(ys.min()), int(xs.max()) + 1, int(ys.max()) + 1


def crop(pixels, rect):
    x0, y0, x1, y1 = rect
    return pixels[y0 + 12:y1 - 12, x0 + 12:x1 - 12]


def quadrants(pixels, rect):
    x0, y0, x1, y1 = rect
    colors = []
    for u, v in [(0.25, 0.25), (0.75, 0.25), (0.25, 0.75), (0.75, 0.75)]:
        colors.append(pixels[int(y0 + (y1-y0)*v), int(x0 + (x1-x0)*u)].astype(int))
    return all(np.max(np.abs(a-b)) < 12 for a, b in zip(colors, [[255,0,0], [0,255,0], [0,0,255], [255,255,0]]))


def buttons(pixels, bottom):
    r, g, b = [pixels[:, :, i] for i in range(3)]
    mask = (r > 75) & (r < 140) & (g > 50) & (g < 120) & (b > 135) & (b < 190)
    rows = np.flatnonzero(mask.sum(axis=1) > 100)
    rows = rows[rows > bottom]
    groups = np.split(rows, np.where(np.diff(rows) > 1)[0] + 1)
    result = []
    for group in groups:
        if len(group) < 30:
            continue
        y = int(group[len(group)//2])
        xs = np.flatnonzero(mask[y])
        result.append((int((xs.min()+xs.max())//2), y))
    return result


def tap(point):
    adb("shell", "input", "tap", str(point[0]), str(point[1]))


try:
    for _ in range(10):
        status = adb("logcat", "-d", "-s", "DorotiTextureProbe:I").decode("utf-8", "replace")
        if not re.search(r"source=canvas received=\d+ drawn=[1-9]\d*", status):
            time.sleep(0.5)
            continue
        first = capture("canvas-live")
        rect = find_texture(first)
        if rect is not None:
            break
        time.sleep(0.5)
    check("native texture becomes ready", "rect" in locals() and rect is not None)
    check("GPU canvas colors and top-left orientation", quadrants(first, rect))
    controls = buttons(first, rect[3])
    check("fixture controls found below texture", len(controls) == 2)
    tap(controls[0])
    time.sleep(0.4)
    frozen_a = capture("canvas-frozen-a")
    time.sleep(0.7)
    frozen_b = capture("canvas-frozen-b")
    check("freeze keeps displayed pixels while native producer runs", np.array_equal(crop(frozen_a, rect), crop(frozen_b, rect)))
    tap(controls[0])
    time.sleep(0.4)
    resumed_a = capture("canvas-resumed-a")
    time.sleep(0.7)
    resumed_b = capture("canvas-resumed-b")
    check("resume displays new native frames", np.count_nonzero(crop(resumed_a, rect) != crop(resumed_b, rect)) > 100)
    tap(controls[1])
    time.sleep(2)
    recreated = capture("canvas-recreated")
    check("producer and registration can be recreated", quadrants(recreated, rect))
    adb("shell", "input", "keyevent", "3")
    time.sleep(0.8)
    adb("shell", "am", "start", "-n", "dev.doroti.testbed/crc64c80c495bd333b69c.MainActivity")
    time.sleep(2)
    restored = capture("canvas-restored")
    check("surface returns after background/resume", quadrants(restored, rect))
    log = adb("logcat", "-d", "-s", "DorotiTexture:D", "DorotiTextureProbe:D", "DorotiGraphite:D", "AndroidRuntime:E").decode("utf-8", "replace")
    (OUT / "canvas-final.log").write_text(log, encoding="utf-8")
    check("no texture renderer or Android runtime errors", not re.search(r" E (DorotiTexture|DorotiGraphite|AndroidRuntime):", log))
    counts = [tuple(map(int, match)) for match in re.findall(r"imported=(\d+) retired=(\d+)", log)]
    check("GPU leases retire and counters progress", bool(counts) and all(a == b for a,b in counts) and max(a for a,b in counts) > 10)
finally:
    (OUT / "canvas-checks.json").write_text(json.dumps(checks, indent=2), encoding="utf-8")
