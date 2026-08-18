// Extracted semantic fixture from Flutter 56b8e1a8.
// Sources: packages/flutter/lib/src/animation/tween.dart,
// packages/flutter/lib/src/painting/geometry.dart, and
// packages/flutter/lib/src/foundation/consolidate_response.dart.

abstract class Animatable<T> {
  T transform(double t);
}

class Tween<T extends Object?> extends Animatable<T> {
  Tween({this.begin, this.end});

  T? begin;
  T? end;

  T lerp(double t) {
    assert(begin != null);
    assert(end != null);
    return (begin as dynamic) + ((end as dynamic) - (begin as dynamic)) * t as T;
  }

  @override
  T transform(double t) {
    if (t == 0.0) {
      return begin as T;
    }
    if (t == 1.0) {
      return end as T;
    }
    return lerp(t);
  }
}

Offset interpolateOffset(Offset begin, Offset end, double t) =>
    begin + (end - begin) * t;

Rect? interpolateRect(Rect? begin, Rect? end, double t) =>
    Rect.lerp(begin, end, t);

Future<List<int>> cancellableResponse(Stream<List<int>> response) async {
  final completer = Completer<List<int>>();
  late final StreamSubscription<List<int>> subscription;
  subscription = response.listen(
    (chunk) {},
    onError: (Object error, StackTrace stackTrace) {
      completer.completeError(error, stackTrace);
      subscription.cancel();
    },
    cancelOnError: true,
  );
  return completer.future;
}

void collectionAndPatternFixture(List<int> values, Object value) {
  for (final item in [...values]) {
    if (item == 1) values.add(2);
  }
  switch (value) {
    case int number when number > 0:
      values.add(number);
    default:
      break;
  }
}
