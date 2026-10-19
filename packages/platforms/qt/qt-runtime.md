# Qt 6.12 runtime distribution

Publishing requires an explicit Qt Framework 6.12.0 SDK and, if WebView is enabled,
an independent WebEngine 6.140.0 SDK built for that Framework. Development build,
run and hot reload may continue to use distribution Qt. Installing SDKs is an
explicit operation; project evaluation never downloads them.

The implementation has native-build and ELF/loader contract evidence. Full
Debian 13 GUI/lifecycle qualification of real distributable SDK inventories is
still required. Do not interpret the old Qt 6.8.2 receipts as Qt 6.12 acceptance.

## Distribution without an APT repository

The default is `DorotiQtWebEngineDeployment=bundled`. Ship one app `.deb` containing
Framework and enabled WebEngine, or a directory containing both. No Qt repository
registration, Qt SDK, first-launch installer or application-time download is needed.

To separate WebEngine, choose `download` **only with `packageformat=deb`**, and set
an explicit engine package ID and Debian version. The publisher receives two `.deb`
files in the same completed artifact directory. Without a repository, distribute
both files together:

```sh
sudo apt install ./my-qt-webengine_6.140.0-1_amd64.deb ./my-app_1.2.0-1_amd64.deb
```

The option name `download` retains the original packaging contract: the app has
an exact APT dependency on the engine. Local files satisfy that dependency without
an engine repository. Installing just the app file cannot fetch the engine unless
an already registered repository supplies it. `dpkg -i` does not fetch dependencies.
OS packages still need APT availability unless already installed/provided offline.
After installation, the app's ordinary execution requires no network access.

An existing signed HTTPS APT repository can optionally host the engine package.
Register its verified key in a dedicated keyring and use `Signed-By` in its source
entry before installing the app. Repository hosting, signing keys and registration
are publisher operations; Doroti does not invent URLs/keys or modify system sources.
Maintainer scripts never run APT or a downloader. APT owns signature/index/hash
verification and reports unavailable versions or failed downloads during install.

The engine package owns
`/opt/doroti/qt-webengine/<package-id>/<debian-version>/`. Its `engine.json` and
`X-Doroti-Qt-Framework-Version` control field record the Framework contract. It does
not depend on distribution Qt, replace system Qt or configure global loader paths.
The app requires the exact engine package version and verifies its independent
engine version and Framework compatibility before loading. Give incompatible
engine families distinct package IDs; use a new Debian revision for changed bytes.
An engine can be installed before its consumers and reused by multiple apps.
App removal never deletes shared engine files; APT dependency/autoremove policy
controls their lifecycle.

## Build settings

```xml
<PropertyGroup>
  <DorotiQtVersion>6.12.0</DorotiQtVersion>
  <DorotiQtSdkDirectory>/build/qt/framework</DorotiQtSdkDirectory>
  <DorotiQtWebEngine>true</DorotiQtWebEngine>
  <DorotiQtWebEngineVersion>6.140.0</DorotiQtWebEngineVersion>
  <DorotiQtWebEngineSdkDirectory>/build/qt/webengine</DorotiQtWebEngineSdkDirectory>
  <DorotiQtWebEngineDeployment>bundled</DorotiQtWebEngineDeployment>
  <DorotiBuildQtNative>true</DorotiBuildQtNative>
</PropertyGroup>
```

For a separate engine `.deb`, change deployment to `download` and add:

```xml
<DorotiQtWebEnginePackageId>my-qt-webengine</DorotiQtWebEnginePackageId>
<DorotiQtWebEnginePackageVersion>6.140.0-1</DorotiQtWebEnginePackageVersion>
```

For provider package builds, use `DorotiQtProviderVersion` (formerly
`DorotiQtVersion`) to select the provider NuGet version. `DorotiQtVersion` now
selects Framework 6.12.0 independently.

