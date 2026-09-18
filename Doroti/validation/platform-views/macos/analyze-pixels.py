"""Pixel checks for the AppKit material; these do not qualify common Gaussian fidelity."""
import argparse
import json
from pathlib import Path
import numpy as np
from PIL import Image

parser = argparse.ArgumentParser()
parser.add_argument('directory', type=Path)
args = parser.parse_args()

def pixels(path):
    metadata = json.loads(path.with_suffix('.json').read_text())
    x, y, width, height = metadata['bounds']
    scale = metadata['scale']
    image = Image.open(path).convert('RGB')
    return np.asarray(image.crop(tuple(round(v * scale) for v in (x, y, x + width, y + height))), dtype=float)

captures = {}
for path in args.directory.glob('strength-*.png'):
    strength = float(path.stem.split('-')[1])
    captures.setdefault(strength, []).append((path.name, pixels(path)))
assert set(captures) == {0, .25, .375, .75, 1}, 'Missing strengths'
base = captures[0][0][1]
# Exclude sharp foreground text at the center, compare a stable checkerboard area.
height, width, _ = base.shape
roi = (slice(int(height*.80), int(height*.90)), slice(int(width*.12), int(width*.8)))
report = {'kind': 'AppKit material opacity, not Gaussian sigma', 'strengths': {}}
for strength in sorted(captures):
    sample = captures[strength][0][1]
    checker = sample[roi]
    report['strengths'][strength] = dict(mae=float(np.abs(sample-base).mean()), checkerContrast=float(checker.std()))
assert all(report['strengths'][s]['mae'] > report['strengths'][p]['mae'] for p,s in zip([0,.25,.375,.75],[.25,.375,.75,1])), 'Strength pixels must change monotonically'
assert report['strengths'][1]['checkerContrast'] < report['strengths'][0]['checkerContrast'] * .2, 'Full material must suppress checker detail'
a, b = [item[1] for item in captures[.375]]
report['decreasingStrengthMae'] = float(np.abs(a-b).mean())
assert report['decreasingStrengthMae'] < 1, 'Returning to the same strength must preserve appearance'
live_a, live_b = pixels(args.directory/'live-a.png'), pixels(args.directory/'live-b.png')
report['liveSourceMae'] = float(np.abs(live_a-live_b).mean())
assert report['liveSourceMae'] > .1, 'Effect must track live native animation'
# The child is black raster text. At full material its local edge contrast must remain sharp.
full = captures[1][0][1]
center = full[int(height*.43):int(height*.58), int(width*.2):int(width*.85)]
report['foregroundRange'] = float(center.max()-center.min())
assert report['foregroundRange'] > 140, 'Sharp foreground text missing'
if (args.directory/'theme-light.png').exists():
    light, dark = pixels(args.directory/'theme-light.png'), pixels(args.directory/'theme-dark.png')
    report['themeEffectMae'] = float(np.abs(light-dark).mean())
    assert report['themeEffectMae'] < 1, 'Fixed source/material must remain stable across window appearances'
report['passed'] = True
(args.directory/'pixels.json').write_text(json.dumps(report, indent=2))
print(json.dumps(report, indent=2))
