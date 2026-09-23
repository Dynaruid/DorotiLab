import 'dart:math' as math;
import 'dart:io';

class RuntimeInteropProbe {
  double curve(double value) => math.sin(value) + math.pi;
  double seeded(int seed) => math.Random(seed).nextDouble();
  Duration delay() => const Duration(milliseconds: 3);
  String filePath(String path) => File(path).path;
}
