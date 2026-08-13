import 'package:analyzer/dart/analysis/results.dart';
import 'package:analyzer/src/dart/analysis/analysis_context_collection.dart';

import 'extractor.dart';

final class AnalysisSession {
  AnalysisSession._(this._collection, this.contextSetupMicroseconds);

  final AnalysisContextCollectionImpl _collection;
  final int contextSetupMicroseconds;

  int get contextCount => _collection.contexts.length;

  static Future<AnalysisSession> create(
    List<String> paths,
    String? packagesPath,
  ) async {
    final stopwatch = Stopwatch()..start();
    final resolvedPackages = packagesPath == null
        ? null
        : await materializeFrameworkPackageConfig(packagesPath);
    final collection = AnalysisContextCollectionImpl(
      includedPaths: paths,
      packagesFile: resolvedPackages,
    );
    stopwatch.stop();
    return AnalysisSession._(collection, stopwatch.elapsedMicroseconds);
  }

  Future<ResolvedUnitResult> resolve(String path) async {
    final result = await _collection
        .contextFor(path)
        .currentSession
        .getResolvedUnit(path);
    if (result is! ResolvedUnitResult) {
      throw StateError(
        'Expected ResolvedUnitResult for $path, got ${result.runtimeType}.',
      );
    }
    return result;
  }

  Future<void> dispose() => _collection.dispose();
}
