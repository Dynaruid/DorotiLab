# 접근성 트리의 위치 증분 갱신 요약

원본 `work3.md` · 작성·최종 검증·보관 2026-11-26 KST · 초기 소스 검토 `34a26d8c2de1`

**PARTIAL.** W3-01~05를 구현했다. 위치만 이동한 경우 기존 접근성 노드와 포커스·읽기 순서·액션을 재사용하고 바뀐 좌표를 갱신한다. 합성 부하에서 공통 투영·전송 비용이 줄었고, 실제 앱의 단계별 비용과 프레임 간격은 플랫폼 및 보조기술 조건에 따라 달랐다.

## 원인과 구현

프레임워크는 dirty 노드만 보냈지만 렌더러가 좌표 투영·정렬 후 전체 스냅샷을 전달했다. Windows App SDK·Qt는 이를 받아 트리 자료구조를 재구성했고 AppKit·Catalyst는 접근성 속성과 배열을 다시 설정했다. Qt 통합 제목 표시줄은 최종 트리에 네이티브 버튼을 다시 붙이기 전에 변화를 비교해 불필요한 `ObjectReorder`를 만들었다. 11월 24일 iOS 수정 `fb07c525`에서 좌표 갱신과 접근성 속성 갱신을 구분한 방식을 다른 호스트에도 적용했다.

