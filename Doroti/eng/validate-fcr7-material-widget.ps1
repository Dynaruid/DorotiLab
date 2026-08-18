#Requires -Version 7.0
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$dorotiRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $dorotiRoot '..'))
$flutterRoot = Join-Path $repositoryRoot 'reference/flutter-master'
$fixturePath = Join-Path $dorotiRoot 'validation/fcr7-material-widget/fixture-manifest.json'
$contractProject = Join-Path $dorotiRoot 'validation/fcr7-material-widget/Doroti.Validation.Fcr7MaterialWidget.csproj'
$evidencePath = Join-Path $dorotiRoot 'validation/evidence/flutter-conformance/fcr7-material-widget-evidence.json'
$matrixPath = Join-Path $dorotiRoot 'validation/evidence/flutter-conformance/framework-parity-matrix.json'
$appPath = Join-Path $repositoryRoot 'DorotiDemoApp/src/App.cs'

function Assert-True([bool] $Condition, [string] $Name) { if (-not $Condition) { throw "$Name failed." } }
function Read-Text([string] $Path) { Assert-True (Test-Path -LiteralPath $Path -PathType Leaf) "source exists: $Path"; Get-Content -Raw -LiteralPath $Path }
function Invoke-Contract([string] $Configuration) {
    $stdout = Join-Path ([IO.Path]::GetTempPath()) ("doroti-fcr7-$Configuration-$([guid]::NewGuid()).log")
    try {
        $process = Start-Process dotnet -ArgumentList @('run', '--project', $contractProject, '-c', $Configuration, '--nologo') -NoNewWindow -PassThru -RedirectStandardOutput $stdout -RedirectStandardError "$stdout.err"
        Assert-True ($process.WaitForExit(1200000)) "FCR-7 runtime contract timeout ($Configuration)"
        $output = ((Get-Content -Raw -LiteralPath $stdout) + (Get-Content -Raw -LiteralPath "$stdout.err"))
        Assert-True ($process.ExitCode -eq 0) "FCR-7 runtime contract exit ($Configuration): $output"
        Assert-True ($output.Contains("FCR-7 material/widget runtime contract: PASS (configuration=$Configuration", [StringComparison]::Ordinal)) "FCR-7 runtime contract result ($Configuration)"
    }
    finally { Remove-Item -LiteralPath $stdout, "$stdout.err" -Force -ErrorAction SilentlyContinue }
}

