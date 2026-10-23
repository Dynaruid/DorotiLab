# 텍스트 입력·커서 동기화의 플랫폼별 검토

작성일: 2026-10-22

Windows·Web 수정일: 2026-10-23

상태: **Windows R1 일반 입력·R2/R3·Web R5 수정 및 자동 검증 완료 / 물리 IME·Safari·성능 검증은 미완료. Apple R1·Qt R4는 미수정.**

검토 대상은 기존 텍스트 중간에서 빠르게 입력할 때의 커서 튐, IME 조합·취소·선택 동기화와 입력 지연이다. iOS, Mac Catalyst, macOS AppKit, Android, Windows MAUI, Windows App SDK, Linux Qt, Web을 확인했다. Material/Cupertino는 위젯 계층이며 플랫폼 입력 호스트와 구분한다.

10월 22일 검토 기준은 `07ba3f31928f7d0f7f131097e094098f721ec1f0`과 당시 Android 수정이었다. 10월 23일 Windows·Web 후속 작업은 깨끗한 `04ad305c92bc5cc354c8f1d86cf84d910006dff3`에서 시작해 제품 코드·회귀 검사를 수정했다. 결과와 재실행 명령은 [Windows·Web 수정 기록](history/26-10-23/windows-web-text-input.md)에 보존한다. `work.md`는 변경하지 않았다.

## 1. 우선 확인할 사항

**10월 22일에는 Apple MAUI와 Windows MAUI에 같은 문자열 기반 커서 추정이 남아 있었다.** 10월 23일 Windows 일반 TextBox 경로는 네이티브 선택 조회로 교체했고, Windows App SDK의 IMM 취소·내부 커서와 Web 조합 범위도 수정했다. Apple·Qt의 기존 지적과 플랫폼별 실제 IME 검증은 남아 있다.

여기서 `소스 확인`은 현재 구현과 API 계약의 비교, `함수 반례`는 실제 소스에서 추출한 함수 실행을 뜻한다. 두 결과 모두 특정 OS·IME에서 사람이 타이핑해 증상을 확인한 `실기기 재현`과 다르다. P1은 입력 정확성과 관련해 먼저 조치할 항목, P2는 다음으로 확인·보완할 항목이다.

| ID | 우선순위 | 대상 | 확인한 내용 | 증거 수준 |
| --- | --- | --- | --- | --- |
| R1 | P1 | iOS / Mac Catalyst / macOS AppKit / Windows MAUI | Windows 일반 Entry/Editor는 실제 선택 조회로 수정. Apple·선택 API 없는 보안 입력 fallback은 남음. | Windows Release native callback 검사, Apple native 미실행 |
| R2 | P1 | Windows App SDK | 소유한 preedit만 취소 시 제거하고 확정 결과는 보존하도록 수정. | production C++ 상태 검사 + native Release 빌드, 실제 Windows IME 미실행 |
| R3 | P1 | Windows App SDK | `GCS_CURSORPOS`와 cursor-only 메시지를 처리하도록 수정. | R2와 합쳐 C++ 상태 검사 16개 PASS, 실제 Windows IME 미실행 |
| R4 | P2 | Linux Qt | IME query에 preedit 내부 커서 offset이 포함된 위치를 돌려준다. | 소스 + Qt 6.12 계약 |
| R5 | P2 | Web, Apple 기기의 Safari 포함 | 전체 조합 범위를 선택과 분리하고 DOM 반영 전 조합 데이터 발행을 방지하도록 수정. | WebKit 21개·Chrome 22개 PASS. Chrome 브라우저 조합 명령 포함. Apple Safari·물리 IME 미실행 |

## 2. 플랫폼별 판단

