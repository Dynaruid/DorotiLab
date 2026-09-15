"""Verify the default gallery's native page stays visible while its spinner animates."""
import json
import sys
from pathlib import Path

from PIL import Image, ImageChops, ImageStat


directory = Path(sys.argv[1])
name = sys.argv[2] if len(sys.argv) > 2 else "embedded-native"
evidence = json.loads((directory / f"{name}.json").read_text(encoding="utf-8-sig"))
scale = evidence["dpi"] / 96
images = [Image.open(directory / f"{name}.json.bmp").convert("RGB")]
images += [Image.open(directory / f"{name}.json.bmp.{index}.bmp").convert("RGB") for index in range(1, 30)]
assert all(image.size == images[0].size for image in images), "Capture geometry changed"
editor_hwnd = next(item["hwnd"] for item in evidence["winUi"] if item["type"].endswith(".TextBox"))
editor = next(item for item in evidence["native"] if item["hwnd"] == editor_hwnd)
button = next(item for item in evidence["native"] if item["hwnd"] != editor_hwnd)


def physical(bounds, dx=0, dy=0):
    return tuple(round((value + (dx if index % 2 == 0 else dy)) * scale) for index, value in enumerate(bounds))


editor_box = physical(editor["bounds"], editor["transform"]["Dx"], editor["transform"]["Dy"])
effect_box = tuple(round(value) for value in evidence["winUiBackdrop"]["bounds"][0]["clip"])
spinner_box = physical((50, 50, 86, 86), button["transform"]["Dx"], button["transform"]["Dy"])


def maximum_difference(box):
    reference = images[0].crop(box)
    return max(sum(ImageStat.Stat(ImageChops.difference(reference, image.crop(box))).mean) / 3 for image in images[1:])


editor_difference = maximum_difference(editor_box)
effect_difference = maximum_difference(effect_box)
spinner_difference = maximum_difference(spinner_box)
assert editor_difference < 1, f"Native editor disappeared or flickered: {editor_difference}"
assert effect_difference < 1, f"Backdrop disappeared or flickered: {effect_difference}"
assert spinner_difference > 0.2, "No live animation observed; frozen/repeated frames cannot establish continuity"
assert evidence["commits"] > 30 and evidence["winUiPlacementBatches"] <= 3, "Stable frames repeatedly mutated HWND placement"
images[0].save(directory / "embedded-native-stable.png")
result = {
    "result": "PASS", "captureFrames": len(images),
    "editorMaximumMeanAbsoluteDifference": editor_difference,
    "backdropMaximumMeanAbsoluteDifference": effect_difference,
    "spinnerMaximumMeanAbsoluteDifference": spinner_difference,
    "commits": evidence["commits"], "placementBatches": evidence["winUiPlacementBatches"],
    "scope": "Default gallery -> Material sample -> native page; no pointer/IME or physical scan-out qualification",
}
(directory / "continuity.json").write_text(json.dumps(result, indent=2) + "\n", encoding="utf-8")
print(json.dumps(result, indent=2))
