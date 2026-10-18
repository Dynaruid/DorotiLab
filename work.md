# Qt 6.12 런타임 번들 및 Linux `.deb` 지원 작업계획

작성일: 2026-10-18

상태: `.deb` 포장과 Debian 13/Qt 6.8.2의 xcb 검증 완료. 2026-10-19에 `.deb` 배포 목표를 Qt 6.12 LTS, Wayland 필수로 변경했다. Qt Framework와 WebEngine은 기본적으로 앱에 동봉하고, 빌드 옵션으로 `.deb` 설치 시에만 WebEngine을 별도 다운로드·설치하도록 구조를 변경한다. 이 구조는 구현·검증 전이며, 8절은 기존 결과, 9절은 변경된 구현 계약이다.

## 1. 목표와 기대 효과

기존 Linux Qt `publish` 결과를 재사용해 설치 가능한 `.deb` 패키지를 생성한다. 기존 폴더 배포를 유지하고, Linux 배포 형식에 `deb` 선택지를 추가한다.

| 목표 | 기대 효과 |
| --- | --- |
| 런타임 의존성 선언 | `apt install ./<app>.deb`가 필요한 시스템 패키지를 함께 설치하도록 한다. |
| 데스크톱 통합 | 앱 메뉴·아이콘·선택적 파일/URI 연결 정보를 설치·제거 과정에 포함한다. |
| 업데이트와 제거 | 동일한 패키지 ID와 버전 규칙으로 업그레이드하고, 패키지 관리자가 설치 파일을 관리한다. |
| 반복 가능한 배포 | 일반 사용자, 내부 PC, 테스트 장비에 같은 산출물을 배포한다. |

Qt 6.12 LTS의 필요한 공유 라이브러리·QML 모듈·플러그인을 앱 바이너리와 함께 배포한다. WebEngine은 Framework와 별도로 버전을 고정해 빌드 환경에 설치한 뒤, 기본적으로 앱에 동봉한다. 별도 다운로드는 `.deb` 배포에서만 빌드 옵션으로 허용하며, 앱을 설치할 때 APT가 WebEngine 런타임 패키지를 함께 내려받아 설치한다. 일반 폴더·portable 배포는 동봉 방식만 사용한다. 기본 동봉 배포에는 Qt 저장소 등록을 요구하지 않고, 선택적 다운로드 배포에는 WebEngine 패키지 공급 저장소를 설치 전에 등록한다.

## 2. 현재 구조와 제약

- Linux Qt provider는 `packages/platforms/qt/tooling/Extension.cs`에서 공통 `DotnetToolExtension`을 사용해 `build/run/publish/dev` 실행 계획을 만든다. `.deb` 포장은 provider 전용 `linux_deb.py`로 구현했다.
- 기존 일반 Qt 호스트의 최소 요구사항은 Quick 6.6, WebView 6.8이었다. 새 번들 배포의 기준은 Quick/WebView 모두 Qt 6.12 LTS다. 일반 폴더·portable는 동봉 방식만 지원하고 `.deb`에만 WebEngine 외부 설치 옵션을 제공한다.
- Debian 13 기본 저장소의 Qt는 6.8.2다. `Depends`의 버전만 높여서는 6.12를 설치할 수 없다. Debian 13에서 동작을 확인한 앱 전용 Qt 번들과 실제 설치 검증이 필요하다. 앱의 `Depends`에는 OS 런타임을 남기고 Qt/QML 시스템 패키지 의존성은 번들로 대체한다.
- .NET의 framework-dependent/self-contained 디렉터리 게시와 일부 단일 파일 게시가 이미 지원된다. 이번 `.deb` 초기 구현은 untrimmed JIT, self-contained 디렉터리 게시를 기준으로 한다.
- `Doroti/eng/install-linux-qt.py`는 별도 경로에 버전별 payload와 launcher를 배치하는 portable 설치기다. `.deb` 설치 파일은 dpkg가 소유·관리하도록 하고, portable 설치기를 패키지 설치 스크립트에서 호출하지 않는다.
- `packages/platforms/build/Doroti.AppIcons.targets`의 `DorotiLinuxIcon`과 게시되는 `appicon.png`를 재사용한다.
- `QtApplicationStorage`는 XDG 사용자 경로와 application ID로 저장 위치를 결정한다. 시스템 설치 경로에 사용자 데이터를 쓰지 않도록 기존 저장 동작을 확인한다.

## 3. 초기 지원 범위

| 항목 | 초기 계획 |
| --- | --- |
| 실행 타겟 | 기존 `linux` / `linux-x64` / Qt provider 유지 |
| 패키지 아키텍처 | `amd64` (`linux-x64` 매핑) |
| 첫 검증 배포판 | Debian 13 `amd64` + 앱에 동봉한 Qt 6.12.0. 같은 조합의 고정된 빌드·실행 환경에서 재검증 |
| .NET | Release, self-contained, untrimmed JIT 디렉터리 게시 |
| Qt | 6.12 LTS 계열 공유 라이브러리를 앱 전용 디렉터리에 동봉. 첫 검증 버전은 6.12.0. LGPL 조건을 충족하는 모듈 구성과 교체 가능한 동적 링크 사용 |
| WebEngine | 별도 릴리스를 빌드 환경에 설치. 기본 `bundled`. `download`는 `.deb` 전용이며 앱 설치 시 APT 의존성으로 설치. Framework와 별도로 버전·해시·호환성을 기록 |
| 디스플레이 | 네이티브 Wayland와 xcb 모두 필수. Wayland 지원 승인을 XWayland 실행 결과로 대체하지 않음 |
| 기능 조합 | 기본 Quick 앱과 WebView 활성 앱을 각각 검증. GStreamer는 초기 `.deb` 지원 범위에서 제외하고 명시적으로 안내 |
| 설치 방식 | 로컬 앱 `.deb`를 APT로 설치. `download`도 앱 설치 시 엔진 준비를 완료하며, 앱 실행 중 다운로드 없음 |
| 후속 범위 | 추가 Ubuntu/Debian 버전, arm64, framework-dependent `.deb`, 단일 파일/R2R 입력, GStreamer, 앱 패키지의 APT 배포 |

Ubuntu 24.04 기본 Qt Quick은 6.4.2이므로 현재 기본 호스트의 Qt 6.6 요구사항을 충족하지 못한다. 초기 지원 대상으로 표시하지 않는다. 해당 버전을 지원하려면 Qt 별도 배포나 호스트 최소 버전 변경을 별도 과제로 검토한다.

배포판 지원 여부는 Qt 최소 버전만으로 결정하지 않는다. 실제 빌드 환경의 Qt·glibc·libstdc++ 요구사항, QML 모듈과 플랫폼 플러그인까지 확인하고 검증한 조합만 문서화한다.

## 4. 단계별 구현 계획

