# Web 일반 화면 처리 비용·iPhone 메모리 대응 요약

- 작업 기록일: 2026-09-22. 보관일: 2026-09-23.
- 대상: 루트 `work.md`의 P0~P5 일반 화면 성능 작업과 추가 M0~M5 메모리 작업.
- 최종 상태: **PARTIAL — 계측·후보 비교·메모리 관리 구현과 로컬 검증은 수행했으나, 전체 성능 목표·실제 iPhone 탭 종료 해결·최종 WebGPU 지연 기준은 미충족.**
- 사용자 요청에 따라 실행 결과와 남은 경계를 요약하고 루트 원본을 삭제했다. 아래 검증은 기존 기록이며, 보관 과정에서 제품 빌드·브라우저·기기 검증을 다시 실행하지 않았다.

## 목표와 보존한 구조

일반 화면의 스크롤·애니메이션에서 managed CPU, 할당, scene 변환·재생 비용을 줄이는 것이 첫 목표였다. 이후 iPhone 12/iOS 26/Chrome의 sample 스크롤 중 탭 종료 관측에 따라 메모리 상한과 자원 회수를 별도 목표로 추가했다.

main runtime 1개와 shared-runtime render Worker, OffscreenCanvas 직접 GPU 출력 구조를 유지했다. 입력은 main DOM → MessagePort → Worker의 Framework·scene·Skia → WebGPU/WebGL 순서로 처리된다. Worker 도입만으로 Worker 내부의 긴 build/layout/paint가 해결되지는 않는다. 기존 latest present 슬롯과 WebGPU in-flight 제한이 있어 무제한 렌더 큐를 전제하지 않았다.

화면·방문 State·focus·IME·semantics·애니메이션·DPR·효과를 줄여 비용을 낮추는 방법은 제외했다. 기존 HAMT, SectionList, SKPicture/raster cache, semantics 캐시를 출발점으로 삼았으며, runtime/AOT 전환이나 명령 표현 전면 교체를 근거 없이 채택하지 않았다.

## P0~P5: 일반 화면 처리 비용

| 단계 | 상태 | 결과와 남은 경계 |
| --- | --- | --- |
| P0 기준선·계측 | PARTIAL | Release runtime·실제 자산 hash, bounded 숫자 ring, allocation flag 전달, export 할당 제외, scroll/animation 기준선 확보. 정확한 input→반영 scene 연결, 전체 stage·명령별 비용, 결정적 계약 fixture는 미완료. |
| P1 명령·snapshot | DEFERRED | 중복 저장·scope 복사가 상위 병목이라는 근거 부족으로 구조 변경 보류. `notApplicable`로 확정하지 않음. |
| P2 재생·캐시 | EXPERIMENT REVERTED | 텍스트 재생의 빈 fallback JSON 생성 축소를 3쌍 비교했으나 일관성과 채택 기준 미달로 원복. cache bypass 전체 분류 미완료. |
| P3 Framework·bridge | PARTIAL | 상세 진단 OFF일 때 raster interop/message/payload 생성 차단. AnimatedBuilder·semantics의 높은 self time 확인. Framework 구조·작업 순서는 유지했으며 병목 개선은 남음. |
| P4 실행 모드 | PARTIAL | Release Mono 10.0.11, threads/SIMD/native relink 확인. managed AOT 설정 없음. Jiterpreter 활성·기여도와 독립 AOT 실험은 `notMeasured`. |
| P5 통합·검증 | PARTIAL | Release publish, ring/분석 계약, native raster 계약, Web 회귀 기록 확보. 전체 corpus·결정적 Framework 계약·물리 체감·전체 플랫폼 성능 PASS는 아님. |

텍스트 후보의 S2 owner 동기 구간 합계 변화는 AB/BA/AB 세 쌍에서 **−1.2%, −2.1%, +1.5%**였고, S3도 일관되게 개선되지 않았다. 후보를 원복하고 계측 및 진단 OFF 경로의 불필요한 작업 제거만 남겼다.

