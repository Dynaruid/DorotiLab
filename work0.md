# Graphite 유지 및 공식 SkiaSharp 바이너리 전환 작업계획

## 후속 변경: 프로젝트의 하드웨어 GPU 차단 제거

사용자 요청으로 장치가 하드웨어 GPU인지에 따른 실행 차단을 프로젝트 전체에서
제거했다. 공통 Vulkan, Windows Vulkan/ANGLE/D3D 및 native adapter 선택,
Qt OpenGL, WebGL/WebGPU가 필요한 API·버전·표시·상호운용 기능으로 장치를 판단한다.
소프트웨어 장치명·종류는 진단 정보로 남기며 GPU 성능 검증 결과와는 구분한다.
`DOROTI_LINUX_VULKAN_ALLOW_SOFTWARE`나 별도 Linux 실행 프로필은 필요 없다.
이하의 opt-in·hardware rejection 기록은 이 변경 전 검증 결과다.

Linux에서 원래 `dotnet run --project ./DorotiTestbedApp/linux/DorotiTestbedApp.Linux.csproj -c Release -r linux-x64`
명령의 실행·6회 resize 요청·종료를 확인했다. Web managed/TypeScript build 및
소프트웨어 WebGL2/WebGPU admission·미지원 API/잘못된 metrics 거부 검사를 통과했다.
Qt OpenGL 비교 경로의 native build도 통과했다. Windows managed 코드 빌드는 PRI 생성만
비활성화한 확인에서 통과했다. Windows 전체 빌드는 Linux에서 MakePri.exe 실행 불가로
제한되며 Windows native/제품 runtime은 미검증이다. 이 정책 변경이 기존 Material depth 동기화 오류를 해결한 것은 아니다.


## 추가 확인: llvmpipe Vulkan 사용 (2026-09-12)

사용자 요청에 따라 `DOROTI_LINUX_VULKAN_ALLOW_SOFTWARE=1`로 Linux Qt의 소프트웨어
Vulkan 검증을 허용했다. 공식 Graphite·공식 NativeAssets는 유지한다.

- 공통 session 360 frames, 2-slot copy/부분 갱신·픽셀 검사 3,000 frames,
  7초 큐 지연 retirement 3 generations를 llvmpipe에서 통과했다. 이 검사들의 validation 오류는 0이다.
- Wayland/XWayland 실제 제품의 표시 제출·6회 resize 요청·종료를 확인했고,
  Material 3 화면도 캡처했다. 제품에서는 depth attachment `WRITE_AFTER_WRITE`가
  각 5건 검출되어 correctness는 **FAIL / 전체 PARTIAL**이다.
- 고정 Skia 소스의 depth barrier가 early-fragment stage만 포함하는 점과 오류 로그가
  일치한다. 공식 내부 barrier 범위 부족이 유력하며, 오류를 숨기거나 커스텀 바이너리로
  바꾸지 않았다. 수정된 공식 자산 또는 검증된 공개 API 해결책이 남는다.
- llvmpipe 결과를 hardware GPU/성능/물리 입력 PASS로 확대하지 않는다.
  [실행 방법·증거·남은 조건](Doroti/docs/validation/linux-official-graphite-2026-09-12.md).

## 후속 검토: Linux Qt 구현·배포 검증 재개 (2026-09-12)

이번 사용자 요청으로 Linux 작업을 실제 검토·수정·검증했다. 아래의 과거
Linux `skippedByUser` 기록은 당시 범위이며, 현재 결과는 이 절과
[Linux 상세 보고서](Doroti/docs/validation/linux-official-graphite-2026-09-12.md)를 따른다.

- 생성 템플릿이 C++ `#ifdef`를 처리해 Graphite 코드를 삭제하던 배포 오류를
  수정했다. Linux native 파일은 원문 복사하며 생성 결과를 패키지와 byte 단위로 검사한다.
- Qt의 프레임별 동기 fence 대기를 2-slot 비동기 경로와 owner-thread idle polling으로
  교체했다. ABI surface 128 bytes, GPU polling callback을 포함한 callbacks 184 bytes,
  required features `0x1ffe`를 native/managed/template/계약 검사에 맞췄다.
- 공식 Linux 모듈의 실제 로드 경로·해시 기록을 보강하고, 지원하지 않는
  trimming/single-file 게시를 빌드 단계에서 명확히 거부한다.
- 테스트베드 Release 게시, managed 계약, 실제 생성된 package-only 앱의
  framework-dependent/self-contained 게시, NuGet 서명·공식 자산·loader 의존성,
  Wayland/XWayland native callback 검사를 통과했다.
- 현재 VMware에는 CPU Vulkan `llvmpipe`만 있다. Wayland/XWayland 제품은
  공식 자산을 로드한 뒤 하드웨어 Vulkan 부재로 exit 69를 반환했다.
  **실제 GPU 렌더링·동기화·지연 종료·성능은 notVerified이며 전체 상태는 PARTIAL**이다.
  native X11·물리 입력/IME/접근성·device loss도 남는다. Apple 검증 생략은 유지한다.

[원본 실패·명령·해시·결과 인덱스](Doroti/docs/validation/evidence/linux-official-graphite-2026-09-12/index.json).

## 후속 수정: Android 가로모드 방향 (2026-09-12)

Vulkan swapchain이 실제로 회전하지 않은 그림에 `currentTransform`을 적용했다고 선언하던 오류를 수정했다.
Android는 identity pre-transform과 실제 SurfaceView 크기를 사용하며, 회전 때문에 반환되는 사용 가능한
`SuboptimalKhr`로 매 프레임 swapchain을 다시 만들지 않는다. 실제 크기 변경·OutOfDate 재생성은 유지한다.
Galaxy arm64 자동 분할 Release 설치 후 사용자 물리 회전, 가로 표시, Color/Components 터치 전환을 확인했다.
회전 시 generation 1→2 이후 탭 전환 중 추가 재생성은 없었다. [원인·검증 범위·증거](Doroti/docs/validation/android-graphite-orientation-2026-09-12.md).

## 후속 수정: Android 자동 분할 APK 배포 (2026-09-12)

폰 확인 중 단일 APK는 실행되지만 자동 AAB/bundletool 분할 배포에서는 기본 APK만 검사하여 시작이 실패했다.
`SourceDir`와 `SplitSourceDirs` 전체에서 현재 ABI의 공식 라이브러리 한 개를 찾아 해시를 검사하고,
실제 로드 경로를 해당 분할 APK 또는 검증된 추출 파일과 대조하도록 수정했다.
13개 archive/path 회귀 사례와 Galaxy S25 arm64 Release 자동 분할 설치·Graphite 표시·cold 재실행을 통과했다.
[수정 및 검증 기록](Doroti/validation/android-graphite-apk/README.md). 전체 성능/플랫폼 qualification의 `PARTIAL` 상태는 유지한다.

작성일: 2026-09-11 · 검토 HEAD: `a6b30264bf5fe41cabdf4df67107ca906ce0e1df`

