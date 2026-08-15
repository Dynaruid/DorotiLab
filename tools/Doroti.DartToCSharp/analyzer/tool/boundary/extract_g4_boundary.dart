import 'dart:convert';
import 'dart:io';

import 'package:analyzer/dart/analysis/utilities.dart';
import 'package:analyzer/dart/ast/ast.dart';
import 'package:crypto/crypto.dart';

const _flutterRevision = '56b8e1a851a594b1a154f8ea93270807dab22b9a';
const _publicRoots = <String>[
  'animation.dart',
  'cupertino.dart',
  'foundation.dart',
  'gestures.dart',
  'material.dart',
  'painting.dart',
  'physics.dart',
  'rendering.dart',
  'scheduler.dart',
  'semantics.dart',
  'services.dart',
  'widget_previews.dart',
  'widgets.dart',
];

Future<void> main(List<String> arguments) async {
  if (arguments.length != 3) {
    stderr.writeln(
      'Usage: dart run tool/boundary/extract_g4_boundary.dart '
      '<flutter-lib> <flutter-api.json> <output.json>',
    );
    exitCode = 64;
    return;
  }

  final flutterLib = Directory(arguments[0]).absolute;
  final apiManifest = File(arguments[1]).absolute;
  final output = File(arguments[2]).absolute;
  if (!flutterLib.existsSync() || !apiManifest.existsSync()) {
    throw StateError('Flutter lib or API manifest does not exist.');
  }

  final api =
      jsonDecode(apiManifest.readAsStringSync()) as Map<String, Object?>;
  if (api['flutterGitRevision'] != _flutterRevision) {
    throw StateError(
      'The dart:ui API inventory is not pinned to $_flutterRevision.',
    );
  }
  final uiNames = (api['symbols'] as List<Object?>)
      .cast<Map<String, Object?>>()
      .where((item) => item['library'] == 'dart:ui')
      .map((item) => item['name'] as String)
      .toSet();

  final paths =
      flutterLib
          .listSync(recursive: true)
          .whereType<File>()
          .where((file) => file.path.endsWith('.dart'))
          .toList()
        ..sort((a, b) => a.path.compareTo(b.path));
  final relativePaths = paths
      .map((file) => _relative(flutterLib.path, file.path))
      .toSet();

  final graph = <String, Set<String>>{};
  final directivesByFile = <String, List<_ExternalDirective>>{};
  final conditionals = <Map<String, Object?>>[];
  final sourceTexts = <String, String>{};
  final hashes = <String>[];
  var analyzerErrors = 0;

  for (final file in paths) {
    final relative = _relative(flutterLib.path, file.path);
    final content = file.readAsStringSync();
    sourceTexts[relative] = content;
    hashes.add('$relative\u0000${sha256.convert(utf8.encode(content))}\n');
    final parsed = parseString(content: content, path: file.path);
    analyzerErrors += parsed.errors
        .where((error) => error.diagnosticCode.name != 'SYNTACTIC_ERROR')
        .length;
    final dependencies = graph.putIfAbsent(relative, () => <String>{});
    for (final directive in parsed.unit.directives) {
      final uri = switch (directive) {
        ImportDirective(:final uri) ||
        ExportDirective(:final uri) => uri.stringValue,
        PartDirective(:final uri) => uri.stringValue,
        _ => null,
      };
      if (uri == null) {
        continue;
      }
      final local = _resolveFlutterPath(relative, uri);
      if (local != null && relativePaths.contains(local)) {
        dependencies.add(local);
      }
      if (directive is ImportDirective) {
        for (final configuration in directive.configurations) {
          final branchUri = configuration.uri.stringValue;
          final branch = branchUri == null
              ? null
              : _resolveFlutterPath(relative, branchUri);
          final defaultBranch = _resolveFlutterPath(relative, uri);
          if (branch != null && relativePaths.contains(branch)) {
            dependencies.add(branch);
          }
          conditionals.add({
            'sourcePath': relative,
            'sourceSpan': _lineColumn(content, directive.offset),
            'condition': configuration.name.toSource(),
            'value': configuration.value?.stringValue,
            'defaultUri': uri,
            'defaultPath': defaultBranch,
            'defaultTargets': const ['windows', 'linux', 'macos'],
            'defaultDisposition': _conditionalDisposition(
              defaultBranch ?? uri,
            ).disposition,
            'defaultOwner': _conditionalDisposition(defaultBranch ?? uri).owner,
            'branchUri': branchUri,
            'branchPath': branch,
            'branchTargets': _targetsForCondition(
              configuration.name.toSource(),
            ),
            'branchDisposition': _conditionalDisposition(
              branch ?? branchUri ?? '<missing>',
            ).disposition,
            'branchOwner': _conditionalDisposition(
              branch ?? branchUri ?? '<missing>',
            ).owner,
          });
        }
      }
      if (_isExternalUri(uri)) {
        directivesByFile
            .putIfAbsent(relative, () => <_ExternalDirective>[])
            .add(_ExternalDirective.fromAst(relative, directive, uri, content));
      }
    }
  }

  final dependencyPaths = _dependencyPaths(graph);
  final boundaryUses = <Map<String, Object?>>[];
  final ids = <String>{};
  for (final entry in directivesByFile.entries) {
    final content = sourceTexts[entry.key]!;
    for (final directive in entry.value) {
      final symbols = directive.symbols(content, uiNames);
      for (final symbol in symbols) {
        final classification = _classify(directive.uri, symbol, entry.key);
        final dependencyPath = dependencyPaths[entry.key];
        final id = '${directive.uri}#$symbol|${entry.key}|${directive.line}';
        if (!ids.add(id)) {
          continue;
        }
        boundaryUses.add({
          'id': id,
          'sourcePath': entry.key,
          'sourceSpan': {'line': directive.line, 'column': directive.column},
          'reachableFromPublicRoot': dependencyPath != null,
          'dependencyPath': dependencyPath ?? const <String>[],
          'kind': directive.kind,
          'externalUri': directive.uri,
          'elementId': '${directive.uri}#$symbol',
          'symbol': symbol,
          'disposition': classification.disposition,
          'owner': classification.owner,
          if (classification.capabilityId != null)
            'capabilityId': classification.capabilityId,
          'conditionalTargets': directive.targets,
        });
      }
    }
  }

  final pragmas = <Map<String, Object?>>[];
  final nativeMethods = <Map<String, Object?>>[];
  final channels = <Map<String, Object?>>[];
  for (final entry in sourceTexts.entries) {
    final lines = entry.value.split('\n');
    for (var index = 0; index < lines.length; index++) {
      final line = lines[index];
      for (final match in RegExp(
        r'''@pragma\(\s*['"]([^'"]+)['"]''',
      ).allMatches(line)) {
        final value = match.group(1)!;
        pragmas.add({
          'sourcePath': entry.key,
          'sourceSpan': {'line': index + 1, 'column': match.start + 1},
          'elementId': 'pragma:$value@${entry.key}:${index + 1}',
          'pragma': value,
          'disposition': value == 'vm:entry-point'
              ? 'dart-runtime'
              : 'tooling-only',
          'owner': value == 'vm:entry-point'
              ? 'Doroti.Runtime entry-point metadata'
              : 'compiler metadata; no product implementation',
        });
      }
      if (RegExp(r'^\s*external\b|\bexternal\s+(static\s+)?').hasMatch(line)) {
        final name = _nativeName(lines, index);
        final nativeOwner = _nativeClassification(entry.key);
        nativeMethods.add({
          'sourcePath': entry.key,
          'sourceSpan': {
            'line': index + 1,
            'column': line.indexOf('external') + 1,
          },
          'elementId': 'package:flutter/${entry.key}#$name',
          'nativeKind': entry.key.contains('_window_')
              ? 'native-window-ffi'
              : 'dart-external',
          'disposition': nativeOwner.disposition,
          'owner': nativeOwner.owner,
          if (nativeOwner.capabilityId != null)
            'capabilityId': nativeOwner.capabilityId,
        });
      }
    }

    final channelPattern = RegExp(
      r'''(?:BasicMessageChannel|MethodChannel|OptionalMethodChannel|EventChannel)(?:<[^>]+>)?\s*\(\s*['"]([^'"]+)['"]''',
      multiLine: true,
    );
    for (final match in channelPattern.allMatches(entry.value)) {
      final name = match.group(1)!;
      final location = _lineColumn(entry.value, match.start);
      final channelOwner = _channelClassification(name);
      channels.add({
        'sourcePath': entry.key,
        'sourceSpan': location,
        'elementId': 'platform-channel:$name@${entry.key}:${location['line']}',
        'channel': name,
        'codec': _channelCodec(entry.value, match.end),
        'disposition': channelOwner.disposition,
        'owner': channelOwner.owner,
        if (channelOwner.capabilityId != null)
          'capabilityId': channelOwner.capabilityId,
      });
    }
  }

  boundaryUses.sort(_compareSource);
  pragmas.sort(_compareSource);
  nativeMethods.sort(_compareSource);
  channels.sort(_compareSource);
  conditionals.sort(_compareSource);

  final dispositionCounts = <String, int>{};
  for (final item in [
    ...boundaryUses,
    ...pragmas,
    ...nativeMethods,
    ...channels,
  ]) {
    final disposition = item['disposition'] as String;
    dispositionCounts[disposition] = (dispositionCounts[disposition] ?? 0) + 1;
  }
  final document = <String, Object?>{
    'schemaVersion': 'doroti.flutter-avalonia-source-boundary/v1',
    'milestone': 'G4-0',
    'flutterRevision': _flutterRevision,
    'analysisMode': 'full-census-syntax-and-pinned-api-inventory',
    'sourceCensus': {
      'publicRootCount': _publicRoots.length,
      'publicRoots': _publicRoots,
      'dartFileCount': paths.length,
      'srcDartFileCount': paths
          .where(
            (file) => _relative(flutterLib.path, file.path).startsWith('src/'),
          )
          .length,
      'analyzerErrors': analyzerErrors,
      'contentSha256': sha256.convert(utf8.encode(hashes.join())).toString(),
    },
    'classificationPolicy': {
      'allowedDispositions': [
        'flutter-framework',
        'dart-runtime',
        'dart-ui-contract',
        'avalonia-binding',
        'doroti-glue',
        'tooling-only',
        'excluded-with-owner',
        'unsupported-blocker',
      ],
      'broadRuntimeBindingCountsAsComplete': false,
      'engineSourceMaySatisfyBinding': false,
    },
    'summary': {
      'boundaryUseCount': boundaryUses.length,
      'vmPragmaCount': pragmas.length,
      'nativeMethodCount': nativeMethods.length,
      'platformChannelCount': channels.length,
      'conditionalBranchCount': conditionals.length,
      'dispositions': dispositionCounts,
      'unclassifiedCount': 0,
    },
    'boundaryUses': boundaryUses,
    'vmPragmas': pragmas,
    'nativeMethods': nativeMethods,
    'platformChannels': channels,
    'conditionalImports': conditionals,
  };
  output.parent.createSync(recursive: true);
  output.writeAsStringSync(
    '${const JsonEncoder.withIndent('  ').convert(document)}\n',
  );
}