Use `-PackageFormat deb` through the CLI. Direct `dotnet publish` with that profile
is an internal packager operation, not a complete Debian packaging command.
WebEngine-disabled apps need no engine SDK or engine redistribution materials.
Single-file, GStreamer and non-Quick bundle publication currently fail explicitly;
development/comparison builds remain available. Qt remains replaceable on disk.

## Explicit SDK installation

`provider-build/install-qt-sdk.py` ships in the target NuGet package. From the
repository it is `packages/platforms/build/install-qt-sdk.py`:

```sh
python3 packages/platforms/build/install-qt-sdk.py \
  --lock packages/platforms/qt/sdk/qt-6.12.0-linux-x64.lock.json --output /build/qt/framework --cache /build/qt/cache
python3 packages/platforms/build/install-qt-sdk.py \
  --lock packages/platforms/qt/sdk/webengine-6.140.0-qt6.12-linux-x64.lock.json --output /build/qt/webengine --cache /build/qt/cache
```

The checked-in locks pin the official Framework/ICU/WebChannel/Positioning SDK
archives and independent WebEngine archive with SHA-256. The WebEngine lock
currently identifies the supplier's **6.140.0-rc** candidate, not an approved final
release. Preserve `releaseName` in its runtime review; CMake reports numeric
6.140.0 independently of that release label. They install build SDKs;
they do not constitute an approved redistribution inventory or corresponding-source
release. Source/notice material is supplied in the separate review step.
A lock describes exact HTTPS archives; no latest-version resolution occurs:

```json
{
  "schemaVersion": "doroti.qt-sdk-lock/v1",
  "architecture": "linux-x64",
  "kind": "framework",
  "version": "6.12.0",
  "frameworkVersion": "6.12.0",
  "payloadPrefix": ".",
  "archives": [
    {"url": "<exact HTTPS binary archive>", "sha256": "<64 lowercase hex digits>", "format": "7z"},
    {"url": "<exact corresponding source archive>", "sha256": "<64 lowercase hex digits>", "format": "file", "destination": "sources/qt-source.tar.xz"}
  ]
}
```

These placeholders are intentionally not a downloadable release lock. Obtain the
hashes from the chosen release and review its provenance. Framework archive
selection should include only required SDK modules; never use `aqt -m all` as a
runtime distribution policy. Separate WebEngine archives use their own source
and release metadata and can have `payloadPrefix: "."`. The installer validates
member paths, extracted links and SHA-256, stages privately, then commits the SDK
and an install receipt. Identical locked content is reusable; changed content at
the same output is rejected. An existing separately installed SDK is also usable.

## Automatic runtime inventory

`doroti-sdk.json` is optional for both directory and `.deb` builds. Without it,
the publisher discovers supported versioned Qt libraries, ICU 73, Quick/QML
imports, xcb/Wayland plugins and enabled WebEngine resources from the selected
SDKs. SDK build tools, headers, static libraries and unsupported modules are
excluded. Discovery does not modify the SDK or make network requests.

The generated runtime manifest and `provenance.json` record selected files and
hashes with `inventory: "discovered"` and `review: null`. If the SDK has an
installation receipt, its identity and selected file hashes are checked, and
the original release name (including RC labels) and archive provenance are kept.
Available SBOMs, build configurations, notices, licenses and sources are copied.
Missing redistribution documents do not block an ordinary build. Automatic
discovery does not claim that redistribution materials have been reviewed.
Native ABI/version checks, ELF/QML dependency checks, and generated bundle hash
verification still apply.

## Optional explicit redistribution inventory

To use a curated file list and validate complete redistribution materials,
provide `doroti-sdk.json` in each SDK prefix. An existing manifest is validated;
an invalid manifest fails instead of falling back to discovery. Build it from
a reviewed recipe with `provider-build/review-qt-runtime.py --sdk <prefix>
--recipe <recipe.json>`. The recipe contains:

