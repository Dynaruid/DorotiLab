class DynamicTarget {
  String invoke(int value) => 'dynamic:$value';
}

String describeRecord((int, String) value) => switch (value) {
  (int amount, String label) when amount > 1 => '$label:$amount',
  _ => 'small',
};

String runCollectionsPatternsDynamic() {
  final seed = <int>[1, 2];
  final enabled = seed.isNotEmpty;
  final int? nullableValue = seed.isNotEmpty ? 7 : null;
  final values = <int>[
    0,
    ...seed,
    if (enabled) 3,
    for (final value in seed) value + 3,
    ?nullableValue,
    for (int index = 0; index < 2; index += 1) index + 8,
  ];
  final states = <String>{
    if (enabled) 'selected',
    if (!enabled) 'disabled',
    ...<String>{'focused'},
  };
  dynamic target = DynamicTarget();
  return '${values.join(',')}|${states.join(',')}|'
      '${describeRecord((values.length, 'count'))}|'
      '${target.invoke(values.last)}';
}
