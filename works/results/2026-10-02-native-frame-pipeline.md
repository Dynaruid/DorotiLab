# 네이티브 공통 C 연결 결과 — 2026-10-02

**구현 및 실행 가능한 자동 검증 완료. 전체 인수: PARTIAL.**
사용자 요청에 따라 iOS / Mac Catalyst / macOS의 빌드·Metal·기기 검증은
**SKIPPED**다. Apple 연결 코드는 구현했으며 과거 iOS payload의 통과 수치를
이번 후보에 소급하지 않았다. 실제 표시 성능, 물리 입력, 플랫폼별 10분 사용은
검증하지 못했다. `work4.md`는 수정하지 않았다.

## 구현

- UIKit에 의존하지 않는 admission, A/B/C 기본 옵션, immutable 준비 descriptor,
  terminal/retirement 계약을 `Doroti.Skia.Rendering`으로 추출했다. 미설정 C,
  shader-only 최대 2, native/shield/replay/resize 직렬 전환과 renderer의 원자적
  fresh 검사에 연결했다. Fast adaptive 기본 API와 커널은 유지했다.
- AppKit은 drawable/`_inFlight` 앞에서 준비하며 상한 3을 2로 변경했다.
  Catalyst의 준비·admission을 iOS 전용 조건 밖으로 연결했다. Apple 실행은 생략했다.
- Android는 유효한 Vulkan viewport를 게시하고 Choreographer callback을
  slot/acquire 앞에서 소비한다. pending 장면은 하나이며 GPU retry는 다음
  callback을 중복 소비하지 않는다. 정책 선택은 실제 Intent 전달 방식에 연결했다.
- Windows App SDK와 MAUI를 각각 수정했다. 기존 Vulkan device/queue를 유지하고
  private recording/copy bank 2개를 사용한다. producer 완료 뒤에도 D3D12의 마지막
  consumer fence가 완료되기 전까지 논리 슬롯과 공유 source를 보존한다.
- 기본 Qt Quick은 정상 프레임의 queue-idle/copy-fence CPU 대기를 producer와
  consumer fence 조회로 대체했다. Qt `afterFrameEnd` 뒤 같은 queue의 fence가
  sampling retirement를 증명한다. `beforeFrameBegin`에서 framework를 준비해
  QRhi admission 앞에 둔다. callback 재요청은 다음 token으로 보존한다.
  host ABI **6 / callback 208 bytes**, feature 21/22 및 offset 192/200을
  Sample2·Testbed·템플릿에 함께 적용했다. basic render loop의 소유 스레드를 유지했다.
  최종 검토에서 QRhi admission 실패 뒤 새 요청 없는 raster retry는 기존 준비
  token을 보존하도록 수정했다. 새 token이 있을 때만 supersede하며 중복 callback을
  소비하지 않는다. Quick native 후보를 재빌드하고 모드·native smoke를 재실행했다.
- Windows MAUI Sample2 실행 프로젝트와 CPU/GPU 회귀·플랫폼 수집 도구를 추가했다.
  CLI의 Windows 기본은 Windows App SDK, Linux 기본은 Qt Quick을 유지한다.
  [공통 계약과 비교 옵션](../../Doroti/docs/native-frame-pipeline.md)을 참고한다.

## P0: 준비·대기·완료 경계

