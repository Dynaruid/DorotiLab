# 플랫폼 기능 공백 전체 구현 — 2026-10-03

사용자 요청은 root `work.md`의 W0~W11 전체 작업이다. 기존 W4 미커밋 변경을 유지하고 나머지 단계를 구현했다. Apple 코드 변경도 포함하지만 **Apple 빌드·실행·기기 검증은 사용자 요청으로 SKIPPED**했다. 이전 Apple 후보의 기록은 이번 변경의 증거로 합산하지 않는다.

Base HEAD: `96d30290500a7a82e714377537d8b1bcbd74ce13`. 모든 변경은 미커밋이다. 외부 게시·설치·서명은 수행하지 않았다. 전체 제품 인수는 **PARTIAL**이며 물리 IME/AT/pen, 모니터 이동, 표시 품질·FPS·장기 사용은 **notVerified/notMeasured**다. 모든 검사는 `run-with-timeout.py --timeout 1200`으로 제한했다.

## 단계별 구현과 경계

| 단계 | 구현 | 확인 / 남은 조건 |
|---|---|---|
| W0 | 기존 owner provider의 backend/generation·graphics/effect budget·texture 종류/format/extent/device·WebView operation·semantics 역할/action/text geometry·혼합 scene query. 최종 transform/DPR/clip/중간 이미지 수를 반영한 allocation preflight, SolidTint 샘플 | CPU bounds/budget/overflow PASS. attach/loss 뒤 다시 조회. Web capture 최대64 MiB, Vulkan effect 예산과 Qt R/P 동적 예산은 별개 |
| W1 | 공통 file filter 정규화·suffix 검사·grant 회수, 모든 picker 연결. URL scheme query, Web 정확한 status 전달·사용자 동작 mailto | CPU 계약 PASS. W4 HWND picker/read/cancel 기록 유지. Web opened는 브라우저 요청 수락이며 메일 앱 완료가 아님; worker 왕복은 activation을 잃을 수 있음 |
| W2 | MAUI configuration-only 변경·endpoint 상태 이전·callback 억제, Windows TextBox 실제 composition range. Windows/Qt native configuration-only 명령 | 빌드/input smoke PASS. MAUI selection 방향·endpoint 교체 composing 보존은 false. 물리 한글 IME·OS keyboard/capitalization/autocorrect notVerified |
| W3 | AppKit session/content를 Apple 공통으로 이동. UIKit의 동일 WKWebView에 controller 명령/event/profile/app content/message/lifetime 연결, legacy HTML JSON 호환 | Apple 코드 검토만 수행. AppKit/iOS/Catalyst 빌드·실행 SKIPPED |
| W4 | HWND picker/read grant·Windows 공유 WebView, owner-local MAUI WinUI NativeOverlay/coordinator/graphics와 embedded surface | [W4 기록](2026-10-03-windows-maui-basic-connections.md)의 후보별 결과 유지. MAUI는 disjoint overlay; foreground interleaving/backdrop/overlap/affine/legacy child-HWND 거절 |
| W5 | copied semantics tree·parent/ordered-child 계층·native PlatformView subtree 소유권 분리. MAUI WinUI AutomationPeer/Android delegate/UIKit traits/actions, owner/action/readOnly/password guards | 공통 hierarchy/action PASS, Windows native UIA 검사 PASS. OS 역할/action은 capability에 명시한 subset. 실제 Narrator/Orca/TalkBack notVerified; VoiceOver SKIPPED |
| W6 | 실제 TextPainter grapheme/line geometry→immutable UTF-16 snapshot, revision/transform/clip/password 보호. Windows ITextProvider/ITextRangeProvider, Qt characterRect/point/scroll-to-range | Windows UIA geometry/UTF-16/revision/owner/password 및 Qt QAccessible glyph rect/point/text PASS. merged child-local geometry는 합산하지 않음. UIA word 이동은 whitespace 경계, multiple selection/일부 attributes/child range 미지원. MAUI/Web native glyph range는 false |
| W7 | opt-in App SDK WebView를 DesktopWindowXamlSource/WinUI WebView2로 연결, editor와 같은 sibling HWND/Composition 경로. 공통 session/profile/명령/frame/lease/retirement | 동시 editor+WebView HTML/JS·8 instance 수명 PASS, mixed commit8/retirement0. 기본 legacy topology 유지. 전체 순서/raster/blur/popup 조합·Tab/물리 클릭/pixels/soak 인수 PARTIAL |
| W8-A | MAUI 실제 Graphite session의 Win32 external-memory/adapter LUID로 native importer 등록. zero/foreign adapter 거절, 마지막 소비 fence retirement | 실제 D3D11 GPU-complete BGRA12 frame import/release·wrong-adapter 거절 PASS. camera/다른GPU/format/producer 지연/forced loss/표시 pixels notVerified |
| W8-B | 실제 추가 MAUI Window/surface/dispatcher/input/semantics/registry/PlatformView owner, shared application lease·추가 창 독립 navigation | 두 native 창/editor·survivor resize·OnLastWindowClosed/Explicit·registry0 PASS. 실제 mixed-monitor DPR·두 창 picker/WebView 전체 동작 notVerified |
| W9 | public runtime 상태/event/restart. threaded main은 page reload, single-thread standalone worker는 fresh canvas/session, SDK build-profile로 소유권 사전 평가. pending lease/texture/frame-cost/old message 정리. Offline font preset/한글/decoder assets·clipboard Unknown query | Chrome main WebGL/WebGPU loss/restart, standalone WebGL crash/restart/gen1→3/첫frame/focus PASS. offline 실제 한글 glyph/외부요청0·decoder 거절 진단 PASS. 다른 브라우저/실기기/CSP 배포 별도 인수 |
| W10 | RequireNativeCloseCancellation 생성 전 정책·atomic/checksummed document recovery 샘플. Qt pure X11/xcb DPR1 위치/outer-frame ACK와 Wayland/XWayland unsupported 정책. pressure/tilt/orientation 공통 단위·가용 상태 | draft/corruption·pen 수치 PASS. Wayland 창/close PASS. Catalyst native close 취소 false 유지. WSLg xcb minimize ACK 실패는 위치 검사와 분리. 물리 pen/dirty document native close/monitor 이동 notVerified |
| W11 | 공통 runner/backend 목록·MAUI Debug metadata 개발·Android runtime 평가 후 restart/deploy/watch·VS Code 설정. 상위 release 개별 platform/backend·MAUI template·candidate feed/consumer | 실제 MAUI Run/Reload/Restart/Stop PASS. Android Debug Mono/JIT/AOT=false 평가 PASS; 연결기기 없어 deploy/watch notVerified. local Windows MAUI Release는 최종 후보 절 참조. Apple release SKIPPED |

