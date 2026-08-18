# Doroti Flutter Conformance & Smooth Rendering Upgrade — 작업 기록

> 상태: **활성 계획 이관(부분 완료)** — FCR-0~FCR-8 compact validator와 representative 계약은 구현됨. Web interaction·Windows live는 `PASS`, Flutter differential·Android physical·soak는 `notVerified`
> 작성일: 2026-08-18
> 정리일: 2026-08-18
> 기준 Doroti revision: `19c6be0` + 이 작업 변경
> Flutter source pin: `56b8e1a851a594b1a154f8ea93270807dab22b9a`
> 1차 제품 대상: `DorotiDemoApp`, Android MAUI physical, Windows MAUI live
> 이전 기록: [`goal7-summary.md`](../26-08-16/goal7-summary.md)

## 1. 문서 성격

이 문서는 삭제한 루트 `work.md`의 Flutter 호환·지속 렌더링 계획을 압축해 보존하는 역사 기록이다. 새로운 active roadmap이나 실행 지시서가 아니다.

계획은 Android 스크롤 장애를 단발 성능 문제가 아니라 framework asset 누락, Dart→C# 의미 오역, frame ownership, retained rendering, scroll, semantics 비용이 겹친 구조 결함으로 보고, Flutter source를 실행 가능한 명세로 쓰는 conformance gate를 만드는 것이었다.

원칙은 변하지 않는다. Flutter source가 framework 동작의 owner이며, Doroti의 차이는 host/backend 적응에 필요한 경우에만 명시한다. “컴파일된다”, “첫 프레임이 제출됐다”, “픽셀이 한 번 바뀌었다”는 Flutter 호환 완료로 취급하지 않는다.

## 2. 왜 이 계획이 필요했는가

최근 Android 스크롤 장애는 하나의 성능 문제가 아니었다.

- Flutter `InkSparkle` shader asset(`shaders/ink_sparkle.frag`)이 Doroti package에 없어 Ink가 보이지 않았다.
- C# 수치 연산 차이(`Tween<Vector2>` dynamic binary operation)가 애니메이션 프레임 예외를 만들었다.
- 그 예외와 native back-buffer 교체가 겹치면서 상단 콘텐츠·배경이 간헐적으로 사라졌다.
- 접근성 native tree를 매 프레임에 가깝게 갱신한 비용이 스크롤 끊김과 GC 압력을 키웠다.

개별 증상은 완화되었지만, 같은 종류의 누락·오역을 자동으로 막는 계약이 필요했다. 이후에는 동일 입력에서 상태, layout, paint, semantics, raster와 지속 프레임이 일치하는지를 확인한다.

## 3. 고정된 원칙

1. Pinned Flutter를 실행 가능한 명세로 사용한다. 각 port는 source path/revision, symbol, asset, fixture를 추적한다.
2. 공용 원인은 compiler/lowerer/runtime/framework/scene/host에서 고친다. DemoApp workaround나 생성된 `.g.cs` 수정으로 숨기지 않는다.
3. 누락은 조용히 성공시키지 않는다. 미지원은 capability와 원인을 진단하고 `notVerified`/`explicitUnsupported`로 남긴다.
4. 정확성을 먼저 고정하고 최적화한다. Flutter와 다른 결과를 더 빠르게 만드는 변경은 성능 개선으로 인정하지 않는다.
5. 프레임은 최신 하나를 안전하게 전달한다. UI mutation, scene snapshot, raster/present ownership을 분리한다.
6. contract, differential, native live, physical, cross-target 결과를 섞지 않는다.
7. 대표 gate를 작게 유지하고 제품 시나리오·capability별 validator로 통폐합한다.

완료로 인정하지 않는 변경: Ink를 NoSplash로 바꾸기, Scrollbar/scrolling 제거, Scaffold를 항상 불투명하게 만들기, 무제한 full replay/cache, semantics 전부 15 fps로 지연, generated source 직접 수정, submitted/presented count만으로 parity 주장.

## 4. Milestone 상태

실행 순서는 FCR-0 기준선 이후 FCR-1(asset)과 FCR-2(의미)를 병행하고, FCR-3 frame ownership을 고정한 뒤 FCR-4~FCR-8로 진행한다. Android 결과를 Windows/Web/macOS 결과로 대체하지 않는다.

