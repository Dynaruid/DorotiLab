# PlatformView·PlatformEffect·WebView 설계와 실행 요약

보관 기준일: 2026-09-21 · 정리일: 2026-09-22.

루트 `idea.md`, `work1.md`, `work2.md`의 설계, 누적 실행 결과와 잔여 작업을
요약했다. **전체 상태는 `PARTIAL`이다.** 이 작업은 문서 보관이며 제품 빌드,
테스트, 브라우저·실기기 실행을 새로 수행하지 않았다. 아래 결과는 삭제 전
문서에 기록된 당시 증거이며 현재 checkout의 재검증 결과가 아니다.

| 원문 | 책임 | 상세 보관본 |
| --- | --- | --- |
| `idea.md` | PlatformView 재구성 아키텍처와 전략 선택 | [설계 원문](platformview-webview/idea.original.md) |
| `work1.md` | native 수명·합성·입력·PlatformEffect·제품 승인 | [PlatformView 원문](platformview-webview/work1.original.md) |
| `work2.md` | WebView controller·탐색·JS·profile·리소스·SDK adapter | [WebView 원문](platformview-webview/work2.original.md) |

원문은 바이트 그대로 보존했다. 경로·크기·SHA-256은
[manifest](platformview-webview/manifest.json)에 있다. 원문 안의 상대 링크는
이동 전 저장소 루트 기준으로 해석하며 수정하지 않았다. 이전 재구성 전 문서는
[2026-09-14 보관본](../26-09-14/platformview-rearchitecture/README.md)에 별도로 있다.

최신 날짜의 실행 업데이트를 초기 TODO·설계 제안보다 우선했다. 특히 iOS는
공개 animator와 9월 21일 `requestedSigma / 30` 보정이 기준이며, 과거 private
Gaussian 전략·`sigma / 16` 설명을 현재 선택으로 옮기지 않았다. 별도 후속 문서의
결과까지 합쳐 원문에 없는 완료나 성능 개선을 추가하지 않았다.

## idea.md — 설계 결정

- PlatformView를 native instance 수명, 장면 분석, 합성 전략, 프레임 자원 수명,
  입력 중재의 다섯 책임으로 나눈다. `(OwnerViewId, InstanceId, InstanceGeneration)`을
  유지하며 생성·attachment·document·frame 수명을 혼동하지 않는다.
- 생성 정보와 mutable settings, 매 프레임 bounds/clip/transform을 분리한다.
  detach는 dispose가 아니며 같은 instance는 동시에 하나의 attachment만 가진다.
  취소 뒤 늦게 생성된 native 객체와 owner close도 dispatcher에서 회수한다.
- 불변 capability/identity snapshot으로 순수 Analyze/Plan을 수행하고, session의
  Admit에서 세대를 재검사하고 lease를 확보한다. ordered mutator·group opacity·
  보수적 coverage·damage를 보존하며 알 수 없는 그림을 잘라 비용을 줄이지 않는다.
- 프레임은 prepare → GPU submit → backend commit/선택 ACK → GPU·presentation
  retirement로 관리한다. 제출 후 취소에도 소비가 끝날 때까지 자원을 유지한다.
  마지막 native 제거 프레임의 빈 batch도 commit한다. GPU 완료·host ACK·물리 표시를
  서로 다른 관측으로 기록한다.
- 합성 의미와 전송 방식을 분리한다. `PlatformPreferred`는 live hierarchy/texture,
  GPU copy/import, bounded readback/upload, DOM/canvas 중 실제 기능·효과·입력과
  예산을 만족하는 방식을 선택한다. HCPP 자체는 필수가 아니며 정지 snapshot이나
  효과 누락을 live WebView의 정상 구현으로 취급하지 않는다.
- Windows controller는 `CoreWebView2CompositionController`로 확정했다.
  `RootVisualTarget`, raster와 effect를 호환 visual tree에 연결한다.
  WindowsAppSDK와 MAUI의 runner 결합·검증은 별도다.
- Linux는 시스템 Qt WebEngine과 Quick 직접 adapter·공개 C ABI를 사용한다.
  Qt 소유 device/queue와 R→P GPU copy를 유지하며 자체 Chromium/Qt 배포나
  private texture 추출을 기본 해법으로 삼지 않는다. Widgets 경로는 별도다.