## 자동 실행 결과

원시 결과는 `temp/testing/platform-audit/all/` 아래 disposable 파일이다. 아래 집계와 identity는 원시 파일 삭제 후에도 보존한다. 각 행의 후보·환경·범위만 PASS다.

| 검사 | 보존할 집계 |
|---|---|
| CPU `contracts-6` | PASS: filter/status/URL, DPR/budget/overflow, hierarchy/disabled/readOnly/password, layout geometry/graphemes/revision, draft checksum/version/corruption, pen 및 기존 suite |
| App SDK `windows-mixed-4` | PASS: Graphite/Vulkan→D3D12, scale2; 동시 editor+WinUI WebView HTML/JS, 8created, mixedXamlCommits=8, sessionPendingRetirements=0, readbackBytes=0. 실제 UIA layout text marker PASS |
| MAUI `maui-multi-4` | PASS: 두 창/editor·두 lifetime·survivor resize·최종registry0 |
| MAUI `maui-texture-2` | PASS: D3D11 shared BGRA12 producer release, wrongAdapterRejected=true, imported=retired/live=0; native test producer DLL. visiblePixels 미검증 |
| Web `web-recovery-main` | PASS: Chrome154.0.8037.93, 명시적 worker-direct-webgl/webgpu; graphics loss→page restart/첫frame/focus. 실제 C# clipboard Unknown/HasStrings=true/readText 호출0 |
| Web `web-worker-single-2` | PASS: WasmEnableThreads=false standalone WebGL, Chrome154.0.8037.93; generation1→3, workerRestartCount2, freshcanvas, unpairedRequestCount0, error0. threaded build는 main ownership |
| Fonts `offline-browser-4` | PASS: Assets startup, 실제 Skia koreanGlyphs=true/registeredBytes>0, 외부요청0. decoder 거절→WOFF2 decoding failed 진단; local Latin fallback으로 시작 가능. 한글 화면 pixels 미검증 |
| Linux `qt-runtime-2` | PASS: Qt6.10.2 Wayland input/desktop/multi, 실제 QAccessible glyph rect/UTF-16 point/text. software Vulkan; physicalGPU/Orca 미검증 |
| Linux `qt-runtime-3` | PASS: 새 DesktopABI2/State72/Command56 Wayland desktop/close/cleanup. producer/final Qt consumer fence 완료, reserved/retiring0 |
| MAUI dev `dev-maui/run-3` | PASS: metadata delta 중 PID32224/State/count5/Hangul/scroll160 유지·owner reassemble. rude field edit는 restart-required/자동restart 없음. Stop/Run 후 PID37740·새runtime, 두host Stop종료 |
| Android `android-dev/runtime.json` | net10.0-android/android-arm64 Debug, UseMonoRuntime=true/AOT=false 평가PASS. metadata transport unqualified→restart. adb 연결기기없음 |
| Node | VSCode contracts8 PASS; Web rendering/texture11 PASS (`--experimental-transform-types --experimental-vm-modules`) |

