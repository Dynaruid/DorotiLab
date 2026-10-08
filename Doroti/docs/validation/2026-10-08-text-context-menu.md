# 텍스트 컨텍스트 메뉴 검토 및 수정

2026-10-08. 범위: Material/Cupertino 텍스트필드, 공통 EditableText, 네이티브 텍스트 작업 연결.

## 구조와 원인

Material `TextField`와 `CupertinoTextField`의 기본 `contextMenuBuilder`가 각 라이브러리의 adaptive toolbar를 만든다. 버튼 목록·선택 범위·복사/잘라내기/붙여넣기/전체 선택은 공통 `EditableTextState`가 담당한다. Share/Look Up/Search Web은 `flutter/platform` 메서드를 통해 플랫폼 호스트로 전달된다. iOS의 시스템 메뉴 경로는 별도의 UIKit 구현을 사용한다.

Web에는 `BrowserTextActions`로 Share/Search Web이 연결되어 있었다. 네이티브 MAUI에는 같은 플랫폼 메서드의 구현이 없었다. `SystemChannels.platform`이 `OptionalMethodChannel`이어서 미구현 호출은 null로 끝났고, Android의 Share 버튼은 표시되어도 아무 UI가 열리지 않았다. iOS의 기존 **시스템 메뉴**에는 구현이 있지만, 프레임워크가 렌더링하는 메뉴·custom builder 경로에도 같은 연결 누락이 있었다.

Windows/Linux/macOS의 기본 메뉴에 Share가 없는 것은 기존 Flutter 플랫폼 분기다. 이번 수정으로 데스크톱 기본 버튼 목록을 확대하지 않는다. CupertinoTextField는 Android에서도 Cupertino 모양의 메뉴/핸들을 유지한다.

## 변경

- `TextActionPlatformMessageCapability`가 Share/Look Up/Search Web 요청을 검증하고 네이티브 함수로 전달한다. 공백 선택은 UI를 열지 않고, 네이티브 오류는 JSON 오류 envelope로 반환하며, 다른 메서드와 채널은 기존 경로로 전달한다.
- Android Share는 UI 스레드에서 MAUI Share API를 호출한다. iOS/Mac Catalyst의 렌더링된 메뉴 경로에는 UIKit 사전/공유 및 웹 검색을 연결한다. UIKit 공유는 해당 view의 window를 사용하고 iPad popover anchor를 지정한다. 기존 UIKit 시스템 메뉴 구현은 유지한다.
- `DefaultProcessTextService` 생성자에서 빠진 `SystemChannels.processText` 초기화를 복구하고, Release에서도 테스트 채널 교체가 실행되게 한다.
- Android `ACTION_PROCESS_TEXT` 앱 조회·선택 실행·결과 반환을 연결한다. 내보낸, 활성화된, 호출 권한이 있는 처리 앱만 노출하며, private relay Activity가 결과와 취소를 전달한다. 앱별 처리 ID는 package/component를 포함한다.
- Android 11+ package visibility에 필요한 PROCESS_TEXT manifest overlay를 로컬 provider bootstrap과 배포용 host `buildTransitive`에 제공한다.
- 메뉴 작업 오류를 보고한다. 처리 앱을 기다리는 동안 텍스트/선택이 바뀌거나 필드가 사라지면 오래된 결과를 적용하지 않는다. Paste도 clipboard 응답 후 unmount 여부를 확인한다. 비동기 처리 앱 목록 로딩 뒤 메뉴를 갱신한다.

## 검증

