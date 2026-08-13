#Requires -Version 5.1
param(
    [ValidateSet('All', 'M0', 'M1', 'M2', 'M3', 'M4', 'M5', 'M6')]
    [string] $Wave = 'All'
)

$ErrorActionPreference = 'Stop'
$env:DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER = '1'
$env:MSBUILDDISABLENODEREUSE = '1'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
$project = Join-Path $dorotiRoot 'validation/Doroti.Validation.G6MaterialGallery/Doroti.Validation.G6MaterialGallery.csproj'
$dll = Join-Path $dorotiRoot 'validation/Doroti.Validation.G6MaterialGallery/bin/Release/net10.0/Doroti.Validation.G6MaterialGallery.dll'
$artifactRoot = Join-Path $dorotiRoot 'artifacts/g6-material-gallery/win-x64'
$migrationRoot = Join-Path $dorotiRoot 'migration/flutter-framework'
$matrixPath = Join-Path $migrationRoot 'g6-material-component-matrix.json'
$aggregatePath = Join-Path $migrationRoot 'g6-material-gallery-evidence.json'
$apiPath = Join-Path $migrationRoot 'g5-4-material-api-manifest.json'
$orderedWaves = @('M0', 'M1', 'M2', 'M3', 'M4', 'M5', 'M6')

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

function Assert-WaveEvidence([string] $Name, [string] $Path) {
    $proof = Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json
    Assert-True ($proof.schemaVersion -eq 'doroti.g6-material-wave-evidence/v1') "$Name schema"
    Assert-True ($proof.wave -eq $Name) "$Name identity"
    Assert-True ($proof.status -eq 'verified-windows-x64-strict-gpu') "$Name live status"
    Assert-True ([bool]$proof.live.nativeWindow) "$Name native window"
    Assert-True ($proof.live.backend -eq 'skia-wgl-opengl-gpu') "$Name strict GPU backend"
    Assert-True (-not [bool]$proof.live.softwareFallback) "$Name software fallback"
    Assert-True ([long]$proof.live.presentedFrames -gt 0) "$Name presented frame"
    Assert-True ([long]$proof.live.failed -eq 0) "$Name failed frames"
    Assert-True ([long]$proof.live.cancelled -eq 0) "$Name cancelled frames"
    Assert-True ([long]$proof.live.changedPixels -ge 100) "$Name interaction raster change"
    Assert-True ([long]$proof.live.semanticsNodes -ge @($proof.framework.interactive).Count) "$Name semantics nodes"
    Assert-True (@($proof.framework.interactionTrace).Count -eq @($proof.framework.interactive).Count) "$Name interaction trace coverage"
    Assert-True ([bool]$proof.framework.stateVariants.defaults) "$Name default state"
    Assert-True ([bool]$proof.framework.stateVariants.disabled) "$Name disabled state"
    Assert-True ([bool]$proof.framework.stateVariants.focused) "$Name focused state"
    Assert-True ([bool]$proof.framework.stateVariants.hovered) "$Name hovered state"
    Assert-True ([bool]$proof.framework.stateVariants.pressedOrSelected) "$Name pressed/selected state"
    Assert-True (@($proof.framework.stateVariants.textScale).Count -ge 2) "$Name text scale variants"
    Assert-True ($proof.framework.stateVariants.dpi -eq 'actual-window-scale') "$Name DPI variant"
    Assert-True ([bool]$proof.live.resourceClosure.isBalanced) "$Name resource closure"
    Assert-True ([long]$proof.unsupportedSilentSuccessCount -eq 0) "$Name silent unsupported success"
    Assert-True ($proof.boundaries.physicalIme -eq 'notVerified') "$Name physical IME boundary"
    Assert-True ($proof.boundaries.physicalAccessibility -eq 'notVerified') "$Name physical accessibility boundary"
    return $proof
}

$lastWaveIndex = if ($Wave -eq 'All') { 6 } else { [Array]::IndexOf($orderedWaves, $Wave) }
$runWaves = @($orderedWaves[0..$lastWaveIndex])
Invoke-Checked { dotnet build $project --configuration Release --nologo } 'G6-5 gallery build failed'

