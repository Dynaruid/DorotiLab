# Qt 6.12 번들·WebEngine 분리 배포 요약

- 작업일: 2026-10-19. 보관일: 2026-10-27 KST.
- 원본: 루트 `work.md`의 9~11절 및 최종 상태. 원본 SHA-256: `496316a00f30978758d3d567d1d560224bdfb4905ca8c0a7319f0ac91ccc9c94`.
- 상태: **CLOSED / ARCHIVED · Qt 6.12 전체 배포 승인 PARTIAL**. 수집·로더·패키징과 계약 검사를 구현했고 실제 Testbed 동봉 `.deb` 생성까지 확인했다. 실제 앱 설치·Wayland/xcb·lifecycle 검증과 재배포 자료 검토는 미완료다.
- 선행 [10월 18일 시스템 Qt `.deb` 기록](../18/linux-deb-support-summary.md)의 Qt 6.8.2/xcb PASS·Wayland FAIL 및 Qt 6.10.2/Wayland PASS를 새 조합의 승인으로 사용하지 않는다.

사용자 요청으로 결정·검증·남은 항목을 보존하고 루트 `work.md`를 삭제했다. 아래는 당시 실행 결과이며 이번 보관에서 제품 빌드·설치·GUI 검증이나 외부 공급 정보 재조사를 수행하지 않았다. 이 문서는 활성 작업계획이 아니다.

## 최종 배포 계약

Qt Framework 6.12.0을 앱 전용 공유 라이브러리로 동봉한다. WebEngine은 독립 버전·출처·해시·ABI를 기록한다. 당시 확보한 SDK의 CMake 숫자 버전은 6.140.0이지만 공급 이름은 **`Qt WebEngine 6.140.0-rc`**였다. 정식 릴리스 확보나 배포 승인으로 표시하지 않는다.

| 배포 방식 | 최종 계약 |
| --- | --- |
| 폴더·portable / `bundled` | Framework와 활성 WebEngine을 앱에 동봉 |
| `.deb` / `bundled` (기본) | 앱 `.deb` 하나에 Framework·WebEngine 포함 |
| `.deb` / `download` | Framework는 앱에 동봉하고 정확한 버전의 별도 엔진 `.deb`를 APT 의존성으로 설치 |
| 폴더·portable / `download` | 게시 전에 거부 |

사용자는 자체 APT 저장소 운영을 필수 범위에서 제외했다. 분리 배포도 `apt install ./engine.deb ./app.deb`로 설치할 수 있다. 원격 엔진 다운로드는 공급자가 기존 서명 저장소를 제공하고 사용자가 사전 등록한 경우에만 가능하다. 앱 실행 중 다운로드·중첩 APT·시스템 Qt fallback·sandbox 해제를 추가하지 않았다. 공유 엔진의 제거는 의존성과 APT 정책을 따르며 앱 제거 스크립트가 직접 삭제하지 않는다.

빌더는 명시적으로 설치한 Framework SDK와 활성 WebEngine SDK를 사용한다. 게시 번들은 시스템 Qt 6.8/6.10을 거부하고 개발 빌드는 기존 시스템 Qt 경로를 유지한다. 새 번들 게시는 single-file·GStreamer·non-Quick를 거부한다. Wayland 세션은 native Wayland, X11은 xcb를 선택하며 명시적 `QT_QPA_PLATFORM`을 보존한다.

주요 설정은 `DorotiQtVersion`, `DorotiQtSdkDirectory`, `DorotiQtWebEngine`, `DorotiQtWebEngineVersion`, `DorotiQtWebEngineSdkDirectory`, `DorotiQtWebEngineDeployment`다. 분리 배포는 `DorotiQtWebEnginePackageId`/`DorotiQtWebEnginePackageVersion`도 필요하다. 공급자 NuGet 버전은 `DorotiQtProviderVersion`으로 분리했다. 구체적 설치·게시 명령은 [런타임 가이드](../../../../packages/platforms/qt/qt-runtime.md)에 남긴다.

## 구현과 소유권