| 제품 호스트 / 기존 backend | 준비와 admission | 정상 C에서 대체한 대기 | 남겨 둔 동기화와 자원 경계 |
| --- | --- | --- | --- |
| UIKit / Metal Graphite | UIKit 소유 pulse → immutable metrics → prepare → 실제 scene/native query → slot/drawable | 기존 C 준비 분리 유지 | Metal completion; native/replay/rotation 직렬·transaction |
| Catalyst / Metal Graphite | 공유 UIKit handler에서 prepare/admission을 공통 조건으로 이동 | 기존 pending gate 앞 prepare | Catalyst resize generation; Metal completion |
| AppKit / Metal | AppKit draw → context bootstrap → prepare → `_inFlight`/drawable | 정상 prepare가 이전 GPU에 종속되지 않음 | layout/native/replay 직렬; 완료 전 recording/drawable 보존 |
| Android / SurfaceView Vulkan | bootstrap/resize viewport → Choreographer 준비 → 실제 scene 및 fence count → nonblocking acquire | 일반 full slot/acquire 실패는 다음 pulse·completion polling으로 재시도 | Vulkan producer/copy fence와 SurfaceView/native 합성 receipt 분리; resize/종료 drain |
| Windows App SDK / Vulkan→D3D12 | native render task → managed BeginFrame → poll/query → private bank/output admission | 일반 C의 CPU Vulkan copy fence/출력 slot wait를 조회·8ms coalesced retry로 대체 | Vulkan queue signal→D3D queue wait; D3D final copy fence; native/정확 resize/종료 동기화 유지 |
| Windows MAUI / Vulkan→D3D12 composition | UI CompositionTarget 준비 → raster owner → scene query → bank admission | producer poll → deferred output copy/present → consumer poll | D3D allocator reuse 및 resize/ACK/EndDraw 유지; embedded composition/Ganesh 직렬 |
| Linux 기본 Quick / Qt-owned Vulkan | `beforeFrameBegin` 준비 → QRhi beginFrame → `beforeSynchronizing` raster/publication → Qt submit → `afterFrameEnd` marker | Begin의 sampling queue-idle와 Complete의 copy-fence CPU 대기 제거 | 같은 basic-loop queue 순서; producer fence 및 Qt consumer fence 모두 완료되어야 bank 재사용; teardown drain |
| Linux Vulkan window 대체 | native callback → BeginFrame → managed slot/acquire | fence 상태 조회 및 native timer retry | Vulkan fence; replay/native/resize 직렬. Quick sampling 결과와 별도 |
| Ganesh/OpenGL 대체 | 기존 context/surface 경로 | 두 프레임 능력 없음 | serial-backend fallback; C 설정만으로 C capability 통과하지 않음 |

