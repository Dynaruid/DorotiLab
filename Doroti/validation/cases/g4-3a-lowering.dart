import 'dart:async';

typedef MultiCallback = void Function(int value, String label, bool enabled);
typedef ValueChanged<T> = void Function(T value);

enum _LoweringDirection { forward, reverse }

class ConstructorTarget {
  const ConstructorTarget(this.value);

  final int value;
}

class SetterOwner {
  int value = 0;

  set forwardedValue(int newValue) {
    value = newValue;
  }
}

class WidgetBase {}
class WidgetChild extends WidgetBase {}

void exerciseContextualTypes(ValueChanged<String>? callback) {
  final ValueChanged<String>? localCallback = callback;
  localCallback?.call('ok');
  final List<WidgetBase> widgets = <WidgetChild>[WidgetChild()];
  if (widgets.isEmpty) {
    throw StateError('missing widget');
  }
  final List<int>? optionalValues = callback == null ? null : <int>[1];
  final bool optionalValuesAreEmpty = optionalValues?.isEmpty ?? true;
  if (!optionalValuesAreEmpty && optionalValues!.first == 0) {
    throw StateError('unexpected inferred value');
  }
}

Future<int> exerciseSchedulerServicesLowering(
  int value, {
  required bool enabled,
}) async {
  int local(int input) => input + 1;

  switch (enabled) {
    case true:
      value = local(value);
    case false:
      value = 0;
  }

  final int switched = switch (value) {
    0 => 10,
    _ => value,
  };

  // Dot shorthand + do-while (G4-4 Physics/Animation/Gestures lowering).
  _LoweringDirection direction = .forward;
  final int directed = switch (direction) {
    .forward => switched + 1,
    .reverse => switched - 1,
  };
  var countdown = directed > 0 ? 1 : 0;
  do {
    countdown -= 1;
  } while (countdown > 0);

  final constructors = <ConstructorTarget Function(int)>[
    ConstructorTarget.new,
  ];
  await Future<void>.value();
  try {
    return constructors[0](directed).value;
  } catch (_) {
    rethrow;
  }
}
