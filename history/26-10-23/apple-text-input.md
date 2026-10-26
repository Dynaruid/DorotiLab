# Apple 텍스트 입력·선택·조합 동기화

날짜: 2026-10-23 (Asia/Seoul)

상태: **AppKit·Catalyst·iOS Simulator Release native 각 40개 시나리오 PASS. iOS Release는 LLVM=false. 실제 IME·실기기·전체 위젯 왕복·화면 성능은 미검증.**

## 범위

- 시작 HEAD: `44f2994cccb4ae5b6f13a10e2d190e6a8d32dff6`, 시작 작업 트리 깨끗함.
- 시작 `memo.md` SHA-256: `040ea4d28f23bb6e5ac33e0b67739d8222e662faa1fdc36d4258accb5cb3253d`.
- 당시 `memo.md`의 R1 중 AppKit·Mac Catalyst·iOS와 Apple 상태 응답의 회귀 검사를 다룬다. 검토 내용은 [플랫폼별 입력 검토 요약](../26-10-22/text-input-platform-review-summary.md)에 보존했다. Windows·Android·Qt·Web 제품 구현과 `work.md`는 당시 변경 범위에서 제외했다.
- 빌드·검사는 `Doroti/eng/run-with-timeout.py --timeout 1200`으로 실행한다.

## 구현

[Apple 선택·상태 처리](../../packages/platforms/maui/Doroti.Host.Maui/MauiTextInputBridge.Apple.cs)는 UIKit의 `SelectedTextRange`를 문서 시작점 기준 UTF-16 offset으로 변환하고, AppKit의 현재 field editor/`NSTextView.SelectedRange`를 사용한다. 문서 길이와 범위 유효성을 확인한다. 선택 API·handler·field editor가 없거나 유효하지 않으면 기존 fallback을 유지한다.

