# iOS / Mac Catalyst 후속 구현·검증 — 2026-09-29

작업 상태: **PARTIAL**. `work2.md`의 두 플랫폼 전체 범위를 검토하고 자동화 가능한
구현·실행 경로를 보강했다. 물리 입력·보조기술·배포를 자동 실행으로 완료 처리하지 않는다.
기준 revision: `a5fe97d7e03c1596250ed68f46a3ec311556fab9` + 이번 작업 트리.

## 환경

macOS 26.6.2 / Apple Silicon, .NET SDK 10.0.400, Xcode 27.0 프로필.
Catalyst `net10.0-maccatalyst27.0` / `maccatalyst-arm64`, iOS
`net10.0-ios27.0` / `iossimulator-arm64`, iPhone 18 Pro Simulator / iOS 27.0.
Debug/Mono이며 iOS 호스트는 기존 프로필의 선택적 Mono AOT를 사용한다.
모든 테스트 명령은 `Doroti/eng/run-with-timeout.py --timeout 1200`으로 제한했다.

물리 iPhone 12 / iOS 26.6.1은 연결·Developer Mode를 확인했다. 최초 `ios-arm64` 빌드는
앱에 맞는 provisioning profile 부재로 서명 단계에서 실패했다. 후속 사용자 요청으로 Xcode
자동 프로비저닝에 성공했다. 이어진 **“ios실기기는 지금은 생략”** 지시에 따라 실기기 추가
빌드·설치·실행은 진행하지 않았다. 물리 IME/VoiceOver는 **notVerified / 사용자 요청으로 보류**다.

## 구현

- Catalyst에 UIKit Graphite PlatformView factory/합성/해제와 native editor·WKWebView,
  UIKit 텍스트 입력·편집 메뉴를 연결했다. raw text/HTML과 공통 sample의 JSON 초기화 값을 지원한다.
- 창별 UIView·Metal queue·dispatcher·PlatformView를 소유하고 application boundary만
  reference counting으로 공유하는 추가 scene factory를 구현했다. API close는 framework
  unmount → GPU drain → scene destruction 순서다. 비활성 desktop 창은 계속 그릴 수 있다.
- UIKit keyboard repeat/up을 공통 pressed-key tracker로 정규화하고 포커스 해제·입력 취소 때
  눌림을 해제했다. 실제 한글 IME·Tab·외부 키보드 판정과 구분한다.
- UIKit document picker를 요청한 창에 연결했다. 취소/owner dispose, 닫힘 완료 대기,
  security-scoped URL, 64-bit offset/64 KiB bounded read와 grant 해제를 제공한다.
  AppKit과 파일 grant 구현을 공유하되 UI picker와 플랫폼 증거는 분리했다.
- 텍스트/URI Copy drop의 native interaction, 비동기 provider 읽기, 좌표·bounds 협상,
  늦은 응답/해제 처리를 연결했다. 파일/송신/Move/Link/virtual file은 미지원이다.
- scene cold/warm URL 및 browsing activity를 기존 activation queue/Navigation/restoration에
  연결했다. main만 앱 링크를 소유하고 추가 창의 route/restoration namespace는 격리한다.
- iOS transitive Host.Maui 참조의 RID/TFM과 NuGet 자산 경로를 맞췄다.
  별도의 IOSSmoke/CatalystSmoke 및 Apple package-only 후보 생성 옵션을 추가했다.

