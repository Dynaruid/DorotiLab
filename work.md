# Doroti 종합 리뷰 후속 작업 결과

- 작성일: 2026-10-04 KST
- 검토 기준: `Doroti-full-review-61f0f23.md` (최초 검토 당시 원본; doctor 후속 검토 시 저장소 내 파일 없음)
- 보고서 기준: `61f0f23b8f112e8e277dc896cf0b2c6627aca878`
- 실행 checkout: `4bbcd6eb95d32c092054815f78f63dcf19323fcf` + 이번 변경
- 상태: **R0–R7 구현 및 자동화 검증 완료 / R8 선택 호스트 검증 / 제품 전체 PARTIAL**
- 실행 범위: 사용자 요청에 따라 F01–F10, A01–A08, D01–D07의 코드·회귀·문서·가능한 실제 호스트 검증 수행
- 상세 결과: [validation](Doroti/docs/validation/2026-10-04-full-review.md), [기계 판독 결과](Doroti/docs/validation/2026-10-04-full-review.json)

## 1. 검토 결과와 범위

보고서의 F01–F10과 8·9절의 추가 지적 8개를 소스, 호출 경로, 기존 테스트 진입점과 대조하고 수정했다. 아래 대응표는 수정 전 조건과 수정 지점을 보존한 것이다. 수정 후에는 실제 caller·worker·MSBuild·프로세스·픽셀 회귀를 실행했다. 최초 checkout을 별도로 빌드한 before 실행 결과는 주장하지 않는다.

원래 계획 작성의 문서 전용 범위는 사용자의 전체 작업 요청으로 구현 범위가 되었다. 제품 소스, 샘플, SDK, 검증 도구와 문서를 변경했다. 과거 보고서의 별도 권고를 추가 작업 허가로 확대하지 않았다.

이번 계획의 목표는 다음과 같다.

- Future 완료, callback 직렬화, 프레임 중간 microtask의 공통 계약을 먼저 바로잡는다.
- 최신 semantics, Web 탐색 정책, startup/종료/연결 수명을 실제 호출 경로의 작은 회귀 테스트로 고정한다.
- Scene snapshot을 실제 픽셀과 소유권이 있는 Image로 연결한다.
- 설치 복구, TypeScript 삭제 경로, Release receipt, 보조 프로세스와 개발 세션의 소유권을 정리한다.
- `doctor`가 선택한 runner·SDK·작업의 실제 필수 조건을 진단하고, 환경 준비와 빌드·장치 수락을 구분하도록 보강한다.
- 수정 후보의 자동화 결과와 실제 OS 입력·접근성·GPU·배포 수락을 별도 증거로 남긴다.

새 위젯·호스트 추가, 대규모 파일 재작성, renderer 기본값 전환은 포함하지 않는다. 플랫폼 중립 App/Runner 분리, view별 capability 및 dispatcher scope, exact-frame transaction, GPU consumer 완료에 따른 자원 회수, 기존 플랫폼 제약을 유지한다. 현재 존재하는 Windows MAUI·Apple·Qt 기능을 미구현으로 다시 분류하지 않는다. [support-status.md](Doroti/docs/support-status.md)에 기록된 CI workflow 삭제 상태도 유지하고, 로컬 검증 진입점을 보완한다.

## 2. 보고서와 현재 코드의 대응

추가 지적에는 추적용 ID A01–A08을 부여했다. 우선순위 P1/P2는 보고서 분류를 유지한다. 실행 순서는 데이터 삭제·정책 우회 위험과 작업 의존성을 고려해 별도로 정한다.

| ID | 소스 확인 결과와 적용 범위 | 주요 수정 지점 | 단계 |
| --- | --- | --- | --- |
| F01 / P1 | captured scheduler가 `TryPost` 결과를 지우고, callback이 실행되어야만 Future 완료. `scheduleTask` 동기 예외도 completer 오류 완료 누락 | [DartAsync.cs](Doroti/src/Doroti.Runtime/DartAsync.cs), [DorotiExecutionContext.cs](Doroti/src/Doroti.Runtime/DorotiExecutionContext.cs), [Scheduler binding.cs](Doroti/src/Doroti.Framework.Scheduler/binding.cs) | R1 |
| F02 / P1 | typed/untyped Future의 오류 복구·필터가 성공 callback의 captured dispatcher 정책을 우회 | [DartAsync.cs](Doroti/src/Doroti.Runtime/DartAsync.cs) | R1 |
| F03 / P1 | `DispatchFrame`은 begin과 draw를 연속 호출. 실제 drain은 외부 dispatch 종료 시점이며 warm-up도 별도 점검 필요 | [PlatformDispatcher.cs](Doroti/src/Doroti.Ui/PlatformDispatcher.cs), [Scheduler binding.cs](Doroti/src/Doroti.Framework.Scheduler/binding.cs) | R2 |
| F04 / P2 | texture 초기화 전 dispose가 `getGl()`을 호출할 수 있고, 정리 예외 뒤 terminal 전이·managed role 완료 누락. startup await 후 계속 진행 가능 | [doroti.raster.worker.ts](packages/platforms/web/Doroti.Host.Web/Web/doroti.raster.worker.ts), [doroti.web.texture-worker.ts](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.texture-worker.ts) | R4 |
| F05 / P2 | `Scene.toImage*`는 크기만 있는 Image를 생성. encode 및 Skia 재페인팅에 필요한 handle이 없음 | [GraphicsAndSemanticsContracts.cs](Doroti/src/Doroti.Ui/GraphicsAndSemanticsContracts.cs), [SkiaSceneRenderer.cs](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs), [layer.cs](Doroti/src/Doroti.Framework.Rendering/layer.cs), [snapshot_widget.cs](Doroti/src/Doroti.Framework.Widgets/snapshot_widget.cs) | R5 |
| F06 / P2 | Web `AllowedOrigins?.length` 검사로 `[]`가 제한 없음으로 처리. 공통 옵션은 빈 배열 허용, Windows는 설정 존재 여부로 검사 | [doroti.web.webview.ts](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.webview.ts), [WebViewContracts.cs](Doroti/src/Doroti.Ui/WebViewContracts.cs), [WindowsWebViewSession.cs](packages/platforms/shared/Windows/WindowsWebViewSession.cs) | R0 |
| F07 / P2 | applied A → pending B → 새 A에서 no-change 조기 반환이 B를 남김. Windows MAUI·Apple·Android non-Graphite bridge에 해당 | [MauiSemanticsBridge.cs](packages/platforms/maui/Doroti.Host.Maui/MauiSemanticsBridge.cs), [DorotiMauiSurface.cs](packages/platforms/maui/Doroti.Host.Maui/DorotiMauiSurface.cs) | R3 |
| F08 / P2 | `owned()`가 잔여 `current.new`를 거절해 install/remove 복구에 도달하지 못함. publish 교체 실패 정리는 staging에 한정 | [install-linux-qt.py](Doroti/eng/install-linux-qt.py) | R6 |
| F09 / P2 | 공유 startup은 receipt를 쓰지만 macOS 실행 환경에는 경로가 없고 판정도 로그 문구에 의존 | [release-candidate.py](Doroti/eng/release-candidate.py) | R6 |
| F10 / P2 | TypeScript Build/Clean이 대소문자 무시 비교를 사용하고 Clean 검증은 더 약함. Linux case sibling 및 경로 경계 위험 | [Doroti.TypeScript.targets](Doroti/src/Doroti.App.Sdk/Sdk/Doroti.TypeScript.targets) | R0 |
| A01 / P2 | detach·capability dispose·host Close 중 예외가 뒤의 정리를 건너뜀. disposed 상태 선확정으로 재시도도 어려움 | [DorotiHostSession.cs](Doroti/src/Doroti.Hosting/DorotiHostSession.cs), [Capabilities.cs](Doroti/src/Doroti.Ui/Capabilities.cs), [PlatformDispatcher.cs](Doroti/src/Doroti.Ui/PlatformDispatcher.cs) | R4 |
| A02 / P2 | Apple provider가 checked/toggled/mixed를 value/trait에 충분히 투영하지 않음. null 문자열 값을 쓰는 기본 위젯에 영향 | [MauiSemanticsLayout.Apple.cs](packages/platforms/maui/Doroti.Host.Maui/MauiSemanticsLayout.Apple.cs), [checkbox.cs](packages/Doroti.Material/src/checkbox.cs) | R3 |
| A03 / P2 | `Image.isCloneOf`가 view와 크기를 비교. Skia 내부에는 공유 storage가 있으나 이를 판정에 사용하지 않음 | [GraphicsAndSemanticsContracts.cs](Doroti/src/Doroti.Ui/GraphicsAndSemanticsContracts.cs), [SkiaSceneRenderer.cs](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs) | R5 |
| A04 / P2 | ProcessRunner가 stdout EOF 뒤 stderr를 읽어 자식의 stderr pipe 포화 시 교착 가능 | [Artifacts.cs](Doroti/tools/Doroti.Tooling/Artifacts.cs) | R6 |
| A05 / P2 | managed 연결 timeout은 accept만 제거. 늦은 port는 닫히지만 `StartAsync`/role lifetime을 끝낼 취소·ACK 없음 | [doroti.web.managed-worker.ts](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.managed-worker.ts), [BrowserManagedRenderThread.cs](packages/platforms/web/Doroti.Host.Web/BrowserManagedRenderThread.cs), [doroti.raster.worker.ts](packages/platforms/web/Doroti.Host.Web/Web/doroti.raster.worker.ts) | R4 |
| A06 / P2 | desktop canvas 한 축 부족 시 두 축을 모두 1.5배 확장. 보고서의 메모리 값은 정책 계산이며 실제 GPU 측정 아님 | [doroti.web.policy.ts](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.policy.ts) | R4 |
| A07 / P2 | 선택 플랫폼 Release 분기의 wrapper와 자식 candidate가 모두 `python` 별칭을 가정. validate wrapper는 별도 선택 로직 보유 | [doroti.ps1](Doroti/eng/doroti.ps1), [validate.ps1](Doroti/eng/validate.ps1) | R6 |
| A08 / P2 | Android Stop이 현 세션의 앱 소유권 확인 없이 package 단위 `force-stop` 수행. 기존 port 테스트는 이 경쟁을 검증하지 않음 | [android-development.py](Doroti/eng/android-development.py), [android_development_bridge.py](Doroti/tests/android_development_bridge.py) | R6 |

