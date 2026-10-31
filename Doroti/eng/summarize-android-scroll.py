"""Summarize exported bounded rings; submission timing is not display FPS."""
import argparse
from collections import Counter
import json
from pathlib import Path


def stats(values):
    values = sorted(values)
    if not values:
        return {'count': 0}
    return {'count': len(values), **{f'p{q}': round(values[int((len(values)-1)*q/100)], 3) for q in (50, 95, 99)},
            'max': round(values[-1], 3)}


def summarize(path):
    data = json.loads(path.read_text(encoding='utf-8'))
    frames = [f for f in data['frames'] if f['gesture'] >= 2]
    inputs = [i for i in data['inputs'] if i['gesture'] >= 2]
    lo = min(i['atUs'] for i in inputs)
    hi = max(f['atUs'] for f in frames)
    trace = [t for t in data['trace'] if lo <= t['recordedAtMicroseconds'] <= hi]
    result = {'file': path.name, 'presentObservedByRing': data['presentObserved'],
              'frames': len(frames), 'submitted': sum(f['submitted'] for f in frames),
              'ownerMs': stats([f['ownerMs'] for f in frames]),
              'callbackMs': stats([f['callbackMs'] for f in frames]),
              'callbackWaitMs': stats([f['callbackWaitMs'] for f in frames]),
              'allocatedBytesPerDraw': stats([f['allocated'] for f in frames]),
              'ownerOver50ms': sum(f['ownerMs'] >= 50 for f in frames),
              'inputAgeMs': stats([max(0, i['arrivalMs']-i['eventMs']) for i in inputs]),
              'dispatchMs': stats([i['dispatchMs'] for i in inputs]),
              'nativeHistorySamples': sum(i['history'] for i in inputs),
              'actions': dict(Counter(i['action'] for i in inputs)),
              'reasons': dict(Counter(f['reason'] for f in frames)),
              'coldOwnerMaxMs': max((f['ownerMs'] for f in data['frames'] if f['gesture'] == 0), default=0),
              'setupFirstContactMaxMs': max((f['ownerMs'] for f in data['frames'] if f['gesture'] == 1), default=0),
              'gcCollections': {k: frames[-1][k]-frames[0][k] for k in ('gen0','gen1','gen2')},
              'vulkanMs': {k: stats([f['vulkan'][k] for f in frames if f['submitted']]) for k in frames[0]['vulkan']}}
    # Phase values are append-only DorotiFramePhase members. Use the separate
    # recording clock, never the causally clamped trace timestamp, for work.
    for name, start, end in [('layoutMs', 8, 9), ('semanticsMs', 27, 28)]:
        durations, pending = [], None
        for t in trace:
            if t['phase'] == start:
                pending = t['recordedAtMicroseconds']
            elif t['phase'] == end and pending is not None:
                durations.append((t['recordedAtMicroseconds']-pending)/1000)
                pending = None
        result[name] = stats(durations)
    for key in ('snapMs', 'insertMs', 'submitMs'):
        if key in frames[0]:
            result[key] = stats([f[key] for f in frames if f['submitted']])
    submitted = [f for f in frames if f['submitted']]
    result['repeatedSubmittedScenes'] = sum(a['scene'] == b['scene'] and a['scene'] != 0 for a,b in zip(submitted, submitted[1:]))
    result['backwardSubmittedScenes'] = sum(a['scene'] > b['scene'] > 0 for a,b in zip(submitted, submitted[1:]))
    # Ticker labels belong to their provider (e.g. ScrollableState), so they
    # cannot reliably count ballistic activity replacements.
    # Do not classify long gaps across independent gestures or final rest as
    # active scrolling. Preserve every raw gap for follow-up attribution.
    result['submitGapsWithinGestureMs'] = stats([(b['atUs']-a['atUs'])/1000 for a,b in zip(submitted, submitted[1:]) if a['gesture']==b['gesture']])
    result['slowFrames'] = sorted(frames, key=lambda f:f['ownerMs'], reverse=True)[:5]
    return result


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('files', nargs='+', type=Path)
    parser.add_argument('--output', type=Path)
    args = parser.parse_args()
    results = [summarize(path) for path in args.files]
    if args.output:
        args.output.write_text(json.dumps(results, indent=2), encoding='utf-8')
    for r in results:
        print(json.dumps({k:v for k,v in r.items() if k != 'slowFrames'}))
