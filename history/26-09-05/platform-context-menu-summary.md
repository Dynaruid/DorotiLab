# 플랫폼별 컨텍스트 메뉴 보완

- 요청: Flutter처럼 플랫폼별 모양과 상태별 항목을 사용하는 Doroti 컨텍스트 메뉴. 추가 확인에 따라 Web은 브라우저 기본 메뉴를 유지한다.
- 기존 구현 확인: Material의 Android/iOS/macOS/Windows/Linux/Fuchsia 분기, Cupertino의 모바일/데스크톱 분기, TextField/SelectableText/SelectionArea 기본 builder, 선택·읽기 전용·비밀번호·클립보드 상태별 항목 생성이 이미 있다.
- 수정: `ContextMenuController`가 루트 오버레이의 context 대신 호출 context에서 테마를 캡처한다. 캡처한 테마 아래의 `Builder`에서 메뉴를 만들고, 열린 메뉴 갱신 때 테마도 갱신하며 제거 때 참조를 해제한다.
- 수정: Windows/Linux adaptive button에 null 콜백을 직접 전달해 비활성 사용자 항목을 비활성으로 유지한다. 기존 람다는 null 콜백도 활성 작업처럼 만들었다.
- 문서: `Doroti/docs/context-menus.md`에 플랫폼 표, 항목 정책, 사용자 메뉴 예제를 추가했다.

## 검증

- Release `Doroti.Validation.Fcr7MaterialWidget`: **PASS**. 20분 process timeout 적용.
- 추가 context-menu 검증: **PASS**. 6개 플랫폼 툴바, 사용자 라벨/콜백, 비활성 항목, 빈 메뉴, Cupertino 분기, 선택/커서/읽기 전용/비밀번호 항목과 순서, 오버레이의 플랫폼·다크 테마 캡처 및 열린 메뉴 갱신, 제거 시 참조 해제, 브라우저 메뉴 기본 활성화.
- `git diff --check`: **PASS**.
- 실제 OS별 메뉴 화면, 터치/마우스 interaction, 시스템 조회·검색·공유 실행, IME/접근성 및 브라우저 실제 팝업: 이번 변경에서 **notVerified**. 런타임 계약 검증은 물리 기기/시각적 acceptance를 의미하지 않는다.

초기 검증 fixture는 내부 형식 접근과 직접 생성한 저수준 위젯의 초기화 누락으로 빌드/실행에 실패했다. 검증 assembly의 friend 접근, 명시적인 EditableText 입력, unmounted overlay에 필요한 최소 binding/BuildOwner를 준비한 뒤 최종 PASS했다. 실제로 등록하지 않은 전역 lifecycle handler를 해제하는 fixture 코드도 제거했다. 이 초기 실패는 제품의 OS별 동작 실패/성공 판정이 아니다.

최종 실행 출력:

```text
Context menus: PASS (six platforms, action states, disabled callbacks, overlay themes, web default)
FCR-7 material/widget runtime contract: PASS (configuration=Release, system-theme-palettes=light+dark)
```
