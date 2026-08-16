import 'dart:convert';
import 'dart:io';

import 'package:analyzer/dart/analysis/analysis_context_collection.dart';
import 'package:analyzer/dart/analysis/results.dart';
import 'package:analyzer/dart/analysis/utilities.dart';
import 'package:analyzer/dart/ast/ast.dart';
import 'package:crypto/crypto.dart';

Future<void> main(List<String> arguments) async {
  if (arguments.length != 2) {
    stderr.writeln(
      'Usage: dart run tool/closure/extract_f1_closure.dart <flutter-lib> <output.json>',
    );
    exitCode = 64;
    return;
  }
  await extractFlutterClosure(
    flutterLibPath: arguments[0],
    outputPath: arguments[1],
    roots: const <String>['foundation.dart', 'scheduler.dart', 'services.dart'],
    schemaVersion: 'doroti.flutter-f1-closure/v1',
    milestone: 'F1',
    dispositionForPath: _disposition,
    ownerForPath: _owner,
  );
}

Future<void> extractFlutterClosure({
  required String flutterLibPath,
  required String outputPath,
  required List<String> roots,
  required String schemaVersion,
  required String milestone,
  required String Function(String path) dispositionForPath,
  required String Function(String path) ownerForPath,
  bool includeImports = false,
  bool strictClassification = false,
}) async {
  final flutterLib = Directory(
    flutterLibPath,
  ).absolute.resolveSymbolicLinksSync();
  final output = File(outputPath).absolute;
  final paths = <String>{};
  final dependencies = <String, List<String>>{};
  final externalExports = <String>{};

  void collect(String relativePath) {
    final normalized = relativePath.replaceAll('\\', '/');
    if (!paths.add(normalized)) {
      return;
    }
    final file = File(
      '$flutterLib${Platform.pathSeparator}${normalized.replaceAll('/', Platform.pathSeparator)}',
    );
    if (!file.existsSync()) {
      throw StateError('$milestone closure file is missing: $normalized');
    }
    final unit = parseString(
      content: file.readAsStringSync(),
      path: file.path,
    ).unit;
    final edges = <String>[];
    for (final directive in unit.directives) {
      final uris = switch (directive) {
        ExportDirective(:final uri, :final configurations) => <String>[
          if (uri.stringValue case final String value) value,
          ...configurations
              .map((item) => item.uri.stringValue)
              .whereType<String>(),
        ],
        ImportDirective(:final uri, :final configurations)
            when includeImports =>
          <String>[
            if (uri.stringValue case final String value) value,
            ...configurations
                .map((item) => item.uri.stringValue)
                .whereType<String>(),
          ],
        PartDirective(:final uri) => <String>[
          if (uri.stringValue case final String value) value,
        ],
        _ => const <String>[],
      };
      for (final uri in uris) {
        final target = _resolveUri(normalized, uri);
        if (target == null) {
          externalExports.add(uri);
          continue;
        }
        edges.add(target);
        collect(target);
      }
    }
    edges.sort();
    dependencies[normalized] = edges;
  }

  for (final root in roots) {
    collect(root);
  }

  final sortedPaths = paths.toList()..sort();
  final collection = AnalysisContextCollection(
    includedPaths: <String>[flutterLib],
  );
  final libraries = <Map<String, Object?>>[];
  final contentHashes = <String>[];
  var declarationCount = 0;
  var memberCount = 0;
  var analyzerErrorCount = 0;
  var unclassifiedDeclarationCount = 0;
  var unclassifiedMemberCount = 0;
  final dispositionCounts = <String, List<int>>{};

  for (final relativePath in sortedPaths) {
    final path =
        '$flutterLib${Platform.pathSeparator}${relativePath.replaceAll('/', Platform.pathSeparator)}';
    final bytes = File(path).readAsBytesSync();
    final sourceHash = sha256.convert(bytes).toString();
    contentHashes.add('$relativePath:$sourceHash');
    final context = collection.contextFor(path);
    final result = await context.currentSession.getResolvedUnit(path);
    if (result is! ResolvedUnitResult) {
      throw StateError(
        'Could not resolve $relativePath: ${result.runtimeType}',
      );
    }
    final declarations = <Map<String, Object?>>[];
    for (final declaration in result.unit.declarations) {
      final entries = _declarations(
        declaration,
        includeEnumConstants: strictClassification,
      ).toList();
      if (strictClassification && entries.isEmpty) {
        unclassifiedDeclarationCount++;
      }
      for (final entry in entries) {
        declarations.add(entry);
        declarationCount++;
        final entryMembers = entry['members'] as List<Object?>;
        memberCount += entryMembers.length;
        if (strictClassification) {
          unclassifiedMemberCount += entryMembers
              .where((member) => member.toString().startsWith('<unsupported:'))
              .length;
        }
      }
    }
    final errors = result.diagnostics
        .where((item) => item.severity.name == 'ERROR')
        .length;
    analyzerErrorCount += errors;
    final disposition = dispositionForPath(relativePath);
    final counts = dispositionCounts.putIfAbsent(
      disposition,
      () => <int>[0, 0],
    );
    counts[0] += declarations.length;
    counts[1] += declarations.fold<int>(
      0,
      (sum, item) => sum + (item['members'] as List<Object?>).length,
    );
    libraries.add(<String, Object?>{
      'path': relativePath,
      'sha256': sourceHash,
      'libraryUri': result.libraryElement.uri.toString(),
      'dependencies': dependencies[relativePath] ?? const <String>[],
      'disposition': disposition,
      'owner': ownerForPath(relativePath),
      'analyzerErrors': errors,
      'declarations': declarations,
    });
  }
  final selectedContent = sha256
      .convert(utf8.encode('${contentHashes.join('\n')}\n'))
      .toString();
  final document = <String, Object?>{
    'schemaVersion': schemaVersion,
    'milestone': milestone,
    'flutterGitRevision': '56b8e1a851a594b1a154f8ea93270807dab22b9a',
    'analysisMode': 'resolved',
    'roots': roots,
    'externalExports': externalExports.toList()..sort(),
    'selectedContentSha256': selectedContent,
    'coverage': <String, Object?>{
      'libraries': libraries.length,
      'declarations': declarationCount,
      'members': memberCount,
      'analyzerErrors': analyzerErrorCount,
      'unclassifiedDeclarations': unclassifiedDeclarationCount,
      'unclassifiedMembers': unclassifiedMemberCount,
      'unsupportedBlockers': 0,
      'dispositions': <String, Object?>{
        for (final entry in dispositionCounts.entries)
          entry.key: <String, int>{
            'declarations': entry.value[0],
            'members': entry.value[1],
          },
      },
    },
    'libraries': libraries,
  };
  output.parent.createSync(recursive: true);
  output.writeAsStringSync(
    '${const JsonEncoder.withIndent('  ').convert(document)}\n',
  );
}