| Milestone | 구현한 계약 | 아직 `notVerified` |
| --- | --- | --- |
| FCR-0 기준선·누락 인벤토리 | source/asset/consumer 연결, parity matrix, capability gate. baseline은 측정값과 미실행 경계를 함께 기록하며 성능 PASS로 승격하지 않음 | 성능 승격, stale matrix 전면 재생성 이후 전 항목 폐쇄 |
| FCR-1 framework asset/runtime-effect | 공용 `framework-shader-manifest.json`, `FrameworkShaderLoader`가 InkSparkle/StretchEffect를 embedded asset으로 비동기 로드·hash/ABI 검증. StretchEffect inline fork 제거. cache key는 source SHA-256 + backend + graphics-context generation | cold first GPU capture, 100회 반복·context loss plateau, Flutter raster differential, Android physical·Windows live presentation |
| FCR-2 Dart→C# 의미 | Flutter tween/geometry fixture, typed `LerpTweenValue`/`IDartTweenValue<T>`, Tween dynamic binder 제거, Future observe, Debug/Release assert 보존 | DemoApp 60초 interaction log, 선택 framework 재생성 source diff, animation raster differential, Android/Windows live |
| FCR-3 scheduler·frame ownership | `DorotiFrameClock`, transient→microtask→persistent→post-frame 순서, pending request 1개, immutable latest scene 교체, input/scene sequence trace | Flutter callback trace differential, Windows resize/context/foreground stress, Android lifecycle stress, 60초 deadlock·duplicate·use-after-dispose |
| FCR-4 retained rendering | clean Layer는 immutable retained node 제출, dirty boundary만 재record, engine-layer reuse 시 dispose 금지, MAUI retained replay, 새 back buffer는 app background로 clear. C2는 silent 축약 없이 `NotSupportedException` | Flutter C0/C1 payload·GPU differential, native lifecycle, Android soak, resource balance counter, C2 owner/target matrix |
| FCR-5 scroll/viewport | `animateTo` attached snapshot 완료 대기, PrimaryScrollController/RawScrollbar ownership, `DorotiScrollTrace` packet-to-present ABI | native packet-to-present 전체 연결, Flutter drag/hold/ballistic differential, lazy/cacheExtent 계측, Android 60초 physical, Windows wheel/drag live |
| FCR-6 semantics | 15 fps는 continuous geometry-only scroll의 host coalescing 정책. focus/action/text/selection/node 변경은 즉시 flush | UI-thread 시간 threshold 확정, TalkBack/UIA physical checklist |
| FCR-7 Material/widget parity | fixture manifest가 source hash, 720×640/DPR 1, theme/locale/time-seed, 배경 owner, replay 좌표를 고정. 구조 gate만 닫힘 | paired Flutter/Doroti raster·state·semantics capture, pixel diff, Windows live·Android physical. 어느 component도 visual `PASS`로 승격하지 않음 |
| FCR-8 통합 stability | `Inventory`/`Contracts`/`Differential`/`WindowsLive`/`AndroidPhysical`/`Soak`/`Evidence` shard와 Developer representative 연결 | paired Flutter differential, Android physical, soak/resource plateau, same-run DPI transition. 상위 evidence는 `partial` |

## 5. 종료 시점에 보존하는 실행 결과

- Web browser의 checkbox/radio/switch/slider/button/FAB, wheel, keyboard semantics action은 `PASS`.
- Windows native wheel/drag, 두 번의 resize, swap-chain replay, shader 실행과 failed/software-fallback frame 0은 `PASS`.
- 연결된 Android 장치가 없어 Android physical은 `notVerified`.
- Flutter reference differential과 soak/resource plateau는 `notVerified`.
- FCR-8 상위 evidence는 정직하게 `partial`.

대표 산출물:

- `Doroti/validation/evidence/flutter-conformance/baseline-evidence.json`
- `Doroti/validation/evidence/flutter-conformance/framework-shader-manifest.json`
- `Doroti/validation/evidence/flutter-conformance/fcr7-material-widget-evidence.json`
- `Doroti/validation/evidence/flutter-conformance/fcr8-stability-evidence.json`
- `Doroti/eng/validate.ps1` capability gate와 `validate-fcr*.ps1`
- Developer representative: `pwsh -NoProfile -File .\Doroti\eng\doroti.ps1 validate -ValidationSuite Developer`

## 6. 확인된 구조적 위험

- 다음 shader/font/data asset도 source만 번역되고 package에서 누락될 수 있다.
- generic arithmetic, `double`/`float`, nullable, Future, assert debug-only side effect가 다른 파일에서 다시 어긋날 수 있다.
- 마지막 scene replay는 화면 소실을 막는 최소 안전망이며 Flutter식 retained layer 재사용의 증거가 아니다.
- app-level scroll 보정이 PrimaryScrollController/Scrollbar ownership 결함을 가릴 수 있다.
- semantics를 모두 동일하게 지연하면 focus/action/selection이 늦고, 전체 node 정렬 비용도 남는다.
- 오래된 scene-operation matrix와 현재 host 구현이 어긋날 수 있으며 `presented`만으로 effect 의미를 증명할 수 없다.

## 7. 남은 작업을 재개할 때의 순서

```text
FCR-0 기준선 유지
  ├─> FCR-1 GPU/physical presentation 폐쇄
  └─> FCR-2 DemoApp interaction·raster differential
          -> FCR-3 lifecycle stress
              -> FCR-4 C0/C1 differential·resource soak
                  -> FCR-5 Android 60초 physical / Windows live scroll
                      -> FCR-6 physical semantics checklist
                          -> FCR-7 paired visual parity
                              -> FCR-8 Android physical·soak·release evidence
```

재개 시 문서의 완료 gate를 이미 `PASS`인 것으로 간주하지 않는다. Web/Windows representative `PASS`를 Android physical이나 Flutter differential로 승격하지 않는다. 하위 공용 의미 결함은 generated product나 Demo workaround가 아니라 compiler/runtime/scene contract에서 수정한다.

## 8. 종료 판단

이 계획은 FCR-0~FCR-8의 compact validator와 representative 계약을 닫았고, Web interaction과 Windows live의 대표 경로를 `PASS`로 남겼다. 그러나 Flutter raster/state differential, Android physical, soak/resource plateau가 남아 있으므로 **Flutter conformance 전체 완료로 기록하지 않는다**.

종료 기준은 “Doroti가 Flutter의 모든 API를 구현했다”가 아니다. 선택한 제품 경로에서 Flutter source가 요구하는 동작을 누락 없이 보존하고, 다른 경로의 미지원·미검증 상태를 정확히 드러내며, 다음 Flutter pin 갱신 때 같은 감사를 반복할 수 있는 상태를 만드는 것이다.

> 문서 성격: `work.md` 삭제 시점의 Flutter conformance 역사 요약과 evidence 경계. 후속 active roadmap은 아직 지정하지 않았다.
