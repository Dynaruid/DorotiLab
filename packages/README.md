# Independent Doroti packages

`Doroti.Material/` and `Doroti.Cupertino/` exclusively own the design implementations. Core packages remain in `../Doroti/src/`. Material depends on Cupertino; Cupertino and core do not depend on Material.

Design versions live in each package's `Version.props` and are independent of the core version. Explicit `DorotiMaterialVersion` / `DorotiCupertinoVersion` properties select local package versions; a global `Version` does not overwrite design identity. Supported dependency ranges are declared in each package's NuGet metadata.

Platform providers own their host, target, bootstrap, native and tooling sources under [`platforms/`](platforms/). Their versions are independent of core and the design packages. Shared provider build policies live in `platforms/build/`; application manifests declare provider/profile/TFM/RID explicitly.

| Provider | Sources and guide |
| --- | --- |
| Windows App SDK | [`platforms/windowsappsdk/`](platforms/windowsappsdk/README.md) |
| MAUI / Apple | [`platforms/maui/`](platforms/maui/README.md) |
| Linux Qt | [`platforms/qt/`](platforms/qt/native/README.md) |
| Web | [`platforms/web/`](platforms/web/README.md) |

The shared Window service is implemented by each host with its own capability policy. See the [framework guide](../Doroti/README.md#platform-support).
