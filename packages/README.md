# Independent Doroti packages

`Doroti.Material/` and `Doroti.Cupertino/` exclusively own the design implementations. Core packages remain in `../Doroti/src/`. Material depends on Cupertino; Cupertino and core do not depend on Material.

Design versions live in each package's `Version.props` and are independent of the core version. Explicit `DorotiMaterialVersion` / `DorotiCupertinoVersion` properties select local release candidates; a global `Version` does not overwrite design identity. Supported dependency ranges are emitted into nuspec and verified by real NuGet-only consumers.

Run `python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/tests/design_package_contract.py` from the repository root. This checks the evaluated graph, real packages, four consumers, independent candidate versions and all three template selections. Its evidence does not establish native provider or device acceptance.

Platform provider relocation and the new native Window service are still pending. Existing hosts remain under `Doroti/src` until their native/shared-tree gate is implemented and verified.
