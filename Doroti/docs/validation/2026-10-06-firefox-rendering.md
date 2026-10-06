# Sample2 Firefox 렌더링·연속 스크롤 후속 조사 — 2026-10-06

최초 [슬라이더 조사](2026-10-06-firefox-sliders.md) 뒤에도 사용자가 전체 WebGL 렌더링과
WebGPU 연속 스크롤 지연을 보고했다. 추가 자료나 확장 설치를 요구하지 않고 설치된
**Firefox 157.0, 별도 Selenium profile, 실제 창, OS DPR 2**에서 재현했다.
소스·게시물 hash와 측정 요약은 [receipt](2026-10-06-firefox-rendering.json)에 보관한다.

## 원인과 수정

1. **공용 그림 캐시가 프레임 사이 재사용되지 않았다.**
   `DorotiFrozenScene`은 소유권 경계를 지키려고 매 제출의 그림 명령을 복사한다.
   `SkiaPictureCommandCache`와 raster cache/warmup은 명령 목록의 참조를 키로 사용해,
   같은 retained 그림도 매번 새 항목으로 취급했다. 재현 실행에서 command cache는
   128개 warmup 항목까지 차고 native command recording/hit, raster promotion/hit는
   모두 0이었다. 두 캐시와 raster warmup을 기존 `Picture.SnapshotIdentity`로 키잉한다.
   제출의 폐쇄된 명령·이미지 lease 복사는 유지하고, 캐시가 producer 객체를 붙잡지 않는다.
   기존 크기·transform·subpixel phase·font/context generation 검사와 LRU/메모리 한도를 유지한다.

2. **Firefox DPR 2에서 캔버스 backing이 표시 영역의 2.25배였다.**
   1280×899.5 logical viewport에 필요한 backing은 2560×1799인데 desktop headroom이
   3840×2699를 할당했다. Firefox의 두 renderer는 정확한 크기로 증가하고 기존 bounded
   delayed shrink를 사용하는 compact capacity 정책을 적용한다. Skia desktop budget과
   renderer 선택은 유지한다. Chrome/Edge desktop capacity 정책은 기존과 같다.
   RGBA8 color backing 추정은 41,456,640→18,421,760 bytes, **55.6% 감소**다.
   이것은 브라우저·드라이버 전체 VRAM 계측이 아니다.

3. **연속 wheel 입력마다 Worker 작업 사이 프레임이 끼어 입력이 밀렸다.**
   pointer admission에 wheel capability를 별도로 협상한다. 이전 입력 묶음이 dispatch되는
   동안 같은 host/좌표/device/signal의 연속 wheel을 모아 한 Worker task에서 원래 packet을
   순서대로 dispatch한다. 각 deltaX/Y, scale, timestamp, contiguous input sequence를 모두
   보존한다. 다른 target, pinch signal, pointer/key/text/focus, snapshot/metrics 전에는 flush한다.
   최대 128개를 넘으면 버리지 않고 전송한다. 이전 pointer-only endpoint는 wheel을 즉시 보낸다.

4. Components의 섹션과 항상 애니메이션하는 activity indicator/switch를
   `RepaintBoundary`로 분리해 retained 그림을 재사용할 수 있게 한다.
   앞선 Volume/블러 notifier 변경과 리스트 재사용도 포함한다.

WebGL 상태 초기화를 생략하는 후보도 측정했지만 일관된 개선이 없어 제외했다.
강제 GC나 매 프레임 GPU cache purge를 추가하지 않았다.

## 설치된 Firefox 157 결과

같은 Release Mono WASM AOT, threads/SIMD, hardware GPU, 1280×900 창·DPR 2에서
renderer마다 고정 180개 wheel을 main rAF 간격으로 보낸 단일 실행을 비교한다.
처음 120개는 3.5px, 뒤 60개는 지수 감쇠해 총 **473.275463px**를 스크롤한다.
각 실행의 p95이며, 여러 실행의 중앙값이나 실제 표시 FPS가 아니다.
baseline은 최초 슬라이더 수정 게시물이고 후속 cache/capacity/wheel 수정 전이다.
baseline 이후 실제 포인터를 고정 위치에 주차하는 절차를 추가했으므로 완전한 통제된
프레임 비교는 아니다. baseline과 최종의 wheel ingress/dispatch는 모두 정확히 180개다.

| 항목 | 수정 전 | 최종 수정 후 |
| --- | ---: | ---: |
| WebGL Components idle framework callback p95 | 9.30ms | 5.82ms |
| WebGL Components idle raster/submit CPU p95 | 7.96ms | 4.28ms |
| WebGL wheel Worker 입력 대기 p95 | 997.20ms | 29.44ms |
| WebGPU Components idle 제출 간격 p95 | 93.32ms | 10.78ms |
| WebGPU Components idle raster/submit CPU p95 | 9.12ms | 5.08ms |
| WebGPU wheel Worker 입력 대기 p95 | 32.88ms | 10.38ms |
| WebGPU wheel 제출 간격 p95 | 24.88ms | 16.24ms |

최종 Components idle에서 native command hit는 WebGL **11,430**, WebGPU **6,822**였다.
WebGL raster hit도 **635**회여서 두 캐시의 재사용을 직접 확인했다.
WebGL framework callback에는 동기 raster 시간이 중첩되므로 두 시간을 더하지 않는다.
WebGPU raster 시간도 GPU 완료 시간이 아닌 CPU/submit 구간이다.

