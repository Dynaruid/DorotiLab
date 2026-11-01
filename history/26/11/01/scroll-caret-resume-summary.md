# 스크롤·커서·백그라운드 복귀 개선 작업 요약

원문 작성: 2026-10-31. 후속 작업·요약 정리: 2026-11-01 (Asia/Seoul).
원문의 검토 기준은 `de6664ed913e733673b1adef9c9771c4be210d75`, 마지막 후속 작업 기준은
`a3ce8df0f6afcb2f5ebbbcb9515e68dca757e71d`와 미커밋 수정이다.

**최종 상태:** Android와 기본 Windows의 백그라운드 복귀 후 스크롤은 사용자 체감 확인을
통과했다. 전체 frame pacing은 `PARTIAL`이며, 표시 deadline·활성 TalkBack·별도 Windows
트랙패드 검증의 미확인 범위는 남는다. 이번 정리는 문서 보관 작업이며 빌드나 제품 검증을
다시 수행한 결과가 아니다.

삭제한 루트 `work.md`의 계획·중간 결과·체크리스트는 [원문 보존본](work.original.md)에
바이트 그대로 남겼다. 원문 안의 상대 링크 기준은 **삭제 전 저장소 루트**다.
이 요약과 아래 세부 기록의 링크는 현재 위치를 기준으로 사용할 수 있다.
원본 크기·SHA-256과 보관 검증은 [보관 명세](work-archive-manifest.json)에 기록한다.

## 문제와 구현 범위

갤럭시에서 최초 실행은 부드럽지만, 이미 로딩된 목록을 백그라운드에 두었다 복귀하면
긴 관성 감속에서 잔끊김이 지속됐다. 커서 핸들 드래그의 지연도 함께 조사했다.
전체 화면과 목록이 실제로 움직이는 느낌을 최종 기준으로 삼고 공통 엔진, 플랫폼,
입력→scene→GPU 제출 경계를 나눠 검토했다. lazy loading의 최초 비용과 로드 완료 후
반복 스크롤의 비용을 구분했으며, 기다리는 대신 GC·메모리 정리를 유도해 복귀를 확인했다.

| 영역 | 적용한 주요 수정 |
| --- | --- |
| 공통 엔진·녹화 | 입력 시각과 release velocity 보존, scheduler/ticker의 관성 연속성 회귀, 행렬·semantics·녹화의 반복 할당 축소, 닫힌 picture 명령 저장소 공유와 자원 수명 보존 |
| 커서·선택 | OS 확대경 적용과 모양 조정, 변하지 않은 글자 좌표 재사용, 선택만 바뀔 때 Material 장식과 화면 전체 갱신 억제, 공유 컨트롤러와 자식 수명 수정 |
| Android Surface·목록 | 살아 있는 view의 GPU context 유지와 Surface/swapchain 교체 분리, 메모리 압박 시 미사용 자원 정리, keep-alive 이동의 하위 트리 재부착 제거, 소비자 상태에 따른 semantics 작업 축소 |
| Android 콜백·GC | 숨은 입력칸의 지연 선택 콜백과 native 프레임 알림의 참조 수명 정리, 필수 비동기 작업의 관리 소유권 유지로 큰 Mono/Java GC bridge 탐색 감소 |
| Windows | 두 호스트의 retained 장면 검사를 인덱스 순회로 변경, MAUI UIA 자식 목록의 native 참조·순서가 같을 때 재사용; 구조·handler 변경과 늦게 준비되는 peer는 다시 반영 |

## 단계별 결과와 근거

아래 수치는 각 기록의 지정 구간에서 측정한 owner 처리 시간·관리 할당이다.
서로 다른 차수의 값들을 하나의 통제된 비교나 표시 FPS로 합치지 않는다.

| 작업 | 기록된 결과 | 세부 기록 |
| --- | --- | --- |
| 공통·Android 스크롤 및 SkiaSharp | `4.154.0` 정식판 전환, Release arm64·회귀·Windows 실행 통과. 당시 갤럭시 owner 최대 56.8ms, 별도 연속 드래그 65.1ms로 전체 성능은 PARTIAL | [10/31 구현·정식판 검증](../../10/31/galaxy-scroll-implementation.md) |
| 커서 핸들 드래그 | 첫 수정의 최대 owner 133.9–167.0 → 38.7–40.1ms. p95는 증가해 지속 드래그 합격은 아님 | [커서 드래그](../../10/31/galaxy-caret-drag.md) |
| 확대경·접근성 후속 | 화면 전체 갱신 등을 줄인 비교의 드래그 p95 36.4–37.6 → 21.2–26.5ms. 별도 필드의 개선 폭과 긴 outlier는 따로 보존 | [확대경·접근성 검토](../../10/31/galaxy-caret-accessibility-review.md) |
| All Components 커서 지연 | 최종 APK 3회의 owner p95 51.5–53.1 → 25.8–28.5ms, 50ms 이상 작업 25 → 0회. 키보드 재열기·한글·범위 선택·포커스/복귀 검증 | [실제 페이지 수정](galaxy-components-caret-fix.md) |
| Android 초기 복귀 수정 | GPU context 재생성 경로 분리. GC/trim 유도, 에뮬레이터 복귀 6회, Activity 재생성의 상태 보존·GPU 종료·arm64 빌드 검증 | [Surface·복귀 수정](android-resume-scroll-fix.md) |
| 갤럭시 긴 관성의 중간판 | 마지막 관성 구간 owner p95 11.384 → 6.362–6.514ms, Gen0 GC 19 → 6–7회. 92.492ms 지연과 두 번째 trim exit 255가 남아 당시 중단·미검증 항목을 보존 | [중간판 및 재개 기록](android-resume-inertia-followup.md) |
| 갤럭시 재개 최종판 | 두 프로세스의 복귀 4회에서 관성 최대 owner 27.779ms, 50ms 이상 0회. 강제 GC/trim 40→60, Activity 재생성, 최종 GPU 종료 통과. 사용자가 복귀 후 부드러워졌다고 확인 | [최종 결과 JSON](android-resume-bridge-results.json) |
| Windows 후속 | 24단 장면 20회 검사 할당 44,640 → 0 bytes. 기본 러너 GC/복귀 3회·native editor·목록 복귀·정상 종료 통과. MAUI도 GC/복귀·UIA 구조 교체·종료 통과. 사용자가 기본 Windows 복귀 후 부드러움을 확인 | [Windows 검토](windows-resume-scroll-review.md), [결과 JSON](windows-resume-scroll-results.json) |

