#Requires -Version 5.1
$ErrorActionPreference = 'Stop'

$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path

function Invoke-Checked {
    param([scriptblock]$Command)
    & $Command
    if ($LASTEXITCODE -ne 0) { throw "Command failed with exit code $LASTEXITCODE." }
}

Invoke-Checked { dotnet build "$dorotiRoot/src/Doroti.Host.Desktop.Framework/Doroti.Host.Desktop.Framework.csproj" --nologo }
Invoke-Checked { dotnet build "$dorotiRoot/../tools/Doroti.DartToCSharp/Doroti.DartToCSharp.csproj" --nologo }
Invoke-Checked { dotnet run --project "$dorotiRoot/validation/Doroti.Validation.G4Ui/Doroti.Validation.G4Ui.csproj" }

Write-Output "G4-1 validation: PASS"
