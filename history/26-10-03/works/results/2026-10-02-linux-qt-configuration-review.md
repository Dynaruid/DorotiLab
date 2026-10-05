# Linux Qt 구성 검토·보완 — 2026-10-02

**구성 보완 및 명시한 자동 검증 PASS. 플랫폼 전체 인수 PARTIAL.**
요청 범위는 [work5 요약](../../../26-10-02/work5-summary.md)의 Linux Qt 구성이다. 이전 [프레임 pipeline 결과](2026-10-02-native-frame-pipeline-linux.json)는 이전 payload의 기록으로 보존했다. 새 source/payload identity와 결과는 [JSON](2026-10-02-linux-qt-configuration-review.json), 원본은 `temp/testing/linux-qt-config/`에 있다.

## 변경

| 발견한 문제 | 반영한 구성 |
| --- | --- |
| SDK·템플릿·CMake는 Widgets, 샘플은 Quick 기본값 | 모두 Quick/Graphite 기본값. WebEngine은 SDK/템플릿에서 optional, 샘플에서 활성화 |
| 옵션 변경 시 native cache 공유 | Configuration 및 Quick/Graphite/WebEngine/GStreamer 조합별 디렉터리 |
| native host가 없으면 복사를 건너뜀 | `DOROTIQT006`으로 build/publish 실패. 다른 옵션의 미생성 캐시로 `publish --no-build`도 실패 |
| 오타나 지원하지 않는 조합을 늦게 발견 | boolean 검증, Quick→Graphite 및 WebEngine/Desktop→Quick 조건을 MSBuild에서 검사 |
| OpenGL 선택 시 CMake Graphite 옵션 전달 없음 | `DorotiQtGraphite=false` 연결. Quick/WebEngine 비활성화 및 실행 환경변수 조건 명시 |
| 문서가 정상 C에서도 매 프레임 queue drain을 요구 | producer/copy fence와 Qt consumer fence 설명으로 수정. native/resize/실패/종료 동기화 유지 |
| Testbed WebEngine 최소 Qt 버전 설명 불일치 | WebEngine 포함 6.8+, 제외 시 Quick 6.6+. 의존성·QML·QPA 조건 정리 |

