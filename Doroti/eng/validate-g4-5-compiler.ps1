#Requires -Version 5.1
param(
    [string] $CacheDirectory,
    [string] $CandidateDirectory
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repositoryRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
. (Join-Path $PSScriptRoot 'local-storage.ps1')
if ([string]::IsNullOrWhiteSpace($CacheDirectory)) {
    $CacheDirectory = Join-Path $repositoryRoot '.doroti/cache/dart-analyzer'
}
elseif (-not [IO.Path]::IsPathRooted($CacheDirectory)) {
    $CacheDirectory = Join-Path $repositoryRoot $CacheDirectory
}
$CacheDirectory = [IO.Path]::GetFullPath($CacheDirectory)

$ownsCandidate = [string]::IsNullOrWhiteSpace($CandidateDirectory)
if ($ownsCandidate) {
    $CandidateDirectory = New-DorotiTemporaryDirectory -DorotiRoot $dorotiRoot -Name 'g4-5-compiler'
}
elseif (-not [IO.Path]::IsPathRooted($CandidateDirectory)) {
    $CandidateDirectory = Join-Path $repositoryRoot $CandidateDirectory
}
$CandidateDirectory = [IO.Path]::GetFullPath($CandidateDirectory)
$started = [DateTime]::UtcNow
$artifactPath = Join-Path $dorotiRoot 'artifacts/validation/g4-5-compiler-gate.json'

function Invoke-Checked {
    param([scriptblock] $Command, [string] $Operation)
    & $Command
    if ($LASTEXITCODE -ne 0) { throw "$Operation failed with exit code $LASTEXITCODE." }
}

function Write-AtomicJson {
    param([string] $Path, [object] $Value)
    $temporary = $Path + '.tmp-' + [Guid]::NewGuid().ToString('N')
    [IO.File]::WriteAllText($temporary, (($Value | ConvertTo-Json -Depth 12) -replace "`r`n", "`n") + "`n", [Text.UTF8Encoding]::new($false))
    if (Test-Path -LiteralPath $Path) {
        $backup = $Path + '.bak-' + [Guid]::NewGuid().ToString('N')
        [IO.File]::Replace($temporary, $Path, $backup, $true)
        Remove-Item -LiteralPath $backup
    }
    else { [IO.File]::Move($temporary, $Path) }
}

try {
    Push-Location $repositoryRoot
    try {
        & "$dorotiRoot/eng/prepare-g4-5.ps1" | Write-Output
        if ($ownsCandidate) {
            New-Item -ItemType Directory -Path $CandidateDirectory | Out-Null
            Invoke-Checked {
                dotnet run --project tools/Doroti.DartToCSharp/Doroti.DartToCSharp.csproj `
                    --configuration Release -- `
                    --manifest Doroti/migration/selections/g4-5-painting-rendering-semantics.json `
                    --output $CandidateDirectory `
                    --cache-dir $CacheDirectory `
                    --parallelism 16
            } 'G4-5 candidate generation'
        }
    }
    finally { Pop-Location }

    foreach ($required in @('converter-report.json', 'framework-coverage.json', 'Doroti.Generated.Framework.slnx')) {
        if (-not (Test-Path -LiteralPath (Join-Path $CandidateDirectory $required))) {
            throw "G4-5 candidate is missing $required."
        }
    }

    $buildOutput = @()
    $aggregateExitCode = 0
    Push-Location $CandidateDirectory
    try {
        $buildOutput = @(& {
            dotnet build Doroti.Generated.Framework.slnx `
                --configuration Release `
                --no-incremental `
                --nologo `
                --verbosity minimal `
                "-p:DorotiRepositoryRoot=$dorotiRoot"
        } 2>&1)
        $aggregateExitCode = $LASTEXITCODE
        $buildOutput | ForEach-Object { Write-Output $_ }
    }
    finally { Pop-Location }

    $report = Get-Content (Join-Path $CandidateDirectory 'converter-report.json') -Raw | ConvertFrom-Json
    $coverage = Get-Content (Join-Path $CandidateDirectory 'framework-coverage.json') -Raw | ConvertFrom-Json
    $aggregateErrors = @($buildOutput | ForEach-Object { [string]$_ } |
        Where-Object { $_ -match '^.+\(\d+,\d+\): error CS\d+:' } | Sort-Object -Unique)
    $aggregateWarnings = @($buildOutput | ForEach-Object { [string]$_ } |
        Where-Object { $_ -match '^.+\(\d+,\d+\): warning CS\d+:' } | Sort-Object -Unique)

    if (-not $report.success) { throw 'G4-5 converter report is not successful.' }
    if ($coverage.declarationCount -ne 663) { throw "G4-5 declaration inventory drifted: $($coverage.declarationCount)." }
    if ($coverage.unclassifiedAstNodeCount -ne 0) { throw 'G4-5 has unclassified analyzer nodes.' }
    if ($coverage.silentOmissionCount -ne 0) { throw 'G4-5 has silent declaration omissions.' }
    if ($coverage.generatedCompileErrorCount -ne 0) { throw 'G4-5 has generated syntax compile errors.' }

    $partitions = [ordered]@{}
    foreach ($partition in @('Painting', 'Rendering', 'Semantics')) {
        $partitionErrors = @($aggregateErrors | Where-Object { $_ -match "[\\/]projects[\\/]$partition[\\/]" })
        $partitionWarnings = @($aggregateWarnings | Where-Object { $_ -match "[\\/]projects[\\/]$partition[\\/]" })
        $partitions[$partition.ToLowerInvariant()] = [ordered]@{
            status = if ($partitionErrors.Count -eq 0 -and $aggregateExitCode -eq 0) { 'verified' } elseif ($partitionErrors.Count -gt 0) { 'failed' } else { 'blocked' }
            warningCount = $partitionWarnings.Count
            errorCount = $partitionErrors.Count
            errorCodes = @($partitionErrors | ForEach-Object { if ($_ -match 'error (CS\d+):') { $Matches[1] } } | Group-Object | Sort-Object Count -Descending | ForEach-Object { [ordered]@{ code = $_.Name; count = $_.Count } })
        }
    }
    $artifact = [ordered]@{
        schemaVersion = 'doroti.g5-0-truth-reset/v2'
        milestone = 'G4-5'
        previousBaseline = [ordered]@{ aggregateErrorCount = 4; claimedStatus = 'stale-partial-pass' }
        currentRun = [ordered]@{
            capturedAtUtc = [DateTimeOffset]::UtcNow
            timeoutMilliseconds = 15 * 60 * 1000
            candidate = [ordered]@{
                status = if ($report.success) { 'verified' } else { 'failed' }
                generatedOutputCount = @($report.outputs).Count
                declarationCount = $coverage.declarationCount
                memberCount = $coverage.memberCount
                astNodeCount = $coverage.astNodeCount
                unclassifiedAstNodeCount = $coverage.unclassifiedAstNodeCount
                silentOmissionCount = $coverage.silentOmissionCount
                generatedCompileErrorCount = $coverage.generatedCompileErrorCount
            }
            aggregate = [ordered]@{
                status = if ($aggregateExitCode -eq 0) { 'verified' } else { 'failed' }
                warningCount = $aggregateWarnings.Count
                errorCount = $aggregateErrors.Count
                partitions = $partitions
            }
            promotion = [ordered]@{ status = 'not-verified'; reason = 'G4-5 product promotion is owned by G5-2.' }
            productBehavior = [ordered]@{ status = 'not-verified'; reason = 'Layout, paint and semantics differential behavior is owned by G5-2.' }
            nativeEvidence = [ordered]@{ status = 'not-verified'; reason = 'GPU, device-loss and automation target evidence is owned by G5-2 and later.' }
            elapsedMilliseconds = [int64]([DateTime]::UtcNow - $started).TotalMilliseconds
        }
    }
    New-Item -ItemType Directory -Force -Path (Split-Path $artifactPath) | Out-Null
    Write-AtomicJson $artifactPath $artifact

    if ($aggregateExitCode -ne 0) {
        throw "G4-5 aggregate build failed with $($aggregateErrors.Count) unique compiler error(s)."
    }
    Write-Output 'G4-5 compiler gate: PASS'
    Write-Output "Artifact: $artifactPath"
}
catch {
    if (-not (Test-Path -LiteralPath $artifactPath) -or (Get-Item -LiteralPath $artifactPath).LastWriteTimeUtc -lt $started) {
        New-Item -ItemType Directory -Force -Path (Split-Path $artifactPath) | Out-Null
        Write-AtomicJson $artifactPath ([ordered]@{
            schemaVersion = 'doroti.g5-0-truth-reset/v2'; milestone = 'G4-5'
            previousBaseline = [ordered]@{ aggregateErrorCount = 4; claimedStatus = 'stale-partial-pass' }
            currentRun = [ordered]@{
                capturedAtUtc = [DateTimeOffset]::UtcNow; timeoutMilliseconds = 15 * 60 * 1000
                candidate = [ordered]@{ status = 'failed'; failure = $_.Exception.Message }
                aggregate = [ordered]@{ status = 'blocked'; warningCount = 0; errorCount = 0; partitions = [ordered]@{} }
                promotion = [ordered]@{ status = 'not-verified' }; productBehavior = [ordered]@{ status = 'not-verified' }; nativeEvidence = [ordered]@{ status = 'not-verified' }
            }
        })
    }
    throw
}
finally {
    if ($ownsCandidate -and (Test-Path -LiteralPath $CandidateDirectory)) {
        Remove-DorotiTemporaryItem -DorotiRoot $dorotiRoot -Path $CandidateDirectory
    }
}