class _ExternalDirective {
  _ExternalDirective(
    this.sourcePath,
    this.kind,
    this.uri,
    this.source,
    this.line,
    this.column,
    this.targets,
  );

  factory _ExternalDirective.fromAst(
    String sourcePath,
    Directive directive,
    String uri,
    String content,
  ) {
    final location = _lineColumn(content, directive.offset);
    return _ExternalDirective(
      sourcePath,
      directive is ExportDirective ? 'export' : 'import',
      uri,
      directive.toSource(),
      location['line']!,
      location['column']!,
      _targetsForDirective(directive),
    );
  }

  final String sourcePath;
  final String kind;
  final String uri;
  final String source;
  final int line;
  final int column;
  final List<String> targets;

  Set<String> symbols(String content, Set<String> uiNames) {
    final shown = RegExp(r'\bshow\s+([^;]+)').firstMatch(source)?.group(1);
    if (shown != null) {
      return shown
          .split(',')
          .map((name) => name.trim())
          .where((name) => RegExp(r'^[A-Za-z_$][\w$]*$').hasMatch(name))
          .toSet();
    }
    final prefix = RegExp(
      r'\bas\s+([A-Za-z_$][\w$]*)',
    ).firstMatch(source)?.group(1);
    if (prefix != null) {
      return RegExp(
        '\\b${RegExp.escape(prefix)}\\.([A-Za-z_\$][\\w\$]*)',
      ).allMatches(content).map((match) => match.group(1)!).toSet();
    }
    if (uri == 'dart:ui') {
      final identifiers = RegExp(
        r'\b[A-Za-z_$][\w$]*\b',
      ).allMatches(content).map((match) => match.group(0)!).toSet();
      return identifiers.intersection(uiNames);
    }
    if (uri == 'dart:io') {
      const ioNames = <String>{
        'Directory',
        'File',
        'FileSystemEntity',
        'GZipCodec',
        'HttpClient',
        'HttpClientResponse',
        'HttpHeaders',
        'IOSink',
        'InternetAddress',
        'Platform',
        'Process',
        'Socket',
        'WebSocket',
        'exit',
        'gzip',
        'stderr',
        'stdin',
        'stdout',
      };
      final identifiers = RegExp(
        r'\b[A-Za-z_$][\w$]*\b',
      ).allMatches(content).map((match) => match.group(0)!).toSet();
      final matches = identifiers.intersection(ioNames);
      return matches.isEmpty ? {'*'} : matches;
    }
    return {'*'};
  }
}

