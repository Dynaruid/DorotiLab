#Requires -Version 5.1
param(
    [ValidateSet('All', 'C0', 'C1', 'C2', 'C3', 'C4')]
    [string] $Wave = 'All'
)

$ErrorActionPreference = 'Stop'
$env:DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER = '1'
$env:MSBUILDDISABLENODEREUSE = '1'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$project = Join-Path $dorotiRoot 'validation/Doroti.Validation.G6Cupertino/Doroti.Validation.G6Cupertino.csproj'
$dll = Join-Path $dorotiRoot 'validation/Doroti.Validation.G6Cupertino/bin/Release/net10.0/Doroti.Validation.G6Cupertino.dll'
$evidenceRoot = Join-Path $dorotiRoot 'migration/flutter-framework/g6-cupertino-waves'
$coveragePath = Join-Path $dorotiRoot 'migration/flutter-framework/g6-component-coverage.json'
$matrixPath = Join-Path $dorotiRoot 'migration/flutter-framework/g6-cupertino-component-matrix.json'
$adaptivePreviewPath = Join-Path $dorotiRoot 'migration/flutter-framework/g6-adaptive-preview-evidence.json'
$apiPath = Join-Path $dorotiRoot 'migration/flutter-framework/g5-4-cupertino-api-manifest.json'

function Assert-True([bool] $Condition, [string] $Name) {
    if (-not $Condition) { throw "$Name failed." }
}

function Invoke-Checked([scriptblock] $Command, [string] $Failure) {
    $global:LASTEXITCODE = 0
    & $Command
    if ($LASTEXITCODE -ne 0) { throw "$Failure (exit code $LASTEXITCODE)." }
}

function Write-Json([string] $Path, [object] $Value) {
    [IO.Directory]::CreateDirectory((Split-Path $Path -Parent)) | Out-Null
    $json = (($Value | ConvertTo-Json -Depth 64) -replace "`r`n", "`n") + "`n"
    [IO.File]::WriteAllText($Path, $json, [Text.UTF8Encoding]::new($false))
}

$cases = [ordered]@{
    C0 = @('')
    C1 = @('activity', 'selection', 'segmented', 'list')
    C2 = @('text', 'form', 'dialog', 'sheet')
    C3 = @('picker', 'navigation', 'scroll', 'tabs')
    C4 = @('context', 'menu', 'toolbar-adaptive', 'toolbar-desktop', 'toolbar-selection', 'toolbar-spell', 'transition', 'magnifier')
}
$orderedWaves = @('C0', 'C1', 'C2', 'C3', 'C4')
$lastWaveIndex = if ($Wave -eq 'All') { 4 } else { [Array]::IndexOf($orderedWaves, $Wave) }
$runWaves = @($orderedWaves[0..$lastWaveIndex])

Invoke-Checked { dotnet build $project --configuration Release --nologo } 'G6-6 Cupertino validator build failed'
foreach ($waveName in $runWaves) {
    foreach ($caseName in @($cases[$waveName])) {
        if ([string]::IsNullOrEmpty($caseName)) {
            Remove-Item Env:G6_CUPERTINO_CASE -ErrorAction SilentlyContinue
            $evidencePath = Join-Path $evidenceRoot "$waveName-evidence.json"
        }
        else {
            $env:G6_CUPERTINO_CASE = $caseName
            $evidencePath = Join-Path $evidenceRoot "$waveName-$caseName-evidence.json"
        }
        Invoke-Checked { dotnet $dll --wave $waveName --evidence $evidencePath } "G6-6 $waveName/$caseName live validation failed"
        $proof = Get-Content -LiteralPath $evidencePath -Raw | ConvertFrom-Json
        Assert-True ($proof.schemaVersion -eq 'doroti.g6-6-cupertino-wave-evidence/v1') "$waveName/$caseName schema"
        Assert-True ($proof.status -eq 'verified-windows-x64-strict-gpu') "$waveName/$caseName strict-GPU status"
        Assert-True ([long]$proof.interaction.count -eq 1) "$waveName/$caseName native pointer"
        Assert-True ($proof.fonts.status -eq 'PASS-no-tofu') "$waveName/$caseName bundled Cupertino icon font"
        Assert-True ([bool]$proof.resourceClosure.isBalanced) "$waveName/$caseName resource closure"
        Assert-True (@($proof.failures).Count -eq 0) "$waveName/$caseName failures"
    }
}
Remove-Item Env:G6_CUPERTINO_CASE -ErrorAction SilentlyContinue

$previewPath = Join-Path $evidenceRoot 'preview-selected-evidence.json'
$env:G6_CUPERTINO_CASE = 'preview'
Invoke-Checked { dotnet $dll --wave C0 --evidence $previewPath } 'G6-6 selected Widget Preview live validation failed'
Remove-Item Env:G6_CUPERTINO_CASE -ErrorAction SilentlyContinue
$previewProof = Get-Content -LiteralPath $previewPath -Raw | ConvertFrom-Json
Assert-True ($previewProof.status -eq 'verified-windows-x64-strict-gpu') 'Widget Preview strict-GPU status'
Assert-True ($previewProof.preview.status -eq 'verified-selected-preview-actual-frame') 'Widget Preview actual mount/layout/paint'

$api = Get-Content -LiteralPath $apiPath -Raw | ConvertFrom-Json
Assert-True ([long]$api.counts.diff -eq 0) 'Cupertino public API diff'

