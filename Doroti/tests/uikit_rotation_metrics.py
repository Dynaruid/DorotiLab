"""Summarize rotation geometry updates separately from Metal presentation times."""
import statistics


def summarize_rotation(results):
    summaries = []
    for result in results:
        updates = result['updates']
        gaps = [(b['time'] - a['time']) * 1000 for a, b in zip(updates, updates[1:])]
        summary = {
            'orientation': result['orientation'],
            'distinctWidths': result['distinctWidths'],
            'meanViewportGapMs': statistics.mean(gaps) if gaps else None,
            'maxViewportGapMs': max(gaps) if gaps else None,
            'presentation': {'status': 'notMeasured', 'fps': None},
        }
        samples = result['samples']
        presentations = sorted(result.get('presentations', []), key=lambda p: p['time'])
        if samples and presentations:
            initial = (samples[0]['width'], samples[0]['height'])
            final = (samples[-1]['width'], samples[-1]['height'])
            intermediate = [i for i, p in enumerate(presentations)
                            if (p['width'], p['height']) not in (initial, final)]
            if intermediate:
                # Include the first final-size presentation, but exclude settled
                # replays and the pause between the two requested rotations.
                start = intermediate[0]
                end = next((i for i in range(intermediate[-1] + 1, len(presentations))
                            if (presentations[i]['width'], presentations[i]['height']) == final), None)
                if end is not None:
                    times = sorted({p['time'] for p in presentations[start:end + 1] if p['time'] > 0})
                    intervals = [(b - a) * 1000 for a, b in zip(times, times[1:])]
                    if intervals:
                        summary['presentation'] = {
                            'status': 'measured',
                            'source': 'Metal drawable PresentedTime; intermediate viewports through first final viewport',
                            'intervals': len(intervals),
                            'spanMs': sum(intervals),
                            'fps': 1000 / statistics.mean(intervals),
                            'meanGapMs': statistics.mean(intervals),
                            'maxGapMs': max(intervals),
                        }
        summaries.append(summary)
    return summaries
