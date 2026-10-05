# iOS 작업 계획

원문 요약: [plan 요약](../../../26-09-28/plan-summary.md) M0~M7 · 작업 상태: **PARTIAL** · 새 실행 검증: **범위별 PASS / 잔여 notVerified**

[전체 작업 인덱스](../README.md)

UIKit 호스트를 대상으로 하며 실제 기기와 시뮬레이터, Graphite와 다른 renderer의 지원·증거를 각각 기록한다.

## 공통 작업과 의존성

M0 → M1 최소 경로 → M2 → M4/M5 → M6(대상 플랫폼) → M7 순서로 진행한다. M3의 생성·실행은 M0 후, Hot Reload는 M1과 개발 호스트·실행 세션 준비 후 연결한다. package-only smoke는 M0부터 시작한다. 아래 공통 문서에서 계약을 정하고 이 문서에서 플랫폼별 연결·실측 결과를 추적한다. M3/M6의 플랫폼 적용 범위는 본문의 경계를 따른다.

- M0/M1: [실행·지원표](../common/00-foundation.md), [공통 테스트 런타임](../common/01-testing.md)
- M2: [Desktop 계약](../common/02-desktop-contract.md), [입력·접근성·PlatformView](../common/03-input-accessibility-platformview.md), [렌더링·수명](../common/04-rendering-lifetime.md)
- M3/M4: [개발 도구·Hot Reload](../common/05-vscode-hot-reload.md), [플러그인 SDK](../common/06-plugin-sdk.md)
- M5: [OS Drag & Drop](../common/07-os-drag-drop.md), [Navigation·복원](../common/08-navigation-restoration.md)
- M6/M7: [멀티윈도우](../common/09-multiwindow.md), [패키지 소비·배포](../common/10-release-packaging.md)

## 1. 실행 기반 — M0/M1, P0 → P1

- [ ] iOS runner·toolchain·기기/시뮬레이터·renderer·build mode별 실행 경로를 정리하고 M1 공통 회귀 fixture를 연결한다.
- [ ] 기존 PlatformView 지원표의 adapter 범위와 실제 호출 경로를 확인한다. 별도 renderer 조합은 자동 지원으로 간주하지 않고 필요한 연결과 미지원 경계를 기록한다.

## 2. 입력·VoiceOver·PlatformView — M2-B, P1

- [ ] 실기기 한글 조합·selection/caret·후보창·multiline·clipboard·focus loss를 확인하고 Doroti TextField ↔ UITextField/WKWebView의 IME 소유권·키 중복/눌림 해제를 검증한다.
- [ ] 외부 키보드가 있는 환경의 Tab/Shift+Tab과 VoiceOver 이름·역할·값·상태·action·focus·탐색 순서를 실제 사용으로 확인한다.
- [ ] PlatformView 복수 owner·clip/transform/z-order·native-origin gesture 경쟁·생성/해제/재생성·늦은 응답·앱 복귀 후 입력을 검증한다.

## 3. 렌더링·수명 — M2-C, P1

- [ ] 기기·해상도·OS·renderer별 baseline과 예산을 정하고 리스트·VariableBlur·다중 효과·WKWebView overlay·이미지/영상 texture·크기 변화 장면을 측정한다.
- [ ] UI/GPU/readback/upload/present/메모리를 구분하고 capture geometry·pixel origin·축소 렌더링 품질, pause/resume·surface/device loss·해제 수명을 검증한다.
- [ ] 폰트 CDN/asset/offline·초기 교체·한글 fallback을 확인한다. iOS 브라우저 메모리 검증은 Web 문서에서 추적한다.

## 4. 앱 기능·Universal Link·복원 — M4/M5, P2

- [ ] FilePicker·URL launcher의 공통 패키지 연결, 취소·권한 거절·외부 파일 접근 수명·늦은 응답을 검증한다.
- [ ] 모바일 OS Drag & Drop 제공 범위를 정하고 지원 환경의 수신·action·좌표·비동기 읽기·취소를 capability와 함께 검증한다.
- [ ] Universal Link의 cold/warm activation을 Router 준비 전 대기·중복 처리와 연결하고 같은/잘못된 링크의 처리를 확인한다.
- [ ] lifecycle 기반 route·입력·선택 상태 저장, 정상 재시작/강제 종료 후 복원, version migration·실패 fallback을 검증한다.

