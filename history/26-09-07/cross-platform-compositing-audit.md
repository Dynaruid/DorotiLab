# 전체 플랫폼 렌더링·캐시·surface 수명 점검

2026-09-07. 사용자가 Windows/Web을 포함한 다른 플랫폼의 잠재적 문제까지 수정을 요청했다.
범위는 앞선 텍스트 문제에서 연결되는 공용 그림 합성, runtime shader 필터, GPU 캐시 소유권,
surface 교체 및 예외 복구다. 저장소 전체의 모든 기능에 결함이 없다는 검증은 아니다.

앞선 기록: [텍스트 호버](text-hover-raster-phase-fix.md), [다른 플랫폼 글꼴 설정](cross-platform-text-raster-audit.md).

## 수정

1. **배경에 의존하는 blend 그림의 잘못된 캐싱.** `clear`, `src`, `dstIn`, `multiply` 등의
   결과를 투명 surface에서 계산한 뒤 `srcOver`로 붙이면 원래 배경과의 합성이 달라진다.
   해당 그림은 실제 대상에 직접 replay한다. immutable command 목록의 판정은 weak cache로 재사용하고,
   일반 `srcOver` 그림의 GPU 캐시는 유지한다. 모든 `BlendMode`를 3프레임씩 직접 GPU 결과와 비교했다.
2. **Shader image-filter 캐시의 소수점 phase 손실.** 그림 캐시와 별도로 남아 있던 fractional 원점 제거를
   수정했다. input/output은 floor/ceil device bounds를 사용하고 phase·surface font policy를 signature에 넣었다.
   perspective 변환은 translation 재사용 대상에서 제외한다. 입력용 pooled surface도 font policy가 다르면 재생성한다.
3. **Filter 캐시의 같은 프레임 eviction 순서.** frame 번호가 같은 항목끼리 새 항목이 먼저 선택되어
   draw 전에 dispose될 수 있었다. 사용마다 증가하는 sequence로 LRU를 결정하고 draw 후 trim한다.
4. **필터 child 예외가 다음 프레임에 상태를 남김.** pooled input surface의 matrix/save stack을
   `finally`의 `RestoreToCount`로 복구한다. child가 save/translate 후 예외를 던져도 다음 프레임이 정상이다.
5. **같은 backend/view ID/generation의 별도 renderer 간 GPU 자원 충돌 가능성.** backend 문자열에는
   이미 view ID가 들어 있었지만, 독립 host/session의 동일 ID 및 renderer 재생성까지 구분하지는 못했다.
   renderer별 owner token을 filter pool 및 compiled-effect key에 추가했다. 한 renderer의
   invalidate/dispose가 다른 renderer의 자원을 지우지 않는다. native cache 해제는 paint lock 안에서 수행한다.
   기존 public `InvalidateContext(string,long)` API는 유지했다.
6. **큰 finite filter bounds의 정수 overflow.** viewport와 double 좌표에서 먼저 교차하고,
   캐시 크기 제한을 확인한 뒤 정수로 변환한다. 캐시를 만들 수 없는 큰 영역도 보이는 부분은 정상 렌더링한다.
7. **Web/Qt framebuffer descriptor 변경 누락.** 크기와 FBO가 같아도 sample count/stencil bits가 바뀌면
   native wrapper를 다시 만든다. Qt는 color format 변경도 비교한다. 이 항목은 코드 검토로 확인한 경계 보완이며
   실제 장치의 해당 descriptor 전환을 재현했다고 표현하지 않는다.

Windows, Android, Linux, macOS, Mac Catalyst, iOS 및 Web Skia 경로가 공용 수정을 사용한다.
Apple에서 기존 picture raster cache 비활성화는 유지되지만, 공용 shader image-filter 경로의 수정은 적용된다.
CanvasKit Web은 별도 구현이며 이번 SkiaSharp cache 결함을 그대로 갖고 있다고 단정하지 않는다.
현재 CanvasKit의 offscreen 원점 정렬 및 pooled surface 복구 코드를 함께 확인했다.

