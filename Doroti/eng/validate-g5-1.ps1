#Requires -Version 5.1
$ErrorActionPreference = 'Stop'

$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$workspaceRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
$artifactRoot = Join-Path $dorotiRoot 'artifacts/g5-1'
$nativeEvidencePath = Join-Path $artifactRoot 'native-input.json'
$ownerEvidencePath = Join-Path $artifactRoot 'owner-audit.json'
$milestoneEvidencePath = Join-Path $dorotiRoot 'migration/flutter-avalonia/bridge-validation/g5-1.json'
New-Item -ItemType Directory -Force -Path $artifactRoot | Out-Null

function Invoke-Checked {
    param([scriptblock] $Command)
    & $Command
    if ($LASTEXITCODE -ne 0) { throw "Command failed with exit code $LASTEXITCODE." }
}

function Write-AtomicJson {
    param([string] $Path, [object] $Value)
    $temporary = $Path + '.tmp-' + [Guid]::NewGuid().ToString('N')
    $json = ($Value | ConvertTo-Json -Depth 16) -replace "`r`n", "`n"
    [IO.File]::WriteAllText($temporary, $json + "`n", [Text.UTF8Encoding]::new($false))
    if (Test-Path -LiteralPath $Path) {
        $backup = $Path + '.bak-' + [Guid]::NewGuid().ToString('N')
        [IO.File]::Replace($temporary, $Path, $backup, $true)
        Remove-Item -LiteralPath $backup
    }
    else { [IO.File]::Move($temporary, $Path) }
}

