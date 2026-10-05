# work2 구현 세션 인계 — 2026-10-05

전체 작업은 **미완료**다. 사용자의 세션 중단·인계 요청에 따라 새 구현과 추가 검증을 중단했다. 다음 세션은 현재 소스/diff와 `work2.md` 전체를 독립적으로 검토해야 한다. 이 문서는 이전 PASS를 최신 전체 수락으로 확대하지 않는다.

## 재개 기준과 작업 트리

- 저장소: `C:\Users\parti\Labo\DorotiLab`. 기준 문서: 루트 `work2.md`; 지침: `.github/copilot-instructions.md`. 문서의 과거 조사 전용 설명보다 사용자의 후속 전체 구현 지시가 우선한다.
- M0/M1/M2 진행 중, M3 미완료. `execution.json`의 `complete=false`를 유지한다. G0/G1 아래의 PASS는 명시된 부분 검증이다.
- 기존 사용자 수정과 이전 세션의 수정이 섞인 큰 미커밋 diff를 그대로 보존했다. reset/restore/clean/stage/commit/push/PR/배포를 수행하지 않았다. 이 문서에서 세션별 변경 소유권을 추정하지 않는다.
- 전체 실제 변경 경로는 `checkpoint-files.txt`, HEAD·상태별 개수는 `session-handoff-state.json`, tracked diff 요약은 `handoff-diff-stat.txt`에 있다. rename은 Git 상태에서 삭제/신규로 나타날 수 있다.
- `tools/Doroti.DartToCSharp/`는 제외 범위다. 관련 코드/fixture를 수정하지 않는다.
- 현재 환경은 Windows/PowerShell이다. 저장소 쓰기·빌드는 이 세션의 sandbox상 `require_escalated`로 실행했다. 승인 거절은 없었다. 실행 도우미는 쓰기 가능한 `C:\Users\parti\Documents\Codex\2026-10-05\task-2`에 있다. **과거 편집 도우미를 재실행하지 말 것**: 오래된 소스를 덮어쓸 수 있다.
- 모든 실행 테스트/빌드는 종료됐다. 마지막 G0 PASS 후 `dotnet build-server shutdown`도 성공했다. CIM 재확인에서 dotnet/python/cmake/MSBuild/Doroti 실행 프로세스가 없었다. 하위 에이전트를 생성하지 않았다. 무관한 Codex/Figma 프로세스는 건드리지 않았다.

## 전체 계획 대비 상태

아래 표는 계획의 기존 체크박스와 기존 execution 작업 상태를 보존한 스냅샷이다. `complete`인 개별 이전 항목도 최종 변경 뒤 전체 회귀/수락 완료를 뜻하지 않는다. pending 항목 일부에도 구현이 시작됐으므로 아래 단계 설명과 실제 diff를 함께 본다. 모든 DS/PW 단계의 최종 수락은 미완료다.

| 단계 | 기존 완료 | 기존 진행 중 | 기존 미착수 |
|---|---|---|---|
| DS0 | — | DS0-1, DS0-2, DS0-3, DS0-4 | — |
| DS1 | — | DS1-1, DS1-2, DS1-3, DS1-4, DS1-5 | — |
| DS2 | DS2-1, DS2-2, DS2-3, DS2-4 | — | — |
| DS3 | DS3-1, DS3-2, DS3-3, DS3-4 | — | — |
| DS4 | — | DS4-1, DS4-2, DS4-3 | DS4-4, DS4-5 |
| DS5 | — | — | DS5-1, DS5-2, DS5-3, DS5-4, DS5-5 |
| DS6 | DS6-2, DS6-3, DS6-4 | DS6-1 | — |
| DS7 | DS7-1, DS7-2, DS7-3, DS7-5 | DS7-4 | — |
| DS8 | DS8-1 | — | DS8-2, DS8-3, DS8-4 |
| DS9 | — | DS9-1, DS9-2, DS9-3, DS9-4, DS9-5 | — |
| PW0 | — | PW0-1, PW0-4 | PW0-2, PW0-3, PW0-5, PW0-6 |
| PW1 | — | PW1-1, PW1-2, PW1-6 | PW1-3, PW1-4, PW1-5, PW1-7 |
| PW2 | — | PW2-1, PW2-2, PW2-3, PW2-4, PW2-5 | PW2-6 |
| PW3 | — | PW3-1, PW3-2, PW3-3, PW3-4, PW3-5, PW3-6, PW3-7, PW3-8 | — |
| PW4 | — | PW4-1, PW4-2, PW4-3, PW4-4, PW4-5, PW4-6, PW4-7 | — |
| PW5 | — | PW5-1, PW5-2, PW5-3 | PW5-4, PW5-5 |
| PW6 | — | PW6-1, PW6-2 | PW6-3, PW6-4 |
| PW7 | — | PW7-1, PW7-2, PW7-3 | PW7-4, PW7-5, PW7-6 |
| PW8 | — | — | PW8-1, PW8-2, PW8-3, PW8-4, PW8-5, PW8-6, PW8-7 |

