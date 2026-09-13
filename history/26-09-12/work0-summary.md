# work0.md — 공식 SkiaSharp Graphite 전환 작업 요약

정리일: 2026-09-13. 사용자 지정 경로 `history/26-09-12`에 원문의 2026-09-11~13 작업과 최신 iPhone 회전 수정까지 정리했다. 루트 `work0.md`는 유지했다. 이번 작업은 문서 보관이며 빌드·제품·기기 검증을 새로 실행하지 않았다.

**공식 SkiaSharp 바이너리를 사용하는 Graphite 기본 경로와 배포 전환은 구현됐다. 플랫폼별 제한된 실행 성공은 있으나, Linux 제품 동기화 실패·성능 수용 미완료·장시간 GPU 장애 등 잔여 조건 때문에 전체 상태는 `PARTIAL`이다.**

## 보관 자료와 판독 기준

- [원문 보존본](work0.original.md): 계획·단계별 기준·실행·실패·후속 수정 전체. 내부 상대 링크는 작성 당시 저장소 루트 기준이다.
- [보관 manifest](work0-archive-manifest.json): 원문 크기·SHA-256·보관 시각과 경로.
- [루트 작업 문서](../../work0.md): 보관 이후 변경될 수 있는 원본.

아래 검증 결과는 **원문에 기록된 당시 결과**다. 오래된 본문·단계표의 Apple/Linux `skippedByUser`, “iOS는 이번 범위 아님”보다 문서 상단의 후속 재개 결과를 우선했다. 성능 반복 중단은 `incomplete/stoppedAfterUserFeedback`이며 사용자 생략이나 성능 합격과 다르다. 원문이 가리키는 일부 상세 보고서는 현재 checkout에 없어, 이 요약이 원자료를 재확인했다는 뜻은 아니다.

## 목표와 구현된 구조

- SkiaSharp `4.154.0-preview.1.26454.9`와 공식 `SkiaSharp.NativeAssets.*`를 사용한다. Windows/Android/Linux의 custom `doroti_graphite_*` ABI·prebuild·staging 의존성을 제거하고 Apple Metal·Web Dawn의 공식 Graphite 경로를 유지했다.
- 공개 `SKGraphiteContext.CreateVulkan`과 공통 session/loader를 연결했다. 공식 NuGet 서명·RID별 자산·해시·실제 로드 경로를 확인하는 provenance 계약을 host/runner/target/template에 반영했다.
- Vulkan은 Graphite 렌더 이미지 R과 출력 이미지 P를 분리한다. 공개 호출 관찰기와 제출 journal이 **성공한 queue submission**의 상태를 확정하고, 같은 큐에서 R→P GPU 복사 후 R의 상태·소유권을 복사 직전 값으로 복원한다.
- 명령 기록·제출·GPU 완료·플랫폼 표시 완료를 구분한다. GPU fence 이후 Graphite completion/frame을 회수하며, 출력 P는 플랫폼 사용 종료까지 별도로 보존한다. 정상 표시 경로에는 CPU readback을 넣지 않는다.
- 기본 배포, Windows package-only consumer, Android APK, Linux 생성 템플릿, Apple 패키지 설정과 문서를 정리했다. 구 custom bridge/runner/source는 원문상 history로 보존했으며 사용자 native cache를 삭제하지 않았다.
- 플랫폼 뷰 합성은 [work1.md](../../work1.md), WebView는 [work2.md](../../work2.md)의 별도 범위다. W0 전환을 미구현 기능의 완료로 해석하지 않는다.

## 종료·GPU 자원 회수 계약

논리적 close, 새 제출 차단, 창 숨김은 **5초 이내**로 응답한다. GPU/native 자원은 실제 완료 또는 실제 device loss를 확인한 뒤 회수한다. 5초 초과는 fault로 보고하되 timeout을 device loss로 바꾸지 않는다. 미회수 generation은 최대 1개이며 해당 owner의 새 renderer 생성을 차단한다. 영구 stall에서는 회수 시점을 보장하지 않는다.

