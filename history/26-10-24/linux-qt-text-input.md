# Linux Qt 텍스트 입력 R4 수정 및 검증

날짜: 2026-10-24 (Asia/Seoul)

상태: **R4 구현 및 Qt 6.12.0 Release Quick/QWindow·xcb/Wayland native query 자동 검사 완료. 실제 ibus/fcitx·managed 전체 왕복·화면 성능은 notVerified.**

## 범위와 수정

- 시작 HEAD: `74bdd0f1b439d27e88b5df6a572c387fe40be7ec`, 시작 작업 트리 깨끗함.
- 시작 `memo.md` SHA-256: `c3df1c2b6fbd8d8916206f25f21bf35a26e09c272715cd38ffd74e2f99812093`.
- [memo.md](../../memo.md)의 Linux Qt R4, 관련 native 검사와 검증 문서를 변경했다. `work.md`는 변경하지 않았다.
- 빌드·테스트는 `Doroti/eng/run-with-timeout.py --timeout 1200`으로 제한했다. 동일 입력의 수백 회 반복은 수행하지 않았다.

[Qt host](../../packages/platforms/qt/native/src/doroti_qt_host.cpp)의 framework 편집 상태는 preedit가 포함된 표시 문서다. 수정 전 IME query도 이 문자열과 표시 커서를 반환했다. 수정 후 `ImSurroundingText`는 preedit를 제외하고 `ImCursorPosition`·`ImAnchorPosition`은 같은 committed 문서의 UTF-16 offset을 반환한다. 조합 안의 위치는 삽입점으로 모으고 뒤쪽 위치는 조합 길이만큼 뺀다. `ImCurrentSelection`도 변환된 양 끝점과 committed 문자열로 계산한다. 선택이 조합 안에서만 움직이면 IME 선택은 빈 문자열이다.

예를 들어 표시 상태가 `a你好b`, 조합 `[1,3)`, 커서 2라면 query는 surrounding `ab`, cursor/anchor 1, 빈 선택을 반환한다. framework callback에는 표시 문자열·커서 2·전체 조합 범위를 계속 보낸다. 실제 caret rectangle은 표시 좌표를 유지한다. ABI나 queued state 프로토콜은 바꾸지 않았다.

