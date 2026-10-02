# 네이티브 공통 C 경로 작업 요약 — work5.md

원문 작성일: **2026-10-02** · 후속 반영: **2026-10-02~03** · 보관일: **2026-10-03**
대상: iOS / Mac Catalyst / macOS / Android / Windows App SDK·MAUI / Linux Qt

**공통 C 구현·실행 가능한 자동 검증 완료, 전체 인수 PARTIAL**을 보존한다. 10월 2일 Apple 실행은 사용자 요청으로 **SKIPPED**, 10월 3일 Apple/AppKit 검토는 **새 후보의 별도 자동 검증**이다. 과거 iOS 성능을 새 payload에 소급하지 않는다. 이후 A/B 제거·동적 texture 예산은 [work6 요약](work6-summary.md)에 있다. 보관 작업 자체는 제품 재실행이 아니다.

## 1. 목표와 공통 계약

[iOS C/Fast 채택](work4-summary.md)을 모든 native 제품 호스트·샘플·템플릿으로 확장했다. 기존 GPU backend/device/queue와 **fastGaussian + adaptiveResolution=true + resolutionScale=0.25** 기본값·sigma/DPR·캡처·품질을 유지했다. Web은 범위에서 제외했다.

공통 순서는 owner thread의 입력/lifecycle → immutable viewport/context 게시 → framework callback 최대 한 번 → 실제 scene/native 상태 조회 → GPU admission/타깃 확보 → 제출 → 플랫폼별 completion이다. GPU full에서도 framework 준비 기회를 유지하고 요청과 최신 pending scene을 각각 하나로 합친다.

fresh shader-only는 논리 GPU 프레임 **최대 2개·비동기 표시**, replay/native/shield·resize/rotation은 drain 후 직렬 처리다. callback과 raster-only 재시도를 구분하며 generation 변경을 다시 검사한다. 논리 상한 2는 front texture·DXGI buffer·Qt bank 수를 무조건 2로 줄인다는 뜻이 아니다.

recording/Surface/snapshot/drawable/native lease/shared texture는 **마지막 GPU 소비자 완료**까지 유지한다. Vulkan producer 완료만으로 Windows D3D12 copy나 Qt sampling 자원을 재사용하지 않는다. GPU 완료·owner callback·실제 표시·native 합성을 구분하며 실패/timeout으로 미완료 자원을 성급하게 회수하지 않는다.

## 2. 플랫폼별 연결과 2026-10-02 실행

[공통 실행 결과](../26-10-03/works/results/2026-10-02-native-frame-pipeline.md)의 플랫폼 JSON·source/payload identity를 기준으로 한다.

| 대상 | 구현·기록된 검증 | 잔여·제한 |
| --- | --- | --- |
| 공통 | NativeFramePipeline admission·descriptor·CPU 계약, 실제 Radeon Vulkan 31조건 A/C 픽셀·두 recording PASS | 전체 frame timing·실제 표시/입력 계측 별도 |
| iOS/Catalyst | 기존 iOS C 유지·공통 준비 연결·Catalyst 전처리 경계 밖으로 연결 | 당시 Apple build/Metal/lifecycle SKIPPED |
| macOS/AppKit | drawable/GPU 앞 준비, shader 상한 3→2, 실제 native/layout에 따른 표시 | 당시 Apple 실행 SKIPPED, 후속 새 후보 별도 |
| Android | Choreographer 준비·비차단 acquire·completion retry·native drain·surface retirement hold | x64/arm64 Release build, emulator A/B/C·native 재생성·3회 복귀 PASS; physical GPU/입력 별도 |
| Windows App SDK | native scheduler/managed 준비·Vulkan producer/D3D12 consumer 수명·비동기 retry | Release C/A/B·native/resize/reset/recovery PASS; physical DPI/IME·표시 별도 |
| Windows MAUI | 별도 DXGI 준비/admission·exact resize/ACK·embedded composition 경계 | Release shader/정책 smoke PASS; native 입력 adapter 미완료 |
| Linux Qt Quick | producer/copy와 Qt consumer fence·publication·retirement·준비 token·ABI 6 | consumer drain PASS, 당시 software Vulkan; physical GPU/표시 별도 |
| Linux 대체 경로 | Vulkan-window·OpenGL/xcb 별도 smoke·capability 구분 | OpenGL Wayland 실패 보존, 기본 Quick 결과로 대체하지 않음 |

공통 CPU 회귀는 GPU full 준비·coalescing·native 전환·replay·generation·역순/중복 완료·실패를 다뤘다. 정상 프레임의 대기를 completion 조회/retry로 대체하되 native/resize/종료에 필요한 동기화는 유지했다.

## 3. 정확성·성능·실사용 인수

당시 계획의 A/C 기본 비교는 Off/Fast σ20·32/Adaptive σ20·32 × 두 정책 × 3회, 구성당 30회였다. 60Hz 목표는 ≥58 FPS, p95 ≤17.5ms, p99 ≤34.2ms, >25ms ≤2%; 고주사율은 실제 H/T 기준으로 판정한다. 이미 주기에 가까운 조건은 추가 10% FPS 증가를 요구하지 않는다.

**실제 표시 계측 권한 부족으로 정량 30회·hardware overlap·물리 입력 지연은 미완료**다. queue/admission·render/present 호출 수를 표시 FPS로 사용하지 않는다. 물리 IME·native 합성 전체·장비별 lifecycle·10분 사용·서명/clean OS는 별도 인수이며, 구현과 자동 검사 완료만으로 PARTIAL을 해제하지 않는다.

