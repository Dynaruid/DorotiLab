# 플랫폼별 텍스트 입력·커서 동기화 검토 요약

- 최초 검토: 2026-10-22. Windows·Web·Apple 후속: 10월 23일. Linux Qt 후속: 10월 24일. 보관일: 2026-10-27 KST.
- 원본: 루트 `memo.md`. 삭제 직전 SHA-256: `e3a66b0bf848d91cb498bbc20bc5a2d63e8dcae9bd0b37e993e45d07a6b3227f`.
- 검토 기준: `07ba3f31928f7d0f7f131097e094098f721ec1f0`과 당시 Android 수정.
- 상태: **CLOSED / ARCHIVED · R1~R5 구현 및 아래 자동 검증 완료 / 실제 IME·전체 framework 왕복·Safari·화면 성능은 미완료**. Windows 보안 입력 fallback과 프로세스 정상 종료도 추가 검증 대상이다.

사용자 요청으로 검토·수정 결과와 남은 항목을 요약하고 루트 `memo.md`를 삭제했다. 아래는 원본의 10월 24일까지 기록이며 보관 과정에서 제품 검증을 재실행하지 않았다. 구현 상세·빌드 프로필·해시·재실행 명령은 이미 있는 날짜별 기록에 보존한다.

| 작업일 | 보존 기록 |
| --- | --- |
| 10월 22일 | [Android 커서 수정·Release/AOT 검증](android-text-input-caret.md), 이 문서의 플랫폼 검토 |
| 10월 23일 | [Windows·Web R1/R2/R3/R5](../23/windows-web-text-input.md), [Apple R1](../23/apple-text-input.md) |
| 10월 24일 | [Linux Qt R4](../24/linux-qt-text-input.md) |

## 검토에서 확인한 문제와 후속 수정

대상은 Android·iOS·Catalyst·AppKit·Windows MAUI·Windows App SDK·Qt·Web의 중간 입력, 선택, 조합·취소, 상태 왕복이다. Material/Cupertino는 위젯 계층이고 실제 입력 처리는 각 플랫폼 호스트를 따른다. `CupertinoTextField`도 공통 `EditableText`를 사용하며 iPhone의 Web 앱은 UIKit MAUI 대신 Web 경로로 판단한다.

| ID·우선순위 | 당시 결함 | 10월 24일까지 수정·한계 |
| --- | --- | --- |
| R1 / P1 | non-Android MAUI가 문자열 접두·접미로 커서를 추정해 반복 문자·emoji 중간 편집을 잘못 계산 | Windows 일반 TextBox는 실제 선택·같은 native callback의 managed 문자열을 사용. UIKit은 `SelectedTextRange`, AppKit은 현재 field editor/`SelectedRange` 사용. handler/API/선택 부재 및 Windows 보안 입력 fallback은 남음 |
| R2 / P1 | Windows IMM 취소·END가 소유한 미확정 preedit를 문서에 남길 수 있음 | 실제 preedit 소유를 추적해 취소/END 시 그것만 제거. START의 원래 선택과 확정 결과 보존, 빈 문자열과 조회 실패 구분. 실제 OS IME 순서는 미검증 |
| R3 / P1 | Windows IMM이 `GCS_CURSORPOS`를 무시하고 커서를 조합 끝으로 고정 | 조합 내부 UTF-16 커서 및 cursor-only 갱신 처리. 음수 조회 결과·경계·result+다음 preedit 검사. 실제 IME 내부 이동은 미검증 |
| R4 / P2 | Qt IME query에 preedit가 포함된 표시 문자열·커서 좌표 사용 | framework 표시 상태를 유지하면서 query의 surrounding·cursor·anchor·선택을 committed 문서 좌표로 변환. 실제 ibus/fcitx·전체 managed 왕복은 미검증 |
| R5 / P2 | Web 전체 조합 범위 끝을 DOM 커서로 계산하고 DOM 반영 전 이벤트와 섞을 수 있음 | 교체 영역 양쪽 문서로 전체 범위를 추적하고 선택은 별도로 읽음. input/DOM 반영 뒤 데이터 일치를 확인하며 clear/reattach의 오래된 callback 무효화. Safari·OS IME·실제 worker 왕복은 미검증 |