기준: 사용자의 **Graphite를 유지하면서 공식 SkiaSharp 바이너리만 사용하는 구조로 프로젝트 수정** 요구, [연구 결과](research/graphite-official-binaries/README.md), [자산·API 조사 증거](research/graphite-official-binaries/evidence.json).

이 문서는 **구현 작업계획 및 실행 상태**다. 독립 제출 관찰기·이미지 journal·R/P GPU copy와 공통 공식 Graphite session을 구현했고 Windows 제품 두 경로, Android arm64 실기기와 Web을 실행했다. **전체 상태는 PARTIAL / 플랫폼별 채택 검증 진행 중**이다. 남은 코드·기본 배포 전환·구 custom 자산 의존 제거·최종 Windows host consumer·Android APK 패키지 확인과 문서 인계를 수행했다. 전체 제품의 성능 수용은 미완료이며 Apple/Linux 검증은 사용자 요청으로 생략했다. 최초 5초 전체 회수 실패는 보존하며 현재 종료 계약은 아래 사용자의 위임에 따른 결정으로 변경했다.

초기 실험: [공식 자산](history/2026-09-12/validation/stock-graphite/2026-09-11/README.md), [비동기 회수 대안](history/2026-09-12/validation/stock-graphite/2026-09-11-retirement/README.md). 후속 구현과 제품 검증은 §10에 기록한다.

## 검증 반복 축소 (사용자 요청, 2026-09-12)

사용자가 테스트 시간이 길고 반복이 과도하다고 지적했다. 진행하던 장시간 성능 반복은 재시작하지 않는다. 이미 통과한 build/GPU/제품 검증을 재사용하며, 이후 검사는 실제 변경이나 실패 원인 확인에 필요한 범위만 수행한다. 구 비교 runner는 custom 경로와 함께 history에 보존했다. 현재 공식 기본값끼리를 baseline/candidate로 잘못 비교하지 않는다. 전체 성능 예산 통과를 선언하려면 별도 근거가 필요하며 이번 반복 축소를 기존 FAIL의 PASS 변경이나 성능 기준 면제로 해석하지 않는다.

완료된 60초 기준선/공식 경로 1쌍은 보존한다. 두 번째 기준선은 전경 창 이탈 방지 guard로 중단됐고 앱 자체는 exit 0으로 닫혔다. 그 결과와 사용자 피드백을 기록하고 반복을 종료했다. Apple/Linux의 명시적 `skippedByUser`와 성능 반복의 `incomplete/stoppedAfterUserFeedback`를 구분한다.

## 이번 실행에서 확정한 범위와 종료 계약 (2026-09-11~12)

- 사용자가 종료 기준의 판단을 위임했다. **논리적 close/admission 차단·창 숨김은 5초 이내**, GPU/native 자원 회수는 실제 완료 또는 실제 device loss를 확인한 뒤 수행한다. 5초 초과는 fault로 보고하며 device loss로 바꾸지 않는다. 미회수 generation은 최대 1개로 제한하고 해당 owner의 새 renderer 생성을 막는다. 영구 stall의 회수 시점은 보장하지 않으며, 이를 정상 종료로 기록하지 않는다.
- 앞선 전체 회수 5초 FAIL과 진단 결과는 역사적 증거로 보존한다. 아래 §8~9의 당시 제안/미승인 표현보다 **이번 사용자의 위임에 따른 결정이 우선**한다.
- 사용자 요청: “일단 애플,리눅스 검증은 스킵하고 코드 구성만 하자”. Apple(macOS/iOS/Mac Catalyst)·Linux는 코드/패키지 구성 범위이며 build/runtime/device/AOT 검증은 **skippedByUser**다. Windows·Android·Web 검증과 구분한다. 코드 구성 완료를 플랫폼 실행 PASS로 기록하지 않는다.
- 현재 독립 관찰기/제출 journal/R→P 복사·R 상태 복원과 공통 공식 session을 구현했다. Windows 두 host와 Android arm64/API 36 x64의 공식 경로를 제품에서 실행했다. 공식 기본값/배포/legacy 제거와 최종 consumer 작업은 §11에 기록한다. 전체 성능 수용은 진행하지 않은 범위로 남긴다.

## 1. 목표와 완료 조건

최종 제품은 SkiaSharp의 공식 NuGet 네이티브 자산으로 Graphite를 실행한다. Windows/Android/Linux의 `doroti_graphite_*` ABI 3 의존성을 제거하고, Apple의 Graphite/Metal 및 Web의 Graphite/Dawn 공식 API 경로는 유지한다.

완료 조건:

- 지원 대상으로 확정한 각 RID에서 공식 package identity·version·native asset과 최종 배포물의 실제 로드 자산이 일치한다.
- restore/build/publish/pack/run이 Skia 소스 checkout·patch·GN/Ninja 소스 빌드·커스텀 Skia staging 없이 동작한다. 기존 로컬 `artifacts/native-graphite`가 없어도 재현된다.
- Graphite가 실제 기본 렌더러다. Ganesh나 CPU 렌더링으로 자동 전환하여 통과시키지 않는다.
- 정상 프레임의 출력은 GPU 경로를 유지한다. CPU readback은 캡처·픽셀 검사에만 사용하고 presentation 경로에는 넣지 않는다.
- Vulkan 이미지 상태·메모리 의존성·큐 소유권·GPU 완료와 플랫폼 표시 완료를 구분하며, resize/취소/실패/종료에서 자원 수명을 보장한다.
- 이미지·텍스트·필터·runtime shader·입력·다중 surface 기능과 성능이 이 문서의 수용 기준을 충족한다.
- host뿐 아니라 runner, target packages, templates, 최종 APK/AAR/publish와 기존에 지원하던 trim/AOT 모드까지 공식 자산으로 재검증한다.

SkiaSharp 버전은 우선 현재 `4.154.0-preview.1.26454.9`를 유지한다. 버전 변경이 필요하면 공식 배포 여부, API·RID·자산·기능 차이를 기록하고 W0-0부터 관련 검증을 다시 수행한다. 공식 자산을 앱용으로 링크하는 과정과 Skia 소스를 빌드하는 과정은 구분한다. Doroti의 OS 연동 native helper, Android bridge, Web의 앱/WASM 링크까지 없애는 작업은 아니다.

## 2. 범위와 다른 계획의 관계

[work1.md](work1.md)의 플랫폼 뷰 합성과 [work2.md](work2.md)의 WebView 구현은 별도 작업이다. W0는 renderer/native dependency 경계를 바꾸며, 해당 계획의 native identity, paint order, alpha, 입력, surface generation, 수명 계약을 보존한다.

- 이미 구현된 플랫폼 뷰·다중 raster surface는 회귀 검증한다. 아직 미구현인 플랫폼 뷰/WebView 기능을 W0에서 새로 완성하지 않는다.
- platform view 담당 코드가 의존하는 GPU surface/retirement 계약이 바뀌면 W0-6에서 adapter를 제공하고 두 계획의 후속 작업에 인계한다.
- 같은 파일을 수정하기 전 현재 작업 상태를 재확인하고 기존 사용자 변경을 보존한다. baseline 이후 HEAD나 renderer 코드가 바뀌면 영향을 받은 증거를 갱신한다.
- 단일 surface 실험의 성공을 다중 segment 합성·전체 플랫폼 지원으로 확대하지 않는다.