수정 전 대조에서 제외할 지적은 없었다. F04·A05·F08·A04·A08은 제어된 실패·경쟁 조건 회귀로 확인했다. A02의 공통 상태 투영과 native provider/VoiceOver 실제 수락은 분리한다. 정상 실행마다 문제가 발생한다거나 모든 호스트가 동일하게 실패한다는 주장으로 확대하지 않는다.

### 2.1. doctor 추가 검토 — 수정 전 근거

현재 [doroti.ps1](Doroti/eng/doroti.ps1)의 `Invoke-Doctor`(487–535행)는 `.NET --version` 실행 성공과 PowerShell 7 이상만으로 `success`를 정한다. workload/Xcode/macOS SDK 정보는 수집하지만 PASS 판정에 사용하지 않는다. [프레임워크 README](Doroti/README.md)는 이 명령을 공통 도구 확인으로 설명하므로 현재 PASS를 모든 플랫폼 빌드 가능의 증거로 해석하면 안 된다. 다만 `-App`/`-Platform`을 받는 진입점으로서 선택 대상의 환경 문제를 미리 설명하는 기능은 부족하다.

추가 검토는 기존 보고서의 결함 번호와 구분해 D01–D07로 추적한다. 아래는 수정 전 진단 공백이다. v4 구현 후 실제 CLI 회귀와 Windows/WSL 환경 진단 결과는 validation 문서에 기록했다.

| ID | 확인된 공백 | 소스 근거 및 필요한 보강 |
| --- | --- | --- |
| D01 | 선택 target이 PASS와 구조화된 report에 반영되지 않음. `-App`은 runner 경로 출력에 그치고 `-WindowsBackend Maui`도 doctor의 runner 선택에 쓰이지 않음 | `Invoke-Doctor`와 `Resolve-WorkspaceRunner`를 연결하고 app/platform/backend/RID/configuration 및 실제 runner를 진단 context에 포함 |
| D02 | doctor와 native doctor가 고정 `dotnet`을 현재 CWD에서 호출. `-DotnetPath`, app/runner의 `global.json`, 실제 SDK feature band·prerelease 정책 확인 없음 | iOS build는 runner 디렉터리에서 SDK를 선택하지만 doctor는 이를 따르지 않음. [root global.json](global.json), [iOS global.json](samples/DorotiTestbedApp/ios/global.json), dev의 `DotnetPath` 사용과 대조 필요 |
| D03 | 선택 작업의 workload·외부 도구·생성 도구 상태가 빠짐. 공통/app build/native build/dev/validation/release의 요구사항 차이 없음 | Windows MSVC/SDK, Android SDK/JDK, Apple Xcode/SDK, Qt profile, Python, 조건부 WGSL compiler 등. [README](Doroti/README.md), SDK targets와 실제 프로젝트에서 요구사항을 구함 |
| D04 | `native doctor` Android는 OS 구분 없이 `gradlew.bat`를 확인하고 JDK·wrapper 파일 존재에 의존. Apple은 `xcode-select -p` 성공만 검사 | 실제 Mac Android wrapper는 `gradlew`를 사용해야 함. wrapper JAR/config·Android API 34 bridge·selected Xcode의 full installation/SDK 등 검사 필요. 실제 build 성공 여부는 별도 |
| D05 | `Get-CommandResult` 및 Java version probe에 timeout·취소 없음. 멈춘 도구가 doctor 전체를 무기한 대기시킬 수 있음 | bounded probe와 동시 stdout/stderr 수집, child exit/timeout/취소 구분 및 자신이 시작한 프로세스 정리가 필요. A04/A07과 공통 실행·도구 선택 정책 연결 |
| D06 | v3 report는 요청 context·check별 상태·실행 경로·조치가 없고 Markdown은 workload/Xcode 결과도 생략. workspace/native 초기 오류는 report 작성 전에 종료 | 실패해도 최종 결과를 남기고 JSON/Markdown/console/exit 의미를 맞춤. AppKit package 등의 상수는 실제 프로젝트 설정에서 읽고 요구 버전 중복 관리 최소화 |
| D07 | 현재 `Doroti/tests`에 doctor/native doctor의 판정·SDK 선택·timeout·report 전용 회귀가 없음 | 기존 [runner_contract.py](Doroti/tests/runner_contract.py)는 validate/audit/release 전파를 검사. doctor의 실제 CLI를 fake tool·workspace로 호출하는 별도 회귀와 실제 OS spot check 추가 |