| 플랫폼 / 호스트 | 중간 입력 커서 처리 | 현재 판단 |
| --- | --- | --- |
| Android MAUI | `EditText.SelectionStart/SelectionEnd`를 읽는다. 텍스트 전체 재복사 대신 managed 값과 native buffer 길이를 확인한다. | 앞선 Galaxy Release 자동 검사에서 수정된 경로 확인. 모든 Android IME나 체감 지연까지 통과한 것은 아니다. |
| iOS MAUI | 공통 문자열 비교 fallback을 사용한다. | R1과 UIKit 비동기 상태 적용의 추가 확인 필요. |
| Mac Catalyst MAUI | iOS와 같은 UIKit 및 공통 fallback 경로다. | iOS와 함께 확인해야 한다. |
| macOS AppKit MAUI | 공통 fallback을 사용한다. AppKit 보조 코드는 focus/Return 처리를 보완한다. | focus 보완이 텍스트 변경 순간의 커서 추정을 교체하지는 않는다. R1 대상이다. |
| Windows MAUI | 일반 TextBox의 실제 선택을 읽고 MAUI가 반영한 문자열과 같은 native TextChanged에서 발행한다. TextChanging 이후 오래된 선택 snapshot을 막는다. | R1 일반 Entry/Editor 자동 검증 완료. 보안 입력 fallback·물리 IME·성능은 별도. |
| Windows App SDK | C++ 모델이 실제 교체 범위와 preedit 소유 상태·IMM 내부 커서를 처리한다. | R2/R3 수정·상태 검사 완료. 실제 IMM 메시지 순서와 비동기 상태 왕복은 추가 확인 필요. |
| Linux Qt | `QInputMethodEvent`의 commit/preedit/selection/cursor 정보를 처리한다. | 문자열 diff 문제는 사용하지 않는다. R4와 실제 ibus/fcitx 입력 확인 필요. |
| Web | DOM 선택과 독립된 전체 조합 범위를 추적하고 오래된 응답을 input sequence로 거절한다. | R5 수정·실제 Chrome/WebKit DOM 검사 완료. Apple Safari·OS IME·실제 managed worker 왕복은 별도. |

`CupertinoTextField`는 내부에서 공통 `EditableText`를 생성하므로 별도 안전 판정을 내릴 수 없다. Android Cupertino에는 Android 호스트 수정이 적용되고, iOS의 Material/Cupertino에는 같은 UIKit 호스트의 위험이 적용된다. iPhone의 **Web 앱**은 UIKit MAUI 대신 Web 경로로 검토해야 한다.

