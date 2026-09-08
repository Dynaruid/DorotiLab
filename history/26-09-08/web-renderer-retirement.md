# Web renderer retirement — 2026-09-08

사용자 요청에 따라 CanvasKit을 먼저 제거하고, 후속 요청으로 `document-webgl`과
`offscreen-bitmap`도 제거했다. 현재 지원 모드는 SkiaSharp WASM의
`worker-direct-webgl`(기본값)과 `offscreen-worker`다. 세 폐기 모드의 URL 값은
기존 미인식 값 처리 규칙에 따라 direct를 선택한다. 폐기 모드를 복구한 결과가 아니다.

기준 HEAD는 `2c68e877a66181bb048430c7bacd270df2c094db`이며 앞선 WebGPU 선언
수정이 남아 있던 worktree에서 진행했다. 그 호환 선언도 CanvasKit 제거에 따라
폐기했다. 기존 병렬 레이아웃 제거와 SkiaSharp 4.154/WebGPU 후속 작업 범위를 유지한다.

## 구현

- CanvasKit C# capability/resource/paragraph/DisplayList 어댑터·캐시, UI/Raster Worker,
  바이너리 DisplayList decoder와 사용처가 없어진 전용 transport 도구를 제거했다.
- CanvasKit npm manifest/lock/pin, 자산 복원·검증·패키징 targets와 전용 검증을 제거했다.
  설치 캐시는 소스 밖 `.doroti/canvaskit-removal/retired-node-modules`로 옮겼다.
- 메인 스레드 WebGL과 같은 스레드 OffscreenCanvas의 front/staging·bitmap presenter,
  Razor 화면 구성과 document runner를 제거했다. Worker의 ImageBitmap 경로는 유지한다.
- loader와 public 타입은 두 Worker 모드만 제공한다. `Blazor.start()` 경로,
  `blazorOptions`, Testbed/template의 Blazor loader 링크를 제거했다.
  generated Main은 UI 작업을 하지 않고 StartWorker/StopWorker가 렌더 역할을 소유한다.
- SDK의 TypeScript 출력은 엄격히 검증한 intermediate 하위 전용 폴더에 생성한다.
  컴파일 전에 이전 출력을 정리해 삭제한 모듈이 다음 패키지에 남지 않게 했다.
- manifest·README·ADR·공용 검증을 갱신했다. 같은 runtime의 렌더 Worker,
  소유권·입력·전용 MessagePort·종료 계약은 유지한다.

## 최종 검증

모든 test/build 실행의 외부 timeout은 1,200초이며 자동 retry는 0이다.

| 검사 | 결과 |
| --- | --- |
| Host 및 Playwright 전체 TypeScript 선언 검사 | PASS; skipLibCheck 없음 |
| source node_modules 없는 MSBuild TypeScript 컴파일 | PASS; 임시 stale JS도 제거됨 |
| TypeScript 출력 경로가 obj 자체/obj와 이름만 비슷한 형제 폴더인 음성 대조 | 기대한 DOROTIWEB014로 거부 |
| Worker 전용 Web Release native publish | PASS; 최종 새 출력 폴더 publish 13.14초 |
| Web host 및 Web target NuGet pack | PASS; 3.78초 / 5.88초 |
| 전체 FCR-7 Material/widget runtime 계약 | PASS; 정상 restore 후 8.43초 |
| Chromium hardware 브라우저 | 15/15 PASS; runtime error 0 |
| 최종 배포 및 nupkg 검사 | PASS; 폐기 자산·실행 모드 참조 없음 |

브라우저 범위는 omitted/auto/오타/폐기 URL/두 유효 모드 선택, CanvasKit·Blazor
loader 네트워크 요청 없음, picker/menu/텍스트 편집, selection 공유 상태,
4개 destination·theme·반응형 navigation, 이미지 palette 추출, 실제 Skia/direct
소유권, 입력 직후 정상 종료, 잘못된 private-port protocol의 단일 역할 종료다.
스크린샷에는 실제 Material 화면과 편집된 텍스트가 표시된다.
물리 키보드/IME·screen reader·다른 브라우저·실기기·scan-out은 이번 수용 범위가
아니며 `notVerified`다. 성능 향상과 물리 프레임률은 측정하지 않았다.

배포 경로·패키지 hash·실행한 browser case는
[검증 요약](web-renderer-retirement-audit.json)에 있다. 최종 로컬 배포 폴더는
`.doroti/canvaskit-removal/worker-only-wwwroot-parent/wwwroot`다.
원본 build/pack/browser 로그는 `.doroti/canvaskit-removal/`, 마지막 Playwright 결과는
`Doroti/validation/web-playwright/artifacts/worker-only-final/`에 있다.
검증용 서버와 브라우저 프로세스는 종료했다.

## 최초 실패와 후속 범위 변경

1. 첫 CanvasKit 제거 publish는 남은 BrowserPictureBlockCache의 DisplayList 타입
   참조 때문에 FAIL이었다. 전용 cache까지 제거한 뒤 publish가 통과했다.
2. 중간 TypeScript 검사에서 삭제된 document presenter 변수의 잔여 참조를 발견해
   제거했다. 최종 전체 선언 검사와 제품 컴파일이 통과했다.
3. CanvasKit만 제거했던 첫 renderer suite에서는 document-webgl의 GL runtime
   해석 오류와 offscreen-bitmap의 같은 오류/120초 준비 timeout이 발생했다.
   첫 묶음은 두 실패 뒤 중단했으며 PASS로 재분류하지 않는다. 이전
   `.doroti/work3/parallel/final-wwwroot`에서도 두 모드의 같은 GL 오류를 재현했다.
   당시 Worker 범위의 후속 검사는 11/11 PASS였다.
4. 사용자가 두 모드도 제거하도록 요청해 해당 구현을 폐기했다. 최종 15/15의
   옛 URL 검사는 새 direct 선택을 검증하므로 3번의 옛 모드 FAIL을 뒤집지 않는다.
5. FCR-7의 최초 --no-restore 실행은 캐시의 SkiaSharp 4.152와 현재 4.154 간
   CS1705/MSB3277로 빌드 FAIL이었다. 정상 restore 후 전체 계약이 PASS였다.

이전 CanvasKit의 정량/물리 미수용과 원본 결과는
[보관된 README 증거](web-canvaskit-retired-readme.md)와 기존 history를 따른다.
