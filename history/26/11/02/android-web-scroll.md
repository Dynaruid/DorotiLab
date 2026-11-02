# Android 브라우저 Web 스크롤 개선

날짜: 2026-11-02 (Asia/Seoul). 시작 HEAD:
`03b654ae17a243728c896e53ff649a3e19d89700`.

사용자가 말한 Android Web은 **Android 브라우저에서 실행한 Web Testbed**다.
네이티브 Android는 이미 부드럽다는 설명을 반영했다. Android 네이티브 입력 브리지,
스크롤 물리·관성 설정은 수정하지 않았다. 아래 공통 Skia 최적화는 다른 호스트에서도
사용되지만, 새 raster promotion 정책은 Web만 명시적으로 활성화한다.

## 원인과 변경

실제 Android 브라우저의 Release AOT Testbed를 움직이면서 owner CPU profile과
framework/raster/input 진단을 함께 확인했다. Main-thread RAF만 보면 대체로 60Hz여도
framework 갱신과 raster/submit은 늦을 수 있었다. 다음 비용을 줄였다.

- [접근성 갱신](../../../../Doroti/src/Doroti.Framework.Rendering/object.cs): 이미 대기 중인
  scroll-offset 전용 갱신은 설정을 다시 만들고 조상을 다시 순회하지 않는다. Flush에서
  마지막 offset을 읽으며, 일반 내용·action 변경이 들어오면 전체 갱신이 우선한다.
- [Web semantics 전송](../../../../packages/platforms/web/Doroti.Host.Web/BrowserSemanticsTransport.cs):
  유지 중인 노드 내용을 비교할 때 `record with` 복사 두 개를 매번 할당하지 않는다.
  기존 content/geometry 구분과 모든 wire 필드의 의미를 유지한다.
- [텍스트 재생](../../../../Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs):
  자간·단어 간격이 있는 텍스트의 glyph와 위치를 bounded native blob으로 재사용한다.
  폰트/style당 최대 32개·2,048 UTF-16 code unit이며 한 문자열은 최대 1,024 unit이다.
  malformed UTF-16과 긴 문자열은 기존 경로로 그린다. 빈 fallback/variation 목록은
  JSON/LINQ 파이프라인을 만들지 않는다. 폰트 등록 시 기존 glyph/picture 캐시를 폐기한다.
- [그림자 picture](../../../../Doroti/src/Doroti.Skia.Rendering/SkiaPictureCommandCache.cs):
  그림자가 포함된 유지 picture도 native command recording을 재사용한다. 그림자의
  device-space 계산을 보존하도록 선형 변환을 recording에 적용하고 실제 translation은
  재생 시 적용한다. 그림자 여백을 cull bounds에 포함한다. Scale/DPR 변경은 무효화하고,
  perspective·unbounded paint·기존 blur 예외는 직접 그린다. 기존 LRU/명령/byte 한도를 유지한다.
- [Web 호스트](../../../../packages/platforms/web/Doroti.Host.Web/BrowserSkiaCapabilities.cs):
  스크롤 중 새 offscreen raster cache 생성을 미룬다. 이미 호환되는 image는 재사용하고,
  miss는 위 native vector recording으로 재생한다. 안정화 후 promotion을 재개한다.
  `DeferPictureRasterPromotionsDuringScroll`의 기본값은 false여서 다른 호스트의 정책은 유지된다.

## Android 브라우저 비교

Pixel 5 AVD, Android 13 x86_64, 1080×2340, Chromium `159.0.8137.0`
(`org.chromium.chrome`), CSS viewport 392×665 / DPR 2.75.
공식 Chromium snapshot revision `1732428` APK를 사용했다.
에뮬레이터 `-gpu host`에서 RTX 4060 / ANGLE Vulkan으로 실행했으며 최종 GPU process
crash count는 0이다. 기본 auto renderer는 이 환경에서 `worker-direct-webgl`로 fallback했다.
.NET `10.0.11` Release Mono WASM AOT이며 baseline과 수정본의 빌드 옵션은 동일하다.
Baseline C#은 시작 HEAD이고, 앞서 완료한 Web IME 선택 수정은 두 빌드 모두 포함한다.