## 3. 채택할 구조와 불변식

우선 후보는 **공식 Graphite API + 호스트 소유 일반 Vulkan 중간 이미지 R + 별도 출력 이미지 P + 같은 큐의 GPU 복사·상태 복원 + 공개 Vulkan 호출 관찰기**다. W0-1~W0-5의 scoped correctness 증거를 바탕으로 구현한 현재 기본 구조다. 전체 기능·성능 qualification과는 구분한다.

프레임 순서:

1. 공개 `SKGraphiteContext.CreateVulkan`과 persistent backend texture로 R에 기록하고 `InsertRecording` / `Submit(Sync=false)`를 호출한다.
2. `GetProcedureAddress` adapter가 관찰한 명령을 실제 성공한 queue submission 순서로 해석하여 R의 예정 최종 상태 L을 얻는다.
3. 같은 큐의 호스트 command buffer가 Graphite 쓰기→복사 읽기 의존성을 설정하고 R을 transfer source로 전환하여 P에 GPU 복사한다.
4. R은 **복사 직전 L과 같은 소유권 상태로 복원**한다. P는 WSI 또는 D3D의 출력 상태로 handoff한다.
5. host fence 완료 뒤 Graphite completion polling과 frame 회수를 수행한다. P는 플랫폼의 사용 종료 조건에 따라 별도로 회수한다.

필수 불변식:

- 단일 generation의 queue 접근을 한 owner가 직렬화한다. 동일 queue의 제출 순서만으로 메모리 동기화가 해결된다고 가정하지 않는다.
- R을 직접 D3D/WSI 외부 소유권과 주고받지 않는다. 첫 후보는 단일 color subresource, output sample count 1, non-alias 이미지로 제한한다. Skia 내부 MSAA/resolve가 나타나는 경우도 관찰·검증해야 한다.
- R의 다음 Graphite 사용 시 GPU 상태가 Skia가 마지막으로 추적한 상태와 같아야 한다. `COLOR_ATTACHMENT_OPTIMAL` 고정 추정, 매 프레임 `UNDEFINED` 재wrap, 전체 redraw 강제로 상태 불일치를 숨기지 않는다.
- observer는 드라이버 호출을 정확히 전달한다. Skia 메모리 offset, private C++ symbol, DLL patch, 위조한 feature/성공/error 결과에 의존하지 않는다.
- callback은 데이터 포인터를 빌려 쓰는 시간 안에 필요한 값만 복사한다. ABI·delegate lifetime·재진입·device별 dispatch를 보장하고 managed 예외를 native 경계 밖으로 내보내지 않는다.
- 기록된 명령, 성공적으로 제출된 명령, GPU 완료, presentation 완료를 별도 상태로 유지한다. 취소되거나 제출되지 않은 명령으로 실제 이미지 상태를 갱신하지 않는다.
- 일반 timeout을 device loss로 바꾸지 않는다. 실제 외부 loss를 관찰했다고 공개 `IsDeviceLost`가 즉시 같은 값을 가진다고 가정하지 않는다.

## 4. 수정 대상 지도

| 영역 | 현재 파일·설정 | 계획 |
|---|---|---|
| 공통 Vulkan binding | [SkiaGraphiteOfficialVulkanInterop.cs](Doroti/src/Doroti.Skia.Rendering/SkiaGraphiteOfficialVulkanInterop.cs) | 공개 context 생성, ABI 3 symbol/private constructor 의존 제거 |
| session·cache·frame | [SkiaGraphiteSession.cs](Doroti/src/Doroti.Skia.Rendering/SkiaGraphiteSession.cs), [Vulkan partial](Doroti/src/Doroti.Skia.Rendering/SkiaGraphiteSession.Vulkan.cs) | 관찰 상태·host completion 계약으로 연결, 기존 recording/upload 취소 수명 유지 |
| 공통 Vulkan host | [GraphiteVulkanWindow.cs](Doroti/src/Doroti.Skia.Vulkan/GraphiteVulkanWindow.cs), [Frames](Doroti/src/Doroti.Skia.Vulkan/GraphiteVulkanWindow.Frames.cs), [D3D12](Doroti/src/Doroti.Skia.Vulkan/GraphiteVulkanWindow.D3D12.cs) | R/P 분리, layout 복원, acquire/copy/fence/retirement 구현 |
| native 선택 | [GraphiteNativeLibrary.cs](Doroti/src/Doroti.Skia.Vulkan/GraphiteNativeLibrary.cs) | 공식 loader/asset identity로 교체, 커스텀 이름과 ABI 3 강제 제거 |
| Windows App SDK | [Graphite presenter](Doroti/src/Doroti.Host.WindowsAppSdk/WindowsManagedVulkanPresenter.Graphite.cs), [host project](Doroti/src/Doroti.Host.WindowsAppSdk/Doroti.Host.WindowsAppSdk.csproj) | D3D11/Windows presentation 계약 보존, 공식 asset 경로 연결 |
| Windows MAUI | [WindowsCompositionSurfacePresenter.cs](Doroti/src/Doroti.Host.Maui/WindowsCompositionSurfacePresenter.cs), [host project](Doroti/src/Doroti.Host.Maui/Doroti.Host.Maui.csproj) | D3D12 공유 output과 Graphite R 분리, 기존 adapter LUID·alpha·completion 보존 |
| Android·Qt | [MAUI host](Doroti/src/Doroti.Host.Maui/Doroti.Host.Maui.csproj), [Qt host](Doroti/src/Doroti.Host.Qt/Doroti.Host.Qt.csproj) | 공식 `.so`, WSI/수명/feature 제한 검증 및 제품 연결 |
| Apple·Web | MAUI Metal handlers, [Web Graphite](Doroti/src/Doroti.Host.Web/DorotiWebWorkerSurface.Graphite.cs) | 공통 session 변경 회귀와 공식 자산 배포 확인 |
| 배포·runner | host/target `buildTransitive`, [Sdk.targets](Doroti/src/Doroti.Runner.Sdk/Sdk/Sdk.targets), [launch-inputs](Doroti/eng/launch-inputs.targets), templates | stock asset 제외 설정·커스텀 복사·staging/launch 입력 제거 및 최종 consumer 확인 |
| 자산 버전 | [Directory.Packages.props](Doroti/Directory.Packages.props) | managed/native 버전 및 RID 일치 유지 |

초기 실험은 [Doroti/validation/stock-graphite-vulkan/](Doroti/validation/stock-graphite-vulkan/README.md)에 구현했다. host project를 참조하지 않으며 실제 resolved graph와 MSBuild preprocess에서도 `EnsureDorotiGraphite*Asset` target·custom staging 의존성이 없음을 확인했다. 공식 SkiaSharp·필요한 Vulkan binding만 참조한다. 공통 observer source는 제품으로 이동했고 독립 하네스는 동일 파일만 link한다. host/custom staging project를 참조하지 않는다.

## 5. 단계와 의존성

