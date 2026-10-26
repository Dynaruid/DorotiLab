# README에서 분리한 개발·검증 기록

정리 날짜: 2026-10-27

프로젝트 소개와 사용 안내에 섞여 있던 개발·검증 기록을 원래 문서별로 보존합니다. 아래 결과와 상태는 각 원문의 작성 시점에 해당하며, 이번 문서 정리에서 빌드·실기기·성능 검증을 새로 수행한 것은 아닙니다. `PASS`, `PARTIAL`, 미검증 및 미측정 범위는 원문의 의미를 유지합니다. 링크 경로는 보관 위치에 맞게 조정했습니다.

## README.md

원래 위치: [README.md](../../../../README.md)

The [2026-10-05 Apple follow-up](../05/work3-summary.md#14-2026-10-05-실행-결과) records current
AppKit auxiliary windows/NSMenu, Catalyst scene policies, shared mobile view
lifetimes and typed iOS device selection. Build, native automation, physical input
and distribution have separate acceptance scopes; the complete plan remains partial.

## README.ko.md

원래 위치: [README.ko.md](../../../../README.ko.md)

[2026-10-05 Apple 후속 결과](../05/work3-summary.md#14-2026-10-05-실행-결과)에 AppKit 보조 창·NSMenu,
Catalyst scene 정책, 모바일 공유 View 수명과 typed iOS 장치 선택을 기록했습니다.
빌드·native 자동화·물리 입력·배포 수락은 각각 구분하며 전체 계획은 PARTIAL입니다.

## packages/platforms/web/README.md

원래 위치: [packages/platforms/web/README.md](../../../../packages/platforms/web/README.md)

Historical Safari checks recorded a 100-second, five-mode slider PASS and verified stale-fence recovery
in WebKit and Chromium. A physical slider/list-scroll repeat still failed its
application control check and later reloaded, so sustained physical Safari
responsiveness is not fully qualified. Its last samples showed continuing
native frames, rather than a blocked GPU queue. An earlier Safari stress run
has a WebContent `highwater` jetsam report at approximately 1.50 GiB.

Historical iOS 27 simulator checks recorded the original full-AOT startup stack overflow and the mixed-AOT fix.
Safari 27.0 on the iPhone 17 simulator completed 180 seconds across all five blur
modes, 10,572 continuous slider moves and a tab round trip after each mode:
8,234 submitted frames, zero failed frames, GPU peak two, and retained filter
storage below 32 MiB in one mixed-AOT run. Submitted frames do not measure displayed FPS.
The final source publish, including immediate shader-wrapper release, still
failed a slider/list-scroll control check and stopped emitting main-page heartbeat
during a separate slider-only run's second mode. The startup overflow is fixed;
sustained simulator stability remains unqualified. A diagnostic runtime 10.0.12
comparison also failed the simulator control check, so the pinned 10.0.11 version
was retained. The final physical Safari
retest could not launch because the iPhone was locked; earlier iOS 26.6.1
observations remain separate evidence.

Historical checks on 2026-10-06 covered iOS Chrome (WebKit) on an iPhone 12 with iOS 26.6.1: the original AOT build reloaded during its
second Fast adaptive drag; the changed AOT build completed 30 synthetic drags
across all five modes with no reload or failed frame. Non-AOT comparison runs
reduced filter surface creation from 8,918 to 1,488. Resident GPU/process memory
was not measured. Playwright WebKit 26.4 still fails AOT startup with a stack
overflow in both builds; that browser/runtime combination is not qualified.
The initial record labeled physical runs as Safari; the recorded `CriOS` UA
corrects that attribution. These zero-ending sweeps also do not qualify
sustained rendering or application input responsiveness.

Paint releases temporary native shader wrappers on the render owner immediately
after `SKPaint` retains its own reference. This avoids retaining those wrappers
until GC/finalization during repeated paints. CPU snapshot regression tests
verify gradient pixels still render correctly after the wrapper is released.

## tools/vscode-doroti/README.md

원래 위치: [tools/vscode-doroti/README.md](../../../../tools/vscode-doroti/README.md)

| Apple target | Retained execution record | Current provider boundary |
| --- | --- | --- |
| AppKit macOS | [2026-10-01](../03/works/platforms/macos.md): real CLI metadata updates, retained state, compile recovery/Stop and installed VSIX PASS | `macos` Debug `dev` uses the provider-packaged Mac adapter with `DorotiMacDevelopment=true`, startup hooks, direct bundle execution and isolated build caches. On 2026-10-17, .NET SDK 10.0.401 AppKit CLI metadata/state preservation, compile recovery, unsupported edits and Stop passed; installed VSIX Run/Hot Reload/Stop also passed. |
| Mac Catalyst | [2026-10-01](../03/works/platforms/maccatalyst.md#2026-10-01-mac-catalyst-metadata-hot-reload): CLI and installed-VSIX metadata/state/Stop PASS | `maccatalyst` Debug `dev` uses the same packaged adapter and enables the editable `all,-Doroti.Host.Maui` interpreter profile and SDK named-pipe transport. On 2026-10-17, .NET SDK 10.0.401 Catalyst CLI metadata/state preservation, compile recovery, unsupported edits and session-matched Stop passed; installed VSIX Run/Hot Reload/Stop also passed. |
| iOS simulator / iPhone | [2026-09-30 and 10-01](../03/works/platforms/ios.md#2026-09-30-ios-metadata-hot-reload): simulator, iPhone 12 .NET 11 CoreCLR USB, and .NET 10 Mono LAN Hot Reload; CLI and installed-VSIX PASS | iOS profiles now advertise Debug `dev` and use the packaged iOS adapter. Select a matching `doroti.device`; .NET 10 simulators use a local acknowledgment relay, .NET 10 iPhones use a LAN SDK relay, and .NET 11 CoreCLR devices use SDK USB deltas plus CoreDevice acknowledgments. On 2026-10-17, .NET SDK 10.0.401 arm64 simulator CLI metadata/state preservation, compile recovery, unsupported edits and session-matched Stop passed; installed VSIX Run/Hot Reload/Stop also passed. On 2026-10-17, iPhone 12 .NET 11 RC2 CoreCLR USB CLI metadata/state preservation, compile recovery, unsupported edits and session-matched Stop passed; installed VSIX Run/Hot Reload/Stop also passed. The .NET 10 iPhone LAN path has not been rerun. |

The retained September/October records include installed-extension execution and describe the earlier launch/profile implementation; their .NET 11 USB run used RC1 plus a specific runtime override. Current 2026-10-17 validation is listed separately in the table above. Repeat native acceptance on other host/toolchain profiles. Physical input, other devices, Release and NativeAOT are outside these Hot Reload results.

The implementation was written independently after static review of `reference/AvaloniaVSCode-ARCHIVE`; no archive source or dependencies were copied. No preview server, LSP, debug adapter, Marketplace publication, multi-root, remote/device discovery or browser-only editor support is included.

## samples/DorotiSampleApp2/README.md

원래 위치: [samples/DorotiSampleApp2/README.md](../../../../samples/DorotiSampleApp2/README.md)

이전 CLI·설치 VSIX의 [실행 기록](../03/works/platforms/ios.md#2026-09-30-ios-metadata-hot-reload)은
보존합니다. 이번 provider 재연결은 자동 계약 검사 범위이며 현재 Mac·실기기 실행 수락은 `notVerified`입니다.

## packages/platforms/maui/README.md

원래 위치: [packages/platforms/maui/README.md](../../../../packages/platforms/maui/README.md)

The tooling provider discovers available iOS simulators and connected paired
physical devices through typed device results. Run selects a device matching the
explicit RID and supplies the SDK `_DeviceName`; ambiguous selection fails.
Android typed planning now uses the provider-packaged ADB development adapter
(Python 3) and declares nonce-matched graceful Stop. The adapter owns the
device/package lease, SDK watcher and recorded USB port independently of the
tool connection. Android Debug Mono metadata reload is qualified separately from
Release/AOT and x64 device execution. Apple Debug development now plans the packaged
Mac/iOS Python adapters using the declared TFM/RID and runner SDK directory.
AppKit/Catalyst enable their development startup/transport policy; iOS simulators
use local request/frame acknowledgments, .NET 10 iPhones use the SDK LAN relay,
and .NET 11 CoreCLR devices use SDK USB deltas plus CoreDevice acknowledgment files.
The adapters retain process-held session/device leases and nonce-matched graceful
Stop. Portable plan/adapter/relay and real tool-package checks are separate from
current Apple runtime/installed-VSIX acceptance, which remains notVerified.
The iOS runtime profile
is owned only by `packages/platforms/build/Doroti.IosNativeAot.props`, including
template consumers. Latest scope and remaining gates are in
[work3](../05/work3-summary.md#14-2026-10-05-실행-결과).

## packages/platforms/qt/README.md

원래 위치: [packages/platforms/qt/README.md](../../../../packages/platforms/qt/README.md)

The launcher and managed loader select native Wayland in a Wayland session and
xcb in an X11 session, preserving explicit `QT_QPA_PLATFORM`. Vulkan uses
`libvulkan.so.1`. The previous system-Qt 6.8.2 xcb receipts do not qualify the
new Qt 6.12 distribution. Both backends, repeated shutdown and physical
GPU/menu activation require separate evidence.

Acceptance and remaining environment-specific gates are recorded in the
[Qt runtime distribution history](../19/qt-runtime-bundle-summary.md).
Only combinations with actual install/runtime evidence should be described as
qualified; package creation or container installation alone is insufficient.

## tools/Doroti.Wgsl/README.md

원래 위치: [tools/Doroti.Wgsl/README.md](../../../../tools/Doroti.Wgsl/README.md)

Reflection contains source/definition/variant hashes, bindings and scalar field
offsets. Generated `Doroti.Generated.<AssetId>Parameters` properties flatten
vectors/matrices/arrays by index; `Snapshot()` writes each scalar at its Naga
offset with explicit little-endian writes. Padding remains zero. A vec3/mat3x3/
array/i32 golden checks offsets through byte 112 in a 128-byte block.
