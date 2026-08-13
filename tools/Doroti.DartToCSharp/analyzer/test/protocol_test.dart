import 'dart:convert';
import 'dart:io';

import 'package:test/test.dart';
import 'package:doroti_dart_analyzer/src/local_storage.dart';

void main() {
  test('syntax-only protocol is v3 and byte deterministic', () async {
    final temporary = createDorotiTemporaryDirectory('analyzer-protocol');
    addTearDown(() => temporary.deleteSync(recursive: true));
    final source = File('${temporary.path}${Platform.pathSeparator}sample.dart')
      ..writeAsStringSync('class Sample { final int value = 1; }\n');

    final first = await _run(<String>[source.path, '--syntax-only']);
    final second = await _run(<String>[source.path, '--syntax-only']);

    expect(first.exitCode, 0, reason: first.stderr as String?);
    expect(second.exitCode, 0, reason: second.stderr as String?);
    expect(second.stdout, first.stdout);
    final document = jsonDecode(first.stdout as String) as Map<String, Object?>;
    expect(document['schemaVersion'], 'doroti.dart-analyzer-output/v3');
    expect(document['analysisMode'], 'syntax-only');
    expect(document['declarations'], isA<List<Object?>>());
    expect(document['diagnostics'], isA<List<Object?>>());
    expect(document.keys.toSet(), <String>{
      'schemaVersion',
      'analysisMode',
      'libraryUri',
      'imports',
      'directives',
      'libraryGraph',
      'declarations',
      'diagnostics',
    });
  });

  test('invalid arguments fail without protocol output', () async {
    final result = await _run(<String>[]);

    expect(result.exitCode, 64);
    expect(result.stdout, isEmpty);
    expect(result.stderr, contains('Usage: dart run entrypoints/extract.dart'));
  });

  test('resolved single and batch payloads are byte identical', () async {
    final temporary = createDorotiTemporaryDirectory('analyzer-batch-identity');
    addTearDown(() => temporary.deleteSync(recursive: true));
    File(
      '${temporary.path}${Platform.pathSeparator}pubspec.yaml',
    ).writeAsStringSync('name: batch_identity\nenvironment:\n  sdk: ^3.9.0\n');
    final source = File('${temporary.path}${Platform.pathSeparator}sample.dart')
      ..writeAsStringSync('class Sample { final int value = 1; }\n');
    final output = File('${temporary.path}${Platform.pathSeparator}batch.json');
    final request =
        File('${temporary.path}${Platform.pathSeparator}request.json')
          ..writeAsStringSync(
            jsonEncode({
              'schemaVersion': 'doroti.dart-analyzer-batch/v1',
              'syntaxOnly': false,
              'packagesPath': null,
              'items': [
                {'ordinal': 0, 'path': source.path, 'outputPath': output.path},
              ],
            }),
          );

    final single = await _run(<String>[source.path]);
    final batch = await Process.run(Platform.resolvedExecutable, <String>[
      'run',
      'entrypoints/extract_batch.dart',
      request.path,
    ], workingDirectory: Directory.current.path);

    expect(single.exitCode, 0, reason: single.stderr as String?);
    expect(batch.exitCode, 0, reason: batch.stderr as String?);
    expect(output.readAsStringSync(), single.stdout);
  });
}

Future<ProcessResult> _run(List<String> arguments) => Process.run(
  Platform.resolvedExecutable,
  <String>['run', 'entrypoints/extract.dart', ...arguments],
  workingDirectory: Directory.current.path,
);
