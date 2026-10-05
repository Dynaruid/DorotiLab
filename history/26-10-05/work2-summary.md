# 디자인·플랫폼 독립 구조와 Windowing 요약 — work2.md

원문 작성일: **2026-10-04 KST** · 갱신/보관일: **2026-10-05 KST**.

**기존 구현·검증 이력 보존, 전체 수락 미완료, 잔여 실행은 [work3 요약](work3-summary.md)으로 이관**한다. 원문의 실행 체크는 **21개 완료·80개 미완료**이며 최종 수락 체크리스트도 미완료다. 부분 구현/자동 PASS를 단계 전체 완료로 승격하지 않는다. [execution.json](../../Doroti/docs/migrations/design-platform/execution.json)의 `complete=false`와 [checkpoint](../../Doroti/docs/migrations/design-platform/latest-checkpoint.json)의 `wholePlanComplete=false`를 유지한다.

기반 증거는 [소유권 ADR](../../Doroti/docs/migrations/design-platform/application-ownership.md), [baseline·이동표](../../Doroti/docs/migrations/design-platform/baseline.json), [재개 기록](../../Doroti/docs/migrations/design-platform/resume-2026-10-05.md), [재개 검증 receipt](../../Doroti/docs/migrations/design-platform/resume-verification-2026-10-05.json)이다. [work.md 리뷰](work-summary.md)의 기존 PASS와 새 구조의 검증은 별개다. 계획 작성 당시의 조사/예정 계약과 후속 실제 구현을 구분한다.

## 사용자 결정과 패키지 경계

Flutter의 디자인 분리·out-of-tree provider·앱 시작 전 등록·Windowing 방향을 Doroti의 공개 계약으로 선행 적용한다. 호환 shim 없이 소비자가 새 패키지/API/schema로 직접 전환하는 구조변경이며 `tools/Doroti.DartToCSharp/`는 제외했다. 사용자 지정 **루트 `packages/`**를 사용한다.

| 영역 | 전환 계약 |
| --- | --- |
| 코어 | 공통 Widgets/Rendering/Painting/Gestures/Animation/Services/Semantics·Ui/Runtime·Hosting·재사용 렌더러는 `Doroti/src/`에 유지 |
| Material | `packages/Doroti.Material/`, PackageId/AssemblyName/namespace `Doroti.Material`. Material 색상 알고리즘·MaterialColorUtilities·InkSparkle descriptor·localization/아이콘 소유 |
| Cupertino | `packages/Doroti.Cupertino/`, 정체성 `Doroti.Cupertino`, 디자인별 theme·selection/magnifier/toolbar·localization/아이콘 소유 |
| provider | `packages/platforms/windowsappsdk`, `web`, `qt`, `maui`에 Host/Target·native/eng/bootstrap/tooling. Host/Target의 기존 정체성은 유지 |
| App/Runner | 앱은 코어·명시 디자인, runner는 앱·선택 Target. OS 필수 shell·앱 ID/권한/아이콘/서명은 runner 입력, 공통 초기화는 provider 소유 |
| 버전 | 코어 릴리스 묶음·각 디자인·각 provider 묶음의 버전 분리. 초기 계획은 core/provider/tool `0.4.0-alpha.1`, 디자인별 `1.0.0-alpha.1` |
| 디자인 선택 | `dotnet new doroti-app --design widgets|material|cupertino`, 기본값 `widgets`; 소비자 namespace를 명시 import |

core/Host/renderer/공통 SDK → 디자인 및 core/공통 SDK/CLI → 구체 provider 참조를 금지한다. **Material → Cupertino는 허용, Cupertino → Material과 순환은 금지**한다. 디자인 패키지는 core friend assembly 대신 검토된 public/protected 확장을 사용한다. 구 `Doroti.Framework.Material/Cupertino` 정체성·facade·TypeForwardedTo·이중 Compile을 남기지 않는다.

RawMenuAnchor/RawRadio/RawTooltip/Expansible/EditableText/SelectableRegion/RawScrollbar 등 기존 기반을 재사용한다. 일반 theme/adaptive 컨트롤, 신규 Material 3 Expressive/Liquid Glass, renderer 전환·컴파일러 변경은 필수 범위가 아니다.

Flutter 참고는 공식 독립 디자인 패키지 방향, 열린 시작 전 hook 제안, 실험 Windowing, 도구 extension prototype을 구분한다. 로컬 prototype 소스 조사 이력은 실행 증거가 아니며 일부 experimental 경로는 이후 reference에 없어 재검증되지 않았다. Flutter 내부 FFI/OS switch/Isolate RPC를 Doroti 내부 계약으로 고정하지 않는다.

## 시작·소유권·typed 경계

시작은 **정적 launch plan → PrepareProcessAsync → ProcessPrepared → startup/Configure·plugin factory → application session → primary 생성/adopt → view 등록/Seal → branch attach → FirstFrameSubmitted** 순서다. 앱 코드를 사전 plan 평가에서 실행하지 않으며 OS가 만든 primary를 다시 만들지 않는다. .NET module initializer나 임의 static 초기화 전체를 통제한다는 뜻은 아니다.

