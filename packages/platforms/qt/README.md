# Qt provider

Host and Target sources are owned by this independently versioned provider. Package identities are unchanged. Version: 0.4.1-beta; supported core: [0.4.0-alpha.1, 0.5.0). Native/bootstrap adoption and physical platform acceptance are tracked separately in the design-platform execution record.

The default host uses Qt Quick / `QQuickWindow` with Qt-owned Vulkan device and presentation. See the [native host guide](native/README.md) for Qt dependencies, ABI versions, optional WebEngine/GStreamer and the QWindow comparison backend.

## Debian packages

Linux `publish` accepts `-PackageFormat directory|deb`. Omission keeps the existing
directory publish. `-Output` is a publish-only destination, relative to the CLI's
working directory. No new execution platform is introduced.
Without `-Output`, directory publish uses the SDK's existing output; deb uses
`<application-workspace>/artifacts/linux-deb` as the artifact bundle root.

Build `.deb` in Debian 13 **amd64**, using the same environment for native Qt
compilation and packaging. Other distributions/RIDs are rejected before publish.
The initial profile requires Release/net10.0/linux-x64, Quick/Graphite, and a
self-contained, untrimmed JIT directory. The provider explicitly selects
`SelfContained=true`, `UseAppHost=true` and disables trimming/single-file/AOT/R2R
for this profile. An already enabled trimming/single-file/AOT/R2R option is rejected
before these properties are applied. GStreamer and the QWindow comparison backend
are outside this profile. WebView is optional. Publication bundles Qt 6.12.0 and independent WebEngine 6.140.0 when enabled; see the [runtime SDK and redistribution guide](qt-runtime.md).

Add metadata to the Linux runner project; values are evaluated by MSBuild, including
imports and conditions. `ApplicationId` must match its selected application manifest.

```xml
<PropertyGroup>
  <DorotiDebPackageId>com-example-myapp</DorotiDebPackageId>
  <DorotiDebMaintainer>Your name &lt;you@example.com&gt;</DorotiDebMaintainer>
  <DorotiDebDescription>My desktop application</DorotiDebDescription>
  <DorotiDebVersion>1.2.0-beta.1</DorotiDebVersion>
  <DorotiDebRevision>1</DorotiDebRevision>
  <DorotiDebExcludeLttngProvider>true</DorotiDebExcludeLttngProvider>
  <DorotiDebCopyright>../LICENSE</DorotiDebCopyright>
  <DorotiDebHomepage>https://example.com/myapp</DorotiDebHomepage>
  <DorotiDebCategories>Utility</DorotiDebCategories>
  <!-- Optional; declare only file types/schemes the app actually handles. -->
  <DorotiDebMimeTypes>application/x-example</DorotiDebMimeTypes>
  <DorotiDebUriSchemes>example-app</DorotiDebUriSchemes>
</PropertyGroup>
```

Required: explicit unique package ID, maintainer, description, copyright/license
file, evaluated `ApplicationId`, `ApplicationTitle`, `AssemblyName`, and square PNG
`DorotiLinuxIcon`. Standard icon targets publish `appicon.png`; packaging checks that
it matches the evaluated icon and installs it at its actual size. Package ID controls
dpkg ownership and `/usr/bin/<package-id>`; application ID controls desktop identity
and user storage. Keep both stable. IDs are never inferred by lowercasing or replacing
application ID characters. Categories default to `Utility`. Homepage, MIME types and
URI schemes are optional; no default association is changed. Sample maintainer
addresses are examples; replace them for your own distribution.

`DorotiDebVersion` falls back to evaluated `Version`. Versions must be
`X.Y.Z[-prerelease]`: `1.2.0-beta.1` becomes `1.2.0~beta.1-1` and `1.2.0` becomes
`1.2.0-1`. Revision defaults to positive integer `1`. SemVer `+build.metadata` is
rejected to prevent silent artifact collisions; assign a distinct version/revision.
The same version's output cannot be overwritten.

