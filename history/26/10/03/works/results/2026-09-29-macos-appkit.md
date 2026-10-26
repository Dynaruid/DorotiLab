# macOS / AppKit 후속 구현·검증 — 2026-09-29

작업 상태: **PARTIAL / 실행 범위별 PASS**. `work2.md`의 macOS 항목을 대상으로 한다.
Mac Catalyst/iOS 및 물리 입력·VoiceOver·다른 모니터·clean OS 결과로 확대하지 않는다.
기준 revision: `b0cf692e394952fc687eba3cae2bd261b7caa0af` + 이번 작업 트리.

## 환경과 실행

- macOS 26.6.2 (25G83), Apple M1 8-core GPU, 내장 Retina 2560×1600 / DPR 2.
- .NET SDK 10.0.400, Xcode 27.0 (27A266a), macOS SDK pack 27.0.10539-xcode27.0.
- `net10.0-macos27.0`, `osx-arm64`, AppKit/MTKView, Graphite Metal와 Ganesh Metal을 분리한다.
- 기본 `net10.0-macos` SDK 26.5는 Xcode 26.6을 요구하여 이 장비에서 거절됐다. 기존 Xcode 27 프로필로 전환했으며 SDK 버전 검사를 우회하지 않았다.
- `tart`/`prlctl`/`vmrun`/`qemu-system-aarch64` 실행기를 이번 PATH에서 찾지 못했으며 clean macOS 환경은 실행하지 않았다.
- `security find-identity -v -p codesigning`에서 Apple Development 인증서 1개를 확인했다. Developer ID 배포 서명·공증 통과를 의미하지 않는다.
- 모든 테스트는 `Doroti/eng/run-with-timeout.py --timeout 1200`으로 실행한다. renderer/lifetime별 resize는 12회로 제한했다.

```sh
python3 Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build samples/DorotiTestbedApp/macos/DorotiTestbedApp.MacOS.csproj -c Debug -r osx-arm64 -p:DorotiMacOSTargetFramework=net10.0-macos27.0
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/tests/macos_smoke.py --skip-build --output temp/testing/macos-work2/graphite --renderer graphite
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/tests/macos_smoke.py --skip-build --output temp/testing/macos-work2/ganesh --renderer ganesh
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/eng/release-candidate.py --targets macos --macos-tfm net10.0-macos27.0
```

`validate.ps1 -Suite MacOSSmoke`도 동일한 기본 Graphite 경로를 제공한다.
자동화 코드는 testbed의 자기 프로세스 창만 제어하며 물리 입력을 합격 처리하지 않는다.

## 구현과 발견한 결함

