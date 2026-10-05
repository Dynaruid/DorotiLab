# Web Release 게시 AOT 기본값 — 2026-10-05

**기본값·override 계약, 옵션 없는 SampleApp2 게시와 WebGL/WebGPU 실행 PASS.**
사용자가 [SampleApp2 AOT 비교](2026-10-05-sample2-web-wasm-aot.md) 후 기본 설정 변경을 요청했다.
Release 게시에 Mono WASM AOT를 기본 적용하고 Debug 개발 프로필은 비-AOT로 유지했다.
정확한 설정 평가와 새 payload/browser 결과는 [receipt](2026-10-05-web-aot-default.json)에 보존한다.

## 변경

- 공통 provider `Sdk.props`에서 Blazor WebAssembly runner의 Release에는
  `RunAOTCompilation=true`, Debug에는 `false`를 기본 지정한다.
- Web target의 배포용 build props에도 같은 조건을 적용하여 source runner SDK props를
  직접 쓰지 않는 소비 경로를 포함한다. 다른 .NET 플랫폼에는 적용하지 않는다.
- 두 설정 모두 값이 비어 있을 때만 적용한다. 명령행의 `-p:RunAOTCompilation=false`와
  프로젝트의 명시 설정을 보존한다.
- `RunAOTCompilationAfterBuild`는 설정하지 않는다. AOT는 SDK의 Release **publish**
  파이프라인에 적용한다. 일반 `dotnet run -c Release`와 workspace CLI `run`은 build
  산출물을 사용하므로 AOT 실행을 의미하지 않는다.
- SampleApp2와 Testbed 한·영 README의 기본 Web 실행을 `publish -c Release`와
  isolation 헤더를 제공하는 서버로 바꿨다. Debug 개발 실행과 비-AOT 비교 명령도 안내한다.

Framework/host/renderer 코드, threads/SIMD, CDN 폰트 설정과 renderer 선택은 변경하지 않았다.
기존 실험 receipt는 당시 기록으로 보존하고 이번 기본값 변경을 별도 연결했다.

## 검증

| 확인 | 결과와 범위 |
| --- | --- |
| SampleApp2·Testbed 실제 MSBuild 설정 | PASS, 각각 Release AOT / Debug 비-AOT / Release 명령행 opt-out |
| Web provider build props | PASS, runner SDK props 없이 Release/Debug, 명령행·프로젝트 opt-out 평가 |
| 비-Web shared props | PASS, Release에 Web AOT 기본값이 전파되지 않음 |
| 옵션 없는 SampleApp2 Release publish | PASS, stock compiler, 53개 AOT module 등록, 경고/오류 match 0 |
| 새 기본값 게시물 WebGL / WebGPU | PASS, 실제 shared WASM heap, 카운터, 여섯 탭, Profile 합성 입력·Done·상태 유지, error/request failure 0 |
| Python·문서·whitespace | PASS |

실제 게시 명령에는 `RunAOTCompilation` 옵션이 없다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet publish samples/DorotiSampleApp2/web/DorotiSampleApp2.Web.csproj -c Release --nologo --artifacts-path ./temp/testing/web-aot-default/2026-10-05/sample2
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/tests/web_aot_profile.py ./temp/testing/web-aot-default/new-profile
```

SDK 10.0.400, Mono/Emscripten/AOT pack 10.0.11, Windows x64, Chrome 154.0.8037.93
headless, 720×840 / DPR 1에서 확인했다. native WASM은 raw **40,371,481 bytes**,
Brotli 압축본 **8,468,401 bytes**다. 전체 다운로드·실제 resident memory 수치가 아니다.

새 기본값으로 Testbed publish/browser를 다시 실행하지 않았다. Testbed는 이번 설정 평가와
[앞선 명시적 AOT 게시·브라우저 결과](2026-10-05-web-wasm-aot.md)를 구분해서 참고한다.
Debug build/browser도 이번 변경에서 다시 실행하지 않았으며, 비-AOT 설정 분리를 MSBuild로 검증했다.
provider build props 평가는 fresh NuGet package 배포나 clean-machine 실행 수락이 아니다.
새 성능 개선율, 표시 FPS, 물리 IME·screen reader·모바일·장기 메모리 수락은 이번 범위에서
notMeasured/notVerified다.

## 원시 자료

모든 publish/probe는 1,200초 process-tree timeout, 자동 retry 0이다. 새 default publish 1회,
프로필 probe 1회와 browser context 2개를 사용했다. 원시 자료는 삭제 가능한
`temp/testing/web-aot-default/2026-10-05/`의 `profile-r1/`, `sample2-default-publish.log`,
`default-payload.json`, `sample2_default_smoke.py`, `browser-r1/`과 게시물에 있다.
문서/receipt는 원시 자료를 정리해도 남는다.
