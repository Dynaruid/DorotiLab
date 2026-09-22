import unittest
from analyze import union, stats


class Intervals(unittest.TestCase):
    def test_nested_raster_is_not_double_counted(self):
        self.assertEqual(union([(10, 30), (15, 20), (28, 40), (50, 60)]),
                         [[10, 40], [50, 60]])

    def test_idle_has_no_percentile(self):
        self.assertEqual(stats([]), {'n': 0, 'p50': None, 'p95': None, 'max': None})

    def test_order_independent_percentile(self):
        self.assertEqual(stats([4, 2, 1, 3]), {'n': 4, 'p50': 2, 'p95': 3, 'max': 4})


if __name__ == '__main__':
    unittest.main()
