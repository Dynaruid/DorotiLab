# G5-5 application compiler and resource/plugin boundary

> Historical bootstrap record. The milestone validator named below has been retired; compiler work is now an explicit migration workflow under ADR-019.

G5-5 extends the reviewed framework compiler into an application compiler. A Dart entry point now owns package/import discovery, conditional graph resolution, generated application project composition, resource manifests, and target plugin requirements without fixture-specific library lists.

## Application package graph

The selection manifest supplies the application package root, entry point, resource/plugin manifests, target RID, selected reviewed framework package, and host bootstrap package. The compiler resolves `package:` URIs through the application's `.dart_tool/package_config.json` and records regular imports plus every conditional-import candidate in `application-graph.json`.

Generation is graph-driven rather than fixture-name-driven. The resolver computes strongly connected components, compares each library's content and dependency identity with the previous published graph, and regenerates only changed SCCs and their reverse dependents. Unaffected generated files are copied from the verified previous output. A no-change incremental run must reuse every output and remain byte-identical to a clean run.

## Generated project boundary

`ApplicationProjectGraph` emits one application project and solution. Its only direct dependencies are:

- the selected reviewed `Doroti.Framework.*` package or project;
- `Doroti.Hosting`, which is the application bootstrap and host-capability seam.

Generated application source is audited for concrete platform, host, Avalonia, Skia, and vendor references. Target implementation remains behind capabilities rather than entering generated Dart application code.

## Resources and localization

The compiler converts the application resource manifest into an embedded `doroti-application-capabilities.json` manifest and embeds declared asset, font, and localization files with stable logical names. `DorotiApplicationBoundary` loads that manifest from the generated assembly and verifies every resource's declared byte length and SHA-256 digest before exposing it through `IApplicationResourceHostCapability`.

The host registers the application resource capability together with the platform-message capability. Assets, fonts, and locale payloads therefore cross one typed UI/host boundary and do not depend on checkout paths at runtime.

## Platform channel and RID plugin boundary

The Dart-facing API and codec stay in framework application code. The native side is a RID-specific package with a `doroti.plugin-abi/v1` capability manifest and an `IDorotiNativePluginHandler` implementation. At startup, the hosting boundary verifies plugin ID, channel, codec, ABI version, and target RID before dispatching a platform message.

An application plugin without a native package for the selected RID produces `DOTAPP005` and fails compilation. A missing handler, mismatched ABI, or unregistered channel also throws a typed capability failure at runtime; none of these cases can silently succeed.

## Retained automated completion evidence

The retired G5-5 milestone gate validated two Material applications, one Cupertino application, and one base Widgets application. For every application it performed clean and incremental generation, byte-identity comparison, direct-reference audit, and generated-project build. It additionally:

- changes one conditional-import implementation and proves regeneration is limited to that library and its dependent SCCs;
- restores and runs a consumer from an isolated local NuGet feed, including asset, font, localization, and `MethodChannel` plugin checks;
- packages a `win-x64` native plugin and verifies its embedded capability manifest;
- requires the unsupported-plugin fixture to fail with exactly one `DOTAPP005` diagnostic;
- rejects repository-private restore fallbacks and platform/vendor concrete references.

The aggregate result is `migration/flutter-framework/g5-5-evidence.json` using `doroti.g5-5-evidence/v1`.

Physical Windows plugin integration, physical font/asset rendering, and physical localization UI remain `notVerified`. They are intentionally deferred to the G5-8 `DorotiDemoApp` target-machine stage.
