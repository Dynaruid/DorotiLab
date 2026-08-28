# Cross-platform first boot MVP implementation

`plan.md`의 안전한 MVP를 2026-08-29에 실행했다. 공용 manifest/theme/text-service 비용, Windows production hashing/diagnostic graph, Android profile/x64 store, Web trimming/symbol, Linux managed contract, CLI artifact reuse를 구현했다.

핵심 검증은 Windows C5-A/C9, Android arm64 physical profile/install/cold-warm/text, Android x64 trimmed build/profile strict check, Web trimmed publish와 desktop Chrome, Linux Qt ABI, CLI normal/reuse에서 PASS했다. 정량 TTID 개선은 반복 측정하지 않았으므로 모두 `expectedImprovement`이며 성능 PASS가 아니다.

전체 Release solution은 Windows에 macOS `sips`가 없어 1 error로 `FAIL`했다. FCR-0 aggregate, Apple 실제 runtime, Linux Wayland/X11, Android x64 emulator·한글 commit·TalkBack, Windows physical IME/Narrator, Web fresh/mobile/production HTTP header는 `notVerified`다. 자세한 수치와 rollback/후속 경계는 루트 `work.md`를 기준으로 한다.
