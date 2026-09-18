"""Verify spatial Gaussian spread, color preservation and customization, not whitening.
Requires NumPy/Pillow; run through validation/run-with-timeout.py.
"""
import argparse
import json
import math
import io
from pathlib import Path
import numpy as np
from PIL import Image, ImageCms

parser = argparse.ArgumentParser()
parser.add_argument('directory', type=Path)
args = parser.parse_args()
root = args.directory

def pixels(name):
    meta = json.loads((root / (name + '.json')).read_text())
    x, y, width, height = meta['bounds']
    scale = meta['scale']
    image = Image.open(root / (name + '.png'))
    if image.info.get('icc_profile'):
        source = ImageCms.ImageCmsProfile(io.BytesIO(image.info['icc_profile']))
        image = ImageCms.profileToProfile(image, source, ImageCms.createProfile('sRGB'), outputMode='RGB')
    else:
        image = image.convert('RGB')
    roi = image.crop(tuple(round(v * scale) for v in (x, y, x + width, y + height)))
    return np.asarray(roi, dtype=float) / 255, scale

def profile(name):
    array, scale = pixels(name)
    return array[round(35*scale):round(60*scale)].mean(axis=(0,2)), scale

def linear(value):
    return np.where(value <= .04045, value/12.92, ((value+.055)/1.055)**2.4)

def fit(name, edge):
    observed, scale = profile(name)
    # Exclude the effect boundary. Both encodings are reported, and choose the
    # lower residual instead of assuming Core Image's working color space.
    selected = (np.arange(len(observed)) >= 10*scale) & (np.arange(len(observed)) < len(observed)-10*scale)
    x = (np.arange(len(observed))[selected] - edge) / scale
    best = None
    for encoding, values in [('sRGB', observed), ('linear-sRGB', linear(observed))]:
        y = values[selected]
        for sigma in np.arange(.25, 100.01, .25):
            cdf = np.asarray([.5*(1+math.erf(v/(sigma*math.sqrt(2)))) for v in x])
            matrix = np.column_stack([np.ones(len(x)), cdf])
            coefficients = np.linalg.lstsq(matrix, y, rcond=None)[0]
            residual = float(np.sqrt(np.mean((matrix @ coefficients - y)**2)))
            if best is None or residual < best['rmse']:
                best = dict(sigma=float(sigma), encoding=encoding, rmse=residual,
                            black=float(coefficients[0]), white=float(sum(coefficients)))
    return best

report = dict(kind='public Core Image spatial backdrop', edges={}, rasterEdges={})
failures = []
def check(value, reason):
    if not value:
        failures.append(reason)

baseline, scale = profile('edge-0')
report['zeroRadiusTransitionPixels'] = int(np.count_nonzero((baseline > .01) & (baseline < .99)))
check(report['zeroRadiusTransitionPixels'] <= 2, 'Zero radius must remove the filter, not preserve the previous blur')
edge = float(np.argmax(np.diff(baseline))) + .5
for sigma in [2,4,8,16,32,64]:
    measured = fit('edge-'+str(sigma), edge)
    report['edges'][sigma] = measured
    check(abs(measured['sigma']-sigma) <= max(.5,sigma*.08), f'WebView sigma {sigma} is {measured["sigma"]}')
    check(measured['rmse'] < .025, f'WebView edge is not Gaussian at {sigma}')
    if sigma <= 16:
        check(abs(measured['black']) < .025 and abs(measured['white']-1) < .025, f'Intrinsic tint/color bias at {sigma}')
raster, _ = profile('raster-edge-0')
report['zeroRasterTransitionPixels'] = int(np.count_nonzero((raster > .01) & (raster < .99)))
check(report['zeroRasterTransitionPixels'] <= 2, 'Zero radius must remove the filter over Metal rasters')
raster_edge = float(np.argmax(np.diff(raster))) + .5
for sigma in [4,16]:
    measured = fit('raster-edge-'+str(sigma), raster_edge)
    report['rasterEdges'][sigma] = measured
    check(abs(measured['sigma']-sigma) <= max(.5,sigma*.08), f'Metal raster sigma {sigma} is {measured["sigma"]}')
    check(measured['rmse'] < .025, f'Metal raster edge is not Gaussian at {sigma}')

