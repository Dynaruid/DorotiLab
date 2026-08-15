#Requires -Version 5.1
$ErrorActionPreference = 'Stop'

$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$workspaceRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
. (Join-Path $PSScriptRoot 'local-storage.ps1')
$temporaryRoot = New-DorotiTemporaryDirectory -DorotiRoot $dorotiRoot -Name 'g4-boundary'

function Invoke-Checked {
    param([scriptblock]$Command)
    & $Command
    if ($LASTEXITCODE -ne 0) { throw "Command failed with exit code $LASTEXITCODE." }
}

try {
    Push-Location (Join-Path $workspaceRoot 'tools/Doroti.DartToCSharp/analyzer')
    try {
        Invoke-Checked {
            dart run tool/boundary/extract_g4_boundary.dart `
                (Join-Path $workspaceRoot 'reference/flutter-master/packages/flutter/lib') `
                (Join-Path $dorotiRoot 'migration/flutter-compat/flutter-api.json') `
                (Join-Path $temporaryRoot 'source-boundary.json')
        }
    }
    finally {
        Pop-Location
    }

    $generated = Get-Content (Join-Path $temporaryRoot 'source-boundary.json') -Raw
    $pinned = Get-Content (Join-Path $dorotiRoot 'migration/flutter-avalonia/source-boundary.json') -Raw
    if ($generated -ne $pinned) {
        throw "The regenerated source-boundary.json differs from the pinned artifact."
    }

    Push-Location $dorotiRoot
    try {
        Invoke-Checked { dotnet build tools/Doroti.SourceTools/Doroti.SourceTools.csproj --nologo }
        Invoke-Checked {
            dotnet tools/Doroti.SourceTools/bin/Debug/net10.0/Doroti.SourceTools.dll `
                flutter-avalonia-boundary-audit `
                --output (Join-Path $dorotiRoot 'artifacts/flutter-avalonia/g4-0-boundary-audit.json')
        }

        $fixtureLog = Join-Path $temporaryRoot 'forbidden-fixture.log'
        dotnet build validation/architecture/forbidden-framework-platform/Doroti.Framework.ForbiddenFixture.csproj `
            --nologo *> $fixtureLog
        if ($LASTEXITCODE -eq 0) {
            throw "The forbidden Flutter -> Platform fixture unexpectedly built successfully."
        }
        if (-not (Select-String -Path $fixtureLog -Pattern 'DOTARCH009' -Quiet)) {
            Get-Content $fixtureLog | Write-Error
            throw "The forbidden fixture failed without DOTARCH009."
        }
    }
    finally {
        Pop-Location
    }

    # The forbidden-fixture build above intentionally fails; do not let its
    # exit code leak into this script's exit status.
    $global:LASTEXITCODE = 0
    Write-Output "G4-0 boundary validation: PASS"
}
finally {
    Remove-DorotiTemporaryItem -DorotiRoot $dorotiRoot -Path $temporaryRoot
}