한 번 누른 뒤 3.6초 동안 위·아래로 세 번 방향을 바꾸는 동일한 shell touch 경로를
120Hz 목표로 주입했다. 입력은 실제 Android browser → managed framework → renderer를
통과한다. DOM `scrollTop`이나 앱 스크롤 값을 직접 설정한 측정이 아니다.
측정 중 빌드·화면 녹화·CPU profiling은 실행하지 않았다.

Warm 비교는 한 번 사전 왕복 후 위로 돌아와 동일한 경로를 다시 측정했다.
Baseline은 한 번이며 최종본은 최초 비교와 추적할 측정 스크립트의 마지막 실행 두 번이다.
각 값은 해당 실행 내 p95다. 반복 모집단에 대한 통계적 보장은 아니다.

| Warm 비교 | 수정 전 | 수정 후 1 / 2 |
| --- | ---: | ---: |
| Framework callback p95 | 12.93 ms | 11.93 / 13.91 ms |
| Raster replay/submit CPU p95 | 15.36 ms | 8.16 / 9.62 ms |
| 입력 대기 p95 | 41.55 ms | 24.77 / 37.42 ms |
| 드래그 중 submit 간격 p50 | 23.99 ms | 18.85 / 21.85 ms |
| 드래그 중 submit 간격 p95 | 43.44 ms | 42.18 / 44.00 ms |
| 측정 구간 새 raster promotion | 31 | 0 / 0 |

Warm 구간 raster CPU p95는 약 37–47%, 입력 대기 p95는 약 10–40% 감소했다.
Framework 및 submit 간격 p95는 실행마다 차이가 있어 일관된 개선으로 주장하지 않는다.
관리 할당량은 interval 전체 58,207,256 → 51,177,528 B이며 callback 수는 143 → 169다.
첫 비교의 callback당 정규화 값은 407,044 → 302,826 B로 약 26% 감소했지만
총 heap/RSS/native memory 측정은 아니다.

Cold 비교는 각각 reload 후 사전 왕복 없이 두 번 측정했다.

| Cold 연속 드래그 | 수정 전 1 / 2 | 수정 후 1 / 2 |
| --- | ---: | ---: |
| Raster CPU p95 | 23.86 / 25.17 ms | 19.60 / 11.83 ms |
| 입력 대기 p95 | 77.47 / 67.92 ms | 77.93 / 46.12 ms |
| 드래그 중 submit 간격 p95 | 67.90 / 58.73 ms | 53.73 / 49.38 ms |
| 새 raster promotion | 17 / 25 | 0 / 0 |

첫 cold 실행의 입력 대기 p95는 개선되지 않았다. 첫 사용 시 긴 framework frame과
40–50ms 이상 submit 간격이 남아 있으므로 **고정 60fps 또는 모든 버벅임 제거를 주장하지 않는다**.
수정 후 일반 swipe 두 번도 오류 없이 통과했다. Windows Chrome `156.0.8078.79`의
동일 Web Testbed touch smoke는 auto WebGPU로 실행돼 오류 없이 통과했다.
이 결과를 Android WebGL 성능과 직접 비교하지 않는다.

`raster`는 worker의 CPU 재생/제출 시간이다. `Presented=0`인 진단은 물리 화면 표시를
증명하지 않는다. RAF, GPU submit 간격, 에뮬레이터 주입 입력만으로 실기기의 손가락 체감이나
display scanout FPS를 승인하지 않는다. 물리 Android 폰과 사용자 브라우저에서의 체감은 미검증이다.

## 회귀 확인

- Release AOT publish 완료. Android browser가 읽은 JS와 native WASM의 SHA-256을
  frozen publish 디렉터리와 대조했다. 파일/소스 hash는 아래 JSON에 보존했다.
- `Doroti.Scroll.Tests`: 반복 scroll semantics coalescing, 최종 offset/extents,
  content/action 우선순위, 모든 Web semantics 필드와 무할당 비교 검사 통과.
- `Doroti.Skia.Rendering.Tests`: 자간 텍스트 21개 및 그림자/text picture 24개의
  정확한 CPU pixel 비교 통과. 한글·emoji·combining mark·fallback·malformed UTF-16,
  fractional 이동·clip·affine/DPR·폰트 등록 무효화·캐시 폐기·무할당 재생을 포함한다.
  Web promotion opt-in/quiet interval/기본 native 정책도 검사했다.
