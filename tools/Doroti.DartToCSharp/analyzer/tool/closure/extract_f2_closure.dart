import 'dart:io';

import 'extract_f1_closure.dart' show extractFlutterClosure;

Future<void> main(List<String> arguments) async {
  if (arguments.length != 2) {
    stderr.writeln(
      'Usage: dart run tool/closure/extract_f2_closure.dart <flutter-lib> <output.json>',
    );
    exitCode = 64;
    return;
  }

  final flutterLib = Directory(arguments[0]).absolute;
  final roots = <String>[
    'gestures.dart',
    'physics.dart',
    'animation.dart',
    ..._matchingRoots(
      Directory(
        '${flutterLib.path}${Platform.pathSeparator}src${Platform.pathSeparator}widgets',
      ),
      'src/widgets',
      (name) =>
          name == 'scrollable.dart' ||
          name == 'viewport.dart' ||
          name.startsWith('scroll_') ||
          name.startsWith('sliver') ||
          name.startsWith('focus_'),
    ),
    ..._matchingRoots(
      Directory(
        '${flutterLib.path}${Platform.pathSeparator}src${Platform.pathSeparator}rendering',
      ),
      'src/rendering',
      (name) => name == 'viewport.dart' || name.startsWith('sliver'),
    ),
  ];

  await extractFlutterClosure(
    flutterLibPath: arguments[0],
    outputPath: arguments[1],
    roots: roots,
    schemaVersion: 'doroti.flutter-f2-closure/v1',
    milestone: 'F2',
    dispositionForPath: _disposition,
    ownerForPath: _owner,
    includeImports: true,
    strictClassification: true,
  );
}

Iterable<String> _matchingRoots(
  Directory directory,
  String prefix,
  bool Function(String name) include,
) sync* {
  final names =
      directory
          .listSync()
          .whereType<File>()
          .map((file) => file.uri.pathSegments.last)
          .where(include)
          .toList()
        ..sort();
  for (final name in names) {
    yield '$prefix/$name';
  }
}

String _disposition(String path) {
  if (path.startsWith('src/physics/') ||
      path == 'src/animation/curves.dart' ||
      path == 'src/animation/animation_controller.dart' ||
      path == 'src/gestures/arena.dart' ||
      path == 'src/gestures/pointer_router.dart' ||
      path == 'src/gestures/pointer_signal_resolver.dart' ||
      path == 'src/gestures/resampler.dart' ||
      path == 'src/gestures/velocity_tracker.dart') {
    return 'generated';
  }
  if (path.startsWith('src/services/') ||
      path.startsWith('src/painting/') ||
      path.startsWith('src/rendering/')) {
    return 'runtime-binding';
  }
  return 'manual-adaptation';
}

String _owner(String path) {
  final disposition = _disposition(path);
  if (disposition == 'generated') {
    return 'Doroti.Framework.F2';
  }
  if (disposition == 'runtime-binding') {
    return 'Doroti.Rendering+Doroti.Platform';
  }
  if (path.startsWith('src/foundation/')) {
    return 'Doroti.Core';
  }
  if (path.startsWith('src/scheduler/')) {
    return 'Doroti.Engine+Doroti.Widgets';
  }
  return 'Doroti.Widgets+Doroti.FlutterCompat';
}
