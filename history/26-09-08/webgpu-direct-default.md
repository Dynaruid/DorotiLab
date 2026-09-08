# Direct WebGPU default — 2026-09-08

후속 작업: [브라우저 타이머 수정](browser-owner-timers.md)이 아래 TimerQueue
실패에 대한 수정과 재검증을 기록한다. 아래 표와 원본 로그는 수정 전 결과로 보존한다.

사용자 요청에 따라 제품 렌더러는 `worker-direct-webgpu`와
`worker-direct-webgl`만 남겼다. 기본값, `auto`, 미지원/폐기 URL 값은
`worker-direct-webgpu`를 선택한다. 초기화 실패 시 WebGL로 자동 전환하지 않는다.
구현은 완료했으며, 확대 브라우저 검증에서 발생한 .NET TimerQueue 예외 때문에
전체 안정성 수용은 **PARTIAL**이다.

기준 HEAD: `6423825205354d260b4517ea0051fd14eddf0d7d`. 작업 시작 시 clean worktree였다.

## 구현

- Graphite/Dawn WebGPU 후보를 `.doroti/work3/evidence/*-sources`에서 복구했다.
  기존 manifest의 SHA-256과 일치하는 내용을 사용했다. 텍스트의 LF/CRLF 차이만
  정규화해 검증하고, 현재 소스에 필요한 변경만 옮겼다. CanvasKit, 폐기된
  document renderer, 병렬 layout 실험은 복구하지 않았다.
- `offscreen-worker`의 ImageBitmap 생성·전달·receipt·bitmaprenderer 표시 경로와
  해당 모드의 public union/manifest를 제거했다. 두 direct 경로는 같은 visible
  OffscreenCanvas와 공용 입력/semantics/lifecycle을 사용한다. 남아 있는 bitmap
  진단 카운터는 기존 진단 소비자를 위해 항상 0을 보고한다.
- 기본 runtime 위치는 `main`이다. Testbed와 생성 템플릿은 threads 및
  main-owned runtime의 JSWebWorker 렌더 역할을 사용한다. 명시적 WebGL의
  독립 Worker runtime 부팅 지원은 유지한다. 렌더러와 runtime 위치는 별도 설정이다.
- 공용 GPU surface 생성과 picture/image-filter cache를 Ganesh/Graphite recorder
  소유권에 연결했다. WebGPU canvas는 Graphite의 destination sampling/copy에 필요한
  texture usage를 명시한다.
- 정상 종료 시 queue 완료뿐 아니라 Skia cache 반환이 시작하는 비동기 mapping을
  각 해제 단계 사이에 기다린다. recorder/context/device는 그 후 순서대로 해제한다.
- device loss로 GPU mapped ArrayBuffer가 detach된 경우, Doroti의 빌드 시점 Dawn
  adapter가 불가능한 copy-back만 생략하고 WASM staging allocation을 해제한다.
  native map 실패 callback/진단은 유지한다. NuGet 캐시와 생성 runtime 파일은 수정하지 않았다.
- Runner SDK는 SkiaSharp의 native JS library 다음에 adapter를 한 번 연결하고,
  adapter 파일을 native link 입력으로 추적한다. source-only 변경으로 relink가
  수행되는 것도 확인했다.
- Worker protocol은 v3으로 변경했다. 종료된 host의 GPU 오류는 화면과 document
  진단 속성에 남는다. WebGPU가 없는 환경에서 조용히 다른 렌더러로 실행하지 않는다.
- README, ADR, target/loader manifest, public 타입, 템플릿 및 검증을 동기화했다.

## 검증

각 build/test 프로세스의 외부 timeout은 1,200초, Playwright test timeout도
1,200초다. 자동 retry는 0이다. expectation의 개별 대기 시간은 별도다.

| 검사 | 결과 |
| --- | --- |
| threaded Release native publish | PASS |
| Host 및 Playwright TypeScript 선언 검사 | PASS |
| mapped range 정상 copy-back 및 실제 ArrayBuffer detach 후 해제 | 2/2 PASS |
| FCR-7 Material/widget runtime 계약 | PASS, Debug |
| runtime shader 계약 | PASS, Debug |
| Host/Target/Runner SDK/Templates NuGet pack | 4개 PASS |
| 배포·패키지 내 폐기 renderer 자산 검사 및 native adapter link 순서/횟수 | PASS |
| 기본/auto/오타/폐기 URL/두 유효 renderer 선택 | 9/9 PASS; WebGPU 실제 queue 완료까지 확인 |
| WebGPU 확대 기능 묶음 | 13 PASS, 메뉴 선택 테스트 1 FAIL; 아래 실패 기록 참조 |
| 수정된 WebGPU 메뉴·편집 테스트 | FAIL: 이후 .NET TimerQueue 런타임 예외; 수용 미완료 |
| 명시적 WebGL 기능·입력·상태·수명 | 7 PASS, WebGPU 전용 device loss 1 skip |
| WebGPU/WebGL DPR2 sheet pointer | 각 1/1 PASS |

