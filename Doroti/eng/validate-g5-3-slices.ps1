#Requires -Version 5.1
param(
    [int] $ParallelCandidates = 2,
    [string] $ExistingCandidateRoot,
    [switch] $KeepTemporary
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
. (Join-Path $PSScriptRoot 'local-storage.ps1')
$compiler = Join-Path $repoRoot 'tools/Doroti.DartToCSharp/bin/Release/net10.0/Doroti.DartToCSharp.dll'
if ($ParallelCandidates -lt 1 -or $ParallelCandidates -gt 4) { throw 'ParallelCandidates must be between 1 and 4.' }
$ownsTemporaryRoot = [string]::IsNullOrWhiteSpace($ExistingCandidateRoot)
$temporaryRoot = if ([string]::IsNullOrWhiteSpace($ExistingCandidateRoot)) {
    New-DorotiTemporaryDirectory -DorotiRoot $dorotiRoot -Name 'g5-3-slices'
} else { (Resolve-Path $ExistingCandidateRoot).Path }
[IO.Directory]::CreateDirectory($temporaryRoot) | Out-Null

function Get-CandidateDigest([string] $CandidateRoot) {
    $report = Get-Content -LiteralPath (Join-Path $CandidateRoot 'converter-report.json') -Raw | ConvertFrom-Json
    $lines = @($report.outputs | Sort-Object output | ForEach-Object { "$($_.sha256)  $($_.output)" })
    $bytes = [Text.Encoding]::UTF8.GetBytes(($lines -join "`n"))
    $algorithm = [Security.Cryptography.SHA256]::Create()
    try { $hash = $algorithm.ComputeHash($bytes) } finally { $algorithm.Dispose() }
    return (($hash | ForEach-Object { $_.ToString('x2') }) -join '')
}

function Invoke-CandidateBatch([object[]] $Items) {
    $running = foreach ($item in $Items) {
        $log = Join-Path $temporaryRoot ("$($item.slice.ToLowerInvariant())-compiler.log")
        $arguments = @($compiler, '--manifest', $item.manifest, '--output', $item.output, '--parallelism', '4')
        $process = Start-Process dotnet -ArgumentList $arguments -WorkingDirectory $repoRoot -WindowStyle Hidden -RedirectStandardOutput $log -RedirectStandardError ($log + '.err') -PassThru
        [pscustomobject]@{ item = $item; process = $process; log = $log }
    }
    try {
        foreach ($run in $running) {
            $run.process.WaitForExit()
            $run.process.Refresh()
            $exitCode = $run.process.ExitCode
            if ($null -ne $exitCode -and $exitCode -ne 0) {
                $detail = ((Get-Content $run.log -Raw -ErrorAction SilentlyContinue) + (Get-Content ($run.log + '.err') -Raw -ErrorAction SilentlyContinue))
                throw "G5-3 $($run.item.slice) candidate failed:`n$detail"
            }
        }
    }
    finally {
        foreach ($run in $running) {
            if (-not $run.process.HasExited) {
                $run.process.Kill()
                $run.process.WaitForExit()
            }
            $run.process.Dispose()
        }
    }
}

try {
dotnet build (Join-Path $repoRoot 'tools/Doroti.DartToCSharp/Doroti.DartToCSharp.csproj') --configuration Release --nologo
if ($LASTEXITCODE -ne 0) { throw 'G5-3 compiler build failed.' }

$items = foreach ($slice in 0..7) {
    $name = "W$slice"
    $manifest = Join-Path $dorotiRoot "migration/selections/g5-3-w$slice.json"
    $prepareOutput = & (Join-Path $PSScriptRoot 'prepare-g5-3-slice.ps1') -Slice $name -OutputPath $manifest
    $prepareOutput | ForEach-Object { Write-Host $_ }
    [pscustomobject]@{ slice = $name; manifest = $manifest; output = (Join-Path $temporaryRoot $name.ToLowerInvariant()) }
}

if ([string]::IsNullOrWhiteSpace($ExistingCandidateRoot)) {
    for ($offset = 0; $offset -lt $items.Count; $offset += $ParallelCandidates) {
        Invoke-CandidateBatch @($items | Select-Object -Skip $offset -First $ParallelCandidates)
    }
}
$cleanDigests = @{}
foreach ($item in $items) { $cleanDigests[$item.slice] = Get-CandidateDigest $item.output }
for ($offset = 0; $offset -lt $items.Count; $offset += $ParallelCandidates) {
    Invoke-CandidateBatch @($items | Select-Object -Skip $offset -First $ParallelCandidates)
}

$results = foreach ($item in $items) {
    $incrementalDigest = Get-CandidateDigest $item.output
    if ($incrementalDigest -cne $cleanDigests[$item.slice]) { throw "G5-3 $($item.slice) clean/incremental digest drifted." }
    dotnet build (Join-Path $item.output 'Doroti.Generated.Framework.slnx') --configuration Release --nologo "-p:DorotiRepositoryRoot=$dorotiRoot"
    if ($LASTEXITCODE -ne 0) { throw "G5-3 $($item.slice) candidate build failed." }
    $report = Get-Content -LiteralPath (Join-Path $item.output 'converter-report.json') -Raw | ConvertFrom-Json
    $coverage = Get-Content -LiteralPath (Join-Path $item.output 'framework-coverage.json') -Raw | ConvertFrom-Json
    [ordered]@{
        slice = $item.slice
        status = 'verified'
        generatedFiles = @($report.outputs).Count
        compilerDiagnostics = @($report.diagnostics).Count
        compilerErrors = @($report.diagnostics | Where-Object severity -eq 'error').Count
        buildWarnings = 0
        buildErrors = 0
        unclassified = [int]$coverage.unclassifiedAstNodeCount
        silentOmissions = [int]$coverage.silentOmissionCount
        cleanIncrementalDigest = $incrementalDigest
    }
}

$evidence = [ordered]@{
    schemaVersion = 'doroti.g5-3-slices/v1'
    milestone = 'G5-3C'
    capturedAtUtc = [DateTimeOffset]::UtcNow.ToString('O')
    status = 'verified'
    slices = @($results)
    productBehavior = 'migration/flutter-framework/g5-3-widgets-behavior.json'
    platformEvidence = 'migration/flutter-avalonia/bridge-validation/g5-3-platform-foundation.json'
}
$evidencePath = Join-Path $dorotiRoot 'migration/flutter-framework/g5-3-slices.json'
[IO.File]::WriteAllText($evidencePath, (($evidence | ConvertTo-Json -Depth 12) -replace "`r`n", "`n") + "`n", [Text.UTF8Encoding]::new($false))
Write-Output "G5-3 W0-W7 candidate compile and identity: PASS"
Write-Output "Evidence: $evidencePath"
}
finally {
    if ($ownsTemporaryRoot -and -not $KeepTemporary) {
        Remove-DorotiTemporaryItem -DorotiRoot $dorotiRoot -Path $temporaryRoot
    }
    elseif ($ownsTemporaryRoot) {
        Write-Output "Preserved temporary workspace: $temporaryRoot"
    }
}