| Field | Required content |
| --- | --- |
| `schemaVersion` | `doroti.qt-sdk/v1` |
| `kind`, `version`, `frameworkVersion`, `architecture` | `framework` or `webengine`, independent release, matching Framework, `linux-x64` |
| `review` | Who reviewed the actual selected binaries, build configuration and third-party terms |
| `archives` | Exact binary HTTPS URLs and SHA-256 hashes |
| `selections` | Explicit `{pattern, module, license?}` runtime file selections; hashes are materialized |
| `documents` | Relative `path` and `role`: `LGPL-3.0`, `GPL-3.0`, `copyright`, `third-party`, `replacement`, `build-instructions`, `sbom` |
| `sources` | Relative source archive `path`, binary archive hashes in `binaryArchives`, `patches` information and a shipped `buildInstructions` path |
| `buildConfiguration` | Actual configuration; WebEngine requires reviewed `ffmpegLicense: "LGPL"` |

All document/source files must exist under the SDK prefix. They are hashed and
shipped beside the runtime. Every binary archive needs corresponding source;
a generic Qt download page is insufficient. Preserve exact patches, required
third-party source, build/install information and original notices. Qt's published
SPDX SBOMs help locate the exact source commits and licenses; they do not replace
source and notice obligations. With an explicit inventory, missing or changed
materials fail publication.

Select real versioned shared objects once, for example
`{ "pattern": "lib/libQt6Core.so.6.12.0", "module": "Core" }`.
Qt runtime selections use `LGPL-3.0-only`; reviewed ICU 73 libraries use module
`ICU` and license `Unicode-DFS-2016`. The official Linux Qt 6.12 SDK needs ICU 73
in its private closure; Debian 13's system ICU 76 does not satisfy that SONAME.
Other external OS libraries are declared through ELF analysis and APT.
Qt build tools, static libraries, headers and unreviewed/GPL-only modules are
rejected. CMake also checks the linked Qt targets. The final runtime inventory
covers ELF dependencies, QML imports/plugins and dynamically selected plugins.
The runtime collector is shared by directory and Debian publication.

Include `QtQuick`/`QtQml` imports and their recursive dependencies, chosen
Quick Controls styles, xcb and native Wayland QPA, and required image, TLS,
Wayland client graphics/shell integrations. Qt 6.12's platform plugin is
`plugins/platforms/libqwayland.so`. Wayland **client** integration is distinct
from the GPL-only Wayland Compositor module. WebEngine additionally needs its
shared libraries, QtWebEngine QML/delegates, helper, resources and locales.
Do not place the Framework's common Qt modules in both SDK inventories.

## Loading and LGPL library replacement

The completed directory contains `qt/framework/6.12.0/`, optional
`qt/webengine/6.140.0/`, `qt.conf`, `doroti-qt-runtime.json`, shims and the app.
Each runtime prefix carries exact sources, documents and `provenance.json`.
Original source-payload hashes and rebased installed ELF hashes remain separate.

The managed loader runs before application configuration and the first shim
load. It preloads the selected private libraries by absolute path, sets Qt/QML/
plugin paths and helper/resources/locales, and retains library handles. Missing
engine packages and incompatible manifests fail before Qt initialization.
Wayland sessions select Wayland, X11 sessions select xcb; explicit
`QT_QPA_PLATFORM` is preserved. No code disables the WebEngine sandbox.

To test an ABI-compatible modified runtime, copy a complete runtime prefix,
replace its shared libraries and set `DOROTI_QT_FRAMEWORK_PATH` or
`DOROTI_QT_WEBENGINE_PATH` to that absolute prefix before starting the app. Retain
the manifest's relative files/SONAMEs, Qt ABI, and for separate WebEngine its
compatible `engine.json`. The loader deliberately does not enforce original
payload hashes/signatures at execution. Publication integrity checks and package
transport checks do not restrict LGPL replacement rights. Application terms
must permit the reverse engineering needed to debug LGPL modifications.

Qt LTS support policy and independent WebEngine/Chromium security updates must
be tracked separately. Only claim supported runtime combinations after the real
Debian 13 install, repeated native Wayland/xcb execution, shutdown, update,
remove/purge and user-data tests pass.