기존 계획에서 체크된 항목 (16개): DS2-1, DS2-2, DS2-3, DS2-4, DS3-1, DS3-2, DS3-3, DS3-4, DS6-2, DS6-3, DS6-4, DS7-1, DS7-2, DS7-3, DS7-5, DS8-1. 최종 acceptance 체크를 추가하지 않았다.

| 단계 | 현재 결과와 남은 핵심 |
|---|---|
| DS0 | 평가 그래프·이동표·소유권 ADR 존재. 최종 파일/타입·자산·ABI/범위 지원표 갱신 필요. |
| DS1 | packages 공통 빌드/독립 버전/메타데이터와 provider 정책 분리 구현. 전역 Version 전파·역할별 평가·core OS 정책 잔여 확인 필요. |
| DS2 | 디자인 소스/정체성/소비자 이동 항목 기존 체크 완료. 최종 구 경로/중복/namespace 검색 필요. |
| DS3 | Runtime 색상 의존 Material 이동 및 friend 제거 항목 기존 체크 완료. 최신 전체 전이 그래프 재확인 필요. |
| DS4 | owner별 shader 등록 기반 구현. 실제 byte hash/ABI 부정 fixture, 아이콘 폰트 공급·라이선스·locale/fallback 최종 증거 필요. |
| DS5 | 실제 EditableText 합성 focus/selection/IME G0 PASS. Raw/widget wrapper, 실제 입력·접근성·전환 회귀는 미완료. |
| DS6 | widgets/material/cupertino 템플릿·혼합 참조 거절 구현/이전 소비자 PASS. 최신 SDK/provider 분리 뒤 재검증 필요. |
| DS7 | 독립 nupkg 네 조합·일부 버전 조합 이전 PASS. 마지막 Future/window/SDK 변경 뒤 재검증 및 집계 필요. |
| DS8 | 실제 Windows 일부 native/GPU probe PASS, Qt/Web/MAUI 이전 빌드 PASS. 최신 startup/interaction/shutdown·trim/AOT 및 장비 검증 미완료. |
| DS9 | 일부 이동 문서/증거 존재. release-candidate 독립 버전/provider 해석·문서 링크·최종 잔여 제거 미완료. |
| PW0 | 이동표/ADR/G0/G1 fixture 존재. 전체 공개 signature/schema/ABI/capability 지원표 확정 미완료. |
| PW1 | typed 서비스·Owned/Borrowed·actual-completion lease·plugin registry 구현, G0/tool 부분 PASS. 전체 capability/race/ABI 수락 미완료. |
| PW2 | staged bootstrap와 공유 session/실패 cleanup 구현. MAUI 종료·startup 모든 실패 단계 및 각 provider runtime 미완료. |
| PW3 | provider-owned SDK 자산·manifest/외부 provider·isolated Windows consumer 구현/이전 PASS. fresh package metadata restore/identity/range/ABI validation 미완료. |
| PW4 | WindowContent/root factory 결합 제거, shared tree·per-view·Future owner 경로 구현. 최신 실제 G1/slow GPU/recreation/native focus 회귀 미완료. |
| PW5 | Windows 5종 창·owner/modal/noactivate 및 OS 메뉴/메뉴바 부분 PASS. 최신 종료 변경 뒤 재실행, extended 입력/물리 mixed-DPI 검증 필요. |
| PW6 | typed menu adapter, native dialog/tooltip portal 구현 중. design probe 마지막 실패, tooltip 미검증, Cupertino context menu·restoration/focus/Semantics 미구현/미완료. |
| PW7 | thin PS1/managed CLI, typed provider doctor options와 bounded tool process 구현. 실제 최신 doctor/device/fresh provider/IDE/릴리스 회귀 미완료. |
| PW8 | Qt/MAUI/Web source ownership/shared session 구현 및 이전 빌드, tool 실제 child process 최신 PASS. 전체 OS/runtime/집계/ABI/trim/독립 버전 조합 미완료. |