두 renderer에서 180개 wheel과 델타 합계에 맞는 최종 managed/semantics scroll position,
마지막 wheel까지 반영한 raster submission을 확인했다. 마지막 입력→submit은 각각
18.06ms/22.68ms였다. 각 renderer에서 창 크기 8단계 변경 후 올바른 backing과 Failed 0도 확인했다.
1680×950·DPR 2의 추가 WebGL 실행은 backing 3360×1899, wheel 대기 p95 26.70ms,
마지막 입력→submit 18.16ms였고 같은 입력·위치·resize 검증을 통과했다.

캐시 수정 직전/직후의 같은 scroll position screenshot은 WebGL RGB 평균 절대 차이
0.0000067/255, WebGPU 0.0071/255였다. 스크롤된 행·블러·텍스트를 시각적으로도 확인했다.
이는 해당 정지 화면 비교이며 모든 중간 프레임의 시각적 동등성 증명은 아니다.

## 회귀 검증과 재실행

- Release AOT 게시, 현재 TypeScript compile/published JS hash 일치: PASS.
- input admission/rendering/lifecycle/startup/managed connection: 45/45 PASS.
- CPU frozen submission/resource ownership와 native command cache: PASS.
  별도 frozen 제출에서 동일 그림의 recording/hit, 현재 offset 적용, 새 그림 교체를 검사한다.
- Playwright Firefox 155 및 Chrome의 두 renderer에서 두 slider의 144개 move, 최종 값,
  탭 왕복 상태, retained list의 dark theme, resize를 검사한다. Firefox DPR은
  `layout.css.devPixelsPerPx`로 설정하고 실제 snapshot DPR과 요청 값을 비교한다.
  Firefox가 context의 `device_scale_factor`만으로 요청 DPR을 적용하지 않은 중간 실행과
  screenshot 좌표 오류 실행은 최종 DPR 2 검증에 포함하지 않는다.

| 최종 실행 | 결과 |
| --- | --- |
| 설치된 Firefox 157, 실제 창·DPR 2, WebGL/WebGPU | 각 PASS |
| 설치된 Firefox 157, 1680×950·DPR 2, WebGL | PASS |
| Playwright Firefox 155 headless·DPR 2, 두 slider | PASS |
| Chrome 154.0.8037.93 headless·DPR 2, WebGL/WebGPU 두 slider | 각 PASS |

Playwright 세 실행은 모두 page/console error 0, 각 slider move 144개 보존, 탭 왕복 값
보존, dark theme과 resize 검증을 통과했다.

최종 Firefox DPR 2 slider 실행의 Ganesh budgeted cache는 블러 드래그 뒤
268,125,252 bytes에서 5.5초 유휴 뒤 49,902,816 bytes로 줄었다. 기존 5초 deferred
purge가 동작했고 texture/text/picture cache 한도 검사도 통과했다. 남아 있는 retained
그림은 재사용하기 위한 cache다. 짧은 Skia accounting 관찰을 전체 VRAM 회수나 장기
누수 부재로 해석하지 않는다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet publish samples/DorotiSampleApp2/web/DorotiSampleApp2.Web.csproj -c Release --artifacts-path ./temp/testing/firefox-sliders/fixed-aot
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/tests/web_firefox_rendering.py --root temp/testing/firefox-sliders/fixed-aot/publish/DorotiSampleApp2.Web/release/wwwroot --out temp/testing/firefox-rendering/new-webgl --renderer webgl --headed
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/tests/web_firefox_rendering.py --root temp/testing/firefox-sliders/fixed-aot/publish/DorotiSampleApp2.Web/release/wwwroot --out temp/testing/firefox-rendering/new-webgpu --renderer webgpu --headed
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/tests/web_sample2_sliders.py --root temp/testing/firefox-sliders/fixed-aot/publish/DorotiSampleApp2.Web/release/wwwroot --out temp/testing/firefox-rendering/new-sliders --browser firefox --dpr 2 --runs 1
```

설치된 Firefox runner는 Selenium/geckodriver, slider runner는 Playwright/Pillow와
Playwright Firefox 또는 설치된 Chrome이 필요하다. 별도 profile/loopback isolated server를
사용하고 종료 시 닫는다. 모든 검증은 1,200초 process-tree timeout을 적용한다.
최종 원시 실행은 `temp/testing/firefox-rendering/cache-final-{webgl,webgpu,wide-webgl}/`와
`cache-sliders-{firefox-dpr2,chrome-webgl,chrome-webgpu}/`, 빌드는 `cache-aot.log`다.
원시 자료는 삭제 가능한 산출물이다. 최초 receipt의 `fixed-aot` 경로는 후속 게시로
갱신됐으므로 실제 게시물 식별에는 각 receipt의 hash를 사용한다.

물리 precision trackpad, 실제 표시 FPS/scanout, 사용자 Firefox profile, GPU process 전체
메모리·장기 안정성·다른 플랫폼은 notVerified다. 이번 PASS는 자동 입력·캐시·렌더러 회귀와
측정된 dispatch/submit 개선의 범위다.
