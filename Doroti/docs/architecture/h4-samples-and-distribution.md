# H4 samples and distribution

> Historical evidence only. The comparison samples, `doroti-counter` template and H4 verifier described below have been removed; G7-3C owns their future C# template replacement.

H4 originally made `Doroti.Host.Avalonia` the default shell for runnable product entrypoints. `samples/AvaloniaHostCounter` preserves that host/frame fixture, while the current `DorotiDemoApp` uses the source-ported desktop shell and runs the F2 interactive 1,000-item fixed-extent sliver slice. Both projects remain non-packable and outside the lean `Doroti.Product.slnx` package build.

Generated packages remain host-neutral. Their checked-in C# and project files do not reference Avalonia or `Doroti.Host.Avalonia`, and the release package inspection continues to require their declared public Doroti/FlutterCompat dependency surface. The external application owns the platform shell and consumes those packages through public APIs.

`eng/verify-h4-distribution.ps1` packs an isolated local feed, installs `Doroti.Templates` into an isolated template hive, creates a project with no repository project references, and uses explicit OS-specific NuGet configurations and package caches. The same generated `Program.cs` and project source is restored, built, run as an Avalonia window, published as a self-contained single-file ReadyToRun application, and run again on Windows `win-x64` and Ubuntu 26.04 WSL2 `linux-x64` under Xvfb. The template is then uninstalled. Source hashes and reviewed target results are pinned in `migration/host/h4-distribution-evidence.json`; the full machine-local report is written to `artifacts/h4-distribution/distribution-report.json`.

The WSL/Xvfb run is real non-Windows restore/build/window-start/publish evidence for H4, but it is not physical Linux desktop evidence for DPI, input methods, clipboard, AT-SPI, GPU/software parity, or H1-H3 lifecycle gates. Those Linux capabilities and a disposable clean Windows VM install/run/uninstall remain explicitly `not-verified`.
