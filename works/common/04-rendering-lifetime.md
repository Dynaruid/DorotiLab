# M2-C. 렌더링 성능과 리소스 수명

원문: [개발 로드맵](../../plan.md) §3 M2-C · 우선순위: **P1** · 작업 상태: **TODO** · 실행 검증: **notVerified**

[작업 인덱스와 공통 완료 규칙](../README.md)

## 담당 범위와 선행 작업

공통 geometry·측정 항목·리소스 수명 회귀를 담당한다. 예산과 실측 결과는 기기·호스트·renderer별로 남긴다.

선행: [M1](01-testing.md)의 최소 회귀 경로와 검증 가능한 대상 renderer.

플랫폼별 연결·검증: [Windows](../platforms/windows.md) · [Web](../platforms/web.md) · [Android](../platforms/android.md) · [macOS / AppKit](../platforms/macos.md) · [iOS](../platforms/ios.md) · [Linux / Qt](../platforms/linux.md) · [Mac Catalyst](../platforms/maccatalyst.md)

## 원문 작업과 완료 기준

아래는 원문의 체크리스트와 완료 기준을 보존한 것이다. 특정 호스트를 명시한 항목은 연결된 플랫폼 문서에서 해당 구현·검증을 추적하고, 이 문서에는 공유 계약 및 통합 결과를 기록한다. 공통 구현 완료가 모든 플랫폼 검증 완료를 뜻하지 않는다.

- [ ] 대표 장면을 고정한다: 긴 리스트, VariableBlur, 여러 효과, WebView 위 overlay, 이미지/영상 texture, resize/DPR 변경.
- [ ] UI build/layout/paint, GPU 작업, readback/upload, 실제 present 간격, 메모리/VRAM을 구분해 측정한다. 대상 기기·해상도별 baseline과 허용 예산을 먼저 정한다.
- [ ] Web PlatformView CPU upload와 frame admission/backpressure, 모바일 Web 메모리, pause/resume, context/device loss 복구를 우선 점검한다.
- [ ] VariableBlur의 축소 품질·adaptive 처리에서도 논리 capture 영역, pixel origin, viewport 크기를 보존한다. 성능 개선의 완료 조건에 geometry 회귀 검증을 포함한다.
- [ ] 폰트 CDN/로컬 asset/offline 정책, 초기 글꼴 교체와 한글 fallback을 실제 호스트에서 재검증한다.

**완료 기준:** 정한 장면·기기·예산으로 전후 수치를 재현할 수 있고, 성능 변경이 크기·입력·합성 정확성을 깨뜨리지 않는다. GPU 완료 시간만으로 화면 FPS를 주장하지 않는다.

## 산출물과 결과 기록

- 공개 계약·구현 변경, 실행 가능한 샘플, 필요한 회귀 검증, 지원표 갱신을 함께 남긴다.
- 이 문서의 완료 기준과 플랫폼별 적용 결과를 연결하고, 미실행 조합은 `notVerified`로 유지한다.
- 결과는 [공통 기록 형식](../README.md#결과-기록-형식)에 revision·환경·명령·기대값·실제값·남은 작업을 남긴다.

현재 기록: 문서 분리만 수행했다. 이 작업 묶음의 구현·실행 결과는 아직 기록하지 않았다.
