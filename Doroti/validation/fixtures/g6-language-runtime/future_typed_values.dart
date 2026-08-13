import 'dart:async';

Future<int> computeValue() async {
  final seed = await Future<int>.value(20);
  return Future<int>.value(seed).then((value) => value * 2);
}

Future<String> runFutureTypedValues() async {
  final value = await computeValue();
  final chained = await Future<int>.value(value).then((item) => item + 2);
  return '$value:$chained:${chained - value}';
}