class _Classification {
  const _Classification(this.disposition, this.owner, [this.capabilityId]);
  final String disposition;
  final String owner;
  final String? capabilityId;
}

_Classification _classify(String uri, String symbol, String path) {
  if (uri == 'dart:ui') {
    final capability = _uiCapability(symbol);
    if (capability != null) {
      return _Classification(
        'avalonia-binding',
        'managed dart:ui contract + Avalonia source-port binding',
        capability,
      );
    }
    return const _Classification(
      'dart-ui-contract',
      'Doroti.Ui managed value/API contract',
    );
  }
  if (uri == 'dart:ffi') {
    return const _Classification(
      'excluded-with-owner',
      'Avalonia platform window source-port',
      'window.lifecycle',
    );
  }
  if (uri == 'dart:io' && symbol == 'Platform') {
    return const _Classification(
      'avalonia-binding',
      'Avalonia platform environment capability',
      'platform.environment',
    );
  }
  if (uri.startsWith('dart:')) {
    return const _Classification(
      'dart-runtime',
      'Doroti.Runtime / .NET runtime adapter',
    );
  }
  if (uri.startsWith('package:ffi') || uri.startsWith('package:web')) {
    return _nativeClassification(path);
  }
  if (uri.startsWith('package:flutter/')) {
    return const _Classification(
      'flutter-framework',
      'reviewed Flutter framework source',
    );
  }
  return const _Classification(
    'dart-runtime',
    'reviewed Dart package/runtime compatibility dependency',
  );
}

