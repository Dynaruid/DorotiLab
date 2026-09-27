# DorotiSampleApp2

Cupertino 스타일의 독립 Doroti 샘플 앱입니다. 공통 C# UI와 Windows / Web 실행 프로젝트로 구성됩니다.

- **Components**: 카운터 버튼, 스위치, 슬라이더, 활동 표시기, 다이얼로그
- **Profile**: 이름 입력과 인사말, 탭 전환 시 입력 상태 유지
- **Settings**: 시스템 / 라이트 / 다크 테마 선택

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
