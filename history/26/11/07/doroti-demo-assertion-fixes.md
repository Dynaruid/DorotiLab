# DorotiDemoApp assertion 수정

`samples/DorotiDemoApp`의 Windows Debug 실행에서 발생한 입력·보관 섹션 오류를 수정했다.
Components 페이지 사본의 동작을 바꾸지 않고 공용 Windows 호스트와 SectionList의 소유권 경로를 고쳤다.

## 원인과 수정

- Windows 호스트가 모든 클릭에 `pointer_identifier=1`을 보냈다. 이전 클릭의 gesture arena
  entry가 다음 클릭의 arena를 참조하면서 `GestureArenaManager._resolve`의 membership assertion이
  발생했다. 각 down/move/up 시퀀스에 새 ID를 배정하고, 같은 시퀀스의 드래그·해제는 ID를 유지한다.
  추가 버튼과 부분 해제는 move로 변환한다. ID 발급기는 창 사이에서도 공유한다.
- SectionList는 keep-alive 자식을 연결된 상태로 보관하면서 기존 상속 relayout boundary도 유지했다.
  자식을 측정하지 않고 부모만 layout을 끝낸 뒤 Image/InputDecorator가 다시 갱신되면
  `RenderObject.markNeedsLayout`의 boundary assertion이 발생했다. 보관할 때 상속 boundary를
  무효화하고, suspended owner의 연결 자식도 보관 버킷으로 옮긴다. 재표시 시 정상 layout이
  같은 자식의 boundary를 다시 설정한다.
- 독립 스크롤로 숨겨진 목록의 GlobalKey 섹션을 가져오면 이전 목록의 투명 wrapper/슬롯이
  정리되지 않아 `SliverMultiBoxAdaptorElement.removeRenderObjectChild` assertion이 발생했다.
  suspended SectionList만 명시적인 인계 경로를 사용해 이전 wrapper와 슬롯을 함께 정리한다.
  일반 GlobalKey 재활성화가 기존 State를 새 소유자로 옮긴다.
- 보관 자식을 제거하는 `dropChild`의 일괄 layout 요청은 이미 깨끗한 다른 viewport를
  layout 중 변경하는 오류를 만들었다. suspended SectionList의 보관 버킷은 layout/paint에
  참여하지 않고 extent도 고정되어 있으므로 이 제거만 layout 요청에서 제외한다.
  실제 detach, compositing 및 semantics 갱신과 일반 자식 제거의 layout 요청은 유지한다.

## 검증

모든 테스트 실행에 저장소 규칙인 1,200초 timeout을 적용했다.

```powershell
& 'C:/Program Files/LLVM/bin/clang++.exe' -std=c++20 -Wall -Wextra -Werror packages/platforms/windowsappsdk/native/tests/pointer_sequence_test.cpp -o temp/doroti-demo-assertions/pointer-sequence-test.exe
python Doroti/eng/run-with-timeout.py --timeout 1200 temp/doroti-demo-assertions/pointer-sequence-test.exe
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Scroll.Tests/Doroti.Scroll.Tests.csproj -c Debug -- --cached-sections
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Scroll.Tests/Doroti.Scroll.Tests.csproj -c Release -m:1 -nr:false
pwsh -NoProfile -File packages/platforms/windowsappsdk/eng/build-hwnd-exact-cpp-native.ps1
dotnet build samples/DorotiDemoApp/windowsappsdk/DorotiDemoApp.WindowsAppSdk.csproj -c Debug -m:1 -nr:false --nologo
dotnet build samples/DorotiDemoApp/windowsappsdk/DorotiDemoApp.WindowsAppSdk.csproj -c Release -m:1 -nr:false --nologo
```

- Native 회귀 검사 PASS: 20회 좌/우 클릭, 드래그·해제 ID, 복수 버튼, 취소·재진입, 여러 창의 ID.
- Debug 보관 섹션 검사 PASS: 실제 PipelineOwner layout, 숨김 중 자식 갱신, 동일 attachment 복원,
  정확한 eviction, 독립 viewport의 GlobalKey 인계와 State 값 유지, 최종 한 번의 dispose.
  인계 검사는 수정 전 슬롯 assertion과 clean sibling layout 변경 오류를 각각 재현했다.
- Release 전체 Scroll.Tests PASS. 최종 Windows Debug·Release 빌드 모두 경고 0 / 오류 0.
- Windows Debug 창에서 OS 입력 자동화와 화면 관찰로 다음 경로를 확인했다.
  폭 900에서 입력창 표시 → `verified` 입력 → 최대화하여 두 열 → 테마 변경 → 폭 복원하여
  입력창 숨김 → 숨김 상태 테마 변경 → 독립 스크롤로 입력창 재표시 → 값 유지 확인 →
  우클릭 Paste/Select all 메뉴 → 두 열 재전환. 로컬 이미지도 정상 표시됐다.
  이 경로에서 first-chance assertion 기록과 framework error 출력이 없었다.
- 임시 first-chance 진단 코드와 테스트용 초기 폭 환경변수 처리를 제거했다.
  기본 1280×900 Debug 앱을 다시 실행해 갤러리 표시 및 stdout/stderr 오류 없음도 확인했다.

최종 Debug/Release 러너의 Rendering·Widgets DLL 및 native host를 각 빌드 원본과 SHA256으로
대조했다. 두 러너의 native host SHA256은
`9A25270736CFB9189BCBDFE20ED2613812BD0D840EB7B19D22F85A774A6DFC7F`이다.

검증 범위는 Windows Debug UI 기능과 콘솔 회귀 검사다. 물리 터치패드, Android/iOS 등 다른
플랫폼의 실기기 동작 및 프레임 표시 성능의 합격 판정은 이 기록의 범위에 포함하지 않는다.
원시 진단 로그는 무시되는 `temp/doroti-demo-assertions`의 로컬 산출물이다.
