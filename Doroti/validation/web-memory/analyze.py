"""Boundary memory accounting; never infer resident memory from WASM/cache capacity."""
import importlib.util
import json
import sys
from pathlib import Path

spec = importlib.util.spec_from_file_location(
    'frame_cost', Path(__file__).parent.parent / 'web-frame-cost' / 'analyze.py')
frame_cost = importlib.util.module_from_spec(spec)
spec.loader.exec_module(frame_cost)


def analyze(directory):
    root = Path(directory)
    timing = frame_cost.analyze(root)
    rows = []
    for segment in json.loads((root / 'segments.json').read_text()):
        after = segment['after'] or {}
        managed = after.get('managed', {})
        cache = managed.get('cache')
        backing = after.get('backing')
        if cache:
            assert cache['TextEntries'] <= cache['TextEntryLimit'], 'text cache exceeded limit'
            assert cache['RasterRgba8Bytes'] <= cache['RasterRgba8Limit'], 'raster cache exceeded limit'
        if backing:
            assert backing['width'] >= backing['requiredWidth'], 'backing crops width'
            assert backing['height'] >= backing['requiredHeight'], 'backing crops height'
        rows.append({'id': segment['id'], 'managedHeapEstimate': managed.get('managedHeapBytes'),
                     'wasmCapacity': after.get('wasmBytes'), 'cache': cache, 'backing': backing,
                     'nativeBudget': managed.get('nativeCacheBudgetBytes'),
                     'ganesh': managed.get('ganesh'),
                     'graphiteContextBudgetedBytes': managed.get('graphiteContextBudgetedBytes'),
                     'graphiteContextLimitBytes': managed.get('graphiteContextLimitBytes'),
                     'graphiteRecorderBudgetedBytes': managed.get('graphiteRecorderBudgetedBytes'),
                     'queue': after.get('webgpu'), 'textures': after.get('textures')})
    repeated = [r for r in rows if r['id'].startswith('visit-') and r['cache']][-3:]
    trends = {}
    if len(repeated) == 3:
        for key in ['TextEntries', 'TextFontResources', 'TextBytesEstimate', 'RasterRgba8Bytes']:
            values = [r['cache'][key] for r in repeated]
            trends[key] = {'values': values, 'strictlyIncreasing': values[0] < values[1] < values[2],
                           'nonDecreasingWithGrowth': values[0] <= values[1] <= values[2] and values[0] < values[2],
                           'stable': values[0] == values[1] == values[2]}
    result = {'run': root.name, 'segments': rows, 'lastThreeVisits': trends,
              'processResident': 'notMeasured', 'gpuResident': 'notMeasured',
              'physicalIPhone': 'notVerified', 'timing': timing['segments']}
    (root / 'memory-summary.json').write_text(json.dumps(result, indent=2))
    return result


if __name__ == '__main__':
    for directory in sys.argv[1:]:
        result = analyze(directory)
        print(result['run'], json.dumps(result['lastThreeVisits']))