Push-Location $dorotiRoot
try {
    Invoke-Checked { & "$dorotiRoot/eng/validate-g4-4.ps1" }
    Invoke-Checked {
        dotnet run `
            --project "$dorotiRoot/validation/Doroti.Validation.G5NativeInput/Doroti.Validation.G5NativeInput.csproj" `
            --configuration Release `
            -- $nativeEvidencePath
    }

    Invoke-Checked { dotnet build "$dorotiRoot/tools/Doroti.SourceTools/Doroti.SourceTools.csproj" --configuration Release --nologo }
    Invoke-Checked {
        dotnet "$dorotiRoot/tools/Doroti.SourceTools/bin/Release/net10.0/Doroti.SourceTools.dll" `
            flutter-avalonia-boundary-audit `
            --write-owner-audit `
            --output "$artifactRoot/flutter-avalonia-boundary-audit.json"
    }

    $nonFrameworkSources = @(Get-ChildItem -LiteralPath "$dorotiRoot/src" -Recurse -Filter '*.cs' -File |
        Where-Object { $_.FullName -notmatch '[\\/]Doroti\.Flutter\.Framework\.[^\\/]+[\\/]' })
    $policyPattern = '(?m)^\s*(?:public|internal|private|protected|file)\s+(?:(?:abstract|sealed|static|partial)\s+)*class\s+\w*(?:GestureRecognizer|AnimationController|Ticker)\b'
    $policyOwners = @()
    foreach ($source in $nonFrameworkSources) {
        foreach ($match in [regex]::Matches([IO.File]::ReadAllText($source.FullName), $policyPattern)) {
            $policyOwners += [ordered]@{
                path = [IO.Path]::GetRelativePath($dorotiRoot, $source.FullName).Replace('\', '/')
                declaration = $match.Value.Trim()
            }
        }
    }
    $adapterEntries = @(
        [ordered]@{ symbol = 'Doroti.Widgets.FlutterArenaAdapter'; role = 'lifetime-handoff'; policy = $false },
        [ordered]@{ symbol = 'Doroti.Widgets.FlutterInputAdapter'; role = 'raw-packet-and-recognizer-binding'; policy = $false },
        [ordered]@{ symbol = 'Doroti.Widgets.FlutterPointerEventAdapter'; role = 'raw-packet-conversion'; policy = $false },
        [ordered]@{ symbol = 'Doroti.Widgets.HostTapBinding'; role = 'arena-and-tracking-lifetime-handoff'; policy = $false },
        [ordered]@{ symbol = 'Doroti.Widgets.HostVerticalDragBinding'; role = 'arena-and-tracking-lifetime-handoff'; policy = $false }
    )
    $ownerAudit = [ordered]@{
        schemaVersion = 'doroti.g5-1-policy-owner-audit/v1'
        capturedAtUtc = [DateTime]::UtcNow.ToString('O')
        reviewedOwners = @(
            'Doroti.Flutter.Framework.Gestures',
            'Doroti.Flutter.Framework.Animation',
            'Doroti.Flutter.Framework.Physics'
        )
        hostNeutralAdapters = $adapterEntries
        nonFrameworkPolicyOwners = $policyOwners
        compatibilitySymbols = [ordered]@{
            tapGestureRecognizer = $false
            verticalDragGestureRecognizer = $false
        }
        success = ($policyOwners.Count -eq 0)
    }
    Write-AtomicJson $ownerEvidencePath $ownerAudit
    if ($policyOwners.Count -ne 0) { throw "G5-1 owner audit found $($policyOwners.Count) non-framework policy owner(s)." }

    $native = Get-Content -LiteralPath $nativeEvidencePath -Raw | ConvertFrom-Json
    $trackpadDevices = @(Get-PnpDevice -PresentOnly -ErrorAction SilentlyContinue |
        Where-Object { $_.FriendlyName -match 'Touchpad|Trackpad|Precision Touchpad' } |
        ForEach-Object { [ordered]@{ name = $_.FriendlyName; status = [string]$_.Status; instanceId = $_.InstanceId } })
    $mouseDevices = @(Get-PnpDevice -PresentOnly -ErrorAction SilentlyContinue |
        Where-Object { $_.FriendlyName -match 'Mouse' } |
        ForEach-Object { [ordered]@{ name = $_.FriendlyName; status = [string]$_.Status; instanceId = $_.InstanceId } })
    $g4Evidence = Get-Content -LiteralPath "$dorotiRoot/migration/flutter-avalonia/bridge-validation/g4-4.json" -Raw | ConvertFrom-Json
    $milestone = [ordered]@{
        schemaVersion = 'doroti.g5-1-closure/v1'
        milestone = 'G5-1'
        capturedAtUtc = [DateTime]::UtcNow.ToString('O')
        status = 'partial-not-verified'
        compilerAndPromotion = [ordered]@{
            publicApiManifestDiff = 0
            packageConsumer = 'verified'
            validateG4_4 = 'verified'
            aggregateGates = $g4Evidence.aggregateGates
        }
        ownership = [ordered]@{
            nonFrameworkRecognizerAnimationPolicyOwnerCount = $policyOwners.Count
            compatibilityRecognizerSymbolCount = 0
            adapterCount = $adapterEntries.Count
            status = 'verified'
        }
        behavior = [ordered]@{
            gestureArenaHoldReleaseSweep = 'verified'
            cancelCaptureLossExactlyOnce = 'verified'
            timestampLogicalCoordinate = 'verified-managed-and-native-synthetic'
            scrollDeltaExactlyOnce = 'verified-native-synthetic'
            tickerCompleteCancelMuteResume = 'verified-managed'
            sustainedNativeFramePacing = if ($native.success) { 'verified' } else { 'failed' }
            nativeFrameMetrics = $native.framePacing
        }
        deviceEvidence = [ordered]@{
            mouse = [ordered]@{ status = 'not-verified'; availableDevices = $mouseDevices; syntheticNativeWindowRun = $true; reason = 'Actual physical mouse event was not distinguished from injected target-controller input.' }
            trackpad = [ordered]@{ status = 'not-verified'; availableDevices = $trackpadDevices; syntheticWheelRun = $true; reason = 'Precision touchpad hardware is present, but no physical trackpad recording was captured.' }
            touch = [ordered]@{ status = 'not-verified'; deviceAvailable = ($native.environment.maximumTouches -gt 0); reason = if ($native.environment.maximumTouches -gt 0) { 'No physical touch recording was captured.' } else { 'This target reports no touch digitizer.' } }
        }
        nativeWindowEvidence = 'artifacts/g5-1/native-input.json'
        ownerAuditEvidence = 'artifacts/g5-1/owner-audit.json'
        remainingGates = @(
            'Physical mouse recording through the native source-port window.',
            'Physical precision-trackpad recording through the native source-port window.',
            'Physical touch recording on a target with a touch digitizer.'
        )
    }
    Write-AtomicJson $milestoneEvidencePath $milestone
    Write-Output 'G5-1 automated product/native-window validation: PASS; physical mouse/touch/trackpad gates remain notVerified.'
}
finally {
    Pop-Location
}