### 1단계 — 빌드 환경과 패키징 계약 확정

- [ ] Debian 13 `amd64` + Qt 6.12.0 빌드 환경과 런타임 검증 환경을 고정하고 Qt/.NET SDK/시스템 라이브러리 버전을 기록한다. 이전 Qt 6.8.2 환경의 기록은 8절에 보존한다.
- [x] 기존 `publish` 실행 계획과 산출물 구조를 확인하고, 포장 단계가 사용할 입력·출력·실패 처리를 정의한다.
- [x] `linux-deb`라는 새 실행 플랫폼을 만들지 않고 Linux provider의 `publish` 옵션으로 배포 형식을 선택하도록 한다.
- [x] `packageformat=directory|deb` 옵션과 출력 경로의 의미를 정의한다. 옵션 생략 시 기존 `publish` 동작을 유지한다.
- [x] `.deb` 전용 옵션은 `publish`에만 허용한다. 미지원 호스트/RID/기능 조합은 빌드 전에 명확한 오류를 반환한다.
- [x] 포장 시 필요한 `dpkg-deb`, `dpkg-shlibdeps`, ELF 분석 도구와 검증 도구를 정의한다. 일반 Linux 빌드에 `.deb` 도구 설치를 요구하지 않는다.

확정된 CLI 형태는 다음과 같다. `-Output`은 CLI 현재 디렉터리 기준이며, 완성된 산출물 묶음만 `<Output>/<package-id>_<debian-version>_amd64/`에 원자적으로 반영한다.

```powershell
./Doroti/eng/doroti.ps1 publish -App ./samples/DorotiCarouselApp -Platform linux -Configuration Release -PackageFormat deb -Output ./Doroti/artifacts/linux-deb
```

### 2단계 — 앱 메타데이터와 버전 규칙 정의

- [x] MSBuild에서 평가한 application ID, 표시 이름, 실행 파일명, 버전, `DorotiLinuxIcon`, Qt 기능 옵션을 읽어 패키징 입력으로 전달한다. 프로젝트 파일을 단순 문자열 검색해 추정하지 않는다.
- [x] Debian package ID, maintainer, description, 선택적 homepage/categories 값을 설정할 방법을 추가한다. 필수 값 누락과 잘못된 형식은 포장 전에 검증한다.
- [x] application ID와 Debian package ID의 역할을 구분하고 모두 안정적으로 유지한다. 자동 변환으로 서로 다른 앱의 package ID가 충돌하지 않도록 한다.
- [x] 버전은 Debian 비교 규칙에 맞게 변환한다. 예: `1.2.0-beta.1` → `1.2.0~beta.1-1`, `1.2.0` → `1.2.0-1`. 패키징 revision을 별도로 정의하고, build metadata의 처리와 충돌 방지를 명시한다.
- [x] RID와 실제 ELF 아키텍처를 확인해 `Architecture: amd64`를 생성한다.
- [x] 메타데이터를 control/desktop 파일에 쓸 때 개행·인수·경로 escaping을 처리한다.

### 3단계 — provider에 포장 단계 추가

- [x] `packages/platforms/qt/tooling/Extension.cs`에 옵션 스키마, 조합 검증, 게시 후 포장 실행 계획을 추가한다.
- [x] provider 전용 패키징 로직과 호출 진입점을 구현한다. 구현은 `packages/platforms/qt/tooling/linux_deb.py`이다. 기존 publish 인수와 선택된 dotnet을 하나의 소유 프로세스로 감싸 취소/실패 시 publish와 staging을 함께 정리하도록 `ExecutionPlan`에 연결했다.
- [x] 선택된 `.NET` 실행 파일과 기존 TFM/RID를 유지하며 `.deb` 프로필에만 `SelfContained=true`, `PublishTrimmed=false`, `PublishSingleFile=false` 등 필요한 게시 속성을 적용한다. 지원하지 않는 속성 조합을 조용히 변경하지 않는다.
- [x] 게시된 파일 집합을 검증한 뒤 별도 staging에 복사한다. 포장 과정에서 publish 원본을 변경하지 않는다.
- [x] 취소·실패 시 성공한 `.deb`로 표시하지 않고 임시 파일을 정리한다. 완성된 산출물만 최종 경로에 반영한다.
- [x] provider NuGet 패키지에 포장 실행에 필요한 코드·리소스를 포함해 저장소 소스가 없는 소비자도 사용할 수 있게 한다.
- [x] 공통 CLI/SDK 변경은 provider 연동에 필요한 부분으로 한정한다. VS Code 확장의 생성물인 `dist/contracts.js`는 직접 수정하지 않는다.

패키지 내부 배치안:

| 경로 | 내용 |
| --- | --- |
| `/opt/<package-id>/` | 앱 publish payload, 네이티브 라이브러리, 기존 `licenses/` |
| `/usr/bin/<package-id>` | 앱 실행 launcher |
| `/usr/share/applications/<application-id>.desktop` | 앱 메뉴와 선택적 MIME/URI 처리 정보 |
| `/usr/share/icons/hicolor/<size>/apps/<application-id>.png` | 기존 앱 아이콘의 실제 크기에 맞는 리소스 |
| `/usr/share/doc/<package-id>/` | 저작권·라이선스 등 배포 문서 |
| `DEBIAN/control` | 패키지 메타데이터와 런타임 의존성 |

- [x] launcher는 인수를 보존하고 실행 파일의 절대 경로를 사용한다. 앱 자산 로딩과 기존 네이티브 라이브러리 해시 검증이 설치 위치에서도 동작하는지 확인한다.
- [x] 실행 파일/launcher 권한과 일반 데이터 파일 권한을 정규화하고 패키지 내부 소유자를 root로 기록한다. 패키지 생성 자체는 관리자 권한 없이 실행한다.
- [x] 설치 경로의 파일은 dpkg가 관리한다. 사용자별 XDG 데이터는 기존 application ID로 유지하고 제거·purge 시 삭제하지 않는다.
- [x] portable 설치기의 `XDG_DATA_HOME=<설치경로>/userdata` 동작을 시스템 설치 launcher에 적용하지 않는다. portable 데이터의 자동 이관은 후속 과제로 둔다.

### 4단계 — 런타임 의존성 생성

체크된 항목은 기존 시스템 Qt 기반 구현의 상태다. 번들 Qt와 다운로드 WebEngine에 대한 변경·재검증은 아래 미완료 항목과 9절을 따른다.

