"""Compare scrolling before/after explicit GC and Home/trim, without a long idle.

Run with run-with-timeout.py --timeout 1200. Requires a shell-only ScrollGesture
dex jar. Forced GC requires the explicit DOROTI_SCROLL_PROFILE launch flag;
ordinary app launches never collect. Submission timings are not display FPS.
"""
import argparse
import hashlib
import json
import os
from pathlib import Path
import re
import shlex
import subprocess
import time
import xml.etree.ElementTree as ET


def stats(values):
    values = sorted(values)
    if not values:
        return {'count': 0}
    return {'count': len(values), **{f'p{q}': round(values[int((len(values)-1)*q/100)], 3)
                                   for q in (50, 95, 99)}, 'max': round(values[-1], 3)}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--serial', required=True)
    parser.add_argument('--label', required=True)
    parser.add_argument('--output', type=Path, default=Path('temp/android-resume-scroll'))
    parser.add_argument('--gesture-jar', type=Path, required=True)
    parser.add_argument('--trials', type=int, choices=range(1, 4), default=3)
    parser.add_argument('--gc-log', action='store_true', help='Separate diagnostic, restores debug.mono.log')
    parser.add_argument('--work-profile', action='store_true', help='Separate allocation/layout diagnostic; adds overhead')
    parser.add_argument('--trace-capacity', type=int, choices=(8192, 32768, 65536),
                        help='Diagnostic trace ring size; requires an APK forwarding this launch flag')
    parser.add_argument('--export-per-stage', action='store_true',
                        help='Preserve each gesture trace before the bounded ring can overwrite it')
    parser.add_argument('--work-trace-per-stage', action='store_true',
                        help='Diagnostic node trace per gesture; requires --export-per-stage and adds overhead')
    parser.add_argument('--skip-forced-gc', action='store_true', help='Control: Home/trim only, no explicit GC')
    parser.add_argument('--resume-cleanup', choices=['natural', 'trim'], default='trim',
                        help='Natural Home lifecycle or explicit background memory-pressure signal')
    parser.add_argument('--gesture-x', type=int, default=65)
    parser.add_argument('--motion', choices=['drag', 'fling'], default='drag',
                        help='Fling measures UP-to-rest separately, in both directions')
    parser.add_argument('--fling-tail-seconds', type=float, default=2.0)
    parser.add_argument('--fling-duration-ms', type=int, default=180)
    parser.add_argument('--fling-distance', type=int, default=700)
    parser.add_argument('--warm-fling-pairs', type=int, choices=range(3), default=0,
                        help='Visit the exact measured fling range before the initial session')
    parser.add_argument('--warm-x', type=int, default=12)
    parser.add_argument('--warm-swipes', type=int, choices=range(13), default=0,
                        help='Visit retained Components sections before comparing GC/resume')
    parser.add_argument('--expect-retained-context', action='store_true')
    parser.add_argument('--verify-lifecycle', action='store_true', help='Also verify Activity recreation/state and final GPU teardown')
    args = parser.parse_args()
    if args.work_trace_per_stage and not args.export_per_stage:
        parser.error('--work-trace-per-stage requires --export-per-stage')
    args.output.mkdir(parents=True, exist_ok=True)
    adb = Path(os.environ['LOCALAPPDATA']) / 'Android/Sdk/platform-tools/adb.exe'
    package = 'dev.doroti.testbed'

    def call(*cmd):
        return subprocess.check_output([str(adb), '-s', args.serial, *map(str, cmd)],
                                       text=True, encoding='utf-8', errors='replace',
                                       stderr=subprocess.STDOUT, timeout=60).strip()

    def snapshot_blocks_input():
        # Samsung can retain an invisible RequestedLayerState after its starting
        # window left the input stack. Layer-name existence is not occlusion.
        state = call('shell', 'dumpsys', 'input')
        for line in state.splitlines():
            if not re.match(r'\s*\d+: name=', line) or 'inputConfig=' not in line:
                continue
            if package + '/' in line and 'SurfaceView[' not in line:
                return False
            if 'SnapshotStartingWindow' in line and 'NOT_VISIBLE' not in line:
                alpha = re.search(r'alpha=([\d.]+)', line)
                if alpha and float(alpha[1]) > 0:
                    return True
        return False

    component = call('shell', 'cmd', 'package', 'resolve-activity', '--brief', package).splitlines()[-1]
    profile_path = f'/sdcard/Android/data/{package}/files/doroti-scroll-profile.json'

    def export_profile(target):
        call('shell', 'rm', '-f', profile_path)
        call('shell', 'am', 'start', '-n', component, '--es', 'DOROTI_SCROLL_EXPORT', '1')
        for _ in range(30):
            if 'doroti-scroll-profile.json' in call('shell', 'ls', f'/sdcard/Android/data/{package}/files'):
                call('pull', profile_path, target)
                return json.loads(target.read_text(encoding='utf-8'))
            time.sleep(.2)
        raise RuntimeError('Profile export did not complete')

    apk = call('shell', 'pm', 'path', package).splitlines()[0].removeprefix('package:')
    installed = args.output / f'{args.label}-installed.apk'
    call('pull', apk, installed)
    call('push', args.gesture_jar, '/data/local/tmp/doroti-scroll.jar')
    previous = call('shell', 'getprop', 'debug.mono.log')
    report = {'serial': args.serial, 'label': args.label,
              'apkSha256': hashlib.sha256(installed.read_bytes()).hexdigest(),
              'presentObserved': False, 'physicalInput': 'notVerified',
              'motion': args.motion, 'resumeCleanup': args.resume_cleanup,
              'flingTailSeconds': args.fling_tail_seconds,
              'flingDurationMs': args.fling_duration_ms, 'flingDistance': args.fling_distance,
              'gestureX': args.gesture_x, 'warmSwipes': args.warm_swipes,
              'warmFlingPairs': args.warm_fling_pairs,
              'exportPerStage': args.export_per_stage,
              'diagnosticWorkProfile': args.work_profile,
              'traceCapacity': args.trace_capacity,
              'diagnosticWorkTrace': args.work_trace_per_stage, 'trials': []}
    try:
        if args.gc_log:
            call('shell', 'setprop', 'debug.mono.log', 'gc,mono_log_level=debug,mono_log_mask=gc')
        for trial in range(1, args.trials + 1):
            prefix = args.output / f'{args.label}-{trial}'
            call('shell', 'am', 'force-stop', package)
            call('shell', 'rm', '-f', f'/sdcard/Android/data/{package}/files/doroti-scroll-profile.json')
            call('logcat', '-c')
            extras = [] if args.skip_forced_gc else ['--es', 'DOROTI_SCROLL_GC_ON_TRIM', '1']
            if args.work_profile:
                for flag in ('DOROTI_STAGE_TRACE', 'DOROTI_LAYOUT_PROFILE', 'DOROTI_ALLOCATION_PROFILE'):
                    extras += ['--es', flag, '1']
            if args.trace_capacity is not None:
                extras += ['--es', 'DOROTI_FRAME_TRACE_CAPACITY', str(args.trace_capacity)]
            call('shell', 'am', 'start', '-n', component, '--es', 'DOROTI_SCROLL_PROFILE', '1', *extras)
            time.sleep(5)  # Cold startup only; no background aging interval.
            for _ in range(args.warm_swipes):
                call('shell', 'input', 'swipe', args.warm_x, '1850', args.warm_x, '450', '120')
                time.sleep(.5)
            if args.warm_swipes:
                time.sleep(1)
            for _ in range(args.warm_fling_pairs):
                for start_y, end_y in ((1800, 1800-args.fling_distance), (1800-args.fling_distance, 1800)):
                    x = args.gesture_x
                    call('shell', 'CLASSPATH=/data/local/tmp/doroti-scroll.jar app_process /system/bin '
                         f'ScrollGesture {x},{start_y},0 {x},{end_y},{args.fling_duration_ms}')
                    time.sleep(args.fling_tail_seconds)
            pid = call('shell', 'pidof', package)
            if not pid:
                raise RuntimeError('App did not survive startup')
            stages = []
            trim_levels = [('BACKGROUND', 40), ('MODERATE', 60), ('COMPLETE', 80)]
            next_trim_level = 0
            requested_trim_levels = []
            trial_report = {'trial': trial, 'pid': pid, 'status': 'running', 'stages': stages,
                            'requestedTrimLevels': requested_trim_levels}
            report['trials'].append(trial_report)
            cleanup_name = 'resume' if args.resume_cleanup == 'natural' else 'resume-trim'
            names = ('initial', cleanup_name, cleanup_name+'-again') if args.skip_forced_gc else (
                'initial', 'foreground-gc', cleanup_name+'-gc', cleanup_name+'-gc-again')
            for name in names:
                if name == 'foreground-gc':
                    call('shell', 'am', 'start', '-n', component, '--es', 'DOROTI_SCROLL_GC', '1')
                    time.sleep(.5)
                elif name.startswith('resume'):
                    old_detaches = call('logcat', '-d', '-v', 'monotonic', '--pid=' + pid).count('detached submitted=')
                    call('shell', 'input', 'keyevent', 'KEYCODE_HOME')
                    # Home's transition can temporarily retain TOP process state.
                    # Poll OS admission rather than assuming an elapsed idle time.
                    for attempt in range(30):
                        try:
                            if args.resume_cleanup == 'trim':
                                # ActivityManager rejects a level no greater than
                                # the process's last injected trim. Escalate the
                                # second signal rather than retrying BACKGROUND.
                                trim_name, trim_level = trim_levels[next_trim_level]
                                call('shell', 'am', 'send-trim-memory', package, trim_name)
                                requested_trim_levels.append(trim_level)
                                next_trim_level += 1
                                break
                            if call('logcat', '-d', '-v', 'monotonic', '--pid=' + pid).count('detached submitted=') > old_detaches:
                                break
                            time.sleep(.2)
                        except subprocess.CalledProcessError as error:
                            if ('Unable to set a higher trim level than current level' in error.output
                                    and next_trim_level + 1 < len(trim_levels)):
                                next_trim_level += 1
                                continue
                            if 'background trim level on a foreground process' not in error.output:
                                raise
                            time.sleep(.2)
                    else:
                        raise RuntimeError('OS never completed the requested background transition')
                    time.sleep(.5)  # Complete explicit GC outside motion.
                    resumed_at = time.monotonic()
                    call('shell', 'am', 'start', '-n', component)
                    # A heavy first frame can keep Android's task snapshot over
                    # the app and cause InputDispatcher to drop the entire DOWN.
                    # Observe removal rather than hiding this cost in a fixed wait.
                    time.sleep(.2)
                    for attempt in range(50):
                        if not snapshot_blocks_input():
                            break
                        time.sleep(.1)
                    else:
                        raise RuntimeError('Android task snapshot still blocks the resumed app')
                    resume_ready_ms = round((time.monotonic()-resumed_at)*1000, 3)
                if call('shell', 'pidof', package) != pid:
                    raise RuntimeError('Process restarted; this is not a resume comparison')
                layers = call('shell', 'dumpsys', 'SurfaceFlinger', '--list')
                layer = next((re.sub(r'^RequestedLayerState\{', '', l).split(' parentId=')[0]
                              for l in layers.splitlines() if 'SurfaceView[' + package in l and '(BLAST)' in l), None)
                x = args.gesture_x
                end_y = 1800 - args.fling_distance
                motions = [('up', f'{x},1800,0 {x},{end_y},{args.fling_duration_ms}'),
                           ('down', f'{x},{end_y},0 {x},1800,{args.fling_duration_ms}')] if args.motion == 'fling' else [
                    ('drag', f'{x},1800,0 {x},1050,650 {x},1780,1300 {x},1100,1950 {x},1780,2600 {x},1780,2850')]
                for direction, points in motions:
                    if args.work_trace_per_stage:
                        call('shell', 'am', 'start', '-n', component, '--es', 'DOROTI_WORK_TRACE_START', '1')
                    motion = call('shell', 'CLASSPATH=/data/local/tmp/doroti-scroll.jar app_process /system/bin '
                                  'ScrollGesture ' + points)
                    if args.motion == 'fling':
                        time.sleep(args.fling_tail_seconds)
                    stage_name = name + '-' + direction if args.motion == 'fling' else name
                    stage_profile = None
                    if args.export_per_stage:
                        stage_profile = Path(str(prefix) + '.' + stage_name + '.json')
                        export_profile(stage_profile)
                    if layer:
                        latency = call('shell', 'dumpsys SurfaceFlinger --latency ' + shlex.quote(layer))
                        Path(str(prefix) + '.' + stage_name + '.latency.txt').write_text(latency, encoding='utf-8')
                    down = int(re.search(r'continuous-down-ms=(\d+)', motion)[1])
                    stages.append({'name': stage_name, 'downMs': down, 'motion': motion,
                                   'meminfo': call('shell', 'dumpsys', 'meminfo', package)})
                    if stage_profile is not None:
                        stages[-1]['profile'] = stage_profile.name
                    if name.startswith('resume'):
                        stages[-1]['resumeSnapshotReadyMs'] = resume_ready_ms
                        if args.resume_cleanup == 'trim':
                            stages[-1]['requestedTrimLevel'] = requested_trim_levels[-1]
                    print(json.dumps({'trial': trial, 'stage': stage_name, 'injection': motion}), flush=True)
            path = prefix.with_suffix('.json')
            data = export_profile(path)
            previous_up = None
            for stage in stages:
                if args.export_per_stage:
                    data = json.loads((args.output / stage['profile']).read_text(encoding='utf-8'))
                inputs = [i for i in data['inputs'] if abs(i['eventMs']-stage['downMs']) < 5 and i['action'] == 0]
                if len(inputs) != 1:
                    raise RuntimeError('Missing or ambiguous measured DOWN')
                first = inputs[0]
                inputs = [i for i in data['inputs'] if i['gesture'] == first['gesture']]
                up = next(i for i in inputs if i['action'] == 1)
                frames = [f for f in data['frames'] if first['atUs'] <= f['atUs'] <= up['atUs']]
                if not frames or not any(f['submitted'] for f in frames):
                    raise RuntimeError('No submitted frames during measured motion')
                stage['summary'] = {'gesture': first['gesture'], 'ownerMs': stats([f['ownerMs'] for f in frames]),
                    'callbackMs': stats([f['callbackMs'] for f in frames]),
                    'snapMs': stats([f['snapMs'] for f in frames]),
                    'allocated': stats([f['allocated'] for f in frames]),
                    'over50ms': sum(f['ownerMs'] >= 50 for f in frames),
                    'gc': {g: frames[-1][g]-frames[0][g] for g in ('gen0', 'gen1', 'gen2')},
                    'submitted': sum(f['submitted'] for f in frames)}
                if args.motion == 'fling':
                    tail = [f for f in data['frames'] if up['atUs'] < f['atUs'] <= up['atUs'] + args.fling_tail_seconds * 1e6]
                    stage['unexpectedDowns'] = [i for i in data['inputs'] if i['action'] == 0
                        and i['gesture'] != first['gesture']
                        and first['atUs'] <= i['atUs'] <= up['atUs'] + args.fling_tail_seconds * 1e6]
                    submitted = [f for f in tail if f['submitted']]
                    native_gaps = [(b['vsyncNanos'] - a['vsyncNanos']) / 1e6 for a, b in zip(submitted, submitted[1:])]
                    owner_gaps = [(b['atUs'] - a['atUs']) / 1000 for a, b in zip(submitted, submitted[1:])]
                    stage['ballistic'] = {'ownerMs': stats([f['ownerMs'] for f in tail]),
                        'callbackMs': stats([f['callbackMs'] for f in tail]),
                        'snapMs': stats([f['snapMs'] for f in tail]),
                        'allocated': stats([f['allocated'] for f in tail]),
                        'submitted': len(submitted), 'nativeGapMs': stats(native_gaps),
                        'completionGapMs': stats(owner_gaps), 'nativeGapsOver12ms': sum(x > 12 for x in native_gaps),
                        'gc': {g: tail[-1][g]-tail[0][g] for g in ('gen0', 'gen1', 'gen2')} if tail else {}}
                    stage['ballistic']['tracedUpdates'] = sum(t['phase'] == 22
                        and t['scrollActivity'] == 'BallisticScrollActivity'
                        and up['atUs'] < t['recordedAtMicroseconds'] < up['atUs'] + args.fling_tail_seconds * 1e6
                        for t in data['trace'])
                if previous_up is not None:
                    stage['summary']['beforeMotionDrawMs'] = stats([
                        f['ownerMs'] for f in data['frames'] if previous_up <= f['atUs'] < first['atUs']])
                previous_up = up['atUs']
            log = call('logcat', '-d', '-v', 'monotonic')
            prefix.with_suffix('.log').write_text(log, encoding='utf-8')
            if 'FATAL EXCEPTION' in log or re.search(r'\bE\s+(?:DorotiGraphite|DorotiMauiFailure)\s*:', log):
                raise RuntimeError('Fatal/render/retirement error in log')
            if not args.skip_forced_gc and (log.count('gc source=intent') != 1
                    or (args.resume_cleanup == 'trim' and any(
                        f'gc source=trim-{level} ' not in log for level in requested_trim_levels))):
                raise RuntimeError('Explicit foreground/background GC probes did not execute')
            context_stats = {'contextCreations': len(re.findall(r'Vulkan device=.* generation=', log)),
                             'contextReattachments': log.count('Surface attached contextGeneration='),
                             'completedSurfaceDetaches': log.count('detached submitted='),
                             'unusedGpuPurges': log.count('Unused GPU resources purged')}
            if context_stats['completedSurfaceDetaches'] and re.search(r'detached submitted=.*outstanding=[1-9]', log):
                raise RuntimeError('A detached surface retained outstanding GPU frames')
            if args.expect_retained_context and (context_stats['contextCreations'] != 1
                    or context_stats['contextReattachments'] != 2 or context_stats['completedSurfaceDetaches'] != 2
                    or (args.resume_cleanup == 'trim' and context_stats['unusedGpuPurges'] < 2)):
                raise RuntimeError('Resume did not retain one drained context and handle memory-pressure cleanup')
            call('shell', 'screencap', '-p', '/sdcard/doroti-resume.png')
            call('pull', '/sdcard/doroti-resume.png', prefix.with_suffix('.png'))
            trial_report.update(status='complete', **context_stats)
            print(json.dumps({'trial': trial, 'summaries': [{k: v for k, v in s.items() if k != 'meminfo'} for s in stages]}), flush=True)
        if args.verify_lifecycle:
            def hierarchy(label):
                call('shell', 'uiautomator', 'dump', '/sdcard/doroti-resume-lifecycle.xml')
                target = args.output / f'{args.label}-{label}.xml'
                call('pull', '/sdcard/doroti-resume-lifecycle.xml', target)
                return sorted(tuple(n.get(k, '') for k in ('class', 'text', 'content-desc', 'bounds'))
                              for n in ET.parse(target).iter('node') if n.get('package') == package)

            before = hierarchy('before-recreate')
            if not before:
                raise RuntimeError('No app semantics before Activity recreation')
            log = call('logcat', '-d', '-v', 'monotonic')
            old_creations = len(re.findall(r'Vulkan device=.* generation=', log))
            call('shell', 'input', 'keyevent', 'KEYCODE_HOME')
            call('shell', 'am', 'start', '-n', component, '--es', 'DOROTI_SCROLL_LIFECYCLE', 'recreate')
            for attempt in range(50):
                time.sleep(.1)
                log = call('logcat', '-d', '-v', 'monotonic')
                if len(re.findall(r'Vulkan device=.* generation=', log)) > old_creations:
                    break
            else:
                raise RuntimeError('Replacement Activity did not create its native renderer')
            if call('shell', 'pidof', package) != pid:
                raise RuntimeError('Activity recreation restarted the process')
            after = hierarchy('after-recreate')
            if before != after:
                raise RuntimeError('Activity recreation changed retained app semantics or geometry')
            old_drains = log.count('drained submitted=')
            call('shell', 'am', 'start', '-n', component, '--es', 'DOROTI_SCROLL_LIFECYCLE', 'finish')
            for attempt in range(50):
                time.sleep(.1)
                log = call('logcat', '-d', '-v', 'monotonic')
                if log.count('drained submitted=') > old_drains:
                    break
            else:
                raise RuntimeError('Final Activity teardown did not retire the GPU context')
            (args.output / f'{args.label}-lifecycle.log').write_text(log, encoding='utf-8')
            if 'FATAL EXCEPTION' in log or re.search(r'\bE\s+(?:DorotiGraphite|DorotiMauiFailure)\s*:', log):
                raise RuntimeError('Activity lifecycle produced a fatal/render/retirement error')
            if re.search(r'drained submitted=.*outstanding=[1-9]', log):
                raise RuntimeError('Final GPU retirement retained outstanding frames')
            report['lifecycle'] = {'samePid': True, 'retainedSemanticsAndGeometry': True,
                                   'semanticNodes': len(after), 'finalGpuRetirement': True}
            print(json.dumps({'lifecycle': report['lifecycle']}), flush=True)
    except Exception as error:
        if report['trials'] and report['trials'][-1]['status'] == 'running':
            report['trials'][-1]['status'] = 'failed'
        report['error'] = {'type': type(error).__name__, 'message': str(error),
                           'output': getattr(error, 'output', None)}
        raise
    finally:
        if args.gc_log:
            call('shell', 'setprop', 'debug.mono.log', shlex.quote(previous))
            if call('shell', 'getprop', 'debug.mono.log') != previous:
                raise RuntimeError('Failed to restore debug.mono.log')
        (args.output / f'{args.label}-manifest.json').write_text(json.dumps(report, indent=2), encoding='utf-8')


if __name__ == '__main__':
    main()
