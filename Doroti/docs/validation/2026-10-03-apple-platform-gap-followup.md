# Apple 플랫폼 기능 공백 후속 검토 — 2026-10-03

대상은 보관된 [W0~W11 작업 요약](../../../history/26-10-03/platform-gap-summary.md)의 AppKit macOS / Mac Catalyst / iOS다. 기준 HEAD는 `55d075af1b6bc2fac59b9a8c811b985d0291c3e4`이며 변경은 미커밋이다. [앞선 전체 구현](2026-10-03-platform-gap-implementation.md)의 Apple SKIPPED 이력은 보존하고 이번 후보를 별도로 검증한다. 제품 인수는 PARTIAL이다.

## 발견한 공백과 보강

- **W2 입력:** 같은 MAUI Entry에 암호/keyboard 설정을 갱신할 때 native selection이 바뀌어도 이전 선택을 복원하지 않았다. Catalyst 실행에서 `한글😀abc`의 선택 `2:4`가 cursor `7` / length `2`로 바뀌는 실패를 확인했다. 변경된 상태만 복원하고 handler 연결·포커스 적용 뒤에도 선택을 보존한다. iOS에서 endpoint 재부착 뒤 native 암호/Return trait 검사가 실패한 후보도 보존하며, 관리 속성이 이미 같은 값인 경우에도 최종 native endpoint에 trait을 다시 적용한다. AppKit deferred focus는 focus 호출 전에 선택을 복사한다.
- **W2 UIKit action:** Editor에는 MAUI ReturnType mapper가 없고 MAUI Editor.Completed는 편집 종료에서도 발생한다. native ReturnKeyType을 Entry/Editor 모두에 적용하고, custom UITextView의 실제 `InsertText("\n")`를 framework action에 연결했다. newline/unspecified/none과 marked text는 native 편집을 유지한다. 설정·client 전환/종료 중 callback, 교체된 handler의 callback을 거절하고 dispose에서 연결을 해제한다. endpoint 교체의 composition 보존·selection 방향 지원은 기존 false 계약을 유지한다.
- **W5 빌드:** 기존 UIAccessibilityCustomAction의 lambda가 두 생성자 오버로드와 일치해 Catalyst 빌드가 CS0121로 실패했다. iOS도 공유하는 코드다. 명시적인 delegate로 해결했다.
- **W5 AppKit:** 기존 새 semantics 컨테이너에는 AppKit provider가 없었고 preview MAUI backend의 layout 명령 보강도 surface에만 등록돼 있었다. AbsoluteLayout과 semantics handler를 연결하고 native 역할·상태·자식 순서·press/increment/decrement/cancel·custom actions와 text value/expanded setter를 framework에 전달한다. proxy child를 접근성 자식에서 제외하고 종료·삭제·disabled/readOnly/password action을 거절한다. macOS 26 이전 heading은 static text로 표시한다.
- **W5 UIKit:** 내부 proxy를 숨긴 뒤 기본 activate/increment/decrement/scroll 동작을 전달할 provider가 없었다. 전용 native layout에 기본 동작과 custom actions를 연결했다. UIAccessibilityContainer의 실제 count/index API와 list/table container type에 semantic children만 전달하며 traits를 조합한다.
- **W5 암호:** Entry.Text와 label/button/description fallback에 obscured value가 복사될 수 있었다. native proxy와 Apple accessibility value에서 이를 제거한다. 암호 copy/cut은 공통 policy에 따라 거절한다.
- **W3 검증:** 기존 Apple 공통 WebKit session의 명령 연결은 존재했지만 새 후보의 Apple 검사가 없었다. 실제 AppKit/UIKit factory를 통해 HTML/JS, null/undefined/error, app resource scheme, message, document generation, ClearData와 close 거절을 검사하는 testbed probe를 추가했다. WebView를 별도로 구현하지 않았다.
- **검사 진입점:** `features`를 AppKit/UIKit 기본 smoke에 추가했다. runner는 플랫폼 디렉터리 밖 source를 거절하므로 probe를 ios/macos에 동일하게 보관하고 `apple_build_profiles.py`에서 parity를 검사한다. iOS cold simulator의 launch가 기존 30초를 초과한 실패를 보존하고 launch에 120초를 허용하며 실패해도 해당 앱을 정리한다. 전체 명령의 timeout은 1,200초다.

