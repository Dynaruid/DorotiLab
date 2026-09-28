# M2-C. 렌더링 성능과 리소스 수명

원문: [개발 로드맵](../../plan.md) §3 M2-C · 우선순위: **P1** · 작업 상태: **PARTIAL** · 실행 검증: CPU/정책 회귀 **PASS**, 전체 GPU·기기 예산 **notMeasured**

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

### 2026-09-28 구현·검증

revision: `a93c047fe2e93d93cff3e0a6bf3c2789862fea81` + 이번 working tree. Windows 10.0.26200, .NET SDK 10.0.400, Debug net10.0, Skia CPU offscreen; Web 정책 테스트 Node 24.15.0. [장면·측정 경계·예산](../../Doroti/docs/rendering-baselines.md)을 고정했다. GPU 완료·CPU raster·실제 present는 서로 대체하지 않는다.

- `RenderingRegressions`에 VariableBlur halo/capture origin/반복 tile/홀수 backing, 6개 size×DPR 픽셀 비교, 1,000행 pointer 스크롤·가상화, reassemble 후 offset 유지, 종료 후 engine-layer 잔여 0 검증을 추가했다.
- 테스트에서 발견한 기본 CupertinoScrollbar의 nullable thickness assertion을 실제 기본값으로 수정했다. 가상 시계와 scroll/metrics quiet-window의 실시간 시계가 달라 settle이 끝나지 않던 문제를 고쳤다. 측정용 monotonic clock은 유지했다.
- reusable RenderView가 호스트 종료 뒤 루트 레이어를 남기던 문제를 영구 종료 경로에서 해제하도록 수정했다. 일반 widget 교체에서의 재사용 계약은 유지한다.
- Web resize admission을 제품 코드의 `ResizeAdmissionWindow`로 분리했다. 30개 연속 요청에 전송 4 + 대기 최신 1, 알 수 없는/중복 ack 무시, reset 후 재개를 검증했다. 모바일 정확한 초기 backing, 지연 축소, 회전 중 최대 면적 2,592,000px 정책 테스트도 통과했다. 이는 실제 VRAM 계측이 아니다.
- 템플릿의 삭제된 `NanumGothic-Regular.ttf` preload를 제거했다. 생성한 Web 앱을 실제 Chrome에서 표시하고 버튼 0→1, `한글 렌더 확인` 입력·glyph 표시, 450×800 리사이즈 후 값 유지와 오류 로그 없음까지 확인했다. 브라우저 자동입력은 물리 IME 조합 증거가 아니다.

명령: `python Doroti/eng/run-with-timeout.py dotnet run --project Doroti/tests/Doroti.Tests -c Debug`, `python Doroti/eng/run-with-timeout.py node --experimental-transform-types --test Doroti/tests/web_rendering.mts`. 모든 회귀 **PASS**. 공통 Developer suite/CI에도 연결했다.

6개 CPU resize 표본의 전체 dispatch+surface 재할당+raster p95는 **1.8107 ms**였다. 단계별 범위는 build **0.002–0.043 ms**, layout+compositing **0.008–0.051 ms**, paint **0.003–0.037 ms**, CPU raster **0.018–0.063 ms**. 작은 고정 장면·단일 로컬 실행의 baseline이며 GPU 성능 향상률이나 표시 FPS로 일반화하지 않는다. teardown 후 engine-layer delta=**0**, 검사 시 text cache estimate=**1,536 bytes**, raster cache=**0/67,108,864 bytes**. 최초 실패에서는 tester당 루트 engine layer **1개**가 남았고 수정 후 **0개**다.

남은 검증: VariableBlur GPU 픽셀/품질·다중 효과·WebView overlay·실제 영상 texture의 같은 기기 전후 timing, 실제 present/VRAM, 모바일 실기기 memory/lifecycle/context/device loss, CDN/local/offline 전체 조합과 cold-start 글꼴 교체. 이 항목은 **notVerified/notMeasured**이며 전체 완료로 체크하지 않는다. raw evidence는 `temp/testing/m4-m5/`의 disposable 파일이고 유지되는 결과는 본 문서다.
