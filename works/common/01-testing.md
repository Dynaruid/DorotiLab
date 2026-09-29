# M1. 테스트 런타임과 핵심 회귀 장면

원문: [개발 로드맵](../../plan.md) §3 M1 · 우선순위: **P0 → P1** · 작업 상태: **PARTIAL** · 실행 검증: **범위별 PASS / 나머지 notVerified**

[작업 인덱스와 공통 완료 규칙](../README.md)

## 담당 범위와 선행 작업

결정적 테스트 런타임과 최소 공유 회귀 fixture를 담당한다. 일회성 검증 스크립트·테스트 프로젝트·산출물은 저장소 루트의 `temp/testing/` 아래에 모으고, 결과 요약 후 삭제한다. 호스트별 물리 입력·실제 화면 결과는 플랫폼 문서에 기록한다.

선행: [M0](00-foundation.md)의 실행기·산출물 규칙.

플랫폼별 연결·검증: [Windows](../platforms/windows.md) · [Web](../platforms/web.md) · [Android](../platforms/android.md) · [macOS / AppKit](../platforms/macos.md) · [iOS](../platforms/ios.md) · [Linux / Qt](../platforms/linux.md) · [Mac Catalyst](../platforms/maccatalyst.md)

## 작업과 완료 기준

원문의 테스트 기능·완료 기준을 유지하되, 검증 파일 누적과 기본 빌드 시간 증가를 막기 위해 임시 작업의 배치·실행·정리 규칙을 추가한다. 특정 호스트를 명시한 항목은 연결된 플랫폼 문서에서 해당 구현·검증을 추적하고, 이 문서에는 공유 계약 및 통합 결과를 기록한다. 공통 구현 완료가 모든 플랫폼 검증 완료를 뜻하지 않는다.

**선행:** M0의 실행기·산출물 규칙.

- [x] 임시 실행 폴더·1,200초 timeout·성공 실행 요약 후 정리를 M0 실행기에 연결했다. 기존 누적 자료의 정리 보류는 아래 관리 항목에서 따로 추적한다.
- [x] `Doroti.Testing`의 최소 계약을 정한다: `pumpWidget`, `pump`, 제한 시간이 있는 `pumpAndSettle`, `find.byType/byKey/text`, `tap/drag`, 키·텍스트 입력, semantics 조회.
- [x] clock/frame scheduler와 TimeProvider timer를 제어하고 bounded settle·frame/timer/scheduled·트리 진단 및 framework error 실패를 연결했다.
- [x] CPU binding/root/focus·engine-layer 해제와 서로 다른 owner thread의 pointer·한글 값·semantics·reassemble 격리를 검증했다. 같은 thread의 nested tester와 다른 thread/dispose 후 API 접근은 거절한다.
- [ ] GPU/texture/native resource·복수 owner의 전체 생성/재생성/종료 격리를 검증한다. CPU 회귀를 임의의 병렬 사용이나 전체 native 자원 수명 보장으로 확대하지 않는다.
- [x] pointer hit test → GestureArena → callback 경로를 통과시킨다. 위젯 콜백 직접 호출만으로 입력 테스트를 통과시키지 않는다.
- [x] Cupertino 탭·ListView 스크롤·합성 한글 조합/selection·Dialog 중간 프레임/모달 hit-test·VariableBlur geometry/DPR CPU 회귀를 고정했다. Windows editor/WebView 각각의 생성·재생성은 WindowsSmoke에서 확인했다.
- [ ] 위 장면의 GPU 품질, 플랫폼별 실제 키/semantics·물리 입력과 PlatformView 전체 조합을 검증한다.
- [ ] golden 비교는 폰트·DPR·renderer·색 공간·허용 오차를 고정한다. CPU 이미지 비교, GPU offscreen 비교, 실제 화면 캡처를 별도 결과로 기록한다.
- [x] 외부 PackageReference-only 앱의 Testing/Cupertino 회귀 실행을 검증했다.
- [ ] 테스트 실패 시 비교 이미지·트리·frame 정보·재현 명령을 해당 실행의 `temp/testing/` 하위에 남기는 전체 장면을 확인한다. CPU PNG/timeout 진단 API 제공과 golden 실패 산출물의 전체 검증을 구분한다.

