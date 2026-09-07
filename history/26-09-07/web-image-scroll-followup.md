# 이미지 구간 진입 및 idle 이후 스크롤 후속 검증

2026-09-07. 사용자는 어느 위치에서든 멈췄다가 다시 스크롤할 때 끊기며,
이미지 데모 상단 진입에서 특히 심하다고 확인했다. 이전 progress 체감 개선과
state/resize 결과를 이 문제의 PASS 근거로 사용하지 않는다.

## 원인과 변경

- 로컬 WebP는 3648×5472, 680,944 encoded bytes다. 표시 높이 240px에도
  원본 약 2천만 픽셀을 디코딩하던 경로를 확인했다. 스크롤마다 재디코딩하는
  결함은 재현되지 않았다. 방문한 구간의 probe에서는 widget rebuild가 없고
  Web semantics 갱신이 주요 동기 비용이었다.
- `instantiateImageCodecFromBuffer/WithSize`의 target-size 요청이 실제 픽셀 크기에
  반영되지 않던 공용 경로를 수정했다. 한 축만 지정하면 종횡비를 유지하고,
  확대 허용 여부를 반영한다. Skia host는 codec의 scaled decode를 사용한 뒤
  필요한 경우 정확한 크기로 resample한다. Web의 Task.Run을 별도 thread라고
  주장하지 않는다. 다른 image host의 기본 구현은 decode 후 rasterize이므로
  모든 backend에서 원본 중간 할당까지 제거했다고 주장하지 않는다.
  최종 Web 경로는 GetPixels로 scale을 검증한다. 해당 codec이 InvalidScale을
  반환하면 intrinsic decode 후 resample하므로 모든 이미지 포맷에서 축소
  중간 저장량을 보장하지 않는다. 기본 데모 WebP의 축소 지원은 별도 검증한다.
- `ResizeImage`의 portrait fit 정수 나눗셈, 동적 provider에 object key를
  전달하던 결함, 비동기 Action/result/completer의 키 완료 경로를 수정했다.
  동기 key는 동기로 유지하고 비동기 key와 오류도 전달한다.
- Image demo 표시용 key는 너비 1024×DPR의 ResizeImage다. DPR 1에서는
  1024×1536, RGBA 약 76.1MiB→6MiB로 약 92.1% 감소한다. DPR 2에서는
  2048×3072, 24MiB다. Contain/Cover는 동일 key를 사용한다.
  명시적인 Extract colors는 원본을 별도로 디코딩하므로 추출 이후 전체
  image cache가 항상 6MiB라는 의미는 아니다. 원본 색상 입력을 바꾸지 않는다.
- geometry-only semantics는 재사용 writer/buffer로 고정 필드를 직접 기록한다.
  node마다 anonymous DTO/rect array/serializer traversal을 만들지 않는다.
  full snapshot, DOM content retention, JSON cache 상한은 유지한다.
  이 할당 감소의 end-to-end 시간 개선은 아직 입증되지 않았다.

## 성능 증거와 수용 상태

source runner 5088, 기본 worker-direct-webgl, Chromium hardware/headless,
1280×900 DPR 1, gutter x=1255, wheel 40px, idle 5초. benchmark는
build/다른 benchmark와 겹치지 않게 순차 실행했다. 상세 ON profile에는
큰 native trace snapshot의 수집 비용도 있으므로 실제 입력/표시 지연과 구별한다.

아래 3회는 배포 호환 수정 전의 소스 측정이다. 실제 이미지 픽셀 표시는
확인했지만, 이를 배포 호환 수정 뒤의 재측정으로 바꿔 표시하지 않는다.

| 지표 | 이전 1회 | 소스 1 | 소스 2 | 소스 3 |
| --- | ---: | ---: | ---: | ---: |
| 첫 이미지 위젯 생성 window callback max | 201.7ms | 216.1ms | 230.8ms | 209.1ms |
| idle 재시작 input→새 scene commit | 14.4ms | 29.2ms | 29.7ms | 19.6ms |
| idle 재시작 window callback max | 54.5ms | 48.3ms | 47.3ms | 64.4ms |

최소 계측 OFF+input marker의 별도 1회는 첫/연속/복귀/재시작
22.5/29.7/41.8/27.6ms였다. callback 및 첫 위젯 생성 비용은 OFF에서
notMeasured다. ON/OFF corpus를 합치거나 OFF 1회를 3회 hard gate PASS로
분류하지 않는다. endpoint는 scene commit 알림이며 scan-out이나 물리 체감이 아니다.

