# Doroti Windows 스크롤 성능/표시 회귀 조사 인계

- 기록일: 2026-08-18
- 상태: **Android 접근성 동기화 frame-drop 수정 및 실기기 자동 게이트 통과, 사용자 손가락 재평가 대기**
- 목표: 스크롤 시작/끝의 반복 끊김을 근본 원인에서 해결하고, 하단 위젯 소실과 스크롤 중 흰 점멸 회귀 없이 Flutter와 비슷한 retained rendering / raster cache / vsync 방식을 사용한다.
- 성능 기준: 사용자가 60 Hz 한 프레임인 약 16.6 ms를 매우 훌륭한 수준으로 지정했다.

## 2026-08-18 Android semantics/build frame-drop 후속

사용자 실기기 재평가에서 명시적인 끊김은 사라졌지만 미묘한 frame drop이 남았고, 외부 FPS보다
framework build 또는 접근성 노드 갱신이 의심된다는 피드백을 받았다. 기존 120 Hz 실기기 evidence를
프레임 내부 phase로 다시 분해한 결과 일반 build/layout은 병목이 아니었다.

```text
framework build p50/p95: 0.128 / 0.254 ms
layout p50/p95:          0.005 / 0.009 ms
framework frame p99:    37.931 ms
semantics received/applied: 710 / 499
native semantics writes:    11,779
native elements created:     1,048 (active 31)
immediate semantics flushes: 461
```

`MauiSemanticsBridge`는 15 Hz 제한을 갖고 있었지만, virtualized scroll에서 노드가 드나드는 topology
변경을 매번 immediate로 분류했다. 그 결과 접근성용 MAUI `Button`/`Label`의 생성, child tree 재부착,
layout bounds write가 시각 프레임과 같은 UI 경로에서 약 73 Hz로 실행됐다. framework의
`flushSemantics()`도 모든 120 Hz 시각 프레임에서 dirty geometry/tree를 다시 만들고 있었다.

적용 내용:

- 자동 topology 변경은 명시적 `immediate`/`scrollEnd`가 아니면 native 15 Hz apply 경계에 포함한다.
- 기존 노드의 label/value/action/flags/selection 변경은 계속 즉시 반영한다.
- 스크롤 중 제거되는 native semantics 요소를 종류별 pool로 재사용하고, 전체 `Children.Clear/Add` 대신
  실제로 달라진 child order만 증분 동기화한다.
- 의미 없는 root generation description write와 동일 layout flags/bounds 재기록을 제거한다.
- framework는 활성 `scrollStart`~`scrollEnd` 구간에서만 semantics build를 15 Hz로 합친다. 일반 UI 변경은
  즉시 처리하고, 지연 timer와 `scrollEnd` frame이 마지막 dirty tree를 보장한다.
- `semanticsBuild/End`, `semanticsApply/End`, `semanticsDeferred`를 공통 frame trace에 추가하고 apply 시간,
  topology apply, element reuse를 구조화된 진단 값으로 노출한다.

동일한 `SM-S931N` Release `AndroidPhysical` 최종 결과:

```text
before: 628 frames / 6.866 s =  91.470 FPS, janky 7.64%, render p95 12 ms, missed vsync 13
after:  783 frames / 7.111 s = 110.114 FPS, janky 3.83%, render p95  8 ms, missed vsync  1

framework frame p99: 37.931 -> 10.467 ms
semantics received/applied: 710/499 -> 274/268
native elements created: 1,048 -> 41 (reused 1,031, active 31)
native property writes: 11,779 -> 6,068
framework active-scroll semantics deferrals in retained trace: 25
native apply average: 1.639 ms
presented 3,170, failed 0, dropped 0, superseded 0
```

Android Release build, `validate-fcr5-scroll.ps1`, `validate-fcr6-semantics.ps1`, 전체 `AndroidPhysical`이
통과했다. UIAutomator 접근성 FAB action과 최종 노드 tree는 통과했지만 TalkBack 수동 조작은 실행하지
않았다. 합성 drag와 phase trace는 개선을 확인했어도 사용자의 실제 손가락 체감을 대체하지 않으므로
최종 완료 판정은 사용자 재평가 뒤에 한다.

## 2026-08-18 Android drag frame-pacing 2차 후속