- **Desktop 추가 창:** 실제 NSWindow와 창별 MAUI surface/session/dispatcher/PlatformView factory를 생성한다. application boundary는 reference counting으로 공유하고 main 창 종료 후에도 survivor의 공통 plugin/resource가 유지된다. 마지막 창 정책과 Explicit 종료를 연결했다.
- **수명 수정:** 네이티브 편집기 포함 창을 닫으면 view 해제 뒤 widget unmount가 frame capability를 요구해 실패했다. widget shutdown → GPU retirement 대기 → capability/native 창 종료로 수정했다. AppKit `WillTerminate`의 호출 불가능한 base 구현 호출도 제거했다.
- **SDK 호환성:** `CVPixelBufferAttributes`에 없는 Metal/IOSurface convenience 속성을 지원되는 CoreVideo dictionary key로 대체했다.
- **파일 선택:** NSOpenPanel sheet를 요청 창에 연결했다. 요청 취소·동시 요청 거절·view 종료 취소·security-scoped URL·bounded random-access read와 grant dispose를 제공한다.
- **URL/activation:** NSWorkspace URL 실행, app delegate `OpenUrls`, 시작 인자 URL, main-owned activation/복원을 연결했다. sample plist에 `doroti-testbed` scheme을 등록했다. 추가 창은 독립 navigation을 갖되 앱 URL 수신·동일 restoration namespace를 중복 소유하지 않는다.
- **Drop:** 실제 NSView drag destination에서 파일/텍스트/URI의 Copy 협상과 view-local 좌표를 연결했다. 수신 payload의 native 파일 접근은 지연·bounded read이며 dispose/view 종료로 해제한다. native child editor/WebView는 자체 drop 경계를 유지한다. view 종료 후 대기 중인 native 전달이 disposed dispatcher에 진입하지 않도록 차단했다. **CanSend=false, Move/Link/virtual-file 미지원**을 명시한다.
- **진단:** GPU command buffer 완료와 별도로 `MTLDrawable.PresentedTime` 기반 최근 30개 실제 표시 간격, 표시 drawable 수, `MTLDevice.CurrentAllocatedSize`를 기록한다. 장치 할당량은 창 전용 VRAM이 아니며 M1 unified memory의 Metal 자원 수치다.
- **빌드·출시 도구:** macOS 전용 smoke suite, macOS target feed/외부 template consumer/Release app 실행·서명 검증 경로를 추가했다. macOS SDK는 `PublishTrimmed=true`를 요구하므로 비트리밍 후보는 `LinkMode=None`으로 표현한다.

