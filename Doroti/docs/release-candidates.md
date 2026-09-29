# Local release candidates

2026-09-29 follow-up: candidate `0.3.0-beta.rc.20260929045407` qualifies Windows,
Web and Android x64 package-only consumers. Chrome and the Android emulator also
showed actual presentation and Increment 0→1. See [results](../../works/results/2026-09-29-web-windows-android.md).

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
evidence. See [dated results](../../works/common/10-release-packaging.md).

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
`notVerified` in [10 results](../../works/common/10-release-packaging.md).


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
