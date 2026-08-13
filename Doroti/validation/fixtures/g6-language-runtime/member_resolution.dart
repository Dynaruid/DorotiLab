class BaseLabel {
  String describe() => 'base';
}

class DerivedLabel extends BaseLabel {
  @override
  String describe() => '${super.describe()}+derived';
}

mixin PrefixLabel on BaseLabel {
  String prefixed() => 'mixin:${describe()}';
}

class MixedLabel extends DerivedLabel with PrefixLabel {}

class BaseTheme {
  const BaseTheme({this.icon});

  final String? icon;
}

class DefaultTheme extends BaseTheme {
  const DefaultTheme();

  @override
  String get icon => 'accent';
}

extension WrappedLabel on String {
  String wrapped() => '[$this]';
}

String runMemberResolution() {
  BaseLabel value = MixedLabel();
  BaseTheme theme = const DefaultTheme();
  return '${value.describe()}|${(value as MixedLabel).prefixed().wrapped()}|${theme.icon}';
}
