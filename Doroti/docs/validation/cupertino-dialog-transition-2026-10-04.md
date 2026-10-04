# Cupertino 다이얼로그의 블러와 내용 페이드

기존 구현은 다이얼로그 내용의 투명도를 줄이는 동안 블러 sigma와 채도도 별도로 줄였다. 약한 블러가 닫기 전환 후반까지 남아 내용보다 늦게 사라져 보일 수 있었다.

`CupertinoPopupSurface`는 Flutter처럼 sigma 30과 테마별 채도 필터를 유지한다. 기본 `CupertinoDialogRoute`는 블러와 내용을 하나의 `FadeTransition`으로 함께 페이드한다. 열기는 1.3 → 1.0 확대 전환을 함께 사용하고, 닫기는 페이드만 사용한다. 기존 spring, 기본 250ms 설정과 사용자 `transitionBuilder` 경로는 유지한다.

Skia 렌더러는 단일 `srcOver` backdrop으로 이어지는 opacity/offset/transform/clip/retained 경로에서 투명도를 backdrop 레이어의 최종 합성에 적용한다. 배경 필터 앞에 투명한 opacity 레이어가 생겨 배경을 읽지 못하는 문제를 피한다. 필터 앞에 별도 그림 형제가 있거나 다른 격리 효과·shader 필터가 있으면 기존 레이어 격리를 유지한다.

비교한 Flutter 원본:

- [CupertinoDialogRoute 전환](https://github.com/flutter/flutter/blob/master/packages/flutter/lib/src/cupertino/route.dart)
- [CupertinoPopupSurface 필터](https://github.com/flutter/flutter/blob/master/packages/flutter/lib/src/cupertino/dialog.dart)
- [BackdropFilterLayer의 합성 상태 상속](https://github.com/flutter/flutter/blob/master/engine/src/flutter/flow/layers/backdrop_filter_layer.cc)

| 검증 | 결과 | 범위 |
| --- | --- | --- |
| CPU 픽셀 | PASS | 투명도 0/.2/.5/.8/1의 단일·중첩 opacity. 완료된 블러+내용과 원래 배경을 같은 alpha로 섞은 기대값 대비 채널 오차 ≤2/255. |
| Vulkan GPU 픽셀 | PASS | AMD Radeon 780M Graphics의 Graphite/Vulkan에서 동일 검사. |
| 위젯 전환 | PASS | 포인터로 연 Builder-wrapped 다이얼로그. 밝은/어두운 테마의 열기·닫기 중간 프레임, 고정 sigma, 공유 fade, 닫기 scale 제거, route teardown. |
| Doroti.Tests 전체 | PASS | CPU 렌더링·입력·수명·필터·탐색 회귀 검사. |
| Android 빌드 | PASS | Sample2 Release, android-arm64, .NET 10 CoreClrJit. 오류 0개, 기존 CoreCLR 실험적 기능 경고 XA1040 1개. |
| Galaxy S25 실행 | PASS | SM-S931N, Adreno 830 Vulkan. 새 APK 설치, 앱 시작, ADB 입력으로 다이얼로그 열기·닫기, 화면 녹화의 중간 프레임 확인. |
| Flutter 실행 화면과 직접 비교 | notVerified | 동일 장면의 Flutter 영상과 픽셀 동등성 검사는 수행하지 않음. 소스 전환 방식과 Doroti 기대 합성을 검증함. |

재현 명령은 저장소 루트에서 실행한다. 각 검사에 20분 제한을 적용한다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Tests/Doroti.Tests.csproj -c Release -- --cupertino-dialog
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Tests/Doroti.Tests.csproj -c Release -r win-x64 -- --cupertino-dialog-gpu
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Tests/Doroti.Tests.csproj -c Release
```

원시 로그와 Galaxy 녹화는 로컬 `temp/testing/cupertino-dialog/20261004/`에 보관했다. 이 경로는 배포 패키지에 포함하지 않는다. Galaxy 입력 검사는 자동 입력이며 사용자 손가락 입력 검증을 뜻하지 않는다.