특히 요구 버전을 doctor에 다시 고정하면 문서와 코드가 갈라질 수 있다. Windows native SDK는 [Doroti.NativeVersions.props](packages/platforms/windowsappsdk/native/Doroti.NativeVersions.props)와 vcxproj의 v145, Qt는 [CMakeLists.txt](packages/platforms/qt/native/CMakeLists.txt)의 Widgets 6.5 / Quick 6.6 / WebEngineQuick 6.8 분기를 따른다. 프레임워크 README의 Qt 6.5 안내와 현재 Testbed Quick/WebEngine 6.8 요구를 profile별로 함께 정리한다. WGSL app build는 [Doroti.GpuEffects.targets](Doroti/src/Doroti.App.Sdk/Sdk/Doroti.GpuEffects.targets)의 준비된 compiler를 사용하며, Rust/Cargo는 그 compiler를 소스에서 만들 때만 요구한다.

## 3. 실행 순서와 공통 규칙

권장 순서: **R0 → R1 → R2 → R3 → R4 → R5 → R6 → R6-D → R7 → R8**.

보고서의 공통 비동기·프레임 우선 방향을 따르되 F06/F10은 앞에서 제한 의미와 삭제 경계를 고정한다. F10은 사용자 지정 경로의 데이터 손실 가능성이 있으므로 이후 빌드·Clean 회귀를 수행하기 전에 처리한다. R4는 R1의 취소·완료 계약을 사용하고, R5는 R4의 자원 수명 계약에 맞춘다. Release candidate 수락은 R6의 검증 도구 수정 뒤에 실행한다.

R6-D는 R6의 bounded 프로세스·Python 선택 정책과 공통 helper를 사용한다. 구현에 앞서 기존 doctor로 공통 도구 정보를 참고할 수 있지만 그 PASS를 target 환경 준비 완료로 사용하지 않는다. 보강된 doctor의 대상별 결과를 R7/R8 실행 전 환경 기록에 포함한다.

- 각 항목은 원인 확인 → 해당 실패 조건의 최소 회귀 테스트 → 수정 → 인접 계약 확인 → 증거·문서 갱신 순서로 처리한다.
- 시간·dispatcher·startup await·프로세스·파일 교체를 제어해 순서를 재현한다. 임의 sleep이나 수백 번 반복에 의존하지 않는다. 반복 검증은 통상 30회 이내로 한다.
- [.github/copilot-instructions.md](.github/copilot-instructions.md)에 따라 모든 테스트 실행의 외부 제한은 1,200초다. 작은 테스트 안에는 완료 누락을 빠르게 감지하는 제한 또는 가상 시간을 별도로 둔다.
- 기존 [Doroti.Tests](Doroti/tests/Doroti.Tests/Doroti.Tests.csproj), [web_worker_lifecycle.mts](Doroti/tests/web_worker_lifecycle.mts), [web_rendering.mts](Doroti/tests/web_rendering.mts), [web_textures.mts](Doroti/tests/web_textures.mts)를 우선 확장한다. 플랫폼 객체 때문에 결정적 테스트가 막히면 작은 공통 정책을 추출하되 실제 adapter/worker 연결도 검사한다.
- 소스 문자열 일치 검사만으로 동작 수정 완료를 판정하지 않는다. Python/MSBuild/프로세스 항목은 실제 엔트리포인트를 disposable consumer나 자식 프로세스로 호출하는 회귀를 둔다.
- 새 회귀는 기본 집계에 등록한다. native·물리 테스트는 해당 OS와 장치가 있는 별도 진입점으로 유지한다.
- 일회성 consumer·로그·캡처는 `temp/testing/full-review/<stage>/<run>/` 아래에 둔다. [LOCAL_STORAGE.md](Doroti/eng/LOCAL_STORAGE.md) 정책에 따라 `Doroti/artifacts` 원시 산출물은 보존 증거로 간주하지 않는다. 삭제 전 필요한 결과·해시·실행 범위를 추적 문서에 남긴다.

## 4. 단계별 작업

### R0. 탐색 정책과 TypeScript 삭제 경계 선행 수정 — F06/F10

- [x] **R0-1** `AllowedOrigins == null`은 제한 미설정, `[]`는 HTTP(S) 허용 origin 없음으로 문서화하고 Web/Windows/Apple/Android의 controller Navigate 조건을 맞춘다. HTML/app-content 정책과 iframe 내부 redirect 관찰 제약은 별도 계약으로 유지한다.
- [x] **R0-2** Web에서 null·빈 목록·정확히 일치·불일치, port/scheme 차이, 잘못된 URL을 검사한다. 거절 시 `InvalidRequest`가 반환되고 `src`, navigation/document generation, 요청 시작이 바뀌지 않아야 한다. 외부 사이트 접속 없이 loopback fixture로 검증한다.
- [x] **R0-3** TypeScript Compile/Clean의 `RemoveDir` 앞에 같은 경로 검증을 적용한다. 프로젝트 기준 절대 경로 정규화, separator 경계, 중간 루트와 동일 경로 거절, OS에 맞는 case 비교, symlink/reparse-point를 통한 외부 경로 우회 거절을 포함한다. 생성 전 존재하는 부모 경로도 검사한다.
- [x] **R0-4** 독립 MSBuild consumer에서 정상 자식 경로, Linux `obj`/`OBJ` sibling, 단순 prefix sibling, 루트 동일, traversal로 외부 탈출, 상대·절대 경로, symlink/junction을 Build와 Clean 양쪽으로 검사한다. 잘못된 경로의 sentinel과 주변 파일은 byte 단위로 유지되어야 한다. 실제 Windows와 Linux 결과를 분리한다.

완료 기준: controller 정책과 Build/Clean 삭제 경계가 공통 의미를 따르고, 삭제 전 검증 실패가 실제 MSBuild 실행에서 확인된다. Windows의 경로 문자열 fixture만으로 Linux 파일시스템 수락을 대신하지 않는다.

### R1. Future 완료와 오류 callback 직렬화 — F01/F02

- [x] **R1-1** `Action<Action>` scheduler capture를 acceptance·lifetime을 보존하는 typed dispatcher capture로 바꾼다. 기존 `DorotiCallbackDispatcher.PostAsync`를 재사용하거나 값 반환 경로를 보완해 중복된 TaskCompletionSource 로직을 줄인다. dispatcher 없는 일반 .NET 호출의 기존 동작은 유지한다.
- [x] **R1-2** 등록 시 거절, 원본 Task 완료 전 owner 취소, enqueue 뒤 실행 전 취소, callback 예외, post 자체 예외, 완료와 취소 경쟁을 다룬다. 모든 반환 Future가 성공·실패·취소 중 하나로 정확히 한 번 끝나고 취소된 owner의 callback은 실행하지 않아야 한다.
- [x] **R1-3** `_TaskEntry.run` 동기 예외를 completer의 `completeError`로 전달한다. 바깥 scheduler의 오류 보고를 유지하고 중복 보고·중복 완료를 방지한다. 오류 후 다음 scheduled task도 실행되어야 한다. Debug/Release 분기를 함께 검증한다.
- [x] **R1-4** typed/untyped `then`의 `onError`, `catchError`, `onError` alias 및 오류 필터를 등록 당시 captured owner queue로 전달한다. async dispatch가 필요한 필터는 C# catch filter 밖에서 평가한다. filter false, 필터/handler 자체 예외, 값/Future 반환과 원래 stack 전파를 보존한다.
- [x] **R1-5** background Task 실패 후 queue pump 전에는 filter/handler 실행이 없어야 한다. pump 시 owner·view·직렬화가 일치하고 입력/프레임 callback과 동시에 UI 상태를 바꾸지 않아야 한다. 두 dispatcher를 번갈아 pump하는 격리 테스트와 owner 종료 후 오류 도착을 포함한다.

