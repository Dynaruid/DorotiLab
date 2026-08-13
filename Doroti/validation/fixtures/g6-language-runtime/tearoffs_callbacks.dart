class Accumulator {
  Accumulator(this.seed);

  final int seed;

  int add(int value) => seed + value;
}

String runTearoffsCallbacks({int Function(int)? transform}) {
  final constructor = Accumulator.new;
  final instance = constructor(5);
  final int Function(int) method = instance.add;
  final selected = transform ?? method;
  var observed = 0;
  void Function(int)? report = (value) => observed = value;
  report.call(selected(7));
  return '${method(3)}:$observed';
}
