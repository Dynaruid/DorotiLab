# 배경 터치 후 네이티브 텍스트 입력 포커스 유지

2026-10-04. TextField에 커서와 소프트 키보드가 남아 있는데 배경 터치 이후 키보드 붙여넣기가 전달되지 않는 문제를 수정했다. 프레임워크의 기본 `onTapOutside`/FocusNode 정책은 바꾸지 않았다.

## 원인과 변경

Android `DorotiAndroidVulkanView.OnTouchEvent`는 Down마다 렌더링 SurfaceView의 `RequestFocus()`를 호출했다. Doroti의 텍스트 연결은 유지되어도 숨겨진 MAUI Entry/Editor가 네이티브 포커스와 IME의 served view를 잃을 수 있었다. 접근성 트리를 임의로 수정할 필요 없이 이 포커스 이동을 막는다.

`MauiTextInputBridge.OwnsNativeFocus`는 활성 클라이언트, 수명 상태와 실제 플랫폼 입력 대상의 포커스를 확인한다. `HasClient`만으로 판단하지 않으므로 네이티브 PlatformView로 명시적으로 포커스를 넘긴 경우는 막지 않는다. 이 조회와 포커스 변경은 플랫폼 UI 스레드에서 수행한다. 배경 포인터 이벤트 자체는 계속 프레임워크에 전달하고, 텍스트/선택/조합 상태를 재설정하거나 키보드를 다시 열지 않는다.

| 경로 | 변경 또는 검토 결과 |
| --- | --- |
| Android Graphite | 활성 네이티브 입력창이 포커스를 가진 경우 SurfaceView의 Down 포커스 요청을 생략한다. |
| Android/iOS/Mac Catalyst MAUI | 공통 표면의 뷰 포커스 요청도 입력창 포커스를 보존한다. UI 디스패처에서 실제 소유자를 확인한다. 기존 Android PlatformView의 포커스 보존 조건도 유지한다. |
| Windows MAUI | raw HWND mouse, native pointer, XAML pointer, 뷰 활성화의 명시적 포커스 요청을 보호한다. 입력용 ContentControl/SwapChainPanel의 `AllowFocusOnInteraction=false`로 WinUI의 자동 포커스 이동도 방지한다. |
| macOS AppKit | 입력창/field editor가 first responder이면 렌더링 뷰의 자동 first responder 수락과 뷰 활성화 요청을 막는다. |
| Web | 기존 `focusActiveEndpoint`가 표시된 IME endpoint를 유지하고 포인터 기본 동작을 차단한다. 소스 검토, 변경 없음. |
| Qt / 독립 Windows App SDK | 프레임워크 텍스트 입력은 포인터를 받는 동일 네이티브 창에서 처리한다. MAUI의 별도 숨김 입력창을 빼앗는 구조와 다르다. 소스 검토, 변경 없음. |

관련 구현: [MauiTextInputBridge.cs](../../Doroti/src/Doroti.Host.Maui/MauiTextInputBridge.cs), [Android SurfaceView](../../Doroti/src/Doroti.Host.Maui/DorotiAndroidVulkanViewHandler.cs), [MAUI 표면](../../Doroti/src/Doroti.Host.Maui/MauiSkiaSurface.cs), [Windows MAUI](../../Doroti/src/Doroti.Host.Maui/DorotiWindowsDxgiSurface.cs), [AppKit 표면](../../Doroti/src/Doroti.Host.Maui/DorotiMacOSMetalView.cs).

