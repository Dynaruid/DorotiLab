# work3 통합 실행 기록

2026-09-08. 진행 중. 전체 완료나 S2 승격을 아직 선언하지 않는다.

## 범위와 기준선

사용자가 work3 전체 구현·검증을 요청했다. 과거 문서 작성 전용 범위는 이번
요청으로 확장됐다. 초기 HEAD `69a2cf7e793f440ab00224e16e45c0cee282d23b`,
tracked dirty 0. S1 source/asset manifest와 `.doroti/threads-fix/repaired-wwwroot`를
그대로 보존했으며 hash 재검사에서 불일치 0이다.

모든 실행은 `Doroti/eng/work3-run.py`의 외부 1,200초 deadline, 자동 retry 0을
사용한다. 각 실행의 exact command/HEAD/dirty/elapsed/최초 오류는
`.doroti/work3/evidence/<label>.json` 및 `.log`에 보존한다. native build와 browser는
순차 실행한다. HTTP 정적 제공과 소스 조회는 준비 작업이다.

## 첫 실행 묶음 예산

U 전환·host 회귀와 G0 진입 가능 여부를 확인하는 통합 묶음이다. 기본 10회에서
최대 20회로 확장한다. 이유는 Android의 실제 managed version 혼합 실패를
분리·수정하고 Windows/MAUI/Linux의 독립 host와 threaded Worker 수명을 검증해야
하기 때문이다. 후속 성능 3쌍·DPR 전체 corpus를 이 묶음에서 생략하고 PASS로
올리지 않는다. 독립 browser context 3개를 포함한 regression 명령은 3회로 센다.
준비 경로 오류와 수정 재실행도 포함한다. 첫 묶음은 18명령/20실행 단위로 종료했다.

| 명령 | 단계 | 결과 | 독립 실행 단위 |
| --- | --- | --- | --- |
| u0-01-inventory | U0 | setup 오류: asset 상대경로 해석 오류, 패키지 다운로드 성공; 원본 보존 | 1 |
| u0-02-inventory-path | U0 | PASS: 중앙 패키지 10개, S1 hash 불일치 0 | 1 |
| u1-03-web-publish | U1/U2 | PASS: threaded trimmed Release, 172.126초 | 1 |
| u2-04-bootstrap | U2 | PASS: shared heap, owner thread, first content 1,380 sampled colors, resize/wheel, errors 0 | 1 |
| u2-05-regression | U2 | PASS: 선택/theme/reparent, role disposal, invalid protocol; 3 cases | 3 |
| u3-06-windowsappsdk | U3 | PASS: Release build, warning/error 0, 42.122초 | 1 |
| u3-07-android | U3 | FAIL: CS1705, old MAUI obj reference versus new renderer assembly | 1 |
| u3-08-android-isolated | U3 | PASS: APK 29,424,213 bytes, warning/error 0, 140.65초 | 1 |
| u3-09-linux | U3 | PASS: managed Linux/Qt head build, warning/error 0, 39.94초 | 1 |
| u3-10-maui-windows | U3 | PASS: MAUI Windows build, warning/error 0, 58.68초 | 1 |
| u0-11-transitive-inventory | U0 | 19개 Skia NuGet native/TFM dependency manifests 확보; S1 assets 불일치 0 | 1 |
| u2-12-s1-profile | U2/T1 | PASS 실행: 29개 섹션·16 resize의 S1 상세 trace | 1 |
| u2-13-s2-profile | U2/T1 | PASS 실행; wide 비교 가능, breakpoint 작업 수 불일치 | 1 |
| u2-14-asset-audit | U1/U2 | PASS: 격리 restore 버전 혼합 0, mt/SIMD 및 interop/Dawn link 입력 각 1회 | 1 |
| u3-15-runner-pack | U3 | PASS: Runner SDK nupkg 생성 | 1 |
| u3-16-windows-content | U3 | PASS: Windows sample, VisibleAfterExactPresent=true, RendererPresented=2, GPU errors=0 | 1 |
| g0-17-publish | G0 | FAIL: obsolete DrawImage overload가 warnings-as-errors에 걸림 | 1 |
| g0-18-publish-sampling | G0 | PASS: 명시적 SKSamplingOptions overload 사용, trimmed publish | 1 |

산출물 복사/서버 시작/검증을 묶은 shell 명령은 자동 승인 검토가 실행 전에
차단했다(구체 사유 미제공). 복사와 서버 시작을 분리해 정상 수행했다. 이 거부는
browser 실행으로 기록하지 않는다.

## 확인된 수정

- 중앙 패키지 10개, Runner SDK 기본값, Web target manifest, MAUI 진단,
  RuntimeEffects release metadata를 `4.154.0-preview.1.26454.9`로 변경했다.
