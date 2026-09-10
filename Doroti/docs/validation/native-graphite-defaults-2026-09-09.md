# Graphite 기본 구성 — 2026-09-09

사용자가 Graphite 기본값 채택과 Android·Linux·iOS·Windows MAUI의 추가 구성을 요청했다. 다음 호스트는 별도 활성화 플래그 없이 Graphite를 선택한다. 이 결정은 기존 성능·물리 표시·접근성 검증 결과를 변경하지 않는다. Linux·Apple 제품 실행 검증은 사용자의 명시적 요청으로 `skippedByUser`다.

| 호스트 | 기본 화면 경로 | 명시적 기존 경로 비교 |
| --- | --- | --- |
| Windows App SDK | Graphite → Vulkan → 기존 D3D11/Windows Presentation | `DOROTI_WINDOWS_GRAPHITE=0` |
| Windows MAUI | Graphite → Vulkan → 같은 LUID의 D3D12 공유 backing → CompositionDrawingSurface | `DOROTI_WINDOWS_MAUI_GRAPHITE=0` |
| Android MAUI | SurfaceView/ANativeWindow → Graphite/Vulkan → Vulkan swapchain | `DOROTI_ANDROID_GRAPHITE=0` |
| Linux Qt | Qt QWindow/QVulkanInstance → 관리되는 Graphite/Vulkan swapchain | native CMake `-DDOROTI_QT_GRAPHITE=OFF` 빌드와 `DOROTI_LINUX_GRAPHITE=0` 둘 다 필요 |
| macOS AppKit | 기존 MTKView/Metal queue → Graphite/Metal | `DOROTI_MACOS_GRAPHITE=0` |
| iOS / Mac Catalyst | MAUI 논리 view → 독립 MTKView/Metal queue → Graphite/Metal | `DOROTI_IOS_GRAPHITE=0` |

환경 변수를 제거하면 Graphite 기본값으로 돌아간다. 프로세스 실행 중 renderer/native module을 교체하지 않는다. Web의 Graphite/Dawn 기본값과 명시적 WebGL 선택은 유지한다. 자동 Ganesh/OpenGL/software fallback은 없다.

Android의 최소 API는 **24**, Vulkan 요구 버전은 **1.1**이다. APK manifest에도 Vulkan 요구 사항을 선언한다. Linux 역시 하드웨어 Vulkan 1.1 graphics/present queue가 필요하며 CPU ICD는 제품 장치 선택에서 거부한다. 과거 VMware SVGA3D OpenGL 가속 증거는 유효하지만 해당 환경의 CPU Vulkan을 제품 가속으로 인정하지 않는다.

## 구현과 배포

