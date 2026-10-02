# Apple frame configuration review — 2026-10-03

대상: [work5](../../../26-10-02/work5-summary.md)·[work6](../../../26-10-02/work6-summary.md)의 iOS / Mac Catalyst / macOS 구성.

상태: **보강 구현·실행 가능한 Apple 자동 검증 PASS, 전체 인수 PARTIAL.** 이전 후보의 Apple SKIPPED 이력은 보존하고 이번 요청의 새 후보를 별도로 검증했다. 물리 입력·정량 성능·10분 사용의 미완료 상태는 유지한다.

## 확인한 누락과 보강

1. **Catalyst 빌드 실패:** iOS에서만 사용하는 `_pipelineDisplayLink`·`_replayRequested` 필드가 Catalyst에서도 선언되어 `TreatWarningsAsErrors`에서 CS0169 두 건이 발생했다. 선언을 `IOS && !MACCATALYST`로 제한했다. 첫 실패 로그는 `temp/testing/apple-frame-review/catalyst/build.log`에 보존했다.
2. **Apple raster retry:** GPU 완료와 drawable 부족의 재시도가 일반 framework 준비로 실행될 수 있었다. [MauiFrameWakeQueue](../../../../Doroti/src/Doroti.Host.Maui/MauiFrameWakeQueue.cs)를 UIKit/Catalyst·AppKit에 연결했다. 재시도만 있으면 준비한 장면을 재사용하고 새 framework 요청이 함께 있으면 우선 처리한다. 새 generation·layout·활성화의 준비 요청을 보존하며 재진입 draw가 대기 요청을 소모하지 않게 했다.
3. **AppKit deferred resize:** `_drawingLayout`만 resize 사유로 사용하면 layout이 GPU full로 거절된 뒤 후속 draw에서 직렬 경계가 사라졌다. 새 surface generation의 실제 제출까지 resize gate를 유지한다. 이전 작업이 남아 있으면 새 크기의 fresh 장면도 먼저 drain한다.
4. **공통 Apple 진단:** 세 Apple target의 snapshot에 `NativeFramePipeline`을 제공한다. C·준비·pending/최댓값·완료·거절·fallback을 기록하고 terminal completion과 drawable 표시를 구분한다. 상세 event history는 기존 profile opt-in을 유지한다.
5. **수집 도구 정합성:** iOS collector의 기존 단위 테스트가 삭제된 A/C 인자를 계속 호출하여 TypeError가 발생했다. C 조건 교차와 중간 반복 재개로 바꿨다. 새 wake queue·Apple effect 파일을 source manifest에 추가하고, Apple 앱의 MonoBundle·dylib·확장자 없는 framework·plist/resources를 payload hash에 포함한다. Python 3.9에서도 hash 계산이 동작한다.
6. **Metal 수명 검사:** 기존 Vulkan fixture와 구분되는 [Metal fixture](../../../../Doroti/tests/Doroti.Tests/NativeFrameMetalGpuRegression.cs)를 추가했다. 제품 `SkiaGraphiteSession`과 renderer를 사용하고 별도 wait/readback은 fixture에서만 수행한다.
7. **Metal 취소 fence:** `AppleGpuEffects`가 실패한 marker의 NSError 유무로 완료를 판단하던 조건을 고쳤다. 상태가 `Completed`일 때만 취소된 segment의 회수를 허용한다. 오류 객체가 없는 실패와 timeout에서도 fence와 frame lease를 보존한다. 실패한 Metal marker의 실제 주입은 별도 미검증이다.
8. **iOS simulator 실행 중단:** 실제 실행에서 `CAMetalDrawable addPresentedHandler:` 미지원으로 Objective-C 예외가 발생했다. optional drawable observation·GPU timestamp의 selector 지원을 runtime에 확인한다. 미지원 환경은 GPU completion으로 정상 회수하고 실제 표시 계측은 제공하지 않는다. 실패 로그는 `ios-verified.log`에 보존한다.
9. **Apple 설정 거절 probe:** AppKit은 builder의 명시적 `ArgumentException`이 발생해도 native event loop가 남을 수 있어 기존 exit-only probe가 timeout됐다. `--apple-receipt`로 초기화 예외 receipt·렌더 증거 없음·cleanup 전 exit 상태를 확인한다. probe가 종료한 상태를 자발적인 실패 exit로 보고하지 않는다.
10. **iOS profile의 UI thread 위반:** 별도 evidence writer가 `OwnerIsActive`를 통해 `UIView.Window`를 읽어 `UIKitThreadAccessException`이 발생했다. UI lifecycle/pulse에서 갱신한 volatile 활성 상태를 snapshot에서 읽도록 변경했다. 무거운 event history 복사·JSON 출력은 background writer에 남긴다.

