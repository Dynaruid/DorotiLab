# Sample2 Firefox 슬라이더 조사 — 2026-10-06

**수정·Release AOT 게시·Firefox/Chrome 기능 회귀 PASS.**
측정값과 source/payload hash는 [receipt](2026-10-06-firefox-sliders.json)에 보관했다.

이 기록은 최초 슬라이더 수정의 검증이다. 이후 사용자에게서 전체 WebGL 렌더링과
연속 스크롤 지연이 계속 보고되어, 설치된 Firefox 157·DPR 2와 공용 그림 캐시를
[후속 조사](2026-10-06-firefox-rendering.md)에서 별도로 확인했다.
아래 DPR 1 자동 검증 PASS를 전체 Firefox 사용성 해결로 해석하지 않는다.

## 원인과 수정

Components의 Volume 변경은 최상위 `CupertinoSampleState.setState`를 호출했다.
슬라이더를 움직일 때마다 `CupertinoApp`, 탭 scaffold, 섹션·버튼·텍스트가 다시 빌드됐다.
Variable Blur도 강도 변경마다 페이지 전체와 `SliverChildBuilderDelegate`를 새로 만들었다.
별도 allocation profile에서 Components는 한 드래그에 `Text` build 2,556회,
`CupertinoListSection` 426회, Variable Blur는 `Text` 3,960회와 `Container` 1,920회를 확인했다.
GPU raster만이 아니라 build·semantics·임시 할당 비용이 컸다.
화면 갱신 범위만 줄인 후보에서도 Firefox의 고빈도 입력 대기 p95가 Volume 약 2초,
블러 약 0.2~0.4초로 남았다. 별도의 공용 입력 전달 수정이 필요했다.

- Volume은 부모 State가 소유·해제하는 `ValueNotifier<double>`와
  `ValueListenableBuilder`로 값 표시·슬라이더만 갱신한다.
- 블러 강도도 같은 구조로 컨트롤과 `BackdropFilter`만 갱신한다.
  리스트 widget/delegate를 재사용하고 `RepaintBoundary`로 별도 페인트한다.
  테마 변경은 리스트 항목의 inherited dependency를 통해 계속 전달된다.
