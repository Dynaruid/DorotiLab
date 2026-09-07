# Cross-platform boot 구현 및 검증 결과 — 2026-09-07

- 요청: repository root `plan.md` P0–P6 전체 실행. 시작 HEAD `ccf61de`, 시작 working tree clean. 9월 6일 계획의 `0499c1d` 분석을 현재 source와 대조한 뒤 구현했다.
- 판정: **공용/Web/Windows/CLI 구현 및 가능한 target 검증 완료. 전체 플랫폼 acceptance는 미완료**다. Apple host, arm64 실기기, 물리 IME/접근성/scan-out 및 대응 성능 비교는 `notVerified`다.
- 원시 증거: [20260907-implementation](../../.doroti/evidence/boot/20260907-implementation/). `<label>.result.json`에 실행 명령, exit code, timeout, 경과 시간, 같은 이름의 stdout/stderr에 원문을 저장했다. 모든 build/test 외부 harness 제한은 1,200초다. 임시 Web server는 별도 수명으로 관리했다.
- 구현 source와 측정 artifact는 구분한다. `web-publish-final`은 Release native WASM build이며 managed AOT가 아니다. 마지막 Testbed CLI build는 별도 artifact다. `artifact-inventory.json`에 실제 파일 hash/크기를 저장한다. 숫자는 현재 artifact의 관측값이며 전후 개선률이 아니다.
- `source-inventory.json`은 최종 변경 파일별 hash, `execution-index.json`은 69개 개별 실행 결과의 원문 인덱스다. 이 수는 테스트 assertion 수나 PASS 수가 아니다. 최종 `git diff --check`, 변경 PowerShell parser/MSBuild XML/JSON, 문서 상대 링크 검사는 PASS다. Web publish 디렉터리 300개 파일의 합은 67,924,711 bytes이며 압축 대체 파일까지 포함하므로 navigation wire 크기가 아니다.

## P0. 부트 판정

Web loader `started` 의미는 runtime/GPU readiness 그대로다. UI Worker의 host-ready/framework-attached/startup-failed trace와 기존 frame/generation trace를 연결했다. `startup.spec.ts`는 exact front generation 확인 뒤 clear와 구분되는 canvas content를 기다리고 PNG를 보존한다. 실제 Testbed/무텍스트 F0/dark autofocus의 캡처를 분리했다. Testbed Material sample과 dark 진단 gallery 캡처에서 해당 root content를 확인했다. 색상 수 검사는 일반적인 시각 정확성 oracle이 아니며 이번 fixture content gate에 한정한다.

`doroti.boot-evidence/v1` JSON은 run ID, revision/artifact, renderer/default 또는 explicit 선택, surface/context/session/scene, 요청 기록과 증거 수준을 담는다. document renderer에 없는 Worker session 값은 생략된다. main performance timeline만 사용하며 Worker의 raw clock을 서로 빼지 않는다. Windows validator는 framework scene + exact-present API와 자동 pointer/key/text/semantics를 기록한다. marker 목록은 충족한 단계의 분류이며 새 OS 시간 측정값이 아니다. Windows 세부 identity는 C9 manifest와 product report diagnostics에 있다.

브라우저 캡처는 physical scan-out이 아니다. `page.keyboard.insertText`로 한글 값과 ARIA mirror를 확인했지만 OS 한글 조합기/Narrator/VoiceOver를 검증한 것은 아니다. Windows 첫 content는 framework/Presentation API 증거이며 이번 작업에서 새 physical capture acceptance를 받지 않았다.

## P1. Web

1. manifest 이후 JS/WASM length/SHA-256 검증을 병렬 시작하고 둘 다 성공해야 Worker를 만든다. 한쪽 실패 시 peer fetch를 abort한다. origin/path/version/hash 검사를 유지했다.
2. UI Worker에서 CanvasKit 초기화와 `dotnet.js` import를 겹친다. import rejection을 보존하고 runtime.create/StartWorker는 UI text와 Raster ready 이후 한 번만 실행한다. 런타임 객체 생성 자체를 무조건 앞당기지 않았다.
3. Testbed/template은 같은 fallback font URL을 `as=fetch`, anonymous CORS로 preload한다. managed decode/register 및 resource ACK 소유권은 유지한다. Blazor loader의 fingerprinted href와 importmap은 유지하면서 기본 Worker에서 불필요한 preload를 제거했다.
4. API/type/loader contract/영문·한글 README에 `blazorOptions`가 document/Blazor 전용임을 명시했다. callback/Response를 Worker로 clone하지 않는다.
5. CanvasKit prepare는 동일 길이/hash 파일과 동일 manifest를 다시 쓰지 않는다. source와 최종 artifact integrity 검사는 계속 실행한다.

