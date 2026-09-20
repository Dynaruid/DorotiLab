"""Actual Qt product API or pixel gate, with sandbox enabled; use run-with-timeout.py."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import subprocess
from PIL import Image, ImageChops, ImageStat

p = argparse.ArgumentParser()
p.add_argument('--app', type=Path, required=True)
p.add_argument('--driver', type=Path, required=True)
p.add_argument('--output', type=Path, required=True)
p.add_argument('--qpa', choices=['xcb', 'wayland'], required=True)
p.add_argument('--clean-env', action='store_true')
p.add_argument('--mode', choices=['api', 'calibration'], default='api')
a = p.parse_args()
out = a.output.resolve()
out.mkdir(parents=True, exist_ok=False)
signal = out / 'evidence'
steps = [dict(action='wait', wait=1500)]
if a.mode == 'api':
    steps += [dict(action='waitFile', path=str(signal)), dict(action='capture', path=str(out/'final'))]
else:
    steps += [dict(action='calibration', path=str(signal), output=str(out))]
script = out/'script.json'
script.write_text(json.dumps(steps))
env = dict(os.environ, QT_QPA_PLATFORM=a.qpa, DOROTI_TESTBED_MODE='platform-effects',
           DOROTI_PLATFORM_VIEW_EVIDENCE=str(out), DOROTI_QT_TEST_SCRIPT=str(script),
           LD_PRELOAD=str(a.driver.resolve()), QSG_INFO='1', QT_LOGGING_RULES='qt.webenginecontext=true',
           XDG_DATA_HOME=str(out/'data'), XDG_CACHE_HOME=str(out/'cache'))
if a.clean_env:
    for variable in ('LD_LIBRARY_PATH','QML2_IMPORT_PATH','QML_IMPORT_PATH','QT_PLUGIN_PATH',
                     'QT_QPA_PLATFORM_PLUGIN_PATH','QTWEBENGINEPROCESS_PATH','QTWEBENGINE_RESOURCES_PATH','QTWEBENGINE_LOCALES_PATH'):
        env.pop(variable, None)
env['DOROTI_QT_WEBVIEW_EVIDENCE' if a.mode == 'api' else 'DOROTI_QT_EFFECT_CALIBRATION'] = str(signal)
if os.getuid() == 0 or env.get('QTWEBENGINE_DISABLE_SANDBOX') or '--no-sandbox' in env.get('QTWEBENGINE_CHROMIUM_FLAGS', ''):
    raise SystemExit('Product gate requires a normal user and Chromium sandbox.')
command = ['dotnet', str(a.app.resolve())]
result = dict(qpa=a.qpa, mode=a.mode, command=command, physical='notVerified', nativeAot='notVerified',
              sandbox='default', binaries={f.name: hashlib.sha256(f.read_bytes()).hexdigest()
              for f in a.app.parent.iterdir() if f.suffix in ('.so','.dll')})
try:
    with (out/'product.log').open('w') as log:
        run = subprocess.run(command, env=env, stdout=log, stderr=log, timeout=180)
    result['exit'] = run.returncode
    assert run.returncode == 0, f'exit={run.returncode}'
    log = (out/'product.log').read_text()
    assert 'VUID-' not in log and 'managed.fatal=' not in log and 'Unhandled exception' not in log, 'product/validation error'
    capture = out/('final.json' if a.mode == 'api' else 'native-zero.json')
    mapped = json.loads(capture.read_text()).get('nativeLibraries', [])
    assert str(a.app.resolve().parent/'libdoroti_qt_host.so') in mapped, ('wrong product native module', mapped)
    assert str(a.app.resolve().parent/'libdoroti_webview_qt.so') in mapped, ('wrong WebView shim', mapped)
    result['nativeLibraries'] = mapped
    if a.mode == 'api':
        evidence = signal.read_text()
        assert 'FAIL' not in evidence and 'PASS Linux Qt WebView product commands' in evidence, evidence
        result['passedChecks'] = evidence.count('PASS ')
    else:
        assert Path(str(signal)+'.done').read_text() == 'PASS'
        images = {}
        for file in out.glob('*.png'):
            image = Image.open(file).convert('RGB'); scale = image.width / 720
            # Same 620-wide source and upper blur strip as the cross-platform fixture.
            images[file.stem] = (image.crop(tuple(round(v*scale) for v in (270,240,470,248))), scale)
        measurements = {}
        for name in ('native-zero','native-4','native-16','raster-4','raster-16'):
            image, scale = images[name]
            values = [sum(image.getpixel((x,y))[0] for y in range(image.height))/image.height for x in range(image.width)]
            def crossing(level):
                for x in range(1,len(values)):
                    if values[x-1] <= level <= values[x] and values[x] > values[x-1]:
                        return x-1+(level-values[x-1])/(values[x]-values[x-1])
                raise AssertionError((name,level,min(values),max(values)))
            sigma = (crossing(229.5)-crossing(25.5))/2.563103/scale
            expected = 0 if name.endswith('zero') else int(name.split('-')[-1])
            measurements[name] = sigma
            assert sigma < .8 if expected == 0 else abs(sigma-expected)/expected < .2, (name,sigma)
        for before, after in [('native-zero','native-reset'),('color-one','color-reset')]:
            delta = max(ImageStat.Stat(ImageChops.difference(images[before][0],images[after][0])).mean)
            measurements[after] = delta
            assert delta < 1, (after,delta)
        colors = {name: ImageStat.Stat(images[name][0]).mean for name in ('color-one','color-zero','color-two','color-tint')}
        assert max(colors['color-zero'])-min(colors['color-zero']) < 3, colors
        assert max(colors['color-two'])-min(colors['color-two']) > 1.5*(max(colors['color-one'])-min(colors['color-one'])), colors
        tint = [v*127/255+t*128/255 for v,t in zip(colors['color-one'],(0,0,255))]
        assert max(abs(v-t) for v,t in zip(colors['color-tint'],tint)) < 5, colors
        result.update(measurements=measurements,colors=colors)
    result['status'] = 'PASS'
except Exception as error:
    result.update(status='FAIL', error=str(error))
finally:
    (out/'result.json').write_text(json.dumps(result,indent=2)+'\n')
print(json.dumps({k:v for k,v in result.items() if k!='binaries'},indent=2))
raise SystemExit(0 if result['status']=='PASS' else 1)