WebGPU 확대 묶음에서 네 destination, light/dark theme, 390/800/999/1280/1600
responsive navigation, 메뉴·대화상자·검색·이미지 스크롤, 전체 component inventory,
스크롤 후 상태 유지, seed/이미지 theme 선택, responsive reparenting, dark TextField
전경색을 확인했다. 정상 종료, invalid private protocol 종료, 실제 device.destroy
주입 후 역할 종료와 main runtime 유지, adapter unavailable의 명시적 실패도 확인했다.

정상/invalid-protocol 종료는 runtime error 0을 요구했다. 의도적인 device loss는
native의 `Buffer async map failed ... Device was lost` 진단을 보존하면서, JS 예외 0,
한 번의 disposed, in-flight/pending map/active mapped allocation 0을 검증했다.
device loss의 예상 오류를 정상 실행 성공으로 재분류한 것이 아니다.

`components-1600.png` 및 `dark-components-800.png`를 직접 열어 실제 Material
화면을 확인했다. Flutter 대비 전체 시각 동등성이나 성능 향상을 측정한 것은 아니다.
물리 키보드/IME, screen reader, 다른 브라우저/실기기, 실제 scan-out/FPS는
`notVerified`다. 새 템플릿의 package-only 외부 앱 publish도 이번 실행 범위 밖이다.

## 실패 기록과 남은 제한

1. 최초 통합 publish는 bitmap 경로 삭제 중 TS 구문 오류로 실패했다. 수정 후
   publish와 선언 검사를 통과했다. 중간 Graphite cleanup API 오인 및 TS의
   GPUTextureUsage global 선언 부재도 각각 build 실패로 기록되어 있다.
2. 초기 후보로 정상/invalid-protocol 종료를 재현하면 native `mapAsync Aborted`
   오류가 발생했다. cache/context 단계별 비동기 drain 후 두 검사가 통과했다.
3. 그 다음 device-loss 검사는 detached ArrayBuffer copy-back이 native runtime을
   중단시켜 FAIL이었다. 빌드 시점 adapter 적용 후 별도 4개 수명/미지원 검사가
   모두 통과했다. 기존 실패 trace는 유지한다.
4. 최초 확대 suite의 진단 화면에서 canvas texture에 `TextureBinding` 권한이
   없다는 GPU validation error를 확인해 중단했다. 권한 수정 후 선택 9건을
   실제 GPU 작업 완료 기준으로 통과했다. Worker 진단 검사도 idle pthread를
   순서대로 기다리던 harness 오류를 수정했다.
5. 확대 suite의 메뉴 테스트는 검색 완료 후 ArrowDown이 Green에서 Blue로
   이동하는 잘못된 순서였다. typed Green이 반영된 것을 확인한 뒤 Enter로
   확정하도록 고쳤다. 수정된 순서는 WebGL에서 전체 clipboard 편집까지 통과했다.
6. 수정된 WebGPU 메뉴 테스트는 검색/입력 후 cut 단계에서
   `System.Threading.SynchronizationLockException`, `TimerQueueTimer.Fire`와
   Mono fatal abort가 발생했다. [동일 스택의 .NET 공개 이슈 #129900](https://github.com/dotnet/runtime/issues/129900)가
   존재한다. 이번 작업은 .NET TimerQueue/실험적 WASM thread runtime을 수정하지
   않았으며, 이 실패를 해결 완료 또는 PASS로 바꾸지 않는다. 렌더러 두 개와
   WebGPU 기본값은 사용자 요청대로 유지한다.

## 증거

- 원본 로그/배포본/패키지: `.doroti/webgpu-direct/`.
- 검사 명령·소요 시간·종료 코드 및 artifact SHA-256:
  [동반 검증 JSON](webgpu-direct-default-audit.json).
- 최종 기능 화면/trace: `Doroti/validation/web-playwright/artifacts/`의
  `webgpu-selection-completion`, `webgpu-functional-final`, `webgpu-picker-final`,
  `webgl-final-2`, `webgpu-dpr2`, `webgl-dpr2`.
- 최초 수명 실패: `webgpu-direct-initial`, `webgpu-direct-lifetime`.
- 최초 texture usage 실패: `webgpu-final`; 최초 추가 진단 harness 실패: `webgpu-final-2`.
- 정상 및 loss 수정 후 검사: `webgpu-direct-loss-fixed`.

새로 만든 테스트 서버(5216/5217)와 브라우저는 검증 후 종료했다.

참고 구현: [Skia DawnBuffer의 cache 반환/asyncMap](https://github.com/google/skia/blob/chrome/m154/src/gpu/graphite/dawn/DawnBuffer.cpp),
[SkiaSharp Graphite context](https://github.com/mono/SkiaSharp/blob/v4.154.0-preview.1.26454.9/binding/SkiaSharp/Gpu/Graphite/SKGraphiteContext.cs).
