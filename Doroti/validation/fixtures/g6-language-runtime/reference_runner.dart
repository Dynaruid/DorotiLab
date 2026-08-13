import 'dart:convert';

import 'collections_patterns_dynamic.dart';
import 'constructors_initializers.dart';
import 'future_typed_values.dart';
import 'generic_variance.dart';
import 'member_resolution.dart';
import 'null_aware_late_required.dart';
import 'nullable_super_defaults.dart';
import 'tearoffs_callbacks.dart';

Future<void> main() async {
  final results = <String, String>{
    'nullable-super-defaults': runNullableSuperDefaults(),
    'constructors-initializers': runConstructorsInitializers(),
    'generic-variance': runGenericVariance(),
    'future-typed-values': await runFutureTypedValues(),
    'null-aware-late-required': runNullAwareLateRequired(upper: true),
    'member-resolution': runMemberResolution(),
    'tearoffs-callbacks': runTearoffsCallbacks(),
    'collections-patterns-dynamic': runCollectionsPatternsDynamic(),
  };
  print(jsonEncode(results));
}
