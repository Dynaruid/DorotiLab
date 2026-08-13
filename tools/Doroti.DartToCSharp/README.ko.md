# Doroti.DartToCSharp

[English](README.md) | **한국어**

`Doroti.DartToCSharp`는 Doroti의 typed semantic compiler입니다. Manifest가 선택한 Dart와 Flutter source를 분석하고, 해석된 언어·framework 의미를 versioned intermediate representation으로 lowering한 뒤 source map, diagnostic, provenance를 포함한 deterministic C# project를 생성합니다.

일반적인 text-to-text Dart transpiler는 아닙니다. 지원하는 언어 범위는 pinned fixture와 framework selection을 통해 확장하며, 지원하지 않는 의미는 동작을 조용히 바꾸는 대신 명시적인 diagnostic으로 보고해야 합니다.

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

각 logical input은 한 번만 분석합니다. Library별 analysis, lowering, emission은 제한된 병렬 실행을 사용하고, 최종 report 집계와 atomic publication은 deterministic한 단일 owner 단계로 유지합니다.

## 요구 사항

- .NET SDK 10.0.300 또는 호환되는 최신 patch
- Compiler-owned analyzer package와 호환되는 Dart SDK
- Flutter selection을 위한 repository의 pinned `flutter-master` checkout
- Shared runtime과 tooling project를 포함한 전체 DorotiLab checkout

Selection과 source path가 일관되게 해석되도록 repository root에서 명령을 실행합니다.

## Build와 실행

Compiler를 build합니다.

```powershell
dotnet build ./tools/Doroti.DartToCSharp/Doroti.DartToCSharp.csproj --nologo
```

Selection을 명시적인 output directory에 compile합니다.

```powershell
dotnet run --project ./tools/Doroti.DartToCSharp -- `
  --manifest ./Doroti/migration/selections/r1.json `
  --output ./Doroti/artifacts/converter
```

명령은 한 줄로 입력해도 됩니다. 성공 시 exit code `0`, review가 필요하면 `2`, 잘못된 input이나 처리하지 못한 실패에는 `1`을 반환합니다.

## Selection compilation

주요 option은 다음과 같습니다.

| Option | 역할 |
| --- | --- |
| `--manifest <file>` | Selection manifest. 기본값은 `migration/selections/r1.json` |
| `--output <directory>` | Compiler-owned output directory에 직접 publish |
| `--workspace-root <directory>` | `--output` 대신 content-addressed workspace에 publish |
| `--cache-dir <directory>` | 명시적인 cache의 analyzer result 재사용 |
| `--parallelism <n>` / `-j <n>` | 공통 bounded parallelism 지정 |
| `--analyzer-workers <n>` | Dart analyzer worker 수 별도 지정 |
| `--lowering-parallelism <n>` | Lowering/emission parallelism 별도 지정 |
| `--telemetry <file>` | Generated output 밖에 compiler telemetry 기록 |
| `--dump-ir <directory>` | 선택적으로 canonical compiler stage snapshot 기록 |
| `--dump-stage <list>` | `analyzer`, `dart`, `core`, `csharp`, `all` 중 stage 선택 |

병렬 compile과 일부 IR dump를 함께 실행하는 예시입니다.

```powershell
dotnet run --project ./tools/Doroti.DartToCSharp -- `
  --manifest ./Doroti/migration/selections/g4-4-physics-animation-gestures.json `
  --output ./Doroti/artifacts/converter-g4-4 `
  --parallelism 8 `
  --dump-ir ./Doroti/artifacts/compiler-ir `
  --dump-stage analyzer,dart,core,csharp
```

IR dump와 telemetry는 compiler-owned generated workspace 밖에 두어야 합니다. 일반 generated project와 package에는 debug dump가 포함되지 않습니다.

## Port workspace

`doroti-port.json` manifest는 immutable generated base에 명시적인 replacement, partial extension, adopted product code 또는 platform port를 합성합니다. 기본 output은 repository root의 `.doroti/workspaces/<workspace-id>`입니다.

```powershell
dotnet run --project ./tools/Doroti.DartToCSharp -- `
  compile --port ./Doroti/migration/ports/c0/doroti-port.json
```