실제 request graph는 `browser/boot-persisted`와 `browser/boot-document-persisted`의 `boot-evidence.json`에 있다. localhost Python server, desktop Chromium 새 context, 압축 전송 설정 없음이다. Worker Testbed에서 Blazor loader 요청은 0건이었다. CanvasKit JS 120,877 bytes, WASM 7,317,345 bytes, fallback font 2,054,744 bytes다. 검증 fetch 이후 두 Worker의 JS/WASM URL load가 여전히 관측되었다. font는 preload와 후속 소비 요청이 같은 URL이다. Playwright의 cache 관련 `responseBodySize`에 음수가 들어오는 항목은 원문 그대로 보존했으며 이를 실제 wire 절감량으로 합산하지 않는다. document 경로의 실제 Blazor 요청은 별도 기록에서 확인한다.

**검토 후 보류:** verified bytes/compiled module 재사용은 pinned CanvasKit의 instance 소유권·transfer/compile/integrity 동등성을 확인하는 별도 변경이 필요하다. 두 CanvasKit instance는 UI text와 Raster 역할이 다르다. 이번에는 URL 요청/compile 중복이 제거됐다고 주장하지 않는다. 배포 Brotli/cache header와 repeat navigation의 전후 성능 비교도 `notVerified`다.

## P2. 공용 첫 장면 비용

MAUI hidden Entry/Editor는 factory로 바꾸었다. no-client에서는 두 관리 객체 및 이벤트 구독이 0개이며 첫 single-line에서 Entry만, multiline 전환에서 Editor를 만든다. 기존 eager internal constructor는 호환을 유지한다. focus/selection/hide/clear/suspend/resume와 생성된 객체의 unsubscribe/dispose를 검증했다. native handler의 플랫폼별 첫 focus 비용은 별도 gate다.

계획이 확인했던 legacy RootApp의 lazy theme와 별개로 현재 `MaterialSample/SampleApp.cs`에는 eager light/dark 필드가 남아 있었다. 실제 sample을 memoized theme factory로 전환하고 color 설정 변경 시 캐시를 무효화한다. FCR7과 실제 light sample/dark gallery를 검증했다.

**검토 후 유지:** typed manifest 직접 생성은 public JSON Load의 schema/RID/duplicate/plugin ABI 검증을 보존하면서 SDK와 boundary의 descriptor 소유권을 함께 바꿔야 한다. 현재 source-generated JSON context와 public load를 유지하며 두 manifest를 중복 관리하지 않는다. 공용 parsing 제거는 후속이다. 첫 semantics snapshot에 제거할 근거가 있는 동일 중복을 확인하지 못했으므로 접근성 활성화와 순서를 유지했다. DLR 전체 제거도 하지 않았다.

## P3. Windows

presenter를 native loading 검사 전에 선택한다. native C++ host import graph에 ANGLE link가 없음을 확인했고 normal Vulkan/D3D12에서는 ANGLE 파일 length/PE 검사를 하지 않는다. 명시적 ANGLE 및 full native audit는 여전히 해당 파일을 요구한다. provenance에 selected presenter/ANGLE inspection 여부를 추가했다. resolver/ABI/architecture 거부와 no app-local Vulkan loader/ICD 정책을 유지했다.

C9에 Vulkan without ANGLE의 opaque/Acrylic 성공과 full audit missing ANGLE 실패를 추가했다. 현재 backend 문자열은 `Vulkan/Composition-Swapchain`이다. adapter/device/Skia/Composition의 LUID·ownership 확인을 임의로 합치지 않았다. Acrylic은 요청 창에만 적용되고 Vulkan 실패에 ANGLE fallback을 도입하지 않았다. 명시적 ANGLE C9와 Windows MAUI Release publish/live frame+semantics는 통과했다. MAUI live는 evidence 확인 후 자신이 시작한 프로세스를 종료한 smoke이며 실제 text 조작/정상 종료는 미검증이다. 별도 D3D12 진단 assembly가 없는 product에서 해당 presenter를 선택하면 안내 오류로 fail-closed함을 확인했다. D3D12 정상 runtime과 물리 IME/Narrator는 `notVerified`다.

