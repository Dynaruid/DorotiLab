# Doroti

[English](README.md) | **한국어**

### C#과 .NET으로 만드는 cross-platform UI framework

> [!WARNING]
> Doroti는 현재 실험 단계입니다. API, architecture, 동작 및 project 구조는 하위 호환성 보장 없이 언제든 크게 변경될 수 있습니다.

Doroti는 공용 C# widget, layout, painting, semantics, rendering pipeline을 Windows, Android, iOS, native AppKit macOS, Mac Catalyst, Web, Linux/Qt에 제공합니다. 익숙한 Material/Cupertino API의 동작 reference는 Flutter이지만, 유지보수하는 제품 구현은 `Doroti.Framework.*`에 있습니다.

Doroti는 Flutter를 WebView에 넣지 않으며 플랫폼 UI control tree로 UI를 구성하지 않습니다. Platform host는 native window/view, GPU surface, input, text, clipboard, accessibility capability를 제공하고 Doroti가 widget/render tree를 소유합니다.

## 현재 개발 방식

프로젝트 초반에는 semantic compiler로 Flutter source의 큰 범위를 일괄 변환해 기반을 만들었습니다. 이 bootstrap은 현재 framework를 만드는 데 유효했지만 이제 일반 기능 개발 방식은 아닙니다.

현재 `Doroti/src/Doroti.Framework.*`는 제품이 직접 소유하는 C# source이며 namespace, assembly, package는 `Doroti.Framework.*`로 일치합니다. 기능과 수정은 소유 framework/runtime/renderer/host 계약에서 직접 개발합니다. Dart-to-C# compiler와 고정 Flutter checkout은 선택적인 import·reference differential 도구로 남고 제품 source를 덮어쓰지 않습니다. `DorotiDemoApp`과 생성된 `doroti-app` project는 C# 전용이며 활성 validation은 그 내부에 Dart package를 만들지 않습니다.

자세한 source 소유권은 [ADR-019](Doroti/docs/adr/ADR-019-product-framework-source-ownership.md), 기본 native bridge graph는 [ADR-022](Doroti/docs/adr/ADR-022-default-native-platform-bridge.md), 현재 Windows host 결정은 [ADR-025](Doroti/docs/adr/ADR-025-windowsappsdk-hwndexact-angle.md)를 참고하세요.

## 현재 동작 범위

- 공용 Material/Cupertino widget, element, layout, paint, semantics, state 기반
- 플랫폼 중립 C# 앱 library와 `macos`(AppKit), `maccatalyst`(UIKit)를 분리한 고정 target runner project
- 하나의 public target-neutral `Program` startup, host 소유 native 초기화, runner-local generated bootstrap code
- Windows 기본값인 self-contained Windows App SDK 2.4 `HwndExactCpp` child-HWND host와 managed hardware-D3D11 ANGLE/EGL Skia 경로. Windows MAUI는 명시적 독립 backend로 유지
- Web WebGL2를 통한 strict Skia GPU rendering
- native AppKit macOS/osx-arm64와 별도로 유지되는 Mac Catalyst를 포함한 고정 runner 빌드
- Linux x64의 Qt 6 `QOpenGLWindow`, versioned C ABI v2, Qt framebuffer 직접 Skia rendering
- 두 Apple desktop runner/binding을 포함하는 package-only template(총 12개 project)

현재 evidence에는 AppKit native launch/Metal presentation, Kubuntu VMware의 Wayland/XWayland Qt live 실행, Windows App SDK 기본 전환이 포함됩니다. 확인한 Windows 실제 resize와 mixed-DPI monitor 경계 이동은 사용자 acceptance를 받았지만 strict synthetic capture/pixel/cadence FAIL은 그대로 유지합니다. Windows 실제 한글 IME/Narrator와 더 넓은 DPI/device/window-management matrix, 물리 Linux와 실제 X11 session, Linux 한글 IME/Orca, context 재생성, 장기 성능, 미실행 target의 native/browser/physical/accessibility/signing/store 및 cross-target parity는 각각 독립적인 `notVerified` gate입니다.

## 구조

