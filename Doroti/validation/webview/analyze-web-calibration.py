"""Validate actual browser screenshots; does not certify physical scanout or other browsers."""
import json
from pathlib import Path
import sys

import numpy as np
from PIL import Image

root = Path(sys.argv[1])
geometry = json.loads((root / 'geometry.json').read_text())['geometry']
x = round(geometry['x'] + geometry['width'] / 2)
y = round(geometry['y'] + 230)


def pixels(name):
    return np.array(Image.open(root / f'{name}.png').convert('RGB'), dtype=float)


measurements = []
for source in ['native', 'raster']:
    for sigma in [4, 16]:
        profile = pixels(f'{source}-{sigma}')[y-5:y+5, x-75:x+75].mean(axis=(0, 2)) / 255
        positions = np.arange(-75, 75)
        measured = (np.interp(.9, profile, positions) - np.interp(.1, profile, positions)) / 2.563103
        assert abs(measured - sigma) <= sigma * .08, (source, sigma, measured)
        measurements.append(dict(source=source, requestedSigma=sigma, measuredSigma=float(measured)))
    difference = float(abs(pixels(f'{source}-0') - pixels(f'{source}-reset')).max())
    assert difference == 0, (source, difference)
    measurements.append(dict(source=source, resetMaximumDifference=difference))

if (root / 'cross-origin-4.png').exists():
    profile = pixels('cross-origin-4')[y-5:y+5, x-75:x+75].mean(axis=(0, 2)) / 255
    measured = (np.interp(.9, profile, np.arange(-75, 75)) - np.interp(.1, profile, np.arange(-75, 75))) / 2.563103
    assert abs(measured - 4) <= .32, measured
    measurements.append(dict(source='cross-origin iframe', requestedSigma=4, measuredSigma=float(measured)))


def sample(name):
    return pixels(name)[y-5:y+5, x-65:x-45].mean(axis=(0, 1))


tint = sample('blue-tint')
assert np.max(abs(tint - [0, 0, 128])) <= 2, tint
gray = sample('saturation-zero')
assert max(gray) - min(gray) <= 1 and 50 <= gray[0] <= 58, gray
assert np.max(abs(sample('live-red') - [255, 0, 0])) <= 2
assert np.max(abs(sample('live-blue') - [0, 0, 255])) <= 2
result = dict(status='PASS', measurements=measurements, halfBlueTint=tint.tolist(),
              saturationZero=gray.tolist(), liveSource='red and blue updated without a Doroti raster repaint',
              scope='browser-composited screenshots', physical='notVerified')
(root / 'pixels.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
print(json.dumps(result))
