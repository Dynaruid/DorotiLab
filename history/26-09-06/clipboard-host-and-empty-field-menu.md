# 클립보드 호스트 연결과 빈 텍스트필드 메뉴 후속 수정

이후 다른 플랫폼 검토에서 Web 상태 속성, Qt await continuation, MAUI UI 스레드/상태 조회를 추가 수정했다. [플랫폼별 후속 기록](clipboard-cross-platform-followup.md)을 참고한다. 아래 기록은 당시 검증 범위로 보존한다.

## 실제 누락

사용자가 Windows에서 복사·붙여넣기가 여전히 안 되고 빈 필드 우클릭 메뉴도 안 나온다고 보고했다.

공통 `Clipboard.setData/getData/hasStrings`는 `flutter/platform`의 OptionalMethodChannel을 호출했지만 Windows/Qt/MAUI/Web 호스트에 해당 clipboard method handler가 없었다. 응답이 null이면 쓰기는 성공처럼 끝나고, 읽기는 null, hasStrings는 false가 된다. 따라서 단축키가 EditableText 액션에 도착해도 OS 클립보드는 호출되지 않았다. 빈 필드에서는 copy/cut/selectAll도 없어 메뉴 항목이 0개가 됐다.

이전 `KeyboardShortcutContracts`는 `EditableTextState.copySelection/cutSelection/pasteText`를 대역으로 교체했다. 이전 PASS는 키 전달과 액션 매칭에 대한 증거이며 실제 클립보드 연결을 증명하지 않는다. Web 실제 검사는 네이티브 textarea의 클립보드 동작을 검증했으므로 공통 Services 경로의 누락을 발견하지 못했다.

## 변경

- Clipboard 공개 API를 기존 `IPlatformServicesHostCapability`에 연결했다. 마우스 커서와 같은 attached-view capability 선택 방식을 사용한다. 네 호스트가 모두 이 기능을 등록하는 것을 확인했다. unsupported format은 null, null/빈 text는 hasStrings false다.
- Windows 네이티브 쓰기에서 `OpenClipboard`에 실제 입력 HWND를 전달한다. [Microsoft OpenClipboard 문서](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-openclipboard)는 null HWND로 열고 EmptyClipboard를 호출하면 SetClipboardData가 실패하는 소유권 조건을 설명한다. 이 부분은 문서상 API 위반을 수정했으며, 기존 바이너리의 실패를 별도로 재현했다고 주장하지 않는다.
- Windows/Linux/macOS 기본 메뉴의 빈 편집 가능 필드는 클립보드가 비었거나 조회 중이어도 비활성 Paste 항목을 유지한다. 내용이 있으면 활성 Paste를 제공한다. 읽기 전용·selection disabled·legacy toolbarOptions의 명시적 설정은 보존한다.
- 회귀 검사에서 실제 Clipboard/EditableText 메서드를 실행하고 OS 경계만 대역으로 바꾼 경우를 추가했다. 외부 텍스트 유입에 따른 paste 상태 갱신도 검사한다.
- 기존 Windows native product fixture도 Services Clipboard 쓰기·읽기·hasStrings를 포함하도록 확장했다.

## 검증

모든 빌드/검사 프로세스 제한은 1,200초다.

| 검사 | 결과 |
| --- | --- |
| 수정 전 Clipboard 복원 + 미구현 채널의 실제 null 응답 + Ctrl+C | **FAIL 재현**: `Ctrl+C reaches the clipboard endpoint once`, `.doroti/clipboard-host-before.log` |
| 최종 전체 FCR-7 Material/Widget | **PASS**, `.doroti/clipboard-menu-material.log` |
| 실제 EditableText 액션 → Clipboard → 등록된 호스트 | **PASS**: Ctrl+A/C/X/V, 전체선택 교체, 빈 필드의 Paste 활성/비활성, 외부 클립보드 내용 갱신 |
| 6개 플랫폼 메뉴 상태 검사 | **PASS**: 빈 필드, clipboard unknown/notPasteable/pasteable, 읽기 전용, disabled callback 포함 |
| Windows native host 빌드 | **PASS**, `.doroti/clipboard-native-build.log` |
| 실행 중 Windows 호스트 + 실제 OS Unicode clipboard | **PASS**: Services 쓰기/읽기/hasStrings 및 직접 호스트 읽기 일치, `.doroti/clipboard-native-product.json` |
| 사용자 CLI 경로의 Windows DemoApp build | **PASS**, 경고 0/오류 0, `.doroti/clipboard-menu-windows-build.log` |
| native source output ↔ DemoApp에 복사된 DLL SHA256 | **일치**: `cfbf6dffcf31571068da3760d9a4da06c95d29c87308d6c2bcb00b87902051f2` |

실제 Windows 키보드/우클릭 메뉴 화면 조작은 **notVerified**다. Computer Use 초기화 뒤 `list_apps`가 `Computer Use native pipe is unavailable ... (os error 2)`로 실패했다. OS clipboard 왕복과 프레임워크 회귀 검사를 실제 화면 조작 PASS로 간주하지 않는다. 네이티브 product fixture는 정상 종료했다. 다른 플랫폼의 실제 기기 입력은 이번 검사에 포함하지 않았다.

저장소 루트 `C:\Users\parti\Labo\DorotiLab`에서 수정된 앱 실행:

```powershell
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform windows
```

이 루트에서 `../DorotiDemoApp`는 상위 디렉터리를 가리켜 workspace manifest 조회에 실패한다. 빌드는 `./DorotiDemoApp`로 수행했다. 이미 떠 있는 앱에는 DLL 변경이 소급 적용되지 않으므로 새 실행이 필요하다.
