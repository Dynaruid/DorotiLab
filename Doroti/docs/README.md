# Doroti documentation map

Start with [ADR-019](adr/ADR-019-product-framework-source-ownership.md). For Web source and startup policy, continue with [ADR-020](adr/ADR-020-web-typescript-bootstrap.md).

- `adr/`: durable decisions. Later ADRs supersede conflicting earlier decisions.
- `architecture/`: implementation records from the G3-G7 bootstrap and migration period. Files named for a milestone preserve what was built and verified at that time; their `validate-g*`, `prepare-g*`, `promote-g*`, and review commands are retired historical commands.
- `validation/`: active validator contracts and committed target/Web evidence.

Current commands are:

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 build
pwsh -File ./Doroti/eng/doroti.ps1 validate
pwsh -File ./Doroti/eng/doroti.ps1 validate -ValidationSuite Fcr0
pwsh -File ./Doroti/eng/doroti.ps1 validate -ValidationSuite Fcr1
pwsh -File ./Doroti/eng/doroti.ps1 validate -ValidationSuite Fcr2
pwsh -File ./Doroti/eng/doroti.ps1 validate -ValidationSuite Release
```

`Fcr1` validates the closed framework shader manifest, Flutter/adapted source pins,
embedded resource and uniform/sampler ABI, async load failure observation, runtime
effect cache context fencing, and unsupported-backend diagnostics. It does not claim
Windows live, Android physical, or Flutter raster differential acceptance; those remain
separate evidence boundaries.

`Fcr2` validates the pinned Flutter animation/future semantic fixture against typed
Offset/Size/Rect/Vector2/double interpolation, Future completion/error/cancellation,
timer disposal, collection/pattern behavior, and Debug/Release assert behavior. It also
checks that new lowering observes discarded Futures and does not emit dynamic Tween
arithmetic. The full app interaction log and target acceptance remain separate evidence
boundaries.