- 입력은 DirectNative/GestureArena/BlockNative를 구분한다. native-origin gesture는
  down부터 중재해야 하며 이미 발생한 native click을 부모에 복제하지 않는다.
  shield, focus·Tab·IME 양보, native semantics와 물리 접근성 검증을 분리한다.

`PlatformEffect`는 같은 owner에서 앞서 그려진 native와 Doroti raster를 sample하고
자기 자신과 선명한 child는 제외한다. 기본은 MatchCommon·입력 통과이며 blur와
shield는 별도다. window backdrop나 Skia 내부 BackdropFilter를 이 기능의 완료로
대체하지 않는다. 강도·채도·tint·실시간 source와 비주얼 유사성을 함께 검사하고,
ExactSigma는 backend별 별도 capability다. SolidTint는 명시적 degraded fallback
또는 접근성 대체이지 정상 blur 성공이 아니다.

## work1.md — 합성·효과 구현과 당시 검증

공통 구현은 descriptor/capability DTO, owner allocator, Analyze/Admit 분리,
ordered effect plan, session·lease·retirement와 계약 fixture까지 진행됐다.
전체 mutator/coverage/damage, mutable settings·keep-alive/facade, host별 fault
matrix와 입력·성능·배포 승인은 남아 있다.

| 플랫폼 / 최신 기록 | 구현과 기록된 검증 | 남은 범위·한계 |
| --- | --- | --- |
| WindowsAppSDK · 9/19 | 같은 WebView2 instance에 공개 controller 연결. touch 주입·mouse·shield·resize·재생성, native 변경 frame 요청 수정, Gaussian→Saturation·독립 tint, Release build/publish·공통 계약 12개·WinUI 픽셀/30-frame·OS SendInput 회귀 | HWND와 CompositionVisual의 한 frame 혼합 미지원. pen/물리 touch·full Tab/IME/UIA·GestureArena·두 제품 owner·loss·성능·clean 배포·NativeAOT 미승인. MAUI 별도 |
| Android · 9/20 | 기존 native WebView/session 재사용, 음수 device ID 입력 수정, RID/TFM·AndroidX build 문제 수정, arm64/x64 Release Mono AOT APK 설치. Galaxy API·입력/수명 12항목·자동 Samsung 한글 IME·복귀 상태 보존. RenderEffect blur·채도·tint 검증 | native-origin parent GestureArena 미구현. full C/E·두 제품 owner·process/device loss·protected media·물리 입력/TalkBack·clean 배포 미승인. 최종 1/4-view animation window p95 81/121ms로 성능 승인 실패 |
| Linux Qt Quick · 9/20 | 공개 controller와 기존 Quick item/session 연결. host ABI 4/192 bytes + 별도 WebView ABI 1/32 bytes. Wayland/XWayland 제품·픽셀·synthetic 한글/Tab/focus·Vulkan layer·native 2-owner/10회 수명·renderer 종료 검증. 재배치 Release API 최종 32개 검사 통과 | VM 관측이며 물리 GPU 성능 승인이 아님. native-origin GestureArena·full framework Tab/물리 IME/Orca·두 전체 제품 owner·full C/E·loss·clean package·NativeAOT 잔여 |
| AppKit · 9/18 | WKWebView/Core Image/Metal 합성. 공개 `NSView.BackgroundFilters + CIGaussianBlur + CIColorControls`, ExactSigma 0–64·채도 0–2·독립 tint. Graphite/Ganesh와 새 폴더에 푼 Release `.pkg` 제품·픽셀 검증. 명시적 macOS 27 SDK 프로필 연결 | effect 1개·isotropic/rect clip. Core Image linear working color와 Skia encoded-color의 픽셀 동등성 미보장. full E3·두 owner·device loss·물리 입력·공통 비주얼/성능·clean 배포·NativeAOT 잔여. Catalyst 별도 |
| iOS UIKit Graphite · 9/21 | 공개 `UIVisualEffectView + UIBlurEffect + UIViewPropertyAnimator` 유지, `requestedSigma / 30` 보정. iOS 27 Simulator의 실제 WKWebView/Metal 강도·테마·복귀·7개 장면, iPhone 12/iOS 26.6.1 보정 및 샘플 패널·YouTube 로딩 통과. 두 Debug/Mono build 경고/오류 0 | MatchCommon sigma 0–16, 1개 isotropic 효과·saturation=1. UIKit 재질 tint 잔존, ExactSigma/일반 Gaussian·채도 변경 미지원. 현행 최종 구성 NativeAOT·전체 OS/기기·정확한 색상 동등성·full E3/두 owner·물리 IME/VoiceOver·성능 잔여 |
| Web · 9/21 | main DOM/managed Worker에 stable iframe·raster/effect/foreground/shield·ACK 연결. WebGPU/Graphite와 WebGL/Ganesh 제품 입력·focus·자동 한글·두 WebView·10회 수명·DPR+resize·same/cross-origin 픽셀 검증. Release 정적 서버 및 로컬 NuGet 생성 template publish/run 통과 | mixed raster는 CPU Skia RGBA upload, 64 MiB/frame 상한. animation raster p95 약 29–32ms·높은 복사량으로 성능 미승인. 기본 Debug Mono stack-bounds assertion 미해결. 두 전체 제품 owner·순수 DPR/monitor·다른 browser/GPU·물리 IME/접근성·clean 배포·NativeAOT 잔여 |

