import 'dart:convert';
import 'dart:io';
import 'package:material_color_utilities/material_color_utilities.dart';

Future<void> main(List<String> args) async {
  for (final path in args) {
    final input = jsonDecode(File(path).readAsStringSync());
    final bytes = base64Decode(input['rgba']);
    final pixels = <int>[];
    for (var i = 0; i < bytes.length; i += 4) {
      pixels.add(bytes[i + 3] << 24 | bytes[i] << 16 | bytes[i + 1] << 8 | bytes[i + 2]);
    }
    final result = await QuantizerCelebi().quantize(pixels, 128);
    final seed = Score.score(result.colorToCount, desired: 1).first;
    final roles = <String, Map<String, int>>{};
    for (final dark in [false, true]) {
      final scheme = SchemeTonalSpot(sourceColorHct: Hct.fromInt(seed), isDark: dark, contrastLevel: 0);
      final colors = {
        'primary': MaterialDynamicColors.primary, 'onPrimary': MaterialDynamicColors.onPrimary,
        'primaryContainer': MaterialDynamicColors.primaryContainer, 'onPrimaryContainer': MaterialDynamicColors.onPrimaryContainer,
        'secondary': MaterialDynamicColors.secondary, 'onSecondary': MaterialDynamicColors.onSecondary,
        'secondaryContainer': MaterialDynamicColors.secondaryContainer, 'onSecondaryContainer': MaterialDynamicColors.onSecondaryContainer,
        'tertiary': MaterialDynamicColors.tertiary, 'onTertiary': MaterialDynamicColors.onTertiary,
        'tertiaryContainer': MaterialDynamicColors.tertiaryContainer, 'onTertiaryContainer': MaterialDynamicColors.onTertiaryContainer,
        'surface': MaterialDynamicColors.surface, 'onSurface': MaterialDynamicColors.onSurface,
        'surfaceContainer': MaterialDynamicColors.surfaceContainer, 'surfaceContainerLow': MaterialDynamicColors.surfaceContainerLow,
        'surfaceContainerLowest': MaterialDynamicColors.surfaceContainerLowest, 'surfaceContainerHigh': MaterialDynamicColors.surfaceContainerHigh,
        'surfaceContainerHighest': MaterialDynamicColors.surfaceContainerHighest, 'surfaceDim': MaterialDynamicColors.surfaceDim,
        'surfaceBright': MaterialDynamicColors.surfaceBright, 'onSurfaceVariant': MaterialDynamicColors.onSurfaceVariant,
        'outline': MaterialDynamicColors.outline, 'outlineVariant': MaterialDynamicColors.outlineVariant,
        'error': MaterialDynamicColors.error, 'onError': MaterialDynamicColors.onError,
        'errorContainer': MaterialDynamicColors.errorContainer, 'onErrorContainer': MaterialDynamicColors.onErrorContainer,
        'inverseSurface': MaterialDynamicColors.inverseSurface, 'onInverseSurface': MaterialDynamicColors.inverseOnSurface,
        'inversePrimary': MaterialDynamicColors.inversePrimary, 'shadow': MaterialDynamicColors.shadow, 'scrim': MaterialDynamicColors.scrim,
        'primaryFixed': MaterialDynamicColors.primaryFixed, 'primaryFixedDim': MaterialDynamicColors.primaryFixedDim,
        'onPrimaryFixed': MaterialDynamicColors.onPrimaryFixed, 'onPrimaryFixedVariant': MaterialDynamicColors.onPrimaryFixedVariant,
        'secondaryFixed': MaterialDynamicColors.secondaryFixed, 'secondaryFixedDim': MaterialDynamicColors.secondaryFixedDim,
        'onSecondaryFixed': MaterialDynamicColors.onSecondaryFixed, 'onSecondaryFixedVariant': MaterialDynamicColors.onSecondaryFixedVariant,
        'tertiaryFixed': MaterialDynamicColors.tertiaryFixed, 'tertiaryFixedDim': MaterialDynamicColors.tertiaryFixedDim,
        'onTertiaryFixed': MaterialDynamicColors.onTertiaryFixed, 'onTertiaryFixedVariant': MaterialDynamicColors.onTertiaryFixedVariant,
        'surfaceTint': MaterialDynamicColors.primary,
      };
      roles[dark ? 'dark' : 'light'] = colors.map((name, color) => MapEntry(name, color.getArgb(scheme)));
    }
    final output = {'seed': seed, 'colors': result.colorToCount.map((key, value) => MapEntry('$key', value)), 'schemes': roles};
    File('$path.oracle.json').writeAsStringSync(jsonEncode(output));
    final mismatches = <String>[];
    if (input['seed'] != seed) mismatches.add('seed ${input['seed']} != $seed');
    for (final theme in roles.keys) {
      for (final role in roles[theme]!.keys) {
        if (input['schemes'][theme][role] != roles[theme]![role]) mismatches.add('$theme/$role');
      }
    }
    final actualColors = Map<String, dynamic>.from(input['colors']);
    final expectedColors = result.colorToCount.map((key, value) => MapEntry('$key', value));
    final quantizerExact = actualColors.length == expectedColors.length && expectedColors.entries.every((e) => actualColors[e.key] == e.value);
    print(jsonEncode({'input': path, 'seed': seed, 'quantizerExact': quantizerExact, 'mismatches': mismatches}));
    if (!quantizerExact || mismatches.isNotEmpty) exitCode = 1;
  }
}
