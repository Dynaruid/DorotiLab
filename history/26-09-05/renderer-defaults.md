# Web / Windows 기본 renderer 변경

사용자 요청으로 Web 기본값을 `worker-canvaskit-webgl`, Windows App SDK 기본값을 `Vulkan`으로 변경했다.

- Web loader의 옵션 미지정/`auto` 기본 선택과 target manifest를 맞췄다. 명시적 renderer 선택은 유지한다.
- Windows의 presenter 환경변수 미지정/빈 값은 Vulkan을 선택한다. `AngleD3D11`은 명시적으로 선택할 수 있다. 다중 GPU의 명시적 장치 선택 정책과 자동 presenter fallback 금지는 유지한다.
- 앱의 `WindowBackdropMode.acrylic` 요청을 기존 Acrylic 구현에 연결했다. 옵션 검증과 requested/effective mode 진단도 일반 이름을 처리한다. `experimentalAcrylic`과 runtime channel은 호환성을 위해 유지한다.
- Backdrop 미지정/`system`은 opaque다. Windows 11 24H2+의 앱 요청 창에만 Acrylic을 적용한다. Windows 데모는 기존 일반 Acrylic 요청과 투명 renderer 배경/반투명 Material surface를 사용하므로 실험 환경변수가 필요 없다.

## 검증

모든 실행은 20분 timeout을 적용했다.

- Web Release build: PASS, 경고/오류 0.
- TypeScript check: PASS.
- 실제 Chromium 기본값 검사: 옵션 미지정, `auto`, 명시적 `document-webgl` 모두 PASS (3/3). 기본값의 UI/Raster Worker diagnostics를 확인했다.
- Windows host/product validator Release build: PASS, 경고/오류 0.
- AMD Radeon 780M에서 기본 Vulkan opaque, 기본 Vulkan 일반 Acrylic, 명시적 ANGLE opaque, 기본 Vulkan legacy Acrylic: PASS (4/4).
- 명시적 ANGLE 일반 Acrylic: 첫 검사는 validator가 opaque ANGLE의 GPU-copy 계약을 사용하여 FAIL. 로그에서 일반 Acrylic 활성화와 copy 0의 직접 공유-buffer 렌더링을 확인했다. Validator에 ContentIsland/3-slot/direct-render 계약과 올바른 backend 이름을 추가한 뒤 재검사 PASS. 원본 실패는 보존했다.

증거: `.doroti/evidence/default-renderers/`의 build/check log, 각 Windows JSON/log, `angle-acrylic-v2.json`. Web은 `Doroti/validation/web-playwright/artifacts/wrapper/default-renderer/`와 `artifacts/default-renderer/`.

기본값 선택과 app 요청 연결 검증이다. 기존 성능/resize 실패 기록을 재분류하지 않으며 전체 GPU/DPI/주사율, physical scan-out, IME/접근성 검증을 완료한 것은 아니다.