Windows MAUI, Mac Catalyst, iOS Ganesh, Qt Widgets는 위 성공을 전용하지 않는
별도 미연결/미지원 범위다. Web DOM/native Qt의 두 owner fixture도 두 전체 Doroti
제품 owner 승인과 구분한다. 여러 플랫폼의 NativeAOT publish는 실제 시도에서
iOS 전용 runner guard `DOROTIAOT002`로 거부됐으며 일반 Release/Mono AOT를
NativeAOT 완료로 계산하지 않았다.

효과 픽셀 기록은 각 환경의 제한된 fixture 결과다. 요청 sigma 4/16에 대해
Windows 3.989/15.947, Linux 3.999/15.866, Web 3.999/15.996을 기록했다.
Android Galaxy 최초 11-stage는 4.064/16.256이며 마지막 색상 순서 보정 후
x64 13-stage는 4.050/16.052였다. iOS 최신 목표 4/6/12/16의 측정은 Simulator
4/6/12/16, iPhone 12 4/6/12/15.75이며 고정 source의 테마·zero/reset·감소 차이는 0이다.
AppKit은 WebView sigma 2/4/8/16/32와 Metal 4/16이 일치했고 64는 약 66.5로
8% gate 안이었다. 이 수치만으로 플랫폼 간 전체 색상·비주얼 동등성을 승인하지 않는다.

### 보존할 실패와 변경 이력

- iOS 초기 공개 animator는 강도별 이미지가 같아지는 픽셀 FAIL이 있었다.
  당시 hierarchy/lifecycle PASS나 이후 private Gaussian의 실기기 NativeAOT PASS로
  이를 덮어쓰지 않는다. 최종 선택은 공개 animator이며 private KVC/filter와 블러
  전용 예외 변환 강제는 제거됐다.
- iOS 27 Scene 미적용 시작 실패 후 단일-window Scene delegate·manifest와
  활성화/복귀를 연결했다. 과거 Xcode 버전 검사 우회 실행과 이후 정식 도구 조합
  검증을 분리한다. iOS 26.6.1 실기기는 iOS 27 실기기 증거가 아니다.
- 전체 Mono interpreter의 `PendingFrame` GC 참조 오류에 대해 iOS 27 Debug는
  `MtouchInterpreter=all,-Doroti.Host.Maui`로 호스트만 Mono AOT를 적용했다.
  이는 확인된 경로의 우회이며 런타임 내부 수정·NativeAOT 검증이 아니다.
- Linux 9/14 Pointer ABI 불일치와 Release Qt6ShaderTools 누락은 수정했다.
  빠른 XWayland resize의 `VUID-VkSwapchainCreateInfoKHR-pNext-07781`은 당시 FAIL을
  보존한다. 9/20에 재현되지 않았다는 사실은 원인 수정의 증거가 아니다.
- Android 마지막 `color(blur(source))`/공통 휘도 계수 보정은 양쪽 ABI build와
  x64 픽셀 검증을 통과했다. USB 단절 뒤 사용자 `skip`은 **그 보정의 Galaxy
  재연결·마지막 색 경계 실기기 재검증만 `skippedByUser`**다. 앞선 Galaxy 결과는
  보정 전 설치본이며 다른 잔여 gate는 생략 승인되지 않았다.
