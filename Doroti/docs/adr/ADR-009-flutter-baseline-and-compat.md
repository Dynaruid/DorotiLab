# ADR-009: Flutter baseline and FlutterCompat boundary

- Status: Accepted
- Date: 2026-08-01

## Decision

The initial compatibility baseline is Flutter Git revision `56b8e1a851a594b1a154f8ea93270807dab22b9a`, with Dart SDK range `>=3.11.0-0 <4.0.0`. `migration/flutter-compat/baseline.json` pins source-file hashes, selected symbols, support state and behavior fixture. `generate-flutter-api` derives `flutter-api.json` deterministically from only those inputs.

`Doroti.FlutterCompat` may depend on Core, Rendering and Widgets. It may not depend on Engine, backends, vendor assemblies, Dart VM or Flutter engine binaries. Doroti runtime behavior is implemented through Doroti-owned ports such as `IFrameDispatcher`.

## Consequences

Changing revision or source hash requires an API manifest diff and behavior review. Deferred symbols remain visible as deferred; the R3 skeleton does not claim full Flutter behavior compatibility.
