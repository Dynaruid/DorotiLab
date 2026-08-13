#!/usr/bin/env bash
set -euo pipefail

doroti_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

dotnet build "$doroti_root/Doroti.Product.slnx" --nologo
dotnet run --project "$doroti_root/validation/Doroti.Validation.G4PhysicsAnimationGestures/Doroti.Validation.G4PhysicsAnimationGestures.csproj"

echo "G4-4 portable product and behavior validation: PASS"
echo "Run eng/validate-g4-4.ps1 on Windows for the pinned compiler, G4-3 regression, and package-only consumer aggregate gate."