- Web 기본 Debug 시작 실패와 CPU 전송 성능 한계를 유지한다. 생성 template의
  Release PDB 자산 오류는 SDK 설정 순서를 수정해 publish/run으로 확인했다.

## work2.md — WebView 기능과 제한

공개 `WebViewController`/`WebViewWidget`는 기존 PlatformView instance·coordinator·
attachment를 사용한다. 별도 ID registry/compositor/shield를 만들지 않는다.
초기 공통 계약은 기존 Ui/Hosting/Services/Widgets net10.0 assembly에 배치됐으며
설계의 `Doroti.WebView.*` 패키지 분할 제안을 모두 구현한 것으로 간주하지 않는다.

명령은 owner/instance/document/request 세대로 식별하고 bounded pending·timeout·
취소·late callback 거부를 적용한다. native ready, page load, 첫 content 표시를
구분한다. dispose는 admission 차단·pending/handler 정리와 공통 retirement를
거치며 profile 데이터 삭제의 범위·완료는 별도 계약이다.

| backend | 기록된 API 구현 | 명시적 제한 |
| --- | --- | --- |
| Windows | HTTP(S)/HTML/app content, history/reload/stop, 상태/event, JSON/null/undefined/오류·취소, view별 InPrivate·shared persistent, ClearData, `doroti-app://content` GET/HEAD/single Range, opt-in origin 검증 메시지 | Promise/cycle 거부. custom origin localStorage가 AllProfile만으로 남던 문제 수정. popup/download/외부 protocol·권한 기본 거부와 공개 앱 정책 완료는 별개. Windows MAUI 미연결 |
| Android | 탐색·상태·JS/오류·취소, 실제 callback까지 16 pending 유지, provider feature 기반 transient/shared profile·명시적 ClearData, appassets origin의 manifest 콘텐츠·Range, AndroidX origin/main-frame 메시지 검증 | transient는 disk-backed일 수 있고 dispose는 native data clear 완료를 기다림. profile shell은 다음 프로세스에 제거. 미지원 provider는 typed 오류. Galaxy provider151/x64 provider133 결과 분리; 초기 blank 탐색 제거로 구형 provider callback 문제 수정 |
| AppKit | 같은 WKWebView의 탐색·JS·event·ephemeral/shared persistent·website data clear·manifest scheme/상대 리소스·native origin/main-frame 메시지 | Promise, named persistent/profile 공유 객체, Range/media·서비스워커/보안 컨텍스트 완료 주장 없음. popup/외부 protocol 기본 거부; 전체 앱 정책·복구 잔여 |
| Linux Quick | 시스템 Qt/WebEngine 6.10.2, 탐색·JSON JS·32 pending·ephemeral/shared persistent·bounded scheme·trusted app QWebChannel 메시지·renderer terminal/recreate | 소스 API 하한 6.8은 해당 runtime/보안 승인 아님. `ClearAllData=false`, Range·임의 HTTP(S) 메시지·shared profile view별 resource map·정책 허용 UI 미지원. 시스템 엔진/helper/data/QML은 앱에 복사하지 않음 |
| Web | iframe URL/HTML, same-origin JS·32 pending/30초, caller cancel·document/close 처리, 명시적 BrowserDefault, same-origin HTTP(S) app content, exact origin/source-window/document/nonce/request 검증 메시지 | default Ephemeral·native SharedPersistent·전체 browser data clear·cross-origin 임의 JS/history·native resource scheme 미지원. unknown history/navigation 명시. AllowedOrigins는 controller 명령에만 적용하며 내부 redirect/link interception을 보장하지 않음 |
| iOS / Catalyst | 기존 WKWebView attachment와 효과 fixture를 WV-5 기능 API의 기반으로 인계 | controller/delegate/JS/profile 공개 제품 기능은 별도 잔여. AppKit 기능이나 iOS 효과 성공을 완료 근거로 전용하지 않음 |

메시지는 SDK/native에서 확인한 origin/frame/source와 document/request·크기 한도로
검사하며 payload의 자칭 origin을 신뢰하지 않는다. 지원하지 않는 API는 typed
Unsupported이며 성공 no-op으로 숨기지 않는다. Windows/Android/AppKit/Qt/Web의
기능·오류·취소·profile·origin·수명 검증은 해당 원문의 실행 환경에 한정한다.
popup/file chooser/download/permission/fullscreen의 전체 공개 정책과 process
복구·상태 보존, media/protected content는 계속 추적해야 한다.

