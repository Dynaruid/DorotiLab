"""Pixel proof from browser display captures, never a production frame readback."""
import json
import sys
from pathlib import Path
import numpy as np
from PIL import Image

root = Path(sys.argv[1])
tests = []
def image(name):
    return np.asarray(Image.open(root / (name + '.png')).convert('RGB')).astype(np.int16)
def check(ok, name):
    assert ok, name
    tests.append(name)
    print('PASS', name)
if '--input' in sys.argv:
    first = image('canvas-button')
    yy, xx = np.where((first[:,:,0]>245)&(first[:,:,1]<10)&(first[:,:,2]<10))
    check(len(xx)>8000, 'Canvas button uses the browser source')
    x,y=int(xx.min()),int(yy.min())
    crop=lambda a:a[y:y+180,x:x+320]
    check(np.array_equal(crop(image('freeze-button-a')),crop(image('freeze-button-b'))), 'Freeze button holds exact pixels')
    check(np.count_nonzero(crop(image('freeze-button-a'))!=crop(image('resume-button')))>100, 'Resume button updates pixels')
    result=json.loads((root/'result.json').read_text());result['status']='PASS';result['pixelTests']=tests
    (root/'result.json').write_text(json.dumps(result,indent=2))
    raise SystemExit(0)
first = image('first')
red = (first[:, :, 0] > 245) & (first[:, :, 1] < 10) & (first[:, :, 2] < 10)
yy, xx = np.where(red)
check(len(xx) > 8000, 'actual large GPU texture content')
x, y = int(xx.min()), int(yy.min())
crop = lambda a: a[y:y+180, x:x+320]
expected = [(255, 0, 0), (0, 255, 0), (0, 0, 255), (255, 255, 0)]
for (dx, dy), color in zip([(40, 25), (270, 25), (40, 150), (270, 150)], expected):
    check(np.max(np.abs(first[y+dy, x+dx] - color)) <= 2, 'upright quadrant ' + str(color))
background = first[y-5, x+160]
check(np.max(np.abs(first[y, x] - background)) <= 2, 'rounded clipping')
check(np.max(np.abs(first[y+90, x+160] - ((np.array([255, 0, 255])+background)/2))) <= 3,
      'premultiplied alpha without dark fringe')
check(np.array_equal(crop(first), crop(image('frozen'))), 'freeze holds exact displayed pixels')
check(np.count_nonzero(crop(first) != crop(image('resumed'))) > 100, 'resume selects the new frame')
opacity = image('opacity')
check(np.max(np.abs(opacity[y+25, x+40] - ((np.array([255, 0, 0])+background)/2))) <= 3, 'widget opacity')
check(np.count_nonzero(crop(image('resumed')) != crop(image('transform'))) > 2000, 'widget transform')
resized = image('resize')
check(np.max(np.abs(resized[y+25,x+159]-(191,64,0)))<=3, 'linear filtering while scaling')
for (dx, dy), color in zip([(40, 25), (270, 25), (40, 150), (270, 150)], expected):
    check(np.max(np.abs(resized[y+dy, x+dx] - color)) <= 2, 'resize preserves orientation ' + str(color))
for source in ['bitmap', 'frame', 'canvas', 'video']:
    check(np.count_nonzero(crop(image(source+'-a')) != crop(image(source+'-b'))) > 50, source+' changes pixels')
check(np.array_equal(crop(image('video-paused-a')), crop(image('video-paused-b'))), 'paused video is stable')
check(np.count_nonzero(crop(image('video-paused-a')) != crop(image('video-seek'))) > 50, 'paused seek changes pixels')
check(np.std(crop(image('camera')).astype(float)) > 20, 'synthetic camera visible')
if (root/'mixed-a.png').exists():
    check(np.count_nonzero(crop(image('mixed-a')) != crop(image('mixed-b'))) > 100, 'mixed WebView GPU texture refresh')
    mixed = image('mixed-b')
    check(np.max(np.abs(mixed[y+70, x+110] - (18,52,86))) <= 3, 'native iframe between raster slices')
    check(np.max(np.abs(mixed[y+85, x+165] - (255,0,0))) <= 3, 'GPU texture in front of native iframe')
if (root/'local-canvas.png').exists():
    local = image('local-canvas')
    for (dx, dy), color in zip([(40,25),(270,25),(40,150),(270,150)], expected):
        check(np.max(np.abs(local[y+dy,x+dx]-color)) <= 2, 'owner-local OffscreenCanvas '+str(color))
    check(np.array_equal(crop(local),crop(image('local-canvas-preserved'))), 'owner-local copy preserves backing')
if (root/'webcodecs.png').exists():
    codec = image('webcodecs')
    for (dx, dy), color in zip([(40,25),(270,25),(40,150),(270,150)], expected):
        check(np.max(np.abs(codec[y+dy,x+dx]-color)) <= 15, 'WebCodecs decoded quadrant '+str(color))
for source in ['bitmap','frame','canvas']:
    check(np.max(np.abs(image(source+'-a')[y+90,x+160]-((np.array([255,0,255])+background)/2))) <= 3, source+' alpha')
for dpr in [1.5, 2]:
    name = 'dpr-'+str(dpr)
    if not (root/(name+'.png')).exists(): continue
    scaled = image(name)
    mask = (scaled[:,:,0]>245)&(scaled[:,:,1]<10)&(scaled[:,:,2]<10)
    sy,sx = np.where(mask);ox,oy=int(sx.min()),int(sy.min())
    for (dx,dy),color in zip([(40,25),(270,25),(40,150),(270,150)],expected):
        check(np.max(np.abs(scaled[oy+round(dy*dpr),ox+round(dx*dpr)]-color))<=3,'DPR '+str(dpr)+' quadrant '+str(color))
result = {'status': 'PASS', 'checks': tests, 'bounds': [x,y,320,180], 'physicalPresentation': 'notVerified'}
(root/'pixels.json').write_text(json.dumps(result, indent=2))
if (root/'result.json').exists():
    result = json.loads((root/'result.json').read_text())
    result['status'] = 'PASS'; result['pixelChecks'] = len(tests)
    (root/'result.json').write_text(json.dumps(result, indent=2))