The official .NET 10 linux-x64 runtime pack contains an optional native LTTng
provider linked to legacy `liblttng-ust.so.0`, which Debian 13 does not supply.
Set `DorotiDebExcludeLttngProvider=true` explicitly to omit only
`libcoreclrtraceptprovider.so` from staging; omission is recorded alongside hashes
of both the original publish and installed payload. Managed assemblies remain
untrimmed. LTTng tracing is unavailable in that package. Without the explicit
setting an unresolved dependency fails packaging. See the
[upstream ABI issue](https://github.com/dotnet/runtime/issues/57784).

The build environment needs Python 3.11+, `dpkg-dev`, `binutils`,
`desktop-file-utils`, `lintian`, `patchelf`, .NET SDK and the selected Qt SDK/native build dependencies in the [host guide](native/README.md). Runtime files are discovered from the SDK; `doroti-sdk.json` is optional for an explicit reviewed inventory (see the runtime guide). Packaging tools are checked
only for `deb`; ordinary Linux build/run/dev/directory publish requires no dpkg tools.
When the SDK contains the GTK platform theme plugin, automatic discovery includes
it. Install its OS libraries in the Debian builder (`libgtk-3-0t64`); ELF analysis
adds those runtime dependencies to the generated package.

```powershell
./Doroti/eng/doroti.ps1 publish -App ./samples/DorotiCarouselApp -Platform linux -Configuration Release -PackageFormat deb -Output ./Doroti/artifacts/linux-deb
```

The provider wraps the existing selected dotnet executable/TFM/RID publish command
in its packaged Python helper. It publishes to a private temporary directory,
validates its file set/ELF64 architecture/runtime mode/native feature closure, copies
to staging, and never modifies a caller's existing publish. The payload goes under
`/opt/<package-id>`, with an argument-preserving launcher under `/usr/bin`, desktop
entry, hicolor icon and `/usr/share/doc/<package-id>` copyright/runtime notices.
Symlinks/special files are rejected; reviewed Qt runtime files are collected under the private qt/ prefix. Owners are recorded as
root without requiring root to build. No maintainer scripts or portable installer
calls are included; standard desktop/icon/MIME package triggers handle caches.

The launcher and managed loader select native Wayland in a Wayland session and
xcb in an X11 session, preserving explicit `QT_QPA_PLATFORM`. Vulkan uses
`libvulkan.so.1`. The previous system-Qt 6.8.2 xcb receipts do not qualify the
new Qt 6.12 distribution. Both backends, repeated shutdown and physical
GPU/menu activation require separate evidence.

`dpkg-shlibdeps` analyzes published ELF files using the private Qt/ICU closure.
Only actual external OS runtime dependencies are declared; distribution Qt/QML
packages are rejected. .NET dynamic system libraries, font and Vulkan requirements
remain explicit. A separate WebEngine package has an exact versioned app dependency.
Runtime Depends never includes compilers, CMake, `*-dev` or .NET SDK. GPU drivers,
Vulkan 1.2 and an actual working display remain separate requirements.

Successful artifacts appear together in
`<Output>/<package-id>_<debian-version>_amd64/`: the `.deb` of the same basename,
SHA-256 sidecar, `receipt.json`, `contents.txt`, `shlibdeps.log` and `lintian.txt`.
The receipt records evaluated metadata, SDK/OS/system library/Qt versions, enabled
features, dependencies and payload hashes. Cancellation/failure removes private
publish/staging files; only a completed artifact directory becomes visible.
Package generation, installation and GUI acceptance have separate receipt fields.

Lintian rejects unrecognized errors. Narrow, commented package overrides cover
the planned third-party `/opt/<package-id>` layout, Skia/.NET's upstream embedded
codec/compression libraries, Qt's JPEG plugin and WebEngine's embedded Chromium
libraries (restricted to known components and exact paths), and unstripped original native binaries whose bytes
are retained for provenance and diagnostics. They do not suppress dependency or
architecture errors. Generated changelog/no-Debian-bug and GUI launcher/no-manpage
warnings are retained in `lintian.txt` for review.

On a target Debian 13 system:

```sh
sudo apt install ./com-example-myapp_1.2.0-1_amd64.deb
com-example-myapp                          # or launch from the app menu
sudo apt install ./com-example-myapp_1.2.1-1_amd64.deb
sudo apt remove com-example-myapp
sudo apt purge com-example-myapp
```

Upgrade/remove/purge leave application-ID-scoped XDG user data intact. The launcher
does not set `XDG_DATA_HOME` to an installation path. Portable data migration is
not implemented. A `.deb` file does not configure an APT repository or automatic
updates. With separate WebEngine deployment, supply both local .deb files to APT; no engine repository is needed. The [runtime guide](qt-runtime.md) explains both workflows. Ubuntu/other distribution support still requires separate qualification.

Acceptance and remaining environment-specific gates are recorded in the
[Qt runtime distribution history](../../../history/26-10-19/qt-runtime-bundle-summary.md).
Only combinations with actual install/runtime evidence should be described as
qualified; package creation or container installation alone is insufficient.