검토 당시 추출 실행한 공통 C# fallback은 `aaaa→aaaaa`의 중간 삽입에서 기대 커서 3 대신 5, `aaaaa→aaaa`의 Backspace에서 2 대신 4, 반복 emoji 삽입에서 4 대신 6을 반환했다. 고유 문자 대조군은 정상이었다. 이는 계산 함수 반례이며 각 native callback·화면 증상을 실제 IME로 재현한 검사가 아니다.

Web의 수정 전 함수 검사는 `ab你好cd`, 조합 시작 2·커서 3에서 기대 전체 범위 `[2,4)` 대신 `[2,3)`을 확인했다. 수정 뒤 Chrome 브라우저 조합 명령에서는 커서 3과 전체 범위 `[2,4)`를 함께 확인했다. Qt는 `ab`의 offset 1에 `한`을 조합하면 표시 상태 `a한b`·커서 2를 유지하되 IME에는 `ab`·cursor/anchor 1을 반환하도록 바꿨다.

## 상태 발행과 지연 응답 처리

Windows MAUI는 native TextChanging 뒤 managed 문자열이 이전 revision인 구간에 선택 snapshot을 발행하지 않고 MAUI 매퍼 뒤의 native TextChanged에서 문자열·선택을 함께 보낸다. 일반 입력에서 native 전체 문자열을 추가로 읽지 않는다.

Apple은 UIKit 조합 메서드 종료 후에도 상태를 발행해 문자열이 같은 unmark를 전달하고, AppKit은 MAUI 알림 없이 바뀐 field editor의 텍스트/storage/선택을 구독한다. 일반 입력은 managed 문자열을 재사용하고 MAUI에 반영되지 않은 AppKit 문자 변경만 dirty revision당 한 번 읽는다. 같은 echo는 다시 쓰지 않으며 formatter는 입력 처리 뒤 revision·client generation·최신 요청·미발행 선택/조합을 확인해 적용한다. 재표시·endpoint 전환도 native 편집 상태를 보존한다.

이 보호는 브리지 안의 큐와 native callback 응답을 대상으로 한다. 외부에서 응답 자체가 지연돼 다음 입력 뒤 처음 `UpdateState`를 호출하면 원래 입력 번호가 없어 모든 오래된 응답을 식별한다고 일반화할 수 없다. Windows App SDK·Qt의 비동기 text-state에도 Web과 같은 acknowledged input sequence가 없어 지연 응답·formatter·client 전환은 전체 왕복 검증이 남는다. 이 구조만으로 순서 결함이 재현됐다고 단정하지 않는다.

선택만 바뀐 MAUI 알림은 UI turn 단위로 합치되 텍스트 변경의 즉시 발행은 유지한다. 공통 `EditableText`의 동일 값 조기 반환이 있으므로 알림 수를 layout/paint 비용으로 바로 환산하지 않는다. callback·문자열 변환·microtask·프레임 대기와 화면 지연은 실제 측정 대상이다.

## 당시 검증과 제외 범위

| 날짜·대상 | 당시 결과 | 제외 범위 |
| --- | --- | --- |
| 10/22 검토 | non-Android C# 추출 함수의 오차 반례 3개, Web strict TypeScript 컴파일 PASS, Node VM 함수의 기존 보호 4개·조합 범위 반례 1개 | native runtime·물리 IME 제품 승인 아님 |
| 10/22 Android | Galaxy S25 / Android 17 Release 프로파일 기반 Mono AOT, 편집 18개 PASS·불일치 0, 포커스/paste 2개 PASS | InputConnection 자동 명령. 사람의 키보드 입력·지연·FPS 제외 |
| 10/23 Windows MAUI | Release Entry/Editor 16개 시나리오·60회 편집·68 callback, 불일치 0 | 실제 WinUI의 합성 편집. 보안 fallback·물리 키보드/IME·화면 지연 제외 |
| 10/23 Windows App SDK | native Release 빌드 및 production C++ IMM 상태 16개 PASS | 실제 IMM context·OS 메시지 구동 제외 |
| 10/23 Web | Release/strict TypeScript 경고·오류 0, WebKit 27.2 21개·Chrome 155.0.8059.86 22개 PASS | 실제 DOM 합성 이벤트, Chrome CDP 조합 포함. worker/렌더러 대체, Apple Safari·OS IME·화면 성능 제외 |
| 10/23 AppKit Release | 40개 시나리오·124단계·145 callback, 불일치 0. 일반 편집 90회의 추가 전체 문자열 읽기 0 | 검사 client를 연결한 native bridge. 물리 IME·전체 위젯 왕복 제외 |
| 10/23 Catalyst Release | 40개 시나리오·124단계·132 callback, 불일치 0 | UIKit native 조합·echo/formatter·전환 검사. OS IME·화면 성능 제외 |
| 10/23 iOS Simulator | iPhone 17 / iOS 27.0, Release 40개·124단계·132 callback PASS. Debug도 40개·130 callback PASS | Release Mono AOT·Optimize=true·MtouchInterpreter=-all·LLVM=false. 실기기·기본 LLVM=true·전체 위젯 왕복 제외 |
| 10/24 Qt 6.12 Release | Quick/QWindow × xcb/Wayland × current/legacy의 8개 실행에서 각 58개 QLineEdit/QTextEdit query 비교 PASS | native 합성 이벤트. 실제 ibus/fcitx·전체 managed 왕복·GPU/화면 제외 |
| Windows MAUI 정상 종료 | **notVerified**. window.Close 뒤 5초 내 종료되지 않아 runner가 생성 프로세스를 정리 | 입력 PASS와 정상 종료를 구분 |

