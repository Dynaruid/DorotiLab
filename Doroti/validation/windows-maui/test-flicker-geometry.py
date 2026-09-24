"""Reject the caption/background color collision that hid the moving border."""
from pathlib import Path
import sys
import unittest

sys.path.insert(0, str(Path(__file__).resolve().parents[2] / 'artifacts/validation/capture-python'))
import numpy as np
from flicker_geometry import DwmRightBorder


def frame(width):
    pixels = np.full((120, 240, 3), (192, 48, 48), dtype=np.uint8)
    pixels[:, :width] = (187, 43, 43)
    pixels[:, width:width+2] = (150, 73, 73)
    return pixels


class BorderTests(unittest.TestCase):
    def test_similar_background_does_not_become_fixed_border(self):
        border = DwmRightBorder(frame(100), 64, 100)
        for width in (100, 148, 76):
            self.assertEqual(border.locate(frame(width)), width)

    def test_missing_border_is_unmeasurable(self):
        border = DwmRightBorder(frame(100), 64, 100)
        with self.assertRaises(ValueError):
            border.locate(np.full((120, 240, 3), (192, 48, 48), dtype=np.uint8))

    def test_wrong_calibration_is_rejected(self):
        with self.assertRaises(ValueError):
            DwmRightBorder(frame(100), 64, 110)

    def test_two_distinct_borders_are_ambiguous(self):
        border = DwmRightBorder(frame(100), 64, 100)
        pixels = frame(100)
        pixels[:, 160:162] = (150, 73, 73)
        with self.assertRaises(ValueError):
            border.locate(pixels)


if __name__ == '__main__':
    unittest.main()