| 작업 | 반영한 계약 |
| --- | --- |
| W3-01 · MAUI·Apple | 노출 계층·액션 연결을 재사용한다. 노드와 액션 소유권을 갱신하고 부모 원점의 영향을 받는 상대 좌표만 비교한다. Apple의 속성·자식·액션 배열은 관련 값·핸들·순서가 바뀔 때 갱신한다. [MAUI 브리지](../../../../packages/platforms/maui/Doroti.Host.Maui/MauiSemanticsBridge.cs) |
| W3-02 · Windows App SDK·Qt | 기존 네이티브 객체에 위치와 필드를 반영한다. Windows UIA는 갱신된 트리를 게시한 뒤 잠금 밖에서 알림을 보내고 실제 구조·역할·패턴 변경을 통지한다. Qt는 JSON을 한 번 파싱하고 제목 표시줄을 포함한 최종 트리를 비교한다. 소비자 활성 여부와 관계없이 현재 노드 조회·액션을 지원한다. [Windows 네이티브 회귀](../../../../packages/platforms/windowsappsdk/native/tests/README.md#windows-accessibility-events-regression) · [Qt 네이티브 호스트](../../../../packages/platforms/qt/native/src/doroti_qt_host.cpp) |
| W3-03 · 공통 투영·전송 | view별 원본 트리와 투영을 보존하고 위치·변환·클립의 영향을 받는 노드만 다시 계산한다. Windows·Qt는 지원되는 ABI에 순번이 있는 필드 패치를 보내고 기존 ABI에는 전체 스냅샷을 보낸다. 첫 연결·`Clear`·기준 순번 불일치는 전체 스냅샷으로 복구하고 늦은 패킷은 거부한다. |
| W3-04 · Android | 가상 노드의 ID·순서·포커스를 재사용하고 단일 노드 변경은 그 노드만 통지한다. 오버레이 delegate를 핸들에 연결해 유지·해제한다. 안전 영역의 중복 적용을 고쳤으며 최신 선택 범위와 허용 스크롤 액션을 제공한다. 암호 노드의 선택 오프셋은 비노출 상태다. |
| W3-05 · Web | 변경 목록으로 DOM을 갱신하고 기존 요소·리스너·포커스를 유지한다. 스크롤 오프셋만 바뀌면 해당 값만 보낸다. 순번 불일치는 전체 상태로 복구한다. |

## 동작 검증

- 공통 Release 회귀에서 전체 투영과의 노드별 동등성, 한 노드 이동, 스크롤 병합, 숨김 조상, 두 view의 순번, 늦은 패킷, `Clear`, RTL·ZWJ·선택·암호 범위, Windows MAUI 자식 캐시를 확인했다. iOS·Catalyst·AppKit의 MAUI 호스트는 빌드했다. [공통 회귀 안내](../../../../Doroti/tests/Doroti.Scroll.Tests/README.md)
- Windows 192 DPI에서 기본 Graphite와 Ganesh로 App SDK Demo 31개, MAUI Testbed 43개 노드의 기존 Runtime ID와 변경된 bounds를 조회했다. App SDK의 `FromPoint`, ZWJ `TextPattern`의 범위·사각형·캐럿, 구버전 네이티브 DLL 조합도 확인했다. Narrator로 최대화 뒤 App SDK 테마 버튼을 실행했다. MAUI에서는 Components와 Color를 열고 최대화한 뒤 새 위치의 Components를 다시 열었다.
- Qt 6.12 WSL/Xvfb에서는 통합·기본 제목 표시줄의 AT-SPI 조회와 스크롤 좌표를 확인했다. 위치 패치와 전체 재동기화의 `ObjectReorder`는 0건이었고 `Clear` 뒤에도 닫기 객체 ID가 유지됐다. 통합 제목 표시줄의 액션과 로컬라이제이션 Demo의 문자 사각형·캐럿 이동을 확인했다. [Qt 텍스트 검증 환경](../../../../Doroti/docs/localization/paragraph-feasibility.md#linux-qt-demo)
- Android x64 에뮬레이터에서 스크롤 뒤 가상 노드 19개와 MAUI 오버레이 노드 17개의 새 bounds를 조회했다. 두 경로에서 `Week`를 선택했고 TalkBack을 켠 에뮬레이터에서 강조 사각형과 두 번 탭 동작을 확인했다. 갤럭시 SM-S931N ARM64에서는 삼성 키보드로 `한글`을 조합·확정하고 두 경로에서 `UiAutomation` 선택 `1:5`, 캐럿 `3:3` 및 TalkBack 연결 상태의 포커스·액션을 검증했다. 오버레이의 접근성 스크롤과 역방향 액션도 확인했다. 오버레이 APK는 네이티브 플랫폼 뷰 선언을 제외한 테스트 매니페스트를 썼다.
- Chromium Debug Mono의 threaded·compatible에서 Demo 리사이즈와 순번 불일치 복구 후 같은 DOM 버튼·포커스와 접근성 역할을 확인했다. Testbed에서 브라우저 IME 한글 삽입, 리사이즈 뒤 입력 요소 유지, Backspace 및 두 앱의 브라우저 smoke가 통과했다. [Web smoke 실행기](../../../../packages/platforms/web/tests/run-browser-tests.ps1)

## 전후 측정

App SDK·Qt·Web·갤럭시 비교 기준은 `4c349c45`였고 Web 변경본은 `7fd291b6`이었다. 프레임 간격 표본에는 스크립트 입력 간격이 포함됐다.

| 환경 | 주요 변화와 범위 |
| --- | --- |
| Windows Release 합성 · 노드 2,048개 중 1개 이동 | 투영 p95 22,460.10→56.70µs, 인코딩 p95 19,126.30→126.80µs, 마지막 패킷 846,619→60바이트. 보조기술 없이 준비 5회 뒤 20회 측정했다. |
| Windows App SDK Demo 리사이즈 | Narrator 연결 실행의 JSON 파싱 p95 2,141.9→495.4µs, 네이티브 반영 할당 p95 126,551→0바이트였다. 두 실행의 패킷 합계는 각각 777,865→53,545바이트였다. 실제 두 열→한 열 전환에서만 구조 알림 1건이 있었다. 소비자 조회는 두 조건 모두 `true`였고 프레임 개선은 관측되지 않았다. |
| Qt Testbed 리사이즈 · Qt 6.12/WSL/Xvfb/llvmpipe | AT-SPI 연결 시 JSON 파싱 p95 956.20→154.19µs, 반영 p95 674.83→247.80µs, 전송량 828,015→69,213바이트, `frameSwapped` 간격 p95 89.59→81.92ms였다. Qt `ObjectReorder` 요청 20→0건, AT-SPI 클라이언트 수신은 양쪽 0건이었다. 미연결 실행에서도 파싱·반영 비용이 감소했다. |
| Web Demo 스크롤 · headless Chromium/SwiftShader | 양 모드에서 오프셋 패킷이 약 9.1% 작아졌다. 캔버스 commit 간격 p95는 threaded 209.960→218.545ms, compatible 214.400→207.600ms였다. |
| Windows MAUI Testbed 리사이즈 · 192 DPI/Ganesh DXGI | 브리지 누적 적용 7.118→3.713ms, 첫 present 응답 간격 p95 247.51→247.89ms. 적용 18회에서 새 peer와 구조 갱신이 0건이었다. |
| 갤럭시 ARM64 Testbed 스크롤 · Vulkan/오버레이 | TalkBack 연결 유무를 바꾼 네 경우에서 공통 투영 할당이 약 26% 감소했다. Vulkan 연결 시 네이티브 반영 20.52→27.11ms 및 관리 할당 125,624→200,504바이트, 미연결 오버레이 p95 19.10→38.89ms로 증가했다. 연결 오버레이 p95는 62.66→21.86ms로 감소했다. |

## 남은 수락과 재현

Apple 네이티브 객체·자식 및 액션 배열 동일성, VoiceOver·iOS Switch Control과 Apple 단계별 계측이 남았다. Windows App SDK는 `UiaClientsAreListening()`이 `false`인 세션의 비교가 필요하다. 갤럭시 TalkBack의 직접 탐색·발화와 Vulkan 연결 및 오버레이 미연결에서 늘어난 반영 비용도 후속 확인 범위다.

공통 Release 회귀는 저장소 루트에서 재현한다. 합성 측정은 같은 명령에 `DOROTI_MEASURE_SEMANTICS=1`을 환경 변수로 지정한다. Qt 회귀는 네이티브 빌드의 `DOROTI_QT_SEMANTICS_TESTS=ON`을 사용한다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Scroll.Tests/Doroti.Scroll.Tests.csproj -c Release -m:1 -nr:false
```

계측 스크립트와 원시 표본은 `Doroti/artifacts/work3-probes`, `work3-native-profile`, `work3-resume` 아래의 삭제 가능한 로컬 산출물이다. Web 기준·변경 게시의 상세 재현 조건을 포함한 원문은 `git show a9a524647d276f89d8871128c2e3e5accd1f3337:work3.md`로 조회할 수 있다.