기술 근거: [Apple NSAccessibilityProtocol](https://developer.apple.com/documentation/appkit/nsaccessibilityprotocol), [UIKit semantic group](https://developer.apple.com/documentation/uikit/uiaccessibilitycontainertype/semanticgroup), [MAUI 10.0.90 EditorHandler](https://github.com/dotnet/maui/blob/10.0.90/src/Core/src/Handlers/Editor/EditorHandler.iOS.cs). 런타임에 제공하는 capability의 역할/action subset과 실제 AT 인수는 구분한다.

## 검증

환경: macOS 26.6.2 arm64 / Apple M1, .NET SDK 10.0.401, Xcode 27.0 (27A266a), Apple workload 27.0.10722. 앱 TFM은 `net10.0-macos27.0`, `net10.0-maccatalyst27.0`, `net10.0-ios27.0`; RID는 각각 `osx-arm64`, `maccatalyst-arm64`, `iossimulator-arm64`다.

최종 결과와 source/payload identity는 [JSON](2026-10-03-apple-platform-gap-followup.json)에 보존한다. 모든 최종 앱 빌드는 경고 0 / 오류 0으로 통과했다.

| 검사 | 결과 | 범위 |
| --- | --- | --- |
| AppKit Debug build + Graphite native smoke | PASS, 9개 결과 | OnLastWindowClosed/Explicit 두 창·생존 창 resize, desktop/close, services, input, 새 features, navigation/restoration, 24회 resize/rendering, lifecycle/최종 owner drain |
| Catalyst Debug build + Graphite native smoke | PASS, 6 case | services, input, 새 features, LaunchServices navigation, restoration, 실제 두 scene/multiwindow |
| iOS simulator Debug build + Graphite native smoke | PASS, 6 case | 최종 `ios-features-3`의 features 및 `ios-regressions`의 input/services/rotation/scene callback navigation/restoration. 실제 iPhone 인수와 구분 |
| Native feature probe | 세 플랫폼 PASS | 실제 factory WebKit 명령·HTML/JS·app resource/message·stale generation/close; semantic hierarchy/order/role·native action·disabled/password/removal/dispose. UIKit은 native selected range/secure/Return trait, Search Return 처리·전환 중 비의도 action 없음까지 assertion |
| 공통 Doroti.Tests CPU 계약 | PASS | 기존 hierarchy/action/readOnly/password·UTF-16/layout geometry·owner/generation·filter/URL·budget 등 전체 기존 suite |
| Apple MSBuild profiles / final probe parity | PASS | device/simulator Debug·Release/NativeAOT 설정 평가, Mac development 조건/거절 및 템플릿·probe parity. device 실행 증거가 아님 |
| Source / Python / diff | PASS | root work.md 포함 링크 검사, runner/installer contracts, smoke/profile/validate Python compile, git diff --check |

최종 native raw run은 `appkit-final-2`, `catalyst-final`, `ios-features-3`, `ios-regressions`다. 먼저 실패한 후보와 중간 통과 후보를 최종 payload의 결과로 합산하지 않는다. iOS 최종 features의 C pipeline은 완료 4 / pending 0 / 최대 1을 기록했으며 짧은 callback 수를 표시 FPS로 환산하지 않는다. 기존 Ganesh 후보의 결과도 이번 Graphite 결과에 합산하지 않는다.

JSON의 bundle digest는 전체 bundle에서 `is_file()`인 경로를 정렬하고 각 상대 경로·byte 길이·파일 SHA256을 NUL/줄바꿈으로 구분해 다시 SHA256한 값이다. 주요 host DLL/실행 파일/plist hash도 함께 보존한다. source digest는 변경한 C#/Python 파일의 상대 경로와 SHA256으로 계산하며 문서 파일은 제외한다.

재현 명령:

```sh
python3 Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build samples/DorotiTestbedApp/macos/DorotiTestbedApp.MacOS.csproj -c Debug -r osx-arm64 -p:DorotiMacOSTargetFramework=net10.0-macos27.0
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/tests/macos_smoke.py --skip-build --output temp/testing/platform-audit/apple-followup/reproduce-appkit
python3 Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build samples/DorotiTestbedApp/macos/DorotiTestbedApp.MacCatalyst.csproj -c Debug -r maccatalyst-arm64 -p:DorotiMacCatalystTargetFramework=net10.0-maccatalyst27.0
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/tests/apple_smoke.py --target maccatalyst --skip-build --output temp/testing/platform-audit/apple-followup/reproduce-catalyst
python3 Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build samples/DorotiTestbedApp/ios/DorotiTestbedApp.iOS.csproj -c Debug -r iossimulator-arm64 -p:DorotiIosTargetFramework=net10.0-ios27.0
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/tests/apple_smoke.py --target ios --skip-build --activation native-callback --cases features,input,services,rotation,navigation,restoration --output temp/testing/platform-audit/apple-followup/reproduce-ios
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/tests/apple_build_profiles.py
python3 Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Tests/Doroti.Tests.csproj -c Debug --artifacts-path temp/testing/platform-audit/apple-followup/contracts
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/eng/validate.py Source
```

## 남은 인수와 OS 제한

물리 한글 IME·VoiceOver·pen, 실제 monitor 이동/DPR·표시 FPS·장기 사용, 실제 iPhone Release/NativeAOT, package-only 새 소비 앱·서명/공증·clean OS 배포는 이번 자동 검사의 통과 범위에 포함하지 않는다. UIKit native glyph range/MAUI text geometry는 false다. UIKit semantics의 setText/setSelection/focus는 지원 action 목록에 없으며 이 subset을 완전한 VoiceOver 텍스트 편집 지원으로 확대하지 않는다. iOS URL 검사는 scene callback 주입이며 OS association 인수가 아니다.

Catalyst의 native scene close 취소·전역 위치·topmost/Dock 제어 등은 작업 요약의 명시적 제한을 유지한다. `RequireNativeCloseCancellation`을 충족하는 것처럼 광고하지 않는다. programmatic CloseAsync와 autosave/recovery 경로, AppKit의 application-scoped Dock 정책 및 위치 제한도 구분한다. 이 제한을 구현 누락이나 전체 PASS로 바꾸지 않는다.

원시 파일은 `temp/testing/platform-audit/apple-followup/`와 `temp/apple-*.log`의 삭제 가능한 로컬 산출물이다. 필요한 결과·실패·identity는 추적 문서/JSON에 보존한다.
