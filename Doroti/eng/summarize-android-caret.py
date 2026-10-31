"""Summarize the last caret gesture's DOWN-to-UP window, excluding idle tail.

Run from the repository root. CPU/submit timing is not physical display latency.
Whole-session work aggregates include setup and profile instrumentation overhead.
"""
import argparse
import importlib.util
import json
from pathlib import Path

spec = importlib.util.spec_from_file_location('scroll_summary', Path(__file__).with_name('summarize-android-scroll.py'))
module = importlib.util.module_from_spec(spec)
spec.loader.exec_module(module)
stats = module.stats


def summarize(path):
    data = json.loads(path.read_text(encoding='utf-8'))
    gesture = max(i['gesture'] for i in data['inputs'])
    inputs = [i for i in data['inputs'] if i['gesture'] == gesture]
    frames = [f for f in data['frames'] if f['gesture'] == gesture]
    lo, hi = min(i['atUs'] for i in inputs), max(i['atUs'] for i in inputs)
    moving = [f for f in frames if lo <= f['atUs'] <= hi]
    submitted = [f for f in moving if f['submitted']]
    trace = [t for t in data['trace'] if lo <= t['recordedAtMicroseconds'] <= hi]
    phases = {}
    for name, start, end in [('build', 7, 8), ('layout', 8, 9), ('paint', 9, 10), ('semantics', 27, 28)]:
        durations, pending = [], None
        for entry in trace:
            if entry['phase'] == start:
                pending = entry['recordedAtMicroseconds']
            elif entry['phase'] == end and pending is not None:
                durations.append((entry['recordedAtMicroseconds'] - pending) / 1000)
                pending = None
        phases[name] = {'totalMs': round(sum(durations), 3), **stats(durations)}
    return {
        'file': path.name, 'gesture': gesture, 'drawsIncludingTail': len(frames), 'drawsDuringInput': len(moving),
        'ownerMs': stats([f['ownerMs'] for f in moving]),
        'callbackMs': stats([f['callbackMs'] for f in moving]),
        'inputAgeMs': stats([max(0, i['arrivalMs'] - i['eventMs']) for i in inputs]),
        'dispatchMs': stats([i['dispatchMs'] for i in inputs]),
        'submitGapsDuringInputMs': stats([(b['atUs'] - a['atUs']) / 1000 for a, b in zip(submitted, submitted[1:])]),
        'allocatedPerDraw': stats([f['allocated'] for f in moving]),
        'ownerOver50ms': sum(f['ownerMs'] >= 50 for f in moving),
        'nativeHistorySamples': sum(i['history'] for i in inputs), 'inputEvents': len(inputs),
        'phasesDuringInput': phases, 'magnifier': data.get('magnifier'), 'semantics': data.get('semantics'),
        'editing': data.get('editing'), 'presentObserved': data['presentObserved'],
        'workWholeSessionTopSelfMs': [
            {'type': w['type'], 'kind': w['kind'], 'calls': w['calls'],
             'selfMs': round(w['selfMicroseconds'] / 1000, 3), 'selfBytes': w['selfAllocatedBytes']}
            for w in sorted(data['work'], key=lambda w: w['selfMicroseconds'], reverse=True)[:12]]}


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('files', nargs='+', type=Path)
    parser.add_argument('--output', type=Path)
    args = parser.parse_args()
    results = [summarize(path) for path in args.files]
    if args.output:
        args.output.write_text(json.dumps(results, indent=2), encoding='utf-8')
    for result in results:
        print(json.dumps(result))
