# All Components 텍스트 커서 드래그 지연 수정

작성일: 2026-11-01 (Asia/Seoul)  
기준 HEAD: `d608a30076c07e3396d76226f219d7bdcda27ae2` + 작업 트리 변경  
판정: **반복되는 큰 관리 GC 탐색 제거, 최종 APK 갤럭시 자동 비교·입력 회귀 통과 / 실제 손가락·표시 합격 notVerified**

사용자가 알려준 “입력 테스트 페이지는 거의 괜찮지만 All Components의 TextField는 심하게 버벅임”을
실제 기본 페이지에서 재현했다. 입력 전용 fixture의 결과로 대체하지 않았다.
최종 APK 3회에서 드래그 중 owner p95는 51.5–53.1ms에서 25.8–28.5ms로 줄었고,
50ms 이상 owner 작업은 25회에서 0회로 줄었다. p50은 13.6–13.7ms에서 14.8–14.9ms로 늘었다.
모든 지표가 개선되었다거나 스크롤 전체 성능 조건을 만족했다고 판정하지 않는다.

집계, APK·소스·원시 증거 hash와 입력 회귀 결과:
[검증 결과 JSON](galaxy-components-caret-fix-results.json).
이전 [확대경·접근성 검토](../../10/31/galaxy-caret-accessibility-review.md)의 fixture 결과와 APK hash는
당시의 별도 증거로 보존한다.

## 확인한 원인과 수정

All Components는 여러 컴포넌트와 상태를 유지한다. Filled 3개와 Outlined 3개는 각각 같은
컨트롤러를 공유한다. 이는 Flutter reference의 `component_screen.dart`와 같은 구성으로 유지했다.
컨트롤러를 분리하거나 페이지 내용을 줄여 측정을 유리하게 만들지 않았다.

1. 선택 위치만 바뀌어도 공유 컨트롤러의 EditableText 3개가 다시 빌드되고, LayoutBuilder 안의
   빌드와 Material 장식의 동일 자식 detach/adopt가 레이아웃으로 이어졌다.
   일반 컨트롤러의 유효한 collapsed selection 이동은 실제 RenderEditable의 선택·paint·semantics만
   갱신한다. 텍스트·조합·범위 선택·포커스·obscure·사용자 컨트롤러·legacy selection controls는
   기존 빌드를 유지한다. 보관된 leaf도 컨트롤러의 최신 값을 읽는다.
2. 이미 포커스와 표시 중인 IME를 가진 입력칸에 반복하던 focus/ShowSoftInput 요청을 생략한다.
   OS의 실제 IME inset 가시성을 확인하므로 키보드를 닫은 후 재열기와 포커스 전환은 정상 경로를 탄다.
3. 긴 지연의 직접 원인은 숨겨진 Android 입력칸의 선택 동기화였다. MAUI의
   `EditTextExtensions.UpdateCursorSelection`과 Doroti의 `PublishPendingEditingState`가
   일회용 Java Runnable을 큐에 등록했다. 특히 지연 알림이 bridge → host → framework 전체를
   강하게 참조하여 Mono SGen의 cross-runtime bridge 탐색이 약 30만 객체를 방문했다.
   별도 진단에서 실제 큐의 메서드 이름과 delegate 연결을 확인했다.
4. Android에서는 `EditText.SetSelection`을 먼저 적용한 뒤 MAUI 속성을 맞춘다. 이미 수락된 네이티브
   선택을 본 mapper가 오래된 선택을 다시 예약하지 않는다. 텍스트/Editable을 교체하지 않으며,
   실제 네이티브 길이를 확인해 범위를 벗어난 호출을 막는다. 지연된 선택 알림은 입력 bridge를
   약하게 참조하는 재사용 Action을 쓴다. 입력 bridge의 소유권은 기존 surface/입력 수명이 유지한다.
   Android의 지연 알림도 현재 네이티브 selection과 현재 텍스트를 함께 읽는다.

접근성 provider와 최신 query/action 경로는 유지했다. enabled accessibility services는 `null`인
상태에서 조사했고 OS 설정은 바꾸지 않았다. 활성 TalkBack의 읽기·탐색 합격은 검증하지 않았다.