- 공통 `Doroti.Skia.Vulkan`은 Web 의존 그래프에 들어가지 않는 native host 전용 패키지다. 실제 enabled extensions/feature descriptor, 단일 owner, persistent Graphite target, acquire/copy fence, queue-family/layout 전환과 swapchain 수명을 관리한다.
- Android AOT는 관리되는 DllImport resolver를 우회할 수 있다. host AAR가 ABI 3 `libSkiaSharp.so`를 제공하고 runner의 최상위 NuGet 의존 그래프에서 stock native runtime asset을 제외한다. RID 중립 AAR는 arm64-v8a와 x86_64를 함께 구성하고 최종 APK가 선택한 ABI를 포함한다. 장치 파일 hash와 APK entry hash를 별도로 확인한다.
- Windows App SDK는 `graphite/libSkiaSharp.dll`, Windows MAUI는 같은 빌드의 `graphite/libDorotiGraphite.dll`, Linux는 `graphite/libDorotiGraphite.so`를 사용한다. `build-provenance.json`, SHA-256, LICENSE, THIRD-PARTY-NOTICES를 함께 배포한다. desktop loader는 ABI/RID/hash를 검사하고 stock Skia가 먼저 로드된 경우 혼용을 거부한다.
- Source build는 native asset을 launch-input/dependency 수집 전에 staging한다. Windows SDK·MAUI·Qt NuGet의 `buildTransitive`는 별도 `graphite` 폴더를 build/publish 산출물에 복사한다. Windows native DLL은 CLR 참조가 아니므로 custom asset 위치에 대한 NuGet `NU5100`만 해당 host 프로젝트에서 제외한다. 실제 package entry를 검사하며 라이브러리를 `lib/`의 managed reference로 분류하지 않는다.
- Linux native asset은 빌드 배포판의 glibc/libstdc++를 사용한다. 다른 Linux 배포판 호환성과 Linux 실행은 이번 검증 범위 밖이다. Apple은 고정 패키지의 public Graphite/Metal API를 사용하며 Vulkan ABI 3을 호출하지 않는다.
- Qt C ABI v2는 Vulkan surface 기능 bit 10과 120-byte surface descriptor를 협상한다. Qt가 생성한 instance/surface와 실제 활성 extension 목록을 빌려 쓰며 native 창이 파괴되기 전에 GPU와 관리되는 WSI를 해제한다. 기존 GL 비교 빌드는 bit 0을 사용한다. [Qt의 instance/extension/surface 계약](https://doc.qt.io/qt-6/qvulkaninstance.html)을 따른다.
- Windows MAUI는 shared D3D12 resource의 COMMON 상태와 Vulkan GENERAL/EXTERNAL 소유권을 fence 사이에서 전달한다. 동일 adapter LUID를 확인하며 [D3D12 shared heap 계약](https://learn.microsoft.com/en-us/windows/win32/direct3d12/shared-heaps)에 맞춰 GPU 복사로 합성한다.
- UIKit은 SKGLView를 MAUI 입력 계약으로만 유지한다. 실제 view는 MTKView이며 가짜 GRContext를 만들지 않는다. 초기 구현은 한 프레임을 동기적으로 drain한다. AppKit은 기존 비동기 완료 경로를 유지한다. 지연·처리량 최적화와 물리 표시 승인은 별도다.

## 확인한 실행

모든 build/test 실행에는 외부 **1,200초 timeout**을 적용했다. 실제 결과와 각 최초 실패는 [버전 관리되는 실행 기록](../../../history/26-09-09/native-graphite-default-execution.json)에 모은다.

| 항목 | 결과와 범위 |
| --- | --- |
| Windows App SDK 기본 실행 | NVIDIA·AMD 모두 Graphite 플래그/native override 없이 PASS. 각 device reset 2회·lifecycle 2회, terminal drain·정상 종료. 일반 출력 완료이며 물리 scan-out 검증은 아님 |
| Windows MAUI 제품 | 실제 Gallery·runtime shader/image filter 출력, resize·정상 종료. 후속 실행은 presented 9, replay 8, failed 0, superseded 7 |
| Windows MAUI 공유 texture | 두 GPU × 3개 크기 × 12프레임, 총 72프레임의 전체 RGBA 픽셀 검증 PASS. 취소 후 재사용 포함. validation layer를 요청한 재실행도 오류 출력 없이 PASS |
| Galaxy S25 제품 | signed Release/AOT APK 설치, Adreno 830의 기본 Graphite/Vulkan 화면·runtime shader/image filter 출력. 버튼/체크박스 터치, 삼성 키보드 표시·`Graphite47` 입력/삭제, background/foreground 2회와 context generation 3개. 후속 실행 presented 88, replay 32, failed 0, software fallback 0 |
| 공통 회귀 | runtime shader 계약(실제 `/viewId` backend 포함), FCR-7 Material/widget 전체 PASS |
| Qt 관리 코드 | Windows에서 managed cross-build와 ABI/keyboard/clipboard 계약 PASS. Linux native 실행 PASS를 뜻하지 않음 |
| Linux / Apple | 제품 native build/run 검증 `skippedByUser`. 소스·handler·기본값·target manifest·build/pack 구성 반영 |
| native 선택 실패/비교 | 잘못된 SHA-256 provenance는 기본 실행을 거부. 원본 복구 후 명시적 Ganesh 비교 실행 PASS |
| Windows publish | canonical publish PASS. 다른 디렉터리로 복사한 self-contained 앱이 Graphite 플래그/native override 없이 실행·종료 PASS, native failed terminal 0 |
| 패키지 | Windows App SDK/MAUI native 파일·provenance·license 포함 확인. 추출한 실제 buildTransitive target의 build/publish 복사 및 ABI 3 로드 PASS. Android AAR의 arm64-v8a/x86_64 native entry와 RID별 provenance 확인 |
| 최종 Android 재배포 | canonical Release/AOT build 및 성공 artifact 등록 PASS. 이후 USB 장치 부재로 재설치가 실패했으며 사용자가 **추가 기기 검증 생략**을 요청했다. 최종 APK 재실행은 `skippedByUser`, 직전 설치본의 기기 실행 PASS와 구분 |

Android 입력 검증은 자동 주입한 영문 입력과 화면/상태 확인이다. 한글 조합·물리 터치·TalkBack 전체 검증으로 확대하지 않는다. Windows의 superseded 프레임은 실제 더 최신 viewport 때문에 출력하지 않은 프레임이며 GPU 실패와 구분한다. 기존 synthetic resize/pixel/cadence 기록은 별도로 보존한다.

Windows App SDK NVIDIA 준비 프레임 receipt 지연 정책은 앞선 사용자 승인대로 **1,000ms**, 다른 GPU는 **50ms**다. 실제 matching present id/tag/display-instance 검증은 유지한다. 이 정책은 Windows MAUI의 Composition commit을 같은 receipt로 간주하지 않는다. [지연 허용 근거](../../../history/26-09-09/native-graphite-nvidia-latency-acceptance.json)를 참고한다.

## 보존한 최초 실패와 수정

1. Android AOT에서 stock `libSkiaSharp.so`와 다른 이름의 Graphite library가 섞여 SIGTRAP 발생. APK의 native asset을 ABI 3 하나로 통일했다. 이후 발견된 duplicate APK entry 경고도 최상위 package runtime 제외로 해결했다.
2. 실제 renderer의 `skiasharp-graphite-vulkan-gpu/1`이 runtime shader backend 목록에서 누락됐다. view별 접미사를 허용하고 해당 식별자를 회귀 계약에 추가했다. paint 실패로 이미 획득한 swapchain 이미지를 반납하지 못하는 경우에는 그 generation을 안전하게 retire하도록 수정했다.
3. Windows MAUI의 정상적인 stale viewport를 기존 코드가 “stale AppKit Metal” 실패로 기록했다. 해당 native completion만 superseded terminal로 연결했다. 실제 예외 경로는 failed를 유지하며 수정 전 failed 43 기록을 삭제하지 않았다.
4. 다중 TFM MAUI package의 바깥 평가에서 Windows native content가 빠지는 문제를 package 내용 검사로 발견해 수정했다. Android RID 중립 AAR의 두 ABI와 RID별 provenance도 구성했다.
5. 생성 전 asset 경로를 launch dependency가 먼저 읽던 문제를 staging target 순서로 수정했다. 실행 중 소스/의존 파일이 변경되어 canonical artifact 등록이 거부된 로그는 보존한다. 변경된 산출물을 성공한 canonical artifact로 취급하지 않는다.
6. 최종 Android build는 성공했으나 재배포 시 실제 USB/ADB 장치 목록이 비었다. bundletool `No connected devices found`와 직접 adb 설치의 device-not-found를 보존한다. 이후 사용자가 추가 기기 검증을 생략했다. 최종 APK에는 후속 surface callback 해제 보완, Android API 24 진단 문자열 및 두 ABI의 패키지 구성이 포함된다. 앞선 Gallery 입력·복귀 PASS는 그 직전 설치본의 결과다.

## 재현

```powershell
# 기본값에는 Graphite 활성화 플래그가 필요 없다.
pwsh -File Doroti/eng/doroti.ps1 build -App DorotiTestbedApp/DorotiTestbedApp.csproj -Platform windows
pwsh -File Doroti/eng/doroti.ps1 run -App DorotiTestbedApp/DorotiTestbedApp.csproj -Platform windows -NoBuild
pwsh -File Doroti/eng/doroti.ps1 build -App DorotiTestbedApp/DorotiTestbedApp.csproj -Platform windows -WindowsBackend Maui
pwsh -File Doroti/eng/doroti.ps1 build -App DorotiTestbedApp/DorotiTestbedApp.csproj -Platform android -Rid android-arm64
pwsh -File Doroti/eng/doroti.ps1 run -App DorotiTestbedApp/DorotiTestbedApp.csproj -Platform android -Rid android-arm64 -Device R3CY30KZA4B -NoBuild
```

NVIDIA/AMD를 직접 선택하는 Windows App SDK 비교에서는 `DOROTI_WINDOWS_VULKAN_DEVICE`를 사용한다. `DOROTI_WINDOWS_GRAPHITE_NATIVE`는 절대 경로의 ABI 3 진단 library를 선택하는 선택적 override이며 기본 실행에는 필요하지 않다. native build 준비와 asset 경로는 [native Graphite README](../../native/graphite/README.md)에 있다. 릴리스 서명·스토어 배포·전 OS 제품 검증은 이번 설정 변경의 PASS로 간주하지 않는다.
