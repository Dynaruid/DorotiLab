# Doroti runtime과 framework

[English](README.md) | **한국어**

Doroti는 MAUI Windows, Android, iOS, native AppKit macOS, Mac Catalyst, Blazor WebAssembly와 Linux/Qt에서 공용 widget, layout, painting, semantics, rendering pipeline을 사용하는 C#/.NET UI framework입니다.

## 개발 방식

`src/Doroti.Framework.*`는 직접 유지보수하는 제품 source입니다. 공개 namespace는 project, assembly, package 이름과 같은 `Doroti.Framework.*`입니다. 기능 추가와 정확성 수정은 소유 framework/runtime/host project에서 직접 수행하고, 바뀐 공용 계약의 모든 consumer를 함께 고칩니다.

Dart-to-C# compiler와 고정 Flutter checkout은 선택적인 import·동작 reference 도구로 남습니다. 일반 build의 필수 조건이 아니며 제품 source를 덮어쓰지 않습니다. Compiler 출력은 명시적으로 검토·채택하기 전까지 격리 workspace에만 둡니다.

Source 소유권은 [ADR-019](docs/adr/ADR-019-product-framework-source-ownership.md), 기본 native bridge graph는 [ADR-022](docs/adr/ADR-022-default-native-platform-bridge.md)를 참고하세요.

## 현재 제품 경계

- `Doroti.Framework.*`: 제품이 소유하는 Foundation, Scheduler, Services, Physics, Animation, Gestures, Painting, Semantics, Rendering, Widgets, Cupertino, Material library
- `Doroti.Runtime`, `Doroti.Ui`, `Doroti.Hosting`: runtime 의미와 target-neutral startup/builder/descriptor 계약
- `Doroti.App.Sdk`: 플랫폼 중립 `net10.0` 앱 assembly와 공용 asset 계약
- `Doroti.Runner.Sdk`: 고정 target runner 검증과 runner-local native/Web bootstrap/plugin registration
- `Doroti.Skia.RuntimeEffects`: native/Web host가 공유하는 fail-closed SkSL compiler와 uniform/image-sampler binder
- `Doroti.Skia.Rendering`: native GPU host가 공유하는 host-neutral scene, paragraph, image, runtime-effect, semantics, cache, terminal-ACK renderer
- `Doroti.Host.Maui`: 공용 MAUI lifecycle과 독립 SKGLView/AppKit MTKView Metal surface adapter
- `Doroti.Host.Web`: host 소유 Blazor composition, WebGL2 canvas, input, accessibility, resource bridge
- `Doroti.Host.Qt`: managed-owned Linux process, Qt 6 `QOpenGLWindow`, versioned C ABI v2, GPU surface, input, IME, desktop service와 accessibility adapter

Web 실행 source는 TypeScript가 소유합니다. 앱은 `web/src/**/*.ts`, Doroti는 `src/Doroti.Host.Web/Web/*.ts`를 편집합니다. `Microsoft.TypeScript.MSBuild` 7.0.0이 runner-local `obj`에 JavaScript를 만들며 publish에는 그 결과만 포함됩니다. 앱 도구로 Node, npm, Bun, bundler를 요구하지 않습니다. 자세한 결정은 [ADR-020](docs/adr/ADR-020-web-typescript-bootstrap.md)에 있습니다.