## 남은 작업과 승인 기준

| 작업 묶음 | 후속 범위 |
| --- | --- |
| R0–R3 | 환경/source/evidence 기준, 계약·facade 마감, 전체 mutator/coverage/damage, host별 prepare/submit/commit/late ACK/resize/loss·retirement fault matrix |
| R4–R5 | native-origin GestureArena·nested scroll, full Tab/IME/semantics, 별도 runner 이관과 두 제품 owner |
| R6·R-E·WV-H | 플랫폼별 실제 WebView 전략 비교, live source·sharp child·공통 시각 허용편차, 효과 sample·전송 비용/메모리 예산 |
| WV-0–WV-7 | 이미 구현된 공개 API를 재사용하고 backend별 미지원·부분 구현과 UIKit/별도 runner 기능 연결 마감 |
| WV-8 | 정책 허용/거부/취소, profile 전체 삭제 범위, process terminal/재생성·복구되지 않는 상태 명시 |
| R7·WV-9 | 동일 제품의 공동 승인: 0/1/4-view × idle/animation/scroll/modal, 물리 입력·접근성·성능·template/clean 배포·NativeAOT |
| WV-X | 별도 요구가 있을 때 headless·고급 pooling·인증/proxy/interception/devtools. 기본 미완료 기능을 선택 확장으로 옮기지 않음 |

C1–C6는 불투명/반투명 가림, native-over-raster, R/N/R/N/R 순서, 이동·clip·DPI·
resize 정합성과 shield/native-origin 입력을 포함한다. E1은 live WebView 위 부분
효과와 선명한 child, E2는 동적 source·이동/resize·입력, E3는 여러 native와 raster의
앞선 배경 전체 sample이다. 일부 fixture 성공을 전체 C/E 승인으로 확대하지 않는다.

`sourceReviewed`, `build`, `automated`, `productLive`, `physical`, `nativeAot`를
독립적으로 기록한다. 재실행 시 저장소 지침의 20분 외부 timeout을 적용하고,
Linux 반복은 후속 요청의 10회 기준을 따른다. 이전 100회 이력은 보존하되
변경·실패 근거 없이 반복하지 않는다. 미실행은 `notVerified`, 구현 잔여는
`TODO`/`PARTIAL`, 실제 지시된 생략만 `skippedByUser`로 유지한다.

## 계약·재현·결과 문서

- [공통 계약](../../Doroti/docs/platform-views/contract.md), [지원표](../../Doroti/docs/platform-views/support-matrix.md), [공통 검증 안내](../../Doroti/validation/platform-views/README.md)
- [Windows WebView 계약](../../Doroti/docs/platform-views/windows-webview.md), [WebView 검증](../../Doroti/validation/webview/README.md)
- [Android 계약](../../Doroti/docs/platform-views/android-webview.md), [Android 결과 9/20](../../Doroti/validation/webview/android-results-2026-09-20.md)
- [Linux Qt 계약](../../Doroti/docs/platform-views/linux-qt.md), [Linux WebView 계약](../../Doroti/docs/platform-views/linux-webview.md), [Linux 결과 9/20](../../Doroti/validation/webview/linux-results-2026-09-20.md)
- [macOS 계약](../../Doroti/docs/platform-views/macos.md), [macOS 재현](../../Doroti/validation/platform-views/macos/README.md)
- [iOS 계약](../../Doroti/docs/platform-views/ios.md), [iOS 재현](../../Doroti/validation/platform-views/ios/README.md), [공개 블러 보정 결과 9/21](../../Doroti/validation/platform-views/ios/public-blur-tuning-2026-09-21.md)
- [Web 계약](../../Doroti/docs/platform-views/web-webview.md), [Web 결과 9/21](../../Doroti/validation/webview/web-results-2026-09-21.md)

상세 로그·캡처·hash 경로는 원문에 보존했다. 대표 경로는
`Doroti/artifacts/platform-views/2026-09-{14,17,18,19}/`,
`Doroti/artifacts/webview/2026-09-19/windows/`,
`Doroti/artifacts/webview/2026-09-20/android/`다. Git 제외·외부 기기 산출물은
다른 checkout에 존재한다는 보장이 없으며 이번 보관에서 재생성하거나 검증하지 않았다.
