import 'dart:io';

import 'extract_f1_closure.dart' show extractFlutterClosure;

Future<void> main(List<String> arguments) async {
  if (arguments.length != 2) {
    stderr.writeln(
      'Usage: dart run tool/closure/extract_f4_closure.dart <flutter-lib> <output.json>',
    );
    exitCode = 64;
    return;
  }

  // Keep the dependency-fan-out order explicit. The final two public-library
  // roots prove that the pilot consumes Flutter's real export graph rather
  // than a hand-selected list of facade symbols.
  const roots = <String>[
    'src/material/theme.dart',
    'src/material/theme_data.dart',
    'src/material/text_theme.dart',
    'src/material/icons.dart',
    'src/cupertino/theme.dart',
    'src/cupertino/text_theme.dart',
    'src/cupertino/icons.dart',
    'src/material/filled_button.dart',
    'src/material/text_field.dart',
    'src/cupertino/button.dart',
    'src/cupertino/text_field.dart',
    'src/material/scaffold.dart',
    'src/material/navigation_bar.dart',
    'src/cupertino/page_scaffold.dart',
    'src/cupertino/nav_bar.dart',
    'src/material/dialog.dart',
    'src/cupertino/dialog.dart',
    'src/material/list_tile.dart',
    'src/widgets/form.dart',
    'src/cupertino/list_tile.dart',
    'src/cupertino/form_section.dart',
    'src/widgets/icon_data.dart',
    'src/widgets/icon_theme_data.dart',
    'src/widgets/localizations.dart',
    'src/widgets/media_query.dart',
    'src/services/asset_bundle.dart',
    'material.dart',
    'cupertino.dart',
  ];

  await extractFlutterClosure(
    flutterLibPath: arguments[0],
    outputPath: arguments[1],
    roots: roots,
    schemaVersion: 'doroti.flutter-f4-closure/v1',
    milestone: 'F4',
    dispositionForPath: _disposition,
    ownerForPath: _owner,
    includeImports: true,
    strictClassification: true,
  );
}

const generatedSlices = <String>{
  'src/material/theme.dart',
  'src/material/theme_data.dart',
  'src/material/text_theme.dart',
  'src/material/icons.dart',
  'src/material/filled_button.dart',
  'src/material/text_field.dart',
  'src/material/scaffold.dart',
  'src/material/navigation_bar.dart',
  'src/material/dialog.dart',
  'src/material/list_tile.dart',
  'src/cupertino/theme.dart',
  'src/cupertino/text_theme.dart',
  'src/cupertino/icons.dart',
  'src/cupertino/button.dart',
  'src/cupertino/text_field.dart',
  'src/cupertino/page_scaffold.dart',
  'src/cupertino/nav_bar.dart',
  'src/cupertino/dialog.dart',
  'src/cupertino/list_tile.dart',
  'src/cupertino/form_section.dart',
  'src/widgets/form.dart',
  'src/widgets/icon_data.dart',
  'src/widgets/icon_theme_data.dart',
  'src/widgets/localizations.dart',
};

String _disposition(String path) {
  if (generatedSlices.contains(path)) {
    return 'generated';
  }
  if (path.startsWith('src/services/') ||
      path == 'src/widgets/media_query.dart' ||
      path == 'src/widgets/image.dart' ||
      path == 'src/widgets/image_icon.dart') {
    return 'runtime-binding';
  }
  return 'manual-adaptation';
}

String _owner(String path) {
  final disposition = _disposition(path);
  if (disposition == 'generated') {
    return 'Doroti.Framework.F4';
  }
  if (disposition == 'runtime-binding') {
    return 'Doroti.FlutterCompat+Doroti.Composition+Doroti.Platform';
  }
  if (path.startsWith('src/foundation/')) {
    return 'Doroti.Core';
  }
  if (path.startsWith('src/painting/') || path.startsWith('src/rendering/')) {
    return 'Doroti.Rendering';
  }
  if (path.startsWith('src/material/') ||
      path.startsWith('src/cupertino/') ||
      path.startsWith('src/widgets/')) {
    return 'Doroti.FlutterCompat+Doroti.Widgets';
  }
  return 'Doroti.FlutterCompat';
}
