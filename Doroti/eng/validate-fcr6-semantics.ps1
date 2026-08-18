#Requires -Version 7.0
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$dorotiRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $dorotiRoot '..'))
$flutterRoot = Join-Path $repositoryRoot 'reference/flutter-master'
$manifestPath = Join-Path $dorotiRoot 'validation/fcr6-semantics/fixture-manifest.json'
$contractProject = Join-Path $dorotiRoot 'validation/fcr6-semantics/Doroti.Validation.Fcr6Semantics.csproj'
$evidencePath = Join-Path $dorotiRoot 'validation/evidence/flutter-conformance/fcr6-semantics-evidence.json'

function Assert-True([bool] $Condition, [string] $Name) { if (-not $Condition) { throw "$Name failed." } }
function Read-Text([string] $Path) { Assert-True (Test-Path -LiteralPath $Path -PathType Leaf) "source exists: $Path"; Get-Content -Raw -LiteralPath $Path }
function Invoke-Contract([string] $Configuration) {
    $stdout = Join-Path ([IO.Path]::GetTempPath()) ("doroti-fcr6-$Configuration-$([guid]::NewGuid()).log")
    try {
        $process = Start-Process dotnet -ArgumentList @('run', '--project', $contractProject, '-c', $Configuration, '--nologo') -NoNewWindow -PassThru -RedirectStandardOutput $stdout -RedirectStandardError "$stdout.err"
        Assert-True ($process.WaitForExit(1200000)) "FCR-6 runtime contract timeout ($Configuration)"
        $output = ((Get-Content -Raw -LiteralPath $stdout) + (Get-Content -Raw -LiteralPath "$stdout.err"))
        Assert-True ($process.ExitCode -eq 0) "FCR-6 runtime contract exit ($Configuration): $output"
        Assert-True ($output.Contains("FCR-6 semantics runtime contract: PASS (configuration=$Configuration", [StringComparison]::Ordinal)) "FCR-6 runtime contract result ($Configuration)"
    }
    finally { Remove-Item -LiteralPath $stdout, "$stdout.err" -Force -ErrorAction SilentlyContinue }
}

Assert-True (Test-Path -LiteralPath $flutterRoot -PathType Container) 'pinned Flutter checkout exists'
$manifest = Get-Content -Raw -LiteralPath $manifestPath | ConvertFrom-Json
Assert-True ([string]$manifest.schemaVersion -eq 'doroti.flutter-conformance-fcr6-fixture/v1') 'FCR-6 fixture schema'
$flutterRevision = (& git -C $flutterRoot rev-parse HEAD).Trim()
Assert-True ($flutterRevision -eq [string]$manifest.flutterRevision) 'Flutter revision pin'
foreach ($source in @($manifest.sources)) {
    $path = Join-Path $flutterRoot ([string]$source.path).Replace('/', '\')
    $text = Read-Text $path
    Assert-True ((Get-FileHash -Algorithm SHA256 -LiteralPath $path).Hash.ToLowerInvariant() -eq [string]$source.sha256) "Flutter source hash: $($source.path)"
    foreach ($anchor in @($source.anchors)) { Assert-True ($text.Contains([string]$anchor, [StringComparison]::Ordinal)) "Flutter source anchor: $($source.path) -> $anchor" }
}

$contracts = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Ui/GraphicsAndSemanticsContracts.cs')
foreach ($anchor in @('SemanticsUpdateUrgency', 'SemanticsUpdateDiffer', 'SemanticsNodeProperty', 'ContentHash')) { Assert-True ($contracts.Contains($anchor, [StringComparison]::Ordinal)) "FCR-6 shared delta contract: $anchor" }
$bridge = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Host.Maui/MauiSemanticsBridge.cs')
foreach ($anchor in @('MinimumApplyInterval', 'SemanticsUpdateUrgency.scrollEnd', 'CancellationTokenSource', 'scheduleGeneration', 'InputTransparent = true', 'nativePropertyWrites', 'SynchronizeChildOrder', 'RecycleState', 'semanticsApplyEnd')) { Assert-True ($bridge.Contains($anchor, [StringComparison]::Ordinal)) "FCR-6 MAUI bridge contract: $anchor" }
$widgetsBinding = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Framework.Widgets/binding.cs')
foreach ($anchor in @('MinimumActiveScrollSemanticsInterval', 'HasActiveScrollActivity', 'hasPendingSemanticsUpdate', 'semanticsDeferred')) { Assert-True ($widgetsBinding.Contains($anchor, [StringComparison]::Ordinal)) "FCR-6 framework coalescing contract: $anchor" }

Invoke-Contract 'Debug'
Invoke-Contract 'Release'
$evidence = [ordered]@{
    schemaVersion = 'doroti.flutter-conformance-fcr6-evidence/v1'; status = 'partial'; capturedAt = [DateTime]::UtcNow.ToString('o')
    repositoryRevision = (& git -C $repositoryRoot rev-parse HEAD).Trim(); flutterRevision = $flutterRevision
    fixtureManifest = 'Doroti/validation/fcr6-semantics/fixture-manifest.json'
    runtimeContract = [ordered]@{ status = 'pass'; debug = 'pass'; release = 'pass'; checks = @('node content hash excludes geometry', 'geometry-only delta is coalescible', 'existing label and selection changes are immediate', 'virtualized topology churn is bounded', 'scroll-end urgency is explicit', 'non-focusable semantics omit focus actions while focusable Windows actions invoke safely') }
    hostContract = [ordered]@{ status = 'pass'; checks = @('15 fps framework semantics build interval is scoped to active scroll and flushes after scroll end', '15 fps minimum native apply interval includes automatic topology churn', 'generation and cancellation fences suppress stale callbacks', 'changed native properties only are written', 'native elements are pooled and child order changes incrementally', 'semantics build and native apply are frame-traced', 'overlay is input-transparent to ordinary touch') }
    acceptance = [ordered]@{ status = 'notVerified'; reason = 'No Android TalkBack or Windows UIA physical run was performed, and no device baseline has established the 10-percent UI-thread threshold.'; notRun = @('Android TalkBack focus/action/scroll physical checklist', 'Windows UIA focus/action/pass-through physical checklist', 'native semantics UI-thread baseline and 10-percent threshold', 'scroll-end dispatch observation on physical hardware') }
}
[IO.Directory]::CreateDirectory((Split-Path $evidencePath -Parent)) | Out-Null
[IO.File]::WriteAllText($evidencePath, (($evidence | ConvertTo-Json -Depth 24) -replace "`r`n", "`n") + "`n", [Text.UTF8Encoding]::new($false))
Write-Output 'Doroti FCR-6 semantics validation: PASS (Debug/Release contracts; physical and timing acceptance remains notVerified)'
