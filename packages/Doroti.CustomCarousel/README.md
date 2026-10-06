# Doroti.CustomCarousel

[flutter_custom_carousel](https://github.com/gskinnerTeam/flutter_custom_carousel)의 스크롤·항목 배치 구조를 Doroti C# 위젯으로 옮긴 패키지입니다.
`effectsBuilder(index, scrollRatio, child)`에서 Doroti의 `Transform`, `Opacity`, `ImageFiltered` 등을
조합합니다. `scrollRatio`는 선택 항목에서 0이며 앞/뒤 표시 범위에 따라 -1/+1로 정규화됩니다.
입력 축과 실제 화면 배치는 서로 독립적입니다.

```csharp
using Doroti.CustomCarousel;
using Doroti.Framework.Painting;
using Doroti.Framework.Widgets;
using Doroti.Ui;

new CustomCarousel(
    children: cards,
    scrollDirection: Axis.horizontal,
    alignment: Alignment.center,
    loop: true,
    itemCountBefore: 2,
    itemCountAfter: 2,
    depthOrder: DepthOrder.selectedInFront,
    effectsBuilder: (_, ratio, child) => Transform.CreateTranslate(
        offset: new Offset(ratio * 425, 0), child: child));
```

`CustomCarouselScrollController`는 `initialItem`, `selectedItem`, `jumpToItem`,
`animateToItem`, `nextItem`, `previousItem`을 제공합니다. 비동기 항목 이동은 `Task`를 반환합니다.
컨트롤러를 전달한 호출자는 위젯의 `dispose()`에서 컨트롤러도 해제해야 합니다.
내부에서 생성된 컨트롤러는 캐러셀이 해제합니다. 한 컨트롤러에는 캐러셀 하나만 연결하세요.

- 루프 이동은 가장 가까운 방향을 선택하며 음수·큰 인덱스도 순환합니다.
- 비루프 항목 이동은 처음/마지막 항목으로 제한됩니다.
- `CustomCarouselScrollPhysics(sticky: true, stiffness: 1)`로 fling당 최대 한 항목 이동을 설정합니다.
- `onSelectedItemChanged`는 가장 가까운 항목이 바뀔 때 호출됩니다.
- `onSettledItemChanged`는 이동 시작에 `null`, 정지 시 해당 인덱스를 전달합니다.
- `depthOrder`는 순방향, 역방향, 선택 항목을 맨 앞에 놓는 세 가지 겹침 순서를 지원합니다.
- `scrollSpeed`, `reverse`, `scrollBehavior`, `tapToSelect`, repaint/semantic 옵션을 제공합니다.
- 캐러셀에는 스크롤 축 방향으로 유한하고 양수인 크기가 필요합니다. `Expanded`나 `SizedBox`로 감싸세요.
- 마우스 드래그를 사용하려면 `ScrollBehavior.copyWith(dragDevices: ...)`에 `PointerDeviceKind.mouse`를 추가합니다.
- 휠 입력은 하나의 연속 스크롤로 누적하고 마지막 입력 후 120ms에 한 번 스냅합니다.
  작은 정밀 입력은 바로 반영하며 큰 휠 입력은 입력마다 재시작하지 않는 틱커로 보간합니다.
  시간상수 12ms의 응답을 사용하고 입력이 계속 들어와도 매 프레임 최신 목적지로 진행합니다.
  버튼 이동·드래그·위젯 해제는 휠 보간과 정지 타이머를 취소합니다.
- 가로 캐러셀은 세로 휠도 처리합니다. 원래 축의 입력만 받으려면 `verticalWheelScroll: false`로 설정합니다.
  안쪽 Scrollable이 이미 처리한 입력은 중복 처리하지 않습니다.
- 루프의 내부 스크롤 좌표는 입력 중 연속으로 유지합니다. 표시 위치와 항목 인덱스만 순환시키고 정지 후 좌표를 정규화합니다.
- 선택 항목이 바뀌어도 탭/포인터 래퍼 구조를 유지합니다. 효과 빌더도 위젯을 삽입·제거하는 대신
  `ImageFiltered.enabled`, `Opacity.opacity`, 변환 값 등을 바꾸면 카드와 이미지 상태를 보존할 수 있습니다.

`flutter_animate`와 그 `effectsBuilderFromAnimate` 어댑터는 의존하지 않습니다.
샘플은 효과를 Doroti 위젯으로 직접 계산합니다. 원본의 모든 효과·전환을 픽셀 단위로 복제하는 API는 아닙니다.
Doroti의 friction simulation에 무한 시간을 전달하면 NaN이 생길 수 있어,
스냅 목적지는 유한 시간에서 최대 30회 평가하여 계산합니다.

실행 앱은 [DorotiCarouselApp](../../samples/DorotiCarouselApp/README.md)입니다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project packages/Doroti.CustomCarousel/tests/Doroti.CustomCarousel.Tests -c Release -- temp/testing/carousel
```

회귀 검사는 루프 경계·음수 인덱스·최단 경로, 탭 hit testing, 합성 포인터 drag/fling,
wheel 스냅·역방향 입력, 크기 변경, 항목 수 변경, 컨트롤러 교체/해제를 확인합니다.
연속 휠의 되감김·이동량 손실, 마우스/트랙패드의 루프 경계 delta, 큰 휠 입력 보간,
버튼·트랙패드 입력의 타이머 취소, trackpad/mouse 휠 packet 종류 보존과 unmount 회귀 10개도 기본 검사에 포함됩니다.
이 입력 검사만 실행하려면 `-- --continuous-input temp/testing/carousel-input`을 전달합니다.
첫 입력 프레임 진행·연속 보간의 지연 상한, 선택 전환 중 이미지 픽셀/위젯 수명,
홈 블러·덱 뒤집기의 이미지 경계 유지 검사 4개도 기본 검사에 포함됩니다.
이 검사만 실행하려면 `-- --presentation-input temp/testing/carousel-presentation`을 전달합니다.
다섯 데모의 실제 위젯 경로를 통해 화면 이동·이미지 상세·토핑 선택·셔플을 검사하고 PNG를 저장합니다.
이 검사는 CPU 렌더링/합성 입력이며 물리 입력이나 디스플레이 scanout 검증은 아닙니다.

Windows 호스트의 입력/프레임 순서 회귀도 별도로 실행할 수 있습니다. Windows Release 샘플 빌드 후 실행하며,
큰/작은 native 휠 burst 사이의 재개 응답과 완료 프레임의 입력 취소를 검사합니다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 python packages/Doroti.CustomCarousel/tests/windows-scroll-smoke.py
```

시작 지연의 실제 사용자 확인과 자동 검사 범위는 [검증 기록](../../samples/DorotiCarouselApp/docs/validation.md)에 있습니다.

원본 프로젝트: [gskinnerTeam/flutter_custom_carousel](https://github.com/gskinnerTeam/flutter_custom_carousel).
Copyright (c) 2024, gskinner.com, inc. BSD 3-Clause 조건은 [LICENSE](LICENSE)에 보존했습니다.
