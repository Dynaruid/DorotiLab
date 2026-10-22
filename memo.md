# 텍스트 입력·커서 동기화의 플랫폼별 검토

작성일: 2026-10-22

상태: **검토 완료 / 아래 미해결 항목은 수정·네이티브 재검증 필요**

검토 대상은 기존 텍스트 중간에서 빠르게 입력할 때의 커서 튐, IME 조합·취소·선택 동기화와 입력 지연이다. iOS, Mac Catalyst, macOS AppKit, Android, Windows MAUI, Windows App SDK, Linux Qt, Web을 확인했다. Material/Cupertino는 위젯 계층이며 플랫폼 입력 호스트와 구분한다.

기준 checkout은 `07ba3f31928f7d0f7f131097e094098f721ec1f0`이며, 앞선 Android 커서·성능 수정의 미커밋 변경을 포함한다. 이번 요청에서는 검토와 문서 작성만 수행했다. 제품 코드 수정, 추가 기기 설치와 `work.md` 변경은 하지 않았다.

## 1. 우선 확인할 사항

**Apple만의 문제가 아니다. 같은 문자열 기반 커서 추정이 Apple MAUI와 Windows MAUI에 남아 있다.** Windows App SDK에는 별도의 IMM 조합 처리 누락이 있고, Qt와 Web에서도 조합 중 커서와 조합 범위를 구분하지 못하는 지점이 있다.

여기서 `소스 확인`은 현재 구현과 API 계약의 비교, `함수 반례`는 실제 소스에서 추출한 함수 실행을 뜻한다. 두 결과 모두 특정 OS·IME에서 사람이 타이핑해 증상을 확인한 `실기기 재현`과 다르다. P1은 입력 정확성과 관련해 먼저 조치할 항목, P2는 다음으로 확인·보완할 항목이다.

| ID | 우선순위 | 대상 | 확인한 내용 | 증거 수준 |
| --- | --- | --- | --- | --- |
| R1 | P1 | iOS / Mac Catalyst / macOS AppKit / Windows MAUI | 반복 문자 중간 편집에서 문자열 비교가 실제 삽입 위치와 다른 커서를 계산한다. | 공통 함수 반례 3개, 플랫폼 callback 실기기 재현은 미실행 |
| R2 | P1 | Windows App SDK | 조합 취소 시 미확정 문자열을 제거하지 않고 조합 범위만 지우는 경로가 있다. | 소스 + Microsoft 메시지 계약 |
| R3 | P1 | Windows App SDK | 조합 중 커서 위치 `GCS_CURSORPOS`를 처리하지 않고 커서를 조합 문자열 끝으로 둔다. | 소스 + Microsoft IME 계약 |
| R4 | P2 | Linux Qt | IME query에 preedit 내부 커서 offset이 포함된 위치를 돌려준다. | 소스 + Qt 6.12 계약 |
| R5 | P2 | Web, Apple 기기의 Safari 포함 | 조합 전체 범위의 끝을 현재 선택 끝으로 계산한다. 조합 내부 커서 이동 시 범위가 짧아질 수 있다. | 컴파일한 production 함수의 합성 입력 반례 |

## 2. 플랫폼별 판단

| 플랫폼 / 호스트 | 중간 입력 커서 처리 | 현재 판단 |
| --- | --- | --- |
| Android MAUI | `EditText.SelectionStart/SelectionEnd`를 읽는다. 텍스트 전체 재복사 대신 managed 값과 native buffer 길이를 확인한다. | 앞선 Galaxy Release 자동 검사에서 수정된 경로 확인. 모든 Android IME나 체감 지연까지 통과한 것은 아니다. |
| iOS MAUI | 공통 문자열 비교 fallback을 사용한다. | R1과 UIKit 비동기 상태 적용의 추가 확인 필요. |
| Mac Catalyst MAUI | iOS와 같은 UIKit 및 공통 fallback 경로다. | iOS와 함께 확인해야 한다. |
| macOS AppKit MAUI | 공통 fallback을 사용한다. AppKit 보조 코드는 focus/Return 처리를 보완한다. | focus 보완이 텍스트 변경 순간의 커서 추정을 교체하지는 않는다. R1 대상이다. |
| Windows MAUI | 공통 fallback을 사용한다. 별도 WinUI 구독은 조합 범위를 전달한다. | 조합 범위 구독이 즉시 발행되는 잘못된 커서 추정을 막지 않는다. R1 대상이다. |
| Windows App SDK | C++ 모델이 실제 교체 범위로 텍스트·커서를 계산한다. | 반복 문자열 diff 문제와는 다른 구현. R2/R3 및 비동기 상태 왕복 확인 필요. |
| Linux Qt | `QInputMethodEvent`의 commit/preedit/selection/cursor 정보를 처리한다. | 문자열 diff 문제는 사용하지 않는다. R4와 실제 ibus/fcitx 입력 확인 필요. |
| Web | DOM `selectionStart/End/Direction`을 읽고 오래된 응답을 input sequence로 거절한다. | 일반 커서·stale 응답 보호는 존재한다. R5 및 실제 브라우저별 조합 이벤트 확인 필요. |