계약: [UIKit selectedTextRange](https://developer.apple.com/documentation/uikit/uitextinput/selectedtextrange), [UTF-16 offset](https://developer.apple.com/documentation/uikit/uitextinput/offset(from:to:)), [AppKit selectedRange](https://developer.apple.com/documentation/appkit/nstextinputclient/selectedrange()).

[공통 브리지](../../packages/platforms/maui/Doroti.Host.Maui/MauiTextInputBridge.cs)는 일반 native 텍스트 변경을 같은 callback에서 전달한다. 선택만 바뀐 알림은 한 UI turn으로 합치고 현재 문자열·native 선택·marked range를 함께 읽는다. 예전 pending 문자열과 새 선택을 묶어 전달하지 않는다.

Apple native 상태와 같은 echo는 다시 쓰지 않는다. callback에서 요청한 formatter 응답은 native 입력 처리 뒤 적용한다. 입력 revision·client generation·요청 순서를 확인하고, 아직 발행되지 않은 선택/marked 변화도 확인한다. 후속 입력이나 새 client를 이전 응답이 덮지 않으며 같은 입력에 여러 formatter 응답이 있으면 마지막 요청을 적용한다. UIKit의 기존 client/focus mutation 지연은 유지한다.

[UIKit native 입력창](../../packages/platforms/maui/Doroti.Host.Maui/DorotiUIKitTextInput.cs)은 `SetMarkedText`/`UnmarkText`가 끝난 뒤 선택·조합 상태를 발행한다. Catalyst에서 문자열과 선택이 그대로인 `UnmarkText`가 MAUI 변경 알림을 만들지 않는 반례를 확인해 보완했다. 제품 코드가 native 전체 문자열을 추가로 읽는 경로는 넣지 않았다.

이 보호는 브리지에 요청이 들어온 뒤 큐에서 대기하는 상태와 native callback의 응답을 대상으로 한다. 외부에서 응답 자체가 지연되어 다음 입력 뒤 처음 `UpdateState`를 호출하는 경우에는 원래 입력 번호를 API로 전달하지 않으므로, 모든 비동기 transport의 오래된 응답을 식별한다고 해석하면 안 된다.

### AppKit에서 추가로 확인한 경로

`NSTextView.SetMarkedText`가 field editor를 바꾸어도 MAUI `TextChanged`가 오지 않는 경우를 native 검사에서 확인했다. [AppKit 구독](../../packages/platforms/maui/Doroti.Host.Maui/MauiTextInputBridge.MacOS.cs)은 field editor의 선택·텍스트·storage 알림을 구독한다. 문자 편집 dirty 상태를 따로 추적하며, MAUI가 이미 materialize한 문자열은 재사용한다. MAUI에 반영되지 않은 native 문자 변경만 storage revision당 한 번 읽는다. 선택/marked 속성만 바뀌면 캐시 문자열을 사용한다.

따라서 아직 MAUI `Text`에 없는 preedit도 발행하고, 같은 값의 echo가 marked text를 없애지 않게 한다. 명시적인 framework 변경은 활성 field editor에도 적용한다. `NSTextField.StringValue`만 바꾸어 native 편집 버퍼가 이전 값을 유지하는 formatter 경로도 검사했다. handler 교체·종료 시 observer를 해제한다.

## native 검사

[검사 소스](../../samples/DorotiTestbedApp/macos/AppleTextEditingProbe.cs)는 runner 경계 때문에 iOS 디렉터리에 같은 내용으로 보존하며 `apple_build_profiles.py`에서 parity를 확인한다. opt-in 환경 변수는 `DOROTI_APPLE_TEXT_EDITING_PROBE`, smoke case는 `text-editing`이다. 기존 WebView/semantics `features` 검사와 따로 실행할 수 있다.

실제 native 입력창에서 `InsertText`/Backspace, `SetMarkedText`/`UnmarkText`, selection 명령을 호출한다. **모든 bridge callback**을 그 순간 native 문자열·선택·조합과 비교하고 최종 전달 상태도 확인한다. 일반 문자열 삽입·삭제는 각 명령이 같은 UI turn에 callback을 발행해야 한다. 선택·조합 알림은 정규화 뒤의 상태를 검사한다. 관측용 native 문자열 읽기는 검사 코드의 비용이며 제품의 추가 문자열 읽기 횟수와 구분한다.

검사 client가 echo와 formatter 응답을 직접 돌려준다. 실제 `EditableText`/Material/Cupertino 위젯의 입력부터 그림까지 전체 왕복을 실행한 검사는 아니다.

40개 시나리오는 Entry/Editor/secure Entry의 반복 문자·공백·emoji·선택 교체와 각각 12회 연속 삽입/Backspace, 일반 Entry/Editor의 조합 내부 커서·echo·확정·동일 문자열 unmark·빈 preedit 취소·formatter·복수 formatter·선택 formatter·후속 입력/새 client 뒤 오래된 응답·조합 중 재표시/endpoint 전환을 포함한다. 준비는 고정 지연 대신 client mutation 완료와 실제 native focus를 기다린다. 이 명령들은 OS 키보드나 실제 한/일/중 IME를 구동한 증거가 아니다.

`ShowTextInput`/configuration/attach는 native 문자열·선택·조합을 함께 캡처한다. AppKit의 같은 handler 재부착은 preedit 캐시를 초기화하지 않으며, field editor가 아직 설치되지 않은 전환 구간에는 선택 알림을 발행하지 않는다. UIKit의 이미 first responder인 입력창에는 다시 focus를 요청하지 않는다.

## 확인 결과

환경은 macOS 27.0 arm64, .NET SDK 10.0.401, Apple workload 27.0.10722다. 일반 MAUI는 10.0.90이며 AppKit은 저장소에 고정된 MacOS preview 패키지를 사용한다.

| 검사 | 결과 | 범위 |
| --- | --- | --- |
| AppKit Release / `net10.0-macos27.0` / `osx-arm64` | 40개 시나리오·124개 편집/상태 단계·145개 callback, 불일치 0 | Entry/Editor/secure Entry. 일반 편집 90회에서 제품의 추가 native 전체 문자열 읽기 0회. 검사 후 정상 종료. |
| Catalyst Release / `net10.0-maccatalyst27.0` / `maccatalyst-arm64` | 40개 시나리오·124개 편집/상태 단계·132개 callback, 불일치 0 | UIKit native 조합·echo·formatter·client/configuration 전환. 검사 뒤 runner가 프로세스를 종료하며 앱의 정상 종료 인수 검사와 구분한다. |
| iOS Debug / `net10.0-ios27.0` / `iossimulator-arm64` | 40개 시나리오·124개 편집/상태 단계·130개 callback, 불일치 0 | 격리 iPhone 17 / iOS 27.0 Simulator. 실기기·Release AOT 실행·전체 위젯 왕복·화면 성능 제외. |
| iOS Simulator Release native bridge | 40개 시나리오·124개 편집/상태 단계·132개 callback, 불일치 0 | iPhone 17 / iOS 27.0 Simulator. Mono AOT·LLVM=false. 실기기·기본 LLVM=true 프로필·전체 위젯 왕복·화면 성능 제외. |
| iOS Simulator Release Mono AOT 빌드 | PASS, 오류 0개 | `MtouchUseLlvm=false` 명시. `Optimize=true`, `MtouchInterpreter=-all`, `TrimMode=copy`. 18분 54.93초. 기존 `PublishFolderType` 경고 321개. 위 Release native 검사에서 실행했다. |
| probe 소스 parity / Python 구문 | PASS | iOS/macOS의 두 probe가 동일하고 runner 세 파일을 `py_compile`로 확인. |
| Apple 빌드/개발 프로필 계약 | PASS | `apple_build_profiles.py`. native-aot·simulator/device development·잘못된 Release/인터프리터 opt-out 혼합 거절·일반 Debug/Release 분리. Native 실행과 별도. |

AppKit/Catalyst Release는 `Optimize=true`다. 평가된 AppKit 프로필은 CoreCLR(`UseMonoRuntime=false`), `TrimMode=copy`이고 Catalyst는 Mono(`UseMonoRuntime=true`), `MtouchInterpreter=-all`, `TrimMode=partial`이다. `PublishAot`가 비어 있는 프로필을 NativeAOT로 표시하지 않는다. 빌드에 기존 `PublishFolderType` license/notice 복사 경고가 남았으며 최종 두 빌드는 오류 0개다. 상세 평가 값은 로컬 `release-profiles.json`에 보존했다.

최종 원시 결과는 `temp/testing/apple-text-input/appkit-release-final/text-editing.json`, `catalyst-release-recheck/text-editing/result.json`, `ios-debug-final/text-editing/result.json`, `ios-release-final/text-editing/result.json`이다. Native 결과의 124는 probe의 편집/상태 단계 카운터 합계이며 준비 선택이나 모든 formatter 응답의 개수를 뜻하지 않는다. 이전 검사 준비 실패·기동 timeout은 최종 PASS와 구분한다. iOS는 설치/launch와 system Safari launch도 timeout이 발생해 CoreSimulator 실행 서비스를 재시작하고 fixture 소유 기기에서 최종 Debug 검사를 통과했다. Release는 원래 iPhone 17 Simulator에서 통과했다. 별도 생성한 fixture 기기는 삭제했고, 원래 기기는 시작 상태인 Shutdown으로 돌렸다.

## 보존 해시 (SHA-256)

현재 제품 소스와 실제 실행한 Mac 산출물을 별도로 기록한다. AppKit probe 뒤 추가한 `.launch` 진단은 검사 시작 지점만 기록하며 입력 검사 동작을 바꾸지 않는다.

| 대상 | 해시 |
| --- | --- |
| MauiTextInputBridge.cs | `69ed9518be352050d10d3f877bd2784225826713fcc480a40afe31911a26bb14` |
| MauiTextInputBridge.Apple.cs | `0c9acba86464c92cc0298980c3b4b9052a1ef9ad00924e113a8c9c9489c5b69a` |
| MauiTextInputBridge.MacOS.cs | `cfa3f66df2138453dedbd0ed5f1a79d2178fbee3b8de30d2acf09d7b5aca19f6` |
| DorotiUIKitTextInput.cs | `009bf3776167785f96a8df54407184e7c773190418df30ae2778f6c0935ec5b2` |
| AppleTextEditingProbe.cs (양쪽 동일) | `a915a44c9eb6c710124f873d7cf045cdcf97cdfbe7d9b03bad2d853ed5902a6f` |
| AppKit 실행 Host DLL | `6e2dcf12bc1239443f8352c07f137e66318dd2b808fddeb50a1244236fe4b707` |
| AppKit 실행 native executable | `b77587b462990c3310e13c57a9dd1fcada0cdb72686de18725de46b201bd3a28` |
| Catalyst 실행 Host DLL | `4285bcab747a4dd87a44d6e139d6dad249bf8d9940539162f921a5475446105f` |
| Catalyst 실행 native executable | `d58571f1b3dcfb3e3a0cf11d35edacfd45b9efd6b6743890829daa18a261421d` |
| iOS Debug 실행 Host DLL | `d12b959000ab91bca395b407f58a3a651c3349e4cdbc49d9f3df0517a5505db0` |
| iOS Debug 실행 native executable | `4fa3dc4bb408c0327a06efe785b730d4a724abaaf45624c34e1b1a69a2945fb0` |
| iOS Release 실행 Host DLL | `6ed54c91d6488f3e9c341b6d8c88534aedb5f51e53887d49cbc04c0346f7bcc7` |
| iOS Release 실행 native executable | `73fdf56d3ad286761b2141cee9e87d660b18765826fa2874802ea43c02cf4723` |

## 재실행

새 output 디렉터리를 사용한다. `--configuration`의 기본값은 Debug이며 Release를 명시할 수 있다.

```sh
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/tests/macos_smoke.py --configuration Release --cases text-editing --output temp/testing/apple-text-input/appkit-new
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/tests/apple_smoke.py --target maccatalyst --configuration Release --cases text-editing --output temp/testing/apple-text-input/catalyst-new
python3 Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build samples/DorotiTestbedApp/ios/DorotiTestbedApp.iOS.csproj -c Release -r iossimulator-arm64 -p:DorotiIosTargetFramework=net10.0-ios27.0 -p:MtouchUseLlvm=false
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/tests/apple_smoke.py --target ios --configuration Release --skip-build --cases text-editing --output temp/testing/apple-text-input/ios-new
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/tests/apple_build_profiles.py
```

물리 IME·iPhone/iPad 실기기·화면 입력 지연·FPS·프레임 페이싱은 별도 검증 대상이다.
