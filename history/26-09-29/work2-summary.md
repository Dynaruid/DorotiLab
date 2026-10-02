# works 미완료 작업 검토 요약 — work2.md

원문 검토일: **2026-09-29** · 후속 반영: **2026-09-29~2026-10-01** · 보관일: **2026-10-03**
검토 기준 HEAD: `4b46e2c927339dfad4c728087c9545cda5dc5e2f`

공통 00~10·플랫폼 7개·지원표의 완료 부분과 잔여를 정리한 루트 `work2.md`의 요약이다. **M0 로컬 기반 완료, M1~M7 PARTIAL**을 보존한다. 보관 자체는 새 제품 검증이 아니며, 당시 상세 상태는 [works 보관 인덱스](../26-10-03/works/README.md)에 남겼다. 이후 지원 현황은 [지원표](../../Doroti/docs/support-status.md)를 따른다. 초기 진행 순서는 [plan 요약](../26-09-28/plan-summary.md)에 있다.

## 1. 판정과 기존 구현 경계

잔여를 추가 구현, 검증 잔여, 문서·정리, 후속/미지원으로 구분했다. 검증 잔여는 실패 확정이나 전체 미구현을 뜻하지 않는다. Windows Desktop·테스트 runtime·Windows/Web Hot Reload·Windows 플러그인/Copy drop/추가 창·Windows/Web/Android Navigation은 기존 구현을 활용한다.

## 2. 공통 작업별 확보 범위와 잔여

| 문서 | 기록된 확보 범위 | 주요 잔여 |
| --- | --- | --- |
| [00 기반](../26-10-03/works/common/00-foundation.md) | CLI 실패 전파·timeout·임시 경로·로컬 suite·CI 구성 이력 | 원격 CI 실행·clean checkout/OS·정리 보류 자료의 존재/보존 확인 |
| [01 테스트](../26-10-03/works/common/01-testing.md) | WidgetTester·가상 clock·pointer/합성 한글·CPU ListView/blur/Dialog·독립 owner-thread·패키지 소비 | GPU golden·전체 native/texture 자원 격리·폰트/DPR/renderer별 기준·실패 산출물 |
| [02 Desktop](../26-10-03/works/common/02-desktop-contract.md) | Windows App SDK 창 상태·close·추가 창·Explicit 종료 | 실제 live resize·mixed-monitor DPI·focus/appearance·다른 adapter 정합 |
| [03 입력·접근성](../26-10-03/works/common/03-input-accessibility-platformview.md) | 입력 장면·합성 조합·해제·Windows editor/WebView 각각 재생성 | 물리 IME·Tab/focus·UIA/TalkBack/VoiceOver/Orca·복수 owner/합성·MAUI 연결 |
| [04 렌더링](../26-10-03/works/common/04-rendering-lifetime.md) | CPU geometry/list/layer·Web admission·stale native scene·VariableBlur 진단 | GPU 품질·실제 present/VRAM 예산·모바일 memory/loss·전체 폰트 정책 |
| [05 VS Code](../26-10-03/works/common/05-vscode-hot-reload.md) | VSIX·snippet/import·metadata Hot Reload/상태 보존, 후속 iOS/AppKit/Catalyst | 실제 마법사/C# 서비스/snippet UI·Restart/종료 경합·Windows/Web 최신 변경 후 통합 재검증 |
| [06 플러그인](../26-10-03/works/common/06-plugin-sdk.md) | 공통 SDK·Windows FilePicker/URL·NuGet 소비·취소/수명·공개 event stream | 실제 native event source qualification·목표 adapter/권한/파일 수명·publish/trimming/AOT 조합 |
| [07 OS 드롭](../26-10-03/works/common/07-os-drag-drop.md) | Windows Copy 수신·큰 파일·OLE fixture, 후속 다른 호스트 연결 | Explorer/Finder/파일 관리자 실제 전달·송신·drag 이미지·Move/Link·virtual file·다중 창 |
| [08 Navigation](../26-10-03/works/common/08-navigation-restoration.md) | 공통·Windows·Web main-owned·Android 및 후속 native activation/복원 | 실제 browser back/forward/bfcache·OS 링크·schema migration·입력/selection 복원·최종 APK 재설치 |
| [09 멀티윈도우](../26-10-03/works/common/09-multiwindow.md) | 창 문맥 ADR·Windows 실제 두 창·native editor island·수명, 후속 AppKit/Qt/Catalyst | 물리 IME/DPI·전체 native content·다른 창의 지속 입력/표시·Satellite 소유권 |
| [10 출시](../26-10-03/works/common/10-release-packaging.md) | 선택 플랫폼 package-only 후보·publish·portable 설치/업데이트 | 최신 수정 포함 새 후보·clean OS·서명/실제 배포·지원 runtime 조합·장기 회귀 |

GPU 완료나 CPU 장면의 시간을 표시 FPS로 사용하지 않는다. Windows editor와 WebView의 개별 재생성 PASS는 같은 프레임 혼합 지원을 뜻하지 않는다. 같은 thread의 nested tester는 거절하는 계약을 유지한다.

## 3. 플랫폼별 후속 결과