| 단계 | 내용 | 선행 단계 | 상태 |
|---|---|---|---|
| W0-0 | 기준선·지원 RID·자산·수용 예산 확정 | 없음 | PARTIAL — 공식 archive/서명/해시/기준선 제품 수정·실행, 비교 성능 baseline 미완료 |
| W0-1 | 공식 바이너리 독립 Graphite 실험 | W0-0 | PASS — Windows AMD/NVIDIA, 실제 Vulkan 1.2 profile |
| W0-2 | dispatch·명령 journal·이미지 상태 관찰 | W0-1 | PASS-scoped — 13 journal 사례, 실제 render pass/MSAA/secondary/sync2 관찰; 미지원 profile 거부 |
| W0-3 | GPU copy·상태 복원·비동기 회수 | W0-2 | PASS-scoped — 두 GPU × 4 모드 × 3세대, 8,160 frames, 1,000-frame 고정 크기/부분 갱신 포함 |
| W0-4 | 취소·실패·device loss·종료 | W0-3 | PARTIAL — 회수 18세대·실제 창 7초 GPU 지연 close 통과; 영구 stall/실제 loss는 미실험 |
| W0-5 | 핵심 후보 채택 판정 | W0-0~4 | Vulkan 1.2 scoped correctness에 근거해 공식 기본 코드 경로 채택; 전체 성능 qualification은 별도 PARTIAL |
| W0-6 | 공통 session·loader·host 계약 | W0-5 | 구현 — public API binding, 제출 상태 검증, desktop/APK provenance |
| W0-7 | Windows App SDK·MAUI | W0-6 | PARTIAL — AppSDK 두 GPU resize/Acrylic/지연 close, MAUI 실제 화면·close PASS; 전체 기능/성능 미완료 |
| W0-8 | Android·Linux Qt | W0-6 | Android arm64 실기기·API 36 x64 emulator 제품 PASS-scoped; API 33 emulator 필수 RP2 부재 FAIL 보존. Linux build/package/template/Qt ABI 및 llvmpipe headless PASS-scoped; 제품 depth sync FAIL, hardware GPU 미검증 |
| W0-9 | Apple·Web | W0-6 | Apple 공식 package/Metal 구성, 검증 skippedByUser. Web Release 및 제품 10 tests PASS |
| W0-10 | 기능·성능·trim/AOT 수용 | W0-7~9 | PARTIAL — Android Release/trim/Mono AOT·제품 입력/수명, Web 10 tests PASS. Windows MAUI 60초 1쌍에서 성능 증가 관측; 반복은 사용자 피드백 후 종료 |
| W0-11 | 공식 배포·custom 빌드 제거 | 해당 W0-10 | 코드/배포 전환 완료 — 공식 기본값, custom prebuild/staging/ABI 제거·history 보존, Windows package consumer 및 Android APK 자산 확인. Linux clean template/공식 자산/publish PASS-scoped, hardware runtime 미검증. Apple은 skippedByUser |
| W0-12 | clean consumer·문서·종결 | W0-11 | 문서/consumer 구현 완료 — 저장소 밖 Windows target package restore/publish/제품 실행, Android package-only APK, README·지원표·work1/work2 인계. 전체 qualification은 PARTIAL |

W0-1~4의 통과는 명시된 profile/진단 경계에 한정된다. 실제 제품 통합 결과를 별도로 기록하며 필수 기능·성능 미검증을 PASS로 확대하지 않는다. Apple 검증 생략은 유지한다. Linux는 문서 맨 앞의 후속 검토 결과를 따른다.

제품별 작업은 공통 gate 이후 독립적으로 진행할 수 있다. 한 플랫폼이 통과해도 공통 package 소비자가 아직 구 ABI를 요구하면 해당 의존성을 먼저 분리하고, 모든 필수 소비자 전환 전 전역 제거를 하지 않는다.

### W0-0. 기준선과 지원 범위 확정

**작업:** 현재 HEAD/dirty tree, package pin, 실제 product startup 경로, custom exports 호출자, prebuild/pack/launch 입력을 목록화한다. 기존 커스텀 경로의 현재 빌드·제품 실행 가능 여부와 실패를 먼저 기록한다. 연구 증거 원본은 보존하고 재조회 결과는 새 timestamp 디렉터리에 저장한다.

RID 목록은 실제 target package/runner에서 산출한다. 최소 확인 대상은 Windows x64 두 host, Android arm64/x64, Linux Qt x64, AppKit macOS·iOS·Mac Catalyst의 실제 지원 RID, browser-wasm이다. upstream가 지원하는 모든 RID를 Doroti 지원 대상으로 임의 확장하지 않는다. 기기·OS·SDK·GPU·Vulkan loader·validation layer 버전을 기록한다.

공식 restore source와 archive integrity/signature 검증 방법, asset hash·export·backend capability 수집, 깨끗한 consumer 경로를 확정한다. optional device feature 비활성화 후보가 제품의 렌더링 기능·shader 요구를 충족하는지 점검한다. WSI/external-memory 확장을 Graphite가 몰라도 되는지는 미검증 가설로 남긴다.

**통과:** 지원/검증 matrix, 실제 baseline 실패 목록, 비교 장면, §6의 수치 예산이 기록된다. baseline이 깨진 플랫폼은 이유를 분리하고 source/build만으로 runtime baseline을 대신하지 않는다. 기준선 부재가 후보 correctness 검증을 막지는 않지만 성능 gate 통과를 선언할 수는 없다.

### W0-1. 공식 자산만으로 context 생성과 기본 그리기

**작업:** 독립 하네스가 NuGet 공식 DLL을 절대 경로 또는 검증된 표준 loader 경로로 로드하도록 구성한다. Skia 관련 native build target과 제품 host 참조가 없어야 한다. Vulkan instance/device/queue를 만들고 공개 CreateVulkan·recorder·surface·insert·submit만 사용한다. callback rooting과 모듈 수명을 검증한다.

단색 외에 텍스트, 서로 다른 배경 위 이미지, alpha, gradient, clip, runtime shader와 image filter를 포함한다. readback은 결과 검사에 한정하고, `Sync=true` 초기 진단과 `Sync=false` 후보 실행 결과를 구분한다.

**통과:** 공식 자산 hash·실제 로드 경로·Graphite backend, enabled feature/extension descriptor, context 생성·픽셀 결과·정상 해제 증거가 남는다. custom exports·Ganesh·CPU 렌더러를 사용하지 않는다. context null/unsupported shader/validation 오류를 우회하지 않는다.

### W0-2. Vulkan 관찰기와 상태 journal 구현

**작업:** `GetProcedureAddress` adapter를 통해 실제 호출을 전달하면서 명령별 상태를 수집한다. instance/device/queue/command buffer별 routing, generation, native callback ABI를 명시한다. 완료되지 않은 native callback보다 delegate와 metadata를 먼저 해제하지 않는다.

