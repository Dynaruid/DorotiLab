# Linux 번들 빌드 `.deb` 지원 작업계획

작성일: 2026-10-18

상태: 계획 작성. 아래 항목은 구현·검증 전이며, 명령과 신규 파일명은 설계안이다.

## 1. 목표와 기대 효과

기존 Linux Qt `publish` 결과를 재사용해 설치 가능한 `.deb` 패키지를 생성한다. 기존 폴더 배포를 유지하고, Linux 배포 형식에 `deb` 선택지를 추가한다.

| 목표 | 기대 효과 |
| --- | --- |
| 런타임 의존성 선언 | `apt install ./<app>.deb`가 필요한 시스템 패키지를 함께 설치하도록 한다. |
| 데스크톱 통합 | 앱 메뉴·아이콘·선택적 파일/URI 연결 정보를 설치·제거 과정에 포함한다. |
| 업데이트와 제거 | 동일한 패키지 ID와 버전 규칙으로 업그레이드하고, 패키지 관리자가 설치 파일을 관리한다. |
| 반복 가능한 배포 | 일반 사용자, 내부 PC, 테스트 장비에 같은 산출물을 배포한다. |

APT 저장소 운영은 후속 작업으로 둔다. `.deb` 파일 배포만으로 자동 업데이트가 제공되는 것은 아니며, 저장소를 운영할 때 APT 업데이트 배포로 확장한다.

## 2. 현재 구조와 제약

- Linux Qt provider는 `packages/platforms/qt/tooling/Extension.cs`에서 공통 `DotnetToolExtension`을 사용해 `build/run/publish/dev` 실행 계획을 만든다. 현재 확인한 경로에는 `.deb` 포장 단계가 없다.
- Qt는 시스템 의존성이다. 기본 Quick 호스트는 Qt 6.6 이상, WebView는 Qt 6.8 이상의 WebEngineQuick/WebChannel을 요구한다.
- .NET의 framework-dependent/self-contained 디렉터리 게시와 일부 단일 파일 게시가 이미 지원된다. 이번 `.deb` 초기 구현은 untrimmed JIT, self-contained 디렉터리 게시를 기준으로 한다.
- `Doroti/eng/install-linux-qt.py`는 별도 경로에 버전별 payload와 launcher를 배치하는 portable 설치기다. `.deb` 설치 파일은 dpkg가 소유·관리하도록 하고, portable 설치기를 패키지 설치 스크립트에서 호출하지 않는다.
- `packages/platforms/build/Doroti.AppIcons.targets`의 `DorotiLinuxIcon`과 게시되는 `appicon.png`를 재사용한다.
- `QtApplicationStorage`는 XDG 사용자 경로와 application ID로 저장 위치를 결정한다. 시스템 설치 경로에 사용자 데이터를 쓰지 않도록 기존 저장 동작을 확인한다.

## 3. 초기 지원 범위

| 항목 | 초기 계획 |
| --- | --- |
| 실행 타겟 | 기존 `linux` / `linux-x64` / Qt provider 유지 |
| 패키지 아키텍처 | `amd64` (`linux-x64` 매핑) |
| 첫 검증 배포판 | Debian 13 `amd64`. 기본 Qt 6.8 계열을 사용하는 고정된 빌드·실행 환경에서 검증 |
| .NET | Release, self-contained, untrimmed JIT 디렉터리 게시 |
| Qt | 시스템 런타임 의존성. Qt 바이너리 자체를 패키지에 복사하지 않음 |
| 기능 조합 | 기본 Quick 앱과 WebView 활성 앱을 각각 검증. GStreamer는 초기 `.deb` 지원 범위에서 제외하고 명시적으로 안내 |
| 설치 방식 | 로컬 `.deb`를 APT로 시스템 설치 |
| 후속 범위 | 추가 Ubuntu/Debian 버전, arm64, framework-dependent `.deb`, 단일 파일/R2R 입력, GStreamer, APT 저장소 |

Ubuntu 24.04 기본 Qt Quick은 6.4.2이므로 현재 기본 호스트의 Qt 6.6 요구사항을 충족하지 못한다. 초기 지원 대상으로 표시하지 않는다. 해당 버전을 지원하려면 Qt 별도 배포나 호스트 최소 버전 변경을 별도 과제로 검토한다.

배포판 지원 여부는 Qt 최소 버전만으로 결정하지 않는다. 실제 빌드 환경의 Qt·glibc·libstdc++ 요구사항, QML 모듈과 플랫폼 플러그인까지 확인하고 검증한 조합만 문서화한다.

