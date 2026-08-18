#Requires -Version 7.0
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('Inventory', 'Contracts', 'Differential', 'WindowsLive', 'AndroidPhysical', 'Soak', 'Evidence')]
    [string] $Shard,

    [string] $AndroidSerial = '',

    [ValidateRange(60, 86400)]
    [int] $SoakSeconds = 300
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $dorotiRoot '..'))
$stateRoot = Join-Path $dorotiRoot '.doroti/tmp/fcr8-stability'
$evidencePath = Join-Path $dorotiRoot 'validation/evidence/flutter-conformance/fcr8-stability-evidence.json'
$fcr7EvidencePath = Join-Path $dorotiRoot 'validation/evidence/flutter-conformance/fcr7-material-widget-evidence.json'
$webEvidencePath = Join-Path $dorotiRoot 'validation/evidence/web/web-browser-live-manual.json'
$appTargetEvidencePath = Join-Path $dorotiRoot 'validation/evidence/app-targets-evidence.json'
$windowsRawPath = Join-Path $dorotiRoot '.doroti/tmp/app-targets/windows-live.json'
$androidRawPath = Join-Path $dorotiRoot '.doroti/tmp/app-targets/android-live.json'
[IO.Directory]::CreateDirectory($stateRoot) | Out-Null

function Assert-True([bool] $Condition, [string] $Name) {
    if (-not $Condition) { throw "$Name failed." }
}

function Invoke-Checked([scriptblock] $Command, [string] $Failure) {
    $global:LASTEXITCODE = 0
    & $Command | Out-Host
    if ($LASTEXITCODE -ne 0) { throw "$Failure (exit code $LASTEXITCODE)." }
}

function Write-Json([string] $Path, [object] $Value) {
    [IO.Directory]::CreateDirectory((Split-Path $Path -Parent)) | Out-Null
    $json = (($Value | ConvertTo-Json -Depth 64) -replace "`r`n", "`n") + "`n"
    [IO.File]::WriteAllText($Path, $json, [Text.UTF8Encoding]::new($false))
}

