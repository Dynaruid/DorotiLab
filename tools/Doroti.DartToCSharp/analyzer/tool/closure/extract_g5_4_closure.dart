import 'dart:io';

import 'extract_f1_closure.dart' show extractFlutterClosure;

Future<void> main(List<String> arguments) async {
  if (arguments.length != 2) {
    stderr.writeln(
      'Usage: dart run tool/closure/extract_g5_4_closure.dart <flutter-lib> <output.json>',
    );
    exitCode = 64;
    return;
  }

  const roots = <String>[
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
    'widgets.dart',
  ];

  await extractFlutterClosure(
    flutterLibPath: arguments[0],
    outputPath: arguments[1],
    roots: roots,
    schemaVersion: 'doroti.flutter-g5-4-closure/v1',
    milestone: 'G5-4',
    dispositionForPath: _disposition,
    ownerForPath: _owner,
    includeImports: true,
    strictClassification: true,
  );
}

String _disposition(String path) =>
    path.startsWith('src/material/') ||
        path.startsWith('src/cupertino/')
    ? 'generated'
    : 'reviewed-predecessor';

String _owner(String path) {
  if (path.startsWith('src/material/')) {
    return 'Doroti.Framework.Material';
  }
  if (path.startsWith('src/cupertino/')) {
    return 'Doroti.Framework.Cupertino';
  }
  return 'Doroti.Framework.Predecessor';
}