## 4. 단계별 구현 계획

### 1단계 — 빌드 환경과 패키징 계약 확정

- [ ] Debian 13 `amd64` 빌드 환경과 런타임 검증 환경을 고정하고 Qt/.NET SDK/시스템 라이브러리 버전을 기록한다.
- [ ] 기존 `publish` 실행 계획과 산출물 구조를 확인하고, 포장 단계가 사용할 입력·출력·실패 처리를 정의한다.
- [ ] `linux-deb`라는 새 실행 플랫폼을 만들지 않고 Linux provider의 `publish` 옵션으로 배포 형식을 선택하도록 한다.
- [ ] `packageformat=directory|deb` 옵션과 출력 경로의 의미를 정의한다. 옵션 생략 시 기존 `publish` 동작을 유지한다.
- [ ] `.deb` 전용 옵션은 `publish`에만 허용한다. 미지원 호스트/RID/기능 조합은 빌드 전에 명확한 오류를 반환한다.
- [ ] 포장 시 필요한 `dpkg-deb`, `dpkg-shlibdeps`, ELF 분석 도구와 검증 도구를 정의한다. 일반 Linux 빌드에 `.deb` 도구 설치를 요구하지 않는다.

예정 CLI 형태는 다음과 같다. 실제 옵션명은 provider 계약과 함께 확정한다.

```powershell
./Doroti/eng/doroti.ps1 publish -App ./samples/DorotiCarouselApp -Platform linux -Configuration Release -PackageFormat deb -Output ./Doroti/artifacts/linux-deb
```

### 2단계 — 앱 메타데이터와 버전 규칙 정의

- [ ] MSBuild에서 평가한 application ID, 표시 이름, 실행 파일명, 버전, `DorotiLinuxIcon`, Qt 기능 옵션을 읽어 패키징 입력으로 전달한다. 프로젝트 파일을 단순 문자열 검색해 추정하지 않는다.
- [ ] Debian package ID, maintainer, description, 선택적 homepage/categories 값을 설정할 방법을 추가한다. 필수 값 누락과 잘못된 형식은 포장 전에 검증한다.
- [ ] application ID와 Debian package ID의 역할을 구분하고 모두 안정적으로 유지한다. 자동 변환으로 서로 다른 앱의 package ID가 충돌하지 않도록 한다.
- [ ] 버전은 Debian 비교 규칙에 맞게 변환한다. 예: `1.2.0-beta.1` → `1.2.0~beta.1-1`, `1.2.0` → `1.2.0-1`. 패키징 revision을 별도로 정의하고, build metadata의 처리와 충돌 방지를 명시한다.
- [ ] RID와 실제 ELF 아키텍처를 확인해 `Architecture: amd64`를 생성한다.
- [ ] 메타데이터를 control/desktop 파일에 쓸 때 개행·인수·경로 escaping을 처리한다.

### 3단계 — provider에 포장 단계 추가

- [ ] `packages/platforms/qt/tooling/Extension.cs`에 옵션 스키마, 조합 검증, 게시 후 포장 실행 계획을 추가한다.
- [ ] provider 전용 패키징 로직과 호출 진입점을 구현한다. 신규 파일 예시는 `packages/platforms/qt/tooling/LinuxDebPackage.cs`이며, 기존 `ExecutionPlan`에 연결할 방식을 확정한다.
- [ ] 선택된 `.NET` 실행 파일과 기존 TFM/RID를 유지하며 `.deb` 프로필에만 `SelfContained=true`, `PublishTrimmed=false`, `PublishSingleFile=false` 등 필요한 게시 속성을 적용한다. 지원하지 않는 속성 조합을 조용히 변경하지 않는다.
- [ ] 게시된 파일 집합을 검증한 뒤 별도 staging에 복사한다. 포장 과정에서 publish 원본을 변경하지 않는다.
- [ ] 취소·실패 시 성공한 `.deb`로 표시하지 않고 임시 파일을 정리한다. 완성된 산출물만 최종 경로에 반영한다.
- [ ] provider NuGet 패키지에 포장 실행에 필요한 코드·리소스를 포함해 저장소 소스가 없는 소비자도 사용할 수 있게 한다.
- [ ] 공통 CLI/SDK 변경은 provider 연동에 필요한 부분으로 한정한다. VS Code 확장의 생성물인 `dist/contracts.js`는 직접 수정하지 않는다.

