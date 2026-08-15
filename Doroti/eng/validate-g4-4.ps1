#Requires -Version 5.1
$ErrorActionPreference = 'Stop'

$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
. (Join-Path $PSScriptRoot 'local-storage.ps1')
$temporaryRoot = New-DorotiTemporaryDirectory -DorotiRoot $dorotiRoot -Name 'g4-4'
$packageRoot = Join-Path $temporaryRoot 'packages'
$consumerRoot = Join-Path $temporaryRoot 'external-consumer'
New-Item -ItemType Directory -Path $packageRoot -Force | Out-Null

function Invoke-Checked {
    param([scriptblock]$Command)
    & $Command
    if ($LASTEXITCODE -ne 0) { throw "Command failed with exit code $LASTEXITCODE." }
}

try {
    Invoke-Checked { & "$dorotiRoot/eng/validate-g4-3.ps1" }
    Invoke-Checked { & "$dorotiRoot/eng/validate-g4-4-compiler.ps1" }
    Invoke-Checked { dotnet build "$dorotiRoot/Doroti.Product.slnx" --nologo }
    Invoke-Checked { dotnet run --project "$dorotiRoot/validation/Doroti.Validation.G4PhysicsAnimationGestures/Doroti.Validation.G4PhysicsAnimationGestures.csproj" }

    foreach ($project in @(
        'Doroti.Runtime',
        'Doroti.Ui',
        'Doroti.Framework.Foundation',
        'Doroti.Framework.Scheduler',
        'Doroti.Framework.Services',
        'Doroti.Framework.Physics',
        'Doroti.Framework.Animation',
        'Doroti.Framework.Gestures')) {
        Invoke-Checked {
            dotnet pack "$dorotiRoot/src/$project/$project.csproj" `
                --configuration Debug --nologo --no-build --output $packageRoot
        }
    }

    Copy-Item -Recurse "$dorotiRoot/validation/generated/g4-4-physics-animation-gestures-package-consumer" $consumerRoot
    Invoke-Checked {
        dotnet restore "$consumerRoot/G4.PhysicsAnimationGestures.PackageConsumer.csproj" `
            --source $packageRoot `
            --packages (Join-Path $temporaryRoot 'nuget-cache') `
            --force-evaluate `
            --nologo
    }
    $consumerOutput = dotnet run --project "$consumerRoot/G4.PhysicsAnimationGestures.PackageConsumer.csproj" --no-restore | Out-String
    if ($LASTEXITCODE -ne 0 -or $consumerOutput -notlike '*G4-4-PHYSICS-ANIMATION-GESTURES-PACKAGE-CONSUMER-PASS*') {
        Write-Error $consumerOutput
        throw 'The package-only G4-4 consumer did not report PASS.'
    }

    $evidencePath = "$dorotiRoot/migration/flutter-avalonia/bridge-validation/g4-4.json"
    $evidence = Get-Content $evidencePath -Raw | ConvertFrom-Json
    $evidence | Add-Member -NotePropertyName aggregateGates -NotePropertyValue ([ordered]@{
        g4_3Regression = $true
        compilerCandidate = $true
        productBuild = $true
        externalPackageConsumer = $true
    }) -Force
    $evidence | Add-Member -NotePropertyName validatedAtUtc -NotePropertyValue ([DateTime]::UtcNow.ToString('O')) -Force
    $temporaryEvidence = $evidencePath + '.tmp-' + [Guid]::NewGuid().ToString('N')
    [IO.File]::WriteAllText($temporaryEvidence, (($evidence | ConvertTo-Json -Depth 12) + "`n"), [Text.UTF8Encoding]::new($false))
    if (Test-Path -LiteralPath $evidencePath) {
        $backupEvidence = $evidencePath + '.bak-' + [Guid]::NewGuid().ToString('N')
        [IO.File]::Replace($temporaryEvidence, $evidencePath, $backupEvidence, $true)
        Remove-Item -LiteralPath $backupEvidence
    }
    else { [IO.File]::Move($temporaryEvidence, $evidencePath) }

    Write-Output 'G4-4 Physics/Animation/Gestures promotion, behavior, package, and regression validation: PASS'
}
finally {
    Remove-DorotiTemporaryItem -DorotiRoot $dorotiRoot -Path $temporaryRoot
}