완료 기준: 대기 Future의 terminal 누락이 없고 성공/오류 경로 모두 같은 소유자 정책을 따른다. 공통 Runtime/Scheduler 및 기존 widget·plugin 수명 회귀가 통과해야 한다.

### R2. begin-frame과 draw-frame 사이 microtask 계약 — F03

- [x] **R2-1** `DispatchFrame`의 begin callback들이 끝난 뒤 draw callback들 전에 같은 microtask 큐를 drain한다. dispatcher/view/environment/scene build scope와 요청 epoch·frame transaction을 유지한다.
- [x] **R2-2** `scheduleWarmUpFrame`의 view 있음/없음 경로도 같은 단계 계약을 따른다. 중첩 dispatch, drain 중 추가 enqueue, microtask 예외, begin/draw 예외, 닫힌 view에 대해 재진입 및 phase 복구 규칙을 명시한다.
- [x] **R2-3** transient A → microtask M → persistent B → post-frame C가 `A M B C` 순서여야 한다. M은 `midFrameMicrotasks`, B는 persistent, C는 post-frame을 관찰해야 한다. M의 상태 변경이 같은 프레임의 build에 반영되는지 확인한다.
- [x] **R2-4** 다중 view, 정상/forced/warm-up frame, resize transaction, 예외 뒤 다음 프레임, nested callback을 검사한다. native/replay/resize 직렬성 및 기존 프레임 admission 상한을 유지한다.

완료 기준: trace상의 phase와 실제 callback 순서가 일치하고 scene build token·view scope가 drain 전후 유지된다. CPU 회귀 PASS는 GPU present·scanout·FPS 증거와 구분한다.

### R3. 최신 semantics와 Apple 상태 전달 — F07/A02

- [x] **R3-1** MAUI `Update`에서 새 snapshot을 항상 최신 권위로 취급한다. applied와 같아도 이전 pending을 취소/대체하고 예약 generation을 무효화한다. `scrollEnd`/immediate 판단이 no-change 조기 반환에 막히지 않게 한다.
- [x] **R3-2** 제어 가능한 clock/dispatcher로 A 적용 → B geometry 대기 → 새 A 입력 → queue flush를 구성한다. 최종 native 상태는 A여야 하며 이전 callback과 action delegate가 다시 살아나지 않아야 한다. topology reversal, 더 오래된 generation, clear, owner dispose, 서로 다른 view를 포함한다.
- [x] **R3-3** AppKit과 UIKit에 checked on/off/mixed, toggled on/off, 문자열 value와 상태 동시 존재, disabled/obscured의 투영 규칙을 정한다. 각 native API의 의미에 맞추고 체크 상태와 선택 상태를 혼동하지 않는다. 문자열 상태가 필요한 곳은 접근성 로컬라이징 정책을 따른다.
- [ ] **R3-4** null value를 사용하는 실제 Material Checkbox/Switch의 provider 값을 off → on → mixed → off 순서로 검사한다. AppKit·Catalyst·iOS의 native property 확인과 실제 VoiceOver 발화·액션은 각각 결과를 남긴다.

완료 기준: A B A의 마지막 A가 유지되고 checked/toggled 상태가 실제 provider로 전달된다. F07은 Android non-Graphite fallback에 적용되며 Android Graphite bridge와 다른 독립 호스트의 동일 결함으로 일반화하지 않는다.

### R4. 종료·startup·연결 수명과 canvas 자원 상한 — F04/A01/A05/A06

- [x] **R4-1** Web startup과 dispose를 같은 session lifetime/generation으로 묶는다. 각 await 복귀 뒤 취소·종료를 검사하고, 늦게 생성된 자원도 해당 owner가 회수한다. textures 미초기화, GL context 미생성, surface/app 부분 생성에서도 종료를 수행할 수 있어야 한다.
- [x] **R4-2** cleanup 항목 하나의 예외가 후속 정리와 caller terminal 완료를 막지 않도록 구조화한다. dispose 중복은 같은 완료를 기다리거나 같은 결과를 반환하고 cleanup 오류를 보고한다. 성공 `disposed` ACK는 실제 정리 완료에만 사용한다. GPU wait timeout에는 자원을 조기 해제하지 않고 실패·보존 상태를 명시한다.
- [x] **R4-3** runtime create, export 조회, WebGPU initialize, surface initialize, app StartWorker, texture initialize 각각의 await를 멈추고 dispose/pagehide/failure를 주입한다. 해제 후 ready·새 입력·frame·timer가 생기지 않고, caller/managed role이 한 번 끝나며 생성 자원은 안전하게 회수되어야 한다. WebGL/WebGPU와 standalone/main-owned 경로를 각각 확인한다.
- [x] **R4-4** managed 연결에 accept/reject ACK와 session 취소를 추가한다. 120초 timeout·실패·supervisor 종료 뒤 늦은 port/role은 자신의 수명을 끝내고 `StartAsync`도 terminal에 도달해야 한다. runtime 소유 pthread Worker를 임의로 terminate하지 않는다. JS/C# 양쪽 취소·token 전달 및 protocol 호환성을 함께 수정한다.
- [x] **R4-5** fake clock으로 timeout 직전/직후 port, duplicate/foreign token, role 실패, 재연결, 재사용 pthread를 검사한다. helper만 검사하지 말고 실제 managed role entry와 supervisor/endpoint 연결을 포함하는 브라우저 회귀를 둔다.
- [x] **R4-6** HostSession의 각 DetachView와 entrypoint Shutdown, dispatcher의 각 view, view Close 뒤 capabilities, registry의 distinct disposable 전체를 끝까지 정리 시도한다. 기존 취소→framework unmount→host/consumer drain 순서를 유지한다. 오류는 원인을 보존해 모아 보고하고 반복 dispose는 중복 해제를 하지 않게 한다.
- [x] **R4-7** 첫 detach/Close/Dispose가 throw해도 남은 view·capability의 정리가 호출되는지 검사한다. 공유 capability는 한 번만 dispose하고, callback lifetime 취소, 이벤트 해제, pending Future의 종료, 살아 있는 다른 owner의 격리를 확인한다.
- [x] **R4-8** desktop canvas는 부족한 축만 독립적으로 확장한다. 초기·growth 경로 모두 DPR 적용 후 active device dimension 및 canvas backing byte 상한을 확인하고 overflow·과도한 headroom을 거절/축소한다. source texture admission과 composition/effect 상한을 섞어 완화하지 않는다. mobile shrink와 축소 후 확장 순서는 유지한다.
- [x] **R4-9** 폭만/높이만 증가, orientation swap, DPR 변경, dimension 상한 근처, desktop headroom과 mobile 안정화 shrink를 검사한다. `1000×1000 → 요청 폭 1001/1501/2251`에서 높이가 불필요하게 커지지 않아야 한다. 정책상 byte 계산과 실제 할당·장치 측정 결과를 분리한다.

완료 기준: 시작 중 종료와 늦은 연결에서도 caller의 무기한 대기가 없고 예외가 다른 owner의 정리를 막지 않는다. GPU 자원은 consumer 완료 또는 확인된 context loss 뒤에만 회수한다. canvas 개선은 회귀 조건의 상한 준수로 판정하며 측정하지 않은 FPS 개선을 주장하지 않는다.

### R5. Scene snapshot과 Image identity — F05/A03

