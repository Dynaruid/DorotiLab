# Windows TextField 단축키와 긴 입력 클리핑 수정

## 원인 및 Flutter 비교

저장소의 Flutter `56b8e1a8` 소스를 기준으로 공통 프레임워크와 Windows 호스트를 수정했다. TestbedApp 전용 우회는 추가하지 않았다.

- `engine/src/flutter/shell/platform/windows/text_input_plugin.cc:KeyboardHook`: Flutter는 방향키, Backspace, Delete 등의 편집을 프레임워크가 맡는다. Doroti HWND는 활성 텍스트 클라이언트가 있으면 Backspace/Delete/Left/Right/Home/End를 가로채고 수정키를 무시한 자체 편집을 했다. 이제 down/up/repeat를 프레임워크로 전달하며, `WM_CHAR(Backspace)`는 중복 삭제하지 않는다. 일반 문자/Enter/IMM32 입력의 네이티브 전송은 유지한다.
- `packages/flutter/lib/src/painting/text_painter.dart:WordBoundary/_UntilTextBoundary`: Doroti의 세 경계 메서드가 `override` 대신 새 `virtual` 메서드였다. `TextBoundary`로 호출하면 기본 메서드들이 서로 재귀 호출해 Ctrl+단어 이동에서 stack overflow가 발생했다. 실제 override를 복구했다.
- `packages/flutter/lib/src/rendering/object.dart:RenderObject` 및 `rendering/editable.dart:paint`: Flutter는 처음부터 repaint boundary의 합성 상태를 반영한다. Doroti의 빈 생성자는 최초 합성 계산이 필요하지 않다고 표시해 커서/선택용 repaint boundary를 포함한 RenderEditable도 `needsCompositing=false`였다. 캔버스 클립을 기록한 picture가 자식 레이어에서 종료되어 뒤의 글자는 클립 밖에 그려졌다. 생성 시 합성 계산을 dirty로 설정해 attach 후 파생 객체가 완성된 상태에서 계산한다.

## 실패 기록

- 수정 전 마운트된 Ctrl+A/C/X/V의 controller 변경은 동작했다. 따라서 모든 단축키가 같은 원인으로 실패했다고 판정하지 않는다. 이전 단축키 매칭/클립보드 수정을 유지한다.
- 수정 전 긴 입력: `maxScrollExtent=2160.96875`, `clip=hardEdge`, `needsCompositing=false`. 240px 필드에서 화면 끝까지 텍스트가 새는 스냅샷을 확인했다. `.doroti/text-before5.log`, `.doroti/text-dump.log`, `.doroti/evidence/windows-sample-a477def5bfe94ff1be74a7cf661729ed/mounted/text-long.png`.
- 단어 경계 override 수정 전: `.doroti/text-fix2.err`에 `TextBoundary.getTextBoundaryAt/getLeadingTextBoundaryAt` stack overflow 보존.
- 초기 신규 fixture는 locale/text-input capability가 빠져 실패했다. fixture를 완성한 뒤 제품 경로를 검증했다.
- Ctrl+Y 검사는 실패했으나 pinned Flutter의 redo 키는 Ctrl+Shift+Z다. Flutter 키맵에 맞춰 테스트를 수정했으며 Ctrl+Y 지원을 주장하지 않는다.
- 기존 `--windows-sample` Debug 실행은 수정하지 않은 `FixtureContext`의 빈 Localizations delegate 목록 assertion에서 중단됐다 (`.doroti/text-windows-regression.err`). Release 검증 결과와 구분한다.

## 자동 검증

모든 테스트 프로세스는 1,200초 timeout을 사용했다.

- 마운트된 MaterialApp/TextField → 실제 키 이벤트/Focus/Shortcuts/EditableText 경로: Ctrl+A/C/X/V/Z, Ctrl+Shift+Z, Shift+Left, Ctrl+Home/Right, Backspace PASS. 네이티브 편집 상태 콜백을 통한 긴 입력 후 수평 스크롤 및 caret viewport 포함 PASS. 클립보드 OS 경계는 `ClipboardFixtureHost`를 사용하며 production Services/Actions는 그대로 실행한다.
- CPU/GPU raster: 긴 입력, 전체 선택, 끝으로 스크롤된 입력에서 viewport 바깥 픽셀 변화 0 및 내부 텍스트 표시 PASS. `.doroti/text-final2.log`, `.doroti/text-gpu.log`. GPU 스냅샷: `.doroti/evidence/windows-sample-966a5c6419c44c5fb31aac840eba5aa9/mounted/`.
- 전체 FCR-7 Debug PASS: `.doroti/text-fcr7.log`.
- 기존 Windows sample 전체 Release 회귀 검사 PASS: picker 포인터 조작, native text/shadow, 이미지 추출, drawer, 전체 SampleHome 양쪽 column/app-bar 스크롤. `.doroti/text-windows-release.log`. Debug fixture assertion 실패는 위 실패 기록에 보존했다.
- 실제 native HWND + Vulkan 제품 호스트 검사 PASS: 활성 텍스트 클라이언트에서도 여섯 편집 키의 down/up/논리 키 전달 및 기존 Unicode clipboard/IME 전송 계약 검사. `.doroti/text-native-report.json`, `.doroti/text-native-test.log`.
- TestbedApp Windows Release build PASS, 경고/오류 0: `.doroti/text-app-build.log`. native DLL과 앱 복사본 SHA-256 모두 `12b8fc6be50943fa543d4629700e28ebb4e3eed7dbf7cb4eeefd9a5071389245f`.

재실행: `dotnet run --project Doroti/validation/fcr7-material-widget -- --mounted-text`. GPU 모드는 `DOROTI_VALIDATION_GPU_RASTER=1`. OS 키 ingress 검사는 `dotnet run --project Doroti/validation/hwnd-exact-cpp-product -- --presenter Vulkan --no-resize-burst --lifecycle-cycles 0`.

## 검증 경계

사용자가 직접 조작한 Windows TestbedApp 화면, 물리 키보드, 한글 IME 후보창/조합 및 실제 디스플레이 acceptance는 `notVerified`다. HWND 자동 전송/마운트 위젯/CPU·GPU raster/빌드를 각각 분리하며 물리 검증으로 대체하지 않는다.
