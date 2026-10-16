# Independent Doroti packages

`Doroti.Material/` and `Doroti.Cupertino/` exclusively own the design implementations. Core packages remain in `../Doroti/src/`. Material depends on Cupertino; Cupertino and core do not depend on Material.

Design versions live in each package's `Version.props` and are independent of the core version. Explicit `DorotiMaterialVersion` / `DorotiCupertinoVersion` properties select local release candidates; a global `Version` does not overwrite design identity. Supported dependency ranges are emitted into nuspec and verified by real NuGet-only consumers.

Run `python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/tests/design_package_contract.py` from the repository root. This checks the evaluated graph, real packages, four consumers, independent candidate versions and all three template selections. Its evidence does not establish native provider or device acceptance.

Platform providers own their host, target, bootstrap, native and tooling sources under [`platforms/`](platforms/). Their versions are independent of core and the design packages. Shared provider build policies live in `platforms/build/`; application manifests declare provider/profile/TFM/RID explicitly.

| Provider | Sources and guide |
| --- | --- |
| Windows App SDK | [`platforms/windowsappsdk/`](platforms/windowsappsdk/README.md) |
| MAUI / Apple | [`platforms/maui/`](platforms/maui/README.md) |
| Linux Qt | [`platforms/qt/`](platforms/qt/native/README.md) |
| Web | [`platforms/web/`](platforms/web/README.md) |

The shared Window service is implemented by each host with its own capability policy. Source/package checks and historical native results do not establish uniform device, physical-input or distribution acceptance. See the [framework guide](../Doroti/README.md#platform-support) and [maintained tests](../Doroti/tests/README.md).
