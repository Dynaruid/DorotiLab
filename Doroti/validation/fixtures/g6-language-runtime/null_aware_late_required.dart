class LateHolder {
  late String fallback;
  String? candidate;

  String resolve({required bool upper}) {
    final selected = candidate?.trim().isEmpty == false
        ? candidate?.trim()
        : fallback;
    return upper ? selected!.toUpperCase() : selected!;
  }
}

String runNullAwareLateRequired({required bool upper}) {
  String Function(String)? callback = upper ? null : (value) => value;
  final holder = LateHolder()
    ..fallback = 'fallback'
    ..candidate = null;
  final first = callback?.call(holder.resolve(upper: upper));
  holder.candidate = ' doroti ';
  return '${first ?? holder.fallback}|${holder.resolve(upper: upper)}';
}
