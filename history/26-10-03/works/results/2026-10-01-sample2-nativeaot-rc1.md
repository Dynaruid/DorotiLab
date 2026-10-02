# SampleApp2 NativeAOT RC1 설치 기록 — 2026-10-01

정리 후 기본 .NET 설치 경로에서 새 Release/NativeAOT 앱을 게시하고 iPhone 12에 설치했다.
설치 후 첫 실행은 iOS 보안 오류로 거부됐지만, 사용자가 개발자 신뢰를 처리한 뒤 재실행에 성공했다.
일반 Components 화면과 실행 프로세스 유지를 확인했다. 실기기 성능·전체 수동 조작은 미검증이다.

## 도구 구성과 정리

- 기본 호스트: `/usr/local/share/dotnet/dotnet`
- SDK: `11.0.100-rc.1.26425.128`
- Workload set: `11.0.100-rc.1.26458.5`
- iOS SDK/bgen: `26.5.12193-net11-rc.1`
- bgen용 데스크톱 런타임: `11.0.0-rc.1.26426.105`, 기본 경로에 설치됨
- MAUI: `11.0.0-rc.1.26451.6`
- Xcode: `27.0` / `27A266a`
- 기기: iPhone 12 / iOS `26.6.1` (`23G83`), 유선 페어링, 개발자 모드 활성화

사용자가 정리 스크립트를 실행한 후 기본 호스트의 SDK/workload 선택과 bgen 실행을 재확인했다.
이전 `$HOME/.local/share/doroti/dotnet-ios-development` 경로가 없음을 확인했다.
사용자 요청에 따라 일회성 정리 스크립트·전용 테스트·설명 파일을 삭제하고 개발 문서의
dotnet 경로를 갱신했다. 기존 공용 도구 업데이트 스크립트는 유지했으며 관련 테스트 7개가 통과했다.

