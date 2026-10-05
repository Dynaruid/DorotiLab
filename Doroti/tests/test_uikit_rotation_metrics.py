import unittest
from uikit_rotation_metrics import summarize_rotation


class RotationMetricsTests(unittest.TestCase):
    def result(self, presentations):
        return dict(orientation='LandscapeLeft', distinctWidths=4,
                    updates=[{'time': 1}, {'time': 1.1}],
                    samples=[dict(width=390, height=844), dict(width=844, height=390)],
                    presentations=presentations)

    def test_callbacks_without_presentations_do_not_become_display_fps(self):
        measured = summarize_rotation([self.result([])])[0]
        self.assertAlmostEqual(measured['meanViewportGapMs'], 100)
        self.assertEqual(measured['presentation'], {'status': 'notMeasured', 'fps': None})

    def test_excludes_settled_replays_and_deduplicates_presentation_time(self):
        frames = [dict(time=t, width=w, height=h) for t, w, h in
                  [(1, 390, 844), (1.1, 500, 734), (1.12, 600, 634),
                   (1.12, 600, 634), (1.14, 844, 390), (1.5, 844, 390)]]
        measured = summarize_rotation([self.result(frames)])[0]['presentation']
        self.assertEqual(measured['intervals'], 2)
        self.assertAlmostEqual(measured['fps'], 50)
        self.assertAlmostEqual(measured['spanMs'], 40)

    def test_requires_final_viewport_and_intermediate_presentations(self):
        for frames in [[dict(time=1.1, width=500, height=734)],
                       [dict(time=1.1, width=844, height=390)]]:
            self.assertEqual(summarize_rotation([self.result(frames)])[0]['presentation']['status'], 'notMeasured')


if __name__ == '__main__':
    unittest.main()