$proofs = [Collections.Generic.List[object]]::new()
$m6Visual = $null
foreach ($name in $runWaves) {
    $waveRoot = Join-Path $artifactRoot $name
    [IO.Directory]::CreateDirectory($waveRoot) | Out-Null
    $path = Join-Path $waveRoot 'run.json'
    if ($name -eq 'M6') {
        $m6Visual = Join-Path $waveRoot 'visual.png'
        Invoke-Checked { dotnet $dll --wave $name --evidence $path --frames 3 --screenshot $m6Visual } "G6-5 $name gallery failed"
        Assert-True ((Test-Path -LiteralPath $m6Visual) -and ((Get-Item -LiteralPath $m6Visual).Length -gt 10000)) 'M6 visual capture'
    }
    else {
        Invoke-Checked { dotnet $dll --wave $name --evidence $path --frames 3 } "G6-5 $name gallery failed"
    }
    $proofs.Add((Assert-WaveEvidence $name $path))
}

$smokePath = Join-Path $artifactRoot "$($runWaves[-1])-30s-smoke.json"
Invoke-Checked { dotnet $dll --wave $runWaves[-1] --evidence $smokePath --frames 301 --cadence-duration-ms 30000 } 'G6-5 aggregate 30 second smoke failed'
$smoke = Assert-WaveEvidence $runWaves[-1] $smokePath
Assert-True ([double]$smoke.live.cadenceActualMs -ge 30000) '30 second smoke duration'
Assert-True ([long]$smoke.live.cadencePresented -ge 301) '30 second smoke cadence'