기존 CPU Cupertino 커서/합성 IME 회귀도 통과했으나 플랫폼 native 입력 증거는 아니다. Windows/Web과 Apple의 연속 합성 삽입·삭제는 각 12회로 검사했다. 테스트·컴파일은 `Doroti/eng/run-with-timeout.py --timeout 1200`을 사용했으며 preliminary 실패·기동 timeout은 최종 입력 PASS와 구분해 상세 기록에 남겼다.

## 남은 확인과 소스 위치

1. R1의 실제 한/일/중 IME·전체 framework 왕복, Windows 보안 fallback·정상 종료, Apple 실기기·다른 빌드 프로필을 확인한다.
2. R2/R3의 실제 IMM 내부 커서·cursor-only·commit·Escape·GCS 없는 취소·result 직후 END와 지연 managed 응답을 확인한다.
3. R4의 실제 ibus/fcitx 입력·취소·reconversion·붙여넣기·formatter·client 전환·화면 반영을 확인한다.
4. R5의 OS IME와 Apple Safari 이벤트 순서·전체 조합 범위·오래된 응답 보호를 실제 managed worker와 연결해 확인한다.
5. 공통으로 실제 조합 중 붙여넣기·formatter·field 전환 및 입력→화면 지연·FPS·프레임 페이싱을 측정한다. 자동 native 검사 통과를 전 플랫폼 입력 문제 해결로 표시하지 않는다.

관련 구현은 [공통 MAUI 브리지](../../../../packages/platforms/maui/Doroti.Host.Maui/MauiTextInputBridge.cs), [Apple 처리](../../../../packages/platforms/maui/Doroti.Host.Maui/MauiTextInputBridge.Apple.cs), [AppKit 구독](../../../../packages/platforms/maui/Doroti.Host.Maui/MauiTextInputBridge.MacOS.cs), [Windows 구독](../../../../packages/platforms/maui/Doroti.Host.Maui/MauiTextInputBridge.Windows.cs), [IMM 상태](../../../../packages/platforms/windowsappsdk/native/src/ime_editing_state.h), [Qt host](../../../../packages/platforms/qt/native/src/doroti_qt_host.cpp), [Web 조합 모듈](../../../../packages/platforms/web/Doroti.Host.Web/Web/doroti.web.text-composition.ts)에 있다.

원시 로그는 당시 `temp/testing/text-input-platform-review/`, `text-editing/platform-review-web-build.log`, `windows-web-text-input/`, `apple-text-input/`, `linux-qt-text-input/` 등에 기록했다. 삭제 가능한 로컬 산출물이며 보관 시 현재 존재·무결성을 확인하지 않았다. 영속 요약·환경·입력 해시·재실행 방법은 위 날짜별 문서를 따른다.

보관 시 기존 상세 기록의 검사 소스 경로 네 곳은 현재 checkout에 없음을 확인했다: `samples/DorotiTestbedApp/macos/AppleTextEditingProbe.cs`, `samples/DorotiTestbedApp/windows/TextInputEditingProbe.cs`, `Doroti/tests/linux_qt_input.py`, `Doroti/tests/native/qt_input_contract.cpp`. 해당 링크와 재실행 명령은 당시 기록이며, 재실행 전 검사 소스 복구 또는 현행 도구 확인이 필요하다.
