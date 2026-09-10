# MediaQuery · SafeArea 전체 플랫폼 정비 완료 요약

보관일: 2026-09-10. 원본: 루트 `work.md`. **상태: 완료 — 사용자 요청에 따라 작업을 완료로 정리하고 활성 계획을 종료했다.**

MQ-0~MQ-9의 구현·검증 결과와 유지할 설계를 요약했다. 당시 실행한 검증의 범위와 미실행 환경은 아래에 사실대로 보존한다. 이번 보관 작업에서 테스트를 다시 실행하거나 기기 상태를 변경하지 않았다.

## 1. 확정된 설계

**MAUI는 가능한 전체 native content 영역을 제공하고 raw geometry·가림 정보를 공급한다. SafeArea 소비와 keyboard 회피는 Doroti가 담당한다.**

- `ViewMetrics`는 실제 렌더 view 원점 기준 physical pixels를 사용하며, MediaQuery 경계에서 DPR로 한 번만 변환한다.
- `viewPadding`은 시스템 edge, `viewInsets`는 IME 등 가림, `systemGestureInsets`는 시스템 제스처 영역으로 구분한다.
- derived padding은 각 변에서 `max(0, viewPadding - viewInsets)`로 계산한다. 키보드가 열려도 원래 viewPadding은 유지한다.
- geometry는 view별, 환경 설정은 dispatcher의 snapshot으로 관리한다. 여러 view의 설정·inset을 혼합하지 않는다.
- metrics generation, resize epoch, surface generation을 구분한다. inset-only 변경 때문에 GPU surface를 재생성하지 않으며 stale event와 동일값의 불필요한 notification을 차단한다.
- MAUI Page/Grid/semantics overlay의 자동 inset 소비를 해제하되 native keyboard/safe-area 관측은 유지한다. 플랫폼이 실제 viewport를 줄인 경우 키보드 높이를 중복 차감하지 않는다.
- 기존 SafeArea/SliverSafeArea 배치와 Scaffold의 size-only 최적화, parent override, 입력·caret·semantics의 view 좌표 계약을 유지한다.

비교에 사용한 Flutter pin은 `56b8e1a851a594b1a154f8ea93270807dab22b9a`다. 제품의 의도적인 차이와 플랫폼별 fallback은 지원 문서와 coverage에 명시한다.

## 2. 완료한 구현

| 영역 | 결과 |
|---|---|
| MQ-0 기준·coverage | 30 MediaQuery field/aspect × 9 host/device-category의 source API, 최소 버전, 초기값, 변경 이벤트, fallback·검증 상태 정리 |
| MQ-1 공통 view 계약 | derived padding, 불변 display-feature snapshot, DPR/geometry 검증, stale metrics 거부, 동일값 알림 억제, view/dispatcher 설정 분리, immutable native text scaler |
| MQ-2 framework | `fromView`의 단일 metrics 캡처, locale/24h/accessibility 전달, parent override 및 aspect별 갱신 연결 |
| MQ-3 MAUI·Android | render observer attach/detach, 자동 소비 해제, typed/legacy insets와 IME animation, fold/cutout/corners, native 글자 배율·환경 설정 |
| MQ-4 UIKit | iOS/iPadOS/Catalyst의 keyboard/safe-area/accessibility 공급과 UIKit view 수명 연결 |
| MQ-5 AppKit | window/backing scale/accessibility 관측과 native macOS metrics 공급 |
| MQ-6 Windows | Windows App SDK와 MAUI의 InputPane/settings 공급, native numeric ABI 2 및 adapter 갱신 |
| MQ-7 Linux Qt | keyboard·scale·환경 설정 공급, Qt numeric ABI 3, Qt 6.9 safe area/6.10 contrast version guard |
| MQ-8 Web | root-local geometry, CSS safe area, VirtualKeyboard/visualViewport fallback, media-query 설정과 worker protocol 4 전달 |
| MQ-9 제품·검증 도구 | 전용 진단 화면, mounted aspect/SafeArea/SliverSafeArea 검사, native ABI·DOM fixture, package/consumer 검증 및 지원 문서 |

호환성 경계는 Windows ABI **2 / metrics 192 bytes**, Qt ABI **3 / metrics 160 bytes**, Web protocol **4**다. managed/native 및 worker 구성 요소를 함께 배포하며 이전 형식은 handshake에서 거부한다. export/header 파일 이름의 기존 v1/v2 표기와 numeric ABI 버전을 혼동하지 않는다.

## 3. 기록된 검증 결과

| 검증 | 결과와 범위 |
|---|---|
| 공통 계약 | **1,074개 계약 통과**. 전체 mounted MediaQuery aspect, parent override, multi-view, stale/동일 metrics, fractional DPR, copy/remove, SafeArea 16 flags/minimum/nesting, SliverSafeArea reverse/RTL, observer 수명 |
| Framework 회귀 | Scaffold body 높이·IME 이후 focused caret reveal, 기존 size-only 최적화, full FCR-7 Material 및 resize 계약 통과 |
| Host·소비자·패키지 | MAUI 각 TFM/RID와 Web/Windows App SDK/Qt host 및 공유 Testbed build, Windows/Web/Android x64 소비자와 관련 package/template 검사. Apple 항목의 당시 검증은 reference SDK 대상 managed cross-build |
| Native ABI | Windows MSVC/SDK 10.0.26100.0 및 Ubuntu WSL Qt 6.10.2 빌드, size/offset·managed adapter 검사 통과 |
| Android 실제 실행 | API 33 x86_64 에뮬레이터, NVIDIA Graphite/Vulkan. drawable/native view 1920×1200, DPR 1.5, IME physical `0→627→0` / logical `0→418→0`, bottom viewPadding 90px 유지, surface generation 유지. Scaffold 회피를 꺼도 MAUI 추가 resize/pan 없음, caret 표시 확인 |
| Windows 실제 실행 | Windows App SDK 및 MAUI Windows의 Graphite/Vulkan 초기 기동, metrics·프레임과 정상 종료 확인 |
| Web 실제 실행 | 설치된 Chrome 152.0.7977.83, AMD WebGPU/WebGL2 양쪽 진단 화면 표시, 900×700 resize, reduced-motion 반영, 설정 변경 중 resize/surface generation 유지, page error 0 |

