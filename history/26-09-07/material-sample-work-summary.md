# Material 샘플·공용 기능 수리·렌더링 개선 작업 요약

- 정리일: **2026-09-07**
- 대상: 저장소 루트 `work.md`의 §1–§19. 사용자 요청으로 요약·원문 보관 후 루트 문서를 삭제한다.
- 종합 상태: **구현 및 범위별 자동 검증 진행 / 전체 PARTIAL**. 문서 정리는 미완료 작업의 완료 선언이 아니다.
- 최초 분석 기준은 `d8efedd`, 최근 검토 기준은 `207010a48eb7040bed4475ace53c5363d819de6f`와 미커밋 변경이다.
  이 요약을 만들면서 제품 소스 수정·빌드·런타임 재검증은 하지 않았다. 아래 결과는 원문에 기록된 실행 이력이다.

## 보관 문서와 후속 계획

- [삭제 직전 원문 전체](material-sample-work-original.txt): 932줄, 바이트 단위 복사본.
  SHA-256: `20D07BF00276013596629519073EAFEBFF975E493284E3645E81A671EF5B086F`.
  원문의 상대 경로는 **저장소 루트 기준**이다. 초기 분석의 미구현 표현과 당시 다음 작업도 시점별 기록으로 보존했다.
- [Web 캐시·스크롤·애니메이션 상세 조사](web-sample-retained-rendering.md): 원문 §16–§19의 원본 측정·실패·한계.
- [SkiaSharp direct 전환·live resize·시작 지연 후속 계획](../../work2.md): 아직 계획이며 구현 미시작.
- [별도 cross-platform 부트 계획 요약·원문 보관](cross-platform-boot-plan-summary.md) 및 [부트 결과](cross-platform-boot-results.md): 샘플 이식과 별도 범위.

사용자는 이후 direct가 훨씬 부드럽다고 확인하고 Web 기본값 전환과 두 성능 문제 수리를 요청했다.
현재 기본값 변경은 `work2.md`의 후속 범위다. 원문 마지막의 “기본값 유지”는 비교 당시 결과이며,
새 전환 방향을 취소하는 정책으로 해석하지 않는다. **sample을 기본 화면으로 바꾸는 것과 Web 기본 렌더러 변경은 별개다.**

## 1. 원래 목표와 구현 범위 — 원문 §1–§6

Flutter Material 샘플을 사용 사례로 삼아 Doroti 공용 Framework/Runtime/Ui/Rendering/Host의
부족한 기능을 수리하고, C# 전용 `DorotiTestbedApp`으로 이식하는 작업이었다.
실제 reference는 `reference/flutter_sample_app`이며 요청에 등장한 `reference/flutter/_sample/_app`는 당시 checkout에 없었다.

- Material 3 전용 Components/Color/Typography/Elevation 네 화면, 여섯 Components 그룹,
  9 seed/6 image theme, local/URL Image demo를 구성했다.
- Home은 논리 폭 ≤1000 bottom bar/한 목록, 1000 초과~1500 rail/두 목록, 1500 초과 extended rail이다.
  낮은 높이 설정, Color 500px·Elevation 450px content 경계, Bar/Rail/OneTwo 전환도 범위에 포함했다.
- 기존 diagnostics/G6/F0/F1/F2는 보존했다. `DOROTI_TESTBED_MODE`와 Web query로 sample을 명시한다.
  문서 보관 시 기본 화면은 diagnostics이며 fixture가 sample보다 우선한다.
- 샘플은 opaque Material surface, diagnostics는 기존 Acrylic/진단 구성을 사용한다.
  폰트/이미지/각색 source의 hash·라이선스·출처와 한·영 실행 문서를 추가했다.
- 공용 결함을 sample 전용 우회나 고정 palette로 감추지 않고, 작은 독립 계약과 실제 sample 통합 검증을 연결했다.

| 단계 | 보관 시 판단 |
| --- | --- |
| P0 기준·최소 재현 | 입력 snapshot, inventory, 대표 baseline 및 blocker 재현 확보 |
| P1 모드·골격 | diagnostics 분리, explicit sample, 네 destination 및 상태 소유 구현 |
| P2 정적 화면·테마 | 구현·자동 경계 검사 확보, 전체 role/spacing/style 시각 대조 잔여 |
| P3 Components | 구현 및 다수 Web 입력 PASS, 전체 focus 복귀/dispose/snapping/스크롤 계약 잔여 |
| P4 이미지·URL | 공용 구현·Web 기능과 MCU differential PASS, native 실제 URL activation 등 잔여 |
| P5 motion·시각·입력 | geometry/일부 pointer·clipboard PASS, 전체 pixel/physical/IME/accessibility 미완료 |
| P6 플랫폼 | Windows/Web 일부 실행, Android package/Linux managed build. 타 OS live는 미검증 |
| P7 sample 기본 화면 | gate 미완료로 미전환. 문서·notice 정리는 수행 |