관리 코드는 기존 공통 C 정책과 Qt ABI 6/208바이트를 유지한다. 설정은 [Doroti.Qt.targets](../../../../packages/platforms/build/Doroti.Qt.targets), 옵션 표는 [Linux Qt 계약](../../../../Doroti/docs/platform-views/linux-qt.md#build-configuration)에 있다. 세 native source/resource/documentation 복사본이 동일한지 검사했다.

## 실행 결과

Ubuntu 26.04, .NET SDK 10.0.400/runtime 10.0.11, Qt 6.10.2, KDE Wayland/xcb, Mesa 26.0.8 llvmpipe Vulkan 1.4.335. 하드웨어 GPU 검증으로 확대하지 않는다.

| 검사 | 결과와 범위 |
| --- | --- |
| MSBuild 구성·복사 회귀 | SDK/샘플/템플릿 기본값, 오류 조합, 옵션별 캐시, 동일 크기/mtime의 다른 library 교체, disabled shim 삭제, prebuilt 및 enabled shim 누락 거절 PASS |
| 빌드 | Testbed Debug + Desktop, Sample2 Release Quick/WebEngine, 별도 OpenGL native build PASS; 경고/오류 0 |
| CPU | 공통 C/A/B·GPU full 준비·native/replay·역순 완료, widget/입력/semantics/수명 회귀 PASS |
| Vulkan fixture | 31조건 A/C 픽셀 동일, 두 unfinished recording 격리, 셋째 admission 거절 PASS; 최대 2, consumer 124/124. 실제 Qt sampling 대신 별도 fixture marker 사용 |
| Wayland Testbed | 두 창/두 lifetime, desktop close, services/파일 선택·drop, navigation/restore/crash/corrupt fallback, editor/WebView 재생성, 20회 resize PASS |
| 확대 배율 | `QT_SCALE_FACTOR=1.5`; 실측 DPR 2.25에서 Wayland 입력/20 resize PASS. 기본 실행 실측 DPR 1.5. 동적 디스플레이 전환 아님 |
| xcb | Quick 입력/20 resize PASS. Vulkan validation layer가 없어 VUID 검증이나 기존 WSI 경쟁 조건 해결을 주장하지 않음 |
| Sample2 Fast σ20 | default→C, 명시 A/B/C 각각 8초 PASS. default C에서 pending 최대 2; consumer 제출/완료 86/86. A/B 상한 1 |
| OpenGL/xcb | 새 MSBuild Graphite=false 옵션으로 native build, 별도 payload에서 Off serial smoke PASS. 전역 선택값 C와 실제 serial capability를 구분 |
| NuGet-only 템플릿 | 새 버전의 local feed·격리 NuGet cache, ProjectReference 없이 Release restore/publish. Quick override 없이 default C, 20 resize 및 consumer 64/64 PASS |
| 배포·게시 | 같은 구성 `publish --no-build` native hash 일치; 다른 미생성 구성 거절; trimming 거절; portable install/run/update·변조 거절·userdata 유지 삭제 PASS |
| 정합 | 유지 중인 Source audit, 수정한 Qt 계약 문서의 로컬 링크, native 복사본 정합 및 `git diff --check` PASS |

정상 종료 Quick summary마다 consumer 제출/완료 수가 일치하고 reserved bytes/retiring layers가 0이며 failed frame이 0이었다. Sample2에서 두 pending 프레임을 관찰한 사실을 실제 GPU 실행 overlap 또는 화면 FPS로 해석하지 않는다. `frameSwapped`와 `afterFrameEnd`의 의미는 [Qt 공식 API](https://doc.qt.io/qt-6/qquickwindow.html#afterFrameEnd)와 [기존 수명 계약](../../../../Doroti/docs/native-frame-pipeline.md)에 따른다.

첫 GPU fixture 실행은 RID 없는 CPU build를 사용해 executable 옆 `libSkiaSharp.so`가 없어서 시작 단계에서 실패했다. 원본 `logs/linux-qt-gpu.log`를 보존했고 `-r linux-x64`로 다시 빌드·실행하여 통과했다. [검사 문서](../../../../Doroti/tests/README.md)에 RID 조건을 추가했다.

## 재실행

저장소 루트에서 각 출력 경로를 새 디렉터리로 지정한다.

```sh
python3 Doroti/eng/run-with-timeout.py python3 Doroti/tests/linux_qt_build_profiles.py --output temp/testing/linux-qt/review-profiles
python3 Doroti/eng/run-with-timeout.py python3 Doroti/tests/linux_qt_smoke.py --qpa wayland --output temp/testing/linux-qt/review-wayland
python3 Doroti/eng/run-with-timeout.py python3 Doroti/tests/linux_qt_packages.py temp/testing/linux-qt/review-packages
python3 Doroti/eng/run-with-timeout.py dotnet run --project Doroti/tests/Doroti.Tests -c Debug -r linux-x64 -- --native-frame-gpu temp/testing/linux-qt/review-gpu.json
```

`LinuxSmoke`에 구성 회귀와 consumer 종료 검사도 연결했다. Qt system development/QML/WebEngine 의존성이 필요하며 package-only 실행도 clean OS 검증을 대신하지 않는다.

## 남은 인수

물리 GPU/device loss, 실제 표시 FPS·input→display·hardware overlap 및 30회 성능 비교, 물리 IME/Orca, 동적 DPR/디스플레이 이동·최소화/복원·지속 복귀, 10분 사용, clean OS/서명·OS protocol 등록은 미검증이다. 기존 Qt/xcb WSI 경쟁 조건과 OpenGL Wayland 실패는 해결 완료로 바꾸지 않았다. 직접 지정한 prebuilt directory의 옵션·ABI 일치는 제공자가 보장해야 하며, 현재 검사는 그 디렉터리의 artifact 존재와 ABI 런타임 검사를 사용한다.
