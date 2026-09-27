# DorotiSampleApp2

Cupertino 스타일의 독립 Doroti 샘플 앱입니다. 공통 C# UI와 Windows / Web 실행 프로젝트로 구성됩니다.

- **Components**: 카운터 버튼, 스위치, 슬라이더, 활동 표시기, 다이얼로그
- **Profile**: 이름 입력과 인사말, 탭 전환 시 입력 상태 유지
- **Settings**: 시스템 / 라이트 / 다크 테마 선택
- **Variable Blur**: 60개 항목의 ListView 위에 상단 고정 VariableBlur 오버레이, 강도 조절과 켜기/끄기

Variable Blur 페이지는 리스트 상단 180px에 `BackdropFilter`와
`ImageFilterConfig.CreateVariableBlur(startSigma: 강도, endSigma: 0, resolutionScale: 0.25)`를 적용합니다.
라디오 버튼으로 다음 네 가지 모드를 비교할 수 있습니다.

- **Full quality**: 전체 구간을 원본 해상도로 처리합니다.
- **Adaptive** (기본): 약한 블러는 원본 해상도, 강한 블러는 1/2·1/4 해상도로 처리하고 경계를 혼합합니다.
- **Fast adaptive**: Adaptive에 7회 샘플링 근사 커널을 적용합니다. 강한 블러에서 품질이 낮아질 수 있습니다.
- **Fixed 1/4**: 전체 구간을 1/4 해상도로 처리합니다. 선명한 구간도 저해상도가 됩니다.

별도의 스위치로 블러 전체를 켜고 끕니다.
위쪽은 흐리고 아래쪽은 선명하며, 블러 영역에서도 리스트를 스크롤할 수 있습니다.
구현은 [src/VariableBlurPage.cs](src/VariableBlurPage.cs)에 있습니다.
VariableBlur에는 GPU 렌더러가 필요하며, 기본 CPU 래스터 검증에는 이 탭의 블러 렌더링이 포함되지 않습니다.

설정과 입력값은 앱 실행 중에만 유지됩니다. Cupertino Icons 1.0.9 폰트와 해당 라이선스는 `assets/fonts`에 포함되어 있습니다.

저장소 루트 `DorotiLab`에서 실행합니다. 루트 `global.json`에 지정된 .NET SDK와 플랫폼별 빌드 도구가 필요합니다.

## Windows

```powershell
dotnet run --project ./samples/DorotiSampleApp2/windowsappsdk/DorotiSampleApp2.WindowsAppSdk.csproj -c Release
```

기존 Material 샘플과 같은 Windows App SDK / Vulkan 호스트를 사용합니다.

## Web

```powershell
dotnet run --project ./samples/DorotiSampleApp2/web/DorotiSampleApp2.Web.csproj -c Release
```

브라우저에서 `http://127.0.0.1:5089`에 접속합니다. WebAssembly 빌드에는 `wasm-tools` 워크로드가 필요합니다.

워크스페이스 CLI에서도 `-App ./samples/DorotiSampleApp2 -Platform windows` 또는 `-Platform web`으로 선택할 수 있습니다.

화면 구현은 [src/App.cs](src/App.cs), 공통 진입점은 [Program.cs](Program.cs)에 있습니다.

## 검증

```powershell
python ./Doroti/validation/run-with-timeout.py dotnet run --project ./Doroti/validation/cupertino-sample -c Release
python ./Doroti/validation/run-with-timeout.py dotnet run --project ./Doroti/validation/cupertino-sample -c Release --no-build -- --portrait
```

실제 위젯과 Skia 렌더러로 720×840 / 400×800 화면을 그려 카운터, 탭 간 상태 유지,
입력 콜백, 다이얼로그 배치, 테마 전환, 활동 표시기 애니메이션을 검증합니다.
탭 전환은 좌표 기반 마우스 / 터치 포인터 이벤트를 위젯 입력 경로로 전달해
히트 테스트, 선택된 탭 번호, 표시된 페이지와 같은 탭 재선택까지 확인합니다.
PNG는 검증 프로젝트의 `bin/Release/net10.0/snapshots`에 생성됩니다.
이 검증에는 실제 OS 입력 및 Windows / 브라우저 화면 표시 확인은 포함되지 않습니다.

Variable Blur는 Vulkan GPU가 있는 환경에서 별도로 검증합니다.

```powershell
python ./Doroti/validation/run-with-timeout.py dotnet run --project ./Doroti/validation/cupertino-sample -c Release -- --variable-blur
python ./Doroti/validation/run-with-timeout.py dotnet run --project ./Doroti/validation/cupertino-sample -c Release --no-build -- --variable-blur --portrait
python ./Doroti/validation/run-with-timeout.py dotnet run --project ./Doroti/validation/cupertino-sample -c Release --no-build -- --variable-blur --high-dpi --oversized-backing
python ./Doroti/validation/run-with-timeout.py dotnet run --project ./Doroti/validation/cupertino-sample -c Release --no-build -- --variable-blur --high-dpi --oversized-backing --2560x1600 --frame-benchmark
```

블러 영역에서 합성 트랙패드 pan/zoom을 시작해 종료 이벤트 전의 리스트 위치와
GPU 렌더링 픽셀 변화를 확인합니다. 실제 물리 트랙패드 입력 및 화면 표시 지연은 별도 확인 대상입니다.
`--frame-benchmark`는 프레임 구성과 GPU 완료까지 측정하며 창 표시 FPS가 아닙니다.
실제 Windows 창을 PowerShell로 조작하는 측정 절차와 결과는
[VariableBlur 성능 기록](../../Doroti/validation/backdrop-filters/variable-blur-performance.md)에 있습니다.