보강 중 optional API helper의 namespace 누락과 protocol wrapper 타입 가정도 수정했다. `INativeObject` handle의 실제 `respondsToSelector:`로 조회하며, Mac의 기존 표시 통보가 계속 들어오는 것도 확인한다. 실패한 helper 빌드·AppKit 관측 assertion은 raw에 보존했다. 다중 창 probe는 모든 창을 닫은 뒤 공유 evidence 파일이 다른 view의 초기 snapshot으로 남을 수 있어, 종료 후 공유 snapshot으로 GPU 진행을 판정하지 않는다. C 진행/상한은 살아 있는 input/rotation probe에서 별도로 확인한다.

## 수행한 검사

| 검사 | 결과 | 범위 |
| --- | --- | --- |
| 공통 CPU C 계약·wake coalescing·dynamic budget | PASS | 준비 중 재요청, GPU full, raster wake, native/replay·generation·역순 완료 |
| 실제 Apple M1 / Graphite-Metal | PASS, 31조건 | Off / Full·Adaptive·Fast·Fixed σ0/1/2/4/8/20/32 / Kawase σ20/32; sequential C와 두 retained submission의 픽셀 일치·장면 격리·third admission 거절·역순 회수·종료 outstanding=0 |
| Apple MSBuild profile evaluation | PASS | iOS device/simulator Debug·Release Mono·NativeAOT 선택과 템플릿 parity; AppKit/Catalyst 일반·development 구성·잘못된 override 거절 |
| iOS 호스트 Debug 컴파일 | PASS | `net10.0-ios27.0`, `iossimulator-arm64`, 경고·오류 0 |
| Python collector 회귀 | PASS, 9 tests | C-only 반복 순서·표시 집계·raster-only latency·Apple bundle 변경 감지 |
| macOS / AppKit Debug | PASS, 경고·오류 0 | 최종 후보의 명시 C·24회 resize·정확한 extent/DPR·표시 통보 유지; 이전 단계 후보의 native input·desktop 상태 검사도 PASS |
| macOS / Ganesh compatibility | PASS | 24회 resize, C 내부 single-frame 제한 확인. 보강 도중 후보의 별도 실행 결과로 구분 |
| Mac Catalyst Debug | PASS, 경고·오류 0 | 최종 후보의 profile-on native input·다중 창 검사; input에서 최대 2, GPU full 준비 1회·완료 진행 확인 |
| iOS simulator Debug | PASS, 경고·오류 0 | 최종 후보의 profile-on input·좌/우 landscape/portrait rotation, 중간 extent·최종 pixels/safe area·display-link 정지 확인; 최대 관측 pending은 1 |
| Apple desktop 제거 설정 | PASS, 각 7조건 | 최종 macOS/Catalyst payload에서 A/B/unknown·legacy 4종 초기화 거절, 정상 렌더 증거 없음 |

도구: macOS 26.6 arm64, .NET SDK 10.0.401, Xcode 27.0 (27A266a), Apple workload 27.0.10722, MAUI 10.0.90, SkiaSharp 4.154.0-preview.1.26454.9. 모든 검사 명령은 `.github/copilot-instructions.md`의 1,200초 wrapper를 사용했다.

Metal 검사는 두 논리 프레임을 owner completion 전까지 유지한다. GPU queue를 강제로 막지 않으므로 두 GPU 작업의 실제 실행 overlap·표시 FPS·native 합성·입력 지연의 통과 증거로 사용하지 않는다. 같은 renderer/backend의 sequential C와 retained C pixels를 비교했다.

앱 빌드는 `net10.0-macos27.0` / `net10.0-maccatalyst27.0` / `net10.0-ios27.0`, 각각 `osx-arm64` / `maccatalyst-arm64` / `iossimulator-arm64`의 명시 Xcode 27 구성이다. iOS simulator는 Mono interpreter 구성이고 성능용 rotation phase-error assertion은 켜지 않았다. 기능 회전 PASS를 실제 iPhone의 표시 동기화·정량 성능 PASS로 확대하지 않는다. 최종 AppKit snapshot은 최대 GPU 2·완료 67·pending 0·표시 통보 41을 기록했다. 스냅샷의 짧은 표시 통보 수를 FPS로 환산하지 않았다.

## 증거와 제한

보존할 집계·source/payload identity는 [JSON](2026-10-03-apple-frame-configuration-review.json)에 기록한다. 원시 로그·플랫폼별 probe·GPU JSON은 `temp/testing/apple-frame-review/`에 있으며 삭제 가능한 로컬 산출물이다. 삭제 뒤 raw가 존재한다고 보고하지 않는다.

이번 보강은 C 정책·Apple 컴파일/준비/직렬 resize·진단/검사 정합성에 관한 것이다. Sample2의 실제 iPhone Release/NativeAOT 성능·3회 지속 복귀, 물리 IME·selection·여러 모니터 DPR·10분 사용, package-only 새 앱·서명/공증/clean OS 배포는 별도 미검증 항목이다. Qt의 동적 texture 예산 결과를 Metal 전체 메모리 예산 검증으로 확대하지 않는다.
