# 검증 기록 — 2026-10-06

| 범위 | 결과 | 확인 내용 |
| --- | --- | --- |
| 공통 앱/패키지 Release 컴파일 | PASS | 경고 0, 오류 0 |
| Windows App SDK Release 빌드 | PASS | 경고 0, 오류 0 |
| Windows 네이티브 시작·합성 입력·종료 | PASS | Graphite/Vulkan/D3D12/DXGI, `VisibleAfterExactPresent=true`, presented terminals 98, failed terminals 0, device loss 0, exit 0 |
| Web Debug 빌드 | PASS | TypeScript/WASM 포함, 경고 0, 오류 0 |
| Chrome Web 화면 | PASS | splash 해제 후 홈의 이미지·텍스트, Cover Slider 진입 및 메뉴 이미지 표시 |
| 캐러셀 회귀 | PASS | 루프 경계, 음수 인덱스, 최단 경로, 세 depth order, 선택/정지 콜백, 탭 hit testing, drag/fling, wheel, 역방향, 크기 변경, 경계 제한, 항목 수 변경, 컨트롤러 교체·해제, 단일 항목 |
| 샘플 회귀 | PASS | 다섯 데모 왕복 이동, Previous/Next, 음식 상세 보기, 토핑 추가/제거, 덱 셔플, 560×850 및 390×740 CPU 캡처 |
| 연속 입력 회귀 | PASS | 휠 burst 누적/한 번 정지, 마우스·트랙패드의 루프 경계, 세로 휠의 가로 이동과 역방향, 큰 휠 보간, 버튼/트랙패드 중단, unmount 타이머 취소 등 9개 |
| 첫 프레임·이미지 연속성 회귀 | PASS | 큰 휠의 첫 입력 프레임/진행 중 지연, 선택 전환 중 이미지 픽셀 및 위젯 수명, 홈 블러·덱 뒤집기의 이미지 경계 유지 등 4개 |
| Windows 입력 재개·프레임 제출 회귀 | PASS | native 휠 24개, 큰/작은 delta의 두 burst, 시작부터 움직인 장면 제출까지 53.49ms / 11.10ms, 입력 때문에 완료 프레임 표시 취소 0회 |
| Windows 시작 지연 사용자 재확인 | 사용자 확인 | 수정 전 실제 입력으로 지연 재현, 수정 후 같은 방식의 트랙패드/휠 재개에서 "시작 지연이 해결됐어요" 응답 |
| 공통 application dispatcher 회귀 | PASS | ThreadPool 포화 중 UI 깨우기/네이티브 완료, 큐 포화, 취소·실행 drain·재진입·종료 등 3개 검사 그룹 |
| Windows 연속 입력 반응 사용자 재확인 | 사용자 확인 | 아래 추가 수정 후 "움직이는 동안도 부드러워졌어요" 응답; OS 스레드 우선순위 변경 없음 |
| 워크스페이스 | PASS | 일곱 플랫폼 별칭, runner/ProjectReference 경로, XML, Windows provider describe |
| Android/iOS/macOS/Mac Catalyst/Linux 네이티브 빌드·실행 | notVerified | runner 소스와 메타데이터 구성만 확인 |
| Web Release AOT publish | notVerified | Debug Web 빌드·브라우저 실행만 확인 |
| 전체 물리 입력 수락·접근성·display scanout·FPS·배포 서명 | notVerified | 시작 지연에 대한 사용자 확인은 전체 입력/표시 성능 수락과 별도 |

