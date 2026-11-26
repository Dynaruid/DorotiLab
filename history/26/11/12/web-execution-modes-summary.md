# Web 자동 실행 모드와 배포 검증 요약

원본 `work.md` · 작성 2026-11-12 · 최종 검증 2026-11-16 · 보관 2026-11-26 KST

**PARTIAL.** 최종 기능 행렬과 동일 캐시 전환은 각각 12/12 PASS였다. 자동 성능 측정은 회귀 기준과 compatible 입력 예산을 넘겼다. 사용자는 두 모드를 직접 실행해 모두 쾌적하다고 보고했다. 여섯 배포본의 generation, 해시, 파일 수와 성능 표본 36개는 검증 기록에 있다. [Web 실행 모드 검증 기록](../16/web-execution-qualification.json)

## 제품 계약과 수정

- `DorotiWebExecutionMode`는 `threaded`, `compatible`, `auto`를 지원한다. `auto`는 .NET 시작 전 secure context, cross-origin isolation, 공유 메모리 지원을 검사해 한 모드를 고른다. 로더는 한 세션에 관리 런타임 하나를 시작하고 선택한 배포 자산만 받는다. 배포나 런타임 실패는 진단으로 보고한다.
- `threaded`는 main 소유 공유 Mono와 JS-affine 렌더 Worker를, `compatible`은 렌더 Worker의 비공유 Mono와 WebGL2를 쓴다. 양쪽 모두 앱·레이아웃·그리기·Skia·GPU의 렌더 소유자는 렌더 Worker다. 공통 프로토콜 v7은 준비·첫 캔버스 프레임·입력 순서·세션 변경·종료를 추적한다.
- 두 모드의 restore·native link·AOT·정적 자산을 generation별로 분리한다. 최종 게시 작업은 SDK boot metadata와 import map으로 활성 loader·runtime·Worker·assembly를 확인하고 이전 fingerprint 및 압축본을 제외한다. 배포 세대가 다르거나 활성 파일이 빠지면 게시를 거부하고 직전 완결본을 보존한다.
- Mono 11 threaded의 `Thread.Start` 멈춤은 초기 Worker 16개의 native preload 응답을 처리한 뒤 managed startup을 허용해 수정했다. Emscripten 6.0.3이 Mono 11의 문자열 `killThread` 명령을 처리하지 못하던 종료 오류는 `DorotiMono11Pthreads.js`를 해당 native link에 넣어 수정했다. 검사는 managed `disposed` 이후 실제 렌더 Worker의 `close`까지 기다린다. [Mono 11 시작 진단과 재현](../../../../packages/platforms/web/tests/Mono11ThreadProbe/README.md)
- 같은 출력 경로에서 mode·TFM·SDK를 바꿀 때 남았던 `Link.semaphore`, `runtime-icall-table.h`, `m2n_cache.txt`는 빌드 identity가 바뀔 때 무효화한다. Texture 교체는 texture-free tree의 `endOfFrame`까지 이전 source를 유지한다. 불변 snapshot과 GPU lease는 소유자가 완료를 확인한 뒤 반환한다.
- 개발 서버의 격리 헤더, 터널 검증, 네 샘플과 새 템플릿의 실행 모드 설정을 반영했다. Editor helper의 중복 assembly attribute와 Dart stub 선언을 고쳤으며 TypeScript 빌드·VSIX 패키징을 검증했다. 제품 기본값은 `net10`/`threaded`이고 `auto`는 opt-in이다. `net11`은 재검증 전용이며 일반 빌드는 `DOROTIWEB050`으로 차단된다. [실행 프로필과 게시 방법](../../../../packages/platforms/web/README.md#execution-profiles-opt-in)

## 2026-11-16 최종 기능 검증

| 범위 | 당시 결과 |
| --- | --- |
| Testbed 주 행렬 | SDK 10/net10, SDK 11/net10, SDK 11/net11 × Debug/Release AOT × 양 모드 **12/12 PASS**. Chromium에서 첫 프레임, 실제 paint owner, 입력, Texture 교체·정리, Worker 종료를 확인했다. 6개 dual 배포의 전체 해시를 맞췄다. Firefox의 강화된 Worker 종료 검사는 net11 threaded의 Debug·Release/AOT를 확인했다. |
| 동일 캐시 전환 | Debug 7단계에서 SDK 10→11→10과 net10→net11→net10을, Release 5단계에서 threaded→compatible→threaded와 AOT→non-AOT→AOT를 같은 경로에서 확인했다. **12/12 PASS**이며 기존 `icall.c:6998`과 `killThread` 오류가 재발하지 않았다. |
| 빌드·게시 계약 | TypeScript 27/27, 지원 차단과 캐시 무효화·보존 8/8, SDK 10·11 게시 계약 각각 29/29, NuGet에 포함된 validator 2/2 PASS. |
| 추가 프로필 | Sample2 SDK 10/net10 및 SDK 11/net11의 Release mixed-AOT 양 모드에서 UI·PNG 픽셀 검사를 통과했다. Testbed의 두 SDK/TFM 조합은 Release non-AOT도 통과했다. Chromium에서 동시 탭, Back, DOM 한글 입력, iframe, 파일, WebGL context loss 경로도 확인했다. Firefox 파일 검사는 controlled fixture 범위이며 OS 파일 대화상자는 미인수다. |

11월 15~16일 중간 실행에서는 Texture 조기 해제와 캐시 재사용 오류를 재현했다. 초기 브라우저 검사가 managed `disposed` 이후의 Worker 오류를 놓친 사례도 있었다.

## 성능 판정과 남은 인수

같은 Windows 호스트에서 SDK 10.0.403/net10/Mono 10.0.15, headless Chromium의 WebGL2와 SwiftShader 허용 조건으로 측정했다. Debug·Release/AOT 각각 기존 threaded·변경 threaded·compatible의 새 브라우저와 같은 페이지 재로드를 3회씩 비교했다. 시간은 캔버스 commit 경계이며 전송량은 main과 Worker 요청을 합산했다.

| 기준 | 결과 |
| --- | --- |
| 변경 threaded의 10% 회귀 기준 | **FAIL.** Debug fresh paint p95 +31.30%, Debug reload commit 간격 p95 +52.36%, Release/AOT fresh paint p95 +16.27%. 첫 프레임과 입력 p95는 모든 비교에서 10% 이내였다. |
| compatible 입력→commit p95 100ms | **FAIL.** 12개 표본 모두 초과했다. 셀별 중앙값은 Debug 341.9~350.7ms, Release/AOT 195.4~217.8ms였다. 첫 프레임은 새 브라우저 10초·재로드 5초 예산을 충족했다. |
| 사용자 체감 | 격리된 threaded와 비격리 compatible 모두 쾌적하다고 확인했다. 첫 lazy 로드의 짧은 지연은 감수할 수 있다고 보고했다. |

compatible의 main ingress→Worker dispatch p95 중앙값은 Debug 148.9~215.0ms, Release/AOT 133.6~145.0ms였다. Worker 큐 대기와 프레임워크 스케줄링·paint 비용의 원인 격리 및 재측정이 남았다. Threaded에서 본 JS interop blocking 경고는 기존 로그에서 최초 폰트 제한 시간의 타이머 스레드 시작으로 추적됐다. 사용자 실행의 호출 지점과 lazy 로드 지연의 인과관계는 미확정이다.

Android Chrome과 실제 iOS/iPadOS Safari, 하드웨어 GPU·표시 시점, 물리 한글 IME, 스크린 리더, 장시간 사용, Firefox OS 파일 대화상자 인수가 남았다. compatible Hot Reload도 미검증이다. 당시 계획에서 CI는 제외했다. 재현 도구는 게시 안내에 있으며 각 실행은 외부 1,200초 타임아웃을 적용한다. [자격 검증 명령](../../../../packages/platforms/web/README.md#sdk-and-runtime-qualification)

`Doroti/artifacts/web-qualification`의 원시 로그와 게시본은 삭제 가능한 로컬 산출물이다. 이 문서는 당시 검증을 요약한다. 원문은 `git show a9a524647d276f89d8871128c2e3e5accd1f3337:work.md`로 조회할 수 있다.
