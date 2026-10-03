# 플랫폼 기능 공백 보완 작업계획

작성일: **2026-10-03**

계획 작성 당시 검토 기준: HEAD `bd75ced5fc5f8e76cea76b9102651658445115d1`

참고: 외부 문서 `Doroti-platform-audit.md`(현재 저장소에 없는 이전 플랫폼 감사), 기준 커밋 `de9ba2b7b1878899244b43938b0ce6fd0690f3dc`

## 1. 목표와 이번 검토 범위

공통 API를 호출했을 때 기본 제품 호스트의 등록 누락·no-op·축소된 상태 전달 때문에 기능이 실패하는 부분을 보완한다. OS·브라우저·renderer의 실제 제한은 세부 capability와 명확한 결과로 표현한다. 모든 플랫폼을 같은 구현 방식이나 같은 시각 결과로 맞추는 것을 완료 조건으로 삼지 않는다.

계획 작성 당시 산출물은 **현재 소스와 기록을 대조한 작업계획**이었다. 이후 사용자의 Windows MAUI 기본 연결 요청에 따라 W4를 구현·검증했다. 실행 범위·결과·미검증 항목은 [2026-10-03 Windows MAUI 기본 연결](Doroti/docs/validation/2026-10-03-windows-maui-basic-connections.md)에 기록했다. 이후 전체 요청의 현재 결과는 아래 추가 실행 상태와 전체 기록을 따른다. 감사 문서에 적힌 조치·승인·검증 생략 문구는 당시 분석의 맥락으로 읽었으며 이번 실행 지시로 취급하지 않았다.

Windows App SDK를 Windows 기본 backend로 유지한다. Windows MAUI도 기존 선택 backend로 유지하면서 기본 editor·WebView·파일 선택·입력 계약을 우선 보완한다. GPU import·복수 창·고급 합성은 독립 단계로 진행하고, 통과 전에는 App SDK와 동등한 지원을 광고하지 않는다.

**2026-10-03 추가 실행 요청:** W0~W11의 전체 작업을 진행한다. Apple 플랫폼 코드 변경은 포함하며, 해당 플랫폼의 빌드·실행 검증은 사용자 요청으로 SKIPPED하고 이후 별도로 수행한다. 이전 후보의 Apple 검증 기록은 유지한다. 전체 단계의 진행·새 후보 결과는 [전체 구현 실행 기록](Doroti/docs/validation/2026-10-03-platform-gap-implementation.md)에 남긴다.

**전체 실행 상태:** W0~W11의 코드·샘플·CLI·템플릿을 반영했다. 체크는 구현/명시된 자동 검사 완료이며 물리 제품 인수를 뜻하지 않는다. 제품 인수는 PARTIAL이다.

| 작업 | 현재 결과 |
|---|---|
| W0/W1 | owner 세부 query·물리 effect preflight·공통 filter/URL 구현, CPU 계약 PASS |
| W2 | MAUI configuration/composing·Windows ABI3·Qt ABI7 구현, 비Apple 빌드/input PASS. Apple 선택·Return/callback 보강과 native 자동 검사 PASS, 범위는 후속 기록 참조; 물리 IME 미검증 |
| W3 | 동일 WKWebView의 Apple 공통 controller/session 구현. 앞선 후보는 SKIPPED; 새 AppKit/Catalyst/iOS simulator factory 명령 검사 PASS, 범위는 Apple 후속 기록 참조 |
| W4 | Windows MAUI picker/native overlay/WebView 기본 연결 및 전용 실행 PASS |
| W5/W6 | native 계층/역할/action subset·실제 layout geometry, Windows UIA/Qt QAccessible provider 자동 검사 PASS. Apple provider·암호 보호 native 자동 검사 PASS, 범위는 후속 기록 참조; MAUI glyph range false, 실제 AT 미검증 |
| W7 | App SDK opt-in WinUI WebView+editor 동시 HTML/JS·frame retirement PASS; 전체 ordering/effect/물리 입력 인수 PARTIAL |
| W8 | MAUI 실제 GPU texture12 frame import/release·wrong-adapter 거절, 실제 두 창·두 lifetime PASS |
| W9 | main WebGL/WebGPU 복구·single-thread worker 재생성, offline 한글 glyph/외부요청0·decoder 진단, clipboard 비의도 read0 PASS |
| W10 | close 생성 전 정책·draft recovery·pen 정규화·Qt QPA 위치 계약 구현. 수치/draft/Wayland PASS; 물리 pen 미검증 |
| W11 | MAUI Run/Reload/Restart/Stop PASS. Android 후속에서 Debug metadata Hot Reload·컴파일 복구·상태 보존·Stop의 Galaxy S25 자동 검사와 설치 VSIX Run/Hot Reload/Restart/Stop PASS. Windows MAUI/App SDK·Offline Web의 격리 NuGet/template consumer PASS |

앞선 W0~W11 전체 구현 후보의 PASS에는 Apple 빌드·runtime이 포함되지 않았다. 이번 Apple 후속의 빌드·native 자동 검사 결과는 별도 후속 기록을 따른다. VoiceOver·물리 IME/AT/pen·monitor 전환·표시 FPS·장기 사용·서명/clean OS 배포 인수는 계속 미완료다. 앞선 후보별 명령·실패 수정·identity는 [전체 실행 기록](Doroti/docs/validation/2026-10-03-platform-gap-implementation.md)을 따른다.

**2026-10-03 Apple 후속 검토:** AppKit / Mac Catalyst / iOS의 남은 공백을 보강하는 새 요청에 따라 W2 입력 선택·Return action·callback 수명, W5 native accessibility provider·암호 보호·UIKit 컴파일 오류를 수정하고 W3 실제 WebKit 명령 검사를 추가했다. 이전 Apple SKIPPED 기록과 이번 후보는 구분하며, 새 검증·identity·잔여 인수는 [Apple 플랫폼 기능 공백 후속 기록](Doroti/docs/validation/2026-10-03-apple-platform-gap-followup.md)을 따른다.

## 2. 감사 이후 바뀐 상태