각 테스트 실행에는 1200초 제한을 적용했다. 자세한 명령·exit code·로그·화면은 [검증 인덱스](../../Doroti/validation/evidence/media-query-safe-area/README.md)에 있다.

## 4. 실행 중 발견해 수정한 문제

- `LocalizationsResolver`의 초기 해석과 observer 등록 누락, nullable locale-list 처리로 Android 초기 MaterialApp이 실패하던 문제.
- C# 기본 인수 dispatch 차이로 `ViewportOffset`을 통한 `ScrollPosition.moveTo`에 null이 들어가 keyboard 이후 caret 스크롤이 누락되던 문제.
- `SystemTextScaler.Equals`의 조기 cast로 `noScaling` 비교 분기에 도달하지 못하던 문제.
- custom HWND에서 `AccessibilitySettings.HighContrastChanged` 등록이 실패하던 문제. desktop Win32 contrast 조회와 설정 broadcast 경로로 처리.
- MAUI Windows metrics worker가 XAML 속성을 읽던 문제. native 환경 snapshot을 XAML thread에서 수집하도록 분리.
- `Ui.Rect`의 computed alias를 그대로 노출해 Web JSON 대소문자 무시 metadata가 충돌하던 문제. 전용 wire DTO 도입.
- RID-less MAUI outer restore와 inner build의 assets 경로 차이로 WindowJava 참조가 소비자에서 누락되던 문제. 공통 restore 경로와 TFM별 output으로 정리.

## 5. 보존하는 검증 범위와 지원 한계

작업 상태는 완료로 정리한다. 다만 당시 실행하지 않은 환경의 측정 결과를 새로 만들어 PASS로 바꾸지는 않는다.

- Android 실기기, API 24–29 fallback, API 34 비선형 scaling, foldable/cutout 및 추가 navigation/IME 조합은 해당 실행 기록에서 미검증이다.
- Apple native linking/signing·실기기와 전체 UIKit scene/keyboard animation, Windows touch keyboard·다중 DPI, Qt 6.5 및 Wayland/X11 실제 UI는 당시 evidence 범위 밖이다.
- Web 실행 증거는 desktop full-page Chrome의 두 renderer다. mobile Safari/Chromium, desktop Safari/Firefox, embedded 제품 조합 전체를 검증한 기록은 아니다.
- floating/split keyboard처럼 4변 inset으로 표현할 수 없는 가림을 전체 폭 bottom inset으로 부풀리지 않는다. Web embedded root의 keyboard fallback, AppKit/구버전 Qt의 공급 API 부재 등은 문서화한 기본값을 사용한다.
- `unsupported`, `notVerified`, 실제 측정된 zero는 서로 다른 상태로 coverage에 유지한다. 이전 실패 로그·오래된 APK evidence와 최종 성공 산출물을 혼동하지 않는다.

이후 NativeAOT 등 별도 작업에서는 이 문서의 확정 설계를 회귀 기준으로 사용한다. 별도 작업에서 추가한 실기기 증거는 그 보고서의 범위로 기록한다.

## 6. 문서·증거와 재현

- [사용법·플랫폼 지원·ABI 정책](../../Doroti/docs/media-query-safe-area.md)
- [전체 검증 인덱스와 재현 절차](../../Doroti/validation/evidence/media-query-safe-area/README.md)
- [전체 field × host coverage](../../Doroti/validation/evidence/media-query-safe-area/coverage.json)
- [공통 계약 결과](../../Doroti/validation/evidence/media-query-safe-area/contracts.json), [build 결과](../../Doroti/validation/evidence/media-query-safe-area/builds.json), [소비자·패키지 결과](../../Doroti/validation/evidence/media-query-safe-area/products.json)
- [Flutter pin·소스 provenance](../../Doroti/validation/evidence/media-query-safe-area/source-provenance.json)
- [Android 최종 실행](../../Doroti/validation/evidence/media-query-safe-area/android-final.json), [Windows native 실행](../../Doroti/validation/evidence/media-query-safe-area/windows-native-smoke.json), [MAUI Windows 실행](../../Doroti/validation/evidence/media-query-safe-area/maui-windows-smoke.json), [브라우저 실행](../../Doroti/validation/evidence/media-query-safe-area/browser-product.json)

저장소 루트에서 기존 검증을 실행하는 명령:

```powershell
python Doroti/validation/media-query-safe-area/run-validation.py all
python Doroti/validation/media-query-safe-area/windows-smoke.py native
python Doroti/validation/media-query-safe-area/windows-smoke.py maui
```

진단 화면은 `DOROTI_TESTBED_MODE=media-query`로 선택한다. Android Activity extra는 `doroti_testbed_mode=media-query`, Web query는 `dorotiTestbedMode=media-query`다. raw/logical/consumed metrics, SafeArea·Scaffold 회피 옵션, SliverSafeArea, 하단/다중행 입력, dialog/bottom sheet를 관찰할 수 있다.
