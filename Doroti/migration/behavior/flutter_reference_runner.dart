import 'dart:convert';
import 'dart:io';

void main(List<String> arguments) {
  if (arguments.length != 2) {
    stderr.writeln('Usage: dart flutter_reference_runner.dart <fixture.json> <output.json>');
    exitCode = 64;
    return;
  }
  final fixture = jsonDecode(File(arguments[0]).readAsStringSync()) as Map<String, dynamic>;
  if (fixture['schemaVersion'] != 'doroti.behavior-fixture/v1') {
    throw FormatException('Unsupported behavior fixture schema ${fixture['schemaVersion']}');
  }
  final cases = (fixture['cases'] as List<dynamic>).cast<Map<String, dynamic>>()..sort((a, b) => (a['id'] as String).compareTo(b['id'] as String));
  final results = cases.map(_run).toList();
  final output = <String, dynamic>{
    'schemaVersion': 'doroti.behavior-result/v1',
    'runner': 'flutter-reference',
    'flutterGitRevision': fixture['flutterGitRevision'],
    'results': results,
  };
  File(arguments[1]).writeAsStringSync('${const JsonEncoder.withIndent('  ').convert(output)}\n');
}

Map<String, dynamic> _run(Map<String, dynamic> fixture) {
  final source = fixture['constraints'] as Map<String, dynamic>;
  var minWidth = (source['minWidth'] as num).toDouble();
  var maxWidth = (source['maxWidth'] as num).toDouble();
  var minHeight = (source['minHeight'] as num).toDouble();
  var maxHeight = (source['maxHeight'] as num).toDouble();
  if (fixture['operation'] == 'loosen-constrain') {
    minWidth = 0;
    minHeight = 0;
  } else if (fixture['operation'] == 'deflate-constrain') {
    final horizontal = (fixture['insetLeft'] as num).toDouble() + (fixture['insetRight'] as num).toDouble();
    final vertical = (fixture['insetTop'] as num).toDouble() + (fixture['insetBottom'] as num).toDouble();
    minWidth = (minWidth - horizontal).clamp(0, double.infinity);
    maxWidth = (maxWidth - horizontal).clamp(0, double.infinity);
    minHeight = (minHeight - vertical).clamp(0, double.infinity);
    maxHeight = (maxHeight - vertical).clamp(0, double.infinity);
  } else if (fixture['operation'] != 'constrain') {
    throw FormatException('Unknown behavior operation ${fixture['operation']}');
  }
  final width = (fixture['width'] as num).toDouble().clamp(minWidth, maxWidth);
  final height = (fixture['height'] as num).toDouble().clamp(minHeight, maxHeight);
  return <String, dynamic>{'id': fixture['id'], 'width': width, 'height': height};
}