Workspace에는 다음 항목이 들어 있습니다.

- `generated-base/`: 수정할 수 없는 generated source
- `manual-snapshot/`: 검토된 manual input의 hash-addressed snapshot
- `effective/`: publication 전에 반드시 build되어야 하는 합성 project
- `port-state.json`, `provenance.json`, `source-map.json`: ownership과 추적 정보

Customization input은 `Doroti/migration/ports/`에서 수정합니다. Compiler-owned workspace tree를 직접 수정하지 마세요.

제품 source를 수정하지 않고 review-only adoption bundle을 만듭니다.

```powershell
dotnet run --project ./tools/Doroti.DartToCSharp -- `
  adopt --port ./Doroti/migration/ports/c0-adoption/doroti-port.json `
  --symbol CounterModel `
  --output ./Doroti/artifacts/adoption-counter
```

이전 workspace를 기준으로 upstream revision 변경을 검토합니다.

```powershell
dotnet run --project ./tools/Doroti.DartToCSharp -- `
  rebase --port ./Doroti/migration/ports/c0/doroti-port.json `
  --source-revision c0-language-fixture/v2 `
  --output ./Doroti/artifacts/rebase-c0
```

Rebase result는 `clean`, `manual-review`, `conflict`, `upstream-symbol-removed`, `fixture-required` 상태를 구분합니다.

## Analyzer cache

Cache 확인과 정리는 명시적인 command로 실행합니다.

```powershell
dotnet run --project ./tools/Doroti.DartToCSharp -- cache-status --cache-dir ./.doroti/cache
dotnet run --project ./tools/Doroti.DartToCSharp -- cache-prune --cache-dir ./.doroti/cache --max-bytes 2147483648 --max-age-days 30
```

## Source 구성

| 경로 | 역할 |
| --- | --- |
| [`analyzer/`](analyzer/) | Pinned Dart frontend, protocol extraction, stub과 Dart test |
| [`src/Application/`](src/Application/) | Compilation plan, orchestration, port composition, adoption과 rebase |
| [`src/Frontend/Dart/`](src/Frontend/Dart/) | Analyzer process/cache client와 protocol decoding |
| [`src/Dart/`](src/Dart/) | Typed Dart symbol, type, declaration, expression과 statement |
| [`src/Core/`](src/Core/) | Target-neutral semantic IR과 runtime intrinsic |
| [`src/Backend/CSharp/`](src/Backend/CSharp/) | C# lowering, structured syntax IR과 formatting-only printer |
| [`src/Publishing/`](src/Publishing/) | Atomic workspace와 artifact publication |
| [`src/Configuration/`](src/Configuration/) | Versioned selection, profile, port manifest와 policy |
| [`src/Diagnostics/`](src/Diagnostics/) | 안정적인 `DOTCONV` diagnostic과 telemetry contract |
| [`src/Identity/`](src/Identity/) | Compiler identity와 content-addressed fingerprint |

## 검증

Dart frontend와 .NET compiler boundary를 검증합니다.

```powershell
Push-Location ./tools/Doroti.DartToCSharp/analyzer
dart pub get --enforce-lockfile
dart format --output=none --set-exit-if-changed .
dart analyze
dart test
Pop-Location

dotnet run --project ./Doroti/validation/Doroti.Validation.Compiler -- --refactor-only
```

Repository-level compiler suite는 다음 명령으로도 실행할 수 있습니다.

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 validate
```

상세 설계는 [typed framework compilation](../../Doroti/docs/architecture/f0-typed-framework-compiler.md), [multi-library compilation](../../Doroti/docs/architecture/g3-1-multi-library-framework-compiler.md), [port ownership](../../Doroti/docs/architecture/p0-port-ownership.md), [adoption/rebase state](../../Doroti/docs/architecture/p2-port-state-adoption-rebase.md)를 참고하세요.

Doroti는 repository의 [BSD 3-Clause license](../../LICENSE)로 배포됩니다. Upstream 고지는 [Doroti third-party notices](../../Doroti/THIRD-PARTY-NOTICES.md)에 기록되어 있습니다.