- normal restore로 Web/Windows/Android lock을 재평가한다. HarfBuzzSharp는
  SkiaSharp.HarfBuzz nuspec의 `14.2.1.400-preview.1.26454.9` 요구를 따른다.
- MAUI 및 플랫폼 실행 헤드의 `obj` override가 `--artifacts-path`에서도 적용돼
  restore/inner-build 사이에 이전 package reference가 섞였다. ArtifactsPath를
  명시한 경우 SDK의 격리 output 경로를 사용하도록 수정했다. 기본 RID별 경로는
  유지한다. 최초 CS1705를 삭제하거나 PASS로 변경하지 않는다.
- performance probe의 모든 pthread를 기다리는 조회를 실제 render role 선택으로
  바꿨다. idle pthread의 Atomics.wait가 측정을 멈추게 하지 않는다. 진단 수집은
  입력 측정 구간 밖에서 수행한다.

## 근거와 한계

Web 검사는 Chromium hardware 자동 입력 및 screenshot 검토다. 물리 입력,
IME/접근성, scan-out, 다른 OS/browser/GPU 수용은 아직 notVerified다.
Windows build는 first-content 실행을 대신하지 않는다.

Skia의 [고정 태그 targets](https://github.com/mono/SkiaSharp/blob/v4.154.0-preview.1.26454.9/binding/SkiaSharp.NativeAssets.WebAssembly/buildTransitive/SkiaSharp.targets)와
[Graphite Dawn test source](https://github.com/mono/SkiaSharp/blob/v4.154.0-preview.1.26454.9/tests/Tests/SkiaSharp/Visual/Renderers/GraphiteDawnRenderer.cs)를
확인했다. upstream test의 process-lifetime context와 CPU readback을 제품의
종료/visible present 구현으로 간주하지 않는다.

## 두 번째 통합 묶음의 범위

첫 묶음의 잔여 gate를 새 대상으로 명시한다. G0 browser/필요한 최소 원인 분리
최대 3회, T1 layout 자체 self-time 계측 build/profile 2회, Apple의 restore 가능
범위 판정 최대 3회, 최종 source/asset/문서 검사 1회를 합쳐 기본 9회다. G0 통과
후의 G1–G4 전체 수용은 별도의 구현·검증이 필요하며 이 예산 안에서 자동 PASS로
처리하지 않는다. 단계마다 20회씩 예산을 새로 부여하지 않는다.

## S1 / S2 기능·비용 판정

Web 기능 gate와 사용 가능한 Windows/Android/Linux managed build gate는 통과했다.
S2 WebGL 기준선은 `.doroti/work3/u2/s2/wwwroot`, 전체 asset hash는
`.doroti/work3/audit.json`에 고정했다. threads=true/main runtime/owner thread 8,
renderer 기본값 WebGL을 유지한다. native WASM은 3.1.56/mt,simd archive를 사용하고
DorotiSkiaInterop 및 Dawn JS/C++/필수 export는 각각 한 번 연결된다.

| 동일 입력 1쌍 | S1 | S2 | 판정 |
| --- | --- | --- | --- |
| 같은 열 LayoutWork | 1,450 | 1,450 | 작업 수 일치 |
| 같은 열 callback 총합 / 최대 | 905.365 / 207.855ms | 857.285 / 179.910ms | 관측 회귀 없음; 3쌍 성능 입증 아님 |
| 열 전환 LayoutWork | 1,585 | 1,269 | notComparable |
| 열 전환 callback 총합 / 최대 | 1067.865 / 271.915ms | 999.785 / 243.030ms | 작업 수가 달라 개선율 계산 금지 |
| 전체 비압축 Web asset | 41,692,247 bytes | 41,699,250 bytes | +7,003 bytes |

같은 열 allocation은 24,802,168→25,396,008 bytes(+2.4%). 해당 구간의 GC 횟수는
S1 [3,1,1], S2 [4,0,0]으로 달랐다. GC.GetTotalMemory snapshot의 44.35→63.13MB를
동일 collection 경계의 live-memory 증가율로 해석하지 않는다. 정확한 retained
live/GPU/process 메모리는 notMeasured다. 열 전환 입력의 coalescing과 anchor별
작업 차이는 유지하고, 성능 개선·전체 플랫폼 물리 수용은 아직 미확정이다.

19개 NuGet의 해시/nuspec/native 파일 목록은 `u0/inventory-complete.json`이다.
두 번째 inventory에서 csproj source hash가 달라진 것은 G0 validation 조건을
추가했기 때문이며 원본 S1 asset은 변하지 않았다. 원본 시작 hash 검사는
`u0/inventory-verified.json`에 남아 있다.

## T1 / T7 판정: 계산 병렬화 미채택

`t1-21-profile-publish`와 `t1-22-layout-cost`는 S2의 직렬 layout에 기본 비활성
계측만 추가했다. `dorotiLayoutProfile=1`에서 RenderObject.layout(kind 7)과
RenderFlex._computeSizes(kind 8)의 중첩 self-time을 분리한다. 계측 dropped=0,
layoutEnabled=true이고, 동일한 29개 섹션과 16개 resize 입력을 사용했다.

| 후보 | 같은 열 resize | 열 전환 resize |
| --- | --- | --- |
| RenderFlex._computeSizes self 합계 / 호출 수 | 2.469ms / 125 | 1.586ms / 105 |
| 전체 callback 합계 | 830.140ms | 938.375ms |
| 후보 self / callback | 0.297% | 0.169% |
| 후보 평균 호출 | 19.75µs | 15.10µs |

이 self-time도 순수 숫자 계산만의 시간이 아니다. 자식 순회와 baseline 조회가
포함되므로 병렬화 가능한 비용의 상한에 가깝다. 더 큰 RenderLayoutBuilder self
4.923/6.281ms와 열 전환 RenderSectionList 5.624ms는 owner의 build/lifecycle
작업이며 immutable 숫자 커널로 독립 실행할 근거가 없다. 현재 workload에서
snapshot/dispatch/join 비용을 감수할 가치 있는 후보를 찾지 못했다.

따라서 T는 **미채택으로 종료**한다. T0b 및 T2–T6의 executor/snapshot/병렬 A/B는
선행 조건 미충족으로 notApplicable이며, 병렬화 속도 향상이나 physical 수용을
주장하지 않는다. T7은 이 결론과 계측을 남기는 것으로 처리한다. B가 없으므로
J0도 notApplicable이다. Flutter object를 공유 스레드에서 실행하지 않는다.

## G0 결과와 G 제품 통합

`g0-19-worker`는 브라우저 device 생성에서 dxil.dll 로딩 오류로 실패했다.
Playwright 기본 headless_shell에는 해당 DLL이 없고 동일 버전의 full Chromium에는
존재함을 확인했다. `g0-20-full-chromium`은 channel=chromium으로 통과했다.
이는 [Playwright가 구분하는 두 headless 배포](https://playwright.dev/docs/browsers)의
환경 차이이며, renderer/runtime fallback을 추가하지 않았다.

G0는 main-owned .NET 10.0.11 shared heap의 render owner thread 8에서 native 154.0,
managed 4.154.0.0 Graphite/Dawn을 생성했다. 320×180→480×240의 실제 transferred
canvas에 도형·텍스트·raster image를 그리고 두 PNG를 검토했다. queue 완료 후
recorder/context/native handles 정상 정리, runtime/console 오류 0이다. 이것은
별도 smoke canvas의 증거이고 제품 전체 기능 PASS가 아니다.

G0 통과에 따라 두 번째 통합 묶음을 최대 20실행 단위로 확장한다. G1/G2의 실제
visible canvas, 공통 picture cache/effects/image provider를 연결하고 제품 first
content/수명 gate를 확인하기 위해서다. 실패 및 타입 수정 재빌드도 모두 센다.
성능 3쌍과 전체 DPR corpus는 제품 gate 통과 후 다음 묶음 대상을 명시한다.

| 명령 | 단계 | 결과 | 독립 실행 단위 |
| --- | --- | --- | --- |
| g0-19-worker | G0 | FAIL: 기본 headless_shell의 dxil.dll 로딩 오류 | 1 |
| g0-20-full-chromium | G0 | PASS: 같은 shared-runtime Worker, first pixels/resize/dispose | 1 |
| t1-21-profile-publish | T1 | PASS: 선택적 layout self-time 계측 publish | 1 |
| t1-22-layout-cost | T1/T7 | 실행 PASS, 가치 있는 순수 커널 미발견 → 미채택 | 1 |
| g1-23-product-publish | G1/G2 | FAIL: TS union 수정 시 잘못된 bitwise 표현식 | 1 |
| g1-24-product-contract | G1/G2 | FAIL: document presenter에 확장 renderer union 할당 | 1 |
| g1-25-product-owner | G1/G2 | PASS: product trimmed publish | 1 |
| g1-26-product-content | G1/G3 | PASS: 실제 Material canvas, AMD rdna-3 hardware WebGPU, colors 1,376, resize/wheel | 1 |
| g3-27-state-lifetime | G3 | 상태/theme/reparent PASS; 정상 종료·invalid protocol 종료 FAIL: mapAsync Aborted | 3 |
| g3-28-drain-publish | G2/G3 | PASS: owned device의 pending map 추적·초기화 실패 해제 보완 build | 1 |
| g3-29-drain-lifetime | G3 | FAIL: 정상/invalid 종료 map 오류 지속, device loss disposed 미확인; unavailable fixture의 DOM 기대도 오류 | 4 |
| g3-30-retire | G3/G4 | PASS: 후보 22파일 hash 검증 보존, 제품 15파일 원복·추가 7파일 제거 | 1 |
| u3-31-ios-restore | U3 | setup FAIL DOROTIAPP203: restore -r은 고정 RuntimeIdentifier를 제공하지 않음 | 1 |
| u3-32-ios-fixed-rid | U3 | PASS: 명시적 RuntimeIdentifier=iossimulator-arm64 정상 restore | 1 |
| u3-33-macos-restore | U3 | PASS: macOS osx-arm64 정상 restore | 1 |

두 번째 묶음은 15명령/20실행 단위로 종료했다.

## G3 필수 gate 실패 / G4 미채택

첫 제품 화면과 상태 보존은 통과했지만 정상 종료의 `mapAsync Aborted` 오류를
해결하지 못했다. queue 완료 후 기존 mapping을 기다리는 변경만으로는 충분하지
않았다. native context/recorder 해제 과정에서 cache 반환이 새 mapping을 시작할
수 있다는 것이 남은 원인 가설이다. upstream m154의
[DawnBuffer](https://raw.githubusercontent.com/google/skia/chrome/m154/src/gpu/graphite/dawn/DawnBuffer.cpp)와
[ResourceCache](https://raw.githubusercontent.com/google/skia/chrome/m154/src/gpu/graphite/ResourceCache.cpp)는
cache 반환 때 비동기 재매핑하는 경로를 가진다. 이를 실제 실패의 확정 원인이나
새 수정의 PASS로 보고하지 않는다.

장치 손실 주입은 `Device was lost before mapping was resolved` 오류와 함께
20초의 disposed 관찰 조건을 충족하지 못했다. 외부 전체 timeout 20분에 도달한
것은 아니다. unavailable 주입은 명시적 오류와 host 제거까지 확인했으나 fixture가
제거된 DOM에 disposed 속성을 기대해 FAIL이었다. 이 harness 오류가 다른 수명
FAIL을 무효화하지 않는다.

14.4절의 원복 규칙을 적용했다. 제품 loader/types/protocol, Graphite surface,
공통 picture cache/effects 변경과 추가 native exports를 제거하고 WebGL 경로로
복귀했다. 통과한 U 패키지/격리 output 수정과 T1 기본 비활성 계측은 유지했다.
[후보 patch](work3-webgpu-candidate/candidate.patch)와 원본 파일 22개 및 SHA-256
[manifest](work3-webgpu-candidate/manifest.json)를 보존했다. G0/G1 성공과 최초 G3
실패를 덮어쓰지 않는다. G3 후속 DPR/이미지/shader 전체 corpus와 G4 성능 3쌍은
필수 수명 gate 실패로 수행하지 않았다. G는 **미채택**, 성능은
`performanceUnproven`이며 WebGPU 추가 완료로 표시하지 않는다.

## 마지막 검증 묶음

WebGPU 원복 후 남는 U/T 소스의 최종 publish, WebGL 상태/수명 3 case, DPR
1/1.25/1.5/2 hover 4 case, MacCatalyst restore, CLI/생성 앱 및 runtime shader
계약, 최종 hash/문서 검사로 범위를 고정한다. 다중 host 및 4개 DPR을 각각 세기
위해 기본 10회에서 최대 20회로 확장한다. G 기능/performance의 실패한 선행 gate를
우회하는 반복은 수행하지 않는다. 물리 Apple/Android/Linux 실행은 이 Windows
환경의 restore/build 결과와 구분한다.

| 명령 | 단계 | 결과 | 독립 실행 단위 |
| --- | --- | --- | --- |
| u2-34-final-publish | U2/T7 | PASS: G 원복 후 최종 threaded trimmed Release, 144.877초 | 1 |
| u2-35-final-regression | U2/U3 | PASS: 상태/theme/reparent, DPR 1/1.25/1.5/2 hover 각 8 captures, 정상/invalid 종료, 7 cases | 7 |
| u3-36-maccatalyst | U3 | PASS: maccatalyst-arm64 정상 restore | 1 |
| u3-37-shader-contract | U3 | PASS: 기존 native runtime shader/image-filter 컴파일·uniform/cache/pixel 계약 | 1 |
| u3-38-generated-runner | U3 | PASS: SyntheticRunner 생성·restore/build, warnings/errors 0 | 1 |
| u3-39-cli-doctor | U3 | PASS: 실제 CLI가 Testbed Web runner와 설치 toolchain 확인 | 1 |
| u3-40-final-audit | U0/U3/G4 | PASS: 버전 혼합 0, native link 각 1회, S1/S2 hash 일치, G 제품 원복 hash 검증 | 1 |
| u3-41-image-text | U2/U3 | 한글 자동 입력 PASS; image fixture FAIL: 오래된 radio locator와 crop 위치 | 2 |
| u3-42-image-locator | U2/U3 | PASS: 현재 ChoiceChip checkbox/사진 bounds로 수정; 사진 pixels·fit·idle scroll·색상 추출 | 1 |
| final-43-diff-check | U3/G4/T7/J0 | PASS: 최종 diff whitespace 검사 | 1 |

총 **43명령 / 57실행 단위**, 통합 묶음별 **20 / 20 / 17**이다. 모든 자동 retry는
0이며 실패·setup·분리된 browser context를 포함한다. 독립 성능 3쌍을 수행한
것으로 세지 않는다. [기계 판독 결과](work3-results.json)에 명령/HEAD/시간/해시와
원본 log 경로를 남겼다. G3 최초/수정 후 실패 log는 후보 폴더에도 복사했다.
검증 서버 5200–5205는 종료했으며 원래 실행 중이던 S1 서버는 건드리지 않았다.

이미지 fixture의 최초 timeout은 `Contain`을 radio로 찾다가 발생했다. 당시 PNG에
사진이 정상 표시되어 있었고, source와 accessibility snapshot 모두 ChoiceChip을
checkbox로 표시했다. 사진이 버튼 위에 있는데 아래를 잘라 보던 crop도 실제 img
bounds 기준으로 수정했다. palette label도 현재 `Primary #RRGGBB` 형식으로 고쳤다.
제품 이미지 로직을 변경하지 않고 색상 추출의 명시적 동작까지 재검증했다.

## 최종 수용 범위

| 대상 | 자동 증거 | 실제 플랫폼/물리 수용 |
| --- | --- | --- |
| Web / Windows Chromium 151 hardware WebGL2 | 최종 publish, first content/resize/wheel, 상태/theme/reparent, DPR 4개, 이미지 fit/색상 추출, 한글 문자열 입력, 역할 종료/protocol PASS | OS 한글 IME 조합·screen reader·scan-out·다른 browser/GPU는 notVerified |
| Windows App SDK / win-x64 | Release build와 실제 native Vulkan first-content smoke PASS, VisibleAfterExactPresent=true, GPU errors=0 | 물리 input/resize/장시간 scroll/성능 notVerified |
| MAUI Windows / win-x64 | Release build PASS | 실행/물리 수용 notVerified |
| Android / android-arm64 | 최초 CS1705 해결 후 Release APK build PASS | device/IME/accessibility notVerified |
| Linux Qt / linux-x64 | Windows에서 managed Linux head build PASS | 실제 Linux Qt/native presenter 실행 notVerified |
| iOS / iossimulator-arm64 | 명시적 fixed RID restore PASS | Xcode build/simulator/device 실행 notVerified |
| macOS / osx-arm64 | restore PASS | AppKit/native build/실행 notVerified |
| Mac Catalyst / maccatalyst-arm64 | restore PASS | native build/실행 notVerified |
| SDK/생성 앱/CLI | Runner SDK local pack, SyntheticRunner 생성·build, CLI doctor PASS | 전체 package 배포와 외부 소비자/다른 RID 전체 행렬은 notVerified |

원복된 최종 Web asset은 41,700,364 bytes(비압축 합계), 고정 S2보다 +1,114 bytes다.
추가된 기본 비활성 T1 계측 등을 포함하며 성능 개선을 뜻하지 않는다. 최종 native
WASM은 8,744,456 bytes다. 관리 heap/GC는 동일 수집 경계 비교가 아니며 GPU 시간,
GPU/process/live memory, physical FPS는 notMeasured로 유지한다.

**U:** 패키지 전환과 사용 가능한 환경의 회귀·플랫폼 범위 판정을 마쳤다. 정확한
19-package/native inventory와 실제 restore 해시를 보존했다. 모든 OS 물리 완료를
의미하지 않는다. S2는 Web 범위에서 수용한다.

**G:** WebGPU 추가는 완료되지 않았다. 첫 제품 출력은 성공했지만 G3 필수 수명
FAIL 때문에 원복했고, 후보 patch는 `git apply --check`를 통과했다. G2 전체 corpus와
G4 성능 비교를 생략한 것을 PASS로 기록하지 않는다.

**T/J:** T1의 낮은 계산 비중에 근거해 병렬 layout은 미채택으로 종료한다. T0b/T2–T6과
J0는 선행 후보가 없어 notApplicable이다. 기본 WebGL, main runtime 1개, shared heap,
JS-affine owner, 직렬 layout을 유지한다.

## 사용자 직접 확인용 실행

후속 요청에 따라 Chrome에 보존된 G 후보와 S2 비교 탭을 열고 두 화면의 첫
Material pixels를 확인했다. `5204`는 `.doroti/work3/g3/drain/wwwroot`의 실패한
WebGPU 후보, `5200`은 `.doroti/work3/u2/s2/wwwroot`의 WebGL 기준선이다. 두 서버는
사용자가 직접 확인할 수 있도록 다시 실행해 두었다. 이 2개의 대화형 확인 세션은
위 57개의 자동 실행 단위와 별도로 기록하며, 포함하면 세 번째 묶음 19/총 59다.
새 성능 비교나 기존 G3 실패의 수용 전환이 아니다. 두 화면의 layout은 모두 직렬이다.

- [WebGPU 후보](http://127.0.0.1:5204/?dorotiRenderer=worker-direct-webgpu&dorotiTestbedMode=sample)
- [WebGL S2 비교](http://127.0.0.1:5200/?dorotiRenderer=worker-direct-webgl&dorotiTestbedMode=sample)

위 두 탭을 처음 열었을 때는 병렬 executor가 없었다. 이후 사용자의 명시적인
구현 요청으로 아래 실험을 추가했다.

## 후속 요청: 직접 실행하는 병렬 레이아웃

사용자의 “병렬레이아웃도 그냥 구현해보면 안되나” 요청에 따라 T를 재개했다.
기존 T1의 0.3% 미만 측정과 미채택 판정은 당시 결과로 보존한다. 이번 목표는
속도 개선 수용과 별개인 **실제 두 스레드 숫자 레이아웃의 가시적인 실험판**이다.

- 공용 `Doroti.Framework.Rendering/PreparedTreemapLayout.cs`: 확정된 weights와
  bounds만 복사하여 두 subtree의 weighted binary treemap geometry를 계산한다.
  동일 알고리즘의 직렬 모드가 있고, 두 전용 managed thread와 lane별 capacity 1
  queue, 한 batch 제한, 취소 및 비동기 종료를 제공한다. compute thread는
  Widget/RenderObject/JS/Skia 객체나 사용자 callback을 실행하지 않는다.
- `DorotiTestbedApp/src/ParallelLayoutLab.cs`: post-frame에서 준비를 시작하고
  await 후 owner thread에서 현재 revision만 반영한다. 새 입력은 이전 작업을
  취소하고 최신 하나로 합친다. 기존 결과를 보이는 동안 비동기 계산하며,
  기존 RenderBox.layout을 기다리게 하거나 speculative child layout을 하지 않는다.
  입력 16,384개마다 실제 표시할 tile 하나를 계산하고 paint가 결과를 소비한다.
- 이 실험은 임의 Material/RenderFlex 트리의 병렬 전환이 아니다. UI build,
  기존 synchronous layout, paint는 owner에서 실행한다. 일반 T3–T6 통합과
  T7 성능 수용, WebGPU 조합 J0는 완료로 바꾸지 않는다.

### 실행과 결과

네 번째 묶음은 사용자 요청에 따른 새 구현·회귀 묶음이다. 최초 빌드 오류와
locale 누락, canvas 입력 fixture 수정을 포함한다. 반복 횟수를 늘리는 성능
벤치마크가 아니며, 모든 실행은 timeout 1,200초, 자동 retry 0이다. 최종 횟수는
아래 ledger와 `work3-results.json`에 기록한다.

| 번호 | 결과 | 내용 |
| --- | --- | --- |
| 44 | FAIL | 첫 Web publish: `EdgeInsets.all` 대신 이 저장소의 `CreateAll` 필요 |
| 45 | PASS | 공용 엔진 native contract: 좌표/가중 면적/겹침 없음/취소/종료 |
| 46 | PASS | API 수정 Web Release publish |
| 47 | PASS | app shutdown에서 executor 정리, 입력 범위 검증 보완 publish |
| 48 | FAIL | 브라우저: 새 MaterialApp locale 누락으로 root 생성 실패 |
| 49 | PASS | 보완된 엔진 native contract 재검증 |
| 50 | PASS | locale 지정 Web Release publish, 최종 frozen assets |
| 51 | FAIL | 병렬 실행 확인 후 fixture의 DOM click을 canvas가 가로챔 |
| 52 | PASS × 2 | 실제 pointer fixture로 병렬 전체 시나리오와 기존 Material 회귀 |
| 53 | PASS | final/parallel/S1/S2 asset·package·native link 및 G 원복 audit |
| 54 | PASS | 최종 `git diff --check` |

네 번째 묶음은 명령 11개, 52번의 두 case를 포함한 자동 실행 단위 12개,
대화형 실패/수정 확인 2개로 총 14개다. 10개를 넘긴 이유는 최초 컴파일·locale·
입력 fixture 실패의 원인별 수정 검증과 기존 샘플 회귀이며, 상한 20개 안에서
종료했다. 전체는 명령 54개, 자동 단위 69개, 대화형 포함 73개다.

52번의 [실제 실행 기록](work3-parallel-execution.json)에서 owner는 8, compute는
11/12이고 첫 계산 중첩은 **8.669678ms**였다. 모든 결과는 owner 8로 돌아왔다.
직렬과 병렬 checksum은 `d9e17eb5aeb97ab9`로 같고, 화면 내 tile 영역의 PNG
바이트도 동일했다. seed 변경은 새 geometry에 반영됐고, 900→720→1000→1280
resize 후 최신 width 1240과 seed가 유지됐다. 정상 종료의 disposed 응답이 있고
fatal 및 browser runtime error는 0이다. [최종 화면](work3-parallel.png)을 보존했다.

`audit-parallel.json`은 S1/S2/이전 Final을 보존하면서 새 snapshot의 파일 해시와
단일 native link, Skia 4.154 정합성을 확인한다. 새 snapshot의 raw 총량
48,731,637 bytes에는 incremental publish의 미참조 과거 managed fingerprint
파일도 포함된다. 이는 네트워크 전송량 또는 깨끗한 배포 패키지 크기 측정이
아니다. 실제 로드되는 runtime은 index import map과 embedded boot resources가
지정하며, native wasm은 8,744,456 bytes 한 개다.

첫 병렬 준비는 thread 초기화를 포함해 38.19ms, 같은 session의 직렬 준비는
5.60ms, 후속 병렬 준비는 5.47ms였다. 이것은 실행 증거이며 조건을 맞춘 반복
성능 비교나 속도 향상/FPS 주장이 아니다. 네이티브 contract의 면적 검증과
브라우저 픽셀/입력 검증도 모든 OS의 물리 수용을 뜻하지 않는다.

### 직접 보기

- [병렬 레이아웃 실험판](http://127.0.0.1:5206/?dorotiRenderer=worker-direct-webgl&dorotiTestbedMode=parallel-layout&dorotiLayoutMode=parallel)
- [같은 레이아웃의 직렬 시작 모드](http://127.0.0.1:5206/?dorotiRenderer=worker-direct-webgl&dorotiTestbedMode=parallel-layout&dorotiLayoutMode=serial)
- 앞서 열어 둔 [WebGPU 후보](http://127.0.0.1:5204/?dorotiRenderer=worker-direct-webgpu&dorotiTestbedMode=sample)는
  계속 G3 실패 후보이며, 병렬 실험판과 결합하지 않았다.

화면의 `Serial` / `Parallel (2 threads)`로 동일 입력의 실행 방식을 바꾸고,
`New layout`으로 입력을 바꾼다. 브라우저 크기를 바꾸면 최신 bounds로 준비한다.
Chrome의 최종 병렬 화면에서도 owner 8, compute 10/11, 양의 중첩과 tile pixels를
직접 관찰했다. 별도 자동 세션의 11/12와 thread ID가 다른 것은 정상이다.
초기 locale 실패 탭과 수정 후 reload를 각각 대화형 확인 1회로 센다.

서버 5206은 `.doroti/work3/parallel/final-wwwroot`를 제공하며 열어 두었다.
다시 시작하려면 저장소 루트에서 다음 명령을 실행한다.

```powershell
python Doroti/eng/serve-isolated-web.py .doroti/work3/parallel/final-wwwroot --port 5206
```

재빌드: `dotnet publish DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj -c Release
--artifacts-path .doroti/work3/parallel/artifacts` (한 줄). output의
`publish/DorotiTestbedApp.Web/release/wwwroot`를 별도 snapshot에 복사하여 제공한다.
이미 보존한 실패/최종 snapshot은 덮어쓰지 않는다.

## 후속 요청: WebGPU Material + 병렬 패널 결합

사용자는 5204 WebGPU sample이 생각보다 괜찮다고 관찰했고, 여기에 병렬
레이아웃을 더해 직접 보고 싶다고 요청했다. 같은 Material 앱의 상단에
`dorotiParallelLayout=1` 패널을 연결했다. 타일 숫자 계산의 병렬화 범위는
유지하며, 임의 기존 Material widget callback의 병렬화로 확대하지 않았다.

제품 루트의 G 원복 상태와 기존 서버를 유지하고, `.doroti/work3/combined/source`
detached worktree에서 보존된 G 후보와 현재 U/T를 결합했다. 첫 결합은 Graphite가
canvas texture를 샘플링할 때 `TextureBinding` usage가 없어 fatal/disposed로
진입했다. 실제 GPU 메시지를 기록한 뒤 texture binding/copy-source 사용을 추가했다.
또한 기존 BlockingCollection.TryAdd가 0 timeout에도 owner의 동기 semaphore
API를 호출한다는 경고를 확인하고 공용 엔진을 bounded Channel로 고쳤다.
이 경고는 이전 WebGL 자동 검증의 runtime error 0 판정과 별개로 이번에 발견됐다.

| 번호 | 결과 | 내용 |
| --- | --- | --- |
| 55 | PASS | 격리 source snapshot 및 원본 G hash 확인 |
| 56 | PASS | 첫 결합 Web Release publish |
| 57 | FAIL × 2 | 첫 parallel 결과 뒤 GPU validation fatal; 이후 테스트도 시작 중 종료 |
| 58 | PASS | 첫 snapshot/source/boot/native-link audit |
| 59 | 진단 완료 | 실제 fatal의 GPUValidationError와 owner 동기 대기 경고 보존 |
| 60 | 진단 완료 | GPU texture binding 누락 메시지 확보; exit 0은 기능 PASS가 아님 |
| 61 | PASS | texture usage·GPU 오류 메시지·Channel 수정 publish |
| 62 | PASS | Channel 엔진 native geometry/cancellation/shutdown contract |
| 63 | PASS | 새 snapshot 작성, 미참조 과거 fingerprint 파일 30개 제외 |
| 64 | FAIL × 2 | 화면은 유지됨; exact PNG 차이와 기존 정상 종료 mapping 오류 |
| 65 | PARTIAL / test FAIL | 좌표·thread·resize·seed·theme·scroll 확인, exact pixel gate FAIL |
| 66 | PASS × 2 | 같은 build의 WebGL 병렬 단독 및 기존 Material 상태·resize·theme 회귀 |
| 67 | PASS | 수정 snapshot의 active boot 74개 hash, native link, package, 이전 snapshot 확인 |
| 68 | PASS | 최종 diff 공백 검사 |

다섯 번째 묶음은 명령 14개/자동 단위 17개와 대화형 실패/수정 확인 2개로 총
19개다. 10개를 넘은 이유는 첫 결합 오류의 구체적 GPU 진단, 공용 queue 경고 수정,
픽셀 차이의 정량 분리, 기존 WebGL 회귀다. timeout 1,200초, retry 0과 상한 20개를
지켰다. 전체는 명령 68개/자동 86개/대화형 포함 92개다.

65번: main runtime 1/shared heap, owner 8, compute 11/12, 최초 overlap 8.369873ms.
직렬과 병렬 checksum은 `cd4c22f67f37d886`로 일치했다. 모든 결과가 owner에
돌아왔고, 새 seed·800→1280 resize·theme·scroll 후 상태를 유지했다. 실행 중
runtime error와 owner 동기 대기 경고는 0이었다. 다만 tile 영역 110,700픽셀 중
23개가 달랐고 최대 채널 차이는 13이다. 첫 병렬/후속 직렬의 렌더 경로 차이 원인은
확정하지 않았으며, exact pixel gate를 통과로 바꾸지 않았다. 실제 overlap 0인
후속 pair도 있으므로 매 작업의 동시 실행이나 속도 개선을 주장하지 않는다.

64번의 정상 종료는 원래 Dawn mapping 오류를 다시 보였다. device loss, 전체
effects/readback/DPR corpus와 정량 성능 수용은 재검증하지 않았다. **직접 실행
가능한 결합 실험판이며 J0 수용 완료는 아니다.** [결합 후보와 증거](work3-combined-candidate/README.md)에
소스 override, 실제 좌표·스레드 기록, 원본 PNG 비교, 최초 오류 로그를 보존했다.

[WebGPU + 병렬 패널 실행](http://127.0.0.1:5208/?dorotiRenderer=worker-direct-webgpu&dorotiTestbedMode=sample&dorotiParallelLayout=1&dorotiLayoutMode=parallel).
Chrome에서 수정 후 Material 및 타일 pixels를 관찰했고 탭과 서버를 열어 두었다.
현재 제공 경로는 `.doroti/work3/combined/repaired-wwwroot`이고 raw 총량은
42,693,478 bytes다. 재시작 명령과 재현 방식은 후보 README를 따른다.
