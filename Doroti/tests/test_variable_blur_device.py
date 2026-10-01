import unittest

from variable_blur_device import summarize
from variable_blur_metal import interval_union


class PresentationSummaryTests(unittest.TestCase):
    def test_gpu_overlapping_channels_are_not_double_counted(self):
        self.assertEqual(interval_union([(20, 25), (0, 10), (5, 15), (2, 3)]), 20)
        self.assertEqual(interval_union([(0, 10), (10, 20)]), 20)

    def evidence(self, intervals):
        return {
            "surface": {
                "presentationIntervalsMilliseconds": intervals,
                "presentedDrawables": len(intervals) + 1,
                "graphicsBackend": "test",
                "pixelWidth": 1170, "pixelHeight": 2532, "devicePixelRatio": 3,
                "commandBuffersErrored": 0,
            },
            "frame": {"failed": 0},
        }

    def test_warm_window_excludes_launch_stall(self):
        result = summarize(self.evidence([5000] + [20] * 1600), 50)
        self.assertEqual(result["intervalSamples"], 1500)
        self.assertEqual(result["presentationMeanFps"], 50)
        self.assertEqual(result["presentationP99Ms"], 20)
        self.assertEqual(result["longIntervalFraction"], 0)
        self.assertIsNone(result["gpuMilliseconds"])

    def test_long_intervals_are_not_gpu_durations(self):
        result = summarize(self.evidence([20] * 250 + [40] * 750), 50)
        self.assertEqual(result["longIntervalFraction"], 1)
        self.assertEqual(result["presentationP50Ms"], 40)

    def test_rejects_incomplete_or_truncated_history(self):
        with self.assertRaisesRegex(ValueError, "Incomplete"):
            summarize(self.evidence([20] * 100), 50)
        evidence = self.evidence([20] * 1800)
        evidence["surface"]["presentedDrawables"] += 1
        with self.assertRaisesRegex(ValueError, "truncated"):
            summarize(evidence, 50)

    def test_raster_uses_recorded_duration_not_clamped_timestamps(self):
        evidence = self.evidence([20] * 1800)
        evidence["frame"]["trace"] = [
            {"phase": 13, "timestampMicroseconds": 1000, "queueLatencyMicroseconds": 999999},
            {"phase": 26, "timestampMicroseconds": 1000, "queueLatencyMicroseconds": 7000},
        ]
        result = summarize(evidence, 50)
        self.assertEqual(result["cpuRasterTraceTail"]["p95Ms"], 7)
        self.assertIsNone(result["gpuMilliseconds"])


if __name__ == "__main__":
    unittest.main()