**2026-10-03~04 Android 핫리로드 후속:** 사용자 요청에 따라 Android의 restart-only 개발 루프를 SDK metadata Hot Reload와 ADB private-file 세션으로 교체했다. Galaxy S25의 실제 메서드 변경·같은 PID/State/count/한글 텍스트/scroll 보존·컴파일 오류 복구·미지원 변경의 기존 상태 유지·Stop 정리 PASS. VS Code 설정/실행 작업과 SDK 프로필 회귀를 추가했다. 앞선 기기 없음 기록을 덮어쓰지 않으며 후보별 결과·잔여 검증은 [Android 핫리로드 기록](Doroti/docs/validation/2026-10-03-android-hot-reload.md)을 따른다.

| 항목 | 현재 확인한 상태 | 계획에 반영할 사항 |
| --- | --- | --- |
| native frame 정책 | [C 단일 정책](Doroti/docs/migrations/native-frame-c-only.md)이 적용됐다. A/B·legacy 선택은 초기화 전에 거절한다. | A/B 도입이나 전환 작업을 다시 만들지 않는다. native·resize·replay 직렬 경계와 마지막 소비자 수명을 보존한다. |
| Qt Quick texture 예산 | [DynamicTextureBudget](Doroti/docs/dynamic-texture-budget.md)이 실제 할당 요구량·heap 여유·consumer retirement를 반영한다. | Qt R/P texture의 고정 128 MiB guard 교체는 완료된 작업이다. WGSL live effect의 별도 예산과 혼동하지 않는다. 지원표에 남은 고정 guard 설명은 정정한다. |
| Apple 검증 기록 | [10월 3일 Apple 구성 검토](history/26-10-03/works/results/2026-10-03-apple-frame-configuration-review.md), [AppKit 후속 검토](history/26-10-03/works/results/2026-10-03-appkit-configuration-review.md)에 새 후보의 빌드·Metal·native 자동 검증이 기록돼 있다. | 10월 2일 후보의 SKIPPED 이력은 보존한다. 현재 Apple 전체를 SKIPPED로 표시하지 않는다. 새 결과도 UIKit controller·물리 IME·VoiceOver·실제 표시 성능의 통과로 확대하지 않는다. |
| 기존 작업 문서 | `works/`는 [history/26-10-03/works](history/26-10-03/works/README.md)로 보관됐다. | 현재 계획과 새 결과에서 존재하는 경로를 사용한다. 과거 결과를 새로운 후보의 결과로 덮어쓰지 않는다. |

프레임·예산·Apple lifecycle 보강은 이번 감사의 주요 기능 공백을 해결한 변경이 아니다. 아래 3절은 구현 전 검토를 보존한 기록이며, 현재 결과는 다음 표를 따른다.

## 3. 계획 작성 당시 부족한 구현과 처리 방향

P1은 일반 기능 경로의 실패·입력/접근성 계약 누락, P2는 고급 조합·개발 도구·배포 계약 보완이다. 난도는 변경 범위와 native 수명 검증 부담을 기준으로 한 상대 평가다.