기능 계약/제한은 [UIKit services](../../Doroti/docs/uikit-services.md)에 있다.
MAUI 창 생성은 [공식 Window API](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/window)를,
도메인 연결은 [Apple Associated Domains](https://developer.apple.com/documentation/xcode/supporting-associated-domains)를 따른다.

## 실행 기록

- 공통 plugin/event stream/취소/소유권 회귀 **PASS**.
- 공통 OS drop 협상/비동기 전달/큰 offset/owner 해제 회귀 **PASS**.
- Catalyst Debug build **PASS**, native editor/WKWebView 각 2회 생성·재생성 **PASS**.
- Catalyst OS LaunchServices warm URL → 실제 Router marker **PASS**.
- Catalyst 두 창의 독립 크기 470×650 / 580×620, DPR 2; main close 뒤 survivor resize/close,
  registry 0 및 Explicit 요청 **PASS**. 물리 입력·실제 표시 품질과 구분한다.
- iOS Simulator Debug build, native editor/WKWebView 각 2회 생성·재생성 **PASS**.
  입력 화면 screenshot을 직접 확인했다. 최종 snapshot은 native view 제거 뒤의 화면이다.
- iOS OS custom-scheme 전달은 시스템 “열기” 확인창에서 대기하여 timeout. 화면으로
  원인을 확인했다. 무인 실행 성공으로 기록하지 않는다.

추가 재실행:

- iOS Simulator: 128 MiB 파일 마지막 offset 읽기/해제 후 거절, picker pre-cancel·표시 후 취소·owner dispose **PASS**.
- Catalyst: 같은 파일 grant/picker 취소·owner dispose와 text/URI Copy capability **PASS**.
- iOS Simulator: 실제 scene delegate에 browsing `NSUserActivity`를 주입 → Router URI 변경 → 프로세스 재시작 후 동일 route 복원 **PASS**. 도메인 AASA/OS Universal Link 검증은 아니다.
- Catalyst: 최종 URL/route 복원·두 scene/survivor/Explicit drain **PASS**. 초기 full Mono AOT 빌드와 후속 Debug interpreter + Host.Maui Mono AOT 빌드를 구분한다.
- AppKit Debug build **PASS**: 공유 파일 grant 추출 뒤 컴파일 회귀. 기존 AppKit 실행 결과를 새 UIKit 결과로 전용하지 않는다.
- Source suite(문서 경로·runner/installer 계약) **PASS**.
- iOS Ganesh Testbed 실행은 Graphite 전용 PlatformView manifest의 factory 부재로 거절됐다. non-PlatformView 앱 실행은 별도이며 해당 smoke를 Ganesh 통과로 쓰지 않는다.

## 재현

```sh
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/tests/apple_smoke.py --target maccatalyst --output temp/testing/apple-work2/catalyst
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/tests/apple_smoke.py --target ios --activation native-callback --cases services,input,navigation,restoration --output temp/testing/apple-work2/ios
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/eng/release-candidate.py --targets maccatalyst
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/eng/release-candidate.py --targets ios
```

`--activation native-callback`는 실제 scene delegate에 browsing NSUserActivity를 주입한다.
OS Universal Link 도메인 검증이나 사용자의 링크 탭 증거가 아니다. `--activation os`는
OS custom scheme을 사용하며 iOS 확인창 조작이 필요한 환경에서는 자동 합격하지 않는다.
`--renderer ganesh`는 renderer 선택 옵션이다. 현재 Testbed manifest에는 Graphite 전용 PlatformView가 있어 Ganesh 시작이 거절된다.
Catalyst Desktop의 Ganesh는 의도적인 미지원이다.

## 남은 전체 범위

- M0/M1: 원격 CI·clean checkout/OS, 모든 renderer GPU golden 및 전체 native 자원 격리.
- M2 입력: 실기기 한글 IME/caret/selection/후보창·외부 키보드/Tab·clipboard·VoiceOver,
  native gesture 경쟁과 전체 clip/transform/z-order 조합.
- M2 렌더링: 실기기 장기 스크롤/메모리, GPU loss/복귀/retirement 스트레스,
  영상/blur/다중 효과의 GPU golden, 실제 present/VRAM 예산, offline/CDN 폰트 정책.
  이번 기능 smoke를 성능/FPS 측정으로 쓰지 않는다.
- M4/M5: 실제 picker 선택·권한/문서 provider·OS drag 수신, 파일 drop·송신·drag image·Move/Link,
  native event source qualification. Universal Link의 실제 domain entitlement/AASA/cold OS 경로,
  앱별 schema migration·사용자 입력/selection 및 강제 종료 복원.
- M6: 두 scene의 물리 IME/focus·다중 모니터·전체 native content/GPU stress, native scene
  복원·owner/modal/satellite/popup·창 간 이동. UIKit native close 취소와 AppKit 전용 정책은 unsupported.
- M7: 실기기 배포 서명, App Store/Developer ID/notarization, clean OS 설치·업데이트·제거,
  device Mono AOT/NativeAOT 및 전체 renderer trimming qualification. 로컬 package-only와 분리한다.
- iOS 추가 창과 Apple IDE Hot Reload 확대는 원문에서 현재 필수 대상이 아니므로 새 의무로 추가하지 않았다.

## 조사·정리

초기 실패: UIKit connectionOptions의 null collection, iOS transitive reference의 잘못된
assets 경로, picker dismiss 완료 전 다음 요청, iOS OS URL 확인창. 원인과 수정/미완료를
구분한다. 원시 자료는 `temp/testing/apple-work2/` 및 해당 release-candidate 실행 소유 경로에 저장한다.


Catalyst 후속 Debug 프로필 재현:

```sh
python3 Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build samples/DorotiTestbedApp/macos/DorotiTestbedApp.MacCatalyst.csproj -c Debug -r maccatalyst-arm64 -p:DorotiMacCatalystTargetFramework=net10.0-maccatalyst27.0 -p:UseInterpreter=true '-p:MtouchInterpreter=all%2C-Doroti.Host.Maui'
```

실행기 조사: Catalyst 초기 `Popen` 직후 `open -a`를 병행하면 별도 process가 생성되어
후속 URL을 다른 프로세스가 받았다. 실행기의 중복 launch를 제거하고 이번 시험이 생성한
추가 프로세스만 종료한 뒤 OS URL/복원을 재검증했다. 제품의 scene 생성과 프로세스 중복은 다르다.


## 최종 소스 재검증

최신 소스의 iOS/Catalyst Debug build는 각각 경고/오류 0으로 통과했다.
iOS Graphite의 서비스·native editor/WKWebView 재생성·native activity callback·route 재시작 복원이 재통과했다.
Catalyst 서비스·native 재생성·OS warm URL도 재통과했다. 두 창 회귀의 content를 input 장면으로
바꿔 각 scene에 native editor를 포함한 상태에서 main 종료 → survivor resize → 종료를 확인했다.

Router 화면 marker는 post-frame restoration 저장보다 먼저 기록될 수 있다. 종료 시점을 그 marker에만
맞춘 한 실행에서는 재시작 후 이전 checkpoint가 복원됐다. smoke는 이제 해당 실행의 고유 restoration
파일에서 route와 직렬화된 Router 상태가 모두 저장됐음을 확인한 뒤 프로세스를 종료한다.
이후 Catalyst route 복원과 두 native editor 창 수명은 PASS다. 저장 완료 전 임의 시점의 강제 종료까지
최신 편집값을 보장한다는 의미는 아니다. 기존 사용자 restoration 파일은 삭제하지 않는다.

공유 Apple 파일 grant 추출 후 AppKit의 실제 panel 취소·owner dispose·128 MiB read·권한/디렉터리 거절
회귀도 PASS다. UIKit 파일 선택의 물리 사용자 선택·iCloud provider 증거로 확대하지 않는다.


## 프로비저닝 요청 결과

사용자 요청에 따라 기존 Apple Development 인증서의 Team `H8SM2C82X7`로
`dev.doroti.testbed`의 Automatic signing을 요청했다. 실행 소유 임시 Xcode app target에
`-allowProvisioningUpdates -allowProvisioningDeviceRegistration`과 연결된 iPhone destination을
지정했으며 **BUILD SUCCEEDED**와 embedded profile을 확인했다. 이 임시 앱은 설치하지 않았다.

- Profile: `iOS Team Provisioning Profile: dev.doroti.testbed`
- UUID: `f68c898a-241e-4101-9807-f0bf050f9571`
- App identifier: `H8SM2C82X7.dev.doroti.testbed`
- 만료: 2026-10-06 08:48:56 UTC
- 설치 경로: `~/Library/Developer/Xcode/UserData/Provisioning Profiles/f68c898a-241e-4101-9807-f0bf050f9571.mobileprovision`

profile은 임시 자료 정리 대상에서 제외한다. 발급 뒤의 실제 Testbed device signing/실행은
사용자의 실기기 생략 지시로 보류했다. 계정의 private key를 내보내거나 배포 계정을 새로 만들지 않았다.


## 최종 package-only 후보

로컬 버전 `0.3.0-beta.rc.20260929082743`의 플랫폼별 feed를 따로 보존한다.
공개 NuGet push/App Store 배포는 하지 않았다. 동일 ID의 단일-TFM Host 패키지이므로
두 feed를 합쳐 범용 multi-platform 패키지로 배포하는 결과가 아니다.

| 후보 디렉터리 (`Doroti/artifacts/release/` 기준) | 패키지/앱 파일 | 검증 |
| --- | --- | --- |
| `0.3.0-beta.rc.20260929082743-catalyst-qualified` | 25 packages / 224 payload files | 격리 cache restore, Release publish, ad-hoc native/app signature 검증, Graphite native frame 2 / software fallback 0 |
| `0.3.0-beta.rc.20260929082743-ios-final` | 24 packages / 238 payload files | 격리 cache Release simulator build, embedded native/app ad-hoc signature, 설치/기동/화면; Graphite와 Ganesh 각각 native frame 2 / software fallback 0 |

위 frame 수는 완료 receipt이며 실제 present 간격/FPS/성능 예산이 아니다.
iOS non-PlatformView 템플릿 앱의 Ganesh 기동은 PASS이며 Graphite 전용 native compositor
지원으로 해석하지 않는다. 두 renderer의 템플릿 화면을 캡처했고 Graphite 화면을 직접 확인했다.

생성 시작 시점 sourceTreeSha256:

- Catalyst: `5cdd02682be28ce4745a8d0412951edb0acbaf99e9ca7a8d73de0bb67ffd4746`
- iOS: `adad0d58ebde4b2df7c7a59a35d7494629e0fccb8f130bb79df89930cd30ae4f`

각 candidate.json에 package/payload SHA-256과 실제 native runtime 요약을 기록했다.
최종 원본 소스의 Catalyst 16 availability guard까지 포함하는 후보는 `-catalyst-qualified`다.

iOS 후처리 복구: Release build 자체는 통과했지만 최초 스크립트가 RID별 `bin/iossimulator-arm64/`
경로를 찾지 못했다. 스크립트를 수정하고 이미 성공한 같은 소비 앱을 정확한 경로에서 복사했다.
또한 `EnableCodeSigning=false` 산출물의 loose Mono dylib는 root `codesign --deep`만으로
다시 서명되지 않아 `CODESIGNING Invalid Page`로 종료됐다. dylib → framework → app 순서로
각각 서명/strict 검증한 뒤 재설치·실행을 통과했다. 이 복구 절차를 후보 도구에 반영했으며
manifest의 recovery 필드에 최초 오류와 후속 검증을 보존했다. 이 iOS 후보의 후처리는 전체
명령 재빌드 대신 이미 빌드한 소비 앱에서 재개한 결과다.

clean OS, Mac App Store/Developer ID/notarization, 전체 renderer trimming과 iOS device
publish/Mono AOT/NativeAOT는 이번 결과에 포함하지 않는다. 실기기 경로는 사용자 요청으로 보류했다.


최종 정리: 결과 문서·유지보수 smoke 소스·위 두 최종 후보만 보존한다.
`temp/testing/apple-work2/`와 이번 실패 조사 소비 앱(`061647792986`, `640518b221e6`),
중간 후보 `0.3.0-beta.rc.20260929082021`, `0.3.0-beta.rc.20260929082743`,
`0.3.0-beta.rc.20260929082743-catalyst-final`은 요약·해시 검증 후 정리했다.
발급된 Xcode profile 및 기존 사용자의 앱 데이터·이전 AppKit 최종 후보는 정리 대상이 아니다.

시뮬레이터 앱 Documents의 이번 실행별 raw 폴더 4개도 정리했다. 별도 smoke namespace의 restoration checkpoint와 설치된 샘플 앱은 앱 데이터 영역에 남아 있으며 기존 사용자 namespace를 지우지 않았다. 최종 Source/runner/installer 검사와 두 후보의 전체 package/payload SHA-256 재대조가 통과했다.


## 실기기 설치 재개 — 2026-09-29 18:02 KST

사용자의 후속 **“이제 실기기에 설치해보자”** 요청으로 이전 보류를 해제했다.
연결된 iPhone 12 / iOS 26.6.1에 최신 source Testbed를 개발 서명하여 설치·기동했다.

```sh
python3 Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build samples/DorotiTestbedApp/ios/DorotiTestbedApp.iOS.csproj -c Debug -r ios-arm64 -p:DorotiIosTargetFramework=net10.0-ios27.0 -p:DorotiCompilationMode=Mono -p:EnableCodeSigning=true '-p:CodesignKey=Apple Development' -p:CodesignProvision=f68c898a-241e-4101-9807-f0bf050f9571
```

- Debug/Mono, 기존 프로필의 Host.Maui Mono AOT + interpreter 조합. build 83.88초, 경고/오류 0.
- `codesign --verify --deep --strict` PASS. 위에서 발급한 development profile 사용.
- `devicectl device install app`로 `dev.doroti.testbed` 설치 PASS. 기존 앱을 uninstall하거나 사용자 데이터를 초기화하지 않았다.
- native process 기동 및 생존 확인. 실제 기기 screenshot(1170×2532)에서 Doroti Material 3 화면·카드·아이콘·탐색 UI를 직접 확인했다.
- 기기 진단: `ios-arm64`, UIKit/MTKView/Graphite-Metal, DPR 3, frame completion receipt 855, failed 0, software fallback 0. FPS/성능 예산 또는 전체 GPU command 계측 결과로 쓰지 않는다.
- 확인 후 evidence 수집 flag를 끄고 일반 실행 상태로 남겼다. 설치된 개발 앱과 profile은 유지한다.
- 이는 source Debug 실기기 설치/기동 결과다. package-only 실기기·Release/NativeAOT·물리 IME/VoiceOver·장기 수명 검증을 뜻하지 않는다.

원시 build/install/launch 로그·캡처·기기 진단은 `temp/testing/ios-device-install/20260929/`에 수집했고 결과 기록 후 정리했다. 서명된 제품 빌드와 기기에 설치된 앱은 유지한다.


## SampleApp2 선택 핸들 떨림 수정 — 2026-09-29

- 환경: iPhone 12 / iOS 26.6.1, SampleApp2 Debug/Mono, `net10.0-ios27.0` / `ios-arm64`, Graphite-Metal.
- 사용자 재현 구간에서 접근성 `setSelection`과 IME editing-state 역전 이벤트는 없었다. 선택값은 정상적으로 변하지만 표시된 화면 순서가 뒤로 돌아가는 문제였다.
- `DorotiUIKitGraphiteView.Draw`가 최대 3개 프레임을 진행하던 동안, 새 장면의 GPU 완료 전 추가 native draw가 이전 `_presentedFrame`을 재생했다. 기존의 1개 제한은 PlatformView composition에만 적용돼 시스템 magnifier의 redraw는 보호하지 못했다.
- UIKit Graphite에서는 장면의 GPU 완료·replay-source 승격 전 추가 draw를 기존 backpressure 경로로 미룬다. UI 스레드를 기다리게 하지 않으며 완료 콜백에서 다시 invalidate한다.
- 임시 native probe로 Profile의 실제 EditableText/IME/magnifier에 양쪽 핸들 각각 25회 pointer move를 전달하고 `SkiaSceneRenderer.FrameReceipt`의 scene sequence를 비교했다. 수정 전 이전 장면 재표시 9회(예: `346 → 345 replay`), 수정 후 0회. 이는 해당 기기에서의 프레임 순서 회귀 검증이며 전체 플랫폼 성능 검증은 아니다.
- 재현 빌드: `dotnet build samples/DorotiSampleApp2/ios/DorotiSampleApp2.iOS.csproj -c Debug -r ios-arm64 -p:DorotiIosTargetFramework=net10.0-ios27.0 -p:DorotiCompilationMode=Mono -p:EnableCodeSigning=true '-p:CodesignKey=Apple Development' -p:CodesignProvision=ecde6fbc-22e8-4096-98d0-aee6e261ec16`.
- 임시 계측 소스를 제거한 최종 source Debug 빌드도 경고/오류 0으로 통과했다. iPhone 12에 재설치하고 진단 flag 없이 정상 실행했다. 원시 검증 자료와 임시 probe는 `temp/testing/selection-jitter/`에서 결과 요약 후 정리했다.