패키지 내부 배치안:

| 경로 | 내용 |
| --- | --- |
| `/opt/<package-id>/` | 앱 publish payload, 네이티브 라이브러리, 기존 `licenses/` |
| `/usr/bin/<package-id>` | 앱 실행 launcher |
| `/usr/share/applications/<application-id>.desktop` | 앱 메뉴와 선택적 MIME/URI 처리 정보 |
| `/usr/share/icons/hicolor/<size>/apps/<application-id>.png` | 기존 앱 아이콘의 실제 크기에 맞는 리소스 |
| `/usr/share/doc/<package-id>/` | 저작권·라이선스 등 배포 문서 |
| `DEBIAN/control` | 패키지 메타데이터와 런타임 의존성 |

- [ ] launcher는 인수를 보존하고 실행 파일의 절대 경로를 사용한다. 앱 자산 로딩과 기존 네이티브 라이브러리 해시 검증이 설치 위치에서도 동작하는지 확인한다.
- [ ] 실행 파일/launcher 권한과 일반 데이터 파일 권한을 정규화하고 패키지 내부 소유자를 root로 기록한다. 패키지 생성 자체는 관리자 권한 없이 실행한다.
- [ ] 설치 경로의 파일은 dpkg가 관리한다. 사용자별 XDG 데이터는 기존 application ID로 유지하고 제거·purge 시 삭제하지 않는다.
- [ ] portable 설치기의 `XDG_DATA_HOME=<설치경로>/userdata` 동작을 시스템 설치 launcher에 적용하지 않는다. portable 데이터의 자동 이관은 후속 과제로 둔다.

### 4단계 — 런타임 의존성 생성

- [ ] 실행 파일, Qt shim, Skia, self-contained .NET 네이티브 파일 등 게시된 ELF를 분석하고 `dpkg-shlibdeps`로 배포판 런타임 의존성을 계산한다.
- [ ] payload 내부 라이브러리는 앱 내부에서 해석하고, 시스템 패키지 의존성과 구분한다. 미해결 외부 의존성이 있으면 포장을 실패시킨다.
- [ ] QML 모듈, Wayland/xcb QPA 플러그인, 동적 로딩 라이브러리는 ELF 분석만으로 모두 발견되지 않으므로 별도 기능별 목록을 관리한다.
- [ ] WebView 활성 시 WebEngineQuick/WebChannel, helper 실행 파일, 리소스·QML 의존성을 기존 runtime manifest와 대조한다. 비활성 앱에는 해당 의존성을 추가하지 않는다.
- [ ] `Depends`에는 런타임 패키지를 선언한다. CMake, compiler, `*-dev`, .NET SDK 등 빌드 도구는 사용자 PC에 설치하도록 요구하지 않는다.
- [ ] `.NET self-contained`에 포함되지 않는 시스템 라이브러리도 확인한다. GPU 드라이버와 Vulkan 1.2 실행 조건은 패키지 의존성만으로 검증할 수 없으므로 별도로 안내한다.
- [ ] 생성된 의존성 목록과 최소 버전을 깨끗한 런타임 환경에서 검증한다.

### 5단계 — 데스크톱 통합과 산출물 기록

- [ ] 표시 이름, 실행 경로, 아이콘, categories를 포함한 `.desktop` 파일을 생성하고 `desktop-file-validate`로 확인한다.
- [ ] 앱이 선언한 파일 형식/URI scheme에만 처리 정보를 등록한다. 설치 과정에서 사용자의 기본 연결을 강제로 변경하지 않는다.
- [ ] 표준 패키지 trigger를 우선 사용하고, 필요한 경우에만 최소한의 maintainer script를 추가한다. 설치 스크립트에서 앱을 실행하지 않는다.
- [ ] 기존 license/notice 파일이 설치 결과에 유지되는지 확인한다.
- [ ] `<package-id>_<debian-version>_amd64.deb`와 SHA-256, 패키지 메타데이터, 빌드 환경, 기능 옵션, 의존성 목록을 기록한다.
- [ ] 패키지 생성 성공과 설치·실행 검증 상태를 분리해 기록한다. 컨테이너 설치 성공만으로 GUI 실행까지 통과했다고 표시하지 않는다.

## 5. 검증 계획

실제 설치·업데이트·제거는 disposable 컨테이너/VM에서 수행한다. 현재 개발 PC의 패키지 상태는 변경하지 않는다. 테스트 작업물과 로그는 `temp/testing/linux-deb/<run>/`에 둔다.