MAUI의 focused EditText mapper가 선택 변경을 `Post`하는 동작은
[공식 구현](https://github.com/dotnet/maui/blob/main/src/Core/src/Platform/Android/EditTextExtensions.cs)에서 확인했다.
Mono의 [Tarjan bridge 구현](https://github.com/dotnet/runtime/blob/main/src/mono/mono/metadata/sgen-tarjan-bridge.c)과
[세대별 live 판정](https://github.com/dotnet/runtime/blob/main/src/mono/mono/sgen/sgen-gc.c)을 함께 검토했다.
아래 원인 판정은 문서 추정에 그치지 않고 기기 로그와 수정 전후 진단을 근거로 한다.

## 실제 기본 페이지 3회 비교

Galaxy SM-S931N, Android 17/API 37, 1080×2340, arm64, normal Release Mono/profiled AOT,
interpreter off, Optimize on, 공식 SkiaSharp 4.154.0을 사용했다.
기본 Components를 열고 왼쪽 가장자리에서 실제 목록을 스크롤해 Filled 필드에 도달했다.
80자 입력 후 커서 핸들을 하나의 DOWN → 287 MOVE → UP으로 2.4초 동안 왕복했다.
실제 필드 위치에서 Y를 보정했으며 공유 Filled 3개의 텍스트와 최종 선택 75:75를 모두 확인했다.
측정 중 접근성 query·할당 상세 계측·GC 상세 로그·Perfetto·work trace는 켜지 않았다.

| 회차 | 기준 owner p50 / p95 / max ms | 최종 owner p50 / p95 / max ms | 기준 → 최종 50ms 이상 작업 |
| --- | --- | --- | --- |
| 1 | 13.603 / 51.749 / 61.949 | 14.774 / 25.836 / 28.524 | 9 → 0 |
| 2 | 13.567 / 53.072 / 55.049 | 14.889 / 27.432 / 31.688 | 9 → 0 |
| 3 | 13.725 / 51.542 / 91.126 | 14.838 / 28.488 / 34.043 | 7 → 0 |

중앙 draw 할당은 기준 1,535,504–1,548,744 bytes에서 최종 727,328–737,112 bytes로 줄었다.
드래그 중 반복 ShowSoftInput 호출은 최종 3회 모두 0회다. 확대경 생성/해제는 각 1/1,
텍스트 보존·3개 peer 값·유효 선택·fatal/render 오류 검사도 모두 통과했다.
원시 submit 최대 간격은 기준 113.267/116.994/119.470ms, 최종 84.280/68.568/81.720ms다.
이 값을 표시 FPS나 손가락부터 화면까지의 지연으로 환산하지 않는다.
캐럿은 글자 경계에 따라 위치가 바뀌고 네이티브 확대경은 별도 경로이므로,
제출 간격만으로 모든 움직임의 표시 합격을 판정하지 않는다.

기준은 기존 기능 APK에 진단 export만 추가한 `components-matched-baseline` 3회,
최종은 아래 전달 APK의 `components-delivered` 3회다. 플링으로 도달한 최종 Y와
지나간 섹션 수는 달라질 수 있다. 온도·가변 주사율·OS 스케줄링까지 고정한 실험은 아니며,
각 회차의 battery/display 상태와 실제 경로는 원시 증거와 manifest에 남겼다.
DOWN부터 UP까지의 구간만 집계하고 idle tail을 제외했다.

## 원인 분리를 위한 별도 GC 진단

GC 로그와 6초 Perfetto를 켠 별도 진단은 위 일반 3회 비교에 섞지 않았다.
여기서 수정 전은 레이아웃/IME 최적화까지 적용했지만 네이티브 선택 큐 문제는 남은 후보이고,
수정 후는 최종 전달 직전의 native-selection 후보다. 후자의 hash도 JSON에 별도로 보존했다.
최종 APK에는 이후 네이티브 범위 검사와 Android 조건부 적용을 반영했고 일반 3회를 다시 실행했다.

| 드래그 중 진단 | 네이티브 선택 큐 수정 전 | 수정 후 후보 |
| --- | --- | --- |
| bridge 탐색 객체 최대 | 307,796 | 184 |
| Tarjan 탐색 최대 | 54.06ms | 0.04ms |
| minor GC STW p50 / p95 / max | 46.40 / 59.33 / 60.42ms | 2.96 / 5.39 / 5.58ms |

레이아웃·할당 개선만 한 첫 후보는 p95 70.676ms로 악화하여 성능 개선으로 채택하지 않았다.
확대경/프레임 콜백 일부를 약한 참조로 변경, live view의 관리 root 추가,
DispatcherLocal의 직접 소유 저장소, 무효화의 UI 스레드 즉시 실행도 큰 탐색을 제거하지 못해 되돌렸다.
`bridge-implementation=new`와 bridge accounting은 객체 종류 확인에만 썼고 전달 APK에 넣지 않았다.
큐의 delegate를 읽은 임시 reflection probe 역시 제거했다. `debug.mono.log`는 각 진단의 finally에서
원래 빈 값으로 복구했고 마지막 일반 실행에서도 빈 값을 재확인했다.

큰 export JSON을 쓰는 중 ADB가 읽던 경합도 확인하여 임시 파일 작성 후 rename으로 게시한다.
하네스는 새 파일을 지운 상태에서 시작하고 완성된 JSON만 제한 횟수 재시도로 수집한다.
초기 잘린 export와 Infinity 직렬화 실패 결과는 주 비교에서 제외했다. work trace의 특수 숫자 직렬화도 수정했다.

## 검증과 전달 상태

- 최종 Android Release arm64 빌드: 0 warning / 0 error, 1분 18.48초.
- Scroll 회귀: 선택-only paint, 공유 컨트롤러 3개, text/composition/range/focus/custom fallback,
  retained leaf의 최신 선택, Material slot 수명, 약한 지연 알림의 생존/해제 등 7개 PASS 보고.
  기존 스크롤·관성·투영·glyph geometry·Windows native 입력 수명 검사도 같은 실행에 포함했다.
- Windows Testbed Debug win-x64 빌드: 0 warning / 0 error, 1분 33.26초. Windows 실물 IME/체감 판정은 하지 않았다.
- 최종 APK 실제 Components: 키보드 닫기/재열기, 삼성 키보드 한글 조합,
  한글 뒤 커서 드래그와 3개 peer 보존, Filled → Outlined 전환 및 14자 입력,
  0:14 범위 선택, HOME/복귀 뒤 두 컨트롤러 값 보존, fatal/render 오류 0.
- 마지막에는 프로세스를 종료하고 진단 extra 없이 일반 실행했다. 실제 All Components의
  Text fields로 이동해 확인용 텍스트와 커서를 남겨 직접 확인할 수 있게 했다.

최종 빌드·설치 APK SHA-256:
`a0199bec52419e0dce81eae73a184c9d687285ed2df443a37539cdcf19175dbe`.
설치본을 다시 pull하여 빌드본·측정 manifest와 일치함을 확인했다.
기기 시작 로그의 공식 native 4.154.0 / arm64-v8a와 APK 안 `libSkiaSharp.so` SHA-256:
`02c4cf50f41d50750d4e83f3e52c6e80dc7ac2ae43f50fefa381b45e18b81e9b`.
전달 application payload에는 임시 accounting 옵션과 reflection probe가 없다.
런타임 라이브러리 자체의 옵션 파서 문자열은 이 잔류 검사에서 구분했다.

모든 빌드/회귀/기기 하네스에 저장소의 20분 timeout을 적용했다. 재현 예:

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build ./samples/DorotiTestbedApp/android/DorotiTestbedApp.Android.csproj -c Release -r android-arm64 -m:1 -nr:false -v:minimal
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/eng/measure-android-caret.py --serial R3CY30KZA4B --label components-check --components --trials 3
python Doroti/eng/summarize-android-caret.py temp/galaxy-caret/components-check-1.json temp/galaxy-caret/components-check-2.json temp/galaxy-caret/components-check-3.json
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Scroll.Tests/Doroti.Scroll.Tests.csproj -c Release -m:1 -nr:false
```

하네스에는 기존 `ScrollGesture.java`로 만든 shell 전용 `doroti-scroll.jar`가 필요하다.
원시 APK/JSON/XML/PNG/log/trace는 ignored `temp/galaxy-caret*`에 두고 152개 hash와
최종 소스 22개 hash를 결과 JSON에 보존했다. 이후 임시 산출물을 정리하면 raw 파일의
존재를 다시 확인해야 한다. 실제 손가락·표시/scanout·활성 TalkBack과 원래 work.md의
전체 스크롤 성능 합격은 여전히 `notVerified`/`PARTIAL`이다.
