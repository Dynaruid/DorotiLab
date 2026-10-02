# 네이티브 C 경로 단일화·동적 texture 예산 요약 — work6.md

원문 작성일: **2026-10-02** · 후속 반영: **2026-10-03** · 보관일: **2026-10-03**
선행 작업: [work5 공통 C 경로 요약](work5-summary.md)

**제품의 A/B 옵션·분기 제거와 동적 texture 예산 구현, 수행 가능한 자동 검증 완료. 전체 인수 PARTIAL**을 보존한다. 10월 2일 Apple 실행 SKIPPED와 10월 3일 새 Apple/AppKit 후보 자동 PASS는 서로 다른 payload의 결과다. 이번 보관 작업은 제품 재실행이나 미완료 인수의 통과 처리가 아니다.

## 1. 유일한 C 정책과 보존 계약

[NativeFramePipeline](../../Doroti/src/Doroti.Skia.Rendering/NativeFramePipeline.cs)을 UIKit/Catalyst·AppKit·Android·Windows App SDK/MAUI·Linux Qt의 유일한 프레임 정책으로 정리했다. Web은 제외하며 기존 backend/device/queue·VariableBlur Fast 기본값/품질을 유지한다.

- 정상 pulse에서 immutable viewport/context를 게시하고 GPU 슬롯/drawable/acquire 전에 framework를 준비한다. 요청과 최신 pending scene은 각각 하나로 합친다.
- fresh shader-only는 지원 capability에서 논리 GPU 프레임 최대 **2개·비동기 표시**를 사용한다. GPU retry는 준비된 장면을 재사용한다.
- native/shield·pending native·replay·resize/rotation은 진행 작업을 drain하고 필요한 직렬 admission/표시 동기화를 쓴다. 이는 **C 내부 전환**이며 A/B 모드가 아니다.
- Ganesh/OpenGL·embedded composition 등 capability 제한 경로는 admission 1을 유지한다. 두 프레임 수명 지원으로 보고하지 않는다.
- recording/snapshot/drawable/native lease/shared texture는 마지막 GPU 소비자 완료 또는 확인된 context loss까지 보존한다. 제출/present 성공·timeout은 재사용 허가가 아니다. Windows resize ACK·Qt consumer retirement·Android surface hold를 유지한다.

## 2. 설정·API 전환

| 입력 | 전환 후 동작 |
| --- | --- |
| `DOROTI_NATIVE_FRAME_MODE` 미설정·빈 값 또는 `C` | C 실행 |
| 같은 키의 `A`, `B`, 그 밖의 값 | GPU 자원 생성 전 구성 오류 |
| `DOROTI_VARIABLE_BLUR_SERIAL_FRAMES`, `DOROTI_VARIABLE_BLUR_PIPELINE`, `DOROTI_NATIVE_PRESENTATION`, `DOROTI_IOS_SHADER_PRESENTATION`의 비어 있지 않은 값 | 과거 C 선택 값도 포함해 제거된 설정 오류 |

Android는 환경과 Intent extra를 모두 검사한다. 명시 C가 legacy 충돌을 숨기지 않는다. `NativeFrameLoopOptions`, resolver·mode bool admission 인자, 불필요한 iOS 래퍼·Compile 링크와 host/collector A/B 실행 분기를 제거했다. 초기 설정 검사만 남기고 hot path의 환경 조회를 없앴다.

이는 **소스·바이너리 API 호환 변경**이다. 소비 호스트/패키지를 함께 rebuild하고 [migration 안내](../../Doroti/docs/migrations/native-frame-c-only.md)를 따른다. 과거 결과의 A/B 문자열과 native/resize/종료에 필요한 저수준 동기화는 유지한다.

## 3. 검사·수집·패키지 전환과 실행 상태

[C 단일화 실행 결과](../26-10-03/works/results/2026-10-02-native-frame-c-only.md)에 이전/새 source·payload hash, 실패·재시도·구성 거절·capability를 기록했다.

- **F0–F4:** 변경 전 Windows 두 host의 Release C payload 보존, 공통 API·모든 host·샘플/템플릿·CLI/VS Code·migration 정리 완료. Windows 교차 C smoke와 Windows/Linux package-only 템플릿 PASS.
- **F3 GPU:** 비교를 A/C에서 **한 scene마다 drain한 C 기준 vs 두 미완료 C recording**으로 변경했다. Off/Full/Adaptive/Fast/Fixed/Kawase 31조건·red/green snapshot·third 거절·역순 완료·consumer drain을 유지했다.
- **F3 수집기:** A/B 정책·serial/presentation 선택 주입을 제거하고 C 조건 교차·negative 설정 probe·lifecycle 복귀 3회/5초 후 지속 갱신으로 전환했다. 기본 성능 비교는 **변경 전 C/변경 후 C × 5조건 × 3회 = 30회**이며 baseline 부재는 after-only로 표시한다.
- **F5:** 공통 CPU·Radeon Vulkan 31조건, Windows native/lifetime, Android x64 APK/C/native/복귀, Linux Quick·지원 대체 backend 자동 검증 PASS. Apple 당시 실행은 사용자 요청 SKIPPED.
- **후보:** Windows 최종 후보 `0.3.0-beta.rc.20261002.conly.dynamic`, Linux는 별도 새 격리 Qt feed·portable install을 검증했다. 기존 후보에 후속 검증을 소급하지 않는다.

