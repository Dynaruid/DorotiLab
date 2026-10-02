# 네이티브 C 단일화 실행 결과 — 2026-10-02

**C 단일화 및 동적 texture 예산 구현 완료. 수행 가능한 자동 검증 PASS, 전체 인수 PARTIAL.**
Apple build/Metal/device는 기존 사용자 요청에 따라 SKIPPED다. 물리 입력,
실제 표시 FPS/overlap, 10분 사용 및 clean-machine/서명 인수는 별도로 남는다.

모든 native host에서 A/B 실행 옵션과 `NativeFrameLoopOptions`를 제거했다.
일반 framework 준비는 GPU admission 앞에서 수행하고, shader-only 최대 2와
native/shield·replay·resize의 내부 직렬 전환을 같은 C 정책으로 처리한다.
obsolete 옵션을 무시해 C로 바꾸는 방식 대신 시작 시 명확히 거절한다.
소스/API 변경은 [migration](../../../../Doroti/docs/migrations/native-frame-c-only.md)에 기록했다.

Qt Quick의 고정 128MiB R/P guard는 [동적 정책](../../../../Doroti/docs/dynamic-texture-budget.md)으로
교체했다. 실제 Vulkan image requirement와 heap별 owned bytes를 사용하고,
지원 시 driver budget/usage를 조회한다. heap/host capacity 10%, 보고된 free
headroom 20%를 남기며 allowance를 수요에 맞춰 늘린다. 호환되는 device-local
heap의 여유를 확인하고, 실제 할당 회수 후 allowance를 줄인다. 압력이나
budget 축소로 GPU/Qt 소비 중인 이미지의 수명을 단축하지 않는다.

| 검증 | 결과 | 범위와 한계 |
| --- | --- | --- |
| CPU | 최종 `validate.py Build` PASS | C-only 설정 거절, full queue 준비, callback coalescing/native 전환, 역순 terminal 및 동적 budget pressure/shrink/recovery/overflow |
| Radeon Vulkan 픽셀 | 31조건 C sequential/overlapped byte-exact PASS | 기존 후보의 31 red/green SHA도 모두 일치. 실제 host native 합성 전체 픽셀 행렬은 별도 |
| 실제 GPU 동적 예산 | 두 미완료 bank, **170,888,800 bytes** 실제 R/P 할당, **213,647,360 bytes** peak allowance PASS | resize 후 축소, 종료 allocated/budget **0/0**, 마지막 소비자 **4/4**. hardware 실행 overlap/scanout 계측은 아님 |
| Windows | Sample2 두 host Release 및 Testbed 두 host Debug build PASS | AppSDK native 상태·입력 view 재생성·2종 창 lifetime 5종 PASS. MAUI 창 상태·종료 PASS; 기존 native 입력 adapter 미완료 유지 |
| Windows C 회귀 | 두 host 각각 이전/새 C의 5조건 교차 smoke **10회**, explicit C PASS | queue/admission max≤2, 표시 성능으로 환산하지 않음 |
| 설정 거절 | Windows AppSDK/MAUI·Linux Qt·Android 각각 7종 PASS | A/B/unknown 및 4 legacy selector. CPU는 빈 값·명시 C·legacy 충돌도 확인 |
| Linux Qt Quick | 올바른 `DorotiLinuxDesktop=true` Testbed에서 multi/desktop/input/20 resize PASS | 실제 Qt 소비자 제출/완료 일치, 종료 reserved/retiring 0. WSL/llvmpipe software Vulkan |
| Linux 대체 backend | Vulkan-window Off/Fast20, OpenGL/xcb Off C smoke PASS | 변경 없는 기존 native backend에 새 Qt host/Rendering DLL을 적용한 호환성 검사. Quick pending 카운터 0을 실제 2-slot 실증으로 사용하지 않음 |
| Android | Sample2/Testbed 최종 Release x64 APK build 및 Sample2 설치·explicit C Fast20 PASS | Mono **full AOT**, profile AOT=false, trimmed. CoreCLR JIT 또는 arm64 physical 결과가 아님 |
| Android lifecycle/native | C 단일화 후보의 3회 지속 복귀 **28→30, 54→78, 95→101**, process/surface 재생성 및 Editor/WebView 4회 생성 PASS | 각 복귀 5초 후 비교. 뒤의 동적 예산은 Qt 전용 적용이며 최종 APK에도 source를 재빌드하고 C smoke를 재확인 |
| Windows packages/template | **0.3.0-beta.rc.20261002.conly.dynamic** package-only template restore/publish/native consumer PASS | 로컬 unsigned candidate. 앞선 `.conly` 후보는 별도 identity로 보존 |
| Linux packages/template | 새 isolated package feed의 Release template restore/publish/run·20 resize·설치/업데이트/제거 PASS | 프로젝트 참조 없이 Quick/C 기본값. 다른 profile의 no-build와 trimming 거절; local OS 인수 |
| Source/tools | 최종 Source/runner/installer contracts, Python compile 및 diff whitespace PASS | Apple 코드 참조·공통 API 정합성은 확인; Apple 실제 build/runtime SKIPPED |

Source/payload/패키지 SHA, 실제 단계별 JSON과 실패는
[추적되는 집계 JSON](2026-10-02-native-frame-c-only.json)에 보존한다.
raw는 `temp/testing/native-frame-c-only/`다. 큰 frame history는 집계에서 제외하고
원본 JSON에 남겼다. successful CPU/Source suite의 자체 raw 디렉터리는 기존
도구 정책에 따라 삭제됐으며, 별도 wrapper log와 PASS 결과만 보존했다.
`Doroti/artifacts/release/0.3.0-beta.rc.20261002.conly.dynamic/`는 삭제 가능한
로컬 산출물이며 그 경로가 미래에도 남아 있다고 보장하지 않는다.

실패/재시도도 보존했다. MAUI는 stdout 대신 GUI exception 파일, Android는
cache 파일 대신 해당 실행의 crash log로 설정 거절을 확인해야 했다.
Linux 첫 native 검사는 Desktop 옵션이 빠져 legacy 2560×1800/DPR2에서
128MiB guard에 걸렸고, 작은 배율 및 첫 dynamic 재시도도 Desktop probe가
없어 timeout이었다. 올바른 옵션으로 재빌드한 최종 native 검사는 통과했다.
일반 bin의 오래된 Linux 바이너리는 Qt consumer 계약 이전 버전이므로
before-C 회귀 기준으로 사용하지 않았다. 첫 GPU dynamic 정책의 half-heap
상한이 256MiB heap을 불필요하게 128MiB로 제한한 실패도 남겼으며, 최종
driver headroom 정책과 128MiB 초과 GPU 검사로 확인했다.

기존 Windows MAUI native 입력 adapter, OpenGL Wayland compatibility,
physical GPU/IME/Orca, 실제 표시 및 정량 30회 성능, 플랫폼별 10분 사용은
이 작업의 통과 결과로 대체하지 않는다. emulator 결과는 실제 Android 기기
인수가 아니다. [work5의 기존 결과](2026-10-02-native-frame-pipeline.md)와
[work6 후속 작업 요약](../../../26-10-02/work6-summary.md)의 증거·미완료 인수를 유지한다.