- [x] 실행 파일, Qt shim, Skia, self-contained .NET 네이티브 파일 등 게시된 ELF를 분석하고 `dpkg-shlibdeps`로 배포판 런타임 의존성을 계산한다.
- [x] payload 내부 라이브러리는 앱 내부에서 해석하고, 시스템 패키지 의존성과 구분한다. 미해결 외부 의존성이 있으면 포장을 실패시킨다.
- [x] QML 모듈, Wayland/xcb QPA 플러그인, 동적 로딩 라이브러리는 ELF 분석만으로 모두 발견되지 않으므로 별도 기능별 목록을 관리한다.
- [x] WebView 활성 시 WebEngineQuick/WebChannel, helper 실행 파일, 리소스·QML 의존성을 기존 runtime manifest와 대조한다. 비활성 앱에는 해당 의존성을 추가하지 않는다.
- [x] `Depends`에는 런타임 패키지를 선언한다. CMake, compiler, `*-dev`, .NET SDK 등 빌드 도구는 사용자 PC에 설치하도록 요구하지 않는다.
- [x] `.NET self-contained`에 포함되지 않는 시스템 라이브러리도 확인한다. GPU 드라이버와 Vulkan 1.2 실행 조건은 패키지 의존성만으로 검증할 수 없으므로 별도로 안내한다.
- [x] 생성된 의존성 목록과 최소 버전을 깨끗한 런타임 환경에서 검증한다.
- [ ] 기존 Qt/QML 시스템 패키지 의존성을 앱 전용 Qt 6.12 번들로 대체하고, 지원 목록 밖의 Qt 버전을 포장 전에 거부한다. 번들 내부 ELF와 외부 OS 런타임의 의존성을 구분한다.
- [ ] Quick/WebView/QML/QPA/WebEngine helper·resources·locales를 검증된 호환 릴리스 조합으로 설치하고, 런타임만 설치한 환경에서 다시 확인한다. Qt 6.12부터 WebEngine의 별도 버전 체계를 반영한다.

### 5단계 — 데스크톱 통합과 산출물 기록

- [x] 표시 이름, 실행 경로, 아이콘, categories를 포함한 `.desktop` 파일을 생성하고 `desktop-file-validate`로 확인한다.
- [x] 앱이 선언한 파일 형식/URI scheme에만 처리 정보를 등록한다. 설치 과정에서 사용자의 기본 연결을 강제로 변경하지 않는다.
- [x] 표준 패키지 trigger를 우선 사용하고, 필요한 경우에만 최소한의 maintainer script를 추가한다. 설치 스크립트에서 앱을 실행하지 않는다.
- [x] 기존 license/notice 파일이 설치 결과에 유지되는지 확인한다.
- [x] `<package-id>_<debian-version>_amd64.deb`와 SHA-256, 패키지 메타데이터, 빌드 환경, 기능 옵션, 의존성 목록을 기록한다.
- [x] 패키지 생성 성공과 설치·실행 검증 상태를 분리해 기록한다. 컨테이너 설치 성공만으로 GUI 실행까지 통과했다고 표시하지 않는다.

## 5. 검증 계획

실제 설치·업데이트·제거는 disposable 컨테이너/VM에서 수행한다. 현재 개발 PC의 패키지 상태는 변경하지 않는다. 테스트 작업물과 로그는 `temp/testing/linux-deb/<run>/`에 둔다.

| 검증 | 확인 내용 |
| --- | --- |
| provider 계약 | 기본 publish 유지, deb 단계 순서, 옵션/출력 경로, 공백·한글 경로, 잘못된 host/RID/설정 거부, 취소·실패 전파 |
| 패키지 내용 | `dpkg-deb --info/--contents`, 메타데이터·권한·소유자, 실행 파일·자산·아이콘·라이선스 누락 여부 |
| 런타임 선택 | Qt SDK·시스템 Qt가 없는 환경과 Debian 기본 Qt 6.8이 있는 환경에서 모두 앱 전용 Qt 6.12를 선택하는지 확인 |
| 의존성 | 개발 도구·Qt 없는 환경에서 `apt install ./<app>.deb`; 번들 내부 ELF/QML/QPA/WebEngine과 외부 OS 런타임의 의존성 확인 |
| WebEngine 다운로드 | `.deb` 전용 옵션 검증, 저장소 등록·서명/패키지 해시·버전 의존성, 설치 시 엔진 다운로드/기설치 재사용·실패/복구·오프라인 실행·업데이트/공유 엔진 제거 검증 |
| 재배포 조건 | LGPL 가능한 모듈·동적 링크·정확한 대응 소스·third-party notice·교체 안내 확인, 수정한 호환 라이브러리로 실제 실행 |
| GUI 실행 | 일반 사용자로 메뉴/launcher 실행, 첫 프레임, 자산 로딩, Wayland/xcb와 WebView 활성 경로 확인. Wayland는 backend를 강제하고 실제 선택 결과를 기록하며 반복 실행·정상 종료까지 확인 |
| 업데이트 | v1 설치 → 사용자 데이터 생성 → v2 설치 → 실행 파일·버전 갱신 및 동일 application ID의 데이터 보존 |
| 제거 | remove/purge 후 패키지 소유 파일·메뉴·아이콘 제거, 사용자 데이터 보존 |
| 별도 소비자 | fresh NuGet feed/cache로 템플릿을 복원하고 `.deb` 생성·설치·실행 |
| 회귀 | 기존 Linux 폴더 게시·portable 설치·개발 실행의 기존 검증 결과 유지 |

- [x] `packages/platforms/qt/tests/Doroti.Tool.Qt.Tests/Program.cs`에 의미 있는 실행 계획·옵션 검증을 추가한다.
- [x] `.deb` 실제 생성·검사와 설치 lifecycle용 테스트를 추가한다. 후보 파일명: `Doroti/tests/linux_deb_package.py`, `Doroti/tests/linux_deb_install.py`.
- [x] 기존 `linux_qt_packages.py`의 소비자 fixture가 현재 provider 구조와 맞는지 확인한 뒤 재사용 범위를 정한다.
- [x] `Doroti/eng/validate.py`에 패키지 정적 검증과 실제 OS 설치/GUI 검증을 구분해 연결한다.
- [x] `lintian`으로 패키지 오류를 확인하고, 경고는 원인과 처리 결과를 기록한다.
- [x] 설치와 GUI 검증 결과를 별도로 기록한다. xcb는 통과했고 Wayland는 실제 실패로 남겼다. 물리 GPU/메뉴 활성화는 미검증이며, 초기 지원 전체 완료는 선언하지 않는다.

## 6. 문서와 완료 기준