사용자 실기기 재평가에서 비정상 흰 점멸과 명확한 스크롤 끊김은 사라졌지만, 손가락으로 스크롤하는 동안
FPS가 출렁이는 듯한 불균일한 움직임이 남았다.

동일한 `SM-S931N` 120 Hz 실기기 trace에서 입력과 offset 갱신은 이미 고주사율이었다.

```text
native input gap p50:  8.399 ms
scroll update gap p50: 8.426 ms
beginFrame gap p50:   16.417 ms
beginFrame gap p95:   25.464 ms
present gap p95:      30.673 ms
```

짧은 ballistic fling은 거의 모든 `beginFrame`이 8.33 ms였으므로 physics, GPU shader, 평균 raster 비용이
주원인이 아니었다. 입력으로 구동되는 drag에서는 다음 Choreographer waiter가 새 framework frame 요청이
생긴 뒤에만 등록됐다. 입력과 GL paint가 display pulse 후반에 겹치면 다음 waiter 등록이 늦어져 120 Hz
입력/offset 사이에 1~3개의 pulse를 불규칙하게 건너뛰었다.

적용 내용:

- Android host가 활성 native touch pointer 집합을 추적한다.
- 손가락이 내려가 있는 동안 Choreographer waiter를 미리 유지하되, dirty framework frame이 없으면 GL render는 요청하지 않는다.
- 요청한 paint가 아직 render thread에서 시작되지 않은 동안 새 pulse가 오면 해당 frame timestamp를 최신 pulse로 전진시킨다.
- 손가락이 모두 올라가면 연속 waiter를 끝내고, ballistic ticker는 기존 framework 요청 기반 pacing으로 돌아간다.
- Android evidence는 intent extra로 명시적으로 켜고 external cache 파일로 회수한다. 8192-entry trace 직렬화는 마지막 paint 뒤 250 ms 동안 조용할 때만 수행한다.
- `AndroidPhysical`에 12회의 500 ms 왕복 drag cadence 게이트를 추가하고, 120 Hz 장치에서는 최소 72 FPS와 render-work p95 16 ms 이하를 요구한다.
- 다크 테마 `#141218`을 black frame으로 오판하지 않도록 startup 검증을 순수 검정 비율로 분리했다.

최종 Release APK의 별도 12회 왕복 측정:

```text
663 frames / 6.984 s = 94.937 FPS
janky 52 (7.84%)
render-work p95 15 ms, p99 32 ms
GPU p95 6 ms
missed vsync 21
```

최종 `AndroidPhysical` 전체 게이트:

```text
display: 120 Hz
628 frames / 6.866 s = 91.470 FPS (gate >= 72 FPS)
janky 48 (7.64%)
render-work p95 12 ms
missed vsync 13
presented 2659, failed 0, dropped 0, superseded 0
```

`validate-fcr5-scroll.ps1` Debug/Release, app-target `Graph`, Android Release build, 그리고 전체
`AndroidPhysical`이 통과했다. ADB 합성 drag 평균과 자동 화면 검증은 사용자의 실제 손가락 체감을 대체하지
않으므로, 이 수정의 최종 완료 판정은 사용자 수동 재평가 뒤에 한다.

## 2026-08-18 Android 실기기 후속 결과

`SM-S931N` arm64 실기기(실제 120 Hz display mode)에서 기존 Android host는 framework animation이 다음
`InvalidateSurface`를 즉시 요청해 TextureView render가 native display pulse와 위상이 어긋났다. GPU p95는 6 ms인데도
Android `gfxinfo` p95가 38 ms였으므로 shader/GPU 연산보다 frame scheduling이 병목이었다.

적용 내용:

- Android animation frame 요청을 `Choreographer`의 display callback으로 전달한다.
- 이전 GL paint가 끝나지 않았으면 최신 framework callback 하나만 보존하고 다음 pulse에서 재시도한다.
- Choreographer frame timestamp를 Doroti monotonic clock에 매핑한다.
- 120 Hz를 고정 60 Hz로 제한하는 실험은 jank 비율이 나빠져 제거했다.

동일한 Release APK, 시작 후 12회의 500 ms 왕복 ADB swipe 비교:

```text
수정 전: 430 frames, janky 106 (24.65%), p95 38 ms, GPU p95 6 ms, missed vsync 38
수정 후: 663 frames, janky  67 (10.11%), p95 14 ms, GPU p95 6 ms, missed vsync 17
```