colors = {}
for saturation in [0,1,2]:
    array, scale = pixels('saturation-'+str(saturation))
    # Uniform color left of the step, away from sharp foreground text.
    patch = array[round(35*scale):round(60*scale),round(25*scale):round(60*scale)]
    rgb = patch.mean(axis=(0,1))
    colors[saturation] = dict(rgb=rgb.tolist(), chroma=float(rgb.max()-rgb.min()))
report['saturation'] = colors
check(colors[0]['chroma'] < .015, 'Saturation zero must be grayscale')
check(colors[1]['chroma'] > .35, 'Saturation one must preserve source color')
report['sourceColorError'] = float(np.abs(np.asarray(colors[1]['rgb']) - np.asarray([220,70,50])/255).max())
check(report['sourceColorError'] < .015, 'Untinted blur must preserve a uniform source color')
check(colors[2]['chroma'] > colors[1]['chroma']+.05, 'Saturation two must increase chroma')
only, scale = pixels('saturation-only')
report['saturationOnlyChroma'] = float(np.ptp(only[round(35*scale):round(60*scale),round(25*scale):round(60*scale)].mean(axis=(0,1))))
check(report['saturationOnlyChroma'] < .015, 'Zero-radius saturation must be supported')
tint, scale = pixels('tint-green')
tint_rgb = tint[round(35*scale):round(60*scale),round(25*scale):round(60*scale)].mean(axis=(0,1))
# Keep the nominal linear-sRGB model as a diagnostic. Validate tint against
# an independent native sRGB CALayer overlay through the same display profile.
expected_linear = .6*linear(np.asarray(colors[1]['rgb']))+.4*np.asarray([0,1,0])
expected = np.where(expected_linear <= .0031308, expected_linear*12.92, 1.055*expected_linear**(1/2.4)-.055)
report['tintLinearSrgbModelError'] = float(np.abs(tint_rgb-expected).max())
reference, scale = pixels('tint-native-reference')
reference_rgb = reference[round(35*scale):round(60*scale),round(25*scale):round(60*scale)].mean(axis=(0,1))
report['tintNativeReferenceError'] = float(np.abs(tint_rgb-reference_rgb).max())
check(report['tintNativeReferenceError'] < .015, 'Authored tint must match independent native sRGB/alpha compositing')
check(float(np.abs(tint_rgb-np.asarray(colors[1]['rgb'])).mean()) > .05, 'Authored tint must visibly change the source')

first, _ = pixels('strength-0.375-9')
reset, _ = pixels('strength-0.375-12')
report['resetMae255'] = float(np.abs(first-reset).mean()*255)
check(report['resetMae255'] < 1, 'Radius reset must reproduce its previous image')
a, _ = pixels('live-a'); b, _ = pixels('live-b')
report['liveMae255'] = float(np.abs(a-b).mean()*255)
check(report['liveMae255'] > .1, 'Live WebView animation must update the filtered pixels')
a, _ = pixels('theme-dark'); b, _ = pixels('theme-light')
report['themeMae255'] = float(np.abs(a-b).mean()*255)
check(report['themeMae255'] < 1, 'Custom filter must not gain an appearance-dependent material tint')
sharp, scale = pixels('strength-1-11')
text = sharp[round(92*scale):round(115*scale),round(50*scale):round(275*scale)]
report['foregroundContrast'] = float(text.max()-text.min())
check(report['foregroundContrast'] > .5, 'Doroti foreground must remain sharp/high contrast')
report['foregroundEdgeGradient'] = float(max(np.abs(np.diff(text.mean(axis=2),axis=0)).max(), np.abs(np.diff(text.mean(axis=2),axis=1)).max()))
check(report['foregroundEdgeGradient'] > .15, 'Foreground letter edges must stay sharp at full blur')
report['passed'] = not failures
report['failures'] = failures
(root/'custom-pixels.json').write_text(json.dumps(report, indent=2))
print(json.dumps(report, indent=2))
raise SystemExit(1 if failures else 0)