- [x] `Doroti/README.md`, `Doroti/README.ko.md`, `packages/platforms/qt/README.md`에 생성·설치·업데이트·제거 방법을 추가한다.
- [x] 템플릿 Linux 안내와 대표 샘플에 package ID/maintainer/version/icon 설정 예시를 제공한다.
- [x] `Doroti/tests/README.md`에 검증 명령, 필요한 환경, 증거 범위를 기록한다.
- [ ] Debian 13 `amd64` + Qt 6.12.0의 고정 환경에서 Quick/WebView 앱이 각각 설치·실행·업데이트·제거와 Wayland/xcb 검증을 통과한다. **기존 Qt 6.8.2의 xcb 통과와 Ubuntu/Qt 6.10.2의 Wayland 통과는 새 조합의 검증을 대신하지 않는다.**
- [ ] Qt 6.12 번들 설치 및 WebEngine `bundled|download` 양쪽의 사용자 절차와 LGPL/third-party 재배포 자료·라이브러리 교체 안내를 제공한다.
- [x] .NET SDK와 개발용 Qt 도구가 없는 사용자 환경에서 앱이 실행된다.
- [x] 사용자 데이터가 업그레이드·remove·purge 후 보존된다.
- [x] 저장소 밖 NuGet 소비자에서도 패키지를 생성하며, 기존 Linux 배포 방식의 회귀가 없다.

구현 순서: 배포판·옵션 계약 확정 → 메타데이터/버전 규칙 → provider 포장 단계 → 의존성/데스크톱 통합 → 실제 설치·실행·업데이트 검증 → 문서와 지원 범위 확정.

## 7. 참고 자료

