# 버튼 호버 텍스트 떨림: 공용 Skia 캐시 좌표 수정

사용자 관찰: DorotiTestbedApp의 Windows와 Web에서 버튼·탭에 마우스를 올리면 해당 글자가 미세하게 떨림.

## 원인과 수정

`SkiaSceneRenderer.DrawPictureLayer`는 캐시 생성 시 변환된 bounds의 소수점 원점을 전부 빼고,
완성된 이미지를 다시 소수점 좌표에 그렸다. 직접 그릴 때와 캐시 승격·재사용 때의 글리프 raster phase 및
이미지 sampling 위치가 달라졌다. 호버 repaint는 이 경로 전환을 드러낸다.
Windows와 Web `worker-direct-webgl`이 이 렌더러를 공유한다.

- 캐시 surface 원점을 device 좌표의 floor로 맞추고 내부 그림에는 원래 소수점 phase를 보존한다.
- 이미지 재생은 정수 device 좌표에서 수행한다. fractional phase는 cache signature에 포함한다.
- 같은 phase의 정수 이동은 재사용하고, phase가 변하면 직접 그린다. phase가 안정된 뒤 승격하여
  소수점 스크롤 중 매 프레임 새 캐시를 만들지 않는다.
- 기존 warm-up 수명, 승격 예산, LRU 및 context invalidation을 유지한다.
- 공통 TextPainter의 색상 변경 전용 재배치에서는 geometry 변화를 재현하지 못했고 수정하지 않았다.

## 수정 전 실패 보존

- `.doroti/text-hover-before.log`, `.doroti/text-hover-before.err`: GPU에서 같은 텍스트·좌표의 두 번째
  프레임 캐시 승격 직후 RGBA 780 bytes 차이로 FAIL.
- `Doroti/validation/web-playwright/artifacts/wrapper/text-hover-direct-dpr/playwright.stdout.log`:
  실제 sample Elevated 버튼의 hover 진입·이탈, 중간 및 안정 프레임 검사에서 4/4 FAIL.
  DPR 1 / 1.25 / 1.5 / 2의 최대 수평 명암 가중 중심 변화는 각각
  0.1409 / 0.2122 / 0.2121 / 0.5000 CSS px. 검사 기준은 0.1 CSS px 미만이다.
- 최초 DPR 1 안정 프레임만 검사한 실행은 PASS였다. 중간 프레임을 포함하자 재현되었으며,
  최초 PASS로 전체 호버 문제를 부정하지 않는다. 원본 로그·trace·video는 각 artifact label로 보존한다.

## 수정 후 검증

모든 테스트 프로세스에는 저장소 지침의 20분 timeout을 적용했다.

- `dotnet run --project Doroti/validation/fcr7-material-widget -c Release -- --text-hover`: PASS.
  실제 paint snapshot의 width/height/offset/line baseline을 색상 변경 12회, 좌/중앙/우 정렬 및
  유한/무한 제약에서 비교한다. GPU에서는 DPR 1/1.25/1.5/2, warm-up/승격/hit,
  정수 이동/소수점 phase 변경/음수 위치의 40개 프레임 모두 직접 그린 GPU reference와 bytes가 일치한다.
  캐시 hit도 요구하므로 캐시를 사용하지 않는 우회로 통과할 수 없다.
  로그: `.doroti/text-hover-final.log`, `.doroti/text-hover-final.err`.
- `--raster-budget`: PASS. 승격 예산, translation 재사용, metadata 제한/만료 및 context 해제.
  로그: `.doroti/text-hover-raster-budget.log`.
- `Doroti/eng/test-material-sample.ps1 -Suite AppBarRaster`: PASS. 420개 중간 스크롤 프레임의
  앱바 CPU/GPU 비교 및 cache pressure/reuse 회귀 포함.
  증거: `.doroti/evidence/material-sample-AppBarRaster-20260907-215533-ef71ce5b/`.
- Web direct 수정본 재빌드 후 `tests/text-hover.spec.ts`: **4/4 PASS**.
  증거: `Doroti/validation/web-playwright/artifacts/wrapper/text-hover-direct-fixed/` 및
  `Doroti/validation/web-playwright/artifacts/text-hover-direct-fixed/`.
- Web CanvasKit의 동일 네 배율 검사: **4/4 PASS**, 해당 경로에서는 수정 전부터 미재현.
  증거: `Doroti/validation/web-playwright/artifacts/wrapper/text-hover-canvaskit-dpr/`.
- DorotiTestbedApp Windows Release 빌드: PASS, warnings/errors 0.
  로그: `.doroti/text-hover-windows-build.log`. Web Release 빌드도 warnings/errors 0.

## 범위와 남은 확인

Web 검사는 실제 Chromium hardware 경로의 sample Elevated 버튼 픽셀을 측정한다.
색상·배경 변화가 중심 계산에 영향을 덜 주도록 명암을 정규화하며, 허용치 미만의 변화가
완전한 픽셀 일치라는 뜻은 아니다. Windows GPU reference는 offscreen D3D12 fixture다.
물리 화면에서의 사용자 재확인, Windows Vulkan presenter의 실제 hover 관찰,
모든 버튼·탭 조합 및 스크롤 성능 재측정은 `notVerified`다. 이 수정으로 기존 resize/성능 gate를 승격하지 않는다.