| 구성 | 구현한 역할 |
| --- | --- |
| [번들 수집기](../../../../packages/platforms/build/qt_runtime.py) | 독립 버전·SDK·prebuilt ABI/기능/원본 shim 해시, ELF/QML 의존성, 명시적 inventory의 고지·대응 소스·허용 모듈 검증. 선택 파일·상대 RPATH·SONAME·`qt.conf`·runtime manifest 생성 |
| [SDK 설치기](../../../../packages/platforms/build/install-qt-sdk.py) / [review 도구](../../../../packages/platforms/build/review-qt-runtime.py) | 공식 URL·SHA-256 lock에 따른 SDK 설치와 recipe 기반 파일 inventory 생성. 프로젝트 평가에서 다운로드하지 않음 |
| [MSBuild targets](../../../../packages/platforms/build/Doroti.Qt.targets) | 옵션 조합·SDK CMake 인수·버전별 출력·publish 수집. 엔진 비활성 앱은 엔진 SDK 불필요 |
| [native CMake](../../../../packages/platforms/qt/native/CMakeLists.txt) / [라이선스 정책](../../../../packages/platforms/qt/native/cmake/DorotiLicensePolicy.cmake) | 선택 SDK의 exact Qt/WebEngine 링크·imported target prefix·공유 링크 검사, 버전·ABI·기능·shim 해시를 native manifest에 기록 |
| [QtRuntime](../../../../packages/platforms/qt/Doroti.Host.Qt/QtRuntime.cs) / [runner](../../../../packages/platforms/qt/Doroti.Host.Qt/DorotiQtRunner.cs) | 앱/Qt shim 이전에 private 라이브러리 로드, QML/plugin/helper/resources/locales 경로와 분리 엔진의 패키지·Framework 버전 확인 |
| [`.deb` 도구](../../../../packages/platforms/qt/tooling/linux_deb.py) | private SONAME local shlibs map, 외부 OS Depends, 엔진 패키지·정확한 앱 의존성·고지·소유권·Lintian, 앱/엔진/receipt 일괄 반영 |
| NuGet·검사 도구 | 수집·설치·recipe 도구와 native/WebView manifest 포함. 설치 runner에 엔진·앱 동시 APT 설치, backend/Qt 버전·기본 3회 실행 검사 추가 |

런타임은 `qt/framework/6.12.0/`, 동봉 시 `qt/webengine/6.140.0/`, `doroti-qt-runtime.json`과 licenses를 사용한다. 실제 Qt 6.12의 Wayland QPA는 `libqwayland.so`이고 Core에는 ICU 73이 필요해 Debian ICU 76으로 대체하지 않는다. Qt를 교체 불가능하게 숨기지 않으며 `DOROTI_QT_FRAMEWORK_PATH`/`DOROTI_QT_WEBENGINE_PATH`로 호환 수정 라이브러리를 선택할 수 있다. 런타임 원본 해시는 실행 허용 조건으로 강제하지 않는다.

## 마지막 변경: SDK inventory 선택 사항

원본 9·10절의 초기 필수 inventory 조건은 사용자 요청을 반영한 **11절이 대체**한다.

- `doroti-sdk.json` 없이도 폴더·`.deb`를 게시한다. 선택 SDK의 지원 Qt/ICU·Quick/QML·QPA 및 활성 엔진의 helper/resources/locales를 자동 수집하며 SDK를 수정하거나 다운로드하지 않는다.
- 결과는 `inventory: discovered`, `review: null`이다. 공급 RC 이름·archive 출처와 SDK에 있는 SBOM·설정·라이선스·고지·소스를 보존한다. SDK 설치 receipt가 있으면 선택 파일의 해시·버전도 검사한다.
- 파일이 있으면 명시적 inventory의 소스·고지·해시 검증을 적용한다. 잘못된 파일을 자동 수집으로 우회하지 않는다.
- 자동 수집이나 SDK 설치 성공을 재배포 자료 검토·정식 릴리스 승인으로 취급하지 않는다. 당시 정확한 배포 바이너리별 대응 소스·patch/build·third-party 자료 검토는 미완료였다. 별도 엔진 배포에도 같은 의무와 라이브러리 교체 안내가 남는다.

## 당시 검증 결과

고정 SDK로 Ubuntu 호스트와 Debian 13 격리 빌더에서 Quick·WebView native shim 빌드를 통과했다. Debian 13.7 amd64 빌더는 .NET SDK 10.0.400, glibc `2.41-12+deb13u4`, GCC/libstdc++ `14.2.0-19`, CMake `3.31.6-2`, patchelf `0.18.0-1.4`, Wayland client 1.23.1, Vulkan headers 1.4.309였다.