추적 대상은 image/view/framebuffer/render pass 생성·해제, barrier와 `*2/KHR` 변형, Begin/Next/EndRenderPass, command buffer/pool reset·free, secondary execution, QueueSubmit/Submit2 결과다. dynamic rendering·imageless framebuffer·alias·추가 subresource 등의 지원 여부를 명시한다. 지원하지 않는 경로는 탐지한 상태로 거부하며 정상 성공으로 계산하지 않는다. 제품 필수 장면이 그 경로를 요구하면 구현 범위를 넓히거나 후보를 중단한다.

command buffer별 journal을 실제 제출 순서에 적용한다. record callback 순서, unsubmitted/cancelled buffer, 실패한 submit의 layout을 전역 실제 상태로 확정하지 않는다. host copy와 상태 복원의 journal도 같은 generation 계약에 반영한다. 알려지지 않은 opcode/상태를 만났을 때는 오류를 native 경계 밖으로 던지지 않고, 안전한 host 경계에서 출력·재사용을 차단한다.

**통과:** 명시적/암묵적 전환, MSAA resolve/subpass, 부분 repaint, image filter/readback 후 layout, buffer reset·재기록·handle 재사용·제출 순서 변경에서 독립 기대값 및 Vulkan validation과 일치한다. 전체 화면을 단색으로 채운 결과만으로 통과하지 않는다. 관찰 metadata가 자원 수명과 함께 회수되고 무제한 증가하지 않는다.

기존 ABI 3 getter를 oracle로 사용하는 선택적 교차 검사는 기존 보존 바이너리의 별도 프로세스에서만 수행한다. 이 결과는 공식 바이너리 실행 증거가 아니다. 새 Skia 빌드를 oracle 준비의 필수 조건으로 만들지 않는다.

### W0-3. GPU copy·복원·동기화 구현

**작업:** 먼저 Windows x64 headless에서 일반 R과 별도 P 사이의 GPU roundtrip을 구현한다. Graphite render→host barrier→copy→R 상태 복원→host fence를 같은 queue에 제출한다. memory availability/visibility, stage/access 범위, family ownership, subresource 범위를 구체적인 barrier 표로 남긴다.

1-slot으로 시작한 뒤 기존 host 수준의 bounded multi-slot을 검증한다. `Sync=false`, nonblocking fence polling, fence 완료 이후 `CheckAsyncWorkCompletion`과 frame retirement를 사용한다. slot 포화 시 무제한 큐잉이나 매 프레임 `DeviceWaitIdle`로 우회하지 않는다. 실패 후 R의 복원을 입증할 수 없으면 해당 generation을 정상 상태로 재사용하지 않는다.

**통과:** 지원 가능한 각 Windows GPU에서 3 context generations와 generation별 최소 120회 반복 copy·복원, 픽셀·상태·자원 lifetime 확인, Vulkan validation 및 synchronization validation 오류/경고 0. 서로 다른 배경과 부분 갱신을 포함한다. GPU 진단 통과이며 화면 표시 통과가 아님을 기록한다. 실제 검증하지 못한 GPU는 별도 `notVerified`다.

### W0-4. 취소·실패·device loss·종료

**작업:** snap/insert/submit/copy 전후의 취소·실패, acquire 후 paint 취소, readback callback pending, timeout, 외부 host가 먼저 관찰한 device loss, close/resize 중 제출을 검사한다. `recorded → submitted → GPU-complete → platform-retired` 상태별 소유 자원과 실패 회수 규칙을 명시한다.

정상 종료는 새 admission을 닫고 owner를 join한 뒤 확인된 GPU/플랫폼 종료 경계에서 자원을 해제한다. 외부 loss는 host generation의 영구 fault로 기록하며 private native 통지 없이 실제 API 결과로 정리가 가능한지 검증한다. 공개 polling만으로 Skia 내부 lost 플래그가 갱신된다고 가정하지 않는다. native destructor의 내부 대기가 유한 종료 요건을 깨는지도 검사한다.

**통과:** 정상·취소·실패 경로에서 double release/use-after-free/stale reuse/pending semaphore 누수가 없고, host shutdown deadline을 지킨다. deadline은 기존 제품 계약을 W0-0에서 기록하고, 계약이 없으면 초기 5초로 두어 실험한다. timeout 시 검증 프로세스를 외부에서 종료한 결과는 제품의 정상 종료 PASS가 아니다. 주입한 loss 처리 통과와 실제 driver/device-loss 복구는 별도 결과로 남긴다. 실기기 강제 GPU reset은 기본 검증에 포함하지 않는다.

### W0-5. 후보 채택 gate

**작업:** W0-1~W0-4의 공식 자산 독립 실행, shader/feature 제약, 관찰기 coverage와 비용, 종료·수명 보장을 함께 검토하여 `decision.json`에 채택/추가 조사/불채택 및 근거를 기록한다.

**통과:** 필요한 기능을 공식 API와 표준 Vulkan adapter만으로 제공하고 핵심 correctness·종료 gate를 만족한다. 이 통과가 제품별 W0-10 성능 검증을 대신하지 않는다.

**중단 기준:** 필수 상태를 추적할 수 없음, 필요한 feature descriptor를 안전하게 전달/제한할 수 없음, private ABI 또는 fake Vulkan 결과가 필요함, 무기한 대기·불명확한 회수, GPU-only 출력 불가. 실패한 경우 제품 전환·기존 빌드 제거를 진행하지 않는다. 증거와 필요한 upstream API/공식 release 조건을 기록하고 관련 없는 플랫폼 조사·문서 등 가능한 작업만 이어간다. Ganesh 전환·CPU presentation·사내 커스텀 Skia NuGet을 성공 대안으로 선택하지 않는다.

### W0-6. 공통 session·loader 통합

**작업:** 검증된 구현을 `Doroti.Skia.Vulkan`/`Doroti.Skia.Rendering`의 기존 책임 경계에 통합한다. 구 ABI binding 대신 공개 context 생성과 typed host state/retirement 계약을 사용한다. framework/widget 계층에 Vulkan 타입을 추가 노출하지 않는다.

공식 library의 backend availability와 package/RID identity를 검증하고 여러 경로에서 다른 Skia를 로드하지 않도록 한다. 기존 custom resolver를 안전하게 제거하거나 공식 경로 검증으로 대체한다. 프로세스 안에서 renderer/native module을 갈아끼우지 않는다. 개발 비교는 별도 프로세스·출력 디렉터리로 분리한다.

recorder ownership, budget, bounded frame 수, cancelled-recording upload cache, image/filter/picture cache, readback callback, 세대 변경 계약을 보존한다. 실패한 recording의 업로드를 committed cache로 재사용하지 않는다. shared session API 변화가 Metal·Web과 기존 platform raster segment에 미치는 영향을 점검한다.

**통과:** 공통 API를 소비하는 각 host가 새 계약으로 빌드되고, 의미 있는 rendering/cache/lifetime 검증을 통과한다. 공식 후보 경로에서 `doroti_graphite_*`와 private constructor 참조가 없다. 아직 전환 전인 host의 개발 비교 경로는 명시적으로 격리한다.

### W0-7. Windows 두 host 전환

