#Requires -Version 5.1
$ErrorActionPreference = 'Stop'

$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
. (Join-Path $PSScriptRoot 'local-storage.ps1')
$artifactRoot = Join-Path $dorotiRoot 'artifacts/g5-2'
$graphicsPath = Join-Path $artifactRoot 'graphics.json'
$automationClientPath = Join-Path $artifactRoot 'automation-client.json'
$automationActionsPath = Join-Path $artifactRoot 'automation-actions.json'
$ownerAuditPath = Join-Path $dorotiRoot 'migration/flutter-framework/g5-2-owner-audit.json'
$milestonePath = Join-Path $dorotiRoot 'migration/flutter-avalonia/bridge-validation/g5-2.json'
$temporaryRoot = New-DorotiTemporaryDirectory -DorotiRoot $dorotiRoot -Name 'g5-2'
$packageRoot = Join-Path $temporaryRoot 'packages'
$consumerRoot = Join-Path $temporaryRoot 'external-consumer'
[IO.Directory]::CreateDirectory($artifactRoot) | Out-Null
[IO.Directory]::CreateDirectory($packageRoot) | Out-Null

function Invoke-Checked {
    param([scriptblock] $Command)
    & $Command
    if ($null -ne $LASTEXITCODE -and $LASTEXITCODE -ne 0) {
        throw "Command failed with exit code $LASTEXITCODE."
    }
}

function Write-AtomicJson {
    param([string] $Path, [object] $Value)
    $temporary = $Path + '.tmp-' + [Guid]::NewGuid().ToString('N')
    [IO.File]::WriteAllText(
        $temporary,
        ((($Value | ConvertTo-Json -Depth 16) -replace "`r`n", "`n") + "`n"),
        [Text.UTF8Encoding]::new($false))
    if (Test-Path -LiteralPath $Path) {
        $backup = $Path + '.bak-' + [Guid]::NewGuid().ToString('N')
        [IO.File]::Replace($temporary, $Path, $backup, $true)
        Remove-Item -LiteralPath $backup -Force
    }
    else { [IO.File]::Move($temporary, $Path) }
}