- [x] **R5-1** `Scene.toImage`와 `toImageSync`용 scene rasterization capability를 정의하고 view의 renderer에 등록한다. 현재 `IImageHostCapability`는 decode 중심이므로 scene render와 sync 지원 여부를 명시적으로 설계한다. Ui 계층이 Skia를 직접 참조하지 않는 기존 의존 방향을 유지한다.
- [x] **R5-2** Skia CPU 경로부터 실제 scene command를 rasterize해 readback·encoding·재페인팅 가능한 owned Image를 반환한다. 크기/overflow/메모리 admission, 취소, scene/view disposed, owner mismatch를 생성 전 확인한다. renderer가 지원하지 않는 scene/native content 또는 sync 호출은 생성 시점에 capability 오류로 거절한다.
- [x] **R5-3** GPU/Web snapshot은 해당 renderer의 thread affinity와 완료 fence를 따른다. async capability만 지원하는 호스트에서 sync API를 blocking wait로 흉내 내지 않는다. 지원 조합과 미지원 조합을 명시하며 모든 host에 크기만 있는 Image를 반환하는 fallback은 제거한다.
- [x] **R5-4** Image와 host handle의 공유 storage identity를 clone chain에 전달하고 `isCloneOf`에 사용한다. 같은 view/크기의 별개 이미지는 false, 원본과 진짜 clone 및 clone의 clone은 true여야 한다. 참조 수 기반 release와 `toByteData` lease를 유지한다.
- [x] **R5-5** 알려진 색·alpha·clip·transform 장면의 RGBA 픽셀, PNG encode/decode, Image 재페인팅을 비교한다. RepaintBoundary/OffsetLayer, SnapshotWidget DPR·resize·cache invalidation, 지원하지 않는 PlatformView 조건을 실제 소비 경로로 검사한다.
- [x] **R5-6** 원본/clone dispose 순서, readback 중 원본 dispose, 예외·취소·view close, 지원하지 않는 renderer를 검사한다. backing 자원 최종 해제는 한 번이어야 하고 handle 없는 이미지가 성공 snapshot으로 반환되지 않아야 한다.

완료 기준: snapshot은 실제 픽셀·인코딩·재페인팅을 통과하거나 생성 시 명시적으로 거절된다. CPU 지원을 모든 GPU/native 조합의 지원으로 확대하지 않는다. host별 capability 지원 여부를 별도 기록한다.

### R6. 설치 복구·Release 검증·도구 및 개발 세션 소유권 — F08/F09/A04/A07/A08

- [x] **R6-1** Linux installer의 임시 링크/파일을 소유권 검증 후 복구한다. marker, 임시 링크 이름, root 안 `versions/<version>` 대상과 실제 경로를 검증하고 정확히 소유한 `current.new`만 예외로 처리한다. 임의 symlink, 외부 target, symlink root 거절을 유지한다.
- [x] **R6-2** run/desktop/current publish의 commit 지점, 이전 선택 유지 및 중간 실패 복구를 정의한다. owned temporary cleanup이 install 재시도와 remove 전에 도달하도록 하며 userdata를 보존한다. 임시 run/desktop 파일과 staging도 처리한다.
- [x] **R6-3** 실제 Linux disposable 설치에서 staging→version 승격과 각 publish `os.replace` 전후 실패를 주입한다. `current.new` 생성 후 첫 교체 실패를 필수로 포함한다. 재시도/제거, 기존 실행 선택, userdata sentinel, tamper 및 외부 symlink 거절을 검사한다. 기존 [installer_contract.py](Doroti/tests/installer_contract.py)는 Windows installer 테스트이므로 Linux 회귀를 별도로 추가한다.
- [x] **R6-4** macOS candidate에 run 전용 `DOROTI_RELEASE_RECEIPT` 절대 경로를 전달한다. Windows/macOS의 receipt 검증을 공유하고 필수 nativeFirstFrame/twoWindows/secondResizedAndClosed와 survivor 수를 검사한다. 과거 receipt를 재사용하지 않고 이번 실행·후보와 연결한다.
- [ ] **R6-5** ambient receipt 변수를 제거한 환경 구성 테스트, 누락/손상/stale receipt 및 native consumer 실패를 검사한다. Mac에서 실제 NuGet-only Release candidate를 실행해 receipt·exit·서명 범위를 기록한다. ad-hoc 서명은 notarization/clean-machine 배포 완료와 구분한다.
- [x] **R6-6** ProcessRunner의 stdout/stderr를 동시에 drain하고 timeout·CancellationToken·프로세스 트리 종료를 지원한다. 기존 결과/호출 API 호환성을 유지하며 exit, 출력 수집, timeout/취소의 실패 의미를 구분한다.
- [x] **R6-7** 자식이 stderr pipe 용량을 넘게 쓰고 stdout도 출력하는 실제 fixture, 양쪽 대량 출력, nonzero exit, timeout, 취소, 손자 프로세스를 검사한다. caller가 bounded terminal에 도달하고 소유한 프로세스만 정리되는지 확인한다.
- [x] **R6-8** Python interpreter를 한 번 해석해 절대 경로로 wrapper와 candidate에 전달한다. validate/release의 선택 정책을 공유하고 공백·한글 경로를 argument 배열로 전달한다. `python3`만 있는 Unix, `python`만 있는 Windows, 둘 다 없는 환경을 검사한다.
- [x] **R6-9** Android 개발 세션에 device serial + application ID 단위의 소유권을 둔다. 우선 동일 조합 중복 세션을 배포 전에 거절하는 정책을 적용하고, 정상 종료·실패·host crash 뒤 잠금 회수와 stale session 식별을 설계한다. Stop은 현 소유권과 runtime/session 식별이 맞을 때만 package 종료를 요청한다.
- [ ] **R6-10** A 시작 → B 동일 조합 시작 거절 → A Stop, 다른 기기/다른 package의 동시 세션, stale A의 Stop과 새 소유자 B, port 재사용·PID 변경을 검사한다. mock ADB 회귀 뒤 실제 장치에서 새 앱이 오래된 Stop에 종료되지 않는지 확인한다. 기존 Debug metadata delta·상태 유지·오류 복구·Restart/Stop 검증도 관련 수정 범위에서 다시 수행한다.

완료 기준: 설치 중간 실패가 재시도/제거를 막지 않고, Release 성공은 이번 후보의 receipt로 확인된다. 출력 pipe 교착과 오래된 개발 세션의 새 앱 종료를 막는다. Linux·macOS·Android의 실제 실행이 없으면 해당 수락은 미검증으로 남긴다.

### R6-D. 선택 대상과 작업에 맞는 doctor 보강 — D01–D07

**구현한 진단 계약**: 인자 없는 doctor는 공통 도구 점검을 유지하고 scope를 명시한다. `-App`/`-Platform`을 지정한 진단은 실제 runner와 작업 profile의 필수 조건을 판정한다. `-DoctorProfile common|build|dev|validation|release|compiler-development`를 지원한다. 기본값은 대상 미지정 시 common, 대상 지정 시 build다. `native doctor`는 native binding build의 조건을 같은 진단 모델로 확인한다. compiler-development는 WGSL의 pinned Rust/Cargo를 read-only로 검사한다.