하나의 application은 하나의 dispatcher/binding/build owner와 **논리 widget tree**를 소유한다. 여러 View branch는 WindowId/viewId/generation·RenderView/PipelineOwner·surface/frame·FocusScope/IME/Semantics/Navigator/restoration을 각각 가진다. native factory는 entrypoint/root를 만들지 않고 같은 Desktop manager가 공통 Window API와 기존 명령/close 수명을 관리한다.

| 수명 | 자원·종료 계약 |
| --- | --- |
| process | OS loop/native library·provider process lease. primary close가 살아 있는 app/survivor loop를 끝내지 않음 |
| application | framework root/dispatcher·window manager·공유 plugin/resource/menu. session coordinator가 마지막 view/callback drain 뒤 해제 |
| view/window | native context·registry·surface generation·입력/IME/semantics. `Owned` 등록만 view가 Dispose, `Borrowed` 공유 서비스는 session이 한 번 해제 |
| frame/consumer | 불변 제출 snapshot·image/texture lease·완료/거절 결과. 실제 GPU consumer 완료 또는 확인한 loss까지 유지 |
| tool extension | typed service/invocation 또는 process RPC peer의 독립 lifetime. 연결 종료와 앱 Stop을 구분 |
| CLI session | app/build·ADB/Web process, Hot Reload/Restart/Stop 소유. tool reconnect가 앱을 임의 재시작하지 않음 |

종료는 새 작업 거절 → widget unmount → callback/IME/semantics 해제 → native/GPU drain → view → 공유 app/plugin → framework → process다. 늦은 생성 결과는 attach하지 않고 자체 정리한다. drain timeout은 실패·보유 lease·재회수 경로로 남긴다.

같은 .NET runtime의 서비스/플러그인은 **interface·typed request/result·Task/ValueTask·CancellationToken**, owner 이동은 bounded `Channel<T>`/typed queue를 쓴다. 살아 있는 Widget/Element/BuildOwner/Scene/native allocation을 다른 owner에 직접 넘기지 않는다. frame drop 정책으로 입력/close/terminal을 버리지 않으며 consumer별 lease를 유지한다.

직렬화는 JS/worker·실제 process wire·native C ABI·manifest/IDE/영구 기록에 한정한다. logical DTO·wire DTO·ABI struct를 분리하고 source-generated JSON의 required/null/enum/64-bit ID/version·reflection fallback 거절, native encoding/layout/buffer/callback 수명을 검사한다. 내부 codec 호출 0을 확인하되 계획만으로 zero-copy/성능 개선을 주장하지 않는다.

## provider·도구·Windowing 계약

`doroti.platform-provider/v1`, `doroti.target-package/v2`, `doroti.workspace/v2`, `doroti.tool-extension/v1`이 identity/version/core range/protocol/ABI·OS/TFM/RID·alias/backend/profile·operation을 선언한다. resolved package와 평가 metadata를 대조하고 `GetDorotiPlatformContract`는 restore/evaluation 뒤 정적 계약을 제공한다. restore/describe/design-time에서 native build·device 연결·앱 시작을 하지 않는다. 독립 소비자는 prebuilt nupkg를 쓰며 native 소스 재빌드는 명시 개발 operation이다.

도구 기본 mode는 managed CLI와 .NET extension의 **`dotnet-inproc` typed 호출**이다. ALC/DependencyResolver는 선언 entry만 로드하고 Contracts assembly identity를 host와 공유한다. ALC를 앱 provider 등록에 사용하거나 강제 중지/crash 격리로 해석하지 않는다. 격리·다른 runtime·비.NET에는 **`process-stdio`**를 명시하고 bounded UTF-8 Content-Length JSON-RPC 2.0을 경계 adapter에서 처리한다. load/계약 실패를 다른 mode 성공으로 숨기지 않는다.

configuration/device/diagnostics/templates/operations는 실제 서비스·실행 계획을 반환한다. CLI executor가 인자 배열·CWD·timeout·exit·process/ADB/Web 세션을 소유하며 shell 문자열을 eval하지 않는다. runtime provider 등록, manifest 발견, tool handshake를 별개로 추적한다.

WindowKind는 **Regular/Dialog/Popup/Tooltip/Satellite**다. owner·modal/result/focus 복구, popup anchor/outside click/Escape, Tooltip no-activate, Satellite owner-relative 이동/close를 실제 OS에서 수락한다. 다중 owner/docking은 별도 capability다. **Auto/Native/Overlay**는 필요한 route/theme/locale/restoration/focus/Semantics를 포함해 평가한다. Auto는 Unsupported에만 Overlay를 쓰며 owner 오류·취소·ABI/device/native 실패를 성공으로 바꾸지 않는다. Web/mobile의 native 추가 창 제한과 OS별 제약을 유지한다.

## 단계별 상태와 후속 대응