try {
    Invoke-Checked { & "$dorotiRoot/eng/promote-g5-2.ps1" }
    Invoke-Checked { & "$dorotiRoot/eng/audit-g5-2-owners.ps1" }
    Invoke-Checked { dotnet build "$dorotiRoot/Doroti.Product.slnx" --configuration Release --nologo }
    Invoke-Checked {
        dotnet build `
            "$dorotiRoot/validation/Doroti.Validation.G5PaintingRenderingSemantics/Doroti.Validation.G5PaintingRenderingSemantics.csproj" `
            --configuration Release --nologo
    }
    Invoke-Checked {
        dotnet run `
            --project "$dorotiRoot/validation/Doroti.Validation.G5PaintingRenderingSemantics/Doroti.Validation.G5PaintingRenderingSemantics.csproj" `
            --configuration Release --no-build -- $graphicsPath
    }
    Invoke-Checked { & "$dorotiRoot/eng/validate-g5-2-automation.ps1" }

    $projects = @(
        'Doroti.Flutter.Runtime',
        'Doroti.Flutter.Ui',
        'Doroti.Flutter.Framework.Foundation',
        'Doroti.Flutter.Framework.Scheduler',
        'Doroti.Flutter.Framework.Services',
        'Doroti.Flutter.Framework.Physics',
        'Doroti.Flutter.Framework.Animation',
        'Doroti.Flutter.Framework.Gestures',
        'Doroti.Flutter.Framework.Painting',
        'Doroti.Flutter.Framework.Semantics',
        'Doroti.Flutter.Framework.Rendering'
    )
    foreach ($project in $projects) {
        Invoke-Checked {
            dotnet pack "$dorotiRoot/src/$project/$project.csproj" `
                --configuration Release --nologo --no-build --output $packageRoot
        }
    }
    Copy-Item -Recurse `
        "$dorotiRoot/validation/generated/g5-2-painting-rendering-semantics-package-consumer" `
        $consumerRoot
    Invoke-Checked {
        dotnet restore "$consumerRoot/G5.PaintingRenderingSemantics.PackageConsumer.csproj" `
            --source $packageRoot `
            --packages (Join-Path $temporaryRoot 'nuget-cache') `
            --force-evaluate --nologo
    }
    $consumerOutput = dotnet run `
        --project "$consumerRoot/G5.PaintingRenderingSemantics.PackageConsumer.csproj" `
        --configuration Release --no-restore | Out-String
    if ($LASTEXITCODE -ne 0 -or $consumerOutput -notlike '*G5-2-PAINTING-RENDERING-SEMANTICS-PACKAGE-CONSUMER-PASS*') {
        Write-Error $consumerOutput
        throw 'The G5-2 package-only external consumer did not report PASS.'
    }

    Invoke-Checked { dotnet build "$dorotiRoot/tools/Doroti.SourceTools/Doroti.SourceTools.csproj" --configuration Release --nologo }
    Push-Location $dorotiRoot
    try {
        Invoke-Checked {
            dotnet "$dorotiRoot/tools/Doroti.SourceTools/bin/Release/net10.0/Doroti.SourceTools.dll" `
                flutter-avalonia-boundary-audit --write-owner-audit `
                --output "$artifactRoot/flutter-avalonia-boundary-audit.json"
        }
    }
    finally {
        Pop-Location
    }
    Invoke-Checked { & "$dorotiRoot/eng/validate-g4-4.ps1" }
    Invoke-Checked { dotnet build "$dorotiRoot/Doroti.slnx" --configuration Release --nologo }

    $graphics = Get-Content -LiteralPath $graphicsPath -Raw | ConvertFrom-Json
    $automationClient = Get-Content -LiteralPath $automationClientPath -Raw | ConvertFrom-Json
    $automationActions = Get-Content -LiteralPath $automationActionsPath -Raw | ConvertFrom-Json
    $ownerAudit = Get-Content -LiteralPath $ownerAuditPath -Raw | ConvertFrom-Json
    $capabilityMap = Get-Content -LiteralPath "$dorotiRoot/migration/flutter-avalonia/g5-2-graphics-semantics-capabilities.json" -Raw | ConvertFrom-Json
    $requiredActions = @('focus', 'tap', 'setText', 'scrollDown')
    $roundTripActions = @($automationActions.actions | ForEach-Object { $_.action } | Select-Object -Unique)
    $milestone = [ordered]@{
        schemaVersion = 'doroti.g5-2-closure/v1'
        milestone = 'G5-2'
        capturedAtUtc = [DateTime]::UtcNow.ToString('O')
        status = 'verified-windows-current-machine'
        compiler = [ordered]@{
            flutterRevision = '56b8e1a851a594b1a154f8ea93270807dab22b9a'
            selectedDeclarations = $graphics.api.selectedDeclarations
            dispositionEntries = $graphics.api.dispositionEntries
            publicApiMissing = $graphics.api.missing
            publicApiExtra = $graphics.api.extra
            generatedProductSources = $graphics.api.generatedProductSources
        }
        ownership = [ordered]@{
            duplicateHandwrittenRenderingOwnerInputs = @($ownerAudit.duplicateHandwrittenRenderingOwnerInputs).Count
            forbiddenLegacyProductReferences = @($ownerAudit.forbiddenProductReferences).Count
            boundaryCompileInputs = $ownerAudit.renderingBoundaryCompileInputs
            legacyCompatibilityProductCompile = $ownerAudit.legacyCompatibility.productCompile
        }
        graphics = [ordered]@{
            capabilityMappingPercent = $capabilityMap.summary.mappingPercent
            backend = $graphics.graphics.backendIdentity
            softwareFallback = $graphics.graphics.softwareFallbackUsed
            submitted = $graphics.graphics.submittedFrames
            presented = $graphics.graphics.presented
            superseded = $graphics.graphics.superseded
            stale = $graphics.graphics.stale
            failed = $graphics.graphics.failed
            cancelled = $graphics.graphics.cancelled
            queueHighWatermark = $graphics.graphics.queueHighWatermark
            surfaceGeneration = $graphics.graphics.surfaceGeneration
            recoveryCount = $graphics.graphics.recoveryCount
            visualMeanAbsoluteError = $graphics.graphics.visualMeanAbsoluteError
            visualTolerance = $graphics.graphics.visualTolerance
            evidenceBoundary = $graphics.evidenceBoundary
        }
        semanticsAutomation = [ordered]@{
            nativeEntrypoint = $automationClient.entrypoint
            exposedTree = $automationClient.exposedTree
            clientActions = $automationClient.actions
            flutterRoundTripEventCount = @($automationActions.actions).Count
            requiredRoundTripActionsPresent = @($requiredActions | Where-Object { $_ -notin $roundTripActions }).Count -eq 0
            internalDelegateOnly = $false
        }
        packageOnlyExternalConsumer = 'verified'
        predecessorValidateG4_4 = 'verified'
        rootBuild = 'verified-release-zero-warning-error'
        notVerified = @(
            'Physical cross-monitor DPI transition was not run; current 200% DPI metrics and generation were verified.',
            'Linux and macOS graphics/accessibility targets belong to G5-6L/G5-6M and were not run.'
        )
        evidence = @(
            'artifacts/g5-2/graphics.json',
            'artifacts/g5-2/automation-client.json',
            'artifacts/g5-2/automation-actions.json',
            'migration/flutter-framework/g5-2-owner-audit.json',
            'migration/flutter-avalonia/g5-2-graphics-semantics-capabilities.json',
            'migration/avalonia-shell/g5-2-skia-automation-provenance.json'
        )
    }
    Write-AtomicJson $milestonePath $milestone
    Write-Output 'G5-2 Painting/Rendering/Semantics product, strict-GPU, device-loss, external automation, package, and regression validation: PASS'
}
finally {
    Remove-DorotiTemporaryItem -DorotiRoot $dorotiRoot -Path $temporaryRoot
}
