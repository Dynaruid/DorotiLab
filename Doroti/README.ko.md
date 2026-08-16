# Doroti runtime과 framework

[English](README.md) | **한국어**

Doroti는 MAUI Windows, MAUI Mac Catalyst, Blazor WebAssembly에서 공용 widget, layout, painting, semantics, rendering pipeline을 사용하는 C#/.NET UI framework입니다.

## 개발 방식

`src/Doroti.Framework.*`는 직접 유지보수하는 제품 source입니다. 공개 namespace는 project, assembly, package 이름과 같은 `Doroti.Framework.*`입니다. 기능 추가와 정확성 수정은 소유 framework/runtime/host project에서 직접 수행하고, 바뀐 공용 계약의 모든 consumer를 함께 고칩니다.

Dart-to-C# compiler와 고정 Flutter checkout은 선택적인 import·동작 reference 도구로 남습니다. 일반 build의 필수 조건이 아니며 제품 source를 덮어쓰지 않습니다. Compiler candidate는 명시적으로 검토·채택하기 전까지 격리 workspace나 `migration/`에만 둡니다.

소유권 결정은 [ADR-019](docs/adr/ADR-019-product-framework-source-ownership.md), 현재 우선순위는 root [작업 목록](../work.md)을 참고하세요.

## 현재 제품 경계

- `Doroti.Framework.*`: 제품이 소유하는 Foundation, Scheduler, Services, Physics, Animation, Gestures, Painting, Semantics, Rendering, Widgets, Cupertino, Material library
- `Doroti.Runtime`, `Doroti.Ui`, `Doroti.Hosting`: Dart/Flutter 호환 runtime 의미와 application bootstrap
- `Doroti.Engine`, `Doroti.Rendering`, `Doroti.Graphics`: frame scheduling, display output, graphics 계약
- `Doroti.App.Sdk`: Windows, Mac Catalyst, Web의 single-project target 선택
- `Doroti.Host.Maui`: 외부 소유 `SKGLView` GPU surface 통합
- `Doroti.Host.Web`: Blazor/WebGL2 canvas, input, accessibility, resource bridge

Windows Release build/publish와 실제 `MauiSKSwapChainPanel` GPU frame은 확인했습니다. Web package-only compile/publish도 확인했습니다. Native hover/wheel/keyboard/IME/UIA, Mac Catalyst native 실행, browser interaction 자동화, physical acceptance와 cross-target parity는 각각 별도의 `notVerified` gate입니다.

## 요구 사항

- [global.json](global.json)에 고정한 .NET SDK 10.0.300 또는 호환 patch
- PowerShell 7
- 선택 target에 맞는 MAUI/WebAssembly workload

`reference/flutter-master`와 `reference/Avalonia-main` checkout은 명시적인 reference 비교나 migration 작업에만 필요합니다. 해당 작업에서 Flutter가 필요하면 `pwsh -File ./Doroti/eng/prepare-flutter-sdk.ps1`로 준비합니다.

## 명령

Repository root에서 실행합니다.

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 doctor
pwsh -File ./Doroti/eng/doroti.ps1 build
pwsh -File ./Doroti/eng/doroti.ps1 validate
```

활성 명령은 다음과 같이 단순화했습니다.

| 명령 | 용도 |
| --- | --- |
| `doctor` | 필수 .NET/PowerShell 도구를 확인하고 선택적인 reference checkout 상태 보고 |
| `build` | `Doroti.Product.slnx` build |
| `validate` | Source 소유권, Release build, application target graph/build 검증 |
| `validate -ValidationSuite Release` | Windows GPU live와 외부 Web template/package publish 시나리오 추가 |
| `audit` | Repository-local storage와 현재 source 소유권 검사 |
| `migration-audit` | Compiler, upstream selection, provenance audit를 명시적으로 실행 |
| `release` | 통합 release suite, audit, pack, package 검사 |
| `clean` | Doroti build output, artifact, 임시 local state 제거 |

직접 suite 진입점은 [validate.ps1](eng/validate.ps1), [validate-app-targets.ps1](eng/validate-app-targets.ps1), [validate-web-product.ps1](eng/validate-web-product.ps1)입니다. 과거 G4-G7 validator는 더 이상 활성 명령이 아니며 결과는 repository root의 `history/`에 보존합니다.

## Source와 artifact 원칙

- 제품 framework 변경은 `src/Doroti.Framework.*`에서 수행하며 compiler 소유 `.g.cs`는 이 경로에서 compile하지 않습니다.
- 공용 동작은 가장 낮은 소유 framework/runtime/rendering/host 계약에서 고칩니다.
- Reference 비교, build, native live, browser live, physical, cross-target 결과를 구분합니다.
- `migration/`에는 provenance, 과거 selection, 검토한 import 입력과 committed evidence를 둡니다.
- `.doroti/`와 `artifacts/`에는 임시 tool·validation output을 둡니다.
- Repository JSON은 `System.Text.Json`을 사용합니다.

## 디렉터리 안내

| 경로 | 내용 |
| --- | --- |
| [`src/`](src/) | 제품 framework, runtime, renderer, host, target, SDK, analyzer |
| [`templates/`](templates/) | Single-project `doroti-app` template |
| [`eng/`](eng/) | 간소화한 build, validation, release, storage, 선택적 reference workflow |
| [`tools/`](tools/) | Source/provenance 및 진단 도구 |
| [`migration/`](migration/) | 과거 변환 입력, provenance와 evidence |
| [`docs/`](docs/) | 현재 ADR과 역사 architecture 기록 |

Doroti는 repository의 BSD 3-Clause license로 배포합니다. Upstream source와 package 표시는 [third-party notices](THIRD-PARTY-NOTICES.md)를 참고하세요.
