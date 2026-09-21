class NullSemanticsProbe {
  int requiredInt(int? value) => value!;
  bool requiredBool(bool? value) => value!;
  String requiredString(String? value) => value!;
  T requiredGeneric<T>(T? value) => value!;
  dynamic requiredDynamic(dynamic value) => value!;
  int defaulted({int value = 7}) => value;
  int Function() defaultBuilder({int value = 7}) => () => value;
  String Function() delayed(String? value) => () => value!;
}
