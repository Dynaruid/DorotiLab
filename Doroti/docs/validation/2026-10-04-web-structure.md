# 2026-10-04 Web 구조 개선 검증

검토 기준은 `9e9068b30de10ea9df2ff57fdec39960f206178b`와 현재 작업 트리다.
IDE에 표시된 루트 `work.md`는 디스크에 없어 현행 Web 소스와 로컬 플랫폼
감사 문서를 확인했다. 과거 플랫폼 인수 결과를 이번 실행 결과로 소급하지 않았다.

## 개선 내용

- Worker 제어, 텍스처 제어, 프레임 비용 진단에 반복되던 요청 ID/제한/타이머/
  종료 처리를 `WorkerRequestMailbox<T>`로 모았다. 응답과 전송 실패가 즉시
  제한 슬롯을 반환하고, 종료 시 남은 요청과 타이머를 취소한다.
- 텍스처 타임아웃이 `disconnect()`의 `Disposed` 오류로 덮이는 순서를 수정했다.
  해당 요청은 `Timeout`, 같은 소유자의 나머지 요청은 `Disposed`로 종료된다.
- 전역 raster/capture 배열을 `PlatformFrameStager`로 분리했다. 다음 프레임의
  캡처는 현재 프레임을 지연시키지 않으며, 폐기·캡처 실패 뒤 늦게 도착한
  비트맵도 닫는다. 동시에 두 commit을 전송하거나 미완료 packet을 덮지 않는다.
- Worker 종료는 기존 GPU/PlatformView drain 뒤 제어 요청을 취소하고 drain한다.
  사용자 파일 선택처럼 타임아웃이 없는 요청도 호출자를 남기지 않는다.
- 기존 브라우저 서비스 테스트의 `webgl`/`webgpu` 축약 쿼리를 실제 정책이 읽는
  `worker-direct-*`로 수정하고 선택 결과를 확인한다. 드롭은 정확한 소유 canvas의
  수신 준비를 기다리며, 탐색/복원은 URL과 텍스트 각각의 완료를 기다린다.

C# JavaScript interop 이름과 메시지 protocol version 5는 유지했다. 렌더러 선택,
스레드 소유 정책, source texture admission 및 composition/GPU 효과의 별도
예산과 GPU 완료 대기는 기존 구현을 사용한다.

## 이번 실행 결과

| 확인 | 결과 | 범위 |
| --- | --- | --- |
| Web Debug 빌드 | PASS, 경고 0 / 오류 0 | .NET SDK 10.0.400, Testbed threaded browser-wasm; strict TypeScript 및 static asset 생성 포함 |
| Node 회귀 검증 | PASS 24/24 | 기존 rendering 6, Worker lifecycle 12, texture 6; 신규 회귀 조건 13개 |
| Chrome graphics loss/restart | PASS | 154.0.8037.93 headless, WebGL/main 및 WebGPU/main; 선택 렌더러 유지, 새 endpoint/첫 프레임/포커스, 오류 0, 미완료 요청 0 |
| Chrome local iframe/raster composition | PASS | WebGL/main 및 WebGPU/main; iframe 카운터 상태/identity 유지, resize 뒤 commit 1→3, 병행 진단 요청 2개, page error 0 |
| Chrome DOM/service 요청 | PASS | 명시적 WebGL/main 및 WebGPU/main; 파일 선택/읽기/취소, Back/Forward, 텍스트/URL reload 복원, 합성 DOM Copy drop, 오류 0 |
| Source 및 diff 점검 | PASS | 현재 문서 링크, 추적 temp 정책, Python 문법, `git diff --check` |

Node 검증은 요청 상한, 순서가 다른/중복/알 수 없는 ACK, 동기 전송 실패,
타임아웃 오류 보존, 종료 시 무기한 chooser 취소, 교체 뒤 오래된 요청 ID,
프레임별 캡처 분리, 폐기/전송 실패/늦은 캡처/동시 commit을 확인했다.

원시 결과는 삭제 가능한 `temp/testing/web-structure/2026-10-04/`에 있다.
`recovery/result.json`, `composition/result.json`과 composition PNG,
`browser-r3/services-webgl/result.json`, `browser-r3/services-webgpu/result.json`을 확인했다.
모든 검증 명령은 `Doroti/eng/run-with-timeout.py --timeout 1200`으로 실행했다.
기본 Python에는 Playwright가 없어 기존 플랫폼 감사 venv의 실행 환경을 사용했다.

추가 브라우저 서비스 검증의 첫 실행은 drop 수신 결과 확인에서, 두 번째 실행은
reload 뒤 URL 검사에서 실패했다. 각 실패 결과를 해당 run 디렉터리에 보존하고
위 fixture 개선을 반영했다. 최종 실행은 두 렌더러의 서비스/합성 경로 모두 통과했다.

## 인수 경계

이번 Chrome 실행은 threaded/main 소유 경로의 자동 브라우저 검증이다.
standalone worker runtime의 실제 재시작, 모바일/Safari/Firefox, 물리 입력·IME,
보조공학 도구, 실제 디스플레이 FPS·scanout, 카메라 및 외부 동영상 재생은
이번 변경에서 `notVerified`다. iframe 합성 검증은 외부 YouTube 응답을 로컬
빈 페이지로 대체했으며 외부 미디어 인수 결과가 아니다.

요청 취소와 DOM `BackendAccepted` ACK는 GPU 완료/물리 표시 증거가 아니다.
기존 전체 제품의 `PARTIAL` 인수를 이번 구조 개선으로 승격하지 않는다.

설계 책임과 후속 수정 기준은 [Web host 구조](../web-host-architecture.md)에,
자동 검증은 [Worker lifecycle](../../tests/web_worker_lifecycle.mts)과
[texture](../../tests/web_textures.mts)에 있다.
