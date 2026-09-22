# Web 네이티브 소스 Texture 및 Debug 시작 오류 수정 요약

정리일: 2026-09-22. 저장소 루트 `work.md`의 계획과 실행 결과를 요약하고 원문을 삭제했다.
이 문서는 당시 기록된 결과를 보존한다. 이번 문서 정리에서 빌드·브라우저·실기기 검증을 새로 수행하지 않았다.

**상태: P0–P6 구현·자동 검증 완료 / Debug 런타임 시작 오류 수정·검증 완료 / 실카메라 `notVerified`.**

## 목표와 구현 결과

Doroti Web의 Canvas·Video·카메라 프레임을 기존 `Texture` 위젯으로 합성하도록 구현했다.
기존 Android·Windows·Apple·Linux 텍스처 구현은 유지했다.

- `HTMLCanvasElement`, `HTMLVideoElement` 등록, 직접 `VideoFrame`·`ImageBitmap` 공급,
  렌더 Worker 소유 `OffscreenCanvas` 입력을 지원한다. 카메라는 `getUserMedia()` → video 경로를 사용한다.
- 공개 진입점은 `texturesForCanvas(canvasId)`이며 `registerVideo`, `registerCanvas`,
  `createFrameProducer`, `markFrameAvailable`, `pushFrame`, `dispose`를 제공한다.
  texture ID는 decimal Int64 문자열로 전달하고 registry는 owning view에 바인딩한다.
- DOM·프레임·GPU 객체는 JS/Worker에 두고 control 메타데이터와 transferable payload를 분리했다.
  앱에서 픽셀을 CPU readback·PNG/Base64·C# `byte[]`로 변환해 전달하는 경로를 만들지 않았다.
- WebGPU는 owning device의 `copyExternalImageToTexture()` → Dawn backend texture → `SKImage`,
  WebGL은 owning WebGL2 context의 source upload → Ganesh wrapper로 합성한다.
  기본 `worker-direct-webgpu`와 명시적 `worker-direct-webgl`을 지원하며 자동 backend fallback은 하지 않는다.
- 프레임 도착은 위젯 트리 재빌드 없이 texture revision과 raster 갱신을 요청한다.
  크기·필터링·transform·clip·opacity·freeze와 retained scene 동작을 연결했다.

## 소유권과 수명 계약

- credit/ACK, main의 최신 후보 1개, Worker pending 제한, source/view별 GPU 예산으로 상류부터 큐를 제한한다.
  Canvas 비동기 snapshot은 1개만 진행하고 추가 dirty 요청은 합친다.
- view·등록/source 세대·frame sequence를 검증하고, 늦은 프레임·snapshot·ACK와 게시 실패를 정리한다.
  `pushFrame` 성공 시 소유권이 이전되며 caller는 해당 프레임을 사용하거나 닫지 않는다.
- 입력 source의 반환 시점과 GPU destination의 마지막 sampling 완료를 구분한다.
  Graphite submit/queue completion 또는 GL 완료에 맞춰 자원을 반환하고 취소·실패·context/device loss를 처리한다.
- freeze는 마지막 표시 texture를 보존하고 최초 freeze에서는 첫 유효 프레임을 얻는다.
  resize는 새 storage generation을 만들고 이전 allocation은 사용 완료 후 반환한다.
- 빌린 video/stream은 기본적으로 중지하지 않는다. 카메라 helper가 직접 만든 stream만 종료 시 `track.stop()`한다.
  카메라는 사용자 시작 액션에서만 요청하며 일반 샘플 시작만으로 권한 요청이나 재생을 시작하지 않는다.
- CORS/origin-clean, 영상 readiness, 크기·예산, 지원하지 않는 source, 권한 거부를 구분한다.
  진단은 프레임 전달·수락·drop·copy·draw·close, pending/in-flight, GPU bytes, 세대를 구분한다.

## 단계별 완료 및 검증 기록