저장소 루트에서 회귀 검사를 다시 실행할 수 있습니다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project packages/Doroti.CustomCarousel/tests/Doroti.CustomCarousel.Tests -c Release -- temp/testing/carousel
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build samples/DorotiCarouselApp/windowsappsdk/DorotiCarouselApp.WindowsAppSdk.csproj -c Release
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build samples/DorotiCarouselApp/web/DorotiCarouselApp.Web.csproj -c Debug
```

실행 당시 PNG, 네이티브 로그와 JSON은 `temp/testing/carousel/`에 생성했습니다.
이 경로는 삭제 가능한 로컬 산출물입니다. 이 문서는 확인한 결과와 범위를 보존하며 원시 파일의 영구 보존을 뜻하지 않습니다.
Windows 시작 검사는 `DOROTI_WINDOWS_APPSDK_SMOKE_MS=6000`으로 첫 화면 후 자동 종료했습니다.
입력 수정 후에는 `DOROTI_WINDOWS_APPSDK_INPUT_SMOKE=1`도 지정했습니다. 이 검사는 native window message 합성이며 물리 입력이 아닙니다.
Chrome 확인은 개발 서버를 실행하고 재빌드 후 서버를 다시 시작한 최신 Debug 산출물에서 수행했습니다.

원본의 스크롤·배치 계약을 옮겼으며 `flutter_animate` 전체와 픽셀 동등성을 검증하지 않았습니다.
shimmer/tint, Hero와 지갑 거래 목록은 샘플 구현 범위에서 제외했습니다.

## 연속 입력 수정

Windows 앱의 휠·트랙패드·드래그 문제를 진행 중 프레임을 포함한 회귀 검사로 재현했습니다.
기존 검사에서는 입력 종료 후 선택된 항목만 검사하여 입력 도중의 되감김을 잡지 못했습니다.

- 수정 전: 30px 휠 입력을 48ms 간격으로 12회 보냈을 때 양의 입력 중 한 프레임에서 약 4.96px 뒤로 이동했습니다.
  입력마다 스냅을 시작하고 start/end 콜백을 반복한 것이 원인이었습니다.
- 수정 후: 같은 burst에서 위치가 단조롭게 360px까지 누적되고, 입력 종료 후 항목 1에 한 번 정지합니다.
  작은 휠 입력은 즉시 반영하고, 큰 입력은 누적 목적지로 보간한 뒤 마지막 입력 후 120ms에 스냅합니다.
  큰 입력의 보간은 아래 추가 수정의 지속 틱커 방식을 사용합니다.
- 수정 전: 루프 경계를 넘는 30px 마우스/트랙패드 이동이 3,170px의 scroll delta로 보고됐습니다.
- 수정 후: 입력 중 내부 좌표를 연속으로 유지하고 표시·선택만 순환시켜 -30px씩 보고합니다.
  루프에는 무한 스크롤 범위를 사용하고 정지할 때 내부 좌표를 정규화합니다.
- 가로 캐러셀의 세로 휠 fallback은 내부 Scrollable이 입력을 처리하지 않은 경우에만 동작합니다.
  `verticalWheelScroll: false`로 비활성화할 수 있습니다.

세부 입력 검사만 실행할 수 있습니다. 동일한 9개 검사는 기본 회귀 명령에도 포함됩니다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project packages/Doroti.CustomCarousel/tests/Doroti.CustomCarousel.Tests -c Release -- --continuous-input temp/testing/carousel-input
```

수정 전/후 raw JSON은 실행 당시 `temp/testing/carousel-input/before/`와 `after/`에 저장했습니다.
CPU 회귀와 Windows 합성 입력의 PASS는 실제 트랙패드 체감, 물리 입력이나 FPS 수락을 대신하지 않습니다.

## 남은 지연과 카드 깜빡임에 대한 추가 수정

90ms tween을 입력마다 재시작하던 경로에서, 16ms마다 큰 휠 입력이 들어오면 매 프레임의
애니메이션 경과 시간이 다시 0이 되어 첫 입력 프레임과 입력 도중에 진행하지 않는 문제가 재현됐습니다.
현재는 단일 틱커의 시간을 유지하며 목적지만 갱신합니다. 응답 시간상수는 12ms이고,
입력이 멈췄을 때 남은 목적지까지 반영한 뒤 스냅합니다.