- [x] **RD-1 / D01** workspace/runner 선택을 build/dev와 공유한다. WindowsBackend, Configuration, RID, CompilationMode, ValidationSuite, Apple framework override와 관련 Qt/Web profile을 context에 반영한다. 외부 app, root/app/runner CWD 및 inherited 설정을 다룬다. manifest 오류·backend 누락·지원하지 않는 RID도 구조화된 진단으로 남긴다.
- [x] **RD-2 / D02** 해당 명령이 실제 사용하는 dotnet 경로와 작업 디렉터리에서 SDK를 resolve한다. dev의 custom `DotnetPath`와 진단의 선택을 일치시키고 native/build/run/publish에 같은 경로 정책을 적용할 때에는 실제 호출과 함께 수정해 기존 호환성을 확인한다. `global.json`의 version/rollForward/allowPrerelease, 선택 SDK·base path·arch, 설치 SDK, 해당 SDK의 workload를 기록한다. iOS는 실제 runner 디렉터리에서 확인하고 모든 iOS app에 Testbed의 .NET 11 설정을 강제하지 않는다.
- [x] **RD-3 / D03** 실제 SDK targets·project properties·native build 설정을 근거로 요구사항 표를 구성한다. SDK/workload 누락과 외부 도구 누락을 각각 설명하고 같은 정보를 launcher/toolchain identity에서 재사용한다. restore 전 import/evaluation 실패는 필요한 정보가 없는 상태로 남긴다. doctor는 build/restore/workload install/download/device 실행을 자동 수행하지 않는다.
- [x] **RD-4 / D03** 아래 플랫폼별 필수/조건부 검사를 구현한다. 경로와 version만으로 GPU·실행·물리 입력 지원을 판정하지 않고 run/device 항목은 별도 미검증 또는 명시적 read-only probe로 표시한다.
- [x] **RD-5 / D03** profile별 도구 필요성을 구분한다. 일반 Web app build에 Node/npm/Bun을 필수로 요구하지 않는다. validation의 Node/Python 및 VSIX 개발 도구, Python을 쓰는 dev/release, 사용 중인 WGSL compiler·app resource·오프라인 cache는 해당 graph에만 검사한다. 준비된 WGSL compiler가 있으면 app build에 Cargo/Rust를 요구하지 않는다. compiler 개발 profile에서는 [rust-toolchain.toml](tools/Doroti.Wgsl/rust-toolchain.toml)을 기준으로 검사한다. optional Flutter reference checkout 부재만으로 일반 제품 doctor를 실패시키지 않는다.
- [x] **RD-6 / D04** Android native wrapper를 Windows `gradlew.bat` / Unix `gradlew`로 선택하고 실행 권한, wrapper JAR/config, JDK 선택 및 bridge compileSdk 설치 상태를 확인한다. `.NET JavaSdkDirectory`·JAVA_HOME·doctor의 선택이 다르면 경로와 이유를 보고한다. 자동 Gradle 다운로드 없이 로컬 상태를 검사한다. Apple은 선택 Xcode 경로와 `xcodebuild`/`xcrun`의 실제 SDK를 확인하고, simulator/device·signing 조건을 해당 profile로 분리한다.
- [x] **RD-7 / D05** 각 probe에 짧은 실행 제한과 취소를 적용하고 doctor 전체도 1,200초 상한 안에 종료한다. stdout/stderr를 동시에 수집하며 missing executable·nonzero exit·timeout·취소·파싱 실패를 구분한다. 실패한 probe 뒤에도 독립 검사를 계속해 한 번의 report에 원인을 모으고 자신이 시작한 프로세스만 정리한다.
- [x] **RD-8 / D06** `doroti.doctor/v4`로 context, timestamp, commit/입력 identity, `checks[]`의 ID·scope·required·expected·actual·tool path·command/cwd·exit·duration·상태·근거·해결 조치를 기록한다. 기존 v3의 주요 공통 필드와 report 경로를 필요한 호환 범위에서 유지한다. 원본 출력은 길이를 제한하고 인증 정보나 서명 비밀을 기록하지 않는다. JSON/Markdown/console 판정이 같아야 하며 workspace·native 오류도 최종 report를 남긴다. output path를 안내하고 최신 결과를 atomic하게 저장한다. 장기 증거는 R7의 추적 문서로 옮긴다.
- [x] **RD-9 / D01/D06** 필수 조건 누락은 FAIL/nonzero, optional 부재는 WARN 또는 notApplicable, 아직 확인 못한 필수 조건은 notVerified/PARTIAL/nonzero로 정한다. `-Platform all`은 host별 불가/미검증을 각 target에 표시하고 모든 대상 준비 완료로 집계하지 않는다. 명시한 전체 범위가 충족되지 않으면 전체 PASS와 exit 0을 내지 않는다. 공통 PASS, build prerequisites PASS, 실제 build/device acceptance는 서로 다른 scope로 표시한다.
- [x] **RD-10 / D07** 실제 doctor CLI를 disposable workspace와 fake dotnet/native 도구로 호출한다. 잘못된 SDK feature band·prerelease/patch rollForward, iOS CWD 선택, custom dotnet 경로, MAUI backend, workload 누락, tool version/경로, 두 stdout/stderr pipe, timeout·취소, native 실패, report/exit 일치, 공백·한글 경로를 검사한다. generic Web에 Node/Rust 불필요, device 미연결이 build를 실패시키지 않음, foreign-host/all 결과, import 미복원 상태도 포함한다. 단순 source 문자열 검사로 대신하지 않는다.
- [ ] **RD-11 / D07** Windows/실제 Mac/Linux에서 선택 profile의 환경 진단을 확인하고 이후 동일 context의 build/dev/validation/release 결과와 대조한다. [README.md](README.md), [README.ko.md](README.ko.md), [Doroti/README.md](Doroti/README.md), [Doroti/README.ko.md](Doroti/README.ko.md), Testbed/template 안내를 새 scope·exit 의미와 맞춘다. doctor 회귀를 Source/Developer에 등록하되 fake tool 테스트가 전 플랫폼 실제 준비 증거가 아님을 명시한다.

| 대상 | build/native 진단에서 확인할 조건 | dev/run/release에만 추가할 조건과 경계 |
| --- | --- | --- |
| Windows App SDK | 선택 Windows runner/RID, 실제 VS MSBuild·v145 toolset·pinned Windows SDK/C++ WinRT, restored native dependency 상태 | Vulkan/D3D12/DirectComposition·WebView2 및 OS 조건은 runtime scope. machine-wide Windows App Runtime을 무조건 필수로 요구하지 않음 |
| Windows MAUI | MAUI runner와 해당 workload/Windows SDK. App SDK의 C++ 조건은 실제 project graph에 있을 때만 요구 | 실제 native overlay/IME/UIA/GPU 실행은 별도 수락 |
| Web | 실제 browser-wasm SDK/선택 빌드 mode의 workload, TypeScript MSBuild 및 bootstrap/profile 설정 | Node는 validation/개발 도구에만 조건부 요구. HTTP COOP/COEP/CSP·SharedArrayBuffer·WebGL/WebGPU adapter는 서버/브라우저 실행에서 별도 검증 |
| Android | `.NET workload`, 선택 Android SDK Platform/Build Tools, bridge API 34, JDK 및 OS별 wrapper. 최소 버전은 실제 project/config에서 읽음 | dev/run은 ADB authorized device·ABI·선택 serial 및 Debug metadata profile 확인. build는 연결 장치 불필요. signing은 release에만 요구 |
| AppKit/Catalyst/iOS | 해당 SDK/global.json/workload, full selected Xcode와 target SDK, host arch/RID, binding project. TFM/CompilationMode와 Xcode 호환 조건 기록 | simulator runtime/UDID, device pairing/provisioning, Hot Reload interpreter/transport, NativeAOT 조건은 선택 작업에만 적용. Xcode version 검사를 우회한 구성을 지원 인증으로 표시하지 않음 |
| Linux Qt | 실제 native build 여부와 Quick/Widgets/WebEngine/GStreamer flags, 이에 맞는 Qt version/modules, CMake 3.24+, C++20, pkg-config/Wayland/scanner/Vulkan headers/fontconfig | display/QPA/active session·driver·WebEngine runtime 자원은 run 범위. software GPU 구분. 명시적 managed-only cross-build와 실제 Qt native build를 구분 |

