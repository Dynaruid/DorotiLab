# Linux Qt 개선 작업 요약

원본: 삭제한 `work3.md`(검토 2026-09-23, 후속 09-24). **전체 PARTIAL: 작업 문장 46개 중 27개 수행, QT-01–10 어느 항목도 전체 완료 기준 승인 없음.** 체크박스의 수행과 항목 전체 수용을 구분한다.

기본 경로는 Qt Quick + Graphite/Vulkan + 선택 WebEngine이다. 검토 시 native/template 20개 파일이 일치했고 후속 검사 범위는 21개가 되었다. 실행 환경은 Ubuntu 26.04.1 VM, .NET SDK 10.0.400, Qt 6.10.2, KWin Wayland와 xcb/XWayland, Mesa llvmpipe 26.0.8이다. 물리 Vulkan GPU 환경이 아니다. 상세 기준선은 [09-23 실행](../../Doroti/validation/linux-qt-quick/work3-results-2026-09-23.md)(`f655bf1121d59a4739707769d97fe02535e46ffb` + 당시 변경)과 [09-24 후속](../../Doroti/validation/linux-qt-quick/work3-results-2026-09-24.md)(`44c61567` + 당시 변경)을 따른다.

## 수행 내용과 남은 완료 기준

| 항목 | 기록된 구현·검증 | 남은 범위 |
| --- | --- | --- |
| QT-01 P0 GPU 오류·종료 | terminal 상태·첫 실패·대기 시간/operation/owner/token 기록, 새 frame 차단, 반복 dispose; drain 불명확 시 process 종료까지 자원 격리, Qt device/instance 소유권 유지; managed timeout/device-loss 주입·10회 lifecycle/bank 계약 | native 전체 GPU 실패의 terminal 1회, 외부 timeout hang 실험, 실물 device loss |
| QT-02 P0 resize·DPR | Qt/QPA/크기/DPR/generation/token 진단, validation layer가 로드된 Wayland 10회 resize, Qt 단독 XWayland WSI 재현 | XWayland VUID 미해결, Wayland minimize/restore, 고배율 전체 좌표·다중 모니터, depth 경고 재확인 |
| QT-03 P1 키 매핑 | physical XKB scan→USB HID와 logical Qt key 분리, 좌우 modifier/keypad, focus-loss 해제; 09-24 scan-code-zero fallback·문장부호·비영문 logical·repeat 계약 보강 | 물리 Wayland/xcb 키보드·비영문 layout·IME 기록 |
| QT-04 P1 포커스 | feature bit 19로 실제 Qt 활성화 요청, 관측된 activation만 framework 상태로 반영; 합성 한글 IME/native Tab fixture | framework→Quick→WebView→framework 전체 Tab/Shift+Tab, modal/hidden/disposed 복귀, 활성화 거절·물리 IME |
| QT-05 P1 접근성 | selected/checked/toggled/expanded·선택/caret 전달, text/editable 인터페이스·선택/편집 action, node별 변경 이벤트, 대응표·제한 문서화 | 문자 geometry, WebView 접근성 하위 트리 연결, Orca 실제 읽기. 일부 구현이 있어도 전체 작업 문장은 미완료 |
| QT-06 P1 입력 소유권 | committed paint order/shield 기반 capture, multi-button mouse·touch/tablet 합성 전달, 숨김/비활성화/grab 상실 취소 | mixed-target touch, 물리 touch/tablet; 부모 scroll GestureArena 중재는 unsupported |
| QT-07 P1 비용 계측 | recording/queue idle/copy submit/fence/native commit 분리 계측, 0/1/4 WebView workload, idle 재요청 제거와 stale startup 재요청 보강 | 합의한 성능 예산·실물 GPU·scan-out 지연; queue-wide drain/기본 render loop 변경은 수명 설계 이후 |
| QT-08 P1 배포 | 저장소 밖 23개 로컬 package만 쓰는 앱 build/publish/run, 두 디렉터리 배포 방식 로컬 실행, ON/ON·ON/OFF·OFF/OFF 및 잔여 의존 검사; 누락 preflight·Quick 직접 OpenGL/OpenGLWidgets 개발 의존 제거 | 깨끗한 VM의 두 배포 방식·실제 로드 hash, 지원 distro/Qt 행렬. xcb-only build 미제공, NativeAOT/trim/single-file 거부 유지 |
| QT-09 P2 ABI·오류 | required mask/unknown additive bit 규칙, version/size/함수 포인터 검증 유지, status/operation/owner 보존; 09-24 native exception status 70·미게시 계약 | 구형 shim·향후 비호환 ABI 독립 release 계약 |
| QT-10 P2 유지보수 | `.clang-format`, 21개 native/template byte 검사, `run-suite.py`, package 소비 runner, displayless CI | native 큰 파일 구조 분리, compositor/GPU CI |

