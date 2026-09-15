"""Compare in-process WGC captures; the negative control omits only XAML samples."""
import json
import sys
from pathlib import Path

from PIL import Image, ImageChops, ImageDraw, ImageStat


def mae(first, second):
    return sum(ImageStat.Stat(ImageChops.difference(first, second)).mean) / 3


def gradient(image):
    image = image.convert("L")
    width, height = image.size
    horizontal = ImageChops.difference(image.crop((0, 0, width - 1, height)), image.crop((1, 0, width, height)))
    vertical = ImageChops.difference(image.crop((0, 0, width, height - 1)), image.crop((0, 1, width, height)))
    return (ImageStat.Stat(horizontal).mean[0] + ImageStat.Stat(vertical).mean[0]) / 2


directory = Path(sys.argv[1])
evidence = json.loads((directory / "platform-views.json").read_text(encoding="utf-8-sig"))
on = Image.open(directory / "platform-views.json.bmp").convert("RGB")
off = Image.open(directory / "platform-views-no-blur.json.bmp").convert("RGB")
raster_only = Image.open(directory / "platform-views-raster-only-probe.json.bmp").convert("RGB")
assert on.size == off.size == raster_only.size, "Capture dimensions differ"
clip = tuple(round(value) for value in evidence["winUiBackdrop"]["bounds"][0]["clip"])
left, top, right, bottom = clip
assert right - left >= 32 and bottom - top >= 32, "Effect region is too small to compare"
on_region, off_region, raster_region = (image.crop(clip) for image in (on, off, raster_only))
blur_difference = mae(on_region, off_region)
native_difference = mae(on_region, raster_region)
off_gradient = gradient(off_region)
gradient_ratio = gradient(on_region) / max(off_gradient, 0.001)
outside = (right + 8, top + 8, min(on.width, right + 100), bottom - 8)
assert outside[2] > outside[0], "Fixture needs a comparison strip outside the clip"
outside_difference = mae(on.crop(outside), off.crop(outside))
assert blur_difference > 1, f"Blur did not change rendered pixels: {blur_difference}"
assert gradient_ratio < 0.85, f"Blur did not soften edges: {gradient_ratio}"
assert native_difference > 0.5, f"WinUI pixels did not contribute to blur: {native_difference}"
assert outside_difference < 1, f"Blur escaped its clip: {outside_difference}"

details = (max(0, left - 120), max(0, top - 64), min(on.width, right + 150), min(on.height, bottom + 32))
width, height = details[2] - details[0], details[3] - details[1]
comparison = Image.new("RGB", (width * 3, height + 32), "#ffffff")
draw = ImageDraw.Draw(comparison)
for index, (label, source) in enumerate((("Blur OFF", off), ("Blur ON: live WinUI", on), ("Negative control: native samples omitted", raster_only))):
    draw.text((index * width + 8, 8), label, fill="#202020")
    comparison.paste(source.crop(details), (index * width, 32))
comparison.save(directory / "winui-blur-comparison.png")
on.save(directory / "winui-blur-on.png")
result = {
    "result": "PASS",
    "capture": "in-process Windows.Graphics.Capture; actual client pixels",
    "blurMeanAbsoluteDifference": blur_difference,
    "edgeGradientRatio": gradient_ratio,
    "nativeContributionMeanAbsoluteDifference": native_difference,
    "outsideClipMeanAbsoluteDifference": outside_difference,
    "physicalInput": "notVerified",
}
(directory / "pixels.json").write_text(json.dumps(result, indent=2) + "\n", encoding="utf-8")
print(json.dumps(result, indent=2))