**완료 기준:** 의도적으로 탭 대상·hit test·blur geometry를 깨뜨리면 해당 테스트가 실패하며, 복구 후 통과한다. 지속 animation에서는 `pumpAndSettle`이 무한 대기하지 않는다.

## 임시 테스트의 배치·실행·정리

아래 경로와 동작을 실행기에 적용했다. 상세 잔여 항목은 체크박스와 실행 결과를 함께 확인한다. `temp`는 OS 임시 폴더가 아닌 **저장소 루트의 `temp/`**를 뜻한다.

| 구분 | 위치·보존 원칙 |
| --- | --- |
| 일회성 재현·실험 | `temp/testing/<작업-ID>/<실행-ID>/`에 스크립트, 테스트 앱·프로젝트, 임시 fixture와 입력 파일을 함께 둔다. |
| 실행 산출물 | 같은 실행 폴더 아래에 로그·캡처·golden diff·trace·report·테스트 앱의 build/publish 결과를 모은다. |
| 제품 테스트 런타임 | 외부 앱에도 제공할 `Doroti.Testing` 구현은 제품 소스로 유지한다. 임시 재현 앱과 구분한다. |
| 상시 회귀 검증 | 반복 방지가 꼭 필요한 최소 테스트 소스·작은 fixture만 `Doroti/tests/`, 재사용하는 최소 실행기만 `Doroti/eng/`에 유지한다. 원시 실행 산출물은 `temp/testing/`에 둔다. |
| 장기 기록 | 해당 작업 문서에 결과 요약·재현 절차·미검증 범위만 남긴다. 일회성 스크립트와 원시 산출물을 `docs/`나 `history/`로 통째로 복사하지 않는다. |

