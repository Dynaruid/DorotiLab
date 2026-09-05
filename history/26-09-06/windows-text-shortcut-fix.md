# Windows TextField 단축키 복구

## 후속 수정: Windows modifier 매핑

사용자 재확인에서 앞선 수정 후에도 단축키가 작동하지 않았다(**실제 사용 FAIL**). 앞선 PASS는 framework 키를 직접 생성한 검사 범위에 한정되며 Windows 입력의 정상 동작을 증명하지 못했다.

`WindowsKeyMap.Logical`의 Ctrl/Shift 코드가 서로 뒤바뀌어 있었다. 제품의 `LogicalKeyboardKey` 정의는 Ctrl L/R=`0x200000100/101`, Shift L/R=`0x200000102/103`인데, 호스트가 반대로 반환했다. 따라서 실제 Ctrl+A는 framework의 Shift+A로 인식됐다. Windows 매핑을 수정했다.

회귀 검사는 이제 제품 `WindowsKeyMap.cs`를 직접 컴파일해 사용한다. Win32 가상키/스캔코드 및 Ctrl+문자의 제어문자 → KeyData → KeyEventManager → HardwareKeyboard → FocusManager의 부모 노드 전파 → ShortcutManager → 실제 EditableText 액션 맵 → controller 변경을 검증한다. 좌/우 Ctrl 모두 A/C/X/V를 실행하고 키 해제 후 modifier 상태가 해제되는지 확인한다. Ctrl/Shift/Alt/Meta 좌우의 물리·논리 코드도 framework 상수와 비교한다.

- 수정 전 **FAIL 재현**: `Win32 Ctrl+A reaches the focused editing action` (`.doroti/shortcut-win32-before.log`).
- 매핑 수정 후 **PASS** (`.doroti/shortcut-win32-after.log`).
- 전체 FCR-7 검사 **PASS** (`.doroti/shortcut-win32-material.log`).
- Windows 빌드 기록: `.doroti/shortcut-win32-build.log`.
- 실제 OS 입력 주입과 시스템 클립보드/IME는 여전히 **notVerified**. 위 검사는 호스트의 실제 변환 함수를 포함하지만 OS 메시지 주입 및 clipboard endpoint는 포함하지 않는다.

추가 조사에서 BrowserKeyMap, QtKeyMap, MauiNativeInput에도 동일한 Ctrl/Shift 코드 역전이 확인됐다. 이번 후속 패치는 사용자가 실행한 WindowsAppSdk 호스트에 적용했으며, 다른 호스트의 수정과 플랫폼 검증은 남아 있다.

이후 사용자 요청으로 다른 호스트도 수정했다. 결과와 검증 경계는 [플랫폼별 후속 검토](platform-keyboard-shortcut-review.md)에 기록했다. 위 실패 기록은 당시 상태로 보존한다.

## 원인과 수정

- `ShortcutManager._getCandidates`가 항상 빈 목록을 반환해 등록된 단축키가 매칭되지 않았다. 원본 Flutter의 순서대로 해당 키 후보와 null-trigger 후보를 조회한다. 키별 등록 순서와 인덱스 갱신을 보존한다.
- `handleKeypress`가 intent/focus 검사 전에 액션을 조회하고, `Action<SpecificIntent>`를 `Action<Intent>`로 찾으려 했다. C# 제네릭 불변성 때문에 실제 텍스트 편집 액션을 반환하지 못했다. 기존 `IIntentAction` 경로로 조회·실행하고, 타입을 지운 키 처리 결과 브리지를 추가해 `toKeyEventResult` 재정의도 보존한다.
- `_CopySelectionAction`과 `_PasteSelectionAction`은 실행 후 무조건 `Dart control flow completed without a value`를 던졌다. 정상적인 `null` 결과로 종료한다. 복사 액션은 잘라내기도 담당한다.

DemoApp 한정 수정이 아니라 공통 Widgets의 단축키 디스패치 및 편집 액션 수정이다. 기존 컨텍스트 메뉴 작업은 보존했다.

## 검증

모든 테스트 프로세스에 1,200초 제한을 적용했다.

- 수정 전 회귀 검사: **FAIL 재현** — Windows Ctrl+A가 편집 intent를 찾지 못함 (`.doroti/shortcut-contract-before.log`).
- 단축키 회귀 검사: **PASS** — Ctrl+A/C/X/V/Z 매칭, 키 해제 무시, 수정키 조건, 후보 우선순위, null-trigger 후보, 캐시 갱신, scoped dispatcher 1회 실행, 사용자 키 결과 재정의, 비활성 액션, modal 처리, focus/action 없음.
- 실제 `EditableText._actions`를 사용하는 검사: **PASS** — Ctrl+A 전체선택, C/X/V 액션 호출, 잘라내기 후 붙여넣기, 전체선택 후 붙여넣기와 커서 위치. 클립보드 endpoint와 렌더링 부수효과는 테스트 대역이며, 액션 맵·overridable action·선택 업데이트·붙여넣기 reducer·controller는 제품 코드를 사용한다.
- `dotnet run --project Doroti/validation/fcr7-material-widget -c Release -- --shortcuts`: **PASS** (`.doroti/shortcut-contract-editing.log`).
- FCR-7 Material/Widget 전체 검사: **PASS** (`.doroti/shortcut-material-final.log`).
- Windows DemoApp 최종 빌드: **PASS**, 경고 0 / 오류 0 (`.doroti/shortcut-windows-build-final.log`).
- `git diff --check`: **PASS**.
- Windows 실제 키보드/시스템 클립보드/IME: **notVerified**. Computer Use의 `list_apps`와 `list_windows`가 모두 `Computer Use native pipe is unavailable ... (os error 2)`로 실패했다. 이 한계를 framework 회귀 검사로 대체해 PASS 처리하지 않는다. 검증용으로 시작한 앱 프로세스는 종료했다.

저장소 루트 기준 빌드/실행:

```powershell
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 build -App ./DorotiDemoApp -Platform windows
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform windows
```

이 checkout에서 저장소 루트 기준 `-App ../DorotiDemoApp`는 상위 디렉터리의 workspace manifest를 찾다가 실패하므로, 검증에는 `./DorotiDemoApp`를 사용했다.