## 실제 주요 변경 파일과 동작

정확한 전 파일 목록은 `checkpoint-files.txt`가 기준이며 아래는 다음 검토를 위한 주요 진입점이다.

- `packages/Doroti.Material/`, `packages/Doroti.Cupertino/`: 새 PackageId/AssemblyName/namespace, 각 Version.props, 색상 알고리즘·MaterialColorUtilities 소유권 이전, shader 등록. core/design 테스트와 샘플/템플릿 참조 이동.
- `packages/Directory.Build.props`, `.targets`, `Directory.Packages.props`, `Doroti/Directory.Build.*`, `Doroti/eng/build/`: 공통 품질 규칙/제품 역할/버전과 provider profile 분리.
- `packages/platforms/{windowsappsdk,web,qt,maui}/`: Host/Target/Diagnostics/native/tooling 이동 및 provider별 Version.props. `packages/platforms/build/`와 패키지 `provider-build/`에 OS build 자산 분리. Runner SDK 활성화 flag로 일반 Target 소비자가 executable/AOT 규칙을 가져오지 않도록 수정.
- `packages/platforms/qt/native`, `qt/eng/build-native.py`: Qt native 원본 단일화와 bounded 빌드. sample/template 중복 원본 삭제. `packages/platforms/shared/Windows/`: Windows 키맵/shared code 소유권 이동.
- `Doroti/src/Doroti.Hosting/DorotiSharedHostSession.cs`, `DorotiManagedPluginRegistry.cs`, provider ApplicationSession/runner startup: application당 root/owner, descriptor factory once, staged Run(Func), process 준비 선행, typed plugin/lifetime 관리. Qt/desktop MAUI/Web 공유 session 연결.
- `Doroti/src/Doroti.Desktop/WindowHost.cs`, Builder/WindowManager/WindowController, `Doroti.Ui/Windowing.cs`, Widgets `windowing.cs`/`View.cs`: WindowContent/native root factory와 LegacyMainWindow/FromLegacy 제거. DefaultMainWindow/FromViewConfiguration, typed WindowSnapshot.Closing 및 close-start notification. core→provider friend 제거와 typed scene/frame 공개 진입점.
- `Doroti/src/Doroti.Framework.Widgets/platform_menu_bar.cs`, Ui menu DTO, `WindowsAppSdkDesktopWindowHost.Menu.cs`, 새 `.MenuBar.cs`: context/view lifetime에 귀속된 typed 메뉴 delegate, HWND HMENU와 WM_COMMAND, WindowId/viewId/generation, replacement/retirement/취소. Windows session이 Borrowed host의 application owner를 보유하도록 수정.
- 새 `Doroti/src/Doroti.Ui/FramePresentation.cs`: bounded 실제 committed-frame 대기 capability. Windows runner에서 해당 framework frame의 composition commit 후 완료한다.
- 새 `Doroti/src/Doroti.Framework.Widgets/NativeWindowPresentation.cs`, `dialog.cs`, Material dialog 및 Cupertino route: shared application view collection 내부 portal, captured theme/localization/directionality, Auto/Native/Overlay 정책, result/close/cancellation, 실제 frame 후 Show. native cross-window barrierDismissible은 미지원: Auto는 Overlay, explicit Native는 오류.
- Widgets `raw_tooltip.cs`, Material `tooltip.cs`: native content portal, explicit nativeSize/nativeCompatible, noactivate Tooltip/anchor, hover generation/cancellation/cleanup scaffold. 최신 기능은 빌드만 PASS, runtime 미검증이다.
- **가장 마지막 구현** `Doroti/src/Doroti.Runtime/FutureAwaiters.cs`, `DartAsync.cs`, `Doroti.Ui/PlatformDispatcher.cs`, `Doroti.Tests.Platform.Contracts.Tests/Program.cs`: Future await가 owner/view callback dispatcher로 돌아오도록 custom awaiter, application microtask를 owner queue에 직접 배치. 실제 await 뒤 owner/view assertion을 G0에 추가, 마지막 G0 PASS. Windows Release executable은 마지막 Ui 변경 이전이므로 재빌드 필수.
- `Doroti/src/Doroti.Tooling.Contracts/Contracts.cs`, `Doroti.Tooling.Extension.Sdk/DotnetToolExtension.cs`, `ProcessOutput.cs`, ProcessToolExtension/ExecutionSession/serialization 및 `Doroti/tools/Doroti.Tooling/`: typed doctor dotnet/timeout/scope options, provider workload/tool requirements, TFM/RID plan; bounded stream/log reader와 failure 시 child cleanup/restart. thin `doroti.ps1`의 실제 host dotnet과 provider probe executable 분리.
- `samples/DorotiTestbedApp/src/NativeDesignPresentationProbe.cs`, `desktop/NativeMenuProbe.cs`, `Doroti/tests/windows_design_presentation.py`, `windows_native_menu.py`, `windows_provider_packages.py`, `tool_extension_contract.py`, G0/frame/typed/provider/design package contracts: 실제 source-owned 검증 및 부정/cleanup fixture.

