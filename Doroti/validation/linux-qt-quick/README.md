# Linux Qt Quick GPU 합성 — 제품 경로

Testbed Linux runner는 기본적으로 **Qt Quick + Graphite Vulkan** 합성을 사용한다.
`Platform views` 탭과 독립 fixture에서 실제 Doroti 위젯과 Qt Quick Controls의
버튼·입력창을 `raster → native → raster → native → raster` 순서로 표시한다.
이전 QWidget/QImage 실험과 달리 managed 제품 renderer에 연결되어 있다.

```sh
dotnet run --project DorotiTestbedApp/linux/DorotiTestbedApp.Linux.csproj -c Release -r linux-x64

# 겹침 장면만 실행
DOROTI_TESTBED_MODE=platform-views \
  dotnet run --project DorotiTestbedApp/linux/DorotiTestbedApp.Linux.csproj -c Release -r linux-x64

# Material 샘플의 Platform views 탭
DOROTI_TESTBED_MODE=sample \
  dotnet run --project DorotiTestbedApp/linux/DorotiTestbedApp.Linux.csproj -c Release -r linux-x64
```

Qt 6.6+ Quick/Qml/QuickControls2 개발 파일과 `QtQuick`, `QtQuick.Controls` 런타임
QML 모듈이 추가로 필요하다. 기존 Vulkan 1.2/Wayland/Qt Widgets 빌드 의존성도 유지한다.
Testbed는 `DorotiQtQuick=true`를 선택한다. 일반 runner/template의 기본값은 false여서
기존 앱에 Quick 링크를 강제하지 않는다. 선택값은 CMake 구성과 native library 복사에 반영된다.

## 합성·GPU 소유권

- Qt Quick이 Vulkan instance/device/queue와 swapchain을 소유한다. Graphite는 공개
  `QSGRendererInterface`에서 얻은 같은 장치·queue를 빌려 사용한다. 장치나 instance를
  managed code에서 파괴하지 않는다. Qt Quick 권장 instance extensions도 활성화한다.
- Graphite는 각 raster segment를 자신이 소유한 R image에 그린다. 성공한 recording을
  submit한 뒤 GPU에서 표시용 P image로 복사하고, P를 shader-read layout으로 전환한다.
  R은 관찰한 원래 layout/queue family로 되돌린다. 기존 Vulkan submission observer를 재사용한다.
- QSG는 `QSGVulkanTexture::fromNative`로 P를 가져온다. 전경 alpha를 보존하고 같은
  Quick scene 안에서 실제 Qt Quick Controls와 교차 배치한다.
- **표시를 위한 CPU readback은 없다.** GPU 내부 R→P copy는 존재한다. 검증 도구의
  `QQuickWindow::grabWindow()`는 별도의 진단용 합성 화면 readback이다.
- `QSG_RENDER_LOOP=basic`으로 GUI/native/render 접근을 직렬화한다. 충돌하는 render-loop
  환경 설정은 시작 시 거부한다. 최초 구현은 queue drain과 copy fence를 기다리는
  보수적인 동기화 방식이며, 비동기 pipeline이나 60Hz 성능 승인을 뜻하지 않는다.

