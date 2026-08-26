# C0 product cutover

C0 now uses `Doroti.Host.Desktop` and `Doroti.Framework` as the active product and package identities. The public C# namespace remains `Doroti.FlutterCompat` so generated Dart bindings do not need a namespace-only rewrite; it is no longer the package or assembly owner. Generated/adopted Material and Cupertino algorithms and their compatibility aliases therefore compile into one framework assembly.


Source, visual-golden and target-artifact SHA-256 equality is no longer an acceptance test. Closure audits retain immutable revision identity, source/license existence, resolved dependencies, symbol ownership, semantic target markers, support-matrix coverage and behavior/visual/runtime evidence. Render tests retain dimensions, visible output and perceptual/tolerance checks; the target verifier retains safe artifact paths and current-run file existence. The vendor audit likewise checks selection, provenance, dependency direction and forbidden references without treating edited source bytes as a failure. Hash fields in older artifacts are historical metadata only.