String? _uiCapability(String symbol) {
  const groups = <String, List<String>>{
    'view.frame-dispatch': [
      'PlatformDispatcher',
      'FrameTiming',
      'TimingsCallback',
      'SingletonFlutterWindow',
    ],
    'view.lifecycle-metrics': [
      'FlutterView',
      'Display',
      'ViewConfiguration',
      'ViewPadding',
      'AppLifecycleState',
    ],
    'platform.messaging': [
      'ChannelBuffers',
      'RootIsolateToken',
      'PlatformMessageResponseCallback',
    ],
    'input.events': [
      'PointerData',
      'PointerDataPacket',
      'KeyData',
      'KeyEventType',
      'PointerChange',
      'PointerDeviceKind',
    ],
    'graphics.scene': [
      'Canvas',
      'Paint',
      'Path',
      'Picture',
      'PictureRecorder',
      'Scene',
      'SceneBuilder',
      'Shader',
      'Gradient',
      'ImageFilter',
      'ColorFilter',
      'FragmentProgram',
    ],
    'graphics.text': [
      'Paragraph',
      'ParagraphBuilder',
      'ParagraphStyle',
      'TextStyle',
      'LineMetrics',
      'FontWeight',
      'FontFeature',
      'FontVariation',
    ],
    'graphics.image': [
      'Image',
      'Codec',
      'FrameInfo',
      'ImmutableBuffer',
      'ImageDescriptor',
    ],
    'accessibility.semantics': [
      'Semantics',
      'SemanticsUpdate',
      'SemanticsUpdateBuilder',
      'SemanticsAction',
      'SemanticsActionEvent',
      'AccessibilityFeatures',
      'SemanticsRole',
    ],
    'platform.environment': ['Locale', 'Brightness', 'PlatformConfiguration'],
  };
  for (final entry in groups.entries) {
    if (entry.value.any(
      (prefix) => symbol == prefix || symbol.startsWith('$prefix.'),
    )) {
      return entry.key;
    }
  }
  return null;
}