Android의 IME 입력 대상과 포커스 계약은 [InputMethodManager 공식 문서](https://developer.android.com/reference/android/view/inputmethod/InputMethodManager), WinUI의 자동 포커스 설정은 [AllowFocusOnInteraction 공식 문서](https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.frameworkelement.allowfocusoninteraction), AppKit의 마우스 first responder 처리는 [Apple 이벤트 문서](https://developer.apple.com/library/archive/documentation/Cocoa/Conceptual/EventOverview/EventArchitecture/EventArchitecture.html)를 참고했다.

## 검증

CPU suite, Android/Windows 빌드, 최종 Android runner와 Windows 실행 명령은 `Doroti/eng/run-with-timeout.py --timeout 1200`으로 실행했다. 원시 로그/JSON은 `temp/testing/text-input-focus/2026-10-04/`에 둔다. 아래 핵심 결과는 원시 산출물 보존 여부와 무관하게 이 문서에 남긴다.

| 검증 | 결과 및 범위 |
| --- | --- |
| CPU Debug / Release | PASS. Android 환경에서 배경 touch 이후 프레임워크 포커스, 텍스트, 선택과 조합이 유지되고 후속 입력이 전달된다. 다른 필드로 전환 및 명시적 unfocus 시 기존 연결이 닫힌다. 기존 CPU 회귀 suite도 통과했다. |
| Android arm64 Debug APK | PASS. `EmbedAssembliesIntoApk=true`, 빌드 경고/오류 0. |
| Galaxy S25 / SM-S931N / Android 16 | PASS-scoped. 아래 대조군과 수정 후 결과를 실제 MAUI/SurfaceView/EditText/IMM에서 자동 검증했다. |
| Windows MAUI Debug | PASS-scoped. 빌드 경고/오류 0. 일반/여러 줄/암호 입력창 모두 네이티브 포커스 유지, 후속 편집 전달, yield/clear 이후 표면 포커스 재획득을 확인했다. 검증 앱 종료 코드 0. |
| Apple 실행/빌드 | SKIPPED. 이번 작업에서 Apple 런타임/빌드 검증은 하지 않았다. |
| 물리 터치 및 Samsung 키보드 클립보드 UI | notVerified. 실기기 자동 명령은 사람의 터치와 실제 키보드 클립보드 패널 조작을 대체하지 않는다. |
| Web / Qt / 독립 Windows App SDK | 소스 검토만 수행. 이번 작업의 런타임 검증 대상이 아니다. |

Galaxy 대조군은 Android SurfaceView의 Down 조건만 기존의 무조건 `RequestFocus()` 동작으로 실행했다. 배경 down/up 이후 다음 실패를 재현했다.

```text
Background touch stole the IME endpoint:
focused=False, active=False, surfaceFocused=True
```

수정 후 동일 실기기에서 일반 Entry와 여러 줄 Editor 모두 다음 결과를 얻었다.

```json
{"status":"PASS","cases":[
  {"type":"text","focused":true,"imeActive":true,"text":"seed한글 붙여넣기"},
  {"type":"multiline","focused":true,"imeActive":true,"text":"seed한글 붙여넣기"}
]}
```

자동 프로브는 같은 UI turn에서 synthetic InputConnection 조합 → 실제 SurfaceView touch 경로 → 조합 범위 보존 → commit을 확인한다. 다음 UI turn에서도 `EditText.IsFocused`와 `InputMethodManager.isActive(view)`가 유지되는지 확인하고, Android 네이티브 클립보드 paste를 실행해 텍스트/선택이 브리지로 전달되는지 검사한다. `ClearClient` 이후 입력창 포커스 해제와 표면 포커스 재획득도 확인한다. 프로브는 종료 시 기존 클립보드를 복원한다. 검증 PID의 AndroidRuntime/mono-rt/DorotiGraphite 오류 로그는 비어 있었다.

회귀 코드: [프레임워크 검증](../../Doroti/tests/Doroti.Tests/TextInputFocusRegression.cs), [Android 프로브](../../samples/DorotiTestbedApp/android/TextInputFocusProbe.cs), [Windows 프로브](../../samples/DorotiTestbedApp/windows/TextInputFocusProbe.cs). Android는 `doroti_text_focus_probe=1` intent extra, Windows는 `DOROTI_TEXT_FOCUS_PROBE=<JSON 출력 경로>`로만 실행하는 opt-in 검증이다.

Windows 프로브는 실제 XAML 입력창/FocusManager를 사용한다. overlay 호스트의 기존 입력 HWND에 raw Composition mouse subscription을 임시로 붙여 down/up 전달과 공통 포커스 보호를 검사하고, XAML 입력용 요소의 자동 포커스 옵션이 꺼져 있는지도 확인한다. 네이티브 TextBox/MauiPasswordTextBox 편집 이벤트가 브리지로 전달되는 것을 검사하며, 물리 키보드/터치와 XAML hit testing 전체를 검증한 것으로 해석하지 않는다. 세 경우 모두 `focused=true`, `routedDowns=1`, `text="seed edited"`로 통과했다.

Windows 검증 앱은 프레임워크 shutdown → 네이티브 렌더링 retirement → 창 닫기 순서로 정리한다. 최종 실행은 종료 코드 0이고 MAUI 예외 파일이 생성되지 않았다. 직접 창을 닫던 초기 프로브의 종료 실패는 최종 PASS 증거에 포함하지 않는다.
