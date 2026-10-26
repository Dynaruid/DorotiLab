# Doroti macOS AppKit + Mac Catalyst 이중 backend 요약

- 기록일: 2026-08-20
- 상태: **AppKit 제품 graph, Metal surface, runner/native bridge/template 계약의 주요 구현 완료, lifecycle·입력·IME·VoiceOver·전체 회귀와 clean consumer 검증은 `notVerified`**
- 원본: 삭제한 루트 `work2.md`의 구현 상태를 압축한 역사 기록

## 1. 문서 성격

이 문서는 기존 Mac Catalyst/UIKit 제품을 유지하면서 native AppKit 제품을 별도 first-class target으로 추가한 작업의 2026-08-20 기준 결과를 보존한다. 새로운 active roadmap이나 두 backend의 전체 acceptance 완료 선언이 아니다.

AppKit 첫 제품 범위는 .NET 10, macOS 14 이상, `osx-arm64`, AppKit + Metal + Skia, 단일 window/view다. Microsoft AppKit backend는 experimental/unsupported preview이므로 exact dependency pin과 독립 evidence를 유지한다.

## 2. 영구 병행 제품 계약

| 항목 | Mac Catalyst | native AppKit |
| --- | --- | --- |
| workspace alias | `maccatalyst` | `macos` |
| TFM | `net10.0-maccatalyst` | `net10.0-macos` |
| RID | `maccatalyst-arm64` | `osx-arm64` |
| entry/UI | UIKit | `NSApplication` + AppKit |
| GPU surface | 기존 SKGLView 계열 | Doroti-owned `MTKView` + Metal/Skia |
| target package | `Doroti.Target.MacCatalyst.Maui.maccatalyst-arm64` | `Doroti.Target.MacOS.Maui.osx-arm64` |
| native binding | Catalyst/UIKit Swift | macOS/AppKit Swift |

- 어느 alias도 다른 backend로 자동 fallback하지 않는다.
- runner, target package, native binding, scheme, plist/resource, lock/evidence를 서로 덮어쓰지 않는 독립 graph로 유지한다.
- 어느 backend도 legacy나 제거 예정으로 표시하지 않으며 중단에는 별도 결정, ADR과 migration 기간이 필요하다.

## 3. 고정한 기술 경계

- `Microsoft.Maui.Platforms.MacOS`와 `.Essentials` `0.1.0-preview.12.26368.2`를 exact pin으로 사용한다.
- AppKit에서는 MAUI `SKGLViewHandler`가 없으므로 Doroti-owned `MTKView`, Metal command queue와 Skia surface adapter를 사용한다.
- scene raster semantics는 `Doroti.Skia.Rendering`이 계속 단일 소유한다.
- `presented` ACK는 paint callback이나 command-buffer commit이 아니라 Metal command-buffer completion에서만 발생한다.
- full-frame CPU readback/copy와 software fallback을 제품 경로에 두지 않는다.
- AppKit 전용 코드는 `net10.0-macos`/`MACOS` 경계로 격리하며 backend internal type이나 reflection에 의존하지 않는다.

## 4. 구현한 주요 결과

### Baseline과 Metal spike

- .NET SDK 10.0.400, macOS workload, Xcode 26.6, Apple Silicon 환경과 preview package/lock 정보를 doctor 및 dependency evidence에 기록했다.
- minimal AppKit 앱의 restore/build/native window 실행과 Catalyst asset 비혼입을 확인했다.
- `MacOSViewHandler` + `MTKView`, Metal/Skia surface lifetime, logical/physical/DPR 변환과 display-cadence coalescing을 구현했다.
- 실제 Doroti scene을 GPU로 표시했고 20회 resize, Retina scale, hide/unhide, minimize/restore와 retained replay를 수행했다.
- raster 요청과 Metal completion terminal ACK의 1:1 경계를 확인했다.

### 공용 host와 product graph

- `DorotiMauiSurface`, `MauiFrameworkHost`, `MauiHostAdapter`가 concrete `SKGLView` 대신 최소 surface 계약을 받게 분리했다.
- AppKit용 `DorotiMacOSMetalSurface`/handler와 `MacOSMauiApplication` delegate, `UseMauiAppMacOS`/MacOS Essentials 등록을 추가했다.
- `Doroti.Target.MacOS.Maui.osx-arm64`, AppKit runner, `AppKit-Main` bootstrap과 독립 intermediate/output 경로를 추가했다.
- workspace가 `macos`와 `maccatalyst`를 별도 runner로 선택하고 RID/TFM/cross-backend binding 오류를 build 전에 거부하게 했다.
- AppKit `.app`의 plist, entitlements, identifier/version에서 UIKit/Catalyst 전용 identity를 분리했다.

