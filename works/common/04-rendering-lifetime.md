# M2-C. 렌더링 성능과 리소스 수명

원문: [개발 로드맵](../../plan.md) §3 M2-C · 우선순위: **P1** · 작업 상태: **PARTIAL** · 실행 검증: CPU/정책 회귀 **PASS**, 전체 GPU·기기 예산 **notMeasured**

[작업 인덱스와 공통 완료 규칙](../README.md)

## 담당 범위와 선행 작업

공통 geometry·측정 항목·리소스 수명 회귀를 담당한다. 예산과 실측 결과는 기기·호스트·renderer별로 남긴다.

선행: [M1](01-testing.md)의 최소 회귀 경로와 검증 가능한 대상 renderer.

플랫폼별 연결·검증: [Windows](../platforms/windows.md) · [Web](../platforms/web.md) · [Android](../platforms/android.md) · [macOS / AppKit](../platforms/macos.md) · [iOS](../platforms/ios.md) · [Linux / Qt](../platforms/linux.md) · [Mac Catalyst](../platforms/maccatalyst.md)

## 원문 작업과 완료 기준

원문의 범위와 완료 기준을 유지하면서 2026-09-28~29 실행 기록에 따라 완료 부분과 잔여 부분을 분리했다. 체크된 항목은 명시한 호스트·검증 범위에 한정하며, 아래 날짜별 결과가 근거다. 공통 구현 완료가 모든 플랫폼 검증 완료를 뜻하지 않는다.

- [x] [측정 기준](../../Doroti/docs/rendering-baselines.md)에 장면·측정 경계를 정하고 긴 리스트·VariableBlur geometry·resize/DPR의 CPU 회귀와 로컬 baseline을 기록했다.
- [ ] 다중 효과·WebView overlay·이미지/실제 영상 texture를 포함한 GPU 장면을 같은 기기에서 비교한다. UI build/layout/paint·GPU·readback/upload·실제 present·메모리/VRAM을 분리 측정하고 기기·해상도별 허용 예산을 채운다.
- [x] Web resize admission의 4 in-flight + latest 1·ack/reset과 모바일 backing 크기·회전 메모리 정책 회귀를 검증했다.
- [ ] 실제 Web PlatformView CPU upload/backpressure·모바일 메모리·pause/resume·context/device loss 복구를 검증한다. 정책 테스트를 기기 계측으로 확대하지 않는다.
- [ ] VariableBlur 축소/adaptive 처리의 GPU 품질과 논리 capture 영역·pixel origin·viewport 보존을 확인한다. CPU geometry 회귀와 별도로 기록한다.
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

### 2026-09-29 수명 회귀 수정

Windows 입력 장면의 native view 재생성 후 이전 scene이 raster에 남아
`stale native identity`로 프로세스를 종료하는 경쟁 조건을 재현했다.
Coordinator가 실제 해제한 handle의 제한된 이력을 유지하고, 해당 scene만 superseded로
종료해 새 framework frame을 요청하도록 수정했다. retained layer 내부의 중첩 command도
동일하게 검사하며, 이 경로를 빠뜨렸을 때의 재현도 별도 회귀로 추가했다. 다른 owner/잘못 만든 handle과
지원하지 않는 geometry는 계속 실패한다. 취소된 raster는 화면에 제출하지 않는다.

해제 handle·같은 ID의 새 generation·다른 owner 분류 회귀를 추가했다.
수정 뒤 WindowsSmoke의 API/native close, editor/WebView 4회 생성·재생성,
실제 2창/editor island·OnLastWindowClosed/Explicit 종료가 모두 **PASS**다.
GPU timing/present/VRAM budget을 측정한 결과는 아니며 기존 notMeasured 항목은 유지한다.


## 2026-09-29 Linux / Qt 후속

Quick 두 QPA의 20회 resize/terminal ACK/종료 후 예약·retiring 0 및 software Vulkan phase 수치를 기록했다. Widgets OpenGL 비교도 PASS. scanout/물리 VRAM/전체 GPU golden은 notMeasured/notVerified.

상세 명령·환경·지원 경계: [Linux Qt 결과](../results/2026-09-29-linux-qt.md).