최초 실험에서 미완료 context의 직접 `Dispose()`는 native fence에서 약 7초 기다렸고 전체 shutdown 약 12초로 **기존 5초 전체 회수 기준 FAIL**이었다. 이후 응답과 회수를 분리하는 계약을 채택했지만 원래 실패는 보존한다. 독립 retirement 실험 18세대는 통과했으며 논리적 close 응답 최대 0.4252ms, 완료 후 Dispose 최대 2.3213ms였다. 이 수치는 headless 진단이며 제품 표시·종료 성능을 대표하지 않는다.

## 플랫폼별 기록된 결과

| 영역 | 구현·검증 결과 | 남은 경계 |
| --- | --- | --- |
| 공통 Vulkan | AMD 780M/NVIDIA RTX 4060의 공식 Vulkan 1.2 실험 통과. journal 13사례, R/P copy 2 GPU × 4모드 × 3세대의 8,160프레임 및 retirement 18세대에서 scoped correctness 확인 | 지원 profile 밖의 동작, 영구 stall·실제 device loss, 전체 제품 성능은 별도 |
| Windows App SDK | 두 GPU의 제품 resize/Acrylic/7초 GPU 지연 close와 저장소 밖 새 캐시의 package-only host restore/publish/실행 통과. helper DLL의 WinUI PRI 포함 오류 수정 | 전체 기능·DPI·다중 창·물리 표시·성능 수용 미완료 |
| Windows MAUI | 공식 기본 자산·D3D12 출력 경로와 실제 화면/close 확인. UI join/Composition completion 교착 경로를 비동기 선회수로 수정한 뒤 창 숨김 약 12ms, 프로세스 종료 약 304ms 기록 | 60초 baseline/candidate 1쌍에서 CPU/native raster/submit·GPU wait p95 증가. 전체 성능 PASS 아님 |
| Android | Galaxy S25 API 36 arm64 및 API 36 x64 emulator 제품 실행은 PASS-scoped. Release Mono AOT/trim, 공식 split 자산, 회전·복귀·버튼·텍스트·탭·스크롤, 생성 package-only 앱 확인. 정상 회수 outstanding 0 | API 33 emulator 필수 RP2 부재 FAIL 보존. synchronization layer·GPU 지연/영구 stall·device loss·전체 성능 미검증. resize/실패 제출의 동기 대기 잔존 |
| Linux Qt | C++ 전처리문이 템플릿 생성 중 삭제되는 오류 수정. 2-slot 비동기 fence/owner-thread polling, ABI·공식 자산 기록 보완. Release·package-only framework-dependent/self-contained publish·Wayland/XWayland callback 확인 | llvmpipe 실제 Material 제품에서 depth attachment `WRITE_AFTER_WRITE` 각 5건으로 correctness FAIL. hardware GPU·native X11·물리 입력/IME/접근성·성능 미검증 |
| macOS AppKit / Mac Catalyst | arm64 Release 빌드 경고·오류 0, 공식 Metal 자산/로드·제품 표시/replay·1초 GPU 지연 수명 검사 PASS-scoped. Catalyst TFM/RID 복원·템플릿 버전 전달, 비동기 완료와 disconnect 자원 보존 수정 | 7초 지연은 드라이버 GPU timeout 취소로 FAIL이며 장시간 조건 미검증. 전체 package-only 실행·물리 입력·성능·DPI/다중 창·NativeAOT 등 잔여. SIGTERM 정리를 정상 종료로 보지 않음 |
| iOS | iPhone 12/iOS 26.6.1 Release NativeAOT 공식 Graphite/Metal, 자산 UUID/section·실제 dyld 로드·Mono 부재, 표시/replay/background-resume 확인. 22개 패키지와 격리 package-only 생성 앱의 게시·외부 generic API·기기 실행 통과. 사용자 정상 조작 확인 당시 표시 1,611프레임/실패 0, 입력·표시 sequence 485 일치 | 실제 5초 이상/영구 stall·device loss·전체 simulator RID·기능/성능·물리 IME/접근성 미검증. deadline callback 직접 호출 검사를 실제 장시간 stall로 확대하지 않음 |
| Web | 공식 Graphite/Dawn 경로 유지, Release 빌드와 제품 10 tests 통과 | 전체 플랫폼/성능 수용을 대신하지 않음 |