### Native bridge, template와 tooling

- `net10.0-macos` binding과 AppKit Swift/Xcode scheme을 추가하고 `SUPPORTED_PLATFORMS=macosx`, `SUPPORTS_MACCATALYST=NO`, minimum macOS 14를 고정했다.
- binding 단독 및 runner transitive build, live bridge 세 method, `platformInfo.platform == "macOS"`, `otool -L`/`file`의 Catalyst/UIKit 비혼입을 확인했다.
- Demo/template에 AppKit과 Catalyst runner, binding, native target, resources를 병행하고 solution/package/naming/ownership/native-bridge contract를 갱신했다.
- CLI/doctor/native 명령과 README/ADR가 `macos/osx-arm64/AppKit`과 `maccatalyst/maccatalyst-arm64/UIKit`을 일관되게 구분한다.
- graph/package/native-interop/template/evidence validator와 app bundle layout, architecture, dependency, plist, entitlement, code-sign 검사를 두 backend별로 분리했다.

## 5. 확인한 완료 경계

- AppKit product graph, runner/target/binding 선택과 Catalyst 분리: `PASS`
- AppKit Metal Doroti scene과 completion 기반 terminal ACK: `PASS`
- AppKit native bridge build/live 및 Catalyst/UIKit artifact 비혼입 검사: `PASS`
- AppKit/Catalyst template·CLI·문서·solution·package map의 이중 identity: `PASS`
- framework-dependent publish: macOS SDK/ILLink 제약의 `NETSDK1102` blocked evidence로 완료 gate에서 제외

이 결과는 삭제한 계획 문서에 기록된 당시 evidence의 요약이다. 이 역사 문서 작성 과정에서 build나 live gate를 다시 실행한 것은 아니다.

## 6. 남은 `notVerified` 및 미완료 항목

### Surface와 lifecycle

- device/context recreate, drawable 부재/error/cancel과 stale Metal completion 처리
- 기존 Windows/Android/iOS/Catalyst SKGLView path의 characterization 및 회귀 검증
- frame stop부터 GPU/session dispose까지 exactly-once 순서
- launch/activate/resign/hide/terminate, red close/`Cmd+Q`/programmatic close와 close veto 제한
- multi-screen 이동, theme/locale/clipboard/focus와 AppKit main-thread affinity

### Input, text와 accessibility

- `NSEvent` mouse/trackpad/button/precise scroll/phase의 전체 canonical sequence
- key repeat와 modifier, focus loss synthesized up/cancel, cursor ownership
- hidden Entry/Editor 왕복, 한글 IME preedit/candidate caret, emoji와 selection replacement
- `NSAccessibility`/VoiceOver tree, focus, action과 scroll coalescing

### Packaging과 acceptance

- 실제 1024px macOS icon과 mobile splash 조건 분리
- AppKit/Catalyst clean bundle의 양방향 dependency 비혼입 검사
- `echoOnMainThread` cancellation/late callback lifetime
- 두 backend runtime shader live 검증
- 외부 임시 경로의 새 template consumer restore/build/run
- fast CI와 macOS-hosted CI 분리 및 clean/no-restore matrix
- live harness의 frame/input/text/lifecycle/native bridge evidence와 frame invariant
- Catalyst/Windows/Android/iOS/Web/Linux 전체 회귀 shard
- backend별 lock/evidence/source fingerprint 분리와 최종 clean clone 검증
- physical input, 한글 IME, VoiceOver, production signing/notarization

build나 AppKit 창 표시를 lifecycle, input, accessibility, signing 또는 다른 platform 회귀의 PASS로 해석하지 않는다.

## 7. 재개 시 기준

- `macos`는 AppKit, `maccatalyst`는 Catalyst라는 명시적 identity와 fallback 금지를 유지한다.
- Metal command-buffer completion 이전에는 scene을 `presented`로 기록하지 않는다.
- AppKit과 기존 MAUI target이 공용 renderer를 쓰되 native surface와 platform adapter는 독립적으로 유지한다.
- preview package 버전은 자동 갱신하지 않고 dependency upgrade마다 restore/build/live를 다시 검증한다.
- Apple native evidence와 Windows cross-build, physical IME/VoiceOver/signing evidence를 서로 대체하지 않는다.

> 문서 성격: 삭제한 루트 `work2.md`의 AppKit + Mac Catalyst 이중 backend 구현 결과와 남은 acceptance 경계.