| 원 단계 | 원문의 완료 체크 | 부분 기반과 남은 최종 수락 |
| --- | --- | --- |
| DS0 | 0/4 | closure·파일/타입 이동·의존·버전/범위의 evaluated/semantic 감사 |
| DS1 | 0/5 | packages 공통 build 품질과 TFM/RID·개별 버전·native/prebuilt 경계 최종 검사 |
| DS2 | 4/4 | 디자인 이동·개명·솔루션/소비자 전환·구 디렉터리 제거 |
| DS3 | 4/4 | Runtime Material 색상 의존 제거·friend 제거·core closure의 디자인 중립성 |
| DS4 | 5/5 | 소유자 shader 등록/hash/ABI/resource·아이콘 공급/라이선스·기존 locale 계약 |
| DS5 | 0/5 | Raw/wrapper·입력/selection/transition·listener dispose와 물리 IME/AT |
| DS6 | 3/4 | 세 디자인 선택·혼합 identity 거절·repo/NuGet 소비 완료. DS6-1 SDK/provider 최종 경계 남음 |
| DS7 | 4/5 | 디자인별 테스트·평가 graph/nuspec/AssemblyRef·격리 소비자·집계 완료. DS7-4 독립 버전/최소·범위 밖 조합 남음 |
| DS8 | 1/4 | 두 샘플 새 디자인 참조 완료. DS8-2~4 전 플랫폼 runtime·자산/trim/AOT·물리 수락 남음 |
| DS9 | 0/5 | release/version/profile·receipt·문서·구 구조 제거·배포 결과 정합성 |
| PW0 | 0/6 | 실제 capability/schema/ABI·ADR·upstream·G0/G1 기준 감사 |
| PW1 | 0/7 | typed 등록·소스/native 이동·public 계약·Owned/Borrowed·내부 codec 0 전체 수락 |
| PW2 | 0/6 | 모든 OS staged startup/adopt/Seal/attach·실패/취소/Restart·단일 cleanup |
| PW3 | 0/8 | workspace/Target·생성 자산·typed operation·외부 provider·tool services/ALC/prebuilt |
| PW4 | 0/7 | shared tree·입력 격리·bounded frozen submission·실제 두 창 survivor·slow consumer/재진입·factory root 결합 제거 |
| PW5 | 0/5 | Windows 다섯 종류의 실제 owner/focus/modal/좌표/close/drain·mixed DPI/물리 수락 |
| PW6 | 0/4 | 디자인 Native/Overlay route 결과·typed OS 메뉴·native Tooltip과 완전한 restoration/focus/Semantics |
| PW7 | 0/6 | CLI/doctor/IDE·builtin/외부 tool services·얇은 entry·실행 세션/릴리스 후보 |
| PW8 | 0/7 | Qt/MAUI/Web/mobile 전환·aggregate·호환 조합·typed/process/wire/ABI·trim/AOT·물리 수락 |

표의 0 체크 단계에도 구현과 scoped PASS가 있다. 통합 milestone은 **M0 공통 계약/G0 → M1 Windows 두 Regular/G1·독립 provider → M2 디자인·다섯 창·도구 → M3 나머지 provider·제품 수락**이다. G0 fake/CPU 결과를 native G1이나 전체 Windowing 수락으로 대체하지 않는다. M1–M3의 전체 완료는 원문에서 선언하지 않았다.

재개 기록은 Cupertino caller theme/locale, native handle의 늦은 취소 회수, RawTooltip late install, Windows attach/startup receipt, MAUI PrepareProcess 뒤 descriptor 생성, provider identity/range 대조, Qt GUI owner·survivor 종료, layout builder/Overlay 재빌드 완료 기록, typed font event·tool doctor 등 실제 결함 수정과 후보를 보존한다.

기록된 Windows 독립 후보는 core alpha.2 / Material alpha.3 / Cupertino alpha.2 / provider alpha.4, Web은 core alpha.2 / Cupertino alpha.2 / provider alpha.4다. Windows 두 창·MAUI Graphite·Web Offline first frame/focus/외부 요청 0, Qt strict **WSLg Wayland/software Vulkan subset**, 당시 Developer/Release와 후속 fixture PASS를 각 원래 후보 범위로 유지한다. 물리·서명/clean-machine 및 새 Apple/Android 결과는 별도 [work3](work3-summary.md)를 따른다.

## 재개·보존 기준

work3는 80개 미완료 원 항목을 44개 후속 항목으로 연결한다. 최종 완료는 소유권·구 identity 부재·패키지/버전/자산·SDK/템플릿·startup/lease/frame·native 창/메뉴·provider/tool·wire/ABI·trim/AOT·플랫폼 runtime/물리/배포를 모두 증거와 대조한 뒤 판단한다.

유지보수 진입점은 `design_package_contract.py`, `platform_provider_contract.py`, `platform_bootstrap_contract.py`, `frame_submission_contract.py`, `tool_extension_contract.py`, `typed_transport_contract.py`, 디자인별 테스트와 Build/Packages/Targets/Developer/Release·host smoke다. 외부 timeout **1,200초**, 반복 통상 **30회 이내**를 적용한다.

`temp/testing/design-decoupling/`, `temp/testing/platform-decoupling/`, `Doroti/artifacts/`는 disposable이다. 영구 상태·hash·명령·한계는 연결한 migration/receipt에 남긴다. 이번 보관은 새 구현·runtime 수락이 아니다.