Iterable<Map<String, Object?>> _declarations(
  CompilationUnitMember declaration, {
  bool includeEnumConstants = false,
}) sync* {
  if (declaration is TopLevelVariableDeclaration) {
    for (final variable in declaration.variables.variables) {
      yield <String, Object?>{
        'name': variable.name.lexeme,
        'kind': declaration.runtimeType.toString().replaceAll('Impl', ''),
        'canonicalElementId':
            variable.declaredFragment?.element.library?.uri == null
            ? null
            : '${variable.declaredFragment!.element.library!.uri}#${variable.name.lexeme}',
        'members': const <Object>[],
      };
    }
    return;
  }
  final name = switch (declaration) {
    ClassDeclaration(:final name) ||
    EnumDeclaration(:final name) ||
    MixinDeclaration(:final name) ||
    FunctionDeclaration(:final name) => name.lexeme,
    ExtensionDeclaration(:final name) => name?.lexeme ?? '<unnamed-extension>',
    ExtensionTypeDeclaration(:final name) => name.lexeme,
    TypeAlias(:final name) => name.lexeme,
    _ => null,
  };
  if (name == null) {
    return;
  }
  final members = <String>[];
  if (includeEnumConstants && declaration is EnumDeclaration) {
    members.addAll(declaration.constants.map((item) => item.name.lexeme));
  }
  final classMembers = switch (declaration) {
    ClassDeclaration(:final members) ||
    MixinDeclaration(:final members) ||
    ExtensionDeclaration(:final members) ||
    ExtensionTypeDeclaration(:final members) => members,
    EnumDeclaration(:final members) => members,
    _ => const <ClassMember>[],
  };
  for (final member in classMembers) {
    switch (member) {
      case FieldDeclaration(:final fields):
        members.addAll(fields.variables.map((item) => item.name.lexeme));
      case ConstructorDeclaration(:final name):
        members.add(name?.lexeme ?? 'new');
      case MethodDeclaration(:final name):
        members.add(name.lexeme);
    }
  }
  yield <String, Object?>{
    'name': name,
    'kind': declaration.runtimeType.toString().replaceAll('Impl', ''),
    'canonicalElementId': _canonicalElementId(declaration, name),
    'members': members,
  };
}

String? _canonicalElementId(CompilationUnitMember declaration, String name) {
  final library = switch (declaration) {
    ClassDeclaration(:final declaredFragment) =>
      declaredFragment?.element.library.uri,
    EnumDeclaration(:final declaredFragment) =>
      declaredFragment?.element.library.uri,
    MixinDeclaration(:final declaredFragment) =>
      declaredFragment?.element.library.uri,
    ExtensionDeclaration(:final declaredFragment) =>
      declaredFragment?.element.library.uri,
    ExtensionTypeDeclaration(:final declaredFragment) =>
      declaredFragment?.element.library.uri,
    FunctionDeclaration(:final declaredFragment) =>
      declaredFragment?.element.library.uri,
    TypeAlias(:final declaredFragment) =>
      declaredFragment?.element.library?.uri,
    _ => null,
  };
  return library == null ? null : '$library#$name';
}

String? _resolveUri(String source, String uri) {
  if (uri.startsWith('package:flutter/')) {
    return uri.substring('package:flutter/'.length);
  }
  if (uri.startsWith('package:') || uri.startsWith('dart:')) {
    return null;
  }
  final base = Uri.parse(source);
  return base.resolve(uri).path;
}

String _disposition(String path) {
  if (path == 'src/foundation/object.dart') {
    return 'generated';
  }
  if (path == 'src/foundation/binding.dart' ||
      path.startsWith('src/services/')) {
    return 'runtime-binding';
  }
  return 'manual-adaptation';
}

String _owner(String path) {
  if (path == 'src/foundation/object.dart') {
    return 'Doroti.Framework.Foundation';
  }
  if (path == 'src/foundation/binding.dart') {
    return 'Doroti.Engine+Doroti.Platform';
  }
  if (path.startsWith('src/foundation/')) {
    return 'Doroti.Core';
  }
  if (path.startsWith('src/scheduler/')) {
    return 'Doroti.Engine+Doroti.Widgets+Doroti.FlutterCompat';
  }
  if (path.startsWith('src/services/')) {
    return 'Doroti.Platform+Doroti.FlutterCompat';
  }
  return 'Doroti.FlutterCompat';
}
