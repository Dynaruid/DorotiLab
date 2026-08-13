class Seed {
  Seed._(this.value) : doubled = value * 2;

  factory Seed.named(int value) = Seed._;

  final int value;
  final int doubled;
}

mixin Stamp {
  late final int stamp = createStamp();

  int createStamp() => 9;
}

class MixedSeed extends Seed with Stamp {
  MixedSeed(int value) : marker = value + 1, super._(value);

  final int marker;
}

class OwnerShadow {
  OwnerShadow.root({required int owner}) : stored = owner {
    captured = owner;
  }

  final int stored;
  int captured = -1;
  int get owner => 99;
}

String runConstructorsInitializers() {
  final redirected = Seed.named(4);
  final mixed = MixedSeed(5);
  final shadow = OwnerShadow.root(owner: 7);
  return '${redirected.value}:${redirected.doubled}|'
      '${mixed.value}:${mixed.doubled}:${mixed.marker}:${mixed.stamp}|'
      '${shadow.stored}:${shadow.captured}';
}
