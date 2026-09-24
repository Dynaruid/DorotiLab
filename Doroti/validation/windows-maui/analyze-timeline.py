"""Join capture CPU observation intervals to native QPC events; not scan-out timing."""
import bisect
import json
from pathlib import Path
import sys

directory = Path(sys.argv[1])
capture = json.loads((directory / 'flicker-result.json').read_text(encoding='utf-8'))
timeline = json.loads((directory / 'timeline.json').read_text(encoding='utf-8'))
assert not timeline['truncated'], 'Timeline truncated; cannot infer complete ordering'
events = sorted(timeline['events'], key=lambda event: event['qpc'])
times = [event['qpc']/event['frequency'] for event in events]
rows = []
for frame in capture['frames']:
    start = frame['captureStartedQpcSeconds']
    end = frame['captureFinishedQpcSeconds']
    boundary = bisect.bisect_right(times, end)
    before = events[:boundary]
    applied = next((e for e in reversed(before)
                    if e['phase'] == 'message-exit' and e['message'] == 0x47), None)
    present = next((e for e in reversed(before) if e['phase'] == 'present-exit'), None)
    during = events[bisect.bisect_left(times, start):boundary]
    rows.append(dict(
        captureStartedQpcSeconds=start, captureFinishedQpcSeconds=end,
        displayedClientRight=frame['displayedClientRight'],
        layoutOutOfPhase=frame['layoutOutOfPhase'], captionOutOfPhase=frame['captionOutOfPhase'],
        exposedEdge=frame['exposedEdge'], lastWindowPosChanged=applied,
        lastPresentCall=present,
        eventsDuringCapture=[dict(qpc=e['qpc'], phase=e['phase'], message=e['message']) for e in during],
    ))
result = dict(
    interpretation='CPU observation intervals and API return times; no display/scan-out receipt',
    captureStatus=capture['status'], samples=len(rows), events=len(events),
    frames=rows,
)
(directory / 'timeline-correlation.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
print(json.dumps({k: v for k, v in result.items() if k != 'frames'}, indent=2))