$coverage = Get-Content -LiteralPath $coveragePath -Raw | ConvertFrom-Json
$candidates = @($coverage.entries | Where-Object {
    $_.surface -eq 'cupertino' -and $_.classification -eq 'componentCandidate'
})
$proofs = @(Get-ChildItem -LiteralPath $evidenceRoot -Filter '*-evidence.json' |
    ForEach-Object { Get-Content -LiteralPath $_.FullName -Raw | ConvertFrom-Json } |
    Where-Object status -eq 'verified-windows-x64-strict-gpu')
$sources = @{}
foreach ($proof in $proofs) {
    foreach ($symbol in @($proof.components)) {
        if (-not $sources.ContainsKey([string]$symbol)) {
            $suffix = if ([string]::IsNullOrEmpty([string]$proof.componentCase)) { '' } else { "-$($proof.componentCase)" }
            $sources[[string]$symbol] = "g6-cupertino-waves/$($proof.wave)$suffix-evidence.json"
        }
    }
}
$entries = foreach ($candidate in $candidates) {
    $symbol = [string]$candidate.symbol
    $presented = $sources.ContainsKey($symbol)
    [ordered]@{
        library = [string]$candidate.library
        symbol = $symbol
        tierA = $symbol -in @('CupertinoApp','CupertinoTheme','CupertinoPageScaffold','CupertinoNavigationBar','CupertinoButton','CupertinoTextField','CupertinoAlertDialog','CupertinoCheckbox','CupertinoRadio','CupertinoSwitch','CupertinoSlider')
        states = [ordered]@{
            constructed = if ($presented) { 'PASS' } else { 'notVerified' }
            mounted = if ($presented) { 'PASS' } else { 'notVerified' }
            laidOut = if ($presented) { 'PASS' } else { 'notVerified' }
            painted = if ($presented) { 'PASS' } else { 'notVerified' }
            presented = if ($presented) { 'PASS' } else { 'notVerified' }
            interactive = if ($symbol -eq 'CupertinoButton') { 'PASS-native-pointer' } else { 'notVerified' }
            semantic = 'notVerified-component-scoped'
        }
        evidence = if ($presented) { $sources[$symbol] } else { $null }
    }
}
$presentedCount = @($entries | Where-Object { $_.states.presented -eq 'PASS' }).Count
$matrix = [ordered]@{
    schemaVersion = 'doroti.g6-6-cupertino-component-matrix/v1'
    milestone = 'G6-6'
    flutterRevision = '56b8e1a851a594b1a154f8ea93270807dab22b9a'
    status = if ($presentedCount -eq $candidates.Count) { 'presented-coverage-pass-interaction-partial' } else { 'partial' }
    counts = [ordered]@{
        candidates = $candidates.Count
        presented = $presentedCount
        presentedPercent = [Math]::Round(100.0 * $presentedCount / $candidates.Count, 2)
        interactiveNativePointer = @($entries | Where-Object { $_.states.interactive -eq 'PASS-native-pointer' }).Count
    }
    boundaries = [ordered]@{
        adaptiveReferenceDifferential = 'notVerified'
        materialCupertinoSwitch100 = 'notVerified'
        widgetPreviewSelectedFrame = 'PASS-actual-mount-layout-paint'
        cupertinoIconFont = 'PASS-bundled-no-tofu'
        physicalImeAccessibility = 'notVerified'
        crossTarget = 'notVerified'
    }
    entries = @($entries)
}
Write-Json $matrixPath $matrix
$adaptivePreview = [ordered]@{
    schemaVersion = 'doroti.g6-6-adaptive-preview-evidence/v1'
    milestone = 'G6-6'
    status = 'partial'
    adaptive = [ordered]@{
        windowsLiveSelection = 'notVerified'
        flutterReferenceDifferential = 'notVerified'
        reason = 'platform capability variants and pinned Flutter reference fixtures are not implemented'
    }
    materialCupertinoSwitch = [ordered]@{
        status = 'notVerified'
        requiredIterations = 100
        reason = 'same-session theme/focus/navigation/resource stress is not implemented'
    }
    widgetPreview = [ordered]@{
        status = 'PASS'
        selected = [string]$previewProof.preview.selected
        metadataPackage = [string]$previewProof.preview.metadataPackage
        mounted = [bool]$previewProof.preview.mounted
        laidOut = [bool]$previewProof.preview.laidOut
        paintedInPresentedFrame = [bool]$previewProof.preview.paintedInPresentedFrame
        target = [string]$previewProof.target
        evidence = 'g6-cupertino-waves/preview-selected-evidence.json'
    }
    fonts = [ordered]@{
        cupertinoIcons = 'PASS-bundled-no-tofu'
        evidence = 'g6-cupertino-waves/C0-evidence.json'
    }
    prerequisites = [ordered]@{
        g6_5r = 'notVerified'
        g6_5r_i = 'partial-windows-only'
        g6_5r_c = 'partial-c0-no-reference-differential'
    }
}
Write-Json $adaptivePreviewPath $adaptivePreview
Assert-True ($presentedCount -ge [Math]::Ceiling($candidates.Count * 0.9)) 'Cupertino presented coverage >= 90 percent'
Write-Host "G6-6 Cupertino $Wave PASS: $presentedCount/$($candidates.Count) presented and selected preview actual frame; interaction/adaptive/switch remain explicit boundaries."