이는 ad-hoc physical 비교이며 사용자의 손가락 지속 스크롤 체감과 전체 `AndroidPhysical` shard를 대체하지 않는다.

다크 모드에서 `Scaffold` 미도색 영역이 고정 light clear `#FFFFFBFE`를 노출하던 문제도 함께 수정했다.

- zero-radius `RRect`는 native `DrawRect`로 제출하고, 일반 `RRect`는 네 모서리 radius를 모두 보존한다.
- `DorotiViewConfiguration`에 dark background를 추가하고 MAUI native surface/back-buffer clear가 theme 변경을 따른다.
- 실기기 live theme 전환에서 동일 body 픽셀이 light `#FFFFFBFE`, dark `#FF141218`로 바뀌었고 process가 생존했다.

## 사용자 관찰

1. 스크롤을 시작할 때와 끝에서 반복적으로 끊긴다. JIT 최초 실행 문제는 아니다.
2. 기존 평균/p95 위주 측정으로는 이 끊김을 잘 검출하지 못했다.
3. 실험적인 캐시 적용 뒤 하단 위젯이 사라지거나 스크롤되지 않는 회귀가 있었다.
4. 그 다음에는 스크롤 중 흰 배경이 점멸하는 회귀가 있었다.
5. 자동 검증 창의 포커스 이동은 문제 제기가 아니라 작업 혼동 여부를 알려 준 것이었다. 이후 숨김/비대화형 검증을 우선했다.

## 확정된 원인과 적용된 수정

### 1. WinUI compositor 이벤트의 잘못된 스레드 접근

숨김 Release 실행에서 다음 종료가 실제로 재현됐다.