합성 입력 120px × 12회, 프레임 간격 16ms 검사에서 첫 프레임의 이동은 0px → 88.37px로 바뀌었고,
현재 입력 목적지와의 차이는 최대 42.96px였습니다. 이는 프레임 시계에 대한 위젯 좌표 측정이며
실제 입력부터 디스플레이 표시까지의 지연 측정이 아닙니다.

선택 항목의 포인터 래퍼 삽입/제거로 계속 보이는 두 카드가 각각 4회 생성·3회 해제되는 문제도 재현했습니다.
래퍼 구조를 유지한 후 각각 1회 생성·0회 해제이며, 선택 경계 왕복의 각 이동 프레임에서
디코딩된 이미지 픽셀이 사라지지 않는지 CPU 래스터로 검사했습니다.
홈의 블러와 덱의 뒤집기도 위젯 삽입/제거 대신 활성 상태와 변환 값을 변경하여 이미지 경계를 유지합니다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project packages/Doroti.CustomCarousel/tests/Doroti.CustomCarousel.Tests -c Release -- --presentation-input temp/testing/carousel-presentation
```

네 개의 새 검사는 기본 회귀 명령에도 포함됩니다. 수정 전/후 JSON은 실행 당시
`temp/testing/carousel-presentation/before/`와 `after/`에 저장했습니다.
Windows Release 및 Web Debug 빌드, 기존 입력/데모 검사, Windows 네이티브 합성 입력 검사를 다시 통과했습니다.
실제 Windows 표시의 단일 프레임 깜빡임이나 물리 입력 체감까지 확인한 결과는 아닙니다.

## Windows에서 스크롤을 다시 시작할 때 멈추던 문제

추가 사용자 확인에서 카드 깜빡임은 사라졌지만, 스크롤을 멈췄다가 재개할 때마다
1~2초간 거의 움직이지 않는 문제가 남았습니다. 진단용 Windows 앱에서 사용자가 실제 입력으로
재현했고, 수정 전 기록에는 새 입력을 이유로 완료 프레임 표시를 취소한 경우가 326회 있었습니다.

Windows 호스트가 입력 콜백과 프레임 콜백을 application owner 큐에 비동기로 게시한 뒤
UI 처리가 완료되기 전에 GPU 렌더링으로 진행하는 순서 문제를 수정했습니다.
`WindowsManagedProductHost.BeginFrame`은 진입 시 대기 입력을 한 번만 가져오고,
같은 application owner에서 입력 적용 → 프레임 콜백 → 불변 장면 제출까지 완료한 뒤
래스터 처리를 시작합니다. 입력 처리 후에 프레임 콜백을 가져오는 기존 깜빡임 방지 순서는 유지합니다.
처리 중 들어온 입력은 다음 프레임에 남겨, 연속 입력을 끝없이 비우느라 현재 프레임이 밀리지 않게 합니다.

진단 시계도 보정했습니다. timestamp가 없는 focus 변경(`qpc=0`)이 후속 하드웨어 입력의
QPC 기준점을 0으로 설정하던 문제를 수정했습니다. Windows 진단 모드에서는 실제 phase 기록 시각을
추가하며, 루프의 무한 scroll extent는 JSON named floating-point literal로 기록합니다.
`DOROTI_FRAME_TRACE_CAPACITY=65536`으로 긴 입력의 시작 구간을 보존할 수 있고, 기본값은 8192입니다.

수정 후 사용자가 같은 방식으로 다시 테스트하여 시작 지연 해결을 확인했습니다.
보존된 사용자 입력 기록의 세 재개 구간은 움직인 장면의 native 제출까지 5.20ms / 9.67ms / 5.58ms였습니다.
이는 CPU/native 제출 시각이며 화면 scanout이나 display FPS를 측정한 값은 아닙니다.
기존 캐러셀/이미지/데모 회귀, Windows Release 및 Web Debug 빌드를 다시 통과했습니다.

재현 가능한 별도 native 회귀 명령은 다음과 같습니다. Windows Release 앱을 먼저 빌드해야 합니다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 python packages/Doroti.CustomCarousel/tests/windows-scroll-smoke.py
```