[공식 RC1 릴리스](https://github.com/dotnet/macios/releases/tag/dotnet-11.0.1xx-rc1-12193)는
Xcode 26.6을 요구한다. 이번 빌드는 기존 실험 프로필처럼 Xcode 버전 검사를 해당 명령에서만
우회했다. SDK/프로젝트의 전역 설정을 바꾸지 않았다.

## 게시 명령과 NativeAOT 확인

SDK 선택은 기존 Testbed iOS 폴더의 `global.json`으로 했다. SampleApp2는 Fixed 기본값 후보
구현이 포함된 현재 소스를 사용한다. renderer/shader/host/API 소스 변경은 없다.

```sh
cd samples/DorotiTestbedApp/ios
env -u MSBuildSDKsPath \
  -u DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR \
  -u DOTNET_MSBUILD_SDK_RESOLVER_SDKS_DIR \
  -u DOTNET_MSBUILD_SDK_RESOLVER_SDKS_VER \
  DOTNET_ROOT=/usr/local/share/dotnet \
  DOTNET_HOST_PATH=/usr/local/share/dotnet/dotnet \
  DOTNET_CUSTOM_PATH=/usr/local/share/dotnet/dotnet \
  DOTNET_CLI_USE_MSBUILD_SERVER=0 MSBUILDDISABLENODEREUSE=1 \
  /usr/local/share/dotnet/dotnet publish \
  ../../DorotiSampleApp2/ios/DorotiSampleApp2.iOS.csproj \
  -c Release -r ios-arm64 \
  -p:DorotiIosTargetFramework=net11.0-ios \
  -p:DorotiCompilationMode=NativeAot \
  -p:ValidateXcodeVersion=false \
  -p:PrepareAssemblies=false \
  -p:_UseDynamicDependenciesForMarkNSObjects=false \
  -p:InlineClassGetHandle=disabled \
  -p:ArtifactsPath=/Users/ceramic/Labo/DorotiLab/Doroti/artifacts/sample2-fixed-nativeaot-net11-unified
```

RC1에서 앞서 재현된 IL2037 preservation 오류와 Xcode 27의 제거된 Objective-C 클래스 심볼
링크 오류를 피하기 위한 옵션을 해당 게시 명령에 한정했다. 워크로드나 bgen 파일을 수정하지 않았다.
게시 종료 코드는 0이며 IPA가 생성됐다. ILC의 `NSLayoutAnchor` 프록시 생성 메서드가
항상 예외를 던진다는 진단이 남아 있어 전체 동작 검증 통과로 취급하지 않는다.

| 확인 항목 | 결과 |
| --- | --- |
| `PublishAot` / `UseNativeAot` | `true` / `true` |
| `UseMonoRuntime` | `false` |
| 앱 번들 내 관리 DLL | 0개 |
| 번들 파일 크기 합계 | 42,182,441 bytes |
| `codesign --verify --deep --strict` | 통과 |
| 실행 파일 SHA-256 | `7a5b8022cc2652b5bf8a56503110dcd5e8ecc97b39c872fa4144f44d6ea0526a` |
| iPhone 설치 | 성공 (`dev.doroti.sample2`) |
| 일반 앱 실행 | 사용자 신뢰 처리 후 성공 (PID 6079) |
| Components 화면 표시 | 기기 스크린샷으로 확인 |

앱: `Doroti/artifacts/sample2-fixed-nativeaot-net11-unified/bin/DorotiSampleApp2.iOS/release_ios-arm64/DorotiSampleApp2.iOS.app`

## 초기 실행 차단과 신뢰 처리 후 재확인

CoreDevice 오류 `10002`, `FBSOpenApplicationErrorDomain` 오류 `3` (`Security`)로 실행이 거부됐다.
오류는 유효하지 않은 코드 서명, 부족한 entitlement 또는 사용자 개발자 신뢰 중 하나를 지목하며,
초기 오류만으로 셋 중 원인을 확정하지 않았다.

기기에 설치된 provisioning profile과 새 앱의 내장 profile은 같은 UUID
`ecde6fbc-22e8-4096-98d0-aee6e261ec16`이다. 기기에서 profile은 Valid로 조회됐고,
별도 `devicectl device profile validate`도 통과했다. 만료는 2026-10-06 18:36:55 KST이며
현재 기기 UDID가 허용 목록에 있다.

새 앱의 서명 entitlement는 이전 정상 실행 Mono 앱과 같다.
`application-identifier=H8SM2C82X7.dev.doroti.sample2`,
`com.apple.developer.team-identifier=H8SM2C82X7`, `get-task-allow=true`다.
두 앱 모두 XML/DER entitlement 서명 슬롯을 포함한다.
이 확인은 기기에서 사용자 개발자 신뢰 또는 실제 실행이 성공했다는 증거를 대신하지 않는다.
사용자는 개발자 신뢰를 눌렀다고 보고했다. 이어 같은 설치 앱을 환경 변수나 benchmark 설정 없이
재실행했고 CoreDevice가 성공을 반환했다. 새 프로세스 PID 6079와 설치 경로를 확인했으며,
뒤이은 프로세스 조회에서도 PID 6079가 유지됐다. 기기 스크린샷 `after-trust.png`에
Cupertino playground/Components 화면과 정상 UI가 표시됐다. 앱을 일반 Components 화면에 남겼다.
이 결과는 초기 실행 차단이 사용자 신뢰 처리 후 해소됐다는 관찰이며, 전체 기능 검증을 뜻하지 않는다.

원본은 `temp/testing/variable-blur/nativeaot-unified-2026-10-01/`에 보관한다.
`publish.log`, `artifact.json`, `install.json`, `launch.json`, `launch.log`,
`device.json`, `profiles.json`, `profile-validation.json`, entitlement plist와 화면 캡처가 포함된다.
신뢰 처리 후 증거는 `launch-after-trust.json`, `launch-after-trust.log`,
`processes-after-trust.json`, `after-trust.png`, `after-trust-screen.json`이다.
NativeAOT FPS 측정은 수행하지 않았으며 기존 Mono의 P1 수치·미완료 수동 게이트와 구분한다.