## 실패·수정과 native ABI

MAUI multiwindow 초기 실패는 dll 직접 실행과 MaximumWindows=1이었다. 실제 exe와 무제한 factory capability로 수정했다. GPU probe 첫 실패는 test producer DLL 이름 오류다. Qt Host API guard를 실제136bytes로 맞추고 Windows obj/NuGet 경로를 공유하지 않도록 Linux artifacts를 격리했다. WSL `/tmp` 삭제 때문에 persistent workspace 경로를 사용했다. template와 두 sample native 사본은 함께 갱신했다.

Web shorthand renderer 인자, 이전 manifest로 새 TS output을 실행하거나 빌드 도중 probe를 시작한 SRI empty-body 실패는 orchestration 오류이며 최종 인수에 포함하지 않는다. standalone worker의 COEP·startup error 미전달·restart policy 누락·single-thread WASM owner context 부재를 수정했다. document COEP를 상속하는 same-origin blob ES module은 CSP의 worker-src blob 또는 적절한 worker COEP header가 필요하다. threaded build는 main ownership을 사용한다.

Offline probe의 과거 삭제 fixture를 현재 Roboto/MaterialIcons/SUITE로 교체했다. decoder 거절은 explicit font diagnostic+가능한 Latin fallback이라는 실제 동작으로 검사한다.

