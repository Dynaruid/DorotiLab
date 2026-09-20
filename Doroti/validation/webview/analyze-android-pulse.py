"""Detect disappearance of the local sample's colored CSS pulse in a screenrecord.

Requires OpenCV/numpy. Select an unobscured ROI containing the pulse at every
animation position; inspect the saved first/lowest frames to validate this
precondition. This detects disappearance, not animation or drag frame pacing.
Run under validation/run-with-timeout.py.
"""
import argparse
import json
from pathlib import Path

import cv2
import numpy as np

p = argparse.ArgumentParser()
p.add_argument('--video', type=Path, required=True)
p.add_argument('--out', type=Path, required=True)
p.add_argument('--roi', type=int, nargs=4, metavar=('X', 'Y', 'WIDTH', 'HEIGHT'), required=True)
p.add_argument('--min-colored', type=int, default=1000)
a = p.parse_args()
a.out.mkdir(parents=True, exist_ok=False)
capture = cv2.VideoCapture(str(a.video))
fps = capture.get(cv2.CAP_PROP_FPS)
counts = []
lowest = None
x, y, width, height = a.roi
try:
    while True:
        ok, frame = capture.read()
        if not ok:
            break
        roi = frame[y:y + height, x:x + width].astype(np.int16)
        if roi.shape[:2] != (height, width):
            raise ValueError('ROI extends outside the video')
        count = int(((roi.max(2) > 65) & (roi.max(2) - roi.min(2) > 25)).sum())
        if not counts:
            cv2.imwrite(str(a.out / 'first.png'), frame)
        if not counts or count < min(counts):
            lowest = frame
        counts.append(count)
finally:
    capture.release()
if not counts:
    raise RuntimeError('No decoded frames')
cv2.imwrite(str(a.out / 'lowest.png'), lowest)
report = dict(video=str(a.video), roi=a.roi, frames=len(counts), fps=fps,
              minColored=min(counts), maxColored=max(counts),
              lowFrames=sum(c < a.min_colored for c in counts),
              threshold=a.min_colored,
              physicalObservation='notVerified', framePacing='notMeasured')
(a.out / 'result.json').write_text(json.dumps(report, indent=2) + '\n')
(a.out / 'counts.csv').write_text('frame,coloredPixels\n' + ''.join(f'{i},{c}\n' for i, c in enumerate(counts)))
print(json.dumps(report, indent=2))
raise SystemExit(1 if report['lowFrames'] else 0)
