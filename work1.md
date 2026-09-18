# PlatformView 재구성 작업계획

## 현재 기준과 다음 작업 (2026-09-18)

**전체 상태는 PARTIAL이다.** 이 절과 아래 현재 상태표·실행 gate가 과거 기록보다 우선한다. 이번 개정은 기존 구현·검증 결과를 반영한 문서 수정이며 새 제품 실행 결과를 추가하지 않는다. 근거는 [지원표](Doroti/docs/platform-views/support-matrix.md), [공통 계약](Doroti/docs/platform-views/contract.md), [iOS 계약](Doroti/docs/platform-views/ios.md)이다.

이번 개정의 효과 범위는 **앱 안의 native view와 앞선 Doroti raster를 함께 흐리는 `PlatformEffect`**다. AppKit의 기존 `NSVisualEffectView`의 `BehindWindow` 창 배경과 Skia 내부 `BackdropFilter` 구현은 이 기능의 완료 증거로 계산하지 않는다. work1은 attachment·합성·입력·효과·retirement를, [work2](work2.md)는 WebView controller·탐색·JS·profile·콘텐츠 정책을 소유한다. `doroti/webview`의 초기 HTML 표시와 합성 fixture가 존재해도 work2 공개 API가 완료된 것은 아니다.

### 현재 iOS 전략과 검증 범위

- 이번 대화에서 정한 공개 API 방향에 따라 `UIKitPlatformBlurView`는 **공개 `UIVisualEffectView + UIBlurEffect + UIViewPropertyAnimator.FractionComplete`**를 사용한다. `UIKitGaussianFilter`, 내부 subview/KVC 필터 조작 및 블러 전용 Objective-C 예외 변환 강제는 제거됐다. 아래의 Flutter 내부 Gaussian 전략은 과거 이력이다.
- common logical sigma `[0,16]`을 16으로 나눠 UIKit intensity `[0,1]`로 적용한다. 고정 Light 재질의 tint도 함께 보간되며 실제 Gaussian 반경을 지정하지 않는다. 1개 isotropic `MatchCommon`, saturation=1을 지원하고 `ExactSigma`·일반 Gaussian native-backdrop 요청은 거부한다. Reduce Transparency에는 명시적 `SolidTint`를 사용한다.
- 강도 변경은 보관한 animator를 갱신한다. bounds·window·appearance·앱/Scene 복귀 때 재설정하며, 해제 시 animator와 observer를 정리한다. 강도 0은 효과를 제거한다.
- iPhone 18 Pro / iOS 27 Simulator에서 네 강도(.25/.375/.75/1)·두 테마, 동일 프로세스 앱 복귀, 7개 효과/입력/수명 장면을 통과했다. 테마/복귀 ROI 차이는 0이다. 별도 전체 보정 캡처 3회가 통과했고, 1→.375와 최초 .375, 0과 원본의 픽셀 차이도 두 테마에서 0이다.
- 전체 Mono interpreter 실행에서는 GC가 `PendingFrame`의 잘못된 Frame/Drawable 참조를 검출했다. iOS 27 Debug 프로필은 기본 `MtouchInterpreter=all,-Doroti.Host.Maui`로 **호스트만 Mono AOT**를 적용한다. 보정 3회 중 2회는 GC 진단을 켰으며 기본 프로필 빌드도 경고/오류 0개다. 이는 확인된 실행 경로의 우회이며 런타임 내부 수정이나 NativeAOT 검증이 아니다. 명시적 `MtouchInterpreter=all`은 우회를 덮어쓰므로 정상 재현 명령에서 제거한다.
- 현재 공개 animator의 실기기·NativeAOT, full E3·두 owner·device loss·성능·한국어 IME/VoiceOver/native-origin GestureArena 승인은 남아 있다. 이전 private-filter 실기기 결과를 현재 효과의 완료 증거로 옮기지 않는다.

재현은 [iOS 검증 안내](Doroti/validation/platform-views/ios/README.md), 현재 증거는 `Doroti/artifacts/platform-views/2026-09-18/ios/public-blur/`의 `appearance/`, `gc-fix/aot-run-{1,2,3}/`, `gc-fix/appearance/`를 따른다. 최초 GC 실패도 보존한다.

### 잔여 PlatformEffect 작업 순서

| 순서 | 작업 | 이번 단계의 종료 조건 |
|---|---|---|
| 1 · AppKit 구현 | 최소 live WKWebView factory/attachment와 `NSVisualEffectView + NSVisualEffectBlendingMode.WithinWindow` 기반 효과를 공통 plan/session에 연결. `PlatformEffectSupport.Unsupported`는 실제 구현·probe가 준비된 뒤 갱신 | 같은 owner의 WKWebView+raster가 흐려지고 child가 선명한 캡처, DOM animation/scroll freshness, clip/이동/제거/재생성·입력 통과/차단. Graphite/Ganesh별 결과 분리 |
| 2 · Web 제품 연결 | 이미 있는 protocol v2/CSS effect adapter를 main DOM registry·worker multi-canvas·resource/placement ACK/close에 연결 | 실제 WebGPU/WebGL 제품에서 iframe→effect→foreground 순서, iframe identity, live pixels, 2 owner·DPR·context loss·late ACK. 독립 DOM harness 성공과 구분 |
| 3 · Android 실행 검증 | 기존 WebView factory·RenderNode/RenderEffect 경로를 실제 지원 OS/provider에서 검증. sample 누락이나 비용 문제가 재현된 부분을 수정 | 실제 WebView animation/scroll이 효과에 반영되는 픽셀과 입력/IME·회전/복귀·자원 비용. API31+ 코드 존재나 일반 View.Draw 성공만으로 완료하지 않음 |
| 4 · 기존 경로 마감 | WindowsAppSDK·Qt Quick·iOS의 기존 통과 증거를 재사용하고 남은 R4/R7·공통 시각 허용편차·배포를 검증 | 현재 source/환경의 잔여 gate와 한계가 명확. 변경·실패 근거 없이 장기 반복 검증하지 않음 |

Windows MAUI, Mac Catalyst, iOS Ganesh, Qt Widgets는 각각 별도 미연결/미지원 경로다. WindowsAppSDK·iOS Graphite·Qt Quick의 성공을 전용하지 않는다. 따라서 AppKit/Web만 끝내면 전체 완료되는 계획은 아니다. 아래 R0~R7은 의존성과 종료 기준이며, 구현이 있는 단계는 남은 범위부터 진행한다.

## 이전 실행·설계 기록 (2026-09-14~17)

다음 기록의 “현재”, “최신”, TODO/PASS 및 private API 허용 문구는 **기록 당시 상태**다. 현행 iOS 전략은 위 공개 animator 방식이며, 과거 반경 수치·실기기/NativeAOT·실패 기록은 해당 당시 구현에만 적용한다.

<details>
<summary>이전 iOS·Linux·Windows 실행과 초기 계획 펼치기</summary>

## 당시 실행 업데이트 — iOS 27 Scene 지원 (2026-09-17)

사용자의 iOS 27 지원 요청으로 `MauiUISceneDelegate` 기반 `DorotiMauiSceneDelegate`와 명시적 Scene configuration을 연결했다. 테스트 앱과 새 앱 템플릿의 Info.plist가 같은 단일-window Scene manifest를 사용한다. AppDelegate가 delegate 타입을 직접 참조하여 trimming/AOT에서 문자열 등록만 의존하지 않는다. Metal의 활성 상태·제출 중단/재개와 blur 재적용은 각 WindowScene의 활성화 알림을 따른다. legacy window 경로의 앱 알림 fallback은 유지한다.

