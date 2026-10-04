# Environment doctor

Run from the repository root, using PowerShell 7. `doctor` performs bounded,
read-only probes. It never builds, restores packages, installs workloads,
downloads Gradle, deploys an app, or launches a device.

```powershell
pwsh -NoProfile -File Doroti/eng/doroti.ps1 doctor
pwsh -NoProfile -File Doroti/eng/doroti.ps1 doctor -App samples/DorotiTestbedApp -Platform windows -WindowsBackend Maui -DoctorProfile build
pwsh -NoProfile -File Doroti/eng/doroti.ps1 doctor -App samples/DorotiTestbedApp -Platform android -DoctorProfile dev -Device DEVICE_SERIAL
pwsh -NoProfile -File Doroti/eng/doroti.ps1 native doctor -App samples/DorotiTestbedApp -Platform android
```

Profiles are `common`, `build`, `dev`, `validation`, `release`, and
`compiler-development`. Without a
target the default is common; with a target it is build. Dev defaults to Debug.
Validation honors `-ValidationSuite`. `-Platform all` reports every declared
target independently, including unsupported build hosts and unresolved checks.
For Windows, MAUI runner selection is shared with build/run/dev.

`-DotnetPath` is resolved to the same executable used by build/run/publish/native
and development launch identity. iOS resolves `global.json` from the selected
runner directory; other runners use the workspace directory. Reports include
SDK policy/version/info, installation inventory, working directory, evaluated
project properties, package versions and input hashes. An evaluation failure
is an unresolved prerequisite; doctor does not restore it automatically.

Windows App SDK checks VS MSBuild, the project's v145 toolset, pinned Windows
SDK/C++ WinRT and restored native packages. MAUI uses its selected workload and
runner. Web native/AOT builds require their SDK workload and MSBuild TypeScript
compiler; Node and Rust are not generic Web build requirements. App resource
and WGSL compiler checks come from the application graph. Android checks its
selected JDK/SDK/Build Tools and OS-specific Gradle wrapper; only dev requires
an authorized device and Debug metadata profile. Apple requires its supported
host, full selected Xcode and native SDK; provisioning/signing remains a
separate release gate. Linux checks the selected Qt modules and their CMake
profile minimum versions, compiler, scanner and pkg-config dependencies;
explicit prebuilt native profiles have a separate scope.

`-DoctorProfile compiler-development` checks the WGSL compiler's pinned channel
from `tools/Doroti.Wgsl/rust-toolchain.toml`, the installed rustup toolchain and
its Rust/Cargo executables in the compiler directory. It needs no app target and
does not install or download a missing toolchain.

The latest `doroti.doctor/v4` JSON and Markdown reports are written atomically
to `Doroti/artifacts/doctor/`. Use `-DoctorReportDirectory <path>` for a run-owned
copy. Each check records required status, expected/actual values, executable,
arguments, working directory, exit/reason/duration, bounded output and action.
`-DoctorProbeTimeoutSeconds` accepts 1–30 seconds (default 15). The complete
probe budget is 1,100 seconds within the test wrapper's 1,200-second limit.
`-DoctorCancellationFile <path>` allows an orchestrator to request cancellation
by creating that file; a canceled probe kills its own process tree and reports
the cancellation. Independent checks continue and cannot produce a false PASS.

Required failures yield FAIL/nonzero exit; unresolved required checks yield
PARTIAL/nonzero; optional absences yield WARN. PASS/exit 0 covers only the named
prerequisite scope. Runtime graphics, native smoke, physical input,
accessibility speech, signing and clean-machine installation require separate
evidence. Raw `artifacts` reports are disposable; preserve qualification in a
dated [validation record](validation/2026-10-04-full-review.md).
