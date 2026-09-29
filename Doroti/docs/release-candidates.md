# Local release candidates

Create a selected candidate with a fresh package version and isolated restore cache:

```powershell
python Doroti/eng/run-with-timeout.py python Doroti/eng/release-candidate.py --targets windows web
# Or one platform through the CLI:
pwsh -File Doroti/eng/doroti.ps1 release -Platform windows
```

The script builds Release product dependencies, packs a local NuGet feed and the
template, installs the template into a private hive, and runs `dotnet new doroti-app`.
The generated app consumes only packages from that feed plus NuGet.org, with its
own empty NuGet/HTTP caches. It publishes Windows self-contained JIT and Web
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

Install/upgrade a candidate to a dedicated local preview directory:

```powershell
pwsh -File Doroti/eng/install-candidate.ps1 -CandidateRoot <candidate> -InstallRoot <dedicated-directory>
pwsh -File Doroti/eng/install-candidate.ps1 -InstallRoot <dedicated-directory> -Action Remove
```

The portable installer verifies every payload checksum, copies a new version into
`versions/`, then atomically changes `current.json`. It preserves `userdata/`
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
