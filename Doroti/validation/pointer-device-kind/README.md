# PointerDeviceKind 및 트랙패드 입력 검토

2026-09-14. 로컬 Flutter `reference/flutter-master`의 `56b8e1a851`과 비교했다.
장치 분류뿐 아니라 네이티브 입력에서 프레임워크 제스처 콜백까지 연결했다.

## 수정된 동작

| 영역 | 수정 |
| --- | --- |
| 공통 이벤트 | 16개 빈 생성자가 정상 기본값을 건너뛰던 오류 수정. 무인수 pan-zoom은 `trackpad`, scale은 1, down/move는 정상 접촉 상태로 초기화 |
| ScaleGestureRecognizer | pan-zoom 갱신 factory가 null 시작 이벤트를 넘겨 예외를 발생시키던 오류 수정. 실제 확대·회전 콜백으로 검증 |
| MAUI 패킷 | 누락된 pan/panDelta/scale/rotation 및 독립 device 식별자, orientation/tilt 전달. 제스처마다 새로운 pointer ID 사용 |
| Windows MAUI / WindowsAppSDK | 공통 `WindowsPrecisionTrackpad`가 DirectManipulation의 접촉 pan/scale, 시작/종료, 관성 취소를 전달. OS 관성 갱신은 프레임워크 관성과 중복 적용하지 않음 |
| Windows HWND touch/pen | 공통 `WindowsNativePointerInput`이 WM_POINTER의 독립 멀티터치, 펜/지우개, 압력·각도를 전달. 승격 WM_MOUSE는 중복 전달하지 않음. WinUI PointerPoint 경로의 종류 구분도 수정 |
| macOS AppKit | scroll/magnify/rotate phase를 하나의 pan-zoom 세션으로 합침. 물리 pan, 누적 배율 `2^magnification`, radians 회전 전달. OS momentum 억제 및 접촉에 의한 관성 취소 |
| iOS / Mac Catalyst | `UIKitTrackpadInput`으로 연속 스크롤, pinch/rotation, discrete wheel을 구분. `AllowedTouchTypes=[]`로 직접 터치 제스처와 중복하지 않음. Graphite와 fallback 공통 |
| Mac Catalyst 관성 | 호스트의 별도 `MauiScrollMomentum` 및 전용 테스트 제거. 관성은 프레임워크에 맡김 |
| Android | Flutter와 같이 SOURCE_MOUSE의 버튼 없는 down/move/up을 pan-zoom으로 변환. 일반 mouse/touch와 분리. Graphite와 fallback에서 동일 reducer 사용 |
| Android 분류 | fallback도 native touch/generic motion으로 tool kind, eraser, unknown, hover, 버튼·압력·각도 보존. API 26+ 시스템 scroll factor 적용 |
| iOS 분류 | Graphite 및 Metal fallback에서 stylus/indirect-pointer mouse 구분 보완 |
| Web | unknown을 mouse로 바꾸지 않음. ctrl-wheel pinch는 `exp(-deltaY/200)` scale 신호로 전달. macOS에서 실제 Control 키를 누른 wheel은 scroll로 유지. Worker payload에도 signal kind/scale 전달 |
| Linux Qt | TouchPad wheel 및 QNativeGesture의 pan/zoom/rotate를 pan-zoom으로 전달. OS momentum 중복 방지, 관성 취소 및 deactivation 종료. Testbed와 템플릿 동기화 |

Qt pointer ABI의 120-byte prefix는 유지하고 48-byte pan-zoom tail을 추가했다.
새 관리 코드는 `struct_size`를 보고 legacy 120-byte 송신자도 읽는다.
Qt가 scroll phase를 제공하지 않을 때만 100ms 입력 중단으로 종료를 추정한다.
이 시간 기준은 실제 장치 감각을 검증한 결과가 아니다.

`PointerDeviceKind` enum 순서, converter의 종류 보존/접촉 버튼 합성,
기본 dragDevices의 mouse 제외 및 종류별 slop 정책은 로컬 Flutter와 일치하여 유지했다.
Windows DirectManipulation과 Android의 해당 Flutter 경로는 회전을 제공하지 않으므로
rotation=0이다. Web도 OS/브라우저가 제공하지 않는 회전 이벤트를 만들어 내지 않는다.

## 검증 결과

모든 실행 명령에는 1,200초 제한을 적용했다. 로그 기준 경로는
`Doroti/artifacts/validation/`이다.

