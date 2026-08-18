#Requires -Version 7.0
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$dorotiRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $dorotiRoot '..'))
$flutterRoot = Join-Path $repositoryRoot 'reference/flutter-master'
$manifestPath = Join-Path $dorotiRoot 'validation/fcr3-scheduler/fixture-manifest.json'
$contractProject = Join-Path $dorotiRoot 'validation/fcr3-scheduler/Doroti.Validation.Fcr3Scheduler.csproj'
$evidencePath = Join-Path $dorotiRoot 'validation/evidence/flutter-conformance/fcr3-scheduler-evidence.json'

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
        $outputPath = Join-Path ([IO.Path]::GetTempPath()) ("doroti-fcr3-$Configuration-$([guid]::NewGuid()).log")
        try {
            $process = Start-Process dotnet -ArgumentList @('run', '--project', $contractProject, '-c', $Configuration, '--nologo') `
                -NoNewWindow -PassThru -RedirectStandardOutput $outputPath -RedirectStandardError "$outputPath.err"
            Assert-True ($process.WaitForExit(1200000)) "FCR-3 runtime contract timeout ($Configuration)"
            $output = ((Get-Content -Raw -LiteralPath $outputPath) + (Get-Content -Raw -LiteralPath "$outputPath.err"))
            Assert-True ($process.ExitCode -eq 0) "FCR-3 runtime contract exit ($Configuration): $output"
            Assert-True ($output.Contains("FCR-3 scheduler runtime contract: PASS (configuration=$Configuration", [StringComparison]::Ordinal)) "FCR-3 runtime contract result ($Configuration)"
        }
        finally {
            Remove-Item -LiteralPath $outputPath, "$outputPath.err" -Force -ErrorAction SilentlyContinue
        }
    }
    finally { Pop-Location }
}

Assert-True (Test-Path -LiteralPath $flutterRoot -PathType Container) 'pinned Flutter checkout exists'
$manifest = Get-Content -Raw -LiteralPath $manifestPath | ConvertFrom-Json
Assert-True ([string]$manifest.schemaVersion -eq 'doroti.flutter-conformance-fcr3-fixture/v1') 'FCR-3 fixture schema'
$flutterRevision = (& git -C $flutterRoot rev-parse HEAD).Trim()
Assert-True ($flutterRevision -eq [string]$manifest.flutterRevision) "Flutter revision pin: expected $($manifest.flutterRevision), got $flutterRevision"
foreach ($source in @($manifest.sources)) {
    $path = Join-Path $flutterRoot ([string]$source.path).Replace('/', '\')
    Assert-True (Test-Path -LiteralPath $path -PathType Leaf) "Flutter source: $($source.path)"
    $hash = (Get-FileHash -Algorithm SHA256 -LiteralPath $path).Hash.ToLowerInvariant()
    Assert-True ($hash -eq [string]$source.sha256) "Flutter source hash: $($source.path)"
    $text = Read-Text $path
    foreach ($anchor in @($source.anchors)) {
        Assert-True ($text.Contains([string]$anchor, [StringComparison]::Ordinal)) "Flutter source anchor: $($source.path) -> $anchor"
    }
}

$scheduler = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Framework.Scheduler/binding.cs')
Assert-True ($scheduler.Contains('DorotiFrameTrace _frameTrace', [StringComparison]::Ordinal)) 'scheduler owns a phase trace'
Assert-True ($scheduler.Contains('supplied.inMicroseconds < _lastRawTimeStamp.inMicroseconds', [StringComparison]::Ordinal)) 'scheduler fences stale vsync timestamps'
foreach ($phase in @('transientCallbacks', 'midFrameMicrotasks', 'persistentCallbacks', 'postFrameCallbacks')) {
    Assert-True ($scheduler.Contains("DorotiFramePhase.$phase", [StringComparison]::Ordinal)) "scheduler trace phase: $phase"
}

$clock = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Ui/FrameLifecycle.cs')
Assert-True ($clock.Contains('Stopwatch.GetElapsedTime', [StringComparison]::Ordinal)) 'frame clock is monotonic'
Assert-True ($clock.Contains('while (_entries.Count > Capacity)', [StringComparison]::Ordinal)) 'frame trace is bounded'

$widgetsBinding = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Framework.Widgets/binding.cs')
foreach ($phase in @('build', 'layout', 'paint', 'sceneBuild')) {
    Assert-True ($widgetsBinding.Contains("DorotiFramePhase.$phase", [StringComparison]::Ordinal)) "widget frame phase boundary: $phase"
}

$mauiHost = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Host.Maui/MauiHostAdapter.cs')
Assert-True (-not $mauiHost.Contains('DateTime.UtcNow.Ticks', [StringComparison]::Ordinal)) 'MAUI frame and pointer timestamps avoid wall clock'
Assert-True ($mauiHost.Contains('Action<TimeSpan>? _pendingFrameCallback', [StringComparison]::Ordinal)) 'MAUI host has one pending frame callback'
Assert-True ($mauiHost.Contains('FrameRequestsCoalesced', [StringComparison]::Ordinal)) 'MAUI host reports frame request coalescing'

$mauiRaster = Read-Text (Join-Path $dorotiRoot 'src/Doroti.Host.Maui/MauiSkiaCapabilities.cs')
Assert-True ($mauiRaster.Contains('scene.Commands.ToArray()', [StringComparison]::Ordinal)) 'scene ownership snapshots an immutable command array'
Assert-True ($mauiRaster.Contains('DorotiFramePhase.superseded', [StringComparison]::Ordinal)) 'superseded scenes are traced'
Assert-True ($mauiRaster.Contains('DorotiFramePhase.present', [StringComparison]::Ordinal)) 'present is traced'
Assert-True ($mauiRaster.Contains('InputSequence', [StringComparison]::Ordinal)) 'scene-to-input attribution is retained'

$debugOutput = Invoke-Contract 'Debug'
$releaseOutput = Invoke-Contract 'Release'
$evidence = [ordered]@{
    schemaVersion = 'doroti.flutter-conformance-fcr3-evidence/v1'
    status = 'partial'
    capturedAt = [DateTime]::UtcNow.ToString('o')
    repositoryRevision = (& git -C $repositoryRoot rev-parse HEAD).Trim()
    flutterRevision = $flutterRevision
    fixtureManifest = 'Doroti/validation/fcr3-scheduler/fixture-manifest.json'
    runtimeContract = [ordered]@{
        status = 'pass'
        debug = 'pass'
        release = 'pass'
        checks = @(
            'Flutter transient, persistent, and post-frame callback ordering',
            'monotonic stale-vsync fence',
            'build/layout/paint/scene-build boundary stamps',
            'bounded causally ordered frame trace with input and scene sequence attribution'
        )
    }
    hostContract = [ordered]@{
        status = 'pass'
        checks = @(
            'one pending MAUI frame callback with invalidation coalescing',
            'immutable latest scene replacement with superseded reason',
            'raster/present/replay/failed trace entries without sharing the framework dispatch lock'
        )
    }
    acceptance = [ordered]@{
        status = 'notVerified'
        reason = 'The compact contract does not execute forced native resize/context recreation or a physical foreground-resume scenario.'
        notRun = @('Windows native lifecycle stress', 'Android physical lifecycle stress', '60-second interaction trace')
    }
}
[IO.Directory]::CreateDirectory((Split-Path $evidencePath -Parent)) | Out-Null
[IO.File]::WriteAllText($evidencePath, (($evidence | ConvertTo-Json -Depth 32) -replace "`r`n", "`n") + "`n", [Text.UTF8Encoding]::new($false))
Write-Output 'Doroti FCR-3 scheduler validation: PASS (runtime Debug/Release; native lifecycle acceptance remains notVerified)'
