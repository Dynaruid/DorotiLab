"""Measure matching captured bodies; results do not declare pixel acceptance.

Requires Pillow and numpy. Pass the sample and Flutter test output directories,
then a new JSON output path. Both capture suites use an 800x900, DPR 1 viewport.
"""
import argparse
import json
from pathlib import Path

import numpy as np
from PIL import Image

parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument("sample", type=Path)
parser.add_argument("reference", type=Path)
parser.add_argument("output", type=Path)
args = parser.parse_args()
if args.output.exists():
    raise SystemExit("Refusing to replace historical evidence")
region = (0, 56, 800, 820)  # Content only; excludes branding and navigation chrome.
rows = []
for theme in ("light", "dark"):
    for screen in ("components", "color", "typography", "elevation"):
        name = f"{'dark-' if theme == 'dark' else ''}{screen}-800.png"
        sample, reference = args.sample / name, args.reference / ("reference-" + name)
        with Image.open(sample) as a, Image.open(reference) as b:
            if a.size != (800, 900) or b.size != a.size:
                raise ValueError(f"Incompatible capture dimensions for {name}")
            delta = np.abs(np.asarray(a.convert("RGB").crop(region), dtype=np.int16)
                           - np.asarray(b.convert("RGB").crop(region), dtype=np.int16))
        rows.append(dict(theme=theme, screen=screen, sample=str(sample), reference=str(reference),
                         region=region, meanAbsoluteChannelDifference=float(delta.mean()),
                         exactPixelFraction=float(np.all(delta == 0, axis=2).mean()),
                         over2ChannelFraction=float(np.any(delta > 2, axis=2).mean()),
                         over20ChannelFraction=float(np.any(delta > 20, axis=2).mean())))
result = dict(status="MEASURED_NOT_ACCEPTANCE",
              note="Body crop excludes branding/navigation; no region tolerance is declared PASS.", rows=rows)
args.output.write_text(json.dumps(result, indent=2) + "\n", encoding="utf-8")
for row in rows:
    print(f"{row['theme']} {row['screen']}: MAE={row['meanAbsoluteChannelDifference']:.3f}, "
          f"pixels over 2={row['over2ChannelFraction']:.2%}")
