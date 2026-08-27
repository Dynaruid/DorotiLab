# macOS AppKit 실시간 창 리사이즈 수정 요약

- 기록일: 2026-08-27
- 대상: `macos` / `osx-arm64` / AppKit `MTKView` / Metal-Skia
- 변경 파일: `Doroti/src/Doroti.Host.Maui/DorotiMacOSMetalView.cs`
- 최종 판정: **상·하·좌·우 창 리사이즈의 스트레칭과 좌측·상단 위치 떨림이 해소됐고, 클릭·스크롤 반응을 포함한 실제 창 동작을 사용자가 확인해 PASS로 승인했다.**

## 1. 증상

native AppKit 경로를 아래 명령으로 실행해 창 크기를 조절하면 이전 drawable이 새 bounds에 맞게 확대되는 스트레칭이 보였다.

```sh
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run \
  -App ./DorotiDemoApp \
  -Platform macos \
  -Rid osx-arm64
```

정확한 drawable 크기를 layout에서 직접 적용한 뒤 스트레칭은 개선됐지만, AppKit view/layer의 화면상 배치 변화가 두드러지는 좌측·상단 border drag에서는 렌더링 위치가 간헐적으로 틀어지며 떨렸다.

초기 동기화 시도에서는 `PresentsWithTransaction`을 계속 활성화했다. 그러나 이 AppKit surface는 표시용 command buffer를 직접 소유하므로 일반 클릭·스크롤로 발생한 frame이 화면에 반영되지 않아 렌더링이 멈춘 것처럼 보였다. 따라서 transaction presentation을 상시 사용하는 방식은 폐기했다.

## 2. 원인

문제는 edge별 좌표 계산이 아니라 AppKit window geometry와 Metal drawable 표시의 commit 경계가 달랐던 것이었다.

- 우측·하단 resize에서는 표시 시점 차이가 상대적으로 덜 드러났다.
- 좌측·상단 resize는 크기 변경과 top-left 기준의 window/view/layer 배치 차이를 더 직접적으로 노출했다.
- `Layout()`에서 Core Animation transaction을 열고 `PresentsWithTransaction = true`를 설정하더라도, `MTLCommandBuffer.PresentDrawable()`로 표시하면 drawable presentation이 그 transaction에 참여하지 않는다.
- 그 결과 새 AppKit geometry와 새 크기의 Metal frame이 서로 다른 compositor 시점에 보이며 위치 떨림이 발생할 수 있었다.

Apple의 `CAMetalLayer.PresentsWithTransaction` 계약에 따라 transaction presentation에서는 먼저 render command buffer를 commit하고 `WaitUntilScheduled()`로 queue ordering을 확보한 다음, drawable 자체의 `Present()`를 호출해야 한다. command buffer의 convenience present는 이 경로를 대신하지 않는다.

참고: <https://developer.apple.com/documentation/quartzcore/cametallayer/presentswithtransaction>

## 3. 최종 수정

### Exact drawable backing을 layout이 소유

- `AutoResizeDrawable = false`로 설정해 MTKView가 이전 drawable을 새 bounds에 자동 확대하지 않게 했다.
- `Layout()`에서 logical bounds와 backing scale을 이용해 pixel drawable 크기를 반올림해 계산한다.
- `DrawableSize`, `CALayer.ContentsScale`, published surface metrics와 draw를 같은 layout 경로에서 갱신한다.
- Retina backing scale 변경 시 같은 exact-backing layout을 다시 수행한다.
- layer는 항상 `TopLeft` gravity와 clipping을 유지한다.

### Live layout 표시만 Core Animation transaction에 결합

- `Layout()`에서만 명시적 `CATransaction`을 열고 implicit animation을 비활성화한다.
- transaction 안에서만 `PresentsWithTransaction = true`를 사용한다.
- Skia submit 뒤 shared Metal queue에 marker command buffer를 commit한다.
- resize frame은 marker가 scheduled될 때까지만 기다린 뒤 `CAMetalDrawable.Present()`로 현재 Core Animation transaction에 표시한다.
- transaction이 끝나면 `PresentsWithTransaction = false`로 즉시 복구한다.

### 일반 입력 frame 경로 보존

클릭·스크롤 등 layout 밖에서 발생하는 일반 frame은 기존 `MTLCommandBuffer.PresentDrawable()` 경로를 그대로 사용한다. 따라서 live resize 표시 동기화를 위해 UI thread에서 GPU 완료까지 기다리거나, 입력 기반 frame을 transaction presentation에 영구 종속시키지 않는다.

`DrawableSizeWillChange`가 layout 중 다시 frame을 요청하는 중복 경로도 억제해 같은 크기에 대한 불필요한 추가 paint를 막았다.

## 4. 폐기한 접근

좌측·상단 drag 방향을 감지해 `TopRight` 또는 `BottomLeft`처럼 반대편 edge에 layer를 동적으로 고정하는 방식은 최종 구현에 포함하지 않았다.

이 방식은 실제 presentation timing 문제를 좌표 보정으로 가리며, 이전 resize 작업에서도 stationary opposite-edge 보정이 안정적인 해법이 되지 못했다. 최종 코드는 edge와 무관하게 `TopLeft`를 유지하고 geometry와 drawable presentation의 transaction ordering을 직접 맞춘다.

## 5. 검증 결과

AppKit host 단독 Release build를 실행했다.

```sh
dotnet build Doroti/src/Doroti.Host.Maui/Doroti.Host.Maui.csproj \
  --configuration Release \
  -p:RuntimeIdentifier=osx-arm64 \
  --nologo
```

결과는 경고 0개, 오류 0개였다.

Demo와 동일한 macOS Release/AOT 전체 graph도 실행했다.

```sh
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 build \
  -App ./DorotiDemoApp \
  -Platform macos \
  -Rid osx-arm64
```

결과는 경고 0개, 오류 0개였으며 `git diff --check`도 통과했다.

사용자는 실제 Demo 창에서 다음 항목을 확인했다.

- resize 중 이전 frame의 스트레칭이 보이지 않는다.
- 좌측·상단을 포함한 네 방향 border drag에서 렌더링 위치가 안정적이다.
- 수정 뒤에도 클릭과 스크롤 기반 frame이 정상적으로 표시된다.

최종 visible acceptance는 사용자의 물리 확인에 근거한 PASS다. multi-display scale 전환, 외부 모니터, Intel Mac, 다른 refresh rate와 Mac Catalyst 또는 타 플랫폼 회귀를 이번 AppKit acceptance에서 새로 검증한 것은 아니다.

## 6. 보존할 구현 원칙

- `AutoResizeDrawable`을 끈 surface는 layout에서 drawable pixel extent와 backing scale을 함께 소유한다.
- 화면상 배치 변화가 드러나는 live resize는 layer anchor 추정이 아니라 window geometry와 drawable presentation의 commit ordering을 맞춰 해결한다.
- `PresentsWithTransaction`을 사용할 때 command-buffer convenience present와 drawable transaction present를 혼동하지 않는다.
- transaction presentation은 live layout에만 한정하고 일반 입력 frame의 비동기 command-buffer 표시를 유지한다.
- UI thread에서는 GPU completion까지 기다리지 않고 queue ordering에 필요한 scheduled 경계까지만 기다린다.
- 자동 build 성공과 사용자-visible resize acceptance를 구분해 기록한다.

> 문서 성격: 2026-08-27 native macOS AppKit 실시간 resize의 증상, 실패한 접근, 최종 표시 동기화 수정, build 검증과 사용자 visible acceptance를 보존하는 역사 기록이다. 새로운 active roadmap이 아니다.
