# Compiler C3

C3 pins compatibility profile `flutter-aware-c3`, lowering rules `c3.0` and emitter `csharp-net10.4-partial`. Its package fixture contains a const app class, a nested basic-widget object graph, a method tear-off event callback and `main() { runApp(const App()); }`. Generated type declarations are partial-friendly so P1 can compose reviewed manual members without editing the generated base.

The converter lowers const constructors to immutable C# constructor/property shape within this selected slice; it does not claim Dart canonical const identity. It emits `GeneratedApplication.CreateRoot()` from the top-level entry and maps that generated method back to the original Dart `main` span. Generated code references only `Doroti.FlutterCompat`, builds in a content-addressed isolated project and needs no source patch.

The C3 gate compares clean/cache-off, incremental/cache-on and cache-hit bytes. It loads the generated assembly, runs the same pointer trace as a hand-written FlutterCompat Counter and compares state sequence, DisplayList byte counts and normalized input/frame/ACK trace. The committed output under `migration/generated/c3/` is evidence, not promoted hand-written runtime source.
