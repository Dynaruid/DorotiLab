# Linux Qt publication

Build/run/dev can use distribution Qt. Publication uses a private Qt 6.12.0 SDK
runtime and optional independently versioned WebEngine 6.140.0. Set
`DorotiQtSdkDirectory` and, for WebView, `DorotiQtWebEngineSdkDirectory` to explicitly
installed SDK prefixes. Runtime files are discovered automatically;
`doroti-sdk.json` is optional for an explicit reviewed inventory. SDK installation
never happens during project evaluation.

The default `DorotiQtWebEngineDeployment=bundled` produces a directory or one app
`.deb` with the required runtime. `download` is available only through provider
`publish -PackageFormat deb`; set `DorotiQtWebEnginePackageId` and
`DorotiQtWebEnginePackageVersion` to generate a separate engine `.deb` and an exact
app dependency. No self-operated repository is necessary:

```sh
sudo apt install ./engine.deb ./app.deb
```

Without a supplied engine file or a preconfigured engine repository, app-only
installation cannot fetch that dependency. Normal execution never downloads Qt.
Use a stable application ID/package ID and preserve user XDG data on upgrades and
removal. Single-file Qt bundle publication currently fails explicitly.

See the repository's `packages/platforms/qt/qt-runtime.md` for SDK locks,
source/notice requirements, supported profiles and LGPL runtime replacement.
