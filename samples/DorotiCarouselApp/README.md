# Doroti Custom Carousel Sample

[gskinnerTeam/flutter_custom_carousel](https://github.com/gskinnerTeam/flutter_custom_carousel)의 예제 앱을 Doroti C#으로 옮긴 독립 예제 앱입니다.
재사용 캐러셀은 [packages/Doroti.CustomCarousel](../../packages/Doroti.CustomCarousel/README.md),
화면 코드는 [src/App.cs](src/App.cs)에 있습니다.

## 사용법

홈의 미리보기를 세로로 드래그하거나 ↑/↓ 또는 하단 선택 표시로 선택한 뒤 **Open**을 누릅니다.
밝은 오프화이트 배경과 큰 실제 이미지 미리보기, 간결한 타이포그래피로 구성하며 작은 화면에서는 제목과 조작부를 조정합니다.
각 데모에서 터치·마우스 드래그와 해당 축의 스크롤 입력을 사용합니다.
가로 데모는 일반 세로 휠, 수평 wheel/trackpad 입력 또는 Shift+wheel을 사용할 수 있습니다.

| 데모 | 동작 |
| --- | --- |
| Cover Slider | 세 개의 가로 루프 목록. 사진을 누르면 상세 보기 |
| Circular Menu | 원형 배치, 거리별 scale/blur. 선택된 토핑을 눌러 버거에서 추가·제거 |
| Card Deck | 비루프 카드 덱, sticky snapping, Previous/Next와 Shuffle |
| Digital Wallet | 역방향 가로 입력과 카드 겹침·fade/blur, 선택 계좌 잔액 표시 |
| Record Box | 세로 루프, 깊이 방향으로 이어지는 9장 앨범과 원근 기울기. 드러난 앨범 가장자리를 눌러 선택 |

## 실행

저장소 루트 `DorotiLab`에서 실행합니다.
[global.json](../../global.json)에 지정된 .NET SDK와 대상 플랫폼의 빌드 도구가 필요합니다.
WebAssembly 빌드에는 `wasm-tools` 워크로드가 필요합니다.

```powershell
# Windows App SDK
pwsh -File Doroti/eng/doroti.ps1 run --app samples/DorotiCarouselApp --platform windows

# Web 개발 서버
dotnet run --project samples/DorotiCarouselApp/web/DorotiCarouselApp.Web.csproj -c Debug
```

### Web Release 게시

Web Release publish는 저장소 기본 WASM AOT 설정을 사용합니다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet publish samples/DorotiCarouselApp/web/DorotiCarouselApp.Web.csproj -c Release -o temp/testing/carousel-web
python Doroti/eng/serve-isolated-web.py temp/testing/carousel-web/wwwroot --port 5226
```

브라우저에서 `http://127.0.0.1:5226`을 엽니다. WASM threads에 필요한 COOP/COEP 헤더를 제공하는 서버입니다.

워크스페이스는 `windows`, `web`, `android`, `ios`, `linux`, `macos`, `maccatalyst`를 선언합니다.
다른 플랫폼은 해당 호스트와 SDK에서 같은 CLI의 `--platform` 값을 바꿔 실행합니다.
예: `pwsh -File Doroti/eng/doroti.ps1 run --app samples/DorotiCarouselApp --platform android`.

## 소스와 라이선스

원본 사진과 카드 이미지는 `assets/images/`에 있으며 공통 앱 DLL에 내장됩니다.
원본 BSD 3-Clause 라이선스는
[assets/LICENSE.flutter_custom_carousel](assets/LICENSE.flutter_custom_carousel)에 있습니다.
애니메이션 효과는 Doroti의 Transform/Opacity/ImageFiltered 조합을 사용합니다.
원본 shimmer/tint, Hero 전환과 지갑 거래 목록은 이 예제에 포함하지 않습니다.

회귀 검사 명령은 [검사 안내](docs/validation.md)에 있습니다.