계약 근거: [Qt 6.12 InputMethodQuery](https://doc.qt.io/qt-6/qt.html#InputMethodQuery-enum), [QInputMethodEvent의 Cursor·Selection·replacement](https://doc.qt.io/qt-6/qinputmethodevent.html).

## 자동 검사

[native driver](../../Doroti/tests/native/qt_input_contract.cpp)는 실제 `QInputMethodQueryEvent`를 Doroti와 QLineEdit/QTextEdit에 전달해 surrounding text·cursor·anchor·현재 선택을 비교한다. 각 기본 입력창당 29개 상태, 실행당 58개 비교다.

- preedit 내부 커서 시작/중간/끝 및 문자열이 같은 cursor-only 갱신.
- 조합 취소·재시작·확정, 양방향 선택 교체 후 조합과 취소.
- 이전 조합 양옆의 committed 문자를 교체하고 새 preedit를 시작한 뒤 확정.
- surrounding/preedit의 emoji와 UTF-16 offset.
- 역방향 IME 선택과 commit 이후 Selection 속성.
- managed 상태에서 조합 뒤/안/양쪽을 가로지르는 정방향·역방향 선택.

framework callback의 표시 텍스트·선택·전체 조합 범위도 별도로 확인한다. 기존 selection-only·commit/replacement/cancel·key repeat·scroll direction·application identity 검사는 유지했다. driver는 입력 검사 후 의도적으로 `DOROTI_QT_ERROR_MANAGED_FATAL`을 반환해 창 표시와 GPU 제출을 피한다. 이 반환은 runner가 기대하는 검사 완료 상태이며 실제 앱의 정상 종료 검증은 아니다.

| 검사 | 결과 |
| --- | --- |
| 수정 전 Qt 6.12 Quick / xcb | 기본 입력창과 22개 query 상태 불일치. 이후 추가한 managed 선택 8개 비교는 이 수정 전 실행에 포함되지 않음. |
| 수정 후 Release Quick / xcb | current·legacy 각각 58개 비교 및 기존 입력 검사 PASS |
| 수정 후 Release Quick / Wayland | current·legacy 각각 58개 비교 및 기존 입력 검사 PASS |
| 수정 후 Release QWindow / xcb | current·legacy 각각 58개 비교 및 기존 입력 검사 PASS |
| 수정 후 Release QWindow / Wayland | current·legacy 각각 58개 비교 및 기존 입력 검사 PASS |

환경은 Ubuntu 26.04.1 LTS / x86_64, GCC 15.2.0, 선택 Qt SDK 6.12.0이다. native 빌드는 `Release`, `-O3 -DNDEBUG`, C++20, Graphite=ON, WebEngine=OFF이며 Quick만 ON/OFF로 구분했다. 시스템 Qt 6.10.2가 아닌 선택 SDK의 headers·라이브러리·QPA 플러그인으로 검사했다. xcb는 현재 Wayland 세션의 XWayland이며 독립 Xorg 검증으로 일반화하지 않는다.

초기 실행기의 PIC 없는 executable은 Qt global copy relocation으로 QApplication 초기화 중 충돌했다. [runner](../../Doroti/tests/linux_qt_input.py)에 `-fPIC`를 추가한 뒤 수정 전 실패를 확인했다. QWindow 빌드에는 기존 accessibility 코드가 사용하는 `QPointer`의 직접 include가 빠져 있었으므로 추가했다. 최종 소스로 두 백엔드를 다시 빌드하고 위 8개 실행을 모두 완료했다. 초기 충돌·QWindow 컴파일 실패·그에 따른 library 부재 실행은 최종 입력 PASS에 포함하지 않는다.

입력 로그의 host portal app ID 등록 경고는 테스트 ID의 desktop 파일이 설치되지 않은 데 따른 것이다. 자동 입력 검사는 모두 정상 종료 코드 0으로 완료됐다.

## 재실행

저장소 루트에서 실행한다. `qtSdk`는 Qt 6.12.0 SDK 위치로, `qtEvidence`는 매번 새로운 `temp/testing/` 하위 경로로 지정한다. runner는 이미 있는 output 디렉터리를 거절한다. `--qpa wayland`는 Wayland 세션, `--qpa xcb`는 연결 가능한 X 디스플레이가 필요하다. PKG_CONFIG_PATH와 LD_LIBRARY_PATH는 같은 SDK를 가리켜야 한다.

```sh
qtSdk="$PWD/temp/qt612-sdk/6.12.0/gcc_64"
qtEvidence="temp/testing/linux-qt-text-input-rerun"
for qtProfile in quick widgets; do
  qtQuick=ON
  if [ "$qtProfile" = widgets ]; then qtQuick=OFF; fi
  python3 Doroti/eng/run-with-timeout.py --timeout 1200 cmake \
    -S packages/platforms/qt/native -B "$qtEvidence/native-$qtProfile" \
    -DCMAKE_BUILD_TYPE=Release -DDOROTI_QT_SDK="$qtSdk" \
    -DDOROTI_QT_VERSION=6.12.0 -DDOROTI_QT_QUICK="$qtQuick" \
    -DDOROTI_QT_GRAPHITE=ON -DDOROTI_QT_WEBENGINE=OFF || exit
  python3 Doroti/eng/run-with-timeout.py --timeout 1200 cmake \
    --build "$qtEvidence/native-$qtProfile" --parallel 4 || exit
  for qtQpa in xcb wayland; do
    PKG_CONFIG_PATH="$qtSdk/lib/pkgconfig" LD_LIBRARY_PATH="$qtSdk/lib" \
      python3 Doroti/eng/run-with-timeout.py --timeout 1200 \
      python3 Doroti/tests/linux_qt_input.py \
      --library "$qtEvidence/native-$qtProfile/libdoroti_qt_host.so" \
      --output "$qtEvidence/$qtProfile-$qtQpa" --qpa "$qtQpa" || exit
  done
done
```

최종 원시 결과는 삭제 가능한 `temp/testing/linux-qt-text-input/{quick,widgets}-{xcb,wayland}-final/`의 `input.log`와 `build.log`다. native 빌드는 같은 상위 경로의 `native-qt612/`·`native-widgets-qt612/`에 있다. 수정 전 실패는 `before-pic/input.log`, 최종 native 빌드 로그는 `build-qt612-final.log`·`build-widgets-qt612-fixed.log`에 남겼다. 원시 파일을 삭제해도 아래 해시·결과·재실행 명령은 이 문서에 보존된다.

## 검증 입력 해시 (SHA-256)

| 대상 | 해시 |
| --- | --- |
| 수정 전 doroti_qt_host.cpp | `c73e135ee2eb8a55243b93adb79347ba408d09426bad9cf09622e3d44487f0c8` |
| 최종 doroti_qt_host.cpp | `2d11ac9565d87743ba952ca3fdbe4301b532c640096af20c89d5074269e71658` |
| qt_input_contract.cpp | `9870bee91c413aa790d1b4af21d5b17a67db45cfdb467c565f7444c7aa94e003` |
| linux_qt_input.py | `079d971aa707a7380e0e508181b3f25779a221918cc6115088a5dc9c7619457c` |
| 최종 Release Quick libdoroti_qt_host.so | `1ad738fa32e436fa96a47c154fdf925b30b67404a9b15cf357fb35c39b8b3f64` |
| 최종 Release QWindow libdoroti_qt_host.so | `dab91420e474ded0880a12c99d511ed0731ff583cef9d4f2e57c76734c4cc577` |

## 남은 검증

이 검사는 Qt native 합성 이벤트와 기본 입력창의 비교다. 실제 ibus/fcitx 엔진을 구동하거나 물리 키보드로 입력하지 않았다. 현재 세션에는 fcitx5가 있지만 선택 Qt 6.12 SDK에는 fcitx input-context 플러그인이 없고 ibus-daemon도 설치되어 있지 않다.

실제 한/일/중 IME의 취소·내부 커서·reconversion, managed framework/formatter 전체 왕복과 지연 응답·client 전환, 붙여넣기, 입력→화면 표시 지연·FPS·프레임 페이싱, 물리 GPU·독립 Xorg는 별도 검증 대상이다. 이번 변경으로 이 항목들이 통과했다고 표시하지 않는다.
