"""Measure captured UIKit effect kernels/colour bias against same-window Gaussian references.
Requires Pillow. Input is the blur-calibration folder emitted by UIKitBlurCalibration.
"""
import argparse
import json
import math
from pathlib import Path
from PIL import Image, ImageChops, ImageStat

parser = argparse.ArgumentParser()
parser.add_argument('directory', type=Path)
args = parser.parse_args()
p = args.directory
geometry = dict(line.split('=') for line in (p/'geometry.txt').read_text().splitlines())
scale = float(geometry['scale'])
left, top = float(geometry['left']), float(geometry['top'])
width, height = int(geometry['width']), int(geometry['height'])

def crop(name):
    image = Image.open(p/name).convert('RGB')
    return image.crop(tuple(round(v*scale) for v in (left, top, left+width, top+height)))

def kernel(image):
    # Sample far from the top/bottom of the horizontal black/white split.
    profile = [sum(image.getpixel((round((x+.5)*scale), round(50*scale))))/3 for x in range(width)]
    black = sum(profile[30:80])/50
    white = sum(profile[240:290])/50
    if white-black < 5:
        return dict(sigma=None, black=black, white=white, fitError=None)
    normalized = [(v-black)/(white-black) for v in profile]
    candidates = []
    for k in range(1, 241):
        sigma = k/4
        error = sum((normalized[x] - .5*(1+math.erf((x+.5-160)/(math.sqrt(2)*sigma))))**2 for x in range(80, 240))/160
        candidates.append((error, sigma))
    error, sigma = min(candidates)
    return dict(sigma=sigma, black=black, white=white, fitError=error)

roi = tuple(round(v*scale) for v in (50, 35, 270, 265))
references = {int(f.stem.split('-')[1]): crop(f.name) for f in p.glob('reference-*.png')}
reference_kernels = {sigma: kernel(image) for sigma,image in references.items()}
assert all(abs(v['sigma']-s) <= 1.25 for s,v in reference_kernels.items()), reference_kernels
rows = []
for file in sorted(p.glob('*-Light.png')):
    style, fraction, _ = file.stem.rsplit('-',2)
    light = crop(file.name); dark = crop(file.name.replace('-Light.png','-Dark.png'))
    info = kernel(light)
    errors = {sigma: sum(ImageStat.Stat(ImageChops.difference(light.crop(roi), image.crop(roi))).mean)/3 for sigma,image in references.items()}
    info.update(style=style, fraction=float(fraction), themeMAE=sum(ImageStat.Stat(ImageChops.difference(light,dark)).mean)/3,
                referenceMAE=errors, closestReference=min(errors,key=errors.get))
    rows.append(info)
result = dict(referenceKernels=reference_kernels, measurements=rows,
              scope='Measured public UIKit interpolation on this device/OS; not an exact Gaussian guarantee')
(p/'analysis.json').write_text(json.dumps(result,indent=2))
for r in rows:
    print(f"{r['style']:30} f={r['fraction']:.2f} sigma={r['sigma']} black/white={r['black']:.1f}/{r['white']:.1f} themeMAE={r['themeMAE']:.2f} closest={r['closestReference']} MAE={min(r['referenceMAE'].values()):.2f}")
