#Requires -Version 7.0
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$dorotiRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $dorotiRoot '..'))
$flutterRoot = Join-Path $repositoryRoot 'reference/flutter-master'
$manifestPath = Join-Path $dorotiRoot 'validation/fcr5-scroll/fixture-manifest.json'
$contractProject = Join-Path $dorotiRoot 'validation/fcr5-scroll/Doroti.Validation.Fcr5Scroll.csproj'
$evidencePath = Join-Path $dorotiRoot 'validation/evidence/flutter-conformance/fcr5-scroll-evidence.json'

function Assert-True([bool] $Condition, [string] $Name) {
    if (-not $Condition) { throw "$Name failed." }
}

function Read-Text([string] $Path) {
    Assert-True (Test-Path -LiteralPath $Path -PathType Leaf) "source exists: $Path"
    return Get-Content -Raw -LiteralPath $Path
}

function Invoke-Contract([string] $Configuration) {
    Push-Location $repositoryRoot
    try {
        # Contract execution has the repository-wide 20-minute test ceiling.
        $outputPath = Join-Path ([IO.Path]::GetTempPath()) ("doroti-fcr5-$Configuration-$([guid]::NewGuid()).log")
        try {
            $process = Start-Process dotnet -ArgumentList @('run', '--project', $contractProject, '-c', $Configuration, '--nologo') `
                -NoNewWindow -PassThru -RedirectStandardOutput $outputPath -RedirectStandardError "$outputPath.err"
            Assert-True ($process.WaitForExit(1200000)) "FCR-5 runtime contract timeout ($Configuration)"
            $output = ((Get-Content -Raw -LiteralPath $outputPath) + (Get-Content -Raw -LiteralPath "$outputPath.err"))
            Assert-True ($process.ExitCode -eq 0) "FCR-5 runtime contract exit ($Configuration): $output"
            Assert-True ($output.Contains("FCR-5 scroll runtime contract: PASS (configuration=$Configuration", [StringComparison]::Ordinal)) "FCR-5 runtime contract result ($Configuration)"
        }
        finally { Remove-Item -LiteralPath $outputPath, "$outputPath.err" -Force -ErrorAction SilentlyContinue }
    }
    finally { Pop-Location }
}

Assert-True (Test-Path -LiteralPath $flutterRoot -PathType Container) 'pinned Flutter checkout exists'
$manifest = Get-Content -Raw -LiteralPath $manifestPath | ConvertFrom-Json
Assert-True ([string]$manifest.schemaVersion -eq 'doroti.flutter-conformance-fcr5-fixture/v1') 'FCR-5 fixture schema'
$flutterRevision = (& git -C $flutterRoot rev-parse HEAD).Trim()
Assert-True ($flutterRevision -eq [string]$manifest.flutterRevision) "Flutter revision pin: expected $($manifest.flutterRevision), got $flutterRevision"
foreach ($source in @($manifest.sources)) {
    $path = Join-Path $flutterRoot ([string]$source.path).Replace('/', '\')
    $text = Read-Text $path
    Assert-True ((Get-FileHash -Algorithm SHA256 -LiteralPath $path).Hash.ToLowerInvariant() -eq [string]$source.sha256) "Flutter source hash: $($source.path)"
    foreach ($anchor in @($source.anchors)) { Assert-True ($text.Contains([string]$anchor, [StringComparison]::Ordinal)) "Flutter source anchor: $($source.path) -> $anchor" }
}

$controller = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Framework.Widgets/scroll_controller.cs')
Assert-True ($controller.Contains('var futures = new List<Future>();', [StringComparison]::Ordinal)) 'controller snapshots per-position animations'
Assert-True ($controller.Contains('foreach (var position in this._positions.ToArray())', [StringComparison]::Ordinal)) 'controller does not enumerate a mutable position list'
Assert-True ($controller.Contains('DartAsyncRuntime.wait<object?>(futures)', [StringComparison]::Ordinal)) 'controller waits for every initial position'

$activity = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Framework.Widgets/scroll_activity.cs')
Assert-True ($activity.Contains('CreateUnbounded(', [StringComparison]::Ordinal) -and $activity.Contains('value: from,', [StringComparison]::Ordinal)) 'driven activity creates its controller at the requested start offset'
Assert-True ($activity.Contains('DartRuntimePrimitives.Observe(', [StringComparison]::Ordinal) -and $activity.Contains('DrivenScrollActivity.animateTo', [StringComparison]::Ordinal)) 'driven animation completion is observed'

$scrollView = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Framework.Widgets/scroll_view.cs')
Assert-True ($scrollView.Contains('PrimaryScrollController.shouldInherit(context, this.scrollDirection)', [StringComparison]::Ordinal)) 'scroll view uses Flutter primary-controller inheritance rule'
Assert-True ($scrollView.Contains('PrimaryScrollController.maybeOf(context)', [StringComparison]::Ordinal)) 'primary scroll view obtains the inherited controller'
$scrollbar = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Framework.Widgets/scrollbar.cs')
Assert-True ($scrollbar.Contains('((RawScrollbar)(object)this.widget).controller ?? (ScrollController)PrimaryScrollController.maybeOf(this.context)', [StringComparison]::Ordinal)) 'scrollbar resolves the same explicit-or-primary controller contract'

$trace = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Ui/ScrollLifecycle.cs')
foreach ($phase in @('nativeInput', 'pointerData', 'hitTest', 'gesture', 'activity', 'viewport', 'layout', 'paint', 'retainedLayer', 'raster', 'present', 'scrollbar', 'semantics')) {
    Assert-True ($trace.Contains($phase, [StringComparison]::Ordinal)) "scroll trace phase: $phase"
}
Assert-True ($trace.Contains('Consumers must supply the', [StringComparison]::Ordinal)) 'scroll trace prevents accidental cross-input attribution'

Invoke-Contract 'Debug'
Invoke-Contract 'Release'
$evidence = [ordered]@{
    schemaVersion = 'doroti.flutter-conformance-fcr5-evidence/v1'
    status = 'partial'
    capturedAt = [DateTime]::UtcNow.ToString('o')
    repositoryRevision = (& git -C $repositoryRoot rev-parse HEAD).Trim()
    flutterRevision = $flutterRevision
    fixtureManifest = 'Doroti/validation/fcr5-scroll/fixture-manifest.json'
    runtimeContract = [ordered]@{
        status = 'pass'; debug = 'pass'; release = 'pass'
        checks = @('ScrollController.animateTo waits for the initial attached-position snapshot', 'DrivenScrollActivity initializes and observes its animation', 'scroll trace preserves one input sequence through its declared causal phases', 'trace capacity is bounded without sequence reuse')
    }
    ownershipContract = [ordered]@{
        status = 'pass'
        checks = @('ScrollView applies PrimaryScrollController.shouldInherit', 'RawScrollbar uses explicit controller or the inherited primary controller')
    }
    acceptance = [ordered]@{
        status = 'notVerified'
        reason = 'This structural contract does not execute Flutter-reference differential, a real native pointer-to-present capture, lazy-child/cache measurement, Android physical 60-second scroll, or Windows live wheel/drag.'
        notRun = @('Flutter drag/hold/ballistic/driven differential', 'lazy child create/dispose and keepAlive/cacheExtent measurement', 'text/shader/paint cache hit-miss-eviction measurement', 'Android physical 60-second alternating drag/ballistic scroll', 'Windows native wheel/drag presentation and process-survival acceptance')
    }
}
[IO.Directory]::CreateDirectory((Split-Path $evidencePath -Parent)) | Out-Null
[IO.File]::WriteAllText($evidencePath, (($evidence | ConvertTo-Json -Depth 24) -replace "`r`n", "`n") + "`n", [Text.UTF8Encoding]::new($false))
Write-Output 'Doroti FCR-5 scroll validation: PASS (runtime Debug/Release and ownership contracts; reference/live/physical acceptance remains notVerified)'