```text
제품 소유 Doroti.Framework.* source
                 │
                 ▼
        runtime + widget/render pipeline
                 │
                 ▼
         target host + GPU surface
  Windows App SDK/ANGLE · Windows MAUI · AppKit · Mac Catalyst
                 WebGL2 · Linux Qt/Skia GL
```

Flutter source는 fidelity 작업에서 동작 reference가 필요할 때 사용합니다. Compiler output은 격리된 candidate이며 제품 source of truth가 아닙니다.

## 실행

.NET SDK 10.0.400, 일치하는 10.0.11 runtime/workload와 PowerShell 7이 필요합니다.

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 build -App ./DorotiDemoApp -Platform windows
pwsh -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform windows
pwsh -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform windows -LastSuccessful
```

Windows 명령은 기본적으로 Windows App SDK/`HwndExactCpp`를 선택합니다. 독립 Windows MAUI runner가 필요할 때만 `-WindowsBackend Maui`를 명시합니다.

Windows presenter 기본값은 계속 `AngleD3D11`입니다. Maintainer는 `DOROTI_WINDOWS_PRESENTER=Vulkan`으로 실험적 Vulkan 경로를 명시적으로 시험할 수 있으며, capable GPU가 둘 이상이거나 선택을 고정할 때 `DOROTI_WINDOWS_VULKAN_DEVICE`를 지정합니다. 제품 경로는 System32 Vulkan 1.1, dedicated D3D11-texture external memory, Windows Presentation을 사용하며 Vulkan WSI swapchain은 0개입니다. SkiaSharp는 `4.152.0-rc.1.26426.14`로 고정하고 Vulkan context는 `SkiaSharp.Vulkan.Silk.NET`으로 구성합니다. Top-level HWND의 native topmost DirectComposition target이 full-capacity identity raster를 소유하고 HWND client clip만 exact viewport를 정합니다. 좌·상단처럼 origin이 움직이는 edge는 현재 raster를 같은 HWND에 둔 채 actual `WM_SIZE`를 latest frame으로 coalesce하고 interactive 각 step을 기다리지 않으며, origin 고정 edge의 bounded exact-frame handshake는 유지합니다. Windows 11 24H2 이상에서는 explicit Vulkan과 `experimentalAcrylic`을 함께 선택할 수 있습니다. Host-backdrop-enabled non-topmost `DesktopWindowTarget`의 active `DesktopAcrylicController` 위에 premultiplied Vulkan overlay를 합성합니다. 실제 resize/scan-out, Acrylic 품질과 전체 GPU/DPI/monitor matrix는 계속 `notVerified`이므로 Vulkan은 실험적 opt-in으로 유지합니다.
`-LastSuccessful`(또는 `-NoBuild`)은 runner, configuration, RID와 source/native input fingerprint가 일치하는 이전 성공 artifact만 재사용하며, 기록이 없거나 stale이면 fail-closed합니다. `-NoRestore`는 build는 수행하고 restore만 생략합니다.

## Repository 구성

| 경로 | 설명 |
| --- | --- |
| [`Doroti/src/`](Doroti/src/) | 제품 framework, runtime, rendering, host, target package와 SDK |
| [`DorotiDemoApp/`](DorotiDemoApp/) | 플랫폼 중립 앱, 7 runner, 4 native binding을 dogfood하는 앱 |
| [`Doroti/templates/`](Doroti/templates/) | `dotnet new doroti-app` template |
| [`Doroti/eng/`](Doroti/eng/) | build, SDK 준비, 로컬 상태와 선택적 진단 도구 |
| [`tools/Doroti.DartToCSharp/`](tools/Doroti.DartToCSharp/) | 선택적 Dart/Flutter import·migration compiler |
| [`history/`](history/) | Archive한 milestone 계획, 명령과 evidence 요약 |

명령과 evidence 경계는 [runtime README](Doroti/README.ko.md)를 참고하세요.

## Roadmap

현재 우선순위는 native desktop capability closure, Web live parity 자동화, target별 대표 release/physical acceptance flow입니다. Build, native live, browser live, physical, cross-target 결과는 서로 대신하지 않습니다.

Doroti는 개인 프로젝트입니다. 아이디어, 피드백, fork와 독립적인 실험을 환영합니다.

## License

[LICENSE](LICENSE)와 [third-party notices](Doroti/THIRD-PARTY-NOTICES.md)를 참고하세요.