숫자 ring은 16,384개 기록으로 제한했다. 프레임 중 JSON/배열 export를 피하고 최종 `finish`에서 같은 Worker turn의 managed/ring snapshot을 읽는다. 구현 corpus는 좌측 Components 스크롤, 보이는 progress 10초, 버튼 입력, 탭 복귀를 포함한다. 우측 전체 방문·독립 소형 animation·스크롤 중 focus 동시 입력·결정적 tick까지 충족한 것은 아니다.

계획의 성능 목표는 대표 병목 p95 15% 이상 및 전체 owner CPU/action 또는 input→submit p95 10% 이상 감소, 60Hz warm 구간의 owner+동기 raster/submit p95 13.3ms 이하, 주요 지표 5% 초과 악화 없음이었다. **달성 결과가 아닌 미충족 목표**로 보존한다. 할당만 20% 이상 줄어든 결과는 `allocationOnly`로 구분하며 버벅임 해결로 간주하지 않는다.

## M0~M5: iPhone 탭 종료 대응·메모리 관리

사용자는 iPhone의 sample 스크롤 중 이미지가 나타날 무렵 탭 종료를 관측했고, 명시적 `worker-direct-webgl`에서는 확인한 구간에 재발하지 않았다고 보고했다. 이는 WebGPU 경로를 우선 조사할 근거이며 OOM·특정 WebKit 버그·WebGPU 원인 해결의 증거는 아니다.

| 단계 | 상태 | 결과와 남은 경계 |
| --- | --- | --- |
| M0 기기·backend·메모리 기준선 | PARTIAL | 연결 기기 iOS/Chrome 정보와 desktop 비교 확보. 기존 종료 URL·동일 자산·관찰 시간 및 실제 종료 원인은 미확인. |
| M1 iOS 기본 WebGL 선택 | PARTIAL | 중앙 선택·명시 override·미지원 계약 통과. 실제 iPhone에서의 `mitigated` 판정은 `notVerified`. |
| M2 텍스트 자원 제한 | PASS | 256-entry LRU, 초과 스타일 반복·퇴출·font 등록·SKPicture 수명·glyph/측정/픽셀 계약 통과. native 계약 범위의 PASS. |
| M3 GPU 예산·회수 | PARTIAL | mobile WebGL native 64MiB/raster 16MiB, 사전 예약·현재 프레임 보호·유휴 회수 채택. WebGPU 작은 예산은 기각하고 기존 값 유지. |
| M4 backing 여유·축소 | PARTIAL | mobile exact backing, 1초 안정·2초 재할당 간격·25% 이상 면적 초과 시 축소, 회전 중간 면적 제한. desktop 및 WebView DPR/resize identity 통과. 실제 IME·주소창 연속성 미확인. |
| M5 통합·기기 검증 | PARTIAL | Release publish, native/정책 계약, 입력·양 backend Texture 픽셀/수명·WebView 계약, 진단 OFF 기본 경로 10분 desktop 관찰 수행. 실제 iPhone 및 WebGPU 지연 5% gate 미충족. |

### 채택한 변경

- `doroti.web.policy.ts`에서 requested/selected/reason과 메모리 프로필을 한 번 계산해 loader → host → Worker에 전달한다. iPhone/iPad/iPod 및 desktop-UA iPad의 자동 선택은 WebGL, 다른 플랫폼은 기존 WebGPU다. 명시 backend 선택을 보존하며 초기화 실패를 숨기는 hot fallback·자동 reload는 추가하지 않았다.
- 텍스트 자원은 owner의 256-entry LRU로 제한하고 등록 font 변경 시 generation 및 관련 캐시를 무효화한다. native 초과 fixture에서 1,024 styles × 4회 후 매회 256 entries, 768 font resources, 286,720 estimated bytes를 기록했다. 이 수치는 native resident memory가 아니다.
- mobile WebGL raster는 24 entries/4M pixels(16MiB RGBA8), native는 64MiB다. WebGPU·desktop은 기존 native 256MiB/raster 16M pixels를 유지한다. 추가 surface 할당 전 예산을 예약하고 현재 프레임의 항목을 보호하며, 한 그림의 점유를 전체 pixel 예산의 1/4 이하로 제한한다. 마지막 render 후 5초 이상 유휴일 때 owner의 프레임 사이에서 native unused cleanup을 수행한다.
- mobile backing은 실제 필요 픽셀 크기로 시작·성장하고 hysteresis 조건에 따라 축소한다. Worker만 transferred canvas 크기를 수정하고 축소할 축부터 변경해 회전 중 불필요한 중간 버퍼를 줄인다. DPR·widget state·효과는 보존한다.

