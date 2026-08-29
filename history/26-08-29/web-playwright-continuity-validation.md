# Web Playwright 연속성 구현과 검증

- 날짜: 2026-08-29
- 대상: `DorotiDemoApp` / browser-wasm / WebGL2
- 자동 브라우저: Playwright hardware Chromium, DPR 2 Chromium, headed Desktop Chrome

## 원인과 수정

기존 wheel event는 managed framework까지 같은 task에서 전달되고 있었지만, exact scene이 `requestPresent`를 호출한 뒤 presenter가 `queueMicrotask`에서 drain되었다. browser-wasm에서는 이 microtask가 현재 managed frame callback 전체가 반환된 다음 실행되어, scene 준비 이후에도 약 한 refresh interval을 추가로 기다렸다.

동일 Playwright 표본에서 수정 전 wheel→exact front p95는 61.9 ms였다. `CanvasPresenter`의 current+latest mailbox, one-in-flight, exactly-once terminal 규칙을 유지하면서 presenter를 동기 drain하도록 변경했다. 최종 실행의 60 sample p95는 21.1 ms, 최대 29.6 ms였다.

추가로 기존 `requestPresent` JS import의 `generation`과 `timestampMicroseconds` `long` 인자에 `JSMarshalAs<JSType.Number>`가 없어 Release source generator가 실패하던 계약을 명시했다. Desktop Chrome이 자동 요청한 `/favicon.ico` 404는 data favicon 선언으로 제거했다.

## Playwright 구성

- `Doroti/validation/web-playwright`에 TypeScript config, 공통 fixture, diagnostic helper와 startup/wheel/resize/flicker/input tests를 구성했다.
- hardware Chromium, `deviceScaleFactor: 2`, Desktop Chrome headed project를 직렬 실행한다.
- `Doroti/eng/run-web-playwright.ps1`이 Release build, 5088 포트 소유권, 서버 readiness, Playwright 실행, process-tree 정리를 담당한다.
- 모든 Playwright test와 wrapper process gate는 20분 timeout을 사용한다.
- 실패 시 trace, video, screenshot, HTML report를 ignored `artifacts/` 아래 남긴다.

## 자동 검증 결과

| 항목 | 결과 | 근거 |
| --- | --- | --- |
| Release build | `PASS` | 경고 0, 오류 0 |
| TypeScript check | `PASS` | `npm run check` |
| Playwright 전체 | `PASS` | 8 tests, hardware Chromium + DPR2 + headed Chrome |
| wheel ingress | `PASS` | 1,200 sample ingress/dispatch sequence 1:1 |
| wheel latency | `PASS` | 최종 60 samples, p95 21.1 ms, max 29.6 ms |
| presenter | `PASS` | queue depth ≤2, failed/stale/unpaired terminal 0 |
| resize | `PASS` | viewport A→B→C와 headed window bounds final exact front |
| DPR 2 | `PASS` | 1080×720 logical, 2160×1440 physical, exact generation |
| flicker oracle | `PASS` | idle 60초 + wheel 30초 + resize 30초, blank 후보 0 |
| context restore | `PASS` | loss/restore 후 context generation 증가, exact front 복구 |
| input/semantics smoke | `PASS` | semantic activation, canvas pointer, keyboard/native input |
| FCR-7 | `PASS` | Material/widget Release contract |
| resize contract v4 | `PASS` | 22/22 terminal, max queue depth 2, stale present 0 |

## 증거 경계

- Playwright `page.mouse.wheel()`과 synthetic `WheelEvent`는 실제 precision trackpad의 OS cadence와 device-kind 판정을 증명하지 않는다.
- `page.setViewportSize()`와 CDP `Browser.setWindowBounds`는 실제 사용자의 Windows border drag 및 compositor scan-out과 동일하지 않다.
- screenshot 기반 blank oracle은 browser 내부 raster 후보를 검출하지만 물리 모니터 scan-out acknowledgement가 아니다.
- 수정 후 실제 trackpad 30초, 실제 border drag 30초, maximize/restore, browser zoom, 실제 한글 IME 조합, 스크린리더와 물리적 깜빡임은 `notVerified`다.
- 별도 인앱 브라우저 연결은 사용 가능한 browser instance가 없어 `notVerified`다. Playwright Desktop Chrome headed 자동 검증은 이 항목과 별개로 `PASS`다.