API 근거: [AppKit URL 수신](https://developer.apple.com/documentation/appkit/nsapplicationdelegate/application(_:open:)),
[Metal 실제 표시 callback](https://developer.apple.com/documentation/metal/mtldrawable/addpresentedhandler(_:)),
[Metal 장치 자원 할당량](https://learn.microsoft.com/en-us/dotnet/api/metal.imtldevice.currentallocatedsize).

## 검증 결과

| 범위 | 확인한 결과 | 증거 경계 |
| --- | --- | --- |
| Debug 빌드 | Xcode 27 프로필, 경고/오류 0 | build |
| Graphite 두 창 | 450×800 / 560×650, DPR 2, 서로 다른 native window ID, 두 실제 창 캡처 | 자동 native API + 화면 |
| Graphite survivor | 첫 창 native close 후 두 번째 창 12회 resize, GPU 완료 증가, GPU error 0 | 자동 native API |
| 종료 정책 | OnLastWindowClosed 종료, Explicit에서는 0개 창 이후 프로세스 유지 및 명시적 종료 | 자동 native API |
| Desktop | 첫 표시 지연·크기·focus·숨김·최소화/복원·최대화·fullscreen·appearance·close 취소/허용 | 자동 native API; 물리 live resize와 구분 |
| 파일 선택 | pre-cancel·표시된 sheet 취소·owner dispose 취소, 128 MiB 파일 끝 offset 읽기와 해제 후 거절 | native panel/API, 물리 picker 선택 미검증 |
| Drop | native NSPasteboard의 한글 text/file URI/128 MiB grant 해석 | native pasteboard; Finder 교차 창 전달 미검증 |
| PlatformView | native editor 2회, WKWebView 2회 생성/재생성 후 제거 | 자동 native lifetime |
| Navigation | cold 인자 URL → OS LaunchServices warm URL → 종료·재시작 후 route 복원 | OS warm 전달, 실제 Router 기록 |
| 공통 CPU | widget/DPR/blur geometry/list/Dialog/한글 합성/복원/동시 owner 격리·layer 해제 | CPU offscreen; GPU golden 아님 |
| 플러그인 공통 | 공유 handler 수명, 취소/권한 결과/늦은 응답/큰 파일 offset/event bounded stream | 공통 계약 |
| Source | 링크·CLI timeout/실패 전파·portable installer 계약 | 자동화 |

**Ganesh도 위 native smoke 전체 PASS**다. Graphite는 계측 추가 후 두 창/종료/resize를 재실행했고, 최종 파일 grant 보강 뒤에는 sheet 취소·128 MiB 읽기·디렉터리 거절·자기 fixture의 POSIX 읽기 권한 거절을 추가로 통과했다. 보안 권한 대화상자/TCC 거절과 POSIX 파일 권한 거절은 구분한다.

### Metal 계측 수치

별도 `--cases rendering` 실행: `HotReloadSample`의 리스트 장면을 24회 API resize한 뒤 수집했다.
간격은 `PresentedTime`의 양수 표본만 사용하며 p95는 nearest-rank다.
이는 정적 장면에 테스트 드라이버가 resize를 요청한 간격이 포함된 값으로 **60 Hz 부하 합격/FPS 수치가 아니다**.

| renderer | 실제 표시 callback 수 | 최근 유효 간격 n | 중앙값 ms | p95 ms | Metal 장치 할당 byte | GPU command error |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Graphite | 33 | 30 | 16.666 | 49.997 | 145,686,528 | 0 |
| Ganesh | 23 | 22 | 66.663 | 66.663 | 159,350,784 | 0 |

native editor를 포함한 transaction 합성 장면에서는 일부 실행의 표시 간격 표본이 0~1개뿐이었다.
그 실행은 p95/FPS **notMeasured**다. callback이 없다는 사실을 표시 실패나 0 ms로 바꾸지 않는다.
Metal 장치 할당은 프로세스의 다른 창/renderer 자원을 포함하므로 창별 VRAM/누수 합격으로 쓰지 않는다.
CPU 공통 회귀의 DPR 1/1.25/2 기하·레이어 해제는 별도 offscreen 결과이며 GPU 영상/blur 품질을 뜻하지 않는다.

## 최종 Release 후보와 로컬 배치

선택한 최종 후보 경로는 `Doroti/artifacts/release/0.3.0-beta.rc.20260929071216-final/`이다.
NuGet 버전은 `0.3.0-beta.rc.20260929071216`, configuration은 Release/CoreCLR,
TFM/RID는 `net10.0-macos27.0` / `osx-arm64`다. **외부 공개/push는 하지 않았다.**
같은 미공개 버전의 중간 로컬 후보는 폐기하고 `-final` 경로의 manifest/hash를 기준으로 삼는다.

- 25개 NuGet 패키지와 281개 macOS payload 파일의 SHA-256을 manifest와 다시 대조: **PASS**.
- sourceTreeSha256(생성 시작 시점): `21d8cdef4a930312acd1ca965d12d78b422b70414210af3942b95e3cd4371e8a`.
- `CandidateApp.MacOS-0.3.0.pkg` SHA-256: `014f8c78e5d865e0350f5b11b9109d7032618ab0d0b1819373c2b715318b1992`.
- 격리 NuGet/HTTP cache의 template 소비 앱 restore/publish → `.pkg` 확장 → 추출한 `.app`의 실제 첫 표시·추가 창·resize·종료: **PASS**. 명시적 완료 marker도 확인했다.
- `codesign --verify --deep --strict`: **PASS (ad-hoc app 서명)**. Developer ID/서명된 installer/notarization을 뜻하지 않는다.
- `macos_package_smoke.py`: 이전 `0.3.0-beta.rc.20260929070616`에서 최종 후보로 로컬 추출 앱을 설치/교체/실행/제거하고 한글 userdata/selection fixture를 보존: **PASS**. 빈 NuGet cache로 각 앱을 실행했다.
- 이 fixture는 macOS Installer의 시스템 설치/등록이나 실제 앱 입력 상태 복원 검증이 아니다. clean OS/Gatekeeper·실제 배포 업데이트는 별도 잔여다.

```sh
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/tests/macos_package_smoke.py --candidate Doroti/artifacts/release/0.3.0-beta.rc.20260929071216-final --output temp/testing/macos-work2/package-reinstall
```

위 명령은 이전 후보를 생략한 동일 버전 재설치 재현이다. 서로 다른 두 버전의 업데이트는
`--previous`에 별도로 보존한 후보 디렉터리를 전달한다. 최종 manifest와 toolchain 기록은 후보 폴더에 보존한다.

## 남은 전체 범위

- 실제 한글 IME/caret/selection/후보창·Tab/Shift+Tab·clipboard·VoiceOver 탐색과 action은 **notVerified**. 이번 화면/NSView/semantics 증거로 대체하지 않는다.
- mixed-monitor DPI·연속 물리 live resize·native gesture 경쟁·전체 clip/transform/z-order·device loss·pause/resume·장기 메모리 누수는 **notVerified**.
- blur/영상/효과/폰트 CDN·offline·초기 교체의 전체 GPU golden과 60 Hz/VRAM 성능 예산 합격은 **notMeasured/notVerified**. resize 표시 간격을 지속 애니메이션 FPS로 환산하지 않는다.
- Finder 실제 교차 창 drop·TCC/선택 권한 대화상자 거절·물리 picker 선택·native event source qualification은 **notVerified**. OS drag 송신·drag 이미지·Move/Link·virtual-file은 **unsupported**, 추가 구현이 남는다.
- OS cold URL 배달(인자 경로와 구분), 앱별 schema migration·사용자 입력/selection·강제 종료 복원은 **notVerified**.
- `_window_macos.cs`의 Flutter Satellite 계층은 `Doroti.Desktop` 창 manager에 연결하지 않는다. owner/modal/satellite/popup/tooltip·창 간 이동은 **unsupported**. 새 추가 창 API는 `Doroti.Desktop`만 사용한다.
- NativeAOT는 기존 `DOROTIAOT002` 정책상 macOS 미지원이다. full renderer trimming·Developer ID 서명·notarization/Gatekeeper·clean OS 설치/업데이트/제거는 **notVerified**.
- macOS VS Code 통합/Hot Reload는 원래 계획의 후속 후보로 유지한다. CI workflow를 새로 만들지 않았다.

## 조사 기록·정리

초기 오류: 기본 SDK/Xcode 불일치, CoreVideo convenience property 부재, 추가 창 capability의 기본 최대값 1,
widget teardown 순서, AppKit delegate base 호출, macOS의 `PublishTrimmed=false` 거절 및 publish 결과가 `.app` 대신 `.pkg`인 경로를 확인하고 수정했다.
공통 plugin/drop 테스트를 동일 Debug 출력에 동시 build한 한 실행은 deps.json 파일 경쟁으로 실패하여 순차 재실행했다.
가려진 Metal 창의 초기 캡처 일부는 renderer 내용이 비어 있었다. 각 자기 창을 전면으로 가져온 뒤 Graphite/Ganesh 각각 두 창을 다시 캡처하여 전체 framework/native 내용과 제목을 직접 확인했다. 가려진 창 캡처를 GPU golden으로 쓰지 않는다.

화면 캡처·로그·테스트 파일·소비 앱은 `temp/testing/macos-work2/` 및 이번 실행의
`temp/testing/release-candidate/` 아래에만 저장했다. 최종 요약 후 정리 상태를 아래에 명시한다.


최종 정리: 위의 결과 요약과 유지보수할 smoke 소스만 추적한다. `temp/testing/macos-work2/`의
원시 로그·캡처·로컬 설치 fixture 및 이번 release-candidate 소비 앱/cache/log는 정리했다.
이번에 만든 중간 후보(`20260929065009`, `20260929065740`, `20260929070616`,
`20260929071216` 기본/`-verified` 경로)도 폐기했다. `20260929071216-final` 후보만 보존한다.
앱의 테스트별 restoration checkpoint는 앱 데이터 영역의 별도 smoke namespace이며, 기존 사용자
namespace를 덮어쓰거나 사용자 앱 데이터를 일괄 삭제하지 않았다. 제품 bin/obj는 일반 빌드 산출물이다.
