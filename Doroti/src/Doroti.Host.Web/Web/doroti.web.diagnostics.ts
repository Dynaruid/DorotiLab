export function pushBounded<T>(values: T[], value: T, capacity: number): void {
  values.push(value);
  if (values.length > capacity) values.splice(0, values.length - capacity);
}