## P4. Target별 결과

| Target | Build/구조 | Runtime 관측 | 남은 gate |
| --- | --- | --- | --- |
| Web 기본 CanvasKit | Release publish/TypeScript PASS | F0/Testbed/dark autofocus content, 한글 값/ARIA, topology/display-list/resize/recovery PASS | physical IME/screen reader, mobile browser, 전후 성능 |
| Web document-webgl | 같은 published artifact | Testbed content 및 text/ARIA 회귀 PASS | 물리 입력, 배포 cache |
| Windows Vulkan opaque/Acrylic | Release C9 PASS | framework/exact-present API, 자동 입력/semantics, optional ANGLE probe PASS | physical scan-out, IME/Narrator, 기존 resize acceptance |
| Windows ANGLE / MAUI / D3D12 | ANGLE C9 PASS; MAUI publish PASS | ANGLE product probe, MAUI frame+semantics, D3D12 missing diagnostic 거부 PASS | MAUI 실제 입력, D3D12 정상 live |
| Android x64 | AOT off + trim on Release publish PASS | emulator install/cold/foreground/content/text client/warm 유지 확인 | physical device/정상 한글 조합기/TalkBack |
| Android arm64 | Release AOT(119 assemblies) publish, 현재 profile strict decode PASS | 실기기 연결 없음 | install/cold/warm/first input/ART 실제 상태 |
| iOS / Catalyst / AppKit | Windows에서 Apple gate 재시도하지 않음 | notVerified | Apple host build/sign/Metal/IME/VoiceOver |
| Linux Wayland / X11 | framework-dependent publish/Qt contract PASS | WSLg + explicit Mesa D3D12에서 각각 presentation API 4/3 frames, failures 0, semantics 46 | 실제 native Wayland/X11, content capture, 입력/IME/Orca |

Android 기존 profile은 현재 DEX checksum과 맞지 않아 strict decode가 실패했다. wrapper 이름만 현재 Testbed 이름으로 rebind하고 현재 APK 기준 binary pair를 다시 만들었다. 3,630 rules가 strict decode에서 정확히 보존됐고 현재 arm64/x64 DEX bytes가 동일하다. 원래 physical CUJ를 이번 emulator CUJ로 바꾸어 표기하지 않았다. ProfileInstaller 강제 install result 1과 `speed-profile` 강제 compile Success를 관측했다. 자연 설치 ART 상태와 구분한다. emulator snapshot은 30-byte empty profile이어서 채택하지 않았고 generator가 empty decoded rules를 거부하도록 고쳤다. 이전 binary/text와 strict failure를 보존했다.

Android `am start -W`의 마지막 OS TotalTime은 5,153ms이며 Doroti TTID/개선률이 아니다. 초기 잘못 추측한 activity launch는 adb exit 0이어도 Error type 3이므로 실패다. resolve-activity로 실제 `crc64c80c495bd333b69c.MainActivity`를 확인한 뒤 재실행했다. emulator 입력 캡처에는 Gboard 한글 배열을 통해 전달된 자모와 숫자가 보인다. `Boot123` literal 성공이나 물리 한글 IME 합격으로 처리하지 않는다. warm 실행은 기존 PID 유지 관측이며 모든 lifecycle 조합을 뜻하지 않는다.

Linux 초기 기본 GL은 llvmpipe여서 제품 정책대로 exit 68로 거부됐다. 성공 실행은 `GALLIUM_DRIVER=d3d12 MESA_LOADER_DRIVER_OVERRIDE=d3d12`를 명시한 AMD Radeon 780M WSLg 조건이다. `QT_QPA_PLATFORM=wayland`와 `xcb`를 따로 실행했다. X11은 WSLg의 XWayland이며 native X11 acceptance가 아니다. 종료를 위한 작은 1회 validation resize cycle을 사용했으며 대규모 resize matrix는 재실행하지 않았다.

## P5. CLI/SDK/template

