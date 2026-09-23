# Linux Qt work3 후속 실행 — 2026-09-24

**PARTIAL.** [전날 실행 결과](work3-results-2026-09-23.md)를 이어서 처리했다. `work3.md`의 46개 작업 문장 중 27개를 수행으로 표시했지만, 어느 QT 항목도 전체 완료 기준까지 승인하지 않았다. 이 결과는 현재 checkout의 HEAD `44c61567`과 후속 작업 트리 변경을 기준으로 한다.

## 이번에 추가한 작업과 확인

| 범위 | 결과 | 근거 |
| --- | --- | --- |
| QT-03 | scan-code-zero 합성 키에서 서로 다른 문장부호와 알 수 없는 Qt 키가 같은 physical 값으로 합쳐지던 fallback을 수정했다. 비영문 logical 문자, 문장부호 위치, repeat 중 logical 변경과 release/focus-loss를 managed 계약으로 확인했다. | `QtKeyMap.cs`, `Program.cs`; `dotnet run --project Doroti/validation/linux-qt-contract/Contract.csproj` exit 0 |
| QT-05 | selected/checked/toggled/expanded/focus, 텍스트·caret, 변경 알림의 Doroti→Qt 대응표와 기존 제한을 제품 계약에 기록했다. | `docs/platform-views/linux-qt.md` |
| QT-07 | Release, Wayland, Qt 6.10.2, llvmpipe VM의 720×640·DPR 1에서 0/1/4 WebView × idle/animation/scroll/modal 12조합을 동일 app/shim/driver로 실행했고 모두 정상 종료했다. 표본 수·p50/p95/p99·PSS·WebEngine 프로세스 수·CPU·host GPU timing을 저장했다. | [원본 측정 JSON](results-2026-09-24/release-workloads.json) |
| QT-08 | `check-runtime.py`로 publish shim/직접 linker 의존성, QPA plugin, QML module 및 필수 plugin, WebEngine helper·pak·locale를 실행 전에 개별 확인한다. package-only 소비자 실행 앞에 연결했다. 완전한 로컬 설치에서 통과하고 host, QPA, QML, helper, resource, locale 누락 주입을 각각 탐지했다. `qtpaths6`가 없는 runtime-only 경로도 linked Qt6Core 위치로 통과했다. native include/link 요구를 확인해 Quick의 직접 Qt OpenGL·OpenGLWidgets 개발 의존성을 제거하고 ON/ON, ON/OFF, OFF/OFF native CMake 빌드를 통과했다. xcb-only build는 현 Wayland backdrop 소스와 추가 검증 비용 때문에 제공하지 않기로 기록했다. | [사전 점검 결과](results-2026-09-24/runtime-preflight.json), [runtime-only fallback](results-2026-09-24/runtime-preflight-without-qtpaths.json), [native 의존성 표](results-2026-09-24/native-dependency-matrix.json) |
| QT-09 | WebView 생성 중 native 예외가 ABI status 70으로 반환되고 항목이 게시되지 않는 독립 계약을 추가했다. Release shim에 연결한 `webview-contract` exit 0. | `webview-contract.cpp` |
| QT-10 | 변경 후 source/template native 21개 바이트 비교 통과. 의존성 변경으로 새로 빌드한 Quick/WebEngine shim에 연결한 native 및 WebView 계약도 exit 0. | `check-native-sync.py`, `native-contract`, `webview-contract` exit 0 |

Release idle 모드의 CPU 관측치는 0/1/4 WebView에서 각각 **1.88% / 1.44% / 1.22%**이고, `frameSwapped` interval 표본 수는 모두 0이다. 따라서 idle interval 백분위는 null이다. Release의 이 수치를 전날 Debug 기준선과 직접 전후 비교하지 않는다. 4 WebView modal 실행에서 PSS 조회 실패 표본이 1개 있었으므로 해당 peak는 관측 가능한 프로세스 합계다. 이 VM의 `frameSwapped`는 scanout 또는 입력 지연 측정이 아니며, 합의된 성능 예산도 아직 없다.

## 열린 완료 기준

- QT-01: native 전체 경로에서 주입한 GPU 실패의 terminal 횟수와 외부 timeout hang 시험, 실물 device loss.
- QT-02: Qt 단독으로 재현되는 XWayland WSI VUID, Wayland minimize/restore, 고배율 전체 좌표, 다중 모니터, Graphite depth 경고 재확인.
- QT-03/04: 물리 Wayland/xcb 키보드와 한글 IME, 전체 framework→Quick→WebView→framework Tab/Shift+Tab 및 modal/hidden 복귀.
- QT-05/06: Orca, WebView 접근성 하위 트리 연결과 문자 위치, mixed-target touch, 물리 touch/tablet 및 GestureArena 중재.
- QT-07/08: 실물 GPU 성능·예산, clean VM의 두 배포 방식과 실제 로드 경로·hash, 더 넓은 지원 배포판/Qt 조합.
- QT-09/10: 구형 shim과 향후 비호환 ABI의 독립 release 계약, native 소스 구조 분리와 compositor/GPU CI.

누락 파일 사전 점검은 파일 존재와 `ldd`의 직접 의존성만 보장한다. Qt plugin 로드 성공, QML 실행, graphics driver 및 compositor 동작은 실제 제품 실행으로 계속 확인해야 한다. 전날 기록한 xcb/XWayland validation VUID와 DPR 1.5/2 전체 시나리오 실패는 이 실행에서 해결되지 않았다.
[새 Quick shim의 Wayland 사전 점검](results-2026-09-24/new-shim-preflight.json)과 [Release 빌드의 xcb 사전 점검](results-2026-09-24/release-xcb-preflight.json)도 통과했지만, xcb 제품의 WSI 오류를 해결했다는 뜻은 아니다.

QT-02의 추가 원인 조사, Qt 단독 재현과 native Wayland 우회 결과는 [WSI 후속 조사](wsi-investigation-2026-09-24.md)에 기록했다.
