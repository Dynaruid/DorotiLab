import 'dart:io';

import 'extract_f1_closure.dart' show extractFlutterClosure;

Future<void> main(List<String> arguments) async {
  if (arguments.length != 2) {
    stderr.writeln(
      'Usage: dart run tool/closure/extract_f3_closure.dart <flutter-lib> <output.json>',
    );
    exitCode = 64;
    return;
  }

  const roots = <String>[
    'painting.dart',
    'rendering.dart',
    'src/widgets/framework.dart',
    'src/widgets/binding.dart',
    'src/widgets/basic.dart',
    'src/widgets/text.dart',
    'src/widgets/focus_manager.dart',
    'src/widgets/focus_scope.dart',
    'src/widgets/focus_traversal.dart',
    'src/widgets/overlay.dart',
    'src/widgets/navigator.dart',
    'src/widgets/routes.dart',
    'src/widgets/editable_text.dart',
    'src/widgets/media_query.dart',
    'src/widgets/image.dart',
  ];

  await extractFlutterClosure(
    flutterLibPath: arguments[0],
    outputPath: arguments[1],
    roots: roots,
    schemaVersion: 'doroti.flutter-f3-closure/v1',
    milestone: 'F3',
    dispositionForPath: _disposition,
    ownerForPath: _owner,
    includeImports: true,
    strictClassification: true,
  );
}

String _disposition(String path) {
  if (path == 'src/widgets/framework.dart' ||
      path == 'src/rendering/object.dart' ||
      path == 'src/painting/basic_types.dart' ||
      path == 'src/painting/alignment.dart') {
    return 'generated';
  }
  if (path.startsWith('src/services/') ||
      path == 'src/painting/image_provider.dart' ||
      path == 'src/painting/image_cache.dart' ||
      path == 'src/rendering/binding.dart') {
    return 'runtime-binding';
  }
  return 'manual-adaptation';
}

String _owner(String path) {
  final disposition = _disposition(path);
  if (disposition == 'generated') {
    return 'Doroti.Generated.Framework.F3';
  }
  if (disposition == 'runtime-binding') {
    return 'Doroti.Rendering+Doroti.Composition+Doroti.Platform';
  }
  if (path.startsWith('src/foundation/')) {
    return 'Doroti.Core';
  }
  if (path.startsWith('src/painting/') || path.startsWith('src/rendering/')) {
    return 'Doroti.Rendering';
  }
  return 'Doroti.Widgets+Doroti.FlutterCompat';
}