**작업:** Windows App SDK의 D3D11/Windows presentation과 Windows MAUI의 D3D12/Composition 경로를 각각 연결한다. 정확한 GPU adapter LUID, external-memory 포맷·usage·import, 외부 소유권 acquire/release, D3D fence/공유 handle 수명을 확인한다. MAUI는 R/P 분리로 생기는 추가 GPU copy 비용을 계측한다.

opaque/alpha/Acrylic, resize/DPI, 최소화·복구, 두 창과 close/reopen, 입력·IME, 기존 화면 합성 경계를 검사한다. `work1`에서 실제 구현된 raster/native/raster 합성은 identity·paint order·alpha를 함께 확인한다. WSI direct window 성공으로 Windows Composition 경로를 대체하지 않는다.

**통과:** 두 host 각각 공식 DLL의 실제 로드 증거, 표시된 제품 화면, GPU/플랫폼별 retirement, 입력·수명 결과를 남긴다. NVIDIA/AMD 등 W0-0 matrix의 각 GPU 검증을 분리한다. 한 host의 성공을 다른 host나 물리 표시 latency 증거로 확대하지 않는다.

### W0-8. Android·Linux Qt 전환

**작업:** Android/Qt의 기존 R→swapchain copy에 observer와 상태 복원을 연결한다. acquire는 P를 사용하는 제출에서 기다리고 present-ready semaphore를 swapchain image 기준으로 관리한다. submit fence 완료를 presentation wait 소비 완료로 오인하지 않는다. split graphics/present family는 지원하거나 생성 전에 명확히 거부하고 지원 matrix에 제한을 기록한다.

Android는 공식 arm64/x64 `.so`, AAR/APK 중복 native library, AOT의 resolver 우회, SurfaceView/ANativeWindow 생성·파괴, rotation/resize, background/resume, 키보드/input/scroll을 검사한다. Linux는 Qt의 실제 instance extensions/surface owner, X11/Wayland, loader/fontconfig/glibc 등 공식 자산의 실행 의존성과 package baseline을 확인한다.

**통과:** Android x64 emulator와 arm64 실기기의 fresh launch·텍스트 입력·화면 표시·foreground/background·close/reopen 결과를 각각 남긴다. Linux X11/Wayland도 source/build/실행 결과를 분리한다. 직접 빌드 Linux `.so`를 유지해야만 실행되는 상태는 공식 바이너리 전환 완료가 아니다.

### W0-9. Apple·Web 공식 경로 회귀

**작업:** AppKit/iOS/Catalyst는 공개 Graphite Metal·동일 Metal queue·drawable 완료 경로를 유지하면서 W0-6의 공통 변경을 반영한다. 실제 지원 RID별 공식 asset과 최종 bundle을 확인한다. Web은 worker-owned Graphite Dawn/WebGPU, 공식 WASM asset, resize/DPR, device loss, async submit/retirement를 검증한다.

**통과:** 플랫폼별 제품 실행·렌더링·수명·최종 자산 증거가 남고 커스텀 Vulkan ABI를 요구하지 않는다. macOS 성공을 iOS/Catalyst로 이전하지 않는다. Web의 공식 `.a` 앱 링크는 허용하되 Skia source build가 아님을 build log로 확인한다. 미사용 Metal/Dawn 코드가 빌드된 것만으로 runtime 통과하지 않는다.

### W0-10. 제품 기능·성능·trim/AOT 수용

**작업:** §6의 동일 조건 baseline/candidate 비교를 수행한다. 텍스트·한글 IME/caret·selection, image decode/upload, filters/shadows/blend, runtime effects, Material scroll/animation, resize/재생성, 다중 surface/창을 검증한다. 기존 제품이 사용하는 이미지 cache와 부분 갱신을 실제로 켠 상태를 포함한다.

Vulkan callbacks/dispatch metadata/정적 rooting/실제 loader 경로를 플랫폼의 Release/trim/AOT 모드에서 검사한다. NativeAOT는 실제 ILC/native link·최종 설치물·기능 검증이 있을 때만 그렇게 기록한다. Android Mono AOT 또는 interpreter 성공은 별도 모드다.

**통과:** 각 플랫폼의 필수 기능·성능 예산이 충족되고 regression 원인과 잔여 범위가 명시된다. stock 경로 통과를 위해 필수 widget 효과·부분 갱신·다중 surface를 끄지 않는다. unavailable 장치는 `notVerified`로 남기며 해당 플랫폼 runtime gate를 완료 처리하지 않는다.

### W0-11. 공식 배포 전환과 커스텀 Skia 빌드 제거

**작업:** 통과한 host의 공식 경로를 기본값으로 전환한다. Windows App SDK/MAUI·Qt prebuild의 `build-windows.py --ensure-distribution`, `stage-android.py`, `stage-linux.py` 호출과 커스텀 native distribution content/pack/launch dependency를 제거한다. Qt의 커스텀 파일 존재 기반 pack gate는 공식 자산 검증으로 교체한다.

Android는 host `.csproj`, `Doroti.Host.Maui.targets`, runner `Sdk.targets`의 stock NativeAssets 제외와 custom EmbeddedNativeLibrary를 함께 정리한다. resolved asset graph와 최종 APK/AAR에서 ABI별 공식 `.so` 하나만 들어가는지 검사한다. consumer가 정식 NativeAssets를 가져오는 경로를 실제 project/package 참조 모드 모두에서 검증한다.

host/target `buildTransitive`, central package pin과 runner override, templates, launch input/identity, native resolver·진단 메시지·provenance schema를 동기화한다. 공식 package identity·RID·hash 검증은 유지하며 ABI 3 필수 검사만 없앤다. notices/license를 보존한다.

구 custom-only validation의 유지/폐기 목록을 만든다. 제품·활성 build graph·새 검증에서 custom bridge가 더 이상 필요하지 않을 때 retired script/bridge/계획을 `history/<실제 날짜>/`에 결과·실패·미완료·증거 링크와 함께 보존하고 활성 경로에서 제거한다. 사용자의 NuGet 캐시나 기존 native artifacts를 재귀 삭제하는 작업은 요구하지 않는다.

**통과:** 실제 resolved dependency, `.nupkg`, publish/bundle/APK/AAR, 새 앱 template의 공식 asset provenance와 runtime load가 일치한다. active product/build/package/launcher에 custom source/staging/export 의존성이 없으며, 보존된 history·비교 증거의 문자열은 허용한다.

### W0-12. 최종 clean consumer와 종결

**작업:** 기존 로컬 Skia source/distribution/cache에 기대지 않는 별도 clean checkout 또는 package-only consumer에서 restore→build→pack/publish→install/launch를 검증한다. Skia source 폴더를 생성하지 않으며 Skia용 GN/Ninja/clang 실행이 없음을 build log·프로세스/입력 증거로 확인한다. 다른 Doroti native helper 컴파일과 구별한다.

최종 README·한국어 README·host/target 지원표와 사용 명령을 갱신한다. research는 당시 조사 증거로 보존하고 실제 채택 결과를 링크한다. work1/work2에는 필요한 renderer 계약 변경과 검증 범위만 인계한다.