## 후속 수정에서 확정된 동작

### Android split APK·회전·수명

- base APK만 보던 검사를 `ApplicationInfo.SourceDir`와 `SplitSourceDirs` 전체로 확장했다. 현재 ABI의 공식 라이브러리가 정확히 하나인지 검사하고 실제 로드 경로·해시를 대조한다. 초기 archive/path 회귀 13사례와 자동 분할 설치·cold 재실행을 통과했다.
- swapchain은 identity pre-transform과 실제 SurfaceView 크기를 사용한다. 사용 가능한 `SuboptimalKhr`만으로 매 프레임 재생성하지 않으며 실제 크기 변경·`OutOfDateKhr`는 재생성한다. 사용자 물리 회전·가로 표시·탭 터치를 확인했다.
- Vulkan manifest를 실제 요구 버전 1.2로 맞추고 템플릿 상위 props 누락에 따른 앱 identity 손실을 수정했다. SurfaceDestroyed에서는 admission을 닫고 native window를 유지한 채 worker가 완료/loss 후 회수하며 새 Activity/View도 이전 세대를 기다린다.

### Linux·공통 GPU 선택 정책

- llvmpipe에서 session 360프레임, 2-slot copy/부분 갱신·픽셀 3,000프레임, 7초 지연 retirement 3세대가 validation 오류 없이 통과했다. 실제 Material 제품의 depth 동기화 FAIL과 구분한다.
- 고정 공식 Skia의 depth barrier stage 범위 부족이 오류와 일치하는 유력 원인으로 기록됐다. 수정된 공식 자산 또는 검증된 공개 API 해결책이 필요하며 오류를 숨기거나 custom 바이너리로 우회하지 않았다.
- 이후 사용자 요청으로 프로젝트 전반의 하드웨어/소프트웨어 장치 종류에 따른 차단을 제거하고 필요한 API·버전·표시·상호운용 기능으로 판단하도록 변경했다. `DOROTI_LINUX_VULKAN_ALLOW_SOFTWARE` opt-in과 과거 hardware rejection/exit 69는 현재 정책이 아니다.
- 정책 변경 뒤 Linux 일반 Release 실행·6회 resize 요청·종료, Web 소프트웨어 WebGL2/WebGPU admission과 invalid API/metrics 거부, Qt OpenGL native build를 확인했다. Linux에서의 Windows managed 빌드는 PRI 생성 비활성화 범위이며 Windows native/제품 runtime 검증은 아니다. depth 동기화 실패는 해결되지 않았다.

### iPhone 회전 최종 정책

크기 변경 프레임을 UIKit layout의 Core Animation transaction에서 표시하도록 동기화했다. 이후 중심 밀림을 줄이기 위해 `ScaleToFill`/`GravityResize`를 적용했으나 사용자가 늘어남을 지적해, **최종적으로 `ContentMode.Center`/`GravityCenter`로 원래 크기·비율을 유지**하도록 변경했다. bounds 밖 내용은 clip하고 새 크기 레이아웃을 다시 그린다. 일반 프레임과 GPU 회수는 비동기 경로를 유지한다.

최종 Core Animation bitmap 합성 6사례에서 중심 및 20×20 표식 크기 오차 0, 기존 수명 검사·Release NativeAOT 게시·재설치·표시·복귀를 통과했다. 과거 top-left 중심 밀림과 scale-to-fill 늘어남은 negative control로 비교했다. **실제 OS 회전 애니메이션의 자연스러움은 사용자 재확인 필요**이며 합성 검사를 실제 회전 화면 캡처로 보지 않는다.

## 단계 상태와 남은 작업

