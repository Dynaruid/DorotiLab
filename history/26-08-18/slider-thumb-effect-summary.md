# Doroti Slider thumb 상호작용 이펙트 잘림 수정 완료 요약

- 기록일: 2026-08-18
- 상태: **공용 원인 수정, 자동 회귀 검증 및 사용자 수동 확인 완료**
- 범위: Dart→C# lowerer, `Doroti.Ui`, `Doroti.Framework.Material`, FCR-7 Material/widget 계약, `DorotiDemoApp`

## 해결한 문제

`DorotiDemoApp`의 Slider thumb가 hover 또는 drag 상태일 때 표시하는 원형 overlay가 트랙 시작점
부근에서 간헐적으로 반쪽만 보이듯 잘렸다. MAUI surface나 native clip을 국소적으로 넓혀야 하는 문제가
아니라, Flutter의 `Size.fromRadius(radius)` named factory가 C#으로 잘못 번역된 공용 의미 결함이었다.

Flutter에서 `Size.fromRadius(24)`는 반지름 24인 원을 담는 `48×48` 크기다. 기존 Doroti 산출물은 이를
한 인자 `new Size(24)`로 낮춰 `24×24`만 layout preferred size로 보고했다. Slider track은 이 잘못된
크기를 기준으로 끝 여백을 계산했지만 painter는 반지름 24 overlay를 그렸으므로, thumb가 가장자리에
있을 때 이펙트의 바깥 절반이 layout/clip 경계 밖으로 나갔다.

## 적용 내용

- `Doroti.Ui.Size.fromRadius(double)`가 양 축에 지름 `radius * 2`를 반환하도록 Dart UI 계약을 추가했다.
- lowerer의 external static factory 대상에 `Size`를 추가해 `Size.fromRadius(...)`와 같은 named factory
  identity를 보존한다.
- 현재 reviewed Material 산출물의 Slider overlay/thumb/tick 크기를 `Size.fromRadius(...)`로 바로잡았다.
- 동일한 오역을 가진 RangeSlider thumb/tick과 Material Switch pressed thumb 크기도 함께 교정했다.
- DemoApp 전용 padding이나 clip 완화는 추가하지 않았다. 이후 framework 재생성에서도 같은 의미가
  유지되도록 pinned Flutter source를 실제 lowerer에 통과시키는 회귀 fixture를 FCR-7에 추가했다.

## 회귀 계약

- `Size.fromRadius(24)`는 `48×48`이다.
- 기본 Slider overlay의 preferred size는 `48×48`이다.
- 기본 Slider와 RangeSlider thumb는 `20×20`, track height 4의 tick은 `2×2`이다.
- pinned Flutter `slider_value_indicator_shape.dart`를 변환한 결과는
  `Size.fromRadius(this.overlayRadius)`를 유지하며 `new Size(this.overlayRadius)`로 축약되지 않는다.
- FCR-7 fixture는 Slider의 `idle → hovered → dragged → released` 상태와 hover/down/move/hold/up
  입력 시퀀스를 추적한다.

## 검증 결과

- `pwsh -NoProfile -File .\Doroti\eng\doroti.ps1 validate -ValidationSuite Fcr7`: `PASS`
- `pwsh -NoProfile -File .\Doroti\eng\doroti.ps1 validate -ValidationSuite Developer`: `PASS`
- Developer 제품 빌드: 경고 0, 오류 0
- `git diff --check`: `PASS`
- 사용자 수동 확인: Slider thumb hover/drag 이펙트가 잘리지 않고 정상 표시됨

## 남은 검증 경계

이번 문제는 사용자 수동 확인까지 완료했지만, 다음 항목은 별도 증거를 새로 만들지 않았으므로 기존
FCR-7 상태대로 `notVerified`를 유지한다.

- pinned Flutter와 Doroti의 paired raster/pixel differential
- Windows native live 자동 hover/drag capture
- Android physical 전용 Slider hover/drag 시나리오
- MacCatalyst native 실행

자동 geometry/lowerer 계약과 사용자 확인은 이 결함의 완료 근거지만, 실행하지 않은 대상의 전체 Material
visual parity로 승격하지 않는다.

## 주요 변경 영역

- `Doroti/src/Doroti.Ui/ViewContracts.cs`
- `tools/Doroti.DartToCSharp/src/Backend/CSharp/Lowering/FrameworkCSharpLowerer.Types.cs`
- `Doroti/src/Doroti.Framework.Material/slider_value_indicator_shape.cs`
- `Doroti/src/Doroti.Framework.Material/slider_parts.cs`
- `Doroti/src/Doroti.Framework.Material/range_slider_parts.cs`
- `Doroti/src/Doroti.Framework.Material/switch.cs`
- `Doroti/validation/fcr7-material-widget/`
- `Doroti/eng/validate-fcr7-material-widget.ps1`
- `Doroti/validation/evidence/flutter-conformance/fcr7-material-widget-evidence.json`
