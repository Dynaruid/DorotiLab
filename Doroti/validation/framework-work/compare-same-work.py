"""Fail closed on contract loss or the first mismatch; never filter target events."""
import argparse
import json
from pathlib import Path

parser = argparse.ArgumentParser()
parser.add_argument('before', type=Path)
parser.add_argument('after', type=Path)
parser.add_argument('--flutter', type=Path)
parser.add_argument('--output', required=True, type=Path)
args = parser.parse_args()
before, after = [json.loads(path.read_text(encoding='utf-8-sig')) for path in [args.before, args.after]]
result = {'status': 'PASS', 'coverage': 'five-tick inherited/reparent fixture; not full sample parity'}
for name, value in [('before', before), ('after', after)]:
    if not value['trace']['Valid'] or value['trace']['Dropped'] != 0:
        result.update(status='FAIL', firstMismatch={'capture': name, 'reason': 'trace dropped events'})
        break
if result['status'] == 'PASS':
    for field, left, right in [('nodes', before['trace']['Nodes'], after['trace']['Nodes']),
                               ('events', before['trace']['Events'], after['trace']['Events']),
                               ('calls', before['calls'], after['calls'])]:
        if left != right:
            index = next((i for i, pair in enumerate(zip(left, right)) if pair[0] != pair[1]), min(len(left), len(right)))
            result.update(status='FAIL', firstMismatch={'field': field, 'index': index,
                'before': left[index] if index < len(left) else None,
                'after': right[index] if index < len(right) else None})
            break
if args.flutter:
    flutter = json.loads(args.flutter.read_text(encoding='utf-8-sig'))
    result['flutterCallbacks'] = 'PASS' if after['calls'] == flutter['calls'] else 'FAIL'
    if result['flutterCallbacks'] == 'FAIL':
        result.update(status='FAIL', flutterBefore=flutter['calls'], dorotiAfter=after['calls'])
    result['flutterInternalWrapperTrace'] = 'notVerified'
result.update(nodes=len(after['trace']['Nodes']), events=len(after['trace']['Events']))
args.output.write_text(json.dumps(result, indent=2), encoding='utf-8')
print(json.dumps(result))
raise SystemExit(0 if result['status'] == 'PASS' else 1)
