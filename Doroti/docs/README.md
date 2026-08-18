# Doroti documentation map

Start with [ADR-019](adr/ADR-019-product-framework-source-ownership.md). For Web source and startup policy, continue with [ADR-020](adr/ADR-020-web-typescript-bootstrap.md).

- `adr/`: durable decisions. Later ADRs supersede conflicting earlier decisions.
- `architecture/`: implementation records from the G3-G7 bootstrap and migration period. Files named for a milestone preserve what was built and verified at that time; their `validate-g*`, `prepare-g*`, `promote-g*`, and review commands are retired historical commands.
- `validation/`: active validator contracts and committed target/Web evidence.

Current commands are:

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 build
pwsh -File ./Doroti/eng/doroti.ps1 validate
pwsh -File ./Doroti/eng/doroti.ps1 validate -ValidationSuite Release
```