후속 구현으로 진척된 항목과 초기 체크리스트를 구분했다. 미체크 항목을 임의로 전체 PASS 처리하지 않는다.

## 2. 공용 이미지·샘플 기능 수리 — 원문 §7–§9

| 결함 | 수리 내용 |
| --- | --- |
| D01–D03 | Picture 실제 rasterization, Image RGBA/PNG readback·수명 계약, Celebi quantization/Score 및 MCU role 비교 |
| D04/D18 | SearchAnchor optional/null 기본값, generic 독립 route 조회, `didPop` override, suggestions/history·선택·재열기·Esc |
| D05 | typed URL launcher capability와 Windows/Web/MAUI/Qt adapter, Web popup/차단 feedback |
| D06–D07 | paintImage 기본 alignment, ImageProvider codec 완료 연결과 normal/ephemeral error 전파 |
| D08–D10 | Drawer GlobalKey identity, Material 3 기본 theme virtual override/nullable state property, optional ActionButton key |
| D11–D12/D21 | public font loading, 등록 완료·fontsChange, UI/Raster 동일 font collection, 잘못된 font alias 및 paragraph intrinsic width |
| D13–D17 | ImageProvider generic 보존, Carousel resolver/layout, semantics ancestor transform, Switch nullable shadow, icon/disabled callback |
| D19–D20 | picker calendar/date normalization/restoration, InheritedModel 조회, OverlayPortal builder, MenuAnchor/group/dismiss/zero-duration animation |
| D22–D24 | semantics DPR 이중 적용, Dropdown decoration theme wrapper, CanvasKit의 실제 Skia shadow 연결 |
| D25–D26 | section height cache 수명과 scroll clamp, radio checked/selected 및 explicit role |

핵심 자동 증거:

- `.doroti/evidence/material-sample-p0/`, `material-image-repair/`, `material-sample-implementation/` 및
  `Doroti/validation/web-playwright/artifacts/`의 각 run. 정확한 파일명과 실패 순서는 원문에 보존했다.
- 이미지 독립 contracts **8/8**, native WebP/URL/합성 입력 MCU differential **6/6**,
  실제 Web MemoryImage/NetworkImageIo readback·수명·손상 이미지, FCR-7 및 Windows/Web Release PASS.
- 같은 host가 읽은 ARGB를 pinned Dart MCU에 넣어 palette/seed/46 light+46 dark roles를 정확 비교했다.
  WebP seed native `0xFFF38301`, Web `0xFFF78C02`; URL은 둘 다 `0xFF769296`.
  **각 readback의 MCU parity PASS이며 cross-host decode pixel/seed 동일성은 아니다.**
- `material-sample-final-v27`: **13 PASS**, CanvasKit hardware Chromium DPR1/2, 6.2분.
  네 destination, lazy inventory, theme/error/retry/latest selection, sheet/dialog/Search,
  picker/menu/Dropdown/selection/clipboard, 폭·높이 경계를 포함한다.
- 기존 diagnostics 입력/resize/DisplayList **6 PASS**, 공용 FCR-6 semantics/FCR-7 PASS.
  자동 resize reversal geometry와 headed TextField spaces/caret도 통과했으나 physical smoothness 증거는 아니다.
- Flutter analyze/widget tests **15 PASS**, Web build 및 캡처 확보. Flutter 테스트는 Doroti 검증을 대신하지 않는다.

시각 비교는 `visual-comparison-v2.json`, 800×900/DPR1, body `(0,56)–(800,820)`, 동일 seed/font 조건이다.
branding/navigation을 제외한 crop이며 **MEASURED_NOT_ACCEPTANCE**다.

| 화면 | light MAE / 차이 >2 pixel 비율 | dark MAE / 차이 >2 pixel 비율 |
| --- | --- | --- |
| Components | 1.277 / 2.43% | 1.180 / 2.08% |
| Color | 1.853 / 3.06% | 1.107 / 2.37% |
| Typography | 0.248 / 0.38% | 0.238 / 0.38% |
| Elevation | 0.362 / 0.99% | 0.372 / 0.97% |

## 3. Windows 사용자 보고 후속 — 원문 §10–§15

