# Doroti.DartToCSharp

**English** | [한국어](README.ko.md)

`Doroti.DartToCSharp` is Doroti's typed semantic compiler. It analyzes manifest-selected Dart and Flutter sources, lowers resolved language and framework semantics through versioned intermediate representations, and publishes deterministic, reviewable C# projects with source maps, diagnostics, and provenance.

It is not a general-purpose text-to-text Dart transpiler. The supported language surface is expanded through pinned fixtures and framework selections, and unsupported semantics must produce explicit diagnostics instead of silently changing behavior.

## Pipeline

```text
selection manifest
       │
       ▼
compiler-owned Dart analyzer
       │  resolved elements and source origins
       ▼
typed Dart IR ──► Core IR ──► structured C# IR
                                      │
                                      ▼
                           C# projects and packages
                         + reports, maps, provenance
```

Each logical input is analyzed once. Per-library analysis, lowering, and emission use bounded parallelism; final report aggregation and atomic publication remain deterministic and single-owner.

## Requirements

- .NET SDK 10.0.400 or a compatible latest patch
- Dart SDK compatible with the compiler-owned analyzer package
- The repository's pinned `reference/flutter-master` checkout for Flutter selections
- A complete DorotiLab checkout; the tool references shared runtime and tooling projects

Run commands from the repository root so selection and source paths resolve consistently. Prepare the optional Flutter checkout with `pwsh -File ./Doroti/eng/prepare-flutter-sdk.ps1` before using the maintained fixtures; even their graph-only tolerance input requires it. Supply an existing selection file with `--manifest`.

## Build and run

Build the compiler:

```powershell
dotnet build ./tools/Doroti.DartToCSharp/Doroti.DartToCSharp.csproj --nologo
```

Compile a selection to an explicit output directory:

```powershell
dotnet run --project ./tools/Doroti.DartToCSharp -- `
  --manifest ./selection.json `
  --output ./Doroti/artifacts/converter
```

The command may also be entered on one line. The process exits with `0` on success, `2` when review is required, and `1` for invalid input or an unhandled failure.

## Selection compilation

Useful options include:

| Option | Purpose |
| --- | --- |
| `--manifest <file>` | Path to your selection manifest |
| `--output <directory>` | Publish directly to a compiler-owned output directory |
| `--workspace-root <directory>` | Publish to a content-addressed workspace instead of `--output` |
| `--cache-dir <directory>` | Reuse analyzer results from an explicit cache |
| `--parallelism <n>` / `-j <n>` | Set the common bounded parallelism |
| `--analyzer-workers <n>` | Override Dart analyzer worker count |
| `--lowering-parallelism <n>` | Override lowering/emission parallelism |
| `--telemetry <file>` | Write compiler telemetry outside generated output |
| `--dump-ir <directory>` | Write opt-in canonical compiler-stage snapshots |
| `--dump-stage <list>` | Select `analyzer`, `dart`, `core`, `csharp`, or `all` |

Example with parallel compilation and selected IR dumps:

```powershell
dotnet run --project ./tools/Doroti.DartToCSharp -- `
  --manifest ./selection.json `
  --output ./Doroti/artifacts/converter-g4-4 `
  --parallelism 8 `
  --dump-ir ./Doroti/artifacts/compiler-ir `
  --dump-stage analyzer,dart,core,csharp
```

IR dumps and telemetry must be outside compiler-owned generated workspaces. Normal generated projects and packages do not contain debug dumps.

## Port workspaces

A `doroti-port.json` manifest composes an immutable generated base with explicit replacements, partial extensions, adopted product code, or platform ports. By default the compiler publishes to `.doroti/workspaces/<workspace-id>` at the repository root.

```powershell
dotnet run --project ./tools/Doroti.DartToCSharp -- `
  compile --port ./path/to/doroti-port.json
```

The workspace contains:

- `generated-base/`: immutable generated source
- `manual-snapshot/`: hash-addressed snapshot of reviewed manual inputs
- `effective/`: composed project that must build before publication
- `port-state.json`, `provenance.json`, and `source-map.json`: ownership and traceability records

Port examples use `./path/to/doroti-port.json`, `CounterModel` and `YOUR_SOURCE_REVISION` as placeholders for your own reviewed port manifest, symbol and source revision. Edit the customization inputs declared by your manifest; never edit compiler-owned workspace trees directly.

Create a review-only adoption bundle without modifying product source:

```powershell
dotnet run --project ./tools/Doroti.DartToCSharp -- `
  adopt --port ./path/to/doroti-port.json `
  --symbol CounterModel `
  --output ./Doroti/artifacts/adoption-counter
```

Review an upstream revision change against a previous workspace:

```powershell
dotnet run --project ./tools/Doroti.DartToCSharp -- `
  rebase --port ./path/to/doroti-port.json `
  --source-revision YOUR_SOURCE_REVISION `
  --output ./Doroti/artifacts/rebase-c0
```

Rebase results distinguish `clean`, `manual-review`, `conflict`, `upstream-symbol-removed`, and `fixture-required` outcomes.

## Analyzer cache

Cache inspection and pruning are explicit operations:

```powershell
dotnet run --project ./tools/Doroti.DartToCSharp -- cache-status --cache-dir ./.doroti/cache
dotnet run --project ./tools/Doroti.DartToCSharp -- cache-prune --cache-dir ./.doroti/cache --max-bytes 2147483648 --max-age-days 30
```

## Source layout

| Path | Responsibility |
| --- | --- |
| [`analyzer/`](analyzer/) | Pinned Dart frontend, protocol extraction, stubs |
| [`src/Application/`](src/Application/) | Compilation plans, orchestration, port composition, adoption, and rebase |
| [`src/Frontend/Dart/`](src/Frontend/Dart/) | Analyzer process/cache client and protocol decoding |
| [`src/Dart/`](src/Dart/) | Typed Dart symbols, types, declarations, expressions, and statements |
| [`src/Core/`](src/Core/) | Target-neutral semantic IR and runtime intrinsics |
| [`src/Backend/CSharp/`](src/Backend/CSharp/) | C# lowering, structured syntax IR, and formatting-only printer |
| [`src/Publishing/`](src/Publishing/) | Atomic workspace and artifact publication |
| [`src/Configuration/`](src/Configuration/) | Versioned selections, profiles, port manifests, and policies |
| [`src/Diagnostics/`](src/Diagnostics/) | Stable `DOTCONV` diagnostic and telemetry contracts |
| [`src/Identity/`](src/Identity/) | Compiler identity and content-addressed fingerprints |
