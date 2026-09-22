# Instructions

- Tests must use a 20-minute timeout.
- 수백번 이상의 반복검증은 불필요 대부분의 검증은 30번 이내로 확인하기
- `Doroti/artifacts`는 삭제 가능한 로컬 빌드·검증 산출물이다. 용량 정리 시 삭제 대상에 포함하며, `Doroti/eng/clean-local-state.ps1 -Action artifacts -Force` 또는 `-Action all -Force`로 정리한다. 보존이 필요한 증거는 추적되는 문서·migration 경로 등에 별도로 남기고, 원시 산출물 삭제 후에는 해당 파일이 남아 있다고 보고하지 않는다.