- 확장자 whitelist를 제거하고 binary font/image/profile/native 입력을 content hash에 넣는다. dependency/generated directory는 재귀 진입 전에 제외한다. lock/pin과 상속 props/targets/SDK/NuGet 설정을 유지한다. 평가된 MSBuild item 및 ProjectReference graph를 추가해 workspace 밖의 실제 Testbed reference image도 포함한다. 외부 symlink 입력은 조용히 생략하지 않고 거부한다.
- launch-state v2는 runner/config/RID, source, SDK/workload 및 확인된 tool executable identity, 실제 TargetPath/TargetDir와 파일별 hash, intermediate static asset manifests 및 참조된 generated Web payload를 저장한다. v1/missing/tampered/stale 출력은 거부한다. 상태는 atomic replace하며 정상 build 성공 후 기록하고 앱 정상 종료와 구분한다.
- Qt CMake target을 공용 Runner SDK로 이동하고 Testbed/template 중복을 제거했다. configuration별 build 폴더와 CMake 자체 증분 의존성 검사를 사용한다. source/pin/toolchain 검증을 약화하는 timestamp-only target은 추가하지 않았다.
- TypeScript의 tsconfig/extends/declarations/toolchain/output 전체 graph를 대체할 근거가 없어 기존 compile을 유지했다. Android Gradle/Apple Xcode binding은 해당 platform target의 기존 build 소유권과 profile을 유지한다. Apple build 성능은 측정하지 않았다.
- 새 template clean build가 generated JSExport unsafe 오류를 드러내어 공용 Web Runner SDK에 `AllowUnsafeBlocks`를 추가했다. 검증 앱은 현재 source SDK/ProjectReference와 Web package props로 연결했다. 따라서 **템플릿 생성 + source 기반 build PASS**이며 새로운 NuGet release 설치 검증은 아니다. fixture wiring 상세는 evidence `BootTemplate/source-wiring.txt`다.
- 실제 CLI build/reuse는 Testbed와 새 template 둘 다 수행했다. template에서 source/toolchain/v1/missing target/tampered target/외부 generated loader 변조를 거부한다. 같은 길이+mtime binary 변경, inherited props, lock, dependency pruning 및 출력 metadata 5종 변조는 별도 contract로 검증했다. positive reuse는 HTTP-ready와 `build=reused` 확인 후 자신이 시작한 process tree를 종료하므로 exit -1이 기록된다. 이것은 앱 정상 종료 테스트가 아니다.

fingerprint/toolchain 검사는 이 환경에서 수 초가 들며 생략하지 않는다. broad framework root 때문에 unrelated platform 수정도 보수적으로 invalidate할 수 있다. 동적으로 발견하는 미선언 외부 build input과 PATH 밖 native SDK 전체를 완전 hermetic하게 봉인하는 cache는 아니다. 해당 target dependency 선언이 늘어나면 collector/identity를 함께 확장해야 한다. 이번 작업은 정확성을 높인 것이며 CLI/TTID 정량 개선을 주장하지 않는다.

## P6. 실행 로그와 보존한 실패

| Gate | 최종 증거 label | 판정 |
| --- | --- | --- |
| 공용 descriptor/negative/session | `app-descriptor`, `dynamic-dispatch` | PASS |
| scheduler/rendering/semantics/Material | `fcr3-scheduler`, `fcr4-retained-rendering`, `fcr6-semantics`, `fcr7-material-widget` | PASS |
| MAUI no-client/lifecycle | `host-maui-build`, `lazy-input-contract` | PASS, native handler acceptance 별도 |
| Web build/publish/types | `host-web-build`, `web-publish-final`, `playwright-typecheck-complete` | PASS |
| Web startup/failure/concurrency/input | `playwright-boot-gates` | 9 PASS |
| Web persisted content | `playwright-boot-persisted`, `playwright-document-persisted` | 3 + 1 PASS |
| Web plugin | `playwright-plugin` | JS host endpoint echo PASS; managed Worker channel 전체 roundtrip은 별도 |
| document input | `playwright-document-input` | 2 PASS |
| Windows deploy/negative | `c9-publish-final`, `c9-final.json` | C9 PASS |
| Windows MAUI / D3D12 | `windows-maui-publish`, `windows-maui-live`, `d3d12-missing-diagnostics-report.json` | MAUI API smoke PASS, D3D12 예상 실패 원문 보존 |
| Android current APK/profile | `android-arm64-publish`, `android-x64-profile-publish`, `profile-rebind-strict`, `profile-arm64-current-strict` | PASS |
| Linux | `linux-qt-contract`, `linux-publish-persistent`, `linux-wayland-d3d12`, `linux-x11-d3d12` | 위 한정 조건 PASS |
| CLI | `launch-identity-contract`, `evaluated-input-graph`, `cli-testbed-final-build`, `cli-testbed-positive`, `template-final-build`, `cli-reuse-final` | PASS |

