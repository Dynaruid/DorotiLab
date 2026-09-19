"""Measure captured product edges, independently of the Android radius implementation."""
import argparse
import json
from pathlib import Path
from PIL import Image

parser = argparse.ArgumentParser()
parser.add_argument('directory', type=Path)
parser.add_argument('--scale', type=float, required=True)
parser.add_argument('--row', type=int, required=True)
parser.add_argument('--edge', type=int, required=True)
parser.add_argument('--color-x', type=int, required=True)
parser.add_argument('--require-color-edge', action='store_true')
args = parser.parse_args()
measurements = {}
for name, expected in [('native-zero', 0), ('native-4', 4), ('native-16', 16), ('raster-4', 4), ('raster-16', 16), ('native-reset', 0)]:
    image = Image.open(args.directory / (name + '.png')).convert('RGB')
    start = max(0, args.edge - int(50 * args.scale))
    end = min(image.width, args.edge + int(50 * args.scale))
    samples = [image.getpixel((x, args.row))[0] for x in range(start, end)]

    def crossing(threshold):
        for index in range(1, len(samples)):
            if samples[index - 1] < threshold <= samples[index]:
                return start + index - 1 + (threshold - samples[index - 1]) / (samples[index] - samples[index - 1])
        raise AssertionError((name, 'No threshold crossing', threshold))

    sigma = (crossing(229.5) - crossing(25.5)) / 2.563103 / args.scale
    measurements[name] = {'measuredLogicalSigma': sigma, 'requested': expected}
    assert sigma < .8 if expected == 0 else abs(sigma - expected) / expected < .2, (name, sigma, expected)

colors = {}
for name, expected in [('color-one', (180, 80, 60)), ('color-zero', (100, 100, 100)),
                       ('color-two', (255, 60, 20)), ('color-tint', (90, 40, 158)), ('color-reset', (180, 80, 60))]:
    actual = Image.open(args.directory / (name + '.png')).convert('RGB').getpixel((args.color_x, args.row))
    assert max(abs(a - b) for a, b in zip(actual, expected)) <= 3, (name, actual)
    colors[name] = actual
result = {'status': 'PASS', 'scope': 'captured product Gaussian edge, saturation, tint and reset; physical display not verified',
          'measurements': measurements, 'colors': colors, 'scale': args.scale, 'row': args.row}
edge_file = args.directory / 'color-edge.png'
if args.require_color_edge:
    assert edge_file.exists(), 'Final Android calibration must include the combined color/blur edge.'
if edge_file.exists():
    edge = Image.open(edge_file).convert('RGB')
    middle = [(edge.getpixel((args.edge - 1, args.row))[c] + edge.getpixel((args.edge, args.row))[c]) / 2 for c in range(3)]
    # Average source is (120,80,120), then apply the common saturation-2 matrix.
    luminance = .2126 * 120 + .7152 * 80 + .0722 * 120
    expected = [240 - luminance, 160 - luminance, 240 - luminance]
    assert max(abs(a - b) for a, b in zip(middle, expected)) <= 5, ('color after blur', middle, expected)
    zero = Image.open(args.directory / 'color-edge-zero.png').convert('RGB')
    for x, color in [(args.edge - 10, (255, 60, 20)), (args.edge + 10, (37, 77, 255))]:
        actual = zero.getpixel((x, args.row))
        assert max(abs(a - b) for a, b in zip(actual, color)) <= 3, ('color edge reset', actual, color)
    result['colorAfterBlur'] = {'middle': middle, 'expected': expected}
(args.directory / 'pixels.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
print(json.dumps(result, indent=2))
