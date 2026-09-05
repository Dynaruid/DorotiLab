"""Summarize independently launched Chromium trials; values are bytes, not MB."""
import json
from pathlib import Path
from statistics import median
import sys

source = Path(sys.argv[1] if len(sys.argv) > 1 else 'artifacts/memory-c0-c1-r1/runs.json')
data = json.loads(source.read_text(encoding='utf-8'))


def metrics(sample):
    processes = sample['processes']
    isolates = sample['isolates']
    assert len({row['isolateId'] for row in isolates}) == 3
    return {
        'privateBytes': processes['totalPrivateBytes'],
        'workingSetBytes': processes['totalWorkingSetBytes'],
        'rendererPrivateBytes': processes['rendererPrivateBytes'],
        'gpuProcessPrivateBytes': sum(row['privateBytes'] for row in processes['rows'] if row['type'] == 'GPU'),
        'v8UsedBytes': sum(row['heap']['usedSize'] for row in isolates),
        'v8AllocatedBytes': sum(row['heap']['totalSize'] for row in isolates),
        'backingStorageBytes': sum(row['heap']['backingStorageSize'] for row in isolates),
        'dotnetLinearMemoryBytes': next(row['dotnetLinearMemoryBytes'] for row in isolates if row['role'] == 'ui'),
    }


trials = []
for run in data['results']:
    assert run['status'] == 'PASS'
    assert run['completedTransitions'] == 140
    final = [metrics(s) for s in run['samples'] if s['label'].startswith('final-')]
    assert len(final) == 5
    trials.append({
        'pair': run['pair'], 'variant': run['variant'],
        'naturalFinal': {key: median(s[key] for s in final) for key in final[0]},
        'warm': metrics(next(s for s in run['samples'] if s['label'] == 'warm')),
        'postV8Gc': metrics(next(s for s in run['samples'] if s['label'] == 'post-v8-gc')),
    })

variants = {}
for variant in ('C0', 'C1'):
    selected = [r for r in trials if r['variant'] == variant]
    if not selected:
        continue
    variants[variant] = {'count': len(selected)}
    for stage in ('naturalFinal', 'warm', 'postV8Gc'):
        variants[variant][stage] = {
            key: {'median': median(r[stage][key] for r in selected),
                  'min': min(r[stage][key] for r in selected),
                  'max': max(r[stage][key] for r in selected)}
            for key in selected[0][stage]
        }

pairs = []
for pair in sorted({r['pair'] for r in trials}):
    selected = {r['variant']: r for r in trials if r['pair'] == pair}
    if len(selected) != 2:
        continue
    pairs.append({'pair': pair, **{
        stage: {key: selected['C1'][stage][key] - selected['C0'][stage][key]
                for key in selected['C0'][stage]}
        for stage in ('naturalFinal', 'warm', 'postV8Gc')
    }})

paired_summary = {
    stage: {key: {'median': median(p[stage][key] for p in pairs),
                  'min': min(p[stage][key] for p in pairs),
                  'max': max(p[stage][key] for p in pairs)}
            for key in pairs[0][stage]}
    for stage in ('naturalFinal', 'warm', 'postV8Gc')
} if pairs else {}
summary = {'source': str(source), 'complete': len(trials) == 8, 'trials': trials,
           'variants': variants, 'pairedC1MinusC0': pairs, 'pairedDeltaSummary': paired_summary}
source.with_name('summary.json').write_text(json.dumps(summary, indent=2) + '\n', encoding='utf-8')
print(f"Trials: {len(trials)}/8; all byte columns below converted to MiB")
for run in trials:
    m = run['naturalFinal']
    print(f"pair={run['pair']} {run['variant']} private={m['privateBytes']/2**20:.2f} "
          f"WS={m['workingSetBytes']/2**20:.2f} renderer={m['rendererPrivateBytes']/2**20:.2f} "
          f"WASM={m['dotnetLinearMemoryBytes']/2**20:.2f} V8={m['v8UsedBytes']/2**20:.2f}")
for variant, values in variants.items():
    print(variant, json.dumps({key: {stat: round(number/2**20, 3) for stat, number in value.items()}
                              for key, value in values['naturalFinal'].items()}))