if ($lastWaveIndex -eq 6) {
    $api = Get-Content -LiteralPath $apiPath -Raw | ConvertFrom-Json
    Assert-True ([long]$api.counts.diff -eq 0) 'Material public API diff'

    $forbiddenPattern = 'Avalonia\.Controls|Avalonia\.Themes|Avalonia\.Markup\.Xaml|x:Class'
    $directDependencies = @(Get-ChildItem -LiteralPath (Join-Path $dorotiRoot 'src/Doroti.Flutter.Framework.Material') -File -Filter '*.cs' |
        Select-String -Pattern $forbiddenPattern)
    Assert-True ($directDependencies.Count -eq 0) 'direct Avalonia Controls/theme/XAML dependency count'

    $unsupported = @(
        [ordered]@{ wave='M2'; library='package:flutter/src/material/segmented_button.dart'; symbol='SegmentedButton'; cause='generated state-segment defaults are not yet live-safe'; owner='Doroti Flutter Framework'; followupCondition='add selected/disabled input fixture and pass strict-GPU first frame' },
        [ordered]@{ wave='M3'; library='package:flutter/src/material/dropdown_menu.dart'; symbol='DropdownMenu'; cause='menu overlay and editable selection path is not in the live fixture'; owner='Doroti Flutter Framework'; followupCondition='add overlay selection trace and balanced close lifecycle' },
        [ordered]@{ wave='M3'; library='package:flutter/src/material/search_anchor.dart'; symbol='SearchAnchor'; cause='search view route and suggestion overlay are not in the live fixture'; owner='Doroti Flutter Framework'; followupCondition='add suggestion callback, semantics, and route-close trace' },
        [ordered]@{ wave='M4'; library='package:flutter/src/material/stepper.dart'; symbol='Stepper'; cause='ticker and connector state path is not yet live-safe'; owner='Doroti Flutter Framework'; followupCondition='pass vertical/horizontal state transition fixture without ticker leak' },
        [ordered]@{ wave='M6'; library='package:flutter/src/material/date_picker.dart'; symbol='DatePickerDialog'; cause='calendar body is live, but dialog route lifecycle is not covered'; owner='Doroti Flutter Framework'; followupCondition='pass open/select/close route and focus restoration trace' },
        [ordered]@{ wave='M6'; library='package:flutter/src/material/time_picker.dart'; symbol='TimePickerDialog'; cause='time dialog and keyboard entry route are not covered'; owner='Doroti Flutter Framework'; followupCondition='pass dial/input mode callback and focus restoration trace' }
    )

    $representatives = @{ Shape='ShapeBorder'; Elevation='Material' }
    $interactive = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    foreach ($proof in $proofs) { foreach ($symbol in @($proof.framework.interactive)) { [void]$interactive.Add([string]$symbol) } }
    $nativePointerPath = Join-Path $migrationRoot 'g6-pointer-interaction-evidence.json'
    $nativeCalendarVerified = $false
    if (Test-Path -LiteralPath $nativePointerPath) {
        $nativePointer = Get-Content -LiteralPath $nativePointerPath -Raw | ConvertFrom-Json
        $nativeCalendarVerified = $nativePointer.status -eq 'verified-windows-x64-strict-gpu-native-input' -and
            [long]$nativePointer.result.directCallbackInvocationCount -eq 0 -and
            [long]$nativePointer.result.callbackCount -eq 1 -and
            [string]$nativePointer.result.selectedDate -eq '2026-08-14' -and
            [long]$nativePointer.result.activeHoverMaxChannelDelta -ge 8
    }
    $components = [Collections.Generic.List[object]]::new()
    foreach ($proof in $proofs) {
        foreach ($symbolObject in @($proof.framework.presented)) {
            $symbol = [string]$symbolObject
            $lookup = if ($representatives.ContainsKey($symbol)) { [string]$representatives[$symbol] } else { $symbol }
            $declaration = @($api.declarations | Where-Object name -eq $lookup | Select-Object -First 1)
            $library = if ($declaration.Count -gt 0) { [string]$declaration[0].library } elseif ($symbol -eq 'Icon') { 'package:flutter/src/widgets/icon.dart' } else { 'package:flutter/material.dart' }
            $components.Add([ordered]@{
                wave = [string]$proof.wave
                library = $library
                symbol = $symbol
                representativeSymbol = $lookup
                tierA = $symbol -in @('MaterialApp','Theme','Scaffold','AppBar','Card','ListTile','ElevatedButton','TextButton','Checkbox','Radio','Switch','Slider','TextField','NavigationBar','Drawer','TabBar','AlertDialog','SnackBar','Tooltip','LinearProgressIndicator','DataTable','CalendarDatePicker')
                presented = $true
                interactive = $interactive.Contains($symbol)
                inputStateTest = $symbol -eq 'CalendarDatePicker' -and $nativeCalendarVerified
                semanticTrace = $symbol -eq 'CalendarDatePicker' -and $nativeCalendarVerified
                nativeInputObserved = $symbol -eq 'CalendarDatePicker' -and $nativeCalendarVerified
                targetHit = $symbol -eq 'CalendarDatePicker' -and $nativeCalendarVerified
                callbackObserved = $symbol -eq 'CalendarDatePicker' -and $nativeCalendarVerified
                stateOrRasterObserved = $symbol -eq 'CalendarDatePicker' -and $nativeCalendarVerified
                interactionStatus = if (-not $interactive.Contains($symbol)) { 'notApplicable' } elseif ($symbol -eq 'CalendarDatePicker' -and $nativeCalendarVerified) { 'verified-native-input' } else { 'notVerified-native-input' }
                evidenceWave = [string]$proof.wave
                status = 'verified-windows-x64-strict-gpu'
            })
        }
    }
    foreach ($entry in $unsupported) {
        $components.Add([ordered]@{ wave=$entry.wave; library=$entry.library; symbol=$entry.symbol; representativeSymbol=$entry.symbol; tierA=$false; presented=$false; interactive=$true; inputStateTest=$false; semanticTrace=$false; evidenceWave=$null; status='unsupported'; cause=$entry.cause; owner=$entry.owner; followupCondition=$entry.followupCondition })
    }

    $presentedCount = @($components | Where-Object presented).Count
    $unsupportedCount = @($components | Where-Object { -not $_.presented }).Count
    $totalCount = $components.Count
    $coveragePercent = [Math]::Round(($presentedCount * 100.0) / $totalCount, 2)
    $unsupportedPercent = [Math]::Round(($unsupportedCount * 100.0) / $totalCount, 2)
    $tierA = @($components | Where-Object tierA)
    $interactiveTargets = @($components | Where-Object { $_.presented -and $_.interactive })
    Assert-True ($coveragePercent -ge 90.0) 'platform-independent Material family presentation coverage'
    Assert-True ($unsupportedPercent -le 10.0) 'unsupported Material family percentage'
    Assert-True (@($tierA | Where-Object { -not $_.presented }).Count -eq 0) 'Tier A presentation coverage'

    Write-Json $matrixPath ([ordered]@{
        schemaVersion = 'doroti.g6-material-component-matrix/v1'
        milestone = 'G6-5'
        flutterGitRevision = [string]$api.flutterGitRevision
        scope = [ordered]@{ definition='platform-independent public Material UI component/foundation families selected from the public manifest; non-UI data/theme helpers, platform services, aliases, private implementation types, and duplicate variants remain outside this family denominator'; publicApiDeclarationOccurrences=[long]$api.counts.declarationOccurrences; scopedFamilies=$totalCount }
        counts = [ordered]@{ presented=$presentedCount; unsupported=$unsupportedCount; coveragePercent=$coveragePercent; unsupportedPercent=$unsupportedPercent; tierA=$tierA.Count; tierAPresented=@($tierA | Where-Object presented).Count; interactiveTargets=$interactiveTargets.Count; interactiveTested=@($interactiveTargets | Where-Object { $_.inputStateTest -and $_.semanticTrace }).Count }
        components = $components
        boundaries = [ordered]@{ automatedNative='verified-windows-x64'; physicalIme='notVerified'; physicalAccessibility='notVerified'; linux='notVerified'; macOS='notVerified'; otherRids='notVerified' }
    })

    Write-Json $aggregatePath ([ordered]@{
        schemaVersion = 'doroti.g6-material-gallery-evidence/v1'
        milestone = 'G6-5'
        capturedAtUtc = [DateTimeOffset]::UtcNow
        status = 'verified-windows-x64-strict-gpu'
        waves = @($proofs | ForEach-Object { [ordered]@{ wave=$_.wave; status=$_.status; presented=@($_.framework.presented).Count; interactive=@($_.framework.interactive).Count; presentedFrames=[long]$_.live.presentedFrames; changedPixels=[long]$_.live.changedPixels; semanticsNodes=[long]$_.live.semanticsNodes; resourceBalanced=[bool]$_.live.resourceClosure.isBalanced } })
        coverage = [ordered]@{ presented=$presentedCount; scopedFamilies=$totalCount; coveragePercent=$coveragePercent; unsupported=$unsupportedCount; unsupportedPercent=$unsupportedPercent; tierAPresented=@($tierA | Where-Object presented).Count; tierATotal=$tierA.Count; interactiveTested=@($interactiveTargets | Where-Object { $_.inputStateTest -and $_.semanticTrace }).Count; interactiveTotal=$interactiveTargets.Count }
        aggregateSmoke = [ordered]@{ wave=[string]$smoke.wave; requestedMs=[double]$smoke.live.cadenceRequestedMs; actualMs=[double]$smoke.live.cadenceActualMs; cadencePresented=[long]$smoke.live.cadencePresented; backend=[string]$smoke.live.backend; failed=[long]$smoke.live.failed; cancelled=[long]$smoke.live.cancelled; softwareFallback=[bool]$smoke.live.softwareFallback; resourceBalanced=[bool]$smoke.live.resourceClosure.isBalanced }
        visualEvidence = [ordered]@{ wave='M6'; capture='strict-gpu-first-frame-readback'; path=$m6Visual.Substring($repoRoot.Length + 1).Replace('\', '/'); sha256=(Get-FileHash -LiteralPath $m6Visual -Algorithm SHA256).Hash.ToLowerInvariant() }
        contracts = [ordered]@{ materialPublicApiDiff=[long]$api.counts.diff; directAvaloniaControlsThemeXamlDependencies=$directDependencies.Count; unsupportedSilentSuccess=0 }
        boundaries = [ordered]@{ automatedNative='verified-windows-x64'; physicalIme='notVerified'; physicalAccessibility='notVerified'; linux='notVerified'; macOS='notVerified'; otherRids='notVerified' }
    })
}

Write-Output "G6-5 Material wave validation: PASS ($($runWaves -join ', '))"
Write-Output "30 second smoke: PASS ($($runWaves[-1]), $([Math]::Round([double]$smoke.live.cadenceActualMs)) ms)"
if ($lastWaveIndex -eq 6) {
    Write-Output "Component matrix: PASS ($presentedCount/$totalCount presented, $coveragePercent%; unsupported $unsupportedPercent%)"
    Write-Output "Evidence: $aggregatePath"
}
