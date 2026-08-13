import 'dart:convert';
import 'dart:io';

void main(List<String> arguments) {
  if (arguments.length != 2) {
    stderr.writeln(
      'Usage: dart flutter_foundation_key_reference_runner.dart <fixture.json> <output.json>',
    );
    exitCode = 64;
    return;
  }

  final fixture =
      jsonDecode(File(arguments[0]).readAsStringSync()) as Map<String, dynamic>;
  if (fixture['schemaVersion'] != 'doroti.foundation-key-fixture/v1') {
    throw FormatException(
      'Unsupported foundation key fixture schema ${fixture['schemaVersion']}',
    );
  }
  final cases = (fixture['cases'] as List<dynamic>).cast<Map<String, dynamic>>()
    ..sort((a, b) => (a['id'] as String).compareTo(b['id'] as String));
  final output = <String, dynamic>{
    'schemaVersion': 'doroti.foundation-key-result/v1',
    'runner': 'flutter-reference',
    'flutterGitRevision': fixture['flutterGitRevision'],
    'results': cases
        .map(
          (item) => <String, dynamic>{
            'id': item['id'],
            'value': _run(item['operation'] as String),
          },
        )
        .toList(),
  };
  File(arguments[1]).writeAsStringSync(
    '${const JsonEncoder.withIndent('  ').convert(output)}\n',
  );
}

bool _run(String operation) {
  final first = _ValueKey<int>(7);
  final second = _ValueKey<int>(7);
  return switch (operation) {
    'value-equal' => first == second,
    'value-different' => first == _ValueKey<int>(8),
    'generic-type-isolation' => first == _ValueKey<String>('7'),
    'subclass-isolation' => first == _PrivateValueKey<int>(7),
    'equal-hash' => first.hashCode == second.hashCode,
    'unique-same-instance' => _sameUnique(),
    'unique-distinct' => _UniqueKey() == _UniqueKey(),
    _ => throw FormatException('Unknown foundation key operation $operation'),
  };
}

bool _sameUnique() {
  final key = _UniqueKey();
  return key == key;
}

class _UniqueKey {}

class _ValueKey<T> {
  const _ValueKey(this.value);

  final T value;

  @override
  bool operator ==(Object other) =>
      other.runtimeType == runtimeType &&
      other is _ValueKey<T> &&
      other.value == value;

  @override
  int get hashCode => Object.hash(runtimeType, value);
}

class _PrivateValueKey<T> extends _ValueKey<T> {
  const _PrivateValueKey(super.value);
}
