// Exercise non-null factory forwarding and the inspector compatibility adapter.
class WarningFactoryBase {
  WarningFactoryBase();
  factory WarningFactoryBase.derived() => WarningFactoryDerived();
  int get value => 1;
}

class WarningFactoryDerived extends WarningFactoryBase {
  @override
  int get value => 73;
}