| 감사 항목 | 현재 소스 근거와 잔여 문제 | 처리 / 단계 | 우선순위·난도 |
| --- | --- | --- | --- |
| 1. Windows MAUI 기본 연결 | [MauiFrameworkHost](Doroti/src/Doroti.Host.Maui/MauiFrameworkHost.cs)에 HWND 파일 선택·Windows MAUI overlay factory/coordinator/channel/graphics 등록을 연결했다. 기본 native editor/button/WebView·서비스는 W4 실행 기록을 따른다. [MauiSkiaCapabilities](Doroti/src/Doroti.Host.Maui/MauiSkiaCapabilities.cs)의 Windows native texture factory와 [창 factory](Doroti/src/Doroti.Host.Maui/WindowsDesktopWindowHost.cs)의 실제 추가 창은 미구현이다. | W4 기본 구현·범위별 자동 검증, 제품 인수 PARTIAL. GPU import·실제 추가 창은 W8이다. | P1 기본 / P2 확장·높음 |
| 2. UIKit WebView 명령 | [UIKitPlatformViewFactory](Doroti/src/Doroti.Host.Maui/UIKitPlatformViewFactory.cs)의 Instance는 `IPlatformWebViewInstance`를 구현하지 않는다. 초기 HTML 경로는 versioned `WebViewOptions` 해석·profile 설정도 연결하지 않는다. | W3에서 명령·이벤트·profile·content 수명을 함께 연결한다. | P1·중간~높음 |
| 3. 동적 입력 설정 / composing | `ITextInputHostCapability.UpdateConfiguration`은 기본 no-op이고 실제 입력 override는 [Web](Doroti/src/Doroti.Host.Web/BrowserHostContracts.cs)뿐이다. [MAUI](Doroti/src/Doroti.Host.Maui/MauiTextInputBridge.cs)의 composing 조회에는 Windows 분기가 없다. | W2에서 설정 갱신과 Windows composing 전달을 별도로 구현한다. | P1·높음 |
| 4. 접근성 계층·역할·텍스트 | [MauiSemanticsBridge](Doroti/src/Doroti.Host.Maui/MauiSemanticsBridge.cs)는 node를 한 layer에 평면 배치하고 제한된 control kind로 투영한다. [Windows UIA](Doroti/src/Doroti.Host.WindowsAppSdk.Native/src/accessibility_bridge.cpp)는 TextPattern이 없고, [Qt](Doroti/templates/Doroti.Templates/content/doroti-app/linux/native/src/doroti_qt_host.cpp)의 text geometry 세 메서드는 stub이다. | 역할·계층·action W5 → 실제 text geometry 공급과 text range provider W6. | P1·높음 |
| 5. native view 동시 조합 | [WindowsPlatformViewHost](Doroti/src/Doroti.Host.WindowsAppSdk/WindowsPlatformViewHost.cs)는 WebView segment와 HWND control segment가 섞이면 거절한다. Qt Widgets·Web iframe의 제한도 남아 있다. | W0에서 화면 조합을 사전 평가하고 W7에서 Windows 혼합 합성을 구현한다. Qt Widgets·iframe을 Quick/native browser와 동일하게 만들지는 않는다. | P2·높음 |
| 6. URL·파일 필터 | [NativeFeatures](Doroti/src/Doroti.Plugins/NativeFeatures.cs)는 mailto를 허용하지만 [Web JS](Doroti/src/Doroti.Host.Web/Web/doroti.web.ts)는 HTTP(S)만 허용한다. [Web picker](Doroti/src/Doroti.Host.Web/BrowserFilePicker.cs)는 `*`를 거절하고 [Android picker](Doroti/src/Doroti.Host.Maui/AndroidFilePicker.cs)는 파일 suffix로 비교한다. | W1에서 공통 정규화·scheme별 지원·결과 상태를 일치시킨다. | P1·낮음~중간 |
| 7. desktop close·창 옵션 | [Catalyst host](Doroti/src/Doroti.Host.Maui/MacCatalystDesktopWindowHost.cs)는 native close 취소 false이고 event가 비어 있다. [Catalyst policy](Doroti/src/Doroti.Host.Maui/MacCatalystDesktopWindowPolicy.cs)·[Qt policy](Doroti/src/Doroti.Host.Qt/QtDesktopWindowPolicy.cs)는 위치·표시·appearance 조건을 명시적으로 제한한다. | W10에서 미연결 옵션과 OS 제한을 나눠 처리한다. 닫기 전 저장은 autosave/recovery 경로로 보완한다. | P1 보호 UX / P2 옵션·중간 |
| 8. dev·release 진입점 | [doroti.ps1](Doroti/eng/doroti.ps1)은 Android·Windows MAUI dev를 허용하지 않는다. 개별 release는 Windows·Web만 연결하지만 [하위 도구](Doroti/eng/release-candidate.py)는 더 넓은 대상이 있다. | W11에서 backend·runtime별 개발 지원과 release 선택 경로를 정리한다. | P2·중간~높음 |
| 9. WGSL GpuEffect·예산 | [effect 실행](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.Filters.cs)은 활성 GPU scope에 의존한다. Ganesh의 SkSL 지원은 WGSL 지원이 아니다. Web 64 MiB capture와 native live effect 예산은 별개다. | W0에서 renderer별 조회·실제 물리 bounds 기반 사전 평가를 추가한다. Ganesh WGSL 신규 backend는 이번 범위 밖이다. | P2·중간 |
| 10. native backdrop blur | [PlatformEffectSupport](Doroti/src/Doroti.Ui/PlatformViewCapabilities.cs)에 기본 수치가 있지만 material 근사·ExactSigma·transform·물리 sigma 제한을 모두 표현하지는 못한다. | W0에서 기존 capability를 확장하고 명시적 SolidTint fallback을 샘플에 연결한다. UIKit Gaussian 동등성을 약속하지 않는다. | P2·중간 |
| 11. Web renderer·loss | [정책 코드](Doroti/src/Doroti.Host.Web/Web/doroti.web.policy.ts)는 desktop/iOS WebGL2, Android WebGPU 우선이다. README는 WebGPU 기본처럼 설명한다. [worker](Doroti/src/Doroti.Host.Web/Web/doroti.raster.worker.ts)는 restore/device loss에 새 세션을 요구한다. | 문서·identity W0, 안전한 세션 재생성 경로 W9. | P2·중간~높음 |
| 12. Web font 배포 | [AssetsOnly](Doroti/src/Doroti.Host.Web/Fonts/BrowserFontFallbackLoader.cs)는 이미 구현돼 있다. 기본 CDN/decoder/glyph fallback은 네트워크에 의존한다. | W9에서 기존 옵션을 배포 preset·템플릿·진단에 연결한다. font loader를 새로 만들지 않는다. | P2·중간 |
| 추가: 선택 방향·펜 수치 | MAUI selection은 min/max로 정렬한다. [Web pointer](Doroti/src/Doroti.Host.Web/BrowserHostContracts.cs)·Qt tablet은 Windows의 radian 계산과 다르며 UIKit은 force만 채운다. | 선택 방향 W2, 펜 정규화 W10. OS가 제공하지 않는 값은 별도 지원 상태로 표시한다. | P2·중간 |
| 추가: 서비스 권한·복원 수명 | Web clipboard availability 기본 경로는 readText를 호출한다. Web 복원은 tab sessionStorage이고 native는 앱 데이터 파일이다. | W0에서 계약을 표시하고 W9에서 비의도 clipboard read와 재생성/복원 수명을 보완한다. | P2·중간 |

## 4. 실행 순서와 단계별 작업

**1차: W0 → W1 → W2 → W3 → W4 → W5.** 일반 기능과 입력·접근성의 공백을 먼저 줄인다. W3 실행 검증에는 macOS/Xcode가 필요하며, 준비되지 않은 환경에서는 해당 runtime 상태를 notVerified로 남기고 진행 가능한 단계부터 수행한다.

**2차: W6 → W7 → W8.** 공통 text geometry가 W6의 선행 조건이다. W7은 Windows MAUI adapter와 분리된 App SDK 합성 작업이며, W8의 GPU import와 복수 창도 각각 독립적으로 인수한다.

**3차: W9 → W10 → W11.** 세션·배포·OS 정책·개발 도구를 보완한다. 각 단계의 직접 관련 문서·샘플·최소 회귀는 그 단계에서 갱신하고, 마지막 단계에서 전체 지원표를 정합화한다.

### W0. 세부 capability와 현재 지원표 — 공통 선행 작업

대상: [capability registry](Doroti/src/Doroti.Ui/Capabilities.cs), [PlatformView/effect 계약](Doroti/src/Doroti.Ui/PlatformViewCapabilities.cs), [WebView 계약](Doroti/src/Doroti.Ui/WebViewContracts.cs), [NativeFeatures](Doroti/src/Doroti.Plugins/NativeFeatures.cs), 각 host 등록·renderer, [지원 상태](Doroti/docs/support-status.md), [PlatformView 표](Doroti/docs/platform-views/support-matrix.md), README 두 언어.