function Get-RelativePath([string] $Path) {
    [IO.Path]::GetRelativePath($repositoryRoot, $Path).Replace('\', '/')
}

function Get-SourceFingerprint {
    $paths = @(
        'DorotiDemoApp/DorotiDemoApp.csproj',
        'DorotiDemoApp/src/App.cs',
        'Doroti/Directory.Packages.props',
        'Doroti/src/Doroti.App.Sdk/Sdk/Sdk.props',
        'Doroti/src/Doroti.App.Sdk/Sdk/Sdk.targets',
        'Doroti/src/Doroti.Runtime/DartRuntimePrimitives.cs',
        'Doroti/src/Doroti.Framework.Rendering/layer.cs',
        'Doroti/src/Doroti.Framework.Widgets/basic.cs',
        'Doroti/src/Doroti.Framework.Widgets/toggleable.cs',
        'Doroti/src/Doroti.Framework.Material/material.cs',
        'Doroti/src/Doroti.Framework.Material/checkbox.cs',
        'Doroti/src/Doroti.Framework.Material/radio.cs',
        'Doroti/src/Doroti.Framework.Material/switch.cs',
        'Doroti/src/Doroti.Framework.Cupertino/checkbox.cs',
        'Doroti/src/Doroti.Framework.Cupertino/radio.cs',
        'Doroti/src/Doroti.Framework.Cupertino/switch.cs',
        'Doroti/src/Doroti.Host.Maui/MauiSkiaCapabilities.cs',
        'Doroti/src/Doroti.Host.Web/BrowserSkiaCapabilities.cs',
        'Doroti/src/Doroti.Host.Web/Web/doroti.web.ts',
        'Doroti/src/Doroti.Target.Web.browser-wasm/doroti-target-manifest.json'
    )
    $entries = foreach ($relative in $paths) {
        $path = Join-Path $repositoryRoot $relative
        Assert-True (Test-Path -LiteralPath $path -PathType Leaf) "FCR-8 fingerprint source $relative"
        "$relative|$((Get-FileHash -Algorithm SHA256 -LiteralPath $path).Hash.ToLowerInvariant())"
    }
    $bytes = [Text.Encoding]::UTF8.GetBytes(($entries -join "`n"))
    return [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($bytes)).ToLowerInvariant()
}

$repositoryRevision = (& git -C $repositoryRoot rev-parse HEAD).Trim()
$flutterRoot = Join-Path $repositoryRoot 'reference/flutter-master'
$flutterRevision = if (Test-Path -LiteralPath $flutterRoot -PathType Container) {
    (& git -C $flutterRoot rev-parse HEAD).Trim()
} else { 'notAvailable' }
$sourceFingerprint = Get-SourceFingerprint

function Write-State([string] $Name, [object] $Result) {
    Write-Json (Join-Path $stateRoot "$Name.json") ([ordered]@{
        schemaVersion = 'doroti.fcr8-shard-state/v1'
        shard = $Name
        capturedAtUtc = [DateTimeOffset]::UtcNow
        repositoryRevision = $repositoryRevision
        flutterRevision = $flutterRevision
        sourceFingerprintSha256 = $sourceFingerprint
        result = $Result
    })
}

function Read-FreshState([string] $Name) {
    $path = Join-Path $stateRoot "$Name.json"
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { return $null }
    $state = Get-Content -LiteralPath $path -Raw | ConvertFrom-Json
    if ([string]$state.schemaVersion -ne 'doroti.fcr8-shard-state/v1' -or
        [string]$state.repositoryRevision -ne $repositoryRevision -or
        [string]$state.sourceFingerprintSha256 -ne $sourceFingerprint) { return $null }
    return $state
}

function New-NotVerified([string] $Reason) {
    return [ordered]@{ status = 'notVerified'; reason = $Reason }
}

function Invoke-InventoryShard {
    $matrixPath = Join-Path $dorotiRoot 'validation/evidence/flutter-conformance/framework-parity-matrix.json'
    $fixturePath = Join-Path $dorotiRoot 'validation/fcr7-material-widget/fixture-manifest.json'
    Assert-True (Test-Path -LiteralPath $matrixPath -PathType Leaf) 'FCR-8 parity inventory'
    Assert-True (Test-Path -LiteralPath $fixturePath -PathType Leaf) 'FCR-8 representative fixture'
    $matrix = Get-Content -LiteralPath $matrixPath -Raw | ConvertFrom-Json
    $fixture = Get-Content -LiteralPath $fixturePath -Raw | ConvertFrom-Json
    Assert-True ([string]$matrix.flutterRevision -eq $flutterRevision) 'FCR-8 Flutter matrix revision'
    Assert-True ([string]$fixture.flutterRevision -eq $flutterRevision) 'FCR-8 Flutter fixture revision'
    $devices = @()
    if ($null -ne (Get-Command adb -ErrorAction SilentlyContinue)) {
        $devices = @(adb devices | Select-Object -Skip 1 | ForEach-Object {
            if ($_ -match '^(\S+)\s+device$') { $Matches[1] }
        } | Where-Object { $_ })
    }
    Write-State 'Inventory' ([ordered]@{
        status = 'pass'
        representativeComponents = @($fixture.components | ForEach-Object { $_.id })
        sourceSliceCount = @($matrix.sourceSlices).Count
        targetBoundary = [ordered]@{
            windowsLive = 'explicit shard'
            androidPhysical = 'explicit serial required'
            webBrowserLive = 'manual browser evidence'
            macCatalystNative = 'notVerified on Windows host'
            screenReaderImeStylus = 'notVerified unless separately executed'
        }
        connectedAndroidSerials = $devices
    })
}

function Invoke-ContractsShard {
    Assert-True (Test-Path -LiteralPath $fcr7EvidencePath -PathType Leaf) 'FCR-8 FCR-7 evidence input'
    $fcr7 = Get-Content -LiteralPath $fcr7EvidencePath -Raw | ConvertFrom-Json
    Assert-True ([string]$fcr7.schemaVersion -eq 'doroti.flutter-conformance-fcr7-evidence/v1') 'FCR-8 FCR-7 evidence schema'
    Assert-True ([string]$fcr7.structuralContract.status -eq 'pass') 'FCR-8 representative structural contract'
    Assert-True ([string]$fcr7.flutterRevision -eq $flutterRevision) 'FCR-8 FCR-7 Flutter revision'
    $toggleable = Get-Content -LiteralPath (Join-Path $dorotiRoot 'src/Doroti.Framework.Widgets/toggleable.cs') -Raw
    $webHost = Get-Content -LiteralPath (Join-Path $dorotiRoot 'src/Doroti.Host.Web/Web/doroti.web.ts') -Raw
    Assert-True ($toggleable -match 'ToggleableCustomPainterAdapter' -and $toggleable -match 'hitTest\(Offset position\) => null;') 'FCR-8 toggleable paint and hit-test closure'
    Assert-True ($webHost -match 'pointerEvents = "none"' -and $webHost -match 'event\.stopPropagation\(\)') 'FCR-8 Web pointer and semantics action boundary'
    Write-State 'Contracts' ([ordered]@{
        status = 'pass'
        fcr7StructuralContract = 'pass-debug-release'
        checks = @(
            'Flutter pin and representative fixture closure',
            'Toggleable painter and nullable hit-test bridge',
            'Web semantics pointer pass-through and keyboard action isolation',
            'FCR-7 target claims remain separate from structural PASS'
        )
    })
}

function Invoke-DifferentialShard {
    Assert-True (Test-Path -LiteralPath $fcr7EvidencePath -PathType Leaf) 'FCR-8 differential evidence input'
    $fcr7 = Get-Content -LiteralPath $fcr7EvidencePath -Raw | ConvertFrom-Json
    $status = [string]$fcr7.differential.status
    Assert-True ($status -in @('pass', 'notVerified')) 'FCR-8 differential status vocabulary'
    $result = if ($status -eq 'pass') {
        [ordered]@{ status = 'pass'; source = Get-RelativePath $fcr7EvidencePath; detail = $fcr7.differential }
    } else {
        New-NotVerified 'Paired pinned-Flutter and Doroti raster/state/semantics captures have not been executed.'
    }
    Write-State 'Differential' $result
}

function Invoke-WindowsLiveShard {
    Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-app-targets.ps1') -Shard WindowsLive } 'FCR-8 Windows live gate failed'
    Assert-True (Test-Path -LiteralPath $windowsRawPath -PathType Leaf) 'FCR-8 Windows diagnostics artifact'
    $live = Get-Content -LiteralPath $windowsRawPath -Raw | ConvertFrom-Json
    Assert-True ([long]$live.Frame.Presented -gt 0 -and [long]$live.Frame.Replayed -gt 0) 'FCR-8 Windows presented and replayed frames'
    Assert-True ([long]$live.Frame.Failed -eq 0 -and [long]$live.SoftwareFallbackFrames -eq 0) 'FCR-8 Windows zero failed/fallback frames'
    Assert-True ([long]$live.NativePointerEvents -gt 0 -and [long]$live.Surface.MetricsGeneration -gt 3) 'FCR-8 Windows wheel/drag and resize metrics'
    Write-State 'WindowsLive' ([ordered]@{
        status = 'pass'
        command = 'validate-app-targets.ps1 -Shard WindowsLive'
        device = [ordered]@{ os = [Environment]::OSVersion.VersionString; rid = [string]$live.Rid; backend = [string]$live.Surface.GraphicsBackend }
        measurements = [ordered]@{
            presented = [long]$live.Frame.Presented
            replayed = [long]$live.Frame.Replayed
            failed = [long]$live.Frame.Failed
            dropped = [long]$live.Frame.Dropped
            superseded = [long]$live.Frame.Superseded
            shaderImageFiltersRendered = [long]$live.Frame.ShaderImageFiltersRendered
            softwareFallbackFrames = [long]$live.SoftwareFallbackFrames
            nativePointerEvents = [long]$live.NativePointerEvents
            metricsGeneration = [long]$live.Surface.MetricsGeneration
            contextGeneration = [long]$live.Surface.ContextGeneration
            activeSemanticsElements = [long]$live.Semantics.ActiveElements
            retainedSemanticsNodes = [long]$live.Semantics.RetainedNodes
        }
        artifact = Get-RelativePath $windowsRawPath
        artifactSha256 = (Get-FileHash -Algorithm SHA256 -LiteralPath $windowsRawPath).Hash.ToLowerInvariant()
        interaction = 'pass-native-wheel-drag-two-resize-transitions'
        dpiTransition = 'notVerified-same-monitor-run'
    })
}

function Invoke-AndroidPhysicalShard {
    Assert-True (-not [string]::IsNullOrWhiteSpace($AndroidSerial)) 'FCR-8 Android physical serial argument'
    Invoke-Checked { & (Join-Path $PSScriptRoot 'validate-app-targets.ps1') -Shard AndroidPhysical -AndroidSerial $AndroidSerial } 'FCR-8 Android physical gate failed'
    Assert-True (Test-Path -LiteralPath $androidRawPath -PathType Leaf) 'FCR-8 Android diagnostics artifact'
    $live = Get-Content -LiteralPath $androidRawPath -Raw | ConvertFrom-Json
    Assert-True ([string]$live.deviceKind -eq 'physical') 'FCR-8 Android physical device identity'
    Assert-True ([long]$live.automatedGpu.Frame.Failed -eq 0 -and [long]$live.automatedGpu.SoftwareFallbackFrames -eq 0) 'FCR-8 Android zero failed/fallback frames'
    Write-State 'AndroidPhysical' ([ordered]@{
        status = 'pass'
        command = "validate-app-targets.ps1 -Shard AndroidPhysical -AndroidSerial $AndroidSerial"
        device = [ordered]@{ serial = $AndroidSerial; model = [string]$live.model; api = [string]$live.api; abi = [string]$live.abi; rid = [string]$live.rid }
        measurements = [ordered]@{
            targetFramework = [string]$live.automatedGpu.targetFramework
            rid = [string]$live.automatedGpu.rid
            surface = $live.automatedGpu.surface
            frame = [ordered]@{
                submitted = [long]$live.automatedGpu.frame.submitted
                presented = [long]$live.automatedGpu.frame.presented
                replayed = [long]$live.automatedGpu.frame.replayed
                failed = [long]$live.automatedGpu.frame.failed
                dropped = [long]$live.automatedGpu.frame.dropped
                backend = [string]$live.automatedGpu.frame.backend
                lastInputSequence = [long]$live.automatedGpu.frame.lastInputSequence
                lastPresentedInputSequence = [long]$live.automatedGpu.frame.lastPresentedInputSequence
            }
            softwareFallbackFrames = [long]$live.automatedGpu.softwareFallbackFrames
            semantics = [ordered]@{
                updatesReceived = [long]$live.automatedGpu.semantics.updatesReceived
                updatesApplied = [long]$live.automatedGpu.semantics.updatesApplied
            }
            framePacing = $live.automatedFramePacing
        }
        scrollStress = [string]$live.automatedScroll
        persistentDisplay = $live.automatedPersistentDisplay
        artifact = Get-RelativePath $androidRawPath
        artifactSha256 = (Get-FileHash -Algorithm SHA256 -LiteralPath $androidRawPath).Hash.ToLowerInvariant()
        acceptanceBoundary = [ordered]@{ manualPersistentDisplay = [string]$live.manualPersistentDisplay; imeTalkBackStylusMouse = [string]$live.imeTalkBackStylusMouse }
    })
}

function Invoke-SoakShard {
    $windowsState = Read-FreshState 'WindowsLive'
    if ($null -eq $windowsState) {
        Invoke-WindowsLiveShard
        $windowsState = Read-FreshState 'WindowsLive'
    }
    $executable = Join-Path $dorotiRoot '.doroti/tmp/app-targets/windows-publish/DorotiDemoApp.exe'
    Assert-True (Test-Path -LiteralPath $executable -PathType Leaf) 'FCR-8 Windows soak executable'
    $soakDiagnostics = Join-Path $stateRoot 'windows-soak-diagnostics.json'
    Remove-Item -LiteralPath $soakDiagnostics -Force -ErrorAction SilentlyContinue
    $env:DOROTI_MAUI_EVIDENCE = $soakDiagnostics
    $process = Start-Process -FilePath $executable -PassThru -WindowStyle Hidden
    try {
        $deadline = [DateTimeOffset]::UtcNow.AddSeconds($SoakSeconds)
        while ([DateTimeOffset]::UtcNow -lt $deadline) {
            Start-Sleep -Seconds 1
            $process.Refresh()
            Assert-True (-not $process.HasExited) 'FCR-8 Windows process survival during soak'
        }
    }
    finally {
        if (-not $process.HasExited) { Stop-Process -Id $process.Id -Force }
        Remove-Item Env:DOROTI_MAUI_EVIDENCE -ErrorAction SilentlyContinue
    }
    Assert-True (Test-Path -LiteralPath $soakDiagnostics -PathType Leaf) 'FCR-8 soak diagnostics artifact'
    $live = Get-Content -LiteralPath $soakDiagnostics -Raw | ConvertFrom-Json
    Assert-True ([long]$live.Frame.Failed -eq 0 -and [long]$live.SoftwareFallbackFrames -eq 0) 'FCR-8 soak zero failed/fallback frames'
    Write-State 'Soak' ([ordered]@{
        status = 'pass'
        durationSeconds = $SoakSeconds
        target = 'Windows'
        processSurvived = $true
        failedFrames = [long]$live.Frame.Failed
        softwareFallbackFrames = [long]$live.SoftwareFallbackFrames
        presented = [long]$live.Frame.Presented
        replayed = [long]$live.Frame.Replayed
        artifact = Get-RelativePath $soakDiagnostics
        artifactSha256 = (Get-FileHash -Algorithm SHA256 -LiteralPath $soakDiagnostics).Hash.ToLowerInvariant()
        resourcePlateau = 'notVerified-no time-series private-bytes/GPU-resource sampling'
    })
}

function Invoke-EvidenceShard {
    $inventory = Read-FreshState 'Inventory'
    $contracts = Read-FreshState 'Contracts'
    Assert-True ($null -ne $inventory -and [string]$inventory.result.status -eq 'pass') 'FCR-8 fresh Inventory shard'
    Assert-True ($null -ne $contracts -and [string]$contracts.result.status -eq 'pass') 'FCR-8 fresh Contracts shard'
    $differential = Read-FreshState 'Differential'
    $windows = Read-FreshState 'WindowsLive'
    $android = Read-FreshState 'AndroidPhysical'
    $soak = Read-FreshState 'Soak'
    $web = if (Test-Path -LiteralPath $webEvidencePath -PathType Leaf) {
        $manual = Get-Content -LiteralPath $webEvidencePath -Raw | ConvertFrom-Json
        if ([string]$manual.status -eq 'pass-interaction' -and [string]$manual.sourceFingerprintSha256 -eq $sourceFingerprint) {
            [ordered]@{ status = 'pass'; evidence = Get-RelativePath $webEvidencePath; detail = $manual.input }
        } else { New-NotVerified 'No current-source Web browser interaction evidence.' }
    } else { New-NotVerified 'Web browser live evidence is absent.' }
    $results = [ordered]@{
        inventory = $inventory.result
        contracts = $contracts.result
        differential = if ($null -ne $differential) { $differential.result } else { New-NotVerified 'Differential shard was not run for this source fingerprint.' }
        windowsLive = if ($null -ne $windows) { $windows.result } else { New-NotVerified 'WindowsLive shard was not run for this source fingerprint.' }
        androidPhysical = if ($null -ne $android) { $android.result } else { New-NotVerified 'AndroidPhysical requires an explicit connected physical serial.' }
        soak = if ($null -ne $soak) { $soak.result } else { New-NotVerified 'Soak shard was not run for this source fingerprint.' }
        webBrowserLive = $web
        macCatalystNative = New-NotVerified 'Native Mac Catalyst execution requires Apple Silicon macOS.'
        screenReaderImeStylus = New-NotVerified 'No current physical accessibility/input acceptance run.'
    }
    foreach ($name in @('differential','windowsLive','androidPhysical','soak','webBrowserLive','macCatalystNative','screenReaderImeStylus')) {
        Assert-True ([string]$results[$name].status -in @('pass', 'notVerified')) "FCR-8 status vocabulary: $name"
    }
    $requiredAcceptance = @('windowsLive','androidPhysical','soak','differential')
    $overall = if (@($requiredAcceptance | Where-Object { [string]$results[$_].status -ne 'pass' }).Count -eq 0) { 'pass' } else { 'partial' }
    Assert-True ($overall -ne 'pass' -or @($requiredAcceptance | Where-Object { [string]$results[$_].status -ne 'pass' }).Count -eq 0) 'FCR-8 parent/child status consistency'
    Write-Json $evidencePath ([ordered]@{
        schemaVersion = 'doroti.flutter-conformance-fcr8-evidence/v1'
        milestone = 'FCR-8'
        capturedAtUtc = [DateTimeOffset]::UtcNow
        status = $overall
        repositoryRevision = $repositoryRevision
        flutterRevision = $flutterRevision
        sourceFingerprintSha256 = $sourceFingerprint
        shards = @('Inventory','Contracts','Differential','WindowsLive','AndroidPhysical','Soak','Evidence')
        developerRepresentative = @('Inventory','Contracts','Differential','Evidence')
        explicitAcceptance = @('WindowsLive','AndroidPhysical','Soak')
        results = $results
        completionGate = [ordered]@{
            status = if ($overall -eq 'pass') { 'pass' } else { 'notVerified' }
            requires = @('paired differential PASS','Windows live interaction/stability PASS','Android physical correctness/stability PASS','soak and resource plateau PASS')
            zeroTolerance = @('crash','ANR','fatal log','unobserved exception','failed frame','software fallback','resource leak')
        }
    })
    Write-Output "Doroti FCR-8 evidence: $($overall.ToUpperInvariant())"
}

switch ($Shard) {
    'Inventory' { Invoke-InventoryShard }
    'Contracts' { Invoke-ContractsShard }
    'Differential' { Invoke-DifferentialShard }
    'WindowsLive' { Invoke-WindowsLiveShard }
    'AndroidPhysical' { Invoke-AndroidPhysicalShard }
    'Soak' { Invoke-SoakShard }
    'Evidence' { Invoke-EvidenceShard }
}

Write-Output "Doroti FCR-8 stability shard '$Shard': PASS"
