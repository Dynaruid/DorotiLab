"""Analyze numeric intervals without adding nested framework/raster durations."""
import json
import sys
from pathlib import Path


def stats(values):
    values = sorted(values)
    if not values:
        return {"n": 0, "p50": None, "p95": None, "max": None}
    return {"n": len(values), "p50": values[int((len(values)-1)*.5)],
            "p95": values[int((len(values)-1)*.95)], "max": values[-1]}


def union(intervals):
    merged = []
    for start, end in sorted(intervals):
        if merged and start <= merged[-1][1]:
            merged[-1][1] = max(merged[-1][1], end)
        else:
            merged.append([start, end])
    return merged


def analyze(directory):
    root = Path(directory)
    result = {"run": root.name, "segments": []}
    for s in json.loads((root / 'segments.json').read_text()):
        trace = s['trace']
        rows = trace['rows'] if trace else []
        active = union([(r[3], r[4]) for r in rows])
        submits = [r for r in rows if r[0] == 3 and r[5] == 1]
        frameworks = [r for r in rows if r[0] == 1]
        # Idle gaps are deliberately excluded; S3 is a continuous animation.
        gaps = [b[4]-a[4] for a,b in zip(submits, submits[1:])] if s['id'] == 'S3' else []
        ingress = []
        for row in rows:
            if row[0] != 2 or not row[5]:
                continue
            following = next((r for r in submits if r[2] >= row[2] and r[4] >= row[4]), None)
            if following:
                ingress.append(following[4]-row[5])
        a = s['after']['managed'] if s['after'] else {}
        before = s['before']['managed'] if s['before'] else {}
        old = {e['id']:e for e in s.get('processBefore', {}).get('processInfo', [])}
        cpu = {}
        for e in s.get('processAfter', {}).get('processInfo', []):
            if e['id'] in old:
                cpu[e['type']] = cpu.get(e['type'], 0) + (e['cpuTime']-old[e['id']]['cpuTime'])*1000
        elapsed = s['end']-s['start']
        value = {"id": s['id'], "milliseconds": elapsed, "framework": stats([r[4]-r[3] for r in frameworks]),
                 "rasterSubmit": stats([r[4]-r[3] for r in rows if r[0] == 3]),
                 "ownerActiveMs": sum(b-a for a,b in active), "ownerActiveSlices": stats([b-a for a,b in active]),
                 "inputWatermarkToSubmitProxy": stats(ingress), "newSceneSubmits": len(submits),
                 "replaySubmits": sum(r[0] == 3 and r[5] == 0 for r in rows),
                 "activeSubmitGap": stats(gaps), "gapsOver50Ms": sum(x > 50 for x in gaps),
                 "frameworkOver16_7Fraction": sum(r[4]-r[3] > 16.7 for r in frameworks)/max(1,len(frameworks)),
                 "ownerAllocatedBytes": a.get('ownerAllocatedBytes'),
                 "bytesPerFrame": a.get('ownerAllocatedBytes',0)/max(1,len(frameworks)),
                 "bytesPerSecond": a.get('ownerAllocatedBytes',0)*1000/max(1,elapsed),
                 "managedHeapBytes": a.get('managedHeapBytes'), "wasmBytes": (s['after'] or {}).get('wasmBytes'),
                 "gcDelta": [v-u for u,v in zip(before.get('gcCollections',[]),a.get('gcCollections',[]))],
                 "processCpuMs": cpu, "droppedRecords": trace['dropped'] if trace else None,
                 "scrollOffsets": sorted(round(e['ScrollOffset'],3) for e in a.get('scroll',[]) or []),
                 "contentLabels": sorted((e['label'] or e.get('description')) for e in s['content'] if (e['label'] or e.get('description')) and 56 <= e['rect'][1] < 900),
                 "cache": a.get('skia',{}).get('Work')}
        result['segments'].append(value)
    (root / 'summary.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
    return result


if __name__ == '__main__':
    for directory in sys.argv[1:]:
        r = analyze(directory)
        print(r['run'])
        for s in r['segments']:
            print(s['id'], 'owner', round(s['ownerActiveMs'],1), 'framework p95', s['framework']['p95'],
                  'raster p95', s['rasterSubmit']['p95'], 'frames', s['framework']['n'],
                  'B/frame', round(s['bytesPerFrame']))