- 최종 Android AOT Testbed의 기본 Components → Text fields → Filled에서 실제
  Gboard 두벌식 키로 `한글` 입력 → 길게 눌러 `[0,2)` 선택 → 소프트 키보드 삭제:
  DOM IME와 공유 Filled controller 모두 빈 문자열로 일치했다. 앞선
  [Web IME 수정](web-ime-selection-deletion.md)을 유지했다.

최종 화면 대조 도중 한 번 한글이 네모로 보였다. 처음에는 새 캐시 회귀를 의심했으나,
HTTP cache를 비우고 network latency를 1,200ms로 준 대조에서 **baseline과 최종본 모두**
다운로드 전에는 네모, Noto fallback 다운로드 후에는 정상 한글로 바뀌었다.
같은 바이너리·문자열에서 정상화됐으며 최종 입력/선택/삭제도 재검증했다.
이는 기존 on-demand font 로딩의 일시적인 표시에 해당한다. 이번 스크롤 수정에서
폰트 공급 정책을 바꾸거나 이 cold-font 지연을 해결했다고 주장하지 않는다.

초기 SwiftShader 시도는 GPU context crash가 있어 비교에서 제외했다. 기존 Chrome 109의
AOT 시작 실패와 옛 gesture jar의 Android 13 reflection 실패도 성능 baseline으로 사용하지 않았다.
Driver를 현재 `Doroti/eng/android/ScrollGesture.java`에서 다시 만들어 비교했다.

## 재검증 명령과 증거

프로젝트 루트에서 모든 build/test 명령은 1,200초 wrapper로 실행한다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet publish samples/DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj -c Release -p:RunAOTCompilation=true -p:WasmEmitSymbolMap=true -o temp/testing/android-web-scroll/final-publish -v:minimal
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Scroll.Tests/Doroti.Scroll.Tests.csproj -c Release -m:1 -nr:false
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Skia.Rendering.Tests/Doroti.Skia.Rendering.Tests.csproj -c Release -m:1 -nr:false
```

측정 도구는 [measure-android-web-scroll.py](../../../../Doroti/eng/measure-android-web-scroll.py)다.
Python Playwright, 격리 헤더를 제공하는 publish 서버, Chrome CDP `9223`, Android reverse
port가 필요하다. 이 도구의 touch 좌표는 위 Pixel 5 화면 전용이다. 아래 `$probePython`은
Playwright를 설치한 Python이고, 예시의 `5102`는 테스트용 publish 서버다.

```powershell
adb -s emulator-5554 forward tcp:9223 localabstract:chrome_devtools_remote
adb -s emulator-5554 reverse tcp:5102 tcp:5102
python Doroti/eng/run-with-timeout.py --timeout 1200 $probePython Doroti/eng/measure-android-web-scroll.py --label verify-swipes --url http://127.0.0.1:5102/ --gesture swipes --trials 2
```

연속 드래그는 현재 `ScrollGesture.java`를 dex jar로 만든 뒤
`/data/local/tmp/doroti-scroll.jar`에 push해야 한다. 이번 빌드는 JDK 21 `javac --release 8`,
Android SDK 35의 `android.jar`, build-tools 36.0.0의 D8 `--min-api 26`을 사용했다.
준비 후 같은 명령에 `--gesture continuous --trials 2`를 주고 warm 비교에는 `--warmup`을 추가한다.
기존 네이티브용 옛 jar를 재사용하지 않는다.

`ownerAllocatedBytes`는 `BeginCostInterval` reset 이후의 **구간 값**이다.
Before 값을 다시 빼면 잘못된 결과가 된다. 누적 work counter만 before/after 차이를 사용한다.
전체 submit 간격에는 idle tail이 포함되므로 드래그 비교에는 `rasterIntervalDuringDrag`를 사용한다.

- [지속 보존 비교·테스트·소스·전달 자산 증거 JSON](android-web-scroll-evidence.json)
- [최종 한글 표시](android-web-hangul.png), [소프트 키보드 삭제 후](android-web-hangul-deleted.png)
- 원시 trace/profile/log/frozen publish: `temp/testing/android-web-scroll/`의 로컬 재생성 산출물.
  비교에 사용한 raw 파일 hash와 요약은 JSON에 보존했다. 원시 디렉터리의 영구 보존은 보장하지 않는다.