완료 기준: 선택한 작업에 꼭 필요한 조건을 검사하고 해당하지 않는 도구 때문에 실패시키지 않는다. SDK·runner·dotnet/CWD가 실제 실행과 일치하며 모든 실패가 bounded report와 nonzero exit로 남는다. doctor PASS는 기록된 환경 조건의 PASS이고, 빌드·native smoke·실제 입력·GPU·배포 수락은 별도다.

### R7. 회귀 집계와 증거·지원 문서 정합성

- [x] **R7-1** [validate.py](Doroti/eng/validate.py)에 공통 C#/Node 회귀와 host 없는 Python/MSBuild/프로세스 회귀를 연결한다. OS 전용 fixture는 OS를 명시한 entry로 등록하고 실행하지 않은 항목을 PASS로 집계하지 않는다. 기존 Source/Build/Developer/Targets/Packages의 의미를 보존한다.
- [x] **R7-2** 실제 `dotnet new` 템플릿, 소스 기반 runner, ProjectReference 없는 독립 NuGet consumer에서 바뀐 Runtime/Ui/Skia/SDK 계약을 확인한다. private feed/cache와 새 candidate 버전을 사용하고 App/Runner 경계 진단도 유지한다.
- [x] **R7-3** [tests/README.md](Doroti/tests/README.md), [support-status.md](Doroti/docs/support-status.md), [web-host-architecture.md](Doroti/docs/web-host-architecture.md), [release-candidates.md](Doroti/docs/release-candidates.md), [development-hot-reload.md](Doroti/docs/development-hot-reload.md)를 변경 범위에 맞춰 갱신한다. README/README.ko의 지원·명령 설명에 영향이 있으면 두 문서를 함께 갱신한다.
- [x] **R7-4** 보고서 F/A ID와 후속 D ID별 before/after 회귀, 명령, 상태, 후보 identity를 새 validation 문서와 기계가 읽는 결과에 남긴다. 과거 결과는 날짜·후보 범위를 유지하고 이번 변경으로 덮어쓰지 않는다. 문서 링크, `git diff --check`, 변경 파일 범위를 확인한다.

완료 기준: 보고서의 18개 지적과 doctor 추가 검토 D01–D07 모두 수정·검증 상태와 잔여 host 수락을 추적할 수 있다. `Developer` 또는 `Release` 이름만으로 Packages·native smoke·물리 입력·서명 설치가 모두 통과했다고 보고하지 않는다.

### R8. 선택한 호스트·renderer 조합의 실제 수락

기능 수정의 범위는 보고서의 18개 지적과 R6-D의 doctor 보강이다. 실행 환경은 아래 조합부터 고정하며 장치·OS·SDK·renderer와 Debug/Release·JIT/AOT를 기록한 뒤 수락한다. 사용할 수 없는 환경은 이유와 함께 미실행으로 남기고 구현 완료와 제품 수락을 분리한다.

| 조합 | 필수 확인 | 결과의 경계 |
| --- | --- | --- |
| Windows App SDK / 현재 기본 Graphite Vulkan + D3D12 presentation | R1/R2/R4/R5, 실제 native 창 종료·추가 창·survivor, resize/DPI, image capture, loss | WinUI 혼합 opt-in·Ganesh는 별도 조건. native API 성공과 실제 표시·입력 구분 |
| Windows MAUI / 실행 때 고정한 renderer | R3 bridge, 종료 오류, native overlay 및 두 owner, 실제 IME·Narrator | disjoint NativeOverlay 계약 유지. App SDK PASS를 MAUI PASS로 환산하지 않음 |
| Web / Chromium / standalone single-thread WebGL 및 main-owned threaded WebGL·WebGPU | R0 탐색, R4 startup·timeout·dispose·reconnect, canvas 한 축 resize/DPR, R5 지원 capability | 서로 다른 build/runtime profile을 분리. Node helper PASS는 실제 .NET Worker boot PASS가 아님 |
| Android MAUI / Graphite 및 semantics non-Graphite fallback | A08 소유권, 개발 세션 회귀, fallback R3, lifecycle·TalkBack | Graphite 결과로 fallback을 대신하지 않음. Debug Hot Reload는 Release/AOT 수락과 별개 |
| AppKit / Catalyst / iOS | R3 native 값·VoiceOver, R4 종료, R5 지원 snapshot, AppKit R6 Release receipt | 각 host 별도. simulator·device, Debug·Release, ad-hoc·배포 서명 구분 |
| Linux / Qt Quick / 실행 세션의 QPA·renderer | F10 실제 case filesystem, F08 실제 symlink·설치 복구, tool subprocess, native smoke | xcb/XWayland/pure X11/Wayland와 software/physical GPU 구분. Qt Widgets는 별도 조합 |

- [ ] **R8-0** 선택 app/target/backend/RID/profile의 doctor report를 환경 증거로 남긴다. report context와 실제 이후 명령이 일치하는지 확인하고 필수 notVerified/FAIL을 해소한다. doctor만으로 아래 수락 항목을 완료 처리하지 않는다.
- [x] **R8-1** package-only consumer와 native smoke를 새 후보로 실행하고 candidate/package 해시가 같은지 확인한다.
- [ ] **R8-2** 실제 IME 조합·commit/cancel·selection·caret, Tab/focus, Narrator/VoiceOver/TalkBack/Orca의 상태·액션을 해당 장치에서 확인한다. 합성 입력과 provider 속성 확인은 별도 결과다.
- [ ] **R8-3** resize·DPR/monitor 이동·다중 창·창 종료 후 callback·device loss·두 unfinished frame·texture 해제를 확인한다. GPU submit/present receipt와 consumer fence 및 실제 scanout을 구분한다.
- [ ] **R8-4** 실행·재시작·재연결 후 약 1분 뒤 정상 동작하는지 확인하고, 종료가 정상 완료되는지 확인한다. displayed FPS를 주장할 때만 실제 표시 timestamp와 측정 도구 근거를 남긴다.
- [ ] **R8-5** 배포 대상이 정해진 조합에서 clean-machine 설치·업데이트 중단/복구·제거·userdata 유지 및 실제 서명/배포 정책을 수락한다. 테스트용 폴더 설치나 ad-hoc 서명만으로 완료 판정하지 않는다.

완료 기준: 선택 조합별 필수 항목의 실제 증거가 있고, 남은 조합·미지원 기능·환경 부재가 결과표에 보존된다. 소스/자동화 작업이 끝나도 물리·배포 수락이 남으면 제품 전체 상태는 `PARTIAL`이다.

## 5. 검증 명령과 진입점

아래는 구현한 재실행 진입점이다. 실제 실행 명령·상태·환경·해시는 validation 문서와 JSON에 기록했다. 저장소 루트에서 실행한다.

doctor 회귀도 외부 1,200초 wrapper로 실행한다. RD-10의 내부 timeout은 짧게 주입한다. `-DoctorProfile`은 구현한 옵션이다.

Android 예시의 `DEVICE_SERIAL`은 사용할 장치 serial로 바꾼다.

