# Validation sources and fixtures

Keep reusable validation code, contracts, input fixtures, and golden baselines here.
Do not store generated builds, captures, logs, reports, or result summaries here.

## Output locations

- .NET and MSBuild C++ builds: `Doroti/artifacts/validation/build/<fixture>/bin` and `obj`. Nested fixture paths stay distinct. `Directory.Build.props` imports the product defaults and redirects only validation projects. An explicit `ArtifactsPath` still takes precedence for .NET builds.
- Playwright captures, traces, and reports: `Doroti/artifacts/validation/web-playwright/<label>/`, where the label defaults to the renderer mode (`auto`). The path is resolved from the configuration file.
- MediaQuery reports: `Doroti/artifacts/validation/media-query-safe-area/`.
- Python bytecode from the timeout wrapper: `Doroti/artifacts/validation/python-cache/`.
- Other diagnostic runners may already use repository-root `.doroti/evidence` or `Doroti/artifacts`; pass an output directory outside this source tree when a runner requires one.
- Deliberately retained milestone records: repository `history/`. Golden reference inputs stay beside their fixtures; they are test inputs, not run output.

Run commands from the repository root with the required 20-minute timeout:

```powershell
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/json-codec -c Release
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/display-list-contract -c Release
```

For browser checks (with the product web server already running):

```powershell
Push-Location Doroti/validation/web-playwright
python ../run-with-timeout.py npm.cmd run check
python ../run-with-timeout.py npm.cmd test
Pop-Location
```

Use `npm` instead of `npm.cmd` outside Windows. `npm run show-report` opens the default `auto` report; pass another label's directory directly to `playwright show-report`.
Dependency installations such as `node_modules` and `.dart_tool` are tool-managed dependencies, not retained test results.
For CMake, use an out-of-source `-B Doroti/artifacts/validation/build/<fixture>` directory.
Run Flutter reference builds from a copied workspace under `Doroti/artifacts/validation`, so Flutter's generated files also stay outside this tree.

## Retained scope

See the [cleanup and archive record](../../history/2026-09-12/validation/README.md),
[MediaQuery evidence](../../history/2026-09-12/validation/evidence/media-query-safe-area/README.md),
and [official Graphite evidence](../../history/2026-09-12/validation/stock-graphite/2026-09-12-cutover/README.md).
Current Graphite support boundaries are documented in the [cutover support table](../docs/validation/official-graphite-cutover-2026-09-12.md).

The old `eng/validate*.ps1` aggregate entry points were removed previously; use the retained projects and runners directly. Historical fixture READMEs may describe those retired commands. Independent Windows spikes remain diagnostic sources; their old PASS/FAIL results do not qualify the current product. Automated checks do not establish physical-device, display, IME, or accessibility acceptance.