공식 API 근거:
[QSGRendererInterface](https://doc.qt.io/qt-6/qsgrendererinterface.html),
[Vulkan texture import](https://doc.qt.io/qt-6/qnativeinterface-qsgvulkantexture.html),
[QQuickGraphicsConfiguration](https://doc.qt.io/qt-6/qquickgraphicsconfiguration.html).

## 실패·resize·종료 계약

- GPU 완료와 **실제로 Qt에 게시된 image generation**을 구분한다. resize 후 framework
  scene이 아직 준비되지 않아 취소된 frame은 이전 QSG texture를 대체하지 않는다.
  이전 published image는 후속 batch가 성공하고 Qt의 GPU 참조가 끝날 때까지 유지한다.
- texture identity는 VkImage handle과 별도다. allocator가 같은 handle 값을 재사용해도
  이전 texture wrapper를 잘못 재사용하지 않는다. QSG node는 texture 교체 시 유지한다.
- raster/native/shield 전체를 먼저 검증하며, 취소 frame은 이전 native 배치를 보존한다.
  geometry와 shader image를 성공한 frame의 순서로 적용한다.
- R/P와 retiring images를 포함해 128 MiB, 17 raster image, 한 변 16384 pixel 한도를 둔다.
  한도를 넘으면 명시적으로 실패한다. DPR·큰 창·복잡한 장면에는 별도 성능/메모리 검증이 필요하다.
- Vulkan instance는 `QQuickWindow`의 base destructor보다 오래 유지한다. 종료·교체로
  실제 swap을 받지 못한 raster frame은 superseded로 완료하며 presented로 꾸미지 않는다.
  managed terminal coverage는 presented/replayed/rasterized-superseded를 모두 검사한다.
- callback ABI 4 / 192 bytes는 유지한다. 선택 feature bit 17을 협상하고, Quick host는
  이 기능을 모르는 이전 managed callback table을 시작 전에 거부한다.
  별도 Quick API v1의 GPU descriptor는 48 bytes, composition part는 96 bytes다.

## 입력·지원 범위

`doroti/native-button`과 `doroti/native-editor`는 이 backend에서 **실제 Qt Quick Controls**다.
별도 OS child window나 QPushButton/QLineEdit를 Quick item으로 변환한 것이 아니다.
Qt native가 노출된 영역은 Qt에 이벤트를 맡기고, 전경 shield 영역은 managed pointer
경로로 한 번 보낸다. native keyboard focus와 Doroti text client도 구분한다.
전체 가림·모달에서 native를 hide/recreate하거나 native snapshot으로 교체하지 않는다.

translation/rect clip과 양방향 겹침을 지원한다. 임의 QWidget/adopt_widget, WebEngine Quick
adapter, native group opacity·rotation·perspective·rounded/path clip, native를 가로지르는
backdrop filter는 이 제품 변경의 지원 범위가 아니다. WebEngine widget-shell 실험의 결과를
이 backend의 WebView 지원으로 계산하지 않는다.

XWayland/Wayland의 제품 합성·합성 입력을 검증한다. 최종 scanout 원자성, 물리 입력,
한글 IME·Orca 전체 탐색, 다양한 DPR/물리 GPU, 두 제품 owner, device loss와 NativeAOT,
장시간 성능은 별도 미검증이다. 제품 `CaptureIncludesNative`는 framework의 raster capture
계약이므로 false를 유지하며 진단용 Qt 합성 캡처와 구분한다.

기존 Widgets B 경로를 명시적으로 실행하려면:

```sh
DOROTI_TESTBED_MODE=platform-views DOROTI_PLATFORM_VIEW_COMPOSITION=overlay \
  dotnet run --project DorotiTestbedApp/linux/DorotiTestbedApp.Linux.csproj -c Release -r linux-x64 \
  -p:DorotiQtQuick=false
```

## 검증

```sh
# Release 제품 build + 두 QPA의 독립 fixture/실제 Material 샘플
python3 Doroti/validation/linux-qt-quick/record.py

# 설치된 Khronos Vulkan validation layer도 켜기
python3 Doroti/validation/linux-qt-quick/record.py --validation

# managed ABI / 기존 Qt 계약
python3 Doroti/validation/run-with-timeout.py dotnet run \
  --project Doroti/validation/linux-qt-contract/Doroti.Validation.LinuxQtContract.csproj
```

각 subprocess는 외부 1200초 timeout을 사용한다. 제품 probe는 테스트용 shared library로
실제 Qt event loop에 합성 입력을 보내며, 제품 코드를 테스트 모드로 대체하지 않는다.
실행별 source hash·명령·exit code·terminal 요약·JSON·GPU 합성 캡처는
`Doroti/artifacts/platform-views/<UTC date>/linux-qt-quick/<run>/`에 기록한다.

확인 항목은 10개 overlap case, native button 단일 click, native text input,
전경 shield on/off의 native press 수, 전체 가림과 편집 내용/identity 보존,
create/dispose, resize, modal, 샘플 popup·navigation·좁은 창 전환이다.
validation 실행은 Vulkan 메시지와 failed frame이 없어야 성공한다.

## 이번 실행 기록 (2026-09-14)

[최종 결과·명령·source hash](../../artifacts/platform-views/2026-09-14/linux-qt-quick/005943-534160/result.json).
Qt 6.10.2 / Ubuntu 26.04 / VMware / llvmpipe / DPR 1에서 다음을 통과했다.

| 경로 | 독립 fixture의 10개 겹침 장면 | 실제 Material sample | Vulkan layer |
|---|---|---|---|
| XWayland (`xcb`) | 합성 픽셀·alpha·입력·수명·modal 통과 | popup·탭 왕복·resize 통과 | 실제 활성화 확인, 오류 0 |
| Wayland | 합성 픽셀·alpha·입력·수명·modal 통과 | popup·탭 왕복·resize 통과 | 실제 활성화 확인, 오류 0 |

[전체 가림 GPU 캡처](../../artifacts/platform-views/2026-09-14/linux-qt-quick/005943-534160/platform-views-xcb/scenario-2.png),
[모달 GPU 캡처](../../artifacts/platform-views/2026-09-14/linux-qt-quick/005943-534160/platform-views-wayland/modal.png),
[샘플 탭 GPU 캡처](../../artifacts/platform-views/2026-09-14/linux-qt-quick/005943-534160/sample-xcb/sample-tab.png).
캡처는 live native가 포함된 Qt GPU 합성 결과이며 최종 모니터 scanout은 아니다.

Quick ABI의 구 caller 거부, 중복·잘못된 part 거부, 20개 pending callback의 종료도 통과했다.
별도로 managed Qt ABI/입력/clipboard/환경 계약 및 Quick OFF native ABI/lifecycle 회귀를 통과했다.
초기 실패 로그는 별도 실행 디렉터리에 보존했다. 초기 resize에서 발견한 freed-image 참조,
종료 ACK 누락 및 Wayland colorspace extension 누락을 수정한 뒤 최종 검사를 다시 실행했다.

추가로 80ms 간격의 **50회 급격한 resize**를 수행했다. 종료 code와 failed frame 수는 0이지만,
Qt 6.10.2의 XWayland swapchain 생성에서 요청 extent와 X 서버의 현재 extent가 다르다는
Vulkan 메시지 2건을 관찰했다. 이 스트레스 실행은 정상 fixture/sample의 validation 통과와
구분하며 완전 승인하지 않는다. [별도 실행 기록](../../artifacts/platform-views/2026-09-14/linux-qt-quick/rapid-resize-followup/result.json).
일반적인 제품 장면 검증을 전체 WSI/고빈도 resize/성능 승인으로 확대하지 않는다.
