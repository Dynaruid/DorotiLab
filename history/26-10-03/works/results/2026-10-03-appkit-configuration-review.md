# AppKit configuration review — 2026-10-03

대상: [work5](../../../26-10-02/work5-summary.md)·[work6](../../../26-10-02/work6-summary.md)의 macOS AppKit 경로. 상태: **보강·native 자동 검증 PASS, 전체 인수 PARTIAL.** 앞선 [Apple 검토](2026-10-03-apple-frame-configuration-review.md)의 payload와 결과는 보존했다.

## 확인한 미흡 사항과 수정

1. **창 간 연결 차단:** [Metal view](../../../../packages/platforms/maui/Doroti.Host.Maui/DorotiMacOSMetalView.cs)의 전역 retiring 집합이 비어야 새 view를 연결할 수 있었다. 다른 창의 지연·실패가 독립적인 queue/recorder까지 막는 조건을 제거하고, [surface](../../../../packages/platforms/maui/Doroti.Host.Maui/DorotiMacOSMetalSurface.cs)의 이전 retirement task로 같은 owner의 교체만 제한했다. 살아 있는 연결을 새 view로 덮어쓰는 것도 거절하며, 회수 중 view는 reference identity로 강하게 유지한다.
2. **분리 시 입력 상태:** handler disconnect와 native window detach에서 눌린 키의 합성 key-up을 전달하고 trackpad 상태·focus를 정리한다. 연결·분리는 AppKit 소유 스레드에서 수행한다.
3. **종료 후/background 진단:** snapshot이 해제된 device의 이름이나 MTKView 속성을 읽을 수 있었다. device 이름·사용하는 pixel format은 캐시/상수로 기록하고, allocation 조회와 resource 해제는 같은 lock으로 보호한다. 회수 후 allocation 값은 `null`이다. 정상 terminal completion에서 drawable wrapper를 회수하며, ordered-out 콘텐츠 drawable은 Show 또는 종료까지 유지한다.
4. **backing factor 불일치:** layout·paint·preparation·pointer가 screen factor를 사용하고 환경/desktop state는 window factor를 사용했다. window의 backing factor로 일치시켰다. AppKit의 [NSWindow.backingScaleFactor](https://developer.apple.com/documentation/appkit/nswindow/backingscalefactor)를 framework의 DPR 입력으로 사용하며 일반 좌표 변환 API를 새로 구현한 변경은 아니다.
5. **숨김/복귀:** native window가 없거나 최소화·앱 숨김·첫 콘텐츠 준비 또는 visible 제출 후 ordered-out 상태일 때 draw가 새 GPU 작업과 drawable 재시도를 만들지 않게 했다. 대기 wake는 유지한다. [desktop host](../../../../packages/platforms/maui/Doroti.Host.Maui/AppKitDesktopWindowHost.cs)의 Hide/최소화는 MAUI `Stopped`, Show/복원은 `Resumed`를 전달하고 복원 시 frame을 요청한다.
6. **검사 공백:** testbed 전용 [lifecycle fixture](../../../../samples/DorotiTestbedApp/macos/AppKitFrameLifecycleProbe.cs)를 추가하고 [macos_smoke.py](../../../../Doroti/tests/macos_smoke.py)의 기본 검사에 포함했다. 제품의 Metal owner·queue·GPU terminal callback을 사용한다. 합성 키는 SDK의 [NSEvent.KeyEvent](https://learn.microsoft.com/en-us/dotnet/api/appkit.nsevent.keyevent?view=net-macos-26.5-10.0)로 생성한다.

숨김 보강의 첫 후보는 초기 layout의 빈 GPU 프레임을 실제 콘텐츠 준비로 간주하여 `ReadyToShow`가 끝나지 않는 회귀를 만들었다. 전체 multiwindow 검사에서 native GPU 완료 1 / framework 제출 0 / visible window 0으로 재현했다. 실제 hidden 콘텐츠의 유효한 completion 또는 visible submit 이후에 차단하도록 수정했고, 새로 빌드한 최종 후보의 전체 Graphite/Ganesh 검사를 통과했다. 첫 fixture 빌드의 obsolete `ReleasedWhenClosed` 사용 오류도 제거했다. 두 실패의 로그와 요약은 JSON에 보존한다.

## 검증

환경: macOS 26.6 arm64 / Apple M1, .NET SDK 10.0.401, Xcode 27.0 (27A266a), Apple workload 27.0.10722, MAUI 10.0.90, SkiaSharp 4.154.0-preview.1.26454.9. 모든 aggregate 명령은 `.github/copilot-instructions.md`에 따라 1,200초 wrapper를 사용했다.

| 검사 | 결과 | 확인한 범위 |
| --- | --- | --- |
| 이전 Apple payload AppKit baseline | PASS | 기존 6 case / 두 lifetime으로 7개 결과. bundle hash가 앞선 Apple 보고서와 일치 |
| Testbed AppKit Debug build | PASS, 경고·오류 0 | `net10.0-macos27.0`, `osx-arm64`, 최종 수정과 fixture |
| Graphite 전체 native smoke | PASS, 7 case / 8개 결과 | 두 lifetime의 다중 창·생존 창 resize, desktop/minimize/fullscreen/materials/close, services, native input/WKWebView, navigation/restoration, rendering, lifecycle |
| Ganesh 전체 native smoke | PASS, 7 case / 8개 결과 | 동일 기능 검사 및 C 내부 single-frame capability |
| 두 backend 24회 resize | PASS | Graphite 최대 pending 2 / 완료 76, Ganesh 최대 1 / 완료 79; 각각 마지막 pending 0·오류 0·표시 callback 46. window extent × DPR 일치 |
| 두 backend owner retirement | PASS | 실제 제출의 pending 1인 UI turn에서 같은 surface 교체 거절, 독립 owner 연결 허용, callback 이후 같은 surface 새 view 연결·렌더, 최종 retiring owner 0 |
| 두 backend snapshot/입력 수명 | PASS | background snapshot 20회 및 회수 후 cached device/null allocation; disconnect와 native detach의 합성 key-up, focus 해제 |
| 두 backend 숨김·복귀/reattach | PASS | hide/show 3회; hidden/detached에서 명시적인 redraw 요청에도 제출 증가 없음, 복귀/재부착 후 completion 증가 |
| 두 backend 합성 backing factor | PASS | 1 → 1.5 → 2, logical 320×220 / pixels 320×220 → 480×330 → 640×440 |
| Sample2 AppKit Debug build | PASS, 경고·오류 0 | 동일 host를 사용하는 실제 앱 소비 프로젝트와 native binding 컴파일 |
| 소스 정합성 | PASS | `git diff --check`, smoke script Python compile, 최종 source/bundle identity |

재현:

```sh
python3 Doroti/eng/run-with-timeout.py dotnet build samples/DorotiTestbedApp/macos/DorotiTestbedApp.MacOS.csproj -c Debug -r osx-arm64 -p:DorotiMacOSTargetFramework=net10.0-macos27.0
python3 Doroti/eng/run-with-timeout.py python3 Doroti/tests/macos_smoke.py --skip-build --output temp/testing/appkit-review/reproduce-graphite
python3 Doroti/eng/run-with-timeout.py python3 Doroti/tests/macos_smoke.py --skip-build --renderer ganesh --output temp/testing/appkit-review/reproduce-ganesh
```

## 증거와 남은 인수

[JSON](2026-10-03-appkit-configuration-review.json)에 baseline·최종 두 앱의 전체 bundle manifest, source hash, native 집계, 실패 진단과 raw hash를 저장했다. raw는 `temp/testing/appkit-review/`의 삭제 가능한 로컬 산출물이다. 이전/최종 후보의 짧은 callback 개수 차이를 FPS나 성능 회귀 판정으로 사용하지 않는다.

owner fixture는 메인 루프에 실제 GPU callback의 owner 회수가 남은 시점을 검사하며 GPU 실행을 강제로 막지 않는다. hardware overlap·실제 표시 FPS/입력 지연은 미측정이다. 키 입력과 backing factor는 합성 검사이며 실제 모니터 이동·물리 키보드/IME·VoiceOver/Finder 인수는 미검증이다. 최소화/복원은 전체 desktop 상태 검사로 확인했고 새 hide/resume fixture와 구분한다. GPU 실패/context loss의 강제 주입, Sample2 실행/정량 성능, 10분 사용, 최소 지원 OS·package-only/서명·공증·clean OS 배포는 이번 통과 범위에 포함하지 않는다. 앞선 실제 Metal 31조건 검사는 별도 Apple 후보의 결과로 유지한다. Qt 동적 texture 예산 범위도 변경하지 않았다.
