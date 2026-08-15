#!/usr/bin/env bash
set -euo pipefail

doroti_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

dotnet build "$doroti_root/src/Doroti.Host.Desktop.Framework/Doroti.Host.Desktop.Framework.csproj" --nologo
dotnet build "$doroti_root/../tools/Doroti.DartToCSharp/Doroti.DartToCSharp.csproj" --nologo
dotnet run --project "$doroti_root/validation/Doroti.Validation.G4Ui/Doroti.Validation.G4Ui.csproj" --no-restore

echo "G4-1 validation: PASS"