## 5. 배포 — M7, P3

- [ ] package-only 소비 앱을 clean 환경에서 검증하고 실제 지원 Release·trimming·Mono AOT/NativeAOT 조합을 기기별로 기록한다.
- [ ] 서명·패키징·설치·업데이트·제거·데이터 유지·crash/로그와 장기 실행·반복 PlatformView 생성·GPU loss/앱 복귀 회귀를 검증한다.

## 완료 기준

실기기 입력·VoiceOver·화면·복귀와 배포 결과를 시뮬레이터 결과와 구분해 남긴다. Mac Catalyst 및 다른 renderer의 지원으로 확대 해석하지 않는다. M6 실제 추가 창은 이번 iOS 필수 범위에 추가하지 않는다.

## 착수 시 확인할 코드·문서

- [Doroti/src/Doroti.Host.Maui](../../../../packages/platforms/maui/Doroti.Host.Maui)
- [Doroti/src/Doroti.Target.iOS.Maui.ios-arm64](../../../../packages/platforms/maui/Doroti.Target.iOS.Maui.ios-arm64)
- [Doroti/docs/platform-views/ios.md](../../../../Doroti/docs/platform-views/ios.md)
- [samples/DorotiTestbedApp/ios](../../../../samples/DorotiTestbedApp/ios)

기존 문서의 과거 증거와 현재 구현을 다시 확인한다. 위 경로는 조사 시작점이며 새로운 실행 검증의 근거는 아니다.

## 검증·결과 기록