| 단계 | 기록된 완료 내용 |
| --- | --- |
| P0 | main/managed/render Worker 전달 경계, native bridge, ID·오류·예산 계약 및 실제 GPU 정적 source 합성 확인 |
| P1 | JS API·프로토콜·타입 선언·패키징, bounded 전달, 소유권·late-frame 정리 구현 |
| P2 | WebGPU/Dawn importer, GPU 완료 기반 retirement, freeze·resize·retained scene 연결 |
| P3 | WebGL/Ganesh importer, GL 상태·texture name 수명 및 context loss 정리 |
| P4 | Canvas·로컬 영상·카메라·직접 프레임·OffscreenCanvas 및 codec capability 확인을 포함한 WebCodecs 예제, 샘플 제어 추가 |
| P5 | 실제 Windows 브라우저의 두 backend, 픽셀·수명·오류·공통 Texture 회귀 검증 |
| P6 | 공개 API·검증 보고서·README·진단 schema, NuGet/publish 자산·native export 및 형식·빌드 검사 |

- 최종 Release publish: WebGPU·WebGL **각각 제품/수명 검사 15개와 픽셀 검사 46개 통과**.
- 두 backend의 device/context loss, 실제 샘플 버튼과 합성된 native 입력 검사 통과.
- owning GPU wrapper, source별 픽셀 합성, freeze/resume, resize, retained scene 및 WebView 중첩 검증.
- 공통 CPU Texture **29개**, native ownership **25개**, DOM 수명 회귀 **18개** 검사 통과.
- NuGet 모듈·타입 선언, publish 자산·native export, CSharpier·TypeScript·managed build·diff 검사 통과.
- Debug 후속 수정: Runner SDK에서 Emscripten의 주소 0 스택 배치를 바로잡고 `-O0`·디버그 심볼·스레드를 유지했다.
  실제 Debug 시작과 WebGPU·WebGL **각각 실행 검사 15개 및 픽셀 검사 46개 통과**.

## 증거와 재현 자료

- [Web Texture 검증 보고서](../../Doroti/validation/textures/web-results-2026-09-22.md):
  구현 파일, 재현 명령, 브라우저/GPU, 자산 hash, 픽셀·소유권·retirement 결과와 제한사항.
- [Debug 시작 오류 검증 보고서](../../Doroti/validation/web-debug-startup-2026-09-22.md).
- [Browser source textures 공개 API](../../Doroti/docs/web-textures.md),
  [공통 Texture 문서](../../Doroti/docs/textures.md).

원문의 실행 정책은 `python Doroti/validation/run-with-timeout.py`로 테스트에 20분 process-tree
timeout을 적용하고 보통 30회 이내의 명시적 시나리오로 검증하는 것이다.
Web host Release build, testbed Release publish, `Textures.csproj` 및 `NativeContracts.csproj` 실행이
기본 명령에 포함되며 구체적인 브라우저 실행 명령은 위 보고서를 따른다.
원문에 기록된 로컬 증거 위치는 `Doroti/artifacts/textures/web/<실행시각>/`이다.
이 경로는 삭제 가능한 로컬 산출물이며 이번 정리에서 원시 파일의 잔존 여부를 검증하지 않았다.

## 미검증 범위와 후속 작업

- 카메라는 가상 장치로 검증했다. **실카메라 물리 센서·권한 UI·분리/재연결, 물리 입력·표시 지연은 `notVerified`**다.
- IDE 중단점 연결은 미검증이다. native submit 오류 코드별 강제 주입과 장시간 성능은 검증 보고서의 제한사항을 따른다.
- native 앱·Android 에뮬레이터·Apple/Linux 실행은 이번 Web 작업의 필수 실행 범위에 포함하지 않았다.
- 하드웨어 디코딩, 브라우저 내부 zero-copy, 전력·메모리·물리 표시 지연 개선은 이 결과로 입증하지 않는다.
- 후속 후보: `GPUExternalTexture` 직접 sampling과 복사 성능 비교, `MediaStreamTrackProcessor` 전용 입력.
- 범위 밖: HDR/wide gamut/DRM/YUV-plane 직접 import, 다른 GPU device/context texture 직접 공유,
  임의 DOM/iframe texture화·cross-origin 정책 우회, 독립 미디어 플레이어·오디오 동기화,
  장시간 성능 검증, 자동 backend fallback 및 Web runtime/renderer 전면 교체.