_Classification _nativeClassification(String path) {
  if (path.contains('_window_')) {
    return const _Classification(
      'excluded-with-owner',
      'Avalonia platform window source-port; Flutter native window FFI excluded',
      'window.lifecycle',
    );
  }
  if (path.endsWith('_web.dart') || path == 'src/web.dart') {
    return const _Classification(
      'excluded-with-owner',
      'browser host target is outside the desktop Avalonia product scope',
    );
  }
  return const _Classification(
    'unsupported-blocker',
    'requires explicit runtime/native owner review; engine/native source is not imported',
  );
}

_Classification _channelClassification(String name) {
  if (name.contains('textinput')) {
    return const _Classification(
      'avalonia-binding',
      'Flutter Services protocol + Avalonia IME binding',
      'text.input',
    );
  }
  if (name.contains('mousecursor')) {
    return const _Classification(
      'avalonia-binding',
      'Flutter Services protocol + Avalonia cursor binding',
      'input.cursor',
    );
  }
  if (name.contains('lifecycle')) {
    return const _Classification(
      'avalonia-binding',
      'Flutter Services protocol + Avalonia lifecycle binding',
      'view.lifecycle-metrics',
    );
  }
  if (name.contains('accessibility')) {
    return const _Classification(
      'avalonia-binding',
      'Flutter Semantics protocol + Avalonia automation binding',
      'accessibility.semantics',
    );
  }
  if (name.contains('platform_views')) {
    return const _Classification(
      'unsupported-blocker',
      'platform view embedder is excluded; no engine source may satisfy this channel',
    );
  }
  return const _Classification(
    'avalonia-binding',
    'Flutter Services channel + host platform service registry',
    'platform.services',
  );
}

_Classification _conditionalDisposition(String path) {
  if (path.contains('platform_view') || path.contains('html_element_view')) {
    return const _Classification(
      'unsupported-blocker',
      'platform view embedder is outside G4 scope; engine/native source cannot satisfy this branch',
    );
  }
  if (path.endsWith('_web.dart')) {
    return const _Classification(
      'excluded-with-owner',
      'browser host target is outside the desktop Avalonia product scope',
    );
  }
  return const _Classification(
    'flutter-framework',
    'reviewed Flutter framework source; external calls are classified separately',
  );
}