이전 FAIL은 삭제하거나 PASS로 바꾸지 않았다.

- `playwright-boot-first`: 4 PASS / 2 FAIL. loader/front 이후 clear capture를 content로 검사하던 시점 오류와 ARIA mirror focus 기대 오류. 실제 content를 기다리고 focus owner `#doroti-ime`를 검증하도록 수정했다.
- `playwright-web-final`: 14 PASS / 1 FAIL(autofocus mirror). 해당 실행의 worker topology/resize/recovery/display-list PASS는 유효하며 최종 autofocus PASS는 별도 label이다. grep에서 제외된 headed text-field suite를 실행했다고 계산하지 않는다.
- `playwright-typecheck-final`: screenshot Buffer generic 오류. 명시적 Buffer 타입으로 수정 후 retry/complete PASS.
- `c9-publish`: Vulkan에서 ANGLE length를 계속 요구한 validator assertion FAIL. `c9-publish-retry`: obsolete effective presenter 문자열 FAIL. 원본 product logs를 남기고 현재 정책/정확한 backend로 assertion을 수정한 뒤 전체 C9 PASS.
- `android-profile-strict-x64`: 이전 DEX checksum mismatch FAIL. 현재 profile rebind와 strict PASS를 별도로 보존.
- `linux-publish`: isolated copy가 실제 외부 reference image를 누락해 FAIL. `linux-publish-retry`: WSL `/tmp` tmpfs가 distro 종료 시 사라져 FAIL. persistent `/home/parti/doroti-boot-20260907` source에 필요한 image를 포함한 publish PASS. 이 발견을 CLI evaluated dependency graph에 반영했다.
- `linux-wayland`/`linux-x11`: llvmpipe 거부 FAIL을 유지한다. explicit D3D12 조건의 PASS로 기본 조건 실패를 덮지 않는다.
- `template-clean-build`: unsafe 설정 누락 FAIL을 공용 SDK에서 수정. `template-build-retry`: source-wired fixture의 Web package target/props 누락 FAIL을 fixture 연결에서 수정. NuGet package regression으로 오분류하지 않는다.
- `cli-missing-output`: 원시 JSON에는 PASS이나 stale source에서 먼저 거부되어 missing-output 증거로 **무효**다. matcher를 원인별로 강화하고 fresh state의 `cli-missing-output-specific` 및 최종 전체 재검증만 missing-output PASS로 채택한다.
- `d3d12-missing-diagnostics`: raw exit 1 / report FAIL은 의도한 missing diagnostic assembly 조건이다. 정확한 `D3D12 is a separate diagnostic artifact` 메시지를 검증한 negative PASS를 별도 JSON에 기록한다. 정상 D3D12 실행으로 세지 않는다.

과거 full/partial Web AOT 실패, Web resize/latency FAIL, Windows Vulkan resize/physical gate, Windows 전체 solution의 Apple `sips` FAIL은 그대로다. 이번 변경은 renderer default, resize FPS, AOT 정책을 바꾸지 않았다.

## 남은 acceptance와 후속 설계

1. Apple host에서 iOS/Catalyst/AppKit 각각 Release build/sign/launch, Metal first content, suspend/resume, IME/VoiceOver.
2. arm64 실기기에서 현재 signed APK cold/warm/입력/ART 상태와 실제 CUJ profile 재수집. x64 emulator 결과로 대체하지 않는다.
3. Windows MAUI 실제 입력/D3D12 정상 live, Windows/Web/mobile 및 Linux의 물리 IME/screen reader/scan-out. Linux native Wayland/X11 input/content capture는 WSLg API smoke와 구분한다.
4. 같은 장치·Release·renderer·cache 조건의 전후 raw launch 비교. 현재는 performance `notVerified`이며 순차 대기/할당 감소를 시간 개선률로 환산하지 않는다.
5. Worker managed plugin 전체 roundtrip, verified CanvasKit bytes/module 재사용 및 typed manifest 생성은 별도 소유권/호환성 검증 후 수행한다.