| 검사 | 당시 결과·증거 범위 |
| --- | --- |
| 초기 `linux_qt_runtime.py --package-tests` | 19개 PASS. 실제 ELF fixture 이동·분리 엔진·SDK 변조·누락 파일·의존성·재배포 자료/미검토 모듈 거부·prebuilt 검사 |
| Debian fixture 엔진 `.deb` | 생성·Lintian 오류 없음·외부 OS Depends·root 소유권·Framework 계약 확인. 실제 Qt/Chromium GUI 승인 아님 |
| C# loader 별도 프로세스 | private 선택과 호환 수정 fixture의 반환값 42→73 확인. 실제 수정 Qt 라이브러리 검증은 별도 |
| 초기 `.deb` 패키징 계약 | 9개 및 Qt provider/host·빌드 프로필·옵션·경로 검사 PASS |
| target/tool NuGet | pack 및 도구/manifest 포함 검사 PASS. fresh 소비자 실제 게시/GUI는 별도 |
| inventory 선택 사항 전환 후 ELF/로더/엔진 계약 | 24개 PASS. manifest 없는 동봉·분리, 변조·버전·경로 거부 및 개발 파일 제외 |
| 실제 SDK 파일 집합 | Framework 1,571개·엔진 83개 런타임 파일의 Qt ELF 의존성 검사 PASS |
| 최종 패키징 계약·분리 fixture 회귀 | 패키징 10개 PASS 및 fixture 엔진 `.deb` 회귀 PASS |
| 실제 Testbed Release/self-contained/동봉 `.deb` | **생성 PASS**, 270.3 MiB. package/desktop·Lintian 오류 없음·SHA-256 sidecar 확인, 시스템 Qt 패키지 Depends 없음 |
| 위 Testbed 패키지 설치·GUI·업데이트·제거 | **미검증** |

최종 Testbed는 Qt 6.12.0 / WebEngine `6.140.0-rc`, 양쪽 `inventory: discovered`·`review: null`이었다. SDK에 `doroti-sdk.json`을 만들지 않았다. 당시 산출물 경로는 `Doroti/artifacts/linux-deb/doroti-testbed_0.4.1-1_amd64/doroti-testbed_0.4.1-1_amd64.deb`다. GTK theme plugin의 OS 의존성 `libgtk-3-0t64`를 빌더에 준비하고, 알려진 Qt JPEG/Chromium embedded 구성 요소의 정확한 경로만 Lintian 예외로 다뤘다. 의존성·아키텍처 오류 검사는 유지했다.

## 남은 승인 조건과 기록 위치

1. WebEngine RC와 정식 릴리스를 구분하고, 실제 배포 조합의 대응 소스·patch/build·third-party 고지와 교체 안내를 검토한다.
2. Qt/SDK 없는 깨끗한 Debian 13에서 Quick·WebView bundled·WebView 분리 앱을 각각 게시·설치하고 native Wayland/xcb 반복 실행·정상 종료·업데이트/remove/purge·XDG 데이터 보존을 확인한다.
3. 분리 엔진의 오프라인 실행·기설치 재사용·동시 업데이트·설치 실패/복구·공유 엔진 제거 정책을 확인한다. 원격 서명 저장소는 선택적 후속 범위이며 검증 완료로 표시하지 않는다.
4. fresh NuGet-only 소비자의 실제 게시/GUI, 실제 수정 Qt 라이브러리, 일반 폴더·portable·개발 실행의 새 조합 회귀와 물리 GPU·화면·메뉴를 확인한다.

당시 로그/receipt는 `temp/testing/linux-deb/qt612-2026-10-19/`, `optional-sdk-manifest/`, `bundle-build-20261019/`에 기록했다. 마지막 빌드 기록은 `bundle-build-20261019/debian-optional-manifest-build.log`와 `result.json`이다. `contracts/contracts.json`은 fixture 범위를 명시한다. 이 경로들과 `Doroti/artifacts`는 삭제 가능한 로컬 산출물이며 이번 보관에서 현재 존재·무결성을 확인하지 않았다.

운영 절차는 [Qt 호스트 안내](../../../../packages/platforms/qt/README.md)와 [런타임 가이드](../../../../packages/platforms/qt/qt-runtime.md), 고정 입력은 [Framework lock](../../../../packages/platforms/qt/sdk/qt-6.12.0-linux-x64.lock.json)과 [WebEngine lock](../../../../packages/platforms/qt/sdk/webengine-6.140.0-qt6.12-linux-x64.lock.json)을 참고한다.
