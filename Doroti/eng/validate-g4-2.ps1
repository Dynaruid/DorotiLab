#Requires -Version 5.1
$ErrorActionPreference = 'Stop'

$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
. (Join-Path $PSScriptRoot 'local-storage.ps1')
$temporaryRoot = New-DorotiTemporaryDirectory -DorotiRoot $dorotiRoot -Name 'g4-2'

function Invoke-Checked {
    param([scriptblock]$Command)
    & $Command
    if ($LASTEXITCODE -ne 0) { throw "Command failed with exit code $LASTEXITCODE." }
}

try {
    Invoke-Checked { dotnet build "$dorotiRoot/src/Doroti.Flutter.Framework.Foundation/Doroti.Flutter.Framework.Foundation.csproj" --nologo }
    Invoke-Checked { dotnet run --project "$dorotiRoot/validation/Doroti.Validation.G4Foundation/Doroti.Validation.G4Foundation.csproj" }

    $packageRoot = Join-Path $temporaryRoot 'packages'
    New-Item -ItemType Directory -Path $packageRoot | Out-Null
    Invoke-Checked { dotnet pack "$dorotiRoot/src/Doroti.Flutter.Runtime/Doroti.Flutter.Runtime.csproj" --configuration Debug --nologo --no-build --output $packageRoot }
    Invoke-Checked { dotnet pack "$dorotiRoot/src/Doroti.Flutter.Ui/Doroti.Flutter.Ui.csproj" --configuration Debug --nologo --no-build --output $packageRoot }
    Invoke-Checked { dotnet pack "$dorotiRoot/src/Doroti.Flutter.Framework.Foundation/Doroti.Flutter.Framework.Foundation.csproj" --configuration Debug --nologo --no-build --output $packageRoot }

    $consumerRoot = Join-Path $temporaryRoot 'external-consumer'
    Copy-Item -Recurse "$dorotiRoot/validation/generated/g4-2-foundation-package-consumer" $consumerRoot
    Invoke-Checked {
        dotnet restore "$consumerRoot/G4.Foundation.PackageConsumer.csproj" `
            --source $packageRoot `
            --packages (Join-Path $temporaryRoot 'nuget-cache') `
            --force-evaluate `
            --nologo
    }
    $consumerOutput = dotnet run --project "$consumerRoot/G4.Foundation.PackageConsumer.csproj" --no-restore | Out-String
    if ($LASTEXITCODE -ne 0 -or $consumerOutput -notlike '*G4-2-FOUNDATION-PACKAGE-CONSUMER-PASS*') {
        Write-Error $consumerOutput
        throw "The package-only Foundation consumer did not report PASS."
    }

    $env:DOROTI_G4_2_PACKAGE_CONSUMER = 'pass'
    try {
        Invoke-Checked { dotnet run --project "$dorotiRoot/validation/Doroti.Validation.G4Foundation/Doroti.Validation.G4Foundation.csproj" }
    }
    finally {
        Remove-Item Env:DOROTI_G4_2_PACKAGE_CONSUMER -ErrorAction SilentlyContinue
    }

    Write-Output "G4-2 validation: PASS"
}
finally {
    Remove-DorotiTemporaryItem -DorotiRoot $dorotiRoot -Path $temporaryRoot
}
