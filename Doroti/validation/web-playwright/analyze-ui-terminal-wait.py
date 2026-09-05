"""Attribute existing cross-worker ACK waits to recorded synchronous UI work.

Recorded UI frames and resize applies are a lower bound, not an event-loop
profile. Cross-worker timestamps have browser precision limits.
"""
import json
from pathlib import Path
from statistics import median
import sys


def analyze(stages, start=float('-inf'), end=float('inf')):
    ui = stages['ui']['entries']
    raster = stages['raster']['entries']
    busy = []
    for entry in ui:
        if entry['stage'] == 'frame-start':
            callback = entry['detail']['callbackId']
            last = next((e for e in ui if e['stage'] == 'frame-end'
                         and e['detail'].get('callbackId') == callback), None)
            if last:
                busy.append((entry['time'], last['time']))
        elif entry['stage'] == 'ui-resize-applied':
            busy.append((entry['time'] - entry['detail']['applyMilliseconds'], entry['time']))
    rows = []
    for sent in raster:
        if sent['stage'] != 'raster-terminal-sent' or not start <= sent['time'] <= end:
            continue
        received = next((e for e in ui if e['stage'] == 'ui-terminal-received'
                         and e['sequence'] == sent['sequence']), None)
        if not received:
            continue
        low, high = sent['time'], received['time']
        # Union intersections: resize and frame intervals must not be summed twice.
        intervals = sorted((max(a, low), min(b, high)) for a, b in busy if min(b, high) > max(a, low))
        merged = []
        for a, b in intervals:
            if merged and a <= merged[-1][1]:
                merged[-1][1] = max(merged[-1][1], b)
            else:
                merged.append([a, b])
        overlap = sum(b-a for a, b in merged)
        rows.append({'scene': sent['sequence'], 'waitMs': high-low, 'recordedUiBusyMs': overlap,
                     'unattributedMs': high-low-overlap})
    return {'rows': rows, 'median': {k: median(r[k] for r in rows) if rows else None
            for k in ('waitMs', 'recordedUiBusyMs', 'unattributedMs')},
            'totalWaitMs': sum(r['waitMs'] for r in rows),
            'totalRecordedUiBusyMs': sum(r['recordedUiBusyMs'] for r in rows)}


if __name__ == '__main__':
    source = Path(sys.argv[1])
    report = json.loads(source.read_text(encoding='utf-8-sig'))
    if 'samples' in report:
        result = [analyze(s['stages']) for s in report['samples'] if s.get('stages')]
    else:
        result = analyze(report['stages'], report['stimulus']['motionStartEpochMilliseconds'],
                         report['stimulus']['motionEndEpochMilliseconds'])
    output = {'source': str(source), 'results': result,
              'limitations': 'Recorded busy windows only; no physical latency proof, no transport attribution for residual.'}
    Path(sys.argv[2]).write_text(json.dumps(output, indent=2), encoding='utf-8')
    print(json.dumps(result, indent=2))