## 실행한 검증: 시점과 범위

모든 명령은 저장소 루트에서 실행한다. 이 표의 PASS는 실행 당시 소스에 한정한다. `execution.json`의 오래된 PASS 문자열보다 아래 최신 시점 제한과 raw/summary 증거를 우선한다.

| 결과 | 명령/실행 | 실제 증거·한계 |
|---|---|---|
| **PASS 최신** | `python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/tests/platform_bootstrap_contract.py` | 마지막 session 96915; bootstrap-verification.json run `ee9118eaa58e4ab6ae9ce619a742fdf3`, UTC 2026-10-04T23:15:16. 실제 shared widget tree/2 CPU RenderViews, synthetic EditableText, codec 0, lease/staged failure/drain retry 및 추가 Future await owner/view assertion. native/physical 수락 제외. |
| **PASS 최신 Tool 소스** | `python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/tests/tool_extension_contract.py` | session 23782. concurrent inproc/ALC Contracts identity, 실제 child handshake/result/error/cancel/crash/hang/reconnect/app Stop/Restart, oversized 100000-char log --wait 실패 시 child kill/restart. 이 wrapper는 tool-verification.json을 쓰지 않는다. |
| PASS 이전 | `python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/tests/windows_native_menu.py` | session 71006; windows-native-menu-verification.json, raw `temp/testing/platform-decoupling/native-menu/868a70dd51184f40824882a2d157917e`. 실제 popup/취소/닫힘, HWND 메뉴바, replacement generation/WM_COMMAND, complete=true pending=0. 뒤의 dialog/frame/Future/tooltip 수정 이전. |
| PASS 이전 | `python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/tests/windows_provider_packages.py` | session 3538; windows-provider-package-verification.json, raw `temp/testing/platform-decoupling/windows-packages/f896c64420574a57b745377dc3a87238`. isolated feed/cache pack/restore/publish, 실제 native 2창/shared app/primary survivor/final drain. 뒤의 dialog/frame/Future/tooltip 수정 이전. |
| **FAIL 마지막 실제 실행** | `python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/tests/windows_design_presentation.py` | session 40832; 마지막 raw `temp/testing/platform-decoupling/design-presentation/a34c141c1b984aac98fe04f19326e5f9`. 첫 Material native result 23/child GPU drain 뒤 Overlay Show에서 90초 timeout. 최신 Future/application microtask 수정 뒤 **재실행 안 함**. Cupertino까지 진행되지 않음. windows-design-verification.json 없음. |
| PASS, 최종 Ui 수정 전 | `python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build samples/DorotiTestbedApp/windowsappsdk/DorotiTestbedApp.WindowsAppSdk.csproj -c Release` | session 60762, 0 warning/error; tooltip/custom Future 포함. 마지막 application microtask 변경은 이 Release bin에 없음. |
| PASS 이전 | Material/Cupertino 프로젝트 각각 `dotnet build ... -c Release`, timeout 1200 wrapper | session 86398, native tooltip scaffold 포함. 최종 Future/Ui 수정 전. |
| PASS 이전 | managed CLI Release build (`Doroti/tools/Doroti.Tooling`) | session 28450, typed doctor options/profiles 포함. 마지막 tool cleanup/PS1 수정 전; 최신 실제 CLI doctor는 미실행. |
| PASS 이전 | `python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Tests -c Release` | session 59560, WindowContent 제거/메뉴/Future의 최종 변경 이전. 최신 전체 core 회귀 재실행 필요. |
| PASS 이전 | `design_package_contract.py`, `platform_provider_contract.py`, `typed_transport_contract.py`, `frame_submission_contract.py` (동일 timeout wrapper) | 독립 4 디자인 조합/버전/template; 외부 laboratory-x provider 실제 CLI; typed codec/lease; frozen CPU frame/epochs. 각 docs verification JSON 참조. 최신 SDK/doctor/Future 변경 뒤 결과 아님. |
| PASS 이전 | `windows_shared_tree.py`, `windows_window_kinds.py` (동일 wrapper) | actual Windows 2 Regular/primary survivor/GPU drain 및 5 종류 HWND/owner/modal/noactivate. raw G1 `1af90b80b30b488f8c9868e0dd3d97b4`, kinds `ab20ecfb53ce414e8480a2da3c8e5ff8`. 후속 변경 뒤 재실행 필요. |
| PASS 이전 | Qt Host Release, Web runner Release/TypeScript/Emscripten wasm, Windows MAUI runner Release | sessions 75524/44077/18436. native Linux/browser/mobile runtime PASS 아님. 후속 friend/shared-session/Future 변경 뒤 재빌드 필요. MAUI Host session 40585 결과 회수 안 됨: PASS로 계산하지 않음. |
| PASS 이전 | IDE `cmd.exe /c npm run test` (timeout wrapper) | session 10709, 8 tests. 실제 VSIX runtime 미실행, 최신 provider metadata 후 재검증 필요. |
| PASS 이전 | thin PS1 describe v2 / source builtin doctor | sessions 6432/10039. 최신 typed scope/probe 변경 전. |
| NOT RUN 최신 | `validate.py Build`, `Packages`, `Targets`, `Developer`, `Release` | 과거 Build PASS가 있어도 최종 소스 전체 집계 PASS는 없다. stale runner/doctor/release fixture 먼저 점검 필요. |
| NOT RUN | Linux Qt/display, Apple native/signing/device, Android/iOS 실제 장치, physical keyboard/IME/mixed-DPI/slow GPU | 현재 Windows 환경에서 대응 장비/OS 수락 증거 없음. Windows MAUI/browser 가능한 runtime도 이번 종료 시점 미완료이며 환경 불가 항목과 구분해야 한다. |