Release 첫 후보는 Host.Maui RID-less build/RID pack 불일치로 실패했다. 두 번째 후보의 startup failure는 패키지 버전 충돌이 아니라 **ReadyToRun이 SkiaSharp managed bytes를 변경**한 문제였다. SDK/Windows MAUI target의 `PublishReadyToRunExclude=SkiaSharp.dll`로 official managed asset을 보존한다. [공식 R2R exclusion 계약](https://learn.microsoft.com/en-us/dotnet/core/deploying/ready-to-run)을 사용하며 provenance hash 검증을 우회하지 않는다.

Windows host ABI3/config feature9/text feature10. Qt host ABI7/config feature23/text feature24, HostAPI136bytes/Pointer184bytes. Qt optional DesktopABI2: State72/Command56/API48bytes. 이전 version/size는 조기 거절한다.

## 재현·배포 계약

App SDK 혼합 opt-in은 `DOROTI_WINDOWS_MIXED_NATIVE_VIEWS=1`; fixture는 `DOROTI_MIXED_SCENE_PROBE=1`, `DOROTI_SAMPLE=input`도 사용한다. opt-in WebView backdrop는 false이며 기본 Windows.UI.Composition WebView와 HWND editor 혼합 거절은 유지한다.

Template `DorotiWebFontPreset=Offline`은 SUITE Korean/Latin variable WOFF2+OFL·local decoder+MIT를 실제 배포하고 자동 CDN/glyph fallback을 끈다. 기본 preset 네트워크 정책은 유지한다. Web clipboard HasStrings는 paste 제공을 위한 낙관적 값이며 실제 내용 관찰이 아니다. 복원은 Web tab sessionStorage/native app data이며 명시적인 직렬화 상태만 복원한다.

`dev -Platform windows -WindowsBackend Maui -Configuration Debug`는 metadata/reassemble 경로다. `dev -Platform android -Device <serial>`은 평가 후 restart loop이며 Hot Reload로 광고하지 않는다. Release/NativeAOT metadata update는 거절한다. release는 한 MAUI platform/backend씩 하위candidate로 전달한다. Linux는 기존 Qt publish/package/installer를 유지한다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/eng/validate.py Source
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Tests/Doroti.Tests.csproj -c Debug --artifacts-path temp/testing/platform-audit/all/contracts-final/build
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/tests/hot_reload_smoke.py temp/testing/platform-audit/all/dev-maui/run-3 temp/testing/platform-audit/all/dev-maui/app Maui --verify-restart
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/eng/release-candidate.py --targets windows --windows-backend Maui --version 0.3.0-beta.audit.20261003.3 --output temp/testing/platform-audit/all/release-maui-3
```

## 최종 후보 identity

최종 Source suite(문서/runner/installer), 공통 CPU 전체 suite, Android Debug, Web Debug, Qt6.10.2 isolated Debug build, VSCode8·WebNode11 검사는 PASS다. Android Debug는 warning/error0이며 기기 deploy는 수행하지 않았다. Apple 빌드/실행은 SKIPPED다. Source suite의 owned raw run은 요약 후 삭제됐다.

| 후보 | 버전 | 생성 시 source-tree SHA256 | 결과 |
|---|---|---|
| Windows MAUI | `0.3.0-beta.audit.20261003.3` | `146a6dbfebe1e269131536bf31d5c8de6f13bb3ad4a806f1a97eba1c6bd4a28c` | PASS; 27 packages / 468 payload files |
| Windows App SDK | `0.3.0-beta.audit.20261003.4` | `da67aa228d93b5a1c76ca5b0cca2154b4961e8b18e2c61bac578ef311cf12b24` | PASS; 26 packages / 550 payload files |
| Web Offline | `0.3.0-beta.audit.20261003.5` | `70a267d8e5412591af8c30695dae26ed32bfcf33ad332ad8ab488a79fab7f950` | PASS; 23 packages / 823 payload files |
| Windows MAUI final preflight | `0.3.0-beta.audit.20261003.6` | `1222f4a3b69a3753e51e42d9ede0269a45fcfc5e3f6ccfddc8de7428ed66f329` | PASS; 27 packages / 468 payload files |

Windows 후보들은 격리 feed/cache의 NuGet-only template consumer를 실제 실행했다. nativeFirstFrame/twoWindows/secondResizedAndClosed=true, main 종료 전 survivors=1, 정상 process 종료를 기록했다. MAUI SkiaSharp managed hash는 official `702657b10552a9aa75dd65ca63fac752e0bb795a9cab3fb169ff52cb2ecee7b9`, Win32 native hash는 `07ce51fd59e099b9561b0327223c27b21aa5605b5b8f4484dd297fdb8c8725a1`이다.

Web `.5`는 `--web-font-preset Offline`의 NuGet-only Release consumer다. SUITE535788bytes/decoder295397bytes/두 license와 threads=true runtime profile을 배포했다. `web-package-accepted`: Chrome154.0.8037.93에서 실제 첫 frame/focus/외부요청0 PASS. loopback 서버는 COOP/COEP와 .mjs=text/javascript를 명시했다. 첫 서버의 Windows registry .mjs=text/plain 설정은 fixture 오류였고 수정했다.

현재 Source Debug의 `web-main-clean`은 WebGL/WebGPU loss 뒤 **같은 renderer를 유지**하는 page restart/첫frame/focus/clipboard read0 PASS다. `web-worker-stable`은 single-thread worker crash/명시restart/gen1→3/새canvas/첫frame/focus/미완료lease0 PASS다. `web-profile-threaded`는 threads=true에서 standalone worker를 .NET allocation 전 명시 진단으로 거절한다. SDK가 배포하는 runtime-profile.json을 사용하며, worker JS 파일 존재로 threading을 추정하지 않는다. 기본 Testbed는 마지막에 threaded Debug로 복원했다.

`offline-csp-final`: 외부요청0/실제 한글glyph/decoder 거절 진단 및 worker-src none CSP 실패 상태/event/restart 경로 PASS. 일반 CSP 조합과 실제 한국어 화면 pixels는 별도 인수다. `qt-wayland-contract` desktop/input, `qt-xwayland-contract` desktop PASS: Wayland/XWayland 위치/centering을 사전 거절하고 close/cleanup을 확인했다. XWayland placement/minimize ACK의 앞선 실패는 성공으로 합산하지 않는다. pure X11/xcb DPR1 위치 요청 구현은 compositor의 실제 outer-frame ACK를 요구하며, 거절/timeout은 unsupported 진단이다. pure X11 실제 위치·물리 monitor 검증은 notVerified다.

후보 snapshot 뒤의 추가 Source startup-error guard와 Apple selected/action-label 변경은 해당 후보의 전체 source 결과로 소급하지 않는다. 현재 Debug build/실행 범위와 Apple SKIPPED를 별도로 기록한다. 패키지·payload 전체 목록의 canonical SHA256 및 주요 개별 hash는 [보존 identity](2026-10-03-platform-gap-identities.json)에 남겼다. 원시 consumer/cache/log는 candidate 도구가 성공 후 정리했으며 남아 있다고 보고하지 않는다. 원시 output/이미지는 disposable이다. signing/clean-machine install/물리GPU·IME·AT/배포 인수는 notVerified다.


## Semantics native mapping subset

| 계약 | Windows App SDK / MAUI | Qt QAccessible | Android MAUI | UIKit/Apple 변경 |
|---|---|---|---|---|
| 계층/순서 | snapshot parent/children, UIA fragments / nested WinUI peers | parent/child/index | nested native containers | nested wrappers; Apple 실행 SKIPPED |
| dialog/list/item | Window/List/ListItem roles | Dialog/List/ListItem | Dialog/ListView; item은 일반 View | 정확한 container role은 광고하지 않고 기본 traits/label 사용 |
| table/row/cell/header | App SDK Table / MAUI DataGrid, DataItem/HeaderItem; 전체 Grid/Table patterns는 별도 | Table/Row/Cell/ColumnHeader roles | 정확한 table/row/cell role 미지원 | 정확한 table/row/cell role 미지원 |
| menu/item/tab | Menu/MenuItem/Tab/TabItem | PopupMenu/MenuItem/PageTab | PopupMenu/TabWidget subset | 정확한 menu/tab role 미지원 |
| 상태 | heading/expanded/selected/enabled/readOnly/password provider guard | QAccessible state + password Value 보호 | heading, enabled/selected/checkable/password; expand/collapse action | Header/Selected/NotEnabled traits, protected value; custom action. 빌드 SKIPPED |
| action | 실제 advertised Invoke/Value/Toggle/ExpandCollapse/ScrollItem subset; UIA layout range는 App SDK | 실제 action names/text selection/scroll range subset | click/long/setText/setSelection/expand/collapse/show | Activate/Long press/Increase/Decrease/Expand/Collapse/Show custom labels, live policy guard; Apple 검증 SKIPPED |

이 표는 역할·provider 연결 범위다. 역할 이름만으로 모든 native pattern/상태/action 또는 실제 AT 탐색을 지원한다고 주장하지 않는다. 미지원 action은 성공으로 소모하지 않으며 현재 owner/hidden/disabled/readOnly/password 조건을 검사한다.

최종 보강: scene query는 native live source가 없는 backdrop와 NativeOverlay 앞쪽 raster ordering을 allocation 전에 거절한다. `contracts-accepted` CPU 전체 suite와 native allocation0 회귀 PASS. 이 공통 query 변경과 최신 Apple 코드의 Windows 제외 조건을 포함한 MAUI `.6` NuGet-only Release 소비 앱은 실제 두 창/첫frame/resize/종료 PASS다. `.3`은 이전 후보 기록으로 보존한다. 기본 threaded Web는 single-thread profile 전환 후 SDK clean/rebuild로 재생성했고, `web-main-clean`에서 WebGL/WebGPU 모두 같은 renderer 복구 PASS다. 새 Source startup-worker error guard는 실패를 무기한 starting으로 남기지 않는다.