**통과:** 모든 필수 platform/RID gate와 최종 consumer가 통과하고 공개 기본 경로가 공식 Graphite 자산만 사용한다. 남은 플랫폼 미검증·기능 축소·외부 release 의존이 있으면 전체 상태는 `PARTIAL`이다. 문서만 완성하거나 Windows만 성공했다고 전체 완료로 바꾸지 않는다.

## 6. 검증 장면·성능 예산

baseline과 candidate는 같은 OS/GPU/driver, 앱 commit(렌더러 변경 외 동일), 해상도·DPR·refresh rate, Release 설정으로 비교한다. validation layer를 켠 correctness 실행과 끈 performance 실행을 구분한다. warmup 10초 후 60초 장면을 최소 3회 수행하고 반복별 값·중앙값을 보존한다. 입력 장면은 같은 자동화와 실제 기기 조작으로 구분한다.

| 항목 | 초기 수용 기준 |
|---|---|
| 시각·기능 | baseline과 동일 fixture의 픽셀/기능 조건 충족. 색 공간·alpha·AA 허용 오차는 W0-0에서 fixture별 수치로 기록 |
| CPU/GPU frame time | 동일 장면 반복 중앙값 기준 p95/p99가 baseline 대비 10% 이상 악화하지 않음. 기존 제품 절대 예산이 더 엄격하면 그 기준도 충족 |
| missed/dropped frame | baseline보다 1 percentage point 넘게 증가하지 않음. display/Choreographer/host 각각 실제 측정 경계 명시 |
| steady-state presentation | CPU readback/upload 경로 0, 무제한 pending queue 0, 매 프레임 DeviceWaitIdle/QueueWaitIdle 우회 0 |
| 메모리 | R/P 추가 이미지의 실제 allocation 크기와 slot/segment 수에 따른 증가를 설명. 고정 크기 1,000-frame 실행 후 frame별 누적 증가 없음. observer metadata 상한과 cache budget 수치 기록 |
| cold start | 동일 조건 최소 3회 fresh launch의 중앙값이 baseline보다 10% 이상 악화하지 않음. 최초 shader/cache 생성 결과는 warm 결과와 분리 |
| GPU 정확성 | Vulkan 및 synchronization validation 오류/경고 0. 환경 경고가 있다면 원문·원인·범위를 남기고 해당 gate는 해결 전 PASS하지 않음 |

위 값은 이 계획의 **초기 기준**이며 기존에 측정·합의된 성능 결과가 아니다. W0-0에서 장치별 baseline과 실제 제품 계약을 연결해 확정한다. 이후 기준을 변경하면 변경 이유·측정값·영향을 계획에 기록하고, 실패 결과를 삭제하지 않는다. observer 시간, 추가 queue submit, GPU copy, allocation, shader cache 비용을 분리하여 regression 원인을 설명한다.

## 7. 증거·실행 규칙

실행 결과는 `Doroti/artifacts/stock-graphite/<timestamp>/<stage>/<platform-rid>/`에 저장하고, 요약·source/package identity·명령·exit code·raw log 링크는 version-controlled 결과 문서에 기록한다. 경로는 구현 시 생성할 제안이다. 하나의 report에는 최소 다음을 포함한다.

- stage/status, commit/dirty state, timestamp, command·timeout·exit code.
- OS/RID/device/GPU/driver, renderer/backend, package version, source/asset SHA-256, 실제 loaded module 경로 또는 최종 static-linked asset provenance.
- feature/extension descriptor, observer coverage, context/frame/generation/slot 수, 제출·GPU 완료·platform retirement 수치와 불균형.
- pixel·sync/lifetime·성능 결과, 실패 원문, `automated`/`emulator`/`product`/`physical` 구분, `notVerified` 항목.

[저장소 지침](.github/copilot-instructions.md)에 따라 **모든 테스트는 20분(1,200초) 외부 timeout**을 적용한다. 기본 wrapper는 [run-with-timeout.py](Doroti/validation/run-with-timeout.py)다. 개별 fence/종료 deadline은 이 외부 timeout보다 짧은 제품 계약으로 별도 관리한다.

```powershell
# 이미 존재하는 연구용 자산 검사. 제품/GPU gate를 대신하지 않는다.
python Doroti/validation/run-with-timeout.py python research/graphite-official-binaries/inspect-stock.py
```

새 하네스의 build/run 명령은 생성 후 실제 확인한 명령만 단계 결과에 기록한다. 종료되지 않은 프로세스, 빌드 성공, headless pixel 성공, emulator 실행을 각각 제품/물리 검증 성공으로 확대하지 않는다. 기기 부재·미실행은 `notVerified`이며 사용자 명시 생략이 없으면 `skippedByUser`로 기록하지 않는다.

## 8. 2026-09-11 실행 결과와 다음 시작점

- 공식 NuGet archive 재다운로드·Microsoft/NuGet.org 서명 검증·설치 자산 대조 통과. 실제 로드 DLL hash 일치.
- AMD 780M / NVIDIA RTX 4060에서 독립 `Sync=true`/`Sync=false` 기본 장면 총 4개 실행 PASS. 각 2-frame의 텍스트·이미지·alpha·gradient·clip·runtime shader·blur 픽셀/coverage 확인. Vulkan/sync validation 메시지 0.
- 같은 큐를 정상 timeline semaphore로 지연시킨 종료 subtest에서 양쪽 모두 5초 시점 `NotReady`, `IsDeviceLost=false`. 공개 `Dispose()`는 native `vkWaitForFences(UINT64_MAX)`에 들어가 실제 host signal을 약 7초 기다렸다. 전체 shutdown 약 12초로 **5초 예산 FAIL**. 실제 GPU reset/loss 실험이나 제품 정상 종료 PASS가 아니다.
- 기존 custom D3D12 진단은 두 GPU에서 PASS. 기존 Windows App SDK 제품 fixture는 renderer 시작 전 `DorotiApplicationBoundary.CreatePlatformViewRegistry:122`의 `NullReferenceException`으로 FAIL. 후보 렌더러 실패와 별도 기준선 문제다.
- target manifest 11개, 공식 NativeAssets package 7개의 cached native entry 43개를 조사했다. Android arm64 실기기는 연결되어 있으나 앱 runtime 미검증, Linux는 WSL 환경만 확인, Apple/Web 제품과 AOT/최종 consumer는 미검증이다.

첫 실험 이후의 문제는 **보통 GPU timeout에서 안전한 owner join·자원 회수·유한 종료를 만족하는 구조 또는 공식 upstream lifecycle API를 확보/검증**하는 것이다. 후속 §9에서 응답과 회수를 분리한 구조를 실험했으므로 독립 W0-2/3 조사는 이어갈 수 있다. 당시 제품 채택을 보류했다. 현재는 앞 절의 수정된 종료 계약과 후속 제품 검증 결과를 적용한다. 성능 수용을 위해 기존 제품 fixture 오류와 플랫폼 baseline도 해결해야 한다. 당시 제품 전환과 custom 빌드 제거는 미완료였다. §11에서 수행한 전환 결과와 원본 실패를 구분한다.

## 9. 추가 대안: 닫기 응답과 GPU retirement 분리

