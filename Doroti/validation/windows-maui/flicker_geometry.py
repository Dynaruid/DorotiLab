"""Pixel geometry for the controlled-background resize capture, not OS bounds."""
import numpy as np


class DwmRightBorder:
    def __init__(self, frame, caption_height, client_width):
        self.y = caption_height - 8
        self.reference = frame[self.y-2:self.y+2, client_width:client_width+2, :3].astype(np.int16)
        if self.reference.shape != (4, 2, 3):
            raise ValueError('DWM outline is outside the capture')
        if abs(self.locate(frame) - client_width) > 2:
            raise ValueError('DWM outline calibration does not match the initial client')

    def locate(self, frame):
        # A last-matching-caption-fill search also matches the red background:
        # Acrylic RGB (187,43,43) differs from background RGB (192,48,48) by 5.
        # Instead match the two-pixel DWM outline and reject ambiguous captures.
        band = frame[self.y-2:self.y+2, :, :3].astype(np.int16)
        error = np.maximum(np.max(np.abs(band[:, :-1]-self.reference[:, :1]), axis=(0, 2)),
                           np.max(np.abs(band[:, 1:]-self.reference[:, 1:]), axis=(0, 2)))
        matches = np.flatnonzero(error < 4)
        if not len(matches) or matches[-1] - matches[0] > 2:
            raise ValueError('Displayed DWM outline missing/ambiguous; capture is not measurable')
        return int(matches[-1])
