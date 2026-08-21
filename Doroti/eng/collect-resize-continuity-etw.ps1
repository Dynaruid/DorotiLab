#Requires -Version 7.0
param(
    [ValidateSet('default', '0', '1')]
    [string] $SwapInterval = '0',

    [ValidateRange(1, 60)]
    [int] $DurationSeconds = 10
)

$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $false
$scriptPath = $PSCommandPath
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
$evidenceRoot = Join-Path $dorotiRoot 'validation/evidence/resize'
[IO.Directory]::CreateDirectory($evidenceRoot) | Out-Null

function Test-IsAdministrator {
    $identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = [Security.Principal.WindowsPrincipal]::new($identity)
    return $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

if (-not (Test-IsAdministrator)) {
    $escapedScript = $scriptPath.Replace("'", "''")
    $elevatedCommand = "& '$escapedScript' -SwapInterval '$SwapInterval' -DurationSeconds $DurationSeconds"
    $encodedCommand = [Convert]::ToBase64String(
        [Text.Encoding]::Unicode.GetBytes($elevatedCommand))
    $process = Start-Process -FilePath (Get-Command pwsh.exe).Source `
        -Verb RunAs `
        -WorkingDirectory $repoRoot `
        -ArgumentList @('-NoProfile', '-NoExit', '-EncodedCommand', $encodedCommand) `
        -PassThru
    Write-Output "UAC requested. Elevated process id: $($process.Id)"
    return
}

Set-Location -LiteralPath $repoRoot
$stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
$logPath = Join-Path $evidenceRoot "rsz0b-$SwapInterval-gpu-$stamp.console.log"
$etlPath = Join-Path $evidenceRoot "rsz0b-$SwapInterval-gpu-$stamp.etl"
$gpuEtlPath = Join-Path $evidenceRoot "rsz0b-$SwapInterval-gpu-$stamp.gpu-only.etl"
$markerEtlPath = Join-Path $evidenceRoot "rsz0b-$SwapInterval-gpu-$stamp.resize-markers.etl"
$resultPath = Join-Path $evidenceRoot "rsz0b-$SwapInterval-gpu-$stamp.result.json"
$markerProviderGuid = '{5a846f8d-54a1-4a4c-9e56-5b5a84e3b3c1}'
$markerSessionName = "DorotiResize-$PID-$stamp"
$recoveredEtlPath = $null
$wprStarted = $false
$markerStarted = $false
$completed = $false
$failure = $null

function Invoke-Wpr([string[]] $Arguments) {
    $output = @(& wpr.exe @Arguments 2>&1 | ForEach-Object { $_.ToString() })
    $exitCode = $LASTEXITCODE
    $output | ForEach-Object { Write-Host $_ }
    return [pscustomobject]@{
        ExitCode = $exitCode
        Text = $output -join "`n"
    }
}

function Invoke-Native([string] $FilePath, [string[]] $Arguments) {
    $output = @(& $FilePath @Arguments 2>&1 | ForEach-Object { $_.ToString() })
    $exitCode = $LASTEXITCODE
    $output | ForEach-Object { Write-Host $_ }
    return [pscustomobject]@{
        ExitCode = $exitCode
        Text = $output -join "`n"
    }
}

Start-Transcript -LiteralPath $logPath -Force | Out-Null
try {
    Write-Host "Doroti RSZ-0B GPU ETW collection" -ForegroundColor Cyan
    Write-Host "Repository: $repoRoot"
    Write-Host "Swap interval: $SwapInterval"
    Write-Host "Duration: $DurationSeconds seconds"

    $start = Invoke-Wpr @('-start', 'GPU', '-filemode')
    if ($start.ExitCode -ne 0 -and
        ($start.Text -match 'already running' -or $start.Text -match '0xc5583001')) {
        $recoveredEtlPath = Join-Path $evidenceRoot "wpr-existing-recovered-$stamp.etl"
        Write-Host "An existing WPR session was found. Preserving it at:" -ForegroundColor Yellow
        Write-Host $recoveredEtlPath -ForegroundColor Yellow
        $stopExisting = Invoke-Wpr @('-stop', $recoveredEtlPath)
        if ($stopExisting.ExitCode -ne 0) {
            throw "Existing WPR session could not be preserved (exit $($stopExisting.ExitCode)). No cancel was attempted."
        }
        $start = Invoke-Wpr @('-start', 'GPU', '-filemode')
    }
    if ($start.ExitCode -ne 0) {
        throw "GPU ETW start failed with exit code $($start.ExitCode)."
    }
    $wprStarted = $true

    Write-Host "Starting Doroti resize marker ETW provider: $markerProviderGuid" -ForegroundColor Cyan
    $markerStart = Invoke-Native 'logman.exe' @(
        'start', $markerSessionName,
        '-ets',
        '-p', $markerProviderGuid, '0xffffffffffffffff', '5',
        '-o', $markerEtlPath)
    if ($markerStart.ExitCode -ne 0) {
        throw "Doroti resize marker ETW start failed with exit code $($markerStart.ExitCode)."
    }
    $markerStarted = $true

    & pwsh -NoProfile -File ./Doroti/eng/validate-resize-continuity-live.ps1 `
        -SwapInterval $SwapInterval `
        -DurationSeconds $DurationSeconds `
        -RetainRawTrace
    if ($LASTEXITCODE -ne 0) {
        throw "Live resize validation failed with exit code $LASTEXITCODE."
    }
    $completed = $true
}
catch {
    $failure = $_.Exception.ToString()
    Write-Host "`nCollection failed:" -ForegroundColor Red
    Write-Host $failure -ForegroundColor Red
}
finally {
    if ($markerStarted) {
        Write-Host "`nStopping Doroti resize marker ETW." -ForegroundColor Cyan
        $markerStop = Invoke-Native 'logman.exe' @('stop', $markerSessionName, '-ets')
        if ($markerStop.ExitCode -ne 0) {
            $completed = $false
            $markerFailure = "Doroti resize marker ETW stop failed with exit code $($markerStop.ExitCode)."
            $failure = if ($failure) { "$failure`n$markerFailure" } else { $markerFailure }
        }
        $markerStarted = $false
    }

    if ($wprStarted) {
        if ($completed -and [string]::IsNullOrWhiteSpace($failure)) {
            Write-Host "`nStopping GPU ETW." -ForegroundColor Cyan
            $stop = Invoke-Wpr @('-stop', $gpuEtlPath)
            if ($stop.ExitCode -ne 0) {
                $completed = $false
                $stopFailure = "GPU ETW stop failed with exit code $($stop.ExitCode)."
                $failure = if ($failure) { "$failure`n$stopFailure" } else { $stopFailure }
            }
        } else {
            Write-Host "`nCancelling GPU ETW because live validation failed." -ForegroundColor Yellow
            $cancel = Invoke-Wpr @('-cancel')
            if ($cancel.ExitCode -ne 0) {
                $cancelFailure = "GPU ETW cancel failed with exit code $($cancel.ExitCode)."
                $failure = if ($failure) { "$failure`n$cancelFailure" } else { $cancelFailure }
            }
        }
    }

    if ($completed -and [string]::IsNullOrWhiteSpace($failure)) {
        if (-not (Test-Path -LiteralPath $gpuEtlPath) -or
            -not (Test-Path -LiteralPath $markerEtlPath)) {
            $completed = $false
            $failure = 'GPU or Doroti marker ETL was not written; the merged trace cannot be produced.'
        } else {
            Write-Host "`nMerging GPU and ResizeEpoch markers into:" -ForegroundColor Cyan
            Write-Host $etlPath -ForegroundColor Cyan
            $merge = Invoke-Native (Get-Command xperf.exe).Source @(
                '-merge', $gpuEtlPath, $markerEtlPath, $etlPath)
            if ($merge.ExitCode -ne 0 -or -not (Test-Path -LiteralPath $etlPath)) {
                $completed = $false
                $failure = "ETW merge failed with exit code $($merge.ExitCode)."
            } else {
                $validatedEvidenceRoot = [IO.Path]::GetFullPath($evidenceRoot) + [IO.Path]::DirectorySeparatorChar
                foreach ($intermediatePath in @($gpuEtlPath, $markerEtlPath)) {
                    $resolvedIntermediate = [IO.Path]::GetFullPath($intermediatePath)
                    if (-not $resolvedIntermediate.StartsWith(
                            $validatedEvidenceRoot,
                            [StringComparison]::OrdinalIgnoreCase)) {
                        throw "Refusing to remove ETW intermediate outside evidence root: $resolvedIntermediate"
                    }
                    Remove-Item -LiteralPath $resolvedIntermediate -Force
                }
            }
        }
    }

    $result = [ordered]@{
        schemaVersion = 'doroti.resize-continuity-etw/v2'
        capturedAt = [DateTimeOffset]::Now.ToString('o')
        completed = $completed -and [string]::IsNullOrWhiteSpace($failure)
        swapInterval = $SwapInterval
        durationSeconds = $DurationSeconds
        markerProviderGuid = $markerProviderGuid
        etlPath = if (Test-Path -LiteralPath $etlPath) {
            [IO.Path]::GetRelativePath($repoRoot, $etlPath).Replace('\', '/')
        } else { $null }
        gpuIntermediatePath = if (Test-Path -LiteralPath $gpuEtlPath) {
            [IO.Path]::GetRelativePath($repoRoot, $gpuEtlPath).Replace('\', '/')
        } else { $null }
        markerIntermediatePath = if (Test-Path -LiteralPath $markerEtlPath) {
            [IO.Path]::GetRelativePath($repoRoot, $markerEtlPath).Replace('\', '/')
        } else { $null }
        recoveredExistingEtlPath = if ($recoveredEtlPath -and (Test-Path -LiteralPath $recoveredEtlPath)) {
            [IO.Path]::GetRelativePath($repoRoot, $recoveredEtlPath).Replace('\', '/')
        } else { $null }
        consoleLogPath = [IO.Path]::GetRelativePath($repoRoot, $logPath).Replace('\', '/')
        failure = $failure
    }
    [IO.File]::WriteAllText(
        $resultPath,
        (($result | ConvertTo-Json -Depth 8) -replace "`r`n", "`n") + "`n",
        [Text.UTF8Encoding]::new($false))
    Stop-Transcript | Out-Null
}

Write-Host "`nResult: $resultPath" -ForegroundColor Cyan
if ($completed -and [string]::IsNullOrWhiteSpace($failure)) {
    Write-Host 'RSZ-0B GPU ETW collection: PASS' -ForegroundColor Green
} else {
    Write-Host 'RSZ-0B GPU ETW collection: FAILED' -ForegroundColor Red
}
Write-Host 'This elevated window will remain open for review.'
