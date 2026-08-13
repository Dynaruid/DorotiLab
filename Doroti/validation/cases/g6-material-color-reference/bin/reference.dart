import 'dart:convert';

import 'package:material_color_utilities/material_color_utilities.dart';

void main() {
  const seeds = <int>[0xff6750a4, 0xff006e1c, 0xffb3261e];
  const contrasts = <double>[0.0, -1.0, 1.0];
  final cases = <Map<String, Object>>[];
  for (final seed in seeds) {
    for (final dark in <bool>[false, true]) {
      for (final contrast in contrasts) {
        final scheme = SchemeTonalSpot(
          sourceColorHct: Hct.fromInt(seed),
          isDark: dark,
          contrastLevel: contrast,
        );
        cases.add(<String, Object>{
          'seed': seed,
          'dark': dark,
          'contrast': contrast,
          'variant': 'tonalSpot',
          'roles': roles(scheme),
        });
      }
    }
  }
  for (final variant in <String>['fidelity', 'content', 'monochrome', 'neutral', 'vibrant', 'expressive', 'rainbow', 'fruitSalad']) {
    for (final dark in <bool>[false, true]) {
      final source = Hct.fromInt(seeds.first);
      final scheme = switch (variant) {
        'fidelity' => SchemeFidelity(sourceColorHct: source, isDark: dark, contrastLevel: 0),
        'content' => SchemeContent(sourceColorHct: source, isDark: dark, contrastLevel: 0),
        'monochrome' => SchemeMonochrome(sourceColorHct: source, isDark: dark, contrastLevel: 0),
        'neutral' => SchemeNeutral(sourceColorHct: source, isDark: dark, contrastLevel: 0),
        'vibrant' => SchemeVibrant(sourceColorHct: source, isDark: dark, contrastLevel: 0),
        'expressive' => SchemeExpressive(sourceColorHct: source, isDark: dark, contrastLevel: 0),
        'rainbow' => SchemeRainbow(sourceColorHct: source, isDark: dark, contrastLevel: 0),
        'fruitSalad' => SchemeFruitSalad(sourceColorHct: source, isDark: dark, contrastLevel: 0),
        _ => throw StateError(variant),
      };
      cases.add(<String, Object>{'seed': seeds.first, 'dark': dark, 'contrast': 0.0, 'variant': variant, 'roles': roles(scheme)});
    }
  }
  print(const JsonEncoder.withIndent('  ').convert(<String, Object>{
    'schemaVersion': 'dotori.g6-material-color-reference/v1',
    'materialColorUtilitiesVersion': '0.13.0',
    'variant': 'tonalSpot',
    'cases': cases,
  }));
}

Map<String, int> roles(DynamicScheme scheme) => <String, int>{
  'primary': scheme.primary,
  'onPrimary': scheme.onPrimary,
  'primaryContainer': scheme.primaryContainer,
  'onPrimaryContainer': scheme.onPrimaryContainer,
  'secondary': scheme.secondary,
  'tertiary': scheme.tertiary,
  'surface': scheme.surface,
  'surfaceDim': scheme.surfaceDim,
  'surfaceBright': scheme.surfaceBright,
  'surfaceContainer': scheme.surfaceContainer,
  'surfaceContainerHighest': scheme.surfaceContainerHighest,
  'onSurface': scheme.onSurface,
  'onSurfaceVariant': scheme.onSurfaceVariant,
  'outline': scheme.outline,
  'outlineVariant': scheme.outlineVariant,
};