```text
System.Runtime.InteropServices.COMException (0x8001010E)
ABI.WinRT.Interop.EventSource`1.Subscribe
Doroti.Host.Maui.MauiHostAdapter.SubscribeToCompositionVsync()
```

framework timer/microtask 스레드가 `CompositionTarget.Rendering`을 직접 구독한 것이 원인이었다. 이 예외는 부분적으로 그려진 트리, 하단 내용 소실, 빈/흰 표면처럼 보이는 현상을 만들 수 있다.

적용 내용:

- `MauiHostAdapter`의 compositor 이벤트 구독/해제를 MAUI UI dispatcher로 전달한다.
- 요청 상태(`_compositionVsyncRequested`)와 실제 연결 상태(`_compositionVsyncAttached`)를 분리한다.
- 종료 프레임 뒤 구독 해제와 dispose 경로도 같은 UI-thread 경계를 사용한다.

검증 결과:

- 수정 후 게시된 Release 앱이 숨김 상태로 5초 이상 생존했다.
- stderr가 비어 있었고 기존 `0x8001010E` 종료가 재현되지 않았다.
- 초기 UI Automation 의미 트리도 생성됐다.

### 2. 위험한 전체 RepaintBoundary 출력 캐시

스크롤 중 전체 경계를 이미지로 재사용하는 실험은 움직이는/클리핑되는 하위 트리를 오래된 이미지로 고정했다. 이것이 하단 위젯 소실과 흰/투명 점멸 회귀를 만들 수 있어 **전부 제거했다**.

현재 남긴 캐시는 범위가 좁고 invalidation 계약이 있는 것뿐이다.

- `PictureLayer`: 동일 picture, 안정된 transform, `willChange == false`, warm-up 이후에만 bounded GPU raster cache 사용
- `ImageFilterLayer`: layer identity와 subtree generation을 키로 사용하는 bounded 출력 캐시
- backdrop/native image-filter 리소스: context별 제한된 재사용

전체 RepaintBoundary raster metadata/cache/counter/검증 문자열은 제거된 상태다.

### 3. 입력이 아니라 raster/native 제출 구간의 병목

추적 용량을 8192개로 늘리고 다음 항목을 추가했다.

- `scrollStart`, `scrollUpdate`, `scrollEnd`
- 실제 offset/delta/activity/minExtent/maxExtent
- ticker의 `animationStart`, `animationEnd`
- `rasterEnd`
- input→offset, offset→present, input→present의 first/p95/max

실측에서 input→offset은 대체로 1 ms 미만이었다. 긴 프레임은 입력 처리나 JIT가 아니라 raster 이후에 발생했다.

`rasterEnd`로 분리한 대표 샘플:

```text
Doroti DrawScene: 약 0.7~1.9 ms
그 뒤 SkiaSharp/ANGLE native 제출 또는 완료 콜백 대기: 약 20~30 ms
```

설치된 SkiaSharp 4.151.1의 `SKSwapChainPanel`은 Doroti `PaintSurface` 콜백 반환 뒤 자체적으로 `canvas.Flush()`와 `GRContext.Flush()`를 수행한다. Doroti가 Windows 콜백 안에서 수행하던 중복 `canvas.Flush()`는 제거했다.

### 4. 검증 JSON I/O가 paint 경로에 있던 문제

`WriteEvidence()`가 native paint callback 안에서 trace snapshot, JSON 직렬화, 파일 쓰기를 수행하던 경로를 제거했다. 현재는 coalesced background writer로 이동했고, 최종 idle 상태도 기록하도록 지연 쓰기와 validator polling을 추가했다.

### 5. 고주사율 ANGLE 큐 백프레셔 실험

현재 장치에는 165 Hz 디스플레이가 있다. compositor 콜백마다 새 swap-chain 작업을 넣어 native 제출 큐가 막히는 가설에 따라 다음 코드가 **현재 적용된 상태**다.

- 첫 프레임은 즉시 요청한다.
- 연속 compositor 요청은 최소 10 ms 간격으로 제한한다.
- pending framework callback은 하나만 유지한다.

그러나 마지막 Release 실측에서 16.6 ms 기준을 만족하지 못했으므로 이 변경은 완료된 해결책으로 간주하면 안 된다.

## 현재 표시 완료 측정의 한계

현재 Windows `present`는 다음 순서로 기록된다.

1. Doroti가 `DrawScene`을 마친다.
2. `PaintSurface` callback 반환 뒤 SkiaSharp가 native flush를 수행한다.
3. `Dispatcher.DispatchDelayed(TimeSpan.Zero, ...)`로 전달된 completion이 실행될 때 `present`를 기록한다.

이 방식은 예전의 pre-native-flush 측정보다 보수적이지만, native flush 시간과 UI dispatcher 대기 시간을 분리하지 못한다. 따라서 마지막 측정의 20~30 ms 일부는 실제 swap 제출이 아니라 completion callback이 UI 큐에서 기다린 시간일 수 있다.

다음 compositor tick에서 completion을 확정하는 방식은 논의만 했고 **아직 적용하지 않았다**. 중단 시점의 소스는 `Dispatcher.DispatchDelayed` 방식이다.

## 최근 Release Windows 실측

### 잘못된 이전 경계(pre-native-flush)에서의 참고값

한 실행은 다음 값으로 게이트를 통과했지만 실제 native flush 이전 값이므로 최종 근거로 사용하면 안 된다.

```text
first input→present: 12.600 ms
input→present p95:   10.021 ms
input→present max:   12.843 ms
animation gap max:   22.830 ms
```

### native callback 반환 뒤 completion 경계

백프레셔 적용 전:

```text
first input→present: 16.216 ms
input→present p95:   17.997 ms
input→present max:   26.151 ms
animation gap p95:   12.192 ms
animation gap max:   36.636 ms
결과: FAIL
```

10 ms 백프레셔 적용 후 마지막 실행:

```text
first input→present: 12.090 ms
input→present p95:   18.428 ms
input→present max:   28.287 ms
animation gap p95:   17.515 ms
animation gap max:   39.179 ms
결과: FAIL
```

따라서 백프레셔가 개선됐다고 결론 내릴 수 없다. 측정 경계에서 dispatcher 대기가 섞이는 문제부터 분리해야 한다.

## 정확성 관련 현재 근거

- 반복 실제 휠 입력에서 scroll extent는 `0.0 ↔ 297.4`까지 도달했다.
- 상단과 하단 offset 샘플이 모두 trace에 존재한다.
- 확인한 실행에서 frame `failed=0`, `dropped=0`, `superseded=0`이었다.
- picture raster cache와 shader image-filter cache는 hit가 miss보다 많았다.
- 전체 RepaintBoundary 출력 캐시는 제거됐다.
- 흰 점멸은 사용자의 Android 실기기 재평가에서 사라졌다고 확인됐다.
- 하단 위젯이 실제 화면에서 계속 유지되는지는 사용자의 이번 수동 평가가 최종 근거다.

## 검증 상태

통과한 항목:

- 최신 `Doroti/eng/validate-fcr5-scroll.ps1`: Debug/Release 계약 통과
- 최신 `Doroti/eng/validate-app-targets.ps1 -Shard Graph -Configuration Release`: 통과
- 최신 `Doroti/eng/validate-app-targets.ps1 -Shard AndroidPhysical -AndroidSerial R3CY30KZA4B -Configuration Release`: 통과
- Windows Release 앱 build: 경고 0, 오류 0
- 수정 후 숨김 startup/process survival: 통과, stderr 없음
- `Doroti/eng/validate-fcr5-scroll.ps1`: 이전 단계에서 Debug/Release 계약 통과
- `Doroti/eng/validate-framework-shaders.ps1`: 통과
- 한 차례 pre-native-flush `WindowsLive Release`: 통과했으나 측정 경계가 불충분해 성능 근거로 폐기

마지막 변경 뒤 다시 실행하지 않았거나 실패한 항목:

- 최신 `validate-framework-shaders.ps1`: 이후 host 변경 뒤 미실행
- 최신 `validate-app-targets.ps1 -Shard WindowsLive -Configuration Release`: **28.287 ms로 실패**
- Windows 스크롤 중 연속 화면 캡처에 의한 흰 점멸 검출: 미실행

## 수동 평가 후 다음 후보 작업

1. 현재 빌드에서 시작 끊김, 하단 도달, 흰 점멸을 먼저 눈으로 평가한다.
2. 성능 작업을 계속한다면 `Dispatcher.DispatchDelayed` completion을 다음 `CompositionTarget.Rendering`에서 확정하도록 바꿔 native 제출과 UI 큐 대기를 분리한다.
3. 새 경계에서 `rasterStart → rasterEnd → compositor-present`를 각각 계산한다.
4. 16.6 ms를 넘는 샘플이 실제 native 제출 구간에 남을 때만 ANGLE frame pacing/backpressure를 다시 조정한다.
5. 스크롤 중 body 영역을 연속 캡처해 흰/투명 프레임을 자동 검출한다.
6. Release WindowsLive를 최소 두 번 연속 통과시키고, extent `0 ↔ max`, failed/dropped/superseded 0을 함께 확인한다.

## 실행 명령

```powershell
dotnet restore ./DorotiDemoApp/DorotiDemoApp.csproj -p:DorotiTarget=Windows --locked-mode
dotnet run --project ./DorotiDemoApp/DorotiDemoApp.csproj --no-restore -p:DorotiTarget=Windows
```

Release 자동 검증:

```powershell
./Doroti/eng/validate-app-targets.ps1 -Shard WindowsLive -Configuration Release
./Doroti/eng/validate-fcr5-scroll.ps1
./Doroti/eng/validate-framework-shaders.ps1
```

주의: `WindowsLive`는 실제 창을 표시하고 포커스를 준 뒤 mouse wheel 입력을 보낸다.

## 주요 변경 파일

- `Doroti/src/Doroti.Host.Maui/MauiHostAdapter.cs`
- `Doroti/src/Doroti.Host.Maui/DorotiMauiSurface.cs`
- `Doroti/src/Doroti.Host.Maui/MauiSkiaCapabilities.cs`
- `Doroti/src/Doroti.Host.Maui/MauiFrameworkHost.cs`
- `Doroti/src/Doroti.Host.Maui/MauiHostContracts.cs`
- `Doroti/src/Doroti.Skia.RuntimeEffects/DorotiSkiaImageFilterRenderer.cs`
- `Doroti/src/Doroti.Framework.Rendering/layer.cs`
- `Doroti/src/Doroti.Ui/FrameLifecycle.cs`
- `Doroti/src/Doroti.Ui/GraphicsAndSemanticsContracts.cs`
- `Doroti/eng/validate-app-targets.ps1`
- `Doroti/eng/validate-fcr5-scroll.ps1`

## 작업 트리 주의

작업 트리는 이 조사 전부터의 변경을 포함해 dirty 상태다. 관련 없는 변경을 되돌리거나 커밋하지 않았다. 이 문서 자체도 현재 새 파일이다.
