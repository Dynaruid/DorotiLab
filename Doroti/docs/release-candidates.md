# Local release candidates

2026-09-29 follow-up: candidate `0.3.0-beta.rc.20260929045407` qualifies Windows,
Web and Android x64 package-only consumers. Chrome and the Android emulator also
showed actual presentation and Increment 0→1. See [results](../../history/26-10-03/works/results/2026-09-29-web-windows-android.md).

Create a selected candidate with a fresh package version and isolated restore cache:

```powershell
python Doroti/eng/run-with-timeout.py python Doroti/eng/release-candidate.py --targets windows web android --android-rid android-x64
# Or one platform through the CLI:
pwsh -File Doroti/eng/doroti.ps1 release -Platform windows
```

The script builds Release product dependencies, packs a local NuGet feed and the
template, installs the template into a private hive, and runs `dotnet new doroti-app`.
The generated app consumes only packages from that feed plus NuGet.org, with its
own empty NuGet/HTTP caches. Android supports selected `android-arm64` or `android-x64`
package-only publish with embedded assemblies and a development-signed APK. It publishes Windows self-contained JIT and Web
without trimming/AOT. The Windows consumer creates two native windows, waits for
presentation, resizes and closes both. Browser startup is a separate check; wasm
payload presence is not browser rendering evidence.

Outputs live in `Doroti/artifacts/release/<version>/`: packages, platform publish
directories, toolchain details and `candidate.json`. The manifest records HEAD,
a working-tree content hash, exact package/payload SHA-256 values, executed checks
and unverified deployment gates. Existing nonempty output is never overwritten.
These are local unsigned preview candidates, not signed production releases.
Disposable consumer sources, caches and logs are under `temp/testing/`; cleanup
failures are recorded explicitly instead of claiming removed evidence.

The last candidate recorded by the 2026-09-29 generation run was
`0.3.0-beta.rc.20260929010529`. The subsequent 00–10 follow-up changed shutdown,
restoration and installer behavior and ran local regressions without generating
a new Release candidate. The installer contract below describes the updated
source; it does not establish that the earlier candidate contains those changes.
Generate and validate a new candidate before using follow-up results as release
evidence. See [dated results](../../history/26-10-03/works/common/10-release-packaging.md).

Install/upgrade a candidate to a dedicated local preview directory:

```powershell
pwsh -File Doroti/eng/install-candidate.ps1 -CandidateRoot <candidate> -InstallRoot <dedicated-directory>
pwsh -File Doroti/eng/install-candidate.ps1 -InstallRoot <dedicated-directory> -Action Remove
```

The portable installer verifies every payload checksum, copies a new version into
an owned staging directory and verifies it before renaming it into `versions/`,
then atomically changes `current.json`. Copy failures clean up that transaction's
staging directory so the same version can be retried; the prior current version
stays selected. Existing version directories must have matching checksums and no
unlisted files. It preserves `userdata/`
through updates and removal. It refuses unowned directories, path traversal and
reparse points. Read `current.json` to find the executable directory. Web uses
`-Platform web` and requires a server providing the existing COOP/COEP headers.
This mechanism does not register Start Menu entries, protocol associations,
MSIX identities or Authenticode trust. Those remain installer/signing work.

Compatibility policy:

- Pin App SDK, Runner SDK, framework, host, target and plugins to the same
  candidate version/feed. A public package version is immutable; do not overwrite
  `0.3.0-beta` in a shared feed. The candidate script generates unique versions.
- Native RID/ABI metadata and bundled Web assets/fonts must match their target
  package. Both SDKs now include the loader's referenced texture declarations.
- Rebuild apps when upgrading this preview: framework singleton fields became
  dispatcher-scoped properties. Source access remains compatible; precompiled
  field references are not a binary compatibility promise.
- Windows full-renderer trimming/NativeAOT, Web AOT, store signing and clean-VM
  installation are not established by these commands. Android device Release
  Mono AOT evidence is recorded separately from package-only consumption.
- Checkpoint format v1 rejects incompatible versions and falls back safely.
  See [navigation](application-navigation.md) for schema and precedence.