| 단계 | 요약 상태 |
| --- | --- |
| W0-0 | PARTIAL: 자산·지원 profile 조사와 기준선 제품 수정은 수행, 비교 성능 baseline 미완료 |
| W0-1~3 | 독립 공식 Graphite 실험 PASS 및 observer/journal/R-P copy PASS-scoped |
| W0-4 | PARTIAL: 취소·지연 회수 검사 통과 범위는 있으나 영구 stall·실제 loss 미실험 |
| W0-5~6 | scoped correctness에 근거해 공식 기본 코드 경로 채택, 공통 session/loader/host 계약 구현 |
| W0-7~9 | 플랫폼별 위 표의 scoped 결과 적용. Linux correctness FAIL 및 각 플랫폼 잔여 검증 유지 |
| W0-10 | PARTIAL: 전체 기능·성능·trim/AOT 수용 미완료. 플랫폼별 확인된 AOT 결과만 인정 |
| W0-11~12 | 공식 코드·배포 전환과 consumer/문서 인계 구현. 전체 qualification 종결은 PARTIAL |

우선 잔여 항목은 Linux depth 동기화 오류 해결, 플랫폼별 실제 장시간/영구 GPU stall·device loss·미회수 세대 수명, Windows 관측 성능 악화 원인과 제품별 성능 수용, 미검증 RID/consumer·DPI/다중 창·물리 입력/IME/접근성이다. iPhone 최종 회전 정책의 체감 확인도 별도로 남는다.

초기 성능 기준은 frame p95/p99 및 cold start의 baseline 대비 10% 이상 악화 방지, missed/dropped frame 증가 1 percentage point 이하, 무제한 pending queue·정상 표시 CPU readback·매 프레임 idle 대기 금지다. 이는 합격한 측정 결과가 아니다. 완료된 60초 비교 1쌍과 두 번째 baseline의 전경 guard 중단 기록을 보존하며, 사용자가 중단한 장시간 반복을 자동으로 재시작하지 않는다.

## 근거 위치와 현재 파일 확인 범위

현재 checkout에서 존재를 확인한 문서:

- [공식 Vulkan 독립 검사·회수 계약](../../Doroti/validation/stock-graphite-vulkan/README.md)
- [Android split APK 검증](../../Doroti/validation/android-graphite-apk/README.md)
- [iPhone 회전 정책·검사 경계](../../Doroti/validation/uikit-metal-lifecycle/rotation-center.md)

아래는 원문에 기록됐지만 이번 checkout에서 찾지 못한 상세 근거 경로다. 경로를 역사적 식별자로 보존하며, 복원되기 전 직접 검증 가능한 원자료 링크로 취급하지 않는다.

| 원문 기록 경로 — 저장소 루트 기준 | 내용 |
| --- | --- |
| `history/2026-09-12/validation/stock-graphite/2026-09-11/README.md` | 최초 공식 자산 실험·5초 회수 실패 |
| `history/2026-09-12/validation/stock-graphite/2026-09-11-retirement/README.md` | 비동기 회수 대안 |
| `history/2026-09-12/validation/stock-graphite/2026-09-12/README.md` | observer/copy/제품 실행·성능 경계 |
| `history/2026-09-12/validation/stock-graphite/2026-09-12-cutover/README.md` | 최종 배포 전환 |
| `history/2026-09-12/work0-custom-graphite/README.md` | 구 custom 구현 보관 위치 |
| `Doroti/docs/validation/official-graphite-cutover-2026-09-12.md` | 공식 전환·지원표 |
| `Doroti/docs/validation/linux-official-graphite-2026-09-12.md` | Linux 배포·llvmpipe·depth sync 실패 |
| `Doroti/docs/validation/android-graphite-orientation-2026-09-12.md` | Android 방향 수정 |
| `history/2026-09-13/android-work0/README.md` | Android 후속 검토 |
| `history/2026-09-13/apple-work0/README.md` | macOS/Catalyst 후속 검토 |
| `history/2026-09-13/ios-work0/README.md` | iOS NativeAOT·기기·consumer 후속 검토 |

원문의 `Doroti/artifacts/ios-rotation*/` 등 산출물 경로도 보존본에 남겼으며 이번 요약에서는 산출물을 이동·재생성하지 않았다. 문서 정리가 기존 FAIL/PARTIAL/notVerified를 PASS로 바꾸지 않는다.
