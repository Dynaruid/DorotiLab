# Independent design package migration

Replace PackageReference `Doroti.Framework.Material` with `Doroti.Material`, and `Doroti.Framework.Cupertino` with `Doroti.Cupertino`. Replace the corresponding `using` directives directly. Import `Doroti.Framework.Widgets` explicitly; C# imports are not re-exported.

The old projects have no active source or project file. There are no forwarding assemblies or product compatibility facades. Original C# source comments continue to describe the conversion baseline; source relocation does not mean latest upstream synchronization.

Material owns `MaterialColorSchemeRuntime`, `MaterialImageColorRuntime`, `MaterialImageQuantizerWu` and MaterialColorUtilities. Runtime has no Material color dependency. `IWidgetStateMapping<T>` and `WidgetStateMapping` are the reviewed public design extension contract; product design friend assemblies are removed.

Ui exposes an explicit, atomic shader descriptor registry and byte/hash/ABI validation. Call `MaterialShaderAssets.Register()` for direct InkSparkle loading. InkSparkle registers before its own use. Widgets and Rendering likewise register their own descriptors. An assembly owner, hash or ABI collision fails. Caller-owned descriptor arrays are frozen on registration; core never discovers Material through reflection.

Applications still supply icon fonts. Existing sample fonts and resource registration remain app-owned. Localization sources/delegates remain in their respective design package; no new locale coverage or physical font rendering is claimed.

`dotnet new doroti-app --design widgets|material|cupertino` defaults to widgets. Core/SDK baseline is `0.4.0-alpha.1`; design baseline is `1.0.0-alpha.1`. The SDK rejects removed direct identities and transitive old assemblies with DOROTIDESIGN001/002. Actual tested package combinations are recorded in `design-package-verification.json`.