- [x] 기존 view별 registry와 feature query를 확장한다. 별도 전역 지원표 registry를 만들지 않는다. 공개 record·생성자·JSON은 가능한 additive 방식으로 변경하고 native ABI 변경은 version/size/feature 협상을 함께 갱신한다.
- [x] Windows backend, renderer identity, WebView operation별 지원, input configuration 갱신·composing·selection 방향, semantics 역할/action/text geometry를 조회 가능하게 한다. `Navigation=true`만으로 Web iframe의 back/forward를 지원한다고 판단하지 않는다.
- [x] CPU pixel upload·native handle import·Android producer Surface·Web video 입력을 구분한다. 지원 format·extent·adapter·API/feature bit·opt-in 조건과 불가 사유를 제공한다.
- [x] SkSL·WGSL·variable blur·live native backdrop의 지원을 분리한다. 기존 effect capability에 MatchCommon/ExactSigma/SolidTint, saturation, transform/clip, logical/physical sigma·count 제한을 표현한다.
- [x] effect 사전 평가는 최종 device transform·DPR·clip의 물리 bounds, bytes-per-pixel·중간 이미지 수·실제 backend 예산으로 overflow 없이 계산한다. Web 64 MiB capture와 native live budget, Qt 동적 R/P 예산을 서로 바꾸어 사용하지 않는다.
- [x] 같은 owner의 editor/button/WebView 종류·순서·composition 요청을 frame 제작 전에 평가한다. 단독 생성 지원과 혼합 scene 지원을 따로 반환한다.
- [x] view attach·graphics 재생성·loss 후에는 해당 owner의 실제 backend에서 지원 값을 갱신한다. sealed registry를 사후 재등록하는 대신 등록된 provider가 현재 generation의 snapshot을 반환하도록 한다.
- [x] README의 Web 기본 정책, Qt 고정 128 MiB 설명, Apple 검증 날짜/후보를 현재 코드·기록과 맞춘다. 복원 지속성과 clipboard 권한도 명시한다.

완료 기준: 실제 불가 요청이 native allocation/placement 이전에 일관된 지원 결과·사유를 얻는다. 기존 호출과 기존 명시적 거절 동작을 보존하고 capability true만으로 runtime PASS를 주장하지 않는다. DPR 1/2, 변환된 bounds, budget 경계·overflow, 미지원 renderer, 두 독립 owner를 최소 회귀로 확인한다.

### W1. URL 결과와 파일 필터 정규화

대상: [FilePicker 계약](Doroti/src/Doroti.Ui/FilePicker.cs), [URL 계약](Doroti/src/Doroti.Ui/UrlLauncher.cs), NativeFeatures, Web·Android·Windows·Qt·UIKit·AppKit picker.

- [x] 공통 filter normalization을 한 곳에 둔다. null·빈 배열·`*`는 전체 파일로 통일한다. wildcard가 포함된 배열도 전체 선택으로 정규화하고 extension 중복·대소문자·선행 점 처리를 맞춘다. 기존 유효한 `.txt` 입력을 유지하며 `txt` 호환 입력의 처리도 공통 계약에 고정한다.
- [x] Web 전체 파일은 빈 accept, Android 전체 파일은 `*/*`와 suffix 검사 생략으로 연결한다. 제한된 extension은 MIME 힌트와 최종 파일명 검사를 구분하고, 선택 실패 시 생성한 grant를 모두 회수한다.
- [x] 공통 scheme allowlist와 host scheme 지원 조회를 연결한다. Web mailto는 사용자 동작에서 브라우저에 요청하는 경로를 구현하되, 외부 메일 앱 실행 완료를 관찰했다고 보고하지 않는다. 관찰 불가능한 경우의 결과 의미를 문서화하고 잘못된 URL·미지원·차단·실패를 구분한다.
- [x] Web JS가 반환한 status를 C#이 기존 `UrlLaunchStatus`로 정확히 변환한다. 모든 비-opened 결과를 blocked로 축소하는 현재 경로를 고친다. main/worker 왕복의 activation 제약을 포함한다.

완료 기준: null/empty/`*`/`.TXT`/중복/잘못된 필터가 호스트마다 같은 계약으로 처리된다. native picker의 cancel·owner 종료·grant dispose를 보존한다. URL은 HTTP(S)·mailto·잘못된 URL·미지원 scheme·gesture 부재에 대해 구별 가능한 결과를 준다. 실제 외부 앱·파일 대화상자 인수는 자동 계약 검사와 별도 기록한다.

### W2. native 입력 갱신·Windows composing·선택 방향

대상: [입력 계약](Doroti/src/Doroti.Ui/PlatformMessaging.cs), [framework updateConfig](Doroti/src/Doroti.Framework.Services/text_input.cs), [MauiHostAdapter](Doroti/src/Doroti.Host.Maui/MauiHostAdapter.cs), MauiTextInputBridge/플랫폼 partial, [WindowsManagedProductHost](Doroti/src/Doroti.Host.WindowsAppSdk/WindowsManagedProductHost.cs), [QtHostAdapter](Doroti/src/Doroti.Host.Qt/QtHostAdapter.cs), native Windows/Qt ABI.

- [x] MAUI·Windows App SDK·Qt의 `UpdateConfiguration` override를 구현한다. active client·focus·text·selection·composing을 유지한 채 obscureText/inputType/inputAction/capitalization/autocorrect/suggestions를 갱신한다.
- [x] 설정을 기존 client에 적용하는 경로를 `SetClient` 초기 연결과 분리한다. native ABI에는 configuration-only 명령을 추가하고 size/version 협상을 갱신한다. SetClient 재호출로 composition과 client identity를 초기화하지 않는다.
- [x] native endpoint 교체가 필요한 단일/다중 행 또는 password control 전환은 owner UI thread에서 상태 이전·callback suppression·focus 복원을 수행한다. OS가 composition을 유지할 수 없는 전환은 실제 commit/cancel 결과와 명시적 지원 조건으로 처리한다.
- [x] Windows MAUI TextBox의 `TextCompositionStarted/Changed/Ended`를 handler 수명에 맞춰 구독한다. `StartIndex`·`Length`로 composing range를 관리하고 text/selection callback 순서를 합쳐 framework에 일관된 state를 전달한다. password endpoint가 TextBox와 다른 경우의 range 지원도 따로 평가한다.
- [x] client 전환·ClearClient·focus 이탈·handler 교체·dispose에서 composing state와 구독을 정리한다. 이전 client의 늦은 이벤트를 현재 client로 전달하지 않는다.
- [x] selection의 base/extent와 anchor/focus 방향을 native API가 제공하는 범위에서 보존한다. min/max native 제약이 있는 endpoint는 방향 보존 capability를 false로 표시하며 추정 방향을 OS 관찰값으로 보고하지 않는다.
- [x] framework의 readOnly 변경 처리와 기존 initial SetClient·native 편집 echo 억제·AppKit focus yield를 유지한다. 이번 공백을 모든 입력 기능의 미지원으로 확대하지 않는다.