| 검증 | 결과 | 로그 |
| --- | --- | --- |
| 6종 enum, 생성자 기본값, converter/copy/transform, 장치 필터, pan-zoom, slop | PASS | `trackpad-contracts.log` |
| MAUI/Android reducer의 세션·delta·DPR·장치 식별·취소·분리 | PASS | 위 로그 |
| 실제 ScaleGestureRecognizer 및 VerticalDragGestureRecognizer 콜백 | PASS | 위 로그 |
| Windows 실제 COM 초기화/등록/해제 + 합성 DirectManipulation transform/phase | PASS | 위 로그 |
| Windows WM_POINTER 합성 레코드의 멀티터치·eraser·압력·종료 | PASS | 위 로그 |
| Web 실제 관리 ingress → PointerScaleEvent | PASS | 위 로그 |
| Chromium의 실제 DOM 핸들러 15건, wheel/pinch/물리 Control/Worker 전달 4건 | PASS | `trackpad-browser.log` |
| 실제 Qt 이벤트 핸들러에 wheel/native gesture 주입 15건 | PASS | `trackpad-qt-contract.log` |
| 기존 FCR-5 스크롤 회귀 | PASS | `trackpad-scroll-contracts.log` |
| 기존 FCR-7 mac-text-menu 회귀 및 새 trackpad 계약 | PASS | `trackpad-material-contracts.log` |
| Windows MAUI / WindowsAppSDK 관리 빌드 | PASS | `trackpad-windows-build.log`, `trackpad-windowsappsdk-build.log` |
| Android MAUI 빌드 | PASS | `trackpad-android-build.log` |
| iOS / Mac Catalyst / macOS 관리 코드 컴파일 | PASS | `trackpad-ios-build.log`, `trackpad-maccatalyst-build.log`, `trackpad-macos-build.log` |
| Qt 관리 코드 / WSL Ubuntu Qt 6.10.2 native Graphite 빌드 | PASS | `trackpad-qt-managed-build.log`, `trackpad-qt-build.log` |

실제 물리 트랙패드·터치스크린·펜·지우개 검증은 `notVerified`다.
Apple 결과는 Windows에서 수행한 관리 코드 컴파일이며 Apple 네이티브 링크/실행이 아니다.
Windows 입력 fixture는 숨겨진 HWND와 실제 COM 객체를 사용하지만 transform/접촉은 합성이다.
Qt 입력 fixture는 offscreen에서 실제 제품 입력 소스를 실행하며 화면 표시/GPU 검증이 아니다.
Web fixture는 실제 DOM/Worker 라우팅을 실행하지만 GPU admission과 managed callbacks를 대체한다.
따라서 구현 연결과 자동 검증은 통과했지만 전 플랫폼 물리 입력 qualification은 `PARTIAL`이다.

## 재실행

저장소 루트 PowerShell에서:

```powershell
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/pointer-device-kind/Doroti.Validation.PointerDeviceKind.csproj --artifacts-path Doroti/artifacts/validation/pointer-device-kind/build -v:q
python Doroti/validation/run-with-timeout.py node Doroti/validation/pointer-device-kind/browser.mjs
python Doroti/validation/run-with-timeout.py python Doroti/validation/pointer-device-kind/qt-run.py
```

Web 테스트는 기존 `web-playwright` Playwright 설치와 첫 명령의 Debug TypeScript 출력이 필요하다.
Qt 테스트는 WSL Ubuntu의 Qt 개발 패키지와 `pointer-device-kind/qt-build` native 빌드 산출물이 필요하다.
생성물은 artifacts에만 둔다.

## 비교 근거

로컬 Flutter의 `gestures/{events,converter,recognizer,scale}.dart`, `scroll_configuration.dart`,
`mouse_tracker.dart`, Web `pointer_binding.dart`, Android `AndroidTouchProcessor.java`,
iOS/macOS `FlutterViewController.mm`, Windows `flutter_window.cc` 및 `direct_manipulation.cc`,
Linux `fl_scrolling_manager.cc`를 비교했다.

API 연결은 [Qt native gesture](https://doc.qt.io/qt-6/qnativegestureevent.html),
[Qt wheel](https://doc.qt.io/qt-6/qwheelevent.html),
[Windows DirectManipulation](https://learn.microsoft.com/en-us/windows/win32/directmanipulation/direct-manipulation-portal)
및 로컬 Windows SDK `directmanipulation.h`/`WinUser.h`를 확인했다.