```powershell
# 선택 대상의 build 준비 / dev 준비를 각각 검사
python Doroti/eng/run-with-timeout.py --timeout 1200 pwsh -NoProfile -File Doroti/eng/doroti.ps1 doctor -App samples/DorotiTestbedApp -Platform windows -WindowsBackend WindowsAppSdk -DoctorProfile build
python Doroti/eng/run-with-timeout.py --timeout 1200 pwsh -NoProfile -File Doroti/eng/doroti.ps1 doctor -App samples/DorotiTestbedApp -Platform windows -WindowsBackend Maui -DoctorProfile build
python Doroti/eng/run-with-timeout.py --timeout 1200 pwsh -NoProfile -File Doroti/eng/doroti.ps1 doctor -App samples/DorotiTestbedApp -Platform android -Configuration Debug -Device 'DEVICE_SERIAL' -DoctorProfile dev
```

```powershell
# CPU 공통 회귀: 새 Runtime/Scheduler/수명/snapshot 회귀를 기본 실행에 연결
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Tests/Doroti.Tests.csproj -c Debug

# 기존 Web 회귀 확장
python Doroti/eng/run-with-timeout.py --timeout 1200 node --experimental-transform-types --test Doroti/tests/web_worker_lifecycle.mts
python Doroti/eng/run-with-timeout.py --timeout 1200 node --experimental-transform-types --test Doroti/tests/web_rendering.mts
python Doroti/eng/run-with-timeout.py --timeout 1200 node --experimental-transform-types --experimental-vm-modules --test Doroti/tests/web_textures.mts

# Android session 소유권의 host 없는 회귀
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/tests/android_development_bridge.py

# validate.ps1이 내부에서 1,200초 wrapper 적용
pwsh -NoProfile -File Doroti/eng/doroti.ps1 validate -ValidationSuite Source
pwsh -NoProfile -File Doroti/eng/doroti.ps1 validate -ValidationSuite Developer
pwsh -NoProfile -File Doroti/eng/doroti.ps1 validate -ValidationSuite Targets
pwsh -NoProfile -File Doroti/eng/doroti.ps1 validate -ValidationSuite Packages
```

Unix에서는 해석된 Python interpreter를 사용한다. `full_review_tools.py`·`doctor_contract.py`·Android 소유권 회귀는 Source/Developer에, C# 및 production Worker 회귀는 Developer에 등록했다. Linux 전용 fixture는 Windows에서 SKIPPED이며 이번 실행에서는 WSL ext4에서 별도로 통과했다. `WindowsSmoke`는 빌드된 App SDK runner를 사용한다. MAUI는 [windows_maui_smoke.py](Doroti/tests/windows_maui_smoke.py), Apple·Qt는 각 OS의 smoke를 사용한다.

R1 완료 시 영향을 받는 C# 회귀, R2 완료 시 CPU/프레임 회귀, R3/R4/R5 완료 시 관련 host/Node 회귀를 수행한다. R6 뒤 관련 OS 도구 회귀, R6-D 뒤 doctor 회귀를 수행하고 R7에서 Developer/Targets/Packages, R8에서 실제 조합을 수락한다. 앞 단계가 통과한 뒤 관련 없는 전 플랫폼 빌드를 반복하지 않는다.

## 6. 증거와 완료 판정

각 실행에는 최소한 다음 필드를 남긴다.

| 구분 | 필수 내용 |
| --- | --- |
| 소스·후보 | commit, dirty 변경 범위, candidate 버전, package/payload SHA-256, 해당 F/A/D ID |
| 환경 | OS/arch, SDK·workload·native 도구, host·renderer·runtime location/QPA, 장치, Debug/Release, JIT/AOT |
| 실행 | 명령·옵션, timeout, 시작/종료, exit code, 원본 로그·receipt·캡처 위치 |
| 판정 | 기대 결과, 실제 결과, 테스트별 상태, 자동화/합성/native API/물리/배포 구분, 미검증 사유 |

상태는 `PASS`(그 실행 범위에서 기대 충족), `FAIL`(실행 후 불충족), `SKIPPED`(명시 사유로 미실행), `notVerified`(필요한 증거 없음), `unsupported`(계약상 미지원), `notMeasured`(측정 안 함)를 구분한다. `PARTIAL`은 필수 범위 일부가 남은 집계 상태다. 과거 PASS는 이번 HEAD/후보의 결과로 사용하지 않는다.

### 최종 체크리스트

- [x] F01–F10 및 A01–A08 각각에 수정과 해당 실패 조건의 회귀가 연결되어 있다.
- [x] D01–D07에 대해 실제 runner/SDK/profile의 doctor 판정·timeout·report/exit와 CLI 회귀가 연결되어 있다.
- [x] 거절·취소·예외·timeout·중복 종료가 caller terminal을 누락하지 않는다.
- [x] success/error callback 소유자와 `A M B C` 프레임 순서가 실제 실행으로 확인된다.
- [x] empty allowlist, 최신 semantics, Apple 공통 toggle state 정책, 실제 snapshot 픽셀/identity 계약이 확인된다. Apple native provider 수락은 미검증이다.
- [x] startup/늦은 연결·cleanup 예외와 GPU 자원 수명, canvas 상한이 확인된다.
- [x] Linux 설치/MSBuild, receipt 환경·판정, pipe drain, Python 및 Android 소유권 자동화가 확인된다. 실제 macOS receipt와 Android 장치 Stop은 미검증이다.
- [x] 새 회귀가 로컬 집계에 들어가고 독립 템플릿/NuGet 소비자 및 문서 링크가 확인된다.
- [x] 실제 host/물리 입력·접근성·GPU·배포 결과와 남은 미검증 범위가 별도 기록된다.

### 이번 실행의 검증 범위

- 수행: 18개 F/A 및 7개 D 지적 수정, Source/Developer/Targets/Packages 집계, Debug/Release CPU 회귀, production Worker 실패 주입, 실제 Windows App SDK/MAUI 및 Chromium 실행, Linux ext4 설치/MSBuild 회귀, 새 private feed와 독립 NuGet-only Release 후보. 세부 호스트 결과와 최종 후보 identity는 validation 기록을 따른다.
- 미검증: Apple native build/provider/VoiceOver/macOS Release 실행, Android 실제 metadata delta·Restart/Stop·TalkBack(ADB 장치 없음), 물리 IME·접근성 발화·monitor 이동/scanout/FPS·실제 서명/clean-machine 설치. 이 범위는 자동화 PASS로 완료 처리하지 않는다.

체크된 항목은 그 항목의 구현과 기록된 자동화 범위의 완료다. 실제 Mac·Android·물리·배포 수락이 함께 요구된 항목은 아래 미완료 목록을 유지하며, 해당 환경 부재를 제품 FAIL로 바꾸거나 과거 PASS를 재사용하지 않는다.

### 남은 실제 수락과 부분 완료 항목

- R3-4/R6-5/R6-10/RD-11: 자동화·문서 구현은 완료. Apple host와 Android 장치가 없어 native provider/VoiceOver/macOS Release/Android metadata delta·Restart·Stop 실제 수락은 SKIPPED.
- R8-0: Windows/Web/Android build/공통/compiler doctor PASS. Linux app graph는 30초 probe timeout으로 PARTIAL이며 실제 Qt build/smoke PASS와 구분한다.
- R8-2/R8-3: native/synthetic 입력, resize/loss/다중 창/자원 회수 자동화 PASS. 물리 IME·발화·monitor 이동·scanout/FPS는 notVerified.
- R8-4: 세 Chrome 조합의 restart 후 60초 안정성 PASS. 모든 native host 조합의 1분 지속 운전과 실제 재연결 수락으로 확대하지 않는다.
- R8-5: Linux disposable 설치 복구 PASS. 실제 서명/배포 대상 clean-machine 설치·업데이트·제거는 notVerified.
