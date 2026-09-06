// Independent validation inputs for the pinned material_color_utilities package.
// Run using the reference app's package_config.json; this does not modify the app.
import 'dart:convert';
import 'package:material_color_utilities/material_color_utilities.dart';

Future<void> main() async {
  final quantized = await QuantizerCelebi().quantize(
    [0xffff0000, 0xff00ff00, 0xff0000ff], 1,
  );
  print(jsonEncode({
    'maxColors1': quantized.colorToCount.map((key, value) => MapEntry('$key', value)),
    'grayMajority': Score.score({0xff808080: 100, 0xffff0000: 10}, desired: 1),
    'emptyScore': Score.score({}, desired: 1),
  }));
}