**결론: 픽셀 작업/저장량은 감소했지만 첫 구간 진입 및 재시작 지연의 개선은
입증되지 않았다. 전체 수용은 PARTIAL이다.** 첫 이미지 구간의 위젯 생성 비용과
semantics 전체 snapshot 비용은 남아 있다. 이전 strict resize FAIL도 유지한다.
사용자 장치의 wheel/trackpad, 실제 창 resize, 120Hz와 시각/접근성 수용은 notVerified다.

## 최초 실패 보존

- 이미지 영역 탐색의 최초 두 실행은 버튼 y<780 조건 때문에 실패했다.
  실제 버튼은 y≈818에 있었다. 탐색 조건을 viewport 경계에 맞춘 뒤 재실행했다.
  `scroll-restart-image-before`, `scroll-image-outer-before` 로그를 보존한다.
- `after-image-sized` 중간 실행은 ResizeImage 로딩 완료가 보장되지 않아 성능
  개선 corpus에서 제외했다. native mount의 `Decodes=0` FAIL과 provider key
  timeout, typed key 전달의 RuntimeBinder 실패가 이를 드러냈다.
  `image-native-sample`, `image-sizing-provider*` 로그를 지우거나 PASS로 바꾸지 않는다.
- 회귀 test 초기 작성 중 loadImage overload 모호성 compile FAIL도 보존했다.
- 최초 trimmed publish는 이미지 진입 DPR 1/2에서 2 FAIL, 상태 유지 1 PASS였다.
  `[MONO] ...aot-runtime-wasm.c:188` 종료를 `image-scroll-browser-publish`에
  보존했다. 일시적 단계 로그에서 GetScaledDimensions 호출 직전까지 실행되고
  직후 로그가 없음을 확인했다 (`image-scroll-runtime-probe`). Web은 이 호출
  대신 GetPixels의 반환값으로 크기 지원 여부를 확인하도록 수정했다.
  진단용 Console 로그는 최종 소스에서 제거했다.

## 검증과 재현

모든 test/command는 `.github/copilot-instructions.md`에 따라 20분 timeout,
retry 0으로 실행했다. 로그는 `Doroti/artifacts/framework-web-work2/`, 원본
측정은 `Doroti/validation/web-playwright/artifacts/scroll-start/`에 있다.

```powershell
dotnet run --project Doroti/validation/fcr7-material-widget -c Release -- --image-sizing
dotnet run --project Doroti/validation/fcr7-material-widget -c Release -- --windows-sample
# Doroti/validation/web-playwright에서
node measure-scroll-start.mjs image-entry 1255 40 --image
node measure-scroll-start.mjs image-minimal 1255 40 --image --minimal
npx playwright test tests/image-scroll-regression.spec.ts tests/state-resize-regression.spec.ts tests/material-sample-motion.spec.ts
```

- native sizing: 실제 codec 축소/종횡비/exact/clamp/upscale, alpha pixel round trip,
  fallback release, portrait ResizeImage.fit, 동기/비동기 key와 오류 전달 PASS.
- native mounted sample: 표시 decode 1회, 명시적 원본 decode 1회, 스크롤 시
  반복 decode/색상 추출 없음. picker/text/app bar/drawer 전체 회귀 PASS.
- 배포 호환 수정 전 source browser: 이미지 DPR 1/2, progress pixels/stop,
  navigation resize, selection/theme/DOM retention 5 PASS.
- 배포 호환 수정 후 native sizing 재검증 PASS. Web Release trimmed publish PASS.
  publish 기본 direct의 이미지 DPR 1/2 및 상태 유지 3 PASS, 명시적
  worker-canvaskit-webgl 이미지 표시/fit/idle scroll 1 PASS.
- `WasmBuildNative=true`로 source native relink까지 복구한 뒤 5088을 재시작했다.
  임시 5091/5092 서버는 종료했다. 별도 package 앱과 Maui/Qt/모바일 장치 실행은
  이번 수정에 대해 notVerified다.
- 최종 파일/빌드 fingerprint: `Doroti/artifacts/framework-web-work2/image-scroll-fingerprint.json`.

배포 호환 수정 후 5088 source의 최소 계측 1회는 첫/연속/복귀/재시작
27.6/40.1/33.3/24.7ms였다 (`final2-image-minimal.json`). 첫 생성 callback은
재계측하지 않았고 실제 scan-out도 측정하지 않았다. 이 결과는 PARTIAL 판정을
변경하지 않는다.
