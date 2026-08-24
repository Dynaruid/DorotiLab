# Windows top-level presentation resize diagnostic

Standard-chrome raw top-level HWND에서 D3D12/DXGI presentation과 interactive resize 순서를 비교하고, origin-moving resize를 composition owner로 옮긴 Windows 전용 diagnostic이다. 제품 host가 아니며, standard non-client phase 분리를 제거한 후속 수정은 Arm N에 구현돼 있다.

모든 명령은 repository root인 `C:\Users\parti\Labo\DorotiLab`에서 PowerShell로 실행한다.

## 가장 빠른 실행

먼저 Release binary를 빌드한다.

```powershell
dotnet build Doroti/validation/windows-top-level-presentation/Doroti.Validation.WindowsTopLevelPresentation.csproj -c Release --no-restore
```

Arm N을 실행한다.

```powershell
& .\Doroti\validation\windows-top-level-presentation\bin\Release\net10.0-windows10.0.19041.0\win-x64\Doroti.Validation.WindowsTopLevelPresentation.exe --arm N
```

`Doroti resize Arm N` surface가 열리면 밝은 보라색 안쪽 border를 실제 mouse로 잡고 다음을 확인한다. Arm N의 top-level HWND capacity는 모니터 작업영역을 덮지만, 평상시 Win32 window region은 보이는 border/chrome/content로 제한되어 바깥의 다른 창을 클릭할 수 있다. resize capture 중에만 envelope를 열고 latest composition commit 뒤 다시 제한한다. 종료는 `Alt+F4`를 사용한다.

1. 좌측 border를 빠르게 왼쪽으로 당겨 확대한다.
2. 같은 border를 빠르게 오른쪽으로 밀어 축소한다.
3. 확대와 축소를 여러 번 왕복한다.
4. border가 pointer와 반대 방향으로 튀는지 확인한다.
5. content가 좌우로 왕복하거나 우측에 흰색/검은색/암색 gap이 나타나는지 확인한다.
6. 우측, 상단, 하단과 네 corner도 같은 방식으로 비교한다.

검사가 끝나면 `Alt+F4`로 프로세스를 종료한다.

## `dotnet run`으로 실행

Binary 경로를 직접 사용하지 않으려면 다음 명령도 가능하다.

```powershell
dotnet run --project Doroti/validation/windows-top-level-presentation/Doroti.Validation.WindowsTopLevelPresentation.csproj -c Release --no-restore -- --arm N
```

## Arm 비교

| Arm | 경로 | 용도 |
| --- | --- | --- |
| `A` | direct-HWND swap chain + `DXGI_SCALING_NONE` | 기존 baseline |
| `S` | direct-HWND swap chain + transient `DXGI_SCALING_STRETCH` | gap 우선, 일시적인 stretch 허용 비교군 |
| `C` | composition swap chain + DirectComposition visual | edge-aware provisional anchor와 geometry-admission gate 검증 |
| `N` | fixed monitor envelope HWND + dual composition fronts | hidden exact front를 준비한 뒤 border/chrome/content geometry와 함께 교체하는 custom non-client 비교군 |

Arm A와 S는 다음처럼 실행한다.

```powershell
& .\Doroti\validation\windows-top-level-presentation\bin\Release\net10.0-windows10.0.19041.0\win-x64\Doroti.Validation.WindowsTopLevelPresentation.exe --arm A
& .\Doroti\validation\windows-top-level-presentation\bin\Release\net10.0-windows10.0.19041.0\win-x64\Doroti.Validation.WindowsTopLevelPresentation.exe --arm S
```

Legacy Arm B는 제거됐으며 실행할 수 없다.

Arm N은 standard non-client를 사용하지 않으므로 현재 diagnostic에서는 Snap Layouts, system menu, maximize/restore, accessibility chrome과 monitor 간 이동을 제공하지 않는다. 이 항목들은 제품 구조로 채택하기 전 별도 구현·검증 대상이다.

## Arm N transaction smoke

실제 pointer를 주입하지 않고 UI message loop에서 좌측, 상단, 좌상단 resize target을 2ms cadence로 게시해 latest-only dual-front 계약을 확인할 수 있다. 실행 후 자동으로 종료된다.

```powershell
$evidence = Join-Path (Get-Location) '.doroti/arm-n-smoke.json'
& .\Doroti\validation\windows-top-level-presentation\bin\Release\net10.0-windows10.0.19041.0\win-x64\Doroti.Validation.WindowsTopLevelPresentation.exe --arm N --owned-smoke --evidence $evidence
Get-Content -LiteralPath $evidence
```

