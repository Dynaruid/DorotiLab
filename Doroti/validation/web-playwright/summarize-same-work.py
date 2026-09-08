"""Report every paired raw value. Four callbacks do not establish a stable p95."""
import json
from pathlib import Path

rows = []
for pair in range(1, 4):
    for mode in ('before', 'hamt'):
        path = Path(f'artifacts/state-resize/same-work-{mode}-{pair}.json')
        x = json.loads(path.read_text())
        a, b = [x[key][0]['managed'] for key in ('profileBefore', 'profiles')]
        events = [e for e in x['after']['trace'] if e['timestampMicroseconds'] / 1000 >= x['before']['time']]
        callbacks = [e['durationMicroseconds'] / 1000 for e in events if e['phase'] == 'framework-frame']
        target = x['after']['snapshot']['resizeEpoch']['generation']
        exact = [e for e in events if e['phase'] == 'front-commit'
                 and json.loads(e['detail']).get('generation') == target
                 and json.loads(e['detail']).get('sceneDisposition') == 'exact-rendered']
        delta = [y - z for y, z in zip(b['components']['values'], a['components']['values'])]
        rows.append(dict(pair=pair, mode=mode, callbacksMs=callbacks, callbackMaxMs=max(callbacks),
            callbackTotalMs=sum(callbacks), mapCalls=delta[0], mapMs=delta[1]/10000,
            mapAllocatedBytes=delta[2], mapInputEntries=delta[3],
            managedAllocatedBytes=b['profile']['managedAllocatedBytes']-a['profile']['managedAllocatedBytes'],
            heapEstimateBefore=a['profile']['managedHeapBytes'], heapEstimateAfter=b['profile']['managedHeapBytes'],
            collections=[y-z for y,z in zip(b['profile']['gcCollections'],a['profile']['gcCollections'])],
            visited=x['visited'], inputs=len(x['steps']), preparation=x['preparation'], errors=x['errors'],
            finalGeneration=target, exactFinalCommit=bool(exact),
            finalSettleMs=exact[0]['timestampMicroseconds']/1000-x['end'] if exact else None))
result = dict(rows=rows, physicalDisplay='notVerified', gcPause='notMeasured',
              processMemory='notMeasured', jiterpreterStats='notMeasured',
              caveat='Short instrumented sequences; no stable p95, minimal-instrumentation control, or full sample target trace.')
Path('artifacts/state-resize/same-work-summary.json').write_text(json.dumps(result, indent=2))
for row in rows:
    print(json.dumps({key: row[key] for key in ('pair','mode','callbackMaxMs','callbackTotalMs','mapCalls','mapMs',
        'mapAllocatedBytes','managedAllocatedBytes','exactFinalCommit','finalSettleMs')}))
if any(row['errors'] or row['visited'] != list(range(29)) or not row['exactFinalCommit'] for row in rows):
    raise SystemExit('Invalid workload; do not promote')
