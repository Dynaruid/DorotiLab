# Doroti.Runtime C#/.NET 전환 요약

원본: 삭제한 `work2.md`(2026-09-23). 계획 검토 기준 `277e4395`, 실행 기준선 `94d24bbd`. **R0–R2 PARTIAL / R3 TODO / R4 PARTIAL / R5 TODO / R6–R7 PARTIAL / R8 TODO**를 보존한다. 전환 전체가 완료된 기록이 아니다.

## 목표와 유지 계약

Dart VM 교체가 아니라 .NET 위의 Dart 호환 API·의미 변환 계층을 제거하고 실제 공개 API와 소비자를 BCL 중심으로 전환하는 작업이다. `Doroti.Runtime` 프로젝트·패키지, Material 색상/quantizer·pointer 등 제품 고유 기능은 유지한다. **사용자 결정에 따라 Flutter API `Duration`은 유지하고 .NET 경계에서만 `TimeSpan`으로 변환한다.**

범위는 Runtime → Ui/Hosting/Framework → host/runner/renderer → sample/template/검증 및 선택 import 도구까지다. 제품 소유 Framework C#을 변환기로 재생성하지 않으며, 원본 출처·라이선스·history를 삭제하지 않는다. renderer/Worker 구조, 전체 PascalCase 변경, `long` 일괄 축소, 실행 엔진 교체는 별도 범위다. [Web 성능·메모리 작업](../26-09-22/web-frame-cost-and-memory-summary.md)도 별도로 남는다.

- Future/Completer는 Task/TaskCompletionSource로 옮기되 UI owner, FIFO·재진입, inline/지연 완료, 오류 sink, view 종료 후 callback 차단을 보존한다. `RunContinuationsAsynchronously`만으로 UI owner 복귀를 가정하지 않는다.
- `SynchronousFuture`의 즉시 결과와 `TickerFuture`의 기본 완료/`orCancel` 차이를 명시한다. `FutureBuilder`/`StreamBuilder`의 stale callback·snapshot 전이를 함께 검증한다.
- Stream은 async pull과 push 구독을 구분한다. Channel 하나를 broadcast 대체로 쓰거나 오류 후 재개를 terminal error로 바꾸지 않는다.
- Map은 lookup/ordered/null-key 목적별로 전환한다. byte/typed-data는 view·복사·offset·endian·비동기 수명과 codec wire 형식을 보존한다. `Span<T>`를 비동기 저장 필드로 쓰지 않는다.
- 공개 API 전환은 source/binary breaking change다. 전체 소비자 재빌드와 이전 문서가 필요하며 legacy alias·type forwarding·별도 제품용 호환 패키지를 최종 상태에 남기지 않는다.

## 단계별 수행 내용과 잔여

| 단계 | 상태 | 수행한 내용 | 남은 완료 기준 |
| --- | --- | --- | --- |
| R0 인벤토리·기준선 | PARTIAL | 타입/멤버 인벤토리, 원본 API 스냅샷, 기준선 빌드·기존 계약 | Roslyn-bound 사용처, 전체 심벌 처리표, 최종 package API diff |
| R1 실행 context·시간·오류 | PARTIAL | `DorotiExecutionContext`/`DorotiCallbackDispatcher`의 시간 소스·후속 큐 캡처 분리, dispose 작업 거부/취소, drain 재진입 순서 수정 | 실제 Web/Windows owner·시간·큐 순서 계약 |
| R2 시간·기초 adapter | PARTIAL | Duration/TimeSpan microsecond/tick·음수 절삭·overflow 예외 보강; StringBuilder, ConditionalWeakTable, FileInfo, Math, Random 소비 전환 | URI/encoding 등 잔여 adapter, animation/gesture/clock 경계 |
| R3 Future·상위 비동기 | TODO | 목표 설계 기록 | Future/Completer/builder 제거, Task 전환, localization/image/navigation/animation/message/widget 통합 회귀 |
| R4 컬렉션·typed-data | PARTIAL | scheduler BCL PriorityQueue, Widgets LinkedList, Matrix4/semantics/Canvas .NET 배열, 숫자 typed-list 제품 참조 제거, codec 숫자 배열 tag·왕복·int32 LE fixture | DartMap, Uint8List/ByteData 및 asset/image/native 전체 경로, 전체 codec 검증 |
| R5 Stream·I/O·문자·진단 | TODO | 목표 설계 기록 | 구독/취소/broadcast, HTTP 소유권·오류, Unicode/JSON/URI, 표준 .NET 진단 |
| R6 helper·도구 경계 | PARTIAL | import File.path/Random/math 일부 lowering 전환, 대표 fixture 분석·생성 C# 빌드 | 잔여 helper/출력/VM capability 조사, 도구 전용 격리, 임시 bridge 제거 |
| R7 소비자·패키지·문서 | PARTIAL | 진행 중 API 이전표, 주요 빌드, Runtime NuGet 외부 소비 실행 | 모든 package/template/platform 소비, 최종 migration guide·API diff |
| R8 통합·최종 삭제 | TODO | 완료 기준 정의 | 임시 bridge/alias 0, 제거 심벌·의존 0, 필수 플랫폼 실행·AOT·비용 회귀 |