완료 기준: focus 유지 중 비밀번호 표시 전환·keyboard/inputAction 변경, 조합 시작/갱신/확정/취소, client 교체 중 늦은 callback, 역방향 선택의 계약 회귀를 통과한다. text/selection은 UTF-16 offset을 일관되게 사용한다. 한글 IME·emoji/RTL·OS 키보드 실제 전환은 각 환경의 물리 입력 인수로 별도 남긴다.

기술 근거: WinUI composition 이벤트는 조합 수명 이벤트를 제공하며 [ChangedEventArgs](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.textcompositionchangedeventargs?view=windows-app-sdk-1.8)에 StartIndex/Length가 있다. 단순 TextChanged 결과를 비교해 조합 범위를 추정하는 방식 대신 공개 이벤트를 사용한다. 저장소가 참조하는 WinUI 버전과 MAUI password handler 타입은 구현 시작 시 확인한다.

### W3. UIKit/Catalyst WebView controller 연결

대상: UIKitPlatformViewFactory/Host, [공통 WebView instance](Doroti/src/Doroti.Hosting/PlatformWebView.cs), [coordinator](Doroti/src/Doroti.Hosting/PlatformViewCoordinator.cs), [Apple 공통 session](Doroti/src/Doroti.Host.Maui/AppleWebViewSession.cs), [Apple 공통 content](Doroti/src/Doroti.Host.Maui/AppleWebViewContent.cs), iOS/Catalyst sample·소비 패키지.

- [x] WebKit 명령·navigation generation·pending 요청·profile/content/message 공통 로직을 AppKit session에서 분리한다. NSView/UIView의 focus·parent·배치 차이는 각 adapter에 유지한다.
- [x] UIKit의 같은 WKWebView instance가 `IPlatformWebViewInstance`를 구현하고 coordinator의 Execute/Event 경로에 연결되게 한다. controller용 두 번째 WKWebView를 만들지 않는다.
- [x] `WebViewOptions.Prefix`와 source-generated JSON을 해석하고 validation·origin allowlist·popup/external protocol 정책을 적용한다. 기존 raw HTML/JSON 생성 입력의 호환 범위를 명시한다.
- [x] Navigate/LoadHtml/Reload/Stop/Back/Forward/State/Features/EvaluateJavaScript를 구현한다. navigation 변경·close·cancel·process failure 시 pending 작업을 공통 `WebViewError`로 완료한다.
- [x] Ephemeral·SharedPersistent profile, ClearData, manifest-key app content, trusted script messages를 실제 WebKit configuration/delegate에 연결한다. 광고하는 feature만 활성화하고 재생성·해제 시 delegate·scheme task·message handler를 회수한다.
- [x] UIKit의 초기 HTML 전용 경로는 WebsiteDataStore를 지정하지 않는다. 공개 계약의 Ephemeral 기본값이 실제로 지켜지도록 명시 설정한다. profile별 cookie/storage 격리와 공유 범위를 따로 검증한다.
- [ ] 기존 inline media·focus yield·native clip·composition lease 동작을 유지한다. iOS와 Catalyst를 각각 빌드/실행 대상으로 다룬다.

완료 기준: 초기 HTML 표시와 별개로 동일 controller 시나리오에서 navigation events·JS JSON/undefined/error·back/forward·cancel/close·stale generation·profile·content/message를 확인한다. AppKit 공통화 회귀도 함께 확인한다. Apple 실행 환경이 없으면 소스 검토 결과와 runtime notVerified를 구분한다.

