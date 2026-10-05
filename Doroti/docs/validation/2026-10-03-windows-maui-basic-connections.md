# Windows MAUI 기본 연결 — 2026-10-03

요청 범위는 root `work.md`의 **W4 Windows MAUI 기본 서비스·PlatformView 연결**이다. Windows 기본 backend는 App SDK이며 MAUI는 선택 backend다. 기본 연결 구현과 명시한 자동 검사는 완료했지만 제품 인수는 **PARTIAL**이다. 물리 IME·Tab·UIA·펜·실제 monitor/DPR 전환·live resize 표시 품질·장기 사용·Release 설치는 notVerified다. W7 interleaving, W8 native GPU texture import·복수 창은 이 결과에 포함하지 않는다.

## 후보와 환경

- base HEAD: `96d30290500a7a82e714377537d8b1bcbd74ce13` + 이 작업의 미커밋 변경. 계획의 이전 기준 `bd75ced5...` 결과를 새 후보로 소급하지 않는다.
- Windows 11 Pro `10.0.26300`, .NET SDK `10.0.400`, `net10.0-windows10.0.19041.0`, `win-x64`, Debug, MAUI `10.0.90`.
- 설치 GPU: AMD Radeon 780M `32.0.13031.3015`, NVIDIA RTX 4060 Laptop `32.0.16.1062`. 테스트 surface DPR은 2다. 이 목록을 두 GPU 모두의 인수로 해석하지 않는다.
- WinUI는 기존 1.8 계열을 유지한다. WebView2 패키지는 기존에 해결되던 `1.0.3179.45`를 Windows MAUI의 명시적 dependency로 고정했다. 설치 WebView2 runtime의 기능은 실제 실행 범위에서 확인했다.
- 원시 결과: `temp/testing/platform-audit/w4/`. 각 maintained MAUI 검사에는 `identity.json`/`connections.json`의 실행 모드, HEAD, 미커밋 상태와 DLL SHA-256을 남긴다. 원시 파일은 삭제 가능한 로컬 증거다.

## 구현

`src/Shared/Windows`의 HWND-owned picker, read grant, owner dispatcher, WebView2 command/document/content session 소스를 두 host가 직접 compile한다. MAUI가 App SDK runner assembly를 참조하지 않는다. 기존 public `WindowsFilePicker`는 공통 서비스 wrapper로 유지했다. MAUI picker는 호출 시 UI thread에서 실제 window HWND를 얻고, 동시 picker·caller/owner 취소·read grant 해제를 보존한다.

MAUI 전용 dispatcher/factory/host를 application boundary, coordinator, platform-view channel과 graphics painter에 연결했다. `doroti/native-button`, `doroti/native-editor`, `doroti/webview`는 **NativeOverlay**, translation, rectangular clip, 직접 native 입력을 제공한다. WebView controller는 지원되는 overlay 전략도 선택한다. 기존 WebView2 profile·navigation·JavaScript·app content·clear data·문서 generation 계약을 공유한다.

WinUI raster는 `DorotiWindowsDxgiHost.CompositionTarget`에 붙고 native slots는 같은 host의 Canvas에 놓인다. PlatformView manifest가 있는 dedicated page도 이 WinUI tree를 사용한다. Embedded surface의 전체 창 권한은 false이며 MAUI parent의 위치·크기·clip·DPR을 따른다. 기존 root native window를 교체하지 않는다. PlatformView가 없는 dedicated page는 기존 HWND/DComp 출력 경로를 유지한다.

InterleavedComposition, overlapping native regions, native backdrop, affine transform과 legacy child-HWND compatibility 출력은 지원하지 않는다. 요청 query와 기존 overlay planner가 이를 거절한다. Ganesh는 `DOROTI_WINDOWS_MAUI_GRAPHITE=0` **및** `DOROTI_WINDOWS_COMPOSITION_SURFACE=1`의 WinUI Composition 경로에서 검사했다. 불투명 overlay로 interleaving을 광고하지 않는다.

Windows용 `Framework.Services` 참조, WebView2 dependency와 Testbed application manifest를 보완했다. Repository runner의 NativeFeatures 생성 등록은 두 Windows MAUI sample에서 `Doroti.Plugins.targets`를 명시적으로 import한다. NuGet에는 Services/WebView2 dependency와 기존 `buildTransitive/Doroti.Host.Maui.targets`가 포함된다. 새 native ABI나 고급 target capability를 추가하지 않았다.

## 검사와 결과

모든 테스트 명령은 `run-with-timeout.py --timeout 1200`으로 실행했다. 동일 조건 수백 회 반복은 하지 않았다.

