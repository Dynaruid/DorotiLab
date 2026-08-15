#Requires -Version 5.1
param(
    [switch] $Quick,
    [int] $Runs = 3,
    [string] $Output = 'artifacts/validation/dart-to-csharp-performance.json',
    [string[]] $SelectionNames = @(),
    [int[]] $ParallelismValues = @(),
    [switch] $Append
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repositoryRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
. (Join-Path $PSScriptRoot 'local-storage.ps1')
. (Join-Path $PSScriptRoot 'flutter-sdk.ps1')
$dartCommand = (Resolve-DorotiFlutterSdk -RepositoryRoot $repositoryRoot).DartCommand
$compiler = Join-Path $repositoryRoot 'tools/Doroti.DartToCSharp/bin/Release/net10.0/Doroti.DartToCSharp.dll'
$timeoutMilliseconds = 15 * 60 * 1000
$temporaryRoot = New-DorotiTemporaryDirectory -DorotiRoot $dorotiRoot -Name 'dart-to-csharp-performance'
$outputPath = if ([IO.Path]::IsPathRooted($Output)) { $Output } else { Join-Path $dorotiRoot $Output }

if ($Runs -lt 1) { throw 'Runs must be positive.' }

$selections = @(
    [ordered]@{ name = 'focused-6'; libraries = 6; manifest = 'Doroti/validation/cases/g4-5-gradient-static-signature.selection.json' },
    [ordered]@{ name = 'g4-3'; libraries = 58; manifest = 'Doroti/migration/selections/g4-3-scheduler-services.json' },
    [ordered]@{ name = 'g4-4'; libraries = 42; manifest = 'Doroti/migration/selections/g4-4-physics-animation-gestures.json' },
    [ordered]@{ name = 'g4-5'; libraries = 101; manifest = 'Doroti/migration/selections/g4-5-painting-rendering-semantics.json' }
)
$parallelism = @(1, 2, 4, 8, 16)
if ($Quick) {
    $selections = @($selections | Where-Object { $_.name -in @('focused-6', 'g4-5') })
    $parallelism = @(4)
    $Runs = 1
}
elseif ($SelectionNames.Count -gt 0 -or $ParallelismValues.Count -gt 0) {
    if ($SelectionNames.Count -gt 0) { $selections = @($selections | Where-Object { $_.name -in $SelectionNames }) }
    if ($ParallelismValues.Count -gt 0) { $parallelism = @($parallelism | Where-Object { $_ -in $ParallelismValues }) }
    if ($selections.Count -eq 0 -or $parallelism.Count -eq 0) { throw 'The requested performance matrix cell selection is empty.' }
}

function Quote-Argument([string] $Value) {
    if ($Value -notmatch '[\s"]') { return $Value }
    return '"' + ($Value -replace '(\\*)"', '$1$1\"' -replace '(\\+)$', '$1$1') + '"'
}

function Invoke-MeasuredCompiler {
    param(
        [string] $Manifest,
        [string] $GeneratedOutput,
        [string] $Cache,
        [string] $Telemetry,
        [int] $Degree
    )
    $arguments = @(
        (Quote-Argument $compiler),
        '--manifest', (Quote-Argument (Join-Path $repositoryRoot $Manifest)),
        '--output', (Quote-Argument $GeneratedOutput),
        '--cache-dir', (Quote-Argument $Cache),
        '--lowering-parallelism', $Degree,
        '--analyzer-workers', 1,
        '--telemetry', (Quote-Argument $Telemetry)
    ) -join ' '
    $startInfo = [Diagnostics.ProcessStartInfo]::new('dotnet', $arguments)
    $startInfo.WorkingDirectory = $repositoryRoot
    $startInfo.UseShellExecute = $false
    $startInfo.CreateNoWindow = $true
    $startInfo.RedirectStandardOutput = $true
    $startInfo.RedirectStandardError = $true
    $process = [Diagnostics.Process]::new()
    $process.StartInfo = $startInfo
    $started = [DateTimeOffset]::UtcNow
    [void]$process.Start()
    if (-not $process.WaitForExit($timeoutMilliseconds)) {
        Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
        return [ordered]@{
            status = 'timeout'
            elapsedMilliseconds = $timeoutMilliseconds
            lastCompletedPhase = $null
            telemetry = $null
            startedUtc = $started
        }
    }
    $stdout = $process.StandardOutput.ReadToEnd()
    $stderr = $process.StandardError.ReadToEnd()
    $exitCode = $process.ExitCode
    $process.Dispose()
    $document = if (Test-Path -LiteralPath $Telemetry) { Get-Content -Raw -LiteralPath $Telemetry | ConvertFrom-Json } else { $null }
    return [ordered]@{
        status = if ($exitCode -eq 0) { 'success' } else { 'failed' }
        exitCode = $exitCode
        stdout = $stdout.Trim()
        stderr = $stderr.Trim()
        elapsedMilliseconds = $document.elapsedMilliseconds
        lastCompletedPhase = $document.lastCompletedPhase
        telemetry = $document
        startedUtc = $started
    }
}

function Median([long[]] $Values) {
    $ordered = @($Values | Sort-Object)
    if ($ordered.Count -eq 0) { return $null }
    return $ordered[[Math]::Floor($ordered.Count / 2)]
}

try {
    New-Item -ItemType Directory -Path $temporaryRoot | Out-Null
    Push-Location $repositoryRoot
    try {
        dotnet build tools/Doroti.DartToCSharp/Doroti.DartToCSharp.csproj --configuration Release --nologo
        if ($LASTEXITCODE -ne 0) { throw 'Release compiler build failed.' }
    }
    finally { Pop-Location }

    $measurements = @()
    if ($Append -and (Test-Path -LiteralPath $outputPath)) {
        $priorArtifact = Get-Content -LiteralPath $outputPath -Raw | ConvertFrom-Json
        $measurements = @($priorArtifact.currentRun.measurements | Where-Object { $null -ne $_ })
    }
    foreach ($selection in $selections) {
        foreach ($degree in $parallelism) {
            $sharedCache = Join-Path $temporaryRoot ("cache-$($selection.name)-j$degree")
            $warmupOutput = Join-Path $temporaryRoot ("warmup-$($selection.name)-j$degree")
            $warmupTelemetry = Join-Path $temporaryRoot ("warmup-$($selection.name)-j$degree.json")
            $null = Invoke-MeasuredCompiler $selection.manifest $warmupOutput $sharedCache $warmupTelemetry $degree
            foreach ($scenario in @('cold', 'warm')) {
                $runsForScenario = @()
                for ($run = 1; $run -le $Runs; $run++) {
                    $cache = if ($scenario -eq 'cold') {
                        Join-Path $temporaryRoot ("cold-cache-$($selection.name)-j$degree-r$run")
                    } else { $sharedCache }
                    $generated = Join-Path $temporaryRoot ("output-$scenario-$($selection.name)-j$degree-r$run")
                    $telemetry = Join-Path $temporaryRoot ("telemetry-$scenario-$($selection.name)-j$degree-r$run.json")
                    $runsForScenario += Invoke-MeasuredCompiler $selection.manifest $generated $cache $telemetry $degree
                }
                $successful = @($runsForScenario | Where-Object status -eq 'success')
                $measurements = @($measurements | Where-Object {
                    -not ($_.selection -eq $selection.name -and $_.scenario -eq $scenario -and $_.loweringParallelism -eq $degree)
                })
                $measurements += [ordered]@{
                    selection = $selection.name
                    libraries = $selection.libraries
                    scenario = $scenario
                    loweringParallelism = $degree
                    runs = $runsForScenario
                    medianElapsedMilliseconds = Median @($successful | ForEach-Object { [long]$_.telemetry.elapsedMilliseconds })
                    medianPeakWorkingSetBytes = Median @($successful | ForEach-Object { [long]$_.telemetry.peakWorkingSetBytes })
                }
            }
        }
    }

    $g45Warm = @($measurements | Where-Object { $_.selection -eq 'g4-5' -and $_.scenario -eq 'warm' })[0]
    if ($null -eq $g45Warm) { throw 'At least one G4-5 warm cell is required to assess the performance budget.' }
    $g45WarmTelemetry = @($g45Warm.runs | Where-Object status -eq 'success')[0].telemetry
    $g45WarmLowering = @($g45WarmTelemetry.invocationPhases | Where-Object name -eq 'csharp-lowering-printing')[0].elapsedMilliseconds
    $elapsedRegressionLimit = [long][Math]::Ceiling(846689 * 1.15)
    $allRunsCompleted = @($measurements | ForEach-Object { $_.runs } | Where-Object { $_.status -ne 'success' }).Count -eq 0
    $matrixComplete = $measurements.Count -eq 40 -and @($measurements | Where-Object { @($_.runs).Count -ne $Runs }).Count -eq 0
    $elapsedBudgetPassed = $null -ne $g45Warm.medianElapsedMilliseconds -and $g45Warm.medianElapsedMilliseconds -le $elapsedRegressionLimit
    $cacheFootprintPassed = $g45WarmTelemetry.cacheReadBytes -le 389124078
    $artifact = [ordered]@{
        schemaVersion = 'doroti.dart-to-csharp-performance/v2'
        capturedUtc = [DateTimeOffset]::UtcNow
        machine = [ordered]@{
            machineName = $env:COMPUTERNAME
            os = [Environment]::OSVersion.VersionString
            logicalProcessors = [Environment]::ProcessorCount
            dotnet = (& dotnet --version).Trim()
            dart = (& $dartCommand --version 2>&1 | Out-String).Trim()
        }
        policy = [ordered]@{
            warmupRuns = 1
            measuredRuns = $Runs
            timeoutMilliseconds = $timeoutMilliseconds
            scenarios = @('cold', 'warm')
            incremental = 'validated-by-dependency-negative-tests; timed-SCC-copy-not-run'
            priority = 'elapsed-time-primary; memory-secondary-per-user-direction-2026-08-10'
        }
        previousBaseline = [ordered]@{
            selection = 'g4-5'
            scenario = 'warm'
            capturedUtc = '2026-08-10T02:31:32.9793934Z'
            elapsedMilliseconds = 846689
            csharpLoweringPrintingMilliseconds = 831283
            peakWorkingSetBytes = 4171014144
            analyzerCachePayloadBytes = 389124078
            dartProcessCount = 0
            cacheHits = 101
            compilerIdentity = '68d502ce733757b1726645a225f4c396c0ff450d840139c2e91458b40c5cd72e'
        }
        assessment = [ordered]@{
            warmEndToEndSpeedup = [Math]::Round(846689.0 / $g45Warm.medianElapsedMilliseconds, 2)
            csharpLoweringPrintingSpeedup = [Math]::Round(831283.0 / $g45WarmLowering, 2)
            cacheFootprintReductionPercent = [Math]::Round((1.0 - ($g45WarmTelemetry.cacheReadBytes / 389124078.0)) * 100.0, 1)
            peakWorkingSetReductionPercent = [Math]::Round((1.0 - ($g45WarmTelemetry.peakWorkingSetBytes / 4171014144.0)) * 100.0, 1)
            elapsedRegressionLimitMilliseconds = $elapsedRegressionLimit
            elapsedBudgetStatus = if ($elapsedBudgetPassed) { 'passed' } else { 'failed' }
            fullMatrixStatus = if ($matrixComplete -and $allRunsCompleted) { 'passed' } else { 'incomplete' }
            cacheFootprintStatus = if ($cacheFootprintPassed) { 'passed' } else { 'failed' }
            memoryBudgetStatus = 'reported-secondary-not-blocking'
            coldBaselineStatus = 'not-captured-before-optimization'
        }
        currentRun = [ordered]@{
            capturedUtc = [DateTimeOffset]::UtcNow
            measurements = $measurements
        }
    }
    New-Item -ItemType Directory -Force -Path (Split-Path $outputPath) | Out-Null
    $temporaryOutput = $outputPath + '.tmp-' + [Guid]::NewGuid().ToString('N')
    [IO.File]::WriteAllText($temporaryOutput, (($artifact | ConvertTo-Json -Depth 20) -replace "`r`n", "`n") + "`n", [Text.UTF8Encoding]::new($false))
    if (Test-Path -LiteralPath $outputPath) {
        $backupOutput = $outputPath + '.bak-' + [Guid]::NewGuid().ToString('N')
        [IO.File]::Replace($temporaryOutput, $outputPath, $backupOutput, $true)
        Remove-Item -LiteralPath $backupOutput
    }
    else { [IO.File]::Move($temporaryOutput, $outputPath) }
    if (($matrixComplete -and -not $allRunsCompleted) -or -not $elapsedBudgetPassed -or -not $cacheFootprintPassed) {
        throw 'Dart-to-C# performance matrix did not satisfy the G5-0 budget.'
    }
    Write-Output "Dart-to-C# performance measurement: $(if ($matrixComplete) { 'PASS' } else { 'CHECKPOINT' })"
    Write-Output "Artifact: $outputPath"
}
finally {
    if (Test-Path -LiteralPath $temporaryRoot) {
        Remove-DorotiTemporaryItem -DorotiRoot $dorotiRoot -Path $temporaryRoot
    }
}
