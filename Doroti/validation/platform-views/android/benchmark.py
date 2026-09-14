"""Compare the same installed APK's baseline and optimized live spinner compositor."""
import argparse
import datetime
import json
from pathlib import Path
import re
import statistics
import subprocess
import time


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--serial', required=True)
    parser.add_argument('--seconds', type=float, default=8)
    args = parser.parse_args()
    root = Path(__file__).resolve().parents[4]
    out = root / 'Doroti/artifacts/platform-views' / datetime.date.today().isoformat() / 'android/performance' / (args.serial + '-' + datetime.datetime.now().strftime('%H%M%S'))
    out.mkdir(parents=True, exist_ok=False)
    commands = []
    def adb(*parts):
        command = ['adb', '-s', args.serial, *map(str, parts)]
        completed = subprocess.run(command, capture_output=True, timeout=1200)
        commands.append(dict(command=command, exitCode=completed.returncode))
        (out / 'commands.json').write_text(json.dumps(commands, indent=2), encoding='utf-8')
        if completed.returncode:
            raise RuntimeError(completed.stderr.decode(errors='replace'))
        return completed.stdout
    def text(*parts):
        return adb(*parts).decode(errors='replace').strip()
    component = text('shell', 'cmd', 'package', 'resolve-activity', '--brief', 'dev.doroti.testbed').splitlines()[-1]
    summary = dict(status='FAIL', serial=args.serial, seconds=args.seconds, workload='idle indeterminate spinner over live Button/EditText, case 5',
                   physicalInput=False, displayScanout='notVerified', variants={})
    try:
        for mode in ('baseline', 'optimized'):
            folder = out / mode
            folder.mkdir()
            adb('shell', 'am', 'force-stop', 'dev.doroti.testbed')
            adb('shell', 'input', 'keyevent', 'KEYCODE_WAKEUP')
            adb('shell', 'wm', 'dismiss-keyguard')
            adb('shell', 'am', 'start', '-W', '-n', component, '--es', 'doroti_testbed_mode', 'platform-views',
                '--es', 'doroti_platform_view_raster_mode', mode, '--es', 'DOROTI_MAUI_EVIDENCE', '1',
                '--es', 'DOROTI_PLATFORM_FRAME_PROFILE', '1')
            time.sleep(3)
            pid = text('shell', 'pidof', 'dev.doroti.testbed')
            since = text('shell', 'date', '+%s.%N')
            started = time.monotonic()
            (folder / 'spinner-a.png').write_bytes(adb('exec-out', 'screencap', '-p'))
            time.sleep(.19)
            (folder / 'spinner-b.png').write_bytes(adb('exec-out', 'screencap', '-p'))
            time.sleep(max(0, args.seconds - (time.monotonic() - started)))
            if text('shell', 'pidof', 'dev.doroti.testbed') != pid:
                raise RuntimeError('Product exited during ' + mode)
            log = text('logcat', '-d', '-T', since, '--pid=' + pid)
            elapsed = time.monotonic() - started
            (folder / 'process.log').write_text(log, encoding='utf-8')
            if re.search(r' E DorotiGraphite| E DorotiMauiFailure|Fatal signal|FATAL EXCEPTION', log):
                raise RuntimeError('Renderer failure in ' + mode)
            rows = [dict((key, float(value)) for key, value in re.findall(r'(ownerMs|vulkanMs|paintMs|fenceMs)=([\d.]+)', line))
                    for line in log.splitlines() if 'DorotiPlatformTiming' in line]
            if len(rows) < 10:
                raise RuntimeError('Missing continuous product frame evidence: ' + mode)
            def distribution(key):
                values = sorted(row[key] for row in rows)
                return dict(mean=statistics.mean(values), p50=values[round((len(values)-1)*.5)],
                    p95=values[round((len(values)-1)*.95)], p99=values[round((len(values)-1)*.99)], maximum=max(values))
            counters = [dict((key, int(value)) for key, value in re.findall(r'(frame|readbackFrames|readbackBytes|reusedSlices)=(\d+)', line))
                        for line in log.splitlines() if 'DorotiPlatformFrame' in line]
            metrics = dict(frames=len(rows), observedSeconds=elapsed, acceptedFramesPerSecond=len(rows)/elapsed,
                timingMs={key: distribution(key) for key in ('ownerMs', 'vulkanMs', 'paintMs', 'fenceMs')})
            if len(counters) > 1:
                metrics['rasterDelta'] = {key: counters[-1][key] - counters[0][key] for key in counters[0]}
            summary['variants'][mode] = metrics
            (folder / 'memory.txt').write_bytes(adb('shell', 'dumpsys', 'meminfo', 'dev.doroti.testbed'))
            print(mode, json.dumps(metrics), flush=True)
        summary['status'] = 'PASS'
    finally:
        (out / 'result.json').write_text(json.dumps(summary, indent=2), encoding='utf-8')
    print(out, flush=True)


if __name__ == '__main__':
    main()