이 검사는 native 휠 입력 24개를 큰 delta와 작은 delta의 두 burst로 보내고 중간에 800ms 쉬며,
실제로 움직인 장면의 native 제출이 500ms 안에 이뤄지는지와 입력 때문에 완료 프레임이 취소되지 않는지 검사합니다.
실행 결과는 53.49ms / 11.10ms, 표시 취소 0회, exit 0입니다. 이 자동 검사의 작은 delta는 합성 휠이며
DirectManipulation 트랙패드 하드웨어 검사가 아닙니다. 결과와 원시 기록은 삭제 가능한
`temp/testing/carousel-windows-scroll/`에 생성합니다.

## 간헐적인 지연과 트랙패드의 연속 반응

이전 시작 지연 수정 이후에도 간헐적인 버벅임이 다시 보고되어, UI 스레드 스케줄링과
입력 중의 반응을 추가로 조사했습니다. 정상적으로 보였던 한 번의 실행으로 전체 해결을 판단하지 않습니다.
이번 수정은 카드 효과·스냅 설정을 변경하지 않고 공통 UI dispatcher와 Windows 메시지 처리에 적용했습니다.

`DorotiApplicationDispatcher`의 두 ThreadPool 의존을 격리 검사로 재현했습니다.
ThreadPool을 하나의 대기 작업으로 포화시킨 수정 전 검사에서 UI 콜백이 3초 안에 시작하지 못했습니다.
Channel의 readiness 통지를 직접 전달하도록 바꾼 중간 검사에서는 UI 작업이 완료됐지만,
네이티브 호출자의 완료가 500ms 안에 돌아오지 않았습니다. `InvokeAsync<T>`의 async wrapper가
완료 Task를 반환하기 전에 ThreadPool continuation을 기다리는 것이 두 번째 경로였습니다.