근거: [CupertinoTextField](packages/Doroti.Cupertino/src/text_field.cs#L1552), [공통 MAUI 입력 브리지](packages/platforms/maui/Doroti.Host.Maui/MauiTextInputBridge.cs#L899), [AppKit 보조 처리](packages/platforms/maui/Doroti.Host.Maui/MauiTextInputBridge.MacOS.cs), [WinUI 조합 구독](packages/platforms/maui/Doroti.Host.Maui/MauiTextInputBridge.Windows.cs#L35).

## 3. 검토 당시 결함과 수정 상태

### R1. Apple MAUI의 잔여 문제와 Windows MAUI 수정

검토 당시 [ReadSelectionAfterTextChange](packages/platforms/maui/Doroti.Host.Maui/MauiTextInputBridge.cs)는 Android에서만 네이티브 선택을 읽고 다른 MAUI 대상은 공통 접두·접미를 비교했다. 같은 글자가 이어지면 문자열만으로 편집 위치를 확정할 수 없다. 아래 표는 **수정 전 공통 fallback**의 반례다.

Windows 일반 TextBox는 [Windows 입력 구독](packages/platforms/maui/Doroti.Host.Maui/MauiTextInputBridge.Windows.cs)에서 실제 `SelectionStart/SelectionLength`를 사용한다. WinUI의 native TextChanging 이후 MAUI의 문자열이 아직 이전 값인 구간에는 선택 알림을 발행하지 않고, MAUI 매퍼 다음 native TextChanged 구독에서 문자열·선택을 함께 발행한다. 텍스트 전달은 해당 native callback 안에서 수행하며 native 전체 문자열을 추가로 읽지 않는다. PasswordBox·보안 모드·handler 부재·유효하지 않은 선택은 fallback으로 남긴다.

10월 22일 C# 메서드 본문을 추출하고 Android 분기를 제외해 실행했다. 아래 기대 위치는 문장 가운데의 지정된 위치에서 삽입·삭제한 경우의 UTF-16 offset이다.

| 편집 | 이전 → 이후 | 실제 편집에 따른 기대 커서 | 수정 전 계산 |
| --- | --- | --- | --- |
| 반복 문자 중간 삽입 | `aaaa` → `aaaaa` | 3 | 5 |
| 반복 문자 중간 Backspace | `aaaaa` → `aaaa` | 2 | 4 |
| 반복 emoji 중간 삽입 | `😀😀` → `😀😀😀` | 4 | 6 |
| 고유 문자 삽입 대조군 | `abc` → `aXbc` | 2 | 2 |

이는 **공통 계산의 오류 반례**다. 각 플랫폼의 MAUI/native callback 순서와 실제 화면의 튐 지속 시간까지 재현한 결과는 아니다. iOS/Windows 등에서 뒤이은 선택 알림이 정상 위치를 전달하더라도, 잘못된 중간 상태가 이미 프레임워크로 전달될 수 있다는 점이 문제다.

보완 방향은 플랫폼이 가진 실제 선택을 우선 사용하는 것이다. UIKit은 `SelectedTextRange`와 문서 시작점 기준 offset, AppKit은 현재 field editor/`NSTextView`의 선택 범위, WinUI는 지원되는 native 입력창의 selection을 확인한다. handler 부재·선택 없음·보안 입력창 등 fallback 조건은 구분해야 한다. [UIKit selectedTextRange](https://developer.apple.com/documentation/uikit/uitextinput/selectedtextrange), [AppKit selectedRange](https://developer.apple.com/documentation/appkit/nstextinputclient/selectedrange%28%29).

전체 native 문자열을 매번 다시 읽는 방식을 그대로 확대하지 않는다. 동일 텍스트 revision을 확인하는 데 필요한 최소 조회로 구성하고, 조합 범위와 텍스트·선택의 일관성을 함께 검사해야 한다.

### R2. Windows App SDK의 조합 취소 처리

근거: [WM_IME_COMPOSITION / END 처리](packages/platforms/windowsappsdk/native/src/exports.cpp#L809), [HandleImeComposition](packages/platforms/windowsappsdk/native/src/exports.cpp#L1727), [ReplaceActiveRange](packages/platforms/windowsappsdk/native/src/exports.cpp#L1751).

수정 전 구현은 조합 문자열을 문서에 직접 넣지만 `WM_IME_ENDCOMPOSITION`에서 조합 범위만 지웠고, GCS 값 없는 취소 메시지에서도 preedit를 제거하지 않았다. Microsoft는 이 메시지에서 표시하던 조합 문자열을 지우도록 명시한다. [WM_IME_COMPOSITION 공식 계약](https://learn.microsoft.com/en-us/windows/win32/intl/wm-ime-composition).

수정 후 [production IMM 상태 모델](packages/platforms/windowsappsdk/native/src/ime_editing_state.h)은 실제 preedit를 삽입했는지 따로 추적한다. GCS 없는 취소와 END에서는 소유한 preedit만 제거한다. START 직후 원래 선택 영역과 RESULTSTR로 확정한 텍스트는 지우지 않는다. 빈 문자열과 IMM 조회 실패도 구분한다. 아래 순서는 수정 전 반례이며 상태 회귀 검사에서 기대 결과를 확인했다.

코드상 반례 순서는 다음과 같다.

1. `ab`의 offset 1에서 `XY` 조합을 시작해 `text_ = aXYb`, composing `[1,3)`이 된다.
2. 빈 `GCS_COMPSTR`나 결과 문자열을 별도로 주지 않고 취소 메시지가 온다.
3. END 처리에서 범위만 지우면 `aXYb`가 남아 미확정 `XY`가 일반 텍스트처럼 전달될 수 있다. 기대 결과는 `ab`다.

빈 `GCS_COMPSTR` 제거·GCS 없는 취소·END·result 이후 END는 수정 모델에서 검사했다. **실제 Windows IME별 메시지 순서는 미검증**이다. 확정 결과와 preedit 소유 상태 구분은 구현했지만 OS IME의 동작까지 통과한 것으로 표시하지 않는다.

### R3. Windows App SDK의 조합 내부 커서 위치 누락

수정 전 `HandleImeComposition`은 `GCS_CURSORPOS`를 읽지 않고 선택을 항상 조합 끝으로 두었다. 수정 후에는 composition string 갱신 뒤 내부 커서를 적용하며 문자열 없는 cursor-only 메시지도 발행한다. 음수 조회 결과는 무시하고 UTF-16 조합 범위 안으로 제한한다. `GCS_CURSORPOS`는 조합 문자열 내부 커서 위치를 전달하는 별도 값이다. [Microsoft IME 값 정의](https://learn.microsoft.com/en-us/windows/win32/intl/ime-composition-string-values).

일본어·중국어 등 실제 Windows IME에서 문자열 변경 없는 내부 커서 이동을 확인해야 한다. cursor-only·범위 경계·emoji·result와 다음 preedit가 한 메시지에 있는 경우는 production 상태 모델에서 통과했지만, Windows IMM 메시지를 실제 IME로 구동한 검사는 남아 있다.

### R4. Qt의 preedit 표시 위치와 IME query 위치가 섞임

근거: [inputMethodQuery](packages/platforms/qt/native/src/doroti_qt_host.cpp#L1158), [HandleInputMethod](packages/platforms/qt/native/src/doroti_qt_host.cpp#L1422).

`HandleInputMethod`는 preedit를 `text_`에 넣고 조합 내부 커서를 `selection_extent_`로 저장한다. query는 그 값을 `Qt::ImCursorPosition`으로 그대로 반환한다. 그러나 Qt 6.12는 이 query의 논리 위치에 preedit 내부 커서 offset을 포함하지 않도록 정의한다. [Qt 6.12 InputMethodQuery](https://doc.qt.io/qt-6/qt.html#InputMethodQuery-enum).

예를 들어 committed text `ab`의 offset 1에 `한`을 조합하고 조합 내부 위치가 1이면, 표시용 텍스트는 `a한b`와 커서 2가 된다. 현재 query는 2를 반환하지만 IME용 논리 삽입 위치는 1이어야 한다. `ImSurroundingText`, anchor/current-selection과의 좌표 일관성도 함께 비교해야 한다.

기존 [Qt native 입력 계약 검사](Doroti/tests/native/qt_input_contract.cpp)는 commit/selection/replacement/preedit/cancel 등을 다루지만, 해당 query의 preedit 좌표 변환을 검사하지 않는다. `QLineEdit`/`QTextEdit`의 같은 입력 및 query 결과를 대조하는 검사를 추가할 필요가 있다. 이번 검토에서는 Qt GUI/ibus/fcitx를 실행하지 않았다.

### R5. Web 조합 범위 끝을 현재 커서로 계산

근거: [composition 이벤트 구독](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.ts#L1134), [emitText](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.ts#L2610).

수정 전에는 시작 위치만 저장하고 `composingExtent`를 DOM `selectionEnd`로 계산했다. 수정 후 [BrowserTextComposition](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.text-composition.ts)은 조합 시작 시 교체 영역 양쪽 문서를 저장해 전체 범위를 구하고, 선택은 DOM에서 별도로 읽는다. 조합 데이터는 input 또는 DOM 반영 후 microtask에서 일치 여부를 확인해 적용한다. 부분 확정·선택 교체·emoji·조합 데이터 없는 경우와 clear/reattach의 오래된 callback도 검사했다. 문서 양쪽이 일치하지 않으면 임의의 범위를 발행하지 않는다.

10월 22일 TypeScript를 컴파일한 production `emitText`의 Node VM 검사에서 `ab你好cd`, 조합 시작 2, 커서 3의 기대 범위 `[2,4)` 대신 `[2,3)`을 확인했다. 10월 23일에는 수정 후 실제 Chrome의 브라우저 조합 명령에서도 커서 3과 조합 범위 `[2,4)`를 함께 확인했다. WebKit은 실제 DOM에 합성 composition/input 이벤트를 보내 검사했으며 Apple Safari나 OS IME를 실행한 증거는 아니다.

조합 이벤트가 DOM 갱신 전에 올 수 있고 일부 IME는 조합 데이터를 노출하지 않으므로 `data.length`만으로 범위를 확정하지 않는다. DOM 양쪽 문서 검증과 이벤트 데이터 확인을 함께 사용한다. [W3C UI Events의 composition 계약](https://www.w3.org/TR/uievents/#events-compositionevents).

기존 native 커서 조회·동일 발행 억제·오래된 worker 응답 거절·조합 중 value/selection 쓰기 방지는 수정 후 Chrome/WebKit DOM 검사에서도 유지됐다. 이 검사는 worker를 대체했으므로 실제 managed worker 전체 왕복의 통과 증거로 사용하지 않는다.

## 4. 별도로 검증해야 할 상태 왕복·성능 위험

아래 항목은 **재현된 결함이 아니라 코드 구조에서 확인한 추가 검증 대상**이다.

| 항목 | 근거 및 확인할 상황 |
| --- | --- |
| UIKit의 지연된 echo | [DispatchInputMutation](packages/platforms/maui/Doroti.Host.Maui/MauiTextInputBridge.cs#L552)은 iOS/Catalyst에서 항상 UI dispatcher로 보낸다. `_publishingNativeTextChange`는 동기 callback 동안만 true이므로, 지연 실행 시 [UpdateStateCore](packages/platforms/maui/Doroti.Host.Maui/MauiTextInputBridge.cs#L262)의 동일 입력 보호가 다른 조건으로 평가된다. 즉시 echo, formatter가 바꾼 상태, 다음 입력 후 도착한 상태를 구분해 검사해야 한다. |
| 비동기 native 상태 적용 | Windows App SDK는 native 텍스트를 managed 입력 큐로 보낸 뒤 상태 갱신을 Win32 메시지로 되돌린다. Qt도 `UpdateTextState`를 queued connection으로 적용한다. 해당 text-state 구조체에는 Web과 같은 acknowledged input sequence가 없다. 이것만으로 순서 오류가 발생한다고 확정할 수는 없지만, 지연된 응답이 최신 입력이나 새 client를 덮지 않는지 확인해야 한다. |
| 선택만 바뀐 알림 | MAUI 선택 알림은 UI turn 단위로 합친다. 텍스트 변경의 즉시 발행을 전부 지연 방식으로 돌리면 공통 브리지 주석에 기록된 연속 Backspace의 갱신 정체가 재발할 수 있다. 최신 텍스트·선택·조합을 함께 발행하면서 텍스트 전달은 굶지 않아야 한다. |
| 동일 알림의 비용 | 공통 `EditableText.updateEditingValue`는 같은 값이면 일찍 반환한다. 따라서 동일 알림이 곧바로 중복 layout/paint를 뜻하지 않는다. callback·문자열 변환·microtask·프레임 대기 비용은 실제 측정으로 구분해야 한다. |
| 플랫폼별 빌드 조건 | 직전 Galaxy Debug는 `AndroidUseInterpreter=true`, `Optimize=false`였다. 앞서 설치한 빌드는 Release/프로파일 기반 Mono AOT다. 이 설정을 Apple/Windows/Qt/Web에 그대로 일반화하지 않고 각 빌드의 실제 값을 기록해야 한다. |

관련 구조: [Windows text state ABI](packages/platforms/windowsappsdk/native/include/doroti_windows_host_v1.h#L147), [Qt text state ABI](packages/platforms/qt/native/include/doroti_qt_host_v2.h#L200), [Qt queued update](packages/platforms/qt/native/src/doroti_qt_host.cpp#L518), [Web sequence 보호](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.ts#L1404), [공통 EditableText](Doroti/src/Doroti.Framework.Widgets/editable_text.cs#L2918).

## 5. 확인한 검사와 한계

| 검사 | 결과 | 의미 / 제외 범위 |
| --- | --- | --- |
| 10/23 Windows MAUI Release 실제 WinUI callback | Entry/Editor 16개 시나리오·60회 편집·68개 callback, 불일치 0 | 실제 native TextBox의 문자열·선택과 매 callback 비교. 반복 문자·공백·emoji·선택 교체 및 12회 연속 삽입/삭제. 합성 native 편집이며 물리 키보드·IME·화면 지연 검사가 아님. |
| 10/23 Windows App SDK native Release + production C++ 상태 | 빌드 PASS, 상태 검사 16개 PASS | cancel/END/commit/cursor-only/빈 preedit/UTF-16/조회 실패. 실제 IMM context·IME 메시지 구동은 제외. |
| 10/23 Web host Release + strict TypeScript | 경고·오류 0, PASS | Release 생성 JS와 새 조합 모듈을 아래 브라우저 검사에서 실제 로드. |
| 10/23 실제 WebKit 27.2 / Chrome 155.0.8059.86 DOM | 각각 21개 / 22개 PASS | DOM 합성 이벤트·선택·stale/formatter 보호·clear/reattach·12회 입력/삭제. Chrome은 CDP 브라우저 조합 명령의 중간 커서·확정/취소 2개 흐름 포함. worker/렌더러 대체, Apple Safari·OS IME·화면 성능 제외. |
| 10/22 검토: non-Android C# 메서드 본문 추출 실행 | 반복 삽입·삭제·emoji 3개 오차 반례, 고유 문자 대조군 정상 | 당시 공통 계산 실행. native runtime 미로드. 제품 검증 PASS가 아님. |
| 10/22 검토: 전체 Web host TypeScript strict 컴파일 | PASS | 당시 소스의 컴파일 검사. |
| 10/22 검토: 컴파일한 Web 함수 + 합성 DOM 형태의 입력 | 기존 보호 4개 확인, 수정 전 조합 범위 반례 1개 | 당시 Node VM 함수 검사. |
| 앞선 동일 checkout: Cupertino 커서 회귀 | PASS | iOS/Android/Windows/macOS/Linux 환경 설정의 CPU 위젯·커서 그림·후속 합성 IME 삽입 검사. 실제 native 입력 경로 아님. |
| 앞선 동일 checkout: Galaxy S25 / Android 17 / Release | 편집 18개 PASS, 커서 불일치 0, 포커스·paste 2개 PASS | 실제 Android host에서 InputConnection 자동 명령 실행. 사람의 Samsung 키보드 타이핑·입력부터 화면 표시까지의 지연·FPS는 별도. |
| Apple 실제 입력 | 미실행 | iPhone/iPad, Catalyst, AppKit의 callback 순서·marked text·실제 키보드·성능 미검증. |
| Windows OS IME / Qt 실제 IME / 브라우저 OS IME·Apple Safari | 미실행 | 브라우저 명령·합성 이벤트는 OS 키보드의 사용자 입력과 다름. |
| Windows MAUI 프로세스 정상 종료 | notVerified | 검사 후 window.Close를 호출해도 5초 내 프로세스가 종료되지 않아 runner가 자신이 띄운 프로세스를 정리했다. 입력 검사 PASS와 정상 종료를 구분한다. |

테스트·컴파일에는 모두 `Doroti/eng/run-with-timeout.py --timeout 1200`을 사용했다. 10/23 원시 산출물은 삭제 가능한 `temp/testing/windows-web-text-input/`와 `temp/testing/text-input-*-build*.log`다. 10/22 검토 산출물은 `temp/testing/text-input-platform-review/`, Web 컴파일 로그는 `temp/testing/text-editing/platform-review-web-build.log`다. 핵심 결과·해시·명령은 이 문서와 [수정 기록](history/26-10-23/windows-web-text-input.md)에 남긴다.

앞선 Android 빌드 설정, APK 해시와 세부 검사는 [커서 수정·성능 기록](history/26-10-22/android-text-input-caret.md)을 참고한다.

## 6. 후속 작업 순서와 통과 조건

1. **R1: Apple 실제 선택 조회와 Windows 잔여 검증.** Apple은 미수정이다. UIKit/AppKit의 native 선택·같은 UI turn·지연 echo를 확인한다. Windows 일반 Entry/Editor 자동 callback 비교는 완료했으며 보안 입력 fallback·실제 IME와 프로세스 정상 종료는 추가 확인한다.
2. **R2/R3: Windows 실제 IMM 취소·커서.** 구현·상태 검사는 완료했다. 실제 한국어·일본어·중국어 IME에서 preedit 내부 이동, cursor-only, commit, Escape, GCS 없는 취소, result 직후 END와 지연 managed 응답을 확인한다.
3. **R4: Qt query 좌표.** 조합 문자열이 없는 논리 문서 좌표와 표시 문서 좌표를 분리한다. preedit 내부 위치 0/중간/끝, 취소·선택·replacement를 Qt 기본 입력창과 비교하고 ibus/fcitx에서도 확인한다.
4. **R5: OS IME와 Apple Safari.** Chrome 브라우저 조합 명령·WebKit DOM 검사는 완료했다. OS IME 및 Apple 기기의 Safari에서 이벤트 순서·전체 조합 범위·stale 응답 보호를 확인하고 실제 managed worker와 연결해 재검증한다.
5. **공통 정확성·성능 회귀.** Windows/Web의 12회 연속 합성 입력·삭제는 완료했다. 실제 조합 중 붙여넣기·formatter·field 전환과 Release 화면 반영을 확인한다. Windows 합성 편집→callback 시간은 기록했지만 물리 입력 지연·FPS·프레임 페이싱은 측정하지 않았다.

플랫폼별 native 재현과 수정 후 검증 전에는 Apple 또는 전체 플랫폼의 입력 문제가 해결됐다고 표시하지 않는다.
