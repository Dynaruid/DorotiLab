# Web 한글·연속 스크롤·래스터·resize 수정

- 날짜: 2026-08-29
- 대상: `DorotiDemoApp` / `browser-wasm` / Chrome WebGL2
- 실행 명령: `pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform web`

## 원인과 변경

- WebAssembly Skia는 브라우저 CSS/system font를 font data로 열 수 없었다. 따라서 system fallback만으로는 한글 glyph를 찾지 못했다.
  - Google Fonts의 `NanumGothic-Regular.ttf`와 OFL 1.1을 Web host 정적 asset으로 포함했다.
  - host가 view 생성 전에 font bytes를 등록하고, `SkiaSceneRenderer`가 같은 등록 typeface를 text 측정과 raster에 함께 사용한다.
- 고해상도 trackpad wheel event를 매 event마다 managed framework에 전달해 동일 browser frame 안에서 layout/raster 요청이 중복됐다.
  - pixel/line/page delta를 logical pixel로 정규화하고, 한 browser rAF 동안의 delta를 누적해 한 번만 전달한다.
  - wheel, resize snapshot, framework animation callback은 host의 동일 latest-only rAF owner를 공유한다.
- `preserveDrawingBuffer=false`인 default framebuffer는 비동기 managed raster 사이 browser paint에서 보존을 가정할 수 없다.
  - exact front/staging FBO 이중 버퍼는 유지한다.
  - scheduled browser frame마다 이전 exact front를 paint 전에 1:1 재-blit하고, 새 staging raster는 완료 전까지 visible surface로 만들지 않는다.
- `ResizeObserver`가 관측한 backing size는 즉시 이전 exact front의 1:1 crop/background로 반영하되 managed snapshot은 shared rAF에서 latest generation만 전달한다. 연속 resize가 중간 generation을 불필요하게 build하지 않는다.

## 검증

- `dotnet build DorotiDemoApp/web/DorotiDemoApp.Web.csproj -c Release`: **PASS**, 경고 0, 오류 0.
- resize contract v4: **PASS**, 22/22 terminal, max queue depth 2, stale present 0.
- FCR-7 Material/widget runtime contract: **PASS**. 포함 font decode와 `한`, `글` glyph 존재를 검사한다.
- Chrome / Windows / DPR 2 / AMD Radeon 780M WebGL2:
  - `Type in English or 한국어`, 실제 입력 `한글 테스트`, helper `Entered: 한글 테스트`가 tofu 없이 보였다.
  - final runtime sample은 failed terminal 0, exact submitted 5, paint 전 `host-frame` stable-front refresh 4를 기록했다.
  - 연속 wheel 실입력 비교 표본에서는 약 1.1초 동안 baseline 35 front commit / 48 superseded에서 44 front commit / 41 superseded로 바뀌었다. 실입력 표본 수가 작으므로 cadence의 절대 성능 보증으로 사용하지 않는다.
  - 5단계 빠른 resize 표본에서 provisional front 반영은 3.9~10.8 ms, 최신 exact front 반영은 5.5~6.6 ms였다. 중간 generation은 superseded되고 failed terminal은 0이었다.

## 판정 경계

- 현재 Chrome 장비의 runtime/visible smoke는 **PASS**다.
- Firefox/Edge, 60~165 Hz matrix, 장시간 실제 trackpad 사용, browser compositor scan-out ACK, context loss/restore는 이번 실행에서 다 확인하지 않았으므로 **notVerified**다.
- GPU submit/rAF 완료를 display scan-out ACK로 승격하지 않는다.

## 포함 font 출처

- Google Fonts `ofl/nanumgothic/NanumGothic-Regular.ttf`, pinned repository commit `ade3d1533e06b2b1462ffcde8e08b129627ca360`
- TTF SHA-256: `76F45EF4A6BCFF344C837C95A7DCC26E017E38B5846D5AE0CDCB5B86BE2E2D31`
- OFL.txt SHA-256: `EEACF16032901D0ED0456876EC77B8F0FDA6B3FECEC7D972F8543EB602E6C30F`
