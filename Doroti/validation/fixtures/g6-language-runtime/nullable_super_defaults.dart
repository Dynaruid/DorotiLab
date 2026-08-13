class BaseOptions {
  BaseOptions(this.value, {this.label = 'base', this.enabled = true});

  final int? value;
  final String label;
  final bool enabled;
}

class DerivedOptions extends BaseOptions {
  DerivedOptions(super.value, {super.label = 'derived', super.enabled});
}

String runNullableSuperDefaults() {
  final defaults = DerivedOptions(null);
  final explicit = DerivedOptions(7, label: 'set', enabled: false);
  final defaultValue = defaults.value == null ? 'null' : '${defaults.value}';
  final defaultEnabled = defaults.enabled ? 'true' : 'false';
  final explicitEnabled = explicit.enabled ? 'true' : 'false';
  return '$defaultValue:${defaults.label}:$defaultEnabled|'
      '${explicit.value}:${explicit.label}:$explicitEnabled';
}