원래 순서는 기준선·동기화 도구 → QT-01/02 안정성 → QT-03/04/05/06 사용성 → 측정 기반 QT-07 → QT-08 배포다. ABI·구조 정리는 독립 변경으로 진행한다. 기능 수정 시 native/template·계약·제품 재현을 함께 갱신하고 큰 format/refactor와 분리한다.

## 실패와 성능 근거

- **XWayland WSI:** `VUID-VkSwapchainCreateInfoKHR-pNext-07781`, Qt 요청 720×640과 surface 요구 757×677 불일치. Doroti/WebEngine 없는 Qt-only fixture에서도 재현했다. 09-24 xcb 제품 10회 통과는 간헐 실패 해결의 근거가 아니다. resize 지연 10/30/60 ms 실험도 실패해 제거했다. [WSI 조사](../../Doroti/validation/linux-qt-quick/wsi-investigation-2026-09-24.md)를 따른다.
- native Wayland QPA는 해당 VM에서 통과한 별도 경로이며 xcb 수정이 아니다. Qt 소유 swapchain을 Doroti가 덮어쓰거나 lifetime drain을 제거해 오류를 숨기지 않는다. exit 0이어도 VUID가 있으면 실패로 판정한다.
- **DPR:** 1/1.25 전체 제품 시나리오 통과. 1.5/2는 가상 화면 높이로 하단 control이 잘려 클릭 assertion 실패했다. 정상 종료·VUID 없음으로 전체 DPR 검증을 승인하지 않는다. Wayland minimize/restore도 PARTIAL이다.
- **09-23 Debug 전후:** 동일 Wayland/720×640/DPR 1/llvmpipe idle에서 0 WebView CPU 51.49%→1.12%, 4 WebView 76.35%→0.47%; swap interval 표본 299→0 / 280→0. PSS 184.8→193.2 MB / 402.2→408.6 MB는 짧은 VM 관측이며 메모리 회귀 결론이 아니다. 최초 수정 후 검은 startup 화면은 stale generation의 재요청으로 수정했고 실패 시도도 별도로 남겼다.
- **09-24 Release:** 같은 app/shim/driver로 0/1/4 WebView × idle/animation/scroll/modal 12조합 정상 종료. idle CPU 1.88%/1.44%/1.22%, interval 표본 0이므로 percentile은 null. 전날 Debug와 직접 전후 비교하지 않는다. 4 WebView modal PSS 읽기 실패 1개로 peak는 관측 가능한 프로세스 합계다. `frameSwapped`는 물리 표시·입력 지연이 아니며 성능 예산도 미합의다.
- **배포 preflight:** host/QPA/QML/WebEngine helper/pak/locale 누락 주입을 탐지하고 `qtpaths6` 없는 runtime-only 경로도 검사했다. 파일 존재·직접 linker 의존 검사로 Qt plugin/QML/driver/compositor 실행까지 보장하지 않는다.

## 근거와 재실행 범위

- [Quick 검증 README](../../Doroti/validation/linux-qt-quick/README.md), [managed 계약](../../Doroti/validation/linux-qt-contract/README.md), [Linux Qt 계약](../../Doroti/docs/platform-views/linux-qt.md), [Linux WebView 계약](../../Doroti/docs/platform-views/linux-webview.md)
- 09-24 보고서가 참조하는 `results-2026-09-24/release-workloads.json`, `runtime-preflight.json`, `native-dependency-matrix.json`은 보관 시 현재 checkout에 없었다. 위 수치는 추적되는 실행 보고서의 기록을 요약한 것이며 원본 JSON을 이번에 재확인한 결과가 아니다.

모든 테스트는 20분 timeout, shared `obj` 빌드 직렬 실행, lifecycle/resize는 우선 최대 10회, 실행별 새 증거 디렉터리와 실패 기록 보존을 따른다. Release driver와 shim을 같은 빌드로 맞추고 validation layer 실제 로드 여부를 확인한다. xcb/XWayland와 순수 X11, managed/합성 입력과 실물 장치, displayless CI와 GPU/접근성 검증을 구분한다. 미실행은 `notVerified`, 명시적 비지원은 `unsupported`, 실제 실패는 `failed`다.

여러 blur/임의 transform, QWidget·Quick 통합 확대, WebView 권한·파일·다운로드·fullscreen 정책, 전체 profile 삭제, 일반 remote messaging, 여러 완전한 제품 창, Linux NativeAOT는 별도 설계 범위다. 이번 보관에서는 제품 실행·성능·장치 검증을 새로 수행하지 않았다.