- Xcode 27.0(27A266a), .NET SDK 10.0.400, iOS SDK pack `27.0.10539-xcode27.0`으로 `net10.0-ios27.0` 프로필을 구성했다. 이 프로필은 현재 제공되는 .NET 10/Mono 바인딩을 선택하고 기존 .NET 11 NativeAOT 프로필과 구분한다. **버전 검사 우회 없이** simulator/device Debug 빌드가 경고·오류 0개로 성공했다. 공식 SDK 팩은 아직 Xcode 27 preview 표시이므로 해당 안내만 프로필에서 억제한다.
- iPhone 18 Pro / iOS 27 Simulator: 실제 foreground-active MAUI Scene 생성, live WKWebView, blur 강도 4단계/양쪽 테마 및 7개 합성·입력·수명 장면 PASS. 테마별 효과 ROI RGB 차이는 모두 0이다. 설정 앱으로 background 전환 후 **동일 PID**로 복귀했고 효과 ROI 차이도 0이었다. 이어지는 강도 변경과 7개 장면이 실행되어 복귀 후 Metal 제출 재개도 확인했다.
- 연결된 iPhone 12 / iOS 26.6.1: 같은 iOS 27 SDK/Scene 구성의 Debug/Mono interpreter 앱 설치·실행, 4단계/양쪽 테마·7개 장면 PASS. 테마별 ROI 차이 0. iOS 27 실기기 실행으로 계산하지 않는다.
- 최초 자동 검사는 WebKit 첫 animation frame 전에 고정 400ms 비교를 하여 실패했다. foreground-active Scene을 기다리고 최대 8초 동안 실제 DOM 이동을 확인하도록 수정했다. 실패 기록은 보존했다.
- 새 앱 템플릿, simulator target의 TFM 선택, 세 RID의 iOS 27 manifest, shared/template iOS profile을 함께 갱신했다. 최소 OS 버전은 15.0이며 다중 Scene은 활성화하지 않았다. iOS 27 NativeAOT·실기기·배포/성능·multi-owner 전체 승인은 별도다.

재현 명령은 [iOS 검증 안내](Doroti/validation/platform-views/ios/README.md), 실행 로그·캡처·프로필은 `Doroti/artifacts/platform-views/2026-09-17/ios/ios27/`에 둔다. 아래의 iOS 27 시작 실패/도구 불일치 기록은 이전 실행 이력이며 이 절이 최신 상태다.

## 당시 설계 변경 — iOS Flutter 방식 채택 (2026-09-17)

사용자의 **“작업계획을 고쳐서 플러터 같은 방식으로”** 요청에 따라 iOS backdrop의 공개 API 한정 조건을 변경한다. 선택 전략은 `UIVisualEffectView`가 구성한 내부 backdrop의 `gaussianBlur` 필터를 복사하고 KVC로 `inputRadius`를 설정하는 Flutter iOS PlatformView 방식이다. 아래 공개 material/animator 보정은 이전 실험 이력이며 새 전략의 완료 증거가 아니다. **새 전략은 C#/.NET iOS로 구현했고 제한된 실기기·시뮬레이터 검증을 통과했다. iOS 전체 상태는 PARTIAL**이다.