실패 후 복구·통과한 과거 원인: nupkg private assets PackPath(NU5129), 일반 Target에 Runner SDK AOT 규칙 유입(DOROTIAOT001), borrowed MenuHost application owner 누락/unstaged Run로 consumer startup hang, G0 await assertion의 app.Stop 이후 삽입(순서 수정). 최신 전체 결과를 대신하지 않는다.

## 알려진 결함·미검증 구현

1. **native design coordinator 최종 실패 재검증이 최우선**. 마지막 Future/owner microtask 수정은 headless G0만 PASS. Native/Overlay route 종료, early native close와 shared root/dialog focus/localization/theme/semantics/result를 Release 재빌드 후 확인한다. probe의 자동 post-frame pop은 실제 Show 전 발생할 수 있으므로 가시성 수락을 주장할 때 Visible 관찰이 필요하다. Manual hidden-window cleanup 완화는 물리 표시 PASS가 아니다.
2. **native tooltip는 runtime 미검증**. ShowContentAsync에서 child close는 entry.Closed만 설정하며 Mounted/presentation await의 owner CT를 취소하지 않아 owner가 살아 있으면 대기가 남을 수 있다. await 뒤 unmounted RawTooltip에서 View.of(context)/Install이 throw하면 새 ContentHandle이 누출될 수 있다. owner capture/finally cleanup, late generation/returned-handle cancellation 검토 필요. 자동 content measurement 미구현; explicit nativeSize만 지원. preferBelow/verticalOffset/custom position 정확한 정책과 content resize 수락도 남았다.
3. **Future custom awaiter**: accepted continuation owner/view는 G0 PASS. owner post 거절 때 rejection-thread에서 GetResult 예외까지 재개하는 경로, generic/non-generic cancellation/cleanup/trim 및 application microtask queue admission/dispatch race를 검토한다. Qt/MAUI GUI thread와 application owner의 연결도 실제 runtime에서 확인해야 한다.
4. **MAUI 종료 순서**: Windows ExitRequested가 factory.Dispose/Application.Exit을 초기화 작업 receipt 완료 전에 수행할 수 있다. controller InitializationWork를 추적/완료한 뒤 최종 종료하도록 해야 한다. public Use(descriptor)는 Func staged 경로와 달리 process 준비보다 이른 factory 생성 위험이 남는다. Web CreateView/Qt constructor startup 모든 단계 실패 cleanup도 추가 검토 필요.
5. **메뉴**: Windows MenuBar의 separator/shortcut/role은 구현했으나 Popup Evaluate/append는 새 DTO의 shortcut/role 미지원 거절과 separator 처리 누락 가능. AppKit/Qt 메뉴 capability 미구현. closed DesktopHost의 application owner registry 누적 여부 검토. 오래된 toChannelRepresentation API 잔여 정리 필요.
6. **CLI fresh package discovery**: PlatformMetadata가 각 runner obj/project.assets.json에 의존. template tool manifest의 unversioned nuget:ToolId와 독립 tool version pinned restore/resolver 정리 필요. GetDorotiPlatformContract/core/protocol/native ABI 불일치 거절 완성 필요. MSBuild metadata reader failure fail-fast도 검토한다.
7. **doctor/device**: typed DotnetPath/Timeout/Scope 계획/프로브는 구현됐지만 최신 실제 CLI 미실행. workload/tool 최소 버전 및 TypeScript/Emscripten/native ABI 검증이 부족하다. mobile ADB/simulator device discovery/transport는 unsupported 상태. source 검색으로 실제 MSBuild 평가/프로브를 대체하지 않는다.
8. **release-candidate.py 미완료**: old Doroti/src Host/Target/Material 경로, global 0.3-beta VersionOverride/static target mapping/hardcoded aliases가 남는다. provider metadata로 roots/assets/operations를 해석하고 독립 core/design/provider/tool versions+provenance receipt+isolated template/feed consumers 구현이 필요하다. 현 상태에서 릴리스 성공을 주장하지 않는다.
9. **core/provider 경계 잔여**: Doroti/Directory.Build.props의 DorotiAndroidTargetFramework, MAUI SupportsDesktop Android/iOS 적용, Desktop 고정 OS regex/provider SDK builtin asset 묶음 검토. Apple/Android sample bootstrap/native 원본 소유권 이동은 Qt만큼 끝나지 않았다. 구 runner_contract.py/doctor_contract.py의 fat PS1/v1 fixture를 계약 보존하며 이행해야 한다.
10. **디자인 나머지**: Cupertino context menu native 정책 미구현. adaptive dialog의 explicit presentation 정책, restoration/native focus/Semantics·Raw wrapper/physical IME·shader 부정 fixture/font/license/locale 범위 및 trim/AOT 최종 검증 필요. 문서의 old source links/history는 당시 증거를 보존하며 갱신한다.

