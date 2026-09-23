# Doroti.Runtime의 Dart 호환 계층 제거 및 C#/.NET 전환 계획

- 작성일: 2026-09-23
- 계획 작성 검토 기준: `277e4395`의 소스, `.github/copilot-instructions.md` (실행 기준선은 아래 `94d24bbd`)
- 상태 (2026-09-23 실행 중): **R0–R2 PARTIAL / R3 TODO / R4 PARTIAL / R5 TODO / R6–R7 PARTIAL / R8 TODO**
- 계획 작성 당시 수행 범위: 소스·의존성 검토와 이 문서 작성. 현재 구현 요청으로 제품 소스 변경을 진행 중이다.

### 2026-09-23 실행 기록

- 기준선 `94d24bbd`: Runtime·Material·import 도구 Release 빌드 경고/오류 0, 기존 계약 103개 통과. [타입·멤버 인벤토리와 영향도](Doroti/validation/runtime-dotnet/inventory.md), [원본 API 스냅샷](Doroti/validation/runtime-dotnet/api-baseline.txt)을 기록했다. R0의 Roslyn-bound 사용처와 최종 package API diff는 남아 있다.
- R1: `DorotiExecutionContext`/`DorotiCallbackDispatcher`에 host 시간 소스와 후속 작업 큐 캡처를 분리했다. dispatcher와 Web runner의 직접 시간 소스 사용을 전환하고, view 종료 시 캡처된 작업을 거절·취소하도록 했다. drain 중 재진입 순서를 수정했다. 실제 Web/Windows owner 실행은 `notVerified`다.
- Web 제품 브라우저 검증은 통과했지만, 선택 `TimerValidationExport`를 페이지 JS에서 직접 호출하는 시도는 그 호출 컨텍스트에 `doroti.web` 타이머 dispatcher가 준비되지 않아 계약 결과를 얻지 못했다. 계측 변경은 되돌렸고 전용 타이머·owner 계약은 `notVerified`로 남긴다.
- R2: 사용자 결정에 따라 Flutter API인 `Duration` 정의와 소비 계약을 유지한다. `Duration`↔`TimeSpan`의 microsecond/tick, 음수 절삭, 범위 초과 계약을 확인하고 초과 시 예외를 내도록 변환 경계를 보강했다. `StringBuffer`→`StringBuilder`, `Expando`→`ConditionalWeakTable`, `DartFile`→`FileInfo`, `Dart_mathLibrary`→`System.Math`, 제품 `DartRandom` 사용→`Random`을 전환했다. 파일 이미지 등 일부 public API는 변경되며 URI·encoding·그 밖의 adapter는 남아 있다.
- R4: scheduler의 Runtime priority queue를 BCL `PriorityQueue`로 교체하고 generic 작업의 타입을 보존했다. Widgets의 두 linked-list 소비자를 `LinkedList<T>`/node로 옮겼다. `Matrix4.storage`와 semantics child ID, Canvas raw 배열 계약을 .NET 배열/읽기 전용 목록으로 옮겼다. 숫자 typed-list의 제품 참조를 제거하고 `StandardMessageCodec`의 `int[]`/`long[]`/`float[]`/`double[]` tag·왕복 및 int32 little-endian fixture를 확인했다. null-key ordered map과 byte view/복사 기준선도 추가했다. `DartMap`·`Uint8List`/`ByteData`의 제품 타입 전환과 전체 codec 통합 검증은 남아 있다.
- `StringBuffer`, `Expando`, Runtime priority/linked-list 타입은 선택 import 도구가 아직 출력할 수 있어 R6 제거 기한의 임시 bridge로 Runtime에 유지한다. 제품 소비 코드는 BCL로 옮겼으며 최종 완료로 판정하지 않는다.
- R6: 선택 import 도구의 `File.path`/Random/math lowering을 일부 .NET 타입으로 옮겼고, [대표 fixture](tools/Doroti.DartToCSharp/validation/runtime-dotnet/validate.ps1)는 Dart 분석 0 diagnostics와 생성 C# 빌드를 통과했다. 다른 File 메서드·출력 경로와 제품 잔여 helper 처리는 남아 있다.
- R7: 현재 변경의 [C# API 이전표](Doroti/validation/runtime-dotnet/api-changes.md)를 작성했다. 이 문서는 진행 중인 변경만 다루며 최종 migration guide는 R3–R6 후 갱신해야 한다.
- 현재 검증: Runtime, Material, Widgets, Web Testbed, Windows App SDK Testbed, `DorotiTestbedApp`, import 도구 빌드와 기존 계약 103개, 새 managed 계약 54개가 통과했다. 대표 compiler fixture 출력 C#도 빌드했다. 최신 배열 변경 후 Chrome CDP Web 제품 검증에서 기본 WebGPU, 체크박스·스위치, 날짜 선택 취소, 텍스트 입력, 탭 재마운트, 페이지 예외 없음이 통과했다. Runtime 로컬 NuGet pack을 새 캐시에 복원하여 패키지 DLL과 현재 빌드 DLL의 SHA-256 일치를 확인했고 저장소 밖 C# 소비 프로젝트도 실행했다. 다른 compiler 출력, Windows/Android/iOS/macOS/Linux 실행, Safari·실기기 Web, 전체 package/template 소비, AOT·성능은 아직 `notVerified`다.

## 1. 목표와 범위

`Doroti.Runtime`이 제공하는 Dart 언어·표준 라이브러리 호환 API를 제거하고, 제품의 공개 API와 내부 소비 코드를 C#/.NET 표준 타입 중심으로 전환한다. `Future → Task`, typed-data → .NET 메모리 타입처럼 실제 계약과 소비 방식까지 변경한다. **Flutter API 구현인 `Duration`은 유지한다(2026-09-23 사용자 결정).** `Dart` 접두사만 바꾸거나 같은 호환 계층을 다른 제품 프로젝트로 옮기는 것은 완료가 아니다.

현재 Runtime은 이미 `Microsoft.NET.Sdk`를 쓰는 C# 프로젝트다. 공통 설정은 `net10.0`, C# 14, nullable 및 warnings-as-errors이며, Runtime에는 `IsAotCompatible=true`가 설정되어 있다. `Future`는 `Task`를 감싸고, 타이머는 `TimeProvider`/`ITimer`, HTTP는 `System.Net.Http.HttpClient`를 사용한다. **이번 작업의 핵심은 Dart VM 교체가 아니라 .NET 위에 남아 있는 Dart 호환 API와 의미 변환 코드의 제거다.**

### 포함 범위

- `Doroti/src/Doroti.Runtime`의 모든 타입을 대체·제품 고유 기능 유지·미사용 삭제·도구 전용 격리 중 하나로 분류한다.
- 해당 타입이 공개 계약이나 호출부에 나타나는 `Doroti.Ui`, `Doroti.Hosting`, `Doroti.Framework.*`, host/target runner, renderer, sample, template, 검증 코드를 함께 수정한다.
- 제품 동작에 필요한 dispatcher, 콜백 순서, 취소·오류 관찰, 버퍼 소유권은 .NET 기반의 명시적인 Doroti 계약으로 유지한다.
- Runtime 제거 API에 의존하는 선택 도구 `tools/Doroti.DartToCSharp`의 빌드·출력 계약을 정리한다.
- 공개 API 변경 목록과 C# 앱의 이전 방법, package/template 소비 검증을 포함한다.

### 범위 경계

- `Doroti.Runtime` 프로젝트·패키지 이름은 유지한다. Material 색상 계산과 작은 공용 계약까지 삭제할 이유는 없다.
- Flutter UI 동작·디자인, 렌더러 구조, 플랫폼 지원 범위, 현재 Web Worker 구조는 유지한다. [보관된 Web 성능·메모리 작업](history/26-09-22/web-frame-cost-and-memory-summary.md)은 별도 작업이며 수정 충돌과 회귀만 함께 관리한다.
- `reference/`, 과거 `history/` 및 선택 import 도구의 Dart 소스·원본 출처·라이선스까지 삭제하지 않는다. 제품 런타임의 Dart 의존 제거와 저장소 전체의 Dart 문자열 제거를 구분한다.
- Framework 전체의 PascalCase 변경, `long → int` 일괄 변경, 전체 import compiler 재작성, Mono/NativeAOT 실행 엔진 교체는 포함하지 않는다.
- Framework 소스는 제품 소유 C#이다. 변환기로 재생성하여 덮어쓰지 않는다. [현재 소유권 설명](Doroti/README.md)

## 2. 현재 구조와 영향도

### 2.1 의존 경로

```text
Doroti.Runtime
  → Doroti.Ui / Doroti.Hosting / Framework 각 계층
  → Framework Widgets / Material / Cupertino의 공개 API
  → 플랫폼 host·target runner / renderer / TestbedApp / templates

Host 이벤트·프레임
  → PlatformDispatcher.EnterScope / DispatchAndDrainMicrotasks
  → DartAsyncRuntime의 scheduler·TimeProvider scope
  → Framework의 Future callback / Timer / 후속 큐 작업

선택 import 도구 Doroti.DartToCSharp
  → Runtime·Ui project reference
  → lowering이 Doroti.Runtime의 Dart API 이름을 C# 출력에 기록
```

Runtime만 수정하면 충분하지 않다. `Doroti.Ui`, `Doroti.Hosting`, 여러 Framework 프로젝트와 Web target은 Runtime을 직접 참조한다. 상위 계층에는 간접 의존도 있다. 특히 Widgets의 `FutureBuilder`/`StreamBuilder`, Scheduler의 `TickerFuture`, Foundation의 `SynchronousFuture`는 타입 교체와 동작 변경을 함께 설계해야 한다.

검토 시 `Doroti/src`의 `.cs`에서 Runtime 폴더를 제외하고 단어 검색한 파일 수는 다음과 같다. 주석·별칭도 포함하는 **정적 영향도 참고치**이며, 호출 수나 완전한 심벌 참조 수가 아니다. sample/template/tool은 이 수에 포함하지 않았다.

| 검색 심벌 | Runtime 외 파일 수 |
| --- | ---: |
| `DartRuntimePrimitives` | 513 |
| `Duration` | 177 |
| `DartMap` | 174 |
| `Future` | 112 |
| `DartAsyncRuntime` | 36 |
| `Uint8List` | 22 |

재집계 예: `rg -l '\bFuture\b' Doroti/src -g '*.cs' -g '!**/Doroti.Runtime/**'`. 최종 삭제 판정에는 이 검색만 사용하지 않고 컴파일 심벌과 패키지의 공개 API를 확인한다.

### 2.2 소스에서 확인한 주요 계약

| 영역 | 현재 구현과 전환 시 주의점 | 근거 |
| --- | --- | --- |
| 비동기 | `Future<T>`/`Future`, custom async method builder, `Completer<T>`, callback chaining이 `Task`를 감싼다. callback은 캡처한 scheduler에 전달한다. | [DartAsync.cs](Doroti/src/Doroti.Runtime/DartAsync.cs) |
| 이벤트·시간 | dispatcher가 FIFO 후속 작업 큐를 drain하고 view를 깨운다. Web은 scoped `BrowserTimeProvider`를 사용하며, browser에서 scope가 없으면 예외를 낸다. | [PlatformDispatcher.cs](Doroti/src/Doroti.Ui/PlatformDispatcher.cs), [BrowserTimeProvider.cs](Doroti/src/Doroti.Host.Web/BrowserTimeProvider.cs), [Web runner](Doroti/src/Doroti.Target.Web.browser-wasm/DorotiWebWorkerRunner.cs) |
| 완료·취소 | `SynchronousFuture.then`은 즉시 호출한다. `TickerFuture`의 기본 완료와 `orCancel`은 취소 시 결과가 다르다. | [synchronous_future.cs](Doroti/src/Doroti.Framework.Foundation/synchronous_future.cs), [ticker.cs](Doroti/src/Doroti.Framework.Scheduler/ticker.cs) |
| Map | `DartMap`은 `List<KeyValuePair<...>>` 기반이며 null 키와 삽입 순서를 지원한다. generic indexer의 없는 키 읽기는 예외다. 별도 helper와 비제네릭 접근 계약도 확인해야 한다. | [DartCollectionsAndConvert.cs](Doroti/src/Doroti.Runtime/DartCollectionsAndConvert.cs) |
| 바이트·숫자 배열 | `Uint8List`는 `IList<long>`이다. 일부 view는 공유 버퍼를 쓰며 `ByteData.asMemory()`는 복사한다. `getUint32`는 big-endian, `asUint32List`는 little-endian 읽기다. | [DartTypedData.cs](Doroti/src/Doroti.Runtime/DartTypedData.cs) |
| Stream | callback listen과 broadcast를 지원한다. `IAsyncEnumerable` 변환은 구독마다 Channel을 만들고 종료 시 구독을 해제한다. `addError`와 채널의 terminal error는 동일한 계약이 아니다. | [DartAsync.cs](Doroti/src/Doroti.Runtime/DartAsync.cs) |
| 부가 어댑터 | 문자열·Unicode·정규식·JSON·URI·파일·수학·enum/hash·weak table·linked list·진단 기능이 섞여 있다. 이미 BCL에 위임하는 항목이 많다. | [Core](Doroti/src/Doroti.Runtime/DartCoreAdapters.cs), [Primitives](Doroti/src/Doroti.Runtime/DartRuntimePrimitives.cs), [Foundation ports](Doroti/src/Doroti.Runtime/FoundationRuntimePorts.cs) |
| 유지 대상 | Material 색상 계산·이미지 quantization과 pointer interface는 Dart 표준 라이브러리 흉내와 구분해야 한다. | [MaterialColorSchemeRuntime.cs](Doroti/src/Doroti.Runtime/MaterialColorSchemeRuntime.cs), [PointerEventContracts.cs](Doroti/src/Doroti.Runtime/PointerEventContracts.cs) |
| import 도구 | Runtime project reference와 `dart:async`, `dart:math`, `dart:convert` lowering의 하드코딩이 있다. | [compiler project](tools/Doroti.DartToCSharp/Doroti.DartToCSharp.csproj), [lowering symbols](tools/Doroti.DartToCSharp/src/Backend/CSharp/Lowering/FrameworkCSharpLowerer.Symbols.cs) |

## 3. 대체 설계

### 3.1 타입별 기본 방향

아래는 목표 설계다. 같은 이름의 타입을 BCL로 연결하는 전역 alias를 최종 구현으로 남기지 않는다. 일대일 대응이 없는 항목은 소비 코드의 실제 목적을 기준으로 계약을 줄인다.

| 기존 타입·기능 | 목표 | 보존하거나 명시할 계약 |
| --- | --- | --- |
| `Future<T>`, `Future`, `Completer<T>`, method builder | `Task<T>`, `Task`, `TaskCompletionSource<T>`, C# `async`/`await` | 성공·오류·취소, inline 완료와 지연 callback 차이, 다중 await, 오류 관찰 |
| `FutureOr`를 `object`로 받는 callback | 명시적 `Func<..., Task<T>>`; 동기 overload는 필요한 곳에만 제공 | 암묵적인 반환값 변환 제거. `ValueTask<T>`는 즉시 완료 비중과 단일 소비 조건이 확인된 내부 경로에 한정 |
| `Duration` | Flutter API 구현으로 유지; .NET 경계에서 `TimeSpan`과 변환 | microsecond→tick 단위, 음수·0·overflow·곱셈 반올림/절삭, wire timestamp 단위 |
| Dart `Timer`·microtask | `TimeProvider`, `ITimer`, dispatcher의 명시적인 후속 작업 큐 | UI owner 복귀, FIFO·재진입, 취소 직전 enqueue 경합, 해제된 view의 callback 차단 |
| `Stream<T>`, controller/subscription | async pull은 `IAsyncEnumerable<T>`/`Channel<T>`; UI push는 명시적 .NET event/구독 계약 | broadcast, 오류 후 재개, 완료, 해제, buffer 정책. Channel 하나를 여러 구독자의 broadcast로 사용하지 않음 |
| `DartMap`, `MapEntry`, collection helpers | `Dictionary`, `OrderedDictionary` 또는 명시적 ordered 항목 목록, `KeyValuePair`, `List`, `HashSet`, LINQ | 순서가 필요한 위치, null 키와 null 값 구분, equality/hash, 없는 키, duplicate/update/remove |
| `PriorityQueue<T>`, `HeapPriorityQueue<T>` | `System.Collections.Generic.PriorityQueue<TElement,TPriority>` | comparator 의미, 동일 우선순위 순서가 필요하면 sequence를 포함한 priority |
| `DartLinkedList`·entry | `LinkedList<T>`/`LinkedListNode<T>` 또는 제품 소유 구독 목록 | node 소속, remove identity, 순회 중 변경과 listener 해제 |
| `ByteBuffer`, `ByteData`, `Uint8List`·typed lists | `byte[]`, `Memory<T>`, `ReadOnlyMemory<T>`, 각 숫자 배열, `BinaryPrimitives` | view/복사 구분, offset/length, endian, native/JS 경계 수명. `Span<T>`를 비동기 저장 필드로 사용하지 않음 |
| Dart `HttpClient`·request/response·`DartFile`·`DartUri` | `System.Net.Http`·`System.IO`·`System.Uri`와 host의 capability 경계 | response/stream dispose, cancellation, Web 시간 소스, URI 정규화·상대 경로·escaping |
| JSON/UTF8/base64/gzip/정규식 | `System.Text.Json`, `Encoding.UTF8`, `Convert`, `GZipStream`, `Regex` | JSON 숫자·null·오브젝트 형태, wire 호환성, 정규식 flags·치환·match 위치 |
| `StringBuffer`, `Characters`, `CharacterRange`, math/random | `StringBuilder`, `StringInfo`/text-element enumeration, `Math`, `Random` | grapheme과 UTF-16 index 차이, 문자열 offset, 결정적 seed가 필요한 소비자 |
| `Expando`, ports·`Invocation` | `ConditionalWeakTable`; typed callback/message 계약; 미사용 invocation 제거 | weak lifetime, 동기/비동기 전달, message correlation. 현 `SendPort`는 직접 callback이며 실제 isolate 실행기가 아님 |
| `DartRuntimePrimitives`·Core extensions·enum/hash·오류 어댑터 | 정적 C# 연산, BCL exception·hash·타입 API, 구체적인 typed helper | null/identity/변환/날짜 정규화/assert의 실제 사용 목적. 무조건 `Equals`, cast 또는 `!`로 바꾸지 않음 |
| `DartDeveloperTimeline`, `Flow`, `TimelineTask` | `ActivitySource`/`DiagnosticSource`/`EventSource` 중 현재 수집 경로에 맞는 표준 진단 | flow 연결·begin/end 짝, 비활성 비용, 필요한 진단 필드와 bounded 기록 |
| Material 색상·quantizer, pointer 계약 | 현재 제품 기능 유지 | 정확성·라이선스·의존 방향. Dart 타입을 사용하는 부분만 BCL로 변경 |

### 3.2 비동기·UI owner 계약

`Task`로 타입만 바꾸면 기존 callback의 큐 진입, 실행 순서와 UI owner가 보존되지 않는다. `await`는 이미 완료된 Task에서 동기 진행할 수 있고, `RunContinuationsAsynchronously`도 UI dispatcher로의 복귀 자체를 보장하지 않는다. [공식 ConfigureAwait 설명](https://devblogs.microsoft.com/dotnet/configureawait-faq/)

- `PlatformDispatcher`가 후속 작업 큐와 view wake-up을 소유하는 구조를 유지한다. Runtime에는 필요하면 host 중립적인 실행 context와 시간 소스 계약만 둔다. Runtime에서 Ui를 참조하는 순환 의존을 만들지 않는다.
- UI 진입 시 host context를 어떻게 설치·복원할지 R1에서 결정한다. 기존 host `SynchronizationContext`와의 통합 또는 명시적 dispatcher 호출을 사용하며, 설치되지 않은 context의 자동 캡처를 가정하지 않는다.
- 큐를 반드시 거쳐야 하는 callback과 즉시 값으로 진행해도 되는 계산을 구분한다. `Task.Run`, `Task.Yield`, `ContinueWith`로 microtask를 일괄 대체하지 않는다.
- `SynchronousFuture`는 별도 Future 상속 없이 즉시 결과 처리와 Task 경로로 바꾸되 localization·image 초기 상태의 동기 전달을 검증한다.
- `TickerFuture`는 기본 완료와 취소 관찰을 명시적인 Task 계약으로 제공하는 제품 타입으로 전환한다. 취소 시 기본 await가 계속 미완료인 현 동작과 `orCancel`의 오류를 기준선에 기록하고, 새 계약에서의 처리와 앱 이전 방법을 명시한다. 두 경로를 무심코 같은 canceled Task로 합치지 않는다.
- `FutureBuilder`/`StreamBuilder`의 위젯 이름 변경은 필수 목표가 아니다. 입력은 .NET 계약으로 전환하고, subscription 교체·dispose 후 stale callback 차단·snapshot 상태 순서를 보존한다.
- fire-and-forget 작업은 오류 sink와 작업 식별자를 통해 관찰한다. 무시된 Task나 `async void`로 오류를 숨기지 않는다.

시간 관련 API는 `TimeProvider`를 전달하는 `Task.Delay`, `WaitAsync`, `CancellationTokenSource` overload를 사용한다. native에서는 system provider, Web에서는 기존 browser provider가 기본 소유자가 된다. [공식 TimeProvider 설명](https://learn.microsoft.com/en-us/dotnet/standard/datetime/timeprovider-overview)

### 3.3 컬렉션·버퍼·API 변경 정책

- 일반 lookup은 Dictionary를 우선 사용한다. 순서에 따라 첫 번째 상태를 고르는 widget-state map, 진단 출력, codec 등은 소비자별로 ordered 계약 필요 여부를 확인한다.
- null 키가 필요한 protocol/map은 일반 Dictionary에 그대로 옮기지 않는다. 명시적인 키 표현 또는 codec 전용 항목 목록으로 처리한다. null **값**이 있다는 이유로 null 키 대책을 추가하지 않는다.
- byte/typed-data 변환은 codec→messenger→host와 asset→image decode→renderer 전체 경로를 같은 단계에서 수정한다. pooling·zero-copy는 별도 성능 근거 없이 함께 도입하지 않는다.
- 공개 API의 `Future`, `DartMap`, typed-data 변경은 source/binary breaking change다. `Duration` 자체는 유지한다. 구형 DLL 교체 호환을 주장하지 않고 전체 소비 프로젝트를 재빌드한다. 다음 배포 시 버전·migration guide에 반영하되 이 계획 작성 단계에서 버전을 올리거나 배포하지 않는다.
- 단계 이행용 bridge는 소유자·제거 단계·남은 소비자를 기록한다. 제품 최종 상태에는 legacy alias, type forwarding, 별도 제품용 Dart compatibility package를 남기지 않는다.

## 4. 단계별 작업

실행 순서는 **R0 → R1 → R2 → R3 → R4 → R5 → R6 → R7 → R8**이다. 각 단계는 대체 구현·모든 관련 소비자·해당 계약 검증을 한 변경 단위로 구성한다. 공통 `obj`를 쓰는 빌드는 직렬 실행한다.

### R0. 심벌 인벤토리·기준선 — PARTIAL

1. Runtime 15개 `.cs` 파일의 public/internal 타입·멤버, 소비 프로젝트, 공개 API 노출, 사용 여부를 수집한다. extension method와 global using은 Roslyn 심벌 조회로 보완한다.
2. 각 항목에 BCL 대체, 제품 고유 계약, 미사용 삭제, import 도구 전용 처리를 배정한다. Runtime 밖의 `IDartEnumIndex`, `IDartTweenValue` 구현과 helper 호출도 포함한다.
3. 현재 build/runtime 성공 여부와 기존 실패를 실제 실행으로 확보한다. 현재 문서의 정적 조사 결과를 실행 PASS로 사용하지 않는다.
4. 기존 [warning-remediation contracts](Doroti/validation/warning-remediation/Contracts/Program.cs)의 null·Future timeout·Ticker·메시지 계약을 검토한다. 내부 타입만 확인하는 검증은 새 API로 바꾸고, 행동 계약은 보존한다.
5. `Doroti/validation/runtime-dotnet/` 아래에 하나의 계약 검증 진입점, README, 인벤토리·API 변경표를 구성할 계획으로 시작한다. 현재 이 경로는 새로 만들 대상이며 존재하는 검증기로 간주하지 않는다.

**통과 기준:** Runtime 전체 심벌의 처리 방향과 영향 경로가 빠짐없이 기록되고, 주요 동작 기준선 및 기존 실패가 구분되어 있다.

### R1. .NET 실행 context·시간·오류 관찰 기반 — PARTIAL

1. dispatcher의 후속 작업 큐·wake-up·scope·view lifetime을 명시적인 Doroti 실행 계약으로 정리한다. 기존 Dart API와의 임시 bridge에는 R3 제거 기한을 둔다.
2. `BrowserTimeProvider`와 Web runner의 scope를 새 계약에 연결한다. HTTP timeout, debug print, Material 비동기 처리 등 provider를 직접 읽는 호출부도 포함한다.
3. callback enqueue 뒤 cancel/dispose되는 경합, 중첩 scope 복원, 복수 dispatcher 사이 격리, 오류 sink를 검증한다.
4. 기존 프레임 전후 drain 순서와 UI owner를 관측해 새 context가 동작을 보존함을 확인한다.

**통과 기준:** desktop와 Web 각각에서 owner·시간 소스·후속 작업 순서의 계약이 성립한다. 후속 Task 전환이 ThreadPool에서 UI를 갱신하는 경로를 만들지 않는다.

### R2. 시간·기초 값과 저위험 어댑터 전환 — PARTIAL

1. Flutter API 구현인 `Duration`을 유지한다. .NET host 경계의 `TimeSpan` 변환과 animation, gesture, scheduler, timestamp 단위 동작을 검증한다.
2. StringBuffer, 단순 math/encoding, Expando, 파일·URI의 직접 대체 가능한 소비자를 BCL로 옮긴다. 애매한 URI/문자열 의미는 R5에 남은 항목으로 기록한다.
3. public constructor의 기본값, nullable 값, 상수·곱셈·나눗셈과 checked 범위를 검토한다.

**통과 기준:** `Duration` API가 유지되고, .NET 경계의 animation·gesture·clock 단위 회귀와 해당 소비자 빌드가 통과한다.

### R3. Future·Completer와 상위 비동기 API 전환 — TODO

1. 기본 Future/Completer 호출을 Task/TaskCompletionSource/async-await로 옮긴다. `then`, `catchError`, `whenComplete`, `wait`, `timeout`, `FutureOr` 각각의 오류·완료 의미를 적용한다.
2. `SynchronousFuture`, `TickerFuture`, localization, 이미지 로딩, navigation/dialog 결과, scroll/animation 완료, 플랫폼 메시지와 `FutureBuilder`까지 연결한다.
3. C# override·callback signature·generic 결과·null 결과를 함께 수정한다. `whenComplete`는 cleanup 오류 처리, `wait`는 결과 순서와 실패 관찰, timeout은 대기 종료와 실제 작업 취소를 구분한다.
4. task cancel/fault와 `TickerCanceled`의 대응을 API 변경표에 기록한다. `.Wait()`/`.Result`를 이용한 UI 동기 대기로 변환하지 않는다.
5. Future 클래스, builder, Completer, `DartErrorHandlers`, FutureOr 범용 object adapter와 R1의 제품용 bridge를 제거한다. stream이 Task를 반환하도록 기본 계약도 함께 갱신한다.

**통과 기준:** 제품 공개 API·구현에 Runtime Future/Completer/builder 의존이 없고, 즉시/지연 완료·취소·timeout·오류·dispose 후 callback 회귀가 통과한다.

### R4. 컬렉션과 typed-data 전환 — PARTIAL

1. `DartMap` 소비자를 lookup/ordered/null-key protocol로 나누어 목적에 맞는 .NET 표현으로 옮긴다. `MapEntry`, map equality·deep equality, collection extension도 정리한다.
2. scheduler priority queue와 Widgets의 linked-list listener를 전환한다. node/listener identity와 동일 우선순위 순서를 검증한다.
3. `ByteData`, `Uint8List`, typed lists를 Services codecs·asset bundle·Ui image API·host/native interop·renderer까지 전환한다.
4. offset view의 변경 가시성, 복사본 독립성, 0-length, endian, overflow·범위 오류, 비동기 완료 전 버퍼 수명을 검증한다.

**통과 기준:** 제품 Runtime의 Dart 컬렉션·typed-data 타입 의존이 제거되고, 플랫폼 메시지의 고정 byte fixture 및 이미지·asset 경로가 일치한다.

### R5. Stream·I/O·텍스트·진단 전환 — TODO

1. Services `platform_channel.cs` 등의 event stream과 Widgets `async.cs`의 소비 계약을 정리한다. async sequence와 push 구독을 별도 설계하고 unsubscribe/cancellation을 연결한다.
2. `StreamController`, `StreamSubscription`, Runtime `Stream<T>`, ReceivePort/SendPort를 삭제한다. message correlation·owner dispatch는 실제 Services 계약에서 유지한다.
3. network image 등 HTTP 소비자를 .NET request/response/stream으로 전환한다. client lifetime, 실패 응답, timeout, 취소, response dispose를 검증한다.
4. JSON의 실제 반환형과 codec 형식을 명시한다. 문자열·Characters·CharacterRange는 emoji, ZWJ, 결합 문자와 selection offset 사례로 검증한다.
5. timeline·flow와 예외 진단을 표준 .NET 진단으로 연결한다. 실제 listener가 수집하는 항목과 diagnostic-off 동작을 확인한다.

**통과 기준:** 새 스트림 계약에서 broadcast/오류/종료/해제 동작을 충족하고, 제품의 I/O·텍스트·진단 호출이 Dart 표준 라이브러리 wrapper 없이 동작한다.

### R6. 언어 helper 제거·선택 도구 경계 정리 — PARTIAL

1. `DartRuntimePrimitives`, `DartCoreExtensions`, `FoundationRuntimePorts`, `Dart_*Library`, `Invocation`, Dart exception과 interface의 잔여 참조를 심벌별로 처리한다.
2. 단순 static typing으로 표현할 수 있는 변환은 C#으로 옮긴다. null→default, reference identity, NaN/음수 zero, 날짜 overflow, enum index, release assert 의미를 전역 치환으로 바꾸지 않는다.
3. 보간·pointer·색상·dispatcher 등 남는 제품 helper는 기능 소유 프로젝트 또는 Runtime의 구체적인 계약으로 정리한다. 거대한 `DotNetRuntimePrimitives`로 이름만 바꾸지 않는다.
4. import 도구는 제품 API에 연결되는 출력부터 새 .NET 타입으로 수정한다. 일반 Dart 언어 의미 때문에 필요한 잔여 호환 구현은 **도구 전용** assembly/namespace로 격리할 수 있으나, 제품이 이를 참조하거나 패키지에 싣지 못하게 한다. 단순히 compiler만 빌드하는 것으로 끝내지 않고 대표 출력 C#도 컴파일한다.
5. Runtime과 연결되는 `DartPerformanceMode` capability 등 주변 Dart VM 명칭·계약은 실제 host 구현 유무를 조사한다. 없는 VM 기능은 새 이름의 가짜 .NET 지원으로 제공하지 않는다. 제품에 필요한 정책만 별도로 설계하고 제거 API를 기록한다.
6. source generator/lowering, package reference, global using, validation fixture에서 제거한 타입이 재유입되지 않도록 확인한다. 원본 Flutter 출처 주석은 보존한다.

**통과 기준:** 제품은 Dart 언어 호환 assembly/API를 사용하지 않는다. 선택 도구의 유지되는 기능·격리 경계·미지원 출력은 명시되어 있고 제품 build에 Dart SDK가 필요하지 않다.

### R7. 전체 소비자·패키지·문서 전환 — PARTIAL

1. Runtime부터 Ui/Hosting, Foundation/Scheduler/Services, 나머지 Framework, host/target, renderer, TestbedApp 순으로 실제 project graph를 따라 빌드한다.
2. `DorotiTestbedApp`, template의 API 사용 예제, validation과 optional tool의 project graph를 정리한다. warnings-as-errors와 AOT 분석 설정을 낮추지 않는다.
3. 로컬 NuGet package를 만들고 저장소 밖 소비 프로젝트와 생성된 template에서 restore/build한다. 이전 Runtime DLL이나 repository-private 참조가 실패를 가리지 않게 한다.
4. 공개 API 전후 차이와 C# 이전 예제를 작성한다. 새 API는 .NET 타입을 받으며 legacy 제품 facade 제거를 확인한다.

**통과 기준:** 지원 대상별 필요한 빌드 결과, 새 package/template 소비 결과와 소스 이전 문서가 있으며 제거 타입의 DLL·전이 패키지 의존이 없다. 실행하지 못한 플랫폼 빌드는 `notVerified`다.

### R8. 통합 회귀·최종 삭제 확인 — TODO

1. 아래 검증 행렬을 수행하고 R0 기준선과 비교한다. 코드 규모 감소를 성능 향상으로 보고하지 않는다.
2. 인벤토리의 모든 심벌 처리 상태, 임시 bridge·alias 0개, 공개 API 및 package dependency 잔여 검사를 확인한다.
3. 기존 검증 스크립트가 삭제 타입이나 없어진 파일을 참조하는 경우 새 계약으로 갱신한다. history 문서의 과거 결과는 수정하지 않는다.
4. 최종 결과는 변경 범위·breaking changes·실행한 명령·환경·결과·미검증 사항으로 기록한다. 원시 산출물은 `Doroti/artifacts`에 두고, 요약과 필수 증거는 추적되는 validation 문서에 보존한다.

**통과 기준:** 구조적 제거와 실행 회귀가 각각 판정되어 있다. 필수 platform/runtime 검증이 누락되면 전체 상태는 PARTIAL이며 계획 항목을 임의로 완료 처리하지 않는다.

## 5. 검증 행렬과 실행 규칙

| 검증층 | 필수 시나리오 | 증명 범위 |
| --- | --- | --- |
| 정적/API | 심벌 처리표, 공개 API diff, assembly/package dependencies, global using·alias, optional tool 출력 | 제품 Dart 호환 계층 제거와 API 표면 |
| managed 계약 | Duration/TimeSpan 경계, Task 성공/실패/취소·동기 완료, callback FIFO/재진입, Ticker, ordered/null-key map, codec byte fixture, view/copy lifetime, Unicode, stream 종료·오류·해제 | 플랫폼과 분리된 의미·수명 계약 |
| Framework 통합 | FutureBuilder/StreamBuilder 갱신·교체·dispose, localization 즉시 로딩, image load/error, Navigator 결과, scroll/animation 완료, focus·text input 후속 큐 | 실제 소비 경로와 UI 상태 전이 |
| Windows 실행 | 사용되는 Windows App SDK 및 MAUI 경로의 startup, 클릭·스크롤·animation·gesture timer, 입력·window 종료 후 작업 | 각 host runtime과 owner 연결 |
| Web 실행 | 현재 기본 WebGPU 및 지원 WebGL 경로, Worker time provider, delay/timeout·HTTP 실패·이미지·stream·view 해제 | browser/Worker 실행. desktop browser 자동화를 Safari/휴대폰 실기기 증거로 사용하지 않음 |
| Android/iOS/macOS/Linux | 해당 OS build/publish와 입력·gesture·timer·메시지·이미지·해제 smoke | build·emulator·실기기 결과를 분리. 필요한 host가 없으면 `notVerified` |
| AOT/trim | 제품이 사용하는 대상의 실제 AOT publish, 분석 경고와 시작 smoke | `IsAotCompatible` 선언이나 보통 build만으로 AOT 완료를 주장하지 않음 |
| package/template | 로컬 pack, 외부 C# 소비자, template 생성 후 restore/build·대표 실행 | 저장소 안 project reference에 가려지지 않은 배포 계약 |
| 비용 회귀 | 동일 Release 화면·입력으로 Task/collection/byte copy 할당, UI 지연, startup·메모리 비교 | 측정된 경로만 판정. [보관된 Web 성능·메모리 작업](history/26-09-22/web-frame-cost-and-memory-summary.md)의 미달 목표를 자동으로 해결했다고 처리하지 않음 |

모든 테스트 프로세스에는 저장소 규칙의 **20분 timeout**을 적용한다. `Doroti/validation/run-with-timeout.py`는 프로세스 트리를 포함해 1,200초 deadline을 적용하는 기존 진입점이다. 수백 회 반복하지 않고 결정적 계약 테스트와 필요한 범위의 반복(대부분 30회 이내)을 사용한다.

아래는 구현 단계의 명령 예시이며 **이번 계획 작성 중 실행하지 않았다**. Runtime 빌드만으로 전체 제품 통과를 판정하지 않는다.

```powershell
python Doroti/validation/run-with-timeout.py dotnet build Doroti/src/Doroti.Runtime/Doroti.Runtime.csproj -c Release
python Doroti/validation/run-with-timeout.py dotnet build Doroti/src/Doroti.Framework.Material/Doroti.Framework.Material.csproj -c Release
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/warning-remediation/Contracts/Contracts.csproj -c Release
python Doroti/validation/run-with-timeout.py dotnet build tools/Doroti.DartToCSharp/Doroti.DartToCSharp.csproj -c Release
```

host/publish 명령은 실행 시 해당 OS·SDK·workload·RID와 현재 `Doroti/eng/doroti.ps1` 경로를 확인해 추가한다. 전체 multi-target solution을 Windows 한 환경에서 빌드하는 것으로 모든 플랫폼 검증을 대체하지 않는다.

## 6. 최종 완료 정의

- [ ] Runtime의 모든 심벌이 대체·유지·삭제·도구 전용으로 분류되고 미처리 항목이 없다.
- [ ] 제품 공개 API와 구현에 Runtime `Future`, `Completer`, Dart collection/typed-data/stdlib wrapper, 언어 범용 helper 의존이 없다. Flutter API `Duration`은 유지한다.
- [ ] 남은 Runtime 코드는 BCL 기반의 제품 고유 기능이며 Dart 호환 계층을 다른 제품 프로젝트에 옮겨 숨기지 않았다.
- [ ] UI owner·후속 큐·시간 소스·취소·오류·버퍼 수명·wire 형식의 계약 검증이 통과했다.
- [ ] Framework·hosts·samples·templates·검증 코드와 선택 도구의 연결 변경이 완료되었다.
- [ ] 제품 일반 build/package 소비에 Dart SDK·import compiler·도구 전용 compatibility assembly가 필요하지 않다.
- [ ] 임시 bridge·legacy alias와 제품 패키지의 전이 호환 의존이 제거되었다.
- [ ] 공개 API migration guide와 플랫폼별 build/runtime/AOT/실기기 결과가 각각 기록되었다.
- [ ] 필수 미검증 항목이 있다면 최종 상태를 PARTIAL로 남겼다.

## 7. 공식 문서 검토 근거

2026-09-23 확인. 아래 표준 라이브러리 특성과 현재 소스의 소비 방식을 함께 검토해 위 설계를 제안했다. 문서의 기능 존재가 Doroti의 실제 동작 검증을 대신하지는 않는다.

- [TimeProvider](https://learn.microsoft.com/en-us/dotnet/standard/datetime/timeprovider-overview): 시간 소스·타이머와 provider를 받는 Delay/WaitAsync/취소 API.
- [ConfigureAwait FAQ](https://devblogs.microsoft.com/dotnet/configureawait-faq/): continuation context와 완료된 await의 실행 방식. UI owner와 큐 순서를 따로 설계하는 근거.
- [System.Threading.Channels](https://learn.microsoft.com/en-us/dotnet/core/extensions/channels): producer/consumer와 bounded/unbounded buffer. broadcast 구독의 직접 대체로 간주하지 않는 근거.
- [.NET 10 PriorityQueue](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.priorityqueue-2?view=net-10.0): 최소 priority queue이며 동일 priority의 FIFO를 보장하지 않으므로 소비 계약 확인 필요.
