import 'boundary.dart';
class Base {
  int get value => 1;
  int calculate({int amount = 1}) => amount;
  int accept(covariant Object value) => 1;
}
class Derived extends Base {
  @override
  int get value => 42;
  @override
  int calculate({int amount = 1, int bias = 7}) => amount + bias;
  @override
  int accept(String value) => value.length;
}
abstract class AbstractDerived extends Base {
  @override
  int get value;
}
class Concrete extends AbstractDerived {
  @override
  int get value => 99;
}
class NullableBase {
  double? get elevation => 1;
}
class NullableDerived extends NullableBase {
  @override
  double get elevation => 2;
}
class GenericBase<T> {
  T transform(T value) => value;
}
class Middle<U> extends GenericBase<List<U>> {}
class Leaf extends Middle<int> {
  @override
  List<int> transform(List<int> value) => value;
}
class Description {
  @override
  String toString({bool detailed = false}) => detailed ? 'detail' : 'summary';
}
class ExtentBase {
  int get extent => 1;
}
class ExtentDerived extends ExtentBase {
  int _extent = 2;
  @override
  int get extent => _extent;
  set extent(int value) { _extent = value; }
}

mixin Metrics {
  int get metric => 3;
  int measure() => 4;
}
class MixedBase with Metrics {}
class MixedDerived extends MixedBase {
  @override
  int get metric => 30;
  @override
  int measure() => 40;
}
class MixedField extends MixedBase {
  @override
  final int metric = 50;
}
class RenamedBase {
  int size(covariant Object oldValue) => 0;
}
class RenamedDerived extends RenamedBase {
  @override
  int size(String newValue) => newValue.length;
}

class ExternalDerived extends ExternalBase {
  @override
  int calculate({int amount = 1, int bias = 7}) => amount + bias;
}

class LeafGeneric<V> extends Middle<V> {
  @override
  List<V> transform(List<V> value) => value;
}

class RuntimeColor extends Color {
  RuntimeColor() : super(0);
  Color resolveFrom(String context) => Color(77);
}
class PrivateDerived extends PrivateBase {
  int _value = 22;
  int readDerived() => _value;
}

class OrderBase {
  int run({int first = 1, int second = 2}) => first * 100 + second;
}
class OrderDerived extends OrderBase {
  @override
  int run({int second = 20, int first = 10}) => first * 100 + second;
}

abstract class GapPainter extends ExternalPainter {
  @override
  int paint(int area, {int gap = 5});
}
class ConcretePainter extends GapPainter {
  @override
  int paint(int area, {int gap = 5}) => area - gap;
}

class GenericChooser extends ExternalGeneric {
  @override
  T? choose<T>(T? value, {bool enabled = true}) => enabled ? value : null;
}
class GenericFieldBase<T> {
  T? get value => null;
}
class GenericField<V> extends GenericFieldBase<List<V>> {
  @override
  final List<V>? value = null;
}
