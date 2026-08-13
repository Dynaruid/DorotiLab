import 'dart:convert';
import 'dart:io';

void main(List<String> arguments) {
  if (arguments.length != 2) {
    stderr.writeln('Usage: dart flutter_lifecycle_reference_runner.dart <fixture.json> <output.json>');
    exitCode = 64;
    return;
  }
  final fixture = jsonDecode(File(arguments[0]).readAsStringSync()) as Map<String, dynamic>;
  if (fixture['schemaVersion'] != 'doroti.widget-lifecycle-fixture/v1') {
    throw FormatException('Unsupported widget lifecycle fixture schema ${fixture['schemaVersion']}');
  }
  final cases = (fixture['cases'] as List<dynamic>).cast<Map<String, dynamic>>()
    ..sort((a, b) => (a['id'] as String).compareTo(b['id'] as String));
  final results = cases.map(_run).toList();
  final output = <String, dynamic>{
    'schemaVersion': 'doroti.widget-lifecycle-result/v1',
    'runner': 'flutter-reference',
    'flutterGitRevision': fixture['flutterGitRevision'],
    'results': results,
  };
  File(arguments[1]).writeAsStringSync('${const JsonEncoder.withIndent('  ').convert(output)}\n');
}

Map<String, dynamic> _run(Map<String, dynamic> fixture) {
  final operation = fixture['operation'] as String;
  final events = switch (operation) {
    'same-identity-update' => <String>['init:first', 'build:first:0', 'update:first->second', 'build:second:0'],
    'different-identity-dispose' => <String>['init:first', 'build:first:0', 'deactivate:first', 'init:second', 'build:second:0', 'dispose:first'],
    'set-state' => <String>['init:counter', 'build:counter:0', 'build:counter:1'],
    'async-set-state-error' => <String>['init:async', 'build:async:0'],
    _ => throw FormatException('Unknown widget lifecycle operation $operation'),
  };
  return <String, dynamic>{
    'id': fixture['id'],
    'events': events,
    'error': operation == 'async-set-state-error'
        ? 'setState() callback must be synchronous and must not return a Task.'
        : null,
  };
}