| 검사 | 결과와 범위 |
|---|---|
| Testbed/Sample2 MAUI Debug build | PASS, 최종 빌드 warning/error 0 |
| MAUI Graphite owner integration | PASS (`final-owner/`): native button/editor 생성·placement/clip·detach·해제, WebView 재생성, JS, old document 거절, app content, Ephemeral/SharedPersistent 경로, clear data, dispatcher context 즉시 복원, NativeFeatures picker·read·동시 선택 거절·caller/owner 취소, native child 0, exit 0 |
| Embedded MAUI surface | PASS (`final-embedded/`): 48 logical pixel MAUI header 아래 재배치, full-window 권한 false, 동일 기본 native 서비스와 정리, exit 0. 자동 native API 검사이며 실제 monitor 이동은 notVerified |
| Ganesh + WinUI Composition | PASS (`final-ganesh/`): 같은 owner integration, exit 0. Legacy child-HWND 출력의 PlatformView는 unsupported |
| 실제 WebView widget scene | PASS (`final-webview/`): `WebViewWidget` → scene planner/painter → WinUI slot → HTML/JS, raster frame, screenshot 검토, exit 0. 실제 Web content와 Doroti toolbar가 표시됐다. Screenshot의 DPI 좌표로 일부 창이 잘려 있어 전체 창의 픽셀 golden·resize 표시 품질 인수로 쓰지 않는다 |
| Sample2 Upload 탭 | PASS (`final-upload/`): 실제 다중 선택 OS dialog에서 txt 한 개 선택, `Windows MAUI upload 한글` preview, caller grant 해제, exit 0. WM_CHAR/OS dialog commands를 사용한 synthetic 입력이다 |
| MAUI Desktop smoke | PASS: `desktop-qualified/`의 API/native close 상태·취소·정리, `input-qualified/`의 editor/WebView 네 번 생성·재생성과 정상 종료. 기본 exe가 App SDK인 smoke에 MAUI exe를 명시했다 |
| 공통 CPU 회귀 | PASS: close 완료가 retired view dispatcher를 기다리지 않음, state-only picture와 drawing/saveLayer/unknown의 overlay 구분, native handle retirement/owner 분리, 기존 widget/rendering contracts |
| App SDK 공통 picker 회귀 | PASS: 실제 HWND dialog 선택·read/release·user/caller 취소·owner close drain 및 loopback URL 경로. 물리 사용자 입력 인수가 아니다 |
| App SDK 기본 Desktop/input 회귀 | PASS (`appsdk-final/`): 기존 backend의 API/native close 및 editor/WebView 재생성, 세 process 모두 exit 0. 복수 창과 물리 입력은 이번 기본 검사에 포함하지 않았다 |
| Windows MAUI NuGet pack | PASS (`packages-final/`): Windows-only package의 DLL/PRI, buildTransitive, Services/WebView2 dependency 확인. 격리 NuGet 소비 앱 설치·Release 배포는 notVerified |
| Source / 문서·runner·installer contracts | PASS: 최종 문서 경로와 임시 파일 정책, timeout/exit·installer 회귀. 실행 ID `d80196a4123040c791792ba6cfbe83b3`; 성공 summary 후 원시 산출물은 자동 정리됐다 |

최종 다섯 maintained MAUI mode는 같은 아래 runtime DLL identity에서 통과했다. 앞선 `upload2/`, `webview-scene6/`의 다른 DLL 결과를 최종 후보로 소급하지 않았다. Hash는 원시 증거 삭제 후에도 후보를 식별하도록 여기 보존한다.

| Runtime 파일 | SHA-256 |
|---|---|
| Doroti.Host.Maui.dll | `476dd11591692a888c9bba7e2e4cb9cae7998ca2161c6d817bd5ae7222bcc505` |
| Doroti.Framework.Services.dll | `0d5204b011dc288b7aa2f036412179f0e0f071af7bc23d65a93ab0beb9a9dfec` |
| Doroti.Desktop.dll | `cd1a60f5f5032e841d8c5be103708a17a4230a70050fd1581887e4331b514c0e` |
| libSkiaSharp.dll | `07ce51fd59e099b9561b0327223c27b21aa5605b5b8f4484dd297fdb8c8725a1` |

별도 Windows RID pack의 `Doroti.Host.Maui.0.3.0-beta.nupkg` SHA-256은 `e8e6ab4c06df4d2a1fec2f55a53c662e6063cb331afa6ef05a88a7a7662144fd`, 그 안의 host DLL은 `57c2d2b26d7a9d5775532cbc0750d794f7614d026080677c5523612b06ec99ed`다. 이는 package 내용·dependency 검증 후보이며 위 sample runtime DLL과 동일 바이너리 인수로 취급하지 않는다.