## 재현 실패와 수정 후 검증

모든 실행은 20분 timeout. 로그는 `.doroti/compositing-*`에 보존한다.

| 검사 | 수정 전 / 근거 | 최종 결과 |
| --- | --- | --- |
| blend | `blend=clear, frame=1`에서 GPU pixel mismatch | 모든 BlendMode, 3프레임씩 byte-exact PASS |
| shader filter phase | 첫 fractional frame부터 GPU pixel mismatch | 8개 fractional 이동 frame PASS |
| filter eviction | 정수 원점·40개 key로 cache pressure에서 pixel mismatch | 40개 key × 3프레임 PASS |
| child exception | 같은 크기의 pooled surface 재사용 시 다음 프레임 mismatch | 8프레임 PASS |
| renderer owners | backend/view ID/generation만으로 owner를 식별하던 코드 경로 | 별도 D3D12 컨텍스트 2개, 동일 cache key/서로 다른 색, 개별 invalidate/dispose 및 compiled shader 유지 PASS |
| huge bounds | ±1e20 finite bounds에서 `OverflowException` | 보이는 영역 8프레임 byte-exact PASS |
| 기존 텍스트 | 앞선 strict/coverage 기준 유지 | 기본 40프레임 byte-exact, 확장 180프레임 PASS |
| runtime-shader-contract | 기존 compiler/uniform/sampler/unsupported-backend 계약 | PASS |
| raster-budget | 기존 승격/LRU/metadata/context 회귀 | PASS |
| AppBarRaster | 기존 앱바·스크롤·cache pressure 회귀 | 420개 중간 프레임 PASS |
| 최종 Web direct | 수정본 Release 재빌드 + 실제 Chromium 호버 | DPR 1/1.25/1.5/2, 4/4 PASS |
| 최종 platform build | Windows Release, Android arm64 Release 패키징, Linux managed Release cross-build | 모두 warnings/errors 0 |

최종 합성 suite 실행:

```powershell
dotnet run --project Doroti/validation/fcr7-material-widget -c Release -- --gpu-compositing blend
# phase, eviction, exception, owners, huge도 각각 실행
dotnet run --project Doroti/validation/fcr7-material-widget -c Release -- --text-platforms
dotnet run --project Doroti/validation/runtime-shader-contract -c Release
```

주요 증거:

- 최초 FAIL: `compositing-blend-before2`, `compositing-phase-before`, `compositing-eviction-before2`,
  `compositing-exception-before2`, `compositing-huge-before`의 `.log/.err`.
- exception 최초 harness는 실패한 surface와 다음 surface의 크기가 달라 새 surface를 만들며 PASS했다.
  같은 크기를 재사용하도록 고친 `before2`를 실제 재현 근거로 사용한다.
- 최종: `compositing-final-{blend,phase,eviction,exception,owners,huge}` 및
  compiled-effect 보존 assertion을 포함한 `compositing-owners-isolation`.
- 텍스트/예산: `compositing-final-{text-platforms,text-hover,raster-budget}`.
- 기존 shader: `compositing-runtime-contract`.
- AppBarRaster: `.doroti/evidence/material-sample-AppBarRaster-20260907-221643-51ca5148/`.
- Web: `Doroti/validation/web-playwright/artifacts/wrapper/compositing-direct-final/`.
- 빌드: `.doroti/compositing-build-{windows,android,linux}.log`.

## 검증의 한계

공용 GPU 비교는 Windows offscreen D3D12에서 수행했다. 다른 OS의 빌드 및 같은 surface 조건 검사는
Android/OpenGL·Apple/Metal·Linux/Qt의 물리 화면, 입력, 접근성, suspend/resume 및 실제 context loss를 대신하지 않는다.
캐시 대상이 줄어드는 blend 그림의 성능 영향, 실제 여러 창의 동시 GPU 실행과 driver/device loss,
Apple native build/run 및 framebuffer descriptor 전환은 `notVerified`다. 기존 성능 gate를 승격하지 않는다.