- [Qt 호스트의 현재 배포 요구사항](packages/platforms/qt/native/README.md)
- [Linux portable 설치기](Doroti/eng/install-linux-qt.py)
- [Debian 패키지 설치와 의존성 처리](https://www.debian.org/doc/manuals/debian-reference/ch02.en.html)
- [Debian 공유 라이브러리 의존성 생성](https://www.debian.org/doc/debian-policy/ch-sharedlibs.html)
- [Debian 데스크톱 메뉴와 MIME/URI 등록](https://www.debian.org/doc/debian-policy/ch-opersys.html)
- [Debian 13 Qt Quick 패키지](https://packages.debian.org/trixie/libqt6quick6)
- [Ubuntu 24.04 Qt Quick 패키지](https://packages.ubuntu.com/noble/libqt6quick6)

## 8. 구현 및 검증 결과 — 2026-10-18

아래는 Debian 기본 Qt 6.8.2를 사용한 이전 구현의 기록이다. 새 배포 기준은 9절을 따른다.

### 확정된 구현 계약

- Qt provider의 `packageformat=directory|deb`, publish-only `output`을 추가했다. 생략 시 기존 폴더 게시를 유지한다. 새 플랫폼이나 VS Code `dist/contracts.js` 변경은 없다.
- `Extension.cs`가 선택된 dotnet/TFM/RID/publish 인수를 보존해 NuGet에 포함된 `linux_deb.py`를 실행한다. 포장 helper가 게시·staging·자식 프로세스를 소유하고 세션별 Stop 신호, 실패 전파, 임시 디렉터리 정리를 담당한다.
- Debian 13 amd64, Release/net10.0/linux-x64, Quick/Graphite, self-contained untrimmed JIT만 허용한다. AOT/trimming/single-file/R2R/GStreamer와 `UseAppHost=false`는 게시 전에 거부한다.
- `DorotiDebPackageId`, `DorotiDebMaintainer`, `DorotiDebDescription`, `DorotiDebCopyright`를 필수 설정으로 정의했다. 선택적인 homepage/categories/MIME/URI 설정, MSBuild `Version` 또는 `DorotiDebVersion`, 양의 정수 `DorotiDebRevision`을 사용한다. application ID는 평가된 manifest와 대조한다.
- SemVer prerelease를 Debian `~` 규칙으로 변환한다. `+build.metadata`는 충돌 방지를 위해 거부한다. 같은 출력 위치의 동일 버전 산출물은 덮어쓰지 않는다.
- 원본 publish와 설치 payload 각각의 SHA-256을 기록한다. 게시 후 NuGet assets 경로를 다시 평가해 다른 호스트/컨테이너의 오래된 캐시 경로를 사용하지 않는다. 모든 설치 ELF의 amd64 아키텍처와 외부 의존성을 검사한다.
- 공식 .NET linux-x64 runtime pack의 선택적 `libcoreclrtraceptprovider.so`는 Debian 13에 없는 `liblttng-ust.so.0`을 요구한다. `DorotiDebExcludeLttngProvider=true`로 명시한 경우에만 staging에서 제외하며, 원본 publish는 유지하고 제외 내역을 receipt에 남긴다. 이 패키지에서는 LTTng tracing을 지원하지 않는다.
- launcher는 절대 실행 경로와 `"$@"`를 사용하고 XDG 경로를 변경하지 않는다. Qt Wayland 로더에 개발용 symlink 대신 `libvulkan.so.1`을 지정한다. 초기 패키지의 기본 QPA는 검증된 `xcb`이며 명시적 환경 설정은 보존한다. Wayland 데스크톱에서는 XWayland display가 필요하다.
- `/opt`, `/usr/bin`, desktop entry, 실제 크기의 hicolor PNG, copyright/.NET notices/changelog를 포장한다. root 소유자, 실행 파일 0755/일반 데이터 0644를 기록한다. 앱 실행/portable 설치/사용자 기본 연결 변경/사용자 데이터 삭제 maintainer script는 없다.
- 완료된 `.deb`, SHA-256, receipt, contents, shlibdeps, lintian 보고서를 하나의 디렉터리로 원자적으로 반영한다. 패키지 생성 receipt의 install/gui/upgrade/remove/purge는 별도 검증 전까지 `not-run`이다.

### 함께 수정한 기존 문제

| 문제 | 수정 |
| --- | --- |
| NuGet provider 탐색이 `obj/project.assets.json`에 고정돼 RID별 obj 경로의 템플릿을 찾지 못함 | 선택된 dotnet/TFM/RID/configuration으로 `ProjectAssetsFile`을 평가하고 provider/target 조회에 전달 |
| Testbed 및 템플릿 Linux props가 상위 application ID/title/version을 상속하지 않음 | 상위 `Directory.Build.props` import 추가 |
| 아이콘 publish 단계가 PDB도 변환 대상으로 분류해 없는 staging PDB 복사를 시도함 | 실제 icon DLL만 교체 대상으로 선택 |
| Qt 6.8.2가 QmlMeta 모듈을 추가로 링크함 | Debian copyright에서 LGPL 옵션을 확인하고 shared-module allowlist/notice 갱신 |
| Qt WebEngine named profile 생성자는 6.9부터 제공됨 | 6.8에서는 storageName/offTheRecord를 명시해 기존 ID별 저장 경로 유지 |
| Qt 6.8의 초기 QML userScripts binding이 미초기화 adapter를 역참조함 | component completion 후 첫 load 전에 스크립트를 설치; 실제 native crash/정상 종료를 gdb로 확인 |

### 고정한 환경과 증거 범위

일반 사용자 UID 1000의 user namespace/bwrap로 disposable Debian rootfs를 구성했다. 현재 Ubuntu PC의 패키지 상태를 변경하지 않았다. 빌드와 깨끗한 런타임은 별도 rootfs이며, 런타임에는 저장소/SDK를 마운트하지 않았다.

| 항목 | 기록 |
| --- | --- |
| Base image | `debian:trixie-slim`, amd64 manifest `sha256:918311b7b6c4c6f68b232ba516584925f6c78ad82b6fd534b98979df6438e483` |
| 빌드 userspace | Debian 13, .NET SDK 10.0.400 / self-contained runtime 10.0.11, Qt/WebEngine 6.8.2 |
| 시스템 라이브러리 | glibc `2.41-12+deb13u4`, libstdc++/libgcc `14.2.0-19`; 나머지는 각 receipt의 최소 의존성과 dpkg 목록 |
| 검증 도구 | dpkg-deb 1.22.22, binutils 2.44, lintian 2.122.0 |
| GUI | xcb/Xvfb 및 Debian Weston 14.0.2 headless; Mesa `25.0.7-2+deb13u1`, software Vulkan |
| 기존 개발 환경 회귀 | Ubuntu 26.04, Qt 6.10.2의 폴더 게시 및 WebView/Wayland 실행 통과 |

| 검증 | 결과 | 증거 (`temp/testing/linux-deb/current/` 기준) |
| --- | --- | --- |
| provider 옵션/기본 폴더 게시/공백·한글 인수/RID·TFM·Release/취소 | PASS | `static-contracts-last.log`, validate의 retained run |
| 버전·메타데이터 주입 거부/실제 ELF 아키텍처/실행 중 취소/실패 rollback | PASS | `contracts-final.log`, 최신 정적 suite |
| 실제 Quick/WebView `.deb`, desktop, owner/mode, payload hashes, Qt 미포함 | PASS | `quick-inspection/`, `webview-inspection/`, `webview-fixed-packages/` |
| ELF/QML/플러그인/.NET/WebEngine runtime 의존성의 clean APT 설치 | PASS | `quick-acceptance/install-v1.log`, `webview-acceptance/install-v1.log` |
| Quick의 SDK/Qt dev 없는 UID 1000 xcb 첫 프레임, v2 실행 | PASS | `quick-acceptance/gui-v1.log`, `gui-v2.log` |
| WebView 활성 경로의 UID 1000 xcb native composition, v2 실행 | PASS | `webview-acceptance/gui-v1.log`, `gui-v2.log`; compositionFrames > 0 |
| 두 샘플 upgrade/remove/reinstall/purge, binary hash 갱신, XDG 데이터 보존 | PASS | 두 acceptance의 `lifecycle.json` 개별 필드 및 `*-data.log` |
| fresh NuGet feed/cache의 template restore, 폴더 게시/portable 설치 | PASS | `nuget-consumer-xcb/`, `nuget-resume.log` |
| Doroti 저장소를 namespace에서 제외한 NuGet provider의 v1/v2 `.deb` 게시 | PASS | `nuget-no-source.log`, `nuget-no-source-v2.log`, `nuget-consumer-xcb/deb-no-source*/` |
| NuGet 소비자의 clean APT 설치/xcb 실행/업데이트/remove/purge/데이터 보존 | PASS | `nuget-acceptance/lifecycle.json` |
| 기존 typed extension의 연결/취소/Stop 소유권 회귀 | PASS | `tool-extension-regression.log` |
| 기존 Qt 6.10.2 폴더 게시 및 WebView/Wayland 실행 | PASS | `host-regression.json`, `host-folder-gui.log` |
| Debian Qt 6.8.2/Mesa 25.0.7 Wayland 종료 | **FAIL** | 두 acceptance의 `wayland-v*.log`, `wayland-gdb.log` |
| 물리 GPU/display, 실제 데스크톱 메뉴 클릭, 물리 입력 | **미검증** | headless/software Vulkan 결과로 대체하지 않음 |

lintian은 패키지 오류를 검사한다. 계획한 third-party `/opt` 배치, Skia/.NET의 embedded codec/compression, native provenance와 진단 심볼 보존에 대해서만 경로가 한정된 사유 있는 override를 포함했다. 나머지 오류는 포장 실패다. `initial-upload-closes-no-bugs`, `no-manual-page` 경고는 생성 changelog/GUI launcher 특성으로 보고서에 그대로 보존했다.

### 남은 지원 승인 조건

- **Wayland 종료 실패를 해결하고 Quick/WebView의 재검증을 통과해야 전체 초기 지원을 완료로 표시할 수 있다.** Vulkan loader 문제는 해결됐지만, Qt 6.8.2/Mesa software Vulkan 경로의 swapchain 종료 중 이미 파괴된 Wayland queue 접근/`wl_display_flush` mutex 대기·abort를 관찰했다. Weston과 host compositor에서 재현됐다. 단발성 성공은 통과로 취급하지 않았다.
- 현재 배포 예제는 xcb를 기본값으로 사용한다. Debian 13의 virtual xcb 실행 증거를 물리 GPU/화면, 실제 메뉴 클릭, 모든 Debian/Ubuntu 조합의 지원으로 확대 해석하지 않는다.
- 실패한 Wayland receipt의 전체 상태는 `failed`로 유지하면서, 독립적으로 통과한 설치/업데이트/remove/purge/데이터 보존/xcb 필드는 보존했다. 릴리스용 전체 승인과 구현 완료를 구분한다.

## 9. Qt 6.12 번들 및 선택적 WebEngine 다운로드 — 2026-10-19

### 확정한 구조

- Qt 6.12 LTS를 기준으로 필요한 런타임을 앱 바이너리와 함께 배포한다. Qt는 공유 라이브러리로 유지한다. 사용자 PC에 Qt SDK를 요구하지 않는다. 기본 동봉 배포에는 별도 Qt 저장소 등록이 필요하지 않다.
- WebEngine은 Framework와 별도로 릴리스를 선택해 **빌드·게시 시 설치하고 앱에 동봉하는 것이 기본**이다. 빌드 옵션으로 **`.deb` 설치 시 WebEngine만 별도 다운로드·설치**하는 배포도 선택할 수 있다. 일반 폴더·portable에는 이 옵션을 허용하지 않는다. Qt Framework는 두 방식 모두 앱에 동봉한다.
- 앱과 Doroti 코드에 GPL 적용을 요구하는 런타임 구성을 피하고, LGPL 선택지가 있는 Qt 모듈을 해당 조건에 따라 사용한다. 동적 링크만으로 모든 재배포 의무가 해결된다고 가정하지 않는다. 별도 다운로드도 라이선스 의무를 회피하는 수단으로 취급하지 않는다.
- Debian 13/Qt 6.8.2 실패에 맞추는 기존 경로 대신 Qt 6.12 번들을 검증한다. Wayland와 xcb를 모두 필수로 하며, Qt 6.10.2의 기존 통과 결과로 Qt 6.12 지원을 선언하지 않는다.
- Qt Framework 전체를 설치하는 별도 APT 저장소는 필요하지 않다. `download`를 선택한 `.deb`만 WebEngine 런타임 패키지를 제공하는 서명 저장소와 최초 등록 절차가 필요하다. 앱을 실행할 때는 네트워크 연결이나 설치 권한을 요구하지 않는다.

### Framework와 WebEngine 버전 계약

Qt 6.12.0은 2026-09-30에 정식 출시됐다. Qt 6.12부터 WebEngine/Pdf는 별도 제품·버전으로 배포되며, 첫 별도 WebEngine 릴리스는 6.140.0이다. 따라서 `Qt == WebEngine` 버전 일치 검사를 사용하지 않는다.

초기 검증 후보는 Framework 6.12.0 + WebEngine 6.140.0이다. 실제 바이너리의 빌드 대상 Qt 버전·플랫폼·아키텍처·필요 라이브러리·helper/QML/resources/locales를 확인한 뒤 호환 조합으로 승인한다. WebEngine과 Chromium의 보안 업데이트는 Framework LTS와 별도로 추적한다. LTS의 장기 상용 지원과 공개 패치 공급 정책도 구분한다.

### 빌드 옵션과 입력

다음 속성 이름과 경로는 구현 계약안이며, 현재 provider가 이미 지원하는 옵션으로 표시하지 않는다. 기존 `DorotiQtWebEngine` 기능 스위치는 유지한다.

| 속성 | 기본/의미 |
| --- | --- |
| `DorotiQtVersion` | 초기 `6.12.0`; 승인한 6.12 LTS 패치 릴리스 |
| `DorotiQtSdkDirectory` | 빌드 환경에 설치한 해당 Qt SDK의 prefix. 빌더에서만 사용 |
| `DorotiQtWebEngine` | 기존 기능 스위치. `false`면 WebEngine 라이브러리·helper·다운로드 설정을 포함하지 않음 |
| `DorotiQtWebEngineVersion` | WebEngine 활성 시 별도 고정 버전. 초기 검증 후보 `6.140.0` |
| `DorotiQtWebEngineSdkDirectory` | 별도 설치한 호환 WebEngine의 prefix. 같은 SDK prefix에 설치한 경우도 출처·버전을 별도로 기록 |
| `DorotiQtWebEngineDeployment` | `bundled` 기본. `download`는 `packageformat=deb`에서만 허용하며 다른 배포 형식과 조합하면 게시 전에 오류 |
| `DorotiQtWebEnginePackageId` | `download`에서 필수인 검증된 WebEngine 런타임 Debian package ID |
| `DorotiQtWebEnginePackageVersion` | `download`에서 필수인 승인된 Debian package 버전. Framework/WebEngine 버전과 별도 기록 |
| WebEngine 저장소 정보 | 게시자의 실제 HTTPS 저장소·서명키·배포판/아키텍처·패키지 해시를 릴리스 자료와 설치 안내에 기록. 실제 URL과 키는 미확정 |

버전·SDK 경로·기능·배포 방식·출처와 재배포 자료를 MSBuild에서 평가해 사용한다. 사용자가 실행할 때 임의의 최신 WebEngine을 고르거나 Framework까지 바꾸지 않는다. `download`에서도 빌더에는 해당 WebEngine이 있어야 shim 빌드·호환성·라이선스·산출물 구성을 검증할 수 있다. WebEngine이 비활성인데 `download`를 지정하거나 `.deb` 외 형식과 조합하면 게시 전에 명확한 오류를 반환한다.

기본 번들 구성 예시:

```xml
<PropertyGroup>
  <DorotiQtVersion>6.12.0</DorotiQtVersion>
  <DorotiQtWebEngine>true</DorotiQtWebEngine>
  <DorotiQtWebEngineVersion>6.140.0</DorotiQtWebEngineVersion>
  <DorotiQtWebEngineDeployment>bundled</DorotiQtWebEngineDeployment>
</PropertyGroup>
```

선택적 다운로드 구성은 `packageformat=deb`, `DorotiQtWebEngineDeployment=download`와 실제 WebEngine runtime package ID·승인 버전을 지정한다. 앱 `.deb`는 해당 패키지를 `Depends`로 선언한다. 패키지와 서명 저장소는 게시자가 준비하며, 이 문서의 예시 버전만으로 호환성 검증 완료를 뜻하지 않는다.

### 게시·로딩·설치 흐름

1. 빌드 환경에서 고정된 Framework SDK와 별도 WebEngine 릴리스를 설치한다. 다운로드 메타데이터·아카이브 출처·해시와 빌드 설정을 보존한다. SDK 설치 단계는 명시적으로 실행하며 일반 프로젝트 평가가 대용량 다운로드를 시작하지 않게 한다.
2. 선택한 SDK로 native shim을 빌드하고 빌드 버전을 manifest에 기록한다. 시스템 Qt를 찾았다는 이유로 6.8/6.10 shim과 6.12 런타임을 섞지 않는다. 명시적 prebuilt shim과 NuGet 소비자에도 같은 검사를 적용한다.
3. 승인된 Qt 모듈·QML import·플랫폼 플러그인·transitive shared library만 수집한다. SDK 디렉터리 전체 복사나 `aqt -m all` 결과 전체 배포는 사용하지 않는다. `bundled`면 WebEngine의 공유 라이브러리·QML·helper·resources·locales·notice도 함께 수집한다.
4. `.deb`의 `download`면 WebEngine 전용 런타임 `.deb`를 별도로 생성한다. 앱에는 승인된 패키지 의존성과 Framework/WebEngine 버전·ABI·필수 파일·재배포 자료 위치를 기록한 manifest를 포함한다. 저장소 게시·서명·호스팅은 게시자의 배포 단계이며, 앱 설치 전에 저장소가 준비되어야 한다.
5. Qt/native shim 로딩 전에 앱 전용 Framework 경로와 선택한 WebEngine 경로를 설정한다. `qt.conf`, ELF의 상대 로더 경로, QML/plugin 경로, helper/resources/locales 경로를 함께 맞춘다. 두 배포 방식에서 동일한 WebView API와 sandbox 정책을 사용한다.
6. 일반 폴더 게시와 `.deb`가 같은 번들 수집·검증 코드를 사용한다. `.deb` staging은 완성된 publish를 재사용하고, Qt/QML 시스템 패키지 대신 실제 외부 OS 의존성만 계산한다. 번들 내부 SONAME을 시스템 Qt 의존성으로 잘못 선언하지 않는다.
7. `apt install ./<app>.deb`는 앱 번들과 OS 런타임을 설치하며, `download`면 WebEngine runtime package도 같은 APT 설치 작업에서 내려받아 설치한다. 의존성 해결은 APT에 맡기고 앱의 maintainer script 안에서 APT나 별도 다운로더를 중첩 실행하지 않는다. 사용자 XDG 데이터와 application ID 유지 계약은 보존한다.
8. Wayland 세션은 Wayland, X11 세션은 xcb를 선택하고 명시적 사용자 설정을 보존한다. 테스트는 Wayland를 강제하고 실제 backend를 기록하여 xcb fallback으로 통과하는 경우를 차단한다.

앱 전용 배치 예시:

```text
publish/
  <app>, libdoroti_qt_host.so, [libdoroti_webview_qt.so]
  qt/framework/6.12.0/{lib,plugins,qml,...}
  qt/webengine/6.140.0/{lib,qml,libexec,resources,translations,...}  # bundled만
  doroti-qt-runtime.json
  licenses/qt/, [licenses/qt-webengine/]
artifacts/
  <app>.deb 또는 폴더 배포물
  [<webengine-runtime-package>_<version>_amd64.deb]  # deb + download만
  대응 소스·notice·빌드/설치/교체 안내 자료
```

실제 레이아웃은 Qt의 경로 탐색과 ELF 검증 결과로 확정한다. Framework와 WebEngine이 공통으로 요구하는 Qt 모듈을 서로 다른 버전으로 중복 배치하지 않는다. 시스템 Qt가 설치되어 있어도 기본 실행이 앱 전용 Qt를 선택해야 한다.

### `.deb` 설치 시 WebEngine 다운로드·설치

| 배포 형식 | `bundled` | `download` |
| --- | --- | --- |
| 일반 폴더·portable | Qt Framework와 활성 WebEngine을 앱과 함께 배포 | 게시 전에 거부 |
| `.deb` | Qt Framework와 활성 WebEngine을 앱 패키지에 동봉 | Framework는 동봉하고 WebEngine runtime package를 APT 의존성으로 설치 |

- 설치 시점은 `apt install ./<app>.deb`다. 앱 실행 시 다운로드·사용자별 엔진 캐시·최초 실행 설치 UI를 두지 않는다. 설치를 마치면 일반 사용자로 네트워크 없이 실행할 수 있어야 한다.
- `download`용 WebEngine runtime package를 실제 HTTPS APT 저장소에 게시한다. 설치 전에 별도 등록 절차로 keyring과 `Signed-By`가 있는 source를 준비한다. `Depends`에는 패키지 ID와 승인 버전을 선언하며 임의의 다운로드 URL을 쓰지 않는다.
- APT는 저장소 릴리스 서명과 인덱스에 기록된 패키지 해시를 확인하고 의존성을 설치한다. 이미 호환 패키지가 설치되어 있으면 재사용한다. `dpkg -i`만으로는 원격 의존성 다운로드가 제공되지 않으므로 기본 설치 안내는 APT를 사용한다.
- WebEngine runtime package는 공유 라이브러리·QML·helper·resources·locales·라이선스·대응 소스 안내를 제공하고, 설치 파일은 dpkg가 소유한다. 앱에 동봉한 Framework와 함께 실행하는 전용 버전 경로를 사용하며 Debian 기본 Qt를 교체하거나 시스템 Qt 6.8에 연결하지 않는다.
- Framework가 앱에 동봉되어 있으므로 엔진 패키지의 Qt 의존성을 배포판 `libqt6*`로 잘못 생성하지 않는다. 소비 앱이 제공하는 Framework 버전·ABI와 엔진의 요구사항을 빌드 manifest·패키지 계약·로더에서 대조하고, 엔진의 실제 외부 OS 의존성은 패키지에 선언한다.
- 엔진과 Framework가 여러 앱에서 공존할 수 있도록 package ID·버전별 prefix·파일 소유권·호환 조합을 확정한다. 처음에는 승인한 정확한 버전을 요구하고, 후속 보안 패치는 검증한 범위에서 의존성 정책을 갱신한다.
- 저장소 미등록·네트워크 단절·서명/해시 오류·후보 버전 없음은 APT 설치 실패로 드러나야 한다. 불완전한 상태를 설치 성공으로 표시하거나 앱 첫 실행으로 다운로드를 미루지 않는다. 시스템 엔진으로 fallback하거나 sandbox를 끄지 않는다.
- 오프라인 설치가 필요하면 engine `.deb`와 app `.deb`를 함께 제공해 `apt install ./<engine>.deb ./<app>.deb`로 설치할 수 있게 한다. 일반 실행에는 네트워크가 필요하지 않다.
- 앱 remove/purge는 해당 앱 소유 파일만 처리하고 사용자 데이터를 유지한다. 공유 엔진은 다른 앱의 의존성과 APT의 `autoremove` 정책에 따라 관리하며, 앱 제거 스크립트에서 직접 삭제하지 않는다.
- APT 패키지의 초기 출처·전송 무결성 검사와 LGPL 라이브러리 교체 권리를 구분한다. 실행 시 원본 해시·서명만 강제하여 수정한 호환 라이브러리 실행을 막지 않으며, 명시적 사용자 런타임 경로와 교체 안내를 제공한다.

### LGPL 기반 재배포 계약

목표는 앱/Doroti 코드에 GPL 적용을 요구하는 런타임 의존성을 포함하지 않는 것이다. 이는 라이선스 이름 검색이나 동적 링크 여부만으로 자동 보장할 수 없으므로 실제 배포 파일과 빌드 구성을 기준으로 확인한다.

- Qt Core/Gui/Widgets/Qml/Quick/QuickControls2 및 필요한 지원 모듈은 해당 버전의 LGPL 선택지를 확인하고 공유 라이브러리로 링크한다. WebEngine은 Qt 부분의 LGPLv3 선택지와 Chromium/FFmpeg 등 third-party 조건을 함께 확인한다.
- GPL 전용 Qt 런타임 모듈과 미검토 런타임을 포함하면 빌드·게시를 실패시킨다. Qt 6.12의 Canvas Painter, Graphs, Quick 3D, Virtual Keyboard, Wayland Compositor 등의 GPL 전용 모듈을 기본 번들에 넣지 않는다. Wayland **클라이언트**와 Compositor를 구분한다.
- CMake 링크 그래프 검사 외에도 최종 ELF 의존성, QML import/plugin, 동적 로딩 플러그인, codec와 transitive 라이브러리를 검사한다. WebEngine의 FFmpeg/codec 설정에 GPL 구성요소가 들어가는 빌드는 승인하지 않는다. 공식 바이너리도 실제 모듈·SBOM·빌드 정보·라이선스를 확인한다.
- Qt 공유 라이브러리를 교체·재링크하고 수정한 호환 버전으로 앱을 실행할 수 있게 한다. 이를 막는 무조건적인 원본 해시 강제, 서명 제한, 약관상의 관련 reverse engineering 금지 등을 두지 않는다. Skia 등 별도 자산의 기존 provenance 검사와 Qt 교체 경로를 구분한다.
- 정확한 배포 바이너리에 대응하는 소스, Qt 수정사항/patch, 필요한 빌드 스크립트·설치 정보와 제3자 소스 의무를 충족하는 제공 방식을 산출물에 연결한다. `download.qt.io` 일반 링크 하나를 대응 소스 제공 완료로 처리하지 않는다. 필수 재배포 자료가 빠지면 게시를 실패시킨다.
- LGPL/GPL 참조 라이선스 전문, 저작권·third-party notice, 사용 모듈·버전·출처·선택한 라이선스, 라이브러리 교체 방법을 함께 제공한다. LGPL이 참조하는 `GPL-3.txt` 문서를 삭제하는 것이 GPL 적용 회피는 아니다.
- 빌드 도구의 GPL 및 출력물 예외와 앱에 링크/동봉하는 런타임의 라이선스를 구분한다. 도구를 실행했다는 이유만으로 앱을 GPL로 분류하지 않으며, 빌드 전용 SDK 도구·정적 아카이브·헤더를 런타임에 함께 넣지 않는다.
- 다운로드 방식에서도 동일한 notice·대응 소스·교체 안내를 제공한다. 별도 파일·프로세스·서버로 옮겼다는 사실만으로 라이선스 의무가 사라진다고 가정하지 않는다.

### 실제 코드 변경 지점과 완료 조건

- [ ] SDK 설치 작업: Framework 6.12.0과 별도 WebEngine의 고정 버전·출처·해시·모듈을 설치하고 재사용한다. SDK 수집과 사용자용 런타임 수집을 분리한다.
- [ ] `packages/platforms/build/Doroti.Qt.targets`: 옵션 기본값·검증·SDK prefix 전달·native 출력 구분·게시 번들 수집 연결을 구현한다. WebEngine 미사용 앱에는 엔진 SDK를 요구하지 않는다.
- [ ] `packages/platforms/qt/native/CMakeLists.txt`: 선택된 Qt 6.12 및 별도 WebEngine으로 링크하고 Framework/WebEngine 버전·ABI·배포 모드를 manifest에 기록한다. Qt 번들 상대 로딩 경로와 prebuilt 일치를 확인한다.
- [ ] `native/cmake/DorotiLicensePolicy.cmake`와 번들 수집기: 6.12/호환 WebEngine의 LGPL 가능한 모듈, QML/플러그인과 최종 ELF 의존성·notice·대응 소스를 검사한다. 기존 타깃 allowlist만으로 배포 승인을 내리지 않는다.
- [ ] `packages/platforms/qt/tooling/linux_deb.py`: Qt 동봉 금지·system-distribution 고정·Qt/QML APT 의존성을 제거하고, 번들 manifest·배포 방식·외부 OS 의존성과 새 launcher를 검증한다.
- [ ] WebEngine runtime `.deb`와 서명 APT 저장소를 준비한다. 실제 공급 URL·키·패키지 소유권·버전 계약을 확정하고 앱 설치 전에 등록할 절차를 제공한다.
- [ ] runtime 경로/로더: Qt/native shim을 로드하기 전에 동봉 또는 APT로 설치된 WebEngine 경로를 선택한다. 앱 실행 중 다운로드 코드를 추가하지 않으며, 패키지 누락/ABI 오류·sandbox 동작·사용자 지정 수정 런타임 경로를 검증한다.
- [ ] NuGet 패키지에 번들 도구·runtime manifest·엔진 `.deb` 생성/의존성 처리에 필요한 파일을 포함하고 저장소 소스 없는 소비자도 게시할 수 있게 한다.
- [ ] Qt/SDK 없는 깨끗한 Debian 13에서 Quick, WebView bundled, WebView download 각각의 설치·Wayland/xcb 반복 실행·정상 종료·업데이트/remove/purge·데이터 보존을 검증한다.
- [ ] `download`의 `.deb` 한정 조합 검증, 저장소 등록·서명/해시·버전 의존성·기설치 엔진 재사용·설치 실패/복구·앱/엔진 동시 업데이트·오프라인 설치와 실행·공유 엔진 제거 정책을 검증하고, 수정한 호환 LGPL 라이브러리로 실제 실행한다.
- [ ] 기존 일반 폴더 게시·portable 설치·NuGet 소비자·개발 실행·단일 파일 기능의 영향을 확인하고 지원 조합 또는 명시적 오류를 정한다. 단일 파일에 Qt를 교체 불가능하게 숨기지 않는다.
- [ ] 앱 `.deb`, 별도 엔진 `.deb`와 서명 저장소/등록 안내, 소스·재배포 자료, 두 배포 방식의 사용자 안내를 갱신한다. receipt에는 Qt/WebEngine의 개별 버전·원본/설치 payload 해시·배포 방식·출처·검증 범위를 기록한다.

### 조사한 공급원과 공식 근거

KDE neon은 Ubuntu 기반이며 Savoury Qt 6.2 PPA는 이번 버전에 맞지 않는다. Debian 13 기본 Qt는 6.8.2이고, 조사 당시 Qt Base testing/sid도 6.11.2였다. 따라서 해당 저장소들을 Debian 13 사용자 PC의 Qt 6.12 설치 수단으로 사용하지 않는다. 커뮤니티 설치기 aqtinstall은 빌더가 Qt 공식 다운로드를 가져오는 후보이며, 사용자에게 설치하도록 요구하는 APT 저장소가 아니다.

- [Qt 6.12 LTS 정식 발표](https://www.qt.io/blog/qt-6.12-released)
- [Qt WebEngine 별도 릴리스와 호환성](https://www.qt.io/blog/qt-webengine-will-release-separate)
- [Qt 6.12 라이선스와 GPL 전용 모듈](https://doc.qt.io/qt-6.12/licensing.html)
- [Qt Quick Controls LGPL 선택지](https://doc.qt.io/qt-6.12/qtquickcontrols-index.html#license-and-attributions)
- [Qt WebEngine 6.140 라이선스](https://doc-snapshots.qt.io/qtwebengine/qtwebengine-licensing.html)
- [Qt의 LGPL 의무 안내](https://www.qt.io/development/open-source-lgpl-obligations)
- [Qt LTS 유지보수 안내](https://www.qt.io/development/qt-framework/qt-lts)
- [aqtinstall](https://github.com/miurahr/aqtinstall)
- [Qt Framework Linux 6.12.0 메타데이터](https://download.qt.io/online/qtsdkrepository/linux_x64/desktop/qt6_6120/qt6_6120/Updates.xml)
- [WebEngine 6.140용 Linux 다운로드 경로](https://download.qt.io/online/qtsdkrepository/linux_x64/extensions/qtwebengine/6120/61400/)

- [Debian 패키지 의존성 선언](https://www.debian.org/doc/debian-policy/ch-relationships.html)
- [APT 로컬 패키지 설치와 의존성 해결](https://www.debian.org/doc/manuals/debian-reference/ch02.en.html)
