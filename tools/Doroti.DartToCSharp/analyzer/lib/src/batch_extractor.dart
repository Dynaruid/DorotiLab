import 'dart:convert';
import 'dart:io';

import 'package:analyzer/dart/analysis/results.dart';
import 'package:analyzer/dart/element/element.dart';
import 'package:crypto/crypto.dart';

import 'analysis_session.dart';
import 'extractor.dart';

Future<void> runBatchExtractor(List<String> args) async {
  if (args.length != 1) {
    stderr.writeln(
      'Usage: dart run entrypoints/extract_batch.dart <request.json>',
    );
    exitCode = 64;
    return;
  }
  final request = jsonDecode(await File(args.single).readAsString()) as Map;
  if (request['schemaVersion'] != 'doroti.dart-analyzer-batch/v1') {
    throw FormatException('Unsupported analyzer batch request schema.');
  }
  final syntaxOnly = request['syntaxOnly'] as bool? ?? false;
  final packagesPath = request['packagesPath'] as String?;
  final items = (request['items'] as List).cast<Map>();
  final paths = items.map((item) => item['path'] as String).toList();
  AnalysisSession? session;
  if (!syntaxOnly) {
    session = await AnalysisSession.create(paths, packagesPath);
  }
  final completions = <Map<String, Object?>>[];
  final dependencyHashes = <String, String>{};
  try {
    for (final item in items) {
      final ordinal = item['ordinal'] as int;
      final path = item['path'] as String;
      final outputPath = item['outputPath'] as String;
      final stopwatch = Stopwatch()..start();
      final resolved = session == null ? null : await session.resolve(path);
      final payload = await extractAnalyzerOutput(
        path,
        syntaxOnly: syntaxOnly,
        packagesPath: packagesPath,
        resolvedUnit: resolved,
      );
      final encoded =
          '${const JsonEncoder.withIndent('  ').convert(payload)}\n';
      final output = File(outputPath);
      await output.parent.create(recursive: true);
      final temporary = File('$outputPath.tmp.$pid');
      await temporary.writeAsString(encoded, flush: true);
      await temporary.rename(outputPath);
      stopwatch.stop();
      final dependencies = await _dependencies(
        path,
        resolved,
        dependencyHashes,
      );
      final dependenciesPath = '$outputPath.dependencies.json';
      final dependenciesFile = File(dependenciesPath);
      final dependenciesTemporary = File('$dependenciesPath.tmp.$pid');
      await dependenciesTemporary.writeAsString(
        jsonEncode(dependencies),
        flush: true,
      );
      await dependenciesTemporary.rename(dependenciesFile.path);
      completions.add({
        'ordinal': ordinal,
        'outputPath': outputPath,
        'outputBytes': encoded.length,
        'elapsedMicroseconds': stopwatch.elapsedMicroseconds,
        'dependenciesPath': dependenciesPath,
      });
    }
  } finally {
    await session?.dispose();
  }
  stdout.writeln(
    jsonEncode({
      'schemaVersion': 'doroti.dart-analyzer-batch-completion/v1',
      'analysisContextCount': session?.contextCount ?? 0,
      'contextSetupMicroseconds': session?.contextSetupMicroseconds ?? 0,
      'items': completions,
    }),
  );
}

Future<List<Map<String, String>>> _dependencies(
  String inputPath,
  ResolvedUnitResult? resolved,
  Map<String, String> hashes,
) async {
  final paths = <String>{File(inputPath).absolute.path};
  if (resolved != null) {
    final pending = <LibraryElement>[resolved.libraryElement];
    final visited = <String>{};
    while (pending.isNotEmpty) {
      final library = pending.removeLast();
      if (!visited.add(library.uri.toString())) continue;
      for (final fragment in library.fragments) {
        final sourcePath = fragment.source.fullName;
        if (File(sourcePath).existsSync()) paths.add(sourcePath);
        pending.addAll(fragment.importedLibraries);
        pending.addAll(
          fragment.libraryExports
              .map((item) => item.exportedLibrary)
              .whereType<LibraryElement>(),
        );
      }
    }
  }
  final dependencies = <Map<String, String>>[];
  for (final path in paths.toList()..sort()) {
    final file = File(path);
    if (!await file.exists()) continue;
    final hash = hashes[path] ??= sha256
        .convert(await file.readAsBytes())
        .toString();
    dependencies.add({'path': path, 'sha256': hash});
  }
  return dependencies;
}