| 검증 | 확인 내용 |
| --- | --- |
| provider 계약 | 기본 publish 유지, deb 단계 순서, 옵션/출력 경로, 공백·한글 경로, 잘못된 host/RID/설정 거부, 취소·실패 전파 |
| 패키지 내용 | `dpkg-deb --info/--contents`, 메타데이터·권한·소유자, 실행 파일·자산·아이콘·라이선스 누락 여부 |
| 의존성 | 개발 도구 없는 환경에서 `apt install ./<app>.deb`; Quick와 WebView 각각의 필수 런타임 설치 및 미해결 의존성 확인 |
| GUI 실행 | 일반 사용자로 메뉴/launcher 실행, 첫 프레임, 자산 로딩, Wayland/xcb와 WebView 활성 경로 확인 |
| 업데이트 | v1 설치 → 사용자 데이터 생성 → v2 설치 → 실행 파일·버전 갱신 및 동일 application ID의 데이터 보존 |
| 제거 | remove/purge 후 패키지 소유 파일·메뉴·아이콘 제거, 사용자 데이터 보존 |
| 별도 소비자 | fresh NuGet feed/cache로 템플릿을 복원하고 `.deb` 생성·설치·실행 |
| 회귀 | 기존 Linux 폴더 게시·portable 설치·개발 실행의 기존 검증 결과 유지 |

- [ ] `packages/platforms/qt/tests/Doroti.Tool.Qt.Tests/Program.cs`에 의미 있는 실행 계획·옵션 검증을 추가한다.
- [ ] `.deb` 실제 생성·검사와 설치 lifecycle용 테스트를 추가한다. 후보 파일명: `Doroti/tests/linux_deb_package.py`, `Doroti/tests/linux_deb_install.py`.
- [ ] 기존 `linux_qt_packages.py`의 소비자 fixture가 현재 provider 구조와 맞는지 확인한 뒤 재사용 범위를 정한다.
- [ ] `Doroti/eng/validate.py`에 패키지 정적 검증과 실제 OS 설치/GUI 검증을 구분해 연결한다.
- [ ] `lintian`으로 패키지 오류를 확인하고, 경고는 원인과 처리 결과를 기록한다.
- [ ] 요청한 설치/GUI 검증을 환경 부족으로 실행하지 못하면 미검증 상태로 남긴다. 초기 지원 완료는 실제 검증 통과 후 선언한다.

## 6. 문서와 완료 기준

- [ ] `Doroti/README.md`, `Doroti/README.ko.md`, `packages/platforms/qt/README.md`에 생성·설치·업데이트·제거 방법을 추가한다.
- [ ] 템플릿 Linux 안내와 대표 샘플에 package ID/maintainer/version/icon 설정 예시를 제공한다.
- [ ] `Doroti/tests/README.md`에 검증 명령, 필요한 환경, 증거 범위를 기록한다.
- [ ] Debian 13 `amd64`의 고정 환경에서 Quick/WebView 앱이 각각 설치·실행·업데이트·제거 검증을 통과한다.
- [ ] .NET SDK와 개발용 Qt 도구가 없는 사용자 환경에서 앱이 실행된다.
- [ ] 사용자 데이터가 업그레이드·remove·purge 후 보존된다.
- [ ] 저장소 밖 NuGet 소비자에서도 패키지를 생성하며, 기존 Linux 배포 방식의 회귀가 없다.

구현 순서: 배포판·옵션 계약 확정 → 메타데이터/버전 규칙 → provider 포장 단계 → 의존성/데스크톱 통합 → 실제 설치·실행·업데이트 검증 → 문서와 지원 범위 확정.

## 7. 참고 자료

- [Qt 호스트의 현재 배포 요구사항](packages/platforms/qt/native/README.md)
- [Linux portable 설치기](Doroti/eng/install-linux-qt.py)
- [Debian 패키지 설치와 의존성 처리](https://www.debian.org/doc/manuals/debian-reference/ch02.en.html)
- [Debian 공유 라이브러리 의존성 생성](https://www.debian.org/doc/debian-policy/ch-sharedlibs.html)
- [Debian 데스크톱 메뉴와 MIME/URI 등록](https://www.debian.org/doc/debian-policy/ch-opersys.html)
- [Debian 13 Qt Quick 패키지](https://packages.debian.org/trixie/libqt6quick6)
- [Ubuntu 24.04 Qt Quick 패키지](https://packages.ubuntu.com/noble/libqt6quick6)