`ownedLatestEpoch == ownedVisibleFrontEpoch == ownedCommittedEpoch`, `ownedFrontGeometryMismatchCount == 0`, `ownedSmokeDrainTimeoutCount == 0`인지 먼저 확인한다. `ownedPreparedFrontCount == ownedFrontSwitchCount + ownedAbandonedPreparedFrontCount`여야 하며 abandoned front는 hidden 상태에서만 폐기된다. 입력 격리는 `ownedOutsideHitTestPass == true`, `ownedInsideHitTestPass == true`, `ownedRegionCommitWaitTimeoutCount == 0`으로 확인한다. 이 smoke는 실제 mouse visible acceptance를 대신하지 않는다.

## App evidence 저장

`--evidence`를 지정하면 창이 종료될 때 app counter와 failure를 JSON으로 기록한다.

```powershell
$evidence = Join-Path (Get-Location) 'Doroti/validation/evidence/resize/manual-arm-c.json'
& .\Doroti\validation\windows-top-level-presentation\bin\Release\net10.0-windows10.0.19041.0\win-x64\Doroti.Validation.WindowsTopLevelPresentation.exe --arm C --evidence $evidence
```

주요 Arm C counter는 다음과 같다.

- `compositionProvisionalCommitCount`: provisional offset/clip commit 수
- `compositionExactCommitCount`: admitted exact visual commit 수
- `compositionPreAdmissionExactRejectCount`: matching `WM_SIZE` 전에 차단한 exact publish 수
- `compositionGeometryAdmissionCount`: matching geometry admission 수
- `compositionGeometryAdmissionRejectCount`: stale/mismatched admission 거부 수
- `compositionStaleExactRejectCount`: stale exact visible publish 거부 수
- `preparedOuterMismatchCount`: prepared target과 실제 outer/client mismatch 수

JSON의 `status: PASS`는 프로세스가 예외 없이 종료됐다는 뜻이다. 실제 border drag가 부드럽다는 visible PASS를 의미하지 않는다.

## 3회 observer qualification

Observer는 native capture binary가 필요하다. 아직 없다면 Visual Studio C++ toolchain과 CMake 환경에서 먼저 빌드한다.

```powershell
cmake -S Doroti/validation/windows-resize-capture -B .doroti/build/windows-resize-capture-vs -G 'Visual Studio 18 2026' -A x64
cmake --build .doroti/build/windows-resize-capture-vs --config Release
```

그다음 Arm C qualification을 3회 실행한다.

```powershell
dotnet build Doroti/validation/windows-top-level-presentation/Doroti.Validation.WindowsTopLevelPresentation.csproj -c Release --no-restore
pwsh -NoProfile -File Doroti/eng/validate-windows-presentation-observer.ps1 -Arm C -Runs 3
```

Summary는 다음 경로에 생성된다.

```text
Doroti/validation/evidence/resize/win-observer-m1-arm-c-summary-*.json
```

M1 qualification은 동일 source fingerprint에서 세 run이 모두 PASS해야 한다. WGC/desktop timing, build, app counter 또는 isolated PASS 하나만으로 실제 visible acceptance를 대신하지 않는다.

## CLI options

| Option | 값 | 설명 |
| --- | --- | --- |
| `--arm` | `A`, `S`, `C`, `N` | 실행할 presentation arm. 기본값은 `A` |
| `--evidence` | JSON path | 종료 시 app evidence 기록 |
| `--qualification` | flag | observer가 사용하는 qualification control 활성화 |
| `--refresh-hz` | `30`–`1000` | qualification cadence 기준. 기본값은 `165` |
| `--owned-smoke` | flag | Arm N의 좌측/상단/좌상단 composition-owned transaction smoke |

`--qualification`과 `--refresh-hz`는 일반적인 수동 visible 확인에는 필요하지 않다.

## 현재 acceptance 상태

2026-08-24 후속 수정 기준으로 Arm C에는 exact-before-geometry 차단, pure-move resize epoch 제외, composition/GPU lock 분리가 적용돼 있지만 standard non-client와 visual의 output-frame phase 분리는 남는다. 최종 observer 3회 결과는 `PASS/FAIL/FAIL`이다.

Arm N은 실제 HWND geometry를 interactive resize 중 바꾸지 않는다. 두 composition front 중 hidden slot에 exact frame을 먼저 latch한 뒤, child visual 교체와 offset/clip을 하나의 DirectComposition commit으로 적용한다. 2ms cadence의 239-target dual-front smoke 3회는 PASS했다. 사용자는 수정된 final binary에서 고속 좌측·상단 확대를 직접 재현해 기존 front/border와 창 경계의 어긋남 및 떨림이 사라졌음을 확인했다. 이 범위의 visible acceptance는 **PASS (scoped manual)**다.

우측·하단·네 corner, 확대/축소 전체 조합, 다른 DPI/refresh/monitor와 standard Windows chrome 기능은 아직 `notVerified`다. Arm C의 observer 3회 실패도 소급 변경되지 않으므로 현재 M1은 계속 **FAIL — hard stop**, G2는 `notVerified`다. 자세한 원인, 수정 내용과 evidence boundary는 repository root의 `problem.md`를 참조한다.
