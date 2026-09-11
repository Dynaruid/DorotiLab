# 공식 Graphite Vulkan 독립 실험

`work0.md`의 공식 Graphite 후보와 수명/제출 상태를 검증한다. **전체 배포 전환 완료를 뜻하지 않는다.**
공통 `Doroti.Skia.Vulkan/Stock` observer source를 독립 하네스에 link하며 제품 session 검증은
별도 `../stock-graphite-session/` 프로젝트에서 수행한다. [현재 실행 상태](../../../work0.md)를 참고한다.
[최초 실패](../../../history/2026-09-12/validation/stock-graphite/2026-09-11/README.md)와 [비동기 회수 대안](../../../history/2026-09-12/validation/stock-graphite/2026-09-11-retirement/README.md)은 당시 증거로 보존한다.
사용자가 판단을 위임한 현재 계약은 close 응답 5초, 실제 GPU 완료 뒤 회수, 미회수 generation 상한 1이다.

## 재현

저장소 루트, Windows x64, .NET 10 호환 SDK, Vulkan 1.2 GPU 및
`VK_LAYER_KHRONOS_validation`/synchronization validation이 필요하다.
현재 runner의 GPU selector는 이 장비의 `AMD`, `NVIDIA`다. 다른 장비는 실제 이름으로 조정해야 한다.

```powershell
python Doroti/validation/stock-graphite-vulkan/run.py
python Doroti/validation/stock-graphite-vulkan/run-retirement.py
python Doroti/validation/stock-graphite-vulkan/run-copy.py
python Doroti/validation/stock-graphite-vulkan/run-windows-official.py --resize --acrylic
```

각 child command는 저장소 `run-with-timeout.py`로 **1,200초** 외부 제한을 적용한다.
`run.py`는 NuGet.org에서 고정 버전 Win32 package를 새 증거 디렉터리로 다운로드하고,
로컬 package/asset과 비교한 후 `dotnet nuget verify --all`을 실행한다.
검증 실패 시 GPU 실행을 진행하지 않는다. 사용자의 package cache는 변경하지 않는다.
프로젝트는 SkiaSharp·공식 NativeAssets·Silk.NET.Vulkan만 참조하고 Doroti host를 참조하지 않는다.

runner exit 1은 실패다. 현 버전에서는 두 `shutdown` 시나리오의 종료 예산 실패가 재현된다.
예상한 실패라는 이유로 PASS로 변환하지 않는다. baseline collector의 exit 0은 수집 완료만 의미하며,
개별 제품의 exit code는 각 `process.json`에 있다.

직접 실행 시에도 외부 wrapper를 사용한다. DLL에는 NuGet.org archive에서 꺼낸 절대 경로를 전달한다.

```powershell
python Doroti/validation/run-with-timeout.py dotnet build Doroti/validation/stock-graphite-vulkan/StockGraphite.csproj -c Release
python Doroti/validation/run-with-timeout.py Doroti/artifacts/validation/build/stock-graphite-vulkan/bin/Release/net10.0/win-x64/StockGraphite.exe <official-dll> NVIDIA async <report.json>
```

mode는 `sync`, `async`, `shutdown`, `retire-ready`, `retire-delayed`, `retire-cancel`,
`copy-one`, `copy-two`, `copy-msaa`, `copy-v2`이며 제품 session 프로젝트에는 `session`이 있다.
`run-retirement.py`는 기존 검증 archive를 다시 서명/hash 검사하고 새 timestamp 디렉터리에 결과를 쓴다.
기본 archive 경로가 없는 환경은 `run.py`로 자산을 확보한 후 해당 run 디렉터리를 인자로 전달한다.

## 검사 경계

- `sync`/`async`: 공개 CreateVulkan/recorder/surface/insert/submit/readback. 서로 다른 흰색·검정 배경에
  불투명/반투명 이미지, 텍스트, 알파, gradient, clip, runtime shader, blur를 그린다.
  픽셀 검사는 정확한 내부 색, blending ±1, gradient/blur 내부 ±2, 텍스트·blur halo coverage로 검사한다.
  readback/PNG encoding은 진단 전용이다. 별도 Windows product runner가 실제 Composition 표시를 확인한다.
- `shutdown`: 호스트 전용 timeline semaphore feature를 **실제 device descriptor에 활성화**하여
  queue를 지연시킨다. 5초 동안 completion fence를 polling하고, device loss가 아님을 확인한 후
  context를 Dispose한다. 별도 helper가 Dispose 시작 7초 후 실제 semaphore를 signal하여 정리한다.
  helper는 Graphite를 호출하지 않는다. `vkWaitForFences` 관찰 callback은 원래 인자·timeout·결과를 그대로 전달한다.
  이는 제어된 지연 실험이며 GPU reset, 실제 device loss, 무기한 hang 실험이 아니다.
- `normalTeardown=PASS`는 semaphore 해제 후 리소스가 정상 정리됐다는 필드다.
  종료 deadline은 별도 `shutdownWithinDeadline=false`, 전체 `status=FAIL`로 평가한다.
- `copy-*`: 성공한 queue 제출 순서에만 journal을 적용한다. R의 관찰된 L에서 복사 후 L로 복원하며
  1/2 slot, 각 3세대, 1,000-frame 실행, 부분 갱신 보존, MSAA resolve/subpass, RP2/Barrier2/Submit2를 검사한다.
  CPU readback은 픽셀 검증 전용이다. metadata/callback 상한 및 미지원 alias/profile 거부를 기록한다.
- `run-windows-official.py`: 실제 Windows App SDK 창·Composition·resize/Acrylic와 Vulkan/sync validation을 검사한다.
  Windows TFM의 managed DLL은 일반 net10.0 자산과 해시가 다르므로 resolved graph의 entry로 확인한다.
- `probe-windows-delayed-close.py`: 실제 창에서 7초 timeline producer wait를 제출하고 WM_CLOSE 이후 숨김/회수를 각각 측정한다.
  5초 fence timeout으로 일반 product fixture가 exit 1을 내는 결과를 정상 종료로 바꾸지 않는다.
- Android는 최종 APK native entry와 `dladdr`로 확인한 실제 loaded path를 검증한다. Vulkan 1.2 profile의
  `vkCreateRenderPass2`를 제공하지 않는 API 33 emulator 결과는 실패로 보존한다.
- Apple/Linux build/runtime/AOT 검증은 사용자 요청으로 `skippedByUser`이며 코드/패키지 구성만 수행한다.

이 실험의 native module/dispatch delegate는 해당 context와 native destructor보다 오래 유지된다.
Graphite context/queue는 단일 owner가 사용한다. 결과 위조, private C++ 메모리 접근,
커스텀 `doroti_graphite_*` 호출, Ganesh/CPU renderer 대체는 없다.

구 custom ABI 실험과 성능 비교 runner는 [history](../../../history/2026-09-12/work0-custom-graphite/README.md)로 보존했습니다. 현재 두 기본값을 구 경로/공식 경로로 잘못 비교하지 않습니다.


Final default/package checks: `run-clean-windows-consumer.py` and `run-clean-android-package.py <feed>`. `probe-maui-windows.py <exe> - <output>` checks the normal app-directory asset without a manifest. Each child uses the 1,200-second wrapper. See [cutover evidence](../../../history/2026-09-12/validation/stock-graphite/2026-09-12-cutover/README.md). Do not rerun the earlier matrix merely because documentation or packaging changed.