근거는 Flutter 엔진의 [PlatformViewFilter 구현](https://api.flutter.dev/ios-embedder/_flutter_platform_views_8mm_source.html)이다. 내부 backdrop/effect subview를 식별하고, `gaussianBlur`와 숫자형 `inputRadius`를 확인한 뒤 필터를 복사한다. backdrop에는 해당 blur 필터만 남기고 내부 effect subview 배경을 투명하게 하여 기본 material의 tint·채도 보정을 제거한다. `UIVibrancyEffect`는 이 블러 경로에 필요하지 않으며 공통 전경에 자동 적용하지 않는다.

| 단계 | 변경 범위 | 완료 조건 |
|---|---|---|
| iOS-FX1 내부 adapter | `UIKitPlatformBlurView.cs`의 Light preset/animator 보정을 내부 Gaussian adapter로 교체. 비공개 접근을 `UIKitGaussianFilter`에 모으고 .NET iOS Foundation/NSCopying 바인딩과 기본 Objective-C 예외 변환을 사용한다. 후속 사용자 요청에 따라 별도 `.m` shim·정적 라이브러리 빌드는 제거한다. 예외 변환을 끄는 앱 설정은 transitive target에서 거부한다. | private class 직접 생성 없이 UIKit이 만든 객체의 구조·필터·숫자 반경·복사 가능 여부를 검사. 구조 불일치·KVC 예외를 명시적 unsupported 결과로 반환하며 앱 종료나 성공처럼 보이는 무효 blur가 없어야 한다. |
| iOS-FX2 의미·협상 | `UIKitPlatformViewFactory.cs`, `UIKitPlatformViewHost.cs`에서 실제 probe 결과를 capability와 lowering에 반영. `MatchCommon`의 기존 logical sigma를 radius에 매핑하고 native content/theme/identity를 유지한다. | 우선 1개 isotropic effect·sigma ≤16·기본 saturation 범위 유지. 직접 radius를 설정했다는 이유로 `ExactSigma`나 임의 Gaussian BackdropFilter를 자동 승인하지 않는다. 지원 불가 시 사유를 반환하고 SolidTint는 명시적으로 선택한 경우만 사용한다. |
| iOS-FX3 합성·수명 | owner-local R/N/R/N/effect/foreground 순서와 clip·shield·GPU retirement를 유지. 효과 객체는 유지하고 필터·반경 갱신을 처리한다. resize·appearance·foreground 복귀 때 UIKit이 내부 필터를 재구성하는 경우를 감지·재적용한다. | live WKWebView와 앞선 raster/native를 sample하고 effect 자신·선명한 child는 제외. Flutter의 view별 clipping을 그대로 복사하여 공통 E3 의미를 약화시키지 않는다. 이동·제거·재생성·실패 rollback·자원 해제 검증. |
| iOS-FX4 픽셀 검증 | `UIKitBlurCalibration.cs`, `PlatformViewEvidence.cs`, iOS 분석/캡처 스크립트를 새 adapter에 맞춘다. 같은 크기·DPR·색공간의 Skia Gaussian reference와 비교한다. | strength .25/.375/.75/1에서 edge-spread·색상·대비·explicit tint 비교. 고정 source의 Light/Dark, 연속 증가·감소, 최초 설정·재설정 모두 확인. 강도 간 동일 캡처는 PASS 금지. 테마에 반응하는 source 변화와 effect 자체 색 변화를 구분한다. |
| iOS-FX5 제품 승인 | 실제 E1/E2/E3, native DOM animation/scroll, sharp child, input pass-through/shield, effect 이동·resize·제거/복구, 앱 background/foreground와 device/simulator를 별도로 검증한다. | 실행한 iOS/Xcode/.NET·기기/renderer·소스/패키지 hash, 구조 probe 및 실제 pixel 결과를 기록. 과거 public-material 또는 hierarchy-only PASS를 새 전략의 visual PASS로 전용하지 않는다. 초기 구조 probe와 빌드 성공만으로 전체 지원을 선언하지 않는다. |

**변경된 허용 범위:** 이 iOS adapter에 한해 Flutter 방식의 내부 UIKit/CAFilter 접근을 허용한다. 이를 Apple의 공개 radius API로 문서화하지 않으며 OS 업데이트 시 구조가 달라질 수 있다는 의존성을 지원표에 명시한다. AppKit·Android 등 다른 플랫폼의 비공개 API 사용 범위는 이번 결정으로 확장하지 않는다. 앱 심사·미래 OS 호환성을 Flutter 사용 사실만으로 보장하지 않는다.

현재 개발 환경은 작업 중 Xcode 26.6에서 27.0으로 변경되었고 설치된 .NET iOS pack은 26.6을 요구한다. 사용자 라이선스 동의는 완료되었다. 새 전략 승인 전 지원되는 Xcode/.NET 조합을 맞추며, `ValidateXcodeVersion=false` 진단 실행을 정식 도구 조합 검증으로 계산하지 않는다. 이전 animator 강도 갱신 실패는 보존하고 새 구현으로 검증한다. C#에서 의도적인 잘못된 KVC 키의 `ObjCException`을 NativeAOT 실기기에서 잡는 것까지 확인했다.

### 당시 전략 실행 결과 — C#/.NET iOS

- `UIKitPlatformBlurView.cs` / `UIKitGaussianFilter`에서 UIKit/Foundation KVC 및 `NSCopying` 프로토콜 바인딩으로 필터 복사·반경 설정·기본 material 색 보정 제거를 구현했다. 사용자 요청에 따라 `native/UIKitBackdropBlur.m`과 clang/libtool/static-library 경로는 제거했다. C#이라고 해서 UIKit 내부 구조 의존성이 사라지는 것은 아니다.
- 초기 C# 이관에서는 일반 `NSObject.Copy()`가 내부 필터를 NSCopying으로 인식하지 못했다. 프로토콜 wrapper를 통한 복사로 수정했다. NativeAOT iPhone 12에서 잘못된 KVC 키의 Objective-C 예외를 `ObjCException`으로 잡는 것과 잘못된 구조 거부를 확인했다. 예외 변환을 끄는 앱 설정은 transitive target에서 거부한다.
- iPhone 12 / iOS 26.6.1: strength .25/.375/.75/1 → 반경 4/6/12/16 전달, 실제 강도별 픽셀 변화와 Light/Dark RGB MAE <1/255, 7개 effect hierarchy/input/lifecycle 장면 PASS. 독립 sRGB 패턴의 측정 sigma는 4.25/6.25/11.5/15pt, 같은 sigma의 Skia 기준 RGB MAE는 1.29/1.10/0.78/0.64이며 테마 차이는 0이다. 이전 파일을 재사용하지 않았는지 기기 파일 수정 시각을 launch 시작 시각과 비교했다.
- iPhone 17 Pro / iOS 26.5 Simulator: C# Debug/Mono interpreter 빌드, 동일 4-strength/양쪽 테마와 7개 기능 장면을 확인했다. 강한 blur에서 화면 대부분이 평탄해지는 fixture의 전체 ROI 차이가 0.974/255라 최초 >1 freshness 기준은 실패했다. freshness 기준을 >0.1로 검토·수정했고, 이력은 보존했다. 반경 정확성 검증은 별도의 Gaussian reference gate다. 이전 animator의 0 차이는 여전히 실패다.
- Xcode 27에서 생성한 iOS 27 Simulator 앱은 기존 Scene lifecycle 미적용으로 UIKit 시작 중 중단됐다. 블러 구현 실행 증거로 계산하지 않는다. 현재 .NET iOS pack/Xcode 불일치 때문에 검증 빌드에만 `ValidateXcodeVersion=false`를 사용했으며 제품 기본값은 변경하지 않았다. 정식 도구 조합, iOS 27 Scene 전환, full E3/두 owner/성능·물리 입력 승인은 남아 있다.
- 새 실행 기록은 `Doroti/artifacts/platform-views/2026-09-17/ios/flutter-blur/`의 `managed-*`에 보존했다. 초기 native bridge의 결과와 최종 C# 결과를 구분한다.

## 당시 실행 업데이트 — iOS UIKit (2026-09-17)

사용자의 iOS 작업 및 연결된 아이폰 검증 요청으로 HEAD `53ee2a7b1c20479bfb6eb0f4e119275fd1f8a82a`의 clean worktree에서 시작했다. **iOS 전체 상태는 PARTIAL**이며 아래 UIKit/Catalyst 미구현 기록 중 iOS Graphite에 해당하는 부분을 갱신한다. Catalyst와 iOS Ganesh는 별도다.

- R3/R5: UIButton·UITextField·WKWebView를 owner-local UIView에 유지하고 Doroti 중간/전경을 투명 CAMetalLayer로 합성한다. 공통 plan/session, commit된 shield, 논리 좌표의 rect clip, 같은 Metal queue의 GPU retirement를 연결했다. GPU 제출 이후 실패에도 native lease를 보존한다. 전경 triple-buffer 추정 저장공간은 256 MiB로 제한한다.
- 실기기에서 새 idle 장면 뒤 이전 완료 장면이 replay되어 blur 해제가 되돌아가는 문제를 재현했다. native 합성 및 제거 프레임이 진행 중일 때 다음 제출을 GPU 완료 이후로 미루도록 수정했다. UI thread를 GPU 완료까지 동기 대기시키지 않는다. 일반 raster 경로의 3-frame 상한은 유지한다.
- R6/R-E: 실제 WKWebView hierarchy + Metal 전경 + 공개 UIVisualEffectView/UIBlurEffect를 선택했다. PlatformEffect의 MatchCommon/ExactSigma 의도를 immutable snapshot까지 보존한다. 1개 isotropic MatchCommon blur를 지원하며 일반 Gaussian BackdropFilter·ExactSigma·saturation 변경은 거부한다. Reduce Transparency에서는 명시적 SolidTint 선택이 필요하다. 공통 시각 허용편차 승인은 아직 없다.
- 검증: iPhone 12 / iOS 26.6.1 / Apple A14 GPU에서 NativeAOT 앱 설치·실행, 합성 장면 10개, native identity/편집 상태, UIKit hit-test 및 programmatic focus/input, **native handle 해제 완료를 기다리는 create/dispose 100회**를 통과했다. WKWebView DOM animation/scroll, blur toggle, shield/pass-through, 두 WebView, effect 이동·제거·재생성도 수정 후 통과했다. 실기기 캡처에서 live WebView가 material 내부 픽셀을 바꾸며 Doroti child가 선명한 것을 확인했다. 이는 물리 touch/한국어 IME/VoiceOver 승인과 구분한다. 공통 계약 11개도 통과했다.
- 환경: 만료된 기존 테스트 앱 provisioning을 기존 Xcode 팀으로 갱신했고, 사용자가 아이폰의 개발자 신뢰를 완료했다. `dotnet build`의 net11 결과는 CoreCLR이므로 NativeAOT로 계산하지 않았다. 별도 `dotnet publish`의 ILC 실행 및 `UseNativeAot=true` 링크 기록과 기기 실행을 확인했다. 시뮬레이터는 iPhone 17 Pro / iOS 26.5에서 별도 검증했다.
- 잔여: native-origin GestureArena, full Tab/한국어 composing/VoiceOver, 두 제품 owner·device loss·전체 성능 예산 및 공통 시각 유사성은 미승인이다. work2의 WebView navigation/JS/profile 공개 API는 이번 범위에 넣지 않았다. 이전 실기기 효과 실패는 성공 기록으로 덮어쓰지 않고 보존했다.

현재 계약은 [ios.md](Doroti/docs/platform-views/ios.md), 재현은 [iOS 검증 안내](Doroti/validation/platform-views/ios/README.md), 실행 로그·캡처·실패·소스 hash는 `Doroti/artifacts/platform-views/2026-09-17/ios/rearchitecture/`에 둔다.

**후속 블러 외형 수정:** 사용자가 지적한 다크/라이트 차이는 기존 strength별 SystemMaterial 4단계 매핑에서 재현했다. effect view만 Light로 고정하고 `UIBlurEffectStyle.Light`를 `UIViewPropertyAnimator`로 연속 보간하는 전용 `UIKitPlatformBlurView`로 교체했다. iPhone 12 실측 `sigma ≈ fraction × 30pt`로 보정했으며 공개 API만 사용한다. native source와 앱 전체의 테마는 강제하지 않는다. 검정/흰색·색상·체커 패턴에서 기존 SystemMaterial의 테마별 RGB MAE는 160.83/255, 고정 Light는 0이었다. sigma 12의 Skia Gaussian 대비 오차는 16.26/255로 UIKit 자체 색 보정이 남으므로 exact Gaussian으로 광고하지 않는다. `UIVibrancyEffect`는 전경 색 표현용 별도 기능이므로 공통 blur 기본값에 추가하지 않았다. 후속 빌드·보정·제품 캡처 기록은 `Doroti/artifacts/platform-views/2026-09-17/ios/blur-match/`에 보존한다.

후속 제품 검증에서는 기존 animator의 fraction만 갱신할 때 강도별 이미지가 같아지는 실패를 검출했다. 효과 hierarchy/lifecycle 7개 PASS와 별개로 이 픽셀 검증은 FAIL이며, 테마 일치 PASS로 전용하지 않는다. 강도 변경 시 animator를 재생성하는 수정의 NativeAOT publish는 성공했다. 일시적인 Xcode 라이선스 차단은 사용자 동의로 해소했으며, 이후 재검증에서도 강도별 픽셀 gate는 FAIL했다(`product-strength-rebuild/`). 공개 API 실험은 승인하지 않고, 위 최신 설계 변경에 따라 Flutter 내부 Gaussian 방식으로 교체할 계획이다.

## 당시 실행 업데이트 — Linux Qt (2026-09-14)

사용자의 Linux Qt 구현 요청으로 HEAD `20298446d098b6fa1325fd90621c3063b080b1a0`의 clean worktree에서 시작했다.
이 절은 아래 Windows 실행 및 설계 시점의 Linux `notVerified` 상태를 갱신한다. **Linux Qt 전체 상태는 PARTIAL**이다.

**반복 횟수 변경:** 후속 사용자 요청에 따라 이후 Linux Qt 수명·GPU·resize 반복 검증은 **10회 기준**으로 수행한다. 아래 100회 결과는 요청 전에 이미 종료된 실행 이력이며 다시 실행하지 않는다. 검증 스크립트 기본값도 10회로 변경했다.

**Release 실행 오류 수정:** `dotnet run --project ./DorotiTestbedApp/linux/DorotiTestbedApp.Linux.csproj -c Release -r linux-x64`에서 `Qt6ShaderTools`를 찾지 못하는 오류를 재현했다. Debug 캐시에만 적용했던 도구 경로 의존성을 제거하고, Qt 6.4 호환 형식의 Vulkan `.qsb`를 소스·hash metadata와 함께 번들에 포함했다. 일반 Debug/Release 빌드에는 ShaderTools/qsb 설정이 필요 없고 셰이더 소스를 수정할 때만 재생성한다. 같은 Release 명령으로 앱 시작·10회 resize·정상 종료를 확인했으며 Release WebView/effect 자동 검증 18개도 PASS했다. 로그는 `Doroti/artifacts/platform-views/2026-09-14/linux-qt/release-fix/`에 보존했다.

- R0: 현재 Ubuntu 26.04 / Qt 6.10.2 환경에서 기준 빌드는 성공했으나 실행은 Pointer ABI 검사(120/168바이트 불일치)로 실패했다. 이를 수정했다. 사라진 과거 Quick/Widgets 검증 파일을 PASS로 복원하지 않고 새 검증 소스와 README를 작성했다. 변경 전 성능 baseline은 없다.
- R3/R5: Quick의 fractional geometry를 보존하고, 표시 중인 P image를 staging으로 덮어쓰지 않도록 bank를 분리했다. 취소·실패·resize와 QSG wrapper 수명, 1,024개 GUI queue 상한, 실제 frameSwapped/미교환 프레임 supersede → 공통 session 연결을 구현했다. basic loop와 queue drain은 유지했다.
- R6/R-E: 선택적 WebEngine Quick의 초기 HTML attachment와 앞선 R/N/R/N 전체를 sample하는 live ShaderEffectSource + 두 Gaussian GPU pass를 연결했다. effect/선명한 child는 sample에서 제외하며 native identity를 보존한다. 1개 isotropic effect, sigma ≤32, sample ≤4M physical pixels로 제한한다. work2의 navigation/JS/profile 공개 API는 별도다.
- 검증: XWayland와 native Wayland에서 실제 WebView animation/blur, native click·shield·wheel·편집 상태, 전경 버튼, 두 WebView, effect 이동·제거·재생성의 **19개 자동 gate가 각각 PASS**다. 두 실행 모두 Vulkan validation layer의 실제 로딩을 확인했고 오류가 없었다. 실제 Qt GPU queue에서 100회 submit 뒤 commit 거부/resize 취소·복구와 budget 거부, native two-owner·stale/prepare/100회 수명·queue close 계약 및 공통 계약 fixture도 PASS다. Testbed/template의 native 소스를 동기화했다.
- 추가 완료 이력: 실제 Material Quick Controls의 10개 합성 장면 캡처와 100회 해제/재생성도 native 해제 완료를 기다리는 방식으로 PASS했다. Widgets는 별도 빌드·20회 resize·정상 종료를 확인했고, WebEngine을 제외한 Quick template 빌드도 PASS했다. 이 반복 횟수들은 위 사용자 요청 이전의 결과다.
- 잔여: 빠른 XWayland resize에서는 Qt WSI가 757×677 swapchain을 요청할 때 X surface가 720×640을 보고한 `VUID-VkSwapchainCreateInfoKHR-pNext-07781` 1건을 재현했다. 이 실패는 남겨 둔다. llvmpipe 결과를 물리 GPU·성능 승인으로 승격하지 않으며, native-origin GestureArena/전체 Tab·한국어 IME·Orca·device loss·공통 시각 허용편차·배포/NativeAOT 승인은 남아 있다.

현재 계약은 [linux-qt.md](Doroti/docs/platform-views/linux-qt.md), 재현은
[Quick 검증](Doroti/validation/linux-qt-quick/README.md)과 [관리 ABI/Widgets 검증](Doroti/validation/linux-qt-contract/README.md),
이번 명령·실패·hash·캡처는 `Doroti/artifacts/platform-views/2026-09-14/linux-qt/rearchitecture/`에 둔다.

## 0. 2026-09-14 실행 업데이트 — Windows 우선

사용자의 이번 구현 지시에 따라 작업을 시작했다. 실제 출발 HEAD는 `ed98776992f7a084fd3f6c0a11b00a6baa569e49`이며 작업 시작 시 worktree는 clean이었다. 아래의 `227a0b4...`와 "계획 작성만" 문구는 앞선 설계 시점의 기록이다. **현재 전체 상태는 PARTIAL**이며 이 실행표가 이전 TODO 상태보다 우선한다.

| 단계 | 이번 구현 / 검증 | 현재 상태 |
|---|---|---|
| R0 | 현 소스와 누락된 검증 source 확인, 20분 timeout wrapper·공통/Windows/Web gate 재구성, 현재 artifact 생성 | Windows 현재 증거 확보; 이전 source와 동일한 성능 baseline 없음 |
| R1 | descriptor/strategy/input/effect/representation/transport/ack DTO, owner allocator, 기존 request/channel과 같은 coordinator, client late-create disposal completion | PARTIAL — mutable settings·public keep-alive·Flutter gesture facade 이관 남음 |
| R2 | 불변 capability/identity snapshot → 순수 Analyze → 별도 Admit, effect segment/sample bounds, backend effect limits 분리, Windows 보수적 clip crop | PARTIAL — 전체 mutator/coverage/damage shadow comparison·pixel golden 남음 |
| R3 | Windows 실제 terminal을 공통 session에 연결. Android/Qt 완료 경로와 AppKit prepared retirement도 source 연결. 실패/취소 뒤 GPU retirement lease 유지, pending 3-frame 상한 | Windows live 및 공통 계약 PASS; 다른 host build/source만 확인, 전체 fault matrix 남음 |
| R4 | Windows WebView mouse/capture/hover/cursor/focus 및 committed shield, 기존 HWND gallery OS SendInput 회귀 | PARTIAL — native-origin GestureArena·pen/touch·full Tab/한국어 IME/UIA 남음 |
| R5 | WindowsAppSdk 새 CompositionVisual attachment, Android native WebView, 각 기존 host 공통 계약/세션 대응, Web DOM effect protocol v2 | PARTIAL — Web Worker 제품 연결·UIKit/Catalyst·Windows MAUI 별도 adapter 남음 |
| R6 | Windows CoreWebView2CompositionController + Windows.UI.Composition tree 실제 연결, environment/core 수명 보존, bounded readback/upload 선택 경로 구현 | Windows 제한 live scene 통과; 전체 플랫폼 전략 비교·WebView 기능·성능 승인 남음 |
| R-E | bounded PlatformEffect + MatchCommon strength/tint·sharp child. Windows backdrop effect brush, Android RenderNode sampling, Web CSS adapter | Windows live source/입력/2-native/effect 이동·수명 확인. Apple/Qt adapter·시각 허용편차·saturation 공통 구현 남음 |
| R7 | 현재 Windows 제품 증거와 다른 host build, DOM harness, API/지원표 작성 | PARTIAL — 전체 workload/performance/device-loss/두 owner/물리/NativeAOT 배포 승인 남음 |

공통 계약 fixture는 owner isolation, old generation, 순수 분석의 무보존, admission race, retirement, stale frame, 실패·취소, 늦은 native 생성과 client disposal을 검사한다. Windows에서는 live WebView의 픽셀을 blur하면서 Doroti child가 선명하게 남고, pass-through/shield/전경 click을 실제 제품에서 확인했다. native object를 snapshot으로 바꾸지 않는다. legacy BUTTON/EDIT HWND 위 블러는 여전히 지원하지 않는다.

자세한 현재 계약은 [contract.md](Doroti/docs/platform-views/contract.md), runner별 범위는 [support-matrix.md](Doroti/docs/platform-views/support-matrix.md), 명령은 [검증 안내](Doroti/validation/platform-views/README.md), 이번 실행 기록은 [implementation.md](Doroti/artifacts/platform-views/2026-09-14/rearchitecture/implementation.md)에 둔다. 다른 플랫폼의 build 성공은 실행·물리·시각 유사성 승인으로 승격하지 않았다.

2026-09-14 · 기준 HEAD `227a0b4c7c9534aff2cf2c9edb5b038c9d2656cc`.

설계 기준은 [idea.md](idea.md)다. **계획 작성 당시 수행은 소스·공식 문서 검토와 계획 재작성까지**이며 아래 R0~R7 구현은 시작하지 않았다. 기존 제품 경로는 `PARTIAL`, 재구성 단계는 `TODO`다. 과거 PV-0~PV-10 결과를 지우지 않고 [원본](history/26-09-14/platformview-rearchitecture/work1.original.md)에 보존했다.

현재 요구에 따라 **R6는 플랫폼별 WebView 전략 비교·통합, R-E는 공통 효과 의미와 비주얼 유사성을 목표로 하는 위젯**이다. HCPP 구현은 필수가 아니며 native hierarchy·live texture·bounded readback·GPU compositor 중 적합한 구성을 선택한다. Windows CoreWebView2CompositionController 선택은 유지한다. 상태는 `TODO`/`notVerified`이며 이번에도 제품 코드는 변경하지 않았다.

</details>

## 1. 목표와 범위

목표는 native instance를 보존하면서 `Doroti 배경 → native A → Doroti 중간 → native B → Doroti 전경`을 정확히 합성하고, 화면 순서에 맞는 입력·focus·semantics와 제한된 프레임 비용을 제공하는 것이다. 공통 계약을 추출한 뒤 현재 host를 순차 이관한다. WebView navigation/JS/profile은 [work2.md](work2.md)가 소유한다.

다음 C1~C6는 새 단계에서도 유지하는 제품 수용 기준이다.

| 기준 | 필수 장면과 관측 |
|---|---|
| C1 | native 위 불투명 Doroti 부분/전체 가림·해제, 같은 native identity·편집 상태 |
| C2 | native 위 반투명 Doroti, 배경·전경 이중 alpha 합성 없음 |
| C3 | Doroti 위 native 부분/전체 가림, 영역 밖 Doroti 그림 보존 |
| C4 | R/N/R/N/R, 역순·native/native 및 투명 native 조합은 capability별 승인 |
| C5 | 이동·scroll·clip·DPI·resize 동안 geometry/raster/input 정합성, 실패 frame 보존 |
| C6 | 전경 버튼·메뉴·modal shield, 노출 native 입력, 통과 정책, native-origin drag의 부모 중재 |

`NativeOverlay` B는 중간 성과이며 C1~C6 전체 목표의 대체물이 아니다. live texture는 state·입력·접근성이 검증되면 선택 전략에 포함한다. 정지 Snapshot은 명시적 별도 기능이다. group opacity/backdrop/stretch는 별도 효과표에 조건을 기록하며 지원하지 않는 조합을 생략하지 않는다.

추가 필수 장면 E1은 **live WebView → 부분 backdrop/material effect → 선명한 Doroti child**다. E2는 같은 장면의 native scroll/animation·효과 이동/resize와 pass-through/block 입력, E3는 R/N/R/N/effect/foreground에서 앞선 배경 전체 sample이다. backend가 명시적으로 금지한 효과끼리의 겹침은 성공 장면에 섞지 않고 거부 사례로 검증한다.

## 2. 현재 구현과 검증 상태

구현·빌드·제한 실행·전체 제품 승인은 별개다. 아래는 2026-09-18 기준 소스와 기존 실행 기록을 대조한 상태이며, 현재 문서 수정에서 재실행하지 않았다.

| 영역 | 구현 / 확인된 증거 | 남은 범위 |
|---|---|---|
| 공통 | capability/identity snapshot, Analyze/Admit, ordered effect plan, 공통 session·lease·retirement 및 계약 fixture | 전체 mutator/coverage/damage, host별 실패·두 owner·입력·성능 승인 |
| WindowsAppSDK | HWND DirectComposition과 별도 WebView2 CompositionController 경로의 live backdrop·sharp child·입력·수명 검증 | 두 attachment 종류의 한 frame 혼합은 미지원. 전체 IME/UIA·device loss·성능/AOT 잔여 |
| iOS UIKit Graphite | 공개 animator 효과·WKWebView/Metal 합성, 위 simulator 강도/테마/복귀/수명·보정 검증 | 현재 공개 효과의 실기기/NativeAOT와 full E3·물리 입력·성능 잔여 |
| Linux Qt Quick | 실제 WebEngine Quick·두 Gaussian GPU pass, XWayland/Wayland 각 19 gate 및 Release 수정 검증 | llvmpipe 증거이며 물리 GPU/성능 승인 아님. XWayland 급격 resize WSI 오류·IME/Orca·배포 잔여 |
| Android | native WebView factory·공통 session·제한 backdrop 코드, arm64 host 빌드 | 현재 WebView live sampling·시각·입력/수명/성능 제품 실행 미검증. 기존 spinner 수치를 전용하지 않음 |
| AppKit | NSView/Metal 합성과 별도 창 배경 blur 구현 | inline native `PlatformEffect`는 Unsupported. WKWebView factory·WithinWindow 효과 adapter 및 현재 실행 증거 필요 |
| Web | protocol v2 effect adapter·CSS backdrop-filter·host 빌드, 독립 DOM harness 8개 검사 | main-DOM/worker 제품 연결·multi-canvas ACK/자원 수명·실제 iframe effect pixels 미검증 |
| Windows MAUI | 공통 계약·host 빌드 | native hierarchy/WebView2 composition 결합과 runner별 효과 승인 별도 |
| Mac Catalyst / iOS Ganesh | 별도 renderer/runner 존재 | 신규 UIKit Graphite 효과 adapter의 지원 범위에 포함되지 않음 |
| Qt Widgets | native-child B 전용 경로의 빌드/제한 실행 | interleaving/WebEngine/effect 미지원. Qt Quick 증거 적용 불가 |
| WebView 공개 API | Windows/iOS/Android/Qt의 최소 native WebView 부착·초기 HTML fixture와 Web DOM adapter 존재 | controller·navigation/document·JS bridge·profile·앱 콘텐츠 등 work2 제품 API는 별도 구현/검증 대상 |

현재 계약·지원 문서는 [contract.md](Doroti/docs/platform-views/contract.md), [support-matrix.md](Doroti/docs/platform-views/support-matrix.md), [ios.md](Doroti/docs/platform-views/ios.md), [linux-qt.md](Doroti/docs/platform-views/linux-qt.md)를 따른다. 과거 부재 AppKit artifact와 새 증거를 구분하고, 실행 결과가 없는 조합을 추정 PASS로 복원하지 않는다.

## 3. 실행 순서와 단계별 gate

| 단계 | 선행 | 주 산출물 | 종료 조건 |
|---|---|---|---|
| R0 | 없음 | 현재 상태·환경·소스/evidence ledger, baseline fixture | 기존 결과와 현재 revision 검증 범위가 분리됨 |
| R1 | R0 | lifetime/attachment/capability/input/frame DTO와 API 이관표 | 두 owner·late create·old generation·요청/실제 전략 계약이 명확 |
| R2 | R1 | analyzer, mutator stack, coverage/damage, 순수 planner | 기존 장면 동등성·alpha·clip·unknown bounds 검증 |
| R3 | R1/R2 | 실제 제품이 사용하는 composition session, host adapters | 제출 후 취소·commit 실패·retirement·resize 책임 단일화 |
| R4 | R1, 제품 연결은 R3 | native gesture/focus/IME/semantics bridge | native-origin drag와 클릭·shield·Tab이 같은 제품에서 정확 |
| R5 | R2/R3, 입력 승인은 R4 | Windows/Android/Qt/AppKit 이관, Web/UIKit 연결 | backend별 C1~C6와 지원/제한표 |
| R6 | 계약 R1/R3, 해당 backend R5 | WebView 전략 비교·선택·통합 | 선택한 구성으로 C1~C6·입력·수명·효과·성능 예산 통과. HCPP 강제 없음 |
| R-E | 조사 R1, 구현 R2/R3, WebView 승인 R6 | 공통 effect intent·native/CSS/GPU adapter·시각 보정 | E1~E3·공통 비주얼 유사성·입력·동적 native·제한 검증 |
| R7 | 해당 backend R4/R5/R6/R-E | 제품·성능·배포 승인표와 가이드 | 광고할 각 조합에 현재 증거가 있음 |

Android R4의 ingress/arena 타당성 probe는 R1부터 시작한다. Qt WebEngine Quick 최소 probe도 work2와 조기에 수행해 attachment 계약의 불일치를 확인한다. 이 선행 조사가 R2/R3 구현 순서를 생략하는 근거는 아니다.

R6/R-E의 Android 전략별 backdrop sampling과 Windows 호환 Composition tree 검증도 R1부터 앞당긴다. 실제 WebView+효과 장면으로 후보를 비교하고 효과/입력 요구를 충족하지 못하는 전략은 제외한다. AppKit은 새 native effect adapter 구현, Web은 기존 CSS adapter의 제품 연결, Android는 기존 경로의 실제 WebView 검증부터 진행한다. 공통 reference 비주얼 보정은 각각의 live source 확인 뒤 수행한다.

### R0 — 소스와 증거 기준 고정

- 위 상태표와 원본 기록을 출발점으로 환경·RID·renderer·control 종류·실제 source hash를 기록한다. 없는 증거는 unavailable로 유지한다. 결과 회수가 가능하면 먼저 재사용하고 무조건 긴 검증을 반복하지 않는다.
- `PlatformViewFixture`와 실제 Material `Platform views` 페이지의 C1~C6·scroll·spinner·modal을 묶어 fixture ID를 고정한다. 서로 다른 scene/source/빌드 결과를 한 수치로 합치지 않는다.
- Android 최종 bounded arm64 install/run 및 x64 build/run을 필요한 경우 먼저 확인한다. native-origin drag와 배경-origin drag를 별도 시나리오로 만든다.
- 0/1/4 native, idle·작은 animation·큰 scroll·modal transition을 성능 workload로 정한다. 목표 refresh rate별 예산은 `1000 / Hz` ms이며 p95/p99 허용 회귀와 메모리 상한을 변경 전 기록한다. 현재 검증되지 않은 임의 수치를 기존 PASS 기준으로 만들지 않는다.

Stop: 현재 source에 대응하는 baseline을 얻지 못하면 해당 backend 성능 향상 수치는 미산출로 남긴다. source-only 작업은 계속 가능하다.

### R1 — 공통 계약과 API 일원화

수정 위치: `Doroti.Ui/PlatformViewContracts.cs`, `Doroti.Hosting/PlatformViewCoordinator.cs`, `Doroti.Framework.Services/PlatformViewClient.cs`, `PlatformViewChannelAdapter.cs`, `Doroti.Framework.Widgets/PlatformView.cs`.

- creation descriptor, mutable settings, attachment/frame placement를 분리한다. owner allocator·explicit ID 호환·dispose completion·same-owner keep-alive를 정의한다.
- capability를 representation/transport/effect/input/observation과 runtime/control 조건으로 확장한다. 기존 `PlatformViewRequest/Support` facade의 대응과 obsolete 시점을 문서화한다. API 이름 정리는 이 계약 확정 후 한다.
- `PlatformPreferred` 전략 정책과 `PlatformEffectSupport`를 정의한다. effect intent·정규화 strength/tint/saturation·MatchCommon 기본 정책, source sampling·시각 근사·runtime availability를 분리한다. NativeCompositor/HCPP는 후보이며 필수 profile이 아니다.
- `PlatformCompositionToken`과 commit 결과에 identity/geometry/resource generation·ack provenance를 연결한다. 물리 원자성을 약속하지 않는 backend를 표현한다.
- Flutter 이식 `AndroidView/UiKitView/AppKitView/PlatformViewLink/PlatformViewSurface`와 typed `PlatformView`의 실제 호출 경로를 표로 작성한다. 두 instance를 만들지 않는 facade로 이관한다. 지원하지 않는 touch/texture/HCPP method는 명시적 오류를 유지한다.
- native 생성 시 view type뿐 아니라 host가 attach 가능한 native 종류를 검사한다. generic Button factory가 통과했다는 이유로 임의 SDK control을 등록하지 않는다.

완료: two-owner isolation, create 중 dispose/owner close, ID reuse, 늦은 focus/event, 취소 후 native 회수, detach/reattach 계약 검사. 공개 SDK/native handle 노출 없음. 입력 기본값 변경은 다음 R4 제품 gate까지 보류한다.

### R2 — 장면 분석·효과·damage 계획

수정 위치: typed layer/scene contracts, `PlatformCompositionPlan.cs`, `SkiaPlatformRasterContent.cs`, `SkiaSceneRenderer.cs` 및 검증 source.

- analyzer는 ordered mutators/group scope와 logical/device 좌표를 보존한다. backend 제약인 Android 단일 Gaussian·sigma를 common parsing에서 분리한다.
- `PlatformEffectSegment`를 raster/native/shield와 같은 ordered plan에 추가한다. sample source는 앞선 배경이며 자기 자신·child를 제외한다. effect output clip과 kernel sample 영역을 분리하고 source grouping이 paint order를 바꾸지 않게 한다.
- planner 입력은 scene + immutable capability/identity snapshot, 출력은 순서 있는 plan이다. host 호출/retain은 R3 admission에서 수행한다. plan 생성과 admission 사이 dispose race를 검사한다.
- immutable picture의 conservative coverage·content revision을 정의한다. 초기에는 실제 clip/viewport, 후속에는 기록된 draw bounds를 사용한다. unknown operation·stroke/filter 확장을 포함한다. R-tree는 측정 후 선택할 내부 최적화다.
- native overlap 밖 raster를 base에 배치하고 overlap을 제외하여 이중 합성을 방지한다. rounded/path clip hole·transparent native·backdrop sample·group opacity는 제한 정책 또는 정확한 lowering을 사용한다.
- stable slot identity와 content/geometry damage를 분리한다. viewport/DPR/GPU generation/effect 변경 시 캐시를 무효화한다. 저장공간은 active·staging·retiring 모두 계산한다.
- 새 분석 결과를 기존 planner 옆에서 비교하고 화면 제출은 기존 경로 한 번만 한다. 불일치는 picture/handle/scope 근거를 기록한다.

완료: R/N/R/N/R 픽셀 기준, 반투명 분할 재합성, fractional clip edge, non-overlap unknown bounds, retained 재사용, backdrop invalidation, 0 native fast path, budget 초과 거부. CPU Skia golden은 수학·raster 검증이며 native 제품 표시 승인이 아니다.

Stop: 분석 불확실성을 근거로 native 위 전경을 base로 내리거나 그림을 누락하면 이관하지 않는다. 보수적 큰 구간으로 유지한다.

### R3 — 실제 frame session으로 연결

수정 위치: `PlatformCompositionSession`과 Windows/Android/AppKit/Qt Draw·Finish 경로, Skia GPU submission/retirement 접점, Web protocol 제안.

- `IPlatformCompositionPresenter`/prepared 계약을 실제 host의 resource prepare·commit·retire에 맞게 확장한다. 사용하지 않는 또 하나의 session을 추가하지 않는다.
- prepare staging, native operation admission, GPU submit, backend commit, optional compositor ACK, GPU/presentation retirement를 분리한다. 기존 R/P 및 성공한 queue submit의 Vulkan observed-state 계약을 유지한다.
- commit 직전 owner/epoch/instance/surface generation을 재검사한다. 실패나 경합에서는 이전 프레임을 유지한다. 제출 후 취소·partial native commit·owner close 경로를 명시한다.
- scene/picture/texture와 native lease를 admission에서 함께 보존하고 마지막 소비 이후 반환한다. 마지막 native 제거·빈 batch·route 전환 때 이전 native/shield가 남지 않는지 확인한다.
- framework/render/UI 작업 queue와 buffer/resource 상한을 둔다. UI thread가 자신에게 보낸 작업을 기다리지 않도록 한다. GPU 대기 중 input/focus queue starvation도 관측한다.
- 하나의 host를 먼저 연결한다. 이번 수행은 사용자 지시에 따라 Windows를 첫 제품 대상으로 하고 Android readback·Qt GPU·AppKit 경로로 계약의 과도한 플랫폼 가정을 검사한다.

완료: prepare 실패, submit 실패, submit 이후 취소, resize 중 supersede, commit 실패, focus reservation 경합, 늦은 ACK, device/context loss·close에서 double free/조기 재사용 없음. 실제 host trace가 공통 session을 통과함을 확인한다. fault injection은 별도 fixture 결과로 표시한다.

### R4 — native-origin gesture와 입력 일관성

- Android wrapper가 down부터 관측 가능한지 확인하고 native가 먼저 side effect를 발생시키지 않는 delayed dispatch 경로를 만든다. arena 승리 시 replay/forward, 패배 시 폐기 또는 platform-specific cancel을 정확히 처리한다.
- 원래 MotionEvent의 pointer IDs/index/action/downTime/time·좌표 변환을 보존한다. pending queue 상한·capture 변경·multi-touch·dispose·modal 진입·재진입 방지를 구현한다.
- DirectNative를 명시적으로 유지하면서 GestureArena를 opt-in한다. native 위 tap은 한 번, native 위 vertical drag는 의도한 parent scroll로, native 내부 horizontal scroll은 해당 정책대로 동작해야 한다.
- PointerInterceptor/shield는 commit된 geometry와 framework hit-test 정책을 따른다. native 안에서 발생한 동일 이벤트를 native·framework 양쪽에 dispatch하지 않는다.
- FocusNode↔native focus, Tab/Shift+Tab, IME client 양보·한글 composing/selection, semantics subtree/중복 action 방지를 각 host adapter에 연결한다.
- UIKit delayed recognizer, desktop wheel/capture, Web same-origin/cooperative 제한은 별도 구현한다. iframe 밖에서 생긴 shield event만으로 iframe 안 drag의 중재를 주장하지 않는다.

완료: 실제 Material page에서 native tap·native-origin drag·background drag·nested scroll·modal·IgnorePointer/AbsorbPointer, 편집 중 scroll·focus 왕복, late/cancel sequence를 재현한다. 자동 입력과 물리 입력·한국어 IME·screen reader 승인을 구분한다.

Stop: native click이 이미 발생한 후 parent에 복제하는 방식이면 GestureArena capability를 켜지 않는다.

### R5 — backend별 이관과 work2 인계

| 순서 / 대상 | 구체 작업 | backend 종료 조건 |
|---|---|---|
| Android | bounded raster/cache/backdrop·surface 재생성을 공통 plan/session에 연결 | 기존 제한 유지, C1~C6·scroll 개선 재현, 최종 revision 증거 |
| WindowsAppSdk | 구현된 HWND DirectComposition/WebView2 CompositionVisual 경로의 잔여 합성·수명 gate 마감 | 각각의 B/C pixel·focus 보존, 혼합 제한 유지, 실제 자원 수명·성능 검증 |
| Linux Quick | Qt 소유 device·P image generation·QSG node·swap terminal 연결 | 현재 basic loop 유지, resize validation 잔여 원인 분리, XWayland/native Wayland 검증 |
| AppKit | 기존 NSView/Metal 합성에 최소 WKWebView와 WithinWindow 효과 adapter 추가 | Graphite/Ganesh별 live source·sharp child·입력/수명 검증. 창 배경 블러 증거 전용 금지 |
| Web | main DOM registry 활성화, worker multi-canvas resource/placement packet·ACK·close 연결 | worker-direct WebGPU/WebGL별 C1~C6, 2 owner·DPR·context loss·iframe identity. DOM harness와 별도 |
| UIKit Graphite / Catalyst | iOS의 구현된 public animator·Scene 경로 유지 및 R4/R7 마감. Catalyst는 별도 adapter 연결 | iOS 현재 simulator 결과 유지, 실기기/NativeAOT 재검증. Catalyst 자동 승인 금지 |
| Windows MAUI / Qt Widgets | runner별 native hierarchy·frame adapter 연결 또는 기존 제한 유지 | WindowsAppSdk/Quick 증거 전용 금지, B-only 조합은 C 미충족 |

선행 플랫폼 결과를 기다리는 동안 다른 backend의 소스 검토·계약 대응은 가능하다. 하지만 이미 통과한 host의 장기 성능 반복은 새 변경/실패가 정당화할 때만 한다.

work2 인계물은 typed attachment·capability snapshot·frame/input policy·dispose completion과 최소 실제 WebView 종류의 attach 검증이다. HWND와 CoreWebView2CompositionController의 RootVisualTarget, QWidget과 Quick item을 다른 attachment 종류로 협상한다. Windows WebView controller는 CoreWebView2CompositionController로 확정하며 WindowsAppSdk/MAUI 모두 적용한다. C 지원이 generic Button에만 있으면 WebView C는 아직 미지원이다.

### R6 — 플랫폼별 WebView 전략 비교·선택·통합

- Android: 기존 native hierarchy/bounded readback, live texture, 공개 GPU surface·HCPP 대응 후보를 비교한다. 실제 WebView scroll/IME/접근성·transparent/media subtree·effect sampling·frame/메모리 예산을 충족하는 구성을 선택한다. HCPP가 유리할 때만 해당 구현을 진행하며 API 34+를 전체 WebView 최소 OS로 강제하지 않는다. 후보별 OS/provider/runtime 요구를 기록한다.
- Windows: CoreWebView2CompositionController를 필수 사용하고 RootVisualTarget·raster visual·backdrop effect의 호환 API family/tree를 확정한다. GPU 전송을 우선 비교하되 bounded upload도 비용과 품질로 판단한다. SendMouseInput/SendPointerInput·cursor/focus·shield를 공통 bridge로 연결한다. WinUI와 Windows Composition 객체를 임의로 혼합하지 않는다.
- UIKit/AppKit: native WKWebView와 Metal 전경 surface·effect view를 같은 owner hierarchy에서 합성한다. Web: iframe/DOM effect와 worker canvas를 browser compositor에 연결한다. Qt: live WebEngine Quick item과 R→P GPU raster/effect를 같은 scene에서 처리한다. 이 경로를 Android Flutter HCPP API 구현으로 부르지 않는다.
- Qt Quick: queue drain/basic loop를 성급하게 바꾸지 않는다. 현재 resize 잔여 문제와 retirement가 해결된 뒤 async handoff의 이득을 측정한다.
- 같은 source/content/환경에서 CPU 전송량, UI/raster/commit p95·p99, memory/retiring backlog를 비교한다. 작은 spinner 수치를 큰 scroll 성능으로 확대하지 않는다.

R6 자체 완료: 실제 WebView로 C1~C6·identity·input·retirement를 통과하고 선택 이유·actual strategy·copy/readback/sample 비용·frame/메모리 예산을 기록한다. 기능·상태 보존을 먼저 통과한 후보끼리 성능을 비교한다. 전체 목표는 R-E와 결합한 E1~E3 및 공통 시각 유사성을 R7/WV-9에서 승인해야 완료된다. 어떤 전략도 충족하지 못하면 `PARTIAL`이며 정지 snapshot·효과 누락으로 성공 처리하지 않는다.

선택은 생성/attach negotiation 시 수행하고 callback·문서·focus identity를 보존한다. runtime 전환이 필요하면 frame/IME/capture 경계와 state-preserving handoff를 검증한다. 새 전략의 성공은 그 전략/renderer의 결과로 기록하며 원래 실패한 경로의 PASS로 바꾸지 않는다.

### R-E — PlatformEffect 위젯과 플랫폼 효과 adapter, 필수 단계

수정 대상: 이미 구현된 `Doroti.Ui` effect DTO/capability, `Doroti.Framework.Widgets`의 `PlatformEffect`, `Doroti.Framework.Rendering`의 typed effect layer, Hosting plan/session, 각 `Doroti.Host.*` effect adapter 및 Web DOM protocol. WebView 패키지와 독립이며 임의 지원 native view 위에서도 재사용한다.

| 세부 gate | 작업 | 종료 조건 |
|---|---|---|
| FX0 계약·probe | 공통 backdrop intent·strength/tint/saturation·MatchCommon 정책, source/clip/input 정의. reference scene·강도별 허용 시각 편차·플랫폼 매핑 규칙 확정 | 의미 보존·시각 유사성 평가표. material/radius 값 동일함을 유사성 근거로 사용하지 않음. source sample 검증 |
| FX1 위젯·프레임 | bounded layout, effect 뒤 선명한 child, owner/effect generation·stable resource, ordered scene payload와 common prepare/commit/retire | rebuild/scroll에서 효과 host 재생성 최소화, 마지막 effect 제거·late frame·2 owner·dispose race 검증 |
| FX2-A AppKit · TODO | 최소 WKWebView attachment와 NSVisualEffectView WithinWindow를 같은 owner hierarchy·plan/session에 연결. 기존 BehindWindow 설정과 수명 분리 | E1~E3 live native/raster sample·sharp child·테마·강도 지원 범위·입력·resize/제거/재생성. API 존재만으로 capability를 켜지 않음 |
| FX2-I UIKit · PARTIAL | 구현된 Light preset + 보관/일시정지한 UIViewPropertyAnimator.FractionComplete 경로 유지. sigma/16을 intensity로 사용하고 비공개 필터 접근 금지 | 현재 simulator 강도/0·감소/테마/복귀 결과 유지, 현재 adapter 실기기·NativeAOT·full E3·두 owner 마감. preset tint와 ExactSigma 미지원 공개 |
| FX3 Web · PARTIAL | 기존 protocol v2/CSS effect adapter를 main DOM·worker multi-canvas·resource/placement ACK/close에 연결. effect는 source iframe 위·sharp child 아래 | iframe + canvas + effect + foreground, same/cross-origin, backdrop-root·parent opacity·DPR·z-order·stale batch. WebGPU/WebGL 제품 검증 |
| FX4 Windows | 호환 Composition backdrop/effect brush와 SpriteVisual을 WebView2/GPU raster tree에 결합 | 실제 WebView pixels blur와 선명한 child, legacy HWND/Mica와 구분, device/resize·runtime availability 검증 |
| FX5 Android · 구현/실행 분리 | 기존 WebView factory·RenderNode/RenderEffect의 실제 OS/provider source sample 검증을 우선하고 재현 실패를 수정 | 실제 source/animation·비주얼 유사성·전송 비용 검증. 별도 surface sample 불가이면 다른 후보 검토. window blur를 inline 효과로 가장하지 않음 |
| FX6 Qt Quick | 앞선 source group의 live GPU ShaderEffectSource + MultiEffect/ShaderEffect, effect/child 제외 | 실제 WebEngine Quick·raster sample, source identity/input 보존, feedback 없음, XWayland/Wayland·GPU lease 검증 |
| FX7 결합 승인 | E1~E3와 공통 체크무늬/글자/사진 reference, 강도·테마별 비교, 동적 source·resize·focus·접근성 | 동일 logical 크기/DPR/색공간에서 blur edge·대비·tint/밝기와 시각 검토. 사전 편차·비용/수명 기준 및 지원표 일치 |

공통 기본값은 MatchCommon과 input pass-through, effect 자체의 focus/semantics 없음이다. child semantics/입력은 유지하고 필요한 전경만 shield로 보호한다. material/blur parameter를 공통 intent로 자동 매핑하는 것은 정상 구현이며 exact-sigma는 advanced opt-in이다. **현행 iOS 계획은 위 공개 animator 방식**이다. 과거 UIKit/CAFilter 예외를 현재 구현 요구로 유지하지 않는다. UIKit preset tint를 제거하거나 numeric sigma를 맞추려고 비공개 필터를 다시 도입하지 않으며 AppKit·Android도 공개 API 범위에서 구현한다.

Web의 `CSS.supports`와 native API 존재는 gate 통과가 아니다. native scroll/video/animation이 바뀔 때 실제 blur도 변해야 한다. secure/protected content나 compositor sampling 경계는 명시적으로 unsupported다. system transparency/고대비 정책에 의한 material 변경 또는 앱 지정 SolidTint는 resolved 상태로 보고하며 blur 성공으로 표시하지 않는다.

Stop: 선택한 조합이 실제 source sample·live 갱신·공통 시각 편차·입력/예산을 충족하지 못하면 다른 전략/adapter를 검토한다. 모두 실패하면 해당 R6+R-E는 `PARTIAL`이다. 정지 snapshot·blur 없는 tint를 정상 근사로 취급하지 않는다. 앱이 허용한 degraded fallback/접근성 대체는 별도로 보고한다.

### R7 — 제품·성능·배포 마감

Linux Qt의 수명·GPU·resize 반복은 후속 사용자 요청에 따라 **10회**를 적용한다. 과거 100회 결과는 재사용 가능한 당시 증거로 보존하며 반복 횟수를 다시 늘리지 않는다. 아래 공통 100회 기준도 Linux에는 이 예외를 적용한다.

- 0/1/4-view × idle/small animation/scroll/modal × 해당 OS/RID/renderer에서 frame percentile·실제 latency 관측 범위·메모리·resource count를 기록한다.
- 두 제품 owner, 100회 제품 attach/detach/create/dispose, 빠른 resize/DPI/cross-monitor, background/resume, device/context loss·close를 수행한다. 이미 동일 source로 통과한 항목은 재사용한다.
- 한국어 IME·접근성·pointer/keyboard/capture는 실제 제품 환경에서 승인한다. 물리 장비가 없으면 `notVerified`; 사용자가 명시한 생략만 `skippedByUser`다.
- opt-in 미사용 앱의 dependency/0-view 비용, Testbed와 생성 template, final publish/install/run, NativeAOT ILC/native link/실행을 따로 검증한다.
- 실제 WebView+PlatformEffect 장면을 포함해 효과 수·sample pixels·추가 GPU pass·native content freshness를 기록한다. 새 위젯을 사용하지 않는 앱과 효과 0개에서 추가 surface/DOM node/pass가 없는지 확인한다.
- 현재 지원/제약/오류/전략 선택을 API 가이드와 지원표에 반영한다. 삭제된 과거 docs를 추정 복구하지 않고 실제 계약과 증거로 새로 작성한다.

완료: 광고할 조합의 R4/R5/R7, WebView의 R6/R-E 결합 증거가 충족된다. 일부 플랫폼·물리·성능·배포가 남으면 전체는 `PARTIAL`이다. 문서 완료와 제품 완료는 별개다.

## 4. 기존 게이트의 인계

| 기존 ID | 새 책임 | 상태 처리 |
|---|---|---|
| PV-0 | R0/R1/R7 지원표·예산 | 기존 기록 보존, 현재 근거로 갱신 |
| PV-1 | R1 registry/controller/channel/lifetime | 구현 기반 유지, 새 facade 이관은 TODO |
| PV-2 | R2/R3 scene·planner·frame | 기존 planner 유지 후 교체 |
| PV-3 | R5 Windows + WebView 필수 R6/R-E | 기존 WindowsAppSdk B/C 결과 유지 |
| PV-4 | R5 Web + R4 | DOM harness와 제품 미완료 분리 |
| PV-5 | R4 + R7 | 기존 shield 성공과 full gesture/IME/accessibility 미완료 분리 |
| PV-6 | R3/R4/R5 Android + WebView 필수 R6/R-E | 최종 bounded 실행 `notVerified` 유지 |
| PV-7/PV-8 | R5 UIKit/AppKit + R4/R7 | runner/증거 별도 |
| PV-9 | R5 Quick/Widgets + work2 WV-7 | Quick 제품 방향 유지, Widgets 이전 경로 보존 |
| PV-10 | R7 | 전체 승인 미완료 |
| PV-X | HCPP 대응 등 추가 전략·최적화와 명시적 Snapshot 기능 | HCPP 자체는 선택. 플랫폼별 적합한 WebView 전략 R6와 공통 효과 R-E는 필수 |

## 5. 검증 실행 규칙

검증 source는 `Doroti/validation/`에, build/capture/trace/report/cache는 `Doroti/artifacts/platform-views/<date>/<target>/<run>/`에 둔다. 원본/개정 이력은 `history/`에 둔다. `[sourceReviewed, build, automated, productLive, physical, nativeAot]`를 독립적으로 기록하고 source hash·dirty state·명령/exit/timeout·실제 renderer/GPU·실패·재개 명령을 보존한다.

모든 build/test/run child는 [.github 지침](.github/copilot-instructions.md)의 **20분 외부 timeout**을 적용한다. 현재 구현에서 사용하는 명령은 다음과 같다. 각 gate의 최근 실행 결과와 source hash는 위 실행 기록에 따로 보존한다.

```powershell
# 저장소 루트. 모든 child에 1200초 외부 timeout을 적용한다.
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/platform-views/Common/Common.csproj --artifacts-path Doroti/artifacts/platform-views/common-build
python Doroti/validation/run-with-timeout.py python Doroti/validation/platform-views/verify-windows-effects.py
python Doroti/validation/run-with-timeout.py python Doroti/validation/platform-views/verify-web-dom.py

# 개별 새 child 명령은 Doroti 디렉터리에서 timeout wrapper로 실행한다.
# python validation/run-with-timeout.py <실제 명령과 인자>
```

Android 명령은 [Android README](Doroti/validation/platform-views/android/README.md), Qt는 [Quick README](Doroti/validation/linux-qt-quick/README.md)를 따른다. 공유 obj/bin을 쓰는 .NET build gate는 순차 실행한다. 기존 증거 재사용 여부는 변경 파일·source hash·환경에 근거하고, 미확인 artifact를 성공 수치로 복사하지 않는다.

## 6. 결정이 필요한 항목과 종료 지점

| 항목 | 결정 단계 | 판단 근거 |
|---|---|---|
| controller 이름·기존 request facade 폐기 시점 | R1 | 모든 기존 caller·호환 API 대응과 실제 lifecycle 검사 |
| draw coverage 메타데이터·index 도입 | R2 | conservative correctness와 planner CPU/메모리 측정 |
| Android gesture delayed-dispatch 방식 | R1 probe/R4 | native tap·nested drag·multi-touch·IME 제품 결과 |
| 공통 frame API의 async/ACK 범위 | R3 | Android readback·Windows UI batch·Qt GPU 세 경로의 실제 소유권 |
| Web canvas 개수·buffer handoff | R5 Web | active runtime topology와 renderer별 C·retirement 검증 |
| 플랫폼별 WebView 전략 선택 | R1 조기 probe/R6 | 합성·입력/IME/접근성·효과·frame budget·memory·안정성·실제 전송 비용 |
| native/Web backdrop sample·material 범위 | R-E FX0/FX2~FX7 | 실제 WebView+effect 장면·동적 source·입력·OS/browser 제약 |
| 플랫폼별 최종 performance/배포 지원 범위 | R0 예산/R7 승인 | 현재 장비·OS·runtime·RID별 증거 |