주요 재현 명령:

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build samples/DorotiTestbedApp/windows/DorotiTestbedApp.Windows.csproj -c Debug --nologo -v quiet -m:1
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build samples/DorotiSampleApp2/windows/DorotiSampleApp2.Windows.csproj -c Debug --nologo -v quiet -m:1
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/tests/windows_maui_smoke.py --output temp/testing/platform-audit/w4/new-graphite --mode graphite
```

다른 mode는 `embedded`, `ganesh`, `webview`, `upload`다. 항상 새 output directory를 사용한다. 공통 회귀는 `dotnet run --project Doroti/tests/Doroti.Tests/Doroti.Tests.csproj -c Debug --artifacts-path temp/testing/platform-audit/w4/new-contracts/build`를 같은 1200초 wrapper로 실행한다. Pack은 `dotnet pack packages/platforms/maui/Doroti.Host.Maui/Doroti.Host.Maui.csproj -c Debug -p:RuntimeIdentifier=win-x64 --output temp/testing/platform-audit/w4/new-packages -m:1`를 사용한다.

## 실패와 보완

- 초기 compile의 WinUI/MAUI type ambiguity와 Windows namespace shadowing을 해소했다. 최종 빌드의 결과만 build PASS로 집계했다.
- WinUI Composition 완료 후 native batch가 이미 닫힌 경우를 확인했다. 완료가 확인된 batch의 event unsubscribe만 허용된 정리 예외로 처리한다. 미완료 GPU 자원을 timeout으로 해제하지 않는다.
- 창 종료에서 view dispatcher를 retire한 뒤 그 dispatcher의 continuation을 기다리는 경로를 제거했다. 독립된 close-completion 회귀와 실제 native 종료로 확인했다.
- 파일 picker의 caller cancellation에서 Close 호출이 STA에 도달해도 modal 창이 남았다. 전용 STA timer에서 Close를 호출하고, 해당 COM dialog의 IOleWindow로 얻은 HWND에 native Cancel을 전달한다. 외부 dialog를 검색하거나 선택 결과로 위장하지 않는다. 근거: [IFileDialog.Close](https://learn.microsoft.com/en-us/windows/win32/api/shobjidl_core/nf-shobjidl_core-ifiledialog-close), [Common Item Dialog의 IOleWindow](https://learn.microsoft.com/en-us/windows/win32/shell/common-file-dialog).
- 다중 선택 dialog 자동화는 ComboBoxEx의 WM_SETTEXT만으로 filename model이 갱신되지 않았다. 실제 내부 Edit에 문자 입력을 보내는 maintained fixture로 수정했고 새 실행에서 통과했다.
- 위젯 scene의 Debug banner와 unclipped app bar는 실제 foreground overlay여서 planner가 거절했다. 기본 sample route의 banner를 끄고 app bar/인접 raster를 명시적으로 clip한다. State-only picture의 의미는 renderer와 planner가 같은 helper로 판단하되 saveLayer/unknown/drawing은 계속 보수적으로 거절한다.
- Native widget 재생성 중 retired handle의 retained replay를 새 GPU failure로 처리했던 경로는 `DorotiFrameSupersededException`으로 terminal한다. 이미 retired된 이전 visible handle 때문에 live replacement의 배치를 건너뛰지 않는다.
- input lifetime은 재생성 네 번과 registry close가 완료돼도 초기 후보에서 종료 후 CoreMessagingXP `0xc000027b`/COM `0x80004003`가 발생했다. Async owner dispatcher가 await 중 UI thread의 ambient SynchronizationContext를 남기던 문제를 바로 복원하도록 보완했다. 이후 동일한 input-only native 검사에서 exit 0을 확인했으며 owner fixture에 context 복원 assertion을 추가했다. 단순히 `.closed` 파일 존재를 clean exit로 인수하지 않았다.
- `--no-build` pack은 RID-less build 경로 및 이전 multi-target restore 그룹과 맞지 않아 실패했다. Windows RID를 선택한 정상 restore/build/pack으로 확인했다. 경고를 숨겨 package PASS로 처리하지 않았다.

## 남은 경계

단일 Windows 장치의 Debug 자동 연결 검사다. 물리 한글 IME·후보/caret·Tab/Shift+Tab·UIA·펜, 실제 monitor/DPR 변경, 다양한 resize/clip 조합, GPU loss, 10분 사용, 표시 FPS/latency, Release/trim/AOT·설치·clean OS는 notVerified 또는 notMeasured다. 혼합 foreground/interleaving, native backdrop, native GPU texture import, 복수 MAUI 창은 이 기본 구현의 지원으로 확장하지 않는다. W0/W2/W5/W7/W8/W11의 나머지 작업은 별도다.