### 최종 전달과 사용자 확인

- Android: Galaxy `SM-S931N`, Android 17/API 37, Release arm64 비교. 보고된 가변 주사율
  10–120Hz는 표시 FPS가 아니다. 최종 APK SHA-256은
  `933c21665bf0ff7d312709f6070c90e148890456759eba22e0f30f43d7f448db`.
  진단 옵션 없는 일반 실행을 전달한 뒤 사용자가 **“복귀 뒤에도 부드러워졌어”**라고 확인했다.
- Windows: 기본 Windows App SDK 수정본을 일반 실행으로 전달한 뒤 사용자가
  **“복귀 뒤에도 부드러워”**라고 확인했다. 입력 기기 종류는 특정되지 않았다.
- 마지막 Windows 후속 기준 회귀 출력 13개와 두 Windows Release 빌드는 통과했다.
  기본 러너의 GC 후 관리 힙은 19.08 → 19.20 → 19.22MB였다.
  MAUI UIA 수정 후 수집은 54.75 → 39.45 → 38.64MB였다. 별도 실행의 힙 관측이며,
  통제된 표시 성능 비교나 전체 메모리 누수 부재의 증명으로 쓰지 않는다.

APK 식별값·빌드·기기 상태는 해당 실행 당시의 기록이다. 최초 설치 APK는 정확한 source
revision을 복구하지 못했으며, 초기 장치/설치 정보와 최종 전달본을 혼동하지 않는다.
이번 보관에서 기기나 실행 중인 앱의 상태를 다시 조회하지 않았다.

## 남은 한계와 원래 합격 기준

- Android 최종판도 별도 정지 후 진단에서 **73.623ms GC 지연**이 남았다. 최종 복귀 관성의
  50ms 이상 0회와 사용자 확인은 해당 구간의 결과이며 전체 frame pacing 합격은 아니다.
- owner 처리 시간, QueuePresent 성공, fence 완료, 제출 cadence는 화면 표시 FPS나
  input-to-display 지연을 대신하지 않는다. 해당 SurfaceView/layer의 실제 표시 연결과
  deadline miss 검증은 남는다.
- 활성 TalkBack의 action/focus, 별도 물리 Windows 트랙패드 관성은 `notVerified` 범위를
  유지한다. 사용자 체감 확인을 이 항목들의 자동 승인으로 확대하지 않는다.
- 초기 계획의 목표는 앱 귀속 50ms 이상 stall 0회, 설명되지 않는 2T 초과 새 frame 공백 0회,
  표시 연결 가능한 구간의 deadline miss 1% 미만, 주요 owner p95 ≤ 0.8T였다.
  `T`는 실제 주사율의 기대 간격이며, 이 수치들은 전체 달성 결과가 아니다.
- 원문의 미체크 항목은 당시의 복합 완료 조건이다. 이후 수행된 GC/trim·재생성·사용자
  확인은 위 최종 기록으로 구분하며, 보관을 이유로 과거 체크리스트를 일괄 완료 처리하지 않는다.

후속 성능 조사가 필요하면 마지막 APK/바이너리와 정확한 입력·화면을 먼저 고정하고,
공통 엔진·플랫폼·연결 경계의 지연을 함께 추적한다. 관성 시간·마찰·정지 tolerance를
바꿔 증상을 숨기거나 in-flight 수만 늘리지 않는 원칙을 유지한다.

## 재현 자료와 보관 범위

- [Scroll 회귀 설명과 실행 명령](../../../../Doroti/tests/Doroti.Scroll.Tests/README.md).
- [Android 복귀 측정 도구](../../../../Doroti/eng/measure-android-resume-scroll.py)와
  [집계 도구](../../../../Doroti/eng/summarize-android-resume-scroll.py).
- 실행 당시 명령·source/binary hash·회차별 집계·검증 경계는 위 세부 Markdown/JSON에 보존한다.
  테스트·빌드에는 저장소 지침의 20분 timeout을 적용한다.
- `temp` 및 `Doroti/artifacts`의 APK·trace·dump·로그는 삭제 가능한 로컬 산출물이다.
  이번 요약은 그 파일들의 현재 잔존이나 기기 설치 상태를 보증하지 않는다.

원문 계획과 중간 기록을 보존한 후 루트 `work.md`만 삭제했다. 기존 코드 수정과 상세
검증 기록은 유지하며, 새 코드 작업·기기 조작·성능 측정·커밋은 수행하지 않았다.
