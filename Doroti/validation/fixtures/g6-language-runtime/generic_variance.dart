class Animal {
  Animal(this.name);

  final String name;
}

class Dog extends Animal {
  Dog(super.name);
}

class Box<T> {
  Box(this.value);

  final T value;
}

String animalNames(List<Animal> values) =>
    values.map((value) => value.name).join(',');

String runGenericVariance() {
  final dogs = <Dog>[Dog('dot'), Dog('ori')];
  dynamic erased = Box<Dog>(Dog('runtime'));
  final Animal recovered = erased.value;
  final typeMatches = erased is Box<Dog> ? 'true' : 'false';
  return '${animalNames(dogs)}|${recovered.name}|$typeMatches';
}