`Program.Retirement.cs`와 `run-retirement.py`에 세 가지 독립 시나리오를 구현했다.

- **retire-ready:** admission을 닫고 제어 스레드에 응답한 뒤 terminal fence 완료 → callback polling → child/context/device 해제.
- **retire-delayed:** 같은 큐의 실제 producer를 7초 지연한다. 5초에는 `FaultedHold`로 두고 GPU 자원을 보존한다. 실제 완료 후 원래 owner가 해제한다. 미완료 동안 generation/새 frame은 거부한다.
- **retire-cancel:** 준비되지 않은 호스트 입력은 `InsertRecording` 및 해당 입력 wait를 제출하기 전에 취소한다. 기존 WSI acquire나 외부 GPU semaphore를 임의로 signal하는 방식이 아니다.

AMD/NVIDIA × 3개 시나리오 × 3 context generations = 18세대 통과했다. validation/sync validation 메시지 0, 완료 후 Dispose의 `vkWaitForFences` 호출 0, 최대 live generation 1, 최종 0이다. 각 세대에서 close 이후 generation/frame admission을 각각 1,000회 거부했다. 지연 시나리오에서는 pending readback callback이 실제 GPU 완료 뒤 안전하게 회수됐다.

제어 스레드의 논리적 닫기 요청 응답은 최대 **0.4252ms**, GPU 완료 후 context Dispose는 최대 **2.3213ms**였다. 정상/제출 전 취소의 실제 native 회수는 약 24~46ms, 지연 시나리오는 약 7.03~7.06초였다. 이는 headless 수명 진단이며 실제 창 제거·표시 latency·제품 성능 수치가 아니다.

**판단 수정:** 앞선 실험의 producer는 Dispose 진입 7초 뒤에 해제되도록 구성되어 있었다. 그 결과는 미완료 context의 직접 Dispose에 대한 FAIL로 보존하되 모든 공식 Graphite host 구조의 실패로 확대하지 않는다. 새 대안은 Dispose 호출을 GPU 완료 이후로 미루고, 제어 응답을 먼저 돌려줄 수 있음을 보여준다.

**당시 예산 경계:** 이 실험 시점에는 새 제안이었다. 이후 사용자가 판단을 위임해 앞 절의 응답/회수 분리 계약을 채택했다. 과거 “모든 자원 회수/owner join 5초” 실패 자체는 바꾸지 않는다. 영구 stall에서는 보존된 한 generation의 회수 시점을 보장할 수 없고 새 renderer 생성도 막힌다. 실제 창 detach, close/reopen, 플랫폼 output retirement, device loss, owner join은 추가 검증 대상이다.

다음 독립 작업은 이 수명 계약을 사용하는 W0-2 이미지 journal과 W0-3 R/P GPU copy다. 제품 통합 전에 실제 플랫폼 수명과 전체 종료 예산을 함께 해결해야 한다. 영구 native stall의 격리가 필수라면 별도 renderer 프로세스를 추가 후보로 조사할 수 있으나, 강제 종료는 정상 GPU 종료 PASS로 기록하지 않는다.


## 10. 후속 구현·제품 결과 (2026-09-12)

[실행 결과와 미완료 경계](history/2026-09-12/validation/stock-graphite/2026-09-12/README.md), [명령/해시/원본 증거 인덱스](history/2026-09-12/validation/stock-graphite/2026-09-12/evidence.json)를 기록했다. 공식 observer/session, Windows 두 제품 경로, Android arm64 및 API 36 x64, Web, 공통 package-only consumer까지 진행했다.

Windows MAUI의 60초 비교 1쌍에서 CPU/native raster/submit·GPU wait p95 증가가 관측됐다. 당시 반복은 사용자 피드백 후 중단했고 전체 성능 PASS를 선언하지 않았다. 이후 Windows 기본 배포 코드 전환은 §11에서 수행했으며, 이를 성능 승인으로 해석하지 않는다. 원래 실패, 수정 뒤 통과, 사용자 생략, 미완료 항목은 결과 문서에서 구분한다.


## 11. 남은 코드·배포 전환 처리 (2026-09-12)

[최종 전환·지원표](Doroti/docs/validation/official-graphite-cutover-2026-09-12.md), [이번 증거](history/2026-09-12/validation/stock-graphite/2026-09-12-cutover/README.md)를 추가했다.

- Windows App SDK/MAUI 기본값을 표준 공식 NativeAssets DLL로 통일했다. Android/Qt·Apple·Web의 공식 package pin, host/target/runner/template 구성과 provenance metadata를 맞췄다. Windows override manifest는 진단용이며 정상 실행에 필요하지 않다.
- 공통 private ABI binding, custom prebuild/staging, 커스텀 DLL pack/launch 항목을 제거했다. 구 bridge/빌드/검증 runner와 전환 전 source를 [history](history/2026-09-12/work0-custom-graphite/README.md)에 보존했다. native artifacts/사용자 캐시는 삭제하지 않았다. `doroti_graphite_interop_version` 문자열은 커스텀 자산을 거부하는 negative guard에만 남는다.
- Windows target의 helper DLL이 WinUI PRI에 잘못 포함되던 NuGet 소비 오류를 수정했다. 별도 디렉터리·새 캐시·PackageReference만으로 실제 host restore/publish/default resize/Acrylic/종료를 통과했다.
- MAUI 종료에서 UI의 동기 join과 Composition completion이 서로 기다리던 경로를 비동기 선회수로 바꿨다. 새 frame admission을 닫고 창을 숨긴 뒤 dispatcher를 유지하며 worker/commit을 회수한다. 실패/timeout은 보존하고 강제 exit 0을 제거했다. 수정 후 창 숨김 약 12ms, 프로세스 종료 약 304ms였다.
- R/P copy barrier를 제출 전후 각각 한 호출로 묶고 observer의 rooted delegate를 재사용했다. 변경 경계의 D3D12 readback/cancel/크기 변경 72 frames만 검사했다. 이는 성능 향상 증명이 아니다.
- Android NuGet host를 별도 consumer에서 사용해 arm64/x64 APK를 만들고 ABI별 공식 `.so` 한 개와 해시를 확인했다. 기존 실기기/에뮬레이터 입력·lifecycle 증거를 재사용했다. 이번 APK는 배포 검사이며 추가 NativeAOT/runtime PASS가 아니다.
- README·한국어 README·지원표·사용법·notices/license·work1/work2 인계를 갱신했다. 공식 NuGet license와 native third-party notices 원문을 패키지에 보존했다.

**남겨 둔 검증 경계:** Apple/Linux build/runtime/AOT는 `skippedByUser`. Windows MAUI 성능 1쌍의 예산 초과 관찰과 반복 중단은 그대로 남는다. 전체 DPI/다중 창/장치·실제 device loss/물리 표시/NativeAOT·별도 생성 template runtime 검증을 이번 짧은 확인으로 대신하지 않는다. 코드·배포 작업의 완료와 전체 qualification의 `PARTIAL`을 분리한다. 추가 장시간 반복을 자동으로 시작하지 않는다.