- **상시 규칙 — 기본은 임시 작성:** 버그 조사용 probe, API 확인, 성능 비교, 일회성 smoke는 처음부터 실행별 폴더에 작성한다. 작업마다 영구 테스트 프로젝트·검증 스크립트·결과 문서를 새로 추가하지 않는다.
- **상시 규칙 — 영구 보존은 선별:** 반복적으로 확인할 공개 계약·실제 회귀가 있고 기존 테스트로 다룰 수 없을 때만 최소 사례를 남긴다. 보존 이유와 실행 시점을 기록하고, 우선 기존 회귀 장면에 통합한다. 기능 검증 완료가 일회성 테스트 파일의 영구 보존을 요구하지는 않는다.
- [x] **기본 개발 경로에서 제외:** `/temp/`를 Git ignore·관련 source glob·프로젝트 검색·watch/launch fingerprint·pack에서 제외했다. 임시 프로젝트와 테스트 executable을 기본 제품 solution에 넣지 않았다. 적용 근거는 [M0 결과](00-foundation.md)를 따른다.
- **상시 규칙 — 필요한 범위만 실행:** 변경과 관련된 최소 테스트를 명시적으로 실행한다. 이미 통과한 검증은 새 변경·실패·미해결 사항이 있을 때 재실행하며, 단순 문서 수정에 제품 빌드·전체 테스트를 요구하지 않는다. 테스트에는 20분 timeout을 적용하고 일반 반복 검증은 30회 이내로 제한한다.
- [x] **성공 실행 정리:** aggregate 실행의 생성·실행·요약·소유 폴더 자동 삭제 사이클을 확인했다.
- [ ] **정리 보류 해소:** 실패·timeout·중단·수동 조사 자료는 [M0](00-foundation.md#임시-정리-보류)와 [전체 실행 기록](../README.md)의 목록을 확인해 필요한 요약 후 정리한다. 미해결 자료의 사유·정리 시점을 남긴다.
- **상시 규칙 — 삭제 범위 제한:** 정리 대상의 절대 경로가 `temp/testing/` 아래인지 확인하고, 해당 실행 폴더만 삭제한다. 동시 실행 중인 다른 작업, 제품 소스, 영구 fixture와 공유 캐시는 삭제 대상에 넣지 않는다. 기존 `Doroti/artifacts` 산출물은 M0와 저장소 정리 지침에 따라 별도로 정리한다.
- [ ] **기존 누적 파일 정리:** 기존 파일을 상시 회귀·진행 중 조사·폐기 가능한 일회성 파일로 분류한다. 진행 중 조사만 `temp/testing/`으로 옮기고, 종료된 일회성 파일은 필요한 요약을 남긴 뒤 삭제한다. 경로를 참조하던 실행기·CI·문서도 함께 수정하며, 불필요한 파일 전체를 새 위치로 복제하지 않는다.

**관리 완료 기준:** 임시 테스트를 생성·실행·요약·삭제하는 한 사이클을 확인한다. `temp/testing/`이 없는 새 체크아웃에서도 제품 빌드와 상시 회귀 검증이 동작하고 실행 시 필요한 폴더를 다시 생성한다. 임시 프로젝트가 기본 빌드·watch·패키지에 포함되지 않으며, 작업 종료 후 임시 파일은 삭제되거나 보류 사유·정리 시점이 기록돼 있다. 삭제한 파일을 남아 있는 증거로 링크하지 않는다.

## 산출물과 결과 기록

- 공개 계약·구현 변경, 실행 가능한 제품 샘플, 선별한 최소 회귀 검증, 지원표 갱신을 함께 남긴다. 임시 테스트 앱은 영구 샘플로 간주하지 않는다.
- 이 문서의 완료 기준과 플랫폼별 적용 결과를 연결하고, 미실행 조합은 `notVerified`로 유지한다.
- 결과는 [공통 기록 형식](../README.md#결과-기록-형식)에 revision·환경·명령·기대값·실제값·남은 작업과 임시 파일 정리 여부를 남긴다. 삭제할 스크립트 경로만 재현 명령으로 남기지 않고, 기존 샘플·영구 테스트 명령 또는 임시 재현 앱의 구성 절차를 적는다.

## 2026-09-28 실행 결과

- 상태: **최소 runtime PASS / 전체 M1 PARTIAL**. revision: a93c047fe2e93d93cff3e0a6bf3c2789862fea81 + 작업 트리 변경 (미커밋).
- `Doroti.Testing` 제품 패키지에 `WidgetTester`, `TestClock`, CPU Skia host를 추가했다. pumpWidget/pump/pumpAndSettle, predicate/type/key/text finder, tap/drag, key/text packet, semantics 및 CPU PNG 진단 API를 제공한다. 초기 기록은 serial/non-nested 범위이며, 현재 독립 owner-thread 검증 범위는 아래 2026-09-29 결과를 따른다.
- `python Doroti/eng/run-with-timeout.py dotnet run --project Doroti/tests/Doroti.Tests -c Debug`: CupertinoTabScaffold의 두 탭을 pointer로 전환하고 controller/page 상태를 확인한다. Hit test를 IgnorePointer로 막는 `--break-tab`은 의도한 실패다. 정상 경로 및 지속 timer bounded settle, 연속 tester 생성·해제 PASS.
- 타이머는 TimeProvider를 통해 결정적으로 전진한다. pumpAndSettle은 pending timer도 기다리므로 cursor blink도 취소하지 않으면 timeout이다. timeout 진단은 frame/timer/scheduled 상태와 위젯 트리를 포함한다. 일반 framework error는 pump 실패로 보고한다.
- 한글 `ㅎ → 한 → 한글` 합성 composing packet의 취소·selection 보존과 pointer focus, TextField unmount 후 native text-client 해제 PASS. 물리 IME 및 후보창 좌표를 시험한 것은 아니다.
- 새 teardown은 root unmount → focus/image/semantics 정리 → binding별 static/scheduler/input 참조 해제 순서다. FocusManager는 dispose 뒤 남은 microtask를 무시한다. Windows host는 GPU preflight 이후 framework를 capability보다 먼저 정리한다.
- 외부 PackageReference-only 소비 앱에서도 Testing/Cupertino 회귀 PASS. aggregate 테스트 원시 빌드는 실행 폴더에 두고 요약 후 자동 삭제했으며 상시 테스트는 제품 solution에 넣지 않았다. 초기 직접 실행의 tests/Doroti.Tests/bin·obj와 수동 확인 산출물은 최종 삭제가 자동 승인 정책에 차단되어 M0의 정리 보류 목록에 남겼다.
- 당시 잔여였던 ListView·VariableBlur geometry는 [04의 CPU 회귀](04-rendering-lifetime.md), Dialog 중간 프레임과 독립 owner-thread 격리는 아래 2026-09-29 결과로 보강했다. 현재 남은 범위는 GPU golden·GPU/texture/native 복수 owner 상세 수명·플랫폼별 키/semantics/물리 입력이다. CPU 이미지 API를 golden 승인으로 표시하지 않으며 같은 thread의 nested tester는 계속 거절한다.
- 다음: [02 Desktop](02-desktop-contract.md), [03 입력](03-input-accessibility-platformview.md)의 미검증 범위를 계속 검증한다.

## 2026-09-29 추가 검증

기존 회귀를 재실행하고 다음을 최소 상시 suite에 통합했다.

후속 보강: 입력·semantics·reassemble·픽셀/PNG API도 실행 전 owner thread/disposed 검사를
수행한다. 다른 thread 및 dispose 뒤 9개 작업을 거절하는 회귀 PASS.
같은 thread nested 금지와 서로 다른 owner thread의 독립 실행을 구분한다.

- 실제 pointer로 Cupertino Dialog를 열어 60 ms 중간 fade 값, 뒤쪽 버튼의 hit-test 차단,
  닫기 pointer와 route 제거를 확인했다(**CPU PASS**, GPU golden 아님).
- 기존 Router/RestorationManager의 대기·중복·state checkpoint 및 한글 bucket 재생성 회귀 **PASS**.
- owner thread가 다른 두 WidgetTester를 동시 실행해 binding·pointer focus·서로 다른 한글 값·
  custom semantics action·해제를 확인했다(**CPU PASS**). 동시 dispose에서 드러난 공유
  default action listener 경쟁은 dispatcher별 action 인스턴스로 수정했다.
- 같은 thread의 nested tester를 거절하고 pump/dispose에서 owner thread를 확인한다.
  앱 callback은 각 dispatcher scope를 사용하며 테스트도 owner thread에서 수행한다.

전체 GPU golden·실제 물리 입력·모든 native 자원 격리를 완료한 결과는 아니다.


## 2026-09-29 Linux / Qt 후속

Linux native Skia asset을 공통 CPU 테스트에 연결하고 widget/rendering/navigation/owner 격리 회귀를 실행했다. CPU PASS와 Qt native/GPU 자동 실행 증거는 구분한다.

상세 명령·환경·지원 경계: [Linux Qt 결과](../results/2026-09-29-linux-qt.md).


## 2026-09-29 macOS/AppKit 후속

macOS에서 공통 CPU 회귀와 owner 격리를 재실행했다. native editor/WKWebView·실제 두 Metal 창의 수명은 별도 native smoke로 확인했다. GPU golden·물리 입력 전체 qualification은 남는다.
[구현·명령·결과·잔여](../results/2026-09-29-macos-appkit.md)를 따른다.