collector 체크 완료는 before/after 수집 기능과 queue smoke의 구현을 뜻한다. 표시 계측 없는 실행은 **실제 FPS·hardware overlap·입력 지연 notMeasured**다. 최종 정량 30회·물리 GPU/IME·10분 사용·서명/clean OS, Windows MAUI native 입력 adapter·OpenGL Wayland 실패는 별도 인수로 남았다.

## 4. 동적 texture 예산 — F6

Qt Quick R/P allocator의 고정 **128MiB guard**를 [DynamicTextureBudget](../../Doroti/src/Doroti.Skia.Rendering/DynamicTextureBudget.cs)으로 대체했다. 범위는 Qt allocator이며 다른 backend 전역 적용을 뜻하지 않는다.

- 화면 크기/DPR·segment·동시 프레임·published/retiring 이미지의 실제 Vulkan allocation requirement를 수요로 사용한다.
- `VK_EXT_memory_budget`의 heap별 budget/usage를 사용하고 미지원/무효 보고는 heap/host capacity 추정으로 구분한다.
- demand에 여유를 더해 allowance를 늘리고 실제 회수 후 줄인다. capacity 10%·driver 잔여 공간 20%의 여유를 유지하며 allocation 성공을 보장하지 않는다.
- compatible device-local memory type 중 headroom이 있는 heap을 선택한다. 부족하면 required/owned/limit/source를 기록해 거절하며 live GPU/Qt 할당을 budget 압력만으로 해제하지 않는다.
- CPU growth/pressure/shrink/recovery/overflow, 실제 GPU **128MiB 초과 두 미완료 bank**·resize 축소·종료 전량 회수, 올바른 Linux Desktop native/input/**20회 resize**를 통과했다.

current/peak allowance·heap owned/limit/source·거절 요청을 진단에 남겼다. 이전 고정 guard 실패·잘못된 검사용 구성도 보존했다. [동적 예산 계약](../../Doroti/docs/dynamic-texture-budget.md)을 따르며 새 코드를 포함한 최종 후보를 별도 생성했다.

## 5. Apple 구성 후속 — 2026-10-03

[Apple 후속 결과](../26-10-03/works/results/2026-10-03-apple-frame-configuration-review.md)는 새 후보 identity와 build/native 실행·실패/재시도를 기록한다.

- Catalyst의 iOS 전용 필드 전처리 누락/CS0169를 수정했다. UIKit/AppKit에서 GPU/drawable 재시도와 framework pulse를 구분해 이미 준비한 scene을 재사용하고, 새 요청/generation/layout은 새 준비를 요구한다.
- AppKit deferred resize의 새 generation 제출까지 직렬 gate를 유지했다. Metal effect marker의 실제 `Completed`를 확인하고 실패/timeout의 resource hold를 유지했다.
- simulator의 미지원 `addPresentedHandler:`와 optional GPU timestamp selector를 runtime 지원 검사로 처리했다. background snapshot은 UI 스레드에서 게시한 활성 상태를 사용한다.
- 공통 snapshot에 C/준비 횟수/pending/완료/거절을 기록했다. 실제 Graphite-Metal **31조건 sequential C vs 두 retained async** 픽셀·snapshot·GPU full 준비·third 거절·역순 회수·drain PASS.
- iOS collector를 C 조건 교차/재개 검사로 갱신하고 payload hash에 managed assembly/dylib/framework/plist/resources를 포함해 변경 감지 회귀를 통과했다.

자동 검증 PASS를 10월 2일 SKIPPED에 소급하지 않는다. 물리 iPhone Sample2/배포 runtime·정량 30회·물리 IME·10분 사용·서명 배포는 별도 상태다.

## 6. AppKit 별도 후속 — 2026-10-03

[AppKit 결과](../26-10-03/works/results/2026-10-03-appkit-configuration-review.md)에 Graphite/Ganesh 검사와 새 identity를 별도로 보관했다.

- 같은 surface의 이전 Metal retirement만 재연결을 막도록 제한하고 다른 window/owner 연결은 허용한다. 회수 중 view는 native handle과 무관한 reference identity로 유지한다.
- owner-thread attach/detach·기존 연결 보호, detach 시 합성 key-up/focus 해제, device identity cache·allocation/resource lock·GPU 완료 후 drawable 회수를 보강했다.
- layout/frame/pointer에 window backing factor를 일관되게 쓰고 hidden/minimize/detach 때 GPU 제출을 멈추면서 pending wake·첫 ReadyToShow 준비를 유지한다. Show/복원은 lifecycle/frame 요청을 재개한다.
- 실제 Metal terminal의 같은 owner 재연결 거절/다른 owner 연결/drain, background/retired snapshot·숨김/복귀 3회·detach/reattach를 자동 검사했다. 합성 키·1/1.5/2 배율은 물리 입력·실제 모니터 이동의 증거가 아니다.

정량 성능·물리 IME·모니터 이동·10분 사용·배포 인수는 **PARTIAL**로 유지한다.

## 7. 보존 자료와 전체 인수 경계

구현 완료는 A/B 실행 선택/분기 부재, C 준비·bounded admission·내부 직렬 전환·마지막 소비자 수명, 설정 거절·수행 가능한 회귀·API migration을 제공했다는 뜻이다. 플랫폼별 실제 GPU/native 합성/lifecycle/표시 성능/물리 입력·장기 사용의 전체 인수와 구분한다.

raw는 `temp/testing/native-frame-c-only/<platform>/<run>/` 등에 기록했으나 현재 존재를 보장하지 않는다. 추적되는 결과·JSON·migration을 보존 근거로 사용한다. 원문의 검사에는 **1,200초 timeout**을 적용했고, 이번 보관은 문서 링크·경로·삭제 범위만 새로 확인한다.