- **Web·Windows·Android:** [2026-09-29 결과](../26-10-03/works/results/2026-09-29-web-windows-android.md)는 전체 **PARTIAL / 범위별 PASS**다. CI workflow는 사용자 지시로 삭제 상태를 유지한다. 원문의 원격 CI 잔여를 workflow 재생성 지시로 해석하지 않는다.
- **iOS/Mac Catalyst:** [후속 결과](../26-10-03/works/results/2026-09-29-ios-catalyst.md)에 UIKit 서비스·Catalyst PlatformView/추가 scene·activation·복원·package-only 기동을 기록했다. 개발 provisioning profile 발급과 iPhone 12 source Debug 설치·화면 확인도 수행했다. 물리 입력·VoiceOver·OS 링크/드롭·GPU 예산·배포는 잔여다.
- **macOS/AppKit:** [후속 결과](../26-10-03/works/results/2026-09-29-macos-appkit.md)에 실제 추가 창·Graphite/Ganesh·Desktop·native 재생성·picker 취소·Copy drop·activation/복원·출시 후보 경로를 기록했다. 물리 IME/VoiceOver·Finder 전달/송신·전체 GPU 예산·clean 배포는 남았다.
- **Linux/Qt:** [후속 결과](../26-10-03/works/results/2026-09-29-linux-qt.md)에 실제 추가 창·FilePicker/URL·Copy drop/송신 취소·activation/복원·양 QPA·Widgets 비교·package-only/portable 설치를 기록했다. 물리 GPU/IME/Orca·clean OS/서명은 남았다.

기존 iOS/Catalyst `TODO`는 당시 새 실행 결과 부재의 기록이며, 후속 **PARTIAL / 범위별 PASS**로 갱신했다. AppKit/Catalyst·App SDK/MAUI·Qt Quick/Widgets·simulator/실기기·GPU backend와 build mode는 각각 구분한다.

## 4. 2026-09-30~10-01 개발 세션과 렌더링 후속

- **iOS Hot Reload:** simulator의 실제 metadata delta·State/입력/스크롤 보존·오류 복구·rude edit 유지·설치 VSIX Run/Hot Reload/Stop PASS. 실기기 USB는 .NET 11 CoreCLR 개발 프로필, .NET 10 Mono는 같은 네트워크 경로로 각각 검증했다. **.NET 10 USB-only 제한**은 유지한다. [iOS 기록](../26-10-03/works/platforms/ios.md#2026-09-30-ios-metadata-hot-reload)
- **AppKit/Catalyst Hot Reload:** 실제 두 앱과 설치 VSIX에서 코드 반영·상태 보존·Stop, CLI 컴파일 오류 복구·rude edit 유지 PASS. [공통 기록](../26-10-03/works/common/05-vscode-hot-reload.md#2026-10-01-appkitmac-catalyst-개발-세션)
- **VariableBlur:** 캡처/패스/Surface 풀 진단과 iPhone 반복 스크롤 진입점을 추가했다. [P0 결과](../26-10-03/works/results/2026-09-30-variable-blur.md)를 GPU 품질·장기 수명 전체 통과로 확대하지 않는다. 이후 Fixed 계획은 [work3 요약](../26-10-01/work3-summary.md)에 보관했다.

Web Hot Reload의 기존 PASS는 **desktop Chrome / main-owned threaded Debug / WebGL·WebGPU** 범위다. Release/AOT·worker-owned·다른 브라우저·물리 모바일에 확대하지 않는다.

## 5. 문서 정합과 권장 후속 순서

2026-09-29에는 공통/플랫폼 체크리스트의 구현과 검증 잔여, Android intent·route 복원, CPU tester와 native 격리 경계, Explicit/추가 창의 과거 미지원 문구, 지원표·release 안내·임시 경로를 맞췄다. 이 정합 작업은 완료했으나 제품 인수 판정을 새로 바꾸지 않았다.

권장 순서는 원격 CI/clean checkout·정리 보류 확인 → Windows/Web 실제 화면·입력·브라우저/VSIX·2창 → Android 최종 APK 재설치/실기기 → 플러그인·드롭·activation 확장 → 준비된 다른 native 플랫폼 → 선택한 출시 범위다. 드롭과 Navigation은 필수 선후 의존성이 없다.

기존 `0.3.0-beta.rc.20260929010529`에 후속 installer·종료·복원 보강이 포함됐다고 간주하지 않는다. 새로운 후보의 package/payload hash·revision·toolchain·실행 결과를 일치시켜야 한다. 원문 당시의 장비/도구 부재를 현재에도 확인된 사실로 단정하지 않는다.

## 6. 범위 제외와 검증·증거 보존

Preview/Inspector/별도 LSP·시각적 디자이너·CodeLens·multi-root/다중 앱 IDE·Android 자동 검색·원격/브라우저 전용 VS Code는 별도 후속이다. Web/Android/iOS 추가 창, 모든 플러그인·Flutter 테스트 일괄 이식, 무차별 `NotImplementedException` 제거도 필수 범위로 확대하지 않는다.

원문은 **1,200초 timeout / 일반 반복 30회 이내**를 요구했다. 자동/offscreen/실제 화면/물리 입력/보조기술을 구분하고 미검증은 `notVerified`, 미측정은 `notMeasured`, 의도적 제한은 `unsupported`로 남긴다. `temp/testing/`·`Doroti/artifacts/` raw는 삭제 가능한 자료이며 현재 존재를 보장하지 않는다. 보관으로 원래 PARTIAL을 해제하지 않았다.
