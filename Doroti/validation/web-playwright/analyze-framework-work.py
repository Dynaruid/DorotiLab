"""Causal trace analysis. Inclusive spans are never added to their children."""
import json
import sys
from pathlib import Path

def analyze(path):
    data = json.loads(path.read_text(encoding='utf-8-sig'))
    managed = (data.get('onsetDiagnostics') or data.get('directDiagnostics') or [{}])[0].get('managed', {})
    trace = managed.get('frame', {}).get('Skia', {}).get('Trace', [])
    request = data.get('causalOnset', {}).get('requestId')
    result = dict(label=data['label'], onset=data.get('causalOnset'), errors=data['errors'],
                  callback=data['frameworkFrameMilliseconds'], steady=data['steadyCommitIntervals'])
    raster_index = next((i for i, e in enumerate(trace) if e['Phase'] == 13 and e['CausalFrameId'] == request and not e['Reason']), None)
    if raster_index is None:
        return result | {'attribution': 'notMeasured: no causal managed raster'}
    build_index = next((i for i in range(raster_index, -1, -1) if trace[i]['Phase'] == 7), None)
    if build_index is None:
        return result | {'attribution': 'missing build boundary'}
    next_build = next((i for i in range(build_index+1,len(trace)) if trace[i]['Phase'] == 7), len(trace))
    following = trace[build_index:next_build]
    phase = {p: next((e for e in following if e['Phase'] == p), None) for p in [7, 8, 9, 10, 27, 28, 32, 33]}
    spans = {}
    for name, a, b in [('build', 7, 8), ('layoutCompositing', 8, 9), ('paint', 9, 10), ('semantics', 27, 28), ('finalize', 32, 33)]:
        if phase[a] and phase[b]:
            spans[name] = (phase[b]['RecordedAtMicroseconds'] - phase[a]['RecordedAtMicroseconds']) / 1000
            if spans[name] < 0: raise ValueError(f'{path}: negative {name} span')
    work = managed.get('work', {})
    boundaries = {s['Boundary']['TraceSequence']: s for s in work.get('Samples', [])}
    start, end = (boundaries.get(phase[p]['Sequence']) if phase[p] else None for p in [7, 8])
    result['work'] = {name: b-a for name, a, b in zip(work['Names'], start['Totals'], end['Totals']) if b != a} if start and end else {'missing': True}
    profile = managed.get('profile', {})
    names = {e['Id']: e for e in profile.get('entries', [])}
    frame = next((f for f in profile.get('frames', []) if f[0] == phase[7]['Sequence']), None)
    result['topBuildTypes'] = [dict(type=names[frame[i]]['Type'], kind=names[frame[i]]['Kind'], calls=frame[i+1], inclusiveMs=frame[i+2]/1000, selfMs=frame[i+3]/1000) for i in range(2,22,4) if frame[i+1]] if frame else []
    before = (data.get('profileBefore') or [{}])[0].get('managed', {}).get('profile', {})
    old = {e['Id']: e for e in before.get('entries', [])}
    result['hostProfileWindow'] = [dict(type=e['Type'], kind=e['Kind'], calls=e['Calls']-old.get(e['Id'],{}).get('Calls',0), inclusiveMs=(e['InclusiveMicroseconds']-old.get(e['Id'],{}).get('InclusiveMicroseconds',0))/1000) for e in profile.get('entries',[]) if e['Kind'] >= 2]
    result['spansMs'] = spans
    result['semanticsAfterRaster'] = bool(phase[27] and phase[27]['Sequence'] > trace[raster_index]['Sequence'])
    result['dropped'] = {'work':work.get('Dropped'), 'profile':profile.get('dropped'), 'profileFrames':profile.get('framesDropped')}
    result['limitation'] = 'Top five types by self time; self excludes instrumented children only. Host profile covers the whole action window. No scan-out or GC pause attribution.'
    return result

if __name__ == '__main__':
    print(json.dumps([analyze(Path(p)) for p in sys.argv[1:]], indent=2))
