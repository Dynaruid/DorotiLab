# Mac Catalyst 실시간 창 리사이즈 떨림 수정 요약

- 기록일: 2026-08-27
- 대상: `maccatalyst-arm64` / UIKit `SKMetalView` / Metal-Skia
- 관련 커밋: `37d450f52373f9e15313aec84667ce9d61b98cec` (`fix Mac Catalyst live resize jitter`)
- 최종 판정: **상·하·좌·우 창 리사이즈의 실시간 추종과 위치 안정성을 사용자가 실제 창에서 확인해 PASS로 승인했다.**

## 1. 증상

초기 구현은 우측·상단 드래그에서는 렌더링이 창 크기를 실시간으로 따라오는 것처럼 보였지만, 좌측·하단 드래그에서는 반영이 늦고 렌더링 위치가 떨렸다. 첫 수정 뒤 전체 동작은 크게 개선됐으나 하단 드래그에서만 간헐적으로 화면이 아래로 한 프레임 내려간 것처럼 보이는 잔여 떨림이 있었다.

좌측·하단 resize는 크기뿐 아니라 native window origin도 함께 움직인다. 기존 경로에서는 UIKit layout, Metal drawable 크기 변경, Core Animation window geometry 표시가 같은 commit 경계에 있지 않았다. 또한 `LayoutSubviews`의 즉시 draw와 resize 중 활성화한 MTKView display-link draw가 동시에 존재해, 하단 origin 이동 중 별도 paint가 중간에 표시될 수 있었다.

## 2. 첫 수정: drawable과 창 geometry의 표시 동기화

`DorotiMacCatalystSkglViewHandler`의 전용 `SKMetalView`에 다음 변경을 적용했다.

- `AutoResizeDrawable = false`인 기존 ownership을 유지했다.
- `PresentsWithTransaction = true`를 설정해 Metal 표시를 Core Animation transaction에 결합했다.
- `DrawableSize`, `Layer.ContentsScale`, `Layer.ContentsGravity` 변경과 `Draw()`를 하나의 명시적 `CATransaction` 안에서 실행했다.
- transaction은 `try/finally`로 닫아 draw 실패 시에도 Core Animation 상태가 누수되지 않게 했다.
- `GravityTopLeft`와 `MasksToBounds`를 유지해 이전 drawable의 확대와 경계 밖 표시를 막았다.

이 수정으로 좌측·하단을 포함한 resize 추종과 위치 안정성이 크게 개선됐다. 사용자가 재확인한 결과 하단에만 간헐적인 수직 떨림이 남았다.

## 3. 두 번째 수정: resize 중 이중 paint 제거

Mac Catalyst surface는 MAUI `SizeChanged`를 받을 때 `HasRenderLoop = true`로 바꿔 display-link paint를 시작하고, 동시에 전용 Metal view의 `LayoutSubviews`에서도 정확한 drawable 크기로 즉시 `Draw()`하고 있었다.

두 경로를 함께 유지할 필요가 없으므로 resize 중 `HasRenderLoop` 활성화를 제거했다.

- live resize frame은 `LayoutSubviews`의 transaction 동기 draw가 단독으로 담당한다.
- drawable이 일시적으로 없었던 경우를 위해 마지막 size pulse로부터 150 ms 뒤 최종 `InvalidateSurface()` 한 번은 유지한다.
- resize target은 추정 logical size가 아니라 실제 Metal backend render target의 pixel width/height를 기준으로 계속 게시한다.

이 변경은 display-link의 독립 paint가 window-origin commit과 drawable-size commit 사이에 끼어드는 경로를 제거한다. 사용자는 같은 Mac Catalyst Demo에서 하단 드래그를 다시 확인한 뒤 결과를 "완벽"으로 승인했다.

## 4. 변경 파일

- `Doroti/src/Doroti.Host.Maui/DorotiMacCatalystSkglViewHandler.cs`
  - transaction 기반 drawable resize/presentation
  - layout callback의 synchronous Metal draw
- `Doroti/src/Doroti.Host.Maui/MauiSkiaSurface.cs`
  - Mac Catalyst resize용 `HasRenderLoop` 제거
  - 150 ms quiescence 뒤 final invalidation 유지

모든 변경은 `MACCATALYST` 조건부 경계 안에 있으며 Windows, Android, iOS, native AppKit surface의 resize 경로를 변경하지 않는다.

## 5. 검증 결과

다음 Mac Catalyst host 단독 build를 실행했다.

```sh
dotnet build Doroti/src/Doroti.Host.Maui/Doroti.Host.Maui.csproj \
  --configuration Release \
  -p:RuntimeIdentifier=maccatalyst-arm64 \
  --nologo
```

결과는 경고 0개, 오류 0개였다.

실제 Demo와 같은 Release/AOT 전체 graph도 실행했다.

```sh
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 build \
  -App ./DorotiDemoApp \
  -Platform maccatalyst \
  -Rid maccatalyst-arm64
```

결과는 경고 0개, 오류 0개였다. `git diff --check`도 통과했다.

사용자는 아래 실행 경로로 실제 창의 우측·상단·좌측·하단 resize를 확인했다.

```sh
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run \
  -App ./DorotiDemoApp \
  -Platform maccatalyst \
  -Rid maccatalyst-arm64
```

최종 visible acceptance는 사용자 물리 확인에 근거한 PASS다. 이번 작업에서 multi-display scale 전환, 외부 모니터, 다른 refresh rate, Intel Mac 또는 iOS 경로를 새로 검증한 것은 아니다.

## 6. 보존할 구현 원칙

- Catalyst live resize에서는 drawable size와 draw presentation을 같은 Core Animation transaction에 둔다.
- `LayoutSubviews`가 exact drawable을 동기 렌더한다면 별도 display-link resize loop를 동시에 시작하지 않는다.
- logical layout callback의 추정값보다 실제 drawable pixel extent를 resize epoch의 authority로 사용한다.
- live resize 종료 뒤에는 일시적 drawable 부재를 복구할 bounded final invalidation만 허용한다.
- 자동 build 성공과 사용자-visible resize acceptance를 구분해 기록한다.

> 문서 성격: 2026-08-27 Mac Catalyst 실시간 resize 진단, 두 단계 수정, build 검증과 사용자 visible acceptance를 보존하는 역사 기록이다. 새로운 active roadmap이 아니다.