Qt signal 선택은 [Qt 6.10 기본 render-loop 소스](https://raw.githubusercontent.com/qt/qtdeclarative/6.10/src/quick/scenegraph/qsgrenderloop.cpp)의
`beforeFrameBegin` → QRhi `beginFrame` → sync 순서에 근거한다. 소비 marker는
[QQuickWindow afterFrameEnd](https://doc.qt.io/qt-6/qquickwindow.html#afterFrameEnd) 뒤에
제출하며, [Vulkan queue-submit fence 계약](https://github.khronos.org/Vulkan-Site/refpages/latest/refpages/source/vkQueueSubmit.html)에
따라 같은 queue의 앞선 Qt 소비 작업을 포함한다. `frameSwapped`만으로 texture를
회수하지 않는다. producer/consumer 완료와 scanout은 별도다.

## 자동 검증

모든 테스트·빌드는 저장소 지침의 1,200초 wrapper를 사용했다.
기본 비교 반복을 수백 회 수행하지 않았다. 실패 실행은 raw 디렉터리에 보존했다.

| 범위 | 결과와 실제 증거 | 한계 |
| --- | --- | --- |
| 공통 CPU | `validate.py Build` PASS. full queue 준비, callback coalescing/native 삽입, raster wake, cap 2, viewport/context 변경, 중복/역순/실패 terminal, 최신 replay, legacy iOS 옵션 | CPU 계약이며 Metal 실행 결과가 아님 |
| Vulkan GPU 픽셀 | Radeon 780M 실제 GPU, **31조건 A/C byte-exact PASS**. Off; Full/Adaptive/Fast/Fixed σ0/1/2/4/8/20/32; Kawase σ20/32 | 96×80 shader fixture. 변환/실제 native 합성/플랫폼별 기기 픽셀 전체 행렬은 미검증 |
| 두 미완료 recording | 같은 device/queue/cache/recorder에 timeline semaphore로 GPU를 지연한 상태에서 red/green 2개 기록. CPU watchdog 미발동, pending 2, third 거절, A/C 독립 픽셀, consumer marker 모두 회수 | 실제 Qt sampling/DWM 출력이나 hardware execution overlap 측정은 아님 |
| Windows App SDK | Sample2/Testbed Debug+Release 빌드 PASS. 최종 Debug 및 Release native smoke 각각 API/native close/input/두 lifetime 정책 **5종 PASS** | 창·native control 자동화. 물리 IME, 실제 보이는 resize 품질, 다중 모니터 DPI 이동 미검증 |
| Windows MAUI | Sample2/Testbed Debug+Release 빌드 PASS. API/native 창 상태·resize·종료 PASS | native Editor/WebView adapter가 등록되지 않은 기존 capability: input probe **FAIL**, 새 adapter 구현은 별도 미완료 |
| Windows A/B/C | 두 Sample2 호스트에서 무설정 C, 명시 A/B/C PASS. C max 2, A/B ≤1. Release Off/Fast/Adaptive σ20·32 및 Full/Fixed/Kawase σ20·32 smoke PASS | submission receipt/queue 증거. 표시 FPS로 변환하지 않음 |
| Windows reset/loss | Release C에서 요청 reset 2, 완료 2, device generation 3, 이후 477 presented. 주입 DEVICE_LOST 후 실패 terminal 1, recovery 1, generation 2, 이후 662 presented; resize 중복/미종료 0 | 실제 물리 device removal이 아닌 기존 fault injection. 일반 zero-failure collector는 loss 실행을 FAIL로 보존, 별도 recovery 판정 |
| Linux Qt Quick | Testbed Debug, Sample2 Release 및 ABI 6 native build PASS. Wayland에서 다중 창 2정책, Desktop, native input 재생성, resize 20회 PASS. 종료 consumer submitted=completed, reserved/retiring 0 | WSL Ubuntu 26.04 / Qt 6.10.2 / llvmpipe software Vulkan |
| Linux 모드/대체 | Quick 무설정 C 및 A/B/C Fast20 PASS. Vulkan-window Release 빌드 및 default/A PASS. OpenGL xcb Off serial fallback PASS | Quick max 1 관찰: 실제 호스트에서 두 GPU 동시 frame을 입증한 결과는 아님. Vulkan-window 진단은 Quick 카운터 0이므로 queue 상한 실증 대신 공통 계약 검사를 사용 |
| Android | Sample2/Testbed Release x64·arm64 APK 빌드 PASS. x64 emulator 설치/실행, 축소 extent Fast20 default/A/B/C, native Editor/WebView 4회 생성 PASS | 실제 runtime **Mono full AOT**, API33 / SwiftShader. arm64는 빌드만, 실제 기기 미연결 |
| Android lifecycle | 3회 복귀 각각 5초 후 frame 증가 **56→71, 89→162, 180→217**. rotation 1102×540, cold recreate PID 변경, 이후 5초 frame 증가 및 screenshot PASS | 540×1170, DPR1.375. 최초 1080×2340 짧은 Fast probe는 frame receipt 0으로 FAIL 보존 |
| CLI/문서 | Source, runner-contract, installer-contract, Python compile PASS. native 3복제본 일치와 diff whitespace 검사 | clean-machine 배포/서명/전체 host package publish 인수는 미검증 |
| Apple | 코드 연결만 수행, 빌드/실행/Metal/기기 **SKIPPED** | 사용자 명시 요청 |

## 환경과 identity

- Windows: SDK 10.0.400, net10.0-windows10.0.19041.0 / win-x64,
  Windows 10.0.26300, Radeon 780M Vulkan 1.3.302. 설치된 대상 .NET runtime
  10.0.11; MAUI 증거 10.0.90 / SkiaSharp 4.154.0. primary display는
  EnumDisplaySettingsW 기준 2560×1600 / **165Hz**, 검사 앱 DPR2.
- Linux: WSL2 Ubuntu 26.04, SDK10.0.400 / runtime10.0.11, Qt6.10.2,
  Wayland WSLg, llvmpipe software Vulkan, DPR2. physical output Hz 미계측.
- Android: emulator-5554 / Pixel_5 API33 / Android13 / x86_64 / SwiftShader,
  nominal60Hz. 원래 1080×2340/DPR2.75와 축소 540×1170/DPR1.375를 구분한다.
  Release 속성은 UseMonoRuntime=true, RunAOTCompilation=true,
  AndroidEnableProfiledAot=false, PublishTrimmed=true,
  EmbedAssembliesIntoApk=true, target platform36.0이다. CoreCLR JIT 결과가 아니다.
  arm64 빌드의 Gradle SDK XML v3/v4 warning은 보존했으며 오류는 없었다.
- [플랫폼별 Windows JSON](2026-10-02-native-frame-pipeline-windows.json),
  [Linux JSON](2026-10-02-native-frame-pipeline-linux.json),
  [Android JSON](2026-10-02-native-frame-pipeline-android.json),
  [Apple 생략 JSON](2026-10-02-native-frame-pipeline-apple.json)에 source manifest,
  managed/native payload·APK SHA256, 검사 결과와 raw 경로를 보존한다.

## 실패와 남은 인수

PresentMon 2.6.0의 자체 ETW trace 시작이 **access denied**였다. 이 세션은
Performance Log Users/admin 권한을 갖지 않는다. 계정/권한을 변경하지 않았다.
따라서 실제 displayed FPS, p50/p95/p99, 긴 interval 비율, hardware GPU overlap,
input→scene→실제 표시 지연은 **notMeasured**다. 공식 도구도
[추적 권한과 display 측정 범위](https://github.com/GameTechDev/PresentMon/blob/main/README-ConsoleApplication.md)를
구분한다. render/present 수로 FPS를 만들지 않았으며, 표시 자료가 없는 30회
실행을 성능 PASS로 대체하지 않았다. 30회 수집 순서와 실제 display-event 입력
도구는 추가했지만 플랫폼별 정량 성능 인수는 미완료다.

raw 실패에는 Windows collector의 초기 close-owner 필터/field-name 오류,
Android 고해상도 짧은 probe, Linux `/mnt/c`의 FIFO/permission fixture 실패,
Windows/WSL 공유 obj restore 충돌, Qt summary opt-in 누락도 포함한다.
Linux fixture는 native Linux 임시 경로로, WSL 빌드는 isolated ArtifactsPath로
수정한 뒤 재검사했다. 처음 사용한 잘못된 loss selector(`device-lost`)는 recovery
0인 무효 probe로 보존하고 `DEVICE_LOST`의 실제 recovery와 구분했다.
OpenGL Wayland는 WSLg socket 초기 연결 실패 뒤, 재시도에서
`xdg_surface has never been configured` protocol error가 발생했다.
OpenGL xcb 직렬 통과로 Wayland 실패를 지우지 않는다.

남은 항목은 실제 Android hardware/손가락 입력·IME/selection, Windows 다중
monitor DPI 및 물리 입력, Linux hardware/IME/Orca, 플랫폼별 실제 GPU native
합성 픽셀 행렬, 최종 앱의 10분 사용과 성능 30회다. Windows MAUI native 입력
adapter와 OpenGL Wayland compatibility 실패도 남는다. Apple 검증 생략은
이 미완료 항목들과 별도의 사용자 요청이다.

원본은 `temp/testing/native-frame-pipeline/{windows,windows-maui,linux,android}/`
아래 보존했다. 결과 JSON의 raw manifest는 성공과 실패를 모두 포함한다.
Qt token 최종 수정 전/후 source 및 native payload manifest는 별도로 보존했다.
초기 collector의 apphost-only SHA는 전체 payload 식별값으로 사용하지 않는다.
수집 도구도 managed/native 파일과 runtime/deps manifest 전체를 hash하도록 수정했다.
기존 `validate.py`의 성공한 CPU/CLI suite raw build는 도구의 원래 정리 정책에
따라 자동 삭제됐으므로, 그 검사들은 세션 결과와 PASS 집계로 보존한다.
Windows의 마지막 pipeline snapshot은 Dispose 이전 상태일 수 있으며, 값이
남았다는 이유만으로 shutdown leak 또는 post-dispose 상태로 해석하지 않는다.
검사용 에뮬레이터의 extent/density와 rotation을 복구하고 해당 emulator만 종료했다.
재현 도구와 timeout 명령은 [검사 README](../../Doroti/tests/README.md)에 있다.