현재는 전용 owner의 read loop를 직접 깨우고, owner가 완료하는 Task 자체를 반환합니다.
사용자 콜백은 계속 전용 UI owner에서 실행하며 Task의 사용자 continuation은 비동기로 유지합니다.
포화된 bounded Channel의 admission 오류·취소도 전달하고, 이미 실행 중인 콜백의 취소를
완료로 오인하지 않도록 기존 drain 계약을 유지합니다. 수정 후 두 포화 검사와 lifetime 검사를 통과했습니다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Tests -c Release -- --application-dispatcher
```

이 수정 후 사용자 확인은 "줄었지만 간헐적인 버벅임이 남아요"였습니다.
Windows 기록의 UI 큐 대기는 최대 2.507ms, UI 완료부터 네이티브 재개까지는 최대 0.360ms였고,
실행 중 UI 작업이 57.178ms 걸린 구간도 한 번 있었습니다. 큐 대기만으로 남은 체감을 설명하지 않았습니다.

Windows의 기본 메시지 처리에서는 posted 메시지가 하드웨어 입력보다 먼저 처리됩니다.
([GetMessage 공식 계약](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getmessagew))
프레임 요청을 매번 posted 메시지로 쌓던 경로를 바꾸어 대기 중 요청을 하나로 합쳤으며,
`PeekMessage(PM_QS_INPUT)`으로 입력을 최대 8개 먼저 처리한 뒤 일반 큐에도 순서를 줍니다.
([PeekMessage 공식 계약](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-peekmessagew))
종료·프레임 완료·창 변경 메시지가 계속 처리되며, 입력 packet 자체는 버리거나 합치지 않습니다.

최종 사용자 재확인은 "움직이는 동안도 부드러워졌어요"입니다.
그 실행에서는 프레임 요청 13,899개 중 4,759개가 중복으로 합쳐졌고,
하드웨어 메시지 큐의 관측 최대 대기는 16ms였습니다. UI 큐 최대 3.280ms,
UI 완료 후 네이티브 재개 최대 0.468ms였습니다. 트랙패드 사용을 포함한 입력은
native wheel 1,420 packet / panZoom 0 packet으로 들어왔으므로 이 기기에서 확인한 경로는 휠 fallback입니다.
이 실행들의 입력량·조작이 같지 않으므로 카운터 차이를 통제된 성능 개선 비율로 사용하지 않습니다.

공통 `Doroti.Tests` 기본 검사, 캐러셀/이미지/다섯 데모 회귀, C++ 및 Windows Release·Web Debug 빌드를 통과했습니다.
최신 native 합성 입력 검사도 두 burst의 이동 장면 제출 47.165ms / 8.102ms,
입력 때문에 완료 프레임 취소 0회, exit 0으로 통과했습니다. display FPS/scanout,
다른 장치의 DirectManipulation 입력이나 다른 플랫폼의 실제 실행은 이 확인과 별도입니다.

`DOROTI_WINDOWS_FRAME_TIMING=1`을 진단 모드와 함께 사용하면 exit JSON의 `frameScheduling`에
UI 큐 대기·UI wall/CPU 작업·완료 후 재개·GC pause를 기록하고, `inputIngress`에 입력 경로별 packet 수를 남깁니다.
최근 2,048 프레임과 16.667ms 이상 걸린 구간 최대 256개를 보존해 긴 조작 뒤에도 간헐적인 지연을 볼 수 있습니다.
기본 실행에서는 이 측정과 버퍼 할당을 하지 않습니다. native 로그의 `doroti.windows.message-pump`에는
입력 메시지 큐 대기와 요청 병합 카운터가 남습니다. 원시 기록은 삭제 가능한
`temp/testing/carousel-scheduling/`에 생성했습니다.

## Windows 휠 입력의 장치 종류

기존 native `WM_MOUSEWHEEL`/`WM_MOUSEHWHEEL` 경로는 promoted touch/pen 서명만 확인하여,
트랙패드가 휠 메시지로 보낸 스크롤을 mouse로 전달했습니다. 이제 메시지 처리 중
`GetCurrentInputMessageSource`를 조회하고, Windows가 `IMDT_TOUCHPAD`로 식별한 입력은
`PointerDeviceKind.trackpad`로 전달합니다. mouse/touch/pen 출처도 해당 종류로 전달합니다.
([장치 출처 API](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getcurrentinputmessagesource),
[Windows 장치 종류](https://learn.microsoft.com/en-us/windows/win32/api/winuser/ne-winuser-input_message_device_type))

커서 이동·클릭·드래그는 기존 cursor 종류를 유지하며, scroll 종류는 그 세션 상태를 덮어쓰지 않습니다.
DirectManipulation의 panZoom 입력은 trackpad 경로를 사용합니다. 출처가 제공되지 않거나
조회에 실패하면 기존 promoted touch/pen 서명 및 mouse fallback을 사용하며,
delta의 크기나 간격으로 장치를 추측하지 않습니다.

C++ 빌드에 포함한 11개 compile-time 검사는 OS 출처 우선순위, 출처 미제공 시의 fallback,
트랙패드 scroll 다음의 mouse leave/drag 및 pen cancellation 종류 유지까지 확인합니다.
캐러셀 연속 입력 회귀 10개 중 추가한 packet 변환 검사는 trackpad/mouse scroll을 번갈아 전달하여
framework의 장치 종류·delta 보존 및 캐러셀의 즉시 이동을 확인합니다. native C++ 및 Windows Release
빌드도 통과했습니다.

진단 모드에서 `inputIngress`에 `trackpadWheelPackets`, `mouseWheelPackets`, `otherWheelPackets`를
추가했고, native 로그의 `doroti.windows.pointer-source`에는 OS가 알려준 출처별 휠 packet 수를 기록합니다.
`SendMessage`로 만든 합성 입력은 이 API에서 `IMDT_UNAVAILABLE`이므로 자동 휠 smoke는
기존 fallback의 회귀만 검증하며 실제 트랙패드 분류를 증명하지 않습니다.
물리 입력 기록은 삭제 가능한 `temp/testing/carousel-pointer-source/`에 생성합니다.

첫 물리 확인에서는 트랙패드·마우스 휠을 모두 사용했으나 OS 출처는 535개 모두 `IMDT_MOUSE`였고,
panZoom은 0개였습니다. API 조회 추가만으로 이 기기의 트랙패드 분류가 해결됐다고 판단하지 않았습니다.
Composition presenter를 연결하면서 `effectiveNative.ChildHwnd`를 숨겨진 raster HWND로 교체한 뒤,
그 창을 DirectManipulation과 touch/pen subclass에도 전달하던 문제가 확인됐습니다.
`WindowsManagedProductHost`에 원래 native host가 제공한 입력 HWND를 별도로 전달하고,
트랙패드 viewport와 touch/pen subclass는 이 입력 창에 연결하도록 수정했습니다.
렌더링·platform raster에는 기존 raster HWND를 사용합니다. 진단 `inputIngress`의
`inputWindow`/`rasterWindow`로 실제 연결을 확인할 수 있습니다.

입력 창 연결 수정 후 사용자 재확인은 "둘 다 정상적으로 움직여요"입니다.
실제 기록은 `inputWindow=4458632`, `rasterWindow=4852538`로 분리됐고,
트랙패드 panZoom update 170개와 mouse wheel 134개를 받았습니다. exit 0, failed terminal 0이며
trackpad/pointer 처리 오류 로그는 없었습니다. 이 기기는 트랙패드를 wheel 종류로 다시 표시하는 대신
DirectManipulation의 별도 trackpad 제스처 경로로 전달합니다. touch/pen도 입력 창 연결은 수정했지만,
실제 터치스크린·펜 입력은 이번 확인에 포함되지 않았습니다.

입력 창 연결 수정 후 캐러셀 기본 검사(연속 입력 10개, presentation 4개, 다섯 데모와 두 viewport)를
통과했습니다. native 합성 휠 smoke도 24 packet, 두 burst의 이동 장면 제출 53.877ms / 7.287ms,
입력으로 인한 프레임 취소 0회로 통과했습니다. 물리 트랙패드 확인과 합성 휠 결과는 각각 별도 기록입니다.

## 밝은 미니멀 UI 개편 — 2026-10-06

오프화이트 배경, 간결한 검정 타이포그래피와 버튼으로 홈과 다섯 데모를 정리했습니다.
홈은 추상 도형 대신 실제 음식 사진, 버거 레이어, 카드 덱, 지갑과 앨범 이미지의 미리보기를 표시합니다.
제목과 설명은 이미지 아래에 두고, 하단 표시로 다섯 데모를 직접 선택할 수 있습니다.

- PASS: Windows App SDK Release 빌드, 경고 0개 / 오류 0개.
- PASS: 기존 캐러셀 기본 검사. 연속 입력 10개, presentation 4개, 다섯 데모의 이동·버튼·토핑·음식 상세·shuffle 및 560×850 / 390×740 캡처.
- PASS: 임시 WidgetTester probe에서 홈의 320×640 / 390×740 / 900×850 배치, 다섯 직접 선택 표시와 320×640 데모 진입. CPU 캡처를 열어 배치와 이미지 표시를 확인했습니다.
- 미통과: 추가 Windows native 합성 휠 smoke는 `Expected two uninterrupted wheel bursts, got 1`로 exit 1입니다. native 앱 자체는 exit 0, failed terminal 0, 입력 때문에 취소된 프레임 0이지만, 두 입력 묶음이 한 scroll lifecycle로 기록되어 기존 검사 조건을 만족하지 못했습니다. 이 실행을 native 휠 회귀 PASS로 보고하지 않습니다.

모든 검사는 `python Doroti/eng/run-with-timeout.py --timeout 1200`으로 실행했습니다.
최종 CPU 캡처는 `temp/testing/carousel-design-final/`, native 원시 기록은
`temp/testing/carousel-design-final-windows/`에 생성했으며 삭제 가능한 로컬 산출물입니다.
이 UI 변경에 대한 물리 입력, 다른 플랫폼 실제 실행, scanout/FPS와 접근성의 신규 수락 결과는 없습니다.

## Record Box 앨범 배치와 회전 개선 — 2026-10-06

앨범은 정면을 유지하며 위아래로 이동하고, 평면 기울기는 최대 약 3.2도로 제한합니다.
선택 앨범을 가장 앞에 두고 양쪽 이웃 한 장씩 표시하며, 흰 슬리브와 부드러운 그림자로 깊이를 표현합니다.
홈 미리보기도 같은 슬리브로 맞췄으며, 이웃 앨범을 눌러 선택할 수 있습니다.

- PASS: 기존 캐러셀 기본 검사와 다섯 데모, 560×850 / 390×740 캡처.
- PASS: 임시 Record Box probe의 560×850 / 390×740 / 320×640 화면과 전환 중 12개 프레임. 실제 렌더 변환에서 표지의 방향과 원근 뒤집기 여부를 검사하고, CPU 캡처를 열어 확인했습니다.
- PASS: Previous의 0→14 루프 경계와 변환된 이웃 앨범의 클릭 선택.
- PASS: Windows App SDK Release 빌드, 경고 0개 / 오류 0개.

검사는 모두 1200초 timeout wrapper로 실행했고, 캡처는 삭제 가능한
`temp/testing/record-box-redesign/`에 생성했습니다. 직전 UI 기록의 native 휠 smoke 미통과 판정은 유지합니다.

## 참고 이미지에 맞춘 Record Box 깊이 배치 — 2026-10-06

현재 구성은 어두운 보라색 영역에 9장의 앨범이 뒤에서 앞으로 이어지는 원근 스택입니다.
뒤쪽은 작고 어둡게, 앞쪽은 넓고 누운 형태로 배치합니다. 회전은 -27도~82도 범위로 제한하며,
앨범 전체의 투명도를 낮추는 대신 배경색 tint를 적용해 표지가 겹쳐 비쳐 보이지 않게 합니다.
앞 절의 정면 3장 슬리브 구성은 이전 단계의 기록입니다.

gskinner 원본의 `Border(top: BorderSide(color: Colors.white38, width: 2))`를 따라
카드 윗변에 2 논리 픽셀 하이라이트를 적용했습니다. 둥근 모서리에서 하이라이트가 튀어나오지 않게 자릅니다.
이 구현은 이미지 평면과 테두리이며, 별도 두께 메시나 옆면을 생성하지 않습니다.
원본은 `reference/flutter_custom_carousel-main/example/lib/views/record_box_view.dart`와
[gskinner 저장소](https://github.com/gskinnerTeam/flutter_custom_carousel/blob/main/example/lib/views/record_box_view.dart)에서 확인했습니다.
홈의 Record Box 미리보기도 같은 효과를 사용하며, 데모 진입 시 15개 앨범을 precache합니다.

- PASS: 기존 캐러셀 기본 검사, 다섯 데모 및 두 viewport 캡처.
- PASS: 560×850 / 390×740 / 320×640, 전환 중 12개 프레임에서 9장 유지와 투영 평면 경계·표지 방향 검사. CPU 캡처를 열어 배치를 확인했습니다.
- PASS: 루프 경계와 드러난 앨범 윗부분을 눌렀을 때의 선택.
- PASS: Windows App SDK Release 빌드, 경고 0개 / 오류 0개.

검사는 1200초 timeout wrapper로 실행했습니다. 캡처는 삭제 가능한 `temp/testing/record-box-depth/`에 생성했습니다.
이 단계에서 native 휠 smoke와 물리 입력·scanout/FPS 판정을 갱신하지 않았습니다.
