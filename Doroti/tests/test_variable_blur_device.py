import unittest
import tempfile
from pathlib import Path

from variable_blur_device import summarize, run_order, frame_loop_summary
from variable_blur_metal import interval_union
from native_frame_pipeline_collect import payload_identity


class PresentationSummaryTests(unittest.TestCase):
    def test_gpu_overlapping_channels_are_not_double_counted(self):
        self.assertEqual(interval_union([(20, 25), (0, 10), (5, 15), (2, 3)]), 20)
        self.assertEqual(interval_union([(0, 10), (10, 20)]), 20)

    def test_c_conditions_alternate_and_resume_without_duplicates(self):
        modes = ["off", "fixed", "adaptive"]
        order = list(run_order(modes, 3))
        self.assertEqual(len(order), 9)
        self.assertEqual(len(set(order)), 9)
        self.assertEqual(order[:3], [(1, mode, "C") for mode in modes])
        self.assertEqual(order[3:6], [(2, mode, "C") for mode in reversed(modes)])
        self.assertTrue(all(policy == "C" for _, _, policy in order))
        self.assertEqual(list(run_order(modes, 2, 2)), order[3:])

    def test_apple_payload_tracks_assets_outside_the_executable_directory(self):
        with tempfile.TemporaryDirectory() as directory:
            bundle = Path(directory) / "Fixture.app"
            exe = bundle / "Contents/MacOS/Fixture"
            exe.parent.mkdir(parents=True)
            exe.write_bytes(b"executable")
            for relative in ("Contents/MonoBundle/Host.dll", "Contents/MonoBundle/libSkiaSharp.dylib",
                             "Contents/Frameworks/Native.framework/Native", "Contents/Info.plist"):
                asset = bundle / relative
                asset.parent.mkdir(parents=True, exist_ok=True)
                asset.write_bytes(b"before")
                before = payload_identity(exe)
                asset.write_bytes(b"after")
                after = payload_identity(exe)
                self.assertNotEqual(before["sha256"], after["sha256"], relative)
                self.assertIn(relative, [entry["path"] for entry in after["files"]])

    def test_overlap_uses_gpu_arrival_and_complete_warm_history(self):
        def event(phase, frame, timestamp, scene=1, duration=0):
            return {"phase": phase, "frameId": frame, "timestampMicroseconds": timestamp,
                "sceneId": scene, "durationMilliseconds": duration}
        events = [event("displayed", 0, 0), event("displayed", 9, 36_000_000),
            event("submitted", 1, 5_100_000), event("gpu-arrived", 1, 5_120_000),
            event("gpu-owner", 1, 5_150_000), event("raster-start", 2, 5_110_000),
            event("raster-end", 2, 5_115_000, duration=5), event("submitted", 2, 5_116_000),
            event("gpu-arrived", 2, 5_125_000), event("displayed", 2, 5_140_000),
            # Owner delay after GPU completion is not CPU/GPU overlap.
            event("raster-start", 3, 5_130_000), event("submitted", 3, 5_135_000),
            event("gpu-arrived", 3, 5_145_000), event("displayed", 3, 5_160_000)]
        result = frame_loop_summary({"policy": "C", "events": events, "maximumPending": 2})
        self.assertEqual(result["overlapCount"], 1)
        self.assertEqual(result["overlapFraction"], .5)
        hardware = frame_loop_summary({"policy": "C", "events": events +
            [event("gpu-ended", 1, 5_105_000), event("gpu-ended", 2, 5_120_000), event("gpu-ended", 3, 5_140_000)], "maximumPending": 2})
        self.assertEqual(hardware["overlapCount"], 0)
        self.assertEqual(hardware["gpuCompletionTimeline"], "same-queue terminal hardware end")
        self.assertEqual(result["cpuStages"]["raster-end"]["p50Ms"], 5)
        self.assertIsNone(result["inputToDisplay"])
        with self.assertRaisesRegex(ValueError, "Truncated"):
            frame_loop_summary({"omittedEvents": 1})

    def test_prepared_scene_latency_survives_a_raster_only_wake(self):
        def event(phase, frame, timestamp, scene=0, duration=0):
            return {"phase": phase, "frameId": frame, "sceneId": scene,
                "timestampMicroseconds": timestamp, "durationMilliseconds": duration}
        result = frame_loop_summary({"policy": "C", "maximumPending": 2, "events": [
            event("displayed", 0, 0), event("displayed", 99, 36_000_000),
            event("framework-start", 10, 5_008_000),
            event("callback", 10, 5_010_000, duration=1),
            event("scene-created", 10, 5_011_000, scene=100),
            event("raster-start", 11, 5_012_000), event("submitted", 11, 5_015_000, scene=100),
            event("gpu-arrived", 11, 5_025_000), event("displayed", 11, 5_040_000, scene=100)]})
        self.assertEqual(result["latency"]["callbackToDisplay"]["p50Ms"], 31)
        self.assertEqual(result["latency"]["frameworkToDisplay"]["p50Ms"], 32)

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