Windows GUI failures write a best-effort local `last-crash.txt` under
`LocalAppData/Doroti/<application-assembly>/logs`; failures still propagate.
Issue reports should include candidate version/manifest, target/RID/renderer,
OS/device, build mode, reproduction steps, expected/actual result and the relevant
local crash log. Do not include credentials or private app contents.

Signing certificates, a clean deployment machine, production domain/service
configuration and release-store credentials were not supplied. Signing,
clean-machine installation, production updates and extended soak tests stay
`notVerified` in [10 results](../../history/26-10-03/works/common/10-release-packaging.md).


Windows app-specific protocol registration is optional and belongs to the installation:

```powershell
pwsh -File Doroti/eng/install-candidate.ps1 -InstallRoot C:/Apps/DorotiPreview -CandidateRoot <candidate> -Platform windows -ProtocolScheme my-doroti-app -ProtocolExecutable CandidateApp.Windows.exe
```

The app must configure the same `ApplicationNavigationOptions.ProtocolScheme`.
Only HKCU is modified; an existing scheme with another owner is rejected. Subsequent
updates reuse `protocol.json` and retarget the command to the new immutable version;
Remove unregisters the owned scheme and preserves userdata. Failed protocol setup
rolls back the current version pointer and prior registration. The OS-dispatch fixture
is `python Doroti/eng/run-with-timeout.py python Doroti/tests/protocol_install.py temp/testing/protocol/new-run`.
It does not qualify a production installer or GUI single-instance routing by itself.


## Linux Qt local candidate (2026-09-29)

`Doroti/artifacts/linux/0.3.0-beta.qt.20260929/` is a separate Linux candidate:
25 private-feed packages, isolated-cache template consumption and a 45-file
Release/JIT framework-dependent payload. Its `candidate.json` records package/
payload SHA-256, base revision and modified source hashes, SDK/Qt and actual
native startup/resize and portable install results. System Qt/.NET remain required;
optional WebEngine is disabled in this minimal package consumer. The source
Testbed separately qualifies Quick WebEngine recreation.

Use `python3 Doroti/eng/install-linux-qt.py install --candidate CANDIDATE --root INSTALL`
then `INSTALL/run`. `--protocol-scheme myapp` emits a desktop-entry artifact without
registering a global association. `remove --root INSTALL` preserves userdata.
Installation, update, unlisted-file rejection, native launch and userdata-preserving
removal pass locally. Trimming/single-file/NativeAOT are unsupported. This unsigned
VM-local result does not establish clean-OS or production release qualification.
[Linux details and remaining gates](../../history/26-10-03/works/results/2026-09-29-linux-qt.md).


## macOS AppKit candidate

```sh
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/eng/release-candidate.py --targets macos --macos-tfm net10.0-macos27.0
```

This builds an osx-arm64 feed and a template consumer with isolated NuGet/HTTP
caches. macOS and Android MAUI candidates use separate runs. The Xcode 27
profile keeps SDK validation enabled. macOS requires PublishTrimmed=true;
LinkMode=None expresses this candidate's non-trimmed CoreCLR scope. NativeAOT
and full trimming remain outside its qualification. The generated .pkg is
expanded and its .app is run to exercise actual native presentation, a second
window, resize and orderly close. An explicit success marker and ad-hoc app
signature verification are required. Candidate manifests record package/payload
hashes and generation-time source/toolchain provenance. This is local package
extraction, not Installer/Gatekeeper/Developer ID/notarization or clean OS
installation qualification. See [current AppKit results](../../history/26-10-03/works/results/2026-09-29-macos-appkit.md).

2026-09-29 UIKit/Catalyst: [implementation, smoke commands and remaining qualification](../../history/26-10-03/works/results/2026-09-29-ios-catalyst.md). iOS Simulator, device signing and Catalyst scenes have separate evidence; package-only publish does not imply clean signed distribution.

Apple candidate options: `--targets maccatalyst` and `--targets ios` each build an
isolated target feed. Catalyst performs Release publish, local ad-hoc signing and
native frame qualification. iOS uses Release **simulator build/install/run**;
Apple's SDK rejects simulator `publish`, and signed device publish remains a
separate provisioning gate. These single-TFM local Host.Maui feeds must not be
merged or published as a universal multi-platform NuGet release. No packages are
pushed by this workflow. Candidate manifests retain the target TFM/RID and hashes.
