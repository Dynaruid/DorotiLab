"""Fixed N4 corpus only. Preserve increases and endpoint limitations."""
import json
import sys
from pathlib import Path

root = Path(__file__).parent / 'artifacts'
rows = []
for label in ['n4l0', 'n4l1', 'n4t1', 'n4t0', 'n4b0', 'n4b1']:
    files = list((root / label / 'test-results').glob('*/fast-resize-0.json'))
    if len(files) != 1:
        raise ValueError(f'{label}: expected one original report, found {len(files)}')
    r = json.loads(files[0].read_text(encoding='utf-8-sig'))
    f = r['followingV2']
    rows.append({'label': label, 'report': str(files[0]), 'stimulus': r['stimulus'],
        'driverSha256': r['driverSha256'], 'manifest': r['manifest'],
        'environment': {key: r['native'].get(key) for key in ['displayRefreshHz','windowDpi','monitorRect']},
        'notification': f['caughtUp'], 'intervals': f['intervals'],
        'firstFrontMilliseconds': f['firstFrontMilliseconds'],
        'boundaryInclusiveGaps': f['boundaryInclusiveGaps'],
        'over100msGapCount': f['over100msGapCount'],
        'exactSettleMilliseconds': r['following']['settleMilliseconds'],
        'settleFromNativeEndMilliseconds': f['settleFromNativeEndMilliseconds'],
        'unreachedTargetCount': f['unreachedTargetCount'],
        'geometry': f['geometry'], 'activeMotionContentAge': f['activeMotionContentAge'],
        'idleBaselineAgeMilliseconds': f['idleBaselineAgeMilliseconds'],
        'nativeGeometry': f['nativeGeometry'], 'latencyStatus': r['following']['status']})
result = {'schema': 'doroti.n4-comparison/v1', 'rows': rows,
    'decision': 'C1 rejected: mixed native results, Bottom cadence/first/settle regressions; retain C0',
    'limitations': ['One pair per condition; no statistical superiority claim',
        'Trace/capture off; no WGC presentation or physical scan-out evidence',
        'Manifest source is harness checkout; actual app revision is identified by explicit serve root and response hashes',
        'nativeGeometry notComparable is preserved; geometry is logical CSS time-weighted coverage']}
Path(sys.argv[1]).write_text(json.dumps(result, indent=2), encoding='utf-8')
for row in rows:
    print(json.dumps({k:row[k] for k in ['label','notification','intervals','firstFrontMilliseconds',
                                       'exactSettleMilliseconds','activeMotionContentAge','geometry']}))
