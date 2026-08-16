# Doroti documentation map

Start with [ADR-019](adr/ADR-019-product-framework-source-ownership.md). It defines the current product-first development model and supersedes milestone-era assumptions that framework source must be regenerated as one compiler batch.

- `adr/`: durable decisions. Later ADRs supersede conflicting earlier decisions.
- `architecture/`: implementation records from the G3-G7 bootstrap and migration period. Files named for a milestone preserve what was built and verified at that time; their `validate-g*`, `prepare-g*`, `promote-g*`, and review commands are retired historical commands.
- `artifact-schemas.md`: schemas for migration/provenance artifacts. These artifacts do not own current product source.

Current commands are:

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 build
pwsh -File ./Doroti/eng/doroti.ps1 validate
pwsh -File ./Doroti/eng/doroti.ps1 validate -ValidationSuite Release
pwsh -File ./Doroti/eng/doroti.ps1 migration-audit
```

Use `migration-audit` only when compiler, upstream selection, import, or provenance inputs change. Ordinary framework and product work uses the first three commands.