| 항목 | 변경·검증 | 남은 경계 |
| --- | --- | --- |
| D27 navigation | loop closure index를 고쳐 네 callback 0/1/2/3 전달 | 전체 OS 입력 조합 아님 |
| D28/D34 native text | 실제 ascent/descent·line offset·baseline·advance, weight/style/자간·최신 등록 폰트 선택 | 전체 font/픽셀 parity 아님 |
| D29/D30 picker | 정수 나눗셈/modulo/현재 날짜, nullable Cancel, driven animation 잘못된 cast 수정. mounted pointer/raster PASS | 실제 모든 picker/IME 입력 아님 |
| D31/D33 이미지 | lazy decode 제거, 명시적 Extract colors/Retry image, 축소 picture 캐시·translation 독립 extent | 최종 연속 scroll cadence·체감은 notVerified |
| D32 Windows resize crash | 100ms prepared-frame timeout을 fatal render failure와 분리, 취소 경합 복구 | 전 방향 부드러움·다른 GPU/DPI 미검증 |
| D35 drawer | 과도한 가로 stretch를 고쳐 304px Drawer/280px destination의 indicator·InkWell·아이콘 정렬 | 검증한 실제 입력 범위에 한정 |
| D36 app bar 출처 | 외곽 두 ScrollPosition만 root scrolledUnder에 허용, 합성 내부 알림 거부 PASS | 원래 flicker 원인 재현 증거가 아님 |
| 공용 GPU cache app bar 누락 | frame 번호 LRU 동률로 새 image가 draw 전에 dispose되는 경로 재현. 매 access 순번, draw 후 trim으로 수리 | 모든 physical frame flicker-free 선언 아님 |

증거 root는 `.doroti/evidence/` 아래 다음 폴더다:

- `windows-sample-repair-20260907/`: native-v11, blockers/regression, Release PASS.
- `windows-sample-resize-20260907/`: 원본 Windows event/NativeFailure와 수정 후 OS resize.
  Left/Right sizing 33회, 실제 timeout recover 1회, accepted 7/presented 6/superseded 1,
  unterminated/duplicate/operational errors 0. prepare/cancel/commit·clock failure injection PASS.
- `windows-sample-scroll-20260907/`: 반복 quantize/decode를 관측하지 못한 profile,
  명시적 추출·font·cache extent·scroll state 회귀. 수정 후 실제 스크롤 성능은 notVerified.
- 드로어 관련 정확한 root와 run은 원문 §13, app bar 출처는 `windows-appbar-compare-20260907/`.
- `windows-appbar-raster-20260907/`: 동일 scene CPU/GPU 비교로 두 열 중간 프레임 누락 재현.
  `final-v2`는 70 위치/420 중간 프레임 PASS, cache hit 23,074/admission 456.
  외곽 전체 pixel 검사는 유지하고 내부는 text edge AA 차이를 제외한 3×3 동일색 영역을 검사했다.

Computer Use 연결 실패·중단 당시의 미확인 결과와 이후 실제 창 확인을 구분했다.
최종 실제 창에서 app bar 표시를 확인한 범위는 있으나 사용자 재확인과 전체 physical acceptance는 별도다.

## 4. Web 캐시·새 구간·애니메이션 개선 — 원문 §16–§19

상세 source·counter·artifact는 [Web 전용 이력](web-sample-retained-rendering.md)을 따른다.

- immutable draw-time path/paragraph/transform snapshot과 bounded mapping/encoding cache,
  clip antialias 전달, wheel 종료 후 100ms semantics quiet interval을 추가했다.
- deferred paragraph 측정·최근 두 폭 cache·숫자 metrics 전달, picture body cache,
  문자열 변경 시 text만 무효화, byte bulk copy와 단일 async WebCrypto SHA-256을 적용했다.
- sample을 29 lazy sections로 나누고 방문 상태를 유지했다. 미배치 keep-alive child를 focus/OverlayPortal이
  참조하던 공용 Widgets 결함도 수정했다.
- animation에는 참조 조회·16 command block cache·실제 문자열 내용 기반 table 재사용·table CRC32를 추가했다.
  cache 상한과 invalidation/golden 검증을 유지했다.
- direct bootstrap의 sample 환경변수 누락을 시작/재시작 모두 수정하고 과도한 진단 DOM 갱신을 묶었다.

