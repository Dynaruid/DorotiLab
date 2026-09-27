import 'dart:convert';
import 'modulo_probe.dart';

void main() {
  final probe = ModuloProbe();
  final pairs = <List<double>>[
    [-5, 3], [-5, -3], [5, -3], [-6, 3], [-0.0, 3],
    [5.5, 3], [-5.5, 3], [-5.5, -3],
    [5, 0], [5, -0.0], [double.infinity, 3], [-double.infinity, 3],
    [5, double.infinity], [-5, double.infinity], [-5, -double.infinity],
    [double.nan, 3], [3, double.nan], [-double.minPositive, 3],
  ];
  print(jsonEncode({
    'double': pairs.map((pair) => (pair[0] % pair[1]).toString()).toList(),
    'indexed': probe.indexed(), 'property': probe.property(),
    'custom': probe.custom(), 'customCompound': probe.customCompound(),
    'dynamicIntegerType': probe.dynamicNumber(-5, 3).runtimeType.toString(),
  }));
}