Assert-True (Test-Path -LiteralPath $flutterRoot -PathType Container) 'pinned Flutter checkout exists'
$fixture = Get-Content -Raw -LiteralPath $fixturePath | ConvertFrom-Json
Assert-True ([string]$fixture.schemaVersion -eq 'doroti.flutter-conformance-fcr7-fixture/v1') 'FCR-7 fixture schema'
$flutterRevision = (& git -C $flutterRoot rev-parse HEAD).Trim()
Assert-True ($flutterRevision -eq [string]$fixture.flutterRevision) 'Flutter revision pin'
foreach ($source in @($fixture.sources)) {
    $path = Join-Path $flutterRoot ([string]$source.path).Replace('/', '\')
    $text = Read-Text $path
    Assert-True ((Get-FileHash -Algorithm SHA256 -LiteralPath $path).Hash.ToLowerInvariant() -eq [string]$source.sha256) "Flutter source hash: $($source.path)"
    foreach ($anchor in @($source.anchors)) { Assert-True ($text.Contains([string]$anchor, [StringComparison]::Ordinal)) "Flutter source anchor: $($source.path) -> $anchor" }
}

Assert-True ($fixture.captureEnvironment.logicalViewport.width -eq 720 -and $fixture.captureEnvironment.logicalViewport.height -eq 640) 'fixed logical viewport'
Assert-True ($fixture.captureEnvironment.devicePixelRatio -eq 1.0) 'fixed DPR'
Assert-True (@($fixture.backgroundOwnership).Count -eq 2) 'transparent and opaque background fixtures'
Assert-True (@($fixture.components).Count -eq 6) 'fixed representative component slice'
foreach ($scenario in @($fixture.replayScenarios)) {
    Assert-True (@($scenario.states).Count -ge 2) "multi-state scenario: $($scenario.id)"
    Assert-True (@($scenario.sequence).Count -gt 0) "replay sequence: $($scenario.id)"
}
$matrix = Get-Content -Raw -LiteralPath $matrixPath | ConvertFrom-Json
foreach ($sourceId in @('material.scaffold', 'material.app-bar', 'material.floating-action-button', 'material.ink-well', 'material.ink-sparkle', 'widgets.scrollbar', 'widgets.scroll-view', 'widgets.sliver')) {
    $slice = @($matrix.sourceSlices | Where-Object { $_.id -eq $sourceId })
    Assert-True ($slice.Count -eq 1) "parity matrix source slice: $sourceId"
    Assert-True (@($slice[0].productSources).Count -gt 0 -and @($slice[0].runtimeDependencyIds).Count -gt 0 -and @($slice[0].hostConsumers).Count -gt 0 -and @($slice[0].testEvidence).Count -gt 0) "parity matrix closure: $sourceId"
}

$app = Read-Text $appPath
foreach ($component in @($fixture.components)) { Assert-True ($app.Contains([string]$component.productAnchor, [StringComparison]::Ordinal)) "Demo product anchor: $($component.id)" }
foreach ($anchor in @('new Text(', 'ListView.CreateBuilder(', 'new ImageFiltered(', 'ActionSemantics(', 'floatingActionButton:', 'thumbVisibility: true')) { Assert-True ($app.Contains($anchor, [StringComparison]::Ordinal)) "Demo visual/interaction anchor: $anchor" }
$inkSparkle = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Framework.Material/ink_sparkle.cs')
Assert-True ($inkSparkle.Contains('splashFactory', [StringComparison]::Ordinal)) 'InkSparkle product factory anchor'

Invoke-Contract 'Debug'
Invoke-Contract 'Release'
$evidence = [ordered]@{
    schemaVersion = 'doroti.flutter-conformance-fcr7-evidence/v1'; status = 'partial'; capturedAt = [DateTime]::UtcNow.ToString('o')
    repositoryRevision = (& git -C $repositoryRoot rev-parse HEAD).Trim(); flutterRevision = $flutterRevision
    fixtureManifest = 'Doroti/validation/fcr7-material-widget/fixture-manifest.json'
    structuralContract = [ordered]@{ status = 'pass'; debug = 'pass'; release = 'pass'; checks = @('pinned Flutter material/widget source hashes and anchors', 'fixed viewport/DPR/font/theme/locale/time-seed fixture', 'transparent shell versus opaque Scaffold ownership', 'coordinate, hover, scroll, frame, and semantics replay coverage', 'Demo source slice anchors') }
    differential = [ordered]@{ status = 'notVerified'; reason = 'No paired Flutter and Doroti raster/state/semantics captures were executed by this structural gate.'; required = @($fixture.comparison.requires) }
    targets = [ordered]@{
        windowsLive = [ordered]@{ status = 'notVerified'; reason = 'No Windows live paired capture was run.' }
        androidPhysical = [ordered]@{ status = 'notVerified'; reason = 'No Android physical paired capture was run.' }
        webBrowserLive = [ordered]@{ status = 'notVerified' }
        macCatalystNative = [ordered]@{ status = 'notVerified' }
    }
    notRun = @('Flutter reference capture', 'Doroti capture', 'per-channel pixel-diff measurement and cause classification', 'FAB/Ink/Scrollbar state trace comparison', 'Windows live paired capture', 'Android physical paired capture')
}
[IO.Directory]::CreateDirectory((Split-Path $evidencePath -Parent)) | Out-Null
[IO.File]::WriteAllText($evidencePath, (($evidence | ConvertTo-Json -Depth 24) -replace "`r`n", "`n") + "`n", [Text.UTF8Encoding]::new($false))
Write-Output 'Doroti FCR-7 Material/widget validation: PASS (structural fixture and Debug/Release contract; paired captures and target acceptance remain notVerified)'