`StringBuffer`, `Expando`, Runtime priority/linked-list 타입은 import 도구가 아직 출력할 수 있어 R6 제거 기한의 임시 bridge로 남았다. 제품 소비 전환을 타입 삭제 완료로 판정하지 않는다. 순서는 R0 → R1 → R2 → R3 → R4 → R5 → R6 → R7 → R8이다.

## 기록된 검증과 한계

- 기준선 Runtime·Material·import Release 빌드 경고/오류 0, 기존 계약 **103개 PASS**. 변경 후 Runtime, Material, Widgets, Web/Windows App SDK Testbed, `DorotiTestbedApp`, import 빌드 및 새 managed 계약 **54개 PASS**를 기록했다.
- 대표 compiler fixture는 Dart 분석 diagnostics 0과 생성 C# 빌드를 통과했다. compiler 자체 빌드만으로 다른 출력 경로를 승인하지 않았다.
- 배열 변경 후 Chrome CDP에서 당시 기본 WebGPU, checkbox/switch, 날짜 선택 취소, 텍스트 입력, 탭 재마운트, 페이지 예외 없음이 통과했다. **이는 09-23 당시 renderer 기록이며 현재 기본 정책을 정의하지 않는다.**
- 선택 `TimerValidationExport`를 페이지 JS에서 호출했지만 해당 컨텍스트에 `doroti.web` 타이머 dispatcher가 준비되지 않아 전용 계약 결과를 얻지 못했다. 계측을 되돌렸으며 Web/Windows 타이머·owner 계약은 `notVerified`다.
- 로컬 Runtime NuGet을 새 캐시로 복원해 package DLL과 현재 build DLL의 SHA-256 일치를 확인하고 저장소 밖 C# 소비 프로젝트를 실행했다. 전체 패키지/template 검증은 아니다.
- Windows/Android/iOS/macOS/Linux 실행, Safari·실기기 Web, 다른 compiler 출력, 전체 package/template, AOT/trim·성능은 `notVerified`다. `IsAotCompatible` 선언이나 일반 빌드만으로 AOT 완료를 주장하지 않는다.

후속 완료에는 managed 의미 계약, Framework 상태 전이, 각 host owner/시간/입력/종료, 실제 AOT publish, 외부 package/template 소비, 동일 Release 비용 비교가 필요하다. 모든 테스트는 [20분 process-tree 제한](../../Doroti/validation/run-with-timeout.py)을 적용하고 shared `obj` 빌드는 직렬 실행한다. 필수 검증이 누락되면 최종 상태는 PARTIAL이다.

## 추적되는 근거

- [Runtime 검증 안내](../../Doroti/validation/runtime-dotnet/README.md), [인벤토리](../../Doroti/validation/runtime-dotnet/inventory.md), [API 기준선](../../Doroti/validation/runtime-dotnet/api-baseline.txt), [진행 중 API 이전표](../../Doroti/validation/runtime-dotnet/api-changes.md)
- [기존 계약](../../Doroti/validation/warning-remediation/Contracts/Program.cs), [import 대표 fixture](../../tools/Doroti.DartToCSharp/validation/runtime-dotnet/validate.ps1)

위 결과는 삭제 직전 계획에 기록된 역사적 상태다. 이번 보관은 후속 소스 변경을 재평가하거나 미완료 단계를 구현하지 않았다.
