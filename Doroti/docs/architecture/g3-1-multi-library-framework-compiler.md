# G3-1 multi-library framework semantic compiler

G3-1 registers the general `framework-semantic` / `flutter-framework` profile. Every general-profile selection names a `frameworkMilestone`; the compiler identity pins `framework-g3.1-multilibrary-typed` lowering and the `csharp-net10.g3.1-project-graph` emitter. The F0 `flutter-framework-f0` profile remains a separate compatibility route and regenerates the original `objectRuntimeType` candidate byte-for-byte.

The analyzer still emits `doroti.dart-analyzer-output/v3`, but its resolved graph now carries import prefixes, defining and part fragments with one owner library, accessible extension element identities, class/field/constructor modifiers, initializing/super formals, and typed expression/initializer identities. A portable package configuration resolves `package:flutter:` without writing into the pinned Flutter checkout. `dart:ui` remains an explicit runtime-binding boundary; a graph-only fragment may record `DOTF0012`, but a symbol selected for emission cannot use this allowance.

`migration/selections/g3-1-framework-multilibrary.json` is a pinned upstream selection, not a handwritten compiler fixture. It mechanically emits `foundation/object.dart`, the generic constructors in `foundation/annotations.dart`, and `physics/tolerance.dart`. It also resolves the public/private `bitfield.dart` and `_bitfield_io.dart` cycle plus `material/animated_icons.dart` as graph-only inputs. The latter proves ownership of the defining unit and 16 real part files without claiming those Material symbols are generated.

`doroti.framework-project-graph/v1` contains the 13-public-root/695-file census, stable library and namespace identities, import prefixes, extension candidates, part ownership, Tarjan SCCs, project partitions, inter-project references, and the conditional SDK-project/NuGet runtime reference contract. A dependency cycle is valid only when every library in the SCC maps to one partition; otherwise `DOTF0011` is an error and `everyCycleMerged` is false. The selected bitfield cycle is merged into `Foundation`. Generated `Physics` references generated `Foundation`.

Unsupported semantic visitors do not emit obsolete placeholder declarations or `default` expression fallbacks. They produce `DOTF0001` with the pinned source, library, canonical symbol dependency, non-empty span, and manual action; the compiler CLI returns exit code 2. The committed negative selection uses the upstream `BitField` redirecting factory constructors.

The compiler validation runs with the repository 15-minute timeout and requires:

- current F0 output to match its committed candidate;
- two clean/cache runs of G3-1 to match each other and the committed project graph byte-for-byte;
- the 13 roots, 640 root exports, 695 Dart files, prefix, extension, part, private-scope, and SCC records to be present;
- generated Foundation and Physics projects to build with zero warnings/errors;
- an external consumer to construct the generated generic `Category`, execute default/custom `Tolerance`, and observe the upstream `objectRuntimeType` behavior;
- the upstream unsupported selection to return `DOTF0001` and process exit code 2.

At the G3-1 boundary the output remains an unreviewed `.g.cs` mechanical candidate. G3-2 subsequently reviews the bounded five-declaration selection and promotes it into product source without changing the still-unselected F1-F4 milestone completion counts.