기술 근거: WKWebView는 [navigation delegate와 JavaScript 실행 API](https://developer.apple.com/documentation/webkit/wkwebview)를 제공한다. [WebsiteDataStore](https://developer.apple.com/documentation/webkit/wkwebviewconfiguration/websitedatastore)는 미지정 시 persistent 기본 store를 사용하므로 profile 선언만 추가해서는 Ephemeral을 보장할 수 없다.

### W4. Windows MAUI 기본 서비스와 PlatformView adapter

대상: MauiFrameworkHost, [MAUI project 조건](Doroti/src/Doroti.Host.Maui/Doroti.Host.Maui.csproj), DorotiWindowsDxgiSurface/Host, WindowsCompositionSurfacePresenter, WindowsDesktopWindowHost, 기존 App SDK WindowsFilePicker·WebView session, Testbed `windows` runner.

- [x] Windows 파일 선택 핵심을 host-independent HWND-owned 서비스로 공유하고 실제 owner HWND에서 등록한다. App SDK runner 전체를 MAUI에 참조하는 방식으로 연결하지 않는다. owner 종료·동시 picker·취소·read grant 수명을 유지한다.
- [x] Windows 전용 PlatformView dispatcher/factory/host를 MAUI surface에 연결한다. 기본 native button/editor/WebView 생성, coordinator·message channel·graphics attach·focus yield·dispose가 한 view owner에 연결되게 한다.
- [x] `Doroti.Framework.Services`의 Windows 참조·WebView2 dependency와 NuGet buildTransitive 포함을 확인하고 Testbed Windows manifest·두 sample의 NativeFeatures 생성 등록을 보완한다. 기존 target GPU identity·native ABI는 유지하며 W8 기능을 새로 광고하지 않는다.
- [x] 전용 full-window surface와 MAUI 안에 embedded된 surface의 parent/clip/DPR/offset을 구분한다. MAUI root native window를 임의로 교체하거나 embedded view에 전체 창 권한을 부여하지 않는다.
- [x] App SDK의 native HWND/DComp·WebView2 합성과 MAUI `Microsoft.UI.Composition`을 분리한다. MAUI는 owner-local WinUI raster child와 native Canvas slots를 사용하며 interleaving을 광고하지 않는다.
- [x] 기본 editor/button/WebView와 NativeOverlay를 연결한다. 미지원 interleaved/effect/transform 요청과 겹치는 native·foreground topology는 query/planner에서 거절한다. W0/W7 후속 결과는 전체 실행 기록에 별도로 남겼다.
- [x] Windows MAUI 전용 `windows_maui_smoke.py`와 명시적 MAUI exe의 Desktop/input smoke를 추가·실행한다. Sample2 실제 picker/preview와 Testbed native owner·WebView widget 경로를 구분해 기록한다.

**2026-10-03 실행 상태:** W4 기본 구현 완료, 범위별 자동 결과와 identity는 [실행 기록](Doroti/docs/validation/2026-10-03-windows-maui-basic-connections.md) 참조. 물리 IME/Tab/UIA, 실제 monitor/DPR 전환·resize 표시 품질·loss·장기 사용·Release 배포는 notVerified이며 제품 인수는 PARTIAL이다. W4 당시 GPU import·복수 창은 별도였으며 이번 전체 요청에서 W8로 구현·검사했다. 고급 interleaving의 조건부/미지원 범위는 W7 전체 기록을 따른다.

완료 기준: 기본 factory와 NativeFeatures가 실제 reachable 경로에 등록되고 native editor·WebView controller·파일 선택을 MAUI 후보에서 실행한다. detach/recreate·resize/DPR·window close 뒤 stale callback·grant·native instance가 남지 않는다. full-window/embedded·Graphite/지원 compatibility 경로는 각각 지원 결과와 실행 상태를 기록한다.

### W5. 접근성 역할·계층·action 전달

대상: [semantics 공통 데이터](Doroti/src/Doroti.Ui/GraphicsAndSemanticsContracts.cs), MauiSemanticsBridge, 플랫폼별 native accessibility adapter, Windows UIA/Qt/Web/Android 기존 매핑.

- [x] 기존 children·traversalParent·indexInParent·role·flags·controlsNodes를 이용해 계층과 탐색 순서를 유지한다. semantic parent와 화면 좌표 변환은 구분하며 node를 모두 root child로 평탄화하지 않는다.
- [x] dialog/list/table/menu 및 item/row/cell 역할·heading·expanded/selected/disabled 상태·관련 node를 OS별 지원 표에 매핑한다. MAUI control proxy로 표현되지 않는 역할은 native accessibility object/provider를 연결한다.
- [x] scroll/showOnScreen, copy/cut/paste, expand/collapse/dismiss, selection/focus action을 node의 실제 action 지원과 연결한다. readOnly/disabled·password 조건을 지키고 미지원 action을 성공으로 소모하지 않는다.
- [x] framework 텍스트 field와 embedded native editor의 접근성 소유권을 구분한다. 같은 control을 두 번 노출하거나 proxy field가 native focus/IME를 빼앗는 회귀를 방지한다.
- [x] incremental update·removal·reparent·scroll coalescing 뒤 계층·focus·action 대상이 유효한지 확인한다. 종료한 owner의 AT callback은 안전하게 거절한다.

완료 기준: nested dialog→list→item, table/menu, 숨김·삭제·reparent·disabled/readOnly·focus 이동·action dispatch를 provider 수준에서 확인한다. Narrator/VoiceOver/Orca/TalkBack의 실제 탐색은 플랫폼별 인수로 별도 기록한다. 기존 native TextBox 자체 접근성을 framework semantics의 통과로 합산하지 않는다.

### W6. text geometry와 Windows/Qt 텍스트 탐색

대상: framework RenderEditable/RenderParagraph와 paragraph geometry, semantics snapshot·ABI, Windows UIA provider, Qt QAccessibleTextInterface.

- [x] 먼저 실제 layout에서 text run·UTF-16 range·line boundary·selection·character/range bounds·hit test·scroll-to-range에 필요한 데이터를 공급한다. 현재 SemanticsNodeUpdate의 전체 node rect만으로 문자 rectangle을 만들어내지 않는다.
- [x] layout revision·view generation·DPR·screen transform·clip/scroll을 함께 다룬다. UIA/Qt callback에서 framework를 동기 재진입하지 않도록 immutable snapshot과 owner-thread action dispatch를 사용한다.
- [x] Windows에 ITextProvider/ITextRangeProvider를 연결하고 document/selection/visible range·range 이동/비교·bounding rectangle·point hit test·scroll을 구현한다. 이전 revision range의 무효화와 password 보호를 포함한다.
- [x] Qt `characterRect`, `offsetAtPoint`, `scrollToSubstring`의 stub을 실제 geometry/action으로 교체한다. managed ABI·template native 코드·샘플 native 사본·배포 shim 버전을 함께 갱신한다.
- [x] emoji surrogate·한글·combining sequence·RTL·line wrap·빈 텍스트·scroll 밖 range의 경계 계약을 최소 회귀로 고정한다.

완료 기준: 문자열 전체 bounds가 아니라 실제 위치별 range를 질의할 수 있다. 지원하지 않는 text geometry는 capability false를 유지한다. Windows provider와 Qt native query 통과에 더해 실제 AT 텍스트 탐색 상태를 따로 기록한다.

기술 근거: [Qt QAccessibleTextInterface](https://doc.qt.io/qt-6.8/qaccessibletextinterface.html)는 문자 rectangle과 point offset을 **screen 좌표**로 다루며 substring 표시 요청도 포함한다. framework local rect를 그대로 반환하면 계약이 맞지 않는다.

### W7. Windows editor·WebView 동시 합성

대상: WindowsPlatformViewHost, WindowsWinUiControls, WindowsWebViewComposition, composition planner/capability, W4의 MAUI adapter.

- [x] 기존 혼합 거절을 없애기 전에 HWND/WinUI island와 WebView2 visual의 실제 compositor·parent·z-order 경계를 확인한다. 하나의 관리 가능한 합성 topology 또는 OS가 ordering을 보장하는 native tree로 연결하는 최소 scene을 만든다.
- [ ] editor+WebView, WebView+editor, raster→editor→raster→WebView→raster, popup/shield와 blur 조합을 각각 확인한다. 플랫폼별 최대 effect·clip/transform·saturation 제한은 계속 검사한다.
- [x] reservation/token·generation·visibility commit·input shield·focus·raster slice·GPU lease를 같은 owner frame에서 관리한다. frame 오류·close·resize 중 기존 active scene의 수명을 보존한다.
- [x] topology를 제품에 연결하고 성공한 조합만 capability에 활성화한다. MAUI와 App SDK는 별도 통합 검사를 수행한다.

완료 기준: 두 view가 단순 생성된 것이 아니라 같은 frame에서 올바른 가림·클릭·Tab/포커스·scroll·resize·제거 순서를 유지한다. 구현 가능한 tree가 확인되지 않으면 기존 사전 거절과 남은 구조 과제를 유지하며, 해당 단계는 **PARTIAL**로 남긴다. CPU screenshot 합성으로 live view 지원을 대체하지 않는다.

### W8. Windows MAUI native GPU 입력과 복수 창

**W8-A GPU 입력** — 대상: MauiSkiaCapabilities, WindowsCompositionSurfacePresenter, 기존 SkiaNativeTextureEntry/VulkanNativeTextureImporter.

- [x] 실제 renderer/Graphite session·raster adapter LUID·import extension·format·extent 조건을 노출하고 native factory를 등록한다. adapter 값을 임의의 0으로 채워 지원을 광고하지 않는다.
- [x] producer synchronization과 마지막 GPU/native consumer release를 기존 import 계약에 연결한다. resize/loss/dispose의 published/retiring texture는 압력이나 제출 성공만으로 회수하지 않는다.
- [x] CPU PushFrame fallback과 GPU handle 입력의 기능·비용을 구분한다. compatibility renderer와 adapter 불일치는 명확한 unsupported 결과를 반환한다.

완료 기준: 실제 같은-adapter D3D 입력·미지원 format/extent·다른 adapter·owner 종료·producer/consumer 지연을 확인한다. GPU importer 등록만으로 camera 실기기/성능 PASS를 선언하지 않는다.

**W8-B 복수 창** — 대상: WindowsDesktopWindowHost.Factory, DorotiMauiApplication, shared application boundary·window manager.

- [x] main host 재사용 대신 실제 추가 MAUI Window·surface·input·semantics·registry·PlatformView owner를 생성한다. ApplicationBoundary 공유와 view 자원 분리를 기존 desktop 계약에 맞춘다.
- [x] ReadyToShow·WhenReady·close decision·lifetime policy·main 종료/추가 창 생존·서로 다른 DPR·picker owner·navigation/restoration ID 충돌을 처리한다.
- [x] 한 창의 retirement/loss가 독립 창의 새 attach·입력·render를 막지 않게 한다. WindowManagerCapabilities는 실제 지원되는 factory에만 true로 바꾼다.

완료 기준: 실제 창 두 개의 editor/WebView/picker가 각각 동작하며 하나를 닫아도 생존 창이 입력·resize·frame 진행을 유지한다. W8-A와 W8-B의 결과를 독립적으로 기록한다.

### W9. Web loss 재생성·offline 배포·서비스 수명

대상: BrowserFrameworkHost, doroti.web.ts/raster.worker.ts, browser font 설정, Web template/sample, navigation/clipboard 서비스.

- [x] 초기 renderer 자동 선택/fallback과 실행 중 graphics loss를 구분한다. fatal/새 세션 필요 이벤트를 앱이 관찰하고 명시적으로 재시작할 수 있는 공통 경로를 제공한다.
- [x] 재시작은 기존 세션·worker·texture·DOM/native view·입력 client·pending request를 정리한 뒤 새 canvas endpoint/session generation을 만든다. 이미 transfer한 canvas를 그대로 다시 transfer하지 않는다.
- [x] navigation/restoration 등 명시적으로 직렬화 가능한 앱 상태만 복원한다. GPU/native handle과 모든 임의 widget State가 자동 보존된다고 약속하지 않는다. 재시작 후 첫 frame·focus·서비스 owner·stale message 차단을 확인한다.
- [x] 기존 AssetsOnly/self-hosted/preload 옵션을 템플릿과 배포 preset에 노출한다. preset에 필요한 font·decoder를 실제 배포 assets에 포함하고 CDN/decoder download 여부와 실패를 진단한다.
- [x] offline preset에서는 startup/default·한국어 glyph까지 외부 font 요청이 없는지 확인한다. 네트워크를 기다리는 현재 기본값을 임의로 전 플랫폼 변경하지 않고 선택한 배포 계약을 문서화한다.
- [x] clipboard availability 조회가 암묵적으로 read permission을 요구하지 않도록 지원/unknown 상태를 분리한다. 실제 read는 명시 사용자 요청에 연결한다. Web 복원의 tab/session 범위와 native 앱 데이터 수명을 지원표·샘플에 반영한다.

완료 기준: WebGL loss/restore·WebGPU device loss의 재생성 fixture, worker/main ownership, pending cancel·old generation 거절, offline/CSP/decoder 차단·한글 렌더·font 실패, clipboard API/권한 부재를 확인한다. Chrome 자동 검사로 Safari/iPhone·실제 표시 FPS를 인수하지 않는다.

### W10. desktop 정책·닫기 보호와 펜 정규화

- [x] Catalyst의 programmatic CloseAsync 결정과 native scene close를 구분한다. native close 취소를 요구하는 앱은 생성 전에 capability를 평가하고 autosave/recovery 또는 지원되는 명시 닫기 UX를 제공한다. 공개 pre-close API가 확인되지 않은 상태에서 `canCancelNativeClose=true`로 바꾸지 않는다.
- [x] Qt 창 옵션은 xcb/Wayland/QPA별 보장을 나눠 평가한다. public Qt API로 구현 가능한 부분부터 연결하고 Wayland에서 위치/topmost/taskbar 보장을 추정하지 않는다. appearance 변경의 실제 runtime 적용 또는 RequiresRecreation 계약을 유지한다.
- [x] 펜 pressure·tilt·orientation의 공통 단위/범위·미지원 상태를 정의한다. Web tiltX/tiltY/twist, Qt x/y tilt/rotation, UIKit altitude/azimuth를 동일한 변환 규칙으로 연결하고 화면 방향/DPR 처리와 독립적으로 검사한다.
- [x] MAUI 일반 입력과 Graphite native 입력 등 여러 입력 adapter가 같은 계약을 사용하게 한다. stylus가 없는 장치의 임의 0을 관찰된 pen 지원으로 해석하지 않는다.

완료 기준: dirty document의 programmatic/native close 계약을 각각 확인한다. 창 옵션은 지원 QPA에서만 인수하며 unsupported 사유를 보존한다. 펜은 수치 변환 회귀와 실제 stylus 검증을 구분한다.

### W11. 개발·release 진입점과 최종 지원표

대상: doroti.ps1, release-candidate.py, development bridge·VS Code extension, templates·test manifest·문서.

- [x] workspace runner/backend 목록과 `developmentTargets`를 같은 지원 데이터에서 만든다. Windows MAUI Debug의 metadata update·reassemble·session 명령 연결을 구현하고 Run/Reload/Restart/Stop을 실제 backend에서 확인한다.
- [x] Android는 runtime·metadata update 지원을 먼저 평가한다. 후속에서 Debug Mono profile에 SDK/ADB development session을 연결하고 state/error/retry를 실기기로 확인했다. 지원되지 않는 profile은 배포 전 명확한 이유로 거절하며 Hot Reload 지원으로 광고하지 않는다. 일반 run/build 경로는 유지하고 Release/NativeAOT metadata reload를 허용하지 않는다.
- [x] 상위 release의 개별 android/macos/ios/maccatalyst 선택을 기존 하위 candidate 도구에 연결하고 platform/TFM/RID/host/toolchain 인자를 전달한다. 모든 MAUI pack을 한 실행에 합치는 하위 제한을 존중한다. Linux는 기존 Qt package/portable 경로와 별도로 매핑한다.
- [x] capability·source·payload·package identity, template dependency·native ABI를 최종 후보에서 확인한다. 수정 전 후보의 결과를 새 후보에 소급하지 않는다.
- [x] 각 단계의 실행 결과를 `Doroti/docs/validation/`의 새 날짜별 문서로 남기고 README/support-status/platform-view 표에서 연결한다. 기존 history를 현재 작업 기록으로 수정하거나 삭제하지 않는다.

완료 기준: 지원 대상의 명령이 실제 runner·bridge·candidate 경로로 도달하고 unsupported 대상은 초기 진단을 준다. 새 소비 앱의 격리 NuGet/template/native payload 경로를 통과한다. 패키지 생성·설치·서명·공증·clean OS·물리 입력 상태는 각각 기록한다.

## 5. 검증 방법과 증거 경계

[.github/copilot-instructions.md](.github/copilot-instructions.md)에 따라 테스트 명령은 **20분(1,200초) timeout**을 사용한다. 일반 반복은 30회 이내로 유지하며 실패한 조건과 그에 직접 영향을 받는 검사만 재실행한다.

- 최소 유지 회귀: 공통 filter/status·input configuration·generation/owner·semantics/text range·bounds/budget 계약처럼 실제 실패를 방지하는 assertion만 기존 test project에 추가한다.
- 일회성 scene·native probe·로그·스크린샷: `temp/testing/platform-audit/<stage>/<platform>/<run>/`. 새 임시 프로젝트를 일반 build/watch/package/CI graph에 등록하지 않는다.
- 관련 host 실행: 기존 Windows/Apple/macOS/Qt/Web smoke를 확장하고 Windows MAUI 전용 경로를 구분한다. Linux Desktop probe는 `-p:DorotiLinuxDesktop=true`로 빌드한다. Qt sample/template의 native 코드 사본과 ABI manifest를 함께 확인한다.
- 새 결과: commit·source 변경 여부·host/backend/renderer·OS/SDK/RID·payload hash·검사 명령·실패/재시도·미검증 조건을 기록한다. 로컬 artifacts는 삭제 가능하므로 필요한 집계/identity만 추적 문서에 보존한다.

다음 진입점은 전체 작업에서 실행했다. 후보별 결과는 전체 실행 기록을 따른다:

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/eng/validate.py Source
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Tests/Doroti.Tests.csproj -c Debug --artifacts-path temp/testing/platform-audit/contracts/build
python Doroti/eng/run-with-timeout.py --timeout 1200 node --experimental-transform-types --test Doroti/tests/web_rendering.mts
```

`Source` suite는 Apple 후속 검토에서 root `work.md`를 문서 목록에 추가해 이 계획의 상대 링크도 검사한다. capability query 테스트·mock provider·synthetic input·GPU receipt는 실제 native 연결이나 물리 사용자 경험의 통과와 구분한다.

| 상태 | 이 계획에서의 의미 |
| --- | --- |
| TODO | 아직 구현/검사를 하지 않은 단계 |
| PASS | 명시한 후보·환경·검사 범위에서만 통과 |
| PARTIAL | 구현 또는 일부 검사는 완료했지만 필수 연결/인수가 남음 |
| notVerified | 필요한 환경/장치에서 해당 동작을 확인하지 않음 |
| notMeasured | 실제 표시 FPS·입력 지연·메모리/성능을 측정하지 않음 |
| unsupported / conditional | 실제 OS/backend/feature 조건에 따른 계약. 구현 TODO와 구분 |
| SKIPPED | 해당 후보·검사에 대한 명시적 생략 기록. 다른 날짜의 새 결과에 확장하지 않음 |

## 6. 최종 완료 조건과 제외 범위

- [x] 감사의 12개 항목과 추가 입력/서비스 의미 차이에 구현·조건부 지원·OS 제한·남은 검증 상태가 모두 연결된다.
- [ ] 기본 기능 P1은 등록 코드 존재에 그치지 않고 각 host의 reachable path와 직접 관련 실행 검사를 갖는다.
- [x] 동적 입력 설정·UIKit controller·파일 필터·URL 상태·semantics 계층/텍스트·Windows 혼합 합성의 결과가 개별적으로 기록된다.
- [ ] Window/GPU/native view/AT/서비스 자원이 두 owner·resize·close·loss에서 수명 계약을 지킨다.
- [x] 현재 지원표·샘플·CLI·템플릿·패키지의 backend/renderer 계약이 일치한다.
- [x] 물리 IME·screen reader·pen·실제 모니터 이동·실제 표시 성능·장기 사용·배포 인수의 미완료 상태를 그대로 남긴다. 전체 PASS는 필수 인수가 모두 완료된 경우에만 사용한다.

이번 후속 계획에는 A/B 프레임 정책 복원, Qt 동적 예산 재구현, 모든 backend의 WGSL 신규 구현, native view의 임의 affine transform/효과 보장, UIKit material의 Gaussian 픽셀 동등성, browser 보안·OS 창 정책 우회, 새 기능과 무관한 광범위 성능 실험을 포함하지 않는다. 당초 실행의 Apple SKIPPED 기록은 보존한다. 이후 명시적인 Apple 후속 요청에 따른 새 W2/W3/W5 후보의 검증은 별도 후속 기록에 남긴다.

W0~W11 구현을 반영했다. 남은 체크와 각 완료 기준의 물리/배포 인수는 전체 실행 기록의 PARTIAL/notVerified/SKIPPED 범위를 따른다.

계획 작성 당시의 정적 확인(현재 실행 결과와 별도): 감사와 현재 소스·후속 기록 대조, W0~W11 단계 구조, 미완료 체크리스트, 상대 링크 51개 존재 여부를 확인했다. 이는 제품 구현·runtime 검증 결과가 아니다.