- 공용 Web presenter는 context 생성 시 얻은 texture/renderbuffer dimension limit을
  보관해 매 프레임 `getParameter` 두 번을 반복하지 않는다. context 소유권과
  크기·메모리 admission은 유지한다. 이 변경만 따로 적용한 시험에서는 일관된 성능
  개선이 없었으므로 아래 개선율을 GPU 조회 제거의 효과로 해석하지 않는다.
  GPU 조회가 동기 대기를 만들 수 있다는 근거는
  [MDN WebGL best practices](https://developer.mozilla.org/en-US/docs/Web/API/WebGL_API/WebGL_best_practices#avoid_blocking_api_calls_in_production)에 있다.
- 공용 DOM→Worker bridge는 이전 move 묶음이 dispatch될 때까지 후속 move를 모은다.
  Worker의 한 task 안에서 각 원래 packet을 순서대로 dispatch하므로 pointer 좌표·압력·
  timestamp와 **모든 contiguous input sequence**를 보존하면서 중간의 반복 frame 작업을 줄인다.
  up/cancel·다른 pointer/host/buttons/modifiers·key/focus/text/wheel·metrics/snapshot 전에는
  남은 move를 먼저 전송한다. 최대 128개 sample 수집 뒤에는 버리지 않고 전송한다.
  owner 종료/교체 시 pending 상태를 비운다. protocol v5의 선택 capability를 협상하고,
  지원을 알리지 않은 이전 endpoint는 원래 즉시 입력 경로를 사용한다.

## 메모리 관찰

수정 전 유효한 Firefox WebGL 실행에서 Components의 Ganesh cache는 약 14MB,
Variable Blur 드래그 뒤에는 약 83MB였다. 블러 탭에서 5.5초 유휴 뒤 약 13MB로 줄었다.
기존 owner의 5초 deferred purge 경로가 작동했다. WASM heap capacity는 약 300MB로
고정됐고, text resource cache는 각각 12/17개로 한도 256개 안에 있었다.
최종 Firefox 실행에서도 capacity가 같았으며, 블러 cache는 마지막 드래그 뒤 약 73MB에서
5.5초 유휴 뒤 약 13MB로 줄었다. 최종 managed heap snapshot은 약 25~39MB였다.
강제 GC·매 프레임 GPU purge·cache budget 축소는 추가하지 않았다.

이는 Skia가 보고한 cache 사용량과 WASM 선형 메모리의 짧은 실행 관찰이다.
드라이버·브라우저 GPU process 전체 메모리나 장기 누수 부재를 입증하지 않는다.

## 검증 방법

같은 Sample2 Release Mono WASM AOT, threads/SIMD, CDN fonts, 720×840·DPR 1의
새 browser context에서 두 탭을 검사한다. 실제 pointerdown으로 thumb를 잡고
main rAF마다 synthetic move를 144개 전송한다. 같은 값 경로와 최종값, 탭 왕복 후
Volume/강도 보존, renderer·hardware GPU, runtime error, cache 한도를 검사한다.
qualification host의 main rAF는 약 165Hz로 드래그당 약 0.86초다.
Worker의 numeric timing ring과 `CaptureCostDiagnostics`를 사용한다.
`--profile`은 원인 귀속에만 사용하고 성능 비교에서는 끈다.
runner에는 Playwright와 Pillow, 해당 Playwright Firefox binary가 필요하다.

초기 probe 두 실행은 thumb를 정확히 잡지 못하거나 최종값이 이전과 같다는 잘못된
oracle 때문에 제외했다. 유효한 비교는 `baseline-firefox-r3`부터다.
과거 AOT 게시 소스와 현재 HEAD 사이 제품 실행 코드 차이는 없고 AOT 기본 설정과
문서만 달랐다. baseline도 이미 `RunAOTCompilation=true`였다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet publish samples/DorotiSampleApp2/web/DorotiSampleApp2.Web.csproj -c Release --nologo --artifacts-path ./temp/testing/firefox-sliders/fixed-aot
python Doroti/eng/run-with-timeout.py --timeout 1200 temp/testing/platform-audit/all/venv/Scripts/python.exe Doroti/tests/web_sample2_sliders.py --root temp/testing/firefox-sliders/fixed-aot/publish/DorotiSampleApp2.Web/release/wwwroot --out temp/testing/firefox-sliders/fixed-firefox --browser firefox --runs 2
```

## 결과

Firefox **155.0 headless**, hardware WebGL2에서 두 번씩 측정했다.
아래 p95는 각 실행의 p95 두 값의 중앙값이고, 할당은 드래그당 전체 managed 할당의
중앙값이다. 각 실행에 같은 144개 이동 샘플이 전달됐고 timing ring overflow는 없었다.
framework callback 시간에는 중첩된 WebGL raster 시간이 포함되므로 두 시간을 더하지 않는다.

| Firefox / 항목 | 수정 전 | 최종 수정 후 |
| --- | ---: | ---: |
| Volume Worker 입력 대기 p95 | 3,944.3 ms | 42.9 ms |
| Blur Worker 입력 대기 p95 | 2,496.4 ms | 19.3 ms |
| Volume framework callback p95 | 26.03 ms | 11.84 ms |
| Blur framework callback p95 | 33.41 ms | 15.70 ms |
| Volume managed 할당 / 드래그 | 215.59 MB | 33.00 MB |
| Blur managed 할당 / 드래그 | 156.46 MB | 36.97 MB |
| Volume raster p95 | 6.45 ms | 5.70 ms |
| Blur raster p95 | 8.56 ms | 9.06 ms |

개선의 중심은 입력 대기와 framework 작업·임시 할당이다. 블러 GPU raster 자체가
빠르게 바뀐 것은 아니다. 이 수치를 물리 입력→표시 latency나 표시 FPS로 사용하지 않는다.

| 최종 게시물 / 실행 | 결과 |
| --- | --- |
| Firefox 155 / WebGL2, 두 탭 각 2 drag | PASS |
| Chrome 154.0.8037.93 / WebGL2, 두 탭 각 1 drag | PASS |
| Chrome 154.0.8037.93 / WebGPU, 두 탭 각 1 drag | PASS (기능 회귀 범위) |
| TypeScript compile + 게시 JS hash 일치 | PASS |
| pointer admission + rendering/lifecycle/startup/managed connection | 41/41 PASS |
| 최종 Release Mono WASM AOT 게시 + 앱/Cupertino AOT 등록 | PASS |

세 browser 실행에서 page/console error는 0개였다. 모든 이동 sample 144개 보존,
최종 Volume 15%/강도 5, 탭 왕복 후 값 유지, 재사용 리스트의 다크 테마 갱신과
900×740 resize를 확인했다. Firefox 다크 리스트와 Chrome WebGPU resize 캡처를
시각적으로 확인했다. WebGPU의 최대 submit interval 356ms는 별도 프레임 페이싱 조사
대상이며 이번 WebGPU PASS는 기능 회귀 범위다.

각 직접 검증은 1,200초 process-tree timeout을 사용한다.
물리 입력, 실제 표시 FPS/scanout, 사용자 Firefox profile, 모바일·Apple·장기 안정성은
이번 자동 검증의 범위에 포함하지 않는다. 설치된 Firefox 157 사용자 profile을 자동으로
조작한 결과가 아니라 별도 Playwright Firefox 155 실행 결과다.

원시 자료는 삭제 가능한 `temp/testing/firefox-sliders/`에 보관한다.
최종 실행은 `final-firefox/`, `final-chrome-webgl/`, `final-chrome-webgpu/`,
빌드 로그는 `fixed-aot.log`, `final-aot.log`, `final-assets.log`, 계약 로그는
`final-contracts.log`다. 초기 무효 probe와 중간 후보는 최종 PASS나 비교 수치에 포함하지 않았다.
