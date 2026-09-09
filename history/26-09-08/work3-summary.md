# work3.md — SkiaSharp 전환·WebGPU 및 병렬 레이아웃 작업 보관

보관일: 2026-09-09. 사용자 요청에 따라 `history/26-09-08`에 구성하고 루트
`work3.md`를 삭제했다. 삭제 직전 문서와 기존 실행 보고서를 정리한 기록이며,
이번 문서 보관에서 빌드·브라우저·성능·실기기 검증을 새로 실행하지 않았다.
보관은 미완료 gate의 PASS 전환을 뜻하지 않는다.

## 보관 자료

| 자료 | 내용 |
| --- | --- |
| [원문](work3.original.md) | 삭제 직전 계획·진행·최초 실패·후속 정리를 byte 단위로 보존 |
| [보관 manifest](work3-archive-manifest.json) | 원본 크기·SHA-256·보관 시각과 경로 |
| [통합 실행 보고서](work3-execution.md) | U/G 단계와 최초 WebGPU·결합 후보의 실행 결과 |
| [병렬 실행 기록](work3-parallel-execution.json) | 제거된 병렬 레이아웃 실험의 증거 |
| [WebGPU 기본값 전환](webgpu-direct-default.md) | 최초 원복 이후의 구현·수명 검사·실패와 한계 |
| [브라우저 타이머 후속 수정](browser-owner-timers.md) | 기본값 전환 보고서의 TimerQueue 실패 후속 기록 |

원문 안의 상대 링크는 작성 당시 **저장소 루트 기준**이다. 원문은 수정하지
않았으며, 이 요약의 링크는 보관 위치에 맞췄다. 과거 후보 URL과 PASS는 해당
실행 당시 결과이며 현재 기능 또는 새 검증의 증거로 재사용하지 않는다.

## 결과와 변경 순서

- **SkiaSharp U0–U3:** `4.154.0-preview.1.26454.9` 전환, 관리/native 의존성
  정합성, WebGL 기준선, 가용 host의 build/restore·생성/pack/CLI 검증을 기록했다.
  정량 성능 개선과 플랫폼별 물리 입력·실기기 수용은 별도 미검증 범위다.
- **최초 WebGPU G0–G4:** shared-runtime 렌더 Worker의 최소 출력 smoke는 PASS,
  첫 제품 출력·resize도 확인했으나 정상 종료 `mapAsync Aborted`와 device-loss
  종료 확인 실패로 G3 FAIL이었다. 당시 후보를 보존하고 WebGL S2로 원복했다.
  전체 effects/readback corpus와 3쌍 성능 비교는 완료하지 못했다.
- **병렬 레이아웃 제거:** 사용자가 이득이 작다고 판단하여 순수 treemap 엔진,
  실험 화면·Material 패널·옵션·전용 검증을 제거했다. T0b/T1–T7 및 J0 계획은
  종료했으며, 기존 결과를 전체 성능 개선으로 판정하지 않는다.
- **선언 충돌 수정과 렌더러 정리:** CanvasKit의 구형 WebGPU 타입 충돌을
  수정하고 전체 선언 검사를 복구했다. 이후 CanvasKit과 document/bitmap 경로를
  제거하면서 임시 호환 진입점도 제거했다. 원문 6.3의 WebGL/offscreen-worker
  설명은 그 단계의 상태이며 후속 기본값 전환 이전 기록이다.
- **후속 WebGPU 기본값 전환:** 보관 당시 문서에 기록된 지원 모드는
  `worker-direct-webgpu`(기본값)와 명시적 `worker-direct-webgl`이다.
  `offscreen-worker`도 제거했다. 별도 전환 보고서는 단계별 비동기 해제와
  device-loss adapter 수정 후 수명 검증 통과를 기록하면서, 확대 검사에서
  발생한 TimerQueue 실패를 보존한다. 타이머 수정·재검증은 후속 보고서를 따른다.

## 보존할 검증 경계와 후속 범위

원문은 병렬 기능 제거 후 Testbed Release build, TypeScript 검사와 참조 정리
PASS를 기록한다. 최초 전체 선언 검사 FAIL 및 이후 수정 PASS를 구분한다.
렌더러 정리 단계의 Release native publish·pack·FCR-7·Chromium 15/15 PASS도
해당 단계의 결과이며, 이후 모든 변경의 포괄적인 수용으로 확대하지 않는다.

WebGPU/WebGL의 수명·기능 회귀와 같은 workload의 성능·메모리 비교는 각각
증거 범위를 확인해야 한다. 기존 `performanceUnproven`, 물리 키보드/IME,
screen reader, 다른 브라우저·OS/GPU·실기기, 실제 scan-out/FPS의
`notVerified`를 문서 보관만으로 해소하지 않는다. Worker owner 및
`closed` → `disposed` 계약, 기능·정량 성능·사용자 체감·물리 수용의 구분을 유지한다.

네이티브 Graphite 전환의 별도 진행은 [현재 work.md](../../work.md)와
[Linux 후속 보고서](../../Doroti/docs/validation/native-graphite-linux-2026-09-09.md)를
따른다. 이 보관 작업은 해당 구현 범위를 실행하거나 변경하지 않는다.