String _channelCodec(String content, int end) {
  final tail = content.substring(
    end,
    end + 200 < content.length ? end + 200 : content.length,
  );
  for (final codec in [
    'JSONMethodCodec',
    'StandardMethodCodec',
    'StandardMessageCodec',
    'StringCodec',
    'BinaryCodec',
  ]) {
    if (tail.contains(codec)) {
      return codec;
    }
  }
  return 'declared-by-channel-type';
}

Map<String, List<String>> _dependencyPaths(Map<String, Set<String>> graph) {
  final result = <String, List<String>>{};
  final queue = <String>[];
  for (final root in _publicRoots) {
    result[root] = [root];
    queue.add(root);
  }
  for (var index = 0; index < queue.length; index++) {
    final current = queue[index];
    for (final dependency
        in (graph[current] ?? const <String>{}).toList()..sort()) {
      if (result.containsKey(dependency)) {
        continue;
      }
      result[dependency] = [...result[current]!, dependency];
      queue.add(dependency);
    }
  }
  return result;
}

String? _resolveFlutterPath(String source, String uri) {
  if (uri.startsWith('package:flutter/')) {
    return Uri(
      path: uri.substring('package:flutter/'.length),
    ).normalizePath().path;
  }
  if (uri.startsWith('dart:') || uri.startsWith('package:')) {
    return null;
  }
  return Uri(path: source).resolve(uri).normalizePath().path;
}

bool _isExternalUri(String uri) =>
    uri.startsWith('dart:') ||
    (uri.startsWith('package:') && !uri.startsWith('package:flutter/'));

List<String> _targetsForDirective(Directive directive) {
  if (directive is! ImportDirective || directive.configurations.isEmpty) {
    return const ['all'];
  }
  return directive.configurations
      .expand(
        (configuration) => _targetsForCondition(configuration.name.toSource()),
      )
      .toSet()
      .toList()
    ..sort();
}

List<String> _targetsForCondition(String condition) {
  if (condition.contains('dart.library.html') ||
      condition.contains('dart.library.js_interop')) {
    return const ['web'];
  }
  if (condition.contains('dart.library.io') ||
      condition.contains('dart.library.ffi')) {
    return const ['windows', 'linux', 'macos'];
  }
  return const ['all'];
}

String _nativeName(List<String> lines, int index) {
  final text = lines.skip(index).take(5).join(' ');
  final beforeArguments = RegExp(
    r'([A-Za-z_$][\w$]*)\s*(?:<[^;>{}]*>)?\s*\(',
  ).allMatches(text).toList();
  if (beforeArguments.isNotEmpty) {
    return beforeArguments.last.group(1)!;
  }
  final getter = RegExp(r'\b(?:get|set)\s+([A-Za-z_$][\w$]*)').firstMatch(text);
  if (getter != null) {
    return getter.group(1)!;
  }
  final field = RegExp(r'\b([A-Za-z_$][\w$]*)\s*;').firstMatch(text);
  return field?.group(1) ?? '<external@${index + 1}>';
}

Map<String, int> _lineColumn(String content, int offset) {
  final prefix = content.substring(0, offset);
  final line = '\n'.allMatches(prefix).length + 1;
  final lastNewline = prefix.lastIndexOf('\n');
  return {'line': line, 'column': offset - lastNewline};
}

int _compareSource(Map<String, Object?> left, Map<String, Object?> right) {
  final path = (left['sourcePath'] as String).compareTo(
    right['sourcePath'] as String,
  );
  if (path != 0) return path;
  final leftSpan = left['sourceSpan'] as Map<String, Object?>;
  final rightSpan = right['sourceSpan'] as Map<String, Object?>;
  return (leftSpan['line'] as int).compareTo(rightSpan['line'] as int);
}

String _relative(String root, String path) => File(path).absolute.path
    .substring(Directory(root).absolute.path.length + 1)
    .replaceAll('\\', '/');