### 기각한 후보와 검증 한계

낮은 raster 예산만 적용한 후보는 warm visit promotion이 약 179~181에서 422~443으로 증가하고 지연이 악화되어 기각했다. WebGPU native 64MiB/raster 16MiB 후보도 지연·사용량 개선이 일관되지 않아 원복했다. 설정 예산을 실제 GPU resident bytes나 총 메모리 상한으로 해석하지 않는다.

최종 자동 선택 경로의 desktop iPhone-UA 관찰은 **진단 OFF, 이미지 구간 왕복·theme/탭 전환 41회, 606.553초, 오류·탭 종료 없음**이었다. 실제 iOS WebKit 또는 iPhone 메모리 안정성 PASS로 대체하지 않는다. 추가 M 단계는 실패한 fixture 준비 2건을 포함해 browser 예산 30/30회를 사용했고, 기존 P 단계 24회와 구분한다.

## 남은 작업과 판정 원칙

1. AnimatedBuilder/semantics 내부 self time을 세분화하고 정확한 input→scene 인과 연결 및 빠진 결정적 계약·corpus를 확보한다.
2. 실제 iPhone에서 종료 당시 URL·자산·동작을 맞추고 foreground 관찰과 crash 원인을 확인한다. 기본 WebGL 안정화, 공용 메모리 개선, WebGPU 원인 해결을 별도로 판정한다.
3. 최종 WebGPU 경로의 주요 지연 5% gate를 충족하는지 검증한다. 실제 기기의 IME·주소창·방향 전환·복귀 및 물리 입력·표시 지연은 로컬 자동화 결과와 구분한다.
4. managed live estimate, WASM capacity, process/GPU resident memory, cache 추정 bytes를 구분한다. 측정 불가는 `notMeasured`, 미검증은 `notVerified`, workload 불일치는 `notComparable`로 남긴다.

후속 실행도 후보별 비교와 제한된 회귀를 사용하며 GPU 작업은 순차 실행한다. 원문 기준은 후보 3쌍 비교, 단계별 browser 예산 30회 이내, 자동 retry 0, build/publish/test/browser runner의 20분 process-tree timeout이었다.

## 상세 근거와 재현 자료

- [일반 화면 처리 비용 결과](../../Doroti/validation/web-frame-cost/results-2026-09-22.md): 환경·source/asset hash·후보 A/B·검증·미완료 경계.
- [일반 화면 처리 비용 재현 방법](../../Doroti/validation/web-frame-cost/README.md).
- [Web 메모리·iPhone 안정성 결과](../../Doroti/validation/web-memory/results-2026-09-22.md): 기기 정보·후보별 결정·수명/픽셀·관찰·최종 gate.
- [Web 메모리 검증 방법](../../Doroti/validation/web-memory/README.md).
- [과거 same-work 실험](../26-09-08/wasm-same-work-execution.md), [과거 작업 계획 요약](../26-09-08/work-plans-summary.md): 별도 역사 자료이며 이번 성능 PASS 근거로 재사용하지 않는다.

`Doroti/artifacts/web-frame-cost/`, `Doroti/artifacts/web-memory/`, `Doroti/artifacts/iphone-memory-review/`는 당시 원시 산출물 경로이자 삭제 가능한 로컬 자료다. 이 요약은 남아 있는 계획·추적되는 결과 문서를 근거로 하며 원시 산출물의 현재 존재나 재검증을 주장하지 않는다.