| workload | 초기 → 최종 기록 | 한계 |
| --- | --- | --- |
| 짧은 왕복 scroll UI | median 69.1 → 23.8 → 16.8ms, p95 115.8 → 61.9 → 53.3ms | 같은 짧은 workload. 전체 sample FPS 아님 |
| 신규 section 전체 sweep | 최종 max **723.8ms**, managed layout/compositing 668.5ms, raster 11ms | 미해결. 중간 454.7ms로 대체하지 않음 |
| warm progress trace-on | UI median 20.1 → 12.4ms, p95 23.0 → 15.6ms, encode 9.4 → 3.0ms | 최종 UI max 35ms/submit gap max 40.7ms 남음 |
| warm progress trace-off | 6.016초 360 submits, UI mean 10.63ms/raster 2.23ms | 실제 표시 FPS·onset 검증 아님 |
| CanvasKit/direct 단일 비교 | 약 6초 359/360 submits, gap p95 19.9/18.5ms, max 33.9/23.9ms | 반복 우위·Flutter parity·새 section direct 측정 아님 |

Release·scheduler/mapper/metrics/encoder/FCR-7·TypeScript PASS.
Web 11 PASS/2 FAIL→해당 2 PASS, 다음 13 PASS/3 FAIL→4 PASS,
animation 12 PASS→최종 문자열 수정 후 4 PASS를 각 실행 단위로 보존한다.
direct progress pixel-change/stop PASS, restart 초기 viewport selector FAIL→수정 후 PASS.
이 수치를 하나의 동시 실행 전체 suite 결과처럼 합산하지 않는다.

## 5. 실패 이력 보존 규칙

원문과 기존 artifacts를 삭제하거나 최종 성공으로 덮어쓰지 않았다. 대표적인 구분은 다음과 같다.

- P0 hidden Windows launch의 visible gate FAIL, Web test 인자/No tests found, image blocker 원본 FAIL.
- 2.5초 Windows smoke FAIL과 이후 8초 PASS는 서로 다른 startup 예산이다.
- source compile/fixture host·locale·using 누락, 앱 DLL/apphost 잠금, wrapper LASTEXITCODE 오류와 제품 결함을 구분한다.
- Drawer/theme/font metrics/Carousel/Switch/Search/picker/overlay/radio/keep-alive 등의 실행 예외는 당시 제품 FAIL이다.
  창·버튼이 보였다는 이유로 PASS하지 않는다.
- continuous animation queue-idle, lazy/offscreen locator, clipboard client handoff, resize 중 두 settings,
  메뉴 hover 후 재클릭, stale dark capture 등의 harness 실패는 원본 FAIL을 남겼다.
- app bar D36 negative test는 의도한 FAIL이다. 이후 공용 GPU eviction 재현과 다른 증거다.
  GPU 내부 Tabs text edge 517px 차이로 실패한 실행도 보존하고, 변경한 검사 범위를 명시했다.
- Web unsigned JS import build FAIL, 문자열 identity cache miss 중간 결과,
  direct 잘못된 diagnostics 화면 측정·screenshot timeout도 이력에 유지한다.

각 실패의 정확한 run/명령/경로는 원문 §7–§19 및 Web 전용 이력에 있다.
보관 시 기존 로그·PNG·trace·profile·golden·snapshot을 이동하거나 정리하지 않았다.

## 6. 남은 작업과 최종 상태

1. **Web direct 기본값·resize·onset:** 사용자 확인을 반영한 [work2.md](../../work2.md)에서 진행한다.
   warm 측정은 첫 버튼 클릭 후 3초를 제외했으므로 시작 지연 해결 근거가 아니다.
2. **P3/P5:** 모든 overlay 취소/확정 focus 복귀·controller dispose, carousel snapping,
   모든 폭/theme에서 height cache·scroll ownership, animation 중 방향 반전의 연속 geometry 확인.
3. **시각/입력:** 전체 화면별 허용차, native shadow parity, 실제 한글 IME·screen reader·physical scroll/resize.
   CanvasKit shadow 수리를 native SkiaSharp 동등성으로 확대하지 않는다.
4. **이미지/URL:** 강제 Worker restart 중 read, animated playback·wide-gamut,
   native browser activation 및 target별 readback/input/accessibility 실행 검증.
5. **플랫폼:** Android arm64 Release package PASS이나 연결 기기 없음. Linux managed cross-build PASS이나 Qt/native live 미확인.
   AppKit/macOS·Mac Catalyst·iOS와 각 물리 검증은 notVerified. Windows/Web build·smoke·자동 PASS로 대체하지 않는다.
6. **sample 기본 화면 P7:** 기존 Windows/Web P1–P5 gate 잔여를 보존한다. 문서 삭제로 sample 기본 전환을 수행하지 않는다.

모든 기존 테스트/빌드 wrapper는 20분 timeout 규칙을 따랐다.
이번 정리의 검증은 원문 SHA-256 일치, 요약·참조 확인 및 루트 `work.md` 삭제 확인에 한정한다.
