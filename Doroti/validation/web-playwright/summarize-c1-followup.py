"""Summarize the bounded C1 -> encoder -> mapper follow-up without native claims."""
import importlib.util
import json
from pathlib import Path
from statistics import median
import sys
sys.dont_write_bytecode = True
root = Path(__file__).resolve().parents[3]
checkpoint = root / '.doroti/checkpoints/c1-next'
artifacts = Path(__file__).parent / 'artifacts'
spec = importlib.util.spec_from_file_location('stationary', Path(__file__).with_name('analyze-stationary-work.py'))
module = importlib.util.module_from_spec(spec)
spec.loader.exec_module(module)
labels = ['c1-next-baseline', 'c1-next-c2', 'c1-next-c3',
          'c1-next-c3-r2', 'c1-next-c2-r2', 'c1-next-c1-r2']
browser = []
for label in labels:
    files = list((artifacts / label / 'test-results').rglob('stationary-work.json'))
    if not files:
        continue
    result = module.analyze(files[0])
    raw = json.loads(files[0].read_text())
    active = []
    for variant in result['variants']:
        if not variant['trace']:
            continue
        assert variant['uiDropped'] == 0 and variant['workDropped'] == 0
        active.extend(f for f in variant['framePhasesAfterFirstResize']['frames']
                      if f['workWithinBuildToSubmit'].get('MediaDependentCheck', 0) > 0)
    browser.append({'label': label, 'source': str(files[0]),
        'traceOffTransitionMeans': [v['stationaryFrameMeanMilliseconds'] for v in result['variants'] if not v['trace']],
        'resizeFrames': active,
        'resizePhaseMedians': {key: median(f['milliseconds'][key] for f in active)
                               for key in ('uiFrame', 'build', 'mapping', 'encoding')},
        'served': [s['served'] for s in raw['samples']]})
managed = {}
for path in checkpoint.glob('*measure*.stdout.log'):
    rows = []
    for line in path.read_text(encoding='utf-8-sig').splitlines():
        if line.startswith(('ENCODER_MEASURE ', 'MAPPER_MEASURE ')):
            row = json.loads(line.split(' ', 1)[1])
            row['bytesPerCall'] = median(b['allocatedBytes']/b['iterations'] for b in row['batches'])
            row['microsecondsPerCall'] = median(b['milliseconds']*1000/b['iterations'] for b in row['batches'])
            rows.append(row)
    managed[path.stem] = rows
result = {'browser': browser, 'managed': managed,
          'limitations': 'CLR fixture allocation; stationary polling means are not native p95/FPS; initial managed timings include tiering unless log label states stable.'}
(artifacts / 'c1-next-summary.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
for row in browser:
    print(row['label'], 'resize frames', len(row['resizeFrames']), row['resizePhaseMedians'])
for name, rows in managed.items():
    print(name, [{k:v for k,v in r.items() if k != 'batches'} for r in rows])
