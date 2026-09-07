"""Summarize bounded work2 corpora; missing endpoints never become PASS."""
import json
from pathlib import Path
from math import ceil
import importlib.util
import sys

sys.dont_write_bytecode = True

spec = importlib.util.spec_from_file_location('work_analysis', Path(__file__).with_name('analyze-framework-work.py'))
module = importlib.util.module_from_spec(spec)
spec.loader.exec_module(module)
root = Path(__file__).resolve().parent / 'artifacts/sample-perf'

def stats(values):
    values = sorted(values)
    return {'count':len(values), 'median':values[len(values)//2] if values else None,
            'p95':values[ceil(len(values)*.95)-1] if values else None, 'max':max(values) if values else None}

result = {'schema':'doroti.work2-results/v1', 'minimal':{}, 'detailed':[], 'flutter':[], 'sweep':[],
          'limitation':'Submit notifications, CPU and ARIA are separate endpoints. Three-run p95 is descriptive, not population qualification.'}
for scope in ['broad','local','restart']:
    runs, intervals, onsets = [], [], []
    for path in sorted(root.glob(f'work2-final-{scope}-?.json')):
        d = json.loads(path.read_text())
        onset = d['causalOnset']['milliseconds']
        if onset is not None: onsets.append(onset)
        commits = [e['timestampMicroseconds']/1000+d['after']['timeOrigin'] for e in d['resizeTrace']
                   if e['phase']=='front-commit' and e['source']=='worker-direct-surface' and
                   d['before']['time']+1000 <= e['timestampMicroseconds']/1000+d['after']['timeOrigin'] <= d['before']['time']+6000]
        intervals.extend(b-a for a,b in zip(commits,commits[1:]))
        runs.append({'label':d['label'], 'onsetMs':onset, 'steady':d['steadyCommitIntervals'], 'errors':d['errors']})
    result['minimal'][scope] = {'runs':runs,'onset':stats(onsets),'pooledSteady':stats(intervals),
        'onsetGate':'PASS-short-run' if len(onsets)==3 and max(onsets)<=50 else 'FAIL-or-incomplete',
        'callbackGate':'notMeasured in minimal mode'}
for path in sorted(root.glob('work2-detail-?.json')):
    result['detailed'].append(module.analyze(path))
for path in sorted(root.glob('work2-flutter-?.json')):
    result['flutter'].append(json.loads(path.read_text()))
for path in sorted(root.glob('work2-sweep-?.json')):
    d=json.loads(path.read_text())
    segments=[]
    for segment in d['sweepSegments']:
        low,high=segment['before']['time'],segment['after']['time']
        frames=[e['durationMicroseconds']/1000 for e in d['resizeTrace'] if e['phase']=='framework-frame' and
                low<=e['timestampMicroseconds']/1000+d['after']['timeOrigin']<=high]
        managed=next((v['managed'] for v in segment['diagnostics'] if v),{})
        trace=managed.get('frame',{}).get('Skia',{}).get('Trace',[])
        build=[]
        for i,e in enumerate(trace):
            if e['Phase']!=7:continue
            end=next((x for x in trace[i+1:] if x['Phase'] in [7,8]),None)
            if end and end['Phase']==8:build.append((end['RecordedAtMicroseconds']-e['RecordedAtMicroseconds'])/1000)
        segments.append({'column':segment['column'],'visit':segment['visit'],'callback':stats(frames),
                         'retainedTraceBuild':stats(build),
                         'limitation':'retainedTraceBuild can include pre-segment frames; callback timestamps are segment-filtered'})
    result['sweep'].append({'label':d['label'],'segments':segments,'errors':d['errors']})
output = Path(__file__).resolve().parents[2] / 'artifacts/framework-web-work2/summary.json'
output.parent.mkdir(parents=True,exist_ok=True)
output.write_text(json.dumps(result,indent=2),encoding='utf-8')
print(output)