`CupertinoTextField`는 내부에서 공통 `EditableText`를 생성하므로 별도 안전 판정을 내릴 수 없다. Android Cupertino에는 Android 호스트 수정이 적용되고, iOS의 Material/Cupertino에는 같은 UIKit 호스트의 위험이 적용된다. iPhone의 **Web 앱**은 UIKit MAUI 대신 Web 경로로 검토해야 한다.

근거: [CupertinoTextField](packages/Doroti.Cupertino/src/text_field.cs#L1552), [공통 MAUI 입력 브리지](packages/platforms/maui/Doroti.Host.Maui/MauiTextInputBridge.cs#L899), [AppKit 보조 처리](packages/platforms/maui/Doroti.Host.Maui/MauiTextInputBridge.MacOS.cs), [WinUI 조합 구독](packages/platforms/maui/Doroti.Host.Maui/MauiTextInputBridge.Windows.cs#L35).

## 3. 미해결 항목 상세

### R1. Apple·Windows MAUI에 남은 문자열 기반 커서 추정

[ReadSelectionAfterTextChange](packages/platforms/maui/Doroti.Host.Maui/MauiTextInputBridge.cs#L1025)는 `#if ANDROID`에서만 네이티브 선택을 읽는다. 다른 MAUI 대상은 `ResolveSelectionAfterTextChange`로 내려가 공통 접두·접미를 비교한다. 같은 글자가 이어지면 문자열만으로 편집 위치를 확정할 수 없다.

현재 C# 메서드 본문을 그대로 추출하고 Android 분기를 제외해 실행했다. 아래 기대 위치는 문장 가운데의 지정된 위치에서 삽입·삭제한 경우의 UTF-16 offset이다.

| 편집 | 이전 → 이후 | 실제 편집에 따른 기대 커서 | 현재 계산 |
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

현재 구현은 조합 문자열을 `text_`에 직접 넣는다. `HandleImeComposition`은 `GCS_RESULTSTR`와 `GCS_COMPSTR`만 처리하고, `WM_IME_ENDCOMPOSITION`에서는 조합 범위를 `-1`로 지운 뒤 그대로 발행한다. 조합 취소를 뜻하는 GCS 값 없는 메시지에서 기존 preedit를 제거하는 처리가 없다. Microsoft는 이 메시지에서 화면에 표시하던 조합 문자열을 지우도록 명시한다. [WM_IME_COMPOSITION 공식 계약](https://learn.microsoft.com/en-us/windows/win32/intl/wm-ime-composition).

코드상 반례 순서는 다음과 같다.

1. `ab`의 offset 1에서 `XY` 조합을 시작해 `text_ = aXYb`, composing `[1,3)`이 된다.
2. 빈 `GCS_COMPSTR`나 결과 문자열을 별도로 주지 않고 취소 메시지가 온다.
3. END 처리에서 범위만 지우면 `aXYb`가 남아 미확정 `XY`가 일반 텍스트처럼 전달될 수 있다. 기대 결과는 `ab`다.

빈 `GCS_COMPSTR`로 preedit 제거를 먼저 통지하는 IME에서는 다른 결과가 나올 수 있다. **실제 IME별 메시지 순서는 미검증**이다. commit 이후 이미 확정된 문자열을 END에서 다시 지우지 않도록, commit/cancel과 현재 preedit 소유 상태를 구분해 보완해야 한다.

### R3. Windows App SDK의 조합 내부 커서 위치 누락

같은 `HandleImeComposition`에서 `GCS_CURSORPOS`를 읽지 않는다. `ReplaceActiveRange`는 선택을 항상 `start + replacement.size()`로 둔다. 문자열은 그대로인 채 IME 커서만 이동하는 메시지도 프레임워크에 갱신되지 않는다. `GCS_CURSORPOS`는 조합 문자열 내부 커서 위치를 전달하는 별도 값이다. [Microsoft IME 값 정의](https://learn.microsoft.com/en-us/windows/win32/intl/ime-composition-string-values).

일본어·중국어 등의 여러 글자 조합 도중 커서를 안쪽으로 옮기면, native IME의 위치와 Doroti 커서가 달라질 수 있다. `GCS_COMPSTR`가 없는 cursor-only 변경도 처리하고, 조합 시작 offset과 내부 위치를 합산·검증한 상태를 발행하는 검사가 필요하다. 이 항목은 소스에서 누락을 확인했으며 실제 Windows IME를 구동해 재현하지는 않았다.

### R4. Qt의 preedit 표시 위치와 IME query 위치가 섞임

근거: [inputMethodQuery](packages/platforms/qt/native/src/doroti_qt_host.cpp#L1158), [HandleInputMethod](packages/platforms/qt/native/src/doroti_qt_host.cpp#L1422).

`HandleInputMethod`는 preedit를 `text_`에 넣고 조합 내부 커서를 `selection_extent_`로 저장한다. query는 그 값을 `Qt::ImCursorPosition`으로 그대로 반환한다. 그러나 Qt 6.12는 이 query의 논리 위치에 preedit 내부 커서 offset을 포함하지 않도록 정의한다. [Qt 6.12 InputMethodQuery](https://doc.qt.io/qt-6/qt.html#InputMethodQuery-enum).

예를 들어 committed text `ab`의 offset 1에 `한`을 조합하고 조합 내부 위치가 1이면, 표시용 텍스트는 `a한b`와 커서 2가 된다. 현재 query는 2를 반환하지만 IME용 논리 삽입 위치는 1이어야 한다. `ImSurroundingText`, anchor/current-selection과의 좌표 일관성도 함께 비교해야 한다.

기존 [Qt native 입력 계약 검사](Doroti/tests/native/qt_input_contract.cpp)는 commit/selection/replacement/preedit/cancel 등을 다루지만, 해당 query의 preedit 좌표 변환을 검사하지 않는다. `QLineEdit`/`QTextEdit`의 같은 입력 및 query 결과를 대조하는 검사를 추가할 필요가 있다. 이번 검토에서는 Qt GUI/ibus/fcitx를 실행하지 않았다.

### R5. Web 조합 범위 끝을 현재 커서로 계산

근거: [composition 이벤트 구독](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.ts#L1134), [emitText](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.ts#L2610).

`compositionStart`는 시작 위치를 저장하지만 `composingExtent`는 현재 DOM `selectionEnd`다. `compositionupdate`의 데이터는 사용하지 않는다. 조합 문자열 전체와 그 안의 커서는 서로 다른 상태이므로, 커서가 조합 중간에 있으면 전체 조합 범위가 잘릴 수 있다.

실제 TypeScript를 컴파일한 뒤 production `emitText`를 Node VM에서 실행했다. 합성 입력 `ab你好cd`, preedit `你好`, 조합 시작 2, 커서 3을 주면 기대 조합 범위는 `[2,4)`지만 현재 함수는 `[2,3)`을 발행한다. 이는 **함수 반례**이며, 실제 Safari/Chrome에서 그 상태를 만드는 이벤트 순서와 사용자 증상은 검증하지 않았다.

조합 문자열 범위와 선택 범위를 따로 추적하고, `compositionupdate.data`, `beforeinput` 등 브라우저가 제공하는 정보를 검토해야 한다. 다만 IME가 DOM에 모든 조합 정보를 노출하지 않을 수 있어 `data.length`만으로 항상 해결된다고 단정하면 안 된다. [W3C UI Events의 composition 계약](https://www.w3.org/TR/uievents/#events-compositionevents).

Web에는 이미 native 커서 직접 조회, 동일 상태 발행 억제, 오래된 worker 응답 거절, 조합 중 value/selection 재기록 방지가 있다. 이번 함수 검사에서 이 네 보호 동작은 유지됨을 확인했다. R5를 Web의 모든 입력 처리가 잘못됐다는 뜻으로 해석하지 않는다.

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
| 이번 검토: non-Android C# 메서드 본문 추출 실행 | 반복 삽입·삭제·emoji 3개 오차 반례, 고유 문자 대조군 정상 | 실제 공통 계산 실행. MAUI/Apple/WinUI runtime은 로드하지 않음. 실행 종료 코드 0은 반례 확인 완료를 뜻하며 제품 검증 PASS가 아님. |
| 이번 검토: 전체 Web host TypeScript strict 컴파일 | PASS | 현재 소스의 컴파일 검사. 브라우저 실행 증거 아님. |
| 이번 검토: 컴파일한 Web 함수 + 합성 DOM 형태의 입력 | 커서·동일 발행 억제·오래된 응답·조합 중 쓰기 방지 4개 확인, 조합 범위 반례 1개 | Node VM 함수 검사. 실제 DOM 이벤트·WebKit·Safari·Chrome IME 검사 아님. |
| 앞선 동일 checkout: Cupertino 커서 회귀 | PASS | iOS/Android/Windows/macOS/Linux 환경 설정의 CPU 위젯·커서 그림·후속 합성 IME 삽입 검사. 실제 native 입력 경로 아님. |
| 앞선 동일 checkout: Galaxy S25 / Android 17 / Release | 편집 18개 PASS, 커서 불일치 0, 포커스·paste 2개 PASS | 실제 Android host에서 InputConnection 자동 명령 실행. 사람의 Samsung 키보드 타이핑·입력부터 화면 표시까지의 지연·FPS는 별도. |
| Apple 실제 입력 | 미실행 | iPhone/iPad, Catalyst, AppKit의 callback 순서·marked text·실제 키보드·성능 미검증. |
| Windows 실제 IME / Qt 실제 IME / 실제 브라우저 IME | 미실행 | R2~R5의 OS·IME별 사용자 증상과 이벤트 순서를 검증하지 않음. |

이번 실행 명령의 테스트·컴파일에는 모두 `Doroti/eng/run-with-timeout.py --timeout 1200`을 사용했다. 산출물은 삭제 가능한 `temp/testing/text-input-platform-review/`이며 C# 추출 메서드·원본 해시·반례, 컴파일한 Web JS·함수 반례를 담는다. Web 컴파일 로그는 `temp/testing/text-editing/platform-review-web-build.log`다. 원시 산출물이 없어져도 위 표의 핵심 결과는 이 문서에 남는다.

앞선 Android 빌드 설정, APK 해시와 세부 검사는 [커서 수정·성능 기록](history/26-10-22/android-text-input-caret.md)을 참고한다.

## 6. 후속 작업 순서와 통과 조건

1. **R1: Apple와 Windows MAUI의 실제 선택 조회.** Entry/Editor 각각 반복 문자·공백·emoji 중간 삽입/삭제를 검사한다. 최종 위치만 보지 않고 전달되는 모든 callback의 텍스트·선택·조합을 native 상태와 비교한다. UIKit/AppKit에서 같은 UI turn과 지연 echo 양쪽을 확인한다.
2. **R2/R3: Windows App SDK IMM 취소·커서.** preedit 내부 이동, 문자열 변경 없는 cursor-only 메시지, commit, Escape 취소, GCS 값 없는 취소, 결과 문자열 직후 END를 각각 확인한다. 취소한 preedit가 확정 텍스트로 남지 않아야 한다.
3. **R4: Qt query 좌표.** 조합 문자열이 없는 논리 문서 좌표와 표시 문서 좌표를 분리한다. preedit 내부 위치 0/중간/끝, 취소·선택·replacement를 Qt 기본 입력창과 비교하고 ibus/fcitx에서도 확인한다.
4. **R5: Web 조합 범위.** 커서가 조합 중간에 있어도 전체 조합 범위가 유지되어야 한다. 실제 Chromium·WebKit 및 Apple 기기 Safari에서 이벤트 순서와 stale 응답 보호를 함께 확인한다.
5. **공통 정확성·성능 회귀.** 빠른 연속 입력/Backspace는 우선 12회 정도의 짧은 검사로 시작한다. 조합 중 범위 선택·붙여넣기·formatter·field 전환·clear/reattach를 추가하고, 수정 플랫폼의 Release에서 입력 callback 시간과 화면 반영 시점을 따로 측정한다. 소스 검사·CPU 검사·자동 IME 명령을 물리 입력·프레임 페이싱 증거로 대체하지 않는다.

플랫폼별 native 재현과 수정 후 검증 전에는 Apple 또는 전체 플랫폼의 입력 문제가 해결됐다고 표시하지 않는다.
