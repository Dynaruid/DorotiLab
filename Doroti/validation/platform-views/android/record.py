"""Installed Android product capture/input probe; every child has a 1200 second timeout."""
import argparse
import datetime
import hashlib
import json
from pathlib import Path
import re
import statistics
import subprocess
import time
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[4]


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--serial', required=True)
    parser.add_argument('--output', type=Path)
    parser.add_argument('--overlay', action='store_true')
    parser.add_argument('--capture-only', action='store_true')
    parser.add_argument('--lifecycle-only', action='store_true')
    parser.add_argument('--raster-mode', choices=('optimized', 'baseline'), default='optimized')
    args = parser.parse_args()
    out = args.output or ROOT / 'Doroti/artifacts/platform-views' / datetime.date.today().isoformat() / 'android' / args.serial / datetime.datetime.now().strftime('%H%M%S')
    out.mkdir(parents=True, exist_ok=False)
    commands, checks = [], []
    def run(command, check=True):
        result = subprocess.run(command, capture_output=True, timeout=1200)
        commands.append(dict(command=command, exitCode=result.returncode))
        (out / 'commands.json').write_text(json.dumps(commands, indent=2), encoding='utf-8')
        if check and result.returncode:
            raise RuntimeError(result.stderr.decode(errors='replace'))
        return result.stdout
    def adb(*parts, check=True):
        return run(['adb', '-s', args.serial, *map(str, parts)], check)
    def text(*parts):
        return adb(*parts).decode(errors='replace').strip()
    package = 'dev.doroti.testbed'
    component = text('shell', 'cmd', 'package', 'resolve-activity', '--brief', package).splitlines()[-1]
    since = text('shell', 'date', '+%s.%N')
    rotation = text('shell', 'wm', 'user-rotation').split()
    result = dict(status='FAIL', sourceReviewed=True, automated=True, productLive=False,
                  physical='notVerified', nativeAot='notVerified', serial=args.serial,
                  model=text('shell', 'getprop', 'ro.product.model'),
                  abi=text('shell', 'getprop', 'ro.product.cpu.abi'),
                  os=text('shell', 'getprop', 'ro.build.version.release'),
                  timeoutSeconds=1200, checks=checks)
    result['rasterMode'] = args.raster_mode
    result['commit'] = run(['git', 'rev-parse', 'HEAD']).decode().strip()
    result['dirty'] = run(['git', 'status', '--short']).decode().splitlines()
    result['sourceHashes'] = {str(p.relative_to(ROOT)): hashlib.sha256(p.read_bytes()).hexdigest()
        for p in [ROOT / 'Doroti/src/Doroti.Host.Maui/AndroidPlatformViewHost.cs',
                  ROOT / 'Doroti/src/Doroti.Skia.Rendering/SkiaGraphiteSession.cs',
                  ROOT / 'Doroti/src/Doroti.Host.Maui/DorotiAndroidVulkanViewHandler.cs']}
    pid = ''
    def capture(label):
        current = text('shell', 'pidof', package)
        if not current or current != pid:
            raise RuntimeError('Product process exited or changed: ' + label)
        (out / (label + '.png')).write_bytes(adb('exec-out', 'screencap', '-p'))
        remote = '/sdcard/doroti-platform-view-' + str(time.time_ns()) + '.xml'
        adb('shell', 'uiautomator', 'dump', remote)
        xml = adb('shell', 'cat', remote)
        adb('shell', 'rm', remote)
        (out / (label + '.xml')).write_bytes(xml)
        nodes = list(ET.fromstring(xml).iter('node'))
        if not any(n.get('package') == package for n in nodes):
            raise RuntimeError('Product is not foreground: ' + label)
        print(label, flush=True)
        return nodes
    def tap_node(nodes, query):
        node = next(n for n in nodes if query in n.get('text', '') or query in n.get('content-desc', ''))
        left, top, right, bottom = map(int, re.findall(r'\d+', node.get('bounds')))
        adb('shell', 'input', 'tap', (left + right) // 2, (top + bottom) // 2)
        time.sleep(.6)
    def native_editor(nodes):
        return next(n for n in nodes if n.get('content-desc', '').startswith('doroti-platform-view-')
                    and n.get('class') == 'android.widget.EditText')
    def bounds(node):
        return list(map(int, re.findall(r'\d+', node.get('bounds'))))
    def foreground_count(nodes):
        caption = next(n.get('text') for n in nodes if n.get('text', '').startswith('Foreground taps:'))
        return int(caption.split(':')[-1])
    try:
        adb('shell', 'input', 'keyevent', 'KEYCODE_WAKEUP')
        adb('shell', 'wm', 'dismiss-keyguard')
        adb('shell', 'am', 'force-stop', package)
        adb('shell', 'am', 'start', '-W', '-n', component,
            '--es', 'doroti_testbed_mode', 'platform-views',
            '--es', 'doroti_platform_view_composition', 'overlay' if args.overlay else 'interleaved',
            '--es', 'doroti_platform_view_raster_mode', args.raster_mode,
            '--es', 'DOROTI_PLATFORM_FRAME_PROFILE', '1',
            '--es', 'DOROTI_MAUI_EVIDENCE', '1')
        time.sleep(3)
        pid = text('shell', 'pidof', package)
        nodes = capture('initial')
        assert any(n.get('class') == 'android.widget.EditText' for n in nodes), 'No live native editor'
        if not args.capture_only:
            tap_node(nodes, 'Native button')
            nodes = capture('native-click')
            assert any(n.get('text') == 'Native clicks: 1' for n in nodes), 'Native click was not delivered once'
            checks.append('native button single delivery')
            editor = native_editor(nodes)
            identity = editor.get('content-desc')
            original_text = editor.get('text')
            left, top, right, bottom = bounds(editor)
            adb('shell', 'input', 'tap', left + 20, top + 20)
            time.sleep(.5)
            adb('shell', 'input', 'keyevent', 'KEYCODE_MOVE_END')
            adb('shell', 'input', 'text', 'PVstate')
            adb('shell', 'input', 'keyevent', 'KEYCODE_BACK')
            time.sleep(.7)
            nodes = capture('editor-seeded')
            seeded_text = native_editor(nodes).get('text')
            # ADB key injection follows the device's hardware keyboard layout.
            # Preserve that setting and compare the actual edited value across frames.
            assert seeded_text and seeded_text != original_text, 'Native editor did not receive text'
            result['nativeEditedText'] = seeded_text
            checks.append('native editor text input')
            if not args.overlay:
                for index in range(0 if args.lifecycle_only else 10):
                    caption = next(n.get('text') for n in nodes if n.get('text', '').startswith('Overlap case'))
                    stage = (int(re.search(r'case (\d+)', caption).group(1)) + 1) % 10
                    tap_node(nodes, 'Overlap case')
                    nodes = capture('case-' + str(stage))
                    editor = native_editor(nodes)
                    assert editor.get('content-desc') == identity and editor.get('text') == seeded_text, 'Native state changed across overlap'
                    if stage in (1, 2, 5, 6, 7, 8, 9):
                        button = next(n for n in nodes if n.get('content-desc', '').startswith('doroti-platform-view-1-'))
                        bx, by, br, bb = bounds(button)
                        scale = (br - bx) / 220
                        before = foreground_count(nodes)
                        # Fixture-local (280,130), inside the editor and foreground rectangle.
                        adb('shell', 'input', 'tap', round(bx + 260 * scale), round(by + 110 * scale))
                        time.sleep(.6)
                        if stage in (7, 8):
                            adb('shell', 'input', 'keyevent', 'KEYCODE_BACK')
                            time.sleep(.5)
                        nodes = capture('input-' + str(stage))
                        expected = before + (stage in (1, 2, 5, 6, 9))
                        assert foreground_count(nodes) == expected, 'Foreground shield delivery mismatch at case ' + str(stage)
                if not args.lifecycle_only:
                    checks.append('ten product scenes captured; visual acceptance is separate')
                    checks.append('same native identity and edited state across all scenes; shield on/off and reverse ordering')
                tap_node(nodes, 'Open modal')
                nodes = capture('modal')
                assert any('Native overlay shield' in n.get('text', '') for n in nodes), 'Modal did not open'
                tap_node(nodes, 'Native overlay shield')
                nodes = capture('modal-closed')
                assert not any('Native overlay shield' in n.get('text', '') for n in nodes), 'Modal did not close'
                assert any(n.get('text') == 'Native clicks: 1' for n in nodes), 'Modal leaked a native button click'
                checks.append('foreground modal open and close without native button activation')
            tap_node(nodes, 'Dispose controls')
            nodes = capture('disposed')
            assert not any(n.get('class') == 'android.widget.Button' and 'Native' in n.get('text', '') for n in nodes)
            tap_node(nodes, 'Create controls')
            nodes = capture('recreated')
            assert any(n.get('text') == 'Native button' for n in nodes)
            checks.append('dispose and recreate')
            recreated_identity = native_editor(nodes).get('content-desc')
            for angle, label in ((1, 'landscape'), (0, 'portrait-restored')):
                adb('shell', 'wm', 'user-rotation', 'lock', angle)
                time.sleep(2)
                nodes = capture(label)
                editors = [n for n in nodes if n.get('content-desc', '').startswith('doroti-platform-view-')
                           and n.get('class') == 'android.widget.EditText']
                if args.overlay and angle == 1 and not editors:
                    checks.append('overlay editor clipped outside landscape viewport')
                else:
                    assert native_editor(nodes).get('content-desc') == recreated_identity, 'Rotation recreated native editor'
            checks.append('rotation and insets preserve native instance')
            adb('shell', 'input', 'keyevent', 'KEYCODE_HOME')
            time.sleep(1)
            adb('shell', 'am', 'start', '-W', '-n', component)
            time.sleep(2)
            nodes = capture('resumed')
            assert native_editor(nodes).get('content-desc') == recreated_identity, 'Surface recreation recreated native editor'
            checks.append('background and foreground same process')
        result['status'] = 'PASS'
        result['productLive'] = True
    except Exception as error:
        result['error'] = str(error)
        raise
    finally:
        log = adb('logcat', '-d', '-T', since, *(['--pid=' + pid] if pid else []), check=False)
        (out / 'process.log').write_bytes(log)
        decoded = log.decode(errors='replace')
        rows = [dict((key, float(value)) for key, value in re.findall(r'(ownerMs|vulkanMs|paintMs|fenceMs)=([\d.]+)', line))
                for line in decoded.splitlines() if 'DorotiPlatformTiming' in line]
        if rows:
            def distribution(key):
                values = sorted(row[key] for row in rows if key in row)
                return dict(count=len(values), mean=statistics.mean(values),
                    p50=values[round((len(values)-1)*.50)], p95=values[round((len(values)-1)*.95)],
                    p99=values[round((len(values)-1)*.99)], maximum=max(values))
            result['frameTimingMs'] = {key: distribution(key) for key in ('ownerMs', 'vulkanMs', 'paintMs', 'fenceMs')}
        counters = [dict((key, int(value)) for key, value in re.findall(r'(frame|readbackFrames|readbackBytes|reusedSlices)=(\d+)', line))
                    for line in decoded.splitlines() if 'DorotiPlatformFrame' in line]
        if counters:
            result['rasterCounters'] = counters[-1]
        if re.search(rb'FATAL EXCEPTION|Fatal signal| E DorotiGraphite| E DorotiMauiFailure', log):
            result['status'] = 'FAIL'
            result['error'] = 'Product failure in process.log'
        if rotation and rotation[0] in ('free', 'lock'):
            adb('shell', 'wm', 'user-rotation', *rotation)
        (out / 'result.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
    if result['status'] != 'PASS':
        raise RuntimeError(result['error'])


if __name__ == '__main__':
    main()
