# 플랫폼별 키보드 단축키 검토 및 수정

후속 사용자 재현에서 공통 Clipboard와 호스트 연결 누락을 발견했다. 아래 키 전달 검사 PASS가 실제 Services 클립보드 연결을 증명하지는 않는다. 원인·수정·실제 Windows clipboard 검증은 [후속 기록](clipboard-host-and-empty-field-menu.md)에 남긴다.

## 수정 범위

| 경로 | 확인한 결함과 수정 |
| --- | --- |
| Web | Ctrl/Shift 논리 코드 역전 수정. 네이티브 textarea가 편집을 맡는 동안에도 수정키는 framework에 전달해 Shift+Tab을 구분한다. key-up은 key-down 당시의 논리 키를 사용하고 character는 비운다. |
| Linux Qt | Ctrl/Shift 역전 수정. Ctrl+문자의 제어문자 또는 빈 key-up text 대신 Qt의 문자 키 ID를 사용한다. 눌린 논리 키를 보존하고 focus loss에서 합성 key-up을 보낸다. |
| Android / MAUI | Android Ctrl/Shift/Alt/Meta의 좌우 HID 매핑 누락을 보완했다. 공통 MAUI 변환 함수를 독립된 `MauiKeyMap`으로 분리해 제품 코드 자체를 테스트한다. native EditText가 활성 클라이언트를 편집하는 기존 소유권은 유지한다. |
| iOS / Mac Catalyst | 공통 MAUI의 Ctrl/Shift 역전 수정. UIKit modifier의 HID fallback과 방향키의 private-use 문자 처리를 복구한다. key-up에 character를 보내지 않는다. 활성 Entry/Editor의 first responder 소유권은 유지한다. |
| native macOS | `FlagsChanged`를 구현해 수정키 누름/해제를 전달한다. pinned Flutter 표로 AppKit 가상키를 HID로 변환하고, 방향키를 일반 문자로 오인하지 않도록 한다. 좌우 수정키를 각각 처리하고, key-up 논리 키를 보존하며 창 비활성화 시 눌린 키를 해제한다. |
| MAUI Windows | 공통 Ctrl/Shift 역전과 key-up character 수정. 스캔코드·확장 비트 및 좌우 VirtualKey로 오른쪽 수정키도 구분한다. |
| 공통 Widgets | 앱 트리에 `WidgetsApp.defaultActions` 전체를 설치한다. 기존에는 스크롤 액션만 설치돼 Tab/Shift+Tab 등 기본 액션이 누락됐다. ReadingOrderTraversalPolicy의 비어 있는 정렬 후보를 실제 노드로 채우고 null 최상위 그룹 키를 허용한다. PrioritizedAction/RequestFocusAction의 정상 실행 뒤 예외도 제거한다. |

네이티브 텍스트 endpoint를 가진 플랫폼은 편집/IME/클립보드를 해당 endpoint가 처리하고 framework에는 결과를 전달하는 기존 경계를 유지한다. 모든 플랫폼에서 실제 기기 입력을 확인했다는 의미는 아니다.

수정키 이벤트 근거: [Apple NSResponder.flagsChanged](https://developer.apple.com/documentation/appkit/nsresponder/flagschanged%28with%3A%29), [Android KeyEvent](https://developer.android.com/reference/android/view/KeyEvent). 숫자 ID와 AppKit HID 표는 저장소의 pinned Flutter `keyboard_key.cs` / `keyboard_maps.cs` 및 `KeyCodeMap_Internal.h`와 대조했다.

## 실패 재현과 회귀 검사

- 수정 전 cross-platform 계약 검사 **FAIL**: Web ControlLeft가 framework ShiftLeft ID로 변환됨. `.doroti/shortcut-platform-before.log`.
- 초기 Chromium 실행: 실제 Ctrl+A/C/X/V와 교체는 통과했으나 Shift+Tab **FAIL**. `.doroti/shortcut-browser-runtime.log`.
- 기본 액션 설치 후에도 traversal **FAIL**. 추적에서 `PreviousFocusAction`까지 도달한 다음 ReadingOrderTraversalPolicy의 빈 목록에서 `NoElements` 예외가 발생하는 것을 확인했다. 임시 추적 코드는 최종 소스에서 제거했다.
- 최종 호스트 키 변환 → KeyEventManager → HardwareKeyboard → FocusManager → ShortcutManager → 실제 EditableText 액션 맵 / controller 검사 **PASS**. Windows 좌우 Ctrl, Web/Qt/Android Ctrl, UIKit/AppKit Command의 A/C/X/V와 modifier identity를 포함한다. 클립보드 endpoint와 렌더링 부수효과는 이 검사에서 대역을 사용한다.
- 실제 rect를 제공하는 focus node들로 읽기 순서 정렬, 최상위 null 그룹의 Tab/Shift+Tab 이동, 기본 앱 액션 설치, PrioritizedAction/RequestFocusAction 정상 반환을 검증했다. `.doroti/shortcut-platform-traversal-contract.log`.
- Qt adapter의 Ctrl+문자 down/up, Shift를 먼저 놓은 뒤의 문자 키 해제, focus loss 합성 key-up **PASS**. `.doroti/shortcut-qt-contract.log`.
- 전체 FCR-7 Material/Widget 회귀 검사 **PASS**. `.doroti/shortcut-platform-complete-material.log`.

## 플랫폼 검증 결과

모든 테스트/빌드/서버 프로세스에 1,200초 제한을 사용했다.

| 대상 | 결과 | 근거 / 한계 |
| --- | --- | --- |
| Web host + DemoApp | **build PASS** | `.doroti/shortcut-web-build.log`, `.doroti/shortcut-web-complete-build.log` |
| Chromium, renderer auto | **runtime PASS** | 실제 Control+A/C/X/V, clipboard 읽기/쓰기, 전체선택 후 교체, Shift+Tab으로 이탈 후 Tab으로 복귀. `.doroti/shortcut-browser-complete-runtime.log`: 1 passed, 6.8s |
| Qt managed host | **build + adapter PASS** | `.doroti/shortcut-qt-build.log`, `.doroti/shortcut-qt-contract.log`. 실제 Linux/Qt GUI는 notVerified |
| MAUI Android | **build PASS** | net10.0-android / android-x64, `.doroti/shortcut-android-build.log`. 기기/IME/물리 키보드는 notVerified |
| MAUI Windows | **build PASS** | `.doroti/shortcut-maui-windows-build.log`. 실제 WinUI 편집은 notVerified |
| native macOS | **build PASS** | net10.0-macos / osx-arm64, `.doroti/shortcut-macos-build.log`. Apple 기기 실행·native link/package는 notVerified |
| iOS | **build PASS** | net10.0-ios / iossimulator-x64, `.doroti/shortcut-ios-build.log`. 시뮬레이터/기기 실행·native link/package는 notVerified |
| Mac Catalyst | **build PASS** | net10.0-maccatalyst / maccatalyst-arm64, `.doroti/shortcut-catalyst-build.log`. 기기 실행·native link/package는 notVerified |
| WindowsAppSdk DemoApp | **build PASS** | 공통 Widgets 변경 포함, `.doroti/shortcut-platform-complete-windows.log`. 실제 Windows 조작은 이번 검사에 포함하지 않음 |

테스트용 Web 서버는 검증 후 종료한다. 기존 컨텍스트 메뉴 변경과 이전 Windows 단축키 실패 이력은 보존한다.