| 항목 | 결과 | 범위 |
| --- | --- | --- |
| 네이티브 텍스트 작업 채널 | PASS | Unicode 선택 보존, 빈 입력, 잘못된 인자, 오류 envelope, 취소, 다른 요청 전달, ProcessText 기본 채널/Release 교체 |
| Material/Cupertino 공통 메뉴 | PASS | 각각 Android/iOS/Windows/Linux/macOS 환경의 CPU widget 테스트: 버튼 분기, 한글·이모지 Copy/Cut/Paste/Select All, 처리 결과의 선택 범위 교체와 오래된 결과 거부, 읽기 전용/비밀번호 제한 |
| Android x64 앱 빌드·설치 | PASS | DorotiSampleApp2 Debug, EmbedAssembliesIntoApk=true, 최종 생성 manifest의 PROCESS_TEXT 조회 항목 확인 |
| Android 네이티브 Share | PASS | Android 13/API 33, emulator-5554. Fonts의 `입력과` 선택 → 시스템 ChooserActivity → 공유 미리보기 `입력과` 확인. 취소 후 선택 유지. 최종 APK로 재확인. 외부 수신 앱으로 전송하지 않음 |
| Android 네이티브 Cut/Paste | PASS | 메뉴 Cut 후 native EditText 값 `한글  선택 · Hello 0123456789`; 공백 범위 선택 후 메뉴 Paste 결과 `한글입력과선택 · Hello 0123456789` |
| Android 네이티브 ProcessText | PASS | 임시 처리 앱 메뉴 조회/표시, 실제 Activity 왕복, 입력 `입력과`, readOnly=False, 결과 `[처리]입력과`; 최종 값 `한글 [처리]입력과 선택 · Hello 0123456789`. 테스트 앱은 검증 후 제거 |
| 네이티브 호스트 컴파일 | PASS | Android ARM64, iOS simulator x64, Mac Catalyst ARM64, Windows MAUI x64, Release |
| Host NuGet manifest 배포 | PASS | Android ARM64 범위의 pack 후 archive에서 buildTransitive targets와 PROCESS_TEXT overlay 파일 및 참조 확인. 전체 플랫폼 패키지 릴리스 검증은 아님 |
| 플랫폼 contract suite | PASS | 전체 headless suite: 기존 다중 view/공유 session/취소/텍스트 입력/typed transport 계약과 새 텍스트 작업 회귀 |
| 기존 텍스트 커서 회귀 | PASS | `--text-input-tap-cursor` CPU suite |
| Web | 소스 검토 | 기존 browser Share/Search Web·fallback 경로 유지. 이번 작업에서 browser runtime 검증은 수행하지 않음 |
| 물리 Android, iOS/iPad/macOS 런타임 | notVerified | 에뮬레이터와 host 컴파일은 물리 입력·다른 IME·Apple 시스템 UI 실행 증거를 대신하지 않음 |

각 검증 명령은 `.github/copilot-instructions.md`에 따라 1200초 timeout wrapper로 실행했다. Android 초기 clean build에는 기존 Gradle SDK XML version 경고 1개가 있었고, 최종 incremental build와 나열한 host 컴파일은 경고/오류 0개였다.

최종 Android APK SHA-256: `9CD0CF3E0E939AC3328C25ACFC186B16D6DB510CA6BD16DE193FADEC30AF8E4C`.

원시 screenshot/UI XML과 임시 처리 앱 소스는 `temp/testing/text-context-menu/2026-10-08/`에 있다. 삭제 가능한 로컬 산출물이며, 이 문서의 결과와 범위는 해당 파일의 영구 보존을 전제하지 않는다.

재실행 명령:

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Platform.Contracts.Tests -c Release -- --text-actions
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project packages/Doroti.Material/tests/Doroti.Material.Tests -c Release -- --editable-context-menu
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project packages/Doroti.Cupertino/tests/Doroti.Cupertino.Tests -c Release -- --editable-context-menu
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build samples/DorotiSampleApp2/android/DorotiSampleApp2.Android.csproj -c Debug -r android-x64 -p:EmbedAssembliesIntoApk=true -v:q
```

비교한 공식 구현: [Flutter EditableText](https://raw.githubusercontent.com/flutter/flutter/stable/packages/flutter/lib/src/widgets/editable_text.dart), [Flutter ProcessText](https://raw.githubusercontent.com/flutter/flutter/stable/packages/flutter/lib/src/services/process_text.dart), [Flutter Android ProcessTextPlugin](https://raw.githubusercontent.com/flutter/engine/main/shell/platform/android/io/flutter/plugin/text/ProcessTextPlugin.java), [MAUI Share](https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/data/share).