당시 명시 A/B와 legacy 옵션 유지·같은 바이너리 A/C 요구는 **work6의 C 단일화 이후 변경 전/후 C 회귀로 대체**했다. native/replay/resize의 C 내부 직렬 처리는 유지하며 과거 A/B/C 측정 자료는 당시 의미 그대로 보존한다.

## 4. Linux Qt 구성 후속 검토

[Qt 구성 결과](../26-10-03/works/results/2026-10-02-linux-qt-configuration-review.md)와 [JSON](../26-10-03/works/results/2026-10-02-linux-qt-configuration-review.json)에 이전 payload와 구분한 새 identity를 남겼다.

- SDK·샘플·템플릿·CMake를 **Quick/Graphite 기본값**으로 맞췄다. WebEngine은 SDK/템플릿 선택 사항, 샘플 기본 활성화다.
- native build cache를 Configuration·Quick/Graphite/WebEngine/GStreamer 조합별로 분리했다. 다른 옵션의 `publish --no-build`가 캐시를 대신 사용하는 일을 거절한다.
- 누락 native 라이브러리/잘못된 boolean/미지원 조합은 초기 오류로 처리하고 누락 산출물은 **DOROTIQT006**으로 실패한다. prebuilt는 같은 옵션·ABI가 필요하다.
- `DorotiQtGraphite=false`를 CMake에 연결하고 OpenGL의 Quick/WebEngine 비활성화·runtime 조건을 문서화했다. 정상 producer/copy/Qt consumer fence retirement, native/resize/종료 동기화와 **ABI 6 / 208바이트**를 유지했다.

Ubuntu 26.04·SDK 10.0.400/runtime 10.0.11·Qt 6.10.2·Wayland/KDE·llvmpipe에서 Debug/Release build, CPU·31조건 GPU 픽셀·두 recording, Wayland smoke/20회 resize, DPR 2.25, xcb 입력/resize, 무설정 C 및 당시 A/B/C, OpenGL/xcb serial, NuGet-only 템플릿 publish/portable 설치를 통과했다. C pending 최대 2·종료 consumer 전량 완료를 확인했다.

정적 확대 배율은 동적 DPR/실제 모니터 이동 인수가 아니며 xcb smoke는 Qt WSI 경쟁 조건 해결의 증거가 아니다. OpenGL Wayland·물리 GPU/IME/Orca·실제 표시·10분 사용·clean 배포는 남았다.

## 5. Apple·AppKit 후속 — 2026-10-03

[Apple 새 후보 결과](../26-10-03/works/results/2026-10-03-apple-frame-configuration-review.md)는 생략했던 Apple 검증을 별도 수행하고 Catalyst 조건·UIKit/AppKit 재시도·AppKit deferred resize를 보강한 기록이다. [AppKit 별도 결과](../26-10-03/works/results/2026-10-03-appkit-configuration-review.md)는 window별 Metal retirement/재연결·background 진단·detach key/focus·backing factor·숨김/복귀와 Graphite/Ganesh native 검사를 다룬다.

두 후속은 새 후보의 자동 검증 PASS와 합성 fixture 한계를 구분한다. 10월 2일 SKIPPED를 소급 수정하거나 물리 입력/실제 모니터 이동/정량 성능/10분 사용·배포 인수를 통과 처리하지 않는다. 상세 공통화 후속은 [work6 요약](work6-summary.md)에 보관했다.

## 6. Linux Qt 핫리로드

[Hot Reload 결과](../26-10-03/works/results/2026-10-02-linux-qt-hot-reload.md)와 [JSON](../26-10-03/works/results/2026-10-02-linux-qt-hot-reload.json)에 CLI/설치 VSIX의 실제 metadata update·같은 PID/State/카운터/한글 텍스트/스크롤 유지·컴파일 오류 복구·rude edit·명시 Restart·Stop PASS를 기록했다.

Linux `describe`/`dev`·VS Code target을 연결하고 `DorotiQtDevelopment=true`, 비최적화 Debug·portable 심볼·startup hook을 사용한다. Release/AOT/trim/single-file 개발 실행은 거절한다. inotify 인스턴스 제한 128의 watcher 실패를 재현해 미설정 때 `DOTNET_USE_POLLING_FILE_WATCHER=1`을 적용하고 명시 환경은 보존했다.

검증 범위는 소스 템플릿·repository ProjectReference의 Quick/Graphite Debug다. NuGet-only Hot Reload·다중 창/대체 renderer·native C++/QML/assets·물리 입력/표시로 확대하지 않는다. 당시 `doroti-local.doroti@0.1.0` 설치와 workspace CLI/.NET 설정을 기록했으며 현재 도구 상태를 새로 확인한 사실은 아니다.

재현: `pwsh -NoProfile -File Doroti/eng/doroti.ps1 dev -App samples/DorotiSampleApp2 -Platform linux`. [개발 계약](../../Doroti/docs/development-hot-reload.md)을 따른다.

## 7. 증거 보존 경계

보관된 실행 결과·플랫폼 JSON에 identity·성공/실패·재시도·제한을 보존한다. `temp/testing/native-frame-pipeline/`, `linux-qt-config/`, `linux-hot-reload/` raw와 로컬 artifacts의 현재 존재는 보장하지 않는다. 과거 iOS·기존 Qt payload·후속 C 단일화 payload를 각각 구분하며 **전체 인수 PARTIAL**은 유지한다.
