# 2열 → 1열 → 2열 뒤 오른쪽 열 누락 수정

2026-09-07. 사용자 화면과 동일하게 양쪽 열을 스크롤하고 1열에서 더 내려간 뒤
2열로 돌아왔을 때 오른쪽이 회색으로 남는 현상을 재현했다.

공용 `Element.activate()`가 nullable `_dependencies`를 무조건 Clear하여
NullReferenceException을 발생시켰다. inherited dependency를 등록한 적 없는
StatefulBuilder 등도 GlobalKey로 부모를 옮길 수 있다. 의존성이 없으면 정리를
건너뛰도록 `_dependencies?.Clear()`로 수정했다. 기존 dependency가 있는 위젯은
그대로 새 inherited 값을 구독한다. 샘플 캐시나 GlobalKey를 제거하지 않았다.

## 검증

- 최초 browser 재현: 오른쪽 crop의 색상이 1개뿐이고 Element.activate 예외가
  발생하여 FAIL (`column-return-before`). 원본 screenshot/trace/log 보존.
- 수정 후 반복 전환과 오른쪽 실제 픽셀 검사, 기존 선택/테마/DOM 유지 2 PASS
  (`column-return-after`). 기존 회귀 검사가 왼쪽 선택 유지에 집중해 놓쳤던
  오른쪽 표시 검사와 깊게 스크롤한 조건을 추가했다.
- native: dependency가 없는 State와 있는 State를 세 차례 부모 사이로 이동해
  동일 State 유지와 새 MediaQuery 값 구독을 확인했다. 전체 Windows sample
  picker/image/app bar/drawer 회귀도 PASS (`column-return-native-final`).
- native fixture 최초 실행은 locale 지정 누락으로 실패했으며, 기존 fixture와
  동일하게 en-US를 명시한 뒤 재검증했다 (`column-return-native` 실패 보존).
- Web Release build PASS. 새 소스로 5088 runner 재시작 완료.
- 모든 검증은 20분 timeout, retry 0. 별도 publish와 물리 창 조작은 이번 수정에서
  재검증하지 않았으며 이전 publish PASS를 이번 변경의 결과로 재분류하지 않는다.

로그: `Doroti/artifacts/framework-web-work2/column-return-*`.
브라우저 산출물: `Doroti/validation/web-playwright/artifacts/column-return-*`.
강화 검사 중 `column-return-final`은 복귀 직후 화면 밖 Image demo의 Cover가
semantics에 존재한다는 가정 때문에 2 FAIL이었다. 복귀 화면을 확인해 오른쪽이
Navigation 상단으로 돌아오는 것을 확인했고, 현재 화면의 실제 픽셀과 스크롤
반응을 검사하도록 수정했다. 화면 밖 Image demo의 상태 보존을 PASS로 주장하지
않는다. 공용 State 재부착 자체는 native의 명시적 부모 이동 fixture로 검사한다.
최종 DPR 1/2 및 오른쪽 스크롤 반응 결과는 아래에 기록한다.

최종 `column-return-verified`: DPR 1/2 모두 각각 두 번의 깊은 스크롤 및
2열→1열→2열 전환을 마치고 오른쪽 픽셀과 아래 방향 스크롤 반응 확인, 2 PASS.
브라우저 runtime error는 0이었다. 5088은 수정된 source build를 제공한다.
