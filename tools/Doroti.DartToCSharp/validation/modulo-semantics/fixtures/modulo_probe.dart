class ModuloProbe {
  static const int constantModulo = -5 % 3;
  int constant() => constantModulo;
  int integer(int a, int b) => a % b;
  double floating(double a, double b) => a % b;
  double mixed(int a, double b) => a % b;
  num number(num a, num b) => a % b;
  dynamic dynamicNumber(dynamic a, dynamic b) => a % b;
  int compound(int a, int b) { a %= b; return a; }
  int compoundValue(int a, int b) => (a %= b) + 10;
  int nested(int a, int b, int c) => a % (b % c);
  int staticCompound() { staticValue = -5; ModuloProbe.staticValue %= 3; return staticValue; }
  static int staticValue = -5;
  int? nullableProperty(ModuloCell? target) => target?.value %= 3;
  int cascade() { var target = ModuloCell()..value %= 3; return target.stored; }
  int cascadeIndex() { var target = <int>[-5]..[0] %= 3; return target[0]; }
  int dynamicCustom() { dynamic value = ModuloValue(-5); return (value % 3).value; }
  int dynamicCustomCompound() { dynamic value = ModuloValue(-5); value %= 3; return value.value; }
  Future<int> asyncDivisor() async { return 3; }
  Future<int> asyncCompound() async { int value = -5; value %= await asyncDivisor(); return value; }

  int calls = 0;
  int indexCalls = 0;
  int rhsCalls = 0;
  List<int> values = [-5];
  List<int> target() { calls++; return values; }
  int index() { indexCalls++; return 0; }
  int rhs() { rhsCalls++; values[0] = 99; return 3; }
  int indexed() {
    calls = 0; indexCalls = 0; rhsCalls = 0; values = [-5];
    int result = (target()[index()] %= rhs());
    return result + calls * 10 + indexCalls * 100 + rhsCalls * 1000 + values[0] * 10000;
  }

  ModuloCell cell = ModuloCell();
  ModuloCell receiver() { calls++; return cell; }
  int property() {
    calls = 0; cell = ModuloCell();
    int result = (receiver().value %= 3);
    return result + calls * 10 + cell.reads * 100 + cell.writes * 1000;
  }
  int custom() => (ModuloValue(-5) % 3).value;
  int customCompound() {
    ModuloValue value = ModuloValue(-5);
    value %= 3;
    return value.value;
  }
}

class ModuloCell {
  int reads = 0;
  int writes = 0;
  int stored = -5;
  int get value { reads++; return stored; }
  set value(int value) { writes++; stored = value; }
}

class ModuloValue {
  final int value;
  ModuloValue(this.value);
  ModuloValue operator %(int divisor) => ModuloValue(value % divisor + 100);
}