Material 앱은 `MaterialApp(theme:, darkTheme:, themeMode: ThemeMode.system)`으로 시스템 다크 모드를 따릅니다. `ColorScheme.CreateFromSeed`에 `Brightness.light`/`Brightness.dark`와 `surface`, `primary`, `outline` 같은 role override를 전달해 두 팔레트를 구성하고, widget은 `Theme.of(context).colorScheme`에서 현재 팔레트를 읽습니다. MAUI와 Web의 시스템 변경 전달 및 전체 예시는 [DorotiDemoApp 다크 모드 문서](../DorotiDemoApp/README.ko.md#시스템-다크-모드와-색-팔레트)를 참고하세요.

Android, iOS, native AppKit macOS, Mac Catalyst runner는 각각 앱 소유 기본 native binding을 참조합니다. Android는 `AndroidGradleProject`, Apple 제품은 명시적인 `XcodeProject` binding 계약을 사용합니다. 최종 앱 소유자는 계속 .NET runner이며 build 결과는 native launch, accessibility, signing, archive 증거를 대신하지 않습니다.

## 요구 사항

- [global.json](global.json)에 고정한 .NET SDK 10.0.400 또는 호환 patch
- PowerShell 7
- 10.0.11의 .NET/ASP.NET/WindowsDesktop 및 browser-wasm runtime pack과 선택 target에 맞는 MAUI/WebAssembly workload
- `web/tsconfig.json`이 있는 Web runner에서만 restore하는 `Microsoft.TypeScript.MSBuild` 7.0.0

Linux runner는 Linux x64 호스트에서 Qt 6.5 이상 Core/Gui/Widgets/OpenGL, CMake, C++ compiler, `pkg-config`, Wayland client 개발 파일, `wayland-scanner`, 실행할 QPA plugin(`wayland` 또는 `xcb`)을 system dependency로 사용합니다.

`reference/flutter-master` checkout은 명시적인 Flutter reference 비교에만 필요합니다. 필요하면 `pwsh -File ./Doroti/eng/prepare-flutter-sdk.ps1`로 준비합니다.

## 명령

Repository root에서 실행합니다.

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 doctor
pwsh -File ./Doroti/eng/doroti.ps1 build
```

활성 명령은 다음과 같이 단순화했습니다.

| 명령 | 용도 |
| --- | --- |
| `doctor` | 필수 .NET/PowerShell 도구를 확인하고 선택적인 reference checkout 상태 보고 |
| `build` | `Doroti.Product.slnx` build |
| `build/run/publish -App <path> -Platform <alias>` | `doroti-workspace.json`에서 runner를 찾아 실행 |
| `native doctor\|build\|open\|add -App <path> -Platform android\|ios\|macos\|maccatalyst` | 기본 native bridge workspace 진단, 빌드, 위치 출력, 확장 |
| `clean` | Doroti build output, artifact, 임시 local state 제거 |

`eng`에는 별도 검증 suite 진입점을 제공하지 않습니다. 기존 검증 계약과 evidence는 [validation](validation/README.md)에, 과거 실행 결과는 repository root의 `history/`에 보존합니다.

## 플랫폼 evidence 경계

Linux Qt의 공용 renderer, 실제 Material gallery, swap 기반 terminal ACK, 기본 input callback, semantics tree, framework-dependent/self-contained publish는 Kubuntu 26.04 VMware의 Wayland/XWayland에서 확인했습니다. 물리 Linux, 실제 X11 session, 한글 IME/Orca, context 강제 재생성, 장기 soak와 성능은 `notVerified`입니다. 자세한 값은 [Linux Qt live evidence](validation/evidence/linux-qt/kubuntu-vmware-spike.json)와 [packaging evidence](validation/evidence/linux-qt/kubuntu-packaging.json)를 참고하세요.

AppKit의 현재 live 범위와 남은 gate는 [AppKit product live record](validation/evidence/appkit-macos/product-live.json)에 따로 기록합니다. Build, native live, browser live, physical/device, accessibility 결과는 서로 대신하지 않습니다.

## Source와 artifact 원칙

- 제품 framework 변경은 `src/Doroti.Framework.*`에서 수행하며 compiler 소유 `.g.cs`는 이 경로에서 compile하지 않습니다.
- 공용 동작은 가장 낮은 소유 framework/runtime/rendering/host 계약에서 고칩니다.
- Reference 비교, build, native live, browser live, physical, cross-target 결과를 구분합니다.
- `validation/contracts/`에는 활성 validator가 읽는 작은 machine-readable contract를 둡니다.
- `validation/evidence/`에는 활성 target/Web validator가 만든 committed summary를 둡니다.
- `.doroti/`와 `artifacts/`에는 임시 tool·validation output을 둡니다.
- Repository JSON은 `System.Text.Json`을 사용합니다.

## 디렉터리 안내

| 경로 | 내용 |
| --- | --- |
| [`src/`](src/) | 제품 framework, runtime, renderer, host, target, SDK, analyzer |
| [`templates/`](templates/) | 7 runner와 4 binding을 포함하는 `doroti-app` template |
| [`eng/`](eng/) | 간소화한 build, validation, release, storage, 선택적 reference workflow |
| [`tools/`](tools/) | 선택적 Dart/Flutter compiler와 shared tooling |
| [`validation/`](validation/) | 활성 validation contract, fixture와 evidence |
| [`docs/`](docs/) | 현재 ADR과 역사 architecture 기록 |

Doroti는 repository의 BSD 3-Clause license로 배포합니다. Upstream source와 package 표시는 [third-party notices](THIRD-PARTY-NOTICES.md)를 참고하세요.
