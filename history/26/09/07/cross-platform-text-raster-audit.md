# 다른 플랫폼의 텍스트 캐시 점검

2026-09-07. [버튼 호버 수정](text-hover-raster-phase-fix.md)에 이어 사용자가 다른 플랫폼의 잠재적 문제도 수정하도록 요청했다.

## 현재 소스에서 확인한 적용 경로

| 플랫폼/호스트 | 그림 raster cache | 이번 공용 수정 적용 |
| --- | --- | --- |
| Windows WindowsAppSdk / MAUI | 사용 | 적용 |
| Android MAUI | 사용 | 적용 |
| Linux Qt/OpenGL | 사용 | 적용 |
| Web direct / document / Skia offscreen | 사용 | 적용 |
| macOS AppKit / Mac Catalyst / iOS | 사용 안 함 | 공용 코드는 포함되지만 해당 캐시 경로를 실행하지 않음 |
| Web CanvasKit | 별도 구현, 해당 SkiaSharp 그림 이미지 캐시 없음 | 이번 결함 경로 없음 |

근거: `DorotiWindowsAppSdkRunner`, `MauiSkiaCapabilities`의 플랫폼 전처리 분기,
`DorotiQtRunner`, `BrowserSkiaCapabilities`, `doroti.canvaskit.worker.ts`.
Apple의 기존 캐시 비활성화는 Metal snapshot 관련 기존 사유를 유지했다.
CanvasKit runtime image-filter offscreen 원점도 floor/ceil device bounds를 사용함을 확인했다.

## 추가로 재현하고 수정한 결함

`SkiaSceneRenderer`가 캐시 surface 생성 시 대상 surface의 `SKSurfaceProperties`를 전달하지 않았다.
`UseDeviceIndependentFonts`가 설정된 화면에서는 직접 그리기와 캐시 승격 때 다른 글꼴 raster policy가 적용된다.
175% 배율·BottomLeft·RGBA에서 두 번째 프레임에 **3546 RGBA bytes가 달라지는 FAIL**을 재현했다.
이는 특정 물리 장치에서 해당 설정이 사용된다는 주장이 아니라, 공유 렌더러의 지원 가능한 surface 구성에서 재현한 결함이다.

- 캐시 surface에 대상의 글꼴 설정과 pixel geometry를 전달한다.
- 두 설정을 캐시 signature에 포함하여, 같은 picture라도 다른 surface policy의 raster를 재사용하지 않는다.
- 앞선 소수점 phase 보존, 정수 이동 재사용, 승격 예산과 context invalidation은 유지한다.
- 앱의 텍스트 위치나 플랫폼별 font size를 보정하는 변경은 없다.

## 검증

모든 프로세스에 20분 timeout 적용.

- `--text-platforms`: **180 GPU 프레임 PASS**. DPR 1.75/2/2.625/3/3.5, TopLeft/BottomLeft,
  RGBA/BGRA, 15도/90도 회전, 수평 반전, fractional picture bounds, 정수/소수점/음수 이동.
  동일 picture를 서로 다른 글꼴 설정의 surface 사이에서 재사용하여 signature 무효화도 확인한다.
  직접 그리는 별도 GPU surface와 비교하고, 실제 캐시 hit 및 비어 있지 않은 글자 픽셀을 요구한다.
- 일반 글꼴은 byte-exact. 기기 독립 글꼴은 중간 surface의 coverage 반올림을 고려하여
  채널 오차 **2/255 이하**, 글자 명암 중심 변화 **0.01 device px 이하**를 각각 요구한다.
  최종 관측 최댓값은 **2/255 및 0.0019764 device px 미만**이다. 완전한 픽셀 일치로 표현하지 않는다.
- 초기 strict 비교에서 속성 전달 수정 후 262.5%의 6 bytes/최대 1, BGRA 200%의 최대 2 차이도
  FAIL로 남겼다. 허용치 변경은 위치 이동과 coverage 반올림을 분리한 것이며, 초기 3546-byte 실패를 삭제하지 않았다.
- `--text-hover`: 기존 100/125/150/200%의 40 GPU 프레임 byte-exact PASS. 색상만 바꾸는
  공통 TextPainter의 실제 paint snapshot geometry 검사도 PASS.
- `--raster-budget`: 승격 예산, 정수 이동 재사용, metadata 제한/만료 및 context 해제 PASS.
- Linux runner Release managed cross-build PASS, warnings/errors 0.
- Android arm64 Release 빌드/패키징 PASS, warnings/errors 0. 장치 실행 검증은 아니다.
- 수정된 공용 렌더러로 Web direct를 재빌드한 뒤 네 배율 호버 검사 **4/4 PASS**.
  로그: `Doroti/validation/web-playwright/artifacts/wrapper/text-platforms-direct/`.

증거는 `.doroti/text-platforms-*` 로그에 보존했다. 주요 항목:

- `text-platforms-props-before`: surface policy 미전달 재현 FAIL.
- `text-platforms-props-fixed`, `text-platforms-props-delta`, `text-platforms-final`: 중간 strict FAIL.
- `text-platforms-policy-switch`: 최종 180프레임 PASS 및 측정값.
- `text-platforms-hover`, `text-platforms-regression`, `text-platforms-linux`: 기존 회귀 및 Linux 빌드.
- `text-platforms-android`: Android arm64 Release 빌드/패키징.

## 검증 범위

GPU 확장 검사는 Windows의 offscreen D3D12 fixture에서 surface 조건을 바꾼 것이다.
Android/OpenGL·Linux/Qt·Apple/Metal 장치의 실제 화면 검증을 대신하지 않는다.
Apple native build/run, 모든 플랫폼의 물리 화면·입력·접근성 및 성능 재측정은 `notVerified`다.
기존 Apple 캐시 비활성화 사유와 resize/성능 gate도 그대로 남긴다.