## 다음 세션 실행 우선순위

1. 작업 트리/지침/work2 전체와 이 문서의 파일을 읽어 현재 코드를 독립 검토한다. 체크된 16개 항목을 제외한 구현/검증을 완료로 추정하지 않는다. 기존 사용자 diff와 증거를 보존한다.
2. Release Windows sample을 다시 빌드한 뒤 아래 bounded probe로 마지막 수정의 실제 효과를 확인한다. 실패 raw/log를 유지하고 native/Overlay route owner·early close·drain을 먼저 해결한다. script는 이미 빌드된 Release dll을 사용하며 자동 build하지 않는다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build samples/DorotiTestbedApp/windowsappsdk/DorotiTestbedApp.WindowsAppSdk.csproj -c Release
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/tests/windows_design_presentation.py
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/tests/platform_bootstrap_contract.py
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Tests -c Release
```

3. native tooltip cancellation/late install leak와 Cupertino context menu, native focus/restoration/Semantics 미완료를 구현한다. Windows shared tree/kinds/menu/nupkg consumer를 최신 bin/source로 재실행해 종료/GPU lease 회귀를 확인한다.
4. MAUI ExitRequested/InitializationWork/staged registration과 Web/Qt 실패 cleanup를 완성하고 가능한 Windows MAUI/browser runtime을 검증한다. 실제 unavailable OS/device 항목은 별도 SKIPPED로 남기고 독립 구현은 계속한다.
5. fresh package-only provider/tool discovery, doctor/device/IDE, provider-resolved independent-version release 후보와 stale regression fixture를 완성한다. core/provider build/version/asset ownership 잔여를 평가 그래프로 확인한다.
6. DS4/DS5/DS9와 PW8의 나머지 회귀·문서/소스 검색·집계 Build/Packages/Targets/Developer/Release를 완성하고 실제 지원 조합만 receipt에 기록한다. 외부 push/PR/배포/서명/공개는 별도 승인 없으므로 수행하지 않는다.

인계 시점에는 새 구현/검증 작업을 실행하지 않는다. 마지막 프로세스 정리와 handoff 파일 기록만 수행했다.
