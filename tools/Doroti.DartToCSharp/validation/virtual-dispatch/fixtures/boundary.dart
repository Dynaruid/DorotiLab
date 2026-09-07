class ExternalBase {
  int calculate({int amount = 1}) => amount;
}
class Color {
  Color(int value);
}
class PrivateBase {
  int _value = 11;
  int readBase() => _value;
}
class ExternalPainter {
  int paint(int area) => area;
}
class ExternalGeneric {
  T? choose<T>(T? value) => value;
}