[공통 완료 규칙과 결과 형식](../README.md#결과-기록-형식)을 적용한다. 테스트는 20분 timeout을 사용하고 일반 반복 검증은 30회 이내로 설계한다. 일회성 테스트·원시 로그·캡처·소비 앱은 `temp/testing/<작업-ID>/<실행-ID>/`에 모으고 요약 후 정리한다. 제품 빌드·release 후보 등 기존 `Doroti/artifacts`는 별도의 삭제 가능한 산출물이며, 필요한 요약·최소 상시 fixture만 추적되는 tests/docs/history에 보존한다.

2026-09-29: [iOS/Catalyst 후속 결과](../results/2026-09-29-ios-catalyst.md)에 구현·build·실행 범위를 기록했다.
아래 보강과 기존 체크리스트의 전체 완료는 구분한다. 물리 입력·VoiceOver·GPU 예산·실제 배포 잔여를 유지한다.

- [x] UIKit FilePicker/URL·텍스트/URI Copy drop capability와 scene URL/Universal Link callback 연결.
- [x] 별도 runner/renderer/TFM을 기록하는 유지보수 smoke 및 iOS RID/TFM 참조 경로 보강.
- [x] native editor/WKWebView 각각 생성·재생성 실행.
- [ ] 물리 입력·VoiceOver·권한 대화상자·OS drag/Universal Link의 전체 외부 전달 검증.
- [ ] 실기기 서명/설치·AOT·clean 배포·성능 예산·장기 수명 검증.

- [x] 2026-09-29 후속 요청으로 iPhone 12 / iOS 26.6.1에 source Debug/Mono 개발 서명·설치·기동·실제 화면 확인. Release/NativeAOT·물리 입력 qualification은 별도다.

### 2026-09-30 — 회전 중 연속 레이아웃

- 기준: `774183eb` 위 작업 트리. iOS Metal/Graphite가 최종 model bounds를 즉시 소비하던 경로에 `UIKitAnimatedViewport`를 연결했다. 회전 중에는 CADisplayLink에서 presentation bounds를 읽어 drawable·프레임워크 크기·safe-area 여백을 함께 갱신하고, 완료 후 실제 bounds로 정착한다. MAUI arrange 시점에 전환 coordinator가 사라진 경우에도 presentation 크기 차이로 추적을 시작한다.
- drawable과 model bounds가 다른 동안 MTKView가 배율을 다시 계산하는 피드백을 차단했다. 창이 속한 screen의 배율을 사용하고 drawable 갱신 뒤 layer contents scale을 복구한다. 화면에서 분리·Graphite 비활성화·해제 시 display link를 정리하며, Reduce Motion은 즉시 최종 크기를 적용한다.
- iPhone 18 Pro Simulator / iOS 27.0 / `net10.0-ios27.0` / `iossimulator-arm64` / Debug Mono / Graphite-Metal: scene의 실제 가로·세로 회전 **PASS**. 서로 다른 drawable 폭은 각각 5개·6개(시작·끝 포함), 관찰한 배율은 모두 3. 최종 `1206×2622 ↔ 2622×1206`, safe-area 일치와 display link 종료를 확인했다. 네이티브 입력 회귀도 **PASS**.
- 최종 Testbed·SampleApp2 iOS simulator Debug build와 Mac Catalyst Host(`net10.0-maccatalyst27.0`, `maccatalyst-arm64`) 컴파일 회귀는 모두 경고/오류 0. 입력 smoke는 native editor/WKWebView를 각각 생성·재생성(총 4회)했으며 물리 IME 검증은 아니다.
- `UIKitRotationProbe`와 Apple smoke의 선택적 `rotation` case를 남겼다. 화면 준비 및 전환 완료를 기다린 후 중간 크기 누락·최종 픽셀 크기 불일치·safe-area 불일치·display link 잔류를 실패로 판정한다. 원시 자료는 `temp/testing/ios-rotation/`에서 결과 요약 후 정리한다.
- 실기기 회전의 시각적 품질/FPS·Ganesh 실행·키보드를 표시한 회전·native overlay가 포함된 회전은 이번 검증 범위가 아니다. 시뮬레이터의 중간 크기 관찰을 성능 예산 통과로 해석하지 않는다.

재현(각 명령 20분 timeout):

```sh
python3 Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build samples/DorotiTestbedApp/ios/DorotiTestbedApp.iOS.csproj -c Debug -r iossimulator-arm64 -p:DorotiIosTargetFramework=net10.0-ios27.0
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/tests/apple_smoke.py --target ios --cases rotation,input --skip-build --output temp/testing/ios-rotation/recheck
```

후속 실기기 설치(2026-09-30): 사용자 요청으로 회전 수정이 포함된 SampleApp2를 iPhone 12 / iOS 26.6.1에 설치·실행했다. `net10.0-ios27.0` / `ios-arm64` / Debug Mono, 기존 개발 프로필 `ecde6fbc-22e8-4096-98d0-aee6e261ec16`을 사용했다. build 74.25초, 경고/오류 0, `codesign --verify --deep --strict` 및 `devicectl device install app` PASS. 기존 앱을 제거하거나 데이터를 초기화하지 않았다. 정상 실행 프로세스와 1170×2532 캡처에서 Components 화면을 확인하고 앱을 실행 상태로 남겼다. 실기기의 회전 동작·FPS 확인은 이 설치/기동 결과와 구분한다. 원시 설치 자료는 `temp/testing/ios-rotation-device/`에서 요약 후 정리했다.

### 2026-09-30 — 네이티브 회전과의 표시 시점 차이 보완

사용자의 실기기 피드백에 따라 중간 크기 존재 여부에 더해 UIKit 표시 크기와 실제 raster viewport의 차이를 측정했다. 기준은 `774183eb` 위 작업 트리이며 iPhone 12 / iOS 26.6.1 / Graphite-Metal / Debug Mono / `net10.0-ios27.0` / `ios-arm64`다.

- Apple의 [Core Animation model/presentation 구분](https://developer.apple.com/library/archive/documentation/Cocoa/Conceptual/CoreAnimation_guide/CoreAnimationBasics/CoreAnimationBasics.html), [다음 표시 시각](https://developer.apple.com/documentation/quartzcore/cadisplaylink/targettimestamp), [전환 coordinator](https://developer.apple.com/documentation/uikit/uiviewcontrollertransitioncoordinator), [Metal transaction 동기화](https://developer.apple.com/documentation/quartzcore/cametallayer/presentswithtransaction)를 검토했다. 현재 표시 크기를 뒤따르던 방식에서 UIKit의 실제 `bounds.size` 애니메이션과 timing function으로 다음 표시 시점의 크기를 계산하도록 변경했다. UIKit의 additive 애니메이션과 숨겨진 begin time(0)을 처리하며, 지원하지 않는 애니메이션은 presentation sampling으로 처리한다.
- 회전 중 CADisplayLink 하나가 draw를 소유하도록 하고, 이미 끝난 GPU 작업은 다음 크기 변경 전에 UI 스레드에서 회수한다. 기존 한 프레임 in-flight 제한을 유지해 이전 장면 replay 역전을 막는다. 일반 Graphite 장면은 회전 동안 최대 크기의 backing을 재사용하면서 viewport를 배율 변경 없이 가운데에 그리며, 완료 후 정확한 drawable 크기로 축소한다. native overlay composition은 기존 exact-size 계약을 유지한다.
- Microsoft의 [Mono interpreter 설명](https://learn.microsoft.com/en-us/dotnet/maui/macios/interpreter?view=net-maui-10.0)과 [MAUI 성능 측정 지침](https://github.com/dotnet/maui/wiki/Profiling-.NET-MAUI-Apps)을 확인했다. 기존 Debug 프로필은 UIKit host 외의 엔진을 해석해 회전 프레임 처리가 늦었다. `net10.0-ios27.0` 실기기 Debug는 기본적으로 엔진/의존성을 Mono AOT로 컴파일하고 앱/진입 어셈블리 및 runtime-generated code에 interpreter를 유지한다. 사용자 `MtouchInterpreter` 설정이 우선이며 시뮬레이터·Release Mono·NativeAOT 정책은 별도로 유지한다.

실제 scene rotation의 viewport 폭 차이(단위 pt, 프레임 callback 계측):

| 방향 | 수정 전 평균 / 최대 | 최종 평균 / 최대 | 최종 서로 다른 raster 폭 |
| --- | --- | --- | --- |
| 세로 → 가로 | 50.39 / 113.52 | 1.94 / 18.17 | 19 |
| 가로 → 세로 | 50.47 / 101.78 | 3.37 / 14.55 | 17 |

`DOROTI_UIKIT_ROTATION_ASSERT_SYNC=1`로 평균 차이 5%·최대 차이 10%(전체 폭 변화 대비), native timing/additive 계산, 중간 raster 크기, 최종 pixels/safe-area, display link 종료, frame failure 0 검사를 통과했다. 화면 녹화를 지원하지 않는 기기이므로 이는 presentation/raster 좌표 계측과 정착 화면 캡처의 증거이며 전체 앱의 FPS 또는 모든 native UI와의 시각적 동등성 판정은 아니다.

- 최종 Testbed/실기기·SampleApp2/실기기 및 Catalyst host 컴파일: 경고/오류 0. native editor/WKWebView 각각 생성·재생성(총 4회) PASS; 물리 IME는 별도다.
- `apple_build_profiles.py`: device/simulator 기본값, 명시적 설정, custom app assembly, interpreter opt-out, Release Mono, NativeAOT의 실제 MSBuild 평가와 template parity PASS.
- 최종 SampleApp2는 개발 서명 검사 후 기존 데이터를 유지하여 같은 아이폰에 재설치·실행했다. 일반 앱 실행에는 probe 환경변수를 주입하지 않았다. 정착 후 1170×2532 세로 및 2532×1170 가로 캡처에서 Components 화면을 확인하고 기기를 원래 방향으로 복구했다.

재현 시 기존 실기기 build 명령에 `MtouchInterpreter` override를 붙이지 않는다. Testbed에 `DOROTI_SAMPLE=reload`, `DOROTI_UIKIT_ROTATION_PROBE=rotation-qualified.json`, `DOROTI_UIKIT_ROTATION_ASSERT_SYNC=1`을 지정하면 Documents에 결과 또는 `.error`가 생성된다. 로컬 원시 자료는 `temp/testing/ios-native-rotation/`에서 결과 요약 후 정리했다. 실기기 Testbed의 Documents에 생성한 회전·입력 probe JSON은 남아 있다. devicectl은 개별 파일 삭제를 제공하지 않고 debugger 정리도 완료하지 못했으므로 앱 데이터를 초기화하지 않았다. SampleApp2에는 이 probe 결과가 생성되지 않는다.


## 2026-09-30 iOS metadata Hot Reload

`dev -Platform ios`와 VS Code iOS target을 연결했다. SDK의 실제 WebSocket metadata agent를 사용하고, Doroti 세션 relay는 요청 ID·기능 상태·완료된 프레임 응답만 전달한다. 저장 전에 앱의 요청 수락을 확인하며 Release/NativeAOT는 거부한다.

수정한 개발 경로는 RID가 섞인 watch graph의 중복 프로젝트, runner 작업 디렉터리, 일반 실행과 개발 빌드의 네이티브 캐시 혼용, design-time binding 컴파일의 빈 native resource package, AOT 모듈에 기록된 의존 어셈블리 MVID 무효화다. Mono 10.0.12의 `IsSupported` false 결과는 실제 delta 적용을 확인한 뒤 SDK와 같은 capability 조회로 보완했다. 일반 Release/NativeAOT 프로필과 구분하는 opt-in 개발 설정이다.

- **Simulator CLI PASS:** iPhone 18 Pro / iOS 27.0 / `net10.0-ios27.0` / `iossimulator-arm64` / Debug Mono / Graphite-Metal. SDK 10.0.401, Mono 10.0.12, iOS workload 27.0.10722.
- 실제 메서드 문구 변경 후 같은 PID·State ID, count 5·한글 텍스트·scroll 160 유지. revision 0→1→2 및 화면 캡처 확인. 실기기 제한을 반영한 최종 코드로 같은 CLI 시나리오를 재실행해 전 항목 PASS를 확인했다.
- CS0103 컴파일 오류 시 revision 증가 없음, 수정 후 재적용 PASS. 필드 타입 변경 ENC0009는 Restart 필요로 남기고 기존 프로세스·State 유지.
- **설치 VSIX PASS:** 별도 VS Code profile에서 iOS 선택 → Run → 실제 Hot Reload 명령 → 요청/프레임 응답 → 상태 보존 → Stop. 최종 simulator 전용 코드와 새 VSIX로 재실행해 같은 결과를 확인했다. 실제 사용자 마우스/키보드 입력 검증은 아니다.
- HTTP relay의 인증·오래된 runtime 요청·저장 전 수락·연결 만료/복구, 기존 확장 단위 테스트 8개, Apple 빌드 프로필 8개와 잘못된 개발 설정 거부, Runner SDK 패키지에 binding hook 포함을 확인했다.
- **실기기 USB 제한:** iPhone 12 / iOS 26.6.1에서 개발 서명·기동·SDK agent 로드와 상태 파일 통신까지 확인했지만 코드 delta 연결은 실패했다. 설치된 .NET 10 iOS native runtime 27.0.10722에는 USB `forward port:` 구현이 없고 .NET 11 프리뷰 runtime에는 해당 코드가 존재함을 확인했다. 당시에는 `ios-arm64` 개발 세션을 거부했다. 아래 2026-10-01 후속에서 .NET 11 CoreCLR 실기기 경로를 구현·검증했다.
- 생성 템플릿의 package-only 개발 실행, Ganesh, Intel simulator, Swift/바인딩 편집 및 Release/NativeAOT 핫리로드를 이 결과로 확대하지 않는다.

재현 명령과 제약은 [개발 세션 계약](../../../../Doroti/docs/development-hot-reload.md)을 따른다. 자동 seed를 사용한 상태 보존 증거이며 물리 IME·포인터 입력 qualification과 구분한다. 원시 자료는 `temp/testing/ios-hot-reload/`에 둔다.


## 2026-10-01 실기기 USB Hot Reload

사용자 요청에 따라 `ios-arm64` 개발 세션을 .NET 11 CoreCLR로 연결했다. 앱 root의 .NET 10 설정을 바꾸지 않고 `-IosSdkVersion`으로 세션별 SDK를 고른다. `-DotnetPath`는 Apple 바인딩 도구에도 전달한다. 시뮬레이터는 기존 .NET 10 Mono 경로를 유지한다.

- **실기기 CLI PASS:** iPhone 12 / iOS 26.6.1 / `net11.0-ios` / Debug CoreCLR / UIKit·Graphite-Metal. 실제 method-body delta와 완료 frame 응답을 확인했다. PID 5247, State ID `19f560d8-22c7-4135-aaba-d81a43382455`를 유지하면서 revision 0→1→2, count 5·`한글 유지`·scroll 160 유지.
- 컴파일 오류 CS0103에서는 revision 불변, 수정 뒤 두 번째 metadata update PASS. 필드 타입 변경 ENC0009에서는 자동 재시작 없이 이전 앱/State 유지. Stop 뒤 세션 closed 및 실제 기기 PID 소멸 확인. 테스트 중 수정한 소스는 복원했다.
- **설치 VSIX 실기기 PASS:** 별도 profile에 설치한 확장에서 iOS/실기기/SDK/dotnet host를 설정하고 Run → Hot Reload → Stop을 실행했다. PID 5248·State ID `eabe3128-edea-49ff-9fda-2f1332049469`, count 5·한글·scroll 160을 유지하며 `iOS VSIX Hot Reload passed`와 revision 1을 확인했다. Stop 뒤 해당 PID 소멸을 확인했다.
- SDK `11.0.100-rc.1.26425.128`, iOS pack `26.5.12193-net11-rc.1`, MAUI `11.0.0-rc.1.26451.6`. 이 RC1 SDK는 iOS Mono를 거부하므로 CoreCLR을 명시하고, 사용자 assembly MVID를 보존하는 `partial-static` registrar를 사용한다.
- 최초 CoreCLR 런타임은 delta 적용 직후 `Bad IL range`를 재현했다. [상류 수정](https://github.com/dotnet/runtime/pull/132837)이 포함된 runtime/crossgen2 `11.0.0-rc.2.26478.114`로 개발 프로필만 고정했다. 패키지 소스 commit `0710df333c4b13d48f0be8e79550d66e988a7112`의 수정 코드를 확인했고, 앱의 실제 `libcoreclr` 버전도 대조했다. Apple runtime pack 버전은 그대로 유지한다.
- 런타임 선택만 바꾸면 기존 네이티브 framework 캐시가 남을 수 있어 출력 경로를 `coreclr-<runtime-version>`으로도 분리했다. 코드 delta는 Apple SDK의 USB WebSocket 연결이 전송하고, Doroti 요청/응답만 CoreDevice의 앱 Documents 파일 복사로 전달한다. 요청 복사 완료 후 저장, 완료 프레임 후 성공 순서를 지킨다.
- 바인딩 도구가 요구하는 데스크톱 runtime `11.0.0-rc.1.26426.105`는 사용자 영역의 별도 dotnet host에 추가했다. `prepare-ios-device-dotnet.py`로 재현하며 시스템 SDK/워크로드 설치는 수정하지 않는다. Xcode 27에서는 기존 RC1 제약에 따라 실행 환경에 `ValidateXcodeVersion=false`를 명시했다. 공식 Xcode 지원 조합이나 출시 배포 검증으로 확대하지 않는다.

단위/설정 검사: HTTP 인증·세션 만료/복구, 실기기 요청 전달 실패 시 저장 금지, 실제 bundle container를 구분하는 종료, 기존 확장 테스트 8개, Apple 프로필 및 runtime/crossgen2·Apple pack 버전 분리, Source 계약 PASS. 자동 seed를 사용했으며 물리 IME·터치 입력이나 장기 성능 시험은 아니다. 재현 명령은 [개발 세션 계약](../../../../Doroti/docs/development-hot-reload.md), 원시 증거는 `temp/testing/ios-device-reload/`에 둔다.

최종 회귀: 실기기 확장을 포함한 코드로 .NET 10 iPhone 18 Pro Simulator CLI 테스트를 재실행했다. metadata update 2회, 동일 PID/State·카운터/한글/스크롤, 컴파일 오류 복구, rude edit 유지, Stop 모두 PASS (`simulator-regression`).

## 2026-10-01 .NET 10 실기기 네트워크 Hot Reload

.NET 10을 유지하는 실기기 개발 경로를 추가했다. iPhone과 Mac을 같은 네트워크에 연결하고 `-Rid ios-arm64 -Device <UDID> -IosTargetFramework net10.0-ios27.0`으로 실행한다. 별도 SDK/런타임 패치 없이 설치된 SDK의 WebSocket agent와 실제 metadata delta를 사용한다. USB-only 전송은 기존 .NET 11 CoreCLR 프로필을 유지한다.

- Python launcher가 Mac의 선택한 LAN IPv4에 세션 전용 TCP relay를 열어 SDK loopback WebSocket 서버로 바이트를 전달한다. SDK 인증·delta 프로토콜은 유지한다. `-IosHotReloadHost` / `doroti.iosHotReloadHost`로 네트워크 어댑터를 지정할 수 있다. 요청 ID·상태는 기존 CoreDevice Documents 경로를 사용하며, 요청 복사 완료 → 저장 → 완료 frame 응답 순서를 유지한다.
- .NET 10 native runtime에는 mlaunch의 USB forwarding 구현이 없다. 해당 실행 인자를 제거하면서 SDK agent를 개발 앱에 원본 그대로 번들하고 startup hook의 Mac 절대 경로를 assembly 이름으로 바꾼다. 경로를 바꾸지 않으면 `mono_runtime_run_startup_hooks`에서 앱이 종료되는 것을 재현했다. 개발 빌드에만 Local Network 사용 설명 plist를 포함한다.
- 앱만 해석하고 framework를 AOT로 실행한 첫 delta는 `StatefulElement.reassemble`의 virtual call에서 Mono 10.0.12 `hot_reload.c:1218` assertion으로 종료됐다. `dev`는 `all,-Doroti.Host.Maui`로 framework 호출자까지 해석하며, 이와 다른 실기기 Mono interpreter 조합을 거부한다. 일반 Debug의 앱 전용 interpreter 분리, Release Mono·NativeAOT는 유지한다. 개발 실행 속도와 일반 빌드 성능을 구분한다.
- 개발 산출물은 `.doroti/cache/development/10.0.401/ios-arm64/mono-interpreter`로 분리한다. 과거 AOT 모듈과 새 interpreter 프로필을 섞지 않는다.

**CLI PASS:** iPhone 12 / iOS 26.6.1 / `net10.0-ios27.0` / `ios-arm64` / Debug Mono 10.0.12 / UIKit·Graphite-Metal, SDK 10.0.401·iOS workload 27.0.10722. 실제 method-body delta 2회와 완료 프레임을 확인했다. PID 5327, State ID `1cf8f954-d2dd-4b8e-be4f-7e97ad741a26`, count 5·`한글 유지`·scroll 160을 유지하며 revision 0→1→2. CS0103 오류에서는 revision 불변, 수정 후 적용 성공. ENC0009 변경은 기존 process/State를 유지하고 Restart 필요로 남겼다. Stop 후 세션 closed 및 실제 PID 소멸을 확인했고 테스트 소스를 복원했다.

HTTP/file relay 및 실제 TCP 양방향 전송·종료 검사 4개, 기존 VS Code 단위 검사 8개, 실제 MSBuild의 일반/개발/명시적/Release/NativeAOT/.NET 11 프로필과 agent 번들·실행 경로 검사 PASS. SDK 패키지에 개발 plist 포함을 확인했다. 자동 seed 기반 상태 보존 검사이며 물리 IME·터치 입력·장기 성능·모든 framework 변경의 qualification은 아니다. 원시 증거: `temp/testing/ios-net10-reload/run5/`. 재현 명령과 사전 조건은 [개발 세션 계약](../../../../Doroti/docs/development-hot-reload.md)을 따른다.

**설치 VSIX PASS:** 같은 .NET 10/iPhone 조합에서 별도 VS Code profile에 새 VSIX를 설치하고 Run → Hot Reload 명령 → Stop을 실행했다. PID 5328 및 State ID `20556444-6a6a-4bde-9b1b-573ad78e7581`, count 5·한글·scroll 160을 유지하면서 `iOS VSIX Hot Reload passed`와 revision 1을 확인했다. extension host exit 0, 소스 복원 및 실제 기기 PID 소멸을 확인했다. 원시 증거: `temp/testing/ios-net10-reload/editor/`. 번들 agent SHA256 `6fac27006b42ea061df345b17164c26e51eaacd510d3b56d85545159c9d18761`은 SDK 10.0.401 원본과 동일하다.
