#!/usr/bin/env python3
"""Bounded Qt Desktop product checks. Run with validation/run-with-timeout.py."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import signal
import subprocess
import time

ROOT = Path(__file__).resolve().parents[3]
parser = argparse.ArgumentParser()
parser.add_argument('--app', type=Path, required=True)
parser.add_argument('--output', type=Path, required=True)
parser.add_argument('--qpa', choices=('wayland', 'xcb'), required=True)
parser.add_argument('--native-close', action='store_true')
parser.add_argument('--explicit', action='store_true')
args = parser.parse_args()
out = args.output.resolve()
out.mkdir(parents=True, exist_ok=False)
app = args.app.resolve()
probe = out / 'probe.json'
driver = out / 'libdesktop-driver.so'
flags = subprocess.check_output(['pkg-config', '--cflags', '--libs', 'Qt6Quick', 'Qt6Widgets'], text=True).split()
subprocess.run(['c++', '-std=c++20', '-shared', '-fPIC', '-pthread',
                str(Path(__file__).with_name('qt-driver.cpp')),
                '-I' + str(ROOT / 'DorotiTestbedApp/linux/native/include'),
                '-o', str(driver), *flags, '-ldl'], check=True, timeout=1200)
env = dict(os.environ, QT_QPA_PLATFORM=args.qpa, DOROTI_QT_DESKTOP_PROBE=str(probe),
           LD_PRELOAD=str(driver), DOROTI_QT_DIAGNOSTICS='1')
env.pop('DOROTI_QT_VALIDATION_RESIZE_CYCLES', None)
env.pop('DOROTI_QT_DESKTOP_NATIVE_CLOSE', None)
env.pop('DOROTI_DESKTOP_LIFETIME', None)
if args.native_close:
    env['DOROTI_QT_DESKTOP_NATIVE_CLOSE'] = '1'
if args.explicit:
    env['DOROTI_DESKTOP_LIFETIME'] = 'Explicit'
timed_out = False
with (out / 'product.log').open('w') as log:
    process = subprocess.Popen(['dotnet', str(app)], env=env, stdout=log,
                               stderr=subprocess.STDOUT, start_new_session=True)
    deadline = time.monotonic() + 120
    while process.poll() is None:
        failed = 'desktop.failure=' in (out / 'product.log').read_text(errors='replace')
        if failed or time.monotonic() > deadline:
            timed_out = not failed
            os.killpg(process.pid, signal.SIGKILL)
            break
        time.sleep(.1)
    exit_code = process.wait()
checks = {'exit': exit_code == 0, 'deadline': not timed_out}
try:
    managed = json.loads(probe.read_text())
    native = json.loads(Path(str(probe) + '.native.json').read_text())
    closed = json.loads(Path(str(probe) + '.closed').read_text())
    teardown = json.loads(Path(str(probe) + '.teardown.json').read_text())
    checks.update({
        'nativeGeometry': native['width'] == 500 and native['height'] == 450,
        'nativeLimitsRestored': native['minimumWidth'] == 0 and native['maximumWidth'] == 16777215,
        'nativeTitleAndChrome': native['title'] == 'Doroti Qt Desktop Probe' and native['nativeDecorations'],
        'actualQpa': native['qpa'] == args.qpa,
        'visibleCapture': native['visible'] and native['capture'],
        'nativeCloseCapability': managed['canCancelNativeClose'],
        'closedOnce': closed == {'Closed': True, 'remaining': 0, 'closingCallbacks': 2},
        'ownerRetired': teardown['staleOwnerRejected'] and teardown['remainingWindows'] == 0,
        'abiGuards': all(native[k] for k in ('invalidVersionRejected', 'invalidSizeRejected', 'wrongThreadRejected')),
        'unsupportedDefaultsRejected': native['unsupportedDefaultsRejected'],
        'unsupportedRejected': len(managed['rejected']) == 6 and managed['appearanceRejected'],
        'mappedProductBinaries': str(app.parent / 'libdoroti_qt_host.so') in native['nativeLibraries']
            and str(app.parent / 'libSkiaSharp.so') in native['nativeLibraries']
            and all(Path(p).parent == app.parent for p in native['nativeLibraries'] if '/libdoroti_' in p),
    })
    if args.native_close:
        checks['nativeCloseCanceled'] = teardown['nativeCloseCanceled']
    else:
        checks['usableAfterCanceledClose'] = managed['afterCanceledClose']['size'] == [510, 460]
    if args.explicit:
        checks['explicitProcessSurvived'] = teardown['explicitProcessSurvived']
    if args.qpa == 'wayland':
        checks['waylandMinimizeRejected'] = managed['waylandMinimizeRejected']
    if 'VK_LAYER_KHRONOS_validation' in env.get('VK_INSTANCE_LAYERS', ''):
        checks['validationLayerLoaded'] = native['validationLayerLoaded']
except (OSError, KeyError, ValueError) as error:
    checks['completeEvidence'] = False
    checks['evidenceError'] = str(error)
log = (out / 'product.log').read_text()
checks['noManagedFatal'] = all(x not in log for x in ('managed.fatal=', 'desktop.failure=', 'Unhandled exception', 'desktop.abort='))
checks['noVulkanValidationError'] = 'VUID-' not in log and 'Validation Error' not in log
report = dict(qpa=args.qpa, session=os.environ.get('XDG_SESSION_TYPE'), nativeClose=args.native_close,
              explicit=args.explicit, exit=exit_code, checks=checks,
              appSha256=hashlib.sha256(app.read_bytes()).hexdigest(),
              shimSha256=hashlib.sha256((app.parent / 'libdoroti_qt_host.so').read_bytes()).hexdigest(),
              managedBinaries={p.name: hashlib.sha256(p.read_bytes()).hexdigest()
                               for p in app.parent.glob('*.dll')},
              driverSha256=hashlib.sha256(driver.read_bytes()).hexdigest(),
              mappedModules={p: hashlib.sha256(Path(p).read_bytes()).hexdigest()
                             for p in locals().get('native', {}).get('nativeLibraries', []) if Path(p).is_file()},
              validationLayers=env.get('VK_INSTANCE_LAYERS', 'notEnabled'),
              physicalInput='notVerified', hiddenReadiness='unsupported', wsiFix='notClaimed')
(out / 'result.json').write_text(json.dumps(report, indent=2) + '\n')
print(json.dumps(report, indent=2))
raise SystemExit(0 if all(value is True for value in checks.values()) else 1)
