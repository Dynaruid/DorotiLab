"""Summarize paired indexed runs without pooling callback distributions."""
import json
import statistics
from pathlib import Path

directory = Path('artifacts/indexed-hamt')
rows = []
for pair in range(1, 4):
    for mode in ('before', 'hamt'):
        data = json.loads((directory / f'indexed-{mode}-{pair}.json').read_text())
        if data['status'] != 'PASS' or data['errors']:
            raise SystemExit(f'Invalid run {mode}/{pair}')
        if data['prepared']['managed']['components']['visitedSections'] != list(range(29)):
            raise SystemExit('Not all 29 sections materialized')
        for segment in data['segments']:
            delta = segment['componentDelta']
            a, b = [segment[key]['managed']['work'] for key in ('profileBefore', 'profileAfter')]
            work = {}
            for name in ('Rebuild', 'LayoutEntry', 'LayoutWork', 'SetState'):
                index = b['Names'].index(name)
                work[name] = b['Samples'][-1]['Totals'][index] - a['Samples'][-1]['Totals'][index]
            row = {key: segment[key] for key in ('scenario', 'callbacksMs', 'callbackMaxMs', 'callbackTotalMs',
                'finalSettleMs', 'managedAllocatedBytes', 'heapEstimateBefore', 'heapEstimateAfter', 'gcCollections')}
            row.update(pair=pair, mode=mode, mapCalls=delta[0], mapMs=delta[1]/10000,
                       mapBytes=delta[2], mapInputEntries=delta[3], work=work,
                       target=segment['exactFinalGeneration'],
                       steps=segment['steps'], gpu=segment['after']['snapshot']['gpu'])
            rows.append(row)
comparison = []
for scenario in ('resize-wide', 'resize-break'):
    result = dict(scenario=scenario)
    for metric in ('callbackMaxMs', 'callbackTotalMs', 'finalSettleMs', 'managedAllocatedBytes', 'mapBytes', 'mapCalls'):
        before = [row[metric] for row in rows if row['scenario'] == scenario and row['mode'] == 'before']
        hamt = [row[metric] for row in rows if row['scenario'] == scenario and row['mode'] == 'hamt']
        a, b = statistics.median(before), statistics.median(hamt)
        result[metric] = dict(before=before, hamt=hamt, medianBefore=a, medianHamt=b,
                             changePercent=(b/a-1)*100 if a else None,
                             pairedChangesPercent=[(y/x-1)*100 if x else None for x, y in zip(before, hamt)])
    comparison.append(result)
report = dict(rows=rows, comparison=comparison,
    caveat='Three independent contexts per mode, short instrumented sequences. Medians are descriptive, not stable p95 or proof of physical FPS. Aggregate counts do not prove full target equivalence.')
(directory / 'summary.json').write_text(json.dumps(report, indent=2))
print(json.dumps(comparison, indent=2))
